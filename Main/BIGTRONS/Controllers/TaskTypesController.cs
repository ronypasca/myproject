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
    public class TaskTypesController : BaseController
    {
        private readonly string title = "Task Types";
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
            MTaskTypesDA m_objMTaskTypesDA = new MTaskTypesDA();
            m_objMTaskTypesDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

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
                m_lstSelect.Add(TaskTypesVM.Prop.TaskTypeID.MapAlias);
                m_lstSelect.Add(TaskTypesVM.Prop.Descriptions.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(TaskTypesVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMTaskTypesDA = m_objMTaskTypesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMTaskTypesDA.Message == string.Empty)
                {
                    m_lstTaskTypesVM = (
                        from DataRow m_drMTaskTypesDA in m_dicMTaskTypesDA[0].Tables[0].Rows
                        select new TaskTypesVM()
                        {
                            TaskTypeID = m_drMTaskTypesDA[TaskTypesVM.Prop.TaskTypeID.Name].ToString(),
                            Descriptions = m_drMTaskTypesDA[TaskTypesVM.Prop.Descriptions.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstTaskTypesVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            MTaskTypesDA m_objMTaskTypesDA = new MTaskTypesDA();
            m_objMTaskTypesDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

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
                m_lstSelect.Add(TaskTypesVM.Prop.TaskTypeID.MapAlias);
                m_lstSelect.Add(TaskTypesVM.Prop.Descriptions.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(TaskTypesVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMTaskTypesDA = m_objMTaskTypesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMTaskTypesDA.Message == string.Empty)
                {
                    m_lstTaskTypesVM = (
                        from DataRow m_drMTaskTypesDA in m_dicMTaskTypesDA[0].Tables[0].Rows
                        select new TaskTypesVM()
                        {
                            TaskGroupDesc = m_drMTaskTypesDA[TaskTypesVM.Prop.TaskGroupDesc.Name].ToString(),
                            TaskTypeID = m_drMTaskTypesDA[TaskTypesVM.Prop.TaskTypeID.Name].ToString(),
                            Descriptions = m_drMTaskTypesDA[TaskTypesVM.Prop.Descriptions.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstTaskTypesVM, m_intCount);
        }

        public ActionResult Home()
        {
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 0;
            return this.Direct(true);
        }

        public ActionResult Add(string Caller)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Add)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            TaskTypesVM m_objTaskTypesVM = new TaskTypesVM();
            ViewDataDictionary m_vddTaskTypes = new ViewDataDictionary();
            m_vddTaskTypes.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddTaskTypes.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
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
                Model = m_objTaskTypesVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddTaskTypes,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Detail(string Caller, string Selected, string TaskTypeID)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            TaskTypesVM m_objTaskTypesVM = new TaskTypesVM();
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
                m_dicSelectedRow = GetFormData(m_nvcParams, TaskTypeID);
            }

            if (m_dicSelectedRow.Count > 0)
                m_objTaskTypesVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
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
                Model = m_objTaskTypesVM,
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
            TaskTypesVM m_objTaskTypesVM = new TaskTypesVM();
            if (m_dicSelectedRow.Count > 0)
                m_objTaskTypesVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddTaskTypes = new ViewDataDictionary();
            m_vddTaskTypes.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddTaskTypes.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objTaskTypesVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddTaskTypes,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<TaskTypesVM> m_lstSelectedRow = new List<TaskTypesVM>();
            m_lstSelectedRow = JSON.Deserialize<List<TaskTypesVM>>(Selected);

            MTaskTypesDA m_objMTaskTypesDA = new MTaskTypesDA();
            m_objMTaskTypesDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (TaskTypesVM m_objTaskTypesVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifTaskTypesVM = m_objTaskTypesVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifTaskTypesVM in m_arrPifTaskTypesVM)
                    {
                        string m_strFieldName = m_pifTaskTypesVM.Name;
                        object m_objFieldValue = m_pifTaskTypesVM.GetValue(m_objTaskTypesVM);
                        if (m_objTaskTypesVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(TaskTypesVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objMTaskTypesDA.DeleteBC(m_objFilter, false);
                    if (m_objMTaskTypesDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMTaskTypesDA.Message);
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

        public ActionResult Browse(string ControlTaskTypeID, string ControlDescriptions, string FilterTaskTypeID = "", string FilterDescriptions = "")
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddTaskTypes = new ViewDataDictionary();
            m_vddTaskTypes.Add("Control" + TaskTypesVM.Prop.TaskTypeID.Name, ControlTaskTypeID);
            m_vddTaskTypes.Add("Control" + TaskTypesVM.Prop.Descriptions.Name, ControlDescriptions);
            m_vddTaskTypes.Add(TaskTypesVM.Prop.TaskTypeID.Name, FilterTaskTypeID);
            m_vddTaskTypes.Add(TaskTypesVM.Prop.Descriptions.Name, FilterDescriptions);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddTaskTypes,
                ViewName = "../TaskTypes/_Browse"
            };
        }

        public ActionResult Save(string Action)
        {
            //action = JSON.Deserialize<string>(action);
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MTaskTypesDA m_objMTaskTypesDA = new MTaskTypesDA();
            string m_strTaskTypeID = string.Empty;
            m_objMTaskTypesDA.ConnectionStringName = Global.ConnStrConfigName;
            try
            {
                m_strTaskTypeID = this.Request.Params[TaskTypesVM.Prop.TaskTypeID.Name];
                string m_strDescriptions = this.Request.Params[TaskTypesVM.Prop.Descriptions.Name];

                m_lstMessage = IsSaveValid(Action, m_strTaskTypeID, m_strDescriptions);
                if (m_lstMessage.Count <= 0)
                {
                    MTaskTypes m_objMTaskTypes = new MTaskTypes();
                    m_objMTaskTypes.TaskTypeID = m_strTaskTypeID;
                    m_objMTaskTypesDA.Data = m_objMTaskTypes;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objMTaskTypesDA.Select();

                    m_objMTaskTypes.Descriptions = m_strDescriptions;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objMTaskTypesDA.Insert(false);
                    else
                        m_objMTaskTypesDA.Update(false);

                    if (!m_objMTaskTypesDA.Success || m_objMTaskTypesDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMTaskTypesDA.Message);
                    m_strTaskTypeID = m_objMTaskTypesDA.Data.TaskTypeID;
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
            }
            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
                return Detail(General.EnumDesc(Buttons.ButtonSave), null, m_strTaskTypeID);
            }
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);
        }

        #endregion

        #region Direct Method

        public ActionResult GetTaskTypes(string ControlTaskTypeID, string ControlDescriptions, string FilterTaskTypeID, string FilterDescriptions, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<TaskTypesVM>> m_dicTaskTypesData = GetTaskTypesData(true, FilterTaskTypeID, FilterDescriptions);
                KeyValuePair<int, List<TaskTypesVM>> m_kvpTaskTypesVM = m_dicTaskTypesData.AsEnumerable().ToList()[0];
                if (m_kvpTaskTypesVM.Key < 1 || (m_kvpTaskTypesVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpTaskTypesVM.Key > 1 && !Exact)
                    return Browse(ControlTaskTypeID, ControlDescriptions, FilterTaskTypeID, FilterDescriptions);

                m_dicTaskTypesData = GetTaskTypesData(false, FilterTaskTypeID, FilterDescriptions);
                TaskTypesVM m_objTaskTypesVM = m_dicTaskTypesData[0][0];
                this.GetCmp<TextField>(ControlTaskTypeID).Value = m_objTaskTypesVM.TaskTypeID;
                this.GetCmp<TextField>(ControlDescriptions).Value = m_objTaskTypesVM.Descriptions;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, string TaskTypeID, string Descriptions)
        {
            List<string> m_lstReturn = new List<string>();

            //if (TaskTypeID == string.Empty)
            //    m_lstReturn.Add(TaskTypesVM.Prop.TaskTypeID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (Descriptions == string.Empty)
                m_lstReturn.Add(TaskTypesVM.Prop.Descriptions.Desc + " " + General.EnumDesc(MessageLib.mustFill));

            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters, string TaskTypeID = "")
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();
            m_dicReturn.Add(TaskTypesVM.Prop.TaskTypeID.Name, (parameters[TaskTypesVM.Prop.TaskTypeID.Name].ToString() == string.Empty ? TaskTypeID : parameters[TaskTypesVM.Prop.TaskTypeID.Name]));
            m_dicReturn.Add(TaskTypesVM.Prop.Descriptions.Name, parameters[TaskTypesVM.Prop.Descriptions.Name]);

            return m_dicReturn;
        }

        private TaskTypesVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            TaskTypesVM m_objTaskTypesVM = new TaskTypesVM();
            MTaskTypesDA m_objMTaskTypesDA = new MTaskTypesDA();
            m_objMTaskTypesDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(TaskTypesVM.Prop.TaskTypeID.MapAlias);
            m_lstSelect.Add(TaskTypesVM.Prop.Descriptions.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objTaskTypesVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(TaskTypesVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMTaskTypesDA = m_objMTaskTypesDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMTaskTypesDA.Message == string.Empty)
            {
                DataRow m_drMTaskTypesDA = m_dicMTaskTypesDA[0].Tables[0].Rows[0];
                m_objTaskTypesVM.TaskTypeID = m_drMTaskTypesDA[TaskTypesVM.Prop.TaskTypeID.Name].ToString();
                m_objTaskTypesVM.Descriptions = m_drMTaskTypesDA[TaskTypesVM.Prop.Descriptions.Name].ToString();
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMTaskTypesDA.Message;

            return m_objTaskTypesVM;
        }

        #endregion

        #region Public Method

        public Dictionary<int, List<TaskTypesVM>> GetTaskTypesData(bool isCount, string TaskTypeID, string Descriptions)
        {
            int m_intCount = 0;
            List<TaskTypesVM> m_lstTaskTypesVM = new List<ViewModels.TaskTypesVM>();
            Dictionary<int, List<TaskTypesVM>> m_dicReturn = new Dictionary<int, List<TaskTypesVM>>();
            MTaskTypesDA m_objMTaskTypesDA = new MTaskTypesDA();
            m_objMTaskTypesDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(TaskTypesVM.Prop.TaskTypeID.MapAlias);
            m_lstSelect.Add(TaskTypesVM.Prop.Descriptions.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(TaskTypeID);
            m_objFilter.Add(TaskTypesVM.Prop.TaskTypeID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(Descriptions);
            m_objFilter.Add(TaskTypesVM.Prop.Descriptions.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMTaskTypesDA = m_objMTaskTypesDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMTaskTypesDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpTaskTypesBL in m_dicMTaskTypesDA)
                    {
                        m_intCount = m_kvpTaskTypesBL.Key;
                        break;
                    }
                else
                {
                    m_lstTaskTypesVM = (
                        from DataRow m_drMTaskTypesDA in m_dicMTaskTypesDA[0].Tables[0].Rows
                        select new TaskTypesVM()
                        {
                            TaskTypeID = m_drMTaskTypesDA[TaskTypesVM.Prop.TaskTypeID.Name].ToString(),
                            Descriptions = m_drMTaskTypesDA[TaskTypesVM.Prop.Descriptions.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstTaskTypesVM);
            return m_dicReturn;
        }

        #endregion

    }
}