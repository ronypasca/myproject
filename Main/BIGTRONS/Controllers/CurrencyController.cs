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
    public class CurrencyController : BaseController
    {
        private readonly string title = "Currency";
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
            MCurrencyDA m_objMCurrencyDA = new MCurrencyDA();
            m_objMCurrencyDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMCurrency = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMCurrency.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = CurrencyVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMCurrencyDA = m_objMCurrencyDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpCurrencyBL in m_dicMCurrencyDA)
            {
                m_intCount = m_kvpCurrencyBL.Key;
                break;
            }

            List<CurrencyVM> m_lstCurrencyVM = new List<CurrencyVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(CurrencyVM.Prop.CurrencyID.MapAlias);
                m_lstSelect.Add(CurrencyVM.Prop.CurrencyDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(CurrencyVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMCurrencyDA = m_objMCurrencyDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMCurrencyDA.Message == string.Empty)
                {
                    m_lstCurrencyVM = (
                        from DataRow m_drMCurrencyDA in m_dicMCurrencyDA[0].Tables[0].Rows
                        select new CurrencyVM()
                        {
                            CurrencyID = m_drMCurrencyDA[CurrencyVM.Prop.CurrencyID.Name].ToString(),
                            CurrencyDesc = m_drMCurrencyDA[CurrencyVM.Prop.CurrencyDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstCurrencyVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            MCurrencyDA m_objMCurrencyDA = new MCurrencyDA();
            m_objMCurrencyDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMCurrency = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMCurrency.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = CurrencyVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMCurrencyDA = m_objMCurrencyDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpCurrencyBL in m_dicMCurrencyDA)
            {
                m_intCount = m_kvpCurrencyBL.Key;
                break;
            }

            List<CurrencyVM> m_lstCurrencyVM = new List<CurrencyVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(CurrencyVM.Prop.CurrencyID.MapAlias);
                m_lstSelect.Add(CurrencyVM.Prop.CurrencyDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(CurrencyVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMCurrencyDA = m_objMCurrencyDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMCurrencyDA.Message == string.Empty)
                {
                    m_lstCurrencyVM = (
                        from DataRow m_drMCurrencyDA in m_dicMCurrencyDA[0].Tables[0].Rows
                        select new CurrencyVM()
                        {
                            CurrencyID = m_drMCurrencyDA[CurrencyVM.Prop.CurrencyID.Name].ToString(),
                            CurrencyDesc = m_drMCurrencyDA[CurrencyVM.Prop.CurrencyDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstCurrencyVM, m_intCount);
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

            CurrencyVM m_objCurrencyVM = new CurrencyVM();
            ViewDataDictionary m_vddCurrency = new ViewDataDictionary();
            m_vddCurrency.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddCurrency.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            if (Caller == General.EnumDesc(Buttons.ButtonDetail))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                Dictionary<string, object> m_dicSelectedRow = GetFormData(m_nvcParams);
                Session[dataSessionName] = JSON.Serialize(m_dicSelectedRow);
            }
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objCurrencyVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddCurrency,
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

            
            CurrencyVM m_objCurrencyVM = new CurrencyVM();
            if (m_dicSelectedRow.Count > 0)
                m_objCurrencyVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
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
                Model = m_objCurrencyVM,
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
            CurrencyVM m_objCurrencyVM = new CurrencyVM();
            if (m_dicSelectedRow.Count > 0)
                m_objCurrencyVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddCurrency = new ViewDataDictionary();
            m_vddCurrency.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddCurrency.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objCurrencyVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddCurrency,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<CurrencyVM> m_lstSelectedRow = new List<CurrencyVM>();
            m_lstSelectedRow = JSON.Deserialize<List<CurrencyVM>>(Selected);

            MCurrencyDA m_objMCurrencyDA = new MCurrencyDA();
            m_objMCurrencyDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (CurrencyVM m_objCurrencyVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifCurrencyVM = m_objCurrencyVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifCurrencyVM in m_arrPifCurrencyVM)
                    {
                        string m_strFieldName = m_pifCurrencyVM.Name;
                        object m_objFieldValue = m_pifCurrencyVM.GetValue(m_objCurrencyVM);
                        if (m_objCurrencyVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(CurrencyVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objMCurrencyDA.DeleteBC(m_objFilter, false);
                    if (m_objMCurrencyDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMCurrencyDA.Message);
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

        public ActionResult Browse(string ControlCurrencyID, string ControlCurrencyDesc, string FilterCurrencyID = "", string FilterCurrencyDesc = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddCurrency = new ViewDataDictionary();
            m_vddCurrency.Add("Control" + CurrencyVM.Prop.CurrencyID.Name, ControlCurrencyID);
            m_vddCurrency.Add("Control" + CurrencyVM.Prop.CurrencyDesc.Name, ControlCurrencyDesc);
            m_vddCurrency.Add(CurrencyVM.Prop.CurrencyID.Name, FilterCurrencyID);
            m_vddCurrency.Add(CurrencyVM.Prop.CurrencyDesc.Name, FilterCurrencyDesc);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddCurrency,
                ViewName = "../Currency/_Browse"
            };
        }

        public ActionResult Save(string Action)
        {
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MCurrencyDA m_objMCurrencyDA = new MCurrencyDA();
            m_objMCurrencyDA.ConnectionStringName = Global.ConnStrConfigName;
            try
            {
                string m_strCurrencyID = this.Request.Params[CurrencyVM.Prop.CurrencyID.Name];
                string m_strCurrencyDesc = this.Request.Params[CurrencyVM.Prop.CurrencyDesc.Name];

                m_lstMessage = IsSaveValid(Action, m_strCurrencyID, m_strCurrencyDesc);
                if (m_lstMessage.Count <= 0)
                {
                    MCurrency m_objMCurrency = new MCurrency();
                    m_objMCurrency.CurrencyID = m_strCurrencyID;
                    m_objMCurrencyDA.Data = m_objMCurrency;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objMCurrencyDA.Select();

                    m_objMCurrency.CurrencyDesc = m_strCurrencyDesc;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objMCurrencyDA.Insert(false);
                    else
                        m_objMCurrencyDA.Update(false);

                    if (!m_objMCurrencyDA.Success || m_objMCurrencyDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMCurrencyDA.Message);
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

        #endregion

        #region Direct Method

        public ActionResult GetCurrency(string ControlCurrencyID, string ControlCurrencyDesc, string FilterCurrencyID, string FilterCurrencyDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<CurrencyVM>> m_dicCurrencyData = GetCurrencyData(true, FilterCurrencyID, FilterCurrencyDesc);
                KeyValuePair<int, List<CurrencyVM>> m_kvpCurrencyVM = m_dicCurrencyData.AsEnumerable().ToList()[0];
                if (m_kvpCurrencyVM.Key < 1 || (m_kvpCurrencyVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpCurrencyVM.Key > 1 && !Exact)
                    return Browse(ControlCurrencyID, ControlCurrencyDesc, FilterCurrencyID, FilterCurrencyDesc);

                m_dicCurrencyData = GetCurrencyData(false, FilterCurrencyID, FilterCurrencyDesc);
                CurrencyVM m_objCurrencyVM = m_dicCurrencyData[0][0];
                this.GetCmp<TextField>(ControlCurrencyID).Value = m_objCurrencyVM.CurrencyID;
                this.GetCmp<TextField>(ControlCurrencyDesc).Value = m_objCurrencyVM.CurrencyDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, string CurrencyID, string CurrencyDesc)
        {
            List<string> m_lstReturn = new List<string>();

            if (CurrencyID == string.Empty)
                m_lstReturn.Add(CurrencyVM.Prop.CurrencyID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (CurrencyDesc == string.Empty)
                m_lstReturn.Add(CurrencyVM.Prop.CurrencyDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));

            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(CurrencyVM.Prop.CurrencyID.Name, parameters[CurrencyVM.Prop.CurrencyID.Name]);
            m_dicReturn.Add(CurrencyVM.Prop.CurrencyDesc.Name, parameters[CurrencyVM.Prop.CurrencyDesc.Name]);

            return m_dicReturn;
        }

        private CurrencyVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            CurrencyVM m_objCurrencyVM = new CurrencyVM();
            MCurrencyDA m_objMCurrencyDA = new MCurrencyDA();
            m_objMCurrencyDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(CurrencyVM.Prop.CurrencyID.MapAlias);
            m_lstSelect.Add(CurrencyVM.Prop.CurrencyDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objCurrencyVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(CurrencyVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMCurrencyDA = m_objMCurrencyDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMCurrencyDA.Message == string.Empty)
            {
                DataRow m_drMCurrencyDA = m_dicMCurrencyDA[0].Tables[0].Rows[0];
                m_objCurrencyVM.CurrencyID = m_drMCurrencyDA[CurrencyVM.Prop.CurrencyID.Name].ToString();
                m_objCurrencyVM.CurrencyDesc = m_drMCurrencyDA[CurrencyVM.Prop.CurrencyDesc.Name].ToString();
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMCurrencyDA.Message;

            return m_objCurrencyVM;
        }

        #endregion

        #region Public Method

        public Dictionary<int, List<CurrencyVM>> GetCurrencyData(bool isCount, string CurrencyID, string CurrencyDesc)
        {
            int m_intCount = 0;
            List<CurrencyVM> m_lstCurrencyVM = new List<ViewModels.CurrencyVM>();
            Dictionary<int, List<CurrencyVM>> m_dicReturn = new Dictionary<int, List<CurrencyVM>>();
            MCurrencyDA m_objMCurrencyDA = new MCurrencyDA();
            m_objMCurrencyDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(CurrencyVM.Prop.CurrencyID.MapAlias);
            m_lstSelect.Add(CurrencyVM.Prop.CurrencyDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(CurrencyID);
            m_objFilter.Add(CurrencyVM.Prop.CurrencyID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(CurrencyDesc);
            m_objFilter.Add(CurrencyVM.Prop.CurrencyDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMCurrencyDA = m_objMCurrencyDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMCurrencyDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpCurrencyBL in m_dicMCurrencyDA)
                    {
                        m_intCount = m_kvpCurrencyBL.Key;
                        break;
                    }
                else
                {
                    m_lstCurrencyVM = (
                        from DataRow m_drMCurrencyDA in m_dicMCurrencyDA[0].Tables[0].Rows
                        select new CurrencyVM()
                        {
                            CurrencyID = m_drMCurrencyDA[CurrencyVM.Prop.CurrencyID.Name].ToString(),
                            CurrencyDesc = m_drMCurrencyDA[CurrencyVM.Prop.CurrencyDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstCurrencyVM);
            return m_dicReturn;
        }

        #endregion
    }
}