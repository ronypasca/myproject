using Ciloci.Flee;
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
using System.Web.Script.Serialization;
using XMVC = Ext.Net.MVC;

namespace com.SML.BIGTRONS.Controllers
{
    public class UnitPriceAnalysisController : BaseController
    {
        private readonly string title = "Unit Price Analysis";
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
            DItemVersionDA m_objMItemVersionDA = new DItemVersionDA();
            m_objMItemVersionDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMItemVersion = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter;
            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMItemVersion.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = ItemVersionVM.Prop.Map(m_strDataIndex, false);
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

            List<string> m_lstUPA = new List<string>();

            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            Dictionary<string, List<object>> m_objFilterItemType = new Dictionary<string, List<object>>();
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ConfigVM.Prop.Desc1.MapAlias);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("ItemTypeID");
            m_objFilterItemType.Add(ConfigVM.Prop.Key1.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("UnitPriceAnalysis");
            m_objFilterItemType.Add(ConfigVM.Prop.Key2.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicUConfigDA = m_objUConfigDA.SelectBC(0, null, false, m_lstSelect, m_objFilterItemType, null, null, null);
            if (m_objUConfigDA.Message == string.Empty)
            {
                foreach (DataRow item in m_dicUConfigDA[0].Tables[0].Rows)
                {
                    m_lstUPA.Add(item[ConfigVM.Prop.Desc1.Name].ToString());
                }
            }
            m_lstUPA = m_lstUPA.Distinct().ToList();

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(String.Join(",", m_lstUPA));
            m_objFilter.Add(ItemVersionVM.Prop.ItemTypeID.Map, m_lstFilter);

            m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemVersionVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(ItemVersionVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(ItemVersionVM.Prop.VersionDesc.MapAlias);
            m_lstSelect.Add(ItemVersionVM.Prop.Version.MapAlias);
            m_lstSelect.Add(ItemVersionVM.Prop.ItemTypeDesc.MapAlias);
            m_lstSelect.Add(ItemVersionVM.Prop.ItemGroupDesc.MapAlias);
            m_lstSelect.Add(ItemVersionVM.Prop.UoMDesc.MapAlias);

            Dictionary<int, DataSet> m_dicMItemVersionDA = m_objMItemVersionDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpItemVersionBL in m_dicMItemVersionDA)
            {
                m_intCount = m_kvpItemVersionBL.Key;
                break;
            }

            List<ItemVersionVM> m_lstItemVersionVM = new List<ItemVersionVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;


                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(ItemVersionVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMItemVersionDA = m_objMItemVersionDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMItemVersionDA.Message == string.Empty)
                {
                    m_lstItemVersionVM = (
                        from DataRow m_drMItemVersionDA in m_dicMItemVersionDA[0].Tables[0].Rows
                        select new ItemVersionVM()
                        {
                            ItemID = m_drMItemVersionDA[ItemVersionVM.Prop.ItemID.Name].ToString(),
                            ItemDesc = m_drMItemVersionDA[ItemVersionVM.Prop.ItemDesc.Name].ToString(),
                            VersionDesc = m_drMItemVersionDA[ItemVersionVM.Prop.VersionDesc.Name].ToString(),
                            Version = (int)m_drMItemVersionDA[ItemVersionVM.Prop.Version.Name],
                            ItemTypeDesc = m_drMItemVersionDA[ItemVersionVM.Prop.ItemTypeDesc.Name].ToString(),
                            ItemGroupDesc = m_drMItemVersionDA[ItemVersionVM.Prop.ItemGroupDesc.Name].ToString(),
                            UoMDesc = m_drMItemVersionDA[ItemVersionVM.Prop.UoMDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstItemVersionVM, m_intCount);
        }
        public ActionResult ReadBrowse(StoreRequestParameters parameters, string FilterInItemType, string FilterInItemTypeID)
        {
            DItemVersionDA m_objMItemVersionDA = new DItemVersionDA();
            m_objMItemVersionDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMItemVersion = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMItemVersion.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = ItemVersionVM.Prop.Map(m_strDataIndex, false);
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

            if (FilterInItemTypeID.Length > 0)
            {
                FilterInItemTypeID = JSON.Deserialize<string>(FilterInItemTypeID).Replace("\"", "");

                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.In);
                m_lstFilter.Add(FilterInItemTypeID);
                m_objFilter.Add(ItemVM.Prop.ItemTypeID.Map, m_lstFilter);
            }

            List<string> m_lstSelects = new List<string>();
            m_lstSelects.Add(ItemVersionVM.Prop.ItemID.MapAlias);
            m_lstSelects.Add(ItemVersionVM.Prop.ItemDesc.MapAlias);
            m_lstSelects.Add(ItemVersionVM.Prop.VersionDesc.MapAlias);
            m_lstSelects.Add(ItemVersionVM.Prop.Version.MapAlias);
            m_lstSelects.Add(ItemVersionVM.Prop.ItemTypeDesc.MapAlias);
            m_lstSelects.Add(ItemVersionVM.Prop.ItemGroupDesc.MapAlias);
            m_lstSelects.Add(ItemVersionVM.Prop.UoMDesc.MapAlias);

            Dictionary<int, DataSet> m_dicMItemVersionDA = m_objMItemVersionDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelects, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpItemVersionBL in m_dicMItemVersionDA)
            {
                m_intCount = m_kvpItemVersionBL.Key;
                break;
            }

            List<ItemVersionVM> m_lstItemVersionVM = new List<ItemVersionVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(ItemVersionVM.Prop.ItemID.MapAlias);
                m_lstSelect.Add(ItemVersionVM.Prop.ItemDesc.MapAlias);
                m_lstSelect.Add(ItemVersionVM.Prop.ItemTypeDesc.MapAlias);
                m_lstSelect.Add(ItemVersionVM.Prop.ItemGroupDesc.MapAlias);
                m_lstSelect.Add(ItemVersionVM.Prop.Version.MapAlias);
                m_lstSelect.Add(ItemVersionVM.Prop.VersionDesc.MapAlias);
                m_lstSelect.Add(ItemVersionVM.Prop.ItemTypeID.MapAlias);
                m_lstSelect.Add(ItemVersionVM.Prop.UoMDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(ItemVersionVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMItemVersionDA = m_objMItemVersionDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMItemVersionDA.Message == string.Empty)
                {
                    m_lstItemVersionVM = (
                        from DataRow m_drMItemVersionDA in m_dicMItemVersionDA[0].Tables[0].Rows
                        select new ItemVersionVM()
                        {
                            ItemID = m_drMItemVersionDA[ItemVersionVM.Prop.ItemID.Name].ToString(),
                            ItemDesc = m_drMItemVersionDA[ItemVersionVM.Prop.ItemDesc.Name].ToString(),
                            ItemTypeDesc = m_drMItemVersionDA[ItemVersionVM.Prop.ItemTypeDesc.Name].ToString(),
                            ItemGroupDesc = m_drMItemVersionDA[ItemVersionVM.Prop.ItemGroupDesc.Name].ToString(),
                            Version = (int)m_drMItemVersionDA[ItemVersionVM.Prop.Version.Name],
                            VersionDesc = m_drMItemVersionDA[ItemVersionVM.Prop.VersionDesc.Name].ToString(),
                            UoMDesc = m_drMItemVersionDA[ItemVersionVM.Prop.UoMDesc.Name].ToString(),
                            ItemTypeID = m_drMItemVersionDA[ItemVersionVM.Prop.ItemTypeID.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstItemVersionVM, m_intCount);
        }
        public ActionResult ReadBrowseChild(StoreRequestParameters parameters, string SelectedRow_)
        {
            List<ItemVersionVM> m_lstItemVersionVM = new List<ItemVersionVM>();
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
            DItemVersionDA m_objMItemVersionDA = new DItemVersionDA();
            m_objMItemVersionDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int? m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMItemVersion = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMItemVersion.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = ItemVersionVM.Prop.Map(m_strDataIndex, false);
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
                    m_objFilter.Add(String.Format("({0} = '{1}' OR {2} = '{1}') AND {0} IN ('{3}')", ItemVersionVM.Prop.ItemTypeID.Map, ItemType, ItemVersionVM.Prop.ItemTypeParentID.Map, String.Join("','", m_lstUPA)), m_lstFilter);
                }
                else
                {
                    m_objFilter.Add(String.Format("({0} = '{1}' OR {2} = '{1}') AND {0}='{4}' AND {0} IN ('{3}')", ItemVersionVM.Prop.ItemTypeID.Map, ItemType, ItemVersionVM.Prop.ItemTypeParentID.Map, String.Join("','", m_lstUPA), childType_), m_lstFilter);
                }
            }
            else
            {
                if (m_objFilter.Count == 0)
                {
                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.In);
                    m_lstFilter.Add(String.Join(",", m_lstUPA));
                    m_objFilter.Add(ItemVersionVM.Prop.ItemTypeID.Map, m_lstFilter);
                }
            }
            List<string> m_lstSelects = new List<string>();
            m_lstSelects.Add(ItemVersionVM.Prop.ItemID.MapAlias);
            m_lstSelects.Add(ItemVersionVM.Prop.ItemDesc.MapAlias);
            m_lstSelects.Add(ItemVersionVM.Prop.VersionDesc.MapAlias);
            m_lstSelects.Add(ItemVersionVM.Prop.Version.MapAlias);
            m_lstSelects.Add(ItemVersionVM.Prop.ItemTypeDesc.MapAlias);
            m_lstSelects.Add(ItemVersionVM.Prop.ItemGroupDesc.MapAlias);
            m_lstSelects.Add(ItemVersionVM.Prop.UoMDesc.MapAlias);

            Dictionary<int, DataSet> m_dicMItemVersionDA = m_objMItemVersionDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelects, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpItemVersionBL in m_dicMItemVersionDA)
            {
                m_intCount = m_kvpItemVersionBL.Key;
                break;
            }

            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(ItemVersionVM.Prop.ItemID.MapAlias);
                m_lstSelect.Add(ItemVersionVM.Prop.ItemDesc.MapAlias);
                m_lstSelect.Add(ItemVersionVM.Prop.ItemTypeDesc.MapAlias);
                m_lstSelect.Add(ItemVersionVM.Prop.ItemGroupDesc.MapAlias);
                m_lstSelect.Add(ItemVersionVM.Prop.Version.MapAlias);
                m_lstSelect.Add(ItemVersionVM.Prop.VersionDesc.MapAlias);
                m_lstSelect.Add(ItemVersionVM.Prop.ItemTypeID.MapAlias);
                m_lstSelect.Add(ItemVersionVM.Prop.UoMDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(ItemVersionVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMItemVersionDA = m_objMItemVersionDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMItemVersionDA.Message == string.Empty)
                {
                    m_lstItemVersionVM = (
                        from DataRow m_drMItemVersionDA in m_dicMItemVersionDA[0].Tables[0].Rows
                        select new ItemVersionVM()
                        {
                            ItemID = m_drMItemVersionDA[ItemVersionVM.Prop.ItemID.Name].ToString(),
                            ItemDesc = m_drMItemVersionDA[ItemVersionVM.Prop.ItemDesc.Name].ToString(),
                            ItemTypeDesc = m_drMItemVersionDA[ItemVersionVM.Prop.ItemTypeDesc.Name].ToString(),
                            ItemGroupDesc = m_drMItemVersionDA[ItemVersionVM.Prop.ItemGroupDesc.Name].ToString(),
                            Version = (int)m_drMItemVersionDA[ItemVersionVM.Prop.Version.Name],
                            VersionDesc = m_drMItemVersionDA[ItemVersionVM.Prop.VersionDesc.Name].ToString(),
                            UoMDesc = m_drMItemVersionDA[ItemVersionVM.Prop.UoMDesc.Name].ToString(),
                            ItemTypeID = m_drMItemVersionDA[ItemVersionVM.Prop.ItemTypeID.Name].ToString()
                        }
                    ).ToList();
                }
            }
            #endregion
            return this.Store(m_lstItemVersionVM, m_intCount);



        }
        public ActionResult ReadBrowseChildUnion(StoreRequestParameters parameters, string ItemVersionChildID, string ParentItemID, int ParentVersion, int ParentSequence, string ParentItemTypeID, string ItemPriceVM)
        {
            ItemPriceVM m_objItemPriceVM = JSON.Deserialize<ItemPriceVM>(ItemPriceVM);

            #region DItemVersionChild
            DItemVersionChildDA m_objDItemVersionChildDA = new DItemVersionChildDA();
            m_objDItemVersionChildDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMItemVersion = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMItemVersion.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = ItemVersionVM.Prop.Map(m_strDataIndex, false);
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

            if (ItemVersionChildID.Length > 0)
            {
                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(ItemVersionChildID);
                m_objFilter.Add(ItemVersionChildVM.Prop.ItemVersionChildID.Map, m_lstFilter);
            }


            Dictionary<int, DataSet> m_dicDItemVersionChildDA = m_objDItemVersionChildDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpItemVersionBL in m_dicDItemVersionChildDA)
            {
                m_intCount = m_kvpItemVersionBL.Key;
                break;
            }

            List<ItemVersionChildVM> m_lstItemVersionChildVM = new List<ItemVersionChildVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(ItemVersionChildVM.Prop.ItemID.MapAlias);
                m_lstSelect.Add(ItemVersionChildVM.Prop.ItemDesc.MapAlias);
                m_lstSelect.Add(ItemVersionChildVM.Prop.ItemTypeDesc.MapAlias);
                m_lstSelect.Add(ItemVersionChildVM.Prop.ItemGroupDesc.MapAlias);
                m_lstSelect.Add(ItemVersionChildVM.Prop.Version.MapAlias);
                m_lstSelect.Add(ItemVersionChildVM.Prop.VersionDesc.MapAlias);
                m_lstSelect.Add(ItemVersionChildVM.Prop.ItemTypeID.MapAlias);
                m_lstSelect.Add(ItemVersionChildVM.Prop.UoMID.MapAlias);
                m_lstSelect.Add(ItemVersionChildVM.Prop.UoMDesc.MapAlias);
                m_lstSelect.Add(ItemVersionChildVM.Prop.ChildItemID.MapAlias);
                m_lstSelect.Add(ItemVersionChildVM.Prop.ChildItemDesc.MapAlias);
                m_lstSelect.Add(ItemVersionChildVM.Prop.ChildVersion.MapAlias);
                m_lstSelect.Add(ItemVersionChildVM.Prop.Sequence.MapAlias);
                m_lstSelect.Add(ItemVersionChildVM.Prop.ItemVersionChildID.MapAlias);
                m_lstSelect.Add(ItemVersionChildVM.Prop.Coefficient.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(ItemVersionChildVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicDItemVersionChildDA = m_objDItemVersionChildDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objDItemVersionChildDA.Message == string.Empty)
                {
                    m_lstItemVersionChildVM = (
                        from DataRow m_drDItemVersionChildDA in m_dicDItemVersionChildDA[0].Tables[0].Rows
                        select new ItemVersionChildVM()
                        {
                            ItemVersionChildID = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemVersionChildID.Name].ToString(),
                            ItemID = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemID.Name].ToString(),
                            ItemDesc = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemDesc.Name].ToString(),
                            ItemTypeID = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemTypeID.Name].ToString(),
                            ItemTypeDesc = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemTypeDesc.Name].ToString(),
                            ItemGroupDesc = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemGroupDesc.Name].ToString(),
                            Version = (int)m_drDItemVersionChildDA[ItemVersionChildVM.Prop.Version.Name],
                            VersionDesc = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.VersionDesc.Name].ToString(),
                            UoMDesc = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.UoMDesc.Name].ToString(),
                            UoMID = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.UoMID.Name].ToString(),
                            ChildItemID = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemID.Name].ToString(),
                            ChildItemDesc = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemDesc.Name].ToString(),
                            ChildVersion = int.Parse(m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ChildVersion.Name].ToString()),
                            Sequence = int.Parse(m_drDItemVersionChildDA[ItemVersionChildVM.Prop.Sequence.Name].ToString()),
                            Coefficient = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.Coefficient.Name].ToString() == String.Empty ? 0 : 
                                            (decimal)m_drDItemVersionChildDA[ItemVersionChildVM.Prop.Coefficient.Name],
                            MaterialAmount = GetUnitPrice(m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemVersionChildID.Name].ToString(),
                                                            m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemID.Name].ToString(),
                                                            m_objItemPriceVM,
                                                            m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemTypeID.Name].ToString(),
                                                            BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name),
                            WageAmount = GetUnitPrice(m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemVersionChildID.Name].ToString(),
                                                            m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemID.Name].ToString(),
                                                            m_objItemPriceVM,
                                                            m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemTypeID.Name].ToString(),
                                                            BudgetPlanVersionStructureVM.Prop.WageAmount.Name),
                            MiscAmount = GetUnitPrice(m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemVersionChildID.Name].ToString(),
                                                            m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemID.Name].ToString(),
                                                            m_objItemPriceVM,
                                                            m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemTypeID.Name].ToString(),
                                                            BudgetPlanVersionStructureVM.Prop.MiscAmount.Name),
                        }
                    ).ToList();
                }
            }
            #endregion

            #region DItemVersionChildAlt


            DItemVersionChildAltDA m_objDItemVersionChildAltDA = new DItemVersionChildAltDA();
            m_objDItemVersionChildAltDA.ConnectionStringName = Global.ConnStrConfigName;

            m_boolIsCount = true;
            Dictionary<int, DataSet> m_dicDItemVersionChildAltDA = m_objDItemVersionChildAltDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            //m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpItemVersionBL in m_dicDItemVersionChildAltDA)
            {
                m_intCount += m_kvpItemVersionBL.Key;
                break;
            }

            List<ItemVersionChildVM> m_lstItemVersionChildAltVM = new List<ItemVersionChildVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(ItemVersionChildAltVM.Prop.ItemID.MapAlias);
                m_lstSelect.Add(ItemVersionChildAltVM.Prop.ItemDesc.MapAlias);
                m_lstSelect.Add(ItemVersionChildAltVM.Prop.ItemTypeDesc.MapAlias);
                m_lstSelect.Add(ItemVersionChildAltVM.Prop.ItemGroupDesc.MapAlias);
                m_lstSelect.Add(ItemVersionChildAltVM.Prop.Version.MapAlias);
                m_lstSelect.Add(ItemVersionChildAltVM.Prop.VersionDesc.MapAlias);
                m_lstSelect.Add(ItemVersionChildAltVM.Prop.ItemTypeID.MapAlias);
                m_lstSelect.Add(ItemVersionChildAltVM.Prop.UoMDesc.MapAlias);
                m_lstSelect.Add(ItemVersionChildAltVM.Prop.ChildItemID.MapAlias);
                m_lstSelect.Add(ItemVersionChildAltVM.Prop.ChildItemDesc.MapAlias);
                m_lstSelect.Add(ItemVersionChildAltVM.Prop.ChildVersion.MapAlias);
                m_lstSelect.Add(ItemVersionChildAltVM.Prop.Sequence.MapAlias);
                m_lstSelect.Add(ItemVersionChildAltVM.Prop.ItemVersionChildID.MapAlias);
                m_lstSelect.Add(ItemVersionChildAltVM.Prop.Coefficient.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(ItemVersionChildAltVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicDItemVersionChildAltDA = m_objDItemVersionChildAltDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objDItemVersionChildAltDA.Message == string.Empty)
                {
                    m_lstItemVersionChildAltVM = (
                        from DataRow m_drDItemVersionChildAltDA in m_dicDItemVersionChildAltDA[0].Tables[0].Rows
                        select new ItemVersionChildVM()
                        {
                            ItemVersionChildID = m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.ItemVersionChildID.Name].ToString(),
                            ItemID = m_drDItemVersionChildAltDA[ItemVersionChildVM.Prop.ItemID.Name].ToString(),
                            ItemDesc = m_drDItemVersionChildAltDA[ItemVersionChildVM.Prop.ItemDesc.Name].ToString(),
                            ItemTypeID = m_drDItemVersionChildAltDA[ItemVersionChildVM.Prop.ItemTypeID.Name].ToString(),
                            ItemTypeDesc = m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.ItemTypeDesc.Name].ToString(),
                            ItemGroupDesc = m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.ItemGroupDesc.Name].ToString(),
                            ChildVersion = (int)m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.Version.Name],
                            Version = (int)m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.Version.Name],
                            VersionDesc = m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.VersionDesc.Name].ToString(),
                            UoMDesc = m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.UoMDesc.Name].ToString(),
                            Coefficient = m_drDItemVersionChildAltDA[ItemVersionChildVM.Prop.Coefficient.Name].ToString() == String.Empty ? 0 :
                                            (decimal)m_drDItemVersionChildAltDA[ItemVersionChildVM.Prop.Coefficient.Name],
                            ChildItemID = m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.ChildItemID.Name].ToString(),
                            ChildItemDesc = m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.ChildItemDesc.Name].ToString(),

                            MaterialAmount = GetUnitPrice(m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.ItemVersionChildID.Name].ToString(),
                                                            m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.ChildItemID.Name].ToString(),
                                                            m_objItemPriceVM,
                                                            m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.ItemTypeID.Name].ToString(),
                                                            BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name),
                            WageAmount = GetUnitPrice(m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.ItemVersionChildID.Name].ToString(),
                                                            m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.ChildItemID.Name].ToString(),
                                                            m_objItemPriceVM,
                                                            m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.ItemTypeID.Name].ToString(),
                                                            BudgetPlanVersionStructureVM.Prop.WageAmount.Name),
                            MiscAmount = GetUnitPrice(m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.ItemVersionChildID.Name].ToString(),
                                                            m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.ChildItemID.Name].ToString(),
                                                            m_objItemPriceVM,
                                                            m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.ItemTypeID.Name].ToString(),
                                                            BudgetPlanVersionStructureVM.Prop.MiscAmount.Name),
                        }
                    ).ToList();
                }


            }



            #endregion

            var m_lstItemVersionChildUnion = m_lstItemVersionChildVM
                .Union(m_lstItemVersionChildAltVM)
                .Select(p => new
                {
                    ChildItemID = p.ChildItemID,
                    ChildItemDesc = p.ChildItemDesc,
                    ItemTypeDesc = p.ItemTypeDesc,
                    ItemGroupDesc = p.ItemGroupDesc,
                    ChildVersion = p.ChildVersion,
                    VersionDesc = p.VersionDesc,
                    UoMDesc = p.UoMDesc,
                    p.UoMID,
                    ItemTypeID = p.ItemTypeID,
                    ParentItemID = ParentItemID,
                    ParentVersion = ParentVersion,
                    ParentSequence = ParentSequence,

                    ItemID = p.ItemID ?? p.ChildItemID,
                    ItemDesc = p.ItemDesc ?? p.ChildItemDesc,
                    Version = p.Version,
                    p.Coefficient,
                    MaterialAmount = p.MaterialAmount,
                    WageAmount = p.WageAmount,
                    MiscAmount = p.MiscAmount,
                    ItemVersionChildID = p.ItemVersionChildID
                })
                .ToList();



            return this.Store(m_lstItemVersionChildUnion, m_intCount);
        }
        public ActionResult Home()
        {
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 0;
            return this.Direct();
        }
        public ActionResult HomeStructure()
        {
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return this.Direct();
        }
        public ActionResult Add(string Caller,string Selected)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Add)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            bool IsCallerFromGetData = false;
            Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();

            ItemVersionVM m_objItemVersionVM = new ItemVersionVM();
            Node m_rootNode = new Node();
            m_rootNode.NodeID = "root";
            m_rootNode.Expanded = true;

            #region sample add node if data existing
            /* NodeCollection nodes = new NodeCollection();
            nodes.Add(new Node()
            {
                AttributesObject = new
                {
                    itemversionchildid = "ItemVersionTypeID",
                    childitemid = "i",
                    versiondesc = "VD",
                    childversion = 1,
                    sequence = 1,
                    sequencedesc = "1",
                    coefficient = 0.9,
                    uomdesc = "UoMDesc",
                    formula = "",
                    formuladesc = "",
                    alternative = new List<object>() { new { ItemID = "1" }, new { ItemID = "2" } }
                },
                Icon = Icon.Folder
            });
            nodes.Add(new Node()
            {
                AttributesObject = new
                {
                    itemversionchildid = "itemversiontypeid",
                    childitemid = "iii",
                    versiondesc = "vd",
                    childversion = 1,
                    sequence = 1,
                    sequencedesc = "2",
                    coefficient = 0.9,
                    uomdesc = "uomdesc",
                    formula = "",
                    formuladesc = "",
                    iconCls = "icon-folder",
                    alternative = new List<object>() { new { ItemID = "1" }, new { ItemID = "2" } }
                }
            });

            m_rootNode.Children.AddRange(nodes);*/
            #endregion

            m_objItemVersionVM.Structure = m_rootNode;
            ViewDataDictionary m_vddItemVersion = new ViewDataDictionary();
            m_vddItemVersion.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddItemVersion.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
            if (Caller == General.EnumDesc(Buttons.ButtonDetail))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                m_dicSelectedRow = GetFormData(m_nvcParams);
                Session[dataSessionName] = JSON.Serialize(m_dicSelectedRow);
            }else 
            if (Caller == "GetData")
            {
                IsCallerFromGetData = true;
                m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
                string m_strMessage = string.Empty;
                if (m_dicSelectedRow.Count > 0)
                    m_objItemVersionVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
                if (m_strMessage != string.Empty)
                {
                    Global.ShowErrorAlert(title, m_strMessage);
                    return this.Direct();
                }
                //m_objItemVersionVM.Version = 0;
            }

            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            m_vddItemVersion.Add("IsCopy", bool.TrueString);
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objItemVersionVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddItemVersion,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult Preview(string Caller, string Selected)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Update)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();
            if (Caller == General.EnumDesc(Buttons.ButtonList))
            {
                m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
            }

            string m_strMessage = string.Empty;
            ItemVersionVM m_objItemVersionVM = new ItemVersionVM();
            if (m_dicSelectedRow.Count > 0)
                m_objItemVersionVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            m_objItemVersionVM.RegionID = string.Empty;
            m_objItemVersionVM.ProjectID = string.Empty;
            m_objItemVersionVM.ClusterID = string.Empty;
            m_objItemVersionVM.UnitTypeID = string.Empty;

            decimal m_decTotalUnitPrice = 0;
            m_objItemVersionVM.ListItemVersionChildVM = GetListItemVersionChildVM(m_objItemVersionVM, ref m_strMessage, ref m_decTotalUnitPrice);

            if (m_objItemVersionVM.ListItemVersionChildVM.Count > 0)
            {
                decimal m_objgrandtotal = 0;
                string m_objitemid = m_objItemVersionVM.ListItemVersionChildVM.Where(x => x.FirstLevel == true).FirstOrDefault().ItemID;
                int m_objrowno = m_objItemVersionVM.ListItemVersionChildVM.Count();
                foreach (var item in m_objItemVersionVM.ListItemVersionChildVM)
                {
                    if (item.ChildItemID != null)
                    {
                        m_objgrandtotal += decimal.Parse(item.TotalUnitPrice.ToString());
                    }
                }
                var m_objVersionChildVMGrandTotal = new ItemVersionChildVM()
                {
                    ChildItemDesc = "Grand Total",
                    FirstLevel = true,
                    TotalUnitPrice = m_objgrandtotal,
                    RowNo = m_objrowno + 1,
                    ItemID = m_objitemid
                };
                m_objItemVersionVM.ListItemVersionChildVM.Add(m_objVersionChildVMGrandTotal);
            }



            ViewDataDictionary m_vddItemVersion = new ViewDataDictionary();
            m_vddItemVersion.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddItemVersion.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonPreview));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objItemVersionVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddItemVersion,
                ViewName = "_Preview",
                WrapByScriptTag = false
            };
        }

        public ActionResult Search(string ItemID, int Version, string RegionID, string ProjectID, string ClusterID, string UnitTypeID)
        {
            ItemVersionVM m_objItemVersionVM = new ItemVersionVM()
            {
                ItemID = ItemID,
                Version = Version,
                RegionID = RegionID,
                ProjectID = ProjectID,
                ClusterID = ClusterID,
                UnitTypeID = UnitTypeID
            };

            string m_strMessage = string.Empty;

            decimal m_decTotalUnitPrice = 0;
            List<ItemVersionChildVM> m_lstItemVersionVM = GetListItemVersionChildVM(m_objItemVersionVM, ref m_strMessage, ref m_decTotalUnitPrice);
            if (m_lstItemVersionVM.Count > 0)
            {
                decimal m_objgrandtotal = 0;
                int m_objrowno = m_lstItemVersionVM.Count();
                string m_objitemid = m_lstItemVersionVM.Where(x => x.FirstLevel == true).FirstOrDefault().ItemID;
                foreach (var item in m_lstItemVersionVM.Where(x => x.FirstLevel == true))
                {
                    if (string.IsNullOrEmpty(item.ChildItemID))
                    {
                        m_objgrandtotal += decimal.Parse(item.TotalUnitPrice.ToString());
                    }
                }
                var m_objVersionChildVMGrandTotal = new ItemVersionChildVM()
                {
                    ChildItemDesc = "Grand Total",
                    FirstLevel = true,
                    TotalUnitPrice = m_objgrandtotal,
                    RowNo = m_objrowno + 1,
                    ItemID = m_objitemid
                };
                m_lstItemVersionVM.Add(m_objVersionChildVMGrandTotal);
            }


            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }
            return this.Store(m_lstItemVersionVM);
        }
        public ActionResult AddStructure(string Caller)
        {
            ItemVersionChildVM m_objItemVersionChildVM = new ItemVersionChildVM();
            Node m_rootNode = new Node();
            m_rootNode.NodeID = "root";
            m_rootNode.Expanded = true;
            m_rootNode.Expandable = true;

            Dictionary<string, object>[] m_arrItemVersionChild = JSON.Deserialize<Dictionary<string, object>[]>(this.Request.Params["Structure"]);

            NodeCollection m_nodes = new NodeCollection();

            for (int i = 0; i < m_arrItemVersionChild.Length; i++)
            {
                m_nodes.Add(new Node()
                {
                    AttributesObject = new
                    {
                        itemversionchildid = m_arrItemVersionChild[i]["itemversionchildid"],
                        childitemid = m_arrItemVersionChild[i]["childitemid"],
                        versiondesc = m_arrItemVersionChild[i]["versiondesc"],
                        childversion = m_arrItemVersionChild[i]["childversion"],
                        sequence = m_arrItemVersionChild[i]["sequence"],
                        sequencedesc = m_arrItemVersionChild[i]["sequencedesc"],
                        coefficient = m_arrItemVersionChild[i]["coefficient"],
                        uomdesc = m_arrItemVersionChild[i]["uomdesc"],
                        formula = m_arrItemVersionChild[i]["formula"],
                        formuladesc = m_arrItemVersionChild[i]["formuladesc"],
                        iconCls = m_arrItemVersionChild[i]["iconCls"],
                        leaf = m_arrItemVersionChild[i]["leaf"]
                    }
                });
            }

            m_rootNode.Children.AddRange(m_nodes);

            m_objItemVersionChildVM.FormulaItem = m_rootNode;

            m_rootNode = new Node();
            m_rootNode.NodeID = "root";
            m_rootNode.Expanded = true;

            m_objItemVersionChildVM.ItemVersionChildID = Global.GenerateUID(8);
            m_objItemVersionChildVM.ChildStructure = m_rootNode;
            m_objItemVersionChildVM.AlternativeItem = new List<ItemVersionVM>();
            ViewDataDictionary m_vddItemVersion = new ViewDataDictionary();
            m_vddItemVersion.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddItemVersion.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 2;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageTwo),
                Model = m_objItemVersionChildVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddItemVersion,
                ViewName = "Structure/Detail/_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult Detail(string Caller, string Selected, int Version = 0)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ItemVersionVM m_objItemVersionVM = new ItemVersionVM();
            string m_strMessage = string.Empty;
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
                m_dicSelectedRow = GetFormData(m_nvcParams);
            }

            if (m_dicSelectedRow.Count > 0)
                m_objItemVersionVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage, Version);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddUnitPriceAnalysis = new ViewDataDictionary();
            m_vddUnitPriceAnalysis.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
            m_vddUnitPriceAnalysis.Add("IsCopy", bool.FalseString);
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objItemVersionVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddUnitPriceAnalysis,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult DetailStructure(string Caller, string Selected, string nodeID, int Version = 0)
        {
            Dictionary<string, object> m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);

            string m_strMessage = string.Empty;
            ItemVersionChildVM m_objItemVersionChildVM = new ItemVersionChildVM();
            if (m_dicSelectedRow.Count > 0)
                m_objItemVersionChildVM = GetSelectedDataStructure(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            m_objItemVersionChildVM.NodeID = nodeID.Trim('"');

            Node m_rootNode = new Node();
            m_rootNode.NodeID = "root";
            m_rootNode.Expanded = true;
            m_rootNode.Expandable = true;

            Dictionary<string, object>[] m_arrItemVersionChild = JSON.Deserialize<Dictionary<string, object>[]>(this.Request.Params["Structure"]);

            NodeCollection m_nodes = new NodeCollection();

            for (int i = 0; i < m_arrItemVersionChild.Length; i++)
            {
                m_nodes.Add(new Node()
                {
                    AttributesObject = new
                    {
                        itemversionchildid = m_arrItemVersionChild[i]["itemversionchildid"],
                        childitemid = m_arrItemVersionChild[i]["childitemid"],
                        versiondesc = m_arrItemVersionChild[i]["versiondesc"],
                        childversion = m_arrItemVersionChild[i]["childversion"],
                        sequence = m_arrItemVersionChild[i]["sequence"],
                        sequencedesc = m_arrItemVersionChild[i]["sequencedesc"],
                        coefficient = m_arrItemVersionChild[i]["coefficient"],
                        uomdesc = m_arrItemVersionChild[i]["uomdesc"],
                        formula = m_arrItemVersionChild[i]["formula"],
                        formuladesc = m_arrItemVersionChild[i]["formuladesc"],
                        iconCls = m_arrItemVersionChild[i]["iconCls"],
                        leaf = m_arrItemVersionChild[i]["leaf"]
                    }
                });
            }

            m_rootNode.Children.AddRange(m_nodes);

            m_objItemVersionChildVM.FormulaItem = m_rootNode;

            ConfigAHSChildVM m_objFormulaBehavior = (ConfigAHSChildVM)((StoreResult)FormulaBehavior(m_objItemVersionChildVM.ItemTypeID)).Data;
            m_objItemVersionChildVM.FormulaBehavior = m_objFormulaBehavior;

            ViewDataDictionary m_vddStructure = new ViewDataDictionary();
            m_vddStructure.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
            m_vddStructure.Add("nodeID", nodeID);

            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 2;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageTwo),
                Model = m_objItemVersionChildVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddStructure,
                ViewName = "Structure/Detail/_Form",
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
                m_dicSelectedRow = GetFormData(m_nvcParams);
                Session[dataSessionName] = JSON.Serialize(m_dicSelectedRow);
            }

            string m_strMessage = string.Empty;
            ItemVersionVM m_objItemVersionVM = new ItemVersionVM();
            if (m_dicSelectedRow.Count > 0)
                m_objItemVersionVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddItemVersion = new ViewDataDictionary();
            m_vddItemVersion.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddItemVersion.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            m_vddItemVersion.Add("IsCopy", bool.FalseString);
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objItemVersionVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddItemVersion,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult UpdateStructure(string Caller, string Selected, string nodeID)
        {
            Dictionary<string, object> m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);

            string m_strMessage = string.Empty;
            ItemVersionChildVM m_objItemVersionChildVM = new ItemVersionChildVM();
            if (m_dicSelectedRow.Count > 0)
                m_objItemVersionChildVM = GetSelectedDataStructure(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            m_objItemVersionChildVM.NodeID = nodeID.Trim('"');

            Node m_rootNode = new Node();
            m_rootNode.NodeID = "root";
            m_rootNode.Expanded = true;
            m_rootNode.Expandable = true;

            Dictionary<string, object>[] m_arrItemVersionChild = JSON.Deserialize<Dictionary<string, object>[]>(this.Request.Params["Structure"]);

            NodeCollection m_nodes = new NodeCollection();

            for (int i = 0; i < m_arrItemVersionChild.Length; i++)
            {
                m_nodes.Add(new Node()
                {
                    AttributesObject = new
                    {
                        itemversionchildid = m_arrItemVersionChild[i]["itemversionchildid"],
                        childitemid = m_arrItemVersionChild[i]["childitemid"],
                        versiondesc = m_arrItemVersionChild[i]["versiondesc"],
                        childversion = m_arrItemVersionChild[i]["childversion"],
                        sequence = m_arrItemVersionChild[i]["sequence"],
                        sequencedesc = m_arrItemVersionChild[i]["sequencedesc"],
                        coefficient = m_arrItemVersionChild[i]["coefficient"],
                        uomdesc = m_arrItemVersionChild[i]["uomdesc"],
                        formula = m_arrItemVersionChild[i]["formula"],
                        formuladesc = m_arrItemVersionChild[i]["formuladesc"],
                        iconCls = m_arrItemVersionChild[i]["iconCls"],
                        leaf = m_arrItemVersionChild[i]["leaf"]
                    }
                });
            }

            m_rootNode.Children.AddRange(m_nodes);

            m_objItemVersionChildVM.FormulaItem = m_rootNode;

            ConfigAHSChildVM m_objFormulaBehavior = (ConfigAHSChildVM)((StoreResult)FormulaBehavior(m_objItemVersionChildVM.ItemTypeID)).Data;
            m_objItemVersionChildVM.FormulaBehavior = m_objFormulaBehavior;

            ViewDataDictionary m_vddStructure = new ViewDataDictionary();
            m_vddStructure.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddStructure.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            m_vddStructure.Add("nodeID", nodeID);

            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 2;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageTwo),
                Model = m_objItemVersionChildVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddStructure,
                ViewName = "Structure/Detail/_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<ItemVersionVM> m_lstSelectedRow = new List<ItemVersionVM>();
            m_lstSelectedRow = JSON.Deserialize<List<ItemVersionVM>>(Selected);

            DItemVersionDA m_objMItemVersionDA = new DItemVersionDA();
            string m_strItemVersionTransName = "ItemVersionDelete";
            object m_objDBConnection = null;
            m_objMItemVersionDA.ConnectionStringName = Global.ConnStrConfigName;
            m_objDBConnection = m_objMItemVersionDA.BeginTrans(m_strItemVersionTransName);

            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (ItemVersionVM m_objItemVersionVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilterItemVersion = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifItemVersionVM = m_objItemVersionVM.GetType().GetProperties();

                    string m_strItemID = "";
                    int m_intVersion = 0;

                    foreach (PropertyInfo m_pifItemVersionVM in m_arrPifItemVersionVM)
                    {
                        string m_strFieldName = m_pifItemVersionVM.Name;
                        object m_objFieldValue = m_pifItemVersionVM.GetValue(m_objItemVersionVM);
                        if (m_objItemVersionVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilterItemVersion.Add(ItemVersionVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                            if (m_strFieldName == ItemVersionVM.Prop.Version.Name)
                            {
                                m_intVersion = (int)m_objFieldValue;
                            }
                            else if (m_strFieldName == ItemVersionVM.Prop.ItemID.Name)
                            {
                                m_strItemID = (string)m_objFieldValue;
                            }
                        }
                        else break;
                    }

                    /*m_objMItemVersionDA.DeleteBC(m_objFilterItemVersion, true, m_objDBConnection);
                    if (m_objMItemVersionDA.Message != string.Empty)
                    {
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMItemVersionDA.Message);
                        break;
                    }*/

                    if (m_objMItemVersionDA.Message == string.Empty)
                    {
                        DItemVersionChildDA m_objDItemVersionChildDA = new DItemVersionChildDA();
                        m_objDItemVersionChildDA.ConnectionStringName = Global.ConnStrConfigName;

                        DItemVersionChildAltDA m_objDItemVersionChildAltDA = new DItemVersionChildAltDA();
                        DItemVersionChildFormulaDA m_objDItemVersionChildFormulaDA = new DItemVersionChildFormulaDA();

                        List<ItemVersionChildVM> m_lstItemVersionChild = new List<ItemVersionChildVM>();

                        Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                        List<object> m_lstFilter = new List<object>();
                        m_lstFilter.Add(Operator.Equals);
                        m_lstFilter.Add(m_strItemID);
                        m_objFilter.Add(ItemVersionChildVM.Prop.ItemID.Map, m_lstFilter);

                        m_lstFilter = new List<object>();
                        m_lstFilter.Add(Operator.Equals);
                        m_lstFilter.Add(m_intVersion);
                        m_objFilter.Add(ItemVersionChildVM.Prop.Version.Map, m_lstFilter);

                        List<string> m_lstSelects = new List<string>();
                        m_lstSelects.Add(ItemVersionChildVM.Prop.ItemVersionChildID.MapAlias);

                        List<ItemVersionChildVM> m_lstDItemVersionChild = new List<ItemVersionChildVM>();

                        Dictionary<int, DataSet> m_dicDItemVersionChildDA = m_objDItemVersionChildDA.SelectBC(0, null, false, m_lstSelects, m_objFilter, null, null, null);
                        if (m_objDItemVersionChildDA.Message == string.Empty)
                        {
                            m_lstDItemVersionChild = (
                                from DataRow m_drMItemVersionChildDA in m_dicDItemVersionChildDA[0].Tables[0].Rows
                                select new ItemVersionChildVM()
                                {
                                    ItemVersionChildID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemVersionChildID.Name].ToString()
                                }
                            ).ToList();
                        }

                        foreach (ItemVersionChildVM m_DItemVersionChild in m_lstDItemVersionChild)
                        {
                            m_objFilter = new Dictionary<string, List<object>>();
                            m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_DItemVersionChild.ItemVersionChildID);
                            m_objFilter.Add(ItemVersionChildVM.Prop.ItemVersionChildID.Name, m_lstFilter);

                            m_objDItemVersionChildAltDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                            if (!m_objDItemVersionChildAltDA.Success)
                            {
                                m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDItemVersionChildAltDA.Message);
                                break;
                            }
                            m_objDItemVersionChildFormulaDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                            if (!m_objDItemVersionChildFormulaDA.Success)
                            {
                                m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDItemVersionChildFormulaDA.Message);
                                break;
                            }
                            m_objDItemVersionChildDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                            if (m_objDItemVersionChildDA.Message != string.Empty)
                            {
                                m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDItemVersionChildDA.Message);
                                break;
                            }
                        }
                    }

                    if (m_lstMessage.Count != 0) break;

                    m_objMItemVersionDA.DeleteBC(m_objFilterItemVersion, true, m_objDBConnection);
                    if (m_objMItemVersionDA.Message != string.Empty)
                    {
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMItemVersionDA.Message);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
            }
            if (m_lstMessage.Count <= 0)
            {
                m_objMItemVersionDA.EndTrans(ref m_objDBConnection, true, m_strItemVersionTransName);
                Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Deleted));
            }
            else
            {
                m_objMItemVersionDA.EndTrans(ref m_objDBConnection, false, m_strItemVersionTransName);
                Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            }

            return this.Direct();
        }
        public ActionResult Browse(string ControlItemID, string ControlItemDesc = "", string ControlTreePanel = "",
            string ControlAlternativeItem = "", string ControlFromBudgetStructure = "", string SelectedRow_ = "",
            string ControlUoMDesc = "", string ControlItemTypeDesc = "", string ControlItemGroupDesc = "",
            string ControlVersionDesc = "", string ControlVersion = "", string ControlItemTypeID = "",
            string FilterItemID = "", string FilterItemDesc = "", bool FilterAHS = false, string FilterItemTypeID = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddItemVersion = new ViewDataDictionary();
            m_vddItemVersion.Add("Control" + ItemVersionVM.Prop.ItemID.Name, ControlItemID);
            m_vddItemVersion.Add("Control" + ItemVersionVM.Prop.ItemDesc.Name, ControlItemDesc);
            m_vddItemVersion.Add("Control" + ItemVersionVM.Prop.VersionDesc.Name, ControlVersionDesc);
            m_vddItemVersion.Add("Control" + ItemVersionVM.Prop.Version.Name, ControlVersion);
            m_vddItemVersion.Add("Control" + ItemVersionVM.Prop.ItemGroupDesc.Name, ControlItemGroupDesc);
            m_vddItemVersion.Add("Control" + ItemVersionVM.Prop.ItemTypeDesc.Name, ControlItemTypeDesc);
            m_vddItemVersion.Add("Control" + ItemVersionVM.Prop.ItemTypeID.Name, ControlItemTypeID);
            m_vddItemVersion.Add("Control" + ItemVersionVM.Prop.UoMDesc.Name, ControlUoMDesc);
            m_vddItemVersion.Add(ItemVersionVM.Prop.ItemID.Name, FilterItemID);
            m_vddItemVersion.Add(ItemVersionVM.Prop.ItemDesc.Name, FilterItemDesc);
            m_vddItemVersion.Add("ControlFromBudgetStructure", ControlFromBudgetStructure);
            m_vddItemVersion.Add("ControlAlternativeItem", ControlAlternativeItem);
            m_vddItemVersion.Add("SelectedRow_", SelectedRow_);
            m_vddItemVersion.Add("ValueFilterInItemTypeID", FilterItemTypeID);

            string filterItemType = "";

            if (FilterAHS)
            {
                List<string> m_lstUPA = new List<string>();

                UConfigDA m_objUConfigDA = new UConfigDA();
                m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
                Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                List<object> m_lstFilter = new List<object>();

                List<string> m_lstSelect = new List<string>();
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

                MItemTypeDA m_objMItemTypeDA = new MItemTypeDA();
                m_objMItemTypeDA.ConnectionStringName = Global.ConnStrConfigName;

                m_lstSelect = new List<string>();
                m_lstSelect.Add(ItemTypeVM.Prop.ItemTypeID.MapAlias);

                m_objFilter = new Dictionary<string, List<object>>();
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.In);
                m_lstFilter.Add(String.Join(",", m_lstUPA));
                m_objFilter.Add(ItemTypeVM.Prop.ItemTypeParentID.Map, m_lstFilter);

                Dictionary<int, DataSet> m_dicMItemTypeDA = m_objMItemTypeDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
                if (m_objMItemTypeDA.Message == string.Empty)
                {
                    foreach (DataRow item in m_dicMItemTypeDA[0].Tables[0].Rows)
                    {
                        m_lstUPA.Add(item[ItemTypeVM.Prop.ItemTypeID.Name].ToString());
                    }
                }

                m_lstUPA = m_lstUPA.Distinct().ToList();

                filterItemType = String.Join(",", m_lstUPA);

            }

            m_vddItemVersion.Add("ValueFilterAHS", filterItemType);

            m_vddItemVersion.Add("ControlTreePanel", ControlTreePanel);
            //m_vddItemVersion.Add(ItemVersionVM.Prop.ItemVersionDesc.Name, FilterItemVersionDesc);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddItemVersion,
                ViewName = "../UnitPriceAnalysis/_Browse"
            };
        }
        public ActionResult BrowseChildUnion(string ControlItemID, string ControlItemDesc = "", string ControlUoMDesc = "", string ControlItemTypeDesc = "", string ControlItemGroupDesc = "",
            string ControlVersionDesc = "", string ControlVersion = "", string ControlItemTypeID = "",
            string FilterItemID = "", string FilterItemDesc = "", string ItemVersionChildID = "", string ParentItemID = "", string ParentItemTypeID = "", string ParentVersion = "", string ParentSequence = "", string ItemPriceVM = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddItemVersion = new ViewDataDictionary();
            m_vddItemVersion.Add("Control" + ItemVersionChildVM.Prop.ItemID.Name, ControlItemID);
            m_vddItemVersion.Add("Control" + ItemVersionChildVM.Prop.ItemDesc.Name, ControlItemDesc);
            m_vddItemVersion.Add("Control" + ItemVersionChildVM.Prop.VersionDesc.Name, ControlVersionDesc);
            m_vddItemVersion.Add("Control" + ItemVersionChildVM.Prop.Version.Name, ControlVersion);
            m_vddItemVersion.Add("Control" + ItemVersionChildVM.Prop.ItemGroupDesc.Name, ControlItemGroupDesc);
            m_vddItemVersion.Add("Control" + ItemVersionChildVM.Prop.ItemTypeDesc.Name, ControlItemTypeDesc);
            m_vddItemVersion.Add("Control" + ItemVersionChildVM.Prop.ItemTypeID.Name, ControlItemTypeID);
            m_vddItemVersion.Add("Control" + ItemVersionChildVM.Prop.UoMDesc.Name, ControlUoMDesc);
            m_vddItemVersion.Add(ItemVersionChildVM.Prop.ItemID.Name, FilterItemID);
            m_vddItemVersion.Add(ItemVersionChildVM.Prop.ItemDesc.Name, FilterItemDesc);
            m_vddItemVersion.Add(ItemVersionChildVM.Prop.ItemVersionChildID.Name, ItemVersionChildID);
            m_vddItemVersion.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemID.Name, ParentItemID);
            m_vddItemVersion.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemTypeID.Name, ParentItemTypeID ?? string.Empty);
            m_vddItemVersion.Add(BudgetPlanTemplateStructureVM.Prop.ParentVersion.Name, ParentVersion);
            m_vddItemVersion.Add(BudgetPlanTemplateStructureVM.Prop.ParentSequence.Name, ParentSequence);
            m_vddItemVersion.Add("ItemPriceVM", ItemPriceVM);
            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddItemVersion,
                ViewName = "../UnitPriceAnalysis/_BrowseChildUnion"
            };
        }
        public ActionResult BrowseAlternative(string ControlTreePanel, string NodeID, string Alternative)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            string m_strControlTreePanel = JSON.Deserialize<string>(this.Request.Params["ControlTreePanel"]);
            string m_strNodeID = JSON.Deserialize<string>(this.Request.Params["NodeID"]);
            Dictionary<string, object>[] m_arrItemVersion = JSON.Deserialize<Dictionary<string, object>[]>(Alternative);
            if (m_arrItemVersion == null)
                m_arrItemVersion = new List<Dictionary<string, object>>().ToArray();

            string m_strVersionKeyNode = "";
            string m_strItemIDKeyNode = "";
            string m_strItemDescKeyNode = "";
            string m_strItemTypeIDKeyNode = "";

            if (m_arrItemVersion.Length > 0)
            {
                foreach (string item in m_arrItemVersion[0].Keys.ToArray())
                {
                    if (item.ToLower() == ItemVersionVM.Prop.Version.Name.ToLower())
                    {
                        m_strVersionKeyNode = item;
                    }
                    else if (item.ToLower() == ItemVersionVM.Prop.ItemID.Name.ToLower())
                    {
                        m_strItemIDKeyNode = item;
                    }
                    else if (item.ToLower() == ItemVersionVM.Prop.ItemDesc.Name.ToLower())
                    {
                        m_strItemDescKeyNode = item;
                    }
                    else if (item.ToLower() == ItemVersionVM.Prop.ItemTypeID.Name.ToLower())
                    {
                        m_strItemTypeIDKeyNode = item;
                    }
                }
            }

            List<ItemVersionVM> m_lstItemVersionVM = new List<ItemVersionVM>();
            m_lstItemVersionVM = (
                            from Dictionary<string, object> m_dicItemVersionVM in m_arrItemVersion
                            select new ItemVersionVM()
                            {
                                Version = int.Parse(m_dicItemVersionVM[m_strVersionKeyNode].ToString()),
                                ItemID = m_dicItemVersionVM[m_strItemIDKeyNode].ToString(),
                                ItemDesc = m_dicItemVersionVM[m_strItemDescKeyNode].ToString(),
                                ItemTypeID = m_dicItemVersionVM[m_strItemTypeIDKeyNode].ToString()
                            }).ToList();

            ViewDataDictionary m_vddItemVersion = new ViewDataDictionary();
            m_vddItemVersion.Add("ControlTreePanel", m_strControlTreePanel);
            m_vddItemVersion.Add("ValueNodeID", m_strNodeID);


            return new XMVC.PartialViewResult
            {
                Model = m_lstItemVersionVM,
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddItemVersion,
                ViewName = "../UnitPriceAnalysis/Simulation/Alternative/_Browse"
            };
        }

        public ActionResult BrowseReportAHSAlternative(string ControlTreePanel, string Selected, string ProjectID, string RegionID, string UnitTypeID, string ClusterID)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<ItemVersionChildVM> m_lstItemVersionVM = new List<ItemVersionChildVM>();
            ItemVersionChildVM m_objSelected = JSON.Deserialize<ItemVersionChildVM>(Selected);

            ItemPriceVM m_objItemPriceVM = new ItemPriceVM()
            {
                RegionID = RegionID,
                ProjectID = ProjectID,
                ClusterID = ClusterID,
                UnitTypeID = UnitTypeID
            };
            List<ItemVersionChildVM> m_lstItemVersionChildAltVM = new List<ItemVersionChildVM>();
            List<ItemVersionChildVM> m_lstItemVersionChildVM = new List<ItemVersionChildVM>();

            #region DItemVersionChild

            DItemVersionChildDA m_objDItemVersionChildDA = new DItemVersionChildDA();
            m_objDItemVersionChildDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemTypeDesc.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemGroupDesc.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Version.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.VersionDesc.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemTypeID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.UoMDesc.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildItemID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildItemDesc.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildVersion.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Sequence.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemVersionChildID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Coefficient.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildItemTypeID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.UoMID.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(m_objSelected.ItemVersionChildID);
            m_objFilter.Add(ItemVersionChildVM.Prop.ItemVersionChildID.Map, m_lstFilter);

            //m_lstFilter = new List<object>();
            //m_lstFilter.Add(Operator.In);
            //m_lstFilter.Add(m_objSelected.ChildVersion);
            //m_objFilter.Add(ItemVersionChildVM.Prop.Version.Map, m_lstFilter);

            //Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            //foreach (DataSorter m_dtsOrder in parameters.Sort)
            //    m_dicOrder.Add(ItemVersionChildVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

            Dictionary<int, DataSet> m_dicDItemVersionChildDA = m_dicDItemVersionChildDA = m_objDItemVersionChildDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);
            if (m_objDItemVersionChildDA.Message == string.Empty)
            {

                foreach (DataRow m_drDItemVersionChildDA in m_dicDItemVersionChildDA[0].Tables[0].Rows)
                {

                    ItemVersionChildVM m_objItemVersionChildVM = new ItemVersionChildVM();

                    //ItemVersionChildID = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemVersionChildID.Name].ToString(),
                    m_objItemVersionChildVM.ItemID = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemID.Name].ToString();
                    m_objItemVersionChildVM.ItemDesc = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemDesc.Name].ToString();
                    m_objItemVersionChildVM.ItemTypeID = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemTypeID.Name].ToString();
                    m_objItemVersionChildVM.ItemTypeDesc = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemTypeDesc.Name].ToString();
                    m_objItemVersionChildVM.ItemGroupDesc = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemGroupDesc.Name].ToString();
                    m_objItemVersionChildVM.ChildVersion = (int)m_drDItemVersionChildDA[ItemVersionChildVM.Prop.Version.Name];
                    m_objItemVersionChildVM.Version = (int)m_drDItemVersionChildDA[ItemVersionChildVM.Prop.Version.Name];
                    m_objItemVersionChildVM.VersionDesc = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.VersionDesc.Name].ToString();
                    m_objItemVersionChildVM.UoMDesc = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.UoMDesc.Name].ToString();
                    m_objItemVersionChildVM.Coefficient = decimal.Parse(m_drDItemVersionChildDA[ItemVersionChildVM.Prop.Coefficient.Name].ToString());
                    m_objItemVersionChildVM.ChildItemID = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemID.Name].ToString();
                    m_objItemVersionChildVM.ChildItemDesc = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemDesc.Name].ToString();
                    m_objItemVersionChildVM.ChildItemTypeID = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemTypeID.Name].ToString();
                    m_objItemVersionChildVM.UoMID = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.UoMID.Name].ToString();

                    m_objItemVersionChildVM.MaterialAmount = GetUnitPrice(m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemVersionChildID.Name].ToString(),
                            m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemID.Name].ToString(),
                            m_objItemPriceVM,
                            m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemTypeID.Name].ToString(),
                            BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name);
                    m_objItemVersionChildVM.WageAmount = GetUnitPrice(m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemVersionChildID.Name].ToString(),
                            m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemID.Name].ToString(),
                            m_objItemPriceVM,
                            m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemTypeID.Name].ToString(),
                            BudgetPlanVersionStructureVM.Prop.WageAmount.Name);
                    m_objItemVersionChildVM.MiscAmount = GetUnitPrice(m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemVersionChildID.Name].ToString(),
                            m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemID.Name].ToString(),
                            m_objItemPriceVM,
                            m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemTypeID.Name].ToString(),
                            BudgetPlanVersionStructureVM.Prop.MiscAmount.Name);
                    m_objItemVersionChildVM.UnitPrice = m_objItemVersionChildVM.MaterialAmount ?? 0 + m_objItemVersionChildVM.WageAmount ?? 0 + m_objItemVersionChildVM.MiscAmount ?? 0;
                    m_objItemVersionChildVM.TotalUnitPrice = m_objItemVersionChildVM.UnitPrice ?? 0 * m_objItemVersionChildVM.Coefficient;

                    m_lstItemVersionChildVM.Add(m_objItemVersionChildVM);
                }
            }


            #endregion

            #region DItemVersionChildAlt


            DItemVersionChildAltDA m_objDItemVersionChildAltDA = new DItemVersionChildAltDA();
            m_objDItemVersionChildAltDA.ConnectionStringName = Global.ConnStrConfigName;


            m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemVersionChildAltVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(ItemVersionChildAltVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(ItemVersionChildAltVM.Prop.ItemTypeDesc.MapAlias);
            m_lstSelect.Add(ItemVersionChildAltVM.Prop.ItemGroupDesc.MapAlias);
            m_lstSelect.Add(ItemVersionChildAltVM.Prop.Version.MapAlias);
            m_lstSelect.Add(ItemVersionChildAltVM.Prop.VersionDesc.MapAlias);
            m_lstSelect.Add(ItemVersionChildAltVM.Prop.ItemTypeID.MapAlias);
            m_lstSelect.Add(ItemVersionChildAltVM.Prop.UoMDesc.MapAlias);
            m_lstSelect.Add(ItemVersionChildAltVM.Prop.ChildItemID.MapAlias);
            m_lstSelect.Add(ItemVersionChildAltVM.Prop.ChildItemDesc.MapAlias);
            m_lstSelect.Add(ItemVersionChildAltVM.Prop.ChildVersion.MapAlias);
            m_lstSelect.Add(ItemVersionChildAltVM.Prop.Sequence.MapAlias);
            m_lstSelect.Add(ItemVersionChildAltVM.Prop.ItemVersionChildID.MapAlias);
            m_lstSelect.Add(ItemVersionChildAltVM.Prop.Coefficient.MapAlias);
            m_lstSelect.Add(ItemVersionChildAltVM.Prop.ChildItemTypeID.MapAlias);
            m_lstSelect.Add(ItemVersionChildAltVM.Prop.UoMID.MapAlias);

            m_objFilter = new Dictionary<string, List<object>>();
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(m_objSelected.ItemVersionChildID);
            m_objFilter.Add(ItemVersionChildVM.Prop.ItemVersionChildID.Map, m_lstFilter);


            Dictionary<int, DataSet> m_dicDItemVersionChildAltDA = m_dicDItemVersionChildAltDA = m_objDItemVersionChildAltDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);
            if (m_objDItemVersionChildAltDA.Message == string.Empty)
            {

                foreach (DataRow m_drDItemVersionChildAltDA in m_dicDItemVersionChildAltDA[0].Tables[0].Rows)
                {

                    ItemVersionChildVM m_objItemVersionChildVM = new ItemVersionChildVM();

                    //ItemVersionChildID = m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.ItemVersionChildID.Name].ToString(),
                    m_objItemVersionChildVM.ItemID = m_drDItemVersionChildAltDA[ItemVersionChildVM.Prop.ItemID.Name].ToString();
                    m_objItemVersionChildVM.ItemDesc = m_drDItemVersionChildAltDA[ItemVersionChildVM.Prop.ItemDesc.Name].ToString();
                    m_objItemVersionChildVM.ItemTypeID = m_drDItemVersionChildAltDA[ItemVersionChildVM.Prop.ItemTypeID.Name].ToString();
                    m_objItemVersionChildVM.ItemTypeDesc = m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.ItemTypeDesc.Name].ToString();
                    m_objItemVersionChildVM.ItemGroupDesc = m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.ItemGroupDesc.Name].ToString();
                    m_objItemVersionChildVM.ChildVersion = (int)m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.Version.Name];
                    m_objItemVersionChildVM.Version = (int)m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.Version.Name];
                    m_objItemVersionChildVM.VersionDesc = m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.VersionDesc.Name].ToString();
                    m_objItemVersionChildVM.UoMDesc = m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.UoMDesc.Name].ToString();
                    m_objItemVersionChildVM.Coefficient = decimal.Parse(m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.Coefficient.Name].ToString());
                    m_objItemVersionChildVM.ChildItemID = m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.ChildItemID.Name].ToString();
                    m_objItemVersionChildVM.ChildItemDesc = m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.ChildItemDesc.Name].ToString();
                    m_objItemVersionChildVM.ChildItemTypeID = m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.ChildItemTypeID.Name].ToString();
                    m_objItemVersionChildVM.UoMID = m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.UoMID.Name].ToString();

                    m_objItemVersionChildVM.MaterialAmount = GetUnitPrice(m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.ItemVersionChildID.Name].ToString(),
                            m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.ChildItemID.Name].ToString(),
                            m_objItemPriceVM,
                            m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.ItemTypeID.Name].ToString(),
                            BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name);
                    m_objItemVersionChildVM.WageAmount = GetUnitPrice(m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.ItemVersionChildID.Name].ToString(),
                            m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.ChildItemID.Name].ToString(),
                            m_objItemPriceVM,
                            m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.ItemTypeID.Name].ToString(),
                            BudgetPlanVersionStructureVM.Prop.WageAmount.Name);
                    m_objItemVersionChildVM.MiscAmount = GetUnitPrice(m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.ItemVersionChildID.Name].ToString(),
                            m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.ChildItemID.Name].ToString(),
                            m_objItemPriceVM,
                            m_drDItemVersionChildAltDA[ItemVersionChildAltVM.Prop.ItemTypeID.Name].ToString(),
                            BudgetPlanVersionStructureVM.Prop.MiscAmount.Name);
                    m_objItemVersionChildVM.UnitPrice = m_objItemVersionChildVM.MaterialAmount ?? 0 + m_objItemVersionChildVM.WageAmount ?? 0 + m_objItemVersionChildVM.MiscAmount ?? 0;
                    m_objItemVersionChildVM.TotalUnitPrice = m_objItemVersionChildVM.UnitPrice ?? 0 * m_objItemVersionChildVM.Coefficient;

                    m_lstItemVersionChildAltVM.Add(m_objItemVersionChildVM);
                }
            }


            #endregion

            m_lstItemVersionChildAltVM = m_lstItemVersionChildAltVM.Union(m_lstItemVersionChildVM).ToList();

            ViewDataDictionary m_vddItemVersion = new ViewDataDictionary();
            m_vddItemVersion.Add("ControlTreePanel", string.Empty);
            m_vddItemVersion.Add("ValueNodeID", Selected);


            return new XMVC.PartialViewResult
            {
                Model = m_lstItemVersionChildAltVM,
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddItemVersion,
                ViewName = "../UnitPriceAnalysis/Preview/Alternative/_Browse"
            };
        }
        public ActionResult Save(string Action)
        {
            //action = JSON.Deserialize<string>(action);
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            int returnVersion = 0;

            List<string> m_lstMessage = new List<string>();
            DItemVersionDA m_objMItemVersionDA = new DItemVersionDA();
            ItemVM m_objMItemVM = new ItemVM();

            string m_strItemVersionTransName = "ItemVersion_UPA";
            object m_objDBConnection = null;
            m_objMItemVersionDA.ConnectionStringName = Global.ConnStrConfigName;
            m_objDBConnection = m_objMItemVersionDA.BeginTrans(m_strItemVersionTransName);

            try
            {
                string m_strItemVersionID = this.Request.Params[ItemVersionVM.Prop.ItemID.Name];
                bool m_boolIsCopy = bool.Parse(this.Request.Params["IsCopy"]);
                string m_strItemVersionDesc = this.Request.Params[ItemVersionVM.Prop.VersionDesc.Name];
                string m_strVersion = this.Request.Params[ItemVersionVM.Prop.Version.Name].ToString();
                int m_intVersion = this.Request.Params[ItemVersionVM.Prop.Version.Name].ToString().Length == 0 ? (int)0 : int.Parse(this.Request.Params[ItemVersionVM.Prop.Version.Name].ToString());
                List<ItemVersionChildVM> m_lstItemVersionChildVM = new List<ItemVersionChildVM>();

                if (this.Request.Params["ListStructure"] != null)
                {
                    Dictionary<string, object>[] m_arrItemVersionChild = JSON.Deserialize<Dictionary<string, object>[]>(this.Request.Params["ListStructure"]);
                    if (m_arrItemVersionChild == null)
                        m_arrItemVersionChild = new List<Dictionary<string, object>>().ToArray();

                    m_lstItemVersionChildVM = (
                            from Dictionary<string, object> m_dicItemVersionChildVM in m_arrItemVersionChild
                            select new ItemVersionChildVM()
                            {
                                Coefficient = decimal.Parse(m_dicItemVersionChildVM[ItemVersionChildVM.Prop.Coefficient.Name.ToLower()].ToString()),
                                ChildVersion = int.Parse(m_dicItemVersionChildVM[ItemVersionChildVM.Prop.ChildVersion.Name.ToLower()].ToString()),
                                Sequence = int.Parse(m_dicItemVersionChildVM[ItemVersionChildVM.Prop.Sequence.Name.ToLower()].ToString()),
                                ChildItemID = m_dicItemVersionChildVM[ItemVersionChildVM.Prop.ChildItemID.Name.ToLower()].ToString(),
                                ItemVersionChildID = m_dicItemVersionChildVM[ItemVersionChildVM.Prop.ItemVersionChildID.Name.ToLower()].ToString(),
                                VersionDesc = m_dicItemVersionChildVM[ItemVersionChildVM.Prop.VersionDesc.Name.ToLower()].ToString(),
                                SequenceDesc = m_dicItemVersionChildVM[ItemVersionChildVM.Prop.SequenceDesc.Name.ToLower()].ToString(),
                                UoMDesc = m_dicItemVersionChildVM[ItemVersionChildVM.Prop.UoMDesc.Name.ToLower()].ToString(),
                                Formula = m_dicItemVersionChildVM[ItemVersionChildVM.Prop.Formula.Name.ToLower()].ToString(),
                                FormulaDesc = m_dicItemVersionChildVM[ItemVersionChildVM.Prop.FormulaDesc.Name.ToLower()].ToString(),
                                AlternativeItem = convertAlternativeItem(m_dicItemVersionChildVM["alternative"].ToString())

                            }).ToList();
                }

                m_lstMessage = IsSaveValid(Action, m_strItemVersionID, m_strItemVersionDesc);
                if (m_lstMessage.Count <= 0)
                {
                    DItemVersion m_objMItemVersion = new DItemVersion();
                    m_objMItemVersion.ItemID = m_strItemVersionID;
                    m_objMItemVersion.VersionDesc = m_strItemVersionDesc;

                    if (m_intVersion == 0)
                    {
                        Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

                        List<string> m_lstSelect = new List<string>();
                        m_lstSelect.Add(ItemVersionVM.Prop.Version.MapAlias);

                        List<object> m_lstFilter = new List<object>();
                        m_lstFilter.Add(Operator.Equals);
                        m_lstFilter.Add(m_objMItemVersion.ItemID);
                        m_objFilter.Add(ItemVersionVM.Prop.ItemID.Map, m_lstFilter);

                        Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                        m_dicOrder.Add(ItemVersionVM.Prop.Version.Map, OrderDirection.Descending);

                        Dictionary<int, DataSet> m_dicLastVersion = m_objMItemVersionDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, m_dicOrder);
                        //Dictionary<int, DataSet> m_dicLastVersionitemVersion = m_objMItemVersionDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, m_dicOrder);

                        if (m_dicLastVersion[0].Tables[0].Rows.Count > 0)
                        {
                            m_intVersion = (int)m_dicLastVersion[0].Tables[0].Rows[0][ItemVersionVM.Prop.Version.Name] + 1;
                        }
                        else
                        {
                            m_intVersion = (int)1;
                        }
                        returnVersion = m_intVersion;
                    }

                    m_objMItemVersion.Version = m_intVersion;

                    m_objMItemVersionDA.Data = m_objMItemVersion;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                    {
                        m_objMItemVersionDA.Select();
                        m_objMItemVersionDA.Data.VersionDesc = m_strItemVersionDesc;
                    }

                    //m_objMItemVersion.ItemVersionDesc = m_strItemVersionDesc;
                    //m_objMItemVersion.DimensionID = m_strDimensionID;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd)&& !m_boolIsCopy)
                        m_objMItemVersionDA.Insert(false);
                    else
                        m_objMItemVersionDA.Update(false);

                    if (m_objMItemVersionDA.Success)
                    {
                        DItemVersionChildDA m_objDItemVersionChildDA = new DItemVersionChildDA();
                        DItemVersionChildAltDA m_objDItemVersionChildAltDA = new DItemVersionChildAltDA();
                        DItemVersionChildFormulaDA m_objDItemVersionChildFormulaDA = new DItemVersionChildFormulaDA();

                        if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        {
                            #region Insert
                            foreach (ItemVersionChildVM item in m_lstItemVersionChildVM)
                            {
                                m_objDItemVersionChildDA.Data = new DItemVersionChild()
                                {
                                    ChildItemID = item.ChildItemID,
                                    ChildVersion = item.ChildVersion,
                                    ItemID = m_objMItemVersion.ItemID,
                                    Version = m_objMItemVersion.Version,
                                    ItemVersionChildID = m_boolIsCopy ? Global.GenerateUID(8)  : item.ItemVersionChildID,
                                    Sequence = item.Sequence
                                };

                                m_objDItemVersionChildDA.Insert(true, m_objDBConnection);
                                if (!m_objDItemVersionChildDA.Success || m_objDItemVersionChildDA.Message != string.Empty)
                                {
                                    m_lstMessage.Add(m_objDItemVersionChildDA.Message);
                                    break;
                                }

                                if (item.Formula.Length > 0 || item.Coefficient > 0)
                                {
                                    m_objDItemVersionChildFormulaDA.Data = new DItemVersionChildFormula()
                                    {
                                        Formula = item.Formula,
                                        Coefficient = item.Coefficient,
                                        ItemVersionChildID = m_objDItemVersionChildDA.Data.ItemVersionChildID
                                    };
                                    m_objDItemVersionChildFormulaDA.Insert(true, m_objDBConnection);
                                    if (!m_objDItemVersionChildFormulaDA.Success || m_objDItemVersionChildFormulaDA.Message != string.Empty)
                                    {
                                        m_lstMessage.Add(m_objDItemVersionChildFormulaDA.Message);
                                        break;
                                    }
                                }

                                if (item.AlternativeItem.Count > 0)
                                {
                                    foreach (ItemVersionVM Alternativeitem in item.AlternativeItem)
                                    {
                                        m_objDItemVersionChildAltDA.Data = new DItemVersionChildAlt()
                                        {
                                            ItemVersionChildID = m_objDItemVersionChildDA.Data.ItemVersionChildID,
                                            ItemID = Alternativeitem.ItemID,
                                            Version = Alternativeitem.Version
                                        };

                                        m_objDItemVersionChildAltDA.Insert(true, m_objDBConnection);
                                        if (!m_objDItemVersionChildAltDA.Success || m_objDItemVersionChildAltDA.Message != string.Empty)
                                        {
                                            m_lstMessage.Add(m_objDItemVersionChildAltDA.Message);
                                            break;
                                        }

                                    }
                                }

                            }
                            #endregion
                        }
                        else
                        {
                            #region Update
                            List<ItemVersionChildVM> m_lstItemVersionChildVMExist = new List<ItemVersionChildVM>();

                            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_strItemVersionID);
                            m_objFilter.Add(ItemVersionChildVM.Prop.ItemID.Map, m_lstFilter);

                            m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_intVersion);
                            m_objFilter.Add(ItemVersionChildVM.Prop.Version.Map, m_lstFilter);

                            List<string> m_lstSelect = new List<string>();
                            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemVersionChildID.MapAlias);
                            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildItemID.MapAlias);
                            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildVersion.MapAlias);

                            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                            m_dicOrder.Add(ItemVersionChildVM.Prop.Sequence.Map, OrderDirection.Ascending);

                            m_objDItemVersionChildDA.ConnectionStringName = Global.ConnStrConfigName;
                            Dictionary<int, DataSet> m_dicMItemVersionChildDA = m_objDItemVersionChildDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                            if (m_objDItemVersionChildDA.Success)
                            {
                                m_lstItemVersionChildVMExist = (
                                    from DataRow m_drMItemVersionChildDA in m_dicMItemVersionChildDA[0].Tables[0].Rows
                                    select new ItemVersionChildVM()
                                    {
                                        ItemVersionChildID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemVersionChildID.Name].ToString(),
                                        ChildItemID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemID.Name].ToString(),
                                        ChildVersion = (int)m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildVersion.Name]
                                    }
                                ).ToList();

                                foreach (ItemVersionChildVM item in m_lstItemVersionChildVM)
                                {
                                    if (m_lstItemVersionChildVMExist.Where(m => m.ItemVersionChildID == item.ItemVersionChildID).ToList().Count > 0)
                                    {
                                        #region Update Existing Item Version Child

                                        m_objDItemVersionChildDA.Data.ItemVersionChildID = item.ItemVersionChildID;
                                        m_objDItemVersionChildDA.Select();

                                        m_objDItemVersionChildDA.Data.Sequence = item.Sequence;

                                        m_objDItemVersionChildDA.Update(true, m_objDBConnection);
                                        if (!m_objDItemVersionChildDA.Success || m_objDItemVersionChildDA.Message != string.Empty)
                                        {
                                            m_lstMessage.Add(m_objDItemVersionChildDA.Message);
                                            break;
                                        }


                                        Dictionary<string, List<object>> m_objFilterChildFormula = new Dictionary<string, List<object>>();
                                        m_objFilterChildFormula.Add(ItemVersionChildFormulaVM.Prop.ItemVersionChildID.Name, new List<object> { Operator.Equals, item.ItemVersionChildID });
                                        m_objDItemVersionChildFormulaDA.DeleteBC(m_objFilterChildFormula, true, m_objDBConnection);
                                        if (!m_objDItemVersionChildFormulaDA.Success)
                                        {
                                            m_lstMessage.Add(m_objDItemVersionChildFormulaDA.Message);
                                            break;
                                        }

                                        m_objDItemVersionChildFormulaDA.Data = new DItemVersionChildFormula()
                                        {
                                            Formula = item.Formula,
                                            Coefficient = item.Coefficient,
                                            ItemVersionChildID = item.ItemVersionChildID
                                        };

                                        m_objDItemVersionChildFormulaDA.Insert(true, m_objDBConnection);
                                        if (!m_objDItemVersionChildFormulaDA.Success)
                                        {
                                            m_lstMessage.Add(m_objDItemVersionChildFormulaDA.Message);
                                            break;
                                        }

                                        Dictionary<string, List<object>> m_objFilterChildAlt = new Dictionary<string, List<object>>();
                                        m_objFilterChildAlt.Add(ItemVersionChildAltVM.Prop.ItemVersionChildID.Name, new List<object> { Operator.Equals, item.ItemVersionChildID });
                                        m_objDItemVersionChildAltDA.DeleteBC(m_objFilterChildAlt, true, m_objDBConnection);
                                        if (!m_objDItemVersionChildAltDA.Success)
                                        {
                                            m_lstMessage.Add(m_objDItemVersionChildAltDA.Message);
                                            break;
                                        }

                                        foreach (ItemVersionVM Alternativeitem in item.AlternativeItem)
                                        {
                                            m_objDItemVersionChildAltDA.Data = new DItemVersionChildAlt()
                                            {
                                                ItemVersionChildID = item.ItemVersionChildID,
                                                ItemID = Alternativeitem.ItemID,
                                                Version = Alternativeitem.Version
                                            };

                                            m_objDItemVersionChildAltDA.Insert(true, m_objDBConnection);
                                            if (!m_objDItemVersionChildAltDA.Success || m_objDItemVersionChildAltDA.Message != string.Empty)
                                            {
                                                m_lstMessage.Add(m_objDItemVersionChildAltDA.Message);
                                                break;
                                            }
                                        }

                                        #endregion
                                    }
                                    else
                                    {
                                        #region Insert New Item Version Child
                                        m_objDItemVersionChildDA.Data = new DItemVersionChild()
                                        {
                                            ChildItemID = item.ChildItemID,
                                            ChildVersion = item.ChildVersion,
                                            ItemID = m_objMItemVersion.ItemID,
                                            Version = m_objMItemVersion.Version,
                                            ItemVersionChildID = item.ItemVersionChildID,
                                            Sequence = item.Sequence
                                        };

                                        m_objDItemVersionChildDA.Insert(true, m_objDBConnection);
                                        if (!m_objDItemVersionChildDA.Success || m_objDItemVersionChildDA.Message != string.Empty)
                                        {
                                            m_lstMessage.Add(m_objDItemVersionChildDA.Message);
                                            break;
                                        }

                                        if (item.Formula.Length > 0 || item.Coefficient > 0)
                                        {
                                            m_objDItemVersionChildFormulaDA.Data = new DItemVersionChildFormula()
                                            {
                                                Formula = item.Formula,
                                                Coefficient = item.Coefficient,
                                                ItemVersionChildID = item.ItemVersionChildID
                                            };

                                            m_objDItemVersionChildFormulaDA.Insert(true, m_objDBConnection);
                                            if (!m_objDItemVersionChildFormulaDA.Success || m_objDItemVersionChildFormulaDA.Message != string.Empty)
                                            {
                                                m_lstMessage.Add(m_objDItemVersionChildFormulaDA.Message);
                                                break;
                                            }
                                        }

                                        if (item.AlternativeItem.Count > 0)
                                        {
                                            foreach (ItemVersionVM Alternativeitem in item.AlternativeItem)
                                            {
                                                m_objDItemVersionChildAltDA.Data = new DItemVersionChildAlt()
                                                {
                                                    ItemVersionChildID = item.ItemVersionChildID,
                                                    ItemID = Alternativeitem.ItemID,
                                                    Version = Alternativeitem.Version
                                                };

                                                m_objDItemVersionChildAltDA.Insert(true, m_objDBConnection);
                                                if (!m_objDItemVersionChildAltDA.Success || m_objDItemVersionChildAltDA.Message != string.Empty)
                                                {
                                                    m_lstMessage.Add(m_objDItemVersionChildAltDA.Message);
                                                    break;
                                                }

                                            }
                                        }
                                        #endregion
                                    }
                                }

                                foreach (ItemVersionChildVM item in m_lstItemVersionChildVMExist)
                                {
                                    #region Delete Item Version Child
                                    if (m_lstItemVersionChildVM.Where(m => m.ItemVersionChildID == item.ItemVersionChildID).ToList().Count == 0)
                                    {
                                        Dictionary<string, List<object>> m_objFilterChild = new Dictionary<string, List<object>>();
                                        m_objFilterChild.Add(ItemVersionChildVM.Prop.ItemVersionChildID.Name, new List<object> { Operator.Equals, item.ItemVersionChildID });
                                        m_objDItemVersionChildDA.DeleteBC(m_objFilterChild, true, m_objDBConnection);
                                        if (!m_objDItemVersionChildDA.Success)
                                        {
                                            m_lstMessage.Add(m_objDItemVersionChildDA.Message);
                                        }
                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                m_lstMessage.Add(m_objDItemVersionChildDA.Message);
                            }
                            #endregion
                        }
                    }

                    if (!m_objMItemVersionDA.Success || m_objMItemVersionDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMItemVersionDA.Message);
                }

            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
            }
            if (m_lstMessage.Count <= 0)
            {
                m_objMItemVersionDA.EndTrans(ref m_objDBConnection, true, m_strItemVersionTransName);
                Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));

                return Detail(General.EnumDesc(Buttons.ButtonSave), null, returnVersion);
            }

            m_objMItemVersionDA.EndTrans(ref m_objDBConnection, false, m_strItemVersionTransName);
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);
        }
        private List<ItemVersionVM> convertAlternativeItem(string strAlternativeItem)
        {
            List<ItemVersionVM> m_objItemVersionVM = new List<ItemVersionVM>();
            Dictionary<string, object>[] m_arrItemVersion = JSON.Deserialize<Dictionary<string, object>[]>(strAlternativeItem);
            if (m_arrItemVersion == null)
                m_arrItemVersion = new List<Dictionary<string, object>>().ToArray();

            string m_strVersionKeyNode = "";
            string m_strItemIDKeyNode = "";

            if (m_arrItemVersion.Length > 0)
            {
                foreach (string item in m_arrItemVersion[0].Keys.ToArray())
                {
                    if (item.ToLower() == ItemVersionVM.Prop.Version.Name.ToLower())
                    {
                        m_strVersionKeyNode = item;
                    }
                    else if (item.ToLower() == ItemVersionVM.Prop.ItemID.Name.ToLower())
                    {
                        m_strItemIDKeyNode = item;
                    }
                }
            }

            m_objItemVersionVM = (
                            from Dictionary<string, object> m_dicItemVersionVM in m_arrItemVersion
                            select new ItemVersionVM()
                            {
                                Version = int.Parse(m_dicItemVersionVM[m_strVersionKeyNode].ToString()),
                                ItemID = m_dicItemVersionVM[m_strItemIDKeyNode].ToString()
                            }).ToList();

            return m_objItemVersionVM;
        }

        #endregion

        #region Direct Method
        private string GetRegionID(string ProjectID)
        {
            string m_strRegionID = "";

            MProjectDA m_objMProjectDA = new MProjectDA();
            m_objMProjectDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ProjectVM.Prop.RegionID.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(ProjectID);
            m_objFilter.Add(ProjectVM.Prop.ProjectID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMProjectDA = m_objMProjectDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMProjectDA.Success && m_objMProjectDA.Message == String.Empty)
            {
                m_strRegionID = m_dicMProjectDA[0].Tables[0].Rows[0][ProjectVM.Prop.RegionID.Name].ToString();
            }

            return m_strRegionID;
        }
        public ActionResult GetItemPrice(string ItemID, string ProjectID, string ClusterID, string UnitTypeID, string VendorID = "", string NodeID = "", string Formula = "")
        {
            if (Formula.Length > 0 && ItemID.Length == 0)
            {
                string m_strMessage = "";
                return this.Store(new { amount = CalculateFormula(Formula, ref m_strMessage), NodeID = NodeID, Formula = Formula });
            }

            decimal m_decAmount = 0;

            if (ItemID == String.Empty || ProjectID == String.Empty)
            {
                return this.Store(new { amount = m_decAmount, NodeID = NodeID });
            }

            m_decAmount = GetItemPriceAmount(ItemID, ProjectID, ClusterID, UnitTypeID, VendorID);

            #region with Formula

            //if (m_decAmount > 0 && Formula != String.Empty)
            //{
            //    m_decAmount = 0;
            //    int m_intPosStart = 0;
            //    int m_intPosEnd = 0;
            //    while (Formula.IndexOf("[", m_intPosStart) >= 0)
            //    {
            //        m_intPosEnd = Formula.IndexOf("]", m_intPosEnd + 1);
            //        m_intPosStart = Formula.IndexOf("[", m_intPosStart);

            //        if (m_intPosEnd > m_intPosStart)
            //        {
            //            string m_ItemID = Formula.Substring(m_intPosStart + 1, m_intPosEnd - m_intPosStart - 1);
            //            string m_decItemIDAmount = GetItemPriceAmount(m_ItemID, ProjectID, ClusterID, UnitTypeID).ToString();
            //            Formula = Formula.Replace("[" + m_ItemID + "]", m_decItemIDAmount);
            //        }

            //        m_intPosStart = m_intPosStart + 1;
            //    }
            //    string m_strMessage = "";
            //    m_decAmount = CalculateFormula(Formula, ref m_strMessage);
            //}

            #endregion

            return this.Store(new { amount = m_decAmount, NodeID = NodeID, Formula = Formula });
        }

        public ActionResult GetItemPriceVendorPeriode(decimal Amount, string ProjectID, string ClusterID, string UnitTypeID, string NodeID = "", string Formula = "")
        {
            decimal m_decAmount = 0;

            if (ProjectID == String.Empty)
            {
                return this.Store(new { amount = m_decAmount, NodeID = NodeID });
            }

            m_decAmount = Amount;
            if (m_decAmount > 0 && Formula != String.Empty)
            {
                m_decAmount = 0;
                int m_intPosStart = 0;
                int m_intPosEnd = 0;
                while (Formula.IndexOf("[", m_intPosStart) >= 0)
                {
                    m_intPosEnd = Formula.IndexOf("]", m_intPosEnd + 1);
                    m_intPosStart = Formula.IndexOf("[", m_intPosStart);

                    if (m_intPosEnd > m_intPosStart)
                    {
                        string m_ItemID = Formula.Substring(m_intPosStart + 1, m_intPosEnd - m_intPosStart - 1);
                        string m_decItemIDAmount = GetItemPriceAmount(m_ItemID, ProjectID, ClusterID, UnitTypeID).ToString();
                        Formula = Formula.Replace("[" + m_ItemID + "]", m_decItemIDAmount);
                    }

                    m_intPosStart = m_intPosStart + 1;
                }
                string m_strMessage = "";
                m_decAmount = CalculateFormula(Formula, ref m_strMessage);

            }

            return this.Store(new { amount = m_decAmount, NodeID = NodeID });
        }

        //string ItemID, string Version, string Sequence, string SequenceDesc
        public ActionResult GetChild(string ChildItemID, int ChildVersion, string SequenceDesc)
        {
            NodeCollection m_nodeCollectChild = new Ext.Net.NodeCollection();

            if (ChildItemID == null)
            {
                return this.Store(m_nodeCollectChild);
            }

            DItemVersionChildDA m_objMItemVersionChildDA = new DItemVersionChildDA();
            m_objMItemVersionChildDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = 0;
            bool m_boolIsCount = true;

            List<ItemVersionChildVM> m_lstItemVersionChildVM = new List<ItemVersionChildVM>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ChildItemID);
            m_objFilter.Add(ItemVersionChildVM.Prop.ItemID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ChildVersion);
            m_objFilter.Add(ItemVersionChildVM.Prop.Version.Map, m_lstFilter);

            m_boolIsCount = false;
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemVersionChildID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildItemID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildVersion.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.VersionDesc.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Coefficient.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.UoMDesc.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Formula.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Sequence.MapAlias);

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
                        ChildItemID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemID.Name].ToString(),
                        ChildVersion = (int)m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildVersion.Name],
                        VersionDesc = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.VersionDesc.Name].ToString(),
                        Sequence = (int)m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildVersion.Name],
                        SequenceDesc = SequenceDesc + m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildVersion.Name].ToString(),
                        Coefficient = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Coefficient.Name].ToString() == String.Empty ? 0 : (decimal)m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Coefficient.Name],
                        UoMDesc = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.UoMDesc.Name].ToString(),
                        Formula = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Formula.Name].ToString(),
                        FormulaDesc = GenerateFormulaDesc(m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Formula.Name].ToString(), ChildItemID, ChildVersion, SequenceDesc)
                    }
                ).ToList();
            }

            foreach (ItemVersionChildVM item in m_lstItemVersionChildVM)
            {
                List<ItemVersionVM> m_lstAlternativeItem = GetAlternativeItem(item.ItemVersionChildID);
                NodeCollection ex = (NodeCollection)((StoreResult)LoadMyChild(new ItemVersionChildVM() { ChildItemID = item.ChildItemID, ChildVersion = item.ChildVersion })).Data;

                m_nodeCollectChild.Add(new Node()
                {
                    Expanded = false,
                    Expandable = ex.Count > 0,
                    AttributesObject = new
                    {
                        itemversionchildid = item.ItemVersionChildID,
                        childitemid = item.ChildItemID,
                        versiondesc = item.VersionDesc,
                        childversion = item.ChildVersion,
                        sequence = item.Sequence,
                        sequencedesc = item.SequenceDesc,
                        coefficient = item.Coefficient.ToString() == String.Empty ? "" : item.Coefficient.ToString(),
                        uomdesc = item.UoMDesc,
                        formula = item.Formula,
                        formuladesc = item.FormulaDesc,
                        leaf = ex.Count > 0 ? false : true,
                        alternative = m_lstAlternativeItem
                    },
                    Icon = ex.Count > 0 ? Icon.Folder : Icon.PageWhite
                });
            }

            #region sample add node

            //if (dataParent.ChildItemID.Length <= 4)
            //{
            //    string _itemid = dataParent.ChildItemID + "i";

            //    NodeCollection ex = (NodeCollection)((StoreResult)LoadMyChild(new ItemVersionChildVM() { ChildItemID = _itemid, Version = 1 })).Data;

            //    data.Expanded = true;
            //    data.AttributesObject = new
            //    {
            //        itemversionchildid = "itemversionchildid",
            //        childitemid = _itemid,
            //        versiondesc = "vd",
            //        sequence = 1,
            //        childversion = 1,
            //        sequencedesc = dataParent.SequenceDesc + ".1",
            //        coefficient = 0.9,
            //        uomdesc = "uomdesc",
            //        iconCls = "icon-folder",
            //        formula = ""
            //    };

            //    if (dataParent.ChildItemID.Length == 4)
            //    {
            //        data.Leaf = true;
            //        data.AttributesObject = new
            //        {
            //            itemversionchildid = "itemversionchildids",
            //            childitemid = "skipthis",
            //            versiondesc = "versiondesc",
            //            sequence = 1,
            //            childversion = 1,
            //            sequencedesc = dataParent.SequenceDesc + ".1",
            //            coefficient = 0.9,
            //            uomdesc = "uomdesc",
            //            formula = "",
            //            iconCls = "icon-folder",
            //            formuladesc = ""
            //        };
            //        m_nodeCollectChild.Add(data);

            //        data = new Node();
            //        data.Leaf = true;
            //        data.AttributesObject = new
            //        {
            //            itemversionchildid = "itemversionchildid",
            //            childitemid = "jskipthis",
            //            versiondesc = "versiondesc",
            //            sequence = 1,
            //            childversion = 1,
            //            sequencedesc = dataParent.SequenceDesc + ".2",
            //            coefficient = 0.9,
            //            uomdesc = "uomdesc",
            //            iconCls = "icon-folder",
            //            formula = "([iii]+[itemversionchildids])/[iiiii]",
            //            formuladesc = ""
            //        };
            //        m_nodeCollectChild.Add(data);
            //        return this.Store(m_nodeCollectChild);
            //    }
            //}

            //m_nodeCollectChild.Add(data);
            #endregion

            return this.Store(m_nodeCollectChild);

        }

        public ActionResult LoadMyChild(ItemVersionChildVM dataParent)
        {

            NodeCollection m_nodeCollectChild = new Ext.Net.NodeCollection();

            if (dataParent.ChildItemID == null)
            {
                return this.Store(m_nodeCollectChild);
            }

            DItemVersionChildDA m_objMItemVersionChildDA = new DItemVersionChildDA();
            m_objMItemVersionChildDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = 0;
            bool m_boolIsCount = true;

            List<ItemVersionChildVM> m_lstItemVersionChildVM = new List<ItemVersionChildVM>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(dataParent.ChildItemID);
            m_objFilter.Add(ItemVersionChildVM.Prop.ItemID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(dataParent.ChildVersion);
            m_objFilter.Add(ItemVersionChildVM.Prop.Version.Map, m_lstFilter);

            if (dataParent.ChildItemID == null)
            {
                m_lstFilter = new List<object>();
            }

            m_boolIsCount = false;
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemVersionChildID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildItemID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemTypeID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildVersion.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.VersionDesc.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Coefficient.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.UoMDesc.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Formula.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Sequence.MapAlias);

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
                        ChildItemID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemID.Name].ToString(),
                        ChildVersion = (int)m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildVersion.Name],
                        VersionDesc = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.VersionDesc.Name].ToString(),
                        Sequence = (int)m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Sequence.Name],
                        SequenceDesc = dataParent.SequenceDesc + m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Sequence.Name].ToString(),
                        Coefficient = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Coefficient.Name].ToString() == String.Empty ? 0 : (decimal)m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Coefficient.Name],
                        UoMDesc = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.UoMDesc.Name].ToString(),
                        Formula = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Formula.Name].ToString(),
                        FormulaDesc = GenerateFormulaDesc(m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Formula.Name].ToString(), dataParent.ChildItemID, dataParent.ChildVersion, dataParent.SequenceDesc),
                        ItemTypeID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemTypeID.Name].ToString()
                    }
                ).ToList();
            }

            foreach (ItemVersionChildVM item in m_lstItemVersionChildVM)
            {
                List<ItemVersionVM> m_lstAlternativeItem = GetAlternativeItem(item.ItemVersionChildID);
                NodeCollection ex = (NodeCollection)((StoreResult)LoadMyChild(new ItemVersionChildVM() { ChildItemID = item.ChildItemID, ChildVersion = item.ChildVersion })).Data;

                m_nodeCollectChild.Add(new Node()
                {
                    /*Expanded = ex.Count > 0,*/
                    Expandable = ex.Count > 0,
                    AttributesObject = new
                    {
                        itemversionchildid = item.ItemVersionChildID,
                        childitemid = item.ChildItemID,
                        versiondesc = item.VersionDesc,
                        childversion = item.ChildVersion,
                        childitemtypeid = item.ItemTypeID,
                        sequence = item.Sequence,
                        sequencedesc = item.SequenceDesc,
                        coefficient = item.Coefficient.ToString() == String.Empty ? "" : item.Coefficient.ToString(),
                        uomdesc = item.UoMDesc,
                        formula = item.Formula,
                        formuladesc = item.FormulaDesc,
                        leaf = ex.Count > 0 ? false : true,
                        alternative = m_lstAlternativeItem
                    },
                    Icon = ex.Count > 0 ? Icon.Folder : Icon.PageWhite
                });
            }

            #region sample add node

            //if (dataParent.ChildItemID.Length <= 4)
            //{
            //    string _itemid = dataParent.ChildItemID + "i";

            //    NodeCollection ex = (NodeCollection)((StoreResult)LoadMyChild(new ItemVersionChildVM() { ChildItemID = _itemid, Version = 1 })).Data;

            //    data.Expanded = true;
            //    data.AttributesObject = new
            //    {
            //        itemversionchildid = "itemversionchildid",
            //        childitemid = _itemid,
            //        versiondesc = "vd",
            //        sequence = 1,
            //        childversion = 1,
            //        sequencedesc = dataParent.SequenceDesc + ".1",
            //        coefficient = 0.9,
            //        uomdesc = "uomdesc",
            //        iconCls = "icon-folder",
            //        formula = ""
            //    };

            //    if (dataParent.ChildItemID.Length == 4)
            //    {
            //        data.Leaf = true;
            //        data.AttributesObject = new
            //        {
            //            itemversionchildid = "itemversionchildids",
            //            childitemid = "skipthis",
            //            versiondesc = "versiondesc",
            //            sequence = 1,
            //            childversion = 1,
            //            sequencedesc = dataParent.SequenceDesc + ".1",
            //            coefficient = 0.9,
            //            uomdesc = "uomdesc",
            //            formula = "",
            //            iconCls = "icon-folder",
            //            formuladesc = ""
            //        };
            //        m_nodeCollectChild.Add(data);

            //        data = new Node();
            //        data.Leaf = true;
            //        data.AttributesObject = new
            //        {
            //            itemversionchildid = "itemversionchildid",
            //            childitemid = "jskipthis",
            //            versiondesc = "versiondesc",
            //            sequence = 1,
            //            childversion = 1,
            //            sequencedesc = dataParent.SequenceDesc + ".2",
            //            coefficient = 0.9,
            //            uomdesc = "uomdesc",
            //            iconCls = "icon-folder",
            //            formula = "([iii]+[itemversionchildids])/[iiiii]",
            //            formuladesc = ""
            //        };
            //        m_nodeCollectChild.Add(data);
            //        return this.Store(m_nodeCollectChild);
            //    }
            //}

            //m_nodeCollectChild.Add(data);
            #endregion

            return this.Store(m_nodeCollectChild);

        }

        private string GenerateFormulaDesc(string Formula, string ItemID, int Version, string SequenceDesc)
        {
            if (Formula.Length == 0 || Formula == String.Empty)
            {
                return Formula;
            }

            DItemVersionChildDA m_objItemVersionChildDA = new DItemVersionChildDA();
            m_objItemVersionChildDA.ConnectionStringName = Global.ConnStrConfigName;
            object m_objDBConnection = m_objItemVersionChildDA.BeginConnection();

            List<ItemVersionChildVM> m_lstItemVersionChildVM = new List<ItemVersionChildVM>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ItemID);
            m_objFilter.Add(ItemVersionChildVM.Prop.ItemID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(Version);
            m_objFilter.Add(ItemVersionChildVM.Prop.Version.Map, m_lstFilter);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(ItemVersionChildVM.Prop.Sequence.Map, OrderDirection.Ascending);

            bool m_boolIsCount = false;
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemVersionChildID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildItemID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildVersion.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Sequence.MapAlias);

            Dictionary<int, DataSet> m_dicMItemVersionDA = m_objItemVersionChildDA.SelectBC(0, null, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);

            if (m_objItemVersionChildDA.Message == "")
            {
                m_lstItemVersionChildVM = (
                   from DataRow m_drMItemVersionChildDA in m_dicMItemVersionDA[0].Tables[0].Rows
                   select new ItemVersionChildVM()
                   {
                       ItemVersionChildID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemVersionChildID.Name].ToString(),
                       ChildItemID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemID.Name].ToString(),
                       ChildVersion = (int)m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildVersion.Name],
                       SequenceDesc = SequenceDesc == null ? m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Sequence.Name].ToString() : SequenceDesc.Length == 0 ? m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Sequence.Name].ToString() : SequenceDesc + "." + m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Sequence.Name].ToString()
                   }
               ).ToList();
            }

            foreach (ItemVersionChildVM item in m_lstItemVersionChildVM)
            {
                Formula = Formula.Replace(item.ItemVersionChildID, item.ChildItemID + "-" + item.ChildVersion.ToString() + "-" + item.SequenceDesc.ToString());
                Formula = LoopChildOfChild(m_objDBConnection, item.ChildItemID, item.ChildVersion, item.SequenceDesc, Formula);
            }

            m_objItemVersionChildDA.EndConnection(ref m_objDBConnection);

            return Formula;
        }

        public string LoopChildOfChild(object p_DBConncetion, string ParentID, int ParentVersion, string LastSequenceDesc, string Formula)
        {

            DItemVersionChildDA m_objItemVersionChildDA = new DItemVersionChildDA();

            List<ItemVersionChildVM> m_lstItemVersionChildVM = new List<ItemVersionChildVM>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ParentID);
            m_objFilter.Add(ItemVersionChildVM.Prop.ItemID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ParentVersion);
            m_objFilter.Add(ItemVersionChildVM.Prop.Version.Map, m_lstFilter);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(ItemVersionChildVM.Prop.Sequence.Map, OrderDirection.Ascending);

            bool m_boolIsCount = false;
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemVersionChildID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildItemID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildVersion.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Sequence.MapAlias);

            Dictionary<int, DataSet> m_dicMItemVersionDA = m_objItemVersionChildDA.SelectBC(0, null, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, p_DBConncetion);

            if (m_objItemVersionChildDA.Message == "")
            {
                m_lstItemVersionChildVM = (
                   from DataRow m_drMItemVersionChildDA in m_dicMItemVersionDA[0].Tables[0].Rows
                   select new ItemVersionChildVM()
                   {
                       ItemVersionChildID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemVersionChildID.Name].ToString(),
                       ChildItemID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemID.Name].ToString(),
                       ChildVersion = (int)m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildVersion.Name],
                       SequenceDesc = LastSequenceDesc + "." + m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Sequence.Name].ToString()
                   }
               ).ToList();
            }

            foreach (ItemVersionChildVM item in m_lstItemVersionChildVM)
            {
                Formula = Formula.Replace(item.ItemVersionChildID, item.ChildItemID + "-" + item.ChildVersion.ToString() + "-" + item.SequenceDesc.ToString());
                Formula = LoopChildOfChild(p_DBConncetion, item.ChildItemID, item.ChildVersion, item.SequenceDesc, Formula);
            }

            return Formula;
        }

        public ActionResult FormulaBehavior(string ItemTypeID)
        {
            ConfigAHSChildVM m_objReturn = new ConfigAHSChildVM();

            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;

            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            m_lstSelect = new List<string>();
            m_lstSelect.Add(ConfigVM.Prop.Key3.MapAlias);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("ItemTypeID");
            m_objFilter.Add(ConfigVM.Prop.Key1.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("HasCoefficient");
            m_objFilter.Add(ConfigVM.Prop.Key2.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ItemTypeID);
            m_objFilter.Add(ConfigVM.Prop.Key3.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicUConfigDA = m_objUConfigDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_dicUConfigDA[0].Tables[0].Rows.Count > 0)
            {
                m_objReturn.HasCoefficient = true;
            }

            m_objFilter = new Dictionary<string, List<object>>();
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("ItemTypeID");
            m_objFilter.Add(ConfigVM.Prop.Key1.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("HasFormula");
            m_objFilter.Add(ConfigVM.Prop.Key2.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ItemTypeID);
            m_objFilter.Add(ConfigVM.Prop.Key3.Map, m_lstFilter);

            m_dicUConfigDA = m_objUConfigDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_dicUConfigDA[0].Tables[0].Rows.Count > 0)
            {
                m_objReturn.HasFormula = true;
            }

            m_objFilter = new Dictionary<string, List<object>>();
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("ItemTypeID");
            m_objFilter.Add(ConfigVM.Prop.Key1.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("HasAlternative");
            m_objFilter.Add(ConfigVM.Prop.Key2.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ItemTypeID);
            m_objFilter.Add(ConfigVM.Prop.Key3.Map, m_lstFilter);
            m_dicUConfigDA = m_objUConfigDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_dicUConfigDA[0].Tables[0].Rows.Count > 0)
            {
                m_objReturn.HasAlternativeItem = true;
            }

            return this.Store(m_objReturn);
        }

        public ActionResult CircularChecking(string ParentItemID, int ParentVersion, string ChildItemID, int ChildVersion)
        {
            bool m_boolIsCircular = false;

            if (ParentVersion == 0)
            {
                return this.Store(m_boolIsCircular);
            }

            DItemVersionChildDA m_objMItemVersionChildDA = new DItemVersionChildDA();
            m_objMItemVersionChildDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = 0;
            bool m_boolIsCount = true;

            List<ItemVersionChildVM> m_lstItemVersionChildVM = new List<ItemVersionChildVM>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ParentItemID);
            m_objFilter.Add(ItemVersionChildVM.Prop.ChildItemID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ParentVersion);
            m_objFilter.Add(ItemVersionChildVM.Prop.ChildVersion.Map, m_lstFilter);

            m_boolIsCount = false;
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Version.MapAlias);

            Dictionary<int, DataSet> m_dicMItemVersionDA = m_objMItemVersionChildDA.SelectBC(m_intSkip, null, m_boolIsCount, m_lstSelect, m_objFilter, null, null, null, null);
            if (m_objMItemVersionChildDA.Message == string.Empty)
            {

                if (
                    m_dicMItemVersionDA[0].Tables[0].Rows[0][ItemVersionChildVM.Prop.ItemID.Name].ToString() == ChildItemID
                    &&
                    m_dicMItemVersionDA[0].Tables[0].Rows[0][ItemVersionChildVM.Prop.Version.Name].ToString() == ChildVersion.ToString()
                    )
                {
                    m_boolIsCircular = true;
                }
                else
                {
                    m_boolIsCircular = CircularCheckingLoop(m_dicMItemVersionDA[0].Tables[0].Rows[0][ItemVersionChildVM.Prop.ItemID.Name].ToString()
                        , (int)m_dicMItemVersionDA[0].Tables[0].Rows[0][ItemVersionChildVM.Prop.Version.Name]
                        , ChildItemID, ChildVersion);
                }

            }

            return this.Store(m_boolIsCircular);

        }

        private bool CircularCheckingLoop(string ParentItemID, int ParentVersion, string ChildItemID, int ChildVersion)
        {
            bool m_return = false;

            DItemVersionChildDA m_objDItemVersionChildDA = new DItemVersionChildDA();
            m_objDItemVersionChildDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = 0;
            bool m_boolIsCount = true;

            List<ItemVersionChildVM> m_lstItemVersionChildVM = new List<ItemVersionChildVM>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ParentItemID);
            m_objFilter.Add(ItemVersionChildVM.Prop.ChildItemID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ParentVersion);
            m_objFilter.Add(ItemVersionChildVM.Prop.ChildVersion.Map, m_lstFilter);

            m_boolIsCount = false;
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Version.MapAlias);

            Dictionary<int, DataSet> m_dicMItemVersionDA = m_objDItemVersionChildDA.SelectBC(m_intSkip, null, m_boolIsCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDItemVersionChildDA.Message == String.Empty)
            {

                if (
                    m_dicMItemVersionDA[0].Tables[0].Rows[0][ItemVersionChildVM.Prop.ItemID.Name].ToString() == ChildItemID
                    &&
                    m_dicMItemVersionDA[0].Tables[0].Rows[0][ItemVersionChildVM.Prop.Version.Name].ToString() == ChildVersion.ToString()
                    )
                {
                    m_return = true;
                }
                else
                {
                    m_return = CircularCheckingLoop(m_dicMItemVersionDA[0].Tables[0].Rows[0][ItemVersionChildVM.Prop.ItemID.Name].ToString()
                        , (int)m_dicMItemVersionDA[0].Tables[0].Rows[0][ItemVersionChildVM.Prop.Version.Name]
                        , ChildItemID, ChildVersion);
                }

            }

            return m_return;
        }

        public ActionResult GetItemVersion(string ControlItemID, string ControlUoMDesc, string ControlItemDesc, string ControlItemTypeDesc, string ControlItemGroupDesc,
             string ControlVersionDesc, string ControlVersion, string ControlItemTypeID, string FilterItemID, string FilterItemDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<ItemVersionVM>> m_dicItemVersionData = GetItemVersionData(true, FilterItemID, FilterItemDesc);
                KeyValuePair<int, List<ItemVersionVM>> m_kvpItemVersionVM = m_dicItemVersionData.AsEnumerable().ToList()[0];
                if (m_kvpItemVersionVM.Key < 1 || (m_kvpItemVersionVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpItemVersionVM.Key > 1 && !Exact)
                    return Browse(ControlItemID, ControlItemDesc, "", "", "", "", ControlUoMDesc,
                        ControlItemTypeDesc, ControlItemGroupDesc, ControlVersionDesc, ControlVersion, ControlItemTypeID, FilterItemID, FilterItemDesc);

                m_dicItemVersionData = GetItemVersionData(false, FilterItemID, FilterItemDesc);
                ItemVersionVM m_objItemVersionVM = m_dicItemVersionData[0][0];
                this.GetCmp<TextField>(ControlItemID).Value = m_objItemVersionVM.ItemID;
                this.GetCmp<TextField>(ControlItemDesc).Value = m_objItemVersionVM.ItemDesc;
                this.GetCmp<TextField>(ControlItemTypeDesc).Value = m_objItemVersionVM.ItemTypeDesc;
                this.GetCmp<TextField>(ControlItemGroupDesc).Value = m_objItemVersionVM.ItemGroupDesc;
                this.GetCmp<TextField>(ControlVersion).Value = m_objItemVersionVM.Version;
                this.GetCmp<TextField>(ControlVersionDesc).Value = m_objItemVersionVM.VersionDesc;
                this.GetCmp<TextField>(ControlItemTypeID).Value = m_objItemVersionVM.ItemTypeID;
                this.GetCmp<TextField>(ControlUoMDesc).Value = m_objItemVersionVM.UoMDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private Node LoadFirstChild(string ItemID, int Version)
        {
            Node m_nodeRet = new Node();
            m_nodeRet.NodeID = "root";
            m_nodeRet.Expanded = true;

            DItemVersionChildDA m_objMItemVersionChildDA = new DItemVersionChildDA();
            m_objMItemVersionChildDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = 0;
            bool m_boolIsCount = true;

            List<ItemVersionChildVM> m_lstItemVersionChildVM = new List<ItemVersionChildVM>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ItemID);
            m_objFilter.Add(ItemVersionChildVM.Prop.ItemID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(Version);
            m_objFilter.Add(ItemVersionChildVM.Prop.Version.Map, m_lstFilter);

            m_boolIsCount = false;
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemVersionChildID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildItemTypeID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildItemID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildVersion.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.VersionDesc.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Coefficient.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Sequence.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.UoMDesc.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Formula.MapAlias);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(ItemVersionChildVM.Prop.Sequence.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicMItemVersionChildDA = m_objMItemVersionChildDA.SelectBC(m_intSkip, null, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
            if (m_objMItemVersionChildDA.Message == string.Empty)
            {
                m_lstItemVersionChildVM = (
                    from DataRow m_drMItemVersionChildDA in m_dicMItemVersionChildDA[0].Tables[0].Rows
                    select new ItemVersionChildVM()
                    {
                        ItemVersionChildID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemVersionChildID.Name].ToString(),
                        ChildItemTypeID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemTypeID.Name].ToString(),
                        ChildItemID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemID.Name].ToString(),
                        ChildVersion = (int)m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildVersion.Name],
                        VersionDesc = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.VersionDesc.Name].ToString(),
                        Coefficient = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Coefficient.Name].ToString() == String.Empty ? 0 : (decimal)m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Coefficient.Name],
                        UoMDesc = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.UoMDesc.Name].ToString(),
                        Formula = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Formula.Name].ToString(),
                        Sequence = (int)m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Sequence.Name],
                        FormulaDesc = GenerateFormulaDesc(m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Formula.Name].ToString(), ItemID, Version, "")
                    }
                ).ToList();
            }

            //foreach (ItemVersionChildVM item in m_lstItemVersionChildVM)
            //{
            //    foreach (ItemVersionChildVM item2 in m_lstItemVersionChildVM)
            //    {
            //        item.FormulaDesc = item.FormulaDesc.Replace(item2.ItemVersionChildID, item2.ChildItemID + "-" + item2.ChildVersion.ToString() + "-" + item2.Sequence.ToString());
            //    }
            //}

            NodeCollection nodes = new NodeCollection();

            foreach (ItemVersionChildVM item in m_lstItemVersionChildVM)
            {
                List<ItemVersionVM> m_lstAlternativeItem = GetAlternativeItem(item.ItemVersionChildID);
                NodeCollection ex = (NodeCollection)((StoreResult)LoadMyChild(new ItemVersionChildVM() { ChildItemID = item.ChildItemID, ChildVersion = item.ChildVersion })).Data;

                nodes.Add(new Node()
                {
                    AttributesObject = new
                    {
                        itemversionchildid = item.ItemVersionChildID,
                        childitemtypeid = item.ChildItemTypeID,
                        childitemid = item.ChildItemID,
                        versiondesc = item.VersionDesc,
                        childversion = item.ChildVersion,
                        sequence = item.Sequence,
                        sequencedesc = item.Sequence.ToString(),
                        coefficient = item.Coefficient == 0 ? "" : item.Coefficient.ToString(),
                        uomdesc = item.UoMDesc,
                        formula = item.Formula,
                        formuladesc = item.FormulaDesc,
                        leaf = ex.Count > 0 ? false : true,
                        alternative = m_lstAlternativeItem
                    },
                    Icon = ex.Count > 0 ? Icon.Folder : Icon.PageWhite
                });
            }

            m_nodeRet.Children.AddRange(nodes);

            return m_nodeRet;
        }
        private List<string> IsSaveValid(string Action, string ItemVersionID, string ItemVersionDesc)
        {
            List<string> m_lstReturn = new List<string>();

            if (ItemVersionID == string.Empty)
                m_lstReturn.Add(ItemVersionVM.Prop.ItemID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (ItemVersionDesc == string.Empty)
                m_lstReturn.Add(ItemVersionVM.Prop.VersionDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            //if (DimensionID == string.Empty)
            //    m_lstReturn.Add(ItemVersionVM.Prop.DimensionID.Desc + " " + General.EnumDesc(MessageLib.mustFill));

            return m_lstReturn;
        }
        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(ItemVersionVM.Prop.ItemID.Name, parameters[ItemVersionVM.Prop.ItemID.Name]);
            m_dicReturn.Add(ItemVersionVM.Prop.Version.Name, parameters[ItemVersionVM.Prop.Version.Name]);

            return m_dicReturn;
        }
        private ItemVersionVM GetSelectedData(Dictionary<string, object> selected, ref string message, int Version = 0)
        {
            ItemVersionVM m_objItemVersionVM = new ItemVersionVM();
            DItemVersionDA m_objMItemVersionDA = new DItemVersionDA();
            m_objMItemVersionDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemVersionVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(ItemVersionVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(ItemVersionVM.Prop.VersionDesc.MapAlias);
            m_lstSelect.Add(ItemVersionVM.Prop.Version.MapAlias);
            m_lstSelect.Add(ItemVersionVM.Prop.ItemTypeDesc.MapAlias);
            m_lstSelect.Add(ItemVersionVM.Prop.ItemGroupDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objItemVersionVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    if (Version > 0 && m_kvpSelectedRow.Key.Equals("Version"))
                    {
                        m_lstFilter.Add(Operator.Equals);
                        m_lstFilter.Add(Version);
                        m_objFilter.Add(ItemVersionVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                        continue;
                    }
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(ItemVersionVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMItemVersionDA = m_objMItemVersionDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMItemVersionDA.Message == string.Empty)
            {
                DataRow m_drMItemVersionDA = m_dicMItemVersionDA[0].Tables[0].Rows[0];
                m_objItemVersionVM.ItemID = m_drMItemVersionDA[ItemVersionVM.Prop.ItemID.Name].ToString();
                m_objItemVersionVM.ItemDesc = m_drMItemVersionDA[ItemVersionVM.Prop.ItemDesc.Name].ToString();
                m_objItemVersionVM.VersionDesc = m_drMItemVersionDA[ItemVersionVM.Prop.VersionDesc.Name].ToString();
                m_objItemVersionVM.Version = (int)m_drMItemVersionDA[ItemVersionVM.Prop.Version.Name];
                m_objItemVersionVM.ItemTypeDesc = m_drMItemVersionDA[ItemVersionVM.Prop.ItemTypeDesc.Name].ToString();
                m_objItemVersionVM.ItemGroupDesc = m_drMItemVersionDA[ItemVersionVM.Prop.ItemGroupDesc.Name].ToString();

                m_objItemVersionVM.Structure = LoadFirstChild(m_drMItemVersionDA[ItemVersionVM.Prop.ItemID.Name].ToString(), (int)m_drMItemVersionDA[ItemVersionVM.Prop.Version.Name]);
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMItemVersionDA.Message;

            return m_objItemVersionVM;
        }
        private ItemVersionChildVM GetSelectedDataStructure(Dictionary<string, object> selected, ref string message)
        {
            ItemVersionChildVM m_objItemVersionChildVM = new ItemVersionChildVM();

            m_objItemVersionChildVM.ChildItemID = selected[ItemVersionChildVM.Prop.ChildItemID.Name.ToLower()].ToString();
            m_objItemVersionChildVM.VersionDesc = selected[ItemVersionChildVM.Prop.VersionDesc.Name.ToLower()].ToString();
            m_objItemVersionChildVM.Version = int.Parse(selected[ItemVersionChildVM.Prop.ChildVersion.Name.ToLower()].ToString());
            m_objItemVersionChildVM.ItemVersionChildID = selected[ItemVersionChildVM.Prop.ItemVersionChildID.Name.ToLower()].ToString();
            m_objItemVersionChildVM.Formula = selected[ItemVersionChildVM.Prop.Formula.Name.ToLower()].ToString();
            m_objItemVersionChildVM.Coefficient = decimal.Parse(selected[ItemVersionChildVM.Prop.Coefficient.Name.ToLower()].ToString());
            m_objItemVersionChildVM.FormulaDesc = selected[ItemVersionChildVM.Prop.FormulaDesc.Name.ToLower()].ToString();
            m_objItemVersionChildVM.ChildStructure = LoadFirstChild(m_objItemVersionChildVM.ChildItemID, m_objItemVersionChildVM.Version);

            Dictionary<string, object>[] m_arrAlternativeItem = JSON.Deserialize<Dictionary<string, object>[]>(selected["alternative"].ToString());

            List<ItemVersionVM> m_lstAlternativeItem = new List<ItemVersionVM>();

            for (int i = 0; i < m_arrAlternativeItem.Length; i++)
            {

                string m_strKeyItemID = "itemid";
                if (m_arrAlternativeItem[0].ContainsKey("itemid"))
                    m_strKeyItemID = "itemid";
                else if (m_arrAlternativeItem[0].ContainsKey("ItemID"))
                    m_strKeyItemID = "ItemID";
                else if (m_arrAlternativeItem[0].ContainsKey("itemID"))
                    m_strKeyItemID = "itemID";

                string m_strKeyItemTypeID = "itemid";
                if (m_arrAlternativeItem[0].ContainsKey("itemtypeid"))
                    m_strKeyItemTypeID = "itemtypeid";
                else if (m_arrAlternativeItem[0].ContainsKey("ItemTypeID"))
                    m_strKeyItemTypeID = "ItemTypeID";
                else if (m_arrAlternativeItem[0].ContainsKey("itemTypeID"))
                    m_strKeyItemTypeID = "itemTypeID";

                string m_strKeyVersion = "version";
                if (m_arrAlternativeItem[0].ContainsKey("version"))
                    m_strKeyVersion = "version";
                else if (m_arrAlternativeItem[0].ContainsKey("Version"))
                    m_strKeyVersion = "Version";

                string m_strKeyItemDesc = "itemdesc";
                if (m_arrAlternativeItem[0].ContainsKey("itemDesc"))
                    m_strKeyItemDesc = "itemDesc";
                else if (m_arrAlternativeItem[0].ContainsKey("ItemDesc"))
                    m_strKeyItemDesc = "ItemDesc";
                else if (m_arrAlternativeItem[0].ContainsKey("itemdesc"))
                    m_strKeyItemDesc = "itemdesc";

                m_lstAlternativeItem.Add(new ItemVersionVM()
                {
                    ItemID = m_arrAlternativeItem[i][m_strKeyItemID].ToString(),
                    Version = int.Parse(m_arrAlternativeItem[i][m_strKeyVersion].ToString()),
                    ItemDesc = m_arrAlternativeItem[i][m_strKeyItemDesc].ToString(),
                    ItemTypeID = m_arrAlternativeItem[i][m_strKeyItemTypeID].ToString()
                });
            }

            m_objItemVersionChildVM.AlternativeItem = m_lstAlternativeItem;
            DItemVersionDA m_objDItemVersionDA = new DItemVersionDA();
            m_objDItemVersionDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemVersionVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(ItemVersionVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(ItemVersionVM.Prop.ItemTypeDesc.MapAlias);
            m_lstSelect.Add(ItemVersionVM.Prop.ItemGroupDesc.MapAlias);
            m_lstSelect.Add(ItemVersionVM.Prop.Version.MapAlias);
            m_lstSelect.Add(ItemVersionVM.Prop.VersionDesc.MapAlias);
            m_lstSelect.Add(ItemVersionVM.Prop.ItemTypeID.MapAlias);
            m_lstSelect.Add(ItemVersionVM.Prop.UoMDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(m_objItemVersionChildVM.ChildItemID);
            m_objFilter.Add(ItemVersionVM.Prop.ItemID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(m_objItemVersionChildVM.Version);
            m_objFilter.Add(ItemVersionVM.Prop.Version.Map, m_lstFilter);


            Dictionary<int, DataSet> m_dicMItemVersionDA = m_objDItemVersionDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null, null);
            if (m_objDItemVersionDA.Message == string.Empty)
            {
                DataRow m_drMItemVersionDA = m_dicMItemVersionDA[0].Tables[0].Rows[0];
                m_objItemVersionChildVM.ItemTypeDesc = m_drMItemVersionDA[ItemVersionVM.Prop.ItemTypeDesc.Name].ToString();
                m_objItemVersionChildVM.ItemGroupDesc = m_drMItemVersionDA[ItemVersionVM.Prop.ItemGroupDesc.Name].ToString();
                m_objItemVersionChildVM.UoMDesc = m_drMItemVersionDA[ItemVersionVM.Prop.UoMDesc.Name].ToString();
                m_objItemVersionChildVM.ItemTypeID = m_drMItemVersionDA[ItemVersionVM.Prop.ItemTypeID.Name].ToString();
                m_objItemVersionChildVM.ChildItemDesc = m_drMItemVersionDA[ItemVersionVM.Prop.ItemDesc.Name].ToString();
            }


            return m_objItemVersionChildVM;
        }
        private List<ItemVersionVM> GetAlternativeItem(string itemVersionChildID)
        {
            List<ItemVersionVM> m_lstReturn = new List<ItemVersionVM>();

            DItemVersionChildAltDA m_objItemVersionChildAltDA = new DItemVersionChildAltDA();
            m_objItemVersionChildAltDA.ConnectionStringName = Global.ConnStrConfigName;

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(itemVersionChildID);
            m_objFilter.Add(ItemVersionChildAltVM.Prop.ItemVersionChildID.Map, m_lstFilter);

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemVersionChildAltVM.Prop.ChildItemID.MapAlias);
            m_lstSelect.Add(ItemVersionChildAltVM.Prop.Version.MapAlias);
            m_lstSelect.Add(ItemVersionChildAltVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(ItemVersionChildAltVM.Prop.ItemTypeID.MapAlias);

            bool m_boolIsCount = false;

            Dictionary<int, DataSet> m_dicMItemVersionDA = m_objItemVersionChildAltDA.SelectBC(0, null, m_boolIsCount, m_lstSelect, m_objFilter, null, null, null, null);
            if (m_objItemVersionChildAltDA.Message == string.Empty)
            {
                m_lstReturn = (
                    from DataRow m_drMItemVersionChildDA in m_dicMItemVersionDA[0].Tables[0].Rows
                    select new ItemVersionVM()
                    {
                        ItemID = m_drMItemVersionChildDA[ItemVersionVM.Prop.ChildItemID.Name].ToString(),
                        Version = (int)m_drMItemVersionChildDA[ItemVersionVM.Prop.Version.Name],
                        ItemDesc = m_drMItemVersionChildDA[ItemVersionVM.Prop.ItemDesc.Name].ToString(),
                        ItemTypeID = m_drMItemVersionChildDA[ItemVersionChildAltVM.Prop.ItemTypeID.Name].ToString()
                    }
                ).ToList();
            }

            return m_lstReturn;
        }
        private List<ConfigVM> GetFilterGroupAHSReportConfig(string filterDesc3)
        {

            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilteru = new Dictionary<string, List<object>>();

            List<string> m_lstUPA = new List<string>();
            List<string> m_lstSelectb = new List<string>();
            m_lstSelectb.Add(ConfigVM.Prop.Key3.MapAlias);
            m_lstSelectb.Add(ConfigVM.Prop.Desc1.MapAlias);

            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name);
            m_objFilteru.Add(ConfigVM.Prop.Key1.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(filterDesc3);
            m_objFilteru.Add(ConfigVM.Prop.Desc3.Map, m_lstFilter);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(ConfigVM.Prop.Desc4.Map, OrderDirection.Ascending);


            Dictionary<int, DataSet> m_dicUConfigDA = m_objUConfigDA.SelectBC(0, null, false, m_lstSelectb, m_objFilteru, null, null, m_dicOrder);
            List<ConfigVM> m_lstConfigVM = new List<ConfigVM>();
            if (m_objUConfigDA.Message == string.Empty)
            {
                m_lstConfigVM = (
                        from DataRow m_drUConfigDA in m_dicUConfigDA[0].Tables[0].Rows
                        select new ConfigVM()
                        {
                            Key3 = m_drUConfigDA[ConfigVM.Prop.Key3.Name].ToString(),
                            Desc1 = m_drUConfigDA[ConfigVM.Prop.Desc1.Name].ToString()
                        }
                    ).ToList();
            }
            return m_lstConfigVM;

        }
        private List<ItemVersionChildVM> GetListItemVersionChildVM(ItemVersionVM ItemVersionVM, ref string message, ref decimal totalUnitPrice, bool firstLvl = true, int rowNo = 0)
        {
            List<ItemVersionChildVM> m_lstReturnItemVersionChildVM = new List<ItemVersionChildVM>();
            List<ItemVersionChildVM> m_lstGroupItemVersionChildVM = new List<ItemVersionChildVM>();
            List<ItemVersionChildVM> m_lstItemVersionChildVM = new List<ItemVersionChildVM>();
            DItemVersionChildDA m_objDItemVersionChildDA = new DItemVersionChildDA();
            m_objDItemVersionChildDA.ConnectionStringName = Global.ConnStrConfigName;


            var m_objListConfigVM = GetFilterGroupAHSReportConfig("GroupAHSReport");
            foreach (var item in m_objListConfigVM)
            {
                m_lstGroupItemVersionChildVM.Add(new ItemVersionChildVM { ChildItemDesc = item.Key3, ChildItemTypeID = item.Desc1 });
            }

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemVersionChildID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Version.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildItemID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildItemDesc.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildVersion.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Coefficient.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildItemTypeID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.UoMID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.UoMDesc.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Formula.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Key3.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc1.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ItemVersionVM.ItemID);
            m_objFilter.Add(ItemVersionChildVM.Prop.ItemID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(ItemVersionVM.Version);
            m_objFilter.Add(ItemVersionChildVM.Prop.Version.Map, m_lstFilter);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(ItemVersionChildVM.Prop.ChildItemTypeID.Map, OrderDirection.Ascending);


            Dictionary<int, DataSet> m_dicMItemVersionChildDA = m_objDItemVersionChildDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);
            if (m_objDItemVersionChildDA.Message == string.Empty)
            {
                foreach (DataRow m_drMItemVersionChildDA in m_dicMItemVersionChildDA[0].Tables[0].Rows)
                {

                    ItemVersionChildVM newItem = new ItemVersionChildVM();
                    newItem.ItemVersionChildID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemVersionChildID.Name].ToString();
                    newItem.ItemID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemID.Name].ToString();
                    newItem.Version = int.Parse(m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Version.Name].ToString());
                    newItem.ChildItemID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemID.Name].ToString();
                    newItem.ChildItemDesc = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemDesc.Name].ToString();
                    newItem.ChildVersion = int.Parse(m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildVersion.Name].ToString());
                    newItem.Coefficient = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Coefficient.Name].ToString() == string.Empty ? 0 : decimal.Parse(m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Coefficient.Name].ToString());
                    newItem.ChildItemTypeID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemTypeID.Name].ToString();
                    newItem.UoMID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.UoMID.Name].ToString();
                    newItem.UoMDesc = m_drMItemVersionChildDA[ItemVersionVM.Prop.UoMDesc.Name].ToString();
                    newItem.IsAHS = m_drMItemVersionChildDA[ConfigVM.Prop.Key3.Name].ToString() == m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemTypeID.Name].ToString() &&
                            m_drMItemVersionChildDA[ConfigVM.Prop.Desc1.Name].ToString() == "FALSE";
                    newItem.FirstLevel = firstLvl;
                    newItem.Formula = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Formula.Name].ToString();
                    newItem.MaterialAmount = GetUnitPrice(m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemVersionChildID.Name].ToString(), m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemID.Name].ToString(),
                                                     new ItemPriceVM { RegionID = ItemVersionVM.RegionID, ProjectID = ItemVersionVM.ProjectID, UnitTypeID = ItemVersionVM.UnitTypeID, ClusterID = ItemVersionVM.ClusterID },
                                                    m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemTypeID.Name].ToString(), BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name, newItem.Formula);
                    newItem.WageAmount = GetUnitPrice(m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemVersionChildID.Name].ToString(), m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemID.Name].ToString(),
                                                     new ItemPriceVM { RegionID = ItemVersionVM.RegionID, ProjectID = ItemVersionVM.ProjectID, UnitTypeID = ItemVersionVM.UnitTypeID, ClusterID = ItemVersionVM.ClusterID },
                                                    m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemTypeID.Name].ToString(), BudgetPlanVersionStructureVM.Prop.WageAmount.Name, newItem.Formula);
                    newItem.MiscAmount = GetUnitPrice(m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemVersionChildID.Name].ToString(), m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemID.Name].ToString(),
                                                     new ItemPriceVM { RegionID = ItemVersionVM.RegionID, ProjectID = ItemVersionVM.ProjectID, UnitTypeID = ItemVersionVM.UnitTypeID, ClusterID = ItemVersionVM.ClusterID },
                                                    m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemTypeID.Name].ToString(), BudgetPlanVersionStructureVM.Prop.MiscAmount.Name, newItem.Formula);

                    m_lstItemVersionChildVM.Add(newItem);

                }
            }

            List<ItemVersionChildVM> m_lstNewItemVersionChildVM = new List<ItemVersionChildVM>();
            List<ItemVersionChildVM> m_lstNewItemVersionChildAHSVM = new List<ItemVersionChildVM>();
            foreach (ItemVersionChildVM objChild in m_lstItemVersionChildVM)
            {
                if (objChild.IsAHS)
                {
                    List<ItemVersionChildVM> m_lstChildItemVerison = GetListItemVersionChildVM(new ItemVersionVM
                    {
                        ItemID = objChild.ChildItemID,
                        Version = objChild.ChildVersion,
                        RegionID = ItemVersionVM.RegionID,
                        ProjectID = ItemVersionVM.ProjectID,
                        ClusterID = ItemVersionVM.ClusterID,
                        UnitTypeID = ItemVersionVM.UnitTypeID
                    }, ref message, ref totalUnitPrice, false, rowNo);

                    foreach (var item in m_lstChildItemVerison)
                    {
                        if (string.IsNullOrEmpty(item.ChildItemID))
                        {
                            if(!m_lstChildItemVerison.Any(d=>d.ChildItemID == item.ItemID))
                                totalUnitPrice += item.TotalUnitPrice.Value;
                        }
                    }

                    m_lstNewItemVersionChildAHSVM.Add(new ItemVersionChildVM
                    {
                        ChildItemID = objChild.ChildItemID,
                        ChildItemTypeID = objChild.ChildItemTypeID,
                        UnitPrice = totalUnitPrice,
                        TotalUnitPrice = totalUnitPrice * objChild.Coefficient
                    });
                    totalUnitPrice = 0;

                    m_lstNewItemVersionChildVM.AddRange(m_lstChildItemVerison);


                }
            }
            m_lstReturnItemVersionChildVM.AddRange(m_lstNewItemVersionChildVM);


            foreach (ItemVersionChildVM m_groupItemVersionChild in m_lstGroupItemVersionChildVM)
            {
                decimal m_decTotUnitPrice = 0;
                if (m_lstItemVersionChildVM.Any(d => d.ChildItemTypeID == m_groupItemVersionChild.ChildItemTypeID))
                {
                    m_groupItemVersionChild.RowNo = rowNo;
                    foreach (ItemVersionChildVM m_filterItemVersionChild in m_lstItemVersionChildVM.Where(d => d.ChildItemTypeID == m_groupItemVersionChild.ChildItemTypeID).ToList())
                    {
                        m_filterItemVersionChild.FirstLevel = firstLvl;
                        m_filterItemVersionChild.RowNo = ++rowNo;
                        m_filterItemVersionChild.UnitPrice = m_filterItemVersionChild.MaterialAmount ?? 0 + m_filterItemVersionChild.WageAmount ?? 0 + m_filterItemVersionChild.MiscAmount ?? 0;
                        m_filterItemVersionChild.TotalUnitPrice = m_filterItemVersionChild.Coefficient * m_filterItemVersionChild.UnitPrice;
                        m_lstReturnItemVersionChildVM.Add(m_filterItemVersionChild);


                        m_groupItemVersionChild.ItemID = m_filterItemVersionChild.ItemID;
                        if (m_filterItemVersionChild.IsAHS)
                        {
                            m_filterItemVersionChild.TotalUnitPrice = m_lstNewItemVersionChildAHSVM.Where(d => d.ChildItemID == m_filterItemVersionChild.ChildItemID).Sum(d => d.TotalUnitPrice);

                        }
                        m_decTotUnitPrice += m_filterItemVersionChild.TotalUnitPrice.Value;
                    }
                    m_groupItemVersionChild.UnitPrice = 0;
                    m_groupItemVersionChild.MaterialAmount = 0;
                    m_groupItemVersionChild.WageAmount = 0;
                    m_groupItemVersionChild.MiscAmount = 0;
                    m_groupItemVersionChild.UnitPrice = m_decTotUnitPrice;
                    m_groupItemVersionChild.TotalUnitPrice = m_decTotUnitPrice * (m_groupItemVersionChild.Coefficient == 0 ? 1 : m_groupItemVersionChild.Coefficient);
                    m_groupItemVersionChild.FirstLevel = firstLvl;
                    m_lstReturnItemVersionChildVM.Add(m_groupItemVersionChild);

                    rowNo++;
                }
            }



            return m_lstReturnItemVersionChildVM;
        }
        private decimal GetItemPriceAmount(string ItemID, string ProjectID, string ClusterID, string UnitTypeID, string VendorID = "")
        {
            decimal m_decAmount = 0;

            if (ItemID == String.Empty || ProjectID == String.Empty)
            {
                return m_decAmount;
            }

            string RegionID = GetRegionID(ProjectID);

            List<ItemPriceVendorPeriodVM> m_lstItemPriceVendorPeriodVM = new List<ItemPriceVendorPeriodVM>();

            DItemPriceVendorPeriodDA m_objDItemPriceVendorPeriodDA = new DItemPriceVendorPeriodDA();
            m_objDItemPriceVendorPeriodDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.ItemPriceID.MapAlias);
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.RegionID.MapAlias);
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.ProjectID.MapAlias);
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.ClusterID.MapAlias);
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.UnitTypeID.MapAlias);
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.Amount.MapAlias);
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.IsDefault.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ItemID);
            m_objFilter.Add(ItemPriceVendorPeriodVM.Prop.ItemID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.None);
            m_objFilter.Add("'" + DateTime.Now.ToString(Global.SqlDateFormat) + "' BETWEEN " + ItemPriceVendorPeriodVM.Prop.ValidFrom.Map
                + " AND " + ItemPriceVendorPeriodVM.Prop.ValidTo.Map, m_lstFilter);

            //m_lstFilter = new List<object>();
            //m_lstFilter.Add(Operator.GreaterThanEqual);
            //m_lstFilter.Add(DateTime.Now.Date.ToString(Global.SqlDateFormat));
            //m_objFilter.Add(ItemPriceVendorPeriodVM.Prop.ValidTo.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicDItemPriceVendorPeriodDA = m_objDItemPriceVendorPeriodDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDItemPriceVendorPeriodDA.Message == string.Empty && m_objDItemPriceVendorPeriodDA.Success)
            {
                m_lstItemPriceVendorPeriodVM = (
                from DataRow m_drDItemPriceVendorPeriodDA in m_dicDItemPriceVendorPeriodDA[0].Tables[0].Rows
                select new ItemPriceVendorPeriodVM()
                {
                    ItemPriceID = m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.ItemPriceID.Name].ToString(),
                    ItemID = m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.ItemID.Name].ToString(),
                    RegionID = m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.RegionID.Name].ToString(),
                    ProjectID = m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.ProjectID.Name].ToString(),
                    VendorID = m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.VendorID.Name].ToString(),
                    UnitTypeID = m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.UnitTypeID.Name].ToString(),
                    Amount = decimal.Parse(m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.Amount.Name].ToString()),
                    IsDefault = bool.Parse(m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.IsDefault.Name].ToString())
                }).ToList();

                if (VendorID.Length > 0)
                {
                    m_lstItemPriceVendorPeriodVM = m_lstItemPriceVendorPeriodVM.Where(m => m.VendorID == VendorID).ToList();
                }
                else
                {
                    m_lstItemPriceVendorPeriodVM = m_lstItemPriceVendorPeriodVM.Where(m => m.IsDefault == true).ToList();
                }

                ItemPriceVendorPeriodVM m_objItemPriceVendorPeriodVM = new ItemPriceVendorPeriodVM();

                if (ClusterID != String.Empty && UnitTypeID != String.Empty)
                {
                    m_objItemPriceVendorPeriodVM = m_lstItemPriceVendorPeriodVM.FirstOrDefault(m => m.RegionID == RegionID && m.ProjectID == ProjectID && m.ClusterID == ClusterID && m.UnitTypeID == UnitTypeID);
                    if (m_objItemPriceVendorPeriodVM == null)
                    {
                        m_objItemPriceVendorPeriodVM = m_lstItemPriceVendorPeriodVM.FirstOrDefault(m => m.RegionID == RegionID && m.ProjectID == ProjectID && m.ClusterID == ClusterID);
                        if (m_objItemPriceVendorPeriodVM == null)
                        {
                            m_objItemPriceVendorPeriodVM = m_lstItemPriceVendorPeriodVM.FirstOrDefault(m => m.RegionID == RegionID && m.ProjectID == ProjectID);
                            if (m_objItemPriceVendorPeriodVM == null)
                            {
                                m_objItemPriceVendorPeriodVM = m_lstItemPriceVendorPeriodVM.FirstOrDefault(m => m.RegionID == RegionID);
                                if (m_objItemPriceVendorPeriodVM == null)
                                {
                                    m_objItemPriceVendorPeriodVM = new ItemPriceVendorPeriodVM() { Amount = 0 };
                                }
                            }
                        }
                    }
                }
                else if (ClusterID != String.Empty && UnitTypeID == String.Empty)
                {
                    m_objItemPriceVendorPeriodVM = m_lstItemPriceVendorPeriodVM.FirstOrDefault(m => m.RegionID == RegionID && m.ProjectID == ProjectID && m.ClusterID == ClusterID);
                    if (m_objItemPriceVendorPeriodVM == null)
                    {
                        m_objItemPriceVendorPeriodVM = m_lstItemPriceVendorPeriodVM.FirstOrDefault(m => m.RegionID == RegionID && m.ProjectID == ProjectID);
                        if (m_objItemPriceVendorPeriodVM == null)
                        {
                            m_objItemPriceVendorPeriodVM = m_lstItemPriceVendorPeriodVM.FirstOrDefault(m => m.RegionID == RegionID);
                            if (m_objItemPriceVendorPeriodVM == null)
                            {
                                m_objItemPriceVendorPeriodVM = new ItemPriceVendorPeriodVM() { Amount = 0 };
                            }
                        }
                    }
                }
                else if (ClusterID == String.Empty && UnitTypeID != String.Empty)
                {
                    m_objItemPriceVendorPeriodVM = m_lstItemPriceVendorPeriodVM.FirstOrDefault(m => m.RegionID == RegionID && m.ProjectID == ProjectID && m.UnitTypeID == UnitTypeID);
                    if (m_objItemPriceVendorPeriodVM == null)
                    {
                        m_objItemPriceVendorPeriodVM = m_lstItemPriceVendorPeriodVM.FirstOrDefault(m => m.RegionID == RegionID && m.ProjectID == ProjectID);
                        if (m_objItemPriceVendorPeriodVM == null)
                        {
                            m_objItemPriceVendorPeriodVM = m_lstItemPriceVendorPeriodVM.FirstOrDefault(m => m.RegionID == RegionID);
                            if (m_objItemPriceVendorPeriodVM == null)
                            {
                                m_objItemPriceVendorPeriodVM = new ItemPriceVendorPeriodVM() { Amount = 0 };
                            }
                        }
                    }
                }
                else
                {
                    m_objItemPriceVendorPeriodVM = m_lstItemPriceVendorPeriodVM.FirstOrDefault(m => m.RegionID == RegionID && m.ProjectID == ProjectID);
                    if (m_objItemPriceVendorPeriodVM == null)
                    {
                        m_objItemPriceVendorPeriodVM = m_lstItemPriceVendorPeriodVM.FirstOrDefault(m => m.RegionID == RegionID);
                        if (m_objItemPriceVendorPeriodVM == null)
                        {
                            m_objItemPriceVendorPeriodVM = new ItemPriceVendorPeriodVM() { Amount = 0 };
                        }
                    }
                }

                #region OLD
                //if (ClusterID == String.Empty && UnitTypeID == String.Empty)
                //{
                //    m_objItemPriceVendorPeriodVM = m_lstItemPriceVendorPeriodVM.FirstOrDefault(m => m.RegionID == RegionID && m.ProjectID == ProjectID);
                //    if (m_objItemPriceVendorPeriodVM == null)
                //    {
                //        m_objItemPriceVendorPeriodVM = m_lstItemPriceVendorPeriodVM.FirstOrDefault(m => m.RegionID == RegionID);
                //    }
                //}
                //else if (ClusterID != String.Empty && UnitTypeID == String.Empty)
                //{
                //    m_objItemPriceVendorPeriodVM = m_lstItemPriceVendorPeriodVM.FirstOrDefault(m => m.RegionID == RegionID && m.ProjectID == ProjectID && m.ClusterID == ClusterID);
                //}
                //else if (ClusterID == String.Empty && UnitTypeID != String.Empty)
                //{
                //    m_objItemPriceVendorPeriodVM = m_lstItemPriceVendorPeriodVM.FirstOrDefault(m => m.RegionID == RegionID && m.ProjectID == ProjectID && m.UnitTypeID == UnitTypeID);
                //}
                //else
                //{
                //    m_objItemPriceVendorPeriodVM = m_lstItemPriceVendorPeriodVM.FirstOrDefault(m => m.RegionID == RegionID && m.ProjectID == ProjectID && m.ClusterID == ClusterID && m.UnitTypeID == UnitTypeID);
                //}
                //if (m_objItemPriceVendorPeriodVM == null)
                //{
                //    m_decAmount = 0;
                //}
                //else
                //{
                //    m_decAmount = m_objItemPriceVendorPeriodVM.Amount;
                //} 
                #endregion

                if (m_objItemPriceVendorPeriodVM == null)
                {
                    m_decAmount = 0;
                }
                else
                {
                    m_decAmount = m_objItemPriceVendorPeriodVM.Amount;
                }
            }

            return m_decAmount;
        }
        #endregion

        #region Public Method

        public Dictionary<int, List<ItemVersionVM>> GetItemVersionData(bool isCount, string ItemID, string ItemDesc)
        {
            int m_intCount = 0;
            List<ItemVersionVM> m_lstItemVersionVM = new List<ViewModels.ItemVersionVM>();
            Dictionary<int, List<ItemVersionVM>> m_dicReturn = new Dictionary<int, List<ItemVersionVM>>();
            DItemVersionDA m_objMItemVersionDA = new DItemVersionDA();
            m_objMItemVersionDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemVersionVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(ItemVersionVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(ItemVersionVM.Prop.Version.MapAlias);
            m_lstSelect.Add(ItemVersionVM.Prop.ItemGroupDesc.MapAlias);
            m_lstSelect.Add(ItemVersionVM.Prop.ItemTypeDesc.MapAlias);
            m_lstSelect.Add(ItemVersionVM.Prop.VersionDesc.MapAlias);
            m_lstSelect.Add(ItemVersionVM.Prop.ItemTypeID.MapAlias);
            m_lstSelect.Add(ItemVersionVM.Prop.UoMDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(ItemID);
            m_objFilter.Add(ItemVersionVM.Prop.ItemID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(ItemDesc);
            m_objFilter.Add(ItemVersionVM.Prop.ItemDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMItemVersionDA = m_objMItemVersionDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMItemVersionDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpItemVersionBL in m_dicMItemVersionDA)
                    {
                        m_intCount = m_kvpItemVersionBL.Key;
                        break;
                    }
                else
                {
                    m_lstItemVersionVM = (
                        from DataRow m_drMItemVersionDA in m_dicMItemVersionDA[0].Tables[0].Rows
                        select new ItemVersionVM()
                        {
                            ItemID = m_drMItemVersionDA[ItemVersionVM.Prop.ItemID.Name].ToString(),
                            VersionDesc = m_drMItemVersionDA[ItemVersionVM.Prop.VersionDesc.Name].ToString(),
                            ItemDesc = m_drMItemVersionDA[ItemVersionVM.Prop.ItemDesc.Name].ToString(),
                            ItemGroupDesc = m_drMItemVersionDA[ItemVersionVM.Prop.ItemGroupDesc.Name].ToString(),
                            ItemTypeDesc = m_drMItemVersionDA[ItemVersionVM.Prop.ItemTypeDesc.Name].ToString(),
                            Version = (int)m_drMItemVersionDA[ItemVersionVM.Prop.Version.Name],
                            ItemTypeID = m_drMItemVersionDA[ItemVersionVM.Prop.ItemTypeID.Name].ToString(),
                            UoMDesc = m_drMItemVersionDA[ItemVersionVM.Prop.UoMDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstItemVersionVM);
            return m_dicReturn;
        }

        #endregion
    }
}