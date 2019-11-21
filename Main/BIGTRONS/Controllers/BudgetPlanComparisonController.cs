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
using XMVC = Ext.Net.MVC;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using System.IO;
using NPOI.SS.UserModel;
namespace com.SML.BIGTRONS.Controllers
{
    public class BudgetPlanComparisonController : BaseController
    {
        private readonly string title = "Budget Plan Comparison";
        private readonly string dataSessionName = "FormData";
        #region Public Property
        #endregion

        #region Public Action

        public ActionResult Index()
        {
            base.Initialize();
            return View();
        }
        public ActionResult List()
        {
            Global.HasAccess = GetHasAccess();
            //if (!Global.HasAccess.Read)
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

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
        public ActionResult Browse(string ControlBudgetPlanID, string ControlDescription, string ControlBudgetPlanTypeID = "",
            string ControlBudgetPlanTypeDesc = "", string ControlStatusID = "", string ControlStatusDesc = "",
            string ControlBudgetPlanVersion = "",
             string ControlBudgetPlanList = "", string FilterStatusID = "",
             string FilterBudgetPlanID = "", string FilterBudgetPlanDesc = "",
             string FilterProjectID = "")
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddBudgetPlan = new ViewDataDictionary();
            m_vddBudgetPlan.Add("Control" + BudgetPlanVM.Prop.BudgetPlanID.Name, ControlBudgetPlanID);
            m_vddBudgetPlan.Add("Control" + BudgetPlanVM.Prop.Description.Name, ControlDescription);
            m_vddBudgetPlan.Add("Control" + BudgetPlanVM.Prop.BudgetPlanTypeDesc.Name, ControlBudgetPlanTypeDesc);
            m_vddBudgetPlan.Add("Control" + BudgetPlanVM.Prop.BudgetPlanTypeID.Name, ControlBudgetPlanTypeID);
            m_vddBudgetPlan.Add("Control" + BudgetPlanVM.Prop.StatusDesc.Name, ControlStatusDesc);
            m_vddBudgetPlan.Add("Control" + BudgetPlanVM.Prop.StatusID.Name, ControlStatusID);
            m_vddBudgetPlan.Add("Control" + BudgetPlanVM.Prop.BudgetPlanVersion.Name, ControlBudgetPlanVersion);
            m_vddBudgetPlan.Add("ControlBudgetPlanList", ControlBudgetPlanList);
            m_vddBudgetPlan.Add(BudgetPlanVM.Prop.BudgetPlanID.Name, FilterBudgetPlanID);
            m_vddBudgetPlan.Add(BudgetPlanVM.Prop.Description.Name, FilterBudgetPlanDesc);
            m_vddBudgetPlan.Add(BudgetPlanVM.Prop.StatusID.Name, FilterStatusID);
            m_vddBudgetPlan.Add("Value" + BudgetPlanVM.Prop.StatusID.Name, FilterStatusID);
            m_vddBudgetPlan.Add("Value" + BudgetPlanVM.Prop.ProjectID.Name, FilterProjectID);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddBudgetPlan,
                ViewName = "../BudgetPlan/_Browse"
            };
        }
        public ActionResult ReadBrowse(StoreRequestParameters parameters, string PackageStatusID = "", string PackageProjectID = "")
        {
            TBudgetPlanDA m_objTBudgetPlanDA = new TBudgetPlanDA();
            m_objTBudgetPlanDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMBudgetPlan = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMBudgetPlan.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = BudgetPlanVM.Prop.Map(m_strDataIndex, false);
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

            if (PackageStatusID.Length > 0)
            {
                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(int.Parse(PackageStatusID));
                m_objFilter.Add(BudgetPlanVM.Prop.StatusID.Map, m_lstFilter);
            }

            if (PackageProjectID.Length > 0)
            {
                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(PackageProjectID);
                m_objFilter.Add(BudgetPlanVM.Prop.ProjectID.Map, m_lstFilter);
            }

            Dictionary<int, DataSet> m_dicMBudgetPlanDA = m_objTBudgetPlanDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpBudgetPlanBL in m_dicMBudgetPlanDA)
            {
                m_intCount = m_kvpBudgetPlanBL.Key;
                break;
            }

            List<BudgetPlanVM> m_lstBudgetPlanVM = new List<BudgetPlanVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanID.MapAlias);
                m_lstSelect.Add(BudgetPlanVM.Prop.Description.MapAlias);
                m_lstSelect.Add(BudgetPlanVM.Prop.StatusDesc.MapAlias);
                m_lstSelect.Add(BudgetPlanVM.Prop.StatusID.MapAlias);
                m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanTypeDesc.MapAlias);
                m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanTypeID.MapAlias);
                //m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanVersion.MapAlias);
                m_lstSelect.Add("MAX(DBudgetPlanVersion.BudgetPlanVersion) OVER(PARTITION BY TBudgetPlan.BudgetPlanID) AS BudgetPlanVersion");


                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(BudgetPlanVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));


                m_dicMBudgetPlanDA = m_objTBudgetPlanDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objTBudgetPlanDA.Message == string.Empty)
                {
                    m_lstBudgetPlanVM = (
                        from DataRow m_drMBudgetPlanDA in m_dicMBudgetPlanDA[0].Tables[0].Rows
                        select new BudgetPlanVM()
                        {
                            BudgetPlanID = m_drMBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanID.Name].ToString(),
                            Description = m_drMBudgetPlanDA[BudgetPlanVM.Prop.Description.Name].ToString(),
                            StatusDesc = m_drMBudgetPlanDA[BudgetPlanVM.Prop.StatusDesc.Name].ToString(),
                            StatusID = int.Parse(m_drMBudgetPlanDA[BudgetPlanVM.Prop.StatusID.Name].ToString()),
                            BudgetPlanTypeID = m_drMBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanTypeID.Name].ToString(),
                            BudgetPlanTypeDesc = m_drMBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanTypeDesc.Name].ToString(),
                            BudgetPlanVersion = int.Parse(m_drMBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanVersion.Name].ToString())
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstBudgetPlanVM, m_intCount);
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            TBudgetPlanDA m_objTBudgetPlanDA = new TBudgetPlanDA();
            m_objTBudgetPlanDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcTBudgetPlan = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcTBudgetPlan.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = BudgetPlanVM.Prop.Map(m_strDataIndex, false);
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
            /*Dictionary<int, DataSet> m_dicTBudgetPlanDA = m_objTBudgetPlanDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpItemBL in m_dicTBudgetPlanDA)
            {
                m_intCount = m_kvpItemBL.Key;
                break;
            }*/

            List<BudgetPlanVM> m_lstBudgetPlanVM = new List<BudgetPlanVM>();
            m_boolIsCount = false;
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanTemplateDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanVersion.MapAlias);
            m_lstSelect.Add(string.Format("(select max(a.BudgetPlanVersion) from DBudgetPlanVersion a where a.BudgetPlanID = DBudgetPlanVersion.BudgetPlanID) as {0}", BudgetPlanVM.Prop.MaxBudgetPlanVersion.Name));
            //m_lstSelect.Add(string.Format("MAX({0}) OVER(PARTITION BY {1}) AS {2}", BudgetPlanVersionVM.Prop.BudgetPlanVersion.Map,BudgetPlanVM.Prop.BudgetPlanID.Map, BudgetPlanVM.Prop.MaxBudgetPlanVersion.Name));
            m_lstSelect.Add(BudgetPlanVM.Prop.Description.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.ProjectDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.ClusterDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.UnitTypeDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.StatusDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.StatusID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.IsBidOpen.MapAlias);

            List<object> m_lstFilters = new List<object>();
            m_lstFilters = new List<object>();
            m_lstFilters.Add(Operator.Equals);
            m_lstFilters.Add(1);
            m_objFilter.Add(BudgetPlanVM.Prop.IsBidOpen.Map, m_lstFilters);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            foreach (DataSorter m_dtsOrder in parameters.Sort)
                m_dicOrder.Add(BudgetPlanVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));
            
            Dictionary<int, DataSet> m_dicTBudgetPlanDA = m_objTBudgetPlanDA.SelectBC(0, null, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
            if (m_objTBudgetPlanDA.Message == string.Empty)
            {
                m_lstBudgetPlanVM = (
                    from DataRow m_drTBudgetPlanDA in m_dicTBudgetPlanDA[0].Tables[0].Rows
                    select new BudgetPlanVM()
                    {
                        BudgetPlanID = m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanID.Name].ToString(),
                        BudgetPlanTemplateDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanTemplateDesc.Name].ToString(),
                        BudgetPlanVersion = int.Parse(m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanVersion.Name].ToString()),
                        Description = m_drTBudgetPlanDA[BudgetPlanVM.Prop.Description.Name].ToString(),
                        ProjectDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.ProjectDesc.Name].ToString(),
                        ClusterDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.ClusterDesc.Name].ToString(),
                        UnitTypeDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.UnitTypeDesc.Name].ToString(),
                        StatusDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.StatusDesc.Name].ToString(),
                        StatusID = int.Parse(m_drTBudgetPlanDA[BudgetPlanVM.Prop.StatusID.Name].ToString()),
                        IsBidOpen = bool.Parse(m_drTBudgetPlanDA[BudgetPlanVM.Prop.IsBidOpen.Name].ToString()),
                        MaxBudgetPlanVersion = int.Parse(m_drTBudgetPlanDA[BudgetPlanVM.Prop.MaxBudgetPlanVersion.Name].ToString())
                    }
                ).Distinct().ToList();
            }


            List<BudgetPlanVM> listBudgetPlan = (m_lstBudgetPlanVM.Any() ?
                m_lstBudgetPlanVM.Where(d => d.MaxBudgetPlanVersion == d.BudgetPlanVersion).Skip(m_intSkip).Take(m_intLength).ToList() : new List<BudgetPlanVM>());
            string messages = "";
            List<BudgetPlanPeriodVM> m_lstBudgetPlanPeriodVM = new List<BudgetPlanPeriodVM>();
            m_lstBudgetPlanPeriodVM = (
                        from BudgetPlanVM pr in listBudgetPlan
                        select new BudgetPlanPeriodVM()
                        {
                            BudgetPlanID = pr.BudgetPlanID,
                            Description = pr.Description,
                            BudgetPlanTemplateDesc = pr.BudgetPlanTemplateDesc,
                            BudgetPlanTypeID = pr.BudgetPlanTypeID,
                            ProjectDesc = pr.ProjectDesc,
                            ClusterDesc = pr.ClusterDesc,
                            UnitTypeDesc = pr.UnitTypeDesc,
                            BudgetPlanVersion = pr.BudgetPlanVersion.ToString(),
                            StatusDesc = pr.StatusDesc,
                            StatusID = pr.StatusID.ToString(),
                            ListBudgetPlanVersionVendorVM = GetListItemVendor(pr.BudgetPlanID, pr.BudgetPlanVersion, ref messages)
                        }
                    ).ToList();

            return this.Store(m_lstBudgetPlanPeriodVM, m_lstBudgetPlanPeriodVM.Count());
        }
        private List<BudgetPlanVersionVendorVM> GetListItemVendor(string BudgetPlanID, int Version, ref string message)
        {
            List<BudgetPlanVersionVendorVM> m_lstBudgetPlanVersionVendorVM = new List<BudgetPlanVersionVendorVM>();
            //BudgetPlanVersionVendorVM m_objBudgetPlanVM = new BudgetPlanVersionVendorVM();
            DBudgetPlanVersionVendorDA m_objDBudgetPlanVersionVendorDA = new DBudgetPlanVersionVendorDA();
            m_objDBudgetPlanVersionVendorDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionVendorID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.LastPeriodVersion.Map);
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.FeePercentage.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.FirstName.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.LastName.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.StartDate.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.EndDate.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanID);
            m_objFilter.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.NotEqual);
            m_lstFilter.Add("");
            m_objFilter.Add(BudgetPlanVersionVendorVM.Prop.LastPeriodVersion.Map, m_lstFilter);

            //m_lstFilter = new List<object>();
            //m_lstFilter.Add(Operator.Equals);
            //m_lstFilter.Add(Version);
            //m_objFilter.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(string.Format("{0},{1}", General.EnumDesc(BudgetPlanVersionStatus.Submitted), General.EnumDesc(BudgetPlanVersionStatus.Approved)));
            m_objFilter.Add(BudgetPlanVersionVendorVM.Prop.StatusIDVendor.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicVersionVendorDA = m_objDBudgetPlanVersionVendorDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDBudgetPlanVersionVendorDA.AffectedRows != 0)
            {

                if (m_objDBudgetPlanVersionVendorDA.Message == string.Empty)
                {
                    foreach (DataRow m_drTBudgetPlanDA in m_dicVersionVendorDA[0].Tables[0].Rows)
                    {
                        BudgetPlanVersionVendorVM m_objBudgetPlanVM = new BudgetPlanVersionVendorVM();
                        m_objBudgetPlanVM.BudgetPlanVersionVendorID = m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionVendorID.Name].ToString();
                        m_objBudgetPlanVM.BudgetPlanID = m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.BudgetPlanID.Name].ToString();
                        m_objBudgetPlanVM.FeePercentage = Convert.ToDecimal(m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.FeePercentage.Name].ToString());
                        m_objBudgetPlanVM.BudgetPlanVersion = (int)m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.BudgetPlanVersion.Name];
                        m_objBudgetPlanVM.VendorID = m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.VendorID.Name].ToString();
                        m_objBudgetPlanVM.FirstName = m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.FirstName.Name].ToString();
                        m_objBudgetPlanVM.LastName = m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.LastName.Name].ToString();
                        m_objBudgetPlanVM.StartDate = DateTime.Parse(m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.StartDate.Name].ToString());
                        m_objBudgetPlanVM.EndDate = DateTime.Parse(m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.EndDate.Name].ToString());
                        m_lstBudgetPlanVersionVendorVM.Add(m_objBudgetPlanVM);
                    }
                }
                else
                    message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDBudgetPlanVersionVendorDA.Message;
            }
            else
            {
            }

            return m_lstBudgetPlanVersionVendorVM;

        }

        public ActionResult GetListBudgetPlanTemplateStructure(StoreRequestParameters parameters, string FilterBudgetPlanTemplateID)
        {
            BudgetPlanTemplateStructureVM m_objMBudgetPlanTemplateStructureVM = new BudgetPlanTemplateStructureVM();
            DBudgetPlanTemplateStructureDA m_objMBudgetPlanTemplateStructureDA = new DBudgetPlanTemplateStructureDA();
            m_objMBudgetPlanTemplateStructureDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;

            FilterHeaderConditions m_fhcTBudgetPlan = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcTBudgetPlan.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = BudgetPlanVM.Prop.Map(m_strDataIndex, false);
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

            if (!string.IsNullOrEmpty(FilterBudgetPlanTemplateID))
            {
                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(FilterBudgetPlanTemplateID);
                m_objFilter.Add(BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.Map, m_lstFilter);
            }

            //Dictionary<int, DataSet> m_dicMBudgetPlanTemplateStructureDA = m_objMBudgetPlanTemplateStructureDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            //int m_intCount = 0;

            //foreach (KeyValuePair<int, DataSet> m_kvpBudgetPlanTemplateDA in m_dicMBudgetPlanTemplateStructureDA)
            //{
            //    m_intCount = m_kvpBudgetPlanTemplateDA.Key;
            //    break;
            //}

            List<BudgetPlanVersionStructureVM> lstBudgetPlanVersionStructureVM = new List<BudgetPlanVersionStructureVM>();
            //if (m_intCount > 0)
            //{
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.Version.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.Sequence.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentSequence.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.HasChild.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemTypeID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemTypeID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.IsDefault.MapAlias);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            foreach (DataSorter m_dtsOrder in parameters.Sort)
                m_dicOrder.Add(BudgetPlanVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

            Dictionary<int, DataSet> m_dicMBudgetPlanTemplateStructureDA = m_objMBudgetPlanTemplateStructureDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMBudgetPlanTemplateStructureDA.Message == string.Empty)
            {
                lstBudgetPlanVersionStructureVM = (
                    from DataRow m_drMBudgetPlanTemplateStructureDA in m_dicMBudgetPlanTemplateStructureDA[0].Tables[0].Rows
                    select new BudgetPlanVersionStructureVM()
                    {
                        ItemDesc = m_drMBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemDesc.Name].ToString(),
                        //BudgetPlanTemplateID = m_drMBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.Name].ToString(),
                        ItemID = m_drMBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemID.Name].ToString(),
                        Version = Convert.ToInt16(m_drMBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.Version.Name].ToString()),
                        Sequence = Convert.ToInt16(m_drMBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.Sequence.Name].ToString()),
                        ParentItemID = m_drMBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentItemID.Name].ToString(),
                        ParentSequence = Convert.ToInt16(m_drMBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentSequence.Name].ToString()),
                        ParentVersion = Convert.ToInt16(m_drMBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentVersion.Name].ToString())
                        //HasChild = Convert.ToBoolean(m_drMBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.HasChild.Name].ToString()),
                        //ItemTypeID = m_drMBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString(),
                        //ParentItemTypeID = m_drMBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentItemTypeID.Name].ToString(),
                        //IsDefault = Convert.ToBoolean(m_drMBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.IsDefault.Name].ToString()),
                        //EnableDefault = false
                    }
                ).ToList();
            }
            //}


            return this.Store(lstBudgetPlanVersionStructureVM);
        }
        public ActionResult Home()
        {
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 0;
            return this.Direct();
        }
        public ActionResult Add(string Caller, string Selected, string BudgetPlanVersion)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Add)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));
            //Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();

            Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();
            if (Caller == General.EnumDesc(Buttons.ButtonUpdate))
            {
                m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);

            }
            string m_strMessage = string.Empty;
            BudgetPlanVM m_objBudgetPlanVM = new BudgetPlanVM();
            m_objBudgetPlanVM.CreatedDate = DateTime.Now;
            m_objBudgetPlanVM.ModifiedDate = DateTime.Now;
            if (m_dicSelectedRow.Count > 0)
                m_objBudgetPlanVM = GetSelectedData(m_dicSelectedRow, BudgetPlanVersion, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowError(title, m_strMessage);
                return this.Direct();
            }
            m_objBudgetPlanVM.ListBudgetPlanVersionStructureVM = new List<BudgetPlanVersionStructureVM>();
            if (!string.IsNullOrEmpty(m_objBudgetPlanVM.BudgetPlanID))
            {
                m_objBudgetPlanVM.BudgetPlanVersion = 0;
                m_objBudgetPlanVM.StatusID = 0;
                m_objBudgetPlanVM.StatusDesc = string.Empty;
                m_objBudgetPlanVM.ListBudgetPlanVersionStructureVM = GetListBudgetPlanStructure(m_objBudgetPlanVM.BudgetPlanID, ref m_strMessage);

            }
            if (m_strMessage != string.Empty)
            {
                Global.ShowError(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddItem = new ViewDataDictionary();
            m_vddItem.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddItem.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));

            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objBudgetPlanVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddItem,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult Detail(string Caller, string Selected, string BudgetPlanVersion, string BudgetPlanID)
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
            else if (Caller == General.EnumDesc(Buttons.ButtonList) || Caller == "ComboBoxVersion")
            {
                m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
            }
            else if (Caller == General.EnumDesc(Buttons.ButtonAdd) || Caller == General.EnumDesc(Buttons.ButtonUpdate))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                m_dicSelectedRow = GetFormData(m_nvcParams, BudgetPlanID);
            }

            BudgetPlanVM m_objBudgetPlanVM = new BudgetPlanVM();
            if (m_dicSelectedRow.Count > 0)
                m_objBudgetPlanVM = GetSelectedData(m_dicSelectedRow, BudgetPlanVersion, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowError(title, m_strMessage);
                return this.Direct();
            }

            m_objBudgetPlanVM.ListBudgetPlanVersionStructureVM = new List<BudgetPlanVersionStructureVM>();
            if (m_objBudgetPlanVM != null)
                m_objBudgetPlanVM.ListBudgetPlanVersionStructureVM = GetListBudgetPlanStructure(m_objBudgetPlanVM.BudgetPlanID, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowError(title, m_strMessage);
                return this.Direct();
            }

            List<BudgetPlanVersionVendorVM> vendorCount = GetListItemVendor(m_objBudgetPlanVM.BudgetPlanID, m_objBudgetPlanVM.BudgetPlanVersion, ref m_strMessage);
            string grpVendor = "";
            
            foreach (BudgetPlanVersionVendorVM vnd in vendorCount)
                grpVendor += "|" + vnd.VendorDesc;
            
            ViewDataDictionary m_vddComparison = new ViewDataDictionary();
            m_vddComparison.Add("FeePercentage", m_objBudgetPlanVM.FeePercentage);
            m_vddComparison.Add("CellColor", GetListColor());
            m_vddComparison.Add("TolerancePrice", GetValuePriceTolerance());
            m_vddComparison.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
            m_vddComparison.Add("Version", m_objBudgetPlanVM.BudgetPlanVersion);
            m_vddComparison.Add("VendorCount", vendorCount.Count());
            m_vddComparison.Add("VendorListName", grpVendor);
            m_vddComparison.Add("ProjectID", m_objBudgetPlanVM.ProjectID);
            m_vddComparison.Add("ClusterID", m_objBudgetPlanVM.ClusterID);
            m_vddComparison.Add("UnitTypeID", m_objBudgetPlanVM.UnitTypeID);
            m_vddComparison.Add("AreaSize", m_objBudgetPlanVM.Area);
            m_vddComparison.Add("TotalUnit", m_objBudgetPlanVM.Unit);
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objBudgetPlanVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddComparison,
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

                BudgetPlanVM m_objBudgetPlan = JSON.Deserialize<BudgetPlanVM>(Selected);
                if (m_objBudgetPlan.StatusID >= 2) //Approved
                {
                    Global.ShowMessage(title, "Do you want to create new version?", MessageBox.Button.YESNO, MessageBox.Icon.INFO, "showResultText");
                    return this.Direct();
                }
            }
            else if (Caller == General.EnumDesc(Buttons.ButtonDetail))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                m_dicSelectedRow = GetFormData(m_nvcParams, string.Empty);
                Session[dataSessionName] = JSON.Serialize(m_dicSelectedRow);
            }

            string m_strMessage = string.Empty;
            BudgetPlanVM m_objBudgetPlanVM = new BudgetPlanVM();
            if (m_dicSelectedRow.Count > 0)
                m_objBudgetPlanVM = GetSelectedData(m_dicSelectedRow, string.Empty, ref m_strMessage);

            if (m_strMessage != string.Empty)
            {
                Global.ShowError(title, m_strMessage);
                return this.Direct();
            }

            m_objBudgetPlanVM.ListBudgetPlanVersionStructureVM = new List<BudgetPlanVersionStructureVM>();
            if (m_objBudgetPlanVM != null)
                m_objBudgetPlanVM.ListBudgetPlanVersionStructureVM = GetListBudgetPlanStructure(m_objBudgetPlanVM.BudgetPlanID, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowError(title, m_strMessage);
                return this.Direct();
            }
            List<BudgetPlanVersionVendorVM> vendorCount = GetListItemVendor(m_objBudgetPlanVM.BudgetPlanID, m_objBudgetPlanVM.BudgetPlanVersion, ref m_strMessage);

            ViewDataDictionary m_vddItemGroup = new ViewDataDictionary();
            m_vddItemGroup.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddItemGroup.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            m_vddItemGroup.Add("vendorCount", m_objBudgetPlanVM.BudgetPlanVersion);
            m_vddItemGroup.Add("ver", m_objBudgetPlanVM.BudgetPlanVersion);
            m_vddItemGroup.Add("ProjectID", m_objBudgetPlanVM.ProjectID);
            m_vddItemGroup.Add("ClusterID", m_objBudgetPlanVM.ClusterID);
            m_vddItemGroup.Add("UnitTypeID", m_objBudgetPlanVM.UnitTypeID);
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objBudgetPlanVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddItemGroup,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<BudgetPlanVM> m_lstSelectedRow = new List<BudgetPlanVM>();
            m_lstSelectedRow = JSON.Deserialize<List<BudgetPlanVM>>(Selected);

            TBudgetPlanDA m_objTBudgetPlanDA = new TBudgetPlanDA();
            DBudgetPlanVersionDA d_objDBudgetPlanVersionDA = new DBudgetPlanVersionDA();
            d_objDBudgetPlanVersionDA.ConnectionStringName = Global.ConnStrConfigName;
            m_objTBudgetPlanDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (BudgetPlanVM m_objBudgetPlanVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifBudgetPlanVM = m_objBudgetPlanVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifBudgetPlanVM in m_arrPifBudgetPlanVM)
                    {
                        string m_strFieldName = m_pifBudgetPlanVM.Name;
                        object m_objFieldValue = m_pifBudgetPlanVM.GetValue(m_objBudgetPlanVM);
                        if (m_objBudgetPlanVM.IsKey(m_strFieldName))
                        {

                            TBudgetPlan m_objTBudgetPlan = new TBudgetPlan();
                            m_objTBudgetPlan.BudgetPlanID = m_objFieldValue.ToString();
                            m_objTBudgetPlanDA.Data = m_objTBudgetPlan;
                            m_objTBudgetPlanDA.Select();

                            m_objTBudgetPlanDA.Update(false);
                        }
                        else break;
                    }

                    if (m_objTBudgetPlanDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objTBudgetPlanDA.Message);
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                return this.Direct(false, ex.Message);

            }
            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfo(title, General.EnumDesc(MessageLib.Deleted));
                return this.Direct(true, string.Empty);
            }
            else
                return this.Direct(false, String.Join(Global.NewLineSeparated, m_lstMessage));

        }
        public ActionResult Save(string Action)
        {
            List<string> m_lstMessage = new List<string>();
            TBudgetPlanDA m_objTBudgetPlanDA = new TBudgetPlanDA();
            BudgetPlanVM m_objMBudgetPlanVM = new BudgetPlanVM();
            DBudgetPlanVersionDA m_objDBudgetPlanVersionDA = new DBudgetPlanVersionDA();
            TBudgetPlan m_objTBudgetPlan = new TBudgetPlan();
            DBudgetPlanVersionStructureDA m_objDBudgetPlanVersionStructureDA = new DBudgetPlanVersionStructureDA();

            string m_strBudgetPlanID = string.Empty;
            m_objTBudgetPlanDA.ConnectionStringName = Global.ConnStrConfigName;

            string m_strTransName = "BudgetPlan";
            object m_objDBConnection = null;
            m_objDBConnection = m_objTBudgetPlanDA.BeginTrans(m_strTransName);
            try
            {
                m_strBudgetPlanID = this.Request.Params[BudgetPlanVM.Prop.BudgetPlanID.Name];
                string m_strDescription = this.Request.Params[BudgetPlanVM.Prop.Description.Name];
                string m_strBudgetPlanTemplateID = this.Request.Params[BudgetPlanVM.Prop.BudgetPlanTemplateID.Name];
                string m_strProjectID = this.Request.Params[BudgetPlanVM.Prop.ProjectID.Name];
                string m_strClusterID = this.Request.Params[BudgetPlanVM.Prop.ClusterID.Name];
                string m_strUnitTypeID = this.Request.Params[BudgetPlanVM.Prop.UnitTypeID.Name];
                string m_strBudgetPlanVersion = this.Request.Params[BudgetPlanVM.Prop.BudgetPlanVersion.Name];
                string m_strArea = this.Request.Params[BudgetPlanVM.Prop.Area.Name];
                string m_strStatusID = this.Request.Params[BudgetPlanVM.Prop.StatusID.Name];

                string m_strListBudgetPlanVersionStructureVM = this.Request.Params[BudgetPlanVM.Prop.ListBudgetPlanVersionStructureVM.Name];

                List<BudgetPlanVersionStructureVM> m_lstListBudgetPlanVersionStructureVM = JSON.Deserialize<List<BudgetPlanVersionStructureVM>>(m_strListBudgetPlanVersionStructureVM);

                m_objMBudgetPlanVM.BudgetPlanID = m_strBudgetPlanID;
                m_objMBudgetPlanVM.Description = m_strDescription;
                m_objMBudgetPlanVM.BudgetPlanTemplateID = m_strBudgetPlanTemplateID;
                m_objMBudgetPlanVM.ProjectID = m_strProjectID;
                m_objMBudgetPlanVM.ClusterID = m_strClusterID;
                m_objMBudgetPlanVM.UnitTypeID = m_strUnitTypeID;
                m_objMBudgetPlanVM.BudgetPlanVersion = int.Parse(m_strBudgetPlanVersion == string.Empty ? "1" : m_strBudgetPlanVersion);
                m_objMBudgetPlanVM.Area = decimal.Parse(m_strArea);
                m_objMBudgetPlanVM.StatusID = int.Parse(string.IsNullOrEmpty(m_strStatusID) ? "0" : m_strStatusID);

                //m_lstMessage = IsSaveValid(Action, m_objMBudgetPlanVM);
                if (m_lstMessage.Count <= 0)
                {
                    List<string> success_status = new List<string>();


                    #region Budget Plan

                    m_objTBudgetPlan.BudgetPlanID = m_objMBudgetPlanVM.BudgetPlanID;
                    m_objTBudgetPlanDA.Data = m_objTBudgetPlan;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objTBudgetPlanDA.Select();



                    m_objTBudgetPlan.BudgetPlanTemplateID = m_objMBudgetPlanVM.BudgetPlanTemplateID;
                    m_objTBudgetPlan.ProjectID = m_objMBudgetPlanVM.ProjectID;
                    m_objTBudgetPlan.ClusterID = m_objMBudgetPlanVM.ClusterID;
                    m_objTBudgetPlan.UnitTypeID = m_objMBudgetPlanVM.UnitTypeID;

                    m_objDBConnection = m_objTBudgetPlanDA.BeginTrans(m_strTransName);
                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                    {
                        BudgetPlanVersionVM objBudgetPlanVersionVM = GetLastBudgetPlanVersion(m_objMBudgetPlanVM.BudgetPlanID, m_objMBudgetPlanVM.BudgetPlanTemplateID);
                        if (objBudgetPlanVersionVM.BudgetPlanID != null)
                            if (objBudgetPlanVersionVM.StatusID < 2)
                            { //TOBE enum
                                Global.ShowError(title, "Cannot add new version");
                                m_objTBudgetPlanDA.EndTrans(ref m_objDBConnection, m_strTransName);
                                return this.Direct();
                            }

                        m_objTBudgetPlanDA.Insert(true, m_objDBConnection);
                    }
                    else
                        m_objTBudgetPlanDA.Update(true, m_objDBConnection);

                    if (!m_objTBudgetPlanDA.Success || m_objTBudgetPlanDA.Message != string.Empty)
                    {
                        m_objTBudgetPlanDA.EndTrans(ref m_objDBConnection, m_strTransName);
                        return this.Direct(false, m_objTBudgetPlanDA.Message);
                    }
                    m_strBudgetPlanID = m_objTBudgetPlanDA.Data.BudgetPlanID;
                    #endregion

                    #region DBudgetPlanVersion
                    DBudgetPlanVersion m_objDBudgetPlanVersion = new DBudgetPlanVersion();

                    m_objDBudgetPlanVersion.BudgetPlanID = m_strBudgetPlanID;
                    m_objDBudgetPlanVersion.BudgetPlanVersion = m_objMBudgetPlanVM.BudgetPlanVersion;
                    m_objDBudgetPlanVersionDA.Data = m_objDBudgetPlanVersion;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objDBudgetPlanVersionDA.Select();

                    m_objDBudgetPlanVersion.Description = m_objMBudgetPlanVM.Description;
                    m_objDBudgetPlanVersion.Area = m_objMBudgetPlanVM.Area;
                    m_objDBudgetPlanVersion.StatusID = m_objMBudgetPlanVM.StatusID;


                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                    {
                        if (string.IsNullOrEmpty(m_objMBudgetPlanVM.BudgetPlanID))
                            m_objMBudgetPlanVM.BudgetPlanVersion = 1;
                        else
                            m_objMBudgetPlanVM.BudgetPlanVersion = (GetLastBudgetPlanVersion(m_objMBudgetPlanVM.BudgetPlanID, m_objMBudgetPlanVM.BudgetPlanTemplateID).BudgetPlanVersion + 1);

                        m_objDBudgetPlanVersionDA.Insert(true, m_objDBConnection);
                    }
                    else
                    {
                        if (m_objDBudgetPlanVersion.StatusID == 0 && m_objDBudgetPlanVersion.BudgetPlanVersion == 1)
                            m_objDBudgetPlanVersionDA.Update(true, m_objDBConnection);
                        else
                        {
                            Global.ShowError(title, "Cannot update. Status must be draft and version is 1");
                            m_objDBudgetPlanVersionDA.EndTrans(ref m_objDBConnection, m_strTransName);
                            return this.Direct();
                        }
                    }

                    if (!m_objDBudgetPlanVersionDA.Success || m_objDBudgetPlanVersionDA.Message != string.Empty)
                    {
                        m_objDBudgetPlanVersionDA.EndTrans(ref m_objDBConnection, m_strTransName);
                        return this.Direct(false, m_objDBudgetPlanVersionDA.Message);
                    }


                    #endregion

                    #region DBudgetPlanVersionStructure


                    m_objDBudgetPlanVersionStructureDA.ConnectionStringName = Global.ConnStrConfigName;

                    if (Action == General.EnumDesc(Buttons.ButtonUpdate))
                    {
                        Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                        List<object> m_lstFilter = new List<object>();
                        m_lstFilter.Add(Operator.Equals);
                        m_lstFilter.Add(m_strBudgetPlanID);
                        m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanID.Map, m_lstFilter);

                        m_objDBudgetPlanVersionStructureDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                    }

                    foreach (BudgetPlanVersionStructureVM objBudgetPlanVersionStructureVM in m_lstListBudgetPlanVersionStructureVM)
                    {
                        DBudgetPlanVersionStructure m_objDBudgetPlanVersionStructure = new DBudgetPlanVersionStructure();

                        m_objDBudgetPlanVersionStructure.BudgetPlanVersionStructureID = Guid.NewGuid().ToString().Replace("-", ""); ;
                        m_objDBudgetPlanVersionStructure.BudgetPlanID = m_strBudgetPlanID;
                        m_objDBudgetPlanVersionStructure.BudgetPlanVersion = m_objDBudgetPlanVersion.BudgetPlanVersion;
                        m_objDBudgetPlanVersionStructure.ItemID = objBudgetPlanVersionStructureVM.ItemID;
                        m_objDBudgetPlanVersionStructure.Version = objBudgetPlanVersionStructureVM.Version;
                        m_objDBudgetPlanVersionStructure.Sequence = objBudgetPlanVersionStructureVM.Sequence;
                        m_objDBudgetPlanVersionStructure.ParentItemID = objBudgetPlanVersionStructureVM.ParentItemID;
                        m_objDBudgetPlanVersionStructure.ParentVersion = objBudgetPlanVersionStructureVM.ParentVersion;
                        m_objDBudgetPlanVersionStructure.ParentSequence = objBudgetPlanVersionStructureVM.ParentSequence;

                        m_objDBudgetPlanVersionStructure.ItemVersionChildID = objBudgetPlanVersionStructureVM.ItemVersionChildID ?? string.Empty;
                        m_objDBudgetPlanVersionStructure.Volume = objBudgetPlanVersionStructureVM.Volume ?? 0;
                        m_objDBudgetPlanVersionStructure.Specification = objBudgetPlanVersionStructureVM.Specification ?? string.Empty;
                        m_objDBudgetPlanVersionStructure.MaterialAmount = objBudgetPlanVersionStructureVM.MaterialAmount ?? 0;
                        m_objDBudgetPlanVersionStructure.WageAmount = objBudgetPlanVersionStructureVM.WageAmount ?? 0;
                        m_objDBudgetPlanVersionStructure.MiscAmount = objBudgetPlanVersionStructureVM.MiscAmount ?? 0;



                        m_objDBudgetPlanVersionStructureDA.Data = m_objDBudgetPlanVersionStructure;
                        m_objDBudgetPlanVersionStructureDA.Insert(true, m_objDBConnection);

                        if (!m_objDBudgetPlanVersionStructureDA.Success || m_objDBudgetPlanVersionStructureDA.Message != string.Empty)
                        {
                            m_objDBudgetPlanVersionStructureDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                            return this.Direct(false, m_objDBudgetPlanVersionStructureDA.Message);
                        }
                    }

                    #endregion



                    if (!m_objTBudgetPlanDA.Success || m_objTBudgetPlanDA.Message != string.Empty)
                        m_lstMessage.Add(m_objTBudgetPlanDA.Message);
                    else
                        m_objTBudgetPlanDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                m_objTBudgetPlanDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
            }
            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfo(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
                return Detail(Action, null, null, m_strBudgetPlanID);
            }
            Global.ShowError(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);
        }
        public ActionResult GetListBudgetPlanVersion(StoreRequestParameters parameters, string BudgetPlanID)
        {
            List<BudgetPlanVersionVM> m_lstBudgetPlanVersionVM = new List<BudgetPlanVersionVM>();
            DBudgetPlanVersionDA m_objDBudgetPlanVersionDA = new DBudgetPlanVersionDA();
            m_objDBudgetPlanVersionDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVersionVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVM.Prop.BudgetPlanVersion.MapAlias);

            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanID);
            m_objFilter.Add(BudgetPlanVersionVM.Prop.BudgetPlanID.Map, m_lstFilter);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(BudgetPlanVersionVM.Prop.BudgetPlanVersion.Map, OrderDirection.Ascending);


            Dictionary<int, DataSet> m_dicDBudgetPlanVersionDA = m_objDBudgetPlanVersionDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, m_dicOrder);
            if (m_objDBudgetPlanVersionDA.Message == string.Empty)
            {
                m_lstBudgetPlanVersionVM = (
                from DataRow m_drDBudgetPlanVersionDA in m_dicDBudgetPlanVersionDA[0].Tables[0].Rows
                select new BudgetPlanVersionVM()
                {
                    BudgetPlanVersion = int.Parse(m_drDBudgetPlanVersionDA[BudgetPlanVersionVM.Prop.BudgetPlanVersion.Name].ToString()),
                    BudgetPlanID = m_drDBudgetPlanVersionDA[BudgetPlanVersionVM.Prop.BudgetPlanID.Name].ToString()
                }).Distinct().ToList();
            }

            return this.Store(m_lstBudgetPlanVersionVM); ;
        }

        #region Budget Plan Structure Action
        public ActionResult AddBudgetPlanVersionStructure(string Caller, string Selected, string ControlBudgetPlanTemplateID, string ControlItemID, string ControlItemDesc,
             string ControlVersion, string ControlSequence, string ControlItemTypeID, string ControlParentItemID, string ControlParentItemDesc,
             string ControlParentVersion, string ControlParentSequence, string ControlParentItemTypeID, string ControlIsDefault, string ControlUoMDesc, string ChildItemTypeID)
        {
            BudgetPlanVersionStructureVM m_objBudgetPlanVersionStructureVM = new BudgetPlanVersionStructureVM();
            m_objBudgetPlanVersionStructureVM = JSON.Deserialize<BudgetPlanVersionStructureVM>(Selected);
            ChildItemTypeID = JSON.Deserialize<string>(ChildItemTypeID);

            if (!(GetFilterTemplateConfig("TRUE", m_objBudgetPlanVersionStructureVM.ItemTypeID).Any() && !(ChildItemTypeID != m_objBudgetPlanVersionStructureVM.ItemTypeID)))
            {
                Global.ShowError("Budget Plan", "Cannot add structure inside selected item!");
            }
            else
            {

                BudgetPlanTemplateController m_ctrlBudgetPlanController = new BudgetPlanTemplateController();
                return m_ctrlBudgetPlanController.BrowseStructure(ControlItemID, ControlItemDesc, ControlVersion, ControlSequence,
                    ControlItemTypeID, ControlParentItemID, ControlParentVersion, ControlParentSequence, ControlParentItemTypeID, ControlIsDefault,
                    m_objBudgetPlanVersionStructureVM.BudgetPlanTemplateID,null, null, null);
            }

            return this.Direct();
        }
        public ActionResult RefreshPriceBudgetPlanVersionStructure(string Caller, string Selected)
        {
            return this.Direct();

        }
        public ActionResult UpdateBudgetPlanVersionStructure(string Caller, string Selected, string ControlBudgetPlanTemplateID, string ControlItemID, string ControlItemDesc,
             string ControlVersion, string ControlSequence, string ControlItemTypeID, string ControlParentItemID, string ControlParentItemDesc,
             string ControlParentVersion, string ControlParentSequence, string ControlParentItemTypeID, string ControlIsDefault, string ControlUoMDesc)
        {

            BudgetPlanVersionStructureVM m_objBudgetPlanVersionStructureVM = new BudgetPlanVersionStructureVM();
            m_objBudgetPlanVersionStructureVM = JSON.Deserialize<BudgetPlanVersionStructureVM>(Selected);

            if (m_objBudgetPlanVersionStructureVM != null)
            {
                if (GetFilterTemplateConfig("FALSE", m_objBudgetPlanVersionStructureVM.ItemTypeID).Any())
                {

                    BudgetPlanTemplateController m_ctrlBudgetPlanController = new BudgetPlanTemplateController();
                    return m_ctrlBudgetPlanController.BrowseStructure(ControlItemID, ControlItemDesc, ControlVersion, ControlSequence,
                        ControlItemTypeID, ControlParentItemID, ControlParentVersion, ControlParentSequence, ControlParentItemTypeID, ControlIsDefault,
                        m_objBudgetPlanVersionStructureVM.BudgetPlanTemplateID, m_objBudgetPlanVersionStructureVM.ParentItemID, m_objBudgetPlanVersionStructureVM.ParentVersion.ToString(), null);
                }
                else
                {
                    UnitPriceAnalysisController m_ctrlUnitPriceAnalysisController = new UnitPriceAnalysisController();
                    return m_ctrlUnitPriceAnalysisController.BrowseChildUnion(ControlItemID, ControlItemDesc, ControlUoMDesc, ControlItemTypeID, "", "", ControlVersion, ControlItemTypeID, "", "", m_objBudgetPlanVersionStructureVM.ItemVersionChildID);
                }
            }
            return this.Direct();

        }
        public ActionResult DeleteBudgetPlanVersionStructure(string Selected)
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));


            return this.Direct();
        }
        #endregion
        #endregion

        #region Direct Method

        public ActionResult GetBudgetPlan(string ControlBudgetPlanID, string ControlDescription, string ControlStatusID, string ControlStatusDesc,
            string ControlBudgetPlanTypeID, string ControlBudgetPlanTypeDesc, string ControlBudgetPlanVersion, string FilterBudgetPlanID, string FilterDescription, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<BudgetPlanVM>> m_dicBudgetPlanData = GetBudgetPlanData(true, FilterBudgetPlanID, FilterDescription);
                KeyValuePair<int, List<BudgetPlanVM>> m_kvpBudgetPlanVM = m_dicBudgetPlanData.AsEnumerable().ToList()[0];
                if (m_kvpBudgetPlanVM.Key < 1 || (m_kvpBudgetPlanVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpBudgetPlanVM.Key > 1 && !Exact)
                    return Browse(ControlBudgetPlanID, ControlDescription, ControlBudgetPlanTypeID, ControlBudgetPlanTypeDesc, ControlStatusID, ControlStatusDesc, ControlBudgetPlanVersion, "", "", FilterBudgetPlanID, FilterDescription);

                m_dicBudgetPlanData = GetBudgetPlanData(false, FilterBudgetPlanID, FilterDescription);
                BudgetPlanVM m_objBudgetPlanVM = m_dicBudgetPlanData[0][0];
                this.GetCmp<TextField>(ControlBudgetPlanID).Value = m_objBudgetPlanVM.BudgetPlanID;
                this.GetCmp<TextField>(ControlDescription).Value = m_objBudgetPlanVM.Description;
                this.GetCmp<TextField>(ControlBudgetPlanTypeDesc).Value = m_objBudgetPlanVM.BudgetPlanTypeDesc;
                this.GetCmp<TextField>(ControlBudgetPlanTypeID).Value = m_objBudgetPlanVM.BudgetPlanTypeID;
                this.GetCmp<TextField>(ControlStatusDesc).Value = m_objBudgetPlanVM.StatusDesc;
                this.GetCmp<TextField>(ControlStatusID).Value = m_objBudgetPlanVM.StatusID;
                this.GetCmp<TextField>(ControlBudgetPlanVersion).Value = m_objBudgetPlanVM.BudgetPlanVersion;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }
        public ActionResult GetVendorLists(string BudgetPlanID, int Version)
        {
            List<BudgetPlanTemplateStructureVM> vendorlist = new List<BudgetPlanTemplateStructureVM>();
            List<string> VendorName = new List<string>();
            string message = "";
            List<BudgetPlanVersionVendorVM> ListBudgetPlanVersionVendorVM = new List<BudgetPlanVersionVendorVM>();
            ListBudgetPlanVersionVendorVM = GetListItemVendor(BudgetPlanID, Version, ref message);
            foreach (BudgetPlanVersionVendorVM cd in ListBudgetPlanVersionVendorVM)
            {
                if (cd.BudgetPlanID != null)
                    VendorName.Add(cd.VendorDesc);
            }
            return this.Store(VendorName);
        }
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
            m_lstFilter.Add("DBudgetPlanVersion");
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

        public ActionResult ClearExcelFile(string filename)
        {
            string fullPath = Request.MapPath("~/Content/" + filename);
            //if (System.IO.File.Exists(fullPath))
            //    System.IO.File.Delete(fullPath);
            return this.Direct();
        }

        public ActionResult ExportExcelReturnDirect(string BPDesc, string GridStructure, string VendorCount )
        {
            int vendorCount_ = 0;
            List<string> grpVendor = new List<string>();
            foreach (string vn in VendorCount.Split('|').ToList())
            {
                if (vn.Length > 0)
                {
                    grpVendor.Add(vn);
                    vendorCount_++;
                }
            }
            string filename = "BUDGET PLAN COMPARISON - ["+ BPDesc + "].xls";
            if (this.Request.Params["GridStructure"] != null)
            {
                DataTable dt = new DataTable();
                //DataRowCollection drCollections = new DataRowCollection[5];
                dt.Columns.Add(new DataColumn("No.", typeof(string)));
                dt.Columns.Add(new DataColumn(BudgetPlanVersionStructureVM.Prop.ItemDesc.Desc, typeof(string)));
                dt.Columns.Add(new DataColumn(BudgetPlanVersionStructureVM.Prop.Specification.Desc, typeof(string)));
                dt.Columns.Add(new DataColumn(BudgetPlanVersionStructureVM.Prop.UoMDesc.Desc, typeof(string)));
                dt.Columns.Add(new DataColumn(BudgetPlanVersionStructureVM.Prop.Volume.Desc, typeof(string)));
                for (int x = 0; x < vendorCount_; x++)
                {
                    dt.Columns.Add(new DataColumn("Volume" + x + 1, typeof(string)));
                }
                //Unit Price
                dt.Columns.Add(new DataColumn("MAT", typeof(string)));
                dt.Columns.Add(new DataColumn("WAG", typeof(string)));
                dt.Columns.Add(new DataColumn("MISC", typeof(string)));
                dt.Columns.Add(new DataColumn(BudgetPlanVersionStructureVM.Prop.TotalUnitPrice.Desc, typeof(string)));
                dt.Columns.Add(new DataColumn("new_materialamount", typeof(string)));
                dt.Columns.Add(new DataColumn("new_wageamount", typeof(string)));
                dt.Columns.Add(new DataColumn("new_miscamount", typeof(string)));
                dt.Columns.Add(new DataColumn("new_"+BudgetPlanVersionStructureVM.Prop.TotalUnitPrice.Desc, typeof(string)));
                for (int x = 0; x < vendorCount_; x++)
                {
                    dt.Columns.Add(new DataColumn("MAT" + x + 1, typeof(string)));
                    dt.Columns.Add(new DataColumn("WAG" + x + 1, typeof(string)));
                    dt.Columns.Add(new DataColumn("MISC" + x + 1, typeof(string)));
                    dt.Columns.Add(new DataColumn(BudgetPlanVersionStructureVM.Prop.TotalUnitPrice.Desc + x + 1, typeof(string)));
                }
                

                //Total
                dt.Columns.Add(new DataColumn(BudgetPlanVersionStructureVM.Prop.Total.Desc, typeof(string)));
                dt.Columns.Add(new DataColumn("new_"+BudgetPlanVersionStructureVM.Prop.Total.Desc, typeof(string)));
                for (int x = 0; x < vendorCount_; x++)
                {
                    dt.Columns.Add(new DataColumn(BudgetPlanVersionStructureVM.Prop.Total.Desc + x + 1, typeof(string)));
                }


                //dt.Rows.Add("", "", "", "", "", "", "", "", "", "");

                Dictionary<string, object>[] m_arrBudgetStructureChild = JSON.Deserialize<Dictionary<string, object>[]>(this.Request.Params["GridStructure"]);
                if (m_arrBudgetStructureChild == null)
                    m_arrBudgetStructureChild = new List<Dictionary<string, object>>().ToArray();


                int collength = m_arrBudgetStructureChild[0].Count;

                foreach (Dictionary<string, object> m_dicBudgetVersionStructureVM in m_arrBudgetStructureChild)
                {
                    if (m_dicBudgetVersionStructureVM.ContainsKey(BudgetPlanVersionStructureVM.Prop.ItemDesc.Name.ToLower()))
                    {
                        string ItemDesc = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.ItemDesc.Name.ToLower()].ToString();
                        string No = m_dicBudgetVersionStructureVM["numbering"] == null ? "" : m_dicBudgetVersionStructureVM["numbering"].ToString();
                        string Specification = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.Specification.Name.ToLower()] == null ? "" : m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.Specification.Name.ToLower()].ToString();
                        string UoM = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.UoMID.Name.ToLower()] == null ? "" : m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.UoMID.Name.ToLower()].ToString();
                        string Volume = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.Volume.Name.ToLower()] == null ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.Volume.Name.ToLower()]).ToString("#,#0.00#");
                        string MAG = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name.ToLower()] == null ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name.ToLower()]).ToString("#,#");
                        string WAG = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.WageAmount.Name.ToLower()] == null ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.WageAmount.Name.ToLower()]).ToString("#,#");
                        string MISC = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.MiscAmount.Name.ToLower()] == null ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.MiscAmount.Name.ToLower()]).ToString("#,#");
                        string TotalUP = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.TotalUnitPrice.Name.ToLower()] == null ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.TotalUnitPrice.Name.ToLower()]).ToString("#,#");
                        string Total = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.Total.Name.ToLower()] == null ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.Total.Name.ToLower()]).ToString("#,#");


                        if (No.Split('.').ToList().Count() == 1 && No != string.Empty && No != "1")
                        {
                            dt.Rows.Add("", "", "", "", "", "", "", "", "", "");
                        }
                        
                        //dt.Rows.Add(No, ItemDesc, Specification, UoM, Volume, MAG, WAG, MISC, TotalUP, Total);

                        object[] arrayRow = new object[5+(grpVendor.Count)+(4*(grpVendor.Count+2))+2+grpVendor.Count];
                        arrayRow[0] = m_dicBudgetVersionStructureVM["numbering"] == null ? "" : m_dicBudgetVersionStructureVM["numbering"].ToString();
                        arrayRow[1] = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.ItemDesc.Name.ToLower()].ToString();
                        arrayRow[2] = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.Specification.Name.ToLower()] == null ? "" : m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.Specification.Name.ToLower()].ToString();
                        arrayRow[3] = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.UoMID.Name.ToLower()] == null ? "" : m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.UoMID.Name.ToLower()].ToString();
                        arrayRow[4] = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.Volume.Name.ToLower()] == null ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.Volume.Name.ToLower()]).ToString("#,#0.00#");
                        int startRowVol = 4;
                        int idx = 1;
                        foreach (string vnName in grpVendor)
                        {
                            //Vendor Volume
                            arrayRow[startRowVol + idx] = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.Volume.Name.ToLower() + "V" + idx.ToString()] == null ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.Volume.Name.ToLower() + "V" + idx.ToString()]).ToString("#,#0.00#");

                            //RAB UPA
                            if (idx == 1)
                            {
                                arrayRow[4 + (grpVendor.Count) + 1] = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name.ToLower()] == null ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name.ToLower()]).ToString("#,#");
                                arrayRow[4 + (grpVendor.Count) + 2] = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.WageAmount.Name.ToLower()] == null ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.WageAmount.Name.ToLower()]).ToString("#,#");
                                arrayRow[4 + (grpVendor.Count) + 3] = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.MiscAmount.Name.ToLower()] == null ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.MiscAmount.Name.ToLower()]).ToString("#,#");
                                arrayRow[4 + (grpVendor.Count) + 4] = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.TotalUnitPrice.Name.ToLower()] == null ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.TotalUnitPrice.Name.ToLower()]).ToString("#,#");

                                //RAB Recent
                                arrayRow[4 + (grpVendor.Count) + 5] = m_dicBudgetVersionStructureVM["new_"+BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name.ToLower()] == null ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM["new_" + BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name.ToLower()]).ToString("#,#");
                                arrayRow[4 + (grpVendor.Count) + 6] = m_dicBudgetVersionStructureVM["new_"+BudgetPlanVersionStructureVM.Prop.WageAmount.Name.ToLower()] == null ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM["new_" + BudgetPlanVersionStructureVM.Prop.WageAmount.Name.ToLower()]).ToString("#,#");
                                arrayRow[4 + (grpVendor.Count) + 7] = m_dicBudgetVersionStructureVM["new_"+BudgetPlanVersionStructureVM.Prop.MiscAmount.Name.ToLower()] == null ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM["new_" + BudgetPlanVersionStructureVM.Prop.MiscAmount.Name.ToLower()]).ToString("#,#");
                                arrayRow[4 + (grpVendor.Count) + 8] = m_dicBudgetVersionStructureVM["new_"+BudgetPlanVersionStructureVM.Prop.TotalUnitPrice.Name.ToLower()] == null ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM["new_" + BudgetPlanVersionStructureVM.Prop.TotalUnitPrice.Name.ToLower()]).ToString("#,#");

                                //RAB Total
                                arrayRow[4 + (grpVendor.Count) + (4 * ((grpVendor.Count + 2))) + 1] = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.Total.Name.ToLower()] == null ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.Total.Name.ToLower()]).ToString("#,#");
                                
                                //RAB Recent Total
                                arrayRow[4 + (grpVendor.Count) + (4 * ((grpVendor.Count + 2))) + 2] = m_dicBudgetVersionStructureVM["new_"+BudgetPlanVersionStructureVM.Prop.Total.Name.ToLower()] == null ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM["new_"+BudgetPlanVersionStructureVM.Prop.Total.Name.ToLower()]).ToString("#,#");
                                
                            }

                            //VendorUPA
                            arrayRow[4 + (grpVendor.Count) + 1 + ((idx + 1) * 4)] = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name.ToLower() + "V" + idx.ToString()] == null ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name.ToLower() + "V" + idx.ToString()]).ToString("#,#");
                            arrayRow[4 + (grpVendor.Count) + 2 + ((idx + 1) * 4)] = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.WageAmount.Name.ToLower() + "V" + idx.ToString()] == null ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.WageAmount.Name.ToLower() + "V" + idx.ToString()]).ToString("#,#");
                            arrayRow[4 + (grpVendor.Count) + 3 + ((idx + 1) * 4)] = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.MiscAmount.Name.ToLower() + "V" + idx.ToString()] == null ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.MiscAmount.Name.ToLower() + "V" + idx.ToString()]).ToString("#,#");
                            arrayRow[4 + (grpVendor.Count) + 4 + ((idx + 1) * 4)] = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.TotalUnitPrice.Name.ToLower() + "V" + idx.ToString()] == null ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.TotalUnitPrice.Name.ToLower() + "V" + idx.ToString()]).ToString("#,#");

                            //VendorTotal
                            arrayRow[4 + (grpVendor.Count) + (4 * ((grpVendor.Count + 2))) + 2 + idx] = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.Total.Name.ToLower() + "V" + idx.ToString()] == null ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.Total.Name.ToLower() + "V" + idx.ToString()]).ToString("#,#");

                            idx++;
                        }
                        
                        
                        dt.Rows.Add(arrayRow);
                    }
                    else
                        dt.Rows.Add("", "", "", "", "", "", "", "", "", "");

                }

               // filename = HttpContext.User.Identity.Name.ToUpperInvariant() + " - " + filename;
                ExportExcel(dt, filename,grpVendor);

            }
            return this.Direct(filename);
        }
        public void ExportExcel(DataTable sourceTable, string fileName, List<string> lst_Vendor)
        {
            HSSFWorkbook m_objWorkbook = new HSSFWorkbook();
            MemoryStream m_objmemoryStream = new MemoryStream();
            HSSFSheet m_objsheet = (HSSFSheet)m_objWorkbook.CreateSheet("BPL-COM");
            HSSFRow m_objTitleRow = (HSSFRow)m_objsheet.CreateRow(0);
            HSSFRow m_objheaderRow = (HSSFRow)m_objsheet.CreateRow(1);

            var FontReg = m_objWorkbook.CreateFont();
            FontReg.FontHeightInPoints = 11;
            FontReg.FontName = "Calibri";
            FontReg.IsBold = false;

            var FontBold = m_objWorkbook.CreateFont();
            FontBold.FontHeightInPoints = 11;
            FontBold.FontName = "Calibri";
            FontBold.IsBold = true;

            #region Title
            m_objTitleRow.HeightInPoints = 26;

            var m_objTitleFont = m_objWorkbook.CreateFont();
            m_objTitleFont.FontHeightInPoints = 20;
            m_objTitleFont.FontName = "Calibri";
            m_objTitleFont.IsBold = true;

            HSSFCell m_objtitleCell = (HSSFCell)m_objTitleRow.CreateCell(0);
            m_objtitleCell.SetCellValue(fileName.Replace(".xls", ""));

            var mergedTitlecell = new NPOI.SS.Util.CellRangeAddress(0, 0, 0, sourceTable.Columns.Count - 1);
            m_objsheet.AddMergedRegion(mergedTitlecell);

            HSSFCellStyle titleCellStyle = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            titleCellStyle.Alignment = HorizontalAlignment.Center;
            titleCellStyle.SetFont(m_objTitleFont);
            titleCellStyle.WrapText = true;
            titleCellStyle.VerticalAlignment = VerticalAlignment.Center;
            titleCellStyle.ShrinkToFit = true;
            m_objtitleCell.CellStyle = titleCellStyle;
            #endregion

            #region Header
            // Create Style
            HSSFCellStyle headerCellStyle = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            //headerCellStyle.FillForegroundColor = HSSFColor.Aqua.Index;
            headerCellStyle.FillPattern = FillPattern.SparseDots;
            headerCellStyle.VerticalAlignment = VerticalAlignment.Center;
            headerCellStyle.Alignment = HorizontalAlignment.Center;
            headerCellStyle.BorderBottom = BorderStyle.Thin;
            headerCellStyle.BorderTop = BorderStyle.Thin;
            headerCellStyle.BorderRight = BorderStyle.Thin;
            headerCellStyle.SetFont(FontBold);
            headerCellStyle.WrapText = true;

            HSSFCellStyle headerCellStyleNoWrap = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            //headerCellStyle.FillForegroundColor = HSSFColor.Aqua.Index;
            headerCellStyleNoWrap.FillPattern = FillPattern.SparseDots;
            headerCellStyleNoWrap.VerticalAlignment = VerticalAlignment.Center;
            headerCellStyleNoWrap.Alignment = HorizontalAlignment.Center;
            headerCellStyleNoWrap.BorderBottom = BorderStyle.Thin;
            headerCellStyleNoWrap.BorderTop = BorderStyle.Thin;
            headerCellStyleNoWrap.BorderRight = BorderStyle.Thin;
            headerCellStyleNoWrap.SetFont(FontBold);
            headerCellStyleNoWrap.WrapText = true;



            // Handling header..
            int columnum = 0;
            int startMergedVolume = 4;
            int endMergedVolume = startMergedVolume;
            int startMergedUPA = endMergedVolume + 1;
            int endMergedUPA = startMergedUPA + 7;
            int startMergedTotalPrice = endMergedUPA + 2;
            int endMergedTotalPrice = startMergedTotalPrice+1;
            
            if (lst_Vendor.Count > 0)
            {
                endMergedVolume += lst_Vendor.Count;
                startMergedUPA = endMergedVolume + 1;
                endMergedUPA = endMergedVolume + (4 * (lst_Vendor.Count+2));
                startMergedTotalPrice = endMergedUPA + 1;
                endMergedTotalPrice = startMergedTotalPrice + 1 + lst_Vendor.Count;
            }
            //Header Row 1
            foreach (DataColumn column in sourceTable.Columns)
            {
                m_objheaderRow.Height = 700;
                HSSFCell headerCell = (HSSFCell)m_objheaderRow.CreateCell(column.Ordinal);
                // Create New Cell
                // Set Cell Value
                if (columnum == startMergedUPA)
                    headerCell.SetCellValue("Unit Price");                
                else if (columnum == startMergedUPA)
                    headerCell.SetCellValue("Total Price");
                else
                    headerCell.SetCellValue(column.ColumnName);

                headerCell.CellStyle = headerCellStyle;


                if (columnum < startMergedVolume)
                {
                    var mergedcell = new NPOI.SS.Util.CellRangeAddress(1, 3, columnum, columnum);
                    m_objsheet.AddMergedRegion(mergedcell);
                }

                if (columnum == startMergedVolume)
                {
                    var mergedcell = new NPOI.SS.Util.CellRangeAddress(1, 1, startMergedVolume, endMergedVolume);
                    m_objsheet.AddMergedRegion(mergedcell);
                }
                else if (columnum == startMergedUPA)
                {
                    var mergedcell = new NPOI.SS.Util.CellRangeAddress(1, 1, startMergedUPA, endMergedUPA);
                    m_objsheet.AddMergedRegion(mergedcell);
                  
                    int startPos = startMergedUPA;
                    int endPos = startPos + 3;
                    for (int x = 0; x <= lst_Vendor.Count+1; x++)
                    {
                        var mergedcell2 = new NPOI.SS.Util.CellRangeAddress(2, 2, startPos, endPos);
                        m_objsheet.AddMergedRegion(mergedcell2);
                        startPos += 4;
                        endPos += 4;
                    }
                }
                else if (columnum == startMergedTotalPrice)
                {
                    var mergedcell = new NPOI.SS.Util.CellRangeAddress(1, 1, startMergedTotalPrice, endMergedTotalPrice);
                    m_objsheet.AddMergedRegion(mergedcell);
                }
                if (columnum >= startMergedVolume && columnum <= endMergedVolume)
                {
                    var mergedcell = new NPOI.SS.Util.CellRangeAddress(2, 3, columnum, columnum);
                    m_objsheet.AddMergedRegion(mergedcell);
                }
                else if (columnum >= startMergedTotalPrice && columnum <= endMergedTotalPrice)
                {
                    var mergedcell = new NPOI.SS.Util.CellRangeAddress(2, 3, columnum, columnum);
                    m_objsheet.AddMergedRegion(mergedcell);
                }

                //m_objsheet.AutoSizeColumn(columnum);
                columnum++;
            }

            //Header Row 2
            HSSFRow m_objdataRowMargin = (HSSFRow)m_objsheet.CreateRow(2);
            int NumberColumnMargin = 0;
            int numUpaIdx = 1;
            for (int x = 0; x < startMergedUPA; x++)
            {
                HSSFCell itemCell = (HSSFCell)m_objdataRowMargin.CreateCell(x);
                itemCell.SetCellValue("");
                itemCell.CellStyle = headerCellStyleNoWrap;
            }
            foreach (string vName in lst_Vendor)
            {
                if (NumberColumnMargin == 0)
                {
                    //RAB Volume
                    HSSFCell itemCellx = (HSSFCell)m_objdataRowMargin.CreateCell(startMergedVolume);
                    itemCellx.SetCellValue("RAB");
                    itemCellx.CellStyle = headerCellStyleNoWrap;

                    //RAB UPA
                    HSSFCell itemCelly = (HSSFCell)m_objdataRowMargin.CreateCell(startMergedUPA);
                    itemCelly.SetCellValue("RAB");
                    itemCelly.CellStyle = headerCellStyleNoWrap;
                    HSSFCell itemCellyw = (HSSFCell)m_objdataRowMargin.CreateCell(startMergedUPA + 3);
                    itemCellyw.SetCellValue("");
                    itemCellyw.CellStyle = headerCellStyleNoWrap;

                    //RAB Recent UPA
                    HSSFCell itemCellywq = (HSSFCell)m_objdataRowMargin.CreateCell(startMergedUPA + 4);
                    itemCellywq.SetCellValue("Recent");
                    itemCellywq.CellStyle = headerCellStyleNoWrap;
                    HSSFCell itemCellyww = (HSSFCell)m_objdataRowMargin.CreateCell(startMergedUPA + 7);
                    itemCellyww.SetCellValue("");
                    itemCellyww.CellStyle = headerCellStyleNoWrap;

                    //RAB Total
                    HSSFCell itemCellz = (HSSFCell)m_objdataRowMargin.CreateCell(startMergedTotalPrice);
                    itemCellz.SetCellValue("RAB");
                    itemCellz.CellStyle = headerCellStyleNoWrap;

                    //RAB Recent Total
                    HSSFCell itemCellza = (HSSFCell)m_objdataRowMargin.CreateCell(startMergedTotalPrice+1);
                    itemCellza.SetCellValue("Recent");
                    itemCellza.CellStyle = headerCellStyleNoWrap;

                }
                //Vendor Volume
                HSSFCell itemCell = (HSSFCell)m_objdataRowMargin.CreateCell(startMergedVolume + 1 + NumberColumnMargin);
                itemCell.SetCellValue(vName);
                itemCell.CellStyle = headerCellStyleNoWrap;
                
                //Vendor UPA
                HSSFCell itemCellnx = (HSSFCell)m_objdataRowMargin.CreateCell(startMergedUPA + ((numUpaIdx+1)*4));
                itemCellnx.SetCellValue(vName);
                itemCellnx.CellStyle = headerCellStyleNoWrap;
                HSSFCell itemCellnxa = (HSSFCell)m_objdataRowMargin.CreateCell(startMergedUPA + ((numUpaIdx + 1) * 4)+3);
                itemCellnxa.SetCellValue("");
                itemCellnxa.CellStyle = headerCellStyleNoWrap;

                //Vendor Total
                HSSFCell itemCelln = (HSSFCell)m_objdataRowMargin.CreateCell(startMergedTotalPrice+2+NumberColumnMargin);
                itemCelln.SetCellValue(vName);
                itemCelln.CellStyle = headerCellStyleNoWrap;

                numUpaIdx++;
                NumberColumnMargin++;
            }
            
            //Header Row 3
            HSSFRow m_objdataRowMargin2 = (HSSFRow)m_objsheet.CreateRow(3);
            int NumberColumnMargin2 = 0;
            for (int x = 0; x < startMergedUPA; x++)
            {
                HSSFCell itemCell = (HSSFCell)m_objdataRowMargin2.CreateCell(x);
                itemCell.SetCellValue("");
                itemCell.CellStyle = headerCellStyleNoWrap;
            }

            for(int y=0;y<=lst_Vendor.Count+1;y++)
            {
                HSSFCell itemCell = (HSSFCell)m_objdataRowMargin2.CreateCell(startMergedUPA+NumberColumnMargin2);
                itemCell.SetCellValue("Material");
                itemCell.CellStyle = headerCellStyleNoWrap;

                HSSFCell itemCell2 = (HSSFCell)m_objdataRowMargin2.CreateCell(startMergedUPA+ NumberColumnMargin2+1);
                itemCell2.SetCellValue("Wage");
                itemCell2.CellStyle = headerCellStyleNoWrap;

                HSSFCell itemCell3 = (HSSFCell)m_objdataRowMargin2.CreateCell(startMergedUPA+ NumberColumnMargin2+2);
                itemCell3.SetCellValue("Other");
                itemCell3.CellStyle = headerCellStyleNoWrap;

                HSSFCell itemCell4 = (HSSFCell)m_objdataRowMargin2.CreateCell(startMergedUPA+ NumberColumnMargin2+3);
                itemCell4.SetCellValue("Total Unit Price");
                itemCell4.CellStyle = headerCellStyleNoWrap;

                NumberColumnMargin2 += 4;
            }
            for (int x = startMergedTotalPrice; x <= endMergedTotalPrice; x++)
            {
                HSSFCell itemCell = (HSSFCell)m_objdataRowMargin2.CreateCell(x);
                itemCell.SetCellValue("");
                itemCell.CellStyle = headerCellStyleNoWrap;
            }
            #endregion
                        
            #region Data Cell Style
            //Create Style for col 1
            HSSFCellStyle itemCellStyleCol1FontReg = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellStyleCol1FontReg.BorderBottom = BorderStyle.Thin;
            itemCellStyleCol1FontReg.BorderRight = BorderStyle.Thin;
            itemCellStyleCol1FontReg.WrapText = true;
            itemCellStyleCol1FontReg.VerticalAlignment = VerticalAlignment.Top;
            itemCellStyleCol1FontReg.Alignment = HorizontalAlignment.Right;
            itemCellStyleCol1FontReg.SetFont(FontReg);

            HSSFCellStyle itemCellStyleCol1FontBold = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellStyleCol1FontBold.BorderBottom = BorderStyle.Thin;
            itemCellStyleCol1FontBold.BorderRight = BorderStyle.Thin;
            itemCellStyleCol1FontBold.WrapText = true;
            itemCellStyleCol1FontBold.VerticalAlignment = VerticalAlignment.Top;
            itemCellStyleCol1FontBold.Alignment = HorizontalAlignment.Right;
            itemCellStyleCol1FontBold.SetFont(FontBold);

            //Create Style for col 2
            List<HSSFCellStyle> lst_indentStyle = new List<HSSFCellStyle>();

            HSSFCellStyle indentStyle = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            indentStyle.BorderBottom = BorderStyle.Thin;
            indentStyle.BorderRight = BorderStyle.Thin;
            indentStyle.WrapText = true;
            indentStyle.VerticalAlignment = VerticalAlignment.Top;
            indentStyle.Alignment = HorizontalAlignment.Left;
            indentStyle.SetFont(FontBold);
            lst_indentStyle.Add(indentStyle);

            //Create Style for col 3
            HSSFCellStyle itemCellStyleCol3Bold = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellStyleCol3Bold.BorderBottom = BorderStyle.Thin;
            itemCellStyleCol3Bold.BorderRight = BorderStyle.Thin;
            itemCellStyleCol3Bold.WrapText = true;
            itemCellStyleCol3Bold.VerticalAlignment = VerticalAlignment.Top;
            itemCellStyleCol3Bold.Alignment = HorizontalAlignment.Left;
            itemCellStyleCol3Bold.SetFont(FontBold);

            HSSFCellStyle itemCellStyleCol3 = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellStyleCol3.BorderBottom = BorderStyle.Thin;
            itemCellStyleCol3.BorderRight = BorderStyle.Thin;
            itemCellStyleCol3.WrapText = true;
            itemCellStyleCol3.VerticalAlignment = VerticalAlignment.Top;
            itemCellStyleCol3.Alignment = HorizontalAlignment.Left;
            itemCellStyleCol3.SetFont(FontReg);

            //Create Style for col 4
            HSSFCellStyle itemCellStyleCol4 = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellStyleCol4.BorderBottom = BorderStyle.Thin;
            itemCellStyleCol4.BorderRight = BorderStyle.Thin;
            itemCellStyleCol4.WrapText = true;
            itemCellStyleCol4.VerticalAlignment = VerticalAlignment.Top;
            itemCellStyleCol4.Alignment = HorizontalAlignment.Center;

            //Create Style for BORDER TOP BOTTOM
            HSSFCellStyle itemCellStyleColTOBBOTTOM = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellStyleColTOBBOTTOM.BorderBottom = BorderStyle.Thin;
            //itemCellStyleColTOBBOTTOM.BorderRight = BorderStyle.Thin;
            itemCellStyleColTOBBOTTOM.WrapText = true;
            itemCellStyleColTOBBOTTOM.VerticalAlignment = VerticalAlignment.Top;
            itemCellStyleColTOBBOTTOM.Alignment = HorizontalAlignment.Center;

            // Create Style for Default Value
            HSSFCellStyle itemCellStyleDefaultValue = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellStyleDefaultValue.BorderBottom = BorderStyle.Thin;
            itemCellStyleDefaultValue.BorderRight = BorderStyle.Thin;
            itemCellStyleDefaultValue.WrapText = true;
            itemCellStyleDefaultValue.VerticalAlignment = VerticalAlignment.Top;
            itemCellStyleDefaultValue.Alignment = HorizontalAlignment.Right;
            itemCellStyleDefaultValue.SetFont(FontReg);

            HSSFCellStyle itemCellStyleDefaultValueLow = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellStyleDefaultValueLow.BorderBottom = BorderStyle.Thin;
            itemCellStyleDefaultValueLow.BorderRight = BorderStyle.Thin;
            itemCellStyleDefaultValueLow.WrapText = true;
            itemCellStyleDefaultValueLow.VerticalAlignment = VerticalAlignment.Top;
            itemCellStyleDefaultValueLow.Alignment = HorizontalAlignment.Right;
            itemCellStyleDefaultValueLow.SetFont(FontReg);
            itemCellStyleDefaultValueLow.FillForegroundColor = HSSFColor.Grey25Percent.Index;
            itemCellStyleDefaultValueLow.FillPattern = FillPattern.SolidForeground;

            HSSFCellStyle itemCellStyleDefaultValueDifferenceVol = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellStyleDefaultValueDifferenceVol.BorderBottom = BorderStyle.Thin;
            itemCellStyleDefaultValueDifferenceVol.BorderRight = BorderStyle.Thin;
            itemCellStyleDefaultValueDifferenceVol.WrapText = true;
            itemCellStyleDefaultValueDifferenceVol.VerticalAlignment = VerticalAlignment.Top;
            itemCellStyleDefaultValueDifferenceVol.Alignment = HorizontalAlignment.Right;
            itemCellStyleDefaultValueDifferenceVol.SetFont(FontReg);
            itemCellStyleDefaultValueDifferenceVol.FillForegroundColor = HSSFColor.Lime.Index;
            itemCellStyleDefaultValueDifferenceVol.FillPattern = FillPattern.SolidForeground;

            HSSFCellStyle itemCellStyleDefaultValueHi = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellStyleDefaultValueHi.BorderBottom = BorderStyle.Thin;
            itemCellStyleDefaultValueHi.BorderRight = BorderStyle.Thin;
            itemCellStyleDefaultValueHi.WrapText = true;
            itemCellStyleDefaultValueHi.VerticalAlignment = VerticalAlignment.Top;
            itemCellStyleDefaultValueHi.Alignment = HorizontalAlignment.Right;
            itemCellStyleDefaultValueHi.SetFont(FontReg);
            itemCellStyleDefaultValueHi.FillForegroundColor = HSSFColor.Yellow.Index;
            itemCellStyleDefaultValueHi.FillPattern = FillPattern.SolidForeground;

            HSSFCellStyle itemCellStyleDefaultValueBold = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellStyleDefaultValueBold.BorderBottom = BorderStyle.Thin;
            itemCellStyleDefaultValueBold.BorderRight = BorderStyle.Thin;
            itemCellStyleDefaultValueBold.WrapText = true;
            itemCellStyleDefaultValueBold.VerticalAlignment = VerticalAlignment.Top;
            itemCellStyleDefaultValueBold.Alignment = HorizontalAlignment.Right;
            itemCellStyleDefaultValueBold.SetFont(FontBold);

            HSSFCellStyle itemCellStyleDefaultValueSingleCellBorderFull = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellStyleDefaultValueSingleCellBorderFull.BorderBottom = BorderStyle.Thin;
            itemCellStyleDefaultValueSingleCellBorderFull.BorderRight = BorderStyle.Thin;
            itemCellStyleDefaultValueSingleCellBorderFull.WrapText = true;
            itemCellStyleDefaultValueSingleCellBorderFull.VerticalAlignment = VerticalAlignment.Top;
            itemCellStyleDefaultValueSingleCellBorderFull.Alignment = HorizontalAlignment.Right;
            itemCellStyleDefaultValueSingleCellBorderFull.SetFont(FontBold);

            HSSFCellStyle itemCellStyleDefaultValueBoldSingleCellBorderFull = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellStyleDefaultValueBoldSingleCellBorderFull.BorderBottom = BorderStyle.Thin;
            itemCellStyleDefaultValueBoldSingleCellBorderFull.BorderRight = BorderStyle.Thin;
            itemCellStyleDefaultValueBoldSingleCellBorderFull.WrapText = true;
            itemCellStyleDefaultValueBoldSingleCellBorderFull.VerticalAlignment = VerticalAlignment.Top;
            itemCellStyleDefaultValueBoldSingleCellBorderFull.Alignment = HorizontalAlignment.Right;
            itemCellStyleDefaultValueBoldSingleCellBorderFull.SetFont(FontBold);

            HSSFCellStyle itemCellStyleDefaultValueBoldColorLow = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellStyleDefaultValueBoldColorLow.BorderBottom = BorderStyle.Thin;
            itemCellStyleDefaultValueBoldColorLow.BorderRight = BorderStyle.Thin;
            itemCellStyleDefaultValueBoldColorLow.WrapText = true;
            itemCellStyleDefaultValueBoldColorLow.VerticalAlignment = VerticalAlignment.Top;
            itemCellStyleDefaultValueBoldColorLow.Alignment = HorizontalAlignment.Right;
            itemCellStyleDefaultValueBoldColorLow.SetFont(FontBold);
            itemCellStyleDefaultValueBoldColorLow.FillForegroundColor = HSSFColor.Grey25Percent.Index;
            itemCellStyleDefaultValueBoldColorLow.FillPattern = FillPattern.SolidForeground;

            HSSFCellStyle itemCellStyleDefaultValueBoldHi = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellStyleDefaultValueBoldHi.BorderBottom = BorderStyle.Thin;
            itemCellStyleDefaultValueBoldHi.BorderRight = BorderStyle.Thin;
            itemCellStyleDefaultValueBoldHi.WrapText = true;
            itemCellStyleDefaultValueBoldHi.VerticalAlignment = VerticalAlignment.Top;
            itemCellStyleDefaultValueBoldHi.Alignment = HorizontalAlignment.Right;
            itemCellStyleDefaultValueBoldHi.SetFont(FontBold);
            itemCellStyleDefaultValueBoldHi.FillForegroundColor = HSSFColor.Yellow.Index;
            itemCellStyleDefaultValueBoldHi.FillPattern = FillPattern.SolidForeground;


            // Create Style for Summary
            HSSFCellStyle itemCellSrtyleSummary = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellSrtyleSummary.BorderBottom = BorderStyle.Thin;
            //itemCellSrtyleSummary.BorderRight = BorderStyle.Thin;
            itemCellSrtyleSummary.WrapText = true;
            itemCellSrtyleSummary.VerticalAlignment = VerticalAlignment.Top;
            itemCellSrtyleSummary.Alignment = HorizontalAlignment.Left;
            itemCellSrtyleSummary.Indention = 2;
            itemCellSrtyleSummary.SetFont(FontBold);

            HSSFCellStyle itemCellSrtyleSummaryIndent = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellSrtyleSummaryIndent.BorderBottom = BorderStyle.Thin;
            //itemCellSrtyleSummaryIndent.BorderRight = BorderStyle.Thin;
            itemCellSrtyleSummaryIndent.WrapText = true;
            itemCellSrtyleSummaryIndent.VerticalAlignment = VerticalAlignment.Top;
            itemCellSrtyleSummaryIndent.Alignment = HorizontalAlignment.Left;
            itemCellSrtyleSummaryIndent.Indention = 4;
            itemCellSrtyleSummaryIndent.SetFont(FontReg);

            HSSFCellStyle itemCellSrtyleSummaryIndentBold = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellSrtyleSummaryIndentBold.BorderBottom = BorderStyle.Thin;
            //itemCellSrtyleSummaryIndentBold.BorderRight = BorderStyle.Thin;
            itemCellSrtyleSummaryIndentBold.WrapText = true;
            itemCellSrtyleSummaryIndentBold.VerticalAlignment = VerticalAlignment.Top;
            itemCellSrtyleSummaryIndentBold.Alignment = HorizontalAlignment.Left;
            itemCellSrtyleSummaryIndentBold.Indention = 4;
            itemCellSrtyleSummaryIndentBold.SetFont(FontBold);
            #endregion

            #region Row Data
            //Create Row Data
            int rowIndex = 4;
            int startUPAVendor = 4 + lst_Vendor.Count + 8 + 1;
            int endUPAVendor = 4 + lst_Vendor.Count + 8 + (lst_Vendor.Count * 4);
            int RABmaterialCellNo = 4 + lst_Vendor.Count + 1;
            int RABwageCellNo = 4 + lst_Vendor.Count + 2;
            int RABmiscCellNo = 4 + lst_Vendor.Count + 3;
            int RABTotalUPACellNo = 4 + lst_Vendor.Count + 4;          
            int VendorVolumeStart = 5;
            int VendorVolumeEnd = 4 + lst_Vendor.Count;
            int Column_No = 0;
            int Column_Structure =  1;
            int Column_Specification =  2;
            int Column_UoMDesc = 3;
            int Column_RABVolume = 4;
            bool additionalInfo = false;

            List<decimal> toleranceprice = GetValuePriceTolerance();            
            foreach (DataRow row in sourceTable.Rows)
            {
                //Style Setting
                HSSFRow m_objdataRow = (HSSFRow)m_objsheet.CreateRow(rowIndex);
                int NumberColumn = 0;                

                DataColumn RABMaterial = new DataColumn();
                DataColumn RABWage = new DataColumn();
                DataColumn RABMisc= new DataColumn();
                DataColumn RABTotalUp= new DataColumn();
               
                int indent = 0;
                bool isHiLevelBOI = false;
                bool firstCellisEmpty = false;
                bool isSummaryValueToBold = false;
                int countingVendorUPA = 1;
                string RABVolValue = "0";
                foreach (DataColumn column in sourceTable.Columns)
                {

                    // Set Cell With Style
                    if (!firstCellisEmpty)
                    {
                        if (NumberColumn == Column_No)
                        {
                            indent = row[column].ToString().Split('.').ToList().Count();
                            isHiLevelBOI = row[column].ToString() != string.Empty && indent < 2 ? true : false;
                            firstCellisEmpty = row[column].ToString() == string.Empty;
                            HSSFCell itemCell = (HSSFCell)m_objdataRow.CreateCell(column.Ordinal);
                            itemCell.SetCellValue(row[column].ToString());
                            if (indent == 1 && isHiLevelBOI == true)
                                itemCell.CellStyle = itemCellStyleCol1FontBold;
                            else
                                itemCell.CellStyle = itemCellStyleCol1FontReg;
                        }
                        else if (NumberColumn == Column_Structure)
                        {
                            HSSFCell itemCell = (HSSFCell)m_objdataRow.CreateCell(column.Ordinal);
                            itemCell.SetCellValue(row[column].ToString());
                            string containingValue = row[column].ToString().ToLower();

                            if (indent > 1)
                            {
                                if (lst_indentStyle.Count() < indent)
                                {
                                    HSSFCellStyle indentStyleAdditional = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
                                    indentStyleAdditional.BorderBottom = BorderStyle.Thin;
                                    indentStyleAdditional.BorderRight = BorderStyle.Thin;
                                    indentStyleAdditional.WrapText = true;
                                    indentStyleAdditional.VerticalAlignment = VerticalAlignment.Top;
                                    indentStyleAdditional.Alignment = HorizontalAlignment.Left;
                                    indentStyleAdditional.Indention = (short)(indent - 1);
                                    lst_indentStyle.Add(indentStyleAdditional);
                                }
                            }
                            itemCell.CellStyle = lst_indentStyle[indent - 1];

                        }
                        else if (NumberColumn == Column_Specification)
                        {
                            HSSFCell itemCell = (HSSFCell)m_objdataRow.CreateCell(column.Ordinal);
                            itemCell.SetCellValue(row[column].ToString());
                            itemCell.CellStyle = indent == 1 && isHiLevelBOI == true ? itemCellStyleCol3Bold : itemCellStyleCol3;
                        }
                        else if (NumberColumn == Column_UoMDesc)
                        {
                            HSSFCell itemCell = (HSSFCell)m_objdataRow.CreateCell(column.Ordinal);
                            itemCell.SetCellValue(row[column].ToString());
                            itemCell.CellStyle = itemCellStyleCol4;
                        }
                        else if (NumberColumn == Column_RABVolume)
                        {
                            RABVolValue = row[column].ToString();
                            HSSFCell itemCell = (HSSFCell)m_objdataRow.CreateCell(column.Ordinal);
                            itemCell.SetCellValue(row[column].ToString());
                            itemCell.CellStyle = itemCellStyleCol4;
                        }
                        else if (NumberColumn >= VendorVolumeStart && NumberColumn <= VendorVolumeEnd)
                        {
                            HSSFCell itemCell = (HSSFCell)m_objdataRow.CreateCell(column.Ordinal);
                            itemCell.SetCellValue(row[column].ToString());

                            //Compare here
                            if (!string.IsNullOrEmpty(RABVolValue) && !string.IsNullOrEmpty(row[column].ToString()))
                            {
                                decimal vendorVal = Convert.ToDecimal(row[column].ToString());
                                decimal rabVal = Convert.ToDecimal(RABVolValue);

                                //Vol Vendor != RAB 
                                if (vendorVal != rabVal)
                                    itemCell.CellStyle = itemCellStyleDefaultValueDifferenceVol;
                                else
                                    itemCell.CellStyle = itemCellStyleDefaultValue;
                            }
                            else
                                itemCell.CellStyle = itemCellStyleDefaultValue;

                            var builtinFormatId = HSSFDataFormat.GetBuiltinFormat("#,##0.00");
                        }
                        else if (NumberColumn >= startUPAVendor && NumberColumn <= endUPAVendor)
                        {
                            HSSFCell itemCell = (HSSFCell)m_objdataRow.CreateCell(column.Ordinal);
                            string RABval = "";
                            switch (countingVendorUPA)
                            {
                                case 1:
                                    RABval = row[RABMaterial].ToString();
                                    break;
                                case 2:
                                    RABval = row[RABWage].ToString();
                                    break;
                                case 3:
                                    RABval = row[RABMisc].ToString();
                                    break;
                                case 4:
                                    RABval = row[RABTotalUp].ToString();
                                    countingVendorUPA = 0;
                                    break;
                            }
                            countingVendorUPA++;

                            itemCell.SetCellValue(row[column].ToString());
                            //Compare here
                            if (!string.IsNullOrEmpty(row[column].ToString()) && !string.IsNullOrEmpty(RABval))
                            {
                                decimal vendorVal = Convert.ToDecimal(row[column].ToString());
                                decimal rabVal = Convert.ToDecimal(RABval);

                                //Vendor < RAB 
                                if (vendorVal < rabVal * (1 - toleranceprice[0]))
                                    itemCell.CellStyle = indent == 1 && (isHiLevelBOI == true) ? itemCellStyleDefaultValueBoldColorLow : itemCellStyleDefaultValueLow;
                                //Vendor > RAB
                                else if (vendorVal > rabVal * (1 + toleranceprice[1]))
                                    itemCell.CellStyle = indent == 1 && (isHiLevelBOI == true) ? itemCellStyleDefaultValueBoldHi : itemCellStyleDefaultValueHi;
                                //Vendor == RAB
                                else
                                    itemCell.CellStyle = indent == 1 && (isHiLevelBOI == true) ? itemCellStyleDefaultValueBold : itemCellStyleDefaultValue;
                            }
                            else
                            {
                                itemCell.CellStyle = indent == 1 && (isHiLevelBOI == true) ? itemCellStyleDefaultValueBold : itemCellStyleDefaultValue;
                            }

                            var builtinFormatId = HSSFDataFormat.GetBuiltinFormat("#,##0.00");
                        }
                        else
                        {
                            if (NumberColumn == RABmaterialCellNo)
                                RABMaterial = column;
                            else if (NumberColumn == RABwageCellNo)
                                RABWage = column;
                            else if (NumberColumn == RABmiscCellNo)
                                RABMisc = column;
                            else if (NumberColumn == RABTotalUPACellNo)
                                RABTotalUp = column;

                            HSSFCell itemCell = (HSSFCell)m_objdataRow.CreateCell(column.Ordinal);
                            itemCell.SetCellValue(row[column].ToString());

                            var builtinFormatId = HSSFDataFormat.GetBuiltinFormat("#,##0.00");
                            itemCell.CellStyle = indent == 1 && isHiLevelBOI == true  ? itemCellStyleDefaultValueBold : itemCellStyleDefaultValue;
                        }
                    }
                    else
                    {
                        if (NumberColumn == Column_Structure)
                        {
                            HSSFCell itemCell = (HSSFCell)m_objdataRow.CreateCell(column.Ordinal);
                            itemCell.SetCellValue(row[column].ToString());
                            string containingValue = row[column].ToString().ToLower();
                            if (containingValue.Contains("sub total"))
                            {
                                itemCell.SetCellValue("∑  Sub Total");
                                itemCell.CellStyle = itemCellSrtyleSummary;
                                isSummaryValueToBold = true;
                                additionalInfo = true;
                            }
                            else if (containingValue.Contains("pembulatan"))
                            {
                                itemCell.CellStyle = itemCellSrtyleSummaryIndentBold;
                                isSummaryValueToBold = true;
                            }
                            else
                                itemCell.CellStyle = itemCellSrtyleSummaryIndent;
                        }
                        else if (NumberColumn < endUPAVendor && NumberColumn > Column_Structure)
                        {
                            HSSFCell itemCell = (HSSFCell)m_objdataRow.CreateCell(column.Ordinal);
                            itemCell.SetCellValue("");
                            itemCell.CellStyle = additionalInfo ? itemCellStyleColTOBBOTTOM: itemCellStyleDefaultValue;
                        }
                        else
                        {
                            HSSFCell itemCell = (HSSFCell)m_objdataRow.CreateCell(column.Ordinal);
                            itemCell.SetCellValue(row[column].ToString());
                            var builtinFormatId = HSSFDataFormat.GetBuiltinFormat("#,##0.00");
                            itemCell.CellStyle = indent == 1 && (isSummaryValueToBold) ? itemCellStyleDefaultValueBoldSingleCellBorderFull : itemCellStyleDefaultValueSingleCellBorderFull;
                        }
                    }
                    NumberColumn++;
                }
                rowIndex++;               
            }
            #endregion
                        

            foreach (DataColumn column in sourceTable.Columns)
                m_objsheet.AutoSizeColumn(column.Ordinal);

            m_objsheet.SetColumnWidth(2, 10000);
            m_objWorkbook.Write(m_objmemoryStream);
            //Save to server before download
            //delete first
            DirectoryInfo di = new DirectoryInfo(Request.MapPath("~/Content/"));
            foreach (FileInfo filez in di.GetFiles())
            {
                string ext = Path.GetFileName(filez.FullName);//Path.GetExtension(filez.Extension);
                if (ext.ToLower().Contains("budget plan comparison") && Path.GetExtension(filez.Extension) == ".xls")
                    filez.Delete();
            }
            FileStream file = new FileStream(Server.MapPath("~/Content/" + fileName), FileMode.Create, FileAccess.Write);
            m_objmemoryStream.WriteTo(file);
            file.Close();
            m_objmemoryStream.Close();
            m_objmemoryStream.Flush();
        }

        #endregion

        #region Private Method
        private BudgetPlanVersionVM GetLastBudgetPlanVersion(string BudgetPlanID, string BudgetPlanTemplateID)
        {
            BudgetPlanVersionVM m_objBudgetPlanVersionVM = new BudgetPlanVersionVM();
            DBudgetPlanVersionDA m_objDBudgetPlanVersionDA = new DBudgetPlanVersionDA();
            m_objDBudgetPlanVersionDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVersionVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVM.Prop.BudgetPlanVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVM.Prop.StatusID.MapAlias);

            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            if (!string.IsNullOrEmpty(BudgetPlanID))
            {
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(BudgetPlanID);
                m_objFilter.Add(BudgetPlanVersionVM.Prop.BudgetPlanID.Map, m_lstFilter);
            }

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanTemplateID);
            m_objFilter.Add(BudgetPlanVM.Prop.BudgetPlanTemplateID.Map, m_lstFilter);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(BudgetPlanVersionVM.Prop.BudgetPlanVersion.Map, OrderDirection.Descending);


            Dictionary<int, DataSet> m_dicDBudgetPlanVersionDA = m_objDBudgetPlanVersionDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, m_dicOrder);
            if (m_objDBudgetPlanVersionDA.Message == string.Empty)
            {
                DataRow m_drDBudgetPlanVersionDA = m_dicDBudgetPlanVersionDA[0].Tables[0].Rows[0];
                m_objBudgetPlanVersionVM = new BudgetPlanVersionVM()
                {
                    BudgetPlanVersion = int.Parse(m_drDBudgetPlanVersionDA[BudgetPlanVersionVM.Prop.BudgetPlanVersion.Name].ToString()),
                    BudgetPlanID = m_drDBudgetPlanVersionDA[BudgetPlanVersionVM.Prop.BudgetPlanID.Name].ToString(),
                    StatusID = int.Parse(m_drDBudgetPlanVersionDA[BudgetPlanVersionVM.Prop.StatusID.Name].ToString())
                };
            }

            return m_objBudgetPlanVersionVM; ;
        }
        private List<ItemPriceVM> GetListItemPrice(string ItemID, string ProjectID, string ClusterID, string UnitTypeID, string FilterNotIncluded, ref string message, string ItemVersionChildID = "")
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
            if (!string.IsNullOrEmpty(ItemVersionChildID))
            {

                ItemID = GetItemVersionChild(ItemVersionChildID).ItemID;
            }

            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ItemID);
            m_objFilter.Add(ItemPriceVM.Prop.ItemID.Map, m_lstFilter);


            if (!string.IsNullOrEmpty(ProjectID))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(ProjectID);
                m_objFilter.Add(ItemPriceVM.Prop.ProjectID.Map, m_lstFilter);
            }

            if (!string.IsNullOrEmpty(ClusterID) && FilterNotIncluded != ItemPriceVM.Prop.ClusterID.Name)
            {

                if (!string.IsNullOrEmpty(UnitTypeID) && FilterNotIncluded != ItemPriceVM.Prop.UnitTypeID.Name)
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


            Dictionary<string, OrderDirection> m_objOrderBy = new Dictionary<string, OrderDirection>();
            m_objOrderBy.Add(ItemVM.Prop.ItemID.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicDItemPriceDA = m_objDItemPriceDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDItemPriceDA.Message == string.Empty)
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
        private ItemVersionChildVM GetFormula(string ItemVersionChildID)
        {

            DItemVersionChildDA m_objItemVersionChildDA = new DItemVersionChildDA();
            m_objItemVersionChildDA.ConnectionStringName = Global.ConnStrConfigName;
            object m_objDBConnection = m_objItemVersionChildDA.BeginConnection();

            ItemVersionChildVM m_objItemVersionChildVM = new ItemVersionChildVM();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ItemVersionChildID);
            m_objFilter.Add(ItemVersionChildVM.Prop.ItemVersionChildID.Map, m_lstFilter);


            //Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            //m_dicOrder.Add(ItemVersionChildVM.Prop.Sequence.Map, OrderDirection.Ascending);

            bool m_boolIsCount = false;
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemVersionChildID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildItemID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildVersion.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Sequence.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Formula.MapAlias);

            Dictionary<int, DataSet> m_dicMItemVersionDA = m_objItemVersionChildDA.SelectBC(0, 1, m_boolIsCount, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objItemVersionChildDA.Message == "")
            {

                DataRow m_drMItemVersionChildDA = m_dicMItemVersionDA[0].Tables[0].Rows[0];
                m_objItemVersionChildVM = new ItemVersionChildVM()
                {
                    ItemVersionChildID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemVersionChildID.Name].ToString(),
                    ChildItemID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemID.Name].ToString(),
                    ChildVersion = (int)m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildVersion.Name],
                    Sequence = (int)m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Sequence.Name],
                    Formula = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Formula.Name].ToString()
                };
            }

            return m_objItemVersionChildVM;
        }
        private string LoopChildOfChild(object p_DBConncetion, string ParentID, int ParentVersion, string LastSequenceDesc, string Formula)
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
        private List<ItemPriceVendorVM> GetListItemPriceVendor(string ItemPriceID)
        {

            List<ItemPriceVendorVM> m_lstItemPriceVendorVM = new List<ItemPriceVendorVM>();
            DItemPriceVendorDA m_objDItemPriceVendorDA = new DItemPriceVendorDA();
            m_objDItemPriceVendorDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemPriceVendorVM.Prop.ItemPriceID.MapAlias);
            m_lstSelect.Add(ItemPriceVendorVM.Prop.VendorDesc.MapAlias);
            m_lstSelect.Add(ItemPriceVendorVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(ItemPriceVendorVM.Prop.IsDefault.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ItemPriceID);
            m_objFilter.Add(ItemPriceVendorVM.Prop.ItemPriceID.Map, m_lstFilter);


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
                    VendorID = m_drDItemPriceVendorDA[ItemPriceVendorVM.Prop.VendorID.Name].ToString(),
                    VendorDesc = m_drDItemPriceVendorDA[ItemPriceVendorVM.Prop.VendorDesc.Name].ToString(),
                    IsDefault = bool.Parse(m_drDItemPriceVendorDA[ItemPriceVendorVM.Prop.IsDefault.Name].ToString())
                }).Distinct().ToList();
            }

            return m_lstItemPriceVendorVM;

        }
        private List<ItemPriceVendorPeriodVM> GetListItemPriceVendorPeriod(string ItemPriceID)
        {

            List<ItemPriceVendorPeriodVM> m_lstItemPriceVendorPeriodVM = new List<ItemPriceVendorPeriodVM>();
            DItemPriceVendorPeriodDA m_objDItemPriceVendorPeriodDA = new DItemPriceVendorPeriodDA();
            m_objDItemPriceVendorPeriodDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.ItemPriceID.MapAlias);
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.VendorDesc.MapAlias);
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.ValidFrom.MapAlias);
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.ValidTo.MapAlias);
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.CurrencyID.MapAlias);
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.CurrencyDesc.MapAlias);
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.Amount.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ItemPriceID);
            m_objFilter.Add(ItemPriceVendorPeriodVM.Prop.ItemPriceID.Map, m_lstFilter);


            Dictionary<string, OrderDirection> m_objOrderBy = new Dictionary<string, OrderDirection>();
            m_objOrderBy.Add(ItemVM.Prop.ItemID.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicDItemPriceVendorPeriodDA = m_objDItemPriceVendorPeriodDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDItemPriceVendorPeriodDA.Message == string.Empty)
            {
                m_lstItemPriceVendorPeriodVM = (
                from DataRow m_drDItemPriceVendorPeriodDA in m_dicDItemPriceVendorPeriodDA[0].Tables[0].Rows
                select new ItemPriceVendorPeriodVM()
                {
                    ItemPriceID = m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.ItemPriceID.Name].ToString(),
                    VendorID = m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.VendorID.Name].ToString(),
                    VendorDesc = m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.VendorDesc.Name].ToString(),
                    ValidFrom = DateTime.Parse(m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.ValidFrom.Name].ToString()),
                    ValidTo = DateTime.Parse(m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.ValidTo.Name].ToString()),
                    CurrencyDesc = m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.CurrencyDesc.Name].ToString(),
                    CurrencyID = m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.CurrencyID.Name].ToString(),
                    Amount = decimal.Parse(m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.Amount.Name].ToString())
                }).Distinct().ToList();
            }

            return m_lstItemPriceVendorPeriodVM;

        }
        private List<string> IsSaveValid(string Action, BudgetPlanVM objBudgetPlanVM)
        {
            List<string> m_lstReturn = new List<string>();

            if (string.IsNullOrEmpty(objBudgetPlanVM.Description))
                m_lstReturn.Add(BudgetPlanVM.Prop.Description.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (string.IsNullOrEmpty(objBudgetPlanVM.BudgetPlanTemplateDesc))
                m_lstReturn.Add(BudgetPlanVM.Prop.BudgetPlanTemplateDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (string.IsNullOrEmpty(objBudgetPlanVM.BudgetPlanTemplateID))
                m_lstReturn.Add(BudgetPlanVM.Prop.BudgetPlanTemplateID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (string.IsNullOrEmpty(objBudgetPlanVM.ProjectDesc))
                m_lstReturn.Add(BudgetPlanVM.Prop.ProjectDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (string.IsNullOrEmpty(objBudgetPlanVM.UnitTypeDesc))
                m_lstReturn.Add(BudgetPlanVM.Prop.UnitTypeDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (string.IsNullOrEmpty(objBudgetPlanVM.ClusterDesc))
                m_lstReturn.Add(BudgetPlanVM.Prop.ClusterDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (objBudgetPlanVM.Area <= 0)
                m_lstReturn.Add(BudgetPlanVM.Prop.Area.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            //if (objBudgetPlanVM.HasParameter)
            //    if (!objBudgetPlanVM.ListItemParameterVM.Any())
            //        m_lstReturn.Add(BudgetPlanVM.Prop.ListItemParameterVM.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            //    else
            //        foreach (ItemParameterVM objItemParameterVM in objBudgetPlanVM.ListItemParameterVM)
            //        {
            //            if (string.IsNullOrEmpty(objItemParameterVM.Value))
            //                m_lstReturn.Add(ItemParameterVM.Prop.Value.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            //        }
            //if (objBudgetPlanVM.HasPrice)
            //    if (!objBudgetPlanVM.ListItemPriceVM.Any())
            //        m_lstReturn.Add(BudgetPlanVM.Prop.ListItemPriceVM.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            //    else
            //        foreach (ItemPriceVM objItemPriceVM in objBudgetPlanVM.ListItemPriceVM)
            //        {
            //            if (string.IsNullOrEmpty(objItemPriceVM.PriceTypeID))
            //                m_lstReturn.Add(ItemPriceVM.Prop.PriceTypeID.Desc + " " + General.EnumDesc(MessageLib.mustFill));

            //        }

            return m_lstReturn;
        }
        private Dictionary<string, object> GetFormData(NameValueCollection parameters, string BudgetPlanID)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();
            m_dicReturn.Add(BudgetPlanVM.Prop.BudgetPlanID.Name, (parameters[BudgetPlanVM.Prop.BudgetPlanID.Name].ToString() == string.Empty ? BudgetPlanID : parameters[BudgetPlanVM.Prop.BudgetPlanID.Name]));
            m_dicReturn.Add(BudgetPlanVM.Prop.Description.Name, parameters[BudgetPlanVM.Prop.Description.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.BudgetPlanTemplateID.Name, parameters[BudgetPlanVM.Prop.BudgetPlanTemplateID.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.BudgetPlanTemplateDesc.Name, parameters[BudgetPlanVM.Prop.BudgetPlanTemplateDesc.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.BudgetPlanTypeDesc.Name, parameters[BudgetPlanVM.Prop.BudgetPlanTypeDesc.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.CompanyDesc.Name, parameters[BudgetPlanVM.Prop.CompanyDesc.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.RegionDesc.Name, parameters[BudgetPlanVM.Prop.RegionDesc.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.LocationDesc.Name, parameters[BudgetPlanVM.Prop.LocationDesc.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.DivisionDesc.Name, parameters[BudgetPlanVM.Prop.DivisionDesc.Name]);

            m_dicReturn.Add(BudgetPlanVM.Prop.ProjectID.Name, parameters[BudgetPlanVM.Prop.ProjectID.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.ProjectDesc.Name, parameters[BudgetPlanVM.Prop.ProjectDesc.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.ClusterID.Name, parameters[BudgetPlanVM.Prop.ClusterID.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.ClusterDesc.Name, parameters[BudgetPlanVM.Prop.ClusterDesc.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.UnitTypeID.Name, parameters[BudgetPlanVM.Prop.UnitTypeID.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.UnitTypeDesc.Name, parameters[BudgetPlanVM.Prop.UnitTypeDesc.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.Area.Name, parameters[BudgetPlanVM.Prop.Area.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.BudgetPlanVersion.Name, parameters[BudgetPlanVM.Prop.BudgetPlanVersion.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.CreatedDate.Name, parameters[BudgetPlanVM.Prop.CreatedDate.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.ModifiedDate.Name, parameters[BudgetPlanVM.Prop.ModifiedDate.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.StatusDesc.Name, parameters[BudgetPlanVM.Prop.StatusDesc.Name]);


            return m_dicReturn;
        }
        private List<BudgetPlanVersionStructureVM> GetListBudgetPlanStructure(string BudgetPlanID, ref string message)
        {

            List<BudgetPlanVersionStructureVM> m_lstBudgetPlanVersionStructureVM = new List<BudgetPlanVersionStructureVM>();
            DBudgetPlanVersionStructureDA m_objDBudgetPlanVersionStructureDA = new DBudgetPlanVersionStructureDA();
            m_objDBudgetPlanVersionStructureDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersionStructureID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ItemVersionChildID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Version.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Sequence.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ParentItemID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ParentItemDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ParentVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ParentSequence.MapAlias);

            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Volume.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Specification.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.MaterialAmount.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.WageAmount.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.MiscAmount.MapAlias);


            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanID);
            m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanID.Map, m_lstFilter);


            Dictionary<string, OrderDirection> m_objOrderBy = new Dictionary<string, OrderDirection>();
            m_objOrderBy.Add(BudgetPlanVM.Prop.BudgetPlanID.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicDBudgetPlanVersionStructureDA = m_objDBudgetPlanVersionStructureDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDBudgetPlanVersionStructureDA.Message == string.Empty)
            {
                m_lstBudgetPlanVersionStructureVM = (
                from DataRow m_drDBudgetPlanVersionStructureDA in m_dicDBudgetPlanVersionStructureDA[0].Tables[0].Rows
                select new BudgetPlanVersionStructureVM()
                {
                    BudgetPlanID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.BudgetPlanID.Name].ToString(),
                    BudgetPlanVersion = int.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.Name].ToString()),
                    ItemVersionChildID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ItemVersionChildID.Name].ToString(),
                    ItemID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ItemID.Name].ToString(),
                    ItemDesc = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ItemDesc.Name].ToString(),
                    Version = int.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Version.Name].ToString()),
                    Sequence = int.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Sequence.Name].ToString()),
                    ParentItemID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ParentItemID.Name].ToString(),
                    ParentItemDesc = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ParentItemDesc.Name].ToString(),
                    ParentVersion = int.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ParentVersion.Name].ToString()),
                    ParentSequence = int.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ParentSequence.Name].ToString()),
                    Volume = decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Volume.Name].ToString()),
                    Specification = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Specification.Name].ToString(),
                    MaterialAmount = decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name].ToString()),
                    WageAmount = decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.WageAmount.Name].ToString()),
                    MiscAmount = decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.MiscAmount.Name].ToString())

                }).Distinct().ToList();
            }
            else
                message = m_objDBudgetPlanVersionStructureDA.Message;

            return m_lstBudgetPlanVersionStructureVM;

        }
        private List<ConfigVM> GetFilterTemplateConfig(string filterDesc1, string filterKey3)
        {

            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilteru = new Dictionary<string, List<object>>();


            List<string> m_lstUPA = new List<string>();
            List<string> m_lstSelectb = new List<string>();
            m_lstSelectb.Add(ConfigVM.Prop.Key3.MapAlias);

            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name);
            m_objFilteru.Add(ConfigVM.Prop.Key1.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("BudgetPlanTemplate");
            m_objFilteru.Add(ConfigVM.Prop.Key2.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(filterKey3);
            m_objFilteru.Add(ConfigVM.Prop.Key3.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(filterDesc1);
            m_objFilteru.Add(ConfigVM.Prop.Desc1.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicUConfigDA = m_objUConfigDA.SelectBC(0, null, false, m_lstSelectb, m_objFilteru, null, null, null);
            List<ConfigVM> m_lstConfigVM = new List<ConfigVM>();
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
            return m_lstConfigVM;

        }
        private List<ConfigVM> GetFilterVersionConfig(string filterKey3)
        {

            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilteru = new Dictionary<string, List<object>>();


            List<string> m_lstUPA = new List<string>();
            List<string> m_lstSelectb = new List<string>();
            m_lstSelectb.Add(ConfigVM.Prop.Key3.MapAlias);

            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name);
            m_objFilteru.Add(ConfigVM.Prop.Key1.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("BudgetPlanVersionStructure");
            m_objFilteru.Add(ConfigVM.Prop.Key2.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(filterKey3);
            m_objFilteru.Add(ConfigVM.Prop.Key3.Map, m_lstFilter);


            Dictionary<int, DataSet> m_dicUConfigDA = m_objUConfigDA.SelectBC(0, null, false, m_lstSelectb, m_objFilteru, null, null, null);
            List<ConfigVM> m_lstConfigVM = new List<ConfigVM>();
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
            return m_lstConfigVM;

        }

        private BudgetPlanVM GetSelectedData(Dictionary<string, object> selected, string budgetPlanVersion, ref string message)
        {
            BudgetPlanVM m_objBudgetPlanVM = new BudgetPlanVM();
            TBudgetPlanDA m_objTBudgetPlanDA = new TBudgetPlanDA();
            m_objTBudgetPlanDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.Description.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanTemplateDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanTemplateID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanTypeDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.CompanyDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.RegionDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.LocationDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.DivisionDesc.MapAlias);

            m_lstSelect.Add(BudgetPlanVM.Prop.ProjectID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.ProjectDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.ClusterDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.ClusterID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.UnitTypeID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.UnitTypeDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVM.Prop.FeePercentage.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.Area.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.Unit.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.CreatedDate.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.ModifiedDate.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.StatusID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.StatusDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objBudgetPlanVM.IsKey(m_kvpSelectedRow.Key))
                {
                    List<object> m_lstFilter = new List<object>();


                    if (m_objBudgetPlanVM.IsKey(m_kvpSelectedRow.Key))
                    {
                        if (!string.IsNullOrEmpty(budgetPlanVersion) && m_kvpSelectedRow.Key == BudgetPlanVersionVM.Prop.BudgetPlanVersion.Name)
                        {

                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(budgetPlanVersion);
                            m_objFilter.Add(BudgetPlanVersionVM.Prop.BudgetPlanVersion.Map, m_lstFilter);
                        }
                        else
                        {
                            m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                            m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_kvpSelectedRow.Value);
                            m_objFilter.Add(BudgetPlanVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                        }
                    }

                }
            }


            Dictionary<int, DataSet> m_dicTBudgetPlanDA = m_objTBudgetPlanDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objTBudgetPlanDA.Message == string.Empty)
            {
                DataRow m_drTBudgetPlanDA = m_dicTBudgetPlanDA[0].Tables[0].Rows[0];
                m_objBudgetPlanVM.BudgetPlanID = m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanID.Name].ToString();
                m_objBudgetPlanVM.Description = m_drTBudgetPlanDA[BudgetPlanVM.Prop.Description.Name].ToString();
                m_objBudgetPlanVM.BudgetPlanTemplateID = m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanTemplateID.Name].ToString();
                m_objBudgetPlanVM.BudgetPlanTemplateDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanTemplateDesc.Name].ToString();
                m_objBudgetPlanVM.BudgetPlanTypeDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanTypeDesc.Name].ToString();
                m_objBudgetPlanVM.CompanyDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.CompanyDesc.Name].ToString();
                m_objBudgetPlanVM.RegionDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.RegionDesc.Name].ToString();
                m_objBudgetPlanVM.LocationDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.LocationDesc.Name].ToString();
                m_objBudgetPlanVM.DivisionDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.DivisionDesc.Name].ToString();
                m_objBudgetPlanVM.ProjectID = m_drTBudgetPlanDA[BudgetPlanVM.Prop.ProjectID.Name].ToString();
                m_objBudgetPlanVM.ProjectDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.ProjectDesc.Name].ToString();
                m_objBudgetPlanVM.ClusterID = m_drTBudgetPlanDA[BudgetPlanVM.Prop.ClusterID.Name].ToString();
                m_objBudgetPlanVM.ClusterDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.ClusterDesc.Name].ToString();
                m_objBudgetPlanVM.UnitTypeID = m_drTBudgetPlanDA[BudgetPlanVM.Prop.UnitTypeID.Name].ToString();
                m_objBudgetPlanVM.UnitTypeDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.UnitTypeDesc.Name].ToString();
                m_objBudgetPlanVM.Area = decimal.Parse(m_drTBudgetPlanDA[BudgetPlanVM.Prop.Area.Name].ToString());
                m_objBudgetPlanVM.Unit = decimal.Parse(m_drTBudgetPlanDA[BudgetPlanVM.Prop.Unit.Name].ToString());
                m_objBudgetPlanVM.FeePercentage = decimal.Parse(m_drTBudgetPlanDA[BudgetPlanVM.Prop.FeePercentage.Name].ToString());
                m_objBudgetPlanVM.BudgetPlanVersion = Convert.ToInt32(m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanVersion.Name].ToString());
                m_objBudgetPlanVM.CreatedDate = DateTime.Parse(m_drTBudgetPlanDA[BudgetPlanVM.Prop.CreatedDate.Name].ToString());
                m_objBudgetPlanVM.ModifiedDate = DateTime.Parse(m_drTBudgetPlanDA[BudgetPlanVM.Prop.ModifiedDate.Name].ToString());
                m_objBudgetPlanVM.StatusDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.StatusDesc.Name].ToString();
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objTBudgetPlanDA.Message;

            return m_objBudgetPlanVM;
        }
        private ItemVersionChildVM GetItemVersionChild(string ItemVersionChildID)
        {
            #region DItemVersionChild
            DItemVersionChildDA m_objDItemVersionChildDA = new DItemVersionChildDA();
            m_objDItemVersionChildDA.ConnectionStringName = Global.ConnStrConfigName;

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            if (string.IsNullOrEmpty(ItemVersionChildID))
            {
                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(ItemVersionChildID);
                m_objFilter.Add(ItemVersionChildVM.Prop.ItemVersionChildID.Map, m_lstFilter);
            }

            ItemVersionChildVM m_objItemVersionChildVM = new ItemVersionChildVM();

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

            //Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            //foreach (DataSorter m_dtsOrder in parameters.Sort)
            //    m_dicOrder.Add(ItemVersionChildVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

            Dictionary<int, DataSet> m_dicDItemVersionChildDA = m_objDItemVersionChildDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null, null);
            if (m_objDItemVersionChildDA.Message == string.Empty)
            {
                DataRow m_drDItemVersionChildDA = m_dicDItemVersionChildDA[0].Tables[0].Rows[0];
                m_objItemVersionChildVM = new ItemVersionChildVM()
                {
                    ItemID = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemID.Name].ToString(),
                    ItemDesc = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemDesc.Name].ToString(),
                    ItemTypeDesc = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemTypeDesc.Name].ToString(),
                    ItemGroupDesc = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemGroupDesc.Name].ToString(),
                    Version = (int)m_drDItemVersionChildDA[ItemVersionChildVM.Prop.Version.Name],
                    VersionDesc = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.VersionDesc.Name].ToString(),
                    UoMDesc = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.UoMDesc.Name].ToString(),
                    ItemTypeID = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemTypeID.Name].ToString(),

                    ChildItemID = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemID.Name].ToString(),
                    ChildItemDesc = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemDesc.Name].ToString(),
                    ChildVersion = int.Parse(m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ChildVersion.Name].ToString()),
                    Sequence = int.Parse(m_drDItemVersionChildDA[ItemVersionChildVM.Prop.Sequence.Name].ToString()),
                };
            }

            #endregion
            return m_objItemVersionChildVM;
        }


        #region Public Method
        public List<string> GetVendorList(string BudgetPlanID, int Version)
        {
            List<string> vendorlist = new List<string>();
            string message = "";
            List<BudgetPlanVersionVendorVM> ListBudgetPlanVersionVendorVM = new List<BudgetPlanVersionVendorVM>();
            ListBudgetPlanVersionVendorVM = GetListItemVendor(BudgetPlanID, Version, ref message);
            foreach (BudgetPlanVersionVendorVM cd in ListBudgetPlanVersionVendorVM)
            {
                if (cd.BudgetPlanID != null)
                    vendorlist.Add(cd.VendorID);
            }
            return vendorlist;

        }
        public List<BudgetPlanVersionStructureVM> getListVendorPrice(string BPlanID, int version, string vendor)
        {
            List<BudgetPlanVersionStructureVM> getListVendorPrice_ = new List<BudgetPlanVersionStructureVM>();

            BudgetPlanVersionStructureVM m_objBudgetPlanVersionStructureVM = new BudgetPlanVersionStructureVM();
            DBudgetPlanVersionStructureDA m_objDBudgetPlanVersionStructureDA = new DBudgetPlanVersionStructureDA();
            m_objDBudgetPlanVersionStructureDA.ConnectionStringName = Global.ConnStrConfigName;

            List<BudgetPlanVersionStructureVM> lstDBudgetPlanVersionStructureVM = new List<BudgetPlanVersionStructureVM>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BPlanID);
            m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(version);
            m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(vendor);
            m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersionVendorID.Map, m_lstFilter);

            //m_boolIsCount = false;
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersionVendorID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersionStructureID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.VendorVolume.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Sequence.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ParentItemID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ParentSequence.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Version.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ParentVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.UoMDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.VendorMaterialAmount.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.VendorMiscAmount.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.VendorWageAmount.MapAlias);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(BudgetPlanTemplateStructureVM.Prop.Sequence.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicDBudgetPlanVersionStructureDA = m_objDBudgetPlanVersionStructureDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, m_dicOrder);
            if (m_objDBudgetPlanVersionStructureDA.AffectedRows > 0)
            {
                foreach (DataRow dr_BPVersionStructure in m_dicDBudgetPlanVersionStructureDA[0].Tables[0].Rows) {
                    BudgetPlanVersionStructureVM BPVersionStructure = new BudgetPlanVersionStructureVM();
                    BPVersionStructure.BudgetPlanID = dr_BPVersionStructure[BudgetPlanVersionStructureVM.Prop.BudgetPlanID.Name].ToString();
                    BPVersionStructure.BudgetPlanVersion = Convert.ToInt16(dr_BPVersionStructure[BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.Name].ToString());
                    BPVersionStructure.BudgetPlanVersionVendorID = dr_BPVersionStructure[BudgetPlanVersionStructureVM.Prop.BudgetPlanVersionVendorID.Name].ToString();
                    BPVersionStructure.BudgetPlanVersionStructureID = dr_BPVersionStructure[BudgetPlanVersionStructureVM.Prop.BudgetPlanVersionStructureID.Name].ToString();
                    BPVersionStructure.VendorVolume = Convert.ToDecimal(dr_BPVersionStructure[BudgetPlanVersionStructureVM.Prop.VendorVolume.Name].ToString());
                    BPVersionStructure.ItemID = dr_BPVersionStructure[BudgetPlanVersionStructureVM.Prop.ItemID.Name].ToString();
                    BPVersionStructure.ParentItemID = dr_BPVersionStructure[BudgetPlanVersionStructureVM.Prop.ParentItemID.Name].ToString();
                    BPVersionStructure.Sequence = Convert.ToInt16(dr_BPVersionStructure[BudgetPlanVersionStructureVM.Prop.Sequence.Name].ToString());
                    BPVersionStructure.ParentSequence = Convert.ToInt16(dr_BPVersionStructure[BudgetPlanVersionStructureVM.Prop.ParentSequence.Name].ToString());
                    BPVersionStructure.Version = Convert.ToInt16(dr_BPVersionStructure[BudgetPlanVersionStructureVM.Prop.Version.Name].ToString());
                    BPVersionStructure.ParentVersion = Convert.ToInt16(dr_BPVersionStructure[BudgetPlanVersionStructureVM.Prop.ParentVersion.Name].ToString());
                    BPVersionStructure.UoMDesc = dr_BPVersionStructure[BudgetPlanVersionStructureVM.Prop.UoMDesc.Name].ToString();

                    if (!string.IsNullOrEmpty(dr_BPVersionStructure[BudgetPlanVersionStructureVM.Prop.VendorMaterialAmount.Name].ToString()))
                        BPVersionStructure.VendorMaterialAmount = Convert.ToDecimal(dr_BPVersionStructure[BudgetPlanVersionStructureVM.Prop.VendorMaterialAmount.Name].ToString());

                    if (!string.IsNullOrEmpty(dr_BPVersionStructure[BudgetPlanVersionStructureVM.Prop.VendorWageAmount.Name].ToString()))
                        BPVersionStructure.VendorWageAmount = Convert.ToDecimal(dr_BPVersionStructure[BudgetPlanVersionStructureVM.Prop.VendorWageAmount.Name].ToString());

                    if (!string.IsNullOrEmpty(dr_BPVersionStructure[BudgetPlanVersionStructureVM.Prop.VendorMiscAmount.Name].ToString()))
                        BPVersionStructure.VendorMiscAmount = Convert.ToDecimal(dr_BPVersionStructure[BudgetPlanVersionStructureVM.Prop.VendorMiscAmount.Name].ToString());
                    lstDBudgetPlanVersionStructureVM.Add(BPVersionStructure);

                }
            }
            #endregion
            return lstDBudgetPlanVersionStructureVM;
        }
        public List<string> GetListColor()
        {
            List<string> m_lstUPA = new List<string>();
            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilteru = new Dictionary<string, List<object>>();

            List<string> m_lstSelectb = new List<string>();
            m_lstSelectb.Add(ConfigVM.Prop.Desc1.MapAlias);


            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("ComparisonCellColor");
            m_objFilteru.Add(ConfigVM.Prop.Key1.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicUConfigDA = m_objUConfigDA.SelectBC(0, null, false, m_lstSelectb, m_objFilteru, null, null, null);
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

            return m_lstUPA;
        }
        private List<decimal> GetValuePriceTolerance()
        {
            List<decimal> m_lstToleranceValue = new List<decimal>();
            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilteru = new Dictionary<string, List<object>>();

            List<string> m_lstSelectb = new List<string>();
            m_lstSelectb.Add(ConfigVM.Prop.Desc1.MapAlias);


            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("ComparisonPriceMargin");
            m_objFilteru.Add(ConfigVM.Prop.Key1.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicUConfigDA = m_objUConfigDA.SelectBC(0, null, false, m_lstSelectb, m_objFilteru, null, null, null);
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
                m_lstToleranceValue.Add(Convert.ToDecimal(item.Desc1));
            }

            return m_lstToleranceValue;
        }
        public ActionResult LoadVendorPrice(string BudgetPlanID, int Version, string VendorID, decimal sizeValue, decimal totalUnit)
        {
            #region GetVendorList
            string message = "";

            List<VendorVM> lst_Vendor = new List<VendorVM>();
            List<BudgetPlanVersionVendorVM> ListBudgetPlanVersionVendorVM = new List<BudgetPlanVersionVendorVM>();
            ListBudgetPlanVersionVendorVM = GetListItemVendor(BudgetPlanID, Version, ref message);

            List<BudgetPlanVersionVendorVM> ListBudgetPlanVersionVendorVMs = new List<BudgetPlanVersionVendorVM>();

            
            foreach (BudgetPlanVersionVendorVM vndr in ListBudgetPlanVersionVendorVM)
            {
                //List<BudgetPlanVersionVendorVM> m_lstBudgetPlanVersionVendorVM = new List<BudgetPlanVersionVendorVM>();
                DBudgetPlanVersionStructureDA m_objDBudgetPlanVersionStructuresDA = new DBudgetPlanVersionStructureDA();
                m_objDBudgetPlanVersionStructuresDA.ConnectionStringName = Global.ConnStrConfigName;

                List<string> m_lstSelectS = new List<string>();
                m_lstSelectS.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersionStructureID.MapAlias);
                m_lstSelectS.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersionVendorID.MapAlias);
                m_lstSelectS.Add(BudgetPlanVersionEntryVM.Prop.MaterialAmount.MapAlias);
                m_lstSelectS.Add(BudgetPlanVersionEntryVM.Prop.MiscAmount.MapAlias);
                m_lstSelectS.Add(BudgetPlanVersionEntryVM.Prop.WageAmount.MapAlias);
                m_lstSelectS.Add(BudgetPlanVersionEntryVM.Prop.Volume.MapAlias);

                Dictionary<string, List<object>> m_objFilterS = new Dictionary<string, List<object>>();
                List<object> m_lstFilterS = new List<object>();
                m_lstFilterS.Add(Operator.Equals);
                m_lstFilterS.Add(vndr.BudgetPlanVersionVendorID);
                m_objFilterS.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersionVendorID.Map, m_lstFilterS);

                //m_lstFilterS = new List<object>();
                //m_lstFilterS.Add(Operator.Equals);
                //m_lstFilterS.Add(1);
                //m_objFilterS.Add(BudgetPlanVersionStructureVM.Prop.ParentSequence.Map, m_lstFilterS);

                Dictionary<int, DataSet> m_dicVersionVendorDA = m_objDBudgetPlanVersionStructuresDA.SelectBC_VendorSummary(0, null, false, m_lstSelectS, m_objFilterS, null, null, null);
                if (m_objDBudgetPlanVersionStructuresDA.AffectedRows != 0)
                {
                    decimal SubTotal = 0; 
                    foreach (DataRow dr in m_dicVersionVendorDA[0].Tables[0].Rows) {
                        decimal wage = Convert.ToDecimal(dr[BudgetPlanVersionEntryVM.Prop.WageAmount.Name].ToString());
                        decimal misc = Convert.ToDecimal(dr[BudgetPlanVersionEntryVM.Prop.MiscAmount.Name].ToString());
                        decimal mat = Convert.ToDecimal(dr[BudgetPlanVersionEntryVM.Prop.MaterialAmount.Name].ToString());
                        decimal volum = Convert.ToDecimal(dr[BudgetPlanVersionEntryVM.Prop.Volume.Name].ToString());
                        SubTotal+= (volum * (wage + misc + mat));
                    }
                    //vndr.RoundedTotal = TotalPembulatan;

                    decimal fees = vndr.FeePercentage > 0 ? (decimal)vndr.FeePercentage/100 : 0;
                    decimal RoundDown = (decimal)(SubTotal + (SubTotal * fees));
                    RoundDown = RoundDown / 1000;
                    RoundDown = Math.Floor(RoundDown);
                    RoundDown = RoundDown * 1000;
                    vndr.RoundedTotal = RoundDown;

                    ListBudgetPlanVersionVendorVMs.Add(vndr);
                }
            }
            //sort nya disini

            ListBudgetPlanVersionVendorVM = ListBudgetPlanVersionVendorVMs.OrderBy(o => o.RoundedTotal).ToList();

            #endregion
            #region GetVendorStructure

            BudgetPlanVersionStructureVM m_objBudgetPlanVersionStructureVM = new BudgetPlanVersionStructureVM();
            DBudgetPlanVersionStructureDA m_objDBudgetPlanVersionStructureDA = new DBudgetPlanVersionStructureDA();
            m_objDBudgetPlanVersionStructureDA.ConnectionStringName = Global.ConnStrConfigName;

            List<BudgetPlanVersionStructureVM> lstDBudgetPlanVersionStructureVM = new List<BudgetPlanVersionStructureVM>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanID);
            m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(Version);
            m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

            //m_boolIsCount = false;
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.MapAlias);
            //m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersionStructureID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Volume.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Sequence.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ParentItemID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ParentSequence.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Version.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ParentVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.MaterialAmount.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.MiscAmount.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.WageAmount.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.UoMDesc.MapAlias);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(BudgetPlanTemplateStructureVM.Prop.Sequence.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicDBudgetPlanVersionStructureDA = m_objDBudgetPlanVersionStructureDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, m_dicOrder);
            if (m_objDBudgetPlanVersionStructureDA.Message == string.Empty)
            {
                lstDBudgetPlanVersionStructureVM = (
                        from DataRow m_drDBudgetPlanVersionStructureDA in m_dicDBudgetPlanVersionStructureDA[0].Tables[0].Rows
                        select new BudgetPlanVersionStructureVM()
                        {
                            BudgetPlanID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.BudgetPlanID.Name].ToString(),
                            BudgetPlanVersion = Convert.ToInt16(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.Name].ToString()),
                            BudgetPlanVersionStructureID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.BudgetPlanVersionStructureID.Name].ToString(),
                            Volume = Convert.ToDecimal(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Volume.Name].ToString()),
                            ItemID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ItemID.Name].ToString(),
                            ParentItemID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ParentItemID.Name].ToString(),
                            Sequence = Convert.ToInt16(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Sequence.Name].ToString()),
                            ParentSequence = Convert.ToInt16(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ParentSequence.Name].ToString()),
                            Version = Convert.ToInt16(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Version.Name].ToString()),
                            ParentVersion = Convert.ToInt16(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ParentVersion.Name].ToString()),
                            MaterialAmount = Convert.ToDecimal(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name].ToString()),
                            WageAmount = Convert.ToDecimal(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.WageAmount.Name].ToString()),
                            UoMDesc = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.UoMDesc.Name].ToString(),
                            MiscAmount = Convert.ToDecimal(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.MiscAmount.Name].ToString())
                        }).ToList();
            }
            #endregion
            #region CreateNodeVendorPrice
            NodeCollection m_nodeCollectAllVendor = new Ext.Net.NodeCollection();
            foreach (BudgetPlanVersionVendorVM Vendor in ListBudgetPlanVersionVendorVM)
            {
                decimal? NettTotal = 0;
                decimal? ContractorFeesValue = Vendor.FeePercentage;
                List<BudgetPlanVersionStructureVM> lstVendorPrice = getListVendorPrice(BudgetPlanID, Version, Vendor.BudgetPlanVersionVendorID);
                  Node m_nodeCollectChild = new Ext.Net.Node();
                m_nodeCollectChild.AttributesObject = new
                {vendordesc = Vendor.VendorDesc};
                List<BudgetPlanVersionStructureVM> parentList = lstDBudgetPlanVersionStructureVM.AsEnumerable()
                                                                 .Where(x => x.ParentSequence == 0)
                                                                 .OrderBy(x => x.Sequence)
                                                                 .ToList();
                foreach (BudgetPlanVersionStructureVM item in parentList)
                {
                    //NettTotal = 0;                    
                    decimal? mat = 0;
                    decimal? wag = 0;
                    decimal? msc = 0;
                    decimal? tot = 0;
                    decimal? totup = 0;
                    decimal? volume = 0;
                    Node node = new Node();
                    bool islast = true;  
                    List<BudgetPlanVersionStructureVM> lstVendorPrice_ = lstVendorPrice.AsEnumerable()
                                 .Where(x => x.BudgetPlanVersionStructureID == item.BudgetPlanVersionStructureID)
                                 .ToList();
                    NodeCollection nodeChildCollection = new NodeCollection();
                    if (lstVendorPrice_.Any())
                    {
                        mat = lstVendorPrice_[0].VendorMaterialAmount;
                        wag = lstVendorPrice_[0].VendorWageAmount;
                        msc = lstVendorPrice_[0].VendorMiscAmount;
                        volume = lstVendorPrice_[0].VendorVolume;
                        nodeChildCollection = LoadChildBPVendorStructure(lstDBudgetPlanVersionStructureVM, lstVendorPrice, item.ItemID, item.Version, item.Sequence, ref tot, ref totup,ref islast);
                        tot = volume * ((mat ?? 0) + (wag ?? 0) + (msc ?? 0));
                        totup = (mat ?? 0) + (wag ?? 0) + (msc ?? 0);

                        node.AttributesObject = new
                        {
                            itemdesc = item.ItemDesc,
                            budgetplantemplateid = item.BudgetPlanTemplateID,
                            itemid = item.ItemID,
                            parentintemid = item.ParentItemID,
                            parentsequence = item.ParentSequence,
                            parentversion = item.ParentVersion,
                            sequence = item.Sequence,
                            version = item.Version,
                            materialamount = (mat==0 ? null : mat),
                            wageamount = (wag == 0 ? null : wag),
                            miscamount = (msc == 0 ? null : msc),
                            totalunitprice = totup == 0 ? null : totup,
                            total = tot == 0 ? null : tot,
                            uomdesc = item.UoMDesc,
                            volume = volume
                        };
                    }
                    else
                    {
                        mat = 0;
                        wag = 0;
                        msc = 0;
                        nodeChildCollection = LoadChildBPVendorStructure(lstDBudgetPlanVersionStructureVM, lstVendorPrice, item.ItemID, item.Version, item.Sequence, ref tot, ref totup, ref islast);

                        node.AttributesObject = new
                        {
                            itemdesc = item.ItemDesc,
                            budgetplantemplateid = item.BudgetPlanTemplateID,
                            itemid = item.ItemID,
                            parentintemid = item.ParentItemID,
                            parentsequence = item.ParentSequence,
                            parentversion = item.ParentVersion,
                            sequence = item.Sequence,
                            version = item.Version,
                            materialamount = (decimal?)null,
                            wageamount = (decimal?)null,
                            miscamount = (decimal?)null,
                            uomdesc = item.UoMDesc,
                            total = tot == 0 ? null : tot ,
                            totalunitprice = totup == 0 ? null : totup,
                            volume = (decimal?)null
                        };
                    }
                    node.Icon = Icon.Folder;
                    node.Expanded = nodeChildCollection.Count > 0;
                    node.Expandable = nodeChildCollection.Count > 0;

                    if (nodeChildCollection.Count > 0)
                        node.Children.AddRange(nodeChildCollection);

                    m_nodeCollectChild.Children.Add(node);
                    NettTotal += tot;
                }
                #region GrandTotal
                if (parentList.Count > 0)
                {
                    string ClsIcon = "display: none !important;";
                    Node BlankNode = new Node();
                    BlankNode.Expandable = false;
                    BlankNode.IconCls = ClsIcon;
                    BlankNode.AttributesObject = new { isGrandTotal = true };
                    m_nodeCollectChild.Children.Add(BlankNode);
                    
                    m_nodeCollectChild.Children.Add(GrandTotalNode(ClsIcon, NettTotal??0, ContractorFeesValue, sizeValue,totalUnit));

                }
                #endregion

                m_nodeCollectAllVendor.Add(m_nodeCollectChild);
            }
            #endregion
            return this.Store(m_nodeCollectAllVendor);
        }
        public NodeCollection LoadChildBPVendorStructure(List<BudgetPlanVersionStructureVM> dataParent, List<BudgetPlanVersionStructureVM> dataVendor, string ParentItemID, int ParentVersion, int ParentSequence, ref decimal? ParentTotal, ref decimal? ParentTotalUnitPrice ,ref bool islast_)
        {
            NodeCollection m_nodeCollectChild = new Ext.Net.NodeCollection();
            List<BudgetPlanVersionStructureVM> parentList = dataParent.AsEnumerable()
                                                             .Where(x => x.ParentItemID == ParentItemID)
                                                             .Where(x => x.ParentVersion == ParentVersion)
                                                             .Where(x => x.ParentSequence == ParentSequence)
                                                             .OrderBy(x => x.Sequence)
                                                             .ToList();

            foreach (BudgetPlanVersionStructureVM item in parentList)
            {
               
                decimal? Material = 0;
                decimal? Wage = 0;
                decimal? Misc = 0;
                decimal? Total = 0;
                decimal? TotalUnitPrice = 0;
                decimal? Volume = 0;
                Node node = new Node();
                List<BudgetPlanVersionStructureVM> dataVendor_ = dataVendor.AsEnumerable()
                             .Where(x => x.BudgetPlanVersionStructureID == item.BudgetPlanVersionStructureID)
                             .ToList();
                NodeCollection nodeChildCollection = new NodeCollection();

                if (dataVendor_.Any())
                {
                    islast_ = false;
                    Material = dataVendor_[0].VendorMaterialAmount;
                    Wage = dataVendor_[0].VendorWageAmount;
                    Misc = dataVendor_[0].VendorMiscAmount;
                    Volume = dataVendor_[0].VendorVolume;
                    bool islast = true;
                    nodeChildCollection = LoadChildBPVendorStructure(dataParent, dataVendor, item.ItemID, item.Version, item.Sequence, ref Total, ref TotalUnitPrice,ref islast);

                    TotalUnitPrice = (Material ?? 0) + (Wage ?? 0) + (Misc ?? 0);
                    Total = Volume * TotalUnitPrice;
                    ParentTotal += Total;
                    ParentTotalUnitPrice += TotalUnitPrice;
                    if (islast)
                    {
                        node.AttributesObject = new
                        {
                            itemdesc = item.ItemDesc,
                            budgetplantemplateid = item.BudgetPlanTemplateID,
                            itemid = item.ItemID,
                            parentintemid = item.ParentItemID,
                            parentsequence = item.ParentSequence,
                            parentversion = item.ParentVersion,
                            sequence = item.Sequence,
                            version = item.Version,
                            materialamount = (Material == 0 ? null : Material),
                            wageamount = (Wage == 0 ? null : Wage),
                            miscamount = (Misc == 0 ? null : Misc),
                            totalunitprice = nodeChildCollection.Count < 1 ? (TotalUnitPrice == 0 ? null : TotalUnitPrice) : TotalUnitPrice,
                            total = nodeChildCollection.Count < 1 ? (Total == 0 ? null : Total) : Total,
                            uomdesc = item.UoMDesc,
                            volume = Volume
                        };
                    }
                    else
                    {
                        node.AttributesObject = new
                        {
                            itemdesc = item.ItemDesc,
                            budgetplantemplateid = item.BudgetPlanTemplateID,
                            itemid = item.ItemID,
                            parentintemid = item.ParentItemID,
                            parentsequence = item.ParentSequence,
                            parentversion = item.ParentVersion,
                            sequence = item.Sequence,
                            version = item.Version,
                            materialamount = (Material == 0 ? null : Material),
                            wageamount = (Wage == 0 ? null : Wage),
                            miscamount = (Misc == 0 ? null : Misc),
                            totalunitprice = nodeChildCollection.Count < 1 ? (TotalUnitPrice == 0 ? null : TotalUnitPrice) : null,
                            total = nodeChildCollection.Count < 1 ? (Total == 0 ? null : Total) : null,
                            uomdesc = item.UoMDesc,
                            volume = Volume
                        };
                    }
                    
                }
                else
                {
                   
                    Material = 0;
                    Wage = 0;
                    Misc = 0;
                    nodeChildCollection = LoadChildBPVendorStructure(dataParent, dataVendor, item.ItemID, item.Version, item.Sequence, ref Total, ref TotalUnitPrice, ref islast_);
                    ParentTotal += Total;
                    ParentTotalUnitPrice += TotalUnitPrice;
                    node.AttributesObject = new
                    {
                        itemdesc = item.ItemDesc,
                        budgetplantemplateid = item.BudgetPlanTemplateID,
                        itemid = item.ItemID,
                        parentintemid = item.ParentItemID,
                        parentsequence = item.ParentSequence,
                        parentversion = item.ParentVersion,
                        sequence = item.Sequence,
                        version = item.Version,
                        materialamount = (decimal?)null,
                        wageamount = (decimal?)null,
                        miscamount = (decimal?)null,
                        uomdesc = item.UoMDesc,
                        volume = (decimal?)null,
                        total = nodeChildCollection.Count <1?( Total == 0 ? null: Total) : null,
                        totalunitprice = nodeChildCollection.Count < 1 ? (TotalUnitPrice == 0 ? null: TotalUnitPrice) : null
                    };
                }
                node.Icon = Icon.Folder;
                node.Expanded = nodeChildCollection.Count > 0;
                node.Expandable = nodeChildCollection.Count > 0;
                node.Children.AddRange(nodeChildCollection);
                m_nodeCollectChild.Add(node);
            }
            return m_nodeCollectChild;

        }
        private Node GrandTotalNode(string ClsIcon, decimal? RABTotal, decimal? ContractorFeesValue, decimal sizeValue,decimal TotalUnit)
        {

            List<string> NameOfChildBudgetPlanTotal = new List<string>();
            NameOfChildBudgetPlanTotal.Add(BudgetPlanVM.Prop.Subtotal.Desc);
            NameOfChildBudgetPlanTotal.Add(BudgetPlanVM.Prop.ContractorFee.Desc);
            NameOfChildBudgetPlanTotal.Add(BudgetPlanVM.Prop.Total.Desc);
            NameOfChildBudgetPlanTotal.Add(BudgetPlanVM.Prop.Rounding.Desc);
            NameOfChildBudgetPlanTotal.Add(BudgetPlanVM.Prop.GrandTotalExcPPN.Desc);
            NameOfChildBudgetPlanTotal.Add(BudgetPlanVM.Prop.PersentageByRAB.Desc);
            NameOfChildBudgetPlanTotal.Add(BudgetPlanVM.Prop.Tax.Desc);
            NameOfChildBudgetPlanTotal.Add(BudgetPlanVM.Prop.GrandTotalIncPPN.Desc);
            

            if (sizeValue > 1)
            {
                NameOfChildBudgetPlanTotal.Add(BudgetPlanVM.Prop.AreaSize.Desc);
                NameOfChildBudgetPlanTotal.Add(BudgetPlanVM.Prop.BasicPriceI.Desc);
                NameOfChildBudgetPlanTotal.Add(BudgetPlanVM.Prop.BasicPriceII.Desc);
                NameOfChildBudgetPlanTotal.Add(BudgetPlanVM.Prop.BasicPriceIII.Desc);
            }

            List<decimal> ValueOfChildBudgetPlanTotal = new List<decimal>();

            //Fee
            ValueOfChildBudgetPlanTotal.Add((decimal)ContractorFeesValue);

            //Total
            ValueOfChildBudgetPlanTotal.Add((decimal)(RABTotal + (RABTotal * ContractorFeesValue/100)));

            //Rounding
            decimal RoundDown = (decimal)(RABTotal + (RABTotal * ContractorFeesValue / 100));
            RoundDown = RoundDown / 1000;
            RoundDown = Math.Floor(RoundDown);
            RoundDown = RoundDown * 1000;
            ValueOfChildBudgetPlanTotal.Add(RoundDown);

            //GrandTotalExcTax
            decimal GrandTExcTax = RoundDown * TotalUnit;
            ValueOfChildBudgetPlanTotal.Add(GrandTExcTax);

            //PersentageByRAB
            ValueOfChildBudgetPlanTotal.Add(0);

            //Tax
            ValueOfChildBudgetPlanTotal.Add((decimal)(GrandTExcTax * Convert.ToDecimal(0.1)));

            //GrandTotalIncTax
            decimal GrandT = GrandTExcTax + ((decimal)(GrandTExcTax * Convert.ToDecimal(0.1)));
            ValueOfChildBudgetPlanTotal.Add(GrandT);

            if (sizeValue > 1)
            {
                //AreaSize
                ValueOfChildBudgetPlanTotal.Add(sizeValue);

                //BasicPI
                ValueOfChildBudgetPlanTotal.Add((decimal)RABTotal / sizeValue);

                //BasicPII
                //ValueOfChildBudgetPlanTotal.Add((decimal)(RoundDown + (RoundDown * ContractorFeesValue)) / sizeValue);
                ValueOfChildBudgetPlanTotal.Add((decimal)RoundDown / sizeValue);

                //BasicPIII
                //ValueOfChildBudgetPlanTotal.Add((RoundDown + ((decimal)(RoundDown * Convert.ToDecimal(0.1)))) / sizeValue);
                ValueOfChildBudgetPlanTotal.Add(GrandT / sizeValue);
            }
            Node BPlanTotal = new Node();
            BPlanTotal.Icon = Icon.Sum;
            BPlanTotal.Expanded = true;
            BPlanTotal.AttributesObject = new {
                numbering = "",
                itemdesc = NameOfChildBudgetPlanTotal[0],
                budgetplantemplateid = (decimal?)null,
                itemid = (decimal?)null,
                uomid ="",
                haschild=true,
                specification = "",
                parentintemid = (decimal?)null,
                parentsequence = (decimal?)null,
                parentversion = (decimal?)null,
                sequence = (decimal?)null,
                version = (decimal?)null,
                materialamount = (decimal?)null,
                wageamount = (decimal?)null,
                miscamount = (decimal?)null,
                uomdesc = (decimal?)null,
                volume = (decimal?)null,
                totalunitprice = (decimal?)null,
                new_materialamount = (decimal?)null,
                new_wageamount = (decimal?)null,
                new_miscamount = (decimal?)null,
                new_totalunitprice = (decimal?)null,
                new_total = (decimal?)null,
                isGrandTotal = true,
                total = RABTotal,
                itemgroupid = ""
            };

            for (int n = 1; n < NameOfChildBudgetPlanTotal.Count; n++)
            {
                Node childBPlanTotal = new Node();
                childBPlanTotal.Expandable = false;
                childBPlanTotal.IconCls = ClsIcon;
                childBPlanTotal.AttributesObject = new {
                    numbering = "",
                    itemdesc = NameOfChildBudgetPlanTotal[n],
                    budgetplantemplateid = (decimal?)null,
                    itemid = (decimal?)null,
                    uomid = "",
                    specification = "",
                    parentintemid = (decimal?)null,
                    parentsequence = (decimal?)null,
                    parentversion = (decimal?)null,
                    sequence = (decimal?)null,
                    version = (decimal?)null,
                    materialamount = (decimal?)null,
                    wageamount = (decimal?)null,
                    miscamount = (decimal?)null,
                    uomdesc = (decimal?)null,
                    volume = (decimal?)null,
                    totalunitprice = (decimal?)null,
                    new_materialamount = (decimal?)null,
                    new_wageamount = (decimal?)null,
                    new_miscamount = (decimal?)null,
                    new_totalunitprice = (decimal?)null,
                    new_total = (decimal?)null,
                    isGrandTotal = true,
                    total = ValueOfChildBudgetPlanTotal[n - 1],
                    itemgroupid = ""
                    //isGrandTotal = true,
                    //itemdesc = NameOfChildBudgetPlanTotal[n],
                    //total = ValueOfChildBudgetPlanTotal[n - 1]
                };
                BPlanTotal.Children.Add(childBPlanTotal);
            }

            return BPlanTotal;
        }
        public Node LoadTemplatePrice(string BudgetPlanID, string Version, string ProjectID, string ClusterID, string UnitTypeID, decimal AreaSize, decimal? FeePercentage, decimal TotalUnit)
        {
            #region Check AllowChild(isBOI)

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
            #region Check NotAllowChild(isAHS)

            List<string> m_lstUPAnotallow = new List<string>();
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
                m_lstUPAnotallow.Add(item.Key3);
            }
            #endregion
            #region GetTemplatePriceDB

            BudgetPlanVersionStructureVM m_objBudgetPlanVersionStructureVM = new BudgetPlanVersionStructureVM();
            DBudgetPlanVersionStructureDA m_objDBudgetPlanVersionStructureDA = new DBudgetPlanVersionStructureDA();
            m_objDBudgetPlanVersionStructureDA.ConnectionStringName = Global.ConnStrConfigName;

            List<BudgetPlanVersionStructureVM> lstDBudgetPlanVersionStructureVM = new List<BudgetPlanVersionStructureVM>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanID);
            m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(Version);
            m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

            //m_boolIsCount = false;
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.MapAlias);
            //m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersionStructureID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Volume.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ItemTypeID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Sequence.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ParentItemID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ParentSequence.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Version.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.UoMID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Specification.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ParentVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.MaterialAmount.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.MiscAmount.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.WageAmount.MapAlias);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(BudgetPlanTemplateStructureVM.Prop.Sequence.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicDBudgetPlanVersionStructureDA = m_objDBudgetPlanVersionStructureDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, m_dicOrder);
            if (m_objDBudgetPlanVersionStructureDA.Message == string.Empty)
            {
                lstDBudgetPlanVersionStructureVM = (
                        from DataRow m_drDBudgetPlanVersionStructureDA in m_dicDBudgetPlanVersionStructureDA[0].Tables[0].Rows
                        select new BudgetPlanVersionStructureVM()
                        {
                            BudgetPlanID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.BudgetPlanID.Name].ToString(),
                            BudgetPlanVersion = Convert.ToInt16(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.Name].ToString()),
                            //VendorID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.VendorID.Name].ToString(),
                            BudgetPlanVersionStructureID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.BudgetPlanVersionStructureID.Name].ToString(),
                            Volume = Convert.ToDecimal(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Volume.Name].ToString()),
                            ItemID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ItemID.Name].ToString(),
                            ItemTypeID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ItemTypeID.Name].ToString(),
                            ItemDesc = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ItemDesc.Name].ToString(),
                            ParentItemID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ParentItemID.Name].ToString(),
                            Sequence = Convert.ToInt16(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Sequence.Name].ToString()),
                            ParentSequence = Convert.ToInt16(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ParentSequence.Name].ToString()),
                            Version = Convert.ToInt16(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Version.Name].ToString()),
                            ParentVersion = Convert.ToInt16(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ParentVersion.Name].ToString()),
                            UoMID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.UoMID.Name].ToString(),
                            Specification = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Specification.Name].ToString(),
                            MaterialAmount = Convert.ToDecimal(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name].ToString()),
                            WageAmount = Convert.ToDecimal(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.WageAmount.Name].ToString()),
                            MiscAmount = Convert.ToDecimal(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.MiscAmount.Name].ToString())
                        }).ToList();
            }
            #endregion
            #region CreateNodeTemplatePrice
            //Assumed
            decimal? RABTotal = 0;
            decimal? ContractorFeesValue = FeePercentage;
            

            ItemPriceVM _itemprice = new ItemPriceVM() { ProjectID = ProjectID, ClusterID = ClusterID, UnitTypeID = UnitTypeID };
            Node m_nodeCollectAllVendor = new Node();
            m_nodeCollectAllVendor.NodeID = "root";
            m_nodeCollectAllVendor.Expanded = false;
            int vendorcount = 0;
            m_nodeCollectAllVendor.AttributesObject = new
            {
                itemdesc = "BudgetPlanComparison",
                enabledefault = false,
                vendorCount = vendorcount
            };

            NodeCollection m_nodeCollectChild = new Ext.Net.NodeCollection();
            List<BudgetPlanVersionStructureVM> ParentList = lstDBudgetPlanVersionStructureVM.AsEnumerable()
                                                             .Where(x => x.ParentSequence == 0)
                                                             .OrderBy(x => x.Sequence)
                                                             .ToList();
            int Numbering = 1;
            foreach (BudgetPlanVersionStructureVM item in ParentList)
            {
                string numbering_ = Numbering.ToString();
                decimal? Material = 0;
                decimal? Wage = 0;
                decimal? Misc = 0;

                decimal? NewMaterial = 0;
                decimal? NewWage = 0;
                decimal? NewMisc = 0;

                decimal? GrandTotal = 0;
                decimal? LatestGrandTotal = 0;
                decimal? ValueTotalUnitPrice = 0;
                decimal? ValueLatestTotalUnitPrice = 0;

                bool? AllowChild = null;

                Node node = new Node();
                foreach (string UPA in m_lstUPA)
                {
                    if (item.ItemTypeID == UPA)
                    {
                        AllowChild = true;
                        node.Icon = Icon.Folder;
                        break;
                    }
                }
                foreach (string upa in m_lstUPAnotallow)
                {
                    if (item.ItemTypeID == upa)
                    {
                        AllowChild = false;
                        node.Icon = Icon.Table;
                        break;
                    }
                }
                NewMaterial = (GetUnitPrice(null, item.ItemID, _itemprice, item.ItemTypeID, BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name)) ?? 0;
                NewWage = (GetUnitPrice(null, item.ItemID, _itemprice, item.ItemTypeID, BudgetPlanVersionStructureVM.Prop.WageAmount.Name)) ?? 0;
                NewMisc = (GetUnitPrice(null, item.ItemID, _itemprice, item.ItemTypeID, BudgetPlanVersionStructureVM.Prop.MiscAmount.Name)) ?? 0;

                //node.Icon = allowchild ? Icon.Folder : Icon.Table;
                Material = item.MaterialAmount;
                Wage = item.WageAmount;
                Misc = item.MiscAmount;

                NodeCollection nodeChildCollection = LoadChildBPTemplate(numbering_, lstDBudgetPlanVersionStructureVM, m_lstUPA, m_lstUPAnotallow, item.ItemID, item.Version, item.Sequence, ref NewMaterial, ref NewWage, ref NewMisc, ref GrandTotal, ref LatestGrandTotal, ref ValueTotalUnitPrice, ref ValueLatestTotalUnitPrice, _itemprice);

                //node.Expanded = nodeChildCollection.Count > 0;
                decimal? Total = item.Volume * (Material + Wage + Misc);
                decimal? LatestTotalUnitPrice = (NewMaterial ?? 0) + (NewWage ?? 0) + (NewMisc ?? 0);
                decimal? TotalUnitPrice = Material + Wage + Misc;
                decimal? LatestTotal = item.Volume * ((NewMaterial ?? 0) + (NewWage ?? 0) + (NewMisc ?? 0));

                if (nodeChildCollection.Count > 0)
                {
                    Total = GrandTotal;
                    LatestTotal = LatestGrandTotal;
                    item.Volume = null;
                    item.UoMID = null;
                    //TotalUnitPrice = ValueTotalUnitPrice; // edit byhq, set total unit price to null
                    TotalUnitPrice = null;
                    LatestTotalUnitPrice = ValueLatestTotalUnitPrice;
                    Material = null;
                    Wage = null;
                    Misc = null;
                    NewMaterial = 0;
                    NewWage = 0;
                    NewMisc = 0;
                }

                Material = Material == 0 ? null : Material;
                Wage = Wage == 0 ? null : Wage;
                Misc = Misc == 0 ? null : Misc;

                NewMaterial = NewMaterial == 0 ? null : NewMaterial;
                NewWage = NewWage == 0 ? null : NewWage;
                NewMisc = NewMisc == 0 ? null : NewMisc;

                node.Expanded = nodeChildCollection.Count > 0;
                node.Expandable = nodeChildCollection.Count > 0;

                node.AttributesObject = new
                {
                    numbering = numbering_,
                    itemdesc = item.ItemDesc,
                    budgetplantemplateid = item.BudgetPlanTemplateID,
                    itemid = item.ItemID,
                    haschild = AllowChild,
                    parentintemid = item.ParentItemID,
                    parentsequence = item.ParentSequence,
                    parentversion = item.ParentVersion,
                    sequence = item.Sequence,
                    version = item.Version,
                    volume = item.Volume,
                    materialamount = Material,
                    wageamount = Wage,
                    miscamount = Misc,
                    totalunitprice = TotalUnitPrice,
                    total = Total,
                    uomid = item.UoMID,
                    specification = item.Specification,
                    new_materialamount = (decimal?)NewMaterial,
                    new_wageamount = (decimal?)NewWage,
                    new_miscamount = (decimal?)NewMisc,
                    new_totalunitprice = LatestTotalUnitPrice,
                    new_total = LatestTotal
                };

                //node.AttributesObject.GetType().GetProperties().SetValue()[0].

                if (nodeChildCollection.Count > 0)
                    node.Children.AddRange(nodeChildCollection);

                if (AllowChild == true)
                {
                    // node.font= new Font(treeView1.Font, FontStyle.Bold);
                    RABTotal += Total;
                    m_nodeCollectChild.Add(node);
                }
                Numbering++;
            }
            #endregion
            #region GrandTotal
            if (ParentList.Count > 0)
            {
                string ClsIcon = "display: none !important;";
                Node BlankNode = new Node();
                BlankNode.Expandable = false;
                BlankNode.IconCls = ClsIcon;
                BlankNode.AttributesObject = new { isGrandTotal = true };
                m_nodeCollectChild.Add(BlankNode);
                m_nodeCollectChild.Add(GrandTotalNode(ClsIcon, RABTotal, ContractorFeesValue, AreaSize, TotalUnit));
            }
            #endregion
            m_nodeCollectAllVendor.Children.AddRange(m_nodeCollectChild);
            return m_nodeCollectAllVendor;
        }
        public NodeCollection LoadChildBPTemplate(string NumberingParent, List<BudgetPlanVersionStructureVM> dataParent, List<string> m_lstUPA, List<string> m_lstUPAnotallow, string ParentItemID, int ParentVersion, int ParentSequence, ref decimal? ParentLatestMaterial, ref decimal? ParentLatestWage, ref decimal? ParentLastestMisc, ref decimal? ParentGrandTotal, ref decimal? ParentLatestGrandTotal, ref decimal? ParentTotalUnitPrice, ref decimal? ParentLatestTotalUnitPrice, ItemPriceVM ItemPriceParent)
        {
            NodeCollection m_nodeCollectChild = new Ext.Net.NodeCollection();
            List<BudgetPlanVersionStructureVM> parentList = dataParent.AsEnumerable()
                                                             .Where(x => x.ParentItemID == ParentItemID)
                                                             .Where(x => x.ParentVersion == ParentVersion)
                                                             .Where(x => x.ParentSequence == ParentSequence)
                                                             .OrderBy(x => x.Sequence)
                                                             .ToList();
            int Number = 1;
            foreach (BudgetPlanVersionStructureVM item in parentList)
            {
                string Numbering = Number.ToString();
                bool? AllowChild = null;

                decimal? Material = item.MaterialAmount;
                decimal? Wage = item.WageAmount;
                decimal? Misc = item.MiscAmount;

                decimal? NewMaterial = 0;
                decimal? NewWage = 0;
                decimal? NewMisc = 0;

                decimal? GrandTotal = 0;
                decimal? LatestGrandTotal = 0;
                decimal? ValueOfTotalUnitPrice = 0;
                decimal? ValueOfLatestTotalUnitPrice = 0;

                Node node = new Node();
                foreach (string upa in m_lstUPA)
                {
                    if (item.ItemTypeID == upa)
                    {
                        AllowChild = true;
                        node.Icon = Icon.Folder;
                        break;
                    }
                }
                foreach (string upa in m_lstUPAnotallow)
                {
                    if (item.ItemTypeID == upa)
                    {
                        AllowChild = false;
                        node.Icon = Icon.Table;
                        break;
                    }
                }
                if (AllowChild == null)
                {
                    node.Icon = Icon.PageWhite;
                }
                NewMaterial = (GetUnitPrice(string.Empty, item.ItemID, ItemPriceParent, item.ItemTypeID, BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name)) ?? 0;
                NewWage = (GetUnitPrice(string.Empty, item.ItemID, ItemPriceParent, item.ItemTypeID, BudgetPlanVersionStructureVM.Prop.WageAmount.Name)) ?? 0;
                NewMisc = (GetUnitPrice(string.Empty, item.ItemID, ItemPriceParent, item.ItemTypeID, BudgetPlanVersionStructureVM.Prop.MiscAmount.Name)) ?? 0;
                
                NodeCollection nodeChildCollection = LoadChildBPTemplate(NumberingParent + "." + Numbering, dataParent, m_lstUPA, m_lstUPAnotallow, item.ItemID, item.Version, item.Sequence, ref NewMaterial, ref NewWage, ref NewMisc, ref GrandTotal, ref LatestGrandTotal, ref ValueOfTotalUnitPrice, ref ValueOfLatestTotalUnitPrice, ItemPriceParent);

                decimal? Total = item.Volume * (Material + Wage + Misc);
                decimal? LatestTotalUnitPrice = (NewMaterial ?? 0) + (NewWage ?? 0) + (NewMisc ?? 0);
                decimal? TotalUnitPrice = Material + Wage + Misc;
                decimal? LatestTotal = item.Volume * ((NewMaterial ?? 0) + (NewWage ?? 0) + (NewMisc ?? 0));

                if (nodeChildCollection.Count > 0)
                {
                    Total = GrandTotal;
                    LatestTotal = LatestGrandTotal;
                    item.Volume = null;
                    item.UoMID = null;
                    TotalUnitPrice = ValueOfTotalUnitPrice;
                    LatestTotalUnitPrice = ValueOfLatestTotalUnitPrice;
                    Material = null;
                    Wage = null;
                    Misc = null;
                    NewMaterial = 0;
                    NewWage = 0;
                    NewMisc = 0;
                }

                ParentGrandTotal += (Total ?? 0);
                ParentLatestGrandTotal += (LatestTotal ?? 0);
                ParentTotalUnitPrice += (TotalUnitPrice ?? 0);
                ParentLatestTotalUnitPrice += (LatestTotalUnitPrice ?? 0);

                ParentLatestMaterial += (NewMaterial ?? 0);
                NewMaterial = NewMaterial == 0 ? null : NewMaterial;

                ParentLatestWage += (NewWage ?? 0);
                NewWage = NewWage == 0 ? null : NewWage;

                ParentLastestMisc += (NewMisc ?? 0);
                NewMisc = NewMisc == 0 ? null : NewMisc;
                Material = Material == 0 ? null : Material;
                Wage = Wage == 0 ? null : Wage;
                Misc = Misc == 0 ? null : Misc;

                node.AttributesObject = new
                {
                    numbering = NumberingParent + "." + Numbering,
                    itemdesc = item.ItemDesc,
                    budgetplantemplateid = item.BudgetPlanTemplateID,
                    itemid = item.ItemID,
                    haschild = AllowChild,
                    parentintemid = item.ParentItemID,
                    parentsequence = item.ParentSequence,
                    parentversion = item.ParentVersion,
                    sequence = item.Sequence,
                    version = item.Version,
                    volume = item.Volume,
                    materialamount = Material,
                    wageamount = Wage,
                    miscamount = Misc,
                    totalunitprice = nodeChildCollection.Count < 1 ? TotalUnitPrice:null,
                    total = nodeChildCollection.Count<1? Total : null,
                    uomid = item.UoMID,
                    specification = item.Specification,
                    new_materialamount = (decimal?)NewMaterial,
                    new_wageamount = (decimal?)NewWage,
                    new_miscamount = (decimal?)NewMisc,
                    new_totalunitprice = nodeChildCollection.Count < 1 ? LatestTotalUnitPrice : null,
                    new_total = nodeChildCollection.Count < 1 ? LatestTotal : null
                };
                node.Expanded = nodeChildCollection.Count > 0;
                node.Expandable = nodeChildCollection.Count > 0;
                node.Children.AddRange(nodeChildCollection);


                if (AllowChild == true)
                    m_nodeCollectChild.Add(node);

                Number++;
            }
            return m_nodeCollectChild;

        }
        public Dictionary<int, List<BudgetPlanVM>> GetBudgetPlanData(bool isCount, string BudgetPlanID, string Description)
        {
            int m_intCount = 0;
            List<BudgetPlanVM> m_lstBudgetPlanVM = new List<ViewModels.BudgetPlanVM>();
            Dictionary<int, List<BudgetPlanVM>> m_dicReturn = new Dictionary<int, List<BudgetPlanVM>>();

            List<string> m_lstSelect = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();

            TBudgetPlanDA m_objTBudgetPlanDA = new TBudgetPlanDA();
            m_objTBudgetPlanDA.ConnectionStringName = Global.ConnStrConfigName;

            m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanTypeID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanTypeDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.StatusID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.StatusDesc.MapAlias);

            m_objFilter = new Dictionary<string, List<object>>();
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(BudgetPlanID);
            m_objFilter.Add(BudgetPlanVM.Prop.BudgetPlanID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(Description);
            m_objFilter.Add(BudgetPlanVM.Prop.Description.Map, m_lstFilter);


            Dictionary<int, DataSet> m_dicTBudgetPlanDA = m_objTBudgetPlanDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objTBudgetPlanDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpItemBL in m_dicTBudgetPlanDA)
                    {
                        m_intCount = m_kvpItemBL.Key;
                        break;
                    }
                else
                {
                    m_lstBudgetPlanVM = (
                        from DataRow m_drTBudgetPlanDA in m_dicTBudgetPlanDA[0].Tables[0].Rows
                        select new BudgetPlanVM()
                        {
                            BudgetPlanID = m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanID.Name].ToString(),
                            Description = m_drTBudgetPlanDA[BudgetPlanVM.Prop.Description.Name].ToString(),
                            StatusID = int.Parse(m_drTBudgetPlanDA[BudgetPlanVM.Prop.StatusID.Name].ToString()),
                            StatusDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.StatusDesc.Name].ToString(),
                            BudgetPlanTypeID = m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanTypeID.Name].ToString(),
                            BudgetPlanTypeDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanTypeDesc.Name].ToString(),
                            BudgetPlanVersion = int.Parse(m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanVersion.Name].ToString())
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstBudgetPlanVM);
            return m_dicReturn;
        }

        #endregion
    }
}