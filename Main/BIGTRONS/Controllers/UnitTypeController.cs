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
    public class UnitTypeController : BaseController
    {
        private readonly string title = "Unit Type";
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
            MUnitTypeDA m_objMUnitTypeDA = new MUnitTypeDA();
            m_objMUnitTypeDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMUnitType = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMUnitType.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = UnitTypeVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMUnitTypeDA = m_objMUnitTypeDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpUnitTypeBL in m_dicMUnitTypeDA)
            {
                m_intCount = m_kvpUnitTypeBL.Key;
                break;
            }

            List<UnitTypeVM> m_lstUnitTypeVM = new List<UnitTypeVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(UnitTypeVM.Prop.UnitTypeID.MapAlias);
                m_lstSelect.Add(UnitTypeVM.Prop.UnitTypeDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(UnitTypeVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMUnitTypeDA = m_objMUnitTypeDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMUnitTypeDA.Message == string.Empty)
                {
                    m_lstUnitTypeVM = (
                        from DataRow m_drMUnitTypeDA in m_dicMUnitTypeDA[0].Tables[0].Rows
                        select new UnitTypeVM()
                        {
                            UnitTypeID = m_drMUnitTypeDA[UnitTypeVM.Prop.UnitTypeID.Name].ToString(),
                            UnitTypeDesc = m_drMUnitTypeDA[UnitTypeVM.Prop.UnitTypeDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstUnitTypeVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            MUnitTypeDA m_objMUnitTypeDA = new MUnitTypeDA();
            m_objMUnitTypeDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMUnitType = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMUnitType.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = UnitTypeVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMUnitTypeDA = m_objMUnitTypeDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpUnitTypeBL in m_dicMUnitTypeDA)
            {
                m_intCount = m_kvpUnitTypeBL.Key;
                break;
            }

            List<UnitTypeVM> m_lstUnitTypeVM = new List<UnitTypeVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(UnitTypeVM.Prop.UnitTypeID.MapAlias);
                m_lstSelect.Add(UnitTypeVM.Prop.UnitTypeDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(UnitTypeVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMUnitTypeDA = m_objMUnitTypeDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMUnitTypeDA.Message == string.Empty)
                {
                    m_lstUnitTypeVM = (
                        from DataRow m_drMUnitTypeDA in m_dicMUnitTypeDA[0].Tables[0].Rows
                        select new UnitTypeVM()
                        {
                            UnitTypeID = m_drMUnitTypeDA[UnitTypeVM.Prop.UnitTypeID.Name].ToString(),
                            UnitTypeDesc = m_drMUnitTypeDA[UnitTypeVM.Prop.UnitTypeDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstUnitTypeVM, m_intCount);
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

            UnitTypeVM m_objUnitTypeVM = new UnitTypeVM();
            ViewDataDictionary m_vddUnitType = new ViewDataDictionary();
            m_vddUnitType.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddUnitType.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
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
                Model = m_objUnitTypeVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddUnitType,
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

            
            UnitTypeVM m_objUnitTypeVM = new UnitTypeVM();
            if (m_dicSelectedRow.Count > 0)
                m_objUnitTypeVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
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
                Model = m_objUnitTypeVM,
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
            UnitTypeVM m_objUnitTypeVM = new UnitTypeVM();
            if (m_dicSelectedRow.Count > 0)
                m_objUnitTypeVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddUnitType = new ViewDataDictionary();
            m_vddUnitType.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddUnitType.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objUnitTypeVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddUnitType,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<UnitTypeVM> m_lstSelectedRow = new List<UnitTypeVM>();
            m_lstSelectedRow = JSON.Deserialize<List<UnitTypeVM>>(Selected);

            MUnitTypeDA m_objMUnitTypeDA = new MUnitTypeDA();
            m_objMUnitTypeDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (UnitTypeVM m_objUnitTypeVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifUnitTypeVM = m_objUnitTypeVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifUnitTypeVM in m_arrPifUnitTypeVM)
                    {
                        string m_strFieldName = m_pifUnitTypeVM.Name;
                        object m_objFieldValue = m_pifUnitTypeVM.GetValue(m_objUnitTypeVM);
                        if (m_objUnitTypeVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(UnitTypeVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objMUnitTypeDA.DeleteBC(m_objFilter, false);
                    if (m_objMUnitTypeDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMUnitTypeDA.Message);
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

        public ActionResult Browse(string ControlUnitTypeID, string ControlUnitTypeDesc, string FilterUnitTypeID = "", string FilterUnitTypeDesc = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddUnitType = new ViewDataDictionary();
            m_vddUnitType.Add("Control" + UnitTypeVM.Prop.UnitTypeID.Name, ControlUnitTypeID);
            m_vddUnitType.Add("Control" + UnitTypeVM.Prop.UnitTypeDesc.Name, ControlUnitTypeDesc);
            m_vddUnitType.Add(UnitTypeVM.Prop.UnitTypeID.Name, FilterUnitTypeID);
            m_vddUnitType.Add(UnitTypeVM.Prop.UnitTypeDesc.Name, FilterUnitTypeDesc);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddUnitType,
                ViewName = "../UnitType/_Browse"
            };
        }

        public ActionResult Save(string Action)
        {
            //action = JSON.Deserialize<string>(action);
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MUnitTypeDA m_objMUnitTypeDA = new MUnitTypeDA();
            m_objMUnitTypeDA.ConnectionStringName = Global.ConnStrConfigName;
            try
            {
                string m_strUnitTypeID = this.Request.Params[UnitTypeVM.Prop.UnitTypeID.Name];
                string m_strUnitTypeDesc = this.Request.Params[UnitTypeVM.Prop.UnitTypeDesc.Name];

                m_lstMessage = IsSaveValid(Action, m_strUnitTypeID, m_strUnitTypeDesc);
                if (m_lstMessage.Count <= 0)
                {
                    MUnitType m_objMUnitType = new MUnitType();
                    m_objMUnitType.UnitTypeID = m_strUnitTypeID;
                    m_objMUnitTypeDA.Data = m_objMUnitType;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objMUnitTypeDA.Select();

                    m_objMUnitType.UnitTypeDesc = m_strUnitTypeDesc;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objMUnitTypeDA.Insert(false);
                    else
                        m_objMUnitTypeDA.Update(false);

                    if (!m_objMUnitTypeDA.Success || m_objMUnitTypeDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMUnitTypeDA.Message);
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

        public ActionResult GetUnitType(string ControlUnitTypeID, string ControlUnitTypeDesc, string FilterUnitTypeID, string FilterUnitTypeDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<UnitTypeVM>> m_dicUnitTypeData = GetUnitTypeData(true, FilterUnitTypeID, FilterUnitTypeDesc);
                KeyValuePair<int, List<UnitTypeVM>> m_kvpUnitTypeVM = m_dicUnitTypeData.AsEnumerable().ToList()[0];
                if (m_kvpUnitTypeVM.Key < 1 || (m_kvpUnitTypeVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpUnitTypeVM.Key > 1 && !Exact)
                    return Browse(ControlUnitTypeID, ControlUnitTypeDesc, FilterUnitTypeID, FilterUnitTypeDesc);

                m_dicUnitTypeData = GetUnitTypeData(false, FilterUnitTypeID, FilterUnitTypeDesc);
                UnitTypeVM m_objUnitTypeVM = m_dicUnitTypeData[0][0];
                this.GetCmp<TextField>(ControlUnitTypeID).Value = m_objUnitTypeVM.UnitTypeID;
                this.GetCmp<TextField>(ControlUnitTypeDesc).Value = m_objUnitTypeVM.UnitTypeDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, string UnitTypeID, string UnitTypeDesc)
        {
            List<string> m_lstReturn = new List<string>();

            if (UnitTypeID == string.Empty)
                m_lstReturn.Add(UnitTypeVM.Prop.UnitTypeID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (UnitTypeDesc == string.Empty)
                m_lstReturn.Add(UnitTypeVM.Prop.UnitTypeDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));

            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(UnitTypeVM.Prop.UnitTypeID.Name, parameters[UnitTypeVM.Prop.UnitTypeID.Name]);
            m_dicReturn.Add(UnitTypeVM.Prop.UnitTypeDesc.Name, parameters[UnitTypeVM.Prop.UnitTypeDesc.Name]);

            return m_dicReturn;
        }

        private UnitTypeVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            UnitTypeVM m_objUnitTypeVM = new UnitTypeVM();
            MUnitTypeDA m_objMUnitTypeDA = new MUnitTypeDA();
            m_objMUnitTypeDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(UnitTypeVM.Prop.UnitTypeID.MapAlias);
            m_lstSelect.Add(UnitTypeVM.Prop.UnitTypeDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objUnitTypeVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(UnitTypeVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMUnitTypeDA = m_objMUnitTypeDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMUnitTypeDA.Message == string.Empty)
            {
                DataRow m_drMUnitTypeDA = m_dicMUnitTypeDA[0].Tables[0].Rows[0];
                m_objUnitTypeVM.UnitTypeID = m_drMUnitTypeDA[UnitTypeVM.Prop.UnitTypeID.Name].ToString();
                m_objUnitTypeVM.UnitTypeDesc = m_drMUnitTypeDA[UnitTypeVM.Prop.UnitTypeDesc.Name].ToString();
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMUnitTypeDA.Message;

            return m_objUnitTypeVM;
        }

        #endregion

        #region Public Method

        public Dictionary<int, List<UnitTypeVM>> GetUnitTypeData(bool isCount, string UnitTypeID, string UnitTypeDesc)
        {
            int m_intCount = 0;
            List<UnitTypeVM> m_lstUnitTypeVM = new List<ViewModels.UnitTypeVM>();
            Dictionary<int, List<UnitTypeVM>> m_dicReturn = new Dictionary<int, List<UnitTypeVM>>();
            MUnitTypeDA m_objMUnitTypeDA = new MUnitTypeDA();
            m_objMUnitTypeDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(UnitTypeVM.Prop.UnitTypeID.MapAlias);
            m_lstSelect.Add(UnitTypeVM.Prop.UnitTypeDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(UnitTypeID);
            m_objFilter.Add(UnitTypeVM.Prop.UnitTypeID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(UnitTypeDesc);
            m_objFilter.Add(UnitTypeVM.Prop.UnitTypeDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMUnitTypeDA = m_objMUnitTypeDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMUnitTypeDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpUnitTypeBL in m_dicMUnitTypeDA)
                    {
                        m_intCount = m_kvpUnitTypeBL.Key;
                        break;
                    }
                else
                {
                    m_lstUnitTypeVM = (
                        from DataRow m_drMUnitTypeDA in m_dicMUnitTypeDA[0].Tables[0].Rows
                        select new UnitTypeVM()
                        {
                            UnitTypeID = m_drMUnitTypeDA[UnitTypeVM.Prop.UnitTypeID.Name].ToString(),
                            UnitTypeDesc = m_drMUnitTypeDA[UnitTypeVM.Prop.UnitTypeDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstUnitTypeVM);
            return m_dicReturn;
        }

        #endregion
    }
}