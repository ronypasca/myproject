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
    public class CountryController : BaseController
    {
        private readonly string title = "Country";
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
            MCountryDA m_objMCountryDA = new MCountryDA();
            m_objMCountryDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMCountry = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMCountry.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = CountryVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMCountryDA = m_objMCountryDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpCountryBL in m_dicMCountryDA)
            {
                m_intCount = m_kvpCountryBL.Key;
                break;
            }

            List<CountryVM> m_lstCountryVM = new List<CountryVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(CountryVM.Prop.CountryID.MapAlias);
                m_lstSelect.Add(CountryVM.Prop.CountryDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(CountryVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMCountryDA = m_objMCountryDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMCountryDA.Message == string.Empty)
                {
                    m_lstCountryVM = (
                        from DataRow m_drMCountryDA in m_dicMCountryDA[0].Tables[0].Rows
                        select new CountryVM()
                        {
                            CountryID = m_drMCountryDA[CountryVM.Prop.CountryID.Name].ToString(),
                            CountryDesc = m_drMCountryDA[CountryVM.Prop.CountryDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstCountryVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            MCountryDA m_objMCountryDA = new MCountryDA();
            m_objMCountryDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMCountry = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMCountry.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = CountryVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMCountryDA = m_objMCountryDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpCountryBL in m_dicMCountryDA)
            {
                m_intCount = m_kvpCountryBL.Key;
                break;
            }

            List<CountryVM> m_lstCountryVM = new List<CountryVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(CountryVM.Prop.CountryID.MapAlias);
                m_lstSelect.Add(CountryVM.Prop.CountryDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(CountryVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMCountryDA = m_objMCountryDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMCountryDA.Message == string.Empty)
                {
                    m_lstCountryVM = (
                        from DataRow m_drMCountryDA in m_dicMCountryDA[0].Tables[0].Rows
                        select new CountryVM()
                        {
                            CountryID = m_drMCountryDA[CountryVM.Prop.CountryID.Name].ToString(),
                            CountryDesc = m_drMCountryDA[CountryVM.Prop.CountryDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstCountryVM, m_intCount);
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

            CountryVM m_objCountryVM = new CountryVM();
            ViewDataDictionary m_vddCountry = new ViewDataDictionary();
            m_vddCountry.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddCountry.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
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
                Model = m_objCountryVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddCountry,
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
            CountryVM m_objCountryVM = new CountryVM();
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
            if (Caller == General.EnumDesc(Buttons.ButtonList))
            {
                m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
            }
            else if (Caller == General.EnumDesc(Buttons.ButtonSave))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                m_dicSelectedRow = GetFormData(m_nvcParams);
            }

            if (m_dicSelectedRow.Count > 0)
                m_objCountryVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddCountry = new ViewDataDictionary();
            m_vddCountry.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objCountryVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddCountry,
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
            CountryVM m_objCountryVM = new CountryVM();
            if (m_dicSelectedRow.Count > 0)
                m_objCountryVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddCountry = new ViewDataDictionary();
            m_vddCountry.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddCountry.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objCountryVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddCountry,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<CountryVM> m_lstSelectedRow = new List<CountryVM>();
            m_lstSelectedRow = JSON.Deserialize<List<CountryVM>>(Selected);

            MCountryDA m_objMCountryDA = new MCountryDA();
            m_objMCountryDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (CountryVM m_objCountryVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifCountryVM = m_objCountryVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifCountryVM in m_arrPifCountryVM)
                    {
                        string m_strFieldName = m_pifCountryVM.Name;
                        object m_objFieldValue = m_pifCountryVM.GetValue(m_objCountryVM);
                        if (m_objCountryVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(CountryVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objMCountryDA.DeleteBC(m_objFilter, false);
                    if (m_objMCountryDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMCountryDA.Message);
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

        public ActionResult Browse(string ControlCountryID, string ControlCountryDesc, string FilterCountryID = "", string FilterCountryDesc = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddCountry = new ViewDataDictionary();
            m_vddCountry.Add("Control" + CountryVM.Prop.CountryID.Name, ControlCountryID);
            m_vddCountry.Add("Control" + CountryVM.Prop.CountryDesc.Name, ControlCountryDesc);
            m_vddCountry.Add(CountryVM.Prop.CountryID.Name, FilterCountryID);
            m_vddCountry.Add(CountryVM.Prop.CountryDesc.Name, FilterCountryDesc);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddCountry,
                ViewName = "../Country/_Browse"
            };
        }

        public ActionResult Save(string Action)
        {
            //action = JSON.Deserialize<string>(action);
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MCountryDA m_objMCountryDA = new MCountryDA();
            m_objMCountryDA.ConnectionStringName = Global.ConnStrConfigName;
            try
            {
                string m_strCountryID = this.Request.Params[CountryVM.Prop.CountryID.Name];
                string m_strCountryDesc = this.Request.Params[CountryVM.Prop.CountryDesc.Name];

                m_lstMessage = IsSaveValid(Action, m_strCountryID, m_strCountryDesc);
                if (m_lstMessage.Count <= 0)
                {
                    MCountry m_objMCountry = new MCountry();
                    m_objMCountry.CountryID = m_strCountryID;
                    m_objMCountryDA.Data = m_objMCountry;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objMCountryDA.Select();

                    m_objMCountry.CountryDesc = m_strCountryDesc;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objMCountryDA.Insert(false);
                    else
                        m_objMCountryDA.Update(false);

                    if (!m_objMCountryDA.Success || m_objMCountryDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMCountryDA.Message);
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

        #endregion

        #region Direct Method

        public ActionResult GetCountry(string ControlCountryID, string ControlCountryDesc, string FilterCountryID, string FilterCountryDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<CountryVM>> m_dicCountryData = GetCountryData(true, FilterCountryID, FilterCountryDesc);
                KeyValuePair<int, List<CountryVM>> m_kvpCountryVM = m_dicCountryData.AsEnumerable().ToList()[0];
                if (m_kvpCountryVM.Key < 1 || (m_kvpCountryVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpCountryVM.Key > 1 && !Exact)
                    return Browse(ControlCountryID, ControlCountryDesc, FilterCountryID, FilterCountryDesc);

                m_dicCountryData = GetCountryData(false, FilterCountryID, FilterCountryDesc);
                CountryVM m_objCountryVM = m_dicCountryData[0][0];
                this.GetCmp<TextField>(ControlCountryID).Value = m_objCountryVM.CountryID;
                this.GetCmp<TextField>(ControlCountryDesc).Value = m_objCountryVM.CountryDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, string CountryID, string CountryDesc)
        {
            List<string> m_lstReturn = new List<string>();

            if (CountryID == string.Empty)
                m_lstReturn.Add(CountryVM.Prop.CountryID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (CountryDesc == string.Empty)
                m_lstReturn.Add(CountryVM.Prop.CountryDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));

            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(CountryVM.Prop.CountryID.Name, parameters[CountryVM.Prop.CountryID.Name]);
            m_dicReturn.Add(CountryVM.Prop.CountryDesc.Name, parameters[CountryVM.Prop.CountryDesc.Name]);

            return m_dicReturn;
        }

        private CountryVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            CountryVM m_objCountryVM = new CountryVM();
            MCountryDA m_objMCountryDA = new MCountryDA();
            m_objMCountryDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(CountryVM.Prop.CountryID.MapAlias);
            m_lstSelect.Add(CountryVM.Prop.CountryDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objCountryVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(CountryVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMCountryDA = m_objMCountryDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMCountryDA.Message == string.Empty)
            {
                DataRow m_drMCountryDA = m_dicMCountryDA[0].Tables[0].Rows[0];
                m_objCountryVM.CountryID = m_drMCountryDA[CountryVM.Prop.CountryID.Name].ToString();
                m_objCountryVM.CountryDesc = m_drMCountryDA[CountryVM.Prop.CountryDesc.Name].ToString();
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMCountryDA.Message;

            return m_objCountryVM;
        }

        #endregion

        #region Public Method

        public Dictionary<int, List<CountryVM>> GetCountryData(bool isCount, string CountryID, string CountryDesc)
        {
            int m_intCount = 0;
            List<CountryVM> m_lstCountryVM = new List<ViewModels.CountryVM>();
            Dictionary<int, List<CountryVM>> m_dicReturn = new Dictionary<int, List<CountryVM>>();
            MCountryDA m_objMCountryDA = new MCountryDA();
            m_objMCountryDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(CountryVM.Prop.CountryID.MapAlias);
            m_lstSelect.Add(CountryVM.Prop.CountryDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(CountryID);
            m_objFilter.Add(CountryVM.Prop.CountryID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(CountryDesc);
            m_objFilter.Add(CountryVM.Prop.CountryDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMCountryDA = m_objMCountryDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMCountryDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpCountryBL in m_dicMCountryDA)
                    {
                        m_intCount = m_kvpCountryBL.Key;
                        break;
                    }
                else
                {
                    m_lstCountryVM = (
                        from DataRow m_drMCountryDA in m_dicMCountryDA[0].Tables[0].Rows
                        select new CountryVM()
                        {
                            CountryID = m_drMCountryDA[CountryVM.Prop.CountryID.Name].ToString(),
                            CountryDesc = m_drMCountryDA[CountryVM.Prop.CountryDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstCountryVM);
            return m_dicReturn;
        }

        #endregion
    }
}