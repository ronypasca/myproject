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
    public class RoleController : BaseController
    {
        private readonly string title = "Role";
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
            MRoleDA m_objMRoleDA = new MRoleDA();
            m_objMRoleDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMRole = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMRole.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = RoleVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMRoleDA = m_objMRoleDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpRoleBL in m_dicMRoleDA)
            {
                m_intCount = m_kvpRoleBL.Key;
                break;
            }

            List<RoleVM> m_lstRoleVM = new List<RoleVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(RoleVM.Prop.RoleID.MapAlias);
                m_lstSelect.Add(RoleVM.Prop.RoleDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(RoleVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMRoleDA = m_objMRoleDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMRoleDA.Message == string.Empty)
                {
                    m_lstRoleVM = (
                        from DataRow m_drMRoleDA in m_dicMRoleDA[0].Tables[0].Rows
                        select new RoleVM()
                        {
                            RoleID = m_drMRoleDA[RoleVM.Prop.RoleID.Name].ToString(),
                            RoleDesc = m_drMRoleDA[RoleVM.Prop.RoleDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstRoleVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            MRoleDA m_objMRoleDA = new MRoleDA();
            m_objMRoleDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMRole = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMRole.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = RoleVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMRoleDA = m_objMRoleDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpRoleBL in m_dicMRoleDA)
            {
                m_intCount = m_kvpRoleBL.Key;
                break;
            }

            List<RoleVM> m_lstRoleVM = new List<RoleVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(RoleVM.Prop.RoleID.MapAlias);
                m_lstSelect.Add(RoleVM.Prop.RoleDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(RoleVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMRoleDA = m_objMRoleDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMRoleDA.Message == string.Empty)
                {
                    m_lstRoleVM = (
                        from DataRow m_drMRoleDA in m_dicMRoleDA[0].Tables[0].Rows
                        select new RoleVM()
                        {
                            RoleID = m_drMRoleDA[RoleVM.Prop.RoleID.Name].ToString(),
                            RoleDesc = m_drMRoleDA[RoleVM.Prop.RoleDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstRoleVM, m_intCount);
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

            RoleVM m_objRoleVM = new RoleVM();
            string m_strMessage = "";
            m_objRoleVM.ListMenuObjectVM = this.GetMenuObjectVM("", ref m_strMessage);
            ViewDataDictionary m_vddRole = new ViewDataDictionary();
            m_vddRole.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddRole.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
            m_vddRole.Add("RoleID", "");

            if (Caller == General.EnumDesc(Buttons.ButtonDetail))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                Dictionary<string, object> m_dicSelectedRow = GetFormData(m_nvcParams);
                Session[dataSessionName] = JSON.Serialize(m_dicSelectedRow);
            }
            if (m_objRoleVM.RoleFunctions == null) m_objRoleVM.RoleFunctions = new List<RoleFunctionVM>();
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objRoleVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddRole,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Detail(string Caller, string Selected)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            RoleVM m_objRoleVM = new RoleVM();
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
                m_objRoleVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            m_objRoleVM.ListMenuObjectVM = this.GetMenuObjectVM(m_objRoleVM.RoleID, ref m_strMessage);
            if (m_objRoleVM.RoleFunctions == null)
                m_objRoleVM.RoleFunctions = new List<RoleFunctionVM>();

            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddRole = new ViewDataDictionary();
            m_vddRole.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
            m_vddRole.Add("RoleID", m_objRoleVM.RoleID);
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objRoleVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddRole,
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
            RoleVM m_objRoleVM = new RoleVM();
            if (m_dicSelectedRow.Count > 0)
                m_objRoleVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);

            m_objRoleVM.ListMenuObjectVM = this.GetMenuObjectVM(m_objRoleVM.RoleID, ref m_strMessage);
            if (m_objRoleVM.RoleFunctions == null)
                m_objRoleVM.RoleFunctions = new List<RoleFunctionVM>();

            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddRole = new ViewDataDictionary();
            m_vddRole.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddRole.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            m_vddRole.Add("RoleID", m_objRoleVM.RoleID);
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objRoleVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddRole,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<RoleVM> m_lstSelectedRow = new List<RoleVM>();
            m_lstSelectedRow = JSON.Deserialize<List<RoleVM>>(Selected);

            MRoleDA m_objMRoleDA = new MRoleDA();
            m_objMRoleDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (RoleVM m_objRoleVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifRoleVM = m_objRoleVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifRoleVM in m_arrPifRoleVM)
                    {
                        string m_strFieldName = m_pifRoleVM.Name;
                        object m_objFieldValue = m_pifRoleVM.GetValue(m_objRoleVM);
                        if (m_objRoleVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(RoleVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objMRoleDA.DeleteBC(m_objFilter, false);
                    if (m_objMRoleDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMRoleDA.Message);
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

        public ActionResult Browse(string ControlRoleID, string ControlRoleDesc,string ControlGrdUserRole, string FilterRoleID = "", string FilterRoleDesc = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddRole = new ViewDataDictionary();
            m_vddRole.Add("Control" + RoleVM.Prop.RoleID.Name, ControlRoleID);
            m_vddRole.Add("Control" + RoleVM.Prop.RoleDesc.Name, ControlRoleDesc);
            m_vddRole.Add("ControlGrdUserRole", ControlGrdUserRole);
            m_vddRole.Add(RoleVM.Prop.RoleID.Name, FilterRoleID);
            m_vddRole.Add(RoleVM.Prop.RoleDesc.Name, FilterRoleDesc);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddRole,
                ViewName = "../Role/_Browse"
            };
        }

        public ActionResult Save(string Action, string RoleMenuAction, string RoleMenuObject)
        {
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MRoleDA m_objMRoleDA = new MRoleDA();
            m_objMRoleDA.ConnectionStringName = Global.ConnStrConfigName;
            DRoleFunctionDA m_objDRoleFunctionDA = new DRoleFunctionDA();
            m_objDRoleFunctionDA.ConnectionStringName = Global.ConnStrConfigName;

            Dictionary<string, object>[] m_arrRoleMenuAction = JSON.Deserialize<Dictionary<string, object>[]>(RoleMenuAction);
            List<RoleMenuObjectVM> m_lstRoleMenuObjectVM = JSON.Deserialize<List<RoleMenuObjectVM>>(RoleMenuObject);

            object m_objDBConnection = null;
            string m_strTransName = "RoleTrans";
            try
            {
                string m_strRoleID = this.Request.Params[RoleVM.Prop.RoleID.Name];
                string m_strRoleDesc = this.Request.Params[RoleVM.Prop.RoleDesc.Name];

                string m_strGrdRoleFunction = this.Request.Params[RoleVM.Prop.RoleFunction.Name];
                List<RoleFunctionVM> m_lstRoleFunctionVM = JSON.Deserialize<List<RoleFunctionVM>>(m_strGrdRoleFunction);

                m_lstMessage = IsSaveValid(Action, m_strRoleID, m_strRoleDesc);
                if (m_lstMessage.Count <= 0)
                {
                    MRole m_objMRole = new MRole();
                    m_objMRole.RoleID = m_strRoleID;
                    m_objMRoleDA.Data = m_objMRole;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objMRoleDA.Select();

                    m_objMRole.RoleDesc = m_strRoleDesc;
                    m_objDBConnection = m_objMRoleDA.BeginTrans(m_strTransName);
                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objMRoleDA.Insert(true, m_objDBConnection);
                    else
                        m_objMRoleDA.Update(true, m_objDBConnection);

                    if (!m_objMRoleDA.Success || m_objMRoleDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMRoleDA.Message);
                    else
                    {
                        #region Insert Role Menu Action
                        RoleMenuActionVM m_objRoleMenuActionVM = new RoleMenuActionVM();
                        Dictionary<string, List<object>> m_objFilterParameter = new Dictionary<string, List<object>>();
                        List<object> m_lstFilterParameter = new List<object>();
                        m_lstFilterParameter.Add(Operator.Equals);
                        m_lstFilterParameter.Add(m_strRoleID);
                        m_objFilterParameter.Add(RoleMenuActionVM.Prop.RoleID.Map, m_lstFilterParameter);
                        DRoleMenuActionDA m_objCRoleMenuActionBL = new DRoleMenuActionDA();
                        m_objCRoleMenuActionBL.DeleteBC(m_objFilterParameter, true, m_objDBConnection);
                        if (!m_objCRoleMenuActionBL.Success)
                        {
                            m_lstMessage.Add(m_objCRoleMenuActionBL.Message);
                        }

                        List<ActionVM> m_lstActionVM = this.GetActionVM();
                        foreach (Dictionary<string, object> m_dicRoleMenuAction in m_arrRoleMenuAction)
                        {
                            foreach (ActionVM mActionVM in m_lstActionVM)
                            {
                                bool m_boolChecked = bool.Parse(m_dicRoleMenuAction[mActionVM.ActionID].ToString());
                                if (m_boolChecked)
                                {
                                    DRoleMenuAction m_objDRoleMenuAction = new DRoleMenuAction();
                                    m_objDRoleMenuAction.RoleID = m_strRoleID;
                                    m_objDRoleMenuAction.MenuID = m_dicRoleMenuAction["MenuID"].ToString();
                                    m_objDRoleMenuAction.ActionID = mActionVM.ActionID;

                                    m_objCRoleMenuActionBL.Data = m_objDRoleMenuAction;
                                    m_objCRoleMenuActionBL.Insert(true, m_objDBConnection);
                                    if (!m_objCRoleMenuActionBL.Success || m_objCRoleMenuActionBL.Message != string.Empty)
                                    {
                                        m_lstMessage.Add(m_objCRoleMenuActionBL.Message);
                                    }
                                }
                            }
                        }
                        #endregion

                        #region Insert Role Menu Object
                        Dictionary<string, List<object>> m_objFilterRole = new Dictionary<string, List<object>>();
                        List<object> m_lstFilterRole = new List<object>();
                        m_lstFilterRole.Add(Operator.Equals);
                        m_lstFilterRole.Add(m_strRoleID);
                        m_objFilterRole.Add(RoleMenuObjectVM.Prop.RoleID.Map, m_lstFilterRole);
                        DRoleMenuObjectDA m_objDRoleMenuObjectDA = new DRoleMenuObjectDA();
                        m_objDRoleMenuObjectDA.DeleteBC(m_objFilterRole, true, m_objDBConnection);
                        if (!m_objDRoleMenuObjectDA.Success)
                        {
                            m_lstMessage.Add(m_objDRoleMenuObjectDA.Message);
                        }

                        foreach (RoleMenuObjectVM m_RoleMenuObjectVM in m_lstRoleMenuObjectVM)
                        {
                            if (!String.IsNullOrEmpty(m_RoleMenuObjectVM.Value))
                            {
                                DRoleMenuObject m_objDRoleMenuObject = new DRoleMenuObject();
                                m_objDRoleMenuObject.RoleID = m_strRoleID;
                                m_objDRoleMenuObject.MenuID = m_RoleMenuObjectVM.MenuID;
                                m_objDRoleMenuObject.ObjectID = m_RoleMenuObjectVM.ObjectID;
                                m_objDRoleMenuObject.Value = m_RoleMenuObjectVM.Value;

                                m_objDRoleMenuObjectDA.Data = m_objDRoleMenuObject;
                                m_objDRoleMenuObjectDA.Insert(true, m_objDBConnection);
                                if (!m_objDRoleMenuObjectDA.Success || m_objDRoleMenuObjectDA.Message != string.Empty)
                                {
                                    m_lstMessage.Add(m_objDRoleMenuObjectDA.Message);
                                }
                            }
                        }
                        #endregion

                        #region DRoleFunction
                        if (m_lstRoleFunctionVM.Any())
                        {
                            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objMRole.RoleID);
                            m_objFilter.Add(RoleFunctionVM.Prop.RoleID.Map, m_lstFilter);

                            m_objDRoleFunctionDA.DeleteBC(m_objFilter, true, m_objDBConnection);

                            foreach (RoleFunctionVM objRoleFunctionVM in m_lstRoleFunctionVM)
                            {
                                DRoleFunction m_objDRoleFunction = new DRoleFunction();
                                m_objDRoleFunctionDA.Data = m_objDRoleFunction;

                                m_objDRoleFunction.RoleID = m_objMRole.RoleID;
                                m_objDRoleFunction.FunctionID = objRoleFunctionVM.FunctionID;

                                m_objDRoleFunctionDA.Insert(true, m_objDBConnection);

                                if (!m_objDRoleFunctionDA.Success || m_objDRoleFunctionDA.Message != string.Empty)
                                {
                                    m_objDRoleFunctionDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                                    m_lstMessage.Add(m_objDRoleFunctionDA.Message);
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
            }
            if (m_lstMessage.Count <= 0)
            {
                m_objMRoleDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
                Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
                return Detail(General.EnumDesc(Buttons.ButtonSave), null);
            }
            m_objMRoleDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);
        }

        public ActionResult DeleteRoleFunction(string Selected)
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));


            List<RoleFunctionVM> m_lstSelectedRow = new List<RoleFunctionVM>();
            m_lstSelectedRow = JSON.Deserialize<List<RoleFunctionVM>>(Selected);

            DRoleFunctionDA m_objDRoleFunctionDA = new DRoleFunctionDA();
            m_objDRoleFunctionDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (RoleFunctionVM m_objRoleFunctionVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifRoleFunctionVM = m_objRoleFunctionVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifRoleFunctionVM in m_arrPifRoleFunctionVM)
                    {
                        string m_strFieldName = m_pifRoleFunctionVM.Name;
                        object m_objFieldValue = m_pifRoleFunctionVM.GetValue(m_objRoleFunctionVM) ?? string.Empty;
                        if (m_objRoleFunctionVM.IsKey(m_strFieldName))
                        {

                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(RoleFunctionVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objDRoleFunctionDA.DeleteBC(m_objFilter, false);
                    if (m_objDRoleFunctionDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDRoleFunctionDA.Message);
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                return this.Direct(false, ex.Message);

            }
            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Deleted));
                return this.Direct(true, string.Empty);
            }
            else
                return this.Direct();

        }
        #endregion

        #region Direct Method

        public ActionResult GetRole(string ControlRoleID, string ControlRoleDesc, string FilterRoleID, string FilterRoleDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<RoleVM>> m_dicRoleData = GetRoleData(true, FilterRoleID, FilterRoleDesc);
                KeyValuePair<int, List<RoleVM>> m_kvpRoleVM = m_dicRoleData.AsEnumerable().ToList()[0];
                if (m_kvpRoleVM.Key < 1 || (m_kvpRoleVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpRoleVM.Key > 1 && !Exact)
                    return Browse(ControlRoleID, ControlRoleDesc, FilterRoleID, FilterRoleDesc);

                m_dicRoleData = GetRoleData(false, FilterRoleID, FilterRoleDesc);
                RoleVM m_objRoleVM = m_dicRoleData[0][0];
                this.GetCmp<TextField>(ControlRoleID).Value = m_objRoleVM.RoleID;
                this.GetCmp<TextField>(ControlRoleDesc).Value = m_objRoleVM.RoleDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method
        private List<MenuObjectVM> GetMenuObjectVM(string roleID, ref string message)
        {
            CMenuObjectDA m_objCMenuObjectBL = new CMenuObjectDA();
            m_objCMenuObjectBL.ConnectionStringName = Global.ConnStrConfigName;
            MenuObjectVM m_objMenuObjectVM = new MenuObjectVM();
            List<MenuObjectVM> m_lstMenuObjectVM = new List<MenuObjectVM>();

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(MenuObjectVM.Prop.MenuID.MapAlias);
            m_lstSelect.Add(MenuObjectVM.Prop.ObjectID.MapAlias);
            m_lstSelect.Add(MenuObjectVM.Prop.Object.MapAlias);

            Dictionary<int, DataSet> m_dicCMenuObjectBL = m_objCMenuObjectBL.SelectBC(0, null, false, m_lstSelect, null, null, null, null);
            if (m_objCMenuObjectBL.Success)
            {
                m_lstMenuObjectVM = (
                    from DataRow m_drCMenuObjectBL in m_dicCMenuObjectBL[0].Tables[0].Rows
                    select new MenuObjectVM()
                    {
                        MenuID = m_drCMenuObjectBL[MenuObjectVM.Prop.MenuID.Name].ToString(),
                        ObjectID = m_drCMenuObjectBL[MenuObjectVM.Prop.ObjectID.Name].ToString(),
                        Object = m_drCMenuObjectBL[MenuObjectVM.Prop.Object.Name].ToString(),
                        Value = ""
                    }).ToList();

                if (!String.IsNullOrEmpty(roleID))
                {
                    List<RoleMenuObjectVM> m_lstRoleMenuObjectVM = this.GetRoleMenuObject(roleID, ref message);
                    foreach (MenuObjectVM m_MenuObjectVM in m_lstMenuObjectVM)
                    {
                        RoleMenuObjectVM m_result = m_lstRoleMenuObjectVM.Where(x => x.ObjectID == m_MenuObjectVM.ObjectID && x.MenuID == m_MenuObjectVM.MenuID).FirstOrDefault();
                        if (m_result != null)
                        {
                            m_MenuObjectVM.Value = m_result.Value;
                        }
                    }
                }
            }
            else
            {
                message = m_objCMenuObjectBL.Message;
            }
            return m_lstMenuObjectVM;
        }
        private List<RoleMenuObjectVM> GetRoleMenuObject(string roleID, ref string message)
        {
            DRoleMenuObjectDA m_objDRoleMenuObjectBL = new DRoleMenuObjectDA();
            m_objDRoleMenuObjectBL.ConnectionStringName = Global.ConnStrConfigName;
            DRoleMenuObject m_objDRoleMenuObject = new DRoleMenuObject();
            m_objDRoleMenuObjectBL.Data = m_objDRoleMenuObject;
            RoleMenuObjectVM m_objRoleMenuObjectVM = new RoleMenuObjectVM();


            List<RoleMenuObjectVM> m_lstRoleMenuObjectVM = new List<RoleMenuObjectVM>();
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(RoleMenuObjectVM.Prop.MenuID.MapAlias);
            m_lstSelect.Add(RoleMenuObjectVM.Prop.ObjectID.MapAlias);
            m_lstSelect.Add(RoleMenuObjectVM.Prop.Value.MapAlias);

            Dictionary<string, List<object>> m_objFilterParameter = new Dictionary<string, List<object>>();
            List<object> m_lstFilterParameter = new List<object>();
            m_lstFilterParameter.Add(Operator.Equals);
            m_lstFilterParameter.Add(roleID);
            m_objFilterParameter.Add(RoleMenuObjectVM.Prop.RoleID.Map, m_lstFilterParameter);

            Dictionary<int, DataSet> m_dicMMenuBL = m_objDRoleMenuObjectBL.SelectBC(0, null, false, m_lstSelect, m_objFilterParameter, null, null, null);
            if (m_objDRoleMenuObjectBL.Message == string.Empty)
            {
                m_lstRoleMenuObjectVM = (
                    from DataRow m_drMMenuBL in m_dicMMenuBL[0].Tables[0].Rows
                    select new RoleMenuObjectVM()
                    {
                        MenuID = m_drMMenuBL[RoleMenuObjectVM.Prop.MenuID.Name].ToString(),
                        ObjectID = m_drMMenuBL[RoleMenuObjectVM.Prop.ObjectID.Name].ToString(),
                        Value = m_drMMenuBL[RoleMenuObjectVM.Prop.Value.Name].ToString()
                    }).ToList();
            }
            return m_lstRoleMenuObjectVM;
        }
        private List<ActionVM> GetActionVM()
        {
            SActionDA m_objSActionDA = new SActionDA();
            m_objSActionDA.ConnectionStringName = Global.ConnStrConfigName;
            ActionVM m_objActionVM = new ActionVM();
            List<ActionVM> m_lstActionVM = new List<ActionVM>();

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ActionVM.Prop.ActionID.MapAlias);
            m_lstSelect.Add(ActionVM.Prop.ActionDesc.MapAlias);
            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(ActionVM.Prop.ActionID.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicSActionDA = m_objSActionDA.SelectBC(0, null, false, m_lstSelect, null, null, null, m_dicOrder);
            if (m_objSActionDA.Message == string.Empty)
            {
                m_lstActionVM = (
                    from DataRow m_drSActionBL in m_dicSActionDA[0].Tables[0].Rows
                    select new ActionVM()
                    {
                        ActionID = m_drSActionBL[ActionVM.Prop.ActionID.Name].ToString(),
                        ActionDesc = m_drSActionBL[ActionVM.Prop.ActionDesc.Name].ToString()
                    }).ToList();
            }
            return m_lstActionVM;
        }
        private List<MenuVM> GetMenuVM()
        {
            SMenuDA m_objSMenuDA = new SMenuDA();
            m_objSMenuDA.ConnectionStringName = Global.ConnStrConfigName;
            SMenu m_objMMenu = new SMenu();
            m_objSMenuDA.Data = m_objMMenu;
            MenuVM m_objMenuVM = new MenuVM();

            FilterHeaderConditions m_fhcMMenu = new FilterHeaderConditions();

            List<MenuVM> m_lstMenuVM = new List<MenuVM>();
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(MenuVM.Prop.MenuID.MapAlias);
            m_lstSelect.Add(MenuVM.Prop.MenuHierarchy.MapAlias);
            m_lstSelect.Add(MenuVM.Prop.MenuDesc.MapAlias);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(MenuVM.Prop.MenuHierarchy.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicMMenuBL = m_objSMenuDA.SelectBC(0, null, false, m_lstSelect, null, null, null, m_dicOrder);
            if (m_objSMenuDA.Message == string.Empty)
            {
                m_lstMenuVM = (
                    from DataRow m_drMMenuBL in m_dicMMenuBL[0].Tables[0].Rows
                    select new MenuVM()
                    {
                        MenuID = m_drMMenuBL[MenuVM.Prop.MenuID.Name].ToString(),
                        MenuHierarchy = m_drMMenuBL[MenuVM.Prop.MenuHierarchy.Name].ToString(),
                        MenuDesc = m_drMMenuBL[MenuVM.Prop.MenuDesc.Name].ToString()
                    }).ToList();
            }
            return m_lstMenuVM;
        }
        private List<RoleMenuActionVM> GetRoleMenuAction(string roleID)
        {
            DRoleMenuActionDA m_objDRoleMenuActionDA = new DRoleMenuActionDA();
            m_objDRoleMenuActionDA.ConnectionStringName = Global.ConnStrConfigName;
            DRoleMenuAction m_objDRoleMenuAction = new DRoleMenuAction();
            m_objDRoleMenuActionDA.Data = m_objDRoleMenuAction;
            RoleMenuActionVM m_objRoleMenuActionVM = new RoleMenuActionVM();


            List<RoleMenuActionVM> m_lstRoleMenuActionVM = new List<RoleMenuActionVM>();
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(RoleMenuActionVM.Prop.RoleID.MapAlias);
            m_lstSelect.Add(RoleMenuActionVM.Prop.MenuID.MapAlias);
            m_lstSelect.Add(RoleMenuActionVM.Prop.ActionID.MapAlias);

            Dictionary<string, List<object>> m_objFilterParameter = new Dictionary<string, List<object>>();
            List<object> m_lstFilterParameter = new List<object>();
            m_lstFilterParameter.Add(Operator.Equals);
            m_lstFilterParameter.Add(roleID);
            m_objFilterParameter.Add(RoleMenuActionVM.Prop.RoleID.Name, m_lstFilterParameter);

            Dictionary<int, DataSet> m_dicMMenuBL = m_objDRoleMenuActionDA.SelectBC(0, null, false, m_lstSelect, m_objFilterParameter, null, null, null);
            if (m_objDRoleMenuActionDA.Message == string.Empty)
            {
                m_lstRoleMenuActionVM = (
                    from DataRow m_drMMenuBL in m_dicMMenuBL[0].Tables[0].Rows
                    select new RoleMenuActionVM()
                    {
                        MenuID = m_drMMenuBL[RoleMenuActionVM.Prop.MenuID.Name].ToString(),
                        ActionID = m_drMMenuBL[RoleMenuActionVM.Prop.ActionID.Name].ToString()
                    }).ToList();
            }
            return m_lstRoleMenuActionVM;
        }
        private List<MenuActionVM> GetMenuActionVM()
        {
            CMenuActionDA m_objCMenuActionDA = new CMenuActionDA();
            m_objCMenuActionDA.ConnectionStringName = Global.ConnStrConfigName;
            MenuActionVM m_objMenuActionVM = new MenuActionVM();
            List<MenuActionVM> m_lstMenuActionVM = new List<MenuActionVM>();

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(MenuActionVM.Prop.MenuID.MapAlias);
            m_lstSelect.Add(MenuActionVM.Prop.ActionID.MapAlias);

            Dictionary<int, DataSet> m_dicCMenuActionBL = m_objCMenuActionDA.SelectBC(0, null, false, m_lstSelect, null, null, null, null);
            if (m_objCMenuActionDA.Message == string.Empty)
            {
                m_lstMenuActionVM = (
                    from DataRow m_drCMenuActionBL in m_dicCMenuActionBL[0].Tables[0].Rows
                    select new MenuActionVM()
                    {
                        MenuID = m_drCMenuActionBL[MenuActionVM.Prop.MenuID.Name].ToString(),
                        ActionID = m_drCMenuActionBL[MenuActionVM.Prop.ActionID.Name].ToString()
                    }).ToList();
            }

            return m_lstMenuActionVM;
        }
        private Store BuildStore(List<MenuVM> lstMenuVM, List<ActionVM> lstActionVM, List<RoleMenuActionVM> m_lstRoleMenuActionVM, string roleID)
        {
            Store store = new Store();
            ModelCollection m_modelCollection = store.Model;
            Model m_model = new Model();
            ModelFieldCollection mfc = m_model.Fields;
            mfc.Add(new ModelField("Index"));
            mfc.Add(new ModelField("MenuID"));
            mfc.Add(new ModelField("MenuDesc"));
            foreach (ActionVM m_objActionVM in lstActionVM)
            {
                mfc.Add(new ModelField(m_objActionVM.ActionID));
            }
            m_modelCollection.Add(m_model);
            store.DataSource = this.GetMenuActionObject(lstMenuVM, lstActionVM, m_lstRoleMenuActionVM, roleID);
            store.PageSize = 30;
            store.RemotePaging = true;
            return store;
        }
        private object[] GetMenuActionObject(List<MenuVM> m_lstMenuVM, List<ActionVM> lstActionVM, List<RoleMenuActionVM> m_lstRoleMenuActionVM, string roleID)
        {
            if (m_lstMenuVM.Any())
            {
                this.ProcessMenuHierarchy(new MenuVM() { MenuHierarchy = "" }, ref m_lstMenuVM, 0);

                object[] m_lstMenuAction = new object[m_lstMenuVM.Count];
                int m_intRow = 0;
                foreach (MenuVM m_MenuVM in m_lstMenuVM)
                {
                    int m_intColumn = 0;
                    object[] m_rowMenuAction = new object[lstActionVM.Count + 3];
                    m_rowMenuAction[m_intColumn] = m_intRow;
                    m_intColumn++;
                    m_rowMenuAction[m_intColumn] = m_MenuVM.MenuID;
                    m_intColumn++;
                    m_rowMenuAction[m_intColumn] = m_MenuVM.MenuDesc;

                    foreach (ActionVM m_ActionVM in lstActionVM)
                    {
                        m_intColumn++;
                        RoleMenuActionVM m_result = m_lstRoleMenuActionVM.Where(x => x.MenuID == m_MenuVM.MenuID && x.ActionID == m_ActionVM.ActionID).FirstOrDefault();
                        m_rowMenuAction[m_intColumn] = m_result == null ? false : true;
                    }
                    m_lstMenuAction[m_intRow] = m_rowMenuAction;
                    m_intRow++;
                }
                return m_lstMenuAction;
            }
            else
            {
                return null;
            }
        }
        private void ProcessMenuHierarchy(MenuVM menuVM, ref List<MenuVM> p_lstMenuVM, int level)
        {
            List<MenuVM> m_lstChild = p_lstMenuVM.Where(x => x.MenuParent == menuVM.MenuHierarchy).ToList();
            if (m_lstChild.Any())
            {
                foreach (MenuVM m_objMenuVM in m_lstChild)
                {
                    string m_strPadLeft = "";
                    for (int i = 0; i < (4 * level); i++)
                    {
                        m_strPadLeft += "&nbsp";
                    }
                    p_lstMenuVM.Find(x => x.MenuID == m_objMenuVM.MenuID).MenuDesc = m_strPadLeft + m_objMenuVM.MenuDesc;
                    this.ProcessMenuHierarchy(m_objMenuVM, ref p_lstMenuVM, level + 1);
                }
            }

        }
        private RoleVM GetRoleVM(Dictionary<string, object> selected, ref string message)
        {
            RoleVM m_objRoleVM = new RoleVM();
            MRoleDA m_objMRoleBL = new MRoleDA();
            m_objMRoleBL.ConnectionStringName = Global.ConnStrConfigName;
            MRole m_objMRole = new MRole();

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(RoleVM.Prop.RoleID.MapAlias);
            m_lstSelect.Add(RoleVM.Prop.RoleDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objRoleVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(RoleVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMRoleBL = m_objMRoleBL.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMRoleBL.Message == string.Empty)
            {
                DataRow m_drMRoleBL = m_dicMRoleBL[0].Tables[0].Rows[0];
                m_objRoleVM.RoleID = m_drMRoleBL[RoleVM.Prop.RoleID.Name].ToString();
                m_objRoleVM.RoleDesc = m_drMRoleBL[RoleVM.Prop.RoleDesc.Name].ToString();
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMRoleBL.Message;

            return m_objRoleVM;
        }
        private List<string> IsSaveValid(string Action, string RoleID, string RoleDesc)
        {
            List<string> m_lstReturn = new List<string>();

            if (RoleID == string.Empty)
                m_lstReturn.Add(RoleVM.Prop.RoleID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (RoleDesc == string.Empty)
                m_lstReturn.Add(RoleVM.Prop.RoleDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));

            return m_lstReturn;
        }
        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(RoleVM.Prop.RoleID.Name, parameters[RoleVM.Prop.RoleID.Name]);
            m_dicReturn.Add(RoleVM.Prop.RoleDesc.Name, parameters[RoleVM.Prop.RoleDesc.Name]);

            return m_dicReturn;
        }
        private RoleVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            RoleVM m_objRoleVM = new RoleVM();
            MRoleDA m_objMRoleDA = new MRoleDA();
            m_objMRoleDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(RoleVM.Prop.RoleID.MapAlias);
            m_lstSelect.Add(RoleVM.Prop.RoleDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objRoleVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(RoleVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMRoleDA = m_objMRoleDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMRoleDA.Message == string.Empty)
            {
                DataRow m_drMRoleDA = m_dicMRoleDA[0].Tables[0].Rows[0];
                m_objRoleVM.RoleID = m_drMRoleDA[RoleVM.Prop.RoleID.Name].ToString();
                m_objRoleVM.RoleDesc = m_drMRoleDA[RoleVM.Prop.RoleDesc.Name].ToString();
                m_objRoleVM.RoleFunctions = GetListRoleFunctions(m_drMRoleDA[RoleVM.Prop.RoleID.Name].ToString(), ref message);
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMRoleDA.Message;

            return m_objRoleVM;
        }
        private List<RoleFunctionVM> GetListRoleFunctions(string roleID, ref string message)
        {
            List<RoleFunctionVM> m_objListRoleFunctionVM = new List<RoleFunctionVM>();
            DRoleFunctionDA m_objDRoleFunctionDA = new DRoleFunctionDA();
            m_objDRoleFunctionDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(RoleFunctionVM.Prop.RoleID.MapAlias);
            m_lstSelect.Add(RoleFunctionVM.Prop.FunctionID.MapAlias);
            m_lstSelect.Add(RoleFunctionVM.Prop.FunctionDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();


            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(roleID);
            m_objFilter.Add(RoleFunctionVM.Prop.RoleID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicDUserRoleDA = m_objDRoleFunctionDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDRoleFunctionDA.Message == string.Empty)
            {
                //DataRow m_drUserRoleDA = m_dicDUserRoleDA[0].Tables[0].Rows[0];
                m_objListRoleFunctionVM = (from DataRow m_drUserRoleDA in m_dicDUserRoleDA[0].Tables[0].Rows
                                       select new RoleFunctionVM
                                       {
                                           RoleID = m_drUserRoleDA[RoleFunctionVM.Prop.RoleID.Name].ToString(),
                                           FunctionID = m_drUserRoleDA[RoleFunctionVM.Prop.FunctionID.Name].ToString(),
                                           FunctionDesc = m_drUserRoleDA[RoleFunctionVM.Prop.FunctionDesc.Name].ToString()
                                       }).ToList();
            }
            //else
            //message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDUserRoleDA.Message;

            return m_objListRoleFunctionVM;
        }
        #endregion

        #region Public Method

        public Dictionary<int, List<RoleVM>> GetRoleData(bool isCount, string RoleID, string RoleDesc)
        {
            int m_intCount = 0;
            List<RoleVM> m_lstRoleVM = new List<ViewModels.RoleVM>();
            Dictionary<int, List<RoleVM>> m_dicReturn = new Dictionary<int, List<RoleVM>>();
            MRoleDA m_objMRoleDA = new MRoleDA();
            m_objMRoleDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(RoleVM.Prop.RoleID.MapAlias);
            m_lstSelect.Add(RoleVM.Prop.RoleDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(RoleID);
            m_objFilter.Add(RoleVM.Prop.RoleID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(RoleDesc);
            m_objFilter.Add(RoleVM.Prop.RoleDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMRoleDA = m_objMRoleDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMRoleDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpRoleBL in m_dicMRoleDA)
                    {
                        m_intCount = m_kvpRoleBL.Key;
                        break;
                    }
                else
                {
                    m_lstRoleVM = (
                        from DataRow m_drMRoleDA in m_dicMRoleDA[0].Tables[0].Rows
                        select new RoleVM()
                        {
                            RoleID = m_drMRoleDA[RoleVM.Prop.RoleID.Name].ToString(),
                            RoleDesc = m_drMRoleDA[RoleVM.Prop.RoleDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstRoleVM);
            return m_dicReturn;
        }

        public string BuildGridPanel(string action, string roleID)
        {
            List<ActionVM> m_lstActionVM = this.GetActionVM();
            List<MenuVM> m_lstMenuVM = this.GetMenuVM();
            List<RoleMenuActionVM> m_lstRoleMenuActionVM = this.GetRoleMenuAction(roleID);
            List<MenuActionVM> m_lstMenuActionVM = this.GetMenuActionVM();
            ItemsCollection<ColumnBase> m_columnBase = new ItemsCollection<ColumnBase>();
            m_columnBase.Add(new Column
            {
                Text = "MenuID",
                DataIndex = "MenuID",
                Flex = 2,
                Visible = false,
                Filterable = false
            });

            TextField m_tfMenuDesc = new TextField();
            m_tfMenuDesc.ID = "filterMenuDesc";
            m_tfMenuDesc.Width = 160;
            m_tfMenuDesc.Icon = Icon.Magnifier;
            m_tfMenuDesc.Listeners.Change.Handler = "applyFilter(this);";
            m_tfMenuDesc.Plugins.Add(new ClearButton());

            m_columnBase.Add(new Column
            {
                Text = "Menu",
                ID = "MenuDesc",
                DataIndex = "MenuDesc",
                Flex = 2,
                Filterable = true
            });

            //,Items = { m_tfMenuDesc }

            bool m_boolDisabled = (action == General.EnumDesc(Buttons.ButtonDetail));
            foreach (ActionVM m_ActionVM in m_lstActionVM)
            {
                ItemsCollection<AbstractComponent> m_colComponent = new ItemsCollection<AbstractComponent>();
                foreach (MenuVM m_objMenuVM in m_lstMenuVM)
                {
                    MenuActionVM m_result = m_lstMenuActionVM.Where(x => x.MenuID == m_objMenuVM.MenuID && x.ActionID == m_ActionVM.ActionID).FirstOrDefault();
                    if (m_result != null)
                    {
                        m_colComponent.Add(new Checkbox
                        {
                            Disabled = m_boolDisabled
                        });
                    }
                    else
                    {
                        m_colComponent.Add(new Label
                        {
                            Hidden = true
                        });
                    }
                }
                ComponentColumn m_objComponentColumn = new ComponentColumn() { };
                m_objComponentColumn.Text = m_ActionVM.ActionID;
                m_objComponentColumn.DataIndex = m_ActionVM.ActionID;
                m_objComponentColumn.Flex = 1;
                m_objComponentColumn.Filterable = false;
                m_objComponentColumn.Editor = true;
                m_objComponentColumn.Component.AddRange(m_colComponent);
                m_objComponentColumn.Listeners.BeforeBind.Handler = "e.config = e.config[e.record.data.Index];";
                m_columnBase.Add(m_objComponentColumn);
            }

            PagingToolbar m_ptBottom = new PagingToolbar();
            m_ptBottom.DisplayInfo = true;
            m_ptBottom.BaseCls = "paging";
            m_ptBottom.DisplayMsg = "Displaying {0} - {1} of {2}";
            m_ptBottom.EmptyMsg = "No records to display";
            m_ptBottom.HideRefresh = true;

            GridPanel m_gridMenuAction = new Ext.Net.GridPanel
            {
                ID = "grdMenuAction",
                Border = false,
                Title = "Menu Action",
                Collapsible = true,
                Height = System.Web.UI.WebControls.Unit.Pixel(250),
                Store =
                {
                    BuildStore(m_lstMenuVM,m_lstActionVM,m_lstRoleMenuActionVM,roleID)
                },
                SelectionModel =
                {
                    new RowSelectionModel() { Mode = SelectionMode.Single }
                },
                ColumnModel =
                {
                    Columns =
                    {
                        m_columnBase
                    }
                },
                View =
                {
                   new Ext.Net.GridView()
                   {
                        StripeRows = true,
                        TrackOver = true
                   }
                },
                BottomBar = { m_ptBottom },
                TopBar = { new Toolbar() { Padding = 10, Items = { m_tfMenuDesc } } }
            };

            return ComponentLoader.ToConfig(m_gridMenuAction);
        }

        #endregion
    }
}