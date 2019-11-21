using com.SML.Lib.Common;
using com.SML.BIGTRONS.DataAccess;
using com.SML.BIGTRONS.Enum;
using com.SML.BIGTRONS.Models;
using com.SML.BIGTRONS.ViewModels;
using Ext.Net;
using Ext.Net.MVC;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using XMVC = Ext.Net.MVC;
using System.IO;
using SW = System.Web;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace com.SML.BIGTRONS.Controllers
{
    public class ItemComparisonController : BaseController
    {
        private readonly string title = "Item Comparison";
        private readonly string dataSessionName = "ComparisonDetail";
        private readonly string dataSessionDetail = "BtnComparisonDetail";
        private readonly string dataUpdate = "DataUpdate";
        private readonly string formData = "DetailCart";
        #region Public Action

        public ActionResult Index()
        {
            //base.Initialize();
            return View();
        }
        public ActionResult List()
        {
            Global.HasAccess = GetHasAccess();
            //if (!Global.HasAccess.Read)
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            //if (Session[dataSessionName] != null)
            Session[dataSessionName] = null;
            Session[dataUpdate] = null;

            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 0;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.HomePage),
                RenderMode = RenderMode.AddTo,
                ViewName = "_List",
                WrapByScriptTag = false
            };
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            MItemComparisonDA m_objMItemComparisonDA = new MItemComparisonDA();
            m_objMItemComparisonDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMItemComparison = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMItemComparison.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = ItemComparisonVM.Prop.Map(m_strDataIndex, false);
                    List<object> m_lstFilter = new List<object>();
                    if (m_strConditionOperator != Global.OpComparation)
                    {
                        m_lstFilter.Add(Global.GetOperator(m_strConditionOperator));
                        m_lstFilter.Add(m_objValue);
                        m_objFilter.Add(m_strDataIndex, m_lstFilter);
                    }
                    else
                    {
                        object m_objStart = null;
                        object m_objEnd = null;
                        foreach (KeyValuePair<string, object> m_kvpFilterDetail in (List<KeyValuePair<string, object>>)m_objValue)
                        {
                            switch (m_kvpFilterDetail.Key)
                            {
                                case Global.OpLessThan:
                                case Global.OpLessThanEqual:
                                    m_objEnd = m_kvpFilterDetail.Value;
                                    break;
                                case Global.OpGreaterThan:
                                case Global.OpGreaterThanEqual:
                                    m_objStart = m_kvpFilterDetail.Value;
                                    break;
                            }
                        }
                        if (m_objStart != null || m_objEnd != null)
                            m_lstFilter.Add((m_objStart != null ? (m_objEnd != null ? Operator.Between
                                : Operator.GreaterThanEqual) : (m_objEnd != null ? Operator.LessThanEqual
                                : Operator.None)));
                        m_lstFilter.Add(m_objStart != null ? m_objStart : "");
                        m_lstFilter.Add(m_objEnd != null ? m_objEnd : "");
                        m_objFilter.Add(m_strDataIndex, m_lstFilter);
                    }
                }
            }
            Dictionary<int, DataSet> m_dicMItemComparisonDA = m_objMItemComparisonDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpItemComparisonBL in m_dicMItemComparisonDA)
            {
                m_intCount = m_kvpItemComparisonBL.Key;
                break;
            }

            List<ItemComparisonVM> m_lstItemComparisonVM = new List<ItemComparisonVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(ItemComparisonVM.Prop.ItemComparisonID.MapAlias);
                m_lstSelect.Add(ItemComparisonVM.Prop.ItemComparisonDesc.MapAlias);
                m_lstSelect.Add(ItemComparisonVM.Prop.UserID.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(ItemComparisonVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMItemComparisonDA = m_objMItemComparisonDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMItemComparisonDA.Message == string.Empty)
                {
                    m_lstItemComparisonVM = (
                        from DataRow m_drMItemComparisonDA in m_dicMItemComparisonDA[0].Tables[0].Rows
                        select new ItemComparisonVM()
                        {
                            ItemComparisonID = m_drMItemComparisonDA[ItemComparisonVM.Prop.ItemComparisonID.Name].ToString(),
                            ItemComparisonDesc = m_drMItemComparisonDA[ItemComparisonVM.Prop.ItemComparisonDesc.Name].ToString(),
                            UserID = m_drMItemComparisonDA[ItemComparisonVM.Prop.UserID.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstItemComparisonVM, m_intCount);
        }
        public ActionResult ReadBrowse(StoreRequestParameters parameters, string FilterInItemType)//, string FilterInItemTypeID)
        {
            DItemPriceVendorPeriodDA m_objDItemPriceVendorPeriodDA = new DItemPriceVendorPeriodDA();
            m_objDItemPriceVendorPeriodDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcDItemPriceVendorPeriod = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcDItemPriceVendorPeriod.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = ItemPriceVendorPeriodVM.Prop.Map(m_strDataIndex, false);
                    List<object> m_lstFilter = new List<object>();
                    if (m_strConditionOperator != Global.OpComparation)
                    {
                        m_lstFilter.Add(Global.GetOperator(m_strConditionOperator));
                        m_lstFilter.Add(m_objValue);
                        m_objFilter.Add(m_strDataIndex, m_lstFilter);
                    }
                    else
                    {
                        object m_objStart = null;
                        object m_objEnd = null;
                        foreach (KeyValuePair<string, object> m_kvpFilterDetail in (List<KeyValuePair<string, object>>)m_objValue)
                        {
                            switch (m_kvpFilterDetail.Key)
                            {
                                case Global.OpLessThan:
                                case Global.OpLessThanEqual:
                                    m_objEnd = m_kvpFilterDetail.Value;
                                    break;
                                case Global.OpGreaterThan:
                                case Global.OpGreaterThanEqual:
                                    m_objStart = m_kvpFilterDetail.Value;
                                    break;
                            }
                        }
                        if (m_objStart != null || m_objEnd != null)
                            m_lstFilter.Add((m_objStart != null ? (m_objEnd != null ? Operator.Between
                                : Operator.GreaterThanEqual) : (m_objEnd != null ? Operator.LessThanEqual
                                : Operator.None)));
                        m_lstFilter.Add(m_objStart != null ? m_objStart : "");
                        m_lstFilter.Add(m_objEnd != null ? m_objEnd : "");
                        m_objFilter.Add(m_strDataIndex, m_lstFilter);
                    }
                }
            }

            if (FilterInItemType.Length > 0)
            {
                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.In);
                m_lstFilter.Add(FilterInItemType);
                m_objFilter.Add(ItemVM.Prop.ItemTypeID.Map, m_lstFilter);
            }

            //if (FilterInItemTypeID.Length > 0)
            //{
            //    FilterInItemTypeID = JSON.Deserialize<string>(FilterInItemTypeID).Replace("\"", "");

            //    List<object> m_lstFilter = new List<object>();
            //    m_lstFilter.Add(Operator.In);
            //    m_lstFilter.Add(FilterInItemTypeID);
            //    m_objFilter.Add(ItemVM.Prop.ItemTypeID.Map, m_lstFilter);
            //}

            List<string> m_lstSelects = new List<string>();
            m_lstSelects.Add(ItemPriceVendorPeriodVM.Prop.ItemID.MapAlias);
            m_lstSelects.Add(ItemPriceVendorPeriodVM.Prop.ItemDesc.MapAlias);
            m_lstSelects.Add(ItemPriceVendorPeriodVM.Prop.VendorDesc.MapAlias);
            m_lstSelects.Add(ItemPriceVendorPeriodVM.Prop.VendorID.MapAlias);
            m_lstSelects.Add(ItemPriceVendorPeriodVM.Prop.Amount.MapAlias);
            m_lstSelects.Add(ItemPriceVendorPeriodVM.Prop.ItemGroupDesc.MapAlias);


            Dictionary<int, DataSet> m_dicDItemPriceVendorPeriodDA = m_objDItemPriceVendorPeriodDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelects, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpItemPriceVendorPeriodBL in m_dicDItemPriceVendorPeriodDA)
            {
                m_intCount = m_kvpItemPriceVendorPeriodBL.Key;
                break;
            }

            List<ItemPriceVendorPeriodVM> m_lstItemPriceVendorPeriodVM = new List<ItemPriceVendorPeriodVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelects.Add(ItemPriceVendorPeriodVM.Prop.ItemID.MapAlias);
                m_lstSelects.Add(ItemPriceVendorPeriodVM.Prop.ItemDesc.MapAlias);
                m_lstSelects.Add(ItemPriceVendorPeriodVM.Prop.VendorDesc.MapAlias);
                m_lstSelects.Add(ItemPriceVendorPeriodVM.Prop.VendorID.MapAlias);
                m_lstSelects.Add(ItemPriceVendorPeriodVM.Prop.Amount.MapAlias);
                m_lstSelects.Add(ItemPriceVendorPeriodVM.Prop.ItemGroupDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(ItemPriceVendorPeriodVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicDItemPriceVendorPeriodDA = m_objDItemPriceVendorPeriodDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objDItemPriceVendorPeriodDA.Message == string.Empty)
                {
                    m_lstItemPriceVendorPeriodVM = (
                        from DataRow m_drDItemPriceVendorPeriodDA in m_dicDItemPriceVendorPeriodDA[0].Tables[0].Rows
                        select new ItemPriceVendorPeriodVM()
                        {
                            ItemID = m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.ItemID.Name].ToString(),
                            ItemDesc = m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.ItemDesc.Name].ToString(),
                            VendorDesc = m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.VendorDesc.Name].ToString(),
                            VendorID = m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.VendorID.Name].ToString(),
                            Amount = (decimal)m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.Amount.Name],
                            ItemGroupDesc = m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.ItemGroupDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstItemPriceVendorPeriodVM, m_intCount);
        }
        public ActionResult Vendor(StoreRequestParameters parameters)
        {
            try
            {
                MVendorDA m_objMVendorDA = new MVendorDA();
                m_objMVendorDA.ConnectionStringName = Global.ConnStrConfigName;

                int m_intSkip = parameters.Start;
                int m_intLength = parameters.Limit;
                bool m_boolIsCount = true;

                FilterHeaderConditions m_fhcMVendor = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
                Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

                foreach (FilterHeaderCondition m_fhcFilter in m_fhcMVendor.Conditions)
                {
                    string m_strDataIndex = m_fhcFilter.DataIndex;
                    string m_strConditionOperator = m_fhcFilter.Operator;
                    object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                    if (m_strDataIndex != string.Empty)
                    {
                        m_strDataIndex = VendorVM.Prop.Map(m_strDataIndex, false);
                        List<object> m_lstFilter = new List<object>();
                        if (m_strConditionOperator != Global.OpComparation)
                        {
                            m_lstFilter.Add(Global.GetOperator(m_strConditionOperator));
                            m_lstFilter.Add(m_objValue);
                            m_objFilter.Add(m_strDataIndex, m_lstFilter);
                        }
                        else
                        {
                            object m_objStart = null;
                            object m_objEnd = null;
                            foreach (KeyValuePair<string, object> m_kvpFilterDetail in (List<KeyValuePair<string, object>>)m_objValue)
                            {
                                switch (m_kvpFilterDetail.Key)
                                {
                                    case Global.OpLessThan:
                                    case Global.OpLessThanEqual:
                                        m_objEnd = m_kvpFilterDetail.Value;
                                        break;
                                    case Global.OpGreaterThan:
                                    case Global.OpGreaterThanEqual:
                                        m_objStart = m_kvpFilterDetail.Value;
                                        break;
                                }
                            }
                            if (m_objStart != null || m_objEnd != null)
                                m_lstFilter.Add((m_objStart != null ? (m_objEnd != null ? Operator.Between
                                    : Operator.GreaterThanEqual) : (m_objEnd != null ? Operator.LessThanEqual
                                    : Operator.None)));
                            m_lstFilter.Add(m_objStart != null ? m_objStart : "");
                            m_lstFilter.Add(m_objEnd != null ? m_objEnd : "");
                            m_objFilter.Add(m_strDataIndex, m_lstFilter);
                        }
                    }
                }
                Dictionary<int, DataSet> m_dicMVendorDA = m_objMVendorDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
                int m_intCount = 0;

                foreach (KeyValuePair<int, DataSet> m_kvpVendorBL in m_dicMVendorDA)
                {
                    m_intCount = m_kvpVendorBL.Key;
                    break;
                }

                List<VendorVM> m_lstVendorVM = new List<VendorVM>();
                if (m_intCount > 0)
                {
                    m_boolIsCount = false;
                    List<string> m_lstSelect = new List<string>();
                    m_lstSelect.Add(VendorVM.Prop.VendorID.MapAlias);
                    m_lstSelect.Add(VendorVM.Prop.VendorDesc.MapAlias);
                    m_lstSelect.Add(VendorVM.Prop.VendorCategoryDesc.MapAlias);
                    m_lstSelect.Add(VendorVM.Prop.VendorSubcategoryDesc.MapAlias);
                    m_lstSelect.Add(VendorVM.Prop.Address.MapAlias);
                    m_lstSelect.Add(VendorVM.Prop.Phone.MapAlias);
                    m_lstSelect.Add(VendorVM.Prop.Email.MapAlias);

                    Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                    foreach (DataSorter m_dtsOrder in parameters.Sort)
                        m_dicOrder.Add(VendorVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                    m_dicMVendorDA = m_objMVendorDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                    if (m_objMVendorDA.Message == string.Empty)
                    {
                        m_lstVendorVM = (
                            from DataRow m_drMVendorDA in m_dicMVendorDA[0].Tables[0].Rows
                            select new VendorVM()
                            {
                                VendorID = m_drMVendorDA[VendorVM.Prop.VendorID.Name].ToString(),
                                VendorDesc = m_drMVendorDA[VendorVM.Prop.VendorDesc.Name].ToString(),
                                VendorCategoryDesc = m_drMVendorDA[VendorVM.Prop.VendorCategoryDesc.Name].ToString(),
                                VendorSubcategoryDesc = m_drMVendorDA[VendorVM.Prop.VendorSubcategoryDesc.Name].ToString(),
                                Address = m_drMVendorDA[VendorVM.Prop.Address.Name].ToString(),
                                Phone = m_drMVendorDA[VendorVM.Prop.Phone.Name].ToString(),
                                Email = m_drMVendorDA[VendorVM.Prop.Email.Name].ToString(),
                            }
                        ).ToList();
                    }
                }

                ItemComparisonVM mItemComparasionVM = new ItemComparisonVM();
                mItemComparasionVM.m_vendorVM = new VendorVM();
                //string test = mItemComparasionVM.m_vendorVM.VendorID.Name.ToString();

                return this.Store(m_lstVendorVM, m_intCount);
            }
            catch (Exception ex)
            {
                return this.Store("Error Get Vendor : " + ex.Message);
            }
        }
        public ActionResult ReadBrowseChild(StoreRequestParameters parameters, string SelectedRow_)
        {
            List<ItemPriceVendorPeriodVM> m_lstItemPriceVendorPeriodVM = new List<ItemPriceVendorPeriodVM>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            string ItemType = "";
            string FilterInItemID = "";
            string version_ = "";
            string childType_ = "";
            string[] Selecteds = JSON.Deserialize<string[]>(SelectedRow_);
            if (SelectedRow_ != "null")
            {
                ItemType = Selecteds[0].ToString();
                FilterInItemID = Selecteds[1].ToString();
                version_ = Selecteds[2].ToString();
                childType_ = Selecteds[3].ToString();
            }
            else
            {
                SelectedRow_ = string.Empty;
            }

            #region Get uConfig
            List<string> m_lstUPA = new List<string>();
            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;

            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilteru = new Dictionary<string, List<object>>();

            List<string> m_lstSelectb = new List<string>();
            m_lstSelectb.Add(ConfigVM.Prop.Key3.MapAlias);

            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("ItemTypeID");
            m_objFilteru.Add(ConfigVM.Prop.Key1.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("BudgetPlanTemplate");
            m_objFilteru.Add(ConfigVM.Prop.Key2.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicUConfigDA = m_objUConfigDA.SelectBC(0, null, false, m_lstSelectb, m_objFilteru, null, null, null);
            List<ConfigVM> m_lstConfigVM = new List<ViewModels.ConfigVM>();
            if (m_objUConfigDA.Message == string.Empty)
            {
                m_lstConfigVM = (
                        from DataRow m_drUConfigDA in m_dicUConfigDA[0].Tables[0].Rows
                        select new ConfigVM()
                        {
                            Key3 = m_drUConfigDA[ConfigVM.Prop.Key3.Name].ToString()
                        }
                    ).ToList();
            }
            foreach (ConfigVM item in m_lstConfigVM)
            {
                m_lstUPA.Add(item.Key3);
            }
            #endregion

            #region GetItemSelected
            DItemPriceVendorPeriodDA m_objDItemPriceVendorPeriodDA = new DItemPriceVendorPeriodDA();
            m_objDItemPriceVendorPeriodDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int? m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcDItemPriceVendorPeriod = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcDItemPriceVendorPeriod.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = ItemPriceVendorPeriodVM.Prop.Map(m_strDataIndex, false);
                    m_lstFilter = new List<object>();
                    if (m_strConditionOperator != Global.OpComparation)
                    {
                        m_lstFilter.Add(Global.GetOperator(m_strConditionOperator));
                        m_lstFilter.Add(m_objValue);
                        m_objFilter.Add(m_strDataIndex, m_lstFilter);
                    }
                    else
                    {
                        object m_objStart = null;
                        object m_objEnd = null;
                        foreach (KeyValuePair<string, object> m_kvpFilterDetail in (List<KeyValuePair<string, object>>)m_objValue)
                        {
                            switch (m_kvpFilterDetail.Key)
                            {
                                case Global.OpLessThan:
                                case Global.OpLessThanEqual:
                                    m_objEnd = m_kvpFilterDetail.Value;
                                    break;
                                case Global.OpGreaterThan:
                                case Global.OpGreaterThanEqual:
                                    m_objStart = m_kvpFilterDetail.Value;
                                    break;
                            }
                        }
                        if (m_objStart != null || m_objEnd != null)
                            m_lstFilter.Add((m_objStart != null ? (m_objEnd != null ? Operator.Between
                                : Operator.GreaterThanEqual) : (m_objEnd != null ? Operator.LessThanEqual
                                : Operator.None)));
                        m_lstFilter.Add(m_objStart != null ? m_objStart : "");
                        m_lstFilter.Add(m_objEnd != null ? m_objEnd : "");
                        m_objFilter.Add(m_strDataIndex, m_lstFilter);
                    }
                }
            }
            List<string> anyBOI = new List<string>();

            if (SelectedRow_.Length > 0)
            {

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.None);
                m_lstFilter.Add(string.Empty);
                if (string.IsNullOrEmpty(childType_))
                {
                    m_objFilter.Add(String.Format("({0} = '{1}' OR {2} = '{1}') AND {0} IN ('{3}')", ItemPriceVendorPeriodVM.Prop.ItemTypeID.Map, ItemType, String.Join("','", m_lstUPA)), m_lstFilter);
                }
                else
                {
                    m_objFilter.Add(String.Format("({0} = '{1}' OR {2} = '{1}') AND {0}='{4}' AND {0} IN ('{3}')", ItemPriceVendorPeriodVM.Prop.ItemTypeID.Map, ItemType, String.Join("','", m_lstUPA), childType_), m_lstFilter);
                }
            }
            else
            {
                if (m_objFilter.Count == 0)
                {
                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.In);
                    m_lstFilter.Add(String.Join(",", m_lstUPA));
                    m_objFilter.Add(ItemPriceVendorPeriodVM.Prop.ItemTypeID.Map, m_lstFilter);
                }
            }
            List<string> m_lstSelects = new List<string>();
            m_lstSelects.Add(ItemPriceVendorPeriodVM.Prop.ItemID.MapAlias);
            m_lstSelects.Add(ItemPriceVendorPeriodVM.Prop.ItemDesc.MapAlias);
            m_lstSelects.Add(ItemPriceVendorPeriodVM.Prop.VendorDesc.MapAlias);
            m_lstSelects.Add(ItemPriceVendorPeriodVM.Prop.VendorID.MapAlias);
            m_lstSelects.Add(ItemPriceVendorPeriodVM.Prop.Amount.MapAlias);
            m_lstSelects.Add(ItemPriceVendorPeriodVM.Prop.ItemGroupDesc.MapAlias);

            Dictionary<int, DataSet> m_dicDItemPriceVendorPeriodDA = m_objDItemPriceVendorPeriodDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelects, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpItemPriceVendorPeriodBL in m_dicDItemPriceVendorPeriodDA)
            {
                m_intCount = m_kvpItemPriceVendorPeriodBL.Key;
                break;
            }

            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelects.Add(ItemPriceVendorPeriodVM.Prop.ItemID.MapAlias);
                m_lstSelects.Add(ItemPriceVendorPeriodVM.Prop.ItemDesc.MapAlias);
                m_lstSelects.Add(ItemPriceVendorPeriodVM.Prop.VendorDesc.MapAlias);
                m_lstSelects.Add(ItemPriceVendorPeriodVM.Prop.VendorID.MapAlias);
                m_lstSelects.Add(ItemPriceVendorPeriodVM.Prop.Amount.MapAlias);
                m_lstSelects.Add(ItemPriceVendorPeriodVM.Prop.ItemGroupDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(ItemPriceVendorPeriodVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicDItemPriceVendorPeriodDA = m_objDItemPriceVendorPeriodDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objDItemPriceVendorPeriodDA.Message == string.Empty)
                {
                    m_lstItemPriceVendorPeriodVM = (
                        from DataRow m_drDItemPriceVendorPeriodDA in m_dicDItemPriceVendorPeriodDA[0].Tables[0].Rows
                        select new ItemPriceVendorPeriodVM()
                        {
                            ItemID = m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.ItemID.Name].ToString(),
                            ItemDesc = m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.ItemDesc.Name].ToString(),
                            VendorDesc = m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.VendorDesc.Name].ToString(),
                            VendorID = m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.VendorID.Name].ToString(),
                            Amount = (decimal)m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.Amount.Name],
                            ItemGroupDesc = m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.ItemGroupDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            #endregion
            return this.Store(m_lstItemPriceVendorPeriodVM, m_intCount);
        }
        public ActionResult ReloadParameter(string[] args, string RegionID, string ProjectID, string ClusterID, string UnitTypeID)
        {
            return this.Store(GetListItemPriceVendorPeriod(args, RegionID, ProjectID, ClusterID, UnitTypeID));
        }
        private List<ItemPriceVendorPeriodVM> GetListItemPriceVendorPeriod(string[] ItemID, string RegionID, string ProjectID, string ClusterID, string UnitTypeID)//(string ItemID)
        {
            List<ItemPriceVendorPeriodVM> m_lstItemPriceVendorPeriod = new List<ItemPriceVendorPeriodVM>();
            DItemPriceVendorPeriodDA m_objDItemPriceVendorPeriodDA = new DItemPriceVendorPeriodDA();
            m_objDItemPriceVendorPeriodDA.ConnectionStringName = Global.ConnStrConfigName;

            //var x = JSON.Deserialize<List<string>>( this.Request.Params["ItemID"]);

            List<string> m_lstSelectedRow = new List<string>();
            m_lstSelectedRow = JSON.Deserialize<List<string>>(this.Request.Params["ItemID"]);

            List<string> m_lstMessage = new List<string>();
            if (!m_lstSelectedRow.Any())
                m_lstMessage.Add("Some of Item Can't be Found");

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.ItemPriceID.MapAlias);
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.ValidFrom.MapAlias);
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.ValidTo.MapAlias);
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.CurrencyID.MapAlias);
            m_lstSelect.Add(ItemPriceVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(VendorVM.Prop.VendorDesc.MapAlias);
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.Amount.MapAlias);
            //m_lstSelect.Add(ItemPriceVendorVM.Prop.IsDefault.MapAlias);
            //m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.VendorID.MapAlias);
            //m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.Amount.MapAlias);
            //m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.CurrencyDesc.MapAlias);
            //m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.VendorDesc.MapAlias);
            //m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.ItemID.MapAlias);
            //m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.ItemDesc.MapAlias);
            //m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.IsDefault.MapAlias);
            //m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.RegionID.MapAlias);
            //m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.ProjectID.MapAlias);
            //m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.ClusterID.MapAlias);
            //m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.UnitTypeID.MapAlias);
            //m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.DisplayPrice.MapAlias);
            //m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.RowNo.MapAlias);

            if (this.Request.Params["ItemID"] != null)
            {
                //foreach (string listItemPriceVendorPeriod in m_lstSelectedRow)
                //{
                Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.In);
                m_lstFilter.Add(string.Join(",", m_lstSelectedRow));
                m_objFilter.Add(ItemPriceVM.Prop.ItemID.Map, m_lstFilter);

                //m_lstFilter = new List<object>();
                //m_lstFilter.Add(Operator.GreaterThanEqual);
                //m_lstFilter.Add(DateTime.Now.Date.ToString(Global.SqlDateFormat));
                //m_objFilter.Add(ItemPriceVendorPeriodVM.Prop.ValidTo.Map, m_lstFilter);

                // ==================================================
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);

                if (RegionID == null)
                    RegionID = string.Empty;

                m_lstFilter.Add(RegionID);
                m_objFilter.Add(ItemPriceVM.Prop.RegionID.Map, m_lstFilter);
                //}

                if (!string.IsNullOrEmpty(ProjectID))
                {
                    if (!string.IsNullOrEmpty(ClusterID))
                    {
                        if (!string.IsNullOrEmpty(UnitTypeID))
                        {
                            m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(UnitTypeID);
                            m_objFilter.Add(ItemPriceVM.Prop.UnitTypeID.Map, m_lstFilter);
                        }

                        m_lstFilter = new List<object>();
                        m_lstFilter.Add(Operator.Equals);
                        m_lstFilter.Add(ClusterID);
                        m_objFilter.Add(ItemPriceVM.Prop.ClusterID.Map, m_lstFilter);
                    }
                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(ProjectID);
                    m_objFilter.Add(ItemPriceVM.Prop.ProjectID.Map, m_lstFilter);
                }

                // ==================================================

                List<string> m_lstGroupBy = new List<string>();
                m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.ItemPriceID.Map);
                m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.ValidFrom.Map);
                m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.ValidTo.Map);
                m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.CurrencyID.Map);
                m_lstSelect.Add(ItemPriceVM.Prop.ItemID.Map);
                m_lstSelect.Add(ItemVM.Prop.ItemDesc.Map);
                m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.VendorID.Map);
                m_lstSelect.Add(VendorVM.Prop.VendorDesc.Map);
                m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.Amount.Map);

                Dictionary<int, DataSet> m_dicDItemPriceVendorPeriodDA = m_objDItemPriceVendorPeriodDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, m_lstGroupBy, null, null);
                if (m_objDItemPriceVendorPeriodDA.Success)
                {
                    m_lstItemPriceVendorPeriod = (
                        from DataRow m_drm_dicItemPriceVendorPeriod in m_dicDItemPriceVendorPeriodDA[0].Tables[0].Rows
                        select new ItemPriceVendorPeriodVM()
                        {
                            ItemPriceID = m_drm_dicItemPriceVendorPeriod[ItemPriceVendorPeriodVM.Prop.ItemPriceID.Name].ToString(),
                            //VendorID = m_drm_dicItemPriceVendorPeriod[ItemPriceVendorPeriodVM.Prop.VendorID.Name].ToString(),
                            ValidFrom = DateTime.Parse(m_drm_dicItemPriceVendorPeriod[ItemPriceVendorPeriodVM.Prop.ValidFrom.Name].ToString()),
                            ValidTo = DateTime.Parse(m_drm_dicItemPriceVendorPeriod[ItemPriceVendorPeriodVM.Prop.ValidTo.Name].ToString()),
                            CurrencyID = m_drm_dicItemPriceVendorPeriod[ItemPriceVendorPeriodVM.Prop.CurrencyID.Name].ToString(),
                            //Amount = (decimal)m_drm_dicItemPriceVendorPeriod[ItemPriceVendorPeriodVM.Prop.Amount.Name],

                            ItemID = m_drm_dicItemPriceVendorPeriod[ItemVM.Prop.ItemID.Name].ToString(),
                            ItemDesc = m_drm_dicItemPriceVendorPeriod[ItemVM.Prop.ItemDesc.Name].ToString(),
                            VendorDesc = (m_drm_dicItemPriceVendorPeriod[ItemPriceVendorVM.Prop.VendorDesc.Name].ToString() == string.Empty ? m_drm_dicItemPriceVendorPeriod[ConfigVM.Prop.Desc2.Name].ToString() : m_drm_dicItemPriceVendorPeriod[ItemPriceVendorVM.Prop.VendorDesc.Name].ToString()),
                            Amount = Convert.ToDecimal(m_drm_dicItemPriceVendorPeriod[ItemPriceVendorPeriodVM.Prop.Amount.Name].ToString()),
                            VendorID = m_drm_dicItemPriceVendorPeriod[ItemPriceVendorPeriodVM.Prop.VendorID.Name].ToString(),


                            //CurrencyDesc = m_drm_dicItemPriceVendorPeriod[ItemPriceVendorPeriodVM.Prop.CurrencyDesc.Name].ToString(),
                            //VendorDesc = m_drm_dicItemPriceVendorPeriod[ItemPriceVendorPeriodVM.Prop.VendorDesc.Name].ToString(),
                            //ItemID = m_drm_dicItemPriceVendorPeriod[ItemPriceVendorPeriodVM.Prop.ItemID.Name].ToString(),
                            //ItemDesc = m_drm_dicItemPriceVendorPeriod[ItemPriceVendorPeriodVM.Prop.ItemDesc.Name].ToString(),
                            //IsDefault = Convert.ToBoolean(m_drm_dicItemPriceVendorPeriod[ItemPriceVendorPeriodVM.Prop.IsDefault.Name].ToString()),
                            //RegionID = m_drm_dicItemPriceVendorPeriod[ItemPriceVendorPeriodVM.Prop.RegionID.Name].ToString(),
                            //ProjectID = m_drm_dicItemPriceVendorPeriod[ItemPriceVendorPeriodVM.Prop.ProjectID.Name].ToString(),
                            //ClusterID = m_drm_dicItemPriceVendorPeriod[ItemPriceVendorPeriodVM.Prop.ClusterID.Name].ToString(),
                            //UnitTypeID = m_drm_dicItemPriceVendorPeriod[ItemPriceVendorPeriodVM.Prop.UnitTypeID.Name].ToString(),
                            //DisplayPrice = Convert.ToBoolean(m_drm_dicItemPriceVendorPeriod[ItemPriceVendorPeriodVM.Prop.DisplayPrice.Name].ToString()),
                            //RowNo = int.Parse(m_drm_dicItemPriceVendorPeriod[ItemPriceVendorPeriodVM.Prop.RowNo.Name].ToString()),

                        }).Distinct().ToList();
                }

                //}
            }
            return m_lstItemPriceVendorPeriod;
        }
        public ActionResult ReadBrowseItemPriceVendorPeriod(StoreRequestParameters parameters, string ItemID, string RegionID, string ProjectID, string ClusterID, string UnitTypeID)
        {
            string m_strMenuObject = GetMenuObject("DisplayPriceBrowse");
            bool m_boolValueObject = Convert.ToBoolean(string.IsNullOrEmpty(m_strMenuObject) ? "True" : m_strMenuObject);
            DItemPriceVendorPeriodDA m_objDItemPriceVendorPeriodDA = new DItemPriceVendorPeriodDA();
            m_objDItemPriceVendorPeriodDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMItem = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<string> m_lstSelect = new List<string>();

            m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMItem.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = ItemVM.Prop.Map(m_strDataIndex, false);
                    m_lstFilter = new List<object>();
                    if (m_strConditionOperator != Global.OpComparation)
                    {
                        m_lstFilter.Add(Global.GetOperator(m_strConditionOperator));
                        m_lstFilter.Add(m_objValue);
                        m_objFilter.Add(m_strDataIndex, m_lstFilter);
                    }
                    else
                    {
                        object m_objStart = null;
                        object m_objEnd = null;
                        foreach (KeyValuePair<string, object> m_kvpFilterDetail in (List<KeyValuePair<string, object>>)m_objValue)
                        {
                            switch (m_kvpFilterDetail.Key)
                            {
                                case Global.OpLessThan:
                                case Global.OpLessThanEqual:
                                    m_objEnd = m_kvpFilterDetail.Value;
                                    break;
                                case Global.OpGreaterThan:
                                case Global.OpGreaterThanEqual:
                                    m_objStart = m_kvpFilterDetail.Value;
                                    break;
                            }
                        }
                        if (m_objStart != null || m_objEnd != null)
                            m_lstFilter.Add((m_objStart != null ? (m_objEnd != null ? Operator.Between
                                : Operator.GreaterThanEqual) : (m_objEnd != null ? Operator.LessThanEqual
                                : Operator.None)));
                        m_lstFilter.Add(m_objStart != null ? m_objStart : "");
                        m_lstFilter.Add(m_objEnd != null ? m_objEnd : "");
                        m_objFilter.Add(m_strDataIndex, m_lstFilter);
                    }
                }
            }

            if (!string.IsNullOrEmpty(ItemID))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(ItemID);
                m_objFilter.Add(ItemPriceVM.Prop.ItemID.Map, m_lstFilter);
            }

            //m_lstFilter = new List<object>();
            //m_lstFilter.Add(Operator.None);
            //m_lstFilter.Add(string.Empty);
            //m_objFilter.Add(GetFilterPrice(new ItemPriceVM { RegionID = RegionID, ProjectID = ProjectID, ClusterID = ClusterID, UnitTypeID = UnitTypeID }), m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.GreaterThanEqual);
            m_lstFilter.Add(DateTime.Now.Date.ToString(Global.SqlDateFormat));
            m_objFilter.Add(ItemPriceVendorPeriodVM.Prop.ValidTo.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.None);
            m_lstFilter.Add("");
            m_objFilter.Add("(DItemPriceVendorPeriod.VendorID <> '')", m_lstFilter);

            // ==================================================
            // ==================================================
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);

            if (RegionID == null)
                RegionID = string.Empty;

            m_lstFilter.Add(RegionID);
            m_objFilter.Add(RegionVM.Prop.RegionID.Map, m_lstFilter);

            if (!string.IsNullOrEmpty(ProjectID))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(ProjectID);
                m_objFilter.Add(ProjectVM.Prop.ProjectID.Map, m_lstFilter);

                if (!string.IsNullOrEmpty(ClusterID))
                {
                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(ClusterID);
                    m_objFilter.Add(ClusterVM.Prop.ClusterID.Map, m_lstFilter);
                }
            }

            if (!string.IsNullOrEmpty(UnitTypeID))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(UnitTypeID);
                m_objFilter.Add(ItemPriceVM.Prop.UnitTypeID.Map, m_lstFilter);
            }
            // ==================================================
            // ==================================================




            // ========================================= build select query string ===================//
            m_boolIsCount = false;
            m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemPriceVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.ItemPriceID.MapAlias);
            m_lstSelect.Add(VendorVM.Prop.VendorDesc.MapAlias);
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.Amount.MapAlias);
            m_lstSelect.Add(ItemPriceVendorVM.Prop.IsDefault.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc1.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc2.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc3.MapAlias);
            // ========================================= build select query string ===================//

            Dictionary<int, DataSet> m_dicDItemPriceVendorPeriodDA = m_objDItemPriceVendorPeriodDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            //foreach (KeyValuePair<int, DataSet> m_kvpItemPriceVendorPeriodDA in m_dicDItemPriceVendorPeriodDA)
            //{
            //    m_intCount = m_kvpItemPriceVendorPeriodDA.Key;
            //    break;
            //}

            try
            {
                m_intCount = m_dicDItemPriceVendorPeriodDA[0].Tables[0].Rows.Count;
            }
            catch
            {
                m_intCount = 0;
            }

            List<ItemPriceVendorPeriodVM> m_lstItemPriceVendorPeriodVM = new List<ItemPriceVendorPeriodVM>();
            if (m_intCount > 0)
            {

                m_boolIsCount = false;

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(ItemPriceVendorPeriodVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicDItemPriceVendorPeriodDA = m_objDItemPriceVendorPeriodDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, null, null);
                if (m_objDItemPriceVendorPeriodDA.Message == string.Empty && m_objDItemPriceVendorPeriodDA.Success)
                {
                    m_lstItemPriceVendorPeriodVM = (
                        from DataRow m_drDItemPriceVendorPeriodDA in m_dicDItemPriceVendorPeriodDA[0].Tables[0].Rows
                        select new ItemPriceVendorPeriodVM()
                        {
                            ItemID = m_drDItemPriceVendorPeriodDA[ItemVM.Prop.ItemID.Name].ToString(),
                            ItemDesc = m_drDItemPriceVendorPeriodDA[ItemVM.Prop.ItemDesc.Name].ToString(),
                            ItemPriceID = m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.ItemPriceID.Name].ToString(),
                            VendorDesc = (m_drDItemPriceVendorPeriodDA[ItemPriceVendorVM.Prop.VendorDesc.Name].ToString() == string.Empty ? m_drDItemPriceVendorPeriodDA[ConfigVM.Prop.Desc2.Name].ToString() : m_drDItemPriceVendorPeriodDA[ItemPriceVendorVM.Prop.VendorDesc.Name].ToString()),
                            Amount = Convert.ToDecimal(m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.Amount.Name].ToString()),
                            VendorID = m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.VendorID.Name].ToString(),
                            IsDefault = Convert.ToBoolean(m_drDItemPriceVendorPeriodDA[ItemPriceVendorVM.Prop.IsDefault.Name].ToString()),
                            DisplayPrice = m_boolValueObject
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstItemPriceVendorPeriodVM, m_intCount);
        }
        public ActionResult ReadBrowseVendorCompare(StoreRequestParameters parameters, string ItemID, string RegionID, string ProjectID, string ClusterID, string UnitTypeID)
        {
            string m_strMenuObject = GetMenuObject("DisplayPriceBrowse");
            bool m_boolValueObject = Convert.ToBoolean(string.IsNullOrEmpty(m_strMenuObject) ? "True" : m_strMenuObject);
            DItemPriceVendorPeriodDA m_objDItemPriceVendorPeriodDA = new DItemPriceVendorPeriodDA();
            m_objDItemPriceVendorPeriodDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMItem = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<string> m_lstSelect = new List<string>();

            m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMItem.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = ItemVM.Prop.Map(m_strDataIndex, false);
                    m_lstFilter = new List<object>();
                    if (m_strConditionOperator != Global.OpComparation)
                    {
                        m_lstFilter.Add(Global.GetOperator(m_strConditionOperator));
                        m_lstFilter.Add(m_objValue);
                        m_objFilter.Add(m_strDataIndex, m_lstFilter);
                    }
                    else
                    {
                        object m_objStart = null;
                        object m_objEnd = null;
                        foreach (KeyValuePair<string, object> m_kvpFilterDetail in (List<KeyValuePair<string, object>>)m_objValue)
                        {
                            switch (m_kvpFilterDetail.Key)
                            {
                                case Global.OpLessThan:
                                case Global.OpLessThanEqual:
                                    m_objEnd = m_kvpFilterDetail.Value;
                                    break;
                                case Global.OpGreaterThan:
                                case Global.OpGreaterThanEqual:
                                    m_objStart = m_kvpFilterDetail.Value;
                                    break;
                            }
                        }
                        if (m_objStart != null || m_objEnd != null)
                            m_lstFilter.Add((m_objStart != null ? (m_objEnd != null ? Operator.Between
                                : Operator.GreaterThanEqual) : (m_objEnd != null ? Operator.LessThanEqual
                                : Operator.None)));
                        m_lstFilter.Add(m_objStart != null ? m_objStart : "");
                        m_lstFilter.Add(m_objEnd != null ? m_objEnd : "");
                        m_objFilter.Add(m_strDataIndex, m_lstFilter);
                    }
                }
            }

            if (!string.IsNullOrEmpty(ItemID))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(ItemID);
                m_objFilter.Add(ItemPriceVM.Prop.ItemID.Map, m_lstFilter);
            }

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.LessThan);

            m_lstFilter.Add(DateTime.Now.Date.ToString(Global.SqlDateFormat));
            m_objFilter.Add(ItemPriceVendorPeriodVM.Prop.ValidFrom.Map, m_lstFilter);


            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.NotEqual);
            //m_lstFilter.Add(Operator.LessThanEqual);
            m_lstFilter.Add("");
            m_objFilter.Add(ItemPriceVendorPeriodVM.Prop.VendorID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicDItemPriceVendorPeriodDA = m_objDItemPriceVendorPeriodDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);

            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpItemPriceVendorPeriodDA in m_dicDItemPriceVendorPeriodDA)
            {
                m_intCount = m_kvpItemPriceVendorPeriodDA.Key;
                break;
            }

            List<ItemPriceVendorPeriodVM> m_lstItemPriceVendorPeriodVM = new List<ItemPriceVendorPeriodVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                m_lstSelect = new List<string>();
                m_lstSelect.Add(ItemPriceVM.Prop.ItemID.MapAlias);
                m_lstSelect.Add(ItemVM.Prop.ItemDesc.MapAlias);
                m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.VendorID.MapAlias);
                m_lstSelect.Add(VendorVM.Prop.VendorDesc.MapAlias);
                m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.Amount.MapAlias);
                m_lstSelect.Add(ItemPriceVendorVM.Prop.IsDefault.MapAlias);
                m_lstSelect.Add(ConfigVM.Prop.Desc1.MapAlias);
                m_lstSelect.Add(ConfigVM.Prop.Desc2.MapAlias);
                m_lstSelect.Add(ConfigVM.Prop.Desc3.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(ItemPriceVendorPeriodVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicDItemPriceVendorPeriodDA = m_objDItemPriceVendorPeriodDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, null, null);
                if (m_objDItemPriceVendorPeriodDA.Message == string.Empty && m_objDItemPriceVendorPeriodDA.Success)
                {
                    m_lstItemPriceVendorPeriodVM = (
                        from DataRow m_drDItemPriceVendorPeriodDA in m_dicDItemPriceVendorPeriodDA[0].Tables[0].Rows
                        select new ItemPriceVendorPeriodVM()
                        {
                            //ItemID = m_drDItemPriceVendorPeriodDA[ItemVM.Prop.ItemID.Name].ToString(),
                            //ItemDesc = m_drDItemPriceVendorPeriodDA[ItemVM.Prop.ItemDesc.Name].ToString(),
                            //VendorDesc = (m_drDItemPriceVendorPeriodDA[ItemPriceVendorVM.Prop.VendorDesc.Name].ToString() == string.Empty ? m_drDItemPriceVendorPeriodDA[ConfigVM.Prop.Desc2.Name].ToString() : m_drDItemPriceVendorPeriodDA[ItemPriceVendorVM.Prop.VendorDesc.Name].ToString()),
                            VendorID = m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.VendorID.Name].ToString(),
                            Amount = Convert.ToDecimal(m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.Amount.Name].ToString()),
                            //IsDefault = Convert.ToBoolean(m_drDItemPriceVendorPeriodDA[ItemPriceVendorVM.Prop.IsDefault.Name].ToString()),
                            //DisplayPrice = m_boolValueObject
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstItemPriceVendorPeriodVM, m_intCount);
        }
        public ActionResult Home()
        {
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 0;
            return this.Direct();
        }
        public ActionResult Add(string Caller, string Selected, string ItemComparisonVersion)
        {
            Global.HasAccess = GetHasAccess();
            //if (!Global.HasAccess.Add)
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ItemComparisonVM m_objItemComparisonVM = new ItemComparisonVM();
            m_objItemComparisonVM.ListItemComparisonDetailsVM = new List<ItemComparisonDetailsVM>();

            ItemComparisonDetailsVM m_objItemComparisonDetailsVM_ = new ItemComparisonDetailsVM();
            m_objItemComparisonDetailsVM_.ItemComparisonDetailID = " ";
            m_objItemComparisonDetailsVM_.ItemComparisonID = "empty";
            m_objItemComparisonVM.ListItemComparisonDetailsVM.Add(m_objItemComparisonDetailsVM_);

            m_objItemComparisonVM.ListVendorVM = new List<VendorVM>();

            ViewDataDictionary m_vddItemComparison = new ViewDataDictionary();
            m_vddItemComparison.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddItemComparison.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            

            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objItemComparisonVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddItemComparison,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult Checkout(string Caller, string Selected)
        {
            //Global.HasAccess = GetHasAccess();
            //if (!Global.HasAccess.Update)
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();
            List<ItemComparisonDetailsVM> m_objCartItemVM = new List<ItemComparisonDetailsVM>();
            if (Session[dataSessionName] != null)
                m_objCartItemVM = JSON.Deserialize<List<ItemComparisonDetailsVM>>(Session[dataSessionName].ToString());

            ItemComparisonVM m_objCatalogCartVM = new ItemComparisonVM();
            m_objCatalogCartVM.ListItemComparisonDetailsVM = m_objCartItemVM;

            ViewDataDictionary m_vddCartItem = new ViewDataDictionary();
            m_vddCartItem.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddCartItem.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objCatalogCartVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddCartItem,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult AddToComparison(string Caller, string Selected)
        {
            ///*Global*/.HasAccess = GetHasAccess();
            ////if (!Global.HasAccess.Add)
            ////    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));
            if (Caller == General.EnumDesc(Buttons.ButtonList))
            {
                List<ItemComparisonDetailsVM> m_objSelectedRows = JSON.Deserialize<List<ItemComparisonDetailsVM>>(Selected);
                if (m_objSelectedRows.Count() > 0)
                {
                    if (Session[dataSessionName] != null)
                    {
                        List<ItemComparisonDetailsVM> m_objSessionData = JSON.Deserialize<List<ItemComparisonDetailsVM>>(Session[dataSessionName].ToString());
                        foreach (var item in m_objSelectedRows)
                        {
                            ItemComparisonDetailsVM objSession = m_objSessionData.Where(d => d.ItemID == item.ItemID && d.VendorID == item.VendorID).FirstOrDefault();
                            if (objSession == null)
                            {
                                m_objSessionData.Add(item);
                            }
                        }
                        Session[dataSessionName] = JSON.Serialize(m_objSessionData);
                    }
                    else
                    {
                        Session[dataSessionName] = JSON.Serialize(m_objSelectedRows);
                    }

                    
                }
                Session[formData] = Session[dataSessionName];

                Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Added));
            }
            return this.Direct();
        }
        public ActionResult Save()
        {
            string itemComparison_desc = Request.Params["m_txtItemComparisonDesc"].ToString();
            List<ItemComparisonDetailsVM> m_objSessionData = JSON.Deserialize<List<ItemComparisonDetailsVM>>(Session[dataSessionName].ToString());
            UserVM userVM = getCurentUser();

            MItemComparisonDA m_objItemComparisonDA = new MItemComparisonDA();
            DItemComparisonDetailsDA m_objItemComparisonDetailsDA = new DItemComparisonDetailsDA();

            ItemComparisonVM m_itemComparisonVM = new ItemComparisonVM();
            ItemComparisonDetailsVM m_itemComparisonDetailsVM = new ItemComparisonDetailsVM();

            m_objItemComparisonDA.ConnectionStringName = Global.ConnStrConfigName;
            string m_strTransName = "ItemComparison";
            object m_objDBConnection = null;
            m_objDBConnection = m_objItemComparisonDA.BeginTrans(m_strTransName);

            if (!IsSaveValid())
            {
                return this.Direct(false, General.EnumDesc(MessageLib.ErrorSave));
            }

            MItemComparison m_objItemComparison = new MItemComparison();

            m_objItemComparison.ItemComparisonID = Guid.NewGuid().ToString().Replace("-", "");
            m_objItemComparison.ItemComparisonDesc = itemComparison_desc;
            m_objItemComparison.UserID = "achmad.baihaqi";
           // m_objItemComparison.UserID = getCurentUser().UserID;
            m_objItemComparisonDA.Data = m_objItemComparison;
            
            if (Session[dataUpdate] == null)
            {
                m_objItemComparisonDA.Insert(true, m_objDBConnection);
            }

            if ((!m_objItemComparisonDA.Success || m_objItemComparisonDA.Message != string.Empty) && Session[dataUpdate] == null)
            {
                return this.Direct(false, General.EnumDesc(MessageLib.ErrorSave));
            }

            DItemComparisonDetails m_objItemComparisonDetails = new DItemComparisonDetails();
            
            foreach (var item in m_objSessionData)
            {
                if (item.ItemPriceID != null)
                {
                    m_objItemComparisonDetails.ItemComparisonDetailID = Guid.NewGuid().ToString().Replace("-", "");
                    
                    m_objItemComparisonDetails.ItemPriceID = item.ItemPriceID;
                    m_objItemComparisonDetails.VendorID = item.VendorID;
                    m_objItemComparisonDetails.ValidFrom = DateTime.Parse(item.ValidFrom.ToString());
                    m_objItemComparisonDetailsDA.Data = m_objItemComparisonDetails;

                    if (Session[dataUpdate] == null)
                    {
                        //Save
                        m_objItemComparisonDetails.ItemComparisonID = m_objItemComparison.ItemComparisonID;
                    }
                    else
                    {
                        //UPDATE
                        if (!string.IsNullOrEmpty(item.ItemComparisonID))
                        {
                            m_objItemComparisonDetails.ItemComparisonID = item.ItemComparisonID;
                        }
                    }

                    if (!IsDuplicate(item.ItemComparisonDetailID))
                    {
                        m_objItemComparisonDetailsDA.Insert(true, m_objDBConnection);

                        if (!m_objItemComparisonDetailsDA.Success || m_objItemComparisonDetailsDA.Message != string.Empty)
                        {
                            m_objItemComparisonDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                            m_objItemComparisonDetailsDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                            return this.Direct(false, General.EnumDesc(MessageLib.ErrorSave));
                        }
                    }
                }
            }

            m_objItemComparisonDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
            m_objItemComparisonDetailsDA.EndTrans(ref m_objDBConnection, true, m_strTransName);

            Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.saved));
            Session[dataSessionName] = null;
            return Home();
        }
        public ActionResult Update(string Caller, string Selected)
        {
            if (Session[dataSessionDetail] == null && Selected != null)
            {
                Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();
                m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
                string m_strMessage = string.Empty;
                List<ItemComparisonDetailsVM> m_objItemComparisonVM = new List<ItemComparisonDetailsVM>();


                m_objItemComparisonVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
                Session[dataSessionName] = JSON.Serialize(m_objItemComparisonVM);
            }

            Session[dataSessionDetail] = null;
            Session[dataUpdate] = "Update";

            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = null,
                RenderMode = RenderMode.AddTo,
                ViewData = null,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public bool IsSaveValid()
        {
            bool is_bool = true;
            if (Session[dataSessionName] == null)
            {
                is_bool = false;
            }
            return is_bool;
        }
        public bool IsDuplicate(string itemComparationDetailID)
        {
            //Check Duplicate ComparisonDetailID
            bool is_bool = true;

            DItemComparisonDetailsDA m_objDItemComparisonDetails = new DItemComparisonDetailsDA();

            m_objDItemComparisonDetails.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemComparisonDetailsVM.Prop.ItemComparisonDetailID.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(itemComparationDetailID);
            m_objFilter.Add(ItemComparisonDetailsVM.Prop.ItemComparisonDetailID.Map, m_lstFilter);

            if (itemComparationDetailID == null)
            {
                is_bool = false;
            }
            else
            {
                Dictionary<int, DataSet> m_dicDItemComparisonDetails = m_objDItemComparisonDetails.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
                if (m_dicDItemComparisonDetails[0].Tables[0].Rows.Count == 0)
                {
                    is_bool = false;
                }
            }
            
            return is_bool;
        }
        public ActionResult Detail(string Caller, string Selected)
        {
            Session[dataSessionDetail] = "Detail";

            Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();
            m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
            string m_strMessage = string.Empty;
            List< ItemComparisonDetailsVM> m_objItemComparisonVM = new List<ItemComparisonDetailsVM>();
            

            m_objItemComparisonVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            Session[dataSessionName] = JSON.Serialize(m_objItemComparisonVM);
            
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objItemComparisonVM,
                RenderMode = RenderMode.AddTo,
                ViewData = null,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult Delete(string Caller, string Selected)
        {
            Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();
            m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected.Substring(1, Selected.Length -2));

            MItemComparisonDA m_objItemComparisonDA = new MItemComparisonDA();
            DItemComparisonDetailsDA m_objItemComparisonDetailsDA = new DItemComparisonDetailsDA();

            ItemComparisonVM m_itemComparisonVM = new ItemComparisonVM();
            ItemComparisonDetailsVM m_itemComparisonDetailsVM = new ItemComparisonDetailsVM();

            MItemComparison m_objItemComparison = new MItemComparison();
            DItemComparisonDetails m_objItemComparisonDetails = new DItemComparisonDetails();

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (KeyValuePair<string, object> m_kvpSelectedRow in m_dicSelectedRow)
            {
                if (m_itemComparisonVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_objItemComparison.ItemComparisonID = m_kvpSelectedRow.Value.ToString();

                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(ItemComparisonDetailsVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }
            
            object m_objDBConnection = null;

            m_objItemComparisonDetailsDA.Data = m_objItemComparisonDetails;
            m_objItemComparisonDA.Data = m_objItemComparison;

            m_objItemComparisonDetailsDA.ConnectionStringName = Global.ConnStrConfigName;
            m_objItemComparisonDA.ConnectionStringName = Global.ConnStrConfigName;
            

            m_objItemComparisonDetailsDA.DeleteBC(m_objFilter, false, m_objDBConnection);
            
            if (!m_objItemComparisonDetailsDA.Success || m_objItemComparisonDetailsDA.Message != string.Empty)
            {
                return this.Direct(false, General.EnumDesc(MessageLib.ErrorSave));
            }
            m_objItemComparisonDA.Delete(false, m_objDBConnection);

            Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Deleted));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 0;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.HomePage),
                RenderMode = RenderMode.AddTo,
                ViewName = "_List",
                WrapByScriptTag = false
            };
        }
        public ActionResult GetPanel()
        {
            Panel BPPanel = new Panel
            {
                ID = "P" + "Item" + "Comparation",
                Frame = true,
                Title = "Item Comparation",
                Border = false
            };
            Toolbar m_BPPanelToolbar = new Toolbar();
            //Search Item
            Button m_btnSearch = new Button() { ID = "BtnSearch", Text = "Search Item", Icon = Icon.CartMagnify };
            m_btnSearch.DirectEvents.Click.Action = Url.Action("list", "catalog");
            m_btnSearch.DirectEvents.Click.EventMask.ShowMask = true;
            m_btnSearch.Visible = Session[dataSessionDetail] == null ? true : false;   
            m_BPPanelToolbar.Items.Add(m_btnSearch);

            //Submit To Cart
            Button m_btnSubmit = new Button() { ID = "BtnCart", Text = "Submit To Cart", Icon = Icon.CartAdd };
            m_btnSubmit.DirectEvents.Click.Action = Url.Action("checkout", "cart");
            m_btnSubmit.DirectEvents.Click.EventMask.ShowMask = true;
            m_btnSubmit.Visible = Session[dataSessionDetail] == null ? true : false;
            m_BPPanelToolbar.Items.Add(m_btnSubmit);

            //View Cart
            Button m_btnView = new Button() { ID = "BtnView", Text = "View Cart", Icon = Icon.CartGo };
            m_btnView.DirectEvents.Click.Action = Url.Action("List", "Cart");
            m_btnView.DirectEvents.Click.EventMask.ShowMask = true;
            m_btnView.Visible = Session[dataSessionDetail] == null ? true : false;
            m_BPPanelToolbar.Items.Add(m_btnView);

            //Save
            Button m_btnSave = new Button() { ID = "BtnSave", Text = "Save", Icon = Icon.DatabaseSave };
            m_btnSave.DirectEvents.Click.Action = Url.Action("Save", "itemcomparison");
            m_btnSave.DirectEvents.Click.EventMask.ShowMask = true;
            m_btnSave.Visible = Session[dataSessionDetail] == null ? true : false;
            m_BPPanelToolbar.Items.Add(m_btnSave);

            //Update
            Button m_btnUpdate = new Button() { ID = "BtnUpdate", Text = "Update", Icon = Icon.DatabaseEdit };
            m_btnUpdate.DirectEvents.Click.Action = Url.Action("Update", "itemcomparison");
            m_btnUpdate.DirectEvents.Click.EventMask.ShowMask = true;
            m_btnUpdate.Visible = Session[dataSessionDetail] == null ? false : true;
            m_BPPanelToolbar.Items.Add(m_btnUpdate);

            //List
            Button m_btnList = new Button() { ID = "BtnList", Text = "List", Icon = Icon.ApplicationViewList };
            m_btnList.DirectEvents.Click.Action = Url.Action("List", "itemcomparison");
            m_btnList.DirectEvents.Click.EventMask.ShowMask = true;
            m_BPPanelToolbar.Items.Add(m_btnList);

            TextField m_textDesc = new TextField();
            m_textDesc.ID = "m_txtItemComparisonDesc";
            m_textDesc.FieldLabel = "Description";
            m_textDesc.Padding = 5;
            BPPanel.Items.Add(m_textDesc);

            if (Session[dataSessionName] != null)
            {
                List<ItemComparisonDetailsVM> m_objSessionData = JSON.Deserialize<List<ItemComparisonDetailsVM>>(Session[dataSessionName].ToString());
                var vendorList = new List<string>();
                var itemList = new List<string>();

                string vendorId;
                string itemId;

                foreach (var item in m_objSessionData)
                {
                    vendorList.Add(item.VendorID);
                    itemList.Add(item.ItemID);
                }

                vendorId = string.Join(",", vendorList.ToArray());
                itemId = string.Join(",", itemList.ToArray());

                GridPanel m_GPBP = generateGridPanel(GetComparation(vendorId, itemId));
                BPPanel.Items.Add(m_GPBP);
            }
            BPPanel.TopBar.Add(m_BPPanelToolbar);
            Session[dataSessionDetail] = null;
            return this.ComponentConfig(BPPanel);
        }
        public ActionResult AddNodeTemplateStructure(string Selected, string jsonItemPrice)
        {
            NodeCollection nodeChildCollection = new NodeCollection();
            Dictionary<string, bool> m_dicDisplayPrice = DisplayPrice();

            BudgetPlanVersionStructureVM m_objSelected = new BudgetPlanVersionStructureVM();
            m_objSelected = JSON.Deserialize<BudgetPlanVersionStructureVM>(Selected);

            ItemPriceVM dataItemPrice = JSON.Deserialize<ItemPriceVM>(jsonItemPrice);

            Node m_node = new Node();
            m_node.Expandable = true;
            m_node.Expanded = true;

            List<BudgetPlanTemplateStructureVM> m_BudgetPlanTemplateStructureVM = new List<BudgetPlanTemplateStructureVM>();
            nodeChildCollection = LoadChildBPTemplateStructure(new BudgetPlanTemplateStructureVM() { BudgetPlanTemplateID = m_objSelected.BudgetPlanTemplateID, ItemID = m_objSelected.ItemID, Version = m_objSelected.Version, Sequence = m_objSelected.Sequence, ParentItemID = m_objSelected.ParentItemID, ParentVersion = m_objSelected.ParentVersion, ParentSequence = m_objSelected.ParentSequence }, dataItemPrice, m_BudgetPlanTemplateStructureVM);

            if (nodeChildCollection.Any())
            {
                m_objSelected.MaterialAmount = 0;
                m_objSelected.WageAmount = 0;
                m_objSelected.MiscAmount = 0;
                foreach (var node_ in nodeChildCollection)
                {
                    if (m_objSelected.IsBOI && (bool)node_.AttributesObject.GetType().GetProperties()[13].GetValue(node_.AttributesObject, null))
                    {
                        m_objSelected.MaterialAmount = null;
                        m_objSelected.WageAmount = null;
                        m_objSelected.MiscAmount = null;
                    }
                    else
                    {
                        m_objSelected.MaterialAmount += (decimal?)node_.AttributesObject.GetType().GetProperties()[15].GetValue(node_.AttributesObject, null) == null ? 0 : (decimal?)node_.AttributesObject.GetType().GetProperties()[15].GetValue(node_.AttributesObject, null);
                        m_objSelected.WageAmount += (decimal?)node_.AttributesObject.GetType().GetProperties()[16].GetValue(node_.AttributesObject, null) == null ? 0 : (decimal?)node_.AttributesObject.GetType().GetProperties()[16].GetValue(node_.AttributesObject, null);
                        m_objSelected.MiscAmount += (decimal?)node_.AttributesObject.GetType().GetProperties()[17].GetValue(node_.AttributesObject, null) == null ? 0 : (decimal?)node_.AttributesObject.GetType().GetProperties()[17].GetValue(node_.AttributesObject, null);
                    }
                }
            }

            m_node = new Node()
            {
                Expanded = nodeChildCollection.Count > 0,
                Expandable = nodeChildCollection.Count > 0,
                AttributesObject = new
                {
                    itemdesc = m_objSelected.ItemDesc,
                    budgetplantemplateid = m_objSelected.BudgetPlanTemplateID,
                    itemversionchildid = m_objSelected.ItemVersionChildID,
                    itemid = m_objSelected.ItemID,
                    version = m_objSelected.Version,
                    sequence = m_objSelected.Sequence,
                    uomdesc = m_objSelected.UoMDesc,
                    parentitemid = m_objSelected.ParentItemID,
                    parentversion = m_objSelected.ParentVersion,
                    parentsequence = m_objSelected.ParentSequence,
                    haschild = nodeChildCollection.Any(),
                    itemtypeid = m_objSelected.ItemTypeID,
                    parentitemtypeid = m_objSelected.ItemTypeID,
                    isboi = m_objSelected.IsBOI,
                    isahs = m_objSelected.IsAHS,
                    materialamount = (m_objSelected.MaterialAmount == 0 ? null : m_objSelected.MaterialAmount),
                    wageamount = (m_objSelected.WageAmount == 0 ? null : m_objSelected.WageAmount),
                    miscamount = (m_objSelected.MiscAmount == 0 ? null : m_objSelected.MiscAmount),
                    leaf = nodeChildCollection.Count > 0 ? false : true,
                    totalunitprice = 0,
                    total = 0,
                    specification = string.Empty,
                    uomid = m_objSelected.UoMID,
                    displayprice = (m_dicDisplayPrice.ContainsKey(m_objSelected.ItemTypeID) ? m_dicDisplayPrice[m_objSelected.ItemTypeID] : true)
                },
                Icon = m_objSelected.IsBOI ? Icon.Folder : (m_objSelected.IsAHS ? Icon.Table : Icon.PageWhite)
            };

            m_node.Children.AddRange(nodeChildCollection);

            return this.Store(m_node);
        }
        public NodeCollection LoadChildBPTemplateStructure(BudgetPlanTemplateStructureVM dataParent, ItemPriceVM dataItemPrice, List<BudgetPlanTemplateStructureVM> budgetPlanTemplateStructures)
        {
            NodeCollection nodeCollection = new NodeCollection();
            Dictionary<string, bool> m_dicDisplayPrice = DisplayPrice();

            BudgetPlanTemplateStructureVM m_objDBudgetPlanTemplateStructureVM = new BudgetPlanTemplateStructureVM();
            DBudgetPlanTemplateStructureDA m_objDBudgetPlanTemplateStructureDA = new DBudgetPlanTemplateStructureDA();
            m_objDBudgetPlanTemplateStructureDA.ConnectionStringName = Global.ConnStrConfigName;

            List<BudgetPlanTemplateStructureVM> lstBudgetPlanTemplateStructureVM = new List<BudgetPlanTemplateStructureVM>();
            if (!budgetPlanTemplateStructures.Any()) budgetPlanTemplateStructures = GetBudgetPlanTemplateStructure(dataParent.BudgetPlanTemplateID);
            lstBudgetPlanTemplateStructureVM = budgetPlanTemplateStructures.Where(d =>
                                                           d.ParentItemID.Equals(dataParent.ItemID) &&
                                                           d.ParentVersion == dataParent.Version &&
                                                           d.ParentSequence == dataParent.Sequence).ToList();


            foreach (BudgetPlanTemplateStructureVM item in lstBudgetPlanTemplateStructureVM)
            {
                string m_strSpecification = string.Empty;

                Node node = new Node();
                NodeCollection nodeCollection_ = LoadChildBPTemplateStructure(new BudgetPlanTemplateStructureVM()
                {
                    BudgetPlanTemplateID = dataParent.BudgetPlanTemplateID,
                    ItemID = item.ItemID,
                    Version = item.Version,
                    Sequence = item.Sequence,
                    ParentItemID = item.ParentItemID,
                    ParentVersion = item.ParentVersion,
                    ParentSequence = item.ParentSequence
                },
                    dataItemPrice, budgetPlanTemplateStructures);


                if (item.IsAHS)
                {
                    nodeCollection_ = LoadChildItemVersion(new ItemVersionChildVM()
                    {
                        ItemID = item.ItemID,
                        Version = item.Version,
                        Sequence = item.Sequence
                    },
                    dataItemPrice);

                }


                node = new Node();
                node.Expanded = nodeCollection_.Count > 0;
                node.Expandable = nodeCollection_.Count > 0;
                node.Leaf = nodeCollection_.Count > 0 ? false : true;

                if (nodeCollection_.Any())
                {
                    item.MaterialAmount = 0;
                    item.WageAmount = 0;
                    item.MiscAmount = 0;
                    foreach (var node_ in nodeCollection_)
                    {

                        if (item.IsBOI && (bool)node_.AttributesObject.GetType().GetProperties()[13].GetValue(node_.AttributesObject, null))
                        {
                            item.MaterialAmount = null;
                            item.WageAmount = null;
                            item.MiscAmount = null;

                        }
                        else
                        {
                            if (item.IsBOI) m_strSpecification += node_.AttributesObject.GetType().GetProperties()[0].GetValue(node_.AttributesObject, null).ToString() + ", ";
                            item.MaterialAmount += (decimal?)node_.AttributesObject.GetType().GetProperties()[15].GetValue(node_.AttributesObject, null) == null ? 0 : (decimal?)node_.AttributesObject.GetType().GetProperties()[15].GetValue(node_.AttributesObject, null);
                            item.WageAmount += (decimal?)node_.AttributesObject.GetType().GetProperties()[16].GetValue(node_.AttributesObject, null) == null ? 0 : (decimal?)node_.AttributesObject.GetType().GetProperties()[16].GetValue(node_.AttributesObject, null);
                            item.MiscAmount += (decimal?)node_.AttributesObject.GetType().GetProperties()[17].GetValue(node_.AttributesObject, null) == null ? 0 : (decimal?)node_.AttributesObject.GetType().GetProperties()[17].GetValue(node_.AttributesObject, null);
                        }
                    }
                }

                node.Icon = item.IsBOI ? Icon.Folder : Icon.Table;
                node.AttributesObject = new
                {
                    itemdesc = item.ItemDesc,
                    budgetplantemplateid = item.BudgetPlanTemplateID,
                    itemid = item.ItemID,
                    version = item.Version,
                    sequence = item.Sequence,
                    parentitemid = dataParent.ItemID,
                    parentversion = dataParent.Version,
                    parentsequence = dataParent.Sequence,
                    haschild = nodeCollection_.Any(),
                    itemtypeid = item.ItemTypeID,
                    parentitemtypeid = dataParent.ParentItemTypeID,
                    isdefault = item.IsDefault,
                    uomdesc = item.UoMDesc,
                    isboi = item.IsBOI,
                    isahs = item.IsAHS,
                    materialamount = (item.MaterialAmount == 0 ? null : item.MaterialAmount),
                    wageamount = (item.WageAmount == 0 ? null : item.WageAmount),
                    miscamount = (item.MiscAmount == 0 ? null : item.MiscAmount),
                    leaf = nodeCollection_.Count > 0 ? false : true,
                    specification = (m_strSpecification.Length > 0 ? m_strSpecification.Substring(0, m_strSpecification.Length - 2) : ""),
                    uomid = item.UoMID,
                    totalunitprice = 0,
                    total = 0,
                    displayprice = (m_dicDisplayPrice.ContainsKey(item.ItemTypeID) ? m_dicDisplayPrice[item.ItemTypeID] : true)
                };

                node.Children.AddRange(nodeCollection_);
                nodeCollection.Add(node);
            }


            return nodeCollection;

        }
        public NodeCollection LoadChildItemVersion(ItemVersionChildVM dataParent, ItemPriceVM dataItemPrice)
        {

            NodeCollection m_nodeCollectChild = new Ext.Net.NodeCollection();
            Dictionary<string, bool> m_dicDisplayPrice = DisplayPrice();

            DItemVersionChildDA m_objMItemVersionChildDA = new DItemVersionChildDA();
            m_objMItemVersionChildDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = 0;
            bool m_boolIsCount = true;

            List<ItemVersionChildVM> m_lstItemVersionChildVM = new List<ItemVersionChildVM>();
            List<BudgetPlanTemplateStructureVM> m_lstBudgetPlanTemplateStructureVM = new List<BudgetPlanTemplateStructureVM>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();


            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(dataParent.ItemID);
            m_objFilter.Add(ItemVersionChildVM.Prop.ItemID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(dataParent.Version);
            m_objFilter.Add(ItemVersionChildVM.Prop.Version.Map, m_lstFilter);

            if (!string.IsNullOrEmpty(dataParent.ChildItemID))
            {
                m_objFilter = new Dictionary<string, List<object>>();
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(dataParent.ChildItemID);
                m_objFilter.Add(ItemVersionChildVM.Prop.ItemID.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(dataParent.ChildVersion);
                m_objFilter.Add(ItemVersionChildVM.Prop.Version.Map, m_lstFilter);
            }

            m_boolIsCount = false;
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemVersionChildID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemTypeID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildItemID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildVersion.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.VersionDesc.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Coefficient.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.UoMDesc.MapAlias);
            m_lstSelect.Add(UoMVM.Prop.UoMID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Formula.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Sequence.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Key3.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc1.MapAlias);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(ItemVersionChildVM.Prop.Sequence.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicMItemVersionDA = m_objMItemVersionChildDA.SelectBC(m_intSkip, null, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
            if (m_objMItemVersionChildDA.Message == string.Empty)
            {
                m_lstItemVersionChildVM = (
                    from DataRow m_drMItemVersionChildDA in m_dicMItemVersionDA[0].Tables[0].Rows
                    select new ItemVersionChildVM()
                    {
                        ItemVersionChildID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemVersionChildID.Name].ToString(),
                        ItemID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemID.Name].ToString(),
                        ItemDesc = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemDesc.Name].ToString(),
                        Version = Convert.ToInt16(m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildVersion.Name].ToString()),
                        ItemTypeID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemTypeID.Name].ToString(),
                        ChildItemID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemID.Name].ToString(),
                        ChildVersion = (int)m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildVersion.Name],
                        VersionDesc = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.VersionDesc.Name].ToString(),
                        Sequence = (int)m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Sequence.Name],
                        SequenceDesc = dataParent.SequenceDesc + m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Sequence.Name].ToString(),
                        Coefficient = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Coefficient.Name].ToString() == String.Empty ? 0 : (decimal)m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Coefficient.Name],
                        UoMDesc = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.UoMDesc.Name].ToString(),
                        UoMID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.UoMID.Name].ToString(),
                        Formula = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Formula.Name].ToString(),
                        IsBOI = m_drMItemVersionChildDA[ConfigVM.Prop.Key3.Name].ToString() == m_drMItemVersionChildDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString()
                                    && m_drMItemVersionChildDA[ConfigVM.Prop.Desc1.Name].ToString() == "TRUE",
                        IsAHS = m_drMItemVersionChildDA[ConfigVM.Prop.Key3.Name].ToString() == m_drMItemVersionChildDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString()
                                    && m_drMItemVersionChildDA[ConfigVM.Prop.Desc1.Name].ToString() == "FALSE",
                        MaterialAmount = GetUnitPrice(m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemVersionChildID.Name].ToString(), m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemID.Name].ToString(), dataItemPrice, m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemTypeID.Name].ToString(), BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name),
                        WageAmount = GetUnitPrice(m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemVersionChildID.Name].ToString(), m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemID.Name].ToString(), dataItemPrice, m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemTypeID.Name].ToString(), BudgetPlanVersionStructureVM.Prop.WageAmount.Name),
                        MiscAmount = GetUnitPrice(m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemVersionChildID.Name].ToString(), m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemID.Name].ToString(), dataItemPrice, m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemTypeID.Name].ToString(), BudgetPlanVersionStructureVM.Prop.MiscAmount.Name),
                    }
                ).ToList();
            }

            int m_intSequence = 0;
            int m_parentSequence = (dataParent.Sequence);
            foreach (ItemVersionChildVM item in m_lstItemVersionChildVM)
            {

                m_intSequence = ++m_parentSequence;

                NodeCollection m_nodeChildCollection = LoadChildItemVersion(new ItemVersionChildVM()
                {
                    ItemVersionChildID = item.ItemVersionChildID,
                    ItemID = item.ItemID,
                    Version = item.Version,
                    Sequence = m_intSequence,
                    ChildItemID = item.ChildItemID,
                    ChildVersion = item.ChildVersion
                }, dataItemPrice);

                if (m_nodeChildCollection.Any())
                {
                    item.MaterialAmount = 0;
                    item.WageAmount = 0;
                    item.MiscAmount = 0;
                    foreach (var node_ in m_nodeChildCollection)
                    {
                        item.MaterialAmount += (decimal?)node_.AttributesObject.GetType().GetProperties()[15].GetValue(node_.AttributesObject, null) == null ? 0 : (decimal?)node_.AttributesObject.GetType().GetProperties()[15].GetValue(node_.AttributesObject, null);
                        item.WageAmount += (decimal?)node_.AttributesObject.GetType().GetProperties()[16].GetValue(node_.AttributesObject, null) == null ? 0 : (decimal?)node_.AttributesObject.GetType().GetProperties()[16].GetValue(node_.AttributesObject, null);
                        item.MiscAmount += (decimal?)node_.AttributesObject.GetType().GetProperties()[17].GetValue(node_.AttributesObject, null) == null ? 0 : (decimal?)node_.AttributesObject.GetType().GetProperties()[17].GetValue(node_.AttributesObject, null);
                    }
                }


                Node node = new Node();
                node = new Node()
                {
                    Expanded = m_nodeChildCollection.Count > 0,
                    Expandable = m_nodeChildCollection.Count > 0,
                    Leaf = m_nodeChildCollection.Count == 0,
                    AttributesObject = new
                    {
                        itemdesc = item.ItemDesc,
                        itemversionchildid = item.ItemVersionChildID,
                        itemid = item.ChildItemID,
                        version = item.ChildVersion,
                        sequence = m_intSequence,
                        uomdesc = item.UoMDesc,
                        parentitemid = item.ItemID,
                        parentversion = dataParent.Version,
                        parentsequence = dataParent.Sequence,
                        haschild = m_nodeChildCollection.Any(),
                        itemtypeid = item.ItemTypeID,
                        parentitemtypeid = dataParent.ItemTypeID,
                        isboi = item.IsBOI,
                        isahs = item.IsAHS,
                        isdefault = item.IsDefault,
                        materialamount = item.MaterialAmount * item.Coefficient,
                        wageamount = item.WageAmount * item.Coefficient,
                        miscamount = item.MiscAmount * item.Coefficient,

                        leaf = m_nodeChildCollection.Count > 0 ? false : true,
                        uomid = item.UoMID,
                        displayprice = (m_dicDisplayPrice.ContainsKey(item.ItemTypeID) ? m_dicDisplayPrice[item.ItemTypeID] : true)
                    },
                    Icon = m_nodeChildCollection.Count > 0 ? Icon.Table : Icon.PageWhite
                };
                node.Children.AddRange(m_nodeChildCollection);
                m_nodeCollectChild.Add(node);
            }

            return m_nodeCollectChild;

        }
        private List<BudgetPlanTemplateStructureVM> GetBudgetPlanTemplateStructure(string budgetPlanTemplateID)
        {

            BudgetPlanTemplateStructureVM m_objDBudgetPlanTemplateStructureVM = new BudgetPlanTemplateStructureVM();
            DBudgetPlanTemplateStructureDA m_objDBudgetPlanTemplateStructureDA = new DBudgetPlanTemplateStructureDA();
            m_objDBudgetPlanTemplateStructureDA.ConnectionStringName = Global.ConnStrConfigName;


            List<BudgetPlanTemplateStructureVM> lstBudgetPlanTemplateStructureVM = new List<BudgetPlanTemplateStructureVM>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(budgetPlanTemplateID);
            m_objFilter.Add(BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.Map, m_lstFilter);

            //m_lstFilter = new List<object>();
            //m_lstFilter.Add(Operator.None);
            //m_lstFilter.Add(string.Empty);
            //m_objFilter.Add(string.Format("({0} IS NULL OR {0} = 1)", BudgetPlanTemplateStructureVM.Prop.IsDefault.Map), m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.None);
            m_lstFilter.Add("");
            m_objFilter.Add("(DBudgetPlanTemplateStructure.IsDefault IS NULL OR DBudgetPlanTemplateStructure.IsDefault = 1)", m_lstFilter);

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.Version.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.Sequence.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentSequence.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemTypeID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.IsDefault.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemTypeID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.UoMDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.UoMID.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Key3.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc1.MapAlias);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(BudgetPlanTemplateStructureVM.Prop.Sequence.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicDBudgetPlanTemplateStructureDA = m_objDBudgetPlanTemplateStructureDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, m_dicOrder);
            if (m_objDBudgetPlanTemplateStructureDA.Message == string.Empty)
            {
                lstBudgetPlanTemplateStructureVM = (
                        from DataRow m_drDBudgetPlanTemplateStructureDA in m_dicDBudgetPlanTemplateStructureDA[0].Tables[0].Rows
                        select new BudgetPlanTemplateStructureVM()
                        {
                            BudgetPlanTemplateID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.Name].ToString(),
                            ItemID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemID.Name].ToString(),
                            ItemDesc = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemDesc.Name].ToString(),
                            Version = Convert.ToInt16(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.Version.Name].ToString()),
                            Sequence = Convert.ToInt16(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.Sequence.Name].ToString()),
                            ParentItemID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentItemID.Name].ToString(),
                            ParentSequence = Convert.ToInt16(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentSequence.Name].ToString()),
                            ParentVersion = Convert.ToInt16(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentVersion.Name].ToString()),
                            ItemTypeID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString(),
                            IsBOI = m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Key3.Name].ToString() == m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString()
                                    && m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Desc1.Name].ToString() == "TRUE",
                            IsAHS = m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Key3.Name].ToString() == m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString()
                                    && m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Desc1.Name].ToString() == "FALSE",
                            IsDefault = (string.IsNullOrEmpty(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.IsDefault.Name].ToString()) ? null :
                                        (bool?)(bool.Parse(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.IsDefault.Name].ToString()))),
                            ParentItemTypeID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentItemTypeID.Name].ToString(),
                            UoMDesc = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.UoMDesc.Name].ToString(),
                            UoMID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.UoMID.Name].ToString()

                        }).ToList();


            }

            return lstBudgetPlanTemplateStructureVM;
        }
        private Dictionary<string, bool> DisplayPrice()
        {
            Dictionary<string, bool> m_dicItemTypeForDisplayPrice = new Dictionary<string, bool>();
            string m_strMenuObject = GetMenuObject("DisplayPrice");
            List<ConfigVM> m_lstConfig = GetItemTypeDisplayPriceConfig();
            bool m_boolValueObject = Convert.ToBoolean(string.IsNullOrEmpty(m_strMenuObject) ? "True" : m_strMenuObject);

            foreach (var item in m_lstConfig)
            {
                m_dicItemTypeForDisplayPrice.Add(item.Key2, m_boolValueObject);
            }
            return m_dicItemTypeForDisplayPrice;
        }
        private List<ConfigVM> GetItemTypeDisplayPriceConfig()
        {

            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();


            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ConfigVM.Prop.Key2.MapAlias);

            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name);
            m_objFilter.Add(ConfigVM.Prop.Key1.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("DisplayPrice");
            m_objFilter.Add(ConfigVM.Prop.Key3.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicUConfigDA = m_objUConfigDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            List<ConfigVM> m_lstConfigVM = new List<ConfigVM>();
            if (m_objUConfigDA.Message == string.Empty)
            {
                m_lstConfigVM = (
                        from DataRow m_drUConfigDA in m_dicUConfigDA[0].Tables[0].Rows
                        select new ConfigVM()
                        {
                            Key2 = m_drUConfigDA[ConfigVM.Prop.Key2.Name].ToString()
                        }
                    ).ToList();
            }
            return m_lstConfigVM;

        }
        public ActionResult Browse(string ControlItemID, string ControlItemDesc, string ControlItemPriceID, string ControlAmount,
            string ControlGridItemCompare, string FilterItemID = "", string ControlFromItem = "", string FilterItemDesc = "", string FilterItemPriceID = "",
            string ControlVendorDesc = "", string ControlVendorID = "", string ControlTreePanel = "",
            string ControlAlternativeItem = "", string SelectedRow_ = "", string FilterAmount = "",
            string RegionID = "", string ProjectID = "", string ClusterID = "", string UnitTypeID = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized)) ;

            ViewDataDictionary m_vddParameter = new ViewDataDictionary();
            m_vddParameter.Add("Control" + ItemVM.Prop.ItemID.Name, ControlItemID);
            m_vddParameter.Add("Control" + ItemVM.Prop.ItemDesc.Name, ControlItemDesc);
            m_vddParameter.Add("Control" + ItemPriceVendorPeriodVM.Prop.ItemPriceID.Name, ControlItemPriceID);
            m_vddParameter.Add("Control" + ItemPriceVendorVM.Prop.VendorDesc.Name, ControlVendorDesc); ;
            m_vddParameter.Add("Control" + ItemPriceVendorVM.Prop.VendorID.Name, ControlVendorID);
            m_vddParameter.Add("Control" + ItemPriceVendorPeriodVM.Prop.Amount.Name, ControlAmount);
            m_vddParameter.Add(ItemVM.Prop.ItemID.Name, FilterItemID);
            m_vddParameter.Add(ItemVM.Prop.ItemDesc.Name, FilterItemDesc);
            m_vddParameter.Add(ItemPriceVendorPeriodVM.Prop.ItemPriceID.Name, FilterItemPriceID);
            m_vddParameter.Add(ItemPriceVendorPeriodVM.Prop.Amount.Name, FilterAmount);
            m_vddParameter.Add(ItemPriceVendorPeriodVM.Prop.RegionID.Name, RegionID);
            m_vddParameter.Add(ItemPriceVendorPeriodVM.Prop.ProjectID.Name, ProjectID);
            m_vddParameter.Add(ItemPriceVendorPeriodVM.Prop.ClusterID.Name, ClusterID);
            m_vddParameter.Add(ItemPriceVendorPeriodVM.Prop.UnitTypeID.Name, UnitTypeID);
            m_vddParameter.Add("ControlFromItem", ControlFromItem);
            m_vddParameter.Add("ControlGridItemCompare", ControlGridItemCompare);
            m_vddParameter.Add("ControlTreePanel", ControlTreePanel);
            m_vddParameter.Add("ControlAlternativeItem", ControlAlternativeItem);
            m_vddParameter.Add("SelectedRow_", SelectedRow_);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddParameter,
                ViewName = "../ItemComparison/_Browse"
            };
        }
    
        public ActionResult BrowseVendor(string ControlVendorID, string ControlVendorDesc, string ControlAmount, string ControlGridVendorCompare, string GridStructure, string ControlItemComparisonDetailID, string FilterVendorID = "", string FilterVendorDesc = "", string FilterAmount = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddParameter = new ViewDataDictionary();
            //m_vddParameter.Add("ControlItemComparisonDetailID" + ItemComparisonDetailsVM.Prop.ItemComparisonDetailID.Name, ControlItemComparisonDetailID);


            //FilterHeaderConditions m_fhcMBudgetPlanTemplate = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            //Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            //foreach (FilterHeaderCondition m_fhcFilter in m_fhcMBudgetPlanTemplate.Conditions)
            //{
            //    Console.WriteLine(m_fhcFilter);
            //}


            Dictionary<string, object>[] m_arrGridStructure = JSON.Deserialize<Dictionary<string, object>[]>(this.Request.Params["GridStructure"]);
            if (m_arrGridStructure == null)
                m_arrGridStructure = new List<Dictionary<string, object>>().ToArray();

            foreach (Dictionary<string, object> m_dicGridStructure in m_arrGridStructure)
            {
                Console.WriteLine(m_dicGridStructure);
            }


            //ItemComparisonDetailsVM.Prop.ItemComparisonDetailID

            m_vddParameter.Add("Control" + VendorVM.Prop.VendorID.Name, ControlVendorID);
            m_vddParameter.Add("Control" + VendorVM.Prop.VendorDesc.Name, ControlVendorDesc);
            m_vddParameter.Add("Control" + ItemPriceVendorPeriodVM.Prop.Amount.Name, ControlAmount);
            m_vddParameter.Add(VendorVM.Prop.VendorID.Name, FilterVendorID);
            m_vddParameter.Add(VendorVM.Prop.VendorDesc.Name, FilterVendorDesc);
            m_vddParameter.Add(ItemPriceVendorPeriodVM.Prop.Amount.Name, FilterAmount);
            m_vddParameter.Add("ControlGridVendorCompare", ControlGridVendorCompare);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddParameter,
                ViewName = "../ItemComparison/_BrowseVendor"
            };
        }
        #endregion

        #region Direct Method

        public ActionResult GetStatusList(StoreRequestParameters parameters)
        {
            List<StatusVM> m_objStatusVM = new List<StatusVM>();

            DataAccess.MStatusDA m_objMStatusDA = new DataAccess.MStatusDA();
            m_objMStatusDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(StatusVM.Prop.StatusDesc.MapAlias);
            m_lstSelect.Add(StatusVM.Prop.StatusID.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("DItemComparisonDetail");
            m_objFilter.Add(StatusVM.Prop.TableName.Name, m_lstFilter);

            Dictionary<int, System.Data.DataSet> m_dicMStatusDA = m_objMStatusDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objMStatusDA.Message == String.Empty)
            {
                for (int i = 0; i < m_dicMStatusDA[0].Tables[0].Rows.Count; i++)
                {
                    m_objStatusVM.Add(new StatusVM() { StatusDesc = m_dicMStatusDA[0].Tables[0].Rows[i][StatusVM.Prop.StatusDesc.Name].ToString(), StatusID = int.Parse(m_dicMStatusDA[0].Tables[0].Rows[i][StatusVM.Prop.StatusID.Name].ToString()) });
                }
            }

            return this.Store(m_objStatusVM);
        }

        #endregion

        #region Private Method
        private List<ItemComparisonDetailsVM> GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            ItemComparisonVM m_objItemComparisonVM = new ItemComparisonVM();
            ItemComparisonDetailsVM m_objItemComparisonDetails = new ItemComparisonDetailsVM();

            DItemComparisonDetailsDA m_objDItemComparisonDetails = new DItemComparisonDetailsDA();
            m_objDItemComparisonDetails.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemComparisonDetailsVM.Prop.ItemComparisonDetailID.MapAlias);
            m_lstSelect.Add(ItemComparisonDetailsVM.Prop.ItemComparisonID.MapAlias);
            m_lstSelect.Add(ItemComparisonDetailsVM.Prop.ItemPriceID.MapAlias);
            m_lstSelect.Add(ItemComparisonDetailsVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(ItemComparisonDetailsVM.Prop.ValidFrom.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.ItemID.MapAlias);


            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objItemComparisonVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(ItemComparisonDetailsVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }
            List<ItemComparisonDetailsVM> listItemComparison = new List<ItemComparisonDetailsVM>();
            Dictionary<int, DataSet> m_dicDItemComparisonDetails = m_objDItemComparisonDetails.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);

            ItemComparisonDetailsVM m_itemComparisonVM;
           

            foreach (DataRow d_itemComparisonDetails in m_dicDItemComparisonDetails[0].Tables[0].Rows)
            {
                m_itemComparisonVM = new ItemComparisonDetailsVM();
                m_itemComparisonVM.ItemComparisonDetailID = d_itemComparisonDetails[ItemComparisonDetailsVM.Prop.ItemComparisonDetailID.Name].ToString();
                m_itemComparisonVM.ItemComparisonID = d_itemComparisonDetails[ItemComparisonDetailsVM.Prop.ItemComparisonID.Name].ToString();
                m_itemComparisonVM.ItemPriceID = d_itemComparisonDetails[ItemComparisonDetailsVM.Prop.ItemPriceID.Name].ToString();
                m_itemComparisonVM.VendorID = d_itemComparisonDetails[ItemComparisonDetailsVM.Prop.VendorID.Name].ToString();
                m_itemComparisonVM.ItemID = d_itemComparisonDetails[ItemVM.Prop.ItemID.Name].ToString();
                m_itemComparisonVM.ValidFrom = DateTime.Parse(d_itemComparisonDetails[ItemPriceVendorPeriodVM.Prop.ValidFrom.Name].ToString());
                listItemComparison.Add(m_itemComparisonVM);
            }
            
            return listItemComparison;
        }
        private Dictionary<string, object> GetFormData(NameValueCollection parameters, string ItemID = "")
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();
            m_dicReturn.Add(ItemVM.Prop.ItemID.Name, (parameters[ItemVM.Prop.ItemID.Name].ToString() == string.Empty ? ItemID : parameters[ItemVM.Prop.ItemID.Name]));
            m_dicReturn.Add(ItemVM.Prop.ItemDesc.Name, parameters[ItemVM.Prop.ItemDesc.Name]);
            m_dicReturn.Add(ItemVM.Prop.ItemGroupID.Name, parameters[ItemVM.Prop.ItemGroupID.Name]);
            m_dicReturn.Add(ItemVM.Prop.ItemGroupDesc.Name, parameters[ItemVM.Prop.ItemGroupDesc.Name]);
            m_dicReturn.Add(ItemVM.Prop.ItemTypeID.Name, parameters[ItemVM.Prop.ItemTypeID.Name]);
            m_dicReturn.Add(ItemVM.Prop.ItemTypeDesc.Name, parameters[ItemVM.Prop.ItemTypeDesc.Name]);

            return m_dicReturn;
        }
        private string GetFilterPrice(ItemPriceVM itemPriceVM)
        {
            string m_strFilter = string.Empty;
            m_strFilter = string.Format("({0}='{4}' AND {1}='{5}' AND {2}='{6}' AND {3}='{7}')", ItemPriceVM.Prop.RegionID.Name, ItemPriceVM.Prop.ProjectID.Name, ItemPriceVM.Prop.ClusterID.Name, ItemPriceVM.Prop.UnitTypeID.Name,
                itemPriceVM.RegionID, itemPriceVM.ProjectID, itemPriceVM.ClusterID, itemPriceVM.UnitTypeID);
            m_strFilter += string.Format("OR ({0}='{4}' AND {1}='{5}' AND {2}='{6}' AND {3}='')", ItemPriceVM.Prop.RegionID.Name, ItemPriceVM.Prop.ProjectID.Name, ItemPriceVM.Prop.ClusterID.Name, ItemPriceVM.Prop.UnitTypeID.Name,
                itemPriceVM.RegionID, itemPriceVM.ProjectID, itemPriceVM.ClusterID);
            m_strFilter += string.Format("OR ({0}='{4}' AND {1}='{5}' AND {2}='' AND {3}='')", ItemPriceVM.Prop.RegionID.Name, ItemPriceVM.Prop.ProjectID.Name, ItemPriceVM.Prop.ClusterID.Name, ItemPriceVM.Prop.UnitTypeID.Name,
                itemPriceVM.RegionID, itemPriceVM.ProjectID);
            m_strFilter += string.Format("OR ({0}='{4}' AND {1}='' AND {2}='' AND {3}='')", ItemPriceVM.Prop.RegionID.Name, ItemPriceVM.Prop.ProjectID.Name, ItemPriceVM.Prop.ClusterID.Name, ItemPriceVM.Prop.UnitTypeID.Name,
                itemPriceVM.RegionID);


            return m_strFilter;
        }
        private GridPanel generateGridPanel(DataTable ListItemComparation)
        {
            GridPanel m_gridpanel = new GridPanel
            {
                ID = "grdItemComparison",
                Padding = 10,
                MinHeight = 200
            };

            //SelectionModel
            RowSelectionModel m_rowSelectionModel = new RowSelectionModel() { Mode = SelectionMode.Multi, AllowDeselect = true };
            m_gridpanel.SelectionModel.Add(m_rowSelectionModel);

            //Store         
            Store m_store = new Store();
            Model m_model = new Model();
            ModelField m_ModelField;
            
            List<ColumnBase> m_ListColumn = new List<ColumnBase>();;
            ColumnBase m_objColumn;
            
            foreach (DataColumn dc in ListItemComparation.Columns)
            {
                m_ModelField = new ModelField(dc.ColumnName);
                m_model.Fields.Add(m_ModelField);

                m_objColumn = new Column { Text = dc.ColumnName, DataIndex = dc.ColumnName, Flex = 1 };
                m_ListColumn.Add(m_objColumn);
            }
            
            m_store.Model.Add(m_model);
            m_store.DataSource = ListItemComparation;
            m_gridpanel.Store.Add(m_store);
            
            m_gridpanel.ColumnModel.Columns.AddRange(m_ListColumn);

            //Plugin
            CellEditing m_objCellEditing = new CellEditing() { ClicksToEdit = 1 };
            m_gridpanel.Plugins.Add(m_objCellEditing);
            
            return m_gridpanel;
        }
        private DataTable GetComparation(string vendorID, string itemID)
        {
            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            Dictionary<string, List<object>> m_objFilter_item = new Dictionary<string, List<object>>();

            DItemComparisonDetailsDA m_objDItemComparisonDetailsDA = new DItemComparisonDetailsDA();
            m_objDItemComparisonDetailsDA.ConnectionStringName = Global.ConnStrConfigName;
            
            m_lstFilter = new List<object>();
            m_objFilter = new Dictionary<string, List<object>>();

            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(vendorID);
            m_objFilter.Add(VendorVM.Prop.VendorID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_objFilter_item = new Dictionary<string, List<object>>();

            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(itemID);
            m_objFilter_item.Add(ItemPriceVM.Prop.ItemID.Map, m_lstFilter);

            Dictionary<int, DataSet> DItemComparisonDetailsDA = m_objDItemComparisonDetailsDA.SelectBC(m_objFilter, m_objFilter_item, null);

            return DItemComparisonDetailsDA[0].Tables[0];
        }
        
        #endregion

        #region Public Method
        public ActionResult GetNodeAppend(string a, string b, string c, string d)
        {
            string thisItemDesc = string.Empty;
            string thisItem = string.Empty;
            string ItemComparisonDetailID = string.Empty;
            Node m_node = new Node();

            List<string> m_lstSelectedRowss = new List<string>();
            m_lstSelectedRowss = JSON.Deserialize<List<string>>(this.Request.Params["ItemID"]);
            
            FilterHeaderConditions m_fhcMBudgetPlanTemplate = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            
            ItemPriceVendorPeriodVM m_objItemPriceVendorPeriod = new ItemPriceVendorPeriodVM();
            Dictionary<string, List<object>> m_objFiltersFirst = new Dictionary<string, List<object>>();
            //DItemVersionDA m_objVersion = new DItemVersionDA();
            MItemDA m_objMItem = new MItemDA();
            m_objMItem.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstSelectsFirst = new List<string>();
            m_lstSelectsFirst.Add(ItemVM.Prop.ItemDesc.Map);
            m_lstSelectsFirst.Add(ItemVM.Prop.ItemID.Map);

            List<object> m_lstFilterss = new List<object>();
            m_lstFilterss.Add(Operator.Equals);
            m_lstFilterss.Add(thisItem);
            m_objFiltersFirst.Add(ItemVM.Prop.ItemID.Map, m_lstFilterss);


            Dictionary<int, DataSet> m_dicMBudgetPlanTemplateDA = m_objMItem.SelectBC(0, null, false, m_lstSelectsFirst, m_objFiltersFirst, null, null, null, null);
            if (m_objMItem.Message == string.Empty)
            {
                DataRow m_dataRow = m_dicMBudgetPlanTemplateDA[0].Tables[0].Rows[0];

                ItemVM m_objItem = new ItemVM();
                m_objItem.ItemDesc = m_dataRow[ItemVM.Prop.ItemDesc.Name].ToString();
                m_objItem.ItemID = m_dataRow[ItemVM.Prop.ItemID.Name].ToString();
                //m_objItem.Version = Convert.ToInt16(m_dataRow[ItemVersionVM.Prop.Version.Name].ToString());
                m_objItem.ItemTypeID = m_dataRow[ItemVersionVM.Prop.ItemTypeID.Name].ToString();
                m_objItem.ItemGroupDesc = m_dataRow[ItemVersionVM.Prop.ItemGroupDesc.Name].ToString();


                m_objItemPriceVendorPeriod.ItemDesc = m_objItem.ItemDesc;
                m_objItemPriceVendorPeriod.ItemID = m_objItem.ItemID;
                m_objItemPriceVendorPeriod.ItemTypeID = m_objItem.ItemTypeID;
                m_objItemPriceVendorPeriod.ItemGroupDesc = m_objItem.ItemGroupDesc;
                m_objItemPriceVendorPeriod.ItemPriceID = ItemComparisonDetailID;
                
                m_node = createNodeVersion(m_objItemPriceVendorPeriod, false);//, isdefaultCheckval);

                #region CheckChildren
                //CheckChildren
                m_node = CheckChildren(m_objItemPriceVendorPeriod.ItemID, m_objItemPriceVendorPeriod.ItemTypeID, ItemComparisonDetailID, m_node);
                if (m_node.Children.Count > 0)
                    m_node.Expandable = true;
                else
                    m_node.Expandable = false;
                #endregion
            }
            else
            {
                m_node = null;
            }
            return this.Store(m_node);
        }
        public ActionResult GetNodeAppendNew()
        {
            string thisItemDesc = string.Empty;
            string thisItem = string.Empty;
            //string thisItemTypeId = string.Empty;
            //string ParentItem = string.Empty;
            //int thisVersion=0;
            //string ParentItemTypeID = string.Empty;
            //int ParentVersion=0; int ParentSequence=0; int Sequence=0;
            //string BudgetTemplateID = string.Empty;
            //bool isdefaultCheck=true;
            Node m_node = new Node();

            bool isdefaultCheckval = false;
            bool isEnableDefault = false;


            List<string> m_lstSelectedRow = new List<string>();
            m_lstSelectedRow = JSON.Deserialize<List<string>>(this.Request.Params["ItemID"]);

            List<string> m_lstMessage = new List<string>();
            if (!m_lstSelectedRow.Any())
                m_lstMessage.Add("Some of Item Can't be Found");

            ItemPriceVendorPeriodVM m_objParentItemPriceVendorPeriod = new ItemPriceVendorPeriodVM();
            Dictionary<string, List<object>> m_objFiltersFirst = new Dictionary<string, List<object>>();
            MItemDA m_objVersion = new MItemDA();
            m_objVersion.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstSelectsFirst = new List<string>();
            m_lstSelectsFirst.Add(ItemVM.Prop.ItemDesc.Map);
            m_lstSelectsFirst.Add(ItemVM.Prop.ItemID.Map);
            m_lstSelectsFirst.Add(ItemVM.Prop.Version.Map);
            m_lstSelectsFirst.Add(ItemVM.Prop.ItemTypeID.Map);
            m_lstSelectsFirst.Add(ItemVM.Prop.ItemGroupDesc.Map);

            List<object> m_lstFilterss = new List<object>();
            m_lstFilterss.Add(Operator.Equals);
            m_lstFilterss.Add(thisItem);
            m_objFiltersFirst.Add(ItemVM.Prop.ItemID.Map, m_lstFilterss);

            //List<object> m_lstFiltersv = new List<object>();
            //m_lstFiltersv.Add(Operator.Equals);
            //m_lstFiltersv.Add(thisVersion);
            //m_objFiltersFirst.Add(ItemVM.Prop.Version.Map, m_lstFiltersv);

            Dictionary<int, DataSet> m_dicMBudgetPlanTemplateDA = m_objVersion.SelectBC(0, null, false, m_lstSelectsFirst, m_objFiltersFirst, null, null, null, null);
            if (m_objVersion.Message == string.Empty)
            {
                DataRow m_dataRow = m_dicMBudgetPlanTemplateDA[0].Tables[0].Rows[0];

                ItemVM m_objitemVersion = new ItemVM();
                m_objitemVersion.ItemDesc = m_dataRow[ItemVM.Prop.ItemDesc.Name].ToString();
                m_objitemVersion.ItemID = m_dataRow[ItemVM.Prop.ItemID.Name].ToString();
                m_objitemVersion.Version = Convert.ToInt16(m_dataRow[ItemVM.Prop.Version.Name].ToString());
                m_objitemVersion.ItemTypeID = m_dataRow[ItemVM.Prop.ItemTypeID.Name].ToString();
                m_objitemVersion.ItemGroupDesc = m_dataRow[ItemVM.Prop.ItemGroupDesc.Name].ToString();

                m_objParentItemPriceVendorPeriod.ItemDesc = m_objitemVersion.ItemDesc;
                m_objParentItemPriceVendorPeriod.ItemTypeID = m_objitemVersion.ItemTypeID;
                m_objParentItemPriceVendorPeriod.ItemGroupDesc = m_objitemVersion.ItemGroupDesc;
                m_objParentItemPriceVendorPeriod.ItemID = m_objitemVersion.ItemID;
                //m_objParentItemComparisonDetails.Version = m_objitemVersion.Version;

                //public string BudgetPlanTemplateID = ItemPriceID
                //public string ItemID = VendorID
                //public int Version = TaskID
                //public int Sequence = StatusID

                //m_objParentItemComparisonDetails.ItemDesc = m_objitemVersion.ItemDesc;
                //m_objParentItemComparisonDetails.ItemTypeID = m_objitemVersion.ItemTypeID;
                //m_objParentItemComparisonDetails.ItemGroupDesc = m_objitemVersion.ItemGroupDesc;
                //m_objParentItemComparisonDetails.ItemID = m_objitemVersion.ItemID;
                //m_objParentItemComparisonDetails.Version = m_objitemVersion.Version;
                //m_objParentItemComparisonDetails.BudgetPlanTemplateID = BudgetTemplateID;
                //m_objParentItemComparisonDetails.Sequence = Sequence + 1;
                //m_objParentItemPriceVendorPeriod.ParentItemID = ParentItem;
                //m_objParentItemPriceVendorPeriod.ParentVersion = ParentVersion;
                //m_objParentItemPriceVendorPeriod.ParentSequence = ParentSequence;
                //m_objParentItemPriceVendorPeriod.ParentItemTypeID = ParentItemTypeID;


                //m_objParentItemComparisonDetails.EnableDefault = isEnableDefault;
                //Sequence++;
                m_node = createNodeVersion(m_objParentItemPriceVendorPeriod, false);//, isdefaultCheckval);

                #region CheckChildren
                ////CheckChildren
                ////m_node = CheckChildren(m_objParentItemPriceVendorPeriod.ItemID, m_objParentItemPriceVendorPeriod.ItemTypeID, m_objParentItemPriceVendorPeriod.Version, ref Sequence, BudgetTemplateID, m_node, m_lstUPA, m_lstUPAAHS, ref isEnableDefault, ref isdefaultCheck);
                ////private Node CheckChildren(string ParentItemID, string ParentItemTypeID, ref int Sequence, string BudgetTemplateID, Node ParentNode, ref bool isEnableDefault, ref bool isdefaultcheck)
                //m_node = CheckChildren(m_objParentItemPriceVendorPeriod.ItemID, m_objParentItemPriceVendorPeriod.ItemTypeID, m_node, ref isEnableDefault);//, ref Sequence, BudgetTemplateID, ref isdefaultCheck);
                //if (m_node.Children.Count > 0)
                //    m_node.Expandable = true;
                //else
                //    m_node.Expandable = false;
                #endregion
            }
            else
            {
                m_node = null;
            }

            return this.Store(m_node);
        }
        private Node createNodeVersion(ItemPriceVendorPeriodVM parent, bool isFromSelect)//, bool isdefaultCheck)
        {
            Node m_node = new Node();

            m_node.Icon = Icon.Folder;
            m_node.Text = parent.ItemID;
            m_node.AttributesObject = new
            {
                number = "",
                itemdesc = parent.ItemDesc,
                itemid = parent.ItemID,
                itemgroupdesc = parent.ItemGroupDesc,
                itemtypeid = parent.ItemTypeID
            };

            return m_node;
        }
        private Node CheckChildren(string ParentItemID, string ParentItemTypeID, string ItemComparisonDetailID, Node ParentNode)//, ref bool isEnableDefault, ref int Sequence, string BudgetTemplateID, ref bool isdefaultcheck)
        {
            bool isdefaultval = false;

            ItemPriceVendorPeriodVM m_objParentItemPriceVendorPeriod = new ItemPriceVendorPeriodVM();
            Node m_node = new Node();
            Dictionary<string, List<object>> m_objFiltersFirst = new Dictionary<string, List<object>>();
            List<string> m_lstSelectsFirst = new List<string>();

            m_lstSelectsFirst.Add(ItemVM.Prop.ItemDesc.Map);
            m_lstSelectsFirst.Add(ItemVM.Prop.ItemID.Map);
            m_lstSelectsFirst.Add(ItemVM.Prop.Version.Map);
            m_lstSelectsFirst.Add(ItemVM.Prop.ItemTypeID.Map);
            m_lstSelectsFirst.Add(ItemVM.Prop.ItemGroupDesc.Map);

            List<object> m_lstFilterss = new List<object>();
            m_lstFilterss.Add(Operator.Equals);
            m_lstFilterss.Add(ParentItemID);
            m_objFiltersFirst.Add(ItemVersionChildVM.Prop.ItemID.Map, m_lstFilterss);

            DItemVersionChildDA m_objVersionChild = new DItemVersionChildDA();
            m_objVersionChild.ConnectionStringName = Global.ConnStrConfigName;

            Dictionary<int, DataSet> m_dicMBudgetPlanTemplateChildDA = m_objVersionChild.SelectBC(0, null, false, m_lstSelectsFirst, m_objFiltersFirst, null, null, null, null);
            if (m_objVersionChild.Message == string.Empty && m_objVersionChild.AffectedRows > 0)
            {

                foreach (DataRow m_dataRow in m_dicMBudgetPlanTemplateChildDA[0].Tables[0].Rows)
                {
                    //bool enabledefaultForThis = false;
                    ItemVersionVM m_objitemVersion = new ItemVersionVM();
                    m_objitemVersion.ItemDesc = m_dataRow[ItemVersionChildVM.Prop.ItemDesc.Name].ToString();
                    m_objitemVersion.ItemID = m_dataRow[ItemVersionChildVM.Prop.ChildItemID.Name].ToString();
                    m_objitemVersion.Version = Convert.ToInt16(m_dataRow[ItemVersionChildVM.Prop.ChildVersion.Name].ToString());
                    m_objitemVersion.ItemTypeID = m_dataRow[ItemVersionChildVM.Prop.ItemTypeID.Name].ToString();
                    m_objitemVersion.ItemGroupDesc = m_dataRow[ItemVersionChildVM.Prop.ItemGroupDesc.Name].ToString();

                    m_objParentItemPriceVendorPeriod.ItemDesc = m_objitemVersion.ItemDesc;
                    m_objParentItemPriceVendorPeriod.ItemID = m_objitemVersion.ItemID;
                    m_objParentItemPriceVendorPeriod.ItemTypeID = m_objitemVersion.ItemTypeID;
                    m_objParentItemPriceVendorPeriod.ItemGroupDesc = m_objitemVersion.ItemGroupDesc;

                    m_objParentItemPriceVendorPeriod.IsDefault = false;
                    //Sequence++;
                    m_node = createNodeVersion(m_objParentItemPriceVendorPeriod, false);//, isdefaultval);

                    #region CheckChild_Loop
                    m_node = CheckChildren(m_objParentItemPriceVendorPeriod.ItemID, m_objParentItemPriceVendorPeriod.ItemTypeID, ItemComparisonDetailID, m_node);//, m_node, ref isEnableDefault);//, ref Sequence, BudgetTemplateID, ref isdefaultcheck);
                    ParentNode.Children.Add(m_node);
                    ParentNode.Expanded = true;
                    #endregion

                }
            }
            else
            {
                //ParentNode.Leaf = true;
            }
            return ParentNode;
        }
        
        #endregion

    }
}