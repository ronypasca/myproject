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
using XMVC = Ext.Net.MVC;
using System.Web.Mvc;

namespace com.SML.BIGTRONS.Controllers
{
    [DirectController(AreaName = "Calendar_Overview", IDMode = DirectMethodProxyIDMode.None)]
    public class MyScheduleController : BaseController
    {               
        private readonly string dataSessionName = "FormData";
        public ActionResult Index()
        {
            base.Initialize();
            ViewBag.Title = "Calendar";
            return View();
        }
        public ActionResult GetCheckBox() {
            FieldSet fs = new FieldSet() {
                Flex = 1,
                Title = "",
                ID = "fieldset1",
                DefaultAnchor = "100%"               
            };
            
            MFunctionsDA objMFunctionsDA = new MFunctionsDA();
            objMFunctionsDA.ConnectionStringName = Global.ConnStrConfigName;
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(FunctionsVM.Prop.FunctionID.MapAlias);
            m_lstSelect.Add(FunctionsVM.Prop.FunctionDesc.MapAlias);
            Dictionary<int, DataSet> dicMFunction = objMFunctionsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);
            if (objMFunctionsDA.Success && objMFunctionsDA.AffectedRows > 0)
            {
                foreach (DataRow item in dicMFunction[0].Tables[0].Rows){
                    Checkbox chck = new Checkbox(){
                        ID = item[FunctionsVM.Prop.FunctionID.Name].ToString(),
                        BoxLabel = item[FunctionsVM.Prop.FunctionDesc.Name].ToString(),
                        Checked = true
                    };
                    chck.Listeners.Change.Fn= "reloadEventCalendar";
                    fs.Items.Add(chck);
                }
                return this.ComponentConfig(fs);
            }
            else
                return this.ComponentConfig(new Label() {ID = "lbl", Text="(Functions is empty)" });
        }
       
        public ActionResult GetReminder(string ListSchedule) {
            TreePanel m_treepanel = new TreePanel
            {
                ID = "treePanelReminder",
                Padding = 10,
                MinHeight = 200,
                Height = 500,
                RowLines = true,
                ColumnLines = true,
                UseArrows = true,
                RootVisible = false,
                SortableColumns = false,
                FolderSort = false
            };

            m_treepanel.ViewConfig = new TreeView() { ToggleOnDblClick = false };
            m_treepanel.Listeners.CellClick.Fn = "ReminderClick";
            //SelectionModel
            RowSelectionModel m_rowSelectionModel = new RowSelectionModel() { Mode = SelectionMode.Single, AllowDeselect = true };
            m_treepanel.SelectionModel.Add(m_rowSelectionModel);

            //Expand

            //Fields
            ModelField m_objModelField = new ModelField();
            ModelFieldCollection m_objModelFieldCollection = new ModelFieldCollection();

            m_objModelField = new ModelField { Name = "reminder" };
            m_objModelFieldCollection.Add(m_objModelField);
            m_treepanel.Fields.AddRange(m_objModelFieldCollection);

            //Columns
            List<ColumnBase> m_ListColumn = new List<ColumnBase>();
            ColumnBase m_objColumn = new Column();
            NumberField m_objNumberField = new NumberField();
            //m_objColumn = new Column { Text = NegotiationBidStructuresVM.Prop.Sequence.Desc, DataIndex = NegotiationBidStructuresVM.Prop.Sequence.Name.ToLower(), Flex = 1, Hidden = true };
            //m_ListColumn.Add(m_objColumn);
            m_objColumn = new TreeColumn { Text = " ", DataIndex = "reminder", Flex = 3 };
            m_ListColumn.Add(m_objColumn);
            //m_objColumn = new NumberColumn
            //{
            //    Text = NegotiationBidStructuresVM.Prop.LastOffer.Desc,
            //    DataIndex = NegotiationBidStructuresVM.Prop.LastOffer.Name.ToLower(),
            //    Flex = 1,
            //    Align = ColumnAlign.End,
            //    Format = Global.DefaultNumberFormat
            //};
            //m_objColumn.Renderer = new Renderer("renderTreeColumn");
            //m_ListColumn.Add(m_objColumn);
            //m_objColumn = new NumberColumn
            //{
            //    Text = NegotiationBidStructuresVM.Prop.LastBid.Desc,
            //    DataIndex = NegotiationBidStructuresVM.Prop.LastBid.Name.ToLower(),
            //    Flex = 1,
            //    Align = ColumnAlign.End,
            //    Format = Global.DefaultNumberFormat
            //};
            //m_objColumn.Renderer = new Renderer("renderTreeColumn");
            //m_ListColumn.Add(m_objColumn);
            //m_objColumn = new NumberColumn
            //{
            //    Text = NegotiationBidStructuresVM.Prop.Bid.Desc,
            //    DataIndex = NegotiationBidStructuresVM.Prop.Bid.Name.ToLower(),
            //    Flex = 1,
            //    Align = ColumnAlign.End,
            //    Format = Global.DefaultNumberFormat

            //};
            //m_objColumn.Renderer = new Renderer("renderTreeColumn");
            //m_objNumberField = new NumberField()
            //{
            //    //ID = "ColBid",
            //    HideTrigger = true,
            //    EnforceMaxLength = true,
            //    MinValue = 0,
            //    DecimalPrecision = 4,
            //    SpinDownEnabled = false,
            //    SpinUpEnabled = false
            //};
            //m_objColumn.Editor.Add(m_objNumberField);

            //m_ListColumn.Add(m_objColumn);
            m_treepanel.ColumnModel.Columns.AddRange(m_ListColumn);



            //Root
            DateTime now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            Node m_node = new Node() { Expanded = true };
            Node Today = new Node()
            {
                Expandable = true,
                Icon = Icon.BulletYellow,
                AttributesObject = new { reminder = "Today" }
            };            
            Today.Children.AddRange(GetEvent(true,false));
            Today.Expandable = Today.Children.Count>0;
            Today.Expanded = Today.Expandable;
            Today.Cls = "rmndBack";
            
            m_node.Children.Add(Today);
            
            Node Tomorrow = new Node()
            {
                Expandable = true,
                Icon = Icon.BulletYellow,
                AttributesObject = new { reminder = "Tomorrow" }
            };
            Tomorrow.Children.AddRange(GetEvent(false,true));
            Tomorrow.Expandable = Tomorrow.Children.Count > 0;
            Tomorrow.Expanded = Tomorrow.Expandable;
            Tomorrow.Cls = "rmndBack";
            m_node.Children.Add(Tomorrow);

            Node NextWeek = new Node()
            {
                Expandable = true,
                Icon = Icon.BulletYellow,
                AttributesObject = new { reminder = "Next Week" }
            };
            
            DateTime nextWeek = now.AddDays(8 - (int)now.DayOfWeek);
            NextWeek.Children.AddRange(GetEvent(false,false,nextWeek.ToString(Global.SqlDateFormat),nextWeek.AddDays(6).ToString(Global.SqlDateFormat)));
            NextWeek.Expandable = NextWeek.Children.Count > 0;
            NextWeek.Expanded = NextWeek.Expandable;
            NextWeek.Cls = "rmndBack";
            m_node.Children.Add(NextWeek);

            Node Later = new Node()
            {
                Expandable = true,
                Icon = Icon.BulletYellow,
                AttributesObject = new { reminder = "Later" }
            };
            Later.Children.AddRange(GetEvent(false, false, nextWeek.AddDays(6).ToString(Global.SqlDateFormat), now.AddDays(360).ToString(Global.SqlDateFormat)));
            Later.Expandable = Later.Children.Count > 0;
            Later.Expanded = Later.Expandable;
            Later.Cls = "rmndBack";
            m_node.Children.Add(Later);

            m_treepanel.Root.Add(m_node);
            if (m_node.Children != null)
                return this.ComponentConfig(m_treepanel);
            else
                return this.ComponentConfig(new Label() { ID = "lblReminder", Text = "(Empty)" });
        }
        public ActionResult ReloadEventCalendar(string ListFunction) {
            List<string> m_dicSelectedRow = JSON.Deserialize<List<string>>(ListFunction);
            string mssg = "";
            return this.Store(LoadMSchedules(ref mssg, m_dicSelectedRow));
        }
        public ActionResult List()
        {

            if (Session[dataSessionName] != null)
                Session[dataSessionName] = null;

            ViewDataDictionary m_vddMyTasks = new ViewDataDictionary();
            m_vddMyTasks.Add("Datenow", DateTime.Now.ToString(Global.ThreeWordMonthDateFormat));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 0;
            string mssg = "";
            //List<SchedulesVM> LstSchedule = LoadMSchedules(ref mssg, "");
            CalendarVM cln = new CalendarVM();
            cln.EventsLoad = LoadMSchedules(ref mssg);
            //int evid = 1;
            //foreach (SchedulesVM sch in LstSchedule) {
            //    EventModel evm = new EventModel();
            //    evm.EventId = evid;
            //    evm.CalendarId = 2;
            //    evm.Title = sch.Subject;
            //    evm.StartDate = sch.StartDate.AddHours(sch.StartDateHour.Hours).AddMinutes(sch.StartDateHour.Minutes);
            //    evm.EndDate = sch.EndDate.AddHours(sch.EndDateHour.Hours).AddMinutes(sch.EndDateHour.Minutes);
            //    evm.IsAllDay = sch.IsAllDay;
            //    evm.Location = sch.Location;
            //    evm.Url = sch.Weblink;
            //    evm.Notes = sch.Notes;
            //    evm.Reminder = "15";
            //    cln.EventsLoad.Add(evm);
            //    evid++;
            //}
            m_vddMyTasks.Add("ListSchedule", new List<string>() {"A","B","C" });
            return new Ext.Net.MVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.HomePage),
                RenderMode = RenderMode.AddTo,
                Model = cln.EventsLoad,
                ViewName = "_List",
                ViewData = m_vddMyTasks,
                WrapByScriptTag = false
            };
        }
        public ActionResult SubmitData(string data)
            {
                List<EventModel> events = JSON.Deserialize<List<EventModel>>(data);

                return new System.Web.Mvc.PartialViewResult
                {
                    ViewName = "EventsViewer",
                    ViewBag =
                {
                    Events = events
                }
                };
            }

        [DirectMethod(Namespace = "CompanyX")]
        public ActionResult ShowMsg(string msg)
            {
                X.Msg.Notify("Message", msg).Show();

                return this.Direct();
            }
        
        private EventModelCollection LoadMSchedules(ref string message, List<string> Functions= null)
        {
            List<SchedulesVM> LstSchedule = new List<SchedulesVM>();
            SchedulesVM m_objScheduleVM = new SchedulesVM();
            m_objScheduleVM.LstRecipients = new List<RecipientsVM>();
            MSchedulesDA m_objMSchedulesDA = new MSchedulesDA();
            m_objMSchedulesDA.ConnectionStringName = Global.ConnStrConfigName;
            DRecipientsDA m_objDRecipientsDA = new DRecipientsDA();
            m_objDRecipientsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(SchedulesVM.Prop.ScheduleID.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.TaskID.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.FPTID.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.FPTDescription.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.StatusID.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.FunctionID.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.FunctionDescription.MapAlias);
            //m_lstSelect.Add(SchedulesVM.Prop.NotifMapID.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.NotificationTemplateID.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.NotificationTemplateDesc.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.StartDate.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.EndDate.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.Subject.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.IsAllDay.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.Notes.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.Weblink.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.Location.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.Priority.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.MailNotificationID.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.OwnerID.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.CreatedBy.MapAlias);

            List<string> lstGroup = new List<string>();
            lstGroup.Add(SchedulesVM.Prop.ScheduleID.Map);
            lstGroup.Add(SchedulesVM.Prop.TaskID.Map);
            lstGroup.Add(SchedulesVM.Prop.FPTID.Map);
            lstGroup.Add(SchedulesVM.Prop.FPTDescription.Map);
            lstGroup.Add(SchedulesVM.Prop.StatusID.Map);
            lstGroup.Add(SchedulesVM.Prop.FunctionID.Map);
            lstGroup.Add(SchedulesVM.Prop.FunctionDescription.Map);
            //lstGroup.Add(SchedulesVM.Prop.NotifMapID.Map);
            lstGroup.Add(SchedulesVM.Prop.NotificationTemplateID.Map);
            lstGroup.Add(SchedulesVM.Prop.NotificationTemplateDesc.Map);
            lstGroup.Add(SchedulesVM.Prop.StartDate.Map);
            lstGroup.Add(SchedulesVM.Prop.EndDate.Map);
            lstGroup.Add(SchedulesVM.Prop.Subject.Map);
            lstGroup.Add(SchedulesVM.Prop.IsAllDay.Map);
            lstGroup.Add(SchedulesVM.Prop.Notes.Map);
            lstGroup.Add(SchedulesVM.Prop.Weblink.Map);
            lstGroup.Add(SchedulesVM.Prop.Location.Map);
            lstGroup.Add(SchedulesVM.Prop.Priority.Map);
            lstGroup.Add(SchedulesVM.Prop.MailNotificationID.Map);
            lstGroup.Add(SchedulesVM.Prop.OwnerID.Map);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            if (Functions != null)
            {                
                 m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.In);
                m_lstFilter.Add(string.Join(",",Functions));
                m_objFilter.Add(SchedulesVM.Prop.FunctionID.Map, m_lstFilter);
                
            }
            
            List<object> m_lstFilter2 = new List<object>();
            //m_lstFilter2.Add(Operator.Equals);
            //m_lstFilter2.Add(System.Web.HttpContext.Current.User.Identity.Name);
            //m_objFilter.Add(SchedulesVM.Prop.OwnerID.Map, m_lstFilter2);

            m_lstFilter2 = new List<object>();
            m_lstFilter2.Add(Operator.None);
            m_lstFilter2.Add(string.Empty);
            // remove created by parameter
            //m_objFilter.Add(string.Format("(({0} = '{1}' ) or ({2} ='{1}'))", SchedulesVM.Prop.OwnerID.Map, System.Web.HttpContext.Current.User.Identity.Name, SchedulesVM.Prop.CreatedBy.Map), m_lstFilter2);
            m_objFilter.Add(string.Format("(({0} = '{1}' ))", SchedulesVM.Prop.OwnerID.Map, System.Web.HttpContext.Current.User.Identity.Name), m_lstFilter2);

            //m_lstFilter2 = new List<object>();
            //m_lstFilter2.Add(Operator.Equals);
            //m_lstFilter2.Add((int)ScheduleStatus.Approved);
            //m_objFilter.Add(SchedulesVM.Prop.StatusID.Map, m_lstFilter2);

            m_lstFilter2 = new List<object>();
            m_lstFilter2.Add(Operator.None);
            m_lstFilter2.Add(string.Empty);
            // remove created by parameter
            //m_objFilter.Add(string.Format("(({0} = '{1}' ) or ({2} ='{1}'))", SchedulesVM.Prop.OwnerID.Map, System.Web.HttpContext.Current.User.Identity.Name, SchedulesVM.Prop.CreatedBy.Map), m_lstFilter2);
            m_objFilter.Add(string.Format("(({0} IN ({1},{2}) ))", SchedulesVM.Prop.StatusID.Map, (int)ScheduleStatus.Approved, (int)ScheduleStatus.Rescheduled), m_lstFilter2);


            Dictionary<string, OrderDirection> m_objOrderBy = new Dictionary<string, OrderDirection>
            {
                { SchedulesVM.Prop.StartDate.Map, OrderDirection.Ascending }
            };

            EventModelCollection cln = new EventModelCollection();
            Dictionary<int, DataSet> m_dicMDivisionDA = m_objDRecipientsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null,null, m_objOrderBy, null);
            if (m_objDRecipientsDA.Message == string.Empty && m_objDRecipientsDA.Success)
            {
               
                int evid = 1;
                int cdID = 1;
                Random random = new Random();
                foreach (DataRow m_drMSchedulesDA in m_dicMDivisionDA[0].Tables[0].Rows)
                {
                    //EventModel evm = new EventModel();
                    m_objScheduleVM = new SchedulesVM();

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
                    m_objScheduleVM.Notes = m_drMSchedulesDA[SchedulesVM.Prop.Notes.Name].ToString();
                    m_objScheduleVM.Weblink = m_drMSchedulesDA[SchedulesVM.Prop.Weblink.Name].ToString();
                    m_objScheduleVM.Location = m_drMSchedulesDA[SchedulesVM.Prop.Location.Name].ToString();
                    m_objScheduleVM.Priority = (int)m_drMSchedulesDA[SchedulesVM.Prop.Priority.Name];
                    m_objScheduleVM.MailNotificationID = m_drMSchedulesDA[SchedulesVM.Prop.MailNotificationID.Name].ToString();
                    //m_objScheduleVM.LstRecipients = GetListScheduleRecipiant(m_objScheduleVM.MailNotificationID);
                    //m_objScheduleVM.LstNotificationValues = GetListNotificationValues(m_objScheduleVM.MailNotificationID, m_objScheduleVM.NotificationTemplateID);
                    //m_objScheduleVM.LstNotificationAttachment = GetListNotificationAttachment(m_objScheduleVM.MailNotificationID);

                    EventModel evm = new EventModel
                    {
                        EventId = evid,
                        CalendarId = cdID,
                        Title = m_objScheduleVM.Subject,
                        StartDate = m_objScheduleVM.StartDate,//.AddHours(m_objScheduleVM.StartDateHour.Hours).AddMinutes(m_objScheduleVM.StartDateHour.Minutes);
                        EndDate = m_objScheduleVM.EndDate,//.AddHours(m_objScheduleVM.EndDateHour.Hours).AddMinutes(m_objScheduleVM.EndDateHour.Minutes);
                        IsAllDay = m_objScheduleVM.IsAllDay,
                        Location = m_objScheduleVM.Location,
                        Url = m_objScheduleVM.Weblink,
                        Notes = m_objScheduleVM.Notes,
                        Reminder = "15"
                    };
                    cln.Add(evm);
                    cdID++;
                    evid++;
                    if (cdID > 10)
                    {
                        cdID = 1;
                    }
                }
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMSchedulesDA.Message;


            return cln;
        }
        private NodeCollection GetEvent(bool isToday,bool isTomorrow,string dateStart = null,string dateEnd = null)
        {
            NodeCollection n_toReturn = new NodeCollection();
            MSchedulesDA objMSchedules = new MSchedulesDA();
            objMSchedules.ConnectionStringName = Global.ConnStrConfigName;
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            if (isToday)
            {
                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Between);
                DateTime now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                m_lstFilter.Add(now.ToString(Global.SqlDateFormat));
                m_lstFilter.Add(now.AddDays(1).ToString(Global.SqlDateFormat));
                m_objFilter.Add(SchedulesVM.Prop.StartDate.Map, m_lstFilter);
            }
            else if (isTomorrow) {
                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Between);
                DateTime now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(1).Day, 0, 0, 0);
                m_lstFilter.Add(now.ToString(Global.SqlDateFormat));
                m_lstFilter.Add(now.AddDays(1).ToString(Global.SqlDateFormat));
                m_objFilter.Add(SchedulesVM.Prop.StartDate.Map, m_lstFilter);
            }
            else if (dateStart != null && dateEnd != null)
            {
                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Between);
                m_lstFilter.Add(dateStart);
                m_lstFilter.Add(dateEnd);
                m_objFilter.Add(SchedulesVM.Prop.StartDate.Map,m_lstFilter);
                //m_objFilter.Add(SchedulesVM.Prop.StartDate.Map, m_lstFilter);
            }

            //List<object> m_lstFilter2 = new List<object>();
            //m_lstFilter2.Add(Operator.Equals);
            //m_lstFilter2.Add(System.Web.HttpContext.Current.User.Identity.Name);
            //m_objFilter.Add(SchedulesVM.Prop.OwnerID.Map, m_lstFilter2);

            List<object> m_lstFilter2 = new List<object>();
            m_lstFilter2 = new List<object>();
            m_lstFilter2.Add(Operator.None);
            m_lstFilter2.Add(string.Empty);
            m_objFilter.Add(string.Format("(({0} = '{1}' ) or ({2} ='{1}'))", SchedulesVM.Prop.OwnerID.Map, System.Web.HttpContext.Current.User.Identity.Name, SchedulesVM.Prop.CreatedBy.Map), m_lstFilter2);

            m_lstFilter2 = new List<object>();
            m_lstFilter2.Add(Operator.Equals);
            m_lstFilter2.Add((int)ScheduleStatus.Approved);
            m_objFilter.Add(SchedulesVM.Prop.StatusID.Map, m_lstFilter2);

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(SchedulesVM.Prop.ScheduleID.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.Weblink.MapAlias);
            //m_lstSelect.Add(SchedulesVM.Prop.Notes.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.Subject.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.StartDate.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.EndDate.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.Location.MapAlias);
            m_lstSelect.Add(SchedulesVM.Prop.IsAllDay.MapAlias);

            List<string> m_lstGROUP = new List<string>();
            m_lstGROUP.Add(SchedulesVM.Prop.ScheduleID.Map);
            m_lstGROUP.Add(SchedulesVM.Prop.Weblink.Map);
            //m_lstSelect.Add(SchedulesVM.Prop.Notes.MapAlias);
            m_lstGROUP.Add(SchedulesVM.Prop.Subject.Map);
            m_lstGROUP.Add(SchedulesVM.Prop.StartDate.Map);
            m_lstGROUP.Add(SchedulesVM.Prop.EndDate.Map);
            m_lstGROUP.Add(SchedulesVM.Prop.Location.Map);
            m_lstGROUP.Add(SchedulesVM.Prop.IsAllDay.Map);

            Dictionary<int, DataSet> dicMSchedules = objMSchedules.SelectBCforCalendar(0, null, false, m_lstSelect, m_objFilter, null, m_lstGROUP, null, null);
            if (objMSchedules.Success && objMSchedules.AffectedRows > 0)
            {
                Node tNode = new Node();
                foreach (DataRow item in dicMSchedules[0].Tables[0].Rows)
                {
                    tNode = new Node();
                    tNode.Icon = Icon.BulletFeed;
                    //tNode.IconCls = "hideico";
                    
                    tNode.AttributesObject = new {
                        reminder= item[SchedulesVM.Prop.Subject.Name].ToString(),
                        start = item[SchedulesVM.Prop.StartDate.Name].ToString(),
                        end = item[SchedulesVM.Prop.EndDate.Name].ToString(),
                        weblink = item[SchedulesVM.Prop.Weblink.Name].ToString(),
                        location = item[SchedulesVM.Prop.Location.Name].ToString()//,
                        //notes = item[SchedulesVM.Prop.Notes.Name].ToString()
                    };
                    tNode.Expandable = false;
                    n_toReturn.Add(tNode);
                }
                
            }

            return n_toReturn;
        }

    }
}
