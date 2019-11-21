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

namespace com.SML.BIGTRONS.Controllers
{
    public class MailNotificationsController : BaseController
    {
        private readonly string title = "List of Mail Notifications";
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
            MMailNotificationsDA m_objMMailNotificationsDA = new MMailNotificationsDA();
            m_objMMailNotificationsDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcCApprovalPath = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();


            foreach (FilterHeaderCondition m_fhcFilter in m_fhcCApprovalPath.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);



                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = MailNotificationsVM.Prop.Map(m_strDataIndex, false);

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
            //filter function
            string msg = string.Empty;
            List<string> m_listRole = GetAllRoleID(ref msg);
            List<RoleFunctionVM> m_lstRoleMenuActionVM = GetListRoleFunctionVM(m_listRole, ref msg);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(string.Join(",", m_lstRoleMenuActionVM.Select(x => x.FunctionID)));
            m_objFilter.Add(MailNotificationsVM.Prop.FunctionID.Map, m_lstFilter);



            Dictionary<int, DataSet> m_dicMMailNotificationsDA = m_objMMailNotificationsDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpApprovalPathBL in m_dicMMailNotificationsDA)
            {
                m_intCount = m_kvpApprovalPathBL.Key;
                break;
            }

            List<MailNotificationsVM> m_lstMailNotificationsVM = new List<MailNotificationsVM>();

            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(MailNotificationsVM.Prop.MailNotificationID.MapAlias);
                m_lstSelect.Add(MailNotificationsVM.Prop.FunctionDesc.MapAlias);
                m_lstSelect.Add(MailNotificationsVM.Prop.FunctionID.MapAlias);
                m_lstSelect.Add(MailNotificationsVM.Prop.Importance.MapAlias);
                m_lstSelect.Add(MailNotificationsVM.Prop.Subject.MapAlias);
                //m_lstSelect.Add(MailNotificationsVM.Prop.Contents.MapAlias);
                m_lstSelect.Add(MailNotificationsVM.Prop.StatusID.MapAlias);
                m_lstSelect.Add(MailNotificationsVM.Prop.TaskID.MapAlias);
                m_lstSelect.Add(MailNotificationsVM.Prop.FPTID.MapAlias);
                m_lstSelect.Add(MailNotificationsVM.Prop.CreatedDate.MapAlias);
                m_lstSelect.Add(MailNotificationsVM.Prop.TaskStatusDesc.MapAlias);
                m_lstSelect.Add(MailNotificationsVM.Prop.StatusDesc.MapAlias);
                m_lstSelect.Add(MailNotificationsVM.Prop.TaskStatusID.MapAlias);


                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(MailNotificationsVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMMailNotificationsDA = m_objMMailNotificationsDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMMailNotificationsDA.Message == string.Empty)
                {
                    m_lstMailNotificationsVM = (
                        from DataRow m_drMMailNotificationsDA in m_dicMMailNotificationsDA[0].Tables[0].Rows
                        select new MailNotificationsVM()
                        {
                            MailNotificationID = m_drMMailNotificationsDA[MailNotificationsVM.Prop.MailNotificationID.Name].ToString(),
                            FunctionDesc = m_drMMailNotificationsDA[MailNotificationsVM.Prop.FunctionDesc.Name].ToString(),
                            FunctionID = m_drMMailNotificationsDA[MailNotificationsVM.Prop.FunctionID.Name].ToString(),
                            Importance = (bool)m_drMMailNotificationsDA[MailNotificationsVM.Prop.Importance.Name],
                            Subject = m_drMMailNotificationsDA[MailNotificationsVM.Prop.Subject.Name].ToString(),
                            //Contents = m_drMMailNotificationsDA[MailNotificationsVM.Prop.Contents.Name].ToString(),
                            StatusID = (int)m_drMMailNotificationsDA[MailNotificationsVM.Prop.StatusID.Name],
                            TaskStatusID = string.IsNullOrEmpty(m_drMMailNotificationsDA[MailNotificationsVM.Prop.TaskStatusID.Name].ToString()) ? null : (int?)m_drMMailNotificationsDA[MailNotificationsVM.Prop.TaskStatusID.Name],
                            TaskID = m_drMMailNotificationsDA[MailNotificationsVM.Prop.TaskID.Name].ToString(),
                            FPTID = m_drMMailNotificationsDA[MailNotificationsVM.Prop.FPTID.Name].ToString(),
                            CreatedDate = !string.IsNullOrEmpty(m_drMMailNotificationsDA[MailNotificationsVM.Prop.CreatedDate.Name].ToString()) ? (DateTime?)m_drMMailNotificationsDA[MailNotificationsVM.Prop.CreatedDate.Name] : null,
                            TaskStatusDesc = m_drMMailNotificationsDA[MailNotificationsVM.Prop.TaskStatusDesc.Name].ToString(),
                            StatusDesc = m_drMMailNotificationsDA[MailNotificationsVM.Prop.StatusDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }

            return this.Store(m_lstMailNotificationsVM, m_intCount);
        }
        public ActionResult Home()
        {
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 0;
            return this.Direct();
        }
        public ActionResult PageOne()
        {
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return this.Direct();
        }
        public ActionResult Add(string Caller)
        {
            //Global.HasAccess = GetHasAccess();
            //if (!Global.HasAccess.Add)
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            MailNotificationsVM m_objMailNotificationsVM = new MailNotificationsVM();
            m_objMailNotificationsVM.RecipientsVM = new List<RecipientsVM>();

            ViewDataDictionary m_vddMailNotifications = new ViewDataDictionary();
            m_vddMailNotifications.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddMailNotifications.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
            if (Caller == General.EnumDesc(Buttons.ButtonDetail))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                //Dictionary<string, object> m_dicSelectedRow = GetFormData(m_nvcParams);
                //Session[dataSessionName] = JSON.Serialize(m_dicSelectedRow);
            }

            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objMailNotificationsVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddMailNotifications,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        [ValidateInput(false)]
        public ActionResult Detail(string Caller, string Selected, string MailNotificationID)
        {
            //Global.HasAccess = GetHasAccess();
            //if (!Global.HasAccess.Read)
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));


            string m_strMessage = string.Empty;
            MailNotificationsVM m_objMailNotificationsVM = new MailNotificationsVM();
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
                m_dicSelectedRow = GetFormData(m_nvcParams, MailNotificationID);
            }
            else
                m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);

            if (m_dicSelectedRow.Count > 0)
                m_objMailNotificationsVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddMailNotifications = new ViewDataDictionary();
            m_vddMailNotifications.Add("StatusData", m_objMailNotificationsVM.StatusDesc);
            m_vddMailNotifications.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;

            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objMailNotificationsVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddMailNotifications,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        [ValidateInput(false)]
        public ActionResult Update(string Caller, string Selected)
        {
            //Global.HasAccess = GetHasAccess();
            //if (!Global.HasAccess.Update)
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            string m_strMessage = string.Empty;
            MailNotificationsVM m_objMailNotificationsVM = new MailNotificationsVM();
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

            if (m_dicSelectedRow.Count > 0)
                m_objMailNotificationsVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddMailNotifications = new ViewDataDictionary();
            m_vddMailNotifications.Add("StatusData", m_objMailNotificationsVM.StatusDesc);
            m_vddMailNotifications.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddMailNotifications.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;

            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objMailNotificationsVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddMailNotifications,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Preview(string MailNotificationID)
        {
            if (string.IsNullOrEmpty(MailNotificationID))
            {
                return null;
            }
            MailNotificationsVM m_MailNotificationsVM = getMailNotificationsVM(MailNotificationID);
            m_MailNotificationsVM.RecipientsTO = string.Join(";", m_MailNotificationsVM.RecipientsVM.Where(x => x.RecipientTypeID == ((int)Enum.RecipientTypes.TO).ToString()).Select(y => y.MailAddress));
            m_MailNotificationsVM.RecipientsCC = string.Join(";", m_MailNotificationsVM.RecipientsVM.Where(x => x.RecipientTypeID == ((int)Enum.RecipientTypes.CC).ToString()).Select(y => y.MailAddress));
            m_MailNotificationsVM.RecipientsBCC = string.Join(";", m_MailNotificationsVM.RecipientsVM.Where(x => x.RecipientTypeID == ((int)Enum.RecipientTypes.BCC).ToString()).Select(y => y.MailAddress));
            m_MailNotificationsVM.NotificationAttachmentName = string.Join(",", m_MailNotificationsVM.NotificationAttachmentVM.Select(x => x.Filename).ToList());

            return View("Preview", m_MailNotificationsVM);
        }



        [ValidateInput(false)]
        public ActionResult Save(string Action)
        {
            //if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
            //    : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            string m_MailNotificationID = Request.Params["MailNotificationID"].ToString();
            MailNotificationsVM m_MailNotificationsVM = getMailNotificationsVM(m_MailNotificationID);
            NotificationTemplateVM m_NotificationTemplateVM = GetNotificationTemplateVM(m_MailNotificationsVM.NotificationMapVM.NotificationTemplateID);
            var m_listTag = new Dictionary<string, string>();
            //foreach (var item in m_MailNotificationsVM.NotificationValuesVM)
            //{
            //    m_listTag.Add(item.FieldTagID, item.Value);
            //}

            //RecipientsVM
            List<RecipientsVM> m_lstRecipientsVM = new List<RecipientsVM>();
            if (this.Request.Params["ListRecipientTOVM"] != null)
            {
                Dictionary<string, object>[] m_arrListRecipient = JSON.Deserialize<Dictionary<string, object>[]>(this.Request.Params["ListRecipientTOVM"]);
                if (m_arrListRecipient == null)
                    m_arrListRecipient = new List<Dictionary<string, object>>().ToArray();
                foreach (Dictionary<string, object> m_dicRecipientSchedule in m_arrListRecipient)
                {
                    RecipientsVM objRecipient = new RecipientsVM();
                    objRecipient.RecipientID = Guid.NewGuid().ToString().Replace("-", "");
                    objRecipient.OwnerID = m_dicRecipientSchedule[RecipientsVM.Prop.OwnerID.Name].ToString();
                    objRecipient.RecipientDesc = m_dicRecipientSchedule[RecipientsVM.Prop.RecipientDesc.Name].ToString();
                    objRecipient.MailAddress = m_dicRecipientSchedule[RecipientsVM.Prop.MailAddress.Name].ToString();
                    objRecipient.RecipientTypeID = m_dicRecipientSchedule[RecipientsVM.Prop.RecipientTypeID.Name].ToString();
                    m_lstRecipientsVM.Add(objRecipient);
                }
            }
            if (this.Request.Params["ListRecipientCCVM"] != null)
            {
                Dictionary<string, object>[] m_arrListRecipient = JSON.Deserialize<Dictionary<string, object>[]>(this.Request.Params["ListRecipientCCVM"]);
                if (m_arrListRecipient == null)
                    m_arrListRecipient = new List<Dictionary<string, object>>().ToArray();
                foreach (Dictionary<string, object> m_dicRecipientSchedule in m_arrListRecipient)
                {
                    RecipientsVM objRecipient = new RecipientsVM();
                    objRecipient.RecipientID = Guid.NewGuid().ToString().Replace("-", "");
                    objRecipient.OwnerID = m_dicRecipientSchedule[RecipientsVM.Prop.OwnerID.Name].ToString();
                    objRecipient.RecipientDesc = m_dicRecipientSchedule[RecipientsVM.Prop.RecipientDesc.Name].ToString();
                    objRecipient.MailAddress = m_dicRecipientSchedule[RecipientsVM.Prop.MailAddress.Name].ToString();
                    objRecipient.RecipientTypeID = m_dicRecipientSchedule[RecipientsVM.Prop.RecipientTypeID.Name].ToString();
                    m_lstRecipientsVM.Add(objRecipient);
                }
            }
            if (this.Request.Params["ListRecipientBCCVM"] != null)
            {
                Dictionary<string, object>[] m_arrListRecipient = JSON.Deserialize<Dictionary<string, object>[]>(this.Request.Params["ListRecipientBCCVM"]);
                if (m_arrListRecipient == null)
                    m_arrListRecipient = new List<Dictionary<string, object>>().ToArray();
                foreach (Dictionary<string, object> m_dicRecipientSchedule in m_arrListRecipient)
                {
                    RecipientsVM objRecipient = new RecipientsVM();
                    objRecipient.RecipientID = Guid.NewGuid().ToString().Replace("-", "");
                    objRecipient.OwnerID = m_dicRecipientSchedule[RecipientsVM.Prop.OwnerID.Name].ToString();
                    objRecipient.RecipientDesc = m_dicRecipientSchedule[RecipientsVM.Prop.RecipientDesc.Name].ToString();
                    objRecipient.MailAddress = m_dicRecipientSchedule[RecipientsVM.Prop.MailAddress.Name].ToString();
                    objRecipient.RecipientTypeID = m_dicRecipientSchedule[RecipientsVM.Prop.RecipientTypeID.Name].ToString();
                    m_lstRecipientsVM.Add(objRecipient);
                }
            }

            //TemplateTagsVM
            List<TemplateTagsVM> m_lstTemplateTagsVM = new List<TemplateTagsVM>();
            if (this.Request.Params["ListTemplateTagsVM"] != null)
            {
                Dictionary<string, object>[] m_arrListRecipient = JSON.Deserialize<Dictionary<string, object>[]>(this.Request.Params["ListTemplateTagsVM"]);
                if (m_arrListRecipient == null)
                    m_arrListRecipient = new List<Dictionary<string, object>>().ToArray();
                foreach (Dictionary<string, object> m_dicRecipientSchedule in m_arrListRecipient)
                {
                    TemplateTagsVM m_TemplateTagsVM = new TemplateTagsVM();
                    m_TemplateTagsVM.TemplateTagID = Guid.NewGuid().ToString().Replace("-", "");
                    m_TemplateTagsVM.TemplateID = m_dicRecipientSchedule[TemplateTagsVM.Prop.TemplateID.Name].ToString();
                    m_TemplateTagsVM.FieldTagID = m_dicRecipientSchedule[TemplateTagsVM.Prop.FieldTagID.Name].ToString();
                    m_TemplateTagsVM.TagDesc = m_dicRecipientSchedule[TemplateTagsVM.Prop.TagDesc.Name].ToString();
                    m_TemplateTagsVM.TemplateType = m_dicRecipientSchedule[TemplateTagsVM.Prop.TemplateType.Name].ToString();
                    m_TemplateTagsVM.Value = m_dicRecipientSchedule["Value"].ToString();
                    m_lstTemplateTagsVM.Add(m_TemplateTagsVM);
                }
            }
            //NotificationAttachmentVM
            List<NotificationAttachmentVM> m_lstNotificationAttachmentVM = new List<NotificationAttachmentVM>();
            if (this.Request.Params["ListNotificationAttachmentVM"] != null)
            {
                Dictionary<string, object>[] m_arrListRecipient = JSON.Deserialize<Dictionary<string, object>[]>(this.Request.Params["ListNotificationAttachmentVM"]);
                if (m_arrListRecipient == null)
                    m_arrListRecipient = new List<Dictionary<string, object>>().ToArray();
                foreach (Dictionary<string, object> m_dicRecipientSchedule in m_arrListRecipient)
                {
                    NotificationAttachmentVM m_NotificationAttachmentVM = new NotificationAttachmentVM();
                    m_NotificationAttachmentVM.AttachmentValueID = Guid.NewGuid().ToString().Replace("-", "");
                    m_NotificationAttachmentVM.No = m_dicRecipientSchedule["No"].ToString();
                    m_NotificationAttachmentVM.Filename = m_dicRecipientSchedule[NotificationAttachmentVM.Prop.Filename.Name].ToString();
                    m_NotificationAttachmentVM.ContentType = m_dicRecipientSchedule[NotificationAttachmentVM.Prop.ContentType.Name].ToString();
                    m_NotificationAttachmentVM.RawData = m_dicRecipientSchedule[NotificationAttachmentVM.Prop.RawData.Name].ToString();
                    m_NotificationAttachmentVM.MailNotificationID = m_dicRecipientSchedule[NotificationAttachmentVM.Prop.MailNotificationID.Name].ToString();
                    m_lstNotificationAttachmentVM.Add(m_NotificationAttachmentVM);
                }
            }

            List<string> m_lstMessage = new List<string>();
            DRecipientsDA m_objDRecipientsDA = new DRecipientsDA();
            TNotificationValuesDA m_objTNotificationValuesDA = new TNotificationValuesDA();
            TNotificationAttachmentsDA m_objTNotificationAttachmentsDA = new TNotificationAttachmentsDA();
            MMailNotificationsDA m_objMMailNotificationsDA = new MMailNotificationsDA();
            m_objDRecipientsDA.ConnectionStringName = Global.ConnStrConfigName;
            string m_strTransName = "DRecipients";
            object m_objDBConnection = null;
            m_objDBConnection = m_objDRecipientsDA.BeginTrans(m_strTransName);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            try
            {
                #region RecipientsVM
                m_objDRecipientsDA.ConnectionStringName = Global.ConnStrConfigName;
                DRecipients m_objDRecipients = new DRecipients();

                //DELETE RecipientsVM
                m_objFilter = new Dictionary<string, List<object>>();
                m_lstFilter = new List<object>();

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(m_MailNotificationID);
                m_objFilter.Add(RecipientsVM.Prop.MailNotificationID.Map, m_lstFilter);

                m_objDRecipientsDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                if (m_objDRecipientsDA.Message == General.EnumDesc(MessageLib.NotExist)) m_objDRecipientsDA.Message = "";
                if (!m_objDRecipientsDA.Success || m_objDRecipientsDA.Message != string.Empty)
                {
                    m_objDRecipientsDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                    return this.Direct(false, m_objDRecipientsDA.Message);
                }

                foreach (var item in m_lstRecipientsVM)
                {
                    m_objDRecipients = new DRecipients();
                    m_objDRecipients.RecipientID = Guid.NewGuid().ToString("N");
                    m_objDRecipients.RecipientDesc = item.RecipientDesc;
                    m_objDRecipients.MailAddress = item.MailAddress;
                    m_objDRecipients.OwnerID = item.OwnerID;
                    m_objDRecipients.RecipientTypeID = item.RecipientTypeID;
                    m_objDRecipients.MailNotificationID = m_MailNotificationID;

                    m_objDRecipientsDA.Data = m_objDRecipients;
                    m_objDRecipientsDA.Insert(true, m_objDBConnection);
                    if (!m_objDRecipientsDA.Success || m_objDRecipientsDA.Message != string.Empty)
                    {
                        m_objDRecipientsDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                        return this.Direct(false, m_objDRecipientsDA.Message);
                    }
                }


                #endregion
                #region TNotificationValues
                m_objTNotificationValuesDA.ConnectionStringName = Global.ConnStrConfigName;
                TNotificationValues m_objTNotificationValues = new TNotificationValues();

                //DELETE TNotificationValues
                m_objFilter = new Dictionary<string, List<object>>();
                m_lstFilter = new List<object>();

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(m_MailNotificationID);
                m_objFilter.Add(NotificationValuesVM.Prop.MailNotificationID.Map, m_lstFilter);

                m_objTNotificationValuesDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                if (m_objTNotificationValuesDA.Message == General.EnumDesc(MessageLib.NotExist)) m_objTNotificationValuesDA.Message = "";
                if (!m_objTNotificationValuesDA.Success || m_objTNotificationValuesDA.Message != string.Empty)
                {
                    m_objTNotificationValuesDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                    return this.Direct(false, m_objTNotificationValuesDA.Message);
                }


                foreach (var item in m_lstTemplateTagsVM)
                {
                    m_objTNotificationValues = new TNotificationValues();
                    m_objTNotificationValues.NotificationValueID = Guid.NewGuid().ToString("N");
                    m_objTNotificationValues.Value = item.Value;
                    m_objTNotificationValues.MailNotificationID = m_MailNotificationID;
                    m_objTNotificationValues.FieldTagID = item.FieldTagID;

                    m_objTNotificationValuesDA.Data = m_objTNotificationValues;
                    m_objTNotificationValuesDA.Insert(true, m_objDBConnection);
                    if (!m_objTNotificationValuesDA.Success || m_objTNotificationValuesDA.Message != string.Empty)
                    {
                        m_objTNotificationValuesDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                        return this.Direct(false, m_objTNotificationValuesDA.Message);
                    }
                    m_listTag.Add(item.FieldTagID, item.Value);
                }
                #endregion
                #region NotificationAttachmentVM
                m_objTNotificationAttachmentsDA.ConnectionStringName = Global.ConnStrConfigName;
                TNotificationAttachments m_objTNotificationAttachments = new TNotificationAttachments();

                //DELETE NotificationAttachmentVM
                m_objFilter = new Dictionary<string, List<object>>();
                m_lstFilter = new List<object>();

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(m_MailNotificationID);//TODO: filter in
                m_objFilter.Add(NotificationAttachmentVM.Prop.MailNotificationID.Map, m_lstFilter);

                m_objTNotificationAttachmentsDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                if (m_objTNotificationAttachmentsDA.Message == General.EnumDesc(MessageLib.NotExist)) m_objTNotificationAttachmentsDA.Message = "";
                if (!m_objTNotificationAttachmentsDA.Success || m_objTNotificationAttachmentsDA.Message != string.Empty)
                {
                    m_objTNotificationAttachmentsDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                    return this.Direct(false, m_objTNotificationAttachmentsDA.Message);
                }

                foreach (var item in m_lstNotificationAttachmentVM)
                {
                    m_objTNotificationAttachments = new TNotificationAttachments();
                    m_objTNotificationAttachments.AttachmentValueID = "";
                    m_objTNotificationAttachments.Filename = item.Filename;
                    m_objTNotificationAttachments.ContentType = item.ContentType;
                    m_objTNotificationAttachments.RawData = item.RawData;
                    m_objTNotificationAttachments.MailNotificationID = m_MailNotificationID;

                    m_objTNotificationAttachmentsDA.Data = m_objTNotificationAttachments;
                    m_objTNotificationAttachmentsDA.Insert(true, m_objDBConnection);
                    if (!m_objTNotificationAttachmentsDA.Success || m_objTNotificationAttachmentsDA.Message != string.Empty)
                    {
                        m_objTNotificationAttachmentsDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                        return this.Direct(false, m_objTNotificationAttachmentsDA.Message);
                    }
                }
                #endregion
                #region MMailNotifications
                m_objMMailNotificationsDA.ConnectionStringName = Global.ConnStrConfigName;
                MMailNotifications m_objMMailNotifications = new MMailNotifications();
                m_objMMailNotifications.MailNotificationID = m_MailNotificationID;
                m_objMMailNotificationsDA.Data = m_objMMailNotifications;
                m_objMMailNotificationsDA.Select();
                m_objMMailNotifications.Contents = Global.ParseParameter(m_NotificationTemplateVM.Contents, m_listTag);
                m_objMMailNotificationsDA.Update(true, m_objDBConnection);
                if (!m_objMMailNotificationsDA.Success || m_objMMailNotificationsDA.Message != string.Empty)
                {
                    m_objMMailNotificationsDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                    return this.Direct(false, m_objMMailNotificationsDA.Message);
                }

                #endregion

                if (!m_objDRecipientsDA.Success || m_objDRecipientsDA.Message != string.Empty)
                    m_lstMessage.Add(m_objDRecipientsDA.Message);
                else
                    m_objDRecipientsDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                m_objDRecipientsDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
            }
            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
                return Detail(General.EnumDesc(Buttons.ButtonSave), null, m_MailNotificationID);

            }
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);
        }
        public ActionResult Delete(string Selected)
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<MailNotificationsVM> m_lstSelectedRow = new List<MailNotificationsVM>();
            m_lstSelectedRow = JSON.Deserialize<List<MailNotificationsVM>>(Selected);

            MMailNotificationsDA m_objMMailNotificationsDA = new MMailNotificationsDA();
            m_objMMailNotificationsDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (MailNotificationsVM m_objMailNotificationsVM in m_lstSelectedRow)
                {
                    MMailNotifications m_objMMailNotifications = new MMailNotifications();


                    m_objMMailNotifications.MailNotificationID = m_objMailNotificationsVM.MailNotificationID;
                    m_objMMailNotificationsDA.Data = m_objMMailNotifications;
                    m_objMMailNotificationsDA.Select();
                    m_objMMailNotifications.StatusID = (int)NotificationStatus.Deleted;

                    m_objMMailNotificationsDA.Update(false);
                    if (!m_objMMailNotificationsDA.Success || m_objMMailNotificationsDA.Message != string.Empty)
                    {
                        m_lstMessage.Add(m_objMMailNotificationsDA.Message);
                    }


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
        public ActionResult ReadAttachment()
        {
            var m_lstNotificationAttachmentVM = new List<NotificationAttachmentVM>();

            for (int i = 0; i < Request.Files.Count; i++)
            {
                var m_NotificationAttachmentVM = new NotificationAttachmentVM();
                //if (Request.Files[i].ContentLength > 500000)
                //{
                //    Global.ShowErrorAlert("Error Upload", "Maximum file size is 500KB");
                //    return this.Direct();
                //}
                byte[] fileData = null;
                using (var binaryReader = new BinaryReader(Request.Files[i].InputStream))
                    fileData = binaryReader.ReadBytes(Request.Files[i].ContentLength);
                m_NotificationAttachmentVM.Filename = Path.GetFileName(Server.MapPath(Request.Files[0].FileName));
                m_NotificationAttachmentVM.ContentType = Path.GetExtension(m_NotificationAttachmentVM.Filename);
                m_NotificationAttachmentVM.RawData = Convert.ToBase64String(fileData);
                m_lstNotificationAttachmentVM.Add(m_NotificationAttachmentVM);
            }
            object m_return = m_lstNotificationAttachmentVM.FirstOrDefault();
            return this.Store(m_lstNotificationAttachmentVM.FirstOrDefault());
        }
        [ValidateInput(false)]
        public ActionResult Verify(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Verify)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            List<MailNotificationsVM> m_lstSelectedRow = new List<MailNotificationsVM>();
            m_lstSelectedRow = JSON.Deserialize<List<MailNotificationsVM>>(Selected);
            //DFPTVendorWinnerDA m_objDFPTVendorWinnerDA = new DFPTVendorWinnerDA();
            MMailNotificationsDA m_MMailNotificationsDA = new MMailNotificationsDA();
            MTasksDA m_objMTasksDA = new MTasksDA();
            DTaskDetailsDA m_objDTaskDetailsDA = new DTaskDetailsDA();
            //todo: load data
            //List<FPTVendorWinnerVM> m_lstFPTVendorWinnerVM = getFPTVendorWinnerVM(m_lstSelectedRow.FirstOrDefault().FPTID);

            m_objMTasksDA.ConnectionStringName = Global.ConnStrConfigName;
            string m_strTransName = "MTasks";
            object m_objDBConnection = null;
            m_objDBConnection = m_objMTasksDA.BeginTrans(m_strTransName);
            string m_strMailNotificationID = string.Empty;

            //insert project & BP upload
            try
            {
                m_strMailNotificationID = m_lstSelectedRow.FirstOrDefault().MailNotificationID;
                string m_strTaskID = "";
                string m_strowner = "";
                string CurrentApprovalRole = GetCurrentApproval(General.EnumDesc(Enum.TaskType.MailNotification), 0);
                //Insert MTasks
                MTasks m_objMTasks = new MTasks()
                {
                    TaskTypeID = General.EnumDesc(TaskType.MailNotification),
                    TaskDateTimeStamp = DateTime.Now,
                    TaskOwnerID = GetParentApproval(ref m_strowner, CurrentApprovalRole, General.EnumDesc(Enum.TaskType.MailNotification)),
                    StatusID = 0,
                    CurrentApprovalLvl = 1,
                    Remarks = "Mail Notification",
                    Summary = ""
                };

                if (!string.IsNullOrEmpty(m_strowner)) m_lstMessage.Add(m_strowner);

                m_objMTasksDA.Data = m_objMTasks;
                m_objMTasksDA.Insert(true, m_objDBConnection);
                if (!m_objMTasksDA.Success || m_objMTasksDA.Message != string.Empty)
                {
                    m_objMTasksDA.EndTrans(ref m_objDBConnection, m_strTransName);
                    return this.Direct(false, m_objMTasksDA.Message);
                }
                m_strTaskID = m_objMTasksDA.Data.TaskID;
                //Insert DTaskDetails
                m_objDTaskDetailsDA.ConnectionStringName = Global.ConnStrConfigName;
                DTaskDetails m_objDTaskDetails = new DTaskDetails()
                {
                    TaskDetailID = Guid.NewGuid().ToString().Replace("-", ""),
                    TaskID = m_strTaskID,
                    StatusID = 5,
                    Remarks = "Mail Notification",

                };
                m_objDTaskDetailsDA.Data = m_objDTaskDetails;
                //m_objDTaskDetailsDA.Select();
                m_objDTaskDetailsDA.Insert(true, m_objDBConnection);
                if (!m_objDTaskDetailsDA.Success || m_objDTaskDetailsDA.Message != string.Empty)
                {
                    m_objDTaskDetailsDA.EndTrans(ref m_objDBConnection, m_strTransName);
                    return this.Direct(false, m_objDTaskDetailsDA.Message);
                }
                //Update MailNotif status
                Dictionary<string, List<object>> m_dicSet = new Dictionary<string, List<object>>();
                Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(m_strMailNotificationID);
                m_objFilter.Add(MailNotificationsVM.Prop.MailNotificationID.Map, m_lstFilter);

                List<object> m_lstSet = new List<object>();
                m_lstSet.Add(typeof(string));
                m_lstSet.Add(m_strTaskID);
                m_dicSet.Add(MailNotificationsVM.Prop.TaskID.Map, m_lstSet);
                m_MMailNotificationsDA.UpdateBC(m_dicSet, m_objFilter, true, m_objDBConnection);
                if (!m_MMailNotificationsDA.Success || m_objDTaskDetailsDA.Message != string.Empty)
                {
                    m_objMTasksDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                    return this.Direct(false, m_MMailNotificationsDA.Message);
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                return this.Direct(false, ex.Message);
            }
            if (m_lstMessage.Count <= 0)
            {
                string m_insmessage = string.Empty;
                Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Verified));
                m_objMTasksDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
                return this.Direct(true, string.Empty);
            }
            else
            {
                m_objMTasksDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                return this.Direct(false, String.Join(Global.NewLineSeparated, m_lstMessage));
            }

        }
        [ValidateInput(false)]
        public ActionResult Resend(string Selected)
        {
            ////if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Verify)))
            ////    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            List<MailNotificationsVM> m_lstSelectedRow = new List<MailNotificationsVM>();
            m_lstSelectedRow = JSON.Deserialize<List<MailNotificationsVM>>(Selected);

            //GET History
            TMailHistories m_objTMailHistoriesVM = new TMailHistories();//TODO:
            //GET MAIL Notification
            MailNotificationsVM m_objMailNotificationsVM = getMailNotificationsVM(m_lstSelectedRow.FirstOrDefault().MailNotificationID);

                       
            var message = new List<string>();
            Object m_obj = null;
            if (SendMail(m_objMailNotificationsVM, ref message, ref m_obj))
            {
                Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Saved));
                return this.Direct(true, string.Empty);
            }
            else
            {
                Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.ErrorSave));
                return this.Direct(false, string.Empty);
            }
            

        }

        #endregion

        #region Private Method
        private Dictionary<string, object> GetFormData(NameValueCollection parameters, string MailNotificationID = "")
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(MailNotificationsVM.Prop.MailNotificationID.Name, (parameters[MailNotificationsVM.Prop.MailNotificationID.Name].ToString() == string.Empty ? MailNotificationID : parameters[MailNotificationsVM.Prop.MailNotificationID.Name]));
            m_dicReturn.Add(MailNotificationsVM.Prop.Subject.Name, parameters[MailNotificationsVM.Prop.Subject.Name]);
            m_dicReturn.Add(MailNotificationsVM.Prop.FunctionDesc.Name, parameters[MailNotificationsVM.Prop.FunctionDesc.Name]);
            m_dicReturn.Add(MailNotificationsVM.Prop.CreatedDate.Name, parameters[MailNotificationsVM.Prop.CreatedDate.Name]);
            m_dicReturn.Add(MailNotificationsVM.Prop.TaskStatusDesc.Name, parameters[MailNotificationsVM.Prop.TaskStatusDesc.Name]);
            m_dicReturn.Add(MailNotificationsVM.Prop.StatusDesc.Name, parameters[MailNotificationsVM.Prop.StatusDesc.Name]);

            return m_dicReturn;
        }
        private MailNotificationsVM GetSelectedData(Dictionary<string, object> selected, ref string message)
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
            m_lstSelect.Add(MailNotificationsVM.Prop.TaskStatusID.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objMailNotificationsVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(MailNotificationsVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

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

                m_objMailNotificationsVM.RecipientsVM = GetListRecipientsVM(m_objMailNotificationsVM.MailNotificationID, ref message);
                m_objMailNotificationsVM.NotificationValuesVM = GetListNotificationValuesVM(m_objMailNotificationsVM.MailNotificationID, ref message);
                m_objMailNotificationsVM.TemplateTagsVM = GetListTemplateTagsVM(m_objMailNotificationsVM.NotificationTemplateID, ref message);
                foreach (var item in m_objMailNotificationsVM.TemplateTagsVM)
                {
                    item.Value = m_objMailNotificationsVM.NotificationValuesVM.Where(x => x.FieldTagID == item.FieldTagID).Any() ? m_objMailNotificationsVM.NotificationValuesVM.Where(x => x.FieldTagID == item.FieldTagID).FirstOrDefault().Value : null;
                }
                m_objMailNotificationsVM.NotificationAttachmentVM = GetListNotificationAttachmentVM(m_objMailNotificationsVM.MailNotificationID, ref message);

            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMMailNotificationsDA.Message;

            return m_objMailNotificationsVM;
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
        private MailNotificationsVM getMailNotificationsVM(string MailNotificationID)
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
                m_objMailNotificationsVM.NotificationValuesVM = GetListNotificationValuesVM(m_objMailNotificationsVM.MailNotificationID, ref msg);
                m_objMailNotificationsVM.TemplateTagsVM = GetListTemplateTagsVM(m_objMailNotificationsVM.NotificationTemplateID, ref msg);
                m_objMailNotificationsVM.NotificationMapVM = GetDefaultNoticationMap(m_objMailNotificationsVM.FunctionID);//todo: catch error
                foreach (var item in m_objMailNotificationsVM.TemplateTagsVM)
                {

                    item.Value = !m_objMailNotificationsVM.NotificationValuesVM.Where(x => x.FieldTagID == item.FieldTagID).Any() ? null : m_objMailNotificationsVM.NotificationValuesVM.Where(x => x.FieldTagID == item.FieldTagID).FirstOrDefault().Value;
                }
                m_objMailNotificationsVM.NotificationAttachmentVM = GetListNotificationAttachmentVM(m_objMailNotificationsVM.MailNotificationID, ref msg);

            }

            return m_objMailNotificationsVM;
        }
        private List<TemplateTagsVM> GetListTemplateTagsVM(string NotificationTemplateID, ref string message)
        {
            List<TemplateTagsVM> m_lstTemplateTagsVM = new List<TemplateTagsVM>();
            DTemplateTagsDA m_objDTemplateTagsDA = new DTemplateTagsDA();
            m_objDTemplateTagsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(TemplateTagsVM.Prop.TemplateTagID.MapAlias);
            m_lstSelect.Add(TemplateTagsVM.Prop.TemplateID.MapAlias);
            m_lstSelect.Add(TemplateTagsVM.Prop.FieldTagID.MapAlias);
            m_lstSelect.Add(TemplateTagsVM.Prop.TagDesc.MapAlias);
            m_lstSelect.Add(TemplateTagsVM.Prop.TemplateType.MapAlias);


            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(NotificationTemplateID);
            m_objFilter.Add(TemplateTagsVM.Prop.TemplateID.Map, m_lstFilter);


            Dictionary<int, DataSet> m_dicTNotificationValuesDA = m_objDTemplateTagsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDTemplateTagsDA.Success)
            {
                foreach (DataRow m_drTNotificationValuesDA in m_dicTNotificationValuesDA[0].Tables[0].Rows)
                {
                    TemplateTagsVM m_objNotificationValuesVM = new TemplateTagsVM();
                    m_objNotificationValuesVM.TemplateTagID = m_drTNotificationValuesDA[TemplateTagsVM.Prop.TemplateTagID.Name].ToString();
                    m_objNotificationValuesVM.TemplateID = m_drTNotificationValuesDA[TemplateTagsVM.Prop.TemplateID.Name].ToString();
                    m_objNotificationValuesVM.FieldTagID = m_drTNotificationValuesDA[TemplateTagsVM.Prop.FieldTagID.Name].ToString();
                    m_objNotificationValuesVM.TagDesc = m_drTNotificationValuesDA[TemplateTagsVM.Prop.TagDesc.Name].ToString();
                    m_objNotificationValuesVM.TemplateType = m_drTNotificationValuesDA[TemplateTagsVM.Prop.TemplateType.Name].ToString();

                    m_lstTemplateTagsVM.Add(m_objNotificationValuesVM);
                }
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDTemplateTagsDA.Message;

            return m_lstTemplateTagsVM;

        }
        private List<NotificationValuesVM> GetListNotificationValuesVM(string MailNotificationID, ref string message)
        {
            List<NotificationValuesVM> m_lstNotificationValuesVM = new List<NotificationValuesVM>();
            TNotificationValuesDA m_objTNotificationValuesDA = new TNotificationValuesDA();
            m_objTNotificationValuesDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(NotificationValuesVM.Prop.NotificationValueID.MapAlias);
            m_lstSelect.Add(NotificationValuesVM.Prop.Value.MapAlias);
            m_lstSelect.Add(NotificationValuesVM.Prop.MailNotificationID.MapAlias);
            m_lstSelect.Add(NotificationValuesVM.Prop.FieldTagID.MapAlias);
            m_lstSelect.Add(NotificationValuesVM.Prop.TagDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(MailNotificationID);
            m_objFilter.Add(NotificationValuesVM.Prop.MailNotificationID.Map, m_lstFilter);


            Dictionary<int, DataSet> m_dicTNotificationValuesDA = m_objTNotificationValuesDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objTNotificationValuesDA.Success)
            {
                foreach (DataRow m_drTNotificationValuesDA in m_dicTNotificationValuesDA[0].Tables[0].Rows)
                {
                    NotificationValuesVM m_objNotificationValuesVM = new NotificationValuesVM();
                    m_objNotificationValuesVM.NotificationValueID = m_drTNotificationValuesDA[NotificationValuesVM.Prop.NotificationValueID.Name].ToString();
                    m_objNotificationValuesVM.Value = m_drTNotificationValuesDA[NotificationValuesVM.Prop.Value.Name].ToString();
                    m_objNotificationValuesVM.MailNotificationID = m_drTNotificationValuesDA[NotificationValuesVM.Prop.MailNotificationID.Name].ToString();
                    m_objNotificationValuesVM.FieldTagID = m_drTNotificationValuesDA[NotificationValuesVM.Prop.FieldTagID.Name].ToString();
                    m_objNotificationValuesVM.TagDesc = m_drTNotificationValuesDA[NotificationValuesVM.Prop.TagDesc.Name].ToString();

                    m_lstNotificationValuesVM.Add(m_objNotificationValuesVM);
                }
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objTNotificationValuesDA.Message;

            return m_lstNotificationValuesVM;

        }

        #endregion

        #region Public Method

        #endregion
    }
}