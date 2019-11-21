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
    public class FunctionController : BaseController
    {
        private readonly string title = "Function";
        private readonly string dataSessionName = "FormData";

        #region Public Action

        public ActionResult Index()
        {
            //base.Initialize();
            return View();
        }

        public ActionResult List()
        {
            //Global.HasAccess = GetHasAccess();
            //if (!Global.HasAccess.Read)
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

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
            MFunctionsDA m_objMFunctionsDA = new MFunctionsDA();
            m_objMFunctionsDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMFunctions = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMFunctions.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = FunctionsVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMFunctionsDA = m_objMFunctionsDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpFunctionBL in m_dicMFunctionsDA)
            {
                m_intCount = m_kvpFunctionBL.Key;
                break;
            }

            List<FunctionsVM> m_lstFunctionsVM = new List<FunctionsVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(FunctionsVM.Prop.FunctionID.MapAlias);
                m_lstSelect.Add(FunctionsVM.Prop.FunctionDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(FunctionsVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMFunctionsDA = m_objMFunctionsDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMFunctionsDA.Message == string.Empty)
                {
                    m_lstFunctionsVM = (
                        from DataRow m_drMFunctionsDA in m_dicMFunctionsDA[0].Tables[0].Rows
                        select new FunctionsVM()
                        {
                            FunctionID = m_drMFunctionsDA[FunctionsVM.Prop.FunctionID.Name].ToString(),
                            FunctionDesc = m_drMFunctionsDA[FunctionsVM.Prop.FunctionDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstFunctionsVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            MFunctionsDA m_objMFunctionsDA = new MFunctionsDA();
            m_objMFunctionsDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMFunctions = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMFunctions.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = FunctionsVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMFunctionsDA = m_objMFunctionsDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpFunctionBL in m_dicMFunctionsDA)
            {
                m_intCount = m_kvpFunctionBL.Key;
                break;
            }

            List<FunctionsVM> m_lstFunctionsVM = new List<FunctionsVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(FunctionsVM.Prop.FunctionID.MapAlias);
                m_lstSelect.Add(FunctionsVM.Prop.FunctionDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(FunctionsVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMFunctionsDA = m_objMFunctionsDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMFunctionsDA.Message == string.Empty)
                {
                    m_lstFunctionsVM = (
                        from DataRow m_drMFunctionsDA in m_dicMFunctionsDA[0].Tables[0].Rows
                        select new FunctionsVM()
                        {
                            FunctionID = m_drMFunctionsDA[FunctionsVM.Prop.FunctionID.Name].ToString(),
                            FunctionDesc = m_drMFunctionsDA[FunctionsVM.Prop.FunctionDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstFunctionsVM, m_intCount);
        }

        public ActionResult Home()
        {
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 0;
            return this.Direct();
        }

        public ActionResult Add(string Caller)
        {
            //Global.HasAccess = GetHasAccess();
            //if (!Global.HasAccess.Add)
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            FunctionsVM m_objFunctionsVM = new FunctionsVM();
            ViewDataDictionary m_vddFunction = new ViewDataDictionary();
            m_vddFunction.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddFunction.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
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
                Model = m_objFunctionsVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddFunction,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Detail(string Caller, string Selected)
        {
            //Global.HasAccess = GetHasAccess();
            //if (!Global.HasAccess.Read)
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            string m_strMessage = string.Empty;
            FunctionsVM m_objFunctionsVM = new FunctionsVM();
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
                m_objFunctionsVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddFunction = new ViewDataDictionary();
            m_vddFunction.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objFunctionsVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddFunction,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Update(string Caller, string Selected)
        {
            //Global.HasAccess = GetHasAccess();
            //if (!Global.HasAccess.Update)
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

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
            FunctionsVM m_objFunctionsVM = new FunctionsVM();
            if (m_dicSelectedRow.Count > 0)
                m_objFunctionsVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddFunction = new ViewDataDictionary();
            m_vddFunction.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddFunction.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objFunctionsVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddFunction,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<FunctionsVM> m_lstSelectedRow = new List<FunctionsVM>();
            m_lstSelectedRow = JSON.Deserialize<List<FunctionsVM>>(Selected);

            MFunctionsDA m_objMFunctionsDA = new MFunctionsDA();
            m_objMFunctionsDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (FunctionsVM m_objFunctionsVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifFunctionsVM = m_objFunctionsVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifFunctionsVM in m_arrPifFunctionsVM)
                    {
                        string m_strFieldName = m_pifFunctionsVM.Name;
                        object m_objFieldValue = m_pifFunctionsVM.GetValue(m_objFunctionsVM);
                        if (m_objFunctionsVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(FunctionsVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objMFunctionsDA.DeleteBC(m_objFilter, false);
                    if (m_objMFunctionsDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMFunctionsDA.Message);
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

        public ActionResult Browse(string ControlFunctionID, string ControlFunctionDesc, string ControlGrdRoleFunction, string FilterFunctionID = "", string FilterFunctionDesc = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddFunction = new ViewDataDictionary();
            m_vddFunction.Add("Control" + FunctionsVM.Prop.FunctionID.Name, ControlFunctionID);
            m_vddFunction.Add("Control" + FunctionsVM.Prop.FunctionDesc.Name, ControlFunctionDesc);
            m_vddFunction.Add("ControlGrdRoleFunction", ControlGrdRoleFunction);
            m_vddFunction.Add(FunctionsVM.Prop.FunctionID.Name, FilterFunctionID);
            m_vddFunction.Add(FunctionsVM.Prop.FunctionDesc.Name, FilterFunctionDesc);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddFunction,
                ViewName = "../Function/_Browse"
            };
        }

        public ActionResult Save(string Action)
        {
            //action = JSON.Deserialize<string>(action);
            //if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
            //    : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MFunctionsDA m_objMFunctionsDA = new MFunctionsDA();
            m_objMFunctionsDA.ConnectionStringName = Global.ConnStrConfigName;
            try
            {
                string m_strFunctionID = this.Request.Params[FunctionsVM.Prop.FunctionID.Name];
                string m_strFunctionDesc = this.Request.Params[FunctionsVM.Prop.FunctionDesc.Name];

                m_lstMessage = IsSaveValid(Action, m_strFunctionID, m_strFunctionDesc);
                if (m_lstMessage.Count <= 0)
                {
                    MFunctions m_objMFunctions = new MFunctions();
                    m_objMFunctions.FunctionID = m_strFunctionID;
                    m_objMFunctionsDA.Data = m_objMFunctions;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objMFunctionsDA.Select();

                    m_objMFunctions.FunctionDesc = m_strFunctionDesc;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objMFunctionsDA.Insert(false);
                    else
                        m_objMFunctionsDA.Update(false);

                    if (!m_objMFunctionsDA.Success || m_objMFunctionsDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMFunctionsDA.Message);
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

        public ActionResult GetFunction(string ControlFunctionID, string ControlFunctionDesc, string FilterFunctionID, string FilterFunctionDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<FunctionsVM>> m_dicFunctionData = GetFunctionData(true, FilterFunctionID, FilterFunctionDesc);
                KeyValuePair<int, List<FunctionsVM>> m_kvpFunctionsVM = m_dicFunctionData.AsEnumerable().ToList()[0];
                if (m_kvpFunctionsVM.Key < 1 || (m_kvpFunctionsVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpFunctionsVM.Key > 1 && !Exact)
                    return Browse(ControlFunctionID, ControlFunctionDesc, FilterFunctionID, FilterFunctionDesc);

                m_dicFunctionData = GetFunctionData(false, FilterFunctionID, FilterFunctionDesc);
                FunctionsVM m_objFunctionsVM = m_dicFunctionData[0][0];
                this.GetCmp<TextField>(ControlFunctionID).Value = m_objFunctionsVM.FunctionID;
                this.GetCmp<TextField>(ControlFunctionDesc).Value = m_objFunctionsVM.FunctionDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, string FunctionID, string FunctionDesc)
        {
            List<string> m_lstReturn = new List<string>();

            if (FunctionID == string.Empty)
                m_lstReturn.Add(FunctionsVM.Prop.FunctionID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (FunctionDesc == string.Empty)
                m_lstReturn.Add(FunctionsVM.Prop.FunctionDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));

            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(FunctionsVM.Prop.FunctionID.Name, parameters[FunctionsVM.Prop.FunctionID.Name]);
            m_dicReturn.Add(FunctionsVM.Prop.FunctionDesc.Name, parameters[FunctionsVM.Prop.FunctionDesc.Name]);

            return m_dicReturn;
        }

        private FunctionsVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            FunctionsVM m_objFunctionsVM = new FunctionsVM();
            MFunctionsDA m_objMFunctionsDA = new MFunctionsDA();
            m_objMFunctionsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(FunctionsVM.Prop.FunctionID.MapAlias);
            m_lstSelect.Add(FunctionsVM.Prop.FunctionDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objFunctionsVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(FunctionsVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMFunctionsDA = m_objMFunctionsDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMFunctionsDA.Message == string.Empty)
            {
                DataRow m_drMFunctionsDA = m_dicMFunctionsDA[0].Tables[0].Rows[0];
                m_objFunctionsVM.FunctionID = m_drMFunctionsDA[FunctionsVM.Prop.FunctionID.Name].ToString();
                m_objFunctionsVM.FunctionDesc = m_drMFunctionsDA[FunctionsVM.Prop.FunctionDesc.Name].ToString();
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMFunctionsDA.Message;

            return m_objFunctionsVM;
        }

        #endregion

        #region Public Method

        public Dictionary<int, List<FunctionsVM>> GetFunctionData(bool isCount, string FunctionID, string FunctionDesc)
        {
            int m_intCount = 0;
            List<FunctionsVM> m_lstFunctionsVM = new List<ViewModels.FunctionsVM>();
            Dictionary<int, List<FunctionsVM>> m_dicReturn = new Dictionary<int, List<FunctionsVM>>();
            MFunctionsDA m_objMFunctionsDA = new MFunctionsDA();
            m_objMFunctionsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(FunctionsVM.Prop.FunctionID.MapAlias);
            m_lstSelect.Add(FunctionsVM.Prop.FunctionDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(FunctionID);
            m_objFilter.Add(FunctionsVM.Prop.FunctionID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(FunctionDesc);
            m_objFilter.Add(FunctionsVM.Prop.FunctionDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMFunctionsDA = m_objMFunctionsDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMFunctionsDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpFunctionBL in m_dicMFunctionsDA)
                    {
                        m_intCount = m_kvpFunctionBL.Key;
                        break;
                    }
                else
                {
                    m_lstFunctionsVM = (
                        from DataRow m_drMFunctionsDA in m_dicMFunctionsDA[0].Tables[0].Rows
                        select new FunctionsVM()
                        {
                            FunctionID = m_drMFunctionsDA[FunctionsVM.Prop.FunctionID.Name].ToString(),
                            FunctionDesc = m_drMFunctionsDA[FunctionsVM.Prop.FunctionDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstFunctionsVM);
            return m_dicReturn;
        }

        #endregion
    }
}