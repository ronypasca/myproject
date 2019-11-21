using Ciloci.Flee;
using com.SML.BIGTRONS.Models;
using com.SML.BIGTRONS.ViewModels;
using com.SML.Lib.Common;
using Ext.Net;
using Ext.Net.MVC;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using SX = System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.Xsl;
using SW = System.Web;
using System.Xml.XPath;
using com.SML.BIGTRONS.DataAccess;
using System.Text.RegularExpressions;
using com.SML.BIGTRONS.Hubs;
using System.Net;
using System.Text;
using com.SML.BIGTRONS.Enum;

namespace com.SML.BIGTRONS.Controllers
{
    public class BaseController : Controller
    {
        private const string _STRMENUIDPRE = "mnu";
        /// <summary>
        /// To check if the user is authorized to access the page
        /// </summary>
        /// <returns></returns>
        private bool IsAuthorized()
        {
            bool m_boolReturn = false;

            try
            {
                //string m_strRoleID = Request.Cookies[FormsAuthentication.FormsCookieName + ".RoleID"].Value;
                string m_strMenuUrl = Global.MenuUrl;
                DRoleMenuActionDA m_objDRoleMenuActionDA = new DRoleMenuActionDA();
                m_objDRoleMenuActionDA.ConnectionStringName = Global.ConnStrConfigName;

                Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.In);
                m_lstFilter.Add(Global.LoggedInRoleID);
                m_objFilter.Add(RoleMenuActionVM.Prop.RoleID.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(m_strMenuUrl);
                m_objFilter.Add(RoleMenuActionVM.Prop.MenuUrl.Map, m_lstFilter);

                Dictionary<int, DataSet> m_dicDRoleMenuActionDA = m_objDRoleMenuActionDA.SelectBC(0, null, true, null, m_objFilter, null, null, null);
                int m_intCount = 0;
                foreach (KeyValuePair<int, DataSet> m_kvpDRoleMenuActionDA in m_dicDRoleMenuActionDA)
                {
                    m_intCount = m_kvpDRoleMenuActionDA.Key;
                    break;
                }
                m_boolReturn = m_intCount > 0;
                //message = m_objDRoleMenuActionDA.Message;
            }
            catch (Exception ex)
            {
                //message = ex.Message;
            }

            return m_boolReturn;
        }

        /// <summary>
        /// Check user identity everytime open new View
        /// </summary>
        protected void Initialize()
        {
            try
            {
                string m_strReturnUrl = string.Empty;
                if (!SW.HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    object m_objMessage = "Please login to access this page";
                    m_strReturnUrl = "~/" + "?ReturnUrl=" + Request.FilePath.ToString() + "&e=0";
                    Response.Redirect(m_strReturnUrl);
                }
                else
                {
                    string m_strMessage = string.Empty;
                    if (!IsAuthorized())
                    {
                        m_strReturnUrl = "~/" + "?ReturnUrl=" + Request.FilePath.ToString() + "&e=1";
                        Response.Redirect(m_strReturnUrl);
                    }
                }

                ViewBag.Title = Global.GetTitle(this.GetType());
            }
            catch (Exception ex)
            {

            }
        }

        public NegotiationHub NegotiationHub { get { return new NegotiationHub(); } }

        /// <summary>
        /// Method to check if current user on current menu has access as parsed
        /// </summary>
        /// <param name="actionID">Action ID to check</param>
        /// <returns>Has access or not</returns>
        protected bool HasAccess(string actionID)
        {
            bool m_boolReturn = false;

            try
            {
                //string m_strRoleID = Request.Cookies[FormsAuthentication.FormsCookieName + ".RoleID"].Value;
                //string m_strMenuUrl = (Request.UrlReferrer == null || !Request.Url.AbsoluteUri.Contains(Request.UrlReferrer.AbsoluteUri) ?
                //    Request.FilePath : Request.UrlReferrer.AbsolutePath);
                //string m_strMenuUrl = "/" + this.GetType().Name.Replace("Controller", "");
                string m_strMenuID = _STRMENUIDPRE + this.GetType().Name.Replace("Controller", "");
                DRoleMenuActionDA m_objDRoleMenuActionDA = new DRoleMenuActionDA();
                m_objDRoleMenuActionDA.ConnectionStringName = Global.ConnStrConfigName;

                Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.In);
                m_lstFilter.Add(Global.LoggedInRoleID);
                m_objFilter.Add(RoleMenuActionVM.Prop.RoleID.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(m_strMenuID);
                m_objFilter.Add(RoleMenuActionVM.Prop.MenuID.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(actionID);
                m_objFilter.Add(RoleMenuActionVM.Prop.ActionID.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add("true");
                m_objFilter.Add("'" + SW.HttpContext.Current.User.Identity.IsAuthenticated.ToString().ToLower() + "'", m_lstFilter);

                Dictionary<int, DataSet> m_dicDRoleMenuActionDA = m_objDRoleMenuActionDA.SelectBC(0, null, true, null, m_objFilter, null, null, null);
                int m_intCount = 0;
                foreach (KeyValuePair<int, DataSet> m_kvpDRoleMenuActionDA in m_dicDRoleMenuActionDA)
                {
                    m_intCount = m_kvpDRoleMenuActionDA.Key;
                    break;
                }
                m_boolReturn = m_intCount > 0;
            }
            catch (Exception ex)
            {
                return false;
            }

            return m_boolReturn;
        }

        /// <summary>
        /// Method to get list of access (Action ID) for current user on current menu
        /// </summary>
        /// <returns>Access (Action ID) List for current user on current menu</returns>
        protected List<string> GetActionList()
        {
            List<string> m_lstReturn = new List<string>();

            try
            {
                //string m_strRoleID = Request.Cookies[FormsAuthentication.FormsCookieName + ".RoleID"].Value;
                //string m_strMenuUrl = (Request.UrlReferrer == null || !Request.Url.AbsoluteUri.Contains(Request.UrlReferrer.AbsoluteUri) ?
                //    Request.FilePath : Request.UrlReferrer.AbsolutePath);
                //string m_strMenuUrl = "/" + this.GetType().Name.Replace("Controller", "");
                string m_strMenuID = _STRMENUIDPRE + this.GetType().Name.Replace("Controller", "");
                DRoleMenuActionDA m_objDRoleMenuActionDA = new DRoleMenuActionDA();
                m_objDRoleMenuActionDA.ConnectionStringName = Global.ConnStrConfigName;

                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(RoleMenuActionVM.Prop.ActionID.MapAlias);

                Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.In);
                m_lstFilter.Add(Global.LoggedInRoleID);
                m_objFilter.Add(RoleMenuActionVM.Prop.RoleID.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(m_strMenuID);
                m_objFilter.Add(RoleMenuActionVM.Prop.MenuID.Map, m_lstFilter);

                Dictionary<int, DataSet> m_dicDRoleMenuActionDA = m_objDRoleMenuActionDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
                if (m_objDRoleMenuActionDA.Message == string.Empty)
                    m_lstReturn = m_dicDRoleMenuActionDA[0].Tables[0].AsEnumerable().Select(m_drDRoleMenuActionDA
                        => m_drDRoleMenuActionDA[RoleMenuActionVM.Prop.ActionID.Name].ToString()).ToList();
            }
            catch (Exception ex)
            {

            }

            return m_lstReturn;
        }
        //protected byte[] ConvertHASHStringToByte(string str)
        //{
        //    String[] arr = str.Split('-');
        //    byte[] array = new byte[arr.Length];
        //    for (int i = 0; i < arr.Length; i++)
        //        array[i] = Convert.ToByte(arr[i], 16);
        //    return array;
        //}
        //protected string ConvertByteToString(byte[] bytes)
        //{            
        //    return BitConverter.ToString(bytes);
        //}

        protected string GetBase64String(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }
        protected byte[] GetByteFromBase64STR(string Base64String)
        {
            return Convert.FromBase64String(Base64String);
        }
        /// <summary>
        /// Method to get list of access (Action ID) for current user on current menu
        /// </summary>
        /// 
        /// <returns>Access (Action ID) List for current user on current menu</returns>
        protected HasAccessVM GetHasAccess()
        {
            List<string> m_lstAccess = GetActionList();
            HasAccessVM m_objHasAccessVM = new HasAccessVM();
            m_objHasAccessVM.Add = m_lstAccess.Contains(General.GetVariableName(() => m_objHasAccessVM.Add));
            m_objHasAccessVM.Cancel = m_lstAccess.Contains(General.GetVariableName(() => m_objHasAccessVM.Cancel));
            m_objHasAccessVM.Delete = m_lstAccess.Contains(General.GetVariableName(() => m_objHasAccessVM.Delete));
            m_objHasAccessVM.Generate = m_lstAccess.Contains(General.GetVariableName(() => m_objHasAccessVM.Generate));
            m_objHasAccessVM.Preview = m_lstAccess.Contains(General.GetVariableName(() => m_objHasAccessVM.Preview));
            m_objHasAccessVM.Print = m_lstAccess.Contains(General.GetVariableName(() => m_objHasAccessVM.Print));
            m_objHasAccessVM.Read = m_lstAccess.Contains(General.GetVariableName(() => m_objHasAccessVM.Read));
            m_objHasAccessVM.Verify = m_lstAccess.Contains(General.GetVariableName(() => m_objHasAccessVM.Verify));
            m_objHasAccessVM.Unverify = m_lstAccess.Contains(General.GetVariableName(() => m_objHasAccessVM.Unverify));
            m_objHasAccessVM.Update = m_lstAccess.Contains(General.GetVariableName(() => m_objHasAccessVM.Update));
            m_objHasAccessVM.Upload = m_lstAccess.Contains(General.GetVariableName(() => m_objHasAccessVM.Upload));
            m_objHasAccessVM.Export = m_lstAccess.Contains(General.GetVariableName(() => m_objHasAccessVM.Export));
            m_objHasAccessVM.Mapping = m_lstAccess.Contains(General.GetVariableName(() => m_objHasAccessVM.Mapping));
            m_objHasAccessVM.Post = m_lstAccess.Contains(General.GetVariableName(() => m_objHasAccessVM.Post));
            m_objHasAccessVM.Reverse = m_lstAccess.Contains(General.GetVariableName(() => m_objHasAccessVM.Reverse));

            //m_objHasAccessVM.Add = true;
            //m_objHasAccessVM.Cancel = true;
            //m_objHasAccessVM.Delete = true;
            //m_objHasAccessVM.Generate = true;
            //m_objHasAccessVM.Preview = true;
            //m_objHasAccessVM.Print = true;
            //m_objHasAccessVM.Read = true;
            //m_objHasAccessVM.Verify = true;
            //m_objHasAccessVM.Unverify = true;
            //m_objHasAccessVM.Update = true;
            //m_objHasAccessVM.Upload = true;
            //m_objHasAccessVM.Export = true;
            //m_objHasAccessVM.Mapping = true;
            //m_objHasAccessVM.Post = true;
            //m_objHasAccessVM.Reverse = true;
            return m_objHasAccessVM;
        }

        /// <summary>
        /// Method to get object's value for current user on current menu
        /// </summary>
        /// <param name="objectID">Object ID that its value will be retrieved</param>
        /// <returns>Value of object</returns>
        protected string GetMenuObject(string objectID)
        {
            string m_strReturn = string.Empty;

            try
            {
                //string m_strRoleID = Request.Cookies[FormsAuthentication.FormsCookieName + ".RoleID"].Value;
                //string m_strMenuUrl = (Request.UrlReferrer == null || !Request.Url.AbsoluteUri.Contains(Request.UrlReferrer.AbsoluteUri) ?
                //    Request.FilePath : Request.UrlReferrer.AbsolutePath);
                //string m_strMenuUrl = "/" + this.GetType().Name.Replace("Controller", "");
                string m_strMenuID = _STRMENUIDPRE + this.GetType().Name.Replace("Controller", "");
                DRoleMenuObjectDA m_objDRoleMenuObjectDA = new DRoleMenuObjectDA();
                m_objDRoleMenuObjectDA.ConnectionStringName = Global.ConnStrConfigName;

                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(RoleMenuObjectVM.Prop.Value.MapAlias);

                Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.In);
                m_lstFilter.Add(Global.LoggedInRoleID);
                m_objFilter.Add(RoleMenuObjectVM.Prop.RoleID.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(m_strMenuID);
                m_objFilter.Add(RoleMenuObjectVM.Prop.MenuID.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(objectID);
                m_objFilter.Add(RoleMenuObjectVM.Prop.ObjectID.Map, m_lstFilter);

                Dictionary<int, DataSet> m_dicDRoleMenuActionDA = m_objDRoleMenuObjectDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
                if (m_objDRoleMenuObjectDA.Message == string.Empty)
                    m_strReturn = m_dicDRoleMenuActionDA[0].Tables[0].Rows[0][RoleMenuObjectVM.Prop.Value.Name].ToString();
            }
            catch (Exception ex)
            {

            }

            return m_strReturn;
        }

        /// <summary>
        /// Method to get object's value for current user on current menu
        /// </summary>
        /// <param name="objectID">Object ID that its value will be retrieved</param>
        /// <returns>Value of object</returns>
        protected Dictionary<string, string> GetMenuObjectList()
        {
            Dictionary<string, string> m_dicReturn = new Dictionary<string, string>();

            try
            {
                //string m_strRoleID = Request.Cookies[FormsAuthentication.FormsCookieName + ".RoleID"].Value;
                //string m_strMenuUrl = (Request.UrlReferrer == null || !Request.Url.AbsoluteUri.Contains(Request.UrlReferrer.AbsoluteUri) ?
                //    Request.FilePath : Request.UrlReferrer.AbsolutePath);
                //string m_strMenuUrl = "/" + this.GetType().Name.Replace("Controller", "");
                string m_strMenuID = _STRMENUIDPRE + this.GetType().Name.Replace("Controller", "");
                DRoleMenuObjectDA m_objDRoleMenuObjectDA = new DRoleMenuObjectDA();
                m_objDRoleMenuObjectDA.ConnectionStringName = Global.ConnStrConfigName;

                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(RoleMenuObjectVM.Prop.ObjectID.MapAlias);
                m_lstSelect.Add(RoleMenuObjectVM.Prop.Value.MapAlias);

                Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.In);
                m_lstFilter.Add(Global.LoggedInRoleID);
                m_objFilter.Add(RoleMenuObjectVM.Prop.RoleID.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(m_strMenuID);
                m_objFilter.Add(RoleMenuObjectVM.Prop.MenuID.Map, m_lstFilter);

                Dictionary<int, DataSet> m_dicDRoleMenuActionDA = m_objDRoleMenuObjectDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
                if (m_objDRoleMenuObjectDA.Message == string.Empty)
                    m_dicReturn = m_dicDRoleMenuActionDA[0].Tables[0].AsEnumerable().ToDictionary(
                        m_drDRoleMenuActionDA => m_drDRoleMenuActionDA[RoleMenuObjectVM.Prop.ObjectID.Name].ToString(),
                        m_drDRoleMenuActionDA => m_drDRoleMenuActionDA[RoleMenuObjectVM.Prop.Value.Name].ToString()
                        );
            }
            catch (Exception ex)
            {

            }

            return m_dicReturn;
        }

        protected bool IsAggregate(string columnName)
        {
            columnName = columnName.Replace(" ", "");
            foreach (string m_strFormulaName in Global.AggregateFunction)
            {
                if (columnName.Contains(m_strFormulaName))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Method to get List of Budget Plan Version from Budget Plan Package
        /// </summary>
        /// <param name="packageID">Package ID to filter</param>
        /// <returns>List Budget Plan Version</returns>
        protected List<PackageListVM> GetPackageList(string packageID)
        {
            DPackageListDA m_objDPackageListDA = new DPackageListDA();
            m_objDPackageListDA.ConnectionStringName = Global.ConnStrConfigName;

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(packageID);
            m_objFilter.Add(PackageListVM.Prop.PackageID.Map, m_lstFilter);

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(PackageListVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(PackageListVM.Prop.BudgetPlanVersion.MapAlias);
            m_lstSelect.Add(PackageListVM.Prop.BudgetPlanTemplateDesc.MapAlias);
            m_lstSelect.Add(PackageListVM.Prop.BudgetPlanTemplateID.MapAlias);
            m_lstSelect.Add(PackageListVM.Prop.ProjectID.MapAlias);
            m_lstSelect.Add(PackageListVM.Prop.ProjectDesc.MapAlias);
            m_lstSelect.Add(PackageListVM.Prop.ClusterID.MapAlias);
            m_lstSelect.Add(PackageListVM.Prop.ClusterDesc.MapAlias);
            m_lstSelect.Add(PackageListVM.Prop.StatusID.MapAlias);

            List<PackageListVM> m_lstPackageListVM = new List<PackageListVM>();

            Dictionary<int, DataSet> m_dicDPackageListDA = m_objDPackageListDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);
            if (m_objDPackageListDA.Success && m_objDPackageListDA.Message.Length == 0)
            {
                m_lstPackageListVM = (
                    from DataRow m_drDPackageListDA in m_dicDPackageListDA[0].Tables[0].Rows
                    select new PackageListVM()
                    {
                        BudgetPlanID = m_drDPackageListDA[PackageListVM.Prop.BudgetPlanID.Name].ToString(),
                        BudgetPlanTemplateDesc = m_drDPackageListDA[PackageListVM.Prop.BudgetPlanTemplateDesc.Name].ToString(),
                        ProjectDesc = m_drDPackageListDA[PackageListVM.Prop.ProjectDesc.Name].ToString(),
                        ClusterDesc = m_drDPackageListDA[PackageListVM.Prop.ClusterDesc.Name].ToString(),
                        StatusID = int.Parse(m_drDPackageListDA[PackageListVM.Prop.StatusID.Name].ToString())
                    }
                ).ToList();
            }

            return m_lstPackageListVM;
        }

        /// <summary>
        /// Method to get detail Budget Plan Package
        /// </summary>
        /// <param name="packageID">Package ID to filter</param>
        /// <returns>PackageVM</returns>
        protected PackageVM GetPackageDetail(string packageID)
        {
            PackageVM m_objPackageVM = new PackageVM();

            TPackageDA m_objTPackageDA = new TPackageDA();
            m_objTPackageDA.ConnectionStringName = Global.ConnStrConfigName;

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(packageID);
            m_objFilter.Add(PackageVM.Prop.PackageID.Map, m_lstFilter);

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(PackageVM.Prop.PackageID.MapAlias);
            m_lstSelect.Add(PackageVM.Prop.PackageDesc.MapAlias);
            m_lstSelect.Add(PackageVM.Prop.StatusID.MapAlias);
            m_lstSelect.Add(PackageVM.Prop.StatusDesc.MapAlias);

            Dictionary<int, DataSet> m_dicTPackageDA = m_objTPackageDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objTPackageDA.Success && m_objTPackageDA.Message == String.Empty)
            {
                DataRow m_dr = m_dicTPackageDA[0].Tables[0].Rows[0];
                m_objPackageVM.PackageID = packageID;
                m_objPackageVM.PackageDesc = m_dr[PackageVM.Prop.PackageDesc.Name].ToString();
                m_objPackageVM.StatusID = int.Parse(m_dr[PackageVM.Prop.StatusID.Name].ToString());
                m_objPackageVM.StatusDesc = m_dr[PackageVM.Prop.StatusDesc.Name].ToString();
            }

            return m_objPackageVM;
        }

        /// <summary>
        /// Method to get List of Budget Plan Version from Budget Plan Package
        /// </summary>
        /// <param name="packageID">Package ID to filter</param>
        /// <returns>List Budget Plan Version</returns>
        protected List<PackageListVM> GetPackageListDetail(string packageID)
        {
            DPackageListDA m_objDPackageListDA = new DPackageListDA();
            m_objDPackageListDA.ConnectionStringName = Global.ConnStrConfigName;

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(packageID);
            m_objFilter.Add(PackageListVM.Prop.PackageID.Map, m_lstFilter);

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(PackageListVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(PackageListVM.Prop.Description.MapAlias);
            m_lstSelect.Add(PackageListVM.Prop.BudgetPlanTypeDesc.MapAlias);
            m_lstSelect.Add(PackageListVM.Prop.BudgetPlanVersion.MapAlias);
            m_lstSelect.Add(PackageListVM.Prop.ProjectDesc.MapAlias);
            m_lstSelect.Add(PackageListVM.Prop.ClusterDesc.MapAlias);
            m_lstSelect.Add(PackageListVM.Prop.RegionDesc.MapAlias);
            m_lstSelect.Add(PackageListVM.Prop.DivisionDesc.MapAlias);
            m_lstSelect.Add(PackageListVM.Prop.CompanyDesc.MapAlias);
            m_lstSelect.Add(PackageListVM.Prop.UnitTypeDesc.MapAlias);
            m_lstSelect.Add(PackageListVM.Prop.Area.MapAlias);
            m_lstSelect.Add(PackageListVM.Prop.Unit.MapAlias);
            m_lstSelect.Add(PackageListVM.Prop.LocationDesc.MapAlias);
            m_lstSelect.Add(PackageListVM.Prop.StatusID.MapAlias);
            m_lstSelect.Add(PackageListVM.Prop.FeePercentage.MapAlias);

            List<PackageListVM> m_lstPackageListVM = new List<PackageListVM>();

            Dictionary<int, DataSet> m_dicDPackageListDA = m_objDPackageListDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);
            if (m_objDPackageListDA.Success && m_objDPackageListDA.Message.Length == 0)
            {
                m_lstPackageListVM = (
                    from DataRow m_drDPackageListDA in m_dicDPackageListDA[0].Tables[0].Rows
                    select new PackageListVM()
                    {
                        BudgetPlanID = m_drDPackageListDA[PackageListVM.Prop.BudgetPlanID.Name].ToString(),
                        Description = m_drDPackageListDA[PackageListVM.Prop.Description.Name].ToString(),
                        BudgetPlanTypeDesc = m_drDPackageListDA[PackageListVM.Prop.BudgetPlanTypeDesc.Name].ToString(),
                        BudgetPlanVersion = int.Parse(m_drDPackageListDA[PackageListVM.Prop.BudgetPlanVersion.Name].ToString()),
                        ProjectDesc = m_drDPackageListDA[PackageListVM.Prop.ProjectDesc.Name].ToString(),
                        ClusterDesc = m_drDPackageListDA[PackageListVM.Prop.ClusterDesc.Name].ToString(),
                        RegionDesc = m_drDPackageListDA[PackageListVM.Prop.RegionDesc.Name].ToString(),
                        DivisionDesc = m_drDPackageListDA[PackageListVM.Prop.DivisionDesc.Name].ToString(),
                        CompanyDesc = m_drDPackageListDA[PackageListVM.Prop.CompanyDesc.Name].ToString(),
                        UnitTypeDesc = m_drDPackageListDA[PackageListVM.Prop.UnitTypeDesc.Name].ToString(),
                        Area = decimal.Parse(m_drDPackageListDA[PackageListVM.Prop.Area.Name].ToString()),
                        LocationDesc = m_drDPackageListDA[PackageListVM.Prop.LocationDesc.Name].ToString(),
                        StatusID = int.Parse(m_drDPackageListDA[PackageListVM.Prop.StatusID.Name].ToString()),
                        Unit = decimal.Parse(m_drDPackageListDA[PackageListVM.Prop.Unit.Name].ToString()),
                        FeePercentage = decimal.Parse(m_drDPackageListDA[PackageListVM.Prop.FeePercentage.Name].ToString())
                    }
                ).ToList();
            }

            return m_lstPackageListVM;
        }


        /// <summary>
        /// Method to get detail Cart
        /// </summary>
        /// <param name="cartID">Cart ID to filter</param>
        /// <returns>PackageVM</returns>
        protected CatalogCartVM GetCartDetail(string cartID)
        {
            CatalogCartVM m_objPackageVM = new CatalogCartVM();
            TCatalogCartDA m_objTPackageDA = new TCatalogCartDA();
            m_objTPackageDA.ConnectionStringName = Global.ConnStrConfigName;


            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(cartID);
            m_objFilter.Add(CatalogCartVM.Prop.CatalogCartID.Map, m_lstFilter);


            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(CatalogCartVM.Prop.CatalogCartID.MapAlias);
            m_lstSelect.Add(CatalogCartVM.Prop.CatalogCartDesc.MapAlias);
            m_lstSelect.Add(CatalogCartVM.Prop.StatusID.MapAlias);
            m_lstSelect.Add(CatalogCartVM.Prop.StatusDesc.MapAlias);

            Dictionary<int, DataSet> m_dicTPackageDA = m_objTPackageDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objTPackageDA.Success && m_objTPackageDA.Message == String.Empty)
            {
                DataRow m_dr = m_dicTPackageDA[0].Tables[0].Rows[0];
                m_objPackageVM.CatalogCartID = cartID;
                m_objPackageVM.CatalogCartDesc = m_dr[CatalogCartVM.Prop.CatalogCartDesc.Name].ToString();
                m_objPackageVM.StatusID = int.Parse(m_dr[CatalogCartVM.Prop.StatusID.Name].ToString());
                m_objPackageVM.StatusDesc = m_dr[CatalogCartVM.Prop.StatusDesc.Name].ToString();
            }

            return m_objPackageVM;
        }


        /// <summary>
        /// Methode get cart list detail
        /// </summary>
        /// <param name="cartID">Cart ID to filter</param>
        /// <returns>List Catalog Cart Item</returns>
        protected List<CartItemVM> GetCartItemList(string cartID)
        {

            DCatalogCartItemsDA m_objDCatalogCartItemsDA = new DCatalogCartItemsDA();
            m_objDCatalogCartItemsDA.ConnectionStringName = Global.ConnStrConfigName;
            List<CartItemVM> m_lstCartListVM = new List<CartItemVM>();

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(cartID);
            m_objFilter.Add(CartItemVM.Prop.CatalogCartID.Map, m_lstFilter);

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(CartItemVM.Prop.CatalogCartItemID.MapAlias);
            m_lstSelect.Add(CartItemVM.Prop.CatalogCartID.MapAlias);
            m_lstSelect.Add(CartItemVM.Prop.Qty.MapAlias);
            m_lstSelect.Add(CartItemVM.Prop.Amount.MapAlias);
            m_lstSelect.Add(CartItemVM.Prop.ItemPriceID.MapAlias);
            m_lstSelect.Add(CartItemVM.Prop.UoMDesc.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.UoMID.MapAlias);
            m_lstSelect.Add(CartItemVM.Prop.ValidFrom.MapAlias);
            //MItem
            m_lstSelect.Add(ItemVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.ItemDesc.MapAlias);

            //MVendor
            m_lstSelect.Add(VendorVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(VendorVM.Prop.VendorDesc.MapAlias);

            Dictionary<int, DataSet> m_dicDCatalogCartItemsDA = m_objDCatalogCartItemsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objDCatalogCartItemsDA.Success && m_objDCatalogCartItemsDA.Message.Length == 0)
            {
                m_lstCartListVM = (
                    from DataRow m_drDCatalogCartItemsDA in m_dicDCatalogCartItemsDA[0].Tables[0].Rows
                    select new CartItemVM()
                    {
                        CatalogCartItemID = m_drDCatalogCartItemsDA[CartItemVM.Prop.CatalogCartItemID.Name].ToString(),
                        CatalogCartID = m_drDCatalogCartItemsDA[CartItemVM.Prop.CatalogCartID.Name].ToString(),
                        Qty = decimal.Parse(m_drDCatalogCartItemsDA[CartItemVM.Prop.Qty.Name].ToString()),
                        Amount = decimal.Parse(m_drDCatalogCartItemsDA[CartItemVM.Prop.Amount.Name].ToString()),
                        ItemPriceID = m_drDCatalogCartItemsDA[CartItemVM.Prop.ItemPriceID.Name].ToString(),

                        //MItem
                        ItemID = m_drDCatalogCartItemsDA[ItemVM.Prop.ItemID.Name].ToString(),
                        ItemDesc = m_drDCatalogCartItemsDA[ItemVM.Prop.ItemDesc.Name].ToString(),
                        UoMID = m_drDCatalogCartItemsDA[ItemVM.Prop.UoMID.Name].ToString(),
                        UoMDesc = m_drDCatalogCartItemsDA[ItemVM.Prop.UoMDesc.Name].ToString(),
                        //MVendor
                        VendorID = m_drDCatalogCartItemsDA[VendorVM.Prop.VendorID.Name].ToString(),
                        VendorDesc = m_drDCatalogCartItemsDA[VendorVM.Prop.VendorDesc.Name].ToString(),
                        ValidFrom = DateTime.Parse(m_drDCatalogCartItemsDA[CartItemVM.Prop.ValidFrom.Name].ToString())
                    }).ToList();
            }

            return m_lstCartListVM;
        }

        protected bool CheckPriceValidation(string budgetPlanID, int budgetPlanVersion, string tableValidation, ref string message)
        {
            DBudgetPlanVersionStructureDA m_objDBudgetPlanVersionStructureDA = new DBudgetPlanVersionStructureDA();
            m_objDBudgetPlanVersionStructureDA.ConnectionStringName = Global.ConnStrConfigName;

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(budgetPlanID);
            m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(budgetPlanVersion);
            m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.None);
            m_lstFilter.Add("");
            m_objFilter.Add($"(CASE WHEN ItemTypeID IN ('BOI') THEN (MaterialAmount+WageAmount+MiscAmount)*Volume ELSE  MaterialAmount+WageAmount+MiscAmount  END)=0", m_lstFilter);


            List<string> m_lstSelect = new List<string>();
            //m_lstSelect.Add(tableValidation);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersionStructureID.MapAlias);
            //m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.MapAlias);
            //m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ItemGroupID.MapAlias);

            Dictionary<int, DataSet> m_dicDBudgetPlanVersionStructureDA = m_objDBudgetPlanVersionStructureDA.SelectBCValidating(0, null, true, m_lstSelect, m_objFilter, null, null, null, null);

            List<BudgetPlanVersionStructureVM> m_lstBudgetPlanVersionStructureVM = new List<BudgetPlanVersionStructureVM>();

            if (m_objDBudgetPlanVersionStructureDA.Success)
            {
                if (m_objDBudgetPlanVersionStructureDA.AffectedRows <= 0)
                    return true;
                else
                {
                    //List<string> m_lsItemGRoupID = (from DataRow m_drDBudgetPlanVersionStrucDA in m_dicDBudgetPlanVersionStructureDA[0].Tables[0].Rows
                    //                                                     select m_drDBudgetPlanVersionStrucDA[ItemGroupVM.Prop.ItemGroupID.Name].ToString()).ToList();

                    //List<ConfigVM> m_uConfigVM = GetConfig("BudgetPlan", ItemGroupVM.Prop.ItemGroupID.Name, "Validate");
                    //string strItemGroupID = m_uConfigVM != null ? string.Join(",", m_uConfigVM.Select(d => d.Key4).ToList()) : string.Empty;

                    //if (m_uConfigVM != null)
                    //    if (m_uConfigVM.Any(d => m_lsItemGRoupID.Contains(d.Key4))) {
                    //        return true;
                    //    }

                    message = "Some item price hasn't entry yet - " + budgetPlanID;
                    return false;
                }
            }

            message = m_objDBudgetPlanVersionStructureDA.Message;
            return false;

        }


        protected UserVM getCurentUser()
        {
            string m_strMessage = string.Empty;

            UserVM m_objUserVM = new UserVM();

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                string m_strUserID = Global.LoggedInUser;

                MUserDA m_objMUserDA = new MUserDA();
                m_objMUserDA.ConnectionStringName = Global.ConnStrConfigName;

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(m_strUserID);
                m_objFilter.Add(UserVM.Prop.UserID.Map, m_lstFilter);

                m_lstSelect = new List<string>();
                m_lstSelect.Add(UserVM.Prop.UserID.MapAlias);
                m_lstSelect.Add(UserVM.Prop.FullName.MapAlias);
                m_lstSelect.Add(UserVM.Prop.VendorID.MapAlias);
                m_lstSelect.Add(UserVM.Prop.VendorDesc.MapAlias);
                m_lstSelect.Add(UserVM.Prop.EmployeeID.MapAlias);
                m_lstSelect.Add(UserVM.Prop.BusinessUnitID.MapAlias);
                m_lstSelect.Add(UserVM.Prop.DivisionID.MapAlias);
                m_lstSelect.Add(UserVM.Prop.ProjectID.MapAlias);
                m_lstSelect.Add(UserVM.Prop.ClusterID.MapAlias);

                Dictionary<int, DataSet> m_dicMUserDA = m_objMUserDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null, null);

                if (m_objMUserDA.Success)
                {
                    m_objUserVM.UserID = m_dicMUserDA[0].Tables[0].Rows[0][UserVM.Prop.UserID.Name].ToString();
                    m_objUserVM.FullName = m_dicMUserDA[0].Tables[0].Rows[0][UserVM.Prop.FullName.Name].ToString();
                    m_objUserVM.VendorID = m_dicMUserDA[0].Tables[0].Rows[0][UserVM.Prop.VendorID.Name].ToString();
                    m_objUserVM.VendorDesc = m_dicMUserDA[0].Tables[0].Rows[0][UserVM.Prop.VendorDesc.Name].ToString();
                    m_objUserVM.EmployeeID = m_dicMUserDA[0].Tables[0].Rows[0][UserVM.Prop.EmployeeID.Name].ToString();
                    m_objUserVM.BusinessUnitID = m_dicMUserDA[0].Tables[0].Rows[0][UserVM.Prop.BusinessUnitID.Name].ToString();
                    m_objUserVM.DivisionID = m_dicMUserDA[0].Tables[0].Rows[0][UserVM.Prop.DivisionID.Name].ToString();
                    m_objUserVM.ProjectID = m_dicMUserDA[0].Tables[0].Rows[0][UserVM.Prop.ProjectID.Name].ToString();
                    m_objUserVM.ClusterID = m_dicMUserDA[0].Tables[0].Rows[0][UserVM.Prop.ClusterID.Name].ToString();

                    m_objUserVM.TCMember = GetTCMember("", m_objUserVM.EmployeeID, ref m_strMessage);
                }
            }
            return m_objUserVM;
        }

        protected void GetUserFilters(ref Dictionary<string, List<object>> objFilter)
        {
            //Dictionary<string, List<object>> m_dicFilters = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();

            //Get User
            UserVM m_objUserVM = getCurentUser();


            if (!string.IsNullOrEmpty(m_objUserVM.ClusterID))
            {
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(m_objUserVM.ClusterID);
                objFilter.Add(ClusterVM.Prop.ClusterID.Map, m_lstFilter);
            }
            else
            if (!string.IsNullOrEmpty(m_objUserVM.ProjectID))
            {
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(m_objUserVM.ProjectID);
                objFilter.Add(ProjectVM.Prop.ProjectID.Map, m_lstFilter);
            }
            else
            if (!string.IsNullOrEmpty(m_objUserVM.DivisionID))
            {
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(m_objUserVM.DivisionID);
                objFilter.Add(DivisionVM.Prop.DivisionID.Map, m_lstFilter);
            }
            else
            if (!string.IsNullOrEmpty(m_objUserVM.BusinessUnitID))
            {
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(m_objUserVM.BusinessUnitID);
                objFilter.Add(BusinessUnitVM.Prop.BusinessUnitID.Map, m_lstFilter);
            }
        }
        protected TCMembersVM GetTCMember(string TCMemberID, string EmployeeID, ref string message)
        {
            TCMembersVM m_objTCMembersVM = new TCMembersVM();
            TTCMembersDA m_objTTCMembersDA = new TTCMembersDA();
            m_objTTCMembersDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(TCMembersVM.Prop.TCMemberID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.EmployeeID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.EmployeeName.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.SuperiorID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.SuperiorName.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.PeriodStart.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.PeriodEnd.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.DelegateStartDate.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.DelegateEndDate.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.DelegateTo.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.TCTypeID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.BusinessUnitID.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();


            List<object> m_lstFilter = new List<object>();
            if (TCMemberID != string.Empty)
            {
                m_lstFilter.Add(Operator.In);
                m_lstFilter.Add(TCMemberID);
                m_objFilter.Add(TCMembersVM.Prop.TCMemberID.Map, m_lstFilter);
            }

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(EmployeeID);
            m_objFilter.Add(TCMembersVM.Prop.EmployeeID.Map, m_lstFilter);


            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.LessThanEqual);
            m_lstFilter.Add(DateTime.Now.Date);
            m_objFilter.Add(TCMembersVM.Prop.PeriodStart.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.GreaterThanEqual);
            m_lstFilter.Add(DateTime.Now.Date);
            m_objFilter.Add(TCMembersVM.Prop.PeriodEnd.Map, m_lstFilter);


            Dictionary<int, DataSet> m_dicTTCMembersDA = m_objTTCMembersDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objTTCMembersDA.Message == string.Empty)
            {
                DataRow m_drTTCMembersDA = m_dicTTCMembersDA[0].Tables[0].Rows[0];
                m_objTCMembersVM.TCMemberID = m_drTTCMembersDA[TCMembersVM.Prop.TCMemberID.Name].ToString();
                m_objTCMembersVM.EmployeeID = m_drTTCMembersDA[TCMembersVM.Prop.EmployeeID.Name].ToString();
                m_objTCMembersVM.EmployeeName = m_drTTCMembersDA[TCMembersVM.Prop.EmployeeName.Name].ToString();
                m_objTCMembersVM.SuperiorID = m_drTTCMembersDA[TCMembersVM.Prop.SuperiorID.Name].ToString();
                m_objTCMembersVM.SuperiorName = m_drTTCMembersDA[TCMembersVM.Prop.SuperiorName.Name].ToString();
                m_objTCMembersVM.PeriodStart = DateTime.Parse(m_drTTCMembersDA[TCMembersVM.Prop.PeriodStart.Name].ToString());
                m_objTCMembersVM.PeriodEnd = DateTime.Parse(m_drTTCMembersDA[TCMembersVM.Prop.PeriodEnd.Name].ToString());
                m_objTCMembersVM.DelegateStartDate = !string.IsNullOrEmpty(m_drTTCMembersDA[TCMembersVM.Prop.DelegateStartDate.Name].ToString()) ? DateTime.Parse(m_drTTCMembersDA[TCMembersVM.Prop.DelegateStartDate.Name].ToString()) : (DateTime?)null;
                m_objTCMembersVM.DelegateEndDate = !string.IsNullOrEmpty(m_drTTCMembersDA[TCMembersVM.Prop.DelegateStartDate.Name].ToString()) ? DateTime.Parse(m_drTTCMembersDA[TCMembersVM.Prop.DelegateEndDate.Name].ToString()) : (DateTime?)null;
                m_objTCMembersVM.DelegateTo = m_drTTCMembersDA[TCMembersVM.Prop.DelegateTo.Name].ToString();
                m_objTCMembersVM.ListTCAppliedTypesVM = GetListTCAppliedTypes(m_objTCMembersVM.TCMemberID, ref message);
                m_objTCMembersVM.TCTypeID = m_drTTCMembersDA[TCMembersVM.Prop.TCTypeID.Name].ToString();
                m_objTCMembersVM.BusinessUnitID = m_drTTCMembersDA[TCMembersVM.Prop.BusinessUnitID.Name].ToString();
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objTTCMembersDA.Message;

            return m_objTCMembersVM;
        }
        protected bool ValidateTCAccess(string FPTID, ref string m_strMessage)
        {

            UserVM userVM = getCurentUser();

            DateTime m_dtNow = DateTime.Now.Date;

            List<FPTTCParticipantsVM> m_lsNegoTCParticipant = GetListNegoTCParticipant(FPTID, ref m_strMessage);

            List<string> m_lTCMemberIDs = m_lsNegoTCParticipant.Where(d => d.StatusID).Select(d => d.TCMemberID).ToList();

            //Get TC Member from configuration
            List<TCMembersVM> m_lstTCMembersVM = GetListTCMemberByFPT(FPTID, ref m_strMessage);
            bool m_boolHasAccess = false;

            if (m_lstTCMembersVM.Any(d => m_lTCMemberIDs.Contains(d.TCMemberID) && d.TCMemberID == (string.IsNullOrEmpty(userVM.TCMember.TCMemberID) ? "" : userVM.TCMember.TCMemberID) &&
                   (d.PeriodStart.Date <= m_dtNow && d.PeriodEnd.Date >= m_dtNow))
                )
            {
                m_boolHasAccess = true;
            }


            foreach (TCMembersVM objTC in m_lstTCMembersVM)
            {
                if (objTC.ListTCMembersDelegationVM != null)
                {
                    if (objTC.ListTCMembersDelegationVM.Any(d => m_lTCMemberIDs.Contains(d.DelegateTo) && d.DelegateTo == (string.IsNullOrEmpty(userVM.TCMember.TCMemberID) ? "" : userVM.TCMember.TCMemberID) && (d.DelegateStartDate <= m_dtNow && d.DelegateEndDate >= m_dtNow)))
                    {
                        m_boolHasAccess = true;
                        break;
                    }
                }
            }

            return m_boolHasAccess;
        }
        protected List<string> GetAllRoleID(ref string message)
        {
            List<string> AllRoleID = new List<string>();
            string UserID = System.Web.HttpContext.Current.User.Identity.Name;
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();
            //List<string> AllUserID = new List<string>();
            DUserRoleDA m_objDUserRoleDA = new DUserRoleDA();
            m_objDUserRoleDA.ConnectionStringName = Global.ConnStrConfigName;

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(UserID);
            m_objFilter.Add(UserRoleVM.Prop.UserID.Map, m_lstFilter);

            m_lstSelect = new List<string>();
            m_lstSelect.Add(UserRoleVM.Prop.RoleID.MapAlias);

            Dictionary<int, DataSet> m_dicMUserDA = m_objDUserRoleDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);
            if (m_objDUserRoleDA.Success)
                foreach (DataRow dr in m_dicMUserDA[0].Tables[0].Rows)
                {
                    AllRoleID.Add(dr[UserRoleVM.Prop.RoleID.Name].ToString());
                }
            else
            {
                message = "Get Role Error: " + m_objDUserRoleDA.Message;
                return AllRoleID;
            }

            return AllRoleID;
        }
        protected bool CheckMatchedRole(string RoleID, ref string message)
        {
            bool availableRole = false;
            string UserID = System.Web.HttpContext.Current.User.Identity.Name;
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();
            //List<string> AllUserID = new List<string>();
            DUserRoleDA m_objDUserRoleDA = new DUserRoleDA();
            m_objDUserRoleDA.ConnectionStringName = Global.ConnStrConfigName;

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(UserID);
            m_objFilter.Add(UserRoleVM.Prop.UserID.Map, m_lstFilter);

            m_lstSelect = new List<string>();
            m_lstSelect.Add(UserRoleVM.Prop.RoleID.MapAlias);

            Dictionary<int, DataSet> m_dicMUserDA = m_objDUserRoleDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);
            if (m_objDUserRoleDA.Success)
                foreach (DataRow dr in m_dicMUserDA[0].Tables[0].Rows)
                {
                    availableRole = dr[UserRoleVM.Prop.RoleID.Name].ToString() == RoleID;
                    if (availableRole)
                        break;
                }
            else
            {
                message = "Error while checking RoleID";
                return false;

            }

            if (m_objDUserRoleDA.Success && availableRole == false)
                message = "Not Authorized to create configuration.";
            return availableRole;
        }
        protected List<TCAppliedTypesVM> GetListTCAppliedTypes(string TCMemberID, ref string message)
        {

            List<TCAppliedTypesVM> m_lstTCAppliedTypesVM = new List<TCAppliedTypesVM>();
            TTCAppliedTypesDA m_objTTCAppliedTypesDA = new TTCAppliedTypesDA();
            m_objTTCAppliedTypesDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(TCAppliedTypesVM.Prop.TCMemberID.MapAlias);
            m_lstSelect.Add(TCAppliedTypesVM.Prop.TCTypeID.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(TCMemberID);
            m_objFilter.Add(TCAppliedTypesVM.Prop.TCMemberID.Map, m_lstFilter);


            Dictionary<string, OrderDirection> m_objOrderBy = new Dictionary<string, OrderDirection>();
            m_objOrderBy.Add(TCAppliedTypesVM.Prop.TCMemberID.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicTTCAppliedTypesDA = m_objTTCAppliedTypesDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objTTCAppliedTypesDA.Message == string.Empty)
            {
                m_lstTCAppliedTypesVM = (
                from DataRow m_drTTCAppliedTypesDA in m_dicTTCAppliedTypesDA[0].Tables[0].Rows
                select new TCAppliedTypesVM()
                {
                    TCMemberID = m_drTTCAppliedTypesDA[TCAppliedTypesVM.Prop.TCMemberID.Name].ToString(),
                    TCTypeID = m_drTTCAppliedTypesDA[TCAppliedTypesVM.Prop.TCTypeID.Name].ToString()
                }).Distinct().ToList();
            }

            return m_lstTCAppliedTypesVM;

        }
        protected bool InsertDFPTStatus(string FPTID, int FPTStatusTypes, DateTime dateTime, ref string message, string Remark = null, object m_objDBConnection = null)
        {
            bool m_boolusetrans = m_objDBConnection != null;
            DFPTStatusDA m_objDFPTStatusDA = new DFPTStatusDA();
            m_objDFPTStatusDA.ConnectionStringName = Global.ConnStrConfigName;
            DFPTStatus m_objDFPTStatus = new DFPTStatus();
            m_objDFPTStatus.FPTStatusID = string.Empty;
            m_objDFPTStatus.FPTID = FPTID;
            m_objDFPTStatus.StatusDateTimeStamp = dateTime;
            m_objDFPTStatus.StatusID = FPTStatusTypes;
            m_objDFPTStatus.Remarks = Remark;
            m_objDFPTStatusDA.Data = m_objDFPTStatus;


            m_objDFPTStatusDA.Insert(m_boolusetrans, m_objDBConnection);

            if (!m_objDFPTStatusDA.Success || m_objDFPTStatusDA.Message != string.Empty)
            {
                message = m_objDFPTStatusDA.Message;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Method to get Structure Budget Plan Version, Only Node ItemTypeID equals BOI
        /// </summary>
        /// <param name="budgetPlanID">Budget Plan ID to filter</param>
        /// /// <param name="budgetPlanVersion">Budget Plan Version to filter</param>
        /// <returns>DataTable Budget Plan Version Structure Columns[WorkItem,Amount]</returns>
        protected List<BudgetPlanVersionStructureWSVM> GetBudgetPlanVersionStructureDetail(string budgetPlanID, int budgetPlanVersion)
        {
            List<BudgetPlanVersionStructureWSVM> m_lstData = new List<BudgetPlanVersionStructureWSVM>();

            DataTable m_dtReturn = new DataTable("BudgetPlanStructure_" + budgetPlanID + "_" + budgetPlanVersion.ToString()
                , "BudgetPlanStructure_" + budgetPlanID + "_" + budgetPlanVersion.ToString());
            m_dtReturn.Columns.Add("WorkItem", typeof(string));
            m_dtReturn.Columns.Add("Amount", typeof(string));

            DataRow m_drReturn;

            PackageVM m_objPackageVM = new PackageVM();

            DBudgetPlanVersionStructureDA m_objDBudgetPlanVersionStructureDA = new DBudgetPlanVersionStructureDA();
            m_objDBudgetPlanVersionStructureDA.ConnectionStringName = Global.ConnStrConfigName;

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(budgetPlanID);
            m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(budgetPlanVersion);
            m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("BOI");
            m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.ItemTypeID.Map, m_lstFilter);

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersionStructureID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanTemplateDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Volume.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.MaterialAmount.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.MiscAmount.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.WageAmount.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Version.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Sequence.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ParentItemID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ParentVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ParentSequence.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ItemDesc.MapAlias);

            Dictionary<int, DataSet> m_dicDBudgetPlanVersionStructureDA = m_objDBudgetPlanVersionStructureDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

            List<BudgetPlanVersionStructureVM> m_lstBudgetPlanVersionStructureVM = new List<BudgetPlanVersionStructureVM>();

            if (m_objDBudgetPlanVersionStructureDA.Success && m_objDBudgetPlanVersionStructureDA.Message == String.Empty)
            {
                m_lstBudgetPlanVersionStructureVM = (
                    from DataRow m_drDBudgetPlanVersionStructureDA in m_dicDBudgetPlanVersionStructureDA[0].Tables[0].Rows
                    select new BudgetPlanVersionStructureVM()
                    {
                        BudgetPlanTemplateDesc = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.BudgetPlanTemplateDesc.Name].ToString(),
                        Volume = decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Volume.Name].ToString()),
                        MaterialAmount = decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name].ToString()),
                        MiscAmount = decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.MiscAmount.Name].ToString()),
                        WageAmount = decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.WageAmount.Name].ToString()),
                        ParentItemID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ParentItemID.Name].ToString(),
                        ParentVersion = int.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ParentVersion.Name].ToString()),
                        ParentSequence = int.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ParentSequence.Name].ToString()),
                        ItemID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ItemID.Name].ToString(),
                        ItemDesc = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ItemDesc.Name].ToString(),
                        Version = int.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Version.Name].ToString()),
                        Sequence = int.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Sequence.Name].ToString())
                    }
                ).ToList();
            }

            List<BudgetPlanVersionStructureVM> m_lstBudgetPlanVersionStructureVMLevel1 = m_lstBudgetPlanVersionStructureVM.Where(m => m.ParentItemID == "0" && m.ParentVersion == 0 && m.ParentSequence == 0).ToList<BudgetPlanVersionStructureVM>();
            m_lstBudgetPlanVersionStructureVMLevel1 = m_lstBudgetPlanVersionStructureVMLevel1.OrderBy(m => m.Sequence).ToList();
            foreach (BudgetPlanVersionStructureVM item in m_lstBudgetPlanVersionStructureVMLevel1)
            {
                #region Amount Count
                decimal m_decVolume = item.Volume.Value;
                decimal m_decAmount = 0;
                if (m_decVolume > 0)
                {
                    m_decAmount = ((decimal)item.WageAmount + (decimal)item.MiscAmount + (decimal)item.MaterialAmount) * (decimal)item.Volume;
                    m_drReturn = m_dtReturn.NewRow();
                    m_drReturn["WorkItem"] = item.ItemDesc;
                    m_drReturn["Amount"] = m_decAmount;
                    m_dtReturn.Rows.Add(m_drReturn);

                    m_lstData.Add(new BudgetPlanVersionStructureWSVM() { WorkItem = item.ItemDesc, Amount = m_decAmount });
                }
                else
                {
                    CountAmount(m_lstBudgetPlanVersionStructureVM, item.ItemID, item.Version, item.Sequence, ref m_decAmount);
                    m_lstData.Add(new BudgetPlanVersionStructureWSVM() { WorkItem = item.ItemDesc, Amount = m_decAmount });
                    m_drReturn = m_dtReturn.NewRow();
                    m_drReturn["WorkItem"] = item.ItemDesc;
                    m_drReturn["Amount"] = m_decAmount;
                    m_dtReturn.Rows.Add(m_drReturn);
                }
                #endregion
            }

            return m_lstData;
        }

        private void CountAmount(List<BudgetPlanVersionStructureVM> lstAllBOI, string ItemID, int Version, int Sequence, ref decimal Amount)
        {
            List<BudgetPlanVersionStructureVM> m_lstChild = lstAllBOI.Where(m => m.ParentItemID == ItemID && m.ParentVersion == Version && m.ParentSequence == Sequence).ToList();

            foreach (BudgetPlanVersionStructureVM item in m_lstChild)
            {
                if (item.Volume == 0)
                {
                    CountAmount(lstAllBOI, item.ItemID, item.Version, item.Sequence, ref Amount);
                }
                else
                {
                    Amount += ((decimal)item.MiscAmount + (decimal)item.MaterialAmount + (decimal)item.WageAmount) * (decimal)item.Volume;
                }
            }
        }

        protected decimal CalculateFormula(string formula, ref string message, Dictionary<string, decimal> variables = null, bool caseSensitive = false)
        {
            decimal m_decResult = 0;
            try
            {
                ExpressionContext m_excCalculate = new ExpressionContext();
                m_excCalculate.Imports.AddType(typeof(Math));
                m_excCalculate.Options.RealLiteralDataType = RealLiteralDataType.Decimal;
                m_excCalculate.Options.ResultType = typeof(decimal);
                m_excCalculate.Options.CaseSensitive = caseSensitive;
                if (variables != null && variables.Count > 0)
                    foreach (KeyValuePair<string, decimal> m_keyVariable in variables)
                        m_excCalculate.Variables[m_keyVariable.Key] = m_keyVariable.Value;
                m_excCalculate.ParserOptions.RecreateParser();
                IDynamicExpression m_ideCalculate = m_excCalculate.CompileDynamic("1.00*" + formula);
                m_decResult = decimal.Parse(m_ideCalculate.Evaluate().ToString());
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return m_decResult;
        }

        /// <summary>
        /// Export Grid to Xml, Excel, or Comma separated
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="type">One of xml, xls, or csv</param>
        /// <param name="fileName">Filename without extension</param>
        public ActionResult ExportGrid(SubmitHandler handler, string type, string fileName)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Export)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));
            Control.ExportGrid(Response, handler.Xml, type, fileName, Server.MapPath(type == "xls" ? Global.ExcelTemplateLocation : Global.CsvTemplateLocation));
            return this.Direct();
        }
        public void ExportToExcel(DataTable sourceTable, string fileName)
        {


        }
        protected string GenerateContent(string MailNotificationID, string NotifTemplateID, List<RecipientsVM> lstRecipient, List<TNotificationValues> LstNotificationValue, ref string message, object objectConnectionIFTrans = null)
        {
            //List<NotificationValuesVM> LstNotificationValue = new List<NotificationValuesVM>();
            //if (LstNotificationValue.Count == 0)
            //{
            //    TNotificationValuesDA m_objTNotificationValuesDA = new TNotificationValuesDA();
            //    m_objTNotificationValuesDA.ConnectionStringName = Global.ConnStrConfigName;
            //    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            //    List<object> m_lstFilter = new List<object>();
            //    List<string> m_lstSelect = new List<string>();
            //    m_lstSelect = new List<string>();
            //    m_lstSelect.Add(NotificationValuesVM.Prop.NotificationValueID.MapAlias);
            //    m_lstSelect.Add(NotificationValuesVM.Prop.FieldTagID.MapAlias);
            //    m_lstSelect.Add(NotificationValuesVM.Prop.MailNotificationID.MapAlias);
            //    m_lstSelect.Add(NotificationValuesVM.Prop.Value.MapAlias);
            //    m_objFilter = new Dictionary<string, List<object>>();
            //    m_lstFilter = new List<object>();
            //    m_lstFilter.Add(Operator.Equals);
            //    m_lstFilter.Add(MailNotificationID);
            //    m_objFilter.Add(NotificationValuesVM.Prop.MailNotificationID.Map, m_lstFilter);
            //    Dictionary<int, DataSet> m_dicDAResult = new Dictionary<int, DataSet>();
            //    m_dicDAResult = m_objTNotificationValuesDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, objectConnectionIFTrans);
            //    if (m_objTNotificationValuesDA.Message == string.Empty && m_objTNotificationValuesDA.AffectedRows > 0)
            //    {
            //        foreach (DataRow dr in m_dicDAResult[0].Tables[0].Rows)
            //        {
            //            LstNotificationValue.Add(new NotificationValuesVM()
            //            {
            //                FieldTagID = dr[NotificationValuesVM.Prop.FieldTagID.Name].ToString(),
            //                Value = dr[NotificationValuesVM.Prop.Value.Name].ToString()
            //            });
            //        }
            //    }
            //    else
            //    {
            //        message = "Error Get TNotificationValues";
            //    }
            //}

            #region Get Param Values
            Dictionary<string, string> paramToSend = new Dictionary<string, string>();
            foreach (TNotificationValues dr in LstNotificationValue)
                paramToSend.Add(dr.FieldTagID, dr.Value);
            #endregion

            #region Get Template Body
            string ContentBody = "";
            MNotificationTemplatesDA m_objTemplateMailDA = new MNotificationTemplatesDA();
            m_objTemplateMailDA.ConnectionStringName = Global.ConnStrConfigName;
            if (ContentBody == "")
            {
                Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                List<object> m_lstFilter = new List<object>();
                List<string> m_lstSelect = new List<string>();
                m_lstSelect = new List<string>();
                m_lstSelect.Add(NotificationTemplateVM.Prop.NotificationTemplateID.MapAlias);
                m_lstSelect.Add(NotificationTemplateVM.Prop.Contents.MapAlias);
                m_objFilter = new Dictionary<string, List<object>>();
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(NotifTemplateID);
                m_objFilter.Add(NotificationTemplateVM.Prop.NotificationTemplateID.Map, m_lstFilter);
                Dictionary<int, DataSet> m_dicDAResult = new Dictionary<int, DataSet>();
                m_dicDAResult = m_objTemplateMailDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
                if (m_objTemplateMailDA.Message == string.Empty && m_objTemplateMailDA.AffectedRows > 0)
                {
                    //List<RecipientsVM> lstRecipient = GetListRecipientsVM(MailNotificationID, ref message);
                    //if (string.IsNullOrEmpty(message))
                    //{
                    ContentBody = m_dicDAResult[0].Tables[0].Rows[0][NotificationTemplateVM.Prop.Contents.Name].ToString();
                    ContentBody = Global.ParseParameter(ContentBody, paramToSend, lstRecipient, ref message);
                    //}
                }
                else
                {
                    message = "error getting content from MNotificationTemplatesDA " + m_objTemplateMailDA.Message;
                    return "Error";
                }
            }
            #endregion

            return ContentBody;
        }
        protected string GenerateMultipleContent(string MailNotificationID, string NotifTemplateID, List<RecipientsVM> lstRecipient, List<NotificationValuesVM> LstNotificationValue, ref string message, object objectConnectionIFTrans = null)
        {

            #region Get Param Values
            Dictionary<string, string> paramToSend = new Dictionary<string, string>();
            foreach (NotificationValuesVM dr in LstNotificationValue)
                if(!paramToSend.ContainsKey(dr.FieldTagID))
                paramToSend.Add(dr.FieldTagID, dr.Value);
            #endregion

            #region Get Template Body
            string ContentBody = "";
            MNotificationTemplatesDA m_objTemplateMailDA = new MNotificationTemplatesDA();
            m_objTemplateMailDA.ConnectionStringName = Global.ConnStrConfigName;
            if (ContentBody == "")
            {
                Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                List<object> m_lstFilter = new List<object>();
                List<string> m_lstSelect = new List<string>();
                m_lstSelect = new List<string>();
                m_lstSelect.Add(NotificationTemplateVM.Prop.NotificationTemplateID.MapAlias);
                m_lstSelect.Add(NotificationTemplateVM.Prop.Contents.MapAlias);
                m_objFilter = new Dictionary<string, List<object>>();
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(NotifTemplateID);
                m_objFilter.Add(NotificationTemplateVM.Prop.NotificationTemplateID.Map, m_lstFilter);
                Dictionary<int, DataSet> m_dicDAResult = new Dictionary<int, DataSet>();
                m_dicDAResult = m_objTemplateMailDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
                if (m_objTemplateMailDA.Message == string.Empty && m_objTemplateMailDA.AffectedRows > 0)
                {
                    //List<RecipientsVM> lstRecipient = GetListRecipientsVM(MailNotificationID, ref message);
                    //if (string.IsNullOrEmpty(message))
                    //{
                    ContentBody = m_dicDAResult[0].Tables[0].Rows[0][NotificationTemplateVM.Prop.Contents.Name].ToString();
                    ContentBody = Global.ParseParameter(ContentBody, paramToSend, lstRecipient, ref message);
                    //}
                }
                else
                {
                    message = "error getting content from MNotificationTemplatesDA " + m_objTemplateMailDA.Message;
                    return "Error";
                }
            }
            #endregion

            return ContentBody;
        }
        protected bool SendMail(MMailNotifications MailNotificationVM, ref string message, bool IsBatch=false)
        {

            string To = "";
            string Cc = "";
            string Bcc = "";
            string From = "noreply@bigtrons.sinarmasland.com";
            string Subject = MailNotificationVM.Subject;

            MailNotificationsVM objNotification = new MailNotificationsVM();
            objNotification.MailNotificationID = MailNotificationVM.MailNotificationID;

            #region Populate List
            List<NotificationAttachmentVM> LstNotificationAttachment = new List<NotificationAttachmentVM>();
            if (LstNotificationAttachment.Count == 0)
            {
                TNotificationAttachmentsDA m_objTNotificationAttachmentDA = new TNotificationAttachmentsDA();
                m_objTNotificationAttachmentDA.ConnectionStringName = Global.ConnStrConfigName;
                Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                List<object> m_lstFilter = new List<object>();
                List<string> m_lstSelect = new List<string>();
                m_lstSelect = new List<string>();
                m_lstSelect.Add(NotificationAttachmentVM.Prop.MailNotificationID.MapAlias);
                m_lstSelect.Add(NotificationAttachmentVM.Prop.AttachmentValueID.MapAlias);
                m_lstSelect.Add(NotificationAttachmentVM.Prop.ContentType.MapAlias);
                m_lstSelect.Add(NotificationAttachmentVM.Prop.RawData.MapAlias);
                m_lstSelect.Add(NotificationAttachmentVM.Prop.Filename.MapAlias);
                m_lstSelect.Add(NotificationAttachmentVM.Prop.Subject.MapAlias);
                m_objFilter = new Dictionary<string, List<object>>();
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(MailNotificationVM.MailNotificationID);
                m_objFilter.Add(NotificationAttachmentVM.Prop.MailNotificationID.Map, m_lstFilter);
                Dictionary<int, DataSet> m_dicDAResult = new Dictionary<int, DataSet>();
                m_dicDAResult = m_objTNotificationAttachmentDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
                if (m_objTNotificationAttachmentDA.Success)
                {
                    foreach (DataRow dr in m_dicDAResult[0].Tables[0].Rows)
                    {
                        Subject = dr[NotificationAttachmentVM.Prop.Subject.Name].ToString();
                        LstNotificationAttachment.Add(new NotificationAttachmentVM()
                        {
                            Filename = dr[NotificationAttachmentVM.Prop.Filename.Name].ToString(),
                            RawData = dr[NotificationAttachmentVM.Prop.RawData.Name].ToString()
                        }
                        );
                    }
                }
                else
                {
                    message = "Error Get TNotificationAttachmentsDA";
                    return false;
                }
            }

            objNotification.NotificationAttachmentVM = LstNotificationAttachment;
            objNotification.Contents = MailNotificationVM.Contents;
            objNotification.TaskID = MailNotificationVM.TaskID;
            objNotification.NotificationTemplateID = MailNotificationVM.NotificationTemplateID;

            List<TNotificationValues> LstNotificationValue = new List<TNotificationValues>();
            if (LstNotificationValue.Count == 0)
            {
                TNotificationValuesDA m_objTNotificationValuesDA = new TNotificationValuesDA();
                m_objTNotificationValuesDA.ConnectionStringName = Global.ConnStrConfigName;
                Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                List<object> m_lstFilter = new List<object>();
                List<string> m_lstSelect = new List<string>();
                m_lstSelect = new List<string>();
                m_lstSelect.Add(NotificationValuesVM.Prop.NotificationValueID.MapAlias);
                m_lstSelect.Add(NotificationValuesVM.Prop.FieldTagID.MapAlias);
                m_lstSelect.Add(NotificationValuesVM.Prop.MailNotificationID.MapAlias);
                m_lstSelect.Add(NotificationValuesVM.Prop.Value.MapAlias);
                m_objFilter = new Dictionary<string, List<object>>();
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(MailNotificationVM.MailNotificationID);
                m_objFilter.Add(NotificationValuesVM.Prop.MailNotificationID.Map, m_lstFilter);
                Dictionary<int, DataSet> m_dicDAResult = new Dictionary<int, DataSet>();
                m_dicDAResult = m_objTNotificationValuesDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
                if (m_objTNotificationValuesDA.Message == string.Empty && m_objTNotificationValuesDA.AffectedRows > 0)
                {
                    foreach (DataRow dr in m_dicDAResult[0].Tables[0].Rows)
                    {
                        LstNotificationValue.Add(new TNotificationValues()
                        {
                            FieldTagID = dr[NotificationValuesVM.Prop.FieldTagID.Name].ToString(),
                            Value = dr[NotificationValuesVM.Prop.Value.Name].ToString()
                        }
                        );
                    }
                }
                else
                {
                    message = "Error Get TNotificationValuesDA";
                    return false;
                }
            }


            List<RecipientsVM> LstRecipients = new List<RecipientsVM>();
            if (LstRecipients.Count == 0)
            {
                DRecipientsDA objRecipient = new DRecipientsDA();
                objRecipient.ConnectionStringName = Global.ConnStrConfigName;
                Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                List<object> m_lstFilter = new List<object>();
                List<string> m_lstSelect = new List<string>();
                m_lstSelect = new List<string>();
                m_lstSelect.Add(RecipientsVM.Prop.MailNotificationID.MapAlias);
                m_lstSelect.Add(RecipientsVM.Prop.RecipientID.MapAlias);
                m_lstSelect.Add(RecipientsVM.Prop.RecipientTypeID.MapAlias);
                m_lstSelect.Add(RecipientsVM.Prop.MailAddress.MapAlias);
                m_lstSelect.Add(RecipientsVM.Prop.RecipientDesc.MapAlias);
                m_objFilter = new Dictionary<string, List<object>>();
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(MailNotificationVM.MailNotificationID);
                m_objFilter.Add(RecipientsVM.Prop.MailNotificationID.Map, m_lstFilter);
                Dictionary<int, DataSet> m_dicDAResult = new Dictionary<int, DataSet>();
                m_dicDAResult = objRecipient.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
                if (objRecipient.Message == string.Empty && objRecipient.AffectedRows > 0)
                {
                    foreach (DataRow dr in m_dicDAResult[0].Tables[0].Rows)
                    {
                        LstRecipients.Add(new RecipientsVM()
                        {
                            MailAddress = dr[RecipientsVM.Prop.MailAddress.Name].ToString(),
                            RecipientTypeID = dr[RecipientsVM.Prop.RecipientTypeID.Name].ToString(),
                            RecipientDesc = dr[RecipientsVM.Prop.RecipientDesc.Name].ToString()
                        }
                    );
                    }
                }
                else
                {
                    message = "Error Get DRecipient";
                    return false;
                }
            }
            objNotification.Subject = Subject;
            objNotification.RecipientsVM = LstRecipients;

            #endregion
            List<string> mssg = new List<string>();
            object conn = null;
            bool status = false;
            if (IsBatch)
            {
                foreach (RecipientsVM recipient in LstRecipients)
                {
                    List<RecipientsVM> m_lsprivateRecipient = new List<RecipientsVM>();
                    m_lsprivateRecipient.Add(recipient);
                    objNotification.Contents = GenerateContent(objNotification.MailNotificationID, objNotification.NotificationTemplateID, m_lsprivateRecipient, LstNotificationValue, ref message);
                    objNotification.RecipientsVM = m_lsprivateRecipient;
                    status = SendMail(objNotification, ref mssg, ref conn);
                    if (mssg.Count>0) {
                        return false;
                    }
                }
            }
            else
                status = SendMail(objNotification, ref mssg, ref conn);
            message = string.Join(Global.NewLineSeparated, mssg);
            return status;

            //#region From,To,CC,Bcc


            //if (LstRecipients == null)
            //{
            //    message = "no recipient";
            //    return false;
            //}
            //else
            //{
            //    foreach (RecipientsVM To_ in LstRecipients.Where(to => to.RecipientTypeID == ((int)Enum.RecipientTypes.TO).ToString()))
            //        To += To_.MailAddress + ";";

            //    foreach (RecipientsVM cc_ in LstRecipients.Where(to => to.RecipientTypeID == ((int)Enum.RecipientTypes.CC).ToString()))
            //        Cc += cc_.MailAddress + ";";

            //    foreach (RecipientsVM bcc_ in LstRecipients.Where(to => to.RecipientTypeID == ((int)Enum.RecipientTypes.BCC).ToString()))
            //        Bcc += bcc_.MailAddress + ";";
            //}
            //#endregion

            //#region Attachment
            //var t = new mailws.MailSender();
            //mailws.MailAttachment[] Attachment = null;
            //if (LstNotificationAttachment != null)
            //    if (LstNotificationAttachment.Count > 0)
            //    {
            //        int TotalAttachment = LstNotificationAttachment.Count;
            //        int numberAttachment = 0;
            //        Attachment = new mailws.MailAttachment[TotalAttachment];
            //        foreach (NotificationAttachmentVM objAttachment in LstNotificationAttachment)
            //        {
            //            var u = new mailws.MailAttachment()
            //            {
            //                FileName = objAttachment.Filename,
            //                Content = GetByteFromBase64STR(objAttachment.RawData)
            //            };
            //            Attachment[numberAttachment] = u;
            //            numberAttachment++;
            //        }
            //    }
            //#endregion


            //#region Send Mail
            //try
            //{
            //    var m_lstconfig = GetConfig("Mail");
            //    if (!m_lstconfig.Any())
            //    {
            //        message ="No Mail configuration found!";
            //        return false;
            //    }
            //    string m_strfrom = string.Empty;
            //    string m_strreply = string.Empty;
            //    string m_strnotif = string.Empty;

            //    m_strfrom = m_lstconfig.Where(x => x.Key2 == "From").FirstOrDefault().Desc1;
            //    m_strreply = m_lstconfig.Where(x => x.Key2 == "Reply").FirstOrDefault().Desc1;
            //    m_strnotif = m_lstconfig.Where(x => x.Key2 == "Notification").FirstOrDefault().Desc1;

            //    string send = t.SendMailDelivery(m_strfrom, m_strreply, m_strnotif,
            //        To, Cc, Bcc, Subject, Contents, Attachment, true);

            //    bool isSuccess = send == "Success";
            //    if (!isSuccess)
            //        message = send;

            //    TMailHistoriesDA m_objTMailHistoriesDA = new TMailHistoriesDA();
            //    m_objTMailHistoriesDA.ConnectionStringName = Global.ConnStrConfigName;
            //    object m_objDBConnection = null;
            //    string m_strTransName = "History";
            //    m_objDBConnection = m_objTMailHistoriesDA.BeginTrans(m_strTransName);
            //    m_objTMailHistoriesDA.Data = new TMailHistories()
            //    {
            //        MailHistoryID = Guid.NewGuid().ToString().Replace("-", ""),
            //        StatusDate = DateTime.Now,
            //        To = To,
            //        CC = Cc,
            //        BCC = Bcc,
            //        Subject = Subject,
            //        Content = Contents,
            //        MailNotificationID = MailNotificationID,
            //        StatusID = isSuccess ? ((int)NotificationStatus.Sent).ToString() : ((int)NotificationStatus.Fail).ToString()
            //    };
            //    m_objTMailHistoriesDA.Insert(true, m_objDBConnection);
            //    if (!m_objTMailHistoriesDA.Success)
            //    {
            //        message = "Error create History ";
            //        m_objTMailHistoriesDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
            //        return false;
            //    }
            //    m_objTMailHistoriesDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
            //    return isSuccess;
            //}
            //catch (Exception e)
            //{
            //    message = "Error sending mail " + e.Message;
            //    return false;
            //}
            //#endregion

        }

        protected List<string> GetVendorDefaultEmail(string VendorID)
        {
            List<string> m_lstemail = new List<string>();

            List<VendorCommunicationsVM> lst_VendorCommunicationsVM = new List<VendorCommunicationsVM>();
            DVendorCommunicationsDA m_objDVendorCommunicationsDA = new DVendorCommunicationsDA();
            m_objDVendorCommunicationsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(VendorCommunicationsVM.Prop.VendorCommID.MapAlias);
            m_lstSelect.Add(VendorCommunicationsVM.Prop.CommunicationTypeID.MapAlias);
            m_lstSelect.Add(VendorCommunicationsVM.Prop.VendorPICID.MapAlias);
            m_lstSelect.Add(VendorCommunicationsVM.Prop.IsDefault.MapAlias);
            m_lstSelect.Add(VendorCommunicationsVM.Prop.CommDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(VendorID);
            m_objFilter.Add(VendorCommunicationsVM.Prop.VendorID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("004");//todo: enum
            m_objFilter.Add(VendorCommunicationsVM.Prop.CommunicationTypeID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicDVendorCommunicationsDA = m_objDVendorCommunicationsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDVendorCommunicationsDA.Message == string.Empty)
            {
                foreach (DataRow dr in m_dicDVendorCommunicationsDA[0].Tables[0].Rows)
                {
                    VendorCommunicationsVM obj_VendorCommunicationsVM = new VendorCommunicationsVM();
                    obj_VendorCommunicationsVM.VendorCommID = dr[VendorCommunicationsVM.Prop.VendorCommID.Name].ToString();
                    obj_VendorCommunicationsVM.CommunicationTypeID = dr[VendorCommunicationsVM.Prop.CommunicationTypeID.Name].ToString();
                    obj_VendorCommunicationsVM.VendorPICID = dr[VendorCommunicationsVM.Prop.VendorPICID.Name].ToString();
                    obj_VendorCommunicationsVM.IsDefault = (bool)dr[VendorCommunicationsVM.Prop.IsDefault.Name];
                    obj_VendorCommunicationsVM.CommDesc = dr[VendorCommunicationsVM.Prop.CommDesc.Name].ToString();
                    lst_VendorCommunicationsVM.Add(obj_VendorCommunicationsVM);
                }
            }
            foreach (var item in lst_VendorCommunicationsVM.Where(x => x.IsDefault))
            {
                m_lstemail.Add(item.CommDesc);
            }

            return m_lstemail;
        }

        protected List<FPTVendorParticipantsVM> GetListNegoVendorParticipant(string FPTID, ref string message)
        {
            DFPTVendorParticipantsDA m_objDFPTVendorParticipantsDA = new DFPTVendorParticipantsDA();
            m_objDFPTVendorParticipantsDA.ConnectionStringName = Global.ConnStrConfigName;

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(FPTID);
            m_objFilter.Add(FPTVendorParticipantsVM.Prop.FPTID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicDFPTVendorParticipantsDA = m_objDFPTVendorParticipantsDA.SelectBC(0, null, true, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpFPTVendorParticipantsBL in m_dicDFPTVendorParticipantsDA)
            {
                m_intCount = m_kvpFPTVendorParticipantsBL.Key;
                break;
            }

            List<FPTVendorParticipantsVM> m_lstFPTVendorParticipantsVM = new List<FPTVendorParticipantsVM>();
            if (m_intCount > 0)
            {
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(FPTVendorParticipantsVM.Prop.VendorID.MapAlias);
                m_lstSelect.Add(FPTVendorParticipantsVM.Prop.VendorName.MapAlias);
                m_lstSelect.Add(FPTVendorParticipantsVM.Prop.NegotiationConfigID.MapAlias);
                m_lstSelect.Add(FPTVendorParticipantsVM.Prop.FPTVendorParticipantID.MapAlias);
                m_lstSelect.Add(FPTVendorParticipantsVM.Prop.StatusID.MapAlias);

                m_dicDFPTVendorParticipantsDA = m_objDFPTVendorParticipantsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);
                if (m_objDFPTVendorParticipantsDA.Message == string.Empty)
                {
                    m_lstFPTVendorParticipantsVM = (
                        from DataRow m_drDFPTVendorParticipantsDA in m_dicDFPTVendorParticipantsDA[0].Tables[0].Rows
                        select new FPTVendorParticipantsVM()
                        {
                            FPTVendorParticipantID = m_drDFPTVendorParticipantsDA[FPTVendorParticipantsVM.Prop.FPTVendorParticipantID.Name].ToString(),
                            NegotiationConfigID = m_drDFPTVendorParticipantsDA[FPTVendorParticipantsVM.Prop.NegotiationConfigID.Name].ToString(),
                            VendorID = m_drDFPTVendorParticipantsDA[FPTVendorParticipantsVM.Prop.VendorID.Name].ToString(),
                            VendorName = m_drDFPTVendorParticipantsDA[FPTVendorParticipantsVM.Prop.VendorName.Name].ToString(),
                            StatusID = m_drDFPTVendorParticipantsDA[FPTVendorParticipantsVM.Prop.StatusID.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return m_lstFPTVendorParticipantsVM;
        }
        protected List<FPTTCParticipantsVM> GetListNegoTCParticipant(string FPTID, ref string message)
        {
            DFPTTCParticipantsDA m_objDFPTTCParticipantsDA = new DFPTTCParticipantsDA();
            m_objDFPTTCParticipantsDA.ConnectionStringName = Global.ConnStrConfigName;

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(FPTID);
            m_objFilter.Add(FPTTCParticipantsVM.Prop.FPTID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicDFPTTCParticipantsDA = m_objDFPTTCParticipantsDA.SelectBC(0, null, true, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpFPTTCParticipantsBL in m_dicDFPTTCParticipantsDA)
            {
                m_intCount = m_kvpFPTTCParticipantsBL.Key;
                break;
            }

            List<FPTTCParticipantsVM> m_lstFPTTCParticipantsVM = new List<FPTTCParticipantsVM>();
            if (m_intCount > 0)
            {
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(FPTTCParticipantsVM.Prop.TCMemberID.MapAlias);
                m_lstSelect.Add(FPTTCParticipantsVM.Prop.EmployeeID.MapAlias);
                m_lstSelect.Add(FPTTCParticipantsVM.Prop.EmployeeName.MapAlias);
                m_lstSelect.Add(FPTTCParticipantsVM.Prop.FPTID.MapAlias);
                m_lstSelect.Add(FPTTCParticipantsVM.Prop.FPTTCParticipantID.MapAlias);
                m_lstSelect.Add(FPTTCParticipantsVM.Prop.StatusID.MapAlias);
                m_lstSelect.Add(FPTTCParticipantsVM.Prop.IsDelegation.MapAlias);
                m_lstSelect.Add(BusinessUnitVM.Prop.BusinessUnitID.MapAlias);
                m_lstSelect.Add(BusinessUnitVM.Prop.BusinessUnitDesc.MapAlias);

                //List<string> m_lstGroup = new List<string>();
                //m_lstGroup.Add(FPTTCParticipantsVM.Prop.TCMemberID.Map);
                //m_lstGroup.Add(FPTTCParticipantsVM.Prop.NegotiationConfigID.Map);
                //m_lstGroup.Add(FPTTCParticipantsVM.Prop.FPTTCParticipantID.Map);
                //m_lstGroup.Add(FPTTCParticipantsVM.Prop.StatusID.Map);
                //m_lstGroup.Add(FPTTCParticipantsVM.Prop.IsDelegation.Map);
                //m_lstGroup.Add(FPTTCParticipantsVM.Prop.EmployeeName.Map);


                m_dicDFPTTCParticipantsDA = m_objDFPTTCParticipantsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);
                if (m_objDFPTTCParticipantsDA.Message == string.Empty)
                {
                    m_lstFPTTCParticipantsVM = (
                        from DataRow m_drDFPTTCParticipantsDA in m_dicDFPTTCParticipantsDA[0].Tables[0].Rows
                        select new FPTTCParticipantsVM()
                        {
                            FPTTCParticipantID = m_drDFPTTCParticipantsDA[FPTTCParticipantsVM.Prop.FPTTCParticipantID.Name].ToString(),
                            FPTID = m_drDFPTTCParticipantsDA[FPTTCParticipantsVM.Prop.FPTID.Name].ToString(),
                            TCMemberID = m_drDFPTTCParticipantsDA[FPTTCParticipantsVM.Prop.TCMemberID.Name].ToString(),
                            EmployeeID = m_drDFPTTCParticipantsDA[FPTTCParticipantsVM.Prop.EmployeeID.Name].ToString(),
                            EmployeeName = m_drDFPTTCParticipantsDA[FPTTCParticipantsVM.Prop.EmployeeName.Name].ToString(),
                            StatusID = bool.Parse(m_drDFPTTCParticipantsDA[FPTTCParticipantsVM.Prop.StatusID.Name].ToString()),
                            IsDelegation = bool.Parse(m_drDFPTTCParticipantsDA[FPTTCParticipantsVM.Prop.IsDelegation.Name].ToString()),
                            BusinessUnitID = m_drDFPTTCParticipantsDA[BusinessUnitVM.Prop.BusinessUnitID.Name].ToString(),
                            BusinessUnitDesc = m_drDFPTTCParticipantsDA[BusinessUnitVM.Prop.BusinessUnitDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return m_lstFPTTCParticipantsVM;
        }

        #region Get Unit Price
        protected decimal? GetUnitPrice(string ItemVersionChildID, string ItemID, ItemPriceVM ItemPriceVM, string ItemTypeID, string ColItemTypeID, string Formula = "", DateTime? CreatedDate = null, decimal? DefaultAmount = 0, bool IsBOI = false)
        {
            bool m_boolHasFormula = false;
            decimal? m_decUnitPrice = 0;

            List<string> m_lstReferenceParameters = new List<string>();
            Dictionary<string, decimal> m_dicVariables = new Dictionary<string, decimal>();

            if (FilterItemTypeConfig(ItemTypeID, ColItemTypeID))
            {
                if (string.IsNullOrEmpty(Formula))
                {
                    ItemVersionChildVM m_objItemVersionChild = GetItemVersionChild(ItemVersionChildID);
                    Formula = m_objItemVersionChild.Formula;
                }
                //ConfigAHSChildVM m_objConfigAHSChildVM = GetConfigAHSChlid(ItemTypeID);

                if (!string.IsNullOrEmpty(Formula))
                {

                    m_decUnitPrice = GetFromFormula(ItemVersionChildID, ItemPriceVM, CreatedDate);
                    if (m_decUnitPrice > 0) m_boolHasFormula = true;

                }
                //else
                //m_decUnitPrice = GetFromItemPrice(ItemID, ItemPriceVM);

                if (!m_boolHasFormula)
                    m_decUnitPrice = GetFromItemPrice(ItemID, ItemPriceVM, CreatedDate);

                //if (m_objConfigAHSChildVM.HasCoefficient)
                //m_decUnitPrice = m_decUnitPrice * m_objItemVersionChild.Coefficient;

            }
            else
            {
                if (IsBOI)
                    m_decUnitPrice = DefaultAmount;
            }
            return m_decUnitPrice == 0 ? null : m_decUnitPrice;
        }
        private decimal GetFromItemPrice(string ItemID, ItemPriceVM ItemPriceVM, DateTime? CreatedDate = null, string FilterNotIncluded = "", string ItemVersionChildID = "", decimal? CoefficientValue = null)
        {
            string message = string.Empty;
            decimal m_decItemPrice = 0;

            ItemPriceVendorPeriodVM m_objItemPriceVendorPeriod = GetItemPriceVendorPeriod(ItemID, ItemPriceVM.RegionID, ItemPriceVM.ProjectID, ItemPriceVM.ClusterID, ItemPriceVM.UnitTypeID, FilterNotIncluded, ref message, ItemVersionChildID, CreatedDate);
            if (m_objItemPriceVendorPeriod != null)
            {
                m_decItemPrice = m_objItemPriceVendorPeriod.Amount;
            }
            else
            {
                if (FilterNotIncluded != ItemPriceVM.Prop.RegionID.Name.ToString() && m_decItemPrice == 0)
                {
                    switch (FilterNotIncluded)
                    {
                        case "ProjectID":
                            FilterNotIncluded = ItemPriceVM.Prop.RegionID.Name.ToString();
                            break;
                        case "ClusterID":
                            FilterNotIncluded = ItemPriceVM.Prop.ProjectID.Name.ToString();
                            break;
                        case "UnitTypeID":
                            FilterNotIncluded = ItemPriceVM.Prop.ClusterID.Name.ToString();
                            break;
                        default:
                            FilterNotIncluded = ItemPriceVM.Prop.UnitTypeID.Name.ToString();
                            break;
                    }
                    m_decItemPrice = GetFromItemPrice(ItemID, ItemPriceVM, CreatedDate, FilterNotIncluded, ItemVersionChildID, null);
                }
            }

            if (CoefficientValue != null)
                m_decItemPrice = m_decItemPrice * (decimal)CoefficientValue;
            return m_decItemPrice;
        }
        private decimal GetFromFormula(string ItemVersionChildID, ItemPriceVM ItemPriceVM, DateTime? CreatedDate)
        {
            string message = string.Empty;
            string m_strReferenceFormula = string.Empty;
            string m_strFormula = string.Empty;
            decimal m_decRefAmount = 0m;
            decimal m_decUnitPriceFormula = 0m;
            string m_strReferenceParameter = string.Empty;
            string m_strParameterPrefix = "X";

            List<string> m_lstReferenceParameters = new List<string>();
            Dictionary<string, decimal> m_dicVariables = new Dictionary<string, decimal>();
            ItemVersionChildVM m_objItemVersionChild = GetItemVersionChild(ItemVersionChildID);

            if (!string.IsNullOrEmpty(m_objItemVersionChild.Formula))
            {
                m_strReferenceFormula = m_objItemVersionChild.Formula.Trim();
                while (m_strReferenceFormula.Contains("[") && m_strReferenceFormula.Contains("]"))
                {
                    m_strReferenceParameter = m_strReferenceFormula.Substring(m_strReferenceFormula.IndexOf("["), m_strReferenceFormula.IndexOf("]", m_strReferenceFormula.IndexOf("["))
                        - m_strReferenceFormula.IndexOf("[") + 1);
                    if (m_strReferenceParameter.Length <= m_strReferenceFormula.Length)
                        m_strReferenceFormula = m_strReferenceFormula.Substring(m_strReferenceFormula.IndexOf("]") + 1).Trim();
                    m_lstReferenceParameters.Add(m_strReferenceParameter.Replace("[", "").Replace("]", "").Trim());
                }
                m_lstReferenceParameters = m_lstReferenceParameters.Distinct().ToList();

                foreach (string m_strParameter in m_lstReferenceParameters)
                {
                    if (m_strParameter != ItemVersionChildID)
                        m_decRefAmount = GetFromFormula(m_strParameter, ItemPriceVM, CreatedDate);
                    if (m_decRefAmount == 0)
                    {
                        ItemVersionChildVM m_objItemVersionChd = GetItemVersionChild(m_strParameter);
                        decimal m_decCoefficient = ItemVersionChildID == m_strParameter ? 1 : m_objItemVersionChd.Coefficient;
                        m_decRefAmount = GetFromItemPrice(string.Empty, ItemPriceVM, CreatedDate, string.Empty, m_strParameter, m_decCoefficient);
                    }
                    m_dicVariables[m_strParameterPrefix + m_strParameter] = m_decRefAmount;
                }
                m_strFormula = m_objItemVersionChild.Formula.Trim().Replace("[", m_strParameterPrefix).Replace("]", string.Empty).Trim();
                m_decUnitPriceFormula = CalculateFormula(m_strFormula, ref message, m_dicVariables, true);
            }
            return m_decUnitPriceFormula;
        }
        private bool FilterItemTypeConfig(string filterKey2, string filterKey4)
        {

            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ConfigVM.Prop.Key3.MapAlias);

            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name);
            m_objFilter.Add(ConfigVM.Prop.Key1.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(filterKey2);
            m_objFilter.Add(ConfigVM.Prop.Key2.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(filterKey4);
            m_objFilter.Add(ConfigVM.Prop.Key4.Map, m_lstFilter);


            Dictionary<int, DataSet> m_dicUConfigDA = m_objUConfigDA.SelectBC(0, null, true, m_lstSelect, m_objFilter, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpUConfigDA in m_dicUConfigDA)
            {
                m_intCount = m_kvpUConfigDA.Key;
                break;
            }


            return m_intCount > 0;

        }
        private ConfigAHSChildVM GetConfigAHSChlid(string filterKey3)
        {

            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<string> m_lstSelect = new List<string>();

            m_lstSelect.Add(ConfigVM.Prop.Key2.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Key3.MapAlias);

            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name);
            m_objFilter.Add(ConfigVM.Prop.Key1.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(filterKey3);
            m_objFilter.Add(ConfigVM.Prop.Key3.Map, m_lstFilter);

            ConfigAHSChildVM m_objConfigAHSChildVM = new ConfigAHSChildVM();
            Dictionary<int, DataSet> m_dicUConfigDA = m_objUConfigDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objUConfigDA.Message == string.Empty)
            {
                foreach (DataRow m_dtRow in m_dicUConfigDA[0].Tables[0].Rows)
                {
                    m_objConfigAHSChildVM.ItemTypeID = m_dtRow[ConfigVM.Prop.Key3.Name].ToString();
                    if (m_dtRow[ConfigVM.Prop.Key2.Name].ToString() == ConfigAHSChildVM.Prop.HasAlternativeItem.Desc)
                        m_objConfigAHSChildVM.HasAlternativeItem = true;
                    else if (m_dtRow[ConfigVM.Prop.Key2.Name].ToString() == ConfigAHSChildVM.Prop.HasCoefficient.Desc)
                        m_objConfigAHSChildVM.HasCoefficient = true;
                    else if (m_dtRow[ConfigVM.Prop.Key2.Name].ToString() == ConfigAHSChildVM.Prop.HasFormula.Desc)
                        m_objConfigAHSChildVM.HasFormula = true;
                }
            }

            return m_objConfigAHSChildVM;

        }
        private ItemPriceVendorPeriodVM GetItemPriceVendorPeriod(string ItemID, string RegionID, string ProjectID, string ClusterID, string UnitTypeID, string FilterNotIncluded, ref string message, string ItemVersionChildID = "", DateTime? CreatedDate = null)
        {

            ItemPriceVendorPeriodVM m_objItemPriceVendorPeriodVM = new ItemPriceVendorPeriodVM();
            DItemPriceVendorPeriodDA m_objDItemPriceVendorPeriodDA = new DItemPriceVendorPeriodDA();
            m_objDItemPriceVendorPeriodDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.ItemPriceID.MapAlias);
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.CurrencyID.MapAlias);
            m_lstSelect.Add(ItemPriceVendorPeriodVM.Prop.Amount.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            if (!string.IsNullOrEmpty(ItemVersionChildID))
            {

                ItemID = GetItemVersionChild(ItemVersionChildID).ChildItemID;
            }

            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ItemID ?? string.Empty);
            m_objFilter.Add(ItemPriceVM.Prop.ItemID.Map, m_lstFilter);


            //if (!string.IsNullOrEmpty(RegionID))
            //{
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);

            if (RegionID == null)
                RegionID = string.Empty;

            m_lstFilter.Add(RegionID);
            m_objFilter.Add(ItemPriceVM.Prop.RegionID.Map, m_lstFilter);
            //}

            if (!string.IsNullOrEmpty(ProjectID) && FilterNotIncluded != ItemPriceVM.Prop.ProjectID.Name)
            {

                if (!string.IsNullOrEmpty(ClusterID) && FilterNotIncluded != ItemPriceVM.Prop.ClusterID.Name)
                {

                    if (!string.IsNullOrEmpty(UnitTypeID) && FilterNotIncluded != ItemPriceVM.Prop.UnitTypeID.Name)
                    {
                        m_lstFilter = new List<object>();
                        m_lstFilter.Add(Operator.Equals);
                        m_lstFilter.Add(UnitTypeID);
                        m_objFilter.Add(ItemPriceVM.Prop.UnitTypeID.Map, m_lstFilter);
                    }

                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(ClusterID);
                    m_objFilter.Add(ItemPriceVM.Prop.ClusterID.Map, m_lstFilter);
                }
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(ProjectID);
                m_objFilter.Add(ItemPriceVM.Prop.ProjectID.Map, m_lstFilter);

            }

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(1);
            m_objFilter.Add(ItemPriceVendorVM.Prop.IsDefault.Map, m_lstFilter);

            string m_strCreatedDate = CreatedDate == null ? DateTime.Now.Date.ToString(Global.SqlDateFormat) : CreatedDate.Value.Date.ToString(Global.SqlDateFormat);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.LessThanEqual);
            m_lstFilter.Add(m_strCreatedDate);
            m_objFilter.Add(ItemPriceVendorPeriodVM.Prop.ValidFrom.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.GreaterThanEqual);
            m_lstFilter.Add(m_strCreatedDate);
            m_objFilter.Add(ItemPriceVendorPeriodVM.Prop.ValidTo.Map, m_lstFilter);

            Dictionary<string, OrderDirection> m_dicOrderBy = new Dictionary<string, OrderDirection>();
            m_dicOrderBy.Add(ItemPriceVM.Prop.ItemID.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicDItemPriceVendorPeriodDA = m_objDItemPriceVendorPeriodDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, m_dicOrderBy);
            if (m_objDItemPriceVendorPeriodDA.Success)
            {
                m_objItemPriceVendorPeriodVM =
                (from DataRow m_drDItemPriceVendorPeriodDA in m_dicDItemPriceVendorPeriodDA[0].Tables[0].Rows
                 select new ItemPriceVendorPeriodVM()
                 {
                     ItemPriceID = m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.ItemPriceID.Name].ToString(),
                     Amount = (decimal)m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.Amount.Name],
                     CurrencyID = m_drDItemPriceVendorPeriodDA[ItemPriceVendorPeriodVM.Prop.CurrencyID.Name].ToString()
                 }).FirstOrDefault();
            }
            else
                message = m_objDItemPriceVendorPeriodDA.Message;

            return m_objItemPriceVendorPeriodVM;

        }
        private ItemVersionChildVM GetFormula(string ItemVersionChildID)
        {

            DItemVersionChildDA m_objItemVersionChildDA = new DItemVersionChildDA();
            m_objItemVersionChildDA.ConnectionStringName = Global.ConnStrConfigName;
            object m_objDBConnection = m_objItemVersionChildDA.BeginConnection();

            ItemVersionChildVM m_objItemVersionChildVM = new ItemVersionChildVM();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ItemVersionChildID);
            m_objFilter.Add(ItemVersionChildVM.Prop.ItemVersionChildID.Map, m_lstFilter);

            bool m_boolIsCount = false;
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemVersionChildID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildItemID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildVersion.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Sequence.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Formula.MapAlias);

            Dictionary<int, DataSet> m_dicMItemVersionDA = m_objItemVersionChildDA.SelectBC(0, 1, m_boolIsCount, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objItemVersionChildDA.Message == "")
            {

                DataRow m_drMItemVersionChildDA = m_dicMItemVersionDA[0].Tables[0].Rows[0];
                m_objItemVersionChildVM = new ItemVersionChildVM()
                {
                    ItemVersionChildID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemVersionChildID.Name].ToString(),
                    ChildItemID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemID.Name].ToString(),
                    ChildVersion = (int)m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildVersion.Name],
                    Sequence = (int)m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Sequence.Name],
                    Formula = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Formula.Name].ToString(),
                    Coefficient = decimal.Parse(m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Coefficient.Name].ToString())
                };
            }

            return m_objItemVersionChildVM;
        }
        private ItemVersionChildVM GetItemVersionChild(string ItemVersionChildID)
        {
            #region DItemVersionChild
            DItemVersionChildDA m_objDItemVersionChildDA = new DItemVersionChildDA();
            m_objDItemVersionChildDA.ConnectionStringName = Global.ConnStrConfigName;

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            if (!string.IsNullOrEmpty(ItemVersionChildID))
            {
                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(ItemVersionChildID);
                m_objFilter.Add(ItemVersionChildVM.Prop.ItemVersionChildID.Map, m_lstFilter);
            }

            ItemVersionChildVM m_objItemVersionChildVM = new ItemVersionChildVM();

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemTypeDesc.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemGroupDesc.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Version.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.VersionDesc.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemTypeID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.UoMDesc.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildItemID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildItemDesc.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildVersion.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Sequence.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Formula.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Coefficient.MapAlias);

            Dictionary<int, DataSet> m_dicDItemVersionChildDA = m_objDItemVersionChildDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null, null);
            if (m_objDItemVersionChildDA.Message == string.Empty)
            {
                DataRow m_drDItemVersionChildDA = m_dicDItemVersionChildDA[0].Tables[0].Rows[0];
                m_objItemVersionChildVM = new ItemVersionChildVM()
                {
                    ItemID = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemID.Name].ToString(),
                    ItemDesc = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemDesc.Name].ToString(),
                    ItemTypeDesc = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemTypeDesc.Name].ToString(),
                    ItemGroupDesc = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemGroupDesc.Name].ToString(),
                    Version = (int)m_drDItemVersionChildDA[ItemVersionChildVM.Prop.Version.Name],
                    VersionDesc = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.VersionDesc.Name].ToString(),
                    UoMDesc = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.UoMDesc.Name].ToString(),
                    ItemTypeID = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemTypeID.Name].ToString(),
                    Coefficient = decimal.Parse(m_drDItemVersionChildDA[ItemVersionChildVM.Prop.Coefficient.Name].ToString()),
                    Formula = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.Formula.Name].ToString(),
                    ChildItemID = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemID.Name].ToString(),
                    ChildItemDesc = m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ItemDesc.Name].ToString(),
                    ChildVersion = int.Parse(m_drDItemVersionChildDA[ItemVersionChildVM.Prop.ChildVersion.Name].ToString()),
                    Sequence = int.Parse(m_drDItemVersionChildDA[ItemVersionChildVM.Prop.Sequence.Name].ToString()),
                };
            }

            #endregion
            return m_objItemVersionChildVM;
        }
        #endregion

        protected string ConvertFirstCharToLower(string stringname)
        {
            return stringname[0].ToString().ToLower() + stringname.Substring(1);
        }
        public static string ToRoman(int number)
        {
            if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException("insert value betwheen 1 and 3999");
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + ToRoman(number - 1000);
            if (number >= 900) return "CM" + ToRoman(number - 900);
            if (number >= 500) return "D" + ToRoman(number - 500);
            if (number >= 400) return "CD" + ToRoman(number - 400);
            if (number >= 100) return "C" + ToRoman(number - 100);
            if (number >= 90) return "XC" + ToRoman(number - 90);
            if (number >= 50) return "L" + ToRoman(number - 50);
            if (number >= 40) return "XL" + ToRoman(number - 40);
            if (number >= 10) return "X" + ToRoman(number - 10);
            if (number >= 9) return "IX" + ToRoman(number - 9);
            if (number >= 5) return "V" + ToRoman(number - 5);
            if (number >= 4) return "IV" + ToRoman(number - 4);
            if (number >= 1) return "I" + ToRoman(number - 1);
            throw new ArgumentOutOfRangeException("Out Of Range");
        }

        #region Negotiation
        protected FPTVM GetFPTVM(string FPTID)
        {
            FPTVM m_objFPTVM = new FPTVM();
            MFPTDA m_objMFPTDA = new MFPTDA();
            m_objMFPTDA.ConnectionStringName = Global.ConnStrConfigName;

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<string> m_lstSelect = new List<string>();
            List<object> m_lstFilter = new List<object>();

            m_lstSelect.Add(FPTVM.Prop.FPTID.MapAlias);
            m_lstSelect.Add(FPTVM.Prop.Descriptions.MapAlias);
            m_lstSelect.Add(FPTVM.Prop.Schedule.MapAlias);
            m_lstSelect.Add(FPTVM.Prop.ClusterID.MapAlias);
            m_lstSelect.Add(FPTVM.Prop.ClusterDesc.MapAlias);
            m_lstSelect.Add(FPTVM.Prop.ProjectID.MapAlias);
            m_lstSelect.Add(FPTVM.Prop.ProjectDesc.MapAlias);
            m_lstSelect.Add(FPTVM.Prop.DivisionID.MapAlias);
            m_lstSelect.Add(FPTVM.Prop.DivisionDesc.MapAlias);
            m_lstSelect.Add(FPTVM.Prop.BusinessUnitID.MapAlias);
            m_lstSelect.Add(FPTVM.Prop.BusinessUnitDesc.MapAlias);
            m_lstSelect.Add(FPTVM.Prop.CreatedDate.MapAlias);
            m_lstSelect.Add(FPTVM.Prop.IsSync.MapAlias);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(FPTID);
            m_objFilter.Add(FPTVM.Prop.FPTID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMFPTDA = m_objMFPTDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMFPTDA.Message == string.Empty)
            {
                DataRow m_drMFPTDA = m_dicMFPTDA[0].Tables[0].Rows[0];

                m_objFPTVM.FPTID = m_drMFPTDA[FPTVM.Prop.FPTID.Name].ToString();
                m_objFPTVM.CreatedDate = string.IsNullOrEmpty(m_drMFPTDA[FPTVM.Prop.CreatedDate.Name].ToString()) ? null : (DateTime?)m_drMFPTDA[FPTVM.Prop.CreatedDate.Name];
                m_objFPTVM.Descriptions = m_drMFPTDA[FPTVM.Prop.Descriptions.Name].ToString();
                m_objFPTVM.ClusterID = m_drMFPTDA[FPTVM.Prop.ClusterID.Name].ToString();
                m_objFPTVM.ClusterDesc = m_drMFPTDA[FPTVM.Prop.ClusterDesc.Name].ToString();
                m_objFPTVM.ProjectID = m_drMFPTDA[FPTVM.Prop.ProjectID.Name].ToString();
                m_objFPTVM.ProjectDesc = m_drMFPTDA[FPTVM.Prop.ProjectDesc.Name].ToString();
                m_objFPTVM.DivisionID = m_drMFPTDA[FPTVM.Prop.DivisionID.Name].ToString();
                m_objFPTVM.DivisionDesc = m_drMFPTDA[FPTVM.Prop.DivisionDesc.Name].ToString();
                m_objFPTVM.BusinessUnitID = m_drMFPTDA[FPTVM.Prop.BusinessUnitID.Name].ToString();
                m_objFPTVM.BusinessUnitDesc = m_drMFPTDA[FPTVM.Prop.BusinessUnitDesc.Name].ToString();
                //m_objFPTVM.Projects = GetListFPTProjectsVM(m_drMFPTDA[FPTVM.Prop.FPTID.Name].ToString(), ref messages);
                //m_objFPTVM.Vendors = GetListBFPTVendorParticipantsVM(m_drMFPTDA[FPTVM.Prop.FPTID.Name].ToString(), ref messages);
                //m_objFPTVM.BudgetPlans = GetListFPTBudgetPlanVM(m_drMFPTDA[FPTVM.Prop.FPTID.Name].ToString(), ref messages);
                //m_objFPTVM.ListFPTStatusVM = GetListFPTStatusVM(m_drMFPTDA[FPTVM.Prop.FPTID.Name].ToString(), ref messages);
                m_objFPTVM.Schedule = m_drMFPTDA[FPTVM.Prop.Schedule.Name].ToString();
                m_objFPTVM.IsSync = (bool)m_drMFPTDA[FPTVM.Prop.IsSync.Name];
                //m_objFPTVM.ListNegotiationConfigurationsVM = GetListNegotiationConfigurationsVM(m_drMFPTDA[FPTVM.Prop.FPTID.Name].ToString(), ref messages);
            }



            return m_objFPTVM;
        }

        protected TCMembersVM GetListTCMembers(Dictionary<string, object> selected, ref string message)
        {
            TCMembersVM m_objTCMembersVM = new TCMembersVM();
            TTCMembersDA m_objTTCMembersDA = new TTCMembersDA();
            m_objTTCMembersDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(TCMembersVM.Prop.TCMemberID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.EmployeeID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.EmployeeName.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.SuperiorID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.SuperiorName.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.TCTypeID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.TCTypeDesc.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.PeriodStart.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.PeriodEnd.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.DelegateStartDate.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.DelegateEndDate.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.DelegateTo.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objTCMembersVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(TCMembersVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicTTCMembersDA = m_objTTCMembersDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objTTCMembersDA.Message == string.Empty)
            {
                DataRow m_drTTCMembersDA = m_dicTTCMembersDA[0].Tables[0].Rows[0];
                m_objTCMembersVM.TCMemberID = m_drTTCMembersDA[TCMembersVM.Prop.TCMemberID.Name].ToString();
                m_objTCMembersVM.EmployeeID = m_drTTCMembersDA[TCMembersVM.Prop.EmployeeID.Name].ToString();
                m_objTCMembersVM.EmployeeName = m_drTTCMembersDA[TCMembersVM.Prop.EmployeeName.Name].ToString();
                m_objTCMembersVM.SuperiorID = m_drTTCMembersDA[TCMembersVM.Prop.SuperiorID.Name].ToString();
                m_objTCMembersVM.SuperiorName = m_drTTCMembersDA[TCMembersVM.Prop.SuperiorName.Name].ToString();
                m_objTCMembersVM.TCTypeID = m_drTTCMembersDA[TCMembersVM.Prop.TCTypeID.Name].ToString();
                m_objTCMembersVM.PeriodStart = DateTime.Parse(m_drTTCMembersDA[TCMembersVM.Prop.PeriodStart.Name].ToString());
                m_objTCMembersVM.PeriodEnd = DateTime.Parse(m_drTTCMembersDA[TCMembersVM.Prop.PeriodEnd.Name].ToString());
                m_objTCMembersVM.DelegateStartDate = !string.IsNullOrEmpty(m_drTTCMembersDA[TCMembersVM.Prop.DelegateStartDate.Name].ToString()) ? DateTime.Parse(m_drTTCMembersDA[TCMembersVM.Prop.DelegateStartDate.Name].ToString()) : (DateTime?)null;
                m_objTCMembersVM.DelegateEndDate = !string.IsNullOrEmpty(m_drTTCMembersDA[TCMembersVM.Prop.DelegateStartDate.Name].ToString()) ? DateTime.Parse(m_drTTCMembersDA[TCMembersVM.Prop.DelegateEndDate.Name].ToString()) : (DateTime?)null;
                m_objTCMembersVM.DelegateTo = m_drTTCMembersDA[TCMembersVM.Prop.DelegateTo.Name].ToString();
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objTTCMembersDA.Message;

            return m_objTCMembersVM;
        }
        protected string GetCurrentApproval(string TaskType, int CurrentLevelApprovalPath)
        {
            string message = "";

            ApprovalPathVM m_objApppathVM = new ApprovalPathVM();
            CApprovalPathDA m_objApppathDA = new CApprovalPathDA();
            m_objApppathDA.ConnectionStringName = Global.ConnStrConfigName;
            string CurrentApprovalRole = "";
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ApprovalPathVM.Prop.RoleID.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();

            int x = 0;
            while (x <= CurrentLevelApprovalPath)
            {
                m_objFilter = new Dictionary<string, List<object>>();

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(TaskType);
                m_objFilter.Add(ApprovalPathVM.Prop.TaskTypeID.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(CurrentApprovalRole);
                m_objFilter.Add(ApprovalPathVM.Prop.RoleChildID.Map, m_lstFilter);

                Dictionary<int, DataSet> m_dicMTasksDA = m_objApppathDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
                if (m_objApppathDA.Message == string.Empty)
                {
                    DataRow m_drAppPathDA = m_dicMTasksDA[0].Tables[0].Rows[0];
                    CurrentApprovalRole = m_drAppPathDA[ApprovalPathVM.Prop.RoleID.Name].ToString();
                }
                else
                {
                    message = m_objApppathDA.Message;
                    return CurrentApprovalRole;
                }
                x++;
            }

            return CurrentApprovalRole;
        }
        private List<string> GetAllRoleIDByUserID(string UserID, ref string message)
        {
            List<string> AllRoleID = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();
            DUserRoleDA m_objDUserRoleDA = new DUserRoleDA();
            m_objDUserRoleDA.ConnectionStringName = Global.ConnStrConfigName;

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(UserID);
            m_objFilter.Add(UserRoleVM.Prop.UserID.Map, m_lstFilter);

            m_lstSelect = new List<string>();
            m_lstSelect.Add(UserRoleVM.Prop.RoleID.MapAlias);

            Dictionary<int, DataSet> m_dicMUserDA = m_objDUserRoleDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);
            if (m_objDUserRoleDA.Success)
                foreach (DataRow dr in m_dicMUserDA[0].Tables[0].Rows)
                {
                    AllRoleID.Add(dr[UserRoleVM.Prop.RoleID.Name].ToString());
                }
            else
            {
                message = "Get Role Error: " + m_objDUserRoleDA.Message;
                return AllRoleID;
            }

            return AllRoleID;

        }
        protected List<string> GetDelegateRoleID(ref string message)
        {
            List<ApprovalDelegationUserVM> ListDelegateUserID = new List<ApprovalDelegationUserVM>();
            List<string> ListTaskTypeID = new List<string>();
            List<string> ListUserID = new List<string>();
            List<string> ListRoleID = new List<string>();

            if (message == string.Empty)
            {
                DApprovalDelegationUserDA m_objDApprovalDelegationUserDA = new DApprovalDelegationUserDA();
                m_objDApprovalDelegationUserDA.ConnectionStringName = Global.ConnStrConfigName;

                CApprovalPathDA m_objCapprovalPath = new CApprovalPathDA();
                m_objCapprovalPath.ConnectionStringName = Global.ConnStrConfigName;

                List<string> m_lstSelects = new List<string>();
                m_lstSelects.Add(ApprovalDelegationUserVM.Prop.TaskTypeID.MapAlias);
                m_lstSelects.Add(ApprovalDelegationUserVM.Prop.ApprovalDelegationUserID.MapAlias);
                m_lstSelects.Add(ApprovalDelegationUserVM.Prop.PeriodStart.MapAlias);
                m_lstSelects.Add(ApprovalDelegationUserVM.Prop.UserID.MapAlias);

                List<object> m_lstFilter = new List<object>();
                Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.LessThanEqual);
                m_lstFilter.Add((DateTime.Now.Date.Add(new TimeSpan(0, 0, 0))).ToString(Global.SqlDateFormat));
                m_objFilter.Add(ApprovalDelegationUserVM.Prop.PeriodStart.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.GreaterThanEqual);
                m_lstFilter.Add((DateTime.Now.Date.Add(new TimeSpan(0, 0, 0))).ToString(Global.SqlDateFormat));
                m_objFilter.Add(ApprovalDelegationUserVM.Prop.PeriodEnd.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(System.Web.HttpContext.Current.User.Identity.Name);
                m_objFilter.Add(ApprovalDelegationUserVM.Prop.DelegateUserID.Map, m_lstFilter);

                Dictionary<int, DataSet> m_dicMApprovalDelegationDA = m_objDApprovalDelegationUserDA.SelectBC(0, null, false, m_lstSelects, m_objFilter, null, null, null, null);

                if (m_objDApprovalDelegationUserDA.Success)
                {
                    string TaskType_ = "";
                    string UserID_ = "";
                    Dictionary<int, DataSet> m_dicApprovalPath = new Dictionary<int, DataSet>();
                    m_lstSelects = new List<string>();
                    m_lstSelects.Add(ApprovalPathVM.Prop.RoleID.MapAlias);
                    m_lstSelects.Add(ApprovalPathVM.Prop.ApprovalPathID.MapAlias);

                    foreach (DataRow m_dr in m_dicMApprovalDelegationDA[0].Tables[0].Rows)
                    {
                        if (message != "")
                            break;

                        TaskType_ = m_dr[ApprovalDelegationUserVM.Prop.TaskTypeID.Name].ToString();
                        UserID_ = m_dr[ApprovalDelegationUserVM.Prop.UserID.Name].ToString();

                        if (!ListDelegateUserID.Any(x => x.TaskTypeID == TaskType_ && x.UserID == UserID_))
                        {
                            List<string> AllRoleID_ = GetAllRoleIDByUserID(UserID_, ref message);
                            if (message != "")
                                break;

                            m_objFilter = new Dictionary<string, List<object>>();

                            m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(TaskType_);
                            m_objFilter.Add(ApprovalPathVM.Prop.TaskTypeID.Map, m_lstFilter);

                            m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.In);
                            m_lstFilter.Add(string.Join(",", AllRoleID_));
                            m_objFilter.Add(ApprovalPathVM.Prop.RoleID.Map, m_lstFilter);

                            m_dicApprovalPath = m_objCapprovalPath.SelectBC(0, null, false, m_lstSelects, m_objFilter, null, null, null, null);
                            if (m_objCapprovalPath.Success)
                            {
                                foreach (DataRow dr__ in m_dicApprovalPath[0].Tables[0].Rows)
                                    if (!ListRoleID.Any(x => x == dr__[ApprovalPathVM.Prop.RoleID.Name].ToString()))
                                        ListRoleID.Add(dr__[ApprovalPathVM.Prop.RoleID.Name].ToString());
                            }
                            else
                                message = m_objCapprovalPath.Message;
                        }
                    }
                }
                else
                    message = m_objDApprovalDelegationUserDA.Message;

            }
            return ListRoleID;
        }
        protected List<string> GetAllowedTaskType(List<string> ListRoleID, ref string message)
        {
            List<string> returnTaskType = new List<string>();
            ApprovalPathVM m_objApppathVM = new ApprovalPathVM();
            CApprovalPathDA m_objApppathDA = new CApprovalPathDA();
            m_objApppathDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ApprovalPathVM.Prop.TaskTypeID.MapAlias);
            m_lstSelect.Add(ApprovalPathVM.Prop.ApprovalPathID.MapAlias);

            List<string> m_lstGroup = new List<string>();
            m_lstGroup.Add(ApprovalPathVM.Prop.TaskTypeID.Map);
            //m_lstGroup.Add(ApprovalPathVM.Prop.ApprovalPathID.Map);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(string.Join(",", ListRoleID));
            m_objFilter.Add(ApprovalPathVM.Prop.RoleID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMTasksDA = m_objApppathDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objApppathDA.Success)
            {
                foreach (DataRow dr in m_dicMTasksDA[0].Tables[0].Rows)
                    returnTaskType.Add(dr[ApprovalPathVM.Prop.TaskTypeID.Name].ToString());
            }
            else
                message = m_objApppathDA.Message;

            var sas = returnTaskType.GroupBy(x => x)
                .Select(g => new { g.Key })
                .ToList();
            returnTaskType = new List<string>();
            foreach (var s in sas)
                returnTaskType.Add(s.Key.ToString());

            return returnTaskType;
        }

        protected string GetFirstApprovalRole(string TaskType, ref string message)
        {

            ApprovalPathVM m_objApppathVM = new ApprovalPathVM();
            CApprovalPathDA m_objApppathDA = new CApprovalPathDA();
            m_objApppathDA.ConnectionStringName = Global.ConnStrConfigName;
            string FirstRoleApproval = "";

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ApprovalPathVM.Prop.RoleID.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(TaskType);
            m_objFilter.Add(ApprovalPathVM.Prop.TaskTypeID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(string.Empty);
            m_objFilter.Add(ApprovalPathVM.Prop.RoleChildID.Map, m_lstFilter);

            List<string> CurrentRoleID = GetAllRoleID(ref message);
            if (!string.IsNullOrEmpty(message))
                return FirstRoleApproval;


            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(string.Join(",", CurrentRoleID));
            m_objFilter.Add(ApprovalPathVM.Prop.RoleID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMTasksDA = m_objApppathDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objApppathDA.Success)
            {
                if (m_objApppathDA.AffectedRows <= 0)
                {
                    message = "Role doesn't match to ApprovalPath";
                    return FirstRoleApproval;
                }
                DataRow m_drAppPathDA = m_dicMTasksDA[0].Tables[0].Rows[0];
                FirstRoleApproval = m_drAppPathDA[ApprovalPathVM.Prop.RoleID.Name].ToString();

            }
            else
            {
                message = m_objApppathDA.Message;
                return FirstRoleApproval;
            }


            return FirstRoleApproval;
        }
        protected decimal GetVendorSubtotal(string VersionVendorID, decimal FeePercentage, ref string message)
        {
            decimal ReturnSubtotal = 0;
            List<BudgetPlanVersionEntryVM> m_lstVersionEntry = new List<BudgetPlanVersionEntryVM>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            DBudgetPlanVersionEntryDA m_objDBudgetPlanVersionEntryDA = new DBudgetPlanVersionEntryDA();
            m_objDBudgetPlanVersionEntryDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add("DBudgetPlanVersionEntry.Volume*(DBudgetPlanVersionEntry.MaterialAmount+DBudgetPlanVersionEntry.MiscAmount+DBudgetPlanVersionEntry.WageAmount)AS SubTotal");
            //m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanID.MapAlias);
            //m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersion.MapAlias);
            //m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.VendorID.MapAlias);
            //m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersionStructureID.MapAlias);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(VersionVendorID);
            m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersionVendorID.Map, m_lstFilter);
            //Dictionary<string, OrderDirection> m_dicOrderBy = new Dictionary<string, OrderDirection>();
            //m_dicOrderBy.Add("SubTotal", OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicDBudgetPlanVersionStructure = m_objDBudgetPlanVersionEntryDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objDBudgetPlanVersionEntryDA.Message == string.Empty && m_objDBudgetPlanVersionEntryDA.AffectedRows > 0)
            {
                m_lstVersionEntry = (
                    from DataRow m_dBudgetPlanVersionVendorDA in m_dicDBudgetPlanVersionStructure[0].Tables[0].Rows
                    select new BudgetPlanVersionEntryVM()
                    {
                        Total = (decimal)m_dBudgetPlanVersionVendorDA["SubTotal"],
                    }
                ).ToList();

                foreach (BudgetPlanVersionEntryVM subtotal_ in m_lstVersionEntry)
                    ReturnSubtotal += (decimal)subtotal_.Total;

            }
            else if (m_objDBudgetPlanVersionEntryDA.Success && m_objDBudgetPlanVersionEntryDA.Message.Length > 0)
            {
                return 0;
            }
            else
            {
                message += string.IsNullOrEmpty(m_objDBudgetPlanVersionEntryDA.Message) ? "Empty Vendor Entry" : m_objDBudgetPlanVersionEntryDA.Message;
            }

            ReturnSubtotal = ReturnSubtotal == 0 ? 0 : Math.Floor((decimal)(ReturnSubtotal + (ReturnSubtotal * FeePercentage / 100)) / 1000) * 1000;
            return ReturnSubtotal;
        }
        protected decimal GetRABSubtotal(string BudgetPlanID, decimal FeePercentage, int BudgetPlabVersion)
        {
            decimal ReturnSubtotal = 0;
            List<BudgetPlanVersionStructureVM> m_lstBudgetPlan = new List<BudgetPlanVersionStructureVM>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            DBudgetPlanVersionStructureDA m_objDBudgetPlanVersionStructureDA = new DBudgetPlanVersionStructureDA();
            m_objDBudgetPlanVersionStructureDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersionStructureID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ItemTypeID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Volume.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.MiscAmount.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.WageAmount.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.MaterialAmount.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Sequence.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ParentSequence.MapAlias);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanID);
            m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("BOI");
            m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.ItemTypeID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlabVersion);
            m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicDBudgetPlanVersionStructure = m_objDBudgetPlanVersionStructureDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objDBudgetPlanVersionStructureDA.Message == string.Empty)
            {
                m_lstBudgetPlan = (
                    from DataRow m_dBudgetPlanStructureDA in m_dicDBudgetPlanVersionStructure[0].Tables[0].Rows
                    select new BudgetPlanVersionStructureVM()
                    {
                        BudgetPlanID = m_dBudgetPlanStructureDA[BudgetPlanVersionStructureVM.Prop.BudgetPlanID.Name].ToString(),
                        BudgetPlanVersion = (int)m_dBudgetPlanStructureDA[BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.Name],
                        Volume = (decimal)m_dBudgetPlanStructureDA[BudgetPlanVersionStructureVM.Prop.Volume.Name],
                        MiscAmount = (decimal)m_dBudgetPlanStructureDA[BudgetPlanVersionStructureVM.Prop.MiscAmount.Name],
                        MaterialAmount = (decimal)m_dBudgetPlanStructureDA[BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name],
                        WageAmount = (decimal)m_dBudgetPlanStructureDA[BudgetPlanVersionStructureVM.Prop.WageAmount.Name],
                        Sequence = (int)m_dBudgetPlanStructureDA[BudgetPlanVersionStructureVM.Prop.Sequence.Name],
                        ParentSequence = (int)m_dBudgetPlanStructureDA[BudgetPlanVersionStructureVM.Prop.ParentSequence.Name]
                    }
                ).ToList();


                foreach (BudgetPlanVersionStructureVM item in m_lstBudgetPlan.Where(x => x.ParentSequence == 0))
                {
                    decimal subtotal = 0;
                    if (item.Volume <= 0)
                    {
                        GetChildSubTotal(m_lstBudgetPlan, item.Sequence, ref subtotal);
                    }
                    else
                    {
                        decimal Vol = item.Volume ?? 0;
                        decimal Mat = item.MaterialAmount ?? 0;
                        decimal Wag = item.WageAmount ?? 0;
                        decimal Misc = item.MiscAmount ?? 0;
                        subtotal += (Vol * (Mat + Wag + Misc));
                    }
                    ReturnSubtotal += subtotal;
                }
            }

            ReturnSubtotal = ReturnSubtotal == 0 ? 0 : Math.Floor((decimal)(ReturnSubtotal + (ReturnSubtotal * FeePercentage / 100)) / 1000) * 1000;

            return ReturnSubtotal;
        }
        private void GetChildSubTotal(List<BudgetPlanVersionStructureVM> m_lstBudgetPlan, int parentSequence, ref decimal Total)
        {
            foreach (BudgetPlanVersionStructureVM child in m_lstBudgetPlan.Where(x => x.ParentSequence == parentSequence))
            {
                if (child.Volume > 0)
                {
                    decimal Vol = child.Volume ?? 0;
                    decimal Mat = child.MaterialAmount ?? 0;
                    decimal Wag = child.WageAmount ?? 0;
                    decimal Misc = child.MiscAmount ?? 0;
                    Total += (Vol * (Mat + Wag + Misc));

                }
                else
                    GetChildSubTotal(m_lstBudgetPlan, child.Sequence, ref Total);
            }
        }

        protected Dictionary<bool, string> GetPreviousApproval(string CurrentRoleID, int CurrentLevelApprovalPath)
        {
            string message = "";
            Dictionary<bool, string> Ret = new Dictionary<bool, string>();
            ApprovalPathVM m_objApppathVM = new ApprovalPathVM();
            CApprovalPathDA m_objApppathDA = new CApprovalPathDA();
            m_objApppathDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ApprovalPathVM.Prop.RoleParentID.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();

            int x = 0;
            while (x <= CurrentLevelApprovalPath)
            {
                m_objFilter = new Dictionary<string, List<object>>();
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(CurrentRoleID);
                m_objFilter.Add(ApprovalPathVM.Prop.RoleID.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(General.EnumDesc(Enum.TaskType.NegotiationConfigurations));
                m_objFilter.Add(ApprovalPathVM.Prop.TaskTypeID.Map, m_lstFilter);

                Dictionary<int, DataSet> m_dicMTasksDA = m_objApppathDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
                if (m_objApppathDA.Message == string.Empty)
                {
                    DataRow m_drAppPathDA = m_dicMTasksDA[0].Tables[0].Rows[0];
                    CurrentRoleID = m_drAppPathDA[ApprovalPathVM.Prop.RoleParentID.Name].ToString();
                }
                else
                {
                    message = m_objApppathDA.Message;
                    Ret.Add(false, message);
                    return Ret;
                }
                x++;
            }
            Ret.Add(true, CurrentRoleID);
            return Ret;
        }
        protected string GetParentApproval(ref string message, string CurrentRoleID, string TaskType)
        {
            Dictionary<bool, string> Ret = new Dictionary<bool, string>();
            ApprovalPathVM m_objApppathVM = new ApprovalPathVM();
            CApprovalPathDA m_objApppathDA = new CApprovalPathDA();
            m_objApppathDA.ConnectionStringName = Global.ConnStrConfigName;

            //string UserID = System.Web.HttpContext.Current.User.Identity.Name;            
            CApprovalPathDA apPathDA = new CApprovalPathDA();
            apPathDA.ConnectionStringName = Global.ConnStrConfigName;

            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(TaskType);
            m_objFilter.Add(ApprovalPathVM.Prop.TaskTypeID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(CurrentRoleID);
            m_objFilter.Add(ApprovalPathVM.Prop.RoleID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.LessThanEqual);
            m_lstFilter.Add(DateTime.Now.ToString(Global.SqlDateFormat));
            m_objFilter.Add(ApprovalPathVM.Prop.StartDate.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.GreaterThanEqual);
            m_lstFilter.Add(DateTime.Now.ToString(Global.SqlDateFormat));
            m_objFilter.Add(ApprovalPathVM.Prop.EndDate.Map, m_lstFilter);

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ApprovalPathVM.Prop.RoleParentID.MapAlias);

            //Dictionary<string, OrderDirection> m_dicOrderBy = new Dictionary<string, OrderDirection>();
            //m_dicOrderBy.Add(UserRoleVM.Prop.CreatedDate.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicCAppPathDA = apPathDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null, null);
            string RoleID = "";
            if (string.IsNullOrEmpty(apPathDA.Message))
                RoleID = m_dicCAppPathDA[0].Tables[0].Rows[0][ApprovalPathVM.Prop.RoleParentID.Name].ToString();
            else
            {
                message = "Appropal Path Error: " + apPathDA.Message;
                return "";
            }
            return RoleID;
        }
        protected string GetChildApproval(ref string message, string CurrentRoleID, string TaskType)
        {
            Dictionary<bool, string> Ret = new Dictionary<bool, string>();
            ApprovalPathVM m_objApppathVM = new ApprovalPathVM();
            CApprovalPathDA m_objApppathDA = new CApprovalPathDA();
            m_objApppathDA.ConnectionStringName = Global.ConnStrConfigName;

            //string UserID = System.Web.HttpContext.Current.User.Identity.Name;            
            CApprovalPathDA apPathDA = new CApprovalPathDA();
            apPathDA.ConnectionStringName = Global.ConnStrConfigName;

            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(TaskType);
            m_objFilter.Add(ApprovalPathVM.Prop.TaskTypeID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(CurrentRoleID);
            m_objFilter.Add(ApprovalPathVM.Prop.RoleID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.LessThanEqual);
            m_lstFilter.Add(DateTime.Now.ToString(Global.SqlDateFormat));
            m_objFilter.Add(ApprovalPathVM.Prop.StartDate.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.GreaterThanEqual);
            m_lstFilter.Add(DateTime.Now.ToString(Global.SqlDateFormat));
            m_objFilter.Add(ApprovalPathVM.Prop.EndDate.Map, m_lstFilter);

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ApprovalPathVM.Prop.RoleChildID.MapAlias);

            //Dictionary<string, OrderDirection> m_dicOrderBy = new Dictionary<string, OrderDirection>();
            //m_dicOrderBy.Add(UserRoleVM.Prop.CreatedDate.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicCAppPathDA = apPathDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null, null);
            string RoleID = "";
            if (string.IsNullOrEmpty(apPathDA.Message))
                RoleID = m_dicCAppPathDA[0].Tables[0].Rows[0][ApprovalPathVM.Prop.RoleChildID.Name].ToString();
            else
            {
                message = "Appropal Path Error: " + apPathDA.Message;
                return "";
            }
            return RoleID;
        }
        protected string FormatDateReport(DateTime dateTime, string lang)
        {
            string m_strRetval = string.Empty;
            string m_suffix = (dateTime.Day % 10 == 1 && dateTime.Day != 11) ? "st" : (dateTime.Day % 10 == 2 && dateTime.Day != 12) ? "nd" : (dateTime.Day % 10 == 3 && dateTime.Day != 13) ? "rd" : "th";
            string[] m_bulan = { "Januari", "Februari", "Maret", "April", "Mei", "Juni", "Juli", "Agustus", "September", "Oktober", "Nopember", "Desember" };
            switch (lang)
            {
                case "ID":
                    m_strRetval = dateTime.ToString("dd") + " " + m_bulan[dateTime.Month - 1] + " " + dateTime.ToString("yyyy");
                    break;
                case "EN":
                    m_strRetval = dateTime.ToString("MMMM dd") + m_suffix + " " + dateTime.ToString("yyyy");
                    break;
                default:
                    break;
            }

            return m_strRetval;
        }
        protected List<ConfigVM> GetConfig(string key1, string key2 = null, string key3 = null, string key4 = null)
        {
            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ConfigVM.Prop.Key1.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Key2.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Key3.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Key4.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc1.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc2.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc3.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc4.MapAlias);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(key1);
            m_objFilter.Add(ConfigVM.Prop.Key1.Map, m_lstFilter);

            //if (!string.IsNullOrEmpty(key2)){
            //    m_lstFilter = new List<object>();
            //    m_lstFilter.Add(Operator.Equals);
            //    m_lstFilter.Add(key2);
            //    m_objFilter.Add(ConfigVM.Prop.Key2.Map, m_lstFilter);
            //}
            if (!string.IsNullOrEmpty(key2))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(key2);
                m_objFilter.Add(ConfigVM.Prop.Key2.Map, m_lstFilter);
            }
            if (!string.IsNullOrEmpty(key3))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(key3);
                m_objFilter.Add(ConfigVM.Prop.Key3.Map, m_lstFilter);
            }
            if (!string.IsNullOrEmpty(key4))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(key4);
                m_objFilter.Add(ConfigVM.Prop.Key4.Map, m_lstFilter);
            }


            Dictionary<int, DataSet> m_dicUConfigDA = m_objUConfigDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            List<ConfigVM> m_lstConfigVM = new List<ConfigVM>();
            if (m_objUConfigDA.Message == string.Empty)
            {
                m_lstConfigVM = (
                        from DataRow m_drUConfigDA in m_dicUConfigDA[0].Tables[0].Rows
                        select new ConfigVM()
                        {
                            Key1 = m_drUConfigDA[ConfigVM.Prop.Key1.Name].ToString(),
                            Key2 = m_drUConfigDA[ConfigVM.Prop.Key2.Name].ToString(),
                            Key3 = m_drUConfigDA[ConfigVM.Prop.Key3.Name].ToString(),
                            Key4 = m_drUConfigDA[ConfigVM.Prop.Key4.Name].ToString(),
                            Desc1 = m_drUConfigDA[ConfigVM.Prop.Desc1.Name].ToString(),
                            Desc2 = m_drUConfigDA[ConfigVM.Prop.Desc2.Name].ToString(),
                            Desc3 = m_drUConfigDA[ConfigVM.Prop.Desc3.Name].ToString(),
                            Desc4 = m_drUConfigDA[ConfigVM.Prop.Desc4.Name].ToString()
                        }
                    ).ToList();
            }
            return m_lstConfigVM;
        }
        protected string GetSOAPString(string SOAPEnv, string URL, ICredentials credential = null)
        {
            HttpWebRequest m_webrequest = (HttpWebRequest)WebRequest.Create(URL);
            //m_webrequest.ContentType = "\"text/xml; charset=\"utf-8\"";
            //m_webrequest.Accept = "text/xml";
            m_webrequest.Accept = "gzip,deflate";
            m_webrequest.ContentType = "text/xml; charset=utf-8";
            m_webrequest.Method = "POST";
            m_webrequest.Credentials = credential;

            using (Stream stm = m_webrequest.GetRequestStream())
            {
                using (StreamWriter stmw = new StreamWriter(stm))
                {
                    stmw.Write(SOAPEnv);
                }
            }
            HttpWebResponse m_webresponse = null;
            try
            {
                m_webresponse = m_webrequest.GetResponse() as HttpWebResponse;
            }
            catch (Exception e)
            {
                return string.Empty;
            }

            StreamReader reader = new StreamReader(m_webresponse.GetResponseStream());
            var msgstr = reader.ReadToEnd();
            reader.Close();
            return msgstr;
        }
        protected List<NegotiationConfigurationsVM> GetListNegoConfiguration(string FPTID, string NegoConfigTypeID, ref string message)
        {

            List<NegotiationConfigurationsVM> m_lstNegotiationConfigurationsVM = new List<NegotiationConfigurationsVM>();
            CNegotiationConfigurationsDA m_objCNegotiationConfigurationsDA = new CNegotiationConfigurationsDA();
            m_objCNegotiationConfigurationsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.NegotiationConfigID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.NegotiationConfigTypeID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.ParameterValue.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.Descriptions.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.TaskID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.CurrentApprovalLvl.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.FPTID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.StatusID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.TCMemberID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.TCTypeID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.EmployeeID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.FirstName.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.ProjectID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.TCTypeDesc.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.ProjectDesc.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.FPTStatusID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.BusinessUnitDesc.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.ParameterValue2.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.NegoBUnitDesc.MapAlias);
            m_lstSelect.Add("[TCLeadName].FullName AS [TCLeadName]");
            m_lstSelect.Add("[MEmployee].FirstName+' '+[MEmployee].MiddleName+' '+[MEmployee].LastName AS [TRMLeadDesc]");

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            if (!string.IsNullOrEmpty(FPTID))
            {
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(FPTID);
                m_objFilter.Add(NegotiationConfigurationsVM.Prop.FPTID.Map, m_lstFilter);

            }

            m_lstFilter = new List<object>();
            if (!string.IsNullOrEmpty(NegoConfigTypeID))
            {
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(NegoConfigTypeID);
                m_objFilter.Add(NegotiationConfigurationsVM.Prop.NegotiationConfigTypeID.Map, m_lstFilter);

            }

            Dictionary<string, OrderDirection> m_dicOrderBy = new Dictionary<string, OrderDirection>();
            m_dicOrderBy.Add(NegotiationConfigurationsVM.Prop.NegotiationConfigID.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicNegotiationConfigurationsDA = m_objCNegotiationConfigurationsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, m_dicOrderBy);
            if (m_objCNegotiationConfigurationsDA.Success)
            {
                m_lstNegotiationConfigurationsVM =
                (from DataRow m_drNegotiationConfigurationsDA in m_dicNegotiationConfigurationsDA[0].Tables[0].Rows
                 select new NegotiationConfigurationsVM()
                 {
                     NegotiationConfigID = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.NegotiationConfigID.Name].ToString(),
                     NegotiationConfigTypeID = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.NegotiationConfigTypeID.Name].ToString(),
                     Descriptions = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.Descriptions.Name].ToString(),
                     FPTID = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.FPTID.Name].ToString(),
                     TaskID = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.TaskID.Name].ToString(),
                     StatusID = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.StatusID.Name].ToString(),
                     FPTStatusID = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.FPTStatusID.Name].ToString(),
                     CurrentApprovalLvl = int.Parse(m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.CurrentApprovalLvl.Name].ToString()),
                     ParameterValue = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.ParameterValue.Name].ToString(),
                     TCTypeID = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.TCTypeID.Name].ToString(),
                     TCMemberID = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.TCMemberID.Name].ToString(),
                     TCTypeDesc = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.TCTypeDesc.Name].ToString(),
                     EmployeeID = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.EmployeeID.Name].ToString(),
                     VendorID = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.VendorID.Name].ToString(),
                     BusinessUnitDesc = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.BusinessUnitDesc.Name].ToString(),
                     FirstName = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.FirstName.Name].ToString(),
                     BudgetPlanID = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.BudgetPlanID.Name].ToString(),
                     ProjectID = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.ProjectID.Name].ToString(),
                     NegoBUnitDesc = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.NegoBUnitDesc.Name].ToString(),
                     ProjectDesc = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.ProjectDesc.Name].ToString(),
                     ParameterValue2 = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.ParameterValue2.Name].ToString(),
                     TRMLeadDesc = string.IsNullOrEmpty(m_drNegotiationConfigurationsDA["TCLeadName"].ToString()) ? "" : m_drNegotiationConfigurationsDA["TCLeadName"].ToString(),
                     EmployeeName = string.IsNullOrEmpty(m_drNegotiationConfigurationsDA["TRMLeadDesc"].ToString()) ? "" : m_drNegotiationConfigurationsDA["TRMLeadDesc"].ToString()

                 }).ToList();
            }
            else
                message = m_objCNegotiationConfigurationsDA.Message;

            return m_lstNegotiationConfigurationsVM;

        }


        protected List<RoleFunctionVM> GetListRoleFunctionVM(List<string> ListRoleID, ref string message)
        {

            List<RoleFunctionVM> m_lstRoleFunctionVM = new List<RoleFunctionVM>();
            DRoleFunctionDA m_objDRoleFunctionDA = new DRoleFunctionDA();
            m_objDRoleFunctionDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(RoleFunctionVM.Prop.RoleID.MapAlias);
            m_lstSelect.Add(RoleFunctionVM.Prop.FunctionID.MapAlias);
            m_lstSelect.Add(RoleFunctionVM.Prop.FunctionDesc.MapAlias);


            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            //m_lstFilter.Add(Operator.Equals);
            //m_lstFilter.Add(RoleID);
            //m_objFilter.Add(RoleFunctionVM.Prop.RoleID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(string.Join(",", ListRoleID));
            m_objFilter.Add(RoleFunctionVM.Prop.RoleID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicDRoleFunctionDA = m_objDRoleFunctionDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDRoleFunctionDA.Success)
            {
                m_lstRoleFunctionVM =
                (from DataRow m_drDRoleFunctionDA in m_dicDRoleFunctionDA[0].Tables[0].Rows
                 select new RoleFunctionVM()
                 {
                     RoleID = m_drDRoleFunctionDA[RoleFunctionVM.Prop.RoleID.Name].ToString(),
                     FunctionID = m_drDRoleFunctionDA[RoleFunctionVM.Prop.FunctionID.Name].ToString(),
                     FunctionDesc = m_drDRoleFunctionDA[RoleFunctionVM.Prop.FunctionDesc.Name].ToString(),


                 }).ToList();
            }
            else
                message = m_objDRoleFunctionDA.Message;

            return m_lstRoleFunctionVM;

        }



        protected bool CreateSchedule(MSchedules model, object m_objDBConnection, ref List<string> Message)
        {
            List<string> m_lstMessage = new List<string>();
            MSchedulesDA m_objMSchedulesDA = new MSchedulesDA();
            m_objMSchedulesDA.ConnectionStringName = Global.ConnStrConfigName;
            string m_strTransName = "MSchedules";
            m_objDBConnection = m_objMSchedulesDA.BeginTrans(m_strTransName);//todo:
            m_objMSchedulesDA.Data = model;
            m_objMSchedulesDA.Insert(true, m_objDBConnection);
            if (!m_objMSchedulesDA.Success || m_objMSchedulesDA.Message != string.Empty)
            {
                m_objMSchedulesDA.EndTrans(ref m_objDBConnection, m_strTransName);
                Message.Add(m_objMSchedulesDA.Message);
                return false;
            }
            m_objMSchedulesDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
            return true;
        }
        protected UserVM GetUserID(bool IsVendor, string Id)
        {
            string m_strMessage = string.Empty;

            UserVM m_objUserVM = new UserVM();

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();

            MUserDA m_objMUserDA = new MUserDA();
            m_objMUserDA.ConnectionStringName = Global.ConnStrConfigName;

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(Id);

            if (IsVendor)
            {
                m_objFilter.Add(UserVM.Prop.VendorID.Map, m_lstFilter);
            }
            else
            {
                m_objFilter.Add(UserVM.Prop.EmployeeID.Map, m_lstFilter);
            }


            m_lstSelect = new List<string>();
            m_lstSelect.Add(UserVM.Prop.UserID.MapAlias);
            m_lstSelect.Add(UserVM.Prop.FullName.MapAlias);
            m_lstSelect.Add(UserVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(UserVM.Prop.VendorDesc.MapAlias);
            m_lstSelect.Add(UserVM.Prop.EmployeeID.MapAlias);
            m_lstSelect.Add(UserVM.Prop.BusinessUnitID.MapAlias);
            m_lstSelect.Add(UserVM.Prop.DivisionID.MapAlias);
            m_lstSelect.Add(UserVM.Prop.ProjectID.MapAlias);
            m_lstSelect.Add(UserVM.Prop.ClusterID.MapAlias);

            Dictionary<int, DataSet> m_dicMUserDA = m_objMUserDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objMUserDA.Success & m_dicMUserDA[0].Tables[0].Rows.Count > 0)
            {
                m_objUserVM.UserID = m_dicMUserDA[0].Tables[0].Rows[0][UserVM.Prop.UserID.Name].ToString();
                m_objUserVM.FullName = m_dicMUserDA[0].Tables[0].Rows[0][UserVM.Prop.FullName.Name].ToString();
                m_objUserVM.VendorID = m_dicMUserDA[0].Tables[0].Rows[0][UserVM.Prop.VendorID.Name].ToString();
                m_objUserVM.VendorDesc = m_dicMUserDA[0].Tables[0].Rows[0][UserVM.Prop.VendorDesc.Name].ToString();
                m_objUserVM.EmployeeID = m_dicMUserDA[0].Tables[0].Rows[0][UserVM.Prop.EmployeeID.Name].ToString();
                m_objUserVM.BusinessUnitID = m_dicMUserDA[0].Tables[0].Rows[0][UserVM.Prop.BusinessUnitID.Name].ToString();
                m_objUserVM.DivisionID = m_dicMUserDA[0].Tables[0].Rows[0][UserVM.Prop.DivisionID.Name].ToString();
                m_objUserVM.ProjectID = m_dicMUserDA[0].Tables[0].Rows[0][UserVM.Prop.ProjectID.Name].ToString();
                m_objUserVM.ClusterID = m_dicMUserDA[0].Tables[0].Rows[0][UserVM.Prop.ClusterID.Name].ToString();

            }

            return m_objUserVM;
        }
        protected bool CreateMailNotification(MailNotificationsVM Model, bool AutoSend, ref List<string> Message, ref string MailNotifID)
        {
            //todo: is save valid

            object m_objDBConnection = null;
            //validation Model
            if (Model.NotificationMapVM == null)
            {
                Message.Add("No default template found!");
                return false;
            }
            //if (!Model.RecipientsVM.Any())
            //{
            //    Message.Add("No recipient found!");
            //    return false;
            //}

            MMailNotificationsDA m_objMMailNotificationsDA = new MMailNotificationsDA();
            DRecipientsDA m_objDRecipientsDA = new DRecipientsDA();
            TNotificationValuesDA m_objTNotificationValuesDA = new TNotificationValuesDA();
            TNotificationAttachmentsDA m_objTNotificationAttachmentsDA = new TNotificationAttachmentsDA();

            NotificationTemplateVM m_NotificationTemplateVM = GetNotificationTemplateVM(Model.NotificationMapVM.NotificationTemplateID);
            m_objMMailNotificationsDA.ConnectionStringName = Global.ConnStrConfigName;
            string m_strTransName = "MailNotifications";
            m_objDBConnection = m_objMMailNotificationsDA.BeginTrans(m_strTransName);

            var m_listTag = new Dictionary<string, string>();
            if (Model.NotificationValuesVM != null)
            {
                foreach (var item in Model.NotificationValuesVM)
                {
                    //m_listTag.Add("Stardate", "01/01/2014");
                    m_listTag.Add(item.FieldTagID, item.Value);
                }
            }


            //Insert MMailNotifications
            string str_MailNotificationID = Guid.NewGuid().ToString("N");
            MailNotifID = str_MailNotificationID;
            m_objMMailNotificationsDA.ConnectionStringName = Global.ConnStrConfigName;
            MMailNotifications m_MMailNotifications = new MMailNotifications();
            m_MMailNotifications.MailNotificationID = str_MailNotificationID;
            m_MMailNotifications.Importance = Model.Importance;
            //m_MMailNotifications.NotifMapID = Model.NotificationMapVM.NotifMapID;
            m_MMailNotifications.FunctionID = Model.NotificationMapVM.FunctionID;
            m_MMailNotifications.NotificationTemplateID = Model.NotificationMapVM.NotificationTemplateID;
            m_MMailNotifications.Subject = Model.Subject;
            m_MMailNotifications.Contents = Global.ParseParameter(m_NotificationTemplateVM.Contents, m_listTag);
            m_MMailNotifications.StatusID = AutoSend ? (int)NotificationStatus.Sent : (int)NotificationStatus.Draft;
            m_MMailNotifications.TaskID = Model.TaskID;
            m_MMailNotifications.FPTID = Model.FPTID;

            Model.MailNotificationID = str_MailNotificationID;
            Model.Contents = m_MMailNotifications.Contents;

            m_objMMailNotificationsDA.Data = m_MMailNotifications;
            m_objMMailNotificationsDA.Insert(true, m_objDBConnection);
            if (!m_objMMailNotificationsDA.Success || m_objMMailNotificationsDA.Message != string.Empty)
            {
                m_objMMailNotificationsDA.EndTrans(ref m_objDBConnection, m_strTransName);
                Message.Add(m_objMMailNotificationsDA.Message);
                return false;
            }

            //Insert NotificationValuesVM
            m_objTNotificationValuesDA.ConnectionStringName = Global.ConnStrConfigName;
            if (Model.NotificationValuesVM != null)
            {
                foreach (var item in Model.NotificationValuesVM)
                {
                    TNotificationValues m_TNotificationValues = new TNotificationValues();
                    m_TNotificationValues.NotificationValueID = item.NotificationValueID;
                    m_TNotificationValues.MailNotificationID = str_MailNotificationID;
                    m_TNotificationValues.FieldTagID = item.FieldTagID;
                    m_TNotificationValues.Value = item.Value;


                    m_objTNotificationValuesDA.Data = m_TNotificationValues;
                    m_objTNotificationValuesDA.Insert(true, m_objDBConnection);
                    if (!m_objTNotificationValuesDA.Success || m_objTNotificationValuesDA.Message != string.Empty)
                    {
                        m_objTNotificationValuesDA.EndTrans(ref m_objDBConnection, m_strTransName);
                        Message.Add(m_objTNotificationValuesDA.Message);
                        return false;
                    }
                }
            }

            //Insert DRecipients
            m_objDRecipientsDA.ConnectionStringName = Global.ConnStrConfigName;
            foreach (var item in Model.RecipientsVM)
            {
                DRecipients m_DRecipients = new DRecipients();
                m_DRecipients.RecipientID = Guid.NewGuid().ToString("N");
                m_DRecipients.RecipientDesc = item.RecipientDesc;
                m_DRecipients.MailAddress = item.MailAddress;
                m_DRecipients.OwnerID = item.OwnerID;
                m_DRecipients.RecipientTypeID = item.RecipientTypeID;
                m_DRecipients.MailNotificationID = str_MailNotificationID;

                m_objDRecipientsDA.Data = m_DRecipients;
                m_objDRecipientsDA.Insert(true, m_objDBConnection);
                if (!m_objDRecipientsDA.Success || m_objDRecipientsDA.Message != string.Empty)
                {
                    m_objDRecipientsDA.EndTrans(ref m_objDBConnection, m_strTransName);
                    Message.Add(m_objDRecipientsDA.Message);
                    return false;
                }
            }

            //Insert TNotificationAttachments
            m_objTNotificationAttachmentsDA.ConnectionStringName = Global.ConnStrConfigName;
            if (Model.NotificationAttachmentVM != null)
            {
                foreach (var item in Model.NotificationAttachmentVM)
                {
                    TNotificationAttachments m_TNotificationAttachments = new TNotificationAttachments();
                    m_TNotificationAttachments.AttachmentValueID = "";
                    m_TNotificationAttachments.Filename = item.Filename;
                    m_TNotificationAttachments.ContentType = item.ContentType;
                    m_TNotificationAttachments.RawData = item.RawData;
                    m_TNotificationAttachments.MailNotificationID = str_MailNotificationID;

                    m_objTNotificationAttachmentsDA.Data = m_TNotificationAttachments;
                    m_objTNotificationAttachmentsDA.Insert(true, m_objDBConnection);
                    if (!m_objTNotificationAttachmentsDA.Success || m_objTNotificationAttachmentsDA.Message != string.Empty)
                    {
                        m_objTNotificationAttachmentsDA.EndTrans(ref m_objDBConnection, m_strTransName);
                        Message.Add(m_objTNotificationAttachmentsDA.Message);
                        return false;
                    }
                }
            }
            if (AutoSend)
            {
                if (SendMail(Model, ref Message, ref m_objDBConnection))
                {
                    m_objTNotificationAttachmentsDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
                    return true;
                }
                else
                {
                    m_objTNotificationAttachmentsDA.EndTrans(ref m_objDBConnection, m_strTransName);
                    return false;
                }
            }
            m_objTNotificationAttachmentsDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
            return true;

        }
        protected NotificationTemplateVM GetNotificationTemplateVM(string NotificationTemplateID)
        {
            MNotificationTemplatesDA m_objMNotificationTemplatesDA = new MNotificationTemplatesDA();
            m_objMNotificationTemplatesDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(NotificationTemplateVM.Prop.NotificationTemplateID.MapAlias);
            m_lstSelect.Add(NotificationTemplateVM.Prop.NotificationTemplateDesc.MapAlias);
            m_lstSelect.Add(NotificationTemplateVM.Prop.Contents.MapAlias);


            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(NotificationTemplateID);
            m_objFilter.Add(NotificationTemplateVM.Prop.NotificationTemplateID.Map, m_lstFilter);


            List<NotificationTemplateVM> m_lstNotificationTemplateVM = new List<NotificationTemplateVM>();
            Dictionary<int, DataSet> m_dicNotificationTemplate = m_objMNotificationTemplatesDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);
            if (m_objMNotificationTemplatesDA.Message == string.Empty)
            {
                m_lstNotificationTemplateVM = (
                  from DataRow m_drDNotificationMap in m_dicNotificationTemplate[0].Tables[0].Rows
                  select new NotificationTemplateVM
                  {
                      NotificationTemplateID = m_drDNotificationMap[NotificationTemplateVM.Prop.NotificationTemplateID.Name].ToString(),
                      NotificationTemplateDesc = m_drDNotificationMap[NotificationTemplateVM.Prop.NotificationTemplateDesc.Name].ToString(),
                      Contents = m_drDNotificationMap[NotificationTemplateVM.Prop.Contents.Name].ToString(),

                  }
                ).ToList();
            }
            NotificationTemplateVM m_objNotificationTemplateVM = m_lstNotificationTemplateVM.FirstOrDefault();
            return m_objNotificationTemplateVM;
        }

        protected MinutesTemplateVM GetMinutesTemplateVM(string MinuteTemplateID)
        {
            MMinuteTemplatesDA m_objMMinuteTemplatesDA = new MMinuteTemplatesDA();
            m_objMMinuteTemplatesDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(MinutesTemplateVM.Prop.MinuteTemplateID.MapAlias);
            m_lstSelect.Add(MinutesTemplateVM.Prop.MinuteTemplateDescriptions.MapAlias);
            m_lstSelect.Add(MinutesTemplateVM.Prop.Contents.MapAlias);


            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(MinuteTemplateID);
            m_objFilter.Add(MinutesTemplateVM.Prop.MinuteTemplateID.Map, m_lstFilter);


            List<MinutesTemplateVM> m_lstMinutesTemplateVM = new List<MinutesTemplateVM>();
            Dictionary<int, DataSet> m_dicMMinuteTemplatesDA = m_objMMinuteTemplatesDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null, null);
            if (m_objMMinuteTemplatesDA.Message == string.Empty)
            {
                m_lstMinutesTemplateVM = (
                  from DataRow m_drDNotificationMap in m_dicMMinuteTemplatesDA[0].Tables[0].Rows
                  select new MinutesTemplateVM
                  {
                      MinuteTemplateID = m_drDNotificationMap[MinutesTemplateVM.Prop.MinuteTemplateID.Name].ToString(),
                      MinuteTemplateDescriptions = m_drDNotificationMap[MinutesTemplateVM.Prop.MinuteTemplateDescriptions.Name].ToString(),
                      Contents = m_drDNotificationMap[MinutesTemplateVM.Prop.Contents.Name].ToString(),

                  }
                ).ToList();
            }
            MinutesTemplateVM m_objMinutesTemplateVM = m_lstMinutesTemplateVM.FirstOrDefault();
            return m_objMinutesTemplateVM;
        }

        protected List<RecipientsVM> GetListRecipientsVM(string MailNotificationID, ref string message)
        {
            List<RecipientsVM> m_lstRecipientsVM = new List<RecipientsVM>();
            DRecipientsDA m_objDRecipientsDA = new DRecipientsDA();
            m_objDRecipientsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(RecipientsVM.Prop.RecipientID.MapAlias);
            m_lstSelect.Add(RecipientsVM.Prop.RecipientDesc.MapAlias);
            m_lstSelect.Add(RecipientsVM.Prop.MailAddress.MapAlias);
            m_lstSelect.Add(RecipientsVM.Prop.OwnerID.MapAlias);
            m_lstSelect.Add(RecipientsVM.Prop.RecipientNRK.MapAlias);
            m_lstSelect.Add(RecipientsVM.Prop.RecipientName.MapAlias);
            m_lstSelect.Add(RecipientsVM.Prop.RecipientTypeID.MapAlias);
            m_lstSelect.Add(RecipientsVM.Prop.RecipientTypeDesc.MapAlias);
            m_lstSelect.Add(RecipientsVM.Prop.MailNotificationID.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(MailNotificationID);
            m_objFilter.Add(RecipientsVM.Prop.MailNotificationID.Map, m_lstFilter);


            Dictionary<int, DataSet> m_dicDRecipientsDA = m_objDRecipientsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDRecipientsDA.Success)
            {
                foreach (DataRow m_drDRecipientsDA in m_dicDRecipientsDA[0].Tables[0].Rows)
                {
                    RecipientsVM m_objRecipientsVM = new RecipientsVM();
                    m_objRecipientsVM.RecipientID = m_drDRecipientsDA[RecipientsVM.Prop.RecipientID.Name].ToString();
                    m_objRecipientsVM.RecipientDesc = m_drDRecipientsDA[RecipientsVM.Prop.RecipientDesc.Name].ToString();
                    m_objRecipientsVM.MailAddress = m_drDRecipientsDA[RecipientsVM.Prop.MailAddress.Name].ToString();
                    m_objRecipientsVM.OwnerID = m_drDRecipientsDA[RecipientsVM.Prop.OwnerID.Name].ToString();
                    m_objRecipientsVM.RecipientNRK = m_drDRecipientsDA[RecipientsVM.Prop.RecipientNRK.Name].ToString();
                    m_objRecipientsVM.RecipientName = m_drDRecipientsDA[RecipientsVM.Prop.RecipientName.Name].ToString();
                    m_objRecipientsVM.RecipientTypeID = m_drDRecipientsDA[RecipientsVM.Prop.RecipientTypeID.Name].ToString();
                    m_objRecipientsVM.RecipientTypeDesc = m_drDRecipientsDA[RecipientsVM.Prop.RecipientTypeDesc.Name].ToString();
                    m_objRecipientsVM.MailNotificationID = m_drDRecipientsDA[RecipientsVM.Prop.MailNotificationID.Name].ToString();

                    m_lstRecipientsVM.Add(m_objRecipientsVM);
                }
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDRecipientsDA.Message;

            return m_lstRecipientsVM;

        }
        protected List<NotificationAttachmentVM> GetListNotificationAttachmentVM(string MailNotificationID, ref string message)
        {
            List<NotificationAttachmentVM> m_lstNotificationAttachmentVM = new List<NotificationAttachmentVM>();
            TNotificationAttachmentsDA m_objTNotificationAttachmentsDA = new TNotificationAttachmentsDA();
            m_objTNotificationAttachmentsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(NotificationAttachmentVM.Prop.AttachmentValueID.MapAlias);
            m_lstSelect.Add(NotificationAttachmentVM.Prop.Filename.MapAlias);
            m_lstSelect.Add(NotificationAttachmentVM.Prop.ContentType.MapAlias);
            m_lstSelect.Add(NotificationAttachmentVM.Prop.MailNotificationID.MapAlias);
            m_lstSelect.Add(NotificationAttachmentVM.Prop.RawData.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(MailNotificationID);
            m_objFilter.Add(NotificationAttachmentVM.Prop.MailNotificationID.Map, m_lstFilter);


            Dictionary<int, DataSet> m_dicDRecipientsDA = m_objTNotificationAttachmentsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objTNotificationAttachmentsDA.Success)
            {
                foreach (DataRow m_drDRecipientsDA in m_dicDRecipientsDA[0].Tables[0].Rows)
                {
                    NotificationAttachmentVM m_objNotificationAttachmentVM = new NotificationAttachmentVM();
                    m_objNotificationAttachmentVM.AttachmentValueID = m_drDRecipientsDA[NotificationAttachmentVM.Prop.AttachmentValueID.Name].ToString();
                    m_objNotificationAttachmentVM.Filename = m_drDRecipientsDA[NotificationAttachmentVM.Prop.Filename.Name].ToString();
                    m_objNotificationAttachmentVM.ContentType = m_drDRecipientsDA[NotificationAttachmentVM.Prop.ContentType.Name].ToString();
                    m_objNotificationAttachmentVM.MailNotificationID = m_drDRecipientsDA[NotificationAttachmentVM.Prop.MailNotificationID.Name].ToString();
                    m_objNotificationAttachmentVM.RawData = m_drDRecipientsDA[NotificationAttachmentVM.Prop.RawData.Name].ToString();
                    m_lstNotificationAttachmentVM.Add(m_objNotificationAttachmentVM);
                }
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objTNotificationAttachmentsDA.Message;

            return m_lstNotificationAttachmentVM;

        }
        protected void LogFile(string LogText)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(Server.MapPath($"/Log{DateTime.Now.ToString("yyyy-dd-M")}.txt"), true))
                {
                    sw.WriteLine($"{DateTime.Now} : {LogText}");
                }
            }
            catch (Exception)
            {

            }
        }

        #endregion
        #region Invitation
        protected bool SendMail(MailNotificationsVM Model, ref List<string> message, ref Object m_objDBConnection)
        {
            //MailNotificationsVM m_MailNotificationsVM = GetMailNotificationsVM(TaskID, ref message);
            var m_lstconfig = GetConfig("Mail");
            if (!m_lstconfig.Any())
            {
                message.Add("No Mail configuration found!");
                return false;
            }
            string m_strfrom = string.Empty;
            string m_strreply = string.Empty;
            string m_strnotif = string.Empty;

            m_strfrom = m_lstconfig.Where(x => x.Key2 == "From").FirstOrDefault().Desc1;
            m_strreply = m_lstconfig.Where(x => x.Key2 == "Reply").FirstOrDefault().Desc1;
            m_strnotif = m_lstconfig.Where(x => x.Key2 == "Notification").FirstOrDefault().Desc1;

            mailws.MailSender ms = new mailws.MailSender();
            string m_to = string.Join(";", Model.RecipientsVM.Where(x => x.RecipientTypeID == ((int)Enum.RecipientTypes.TO).ToString()).Select(y => y.MailAddress));
            string m_cc = string.Join(";", Model.RecipientsVM.Where(x => x.RecipientTypeID == ((int)Enum.RecipientTypes.CC).ToString()).Select(y => y.MailAddress));
            string m_bcc = string.Join(";", Model.RecipientsVM.Where(x => x.RecipientTypeID == ((int)Enum.RecipientTypes.BCC).ToString()).Select(y => y.MailAddress));
            List<mailws.MailAttachment> attachments = new List<mailws.MailAttachment>();
            if (Model.NotificationAttachmentVM.Count > 0)
            {
                foreach (var item in Model.NotificationAttachmentVM)
                {
                    mailws.MailAttachment m_att = new mailws.MailAttachment();
                    m_att.Content = Convert.FromBase64String(item.RawData);
                    m_att.FileName = item.Filename + item.ContentType;
                    attachments.Add(m_att);
                }
            }
            string m_result = string.Empty;
            try
            {
                m_result = ms.SendMailDelivery(m_strfrom, m_strreply, m_strnotif, m_to, m_cc, m_bcc, Model.Subject, Model.Contents, attachments.ToArray(), true);
            }
            catch (Exception e)
            {
                m_result = e.Message;
            }


            //Save Histories
            TMailHistoriesDA m_objTMailHistoriesDA = new TMailHistoriesDA();
            m_objTMailHistoriesDA.ConnectionStringName = Global.ConnStrConfigName;
            TMailHistories m_objTMailHistories = new TMailHistories();

            m_objTMailHistories.MailHistoryID = Guid.NewGuid().ToString().Replace("-", "");
            m_objTMailHistories.StatusDate = DateTime.Now;
            m_objTMailHistories.To = m_to;
            m_objTMailHistories.CC = m_cc;
            m_objTMailHistories.BCC = m_bcc;
            m_objTMailHistories.Subject = Model.Subject;
            m_objTMailHistories.Content = Model.Contents;
            m_objTMailHistories.MailNotificationID = Model.MailNotificationID;
            m_objTMailHistories.StatusID = (m_result == "Success") ? ((int)NotificationStatus.Sent).ToString() : ((int)NotificationStatus.Fail).ToString();

            m_objTMailHistoriesDA.Data = m_objTMailHistories;

            m_objTMailHistoriesDA.Insert(m_objDBConnection != null, m_objDBConnection);


            if (!m_objTMailHistoriesDA.Success || m_objTMailHistoriesDA.Message != string.Empty)
            {
                message.Add(m_objTMailHistoriesDA.Message);
                return false;
            }
            //Update Mailnotif Status
            MMailNotificationsDA m_objMMailNotificationsDA = new MMailNotificationsDA();
            m_objMMailNotificationsDA.ConnectionStringName = Global.ConnStrConfigName;

            Dictionary<string, List<object>> m_dicSet = new Dictionary<string, List<object>>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();

            if (!string.IsNullOrEmpty(Model.TaskID))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(Model.TaskID);
                m_objFilter.Add(MailNotificationsVM.Prop.TaskID.Map, m_lstFilter);
            }
            else {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(Model.MailNotificationID);
                m_objFilter.Add(MailNotificationsVM.Prop.MailNotificationID.Map, m_lstFilter);

            }

            int m_statusid = (m_result == "Success") ? ((int)NotificationStatus.Sent) : ((int)NotificationStatus.Fail);
            List<object> m_lstSet = new List<object>();
            m_lstSet.Add(typeof(int));
            m_lstSet.Add(m_statusid);
            m_dicSet.Add(MailNotificationsVM.Prop.StatusID.Map, m_lstSet);
            m_objMMailNotificationsDA.UpdateBC(m_dicSet, m_objFilter, m_objDBConnection != null, m_objDBConnection);


            if (!m_objMMailNotificationsDA.Success || m_objMMailNotificationsDA.Message != string.Empty)
            {
                message.Add(m_objMMailNotificationsDA.Message);
                return false;
            }


            return true;

        }
        public bool SendMailScheduller(string Subject, string To, string Content, ref string result)
        {

            var m_lstconfig = GetConfig("Mail");
            if (!m_lstconfig.Any())
            {
                result = "No Mail configuration found!";
                return false;
            }
            string m_strfrom = string.Empty;
            string m_strreply = string.Empty;
            string m_strnotif = string.Empty;

            m_strfrom = m_lstconfig.Where(x => x.Key2 == "From").FirstOrDefault().Desc1;
            m_strreply = m_lstconfig.Where(x => x.Key2 == "Reply").FirstOrDefault().Desc1;
            m_strnotif = m_lstconfig.Where(x => x.Key2 == "Notification").FirstOrDefault().Desc1;

            mailws.MailSender ms = new mailws.MailSender();
            result = ms.SendMailDelivery(m_strfrom, m_strreply, m_strnotif, To, null, null, Subject, Content, new mailws.MailAttachment[] { }, true);
            return true;

        }
        protected bool AutoEmailActive()
        {
            bool m_retval = false;
            List<ConfigVM> m_lstconfig = GetConfig("AutoEmailActive");
            if (m_lstconfig.Any())
            {
                m_retval = m_lstconfig.Any(x => x.Desc1.ToLower() == "true");
            }
            return m_retval;
        }

        protected List<FieldTagReferenceVM> GetListFieldTagReferenceVM(string NotificationTemplateID)
        {

            List<FieldTagReferenceVM> m_lsFieldTagReferenceVM = new List<FieldTagReferenceVM>();
            MFieldTagReferencesDA m_objMFieldTagReferencesDA = new MFieldTagReferencesDA();
            m_objMFieldTagReferencesDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(FieldTagReferenceVM.Prop.FieldTagID.MapAlias);
            m_lstSelect.Add(FieldTagReferenceVM.Prop.RefIDColumn.MapAlias);
            m_lstSelect.Add(FieldTagReferenceVM.Prop.RefTable.MapAlias);
            m_lstSelect.Add(FieldTagReferenceVM.Prop.TagDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(string.Join(",", GetListTemplateTagsVM(NotificationTemplateID).Select(x => x.FieldTagID).ToList()));
            m_objFilter.Add(FieldTagReferenceVM.Prop.FieldTagID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMFieldTagReferencesDA = m_objMFieldTagReferencesDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMFieldTagReferencesDA.Success)
            {
                foreach (DataRow m_drMFieldTagReferencesDA in m_dicMFieldTagReferencesDA[0].Tables[0].Rows)
                {
                    FieldTagReferenceVM m_objFieldTagReferenceVM = new FieldTagReferenceVM();
                    m_objFieldTagReferenceVM.FieldTagID = m_drMFieldTagReferencesDA[FieldTagReferenceVM.Prop.FieldTagID.Name].ToString();
                    m_objFieldTagReferenceVM.RefIDColumn = m_drMFieldTagReferencesDA[FieldTagReferenceVM.Prop.RefIDColumn.Name].ToString();
                    m_objFieldTagReferenceVM.RefTable = (m_drMFieldTagReferencesDA[FieldTagReferenceVM.Prop.RefTable.Name].ToString());
                    m_objFieldTagReferenceVM.TagDesc = (m_drMFieldTagReferencesDA[FieldTagReferenceVM.Prop.TagDesc.Name].ToString());
                    m_lsFieldTagReferenceVM.Add(m_objFieldTagReferenceVM);
                }
            }

            return m_lsFieldTagReferenceVM;

        }
        protected List<TemplateTagsVM> GetListTemplateTagsVM(string NotificationTemplateID)
        {
            List<TemplateTagsVM> m_lstTemplateTagsVM = new List<TemplateTagsVM>();
            DTemplateTagsDA m_objDTemplateTagsDA = new DTemplateTagsDA();
            m_objDTemplateTagsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(TemplateTagsVM.Prop.TemplateTagID.MapAlias);
            m_lstSelect.Add(TemplateTagsVM.Prop.TemplateID.MapAlias);
            m_lstSelect.Add(TemplateTagsVM.Prop.FieldTagID.MapAlias);
            m_lstSelect.Add(TemplateTagsVM.Prop.TagDesc.MapAlias);
            m_lstSelect.Add(TemplateTagsVM.Prop.TemplateType.MapAlias);


            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(NotificationTemplateID);
            m_objFilter.Add(TemplateTagsVM.Prop.TemplateID.Map, m_lstFilter);


            Dictionary<int, DataSet> m_dicTNotificationValuesDA = m_objDTemplateTagsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDTemplateTagsDA.Success)
            {
                foreach (DataRow m_drTNotificationValuesDA in m_dicTNotificationValuesDA[0].Tables[0].Rows)
                {
                    TemplateTagsVM m_objNotificationValuesVM = new TemplateTagsVM();
                    m_objNotificationValuesVM.TemplateTagID = m_drTNotificationValuesDA[TemplateTagsVM.Prop.TemplateTagID.Name].ToString();
                    m_objNotificationValuesVM.TemplateID = m_drTNotificationValuesDA[TemplateTagsVM.Prop.TemplateID.Name].ToString();
                    m_objNotificationValuesVM.FieldTagID = m_drTNotificationValuesDA[TemplateTagsVM.Prop.FieldTagID.Name].ToString();
                    m_objNotificationValuesVM.TagDesc = m_drTNotificationValuesDA[TemplateTagsVM.Prop.TagDesc.Name].ToString();
                    m_objNotificationValuesVM.TemplateType = m_drTNotificationValuesDA[TemplateTagsVM.Prop.TemplateType.Name].ToString();

                    m_lstTemplateTagsVM.Add(m_objNotificationValuesVM);
                }
            }
            return m_lstTemplateTagsVM;

        }


        protected List<NotificationValuesVM> GetListNotificationValues(string MailNotifID, string TemplateID)
        {
            string message = "";
            DTemplateTagsDA m_objTemplateTags = new DTemplateTagsDA();
            m_objTemplateTags.ConnectionStringName = Global.ConnStrConfigName;

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<NotificationValuesVM> m_lsNotificationValuesVM = new List<NotificationValuesVM>();
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(TemplateTagsVM.Prop.TemplateTagID.MapAlias);
            m_lstSelect.Add(TemplateTagsVM.Prop.FieldTagID.MapAlias);
            m_lstSelect.Add(TemplateTagsVM.Prop.TagDesc.MapAlias);

            List<string> m_lstm_Group = new List<string>();
            m_lstm_Group.Add(TemplateTagsVM.Prop.TemplateTagID.Map);
            m_lstm_Group.Add(TemplateTagsVM.Prop.FieldTagID.Map);
            m_lstm_Group.Add(TemplateTagsVM.Prop.TagDesc.Map);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.None);
            m_lstFilter.Add(string.Empty);
            m_objFilter.Add(string.Format("(({0} IS NULL )or ({0} <>'MSchedules'))", TemplateTagsVM.Prop.RefTable.Map), m_lstFilter);

            if (!string.IsNullOrEmpty(MailNotifID))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(MailNotifID);
                m_objFilter.Add(TemplateTagsVM.Prop.MailNotificationID.Map, m_lstFilter);
            }

            if (TemplateID.Length > 0)
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(TemplateID);
                m_objFilter.Add(TemplateTagsVM.Prop.TemplateID.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.NotEqual);
                m_lstFilter.Add("System_");
                m_objFilter.Add(FieldTagReferenceVM.Prop.RefTable.Map, m_lstFilter);

                Dictionary<int, DataSet> m_dicTemplateTagDA = m_objTemplateTags.SelectBC_forPopulateSchedule(0, null, false, m_lstSelect, m_objFilter, null, m_lstm_Group, null, null);
                if (m_objTemplateTags.Message == string.Empty)
                {
                    m_lsNotificationValuesVM = (
                        from DataRow m_drTNotificationValues in m_dicTemplateTagDA[0].Tables[0].Rows
                        select new NotificationValuesVM()
                        {
                            FieldTagID = m_drTNotificationValues[TemplateTagsVM.Prop.FieldTagID.Name].ToString(),
                            TagDesc = m_drTNotificationValues[TemplateTagsVM.Prop.TagDesc.Name].ToString(),
                            Value = string.IsNullOrEmpty(MailNotifID) ? "" : GetNotifValues(MailNotifID, m_drTNotificationValues[TemplateTagsVM.Prop.FieldTagID.Name].ToString(), ref message)
                        }
                    ).Distinct().ToList();
                }
            }

            return m_lsNotificationValuesVM;
        }
        private string GetNotifValues(string mailNotifID, string FieldTagID, ref string message)
        {

            TNotificationValuesDA m_objTnotval = new TNotificationValuesDA();
            m_objTnotval.ConnectionStringName = Global.ConnStrConfigName;
            NotificationValuesVM objRet = new NotificationValuesVM();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(FieldTagID);
            m_objFilter.Add(NotificationValuesVM.Prop.FieldTagID.Map, m_lstFilter);
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(mailNotifID);
            m_objFilter.Add(NotificationValuesVM.Prop.MailNotificationID.Map, m_lstFilter);

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(NotificationValuesVM.Prop.MailNotificationID.MapAlias);
            m_lstSelect.Add(NotificationValuesVM.Prop.FieldTagID.MapAlias);
            m_lstSelect.Add(NotificationValuesVM.Prop.Value.MapAlias);

            Dictionary<int, DataSet> m_dicTTNotifValDA = m_objTnotval.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);
            if (m_objTnotval.Success && m_objTnotval.AffectedRows > 0)
            {
                DataRow m_drTNotificationValues = m_dicTTNotifValDA[0].Tables[0].Rows[0];
                objRet = new NotificationValuesVM()
                {
                    Value = m_drTNotificationValues[NotificationValuesVM.Prop.Value.Name].ToString()
                };
            }
            else
            {
                objRet.Value = "";
                message = m_objTnotval.Message;
            }

            return objRet.Value;
        }
        protected List<NotificationAttachmentVM> GetListNotificationAttachment(string MailNotifID)
        {
            List<NotificationAttachmentVM> lst_NotificationAttachment = new List<NotificationAttachmentVM>();
            TNotificationAttachmentsDA m_objTNotificationAttachmentDA = new TNotificationAttachmentsDA();
            m_objTNotificationAttachmentDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(NotificationAttachmentVM.Prop.AttachmentValueID.MapAlias);
            m_lstSelect.Add(NotificationAttachmentVM.Prop.MailNotificationID.MapAlias);
            m_lstSelect.Add(NotificationAttachmentVM.Prop.Filename.MapAlias);
            m_lstSelect.Add(NotificationAttachmentVM.Prop.ContentType.MapAlias);
            m_lstSelect.Add(NotificationAttachmentVM.Prop.RawData.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(MailNotifID);
            m_objFilter.Add(NotificationAttachmentVM.Prop.MailNotificationID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicTNotificationValuesDA = m_objTNotificationAttachmentDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objTNotificationAttachmentDA.Message == string.Empty)
            {
                foreach (DataRow dr in m_dicTNotificationValuesDA[0].Tables[0].Rows)
                {
                    NotificationAttachmentVM objNotificationVal = new NotificationAttachmentVM();
                    objNotificationVal.MailNotificationID = dr[NotificationAttachmentVM.Prop.MailNotificationID.Name].ToString();
                    objNotificationVal.Filename = dr[NotificationAttachmentVM.Prop.Filename.Name].ToString();
                    objNotificationVal.ContentType = dr[NotificationAttachmentVM.Prop.ContentType.Name].ToString();
                    objNotificationVal.RawData = dr[NotificationAttachmentVM.Prop.RawData.Name].ToString();
                    //objNotificationVal.NotificationValueID = dr[NotificationValuesVM.Prop.NotificationValueID.Name].ToString();
                    //objNotificationVal.MailNotificationID = dr[NotificationValuesVM.Prop.MailNotificationID.Name].ToString();
                    //objNotificationVal.FieldTagID = dr[NotificationValuesVM.Prop.FieldTagID.Name].ToString();
                    //objNotificationVal.TagDesc = dr[NotificationValuesVM.Prop.TagDesc.Name].ToString();
                    //objNotificationVal.Value = dr[NotificationValuesVM.Prop.Value.Name].ToString();

                    lst_NotificationAttachment.Add(objNotificationVal);
                }
            }

            return lst_NotificationAttachment;
        }

        #endregion

        #region Private
        private List<TCMembersVM> GetListTCMemberByFPT(string FPTID, ref string message)
        {
            List<TCMembersVM> m_lstTCMembersVM = new List<TCMembersVM>();
            TTCMembersDA m_objTTCMembersDA = new TTCMembersDA();
            m_objTTCMembersDA.ConnectionStringName = Global.ConnStrConfigName;
            CNegotiationConfigurationsDA m_objCNegotiationConfigurationsDA = new CNegotiationConfigurationsDA();
            m_objCNegotiationConfigurationsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(TCMembersVM.Prop.TCMemberID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.EmployeeID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.EmployeeName.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.SuperiorID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.SuperiorName.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.PeriodStart.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.PeriodEnd.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.BusinessUnitID.MapAlias);
            //m_lstSelect.Add(TCMembersVM.Prop.DelegateStartDate.MapAlias);
            //m_lstSelect.Add(TCMembersVM.Prop.DelegateEndDate.MapAlias);
            //m_lstSelect.Add(TCMembersVM.Prop.DelegateTo.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(FPTID);
            m_objFilter.Add(NegotiationConfigurationsVM.Prop.FPTID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(General.EnumDesc(NegoConfigTypes.TCMember));
            m_objFilter.Add(NegotiationConfigurationsVM.Prop.NegotiationConfigTypeID.Map, m_lstFilter);


            Dictionary<int, DataSet> m_dicCNegotiationConfigurationsDA = m_objCNegotiationConfigurationsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objCNegotiationConfigurationsDA.Success && m_objCNegotiationConfigurationsDA.Message == string.Empty)
            {
                foreach (DataRow m_drTTCMembersDA in m_dicCNegotiationConfigurationsDA[0].Tables[0].Rows)
                {
                    TCMembersVM m_objTCMembersVM = new TCMembersVM();
                    if (!string.IsNullOrEmpty(m_drTTCMembersDA[TCMembersVM.Prop.TCMemberID.Name].ToString()))
                    {
                        m_objTCMembersVM.TCMemberID = m_drTTCMembersDA[TCMembersVM.Prop.TCMemberID.Name].ToString();
                        m_objTCMembersVM.EmployeeID = m_drTTCMembersDA[TCMembersVM.Prop.EmployeeID.Name].ToString();
                        m_objTCMembersVM.EmployeeName = m_drTTCMembersDA[TCMembersVM.Prop.EmployeeName.Name].ToString();
                        m_objTCMembersVM.SuperiorID = m_drTTCMembersDA[TCMembersVM.Prop.SuperiorID.Name].ToString();
                        m_objTCMembersVM.SuperiorName = m_drTTCMembersDA[TCMembersVM.Prop.SuperiorName.Name].ToString();
                        m_objTCMembersVM.PeriodStart = DateTime.Parse(m_drTTCMembersDA[TCMembersVM.Prop.PeriodStart.Name].ToString());
                        m_objTCMembersVM.PeriodEnd = DateTime.Parse(m_drTTCMembersDA[TCMembersVM.Prop.PeriodEnd.Name].ToString());
                        m_objTCMembersVM.BusinessUnitID = m_drTTCMembersDA[TCMembersVM.Prop.BusinessUnitID.Name].ToString();
                        //m_objTCMembersVM.DelegateStartDate = !string.IsNullOrEmpty(m_drTTCMembersDA[TCMembersVM.Prop.DelegateStartDate.Name].ToString()) ? DateTime.Parse(m_drTTCMembersDA[TCMembersVM.Prop.DelegateStartDate.Name].ToString()) : (DateTime?)null;
                        //m_objTCMembersVM.DelegateEndDate = !string.IsNullOrEmpty(m_drTTCMembersDA[TCMembersVM.Prop.DelegateStartDate.Name].ToString()) ? DateTime.Parse(m_drTTCMembersDA[TCMembersVM.Prop.DelegateEndDate.Name].ToString()) : (DateTime?)null;
                        //m_objTCMembersVM.DelegateTo = m_drTTCMembersDA[TCMembersVM.Prop.DelegateTo.Name].ToString();
                        //m_objTCMembersVM.ListTCAppliedTypesVM = GetListTCAppliedTypes(m_objTCMembersVM.TCMemberID, ref message);
                        m_objTCMembersVM.ListTCMembersDelegationVM = GetListTCMembersDelegationVM(m_objTCMembersVM.TCMemberID);
                        m_lstTCMembersVM.Add(m_objTCMembersVM);
                    }
                }
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objTTCMembersDA.Message;

            return m_lstTCMembersVM;
        }
        private List<TCMembersDelegationVM> GetListTCMembersDelegationVM(string TCMemberID)
        {
            List<TCMembersDelegationVM> m_listTCMembersDelegationVM = new List<TCMembersDelegationVM>();
            TTCMemberDelegationsDA m_objTTCMemberDelegationsDA = new TTCMemberDelegationsDA();
            m_objTTCMemberDelegationsDA.ConnectionStringName = Global.ConnStrConfigName;

            FilterHeaderConditions m_fhcTTCMembers = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(TCMemberID);
            m_objFilter.Add(TCMembersDelegationVM.Prop.TCMemberID.Map, m_lstFilter);

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(TCMembersDelegationVM.Prop.TCDelegationID.MapAlias);
            m_lstSelect.Add(TCMembersDelegationVM.Prop.TCMemberID.MapAlias);
            m_lstSelect.Add(TCMembersDelegationVM.Prop.DelegateTo.MapAlias);
            m_lstSelect.Add(TCMembersDelegationVM.Prop.DelegateName.MapAlias);
            m_lstSelect.Add(TCMembersDelegationVM.Prop.DelegateStartDate.MapAlias);
            m_lstSelect.Add(TCMembersDelegationVM.Prop.DelegateEndDate.MapAlias);


            Dictionary<int, DataSet> m_dicTTCMemberDelegationsDA = m_objTTCMemberDelegationsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);
            if (m_objTTCMemberDelegationsDA.Message == string.Empty)
            {
                m_listTCMembersDelegationVM = (
                    from DataRow m_drTTCMembersDA in m_dicTTCMemberDelegationsDA[0].Tables[0].Rows
                    select new TCMembersDelegationVM()
                    {
                        TCDelegationID = m_drTTCMembersDA[TCMembersDelegationVM.Prop.TCDelegationID.Name].ToString(),
                        TCMemberID = m_drTTCMembersDA[TCMembersDelegationVM.Prop.TCMemberID.Name].ToString(),
                        DelegateTo = m_drTTCMembersDA[TCMembersDelegationVM.Prop.DelegateTo.Name].ToString(),
                        DelegateName = m_drTTCMembersDA[TCMembersDelegationVM.Prop.DelegateName.Name].ToString(),
                        DelegateStartDate = !string.IsNullOrEmpty(m_drTTCMembersDA[TCMembersDelegationVM.Prop.DelegateStartDate.Name].ToString()) ? DateTime.Parse(m_drTTCMembersDA[TCMembersDelegationVM.Prop.DelegateStartDate.Name].ToString()) : (DateTime?)null,
                        DelegateEndDate = !string.IsNullOrEmpty(m_drTTCMembersDA[TCMembersDelegationVM.Prop.DelegateEndDate.Name].ToString()) ? DateTime.Parse(m_drTTCMembersDA[TCMembersDelegationVM.Prop.DelegateEndDate.Name].ToString()) : (DateTime?)null,
                    }
                ).ToList();
            }

            return m_listTCMembersDelegationVM;
        }
        #endregion
    }
}