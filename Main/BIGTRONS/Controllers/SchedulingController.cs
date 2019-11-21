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
using System.Net;
using System.Xml;
using System.IO;
using System.Web;
using System.Runtime.Serialization.Formatters.Binary;

namespace com.SML.BIGTRONS.Controllers
{
    public class SchedulingController : BaseController
    {
        private readonly string title = "My Schedule";
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
            //SendMailWithCheck();
            MSchedulesDA m_objMSchedulesDA = new MSchedulesDA();
            m_objMSchedulesDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMSchedules = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMSchedules.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = SchedulesVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMSchedulesDA = m_objMSchedulesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;
            foreach (KeyValuePair<int, DataSet> m_kvpDivisionBL in m_dicMSchedulesDA)
            {
                m_intCount = m_kvpDivisionBL.Key;
                break;
            }
            List<SchedulesVM> m_lstSchedulesVM = new List<SchedulesVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(SchedulesVM.Prop.ScheduleID.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.TaskID.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.TaskTypeID.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.TaskOwnerID.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.FPTID.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.StatusID.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.StatusDesc.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.FunctionID.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.FunctionDescription.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.NotificationTemplateID.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.StartDate.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.EndDate.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.Subject.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.IsAllDay.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.Notes.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.Weblink.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.Location.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.Priority.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.CreatedDate.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.MailNotificationID.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.IsBatchMail.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(SchedulesVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                if (!m_dicOrder.Any())
                {
                    m_dicOrder = new Dictionary<string, OrderDirection>();
                    m_dicOrder.Add(SchedulesVM.Prop.CreatedDate.Map, OrderDirection.Ascending);
                }
                m_dicMSchedulesDA = m_objMSchedulesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMSchedulesDA.Message == string.Empty)
                {
                    m_lstSchedulesVM = (
                        from DataRow m_drMScheduleDA in m_dicMSchedulesDA[0].Tables[0].Rows
                        select new SchedulesVM()
                        {
                            ScheduleID = m_drMScheduleDA[SchedulesVM.Prop.ScheduleID.Name].ToString(),
                            FPTID = m_drMScheduleDA[SchedulesVM.Prop.FPTID.Name].ToString(),
                            TaskID = m_drMScheduleDA[SchedulesVM.Prop.TaskID.Name].ToString(),
                            TaskTypeID = m_drMScheduleDA[SchedulesVM.Prop.TaskTypeID.Name].ToString(),
                            TaskOwnerID = m_drMScheduleDA[SchedulesVM.Prop.TaskOwnerID.Name].ToString(),
                            StatusID = m_drMScheduleDA[SchedulesVM.Prop.StatusID.Name].ToString(),
                            StatusDesc = m_drMScheduleDA[SchedulesVM.Prop.StatusDesc.Name].ToString(),
                            FunctionID = m_drMScheduleDA[SchedulesVM.Prop.FunctionID.Name].ToString(),
                            FunctionDescription = m_drMScheduleDA[SchedulesVM.Prop.FunctionDescription.Name].ToString(),
                            NotificationTemplateID = m_drMScheduleDA[SchedulesVM.Prop.NotificationTemplateID.Name].ToString(),
                            StartDate = (DateTime)m_drMScheduleDA[SchedulesVM.Prop.StartDate.Name],
                            EndDate = (DateTime)m_drMScheduleDA[SchedulesVM.Prop.EndDate.Name],
                            Subject = m_drMScheduleDA[SchedulesVM.Prop.Subject.Name].ToString(),
                            IsAllDay = (bool)m_drMScheduleDA[SchedulesVM.Prop.IsAllDay.Name],
                            Notes = m_drMScheduleDA[SchedulesVM.Prop.Notes.Name].ToString(),
                            Weblink = m_drMScheduleDA[SchedulesVM.Prop.Weblink.Name].ToString(),
                            Location = m_drMScheduleDA[SchedulesVM.Prop.Location.Name].ToString(),
                            Priority = (int)m_drMScheduleDA[SchedulesVM.Prop.Priority.Name],
                            CreatedDate = (DateTime)m_drMScheduleDA[SchedulesVM.Prop.CreatedDate.Name],
                            IsBatchMail = string.IsNullOrEmpty(m_drMScheduleDA[SchedulesVM.Prop.IsBatchMail.Name].ToString())? false: (bool)m_drMScheduleDA[SchedulesVM.Prop.IsBatchMail.Name],
                            MailNotificationID = m_drMScheduleDA[SchedulesVM.Prop.MailNotificationID.Name].ToString()
                        }
                    ).ToList();
                }
            }
            //var t = new mailws.MailSender();
            //string send = t.SendMail("mangkuk@sinarmasland.com","ronypasca@gmail.com","rezaseptiandra@gmail.com","","Rambutku keren","",null);
            
            return this.Store(m_lstSchedulesVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            MSchedulesDA m_objMSchedulesDA = new MSchedulesDA();
            m_objMSchedulesDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMSchedules = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMSchedules.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = SchedulesVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMSchedulesDA = m_objMSchedulesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpDivisionBL in m_dicMSchedulesDA)
            {
                m_intCount = m_kvpDivisionBL.Key;
                break;
            }

            List<SchedulesVM> m_lstSchedulesVM = new List<SchedulesVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(SchedulesVM.Prop.ScheduleID.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.TaskID.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.FPTID.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.FPTDescription.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.StatusID.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.StatusDesc.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.FunctionID.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.FunctionDescription.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.NotificationTemplateID.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.StartDate.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.EndDate.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.Subject.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.IsAllDay.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.Notes.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.Weblink.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.Location.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.Priority.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.IsBatchMail.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.MailNotificationID.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(SchedulesVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMSchedulesDA = m_objMSchedulesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMSchedulesDA.Message == string.Empty)
                {
                    m_lstSchedulesVM = (
                        from DataRow m_drMScheduleDA in m_dicMSchedulesDA[0].Tables[0].Rows
                        select new SchedulesVM()
                        {
                            ScheduleID = m_drMScheduleDA[SchedulesVM.Prop.ScheduleID.Name].ToString(),
                            FPTID = m_drMScheduleDA[SchedulesVM.Prop.FPTID.Name].ToString(),
                            FPTDescription = m_drMScheduleDA[SchedulesVM.Prop.FPTDescription.Name].ToString(),
                            TaskID = m_drMScheduleDA[SchedulesVM.Prop.TaskID.Name].ToString(),
                            StatusID = m_drMScheduleDA[SchedulesVM.Prop.StatusID.Name].ToString(),
                            StatusDesc = m_drMScheduleDA[SchedulesVM.Prop.StatusDesc.Name].ToString(),
                            FunctionID = m_drMScheduleDA[SchedulesVM.Prop.FunctionID.Name].ToString(),
                            FunctionDescription = m_drMScheduleDA[SchedulesVM.Prop.FunctionDescription.Name].ToString(),
                            NotificationTemplateID = m_drMScheduleDA[SchedulesVM.Prop.NotificationTemplateID.Name].ToString(),
                            StartDate = (DateTime)m_drMScheduleDA[SchedulesVM.Prop.StartDate.Name],
                            EndDate = (DateTime)m_drMScheduleDA[SchedulesVM.Prop.EndDate.Name],
                            Subject = m_drMScheduleDA[SchedulesVM.Prop.Subject.Name].ToString(),
                            IsAllDay = (bool)m_drMScheduleDA[SchedulesVM.Prop.IsAllDay.Name],
                            Notes = m_drMScheduleDA[SchedulesVM.Prop.Notes.Name].ToString(),
                            Weblink = m_drMScheduleDA[SchedulesVM.Prop.Weblink.Name].ToString(),
                            Location = m_drMScheduleDA[SchedulesVM.Prop.Location.Name].ToString(),
                            Priority = (int)m_drMScheduleDA[SchedulesVM.Prop.Priority.Name],
                            IsBatchMail  = string.IsNullOrEmpty(m_drMScheduleDA[SchedulesVM.Prop.IsBatchMail.Name].ToString()) ? false : (bool)m_drMScheduleDA[SchedulesVM.Prop.IsBatchMail.Name],
                            MailNotificationID = m_drMScheduleDA[SchedulesVM.Prop.MailNotificationID.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstSchedulesVM, m_intCount);
        }
        public ActionResult CheckHoliday(string startDate, string endDate)
        {
            DateTime StartDate = Convert.ToDateTime(JSON.Deserialize<string>(startDate));
            DateTime EndDate = Convert.ToDateTime(JSON.Deserialize<string>(endDate));

            MHolidaysDA HolidayDA = new MHolidaysDA();
            HolidayDA.ConnectionStringName = Global.ConnStrConfigName;
            List<DateTime> lstDateHoliday = new List<DateTime>();
            List<string> lstSelectHoliday = new List<string>();
            lstSelectHoliday.Add(HolidayVM.Prop.HolidayID.MapAlias);
            lstSelectHoliday.Add(HolidayVM.Prop.HolidayDate.MapAlias);

            Dictionary<string, List<object>> m_objFilter_ = new Dictionary<string, List<object>>();
            List<object> m_lstFilter_ = new List<object>();

            m_lstFilter_ = new List<object>();
            m_lstFilter_.Add(Operator.None);
            m_lstFilter_.Add(string.Empty);
            m_objFilter_.Add(string.Format("({2} >= '{0}' AND {2} <='{1}') ",StartDate.ToString(Global.SqlDateFormat), EndDate.ToString(Global.SqlDateFormat), HolidayVM.Prop.HolidayDate.Map), m_lstFilter_);

            Dictionary<int, DataSet> m_dicMailNotif = HolidayDA.SelectBC(0, null, false, lstSelectHoliday, m_objFilter_, null, null, null);
            if (HolidayDA.Success && HolidayDA.Message == "")
                foreach (DataRow dr in m_dicMailNotif[0].Tables[0].Rows)
                    return this.Direct("true");
                
            return this.Direct("false");
        }
        public ActionResult Home()
        {
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 0;
            return this.Direct();
        }

        public ActionResult Add(string Caller,string Selected)
        {
            string m_strMessage = string.Empty;
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Add)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            SchedulesVM m_objSchedulesVM = new SchedulesVM();
            ViewDataDictionary m_vddSchedule = new ViewDataDictionary();
            m_vddSchedule.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddSchedule.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
            Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();

            m_objSchedulesVM.StatusID = ((int)ScheduleStatus.Draft).ToString();//Check
            m_objSchedulesVM.StartDate = DateTime.Now;
            m_objSchedulesVM.EndDate = DateTime.Now;
            m_objSchedulesVM.LstRecipients = new List<RecipientsVM>();
            m_objSchedulesVM.LstNotificationValues = new List<NotificationValuesVM>();
            m_objSchedulesVM.LstNotificationAttachment = new List<NotificationAttachmentVM>();

            if (Caller == General.EnumDesc(Buttons.ButtonDetail))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                m_dicSelectedRow = GetFormData(m_nvcParams);
                Session[dataSessionName] = JSON.Serialize(m_dicSelectedRow);
            } else if (Caller == "GetData") {
                m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
                if (m_dicSelectedRow.Count > 0)
                    m_objSchedulesVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
                m_objSchedulesVM.ScheduleID = string.Empty;
                m_objSchedulesVM.StatusID = "0";
                if (m_strMessage != string.Empty)
                {
                    Global.ShowErrorAlert(title, m_strMessage);
                    return this.Direct();
                }
            }
            m_vddSchedule.Add("IsCopyData", Caller == "GetData"?bool.TrueString:bool.FalseString);
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objSchedulesVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddSchedule,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult ReloadParameter(string MailNotificationID ="",string TemplateID="")
        {
           return this.Store(GetListNotificationValues(MailNotificationID, TemplateID));
        }
        public ActionResult ReloadFunction(string FunctionID = "")
        {
            
            DTCFunctionsDA m_objTCFunctions = new DTCFunctionsDA();
            m_objTCFunctions.ConnectionStringName = Global.ConnStrConfigName;

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<TCFunctionsVM> m_lsTCFunctionVM = new List<TCFunctionsVM>();
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(TCFunctionsVM.Prop.FunctionID.MapAlias);
            m_lstSelect.Add(TCFunctionsVM.Prop.FirstName.MapAlias);
            m_lstSelect.Add(TCFunctionsVM.Prop.Email.MapAlias);
            m_lstSelect.Add(TCFunctionsVM.Prop.UserID.MapAlias);

            //List<string> m_lstm_Group = new List<string>();
            //m_lstm_Group.Add(TCFunctionVM.Prop.TemplateTagID.Map);
            //m_lstm_Group.Add(TCFunctionVM.Prop.FieldTagID.Map);
            //m_lstm_Group.Add(TCFunctionVM.Prop.TagDesc.Map);
            //m_lstm_Group.Add(TCFunctionVM.Prop.TemplateID.Map);
            //m_lstm_Group.Add(TCFunctionVM.Prop.TemplateType.Map);

            if (FunctionID.Length > 0)
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(FunctionID);
                m_objFilter.Add(TCFunctionsVM.Prop.FunctionID.Map, m_lstFilter);

                Dictionary<int, DataSet> m_dicTemplateTagDA = m_objTCFunctions.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);
                if (m_objTCFunctions.Message == string.Empty)
                {
                    m_lsTCFunctionVM = (
                        from DataRow m_drTNotificationValues in m_dicTemplateTagDA[0].Tables[0].Rows
                        select new TCFunctionsVM()
                        {
                            UserID = m_drTNotificationValues[TCFunctionsVM.Prop.UserID.Name].ToString(),
                            Email= m_drTNotificationValues[TCFunctionsVM.Prop.Email.Name].ToString(),
                            FirstName = m_drTNotificationValues[TCFunctionsVM.Prop.FirstName.Name].ToString()
                        }
                    ).Distinct().ToList();
                }
            }
            return this.Store(m_lsTCFunctionVM);
        }

        public ActionResult Detail(string Caller, string Selected, string ScheduleID="")
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            SchedulesVM m_objSchedulesVM = new SchedulesVM();
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
                m_objSchedulesVM = ScheduleID.Length>0? GetSelectedData(m_dicSelectedRow, ref m_strMessage,ScheduleID) : GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }
            
            
            ViewDataDictionary m_vddWorkCenter = new ViewDataDictionary();
            m_vddWorkCenter.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objSchedulesVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddWorkCenter,
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
            if (Caller == General.EnumDesc(Buttons.ButtonList) ||
                Caller == "Reschedule" ||
                Caller == "Cancellation" )
            {
                m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
            }
            else if (Caller == General.EnumDesc(Buttons.ButtonDetail) )
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                m_dicSelectedRow = GetFormData(m_nvcParams);
                Session[dataSessionName] = JSON.Serialize(m_dicSelectedRow);
            }

            string m_strMessage = string.Empty;
            SchedulesVM m_bojscheduleVM = new SchedulesVM();
            if (m_dicSelectedRow.Count > 0)
                m_bojscheduleVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddDivision = new ViewDataDictionary();
            m_vddDivision.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddDivision.Add(General.EnumDesc(Params.Action),Caller == "Reschedule" || Caller == "Cancellation" ? Caller: General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_bojscheduleVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddDivision,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<SchedulesVM> m_lstSelectedRow = new List<SchedulesVM>();
            m_lstSelectedRow = JSON.Deserialize<List<SchedulesVM>>(Selected);

            MSchedulesDA m_objMSchedulesDA = new MSchedulesDA();
            m_objMSchedulesDA.ConnectionStringName = Global.ConnStrConfigName;

            MMailNotificationsDA m_objMMailNotificationsDA = new MMailNotificationsDA();
            m_objMMailNotificationsDA.ConnectionStringName = Global.ConnStrConfigName;

            MTasksDA m_objMTasksDA = new MTasksDA();
            m_objMTasksDA.ConnectionStringName = Global.ConnStrConfigName;

            DRecipientsDA m_objDRecipientsDA = new DRecipientsDA();
            m_objDRecipientsDA.ConnectionStringName = Global.ConnStrConfigName;

            TNotificationAttachmentsDA m_objTNotificationAttachmentsDA = new TNotificationAttachmentsDA();
            m_objTNotificationAttachmentsDA.ConnectionStringName = Global.ConnStrConfigName;

            TNotificationValuesDA m_objTNotificationValuesDA = new TNotificationValuesDA();
            m_objTNotificationValuesDA.ConnectionStringName = Global.ConnStrConfigName;

            object m_objDBConnection = null;
            string m_strTransName = "DeleteSchedule";
            m_objDBConnection = m_objMSchedulesDA.BeginTrans(m_strTransName);

            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (SchedulesVM m_objScheduleVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    List<string> m_lstMailNotificationID = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifScheduleVM = m_objScheduleVM.GetType().GetProperties();
                    string taskID = "";
                    foreach (PropertyInfo m_pifCompanyVM in m_arrPifScheduleVM)
                    {
                        string m_strFieldName = m_pifCompanyVM.Name;
                        object m_objFieldValue = m_pifCompanyVM.GetValue(m_objScheduleVM);
                        if (m_objScheduleVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(SchedulesVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else if(m_strFieldName == SchedulesVM.Prop.TaskID.Name)
                        {
                            taskID = (string)m_pifCompanyVM.GetValue(m_objScheduleVM);
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(taskID);
                            m_objFilter.Add(SchedulesVM.Prop.TaskID.Map,m_lstFilter);

                            List<string> m_lstSelect_ = new List<string>();
                            m_lstSelect_.Add(MailNotificationsVM.Prop.MailNotificationID.MapAlias);

                            Dictionary<string, List<object>> m_objFilter_ = new Dictionary<string, List<object>>();
                            List<object> m_lstFilter_ = new List<object>();
                            m_lstFilter_.Add(Operator.Equals);
                            m_lstFilter_.Add(taskID);
                            m_objFilter_.Add(MailNotificationsVM.Prop.TaskID.Map, m_lstFilter_);

                            Dictionary<int, DataSet> m_dicMailNotif = m_objMMailNotificationsDA.SelectBC(0, null, false, m_lstSelect_, m_objFilter_, null, null, null);
                            if (m_objMMailNotificationsDA.Success && m_objMMailNotificationsDA.Message == "")
                               foreach (DataRow dr in m_dicMailNotif[0].Tables[0].Rows)
                                    m_lstMailNotificationID.Add(dr[MailNotificationsVM.Prop.MailNotificationID.Name].ToString());
                            else
                                m_lstMessage.Add("Error while Get List MailNotificationID");

                        }
                        if (m_objFilter.Count >= 2)
                            break;
                    }

                    //MSchedules
                    m_objMSchedulesDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                    if (m_objMSchedulesDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMSchedulesDA.Message);

                    //Attachments
                    if (m_lstMessage.Count == 0)
                    {
                        m_objFilter = new Dictionary<string, List<object>>();
                        foreach (string mmailnotificationid in m_lstMailNotificationID)
                        {
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(mmailnotificationid);
                            m_objFilter.Add(NotificationAttachmentVM.Prop.MailNotificationID.Map, m_lstFilter);
                            m_objTNotificationAttachmentsDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                            if (!m_objTNotificationAttachmentsDA.Success)
                                m_lstMessage.Add(m_objTNotificationAttachmentsDA.Message); break;
                        }
                    }

                    //NotificationValues
                    if (m_lstMessage.Count == 0)
                    {
                        m_objFilter = new Dictionary<string, List<object>>();
                        foreach (string mmailnotificationid in m_lstMailNotificationID)
                        {
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(mmailnotificationid);
                            m_objFilter.Add(NotificationValuesVM.Prop.MailNotificationID.Map, m_lstFilter);
                            m_objTNotificationValuesDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                            if (!m_objTNotificationValuesDA.Success)
                                m_lstMessage.Add(m_objTNotificationValuesDA.Message);break;
                        }
                    }

                    //Recipients
                    if (m_lstMessage.Count == 0)
                    {
                        m_objFilter = new Dictionary<string, List<object>>();
                        foreach (string mmailnotificationid in m_lstMailNotificationID)
                        {
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(mmailnotificationid);
                            m_objFilter.Add(RecipientsVM.Prop.MailNotificationID.Map, m_lstFilter);
                            m_objDRecipientsDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                            if (!m_objDRecipientsDA.Success)
                                m_lstMessage.Add(m_objDRecipientsDA.Message); break;
                        }
                    }

                    //MMailNotification
                    if (m_lstMessage.Count == 0)
                    {
                        m_objFilter = new Dictionary<string, List<object>>();                        
                        List<object> m_lstFilter = new List<object>();
                        m_lstFilter.Add(Operator.In);
                        m_lstFilter.Add(string.Join(",",m_lstMailNotificationID));
                        m_objFilter.Add(MailNotificationsVM.Prop.MailNotificationID.Map, m_lstFilter);
                        m_objMMailNotificationsDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                        if (!m_objMMailNotificationsDA.Success)
                            m_lstMessage.Add(m_objMMailNotificationsDA.Message); break;
                        
                    }

                    //MTasks
                    if (m_lstMessage.Count == 0)
                    {
                        m_objFilter = new Dictionary<string, List<object>>();
                        List<object> m_lstFilter = new List<object>();
                        m_lstFilter.Add(Operator.Equals);
                        m_lstFilter.Add(taskID);
                        m_objFilter.Add(MyTaskVM.Prop.TaskID.Map, m_lstFilter);
                        m_objMTasksDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                        if (!m_objMTasksDA.Success)
                            m_lstMessage.Add(m_objMTasksDA.Message); break;

                    }

                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
                m_objMSchedulesDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
            }
            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Deleted));
                m_objMSchedulesDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
            }
            else
            {
                Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
                m_objMSchedulesDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
            }
            return this.Direct();
        }

        public ActionResult Change(string Caller, string Selected)
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
            SchedulesVM m_bojscheduleVM = new SchedulesVM();
            if (m_dicSelectedRow.Count > 0)
                m_bojscheduleVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddDivision = new ViewDataDictionary();
            m_vddDivision.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddDivision.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonChange));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_bojscheduleVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddDivision,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult Cancel(string Caller, string Selected)
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
            SchedulesVM m_bojscheduleVM = new SchedulesVM();
            if (m_dicSelectedRow.Count > 0)
                m_bojscheduleVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddDivision = new ViewDataDictionary();
            m_vddDivision.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddDivision.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonCancel));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_bojscheduleVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddDivision,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult ReadAttachment()
        {                       
            List<byte[]> lstByte = new List<byte[]>();
            string ContentType = "";
            string FileName = "";
            string StringRawData = "";
            //byte[] RawData = null;
            for (int x = 0; x < Request.Files.Count; x++)
            {
                if (Request.Files[x].ContentLength > 500000)
                {
                    Global.ShowErrorAlert("Error Upload", "Maximum file size is 500KB");
                    return this.Direct();
                }
                byte[] fileData = null;
                using (var binaryReader = new BinaryReader(Request.Files[x].InputStream)) 
                    fileData = binaryReader.ReadBytes(Request.Files[x].ContentLength);
                FileName = Path.GetFileName(Server.MapPath(Request.Files[0].FileName));
                ContentType = Path.GetExtension(FileName);
                StringRawData = GetBase64String(fileData);

                //RawData = ConvertHASHStringToByte(StringRawData);
                //var t = new mailws.MailSender();
                //mailws.MailAttachment[] Attachment = new mailws.MailAttachment[1];
                //var u = new mailws.MailAttachment()
                //{
                //    FileName = FileName,
                //    Content = RawData
                //};
                //Attachment[0] = u;
                ////string send = t.SendMail("bigtronstes@sinarmasland.com", "ronypasca@gmail.com", "rezaseptiandra@gmail.com", "", "Coba Kirim Attachment", "", Attachment);
                //l///stByte.Add(fileData);
            }
            NotificationAttachmentVM objs = new NotificationAttachmentVM()
            {               
                Filename = FileName,
                ContentType = ContentType,
                RawData = StringRawData,
                MailNotificationID = ""

            };
            object ls = objs;
            Global.ShowInfo("Upload", "Success");
            return this.Store(ls);
        }
       
        public ActionResult SendMailTest(string base64str)
        {
            var t = new mailws.MailSender();
            mailws.MailAttachment[] Attachment = new mailws.MailAttachment[1];

            var u = new mailws.MailAttachment() {
                FileName="cobasaja.xls",
                Content = GetByteFromBase64STR(base64str)               
            };

            Attachment[0] = u;
            string send = t.SendMail("bigtronsTest@sinarmasland.com", "revo.christie@gmail.com", "rezaseptiandra@gmail.com","","Rambutku keren","", Attachment);
            return this.Direct();
        }
        public ActionResult Browse(string ControlScheduleID, string ControlSubject, string ControlStartDate, string ControlEndDate, string ControlLocation, string ControlNotes, string ControlFPTID, string ControlFPTDescription,
            string ControlFunctionID, string ControlFunctionDescription, string FilterScheduleID = "", string FilterSubject = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddSchedule = new ViewDataDictionary();
            m_vddSchedule.Add("Control" + SchedulesVM.Prop.ScheduleID.Name, ControlScheduleID);
            m_vddSchedule.Add("Control" + SchedulesVM.Prop.Subject.Name, ControlSubject);
            m_vddSchedule.Add("Control" + SchedulesVM.Prop.StartDate.Name, ControlStartDate);
            m_vddSchedule.Add("Control" + SchedulesVM.Prop.EndDate.Name, ControlEndDate);
            m_vddSchedule.Add("Control" + SchedulesVM.Prop.Notes.Name, ControlNotes);
            m_vddSchedule.Add("Control" + SchedulesVM.Prop.FPTID.Name, ControlFPTID);
            m_vddSchedule.Add("Control" + SchedulesVM.Prop.FPTDescription.Name, ControlFPTDescription);
            m_vddSchedule.Add("Control" + SchedulesVM.Prop.FunctionID.Name, ControlFunctionID);
            m_vddSchedule.Add("Control" + SchedulesVM.Prop.FunctionDescription.Name, ControlFunctionDescription);
            m_vddSchedule.Add(SchedulesVM.Prop.ScheduleID.Name, FilterScheduleID);
            m_vddSchedule.Add(SchedulesVM.Prop.Subject.Name, FilterSubject);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddSchedule,
                ViewName = "../Scheduling/_Browse"
            };
        }

        //BrowsePreBuildRecipients
        public ActionResult BrowsePreBuildRecipients(string ControlDivisionID, string ControlDivisionDesc, string FilterDivisionID = "", string FilterDivisionDesc = "", string FilterBusinessUnitID = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));
            

            ViewDataDictionary m_vddDivision = new ViewDataDictionary();
            m_vddDivision.Add("Control" + DivisionVM.Prop.DivisionID.Name, ControlDivisionID);
            m_vddDivision.Add("Control" + DivisionVM.Prop.DivisionDesc.Name, ControlDivisionDesc);
            m_vddDivision.Add(DivisionVM.Prop.DivisionID.Name, FilterDivisionID);
            m_vddDivision.Add(DivisionVM.Prop.DivisionDesc.Name, FilterDivisionDesc);
            m_vddDivision.Add(BusinessUnitVM.Prop.BusinessUnitID.Name, FilterBusinessUnitID);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddDivision,
                ViewName = "../Division/_Browse"
            };
        }
        [ValidateInput(false)]
        public ActionResult Save(string Action,string Caller)
        {
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));
           
            bool isUpdate = Action == General.EnumDesc(Buttons.ButtonUpdate);            
            bool isCancel = Action == General.EnumDesc(Buttons.ButtonCancel);
            bool isReschedule = Action == General.EnumDesc(Buttons.ButtonChange);
            bool isAdd = (Action == General.EnumDesc(Buttons.ButtonAdd)) || isCancel || isReschedule;

            string schedID = isAdd ? Guid.NewGuid().ToString().Replace("-", "") : this.Request.Params[SchedulesVM.Prop.ScheduleID.Name];
            string m_taskID = "";
            List<string> m_lstMessage = new List<string>();

            MSchedulesDA m_objMSchedulesDA = new MSchedulesDA();
            m_objMSchedulesDA.ConnectionStringName = Global.ConnStrConfigName;

            DNotificationMapDA m_objNotificationMapDA = new DNotificationMapDA();
            m_objNotificationMapDA.ConnectionStringName = Global.ConnStrConfigName;

            MFieldTagReferencesDA tagrefDA = new MFieldTagReferencesDA();
            tagrefDA.ConnectionStringName = Global.ConnStrConfigName;

            MTasksDA m_objMTaskDA = new MTasksDA();
            m_objMTaskDA.ConnectionStringName = Global.ConnStrConfigName;

            DTaskDetailsDA m_objDTaskDetailsDA = new DTaskDetailsDA();
            m_objDTaskDetailsDA.ConnectionStringName = Global.ConnStrConfigName;

            MMailNotificationsDA m_objMMailNotifDA = new MMailNotificationsDA();
            m_objMMailNotifDA.ConnectionStringName = Global.ConnStrConfigName;

            DRecipientsDA m_objDRecipientDA = new DRecipientsDA();
            m_objDRecipientDA.ConnectionStringName = Global.ConnStrConfigName;

            TNotificationValuesDA m_objTNotificationValues = new TNotificationValuesDA();
            m_objTNotificationValues.ConnectionStringName = Global.ConnStrConfigName;

            TNotificationAttachmentsDA m_objTNotificationAttachment = new TNotificationAttachmentsDA();
            m_objTNotificationAttachment.ConnectionStringName = Global.ConnStrConfigName;

            object m_objDBConnection = null;
            string m_strTransName = "SaveSchedule";
            m_objDBConnection = m_objMSchedulesDA.BeginTrans(m_strTransName);

            try
            {
                string mailnotifID = "";
                MSchedules objSchedule = new MSchedules();
                bool isUpdateFromBatchButNotBatch = false;
                DateTime m_StartDate = DateTime.Parse(this.Request.Params[SchedulesVM.Prop.StartDate.Name]);
                TimeSpan m_StartDateHour = TimeSpan.Parse((DateTime.Parse(this.Request.Params["StartDateHour"].ToString())).ToString("HH:mm"));
                objSchedule.StartDate = m_StartDate.Add(m_StartDateHour);
                DateTime m_EndDate = DateTime.Parse(this.Request.Params[SchedulesVM.Prop.EndDate.Name]);
                TimeSpan m_EndDateHour = TimeSpan.Parse((DateTime.Parse(this.Request.Params["EndDateHour"].ToString())).ToString("HH:mm"));
                objSchedule.EndDate = m_EndDate.Add(m_EndDateHour);

                objSchedule.FPTID = string.IsNullOrEmpty(this.Request.Params[SchedulesVM.Prop.FPTID.Name]) ? null : this.Request.Params[SchedulesVM.Prop.FPTID.Name];
                objSchedule.StatusID = Caller == "Reschedule" ? ((int)ScheduleStatus.Draft_Reschedule).ToString() :
                                       Caller == "Cancellation" ? ((int)ScheduleStatus.Draft_Cancellation).ToString() : this.Request.Params[SchedulesVM.Prop.StatusID.Name];
                string m_TemplateID = this.Request.Params[SchedulesVM.Prop.NotificationTemplateID.Name];
                objSchedule.Subject = this.Request.Params[SchedulesVM.Prop.Subject.Name];
                objSchedule.Notes = this.Request.Params[SchedulesVM.Prop.Notes.Name];
                objSchedule.Weblink = this.Request.Params[SchedulesVM.Prop.Weblink.Name];
                objSchedule.ProjectID = this.Request.Params[SchedulesVM.Prop.ProjectID.Name];
                objSchedule.ClusterID = string.IsNullOrEmpty(this.Request.Params[SchedulesVM.Prop.ClusterID.Name])? null : this.Request.Params[SchedulesVM.Prop.ClusterID.Name];
                string ProjectDesc = this.Request.Params[SchedulesVM.Prop.ProjectDesc.Name]; 
                string ClusterDesc = this.Request.Params[SchedulesVM.Prop.ClusterDesc.Name];
                objSchedule.Location = this.Request.Params[SchedulesVM.Prop.Location.Name];
                objSchedule.Priority = Convert.ToInt16(this.Request.Params[SchedulesVM.Prop.Priority.Name]);
                objSchedule.IsAllDay = Convert.ToBoolean(this.Request.Params[SchedulesVM.Prop.IsAllDay.Name]);
                objSchedule.IsBatchMail = Convert.ToBoolean(this.Request.Params[SchedulesVM.Prop.IsBatchMail.Name]);
                string m_pFunctionID = this.Request.Params[SchedulesVM.Prop.FunctionID.Name];
                string m_pTemplateID = this.Request.Params[SchedulesVM.Prop.NotificationTemplateID.Name];
                bool m_bKeepTask = string.IsNullOrEmpty(this.Request.Params["IsKeepTask"].ToString())?false:bool.Parse(this.Request.Params["IsKeepTask"].ToString());
                m_taskID = this.Request.Params[SchedulesVM.Prop.TaskID.Name];
                
                List<string> ListMailNotif = new List<string>();
                if (isUpdate)
                {
                    List<string> m_lstSelect_ = new List<string>();
                    m_lstSelect_.Add(MailNotificationsVM.Prop.MailNotificationID.MapAlias);

                    Dictionary<string, List<object>> m_objFilter_ = new Dictionary<string, List<object>>();
                    List<object> m_lstFilter_ = new List<object>();
                    m_lstFilter_.Add(Operator.Equals);
                    m_lstFilter_.Add(m_taskID);
                    m_objFilter_.Add(MailNotificationsVM.Prop.TaskID.Map, m_lstFilter_);

                    Dictionary<int, DataSet> m_dicMailNotif = m_objMMailNotifDA.SelectBC(0, null, false, m_lstSelect_, m_objFilter_, null, null, null);
                    if (m_objMMailNotifDA.Success && m_objMMailNotifDA.Message == "")
                    {
                        foreach (DataRow dr in m_dicMailNotif[0].Tables[0].Rows)
                            ListMailNotif.Add(dr[MailNotificationsVM.Prop.MailNotificationID.Name].ToString());
                        isUpdateFromBatchButNotBatch = ListMailNotif.Count > 1 && isUpdate && !objSchedule.IsBatchMail;
                    }
                    else
                        m_lstMessage.Add("Error while Get List MailNotificationID");

                }

                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(NotificationMapVM.Prop.NotifMapID.MapAlias);

                Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                List<object> m_lstFilter = new List<object>();
                List<TNotificationValues> ListNotificationValues = new List<TNotificationValues>();
                List<FieldTagReferenceVM> TagRefList = new List<FieldTagReferenceVM>();
                m_lstSelect = new List<string>();
                m_lstSelect.Add(FieldTagReferenceVM.Prop.FieldTagID.MapAlias);
                m_lstSelect.Add(FieldTagReferenceVM.Prop.RefTable.MapAlias);
                m_lstSelect.Add(FieldTagReferenceVM.Prop.RefIDColumn.MapAlias);

                m_objFilter = new Dictionary<string, List<object>>();
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add("MSchedules");
                m_objFilter.Add(FieldTagReferenceVM.Prop.RefTable.Map, m_lstFilter);

                Dictionary<int, DataSet> m_dicTagRefDA = tagrefDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
                if (tagrefDA.Success && tagrefDA.Message == "")
                {
                    SchedulesVM objSchedToPopulate = new SchedulesVM();
                    objSchedToPopulate.StartDate = objSchedule.StartDate;
                    objSchedToPopulate.EndDate = objSchedule.EndDate;
                    objSchedToPopulate.Subject = objSchedule.Subject;
                    objSchedToPopulate.Location= objSchedule.Location;
                    objSchedToPopulate.Weblink = objSchedule.Weblink;
                    objSchedToPopulate.ProjectDesc = ProjectDesc;
                    objSchedToPopulate.ClusterDesc = ClusterDesc;
                    objSchedToPopulate.Notes= objSchedule.Notes;
                    objSchedToPopulate.IsAllDay = objSchedule.IsAllDay;
                    objSchedToPopulate.EndTime = objSchedule.EndDate.ToString(Global.ShortTimeFormat);  
                    objSchedToPopulate.StartTime= objSchedule.StartDate.ToString(Global.ShortTimeFormat);  
                    objSchedToPopulate.Priority = objSchedule.Priority;

                    foreach (DataRow dr in m_dicTagRefDA[0].Tables[0].Rows)
                    {
                        TNotificationValues obj = new TNotificationValues();
                        obj.NotificationValueID = Guid.NewGuid().ToString().Replace("-", "");
                        obj.FieldTagID = dr[FieldTagReferenceVM.Prop.FieldTagID.Name].ToString();
                        string refColID = dr[FieldTagReferenceVM.Prop.RefIDColumn.Name].ToString();

                        List<PropertyInfo> lstProp = new List<PropertyInfo>(objSchedToPopulate.GetType().GetProperties());
                        foreach (PropertyInfo prop in lstProp.Where(x => x.Name == dr[FieldTagReferenceVM.Prop.RefIDColumn.Name].ToString()))
                            obj.Value = prop.PropertyType.Name != nameof(DateTime) ? prop.GetValue(objSchedToPopulate).ToString(): ((DateTime)prop.GetValue(objSchedToPopulate)).ToString(GetConfig("Schedule_StartDate", "DateTime")[0].Key3, new System.Globalization.CultureInfo(GetConfig("Schedule_StartDate", "DateTime")[0].Key4));
                     ListNotificationValues.Add(obj);
                    }
                }
                string ParamName = "ListNotificationValues";
                if (this.Request.Params[ParamName] != null)
                {
                    Dictionary<string, object>[] m_arrListNotVal = JSON.Deserialize<Dictionary<string, object>[]>(this.Request.Params[ParamName]);
                    if (m_arrListNotVal == null)
                        m_arrListNotVal = new List<Dictionary<string, object>>().ToArray();
                    foreach (Dictionary<string, object> m_dicNotVal in m_arrListNotVal)
                    {
                        TNotificationValues obj = new TNotificationValues();
                        obj.NotificationValueID = Guid.NewGuid().ToString().Replace("-", "");
                        obj.FieldTagID = m_dicNotVal[NotificationValuesVM.Prop.FieldTagID.Name].ToString();
                        obj.Value = m_dicNotVal[NotificationValuesVM.Prop.Value.Name].ToString();
                        //if(!string.IsNullOrEmpty(obj.Value))
                        ListNotificationValues.Add(obj);
                    }
                }

                List<RecipientsVM> ListRecipient = new List<RecipientsVM>();
                ParamName = "ListRecipient";
                if (this.Request.Params[ParamName] != null)
                {
                    Dictionary<string, object>[] m_arrListRecipient = JSON.Deserialize<Dictionary<string, object>[]>(this.Request.Params[ParamName]);
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
                        ListRecipient.Add(objRecipient);
                    }
                }

                List<NotificationAttachmentVM> ListAttachment = new List<NotificationAttachmentVM>();
                ParamName = "ListAttachment";
                if (this.Request.Params[ParamName] != null)
                {
                    Dictionary<string, object>[] m_arrListAttachment = JSON.Deserialize<Dictionary<string, object>[]>(this.Request.Params[ParamName]);
                    if (m_arrListAttachment == null)
                        m_arrListAttachment = new List<Dictionary<string, object>>().ToArray();
                    foreach (Dictionary<string, object> m_dicAttachment in m_arrListAttachment)
                    {
                        NotificationAttachmentVM objAttachment = new NotificationAttachmentVM();
                        objAttachment.Filename = m_dicAttachment[NotificationAttachmentVM.Prop.Filename.Name].ToString();
                        objAttachment.ContentType = m_dicAttachment[NotificationAttachmentVM.Prop.ContentType.Name].ToString();
                        objAttachment.RawData = m_dicAttachment[NotificationAttachmentVM.Prop.RawData.Name].ToString();
                        ListAttachment.Add(objAttachment);
                    }
                }

                //Validation
                m_lstMessage = IsSaveValid(objSchedule, m_pFunctionID, m_pTemplateID, ListRecipient);
                
                //BATCH
                objSchedule.MailNotificationID = isAdd?"": this.Request.Params[SchedulesVM.Prop.MailNotificationID.Name];
                if (objSchedule.IsBatchMail)
                { if ((isUpdate && ListMailNotif.Count > 1))
                    {
                        SaveBatch(isUpdate, isAdd, isUpdateFromBatchButNotBatch, m_pFunctionID, m_pTemplateID, objSchedule, ListMailNotif, ListRecipient, ListAttachment, ListNotificationValues, ref m_objMSchedulesDA, ref m_lstMessage, ref schedID, ref m_objDBConnection);
                        if (m_lstMessage.Count <= 0)
                        {
                            m_objMSchedulesDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
                            Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
                            if (isAdd || isUpdateFromBatchButNotBatch || objSchedule.IsBatchMail)
                                return Detail(General.EnumDesc(Buttons.ButtonSave), null, schedID);
                            else
                                return Detail(General.EnumDesc(Buttons.ButtonSave), null);
                        }
                    }
                }

                #region MTask & DTaskDetails 
                if (!m_bKeepTask)
                {
                    //Mtask
                    if (m_lstMessage.Count <= 0)
                    {
                        string messageErr = "";
                        MTasks m_objMTask = new MTasks();
                        m_objMTaskDA.Data = m_objMTask;
                        if (!isAdd)
                        {
                            m_objMTask.TaskID = this.Request.Params[SchedulesVM.Prop.TaskID.Name];
                            m_objMTaskDA.Select();
                        }
                        m_objMTask.TaskTypeID = General.EnumDesc(TaskType.Invitation_Schedules);
                        m_objMTask.TaskDateTimeStamp = DateTime.Now;
                        m_objMTask.TaskOwnerID = GetFirstApprovalRole(General.EnumDesc(TaskType.Invitation_Schedules), ref messageErr);// "PWR";//GetCurrentApproval(TaskType.ScheduleInvitation.ToString(),0);//TODO  get first level approval ownerid
                        m_objMTask.StatusID = (int)TaskStatus.Draft;
                        m_objMTask.CurrentApprovalLvl = 0;
                        m_objMTask.TaskDesc = isAdd ? "New From Invitation" : "Update From Invitation";
                        m_objMTask.Remarks = isAdd ? "New From Invitation" : "Update From Invitation";
                        m_objMTask.Summary = CreateSummary(ListRecipient, ListAttachment, objSchedule.Subject, objSchedule.StartDate.ToString(Global.ThreeWordMonthDateFormat));
                        if (messageErr.Length == 0)
                        {
                            if (isAdd)
                            {
                                m_objMTaskDA.Insert(true, m_objDBConnection);
                            }
                            else
                                m_objMTaskDA.Update(true, m_objDBConnection);

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
                        m_obTaskDetail.TaskID = isAdd ? m_objMTaskDA.Data.TaskID : this.Request.Params[SchedulesVM.Prop.TaskID.Name];
                        m_obTaskDetail.StatusID = (int)TaskStatus.Draft;
                        m_obTaskDetail.Remarks = "Draft Created Schedule Invitation";
                        m_objDTaskDetailsDA.Insert(true, m_objDBConnection);

                        if (!m_objDTaskDetailsDA.Success || m_objDTaskDetailsDA.Message != string.Empty)
                            m_lstMessage.Add(m_objDTaskDetailsDA.Message);

                    }
                }
                #endregion

                #region Notifications
                if (m_lstMessage.Count <= 0)
                {
                    string errMessage = "";
                    MMailNotifications objMailNotif = new MMailNotifications();
                    objMailNotif.MailNotificationID = isAdd ? Guid.NewGuid().ToString().Replace("-", "") : this.Request.Params[SchedulesVM.Prop.MailNotificationID.Name];
                    mailnotifID = objMailNotif.MailNotificationID;

                    m_objMMailNotifDA.Data = objMailNotif;
                    if (isUpdate)
                        m_objMMailNotifDA.Select();

                    objMailNotif.Importance = objSchedule.Priority == 1;//TODO
                    objMailNotif.Subject = objSchedule.Subject;
                    objMailNotif.StatusID = (int)NotificationStatus.Draft;
                    objMailNotif.FPTID = objSchedule.FPTID;
                    objMailNotif.MailNotificationID = mailnotifID;
                    objMailNotif.FunctionID = m_pFunctionID;
                    objMailNotif.NotificationTemplateID = m_pTemplateID;

                    objMailNotif.Contents = GenerateContent(objMailNotif.MailNotificationID, m_pTemplateID, ListRecipient, ListNotificationValues, ref errMessage); 
                    objMailNotif.TaskID = m_bKeepTask? this.Request.Params[SchedulesVM.Prop.TaskID.Name] : m_objMTaskDA.Data.TaskID;
                    m_taskID = objMailNotif.TaskID;

                    m_objMMailNotifDA.Data = objMailNotif;
                    if (isAdd)
                        m_objMMailNotifDA.Insert(true, m_objDBConnection);
                    else if (isUpdate)
                        m_objMMailNotifDA.Update(true, m_objDBConnection);

                    if (!m_objMMailNotifDA.Success || m_objMMailNotifDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMMailNotifDA.Message);

                }
                #endregion

                #region Recipient               
                if (m_lstMessage.Count <= 0)
                {
                    if (isUpdate)//Delete Insert
                    {
                        Dictionary<string, List<object>> ObjFilterNotificationRecipient = new Dictionary<string, List<object>>();
                        List<object> m_lstFilterNotificationRecipient = new List<object>();
                        m_lstFilterNotificationRecipient.Add(Operator.Equals);
                        m_lstFilterNotificationRecipient.Add(mailnotifID);
                        ObjFilterNotificationRecipient.Add(RecipientsVM.Prop.MailNotificationID.Map, m_lstFilterNotificationRecipient);
                        m_objDRecipientDA.DeleteBC(ObjFilterNotificationRecipient, true, m_objDBConnection);
                    }

                    if ((m_objDRecipientDA.Success && isUpdate) || isAdd)
                    {
                        foreach (RecipientsVM obj in ListRecipient)
                        {
                            DRecipients objDRecipient = new DRecipients();

                            objDRecipient.OwnerID = obj.OwnerID;
                            objDRecipient.RecipientID = obj.RecipientID;
                            objDRecipient.RecipientDesc = obj.RecipientDesc;
                            objDRecipient.RecipientTypeID = obj.RecipientTypeID;
                            objDRecipient.MailAddress = obj.MailAddress;
                            objDRecipient.MailNotificationID = mailnotifID;
                            m_objDRecipientDA.Data = objDRecipient;
                            m_objDRecipientDA.Insert(true, m_objDBConnection);                            
                        }
                    }
                    if (!m_objDRecipientDA.Success || m_objDRecipientDA.Message != string.Empty)
                        m_lstMessage.Add(m_objDRecipientDA.Message);
                }
                #endregion

                #region Attachment               
                if (m_lstMessage.Count <= 0 && ListAttachment.Count > 0)
                {
                    if (isUpdate)//Delete Insert
                    {
                        Dictionary<string, List<object>> ObjFilterNotificationAttachment = new Dictionary<string, List<object>>();
                        List<object> m_lstFilterNotificationAttachment = new List<object>();
                        m_lstFilterNotificationAttachment.Add(Operator.Equals);
                        m_lstFilterNotificationAttachment.Add(mailnotifID);
                        ObjFilterNotificationAttachment.Add(NotificationAttachmentVM.Prop.MailNotificationID.Map, m_lstFilterNotificationAttachment);
                        m_objTNotificationAttachment.DeleteBC(ObjFilterNotificationAttachment, true, m_objDBConnection);
                    }

                    if ((m_objTNotificationAttachment.Success && isUpdate) || isAdd)
                    {
                        foreach (NotificationAttachmentVM obj in ListAttachment)
                        {
                            TNotificationAttachments objAttachment = new TNotificationAttachments();
                            objAttachment.Filename = obj.Filename;
                            objAttachment.ContentType = obj.ContentType;
                            objAttachment.RawData = obj.RawData;
                            objAttachment.MailNotificationID = mailnotifID;
                            m_objTNotificationAttachment.Data = objAttachment;
                            m_objTNotificationAttachment.Insert(true, m_objDBConnection);
                        }
                    }
                    if (!m_objTNotificationAttachment.Success || m_objTNotificationAttachment.Message != string.Empty)
                        m_lstMessage.Add(m_objTNotificationAttachment.Message);
                }
                #endregion

                #region Schedule
                if (m_lstMessage.Count <= 0)
                {
                    MSchedules objScheduleToInsert = new MSchedules();
                    objScheduleToInsert.ScheduleID = schedID;
                    //schedID = objScheduleToInsert.ScheduleID;
                    
                    m_objMSchedulesDA.Data = objScheduleToInsert;
                    if (isUpdate) 
                        m_objMSchedulesDA.Select();
                    

                    objScheduleToInsert = objSchedule;
                    objScheduleToInsert.ScheduleID = schedID;
                    objScheduleToInsert.TaskID = isAdd && !m_bKeepTask? m_taskID : this.Request.Params[SchedulesVM.Prop.TaskID.Name];
                    objScheduleToInsert.MailNotificationID = mailnotifID;    
                    
                    m_objMSchedulesDA.Data = objScheduleToInsert;
                    if (isAdd)
                        m_objMSchedulesDA.Insert(true, m_objDBConnection);
                    else
                        m_objMSchedulesDA.Update(true, m_objDBConnection);

                    if (!m_objMSchedulesDA.Success || m_objMSchedulesDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMSchedulesDA.Message);
                }
                #endregion

                #region Parameter Values (TNotificationValues)
                if (m_lstMessage.Count <= 0)
                {
                    if (isUpdate)//Delete Insert
                    {
                        Dictionary<string, List<object>> ObjFilterNotificationValues = new Dictionary<string, List<object>>();
                        List<object> m_lstFilterNotificationValues = new List<object>();
                        m_lstFilterNotificationValues.Add(Operator.Equals);
                        m_lstFilterNotificationValues.Add(mailnotifID);
                        ObjFilterNotificationValues.Add(NotificationValuesVM.Prop.MailNotificationID.Map, m_lstFilterNotificationValues);
                        m_objTNotificationValues.DeleteBC(ObjFilterNotificationValues, true, m_objDBConnection);
                    }

                    if ((m_objTNotificationValues.Success && isUpdate) || isAdd)
                    {
                        foreach (var objNotVal in ListNotificationValues)
                        {
                            TNotificationValues objNotifValues = new TNotificationValues();
                            objNotifValues.NotificationValueID = Guid.NewGuid().ToString().Replace("-", "");
                            objNotifValues.MailNotificationID = mailnotifID;
                            objNotifValues.FieldTagID = objNotVal.FieldTagID;
                            objNotifValues.Value = objNotVal.Value;

                            m_objTNotificationValues.Data = objNotifValues;
                            m_objTNotificationValues.Insert(true, m_objDBConnection);

                            if (!m_objTNotificationValues.Success)
                                break;
                        }
                    }
                    if (!m_objTNotificationValues.Success || m_objTNotificationValues.Message != string.Empty)
                        m_lstMessage.Add(m_objTNotificationValues.Message);
                }
                #endregion
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                m_objMSchedulesDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
            }
            if (m_lstMessage.Count <= 0)
            {

                m_objMSchedulesDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
                Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
                return Detail(General.EnumDesc(Buttons.ButtonSave), null, schedID);
                
            }
            m_objMSchedulesDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);
        }
        public ActionResult Preview()
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));
            string message = "";
            MFieldTagReferencesDA tagrefDA = new MFieldTagReferencesDA();
            tagrefDA.ConnectionStringName = Global.ConnStrConfigName;

            MSchedules objSchedule = new MSchedules();
            DateTime m_StartDate = DateTime.Parse(this.Request.Params[SchedulesVM.Prop.StartDate.Name]);
            TimeSpan m_StartDateHour = TimeSpan.Parse((DateTime.Parse(this.Request.Params["StartDateHour"].ToString())).ToString("HH:mm"));
            objSchedule.StartDate = m_StartDate.Add(m_StartDateHour);
            DateTime m_EndDate = DateTime.Parse(this.Request.Params[SchedulesVM.Prop.EndDate.Name]);
            TimeSpan m_EndDateHour = TimeSpan.Parse((DateTime.Parse(this.Request.Params["EndDateHour"].ToString())).ToString("HH:mm"));
            objSchedule.EndDate = m_EndDate.Add(m_EndDateHour);            
            objSchedule.Subject = this.Request.Params[SchedulesVM.Prop.Subject.Name];
            objSchedule.Notes = this.Request.Params[SchedulesVM.Prop.Notes.Name];
            objSchedule.ProjectID = this.Request.Params[SchedulesVM.Prop.ProjectDesc.Name];
            objSchedule.ClusterID = this.Request.Params[SchedulesVM.Prop.ClusterDesc.Name];
            objSchedule.Weblink = this.Request.Params[SchedulesVM.Prop.Weblink.Name];
            objSchedule.Location = this.Request.Params[SchedulesVM.Prop.Location.Name];
            objSchedule.Priority = Convert.ToInt16(this.Request.Params[SchedulesVM.Prop.Priority.Name]);
            objSchedule.IsAllDay = Convert.ToBoolean(this.Request.Params[SchedulesVM.Prop.IsAllDay.Name]);
            string ProjectDesc = this.Request.Params[SchedulesVM.Prop.ProjectDesc.Name];
            string ClusterDesc = this.Request.Params[SchedulesVM.Prop.ClusterDesc.Name];
            string m_TemplateID = this.Request.Params[SchedulesVM.Prop.NotificationTemplateID.Name];
            if (string.IsNullOrEmpty(m_TemplateID))
            {
               Global.ShowErrorAlert("Error Preview", "Please choose template");
               return this.Direct(true);
            }
            List<TNotificationValues> ListNotificationValues = new List<TNotificationValues>();
            List<RecipientsVM> ListRecipient_ = new List<RecipientsVM>();
            List<FieldTagReferenceVM> TagRefList = new List<FieldTagReferenceVM>();
            List<string> m_lstSelect = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();           
            m_lstSelect.Add(FieldTagReferenceVM.Prop.FieldTagID.MapAlias);
            m_lstSelect.Add(FieldTagReferenceVM.Prop.RefTable.MapAlias);
            m_lstSelect.Add(FieldTagReferenceVM.Prop.RefIDColumn.MapAlias);            
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("MSchedules");
            m_objFilter.Add(FieldTagReferenceVM.Prop.RefTable.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicTagRefDA = tagrefDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (tagrefDA.Success && tagrefDA.Message == "")
            {
                SchedulesVM objSchedToPopulate = new SchedulesVM();
                objSchedToPopulate.StartDate = objSchedule.StartDate;
                objSchedToPopulate.EndDate = objSchedule.EndDate;
                objSchedToPopulate.Subject = objSchedule.Subject;
                objSchedToPopulate.Location = objSchedule.Location;
                objSchedToPopulate.Weblink = objSchedule.Weblink;
                objSchedToPopulate.ProjectDesc = ProjectDesc;
                objSchedToPopulate.ClusterDesc = ClusterDesc;
                objSchedToPopulate.Notes = objSchedule.Notes;
                objSchedToPopulate.IsAllDay = objSchedule.IsAllDay;
                objSchedToPopulate.EndTime = objSchedule.EndDate.ToString(Global.ShortTimeFormat);
                objSchedToPopulate.StartTime = objSchedule.StartDate.ToString(Global.ShortTimeFormat);
                objSchedToPopulate.Priority = objSchedule.Priority;

                foreach (DataRow dr in m_dicTagRefDA[0].Tables[0].Rows)
                {
                    TNotificationValues obj = new TNotificationValues();
                    obj.NotificationValueID = Guid.NewGuid().ToString().Replace("-", "");
                    obj.FieldTagID = dr[FieldTagReferenceVM.Prop.FieldTagID.Name].ToString();
                    string refColID = dr[FieldTagReferenceVM.Prop.RefIDColumn.Name].ToString();

                    List<PropertyInfo> lstProp = new List<PropertyInfo>(objSchedToPopulate.GetType().GetProperties());
                    foreach (PropertyInfo prop in lstProp.Where(x => x.Name == dr[FieldTagReferenceVM.Prop.RefIDColumn.Name].ToString()))
                        obj.Value = prop.PropertyType.Name != nameof(DateTime) ? prop.GetValue(objSchedToPopulate).ToString() : ((DateTime)prop.GetValue(objSchedToPopulate)).ToString(GetConfig("Schedule_StartDate", "DateTime")[0].Key3, new System.Globalization.CultureInfo(GetConfig("Schedule_StartDate", "DateTime")[0].Key4));
                    ListNotificationValues.Add(obj);
                }
            }

            string ParamName = "ListNotificationValues";
            if (this.Request.Params[ParamName] != null)
            {
                Dictionary<string, object>[] m_arrListNotVal = JSON.Deserialize<Dictionary<string, object>[]>(this.Request.Params[ParamName]);
                if (m_arrListNotVal == null)
                    m_arrListNotVal = new List<Dictionary<string, object>>().ToArray();
                foreach (Dictionary<string, object> m_dicNotVal in m_arrListNotVal)
                {
                    TNotificationValues obj = new TNotificationValues();
                    obj.NotificationValueID = Guid.NewGuid().ToString().Replace("-", "");
                    obj.FieldTagID = m_dicNotVal[NotificationValuesVM.Prop.FieldTagID.Name].ToString();
                    obj.Value = m_dicNotVal[NotificationValuesVM.Prop.Value.Name].ToString();
                    //if(!string.IsNullOrEmpty(obj.Value))
                    ListNotificationValues.Add(obj);
                }
            };
            ParamName = "ListRecipient";
            if (this.Request.Params[ParamName] != null)
            {
                Dictionary<string, object>[] m_arrListNotVal = JSON.Deserialize<Dictionary<string, object>[]>(this.Request.Params[ParamName]);
                if (m_arrListNotVal == null)
                    m_arrListNotVal = new List<Dictionary<string, object>>().ToArray();
                foreach (Dictionary<string, object> m_dicNotVal in m_arrListNotVal)
                {
                    RecipientsVM obj = new RecipientsVM();
                    obj.RecipientDesc = m_dicNotVal[RecipientsVM.Prop.RecipientDesc.Name].ToString();
                    ListRecipient_.Add(obj);
                }
            };

            var m_listTag = new Dictionary<string, string>();
            if (ListNotificationValues != null)
            {
                foreach (var item in ListNotificationValues)
                {
                    m_listTag.Add(item.FieldTagID, item.Value);
                }
            }
            string Content = "";
            try
            {
                NotificationTemplateVM ResultTemplateContent = GetNotificationTemplateVM(m_TemplateID);
                Content = Global.ParseParameter(ResultTemplateContent.Contents, m_listTag, ListRecipient_,ref message); //GenerateContent(ListNotificationValues, m_TemplateID,ref message);
            }
            catch (Exception e)
            {
                message += "Error While Get Notification Template Content \n"+ e.Message;
                Global.ShowErrorAlert("Error Preview", message);
                return this.Direct(false);
            }

            if (tagrefDA.Success && string.IsNullOrEmpty(tagrefDA.Message))
            {
                ViewDataDictionary m_vdd = new ViewDataDictionary();
                m_vdd.Add("content", Content);
                return new XMVC.PartialViewResult
                {
                    RenderMode = RenderMode.Auto,
                    WrapByScriptTag = false,
                    ViewData = m_vdd,
                    ViewName = "../MyTask/_HTMLpreview"
                };
            }
            else
            {
                message +="\n "+ tagrefDA.Message;
                Global.ShowErrorAlert("Error Preview", "Error Preview " + message);
                return this.Direct(false);
            }
        }
        public ActionResult EditValue(string RowID)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));
            string message = "";

            ViewDataDictionary m_vdd = new ViewDataDictionary();
            m_vdd.Add("RowID", RowID);
            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vdd,
                ViewName = "../Scheduling/_EditValue"
            };

        }

        public ActionResult Verify(string Selected)
        {


            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Verify)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            SchedulesVM SCHD = null;
            //bool IsBatch = false;
            string TaskID = "";
            string TaskOwnerID = "";
            string TaskTypeID = "";
            string FPTID = "";
            string StatusID = "";
            string ScheduleID = "";
            string MailNotificationID = "";
            string NotificationTemplateID = "";
            string message_ = "";
            bool isBatch = false;
            List<RecipientsVM> lstRecipient = new List<RecipientsVM>();
            List<string> m_lstMessage = new List<string>();
            List<SchedulesVM> m_lstSelectedRow = new List<SchedulesVM>();
            try
            {
                m_lstSelectedRow = JSON.Deserialize<List<SchedulesVM>>(Selected);
                TaskID = m_lstSelectedRow[0].TaskID;
                TaskOwnerID = m_lstSelectedRow[0].TaskOwnerID;
                TaskTypeID = m_lstSelectedRow[0].TaskTypeID;
                FPTID = m_lstSelectedRow[0].FPTID;
                StatusID = m_lstSelectedRow[0].StatusID;
                ScheduleID = m_lstSelectedRow[0].ScheduleID;
                MailNotificationID = m_lstSelectedRow[0].MailNotificationID;
                NotificationTemplateID = m_lstSelectedRow[0].NotificationTemplateID;
                SCHD = GetSelectedData(null, ref message_, ScheduleID);
                isBatch = Convert.ToBoolean(m_lstSelectedRow[0].IsBatchMail.ToString());
                lstRecipient = GetListRecipientsVM(MailNotificationID, ref message_);
                if (!string.IsNullOrEmpty(message_))
                    return this.Direct(true, message_);

            }
            catch (Exception e)
            {
                m_lstMessage.Add(e.Message);
                return this.Direct(true, String.Join(Global.NewLineSeparated, m_lstMessage));
            }

            // string currentApprovalRole = GetFirstApprovalRole(General.EnumDesc(TaskType.ScheduleInvitation), ref message_);
            string ParentApproval = GetParentApproval(ref message_, TaskOwnerID, TaskTypeID);
            if (!string.IsNullOrEmpty(message_))
                m_lstMessage.Add(message_);

            MTasksDA m_objMTasksDA = new MTasksDA();
            MTasks m_objMTask = new MTasks();
            m_objMTasksDA.ConnectionStringName = Global.ConnStrConfigName;

            DTaskDetailsDA m_objDTaskDetailsDA = new DTaskDetailsDA();
            DTaskDetails m_obTaskDetail = new DTaskDetails();
            m_objDTaskDetailsDA.ConnectionStringName = Global.ConnStrConfigName;

            MSchedulesDA m_objMScheduleDA = new MSchedulesDA();
            MSchedules m_objMSchedule = new MSchedules();
            m_objMScheduleDA.ConnectionStringName = Global.ConnStrConfigName;

            MMailNotificationsDA m_objNotificationDA = new MMailNotificationsDA();
            MMailNotifications m_objNotification = new MMailNotifications();
            m_objNotificationDA.ConnectionStringName = Global.ConnStrConfigName;

            object m_objConnection = null;
            string m_strTransactionName = "TransactionVerifyEntry";
            m_objConnection = m_objMTasksDA.BeginTrans(m_strTransactionName);
            try
            {
                List<string> ListMailNotif = new List<string>();
                if (isBatch)
                {
                    List<string> m_lstSelect_ = new List<string>();
                    m_lstSelect_.Add(MailNotificationsVM.Prop.MailNotificationID.MapAlias);

                    Dictionary<string, List<object>> m_objFilter_ = new Dictionary<string, List<object>>();
                    List<object> m_lstFilter_ = new List<object>();
                    m_lstFilter_.Add(Operator.Equals);
                    m_lstFilter_.Add(TaskID);
                    m_objFilter_.Add(MailNotificationsVM.Prop.TaskID.Map, m_lstFilter_);

                    Dictionary<int, DataSet> m_dicMailNotif = m_objNotificationDA.SelectBC(0, null, false, m_lstSelect_, m_objFilter_, null, null, null);
                    if (m_objNotificationDA.Success && m_objNotificationDA.Message == "")
                    {
                        foreach (DataRow dr in m_dicMailNotif[0].Tables[0].Rows)
                            ListMailNotif.Add(dr[MailNotificationsVM.Prop.MailNotificationID.Name].ToString());
                    }
                    else
                        m_lstMessage.Add("Error while Get List MailNotificationID");
                }

                #region MTask
                m_objMTasksDA.Data = m_objMTask;
                m_objMTask.TaskID = TaskID;
                m_objMTasksDA.Select();
                m_objMTask.TaskOwnerID = ParentApproval;
                m_objMTask.TaskDateTimeStamp = DateTime.Now;
                m_objMTask.StatusID = (int)TaskStatus.InProgress;
                m_objMTask.TaskTypeID = TaskTypeID;
                m_objMTask.CurrentApprovalLvl = 1;
                m_objMTask.Remarks = "Invitation Schedule";

                if (m_lstMessage.Count <= 0)
                    m_objMTasksDA.Update(true, m_objConnection);
                #endregion

                #region DTaskDetailDA
                m_objDTaskDetailsDA.Data = m_obTaskDetail;
                m_obTaskDetail.TaskDetailID = Guid.NewGuid().ToString().Replace("-", "");
                m_obTaskDetail.TaskID = m_lstSelectedRow[0].TaskID;
                m_obTaskDetail.StatusID = (int)TaskDetailStatus.Submit;
                m_obTaskDetail.Remarks = "Submit Invitation Schedule";

                if (string.IsNullOrEmpty(m_objMTasksDA.Message))
                    m_objDTaskDetailsDA.Insert(true, m_objConnection);
                else
                    m_lstMessage.Add(m_objMTasksDA.Message);
                #endregion

                #region Get Schedules By Task 
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(SchedulesVM.Prop.ScheduleID.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.TaskID.MapAlias);
                m_lstSelect.Add(SchedulesVM.Prop.MailNotificationID.MapAlias);

                Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.In);
                m_lstFilter.Add(TaskID);
                m_objFilter.Add(SchedulesVM.Prop.TaskID.Map, m_lstFilter);

                Dictionary<int, DataSet> m_dicMSchedulesDA = m_objMScheduleDA.SelectBC(0,null,false, m_lstSelect, m_objFilter,null,null,null);
                List<SchedulesVM> m_lstSchedulesVM = new List<SchedulesVM>();
                if (m_objMScheduleDA.Success && m_objMScheduleDA.Message == "")
                {
                    m_lstSchedulesVM = (from DataRow dr in m_dicMSchedulesDA[0].Tables[0].Rows
                                        select new SchedulesVM
                                        {
                                            ScheduleID = dr[SchedulesVM.Prop.ScheduleID.Name].ToString(),
                                            MailNotificationID = dr[SchedulesVM.Prop.MailNotificationID.Name].ToString()
                                        }).ToList();
                }

                #endregion

                foreach (SchedulesVM objSchedulesVM in m_lstSchedulesVM)
                {
                    #region MSchedule
                    m_objMSchedule.FPTID = FPTID;
                    m_objMSchedule.ScheduleID = objSchedulesVM.ScheduleID;
                    m_objMScheduleDA.Data = m_objMSchedule;
                    m_objMScheduleDA.Select();
                    //if (m_objMScheduleDA.Data.IsBatchMail)
                    //{
                    //    IsBatch = true;               

                    //}
                    m_objMScheduleDA.Data.StatusID = "";
                    switch (StatusID)
                    {
                        case "0"://Draft -> Submitted
                            m_objMScheduleDA.Data.StatusID = ((int)ScheduleStatus.Submitted).ToString();
                            break;
                        case "3"://Draft-Reschedule -> Submitted-Reschedule
                            m_objMScheduleDA.Data.StatusID = ((int)ScheduleStatus.Submitted_Reschedule).ToString();
                            break;
                        case "4"://Draft-Cancellation -> Submitted-Cancellation
                            m_objMScheduleDA.Data.StatusID = ((int)ScheduleStatus.Submitted_Cancellation).ToString();
                            break;
                    }

                    if (string.IsNullOrEmpty(m_objDTaskDetailsDA.Message) && !m_lstMessage.Any())
                        m_objMScheduleDA.Update(true, m_objConnection);
                    else
                        m_lstMessage.Add(m_objDTaskDetailsDA.Message);
                    #endregion

                    #region MailNotification

                    if (isBatch)
                    {
                        foreach (string mailnotifid_ in ListMailNotif)
                        {
                            m_objNotificationDA.Data = new MMailNotifications();
                            m_objNotificationDA.Data.MailNotificationID = mailnotifid_;
                            m_objNotificationDA.Select();
                            if (m_objNotificationDA.Success && string.IsNullOrEmpty(m_objNotificationDA.Message))
                            {
                                m_objNotificationDA.Data.StatusID = (int)NotificationStatus.Verified;
                                //m_objNotificationDA.Data.Contents = GenerateContent(mailnotifid_, NotificationTemplateID, lstRecipient, ref message_);
                                if (message_.Length > 0)
                                {
                                    m_objMTasksDA.EndTrans(ref m_objConnection, false, m_strTransactionName);
                                    Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
                                    return this.Direct(false, String.Join(Global.NewLineSeparated, m_lstMessage));
                                }
                                m_objNotificationDA.Update(true, m_objConnection);
                            }
                        }
                    }
                    else
                    {
                        m_objNotificationDA.Data.MailNotificationID = objSchedulesVM.MailNotificationID;
                        m_objNotificationDA.Select();
                        if (m_objNotificationDA.Success && string.IsNullOrEmpty(m_objNotificationDA.Message))
                        {
                            m_objNotificationDA.Data.StatusID = (int)NotificationStatus.Verified;
                            //m_objNotificationDA.Data.Contents = GenerateContent(MailNotificationID, NotificationTemplateID, lstRecipient, ref message_);
                            if (message_.Length > 0)
                            {
                                m_objMTasksDA.EndTrans(ref m_objConnection, false, m_strTransactionName);
                                Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
                                return this.Direct(false, String.Join(Global.NewLineSeparated, m_lstMessage));
                            }
                            m_objNotificationDA.Update(true, m_objConnection);
                        }
                    }

                    #endregion
                }




            }
            catch (Exception e)
            {
                m_objMTasksDA.EndTrans(ref m_objConnection, false, m_strTransactionName);
                Global.ShowErrorAlert(title, e.Message);
                return this.Direct(false, e.Message);
            }
            if (!m_objNotificationDA.Success || !m_objMScheduleDA.Success || !m_objMTasksDA.Success || !string.IsNullOrEmpty(m_objMTasksDA.Message) || m_lstMessage.Count > 0)
            {
                m_objMTasksDA.EndTrans(ref m_objConnection, false, m_strTransactionName);
                Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
                return this.Direct(false, String.Join(Global.NewLineSeparated, m_lstMessage));
            }
            else
            {
                m_objMTasksDA.EndTrans(ref m_objConnection, true, m_strTransactionName);
                Global.ShowInfoAlert(title, "Verified");
                return this.Direct(true, string.Empty);
            }
        }
        public ActionResult Sync()
        {
            
            int m_intadded = 0;
            int m_intupdated = 0;
            int m_intfailed = 0;
            string m_soapenv = @"<soap:Envelope xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/'>
              <soap:Body>
                <Division xmlns='http://tempuri.org/' />
              </soap:Body>
            </soap:Envelope>";

            
            var m_lstconfig = GetConfig("WS", null, "ETT");
            if (!m_lstconfig.Any())
            {
                return this.Direct(false);
            }
            string m_struser = m_lstconfig.Any(x => x.Key2 == "Credential" && x.Key4 == "User") ? m_lstconfig.Where(x => x.Key2 == "Credential" && x.Key4 == "User").FirstOrDefault().Desc1 : string.Empty;
            string m_strpass = m_lstconfig.Any(x => x.Key2 == "Credential" && x.Key4 == "Password") ? m_lstconfig.Where(x => x.Key2 == "Credential" && x.Key4 == "Password").FirstOrDefault().Desc1 : string.Empty;
            string m_strdomain = m_lstconfig.Any(x => x.Key2 == "Credential" && x.Key4 == "Domain") ? m_lstconfig.Where(x => x.Key2 == "Credential" && x.Key4 == "Domain").FirstOrDefault().Desc1 : string.Empty;
            string m_strurl = m_lstconfig.Any(x => x.Key2 == "URL" && x.Key4 == "Division") ? m_lstconfig.Where(x => x.Key2 == "URL" && x.Key4 == "Division").FirstOrDefault().Desc1 : string.Empty;
            NetworkCredential m_credential = new NetworkCredential(m_struser, m_strpass, m_strdomain);
            string m_strsoapresult = GetSOAPString(m_soapenv, m_strurl, m_credential);
            if (string.IsNullOrEmpty(m_strsoapresult))
            {
                return this.Direct(false);
            }

            XmlDocument document = new XmlDocument();
            document.LoadXml(m_strsoapresult);
            XmlNamespaceManager manager = new XmlNamespaceManager(document.NameTable);

            XmlNodeList xnList = document.ChildNodes[1].ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes;
            List<MDivision> m_lstDivision = new List<MDivision>();

            foreach (XmlNode item in xnList)
            {
                MDivision m_Division = new MDivision();
                m_Division.DivisionID = item["ID"].InnerText;
                m_Division.DivisionDesc = item["BusinessAreaName"].InnerText;
                //m_Division.BusinessUnitID = item["FIDProjectLocation"].InnerText;
                m_lstDivision.Add(m_Division);
            }

            if (!m_lstDivision.Any())
            {
                return this.Direct(false, "Error Read Data");
            }
            else
            {
                MDivisionDA m_objMDivisionDA = new MDivisionDA();
                m_objMDivisionDA.ConnectionStringName = Global.ConnStrConfigName;
                foreach (var item in m_lstDivision)
                {
                    try
                    {
                        
                            m_objMDivisionDA.Data = item;
                            m_objMDivisionDA.Insert(false);
                            if (!m_objMDivisionDA.Success || m_objMDivisionDA.Message != string.Empty)
                                                        {
                            if (m_objMDivisionDA.Message == "Cannot insert duplicate data.")
                            {
                                m_intupdated += 1;
                            }
                            else
                            {
                                m_intfailed += 1;
                            }
                        }
                        else
                        {
                            m_intadded += 1;
                        }
                    }
                    catch (Exception ex)
                    {
                        m_intfailed += 1;
                    }
                    }
                Global.ShowInfoAlert(title, $"Total : {m_lstDivision.Count}  New : {m_intadded} Exist : {m_intupdated} Failed: {m_intfailed}");
                return this.Direct(true);
            }

        }

        #endregion

        #region Direct Method

        /*public ActionResult GetDivision(string ControlDivisionID, string ControlDivisionDesc, string FilterDivisionID, string FilterDivisionDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<DivisionVM>> m_dicDivisionData = GetDivisionData(true, FilterDivisionID, FilterDivisionDesc);
                KeyValuePair<int, List<DivisionVM>> m_kvpDivisionVM = m_dicDivisionData.AsEnumerable().ToList()[0];
                if (m_kvpDivisionVM.Key < 1 || (m_kvpDivisionVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpDivisionVM.Key > 1 && !Exact)
                    return Browse(ControlDivisionID, ControlDivisionDesc, FilterDivisionID, FilterDivisionDesc);

                m_dicDivisionData = GetDivisionData(false, FilterDivisionID, FilterDivisionDesc);
                DivisionVM m_objDivisionVM = m_dicDivisionData[0][0];
                this.GetCmp<TextField>(ControlDivisionID).Value = m_objDivisionVM.DivisionID;
                this.GetCmp<TextField>(ControlDivisionDesc).Value = m_objDivisionVM.DivisionDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }*/

        #endregion

        #region Private Method

        private List<string> IsSaveValid(MSchedules ObjSchedule,string FunctionID,string TemplateID,List<RecipientsVM> RecipientList)
        {
            List<string> m_lstReturn = new List<string>();
            if (FunctionID == string.Empty)
                m_lstReturn.Add(SchedulesVM.Prop.FunctionID.Desc+ " " + General.EnumDesc(MessageLib.mustFill));
            if (TemplateID == string.Empty)
                m_lstReturn.Add(SchedulesVM.Prop.NotificationTemplateID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (ObjSchedule.Subject == string.Empty)
                m_lstReturn.Add(SchedulesVM.Prop.Subject.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (ObjSchedule.StartDate > ObjSchedule.EndDate)
                m_lstReturn.Add(SchedulesVM.Prop.EndDate.Desc + " " + General.EnumDesc(MessageLib.invalid));
            if (ObjSchedule.ProjectID == string.Empty)
                m_lstReturn.Add(SchedulesVM.Prop.ProjectDesc.Desc + General.EnumDesc(MessageLib.mustFill));
            if (RecipientList.Count<=0)
                m_lstReturn.Add("Recipient at least 1");
            return m_lstReturn;
        }
        public string CreateSummary(List<RecipientsVM> lstRecipient, List<NotificationAttachmentVM> lstNotificationAttachment, string Subject, string ScheduleDate)
        {
            string Summary = "Subject : \n";
            Summary += " -" + Subject + "\n \n" ;
            Summary += "Schedule Date : \n";
            Summary += " -" + ScheduleDate + "\n \n";
            Summary += "Recipients : \n";

            foreach (RecipientsVM recipient in lstRecipient.Where(f=>f.RecipientTypeID == ((int)RecipientTypes.TO).ToString()))
                Summary+= " - To : " + recipient.MailAddress + "\n" ;
            foreach (RecipientsVM recipient in lstRecipient.Where(f => f.RecipientTypeID == ((int)RecipientTypes.CC).ToString()))
                Summary += " - Cc : " + recipient.MailAddress + "\n";
            foreach (RecipientsVM recipient in lstRecipient.Where(f => f.RecipientTypeID == ((int)RecipientTypes.BCC).ToString()))
                Summary += " - Bcc : " + recipient.MailAddress + "\n";
            Summary += "\n";

            Summary += "Attachment (" + lstNotificationAttachment.Count + ") \n";
            int noAttachment = 1;
            foreach (NotificationAttachmentVM attachment in lstNotificationAttachment)
                Summary += noAttachment + ". " + attachment.Filename+"\n";

            return Summary;
        }
        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(SchedulesVM.Prop.ScheduleID.Name, parameters[SchedulesVM.Prop.ScheduleID.Name]);
            m_dicReturn.Add(SchedulesVM.Prop.TaskID.Name, parameters[SchedulesVM.Prop.TaskID.Name]);
            m_dicReturn.Add(SchedulesVM.Prop.FPTID.Name, parameters[SchedulesVM.Prop.FPTID.Name]);
            m_dicReturn.Add(SchedulesVM.Prop.StatusID.Name, parameters[SchedulesVM.Prop.StatusID.Name]);
            m_dicReturn.Add(SchedulesVM.Prop.FunctionID.Name, parameters[SchedulesVM.Prop.FunctionID.Name]);
            m_dicReturn.Add(SchedulesVM.Prop.NotificationTemplateID.Name, parameters[SchedulesVM.Prop.NotificationTemplateID.Name]);
            m_dicReturn.Add(SchedulesVM.Prop.StartDate.Name, parameters[SchedulesVM.Prop.StartDate.Name]);
            m_dicReturn.Add(SchedulesVM.Prop.EndDate.Name, parameters[SchedulesVM.Prop.EndDate.Name]);
            m_dicReturn.Add(SchedulesVM.Prop.Subject.Name, parameters[SchedulesVM.Prop.Subject.Name]);
            m_dicReturn.Add(SchedulesVM.Prop.IsAllDay.Name, parameters[SchedulesVM.Prop.IsAllDay.Name]);
            m_dicReturn.Add(SchedulesVM.Prop.Notes.Name, parameters[SchedulesVM.Prop.Notes.Name]);
            m_dicReturn.Add(SchedulesVM.Prop.Weblink.Name, parameters[SchedulesVM.Prop.Weblink.Name]);
            m_dicReturn.Add(SchedulesVM.Prop.Priority.Name, parameters[SchedulesVM.Prop.Priority.Name]);
            m_dicReturn.Add(SchedulesVM.Prop.Location.Name, parameters[SchedulesVM.Prop.Location.Name]);
            m_dicReturn.Add(SchedulesVM.Prop.MailNotificationID.Name, parameters[SchedulesVM.Prop.MailNotificationID.Name]);


            return m_dicReturn;
        }
        private void SaveBatch(bool isUpdate, bool isAdd,bool isUpdateFromBatchbutNotBatch,string m_pFunctionID, string m_pTemplateID, MSchedules objSchedule, List<string> ListMailNotificationID, List<RecipientsVM> ListRecipient, List<NotificationAttachmentVM> ListAttachment, List<TNotificationValues> ListNotificationValues,ref MSchedulesDA m_objMSchedulesDA, ref List<string> m_lstMessage, ref string schedID,ref object m_objDBConnection)
        {
            DNotificationMapDA m_objNotificationMapDA = new DNotificationMapDA();
            m_objNotificationMapDA.ConnectionStringName = Global.ConnStrConfigName;

            MFieldTagReferencesDA tagrefDA = new MFieldTagReferencesDA();
            tagrefDA.ConnectionStringName = Global.ConnStrConfigName;

            MTasksDA m_objMTaskDA = new MTasksDA();
            m_objMTaskDA.ConnectionStringName = Global.ConnStrConfigName;

            DTaskDetailsDA m_objDTaskDetailsDA = new DTaskDetailsDA();
            m_objDTaskDetailsDA.ConnectionStringName = Global.ConnStrConfigName;

            MMailNotificationsDA m_objMMailNotifDA = new MMailNotificationsDA();
            m_objMMailNotifDA.ConnectionStringName = Global.ConnStrConfigName;

            DRecipientsDA m_objDRecipientDA = new DRecipientsDA();
            m_objDRecipientDA.ConnectionStringName = Global.ConnStrConfigName;

            TNotificationValuesDA m_objTNotificationValues = new TNotificationValuesDA();
            m_objTNotificationValues.ConnectionStringName = Global.ConnStrConfigName;

            TNotificationAttachmentsDA m_objTNotificationAttachment = new TNotificationAttachmentsDA();
            m_objTNotificationAttachment.ConnectionStringName = Global.ConnStrConfigName;

            List<RecipientsVM> ListTORecipient = new List<RecipientsVM>();
            foreach (RecipientsVM obj in ListRecipient.Where(r => r.RecipientTypeID == ((int)RecipientTypes.TO).ToString()))
                ListTORecipient.Add(obj);        


            try
            {
                #region Delete First If Update
                if (isUpdate)//Delete Insert
                {
                    Dictionary<string, List<object>> ObjFilterScheduleID = new Dictionary<string, List<object>>();
                    List<object> m_lstFilterSchedules = new List<object>();
                    m_lstFilterSchedules.Add(Operator.Equals);
                    m_lstFilterSchedules.Add(schedID);
                    ObjFilterScheduleID.Add(SchedulesVM.Prop.ScheduleID.Map, m_lstFilterSchedules);
                    m_objMSchedulesDA.DeleteBC(ObjFilterScheduleID, true, m_objDBConnection);
                    if (!m_objMSchedulesDA.Success)
                        m_lstMessage.Add("Error Delete Schedule ID");

                    foreach (string MailNotif in ListMailNotificationID)
                    {
                        if (!m_lstMessage.Any())
                        {
                            Dictionary<string, List<object>> ObjFilterNotificationRecipient = new Dictionary<string, List<object>>();
                            List<object> m_lstFilterNotificationRecipient = new List<object>();
                            m_lstFilterNotificationRecipient.Add(Operator.Equals);
                            m_lstFilterNotificationRecipient.Add(MailNotif);
                            ObjFilterNotificationRecipient.Add(RecipientsVM.Prop.MailNotificationID.Map, m_lstFilterNotificationRecipient);
                            m_objDRecipientDA.DeleteBC(ObjFilterNotificationRecipient, true, m_objDBConnection);
                            if (!m_objDRecipientDA.Success)
                                m_lstMessage.Add("Error Delete Recipient");

                            Dictionary<string, List<object>> ObjFilterNotificationAttachment = new Dictionary<string, List<object>>();
                            List<object> m_lstFilterNotificationAttachment = new List<object>();
                            m_lstFilterNotificationAttachment.Add(Operator.Equals);
                            m_lstFilterNotificationAttachment.Add(MailNotif);
                            ObjFilterNotificationAttachment.Add(NotificationAttachmentVM.Prop.MailNotificationID.Map, m_lstFilterNotificationAttachment);
                            m_objTNotificationAttachment.DeleteBC(ObjFilterNotificationAttachment, true, m_objDBConnection);
                            if (!m_objTNotificationAttachment.Success)
                                m_lstMessage.Add("Error Delete Attachment");

                            Dictionary<string, List<object>> ObjFilterNotificationValues = new Dictionary<string, List<object>>();
                            List<object> m_lstFilterNotificationValues = new List<object>();
                            m_lstFilterNotificationValues.Add(Operator.Equals);
                            m_lstFilterNotificationValues.Add(MailNotif);
                            ObjFilterNotificationValues.Add(NotificationValuesVM.Prop.MailNotificationID.Map, m_lstFilterNotificationValues);
                            m_objTNotificationValues.DeleteBC(ObjFilterNotificationValues, true, m_objDBConnection);
                            if (!m_objTNotificationValues.Success)
                                m_lstMessage.Add("Error Delete Parameter Value");
                            

                            Dictionary<string, List<object>> ObjFilterMailNotif = new Dictionary<string, List<object>>();
                            List<object> m_lstFilterNotification = new List<object>();
                            m_lstFilterNotification.Add(Operator.Equals);
                            m_lstFilterNotification.Add(MailNotif);
                            ObjFilterMailNotif.Add(MailNotificationsVM.Prop.MailNotificationID.Map, m_lstFilterNotification);
                            m_objMMailNotifDA.Data.MailNotificationID = MailNotif;
                            m_objMMailNotifDA.Delete(true, m_objDBConnection);
                            if (!m_objMMailNotifDA.Success)
                                m_lstMessage.Add("Error Delete MailNotification");
                        }
                        else
                            break;
                    }
                }

                #endregion              

                ListMailNotificationID = new List<string>();
                foreach (RecipientsVM Recipient in ListRecipient.Where(r => r.RecipientTypeID == ((int)RecipientTypes.TO).ToString()))
                    ListMailNotificationID.Add(Guid.NewGuid().ToString().Replace("-", ""));


                List<List<RecipientsVM>> ListOfListRecipient = new List<List<RecipientsVM>>();
                foreach (RecipientsVM Recipient in ListRecipient.Where(r => r.RecipientTypeID == ((int)RecipientTypes.TO).ToString()))
                {
                    List<RecipientsVM> lstR = new List<RecipientsVM>();
                    lstR = ListRecipient.Where(r => r.RecipientTypeID != ((int)RecipientTypes.TO).ToString()).ToList();
                    lstR.Add(Recipient);
                    ListOfListRecipient.Add(lstR);
                }

                if (isUpdateFromBatchbutNotBatch)
                {
                    ListMailNotificationID = new List<string>();
                    ListMailNotificationID.Add(Guid.NewGuid().ToString().Replace("-", ""));
                }
                #region Task & History 
                //Mtask
                string m_taskID = "";
                if (m_lstMessage.Count <= 0)
                {
                    string messageErr = "";
                    MTasks m_objMTask = new MTasks();
                    m_objMTaskDA.Data = m_objMTask;
                    if (isUpdate)
                    {
                        m_objMTask.TaskID = this.Request.Params[SchedulesVM.Prop.TaskID.Name];
                        m_objMTaskDA.Select();
                    }
                    m_objMTask.TaskTypeID = General.EnumDesc(TaskType.Invitation_Schedules);
                    m_objMTask.TaskDateTimeStamp = DateTime.Now;
                    m_objMTask.TaskOwnerID = GetFirstApprovalRole(General.EnumDesc(TaskType.Invitation_Schedules), ref messageErr);// "PWR";//GetCurrentApproval(TaskType.ScheduleInvitation.ToString(),0);//TODO  get first level approval ownerid
                    m_objMTask.StatusID = (int)TaskStatus.Draft;
                    m_objMTask.CurrentApprovalLvl = 0;
                    m_objMTask.Remarks = isAdd ? "New From Invitation" : "Update From Invitation";
                    m_objMTask.Summary = CreateSummary(ListRecipient, ListAttachment, objSchedule.Subject, objSchedule.StartDate.ToString(Global.ThreeWordMonthDateFormat));
                    if (messageErr.Length == 0)
                    {
                        if (isAdd)
                            m_objMTaskDA.Insert(true, m_objDBConnection);
                        else
                            m_objMTaskDA.Update(true, m_objDBConnection);

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
                    m_obTaskDetail.TaskID = isAdd ? m_objMTaskDA.Data.TaskID : this.Request.Params[SchedulesVM.Prop.TaskID.Name];
                    m_obTaskDetail.StatusID = (int)TaskStatus.Draft;
                    m_obTaskDetail.Remarks = "Draft Created Schedule Invitation";
                    m_objDTaskDetailsDA.Insert(true, m_objDBConnection);

                    if (!m_objDTaskDetailsDA.Success || m_objDTaskDetailsDA.Message != string.Empty)
                        m_lstMessage.Add(m_objDTaskDetailsDA.Message);

                }

                #endregion

                #region Notifications
                int recipient_ = 0;
                foreach (string MailNotif in ListMailNotificationID)
                {
                    string errMessage = "";
                    if (m_lstMessage.Count <= 0)
                    {
                        MMailNotifications objMailNotif = new MMailNotifications();
                        objMailNotif.Importance = objSchedule.Priority == 1;
                        objMailNotif.Subject = objSchedule.Subject;
                        objMailNotif.StatusID = (int)NotificationStatus.Draft;
                        objMailNotif.FPTID = objSchedule.FPTID;
                        objMailNotif.MailNotificationID = MailNotif;
                        //objMailNotif.NotifMapID = m_NotifMapID; m_pFunctionID m_pTemplateID
                        objMailNotif.FunctionID = m_pFunctionID;
                        objMailNotif.NotificationTemplateID = m_pTemplateID;
                        objMailNotif.Contents = GenerateContent(objMailNotif.MailNotificationID, m_pTemplateID, ListOfListRecipient[recipient_], ListNotificationValues, ref errMessage);
                        objMailNotif.TaskID = m_objMTaskDA.Data.TaskID;
                        m_taskID = objMailNotif.TaskID;
                        m_objMMailNotifDA.Data = objMailNotif;
                        //m_taskID = objMailNotif.TaskID;

                        m_objMMailNotifDA.Data = objMailNotif;
                        m_objMMailNotifDA.Insert(true, m_objDBConnection);

                        if (!m_objMMailNotifDA.Success || m_objMMailNotifDA.Message != string.Empty)
                            m_lstMessage.Add(m_objMMailNotifDA.Message);

                    }
                    recipient_++;
                }
                #endregion

                #region Recipient   
                recipient_ = 0;
                
                foreach (string MailNotif in ListMailNotificationID)
                {
                    if (!isUpdateFromBatchbutNotBatch)
                    {
                        foreach (RecipientsVM obj in ListRecipient.Where(r => r.RecipientTypeID != ((int)RecipientTypes.TO).ToString()))
                        {
                            if (m_lstMessage.Count <= 0)
                            {

                                DRecipients objDRecipient = new DRecipients();

                                objDRecipient.OwnerID = obj.OwnerID;
                                objDRecipient.RecipientID = Guid.NewGuid().ToString().Replace("-", "");
                                objDRecipient.RecipientDesc = obj.RecipientDesc;
                                objDRecipient.RecipientTypeID = obj.RecipientTypeID;
                                objDRecipient.MailAddress = obj.MailAddress;
                                objDRecipient.MailNotificationID = MailNotif;
                                m_objDRecipientDA.Data = objDRecipient;
                                m_objDRecipientDA.Insert(true, m_objDBConnection);
                            }

                            if (!m_objDRecipientDA.Success || m_objDRecipientDA.Message != string.Empty)
                                m_lstMessage.Add(m_objDRecipientDA.Message);
                        }

                        //Insert Type TO
                        if (m_lstMessage.Count <= 0)
                        {

                            DRecipients objDRecipient = new DRecipients();

                            objDRecipient.OwnerID = ListTORecipient[recipient_].OwnerID;
                            objDRecipient.RecipientID = Guid.NewGuid().ToString().Replace("-", "");
                            objDRecipient.RecipientDesc = ListTORecipient[recipient_].RecipientDesc;
                            objDRecipient.RecipientTypeID = ListTORecipient[recipient_].RecipientTypeID;
                            objDRecipient.MailAddress = ListTORecipient[recipient_].MailAddress;
                            objDRecipient.MailNotificationID = MailNotif;
                            m_objDRecipientDA.Data = objDRecipient;
                            m_objDRecipientDA.Insert(true, m_objDBConnection);
                        }

                        if (!m_objDRecipientDA.Success || m_objDRecipientDA.Message != string.Empty)
                            m_lstMessage.Add(m_objDRecipientDA.Message);
                        recipient_++;
                    }
                    else
                    {                        
                        foreach (RecipientsVM obj in ListOfListRecipient[recipient_])
                        {
                            if (m_lstMessage.Count <= 0)
                            {
                                DRecipients objDRecipient = new DRecipients();
                                objDRecipient.OwnerID = obj.OwnerID;
                                objDRecipient.RecipientID = Guid.NewGuid().ToString().Replace("-", "");
                                objDRecipient.RecipientDesc = obj.RecipientDesc;
                                objDRecipient.RecipientTypeID = obj.RecipientTypeID;
                                objDRecipient.MailAddress = obj.MailAddress;
                                objDRecipient.MailNotificationID = MailNotif;
                                m_objDRecipientDA.Data = objDRecipient;
                                m_objDRecipientDA.Insert(true, m_objDBConnection);
                            }

                            if (!m_objDRecipientDA.Success || m_objDRecipientDA.Message != string.Empty)
                                m_lstMessage.Add(m_objDRecipientDA.Message);
                        }
                    }
                    
                }
                #endregion

                #region Attachment               
                if (m_lstMessage.Count <= 0 && ListAttachment.Count > 0)
                {
                    foreach (string MailNotif in ListMailNotificationID)
                    {
                        if ((m_objTNotificationAttachment.Success && isUpdate) || isAdd)
                        {
                            foreach (NotificationAttachmentVM obj in ListAttachment)
                            {
                                TNotificationAttachments objAttachment = new TNotificationAttachments();
                                objAttachment.Filename = obj.Filename;
                                objAttachment.ContentType = obj.ContentType;
                                objAttachment.RawData = obj.RawData;
                                objAttachment.MailNotificationID = MailNotif;
                                m_objTNotificationAttachment.Data = objAttachment;
                                m_objTNotificationAttachment.Insert(true, m_objDBConnection);
                            }
                        }
                    }
                    if (!m_objTNotificationAttachment.Success || m_objTNotificationAttachment.Message != string.Empty)
                        m_lstMessage.Add(m_objTNotificationAttachment.Message);
                }
                #endregion

                #region Parameter Values (TNotificationValues)
                foreach (string MailNotif in ListMailNotificationID)
                {
                    if (m_lstMessage.Count <= 0)
                    {

                        foreach (var objNotVal in ListNotificationValues)
                        {
                            TNotificationValues objNotifValues = new TNotificationValues();
                            objNotifValues.NotificationValueID = Guid.NewGuid().ToString().Replace("-", "");
                            objNotifValues.MailNotificationID = MailNotif;
                            objNotifValues.FieldTagID = objNotVal.FieldTagID;
                            objNotifValues.Value = objNotVal.Value;

                            m_objTNotificationValues.Data = objNotifValues;
                            m_objTNotificationValues.Insert(true, m_objDBConnection);

                            if (!m_objTNotificationValues.Success)
                                break;
                        }

                        if (!m_objTNotificationValues.Success || m_objTNotificationValues.Message != string.Empty)
                            m_lstMessage.Add(m_objTNotificationValues.Message);
                    }
                }
                #endregion

                #region Schedule
                if (m_lstMessage.Count <= 0)
                {

                    MSchedules objScheduleToInsert = new MSchedules();
                    objScheduleToInsert.ScheduleID =Guid.NewGuid().ToString().Replace("-", "");
                    schedID = objScheduleToInsert.ScheduleID;

                    m_objMSchedulesDA.Data = objScheduleToInsert;

                    objScheduleToInsert = objSchedule;
                    objScheduleToInsert.ScheduleID = schedID;
                    objScheduleToInsert.TaskID = m_taskID;
                    objScheduleToInsert.MailNotificationID = ListMailNotificationID[0] ;

                    m_objMSchedulesDA.Data = objScheduleToInsert;
                    m_objMSchedulesDA.Insert(true, m_objDBConnection);

                    if (!m_objMSchedulesDA.Success || m_objMSchedulesDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMSchedulesDA.Message);
                }
                #endregion

            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
            }


        }
        private SchedulesVM GetSelectedData(Dictionary<string, object> selected, ref string message, string ScheduleID = "")
        {
            SchedulesVM m_objScheduleVM = new SchedulesVM();
            m_objScheduleVM.LstRecipients = new List<RecipientsVM>();
            MSchedulesDA m_objMSchedulesDA = new MSchedulesDA();
            m_objMSchedulesDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(SchedulesVM.Prop.ScheduleID.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.TaskID.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.FPTID.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.FPTDescription.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.StatusID.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.FunctionID.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.FunctionDescription.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.NotificationTemplateID.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.NotificationTemplateDesc.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.StartDate.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.EndDate.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.Subject.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.IsAllDay.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.Notes.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.Weblink.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.ProjectID.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.ClusterID.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.ClusterDesc.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.ProjectDesc.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.Location.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.Priority.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.CreatedDate.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.IsBatchMail.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.MailNotificationID.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            if (ScheduleID.Length > 0)
            {
                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(ScheduleID);
                m_objFilter.Add(SchedulesVM.Prop.ScheduleID.Map, m_lstFilter);
            }
            else
            {
                foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
                {
                    if (m_objScheduleVM.IsKey(m_kvpSelectedRow.Key))
                    {

                        m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                        List<object> m_lstFilter = new List<object>();
                        m_lstFilter.Add(Operator.Equals);
                        m_lstFilter.Add(m_kvpSelectedRow.Value);
                        m_objFilter.Add(SchedulesVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);


                    }
                }
            }


            Dictionary<int, DataSet> m_dicMDivisionDA = m_objMSchedulesDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMSchedulesDA.Message == string.Empty)
            {
                DataRow m_drMSchedulesDA = m_dicMDivisionDA[0].Tables[0].Rows[0];
                m_objScheduleVM.ScheduleID = m_drMSchedulesDA[SchedulesVM.Prop.ScheduleID.Name].ToString();                
                m_objScheduleVM.TaskID = m_drMSchedulesDA[SchedulesVM.Prop.TaskID.Name].ToString();
                m_objScheduleVM.FPTID = m_drMSchedulesDA[SchedulesVM.Prop.FPTID.Name].ToString();
                m_objScheduleVM.FPTDescription = m_drMSchedulesDA[SchedulesVM.Prop.FPTDescription.Name].ToString();
                m_objScheduleVM.StatusID = m_drMSchedulesDA[SchedulesVM.Prop.StatusID.Name].ToString();
                m_objScheduleVM.FunctionID = m_drMSchedulesDA[SchedulesVM.Prop.FunctionID.Name].ToString();
                m_objScheduleVM.FunctionDescription = m_drMSchedulesDA[SchedulesVM.Prop.FunctionDescription.Name].ToString();
                m_objScheduleVM.NotificationTemplateID = m_drMSchedulesDA[SchedulesVM.Prop.NotificationTemplateID.Name].ToString();
                m_objScheduleVM.NotificationTemplateDesc = m_drMSchedulesDA[SchedulesVM.Prop.NotificationTemplateDesc.Name].ToString();
                m_objScheduleVM.StartDate = (DateTime)m_drMSchedulesDA[SchedulesVM.Prop.StartDate.Name];
                m_objScheduleVM.EndDate = (DateTime)m_drMSchedulesDA[SchedulesVM.Prop.EndDate.Name];
                m_objScheduleVM.Subject = m_drMSchedulesDA[SchedulesVM.Prop.Subject.Name].ToString();
                m_objScheduleVM.IsAllDay = (bool)m_drMSchedulesDA[SchedulesVM.Prop.IsAllDay.Name];
                m_objScheduleVM.IsBatchMail = string.IsNullOrEmpty(m_drMSchedulesDA[SchedulesVM.Prop.IsBatchMail.Name].ToString()) ? false : (bool)m_drMSchedulesDA[SchedulesVM.Prop.IsBatchMail.Name];
                m_objScheduleVM.Notes = m_drMSchedulesDA[SchedulesVM.Prop.Notes.Name].ToString();
                m_objScheduleVM.Weblink = m_drMSchedulesDA[SchedulesVM.Prop.Weblink.Name].ToString();
                m_objScheduleVM.ProjectID = m_drMSchedulesDA[SchedulesVM.Prop.ProjectID.Name].ToString();
                m_objScheduleVM.ProjectDesc = m_drMSchedulesDA[SchedulesVM.Prop.ProjectDesc.Name].ToString();
                m_objScheduleVM.ClusterID = m_drMSchedulesDA[SchedulesVM.Prop.ClusterID.Name].ToString();
                m_objScheduleVM.ClusterDesc= m_drMSchedulesDA[SchedulesVM.Prop.ClusterDesc.Name].ToString();
                m_objScheduleVM.Location = m_drMSchedulesDA[SchedulesVM.Prop.Location.Name].ToString();
                m_objScheduleVM.Priority = (int)m_drMSchedulesDA[SchedulesVM.Prop.Priority.Name];
                m_objScheduleVM.CreatedDate = (DateTime)m_drMSchedulesDA[SchedulesVM.Prop.CreatedDate.Name];
                m_objScheduleVM.MailNotificationID = m_drMSchedulesDA[SchedulesVM.Prop.MailNotificationID.Name].ToString();
                
                m_objScheduleVM.LstRecipients = GetListScheduleRecipiant(m_objScheduleVM.MailNotificationID, m_objScheduleVM.TaskID, m_objScheduleVM.IsBatchMail, ref message);
                m_objScheduleVM.LstNotificationValues = GetListNotificationValues(m_objScheduleVM.MailNotificationID, m_objScheduleVM.NotificationTemplateID);
                m_objScheduleVM.LstNotificationAttachment = GetListNotificationAttachment(m_objScheduleVM.MailNotificationID);
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMSchedulesDA.Message;


           return m_objScheduleVM;
        }

        #endregion

        #region Public Method
        public Dictionary<int, List<DivisionVM>> GetDivisionData(bool isCount, string DivisionID, string DivisionDesc)
        {
            int m_intCount = 0;
            List<DivisionVM> m_lstDivisionVM = new List<ViewModels.DivisionVM>();
            Dictionary<int, List<DivisionVM>> m_dicReturn = new Dictionary<int, List<DivisionVM>>();
            MDivisionDA m_objMDivisionDA = new MDivisionDA();
            m_objMDivisionDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(DivisionVM.Prop.DivisionID.MapAlias);
            m_lstSelect.Add(DivisionVM.Prop.DivisionDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(DivisionID);
            m_objFilter.Add(DivisionVM.Prop.DivisionID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(DivisionDesc);
            m_objFilter.Add(DivisionVM.Prop.DivisionDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMDivisionDA = m_objMDivisionDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMDivisionDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpDivisionBL in m_dicMDivisionDA)
                    {
                        m_intCount = m_kvpDivisionBL.Key;
                        break;
                    }
                else
                {
                    m_lstDivisionVM = (
                        from DataRow m_drMDivisionDA in m_dicMDivisionDA[0].Tables[0].Rows
                        select new DivisionVM()
                        {
                            DivisionID = m_drMDivisionDA[DivisionVM.Prop.DivisionID.Name].ToString(),
                            DivisionDesc = m_drMDivisionDA[DivisionVM.Prop.DivisionDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstDivisionVM);
            return m_dicReturn;
        }
        public List<RecipientsVM> GetListScheduleRecipiant(string MailNotifID, string TaskID, bool isBatch, ref string message)
        {
            List<string> ListMailNotif = new List<string>();
            ListMailNotif.Add(MailNotifID);
            if (isBatch)
            {
                ListMailNotif = new List<string>();
                MMailNotificationsDA m_objMMailNotifDA = new MMailNotificationsDA();
                m_objMMailNotifDA.ConnectionStringName = Global.ConnStrConfigName;

                List<string> m_lstSelect_ = new List<string>();
                m_lstSelect_.Add(MailNotificationsVM.Prop.MailNotificationID.MapAlias);

                Dictionary<string, List<object>> m_objFilter_ = new Dictionary<string, List<object>>();
                List<object> m_lstFilter_ = new List<object>();
                m_lstFilter_.Add(Operator.Equals);
                m_lstFilter_.Add(TaskID);
                m_objFilter_.Add(MailNotificationsVM.Prop.TaskID.Map, m_lstFilter_);

                Dictionary<int, DataSet> m_dicMailNotif = m_objMMailNotifDA.SelectBC(0, null, false, m_lstSelect_, m_objFilter_, null, null, null);
                if (m_objMMailNotifDA.Success && m_objMMailNotifDA.Message == "")
                {
                    foreach (DataRow dr in m_dicMailNotif[0].Tables[0].Rows)
                        ListMailNotif.Add(dr[MailNotificationsVM.Prop.MailNotificationID.Name].ToString());
                }
                else
                    message = "Error while Get List MailNotificationID";
            }

            List<RecipientsVM> lst_ScheduleRecipient = new List<RecipientsVM>();
            DRecipientsDA m_objDRecipientsDA = new DRecipientsDA();
            m_objDRecipientsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(RecipientsVM.Prop.RecipientID.MapAlias);
            m_lstSelect.Add(RecipientsVM.Prop.MailNotificationID.MapAlias);
            m_lstSelect.Add(RecipientsVM.Prop.RecipientDesc.MapAlias);
            m_lstSelect.Add(RecipientsVM.Prop.RecipientTypeID.MapAlias);
            m_lstSelect.Add(RecipientsVM.Prop.OwnerID.MapAlias);
            m_lstSelect.Add(RecipientsVM.Prop.MailAddress.MapAlias);



            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(string.Join(",",ListMailNotif));
            m_objFilter.Add(RecipientsVM.Prop.MailNotificationID.Map, m_lstFilter);
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(((int)RecipientTypes.TO).ToString());
            m_objFilter.Add(RecipientsVM.Prop.RecipientTypeID.Map, m_lstFilter);


            Dictionary<int, DataSet> m_dicTScheduleRecipientDA = m_objDRecipientsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDRecipientsDA.Message == string.Empty)
            {
                foreach (DataRow dr in m_dicTScheduleRecipientDA[0].Tables[0].Rows)
                {
                    RecipientsVM objRecipient = new RecipientsVM();
                    objRecipient.RecipientID = dr[RecipientsVM.Prop.RecipientID.Name].ToString();
                    objRecipient.MailNotificationID = dr[RecipientsVM.Prop.MailNotificationID.Name].ToString();
                    objRecipient.OwnerID = dr[RecipientsVM.Prop.OwnerID.Name].ToString();
                    objRecipient.RecipientDesc = dr[RecipientsVM.Prop.RecipientDesc.Name].ToString();
                    objRecipient.RecipientTypeID = dr[RecipientsVM.Prop.RecipientTypeID.Name].ToString();
                    objRecipient.MailAddress = dr[RecipientsVM.Prop.MailAddress.Name].ToString();

                    lst_ScheduleRecipient.Add(objRecipient);
                }

                m_objFilter = new Dictionary<string, List<object>>();
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(MailNotifID);
                m_objFilter.Add(RecipientsVM.Prop.MailNotificationID.Map, m_lstFilter);
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.NotEqual);
                m_lstFilter.Add(((int)RecipientTypes.TO).ToString());
                m_objFilter.Add(RecipientsVM.Prop.RecipientTypeID.Map, m_lstFilter);

                m_dicTScheduleRecipientDA = m_objDRecipientsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
                if (m_objDRecipientsDA.Message == string.Empty)
                {
                    foreach (DataRow dr in m_dicTScheduleRecipientDA[0].Tables[0].Rows)
                    {
                        RecipientsVM objRecipient = new RecipientsVM();
                        objRecipient.RecipientID = dr[RecipientsVM.Prop.RecipientID.Name].ToString();
                        objRecipient.MailNotificationID = dr[RecipientsVM.Prop.MailNotificationID.Name].ToString();
                        objRecipient.OwnerID = dr[RecipientsVM.Prop.OwnerID.Name].ToString();
                        objRecipient.RecipientDesc = dr[RecipientsVM.Prop.RecipientDesc.Name].ToString();
                        objRecipient.RecipientTypeID = dr[RecipientsVM.Prop.RecipientTypeID.Name].ToString();
                        objRecipient.MailAddress = dr[RecipientsVM.Prop.MailAddress.Name].ToString();

                        lst_ScheduleRecipient.Add(objRecipient);
                    }
                }

            }
            else
                message = m_objDRecipientsDA.Message;

            return lst_ScheduleRecipient;
        }

        #endregion
    }
}