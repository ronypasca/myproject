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
    public class ActionController : BaseController
    {
        private readonly string title = "Action";
        //
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
            SActionDA m_objSActionDA = new SActionDA();
            m_objSActionDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcSAction = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcSAction.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = ActionVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicSActionDA = m_objSActionDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpActionBL in m_dicSActionDA)
            {
                m_intCount = m_kvpActionBL.Key;
                break;
            }

            List<ActionVM> m_lstActionVM = new List<ActionVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(ActionVM.Prop.ActionID.MapAlias);
                m_lstSelect.Add(ActionVM.Prop.ActionDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(ActionVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicSActionDA = m_objSActionDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objSActionDA.Message == string.Empty)
                {
                    m_lstActionVM = (
                        from DataRow m_drSActionDA in m_dicSActionDA[0].Tables[0].Rows
                        select new ActionVM()
                        {
                            ActionID = m_drSActionDA[ActionVM.Prop.ActionID.Name].ToString(),
                            ActionDesc = m_drSActionDA[ActionVM.Prop.ActionDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstActionVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            SActionDA m_objSActionDA = new SActionDA();
            m_objSActionDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcSAction = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcSAction.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = ActionVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicSActionDA = m_objSActionDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpActionBL in m_dicSActionDA)
            {
                m_intCount = m_kvpActionBL.Key;
                break;
            }

            List<ActionVM> m_lstActionVM = new List<ActionVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(ActionVM.Prop.ActionID.MapAlias);
                m_lstSelect.Add(ActionVM.Prop.ActionDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(ActionVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicSActionDA = m_objSActionDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objSActionDA.Message == string.Empty)
                {
                    m_lstActionVM = (
                        from DataRow m_drSActionDA in m_dicSActionDA[0].Tables[0].Rows
                        select new ActionVM()
                        {
                            ActionID = m_drSActionDA[ActionVM.Prop.ActionID.Name].ToString(),
                            ActionDesc = m_drSActionDA[ActionVM.Prop.ActionDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstActionVM, m_intCount);
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

            ActionVM m_objActionVM = new ActionVM();
            ViewDataDictionary m_vddAction = new ViewDataDictionary();
            m_vddAction.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddAction.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objActionVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddAction,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Detail(string Caller, string Selected)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();
            if (Caller == General.EnumDesc(Buttons.ButtonList))
            {
                m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
            }
            else if (Caller == General.EnumDesc(Buttons.ButtonAdd) || Caller == General.EnumDesc(Buttons.ButtonUpdate))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                m_dicSelectedRow = GetFormData(m_nvcParams);
            }

            string m_strMessage = string.Empty;
            ActionVM m_objActionVM = new ActionVM();
            if (m_dicSelectedRow.Count > 0)
                m_objActionVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
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
                Model = m_objActionVM,
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
            }

            string m_strMessage = string.Empty;
            ActionVM m_objActionVM = new ActionVM();
            if (m_dicSelectedRow.Count > 0)
                m_objActionVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddAction = new ViewDataDictionary();
            m_vddAction.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddAction.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objActionVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddAction,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<ActionVM> m_lstSelectedRow = new List<ActionVM>();
            m_lstSelectedRow = JSON.Deserialize<List<ActionVM>>(Selected);

            SActionDA m_objSActionDA = new SActionDA();
            m_objSActionDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (ActionVM m_objActionVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifActionVM = m_objActionVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifActionVM in m_arrPifActionVM)
                    {
                        string m_strFieldName = m_pifActionVM.Name;
                        object m_objFieldValue = m_pifActionVM.GetValue(m_objActionVM);
                        if (m_objActionVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(ActionVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objSActionDA.DeleteBC(m_objFilter, false);
                    if (m_objSActionDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objSActionDA.Message);
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

        public ActionResult Browse(string ControlActionID, string ControlActionDesc, string FilterActionID = "", string FilterActionDesc = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
                //return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddAction = new ViewDataDictionary();
            m_vddAction.Add("Control" + ActionVM.Prop.ActionID.Name, ControlActionID);
            m_vddAction.Add("Control" + ActionVM.Prop.ActionDesc.Name, ControlActionDesc);
            m_vddAction.Add(ActionVM.Prop.ActionID.Name, FilterActionID);
            m_vddAction.Add(ActionVM.Prop.ActionDesc.Name, FilterActionDesc);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddAction,
                ViewName = "../Action/_Browse"
            };
        }

        public ActionResult Save(string Action)
        {
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            SActionDA m_objSActionDA = new SActionDA();
            m_objSActionDA.ConnectionStringName = Global.ConnStrConfigName;
            try
            {
                string m_strActionID = this.Request.Params[ActionVM.Prop.ActionID.Name];
                string m_strActionDesc = this.Request.Params[ActionVM.Prop.ActionDesc.Name];
                string m_strProjectID = this.Request.Params[ProjectVM.Prop.ProjectID.Name];

                m_lstMessage = IsSaveValid(Action, m_strActionID, m_strActionDesc, m_strProjectID);
                if (m_lstMessage.Count <= 0)
                {
                    SAction m_objSAction = new SAction();
                    m_objSAction.ActionID = m_strActionID;
                    m_objSActionDA.Data = m_objSAction;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objSActionDA.Select();

                    m_objSAction.ActionDesc = m_strActionDesc;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objSActionDA.Insert(false);
                    else
                        m_objSActionDA.Update(false);

                    if (!m_objSActionDA.Success || m_objSActionDA.Message != string.Empty)
                        m_lstMessage.Add(m_objSActionDA.Message);
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
            }
            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
                return Detail(Action, null);
            }
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);
        }

        #endregion

        #region Direct Method

        public ActionResult GetAction(string ControlActionID, string ControlActionDesc, string FilterActionID, string FilterActionDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<ActionVM>> m_dicActionData = GetActionData(true, FilterActionID, FilterActionDesc);
                KeyValuePair<int, List<ActionVM>> m_kvpActionVM = m_dicActionData.AsEnumerable().ToList()[0];
                if (m_kvpActionVM.Key < 1 || (m_kvpActionVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpActionVM.Key > 1 && !Exact)
                    return Browse(ControlActionID, ControlActionDesc, FilterActionID, FilterActionDesc);

                m_dicActionData = GetActionData(false, FilterActionID, FilterActionDesc);
                ActionVM m_objActionVM = m_dicActionData[0][0];
                this.GetCmp<TextField>(ControlActionID).Value = m_objActionVM.ActionID;
                this.GetCmp<TextField>(ControlActionDesc).Value = m_objActionVM.ActionDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, string ActionID, string ActionDesc, string ProjectID)
        {
            List<string> m_lstReturn = new List<string>();

            if (ActionID == string.Empty)
                m_lstReturn.Add(ActionVM.Prop.ActionID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (ActionDesc == string.Empty)
                m_lstReturn.Add(ActionVM.Prop.ActionDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(ActionVM.Prop.ActionID.Name, parameters[ActionVM.Prop.ActionID.Name]);
            m_dicReturn.Add(ActionVM.Prop.ActionDesc.Name, parameters[ActionVM.Prop.ActionDesc.Name]);

            return m_dicReturn;
        }

        private ActionVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            ActionVM m_objActionVM = new ActionVM();
            SActionDA m_objSActionDA = new SActionDA();
            m_objSActionDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ActionVM.Prop.ActionID.MapAlias);
            m_lstSelect.Add(ActionVM.Prop.ActionDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objActionVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(ActionVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicSActionDA = m_objSActionDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objSActionDA.Message == string.Empty)
            {
                DataRow m_drSActionDA = m_dicSActionDA[0].Tables[0].Rows[0];
                m_objActionVM.ActionID = m_drSActionDA[ActionVM.Prop.ActionID.Name].ToString();
                m_objActionVM.ActionDesc = m_drSActionDA[ActionVM.Prop.ActionDesc.Name].ToString();
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objSActionDA.Message;

            return m_objActionVM;
        }

        #endregion

        #region Public Method

        public Dictionary<int, List<ActionVM>> GetActionData(bool isCount, string ActionID, string ActionDesc)
        {
            int m_intCount = 0;
            List<ActionVM> m_lstActionVM = new List<ViewModels.ActionVM>();
            Dictionary<int, List<ActionVM>> m_dicReturn = new Dictionary<int, List<ActionVM>>();
            SActionDA m_objSActionDA = new SActionDA();
            m_objSActionDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ActionVM.Prop.ActionID.MapAlias);
            m_lstSelect.Add(ActionVM.Prop.ActionDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(ActionID);
            m_objFilter.Add(ActionVM.Prop.ActionID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(ActionDesc);
            m_objFilter.Add(ActionVM.Prop.ActionDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicSActionDA = m_objSActionDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objSActionDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpActionBL in m_dicSActionDA)
                    {
                        m_intCount = m_kvpActionBL.Key;
                        break;
                    }
                else
                {
                    m_lstActionVM = (
                        from DataRow m_drSActionDA in m_dicSActionDA[0].Tables[0].Rows
                        select new ActionVM()
                        {
                            ActionID = m_drSActionDA[ActionVM.Prop.ActionID.Name].ToString(),
                            ActionDesc = m_drSActionDA[ActionVM.Prop.ActionDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstActionVM);
            return m_dicReturn;
        }

        #endregion
    }
}