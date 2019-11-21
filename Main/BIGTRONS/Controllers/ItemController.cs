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
using SWeb = System.Web;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;

namespace com.SML.BIGTRONS.Controllers
{
    public class ItemController : BaseController
    {
        private readonly string title = "Item";
        private readonly string dataSessionName = "FormData";

        #region Public Action

        public ActionResult Index()
        {
            base.Initialize();
            return View();
        }
        public ActionResult List()
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            if (Session[dataSessionName] != null)
                Session[dataSessionName] = null;

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
            MItemDA m_objMItemDA = new MItemDA();
            m_objMItemDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMItem = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMItem.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = ItemVM.Prop.Map(m_strDataIndex, false);
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

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.ItemTypeDesc.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.ItemTypeID.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.ItemGroupID.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.ItemGroupDesc.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.UoMID.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.UoMDesc.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.IsActive.MapAlias);

            List<string> m_lstGroup = new List<string>();
            m_lstGroup.Add(ItemVM.Prop.ItemID.Map);
            m_lstGroup.Add(ItemVM.Prop.ItemDesc.Map);
            m_lstGroup.Add(ItemVM.Prop.ItemTypeDesc.Map);
            m_lstGroup.Add(ItemVM.Prop.ItemTypeID.Map);
            m_lstGroup.Add(ItemVM.Prop.ItemGroupID.Map);
            m_lstGroup.Add(ItemVM.Prop.ItemGroupDesc.Map);
            m_lstGroup.Add(ItemVM.Prop.UoMID.Map);
            m_lstGroup.Add(ItemVM.Prop.UoMDesc.Map);
            m_lstGroup.Add(ItemVM.Prop.IsActive.Map);

            Dictionary<int, DataSet> m_dicMItemDA = m_objMItemDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, m_lstGroup, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpItemBL in m_dicMItemDA)
            {
                m_intCount = m_kvpItemBL.Key;
                break;
            }

            List<ItemVM> m_lstItemVM = new List<ItemVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                //List<string> m_lstSelect = new List<string>();
                //m_lstSelect.Add(ItemVM.Prop.ItemID.MapAlias);
                //m_lstSelect.Add(ItemVM.Prop.ItemDesc.MapAlias);
                //m_lstSelect.Add(ItemVM.Prop.ItemTypeDesc.MapAlias);
                //m_lstSelect.Add(ItemVM.Prop.ItemTypeID.MapAlias);
                //m_lstSelect.Add(ItemVM.Prop.ItemGroupID.MapAlias);
                //m_lstSelect.Add(ItemVM.Prop.ItemGroupDesc.MapAlias);
                //m_lstSelect.Add(ItemVM.Prop.UoMID.MapAlias);
                //m_lstSelect.Add(ItemVM.Prop.UoMDesc.MapAlias);
                //m_lstSelect.Add(ItemVM.Prop.IsActive.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(ItemVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMItemDA = m_objMItemDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, m_lstGroup, m_dicOrder, null);
                if (m_objMItemDA.Message == string.Empty)
                {
                    m_lstItemVM = (
                        from DataRow m_drMItemDA in m_dicMItemDA[0].Tables[0].Rows
                        select new ItemVM()
                        {
                            ItemID = m_drMItemDA[ItemVM.Prop.ItemID.Name].ToString(),
                            ItemDesc = m_drMItemDA[ItemVM.Prop.ItemDesc.Name].ToString(),
                            ItemTypeDesc = m_drMItemDA[ItemVM.Prop.ItemTypeDesc.Name].ToString(),
                            ItemGroupDesc = m_drMItemDA[ItemVM.Prop.ItemGroupDesc.Name].ToString(),
                            UoMDesc = m_drMItemDA[ItemVM.Prop.UoMDesc.Name].ToString(),
                            UoMID = m_drMItemDA[ItemVM.Prop.UoMID.Name].ToString(),
                            IsActive = Convert.ToBoolean(m_drMItemDA[ItemVM.Prop.IsActive.Name].ToString())
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstItemVM, m_intCount);
        }
        public ActionResult ReadBrowse(StoreRequestParameters parameters, bool FilterUPA)
        {
            MItemDA m_objMItemDA = new MItemDA();
            m_objMItemDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMItem = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<string> m_lstSelect = new List<string>();

            List<string> m_lstUPA = new List<string>();
            List<string> m_lstUPAChild = new List<string>();

            #region with filter UPA
            if (FilterUPA)
            {
                UConfigDA m_objUConfigDA = new UConfigDA();
                m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
                List<object> m_lstFilter = new List<object>();

                m_lstSelect = new List<string>();
                m_lstSelect.Add(ConfigVM.Prop.Desc1.MapAlias);

                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add("ItemTypeID");
                m_objFilter.Add(ConfigVM.Prop.Key1.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add("UnitPriceAnalysis");
                m_objFilter.Add(ConfigVM.Prop.Key2.Map, m_lstFilter);

                Dictionary<int, DataSet> m_dicUConfigDA = m_objUConfigDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
                if (m_objUConfigDA.Message == string.Empty)
                {
                    foreach (DataRow item in m_dicUConfigDA[0].Tables[0].Rows)
                    {
                        m_lstUPA.Add(item[ConfigVM.Prop.Desc1.Name].ToString());
                    }
                }

                m_lstUPA = m_lstUPA.Distinct().ToList();
            }
            #endregion

            m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMItem.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = ItemVM.Prop.Map(m_strDataIndex, false);
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

            #region with filter UPA
            if (FilterUPA)
            {
                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.In);
                m_lstFilter.Add(String.Join(",", m_lstUPA.Distinct()));
                m_objFilter.Add(ItemVM.Prop.ItemTypeID.Map, m_lstFilter);
            }
            #endregion

            Dictionary<int, DataSet> m_dicMItemDA = m_objMItemDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpItemGroupBL in m_dicMItemDA)
            {
                m_intCount = m_kvpItemGroupBL.Key;
                break;
            }

            List<ItemVM> m_lstItemVM = new List<ItemVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                m_lstSelect = new List<string>();
                m_lstSelect.Add(ItemVM.Prop.ItemID.MapAlias);
                m_lstSelect.Add(ItemVM.Prop.ItemDesc.MapAlias);
                m_lstSelect.Add(ItemVM.Prop.ItemGroupID.MapAlias);
                m_lstSelect.Add(ItemVM.Prop.ItemGroupDesc.MapAlias);
                m_lstSelect.Add(ItemVM.Prop.ItemTypeDesc.MapAlias);
                m_lstSelect.Add(ItemVM.Prop.HasParameter.MapAlias);
                m_lstSelect.Add(ItemVM.Prop.HasPrice.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(ItemVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMItemDA = m_objMItemDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMItemDA.Message == string.Empty)
                {
                    m_lstItemVM = (
                        from DataRow m_drMItemDA in m_dicMItemDA[0].Tables[0].Rows
                        select new ItemVM()
                        {
                            ItemID = m_drMItemDA[ItemVM.Prop.ItemID.Name].ToString(),
                            ItemDesc = m_drMItemDA[ItemVM.Prop.ItemDesc.Name].ToString(),
                            ItemGroupID = m_drMItemDA[ItemVM.Prop.ItemGroupID.Name].ToString(),
                            ItemGroupDesc = m_drMItemDA[ItemVM.Prop.ItemGroupDesc.Name].ToString(),
                            ItemTypeDesc = m_drMItemDA[ItemVM.Prop.ItemTypeDesc.Name].ToString(),
                            HasParameter = Convert.ToBoolean(m_drMItemDA[ItemVM.Prop.HasParameter.Name].ToString()),
                            HasPrice = Convert.ToBoolean(m_drMItemDA[ItemVM.Prop.HasPrice.Name].ToString())
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstItemVM, m_intCount);
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

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.None);
            m_lstFilter.Add(string.Empty);
            m_objFilter.Add(GetFilterPrice(new ItemPriceVM { RegionID = RegionID, ProjectID = ProjectID, ClusterID = ClusterID, UnitTypeID = UnitTypeID }), m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.LessThanEqual);
            m_lstFilter.Add(DateTime.Now.Date.ToString(Global.SqlDateFormat));
            m_objFilter.Add(ItemPriceVendorPeriodVM.Prop.ValidFrom.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.GreaterThanEqual);
            m_lstFilter.Add(DateTime.Now.Date.ToString(Global.SqlDateFormat));
            m_objFilter.Add(ItemPriceVendorPeriodVM.Prop.ValidTo.Map, m_lstFilter);

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
                            ItemID = m_drDItemPriceVendorPeriodDA[ItemVM.Prop.ItemID.Name].ToString(),
                            ItemDesc = m_drDItemPriceVendorPeriodDA[ItemVM.Prop.ItemDesc.Name].ToString(),
                            VendorDesc = (m_drDItemPriceVendorPeriodDA[ItemPriceVendorVM.Prop.VendorDesc.Name].ToString() == string.Empty ? m_drDItemPriceVendorPeriodDA[ConfigVM.Prop.Desc2.Name].ToString() : m_drDItemPriceVendorPeriodDA[ItemPriceVendorVM.Prop.VendorDesc.Name].ToString()),
                            Amount = Convert.ToDecimal(m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.Amount.Name].ToString()),
                            IsDefault = Convert.ToBoolean(m_drDItemPriceVendorPeriodDA[ItemPriceVendorVM.Prop.IsDefault.Name].ToString()),
                            VendorID = m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.VendorID.Name].ToString(),
                            DisplayPrice = m_boolValueObject
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstItemPriceVendorPeriodVM, m_intCount);
        }
        public ActionResult GetListParamterByItemGroupID(string FilterItemGroupID)
        {
            DItemGroupParameterDA m_objItemGroupParameterDA = new DItemGroupParameterDA();
            m_objItemGroupParameterDA.ConnectionStringName = Global.ConnStrConfigName;

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(FilterItemGroupID);
            m_objFilter.Add(ItemGroupParameterVM.Prop.ItemGroupID.Map, m_lstFilter);


            List<ItemGroupParameterVM> m_lstItemGroupParameterVM = new List<ItemGroupParameterVM>();

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemGroupParameterVM.Prop.ParameterID.MapAlias);
            m_lstSelect.Add(ItemGroupParameterVM.Prop.ParameterDesc.MapAlias);


            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();

            Dictionary<int, DataSet> m_dicItemGroupParameterDA = m_objItemGroupParameterDA.SelectBC(0, 30, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objItemGroupParameterDA.Message == string.Empty)
            {
                m_lstItemGroupParameterVM = (
                    from DataRow m_drMItemGroupDA in m_dicItemGroupParameterDA[0].Tables[0].Rows
                    select new ItemGroupParameterVM()
                    {
                        ParameterID = m_drMItemGroupDA[ParameterVM.Prop.ParameterID.Name].ToString(),
                        ParameterDesc = m_drMItemGroupDA[ParameterVM.Prop.ParameterDesc.Name].ToString()
                    }
                ).ToList();
            }
            return this.Store(m_lstItemGroupParameterVM);
        }
        public ActionResult Home()
        {
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 0;
            return this.Direct();
        }
        public ActionResult Add(string Caller)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Add)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ItemVM m_objItemVM = new ItemVM();
            m_objItemVM.ListItemParameterVM = new List<ItemParameterVM>();
            m_objItemVM.ListItemPriceVM = new List<ItemPriceVM>();

            ViewDataDictionary m_vddItem = new ViewDataDictionary();
            m_vddItem.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddItem.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
            if (Caller == General.EnumDesc(Buttons.ButtonDetail))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                Dictionary<string, object> m_dicSelectedRow = GetFormData(m_nvcParams);
                Session[dataSessionName] = JSON.Serialize(m_dicSelectedRow);
            }

            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objItemVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddItem,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult Detail(string Caller, string Selected, string ItemID)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            string m_strMessage = string.Empty;
            ItemVM m_objItemVM = new ItemVM();
            Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();
            if (Caller == General.EnumDesc(Buttons.ButtonCancel))
            {
                if (Session[dataSessionName] != null)
                {
                    try
                    {
                        m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Session[dataSessionName].ToString());
                    }
                    catch (Exception ex)
                    {
                        m_strMessage = ex.Message;
                    }
                    Session[dataSessionName] = null;
                }
                else
                    m_strMessage = General.EnumDesc(MessageLib.Unknown);
            }
            else if (Caller == General.EnumDesc(Buttons.ButtonList))
                m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
            else if (Caller == General.EnumDesc(Buttons.ButtonSave))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                m_dicSelectedRow = GetFormData(m_nvcParams, ItemID);
            }


            if (m_dicSelectedRow.Count > 0)
                m_objItemVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            m_objItemVM.ListItemParameterVM = new List<ItemParameterVM>();
            if (m_objItemVM != null)
                if (m_objItemVM.HasParameter) m_objItemVM.ListItemParameterVM = GetListItemParameter(m_objItemVM.ItemID, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            m_objItemVM.ListItemPriceVM = new List<ItemPriceVM>();
            if (m_objItemVM != null)
                if (m_objItemVM.HasPrice) m_objItemVM.ListItemPriceVM = GetListItemPrice(m_objItemVM.ItemID, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddItem = new ViewDataDictionary();
            m_vddItem.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objItemVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddItem,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult Update(string Caller, string Selected)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Update)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();
            if (Caller == General.EnumDesc(Buttons.ButtonList))
            {
                m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
            }
            else if (Caller == General.EnumDesc(Buttons.ButtonDetail))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                m_dicSelectedRow = GetFormData(m_nvcParams, string.Empty);
                Session[dataSessionName] = JSON.Serialize(m_dicSelectedRow);
            }

            string m_strMessage = string.Empty;
            ItemVM m_objItemVM = new ItemVM();
            if (m_dicSelectedRow.Count > 0)
                m_objItemVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            m_objItemVM.ListItemParameterVM = new List<ItemParameterVM>();
            if (m_objItemVM != null)
                if (m_objItemVM.HasParameter) m_objItemVM.ListItemParameterVM = GetListItemParameter(m_objItemVM.ItemID, ref m_strMessage);


            m_objItemVM.ListItemPriceVM = new List<ItemPriceVM>();
            if (m_objItemVM != null)
                if (m_objItemVM.HasPrice) m_objItemVM.ListItemPriceVM = GetListItemPrice(m_objItemVM.ItemID, ref m_strMessage);


            ViewDataDictionary m_vddItem = new ViewDataDictionary();
            m_vddItem.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddItem.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objItemVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddItem,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<ItemVM> m_lstSelectedRow = new List<ItemVM>();
            m_lstSelectedRow = JSON.Deserialize<List<ItemVM>>(Selected);

            MItemDA m_objMItemDA = new MItemDA();
            DItemParameterDA d_objDItemParameter = new DItemParameterDA();
            d_objDItemParameter.ConnectionStringName = Global.ConnStrConfigName;
            m_objMItemDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (ItemVM m_objItemVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifItemVM = m_objItemVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifItemVM in m_arrPifItemVM)
                    {
                        string m_strFieldName = m_pifItemVM.Name;
                        object m_objFieldValue = m_pifItemVM.GetValue(m_objItemVM);
                        if (m_objItemVM.IsKey(m_strFieldName))
                        {

                            MItem m_objMItem = new MItem();
                            m_objMItem.ItemID = m_objFieldValue.ToString();
                            m_objMItemDA.Data = m_objMItem;
                            m_objMItemDA.Select();

                            m_objMItem.IsActive = false;
                            m_objMItemDA.Update(false);
                        }
                        else break;
                    }

                    if (m_objMItemDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMItemDA.Message);
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                return this.Direct(false, ex.Message);

            }
            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Deleted));
                return this.Direct(true, string.Empty);
            }
            else
                return this.Direct(false, String.Join(Global.NewLineSeparated, m_lstMessage));

        }
        public ActionResult Browse(string ControlItemID, string ControlItemDesc, string ControlItemGroupID,
            string ControlItemGroupDesc, string ControlItemTypeDesc, string ControlHasParameter, string ControlVersionDesc,
            string ControlHasPrice, bool FilterUPA = false, string FilterItemID = "", string FilterItemDesc = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddItem = new ViewDataDictionary();
            m_vddItem.Add("Control" + ItemVM.Prop.ItemID.Name, ControlItemID);
            m_vddItem.Add("Control" + ItemVM.Prop.ItemDesc.Name, ControlItemDesc);
            m_vddItem.Add("Control" + ItemVM.Prop.ItemGroupID.Name, ControlItemGroupID);
            m_vddItem.Add("Control" + ItemVM.Prop.ItemGroupDesc.Name, ControlItemGroupDesc);
            m_vddItem.Add("Control" + ItemVM.Prop.ItemTypeDesc.Name, ControlItemTypeDesc);
            m_vddItem.Add("Control" + ItemVM.Prop.HasParameter.Name, ControlHasParameter);
            m_vddItem.Add("Control" + ItemVM.Prop.HasPrice.Name, ControlHasPrice);
            m_vddItem.Add("Control" + ItemVersionVM.Prop.VersionDesc.Name, ControlVersionDesc);
            m_vddItem.Add(ItemVM.Prop.ItemID.Name, FilterItemID);
            m_vddItem.Add(ItemVM.Prop.ItemDesc.Name, FilterItemDesc);
            m_vddItem.Add("FilterUPA", FilterUPA);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddItem,
                ViewName = "../Item/_Browse"
            };
        }

        public ActionResult BrowseItemPriceVendorPeriod(string ControlItemID, string ControlItemDesc, string ControlItemGroupID,
            string ControlItemGroupDesc, string ControlItemTypeDesc, string ControlHasParameter, string ControlVersionDesc,
            string ControlHasPrice, string ValueItemID = "", string RegionID = "", string ProjectID = "", string ClusterID = "", string UnitTypeID = "", string FilterItemID = "", string FilterItemDesc = "")
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddItem = new ViewDataDictionary();
            m_vddItem.Add("Control" + ItemVM.Prop.ItemID.Name, ControlItemID);
            m_vddItem.Add("Control" + ItemVM.Prop.ItemDesc.Name, ControlItemDesc);
            m_vddItem.Add("Control" + ItemVM.Prop.ItemGroupID.Name, ControlItemGroupID);
            m_vddItem.Add("Control" + ItemVM.Prop.ItemGroupDesc.Name, ControlItemGroupDesc);
            m_vddItem.Add("Control" + ItemVM.Prop.ItemTypeDesc.Name, ControlItemTypeDesc);
            m_vddItem.Add("Control" + ItemVM.Prop.HasParameter.Name, ControlHasParameter);
            m_vddItem.Add("Control" + ItemVM.Prop.HasPrice.Name, ControlHasPrice);
            m_vddItem.Add("Control" + ItemVersionVM.Prop.VersionDesc.Name, ControlVersionDesc);
            m_vddItem.Add(ItemVM.Prop.ItemID.Name, FilterItemID);
            m_vddItem.Add(ItemVM.Prop.ItemDesc.Name, FilterItemDesc);
            m_vddItem.Add("Value" + ItemPriceVM.Prop.ItemID.Name, ValueItemID);
            m_vddItem.Add(ItemPriceVM.Prop.RegionID.Name, RegionID);
            m_vddItem.Add(ItemPriceVM.Prop.ProjectID.Name, ProjectID);
            m_vddItem.Add(ItemPriceVM.Prop.ClusterID.Name, ClusterID);
            m_vddItem.Add(ItemPriceVM.Prop.UnitTypeID.Name, UnitTypeID);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddItem,
                ViewName = "../Item/ItemPrice/VendorPeriod/_Browse"
            };
        }
        public ActionResult Save(string Action)
        {
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MItemDA m_objMItemDA = new MItemDA();
            ItemVM m_objMItemVM = new ItemVM();
            DItemVersionDA m_objDItemVersionDA = new DItemVersionDA();
            MItem m_objMItem = new MItem();
            DItemParameterDA m_objDItemParameterDA = new DItemParameterDA();
            DItemPriceDA m_objDItemPriceDA = new DItemPriceDA();
            DItemPriceVendorDA m_objDItemPriceVendorDA = new DItemPriceVendorDA();
            DItemPriceVendorPeriodDA m_objDItemPriceVendorPeriodDA = new DItemPriceVendorPeriodDA();
            string m_strItemID = string.Empty;
            m_objMItemDA.ConnectionStringName = Global.ConnStrConfigName;
            ConfigVM m_objConfigVM = GetVendorConfig();

            string m_strTransName = "Item";
            object m_objDBConnection = null;
            m_objDBConnection = m_objMItemDA.BeginTrans(m_strTransName);
            string m_strMessage = string.Empty;

            try
            {
                m_strItemID = this.Request.Params[ItemVM.Prop.ItemID.Name];
                string m_strItemDesc = this.Request.Params[ItemVM.Prop.ItemDesc.Name];
                string m_strItemGroupID = this.Request.Params[ItemVM.Prop.ItemGroupID.Name];
                string m_strUoMID = this.Request.Params[ItemVM.Prop.UoMID.Name];
                string m_strIsActive = this.Request.Params[ItemVM.Prop.IsActive.Name];
                string m_strHasParameter = this.Request.Params[ItemVM.Prop.HasParameter.Name];
                string m_strHasPrice = this.Request.Params[ItemVM.Prop.HasPrice.Name];

                string m_strListItemParamterVM = this.Request.Params[ItemVM.Prop.ListItemParameterVM.Name];
                string m_strListItemPriceVM = this.Request.Params[ItemVM.Prop.ListItemPriceVM.Name];

                List<ItemParameterVM> m_lstItemParameterVM = JSON.Deserialize<List<ItemParameterVM>>(m_strListItemParamterVM);
                List<ItemPriceVM> m_lstItemPriceVM = JSON.Deserialize<List<ItemPriceVM>>(m_strListItemPriceVM);

                m_objMItemVM.ItemID = m_strItemID;
                m_objMItemVM.ItemDesc = m_strItemDesc;
                m_objMItemVM.ItemGroupID = m_strItemGroupID;
                m_objMItemVM.UoMID = m_strUoMID;
                m_objMItemVM.HasParameter = bool.Parse(m_strHasParameter);
                m_objMItemVM.HasPrice = bool.Parse(m_strHasPrice);
                m_objMItemVM.IsActive = bool.Parse(m_strIsActive);
                m_objMItemVM.ListItemParameterVM = m_lstItemParameterVM;
                m_objMItemVM.ListItemPriceVM = m_lstItemPriceVM;

                m_lstMessage = IsSaveValid(Action, m_objMItemVM);
                if (m_lstMessage.Count <= 0)
                {
                    List<string> success_status = new List<string>();


                    #region Item
                    m_objMItem.ItemID = m_objMItemVM.ItemID;
                    m_objMItemDA.Data = m_objMItem;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objMItemDA.Select();

                    m_objMItem.ItemDesc = m_objMItemVM.ItemDesc;
                    m_objMItem.ItemGroupID = m_objMItemVM.ItemGroupID;
                    m_objMItem.UoMID = m_objMItemVM.UoMID;
                    m_objMItem.IsActive = m_objMItemVM.IsActive;

                    m_objDBConnection = m_objMItemDA.BeginTrans(m_strTransName);
                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objMItemDA.Insert(true, m_objDBConnection);
                    else
                        m_objMItemDA.Update(true, m_objDBConnection);

                    if (!m_objMItemDA.Success || m_objMItemDA.Message != string.Empty)
                    {
                        m_objMItemDA.EndTrans(ref m_objDBConnection, m_strTransName);
                        return this.Direct(false, m_objMItemDA.Message);
                    }
                    m_strItemID = m_objMItemDA.Data.ItemID;
                    #endregion

                    #region DItemParameter

                    if (m_objMItemVM.HasParameter)
                    {
                        m_objDItemParameterDA.ConnectionStringName = Global.ConnStrConfigName;

                        if (Action == General.EnumDesc(Buttons.ButtonUpdate))
                        {
                            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_strItemID);
                            m_objFilter.Add(ItemParameterVM.Prop.ItemID.Map, m_lstFilter);

                            m_objDItemParameterDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                        }

                        foreach (ItemParameterVM objItemParameterVM in m_lstItemParameterVM)
                        {
                            DItemParameter m_objDItemParameter = new DItemParameter();

                            m_objDItemParameter.ItemID = m_strItemID;
                            m_objDItemParameter.ItemGroupID = m_objMItemVM.ItemGroupID;
                            m_objDItemParameter.ParameterID = objItemParameterVM.ParameterID;
                            m_objDItemParameter.Value = objItemParameterVM.Value;

                            m_objDItemParameterDA.Data = m_objDItemParameter;
                            m_objDItemParameterDA.Insert(true, m_objDBConnection);

                            if (!m_objDItemParameterDA.Success || m_objDItemParameterDA.Message != string.Empty)
                            {
                                m_objDItemParameterDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                                return this.Direct(false, m_objDItemParameterDA.Message);
                            }
                        }
                    }
                    #endregion

                    #region DItemPrice
                    if (m_objMItemVM.HasPrice)
                    {
                        m_objDItemPriceDA.ConnectionStringName = Global.ConnStrConfigName;

                        if (Action == General.EnumDesc(Buttons.ButtonUpdate))
                        {
                            #region Get Item Price
                            List<string> m_lstSelect = new List<string>();
                            m_lstSelect.Add(ItemPriceVM.Prop.ItemPriceID.MapAlias);

                            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_strItemID);
                            m_objFilter.Add(ItemPriceVM.Prop.ItemID.Map, m_lstFilter);

                            Dictionary<int, DataSet> m_dicDItemPriceDA = m_objDItemPriceDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
                            List<ItemPriceVM> m_listItemPriceVM = new List<ItemPriceVM>();
                            if (m_objDItemPriceDA.Success)
                            {
                                m_listItemPriceVM = (
                                from DataRow m_drDItemPriceDA in m_dicDItemPriceDA[0].Tables[0].Rows
                                select new ItemPriceVM()
                                {
                                    ItemPriceID = m_drDItemPriceDA[ItemPriceVM.Prop.ItemPriceID.Name].ToString(),
                                }).Distinct().ToList();
                            }
                            #endregion

                            List<ItemPriceVM> m_delItemPriceVM = m_listItemPriceVM.Where(d => !m_lstItemPriceVM.Select(x => x.ItemPriceID).Contains(d.ItemPriceID)).ToList();

                            foreach (ItemPriceVM m_itemPriceVM in m_delItemPriceVM)
                            {
                                m_objFilter = new Dictionary<string, List<object>>();
                                m_lstFilter = new List<object>();
                                m_lstFilter.Add(Operator.Equals);
                                m_lstFilter.Add(m_itemPriceVM.ItemPriceID);
                                m_objFilter.Add(ItemPriceVM.Prop.ItemPriceID.Map, m_lstFilter);

                                m_objDItemPriceDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                            }
                        }

                        foreach (ItemPriceVM objItemPriceVM in m_lstItemPriceVM)
                        {
                            DItemPrice m_objDItemPrice = new DItemPrice();

                            m_objDItemPrice.ItemPriceID = objItemPriceVM.ItemPriceID;

                            m_objDItemPriceDA.Data = m_objDItemPrice;
                            m_objDItemPriceDA.Select();

                            m_objDItemPrice.PriceTypeID = objItemPriceVM.PriceTypeID;
                            m_objDItemPrice.ProjectID = objItemPriceVM.ProjectID;
                            m_objDItemPrice.RegionID = objItemPriceVM.RegionID;
                            m_objDItemPrice.UnitTypeID = objItemPriceVM.UnitTypeID;
                            m_objDItemPrice.ClusterID = objItemPriceVM.ClusterID;

                            if (string.IsNullOrEmpty(m_objDItemPriceDA.Data.ItemPriceID))
                            {
                                m_objDItemPrice.ItemID = m_strItemID;
                                m_objDItemPrice.ItemPriceID = Guid.NewGuid().ToString().Replace("-", "");

                                m_objDItemPriceDA.Insert(true, m_objDBConnection);
                            }
                            else
                                m_objDItemPriceDA.Update(true, m_objDBConnection);

                            if (!m_objDItemPriceDA.Success || m_objDItemPriceDA.Message != string.Empty)
                            {
                                m_objDItemPriceDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                                return this.Direct(false, m_objDItemPriceDA.Message);
                            }

                            string m_strItemPriceID = m_objDItemPriceDA.Data.ItemPriceID;

                            #region DItemPriceVendor

                            if (objItemPriceVM.ListItemPriceVendorVM != null)
                            {
                                foreach (ItemPriceVendorVM objItemPriceVendorVM in objItemPriceVM.ListItemPriceVendorVM)
                                {
                                    DItemPriceVendor m_objDItemPriceVendor = new DItemPriceVendor();

                                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                                    List<object> m_lstFilter = new List<object>();

                                    m_lstFilter.Add(Operator.Equals);
                                    m_lstFilter.Add(m_strItemPriceID);
                                    m_objFilter.Add(ItemPriceVendorVM.Prop.ItemPriceID.Map, m_lstFilter);

                                    m_lstFilter = new List<object>();
                                    m_lstFilter.Add(Operator.Equals);
                                    m_lstFilter.Add((m_objConfigVM.Desc1 == objItemPriceVendorVM.VendorID ? "" : objItemPriceVendorVM.VendorID));
                                    m_objFilter.Add(ItemPriceVendorVM.Prop.VendorID.Map, m_lstFilter);

                                    m_objDItemPriceVendorDA.DeleteBC(m_objFilter, true, m_objDBConnection);



                                    if (m_objDItemPriceVendorDA.Success)
                                    {
                                        m_objDItemPriceVendor.IsDefault = objItemPriceVendorVM.IsDefault;
                                        //m_objDItemPriceVendor.ItemPriceID = objItemPriceVendorVM.ItemPriceID ?? string.Empty;
                                        m_objDItemPriceVendor.VendorID = (m_objConfigVM.Desc1 == objItemPriceVendorVM.VendorID ? "" : objItemPriceVendorVM.VendorID);
                                        m_objDItemPriceVendor.ItemPriceID = m_strItemPriceID;

                                        m_objDItemPriceVendorDA.Data = m_objDItemPriceVendor;
                                        m_objDItemPriceVendorDA.Insert(true, m_objDBConnection);
                                        if (!m_objDItemPriceVendorDA.Success)
                                        {
                                            m_objDItemPriceVendorDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                                            return this.Direct(false, m_objDItemPriceVendorDA.Message);
                                        }
                                    }
                                    else
                                    {
                                        m_objDItemPriceVendorDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                                        return this.Direct(false, m_objDItemPriceVendorDA.Message);
                                    }


                                }
                            }
                            #endregion

                            #region DItemPriceVendorPeriod
                            if (objItemPriceVM.ListItemPriceVendorPeriodVM != null)
                            {

                                foreach (ItemPriceVendorPeriodVM objItemPriceVendorPeriodVM in objItemPriceVM.ListItemPriceVendorPeriodVM)
                                {
                                    DItemPriceVendorPeriod m_objDItemPriceVendorPeriod = new DItemPriceVendorPeriod();

                                    //m_objDItemPriceVendorPeriod.ItemPriceID = objItemPriceVendorPeriodVM.ItemPriceID ?? string.Empty;
                                    m_objDItemPriceVendorPeriod.ItemPriceID = m_strItemPriceID;
                                    //m_objDItemPriceVendorPeriod.ValidTo = objItemPriceVendorPeriodVM.ValidTo;
                                    //m_objDItemPriceVendorPeriod.ValidFrom = objItemPriceVendorPeriodVM.ValidFrom;


                                    //m_objDItemPriceVendorPeriodDA.Select();
                                    //DItemPriceVendorPeriod m_objDItemPriceVendorPeriod = new DItemPriceVendorPeriod();

                                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                                    List<object> m_lstFilter = new List<object>();

                                    m_lstFilter.Add(Operator.Equals);
                                    m_lstFilter.Add(m_strItemPriceID);
                                    m_objFilter.Add(ItemPriceVendorPeriodVM.Prop.ItemPriceID.Map, m_lstFilter);

                                    m_lstFilter = new List<object>();
                                    m_lstFilter.Add(Operator.Equals);
                                    m_lstFilter.Add((m_objConfigVM.Desc1 == objItemPriceVendorPeriodVM.VendorID ? "" : objItemPriceVendorPeriodVM.VendorID));
                                    m_objFilter.Add(ItemPriceVendorPeriodVM.Prop.VendorID.Map, m_lstFilter);

                                    m_lstFilter = new List<object>();
                                    m_lstFilter.Add(Operator.Equals);
                                    m_lstFilter.Add(objItemPriceVendorPeriodVM.ValidFrom);
                                    m_objFilter.Add(ItemPriceVendorPeriodVM.Prop.ValidFrom.Map, m_lstFilter);

                                    m_objDItemPriceVendorPeriodDA.DeleteBC(m_objFilter, true, m_objDBConnection);

                                    if (m_objDItemPriceVendorPeriodDA.Success)
                                    {

                                        m_objDItemPriceVendorPeriod.VendorID = (m_objConfigVM.Desc1 == objItemPriceVendorPeriodVM.VendorID ? "" : objItemPriceVendorPeriodVM.VendorID);
                                        m_objDItemPriceVendorPeriod.ValidTo = objItemPriceVendorPeriodVM.ValidTo;
                                        m_objDItemPriceVendorPeriod.ValidFrom = objItemPriceVendorPeriodVM.ValidFrom;
                                        m_objDItemPriceVendorPeriod.CurrencyID = objItemPriceVendorPeriodVM.CurrencyID;
                                        m_objDItemPriceVendorPeriod.Amount = objItemPriceVendorPeriodVM.Amount;


                                        m_objDItemPriceVendorPeriodDA.Data = m_objDItemPriceVendorPeriod;
                                        m_objDItemPriceVendorPeriodDA.Insert(true, m_objDBConnection);
                                    }

                                    if (!m_objDItemPriceVendorPeriodDA.Success)
                                    {
                                        m_objDItemPriceVendorPeriodDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                                        return this.Direct(false, m_objDItemPriceVendorPeriodDA.Message);
                                    }

                                }
                            }
                            #endregion
                        }


                    }
                    #endregion

                    #region DItemVersion
                    DItemVersion m_objDItemVersion = new DItemVersion();

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                    {
                        m_objDItemVersion.ItemID = m_strItemID;
                        m_objDItemVersion.VersionDesc = m_objMItemVM.ItemDesc;
                        m_objDItemVersion.Version = 1;
                        m_objDItemVersionDA.Data = m_objDItemVersion;

                        m_objDItemVersionDA.Insert(true, m_objDBConnection);

                        if (!m_objDItemVersionDA.Success || m_objDItemVersionDA.Message != string.Empty)
                        {
                            m_objDItemVersionDA.EndTrans(ref m_objDBConnection, m_strTransName);
                            return this.Direct(false, m_objDItemVersionDA.Message);
                        }
                    }
                    else
                    {
                        DItemVersionDA m_objDItemVersionDANew = new DItemVersionDA();
                        ItemVersionVM m_objDItemVersionVM = new ItemVersionVM();
                        Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

                        List<object> m_lstFilter = new List<object>();
                        m_lstFilter.Add(Operator.Equals);
                        m_lstFilter.Add(m_strItemID);
                        m_objFilter.Add(ItemVersionVM.Prop.ItemID.Map, m_lstFilter);

                        List<string> m_lstSelect = new List<string>();
                        m_lstSelect.Add(ItemVersionVM.Prop.ItemID.MapAlias);
                        m_lstSelect.Add(ItemVersionVM.Prop.Version.MapAlias);

                        Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                        m_dicOrder.Add(ItemVersionVM.Prop.Version.Map, OrderDirection.Descending);

                        m_objDItemVersionDANew.ConnectionStringName = Global.ConnStrConfigName;
                        Dictionary<int, DataSet> m_dicMItemVersionChildDA = m_objDItemVersionDANew.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                        if (m_objDItemVersionDANew.Success)
                        {
                            foreach (DataRow m_drMItemVersionChildDA in m_dicMItemVersionChildDA[0].Tables[0].Rows)
                            {
                                m_objDItemVersionVM.ItemID = m_drMItemVersionChildDA[ItemVersionVM.Prop.ItemID.Name].ToString();
                                m_objDItemVersionVM.Version = (int)m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Version.Name];
                            }

                        }

                        m_objDItemVersion.ItemID = m_strItemID;
                        m_objDItemVersion.Version = m_objDItemVersionVM.Version;
                        m_objDItemVersionDA.Data = m_objDItemVersion;
                        m_objDItemVersionDA.Select();
                        m_objDItemVersion.VersionDesc = m_objMItemVM.ItemDesc;

                        m_objDItemVersionDA.Update(true, m_objDBConnection);

                        if (!m_objDItemVersionDA.Success || m_objDItemVersionDA.Message != string.Empty)
                        {
                            m_objDItemVersionDA.EndTrans(ref m_objDBConnection, m_strTransName);
                            return this.Direct(false, m_objDItemVersionDA.Message);
                        }
                    }


                    #endregion

                    if (!m_objMItemDA.Success || m_objMItemDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMItemDA.Message);
                    else
                        m_objMItemDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                m_objMItemDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
            }
            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
                return Detail(General.EnumDesc(Buttons.ButtonSave), null, m_strItemID);
            }
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);
        }

        #region Item Price Action
        public ActionResult AddItemPrice(string Caller)
        {

            ItemPriceVM m_objItemPriceVM = new ItemPriceVM();
            m_objItemPriceVM.ListItemPriceVendorPeriodVM = new List<ItemPriceVendorPeriodVM>();
            m_objItemPriceVM.ListItemPriceVendorVM = new List<ItemPriceVendorVM>();

            ConfigVM m_objConfigVM = GetVendorConfig();
            ViewDataDictionary m_vddItemPrice = new ViewDataDictionary();
            m_vddItemPrice.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddItemPrice.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
            m_vddItemPrice.Add(ItemPriceVendorVM.Prop.VendorID.Name, m_objConfigVM.Desc1);
            m_vddItemPrice.Add(ItemPriceVendorVM.Prop.VendorDesc.Name, m_objConfigVM.Desc2);
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 2;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageTwo),
                Model = m_objItemPriceVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddItemPrice,
                ViewName = "ItemPrice/_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult DetailItemPrice(string Caller, string Selected)
        {

            ItemPriceVM m_objItemPriceVM = new ItemPriceVM();
            Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();
            if (Caller == General.EnumDesc(Buttons.ButtonList))
            {
                m_objItemPriceVM = JSON.Deserialize<ItemPriceVM>(Selected);
            }
            else if (Caller == General.EnumDesc(Buttons.ButtonDetail))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                m_objItemPriceVM = GetFormItemPriceData(m_nvcParams);
            }

            if (!string.IsNullOrEmpty(m_objItemPriceVM.ItemPriceID))
            {
                m_objItemPriceVM.ListItemPriceVendorVM = GetListItemPriceVendor(m_objItemPriceVM.ItemPriceID);
                if (m_objItemPriceVM.ListItemPriceVendorVM == null)
                    m_objItemPriceVM.ListItemPriceVendorVM = new List<ItemPriceVendorVM>();

                m_objItemPriceVM.ListItemPriceVendorPeriodVM = GetListItemPriceVendorPeriod(m_objItemPriceVM.ItemPriceID);
                if (m_objItemPriceVM.ListItemPriceVendorPeriodVM == null)
                    m_objItemPriceVM.ListItemPriceVendorPeriodVM = new List<ItemPriceVendorPeriodVM>();
            }

            ConfigVM m_objConfigVM = GetVendorConfig();
            ViewDataDictionary m_vddItemPrice = new ViewDataDictionary();
            m_vddItemPrice.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddItemPrice.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
            m_vddItemPrice.Add(ItemPriceVendorVM.Prop.VendorID.Name, m_objConfigVM.Desc1);
            m_vddItemPrice.Add(ItemPriceVendorVM.Prop.VendorDesc.Name, m_objConfigVM.Desc2);
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 2;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageTwo),
                Model = m_objItemPriceVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddItemPrice,
                ViewName = "ItemPrice/_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult UpdateItemPrice(string Caller, string Selected)
        {

            ItemPriceVM m_objItemPriceVM = new ItemPriceVM();
            if (Caller == General.EnumDesc(Buttons.ButtonList))
            {
                m_objItemPriceVM = JSON.Deserialize<ItemPriceVM>(Selected);
            }
            else if (Caller == General.EnumDesc(Buttons.ButtonDetail))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                m_objItemPriceVM = GetFormItemPriceData(m_nvcParams);
            }

            string m_strMessage = string.Empty;

            if (!string.IsNullOrEmpty(m_objItemPriceVM.ItemPriceID))
            {
                m_objItemPriceVM.ListItemPriceVendorVM = GetListItemPriceVendor(m_objItemPriceVM.ItemPriceID);
                if (m_objItemPriceVM.ListItemPriceVendorVM == null)
                    m_objItemPriceVM.ListItemPriceVendorVM = new List<ItemPriceVendorVM>();

                m_objItemPriceVM.ListItemPriceVendorPeriodVM = GetListItemPriceVendorPeriod(m_objItemPriceVM.ItemPriceID);
                if (m_objItemPriceVM.ListItemPriceVendorPeriodVM == null)
                    m_objItemPriceVM.ListItemPriceVendorPeriodVM = new List<ItemPriceVendorPeriodVM>();
            }

            ConfigVM m_objConfigVM = GetVendorConfig();
            ViewDataDictionary m_vddItemPrice = new ViewDataDictionary();
            m_vddItemPrice.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddItemPrice.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            m_vddItemPrice.Add(ItemPriceVendorVM.Prop.VendorID.Name, m_objConfigVM.Desc1);
            m_vddItemPrice.Add(ItemPriceVendorVM.Prop.VendorDesc.Name, m_objConfigVM.Desc2);
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 2;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageTwo),
                Model = m_objItemPriceVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddItemPrice,
                ViewName = "ItemPrice/_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult CancelItemPrice(string Caller)
        {

            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return this.Direct();
        }
        public ActionResult DeleteItemPrice(string Selected)
        {

            Global.ShowInfoAlert("Item Price", General.EnumDesc(MessageLib.Deleted));
            return this.Direct();
        }
        #endregion

        #region Item Price Vendor Action
        public ActionResult DeleteItemPriceVendor(string Selected)
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            string m_strVendorID = string.Empty;
            ConfigVM m_objConfigVM = GetVendorConfig();

            List<ItemPriceVendorVM> m_lstSelectedRow = new List<ItemPriceVendorVM>();
            m_lstSelectedRow = JSON.Deserialize<List<ItemPriceVendorVM>>(Selected);

            DItemPriceVendorDA m_objDItemPriceVendorDA = new DItemPriceVendorDA();
            m_objDItemPriceVendorDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (ItemPriceVendorVM m_objDItemPriceVendorVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifItemPriceVendorVM = m_objDItemPriceVendorVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifItemPriceVendorVM in m_arrPifItemPriceVendorVM)
                    {
                        string m_strFieldName = m_pifItemPriceVendorVM.Name;
                        object m_objFieldValue = m_pifItemPriceVendorVM.GetValue(m_objDItemPriceVendorVM) ?? string.Empty;
                        if (m_objDItemPriceVendorVM.IsKey(m_strFieldName))
                        {
                            if (m_strFieldName.Equals(ItemPriceVendorVM.Prop.VendorID.Name))
                            {
                                m_strVendorID = (m_objConfigVM.Desc1 == m_objFieldValue.ToString() ? "" : m_objFieldValue.ToString());
                                m_objFieldValue = m_strVendorID;
                            }

                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(ItemPriceVendorVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objDItemPriceVendorDA.DeleteBC(m_objFilter, false);
                    if (m_objDItemPriceVendorDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDItemPriceVendorDA.Message);
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                return this.Direct(false, ex.Message);

            }
            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfoAlert("Item Price Vendor", General.EnumDesc(MessageLib.Deleted));
                return this.Direct(true, string.Empty);
            }
            else
                return this.Direct();

        }
        #endregion

        #region Item Price Vendor Period Action
        public ActionResult DeleteItemPriceVendorPeriod(string Selected)
        {
            string m_strVendorID = string.Empty;
            ConfigVM m_objConfigVM = GetVendorConfig();

            List<ItemPriceVendorPeriodVM> m_lstSelectedRow = new List<ItemPriceVendorPeriodVM>();
            m_lstSelectedRow = JSON.Deserialize<List<ItemPriceVendorPeriodVM>>(Selected);

            DItemPriceVendorPeriodDA m_objDItemPriceVendorPeriodDA = new DItemPriceVendorPeriodDA();
            m_objDItemPriceVendorPeriodDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (ItemPriceVendorPeriodVM m_objDItemPriceVendorPeriodVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifItemPriceVendorPeriodVM = m_objDItemPriceVendorPeriodVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifItemPriceVendorVM in m_arrPifItemPriceVendorPeriodVM)
                    {
                        string m_strFieldName = m_pifItemPriceVendorVM.Name;
                        object m_objFieldValue = m_pifItemPriceVendorVM.GetValue(m_objDItemPriceVendorPeriodVM) ?? string.Empty;
                        if (m_objDItemPriceVendorPeriodVM.IsKey(m_strFieldName))
                        {
                            if (m_strFieldName.Equals(ItemPriceVendorVM.Prop.VendorID.Name))
                            {
                                m_strVendorID = (m_objConfigVM.Desc1 == m_objFieldValue.ToString() ? "" : m_objFieldValue.ToString());
                                m_objFieldValue = m_strVendorID;
                            }

                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(ItemPriceVendorPeriodVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objDItemPriceVendorPeriodDA.DeleteBC(m_objFilter, false);
                    if (m_objDItemPriceVendorPeriodDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDItemPriceVendorPeriodDA.Message);
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                return this.Direct(false, ex.Message);

            }
            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfoAlert("Item Price Vendor Period", General.EnumDesc(MessageLib.Deleted));
                return this.Direct(true, string.Empty);
            }
            else
                //return this.Direct(false, String.Join(Global.NewLineSeparated, m_lstMessage));
                return this.Direct();
        }
        #endregion

        #endregion

        #region Direct Method

        public ActionResult GetItem(string ControlItemID, string ControlItemDesc, string ControlItemGroupID,
            string ControlItemGroupDesc, string ControlItemTypeDesc, string ControlHasParameter, string ControlVersionDesc,
            string ControlHasPrice, bool FilterUPA, string FilterItemID, string FilterItemDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<ItemVM>> m_dicItemData = GetItemData(true, FilterItemID, FilterItemDesc, FilterUPA);
                KeyValuePair<int, List<ItemVM>> m_kvpItemVM = m_dicItemData.AsEnumerable().ToList()[0];
                if (m_kvpItemVM.Key < 1 || (m_kvpItemVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpItemVM.Key > 1 && !Exact)
                    return Browse(ControlItemID, ControlItemDesc, ControlItemGroupID, ControlItemGroupDesc, ControlItemTypeDesc, ControlHasParameter, ControlVersionDesc, ControlHasPrice, FilterUPA, FilterItemID, FilterItemDesc);

                m_dicItemData = GetItemData(false, FilterItemID, FilterItemDesc, FilterUPA);
                ItemVM m_objItemVM = m_dicItemData[0][0];
                this.GetCmp<TextField>(ControlItemID).Value = m_objItemVM.ItemID;
                this.GetCmp<TextField>(ControlItemDesc).Value = m_objItemVM.ItemDesc;
                this.GetCmp<TextField>(ControlItemGroupDesc).Value = m_objItemVM.ItemGroupDesc;
                this.GetCmp<TextField>(ControlItemTypeDesc).Value = m_objItemVM.ItemTypeDesc;
                this.GetCmp<TextField>(ControlVersionDesc).Value = m_objItemVM.ItemDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

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
        private List<string> IsSaveValid(string Action, ItemVM objItemVM)
        {
            List<string> m_lstReturn = new List<string>();

            if (string.IsNullOrEmpty(objItemVM.ItemDesc))
                m_lstReturn.Add(ItemVM.Prop.ItemGroupID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (string.IsNullOrEmpty(objItemVM.ItemGroupID))
                m_lstReturn.Add(ItemVM.Prop.ItemGroupID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (string.IsNullOrEmpty(objItemVM.UoMID))
                m_lstReturn.Add(ItemVM.Prop.UoMID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (objItemVM.HasParameter)
                if (!objItemVM.ListItemParameterVM.Any())
                    m_lstReturn.Add(ItemVM.Prop.ListItemParameterVM.Desc + " " + General.EnumDesc(MessageLib.mustFill));
                else
                    foreach (ItemParameterVM objItemParameterVM in objItemVM.ListItemParameterVM)
                    {
                        if (string.IsNullOrEmpty(objItemParameterVM.Value))
                            m_lstReturn.Add(ItemParameterVM.Prop.Value.Desc + " " + General.EnumDesc(MessageLib.mustFill));
                    }
            if (objItemVM.HasPrice)
                if (!objItemVM.ListItemPriceVM.Any())
                    m_lstReturn.Add(ItemVM.Prop.ListItemPriceVM.Desc + " " + General.EnumDesc(MessageLib.mustFill));
                else
                    foreach (ItemPriceVM objItemPriceVM in objItemVM.ListItemPriceVM)
                    {
                        if (string.IsNullOrEmpty(objItemPriceVM.PriceTypeID))
                            m_lstReturn.Add(ItemPriceVM.Prop.PriceTypeID.Desc + " " + General.EnumDesc(MessageLib.mustFill));

                    }

            return m_lstReturn;
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
        private ItemPriceVM GetFormItemPriceData(NameValueCollection parameters)
        {
            ItemPriceVM m_objItemPriceVM = new ItemPriceVM()
            {
                ItemPriceID = parameters[ItemPriceVM.Prop.ItemPriceID.Name],
                ClusterDesc = parameters[ItemPriceVM.Prop.ClusterDesc.Name],
                ClusterID = parameters[ItemPriceVM.Prop.ClusterID.Name],
                ItemID = parameters[ItemPriceVM.Prop.ItemID.Name],
                PriceTypeDesc = parameters[ItemPriceVM.Prop.PriceTypeDesc.Name],
                PriceTypeID = parameters[ItemPriceVM.Prop.PriceTypeID.Name],
                ProjectDesc = parameters[ItemPriceVM.Prop.ProjectDesc.Name],
                ProjectID = parameters[ItemPriceVM.Prop.ProjectID.Name],
                RegionDesc = parameters[ItemPriceVM.Prop.RegionDesc.Name],
                RegionID = parameters[ItemPriceVM.Prop.RegionID.Name],
                UnitTypeDesc = parameters[ItemPriceVM.Prop.UnitTypeDesc.Name],
                UnitTypeID = parameters[ItemPriceVM.Prop.UnitTypeID.Name],
                ListItemPriceVendorPeriodVM = JSON.Deserialize<List<ItemPriceVendorPeriodVM>>(parameters[ItemPriceVM.Prop.ListItemPriceVendorPeriodVM.Name]),
                ListItemPriceVendorVM = JSON.Deserialize<List<ItemPriceVendorVM>>(parameters[ItemPriceVM.Prop.ListItemPriceVendorVM.Name])
            };

            return m_objItemPriceVM;
        }
        private List<ItemParameterVM> GetListItemParameter(string ItemID, ref string message)
        {

            List<ItemParameterVM> m_lstItemParameterVM = new List<ItemParameterVM>();
            DItemParameterDA m_objDItemParameterDA = new DItemParameterDA();
            m_objDItemParameterDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemParameterVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(ItemParameterVM.Prop.ItemGroupID.MapAlias);
            m_lstSelect.Add(ItemParameterVM.Prop.ParameterID.MapAlias);
            m_lstSelect.Add(ItemParameterVM.Prop.ParameterDesc.MapAlias);
            m_lstSelect.Add(ItemParameterVM.Prop.Value.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ItemID);
            m_objFilter.Add(ItemParameterVM.Prop.ItemID.Map, m_lstFilter);


            Dictionary<string, OrderDirection> m_objOrderBy = new Dictionary<string, OrderDirection>();
            m_objOrderBy.Add(ItemVM.Prop.ItemID.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicDItemParameterDA = m_objDItemParameterDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDItemParameterDA.Success)
            {
                m_lstItemParameterVM = (
                from DataRow m_drDItemParameterDA in m_dicDItemParameterDA[0].Tables[0].Rows
                select new ItemParameterVM()
                {
                    ItemID = m_drDItemParameterDA[ItemParameterVM.Prop.ItemID.Name].ToString(),
                    ItemGroupID = m_drDItemParameterDA[ItemParameterVM.Prop.ItemGroupID.Name].ToString(),
                    ParameterDesc = m_drDItemParameterDA[ItemParameterVM.Prop.ParameterDesc.Name].ToString(),
                    ParameterID = m_drDItemParameterDA[ItemParameterVM.Prop.ParameterID.Name].ToString(),
                    Value = m_drDItemParameterDA[ItemParameterVM.Prop.Value.Name].ToString(),
                }).Distinct().ToList();
            }
            else
                message = m_objDItemParameterDA.Message;

            return m_lstItemParameterVM;

        }
        private List<ItemPriceVM> GetListItemPrice(string ItemID, ref string message, string RegionID = null, string ProjectID = null, string ClusterID = null, string UnitTypeID = null)
        {

            List<ItemPriceVM> m_lstItemPriceVM = new List<ItemPriceVM>();
            DItemPriceDA m_objDItemPriceDA = new DItemPriceDA();
            m_objDItemPriceDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemPriceVM.Prop.ItemPriceID.MapAlias);
            m_lstSelect.Add(ItemPriceVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(ItemPriceVM.Prop.PriceTypeDesc.MapAlias);
            m_lstSelect.Add(ItemPriceVM.Prop.PriceTypeID.MapAlias);
            m_lstSelect.Add(ItemPriceVM.Prop.ProjectDesc.MapAlias);
            m_lstSelect.Add(ItemPriceVM.Prop.ProjectID.MapAlias);
            m_lstSelect.Add(ItemPriceVM.Prop.RegionDesc.MapAlias);
            m_lstSelect.Add(ItemPriceVM.Prop.RegionID.MapAlias);
            m_lstSelect.Add(ItemPriceVM.Prop.UnitTypeDesc.MapAlias);
            m_lstSelect.Add(ItemPriceVM.Prop.UnitTypeID.MapAlias);
            m_lstSelect.Add(ItemPriceVM.Prop.ClusterDesc.MapAlias);
            m_lstSelect.Add(ItemPriceVM.Prop.ClusterID.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ItemID);
            m_objFilter.Add(ItemPriceVM.Prop.ItemID.Map, m_lstFilter);


            if (RegionID != null)
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(RegionID);
                m_objFilter.Add(ItemPriceVM.Prop.RegionID.Map, m_lstFilter);
            }

            if (ProjectID != null)
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(ProjectID);
                m_objFilter.Add(ItemPriceVM.Prop.ProjectID.Map, m_lstFilter);
            }

            if (ClusterID != null)
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(ClusterID);
                m_objFilter.Add(ItemPriceVM.Prop.ClusterID.Map, m_lstFilter);
            }

            if (UnitTypeID != null)
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(UnitTypeID);
                m_objFilter.Add(ItemPriceVM.Prop.UnitTypeID.Map, m_lstFilter);
            }

            Dictionary<string, OrderDirection> m_objOrderBy = new Dictionary<string, OrderDirection>();
            m_objOrderBy.Add(ItemVM.Prop.ItemID.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicDItemPriceDA = m_objDItemPriceDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDItemPriceDA.Success && m_objDItemPriceDA.Message == string.Empty)
            {
                m_lstItemPriceVM = (
                from DataRow m_drDItemPriceDA in m_dicDItemPriceDA[0].Tables[0].Rows
                select new ItemPriceVM()
                {
                    ItemPriceID = m_drDItemPriceDA[ItemPriceVM.Prop.ItemPriceID.Name].ToString(),
                    ItemID = m_drDItemPriceDA[ItemPriceVM.Prop.ItemID.Name].ToString(),
                    PriceTypeDesc = m_drDItemPriceDA[ItemPriceVM.Prop.PriceTypeDesc.Name].ToString(),
                    PriceTypeID = m_drDItemPriceDA[ItemPriceVM.Prop.PriceTypeID.Name].ToString(),
                    ClusterID = m_drDItemPriceDA[ItemPriceVM.Prop.ClusterID.Name].ToString(),
                    ClusterDesc = m_drDItemPriceDA[ItemPriceVM.Prop.ClusterDesc.Name].ToString(),
                    ProjectDesc = m_drDItemPriceDA[ItemPriceVM.Prop.ProjectDesc.Name].ToString(),
                    ProjectID = m_drDItemPriceDA[ItemPriceVM.Prop.ProjectID.Name].ToString(),
                    RegionDesc = m_drDItemPriceDA[ItemPriceVM.Prop.RegionDesc.Name].ToString(),
                    RegionID = m_drDItemPriceDA[ItemPriceVM.Prop.RegionID.Name].ToString(),
                    UnitTypeDesc = m_drDItemPriceDA[ItemPriceVM.Prop.UnitTypeDesc.Name].ToString(),
                    UnitTypeID = m_drDItemPriceDA[ItemPriceVM.Prop.UnitTypeID.Name].ToString(),
                    ListItemPriceVendorVM = GetListItemPriceVendor(m_drDItemPriceDA[ItemPriceVM.Prop.ItemPriceID.Name].ToString()),
                    ListItemPriceVendorPeriodVM = GetListItemPriceVendorPeriod(m_drDItemPriceDA[ItemPriceVM.Prop.ItemPriceID.Name].ToString())
                }).Distinct().ToList();
            }
            else
                message = m_objDItemPriceDA.Message;

            return m_lstItemPriceVM;

        }
        private List<ItemPriceVendorVM> GetListItemPriceVendor(string ItemPriceID, string VendorID = "")
        {

            List<ItemPriceVendorVM> m_lstItemPriceVendorVM = new List<ItemPriceVendorVM>();
            DItemPriceVendorDA m_objDItemPriceVendorDA = new DItemPriceVendorDA();
            m_objDItemPriceVendorDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemPriceVendorVM.Prop.ItemPriceID.MapAlias);
            m_lstSelect.Add(ItemPriceVendorVM.Prop.VendorDesc.MapAlias);
            m_lstSelect.Add(ItemPriceVendorVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(ItemPriceVendorVM.Prop.IsDefault.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc1.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc2.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc3.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ItemPriceID);
            m_objFilter.Add(ItemPriceVendorVM.Prop.ItemPriceID.Map, m_lstFilter);

            if (VendorID != string.Empty)
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(VendorID);
                m_objFilter.Add(ItemPriceVendorVM.Prop.VendorID.Map, m_lstFilter);
            }


            Dictionary<string, OrderDirection> m_objOrderBy = new Dictionary<string, OrderDirection>();
            m_objOrderBy.Add(ItemVM.Prop.ItemID.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicDItemPriceVendorDA = m_objDItemPriceVendorDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDItemPriceVendorDA.Message == string.Empty)
            {
                m_lstItemPriceVendorVM = (
                from DataRow m_drDItemPriceVendorDA in m_dicDItemPriceVendorDA[0].Tables[0].Rows
                select new ItemPriceVendorVM()
                {
                    ItemPriceID = m_drDItemPriceVendorDA[ItemPriceVendorVM.Prop.ItemPriceID.Name].ToString(),
                    VendorID = (m_drDItemPriceVendorDA[ItemPriceVendorVM.Prop.VendorID.Name].ToString() == "" ?
                              m_drDItemPriceVendorDA[ConfigVM.Prop.Desc1.Name].ToString() : m_drDItemPriceVendorDA[ItemPriceVendorVM.Prop.VendorID.Name].ToString()),
                    VendorDesc = (m_drDItemPriceVendorDA[ItemPriceVendorVM.Prop.VendorDesc.Name].ToString() == "" ?
                                m_drDItemPriceVendorDA[ConfigVM.Prop.Desc2.Name].ToString() : m_drDItemPriceVendorDA[ItemPriceVendorVM.Prop.VendorDesc.Name].ToString()),
                    IsDefault = bool.Parse(m_drDItemPriceVendorDA[ItemPriceVendorVM.Prop.IsDefault.Name].ToString())
                }).Distinct().ToList();
            }

            return m_lstItemPriceVendorVM;

        }
        private List<ItemPriceVendorPeriodVM> GetListItemPriceVendorPeriod(string ItemPriceID, string VendorID = "", string ValidFrom = "")
        {

            List<ItemPriceVendorPeriodVM> m_lstItemPriceVendorPeriodVM = new List<ItemPriceVendorPeriodVM>();
            DItemPriceVendorPeriodDA m_objDItemPriceVendorPeriodDA = new DItemPriceVendorPeriodDA();
            m_objDItemPriceVendorPeriodDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemPriceVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(ItemPriceVM.Prop.PriceTypeID.MapAlias);
            m_lstSelect.Add(ItemPriceVM.Prop.ProjectID.MapAlias);
            m_lstSelect.Add(ItemPriceVM.Prop.RegionID.MapAlias);
            m_lstSelect.Add(ItemPriceVM.Prop.UnitTypeID.MapAlias);
            m_lstSelect.Add(ItemPriceVM.Prop.ClusterID.MapAlias);
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.ItemPriceID.MapAlias);
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.VendorDesc.MapAlias);
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.ValidFrom.MapAlias);
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.ValidTo.MapAlias);
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.CurrencyID.MapAlias);
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.CurrencyDesc.MapAlias);
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.Amount.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc1.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc2.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc3.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();

            if (ItemPriceID != string.Empty)
            {
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(ItemPriceID);
                m_objFilter.Add(ItemPriceVendorPeriodVM.Prop.ItemPriceID.Map, m_lstFilter);
            }

            if (ValidFrom != string.Empty)
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.None);
                m_lstFilter.Add(string.Empty);
                m_objFilter.Add($"('{ValidFrom}' BETWEEN {ItemPriceVendorPeriodVM.Prop.ValidFrom.Map} AND {ItemPriceVendorPeriodVM.Prop.ValidTo.Map})", m_lstFilter);
            }

            Dictionary<string, OrderDirection> m_objOrderBy = new Dictionary<string, OrderDirection>();
            m_objOrderBy.Add(ItemVM.Prop.ItemID.Map, OrderDirection.Ascending);


            if (VendorID != string.Empty)
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(VendorID);
                m_objFilter.Add(ItemPriceVendorPeriodVM.Prop.VendorID.Map, m_lstFilter);

                m_objOrderBy = new Dictionary<string, OrderDirection>();
                m_objOrderBy.Add(ItemPriceVendorPeriodVM.Prop.ValidFrom.Map, OrderDirection.Ascending);
            }

            Dictionary<int, DataSet> m_dicDItemPriceVendorPeriodDA = m_objDItemPriceVendorPeriodDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDItemPriceVendorPeriodDA.Message == string.Empty)
            {
                m_lstItemPriceVendorPeriodVM = (
                from DataRow m_drDItemPriceVendorPeriodDA in m_dicDItemPriceVendorPeriodDA[0].Tables[0].Rows
                select new ItemPriceVendorPeriodVM()
                {
                    ItemID = m_drDItemPriceVendorPeriodDA[ItemPriceVM.Prop.ItemID.Name].ToString(),
                    ItemDesc = m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.ItemDesc.Name].ToString(),
                    PriceTypeID = m_drDItemPriceVendorPeriodDA[ItemPriceVM.Prop.PriceTypeID.Name].ToString(),
                    ClusterID = m_drDItemPriceVendorPeriodDA[ItemPriceVM.Prop.ClusterID.Name].ToString(),
                    ProjectID = m_drDItemPriceVendorPeriodDA[ItemPriceVM.Prop.ProjectID.Name].ToString(),
                    RegionID = m_drDItemPriceVendorPeriodDA[ItemPriceVM.Prop.RegionID.Name].ToString(),
                    UnitTypeID = m_drDItemPriceVendorPeriodDA[ItemPriceVM.Prop.UnitTypeID.Name].ToString(),
                    ItemPriceID = m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.ItemPriceID.Name].ToString(),
                    VendorID = (m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.VendorID.Name].ToString() == "" ?
                               m_drDItemPriceVendorPeriodDA[ConfigVM.Prop.Desc1.Name].ToString() : m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.VendorID.Name].ToString()),
                    VendorDesc = (m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.VendorDesc.Name].ToString() == "" ?
                                m_drDItemPriceVendorPeriodDA[ConfigVM.Prop.Desc2.Name].ToString() : m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.VendorDesc.Name].ToString()),
                    ValidFrom = DateTime.Parse(m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.ValidFrom.Name].ToString()),
                    ValidTo = DateTime.Parse(m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.ValidTo.Name].ToString()),
                    CurrencyDesc = m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.CurrencyDesc.Name].ToString(),
                    CurrencyID = m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.CurrencyID.Name].ToString(),
                    Amount = decimal.Parse(m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.Amount.Name].ToString())
                }).Distinct().ToList();
            }

            return m_lstItemPriceVendorPeriodVM;

        }
        private ItemVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            ItemVM m_objItemVM = new ItemVM();
            MItemDA m_objMItemDA = new MItemDA();
            m_objMItemDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.ItemGroupID.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.ItemGroupDesc.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.ItemTypeID.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.ItemTypeDesc.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.UoMID.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.UoMDesc.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.IsActive.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.HasParameter.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.HasPrice.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objItemVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(ItemVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMItemDA = m_objMItemDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMItemDA.Message == string.Empty)
            {
                DataRow m_drMItemDA = m_dicMItemDA[0].Tables[0].Rows[0];
                m_objItemVM.ItemID = m_drMItemDA[ItemVM.Prop.ItemID.Name].ToString();
                m_objItemVM.ItemDesc = m_drMItemDA[ItemVM.Prop.ItemDesc.Name].ToString();
                m_objItemVM.ItemGroupID = m_drMItemDA[ItemVM.Prop.ItemGroupID.Name].ToString();
                m_objItemVM.ItemGroupDesc = m_drMItemDA[ItemVM.Prop.ItemGroupDesc.Name].ToString();
                m_objItemVM.ItemTypeID = m_drMItemDA[ItemTypeVM.Prop.ItemTypeID.Name].ToString();
                m_objItemVM.ItemTypeDesc = m_drMItemDA[ItemTypeVM.Prop.ItemTypeDesc.Name].ToString();
                m_objItemVM.UoMID = m_drMItemDA[ItemVM.Prop.UoMID.Name].ToString();
                m_objItemVM.UoMDesc = m_drMItemDA[ItemVM.Prop.UoMDesc.Name].ToString();
                m_objItemVM.IsActive = bool.Parse(m_drMItemDA[ItemVM.Prop.IsActive.Name].ToString());
                m_objItemVM.HasParameter = bool.Parse(m_drMItemDA[ItemVM.Prop.HasParameter.Name].ToString());
                m_objItemVM.HasPrice = bool.Parse(m_drMItemDA[ItemVM.Prop.HasPrice.Name].ToString());
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMItemDA.Message;

            return m_objItemVM;
        }
        private List<ItemVM> GetListItems(ref string message)
        {
            MItemDA m_objMItemDA = new MItemDA();
            m_objMItemDA.ConnectionStringName = Global.ConnStrConfigName;

            //int m_intSkip = parameters.Start;
            //int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            List<object> m_lstFilter = new List<object>();
            FilterHeaderConditions m_fhcMItem = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMItem.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);
                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = ItemVM.Prop.Map(m_strDataIndex, false);

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
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.ItemTypeDesc.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.ItemTypeID.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.ItemGroupID.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.ItemGroupDesc.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.UoMID.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.UoMDesc.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.IsActive.MapAlias);

            List<string> m_lstGroup = new List<string>();
            m_lstGroup.Add(ItemVM.Prop.ItemID.Map);
            m_lstGroup.Add(ItemVM.Prop.ItemDesc.Map);
            m_lstGroup.Add(ItemVM.Prop.ItemTypeDesc.Map);
            m_lstGroup.Add(ItemVM.Prop.ItemTypeID.Map);
            m_lstGroup.Add(ItemVM.Prop.ItemGroupID.Map);
            m_lstGroup.Add(ItemVM.Prop.ItemGroupDesc.Map);
            m_lstGroup.Add(ItemVM.Prop.UoMID.Map);
            m_lstGroup.Add(ItemVM.Prop.UoMDesc.Map);
            m_lstGroup.Add(ItemVM.Prop.IsActive.Map);

            //UserVM m_objUserVM = getCurentUser();
            //if (!string.IsNullOrEmpty(m_objUserVM.VendorID))
            //{
            //    m_lstFilter = new List<object>();
            //    m_lstFilter.Add(Operator.Equals);
            //    m_lstFilter.Add(m_objUserVM.VendorID);
            //    m_objFilter.Add(ItemPriceVendorVM.Prop.VendorID.Map, m_lstFilter);
            //}

            Dictionary<int, DataSet> m_dicMItemDA = m_objMItemDA.SelectBC(0, null, m_boolIsCount, null, m_objFilter, null, m_lstGroup, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpItemBL in m_dicMItemDA)
            {
                m_intCount = m_kvpItemBL.Key;
                break;
            }

            List<ItemVM> m_lstItemVM = new List<ItemVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;

                //m_lstSelect.Add(ItemVM.Prop.ItemID.MapAlias);
                //m_lstSelect.Add(ItemVM.Prop.ItemDesc.MapAlias);
                //m_lstSelect.Add(ItemVM.Prop.ItemTypeDesc.MapAlias);
                //m_lstSelect.Add(ItemVM.Prop.ItemTypeID.MapAlias);
                //m_lstSelect.Add(ItemVM.Prop.ItemGroupID.MapAlias);
                //m_lstSelect.Add(ItemVM.Prop.ItemGroupDesc.MapAlias);
                //m_lstSelect.Add(ItemVM.Prop.UoMID.MapAlias);
                //m_lstSelect.Add(ItemVM.Prop.UoMDesc.MapAlias);
                //m_lstSelect.Add(ItemVM.Prop.IsActive.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                //foreach (DataSorter m_dtsOrder in parameters.Sort)
                //    m_dicOrder.Add(ItemVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMItemDA = m_objMItemDA.SelectBC(0, null, m_boolIsCount, m_lstSelect, m_objFilter, null, m_lstGroup, m_dicOrder, null);
                if (m_objMItemDA.Message == string.Empty)
                {
                    m_lstItemVM = (
                        from DataRow m_drMItemDA in m_dicMItemDA[0].Tables[0].Rows
                        select new ItemVM()
                        {
                            ItemID = m_drMItemDA[ItemVM.Prop.ItemID.Name].ToString(),
                            ItemDesc = m_drMItemDA[ItemVM.Prop.ItemDesc.Name].ToString(),
                            ItemTypeDesc = m_drMItemDA[ItemVM.Prop.ItemTypeDesc.Name].ToString(),
                            ItemGroupDesc = m_drMItemDA[ItemVM.Prop.ItemGroupDesc.Name].ToString(),
                            UoMDesc = m_drMItemDA[ItemVM.Prop.UoMDesc.Name].ToString(),
                            UoMID = m_drMItemDA[ItemVM.Prop.UoMID.Name].ToString(),
                            IsActive = Convert.ToBoolean(m_drMItemDA[ItemVM.Prop.IsActive.Name].ToString())
                        }
                    ).ToList();
                }
            }
            return m_lstItemVM;
        }
        private List<ItemDetailVM> GetListItemDetails(string ItemID, string VendorID, ref string message)
        {

            List<ItemDetailVM> m_lstItemDetailVM = new List<ItemDetailVM>();
            DItemDetailsDA m_objDItemDetailsDA = new DItemDetailsDA();
            m_objDItemDetailsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemDetailVM.Prop.ItemDetailID.MapAlias);
            m_lstSelect.Add(ItemDetailVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(ItemDetailVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(ItemDetailVM.Prop.ItemDetailDesc.MapAlias);
            m_lstSelect.Add(ItemDetailVM.Prop.ItemDetailTypeID.MapAlias);
            m_lstSelect.Add(ItemDetailVM.Prop.ItemDetailTypeDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();

            if (!string.IsNullOrEmpty(ItemID))
            {
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(ItemID);
                m_objFilter.Add(ItemDetailVM.Prop.ItemID.Map, m_lstFilter);
            }
            if (!string.IsNullOrEmpty(VendorID))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(VendorID);
                m_objFilter.Add(ItemDetailVM.Prop.VendorID.Map, m_lstFilter);
            }


            Dictionary<string, OrderDirection> m_objOrderBy = new Dictionary<string, OrderDirection>();
            m_objOrderBy.Add(ItemDetailVM.Prop.ItemDetailTypeID.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicDItemDetailDA = m_objDItemDetailsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDItemDetailsDA.Message == string.Empty)
            {
                m_lstItemDetailVM = (
                from DataRow m_drDItemDetail in m_dicDItemDetailDA[0].Tables[0].Rows
                select new ItemDetailVM()
                {
                    ItemID = m_drDItemDetail[ItemDetailVM.Prop.ItemID.Name].ToString(),
                    ItemDesc = m_drDItemDetail[ItemVM.Prop.ItemDesc.Name].ToString(),
                    VendorID = m_drDItemDetail[ItemDetailVM.Prop.VendorID.Name].ToString(),
                    ItemDetailID = m_drDItemDetail[ItemDetailVM.Prop.ItemDetailID.Name].ToString(),
                    ItemDetailDesc = m_drDItemDetail[ItemDetailVM.Prop.ItemDetailDesc.Name].ToString(),
                    ItemDetailTypeID = m_drDItemDetail[ItemDetailVM.Prop.ItemDetailTypeID.Name].ToString(),
                    ItemDetailTypeDesc = m_drDItemDetail[ItemDetailVM.Prop.ItemDetailTypeDesc.Name].ToString()
                }).Distinct().ToList();
            }
            else
                message = m_objDItemDetailsDA.Message;

            return m_lstItemDetailVM;

        }
        private ConfigVM GetVendorConfig()
        {

            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ConfigVM.Prop.Desc1.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc2.MapAlias);

            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ItemPriceVendorVM.Prop.VendorID.Name);
            m_objFilter.Add(ConfigVM.Prop.Key1.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ItemPriceVendorVM.Prop.VendorDesc.Name);
            m_objFilter.Add(ConfigVM.Prop.Key2.Map, m_lstFilter);


            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("ItemPriceVendor");
            m_objFilter.Add(ConfigVM.Prop.Key3.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicUConfigDA = m_objUConfigDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            ConfigVM m_objConfigVM = new ConfigVM();
            if (m_objUConfigDA.Message == string.Empty)
            {
                DataRow m_drUConfigDA = m_dicUConfigDA[0].Tables[0].Rows[0];
                m_objConfigVM.Desc1 = m_drUConfigDA[ConfigVM.Prop.Desc1.Name].ToString();
                m_objConfigVM.Desc2 = m_drUConfigDA[ConfigVM.Prop.Desc2.Name].ToString();
            }
            return m_objConfigVM;

        }

        private ItemPriceVM GetItemPrice(string ItemID, ref string message, string RegionID = null, string ProjectID = null, string ClusterID = null, string UnitTypeID = null)
        {

            ItemPriceVM m_objItemPriceVM = new ItemPriceVM();
            DItemPriceDA m_objDItemPriceDA = new DItemPriceDA();
            m_objDItemPriceDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemPriceVM.Prop.ItemPriceID.MapAlias);
            m_lstSelect.Add(ItemPriceVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(ItemPriceVM.Prop.PriceTypeDesc.MapAlias);
            m_lstSelect.Add(ItemPriceVM.Prop.PriceTypeID.MapAlias);
            m_lstSelect.Add(ItemPriceVM.Prop.ProjectDesc.MapAlias);
            m_lstSelect.Add(ItemPriceVM.Prop.ProjectID.MapAlias);
            m_lstSelect.Add(ItemPriceVM.Prop.RegionDesc.MapAlias);
            m_lstSelect.Add(ItemPriceVM.Prop.RegionID.MapAlias);
            m_lstSelect.Add(ItemPriceVM.Prop.UnitTypeDesc.MapAlias);
            m_lstSelect.Add(ItemPriceVM.Prop.UnitTypeID.MapAlias);
            m_lstSelect.Add(ItemPriceVM.Prop.ClusterDesc.MapAlias);
            m_lstSelect.Add(ItemPriceVM.Prop.ClusterID.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ItemID);
            m_objFilter.Add(ItemPriceVM.Prop.ItemID.Map, m_lstFilter);


            if (RegionID != null)
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(RegionID);
                m_objFilter.Add(ItemPriceVM.Prop.RegionID.Map, m_lstFilter);
            }

            if (ProjectID != null)
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(ProjectID);
                m_objFilter.Add(ItemPriceVM.Prop.ProjectID.Map, m_lstFilter);
            }

            if (ClusterID != null)
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(ClusterID);
                m_objFilter.Add(ItemPriceVM.Prop.ClusterID.Map, m_lstFilter);
            }

            if (UnitTypeID != null)
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(UnitTypeID);
                m_objFilter.Add(ItemPriceVM.Prop.UnitTypeID.Map, m_lstFilter);
            }

            Dictionary<string, OrderDirection> m_objOrderBy = new Dictionary<string, OrderDirection>();
            m_objOrderBy.Add(ItemVM.Prop.ItemID.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicDItemPriceDA = m_objDItemPriceDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDItemPriceDA.Success && m_objDItemPriceDA.Message == string.Empty)
            {
                m_objItemPriceVM = (
                from DataRow m_drDItemPriceDA in m_dicDItemPriceDA[0].Tables[0].Rows
                select new ItemPriceVM()
                {
                    ItemPriceID = m_drDItemPriceDA[ItemPriceVM.Prop.ItemPriceID.Name].ToString(),
                    ItemID = m_drDItemPriceDA[ItemPriceVM.Prop.ItemID.Name].ToString(),
                    PriceTypeDesc = m_drDItemPriceDA[ItemPriceVM.Prop.PriceTypeDesc.Name].ToString(),
                    PriceTypeID = m_drDItemPriceDA[ItemPriceVM.Prop.PriceTypeID.Name].ToString(),
                    ClusterID = m_drDItemPriceDA[ItemPriceVM.Prop.ClusterID.Name].ToString(),
                    ClusterDesc = m_drDItemPriceDA[ItemPriceVM.Prop.ClusterDesc.Name].ToString(),
                    ProjectDesc = m_drDItemPriceDA[ItemPriceVM.Prop.ProjectDesc.Name].ToString(),
                    ProjectID = m_drDItemPriceDA[ItemPriceVM.Prop.ProjectID.Name].ToString(),
                    RegionDesc = m_drDItemPriceDA[ItemPriceVM.Prop.RegionDesc.Name].ToString(),
                    RegionID = m_drDItemPriceDA[ItemPriceVM.Prop.RegionID.Name].ToString(),
                    UnitTypeDesc = m_drDItemPriceDA[ItemPriceVM.Prop.UnitTypeDesc.Name].ToString(),
                    UnitTypeID = m_drDItemPriceDA[ItemPriceVM.Prop.UnitTypeID.Name].ToString()
                }).FirstOrDefault();
            }
            else
                message = m_objDItemPriceDA.Message;

            return m_objItemPriceVM;

        }
        #endregion

        #region Public Method

        public Dictionary<int, List<ItemVM>> GetItemData(bool isCount, string ItemID, string ItemDesc, bool FilterUPA)
        {
            int m_intCount = 0;
            List<ItemVM> m_lstItemVM = new List<ViewModels.ItemVM>();
            Dictionary<int, List<ItemVM>> m_dicReturn = new Dictionary<int, List<ItemVM>>();

            List<string> m_lstSelect = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();

            List<string> m_lstUPA = new List<string>();

            #region with filter UPA
            if (FilterUPA)
            {
                UConfigDA m_objUConfigDA = new UConfigDA();
                m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;

                m_lstSelect = new List<string>();
                m_lstSelect.Add(ConfigVM.Prop.Desc1.MapAlias);

                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add("ItemTypeID");
                m_objFilter.Add(ConfigVM.Prop.Key1.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add("UnitPriceAnalysis");
                m_objFilter.Add(ConfigVM.Prop.Key2.Map, m_lstFilter);

                Dictionary<int, DataSet> m_dicUConfigDA = m_objUConfigDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
                List<ConfigVM> m_lstConfigVM = new List<ViewModels.ConfigVM>();
                if (m_objUConfigDA.Message == string.Empty)
                {
                    m_lstConfigVM = (
                            from DataRow m_drUConfigDA in m_dicUConfigDA[0].Tables[0].Rows
                            select new ConfigVM()
                            {
                                Desc1 = m_drUConfigDA[ConfigVM.Prop.Desc1.Name].ToString()
                            }
                        ).ToList();
                }

                foreach (ConfigVM item in m_lstConfigVM)
                {
                    m_lstUPA.Add(item.Desc1);
                }
            }
            #endregion

            MItemDA m_objMItemDA = new MItemDA();
            m_objMItemDA.ConnectionStringName = Global.ConnStrConfigName;

            m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.ItemGroupID.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.ItemGroupDesc.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.ItemTypeDesc.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.HasParameter.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.HasPrice.MapAlias);

            m_objFilter = new Dictionary<string, List<object>>();
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(ItemID);
            m_objFilter.Add(ItemVM.Prop.ItemID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(ItemDesc);
            m_objFilter.Add(ItemVM.Prop.ItemDesc.Map, m_lstFilter);

            #region with filter UPA
            if (FilterUPA)
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.In);
                m_lstFilter.Add(String.Join(",", m_lstUPA));
                m_objFilter.Add(ItemVM.Prop.ItemTypeID.Map, m_lstFilter);
            }
            #endregion

            Dictionary<int, DataSet> m_dicMItemDA = m_objMItemDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMItemDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpItemBL in m_dicMItemDA)
                    {
                        m_intCount = m_kvpItemBL.Key;
                        break;
                    }
                else
                {
                    m_lstItemVM = (
                        from DataRow m_drMItemDA in m_dicMItemDA[0].Tables[0].Rows
                        select new ItemVM()
                        {
                            ItemID = m_drMItemDA[ItemVM.Prop.ItemID.Name].ToString(),
                            ItemDesc = m_drMItemDA[ItemVM.Prop.ItemDesc.Name].ToString(),
                            ItemGroupDesc = m_drMItemDA[ItemVM.Prop.ItemGroupDesc.Name].ToString(),
                            ItemTypeDesc = m_drMItemDA[ItemVM.Prop.ItemTypeDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstItemVM);
            return m_dicReturn;
        }

        #endregion

        #region Upload
        public ActionResult GetTemplateWithData(string Caller)
        {
            string m_strMessage = string.Empty;
            //UserVM m_objUser = getCurentUser();
            List<ItemVM> m_objLstItems = GetListItems(ref m_strMessage);
            List<ItemPriceVendorPeriodVM> m_objLstItemPrice = GetListItemPriceVendorPeriod(string.Empty,string.Empty, DateTime.Now.ToString("yyyy-MM-dd"));
            List<ItemDetailVM> m_objLstItemSpec = GetListItemDetails(string.Empty, string.Empty, ref m_strMessage);

            string m_filepath = Server.MapPath($"~/Content/Template/Item Price/UploadTemplate-ItemPricev3.0.xlsx");

            XSSFWorkbook m_xssFWorkbook;
            using (FileStream file = new FileStream(m_filepath, FileMode.Open, FileAccess.Read))
            {
                m_xssFWorkbook = new XSSFWorkbook(file);
            }
            ISheet sheet, sheetItem, sheetDetail;
            sheetItem = m_xssFWorkbook.GetSheetAt(0);
            sheet = m_xssFWorkbook.GetSheetAt(1);
            sheetDetail = m_xssFWorkbook.GetSheetAt(2);

            #region Items
            DataTable m_dttable = new DataTable();
            IRow m_iRowheader = sheetItem.GetRow(1);
            int m_cellCount = m_iRowheader.LastCellNum;
            Dictionary<string, int> m_dicHeaderItem = new Dictionary<string, int>();
            for (int i = m_iRowheader.FirstCellNum; i < m_cellCount; i++)
            {
                if (!m_dicHeaderItem.ContainsKey(m_iRowheader.GetCell(i).StringCellValue.Trim()))
                    m_dicHeaderItem.Add(m_iRowheader.GetCell(i).StringCellValue.Trim(), i);
            }
            int rowCount = m_objLstItems.Count;
            List<ItemVM> m_lstItemVM = new List<ItemVM>();
            List<ItemPriceUploadVM> m_lstItemPriceVM = new List<ItemPriceUploadVM>();
            List<string> m_lstMessage = new List<string>();

            string[] m_headerInfoItem = { "Tablename", "Fieldname", "Max Length" };
            int idx = 0;
            for (int i = sheetItem.FirstRowNum + 3; i < rowCount + 3; i++)
            {

                try
                {
                    IRow row = sheetItem.CreateRow(i);

                    ItemVM m_objItemVM = new ItemVM();


                    //ItemID
                    row.CreateCell(m_dicHeaderItem.Where(x => x.Key.Trim().Equals(ItemVM.Prop.ItemID.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value).SetCellValue(m_objLstItems.ElementAt(idx).ItemID);

                    //ItemDesc
                    row.CreateCell(m_dicHeaderItem.Where(x => x.Key.Trim().Equals(ItemVM.Prop.ItemDesc.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value).SetCellValue(m_objLstItems.ElementAt(idx).ItemDesc);

                    //ItemGroupID
                    row.CreateCell(m_dicHeaderItem.Where(x => x.Key.Trim().Equals(ItemVM.Prop.ItemGroupID.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value).SetCellValue(m_objLstItems.ElementAt(idx).ItemGroupID);

                    //UoMID
                    row.CreateCell(m_dicHeaderItem.Where(x => x.Key.Trim().Equals(ItemVM.Prop.UoMID.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value).SetCellValue(m_objLstItems.ElementAt(idx).UoMID);

                    //IsActive
                    row.CreateCell(m_dicHeaderItem.Where(x => x.Key.Trim().Equals(ItemVM.Prop.IsActive.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value).SetCellValue(m_objLstItems.ElementAt(idx).IsActive);



                    //}
                }
                catch (Exception ex)
                {
                    m_lstMessage.Add(String.Format("Error Row [{0}] : {1}", i, ex.Message));
                    break;
                }

                idx++;

            }
            #endregion

            #region Item Price

            DataTable table = new DataTable();
            IRow headerRow = sheet.GetRow(1);
            int cellCount = headerRow.LastCellNum;
            Dictionary<string, int> m_dicHeader = new Dictionary<string, int>();
            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                if (!m_dicHeader.ContainsKey(headerRow.GetCell(i).StringCellValue.Trim()))
                    m_dicHeader.Add(headerRow.GetCell(i).StringCellValue.Trim(), i);
            }
            rowCount = m_objLstItemPrice.Count;
            m_lstMessage = new List<string>();

            string[] m_headerInfo = { "Tablename", "Fieldname", "Max Length" };
            idx = 0;
            for (int i = sheet.FirstRowNum + 3; i < rowCount + 3; i++)
            {

                try
                {
                    IRow row = sheet.CreateRow(i);

                    ItemPriceUploadVM m_objItemPriceVM = new ItemPriceUploadVM();
                    //m_objItemPriceVM.ItemPriceID = Guid.NewGuid().ToString().Replace("-", "");

                    #region Item Price
                    //ItemID
                    row.CreateCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemPriceVM.Prop.ItemID.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value).SetCellValue(m_objLstItemPrice.ElementAt(idx).ItemID);

                    //ItemDesc
                    row.CreateCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemVM.Prop.ItemDesc.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value).SetCellValue(m_objLstItemPrice.ElementAt(idx).ItemDesc);

                    //RegionID
                    row.CreateCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemPriceVM.Prop.RegionID.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value).SetCellValue(m_objLstItemPrice.ElementAt(idx).RegionID);

                    //ProjectID
                    row.CreateCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemPriceVM.Prop.ProjectID.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value).SetCellValue(m_objLstItemPrice.ElementAt(idx).ProjectID);

                    //ClusterID
                    row.CreateCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemPriceVM.Prop.ClusterID.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value).SetCellValue(m_objLstItemPrice.ElementAt(idx).ClusterID);

                    //UnitTypeID
                    row.CreateCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemPriceVM.Prop.UnitTypeID.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value).SetCellValue(m_objLstItemPrice.ElementAt(idx).UnitTypeID);

                    //PriceTypeID
                    row.CreateCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemPriceVM.Prop.PriceTypeID.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value).SetCellValue(m_objLstItemPrice.ElementAt(idx).UnitTypeID);




                    #endregion

                    #region Item Price Vendor
                    //VendorID
                    row.CreateCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemPriceVendorVM.Prop.VendorID.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value).SetCellValue(m_objLstItemPrice.ElementAt(idx).VendorID);

                    //IsDefault
                    row.CreateCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemPriceVendorVM.Prop.IsDefault.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value).SetCellValue(m_objLstItemPrice.ElementAt(idx).IsDefault);

                    #endregion

                    #region Item Price Vendor Period
                    //ValidFrom
                    row.CreateCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemPriceVendorPeriodVM.Prop.ValidFrom.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value).SetCellValue(m_objLstItemPrice.ElementAt(idx).ValidFrom.ToString(Global.DefaultDateFormat));


                    //ValidTo
                    row.CreateCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemPriceVendorPeriodVM.Prop.ValidTo.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value).SetCellValue(m_objLstItemPrice.ElementAt(idx).ValidTo.ToString(Global.DefaultDateFormat));

                    //CurrencyID
                    row.CreateCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemPriceVendorPeriodVM.Prop.CurrencyID.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value).SetCellValue(m_objLstItemPrice.ElementAt(idx).CurrencyID);

                    //Amount
                    row.CreateCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemPriceVendorPeriodVM.Prop.Amount.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value).SetCellValue((double)m_objLstItemPrice.ElementAt(idx).Amount);

                    #endregion

                    idx++;
                }
                catch (Exception ex)
                {
                    m_lstMessage.Add(String.Format("Error Row [{0}] : {1}", i, ex.Message));
                    break;
                }



            }
            #endregion

            #region Item Details
            List<ItemDetailUploadVM> m_lsGroupItemVendor = m_objLstItemSpec.GroupBy(d => new { d.ItemID, d.ItemDesc, d.VendorID }).Select(d => new ItemDetailUploadVM { ItemID = d.Key.ItemID, ItemDesc = d.Key.ItemDesc, VendorID = d.Key.VendorID }).ToList();
            table = new DataTable();
            headerRow = sheetDetail.GetRow(1);
            cellCount = headerRow.LastCellNum;
            m_dicHeader = new Dictionary<string, int>();
            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                if (!m_dicHeader.ContainsKey(headerRow.GetCell(i).StringCellValue.Trim()))
                    m_dicHeader.Add(headerRow.GetCell(i).StringCellValue.Trim(), i);
            }
            rowCount = m_lsGroupItemVendor.Count;
            m_lstMessage = new List<string>();

            string[] m_hdrInfo = { "Tablename", "Fieldname", "Max Length" };
            idx = 0;
            for (int i = sheetDetail.FirstRowNum + 3; i < rowCount + 3; i++)
            {

                try
                {
                    IRow row = sheetDetail.CreateRow(i);

                    ItemDetailUploadVM m_objItemDetailVM = new ItemDetailUploadVM();
                    //m_objItemPriceVM.ItemPriceID = Guid.NewGuid().ToString().Replace("-", "");

                    #region Item Detail
                    //ItemID
                    row.CreateCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemDetailUploadVM.Prop.ItemID.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value).SetCellValue(m_lsGroupItemVendor.ElementAt(idx).ItemID);

                    //ItemDesc
                    row.CreateCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemDetailUploadVM.Prop.ItemDesc.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value).SetCellValue(m_lsGroupItemVendor.ElementAt(idx).ItemDesc);

                    //VendorID
                    row.CreateCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemDetailUploadVM.Prop.VendorID.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value).SetCellValue(m_lsGroupItemVendor.ElementAt(idx).VendorID);

                    foreach (var strItemDetailType in System.Enum.GetNames(typeof(ItemDetailTypes)).ToList())
                    {
                        ItemDetailVM obj = m_objLstItemSpec.Where(d => d.ItemDetailTypeDesc == strItemDetailType && d.ItemID == m_lsGroupItemVendor.ElementAt(idx).ItemID).FirstOrDefault();
                        if (m_dicHeader.ContainsKey(strItemDetailType))
                        {
                            row.CreateCell(m_dicHeader[strItemDetailType]).SetCellValue(obj != null ? obj.ItemDetailDesc ?? string.Empty : string.Empty);
                        }
                    }
                    #endregion



                    idx++;
                }
                catch (Exception ex)
                {
                    m_lstMessage.Add(String.Format("Error Row [{0}] : {1}", i, ex.Message));
                    break;
                }

            }
            #endregion

            if (m_lstMessage.Count > 0)
            {
                Global.ShowErrorAlert(title, String.Join(Global.NewLineSeparated, m_lstMessage));
                return this.Direct();
            }

            // Generate a new unique identifier against which the file can be stored
            string handle = Guid.NewGuid().ToString().Replace("-", "");

            using (MemoryStream memoryStream = new MemoryStream())
            {
                m_xssFWorkbook.Write(memoryStream);
                memoryStream.Position = 0;
                TempData[handle] = memoryStream.ToArray();

            }

            return this.Direct(new
            {
                Data = new { FileGuid = handle, FileName = $"Tpl-item-price-specification-v3.xlsx" }
            });
        }
        public virtual ActionResult Download(string fileGuid, string fileName)
        {
            if (TempData[fileGuid] != null)
            {
                byte[] data = TempData[fileGuid] as byte[];
                return File(data, "application/vnd.ms-excel", fileName);
            }
            else
            {
                // Problem - Log the error, generate a blank file,
                //           redirect to another controller action - whatever fits with your application
                return new EmptyResult();
            }
        }
        public ActionResult Upload(string Caller)
        {
            ItemVM m_objItemVM = new ItemVM();
            m_objItemVM.ListItemParameterVM = new List<ItemParameterVM>();
            m_objItemVM.ListItemPriceVM = new List<ItemPriceVM>();

            ViewDataDictionary m_vddItem = new ViewDataDictionary();
            m_vddItem.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddItem.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
            if (Caller == General.EnumDesc(Buttons.ButtonDetail))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                Dictionary<string, object> m_dicSelectedRow = GetFormData(m_nvcParams);
                Session[dataSessionName] = JSON.Serialize(m_dicSelectedRow);
            }

            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objItemVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddItem,
                ViewName = "ItemPrice/Upload/_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult ReadFileUpload()
        {
            XSSFWorkbook xhssfwb = new XSSFWorkbook();
            HSSFWorkbook hssfwb = new HSSFWorkbook();
            MItemDA m_objMItemDA = new MItemDA();
            DItemPriceDA m_objDItemPriceDA = new DItemPriceDA();
            DItemPriceVendorDA m_objDItemPriceVendorDA = new DItemPriceVendorDA();
            DItemPriceVendorPeriodDA m_objDItemPriceVendorPeriodDA = new DItemPriceVendorPeriodDA();
            string m_strItemID = string.Empty;
            m_objDItemPriceDA.ConnectionStringName = Global.ConnStrConfigName;
            m_objMItemDA.ConnectionStringName = Global.ConnStrConfigName;
            //string m_strTransName = "DItemPrice";
            //object m_objDBConnection = null;
            //m_objDBConnection = m_objDItemPriceDA.BeginTrans(m_strTransName);
            //string m_strMessage = string.Empty;


            try
            {
                SWeb.HttpPostedFile m_fileUpload = X.GetCmp<FileUploadField>(ItemPriceVM.Prop.FileUpload.Name).PostedFile;
                ISheet sheet, sheetItem, sheetDetail;
                string filename = Path.GetFileName(Server.MapPath(m_fileUpload.FileName));
                var fileExt = Path.GetExtension(filename);
                if (fileExt == ".xls")
                {
                    hssfwb = new HSSFWorkbook(m_fileUpload.InputStream);
                    sheetItem = hssfwb.GetSheetAt(0);
                    sheet = hssfwb.GetSheetAt(1);
                    sheetDetail = hssfwb.GetSheetAt(2);
                }
                else
                {
                    xhssfwb = new XSSFWorkbook(m_fileUpload.InputStream);
                    sheetItem = xhssfwb.GetSheetAt(0);
                    sheet = xhssfwb.GetSheetAt(1);
                    sheetDetail = xhssfwb.GetSheetAt(2);
                }
                #endregion

                #region Items
                DataTable m_dttable = new DataTable();
                IRow m_iRowheader = sheetItem.GetRow(1);
                int m_cellCount = m_iRowheader.LastCellNum;
                Dictionary<string, int> m_dicHeaderItem = new Dictionary<string, int>();
                for (int i = m_iRowheader.FirstCellNum; i < m_cellCount; i++)
                {
                    if (!m_dicHeaderItem.ContainsKey(m_iRowheader.GetCell(i).StringCellValue.Trim()))
                        m_dicHeaderItem.Add(m_iRowheader.GetCell(i).StringCellValue.Trim(), i);
                }
                int rowCount = sheetItem.PhysicalNumberOfRows;
                List<ItemVM> m_lstItemVM = new List<ItemVM>();
                List<ItemPriceUploadVM> m_lstItemPriceVM = new List<ItemPriceUploadVM>();
                List<string> m_lstMessage = new List<string>();

                string[] m_headerInfoItem = { "Tablename", "Fieldname", "Max Length" };

                for (int i = sheetItem.FirstRowNum + 3; i < rowCount; i++)
                {

                    try
                    {
                        IRow row = sheetItem.GetRow(i);
                        ICell m_cellValue = null;

                        if (row.GetCell(1, MissingCellPolicy.RETURN_BLANK_AS_NULL) == null)
                        {
                            break;
                        }

                        if (String.IsNullOrEmpty(row.GetCell(m_dicHeaderItem.Where(x => x.Key.Trim().Equals(ItemVM.Prop.ItemID.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value).ToString())
                            && String.IsNullOrEmpty(row.GetCell(m_dicHeaderItem.Where(x => x.Key.Trim().Equals(ItemVM.Prop.ItemDesc.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value).ToString()))
                        {
                            break;
                        }
                        m_cellValue = row.GetCell(0);
                        if (!m_headerInfoItem.Contains(m_cellValue == null ? string.Empty : m_cellValue.ToString()) && !string.IsNullOrEmpty(row.GetCell(1).ToString()))
                        {

                            ItemVM m_objItemVM = new ItemVM();
                            //m_objItemPriceVM.ItemPriceID = Guid.NewGuid().ToString().Replace("-", "");

                            #region Item
                            //ItemID
                            m_cellValue = row.GetCell(m_dicHeaderItem.Where(x => x.Key.Trim().Equals(ItemVM.Prop.ItemID.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                            if (m_cellValue == null || String.IsNullOrEmpty(m_cellValue.ToString()))
                            {
                                m_lstMessage.Add(String.Format("Row [{0}] Column {1} {2}", i, ItemVM.Prop.ItemID.Desc, General.EnumDesc(MessageLib.mustFill)));
                            }
                            else
                            {
                                m_objItemVM.ItemID = m_cellValue.ToString();
                            }

                            //ItemDesc
                            m_cellValue = row.GetCell(m_dicHeaderItem.Where(x => x.Key.Trim().Equals(ItemVM.Prop.ItemDesc.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                            if (m_cellValue == null || String.IsNullOrEmpty(m_cellValue.ToString()))
                            {
                                m_lstMessage.Add(String.Format("Row [{0}] Column {1} {2}", i, ItemVM.Prop.ItemDesc.Desc, General.EnumDesc(MessageLib.mustFill)));
                            }
                            else
                            {
                                m_objItemVM.ItemDesc = m_cellValue.ToString();
                            }

                            //ItemGroupID
                            m_cellValue = row.GetCell(m_dicHeaderItem.Where(x => x.Key.Trim().Equals(ItemVM.Prop.ItemGroupID.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                            m_objItemVM.ItemGroupID = m_cellValue == null ? string.Empty : m_cellValue.ToString();


                            //UoMID
                            m_cellValue = row.GetCell(m_dicHeaderItem.Where(x => x.Key.Trim().Equals(ItemVM.Prop.UoMID.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                            m_objItemVM.UoMID = m_cellValue == null ? string.Empty : m_cellValue.ToString();


                            //IsActive
                            m_cellValue = row.GetCell(m_dicHeaderItem.Where(x => x.Key.Trim().Equals(ItemVM.Prop.IsActive.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                            m_objItemVM.IsActive = (m_cellValue.ToString() == "1" ? true : false);


                            //ParameterID
                            m_cellValue = row.GetCell(m_dicHeaderItem.Where(x => x.Key.Trim().Equals(ItemPriceVM.Prop.PriceTypeID.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                            m_objItemVM.ParameterID = m_cellValue == null ? string.Empty : m_cellValue.ToString();

                            //Value
                            m_cellValue = row.GetCell(m_dicHeaderItem.Where(x => x.Key.Trim().Equals(ItemPriceVM.Prop.PriceTypeID.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                            m_objItemVM.Value = m_cellValue == null ? string.Empty : m_cellValue.ToString();


                            //Get ItemPriceID if exist
                            //string m_strMessage = string.Empty;
                            //List<ItemPriceVM> m_listItemPriceVM = GetListItemPrice(m_objItemPriceVM.ItemID, ref m_strMessage, m_objItemPriceVM.RegionID, m_objItemPriceVM.ProjectID, m_objItemPriceVM.ClusterID, m_objItemPriceVM.UnitTypeID);
                            //if (m_strMessage == string.Empty)
                            //{
                            //    m_objItemPriceVM.ItemPriceID = m_listItemPriceVM.FirstOrDefault().ItemPriceID;
                            //    //m_objItemPriceVM.ItemID = m_listItemPriceVM.FirstOrDefault().ItemID;
                            //}

                            #endregion


                            if (!m_lstItemPriceVM.Any(d => d.ItemID == m_objItemVM.ItemID))
                            {
                                m_lstItemVM.Add(m_objItemVM);
                                //MItem m_objMItem = new MItem();
                                //m_objMItem.ItemID = m_objItemVM.ItemID;
                                //m_objMItemDA.Data = m_objMItem;
                                //m_objMItemDA.Select();
                                //if (m_objMItemDA.Message != string.Empty && !m_objMItemDA.Success)
                                //{
                                //    m_lstItemVM.Add(m_objItemVM);
                                //}
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        m_lstMessage.Add(String.Format("Error Row [{0}] : {1}", i, ex.Message));
                        break;
                    }

                }

                if (m_lstMessage.Count > 0)
                {
                    Global.ShowErrorAlert(title, String.Join(Global.NewLineSeparated, m_lstMessage));
                    return this.Direct();
                }
                #endregion

                #region Item Price

                DataTable table = new DataTable();
                IRow headerRow = sheet.GetRow(1);
                int cellCount = headerRow.LastCellNum;
                Dictionary<string, int> m_dicHeader = new Dictionary<string, int>();
                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                {
                    if (!m_dicHeader.ContainsKey(headerRow.GetCell(i).StringCellValue.Trim()))
                        m_dicHeader.Add(headerRow.GetCell(i).StringCellValue.Trim(), i);
                }
                rowCount = sheet.PhysicalNumberOfRows;
                m_lstMessage = new List<string>();

                string[] m_headerInfo = { "Tablename", "Fieldname", "Max Length" };

                for (int i = sheet.FirstRowNum + 3; i < rowCount; i++)
                {

                    try
                    {
                        IRow row = sheet.GetRow(i);
                        ICell m_cellValue = null;

                        if (row.GetCell(1, MissingCellPolicy.RETURN_BLANK_AS_NULL) == null)
                        {
                            break;
                        }

                        if (String.IsNullOrEmpty(row.GetCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemPriceVM.Prop.ItemID.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value).ToString())
                            && String.IsNullOrEmpty(row.GetCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemPriceVM.Prop.PriceTypeID.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value).ToString()))
                        {
                            break;
                        }
                        m_cellValue = row.GetCell(0);
                        if (!m_headerInfo.Contains(m_cellValue == null ? string.Empty : m_cellValue.ToString()) && !string.IsNullOrEmpty(row.GetCell(1).ToString()))
                        {

                            ItemPriceUploadVM m_objItemPriceVM = new ItemPriceUploadVM();
                            //m_objItemPriceVM.ItemPriceID = Guid.NewGuid().ToString().Replace("-", "");

                            #region Item Price
                            //ItemID
                            m_cellValue = row.GetCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemPriceVM.Prop.ItemID.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                            if (m_cellValue == null || String.IsNullOrEmpty(m_cellValue.ToString()))
                            {
                                m_lstMessage.Add(String.Format("Row [{0}] Column {1} {2}", i, ItemPriceVM.Prop.ItemID.Desc, General.EnumDesc(MessageLib.mustFill)));
                            }
                            else
                            {
                                m_objItemPriceVM.ItemID = m_cellValue.ToString();
                            }

                            //ItemID
                            m_cellValue = row.GetCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemPriceUploadVM.Prop.ItemDesc.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                            if (m_cellValue == null || String.IsNullOrEmpty(m_cellValue.ToString()))
                            {
                                m_lstMessage.Add(String.Format("Row [{0}] Column {1} {2}", i, ItemPriceUploadVM.Prop.ItemDesc.Desc, General.EnumDesc(MessageLib.mustFill)));
                            }
                            else
                            {
                                m_objItemPriceVM.ItemDesc = m_cellValue.ToString();
                            }

                            //RegionID
                            m_cellValue = row.GetCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemPriceVM.Prop.RegionID.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                            if (m_cellValue == null || String.IsNullOrEmpty(m_cellValue.ToString()))
                            {
                                m_lstMessage.Add(String.Format("Row [{0}] Column {1} {2}", i, ItemPriceVM.Prop.RegionID.Desc, General.EnumDesc(MessageLib.mustFill)));
                            }
                            else
                            {
                                m_objItemPriceVM.RegionID = m_cellValue.ToString();
                            }

                            //ProjectID
                            m_cellValue = row.GetCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemPriceVM.Prop.ProjectID.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                            m_objItemPriceVM.ProjectID = m_cellValue == null ? string.Empty : m_cellValue.ToString();


                            //ClusterID
                            m_cellValue = row.GetCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemPriceVM.Prop.ClusterID.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                            m_objItemPriceVM.ClusterID = m_cellValue == null ? string.Empty : m_cellValue.ToString();


                            //UnitTypeID
                            m_cellValue = row.GetCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemPriceVM.Prop.UnitTypeID.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                            m_objItemPriceVM.UnitTypeID = m_cellValue == null ? string.Empty : m_cellValue.ToString();


                            //PriceTypeID
                            m_cellValue = row.GetCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemPriceVM.Prop.PriceTypeID.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                            m_objItemPriceVM.PriceTypeID = m_cellValue == null ? string.Empty : m_cellValue.ToString();


                            //Get ItemPriceID if exist
                            string m_strMessage = string.Empty;
                            ItemPriceVM m_objItemPrice = GetItemPrice(m_objItemPriceVM.ItemID, ref m_strMessage, m_objItemPriceVM.RegionID, m_objItemPriceVM.ProjectID??string.Empty, m_objItemPriceVM.ClusterID ?? string.Empty, m_objItemPriceVM.UnitTypeID ?? string.Empty);
                            if (m_strMessage == string.Empty)
                            {
                                m_objItemPriceVM.ItemPriceID = m_objItemPrice.ItemPriceID;
                                //m_objItemPriceVM.ItemID = m_listItemPriceVM.FirstOrDefault().ItemID;
                            }
                            else
                            {
                                m_objItemPriceVM.ItemPriceID = $"MIG-{DateTime.Now.ToString("ddMMyy-hhmmssfff")}-{i}";
                            }

                            #endregion

                            #region Item Price Vendor
                            //VendorID
                            m_cellValue = row.GetCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemPriceVendorVM.Prop.VendorID.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                            m_objItemPriceVM.VendorID = m_cellValue == null ? string.Empty : m_cellValue.ToString();


                            //IsDefault
                            m_cellValue = row.GetCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemPriceVendorVM.Prop.IsDefault.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                            m_objItemPriceVM.IsDefault = m_cellValue != null ? ((m_cellValue.ToString() == "1" ? true : false)) : false;
                            //if (m_cellValue == null || String.IsNullOrEmpty(m_cellValue.ToString()))
                            //{
                            //    m_objItemPriceVM.IsDefault = (m_cellValue.ToString() == "1" ? true : false);
                            //    //m_lstMessage.Add(String.Format("Row [{0}] Column {1} {2}", i, ItemPriceVendorVM.Prop.IsDefault.Desc, General.EnumDesc(MessageLib.mustFill)));
                            //}
                            //else
                            //{
                            //    m_objItemPriceVM.IsDefault = (m_cellValue.ToString() == "1" ? true : false);
                            //}

                            //Get ItemPriceID if exist
                            //m_strMessage = string.Empty;
                            //List<ItemPriceVendorVM> m_listItemPriceVendorVM = GetListItemPriceVendor(m_objItemPriceVM.ItemPriceID, m_objItemPriceVM.VendorID);
                            //if (m_strMessage == string.Empty)
                            //{
                            //    m_objItemPriceVM.ItemPriceID = m_listItemPriceVM.FirstOrDefault().ItemPriceID;
                            //}

                            #endregion

                            #region Item Price Vendor Period
                            //ValidFrom
                            m_cellValue = row.GetCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemPriceVendorPeriodVM.Prop.ValidFrom.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                            if (m_cellValue == null || String.IsNullOrEmpty(m_cellValue.ToString()))
                            {
                                m_lstMessage.Add(String.Format("Row [{0}] Column {1} {2}", i, ItemPriceVendorPeriodVM.Prop.ValidFrom.Desc, General.EnumDesc(MessageLib.mustFill)));
                            }
                            else
                            {
                                DateTime m_tempDate;
                                if (DateTime.TryParseExact(m_cellValue.ToString(), "dd-MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out m_tempDate))
                                    m_objItemPriceVM.ValidFrom = m_tempDate;
                            }

                            //ValidTo
                            m_cellValue = row.GetCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemPriceVendorPeriodVM.Prop.ValidTo.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                            if (m_cellValue == null || String.IsNullOrEmpty(m_cellValue.ToString()))
                            {
                                m_lstMessage.Add(String.Format("Row [{0}] Column {1} {2}", i, ItemPriceVendorPeriodVM.Prop.ValidTo.Desc, General.EnumDesc(MessageLib.mustFill)));
                            }
                            else
                            {
                                DateTime m_tempDate;
                                if (DateTime.TryParseExact(m_cellValue.ToString(), "dd-MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out m_tempDate))
                                    m_objItemPriceVM.ValidTo = m_tempDate;
                            }

                            //CurrencyID
                            m_cellValue = row.GetCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemPriceVendorPeriodVM.Prop.CurrencyID.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                            if (m_cellValue == null || String.IsNullOrEmpty(m_cellValue.ToString()))
                            {
                                m_lstMessage.Add(String.Format("Row [{0}] Column {1} {2}", i, ItemPriceVendorPeriodVM.Prop.CurrencyID.Desc, General.EnumDesc(MessageLib.mustFill)));
                            }
                            else
                            {
                                m_objItemPriceVM.CurrencyID = m_cellValue.ToString();
                            }

                            //Amount
                            m_cellValue = row.GetCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemPriceVendorPeriodVM.Prop.Amount.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                            if (m_cellValue == null || String.IsNullOrEmpty(m_cellValue.ToString()))
                            {
                                m_lstMessage.Add(String.Format("Row [{0}] Column {1} {2}", i, ItemPriceVendorPeriodVM.Prop.Amount.Desc, General.EnumDesc(MessageLib.mustFill)));
                            }
                            else
                            {
                                m_objItemPriceVM.Amount = decimal.Parse(m_cellValue.NumericCellValue.ToString());
                            }

                            #endregion

                            if (!m_lstItemPriceVM.Any(d => d.ItemID == m_objItemPriceVM.ItemID &&
                             d.RegionID == m_objItemPriceVM.RegionID &&
                             d.ProjectID == m_objItemPriceVM.ProjectID &&
                             d.ClusterID == m_objItemPriceVM.ClusterID &&
                             d.UnitTypeID == m_objItemPriceVM.UnitTypeID &&
                             d.VendorID == m_objItemPriceVM.VendorID &&
                             d.IsDefault == m_objItemPriceVM.IsDefault &&
                             d.ValidFrom == m_objItemPriceVM.ValidFrom &&
                             d.ValidTo == m_objItemPriceVM.ValidTo &&
                             d.CurrencyID == m_objItemPriceVM.CurrencyID &&
                             d.Amount == m_objItemPriceVM.Amount
                             ))
                            {
                                m_lstItemPriceVM.Add(m_objItemPriceVM);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        m_lstMessage.Add(String.Format("Error Row [{0}] : {1}", i, ex.Message));
                        break;
                    }

                }
                #endregion

                #region Item Details

                List<ItemDetailUploadVM> m_lstItemDetailUploadVM = new List<ItemDetailUploadVM>();
                table = new DataTable();
                headerRow = sheetDetail.GetRow(1);
                cellCount = headerRow.LastCellNum;
                m_dicHeader = new Dictionary<string, int>();
                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                {
                    if (!m_dicHeader.ContainsKey(headerRow.GetCell(i).StringCellValue.Trim()))
                        m_dicHeader.Add(headerRow.GetCell(i).StringCellValue.Trim(), i);
                }
                rowCount = sheetDetail.PhysicalNumberOfRows;
                m_lstMessage = new List<string>();

                string[] m_hdrInfo = { "Tablename", "Fieldname", "Max Length" };

                for (int i = sheetDetail.FirstRowNum + 3; i < rowCount; i++)
                {

                    try
                    {
                        IRow row = sheetDetail.GetRow(i);
                        ICell m_cellValue = null;

                        if (row.GetCell(1, MissingCellPolicy.RETURN_BLANK_AS_NULL) == null)
                        {
                            break;
                        }

                        if (String.IsNullOrEmpty(row.GetCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemPriceVM.Prop.ItemID.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value).ToString()))
                        {
                            break;
                        }
                        m_cellValue = row.GetCell(0);
                        if (!m_hdrInfo.Contains(m_cellValue == null ? string.Empty : m_cellValue.ToString()) && !string.IsNullOrEmpty(row.GetCell(1).ToString()))
                        {

                            ItemDetailUploadVM m_objItemDetailVM = new ItemDetailUploadVM();
                            //m_objItemPriceVM.ItemPriceID = Guid.NewGuid().ToString().Replace("-", "");

                            #region Item Detail
                            //ItemID
                            m_cellValue = row.GetCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemDetailUploadVM.Prop.ItemID.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                            if (m_cellValue == null || String.IsNullOrEmpty(m_cellValue.ToString()))
                            {
                                m_lstMessage.Add(String.Format("Row [{0}] Column {1} {2}", i, ItemDetailUploadVM.Prop.ItemID.Desc, General.EnumDesc(MessageLib.mustFill)));
                            }
                            else
                            {
                                m_objItemDetailVM.ItemID = m_cellValue.ToString();
                            }

                            //ItemDesc
                            m_cellValue = row.GetCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemDetailUploadVM.Prop.ItemDesc.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                            if (m_cellValue == null || String.IsNullOrEmpty(m_cellValue.ToString()))
                            {
                                m_lstMessage.Add(String.Format("Row [{0}] Column {1} {2}", i, ItemDetailUploadVM.Prop.ItemDesc.Desc, General.EnumDesc(MessageLib.mustFill)));
                            }
                            else
                            {
                                m_objItemDetailVM.ItemDesc = m_cellValue.ToString();
                            }

                            //VendorID
                            m_cellValue = row.GetCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemDetailUploadVM.Prop.VendorID.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                            if (m_cellValue == null || String.IsNullOrEmpty(m_cellValue.ToString()))
                            {
                                m_lstMessage.Add(String.Format("Row [{0}] Column {1} {2}", i, ItemDetailUploadVM.Prop.VendorID.Desc, General.EnumDesc(MessageLib.mustFill)));
                            }
                            else
                            {
                                m_objItemDetailVM.VendorID = m_cellValue.ToString();
                            }

                            Array m_objItemDetailTypes = System.Enum.GetValues(typeof(ItemDetailTypes));

                            foreach (string strItemDetailType in m_objItemDetailTypes)
                            {
                                var propertyInfo = m_objItemDetailVM.GetType().GetProperty(strItemDetailType);
                                m_cellValue = row.GetCell(m_dicHeader.Where(x => x.Key.Trim().Equals(strItemDetailType, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                                propertyInfo.SetValue(m_objItemDetailVM, m_cellValue == null ? string.Empty : m_cellValue.ToString());

                            }

                                //Brand
                                //m_cellValue = row.GetCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemDetailTypes.Brand.ToString(), StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                                //m_objItemDetailVM.Brand = m_cellValue == null ? string.Empty : m_cellValue.ToString();


                                ////Height
                                //m_cellValue = row.GetCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemDetailTypes.Height.ToString(), StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                                //m_objItemDetailVM.Height = m_cellValue == null ? string.Empty : m_cellValue.ToString();


                                ////Length
                                //m_cellValue = row.GetCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemDetailTypes.Length.ToString(), StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                                //m_objItemDetailVM.Length = m_cellValue == null ? string.Empty : m_cellValue.ToString();


                                ////Width
                                //m_cellValue = row.GetCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemDetailTypes.Width.ToString(), StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                                //m_objItemDetailVM.Width = m_cellValue == null ? string.Empty : m_cellValue.ToString();

                                ////Weight
                                //m_cellValue = row.GetCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemDetailTypes.Weight.ToString(), StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                                //m_objItemDetailVM.Weight = m_cellValue == null ? string.Empty : m_cellValue.ToString();

                                ////Warranty
                                //m_cellValue = row.GetCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemDetailTypes.Warranty.ToString(), StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                                //m_objItemDetailVM.Warranty = m_cellValue == null ? string.Empty : m_cellValue.ToString();

                                ////Delivery
                                //m_cellValue = row.GetCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemDetailTypes.Delivery.ToString(), StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                                //m_objItemDetailVM.Delivery = m_cellValue == null ? string.Empty : m_cellValue.ToString();

                                ////Retention
                                //m_cellValue = row.GetCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemDetailTypes.Retention.ToString(), StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                                //m_objItemDetailVM.Retention = m_cellValue == null ? string.Empty : m_cellValue.ToString();

                                ////Maintenance Period
                                //m_cellValue = row.GetCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemDetailTypes.MaintenancePeriod.ToString(), StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                                //m_objItemDetailVM.MaintenancePeriod = m_cellValue == null ? string.Empty : m_cellValue.ToString();

                                ////Remarks
                                //m_cellValue = row.GetCell(m_dicHeader.Where(x => x.Key.Trim().Equals(ItemDetailTypes.Descriptions.ToString(), StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value);
                                //m_objItemDetailVM.Descriptions = m_cellValue == null ? string.Empty : m_cellValue.ToString();

                                //Get ItemPriceID if exist
                                //string m_strMessage = string.Empty;
                                //ItemPriceVM m_objItemPrice = GetItemPrice(m_objItemPriceVM.ItemID, ref m_strMessage, m_objItemPriceVM.RegionID, m_objItemPriceVM.ProjectID ?? string.Empty, m_objItemPriceVM.ClusterID ?? string.Empty, m_objItemPriceVM.UnitTypeID ?? string.Empty);
                                //if (m_strMessage == string.Empty)
                                //{
                                //    m_objItemPriceVM.ItemPriceID = m_objItemPrice.ItemPriceID;
                                //    //m_objItemPriceVM.ItemID = m_listItemPriceVM.FirstOrDefault().ItemID;
                                //}
                                //else
                                //{
                                //    m_objItemPriceVM.ItemPriceID = $"MIG-{DateTime.Now.ToString("ddMMyy-hhmmssfff")}-{i}";
                                //}

                                #endregion


                                m_lstItemDetailUploadVM.Add(m_objItemDetailVM);
                                //}
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        m_lstMessage.Add(String.Format("Error Row [{0}] : {1}", i, ex.Message));
                        break;
                    }

                }
                #endregion

                if (m_lstMessage.Count <= 0)
                {
                    //if (!m_objDItemPriceDA.Success || m_objDItemPriceDA.Message != string.Empty)
                    //    m_lstMessage.Add(m_objDItemPriceDA.Message);
                    //else
                    //    m_objDItemPriceDA.EndTrans(ref m_objDBConnection, true, m_strTransName);

                    return this.Direct(new { dataItem = m_lstItemVM, dataItemPrice = m_lstItemPriceVM, dataItemDetails = m_lstItemDetailUploadVM });
                }
                else
                {
                    Global.ShowErrorAlert(title, String.Join(Global.NewLineSeparated, m_lstMessage));
                    return this.Direct();
                }
            }
            catch (Exception ex)
            {
                Global.ShowErrorAlert(title, String.Join(Global.NewLineSeparated, ex.Message));
                return this.Direct();
            }
        }
        public ActionResult SaveUpload(string Action)
        {
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            DItemPriceDA m_objDItemPriceDA = new DItemPriceDA();
            DItemPriceVendorDA m_objDItemPriceVendorDA = new DItemPriceVendorDA();
            DItemPriceVendorPeriodDA m_objDItemPriceVendorPeriodDA = new DItemPriceVendorPeriodDA();

            DItemVersionDA m_objDItemVersionDA = new DItemVersionDA();
            MItemDA m_objMItemDA = new MItemDA();
            DItemParameterDA m_objDItemParameterDA = new DItemParameterDA();


            string m_strItemID = string.Empty;
            m_objMItemDA.ConnectionStringName = Global.ConnStrConfigName;
            m_objDItemVersionDA.ConnectionStringName = Global.ConnStrConfigName;
            m_objDItemPriceDA.ConnectionStringName = Global.ConnStrConfigName;
            m_objDItemPriceVendorPeriodDA.ConnectionStringName = Global.ConnStrConfigName;
            m_objDItemPriceVendorDA.ConnectionStringName = Global.ConnStrConfigName;
            ConfigVM m_objConfigVM = GetVendorConfig();

            MTasksDA m_objMTaskDA = new MTasksDA();
            m_objMTaskDA.ConnectionStringName = Global.ConnStrConfigName;

            DTaskDetailsDA m_objDTaskDetailsDA = new DTaskDetailsDA();
            m_objDTaskDetailsDA.ConnectionStringName = Global.ConnStrConfigName;

            MItemUploadDA m_objMItemUploadDA = new MItemUploadDA();
            m_objMItemUploadDA.ConnectionStringName = Global.ConnStrConfigName;

            DItemPriceUploadDA m_objDItemPriceUploadDA = new DItemPriceUploadDA();
            m_objDItemPriceUploadDA.ConnectionStringName = Global.ConnStrConfigName;

            DItemDetailUploadDA m_objDItemDetailUploadDA = new DItemDetailUploadDA();
            m_objDItemDetailUploadDA.ConnectionStringName = Global.ConnStrConfigName;

            string m_strTransName = "ItemPrice";
            string m_strTransItemName = "Items";
            object m_objDBConnection = null;
            object m_objDBConnection_ = null;
            string m_strMessage = string.Empty;
            string m_ItemID = string.Empty;
            string m_RegionID = string.Empty;
            UserVM m_userLogged = getCurentUser();
            try
            {
                string m_strListItemPriceVM = this.Request.Params[ItemVM.Prop.ListItemPriceVM.Name];
                string m_strListItemVM = this.Request.Params["ListItemVM"];
                string m_strListItemDetailVM = this.Request.Params["ListItemDetailVM"];

                List<ItemPriceUploadVM> m_lstItemPriceVM = JSON.Deserialize<List<ItemPriceUploadVM>>(m_strListItemPriceVM);
                List<ItemVM> m_lstItemVM = JSON.Deserialize<List<ItemVM>>(m_strListItemVM);
                List<ItemDetailUploadVM> m_lstItemDetailUploadVM = JSON.Deserialize<List<ItemDetailUploadVM>>(m_strListItemDetailVM);

                // m_lstMessage = IsSaveValid(Action, m_objMItemVM);
                if (m_lstMessage.Count <= 0)
                {
                    List<string> success_status = new List<string>();
                    

                    m_objDBConnection_ = m_objMTaskDA.BeginTrans(m_strTransName);

                    #region MTask & DTaskDetails 
                    //Mtask
                    if (m_lstMessage.Count <= 0)
                    {
                        string messageErr = "";
                        MTasks m_objMTask = new MTasks();

                        #region Get Approval
                        CApprovalPathDA apPathDA = new CApprovalPathDA();
                        apPathDA.ConnectionStringName = Global.ConnStrConfigName;

                        List<object> m_lstFilter = new List<object>();
                        Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                        m_lstFilter.Add(Operator.Equals);
                        m_lstFilter.Add(General.EnumDesc(TaskType.UploadItem));
                        m_objFilter.Add(ApprovalPathVM.Prop.TaskTypeID.Map, m_lstFilter);

                        m_lstFilter = new List<object>();
                        m_lstFilter.Add(Operator.Equals);
                        m_lstFilter.Add(string.Empty);
                        m_objFilter.Add(ApprovalPathVM.Prop.RoleParentID.Map, m_lstFilter);

                        List<string> m_lstSelect = new List<string>();
                        m_lstSelect.Add(ApprovalPathVM.Prop.RoleID.MapAlias);

                        Dictionary<string, OrderDirection> m_dicOrderBy = new Dictionary<string, OrderDirection>();
                        // m_dicOrderBy.Add(UserRoleVM.Prop.CreatedDate.Map, OrderDirection.Ascending);

                        Dictionary<int, DataSet> m_dicCAppPathDA = apPathDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null, null);
                        string RoleIDUser = "";
                        if (string.IsNullOrEmpty(apPathDA.Message))
                            RoleIDUser = m_dicCAppPathDA[0].Tables[0].Rows[0][ApprovalPathVM.Prop.RoleID.Name].ToString();

                        //string message_ = "";
                        //bool isAllowedToCreateConfig = CheckMatchedRole(RoleIDUser, ref message_);
                        //if (message_.Length > 0)
                        //    m_lstMessage.Add(message_);
                        #endregion

                        m_objMTask.TaskTypeID = General.EnumDesc(TaskType.UploadItem);
                        m_objMTask.TaskDesc = $@"Upload Item";
                        m_objMTask.TaskDateTimeStamp = DateTime.Now;
                        m_objMTask.TaskOwnerID = GetCurrentApproval(General.EnumDesc(TaskType.UploadItem), 2);// "PWR";//GetCurrentApproval(TaskType.ScheduleInvitation.ToString(),0);//TODO  get first level approval ownerid
                        m_objMTask.StatusID = (int)TaskStatus.InProgress;
                        m_objMTask.CurrentApprovalLvl = 2; //jump to catappr
                        m_objMTask.Remarks = $"New Uploaded By : { m_userLogged.EmployeeName }";
                        m_objMTask.Summary = $@"Uploaded By : { m_userLogged.EmployeeName }. Total : {m_lstItemPriceVM.Count} Item(s)";

                        m_objMTaskDA.Data = m_objMTask;
                        if (messageErr.Length == 0)
                        {
                            //if (isAdd)
                            m_objMTaskDA.Insert(true, m_objDBConnection_);

                            if (!m_objMTaskDA.Success || m_objMTaskDA.Message != string.Empty)
                                m_lstMessage.Add(m_objMTaskDA.Message);
                        }
                        else
                            m_lstMessage.Add(messageErr);
                    }

                    //DTaskDetails
                    if (m_objMTaskDA.Success && m_lstMessage.Count <= 0)
                    {
                        DTaskDetails m_obTaskDetail = new DTaskDetails();
                        m_objDTaskDetailsDA.Data = m_obTaskDetail;
                        m_obTaskDetail.TaskDetailID = Guid.NewGuid().ToString().Replace("-", "");
                        m_obTaskDetail.TaskID = m_objMTaskDA.Data.TaskID;
                        m_obTaskDetail.StatusID = (int)TaskStatus.Draft;
                        m_obTaskDetail.Remarks = "Upload Item";
                        m_objDTaskDetailsDA.Insert(true, m_objDBConnection_);

                        if (!m_objDTaskDetailsDA.Success || m_objDTaskDetailsDA.Message != string.Empty)
                            m_lstMessage.Add(m_objDTaskDetailsDA.Message);

                    }
                    else
                    {
                        m_objMTaskDA.EndTrans(ref m_objDBConnection_, false, m_strTransName);
                        return this.Direct(false, m_objMTaskDA.Message);
                    }
                    #endregion

                    //Item
                    foreach (ItemVM objItemVM in m_lstItemVM)
                    {
                        #region ItemUpload
                        MItemUpload m_objMItemUpload = new MItemUpload();
                        m_objMItemUpload.ItemUploadID = Guid.NewGuid().ToString().Replace("-", "");
                        m_objMItemUpload.ItemID = objItemVM.ItemID;
                        m_objMItemUpload.ItemDesc = objItemVM.ItemDesc;
                        m_objMItemUpload.ItemGroupID = objItemVM.ItemGroupID;
                        m_objMItemUpload.UoMID = objItemVM.UoMID;
                        m_objMItemUpload.IsActive = objItemVM.IsActive;
                        m_objMItemUpload.TaskID = m_objMTaskDA.Data.TaskID;
                        m_objMItemUploadDA.Data = m_objMItemUpload;

                        m_objMItemUploadDA.Insert(true, m_objDBConnection_);

                        if (!m_objMItemUploadDA.Success || m_objMItemUploadDA.Message != string.Empty)
                        {
                            m_objMItemUploadDA.EndTrans(ref m_objDBConnection_, m_strTransItemName);
                            return this.Direct(false, $"MItem: {m_objMItemDA.Message}, ItemID: {objItemVM.ItemID}, FK:ItemGroupID: {objItemVM.ItemGroupID}, FK:UoMID: {objItemVM.UoMID}");
                        }
                        #endregion
                    }

                    //ItemPrice
                    foreach (ItemPriceUploadVM objItemPriceVM in m_lstItemPriceVM)
                    {
                        #region DItemPriceUpload
                        DItemPriceUpload m_objDItemPriceUpload = new DItemPriceUpload();

                        m_objDItemPriceUpload.ItemPriceUploadID = Guid.NewGuid().ToString().Replace("-", "");
                        m_objDItemPriceUpload.ItemPriceID = objItemPriceVM.ItemPriceID;
                        m_objDItemPriceUpload.ItemID = objItemPriceVM.ItemID;
                        m_objDItemPriceUpload.ItemDesc = objItemPriceVM.ItemDesc;
                        m_objDItemPriceUpload.PriceTypeID = objItemPriceVM.PriceTypeID;
                        m_objDItemPriceUpload.ProjectID = objItemPriceVM.ProjectID;
                        m_objDItemPriceUpload.RegionID = objItemPriceVM.RegionID;
                        m_objDItemPriceUpload.UnitTypeID = objItemPriceVM.UnitTypeID;
                        m_objDItemPriceUpload.ClusterID = objItemPriceVM.ClusterID;

                        m_objDItemPriceUpload.VendorID = objItemPriceVM.VendorID;
                        m_objDItemPriceUpload.IsDefault = objItemPriceVM.IsDefault;

                        m_objDItemPriceUpload.ValidFrom = objItemPriceVM.ValidFrom;
                        m_objDItemPriceUpload.ValidTo = objItemPriceVM.ValidTo;
                        m_objDItemPriceUpload.CurrencyID = objItemPriceVM.CurrencyID;
                        m_objDItemPriceUpload.Amount = objItemPriceVM.Amount;
                        m_objDItemPriceUpload.TaskID = m_objMTaskDA.Data.TaskID;
                        m_objDItemPriceUploadDA.Data = m_objDItemPriceUpload;
                        m_objDItemPriceUploadDA.Insert(true, m_objDBConnection_);

                        if (!m_objDItemPriceUploadDA.Success || m_objDItemPriceUploadDA.Message != string.Empty)
                        {
                            m_objDItemPriceUploadDA.EndTrans(ref m_objDBConnection_, false, m_strTransName);
                            m_lstMessage.Add(m_objDItemPriceUploadDA.Message);
                            return this.Direct(false, m_objDItemPriceUploadDA.Message);
                        }
                        #endregion

                    }

                    //ItemDetail
                    foreach (ItemDetailUploadVM objItemDetailVM in m_lstItemDetailUploadVM)
                    {
                        Array m_objItemDetailTypes = System.Enum.GetValues(typeof(ItemDetailTypes));

                        foreach (var strItemDetailType in m_objItemDetailTypes)
                        {
                            #region DItemPriceUpload
                            DItemDetailUpload m_objDItemDetailUpload = new DItemDetailUpload();

                            m_objDItemDetailUpload.ItemDetailUploadID = Guid.NewGuid().ToString().Replace("-", "");
                            m_objDItemDetailUpload.ItemDetailID = string.Empty;
                            m_objDItemDetailUpload.ItemID = objItemDetailVM.ItemID;
                            m_objDItemDetailUpload.ItemDesc = objItemDetailVM.ItemDesc;
                            m_objDItemDetailUpload.VendorID = objItemDetailVM.VendorID;
                            m_objDItemDetailUpload.ItemDetailTypeID = General.EnumDesc((ItemDetailTypes)System.Enum.Parse(typeof(ItemDetailTypes), strItemDetailType.ToString()));
                            m_objDItemDetailUpload.ItemDetailTypeDesc = strItemDetailType.ToString();
                            var propertyInfo = objItemDetailVM.GetType().GetProperty(strItemDetailType.ToString());
                            m_objDItemDetailUpload.ItemDetailDesc = propertyInfo.GetValue(objItemDetailVM, null).ToString();
                            m_objDItemDetailUpload.TaskID = m_objMTaskDA.Data.TaskID;
                            m_objDItemDetailUploadDA.Data = m_objDItemDetailUpload;
                            m_objDItemDetailUploadDA.Insert(true, m_objDBConnection_);

                            if (!m_objDItemDetailUploadDA.Success || m_objDItemDetailUploadDA.Message != string.Empty)
                            {
                                m_objDItemDetailUploadDA.EndTrans(ref m_objDBConnection_, false, m_strTransName);
                                m_lstMessage.Add(m_objDItemDetailUploadDA.Message);
                                return this.Direct(false, m_objDItemDetailUploadDA.Message);
                            }
                            #endregion
                        }


                    }



                    if (m_objMTaskDA.Success && m_objMTaskDA.Message == string.Empty)
                        m_objMTaskDA.EndTrans(ref m_objDBConnection_, true, m_strTransItemName);


                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add($"Exception: {ex.Message}, ItemID: {m_ItemID}, RegionID:{m_RegionID}");
                m_objDItemPriceDA.EndTrans(ref m_objDBConnection_, true, m_strTransName);
            }
            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
                return this.Direct(true);
                // return Detail(General.EnumDesc(Buttons.ButtonSave), null, m_strItemID);
            }
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);
        }
    }
}