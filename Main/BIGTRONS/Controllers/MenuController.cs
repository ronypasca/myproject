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
using System.Text;
using System.Web.Mvc;
using XMVC = Ext.Net.MVC;
using System.Web.Security;

namespace com.SML.BIGTRONS.Controllers
{
    public class MenuController : BaseController
    {
        private readonly string title = "Menu";
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
            SMenuDA m_objSMenuDA = new SMenuDA();
            m_objSMenuDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcSMenu = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcSMenu.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = MenuVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicSMenuDA = m_objSMenuDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpMenuBL in m_dicSMenuDA)
            {
                m_intCount = m_kvpMenuBL.Key;
                break;
            }

            List<MenuVM> m_lstMenuVM = new List<MenuVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(MenuVM.Prop.MenuID.MapAlias);
                m_lstSelect.Add(MenuVM.Prop.MenuDesc.MapAlias);
                m_lstSelect.Add(MenuVM.Prop.MenuIcon.MapAlias);
                m_lstSelect.Add(MenuVM.Prop.MenuUrl.MapAlias);
                m_lstSelect.Add(MenuVM.Prop.MenuVisible.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(MenuVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicSMenuDA = m_objSMenuDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objSMenuDA.Message == string.Empty)
                {
                    m_lstMenuVM = (
                        from DataRow m_drSMenuDA in m_dicSMenuDA[0].Tables[0].Rows
                        select new MenuVM()
                        {
                            MenuID = m_drSMenuDA[MenuVM.Prop.MenuID.Name].ToString(),
                            MenuDesc = m_drSMenuDA[MenuVM.Prop.MenuDesc.Name].ToString(),
                            MenuIcon = m_drSMenuDA[MenuVM.Prop.MenuIcon.Name].ToString(),
                            MenuUrl = m_drSMenuDA[MenuVM.Prop.MenuUrl.Name].ToString(),
                            MenuVisible = bool.Parse(m_drSMenuDA[MenuVM.Prop.MenuVisible.Name].ToString())
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstMenuVM, m_intCount);
        }
        public ActionResult ReadMenuAction(StoreRequestParameters parameters, string menuID)
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
                    m_strDataIndex = MenuActionVM.Prop.Map(m_strDataIndex, false);
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

            foreach (KeyValuePair<int, DataSet> m_kvpMenuBL in m_dicSActionDA)
            {
                m_intCount = m_kvpMenuBL.Key;
                break;
            }

            List<MenuActionVM> m_lstMenuActionVM = new List<MenuActionVM>();
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
                    m_lstMenuActionVM = (
                        from DataRow m_drSActionDA in m_dicSActionDA[0].Tables[0].Rows
                        select new MenuActionVM()
                        {
                            ActionID = m_drSActionDA[ActionVM.Prop.ActionID.Name].ToString(),
                            ActionDesc = m_drSActionDA[ActionVM.Prop.ActionDesc.Name].ToString(),
                            Checked = IsCheckedAction(m_drSActionDA[ActionVM.Prop.ActionID.Name].ToString(), menuID)
                        }
                    ).Distinct().ToList();
                }
            }

            return this.Store(m_lstMenuActionVM, m_intCount);
        }
        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            SMenuDA m_objSMenuDA = new SMenuDA();
            m_objSMenuDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcSMenu = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcSMenu.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = MenuVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicSMenuDA = m_objSMenuDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpMenuGroupBL in m_dicSMenuDA)
            {
                m_intCount = m_kvpMenuGroupBL.Key;
                break;
            }

            List<MenuVM> m_lstMenuVM = new List<MenuVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(MenuVM.Prop.MenuID.MapAlias);
                m_lstSelect.Add(MenuVM.Prop.MenuDesc.MapAlias);
                m_lstSelect.Add(MenuVM.Prop.MenuIcon.MapAlias);
                m_lstSelect.Add(MenuVM.Prop.MenuUrl.MapAlias);
                m_lstSelect.Add(MenuVM.Prop.MenuVisible.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(MenuVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicSMenuDA = m_objSMenuDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objSMenuDA.Message == string.Empty)
                {
                    m_lstMenuVM = (
                        from DataRow m_drSMenuDA in m_dicSMenuDA[0].Tables[0].Rows
                        select new MenuVM()
                        {
                            MenuID = m_drSMenuDA[MenuVM.Prop.MenuID.Name].ToString(),
                            MenuDesc = m_drSMenuDA[MenuVM.Prop.MenuDesc.Name].ToString(),
                            MenuIcon = m_drSMenuDA[MenuVM.Prop.MenuIcon.Name].ToString(),
                            MenuUrl = m_drSMenuDA[MenuVM.Prop.MenuUrl.Name].ToString(),
                            MenuVisible = bool.Parse(m_drSMenuDA[MenuVM.Prop.MenuVisible.Name].ToString())
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstMenuVM, m_intCount);
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


            MenuVM m_objMenuVM = new MenuVM();
            m_objMenuVM.MenuParentHierarchy = "-";
            m_objMenuVM.MenuLeftHierarchy = "-";
            m_objMenuVM.ListMenuActionVM = new List<MenuActionVM>();
            m_objMenuVM.ListMenuObjectVM = new List<MenuObjectVM>();

            ViewDataDictionary m_vddMenu = new ViewDataDictionary();
            m_vddMenu.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddMenu.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
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
                Model = m_objMenuVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddMenu,
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
            string m_strMessage = string.Empty;
            MenuVM m_objMenuVM = new MenuVM();
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
            if (Caller == General.EnumDesc(Buttons.ButtonList))
            {
                m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
                m_dicSelectedRow = GetFromTree(m_dicSelectedRow);

            }
            else if (Caller == General.EnumDesc(Buttons.ButtonSave))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                m_dicSelectedRow = GetFormData(m_nvcParams);
            }

            if (m_dicSelectedRow.Count > 0)
                m_objMenuVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }


            if (m_objMenuVM != null)
            {
                m_objMenuVM.MenuParentHierarchy = m_objMenuVM.MenuHierarchy.Substring(0, m_objMenuVM.MenuHierarchy.Length - 2);
                m_objMenuVM.MenuLeftHierarchy = string.Empty;
                int m_leftHierarchy = (Convert.ToInt32(m_objMenuVM.MenuHierarchy.Substring(m_objMenuVM.MenuHierarchy.Length - 2)));
                if (m_leftHierarchy > 0 && m_objMenuVM.MenuHierarchy.Length > 2)
                    m_objMenuVM.MenuLeftHierarchy = string.Format("{0}{1}", m_objMenuVM.MenuParentHierarchy, (m_leftHierarchy - 1).ToString("D2"));


                m_objMenuVM.ListMenuActionVM = GetListMenuAction(m_objMenuVM.MenuID, ref m_strMessage);
            }
            if (!m_objMenuVM.ListMenuActionVM.Any()) m_objMenuVM.ListMenuActionVM = new List<MenuActionVM>();
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }


            if (m_objMenuVM != null)
                m_objMenuVM.ListMenuObjectVM = GetListMenuObject(m_objMenuVM.MenuID, ref m_strMessage);
            if (!m_objMenuVM.ListMenuObjectVM.Any()) m_objMenuVM.ListMenuObjectVM = new List<MenuObjectVM>();
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddMenu = new ViewDataDictionary();
            m_vddMenu.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objMenuVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddMenu,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult Update(string Caller, string Selected)
        {
            Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();
            if (Caller == General.EnumDesc(Buttons.ButtonList))
            {
                m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
                m_dicSelectedRow = GetFromTree(m_dicSelectedRow);
            }
            else if (Caller == General.EnumDesc(Buttons.ButtonDetail))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                m_dicSelectedRow = GetFormData(m_nvcParams);
                Session[dataSessionName] = JSON.Serialize(m_dicSelectedRow);
            }

            string m_strMessage = string.Empty;
            MenuVM m_objMenuVM = new MenuVM();
            if (m_dicSelectedRow.Count > 0)
                m_objMenuVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            if (m_objMenuVM != null)
            {
                m_objMenuVM.MenuParentHierarchy = m_objMenuVM.MenuHierarchy.Substring(0, m_objMenuVM.MenuHierarchy.Length - 2);
                m_objMenuVM.MenuLeftHierarchy = string.Empty;
                int m_leftHierarchy = (Convert.ToInt32(m_objMenuVM.MenuHierarchy.Substring(m_objMenuVM.MenuHierarchy.Length - 2).Trim()));
                if (m_leftHierarchy > 0 && m_objMenuVM.MenuHierarchy.Length > 2)
                    m_objMenuVM.MenuLeftHierarchy = string.Format("{0}{1}", m_objMenuVM.MenuParentHierarchy, (m_leftHierarchy - 1).ToString("D2"));


                m_objMenuVM.ListMenuActionVM = GetListMenuAction(m_objMenuVM.MenuID, ref m_strMessage);
            }
            if (!m_objMenuVM.ListMenuActionVM.Any()) m_objMenuVM.ListMenuActionVM = new List<MenuActionVM>();
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }


            if (m_objMenuVM != null)
                m_objMenuVM.ListMenuObjectVM = GetListMenuObject(m_objMenuVM.MenuID, ref m_strMessage);
            if (!m_objMenuVM.ListMenuObjectVM.Any()) m_objMenuVM.ListMenuObjectVM = new List<MenuObjectVM>();
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddMenu = new ViewDataDictionary();
            m_vddMenu.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddMenu.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objMenuVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddMenu,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<MenuVM> m_lstSelectedRow = new List<MenuVM>();
            m_lstSelectedRow = JSON.Deserialize<List<MenuVM>>(Selected);

            SMenuDA m_objSMenuDA = new SMenuDA();

            m_objSMenuDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (MenuVM m_objMenuVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifMenuVM = m_objMenuVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifMenuVM in m_arrPifMenuVM)
                    {
                        string m_strFieldName = m_pifMenuVM.Name;
                        object m_objFieldValue = m_pifMenuVM.GetValue(m_objMenuVM);
                        if (m_objMenuVM.IsKey(m_strFieldName))
                        {

                            SMenu m_objSMenu = new SMenu();
                            m_objSMenu.MenuID = m_objFieldValue.ToString();
                            m_objSMenuDA.Data = m_objSMenu;
                            m_objSMenuDA.Select();

                            m_objSMenu.MenuVisible = false;
                            m_objSMenuDA.Update(false);
                        }
                        else break;
                    }

                    if (m_objSMenuDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objSMenuDA.Message);
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
                return this.Direct(false, String.Join(Global.NewLineSeparated, m_lstMessage));
        }
        public ActionResult Browse(string ControlMenuID, string ControlMenuDesc, string FilterMenuID = "", string FilterMenuDesc = "")
        {

            ViewDataDictionary m_vddMenu = new ViewDataDictionary();
            m_vddMenu.Add("Control" + MenuVM.Prop.MenuID.Name, ControlMenuID);
            m_vddMenu.Add("Control" + MenuVM.Prop.MenuDesc.Name, ControlMenuDesc);
            m_vddMenu.Add(MenuVM.Prop.MenuID.Name, FilterMenuID);
            m_vddMenu.Add(MenuVM.Prop.MenuDesc.Name, FilterMenuDesc);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddMenu,
                ViewName = "../Menu/_Browse"
            };
        }
        public ActionResult Save(string Action)
        {
            List<string> m_lstMessage = new List<string>();
            SMenuDA m_objSMenuDA = new SMenuDA();
            MenuVM m_objSMenuVM = new MenuVM();
            SMenu m_objSMenu = new SMenu();
            CMenuActionDA m_objCMenuActionDA = new CMenuActionDA();
            CMenuObjectDA m_objCMenuObjectDA = new CMenuObjectDA();
            string m_currMenuHirarchy = string.Empty;
            m_objSMenuDA.ConnectionStringName = Global.ConnStrConfigName;
            try
            {
                string m_strMenuID = this.Request.Params[MenuVM.Prop.MenuID.Name];
                string m_strMenuDesc = this.Request.Params[MenuVM.Prop.MenuDesc.Name];
                string m_strMenuUrl = this.Request.Params[MenuVM.Prop.MenuUrl.Name];
                string m_strMenuIcon = this.Request.Params[MenuVM.Prop.MenuIcon.Name];
                string m_strMenuHierarchy = this.Request.Params[MenuVM.Prop.MenuHierarchy.Name];
                string m_strMenuVisible = this.Request.Params[MenuVM.Prop.MenuVisible.Name];
                string m_strMenuLeft = this.Request.Params[MenuVM.Prop.MenuLeftHierarchy.Name];
                string m_strMenuParent = this.Request.Params[MenuVM.Prop.MenuParentHierarchy.Name];

                string m_strListMenuParamterVM = this.Request.Params[MenuVM.Prop.ListMenuActionVM.Name];
                string m_strListMenuObjectVM = this.Request.Params[MenuVM.Prop.ListMenuObjectVM.Name];

                List<MenuActionVM> m_lstMenuActionVM = JSON.Deserialize<List<MenuActionVM>>(m_strListMenuParamterVM);
                List<MenuObjectVM> m_lstMenuObjectVM = JSON.Deserialize<List<MenuObjectVM>>(m_strListMenuObjectVM);

                m_objSMenuVM.MenuID = m_strMenuID;
                m_objSMenuVM.MenuDesc = m_strMenuDesc;
                m_objSMenuVM.MenuHierarchy = m_strMenuHierarchy;
                m_objSMenuVM.MenuIcon = m_strMenuIcon;
                m_objSMenuVM.MenuUrl = m_strMenuUrl;
                m_objSMenuVM.MenuVisible = bool.Parse(m_strMenuVisible);

                m_objSMenuVM.MenuLeftHierarchy = m_strMenuLeft;
                m_objSMenuVM.MenuParentHierarchy = m_strMenuParent;


                m_objSMenuVM.ListMenuActionVM = m_lstMenuActionVM;
                m_objSMenuVM.ListMenuObjectVM = m_lstMenuObjectVM;

                m_lstMessage = IsSaveValid(Action, m_objSMenuVM);
                if (m_lstMessage.Count <= 0)
                {
                    List<string> success_status = new List<string>();
                    object m_objDBConnection = null;
                    string m_strTransName = "Menu";
                    m_objDBConnection = m_objSMenuDA.BeginTrans(m_strTransName);

                    #region Menu
                    m_objSMenu.MenuID = m_objSMenuVM.MenuID;
                    m_objSMenuDA.Data = m_objSMenu;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objSMenuDA.Select();

                    if (!string.IsNullOrEmpty(m_objSMenuVM.MenuParentHierarchy) && m_objSMenuVM.MenuParentHierarchy != "-")
                    {
                        if (!string.IsNullOrEmpty(m_objSMenuVM.MenuLeftHierarchy) && m_objSMenuVM.MenuLeftHierarchy != "-")
                        {
                            string m_strleftHierarchy = m_objSMenuVM.MenuLeftHierarchy.Substring(m_objSMenuVM.MenuLeftHierarchy.Length - 2);
                            int currLeft = int.Parse(m_strleftHierarchy);
                            currLeft++;
                            m_currMenuHirarchy = string.Format("{0}{1}", m_objSMenuVM.MenuParentHierarchy, (currLeft).ToString("D2"));
                        }
                        else
                        {
                            m_objSMenuVM.MenuLeftHierarchy = string.Empty;
                            m_currMenuHirarchy = string.Format("{0}{1}", m_objSMenuVM.MenuParentHierarchy, "00");
                        }
                        
                    }

                    if (m_objSMenu.MenuHierarchy != m_currMenuHirarchy && m_currMenuHirarchy != string.Empty)
                    {
                        m_objSMenu.MenuHierarchy = m_currMenuHirarchy;
                        m_objSMenuDA.UpdateHierarchy(m_objSMenuVM.MenuParentHierarchy, m_objSMenuVM.MenuLeftHierarchy, true, m_objDBConnection);
                    }

                    m_objSMenu.MenuDesc = m_objSMenuVM.MenuDesc;
                    m_objSMenu.MenuIcon = m_objSMenuVM.MenuIcon;
                    m_objSMenu.MenuUrl = m_objSMenuVM.MenuUrl;
                    m_objSMenu.MenuVisible = m_objSMenuVM.MenuVisible;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                    {
                        if (m_currMenuHirarchy == string.Empty)
                            m_currMenuHirarchy = GetLastRootHirarchy();

                        m_objSMenu.MenuHierarchy = m_currMenuHirarchy;
                        m_objSMenuDA.Insert(true, m_objDBConnection);
                    }
                    else
                        m_objSMenuDA.Update(true, m_objDBConnection);

                    
                    if (!m_objSMenuDA.Success || m_objSMenuDA.Message != string.Empty)
                    {
                        m_objSMenuDA.EndTrans(ref m_objDBConnection, m_strTransName);
                        return this.Direct(false, m_objSMenuDA.Message);
                    }
                    m_strMenuID = m_objSMenuDA.Data.MenuID;
                    #endregion

                    #region CMenuAction

                    if (m_objSMenuVM.ListMenuActionVM.Any())
                    {
                        m_objCMenuActionDA.ConnectionStringName = Global.ConnStrConfigName;

                        if (Action == General.EnumDesc(Buttons.ButtonUpdate))
                        {
                            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_strMenuID);
                            m_objFilter.Add(MenuActionVM.Prop.MenuID.Map, m_lstFilter);

                            string[] arr = m_objSMenuVM.ListMenuActionVM
                                 .Select(x => x.ActionID)
                                 .ToArray();
                            m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.NotIn);
                            m_lstFilter.Add(string.Join(",", arr));
                            m_objFilter.Add(MenuActionVM.Prop.ActionID.Map, m_lstFilter);

                            m_objCMenuActionDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                        }

                        foreach (MenuActionVM objMenuActionVM in m_lstMenuActionVM)
                        {
                            CMenuAction m_objCMenuAction = new CMenuAction();

                            m_objCMenuAction.MenuID = m_strMenuID;
                            m_objCMenuAction.ActionID = objMenuActionVM.ActionID;

                            m_objCMenuActionDA.Data = m_objCMenuAction;
                            m_objCMenuActionDA.Select();

                            if (m_objCMenuActionDA.Message != string.Empty)
                                m_objCMenuActionDA.Insert(true, m_objDBConnection);
                        }

                        //if (!m_objCMenuActionDA.Success || m_objCMenuActionDA.Message != string.Empty)
                        //{
                        //    m_objCMenuActionDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                        //    return this.Direct(false, m_objCMenuActionDA.Message);
                        //}

                    }
                    #endregion

                    #region CMenuObject
                    if (m_objSMenuVM.ListMenuObjectVM.Any())
                    {
                        m_objCMenuObjectDA.ConnectionStringName = Global.ConnStrConfigName;

                        if (Action == General.EnumDesc(Buttons.ButtonUpdate))
                        {
                            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_strMenuID);
                            m_objFilter.Add(MenuObjectVM.Prop.MenuID.Map, m_lstFilter);

                            m_objCMenuObjectDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                        }

                        foreach (MenuObjectVM objMenuObjectVM in m_lstMenuObjectVM)
                        {
                            CMenuObject m_objCMenuObject = new CMenuObject();

                            m_objCMenuObject.MenuID = m_strMenuID;
                            m_objCMenuObject.ObjectID = objMenuObjectVM.ObjectID;
                            m_objCMenuObject.ObjectDesc = objMenuObjectVM.ObjectDesc;
                            m_objCMenuObject.ObjectLongDesc = objMenuObjectVM.ObjectLongDesc;

                            m_objCMenuObjectDA.Data = m_objCMenuObject;
                            m_objCMenuObjectDA.Insert(true, m_objDBConnection);

                            if (!m_objCMenuObjectDA.Success || m_objCMenuObjectDA.Message != string.Empty)
                            {
                                m_objCMenuObjectDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                                return this.Direct(false, m_objCMenuObjectDA.Message);
                            }
                        }
                    }
                    #endregion



                    if (!m_objSMenuDA.Success || m_objSMenuDA.Message != string.Empty)
                        m_lstMessage.Add(m_objSMenuDA.Message);
                    else
                        m_objSMenuDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
            }
            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
                return Detail(General.EnumDesc(Buttons.ButtonSave), null);
            }
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);
        }
        public ActionResult GetListDropDownMenu(StoreRequestParameters parameters, string MenuParentHierarchy, string MenuHierarchy, string Caller)
        {
            List<MenuVM> m_lstMenuVM = new List<MenuVM>();
            SMenuDA m_objSMenuDA = new SMenuDA();
            m_objSMenuDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(MenuVM.Prop.MenuID.MapAlias);
            m_lstSelect.Add(MenuVM.Prop.MenuDesc.MapAlias);
            m_lstSelect.Add(MenuVM.Prop.MenuHierarchy.MapAlias);

            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(MenuVM.Prop.MenuHierarchy.Map, OrderDirection.Ascending);


            List<string> m_lstStrFilter = new List<string>();
            string strFilterFormat = string.Empty;

            if (MenuParentHierarchy == "-")
            {
                strFilterFormat = string.Format(@"LEN({0})=2", MenuVM.Prop.MenuHierarchy.Map);
                m_lstStrFilter.Add(strFilterFormat);
                m_lstFilter.Add(Operator.None);
                m_lstFilter.Add(string.Empty);
                m_objFilter.Add(String.Join(string.Empty, m_lstStrFilter), m_lstFilter);
            }
            else if (!string.IsNullOrEmpty(MenuParentHierarchy))
            {
                strFilterFormat = string.Format(@"({0} LIKE '{1}%'AND 
                                                         LEN({0})>2 AND  
                                                         LEN(SUBSTRING({0},LEN('{1}')+1,99)) = 2)", MenuVM.Prop.MenuHierarchy.Map, MenuParentHierarchy);

                m_lstStrFilter.Add(strFilterFormat);
                m_lstFilter.Add(Operator.None);
                m_lstFilter.Add(string.Empty);
                m_objFilter.Add(String.Join(string.Empty, m_lstStrFilter), m_lstFilter);
            }


            if (!string.IsNullOrEmpty(MenuHierarchy))
            {
                m_lstStrFilter = new List<string>();
                strFilterFormat = string.Format(@"{0} NOT LIKE '{1}'", MenuVM.Prop.MenuHierarchy.Map, MenuHierarchy);
                m_lstStrFilter.Add(strFilterFormat);
                m_lstFilter.Add(Operator.None);
                m_lstFilter.Add(string.Empty);
                m_objFilter.Add(String.Join(string.Empty, m_lstStrFilter), m_lstFilter);
            }

            Dictionary<int, DataSet> m_dicSMenuDA = m_objSMenuDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, m_dicOrder);
            if (m_objSMenuDA.Message == string.Empty)
            {
                m_lstMenuVM = (
                from DataRow m_drSMenuDA in m_dicSMenuDA[0].Tables[0].Rows
                select new MenuVM()
                {
                    MenuHierarchy = m_drSMenuDA[MenuVM.Prop.MenuHierarchy.Name].ToString(),
                    MenuDesc = m_drSMenuDA[MenuVM.Prop.MenuDesc.Name].ToString()
                }).Distinct().ToList();
            }

            return this.Store(m_lstMenuVM); ;
        }
        public ActionResult GenerateHTMLMenu()
        {
            StringBuilder m_sbMenuLi = new StringBuilder();
            Html m_strHTMLMenu = new Html();
            List<MenuVM> m_lstMenu = GetListMenuRoleAction();
            List<MenuVM> m_lstFirstLevelMenu = m_lstMenu.Where(x => x.MenuHierarchy.Length == 2).ToList();
            m_lstFirstLevelMenu = m_lstFirstLevelMenu.OrderBy(d => d.MenuHierarchy).ToList();
            foreach (MenuVM objMenu in m_lstFirstLevelMenu)
            {
                m_sbMenuLi.AppendLine(GetChildMenu(objMenu, m_lstMenu));
            }
            return this.Direct(m_sbMenuLi);
        }
        public ActionResult GenerateTreeMenu(StoreRequestParameters parameters)
        {
            NodeCollection m_ncReturn = new NodeCollection();

            List<MenuVM> m_lstMenu = GetListMenu(parameters);
            List<MenuVM> m_lstFirstLevelMenu = m_lstMenu.Where(x => x.MenuParent == String.Empty).ToList();

            string m_strMenuUrl = Global.MenuUrl;
            MenuVM m_objSelectedMenu = null;

            foreach (MenuVM menu in m_lstFirstLevelMenu)
            {
                Node m_extNode = this.GetNode(menu, m_lstMenu.Where(x => x.MenuParent.Trim() == menu.MenuHierarchy.Trim()).ToList(), m_objSelectedMenu, m_lstMenu);
                m_extNode.NodeID = menu.MenuID;
                m_extNode.Expandable = true;
                m_extNode.Expanded = true;
                m_extNode.AttributesObject = new
                {
                    menuid = menu.MenuID,
                    menudesc = menu.MenuDesc,
                    menuhierarchy = menu.MenuHierarchy,
                    menuurl = menu.MenuUrl,
                    menuicon = menu.MenuIcon,
                    menuvisible = menu.MenuVisible
                };
                m_ncReturn.Add(m_extNode);
            }

            return this.Store(m_ncReturn);
        }

        #endregion

        #region Menu Object
        public ActionResult AddMenuObject(string Caller)
        {
            //Caller = JSON.Deserialize<string>(Caller);
            MenuObjectVM m_objMenuObjectVM = new MenuObjectVM();
            ViewDataDictionary m_vddMenuObject = new ViewDataDictionary();
            m_vddMenuObject.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddMenuObject.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 2;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageTwo),
                Model = m_objMenuObjectVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddMenuObject,
                ViewName = "Object/_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult DetailMenuObject(string Caller, string Selected)
        {

            MenuObjectVM m_objMenuObjectVM = new MenuObjectVM();
            if (Caller == General.EnumDesc(Buttons.ButtonList))
            {
                //m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
                m_objMenuObjectVM = JSON.Deserialize<MenuObjectVM>(Selected);
            }
            else if (Caller == General.EnumDesc(Buttons.ButtonDetail))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                m_objMenuObjectVM = GetFormMenuObjectData(m_nvcParams);
            }

            ViewDataDictionary m_vddMenuObject = new ViewDataDictionary();
            m_vddMenuObject.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddMenuObject.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 2;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageTwo),
                Model = m_objMenuObjectVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddMenuObject,
                ViewName = "Object/_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult UpdateMenuObject(string Caller, string Selected)
        {

            //Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();
            MenuObjectVM m_objMenuObjectVM = new MenuObjectVM();
            if (Caller == General.EnumDesc(Buttons.ButtonList))
            {
                //m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
                m_objMenuObjectVM = JSON.Deserialize<MenuObjectVM>(Selected);
            }
            else if (Caller == General.EnumDesc(Buttons.ButtonDetail))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                m_objMenuObjectVM = GetFormMenuObjectData(m_nvcParams);
            }

            string m_strMessage = string.Empty;

            //if (m_dicSelectedRow.Count > 0)
            //    m_objMenuObjectVM = GetSelectedMenuObjectData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddMenuObject = new ViewDataDictionary();
            m_vddMenuObject.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddMenuObject.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 2;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageTwo),
                Model = m_objMenuObjectVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddMenuObject,
                ViewName = "Object/_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult DeleteMenuObject(string Selected)
        {
            Global.ShowInfo("Menu Object", General.EnumDesc(MessageLib.Deleted));
            return this.Direct();
        }
        #endregion

        #region Direct Method

        public ActionResult GetMenu(string ControlMenuID, string ControlMenuDesc, string ControlMenuGroupID,
            string ControlMenuGroupDesc, string ControlMenuTypeDesc, string ControlHasParameter, string ControlVersionDesc,
            string ControlHasPrice, bool FilterUPA, string FilterMenuID, string FilterMenuDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<MenuVM>> m_dicMenuData = GetMenuData(true, FilterMenuID, FilterMenuDesc);
                KeyValuePair<int, List<MenuVM>> m_kvpMenuVM = m_dicMenuData.AsEnumerable().ToList()[0];
                if (m_kvpMenuVM.Key < 1 || (m_kvpMenuVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpMenuVM.Key > 1 && !Exact)
                    return Browse(ControlMenuID, ControlMenuDesc, FilterMenuID, FilterMenuDesc);

                m_dicMenuData = GetMenuData(false, FilterMenuID, FilterMenuDesc);
                MenuVM m_objMenuVM = m_dicMenuData[0][0];
                this.GetCmp<TextField>(ControlMenuID).Value = m_objMenuVM.MenuID;
                this.GetCmp<TextField>(ControlMenuDesc).Value = m_objMenuVM.MenuDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, MenuVM objMenuVM)
        {
            List<string> m_lstReturn = new List<string>();

            if (string.IsNullOrEmpty(objMenuVM.MenuDesc))
                m_lstReturn.Add(MenuVM.Prop.MenuDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (string.IsNullOrEmpty(objMenuVM.MenuID))
                m_lstReturn.Add(MenuVM.Prop.MenuID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (string.IsNullOrEmpty(objMenuVM.MenuUrl))
                m_lstReturn.Add(MenuVM.Prop.MenuUrl.Desc + " " + General.EnumDesc(MessageLib.mustFill));


            return m_lstReturn;
        }
        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();
            m_dicReturn.Add(MenuVM.Prop.MenuID.Name, parameters[MenuVM.Prop.MenuID.Name]);
            m_dicReturn.Add(MenuVM.Prop.MenuDesc.Name, parameters[MenuVM.Prop.MenuDesc.Name]);
            m_dicReturn.Add(MenuVM.Prop.MenuHierarchy.Name, parameters[MenuVM.Prop.MenuHierarchy.Name]);
            m_dicReturn.Add(MenuVM.Prop.MenuIcon.Name, parameters[MenuVM.Prop.MenuIcon.Name]);
            m_dicReturn.Add(MenuVM.Prop.MenuUrl.Name, parameters[MenuVM.Prop.MenuUrl.Name]);
            m_dicReturn.Add(MenuVM.Prop.MenuVisible.Name, parameters[MenuVM.Prop.MenuVisible.Name]);

            return m_dicReturn;
        }
        private Dictionary<string, object> GetFromTree(Dictionary<string, object> dicSelected)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();
            m_dicReturn.Add(MenuVM.Prop.MenuID.Name, dicSelected[MenuVM.Prop.MenuID.Name.ToLower()]);
            m_dicReturn.Add(MenuVM.Prop.MenuDesc.Name, dicSelected[MenuVM.Prop.MenuDesc.Name.ToLower()]);
            m_dicReturn.Add(MenuVM.Prop.MenuHierarchy.Name, dicSelected[MenuVM.Prop.MenuHierarchy.Name.ToLower()]);
            m_dicReturn.Add(MenuVM.Prop.MenuIcon.Name, dicSelected[MenuVM.Prop.MenuIcon.Name.ToLower()]);
            m_dicReturn.Add(MenuVM.Prop.MenuUrl.Name, dicSelected[MenuVM.Prop.MenuUrl.Name.ToLower()]);
            m_dicReturn.Add(MenuVM.Prop.MenuVisible.Name, dicSelected[MenuVM.Prop.MenuVisible.Name.ToLower()]);

            return m_dicReturn;
        }
        private MenuObjectVM GetFormMenuObjectData(NameValueCollection parameters)
        {
            MenuObjectVM m_objMenuObjectVM = new MenuObjectVM()
            {

                MenuID = parameters[MenuObjectVM.Prop.MenuID.Name],
                ObjectID = parameters[MenuObjectVM.Prop.ObjectID.Name],
                ObjectDesc = parameters[MenuObjectVM.Prop.ObjectDesc.Name],
                ObjectLongDesc = parameters[MenuObjectVM.Prop.ObjectLongDesc.Name]
            };

            return m_objMenuObjectVM;
        }
        private List<MenuActionVM> GetListMenuAction(string MenuID, ref string message)
        {

            List<MenuActionVM> m_lstMenuActionVM = new List<MenuActionVM>();
            CMenuActionDA m_objCMenuActionDA = new CMenuActionDA();
            m_objCMenuActionDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(MenuActionVM.Prop.MenuID.MapAlias);
            m_lstSelect.Add(MenuActionVM.Prop.ActionID.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(MenuID);
            m_objFilter.Add(MenuActionVM.Prop.MenuID.Map, m_lstFilter);


            Dictionary<string, OrderDirection> m_objOrderBy = new Dictionary<string, OrderDirection>();
            m_objOrderBy.Add(MenuVM.Prop.MenuID.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicCMenuActionDA = m_objCMenuActionDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objCMenuActionDA.Message == string.Empty)
            {
                m_lstMenuActionVM = (
                from DataRow m_drCMenuActionDA in m_dicCMenuActionDA[0].Tables[0].Rows
                select new MenuActionVM()
                {
                    MenuID = m_drCMenuActionDA[MenuActionVM.Prop.MenuID.Name].ToString(),
                    ActionID = m_drCMenuActionDA[MenuActionVM.Prop.ActionID.Name].ToString()
                }).Distinct().ToList();
            }

            return m_lstMenuActionVM;

        }
        private List<MenuObjectVM> GetListMenuObject(string MenuID, ref string message)
        {

            List<MenuObjectVM> m_lstMenuObjectVM = new List<MenuObjectVM>();
            CMenuObjectDA m_objCMenuObjectDA = new CMenuObjectDA();
            m_objCMenuObjectDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(MenuObjectVM.Prop.MenuID.MapAlias);
            m_lstSelect.Add(MenuObjectVM.Prop.ObjectID.MapAlias);
            m_lstSelect.Add(MenuObjectVM.Prop.ObjectDesc.MapAlias);
            m_lstSelect.Add(MenuObjectVM.Prop.ObjectLongDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(MenuID);
            m_objFilter.Add(MenuObjectVM.Prop.MenuID.Map, m_lstFilter);


            Dictionary<string, OrderDirection> m_objOrderBy = new Dictionary<string, OrderDirection>();
            m_objOrderBy.Add(MenuVM.Prop.MenuID.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicCMenuObjectDA = m_objCMenuObjectDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objCMenuObjectDA.Message == string.Empty)
            {
                m_lstMenuObjectVM = (
                from DataRow m_drCMenuObjectDA in m_dicCMenuObjectDA[0].Tables[0].Rows
                select new MenuObjectVM()
                {
                    MenuID = m_drCMenuObjectDA[MenuObjectVM.Prop.MenuID.Name].ToString(),
                    ObjectID = m_drCMenuObjectDA[MenuObjectVM.Prop.ObjectID.Name].ToString(),
                    ObjectDesc = m_drCMenuObjectDA[MenuObjectVM.Prop.ObjectDesc.Name].ToString(),
                    ObjectLongDesc = m_drCMenuObjectDA[MenuObjectVM.Prop.ObjectLongDesc.Name].ToString()
                }).Distinct().ToList();
            }

            return m_lstMenuObjectVM;

        }
        private MenuVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            MenuVM m_objMenuVM = new MenuVM();
            SMenuDA m_objSMenuDA = new SMenuDA();
            m_objSMenuDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(MenuVM.Prop.MenuID.MapAlias);
            m_lstSelect.Add(MenuVM.Prop.MenuDesc.MapAlias);
            m_lstSelect.Add(MenuVM.Prop.MenuHierarchy.MapAlias);
            m_lstSelect.Add(MenuVM.Prop.MenuUrl.MapAlias);
            m_lstSelect.Add(MenuVM.Prop.MenuIcon.MapAlias);
            m_lstSelect.Add(MenuVM.Prop.MenuVisible.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objMenuVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(MenuVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicSMenuDA = m_objSMenuDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objSMenuDA.Message == string.Empty)
            {
                DataRow m_drSMenuDA = m_dicSMenuDA[0].Tables[0].Rows[0];
                m_objMenuVM.MenuID = m_drSMenuDA[MenuVM.Prop.MenuID.Name].ToString();
                m_objMenuVM.MenuDesc = m_drSMenuDA[MenuVM.Prop.MenuDesc.Name].ToString();
                m_objMenuVM.MenuHierarchy = m_drSMenuDA[MenuVM.Prop.MenuHierarchy.Name].ToString().Trim();
                m_objMenuVM.MenuUrl = m_drSMenuDA[MenuVM.Prop.MenuUrl.Name].ToString();
                m_objMenuVM.MenuIcon = m_drSMenuDA[MenuVM.Prop.MenuIcon.Name].ToString();
                m_objMenuVM.MenuVisible = bool.Parse(m_drSMenuDA[MenuVM.Prop.MenuVisible.Name].ToString());
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objSMenuDA.Message;

            return m_objMenuVM;
        }
        private string GetLastRootHirarchy()
        {
            int m_intCurrMenuHirarchy = 0;
            string m_strMenuHirarchy = string.Empty;
            MenuVM m_objMenuVM = new MenuVM();
            SMenuDA m_objSMenuDA = new SMenuDA();
            m_objSMenuDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(MenuVM.Prop.MenuHierarchy.MapAlias);

            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(MenuVM.Prop.MenuHierarchy.Map, OrderDirection.Descending);

            List<string> m_lstStrFilter = new List<string>();
            string strFilterFormat = string.Format(@" LEN({0})=2 ", MenuVM.Prop.MenuHierarchy.Map);
            m_lstStrFilter.Add(strFilterFormat);
            m_lstFilter.Add(Operator.None);
            m_lstFilter.Add(string.Empty);
            m_objFilter.Add(String.Join(string.Empty, m_lstStrFilter), m_lstFilter);

            Dictionary<int, DataSet> m_dicSMenuDA = m_objSMenuDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, m_dicOrder);
            if (m_objSMenuDA.Message == string.Empty)
            {
                DataRow m_drSMenuDA = m_dicSMenuDA[0].Tables[0].Rows[0];
                m_objMenuVM.MenuHierarchy = m_drSMenuDA[MenuVM.Prop.MenuHierarchy.Name].ToString();
            }
            if (m_objMenuVM.MenuHierarchy == null)
                m_strMenuHirarchy = "00";
            else
            {
                m_intCurrMenuHirarchy = Convert.ToInt32(m_objMenuVM.MenuHierarchy);
                m_intCurrMenuHirarchy++;
                m_strMenuHirarchy = m_intCurrMenuHirarchy.ToString("D2");
            }

            return m_strMenuHirarchy;
        }
        private bool IsCheckedAction(string menuActionID, string menuID)
        {
            MenuActionVM m_objMenuActionVM = new MenuActionVM();
            CMenuActionDA m_objCMenuActionDA = new CMenuActionDA();
            m_objCMenuActionDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(MenuActionVM.Prop.MenuID.MapAlias);
            m_lstSelect.Add(MenuActionVM.Prop.ActionID.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(menuID);
            m_objFilter.Add(MenuActionVM.Prop.MenuID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(menuActionID);
            m_objFilter.Add(MenuActionVM.Prop.ActionID.Map, m_lstFilter);

            Dictionary<string, OrderDirection> m_objOrderBy = new Dictionary<string, OrderDirection>();
            m_objOrderBy.Add(MenuVM.Prop.MenuID.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicCMenuActionDA = m_objCMenuActionDA.SelectBC(0, 1, true, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpMenuBL in m_dicCMenuActionDA)
            {
                m_intCount = m_kvpMenuBL.Key;
                break;
            }
            return (m_intCount > 0 ? true : false);
        }
        #endregion

        #region Public Method

        public Dictionary<int, List<MenuVM>> GetMenuData(bool isCount, string MenuID, string MenuDesc)
        {
            int m_intCount = 0;
            List<MenuVM> m_lstMenuVM = new List<ViewModels.MenuVM>();
            Dictionary<int, List<MenuVM>> m_dicReturn = new Dictionary<int, List<MenuVM>>();
            SMenuDA m_objSMenuDA = new SMenuDA();
            m_objSMenuDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(MenuVM.Prop.MenuID.MapAlias);
            m_lstSelect.Add(MenuVM.Prop.MenuDesc.MapAlias);
            m_lstSelect.Add(MenuVM.Prop.MenuHierarchy.MapAlias);
            m_lstSelect.Add(MenuVM.Prop.MenuIcon.MapAlias);
            m_lstSelect.Add(MenuVM.Prop.MenuUrl.MapAlias);
            m_lstSelect.Add(MenuVM.Prop.MenuVisible.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(MenuID);
            m_objFilter.Add(MenuVM.Prop.MenuID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(MenuDesc);
            m_objFilter.Add(MenuVM.Prop.MenuDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicSMenuDA = m_objSMenuDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objSMenuDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpMenuBL in m_dicSMenuDA)
                    {
                        m_intCount = m_kvpMenuBL.Key;
                        break;
                    }
                else
                {
                    m_lstMenuVM = (
                        from DataRow m_drSMenuDA in m_dicSMenuDA[0].Tables[0].Rows
                        select new MenuVM()
                        {
                            MenuID = m_drSMenuDA[MenuVM.Prop.MenuID.Name].ToString(),
                            MenuDesc = m_drSMenuDA[MenuVM.Prop.MenuDesc.Name].ToString(),
                            MenuHierarchy = m_drSMenuDA[MenuVM.Prop.MenuHierarchy.Name].ToString(),
                            MenuIcon = m_drSMenuDA[MenuVM.Prop.MenuIcon.Name].ToString(),
                            MenuUrl = m_drSMenuDA[MenuVM.Prop.MenuUrl.Name].ToString(),
                            MenuVisible = bool.Parse(m_drSMenuDA[MenuVM.Prop.MenuVisible.Name].ToString())
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstMenuVM);
            return m_dicReturn;
        }
        public List<MenuVM> GetListMenuRoleAction()
        {
            DRoleMenuActionDA m_objDRoleMenuActionDA = new DRoleMenuActionDA();
            m_objDRoleMenuActionDA.ConnectionStringName = Global.ConnStrConfigName;
            DRoleMenuAction m_objDRoleMenuAction = new DRoleMenuAction();
            m_objDRoleMenuActionDA.Data = m_objDRoleMenuAction;
            List<MenuVM> m_lstMenuVM = new List<MenuVM>();

            FilterHeaderConditions m_fhcMMenu = new FilterHeaderConditions();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();

            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(1); //Visible
            m_objFilter.Add(MenuVM.Prop.MenuVisible.Map, m_lstFilter);

            if (Global.LoggedInRoleID != string.Empty)
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.In);
                m_lstFilter.Add(Global.LoggedInRoleID);
                m_objFilter.Add(MenuVM.Prop.RoleID.Map, m_lstFilter);
            }

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(MenuVM.Prop.MenuID.MapAlias);
            m_lstSelect.Add(MenuVM.Prop.MenuDesc.MapAlias);
            m_lstSelect.Add(MenuVM.Prop.MenuHierarchy.MapAlias);
            m_lstSelect.Add(MenuVM.Prop.MenuUrl.MapAlias);
            m_lstSelect.Add(MenuVM.Prop.MenuIcon.MapAlias);
            m_lstSelect.Add(MenuVM.Prop.MenuVisible.MapAlias);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(MenuVM.Prop.MenuHierarchy.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicDRoleMenuActionDA = m_objDRoleMenuActionDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDRoleMenuActionDA.Message == string.Empty)
            {

                foreach (DataRow item in m_dicDRoleMenuActionDA[0].Tables[0].Rows)
                {
                    if (!m_lstMenuVM.Any(d => d.MenuID == item[MenuVM.Prop.MenuID.Name].ToString()))
                    {
                        MenuVM m_objMenuVM = new MenuVM()
                        {
                            MenuID = item[MenuVM.Prop.MenuID.Name].ToString(),
                            MenuDesc = item[MenuVM.Prop.MenuDesc.Name].ToString(),
                            MenuHierarchy = item[MenuVM.Prop.MenuHierarchy.Name].ToString().Trim(),
                            MenuUrl = item[MenuVM.Prop.MenuUrl.Name].ToString(),
                            MenuIcon = item[MenuVM.Prop.MenuIcon.Name].ToString(),
                            MenuVisible = bool.Parse(item[MenuVM.Prop.MenuVisible.Name].ToString())
                        };
                        m_lstMenuVM.Add(m_objMenuVM);
                    }
                }
            }

            return m_lstMenuVM;
        }
        public List<MenuVM> GetListMenu(StoreRequestParameters parameters)
        {
            SMenuDA m_objSMenuDA = new SMenuDA();
            m_objSMenuDA.ConnectionStringName = Global.ConnStrConfigName;
            SMenu m_objSMenu = new SMenu();
            m_objSMenuDA.Data = m_objSMenu;
            List<MenuVM> m_lstMenuVM = new List<MenuVM>();
            List<MenuVM> m_lstMenuParentVM = new List<MenuVM>();

            Dictionary<string, string> m_dicKeyPropertyMap = new Dictionary<string, string>();
            var properties = new MenuVM().GetType().GetProperties();
            foreach (var p in properties)
            {
                m_dicKeyPropertyMap.Add(p.Name.ToLower(), p.Name);
            }

            FilterHeaderConditions m_fhcSMenu = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcSMenu.Conditions)
            {
                string m_strDataIndex = m_dicKeyPropertyMap[m_fhcFilter.DataIndex];
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = MenuVM.Prop.Map(m_strDataIndex, false);
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


            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(MenuVM.Prop.MenuID.MapAlias);
            m_lstSelect.Add(MenuVM.Prop.MenuDesc.MapAlias);
            m_lstSelect.Add(MenuVM.Prop.MenuHierarchy.MapAlias);
            m_lstSelect.Add(MenuVM.Prop.MenuUrl.MapAlias);
            m_lstSelect.Add(MenuVM.Prop.MenuIcon.MapAlias);
            m_lstSelect.Add(MenuVM.Prop.MenuVisible.MapAlias);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(MenuVM.Prop.MenuHierarchy.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicDRoleMenuActionDA = m_objSMenuDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objSMenuDA.Message == string.Empty)
            {
                m_lstMenuVM = (
                    from DataRow m_drSMenu in m_dicDRoleMenuActionDA[0].Tables[0].Rows
                    select new MenuVM()
                    {
                        MenuID = m_drSMenu[MenuVM.Prop.MenuID.Name].ToString(),
                        MenuDesc = m_drSMenu[MenuVM.Prop.MenuDesc.Name].ToString(),
                        MenuHierarchy = m_drSMenu[MenuVM.Prop.MenuHierarchy.Name].ToString().Trim(),
                        MenuUrl = m_drSMenu[MenuVM.Prop.MenuUrl.Name].ToString(),
                        MenuIcon = m_drSMenu[MenuVM.Prop.MenuIcon.Name].ToString(),
                        MenuVisible = bool.Parse(m_drSMenu[MenuVM.Prop.MenuVisible.Name].ToString())
                    }).ToList();
            }

            if (m_objFilter.Count > 0 && m_lstMenuVM.Any())
            {
                m_lstSelect = new List<string>();
                m_lstSelect.Add(MenuVM.Prop.MenuID.MapAlias);
                m_lstSelect.Add(MenuVM.Prop.MenuDesc.MapAlias);
                m_lstSelect.Add(MenuVM.Prop.MenuHierarchy.MapAlias);
                m_lstSelect.Add(MenuVM.Prop.MenuUrl.MapAlias);
                m_lstSelect.Add(MenuVM.Prop.MenuIcon.MapAlias);
                m_lstSelect.Add(MenuVM.Prop.MenuVisible.MapAlias);

                m_dicOrder = new Dictionary<string, OrderDirection>();
                m_dicOrder.Add(MenuVM.Prop.MenuHierarchy.Map, OrderDirection.Ascending);

                List<object> m_lstFilter = new List<object>();
                string filterHierarchy = string.Empty;
                List<string> filterList = new List<string>();
                foreach (MenuVM menu in m_lstMenuVM)
                {
                    IEnumerable<string> hierarchies = Enumerable.Range(0, (menu.MenuHierarchy.Length) / 2).Select(i => menu.MenuHierarchy.Substring(0, i * 2));
                    foreach (string hierarchy in hierarchies)
                    {
                        if (hierarchy != string.Empty)
                            filterList.Add(string.Format(" {0} LIKE '{1}' ", MenuVM.Prop.MenuHierarchy.Map, hierarchy));
                    }

                }

                m_objFilter = new Dictionary<string, List<object>>();
                m_lstFilter.Add(Operator.None);
                m_lstFilter.Add(string.Empty);
                m_objFilter.Add(string.Join("OR", filterList), m_lstFilter);


                m_dicDRoleMenuActionDA = m_objSMenuDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
                if (m_objSMenuDA.Message == string.Empty)
                {
                    m_lstMenuParentVM = (
                        from DataRow m_drSMenu in m_dicDRoleMenuActionDA[0].Tables[0].Rows
                        select new MenuVM()
                        {
                            MenuID = m_drSMenu[MenuVM.Prop.MenuID.Name].ToString(),
                            MenuDesc = m_drSMenu[MenuVM.Prop.MenuDesc.Name].ToString(),
                            MenuHierarchy = m_drSMenu[MenuVM.Prop.MenuHierarchy.Name].ToString().Trim(),
                            MenuUrl = m_drSMenu[MenuVM.Prop.MenuUrl.Name].ToString(),
                            MenuIcon = m_drSMenu[MenuVM.Prop.MenuIcon.Name].ToString(),
                            MenuVisible = bool.Parse(m_drSMenu[MenuVM.Prop.MenuVisible.Name].ToString())
                        }).ToList();
                }

            }

            m_lstMenuVM = m_lstMenuVM.Union(m_lstMenuParentVM).ToList();

            return m_lstMenuVM;
        }
        public string GetChildMenu(MenuVM objMenu, List<MenuVM> lstMenu)
        {
            StringBuilder m_sbMenuLi = new StringBuilder();
            string m_strMenuLi = string.Empty;
            List<MenuVM> m_lstChildLevelMenu = lstMenu.Where(d => d.MenuHierarchy.Trim().StartsWith(objMenu.MenuHierarchy) && !d.MenuHierarchy.Trim().Equals(objMenu.MenuHierarchy) && d.MenuHierarchy.Trim().Length == objMenu.MenuHierarchy.Length + 2).ToList();
            m_lstChildLevelMenu = m_lstChildLevelMenu.OrderBy(d => d.MenuHierarchy).ToList();
            if (!m_lstChildLevelMenu.Any())
            {
                m_sbMenuLi.AppendLine(string.Format("<li><a href='{0}'>{1}</a></li>", objMenu.MenuUrl, objMenu.MenuDesc));
            }
            else
            {
                m_sbMenuLi.AppendLine(string.Format(@"<li class='dropdown'><a class='dropdown-toggle' data-toggle='collapse' data-target='#{1}'>{0}<span class='caret'></span></a>
                                <ul class='dropdown-menu collapse' role='menu' id='{1}'>", objMenu.MenuDesc, objMenu.MenuHierarchy));
                foreach (MenuVM menuVM in m_lstChildLevelMenu)
                {
                    m_sbMenuLi.AppendLine(GetChildMenu(menuVM, lstMenu));
                }

                m_sbMenuLi.AppendLine("</ul>");
            }
            m_strMenuLi = m_sbMenuLi.ToString();
            return m_strMenuLi;

        }
        private Node GetNode(MenuVM currentMenu, List<MenuVM> lstChild, MenuVM selectedMenu, List<MenuVM> lstAllMenu)
        {
            Ext.Net.Node m_extNode = new Ext.Net.Node();
            m_extNode.NodeID = currentMenu.MenuID;
            m_extNode.Text = currentMenu.MenuDesc;
            m_extNode.Expandable = true;
            m_extNode.Expanded = true;
            m_extNode.AttributesObject = new
            {
                menuid = currentMenu.MenuID,
                menudesc = currentMenu.MenuDesc,
                menuhierarchy = currentMenu.MenuHierarchy,
                menuurl = currentMenu.MenuUrl,
                menuicon = currentMenu.MenuIcon,
                menuvisible = currentMenu.MenuVisible
            };
            if (lstChild.Any())
            {
                m_extNode.Icon = Icon.Folder;
                if (selectedMenu != null)
                {
                    for (int i = 1; i <= selectedMenu.MenuParent.Length / 2; i++)
                    {
                        if (currentMenu.MenuHierarchy == selectedMenu.MenuParent.Substring(0, i * 2))
                        {
                            m_extNode.Expanded = true;
                        }
                    }
                }
                foreach (MenuVM menu in lstChild)
                {
                    m_extNode.Children.Add(this.GetNode(menu, lstAllMenu.Where(x => x.MenuParent == menu.MenuHierarchy).ToList(), selectedMenu, lstAllMenu));
                }
            }
            else
            {
                m_extNode.Leaf = true;
                //m_extNode.Href = currentMenu.MenuUrl;
                if (currentMenu.MenuHierarchy.Equals("00"))
                    m_extNode.Icon = Icon.ApplicationHome;
                else
                    m_extNode.Icon = Icon.ApplicationViewColumns;
            }
            return m_extNode;
        }

        #endregion
    }
}