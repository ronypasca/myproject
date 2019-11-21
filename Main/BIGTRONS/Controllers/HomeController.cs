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
using SW = System.Web;
using System.DirectoryServices;

namespace com.SML.BIGTRONS.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string m_strError = Request.QueryString["e"];
            object m_objMessage = string.Empty;
            switch (m_strError)
            {
                case "0":
                    m_objMessage = General.EnumDesc(MessageLib.NotAuthenticated);
                    break;
                case "1":
                    m_objMessage = General.EnumDesc(MessageLib.NotAuthorized);
                    break;
            }
            ViewBag.Title = Global.GetTitle(this.GetType());
            return View(m_objMessage);
        }

        public ActionResult Home()
        {
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumDesc(Params.Home),
                RenderMode = RenderMode.AddTo,
                ViewName = SW.HttpContext.Current.User.Identity.IsAuthenticated ? "_Home" : "_Login",
                WrapByScriptTag = false
            };
        }

        public ActionResult Login()
        {
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumDesc(Params.HomePage),
                RenderMode = RenderMode.AddTo,
                ViewName = SW.HttpContext.Current.User.Identity.IsAuthenticated ? "_Home" : "_Login",
                WrapByScriptTag = false
            };
        }

        public ActionResult Verify()
        {
            try
            {
                UserVM m_objUserVM = new UserVM();
                string m_strUserID = this.Request.Params[UserVM.Prop.UserID.Name];
                string m_strPassword = this.Request.Params[UserVM.Prop.Password.Name];
                bool m_boolIsRemember = Boolean.Parse(this.Request.Params[UserVM.Prop.IsRemember.Name]);

                List<string> m_lstResult = IsSaveValid(m_strUserID, m_strPassword);
                if (m_lstResult.Count > 0)
                    return this.Direct(false, String.Join(Global.NewLineSeparated, m_lstResult));

                m_strUserID = m_strUserID.Substring(m_strUserID.LastIndexOf('\\') + 1);
                MUser m_objMUser = new MUser();
                m_objUserVM.UserID = m_strUserID;
                string m_strResult = IsAuthenticated(m_strUserID, m_strPassword, ref m_objUserVM);
                if (m_strResult == string.Empty)
                {
                    string m_strRoleIDs = String.Join(Global.OneLineSeparated, (
                        from m_objUserRole in m_objUserVM.UserRoles
                        select m_objUserRole.RoleID
                        ).ToList());

                    Global.HoldLoginData(m_strRoleIDs, m_strUserID, m_objUserVM.FullName, m_strPassword, m_boolIsRemember);

                    DateTime m_dateNow = DateTime.Now;
                    m_objMUser.LastLogin = m_dateNow;
                    m_objMUser.ModifiedBy = m_strUserID;
                    m_objMUser.Password = Convert.ToBase64String(Encryption.SHA1Encrypt(m_strPassword + Convert.ToBase64String(Encryption.GenerateSalt(m_dateNow.ToString(Global.SqlDateFormat)))));
                    m_objMUser.ModifiedDate = m_dateNow;
                    m_objMUser.ModifiedHost = Global.LocalHostName;
                    MUserDA m_objMUserDA = new MUserDA();
                    m_objMUserDA.ConnectionStringName = Global.ConnStrConfigName;
                    m_objMUserDA.Data = m_objMUser;
                    m_objMUserDA.Update(false);

                    string m_strReferer = SW.HttpContext.Current.Request.UrlReferrer.Query;
                    m_strReferer = SW.HttpUtility.UrlDecode(m_strReferer);

                    List<string> m_lstRedirectURL = new List<string>();
                    m_lstRedirectURL.Add("");
                    if (m_strReferer != string.Empty)
                    {
                        int m_intStart = m_strReferer.IndexOf("?ReturnUrl=") + 11;
                        int m_intSeparatorPosition = m_strReferer.IndexOf("&", m_intStart);
                        string m_strRedirectURL = m_strReferer.Substring(m_intStart,
                            (m_intSeparatorPosition >= 0 ? m_intSeparatorPosition : m_strReferer.Length) - m_intStart);
                        if (m_strRedirectURL.StartsWith("/"))
                            m_strRedirectURL = m_strRedirectURL.Substring(1);
                        m_lstRedirectURL = m_strRedirectURL.Split('/').ToList();
                    }
                    return RedirectToAction(m_lstRedirectURL.Count > 1 ? m_lstRedirectURL[1] : "", m_lstRedirectURL[0]);
                }
                else
                {
                    Global.ShowErrorAlert("Home", m_strResult);
                    return this.Direct();
                }
            }
            catch (Exception ex)
            {
                Global.ShowErrorAlert("Home", ex.Message);
                return this.Direct();
            }
        }

        public ActionResult Logout()
        {
            System.Web.Security.FormsAuthentication.SignOut();
            Global.RewriteLoginCache(true);
            return RedirectToAction("Index", "Home");
        }

        private List<string> IsSaveValid(string UserID, string Password)
        {
            List<string> m_lstReturn = new List<string>();

            if (UserID == string.Empty)
                m_lstReturn.Add("User ID " + General.EnumDesc(MessageLib.mustFill));
            if (Password == string.Empty)
                m_lstReturn.Add("Password " + General.EnumDesc(MessageLib.mustFill));

            return m_lstReturn;
        }

        public string IsAuthenticated(string userID, string password, ref UserVM userVM)
        {
            string m_strReturn = string.Empty;
            string m_strDisplayName = string.Empty;

            MUserDA m_objMUserDA = new MUserDA();
            MUser user = new MUser();
            user.UserID = userVM.UserID;
            m_objMUserDA.Data = user;
            m_objMUserDA.ConnectionStringName = Global.ConnStrConfigName;
            m_objMUserDA.Select();
            user = m_objMUserDA.Data;

            userVM.FullName = user.FullName;

            //User Role
            UserRoleVM m_objUserRoleVM = new UserRoleVM();
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(UserRoleVM.Prop.RoleID.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(userID);
            m_objFilter.Add(UserRoleVM.Prop.UserID.Map, m_lstFilter);

            DUserRoleDA m_objDUserRoleDA = new DUserRoleDA();
            m_objDUserRoleDA.ConnectionStringName = Global.ConnStrConfigName;
            Dictionary<int, DataSet> m_dicMUser = m_objDUserRoleDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null,null,null);
            if (m_objDUserRoleDA.Message != string.Empty)
                m_strReturn = m_objDUserRoleDA.Message;
            else if (m_dicMUser[0] != null && m_dicMUser[0].Tables.Count > 0 && m_dicMUser[0].Tables[0].Rows.Count > 0)
            {
                List<UserRoleVM> m_lstUserRoleVM = (
                    from DataRow m_drMUser in m_dicMUser[0].Tables[0].Rows
                    select new UserRoleVM()
                    {
                        RoleID = m_drMUser[UserRoleVM.Prop.RoleID.Name].ToString()
                    }).ToList();
                userVM.UserRoles = m_lstUserRoleVM;
            }

            if (!m_objMUserDA.Success && m_objMUserDA.Message != string.Empty)
                m_strReturn = General.EnumDesc(MessageLib.NotExist);
            else
                if (user.VendorID != null)
            {
                string m_strPasswordEncrypted = Convert.ToBase64String(Encryption.SHA1Encrypt(password + Convert.ToBase64String(Encryption.GenerateSalt(user.ModifiedDate.Value.ToString(Global.SqlDateFormat)))));
                if (!m_strPasswordEncrypted.Equals(user.Password))
                    m_strReturn = UserVM.Prop.Password.Name + General.EnumDesc(MessageLib.notMatched);
            }
            else
            {
                try
                {
                    ConfigController m_objConfigController = new ConfigController();
                    string m_strLDAPPath = m_objConfigController.GetLDAPPath();

                    using (DirectoryEntry m_dieLDAP = new DirectoryEntry(m_strLDAPPath, userID, password))
                    {
                        using (DirectorySearcher m_disLDAP = new DirectorySearcher(m_dieLDAP))
                        {
                            m_disLDAP.Filter = "(SAMAccountName=" + userID + ")";
                            m_disLDAP.PropertiesToLoad.Clear();
                            m_disLDAP.PropertiesToLoad.Add("displayName");
                            SearchResult m_srsLDAP = m_disLDAP.FindOne();
                            m_strDisplayName = ((ResultPropertyValueCollection)m_srsLDAP.Properties["displayName"])[0].ToString();
                        }
                    }

                    if (m_objMUserDA.Message != string.Empty)
                        m_strReturn = m_objMUserDA.Message;
                    else if (!m_objMUserDA.Data.IsActive)
                        m_strReturn = "User is not active!";
                    user.FullName = m_strDisplayName;
                }
                catch (DirectoryServicesCOMException ex)
                {
                    m_strReturn = ex.Message;
                }
                catch (Exception ex)
                {
                    m_strReturn = ex.Message;
                }
            }
            return m_strReturn;
        }
    }
}