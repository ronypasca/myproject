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
using System.Net;
using System.IO;
using System.Xml;

namespace com.SML.BIGTRONS.Controllers
{
    public class CompanyController : BaseController
    {
        private readonly string title = "Company";
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
            MCompanyDA m_objMCompanyDA = new MCompanyDA();
            m_objMCompanyDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMCompany = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMCompany.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = CompanyVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMCompanyDA = m_objMCompanyDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpCompanyBL in m_dicMCompanyDA)
            {
                m_intCount = m_kvpCompanyBL.Key;
                break;
            }

            List<CompanyVM> m_lstCompanyVM = new List<CompanyVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(CompanyVM.Prop.CompanyID.MapAlias);
                m_lstSelect.Add(CompanyVM.Prop.CompanyDesc.MapAlias);
                m_lstSelect.Add(CompanyVM.Prop.CountryDesc.MapAlias);
                m_lstSelect.Add(CompanyVM.Prop.City.MapAlias);
                m_lstSelect.Add(CompanyVM.Prop.Street.MapAlias);
                m_lstSelect.Add(CompanyVM.Prop.Postal.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(CompanyVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMCompanyDA = m_objMCompanyDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMCompanyDA.Message == string.Empty)
                {
                    m_lstCompanyVM = (
                        from DataRow m_drMCompanyDA in m_dicMCompanyDA[0].Tables[0].Rows
                        select new CompanyVM()
                        {
                            CompanyID = m_drMCompanyDA[CompanyVM.Prop.CompanyID.Name].ToString(),
                            CompanyDesc = m_drMCompanyDA[CompanyVM.Prop.CompanyDesc.Name].ToString(),
                            CountryDesc = m_drMCompanyDA[CompanyVM.Prop.CountryDesc.Name].ToString(),
                            City = m_drMCompanyDA[CompanyVM.Prop.City.Name].ToString(),
                            Street = m_drMCompanyDA[CompanyVM.Prop.Street.Name].ToString(),
                            Postal = m_drMCompanyDA[CompanyVM.Prop.Postal.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstCompanyVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            MCompanyDA m_objMCompanyDA = new MCompanyDA();
            m_objMCompanyDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMCompany = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMCompany.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = CompanyVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMCompanyDA = m_objMCompanyDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpCompanyBL in m_dicMCompanyDA)
            {
                m_intCount = m_kvpCompanyBL.Key;
                break;
            }

            List<CompanyVM> m_lstCompanyVM = new List<CompanyVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(CompanyVM.Prop.CompanyID.MapAlias);
                m_lstSelect.Add(CompanyVM.Prop.CompanyDesc.MapAlias);
                m_lstSelect.Add(CompanyVM.Prop.CountryDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(CompanyVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMCompanyDA = m_objMCompanyDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMCompanyDA.Message == string.Empty)
                {
                    m_lstCompanyVM = (
                        from DataRow m_drMCompanyDA in m_dicMCompanyDA[0].Tables[0].Rows
                        select new CompanyVM()
                        {
                            CompanyID = m_drMCompanyDA[CompanyVM.Prop.CompanyID.Name].ToString(),
                            CompanyDesc = m_drMCompanyDA[CompanyVM.Prop.CompanyDesc.Name].ToString(),
                            CountryDesc = m_drMCompanyDA[CompanyVM.Prop.CountryDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstCompanyVM, m_intCount);
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

            CompanyVM m_objCompanyVM = new CompanyVM();
            ViewDataDictionary m_vddCompany = new ViewDataDictionary();
            m_vddCompany.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddCompany.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
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
                Model = m_objCompanyVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddCompany,
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
            CompanyVM m_objCompanyVM = new CompanyVM();
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
                m_objCompanyVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddCompany = new ViewDataDictionary();
            m_vddCompany.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objCompanyVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddCompany,
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
            CompanyVM m_objCompanyVM = new CompanyVM();
            if (m_dicSelectedRow.Count > 0)
                m_objCompanyVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddCompany = new ViewDataDictionary();
            m_vddCompany.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddCompany.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objCompanyVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddCompany,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<CompanyVM> m_lstSelectedRow = new List<CompanyVM>();
            m_lstSelectedRow = JSON.Deserialize<List<CompanyVM>>(Selected);

            MCompanyDA m_objMCompanyDA = new MCompanyDA();
            m_objMCompanyDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (CompanyVM m_objCompanyVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifCompanyVM = m_objCompanyVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifCompanyVM in m_arrPifCompanyVM)
                    {
                        string m_strFieldName = m_pifCompanyVM.Name;
                        object m_objFieldValue = m_pifCompanyVM.GetValue(m_objCompanyVM);
                        if (m_objCompanyVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(CompanyVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objMCompanyDA.DeleteBC(m_objFilter, false);
                    if (m_objMCompanyDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMCompanyDA.Message);
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

        public ActionResult Browse(string ControlCompanyID, string ControlCompanyDesc, string FilterCompanyID = "", string FilterCompanyDesc = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddCompany = new ViewDataDictionary();
            m_vddCompany.Add("Control" + CompanyVM.Prop.CompanyID.Name, ControlCompanyID);
            m_vddCompany.Add("Control" + CompanyVM.Prop.CompanyDesc.Name, ControlCompanyDesc);
            m_vddCompany.Add(CompanyVM.Prop.CompanyID.Name, FilterCompanyID);
            m_vddCompany.Add(CompanyVM.Prop.CompanyDesc.Name, FilterCompanyDesc);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddCompany,
                ViewName = "../Company/_Browse"
            };
        }

        public ActionResult Save(string Action)
        {
            //action = JSON.Deserialize<string>(action);
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MCompanyDA m_objMCompanyDA = new MCompanyDA();
            m_objMCompanyDA.ConnectionStringName = Global.ConnStrConfigName;
            try
            {
                string m_strCompanyID = this.Request.Params[CompanyVM.Prop.CompanyID.Name];
                string m_strCompanyDesc = this.Request.Params[CompanyVM.Prop.CompanyDesc.Name];
                string m_strCountryID = this.Request.Params[CountryVM.Prop.CountryID.Name];
                string m_strCity = this.Request.Params[CompanyVM.Prop.City.Name];
                string m_strStreet = this.Request.Params[CompanyVM.Prop.Street.Name];
                string m_strPostal = this.Request.Params[CompanyVM.Prop.Postal.Name];

                m_lstMessage = IsSaveValid(Action, m_strCompanyID, m_strCompanyDesc, m_strCountryID, m_strCity, m_strStreet, m_strPostal);
                if (m_lstMessage.Count <= 0)
                {
                    MCompany m_objMCompany = new MCompany();
                    m_objMCompany.CompanyID = m_strCompanyID;
                    m_objMCompanyDA.Data = m_objMCompany;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objMCompanyDA.Select();

                    m_objMCompany.CompanyDesc = m_strCompanyDesc;
                    m_objMCompany.CountryID = m_strCountryID;
                    m_objMCompany.City = m_strCity;
                    m_objMCompany.Street = m_strStreet;
                    m_objMCompany.Postal = m_strPostal;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objMCompanyDA.Insert(false);
                    else
                        m_objMCompanyDA.Update(false);

                    if (!m_objMCompanyDA.Success || m_objMCompanyDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMCompanyDA.Message);
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
            string m_soapenv = @"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:urn='urn:sap-com:document:sap:rfc:functions'>
                                   <soapenv:Header/>
                                   <soapenv:Body>
                                      <urn:ZRFC_READ_TABLE>
                                         <DATA>
                                            <!--Zero or more repetitions:-->
                                            <item>
                                               <FIELD1></FIELD1>
                                               <FIELD2></FIELD2>
                                               <FIELD3></FIELD3>
                                               <FIELD4></FIELD4>
                                               <FIELD5></FIELD5>
                                               <FIELD6></FIELD6>
                                               <FIELD7></FIELD7>
                                               <FIELD8></FIELD8>
                                               <FIELD9></FIELD9>
                                               <FIELD10></FIELD10>
                                               <FIELD11></FIELD11>
                                               <FIELD12></FIELD12>
                                               <FIELD13></FIELD13>
                                               <FIELD14></FIELD14>
                                               <FIELD15></FIELD15>
                                               <FIELD16></FIELD16>
                                               <FIELD17></FIELD17>
                                               <FIELD18></FIELD18>
                                               <FIELD19></FIELD19>
                                               <FIELD20></FIELD20>
                                               <FIELD21></FIELD21>
                                               <FIELD22></FIELD22>
                                               <FIELD23></FIELD23>
                                               <FIELD24></FIELD24>
                                               <FIELD25></FIELD25>
                                               <FIELD26></FIELD26>
                                               <FIELD27></FIELD27>
                                               <FIELD28></FIELD28>
                                               <FIELD29></FIELD29>
                                               <FIELD30></FIELD30>
                                               <FIELD31></FIELD31>
                                               <FIELD32></FIELD32>
                                            </item>
                                         </DATA>
                                         <!--Optional:-->
                                         <DELIMITER></DELIMITER>
                                         <FIELDS>
                                            <!--Zero or more repetitions:-->
                                            <item>
                                               <FIELDNAME></FIELDNAME>
                                               <OFFSET></OFFSET>
                                               <LENGTH></LENGTH>
                                               <TYPE></TYPE>
                                               <FIELDTEXT></FIELDTEXT>
                                            </item>
                                         </FIELDS>
                                         <!--Optional:-->
                                         <NO_DATA></NO_DATA>
                                         <OPTIONS>
                                            <!--Zero or more repetitions:-->
                                            <item>
                                               <TEXT></TEXT>
                                            </item>
                                         </OPTIONS>
                                         <QUERY_TABLE>T001</QUERY_TABLE>
                                         <!--Optional:-->
                                         <ROWCOUNT></ROWCOUNT>
                                         <!--Optional:-->
                                         <ROWSKIPS></ROWSKIPS>
                                      </urn:ZRFC_READ_TABLE>
                                   </soapenv:Body>
                                </soapenv:Envelope>";


            var m_lstconfig = GetConfig("WS", null, "SAP");
            if (!m_lstconfig.Any())
            {
                return this.Direct(false);
            }
            string m_strurl = m_lstconfig.Any(x => x.Key2 == "URL" && x.Key4 == "Company") ? m_lstconfig.Where(x => x.Key2 == "URL" && x.Key4 == "Company").FirstOrDefault().Desc1 : string.Empty;
            string m_strsoapresult = GetSOAPString(m_soapenv, m_strurl);
            if (string.IsNullOrEmpty(m_strsoapresult))
            {
                return this.Direct(false);
            }
            XmlDocument m_document = new XmlDocument();
            m_document.LoadXml(m_strsoapresult);
            XmlNodeList xnList = m_document.ChildNodes[0].ChildNodes[1].ChildNodes[0].ChildNodes[0].ChildNodes;
            List<MCompany> m_lstCompany = new List<MCompany>();

            foreach (XmlNode item in xnList)
            {
                MCompany m_Company = new MCompany();
                m_Company.CompanyID = item["FIELD1"].InnerText;
                m_Company.CompanyDesc = item["FIELD2"].InnerText;
                m_Company.City = item["FIELD3"].InnerText;
                m_Company.CountryID = item["FIELD4"].InnerText;
                m_lstCompany.Add(m_Company);
            }

            if (!m_lstCompany.Any())
            {
                return this.Direct(false, "Error Read Data");
            }
            else
            {
                MCompanyDA m_objMCompanyDA = new MCompanyDA();
                m_objMCompanyDA.ConnectionStringName = Global.ConnStrConfigName;
                foreach (var item in m_lstCompany)
                {
                    item.Street = item.Street == null ? "-" : item.Street;
                    item.Postal = item.Postal == null ? "-" : item.Postal;
                    try
                    {
                        m_objMCompanyDA.Data = item;
                        m_objMCompanyDA.Insert(false);
                        if (!m_objMCompanyDA.Success || m_objMCompanyDA.Message != string.Empty)
                        {
                            if (m_objMCompanyDA.Message == "Cannot insert duplicate data.")
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
                Global.ShowInfoAlert(title, $"Total : {m_lstCompany.Count}  New : {m_intadded} Exist : {m_intupdated} Failed: {m_intfailed}");
                return this.Direct(true);
            }
            
        }
        #endregion

        #region Direct Method

        public ActionResult GetCompany(string ControlCompanyID, string ControlCompanyDesc, string FilterCompanyID, string FilterCompanyDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<CompanyVM>> m_dicCompanyData = GetCompanyData(true, FilterCompanyID, FilterCompanyDesc);
                KeyValuePair<int, List<CompanyVM>> m_kvpCompanyVM = m_dicCompanyData.AsEnumerable().ToList()[0];
                if (m_kvpCompanyVM.Key < 1 || (m_kvpCompanyVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpCompanyVM.Key > 1 && !Exact)
                    return Browse(ControlCompanyID, ControlCompanyDesc, FilterCompanyID, FilterCompanyDesc);

                m_dicCompanyData = GetCompanyData(false, FilterCompanyID, FilterCompanyDesc);
                CompanyVM m_objCompanyVM = m_dicCompanyData[0][0];
                this.GetCmp<TextField>(ControlCompanyID).Value = m_objCompanyVM.CompanyID;
                this.GetCmp<TextField>(ControlCompanyDesc).Value = m_objCompanyVM.CompanyDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        

        private List<string> IsSaveValid(string Action, string CompanyID, string CompanyDesc, string CountryID, string City, string Street, string Postal)
        {
            List<string> m_lstReturn = new List<string>();

            if (CompanyID == string.Empty)
                m_lstReturn.Add(CompanyVM.Prop.CompanyID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (CompanyDesc == string.Empty)
                m_lstReturn.Add(CompanyVM.Prop.CompanyDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (CountryID == string.Empty)
                m_lstReturn.Add(CompanyVM.Prop.CountryID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (City == string.Empty)
                m_lstReturn.Add(CompanyVM.Prop.City.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (Street == string.Empty)
                m_lstReturn.Add(CompanyVM.Prop.Street.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (Postal == string.Empty)
                m_lstReturn.Add(CompanyVM.Prop.Postal.Desc + " " + General.EnumDesc(MessageLib.mustFill));

            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(CompanyVM.Prop.CompanyID.Name, parameters[CompanyVM.Prop.CompanyID.Name]);
            m_dicReturn.Add(CompanyVM.Prop.CompanyDesc.Name, parameters[CompanyVM.Prop.CompanyDesc.Name]);
            m_dicReturn.Add(CompanyVM.Prop.CountryID.Name, parameters[CompanyVM.Prop.CountryID.Name]);
            m_dicReturn.Add(CompanyVM.Prop.CountryDesc.Name, parameters[CompanyVM.Prop.CountryDesc.Name]);

            return m_dicReturn;
        }

        private CompanyVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            CompanyVM m_objCompanyVM = new CompanyVM();
            MCompanyDA m_objMCompanyDA = new MCompanyDA();
            m_objMCompanyDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(CompanyVM.Prop.CompanyID.MapAlias);
            m_lstSelect.Add(CompanyVM.Prop.CompanyDesc.MapAlias);
            m_lstSelect.Add(CompanyVM.Prop.CountryID.MapAlias);
            m_lstSelect.Add(CompanyVM.Prop.CountryDesc.MapAlias);
            m_lstSelect.Add(CompanyVM.Prop.City.MapAlias);
            m_lstSelect.Add(CompanyVM.Prop.Street.MapAlias);
            m_lstSelect.Add(CompanyVM.Prop.Postal.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objCompanyVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(CompanyVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMCompanyDA = m_objMCompanyDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMCompanyDA.Message == string.Empty)
            {
                DataRow m_drMCompanyDA = m_dicMCompanyDA[0].Tables[0].Rows[0];
                m_objCompanyVM.CompanyID = m_drMCompanyDA[CompanyVM.Prop.CompanyID.Name].ToString();
                m_objCompanyVM.CompanyDesc = m_drMCompanyDA[CompanyVM.Prop.CompanyDesc.Name].ToString();
                m_objCompanyVM.CountryID = m_drMCompanyDA[CountryVM.Prop.CountryID.Name].ToString();
                m_objCompanyVM.CountryDesc = m_drMCompanyDA[CountryVM.Prop.CountryDesc.Name].ToString();
                m_objCompanyVM.City = m_drMCompanyDA[CompanyVM.Prop.City.Name].ToString();
                m_objCompanyVM.Street = m_drMCompanyDA[CompanyVM.Prop.Street.Name].ToString();
                m_objCompanyVM.Postal = m_drMCompanyDA[CompanyVM.Prop.Postal.Name].ToString();
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMCompanyDA.Message;

            return m_objCompanyVM;
        }

        #endregion

        #region Public Method

        public Dictionary<int, List<CompanyVM>> GetCompanyData(bool isCount, string CompanyID, string CompanyDesc)
        {
            int m_intCount = 0;
            List<CompanyVM> m_lstCompanyVM = new List<ViewModels.CompanyVM>();
            Dictionary<int, List<CompanyVM>> m_dicReturn = new Dictionary<int, List<CompanyVM>>();
            MCompanyDA m_objMCompanyDA = new MCompanyDA();
            m_objMCompanyDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(CompanyVM.Prop.CompanyID.MapAlias);
            m_lstSelect.Add(CompanyVM.Prop.CompanyDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(CompanyID);
            m_objFilter.Add(CompanyVM.Prop.CompanyID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(CompanyDesc);
            m_objFilter.Add(CompanyVM.Prop.CompanyDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMCompanyDA = m_objMCompanyDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMCompanyDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpCompanyBL in m_dicMCompanyDA)
                    {
                        m_intCount = m_kvpCompanyBL.Key;
                        break;
                    }
                else
                {
                    m_lstCompanyVM = (
                        from DataRow m_drMCompanyDA in m_dicMCompanyDA[0].Tables[0].Rows
                        select new CompanyVM()
                        {
                            CompanyID = m_drMCompanyDA[CompanyVM.Prop.CompanyID.Name].ToString(),
                            CompanyDesc = m_drMCompanyDA[CompanyVM.Prop.CompanyDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstCompanyVM);
            return m_dicReturn;
        }

        #endregion
    }
}