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
using System.Xml;
using System.Net;

namespace com.SML.BIGTRONS.Controllers
{
    public class ProjectController : BaseController
    {
        private readonly string title = "Project";
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
            MProjectDA m_objMProjectDA = new MProjectDA();
            m_objMProjectDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMProject = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMProject.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = ProjectVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMProjectDA = m_objMProjectDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpProjectBL in m_dicMProjectDA)
            {
                m_intCount = m_kvpProjectBL.Key;
                break;
            }

            List<ProjectVM> m_lstProjectVM = new List<ProjectVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(ProjectVM.Prop.ProjectID.MapAlias);
                m_lstSelect.Add(ProjectVM.Prop.ProjectDesc.MapAlias);
                m_lstSelect.Add(ProjectVM.Prop.CompanyDesc.MapAlias);
                m_lstSelect.Add(ProjectVM.Prop.DivisionDesc.MapAlias);
                m_lstSelect.Add(ProjectVM.Prop.LocationDesc.MapAlias);
                m_lstSelect.Add(ProjectVM.Prop.City.MapAlias);
                m_lstSelect.Add(ProjectVM.Prop.Street.MapAlias);
                m_lstSelect.Add(ProjectVM.Prop.Postal.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(ProjectVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMProjectDA = m_objMProjectDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMProjectDA.Message == string.Empty)
                {
                    m_lstProjectVM = (
                        from DataRow m_drMProjectDA in m_dicMProjectDA[0].Tables[0].Rows
                        select new ProjectVM()
                        {
                            ProjectID = m_drMProjectDA[ProjectVM.Prop.ProjectID.Name].ToString(),
                            ProjectDesc = m_drMProjectDA[ProjectVM.Prop.ProjectDesc.Name].ToString(),
                            CompanyDesc = m_drMProjectDA[ProjectVM.Prop.CompanyDesc.Name].ToString(),
                            DivisionDesc = m_drMProjectDA[ProjectVM.Prop.DivisionDesc.Name].ToString(),
                            LocationDesc = m_drMProjectDA[ProjectVM.Prop.LocationDesc.Name].ToString(),
                            City = m_drMProjectDA[ProjectVM.Prop.City.Name].ToString(),
                            Street = m_drMProjectDA[ProjectVM.Prop.Street.Name].ToString(),
                            Postal = m_drMProjectDA[ProjectVM.Prop.Postal.Name].ToString(),
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstProjectVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters, string DivisionID)
        {
            MProjectDA m_objMProjectDA = new MProjectDA();
            m_objMProjectDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMProject = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMProject.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = ProjectVM.Prop.Map(m_strDataIndex, false);
                    
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
            if (!string.IsNullOrEmpty(DivisionID))
            {
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(DivisionID);
                m_objFilter.Add(ProjectVM.Prop.DivisionID.Map, m_lstFilter);
            }
            Dictionary<int, DataSet> m_dicMProjectDA = m_objMProjectDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpProjectBL in m_dicMProjectDA)
            {
                m_intCount = m_kvpProjectBL.Key;
                break;
            }

            List<ProjectVM> m_lstProjectVM = new List<ProjectVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(ProjectVM.Prop.ProjectID.MapAlias);
                m_lstSelect.Add(ProjectVM.Prop.ProjectDesc.MapAlias);
                m_lstSelect.Add(ProjectVM.Prop.CompanyID.MapAlias);
                m_lstSelect.Add(ProjectVM.Prop.CompanyDesc.MapAlias);
                m_lstSelect.Add(ProjectVM.Prop.DivisionDesc.MapAlias);
                m_lstSelect.Add(ProjectVM.Prop.LocationDesc.MapAlias);
                m_lstSelect.Add(ProjectVM.Prop.City.MapAlias);
                m_lstSelect.Add(ProjectVM.Prop.Street.MapAlias);
                m_lstSelect.Add(ProjectVM.Prop.Postal.MapAlias);
                m_lstSelect.Add(ProjectVM.Prop.RegionDesc.MapAlias);
                m_lstSelect.Add(RegionVM.Prop.RegionID.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(ProjectVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMProjectDA = m_objMProjectDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMProjectDA.Message == string.Empty)
                {
                    m_lstProjectVM = (
                        from DataRow m_drMProjectDA in m_dicMProjectDA[0].Tables[0].Rows
                        select new ProjectVM()
                        {
                            ProjectID = m_drMProjectDA[ProjectVM.Prop.ProjectID.Name].ToString(),
                            ProjectDesc = m_drMProjectDA[ProjectVM.Prop.ProjectDesc.Name].ToString(),
                            CompanyID = m_drMProjectDA[ProjectVM.Prop.CompanyID.Name].ToString(),
                            CompanyDesc = m_drMProjectDA[ProjectVM.Prop.CompanyDesc.Name].ToString(),
                            DivisionDesc = m_drMProjectDA[ProjectVM.Prop.DivisionDesc.Name].ToString(),
                            LocationDesc = m_drMProjectDA[ProjectVM.Prop.LocationDesc.Name].ToString(),
                            City = m_drMProjectDA[ProjectVM.Prop.City.Name].ToString(),
                            Street = m_drMProjectDA[ProjectVM.Prop.Street.Name].ToString(),
                            Postal = m_drMProjectDA[ProjectVM.Prop.Postal.Name].ToString(),
                            RegionDesc = m_drMProjectDA[ProjectVM.Prop.RegionDesc.Name].ToString(),
                            RegionID = m_drMProjectDA[RegionVM.Prop.RegionID.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstProjectVM, m_intCount);
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

            ProjectVM m_objProjectVM = new ProjectVM();
            ViewDataDictionary m_vddProject = new ViewDataDictionary();
            m_vddProject.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddProject.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
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
                Model = m_objProjectVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddProject,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Detail(string Caller, string Selected)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            string m_strMessage = string.Empty;
            ProjectVM m_objProjectVM = new ProjectVM();
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
                m_objProjectVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
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
                Model = m_objProjectVM,
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
            ProjectVM m_objProjectVM = new ProjectVM();
            if (m_dicSelectedRow.Count > 0)
                m_objProjectVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddProject = new ViewDataDictionary();
            m_vddProject.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddProject.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objProjectVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddProject,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<ProjectVM> m_lstSelectedRow = new List<ProjectVM>();
            m_lstSelectedRow = JSON.Deserialize<List<ProjectVM>>(Selected);

            MProjectDA m_objMProjectDA = new MProjectDA();
            m_objMProjectDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (ProjectVM m_objProjectVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifProjectVM = m_objProjectVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifProjectVM in m_arrPifProjectVM)
                    {
                        string m_strFieldName = m_pifProjectVM.Name;
                        object m_objFieldValue = m_pifProjectVM.GetValue(m_objProjectVM);
                        if (m_objProjectVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(ProjectVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objMProjectDA.DeleteBC(m_objFilter, false);
                    if (m_objMProjectDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMProjectDA.Message);
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

        public ActionResult Browse(string ControlProjectID, string ControlProjectDesc, string ControlCompanyID, string ControlCompanyDesc, string ControlDivisionDesc, string ControlLocationDesc, string ControlRegionID, string ControlRegionDesc, string ValueCompanyDesc = "", string FilterProjectID = "", string FilterProjectDesc = "", string FilterDivisionID = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddProject = new ViewDataDictionary();
            m_vddProject.Add("Control" + ProjectVM.Prop.ProjectID.Name, ControlProjectID);
            m_vddProject.Add("Control" + ProjectVM.Prop.ProjectDesc.Name, ControlProjectDesc);
            m_vddProject.Add("Control" + ProjectVM.Prop.CompanyDesc.Name, ControlCompanyDesc);
            m_vddProject.Add("Control" + ProjectVM.Prop.CompanyID.Name, ControlCompanyID);
            m_vddProject.Add("Control" + ProjectVM.Prop.DivisionDesc.Name, ControlDivisionDesc);
            m_vddProject.Add("Control" + ProjectVM.Prop.LocationDesc.Name, ControlLocationDesc);
            m_vddProject.Add("Control" + RegionVM.Prop.RegionID.Name, ControlRegionID);
            m_vddProject.Add("Control" + ProjectVM.Prop.RegionDesc.Name, ControlRegionDesc);
            m_vddProject.Add(CompanyVM.Prop.CompanyDesc.Name, ValueCompanyDesc);
            m_vddProject.Add(ProjectVM.Prop.ProjectID.Name, FilterProjectID);
            m_vddProject.Add(ProjectVM.Prop.ProjectDesc.Name, FilterProjectDesc);
            m_vddProject.Add(ProjectVM.Prop.DivisionID.Name, FilterDivisionID);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddProject,
                ViewName = "../Project/_Browse"
            };
        }

        public ActionResult Save(string Action)
        {
            //action = JSON.Deserialize<string>(action);
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MProjectDA m_objMProjectDA = new MProjectDA();
            m_objMProjectDA.ConnectionStringName = Global.ConnStrConfigName;
            try
            {
                string m_strProjectID = this.Request.Params[ProjectVM.Prop.ProjectID.Name];
                string m_strProjectDesc = this.Request.Params[ProjectVM.Prop.ProjectDesc.Name];
                string m_strCompanyID = this.Request.Params[ProjectVM.Prop.CompanyID.Name];
                string m_strDivisionID = this.Request.Params[ProjectVM.Prop.DivisionID.Name];
                string m_strLocationID = this.Request.Params[ProjectVM.Prop.LocationID.Name];
                string m_strCity = this.Request.Params[ProjectVM.Prop.City.Name];
                string m_strStreet = this.Request.Params[ProjectVM.Prop.Street.Name];
                string m_strPostal = this.Request.Params[ProjectVM.Prop.Postal.Name];

                m_lstMessage = IsSaveValid(Action, m_strProjectID, m_strProjectDesc, m_strCompanyID, m_strDivisionID, m_strLocationID, m_strCity, m_strStreet, m_strPostal);
                if (m_lstMessage.Count <= 0)
                {
                    MProject m_objMProject = new MProject();
                    m_objMProject.ProjectID = m_strProjectID;

                    m_objMProjectDA.Data = m_objMProject;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objMProjectDA.Select();

                    m_objMProject.ProjectDesc = m_strProjectDesc;
                    m_objMProject.City = m_strCity;
                    m_objMProject.Street = m_strStreet;
                    m_objMProject.Postal = m_strPostal;
                    m_objMProject.CompanyID = m_strCompanyID;
                    m_objMProject.DivisionID = m_strDivisionID;
                    m_objMProject.LocationID = m_strLocationID;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objMProjectDA.Insert(false);
                    else
                        m_objMProjectDA.Update(false);

                    if (!m_objMProjectDA.Success || m_objMProjectDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMProjectDA.Message);
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

        public ActionResult Sync()
        {
            int m_intadded = 0;
            int m_intupdated = 0;
            int m_intfailed = 0;
            string m_soapenv = @"<soap:Envelope xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/'>
  <soap:Body>
    <Project xmlns='http://tempuri.org/' />
  </soap:Body>
</soap:Envelope>";  

            var m_lstconfig = GetConfig("WS", null,"ETT");
            if (!m_lstconfig.Any())
            {
                return this.Direct(false);
            }
            string m_struser = m_lstconfig.Any(x => x.Key2 == "Credential" && x.Key4 == "User") ? m_lstconfig.Where(x => x.Key2 == "Credential" && x.Key4 == "User").FirstOrDefault().Desc1 : string.Empty;
            string m_strpass = m_lstconfig.Any(x => x.Key2 == "Credential" && x.Key4 == "Password") ? m_lstconfig.Where(x => x.Key2 == "Credential" && x.Key4 == "Password").FirstOrDefault().Desc1 : string.Empty;
            string m_strdomain = m_lstconfig.Any(x => x.Key2 == "Credential" && x.Key4 == "Domain") ? m_lstconfig.Where(x => x.Key2 == "Credential" && x.Key4 == "Domain").FirstOrDefault().Desc1 : string.Empty;
            string m_strurl = m_lstconfig.Any(x => x.Key2 == "URL" && x.Key4 == "Project") ? m_lstconfig.Where(x => x.Key2 == "URL" && x.Key4 == "Project").FirstOrDefault().Desc1 : string.Empty;
            NetworkCredential m_credential = new NetworkCredential(m_struser, m_strpass, m_strdomain);
            string m_strsoapresult = GetSOAPString(m_soapenv, m_strurl, m_credential);
            if (string.IsNullOrEmpty(m_strsoapresult))
            {
                return this.Direct(false);
            }

            XmlDocument document = new XmlDocument();
            document.LoadXml(m_strsoapresult);
            XmlNamespaceManager manager = new XmlNamespaceManager(document.NameTable);

            XmlNodeList xnList = document.ChildNodes[1].ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes;
            List<MProject> m_lstProject = new List<MProject>();

            foreach (XmlNode item in xnList)
            {
                MProject m_Project = new MProject();
                m_Project.ProjectID = item["ID"].InnerText;
                m_Project.ProjectDesc = item["ProjectName"].InnerText;
                //m_Project.CompanyID = item["FIELD3"].InnerText;
                m_Project.DivisionID = item["FIDBusinessArea"].InnerText;
                //m_Project.LocationID = item["FIDProjectLocation"].InnerText;
                m_Project.City = item["FIDProjectLocation"].InnerText;
                m_Project.Street = "-";
                m_Project.Postal = "-";
                m_lstProject.Add(m_Project);
            }

            if (!m_lstProject.Any())
            {
                return this.Direct(false, "Error Read Data");
            }
            else
            {
                MProjectDA m_objMProjectDA = new MProjectDA();
                m_objMProjectDA.ConnectionStringName = Global.ConnStrConfigName;
                foreach (var item in m_lstProject)
                {
                    item.Street = item.Street == null ? "-" : item.Street;
                    item.Postal = item.Postal == null ? "-" : item.Postal;
                   
                    try
                    {
                        m_objMProjectDA.Data = item;
                        m_objMProjectDA.Insert(false);
                        if (!m_objMProjectDA.Success || m_objMProjectDA.Message != string.Empty)
                        {
                            if (m_objMProjectDA.Message == "Cannot insert duplicate data.")
                            {
                                m_intupdated += 1;
                            }
                            else
                            {
                                m_intfailed += 1;
                            }
                        }
                        else
                        {
                            m_intadded += 1;
                        }

                    }
                    catch (Exception ex)
                    {
                        m_intfailed += 1;
                    }
                    
                }
                Global.ShowInfoAlert(title, $"Total : {m_lstProject.Count}  New : {m_intadded} Exist : {m_intupdated} Failed: {m_intfailed}");
                return this.Direct(true);
            }

        }

        #endregion

        #region Direct Method

        public ActionResult GetProject(string ControlProjectID, string ControlProjectDesc, string ControlCompanyID, string ControlCompanyDesc,string ControlRegionID,
            string ControlRegionDesc, string ControlLocationDesc, string ControlDivisionDesc,
            string FilterProjectID, string FilterProjectDesc, bool Exact = false, string CompanyDesc = "")
        {
            try
            {
                Dictionary<int, List<ProjectVM>> m_dicProjectData = GetProjectData(true, CompanyDesc, FilterProjectID, FilterProjectDesc);
                KeyValuePair<int, List<ProjectVM>> m_kvpProjectVM = m_dicProjectData.AsEnumerable().ToList()[0];
                if (m_kvpProjectVM.Key < 1 || (m_kvpProjectVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpProjectVM.Key > 1 && !Exact)
                    return Browse(ControlProjectID, ControlProjectDesc, ControlCompanyID, ControlCompanyDesc, ControlDivisionDesc, ControlLocationDesc,ControlRegionID, ControlRegionDesc, CompanyDesc, FilterProjectID, FilterProjectDesc);

                m_dicProjectData = GetProjectData(false, CompanyDesc, FilterProjectID, FilterProjectDesc);
                ProjectVM m_objProjectVM = m_dicProjectData[0][0];
                this.GetCmp<TextField>(ControlProjectID).Value = m_objProjectVM.ProjectID;
                this.GetCmp<TextField>(ControlProjectDesc).Value = m_objProjectVM.ProjectDesc;
                if (!string.IsNullOrEmpty(ControlCompanyID)) this.GetCmp<TextField>(ControlCompanyID).Value = m_objProjectVM.CompanyID;
                if (!string.IsNullOrEmpty(ControlCompanyDesc)) this.GetCmp<TextField>(ControlCompanyDesc).Value = m_objProjectVM.CompanyDesc;

                if (!string.IsNullOrEmpty(ControlRegionDesc)) this.GetCmp<TextField>(ControlRegionDesc).Value = m_objProjectVM.RegionDesc;
                if (!string.IsNullOrEmpty(ControlLocationDesc)) this.GetCmp<TextField>(ControlLocationDesc).Value = m_objProjectVM.LocationDesc;
                if (!string.IsNullOrEmpty(ControlDivisionDesc)) this.GetCmp<TextField>(ControlDivisionDesc).Value = m_objProjectVM.DivisionDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, string ProjectID, string ProjectDesc, string CompanyID, string DivisionID, string LocationID, string City, string Street, string Postal)
        {
            List<string> m_lstReturn = new List<string>();

            if (ProjectID == string.Empty)
                m_lstReturn.Add(ProjectVM.Prop.ProjectID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (ProjectDesc == string.Empty)
                m_lstReturn.Add(ProjectVM.Prop.ProjectDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (CompanyID == string.Empty)
                m_lstReturn.Add(ProjectVM.Prop.CompanyID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (DivisionID == string.Empty)
                m_lstReturn.Add(ProjectVM.Prop.DivisionID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (LocationID == string.Empty)
                m_lstReturn.Add(ProjectVM.Prop.LocationID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (City == string.Empty)
                m_lstReturn.Add(ProjectVM.Prop.City.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (Street == string.Empty)
                m_lstReturn.Add(ProjectVM.Prop.Street.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (Postal == string.Empty)
                m_lstReturn.Add(ProjectVM.Prop.Postal.Desc + " " + General.EnumDesc(MessageLib.mustFill));

            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(ProjectVM.Prop.ProjectID.Name, parameters[ProjectVM.Prop.ProjectID.Name]);
            m_dicReturn.Add(ProjectVM.Prop.ProjectDesc.Name, parameters[ProjectVM.Prop.ProjectDesc.Name]);
            m_dicReturn.Add(ProjectVM.Prop.CompanyID.Name, parameters[ProjectVM.Prop.CompanyID.Name]);
            m_dicReturn.Add(ProjectVM.Prop.CompanyDesc.Name, parameters[ProjectVM.Prop.CompanyDesc.Name]);
            m_dicReturn.Add(ProjectVM.Prop.City.Name, parameters[ProjectVM.Prop.City.Name]);
            m_dicReturn.Add(ProjectVM.Prop.Street.Name, parameters[ProjectVM.Prop.Street.Name]);
            m_dicReturn.Add(ProjectVM.Prop.Postal.Name, parameters[ProjectVM.Prop.Postal.Name]);

            return m_dicReturn;
        }

        private ProjectVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            ProjectVM m_objProjectVM = new ProjectVM();
            MProjectDA m_objMProjectDA = new MProjectDA();
            m_objMProjectDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ProjectVM.Prop.ProjectID.MapAlias);
            m_lstSelect.Add(ProjectVM.Prop.ProjectDesc.MapAlias);
            m_lstSelect.Add(ProjectVM.Prop.CompanyID.MapAlias);
            m_lstSelect.Add(ProjectVM.Prop.CompanyDesc.MapAlias);
            m_lstSelect.Add(ProjectVM.Prop.DivisionID.MapAlias);
            m_lstSelect.Add(ProjectVM.Prop.DivisionDesc.MapAlias);
            m_lstSelect.Add(ProjectVM.Prop.LocationID.MapAlias);
            m_lstSelect.Add(ProjectVM.Prop.LocationDesc.MapAlias);
            m_lstSelect.Add(ProjectVM.Prop.City.MapAlias);
            m_lstSelect.Add(ProjectVM.Prop.Street.MapAlias);
            m_lstSelect.Add(ProjectVM.Prop.Postal.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objProjectVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(ProjectVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMProjectDA = m_objMProjectDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMProjectDA.Message == string.Empty)
            {
                DataRow m_drMProjectDA = m_dicMProjectDA[0].Tables[0].Rows[0];
                m_objProjectVM.ProjectID = m_drMProjectDA[ProjectVM.Prop.ProjectID.Name].ToString();
                m_objProjectVM.ProjectDesc = m_drMProjectDA[ProjectVM.Prop.ProjectDesc.Name].ToString();
                m_objProjectVM.CompanyID = m_drMProjectDA[ProjectVM.Prop.CompanyID.Name].ToString();
                m_objProjectVM.CompanyDesc = m_drMProjectDA[ProjectVM.Prop.CompanyDesc.Name].ToString();
                m_objProjectVM.DivisionID = m_drMProjectDA[ProjectVM.Prop.DivisionID.Name].ToString();
                m_objProjectVM.DivisionDesc = m_drMProjectDA[ProjectVM.Prop.DivisionDesc.Name].ToString();
                m_objProjectVM.LocationID = m_drMProjectDA[ProjectVM.Prop.LocationID.Name].ToString();
                m_objProjectVM.LocationDesc = m_drMProjectDA[ProjectVM.Prop.LocationDesc.Name].ToString();
                m_objProjectVM.City = m_drMProjectDA[ProjectVM.Prop.City.Name].ToString();
                m_objProjectVM.Street = m_drMProjectDA[ProjectVM.Prop.Street.Name].ToString();
                m_objProjectVM.Postal = m_drMProjectDA[ProjectVM.Prop.Postal.Name].ToString();
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMProjectDA.Message;

            return m_objProjectVM;
        }

        #endregion

        #region Public Method

        public Dictionary<int, List<ProjectVM>> GetProjectData(bool isCount, string CompanyDesc, string ProjectID, string ProjectDesc)
        {
            int m_intCount = 0;
            List<ProjectVM> m_lstProjectVM = new List<ViewModels.ProjectVM>();
            Dictionary<int, List<ProjectVM>> m_dicReturn = new Dictionary<int, List<ProjectVM>>();
            MProjectDA m_objMProjectDA = new MProjectDA();
            m_objMProjectDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ProjectVM.Prop.ProjectID.MapAlias);
            m_lstSelect.Add(ProjectVM.Prop.ProjectDesc.MapAlias);
            m_lstSelect.Add(ProjectVM.Prop.CompanyID.MapAlias);
            m_lstSelect.Add(ProjectVM.Prop.CompanyDesc.MapAlias);
            m_lstSelect.Add(ProjectVM.Prop.RegionDesc.MapAlias);
            m_lstSelect.Add(ProjectVM.Prop.LocationDesc.MapAlias);
            m_lstSelect.Add(ProjectVM.Prop.DivisionDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(ProjectID);
            m_objFilter.Add(ProjectVM.Prop.ProjectID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(ProjectDesc);
            m_objFilter.Add(ProjectVM.Prop.ProjectDesc.Map, m_lstFilter);

            if (!string.IsNullOrEmpty(CompanyDesc))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Contains);
                m_lstFilter.Add(CompanyDesc);
                m_objFilter.Add(ProjectVM.Prop.CompanyDesc.Map, m_lstFilter);
            }

            Dictionary<int, DataSet> m_dicMProjectDA = m_objMProjectDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMProjectDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpProjectBL in m_dicMProjectDA)
                    {
                        m_intCount = m_kvpProjectBL.Key;
                        break;
                    }
                else
                {
                    m_lstProjectVM = (
                        from DataRow m_drMProjectDA in m_dicMProjectDA[0].Tables[0].Rows
                        select new ProjectVM()
                        {
                            ProjectID = m_drMProjectDA[ProjectVM.Prop.ProjectID.Name].ToString(),
                            ProjectDesc = m_drMProjectDA[ProjectVM.Prop.ProjectDesc.Name].ToString(),
                            CompanyID = m_drMProjectDA[ProjectVM.Prop.CompanyID.Name].ToString(),
                            CompanyDesc = m_drMProjectDA[ProjectVM.Prop.CompanyDesc.Name].ToString(),
                            RegionDesc = m_drMProjectDA[ProjectVM.Prop.RegionDesc.Name].ToString(),
                            LocationDesc = m_drMProjectDA[ProjectVM.Prop.LocationDesc.Name].ToString(),
                            DivisionDesc = m_drMProjectDA[ProjectVM.Prop.DivisionDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstProjectVM);
            return m_dicReturn;
        }

        #endregion
    }
}