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
    public class NegotiationConfigTypesController : BaseController
    {
        private readonly string title = "Negotiation Config Types";
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
            SNegotiationConfigTypesDA m_objSNegotiationConfigTypesDA = new SNegotiationConfigTypesDA();
            m_objSNegotiationConfigTypesDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcSNegotiationConfigTypes = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcSNegotiationConfigTypes.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = NegotiationConfigTypesVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicSNegotiationConfigTypesDA = m_objSNegotiationConfigTypesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpNegotiationConfigTypesBL in m_dicSNegotiationConfigTypesDA)
            {
                m_intCount = m_kvpNegotiationConfigTypesBL.Key;
                break;
            }

            List<NegotiationConfigTypesVM> m_lstNegotiationConfigTypesVM = new List<NegotiationConfigTypesVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(NegotiationConfigTypesVM.Prop.NegotiationConfigTypeID.MapAlias);
                m_lstSelect.Add(NegotiationConfigTypesVM.Prop.Descriptions.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(NegotiationConfigTypesVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicSNegotiationConfigTypesDA = m_objSNegotiationConfigTypesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objSNegotiationConfigTypesDA.Message == string.Empty)
                {
                    m_lstNegotiationConfigTypesVM = (
                        from DataRow m_drSNegotiationConfigTypesDA in m_dicSNegotiationConfigTypesDA[0].Tables[0].Rows
                        select new NegotiationConfigTypesVM()
                        {
                            NegotiationConfigTypeID = m_drSNegotiationConfigTypesDA[NegotiationConfigTypesVM.Prop.NegotiationConfigTypeID.Name].ToString(),
                            Descriptions = m_drSNegotiationConfigTypesDA[NegotiationConfigTypesVM.Prop.Descriptions.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstNegotiationConfigTypesVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            SNegotiationConfigTypesDA m_objSNegotiationConfigTypesDA = new SNegotiationConfigTypesDA();
            m_objSNegotiationConfigTypesDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcSNegotiationConfigTypes = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcSNegotiationConfigTypes.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = NegotiationConfigTypesVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicSNegotiationConfigTypesDA = m_objSNegotiationConfigTypesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpNegotiationConfigTypesBL in m_dicSNegotiationConfigTypesDA)
            {
                m_intCount = m_kvpNegotiationConfigTypesBL.Key;
                break;
            }

            List<NegotiationConfigTypesVM> m_lstNegotiationConfigTypesVM = new List<NegotiationConfigTypesVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(NegotiationConfigTypesVM.Prop.NegotiationConfigTypeID.MapAlias);
                m_lstSelect.Add(NegotiationConfigTypesVM.Prop.Descriptions.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(NegotiationConfigTypesVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicSNegotiationConfigTypesDA = m_objSNegotiationConfigTypesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objSNegotiationConfigTypesDA.Message == string.Empty)
                {
                    m_lstNegotiationConfigTypesVM = (
                        from DataRow m_drSNegotiationConfigTypesDA in m_dicSNegotiationConfigTypesDA[0].Tables[0].Rows
                        select new NegotiationConfigTypesVM()
                        {
                            NegotiationConfigTypeID = m_drSNegotiationConfigTypesDA[NegotiationConfigTypesVM.Prop.NegotiationConfigTypeID.Name].ToString(),
                            Descriptions = m_drSNegotiationConfigTypesDA[NegotiationConfigTypesVM.Prop.Descriptions.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstNegotiationConfigTypesVM, m_intCount);
        }

        public ActionResult Home()
        {
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 0;
            return this.Direct(true);
        }

        public ActionResult Add(string Caller)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Add)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            NegotiationConfigTypesVM m_objNegotiationConfigTypesVM = new NegotiationConfigTypesVM();
            ViewDataDictionary m_vddNegotiationConfigTypes = new ViewDataDictionary();
            m_vddNegotiationConfigTypes.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddNegotiationConfigTypes.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
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
                Model = m_objNegotiationConfigTypesVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddNegotiationConfigTypes,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Detail(string Caller, string Selected, string NegotiationConfigTypeID)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            NegotiationConfigTypesVM m_objNegotiationConfigTypesVM = new NegotiationConfigTypesVM();
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
                m_dicSelectedRow = GetFormData(m_nvcParams, NegotiationConfigTypeID);
            }

            if (m_dicSelectedRow.Count > 0)
                m_objNegotiationConfigTypesVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
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
                Model = m_objNegotiationConfigTypesVM,
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
            NegotiationConfigTypesVM m_objNegotiationConfigTypesVM = new NegotiationConfigTypesVM();
            if (m_dicSelectedRow.Count > 0)
                m_objNegotiationConfigTypesVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddNegotiationConfigTypes = new ViewDataDictionary();
            m_vddNegotiationConfigTypes.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddNegotiationConfigTypes.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objNegotiationConfigTypesVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddNegotiationConfigTypes,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<NegotiationConfigTypesVM> m_lstSelectedRow = new List<NegotiationConfigTypesVM>();
            m_lstSelectedRow = JSON.Deserialize<List<NegotiationConfigTypesVM>>(Selected);

            SNegotiationConfigTypesDA m_objSNegotiationConfigTypesDA = new SNegotiationConfigTypesDA();
            m_objSNegotiationConfigTypesDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (NegotiationConfigTypesVM m_objNegotiationConfigTypesVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifNegotiationConfigTypesVM = m_objNegotiationConfigTypesVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifNegotiationConfigTypesVM in m_arrPifNegotiationConfigTypesVM)
                    {
                        string m_strFieldName = m_pifNegotiationConfigTypesVM.Name;
                        object m_objFieldValue = m_pifNegotiationConfigTypesVM.GetValue(m_objNegotiationConfigTypesVM);
                        if (m_objNegotiationConfigTypesVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(NegotiationConfigTypesVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objSNegotiationConfigTypesDA.DeleteBC(m_objFilter, false);
                    if (m_objSNegotiationConfigTypesDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objSNegotiationConfigTypesDA.Message);
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

        public ActionResult Browse(string ControlNegotiationConfigTypeID, string ControlDescriptions, string FilterNegotiationConfigTypeID = "", string FilterDescriptions = "")
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddNegotiationConfigTypes = new ViewDataDictionary();
            m_vddNegotiationConfigTypes.Add("Control" + NegotiationConfigTypesVM.Prop.NegotiationConfigTypeID.Name, ControlNegotiationConfigTypeID);
            m_vddNegotiationConfigTypes.Add("Control" + NegotiationConfigTypesVM.Prop.Descriptions.Name, ControlDescriptions);
            m_vddNegotiationConfigTypes.Add(NegotiationConfigTypesVM.Prop.NegotiationConfigTypeID.Name, FilterNegotiationConfigTypeID);
            m_vddNegotiationConfigTypes.Add(NegotiationConfigTypesVM.Prop.Descriptions.Name, FilterDescriptions);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddNegotiationConfigTypes,
                ViewName = "../NegotiationConfigTypes/_Browse"
            };
        }

        public ActionResult Save(string Action)
        {
            //action = JSON.Deserialize<string>(action);
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            SNegotiationConfigTypesDA m_objSNegotiationConfigTypesDA = new SNegotiationConfigTypesDA();
            string m_strNegotiationConfigTypeID = string.Empty;
            m_objSNegotiationConfigTypesDA.ConnectionStringName = Global.ConnStrConfigName;
            try
            {
                m_strNegotiationConfigTypeID = this.Request.Params[NegotiationConfigTypesVM.Prop.NegotiationConfigTypeID.Name];
                string m_strDescriptions = this.Request.Params[NegotiationConfigTypesVM.Prop.Descriptions.Name];

                m_lstMessage = IsSaveValid(Action, m_strNegotiationConfigTypeID, m_strDescriptions);
                if (m_lstMessage.Count <= 0)
                {
                    SNegotiationConfigTypes m_objSNegotiationConfigTypes = new SNegotiationConfigTypes();
                    m_objSNegotiationConfigTypes.NegotiationConfigTypeID = m_strNegotiationConfigTypeID;
                    m_objSNegotiationConfigTypesDA.Data = m_objSNegotiationConfigTypes;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objSNegotiationConfigTypesDA.Select();

                    m_objSNegotiationConfigTypes.Descriptions = m_strDescriptions;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objSNegotiationConfigTypesDA.Insert(false);
                    else
                        m_objSNegotiationConfigTypesDA.Update(false);

                    if (!m_objSNegotiationConfigTypesDA.Success || m_objSNegotiationConfigTypesDA.Message != string.Empty)
                        m_lstMessage.Add(m_objSNegotiationConfigTypesDA.Message);
                    m_strNegotiationConfigTypeID = m_objSNegotiationConfigTypesDA.Data.NegotiationConfigTypeID;
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
            }
            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
                return Detail(General.EnumDesc(Buttons.ButtonSave), null, m_strNegotiationConfigTypeID);
            }
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);
        }

        #endregion

        #region Direct Method

        public ActionResult GetNegotiationConfigTypes(string ControlNegotiationConfigTypeID, string ControlDescriptions, string FilterNegotiationConfigTypeID, string FilterDescriptions, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<NegotiationConfigTypesVM>> m_dicNegotiationConfigTypesData = GetNegotiationConfigTypesData(true, FilterNegotiationConfigTypeID, FilterDescriptions);
                KeyValuePair<int, List<NegotiationConfigTypesVM>> m_kvpNegotiationConfigTypesVM = m_dicNegotiationConfigTypesData.AsEnumerable().ToList()[0];
                if (m_kvpNegotiationConfigTypesVM.Key < 1 || (m_kvpNegotiationConfigTypesVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpNegotiationConfigTypesVM.Key > 1 && !Exact)
                    return Browse(ControlNegotiationConfigTypeID, ControlDescriptions, FilterNegotiationConfigTypeID, FilterDescriptions);

                m_dicNegotiationConfigTypesData = GetNegotiationConfigTypesData(false, FilterNegotiationConfigTypeID, FilterDescriptions);
                NegotiationConfigTypesVM m_objNegotiationConfigTypesVM = m_dicNegotiationConfigTypesData[0][0];
                this.GetCmp<TextField>(ControlNegotiationConfigTypeID).Value = m_objNegotiationConfigTypesVM.NegotiationConfigTypeID;
                this.GetCmp<TextField>(ControlDescriptions).Value = m_objNegotiationConfigTypesVM.Descriptions;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, string NegotiationConfigTypeID, string Descriptions)
        {
            List<string> m_lstReturn = new List<string>();

            //if (NegotiationConfigTypeID == string.Empty)
            //    m_lstReturn.Add(NegotiationConfigTypesVM.Prop.NegotiationConfigTypeID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (Descriptions == string.Empty)
                m_lstReturn.Add(NegotiationConfigTypesVM.Prop.Descriptions.Desc + " " + General.EnumDesc(MessageLib.mustFill));

            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters, string NegotiationConfigTypeID = "")
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();
            m_dicReturn.Add(NegotiationConfigTypesVM.Prop.NegotiationConfigTypeID.Name, (parameters[NegotiationConfigTypesVM.Prop.NegotiationConfigTypeID.Name].ToString() == string.Empty ? NegotiationConfigTypeID : parameters[NegotiationConfigTypesVM.Prop.NegotiationConfigTypeID.Name]));
            m_dicReturn.Add(NegotiationConfigTypesVM.Prop.Descriptions.Name, parameters[NegotiationConfigTypesVM.Prop.Descriptions.Name]);

            return m_dicReturn;
        }

        private NegotiationConfigTypesVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            NegotiationConfigTypesVM m_objNegotiationConfigTypesVM = new NegotiationConfigTypesVM();
            SNegotiationConfigTypesDA m_objSNegotiationConfigTypesDA = new SNegotiationConfigTypesDA();
            m_objSNegotiationConfigTypesDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(NegotiationConfigTypesVM.Prop.NegotiationConfigTypeID.MapAlias);
            m_lstSelect.Add(NegotiationConfigTypesVM.Prop.Descriptions.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objNegotiationConfigTypesVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(NegotiationConfigTypesVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicSNegotiationConfigTypesDA = m_objSNegotiationConfigTypesDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objSNegotiationConfigTypesDA.Message == string.Empty)
            {
                DataRow m_drSNegotiationConfigTypesDA = m_dicSNegotiationConfigTypesDA[0].Tables[0].Rows[0];
                m_objNegotiationConfigTypesVM.NegotiationConfigTypeID = m_drSNegotiationConfigTypesDA[NegotiationConfigTypesVM.Prop.NegotiationConfigTypeID.Name].ToString();
                m_objNegotiationConfigTypesVM.Descriptions = m_drSNegotiationConfigTypesDA[NegotiationConfigTypesVM.Prop.Descriptions.Name].ToString();
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objSNegotiationConfigTypesDA.Message;

            return m_objNegotiationConfigTypesVM;
        }

        #endregion

        #region Public Method

        public Dictionary<int, List<NegotiationConfigTypesVM>> GetNegotiationConfigTypesData(bool isCount, string NegotiationConfigTypeID, string Descriptions)
        {
            int m_intCount = 0;
            List<NegotiationConfigTypesVM> m_lstNegotiationConfigTypesVM = new List<ViewModels.NegotiationConfigTypesVM>();
            Dictionary<int, List<NegotiationConfigTypesVM>> m_dicReturn = new Dictionary<int, List<NegotiationConfigTypesVM>>();
            SNegotiationConfigTypesDA m_objSNegotiationConfigTypesDA = new SNegotiationConfigTypesDA();
            m_objSNegotiationConfigTypesDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(NegotiationConfigTypesVM.Prop.NegotiationConfigTypeID.MapAlias);
            m_lstSelect.Add(NegotiationConfigTypesVM.Prop.Descriptions.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(NegotiationConfigTypeID);
            m_objFilter.Add(NegotiationConfigTypesVM.Prop.NegotiationConfigTypeID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(Descriptions);
            m_objFilter.Add(NegotiationConfigTypesVM.Prop.Descriptions.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicSNegotiationConfigTypesDA = m_objSNegotiationConfigTypesDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objSNegotiationConfigTypesDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpNegotiationConfigTypesBL in m_dicSNegotiationConfigTypesDA)
                    {
                        m_intCount = m_kvpNegotiationConfigTypesBL.Key;
                        break;
                    }
                else
                {
                    m_lstNegotiationConfigTypesVM = (
                        from DataRow m_drSNegotiationConfigTypesDA in m_dicSNegotiationConfigTypesDA[0].Tables[0].Rows
                        select new NegotiationConfigTypesVM()
                        {
                            NegotiationConfigTypeID = m_drSNegotiationConfigTypesDA[NegotiationConfigTypesVM.Prop.NegotiationConfigTypeID.Name].ToString(),
                            Descriptions = m_drSNegotiationConfigTypesDA[NegotiationConfigTypesVM.Prop.Descriptions.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstNegotiationConfigTypesVM);
            return m_dicReturn;
        }

        #endregion

    }
}