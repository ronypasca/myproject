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
using System.Xml;

namespace com.SML.BIGTRONS.Controllers
{
    public class DivisionController : BaseController
    {
        private readonly string title = "Division";
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
            MDivisionDA m_objMDivisionDA = new MDivisionDA();
            m_objMDivisionDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMDivision = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMDivision.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = DivisionVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMDivisionDA = m_objMDivisionDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpDivisionBL in m_dicMDivisionDA)
            {
                m_intCount = m_kvpDivisionBL.Key;
                break;
            }

            List<DivisionVM> m_lstDivisionVM = new List<DivisionVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(DivisionVM.Prop.DivisionID.MapAlias);
                m_lstSelect.Add(DivisionVM.Prop.DivisionDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(DivisionVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMDivisionDA = m_objMDivisionDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMDivisionDA.Message == string.Empty)
                {
                    m_lstDivisionVM = (
                        from DataRow m_drMDivisionDA in m_dicMDivisionDA[0].Tables[0].Rows
                        select new DivisionVM()
                        {
                            DivisionID = m_drMDivisionDA[DivisionVM.Prop.DivisionID.Name].ToString(),
                            DivisionDesc = m_drMDivisionDA[DivisionVM.Prop.DivisionDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstDivisionVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters, string BusinessUnitID)
        {
            MDivisionDA m_objMDivisionDA = new MDivisionDA();
            m_objMDivisionDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMDivision = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMDivision.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = DivisionVM.Prop.Map(m_strDataIndex, false);
                    
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
            if (!string.IsNullOrEmpty(BusinessUnitID))
            {
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(BusinessUnitID);
                m_objFilter.Add(DivisionVM.Prop.BusinessUnitID.Map, m_lstFilter);
            }

            Dictionary<int, DataSet> m_dicMDivisionDA = m_objMDivisionDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpDivisionBL in m_dicMDivisionDA)
            {
                m_intCount = m_kvpDivisionBL.Key;
                break;
            }

            List<DivisionVM> m_lstDivisionVM = new List<DivisionVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(DivisionVM.Prop.DivisionID.MapAlias);
                m_lstSelect.Add(DivisionVM.Prop.DivisionDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(DivisionVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMDivisionDA = m_objMDivisionDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMDivisionDA.Message == string.Empty)
                {
                    m_lstDivisionVM = (
                        from DataRow m_drMDivisionDA in m_dicMDivisionDA[0].Tables[0].Rows
                        select new DivisionVM()
                        {
                            DivisionID = m_drMDivisionDA[DivisionVM.Prop.DivisionID.Name].ToString(),
                            DivisionDesc = m_drMDivisionDA[DivisionVM.Prop.DivisionDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstDivisionVM, m_intCount);
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

            DivisionVM m_objDivisionVM = new DivisionVM();
            ViewDataDictionary m_vddDivision = new ViewDataDictionary();
            m_vddDivision.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddDivision.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
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
                Model = m_objDivisionVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddDivision,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Detail(string Caller, string Selected)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            DivisionVM m_objDivisionVM = new DivisionVM();
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
                m_objDivisionVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
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
                Model = m_objDivisionVM,
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
            DivisionVM m_objDivisionVM = new DivisionVM();
            if (m_dicSelectedRow.Count > 0)
                m_objDivisionVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddDivision = new ViewDataDictionary();
            m_vddDivision.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddDivision.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objDivisionVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddDivision,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<DivisionVM> m_lstSelectedRow = new List<DivisionVM>();
            m_lstSelectedRow = JSON.Deserialize<List<DivisionVM>>(Selected);

            MDivisionDA m_objMDivisionDA = new MDivisionDA();
            m_objMDivisionDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (DivisionVM m_objDivisionVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifDivisionVM = m_objDivisionVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifDivisionVM in m_arrPifDivisionVM)
                    {
                        string m_strFieldName = m_pifDivisionVM.Name;
                        object m_objFieldValue = m_pifDivisionVM.GetValue(m_objDivisionVM);
                        if (m_objDivisionVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(DivisionVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objMDivisionDA.DeleteBC(m_objFilter, false);
                    if (m_objMDivisionDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMDivisionDA.Message);
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

        public ActionResult Browse(string ControlDivisionID, string ControlDivisionDesc, string FilterDivisionID = "", string FilterDivisionDesc = "", string FilterBusinessUnitID = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddDivision = new ViewDataDictionary();
            m_vddDivision.Add("Control" + DivisionVM.Prop.DivisionID.Name, ControlDivisionID);
            m_vddDivision.Add("Control" + DivisionVM.Prop.DivisionDesc.Name, ControlDivisionDesc);
            m_vddDivision.Add(DivisionVM.Prop.DivisionID.Name, FilterDivisionID);
            m_vddDivision.Add(DivisionVM.Prop.DivisionDesc.Name, FilterDivisionDesc);
            m_vddDivision.Add(BusinessUnitVM.Prop.BusinessUnitID.Name, FilterBusinessUnitID);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddDivision,
                ViewName = "../Division/_Browse"
            };
        }

        public ActionResult Save(string Action)
        {
            //action = JSON.Deserialize<string>(action);
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MDivisionDA m_objMDivisionDA = new MDivisionDA();
            m_objMDivisionDA.ConnectionStringName = Global.ConnStrConfigName;
            try
            {
                string m_strDivisionID = this.Request.Params[DivisionVM.Prop.DivisionID.Name];
                string m_strDivisionDesc = this.Request.Params[DivisionVM.Prop.DivisionDesc.Name];

                m_lstMessage = IsSaveValid(Action, m_strDivisionID, m_strDivisionDesc);
                if (m_lstMessage.Count <= 0)
                {
                    MDivision m_objMDivision = new MDivision();
                    m_objMDivision.DivisionID = m_strDivisionID;
                    m_objMDivisionDA.Data = m_objMDivision;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objMDivisionDA.Select();

                    m_objMDivision.DivisionDesc = m_strDivisionDesc;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objMDivisionDA.Insert(false);
                    else
                        m_objMDivisionDA.Update(false);

                    if (!m_objMDivisionDA.Success || m_objMDivisionDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMDivisionDA.Message);
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
    <Division xmlns='http://tempuri.org/' />
  </soap:Body>
</soap:Envelope>";

            
            var m_lstconfig = GetConfig("WS", null, "ETT");
            if (!m_lstconfig.Any())
            {
                return this.Direct(false);
            }
            string m_struser = m_lstconfig.Any(x => x.Key2 == "Credential" && x.Key4 == "User") ? m_lstconfig.Where(x => x.Key2 == "Credential" && x.Key4 == "User").FirstOrDefault().Desc1 : string.Empty;
            string m_strpass = m_lstconfig.Any(x => x.Key2 == "Credential" && x.Key4 == "Password") ? m_lstconfig.Where(x => x.Key2 == "Credential" && x.Key4 == "Password").FirstOrDefault().Desc1 : string.Empty;
            string m_strdomain = m_lstconfig.Any(x => x.Key2 == "Credential" && x.Key4 == "Domain") ? m_lstconfig.Where(x => x.Key2 == "Credential" && x.Key4 == "Domain").FirstOrDefault().Desc1 : string.Empty;
            string m_strurl = m_lstconfig.Any(x => x.Key2 == "URL" && x.Key4 == "Division") ? m_lstconfig.Where(x => x.Key2 == "URL" && x.Key4 == "Division").FirstOrDefault().Desc1 : string.Empty;
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
            List<MDivision> m_lstDivision = new List<MDivision>();

            foreach (XmlNode item in xnList)
            {
                MDivision m_Division = new MDivision();
                m_Division.DivisionID = item["ID"].InnerText;
                m_Division.DivisionDesc = item["BusinessAreaName"].InnerText;
                //m_Division.BusinessUnitID = item["FIDProjectLocation"].InnerText;
                m_lstDivision.Add(m_Division);
            }

            if (!m_lstDivision.Any())
            {
                return this.Direct(false, "Error Read Data");
            }
            else
            {
                MDivisionDA m_objMDivisionDA = new MDivisionDA();
                m_objMDivisionDA.ConnectionStringName = Global.ConnStrConfigName;
                foreach (var item in m_lstDivision)
                {
                    try
                    {
                        
                            m_objMDivisionDA.Data = item;
                            m_objMDivisionDA.Insert(false);
                            if (!m_objMDivisionDA.Success || m_objMDivisionDA.Message != string.Empty)
                                                        {
                            if (m_objMDivisionDA.Message == "Cannot insert duplicate data.")
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
                Global.ShowInfoAlert(title, $"Total : {m_lstDivision.Count}  New : {m_intadded} Exist : {m_intupdated} Failed: {m_intfailed}");
                return this.Direct(true);
            }

        }

        #endregion

        #region Direct Method

        public ActionResult GetDivision(string ControlDivisionID, string ControlDivisionDesc, string FilterDivisionID, string FilterDivisionDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<DivisionVM>> m_dicDivisionData = GetDivisionData(true, FilterDivisionID, FilterDivisionDesc);
                KeyValuePair<int, List<DivisionVM>> m_kvpDivisionVM = m_dicDivisionData.AsEnumerable().ToList()[0];
                if (m_kvpDivisionVM.Key < 1 || (m_kvpDivisionVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpDivisionVM.Key > 1 && !Exact)
                    return Browse(ControlDivisionID, ControlDivisionDesc, FilterDivisionID, FilterDivisionDesc);

                m_dicDivisionData = GetDivisionData(false, FilterDivisionID, FilterDivisionDesc);
                DivisionVM m_objDivisionVM = m_dicDivisionData[0][0];
                this.GetCmp<TextField>(ControlDivisionID).Value = m_objDivisionVM.DivisionID;
                this.GetCmp<TextField>(ControlDivisionDesc).Value = m_objDivisionVM.DivisionDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, string DivisionID, string DivisionDesc)
        {
            List<string> m_lstReturn = new List<string>();

            if (DivisionID == string.Empty)
                m_lstReturn.Add(DivisionVM.Prop.DivisionID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (DivisionDesc == string.Empty)
                m_lstReturn.Add(DivisionVM.Prop.DivisionDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));

            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(DivisionVM.Prop.DivisionID.Name, parameters[DivisionVM.Prop.DivisionID.Name]);
            m_dicReturn.Add(DivisionVM.Prop.DivisionDesc.Name, parameters[DivisionVM.Prop.DivisionDesc.Name]);

            return m_dicReturn;
        }

        private DivisionVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            DivisionVM m_objDivisionVM = new DivisionVM();
            MDivisionDA m_objMDivisionDA = new MDivisionDA();
            m_objMDivisionDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(DivisionVM.Prop.DivisionID.MapAlias);
            m_lstSelect.Add(DivisionVM.Prop.DivisionDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objDivisionVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(DivisionVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMDivisionDA = m_objMDivisionDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMDivisionDA.Message == string.Empty)
            {
                DataRow m_drMDivisionDA = m_dicMDivisionDA[0].Tables[0].Rows[0];
                m_objDivisionVM.DivisionID = m_drMDivisionDA[DivisionVM.Prop.DivisionID.Name].ToString();
                m_objDivisionVM.DivisionDesc = m_drMDivisionDA[DivisionVM.Prop.DivisionDesc.Name].ToString();
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMDivisionDA.Message;

            return m_objDivisionVM;
        }

        #endregion

        #region Public Method

        public Dictionary<int, List<DivisionVM>> GetDivisionData(bool isCount, string DivisionID, string DivisionDesc)
        {
            int m_intCount = 0;
            List<DivisionVM> m_lstDivisionVM = new List<ViewModels.DivisionVM>();
            Dictionary<int, List<DivisionVM>> m_dicReturn = new Dictionary<int, List<DivisionVM>>();
            MDivisionDA m_objMDivisionDA = new MDivisionDA();
            m_objMDivisionDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(DivisionVM.Prop.DivisionID.MapAlias);
            m_lstSelect.Add(DivisionVM.Prop.DivisionDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(DivisionID);
            m_objFilter.Add(DivisionVM.Prop.DivisionID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(DivisionDesc);
            m_objFilter.Add(DivisionVM.Prop.DivisionDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMDivisionDA = m_objMDivisionDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMDivisionDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpDivisionBL in m_dicMDivisionDA)
                    {
                        m_intCount = m_kvpDivisionBL.Key;
                        break;
                    }
                else
                {
                    m_lstDivisionVM = (
                        from DataRow m_drMDivisionDA in m_dicMDivisionDA[0].Tables[0].Rows
                        select new DivisionVM()
                        {
                            DivisionID = m_drMDivisionDA[DivisionVM.Prop.DivisionID.Name].ToString(),
                            DivisionDesc = m_drMDivisionDA[DivisionVM.Prop.DivisionDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstDivisionVM);
            return m_dicReturn;
        }

        #endregion
    }
}