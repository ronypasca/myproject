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
using Novacode;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Xml;
using HtmlAgilityPack;

//using System.ComponentModel;

namespace com.SML.BIGTRONS.Controllers
{
    public class MyTaskController : BaseController
    {
        private readonly string title = "My Task & Approval";
        private readonly string dataSessionName = "FormData";

        #region Public Action

        public ActionResult Index()
        {
            base.Initialize();
            ViewBag.Title = title;
            return View();
        }

        public ActionResult List()
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            if (Session[dataSessionName] != null)
                Session[dataSessionName] = null;

            this.GetCmp<Ext.Net.Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 0;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.HomePage),
                RenderMode = RenderMode.AddTo,
                ViewName = "_List",
                WrapByScriptTag = false
            };
        }

        public ActionResult Read(StoreRequestParameters parameters, string isFromApproval, string eNegotiation, string eInvitation, string InvitationFunctionID, string eCatalog)
        {
            MTasksDA m_objMTaskDA = new MTasksDA();
            m_objMTaskDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;
            string message = "";

            List<string> AllRoleID = new List<string>();
            List<string> AllowedTaskType = new List<string>();

            AllRoleID = GetAllRoleID(ref message);
            AllowedTaskType = GetAllowedTaskType(AllRoleID, ref message);

            foreach (string RoleDelegate in GetDelegateRoleID(ref message))
                if (!AllRoleID.Any(x => x == RoleDelegate))
                    AllRoleID.Add(RoleDelegate);

            FilterHeaderConditions m_fhcTTCMembers = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcTTCMembers.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    if (m_strDataIndex == "TaskDesciption")
                    {
                        m_strDataIndex = MyTaskVM.Prop.Subject.Map;
                    }
                    else
                        m_strDataIndex = MyTaskVM.Prop.Map(m_strDataIndex, false);

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
            //Dictionary<int, DataSet> m_dicMyTaskDA = m_objMTaskDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            //int m_intCount = 0;

            //foreach (KeyValuePair<int, DataSet> m_kvpMyTaskBL in m_dicMyTaskDA)
            //{
            //    m_intCount = m_kvpMyTaskBL.Key;
            //    break;
            //}
            //string TaskDescription = "";
            List<object> m_lstFilters = new List<object>();
            string UserID = System.Web.HttpContext.Current.User.Identity.Name;
            if (!string.IsNullOrEmpty(isFromApproval) ? (isFromApproval.Length > 0) : false)
            {
                if (Convert.ToBoolean(isFromApproval))
                {
                    //m_objFilter = new Dictionary<string, List<object>>();
                    List<object> m_lstFilter = new List<object>();
                    //List<string> m_lstSelect = new List<string>();
                    //m_objFilter = new Dictionary<string, List<object>>();
                    m_lstFilters.Add(Operator.In);
                    m_lstFilters.Add(string.Join(",", AllRoleID));
                    m_objFilter.Add(MyTaskVM.Prop.TaskOwnerID.Map, m_lstFilters);
                }
                else
                {
                    //For Nego Config Task List
                    //m_objFilter = new Dictionary<string, List<object>>();
                    //m_lstFilters.Add(Operator.Equals);
                    //m_lstFilters.Add(UserID);
                    //m_objFilter.Add(MyTaskVM.Prop.CreatedBy.Map, m_lstFilters);
                }
            }



            if (!string.IsNullOrEmpty(eNegotiation))
            {
                List<string> NegoAllowedTaskType = new List<string>();
                switch (eNegotiation.ToLower())
                {
                    case "negotiationconfig":
                        foreach (string Allow in AllowedTaskType.Where(x => x == General.EnumDesc(TaskType.NegotiationConfigurations)))
                        {
                            NegoAllowedTaskType.Add(Allow);
                            //m_objFilter = new Dictionary<string, List<object>>();
                            //m_lstFilters.Add(Operator.Equals);
                            //m_lstFilters.Add(General.EnumDesc(TaskType.NegotiationConfigurations));
                            //m_objFilter.Add(MyTaskVM.Prop.TaskTypeID.Map, m_lstFilters);
                            //break;
                        }
                        break;
                    case "vendorwinner":
                        foreach (string Allow in AllowedTaskType.Where(x => x == General.EnumDesc(TaskType.VendorWinner)))
                        {
                            NegoAllowedTaskType.Add(Allow);
                            ////m_objFilter = new Dictionary<string, List<object>>();
                            //m_lstFilters.Add(Operator.Equals);
                            //m_lstFilters.Add(General.EnumDesc(TaskType.VendorWinner));
                            //m_objFilter.Add(MyTaskVM.Prop.TaskTypeID.Map, m_lstFilters);
                            //break;
                        }
                        break;

                }
                AllowedTaskType = NegoAllowedTaskType;
            }
            else if (!string.IsNullOrEmpty(eInvitation))
            {
                List<string> ListInvitationTaskType = GetListTypeOfInvitationGroup();
                var newData = ListInvitationTaskType.Select(i => i.ToString()).Intersect(AllowedTaskType);
                ListInvitationTaskType = newData.ToList();
                AllowedTaskType = ListInvitationTaskType;


                ////m_objFilter = new Dictionary<string, List<object>>();
                //m_lstFilters.Add(Operator.In);
                //m_lstFilters.Add(string.Join(",", ListInvitationTaskType));
                //m_objFilter.Add(MyTaskVM.Prop.TaskTypeID.Map, m_lstFilters);
            }
            else if (!string.IsNullOrEmpty(eCatalog))
            {
                AllowedTaskType.Clear();
                AllowedTaskType.Add(General.EnumDesc(TaskType.UploadItem));
            }

            List<MyTaskVM> m_lsMyTasksVM = new List<MyTaskVM>();
            //if (m_intCount > 0)
            //{
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(MyTaskVM.Prop.TaskID.MapAlias);
                m_lstSelect.Add(MyTaskVM.Prop.TaskTypeID.MapAlias);
                m_lstSelect.Add(MyTaskVM.Prop.TaskDescription.MapAlias);
                m_lstSelect.Add(MyTaskVM.Prop.TaskDesc.MapAlias);
                m_lstSelect.Add(MyTaskVM.Prop.StatusDesc.MapAlias);
                m_lstSelect.Add(MyTaskVM.Prop.StatusID.MapAlias);
                m_lstSelect.Add(MyTaskVM.Prop.CurrentApprovalLvl.MapAlias);
                m_lstSelect.Add(MyTaskVM.Prop.TaskOwnerID.MapAlias);
                m_lstSelect.Add(MyTaskVM.Prop.TaskDateTimeStamp.MapAlias);
                m_lstSelect.Add(MyTaskVM.Prop.Remarks.MapAlias);
                m_lstSelect.Add(MyTaskVM.Prop.CreatedBy.MapAlias);
                //m_lstSelect.Add(MyTaskVM.Prop.CreatedRoleID.MapAlias);
                m_lstSelect.Add(MyTaskVM.Prop.CreatedDate.MapAlias);
                m_lstSelect.Add(MyTaskVM.Prop.CreatorFullName.MapAlias);
                m_lstSelect.Add(MyTaskVM.Prop.FPTVendorWinnerDesc.MapAlias);
                m_lstSelect.Add(MyTaskVM.Prop.FPTNegoConfigDescrptions.MapAlias);
                //m_lstSelect.Add(MyTaskVM.Prop.MailNotificationID.MapAlias);
                m_lstSelect.Add(MyTaskVM.Prop.ScheduleID.MapAlias);
                //m_lstSelect.Add(MyTaskVM.Prop.Contents.MapAlias);
                m_lstSelect.Add(MyTaskVM.Prop.Subject.MapAlias);

                List<int> notStatus = new List<int>();
                notStatus.Add((int)TaskStatus.Draft);
                notStatus.Add((int)TaskStatus.Rejected);

                m_lstFilters = new List<object>();
                m_lstFilters.Add(Operator.NotIn);
                m_lstFilters.Add(String.Join(",", notStatus));//Not Draft
                m_objFilter.Add(MyTaskVM.Prop.StatusID.Map, m_lstFilters);

                m_lstFilters = new List<object>();
                m_lstFilters.Add(Operator.In);
                m_lstFilters.Add(string.Join(",", AllowedTaskType));
                m_objFilter.Add(MyTaskVM.Prop.TaskTypeID.Map, m_lstFilters);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(MyTaskVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));
                if (string.IsNullOrEmpty(message))
                {
                     Dictionary<int, DataSet> m_dicMyTaskDA = m_objMTaskDA.SelectBC(0, null, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                    if (m_objMTaskDA.Message == string.Empty)
                    {
                        foreach (DataRow m_drMTasksDA in m_dicMyTaskDA[0].Tables[0].Rows)
                        {
                            MyTaskVM mtVm = new MyTaskVM();
                            if (!m_lsMyTasksVM.Any(d => d.TaskID.Equals(m_drMTasksDA[MyTaskVM.Prop.TaskID.Name].ToString())))
                            {
                                mtVm.TaskID = m_drMTasksDA[MyTaskVM.Prop.TaskID.Name].ToString();
                                mtVm.TaskTypeID = m_drMTasksDA[MyTaskVM.Prop.TaskTypeID.Name].ToString();

                                mtVm.InvitationFunctionID = "";
                                if (mtVm.TaskTypeID == General.EnumDesc(TaskType.NegotiationConfigurations))
                                {
                                    mtVm.TaskDesciption = m_drMTasksDA[MyTaskVM.Prop.FPTNegoConfigDescrptions.Name].ToString();
                                }
                                else if (mtVm.TaskTypeID == General.EnumDesc(TaskType.VendorWinner))
                                {
                                    mtVm.TaskDesciption = m_drMTasksDA[MyTaskVM.Prop.FPTVendorWinnerDesc.Name].ToString();
                                }
                                else if (mtVm.TaskTypeID == General.EnumDesc(TaskType.UploadItem))
                                {
                                    mtVm.TaskDesciption = m_drMTasksDA[MyTaskVM.Prop.TaskDesc.Name].ToString();
                                }
                                else if (isTypeOfInvitationGroup(mtVm.TaskTypeID))
                                {
                                    string fdesc = GetFunctionDesc(mtVm.TaskTypeID, mtVm.TaskID);
                                    mtVm.InvitationFunctionID = GetFunctionID(mtVm.TaskTypeID, mtVm.TaskID);
                                    mtVm.TaskDesciption = "Invitation - " + fdesc + " : " + m_drMTasksDA[MyTaskVM.Prop.Subject.Name].ToString();
                                }
                                mtVm.TaskOwnerID = m_drMTasksDA[MyTaskVM.Prop.TaskOwnerID.Name].ToString();
                                mtVm.TaskDateTimeStamp = DateTime.Parse(m_drMTasksDA[MyTaskVM.Prop.TaskDateTimeStamp.Name].ToString());
                                mtVm.StatusDesc = m_drMTasksDA[MyTaskVM.Prop.StatusDesc.Name].ToString();
                                mtVm.StatusID = (int)m_drMTasksDA[MyTaskVM.Prop.StatusID.Name];
                                mtVm.CurrentApprovalLvl = (int)m_drMTasksDA[MyTaskVM.Prop.CurrentApprovalLvl.Name];

                                mtVm.Remarks = m_drMTasksDA[MyTaskVM.Prop.Remarks.Name] == null ? "" : m_drMTasksDA[MyTaskVM.Prop.Remarks.Name].ToString();
                                //mtVm.MailNotificationID = m_drMTasksDA[MyTaskVM.Prop.MailNotificationID.Name] == null ? "" : m_drMTasksDA[MyTaskVM.Prop.MailNotificationID.Name].ToString();
                                //mtVm.Contents = m_drMTasksDA[MyTaskVM.Prop.Contents.Name] == null ? "" : m_drMTasksDA[MyTaskVM.Prop.Contents.Name].ToString();
                                mtVm.ScheduleID = m_drMTasksDA[MyTaskVM.Prop.ScheduleID.Name] == null ? "" : m_drMTasksDA[MyTaskVM.Prop.ScheduleID.Name].ToString();
                                mtVm.CreatedBy = m_drMTasksDA[MyTaskVM.Prop.CreatedBy.Name] == null ? "" : m_drMTasksDA[MyTaskVM.Prop.CreatedBy.Name].ToString();
                                mtVm.CreatorFullName = m_drMTasksDA[MyTaskVM.Prop.CreatorFullName.Name] == null ? "" : m_drMTasksDA[MyTaskVM.Prop.CreatorFullName.Name].ToString();
                                mtVm.CreatedDate = string.IsNullOrEmpty(m_drMTasksDA[MyTaskVM.Prop.CreatedDate.Name].ToString()) ? (DateTime?)null
                                                    : DateTime.Parse(m_drMTasksDA[MyTaskVM.Prop.CreatedDate.Name].ToString());

                                m_lsMyTasksVM.Add(mtVm);
                            }
                        }
                        if (!string.IsNullOrEmpty(InvitationFunctionID) && InvitationFunctionID != "null")
                        {
                            m_lsMyTasksVM = m_lsMyTasksVM.Where(x => x.InvitationFunctionID == InvitationFunctionID).ToList();
                        }
                    }
                }
            //}
            return this.Store(m_lsMyTasksVM.Skip(m_intSkip).Take(m_intLength), m_lsMyTasksVM.Count);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters, string SuperiorID)
        {
            TTCMembersDA m_objTTCMembersDA = new TTCMembersDA();
            m_objTTCMembersDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcTTCMembers = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcTTCMembers.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = TCMembersVM.Prop.Map(m_strDataIndex, false);

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
            if (SuperiorID != string.Empty)
            {
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(SuperiorID);
                m_objFilter.Add(TCMembersVM.Prop.SuperiorID.Map, m_lstFilter);
            }

            Dictionary<int, DataSet> m_dicTTCMembersDA = m_objTTCMembersDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpTCMemberBL in m_dicTTCMembersDA)
            {
                m_intCount = m_kvpTCMemberBL.Key;
                break;
            }

            List<TCMembersVM> m_lsTCMembersVM = new List<TCMembersVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(TCMembersVM.Prop.TCMemberID.MapAlias);
                m_lstSelect.Add(TCMembersVM.Prop.EmployeeID.MapAlias);
                m_lstSelect.Add(TCMembersVM.Prop.EmployeeName.MapAlias);
                //m_lstSelect.Add(TCMembersVM.Prop.TCTypeDesc.MapAlias);
                //m_lstSelect.Add(TCMembersVM.Prop.SuperiorName.MapAlias);
                //m_lstSelect.Add(TCMembersVM.Prop.PeriodStart.MapAlias);
                //m_lstSelect.Add(TCMembersVM.Prop.PeriodEnd.MapAlias);
                //m_lstSelect.Add(TCMembersVM.Prop.DelegateStartDate.MapAlias);
                //m_lstSelect.Add(TCMembersVM.Prop.DelegateEndDate.MapAlias);


                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(TCMembersVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicTTCMembersDA = m_objTTCMembersDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objTTCMembersDA.Message == string.Empty)
                {
                    m_lsTCMembersVM = (
                        from DataRow m_drTTCMembersDA in m_dicTTCMembersDA[0].Tables[0].Rows
                        select new TCMembersVM()
                        {
                            TCMemberID = m_drTTCMembersDA[TCMembersVM.Prop.EmployeeID.Name].ToString(),
                            EmployeeName = m_drTTCMembersDA[TCMembersVM.Prop.EmployeeName.Name].ToString(),
                            //TCTypeDesc = m_drTTCMembersDA[TCMembersVM.Prop.TCTypeDesc.Name].ToString(),
                            //SuperiorName = m_drTTCMembersDA[TCMembersVM.Prop.SuperiorName.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lsTCMembersVM, m_intCount);
        }

        public ActionResult Home()
        {
            this.GetCmp<Ext.Net.Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 0;
            return this.Direct();
        }



        public ActionResult Detail(string Caller, string Selected, string TaskID, MyTaskVM Task, string FromBtn = "")
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            MyTaskVM m_objMTasksVM = new MyTaskVM();
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
                if (Task == null)
                {
                    NameValueCollection m_nvcParams = this.Request.Params;
                    m_dicSelectedRow = GetFormData(m_nvcParams);
                    if (m_dicSelectedRow.ContainsKey(MyTaskVM.Prop.TaskID.Name)
                        && string.IsNullOrEmpty(m_dicSelectedRow[MyTaskVM.Prop.TaskID.Name].ToString()))
                    {
                        m_dicSelectedRow[MyTaskVM.Prop.TaskID.Name] = TaskID;
                    }
                }
                else
                {
                    m_objMTasksVM = Task;
                    m_dicSelectedRow.Add("TaskID", m_objMTasksVM.TaskID);
                }
            }



            if (m_dicSelectedRow.Count > 0)
                m_objMTasksVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            bool hasMultipleSchedule = GetSchedulesByTask(m_objMTasksVM.TaskID).Count>1;
            if (hasMultipleSchedule) m_objMTasksVM.Summary = "";

            string disableforclarify = m_objMTasksVM.TaskOwnerID == m_objMTasksVM.CreatedRoleID ? "TRUE" : "FALSE";

            //m_objMTasksVM.RoleParentID = GetParentApproval(ref m_strMessage, m_objMTasksVM.TaskTypeID);
            //m_objMTasksVM.RoleChildID = GetChildApproval(ref m_strMessage, m_objMTasksVM.TaskTypeID);

            ViewDataDictionary m_vddWorkCenter = new ViewDataDictionary();
            m_vddWorkCenter.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
            m_vddWorkCenter.Add("FromBtn", JSON.Deserialize(FromBtn));
            //m_vddWorkCenter.Add("MailNotificationID", m_objMTasksVM.MailNotificationID);
            m_vddWorkCenter.Add("isFromInvitation", (isTypeOfInvitationGroup(m_objMTasksVM.TaskTypeID)).ToString());
            m_vddWorkCenter.Add("MailNotificationID", GetNotifID(m_objMTasksVM.TaskID));

            //m_vddWorkCenter.Add("Contents", m_objMTasksVM.Contents);
            m_vddWorkCenter.Add("DisableForClarify", disableforclarify);
            m_vddWorkCenter.Add("TaskID", m_objMTasksVM.TaskID);
            m_vddWorkCenter.Add("isTaskCreator", "FALSE");
            this.GetCmp<Ext.Net.Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objMTasksVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddWorkCenter,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Update(string Caller, string Selected, string FromBtn)
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
            MyTaskVM m_objMTasksVM = new MyTaskVM();
            if (m_dicSelectedRow.Count > 0)
                m_objMTasksVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            bool hasMultipleSchedule = GetSchedulesByTask(m_objMTasksVM.TaskID).Count>1;
            if (hasMultipleSchedule) m_objMTasksVM.Summary = "";

            m_objMTasksVM.ApprovalStatusDesc = "";
            m_objMTasksVM.ApprovalRemarks = "";

            string disableforclarify = m_objMTasksVM.TaskOwnerID == m_objMTasksVM.CreatedRoleID ? "TRUE" : "FALSE";
            string CurrentApprovalRole = GetCurrentApproval(m_objMTasksVM.TaskTypeID, m_objMTasksVM.CurrentApprovalLvl);
            m_objMTasksVM.RoleParentID = GetParentApproval(ref m_strMessage, CurrentApprovalRole, m_objMTasksVM.TaskTypeID);
            m_objMTasksVM.RoleChildID = GetChildApproval(ref m_strMessage, CurrentApprovalRole, m_objMTasksVM.TaskTypeID);

            string SubmitterRoleID = GetCurrentApproval(m_objMTasksVM.TaskTypeID, 0);
            string isTaskOwner = CheckMatchedRole(SubmitterRoleID, ref m_strMessage) ? "TRUE" : "FALSE";

            ViewDataDictionary m_vddMyTasks = new ViewDataDictionary();
            m_vddMyTasks.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddMyTasks.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            m_vddMyTasks.Add("FromBtn", JSON.Deserialize(FromBtn));
            m_vddMyTasks.Add("isFromInvitation", (isTypeOfInvitationGroup(m_objMTasksVM.TaskTypeID)).ToString());
            m_vddMyTasks.Add("MailNotificationID", GetNotifID(m_objMTasksVM.TaskID));
            //m_vddMyTasks.Add("Contents", m_objMTasksVM.Contents);
            m_vddMyTasks.Add("DisableForClarify", disableforclarify);
            m_vddMyTasks.Add("TaskID", m_objMTasksVM.TaskID);
            m_vddMyTasks.Add("isTaskCreator", isTaskOwner);
            this.GetCmp<Ext.Net.Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objMTasksVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddMyTasks,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }



        public ActionResult Preview(string TaskID, string TaskTypeID)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));
            List<string> m_lstSelect = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            UserVM m_userVM = getCurentUser();
            if (TaskTypeID == General.EnumDesc(TaskType.UploadItem))
            {

                #region Get Item Upload


                MItemUploadDA m_objMItemUploadDA = new MItemUploadDA();
                m_objMItemUploadDA.ConnectionStringName = Global.ConnStrConfigName;

                m_lstSelect.Add(ItemUploadVM.Prop.ItemID.MapAlias);
                m_lstSelect.Add(ItemUploadVM.Prop.ItemDesc.MapAlias);
                m_lstSelect.Add(ItemUploadVM.Prop.UoMID.MapAlias);
                m_lstSelect.Add(ItemUploadVM.Prop.ItemGroupID.MapAlias);
                m_lstSelect.Add(ItemUploadVM.Prop.IsActive.MapAlias);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(TaskID);
                m_objFilter.Add(ItemUploadVM.Prop.TaskID.Map, m_lstFilter);

                List<ItemVM> m_lsItemVM = new List<ItemVM>();

                Dictionary<int, DataSet> m_dicMItemDA = m_objMItemUploadDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);
                if (m_objMItemUploadDA.Success && string.IsNullOrEmpty(m_objMItemUploadDA.Message))
                {
                    m_lsItemVM = (from DataRow m_drRow in m_dicMItemDA[0].Tables[0].Rows
                                             select new ItemVM
                                             {
                                                 ItemID = m_drRow[ItemVM.Prop.ItemID.Name].ToString(),
                                                 ItemDesc = m_drRow[ItemVM.Prop.ItemDesc.Name].ToString(),
                                                 UoMID = m_drRow[ItemVM.Prop.UoMID.Name].ToString(),
                                                 ItemGroupID = m_drRow[ItemVM.Prop.ItemGroupID.Name].ToString(),
                                                 IsActive = bool.Parse(m_drRow[ItemVM.Prop.IsActive.Name].ToString())
                                             }).ToList();
                }
                #endregion

                #region Get Item Price Upload

                m_objFilter = new Dictionary<string, List<object>>();
                m_lstSelect = new List<string>();

                DItemPriceUploadDA m_objDItemPriceUploadDA = new DItemPriceUploadDA();
                m_objDItemPriceUploadDA.ConnectionStringName = Global.ConnStrConfigName;

                m_lstSelect.Add(ItemPriceUploadVM.Prop.ItemPriceID.MapAlias);
                m_lstSelect.Add(ItemPriceUploadVM.Prop.ItemID.MapAlias);
                m_lstSelect.Add(ItemPriceUploadVM.Prop.ItemDesc.MapAlias);
                m_lstSelect.Add(ItemPriceUploadVM.Prop.ProjectID.MapAlias);
                m_lstSelect.Add(ItemPriceUploadVM.Prop.RegionID.MapAlias);
                m_lstSelect.Add(ItemPriceUploadVM.Prop.ClusterID.MapAlias);
                m_lstSelect.Add(ItemPriceUploadVM.Prop.UnitTypeID.MapAlias);
                m_lstSelect.Add(ItemPriceUploadVM.Prop.PriceTypeID.MapAlias);
                m_lstSelect.Add(ItemPriceUploadVM.Prop.VendorID.MapAlias);
                m_lstSelect.Add(ItemPriceUploadVM.Prop.IsDefault.MapAlias);
                m_lstSelect.Add(ItemPriceUploadVM.Prop.ValidFrom.MapAlias);
                m_lstSelect.Add(ItemPriceUploadVM.Prop.ValidTo.MapAlias);
                m_lstSelect.Add(ItemPriceUploadVM.Prop.CurrencyID.MapAlias);
                m_lstSelect.Add(ItemPriceUploadVM.Prop.Amount.MapAlias);
                m_lstSelect.Add(ItemPriceUploadVM.Prop.TaskID.MapAlias);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(TaskID);
                m_objFilter.Add(ItemPriceUploadVM.Prop.TaskID.Map, m_lstFilter);

                List<ItemPriceUploadVM> m_lsItemPriceUploadVM = new List<ItemPriceUploadVM>();

                Dictionary<int, DataSet> m_dicDItemPriceVendorUploadDA = m_objDItemPriceUploadDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);
                if (m_objDItemPriceUploadDA.Success && string.IsNullOrEmpty(m_objDItemPriceUploadDA.Message))
                {
                    m_lsItemPriceUploadVM = (from DataRow m_drRow in m_dicDItemPriceVendorUploadDA[0].Tables[0].Rows
                                             select new ItemPriceUploadVM
                                             {
                                                 ItemPriceID = m_drRow[ItemPriceUploadVM.Prop.ItemPriceID.Name].ToString(),
                                                 ItemID = m_drRow[ItemPriceUploadVM.Prop.ItemID.Name].ToString(),
                                                 ItemDesc = m_drRow[ItemPriceUploadVM.Prop.ItemDesc.Name].ToString(),
                                                 ProjectID = m_drRow[ItemPriceUploadVM.Prop.ProjectID.Name].ToString(),
                                                 ClusterID = m_drRow[ItemPriceUploadVM.Prop.ClusterID.Name].ToString(),
                                                 RegionID = m_drRow[ItemPriceUploadVM.Prop.RegionID.Name].ToString(),
                                                 PriceTypeID = m_drRow[ItemPriceUploadVM.Prop.PriceTypeID.Name].ToString(),
                                                 VendorID = m_drRow[ItemPriceUploadVM.Prop.VendorID.Name].ToString(),
                                                 IsDefault = bool.Parse(m_drRow[ItemPriceUploadVM.Prop.IsDefault.Name].ToString()),
                                                 UnitTypeID = m_drRow[ItemPriceUploadVM.Prop.UnitTypeID.Name].ToString(),
                                                 CurrencyID = m_drRow[ItemPriceUploadVM.Prop.CurrencyID.Name].ToString(),
                                                 TaskID = m_drRow[ItemPriceUploadVM.Prop.TaskID.Name].ToString(),
                                                 ValidFrom = DateTime.Parse(m_drRow[ItemPriceUploadVM.Prop.ValidFrom.Name].ToString()),
                                                 ValidTo = DateTime.Parse(m_drRow[ItemPriceUploadVM.Prop.ValidTo.Name].ToString()),
                                                 Amount = decimal.Parse(m_drRow[ItemPriceUploadVM.Prop.Amount.Name].ToString())
                                             }).ToList();
                }
                #endregion

                #region Get Item Details

                m_objFilter = new Dictionary<string, List<object>>();
                m_lstSelect = new List<string>();

                DItemDetailUploadDA m_objDItemDetailUploadDA = new DItemDetailUploadDA();
                m_objDItemDetailUploadDA.ConnectionStringName = Global.ConnStrConfigName;

                m_lstSelect.Add(ItemDetailUploadVM.Prop.ItemDetailUploadID.MapAlias);
                m_lstSelect.Add(ItemDetailUploadVM.Prop.ItemID.MapAlias);
                m_lstSelect.Add(ItemDetailUploadVM.Prop.ItemDesc.MapAlias);
                m_lstSelect.Add(ItemDetailUploadVM.Prop.VendorID.MapAlias);
                m_lstSelect.Add(ItemDetailUploadVM.Prop.ItemDetailTypeID.MapAlias);
                m_lstSelect.Add(ItemDetailUploadVM.Prop.ItemDetailTypeDesc.MapAlias);
                m_lstSelect.Add(ItemDetailUploadVM.Prop.ItemDetailDesc.MapAlias);
                m_lstSelect.Add(ItemDetailUploadVM.Prop.TaskID.MapAlias);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(TaskID);
                m_objFilter.Add(ItemDetailUploadVM.Prop.TaskID.Map, m_lstFilter);

                List<ItemDetailUploadVM> m_lsItemDetailUploadVM = new List<ItemDetailUploadVM>();
                List<ItemDetailUploadVM> m_lsItemDetailUploadToGridVM = new List<ItemDetailUploadVM>();
                Dictionary<int, DataSet> m_dicDItemDetailUploadDA = m_objDItemDetailUploadDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);
                if (m_objDItemDetailUploadDA.Success && string.IsNullOrEmpty(m_objDItemDetailUploadDA.Message))
                {
                    m_lsItemDetailUploadVM = (from DataRow m_drRow in m_dicDItemDetailUploadDA[0].Tables[0].Rows
                                             select new ItemDetailUploadVM
                                             {
                                                 ItemDetailUploadID = m_drRow[ItemDetailUploadVM.Prop.ItemDetailUploadID.Name].ToString(),
                                                 ItemID = m_drRow[ItemDetailUploadVM.Prop.ItemID.Name].ToString(),
                                                 ItemDesc = m_drRow[ItemDetailUploadVM.Prop.ItemDesc.Name].ToString(),
                                                 VendorID = m_drRow[ItemDetailUploadVM.Prop.VendorID.Name].ToString(),
                                                 ItemDetailTypeID = m_drRow[ItemDetailUploadVM.Prop.ItemDetailTypeID.Name].ToString(),
                                                 ItemDetailTypeDesc = m_drRow[ItemDetailUploadVM.Prop.ItemDetailTypeDesc.Name].ToString(),
                                                 ItemDetailDesc = m_drRow[ItemDetailUploadVM.Prop.ItemDetailDesc.Name].ToString()
                                             }).ToList();
                    Array m_objItemDetailTypes = System.Enum.GetValues(typeof(ItemDetailTypes));


                    foreach (var item in m_lsItemDetailUploadVM)
                    {
                        ItemDetailUploadVM objItemDetailUploadVM = m_lsItemDetailUploadToGridVM.Where(d => d.ItemID == item.ItemID && d.VendorID == item.VendorID).FirstOrDefault();

                        if (objItemDetailUploadVM == null)
                        {
                            objItemDetailUploadVM = new ItemDetailUploadVM { ItemID=item.ItemID,ItemDesc=item.ItemDesc,VendorID=item.VendorID };

                            var propertyInfo = objItemDetailUploadVM.GetType().GetProperty(item.ItemDetailTypeDesc.ToString());
                            propertyInfo.SetValue(objItemDetailUploadVM, item.ItemDetailDesc);
                            
                            m_lsItemDetailUploadToGridVM.Add(objItemDetailUploadVM);
                        }
                        else {
                            var propertyInfo = objItemDetailUploadVM.GetType().GetProperty(item.ItemDetailTypeDesc.ToString());
                            propertyInfo.SetValue(objItemDetailUploadVM, item.ItemDetailDesc);
                            
                        }

                    
}
                }
                #endregion

                TabPanel m_tabPanel = new TabPanel();

                #region Items
                GridPanel m_gridPanel = new GridPanel();
                m_gridPanel.Title = "Items";
                m_gridPanel.Collapsible = true;
                m_gridPanel.ID = "grdPnlItemUpload";
                m_gridPanel.Padding = 10;
                //m_gridPanel.MinHeight = 450;
                m_gridPanel.AutoUpdateLayout = true;

                Store m_store = new Store();
                m_store.AutoLoad = true;
                m_store.RemoteSort = false;
                m_store.RemotePaging = false;
                m_store.RemoteFilter = true;
                m_store.PageSize = 15;
                m_store.DataSource = m_lsItemVM;

                Model m_model = new Model();
                ModelField m_modelField = new ModelField();
                m_modelField = new ModelField() { Name = ItemUploadVM.Prop.ItemID.Name };
                m_model.Fields.Add(m_modelField);
                m_modelField = new ModelField() { Name = ItemUploadVM.Prop.ItemDesc.Name };
                m_model.Fields.Add(m_modelField);
                m_modelField = new ModelField() { Name = ItemUploadVM.Prop.ItemGroupID.Name };
                m_model.Fields.Add(m_modelField);
                m_modelField = new ModelField() { Name = ItemUploadVM.Prop.UoMID.Name };
                m_model.Fields.Add(m_modelField);
                m_modelField = new ModelField() { Name = ItemUploadVM.Prop.IsActive.Name };
                m_model.Fields.Add(m_modelField);
                m_modelField = new ModelField() { Name = ItemUploadVM.Prop.TaskID.Name };
                m_model.Fields.Add(m_modelField);
                m_store.Model.Add(m_model);

                m_gridPanel.Store.Add(m_store);


                Ext.Net.Container  m_container = new Ext.Net.Container();
                m_container.CustomConfig.Add(new ConfigItem("getValue", "getValueText", ParameterMode.Raw));
                m_container.Layout = LayoutType.Fit.ToString();
                TextField m_tField = new TextField();
                m_tField.Listeners.Change.Handler = "this.up('grid').filterHeader.onFieldChange(this.up('container'));";
                m_container.Items.Add(m_tField);

                List<ColumnBase> m_columnBase = new List<ColumnBase>();
                ColumnBase m_column = new Column();

                m_column = new Column() { Text = ItemVM.Prop.ItemID.Desc, DataIndex = ItemVM.Prop.ItemID.Name, Flex = 3 };
                m_columnBase.Add(m_column);
                m_column = new Column() { Text = ItemVM.Prop.ItemDesc.Desc, DataIndex = ItemVM.Prop.ItemDesc.Name, Flex = 4 };
                m_columnBase.Add(m_column);
                m_column = new Column() { Text = ItemVM.Prop.ItemGroupID.Desc, DataIndex = ItemVM.Prop.ItemGroupID.Name, Flex = 1 };
                m_columnBase.Add(m_column);
                m_column = new Column() { Text = ItemVM.Prop.UoMID.Desc, DataIndex = ItemVM.Prop.UoMID.Name, Flex = 1 };
                m_columnBase.Add(m_column);
                m_column = new Column() { Text = ItemVM.Prop.IsActive.Desc, DataIndex = ItemVM.Prop.IsActive.Name, Flex = 1 };
                m_columnBase.Add(m_column);

                m_gridPanel.ColumnModel.Add(m_columnBase);

                PagingToolbar m_pageToolbar = new PagingToolbar();
                m_pageToolbar.HideRefresh = true;
                //m_pageToolbar.DisplayInfo = true;
                //m_pageToolbar.BaseCls = "paging";
                //m_pageToolbar.DisplayMsg = "Displaying {0} - {1} of {2}";
                //m_pageToolbar.EmptyMsg = "No records to display";
                m_gridPanel.BottomBar.Add(m_pageToolbar);

                FilterHeader m_filterHeader = new FilterHeader();
                m_filterHeader.Remote = false;
                m_gridPanel.Plugins.Add(m_filterHeader);

                m_tabPanel.Items.Add(m_gridPanel);
                #endregion

                #region Price
                m_gridPanel = new GridPanel();
                m_gridPanel.Title = "Price";
                m_gridPanel.Collapsible = true;
                m_gridPanel.ID = "grdPnlItemPriceUpload";
                m_gridPanel.Padding = 10;
                //m_gridPanel.MinHeight = 450;
                m_gridPanel.AutoUpdateLayout = true;

                m_store = new Store();
                m_store.AutoLoad = true;
                m_store.RemoteSort = false;
                m_store.RemotePaging = false;
                m_store.RemoteFilter = true;
                m_store.PageSize = 15;
                m_store.DataSource = m_lsItemPriceUploadVM;

                m_model = new Model();
                m_modelField = new ModelField();
                m_modelField = new ModelField() { Name = ItemPriceUploadVM.Prop.ItemPriceID.Name };
                m_model.Fields.Add(m_modelField);
                m_modelField = new ModelField() { Name = ItemPriceUploadVM.Prop.ItemID.Name };
                m_model.Fields.Add(m_modelField);
                m_modelField = new ModelField() { Name = ItemPriceUploadVM.Prop.ItemDesc.Name };
                m_model.Fields.Add(m_modelField);
                m_modelField = new ModelField() { Name = ItemPriceUploadVM.Prop.RegionID.Name };
                m_model.Fields.Add(m_modelField);
                m_modelField = new ModelField() { Name = ItemPriceUploadVM.Prop.ProjectID.Name };
                m_model.Fields.Add(m_modelField);
                m_modelField = new ModelField() { Name = ItemPriceUploadVM.Prop.ClusterID.Name };
                m_model.Fields.Add(m_modelField);
                m_modelField = new ModelField() { Name = ItemPriceUploadVM.Prop.UnitTypeID.Name };
                m_model.Fields.Add(m_modelField);
                m_modelField = new ModelField() { Name = ItemPriceUploadVM.Prop.PriceTypeID.Name };
                m_model.Fields.Add(m_modelField);
                m_modelField = new ModelField() { Name = ItemPriceUploadVM.Prop.ValidFrom.Name };
                m_model.Fields.Add(m_modelField);
                m_modelField = new ModelField() { Name = ItemPriceUploadVM.Prop.ValidTo.Name };
                m_model.Fields.Add(m_modelField);
                m_modelField = new ModelField() { Name = ItemPriceUploadVM.Prop.Amount.Name };
                m_model.Fields.Add(m_modelField);
                m_modelField = new ModelField() { Name = ItemPriceUploadVM.Prop.CurrencyID.Name };
                m_model.Fields.Add(m_modelField);
                m_modelField = new ModelField() { Name = ItemPriceUploadVM.Prop.VendorID.Name };
                m_model.Fields.Add(m_modelField);
                m_modelField = new ModelField() { Name = ItemPriceUploadVM.Prop.IsDefault.Name };
                m_model.Fields.Add(m_modelField);
                m_modelField = new ModelField() { Name = ItemPriceUploadVM.Prop.TaskID.Name };
                m_model.Fields.Add(m_modelField);
                m_store.Model.Add(m_model);

                m_gridPanel.Store.Add(m_store);


                m_container = new Ext.Net.Container();
                m_container.CustomConfig.Add(new ConfigItem("getValue", "getValueText", ParameterMode.Raw));
                m_container.Layout = LayoutType.Fit.ToString();
                m_tField = new TextField();
                m_tField.Listeners.Change.Handler = "this.up('grid').filterHeader.onFieldChange(this.up('container'));";
                m_container.Items.Add(m_tField);

                m_columnBase = new List<ColumnBase>();
                ColumnBase m_dcolumn = new Column();
                m_column = new Column();
                m_dcolumn.Text = "Item  Price";
                m_dcolumn.Flex = 4;
                m_column = new Column() { Text = ItemPriceVM.Prop.ItemID.Desc, DataIndex = ItemPriceVM.Prop.ItemID.Name, Flex = 3 };
                m_dcolumn.Columns.Add(m_column);
                m_column = new Column() { Text = ItemVM.Prop.ItemDesc.Desc, DataIndex = ItemVM.Prop.ItemDesc.Name, Flex = 4 };
                m_dcolumn.Columns.Add(m_column);
                m_column = new Column() { Text = ItemPriceVM.Prop.RegionID.Desc, DataIndex = ItemPriceVM.Prop.RegionID.Name, Flex = 1 };
                m_dcolumn.Columns.Add(m_column);
                m_column = new Column() { Text = ItemPriceVM.Prop.ProjectID.Desc, DataIndex = ItemPriceVM.Prop.ProjectID.Name, Flex = 1 };
                m_dcolumn.Columns.Add(m_column);
                m_column = new Column() { Text = ItemPriceVM.Prop.ClusterID.Desc, DataIndex = ItemPriceVM.Prop.ClusterID.Name, Flex = 1 };
                m_dcolumn.Columns.Add(m_column);
                m_column = new Column() { Text = ItemPriceVM.Prop.UnitTypeID.Desc, DataIndex = ItemPriceVM.Prop.UnitTypeID.Name, Flex = 1 };
                m_dcolumn.Columns.Add(m_column);
                m_column = new Column() { Text = ItemPriceVM.Prop.PriceTypeID.Desc, DataIndex = ItemPriceVM.Prop.PriceTypeID.Name, Flex = 1 };
                m_dcolumn.Columns.Add(m_column);
                m_columnBase.Add(m_dcolumn);

                m_dcolumn = new Column();
                m_dcolumn.Text = "Vendor";
                m_dcolumn.Flex = 2;
                m_column = new Column() { Text = ItemPriceVendorPeriodVM.Prop.VendorDesc.Desc, DataIndex = ItemPriceVendorPeriodVM.Prop.VendorDesc.Name, Flex = 1, Visible = false };
                m_dcolumn.Columns.Add(m_column);
                m_column = new Column() { Text = ItemPriceVendorPeriodVM.Prop.VendorID.Desc, DataIndex = ItemPriceVendorPeriodVM.Prop.VendorID.Name, Flex = 1 };
                m_dcolumn.Columns.Add(m_column);
                m_column = new Column() { Text = ItemPriceVendorPeriodVM.Prop.IsDefault.Desc, DataIndex = ItemPriceVendorPeriodVM.Prop.IsDefault.Name, Flex = 1 };
                m_dcolumn.Columns.Add(m_column);
                m_columnBase.Add(m_dcolumn);

                m_dcolumn = new Column();
                m_dcolumn.Text = "Period";
                m_dcolumn.Flex = 3;
                DateColumn m_dtcolumn = new DateColumn() { Text = ItemPriceVendorPeriodVM.Prop.ValidFrom.Desc, DataIndex = ItemPriceVendorPeriodVM.Prop.ValidFrom.Name, Flex = 1, Format = Global.DefaultDateFormat, Align = ColumnAlign.End };
                m_dcolumn.Columns.Add(m_dtcolumn);
                m_dtcolumn = new DateColumn() { Text = ItemPriceVendorPeriodVM.Prop.ValidTo.Desc, DataIndex = ItemPriceVendorPeriodVM.Prop.ValidTo.Name, Flex = 1, Format = Global.DefaultDateFormat, Align = ColumnAlign.End };
                m_dcolumn.Columns.Add(m_dtcolumn);
                NumberColumn m_numcolumn = new NumberColumn() { Text = ItemPriceVendorPeriodVM.Prop.Amount.Desc, DataIndex = ItemPriceVendorPeriodVM.Prop.Amount.Name, Flex = 1, Format = Global.IntegerNumberFormat, Align = ColumnAlign.End };
                m_dcolumn.Columns.Add(m_numcolumn);
                m_columnBase.Add(m_dcolumn);

                m_gridPanel.ColumnModel.Add(m_columnBase);

                m_pageToolbar = new PagingToolbar();
                m_pageToolbar.HideRefresh = true;
                //m_pageToolbar.DisplayInfo = true;
                //m_pageToolbar.BaseCls = "paging";
                //m_pageToolbar.DisplayMsg = "Displaying {0} - {1} of {2}";
                //m_pageToolbar.EmptyMsg = "No records to display";
                m_gridPanel.BottomBar.Add(m_pageToolbar);

                m_filterHeader = new FilterHeader();
                m_filterHeader.Remote = false;
                m_gridPanel.Plugins.Add(m_filterHeader);

                m_tabPanel.Items.Add(m_gridPanel);
                #endregion



                #region Item Details
                m_gridPanel = new GridPanel();
                m_gridPanel.Title = "Specifications";
                m_gridPanel.Collapsible = true;
                m_gridPanel.ID = "grdPnlItemDetailUpload";
                m_gridPanel.Padding = 10;
                //m_gridPanel.MinHeight = 450;
                m_gridPanel.AutoUpdateLayout = true;

                m_store = new Store();
                m_store.AutoLoad = true;
                m_store.RemoteSort = false;
                m_store.RemotePaging = false;
                m_store.RemoteFilter = true;
                m_store.PageSize = 15;
                m_store.Data = m_lsItemDetailUploadToGridVM;

                m_model = new Model();
                m_modelField = new ModelField();
                m_modelField = new ModelField() { Name = ItemDetailUploadVM.Prop.VendorID.Name };
                m_model.Fields.Add(m_modelField);
                m_modelField = new ModelField() { Name = ItemDetailUploadVM.Prop.ItemID.Name };
                m_model.Fields.Add(m_modelField);
                m_modelField = new ModelField() { Name = ItemDetailUploadVM.Prop.ItemDesc.Name };
                m_model.Fields.Add(m_modelField);

                Array m_arrobjItemDetailTypes = System.Enum.GetValues(typeof(ItemDetailTypes));

                foreach (var strItemDetailType in m_arrobjItemDetailTypes)
                {
                    m_modelField = new ModelField() { Name = strItemDetailType.ToString() };
                    m_model.Fields.Add(m_modelField);
                }

                m_gridPanel.Store.Add(m_store);

                m_container = new Ext.Net.Container();
                m_container.CustomConfig.Add(new ConfigItem("getValue", "getValueText", ParameterMode.Raw));
                m_container.Layout = LayoutType.Fit.ToString();
                m_tField = new TextField();
                m_tField.Listeners.Change.Handler = "this.up('grid').filterHeader.onFieldChange(this.up('container'));";
                m_container.Items.Add(m_tField);

                m_columnBase = new List<ColumnBase>();
                m_column = new Column();
                m_column = new Column() { Text = ItemDetailUploadVM.Prop.ItemID.Desc, DataIndex = ItemDetailUploadVM.Prop.ItemID.Name, Flex = 3 };
                m_columnBase.Add(m_column);
                m_column = new Column() { Text = ItemDetailUploadVM.Prop.ItemDesc.Desc, DataIndex = ItemDetailUploadVM.Prop.ItemDesc.Name, Flex = 4 };
                m_columnBase.Add(m_column);
                m_column = new Column() { Text = ItemDetailUploadVM.Prop.VendorID.Desc, DataIndex = ItemDetailUploadVM.Prop.VendorID.Name, Flex = 1 };
                m_columnBase.Add(m_column);
                foreach (var strItemDetailType in m_arrobjItemDetailTypes)
                {
                    m_column = new Column() { Text = strItemDetailType.ToString(), DataIndex = strItemDetailType.ToString(), Flex = 1 };
                    m_columnBase.Add(m_column);
                }

                m_gridPanel.ColumnModel.Add(m_columnBase);

                m_pageToolbar = new PagingToolbar();
                m_pageToolbar.HideRefresh = true;
                //m_pageToolbar.DisplayInfo = true;
                //m_pageToolbar.BaseCls = "paging";
                //m_pageToolbar.DisplayMsg = "Displaying {0} - {1} of {2}";
                //m_pageToolbar.EmptyMsg = "No records to display";
                m_gridPanel.BottomBar.Add(m_pageToolbar);

                m_filterHeader = new FilterHeader();
                m_filterHeader.Remote = false;
                m_gridPanel.Plugins.Add(m_filterHeader);

                m_tabPanel.Items.Add(m_gridPanel);
                #endregion

                return this.ComponentConfig(m_tabPanel);

            }

            if (TaskTypeID == General.EnumDesc(TaskType.Invitation_Schedules))
            {

                try
                {
                    TaskID = JSON.Deserialize<string>(TaskID);
                }
                catch { }

                bool hasMultipleSchedule = GetSchedulesByTask(TaskID).Count>1;
                if(hasMultipleSchedule) return this.ComponentConfig(new Panel() { Html = "Unable to preview "});

                string content_ = "";
                MMailNotificationsDA m_objMMailNotifDA = new MMailNotificationsDA();
                m_objMMailNotifDA.ConnectionStringName = Global.ConnStrConfigName;

                m_lstSelect = new List<string>();
                m_lstSelect.Add(MailNotificationsVM.Prop.MailNotificationID.MapAlias);
                m_lstSelect.Add(MailNotificationsVM.Prop.Contents.MapAlias);

                m_objFilter = new Dictionary<string, List<object>>();
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(TaskID);
                m_objFilter.Add(MailNotificationsVM.Prop.TaskID.Map, m_lstFilter);

                Dictionary<int, DataSet> m_dicMMailNotifDA = m_objMMailNotifDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null, null);
                if (m_objMMailNotifDA.Success && string.IsNullOrEmpty(m_objMMailNotifDA.Message))
                {
                    DataRow m_drMMailNotifDA = m_dicMMailNotifDA[0].Tables[0].Rows[0];
                    content_ = m_drMMailNotifDA[MailNotificationsVM.Prop.Contents.Name].ToString();

                    //ViewDataDictionary m_vdd = new ViewDataDictionary();
                    //m_vdd.Add("content", content_);

                    MailNotificationsVM m_MailNotificationsVM = GetMailNotificationsVM(m_drMMailNotifDA[MailNotificationsVM.Prop.MailNotificationID.Name].ToString());
                    m_MailNotificationsVM.RecipientsTO = string.Join(";", m_MailNotificationsVM.RecipientsVM.Where(x => x.RecipientTypeID == ((int)Enum.RecipientTypes.TO).ToString()).Select(y => y.MailAddress));
                    m_MailNotificationsVM.RecipientsCC = string.Join(";", m_MailNotificationsVM.RecipientsVM.Where(x => x.RecipientTypeID == ((int)Enum.RecipientTypes.CC).ToString()).Select(y => y.MailAddress));
                    m_MailNotificationsVM.RecipientsBCC = string.Join(";", m_MailNotificationsVM.RecipientsVM.Where(x => x.RecipientTypeID == ((int)Enum.RecipientTypes.BCC).ToString()).Select(y => y.MailAddress));
                    m_MailNotificationsVM.NotificationAttachmentName = string.Join(",", m_MailNotificationsVM.NotificationAttachmentVM.Select(x => x.Filename).ToList());


                    //return RedirectToAction("Preview", "MailNotifications", new { MailNotificationID = m_drMMailNotifDA[MailNotificationsVM.Prop.MailNotificationID.Name].ToString() });
                    Panel m_Panel = new Panel();
                    m_Panel.ID = "previewEmail";
                    m_Panel.Layout = LayoutType.Fit.ToString();
                    TextField m_txtToField = new TextField();
                    m_txtToField.FieldLabel = "TO";
                    m_txtToField.Padding = 5;
                    m_txtToField.Value = m_MailNotificationsVM.RecipientsTO;
                    m_txtToField.ReadOnly = true;
                    m_Panel.Add(m_txtToField);


                    TextField m_txtCCField = new TextField();
                    m_txtCCField.FieldLabel = "CC";
                    m_txtCCField.Padding = 5;
                    m_txtCCField.ReadOnly = true;
                    m_txtCCField.Value = m_MailNotificationsVM.RecipientsCC;
                    m_Panel.Add(m_txtCCField);

                    TextField m_txtBccField = new TextField();
                    m_txtBccField.FieldLabel = "BCC";
                    m_txtBccField.Padding = 5;
                    m_txtBccField.ReadOnly = true;
                    m_txtBccField.Value = m_MailNotificationsVM.RecipientsBCC;
                    m_Panel.Add(m_txtBccField);

                    TextField m_txtSubjectField = new TextField();
                    m_txtSubjectField.FieldLabel = "Subject";
                    m_txtSubjectField.Padding = 5;
                    m_txtSubjectField.ReadOnly = true;
                    m_txtSubjectField.Value = m_MailNotificationsVM.Subject;
                    m_Panel.Add(m_txtSubjectField);

                    TextField m_txtAttcField = new TextField();
                    m_txtAttcField.FieldLabel = "Attachment";
                    m_txtAttcField.Padding = 5;
                    m_txtAttcField.ReadOnly = true;
                    m_txtAttcField.Value = m_MailNotificationsVM.NotificationAttachmentName;
                    m_Panel.Add(m_txtAttcField);

                    HtmlEditor html = new HtmlEditor();
                    html.StyleHtmlContent = false;
                    html.Value = m_MailNotificationsVM.Contents;
                    html.Height = 500;
                    html.Width = m_Panel.Width;
                    html.ReadOnly = true; 
                    html.Listeners.AfterRender.Handler = "function(editor) {var toolbar = editor.getToolbar();toolbar.hideParent = true;toolbar.hide();editor.setWidth(App.previewEmail.getWidth()-10);}";
                    
                    Panel m_content = new Panel();
                    m_content.Frame = true;
                    m_content.Shadow = true;
                    m_content.BodyPadding = 10;
                    m_content.Add(html);
                    m_Panel.Scrollable = ScrollableOption.Vertical;
                    m_Panel.Add(m_content);
                    return this.ComponentConfig(m_Panel);
                }
                else
                {
                    return this.ComponentConfig(new Panel() { Html = "Error Preview " + m_objMMailNotifDA.Message });
                }
            }
            else if (TaskTypeID == General.EnumDesc(TaskType.VendorWinner) || TaskTypeID == General.EnumDesc(TaskType.NegotiationConfigurations))
            {
                //if vendor winner / nego config
                try
                {
                    FormPanel vp = new FormPanel();
                    vp.Collapsible = false;
                    vp.Layout = "ColumnLayout";
                    Panel panel;

                    //ambil FPTID based on Task ID
                    MTasksDA m_objMTaskDA = new MTasksDA();
                    m_objMTaskDA.ConnectionStringName = Global.ConnStrConfigName;

                    m_objFilter = new Dictionary<string, List<object>>();
                    m_lstFilter = new List<object>();

                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(TaskID);
                    m_objFilter.Add(MyTaskVM.Prop.TaskID.Map, m_lstFilter);

                    m_lstSelect = new List<string>();
                    m_lstSelect.Add(MyTaskVM.Prop.FPTVendorWinnerFPTID.MapAlias);

                    Dictionary<int, DataSet> m_dicMyTaskDA = m_objMTaskDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

                    string FPTID = "";
                    FPTID = m_dicMyTaskDA[0].Tables[0].Rows[0][MyTaskVM.Prop.FPTVendorWinnerFPTID.Name].ToString();

                    DFPTVendorParticipantsDA m_objVendorParticipants = new DFPTVendorParticipantsDA();
                    m_objVendorParticipants.ConnectionStringName = Global.ConnStrConfigName;

                    m_lstFilter = new List<object>();
                    m_objFilter = new Dictionary<string, List<object>>();

                    m_lstFilter.Add(Operator.None);
                    m_lstFilter.Add(String.Empty);
                    m_objFilter.Add(String.Format("{0} IN (select NegotiationConfigID from CNegotiationConfigurations where FPTID = '{1}')", FPTVendorParticipantsVM.Prop.NegotiationConfigID.Map, FPTID), m_lstFilter);


                    Dictionary<int, DataSet> m_dicVendorParticipantsDA = m_objVendorParticipants.SelectBC(FPTID, m_objFilter);

                    string table = "";
                    table = "<div style='overflow:auto;max-height: 200px;'><table style='width: 100%;' class='table table-bordered table-responsive table-striped table-fixed'>" +
                                "<thead>" +
                                    "<tr>" +
                                        "<th style='text-align: center;'>Item ID</th>" +
                                        "<th style='text-align: center;'>Item Desc</th>" +
                                         "<th style='text-align: center;'>TRM</th>";

                    for(int i = 3; i < m_dicVendorParticipantsDA[0].Tables[1].Columns.Count;i++)
                    {
                        table += String.Format("<th style='text-align: center;'>{0}</th>", m_dicVendorParticipantsDA[0].Tables[1].Columns[i].ColumnName);
                    }

                    table += "</tr>" +
                                "</thead>";

                    //body
                    table += "<tbody>";

                    int i_row = 0;

                    if (m_dicVendorParticipantsDA[0].Tables[0].Rows.Count != m_dicVendorParticipantsDA[0].Tables[1].Rows.Count)
                    {
                        throw new Exception("RAB Rows Number and Vendor Bid Rows Number not same");
                    }


                    for (i_row = 0;i_row < m_dicVendorParticipantsDA[0].Tables[0].Rows.Count;i_row++)
                    {
                        table += "<tr "+(m_dicVendorParticipantsDA[0].Tables[0].Rows[i_row]["ItemParentID"].ToString() == "0" ? "style='font-weight:bold'" : "") +">";
                        table += String.Format("<td style='text-align: left;'>{0}</td><td style='text-align: left;'>{1}</td><td style='text-align: right;'>{2}</td>",
                            m_dicVendorParticipantsDA[0].Tables[0].Rows[i_row]["ItemID"].ToString(), m_dicVendorParticipantsDA[0].Tables[0].Rows[i_row]["ItemDesc"].ToString(),
                            String.Format("{0:n0}", decimal.Parse(m_dicVendorParticipantsDA[0].Tables[0].Rows[i_row]["BudgetPlanDefaultValue"].ToString())));

                        for (int i = 3; i < m_dicVendorParticipantsDA[0].Tables[1].Columns.Count; i++)
                        {
                            table += String.Format("<td style='text-align: right;'>{0}</td>", String.Format("{0:n0}", decimal.Parse(m_dicVendorParticipantsDA[0].Tables[1].Rows[i_row][i].ToString())));
                        }

                        table += "</tr>";
                    }

                    table += "</tbody>";

                    table += "</table></div>";

                    vp.Html = table;

                    return this.ComponentConfig(vp);
                }
                catch(Exception ex)
                {
                    return this.ComponentConfig(new Panel() { Html = ex.Message });

                }
               
            }
            else
            {
                return this.ComponentConfig(new Panel() { Html = "No data to display!" });
            }
        }

        private void M_Editor_AfterClientInit(Observable sender)
        {
            throw new NotImplementedException();
        }

        public ActionResult Browse(string ControlTCMemberID, string ControlEmployeeName, string FilterTCMemberID = "", string FilterEmployeeName = "", string FilterSuperiorID = "")
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddTCMember = new ViewDataDictionary();
            m_vddTCMember.Add("Control" + TCMembersVM.Prop.TCMemberID.Name, ControlTCMemberID);
            m_vddTCMember.Add("Control" + TCMembersVM.Prop.EmployeeName.Name, ControlEmployeeName);
            m_vddTCMember.Add(TCMembersVM.Prop.TCMemberID.Name, FilterTCMemberID);
            m_vddTCMember.Add(TCMembersVM.Prop.EmployeeName.Name, FilterEmployeeName);
            m_vddTCMember.Add(TCMembersVM.Prop.SuperiorID.Name, FilterSuperiorID);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddTCMember,
                ViewName = "../TCMember/_Browse"
            };
        }
        protected string GetFirstApprovalRoleStraight(string TaskType, ref string message)
        {

            ApprovalPathVM m_objApppathVM = new ApprovalPathVM();
            CApprovalPathDA m_objApppathDA = new CApprovalPathDA();
            m_objApppathDA.ConnectionStringName = Global.ConnStrConfigName;
            string FirstRoleApproval = "";

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ApprovalPathVM.Prop.RoleID.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(TaskType);
            m_objFilter.Add(ApprovalPathVM.Prop.TaskTypeID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(string.Empty);
            m_objFilter.Add(ApprovalPathVM.Prop.RoleChildID.Map, m_lstFilter);

            //List<string> CurrentRoleID = GetAllRoleID(ref message);
            //if (!string.IsNullOrEmpty(message))
            //    return FirstRoleApproval;


            //m_lstFilter = new List<object>();
            //m_lstFilter.Add(Operator.In);
            //m_lstFilter.Add(string.Join(",", CurrentRoleID));
            //m_objFilter.Add(ApprovalPathVM.Prop.RoleID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMTasksDA = m_objApppathDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objApppathDA.Success)
            {
                if (m_objApppathDA.AffectedRows <= 0)
                {
                    message = "Role doesn't match to ApprovalPath";
                    return FirstRoleApproval;
                }
                DataRow m_drAppPathDA = m_dicMTasksDA[0].Tables[0].Rows[0];
                FirstRoleApproval = m_drAppPathDA[ApprovalPathVM.Prop.RoleID.Name].ToString();

            }
            else
            {
                message = m_objApppathDA.Message;
                return FirstRoleApproval;
            }


            return FirstRoleApproval;
        }

        private ApprovalPathVM GetParentChildApproval(string TaskType,string TaskOwnerID)
        {
            ApprovalPathVM lstAppPath = new ApprovalPathVM();
            ApprovalPathVM m_objApppathVM = new ApprovalPathVM();
            CApprovalPathDA m_objApppathDA = new CApprovalPathDA();
            m_objApppathDA.ConnectionStringName = Global.ConnStrConfigName;
            string RoleCreator = TaskOwnerID;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ApprovalPathVM.Prop.RoleID.MapAlias);
            m_lstSelect.Add(ApprovalPathVM.Prop.RoleChildID.MapAlias);
            m_lstSelect.Add(ApprovalPathVM.Prop.RoleParentID.MapAlias);

            Dictionary<int, DataSet> m_dicMTasksDA = new Dictionary<int, DataSet>();

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();

            m_lstFilter = new List<object>
            {
                Operator.Equals,
                TaskType
            };
            m_objFilter.Add(ApprovalPathVM.Prop.TaskTypeID.Map, m_lstFilter);

            m_lstFilter = new List<object>
            {
                Operator.None,
                String.Empty
            };
            m_objFilter.Add(String.Format("({0} = '{1}' OR {2} = '{1}')", ApprovalPathVM.Prop.RoleID.Map, TaskOwnerID, ApprovalPathVM.Prop.RoleChildID.Map), m_lstFilter);

            m_dicMTasksDA = m_objApppathDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objApppathDA.Success)
            {
                if (m_objApppathDA.AffectedRows <= 0)
                    throw new Exception("Role " + RoleCreator + " doesn't match to ApprovalPath");

                foreach (DataRow m_drAppPathDA in m_dicMTasksDA[0].Tables[0].Rows)
                {
                    lstAppPath = new ApprovalPathVM
                    {
                        RoleID = m_drAppPathDA[ApprovalPathVM.Prop.RoleID.Name].ToString(),
                        RoleChildID = m_drAppPathDA[ApprovalPathVM.Prop.RoleChildID.Name].ToString(),
                        RoleParentID = m_drAppPathDA[ApprovalPathVM.Prop.RoleParentID.Name].ToString()
                    };
                }
            }
            else
                throw new Exception(m_objApppathDA.Message);

            return lstAppPath;
        }

        protected string GetFirstApprovalRoleByOwner(string TaskType, string TaskOwnerID, ref string message)
        {
            bool EndLoop = false;
            

            ApprovalPathVM cr = new ApprovalPathVM();
            string RoleCreator = String.Empty;
            try
            {
                while (!EndLoop)
                {
                    cr = GetParentChildApproval(TaskType, TaskOwnerID);
                    RoleCreator = cr.RoleID;
                    
                    EndLoop = cr.RoleChildID == string.Empty;

                    if(EndLoop && TaskType == General.EnumDesc(Enum.TaskType.UploadItem))
                    {
                        RoleCreator = cr.RoleParentID;
                    }

                    TaskOwnerID = cr.RoleChildID;
                }
            }
            catch (Exception e)
            {
                message = e.Message;
            }

            return RoleCreator;
        }

        [ValidateInput(false)]
        public ActionResult Save(string Action, string FromBtn = "", string isApproved = "", string selected = "")
        {
            //action = JSON.Deserialize<string>(action);
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MTasksDA m_objMTasksDA = new MTasksDA();
            DTaskDetailsDA m_objDTaskDetailsDA = new DTaskDetailsDA();
            List<MyTaskVM> m_arrRowTask = new List<MyTaskVM>();
            //Todo: simpiflied filter
            FPTVM m_objFPTVM = new FPTVM();
            List<NegotiationConfigurationsVM> m_lstNegotiationConfigurationsVM = new List<NegotiationConfigurationsVM>();
            List<FPTVendorParticipantsVM> m_lstFPTVendorParticipantsVM = new List<FPTVendorParticipantsVM>();
            List<TCMembersVM> m_lstTCMembersVM = new List<TCMembersVM>();
            List<string> ListMailNotifID = new List<string>();
            List<FPTVendorWinnerVM> m_lstFPTVendorWinnerVM = new List<FPTVendorWinnerVM>();
            MailNotificationsVM m_MailNotificationsVM = new MailNotificationsVM();
            List<MailNotificationsVM> m_lsMailNotificationsVM = new List<MailNotificationsVM>();

            m_objDTaskDetailsDA.ConnectionStringName = Global.ConnStrConfigName;
            m_objMTasksDA.ConnectionStringName = Global.ConnStrConfigName;
            string messageErr = "";
            string m_strTransName = "MTasks";
            object m_objDBConnection = null;
            m_objDBConnection = m_objMTasksDA.BeginTrans(m_strTransName,IsolationLevel.ReadUncommitted);
            string m_strTaskID = "";
            bool sentSuccess = false;
            string MailNotifID = "";
            bool hasMultipleSchedule = false;
            bool IsBatch = false;

            try
            {
                m_strTaskID = this.Request.Params[MyTaskVM.Prop.TaskID.Name];
                m_lstFPTVendorWinnerVM = getFPTVendorWinnerVM(m_strTaskID);
                string m_strTaskOwneID = this.Request.Params[MyTaskVM.Prop.TaskOwnerID.Name];
                string m_strTaskTypeID = this.Request.Params[MyTaskVM.Prop.TaskTypeID.Name];
                string m_strLastStatus = this.Request.Params[MyTaskVM.Prop.StatusID.Name];
                string m_strStatus = this.Request.Params[MyTaskVM.Prop.ApprovalStatusID.Name];
                string m_strStatusDesc = this.Request.Params[MyTaskVM.Prop.ApprovalStatusDesc.Name];
                string m_strRemakrs = this.Request.Params[MyTaskVM.Prop.ApprovalRemarks.Name];
                string m_strRoleID = this.Request.Params[MyTaskVM.Prop.TaskOwnerID.Name];
                string m_strRoleParentID = this.Request.Params[MyTaskVM.Prop.RoleParentID.Name];
                string m_strRoleChildID = this.Request.Params[MyTaskVM.Prop.RoleChildID.Name];
                string m_strCreatorRole = GetFirstApprovalRoleByOwner(m_strTaskTypeID, m_strTaskOwneID, ref messageErr);
                string UpdateDateTime = this.Request.Params[MyTaskVM.Prop.TaskDateTimeStamp.Name];
                string CurrentAppLevel = this.Request.Params[MyTaskVM.Prop.CurrentApprovalLvl.Name];
                int currentApprovalLevel = int.Parse(CurrentAppLevel);
                string ApprovalLevelInterval = this.Request.Params[MyTaskVM.Prop.ConfigApprovalInterval.Name];
                int approvalLevelInterval = int.Parse(ApprovalLevelInterval);

                if (m_strTaskTypeID == General.EnumDesc(TaskType.MailNotification) || m_strTaskTypeID == General.EnumDesc(TaskType.MinutesOfMeeting))
                {
                    List<string> mes = new List<string>();
                    m_MailNotificationsVM = GetMailNotificationsVM(m_strTaskID, ref mes);
                }

                if (m_strTaskTypeID == General.EnumDesc(TaskType.NegotiationConfigurations))
                {
                    List<string> mes = new List<string>();

                    m_lstNegotiationConfigurationsVM = GetlstNegotiationConfigurationsVM(m_strTaskID, ref mes);
                    m_lstFPTVendorParticipantsVM = GetFPTVendorParticipantsVM(m_lstNegotiationConfigurationsVM.FirstOrDefault().FPTID, ref mes);
                    m_objFPTVM = GetFPTVM(m_lstNegotiationConfigurationsVM.FirstOrDefault().FPTID);
                    m_lstTCMembersVM = GetTCMembersVM(m_lstNegotiationConfigurationsVM.Where(x => x.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.TCMember)).Select(y => y.ParameterValue).ToList(), ref mes);
                }

                if (m_strTaskTypeID == General.EnumDesc(TaskType.Invitation_Schedules))
                {

                    List<string> mes = new List<string>();
                    List<SchedulesVM> m_lstScheduleVM = GetSchedulesByTask(m_strTaskID);
                    IsBatch = m_lstScheduleVM.Select(f=>f.IsBatchMail).FirstOrDefault();
                    hasMultipleSchedule = m_lstScheduleVM.Count > 1;
                    m_lsMailNotificationsVM = GetListMailNotificationsVM(m_strTaskID, ref mes);
                }

                bool Approved_ = true;


                //validation Child Approval
                if (currentApprovalLevel > 0 && string.IsNullOrEmpty(m_strRoleChildID))
                {
                    m_lstMessage.Add("Error Child Approval");
                    m_objMTasksDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                    Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
                    return this.Direct(true);
                }

                bool isApp = !string.IsNullOrEmpty(isApproved) ? Convert.ToBoolean(isApproved) : false;

                if (isApp)
                {
                    Approved_ = Convert.ToBoolean((JSON.Deserialize(isApproved)));
                    if (selected.Length > 0)
                    {
                        m_arrRowTask = JSON.Deserialize<List<MyTaskVM>>(selected).ToList();

                        foreach (MyTaskVM o in m_arrRowTask)
                        {
                            m_strTaskID = o.TaskID;
                            m_strTaskOwneID = o.TaskOwnerID;
                            m_strLastStatus = o.StatusID.ToString();
                            m_strStatus = o.ApprovalStatusID.ToString();
                            m_strRemakrs = o.ApprovalRemarks;
                            m_strRoleID = o.RoleID;
                            m_strRoleParentID = o.RoleParentID;
                            m_strRoleChildID = o.RoleChildID;
                            m_strCreatorRole = o.CreatedRoleID;
                            UpdateDateTime = o.TaskDateTimeStamp.ToString();
                        }
                    }
                }

                //If Update from Clarify
                /*string getLastRole = "";
                if (m_strCreatorRole == m_strTaskOwneID)
                {
                    if (GetPreviousApproval(m_strTaskOwneID, currentApprovalLevel).TryGetValue(true, out getLastRole))
                        m_strRoleParentID = getLastRole;
                    else
                    {
                        m_lstMessage.Add("Error Get Last Approval Role - " + getLastRole);
                        m_objMTasksDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                        Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
                        return this.Direct(true);
                    }
                }*/

                /*DFPTStatus
                 *StatusID | StatusDesc
                 *----------------------
                    0	   |  Revise        => back to 1st and update from NegoConfig then stick flow , MTasks Status = Revise (1)
                    1	   |  Clarify       => back to 1st and update from mytask then back to before, MTasks Status = In Progress(0)
                    2	   |  Approve       => next approval, MTasks Status = Approved if last path else In Progress (2/0)
                    3	   |  Reject        => back to 1st and update from NegoConfig then MTasks Status = Rejected (3) then back to submitter and stick flow
                 
                 Note :
                 Case when task type is upload item, when revise go back to 2nd role, because first role is Vendor [taufik 25 Oct 2019]
                 
                 */
                string TaskOwner = m_strTaskOwneID;
                int TaskStatus = 0;
                switch (m_strStatusDesc)
                {
                    case "Revise":                           //DFPTStatus = Revise
                        TaskStatus = (m_strTaskTypeID == General.EnumDesc(TaskType.UploadItem) ? 0 : 1);                 //Mtask Status = Revise
                        currentApprovalLevel = (m_strTaskTypeID == General.EnumDesc(TaskType.UploadItem) ? 1 : 0);       //
                        TaskOwner = m_strCreatorRole;   //Back To Submitter Role
                        break;
                    case "Clarify":                           //Clarify
                        TaskStatus = 0;                 //In Progress
                        TaskOwner = m_strCreatorRole;   //Back To Submitter Role
                        currentApprovalLevel -= 1;      //
                        break;
                    case "Approve":                           //Approve
                        currentApprovalLevel += 1;
                        //Validation parent
                        if ((currentApprovalLevel < approvalLevelInterval) && string.IsNullOrEmpty(m_strRoleParentID))
                        {
                            m_lstMessage.Add("Error Parent Approval");
                            m_objMTasksDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
                            return this.Direct(true);
                        }
                        //Next Approval
                        TaskStatus = currentApprovalLevel >= approvalLevelInterval ? 2 : 0;                              //if end -> Approved else In Progress
                        TaskOwner = currentApprovalLevel >= approvalLevelInterval ? String.Empty : m_strRoleParentID;   //if end -> "" else ParentRole
                        break;
                    case "Reject":                                           //Reject
                        currentApprovalLevel = 0;
                        TaskStatus = 3;                                 //Rejected
                        TaskOwner = m_strCreatorRole;                   // Back To Submitter
                        break;
                }
                //isvalid
                if (m_lstMessage.Count <= 0)
                {
                    #region Update MTask
                    MTasks m_objMTasks = new MTasks();
                    m_objMTasks.TaskID = m_strTaskID;
                    m_objMTasksDA.Data = m_objMTasks;
                    m_objMTasksDA.Select();

                    m_objMTasks.TaskOwnerID = TaskOwner;
                    m_objMTasks.Remarks = m_strRemakrs;
                    m_objMTasks.StatusID = TaskStatus;
                    m_objMTasks.CurrentApprovalLvl = currentApprovalLevel;
                    m_objMTasks.TaskDateTimeStamp = DateTime.Parse(UpdateDateTime);

                    m_objMTasksDA.Update(true, m_objDBConnection);
                    #endregion

                    #region Insert DTaskDetails
                    DTaskDetails m_objDetailTask = new DTaskDetails();
                    m_objDetailTask.TaskDetailID = Guid.NewGuid().ToString().Replace("-", "");
                    m_objDetailTask.TaskID = m_strTaskID;
                    m_objDetailTask.StatusID = Convert.ToInt16(m_strStatus);
                    m_objDetailTask.Remarks = m_strRemakrs;
                    m_objDTaskDetailsDA.Data = m_objDetailTask;
                    m_objDTaskDetailsDA.Insert(true, m_objDBConnection);
                    #endregion

                    #region DFPTStatus
                    if (m_strStatus == "3")
                    {
                        //DFPTStatusDA m_objDFPTStatusDA = new DFPTStatusDA();
                        //DFPTStatus m_objDFPTStatus = new DFPTStatus();
                        //m_objDFPTStatusDA.ConnectionStringName = Global.ConnStrConfigName;

                        //m_objDFPTStatusDA.Data = m_objDFPTStatus;
                        //m_objDFPTStatus.FPTID = m_strFPTID;
                        //TODO get FPTID
                        //m_objDFPTStatus.StatusDateTimeStamp = DateTime.Now;
                        //m_objDFPTStatus.StatusID = 8; //Draft
                        //m_objDFPTStatusDA.Insert(true, m_objDBConnection);
                    }
                    #endregion

                    if (!m_objMTasksDA.Success || m_objMTasksDA.Message != string.Empty || !m_objDTaskDetailsDA.Success || m_objDTaskDetailsDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMTasksDA.Message + " " + m_objDTaskDetailsDA.Message);
                    //Update Vendor Winner
                    // m_strTaskTypeID m_strTaskID
                    string message_ = "";

                    if (!m_lstMessage.Any())
                    {
                        if (m_strTaskTypeID == General.EnumDesc(TaskType.VendorWinner) && TaskStatus == 2)
                        {
                            UpdateVendorWinner(m_lstFPTVendorWinnerVM, m_objDBConnection, ref m_lstMessage);
                        }
                        else if (m_strTaskTypeID == General.EnumDesc(TaskType.MailNotification) && TaskStatus == 2)
                        {
                            bool m_sendsuccess = SendMail(m_MailNotificationsVM, ref m_lstMessage, ref m_objDBConnection);
                        }
                        else if (m_strTaskTypeID == General.EnumDesc(TaskType.NegotiationConfigurations) && TaskStatus == 2 && AutoEmailActive())
                        {
                            CreateMailNotifNego(m_lstNegotiationConfigurationsVM, m_lstFPTVendorParticipantsVM, m_lstTCMembersVM, m_objFPTVM, ref m_lstMessage);
                        }
                        else if ((m_strTaskTypeID == General.EnumDesc(TaskType.Invitation_Schedules) ||
                                 (m_strTaskTypeID == General.EnumDesc(TaskType.Invitation_Pretender))))
                        {
                            //MailNotifID = this.Request.Params[MyTaskVM.Prop.MailNotificationID.Name];
                            if (TaskStatus == (int)Enum.TaskStatus.Approved)
                            {
                                try
                                {
                                    sentSuccess = UpdateMSchedule(this.Request.Params[MyTaskVM.Prop.ScheduleID.Name], m_strTaskID, (int)Enum.TaskStatus.Approved, m_objDBConnection,  ref message_, ref ListMailNotifID, hasMultipleSchedule, m_lsMailNotificationsVM);
                                    if (!sentSuccess)
                                        m_lstMessage.Add("Update Schedule Error. " + "<br />Sending mail error");
                                }
                                catch (Exception e)
                                {
                                    m_lstMessage.Add("Update Schedule Error. " + e.Message);
                                }
                            }
                            else if (TaskStatus == (int)Enum.TaskStatus.Rejected)
                            {
                                UpdateMSchedule(this.Request.Params[MyTaskVM.Prop.ScheduleID.Name], m_strTaskID, (int)Enum.TaskStatus.Rejected, m_objDBConnection, ref message_, ref ListMailNotifID);
                            }
                            else if (TaskStatus == (int)Enum.TaskStatus.Revise)
                            {
                                UpdateMSchedule(this.Request.Params[MyTaskVM.Prop.ScheduleID.Name], m_strTaskID, (int)Enum.TaskStatus.Revise, m_objDBConnection, ref message_, ref ListMailNotifID);
                            }
                        }
                        else if (m_strTaskTypeID == General.EnumDesc(TaskType.MinutesOfMeeting))
                        {
                            if (TaskStatus == (int)Enum.TaskStatus.Approved)
                            {
                                UpdateTMinuteEntries(m_MailNotificationsVM.MailNotificationID, m_objDBConnection, (int)MinutesStatus.Approved, ref message_);
                                if (message_ == string.Empty)
                                {
                                    bool m_sendsuccess = SendMail(m_MailNotificationsVM, ref m_lstMessage, ref m_objDBConnection);
                                }
                            }
                            else if (TaskStatus == (int)Enum.TaskStatus.Rejected)
                            {
                                UpdateTMinuteEntries(m_MailNotificationsVM.MailNotificationID, m_objDBConnection, (int)MinutesStatus.Deleted, ref message_);
                            }
                            else if (TaskStatus == (int)Enum.TaskStatus.Revise)
                            {
                                UpdateTMinuteEntries(m_MailNotificationsVM.MailNotificationID, m_objDBConnection, (int)MinutesStatus.Draft, ref message_);

                            }
                        }
                        else if (currentApprovalLevel == approvalLevelInterval && m_strTaskTypeID == General.EnumDesc(TaskType.UploadItem))
                        {
                            if (TaskStatus == (int)Enum.TaskStatus.Approved)
                            {
                                string message = string.Empty;
                                string m_strListItemUpload = this.Request.Params["ListItemUpload"].ToString();
                                string m_strListItemPriceUpload = this.Request.Params["ListItemPriceUpload"].ToString();
                                string m_strListItemDetailUpload = this.Request.Params["ListItemDetailUpload"].ToString();
                                List<ItemUploadVM> m_objItemUploadVM = JSON.Deserialize<List<ItemUploadVM>>(m_strListItemUpload);
                                List<ItemPriceUploadVM> m_objItemPriceUploadVM = JSON.Deserialize<List<ItemPriceUploadVM>>(m_strListItemPriceUpload);
                                List<ItemDetailUploadVM> m_objItemDetailUploadVM = JSON.Deserialize<List<ItemDetailUploadVM>>(m_strListItemDetailUpload);
                                UpdateItemPrice(m_objItemUploadVM,m_objItemPriceUploadVM, m_objItemDetailUploadVM, m_objDBConnection, ref message);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                m_objMTasksDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                m_lstMessage.Add(ex.Message);
            }
            if (m_lstMessage.Count <= 0)
            {
                m_objMTasksDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
                if (sentSuccess)
                {
                    MMailNotificationsDA m_objMMailNotifDA = new MMailNotificationsDA();
                    foreach (string MailNotifID_ in ListMailNotifID)
                    {
                        m_objMMailNotifDA = new MMailNotificationsDA();
                        m_objMMailNotifDA.ConnectionStringName = Global.ConnStrConfigName;
                        m_objMMailNotifDA.Data = new MMailNotifications();
                        m_objMMailNotifDA.Data.MailNotificationID = MailNotifID_;
                        m_objMMailNotifDA.Select();
                        if (m_objMMailNotifDA.Success && string.IsNullOrEmpty(m_objMMailNotifDA.Message))
                        {
                            string mssg = "";
                            sentSuccess = SendMail(m_objMMailNotifDA.Data, ref mssg,IsBatch);
                            if (mssg.Length > 0)
                                m_lstMessage.Add(mssg);
                            if (sentSuccess)
                            {
                                m_objMMailNotifDA.Data.StatusID = (int)NotificationStatus.Sent;
                                m_objMMailNotifDA.Update(false);
                            }
                        }
                        else
                            m_lstMessage.Add(m_objMMailNotifDA.Message);
                    }
                    if (m_lstMessage.Any())
                        Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
                    else
                        Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
                }
                else
                    Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));

                if (m_objMTasksDA.Success || m_objMTasksDA.Message == string.Empty)
                    m_objMTasksDA.EndTrans(ref m_objDBConnection,true, m_strTransName);

                return Detail(General.EnumDesc(Buttons.ButtonSave), selected, m_strTaskID, m_arrRowTask.Count > 0 ? m_arrRowTask[0] : null, FromBtn);
            }
            m_objMTasksDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);
        }

        private string GetFunctionDesc(string TaskType, string TaskID)
        {
            string retDesc = "";
            if (GetListTypeOfInvitationGroup().Any(x => x == TaskType))
            {
                if (TaskType == General.EnumDesc(Enum.TaskType.MailNotification) || TaskType == General.EnumDesc(Enum.TaskType.MinutesOfMeeting))
                {
                    MMailNotificationsDA m_objMMailNotificationDA = new MMailNotificationsDA();
                    m_objMMailNotificationDA.ConnectionStringName = Global.ConnStrConfigName;

                    List<string> m_lstSelect = new List<string>();
                    m_lstSelect.Add(MailNotificationsVM.Prop.MailNotificationID.MapAlias);
                    m_lstSelect.Add(MailNotificationsVM.Prop.FunctionDesc.MapAlias);

                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Contains);
                    m_lstFilter.Add(TaskID);
                    m_objFilter.Add(MailNotificationsVM.Prop.TaskID.Map, m_lstFilter);
                    Dictionary<int, DataSet> m_dicMCountryDA = m_objMMailNotificationDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);

                    foreach (DataRow m_drMStatusDA in m_dicMCountryDA[0].Tables[0].Rows)
                        retDesc = m_drMStatusDA[MailNotificationsVM.Prop.FunctionDesc.Name].ToString();


                }
                if (TaskType == General.EnumDesc(Enum.TaskType.Invitation_Pretender) || TaskType == General.EnumDesc(Enum.TaskType.Invitation_Schedules))
                {
                    MSchedulesDA m_objMScheduleDA = new MSchedulesDA();
                    m_objMScheduleDA.ConnectionStringName = Global.ConnStrConfigName;

                    List<string> m_lstSelect = new List<string>();
                    m_lstSelect.Add(SchedulesVM.Prop.ScheduleID.MapAlias);
                    m_lstSelect.Add(SchedulesVM.Prop.FunctionDescription.MapAlias);

                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Contains);
                    m_lstFilter.Add(TaskID);
                    m_objFilter.Add(SchedulesVM.Prop.TaskID.Map, m_lstFilter);
                    Dictionary<int, DataSet> m_dicMCountryDA = m_objMScheduleDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);

                    foreach (DataRow m_drMStatusDA in m_dicMCountryDA[0].Tables[0].Rows)
                        retDesc = m_drMStatusDA[SchedulesVM.Prop.FunctionDescription.Name].ToString();

                }

            }
            return retDesc;
        }
        private string GetFunctionID(string TaskType, string TaskID)
        {
            string retDesc = "";
            if (GetListTypeOfInvitationGroup().Any(x => x == TaskType))
            {
                if (TaskType == General.EnumDesc(Enum.TaskType.MailNotification) || TaskType == General.EnumDesc(Enum.TaskType.MinutesOfMeeting))
                {
                    MMailNotificationsDA m_objMMailNotificationDA = new MMailNotificationsDA();
                    m_objMMailNotificationDA.ConnectionStringName = Global.ConnStrConfigName;

                    List<string> m_lstSelect = new List<string>();
                    m_lstSelect.Add(MailNotificationsVM.Prop.MailNotificationID.MapAlias);
                    m_lstSelect.Add(MailNotificationsVM.Prop.FunctionID.MapAlias);

                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Contains);
                    m_lstFilter.Add(TaskID);
                    m_objFilter.Add(MailNotificationsVM.Prop.TaskID.Map, m_lstFilter);
                    Dictionary<int, DataSet> m_dicMCountryDA = m_objMMailNotificationDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);

                    foreach (DataRow m_drMStatusDA in m_dicMCountryDA[0].Tables[0].Rows)
                        retDesc = m_drMStatusDA[MailNotificationsVM.Prop.FunctionID.Name].ToString();


                }
                if (TaskType == General.EnumDesc(Enum.TaskType.Invitation_Pretender) || TaskType == General.EnumDesc(Enum.TaskType.Invitation_Schedules))
                {
                    MSchedulesDA m_objMScheduleDA = new MSchedulesDA();
                    m_objMScheduleDA.ConnectionStringName = Global.ConnStrConfigName;

                    List<string> m_lstSelect = new List<string>();
                    m_lstSelect.Add(SchedulesVM.Prop.ScheduleID.MapAlias);
                    m_lstSelect.Add(SchedulesVM.Prop.FunctionID.MapAlias);

                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Contains);
                    m_lstFilter.Add(TaskID);
                    m_objFilter.Add(SchedulesVM.Prop.TaskID.Map, m_lstFilter);
                    Dictionary<int, DataSet> m_dicMCountryDA = m_objMScheduleDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);

                    foreach (DataRow m_drMStatusDA in m_dicMCountryDA[0].Tables[0].Rows)
                        retDesc = m_drMStatusDA[SchedulesVM.Prop.FunctionID.Name].ToString();

                }

            }
            return retDesc;
        }

        public Dictionary<int, List<StatusVM>> GetStatusData(bool isCount, string StatusID, string StatusDesc)
        {
            int m_intCount = 0;
            List<StatusVM> m_lstStatusVM = new List<StatusVM>();
            Dictionary<int, List<StatusVM>> m_dicReturn = new Dictionary<int, List<StatusVM>>();
            MStatusDA m_objMStatusDA = new MStatusDA();
            m_objMStatusDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(StatusVM.Prop.StatusID.MapAlias);
            m_lstSelect.Add(StatusVM.Prop.StatusDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(StatusID);
            m_objFilter.Add(StatusVM.Prop.StatusID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(StatusDesc);
            m_objFilter.Add(StatusVM.Prop.StatusDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMCountryDA = m_objMStatusDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMStatusDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpCountryBL in m_dicMCountryDA)
                    {
                        m_intCount = m_kvpCountryBL.Key;
                        break;
                    }
                else
                {
                    m_lstStatusVM = (
                        from DataRow m_drMStatusDA in m_dicMCountryDA[0].Tables[0].Rows
                        select new StatusVM()
                        {
                            StatusID = (int)m_drMStatusDA[StatusVM.Prop.StatusID.Name],
                            StatusDesc = m_drMStatusDA[StatusVM.Prop.StatusDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstStatusVM);
            return m_dicReturn;
        }

        public ActionResult BrowseStatus(string ControlStatusID, string ControlStatusDesc, string isTaskCreator, string FilterStatusID = "", string FilterStatusDesc = "")
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddStatus = new ViewDataDictionary();
            m_vddStatus.Add("Control" + StatusVM.Prop.StatusID.Name, ControlStatusID);
            m_vddStatus.Add("Control" + StatusVM.Prop.StatusDesc.Name, ControlStatusDesc);
            m_vddStatus.Add(StatusVM.Prop.StatusID.Name, FilterStatusID);
            m_vddStatus.Add(StatusVM.Prop.StatusDesc.Name, FilterStatusDesc);
            m_vddStatus.Add("isTaskCreator", isTaskCreator);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddStatus,
                ViewName = "../MyTask/_BrowseStatus"
            };
        }

        public ActionResult ReadBrowseStatus(StoreRequestParameters parameters, string isTaskCreator)
        {
            MStatusDA m_objMStatusDA = new MStatusDA();
            m_objMStatusDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMStatus = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMStatus.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = StatusVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMStatusDA = m_objMStatusDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpStatusBL in m_dicMStatusDA)
            {
                m_intCount = m_kvpStatusBL.Key;
                break;
            }

            List<StatusVM> m_lstStatusVM = new List<StatusVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(StatusVM.Prop.StatusID.MapAlias);
                m_lstSelect.Add(StatusVM.Prop.StatusDesc.MapAlias);

                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Contains);
                m_lstFilter.Add("DTaskDetails");
                m_objFilter.Add(StatusVM.Prop.TableName.Map, m_lstFilter);

                if (string.IsNullOrEmpty(isTaskCreator))
                {
                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.In);
                    m_lstFilter.Add("0,1,2,3");
                    m_objFilter.Add(StatusVM.Prop.StatusID.Map, m_lstFilter);
                }
                else
                {
                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.In);
                    m_lstFilter.Add(Convert.ToBoolean(isTaskCreator) ? "2" : "0,1,2,3");
                    m_objFilter.Add(StatusVM.Prop.StatusID.Map, m_lstFilter);
                }
                //if (CurrentApprovalLvl.Length > 0)
                //{
                //    if (int.Parse(CurrentApprovalLvl) <= 0)
                //    {
                //        m_lstFilter = new List<object>();
                //        m_lstFilter.Add(Operator.Contains);
                //        m_lstFilter.Add(2);//Only show approved choice
                //        m_objFilter.Add(StatusVM.Prop.StatusID.Map, m_lstFilter);
                //    }
                //}
                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(StatusVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                //MTasks
                m_dicMStatusDA = m_objMStatusDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMStatusDA.Message == string.Empty)
                {
                    m_lstStatusVM = (
                        from DataRow m_drMStatusDA in m_dicMStatusDA[0].Tables[0].Rows
                        select new StatusVM()
                        {
                            StatusID = (int)m_drMStatusDA[StatusVM.Prop.StatusID.Name],
                            StatusDesc = m_drMStatusDA[StatusVM.Prop.StatusDesc.Name].ToString()
                        }
                    ).Distinct().ToList();
                }
            }
            return this.Store(m_lstStatusVM, m_intCount);
        }
        #endregion

        #region Direct Method
        public ActionResult GeTTCMembers(string ControlTCMemberID, string ControlTCMemberDesc, string FilterTCMemberID, string FilterTCMemberDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<TCMembersVM>> m_dicTCMemberData = GeTTCMembersData(true, FilterTCMemberID, FilterTCMemberDesc);
                KeyValuePair<int, List<TCMembersVM>> m_kvpTCMembersVM = m_dicTCMemberData.AsEnumerable().ToList()[0];
                if (m_kvpTCMembersVM.Key < 1 || (m_kvpTCMembersVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpTCMembersVM.Key > 1 && !Exact)
                    return Browse(ControlTCMemberID, ControlTCMemberDesc, FilterTCMemberID, FilterTCMemberDesc);

                m_dicTCMemberData = GeTTCMembersData(false, FilterTCMemberID, FilterTCMemberDesc);
                TCMembersVM m_objTCMembersVM = m_dicTCMemberData[0][0];
                this.GetCmp<TextField>(ControlTCMemberID).Value = m_objTCMembersVM.TCMemberID;
                this.GetCmp<TextField>(ControlTCMemberDesc).Value = m_objTCMembersVM.EmployeeName;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        public ActionResult GetStatus(string ControlStatusID, string ControlStatusDesc, string FilterStatusID, string FilterStatusDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<StatusVM>> m_dicStatusData = GetStatusData(true, FilterStatusID, FilterStatusDesc);
                KeyValuePair<int, List<StatusVM>> m_kvpStatusVM = m_dicStatusData.AsEnumerable().ToList()[0];
                if (m_kvpStatusVM.Key < 1 || (m_kvpStatusVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpStatusVM.Key > 1 && !Exact)
                    return BrowseStatus(ControlStatusID, ControlStatusDesc, FilterStatusID, FilterStatusDesc);

                m_dicStatusData = GetStatusData(false, FilterStatusID, FilterStatusDesc);
                StatusVM m_objStatusVM = m_dicStatusData[0][0];
                this.GetCmp<TextField>(ControlStatusID).Value = m_objStatusVM.StatusID;
                this.GetCmp<TextField>(ControlStatusDesc).Value = m_objStatusVM.StatusDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }
        #endregion

        #region Private Method
        private MailNotificationsVM GetMailNotificationsVM(string MailNotificationID)
        {

            string msg = string.Empty;

            MailNotificationsVM m_objMailNotificationsVM = new MailNotificationsVM();
            MMailNotificationsDA m_objMMailNotificationsDA = new MMailNotificationsDA();
            m_objMMailNotificationsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();

            m_lstSelect.Add(MailNotificationsVM.Prop.MailNotificationID.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.FunctionID.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.FunctionDesc.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.Importance.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.Subject.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.Contents.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.StatusID.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.TaskID.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.FPTID.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.FPTDesc.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.CreatedDate.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.TaskStatusDesc.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.StatusDesc.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.NotificationTemplateID.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.NotificationTemplateDesc.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.TaskStatusID.MapAlias);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(MailNotificationID);
            m_objFilter.Add(MailNotificationsVM.Prop.MailNotificationID.Map, m_lstFilter);
            Dictionary<int, DataSet> m_dicMMailNotificationsDA = m_objMMailNotificationsDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMMailNotificationsDA.Message == string.Empty)
            {

                DataRow m_drMMailNotificationsDA = m_dicMMailNotificationsDA[0].Tables[0].Rows[0];
                m_objMailNotificationsVM.MailNotificationID = m_drMMailNotificationsDA[MailNotificationsVM.Prop.MailNotificationID.Name].ToString();
                m_objMailNotificationsVM.FunctionID = m_drMMailNotificationsDA[MailNotificationsVM.Prop.FunctionID.Name].ToString();
                m_objMailNotificationsVM.FunctionDesc = m_drMMailNotificationsDA[MailNotificationsVM.Prop.FunctionDesc.Name].ToString();
                m_objMailNotificationsVM.Importance = (bool)m_drMMailNotificationsDA[MailNotificationsVM.Prop.Importance.Name];
                m_objMailNotificationsVM.Subject = m_drMMailNotificationsDA[MailNotificationsVM.Prop.Subject.Name].ToString();
                m_objMailNotificationsVM.Contents = m_drMMailNotificationsDA[MailNotificationsVM.Prop.Contents.Name].ToString();
                m_objMailNotificationsVM.StatusID = (int)m_drMMailNotificationsDA[MailNotificationsVM.Prop.StatusID.Name];
                m_objMailNotificationsVM.TaskID = m_drMMailNotificationsDA[MailNotificationsVM.Prop.TaskID.Name].ToString();
                m_objMailNotificationsVM.FPTID = m_drMMailNotificationsDA[MailNotificationsVM.Prop.FPTID.Name].ToString();
                m_objMailNotificationsVM.FPTDesc = m_drMMailNotificationsDA[MailNotificationsVM.Prop.FPTDesc.Name].ToString();
                m_objMailNotificationsVM.CreatedDate = (DateTime)m_drMMailNotificationsDA[MailNotificationsVM.Prop.CreatedDate.Name];
                m_objMailNotificationsVM.TaskStatusDesc = m_drMMailNotificationsDA[MailNotificationsVM.Prop.TaskStatusDesc.Name].ToString();
                m_objMailNotificationsVM.StatusDesc = m_drMMailNotificationsDA[MailNotificationsVM.Prop.StatusDesc.Name].ToString();
                m_objMailNotificationsVM.NotificationTemplateID = m_drMMailNotificationsDA[MailNotificationsVM.Prop.NotificationTemplateID.Name].ToString();
                m_objMailNotificationsVM.NotificationTemplateDesc = m_drMMailNotificationsDA[MailNotificationsVM.Prop.NotificationTemplateDesc.Name].ToString();
                m_objMailNotificationsVM.TaskStatusID = string.IsNullOrEmpty(m_drMMailNotificationsDA[MailNotificationsVM.Prop.TaskStatusID.Name].ToString()) ? null : (int?)m_drMMailNotificationsDA[MailNotificationsVM.Prop.TaskStatusID.Name];

                m_objMailNotificationsVM.RecipientsVM = GetListRecipientsVM(m_objMailNotificationsVM.MailNotificationID, ref msg);
                //m_objMailNotificationsVM.NotificationValuesVM = GetListNotificationValuesVM(m_objMailNotificationsVM.MailNotificationID, ref msg);
                //m_objMailNotificationsVM.TemplateTagsVM = GetListTemplateTagsVM(m_objMailNotificationsVM.NotificationTemplateID, ref msg);
                m_objMailNotificationsVM.NotificationMapVM = GetDefaultNoticationMap(m_objMailNotificationsVM.FunctionID);//todo: catch error
                ////foreach (var item in m_objMailNotificationsVM.TemplateTagsVM)
                ////{

                ////    item.Value = !m_objMailNotificationsVM.NotificationValuesVM.Where(x => x.FieldTagID == item.FieldTagID).Any() ? null : m_objMailNotificationsVM.NotificationValuesVM.Where(x => x.FieldTagID == item.FieldTagID).FirstOrDefault().Value;
                ////}
                m_objMailNotificationsVM.NotificationAttachmentVM = GetListNotificationAttachmentVM(m_objMailNotificationsVM.MailNotificationID, ref msg);

            }

            return m_objMailNotificationsVM;
        }
        public ActionResult LoadTaskHistory(StoreRequestParameters parameters, string TaskID_)
        {
            DTaskDetailsDA m_objDTaskDetailsDA = new DTaskDetailsDA();
            m_objDTaskDetailsDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcDTaskDetails = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcDTaskDetails.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = MyTaskVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicDTaskDetailDA = m_objDTaskDetailsDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpDTaskDetailsBL in m_dicDTaskDetailDA)
            {
                m_intCount = m_kvpDTaskDetailsBL.Key;
                break;
            }
            List<object> m_lstFilters = new List<object>();

            List<TaskDetailsVM> m_lsTaskDetailsVM = new List<TaskDetailsVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(TaskDetailsVM.Prop.TaskID.MapAlias);
                m_lstSelect.Add(TaskDetailsVM.Prop.TaskDetailID.MapAlias);
                m_lstSelect.Add(TaskDetailsVM.Prop.StatusID.MapAlias);
                m_lstSelect.Add(TaskDetailsVM.Prop.StatusDesc.MapAlias);
                m_lstSelect.Add(TaskDetailsVM.Prop.Remarks.MapAlias);
                m_lstSelect.Add(TaskDetailsVM.Prop.CreatorFullName.MapAlias);
                m_lstSelect.Add(TaskDetailsVM.Prop.CreatedDate.MapAlias);

                m_lstFilters = new List<object>();
                m_lstFilters.Add(Operator.Equals);
                m_lstFilters.Add(TaskID_);
                m_objFilter.Add(TaskDetailsVM.Prop.TaskID.Map, m_lstFilters);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                //foreach (DataSorter m_dtsOrder in parameters.Sort)
                m_dicOrder.Add(TaskDetailsVM.Prop.CreatedDate.Map, OrderDirection.Descending);

                m_dicDTaskDetailDA = m_objDTaskDetailsDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objDTaskDetailsDA.Message == string.Empty)
                {
                    m_lsTaskDetailsVM = (
                        from DataRow m_drDTaskDetailsDA in m_dicDTaskDetailDA[0].Tables[0].Rows
                        select new TaskDetailsVM()
                        {
                            TaskID = m_drDTaskDetailsDA[TaskDetailsVM.Prop.TaskID.Name].ToString(),
                            TaskDetailID = m_drDTaskDetailsDA[TaskDetailsVM.Prop.TaskDetailID.Name].ToString(),
                            StatusID = int.Parse(m_drDTaskDetailsDA[TaskDetailsVM.Prop.StatusID.Name].ToString()),
                            StatusDesc = m_drDTaskDetailsDA[TaskDetailsVM.Prop.StatusDesc.Name].ToString(),
                            CreatorFullName = m_drDTaskDetailsDA[TaskDetailsVM.Prop.CreatorFullName.Name].ToString(),
                            Remarks = m_drDTaskDetailsDA[TaskDetailsVM.Prop.Remarks.Name].ToString(),
                            CreatedDate = DateTime.Parse(m_drDTaskDetailsDA[TaskDetailsVM.Prop.CreatedDate.Name].ToString())
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lsTaskDetailsVM, m_intCount);
        }
        private List<NegotiationConfigurationsVM> GetNegoConfig(string TaskID)
        {
            List<NegotiationConfigurationsVM> m_lstNegotiationConfigurationsVM = new List<NegotiationConfigurationsVM>();
            CNegotiationConfigurationsDA m_objCNegotiationConfigurationsDA = new CNegotiationConfigurationsDA();
            m_objCNegotiationConfigurationsDA.ConnectionStringName = Global.ConnStrConfigName;
            string message = "";
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.NegotiationConfigID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.NegotiationConfigTypeID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.ParameterValue.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.Descriptions.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.TaskID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.StatusID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.FPTID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.TCMemberID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.EmployeeID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.EmployeeName.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.TCLeadName.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.VendorName.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.BudgetPlanDescription.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(TaskID);
            m_objFilter.Add(NegotiationConfigurationsVM.Prop.TaskID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.NotEqual);
            m_lstFilter.Add("3");
            m_objFilter.Add(NegotiationConfigurationsVM.Prop.StatusID.Map, m_lstFilter);

            Dictionary<string, OrderDirection> m_dicOrderBy = new Dictionary<string, OrderDirection>();
            m_dicOrderBy.Add(NegotiationConfigurationsVM.Prop.NegotiationConfigID.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicNegotiationConfigurationsDA = m_objCNegotiationConfigurationsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, m_dicOrderBy);
            if (m_objCNegotiationConfigurationsDA.Success)
            {
                //Dictionary<string, List<string>> ConfigType = new Dictionary<string, List<string>>();
                //foreach (DataRow m_drNegotiationConfigurationsDA in m_dicNegotiationConfigurationsDA[0].Tables[0].Rows)
                //{
                //    m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.NegotiationConfigID.Name].ToString()
                //}
                //ConfigType.Add(General.EnumDesc(NegoConfigTypes.Round), m_strNegoRound);


                m_lstNegotiationConfigurationsVM =
                (from DataRow m_drNegotiationConfigurationsDA in m_dicNegotiationConfigurationsDA[0].Tables[0].Rows
                 select new NegotiationConfigurationsVM()
                 {
                     NegotiationConfigID = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.NegotiationConfigID.Name].ToString(),
                     NegotiationConfigTypeID = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.NegotiationConfigTypeID.Name].ToString(),
                     Descriptions = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.Descriptions.Name].ToString(),
                     FPTID = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.FPTID.Name].ToString(),
                     TaskID = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.TaskID.Name].ToString(),
                     StatusID = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.StatusID.Name].ToString(),
                     ParameterValue = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.ParameterValue.Name].ToString(),
                     TCMemberID = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.TCMemberID.Name].ToString(),
                     EmployeeID = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.EmployeeID.Name].ToString(),
                     TCLeadName = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.TCLeadName.Name].ToString(),
                     //TRMLeadDesc = string.IsNullOrEmpty(m_drNegotiationConfigurationsDA["TRMLeadDesc"].ToString()) ? "" : m_drNegotiationConfigurationsDA["TRMLeadDesc"].ToString(),
                     VendorName = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.VendorName.Name].ToString(),
                     BudgetPlanDescription = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.BudgetPlanDescription.Name].ToString(),
                     EmployeeName = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.EmployeeName.Name].ToString()
                 }).ToList();
            }
            else
                message = m_objCNegotiationConfigurationsDA.Message;

            return m_lstNegotiationConfigurationsVM;
        }
        private List<FPTVendorWinnerVM> getFPTVendorWinnerVM(string TaskID)
        {
            List<FPTVendorWinnerVM> m_lstFPTVendorWinnerVM = new List<FPTVendorWinnerVM>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();
            DFPTVendorWinnerDA m_objDFPTVendorWinnerDA = new DFPTVendorWinnerDA();
            m_objDFPTVendorWinnerDA.ConnectionStringName = Global.ConnStrConfigName;

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(TaskID);
            m_objFilter.Add(FPTVendorWinnerVM.Prop.TaskID.Map, m_lstFilter);

            m_lstSelect = new List<string>();
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.VendorWinnerID.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.FPTID.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.TaskID.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.VendorName.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.VendorAddress.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.VendorEmail.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.FPTVendorParticipantID.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.IsWinner.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.IsSync.MapAlias);

            Dictionary<int, DataSet> m_dicDFPTVendorWinnerDA = m_objDFPTVendorWinnerDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objDFPTVendorWinnerDA.Success)
            {
                foreach (DataRow item in m_dicDFPTVendorWinnerDA[0].Tables[0].Rows)
                {
                    FPTVendorWinnerVM m_objFPTVendorWinnerVM = new FPTVendorWinnerVM();
                    m_objFPTVendorWinnerVM.VendorWinnerID = item[FPTVendorWinnerVM.Prop.VendorWinnerID.Name].ToString();
                    m_objFPTVendorWinnerVM.FPTID = item[FPTVendorWinnerVM.Prop.FPTID.Name].ToString();
                    m_objFPTVendorWinnerVM.TaskID = item[FPTVendorWinnerVM.Prop.TaskID.Name].ToString();
                    m_objFPTVendorWinnerVM.VendorName = item[FPTVendorWinnerVM.Prop.VendorName.Name].ToString();
                    m_objFPTVendorWinnerVM.VendorAddress = item[FPTVendorWinnerVM.Prop.VendorAddress.Name].ToString();
                    m_objFPTVendorWinnerVM.VendorEmail = item[FPTVendorWinnerVM.Prop.VendorEmail.Name].ToString();
                    m_objFPTVendorWinnerVM.FPTVendorParticipantID = item[FPTVendorWinnerVM.Prop.FPTVendorParticipantID.Name].ToString();
                    m_objFPTVendorWinnerVM.IsWinner = (bool)item[FPTVendorWinnerVM.Prop.IsWinner.Name];
                    m_objFPTVendorWinnerVM.IsSync = (bool)item[FPTVendorWinnerVM.Prop.IsSync.Name];
                    m_lstFPTVendorWinnerVM.Add(m_objFPTVendorWinnerVM);
                }
            }
            return m_lstFPTVendorWinnerVM;
        }
        private string GetSummary(string TaskID, string TaskTypeID, DataRow dr)
        {

            List<NegotiationConfigurationsVM> NegoConfVM = new List<NegotiationConfigurationsVM>();

            string Ret = "";

            #region Summary of e-Negotiation
            if (General.EnumDesc(TaskType.NegotiationConfigurations) == TaskTypeID)
            {
                NegoConfVM = GetNegoConfig(TaskID);
                string c = General.EnumName(NegoConfigTypes.Schedule);
                Ret = "";
                bool isEmployee = false;
                int tcmembercount = 0;
                int vendorparticipantcount = 0;
                bool first = true;
                foreach (NegotiationConfigurationsVM NVM in NegoConfVM)
                {
                    if (NVM.Descriptions.ToLower().Contains("tc member") || NVM.Descriptions.ToLower().Contains("trm lead"))
                    {
                        NVM.ParameterValue = NVM.EmployeeName;
                        if (NVM.Descriptions.ToLower().Contains("tc member"))
                        {
                            if (tcmembercount > 0)
                                Ret += "- " + NVM.ParameterValue + "\n";
                            else
                                Ret += "\n" + NVM.Descriptions + " : \n" + "- " + NVM.ParameterValue + "\n";
                            tcmembercount++;
                        }
                        else
                            Ret += "\n" + NVM.Descriptions + " : \n" + "- " + NVM.TCLeadName + "\n";


                        //isEmployee = true;
                    }
                    else if (NVM.Descriptions.ToLower().Contains("vendor"))
                    {
                        if (vendorparticipantcount > 0)
                            Ret += "- " + NVM.VendorName + "\n";
                        else
                            Ret += "\n" + "Vendor Participant : \n" + "- " + NVM.VendorName + "\n";
                        vendorparticipantcount++;
                    }
                    else if (NVM.Descriptions.ToLower().Contains("project"))
                    {
                        string _GrandTotalRAB = "";
                        List<FPTVendorParticipantsVM> lstFPTVendorParticipants = GetFPTParticipant(NVM.NegotiationConfigID);

                        if (first)
                        {
                            _GrandTotalRAB = GrandTotalRAB(NVM.NegotiationConfigID);
                            if (!string.IsNullOrEmpty(NVM.ParameterValue))
                                Ret += "\n Project : \n" + "- " + NVM.BudgetPlanDescription + " (BPID: " + NVM.ParameterValue + " | RAB Amount: " + _GrandTotalRAB + ") \n";
                            else
                                Ret += "\n Project : \n" + "- BudgetPlan from Upload (RAB Amount: " + _GrandTotalRAB + ") \n";

                            first = false;
                        }
                        else
                        {
                            _GrandTotalRAB = GrandTotalRAB(NVM.NegotiationConfigID);
                            Ret += "- " + NVM.BudgetPlanDescription + " (BPID: " + NVM.ParameterValue + " | RAB Amount: " + _GrandTotalRAB + ") \n";
                        }

                        Ret += "     Participant:\n";

                        foreach (FPTVendorParticipantsVM vd in lstFPTVendorParticipants)
                            Ret += "     • " + vd.VendorName + " (VendorID: " + vd.VendorID + ") \n";

                        Ret += "\n";

                    }
                    else
                    {
                        //isEmployee = false;
                        Ret += NVM.Descriptions + " : ";
                        Ret += NVM.ParameterValue + "\n";
                    }
                }
            }

            else if (General.EnumDesc(TaskType.VendorWinner) == TaskTypeID)
            {
                string summ = dr[MyTaskVM.Prop.Summary.Name].ToString();
                List<string> ls = summ.Split('|').ToList();
                foreach (string fill in ls)
                    Ret += fill + "\n";
            }
            else
                Ret = dr[MyTaskVM.Prop.Summary.Name].ToString();

            return Ret;
            #endregion
        }
        private List<FPTVendorParticipantsVM> GetFPTParticipant(string NegoConfigID)
        {
            //List<string> m_lstBPlan = new List<string>();
            //foreach (NegotiationConfigurationsVM nConf in m_lstNegotiationConfigurationsVM.Where(x => x.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.BudgetPlan)))
            //    m_lstBPlan.Add(nConf.NegotiationConfigID);
            List<FPTVendorParticipantsVM> retListFPTParticipants = new List<FPTVendorParticipantsVM>();

            DFPTVendorParticipantsDA m_objDFPTVendorParticipantsDA = new DFPTVendorParticipantsDA();
            DFPTVendorParticipants m_objDFPTVendorParticipants = new DFPTVendorParticipants();
            m_objDFPTVendorParticipantsDA.ConnectionStringName = Global.ConnStrConfigName;

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.NegotiationConfigID.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.VendorName.MapAlias);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(NegoConfigID);
            m_objFilter.Add(FPTVendorParticipantsVM.Prop.NegotiationConfigID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicFPTParticipantsDA = m_objDFPTVendorParticipantsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDFPTVendorParticipantsDA.Success)
            {
                retListFPTParticipants =
                (from DataRow m_drFPTPartDA in m_dicFPTParticipantsDA[0].Tables[0].Rows
                 select new FPTVendorParticipantsVM()
                 {
                     NegotiationConfigID = m_drFPTPartDA[FPTVendorParticipantsVM.Prop.NegotiationConfigID.Name].ToString(),
                     VendorID = m_drFPTPartDA[FPTVendorParticipantsVM.Prop.VendorID.Name].ToString(),
                     VendorName = m_drFPTPartDA[FPTVendorParticipantsVM.Prop.VendorName.Name].ToString()

                 }).ToList();
            }

            return retListFPTParticipants;
        }
        private string GrandTotalRAB(string NegoConfigID)
        {
            TNegotiationBidStructuresDA m_objTNegoBidStructDA = new TNegotiationBidStructuresDA();
            TNegotiationBidStructures m_objNegoConfig = new TNegotiationBidStructures();
            m_objTNegoBidStructDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.BudgetPlanDefaultValue.MapAlias);
            Dictionary<string, List<object>> m_objFilter_ = new Dictionary<string, List<object>>();
            List<object> m_lstFilter_ = new List<object>();
            m_lstFilter_.Add(Operator.Equals);
            m_lstFilter_.Add(NegoConfigID);
            m_objFilter_.Add(NegotiationBidStructuresVM.Prop.NegotiationConfigID.Map, m_lstFilter_);
            //m_lstFilter_ = new List<object>();
            //m_lstFilter_.Add(Operator.None);
            //m_lstFilter_.Add("");
            //m_objFilter_.Add(NegotiationBidStructuresVM.Prop.ItemID.Map + " IS NULL", m_lstFilter_);
            m_lstFilter_ = new List<object>();
            m_lstFilter_.Add(Operator.Equals);
            m_lstFilter_.Add("9999");
            m_objFilter_.Add(NegotiationBidStructuresVM.Prop.Sequence.Map, m_lstFilter_);
            Dictionary<int, DataSet> m_dicNegotiationBidStructureDA = m_objTNegoBidStructDA.SelectBC(0, null, false, m_lstSelect, m_objFilter_, null, null, null, null);
            if (m_objTNegoBidStructDA.Success && m_objTNegoBidStructDA.AffectedRows > 0)
            {
                string GTotal = m_dicNegotiationBidStructureDA[0].Tables[0].Rows[0][NegotiationBidStructuresVM.Prop.BudgetPlanDefaultValue.Name].ToString();
                decimal GT = decimal.Parse(string.IsNullOrEmpty(GTotal) ? "0" : GTotal);
                return string.Format("{0:C}", GT).Remove(0, 1);
            }
            else
                return "";

        }
        private List<string> IsSaveValid(string Action, string CountryID, string CountryDesc)
        {
            List<string> m_lstReturn = new List<string>();

            if (CountryID == string.Empty)
                m_lstReturn.Add(CountryVM.Prop.CountryID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (CountryDesc == string.Empty)
                m_lstReturn.Add(CountryVM.Prop.CountryDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));

            return m_lstReturn;
        }
        private List<string> GetListTypeOfInvitationGroup()
        {
            List<string> ListInvitationTaskType = new List<string>();
            ListInvitationTaskType.Add(General.EnumDesc(TaskType.Invitation_Schedules));
            ListInvitationTaskType.Add(General.EnumDesc(TaskType.Invitation_Pretender));
            ListInvitationTaskType.Add(General.EnumDesc(TaskType.MinutesOfMeeting));
            ListInvitationTaskType.Add(General.EnumDesc(TaskType.MailNotification));
            return ListInvitationTaskType;
        }
        private List<int> GetListIntegerTypeOfInvitationGroup()
        {
            List<int> ListInvitationTaskType = new List<int>();
            ListInvitationTaskType.Add((int)TaskType.Invitation_Schedules);
            ListInvitationTaskType.Add((int)TaskType.Invitation_Pretender);
            ListInvitationTaskType.Add((int)TaskType.MinutesOfMeeting);
            ListInvitationTaskType.Add((int)TaskType.MailNotification);
            return ListInvitationTaskType;
        }
        private bool isTypeOfInvitationGroup(string TaskTypeID)
        {
            foreach (string Type_ in GetListTypeOfInvitationGroup().Where(x => x == TaskTypeID))
                return true;
            return false;
        }
        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(MyTaskVM.Prop.TaskID.Name, parameters[MyTaskVM.Prop.TaskID.Name]);
            m_dicReturn.Add(MyTaskVM.Prop.TaskOwnerID.Name, parameters[MyTaskVM.Prop.TaskOwnerID.Name]);
            m_dicReturn.Add(MyTaskVM.Prop.TaskOwnerDesc.Name, parameters[MyTaskVM.Prop.TaskOwnerID.Name]);
            m_dicReturn.Add(MyTaskVM.Prop.TaskTypeID.Name, parameters[MyTaskVM.Prop.TaskTypeID.Name]);
            m_dicReturn.Add(MyTaskVM.Prop.TaskDescription.Name, parameters[MyTaskVM.Prop.TaskDescription.Name]);
            m_dicReturn.Add(MyTaskVM.Prop.TaskDateTimeStamp.Name, parameters[MyTaskVM.Prop.TaskDateTimeStamp.Name]);
            m_dicReturn.Add(MyTaskVM.Prop.StatusID.Name, parameters[MyTaskVM.Prop.StatusDesc.Name]);
            m_dicReturn.Add(MyTaskVM.Prop.StatusDesc.Name, parameters[MyTaskVM.Prop.StatusDesc.Name]);
            m_dicReturn.Add(MyTaskVM.Prop.CurrentApprovalLvl.Name, parameters[MyTaskVM.Prop.CurrentApprovalLvl.Name]);
            m_dicReturn.Add(MyTaskVM.Prop.ConfigApprovalInterval.Name, parameters[MyTaskVM.Prop.ConfigApprovalInterval.Name]);
            m_dicReturn.Add(MyTaskVM.Prop.Remarks.Name, parameters[MyTaskVM.Prop.Remarks.Name]);
            m_dicReturn.Add(MyTaskVM.Prop.CreatedBy.Name, parameters[MyTaskVM.Prop.CreatedBy.Name]);
            m_dicReturn.Add(MyTaskVM.Prop.Subject.Name, parameters[MyTaskVM.Prop.Subject.Name]);
            //m_dicReturn.Add(MyTaskVM.Prop.MailNotificationID.Name, parameters[MyTaskVM.Prop.MailNotificationID.Name]);
            m_dicReturn.Add(MyTaskVM.Prop.NotificationTemplateID.Name, parameters[MyTaskVM.Prop.NotificationTemplateID.Name]);
            //m_dicReturn.Add(MyTaskVM.Prop.CreatedRoleID.Name, parameters[MyTaskVM.Prop.CreatedRoleID.Name]);

            return m_dicReturn;
        }
        private MyTaskVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            MyTaskVM m_objMTasksVM = new MyTaskVM();
            MTasksDA m_objMTaskDA = new MTasksDA();
            m_objMTaskDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(MyTaskVM.Prop.TaskID.MapAlias);
            m_lstSelect.Add(MyTaskVM.Prop.TaskOwnerID.MapAlias);
            m_lstSelect.Add(MyTaskVM.Prop.TaskOwnerDesc.MapAlias);
            m_lstSelect.Add(MyTaskVM.Prop.TaskTypeID.MapAlias);
            m_lstSelect.Add(MyTaskVM.Prop.TaskDescription.MapAlias);
            m_lstSelect.Add(MyTaskVM.Prop.TaskDateTimeStamp.MapAlias);
            m_lstSelect.Add(MyTaskVM.Prop.StatusID.MapAlias);
            m_lstSelect.Add(MyTaskVM.Prop.StatusDesc.MapAlias);
            m_lstSelect.Add(MyTaskVM.Prop.CurrentApprovalLvl.MapAlias);
            m_lstSelect.Add(MyTaskVM.Prop.Remarks.MapAlias);
            m_lstSelect.Add(MyTaskVM.Prop.Summary.MapAlias);
            //m_lstSelect.Add(MyTaskVM.Prop.RoleID.MapAlias);
            m_lstSelect.Add(MyTaskVM.Prop.CreatedBy.MapAlias);
            //m_lstSelect.Add(MyTaskVM.Prop.CreatedRoleID.MapAlias);
            m_lstSelect.Add(MyTaskVM.Prop.CreatorFullName.MapAlias);
            //m_lstSelect.Add(MyTaskVM.Prop.MailNotificationID.MapAlias);
            m_lstSelect.Add(MyTaskVM.Prop.Subject.MapAlias);
            // m_lstSelect.Add(MyTaskVM.Prop.Contents.MapAlias);
            m_lstSelect.Add(MyTaskVM.Prop.ScheduleID.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objMTasksVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(MyTaskVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMTasksDA = m_objMTaskDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMTaskDA.Message == string.Empty)
            {
                DataRow m_drMtasksDA = m_dicMTasksDA[0].Tables[0].Rows[0];
                m_objMTasksVM.TaskID = m_drMtasksDA[MyTaskVM.Prop.TaskID.Name].ToString();
                m_objMTasksVM.TaskOwnerID = m_drMtasksDA[MyTaskVM.Prop.TaskOwnerID.Name].ToString();
                m_objMTasksVM.TaskOwnerDesc = m_drMtasksDA[MyTaskVM.Prop.TaskOwnerDesc.Name].ToString();
                m_objMTasksVM.TaskTypeID = m_drMtasksDA[MyTaskVM.Prop.TaskTypeID.Name].ToString();
                m_objMTasksVM.TaskDesciption = m_drMtasksDA[MyTaskVM.Prop.TaskDescription.Name].ToString();
                m_objMTasksVM.TaskDateTimeStamp = DateTime.Parse(m_drMtasksDA[MyTaskVM.Prop.TaskDateTimeStamp.Name].ToString());
                m_objMTasksVM.StatusID = int.Parse(m_drMtasksDA[MyTaskVM.Prop.StatusID.Name].ToString());
                m_objMTasksVM.StatusDesc = m_drMtasksDA[MyTaskVM.Prop.StatusDesc.Name].ToString();
                m_objMTasksVM.CurrentApprovalLvl = int.Parse(m_drMtasksDA[MyTaskVM.Prop.CurrentApprovalLvl.Name].ToString());
                m_objMTasksVM.Remarks = m_drMtasksDA[MyTaskVM.Prop.Remarks.Name].ToString();
                //m_objMTasksVM.Contents = m_drMtasksDA[MyTaskVM.Prop.Contents.Name] == null?"" : m_drMtasksDA[MyTaskVM.Prop.Contents.Name].ToString();
                //m_objMTasksVM.MailNotificationID = m_drMtasksDA[MyTaskVM.Prop.MailNotificationID.Name] == null ? "" : m_drMtasksDA[MyTaskVM.Prop.MailNotificationID.Name].ToString();
                m_objMTasksVM.Subject = m_drMtasksDA[MyTaskVM.Prop.Subject.Name] == null ? "" : m_drMtasksDA[MyTaskVM.Prop.Subject.Name].ToString();
                m_objMTasksVM.ScheduleID = m_drMtasksDA[MyTaskVM.Prop.ScheduleID.Name] == null ? "" : m_drMtasksDA[MyTaskVM.Prop.ScheduleID.Name].ToString();
                //m_objMTasksVM.RoleParentID = GetParentApproval(ref message, m_objMTasksVM.TaskTypeID);
                //m_objMTasksVM.RoleChildID = GetChildApproval(ref message, m_objMTasksVM.TaskTypeID);
                //m_objMTasksVM.RoleID = m_drMtasksDA[MyTaskVM.Prop.RoleID.Name].ToString();
                m_objMTasksVM.CreatedBy = m_drMtasksDA[MyTaskVM.Prop.CreatedBy.Name].ToString();
                m_objMTasksVM.CreatorFullName = m_drMtasksDA[MyTaskVM.Prop.CreatorFullName.Name].ToString();
                //m_objMTasksVM.CreatedRoleID = m_drMtasksDA[MyTaskVM.Prop.CreatedRoleID.Name].ToString();
                //if (m_objMTasksVM.RoleID != m_objMTasksVM.TaskOwnerID)
                //{
                m_objMTasksVM.ApprovalStatusDesc = m_objMTasksVM.StatusDesc;
                m_objMTasksVM.ApprovalRemarks = m_objMTasksVM.Remarks;
                //}
                m_objMTasksVM.Summary = GetSummary(m_objMTasksVM.TaskID, m_objMTasksVM.TaskTypeID, m_drMtasksDA);

                UConfigDA m_objUConfigDA = new UConfigDA();
                m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
                List<object> m_lstFilter = new List<object>();
                Dictionary<string, List<object>> m_objFilteru = new Dictionary<string, List<object>>();

                List<string> m_lstSelectb = new List<string>();
                m_lstSelectb.Add(ConfigVM.Prop.Key3.MapAlias);

                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add("TaskType");
                m_objFilteru.Add(ConfigVM.Prop.Key1.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(m_objMTasksVM.TaskTypeID);
                m_objFilteru.Add(ConfigVM.Prop.Key2.Map, m_lstFilter);

                Dictionary<int, DataSet> m_dicUConfigDA = m_objUConfigDA.SelectBC(0, 1, false, m_lstSelectb, m_objFilteru, null, null, null);
                if (m_objUConfigDA.Message == string.Empty)
                    m_objMTasksVM.ConfigApprovalInterval = int.Parse(m_dicUConfigDA[0].Tables[0].Rows[0][ConfigVM.Prop.Key3.Name].ToString());

            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMTaskDA.Message;

            return m_objMTasksVM;
        }
        private List<NegotiationConfigurationsVM> GetlstNegotiationConfigurationsVM(string TaskID, ref List<string> message, string NegotiationConfigTypeID = "")
        {
            List<NegotiationConfigurationsVM> m_lstNegotiationConfigurationsVM = new List<NegotiationConfigurationsVM>();

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();
            CNegotiationConfigurationsDA m_objCNegotiationConfigurationsDA = new CNegotiationConfigurationsDA();
            m_objCNegotiationConfigurationsDA.ConnectionStringName = Global.ConnStrConfigName;

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(TaskID);
            m_objFilter.Add(NegotiationConfigurationsVM.Prop.TaskID.Map, m_lstFilter);

            if (NegotiationConfigTypeID != string.Empty)
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(NegotiationConfigTypeID);
                m_objFilter.Add(NegotiationConfigurationsVM.Prop.NegotiationConfigTypeID.Map, m_lstFilter);
            }

            m_lstSelect = new List<string>();
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.NegotiationConfigID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.NegotiationConfigTypeID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.FPTID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.TaskID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.ParameterValue.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.ParameterValue2.MapAlias);
            Dictionary<int, DataSet> m_dicCNegotiationConfigurationsDA = m_objCNegotiationConfigurationsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objCNegotiationConfigurationsDA.Success)
            {
                foreach (DataRow m_drCNegotiationConfigurationsDA in m_dicCNegotiationConfigurationsDA[0].Tables[0].Rows)
                {
                    NegotiationConfigurationsVM m_objNegotiationConfigurationsVM = new NegotiationConfigurationsVM();
                    m_objNegotiationConfigurationsVM.NegotiationConfigID = m_drCNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.NegotiationConfigID.Name].ToString();
                    m_objNegotiationConfigurationsVM.NegotiationConfigTypeID = m_drCNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.NegotiationConfigTypeID.Name].ToString();
                    m_objNegotiationConfigurationsVM.FPTID = m_drCNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.FPTID.Name].ToString();
                    m_objNegotiationConfigurationsVM.TaskID = m_drCNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.TaskID.Name].ToString();
                    m_objNegotiationConfigurationsVM.ParameterValue = m_drCNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.ParameterValue.Name].ToString();
                    m_objNegotiationConfigurationsVM.ParameterValue2 = m_drCNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.ParameterValue2.Name].ToString();
                    m_lstNegotiationConfigurationsVM.Add(m_objNegotiationConfigurationsVM);
                }
            }
            return m_lstNegotiationConfigurationsVM;

        }
        private List<FPTVendorParticipantsVM> GetFPTVendorParticipantsVM(string FPTID, ref List<string> message)
        {
            List<FPTVendorParticipantsVM> m_lstFPTVendorParticipantsVM = new List<FPTVendorParticipantsVM>();
            DFPTVendorParticipantsDA m_objDFPTVendorParticipantsDA = new DFPTVendorParticipantsDA();
            m_objDFPTVendorParticipantsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();

            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.FPTID.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.ParameterValue.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.VendorEmail.MapAlias);

            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.FPTVendorParticipantID.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.ProjectDesc.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.FirstName.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.ProjectID.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.VendorName.MapAlias);


            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(FPTID);
            m_objFilter.Add(FPTVendorParticipantsVM.Prop.FPTID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicm_objCNegotiationConfigurationsDA = m_objDFPTVendorParticipantsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDFPTVendorParticipantsDA.Success)
            {
                foreach (DataRow m_drCNegotiationConfigurationsDA in m_dicm_objCNegotiationConfigurationsDA[0].Tables[0].Rows)
                {
                    FPTVendorParticipantsVM m_objFPTVendorParticipantsVM = new FPTVendorParticipantsVM();
                    m_objFPTVendorParticipantsVM.FPTID = m_drCNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.FPTID.Name].ToString();
                    m_objFPTVendorParticipantsVM.ParameterValue = m_drCNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.ParameterValue.Name].ToString();
                    m_objFPTVendorParticipantsVM.VendorEmail = m_drCNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.VendorEmail.Name].ToString();
                    m_objFPTVendorParticipantsVM.FPTVendorParticipantID = m_drCNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.FPTVendorParticipantID.Name].ToString();
                    m_objFPTVendorParticipantsVM.ProjectDesc = m_drCNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.ProjectDesc.Name].ToString();
                    m_objFPTVendorParticipantsVM.BudgetPlanID = m_drCNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.BudgetPlanID.Name].ToString();
                    m_objFPTVendorParticipantsVM.FirstName = m_drCNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.FirstName.Name].ToString();
                    m_objFPTVendorParticipantsVM.ProjectID = m_drCNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.ProjectID.Name].ToString();
                    m_objFPTVendorParticipantsVM.VendorID = m_drCNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.VendorID.Name].ToString();
                    m_objFPTVendorParticipantsVM.VendorName = m_drCNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.VendorName.Name].ToString();
                    m_objFPTVendorParticipantsVM.BidValue = 0;

                    m_lstFPTVendorParticipantsVM.Add(m_objFPTVendorParticipantsVM);
                }
            }
            else
                message.Add(m_objDFPTVendorParticipantsDA.Message);
            return m_lstFPTVendorParticipantsVM;
        }
        private List<TCMembersVM> GetTCMembersVM(List<string> LstTCMemberID, ref List<string> message)
        {
            List<TCMembersVM> m_lstTCMembersVM = new List<TCMembersVM>();
            TTCMembersDA m_objTTCMembersDA = new TTCMembersDA();
            m_objTTCMembersDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();

            //m_RecipientsVM.RecipientDesc = item.EmployeeName;
            //m_RecipientsVM.MailAddress = item.Email;
            //m_RecipientsVM.OwnerID = item.EmployeeID;//todo: change to user id

            m_lstSelect.Add(TCMembersVM.Prop.TCMemberID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.EmployeeID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.SuperiorID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.TCTypeID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.BusinessUnitID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.PeriodStart.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.PeriodEnd.MapAlias);

            m_lstSelect.Add(TCMembersVM.Prop.EmployeeName.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.Email.MapAlias);



            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(string.Join(",", LstTCMemberID));
            m_objFilter.Add(TCMembersVM.Prop.TCMemberID.Map, m_lstFilter);
            string msg = string.Empty;
            Dictionary<int, DataSet> m_dicMMailNotificationsDA = m_objTTCMembersDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objTTCMembersDA.Message == string.Empty)
            {
                TCMembersVM m_objTCMembersVM = new TCMembersVM();
                DataRow m_drm_objTTCMembersDA = m_dicMMailNotificationsDA[0].Tables[0].Rows[0];
                m_objTCMembersVM.TCMemberID = m_drm_objTTCMembersDA[TCMembersVM.Prop.TCMemberID.Name].ToString();
                m_objTCMembersVM.EmployeeID = m_drm_objTTCMembersDA[TCMembersVM.Prop.EmployeeID.Name].ToString();
                m_objTCMembersVM.SuperiorID = m_drm_objTTCMembersDA[TCMembersVM.Prop.SuperiorID.Name].ToString();
                m_objTCMembersVM.TCTypeID = m_drm_objTTCMembersDA[TCMembersVM.Prop.TCTypeID.Name].ToString();
                m_objTCMembersVM.BusinessUnitID = m_drm_objTTCMembersDA[TCMembersVM.Prop.BusinessUnitID.Name].ToString();
                m_objTCMembersVM.PeriodStart = (DateTime)m_drm_objTTCMembersDA[TCMembersVM.Prop.PeriodStart.Name];
                m_objTCMembersVM.PeriodEnd = (DateTime)m_drm_objTTCMembersDA[TCMembersVM.Prop.PeriodEnd.Name];

                m_objTCMembersVM.EmployeeName = m_drm_objTTCMembersDA[TCMembersVM.Prop.EmployeeName.Name].ToString();
                m_objTCMembersVM.Email = m_drm_objTTCMembersDA[TCMembersVM.Prop.Email.Name].ToString();
                m_objTCMembersVM.EmployeeID = m_drm_objTTCMembersDA[TCMembersVM.Prop.EmployeeID.Name].ToString();
                m_lstTCMembersVM.Add(m_objTCMembersVM);
            }
            else
                message.Add("[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objTTCMembersDA.Message);

            return m_lstTCMembersVM;
        }
        private MailNotificationsVM GetMailNotificationsVM(string TaskID, ref List<string> message)
        {
            MailNotificationsVM m_objMailNotificationsVM = new MailNotificationsVM();
            MMailNotificationsDA m_objMMailNotificationsDA = new MMailNotificationsDA();
            m_objMMailNotificationsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();

            m_lstSelect.Add(MailNotificationsVM.Prop.MailNotificationID.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.FunctionID.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.FunctionDesc.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.Importance.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.Subject.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.Contents.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.StatusID.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.TaskID.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.FPTID.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.FPTDesc.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.CreatedDate.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.TaskStatusDesc.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.StatusDesc.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.NotificationTemplateID.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.NotificationTemplateDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(TaskID);
            m_objFilter.Add(MailNotificationsVM.Prop.TaskID.Map, m_lstFilter);
            string msg = string.Empty;
            Dictionary<int, DataSet> m_dicMMailNotificationsDA = m_objMMailNotificationsDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMMailNotificationsDA.Message == string.Empty)
            {

                DataRow m_drMMailNotificationsDA = m_dicMMailNotificationsDA[0].Tables[0].Rows[0];
                m_objMailNotificationsVM.MailNotificationID = m_drMMailNotificationsDA[MailNotificationsVM.Prop.MailNotificationID.Name].ToString();
                m_objMailNotificationsVM.FunctionID = m_drMMailNotificationsDA[MailNotificationsVM.Prop.FunctionID.Name].ToString();
                m_objMailNotificationsVM.FunctionDesc = m_drMMailNotificationsDA[MailNotificationsVM.Prop.FunctionDesc.Name].ToString();
                m_objMailNotificationsVM.Importance = (bool)m_drMMailNotificationsDA[MailNotificationsVM.Prop.Importance.Name];
                m_objMailNotificationsVM.Subject = m_drMMailNotificationsDA[MailNotificationsVM.Prop.Subject.Name].ToString();
                m_objMailNotificationsVM.Contents = m_drMMailNotificationsDA[MailNotificationsVM.Prop.Contents.Name].ToString();
                m_objMailNotificationsVM.StatusID = (int)m_drMMailNotificationsDA[MailNotificationsVM.Prop.StatusID.Name];
                m_objMailNotificationsVM.TaskID = m_drMMailNotificationsDA[MailNotificationsVM.Prop.TaskID.Name].ToString();
                m_objMailNotificationsVM.FPTID = m_drMMailNotificationsDA[MailNotificationsVM.Prop.FPTID.Name].ToString();
                m_objMailNotificationsVM.FPTDesc = m_drMMailNotificationsDA[MailNotificationsVM.Prop.FPTDesc.Name].ToString();
                m_objMailNotificationsVM.CreatedDate = (DateTime)m_drMMailNotificationsDA[MailNotificationsVM.Prop.CreatedDate.Name];
                m_objMailNotificationsVM.TaskStatusDesc = m_drMMailNotificationsDA[MailNotificationsVM.Prop.TaskStatusDesc.Name].ToString();
                m_objMailNotificationsVM.StatusDesc = m_drMMailNotificationsDA[MailNotificationsVM.Prop.StatusDesc.Name].ToString();
                m_objMailNotificationsVM.NotificationTemplateID = m_drMMailNotificationsDA[MailNotificationsVM.Prop.NotificationTemplateID.Name].ToString();
                m_objMailNotificationsVM.NotificationTemplateDesc = m_drMMailNotificationsDA[MailNotificationsVM.Prop.NotificationTemplateDesc.Name].ToString();

                m_objMailNotificationsVM.RecipientsVM = GetListRecipientsVM(m_objMailNotificationsVM.MailNotificationID, ref msg);
                m_objMailNotificationsVM.NotificationAttachmentVM = GetListNotificationAttachmentVM(m_objMailNotificationsVM.MailNotificationID, ref msg);

            }
            else
                message.Add("[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMMailNotificationsDA.Message);

            return m_objMailNotificationsVM;
        }
        private List<MailNotificationsVM> GetListMailNotificationsVM(string TaskID, ref List<string> message)
        {
            List<MailNotificationsVM> m_objLstMailNotificationsVM = new List<MailNotificationsVM>();
            MMailNotificationsDA m_objMMailNotificationsDA = new MMailNotificationsDA();
            m_objMMailNotificationsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();

            m_lstSelect.Add(MailNotificationsVM.Prop.MailNotificationID.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.FunctionID.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.FunctionDesc.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.Importance.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.Subject.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.Contents.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.StatusID.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.TaskID.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.FPTID.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.FPTDesc.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.CreatedDate.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.TaskStatusDesc.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.StatusDesc.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.NotificationTemplateID.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.NotificationTemplateDesc.MapAlias);
            m_lstSelect.Add(MailNotificationsVM.Prop.ScheduleStartDate.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(TaskID);
            m_objFilter.Add(MailNotificationsVM.Prop.TaskID.Map, m_lstFilter);
            string msg = string.Empty;

            Dictionary<string, OrderDirection> m_objOrderBy = new Dictionary<string, OrderDirection>();
            m_objOrderBy.Add(MailNotificationsVM.Prop.ScheduleStartDate.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicMMailNotificationsDA = m_objMMailNotificationsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, m_objOrderBy);
            if (m_objMMailNotificationsDA.Message == string.Empty)
            {
                foreach (DataRow m_drMMailNotificationsDA in m_dicMMailNotificationsDA[0].Tables[0].Rows)
                {
                    MailNotificationsVM m_objMailNotificationsVM = new MailNotificationsVM();

                    m_objMailNotificationsVM.MailNotificationID = m_drMMailNotificationsDA[MailNotificationsVM.Prop.MailNotificationID.Name].ToString();
                    m_objMailNotificationsVM.FunctionID = m_drMMailNotificationsDA[MailNotificationsVM.Prop.FunctionID.Name].ToString();
                    m_objMailNotificationsVM.FunctionDesc = m_drMMailNotificationsDA[MailNotificationsVM.Prop.FunctionDesc.Name].ToString();
                    m_objMailNotificationsVM.Importance = (bool)m_drMMailNotificationsDA[MailNotificationsVM.Prop.Importance.Name];
                    m_objMailNotificationsVM.Subject = m_drMMailNotificationsDA[MailNotificationsVM.Prop.Subject.Name].ToString();
                    m_objMailNotificationsVM.Contents = m_drMMailNotificationsDA[MailNotificationsVM.Prop.Contents.Name].ToString();
                    m_objMailNotificationsVM.StatusID = (int)m_drMMailNotificationsDA[MailNotificationsVM.Prop.StatusID.Name];
                    m_objMailNotificationsVM.TaskID = m_drMMailNotificationsDA[MailNotificationsVM.Prop.TaskID.Name].ToString();
                    m_objMailNotificationsVM.FPTID = m_drMMailNotificationsDA[MailNotificationsVM.Prop.FPTID.Name].ToString();
                    m_objMailNotificationsVM.FPTDesc = m_drMMailNotificationsDA[MailNotificationsVM.Prop.FPTDesc.Name].ToString();
                    m_objMailNotificationsVM.CreatedDate = (DateTime)m_drMMailNotificationsDA[MailNotificationsVM.Prop.CreatedDate.Name];
                    m_objMailNotificationsVM.TaskStatusDesc = m_drMMailNotificationsDA[MailNotificationsVM.Prop.TaskStatusDesc.Name].ToString();
                    m_objMailNotificationsVM.StatusDesc = m_drMMailNotificationsDA[MailNotificationsVM.Prop.StatusDesc.Name].ToString();
                    m_objMailNotificationsVM.ScheduleStartDate = m_drMMailNotificationsDA[MailNotificationsVM.Prop.ScheduleStartDate.Name].ToString();
                    m_objMailNotificationsVM.NotificationTemplateID = m_drMMailNotificationsDA[MailNotificationsVM.Prop.NotificationTemplateID.Name].ToString();
                    m_objMailNotificationsVM.NotificationTemplateDesc = m_drMMailNotificationsDA[MailNotificationsVM.Prop.NotificationTemplateDesc.Name].ToString();
                    m_objMailNotificationsVM.NotificationValuesVM = GetListNotificationValues(m_objMailNotificationsVM.MailNotificationID, m_objMailNotificationsVM.NotificationTemplateID);
                    m_objMailNotificationsVM.RecipientsVM = GetListRecipientsVM(m_objMailNotificationsVM.MailNotificationID, ref msg);
                    m_objMailNotificationsVM.NotificationAttachmentVM = GetListNotificationAttachmentVM(m_objMailNotificationsVM.MailNotificationID, ref msg);
                    if(!m_objLstMailNotificationsVM.Any(d=>d.MailNotificationID== m_objMailNotificationsVM.MailNotificationID))
                    m_objLstMailNotificationsVM.Add(m_objMailNotificationsVM);
                }
            }
            else
                message.Add("[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMMailNotificationsDA.Message);

            return m_objLstMailNotificationsVM;
        }
        private bool UpdateVendorWinner(List<FPTVendorWinnerVM> LstFPTVendorWinnerVM, object m_objDBConnection, ref List<string> message)
        {
            bool m_boolretval = true;
            int m_intlastnum = 0;
            //Get Last TNumber 
            List<TNumbering> m_lstTNumbering = new List<TNumbering>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();
            TNumberingDA m_objTNumberingDA = new TNumberingDA();
            DFPTVendorWinnerDA m_objDFPTVendorWinnerDA = new DFPTVendorWinnerDA();


            m_objTNumberingDA.ConnectionStringName = Global.ConnStrConfigName;
            m_objDFPTVendorWinnerDA.ConnectionStringName = Global.ConnStrConfigName;
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("SP");
            m_objFilter.Add(nameof(TNumbering.Header), m_lstFilter);
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(DateTime.Now.Year.ToString());//todo:
            m_objFilter.Add(nameof(TNumbering.Year), m_lstFilter);

            m_lstSelect = new List<string>();
            m_lstSelect.Add(nameof(TNumbering.LastNo));

            Dictionary<int, DataSet> m_dicTNumberingDA = m_objTNumberingDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null, null);
            if (m_objTNumberingDA.Success)
            {
                foreach (DataRow item in m_dicTNumberingDA[0].Tables[0].Rows)
                {
                    TNumbering m_objTNumbering = new TNumbering();
                    m_objTNumbering.LastNo = (int)item[nameof(TNumbering.LastNo)];
                    m_lstTNumbering.Add(m_objTNumbering);
                }
            }
            if (m_lstTNumbering.Any())
            {
                m_intlastnum = m_lstTNumbering.FirstOrDefault().LastNo;
            }
            else
            {
                //insert tnumbering
                TNumbering m_objTNumberinginsert = new TNumbering();
                m_objTNumberinginsert.Header = "SP";
                m_objTNumberinginsert.Year = DateTime.Now.Year.ToString();
                m_objTNumberinginsert.Month = string.Empty;
                m_objTNumberinginsert.CompanyID = string.Empty;
                m_objTNumberinginsert.ProjectID = string.Empty;
                m_objTNumberinginsert.LastNo = 0;

                m_objTNumberingDA.Data = m_objTNumberinginsert;
                m_objTNumberingDA.Insert(true, m_objDBConnection);
                if (!m_objTNumberingDA.Success || m_objTNumberingDA.Message != string.Empty)
                {
                    return false;
                }
            }

            //Update vendor winner 
            var m_lstVendorWinner = LstFPTVendorWinnerVM;
            foreach (var item in m_lstVendorWinner)
            {
                m_intlastnum += 1;
                DFPTVendorWinner m_objDFPTVendorWinner = new DFPTVendorWinner();
                m_objDFPTVendorWinner.VendorWinnerID = item.VendorWinnerID;
                m_objDFPTVendorWinnerDA.Data = m_objDFPTVendorWinner;
                m_objDFPTVendorWinnerDA.Select();
                m_objDFPTVendorWinner.LetterNumber = m_intlastnum.ToString().PadLeft(4, '0');

                m_objDFPTVendorWinnerDA.Update(true, m_objDBConnection);
                if (!m_objDFPTVendorWinnerDA.Success || m_objDFPTVendorWinnerDA.Message != string.Empty)
                {
                    return false;
                }
            }

            //Update Tnumbering
            TNumbering m_objTNumberingupdate = new TNumbering();
            m_objTNumberingupdate.Header = "SP";
            m_objTNumberingupdate.Year = DateTime.Now.Year.ToString();
            m_objTNumberingupdate.Month = string.Empty;
            m_objTNumberingupdate.CompanyID = string.Empty;
            m_objTNumberingupdate.ProjectID = string.Empty;
            m_objTNumberingDA.Data = m_objTNumberingupdate;
            m_objTNumberingDA.Select(m_objDBConnection);
            m_objTNumberingupdate.LastNo = m_intlastnum;
            m_objTNumberingDA.Update(true, m_objDBConnection);
            if (m_lstVendorWinner.FirstOrDefault().IsSync)
            {
                Sync(m_lstVendorWinner.FirstOrDefault().FPTID, ref message);
            }
            if (!m_objTNumberingDA.Success || m_objTNumberingDA.Message != string.Empty)
            {
                message.Add(m_objTNumberingDA.Message);
                return false;
            }

            return m_boolretval;
        }
        private bool Sync(string FPTID, ref List<string> message)
        {

            string m_soapenv = $@"<?xml version='1.0' encoding='utf-8'?>
            <soap:Envelope xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/'>
              <soap:Body>
                <UpdateAppointment xmlns='http://tempuri.org/'>
                  <prmRequestNo>{FPTID}</prmRequestNo>
                  <prmDtTarget>{DateTime.Now.ToString("dd.MM.yyyy")}</prmDtTarget>
                  <prmDtActual>{DateTime.Now.ToString("dd.MM.yyyy")}</prmDtActual>
                  <prmUpdateOnly>0</prmUpdateOnly>
                </UpdateAppointment>
              </soap:Body>
            </soap:Envelope>";


            var m_lstconfig = GetConfig("WS", null, "ETT");
            if (!m_lstconfig.Any())
            {
                message.Add("Fail Sync ETT");
                return false;
            }
            string m_struser = m_lstconfig.Any(x => x.Key2 == "Credential" && x.Key4 == "User") ? m_lstconfig.Where(x => x.Key2 == "Credential" && x.Key4 == "User").FirstOrDefault().Desc1 : string.Empty;
            string m_strpass = m_lstconfig.Any(x => x.Key2 == "Credential" && x.Key4 == "Password") ? m_lstconfig.Where(x => x.Key2 == "Credential" && x.Key4 == "Password").FirstOrDefault().Desc1 : string.Empty;
            string m_strdomain = m_lstconfig.Any(x => x.Key2 == "Credential" && x.Key4 == "Domain") ? m_lstconfig.Where(x => x.Key2 == "Credential" && x.Key4 == "Domain").FirstOrDefault().Desc1 : string.Empty;
            string m_strurl = m_lstconfig.Any(x => x.Key2 == "URL" && x.Key4 == "Division") ? m_lstconfig.Where(x => x.Key2 == "URL" && x.Key4 == "Division").FirstOrDefault().Desc1 : string.Empty;
            NetworkCredential m_credential = new NetworkCredential(m_struser, m_strpass, m_strdomain);
            string m_strsoapresult = GetSOAPString(m_soapenv, m_strurl, m_credential);
            if (string.IsNullOrEmpty(m_strsoapresult))
            {
                return false;
            }

            System.Xml.XmlDocument document = new System.Xml.XmlDocument();
            document.LoadXml(m_strsoapresult);
            XmlNamespaceManager manager = new XmlNamespaceManager(document.NameTable);

            XmlNodeList xnList = document.GetElementsByTagName("UpdateAppointmentResult");

            foreach (XmlNode item in xnList)
            {
                if (/*item["UpdateAppointmentResult"].InnerText == string.Empty*/ item.Name == "UpdateAppointmentResult" && item.InnerText == "")
                {
                    return true;
                }
                else
                {
                    message.Add("Fail Sync ETT");
                    return false;
                }
            }
            message.Add("Fail Sync ETT");
            return false;

        }
        private bool CreateMailNotifNego(List<NegotiationConfigurationsVM> LstNegotiationConfigurationsVM, List<FPTVendorParticipantsVM> LstFPTVendorParticipantsVM, List<TCMembersVM> LstTCMembersVM, FPTVM m_objFPTVM, ref List<string> message)
        {

            var m_lstconfig = GetConfig("FunctionID");
            var m_lstconfigformat = GetConfig("FormatString");
            //FPTVM m_objFPTVM = GetFPTVM(LstNegotiationConfigurationsVM.FirstOrDefault().FPTID, m_objDBConnection);
            m_objFPTVM.ScheduleDate = Convert.ToDateTime(LstNegotiationConfigurationsVM.FirstOrDefault(x => x.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.Schedule)).ParameterValue);


            //TODO:
            //List Config
            //DateTime m_datenego = Convert.ToDateTime(LstNegotiationConfigurationsVM.FirstOrDefault(x => x.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.Schedule)).ParameterValue);
            //string m_Budgetplan = LstNegotiationConfigurationsVM.FirstOrDefault().BudgetPlan;
            //string m_Fpt = LstNegotiationConfigurationsVM.FirstOrDefault().FPTID;

            //List Vendor
            NotificationMapVM m_NotificationMapVMVendor = GetDefaultNoticationMap(m_lstconfig.Where(x => x.Key2 == "InvitationVendor").FirstOrDefault().Desc1);
            List<FieldTagReferenceVM> m_lstFieldTagReferenceVMVendor = GetListFieldTagReferenceVM(m_NotificationMapVMVendor.NotificationTemplateID);
            PropertyInfo[] m_arrPInfoVendor = (new FPTVM()).GetType().GetProperties();
            m_lstFieldTagReferenceVMVendor = m_lstFieldTagReferenceVMVendor.Where(y => m_arrPInfoVendor.Select(x => x.Name).ToList().Contains(y.RefIDColumn)).ToList();

            foreach (var item in LstFPTVendorParticipantsVM)
            {

                m_objFPTVM.VendorName = item.VendorName;

                string m_strsubject = "Undangan negosiasi";
                MailNotificationsVM m_MailNotificationsVM = new MailNotificationsVM();
                m_MailNotificationsVM.MailNotificationID = "";
                m_MailNotificationsVM.Importance = true;
                m_MailNotificationsVM.Subject = $"{m_strsubject} {item.FPTID} {item.ParameterValue} {item.VendorName}";
                m_MailNotificationsVM.Contents = "";
                m_MailNotificationsVM.StatusID = (int)NotificationStatus.Draft;
                m_MailNotificationsVM.FPTID = item.FPTID;


                //Values
                PropertyInfo[] m_arrPInfoitem = m_objFPTVM.GetType().GetProperties();
                m_MailNotificationsVM.NotificationValuesVM = new List<NotificationValuesVM>();
                foreach (var m_FieldTagReferenceVM in m_lstFieldTagReferenceVMVendor)
                {
                    NotificationValuesVM M_NotificationValuesVMref = new NotificationValuesVM();
                    M_NotificationValuesVMref.NotificationValueID = Guid.NewGuid().ToString("N");
                    M_NotificationValuesVMref.MailNotificationID = "";
                    M_NotificationValuesVMref.FieldTagID = m_FieldTagReferenceVM.FieldTagID;
                    var m_objvalue = m_arrPInfoitem.FirstOrDefault(d => d.Name.Equals(m_FieldTagReferenceVM.RefIDColumn)).GetValue(m_objFPTVM);
                    string m_strvalue = string.Empty;
                    string m_strformat = string.Empty;
                    string m_strculture = string.Empty;
                    m_strformat = m_lstconfigformat.Where(x => x.Key3 == m_FieldTagReferenceVM.FieldTagID).Any() ? m_lstconfigformat.Where(x => x.Key3 == m_FieldTagReferenceVM.FieldTagID).FirstOrDefault().Desc1 : string.Empty;
                    m_strculture = m_lstconfigformat.Where(x => x.Key3 == m_FieldTagReferenceVM.FieldTagID).Any() ? m_lstconfigformat.Where(x => x.Key3 == m_FieldTagReferenceVM.FieldTagID).FirstOrDefault().Desc2 : string.Empty;
                    if (Object.ReferenceEquals(m_objvalue.GetType(), typeof(DateTime)))
                    {
                        m_strvalue = m_objvalue == null ? "" : ((DateTime)m_objvalue).ToString(m_strformat, new System.Globalization.CultureInfo(m_strculture));
                    }
                    else
                    {
                        m_strvalue = m_objvalue.ToString();
                    }

                    M_NotificationValuesVMref.Value = m_strvalue;
                    m_MailNotificationsVM.NotificationValuesVM.Add(M_NotificationValuesVMref);
                }




                //Recipient
                List<RecipientsVM> m_lstm_RecipientsVM = new List<RecipientsVM>();
                var m_vendoruserid = GetUserID(true, item.VendorID).UserID;
                foreach (var vendoremail in GetVendorDefaultEmail(item.VendorID))
                {
                    RecipientsVM m_RecipientsVM = new RecipientsVM();
                    m_RecipientsVM.RecipientID = "";
                    m_RecipientsVM.RecipientDesc = item.FirstName;
                    m_RecipientsVM.MailAddress = vendoremail;
                    m_RecipientsVM.OwnerID = m_vendoruserid;
                    m_RecipientsVM.RecipientTypeID = ((int)RecipientTypes.TO).ToString();
                    m_lstm_RecipientsVM.Add(m_RecipientsVM);
                }
                m_MailNotificationsVM.RecipientsVM = m_lstm_RecipientsVM;
                m_MailNotificationsVM.NotificationMapVM = m_NotificationMapVMVendor;
                string MailnotifID = string.Empty;

                if (this.CreateMailNotification(m_MailNotificationsVM, true, ref message, ref MailnotifID))
                {
                    MSchedules m_Schedules = new MSchedules();
                    m_Schedules.ScheduleID = Guid.NewGuid().ToString("N");
                    m_Schedules.StartDate = m_objFPTVM.ScheduleDate;
                    m_Schedules.EndDate = m_objFPTVM.ScheduleDate;
                    m_Schedules.Subject = $"{m_strsubject} {item.FPTID} {item.ParameterValue} {item.VendorName}";
                    m_Schedules.Notes = string.Empty;
                    m_Schedules.Weblink = string.Empty;
                    m_Schedules.Location = string.Empty;
                    m_Schedules.Priority = 0;
                    m_Schedules.IsAllDay = true;
                    m_Schedules.TaskID = null;
                    m_Schedules.StatusID = ((int)ScheduleStatus.Approved).ToString();
                    m_Schedules.FPTID = null;
                    m_Schedules.MailNotificationID = MailnotifID;
                    m_Schedules.ProjectID = string.IsNullOrEmpty(m_objFPTVM.ProjectID) ? null : m_objFPTVM.ProjectID;
                    m_Schedules.ClusterID = string.IsNullOrEmpty(m_objFPTVM.ClusterID) ? null : m_objFPTVM.ClusterID;
                    CreateSchedule(m_Schedules, null, ref message);
                }
                else
                {
                    return false;
                }

            }
            //List TC
            NotificationMapVM m_NotificationMapVMTC = GetDefaultNoticationMap(m_lstconfig.Where(x => x.Key2 == "InvitationTC").FirstOrDefault().Desc1);
            List<FieldTagReferenceVM> m_lstFieldTagReferenceVMTC = GetListFieldTagReferenceVM(m_NotificationMapVMTC.NotificationTemplateID);
            PropertyInfo[] m_arrPInfoTC = (new FPTVM()).GetType().GetProperties();
            m_lstFieldTagReferenceVMTC = m_lstFieldTagReferenceVMTC.Where(y => m_arrPInfoVendor.Select(x => x.Name).ToList().Contains(y.RefIDColumn)).ToList();
            m_objFPTVM.ListVendor = string.Join(",", LstFPTVendorParticipantsVM.Select(x => x.VendorName).Distinct().ToList());
            foreach (var item in LstTCMembersVM)
            {

                m_objFPTVM.TCName = item.EmployeeName;
                m_objFPTVM.TCType = item.TCTypeID;

                string m_strsubject = "Undangan Negosiasi";
                MailNotificationsVM m_MailNotificationsVM = new MailNotificationsVM();
                m_MailNotificationsVM.MailNotificationID = "";
                m_MailNotificationsVM.Importance = true;
                m_MailNotificationsVM.Subject = $"{m_strsubject} {m_objFPTVM.FPTID} {m_objFPTVM.BudgetPlans} {item.EmployeeName}";
                m_MailNotificationsVM.Contents = "";
                m_MailNotificationsVM.StatusID = (int)NotificationStatus.Draft;
                m_MailNotificationsVM.FPTID = m_objFPTVM.FPTID;


                //Values
                PropertyInfo[] m_arrPInfoitem = m_objFPTVM.GetType().GetProperties();
                m_MailNotificationsVM.NotificationValuesVM = new List<NotificationValuesVM>();
                foreach (var m_FieldTagReferenceVM in m_lstFieldTagReferenceVMTC)
                {
                    NotificationValuesVM M_NotificationValuesVMref = new NotificationValuesVM();
                    M_NotificationValuesVMref.NotificationValueID = Guid.NewGuid().ToString("N");
                    M_NotificationValuesVMref.MailNotificationID = "";
                    M_NotificationValuesVMref.FieldTagID = m_FieldTagReferenceVM.FieldTagID;
                    var m_objvalue = m_arrPInfoitem.FirstOrDefault(d => d.Name.Equals(m_FieldTagReferenceVM.RefIDColumn)).GetValue(m_objFPTVM);
                    string m_strvalue = string.Empty;
                    string m_strformat = string.Empty;
                    string m_strculture = string.Empty;
                    m_strformat = m_lstconfigformat.Where(x => x.Key3 == m_FieldTagReferenceVM.FieldTagID).Any() ? m_lstconfigformat.Where(x => x.Key3 == m_FieldTagReferenceVM.FieldTagID).FirstOrDefault().Desc1 : string.Empty;
                    m_strculture = m_lstconfigformat.Where(x => x.Key3 == m_FieldTagReferenceVM.FieldTagID).Any() ? m_lstconfigformat.Where(x => x.Key3 == m_FieldTagReferenceVM.FieldTagID).FirstOrDefault().Desc2 : string.Empty;
                    if (Object.ReferenceEquals(m_objvalue.GetType(), typeof(DateTime)))
                    {
                        m_strvalue = m_objvalue == null ? "" : ((DateTime)m_objvalue).ToString(m_strformat, new System.Globalization.CultureInfo(m_strculture));
                    }
                    else
                    {
                        m_strvalue = m_objvalue.ToString();
                    }

                    M_NotificationValuesVMref.Value = m_strvalue;

                    m_MailNotificationsVM.NotificationValuesVM.Add(M_NotificationValuesVMref);
                }

                //Recipient
                RecipientsVM m_RecipientsVM = new RecipientsVM();
                m_RecipientsVM.RecipientID = "";
                m_RecipientsVM.RecipientDesc = item.EmployeeName;
                m_RecipientsVM.MailAddress = item.Email;
                m_RecipientsVM.OwnerID = GetUserID(false, item.EmployeeID).UserID;
                m_RecipientsVM.RecipientTypeID = ((int)RecipientTypes.TO).ToString();
                List<RecipientsVM> m_lstm_RecipientsVM = new List<RecipientsVM>();
                m_lstm_RecipientsVM.Add(m_RecipientsVM);

                List<TCMembersVM> m_lstTCMember = GetTCDelegation(item.TCMemberID, m_objFPTVM.ScheduleDate);
                if (m_lstTCMember.Any())
                {
                    foreach (var tcmember in m_lstTCMember)
                    {
                        RecipientsVM m_RecipientsVMtc = new RecipientsVM();
                        m_RecipientsVMtc.RecipientID = "";
                        m_RecipientsVMtc.RecipientDesc = item.EmployeeName;
                        m_RecipientsVMtc.MailAddress = item.Email;
                        m_RecipientsVMtc.OwnerID = GetUserID(false, item.EmployeeID).UserID;
                        m_RecipientsVMtc.RecipientTypeID = ((int)RecipientTypes.CC).ToString();
                        m_lstm_RecipientsVM.Add(m_RecipientsVMtc);
                    }
                }

                m_MailNotificationsVM.RecipientsVM = m_lstm_RecipientsVM;
                m_MailNotificationsVM.NotificationMapVM = m_NotificationMapVMTC;


                string MailnotifID = string.Empty;
                if (this.CreateMailNotification(m_MailNotificationsVM, true, ref message, ref MailnotifID))
                {
                    MSchedules m_Schedules = new MSchedules();
                    m_Schedules.ScheduleID = Guid.NewGuid().ToString("N");
                    m_Schedules.StartDate = m_objFPTVM.ScheduleDate;
                    m_Schedules.EndDate = m_objFPTVM.ScheduleDate;
                    m_Schedules.Subject = $"{m_strsubject} {m_objFPTVM.FPTID} {m_objFPTVM.BudgetPlans} {item.EmployeeName}";
                    m_Schedules.Notes = string.Empty;
                    m_Schedules.Weblink = string.Empty;
                    m_Schedules.Location = string.Empty;
                    m_Schedules.Priority = 0;
                    m_Schedules.IsAllDay = true;
                    m_Schedules.TaskID = null;
                    m_Schedules.StatusID = ((int)ScheduleStatus.Approved).ToString();
                    m_Schedules.FPTID = null;
                    m_Schedules.MailNotificationID = MailnotifID;
                    m_Schedules.ProjectID = string.IsNullOrEmpty(m_objFPTVM.ProjectID) ? null : m_objFPTVM.ProjectID;
                    m_Schedules.ClusterID = string.IsNullOrEmpty(m_objFPTVM.ClusterID) ? null : m_objFPTVM.ClusterID;
                    CreateSchedule(m_Schedules, null, ref message);
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
        private List<TCMembersVM> GetTCDelegation(string TCMemberID, DateTime Date)
        {
            List<TCMembersVM> Result = new List<TCMembersVM>();
            List<string> message = new List<string>();
            List<TCMembersDelegationVM> m_lstTCMembersDelegationVM = new List<TCMembersDelegationVM>();
            TTCMemberDelegationsDA m_objTTCMemberDelegationsDA = new TTCMemberDelegationsDA();
            m_objTTCMemberDelegationsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();

            m_lstSelect.Add(TCMembersDelegationVM.Prop.TCDelegationID.MapAlias);
            m_lstSelect.Add(TCMembersDelegationVM.Prop.TCMemberID.MapAlias);
            m_lstSelect.Add(TCMembersDelegationVM.Prop.DelegateTo.MapAlias);
            m_lstSelect.Add(TCMembersDelegationVM.Prop.DelegateStartDate.MapAlias);
            m_lstSelect.Add(TCMembersDelegationVM.Prop.DelegateEndDate.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(TCMemberID);
            m_objFilter.Add(TCMembersVM.Prop.TCMemberID.Map, m_lstFilter);


            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.LessThanEqual);
            m_lstFilter.Add(Date);
            m_objFilter.Add(TCMembersVM.Prop.DelegateStartDate.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.GreaterThanEqual);
            m_lstFilter.Add(Date);
            m_objFilter.Add(TCMembersVM.Prop.DelegateEndDate.Map, m_lstFilter);


            string msg = string.Empty;
            Dictionary<int, DataSet> m_dicMMailNotificationsDA = m_objTTCMemberDelegationsDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objTTCMemberDelegationsDA.Message == string.Empty)
            {
                TCMembersDelegationVM m_objTCMembersDelegationVM = new TCMembersDelegationVM();
                DataRow m_drm_objTTCMembersDA = m_dicMMailNotificationsDA[0].Tables[0].Rows[0];
                m_objTCMembersDelegationVM.TCDelegationID = m_drm_objTTCMembersDA[TCMembersDelegationVM.Prop.TCDelegationID.Name].ToString();
                m_objTCMembersDelegationVM.TCMemberID = m_drm_objTTCMembersDA[TCMembersDelegationVM.Prop.TCMemberID.Name].ToString();
                m_objTCMembersDelegationVM.DelegateTo = m_drm_objTTCMembersDA[TCMembersDelegationVM.Prop.DelegateTo.Name].ToString();
                m_objTCMembersDelegationVM.DelegateStartDate = (DateTime)m_drm_objTTCMembersDA[TCMembersDelegationVM.Prop.DelegateStartDate.Name];
                m_objTCMembersDelegationVM.DelegateEndDate = (DateTime)m_drm_objTTCMembersDA[TCMembersDelegationVM.Prop.DelegateEndDate.Name];
                m_lstTCMembersDelegationVM.Add(m_objTCMembersDelegationVM);
            }
            else
            {

            }
            if (m_lstTCMembersDelegationVM.Any())
            {
                Result = GetTCMembersVM(m_lstTCMembersDelegationVM.Select(x => x.DelegateTo).ToList(), ref message);
            }

            return Result;
        }
        private NotificationAttachmentVM GetSP(FPTVendorWinnerVM Model)
        {
            NotificationAttachmentVM m_NotificationAttachmentVM = new NotificationAttachmentVM();
            if (!Directory.Exists(Server.MapPath(@"~/Temp/"))) Directory.CreateDirectory(Server.MapPath(@"~/Temp/"));
            string m_filepath = Server.MapPath($"~/Content/Template/Report/sp-new-tender-ID.docx");

            DocX m_document = null;

            List<string> m_lstfilename = new List<string>();
            try
            {
                m_document = DocX.Load(m_filepath);
            }
            catch (Exception e)
            {
                throw e;
            }

            Model.FPTID = Model.FPTID == null ? string.Empty : Model.FPTID;
            Model.VendorName = Model.VendorName == null ? string.Empty : Model.VendorName;
            Model.VendorAddress = Model.VendorAddress == null ? string.Empty : Model.VendorAddress;
            Model.VendorEmail = Model.VendorEmail == null ? string.Empty : Model.VendorEmail;
            Model.VendorPhone = Model.VendorPhone == null ? string.Empty : Model.VendorPhone;
            Model.ProjectName = Model.ProjectName == null ? string.Empty : Model.ProjectName;
            Model.CompanyDesc = Model.CompanyDesc == null ? string.Empty : Model.CompanyDesc;
            Model.BudgetPlanName = Model.BudgetPlanName == null ? string.Empty : Model.BudgetPlanName;

            decimal m_Bidafterfee = Model.BidValue * (1 + (Model.BidFee / 100));
            m_Bidafterfee = Math.Round(m_Bidafterfee);
            DocX m_documenttemp = m_document.Copy();
            string m_letterno = (Model.ModifiedDate == null) ? "..." : $"{Model.LetterNumber.ToString()}/TRM-{Model.BusinessUnitID}-{Model.DivisionID}/TENDER/{ToRoman(((DateTime)Model.ModifiedDate).Month)}/{((DateTime)Model.ModifiedDate).ToString("yy")}";
            string m_rnd = "sp-new-tender " + Model.FPTID + " " + Model.BudgetPlanName + " " + Model.VendorName;
            m_documenttemp.ReplaceText("[date]", FormatDateReport(DateTime.Now, "ID"));
            m_documenttemp.ReplaceText("[letterno]", m_letterno);
            m_documenttemp.ReplaceText("[vendorname]", Model.VendorName);
            m_documenttemp.ReplaceText("[vendoraddress]", Model.VendorAddress);
            m_documenttemp.ReplaceText("[vendorup]", "...");
            m_documenttemp.ReplaceText("[vendoremail]", Model.VendorEmail);
            m_documenttemp.ReplaceText("[vendortelp]", Model.VendorPhone != null ? Model.VendorPhone : "");
            m_documenttemp.ReplaceText("[vendorfax]", "...");
            string m_strBid = ("ID" == "ID") ? Global.GetTerbilang((long)m_Bidafterfee) : Global.GetTerbilangEN(m_Bidafterfee);
            m_strBid += " Rupiah";
            m_documenttemp.ReplaceText("[bidvalue]", $"{((long)m_Bidafterfee).ToString(Global.DefaultNumberFormat)} ({m_strBid})");
            m_documenttemp.ReplaceText("[trm]", "...");
            m_documenttemp.ReplaceText("[budgetplan]", Model.BudgetPlanName != null ? Model.BudgetPlanName : "");
            m_documenttemp.ReplaceText("[project]", Model.ProjectName != null ? Model.ProjectName : "");
            m_documenttemp.ReplaceText("[company]", Model.CompanyDesc != null ? Model.CompanyDesc : "");
            m_documenttemp.ReplaceText("[cc]", "..");
            m_documenttemp.SaveAs(Server.MapPath("~/Temp/" + m_rnd + ".docx"));
            m_lstfilename.Add(m_rnd);
            m_NotificationAttachmentVM.AttachmentValueID = "";
            m_NotificationAttachmentVM.Filename = "Surat Penunjukan Tender.zip";
            m_NotificationAttachmentVM.ContentType = "application/zip";

            using (var memoryStream = new MemoryStream())
            {
                using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var item in m_lstfilename)
                    {
                        ziparchive.CreateEntryFromFile(Server.MapPath("~/Temp/" + item + ".docx"), item + ".docx");
                    }
                }
                foreach (var item in m_lstfilename)
                {
                    System.IO.File.Delete(Server.MapPath("~/Temp/" + item + ".docx"));
                }
                m_NotificationAttachmentVM.RawData = Convert.ToBase64String(memoryStream.ToArray());
            }

            return m_NotificationAttachmentVM;
        }
        private NotificationAttachmentVM GetTL(FPTVendorWinnerVM Model, string Winner)
        {
            NotificationAttachmentVM m_NotificationAttachmentVM = new NotificationAttachmentVM();
            m_NotificationAttachmentVM.AttachmentValueID = "";
            m_NotificationAttachmentVM.Filename = "Thank You Letter.zip";
            m_NotificationAttachmentVM.ContentType = "application/zip";


            if (!Directory.Exists(Server.MapPath(@"~/Temp/"))) Directory.CreateDirectory(Server.MapPath(@"~/Temp/"));

            string m_filepath = Server.MapPath($"~/Content/Template/Report/thank-you-letter-ID.docx");
            DocX m_document = null;


            List<string> m_lstfilename = new List<string>();
            try
            {
                m_document = DocX.Load(m_filepath);
            }
            catch (Exception e)
            {
                throw e;
            }

            Model.FPTID = Model.FPTID == null ? string.Empty : Model.FPTID;
            Model.VendorName = Model.VendorName == null ? string.Empty : Model.VendorName;
            Model.VendorAddress = Model.VendorAddress == null ? string.Empty : Model.VendorAddress;
            Model.VendorEmail = Model.VendorEmail == null ? string.Empty : Model.VendorEmail;
            Model.VendorPhone = Model.VendorPhone == null ? string.Empty : Model.VendorPhone;
            Model.ProjectName = Model.ProjectName == null ? string.Empty : Model.ProjectName;
            Model.CompanyDesc = Model.CompanyDesc == null ? string.Empty : Model.CompanyDesc;
            Model.BudgetPlanName = Model.BudgetPlanName == null ? string.Empty : Model.BudgetPlanName;

            DocX m_documenttemp = m_document.Copy();
            string m_rnd = "FPTPPT " + Model.FPTID + " " + Model.BudgetPlanName + " " + Model.VendorName;
            string m_letterno = (Model.ModifiedDate == null) ? "..." : $"{Model.LetterNumber.ToString()}/TRM-{Model.BusinessUnitID}-{Model.DivisionID}/TENDER/{ToRoman(((DateTime)Model.ModifiedDate).Month)}/{((DateTime)Model.ModifiedDate).ToString("yy")}";
            m_documenttemp.ReplaceText("[date]", FormatDateReport(DateTime.Now, "ID"));
            m_documenttemp.ReplaceText("[letterno]", m_letterno);
            m_documenttemp.ReplaceText("[vendorname]", Model.VendorName);
            m_documenttemp.ReplaceText("[vendoraddress]", Model.VendorAddress);
            m_documenttemp.ReplaceText("[vendorup]", "...");
            m_documenttemp.ReplaceText("[vendoremail]", Model.VendorEmail);
            m_documenttemp.ReplaceText("[vendortelp]", Model.VendorPhone);
            m_documenttemp.ReplaceText("[vendorfax]", "...");
            m_documenttemp.ReplaceText("[vendorwinner]", Winner);//todo: use bpid
            m_documenttemp.ReplaceText("[trm]", "...");
            m_documenttemp.ReplaceText("[project]", Model.ProjectName);
            m_documenttemp.ReplaceText("[budgetplan]", Model.BudgetPlanName);
            m_documenttemp.ReplaceText("[company]", Model.CompanyDesc);
            m_documenttemp.ReplaceText("[cc]", "...");

            m_documenttemp.SaveAs(Server.MapPath("~/Temp/" + m_rnd + ".docx"));
            m_lstfilename.Add(m_rnd);

            using (var memoryStream = new MemoryStream())
            {
                using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var item in m_lstfilename)
                    {
                        ziparchive.CreateEntryFromFile(Server.MapPath("~/Temp/" + item + ".docx"), item + ".docx");
                    }
                }
                foreach (var item in m_lstfilename)
                {
                    System.IO.File.Delete(Server.MapPath("~/Temp/" + item + ".docx"));
                }
                m_NotificationAttachmentVM.RawData = Convert.ToBase64String(memoryStream.ToArray());
            }

            return m_NotificationAttachmentVM;
        }
        private NotificationMapVM GetDefaultNoticationMap(string FunctionID)
        {
            DNotificationMapDA m_objDNotificationMapDA = new DNotificationMapDA();
            m_objDNotificationMapDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(NotificationMapVM.Prop.NotifMapID.MapAlias);
            m_lstSelect.Add(NotificationMapVM.Prop.FunctionID.MapAlias);
            m_lstSelect.Add(NotificationMapVM.Prop.IsDefault.MapAlias);
            m_lstSelect.Add(NotificationMapVM.Prop.NotificationTemplateID.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(FunctionID);
            m_objFilter.Add(NotificationMapVM.Prop.FunctionID.Map, m_lstFilter);

            //m_lstFilter = new List<object>();
            //m_lstFilter.Add(Operator.Equals);
            //m_lstFilter.Add(NotificationTemplateID);
            //m_objFilter.Add(NotificationMapVM.Prop.NotificationTemplateID.Map, m_lstFilter);


            //Dictionary<string, OrderDirection> m_objOrderBy = new Dictionary<string, OrderDirection>();
            //m_objOrderBy.Add(NotificationMapVM.Prop.FunctionID.Map, OrderDirection.Ascending);
            List<NotificationMapVM> m_lstNotificationMapVM = new List<NotificationMapVM>();
            Dictionary<int, DataSet> m_dicDNotificationMap = m_objDNotificationMapDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);
            if (m_objDNotificationMapDA.Message == string.Empty)
            {
                m_lstNotificationMapVM = (
                  from DataRow m_drDNotificationMap in m_dicDNotificationMap[0].Tables[0].Rows
                  select new NotificationMapVM
                  {
                      NotifMapID = m_drDNotificationMap[NotificationMapVM.Prop.NotifMapID.Name].ToString(),
                      FunctionID = m_drDNotificationMap[NotificationMapVM.Prop.FunctionID.Name].ToString(),
                      NotificationTemplateID = m_drDNotificationMap[NotificationMapVM.Prop.NotificationTemplateID.Name].ToString(),
                      IsDefault = (bool)m_drDNotificationMap[NotificationMapVM.Prop.IsDefault.Name]
                  }
                ).ToList();
            }
            NotificationMapVM m_objNotificationMapVM = m_lstNotificationMapVM.Where(x => x.IsDefault).Any() ? m_lstNotificationMapVM.Where(x => x.IsDefault).FirstOrDefault() : null;
            return m_objNotificationMapVM;
        }
        private bool UpdateMSchedule(string ScheduleID, string TaskID, int TaskType, object m_objDBConnection,  ref string message, ref List<string> ListMailNotif,bool isMupltipleSchedule=false, List<MailNotificationsVM> lsMailNotificationsVM=null)
        {
            bool retSuccess = false;
            MSchedulesDA m_objMSchedulesDA = new MSchedulesDA();
            m_objMSchedulesDA.ConnectionStringName = Global.ConnStrConfigName;

            MMailNotificationsDA m_objMMailNotifDA = new MMailNotificationsDA();
            m_objMMailNotifDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(MailNotificationsVM.Prop.MailNotificationID.MapAlias);


            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(TaskID);
            m_objFilter.Add(MailNotificationsVM.Prop.TaskID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMMailNotifDA = m_objMMailNotifDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

            //string MailNotificationID = "";
            if (m_objMMailNotifDA.Success && string.IsNullOrEmpty(m_objMMailNotifDA.Message))
            {
                DataRow m_drMMailNotifDA = m_dicMMailNotifDA[0].Tables[0].Rows[0];
                foreach (DataRow dr in m_dicMMailNotifDA[0].Tables[0].Rows)
                    if(!ListMailNotif.Any(d=>d.Equals(dr[MailNotificationsVM.Prop.MailNotificationID.Name].ToString())))
                        ListMailNotif.Add(dr[MailNotificationsVM.Prop.MailNotificationID.Name].ToString());

                m_objMSchedulesDA.Data = new MSchedules()
                {
                    ScheduleID = ScheduleID
                };
                m_objMSchedulesDA.Select();

                if (TaskType == (int)TaskStatus.Approved)
                {
                    if (m_objMSchedulesDA.Data.StatusID == ((int)ScheduleStatus.Submitted).ToString())
                        m_objMSchedulesDA.Data.StatusID = ((int)ScheduleStatus.Approved).ToString();
                    else if (m_objMSchedulesDA.Data.StatusID == ((int)ScheduleStatus.Submitted_Reschedule).ToString())
                        m_objMSchedulesDA.Data.StatusID = ((int)ScheduleStatus.Rescheduled).ToString();
                    else if (m_objMSchedulesDA.Data.StatusID == ((int)ScheduleStatus.Submitted_Cancellation).ToString())
                        m_objMSchedulesDA.Data.StatusID = ((int)ScheduleStatus.Cancelled).ToString();
                }
                else if (TaskType == (int)TaskStatus.Rejected)
                {
                    m_objMSchedulesDA.Data.StatusID = ((int)ScheduleStatus.Cancelled).ToString();
                }
                else if (TaskType == (int)TaskStatus.Revise)
                {
                    if (m_objMSchedulesDA.Data.StatusID == ((int)ScheduleStatus.Submitted).ToString())
                        m_objMSchedulesDA.Data.StatusID = ((int)ScheduleStatus.Draft).ToString();
                    else if (m_objMSchedulesDA.Data.StatusID == ((int)ScheduleStatus.Submitted_Reschedule).ToString())
                        m_objMSchedulesDA.Data.StatusID = ((int)ScheduleStatus.Draft_Reschedule).ToString();
                    else if (m_objMSchedulesDA.Data.StatusID == ((int)ScheduleStatus.Submitted_Cancellation).ToString())
                        m_objMSchedulesDA.Data.StatusID = ((int)ScheduleStatus.Draft_Cancellation).ToString();
                }

                Dictionary<string, List<object>> m_dicSet = new Dictionary<string, List<object>>();
                List<object> m_lstSet = new List<object>();
                m_lstSet.Add(typeof(int));
                m_lstSet.Add(m_objMSchedulesDA.Data.StatusID);
                 m_dicSet.Add(SchedulesVM.Prop.StatusID.Map, m_lstSet);

                m_objFilter = new Dictionary<string, List<object>>();
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(TaskID);
                m_objFilter.Add(SchedulesVM.Prop.TaskID.Map, m_lstFilter);

                m_objMSchedulesDA.UpdateBC(m_dicSet, m_objFilter,true, m_objDBConnection);
                if (!m_objMSchedulesDA.Success || m_objMSchedulesDA.Message != string.Empty)
                {
                    message = m_objMSchedulesDA.Message;
                    return false;
                }

                if (TaskType == (int)TaskStatus.Approved)
                    if (isMupltipleSchedule)
                    {

                        List<RecipientsVM> lsRecipient = new List<RecipientsVM>();
                        List<NotificationValuesVM> lsNotifValue = new List<NotificationValuesVM>();
                        List<NotificationAttachmentVM> lsNotifAttach = new List<NotificationAttachmentVM>();
                        Dictionary<string, List<NotificationValuesVM>> lsRow = new Dictionary<string, List<NotificationValuesVM>>();
                        List<string> strContentMail = new List<string>();
                        HtmlDocument htmlMain = new HtmlDocument();
                        MailNotificationsVM objMailNotificationsVM = new MailNotificationsVM();
                        List<string> lsMessage = new List<string>();
                        int i = 0;
                        lsMailNotificationsVM = lsMailNotificationsVM.OrderBy(d => DateTime.Parse(d.ScheduleStartDate)).ToList();
                        foreach (MailNotificationsVM mailnotif in lsMailNotificationsVM)
                        {
                            lsRecipient.AddRange(mailnotif.RecipientsVM);
                            lsNotifValue.AddRange(mailnotif.NotificationValuesVM);
                            lsRow.Add(mailnotif.MailNotificationID, lsNotifValue);
                            lsNotifAttach.AddRange(mailnotif.NotificationAttachmentVM);
                            strContentMail.Add(mailnotif.Contents);
                            if (i > 0)
                            {

                                var contentAppend = new HtmlDocument();
                                contentAppend.LoadHtml(strContentMail[i]);


                                //create table head for tender process
                                HtmlNode htmlThFunc = HtmlNode.CreateNode("<th 'text-align:center;width:120px;'>Agenda Tender</th>");
                                contentAppend.DocumentNode.SelectSingleNode("//thead/tr").PrependChild(htmlThFunc);

                                //create row tender process
                                HtmlNode htmlNodeTrAppend = HtmlNode.CreateNode($"<td style='text-align:center;width:120px;'><strong>{mailnotif.FunctionDesc}</strong></td>");
                                contentAppend.DocumentNode.SelectNodes("//tbody/tr")[1].PrependChild(htmlNodeTrAppend);

                                HtmlNode htmlNode = contentAppend.DocumentNode.SelectNodes("//tbody/tr")[1];
                                htmlMain.DocumentNode.SelectNodes("//tbody")[1].AppendChild(htmlNode);

                            }
                            else
                            {
                                htmlMain.LoadHtml(strContentMail[i]);

                                //create table head for tender process
                                HtmlNode htmlThFunc = HtmlNode.CreateNode("<th scope='col' style='width:120px'>Agenda Tender</th>");
                                htmlMain.DocumentNode.SelectSingleNode("//thead/tr").PrependChild(htmlThFunc);

                                //create row tender process
                                HtmlNode htmlNodeTr = HtmlNode.CreateNode($"<td style='text-align:center;width:120px;'><strong>{mailnotif.FunctionDesc}<strong></td>");
                                htmlMain.DocumentNode.SelectNodes("//tbody/tr")[1].PrependChild(htmlNodeTr);
                            }

                            i++;
                        }


                        HtmlNode htmlTable = htmlMain.DocumentNode
                                    .SelectNodes("//table")[1];

                        lsNotifValue.Add(new NotificationValuesVM { FieldTagID = "TableListSchedule", Value = htmlTable.OuterHtml.ToString() });

                        lsRecipient = lsRecipient.GroupBy(d => new { d.RecipientDesc, d.MailAddress, d.RecipientTypeID }).Select(d => new RecipientsVM { MailAddress = d.Key.MailAddress, RecipientDesc = d.Key.RecipientDesc, RecipientTypeID = d.Key.RecipientTypeID }).ToList();

                        objMailNotificationsVM.MailNotificationID = lsMailNotificationsVM.Select(d => d.MailNotificationID).FirstOrDefault();
                        objMailNotificationsVM.Subject = $"Rekapitulasi Jadwal Tender - {lsMailNotificationsVM.Select(d => DateTime.Parse(d.ScheduleStartDate).ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"))).FirstOrDefault()}";
                        objMailNotificationsVM.Contents = GenerateMultipleContent(null, "MULTIPLE_SCHEDULER", lsRecipient, lsNotifValue, ref message);
                        objMailNotificationsVM.RecipientsVM = lsRecipient;
                        objMailNotificationsVM.NotificationAttachmentVM = new List<NotificationAttachmentVM>();
                        if (lsNotifAttach.Count > 0)
                        {
                            objMailNotificationsVM.NotificationAttachmentVM = new List<NotificationAttachmentVM>
                            {
                                lsNotifAttach.FirstOrDefault()
                            };
                        }
                        objMailNotificationsVM.TaskID = lsMailNotificationsVM.Select(d => d.TaskID).FirstOrDefault();
                        bool sentSuccess = SendMail(objMailNotificationsVM, ref lsMessage, ref m_objDBConnection);
                        if (lsMessage.Count > 0)
                            message = string.Join(",", lsMessage);
                        else
                            retSuccess = true;

                        ListMailNotif.Clear();

                    }
                    else
                        if(message==string.Empty)retSuccess = true;

            }
            else
                message = m_objMMailNotifDA.Message;

            return retSuccess;
        }
        
        private string GetNotifID(string TaskID)
        {
            MMailNotificationsDA m_objMMailNotifDA = new MMailNotificationsDA();
            m_objMMailNotifDA.ConnectionStringName = Global.ConnStrConfigName;
            string m_strreturn = string.Empty;
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(MailNotificationsVM.Prop.MailNotificationID.MapAlias);


            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(TaskID);
            m_objFilter.Add(MailNotificationsVM.Prop.TaskID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMMailNotifDA = m_objMMailNotifDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null, null);
            if (m_objMMailNotifDA.Success && string.IsNullOrEmpty(m_objMMailNotifDA.Message))
            {
                DataRow m_drMMailNotifDA = m_dicMMailNotifDA[0].Tables[0].Rows[0];
                m_strreturn = m_drMMailNotifDA[MailNotificationsVM.Prop.MailNotificationID.Name].ToString();
            }
            else
            {
                return string.Empty;
            }

            return m_strreturn;
        }
        private void UpdateTMinuteEntries(string MailNotificationID, object m_objDBConnection, int Status, ref string message)
        {
            bool retSuccess = false;
            TMinuteEntriesDA m_objTMinuteEntriesDA = new TMinuteEntriesDA();
            m_objTMinuteEntriesDA.ConnectionStringName = Global.ConnStrConfigName;

            MMailNotificationsDA m_objMMailNotifDA = new MMailNotificationsDA();
            m_objMMailNotifDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(MinutesEntryVM.Prop.MinuteEntryID.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(MailNotificationID);
            m_objFilter.Add(MinutesEntryVM.Prop.MailNotificationID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicTMinuteEntriesDA = m_objTMinuteEntriesDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null, null);
            DataRow m_drTMinuteEntriesDA = m_dicTMinuteEntriesDA[0].Tables[0].Rows[0];

            m_objTMinuteEntriesDA.Data = new TMinuteEntries()
            {
                MinuteEntryID = m_drTMinuteEntriesDA[MinutesEntryVM.Prop.MinuteEntryID.Name].ToString()
            };

            m_objTMinuteEntriesDA.Select();
            m_objTMinuteEntriesDA.Data.StatusID = Status;
            m_objTMinuteEntriesDA.Update(true, m_objDBConnection);
            if (!m_objTMinuteEntriesDA.Success || m_objTMinuteEntriesDA.Message != string.Empty)
            {
                message = m_objTMinuteEntriesDA.Message;
            }
            /*m_objMMailNotifDA.Data = new MMailNotifications()
            {
                MailNotificationID = MailNotificationID
            };
            m_objMMailNotifDA.Select();
            m_objMMailNotifDA.Data.StatusID = (int)NotificationStatus.Fail;
            m_objMMailNotifDA.Update(true, m_objDBConnection);
            retSuccess = m_objMMailNotifDA.Success;

            if (!m_objMMailNotifDA.Success || m_objMMailNotifDA.Message != string.Empty)
            {
                message = m_objMMailNotifDA.Message;
            }*/
        }
        private void AfterSave(string m_strTaskTypeID, int TaskStatus, string TaskID, ref List<string> m_lstMessage, object m_objDBConnection = null)
        {

            if (m_strTaskTypeID == General.EnumDesc(TaskType.VendorWinner) && TaskStatus == 2)
            {
                List<FPTVendorWinnerVM> m_lstFPTVendorWinnerVM = new List<FPTVendorWinnerVM>();
                m_lstFPTVendorWinnerVM = getFPTVendorWinnerVM(TaskID);
                UpdateVendorWinner(m_lstFPTVendorWinnerVM, m_objDBConnection, ref m_lstMessage);
            }
            else if (m_strTaskTypeID == General.EnumDesc(TaskType.MailNotification) && TaskStatus == 2)
            {
                // bool m_sendsuccess = SendMail(m_MailNotificationsVM, ref m_lstMessage, ref m_objDBConnection);
            }
            else if (m_strTaskTypeID == General.EnumDesc(TaskType.NegotiationConfigurations) && TaskStatus == 2 && AutoEmailActive())
            {
                //CreateMailNotifNego(m_lstNegotiationConfigurationsVM, m_lstFPTVendorParticipantsVM, m_lstTCMembersVM, m_objFPTVM, ref m_lstMessage);
            }
            else if ((m_strTaskTypeID == General.EnumDesc(TaskType.Invitation_Schedules) ||
                     (m_strTaskTypeID == General.EnumDesc(TaskType.Invitation_Pretender))))
            {

                ////MailNotifID = this.Request.Params[MyTaskVM.Prop.MailNotificationID.Name];
                //if (TaskStatus == (int)Enum.TaskStatus.Approved)
                //{
                //    try
                //    {
                //        sentSuccess = UpdateMSchedule(this.Request.Params[MyTaskVM.Prop.ScheduleID.Name], TaskID, (int)Enum.TaskStatus.Approved, m_objDBConnection, ref message_, ref ListMailNotifID);
                //        if (!sentSuccess)
                //            m_lstMessage.Add("Update Schedule Error. " + "<br />Sending mail error");
                //    }
                //    catch (Exception e)
                //    {
                //        m_lstMessage.Add("Update Schedule Error. " + e.Message);
                //    }
                //}
                //else if (TaskStatus == (int)Enum.TaskStatus.Rejected)
                //{
                //    UpdateMSchedule(this.Request.Params[MyTaskVM.Prop.ScheduleID.Name], TaskID, (int)Enum.TaskStatus.Rejected, m_objDBConnection, ref message_, ref ListMailNotifID);
                //}
                //else if (TaskStatus == (int)Enum.TaskStatus.Revise)
                //{
                //    UpdateMSchedule(this.Request.Params[MyTaskVM.Prop.ScheduleID.Name], TaskID, (int)Enum.TaskStatus.Revise, m_objDBConnection, ref message_, ref ListMailNotifID);
                //}
            }
            else if (m_strTaskTypeID == General.EnumDesc(TaskType.MinutesOfMeeting))
            {
                //if (TaskStatus == (int)Enum.TaskStatus.Approved)
                //{
                //    //string m_strMailNotificationID = this.Request.Params[MyTaskVM.Prop.MailNotificationID.Name];
                //    UpdateTMinuteEntries(m_MailNotificationsVM.MailNotificationID, m_objDBConnection, (int)MinutesStatus.Approved, ref message_);
                //    if (message_ == string.Empty)
                //    {
                //        //m_objMTasksDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
                //        bool m_sendsuccess = SendMail(m_MailNotificationsVM, ref m_lstMessage, ref m_objDBConnection);
                //    }
                //}
                //else if (TaskStatus == (int)Enum.TaskStatus.Rejected)
                //{
                //    UpdateTMinuteEntries(m_MailNotificationsVM.MailNotificationID, m_objDBConnection, (int)MinutesStatus.Deleted, ref message_);
                //}
                //else if (TaskStatus == (int)Enum.TaskStatus.Revise)
                //{
                //    UpdateTMinuteEntries(m_MailNotificationsVM.MailNotificationID, m_objDBConnection, (int)MinutesStatus.Draft, ref message_);
                //}
            }
        }
        private void UpdateItemPrice(List<ItemUploadVM> lstItemUploadVM, List<ItemPriceUploadVM> lstItemPriceVM,List<ItemDetailUploadVM> lstItemDetailUploadVM,object m_objDBConnection, ref string message)
        {
            DItemPriceDA m_objDItemPriceDA = new DItemPriceDA();
            DItemPriceVendorDA m_objDItemPriceVendorDA = new DItemPriceVendorDA();
            DItemPriceVendorPeriodDA m_objDItemPriceVendorPeriodDA = new DItemPriceVendorPeriodDA();

            DItemVersionDA m_objDItemVersionDA = new DItemVersionDA();
            MItemDA m_objMItemDA = new MItemDA();
            DItemParameterDA m_objDItemParameterDA = new DItemParameterDA();

            DItemDetailsDA m_objDItemDetailsDA = new DItemDetailsDA();
            DItemDetails m_objDItemDetails = new DItemDetails();

            string m_strTransName = "Item";
            string m_strItemID = string.Empty;
            string m_strMessage = string.Empty;
            string m_ItemID = string.Empty;
            string m_RegionID = string.Empty;
            //object m_objDBConnection2 = null;

            m_objMItemDA.ConnectionStringName = Global.ConnStrConfigName;
            m_objDItemVersionDA.ConnectionStringName = Global.ConnStrConfigName;
            m_objDItemPriceDA.ConnectionStringName = Global.ConnStrConfigName;
            m_objDItemPriceVendorPeriodDA.ConnectionStringName = Global.ConnStrConfigName;
            m_objDItemPriceVendorDA.ConnectionStringName = Global.ConnStrConfigName;

            m_objDItemDetailsDA.ConnectionStringName = Global.ConnStrConfigName;

            //m_objDBConnection = m_objMItemDA.BeginTrans(m_strTransName);

            #region Items
            foreach (var objItemVM in lstItemUploadVM)
            {
                MItem m_objMItem = new MItem();
                m_objMItem.ItemID = objItemVM.ItemID;
                m_objMItemDA.Data = m_objMItem;
                m_objMItemDA.Select();

                m_objMItem.ItemDesc = objItemVM.ItemDesc;
                m_objMItem.ItemGroupID = objItemVM.ItemGroupID;
                m_objMItem.UoMID = objItemVM.UoMID;
                m_objMItem.IsActive = objItemVM.IsActive;
                m_objMItemDA.Data = m_objMItem;

                if (!m_objMItemDA.Success)
                    m_objMItemDA.InsertItemImport(true, m_objDBConnection);
                else
                    m_objMItemDA.Update(true, m_objDBConnection);

                if (!m_objMItemDA.Success || m_objMItemDA.Message != string.Empty)
                {
                    m_objMItemDA.EndTrans(ref m_objDBConnection, m_strTransName);
                    //return this.Direct(false, $"MItem: {m_objMItemDA.Message}, ItemID: {objItemVM.ItemID}, FK:ItemGroupID: {objItemVM.ItemGroupID}, FK:UoMID: {objItemVM.UoMID}");
                }

                #region DItemVersion
                
                DItemVersion m_objDItemVersion = new DItemVersion();

                m_objDItemVersion.ItemID = m_objMItemDA.Data.ItemID;
                m_objDItemVersion.Version = 1;
                m_objDItemVersionDA.Data = m_objDItemVersion;
                m_objDItemVersionDA.Select();

                m_objDItemVersion.VersionDesc = objItemVM.ItemDesc;

                if (m_objDItemVersionDA.Message != string.Empty || !m_objDItemVersionDA.Success)
                    m_objDItemVersionDA.Insert(true, m_objDBConnection);

                if (!m_objDItemVersionDA.Success || m_objDItemVersionDA.Message != string.Empty)
                {
                    m_objDItemVersionDA.EndTrans(ref m_objDBConnection, m_strTransName);
                    throw new Exception(m_objDItemVersionDA.Message);
                    //return this.Direct(false, $"DItemVersion: {m_objDItemVersionDA.Message}, ItemID: {objItemVM.ItemID}");
                }



                #endregion

            }



            #endregion

            #region Specifications


            foreach (ItemDetailUploadVM objItemDetailVM in lstItemDetailUploadVM)
            {
                #region Delete old data

                Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(objItemDetailVM.ItemID);
                m_objFilter.Add(ItemDetailVM.Prop.ItemID.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(objItemDetailVM.VendorID);
                m_objFilter.Add(ItemDetailVM.Prop.VendorID.Map, m_lstFilter);

                m_objDItemDetailsDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                #endregion


                Array m_objItemDetailTypes = System.Enum.GetValues(typeof(ItemDetailTypes));

                foreach (var strItemDetailType in m_objItemDetailTypes)
                {
                    #region DItemPriceUpload

                    m_objDItemDetails.ItemDetailID = Guid.NewGuid().ToString().Replace("-", "");
                    m_objDItemDetails.ItemID = objItemDetailVM.ItemID;
                    m_objDItemDetails.VendorID = objItemDetailVM.VendorID;
                    m_objDItemDetails.ItemDetailTypeID = General.EnumDesc((ItemDetailTypes)System.Enum.Parse(typeof(ItemDetailTypes), strItemDetailType.ToString()));

                    var propertyInfo = objItemDetailVM.GetType().GetProperty(strItemDetailType.ToString());
                    m_objDItemDetails.ItemDetailDesc = propertyInfo.GetValue(objItemDetailVM, null).ToString();
                    m_objDItemDetailsDA.Data = m_objDItemDetails;

                    m_objDItemDetailsDA.Insert(true, m_objDBConnection);

                    if (!m_objDItemDetailsDA.Success || m_objDItemDetailsDA.Message != string.Empty)
                    {
                        m_objDItemDetailsDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                        //m_lstMessage.Add(m_objDItemDetailsDA.Message);
                        //return this.Direct(false, m_objDItemDetailUploadDA.Message);
                    }
                    #endregion
                }


            }
            #endregion



            //if (m_objMItemDA.Success && m_objMItemDA.Message == string.Empty)
            //    m_objMItemDA.EndTrans(ref m_objDBConnection, true, m_strTransName);

            //m_objDBConnection2 = m_objDItemPriceDA.BeginTrans(m_strTransName);

            #region Price
            DItemPriceDA m_objDItemPriceDASelect;
            foreach (ItemPriceUploadVM objItemPriceVM in lstItemPriceVM)
            {
                m_ItemID = objItemPriceVM.ItemID;
                m_RegionID = objItemPriceVM.RegionID;

                List<ItemPriceVendorPeriodVM> m_lstdItemPriceVendorPeriodVM = GetListItemPriceVendorPeriod(objItemPriceVM.ItemPriceID ?? string.Empty, ref message, objItemPriceVM.VendorID);

                #region DItemPrice

                //check if exist
                m_objDItemPriceDASelect = new DItemPriceDA
                {
                    ConnectionStringName = Global.ConnStrConfigName
                };
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(ItemPriceVM.Prop.ItemPriceID.MapAlias);
                Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                List<object> m_lstFilter = new List<object>
                {
                    Operator.Equals,
                    objItemPriceVM.ItemID
                };
                m_objFilter.Add(ItemPriceVM.Prop.ItemID.Map, m_lstFilter);
                m_lstFilter = new List<object>
                {
                    Operator.Equals,
                    objItemPriceVM.RegionID
                };
                m_objFilter.Add(ItemPriceVM.Prop.RegionID.Map, m_lstFilter);
                Dictionary<int, DataSet> m_dicm_objDItemPriceDASelectDA = m_objDItemPriceDASelect.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);

                DItemPrice m_objDItemPrice = new DItemPrice
                {
                    ItemPriceID = objItemPriceVM.ItemPriceID,
                    ItemID = objItemPriceVM.ItemID,
                    RegionID = objItemPriceVM.RegionID
                };

                m_objDItemPriceDA.Data = m_objDItemPrice;
                m_objDItemPriceDA.Select();
                
                m_objDItemPrice.ItemID = objItemPriceVM.ItemID;
                m_objDItemPrice.PriceTypeID = objItemPriceVM.PriceTypeID;
                m_objDItemPrice.ProjectID = objItemPriceVM.ProjectID;
                m_objDItemPrice.RegionID = objItemPriceVM.RegionID;
                m_objDItemPrice.UnitTypeID = objItemPriceVM.UnitTypeID;
                m_objDItemPrice.ClusterID = objItemPriceVM.ClusterID;

                if (m_objDItemPriceDASelect.AffectedRows < 1)
                {
                    m_objDItemPrice.ItemPriceID = Guid.NewGuid().ToString().Replace("-", "");
                    m_objDItemPriceDA.Insert(true, m_objDBConnection);
                }
                else
                {
                    m_objDItemPrice.ItemPriceID = m_dicm_objDItemPriceDASelectDA[0].Tables[0].Rows[0][ItemPriceVM.Prop.ItemPriceID.Name].ToString().Trim();
                    m_objDItemPriceDA.Update(true, m_objDBConnection);
                }

                if (!m_objDItemPriceDA.Success || m_objDItemPriceDA.Message != string.Empty)
                {
                    m_objDItemPriceDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                    message = $"DItemPrice: {m_objDItemPriceDA.Message}, ItemID: {m_ItemID}, RegionID:{m_RegionID}";
                    throw new Exception(message);
                }

                string m_strItemPriceID = m_objDItemPriceDA.Data.ItemPriceID;
                #endregion

                #region DItemPriceVendor

                DItemPriceVendor m_objDItemPriceVendor = new DItemPriceVendor();

                m_objDItemPriceVendor.ItemPriceID = m_strItemPriceID;
                m_objDItemPriceVendor.VendorID = objItemPriceVM.VendorID;

                m_objDItemPriceVendorDA.Data = m_objDItemPriceVendor;
                m_objDItemPriceVendorDA.Select();

                m_objDItemPriceVendor.IsDefault = objItemPriceVM.IsDefault;

                if (!m_objDItemPriceVendorDA.Success || m_objDItemPriceVendorDA.Message != string.Empty)
                {
                    m_objDItemPriceVendor.ItemPriceID = m_strItemPriceID;
                    m_objDItemPriceVendorDA.Insert(true, m_objDBConnection);
                }
                else
                    m_objDItemPriceVendorDA.Update(true, m_objDBConnection);

                if (!m_objDItemPriceVendorDA.Success)
                {
                    m_objDItemPriceVendorDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                    message = $"DItemPriceVendor: {m_objDItemPriceVendorDA.Message}, ItemID: {m_ItemID}, RegionID:{m_RegionID}";
                    throw new Exception(message);
                }
                #endregion

                #region DItemPriceVendorPeriod
                DItemPriceVendorPeriod m_objDItemPriceVendorPeriod = new DItemPriceVendorPeriod();
                //List<ItemPriceVendorPeriodVM> m_lstdItemPriceVendorPeriodVM = GetListItemPriceVendorPeriod(m_strItemPriceID, objItemPriceVM.VendorID);

                if (m_lstdItemPriceVendorPeriodVM.Any())
                {
                    //validate duplicate data
                    if (!m_lstdItemPriceVendorPeriodVM.Any(d => d.ValidTo == objItemPriceVM.ValidTo && d.ValidFrom == objItemPriceVM.ValidFrom))
                    {
                        m_lstdItemPriceVendorPeriodVM = m_lstdItemPriceVendorPeriodVM.OrderBy(o => o.RowNo).ToList();

                        if (m_lstdItemPriceVendorPeriodVM.Where(d => d.ValidFrom < objItemPriceVM.ValidFrom).Any())
                        {
                            //insert new price
                            m_objDItemPriceVendorPeriod.ItemPriceID = m_strItemPriceID;
                            m_objDItemPriceVendorPeriod.VendorID = objItemPriceVM.VendorID;
                            m_objDItemPriceVendorPeriod.ValidFrom = objItemPriceVM.ValidFrom;
                            m_objDItemPriceVendorPeriod.ValidTo = objItemPriceVM.ValidTo;
                            m_objDItemPriceVendorPeriod.CurrencyID = objItemPriceVM.CurrencyID;
                            m_objDItemPriceVendorPeriod.Amount = objItemPriceVM.Amount;

                            m_objDItemPriceVendorPeriod.TaskID = objItemPriceVM.TaskID;
                            m_objDItemPriceVendorPeriodDA.Data = m_objDItemPriceVendorPeriod;

                            m_objDItemPriceVendorPeriodDA.Insert(true, m_objDBConnection);

                            if (!m_objDItemPriceVendorPeriodDA.Success)
                            {
                                m_objDItemPriceVendorPeriodDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                                message = $"DItemPriceVendorPeriod: {m_objDItemPriceVendorPeriodDA.Message}, ItemID: {m_ItemID}, RegionID:{m_RegionID}";
                                throw new Exception(message);
                            }

                            //update the previous data
                            ItemPriceVendorPeriodVM m_objdItemPriceVendorPeriodVM = m_lstdItemPriceVendorPeriodVM.Where(d => d.ValidFrom < objItemPriceVM.ValidFrom).LastOrDefault();
                            m_objDItemPriceVendorPeriod = new DItemPriceVendorPeriod();
                            if (m_objdItemPriceVendorPeriodVM != null)
                            {
                                m_objDItemPriceVendorPeriod.ItemPriceID = m_objdItemPriceVendorPeriodVM.ItemPriceID;
                                m_objDItemPriceVendorPeriod.VendorID = m_objdItemPriceVendorPeriodVM.VendorID;
                                m_objDItemPriceVendorPeriod.ValidFrom = m_objdItemPriceVendorPeriodVM.ValidFrom;
                                m_objDItemPriceVendorPeriodDA.Data = m_objDItemPriceVendorPeriod;
                                m_objDItemPriceVendorPeriodDA.Select();

                                m_objDItemPriceVendorPeriod.ValidTo = objItemPriceVM.ValidFrom.AddDays(-1);

                                m_objDItemPriceVendorPeriodDA.Update(true, m_objDBConnection);

                                if (!m_objDItemPriceVendorPeriodDA.Success)
                                {
                                    m_objDItemPriceVendorPeriodDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                                    message = $"DItemPriceVendorPeriod: {m_objDItemPriceVendorPeriodDA.Message}, ItemID: {m_ItemID}, RegionID:{m_RegionID}";
                                    throw new Exception(message);
                                }
                            }
                            //update the next data
                            m_objdItemPriceVendorPeriodVM = m_lstdItemPriceVendorPeriodVM.Where(d => d.ValidFrom > objItemPriceVM.ValidFrom).FirstOrDefault();
                            m_objDItemPriceVendorPeriod = new DItemPriceVendorPeriod();
                            if (m_objdItemPriceVendorPeriodVM != null)
                            {
                                if (m_objdItemPriceVendorPeriodVM.ValidTo < objItemPriceVM.ValidTo)
                                {
                                    m_objDItemPriceVendorPeriodDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                                    message = "Invalid date";
                                    throw new Exception(message);
                                }

                                m_objDItemPriceVendorPeriod.ItemPriceID = m_objdItemPriceVendorPeriodVM.ItemPriceID;
                                m_objDItemPriceVendorPeriod.VendorID = m_objdItemPriceVendorPeriodVM.VendorID;
                                m_objDItemPriceVendorPeriod.ValidFrom = m_objdItemPriceVendorPeriodVM.ValidFrom;
                                m_objDItemPriceVendorPeriodDA.Data = m_objDItemPriceVendorPeriod;
                                m_objDItemPriceVendorPeriodDA.Select();

                                m_objDItemPriceVendorPeriod.ValidFrom = objItemPriceVM.ValidTo.AddDays(1);

                                m_objDItemPriceVendorPeriodDA.Update(true, m_objDBConnection);

                                if (!m_objDItemPriceVendorPeriodDA.Success)
                                {
                                    m_objDItemPriceVendorPeriodDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                                    message = $"DItemPriceVendorPeriod: {m_objDItemPriceVendorPeriodDA.Message}, ItemID: {m_ItemID}, RegionID:{m_RegionID}";
                                    throw new Exception(message);
                                }
                            }

                        }
                    }
                }
                else
                {
                    m_objDItemPriceVendorPeriod = new DItemPriceVendorPeriod();

                    m_objDItemPriceVendorPeriod.ItemPriceID = m_strItemPriceID;
                    m_objDItemPriceVendorPeriod.VendorID = objItemPriceVM.VendorID;
                    m_objDItemPriceVendorPeriod.ValidFrom = objItemPriceVM.ValidFrom;

                    m_objDItemPriceVendorPeriodDA.Data = m_objDItemPriceVendorPeriod;
                    m_objDItemPriceVendorPeriodDA.Select();

                    m_objDItemPriceVendorPeriod.ValidTo = objItemPriceVM.ValidTo;
                    m_objDItemPriceVendorPeriod.CurrencyID = objItemPriceVM.CurrencyID;
                    m_objDItemPriceVendorPeriod.Amount = objItemPriceVM.Amount;
                    m_objDItemPriceVendorPeriod.TaskID = objItemPriceVM.TaskID;
                    m_objDItemPriceVendorPeriodDA.Insert(true, m_objDBConnection);

                    if (!m_objDItemPriceVendorPeriodDA.Success)
                    {
                        m_objDItemPriceVendorPeriodDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                        message = $"DItemPriceVendorPeriod: {m_objDItemPriceVendorPeriodDA.Message}, ItemID: {m_ItemID}, RegionID:{m_RegionID}";
                        throw new Exception(message);
                    }
                }


                #endregion


            }

            if (m_objDItemPriceDA.Success && m_objDItemPriceDA.Message == string.Empty)
                m_objDItemPriceDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
            #endregion

            

        }
        private List<ItemPriceVendorPeriodVM> GetListItemPriceVendorPeriod(string ItemPriceID,ref string message, string VendorID = "", string ValidFrom = "")
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
            m_lstSelect.Add(ConfigVM.Prop.Desc1.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc2.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc3.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ItemPriceID);
            m_objFilter.Add(ItemPriceVendorPeriodVM.Prop.ItemPriceID.Map, m_lstFilter);


            if (ValidFrom != string.Empty)
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(ValidFrom);
                m_objFilter.Add(ItemPriceVendorPeriodVM.Prop.ValidFrom.Map, m_lstFilter);
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
            if (m_objDItemPriceVendorPeriodDA.Success && m_objDItemPriceVendorPeriodDA.AffectedRows>0)
            {
                m_lstItemPriceVendorPeriodVM = (
                from DataRow m_drDItemPriceVendorPeriodDA in m_dicDItemPriceVendorPeriodDA[0].Tables[0].Rows
                select new ItemPriceVendorPeriodVM()
                {
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
            else
                message = m_objDItemPriceVendorPeriodDA.Message;

            return m_lstItemPriceVendorPeriodVM;

        }
        private List<SchedulesVM> GetSchedulesByTask(string TaskID) {
            MSchedulesDA m_objMScheduleDA = new MSchedulesDA();
            MSchedules m_objMSchedule = new MSchedules();
            m_objMScheduleDA.ConnectionStringName = Global.ConnStrConfigName;

            
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(SchedulesVM.Prop.ScheduleID.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.StartDate.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.EndDate.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.Subject.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.Location.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.TaskID.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.MailNotificationID.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.IsBatchMail.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(TaskID);
            m_objFilter.Add(SchedulesVM.Prop.TaskID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMSchedulesDA = m_objMScheduleDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            List<SchedulesVM> m_lstSchedulesVM = new List<SchedulesVM>();
            if (m_objMScheduleDA.Success && m_objMScheduleDA.Message == "")
            {
                m_lstSchedulesVM = (from DataRow dr in m_dicMSchedulesDA[0].Tables[0].Rows
                                    select new SchedulesVM
                                    {
                                        ScheduleID = dr[SchedulesVM.Prop.ScheduleID.Name].ToString(),
                                        MailNotificationID = dr[SchedulesVM.Prop.MailNotificationID.Name].ToString(),
                                        StartDate = DateTime.Parse(dr[SchedulesVM.Prop.StartDate.Name].ToString()),
                                        Location = dr[SchedulesVM.Prop.Location.Name].ToString(),
                                        Subject = dr[SchedulesVM.Prop.Subject.Name].ToString(),
                                        IsBatchMail = bool.Parse(dr[SchedulesVM.Prop.IsBatchMail.Name].ToString())
                                    }).ToList();
            }
            
            return m_lstSchedulesVM;
        }
        #endregion

        #region Public Method

        public Dictionary<int, List<TCMembersVM>> GeTTCMembersData(bool isCount, string TCMemberID, string TCMemberDesc)
        {
            int m_intCount = 0;
            List<TCMembersVM> m_lsTCMembersVM = new List<ViewModels.TCMembersVM>();
            Dictionary<int, List<TCMembersVM>> m_dicReturn = new Dictionary<int, List<TCMembersVM>>();
            TTCMembersDA m_objTTCMembersDA = new TTCMembersDA();
            m_objTTCMembersDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(TCMembersVM.Prop.TCMemberID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.EmployeeName.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(TCMemberID);
            m_objFilter.Add(TCMembersVM.Prop.TCMemberID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(TCMemberDesc);
            m_objFilter.Add(TCMembersVM.Prop.EmployeeName.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicTTCMembersDA = m_objTTCMembersDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objTTCMembersDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpTCMemberBL in m_dicTTCMembersDA)
                    {
                        m_intCount = m_kvpTCMemberBL.Key;
                        break;
                    }
                else
                {
                    m_lsTCMembersVM = (
                        from DataRow m_drTTCMembersDA in m_dicTTCMembersDA[0].Tables[0].Rows
                        select new TCMembersVM()
                        {
                            TCMemberID = m_drTTCMembersDA[TCMembersVM.Prop.TCMemberID.Name].ToString(),
                            EmployeeName = m_drTTCMembersDA[TCMembersVM.Prop.EmployeeName.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lsTCMembersVM);
            return m_dicReturn;
        }

        #endregion
    }
}