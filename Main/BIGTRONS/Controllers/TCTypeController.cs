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
    public class TCTypeController : BaseController
    {
        private readonly string title = "CType";
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
            MTCTypesDA m_objMTCTypesDA = new MTCTypesDA();
            m_objMTCTypesDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMCType = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMCType.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = TCTypesVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMTCTypesDA = m_objMTCTypesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpCTypeBL in m_dicMTCTypesDA)
            {
                m_intCount = m_kvpCTypeBL.Key;
                break;
            }

            List<TCTypesVM> m_lsTCTypesVM = new List<TCTypesVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(TCTypesVM.Prop.TCTypeID.MapAlias);
                m_lstSelect.Add(TCTypesVM.Prop.TCTypeDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(TCTypesVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMTCTypesDA = m_objMTCTypesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMTCTypesDA.Message == string.Empty)
                {
                    m_lsTCTypesVM = (
                        from DataRow m_drMTCTypesDA in m_dicMTCTypesDA[0].Tables[0].Rows
                        select new TCTypesVM()
                        {
                            TCTypeID = m_drMTCTypesDA[TCTypesVM.Prop.TCTypeID.Name].ToString(),
                            TCTypeDesc = m_drMTCTypesDA[TCTypesVM.Prop.TCTypeDesc.Name].ToString(),
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lsTCTypesVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            MTCTypesDA m_objMTCTypesDA = new MTCTypesDA();
            m_objMTCTypesDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMCType = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMCType.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = TCTypesVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMTCTypesDA = m_objMTCTypesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpCTypeBL in m_dicMTCTypesDA)
            {
                m_intCount = m_kvpCTypeBL.Key;
                break;
            }

            List<TCTypesVM> m_lsTCTypesVM = new List<TCTypesVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(TCTypesVM.Prop.TCTypeID.MapAlias);
                m_lstSelect.Add(TCTypesVM.Prop.TCTypeDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(TCTypesVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMTCTypesDA = m_objMTCTypesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMTCTypesDA.Message == string.Empty)
                {
                    m_lsTCTypesVM = (
                        from DataRow m_drMTCTypesDA in m_dicMTCTypesDA[0].Tables[0].Rows
                        select new TCTypesVM()
                        {
                            TCTypeID = m_drMTCTypesDA[TCTypesVM.Prop.TCTypeID.Name].ToString(),
                            TCTypeDesc = m_drMTCTypesDA[TCTypesVM.Prop.TCTypeDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lsTCTypesVM, m_intCount);
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

            TCTypesVM m_objTCTypesVM = new TCTypesVM();
            ViewDataDictionary m_vddCType = new ViewDataDictionary();
            m_vddCType.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddCType.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
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
                Model = m_objTCTypesVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddCType,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Detail(string Caller, string Selected)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            TCTypesVM m_objTCTypesVM = new TCTypesVM();
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
                m_objTCTypesVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
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
                Model = m_objTCTypesVM,
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
            TCTypesVM m_objTCTypesVM = new TCTypesVM();
            if (m_dicSelectedRow.Count > 0)
                m_objTCTypesVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddCType = new ViewDataDictionary();
            m_vddCType.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddCType.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objTCTypesVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddCType,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<TCTypesVM> m_lstSelectedRow = new List<TCTypesVM>();
            m_lstSelectedRow = JSON.Deserialize<List<TCTypesVM>>(Selected);

            MTCTypesDA m_objMTCTypesDA = new MTCTypesDA();
            m_objMTCTypesDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (TCTypesVM m_objTCTypesVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifTCTypesVM = m_objTCTypesVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifTCTypesVM in m_arrPifTCTypesVM)
                    {
                        string m_strFieldName = m_pifTCTypesVM.Name;
                        object m_objFieldValue = m_pifTCTypesVM.GetValue(m_objTCTypesVM);
                        if (m_objTCTypesVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(TCTypesVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objMTCTypesDA.DeleteBC(m_objFilter, false);
                    if (m_objMTCTypesDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMTCTypesDA.Message);
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

        public ActionResult Browse(string ControlTCTypeID, string ControlTCTypeDesc, string FilterTCType = "", string FilterCTypeDesc = "")
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddCType = new ViewDataDictionary();
            m_vddCType.Add("Control" + TCTypesVM.Prop.TCTypeID.Name, ControlTCTypeID);
            m_vddCType.Add("Control" + TCTypesVM.Prop.TCTypeDesc.Name, ControlTCTypeDesc);
            m_vddCType.Add(TCTypesVM.Prop.TCTypeID.Name, FilterTCType);
            m_vddCType.Add(TCTypesVM.Prop.TCTypeDesc.Name, FilterCTypeDesc);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddCType,
                ViewName = "../TCType/_Browse"
            };
        }

        public ActionResult Save(string Action)
        {
            //action = JSON.Deserialize<string>(action);
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MTCTypesDA m_objMTCTypesDA = new MTCTypesDA();
            m_objMTCTypesDA.ConnectionStringName = Global.ConnStrConfigName;
            try
            {
                string m_strTCTypeID = this.Request.Params[TCTypesVM.Prop.TCTypeID.Name];
                string m_strTCTypeDesc = this.Request.Params[TCTypesVM.Prop.TCTypeDesc.Name];

                m_lstMessage = IsSaveValid(Action, m_strTCTypeID, m_strTCTypeDesc);
                if (m_lstMessage.Count <= 0)
                {
                    MTCTypes m_objMCType = new MTCTypes();
                    m_objMCType.TCTypeID = m_strTCTypeID;
                    m_objMTCTypesDA.Data = m_objMCType;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objMTCTypesDA.Select();

                    m_objMCType.Descriptions = m_strTCTypeDesc;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objMTCTypesDA.Insert(false);
                    else
                        m_objMTCTypesDA.Update(false);

                    if (!m_objMTCTypesDA.Success || m_objMTCTypesDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMTCTypesDA.Message);
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

        public ActionResult GetCType(string ControlTCType, string ControlCTypeDesc, string FilterTCType, string FilterCTypeDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<TCTypesVM>> m_dicCTypeData = GetCTypeData(true, FilterTCType, FilterCTypeDesc);
                KeyValuePair<int, List<TCTypesVM>> m_kvpTCTypesVM = m_dicCTypeData.AsEnumerable().ToList()[0];
                if (m_kvpTCTypesVM.Key < 1 || (m_kvpTCTypesVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpTCTypesVM.Key > 1 && !Exact)
                    return Browse(ControlTCType, ControlCTypeDesc, FilterTCType, FilterCTypeDesc);

                m_dicCTypeData = GetCTypeData(false, FilterTCType, FilterCTypeDesc);
                TCTypesVM m_objTCTypesVM = m_dicCTypeData[0][0];
                this.GetCmp<TextField>(ControlTCType).Value = m_objTCTypesVM.TCTypeID;
                this.GetCmp<TextField>(ControlCTypeDesc).Value = m_objTCTypesVM.TCTypeDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, string TCTypeID, string TCTypeDesc)
        {
            List<string> m_lstReturn = new List<string>();

            if (TCTypeID == string.Empty)
                m_lstReturn.Add(TCTypesVM.Prop.TCTypeID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (TCTypeDesc == string.Empty)
                m_lstReturn.Add(TCTypesVM.Prop.TCTypeDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            //if (DimensionID == string.Empty)
            //    m_lstReturn.Add(TCTypesVM.Prop.DimensionID.Desc + " " + General.EnumDesc(MessageLib.mustFill));

            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(TCTypesVM.Prop.TCTypeID.Name, parameters[TCTypesVM.Prop.TCTypeID.Name]);
            m_dicReturn.Add(TCTypesVM.Prop.TCTypeDesc.Name, parameters[TCTypesVM.Prop.TCTypeDesc.Name]);

            return m_dicReturn;
        }

        private TCTypesVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            TCTypesVM m_objTCTypesVM = new TCTypesVM();
            MTCTypesDA m_objMTCTypesDA = new MTCTypesDA();
            m_objMTCTypesDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(TCTypesVM.Prop.TCTypeID.MapAlias);
            m_lstSelect.Add(TCTypesVM.Prop.TCTypeDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objTCTypesVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(TCTypesVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMTCTypesDA = m_objMTCTypesDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMTCTypesDA.Message == string.Empty)
            {
                DataRow m_drMTCTypesDA = m_dicMTCTypesDA[0].Tables[0].Rows[0];
                m_objTCTypesVM.TCTypeID = m_drMTCTypesDA[TCTypesVM.Prop.TCTypeID.Name].ToString();
                m_objTCTypesVM.TCTypeDesc = m_drMTCTypesDA[TCTypesVM.Prop.TCTypeDesc.Name].ToString();
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMTCTypesDA.Message;

            return m_objTCTypesVM;
        }

        #endregion

        #region Public Method

        public Dictionary<int, List<TCTypesVM>> GetCTypeData(bool isCount, string TCType, string CTypeDesc)
        {
            int m_intCount = 0;
            List<TCTypesVM> m_lsTCTypesVM = new List<ViewModels.TCTypesVM>();
            Dictionary<int, List<TCTypesVM>> m_dicReturn = new Dictionary<int, List<TCTypesVM>>();
            MTCTypesDA m_objMTCTypesDA = new MTCTypesDA();
            m_objMTCTypesDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(TCTypesVM.Prop.TCTypeID.MapAlias);
            m_lstSelect.Add(TCTypesVM.Prop.TCTypeDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(TCType);
            m_objFilter.Add(TCTypesVM.Prop.TCTypeID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(CTypeDesc);
            m_objFilter.Add(TCTypesVM.Prop.TCTypeDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMTCTypesDA = m_objMTCTypesDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMTCTypesDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpCTypeBL in m_dicMTCTypesDA)
                    {
                        m_intCount = m_kvpCTypeBL.Key;
                        break;
                    }
                else
                {
                    m_lsTCTypesVM = (
                        from DataRow m_drMTCTypesDA in m_dicMTCTypesDA[0].Tables[0].Rows
                        select new TCTypesVM()
                        {
                            TCTypeID = m_drMTCTypesDA[TCTypesVM.Prop.TCTypeID.Name].ToString(),
                            TCTypeDesc = m_drMTCTypesDA[TCTypesVM.Prop.TCTypeDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lsTCTypesVM);
            return m_dicReturn;
        }

        #endregion
    }
}