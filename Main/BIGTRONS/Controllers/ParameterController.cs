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
    public class ParameterController : BaseController
    {
        private readonly string title = "Parameter";
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
            MParameterDA m_objMParameterDA = new MParameterDA();
            m_objMParameterDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMParameter = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMParameter.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = ParameterVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMParameterDA = m_objMParameterDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpParameterBL in m_dicMParameterDA)
            {
                m_intCount = m_kvpParameterBL.Key;
                break;
            }

            List<ParameterVM> m_lstParameterVM = new List<ParameterVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(ParameterVM.Prop.ParameterID.MapAlias);
                m_lstSelect.Add(ParameterVM.Prop.ParameterDesc.MapAlias);
                m_lstSelect.Add(ParameterVM.Prop.DataType.MapAlias);
                m_lstSelect.Add(ParameterVM.Prop.Length.MapAlias);
                m_lstSelect.Add(ParameterVM.Prop.Precision.MapAlias);
                m_lstSelect.Add(ParameterVM.Prop.RefDescColumn.MapAlias);
                m_lstSelect.Add(ParameterVM.Prop.RefIDColumn.MapAlias);
                m_lstSelect.Add(ParameterVM.Prop.RefTable.MapAlias);
                m_lstSelect.Add(ParameterVM.Prop.Scale.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(ParameterVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMParameterDA = m_objMParameterDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMParameterDA.Message == string.Empty)
                {
                    m_lstParameterVM = (
                        from DataRow m_drMParameterDA in m_dicMParameterDA[0].Tables[0].Rows
                        select new ParameterVM()
                        {
                            ParameterID = m_drMParameterDA[ParameterVM.Prop.ParameterID.Name].ToString(),
                            ParameterDesc = m_drMParameterDA[ParameterVM.Prop.ParameterDesc.Name].ToString(),
                            DataType = m_drMParameterDA[ParameterVM.Prop.DataType.Name].ToString(),
                            Length = String.IsNullOrEmpty(m_drMParameterDA[ParameterVM.Prop.Length.Name].ToString()) ? 0 : Convert.ToInt32(m_drMParameterDA[ParameterVM.Prop.Length.Name].ToString()),
                            Precision = String.IsNullOrEmpty(m_drMParameterDA[ParameterVM.Prop.Precision.Name].ToString()) ? 0 : Convert.ToInt32(m_drMParameterDA[ParameterVM.Prop.Precision.Name].ToString()),
                            RefDescColumn = m_drMParameterDA[ParameterVM.Prop.RefDescColumn.Name].ToString(),
                            RefIDColumn = m_drMParameterDA[ParameterVM.Prop.RefIDColumn.Name].ToString(),
                            RefTable = m_drMParameterDA[ParameterVM.Prop.RefTable.Name].ToString(),
                            Scale = String.IsNullOrEmpty(m_drMParameterDA[ParameterVM.Prop.Scale.Name].ToString()) ? 0 : Convert.ToInt32(m_drMParameterDA[ParameterVM.Prop.Scale.Name].ToString())
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstParameterVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            MParameterDA m_objMParameterDA = new MParameterDA();
            m_objMParameterDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMParameter = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMParameter.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = ParameterVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMParameterDA = m_objMParameterDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpParameterBL in m_dicMParameterDA)
            {
                m_intCount = m_kvpParameterBL.Key;
                break;
            }

            List<ParameterVM> m_lstParameterVM = new List<ParameterVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(ParameterVM.Prop.ParameterID.MapAlias);
                m_lstSelect.Add(ParameterVM.Prop.ParameterDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(ParameterVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMParameterDA = m_objMParameterDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMParameterDA.Message == string.Empty)
                {
                    m_lstParameterVM = (
                        from DataRow m_drMParameterDA in m_dicMParameterDA[0].Tables[0].Rows
                        select new ParameterVM()
                        {
                            ParameterID = m_drMParameterDA[ParameterVM.Prop.ParameterID.Name].ToString(),
                            ParameterDesc = m_drMParameterDA[ParameterVM.Prop.ParameterDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstParameterVM, m_intCount);
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

            ParameterVM m_objParameterVM = new ParameterVM();
            ViewDataDictionary m_vddParameter = new ViewDataDictionary();
            m_vddParameter.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddParameter.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
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
                Model = m_objParameterVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddParameter,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Detail(string Caller, string Selected)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ParameterVM m_objParameterVM = new ParameterVM();
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
            {
                m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
            }
            else if (Caller == General.EnumDesc(Buttons.ButtonAdd) || Caller == General.EnumDesc(Buttons.ButtonUpdate))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                m_dicSelectedRow = GetFormData(m_nvcParams);
            }


            if (m_dicSelectedRow.Count > 0)
                m_objParameterVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
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
                Model = m_objParameterVM,
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
            ParameterVM m_objParameterVM = new ParameterVM();
            if (m_dicSelectedRow.Count > 0)
                m_objParameterVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddParameter = new ViewDataDictionary();
            m_vddParameter.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddParameter.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objParameterVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddParameter,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<ParameterVM> m_lstSelectedRow = new List<ParameterVM>();
            m_lstSelectedRow = JSON.Deserialize<List<ParameterVM>>(Selected);

            MParameterDA m_objMParameterDA = new MParameterDA();
            m_objMParameterDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (ParameterVM m_objParameterVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifParameterVM = m_objParameterVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifParameterVM in m_arrPifParameterVM)
                    {
                        string m_strFieldName = m_pifParameterVM.Name;
                        object m_objFieldValue = m_pifParameterVM.GetValue(m_objParameterVM);
                        if (m_objParameterVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(ParameterVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objMParameterDA.DeleteBC(m_objFilter, false);
                    if (m_objMParameterDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMParameterDA.Message);
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

        public ActionResult Browse(string ControlParameterID, string ControlParameterDesc, string ControlgrdParameter, string FilterParameterID = "", string FilterParameterDesc = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddParameter = new ViewDataDictionary();
            m_vddParameter.Add("Control" + ParameterVM.Prop.ParameterID.Name, ControlParameterID);
            m_vddParameter.Add("Control" + ParameterVM.Prop.ParameterDesc.Name, ControlParameterDesc);
            m_vddParameter.Add(ParameterVM.Prop.ParameterID.Name, FilterParameterID);
            m_vddParameter.Add(ParameterVM.Prop.ParameterDesc.Name, FilterParameterDesc);
            m_vddParameter.Add("ControlgrdParameter", ControlgrdParameter);


            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddParameter,
                ViewName = "../Parameter/_Browse"
            };
        }

        public ActionResult Save(string Action)
        {
            //action = JSON.Deserialize<string>(action);
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MParameterDA m_objMParameterDA = new MParameterDA();
            m_objMParameterDA.ConnectionStringName = Global.ConnStrConfigName;
            try
            {
                string m_strParameterID = this.Request.Params[ParameterVM.Prop.ParameterID.Name];
                string m_strParameterDesc = this.Request.Params[ParameterVM.Prop.ParameterDesc.Name];
                string m_strDataType = this.Request.Params[ParameterVM.Prop.DataType.Name];
                string m_intLength = this.Request.Params[ParameterVM.Prop.Length.Name];
                string m_intPrecision = this.Request.Params[ParameterVM.Prop.Precision.Name];
                string m_strRefDescColumn = this.Request.Params[ParameterVM.Prop.RefDescColumn.Name];
                string m_strRefIDColumn = this.Request.Params[ParameterVM.Prop.RefIDColumn.Name];
                string m_strRefTable = this.Request.Params[ParameterVM.Prop.RefTable.Name];
                string m_intScale = this.Request.Params[ParameterVM.Prop.Scale.Name];

                m_lstMessage = IsSaveValid(Action, m_strParameterID, m_strParameterDesc, m_strDataType,
                    m_intLength, m_intPrecision, m_strRefDescColumn, m_strRefIDColumn, m_strRefTable, m_intScale);
                if (m_lstMessage.Count <= 0)
                {
                    MParameter m_objMParameter = new MParameter();
                    m_objMParameter.ParameterID = m_strParameterID;
                    m_objMParameterDA.Data = m_objMParameter;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objMParameterDA.Select();

                    m_objMParameter.ParameterDesc = m_strParameterDesc;
                    m_objMParameter.DataType = m_strDataType;
                    m_objMParameter.Length = Convert.ToInt32(m_intLength);
                    m_objMParameter.Precision = Convert.ToInt32(m_intPrecision);
                    m_objMParameter.RefDescColumn = m_strRefDescColumn;
                    m_objMParameter.RefIDColumn = m_strRefIDColumn;
                    m_objMParameter.RefTable = m_strRefTable;
                    m_objMParameter.Scale = Convert.ToInt32(m_intScale);

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objMParameterDA.Insert(false);
                    else
                        m_objMParameterDA.Update(false);

                    if (!m_objMParameterDA.Success || m_objMParameterDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMParameterDA.Message);
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

        public ActionResult GetParameter(string ControlParameterID, string ControlParameterDesc, string FilterParameterID, string FilterParameterDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<ParameterVM>> m_dicParameterData = GetParameterData(true, FilterParameterID, FilterParameterDesc);
                KeyValuePair<int, List<ParameterVM>> m_kvpParameterVM = m_dicParameterData.AsEnumerable().ToList()[0];
                if (m_kvpParameterVM.Key < 1 || (m_kvpParameterVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpParameterVM.Key > 1 && !Exact)
                    return Browse(ControlParameterID, ControlParameterDesc, "", FilterParameterID, FilterParameterDesc);

                m_dicParameterData = GetParameterData(false, FilterParameterID, FilterParameterDesc);
                ParameterVM m_objParameterVM = m_dicParameterData[0][0];
                this.GetCmp<TextField>(ControlParameterID).Value = m_objParameterVM.ParameterID;
                this.GetCmp<TextField>(ControlParameterDesc).Value = m_objParameterVM.ParameterDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, string ParameterID, string ParameterDesc, string DataType,
                    string Length, string Precision, string RefDescColumn, string RefIDColumn, string RefTable,
                    string Scale)
        {
            List<string> m_lstReturn = new List<string>();

            if (ParameterID == string.Empty)
                m_lstReturn.Add(ParameterVM.Prop.ParameterID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (ParameterDesc == string.Empty)
                m_lstReturn.Add(ParameterVM.Prop.ParameterDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (DataType == string.Empty)
                m_lstReturn.Add(ParameterVM.Prop.DataType.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (Length == string.Empty)
                m_lstReturn.Add(ParameterVM.Prop.Length.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            else
            {
                try
                {
                    int.Parse(Length);
                }
                catch
                {
                    m_lstReturn.Add(ParameterVM.Prop.Length.Desc + " " + General.EnumDesc(MessageLib.Invalid));
                }
            }
            if (Precision == string.Empty)
                m_lstReturn.Add(ParameterVM.Prop.Precision.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            else
            {
                try
                {
                    int.Parse(Precision);
                }
                catch
                {
                    m_lstReturn.Add(ParameterVM.Prop.Precision.Desc + " " + General.EnumDesc(MessageLib.Invalid));
                }
            }
            //if (RefDescColumn == string.Empty)
            //    m_lstReturn.Add(ParameterVM.Prop.RefDescColumn.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            //if (RefIDColumn == string.Empty)
            //    m_lstReturn.Add(ParameterVM.Prop.RefIDColumn.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            //if (RefTable == string.Empty)
            //    m_lstReturn.Add(ParameterVM.Prop.RefTable.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (Scale == string.Empty)
                m_lstReturn.Add(ParameterVM.Prop.Scale.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            else
            {
                try
                {
                    int.Parse(Scale);
                }
                catch
                {
                    m_lstReturn.Add(ParameterVM.Prop.Scale.Desc + " " + General.EnumDesc(MessageLib.Invalid));
                }
            }


            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(ParameterVM.Prop.ParameterID.Name, parameters[ParameterVM.Prop.ParameterID.Name]);
            m_dicReturn.Add(ParameterVM.Prop.ParameterDesc.Name, parameters[ParameterVM.Prop.ParameterDesc.Name]);

            return m_dicReturn;
        }

        private ParameterVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            ParameterVM m_objParameterVM = new ParameterVM();
            MParameterDA m_objMParameterDA = new MParameterDA();
            m_objMParameterDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ParameterVM.Prop.ParameterID.MapAlias);
            m_lstSelect.Add(ParameterVM.Prop.ParameterDesc.MapAlias);
            m_lstSelect.Add(ParameterVM.Prop.DataType.MapAlias);
            m_lstSelect.Add(ParameterVM.Prop.Length.MapAlias);
            m_lstSelect.Add(ParameterVM.Prop.Precision.MapAlias);
            m_lstSelect.Add(ParameterVM.Prop.Scale.MapAlias);
            m_lstSelect.Add(ParameterVM.Prop.RefTable.MapAlias);
            m_lstSelect.Add(ParameterVM.Prop.RefIDColumn.MapAlias);
            m_lstSelect.Add(ParameterVM.Prop.RefDescColumn.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objParameterVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(ParameterVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMParameterDA = m_objMParameterDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMParameterDA.Message == string.Empty)
            {
                DataRow m_drMParameterDA = m_dicMParameterDA[0].Tables[0].Rows[0];
                m_objParameterVM.ParameterID = m_drMParameterDA[ParameterVM.Prop.ParameterID.Name].ToString();
                m_objParameterVM.ParameterDesc = m_drMParameterDA[ParameterVM.Prop.ParameterDesc.Name].ToString();
                m_objParameterVM.DataType = m_drMParameterDA[ParameterVM.Prop.DataType.Name].ToString();
                m_objParameterVM.Length = (int)m_drMParameterDA[ParameterVM.Prop.Length.Name];
                m_objParameterVM.Precision = (int)m_drMParameterDA[ParameterVM.Prop.Precision.Name];
                m_objParameterVM.RefDescColumn = m_drMParameterDA[ParameterVM.Prop.RefDescColumn.Name].ToString();
                m_objParameterVM.RefIDColumn = m_drMParameterDA[ParameterVM.Prop.RefIDColumn.Name].ToString();
                m_objParameterVM.RefTable = m_drMParameterDA[ParameterVM.Prop.RefTable.Name].ToString();
                m_objParameterVM.Scale = (int)m_drMParameterDA[ParameterVM.Prop.Scale.Name];
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMParameterDA.Message;

            return m_objParameterVM;
        }

        #endregion

        #region Public Method

        public Dictionary<int, List<ParameterVM>> GetParameterData(bool isCount, string ParameterID, string ParameterDesc)
        {
            int m_intCount = 0;
            List<ParameterVM> m_lstParameterVM = new List<ViewModels.ParameterVM>();
            Dictionary<int, List<ParameterVM>> m_dicReturn = new Dictionary<int, List<ParameterVM>>();
            MParameterDA m_objMParameterDA = new MParameterDA();
            m_objMParameterDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ParameterVM.Prop.ParameterID.MapAlias);
            m_lstSelect.Add(ParameterVM.Prop.ParameterDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(ParameterID);
            m_objFilter.Add(ParameterVM.Prop.ParameterID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(ParameterDesc);
            m_objFilter.Add(ParameterVM.Prop.ParameterDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMParameterDA = m_objMParameterDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMParameterDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpParameterBL in m_dicMParameterDA)
                    {
                        m_intCount = m_kvpParameterBL.Key;
                        break;
                    }
                else
                {
                    m_lstParameterVM = (
                        from DataRow m_drMParameterDA in m_dicMParameterDA[0].Tables[0].Rows
                        select new ParameterVM()
                        {
                            ParameterID = m_drMParameterDA[ParameterVM.Prop.ParameterID.Name].ToString(),
                            ParameterDesc = m_drMParameterDA[ParameterVM.Prop.ParameterDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstParameterVM);
            return m_dicReturn;
        }

        #endregion
    }
}