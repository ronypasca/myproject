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

namespace com.SML.BIGTRONS.Controllers
{
    public class DBConfigController : Controller
    {
        private string title = "Database Configuration";

        private string DoValidation(Dictionary<string, object> data)
        {
            List<string> m_lstMessage = new List<string>();
            DBConfigVM m_objDBConfigVM = new DBConfigVM();

            string m_strAccessPassword = data[DBConfigVM.Prop.AccessPassword.Name].ToString();
            string m_strServerName = data[DBConfigVM.Prop.ServerName.Name].ToString().Trim();
            string m_strDatabaseName = data[DBConfigVM.Prop.DatabaseName.Name].ToString().Trim();
            string m_strUserID = data[DBConfigVM.Prop.UserID.Name].ToString().Trim();
            string m_strDatabasePassword = data[DBConfigVM.Prop.DatabasePassword.Name].ToString();

            if (m_strAccessPassword == string.Empty)
                m_lstMessage.Add(GetMappedDisplayName(DBConfigVM.Prop.AccessPassword.Name) + " " + General.EnumDesc(MessageLib.mustFill));
            if (m_strServerName == string.Empty)
                m_lstMessage.Add(GetMappedDisplayName(DBConfigVM.Prop.ServerName.Name) + " " + General.EnumDesc(MessageLib.mustFill));
            if (m_strDatabaseName == string.Empty)
                m_lstMessage.Add(GetMappedDisplayName(DBConfigVM.Prop.DatabaseName.Name) + " " + General.EnumDesc(MessageLib.mustFill));
            if (m_strUserID == string.Empty)
                m_lstMessage.Add(GetMappedDisplayName(DBConfigVM.Prop.UserID.Name) + " " + General.EnumDesc(MessageLib.mustFill));
            if (m_strDatabasePassword == string.Empty)
                m_lstMessage.Add(GetMappedDisplayName(DBConfigVM.Prop.DatabasePassword.Name) + " " + General.EnumDesc(MessageLib.mustFill));

            return String.Join(Global.NewLineSeparated, m_lstMessage.ToArray());
        }

        private string GetMappedDisplayName(string columnName)
        {
            DBConfigVM m_objDBConfigVM = new DBConfigVM();
            string m_strReturn = string.Empty;

            if (columnName == DBConfigVM.Prop.AccessPassword.Name)
                m_strReturn = "Access Password";
            else if (columnName == DBConfigVM.Prop.ServerName.Name)
                m_strReturn = "Server Name";
            else if (columnName == DBConfigVM.Prop.DatabaseName.Name)
                m_strReturn = "Database Name";
            else if (columnName == DBConfigVM.Prop.UserID.Name)
                m_strReturn = "User ID";
            else if (columnName == DBConfigVM.Prop.DatabasePassword.Name)
                m_strReturn = "Database Password";

            return m_strReturn;
        }

        // GET: DBConfigDA
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Form()
        {
            DBConfigDA m_objDBConfigDA = new DBConfigDA();
            m_objDBConfigDA.ConnectionStringName = Global.ConnStrConfigName;

            DBConfigVM m_objDBConfigVM = new DBConfigVM();
            m_objDBConfigVM.AccessPassword = string.Empty;
            m_objDBConfigVM.ServerName = m_objDBConfigDA.ServerName;
            m_objDBConfigVM.DatabaseName = m_objDBConfigDA.DatabaseName;
            m_objDBConfigVM.UserID = m_objDBConfigDA.UserID;
            m_objDBConfigVM.DatabasePassword = string.Empty;

            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumDesc(Params.Home),
                Model = m_objDBConfigVM,
                RenderMode = RenderMode.AddTo,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public DirectResult Save()
        {
            List<string> m_lstMessage = new List<string>();
            string m_strSelectedDatas = this.Request.Params[General.EnumDesc(Params.Selected)];
            Dictionary<string, object> m_dicData = JSON.Deserialize<Dictionary<string, object>>(m_strSelectedDatas);

            string m_strValidationMessage = DoValidation(m_dicData);
            if (m_strValidationMessage != string.Empty)
                Global.ShowErrorAlert(title, m_strValidationMessage);
            else
            {
                DBConfigVM m_objDBConfigVM = new DBConfigVM();

                string m_strAccessPassword = m_dicData[DBConfigVM.Prop.AccessPassword.Name].ToString();
                string m_strServerName = m_dicData[DBConfigVM.Prop.ServerName.Name].ToString().Trim();
                string m_strDatabaseName = m_dicData[DBConfigVM.Prop.DatabaseName.Name].ToString().Trim();
                string m_strUserID = m_dicData[DBConfigVM.Prop.UserID.Name].ToString().Trim();
                string m_strDatabasePassword = m_dicData[DBConfigVM.Prop.DatabasePassword.Name].ToString();

                StringBuilder m_stbConnectionString = new StringBuilder();
                m_stbConnectionString.Append("server="); m_stbConnectionString.Append(m_strServerName);
                m_stbConnectionString.Append(";database="); m_stbConnectionString.Append(m_strDatabaseName);
                m_stbConnectionString.Append(";uid="); m_stbConnectionString.Append(m_strUserID);
                m_stbConnectionString.Append(";pwd="); m_stbConnectionString.Append(m_strDatabasePassword);

                DBConfigDA m_objDBConfigDA = new DBConfigDA();
                m_objDBConfigDA.ConnectionStringName = Global.ConnStrConfigName;
                m_objDBConfigDA.WriteSettingFile(m_stbConnectionString.ToString(), m_strAccessPassword);
                if (m_objDBConfigDA.Message != "")
                    Global.ShowErrorAlert(title, String.Join(Global.NewLineSeparated, m_objDBConfigDA.Message));
                else
                    Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Saved));
            }

            return this.Direct();
        }

        public ActionResult Cancel()
        {
            DBConfigDA m_objDBConfigDA = new DBConfigDA();
            m_objDBConfigDA.ConnectionStringName = Global.ConnStrConfigName;

            return this.Store(new DBConfigVM()
            {
                AccessPassword = string.Empty,
                ServerName = m_objDBConfigDA.ServerName,
                DatabaseName = m_objDBConfigDA.DatabaseName,
                UserID = m_objDBConfigDA.UserID,
                DatabasePassword = string.Empty
            });
        }
    }
}