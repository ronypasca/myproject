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
    public class ChangePasswordController : BaseController
    {
        private readonly string title = "ChangePassword";
        private readonly string dataSessionName = "FormData";
        public ActionResult Index()
        {
            base.Initialize();
            return View();
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

        public ActionResult ChangePassword()
        {

            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumDesc(Params.HomePage),
                RenderMode = RenderMode.AddTo,
                ViewName = SW.HttpContext.Current.User.Identity.IsAuthenticated ? "_ChangePassword" : "_Login",
                WrapByScriptTag = false
            };
        }

        public ActionResult Save()
        {
            List<string> m_lstMessage = new List<string>();
            try
            {
                UserVM m_objUserVM = new UserVM();

                MUserDA m_objMUserDA = new MUserDA();
                m_objMUserDA.ConnectionStringName = Global.ConnStrConfigName;
                string m_strOldPassword = this.Request.Params[UserVM.Prop.Password.Name];
                string m_strNewPassword = this.Request.Params[UserVM.Prop.NewPassword.Name];
                string m_strReTypeNewPassword = this.Request.Params[UserVM.Prop.ReTypeNewPassword.Name];

                m_lstMessage = IsSaveValid(m_strOldPassword, m_strNewPassword, m_strReTypeNewPassword);
                if (m_lstMessage.Count > 0)
                {
                    Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
                    return this.Direct();
                }
                
                MUser m_objMUser = new MUser();
                m_objMUser.UserID = Global.LoggedInUser;
                m_objMUserDA.Data = m_objMUser;
                m_objMUserDA.Select();

                string m_strPasswordEncrypted = Convert.ToBase64String(Encryption.SHA1Encrypt(m_strOldPassword + Convert.ToBase64String(Encryption.GenerateSalt(m_objMUser.ModifiedDate.Value.ToString(Global.SqlDateFormat)))));
                if (m_objMUser.Password != m_strPasswordEncrypted)
                {
                    Global.ShowErrorAlert(title, "Old " + UserVM.Prop.Password.Desc + " " + General.EnumDesc(MessageLib.notMatched));
                    return this.Direct(true);
                }

                DateTime m_dateNow = DateTime.Now;
                m_objMUser.LastLogin = m_dateNow;
                m_objMUser.Password = Convert.ToBase64String(Encryption.SHA1Encrypt(m_strNewPassword + Convert.ToBase64String(Encryption.GenerateSalt(m_dateNow.ToString(Global.SqlDateFormat)))));
                m_objMUser.ModifiedDate = m_dateNow;
                m_objMUser.ModifiedHost = Global.LocalHostName;

                m_objMUserDA.Update(false);

                if (!m_objMUserDA.Success)
                {
                    m_lstMessage.Add(m_objMUserDA.Message);
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
            }
            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Updated));
            }
            else
            {
                Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            }
            return this.Direct(true);
        }
        private List<string> IsSaveValid(string OldPassword, string NewPassword,string RetypeNewPassword = "")
        {
            List<string> m_lstReturn = new List<string>();

            if (OldPassword == string.Empty)
                m_lstReturn.Add(UserVM.Prop.NewPassword.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (NewPassword == string.Empty)
                m_lstReturn.Add(UserVM.Prop.NewPassword.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (RetypeNewPassword == string.Empty)
                m_lstReturn.Add(UserVM.Prop.NewPassword.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if(NewPassword != string.Empty && RetypeNewPassword != string.Empty && NewPassword != RetypeNewPassword)
                m_lstReturn.Add(UserVM.Prop.NewPassword.Desc + " " + General.EnumDesc(MessageLib.notMatched));

            return m_lstReturn;
        }
        
    }
}