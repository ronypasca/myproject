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
    public class UserController : BaseController
    {
        private readonly string title = "User";
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
            string m_strMessage = string.Empty;

            MUserDA m_objMUserDA = new MUserDA();
            m_objMUserDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMUser = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMUser.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = UserVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMUserDA = m_objMUserDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpUserBL in m_dicMUserDA)
            {
                m_intCount = m_kvpUserBL.Key;
                break;
            }

            List<UserVM> m_lstUserVM = new List<UserVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(UserVM.Prop.UserID.MapAlias);
                m_lstSelect.Add(UserVM.Prop.FullName.MapAlias);
                // m_lstSelect.Add(UserVM.Prop.Password.MapAlias);
                m_lstSelect.Add(UserVM.Prop.LastLogin.MapAlias);
                //m_lstSelect.Add(UserVM.Prop.RoleID.MapAlias);
                //m_lstSelect.Add(UserVM.Prop.RoleDesc.MapAlias);
                m_lstSelect.Add(VendorVM.Prop.FirstName.MapAlias);
                m_lstSelect.Add(VendorVM.Prop.LastName.MapAlias);
                m_lstSelect.Add(UserVM.Prop.VendorID.MapAlias);
                m_lstSelect.Add(UserVM.Prop.HostIP.MapAlias);
                m_lstSelect.Add(UserVM.Prop.IsActive.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(UserVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMUserDA = m_objMUserDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMUserDA.Message == string.Empty)
                {
                    m_lstUserVM = (
                        from DataRow m_drMUserDA in m_dicMUserDA[0].Tables[0].Rows
                        select new UserVM()
                        {
                            UserID = m_drMUserDA[UserVM.Prop.UserID.Name].ToString(),
                            FullName = m_drMUserDA[UserVM.Prop.FullName.Name].ToString(),
                            //Password = m_drMUserDA[UserVM.Prop.Password.Name].ToString(),
                            LastLogin = DateTime.Parse(m_drMUserDA[UserVM.Prop.LastLogin.Name].ToString()),
                            VendorID = m_drMUserDA[UserVM.Prop.VendorID.Name].ToString(),
                            VendorDesc = m_drMUserDA[VendorVM.Prop.FirstName.Name].ToString() + " " + m_drMUserDA[VendorVM.Prop.LastName.Name].ToString(),
                            HostIP = m_drMUserDA[UserVM.Prop.HostIP.Name].ToString(),
                            IsActive = Convert.ToBoolean(m_drMUserDA[UserVM.Prop.IsActive.Name].ToString()),
                            UserRoles = GetListUserRoles(m_drMUserDA[UserVM.Prop.UserID.Name].ToString(), ref m_strMessage)
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstUserVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            MUserDA m_objMUserDA = new MUserDA();
            m_objMUserDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMUser = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMUser.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = UserVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMUserDA = m_objMUserDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpUserBL in m_dicMUserDA)
            {
                m_intCount = m_kvpUserBL.Key;
                break;
            }

            List<UserVM> m_lstUserVM = new List<UserVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(UserVM.Prop.UserID.MapAlias);
                m_lstSelect.Add(UserVM.Prop.FullName.MapAlias);
                //m_lstSelect.Add(UserVM.Prop.RoleDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(UserVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMUserDA = m_objMUserDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMUserDA.Message == string.Empty)
                {
                    m_lstUserVM = (
                        from DataRow m_drMUserDA in m_dicMUserDA[0].Tables[0].Rows
                        select new UserVM()
                        {
                            UserID = m_drMUserDA[UserVM.Prop.UserID.Name].ToString(),
                            FullName = m_drMUserDA[UserVM.Prop.FullName.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstUserVM, m_intCount);
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

            UserVM m_objUserVM = new UserVM();
            ViewDataDictionary m_vddUser = new ViewDataDictionary();
            m_vddUser.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddUser.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            if (Caller == General.EnumDesc(Buttons.ButtonDetail))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                Dictionary<string, object> m_dicSelectedRow = GetFormData(m_nvcParams);
                Session[dataSessionName] = JSON.Serialize(m_dicSelectedRow);
            }

            if (m_objUserVM.UserRoles == null)
                m_objUserVM.UserRoles = new List<UserRoleVM>();
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objUserVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddUser,
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
            {
                m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
            }
            else if (Caller == General.EnumDesc(Buttons.ButtonAdd) || Caller == General.EnumDesc(Buttons.ButtonUpdate))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                m_dicSelectedRow = GetFormData(m_nvcParams);
            }

            UserVM m_objUserVM = new UserVM();
            if (m_dicSelectedRow.Count > 0)
                m_objUserVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);


            if (m_objUserVM.UserRoles == null)
                m_objUserVM.UserRoles = new List<UserRoleVM>();

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
                Model = m_objUserVM,
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
            UserVM m_objUserVM = new UserVM();
            if (m_dicSelectedRow.Count > 0)
                m_objUserVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);

            if (m_objUserVM.UserRoles == null)
                m_objUserVM.UserRoles = new List<UserRoleVM>();

            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddUser = new ViewDataDictionary();
            m_vddUser.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddUser.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objUserVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddUser,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<UserVM> m_lstSelectedRow = new List<UserVM>();
            m_lstSelectedRow = JSON.Deserialize<List<UserVM>>(Selected);

            MUserDA m_objMUserDA = new MUserDA();
            m_objMUserDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (UserVM m_objUserVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifUserVM = m_objUserVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifUserVM in m_arrPifUserVM)
                    {
                        string m_strFieldName = m_pifUserVM.Name;
                        object m_objFieldValue = m_pifUserVM.GetValue(m_objUserVM);
                        if (m_objUserVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(UserVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objMUserDA.DeleteBC(m_objFilter, false);
                    if (m_objMUserDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMUserDA.Message);
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

        public ActionResult Browse(string ControlUserID, string ControlFullName, string FilterUserID = "", string FilterFullName = "",string GridCaller="", string ControllerCaller="")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddUser = new ViewDataDictionary();
            m_vddUser.Add("Control" + UserVM.Prop.UserID.Name, ControlUserID);
            m_vddUser.Add("Control" + UserVM.Prop.FullName.Name, ControlFullName);
            m_vddUser.Add(UserVM.Prop.UserID.Name, FilterUserID);
            m_vddUser.Add(UserVM.Prop.FullName.Name, FilterFullName);
            m_vddUser.Add("ControllerCaller", ControllerCaller);
            m_vddUser.Add("GridCaller", GridCaller);
            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddUser,
                ViewName = "../User/_Browse"
            };
        }

        public ActionResult Save(string Action)
        {
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MUserDA m_objMUserDA = new MUserDA();
            m_objMUserDA.ConnectionStringName = Global.ConnStrConfigName;
            DUserRoleDA m_objDUserRoleDA = new DUserRoleDA();
            m_objDUserRoleDA.ConnectionStringName = Global.ConnStrConfigName;

            try
            {
                string m_strUserID = this.Request.Params[UserVM.Prop.UserID.Name];
                string m_strFullName = this.Request.Params[UserVM.Prop.FullName.Name];
                //string m_strRoleID = this.Request.Params[UserVM.Prop.RoleID.Name];
                string m_strVendorID = this.Request.Params[UserVM.Prop.VendorID.Name];
                string m_strPassword = this.Request.Params[UserVM.Prop.Password.Name];
                string m_strlastLogin = this.Request.Params[UserVM.Prop.LastLogin.Name];
                string m_strHostIP = this.Request.Params[UserVM.Prop.HostIP.Name];
                bool m_isActive = Convert.ToBoolean(this.Request.Params[UserVM.Prop.IsActive.Name].ToString());

                string m_strEmployeeID = this.Request.Params[UserVM.Prop.EmployeeID.Name];
                string m_strBusinessUnitID = this.Request.Params[UserVM.Prop.BusinessUnitID.Name];
                string m_strDivisionID = this.Request.Params[UserVM.Prop.DivisionID.Name];
                string m_strProjectID = this.Request.Params[UserVM.Prop.ProjectID.Name];
                string m_strClusterID = this.Request.Params[UserVM.Prop.ClusterID.Name];

                string m_strGrdUserRole = this.Request.Params[UserVM.Prop.UserRole.Name];

                List<UserRoleVM> m_lstUserRoleVM = JSON.Deserialize<List<UserRoleVM>>(m_strGrdUserRole);

                string m_strTransName = "UserRole";
                object m_objDBConnection = null;
                m_objDBConnection = m_objMUserDA.BeginTrans(m_strTransName);

                m_lstMessage = IsSaveValid(Action, m_strUserID, m_strFullName, "", m_strVendorID, m_strlastLogin, m_strPassword);
                if (m_lstMessage.Count <= 0)
                {
                    MUser m_objMUser = new MUser();
                    DateTime m_dateNow = DateTime.Now;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objMUserDA.Select();

                    m_objMUser.UserID = m_strUserID;
                    m_objMUser.FullName = m_strFullName;
                    m_objMUser.VendorID = (m_strVendorID == string.Empty ? null : m_strVendorID);
                    m_objMUser.LastLogin = m_strlastLogin == null ? DateTime.Now : DateTime.Parse(m_strlastLogin);
                    if (m_objMUser.LastLogin == DateTime.MinValue)
                        m_objMUser.LastLogin = DateTime.Now;
                    m_objMUser.IsActive = m_isActive;
                    m_objMUser.Password = Convert.ToBase64String(Encryption.SHA1Encrypt(m_strPassword + Convert.ToBase64String(Encryption.GenerateSalt(m_dateNow.ToString(Global.SqlDateFormat)))));
                    m_objMUser.EmployeeID = m_strEmployeeID == string.Empty ? null : m_strEmployeeID;
                    m_objMUser.BusinessUnitID = m_strBusinessUnitID == string.Empty ? null : m_strBusinessUnitID;
                    m_objMUser.DivisionID = m_strDivisionID == string.Empty ? null : m_strDivisionID;
                    m_objMUser.ProjectID = m_strProjectID == string.Empty ? null : m_strProjectID;
                    m_objMUser.ClusterID = m_strClusterID == string.Empty ? null : m_strClusterID;
                    m_objMUserDA.Data = m_objMUser;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                    {
                        m_objMUser.HostIP = Global.LocalHostIP;
                        m_objMUserDA.Insert(true, m_objDBConnection);
                    }
                    else
                    {
                        m_objMUser.ModifiedDate = m_dateNow;
                        m_objMUser.HostIP = m_strHostIP;
                        m_objMUserDA.Update(true, m_objDBConnection);
                    }
                    if (!m_objMUserDA.Success || m_objMUserDA.Message != string.Empty)
                    {
                        m_objMUserDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                        m_lstMessage.Add(m_objMUserDA.Message);
                    }

                    #region DUserRole
                    if (m_lstUserRoleVM.Any())
                    {
                        Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                        List<object> m_lstFilter = new List<object>();
                        m_lstFilter.Add(Operator.Equals);
                        m_lstFilter.Add(m_objMUser.UserID);
                        m_objFilter.Add(UserRoleVM.Prop.UserID.Map, m_lstFilter);

                        m_objDUserRoleDA.DeleteBC(m_objFilter, true, m_objDBConnection);

                        foreach (UserRoleVM objUserRoleVM in m_lstUserRoleVM)
                        {
                            DUserRole m_objDUserRole = new DUserRole();
                            m_objDUserRoleDA.Data = m_objDUserRole;

                            m_objDUserRole.UserID = m_objMUser.UserID;
                            m_objDUserRole.RoleID = objUserRoleVM.RoleID;

                            m_objDUserRoleDA.Insert(true, m_objDBConnection);

                            if (!m_objDUserRoleDA.Success || m_objDUserRoleDA.Message != string.Empty)
                            {
                                m_objDUserRoleDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                                m_lstMessage.Add(m_objDUserRoleDA.Message);
                            }

                        }

                    }
                    #endregion

                    if (!m_objMUserDA.Success || m_objMUserDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMUserDA.Message);
                    else
                        m_objMUserDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
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

        public ActionResult DeleteUserRole(string Selected)
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));


            List<UserRoleVM> m_lstSelectedRow = new List<UserRoleVM>();
            m_lstSelectedRow = JSON.Deserialize<List<UserRoleVM>>(Selected);

            DUserRoleDA m_objDUserRoleDA = new DUserRoleDA();
            m_objDUserRoleDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (UserRoleVM m_objUserRoleVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifUserRoleVM = m_objUserRoleVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifUserRoleVM in m_arrPifUserRoleVM)
                    {
                        string m_strFieldName = m_pifUserRoleVM.Name;
                        object m_objFieldValue = m_pifUserRoleVM.GetValue(m_objUserRoleVM) ?? string.Empty;
                        if (m_objUserRoleVM.IsKey(m_strFieldName))
                        {

                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(UserRoleVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objDUserRoleDA.DeleteBC(m_objFilter, false);
                    if (m_objDUserRoleDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDUserRoleDA.Message);
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

        public ActionResult GetUser(string ControlUserID, string ControlFullName, string FilterUserID, string FilterFullName, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<UserVM>> m_dicUserData = GetUserData(true, FilterUserID, FilterFullName);
                KeyValuePair<int, List<UserVM>> m_kvpUserVM = m_dicUserData.AsEnumerable().ToList()[0];
                if (m_kvpUserVM.Key < 1 || (m_kvpUserVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpUserVM.Key > 1 && !Exact)
                    return Browse(ControlUserID, ControlFullName, FilterUserID, FilterFullName);

                m_dicUserData = GetUserData(false, FilterUserID, FilterFullName);
                UserVM m_objUserVM = m_dicUserData[0][0];
                this.GetCmp<TextField>(ControlUserID).Value = m_objUserVM.UserID;
                this.GetCmp<TextField>(ControlFullName).Value = m_objUserVM.FullName;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, string UserID, string FullName, string RoleID, string VendorID, string LastLogin, string Password)
        {
            List<string> m_lstReturn = new List<string>();

            if (UserID == string.Empty)
                m_lstReturn.Add(UserVM.Prop.UserID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (FullName == string.Empty)
                m_lstReturn.Add(UserVM.Prop.FullName.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            //if (RoleID == string.Empty)
            //    m_lstReturn.Add(UserVM.Prop.RoleID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (VendorID != string.Empty)
            {
                string m_strMessage = General.IsPasswordValid(Password);
                if (m_strMessage != string.Empty)
                    m_lstReturn.Add(m_strMessage);
            }
            if (LastLogin == string.Empty)
                m_lstReturn.Add(UserVM.Prop.LastLogin.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(UserVM.Prop.UserID.Name, parameters[UserVM.Prop.UserID.Name]);
            m_dicReturn.Add(UserVM.Prop.FullName.Name, parameters[UserVM.Prop.FullName.Name]);
            //m_dicReturn.Add(UserVM.Prop.RoleID.Name, parameters[UserVM.Prop.RoleID.Name]);
            //m_dicReturn.Add(UserVM.Prop.RoleDesc.Name, parameters[UserVM.Prop.RoleDesc.Name]);
            m_dicReturn.Add(UserVM.Prop.VendorID.Name, parameters[UserVM.Prop.VendorID.Name]);
            m_dicReturn.Add(UserVM.Prop.LastLogin.Name, parameters[UserVM.Prop.LastLogin.Name]);
            m_dicReturn.Add(UserVM.Prop.IsActive.Name, parameters[UserVM.Prop.IsActive.Name]);

            return m_dicReturn;
        }

        private UserVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            UserVM m_objUserVM = new UserVM();
            MUserDA m_objMUserDA = new MUserDA();
            m_objMUserDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(UserVM.Prop.UserID.MapAlias);
            m_lstSelect.Add(UserVM.Prop.FullName.MapAlias);
            m_lstSelect.Add(UserVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(VendorVM.Prop.FirstName.MapAlias);
            m_lstSelect.Add(VendorVM.Prop.LastName.MapAlias);
            m_lstSelect.Add(UserVM.Prop.Password.MapAlias);
            m_lstSelect.Add(UserVM.Prop.LastLogin.MapAlias);
            m_lstSelect.Add(UserVM.Prop.HostIP.MapAlias);
            m_lstSelect.Add(UserVM.Prop.IsActive.MapAlias);
            m_lstSelect.Add(UserVM.Prop.EmployeeID.MapAlias);
            m_lstSelect.Add(UserVM.Prop.BusinessUnitID.MapAlias);
            m_lstSelect.Add(UserVM.Prop.DivisionID.MapAlias);
            m_lstSelect.Add(UserVM.Prop.ProjectID.MapAlias);
            m_lstSelect.Add(UserVM.Prop.ClusterID.MapAlias);
            m_lstSelect.Add(EmployeeVM.Prop.EmployeeName.MapAlias);
            m_lstSelect.Add(UserVM.Prop.BusinessUnitDesc.MapAlias);
            m_lstSelect.Add(UserVM.Prop.DivisionDesc.MapAlias);
            m_lstSelect.Add(UserVM.Prop.ProjectDesc.MapAlias);
            m_lstSelect.Add(UserVM.Prop.ClusterDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objUserVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(UserVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMUserDA = m_objMUserDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMUserDA.Message == string.Empty)
            {
                DataRow m_drMUserDA = m_dicMUserDA[0].Tables[0].Rows[0];
                m_objUserVM.UserID = m_drMUserDA[UserVM.Prop.UserID.Name].ToString();
                m_objUserVM.FullName = m_drMUserDA[UserVM.Prop.FullName.Name].ToString();
                m_objUserVM.VendorID = m_drMUserDA[UserVM.Prop.VendorID.Name].ToString();
                m_objUserVM.VendorDesc = m_drMUserDA[VendorVM.Prop.FirstName.Name].ToString() + " " + m_drMUserDA[VendorVM.Prop.LastName.Name].ToString();
                m_objUserVM.Password = string.Empty;
                m_objUserVM.LastLogin = DateTime.Parse(m_drMUserDA[UserVM.Prop.LastLogin.Name].ToString());
                m_objUserVM.HostIP = m_drMUserDA[UserVM.Prop.HostIP.Name].ToString();
                m_objUserVM.IsActive = Convert.ToBoolean(m_drMUserDA[UserVM.Prop.IsActive.Name].ToString());
                m_objUserVM.EmployeeID = m_drMUserDA[UserVM.Prop.EmployeeID.Name].ToString();
                m_objUserVM.BusinessUnitID = m_drMUserDA[UserVM.Prop.BusinessUnitID.Name].ToString();
                m_objUserVM.DivisionID = m_drMUserDA[UserVM.Prop.DivisionID.Name].ToString();
                m_objUserVM.ProjectID = m_drMUserDA[UserVM.Prop.ProjectID.Name].ToString();
                m_objUserVM.ClusterID = m_drMUserDA[UserVM.Prop.ClusterID.Name].ToString();
                m_objUserVM.EmployeeName = m_drMUserDA[UserVM.Prop.EmployeeName.Name].ToString();
                m_objUserVM.BusinessUnitDesc = m_drMUserDA[UserVM.Prop.BusinessUnitDesc.Name].ToString();
                m_objUserVM.DivisionDesc = m_drMUserDA[UserVM.Prop.DivisionDesc.Name].ToString();
                m_objUserVM.ProjectDesc = m_drMUserDA[UserVM.Prop.ProjectDesc.Name].ToString();
                m_objUserVM.ClusterDesc = m_drMUserDA[UserVM.Prop.ClusterDesc.Name].ToString();
                m_objUserVM.UserRoles = GetListUserRoles(m_drMUserDA[UserVM.Prop.UserID.Name].ToString(), ref message);
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMUserDA.Message;

            return m_objUserVM;
        }

        private List<UserRoleVM> GetListUserRoles(string userID, ref string message)
        {
            List<UserRoleVM> m_objListUserRoleVM = new List<UserRoleVM>();
            DUserRoleDA m_objDUserRoleDA = new DUserRoleDA();
            m_objDUserRoleDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(UserRoleVM.Prop.UserID.MapAlias);
            m_lstSelect.Add(UserRoleVM.Prop.RoleID.MapAlias);
            m_lstSelect.Add(UserRoleVM.Prop.RoleDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();


            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(userID);
            m_objFilter.Add(UserRoleVM.Prop.UserID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicDUserRoleDA = m_objDUserRoleDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDUserRoleDA.Message == string.Empty)
            {
                //DataRow m_drUserRoleDA = m_dicDUserRoleDA[0].Tables[0].Rows[0];
                m_objListUserRoleVM = (from DataRow m_drUserRoleDA in m_dicDUserRoleDA[0].Tables[0].Rows
                                       select new UserRoleVM
                                       {
                                           UserID = m_drUserRoleDA[UserRoleVM.Prop.UserID.Name].ToString(),
                                           RoleID = m_drUserRoleDA[UserRoleVM.Prop.RoleID.Name].ToString(),
                                           RoleDesc = m_drUserRoleDA[UserRoleVM.Prop.RoleDesc.Name].ToString()
                                       }).ToList();
            }
            //else
            //message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDUserRoleDA.Message;

            return m_objListUserRoleVM;
        }
        #endregion

        #region Public Method

        public Dictionary<int, List<UserVM>> GetUserData(bool isCount, string UserID, string FullName)
        {
            int m_intCount = 0;
            List<UserVM> m_lstUserVM = new List<ViewModels.UserVM>();
            Dictionary<int, List<UserVM>> m_dicReturn = new Dictionary<int, List<UserVM>>();
            MUserDA m_objMUserDA = new MUserDA();
            m_objMUserDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(UserVM.Prop.UserID.MapAlias);
            m_lstSelect.Add(UserVM.Prop.FullName.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(UserID);
            m_objFilter.Add(UserVM.Prop.UserID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(FullName);
            m_objFilter.Add(UserVM.Prop.FullName.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMUserDA = m_objMUserDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMUserDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpUserBL in m_dicMUserDA)
                    {
                        m_intCount = m_kvpUserBL.Key;
                        break;
                    }
                else
                {
                    m_lstUserVM = (
                        from DataRow m_drMUserDA in m_dicMUserDA[0].Tables[0].Rows
                        select new UserVM()
                        {
                            UserID = m_drMUserDA[UserVM.Prop.UserID.Name].ToString(),
                            FullName = m_drMUserDA[UserVM.Prop.FullName.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstUserVM);
            return m_dicReturn;
        }

        #endregion
    }
}