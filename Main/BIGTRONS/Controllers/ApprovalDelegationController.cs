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

namespace com.SML.BIGTRONS.Controllers
{
    public class ApprovalDelegationController : BaseController
    {
        private readonly string title = "Approval Delegation";
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
            MApprovalDelegationDA m_objMApprovalDelegationDA = new MApprovalDelegationDA();
            m_objMApprovalDelegationDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMApprovalDelegation = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMApprovalDelegation.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = ApprovalDelegationVM.Prop.Map(m_strDataIndex, false);
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
            
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(System.Web.HttpContext.Current.User.Identity.Name);
            m_objFilter.Add(ApprovalDelegationVM.Prop.UserID.Map, m_lstFilter);
            Dictionary<int, DataSet> m_dicMApprovalDelegationDA = m_objMApprovalDelegationDA.SelectDistinctBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpApprovalDelegationBL in m_dicMApprovalDelegationDA)
            {
                m_intCount = m_kvpApprovalDelegationBL.Key;
                break;
            }

            List<ApprovalDelegationVM> m_lstApprovalDelegationVM = new List<ApprovalDelegationVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(ApprovalDelegationVM.Prop.TaskGroupID.MapAlias);
                m_lstSelect.Add(ApprovalDelegationVM.Prop.TaskGroupDesc.MapAlias);
                m_lstSelect.Add(ApprovalDelegationVM.Prop.PeriodStart.MapAlias);
                m_lstSelect.Add(ApprovalDelegationVM.Prop.PeriodEnd.MapAlias);
                m_lstSelect.Add(ApprovalDelegationVM.Prop.UserID.MapAlias);
                m_lstSelect.Add(ApprovalDelegationVM.Prop.OwnerName.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(ApprovalDelegationVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMApprovalDelegationDA = m_objMApprovalDelegationDA.SelectDistinctBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMApprovalDelegationDA.Message == string.Empty)
                {
                    string message = "";
                    m_lstApprovalDelegationVM = (
                        from DataRow m_drMApprovalDelegationDA in m_dicMApprovalDelegationDA[0].Tables[0].Rows
                        select new ApprovalDelegationVM()
                        {
                            UserID = m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.UserID.Name].ToString(),
                            TaskGroupID = m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.TaskGroupID.Name].ToString(),
                            TaskGroupDesc = m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.TaskGroupDesc.Name].ToString(),
                            OwnerName = m_drMApprovalDelegationDA["FullName"].ToString(),
                            PeriodStart = (DateTime)m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.PeriodStart.Name],
                            PeriodEnd = (DateTime)m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.PeriodEnd.Name],
                            ListApprovalDelegateID = GetListApprovalDelegationID(m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.TaskGroupID.Name].ToString(), (DateTime)m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.PeriodStart.Name], (DateTime)m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.PeriodEnd.Name], ref message),
                            lstApprovalDelegationUser = GetListDelegationUser(m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.TaskGroupID.Name].ToString(), (DateTime)m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.PeriodStart.Name], (DateTime)m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.PeriodEnd.Name], ref message),
                            lstTaskType = GetListTaskType(m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.TaskGroupID.Name].ToString(), (DateTime)m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.PeriodStart.Name], (DateTime)m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.PeriodEnd.Name] ,ref message)
                        }
                    ).ToList();

                }
            }
            return this.Store(m_lstApprovalDelegationVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            MApprovalDelegationDA m_objMApprovalDelegationDA = new MApprovalDelegationDA();
            m_objMApprovalDelegationDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMApprovalDelegation = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMApprovalDelegation.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = ApprovalDelegationVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMApprovalDelegationDA = m_objMApprovalDelegationDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpApprovalDelegationBL in m_dicMApprovalDelegationDA)
            {
                m_intCount = m_kvpApprovalDelegationBL.Key;
                break;
            }

            List<ApprovalDelegationVM> m_lstApprovalDelegationVM = new List<ApprovalDelegationVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(ApprovalDelegationVM.Prop.ApprovalDelegateID.MapAlias);
                m_lstSelect.Add(ApprovalDelegationVM.Prop.UserID.MapAlias);
                m_lstSelect.Add(ApprovalDelegationVM.Prop.TaskTypeID.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(ApprovalDelegationVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMApprovalDelegationDA = m_objMApprovalDelegationDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMApprovalDelegationDA.Message == string.Empty)
                {
                    m_lstApprovalDelegationVM = (
                        from DataRow m_drMApprovalDelegationDA in m_dicMApprovalDelegationDA[0].Tables[0].Rows
                        select new ApprovalDelegationVM()
                        {
                            ApprovalDelegateID = m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.ApprovalDelegateID.Name].ToString(),
                            UserID = m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.UserID.Name].ToString(),
                            TaskTypeID = m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.TaskTypeID.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstApprovalDelegationVM, m_intCount);
        }
        public ActionResult ReadBrowseTaskGroup(StoreRequestParameters parameters)
        {
            MTaskTypesDA m_objMTaskTypesDA = new MTaskTypesDA();
            m_objMTaskTypesDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;
            List<object> m_lstFilter = new List<object>();
            FilterHeaderConditions m_fhcMTaskTypes = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMTaskTypes.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = TaskTypesVM.Prop.Map(m_strDataIndex, false);
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


            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.None);
            m_lstFilter.Add("");
            m_objFilter.Add("(MTaskTypes.TaskGroupID IS NOT NULL)", m_lstFilter);

            Dictionary<int, DataSet> m_dicMTaskTypesDA = m_objMTaskTypesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpTaskTypesBL in m_dicMTaskTypesDA)
            {
                m_intCount = m_kvpTaskTypesBL.Key;
                break;
            }

            List<TaskTypesVM> m_lstTaskTypesVM = new List<TaskTypesVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(TaskTypesVM.Prop.TaskGroupDesc.MapAlias);
                m_lstSelect.Add(TaskTypesVM.Prop.TaskGroupID.MapAlias);

                List<string> m_lstGroup = new List<string>();
                m_lstGroup.Add(TaskTypesVM.Prop.TaskGroupDesc.Map);
                m_lstGroup.Add(TaskTypesVM.Prop.TaskGroupID.Map);


                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(TaskTypesVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMTaskTypesDA = m_objMTaskTypesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, m_lstGroup, m_dicOrder, null);
                if (m_objMTaskTypesDA.Message == string.Empty)
                {
                    m_lstTaskTypesVM = (
                        from DataRow m_drMTaskTypesDA in m_dicMTaskTypesDA[0].Tables[0].Rows
                        select new TaskTypesVM()
                        {
                            TaskGroupDesc = m_drMTaskTypesDA[TaskTypesVM.Prop.TaskGroupDesc.Name].ToString(),
                            TaskGroupID = m_drMTaskTypesDA[TaskTypesVM.Prop.TaskGroupID.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstTaskTypesVM, m_lstTaskTypesVM.Count);
        }
        public ActionResult ReadTaskType(string TaskGroupID="", string ListTaskTypeID ="")
        {
            string message = "";
            List<TaskTypesVM> lst_objTaskTypeVM = new List<TaskTypesVM>();
            MTaskTypesDA m_objMTaskTypesDA = new MTaskTypesDA();
            m_objMTaskTypesDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(TaskTypesVM.Prop.TaskTypeID.MapAlias);
            m_lstSelect.Add(TaskTypesVM.Prop.TaskGroupID.MapAlias);
            m_lstSelect.Add(TaskTypesVM.Prop.Descriptions.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            if (TaskGroupID.Length > 0)
            {
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(TaskGroupID);
                m_objFilter.Add(TaskTypesVM.Prop.TaskGroupID.Map, m_lstFilter);
            }

            if (TaskGroupID != "")
            {
                Dictionary<int, DataSet> m_dicMApprovalDelegationDA = m_objMTaskTypesDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
                if (m_objMTaskTypesDA.Message == string.Empty)
                {
                    List<ApprovalDelegationVM> lstTaskType = new List<ApprovalDelegationVM>();
                    if (ListTaskTypeID.Length > 0)
                    {
                        lstTaskType = JSON.Deserialize<List<ApprovalDelegationVM>>(ListTaskTypeID);
                    }
                    foreach (DataRow m_drMApprovalDelegationDA in m_dicMApprovalDelegationDA[0].Tables[0].Rows)
                    {
                        TaskTypesVM m_objApprovalDelegationUserVM = new TaskTypesVM();
                        m_objApprovalDelegationUserVM.TaskTypeID = m_drMApprovalDelegationDA[TaskTypesVM.Prop.TaskTypeID.Name].ToString();
                        m_objApprovalDelegationUserVM.TaskGroupID = m_drMApprovalDelegationDA[TaskTypesVM.Prop.TaskGroupID.Name].ToString();
                        m_objApprovalDelegationUserVM.Descriptions = m_drMApprovalDelegationDA[TaskTypesVM.Prop.Descriptions.Name].ToString();
                        if (ListTaskTypeID.Length > 0)
                        {
                            foreach (ApprovalDelegationVM obj in lstTaskType.Where(x => x.TaskTypeID == m_objApprovalDelegationUserVM.TaskTypeID))
                            {
                                m_objApprovalDelegationUserVM.isSelected = true;
                                lst_objTaskTypeVM.Add(m_objApprovalDelegationUserVM);
                                break;
                            }
                        }

                        if (ListTaskTypeID.Length == 0)
                        {
                            lst_objTaskTypeVM.Add(m_objApprovalDelegationUserVM);
                        }
                    }
                    

                }
                else
                    message = " Error TaskType" + m_objMTaskTypesDA.Message;
            }


            return this.Store(lst_objTaskTypeVM, lst_objTaskTypeVM.Count);
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

            ApprovalDelegationVM m_objApprovalDelegationVM = new ApprovalDelegationVM();
            ViewDataDictionary m_vddApprovalDelegation = new ViewDataDictionary();
            m_vddApprovalDelegation.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddApprovalDelegation.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
            //if (Caller == General.EnumDesc(Buttons.ButtonDetail))
            //{
            //    NameValueCollection m_nvcParams = this.Request.Params;
            //    Dictionary<string, object> m_dicSelectedRow = GetFormData(m_nvcParams);
            //    Session[dataSessionName] = JSON.Serialize(m_dicSelectedRow);
            //}


            //Get Owner Name
            m_objApprovalDelegationVM.lstApprovalDelegationUser = new List<ApprovalDelegationUserVM>();
            try
            {
                m_objApprovalDelegationVM.UserID = System.Web.HttpContext.Current.User.Identity.Name;
                MUserDA m_objMUserDA = new MUserDA();
                m_objMUserDA.ConnectionStringName = Global.ConnStrConfigName;
                m_objMUserDA.Data = new MUser() { UserID = m_objApprovalDelegationVM.UserID };
                m_objMUserDA.Select();
                m_objApprovalDelegationVM.OwnerName = m_objMUserDA.Data.FullName;
            }
            catch (Exception e)
            {
                Global.ShowErrorAlert(title, e.Message);
                return this.Direct();
            }

            m_objApprovalDelegationVM.PeriodStart = DateTime.Now;
            m_objApprovalDelegationVM.PeriodEnd= DateTime.Now;
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objApprovalDelegationVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddApprovalDelegation,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Detail(string Caller, string Selected)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));
            DateTime PeriodStart_= new DateTime();
            DateTime PeriodEnd_ = new DateTime();
            string m_strMessage = string.Empty;
            ApprovalDelegationVM m_objApprovalDelegationVM = new ApprovalDelegationVM();
            Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();
            if (Caller == General.EnumDesc(Buttons.ButtonCancel))
            {
                if (Session[dataSessionName] != null)
                {
                    try
                    {
                        m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Session[dataSessionName].ToString());
                        PeriodStart_ = (DateTime)m_dicSelectedRow[ApprovalDelegationVM.Prop.PeriodStart.Name];
                        PeriodEnd_ = (DateTime)m_dicSelectedRow[ApprovalDelegationVM.Prop.PeriodEnd.Name];
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
            if (Caller == General.EnumDesc(Buttons.ButtonList))
            {
                m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
                PeriodStart_ = (DateTime)m_dicSelectedRow[ApprovalDelegationVM.Prop.PeriodStart.Name];
                PeriodEnd_ = (DateTime)m_dicSelectedRow[ApprovalDelegationVM.Prop.PeriodEnd.Name];
            }
            else if (Caller == General.EnumDesc(Buttons.ButtonSave))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                m_dicSelectedRow = GetFormData(m_nvcParams);
                PeriodStart_ = Convert.ToDateTime(this.Request.Params[ApprovalDelegationVM.Prop.PeriodStart.Name]);
                PeriodEnd_ = Convert.ToDateTime(this.Request.Params[ApprovalDelegationVM.Prop.PeriodEnd.Name]);
                PeriodStart_ = PeriodStart_.Date.Add(new TimeSpan(0, 0, 0));
                PeriodEnd_ = PeriodEnd_.Date.Add(new TimeSpan(0, 0, 0));
                //PeriodStart_ =DateTime.Parse( m_dicSelectedRow[ApprovalDelegationVM.Prop.PeriodStart.Name].ToString());
                //PeriodEnd_ = DateTime.Parse( m_dicSelectedRow[ApprovalDelegationVM.Prop.PeriodEnd.Name].ToString());
            }
            if (m_dicSelectedRow.Count > 0)
                m_objApprovalDelegationVM = GetSelectedDataDistinct(m_dicSelectedRow[ApprovalDelegationVM.Prop.TaskGroupID.Name].ToString(), PeriodStart_, PeriodEnd_, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddApprovalDelegation = new ViewDataDictionary();
            m_vddApprovalDelegation.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
            m_vddApprovalDelegation.Add("SelectedTypes", JSON.Serialize(m_objApprovalDelegationVM.lstTaskType));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objApprovalDelegationVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddApprovalDelegation,
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
            ApprovalDelegationVM m_objApprovalDelegationVM = new ApprovalDelegationVM();
            if (m_dicSelectedRow.Count > 0)
                m_objApprovalDelegationVM = GetSelectedData(m_dicSelectedRow[ApprovalDelegationVM.Prop.TaskGroupID.Name].ToString(), (DateTime)m_dicSelectedRow[ApprovalDelegationVM.Prop.PeriodStart.Name], (DateTime)m_dicSelectedRow[ApprovalDelegationVM.Prop.PeriodEnd.Name], ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }                      

            ViewDataDictionary m_vddApprovalDelegation = new ViewDataDictionary();
            m_vddApprovalDelegation.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddApprovalDelegation.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            m_vddApprovalDelegation.Add("SelectedTypes", JSON.Serialize(m_objApprovalDelegationVM.lstTaskType));
            m_vddApprovalDelegation.Add("ListApprovalDelegateID", JSON.Serialize(m_objApprovalDelegationVM.ListApprovalDelegateID));

            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objApprovalDelegationVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddApprovalDelegation,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstSelectedRow = new List<string>();
            m_lstSelectedRow = JSON.Deserialize<List<string>>(Selected);
            
            MApprovalDelegationDA m_objMApprovalDelegationDA = new MApprovalDelegationDA();
            m_objMApprovalDelegationDA.ConnectionStringName = Global.ConnStrConfigName;

            DApprovalDelegationUserDA m_objDApprovalDelegationUserDA = new DApprovalDelegationUserDA();
            m_objDApprovalDelegationUserDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstMessage = new List<string>();
            if (!m_lstSelectedRow.Any())
                m_lstMessage.Add("Some of selected row doesn't have list of Approval ID.");

            object m_objDBConnection = null;
            string m_strTransName = "DeleteApprovalDelegation";
            m_objDBConnection = m_objDApprovalDelegationUserDA.BeginTrans(m_strTransName);

            if (!m_lstMessage.Any())
            try
            {
                foreach (string listApprovalDelegateID in m_lstSelectedRow)
                {                    
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.In);
                    m_lstFilter.Add(string.Join(",", listApprovalDelegateID));
                    m_objFilter.Add(ApprovalDelegationUserVM.Prop.ApprovalDelegateID.Map, m_lstFilter);
                    m_objDApprovalDelegationUserDA.DeleteBC(m_objFilter, true, m_objDBConnection);                                      
                }

                if (!m_objDApprovalDelegationUserDA.Success)
                    m_lstMessage.Add(m_objDApprovalDelegationUserDA.Message);
                else
                    foreach (string listDelegateID in m_lstSelectedRow)
                    {
                        Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                        List<object> m_lstFilter = new List<object>();
                        m_lstFilter.Add(Operator.In);
                        m_lstFilter.Add(string.Join(",", listDelegateID));
                        m_objFilter.Add(ApprovalDelegationVM.Prop.ApprovalDelegateID.Map, m_lstFilter);
                        m_objMApprovalDelegationDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                        if (!m_objMApprovalDelegationDA.Success)
                            m_lstMessage.Add(m_objMApprovalDelegationDA.Message);
                    }

            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);               
            }

            if (m_lstMessage.Count <= 0)
            {
                m_objDApprovalDelegationUserDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
                Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Deleted));                
            }
            else
            {
                m_objDApprovalDelegationUserDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));                
            }

            return this.Direct();
        }

        public ActionResult Browse(string ControlApprovalDelegationID, string ControlUserID,string ControlFunctionID, string FilterApprovalDelegationID = "", string FilterApprovalUserID = "", string FilterFunctionID = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddApprovalDelegation = new ViewDataDictionary();
            m_vddApprovalDelegation.Add("Control" + ApprovalDelegationVM.Prop.ApprovalDelegateID.Name, ControlApprovalDelegationID);
            m_vddApprovalDelegation.Add("Control" + ApprovalDelegationVM.Prop.UserID.Name, ControlUserID);
            m_vddApprovalDelegation.Add("Control" + ApprovalDelegationVM.Prop.TaskTypeID.Name, ControlUserID);
            m_vddApprovalDelegation.Add(ApprovalDelegationVM.Prop.ApprovalDelegateID.Name, FilterApprovalDelegationID);
            m_vddApprovalDelegation.Add(ApprovalDelegationVM.Prop.UserID.Name, FilterApprovalUserID);
            m_vddApprovalDelegation.Add(ApprovalDelegationVM.Prop.TaskTypeID.Name, FilterApprovalUserID);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddApprovalDelegation,
                ViewName = "../ApprovalDelegation/_Browse"
            };
        }
        public ActionResult BrowseTaskGroup(string ControlTaskGroupID, string ControlTaskGroupDesc, string FilterTaskGroupID = "", string FilterTaskGroupDesc = "")
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddTaskTypes = new ViewDataDictionary();
            m_vddTaskTypes.Add("Control" + TaskTypesVM.Prop.TaskGroupID.Name, ControlTaskGroupID);
            m_vddTaskTypes.Add("Control" + TaskTypesVM.Prop.TaskGroupDesc.Name, ControlTaskGroupDesc);
            m_vddTaskTypes.Add(TaskTypesVM.Prop.TaskGroupID.Name, FilterTaskGroupID);
            m_vddTaskTypes.Add(TaskTypesVM.Prop.TaskGroupDesc.Name, FilterTaskGroupDesc);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddTaskTypes,
                ViewName = "../ApprovalDelegation/_BrowseTaskGroup"
            };
        }
        public ActionResult BackupSave(string Action)
        {
            //action = JSON.Deserialize<string>(action);
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MApprovalDelegationDA m_objMApprovalDelegationDA = new MApprovalDelegationDA();
            MApprovalDelegation m_objMApprovalDelegation = new MApprovalDelegation();
            m_objMApprovalDelegationDA.ConnectionStringName = Global.ConnStrConfigName;

            DApprovalDelegationUserDA m_objMApprovalDelegationUserDA = new DApprovalDelegationUserDA();
            DApprovalDelegationUser m_objMApprovalDelegationUser = new DApprovalDelegationUser();
            m_objMApprovalDelegationUserDA.ConnectionStringName = Global.ConnStrConfigName;

            object m_objDBConnection = null;
            string m_strTransName = "SaveApprovalDelegation";
            m_objDBConnection = m_objMApprovalDelegationDA.BeginTrans(m_strTransName);

            try
            {
                string m_strApprovalDelegationID = Action == General.EnumDesc(Buttons.ButtonAdd) ? Guid.NewGuid().ToString().Replace("-", "") :this.Request.Params[ApprovalDelegationVM.Prop.ApprovalDelegateID.Name];
                string m_strUserID = this.Request.Params[ApprovalDelegationVM.Prop.UserID.Name];   string m_strTaskTypeID = this.Request.Params[ApprovalDelegationVM.Prop.TaskTypeID.Name];
                DateTime m_strPeriodStart =Convert.ToDateTime(this.Request.Params[ApprovalDelegationVM.Prop.PeriodStart.Name]);
                DateTime m_strPeriodEnd = Convert.ToDateTime(this.Request.Params[ApprovalDelegationVM.Prop.PeriodStart.Name]);
                m_strPeriodStart = m_strPeriodStart.Date.Add(new TimeSpan(0, 0, 0));
                m_strPeriodEnd = m_strPeriodStart.Date.Add(new TimeSpan(0, 0, 0));

                List<DApprovalDelegationUser> lst_strApprovalDelegationUserID = new List<DApprovalDelegationUser>();
                string ParamName = "ListApprovalDelegationUser";
                if (this.Request.Params[ParamName] != null)
                {
                    Dictionary<string, object>[] m_arrListApprovalDelegationUser = JSON.Deserialize<Dictionary<string, object>[]>(this.Request.Params[ParamName]);
                    if (m_arrListApprovalDelegationUser == null)
                        m_arrListApprovalDelegationUser = new List<Dictionary<string, object>>().ToArray();
                    foreach (Dictionary<string, object> m_dicApprovalDelegationUser in m_arrListApprovalDelegationUser)
                    {
                        DApprovalDelegationUser objApprovalDelegation = new DApprovalDelegationUser();
                        objApprovalDelegation.ApprovalDelegationUserID = Guid.NewGuid().ToString().Replace("-", "");                        
                        objApprovalDelegation.DelegateUserID= m_dicApprovalDelegationUser[ApprovalDelegationUserVM.Prop.DelegateUserID.Name].ToString();
                        lst_strApprovalDelegationUserID.Add(objApprovalDelegation);
                    }
                }

                
                List<string> lst_strTaskType = new List<string>();
                ParamName = "ListTaskType";
                if (this.Request.Params[ParamName] != null)
                {
                    Dictionary<string, object>[] m_arrListTaskType = JSON.Deserialize<Dictionary<string, object>[]>(this.Request.Params[ParamName]);
                    if (m_arrListTaskType == null)
                        m_arrListTaskType = new List<Dictionary<string, object>>().ToArray();
                    foreach (Dictionary<string, object> m_dicTaskType in m_arrListTaskType)
                        lst_strTaskType.Add(m_dicTaskType[TaskTypesVM.Prop.TaskTypeID.Name].ToString());

                }

                //m_lstMessage = IsSaveValid(Action, m_strApprovalDelegationID, m_strUserID, m_strTaskTypeID);
                if (m_lstMessage.Count <= 0)
                {
                    foreach (string tasktype_ in lst_strTaskType)
                    {
                        m_strApprovalDelegationID = Guid.NewGuid().ToString().Replace("-", "");
                        #region ApprovalDelegation
                        m_objMApprovalDelegation = new MApprovalDelegation();
                        m_objMApprovalDelegation.ApprovalDelegateID = m_strApprovalDelegationID;
                        m_objMApprovalDelegationDA.Data = m_objMApprovalDelegation;

                        if (Action != General.EnumDesc(Buttons.ButtonAdd))
                            m_objMApprovalDelegationDA.Select();

                        m_objMApprovalDelegationDA.Data.TaskTypeID = tasktype_;
                        m_objMApprovalDelegationDA.Data.UserID = m_strUserID;
                        m_objMApprovalDelegationDA.Data.PeriodStart = m_strPeriodStart;
                        m_objMApprovalDelegationDA.Data.PeriodEnd = m_strPeriodEnd;

                        if (Action == General.EnumDesc(Buttons.ButtonAdd))
                            m_objMApprovalDelegationDA.Insert(true, m_objDBConnection);
                        else
                            m_objMApprovalDelegationDA.Update(true, m_objDBConnection);

                        if (!m_objMApprovalDelegationDA.Success || m_objMApprovalDelegationDA.Message != string.Empty)
                            m_lstMessage.Add(m_objMApprovalDelegationDA.Message);
                        #endregion

                        #region List Delegation User
                        if (!m_lstMessage.Any())
                        {
                            //Delete If Update
                            //if (Action == General.EnumDesc(Buttons.ButtonUpdate))
                            //{
                            //    Dictionary<string, List<object>> m_objFilter_ = new Dictionary<string, List<object>>();
                            //    List<object> m_lstFilter_ = new List<object>();
                            //    m_lstFilter_.Add(Operator.Equals);
                            //    m_lstFilter_.Add(m_strApprovalDelegationID);
                            //    m_objFilter_.Add(ApprovalDelegationUserVM.Prop.ApprovalDelegateID.Map, m_lstFilter_);
                            //    m_objMApprovalDelegationUserDA.DeleteBC(m_objFilter_, true, m_objDBConnection);
                            //    if (!m_objMApprovalDelegationUserDA.Success)
                            //        m_lstMessage.Add(m_objMApprovalDelegationUserDA.Message);
                            //}
                            //Insert
                            if (!m_lstMessage.Any())
                            {
                                foreach (DApprovalDelegationUser objDelegationUser in lst_strApprovalDelegationUserID)
                                {
                                    m_objMApprovalDelegationUserDA.Data = objDelegationUser;
                                    m_objMApprovalDelegationUserDA.Data.ApprovalDelegateID = m_strApprovalDelegationID;
                                    m_objMApprovalDelegationUserDA.Insert(true, m_objDBConnection);
                                    if (!m_objMApprovalDelegationUserDA.Success)
                                    {
                                        m_lstMessage.Add(m_objMApprovalDelegationUserDA.Message);
                                        break;
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                m_objMApprovalDelegationDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            }
            if (m_lstMessage.Count <= 0)
            {
                m_objMApprovalDelegationDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
                Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
                return Detail(General.EnumDesc(Buttons.ButtonSave), null);
            }
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);
        }
        public ActionResult Save(string Action , string ListApprovalDelegateID="")
        {
            //action = JSON.Deserialize<string>(action);
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MApprovalDelegationDA m_objMApprovalDelegationDA = new MApprovalDelegationDA();
            MApprovalDelegation m_objMApprovalDelegation = new MApprovalDelegation();
            m_objMApprovalDelegationDA.ConnectionStringName = Global.ConnStrConfigName;

            DApprovalDelegationUserDA m_objMApprovalDelegationUserDA = new DApprovalDelegationUserDA();
            DApprovalDelegationUser m_objMApprovalDelegationUser = new DApprovalDelegationUser();
            m_objMApprovalDelegationUserDA.ConnectionStringName = Global.ConnStrConfigName;

            DateTime m_strPeriodStart = new DateTime();
            DateTime m_strPeriodEnd = new DateTime();
            List<string> lst_strTaskType = new List<string>();
            string ParamName;
            try
            {
                m_strPeriodStart = Convert.ToDateTime(this.Request.Params[ApprovalDelegationVM.Prop.PeriodStart.Name]);
                m_strPeriodEnd = Convert.ToDateTime(this.Request.Params[ApprovalDelegationVM.Prop.PeriodEnd.Name]);
                m_strPeriodStart = m_strPeriodStart.Date.Add(new TimeSpan(0, 0, 0));
                m_strPeriodEnd = m_strPeriodEnd.Date.Add(new TimeSpan(0, 0, 0));

                ParamName = "ListTaskType";
                if (this.Request.Params[ParamName] != null)
                {
                    Dictionary<string, object>[] m_arrListTaskType = JSON.Deserialize<Dictionary<string, object>[]>(this.Request.Params[ParamName]);
                    if (m_arrListTaskType == null)
                        m_arrListTaskType = new List<Dictionary<string, object>>().ToArray();
                    foreach (Dictionary<string, object> m_dicTaskType in m_arrListTaskType)
                        lst_strTaskType.Add(m_dicTaskType[TaskTypesVM.Prop.TaskTypeID.Name].ToString());

                }
                List<ApprovalDelegationVM> m_lstApprovalDelegationVM = new List<ApprovalDelegationVM>();
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(ApprovalDelegationVM.Prop.ApprovalDelegateID.MapAlias);
                m_lstSelect.Add(ApprovalDelegationVM.Prop.TaskTypeDesc.MapAlias);

                Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                List<object> m_lstFilter = new List<object>();

                m_lstFilter.Add(Action == (General.EnumDesc(Buttons.ButtonAdd)) ? Operator.In : Operator.NotIn);
                m_lstFilter.Add(string.Join(",", lst_strTaskType));
                m_objFilter.Add(ApprovalDelegationVM.Prop.TaskTypeID.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(System.Web.HttpContext.Current.User.Identity.Name);
                m_objFilter.Add(ApprovalDelegationVM.Prop.UserID.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.None);
                m_lstFilter.Add(string.Empty);
                m_objFilter.Add(string.Format("((PeriodStart < '{0}' and PeriodEnd > '{1}')OR(PeriodStart BETWEEN  '{0}' and '{1}')OR(PeriodEnd BETWEEN  '{0}' and '{1}'))", m_strPeriodStart.ToString(Global.SqlDateFormat), m_strPeriodEnd.ToString(Global.SqlDateFormat)), m_lstFilter);

                Dictionary<int, DataSet> m_dicMApprovalDelegation = m_objMApprovalDelegationDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
                if (m_objMApprovalDelegationDA.Success && m_objMApprovalDelegationDA.AffectedRows > 0)
                {
                    List<string> TaskIntersects = new List<string>();
                    foreach (DataRow obj in m_dicMApprovalDelegation[0].Tables[0].Rows)
                        TaskIntersects.Add(obj[ApprovalDelegationVM.Prop.TaskTypeDesc.Name].ToString());

                    m_lstMessage.Add("Date range already taken (" + string.Join(",", TaskIntersects) + ")");
                }
                else if (!m_objMApprovalDelegationDA.Success)
                {
                    m_lstMessage.Add(m_objMApprovalDelegationDA.Message);
                }

            }
            catch (Exception e)
            {
                m_lstMessage.Add(e.Message);
            }

            object m_objDBConnection = null;
            string m_strTransName = "SaveApprovalDelegation";
            m_objDBConnection = m_objMApprovalDelegationDA.BeginTrans(m_strTransName);


            try
            {
                string m_strUserID = this.Request.Params[ApprovalDelegationVM.Prop.UserID.Name]; string m_strTaskTypeID = this.Request.Params[ApprovalDelegationVM.Prop.TaskTypeID.Name];
                if (Action == General.EnumDesc(Buttons.ButtonAdd))
                {
                    List<DApprovalDelegationUser> lst_strApprovalDelegationUserID = new List<DApprovalDelegationUser>();
                    ParamName = "ListApprovalDelegationUser";
                    if (this.Request.Params[ParamName] != null)
                    {
                        Dictionary<string, object>[] m_arrListApprovalDelegationUser = JSON.Deserialize<Dictionary<string, object>[]>(this.Request.Params[ParamName]);
                        if (m_arrListApprovalDelegationUser == null)
                            m_arrListApprovalDelegationUser = new List<Dictionary<string, object>>().ToArray();
                        foreach (Dictionary<string, object> m_dicApprovalDelegationUser in m_arrListApprovalDelegationUser)
                        {
                            DApprovalDelegationUser objApprovalDelegation = new DApprovalDelegationUser();
                            objApprovalDelegation.DelegateUserID = m_dicApprovalDelegationUser[ApprovalDelegationUserVM.Prop.DelegateUserID.Name].ToString();
                            lst_strApprovalDelegationUserID.Add(objApprovalDelegation);
                        }
                    }

                    m_lstMessage = !m_lstMessage.Any() ? IsSaveValid(Action, lst_strTaskType, lst_strApprovalDelegationUserID) : m_lstMessage;
                    if (m_lstMessage.Count <= 0)
                    {
                        foreach (string tasktype in lst_strTaskType)
                        {
                            string m_strApprovalDelegationID = Guid.NewGuid().ToString().Replace("-", "");
                            #region ApprovalDelegation
                            m_objMApprovalDelegation = new MApprovalDelegation();
                            m_objMApprovalDelegation.ApprovalDelegateID = m_strApprovalDelegationID;
                            m_objMApprovalDelegationDA.Data = m_objMApprovalDelegation;
                            m_objMApprovalDelegationDA.Data.TaskTypeID = tasktype;
                            m_objMApprovalDelegationDA.Data.UserID = m_strUserID;
                            m_objMApprovalDelegationDA.Data.PeriodStart = m_strPeriodStart;
                            m_objMApprovalDelegationDA.Data.PeriodEnd = m_strPeriodEnd;

                            m_objMApprovalDelegationDA.Insert(true, m_objDBConnection);

                            if (!m_objMApprovalDelegationDA.Success || m_objMApprovalDelegationDA.Message != string.Empty)
                                m_lstMessage.Add(m_objMApprovalDelegationDA.Message);
                            #endregion

                            #region List Delegation User
                            if (!m_lstMessage.Any())
                            {
                                foreach (DApprovalDelegationUser objDelegationUser in lst_strApprovalDelegationUserID)
                                {
                                    m_objMApprovalDelegationUserDA.Data = objDelegationUser;
                                    m_objMApprovalDelegationUserDA.Data.ApprovalDelegationUserID = Guid.NewGuid().ToString().Replace("-", "");
                                    m_objMApprovalDelegationUserDA.Data.ApprovalDelegateID = m_strApprovalDelegationID;
                                    m_objMApprovalDelegationUserDA.Insert(true, m_objDBConnection);
                                    if (!m_objMApprovalDelegationUserDA.Success)
                                    {
                                        m_lstMessage.Add(m_objMApprovalDelegationUserDA.Message);
                                        break;
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }
                else
                {
                    Dictionary<string, List<object>> m_objSet = new Dictionary<string, List<object>>();
                    List<object> m_lstSet = new List<object>();
                    m_lstSet.Add(typeof(DateTime));
                    m_lstSet.Add(m_strPeriodStart);
                    m_objSet.Add(ApprovalDelegationVM.Prop.PeriodStart.Map, m_lstSet);

                    m_lstSet = new List<object>();
                    m_lstSet.Add(typeof(DateTime));
                    m_lstSet.Add(m_strPeriodEnd);
                    m_objSet.Add(ApprovalDelegationVM.Prop.PeriodEnd.Map, m_lstSet);

                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.In);
                    m_lstFilter.Add(string.Join(",",JSON.Deserialize<List<string>>(ListApprovalDelegateID)));
                    m_objFilter.Add(ApprovalDelegationVM.Prop.ApprovalDelegateID.Map, m_lstFilter);


                    m_objMApprovalDelegationDA.UpdateBC(m_objSet, m_objFilter, true, m_objDBConnection);
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                m_objMApprovalDelegationDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            }
       
            if (m_lstMessage.Count <= 0)
            {
                m_objMApprovalDelegationDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
                Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
                return Detail(General.EnumDesc(Buttons.ButtonSave), null);
            }
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);
        }

        #endregion

        #region Direct Method

        public ActionResult GetApprovalDelegation(string ControlApprovalDelegationID, string ControlUserID, string ControlFunctionID, string FilterApprovalDelegationID, string FilterUserID,  string FilterFunctionID ,bool Exact = false)
        {
            try
            {
                Dictionary<int, List<ApprovalDelegationVM>> m_dicApprovalDelegationData = GetApprovalDelegationData(true, FilterApprovalDelegationID, FilterUserID, FilterFunctionID);
                KeyValuePair<int, List<ApprovalDelegationVM>> m_kvpApprovalDelegationVM = m_dicApprovalDelegationData.AsEnumerable().ToList()[0];
                if (m_kvpApprovalDelegationVM.Key < 1 || (m_kvpApprovalDelegationVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpApprovalDelegationVM.Key > 1 && !Exact)
                    return Browse(ControlApprovalDelegationID, ControlUserID,ControlFunctionID, FilterApprovalDelegationID, FilterUserID, FilterFunctionID);

                m_dicApprovalDelegationData = GetApprovalDelegationData(false, FilterApprovalDelegationID, FilterUserID, FilterFunctionID);
                ApprovalDelegationVM m_objApprovalDelegationVM = m_dicApprovalDelegationData[0][0];
                this.GetCmp<TextField>(ControlApprovalDelegationID).Value = m_objApprovalDelegationVM.ApprovalDelegateID;
                this.GetCmp<TextField>(ControlUserID).Value = m_objApprovalDelegationVM.UserID;
                this.GetCmp<TextField>(ControlUserID).Value = m_objApprovalDelegationVM.TaskTypeID;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, List<string> ListTaskType, List<DApprovalDelegationUser> ListUserDelegation)
        {
            List<string> m_lstReturn = new List<string>();
            if (!ListTaskType.Any())
                m_lstReturn.Add("TaskType at least 1 checked");
            if (!ListUserDelegation.Any())
                m_lstReturn.Add("Delegation user requires at least 1 data");


            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();
            
            m_dicReturn.Add(ApprovalDelegationVM.Prop.PeriodEnd.Name, parameters[ApprovalDelegationVM.Prop.PeriodEnd.Name]);
            m_dicReturn.Add(ApprovalDelegationVM.Prop.PeriodStart.Name, parameters[ApprovalDelegationVM.Prop.PeriodStart.Name]);
            m_dicReturn.Add(ApprovalDelegationVM.Prop.UserID.Name, parameters[ApprovalDelegationVM.Prop.UserID.Name]);
            m_dicReturn.Add(ApprovalDelegationVM.Prop.TaskGroupID.Name, parameters[ApprovalDelegationVM.Prop.TaskGroupID.Name]);
            m_dicReturn.Add(ApprovalDelegationVM.Prop.TaskTypeDesc.Name, parameters[ApprovalDelegationVM.Prop.TaskTypeDesc.Name]);

            return m_dicReturn;
        }
        private ApprovalDelegationVM GetSelectedData(string TaskGroupID, DateTime PeriodStart, DateTime PeriodEnd, ref string message)
        {
            List<ApprovalDelegationVM> mlst_objApprovalDelegationVM = new List<ApprovalDelegationVM>();
            ApprovalDelegationVM m_objApprovalDelegationVM = new ApprovalDelegationVM();
            MApprovalDelegationDA m_objMApprovalDelegationDA = new MApprovalDelegationDA();
            m_objMApprovalDelegationDA.ConnectionStringName = Global.ConnStrConfigName;

            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(PeriodStart.ToString(Global.SqlDateFormat));
            m_objFilter.Add("MApprovalDelegation.PeriodStart", m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(PeriodEnd.ToString(Global.SqlDateFormat));
            m_objFilter.Add("MApprovalDelegation.PeriodEnd", m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(TaskGroupID);
            m_objFilter.Add("MApprovalDelegation.TaskGroupID", m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(System.Web.HttpContext.Current.User.Identity.Name);
            m_objFilter.Add("MApprovalDelegation.UserID", m_lstFilter);

            Dictionary<int, DataSet> m_dicMApprovalDelegationDA = m_objMApprovalDelegationDA.SelectDistinctBC(0, null, false, null, m_objFilter, null, null, null, null);
            if (m_objMApprovalDelegationDA.Message == string.Empty)
            {
                DataRow m_drMApprovalDelegationDA = m_dicMApprovalDelegationDA[0].Tables[0].Rows[0];
                m_objApprovalDelegationVM = new ApprovalDelegationVM()
                {
                    UserID = m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.UserID.Name].ToString(),
                    TaskGroupID = m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.TaskGroupID.Name].ToString(),
                    TaskGroupDesc = m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.TaskGroupDesc.Name].ToString(),
                    OwnerName = m_drMApprovalDelegationDA["FullName"].ToString(),
                    PeriodStart = (DateTime)m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.PeriodStart.Name],
                    PeriodEnd = (DateTime)m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.PeriodEnd.Name],
                    lstApprovalDelegationUser = GetListDelegationUser(m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.TaskGroupID.Name].ToString(), (DateTime)m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.PeriodStart.Name], (DateTime)m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.PeriodEnd.Name], ref message),
                    lstTaskType = GetListTaskType(m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.TaskGroupID.Name].ToString(), (DateTime)m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.PeriodStart.Name], (DateTime)m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.PeriodEnd.Name], ref message)
                };

                if (m_objMApprovalDelegationDA.AffectedRows <= 0)
                {
                    m_objApprovalDelegationVM.lstApprovalDelegationUser = new List<ApprovalDelegationUserVM>();
                    m_objApprovalDelegationVM.lstTaskType = new List<ApprovalDelegationVM>();
                }
                
                //Get List Approval ID
                m_objApprovalDelegationVM.ListApprovalDelegateID = GetListApprovalDelegationID(TaskGroupID,PeriodStart, PeriodEnd, ref message);
              
            }
            return m_objApprovalDelegationVM;
        }
        private List<string> GetListApprovalDelegationID(string TaskGroupID,DateTime PeriodStart, DateTime PeriodEnd, ref string message) {

            MApprovalDelegationDA m_objMApprovalDelegationDA = new MApprovalDelegationDA();
            m_objMApprovalDelegationDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> ListApprovalDelegateID = new List<string>();
            List<string> m_lstSelects = new List<string>();
            m_lstSelects.Add(ApprovalDelegationVM.Prop.ApprovalDelegateID.MapAlias);
            m_lstSelects.Add(ApprovalDelegationVM.Prop.PeriodStart.MapAlias);

            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            m_lstFilter = new List<object>();
            m_objFilter = new Dictionary<string, List<object>>();
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(PeriodStart.ToString(Global.SqlDateFormat));
            m_objFilter.Add(ApprovalDelegationVM.Prop.PeriodStart.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(PeriodEnd.ToString(Global.SqlDateFormat));
            m_objFilter.Add(ApprovalDelegationVM.Prop.PeriodEnd.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(TaskGroupID);
            m_objFilter.Add(ApprovalDelegationVM.Prop.TaskGroupID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(System.Web.HttpContext.Current.User.Identity.Name);
            m_objFilter.Add(ApprovalDelegationVM.Prop.UserID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMApprovalDelegationDA = m_objMApprovalDelegationDA.SelectBC(0, null, false, m_lstSelects, m_objFilter, null, null, null, null);

            if (m_objMApprovalDelegationDA.Success)
                foreach (DataRow m_dr in m_dicMApprovalDelegationDA[0].Tables[0].Rows)
                    ListApprovalDelegateID.Add(m_dr[ApprovalDelegationVM.Prop.ApprovalDelegateID.Name].ToString());
            else
                message = m_objMApprovalDelegationDA.Message;


            return ListApprovalDelegateID;
        }

        private ApprovalDelegationVM GetSelectedDataDistinct(string TaskGroupID, DateTime PeriodStart,DateTime PeriodEnd , ref string message)
        {
            List<ApprovalDelegationVM> mlst_objApprovalDelegationVM = new List<ApprovalDelegationVM>();
            ApprovalDelegationVM m_objApprovalDelegationVM = new ApprovalDelegationVM();
            MApprovalDelegationDA m_objMApprovalDelegationDA = new MApprovalDelegationDA();
            m_objMApprovalDelegationDA.ConnectionStringName = Global.ConnStrConfigName;
            
            
            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(PeriodStart.ToString(Global.SqlDateFormat));
            m_objFilter.Add("MApprovalDelegation.PeriodStart", m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(PeriodEnd.ToString(Global.SqlDateFormat));
            m_objFilter.Add("MApprovalDelegation.PeriodEnd", m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(TaskGroupID);
            m_objFilter.Add("MApprovalDelegation.TaskGroupID", m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(System.Web.HttpContext.Current.User.Identity.Name);
            m_objFilter.Add("MApprovalDelegation.UserID", m_lstFilter);

            Dictionary<int, DataSet> m_dicMApprovalDelegationDA = m_objMApprovalDelegationDA.SelectDistinctBC(0, null, false, null, m_objFilter, null, null, null, null);
            if (m_objMApprovalDelegationDA.Message == string.Empty)
            {
                DataRow m_drMApprovalDelegationDA = m_dicMApprovalDelegationDA[0].Tables[0].Rows[0];
                m_objApprovalDelegationVM = new ApprovalDelegationVM()
                {
                    UserID = m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.UserID.Name].ToString(),
                    TaskGroupID = m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.TaskGroupID.Name].ToString(),
                    TaskGroupDesc = m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.TaskGroupDesc.Name].ToString(),
                    OwnerName = m_drMApprovalDelegationDA["FullName"].ToString(),
                    PeriodStart = (DateTime)m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.PeriodStart.Name],
                    PeriodEnd = (DateTime)m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.PeriodEnd.Name],
                    lstApprovalDelegationUser = GetListDelegationUser(m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.TaskGroupID.Name].ToString(), (DateTime)m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.PeriodStart.Name], (DateTime)m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.PeriodEnd.Name], ref message),
                    lstTaskType = GetListTaskType(m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.TaskGroupID.Name].ToString(), (DateTime)m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.PeriodStart.Name], (DateTime)m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.PeriodEnd.Name], ref message)
                };
                if (m_objMApprovalDelegationDA.AffectedRows <= 0)
                {
                    m_objApprovalDelegationVM.lstApprovalDelegationUser = new List<ApprovalDelegationUserVM>();
                    m_objApprovalDelegationVM.lstTaskType = new List<ApprovalDelegationVM>();
                }
            }
            return m_objApprovalDelegationVM;
        }

        private List<ApprovalDelegationUserVM> GetListDelegationUser(string TaskGroupID, DateTime PeriodStart, DateTime PeriodEnd, ref string message)
        {
            List<ApprovalDelegationUserVM> lst_objApprovalDelegationUserVM = new List<ApprovalDelegationUserVM>();
            DApprovalDelegationUserDA m_objDApprovalDelegationUserDA = new DApprovalDelegationUserDA();
            m_objDApprovalDelegationUserDA.ConnectionStringName = Global.ConnStrConfigName;

          

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();          
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(PeriodStart.ToString(Global.SqlDateFormat));
            m_objFilter.Add("[DApprovalDelegationUser].PeriodStart", m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(PeriodEnd.ToString(Global.SqlDateFormat));
            m_objFilter.Add("[DApprovalDelegationUser].PeriodEnd", m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(TaskGroupID);
            m_objFilter.Add("[DApprovalDelegationUser].TaskGroupID", m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(System.Web.HttpContext.Current.User.Identity.Name);
            m_objFilter.Add("[DApprovalDelegationUser].UserID", m_lstFilter);


            Dictionary<int, DataSet> m_dicMApprovalDelegationDA = m_objDApprovalDelegationUserDA.SelectDistinctBC(0, null, false, null, m_objFilter, null, null, null);
            if (m_objDApprovalDelegationUserDA.Message == string.Empty)
            {
                foreach (DataRow m_drMApprovalDelegationDA in m_dicMApprovalDelegationDA[0].Tables[0].Rows)
                {
                    ApprovalDelegationUserVM m_objApprovalDelegationUserVM = new ApprovalDelegationUserVM();
                    m_objApprovalDelegationUserVM.UserID = m_drMApprovalDelegationDA[ApprovalDelegationUserVM.Prop.UserID.Name].ToString();
                    m_objApprovalDelegationUserVM.PeriodStart = (DateTime)m_drMApprovalDelegationDA[ApprovalDelegationUserVM.Prop.PeriodStart.Name];
                    m_objApprovalDelegationUserVM.PeriodEnd= (DateTime)m_drMApprovalDelegationDA[ApprovalDelegationUserVM.Prop.PeriodEnd.Name];
                    m_objApprovalDelegationUserVM.TaskGroupID= m_drMApprovalDelegationDA[ApprovalDelegationUserVM.Prop.TaskGroupID.Name].ToString();
                    m_objApprovalDelegationUserVM.DelegateUserID = m_drMApprovalDelegationDA[ApprovalDelegationUserVM.Prop.DelegateUserID.Name].ToString();
                    m_objApprovalDelegationUserVM.DelegateTo = m_drMApprovalDelegationDA["FullName"].ToString();
                    lst_objApprovalDelegationUserVM.Add(m_objApprovalDelegationUserVM);
                }
            }
            else
                message = " Error Get list User Delegate " + m_objDApprovalDelegationUserDA.Message;

            return lst_objApprovalDelegationUserVM;
        }
        private List<ApprovalDelegationVM> GetListTaskType(string TaskGroupID,DateTime PeriodStart, DateTime PeriodEnd, ref string message)
        {
            List<ApprovalDelegationVM> lst_objApprovalDelegationVM = new List<ApprovalDelegationVM>();
            MApprovalDelegationDA m_objMApprovalDelegationDA = new MApprovalDelegationDA();
            m_objMApprovalDelegationDA.ConnectionStringName = Global.ConnStrConfigName;
            
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ApprovalDelegationVM.Prop.PeriodStart.MapAlias);
            m_lstSelect.Add(ApprovalDelegationVM.Prop.PeriodEnd.MapAlias);
            m_lstSelect.Add(ApprovalDelegationVM.Prop.TaskTypeID.MapAlias);
            m_lstSelect.Add(ApprovalDelegationVM.Prop.TaskTypeDesc.MapAlias);
            m_lstSelect.Add(ApprovalDelegationVM.Prop.TaskGroupID.MapAlias);
            m_lstSelect.Add(ApprovalDelegationVM.Prop.UserID.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(TaskGroupID);
            m_objFilter.Add(ApprovalDelegationVM.Prop.TaskGroupID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(PeriodStart.ToString(Global.SqlDateFormat));
            m_objFilter.Add(ApprovalDelegationVM.Prop.PeriodStart.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(PeriodEnd.ToString(Global.SqlDateFormat));
            m_objFilter.Add(ApprovalDelegationVM.Prop.PeriodEnd.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(System.Web.HttpContext.Current.User.Identity.Name);
            m_objFilter.Add(ApprovalDelegationVM.Prop.UserID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMApprovalDelegationDA = m_objMApprovalDelegationDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMApprovalDelegationDA.Message == string.Empty)
            {
                foreach (DataRow m_drMApprovalDelegationDA in m_dicMApprovalDelegationDA[0].Tables[0].Rows)
                {
                    ApprovalDelegationVM m_objApprovalDelegationVM = new ApprovalDelegationVM();
                    m_objApprovalDelegationVM.TaskTypeID = m_drMApprovalDelegationDA[ApprovalDelegationUserVM.Prop.TaskTypeID.Name].ToString();
                    m_objApprovalDelegationVM.TaskTypeDesc= m_drMApprovalDelegationDA[ApprovalDelegationUserVM.Prop.TaskTypeDesc.Name].ToString();
                    m_objApprovalDelegationVM.TaskGroupID = m_drMApprovalDelegationDA[ApprovalDelegationUserVM.Prop.TaskGroupID.Name].ToString();
                    m_objApprovalDelegationVM.PeriodStart= (DateTime)m_drMApprovalDelegationDA[ApprovalDelegationUserVM.Prop.PeriodStart.Name];
                    m_objApprovalDelegationVM.PeriodEnd = (DateTime)m_drMApprovalDelegationDA[ApprovalDelegationUserVM.Prop.PeriodEnd.Name];
                    lst_objApprovalDelegationVM.Add(m_objApprovalDelegationVM);
                }
            }
            else
                message = " Error Get list User Delegate " + m_objMApprovalDelegationDA.Message;

            return lst_objApprovalDelegationVM; 
        }

        #endregion

        #region Public Method

        public Dictionary<int, List<ApprovalDelegationVM>> GetApprovalDelegationData(bool isCount, string ApprovalDelegationID, string UserID, string FunctionID)
        {
            int m_intCount = 0;
            List<ApprovalDelegationVM> m_lstApprovalDelegationVM = new List<ViewModels.ApprovalDelegationVM>();
            Dictionary<int, List<ApprovalDelegationVM>> m_dicReturn = new Dictionary<int, List<ApprovalDelegationVM>>();
            MApprovalDelegationDA m_objMApprovalDelegationDA = new MApprovalDelegationDA();
            m_objMApprovalDelegationDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ApprovalDelegationVM.Prop.ApprovalDelegateID.MapAlias);
            m_lstSelect.Add(ApprovalDelegationVM.Prop.UserID.MapAlias);
            m_lstSelect.Add(ApprovalDelegationVM.Prop.TaskTypeID.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(ApprovalDelegationID);
            m_objFilter.Add(ApprovalDelegationVM.Prop.ApprovalDelegateID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(UserID);
            m_objFilter.Add(ApprovalDelegationVM.Prop.UserID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(FunctionID);
            m_objFilter.Add(ApprovalDelegationVM.Prop.TaskTypeID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMApprovalDelegationDA = m_objMApprovalDelegationDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMApprovalDelegationDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpApprovalDelegationBL in m_dicMApprovalDelegationDA)
                    {
                        m_intCount = m_kvpApprovalDelegationBL.Key;
                        break;
                    }
                else
                {
                    m_lstApprovalDelegationVM = (
                        from DataRow m_drMApprovalDelegationDA in m_dicMApprovalDelegationDA[0].Tables[0].Rows
                        select new ApprovalDelegationVM()
                        {
                            ApprovalDelegateID = m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.ApprovalDelegateID.Name].ToString(),
                            TaskTypeID = m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.TaskTypeID.Name].ToString(),
                            UserID = m_drMApprovalDelegationDA[ApprovalDelegationVM.Prop.UserID.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstApprovalDelegationVM);
            return m_dicReturn;
        }

        #endregion
    }
}