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
using System.Web;

namespace com.SML.BIGTRONS.Controllers
{
    public class BudgetPlanTemplateController : BaseController
    {
        private readonly string title = "BudgetPlanTemplate";
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
            MBudgetPlanTemplateDA m_objMBudgetPlanTemplateDA = new MBudgetPlanTemplateDA();
            m_objMBudgetPlanTemplateDA.ConnectionStringName = Global.ConnStrConfigName;
            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMBudgetPlanTemplate = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMBudgetPlanTemplate.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = BudgetPlanTemplateVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMBudgetPlanTemplateDA = m_objMBudgetPlanTemplateDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpBudgetPlanTemplateBL in m_dicMBudgetPlanTemplateDA)
            {
                m_intCount = m_kvpBudgetPlanTemplateBL.Key;
                break;
            }

            List<BudgetPlanTemplateVM> m_lstBudgetPlanTemplateVM = new List<BudgetPlanTemplateVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(BudgetPlanTemplateVM.Prop.BudgetPlanTemplateID.MapAlias);
                m_lstSelect.Add(BudgetPlanTemplateVM.Prop.BudgetPlanTemplateDesc.MapAlias);
                m_lstSelect.Add(BudgetPlanTemplateVM.Prop.BudgetPlanTypeDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(BudgetPlanTemplateVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMBudgetPlanTemplateDA = m_objMBudgetPlanTemplateDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMBudgetPlanTemplateDA.Message == string.Empty)
                {
                    m_lstBudgetPlanTemplateVM = (
                        from DataRow m_drMBudgetPlanTemplateDA in m_dicMBudgetPlanTemplateDA[0].Tables[0].Rows
                        select new BudgetPlanTemplateVM()
                        {
                            BudgetPlanTemplateID = m_drMBudgetPlanTemplateDA[BudgetPlanTemplateVM.Prop.BudgetPlanTemplateID.Name].ToString(),
                            BudgetPlanTemplateDesc = m_drMBudgetPlanTemplateDA[BudgetPlanTemplateVM.Prop.BudgetPlanTemplateDesc.Name].ToString(),
                            BudgetPlanTypeDesc = m_drMBudgetPlanTemplateDA[BudgetPlanTemplateVM.Prop.BudgetPlanTypeDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstBudgetPlanTemplateVM, m_intCount);
        }
        public ActionResult ReadBudgetTemplateStructure(StoreRequestParameters parameters, string Selected)
        {
            DBudgetPlanTemplateStructureDA m_objDBudgetPlanTemplateStructureDA = new DBudgetPlanTemplateStructureDA();
            m_objDBudgetPlanTemplateStructureDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMBudgetPlanTemplate = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMBudgetPlanTemplate.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = BudgetPlanTemplateStructureVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicDBudgetPlanTemplateStructureDA = m_objDBudgetPlanTemplateStructureDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpBudgetPlanTemplateStructureBL in m_dicDBudgetPlanTemplateStructureDA)
            {
                m_intCount = m_kvpBudgetPlanTemplateStructureBL.Key;
                break;
            }

            List<BudgetPlanTemplateStructureVM> m_lstBudgetPlanTemplateStructureVM = new List<BudgetPlanTemplateStructureVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.MapAlias);
                m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemID.MapAlias);
                m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.Version.MapAlias);
                m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.Sequence.MapAlias);
                m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemID.MapAlias);
                m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentVersion.MapAlias);
                m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentSequence.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(BudgetPlanTemplateStructureVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicDBudgetPlanTemplateStructureDA = m_objDBudgetPlanTemplateStructureDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objDBudgetPlanTemplateStructureDA.Message == string.Empty)
                {
                    m_lstBudgetPlanTemplateStructureVM = (
                        from DataRow m_drDBudgetPlanTemplateStructureDA in m_dicDBudgetPlanTemplateStructureDA[0].Tables[0].Rows
                        select new BudgetPlanTemplateStructureVM()
                        {
                            BudgetPlanTemplateID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.Name].ToString(),
                            ItemID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemID.Name].ToString(),
                            Version = Convert.ToInt32(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.Version.Name].ToString()),
                            Sequence = Convert.ToInt32(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.Sequence.Name].ToString()),
                            ParentItemID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentItemID.Name].ToString(),
                            ParentVersion = Convert.ToInt32(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentVersion.Name].ToString()),
                            ParentSequence = Convert.ToInt32(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentSequence.Name].ToString())
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstBudgetPlanTemplateStructureVM, m_intCount);
        }
        public ActionResult ReadBrowseItemVersion(StoreRequestParameters parameters)
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
            Dictionary<int, DataSet> m_dicMItemVersionDA = m_objMItemVersionDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
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
                //m_lstSelect.Add(ItemVersionVM.Prop.ItemVersionDesc.MapAlias);
                //m_lstSelect.Add(ItemVersionVM.Prop.DimensionDesc.MapAlias);

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
                            ItemID = m_drMItemVersionDA[ItemVersionVM.Prop.ItemID.Name].ToString()
                            //ItemVersionDesc = m_drMItemVersionDA[ItemVersionVM.Prop.ItemVersionDesc.Name].ToString(),
                            //DimensionDesc = m_drMItemVersionDA[ItemVersionVM.Prop.DimensionDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstItemVersionVM, m_intCount);
        }
        public ActionResult ReadBrowse (StoreRequestParameters parameters,string FromPage = "")
        {
            MBudgetPlanTemplateDA m_objMBudgetPlanTemplateDA = new MBudgetPlanTemplateDA();
            m_objMBudgetPlanTemplateDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMBudgetPlanTemplate = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMBudgetPlanTemplate.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = BudgetPlanTemplateVM.Prop.Map(m_strDataIndex, false);
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


            //add filter, if from budget plan, check is logged in user has template role
            if(FromPage != String.Empty)
            {
                List<object> m_lstFilter = new List<object>();
               
                switch (FromPage.ToUpper())
                {
                    case "BUDGETPLAN" :
                        m_lstFilter.Add(Operator.None);
                        m_lstFilter.Add(String.Empty);
                        m_objFilter.Add($" ISNULL([MBudgetPlanTemplate].BudgetPlanTemplateID,'') IN (SELECT DUserBudgetPlanAccess.BudgetPlanTemplateID FROM DUserBudgetPlanAccess WHERE GETDATE() BETWEEN [DUserBudgetPlanAccess].StartDate AND [DUserBudgetPlanAccess].EndDate AND DUserBudgetPlanAccess.UserID = '{Global.LoggedInUser}' ) ", m_lstFilter);
                        break;
                    default:
                        break;
                }
               
                
            }



            Dictionary<int, DataSet> m_dicMBudgetPlanTemplateDA = m_objMBudgetPlanTemplateDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpBudgetPlanTemplateBL in m_dicMBudgetPlanTemplateDA)
            {
                m_intCount = m_kvpBudgetPlanTemplateBL.Key;
                break;
            }

            List<BudgetPlanTemplateVM> m_lstBudgetPlanTemplateVM = new List<BudgetPlanTemplateVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(BudgetPlanTemplateVM.Prop.BudgetPlanTemplateID.MapAlias);
                m_lstSelect.Add(BudgetPlanTemplateVM.Prop.BudgetPlanTemplateDesc.MapAlias);
                m_lstSelect.Add(BudgetPlanTemplateVM.Prop.BudgetPlanTypeDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(BudgetPlanTemplateVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMBudgetPlanTemplateDA = m_objMBudgetPlanTemplateDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMBudgetPlanTemplateDA.Message == string.Empty)
                {
                    m_lstBudgetPlanTemplateVM = (
                        from DataRow m_drMBudgetPlanTemplateDA in m_dicMBudgetPlanTemplateDA[0].Tables[0].Rows
                        select new BudgetPlanTemplateVM()
                        {
                            BudgetPlanTemplateID = m_drMBudgetPlanTemplateDA[BudgetPlanTemplateVM.Prop.BudgetPlanTemplateID.Name].ToString(),
                            BudgetPlanTemplateDesc = m_drMBudgetPlanTemplateDA[BudgetPlanTemplateVM.Prop.BudgetPlanTemplateDesc.Name].ToString(),
                            BudgetPlanTypeDesc = m_drMBudgetPlanTemplateDA[BudgetPlanTemplateVM.Prop.BudgetPlanTypeDesc.Name].ToString(),
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstBudgetPlanTemplateVM, m_intCount);
        }
        public ActionResult ReadBrowseStructure(StoreRequestParameters parameters, string BudgetPlanTemplateID, string ParentItemID, string ParentVersion, string Caller)
        {
            DBudgetPlanTemplateStructureDA m_objDBudgetPlanTemplateStructureDA = new DBudgetPlanTemplateStructureDA();
            m_objDBudgetPlanTemplateStructureDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMBudgetPlanTemplate = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMBudgetPlanTemplate.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = BudgetPlanTemplateStructureVM.Prop.Map(m_strDataIndex, false);
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

            if (!string.IsNullOrEmpty(ParentItemID))
            {
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(ParentItemID);
                m_objFilter.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemID.Map, m_lstFilter);
            }

            if (!string.IsNullOrEmpty(ParentVersion))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(ParentVersion);
                m_objFilter.Add(BudgetPlanTemplateStructureVM.Prop.ParentVersion.Map, m_lstFilter);
            }


            if (!string.IsNullOrEmpty(BudgetPlanTemplateID))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(BudgetPlanTemplateID);
                m_objFilter.Add(BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.Map, m_lstFilter);
            }

            string m_strIsDefault = "({0} IS NULL OR {0} = 1)"; //!=False
            if (Caller == General.EnumDesc(Buttons.ButtonUpdate))
            {
                m_strIsDefault = "{0} IS NOT NULL"; //!=NULL
            }
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.None);
            m_lstFilter.Add(string.Empty);
            m_objFilter.Add(string.Format(m_strIsDefault, BudgetPlanTemplateStructureVM.Prop.IsDefault.Map), m_lstFilter);

            Dictionary<int, DataSet> m_dicDBudgetPlanTemplateStructureDA = m_objDBudgetPlanTemplateStructureDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpBudgetPlanTemplateStructureBL in m_dicDBudgetPlanTemplateStructureDA)
            {
                m_intCount = m_kvpBudgetPlanTemplateStructureBL.Key;
                break;
            }

            List<BudgetPlanTemplateStructureVM> m_lstBudgetPlanTemplateStructureVM = new List<BudgetPlanTemplateStructureVM>();
            if (m_intCount > 0)
            {
                if (Caller == General.EnumDesc(Buttons.ButtonAdd))
                {
                    m_boolIsCount = false;
                    List<string> m_lstSelect = new List<string>();
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemID.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemDesc.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.Version.MapAlias);
                    //m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.Sequence.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemTypeID.MapAlias);
                    //m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemID.MapAlias);
                    //m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemTypeID.MapAlias);
                    //m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentVersion.MapAlias);
                    //m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentSequence.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemVersionChildID.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.IsDefault.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.UoMDesc.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.UoMID.MapAlias);
                    m_lstSelect.Add(ConfigVM.Prop.Key3.MapAlias);
                    m_lstSelect.Add(ConfigVM.Prop.Desc1.MapAlias);

                    //m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.HasChild.MapAlias);

                    Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                    foreach (DataSorter m_dtsOrder in parameters.Sort)
                        m_dicOrder.Add(BudgetPlanTemplateStructureVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                    List<string> m_lstGroup = new List<string>();
                    m_lstGroup.Add(BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.Map);
                    m_lstGroup.Add(BudgetPlanTemplateStructureVM.Prop.ItemID.Map);
                    m_lstGroup.Add(BudgetPlanTemplateStructureVM.Prop.ItemDesc.Map);
                    m_lstGroup.Add(BudgetPlanTemplateStructureVM.Prop.Version.Map);
                    m_lstGroup.Add(BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Map);
                    m_lstGroup.Add(BudgetPlanTemplateStructureVM.Prop.ItemVersionChildID.Map);
                    m_lstGroup.Add(BudgetPlanTemplateStructureVM.Prop.IsDefault.Map);
                    m_lstGroup.Add(BudgetPlanTemplateStructureVM.Prop.UoMDesc.Map);
                    m_lstGroup.Add(BudgetPlanTemplateStructureVM.Prop.UoMID.Map);
                    m_lstGroup.Add(ConfigVM.Prop.Key3.Map);
                    m_lstGroup.Add(ConfigVM.Prop.Desc1.Map);

                    m_dicDBudgetPlanTemplateStructureDA = m_objDBudgetPlanTemplateStructureDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, m_lstGroup, m_dicOrder, null);
                    if (m_objDBudgetPlanTemplateStructureDA.Message == string.Empty)
                    {
                        m_lstBudgetPlanTemplateStructureVM = (
                            from DataRow m_drDBudgetPlanTemplateStructureDA in m_dicDBudgetPlanTemplateStructureDA[0].Tables[0].Rows
                            select new BudgetPlanTemplateStructureVM()
                            {
                                BudgetPlanTemplateID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.Name].ToString(),
                                ItemID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemID.Name].ToString(),
                                ItemDesc = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemDesc.Name].ToString(),
                                UoMDesc = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.UoMDesc.Name].ToString(),
                                UoMID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.UoMID.Name].ToString(),
                                Version = Convert.ToInt32(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.Version.Name].ToString()),
                                Sequence = 0,//Convert.ToInt32(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.Sequence.Name].ToString()),
                                ItemTypeID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString(),
                                //ParentItemID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentItemID.Name].ToString(),
                                ParentItemTypeID = "",// m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentItemTypeID.Name].ToString(),
                                ParentVersion = 0,// Convert.ToInt32(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentVersion.Name].ToString()),
                                ParentSequence = 0,// Convert.ToInt32(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentSequence.Name].ToString()),
                                IsDefault = (string.IsNullOrEmpty(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.IsDefault.Name].ToString()) ? null :
                                            (bool?)(bool.Parse(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.IsDefault.Name].ToString()))),
                                IsBOI = m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Key3.Name].ToString() == m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString()
                                        && m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Desc1.Name].ToString() == "TRUE",
                                IsAHS = m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Key3.Name].ToString() == m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString()
                                        && m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Desc1.Name].ToString() == "FALSE",
                                HasChild = m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Key3.Name].ToString() == m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString()
                                        && m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Desc1.Name].ToString() == "TRUE"

                            }
                        ).ToList();
                    }
                }
                else
                {
                    m_boolIsCount = false;
                    List<string> m_lstSelect = new List<string>();
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemID.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemDesc.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.Version.MapAlias);
                    //m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.Sequence.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemTypeID.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemID.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemTypeID.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentVersion.MapAlias);
                    //m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentSequence.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemVersionChildID.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.IsDefault.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.UoMDesc.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.UoMID.MapAlias);
                    m_lstSelect.Add(ConfigVM.Prop.Key3.MapAlias);
                    m_lstSelect.Add(ConfigVM.Prop.Desc1.MapAlias);
                    

                    Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                    foreach (DataSorter m_dtsOrder in parameters.Sort)
                        m_dicOrder.Add(BudgetPlanTemplateStructureVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                    List<string> m_lstGroup = new List<string>();
                    m_lstGroup.Add(BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.Map);
                    m_lstGroup.Add(BudgetPlanTemplateStructureVM.Prop.ItemID.Map);
                    m_lstGroup.Add(BudgetPlanTemplateStructureVM.Prop.ItemDesc.Map);
                    m_lstGroup.Add(BudgetPlanTemplateStructureVM.Prop.Version.Map);
                    //m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.Sequence.MapAlias);
                    m_lstGroup.Add(BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Map);
                    m_lstGroup.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemID.Map);
                    m_lstGroup.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemTypeID.Map);
                    m_lstGroup.Add(BudgetPlanTemplateStructureVM.Prop.ParentVersion.Map);
                    //m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentSequence.MapAlias);
                    m_lstGroup.Add(BudgetPlanTemplateStructureVM.Prop.ItemVersionChildID.Map);
                    m_lstGroup.Add(BudgetPlanTemplateStructureVM.Prop.IsDefault.Map);
                    m_lstGroup.Add(BudgetPlanTemplateStructureVM.Prop.UoMDesc.Map);
                    m_lstGroup.Add(BudgetPlanTemplateStructureVM.Prop.UoMID.Map);
                    m_lstGroup.Add(ConfigVM.Prop.Key3.Map);
                    m_lstGroup.Add(ConfigVM.Prop.Desc1.Map);

                    m_dicDBudgetPlanTemplateStructureDA = m_objDBudgetPlanTemplateStructureDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, m_lstGroup, m_dicOrder, null);
                    if (m_objDBudgetPlanTemplateStructureDA.Message == string.Empty)
                    {
                        m_lstBudgetPlanTemplateStructureVM = (
                            from DataRow m_drDBudgetPlanTemplateStructureDA in m_dicDBudgetPlanTemplateStructureDA[0].Tables[0].Rows
                            select new BudgetPlanTemplateStructureVM()
                            {
                                BudgetPlanTemplateID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.Name].ToString(),
                                ItemID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemID.Name].ToString(),
                                ItemDesc = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemDesc.Name].ToString(),
                                UoMDesc = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.UoMDesc.Name].ToString(),
                                UoMID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.UoMID.Name].ToString(),
                                Version = Convert.ToInt32(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.Version.Name].ToString()),
                                Sequence = 0,//Convert.ToInt32(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.Sequence.Name].ToString()),
                            ItemTypeID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString(),
                                ParentItemID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentItemID.Name].ToString(),
                                ParentItemTypeID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentItemTypeID.Name].ToString(),
                                ParentVersion = Convert.ToInt32(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentVersion.Name].ToString()),
                                ParentSequence = 0,// Convert.ToInt32(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentSequence.Name].ToString()),
                            IsDefault = (string.IsNullOrEmpty(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.IsDefault.Name].ToString()) ? null :
                                            (bool?)(bool.Parse(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.IsDefault.Name].ToString()))),
                            IsBOI = m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Key3.Name].ToString() == m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString()
                                        && m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Desc1.Name].ToString() == "TRUE",
                                IsAHS = m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Key3.Name].ToString() == m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString()
                                        && m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Desc1.Name].ToString() == "FALSE",
                                HasChild = m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Key3.Name].ToString() == m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString()
                                        && m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Desc1.Name].ToString() == "TRUE"

                            }
                        ).ToList();
                    }
                }
            }
            
            return this.Store(m_lstBudgetPlanTemplateStructureVM, m_intCount);
        }
        public ActionResult ReadBrowseStructureTree(StoreRequestParameters parameters, string BudgetPlanTemplateID, string ParentItemID, string ParentVersion, string Caller)
        {
            DBudgetPlanTemplateStructureDA m_objDBudgetPlanTemplateStructureDA = new DBudgetPlanTemplateStructureDA();
            m_objDBudgetPlanTemplateStructureDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = 0;// parameters.Start;
            int? m_intLength = null;// parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMBudgetPlanTemplate = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMBudgetPlanTemplate.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = BudgetPlanTemplateStructureVM.Prop.Map(m_strDataIndex, false);
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

            if (!string.IsNullOrEmpty(ParentItemID))
            {
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(ParentItemID);
                m_objFilter.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemID.Map, m_lstFilter);
            }

            if (!string.IsNullOrEmpty(ParentVersion))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(ParentVersion);
                m_objFilter.Add(BudgetPlanTemplateStructureVM.Prop.ParentVersion.Map, m_lstFilter);
            }


            if (!string.IsNullOrEmpty(BudgetPlanTemplateID))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(BudgetPlanTemplateID);
                m_objFilter.Add(BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.Map, m_lstFilter);
            }

            string m_strIsDefault = "({0} IS NULL OR {0} = 1)"; //!=False
            if (Caller == General.EnumDesc(Buttons.ButtonUpdate))
            {
                m_strIsDefault = "{0} IS NOT NULL"; //!=NULL
            }
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.None);
            m_lstFilter.Add(string.Empty);
            m_objFilter.Add(string.Format(m_strIsDefault, BudgetPlanTemplateStructureVM.Prop.IsDefault.Map), m_lstFilter);

            Dictionary<string, OrderDirection> m_objOrderBy = new Dictionary<string, OrderDirection>();
            m_objOrderBy.Add(BudgetPlanTemplateStructureVM.Prop.Sequence.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicDBudgetPlanTemplateStructureDA = m_objDBudgetPlanTemplateStructureDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, m_objOrderBy, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpBudgetPlanTemplateStructureBL in m_dicDBudgetPlanTemplateStructureDA)
            {
                m_intCount = m_kvpBudgetPlanTemplateStructureBL.Key;
                break;
            }

            List<BudgetPlanTemplateStructureVM> m_lstBudgetPlanTemplateStructureVM = new List<BudgetPlanTemplateStructureVM>();
            if (m_intCount > 0)
            {
                if (Caller == General.EnumDesc(Buttons.ButtonAdd))
                {
                    m_boolIsCount = false;
                    List<string> m_lstSelect = new List<string>();
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemID.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemDesc.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.Version.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.Sequence.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemTypeID.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemID.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemTypeID.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentVersion.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentSequence.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemVersionChildID.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.IsDefault.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.UoMDesc.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.UoMID.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemGroupDesc.MapAlias);
                    m_lstSelect.Add(ConfigVM.Prop.Key3.MapAlias);
                    m_lstSelect.Add(ConfigVM.Prop.Desc1.MapAlias);

                    //m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.HasChild.MapAlias);

                    //Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                    //foreach (DataSorter m_dtsOrder in parameters.Sort)
                    //    m_dicOrder.Add(BudgetPlanTemplateStructureVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));
                    

                    m_dicDBudgetPlanTemplateStructureDA = m_objDBudgetPlanTemplateStructureDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_objOrderBy, null);
                    if (m_objDBudgetPlanTemplateStructureDA.Message == string.Empty)
                    {
                        foreach (DataRow m_dataRow in m_dicDBudgetPlanTemplateStructureDA[0].Tables[0].Rows)
                        {
                            BudgetPlanTemplateStructureVM m_objMBudgetPlanTemplateStructureVM = new BudgetPlanTemplateStructureVM();
                            m_objMBudgetPlanTemplateStructureVM.UoMID = m_dataRow[BudgetPlanTemplateStructureVM.Prop.UoMID.Name].ToString();
                            m_objMBudgetPlanTemplateStructureVM.UoMDesc = m_dataRow[BudgetPlanTemplateStructureVM.Prop.UoMDesc.Name].ToString();
                            m_objMBudgetPlanTemplateStructureVM.ItemDesc = m_dataRow[BudgetPlanTemplateStructureVM.Prop.ItemDesc.Name].ToString();
                            m_objMBudgetPlanTemplateStructureVM.BudgetPlanTemplateID = m_dataRow[BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.Name].ToString();
                            m_objMBudgetPlanTemplateStructureVM.ItemID = m_dataRow[BudgetPlanTemplateStructureVM.Prop.ItemID.Name].ToString();
                            m_objMBudgetPlanTemplateStructureVM.Version = Convert.ToInt16(m_dataRow[BudgetPlanTemplateStructureVM.Prop.Version.Name].ToString());
                            m_objMBudgetPlanTemplateStructureVM.Sequence = Convert.ToInt16(m_dataRow[BudgetPlanTemplateStructureVM.Prop.Sequence.Name].ToString());
                            m_objMBudgetPlanTemplateStructureVM.ParentItemID = m_dataRow[BudgetPlanTemplateStructureVM.Prop.ParentItemID.Name].ToString();
                            m_objMBudgetPlanTemplateStructureVM.ParentSequence = Convert.ToInt16(m_dataRow[BudgetPlanTemplateStructureVM.Prop.ParentSequence.Name].ToString());
                            m_objMBudgetPlanTemplateStructureVM.ParentVersion = Convert.ToInt16(m_dataRow[BudgetPlanTemplateStructureVM.Prop.ParentVersion.Name].ToString());
                            m_objMBudgetPlanTemplateStructureVM.ItemGroupDesc = m_dataRow[BudgetPlanTemplateStructureVM.Prop.ItemGroupDesc.Name].ToString();
                            m_objMBudgetPlanTemplateStructureVM.ItemTypeID = m_dataRow[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString();
                            m_objMBudgetPlanTemplateStructureVM.ParentItemTypeID = m_dataRow[BudgetPlanTemplateStructureVM.Prop.ParentItemTypeID.Name].ToString();
                            m_objMBudgetPlanTemplateStructureVM.IsBOI = m_dataRow[ConfigVM.Prop.Key3.Name].ToString() == m_dataRow[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString()
                                    && m_dataRow[ConfigVM.Prop.Desc1.Name].ToString() == "TRUE";
                            m_objMBudgetPlanTemplateStructureVM.IsAHS = m_dataRow[ConfigVM.Prop.Key3.Name].ToString() == m_dataRow[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString()
                                    && m_dataRow[ConfigVM.Prop.Desc1.Name].ToString() == "FALSE";
                            m_objMBudgetPlanTemplateStructureVM.HasChild = m_objMBudgetPlanTemplateStructureVM.IsBOI;

                            if (!string.IsNullOrEmpty(m_dataRow[BudgetPlanTemplateStructureVM.Prop.IsDefault.Name].ToString()))
                            {
                                m_objMBudgetPlanTemplateStructureVM.EnableDefault = true;
                                m_objMBudgetPlanTemplateStructureVM.IsDefault = Convert.ToBoolean(m_dataRow[BudgetPlanTemplateStructureVM.Prop.IsDefault.Name].ToString());
                            }
                            else
                            {
                                m_objMBudgetPlanTemplateStructureVM.IsDefault = null;
                                m_objMBudgetPlanTemplateStructureVM.EnableDefault = false;
                            }
                            m_lstBudgetPlanTemplateStructureVM.Add(m_objMBudgetPlanTemplateStructureVM);
                       }
                    }
                }
                else
                {
                    m_boolIsCount = false;
                    List<string> m_lstSelect = new List<string>();
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemID.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemDesc.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.Version.MapAlias);
                    //m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.Sequence.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemTypeID.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemID.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemTypeID.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentVersion.MapAlias);
                    //m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentSequence.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemVersionChildID.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.IsDefault.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.UoMDesc.MapAlias);
                    m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.UoMID.MapAlias);
                    m_lstSelect.Add(ConfigVM.Prop.Key3.MapAlias);
                    m_lstSelect.Add(ConfigVM.Prop.Desc1.MapAlias);


                    Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                    foreach (DataSorter m_dtsOrder in parameters.Sort)
                        m_dicOrder.Add(BudgetPlanTemplateStructureVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                    List<string> m_lstGroup = new List<string>();
                    m_lstGroup.Add(BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.Map);
                    m_lstGroup.Add(BudgetPlanTemplateStructureVM.Prop.ItemID.Map);
                    m_lstGroup.Add(BudgetPlanTemplateStructureVM.Prop.ItemDesc.Map);
                    m_lstGroup.Add(BudgetPlanTemplateStructureVM.Prop.Version.Map);
                    //m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.Sequence.MapAlias);
                    m_lstGroup.Add(BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Map);
                    m_lstGroup.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemID.Map);
                    m_lstGroup.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemTypeID.Map);
                    m_lstGroup.Add(BudgetPlanTemplateStructureVM.Prop.ParentVersion.Map);
                    //m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentSequence.MapAlias);
                    m_lstGroup.Add(BudgetPlanTemplateStructureVM.Prop.ItemVersionChildID.Map);
                    m_lstGroup.Add(BudgetPlanTemplateStructureVM.Prop.IsDefault.Map);
                    m_lstGroup.Add(BudgetPlanTemplateStructureVM.Prop.UoMDesc.Map);
                    m_lstGroup.Add(BudgetPlanTemplateStructureVM.Prop.UoMID.Map);
                    m_lstGroup.Add(ConfigVM.Prop.Key3.Map);
                    m_lstGroup.Add(ConfigVM.Prop.Desc1.Map);

                    m_dicDBudgetPlanTemplateStructureDA = m_objDBudgetPlanTemplateStructureDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, m_lstGroup, m_dicOrder, null);
                    if (m_objDBudgetPlanTemplateStructureDA.Message == string.Empty)
                    {
                        m_lstBudgetPlanTemplateStructureVM = (
                            from DataRow m_drDBudgetPlanTemplateStructureDA in m_dicDBudgetPlanTemplateStructureDA[0].Tables[0].Rows
                            select new BudgetPlanTemplateStructureVM()
                            {
                                BudgetPlanTemplateID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.Name].ToString(),
                                ItemID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemID.Name].ToString(),
                                ItemDesc = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemDesc.Name].ToString(),
                                UoMDesc = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.UoMDesc.Name].ToString(),
                                UoMID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.UoMID.Name].ToString(),
                                Version = Convert.ToInt32(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.Version.Name].ToString()),
                                Sequence = 0,//Convert.ToInt32(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.Sequence.Name].ToString()),
                                ItemTypeID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString(),
                                ParentItemID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentItemID.Name].ToString(),
                                ParentItemTypeID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentItemTypeID.Name].ToString(),
                                ParentVersion = Convert.ToInt32(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentVersion.Name].ToString()),
                                ParentSequence = 0,// Convert.ToInt32(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentSequence.Name].ToString()),
                                IsDefault = (string.IsNullOrEmpty(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.IsDefault.Name].ToString()) ? null :
                                            (bool?)(bool.Parse(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.IsDefault.Name].ToString()))),
                                IsBOI = m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Key3.Name].ToString() == m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString()
                                        && m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Desc1.Name].ToString() == "TRUE",
                                IsAHS = m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Key3.Name].ToString() == m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString()
                                        && m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Desc1.Name].ToString() == "FALSE",
                                HasChild = m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Key3.Name].ToString() == m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString()
                                        && m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Desc1.Name].ToString() == "TRUE"

                            }
                        ).ToList();
                    }
                }
            }

            return this.Store(GetNodeTemplateStructure(m_lstBudgetPlanTemplateStructureVM), m_intCount);
        }
        public ActionResult Home()
        {
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 0;
            return this.Direct();
        }

        public ActionResult Add(string Caller, string Selected)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Add)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            BudgetPlanTemplateVM m_objBudgetPlanTemplateVM = new BudgetPlanTemplateVM();
            m_objBudgetPlanTemplateVM.ListBudgetPlanTemplateStructureVM = new List<BudgetPlanTemplateStructureVM>();
            m_objBudgetPlanTemplateVM.lstUserBudgetPlanAccessVM = GetSelectedDataUserAccess("");

            BudgetPlanTemplateStructureVM m_objMBudgetPlanTemplateStructureVM_ = new BudgetPlanTemplateStructureVM();
            m_objMBudgetPlanTemplateStructureVM_.BudgetPlanTemplateID = " ";
            m_objMBudgetPlanTemplateStructureVM_.ItemID = "empty";
            m_objBudgetPlanTemplateVM.ListBudgetPlanTemplateStructureVM.Add(m_objMBudgetPlanTemplateStructureVM_);
            m_objBudgetPlanTemplateVM.lstUser = GetSelectedDataUser();
            m_objBudgetPlanTemplateVM.lstUserBudgetPlanAccessVM = GetSelectedDataUserAccess(m_objMBudgetPlanTemplateStructureVM_.BudgetPlanTemplateID);
            ViewDataDictionary m_vddBudgetPlanTemplate = new ViewDataDictionary();
            m_vddBudgetPlanTemplate.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddBudgetPlanTemplate.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
            m_vddBudgetPlanTemplate.Add("IsCopyFromPrevious", false);
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();
            if (Caller == General.EnumDesc(Buttons.ButtonDetail))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                m_dicSelectedRow = GetFormData(m_nvcParams);
                Session[dataSessionName] = JSON.Serialize(m_dicSelectedRow);
            }
            else if (Caller == "GetData")
            {
                m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
                Session["GetData"] = m_dicSelectedRow;
                return Update(General.EnumDesc(Buttons.ButtonList), Selected);
            }
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objBudgetPlanTemplateVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddBudgetPlanTemplate,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Detail(string Caller, string Selected, string BudgetPlantemplateID)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();
            string m_strMessage = string.Empty;
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
            {
                m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
            }
            else if (Caller == General.EnumDesc(Buttons.ButtonDetail))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                m_dicSelectedRow = GetFormData(m_nvcParams);
            }
            
            BudgetPlanTemplateVM lst_objBudgetPlanTemplateVM = new BudgetPlanTemplateVM();


            if (m_dicSelectedRow.Count > 0)
            {
                lst_objBudgetPlanTemplateVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage, null);
                lst_objBudgetPlanTemplateVM.lstUserBudgetPlanAccessVM = GetSelectedDataUserAccess(m_dicSelectedRow[UserBudgetPlanAccessVM.Prop.BudgetPlanTemplateID.Name].ToString());
            }
            else if (!string.IsNullOrEmpty(BudgetPlantemplateID))
            {
                lst_objBudgetPlanTemplateVM = GetSelectedData(null, ref m_strMessage, BudgetPlantemplateID);
                lst_objBudgetPlanTemplateVM.lstUserBudgetPlanAccessVM = GetSelectedDataUserAccess(BudgetPlantemplateID);
            }
            
            
            lst_objBudgetPlanTemplateVM.lstUser = GetSelectedDataUser();
            
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }
            Session["GetData"] = null;

            ViewDataDictionary m_vddBudgetPlanTemplate = new ViewDataDictionary();
            m_vddBudgetPlanTemplate.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddBudgetPlanTemplate.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
            m_vddBudgetPlanTemplate.Add("IsCopyFromPrevious", true);
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = lst_objBudgetPlanTemplateVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddBudgetPlanTemplate,
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
                m_dicSelectedRow = GetFormData(m_nvcParams);
                Session[dataSessionName] = JSON.Serialize(m_dicSelectedRow);
            }

            string m_strMessage = string.Empty;
            BudgetPlanTemplateVM lst_objBudgetPlanTemplateVM = new BudgetPlanTemplateVM();
            if (m_dicSelectedRow.Count > 0)
            {
                lst_objBudgetPlanTemplateVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage, null);
                lst_objBudgetPlanTemplateVM.lstUser = GetSelectedDataUser();
                lst_objBudgetPlanTemplateVM.lstUserBudgetPlanAccessVM = GetSelectedDataUserAccess(m_dicSelectedRow[UserBudgetPlanAccessVM.Prop.BudgetPlanTemplateID.Name].ToString());
            }
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddBudgetPlanTemplate = new ViewDataDictionary();
            m_vddBudgetPlanTemplate.Add(General.EnumDesc(Params.Caller), Caller);

            if (Session["GetData"] == null)
            {
                m_vddBudgetPlanTemplate.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            }
            else
            {
                m_vddBudgetPlanTemplate.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
            }
            Session["GetData"] = null;
            m_vddBudgetPlanTemplate.Add("IsCopyFromPrevious", false);
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = lst_objBudgetPlanTemplateVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddBudgetPlanTemplate,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<BudgetPlanTemplateVM> m_lstSelectedRow = new List<BudgetPlanTemplateVM>();
            m_lstSelectedRow = JSON.Deserialize<List<BudgetPlanTemplateVM>>(Selected);

            MBudgetPlanTemplateDA m_objMBudgetPlanTemplateDA = new MBudgetPlanTemplateDA();
            m_objMBudgetPlanTemplateDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (BudgetPlanTemplateVM m_objBudgetPlanTemplateVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifBudgetPlanTemplateVM = m_objBudgetPlanTemplateVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifBudgetPlanTemplateVM in m_arrPifBudgetPlanTemplateVM)
                    {
                        string m_strFieldName = m_pifBudgetPlanTemplateVM.Name;
                        object m_objFieldValue = m_pifBudgetPlanTemplateVM.GetValue(m_objBudgetPlanTemplateVM);
                        if (m_objBudgetPlanTemplateVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(BudgetPlanTemplateVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objMBudgetPlanTemplateDA.DeleteBC(m_objFilter, false);
                    if (m_objMBudgetPlanTemplateDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMBudgetPlanTemplateDA.Message);
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
            }
            if (m_lstMessage.Count <= 0)
                Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Deleted));
            else
                Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));

            return this.Direct();
        }
        public ActionResult DeleteVersion(string Selected)
        {
            //return this.Direct();

            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            //List<BudgetPlanTemplateVM> m_lstSelectedRow = new List<BudgetPlanTemplateVM>();
            //m_lstSelectedRow = JSON.Deserialize<List<BudgetPlanTemplateVM>>(Selected);

            //MBudgetPlanTemplateDA m_objMBudgetPlanTemplateDA = new MBudgetPlanTemplateDA();
            //m_objMBudgetPlanTemplateDA.ConnectionStringName = Global.ConnStrConfigName;
            //List<string> m_lstMessage = new List<string>();

            //try
            //{
            //    foreach (BudgetPlanTemplateVM m_objBudgetPlanTemplateVM in m_lstSelectedRow)
            //    {
            //        List<string> m_lstKey = new List<string>();
            //        Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            //        PropertyInfo[] m_arrPifBudgetPlanTemplateVM = m_objBudgetPlanTemplateVM.GetType().GetProperties();

            //        foreach (PropertyInfo m_pifBudgetPlanTemplateVM in m_arrPifBudgetPlanTemplateVM)
            //        {
            //            string m_strFieldName = m_pifBudgetPlanTemplateVM.Name;
            //            object m_objFieldValue = m_pifBudgetPlanTemplateVM.GetValue(m_objBudgetPlanTemplateVM);
            //            if (m_objBudgetPlanTemplateVM.IsKey(m_strFieldName))
            //            {
            //                m_lstKey.Add(m_objFieldValue.ToString());
            //                List<object> m_lstFilter = new List<object>();
            //                m_lstFilter.Add(Operator.Equals);
            //                m_lstFilter.Add(m_objFieldValue);
            //                m_objFilter.Add(BudgetPlanTemplateVM.Prop.Map(m_strFieldName, false), m_lstFilter);
            //            }
            //            else break;
            //        }

            //        m_objMBudgetPlanTemplateDA.DeleteBC(m_objFilter, false);
            //        if (m_objMBudgetPlanTemplateDA.Message != string.Empty)
            //            m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMBudgetPlanTemplateDA.Message);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    m_lstMessage.Add(ex.Message);
            //}
            //if (m_lstMessage.Count <= 0)
            //    Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Deleted));
            //else
            //    Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));

            return this.Direct();
        }

        public ActionResult Browse(string ControlBudgetPlanTemplateID, string ControlBudgetPlanTemplateDesc, string ControlBudgetPlanTypeDesc, string FilterBudgetPlanTemplateID = "", string FilterBudgetPlanTemplateDesc = "",string ControlFromPage = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddBudgetPlanTemplate = new ViewDataDictionary();
            m_vddBudgetPlanTemplate.Add("Control" + BudgetPlanTemplateVM.Prop.BudgetPlanTemplateID.Name, ControlBudgetPlanTemplateID);
            m_vddBudgetPlanTemplate.Add("Control" + BudgetPlanTemplateVM.Prop.BudgetPlanTemplateDesc.Name, ControlBudgetPlanTemplateDesc);
            m_vddBudgetPlanTemplate.Add("Control" + BudgetPlanTemplateVM.Prop.BudgetPlanTypeDesc.Name, ControlBudgetPlanTypeDesc);
            m_vddBudgetPlanTemplate.Add("ControlFromPage", ControlFromPage);
            m_vddBudgetPlanTemplate.Add(BudgetPlanTemplateVM.Prop.BudgetPlanTemplateID.Name, FilterBudgetPlanTemplateID);
            m_vddBudgetPlanTemplate.Add(BudgetPlanTemplateVM.Prop.BudgetPlanTemplateDesc.Name, FilterBudgetPlanTemplateDesc);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddBudgetPlanTemplate,
                ViewName = "../BudgetPlanTemplate/_Browse"
            };
        }
        public ActionResult BrowseStructure(string ControlItemID, string ControlItemDesc, string ControlVersion, string ControlSequence, string ControlItemTypeID,
            string ControlParentItemID, string ControlParentVersion, string ControlParentSequence, string ControlParentItemTypeID,
            string ControlIsDefault, string BudgetPlanTemplateID, string ParentItemID, string ParentVersion, string Caller)
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddBudgetPlanTemplate = new ViewDataDictionary();
            m_vddBudgetPlanTemplate.Add("Control" + BudgetPlanTemplateStructureVM.Prop.ItemID.Name, ControlItemID);
            m_vddBudgetPlanTemplate.Add("Control" + BudgetPlanTemplateStructureVM.Prop.ItemDesc.Name, ControlItemDesc);
            m_vddBudgetPlanTemplate.Add("Control" + BudgetPlanTemplateStructureVM.Prop.Version.Name, ControlVersion);
            m_vddBudgetPlanTemplate.Add("Control" + BudgetPlanTemplateStructureVM.Prop.Sequence.Name, ControlSequence);
            m_vddBudgetPlanTemplate.Add("Control" + BudgetPlanTemplateStructureVM.Prop.ParentItemID.Name, ControlParentItemID);
            m_vddBudgetPlanTemplate.Add("Control" + BudgetPlanTemplateStructureVM.Prop.ParentVersion.Name, ControlParentVersion);
            m_vddBudgetPlanTemplate.Add("Control" + BudgetPlanTemplateStructureVM.Prop.ParentSequence.Name, ControlParentSequence);
            m_vddBudgetPlanTemplate.Add("Control" + BudgetPlanTemplateStructureVM.Prop.IsDefault.Name, ControlIsDefault);
            m_vddBudgetPlanTemplate.Add(BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.Name, BudgetPlanTemplateID);
            m_vddBudgetPlanTemplate.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemID.Name, ParentItemID);
            m_vddBudgetPlanTemplate.Add(BudgetPlanTemplateStructureVM.Prop.ParentVersion.Name, ParentVersion);

            m_vddBudgetPlanTemplate.Add(General.EnumDesc(Params.Caller), Caller);
            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddBudgetPlanTemplate,
                ViewName = "../BudgetPlanTemplate/Structure/_Browse"
            };
        }

        public ActionResult BrowseStructureTree(string ControlItemID, string ControlItemDesc, string ControlVersion, string ControlSequence, string ControlItemTypeID,
            string ControlParentItemID, string ControlParentVersion, string ControlParentSequence, string ControlParentItemTypeID,
            string ControlIsDefault, string BudgetPlanTemplateID, string ParentItemID, string ParentVersion, string Caller)
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            string message = string.Empty;
            List<BudgetPlanTemplateStructureVM> m_lstBudgetPlanVersionStructureVM = GetSelectedDataStructure(null, ref message, BudgetPlanTemplateID, Caller);


            ViewDataDictionary m_vddBudgetPlanTemplate = new ViewDataDictionary();
            m_vddBudgetPlanTemplate.Add("Control" + BudgetPlanTemplateStructureVM.Prop.ItemID.Name, ControlItemID);
            m_vddBudgetPlanTemplate.Add("Control" + BudgetPlanTemplateStructureVM.Prop.ItemDesc.Name, ControlItemDesc);
            m_vddBudgetPlanTemplate.Add("Control" + BudgetPlanTemplateStructureVM.Prop.Version.Name, ControlVersion);
            m_vddBudgetPlanTemplate.Add("Control" + BudgetPlanTemplateStructureVM.Prop.Sequence.Name, ControlSequence);
            m_vddBudgetPlanTemplate.Add("Control" + BudgetPlanTemplateStructureVM.Prop.ParentItemID.Name, ControlParentItemID);
            m_vddBudgetPlanTemplate.Add("Control" + BudgetPlanTemplateStructureVM.Prop.ParentVersion.Name, ControlParentVersion);
            m_vddBudgetPlanTemplate.Add("Control" + BudgetPlanTemplateStructureVM.Prop.ParentSequence.Name, ControlParentSequence);
            m_vddBudgetPlanTemplate.Add("Control" + BudgetPlanTemplateStructureVM.Prop.IsDefault.Name, ControlIsDefault);
            m_vddBudgetPlanTemplate.Add(BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.Name, BudgetPlanTemplateID);
            m_vddBudgetPlanTemplate.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemID.Name, ParentItemID);
            m_vddBudgetPlanTemplate.Add(BudgetPlanTemplateStructureVM.Prop.ParentVersion.Name, ParentVersion);
            m_vddBudgetPlanTemplate.Add(General.EnumDesc(Params.Caller), Caller);
            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddBudgetPlanTemplate,
                Model = m_lstBudgetPlanVersionStructureVM,
                ViewName = "../BudgetPlanTemplate/Structure/_BrowseTree"
            };
        }

        public ActionResult Save(string Action, string ListStructure, string UpdateForm)
        {
            List<BudgetPlanTemplateStructureVM> d_lstBudgetPlanTemplateStructureVM = new List<BudgetPlanTemplateStructureVM>();
            BudgetPlanTemplateStructureVM obj_BudgetPlanTemplateStructureVM = new BudgetPlanTemplateStructureVM();
            string BudgetPlanTemplateID = string.Empty;
            string ParamUserAccess = "ListUserAccess";

            if (this.Request.Params["ListStructure"] != null)
            {
                Dictionary<string, object>[] m_arrBudgetStructureChild = JSON.Deserialize<Dictionary<string, object>[]>(this.Request.Params["ListStructure"]);
                if (m_arrBudgetStructureChild == null)
                    m_arrBudgetStructureChild = new List<Dictionary<string, object>>().ToArray();
                foreach (Dictionary<string, object> m_dicBudgetStructureVM in m_arrBudgetStructureChild)
                {
                    BudgetPlanTemplateStructureVM obj = new BudgetPlanTemplateStructureVM();
                    obj.ItemDesc = m_dicBudgetStructureVM[BudgetPlanTemplateStructureVM.Prop.ItemDesc.Name.ToLower()].ToString();
                    obj.BudgetPlanTemplateID = m_dicBudgetStructureVM[BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.Name.ToLower()].ToString();

                    

                    obj.ItemID = m_dicBudgetStructureVM[BudgetPlanTemplateStructureVM.Prop.ItemID.Name.ToLower()].ToString();
                    obj.Version = Convert.ToInt16(m_dicBudgetStructureVM[BudgetPlanTemplateStructureVM.Prop.Version.Name.ToLower()].ToString());
                    obj.Sequence = Convert.ToInt16(m_dicBudgetStructureVM[BudgetPlanTemplateStructureVM.Prop.Sequence.Name.ToLower()].ToString());
                    obj.ParentItemID = m_dicBudgetStructureVM[BudgetPlanTemplateStructureVM.Prop.ParentItemID.Name.ToLower()].ToString();
                    obj.ParentVersion = Convert.ToInt16(m_dicBudgetStructureVM[BudgetPlanTemplateStructureVM.Prop.ParentVersion.Name.ToLower()].ToString());
                    obj.ParentSequence = Convert.ToInt16(m_dicBudgetStructureVM[BudgetPlanTemplateStructureVM.Prop.ParentSequence.Name.ToLower()].ToString());
                    if (!bool.Parse(m_dicBudgetStructureVM[BudgetPlanTemplateStructureVM.Prop.EnableDefault.Name.ToLower()].ToString()))
                        obj.IsDefault = null;
                    else
                    {
                        obj.IsDefault = Convert.ToBoolean(m_dicBudgetStructureVM[BudgetPlanTemplateStructureVM.Prop.IsDefault.Name.ToLower()].ToString());
                    }
                    d_lstBudgetPlanTemplateStructureVM.Add(obj);
                }
            }


            List<string> m_lstMessage = new List<string>();
            MBudgetPlanTemplateDA m_objMBudgetPlanTemplateDA = new MBudgetPlanTemplateDA();
            m_objMBudgetPlanTemplateDA.ConnectionStringName = Global.ConnStrConfigName;
            DBudgetPlanTemplateStructureDA d_objDBudgetPlanTemplateStructureDA = new DBudgetPlanTemplateStructureDA();
            d_objDBudgetPlanTemplateStructureDA.ConnectionStringName = Global.ConnStrConfigName;

            string m_strBudgetPlanTemplateID = this.Request.Params[BudgetPlanTemplateVM.Prop.BudgetPlanTemplateID.Name];
            object m_objDBConnection = null;
            string m_strTransName = "SaveBudgetPlanStructure";
            try
            {


                m_objDBConnection = m_objMBudgetPlanTemplateDA.BeginTrans(m_strTransName);
                string m_strBudgetPlanTemplateDesc = this.Request.Params[BudgetPlanTemplateVM.Prop.BudgetPlanTemplateDesc.Name];
                string m_strBudgetPlanTypeID = this.Request.Params[BudgetPlanTypeVM.Prop.BudgetPlanTypeID.Name];


                m_lstMessage = IsSaveValid(Action, m_strBudgetPlanTemplateID, m_strBudgetPlanTemplateDesc, m_strBudgetPlanTypeID);
                if (m_lstMessage.Count <= 0)
                {
                    MBudgetPlanTemplate m_objMBudgetPlanTemplate = new MBudgetPlanTemplate();
                    m_objMBudgetPlanTemplate.BudgetPlanTemplateID = m_strBudgetPlanTemplateID;
                    m_objMBudgetPlanTemplateDA.Data = m_objMBudgetPlanTemplate;

                    m_objMBudgetPlanTemplate.BudgetPlanTemplateDesc = m_strBudgetPlanTemplateDesc;
                    m_objMBudgetPlanTemplate.BudgetPlanTypeID = m_strBudgetPlanTypeID;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd) && string.IsNullOrEmpty(UpdateForm))
                    {
                        m_objMBudgetPlanTemplateDA.Insert(true, m_objDBConnection);
                    }
                    else
                    {
                        m_objMBudgetPlanTemplateDA.Update(true, m_objDBConnection);
                        Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                        List<object> m_lstFilter = new List<object>();
                        m_lstFilter.Add(Operator.Equals);
                        m_lstFilter.Add(m_strBudgetPlanTemplateID);
                        m_objFilter.Add(BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.Map, m_lstFilter);

                        bool m_boolIsCount = false;
                        List<string> m_lstSelect = new List<string>();
                        m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.MapAlias);
                        m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemID.MapAlias);
                        m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemDesc.MapAlias);
                        m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.Version.MapAlias);
                        m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.Sequence.MapAlias);
                        m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemTypeID.MapAlias);
                        m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemID.MapAlias);
                        m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemTypeID.MapAlias);
                        m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentVersion.MapAlias);
                        m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentSequence.MapAlias);
                        m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemVersionChildID.MapAlias);
                        m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.IsDefault.MapAlias);
                        m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.UoMDesc.MapAlias);
                        
                        d_objDBudgetPlanTemplateStructureDA.SelectBC(0, null, m_boolIsCount, m_lstSelect, m_objFilter, null, null, null, null);
                        if (d_objDBudgetPlanTemplateStructureDA.AffectedRows > 0)
                            d_objDBudgetPlanTemplateStructureDA.DeleteBC(m_objFilter, true, m_objDBConnection);

                    }
                    DBudgetPlanTemplateStructure d_objMBudgetPlanTemplateStructure = new DBudgetPlanTemplateStructure();
                    foreach (BudgetPlanTemplateStructureVM dvm in d_lstBudgetPlanTemplateStructureVM)
                    {
                        d_objMBudgetPlanTemplateStructure.BudgetPlanTemplateID = m_strBudgetPlanTemplateID;
                        d_objMBudgetPlanTemplateStructure.ItemID = dvm.ItemID;
                        d_objMBudgetPlanTemplateStructure.Version = dvm.Version;
                        d_objMBudgetPlanTemplateStructure.Sequence = dvm.Sequence;
                        d_objMBudgetPlanTemplateStructure.ParentItemID = dvm.ParentItemID;
                        d_objMBudgetPlanTemplateStructure.ParentVersion = dvm.ParentVersion;
                        d_objMBudgetPlanTemplateStructure.ParentSequence = dvm.ParentSequence;
                        if (dvm.IsDefault != null)
                            d_objMBudgetPlanTemplateStructure.IsDefault = dvm.IsDefault == true ? true : false;
                        else
                            d_objMBudgetPlanTemplateStructure.IsDefault = null;

                        d_objDBudgetPlanTemplateStructureDA.Data = d_objMBudgetPlanTemplateStructure;

                        if (Action == General.EnumDesc(Buttons.ButtonAdd) && string.IsNullOrEmpty(UpdateForm))
                        {
                            //d_objDBudgetPlanTemplateStructureDA.Select();
                            d_objDBudgetPlanTemplateStructureDA.Insert(true, m_objDBConnection);
                            if (d_objDBudgetPlanTemplateStructureDA.Message.Count() > 0)
                            {
                                m_objMBudgetPlanTemplateDA.Success = false;
                                break;
                            }
                        }
                        else if (Action == General.EnumDesc(Buttons.ButtonUpdate) || !string.IsNullOrEmpty(UpdateForm))
                        {
                            if (d_objDBudgetPlanTemplateStructureDA.Success)
                            {
                                d_objDBudgetPlanTemplateStructureDA.Data = d_objMBudgetPlanTemplateStructure;
                                //d_objDBudgetPlanTemplateStructureDA.Select();
                                d_objDBudgetPlanTemplateStructureDA.Insert(true, m_objDBConnection);
                                if (d_objDBudgetPlanTemplateStructureDA.Message.Count() > 0)
                                {
                                    m_objMBudgetPlanTemplateDA.Success = false;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (!m_objMBudgetPlanTemplateDA.Success || m_objMBudgetPlanTemplateDA.Message != string.Empty)
                {
                    m_objMBudgetPlanTemplateDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
                    m_lstMessage.Add(m_objMBudgetPlanTemplateDA.Message);
                }
                else
                    m_objMBudgetPlanTemplateDA.EndTrans(ref m_objDBConnection, true, m_strTransName);

                if (!this.SaveUserAccess(ParamUserAccess, m_strBudgetPlanTemplateID))
                {
                    Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.ErrorSave));
                    return Detail(Action, null, m_strBudgetPlanTemplateID);
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                m_objMBudgetPlanTemplateDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
                Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            }

            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfoAlert(title, General.EnumDesc(!string.IsNullOrEmpty(UpdateForm) ? MessageLib.Updated : MessageLib.Added));
                return Detail(Action, null, m_strBudgetPlanTemplateID);
            }

            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);
        }

        #endregion

        #region Direct Method

        public ActionResult GetBudgetPlanTemplate(string ControlBudgetPlanTemplateID, string ControlBudgetPlanTemplateDesc, string ControlBudgetPlanTypeDesc, string FilterBudgetPlanTemplateID, string FilterBudgetPlanTemplateDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<BudgetPlanTemplateVM>> m_dicBudgetPlanTemplateData = GetBudgetPlanTemplateData(true, FilterBudgetPlanTemplateID, FilterBudgetPlanTemplateDesc);
                KeyValuePair<int, List<BudgetPlanTemplateVM>> m_kvpBudgetPlanTemplateVM = m_dicBudgetPlanTemplateData.AsEnumerable().ToList()[0];
                if (m_kvpBudgetPlanTemplateVM.Key < 1 || (m_kvpBudgetPlanTemplateVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpBudgetPlanTemplateVM.Key > 1 && !Exact)
                    return Browse(ControlBudgetPlanTemplateID, ControlBudgetPlanTemplateDesc, ControlBudgetPlanTypeDesc, FilterBudgetPlanTemplateID, FilterBudgetPlanTemplateDesc);

                m_dicBudgetPlanTemplateData = GetBudgetPlanTemplateData(false, FilterBudgetPlanTemplateID, FilterBudgetPlanTemplateDesc);
                BudgetPlanTemplateVM m_objBudgetPlanTemplateVM = m_dicBudgetPlanTemplateData[0][0];
                this.GetCmp<TextField>(ControlBudgetPlanTemplateID).Value = m_objBudgetPlanTemplateVM.BudgetPlanTemplateID;
                this.GetCmp<TextField>(ControlBudgetPlanTemplateDesc).Value = m_objBudgetPlanTemplateVM.BudgetPlanTemplateDesc;
                this.GetCmp<TextField>(ControlBudgetPlanTypeDesc).Value = m_objBudgetPlanTemplateVM.BudgetPlanTypeDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private Node CheckChildren(string ParentItemID, string ParentItemTypeID, int ParentVersion, ref int Sequence, string BudgetTemplateID, Node ParentNode, List<string> m_lstUPA, List<string> m_lstUPAAHS, ref bool isEnableDefault, ref bool isdefaultcheck)
        {

            bool isdefaultval = false;

            BudgetPlanTemplateStructureVM m_objParentBudgetTemplateStructure = new BudgetPlanTemplateStructureVM();
            Node m_node = new Node();
            Dictionary<string, List<object>> m_objFiltersFirst = new Dictionary<string, List<object>>();
            List<string> m_lstSelectsFirst = new List<string>();

            m_lstSelectsFirst.Add(ItemVersionChildVM.Prop.ItemDesc.Map);
            m_lstSelectsFirst.Add(ItemVersionChildVM.Prop.ItemID.Map);
            m_lstSelectsFirst.Add(ItemVersionChildVM.Prop.Version.Map);
            m_lstSelectsFirst.Add(ItemVersionChildVM.Prop.Sequence.Map);
            m_lstSelectsFirst.Add(ItemVersionChildVM.Prop.ItemTypeID.Map);
            m_lstSelectsFirst.Add(ItemVersionChildVM.Prop.ItemGroupDesc.Map);
            m_lstSelectsFirst.Add(ItemVersionChildVM.Prop.ChildItemID.Map);
            m_lstSelectsFirst.Add(ItemVersionChildVM.Prop.ChildVersion.Map);

            List<object> m_lstFilterss = new List<object>();
            m_lstFilterss.Add(Operator.Equals);
            m_lstFilterss.Add(ParentItemID);
            m_objFiltersFirst.Add(ItemVersionChildVM.Prop.ItemID.Map, m_lstFilterss);

            List<object> m_lstFiltersv = new List<object>();
            m_lstFiltersv.Add(Operator.Equals);
            m_lstFiltersv.Add(ParentVersion);
            m_objFiltersFirst.Add(ItemVersionChildVM.Prop.Version.Map, m_lstFiltersv);

            DItemVersionChildDA m_objVersionChild = new DItemVersionChildDA();
            m_objVersionChild.ConnectionStringName = Global.ConnStrConfigName;

            Dictionary<int, DataSet> m_dicMBudgetPlanTemplateChildDA = m_objVersionChild.SelectBC(0, null, false, m_lstSelectsFirst, m_objFiltersFirst, null, null, null, null);
            if (m_objVersionChild.Message == string.Empty && m_objVersionChild.AffectedRows > 0)
            {

                foreach (DataRow m_dataRow in m_dicMBudgetPlanTemplateChildDA[0].Tables[0].Rows)
                {
                    bool enabledefaultForThis = false;
                    ItemVersionVM m_objitemVersion = new ItemVersionVM();
                    m_objitemVersion.ItemDesc = m_dataRow[ItemVersionChildVM.Prop.ItemDesc.Name].ToString();
                    m_objitemVersion.ItemID = m_dataRow[ItemVersionChildVM.Prop.ChildItemID.Name].ToString();
                    m_objitemVersion.Version = Convert.ToInt16(m_dataRow[ItemVersionChildVM.Prop.ChildVersion.Name].ToString());
                    m_objitemVersion.ItemTypeID = m_dataRow[ItemVersionChildVM.Prop.ItemTypeID.Name].ToString();
                    m_objitemVersion.ItemGroupDesc = m_dataRow[ItemVersionChildVM.Prop.ItemGroupDesc.Name].ToString();
                    m_objParentBudgetTemplateStructure.IsAHS = false;
                    m_objParentBudgetTemplateStructure.IsBOI = false;
                    foreach (string match in m_lstUPA)
                    {
                        if (match == m_objitemVersion.ItemTypeID)
                        {
                            m_objParentBudgetTemplateStructure.HasChild = true;
                            m_objParentBudgetTemplateStructure.AllowDelete = true;
                            m_objParentBudgetTemplateStructure.IsBOI = true;
                            break;
                        }
                        else
                        {
                            m_objParentBudgetTemplateStructure.HasChild = false;
                            m_objParentBudgetTemplateStructure.AllowDelete = false;
                        }
                    }
                    foreach (string keyahs in m_lstUPAAHS)
                    {
                        if (keyahs == m_objitemVersion.ItemTypeID)
                        {
                            m_objParentBudgetTemplateStructure.IsAHS = true;
                            break;
                        }
                    }

                    if (!isEnableDefault && !m_objParentBudgetTemplateStructure.HasChild)
                    {
                        enabledefaultForThis = true;
                        isEnableDefault = true;
                        if (isdefaultcheck)
                        {
                            isdefaultval = true;
                            isdefaultcheck = false;
                        }
                    }

                    m_objParentBudgetTemplateStructure.ItemDesc = m_objitemVersion.ItemDesc;
                    m_objParentBudgetTemplateStructure.BudgetPlanTemplateID = BudgetTemplateID;
                    m_objParentBudgetTemplateStructure.ItemID = m_objitemVersion.ItemID;
                    m_objParentBudgetTemplateStructure.Version = m_objitemVersion.Version;
                    m_objParentBudgetTemplateStructure.Sequence = Sequence + 1;
                    m_objParentBudgetTemplateStructure.ParentItemID = ParentItemID;
                    m_objParentBudgetTemplateStructure.ParentVersion = ParentVersion;
                    m_objParentBudgetTemplateStructure.ParentSequence = Sequence;
                    m_objParentBudgetTemplateStructure.ItemTypeID = m_objitemVersion.ItemTypeID;
                    m_objParentBudgetTemplateStructure.ItemGroupDesc = m_objitemVersion.ItemGroupDesc;
                    m_objParentBudgetTemplateStructure.ParentItemTypeID = ParentItemTypeID;
                    m_objParentBudgetTemplateStructure.IsDefault = false;
                    m_objParentBudgetTemplateStructure.EnableDefault = enabledefaultForThis;
                    Sequence++;
                    m_node = createNodeVersion(m_objParentBudgetTemplateStructure, false, isdefaultval);

                    #region CheckChild_Loop
                    m_node = CheckChildren(m_objParentBudgetTemplateStructure.ItemID, m_objParentBudgetTemplateStructure.ItemTypeID, m_objParentBudgetTemplateStructure.Version, ref Sequence, BudgetTemplateID, m_node, m_lstUPA, m_lstUPAAHS, ref isEnableDefault, ref isdefaultcheck);
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
        private Node CheckChildrenFirstLoad(BudgetPlanTemplateStructureVM itemmodel, ref int sequence, Node ParentNode, List<string> lstAHS, List<string> lstBOI)
        {
            Dictionary<string, List<object>> m_objFiltersFirst = new Dictionary<string, List<object>>();
            List<string> m_lstSelectsFirst = new List<string>();

            m_lstSelectsFirst.Add(ItemVersionChildVM.Prop.ItemDesc.Map);
            m_lstSelectsFirst.Add(ItemVersionChildVM.Prop.ItemID.Map);
            m_lstSelectsFirst.Add(ItemVersionChildVM.Prop.Version.Map);
            m_lstSelectsFirst.Add(ItemVersionChildVM.Prop.Sequence.Map);
            m_lstSelectsFirst.Add(ItemVersionChildVM.Prop.ItemTypeID.Map);
            m_lstSelectsFirst.Add(ItemVersionChildVM.Prop.ChildItemID.Map);
            m_lstSelectsFirst.Add(ItemVersionChildVM.Prop.ChildVersion.Map);

            List<object> m_lstFilterss = new List<object>();
            m_lstFilterss.Add(Operator.Equals);
            m_lstFilterss.Add(itemmodel.ItemID);
            m_objFiltersFirst.Add(ItemVersionChildVM.Prop.ItemID.Map, m_lstFilterss);

            List<object> m_lstFiltersv = new List<object>();
            m_lstFiltersv.Add(Operator.Equals);
            m_lstFiltersv.Add(itemmodel.Version);
            m_objFiltersFirst.Add(ItemVersionChildVM.Prop.Version.Map, m_lstFiltersv);

            DItemVersionChildDA m_objVersionChild = new DItemVersionChildDA();
            m_objVersionChild.ConnectionStringName = Global.ConnStrConfigName;

            Dictionary<string, OrderDirection> m_objOrderBy = new Dictionary<string, OrderDirection>();
            m_objOrderBy.Add(ItemVersionChildVM.Prop.Sequence.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicMBudgetPlanTemplateChildDA = m_objVersionChild.SelectBC(0, null, false, m_lstSelectsFirst, m_objFiltersFirst, null, null, m_objOrderBy, null);
            if (m_objVersionChild.Message == string.Empty && m_objVersionChild.AffectedRows > 0)
            {

                foreach (DataRow m_dataRow in m_dicMBudgetPlanTemplateChildDA[0].Tables[0].Rows)
                {
                    sequence += 1;
                    BudgetPlanTemplateStructureVM m_objitemVersion = new BudgetPlanTemplateStructureVM();
                    m_objitemVersion.ItemDesc = m_dataRow[ItemVersionChildVM.Prop.ItemDesc.Name].ToString();
                    m_objitemVersion.ItemID = m_dataRow[ItemVersionChildVM.Prop.ChildItemID.Name].ToString();
                    m_objitemVersion.Version = Convert.ToInt16(m_dataRow[ItemVersionChildVM.Prop.ChildVersion.Name].ToString());
                    m_objitemVersion.ItemTypeID = m_dataRow[ItemVersionChildVM.Prop.ItemTypeID.Name].ToString();
                    m_objitemVersion.BudgetPlanTemplateID = itemmodel.BudgetPlanTemplateID;
                    m_objitemVersion.Sequence = sequence;
                    m_objitemVersion.ParentItemID = itemmodel.ItemID;
                    m_objitemVersion.ParentVersion = itemmodel.Version;
                    m_objitemVersion.ParentSequence = itemmodel.Sequence;
                    m_objitemVersion.ParentItemTypeID = itemmodel.ItemTypeID;
                    m_objitemVersion.IsDefault = null;
                    m_objitemVersion.EnableDefault = false;
                    m_objitemVersion.IsAHS = isAHS(m_objitemVersion.ItemTypeID, lstAHS);
                    m_objitemVersion.IsBOI = isBOI(m_objitemVersion.ItemTypeID, lstBOI);
                    m_objitemVersion.AllowDelete = m_objitemVersion.IsBOI;

                    //if (m_objitemVersion.ItemID != m_objitemVersion.ParentItemID && m_objitemVersion.Version != m_objitemVersion.ParentVersion && m_objitemVersion.Sequence != m_objitemVersion.ParentSequence)
                    //{
                    Node cnode = new Node();
                    cnode.Icon = !m_objitemVersion.IsAHS && !m_objitemVersion.IsBOI ? Icon.PageWhite : Icon.Table;
                    cnode.AttributesObject = new
                    {
                        number = "",
                        itemdesc = m_objitemVersion.ItemDesc,
                        budgetplantemplateid = m_objitemVersion.BudgetPlanTemplateID,
                        itemid = m_objitemVersion.ItemID,
                        version = m_objitemVersion.Version,
                        sequence = m_objitemVersion.Sequence,
                        parentitemid = m_objitemVersion.ParentItemID,
                        parentversion = m_objitemVersion.ParentVersion,
                        parentsequence = m_objitemVersion.ParentSequence,
                        isdefault = false,
                        allowdelete = m_objitemVersion.AllowDelete,
                        haschild = m_objitemVersion.HasChild,
                        itemgroupdesc = m_objitemVersion.ItemGroupDesc,
                        itemtypeid = m_objitemVersion.ItemTypeID,
                        parentitemtypeid = m_objitemVersion.ParentItemTypeID,
                        enabledefault = m_objitemVersion.EnableDefault
                    };
                    #region CheckChild_Loop

                    cnode = CheckChildrenFirstLoad(m_objitemVersion, ref sequence, cnode, lstAHS, lstBOI);
                    //cnode.Expandable = cnode.Children.Count > 0;
                    //cnode.Expanded = cnode.Expandable;

                    ParentNode.Children.Add(cnode);

                    ParentNode.Expandable = true;
                    ParentNode.Expanded = true;
                    #endregion
                    //}
                    //else
                    //{
                    //    Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, "May cause Recursive Infinite Loop"));
                    //    break;
                    //}
                }
            }
            else
            {
                ParentNode.Expandable = false;
                ParentNode.Expanded = false;
            }

            return ParentNode;
        }

        private Node ListToAppend(string itemdesc, string ItemId, string itemtypeid, int version_, string parentitem, int parentversion, string ParentItemTypeID, int parentsequence, List<BudgetPlanTemplateStructureVM> lst_BudgetPlan, string budgetplanstructureID, int sequence, ref string idNode, bool enableDefault)
        {
            string[] generateID = idNode.Split('/');
            generateID[1] = (Convert.ToInt16(generateID[1]) + 1).ToString();
            idNode = generateID[0] + "." + generateID[1] + "/0";
            string getID = generateID[0] + "." + generateID[1];

            Node m_node = new Node();

            BudgetPlanTemplateStructureVM m_objParentBudgetTemplateStructure = new BudgetPlanTemplateStructureVM();
            List<BudgetPlanTemplateStructureVM> lstStructure = new List<BudgetPlanTemplateStructureVM>();
            List<ItemVersionChildVM> ListtoAppendss = new List<ItemVersionChildVM>();
            DItemVersionDA m_objVersion = new DItemVersionDA();
            m_objVersion.ConnectionStringName = Global.ConnStrConfigName;
            DItemVersionChildDA m_objMItemVersionDA = new DItemVersionChildDA();
            m_objMItemVersionDA.ConnectionStringName = Global.ConnStrConfigName;

            Dictionary<string, List<object>> m_objFiltersFirst = new Dictionary<string, List<object>>();
            Dictionary<string, List<object>> m_objFilters = new Dictionary<string, List<object>>();

            List<string> m_lstSelectsFirst = new List<string>();
            List<string> m_lstSelects = new List<string>();
            List<string> m_lstUPA = new List<string>();
            if (ItemId != "")
            {
                m_lstSelectsFirst.Add(ItemVersionVM.Prop.ItemDesc.Map);
                m_lstSelectsFirst.Add(ItemVersionVM.Prop.ItemID.Map);
                m_lstSelectsFirst.Add(ItemVersionVM.Prop.Version.Map);
                m_lstSelectsFirst.Add(ItemVersionVM.Prop.ItemTypeID.Map);

                List<object> m_lstFilterss = new List<object>();
                m_lstFilterss.Add(Operator.Equals);
                m_lstFilterss.Add(ItemId);
                m_objFiltersFirst.Add(ItemVersionVM.Prop.ItemID.Map, m_lstFilterss);

                m_lstSelects.Add(ItemVersionChildVM.Prop.ItemDesc.Map);
                m_lstSelects.Add(ItemVersionChildVM.Prop.ItemID.Map);
                m_lstSelects.Add(ItemVersionChildVM.Prop.Version.Map);
                m_lstSelects.Add(ItemVersionChildVM.Prop.ChildItemID.Map);
                m_lstSelects.Add(ItemVersionChildVM.Prop.ChildVersion.Map);
                m_lstSelects.Add(ItemVersionChildVM.Prop.ItemTypeID.Map);
                m_lstSelects.Add(ItemVersionChildVM.Prop.Sequence.Map);

                List<object> m_lstFilters = new List<object>();
                m_lstFilters.Add(Operator.Equals);
                m_lstFilters.Add(ItemId);
                m_objFilters.Add(ItemVersionChildVM.Prop.ItemID.Map, m_lstFilters);

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

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add("TRUE");
                m_objFilteru.Add(ConfigVM.Prop.Desc1.Map, m_lstFilter);

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

                m_lstFilters = new List<object>();
                m_lstFilters.Add(Operator.In);
                m_lstFilters.Add(String.Join(",", m_lstUPA));

                List<object> m_lstFiltersChild = new List<object>();
                m_lstFiltersChild.Add(Operator.Equals);
                m_lstFiltersChild.Add(version_);
                m_objFilters.Add(ItemVersionChildVM.Prop.Version.Map, m_lstFiltersChild);


                List<object> m_lstFiltersv = new List<object>();
                m_lstFiltersv.Add(Operator.Equals);
                m_lstFiltersv.Add(version_);
                m_objFiltersFirst.Add(ItemVersionVM.Prop.Version.Map, m_lstFiltersv);

                //comment if child not follow CONFIG filter 
                //m_objFilters.Add(ItemVersionVM.Prop.ItemTypeID.Map, m_lstFilters);
                //m_objFiltersFirst.Add(ItemVersionVM.Prop.ItemTypeID.Map, m_lstFilters);

            }

            Dictionary<int, DataSet> m_dicMItemVersionDA = m_objMItemVersionDA.SelectBC(0, null, false, m_lstSelects, m_objFilters, null, null, null, null);
            List<ItemVersionChildVM> m_lstItemVersionVM = new List<ItemVersionChildVM>();
            if (m_objMItemVersionDA.AffectedRows > 0)
            {
                if (m_objMItemVersionDA.Message == string.Empty)
                {
                    m_lstItemVersionVM = (
                        from DataRow m_drMItemVersionDA in m_dicMItemVersionDA[0].Tables[0].Rows
                        select new ItemVersionChildVM()
                        {
                            ItemDesc = m_drMItemVersionDA[ItemVersionChildVM.Prop.ItemDesc.Name].ToString(),
                            ItemID = m_drMItemVersionDA[ItemVersionChildVM.Prop.ItemID.Name].ToString(),
                            Version = (int)m_drMItemVersionDA[ItemVersionChildVM.Prop.Version.Name],
                            ChildItemID = m_drMItemVersionDA[ItemVersionChildVM.Prop.ChildItemID.Name].ToString(),
                            ChildVersion = (int)m_drMItemVersionDA[ItemVersionChildVM.Prop.ChildVersion.Name],
                            ItemTypeID = m_drMItemVersionDA[ItemVersionChildVM.Prop.ItemTypeID.Name].ToString(),
                            Sequence = (int)m_drMItemVersionDA[ItemVersionChildVM.Prop.Sequence.Name]
                        }
                    ).ToList();
                    m_lstItemVersionVM = m_lstItemVersionVM.AsEnumerable().OrderBy(x => x.Sequence).ToList();
                    if (m_lstItemVersionVM.Count() > 0)
                    {

                        //Node test = new Node();
                        //m_node.Children.Add(test);
                        foreach (string match in m_lstUPA)
                        {
                            if (match == m_lstItemVersionVM[0].ItemTypeID)
                            {
                                m_objParentBudgetTemplateStructure.HasChild = true;
                                m_objParentBudgetTemplateStructure.AllowDelete = true;
                                break;
                            }
                            else
                                m_objParentBudgetTemplateStructure.HasChild = false;
                            m_objParentBudgetTemplateStructure.AllowDelete = false;
                        }
                        m_objParentBudgetTemplateStructure.ItemDesc = m_lstItemVersionVM[0].ItemDesc;//itemdesc;//m_lstItemVersionVM[0].ItemDesc;
                        m_objParentBudgetTemplateStructure.BudgetPlanTemplateID = budgetplanstructureID;
                        m_objParentBudgetTemplateStructure.ItemID = m_lstItemVersionVM[0].ItemID;
                        m_objParentBudgetTemplateStructure.Version = m_lstItemVersionVM[0].Version;
                        m_objParentBudgetTemplateStructure.Sequence = sequence/* + 100;*/ ;
                        m_objParentBudgetTemplateStructure.ParentItemID = parentitem;
                        m_objParentBudgetTemplateStructure.ParentVersion = parentversion;
                        m_objParentBudgetTemplateStructure.ParentSequence = parentsequence;
                        m_objParentBudgetTemplateStructure.ItemTypeID = m_lstItemVersionVM[0].ItemTypeID;
                        m_objParentBudgetTemplateStructure.ParentItemTypeID = ParentItemTypeID;
                        m_objParentBudgetTemplateStructure.IsDefault = false;
                        m_objParentBudgetTemplateStructure.EnableDefault = enableDefault;

                        if (enableDefault)
                            enableDefault = false;

                        lst_BudgetPlan.Add(m_objParentBudgetTemplateStructure);
                        m_node = createNodeVersion(m_objParentBudgetTemplateStructure, false, false);
                        string itemdescription = m_lstItemVersionVM[0].ItemDesc;
                        foreach (ItemVersionChildVM m_objitemVersion in m_lstItemVersionVM)
                        {

                            Node anyNode = new Node();
                            //anyNode =  createNodeVersion(m_objParentBudgetTemplateStructure, getID);
                            anyNode = ListToAppend(itemdescription, m_objitemVersion.ChildItemID, "", m_objitemVersion.ChildVersion, m_objParentBudgetTemplateStructure.ItemID, m_objParentBudgetTemplateStructure.Version, m_objParentBudgetTemplateStructure.ItemTypeID, m_objParentBudgetTemplateStructure.Sequence, lst_BudgetPlan, budgetplanstructureID, sequence + 1, ref idNode, enableDefault);
                            m_node.Children.Add(anyNode);
                        }

                    }
                }
            }
            else
            {
                List<ItemVersionVM> lst_budgetStructureVM = new List<ItemVersionVM>();
                Dictionary<int, DataSet> m_dicMBudgetPlanTemplateDA = m_objVersion.SelectBC(0, null, false, m_lstSelectsFirst, m_objFiltersFirst, null, null, null, null);
                if (m_objVersion.Message == string.Empty)
                {
                    foreach (DataRow m_dataRow in m_dicMBudgetPlanTemplateDA[0].Tables[0].Rows)
                    {
                        ItemVersionVM m_objMBudgetPlanTemplateStructureVM2 = new ItemVersionVM();
                        m_objMBudgetPlanTemplateStructureVM2.ItemDesc = m_dataRow[ItemVersionVM.Prop.ItemDesc.Name].ToString();
                        m_objMBudgetPlanTemplateStructureVM2.ItemID = m_dataRow[ItemVersionVM.Prop.ItemID.Name].ToString();
                        m_objMBudgetPlanTemplateStructureVM2.Version = Convert.ToInt16(m_dataRow[ItemVersionVM.Prop.Version.Name].ToString());
                        m_objMBudgetPlanTemplateStructureVM2.ItemTypeID = m_dataRow[ItemVersionVM.Prop.ItemTypeID.Name].ToString();

                        lst_budgetStructureVM.Add(m_objMBudgetPlanTemplateStructureVM2);
                    }


                    foreach (ItemVersionVM m_objitemVersion in lst_budgetStructureVM)
                    {
                        //sequence++;
                        foreach (string match in m_lstUPA)
                        {
                            if (match == m_objitemVersion.ItemTypeID)
                            {
                                m_objParentBudgetTemplateStructure.HasChild = true;
                                m_objParentBudgetTemplateStructure.AllowDelete = true;
                                break;
                            }
                            else
                                m_objParentBudgetTemplateStructure.HasChild = false;
                            m_objParentBudgetTemplateStructure.AllowDelete = false;
                        }
                        m_objParentBudgetTemplateStructure.ItemDesc = m_objitemVersion.ItemDesc;
                        m_objParentBudgetTemplateStructure.BudgetPlanTemplateID = budgetplanstructureID;
                        m_objParentBudgetTemplateStructure.ItemID = m_objitemVersion.ItemID;
                        m_objParentBudgetTemplateStructure.Version = m_objitemVersion.Version;
                        m_objParentBudgetTemplateStructure.Sequence = sequence/* + 100*/; ;
                        m_objParentBudgetTemplateStructure.ParentItemID = parentitem;
                        m_objParentBudgetTemplateStructure.ParentVersion = parentversion;
                        m_objParentBudgetTemplateStructure.ParentSequence = parentsequence;
                        m_objParentBudgetTemplateStructure.ItemTypeID = m_objitemVersion.ItemTypeID;
                        m_objParentBudgetTemplateStructure.ParentItemTypeID = ParentItemTypeID;
                        m_objParentBudgetTemplateStructure.IsDefault = false;
                        m_objParentBudgetTemplateStructure.EnableDefault = enableDefault;

                        //lst_BudgetPlan.Add(m_objParentBudgetTemplateStructure);
                    }

                    m_node = createNodeVersion(m_objParentBudgetTemplateStructure, false, false);
                }
                else
                {
                    m_node = null;
                }
            }

            return m_node;
        }
        private Node createNodeVersion(BudgetPlanTemplateStructureVM parent, bool isFromSelect, bool isdefaultCheck)
        {
            Node m_node = new Node();


            //m_node.Expanded = true;
            m_node.Icon = Icon.Folder;
            m_node.Text = parent.ItemID;
            //m_node.Leaf = true;
            // m_node.Expanded = false;
            m_node.AttributesObject = new
            {
                number = "",
                itemdesc = parent.ItemDesc,
                budgetplantemplateid = parent.BudgetPlanTemplateID,
                itemid = parent.ItemID,
                version = parent.Version,
                sequence = parent.Sequence,
                parentitemid = parent.ParentItemID,
                parentversion = parent.ParentVersion,
                parentsequence = parent.ParentSequence,
                isdefault = isdefaultCheck,
                allowdelete = parent.AllowDelete,
                haschild = parent.HasChild,/*morecode*/
                itemgroupdesc = parent.ItemGroupDesc,
                itemtypeid = parent.ItemTypeID,
                parentitemtypeid = parent.ParentItemTypeID,
                enabledefault = parent.EnableDefault
            };

            m_node.Icon = !parent.IsAHS && !parent.IsBOI ? Icon.PageWhite : (parent.IsAHS ? Icon.Table : Icon.Folder);
            //if (!parent.HasChild)
            //    m_node.Icon = Icon.Table;

            return m_node;
        }

        //private createRecursiveNode
        private List<string> IsSaveValid(string Action, string BudgetPlanTemplateID, string BudgetPlanTemplateDesc, string BudgetPlanTypeID)
        {
            List<string> m_lstReturn = new List<string>();
            if (BudgetPlanTemplateID == string.Empty)
                m_lstReturn.Add(BudgetPlanTemplateVM.Prop.BudgetPlanTemplateID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (BudgetPlanTemplateDesc == string.Empty)
                m_lstReturn.Add(BudgetPlanTemplateVM.Prop.BudgetPlanTemplateDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (BudgetPlanTypeID == string.Empty)
                m_lstReturn.Add(BudgetPlanTemplateVM.Prop.BudgetPlanTypeID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            //if (BudgetPlanTypeID == string.Empty)
            //    m_lstReturn.Add(BudgetPlanTemplateVM.Prop.BudgetPlanTypeID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();
            m_dicReturn.Add(BudgetPlanTemplateVM.Prop.BudgetPlanTemplateID.Name, parameters[BudgetPlanTemplateVM.Prop.BudgetPlanTemplateID.Name]);
            m_dicReturn.Add(BudgetPlanTemplateVM.Prop.BudgetPlanTemplateDesc.Name, parameters[BudgetPlanTemplateVM.Prop.BudgetPlanTemplateDesc.Name]);
            m_dicReturn.Add(BudgetPlanTemplateVM.Prop.BudgetPlanTypeID.Name, parameters[BudgetPlanTemplateVM.Prop.BudgetPlanTypeID.Name]);
            m_dicReturn.Add(BudgetPlanTemplateVM.Prop.BudgetPlanTypeDesc.Name, parameters[BudgetPlanTemplateVM.Prop.BudgetPlanTypeDesc.Name]);
            return m_dicReturn;
        }

        private BudgetPlanTemplateVM GetSelectedData(Dictionary<string, object> selected, ref string message, string BudgetPlantemplateID_)
        {
            BudgetPlanTemplateVM m_objBudgetPlanTemplateVM = new BudgetPlanTemplateVM();
            MBudgetPlanTemplateDA m_objMBudgetPlanTemplateDA = new MBudgetPlanTemplateDA();
            m_objMBudgetPlanTemplateDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanTemplateVM.Prop.BudgetPlanTemplateID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateVM.Prop.BudgetPlanTemplateDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateVM.Prop.BudgetPlanTypeID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateVM.Prop.BudgetPlanTypeDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            if (!string.IsNullOrEmpty(BudgetPlantemplateID_))
            {
                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(BudgetPlantemplateID_);
                m_objFilter.Add(BudgetPlanTemplateVM.Prop.BudgetPlanTemplateID.Map, m_lstFilter);
            }
            else
            {
                foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
                {
                    if (m_objBudgetPlanTemplateVM.IsKey(m_kvpSelectedRow.Key))
                    {
                        m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                        List<object> m_lstFilter = new List<object>();
                        m_lstFilter.Add(Operator.Equals);
                        m_lstFilter.Add(m_kvpSelectedRow.Value);
                        m_objFilter.Add(BudgetPlanTemplateVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                    }
                }
            }
            Dictionary<int, DataSet> m_dicMBudgetPlanTemplateDA = m_objMBudgetPlanTemplateDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMBudgetPlanTemplateDA.Message == string.Empty)
            {
                DataRow m_drMBudgetPlanTemplateDA = m_dicMBudgetPlanTemplateDA[0].Tables[0].Rows[0];
                m_objBudgetPlanTemplateVM.BudgetPlanTemplateID = m_drMBudgetPlanTemplateDA[BudgetPlanTemplateVM.Prop.BudgetPlanTemplateID.Name].ToString();
                m_objBudgetPlanTemplateVM.BudgetPlanTemplateDesc = m_drMBudgetPlanTemplateDA[BudgetPlanTemplateVM.Prop.BudgetPlanTemplateDesc.Name].ToString();
                m_objBudgetPlanTemplateVM.BudgetPlanTypeID = m_drMBudgetPlanTemplateDA[BudgetPlanTemplateVM.Prop.BudgetPlanTypeID.Name].ToString();
                m_objBudgetPlanTemplateVM.BudgetPlanTypeDesc = m_drMBudgetPlanTemplateDA[BudgetPlanTemplateVM.Prop.BudgetPlanTypeDesc.Name].ToString();
                m_objBudgetPlanTemplateVM.lstUserBudgetPlanAccessVM = GetSelectedDataUserAccess(m_drMBudgetPlanTemplateDA[BudgetPlanTemplateVM.Prop.BudgetPlanTemplateID.Name].ToString());
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMBudgetPlanTemplateDA.Message;
            List<BudgetPlanTemplateStructureVM> lstBudgetStructure = GetSelectedDataStructure(selected, ref message, BudgetPlantemplateID_);


            if (lstBudgetStructure.Count() == 0)
            {
                BudgetPlanTemplateStructureVM m_objMBudgetPlanTemplateStructureVM_ = new BudgetPlanTemplateStructureVM();
                m_objMBudgetPlanTemplateStructureVM_.BudgetPlanTemplateID = m_objBudgetPlanTemplateVM.BudgetPlanTemplateID;
                m_objMBudgetPlanTemplateStructureVM_.ItemID = "empty";
                lstBudgetStructure.Add(m_objMBudgetPlanTemplateStructureVM_);
            }
            m_objBudgetPlanTemplateVM.ListBudgetPlanTemplateStructureVM = lstBudgetStructure;
            return m_objBudgetPlanTemplateVM;
        }
        private List<UserBudgetPlanAccessVM> GetSelectedDataUserAccess(string BudgetPlanTemplateID)
        {
            List<UserBudgetPlanAccessVM> lst_objUserBudgetPlanAccessVM = new List<UserBudgetPlanAccessVM>();
            UserBudgetPlanAccessVM m_objUserBudgetPlanAccessVM;
            DUserBudgetPlanAccessDA m_objUserBudgetPlanAccessDA = new DUserBudgetPlanAccessDA();

            m_objUserBudgetPlanAccessDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(UserBudgetPlanAccessVM.Prop.UserID.MapAlias);
            m_lstSelect.Add(UserBudgetPlanAccessVM.Prop.BudgetPlanTemplateID.MapAlias);
            m_lstSelect.Add(UserBudgetPlanAccessVM.Prop.StartDate.MapAlias);
            m_lstSelect.Add(UserBudgetPlanAccessVM.Prop.EndDate.MapAlias);

            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanTemplateID);

            m_objFilter.Add(BudgetPlanTemplateVM.Prop.BudgetPlanTemplateID.Map, m_lstFilter);

            //BudgetPlanTemplateID = 'JL1' AND GETDATE() BETWEEN StartDate AND EndDate
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.None);
            m_lstFilter.Add(String.Empty);
            m_objFilter.Add($" GETDATE() BETWEEN StartDate AND EndDate ", m_lstFilter);

            Dictionary<int, DataSet> m_dicDUserBudgetPlanAccessDA = m_objUserBudgetPlanAccessDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objUserBudgetPlanAccessDA.Message == string.Empty)
            {
                foreach (DataRow m_drDUserBudgetPlanAccessDA in m_dicDUserBudgetPlanAccessDA[0].Tables[0].Rows )
                {
                    m_objUserBudgetPlanAccessVM = new UserBudgetPlanAccessVM();
                    m_objUserBudgetPlanAccessVM.UserID = m_drDUserBudgetPlanAccessDA[UserBudgetPlanAccessVM.Prop.UserID.Name].ToString();
                    m_objUserBudgetPlanAccessVM.BudgetPlanTemplateID = m_drDUserBudgetPlanAccessDA[UserBudgetPlanAccessVM.Prop.BudgetPlanTemplateID.Name].ToString();
                    m_objUserBudgetPlanAccessVM.StartDate = (DateTime)m_drDUserBudgetPlanAccessDA[UserBudgetPlanAccessVM.Prop.StartDate.Name];
                    m_objUserBudgetPlanAccessVM.EndDate = (DateTime)m_drDUserBudgetPlanAccessDA[UserBudgetPlanAccessVM.Prop.EndDate.Name];
                    lst_objUserBudgetPlanAccessVM.Add(m_objUserBudgetPlanAccessVM);
                }
              
            }

            return lst_objUserBudgetPlanAccessVM;
        }
        private List<UserVM> GetSelectedDataUser()
        {
            List<UserVM> lst_objUserVM = new List<UserVM>();
            UserVM m_objUserVM = new UserVM();
            MUserDA m_objMUserDA = new MUserDA();
            m_objMUserDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(UserVM.Prop.UserID.MapAlias);
            m_lstSelect.Add(UserVM.Prop.FullName.MapAlias);
            m_lstSelect.Add(UserVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(VendorVM.Prop.FirstName.MapAlias);
            m_lstSelect.Add(VendorVM.Prop.LastName.MapAlias);
            m_lstSelect.Add(UserVM.Prop.Password.MapAlias);
            m_lstSelect.Add(UserVM.Prop.LastLogin.MapAlias);
            m_lstSelect.Add(UserVM.Prop.HostIP.MapAlias);
            m_lstSelect.Add(UserVM.Prop.IsActive.MapAlias);
            m_lstSelect.Add(UserVM.Prop.EmployeeID.MapAlias);
            m_lstSelect.Add(UserVM.Prop.BusinessUnitID.MapAlias);
            m_lstSelect.Add(UserVM.Prop.DivisionID.MapAlias);
            m_lstSelect.Add(UserVM.Prop.ProjectID.MapAlias);
            m_lstSelect.Add(UserVM.Prop.ClusterID.MapAlias);
            m_lstSelect.Add(EmployeeVM.Prop.EmployeeName.MapAlias);
            m_lstSelect.Add(UserVM.Prop.BusinessUnitDesc.MapAlias);
            m_lstSelect.Add(UserVM.Prop.DivisionDesc.MapAlias);
            m_lstSelect.Add(UserVM.Prop.ProjectDesc.MapAlias);
            m_lstSelect.Add(UserVM.Prop.ClusterDesc.MapAlias);
            

            Dictionary<int, DataSet> m_dicMUserDA = m_objMUserDA.SelectBC(0, 1, false, m_lstSelect, null, null, null, null);
            if (m_objMUserDA.Message == string.Empty)
            {
                DataRow m_drMUserDA = m_dicMUserDA[0].Tables[0].Rows[0];
                m_objUserVM.UserID = m_drMUserDA[UserVM.Prop.UserID.Name].ToString();
                m_objUserVM.FullName = m_drMUserDA[UserVM.Prop.FullName.Name].ToString();
                m_objUserVM.VendorID = m_drMUserDA[UserVM.Prop.VendorID.Name].ToString();
                m_objUserVM.VendorDesc = m_drMUserDA[VendorVM.Prop.FirstName.Name].ToString() + " " + m_drMUserDA[VendorVM.Prop.LastName.Name].ToString();
                m_objUserVM.Password = string.Empty;
                m_objUserVM.LastLogin = DateTime.Parse(m_drMUserDA[UserVM.Prop.LastLogin.Name].ToString());
                m_objUserVM.HostIP = m_drMUserDA[UserVM.Prop.HostIP.Name].ToString();
                m_objUserVM.IsActive = Convert.ToBoolean(m_drMUserDA[UserVM.Prop.IsActive.Name].ToString());
                m_objUserVM.EmployeeID = m_drMUserDA[UserVM.Prop.EmployeeID.Name].ToString();
                m_objUserVM.BusinessUnitID = m_drMUserDA[UserVM.Prop.BusinessUnitID.Name].ToString();
                m_objUserVM.DivisionID = m_drMUserDA[UserVM.Prop.DivisionID.Name].ToString();
                m_objUserVM.ProjectID = m_drMUserDA[UserVM.Prop.ProjectID.Name].ToString();
                m_objUserVM.ClusterID = m_drMUserDA[UserVM.Prop.ClusterID.Name].ToString();
                m_objUserVM.EmployeeName = m_drMUserDA[UserVM.Prop.EmployeeName.Name].ToString();
                m_objUserVM.BusinessUnitDesc = m_drMUserDA[UserVM.Prop.BusinessUnitDesc.Name].ToString();
                m_objUserVM.DivisionDesc = m_drMUserDA[UserVM.Prop.DivisionDesc.Name].ToString();
                m_objUserVM.ProjectDesc = m_drMUserDA[UserVM.Prop.ProjectDesc.Name].ToString();
                m_objUserVM.ClusterDesc = m_drMUserDA[UserVM.Prop.ClusterDesc.Name].ToString();
                lst_objUserVM.Add(m_objUserVM);
            }

            return lst_objUserVM;
        }
        private List<BudgetPlanTemplateStructureVM> GetSelectedDataStructure(Dictionary<string, object> selected, ref string message, string BudgetPlantemplateID_, string Caller = "")
        {
            #region GetListConfigIsBOI
            List<string> m_lstUPABOI = new List<string>();
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

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("TRUE");
            m_objFilteru.Add(ConfigVM.Prop.Desc1.Map, m_lstFilter);

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
                m_lstUPABOI.Add(item.Key3);
            }
            #endregion
            #region GetListConfigIsAHS
            List<string> m_lstUPAAHS = new List<string>();
            m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;

            m_lstFilter = new List<object>();
            m_objFilteru = new Dictionary<string, List<object>>();

            m_lstSelectb = new List<string>();
            m_lstSelectb.Add(ConfigVM.Prop.Key3.MapAlias);

            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("ItemTypeID");
            m_objFilteru.Add(ConfigVM.Prop.Key1.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("BudgetPlanTemplate");
            m_objFilteru.Add(ConfigVM.Prop.Key2.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("FALSE");
            m_objFilteru.Add(ConfigVM.Prop.Desc1.Map, m_lstFilter);

            m_dicUConfigDA = m_objUConfigDA.SelectBC(0, null, false, m_lstSelectb, m_objFilteru, null, null, null);
            m_lstConfigVM = new List<ViewModels.ConfigVM>();
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
                m_lstUPAAHS.Add(item.Key3);
            }
            #endregion

            BudgetPlanTemplateStructureVM m_objMBudgetPlanTemplateStructureVM = new BudgetPlanTemplateStructureVM();
            DBudgetPlanTemplateStructureDA m_objMBudgetPlanTemplateStructureDA = new DBudgetPlanTemplateStructureDA();
            m_objMBudgetPlanTemplateStructureDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.Version.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.Sequence.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentSequence.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemGroupDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemTypeID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemTypeID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.IsDefault.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Key3.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc1.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            if (!string.IsNullOrEmpty(BudgetPlantemplateID_))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(BudgetPlantemplateID_);
                m_objFilter.Add(BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.Map, m_lstFilter);
            }
            else
            {
                foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
                {
                    if (m_objMBudgetPlanTemplateStructureVM.IsKey(m_kvpSelectedRow.Key) && m_kvpSelectedRow.Key == BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.Name)
                    {
                        m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                        m_lstFilter = new List<object>();
                        m_lstFilter.Add(Operator.Equals);
                        m_lstFilter.Add(m_kvpSelectedRow.Value);
                        m_objFilter.Add(BudgetPlanTemplateStructureVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                    }
                }
            }

            if (Caller != string.Empty)
            {
                string m_strIsDefault = "({0} IS NULL OR {0} = 1)"; //!=False
                if (Caller == General.EnumDesc(Buttons.ButtonUpdate))
                {
                    m_strIsDefault = "{0} IS NOT NULL"; //!=NULL
                }
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.None);
                m_lstFilter.Add(string.Empty);
                m_objFilter.Add(string.Format(m_strIsDefault, BudgetPlanTemplateStructureVM.Prop.IsDefault.Map), m_lstFilter);
            }

            List<BudgetPlanTemplateStructureVM> lst_budgetStructureVM = new List<BudgetPlanTemplateStructureVM>();
            Dictionary<int, DataSet> m_dicMBudgetPlanTemplateDA = m_objMBudgetPlanTemplateStructureDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMBudgetPlanTemplateStructureDA.Message == string.Empty)
            {
                foreach (DataRow m_dataRow in m_dicMBudgetPlanTemplateDA[0].Tables[0].Rows)
                {
                    BudgetPlanTemplateStructureVM m_objMBudgetPlanTemplateStructureVM2 = new BudgetPlanTemplateStructureVM();

                    m_objMBudgetPlanTemplateStructureVM2.ItemDesc = m_dataRow[BudgetPlanTemplateStructureVM.Prop.ItemDesc.Name].ToString();
                    m_objMBudgetPlanTemplateStructureVM2.BudgetPlanTemplateID = m_dataRow[BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.Name].ToString();
                    m_objMBudgetPlanTemplateStructureVM2.ItemID = m_dataRow[BudgetPlanTemplateStructureVM.Prop.ItemID.Name].ToString();
                    m_objMBudgetPlanTemplateStructureVM2.Version = Convert.ToInt16(m_dataRow[BudgetPlanTemplateStructureVM.Prop.Version.Name].ToString());
                    m_objMBudgetPlanTemplateStructureVM2.Sequence = Convert.ToInt16(m_dataRow[BudgetPlanTemplateStructureVM.Prop.Sequence.Name].ToString());
                    m_objMBudgetPlanTemplateStructureVM2.ParentItemID = m_dataRow[BudgetPlanTemplateStructureVM.Prop.ParentItemID.Name].ToString();
                    m_objMBudgetPlanTemplateStructureVM2.ParentSequence = Convert.ToInt16(m_dataRow[BudgetPlanTemplateStructureVM.Prop.ParentSequence.Name].ToString());
                    m_objMBudgetPlanTemplateStructureVM2.ParentVersion = Convert.ToInt16(m_dataRow[BudgetPlanTemplateStructureVM.Prop.ParentVersion.Name].ToString());
                    m_objMBudgetPlanTemplateStructureVM2.ItemGroupDesc = m_dataRow[BudgetPlanTemplateStructureVM.Prop.ItemGroupDesc.Name].ToString();
                    m_objMBudgetPlanTemplateStructureVM2.ItemTypeID = m_dataRow[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString();
                    m_objMBudgetPlanTemplateStructureVM2.ParentItemTypeID = m_dataRow[BudgetPlanTemplateStructureVM.Prop.ParentItemTypeID.Name].ToString();
                    m_objMBudgetPlanTemplateStructureVM2.IsBOI = m_dataRow[ConfigVM.Prop.Key3.Name].ToString() == m_dataRow[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString()
                            && m_dataRow[ConfigVM.Prop.Desc1.Name].ToString() == "TRUE";
                    m_objMBudgetPlanTemplateStructureVM2.IsAHS = m_dataRow[ConfigVM.Prop.Key3.Name].ToString() == m_dataRow[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString()
                            && m_dataRow[ConfigVM.Prop.Desc1.Name].ToString() == "FALSE";
                    m_objMBudgetPlanTemplateStructureVM2.HasChild = m_objMBudgetPlanTemplateStructureVM2.IsBOI;

                    if (!string.IsNullOrEmpty(m_dataRow[BudgetPlanTemplateStructureVM.Prop.IsDefault.Name].ToString()))
                    {
                        m_objMBudgetPlanTemplateStructureVM2.EnableDefault = true;
                        m_objMBudgetPlanTemplateStructureVM2.IsDefault = Convert.ToBoolean(m_dataRow[BudgetPlanTemplateStructureVM.Prop.IsDefault.Name].ToString());
                    }
                    else
                    {
                        m_objMBudgetPlanTemplateStructureVM2.IsDefault = null;
                        m_objMBudgetPlanTemplateStructureVM2.EnableDefault = false;
                    }
                    //m_objMBudgetPlanTemplateStructureVM2.EnableDefault = false;

                    lst_budgetStructureVM.Add(m_objMBudgetPlanTemplateStructureVM2);
                }
            }
            //else
            // message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMBudgetPlanTemplateStructureDA.Message;
            lst_budgetStructureVM = lst_budgetStructureVM.AsEnumerable().OrderBy(x => x.Sequence).ToList();


            return lst_budgetStructureVM;
        }
        private bool isBOI(string item, List<string> lst_BOI)
        {
            bool ret = false;
            foreach (string i in lst_BOI)
            {
                if (i == item)
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }
        private bool isAHS(string item, List<string> lst_AHS)
        {
            bool ret = false;
            foreach (string i in lst_AHS)
            {
                if (i == item)
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        private bool SaveUserAccess(string ParamName, string BudgetPlanTemplateID)
        {
            bool IsSuccess = true;

            if (this.Request.Params[ParamName] != null)
            {
                DUserBudgetPlanAccessDA m_objDUserBudgetPlanAccessDA = new DUserBudgetPlanAccessDA();
                DUserBudgetPlanAccess m_objDUserBudgetPlanAccess = new DUserBudgetPlanAccess();
                m_objDUserBudgetPlanAccessDA.ConnectionStringName = Global.ConnStrConfigName;

                Dictionary<string, object>[] m_arrListUserAccess = JSON.Deserialize<Dictionary<string, object>[]>(this.Request.Params[ParamName]);

                foreach (Dictionary<string, object> m_dicUserAccess in m_arrListUserAccess)
                {
                    if (m_dicUserAccess.Count < 5)
                        m_dicUserAccess.Add(UserBudgetPlanAccessVM.Prop.BudgetPlanTemplateID.Name, BudgetPlanTemplateID);

                    m_objDUserBudgetPlanAccess.UserID = m_dicUserAccess[UserBudgetPlanAccessVM.Prop.UserID.Name].ToString();
                    m_objDUserBudgetPlanAccess.BudgetPlanTemplateID = m_dicUserAccess[UserBudgetPlanAccessVM.Prop.BudgetPlanTemplateID.Name].ToString();
                    m_objDUserBudgetPlanAccess.StartDate = (DateTime)m_dicUserAccess[UserBudgetPlanAccessVM.Prop.StartDate.Name]; ;
                    m_objDUserBudgetPlanAccess.EndDate = (DateTime)m_dicUserAccess[UserBudgetPlanAccessVM.Prop.EndDate.Name]; ;
                    m_objDUserBudgetPlanAccessDA.Data = m_objDUserBudgetPlanAccess;

                    if (!IsSaveUserAccesValid(m_dicUserAccess[UserBudgetPlanAccessVM.Prop.BudgetPlanTemplateID.Name].ToString(), m_dicUserAccess[UserBudgetPlanAccessVM.Prop.UserID.Name].ToString()))
                    {
                        m_objDUserBudgetPlanAccessDA.Insert(false);
                    }
                    else
                    {
                        m_objDUserBudgetPlanAccessDA.Update(false);
                    }
                }

                Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                List<string> lst_userId = new List<string>();

                List<object> m_lstSet = new List<object>()
                {
                    typeof(DateTime),
                    DateTime.Now.AddDays(-1)
                };
                Dictionary<string, List<object>> m_objSet = new Dictionary<string, List<object>>();
                m_objSet.Add(UserBudgetPlanAccessVM.Prop.EndDate.Map, m_lstSet);

                foreach (Dictionary<string, object> m_dicUserAccess in m_arrListUserAccess)
                {
                    
                    lst_userId.Add(m_dicUserAccess[UserBudgetPlanAccessVM.Prop.UserID.Name].ToString());
                }



                List<object> m_lstFilters = new List<object>();
                m_lstFilters.Add(Operator.None);
                m_lstFilters.Add(string.Empty);
                m_objFilter.Add(string.Format("{0} NOT IN ('{1}') AND {2} = '" + BudgetPlanTemplateID + "'", UserBudgetPlanAccessVM.Prop.UserID.Map, string.Join("','", lst_userId), UserBudgetPlanAccessVM.Prop.BudgetPlanTemplateID.Map), m_lstFilters);
                m_objDUserBudgetPlanAccessDA.UpdateBC(m_objSet, m_objFilter, false);
            }
            return IsSuccess;
        }

        private bool IsSaveUserAccesValid(string BudgetPlanTemplateID, string UserID)
        {
            bool isValid = true;

            List<UserBudgetPlanAccessVM> lst_objUserBudgetPlanAccessVM = new List<UserBudgetPlanAccessVM>();
            DUserBudgetPlanAccessDA m_objUserBudgetPlanAccessDA = new DUserBudgetPlanAccessDA();

            m_objUserBudgetPlanAccessDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(UserBudgetPlanAccessVM.Prop.BudgetPlanTemplateID.MapAlias);

            List<object> m_lstFilter;
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanTemplateID);
            m_objFilter.Add(UserBudgetPlanAccessVM.Prop.BudgetPlanTemplateID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(UserID);
            m_objFilter.Add(UserBudgetPlanAccessVM.Prop.UserID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicDUserBudgetPlanAccessDA = m_objUserBudgetPlanAccessDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);

            if (!string.IsNullOrEmpty(m_objUserBudgetPlanAccessDA.Message))
                isValid = false;

            return isValid;
        }
        #endregion

        #region Public Method
        public Node GetNodeBudgetPlanTemplate(List<BudgetPlanTemplateStructureVM> LstModel)
        {
            #region GetListConfigIsBOI
            List<string> m_lstUPABOI = new List<string>();
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

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("TRUE");
            m_objFilteru.Add(ConfigVM.Prop.Desc1.Map, m_lstFilter);

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
                m_lstUPABOI.Add(item.Key3);
            }
            #endregion
            #region GetListConfigIsAHS
            List<string> m_lstUPAAHS = new List<string>();
            m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;

            m_lstFilter = new List<object>();
            m_objFilteru = new Dictionary<string, List<object>>();

            m_lstSelectb = new List<string>();
            m_lstSelectb.Add(ConfigVM.Prop.Key3.MapAlias);

            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("ItemTypeID");
            m_objFilteru.Add(ConfigVM.Prop.Key1.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("BudgetPlanTemplate");
            m_objFilteru.Add(ConfigVM.Prop.Key2.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("FALSE");
            m_objFilteru.Add(ConfigVM.Prop.Desc1.Map, m_lstFilter);

            m_dicUConfigDA = m_objUConfigDA.SelectBC(0, null, false, m_lstSelectb, m_objFilteru, null, null, null);
            m_lstConfigVM = new List<ViewModels.ConfigVM>();
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
                m_lstUPAAHS.Add(item.Key3);
            }
            #endregion

            Node getNode = new Node();
            getNode.NodeID = "root";

            getNode.AttributesObject = new
            {
                itemdesc = "BudgetPlan",
                haschild = true,
                enabledefault = false
            };
            NodeCollection m_nodeCollection = new NodeCollection();
            List<BudgetPlanTemplateStructureVM> LstModelparent = new List<BudgetPlanTemplateStructureVM>();
            LstModelparent = LstModel.AsEnumerable()
                .Where(x => x.ParentItemID == "0")
                .OrderBy(x => x.Sequence).ToList();

            if (LstModelparent.Count > 0)
            {
                foreach (BudgetPlanTemplateStructureVM lstModel in LstModelparent)
                {
                    Node m_node = new Node();
                    bool allowdelete = lstModel.IsBOI;

                    #region CheckChildren
                    m_node = GetNodeRecursive(m_node, LstModel, lstModel.ItemID, lstModel.Version, lstModel.Sequence, m_lstUPAAHS, m_lstUPABOI);
                    #endregion
                    if (!lstModel.IsBOI && !lstModel.IsAHS)
                    {
                        m_node.Icon = Icon.PageWhite;
                    }
                    else
                        m_node.Icon = lstModel.IsBOI ? Icon.Folder : Icon.Table;


                    if (m_node.Children.Count > 0)
                    {
                        m_node.Expandable = true;
                        m_node.Expanded = false;
                    }
                    else
                    {
                        if (lstModel.IsAHS)
                        {
                            int lastSequence = 0;
                            m_node = CheckChildrenFirstLoad(lstModel, ref lastSequence, m_node, m_lstUPAAHS, m_lstUPABOI);
                        }
                        else
                        {
                            m_node.Expandable = false;
                            m_node.Expanded = false;
                        }
                    }

                    m_node.AttributesObject = new
                    {
                        number = "",
                        itemdesc = lstModel.ItemDesc,
                        budgetplantemplateid = lstModel.BudgetPlanTemplateID,
                        itemid = lstModel.ItemID,
                        version = lstModel.Version,
                        sequence = lstModel.Sequence,
                        parentitemid = lstModel.ParentItemID,
                        parentversion = lstModel.ParentVersion,
                        parentsequence = lstModel.ParentSequence,
                        allowdelete = allowdelete,
                        isBOI = lstModel.IsBOI,
                        isHAS = lstModel.IsAHS,
                        haschild = lstModel.HasChild,
                        itemgroupdesc = lstModel.ItemGroupDesc,
                        itemtypeid = lstModel.ItemTypeID,
                        parentitemtypeid = lstModel.ParentItemTypeID,
                        isdefault = lstModel.IsDefault,
                        enabledefault = lstModel.EnableDefault
                    };

                    m_nodeCollection.Add(m_node);
                }
                getNode.Children.AddRange(m_nodeCollection);
                getNode.Expandable = true;
                getNode.Expanded = true;

            }
            else
            {
                getNode.Expandable = false;
                getNode.Expanded = false;
            }
            return getNode;
        }

        private Node GetNodeRecursive(Node ParentNode, List<BudgetPlanTemplateStructureVM> lst_budgetStructure, string ParentItemID, int ParentVersion, int ParentSequence, List<string> m_lstUPAAHS, List<string> m_lstUPABOI)
        {
            List<BudgetPlanTemplateStructureVM> m_lstLevelFinancialStmtItemHierarchyVM = new List<BudgetPlanTemplateStructureVM>();
            m_lstLevelFinancialStmtItemHierarchyVM = lst_budgetStructure.AsEnumerable()
                                                                .Where(x => x.ParentItemID == ParentItemID)
                                                                .Where(x => x.ParentVersion == ParentVersion)
                                                                .Where(x => x.ParentSequence == ParentSequence)
                                                                .OrderBy(x => x.Sequence)
                                                                .ToList();

            if (m_lstLevelFinancialStmtItemHierarchyVM.Count > 0)
            {

                foreach (BudgetPlanTemplateStructureVM listP in m_lstLevelFinancialStmtItemHierarchyVM)
                {
                    Node m_node = new Node();
                    bool allowDelete = listP.IsBOI;
                    if (!listP.IsBOI && !listP.IsAHS)
                    {
                        m_node.Icon = Icon.PageWhite;
                    }
                    else
                        m_node.Icon = listP.IsBOI ? Icon.Folder : Icon.Table;


                    m_node.Text = listP.BudgetPlanTemplateID;
                    m_node = GetNodeRecursive(m_node, lst_budgetStructure, listP.ItemID, listP.Version, listP.Sequence, m_lstUPAAHS, m_lstUPABOI);
                    if (m_node.Children.Count > 0)
                        m_node.Expanded = true;
                    else
                    {
                        if (listP.IsAHS)
                        {
                            int seq = listP.Sequence;
                            m_node = CheckChildrenFirstLoad(listP, ref seq, m_node, m_lstUPAAHS, m_lstUPABOI);
                        }
                        else
                        {
                            m_node.Expandable = false;
                            m_node.Expanded = false;
                        }
                    }
                    m_node.AttributesObject = new
                    {
                        itemdesc = listP.ItemDesc,
                        budgetplantemplateid = listP.BudgetPlanTemplateID,
                        itemid = listP.ItemID,
                        version = listP.Version,
                        sequence = listP.Sequence,
                        parentitemid = listP.ParentItemID,
                        parentversion = listP.ParentVersion,
                        parentsequence = listP.ParentSequence,
                        allowdelete = allowDelete,
                        haschild = listP.HasChild,
                        isBOI = listP.IsBOI,
                        isAHS = listP.IsAHS,
                        itemgroupdesc = listP.ItemGroupDesc,
                        itemtypeid = listP.ItemTypeID,
                        parentitemtypeid = listP.ParentItemTypeID,
                        isdefault = listP.IsDefault,
                        enabledefault = listP.EnableDefault//,enabledefault = setEnableDefault
                    };

                    ParentNode.Children.Add(m_node);
                }
            }
            return ParentNode;
        }

        private Node GetMoreChildStructure(BudgetPlanTemplateStructureVM listP, string ID)
        {
            //string budgetplantmplID = this.Request.Params[BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.Name];
            List<BudgetPlanTemplateStructureVM> lst_append = new List<BudgetPlanTemplateStructureVM>();
            ID = "_" + ID;
            Node node = new Node();
            node = ListToAppend(listP.ItemDesc, listP.ItemID, "", listP.Version, listP.ParentItemID, listP.ParentVersion, listP.ParentItemTypeID, listP.ParentSequence, lst_append, listP.BudgetPlanTemplateID, listP.ParentSequence, ref ID, listP.EnableDefault);


            //node = ListToAppend(listP.ItemID, "", listP.Version, listP.ParentItemID, listP.ParentVersion, listP.ParentItemTypeID, listP.ParentSequence, lst_append, listP.BudgetPlanTemplateID, listP.ParentSequence, ref ID, node);

            return node;


        }
        public ActionResult GetNodeAppendNew(string thisItemDesc, string thisItem, string thisItemTypeId, string ParentItem, int thisVersion, string ParentItemTypeID, int ParentVersion, int ParentSequence, int Sequence, string BudgetTemplateID, bool isdefaultCheck)
        {

            Node m_node = new Node();

            bool isdefaultCheckval = false;
            bool isEnableDefault = false;
            #region get allowed child list (BOI)
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

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("TRUE");
            m_objFilteru.Add(ConfigVM.Prop.Desc1.Map, m_lstFilter);

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
            #region get not allowed child list (AHS)
            List<string> m_lstUPAAHS = new List<string>();
            m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;

            m_lstFilter = new List<object>();
            m_objFilteru = new Dictionary<string, List<object>>();

            m_lstSelectb = new List<string>();
            m_lstSelectb.Add(ConfigVM.Prop.Key3.MapAlias);

            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("ItemTypeID");
            m_objFilteru.Add(ConfigVM.Prop.Key1.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("BudgetPlanTemplate");
            m_objFilteru.Add(ConfigVM.Prop.Key2.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("FALSE");
            m_objFilteru.Add(ConfigVM.Prop.Desc1.Map, m_lstFilter);

            m_dicUConfigDA = m_objUConfigDA.SelectBC(0, null, false, m_lstSelectb, m_objFilteru, null, null, null);
            m_lstConfigVM = new List<ViewModels.ConfigVM>();
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
                m_lstUPAAHS.Add(item.Key3);
            }
            #endregion

            BudgetPlanTemplateStructureVM m_objParentBudgetTemplateStructure = new BudgetPlanTemplateStructureVM();
            Dictionary<string, List<object>> m_objFiltersFirst = new Dictionary<string, List<object>>();
            DItemVersionDA m_objVersion = new DItemVersionDA();
            m_objVersion.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstSelectsFirst = new List<string>();
            m_lstSelectsFirst.Add(ItemVersionVM.Prop.ItemDesc.Map);
            m_lstSelectsFirst.Add(ItemVersionVM.Prop.ItemID.Map);
            m_lstSelectsFirst.Add(ItemVersionVM.Prop.Version.Map);
            m_lstSelectsFirst.Add(ItemVersionVM.Prop.ItemTypeID.Map);
            m_lstSelectsFirst.Add(ItemVersionVM.Prop.ItemGroupDesc.Map);

            List<object> m_lstFilterss = new List<object>();
            m_lstFilterss.Add(Operator.Equals);
            m_lstFilterss.Add(thisItem);
            m_objFiltersFirst.Add(ItemVersionVM.Prop.ItemID.Map, m_lstFilterss);

            List<object> m_lstFiltersv = new List<object>();
            m_lstFiltersv.Add(Operator.Equals);
            m_lstFiltersv.Add(thisVersion);
            m_objFiltersFirst.Add(ItemVersionVM.Prop.Version.Map, m_lstFiltersv);

            Dictionary<int, DataSet> m_dicMBudgetPlanTemplateDA = m_objVersion.SelectBC(0, null, false, m_lstSelectsFirst, m_objFiltersFirst, null, null, null, null);
            if (m_objVersion.Message == string.Empty)
            {
                DataRow m_dataRow = m_dicMBudgetPlanTemplateDA[0].Tables[0].Rows[0];

                ItemVersionVM m_objitemVersion = new ItemVersionVM();
                m_objitemVersion.ItemDesc = m_dataRow[ItemVersionVM.Prop.ItemDesc.Name].ToString();
                m_objitemVersion.ItemID = m_dataRow[ItemVersionVM.Prop.ItemID.Name].ToString();
                m_objitemVersion.Version = Convert.ToInt16(m_dataRow[ItemVersionVM.Prop.Version.Name].ToString());
                m_objitemVersion.ItemTypeID = m_dataRow[ItemVersionVM.Prop.ItemTypeID.Name].ToString();
                m_objitemVersion.ItemGroupDesc = m_dataRow[ItemVersionVM.Prop.ItemGroupDesc.Name].ToString();

                m_objParentBudgetTemplateStructure.IsAHS = false;
                m_objParentBudgetTemplateStructure.IsBOI = false;

                foreach (string match in m_lstUPA)
                {
                    if (match == m_objitemVersion.ItemTypeID)
                    {
                        m_objParentBudgetTemplateStructure.HasChild = true;
                        m_objParentBudgetTemplateStructure.AllowDelete = true;
                        m_objParentBudgetTemplateStructure.IsDefault = null;
                        m_objParentBudgetTemplateStructure.IsBOI = true;
                        m_objParentBudgetTemplateStructure.IsAHS = false;
                        break;

                    }
                    else
                    {
                        m_objParentBudgetTemplateStructure.HasChild = false;
                        isEnableDefault = true;
                        m_objParentBudgetTemplateStructure.AllowDelete = false;
                        m_objParentBudgetTemplateStructure.IsDefault = false;
                    }
                }
                foreach (string keyAHS in m_lstUPAAHS)
                {
                    if (keyAHS == m_objitemVersion.ItemTypeID)
                    {
                        m_objParentBudgetTemplateStructure.IsAHS = true;
                        if (isdefaultCheck && isEnableDefault)
                        {
                            m_objParentBudgetTemplateStructure.IsDefault = true;
                            isdefaultCheckval = true;
                            isdefaultCheck = false;
                        }

                        break;
                    }
                }

                m_objParentBudgetTemplateStructure.ItemDesc = m_objitemVersion.ItemDesc;
                m_objParentBudgetTemplateStructure.ItemTypeID = m_objitemVersion.ItemTypeID;
                m_objParentBudgetTemplateStructure.ItemGroupDesc = m_objitemVersion.ItemGroupDesc;
                m_objParentBudgetTemplateStructure.ItemID = m_objitemVersion.ItemID;
                m_objParentBudgetTemplateStructure.Version = m_objitemVersion.Version;
                m_objParentBudgetTemplateStructure.BudgetPlanTemplateID = BudgetTemplateID;
                m_objParentBudgetTemplateStructure.Sequence = Sequence + 1;
                m_objParentBudgetTemplateStructure.ParentItemID = ParentItem;
                m_objParentBudgetTemplateStructure.ParentVersion = ParentVersion;
                m_objParentBudgetTemplateStructure.ParentSequence = ParentSequence;
                m_objParentBudgetTemplateStructure.ParentItemTypeID = ParentItemTypeID;


                m_objParentBudgetTemplateStructure.EnableDefault = isEnableDefault;
                Sequence++;
                m_node = createNodeVersion(m_objParentBudgetTemplateStructure, false, isdefaultCheckval);

                #region CheckChildren
                //CheckChildren
                m_node = CheckChildren(m_objParentBudgetTemplateStructure.ItemID, m_objParentBudgetTemplateStructure.ItemTypeID, m_objParentBudgetTemplateStructure.Version, ref Sequence, BudgetTemplateID, m_node, m_lstUPA, m_lstUPAAHS, ref isEnableDefault, ref isdefaultCheck);
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


        public ActionResult GetNodeAppendOld(string itemdesc, string thisItem, string thisItemTypeId, string ParentItem, int thisVersion, string ParentItemTypeID, int ParentVersion, int ParentSequence, int Sequence, string BudgetTemplateID, string idNode)
        {

            string budgetplantmplID = this.Request.Params[BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.Name];
            List<BudgetPlanTemplateStructureVM> lst_append = new List<BudgetPlanTemplateStructureVM>();
            Node data = new Node();

            data = ListToAppend(itemdesc, thisItem, thisItemTypeId, thisVersion, ParentItem, ParentVersion, ParentItemTypeID, ParentSequence, lst_append, BudgetTemplateID, Sequence, ref idNode, false);
            if (data != null)
            {

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

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add("FALSE");
                m_objFilteru.Add(ConfigVM.Prop.Desc1.Map, m_lstFilter);

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
                    m_lstUPA.Add(item.Key3);


                data.Expanded = true;
            }
            return this.Store(data);

        }
        public Dictionary<int, List<BudgetPlanTemplateVM>> GetBudgetPlanTemplateData(bool isCount, string BudgetPlanTemplateID, string BudgetPlanTemplateDesc)
        {
            int m_intCount = 0;
            List<BudgetPlanTemplateVM> m_lstBudgetPlanTemplateVM = new List<ViewModels.BudgetPlanTemplateVM>();
            Dictionary<int, List<BudgetPlanTemplateVM>> m_dicReturn = new Dictionary<int, List<BudgetPlanTemplateVM>>();
            MBudgetPlanTemplateDA m_objMBudgetPlanTemplateDA = new MBudgetPlanTemplateDA();
            m_objMBudgetPlanTemplateDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanTemplateVM.Prop.BudgetPlanTemplateID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateVM.Prop.BudgetPlanTemplateDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateVM.Prop.BudgetPlanTypeDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(BudgetPlanTemplateID);
            m_objFilter.Add(BudgetPlanTemplateVM.Prop.BudgetPlanTemplateID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(BudgetPlanTemplateDesc);
            m_objFilter.Add(BudgetPlanTemplateVM.Prop.BudgetPlanTemplateDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMBudgetPlanTemplateDA = m_objMBudgetPlanTemplateDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMBudgetPlanTemplateDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpBudgetPlanTemplateBL in m_dicMBudgetPlanTemplateDA)
                    {
                        m_intCount = m_kvpBudgetPlanTemplateBL.Key;
                        break;
                    }
                else
                {
                    m_lstBudgetPlanTemplateVM = (
                        from DataRow m_drMBudgetPlanTemplateDA in m_dicMBudgetPlanTemplateDA[0].Tables[0].Rows
                        select new BudgetPlanTemplateVM()
                        {
                            BudgetPlanTemplateID = m_drMBudgetPlanTemplateDA[BudgetPlanTemplateVM.Prop.BudgetPlanTemplateID.Name].ToString(),
                            BudgetPlanTemplateDesc = m_drMBudgetPlanTemplateDA[BudgetPlanTemplateVM.Prop.BudgetPlanTemplateDesc.Name].ToString(),
                            BudgetPlanTypeDesc = m_drMBudgetPlanTemplateDA[BudgetPlanTemplateVM.Prop.BudgetPlanTypeDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstBudgetPlanTemplateVM);
            return m_dicReturn;
        }

        public NodeCollection GetNodeTemplateStructure(List<BudgetPlanTemplateStructureVM> listBudgetPlanTemplateStructureVM)
        {

            NodeCollection m_nodeCollectChild = new NodeCollection();
            
            List<BudgetPlanTemplateStructureVM> lstBudgetPlanTemplateStructureVM = new List<BudgetPlanTemplateStructureVM>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();


            lstBudgetPlanTemplateStructureVM = listBudgetPlanTemplateStructureVM.Where(d => d.ParentItemID == "0").ToList();

            //SiteMapNode m_siteMapNode = SiteMap.RootNode;

            foreach (BudgetPlanTemplateStructureVM item in lstBudgetPlanTemplateStructureVM)
            {
                string m_strSpecification = string.Empty;

                Node node = new Node();
                NodeCollection nodeChildCollection = LoadChildBPTemplateStructure(new BudgetPlanTemplateStructureVM()
                {
                    BudgetPlanTemplateID = item.BudgetPlanTemplateID,
                    ItemID = item.ItemID,
                    Version = item.Version,
                    Sequence = item.Sequence,
                    ParentItemID = item.ParentItemID,
                    ParentVersion = item.ParentVersion,
                    ParentSequence = item.ParentSequence
                }, listBudgetPlanTemplateStructureVM);
                

                node = new Node()
                {
                    NodeID= Guid.NewGuid().ToString() ,
                    Text = item.ItemDesc,
                    Expanded = nodeChildCollection.Count > 0,
                    Expandable = nodeChildCollection.Count > 0,
                    Leaf = nodeChildCollection.Count > 0 ? false : true,
                    AttributesObject = new
                    {
                        itemdesc = item.ItemDesc,
                        budgetplantemplateid = item.BudgetPlanTemplateID,
                        itemid = item.ItemID,
                        version = item.Version,
                        sequence = item.Sequence,
                        parentitemid = item.ParentItemID,
                        parentversion = item.ParentVersion,
                        parentsequence = item.ParentSequence,
                        haschild = nodeChildCollection.Any(),
                        itemtypeid = item.ItemTypeID,
                        parentitemtypeid = item.ItemTypeID,
                        isdefault = item.IsDefault,
                        uomdesc = item.UoMDesc,
                        isboi = item.IsBOI,
                        isahs = item.IsAHS,
                        materialamount = (item.MaterialAmount == 0 ? null : item.MaterialAmount),
                        wageamount = (item.WageAmount == 0 ? null : item.WageAmount),
                        miscamount = (item.MiscAmount == 0 ? null : item.MiscAmount),
                        leaf = nodeChildCollection.Count > 0 ? false : true,
                        specification = (m_strSpecification.Length > 0 ? m_strSpecification.Substring(0, m_strSpecification.Length - 2) : ""),
                        uomid = item.UoMID,
                        total = 0,
                        //haschild = item.HasChild,
                        itemgroupdesc = item.ItemGroupDesc,
                        enabledefault = item.EnableDefault
                    },
                    Icon = Icon.Folder
                };
                node.Children.AddRange(nodeChildCollection);
                m_nodeCollectChild.Add(node);
            }

            return m_nodeCollectChild;

        }
        public NodeCollection LoadChildBPTemplateStructure(BudgetPlanTemplateStructureVM dataParent, List<BudgetPlanTemplateStructureVM> listBudgetPlanTemplateStructureVM)
        {
            NodeCollection nodeCollection = new NodeCollection();
            //Dictionary<string, bool> m_dicDisplayPrice = DisplayPrice();

            BudgetPlanTemplateStructureVM m_objDBudgetPlanTemplateStructureVM = new BudgetPlanTemplateStructureVM();
            DBudgetPlanTemplateStructureDA m_objDBudgetPlanTemplateStructureDA = new DBudgetPlanTemplateStructureDA();
            m_objDBudgetPlanTemplateStructureDA.ConnectionStringName = Global.ConnStrConfigName;

            List<BudgetPlanTemplateStructureVM> lstBudgetPlanTemplateStructureVM = new List<BudgetPlanTemplateStructureVM>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            lstBudgetPlanTemplateStructureVM = listBudgetPlanTemplateStructureVM.Where(
                d=>d.ParentItemID.Equals(dataParent.ItemID) &&
                d.ParentVersion.Equals(dataParent.Version) &&
                d.ParentSequence.Equals(dataParent.Sequence)
                ).ToList();
            
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
                }, listBudgetPlanTemplateStructureVM);

                

                node = new Node();
                node.NodeID = Guid.NewGuid().ToString();
                node.Text = item.ItemDesc;
                node.Expanded = nodeCollection_.Count > 0;
                node.Expandable = nodeCollection_.Count > 0;
                node.Leaf = nodeCollection_.Count > 0 ? false : true;
                

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
                    //haschild = nodeCollection_.Any(),
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
                    itemgroupdesc = item.ItemGroupDesc,
                    enabledefault = item.EnableDefault
                };

                node.Children.AddRange(nodeCollection_);
                nodeCollection.Add(node);
            }


            return nodeCollection;

        }

        #endregion
    }
}