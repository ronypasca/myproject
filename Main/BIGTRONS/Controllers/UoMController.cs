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
    public class UoMController : BaseController
    {
        private readonly string title = "UoM";
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
            MUoMDA m_objMUoMDA = new MUoMDA();
            m_objMUoMDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMUoM = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMUoM.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = UoMVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMUoMDA = m_objMUoMDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpUoMBL in m_dicMUoMDA)
            {
                m_intCount = m_kvpUoMBL.Key;
                break;
            }

            List<UoMVM> m_lstUoMVM = new List<UoMVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(UoMVM.Prop.UoMID.MapAlias);
                m_lstSelect.Add(UoMVM.Prop.UoMDesc.MapAlias);
                m_lstSelect.Add(UoMVM.Prop.DimensionDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(UoMVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMUoMDA = m_objMUoMDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMUoMDA.Message == string.Empty)
                {
                    m_lstUoMVM = (
                        from DataRow m_drMUoMDA in m_dicMUoMDA[0].Tables[0].Rows
                        select new UoMVM()
                        {
                            UoMID = m_drMUoMDA[UoMVM.Prop.UoMID.Name].ToString(),
                            UoMDesc = m_drMUoMDA[UoMVM.Prop.UoMDesc.Name].ToString(),
                            DimensionDesc = m_drMUoMDA[UoMVM.Prop.DimensionDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstUoMVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            MUoMDA m_objMUoMDA = new MUoMDA();
            m_objMUoMDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMUoM = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMUoM.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = UoMVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMUoMDA = m_objMUoMDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpUoMBL in m_dicMUoMDA)
            {
                m_intCount = m_kvpUoMBL.Key;
                break;
            }

            List<UoMVM> m_lstUoMVM = new List<UoMVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(UoMVM.Prop.UoMID.MapAlias);
                m_lstSelect.Add(UoMVM.Prop.UoMDesc.MapAlias);
                m_lstSelect.Add(UoMVM.Prop.DimensionDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(UoMVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMUoMDA = m_objMUoMDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMUoMDA.Message == string.Empty)
                {
                    m_lstUoMVM = (
                        from DataRow m_drMUoMDA in m_dicMUoMDA[0].Tables[0].Rows
                        select new UoMVM()
                        {
                            UoMID = m_drMUoMDA[UoMVM.Prop.UoMID.Name].ToString(),
                            UoMDesc = m_drMUoMDA[UoMVM.Prop.UoMDesc.Name].ToString(),
                            DimensionDesc = m_drMUoMDA[UoMVM.Prop.DimensionDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstUoMVM, m_intCount);
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

            UoMVM m_objUoMVM = new UoMVM();
            ViewDataDictionary m_vddUoM = new ViewDataDictionary();
            m_vddUoM.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddUoM.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
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
                Model = m_objUoMVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddUoM,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Detail(string Caller, string Selected)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            UoMVM m_objUoMVM = new UoMVM();
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
                m_objUoMVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
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
                Model = m_objUoMVM,
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
            UoMVM m_objUoMVM = new UoMVM();
            if (m_dicSelectedRow.Count > 0)
                m_objUoMVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddUoM = new ViewDataDictionary();
            m_vddUoM.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddUoM.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objUoMVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddUoM,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<UoMVM> m_lstSelectedRow = new List<UoMVM>();
            m_lstSelectedRow = JSON.Deserialize<List<UoMVM>>(Selected);

            MUoMDA m_objMUoMDA = new MUoMDA();
            m_objMUoMDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (UoMVM m_objUoMVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifUoMVM = m_objUoMVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifUoMVM in m_arrPifUoMVM)
                    {
                        string m_strFieldName = m_pifUoMVM.Name;
                        object m_objFieldValue = m_pifUoMVM.GetValue(m_objUoMVM);
                        if (m_objUoMVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(UoMVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objMUoMDA.DeleteBC(m_objFilter, false);
                    if (m_objMUoMDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMUoMDA.Message);
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

        public ActionResult Browse(string ControlUoMID, string ControlUoMDesc, string FilterUoMID = "", string FilterUoMDesc = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddUoM = new ViewDataDictionary();
            m_vddUoM.Add("Control" + UoMVM.Prop.UoMID.Name, ControlUoMID);
            m_vddUoM.Add("Control" + UoMVM.Prop.UoMDesc.Name, ControlUoMDesc);
            m_vddUoM.Add(UoMVM.Prop.UoMID.Name, FilterUoMID);
            m_vddUoM.Add(UoMVM.Prop.UoMDesc.Name, FilterUoMDesc);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddUoM,
                ViewName = "../UoM/_Browse"
            };
        }

        public ActionResult Save(string Action)
        {
            //action = JSON.Deserialize<string>(action);
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MUoMDA m_objMUoMDA = new MUoMDA();
            m_objMUoMDA.ConnectionStringName = Global.ConnStrConfigName;
            try
            {
                string m_strUoMID = this.Request.Params[UoMVM.Prop.UoMID.Name];
                string m_strUoMDesc = this.Request.Params[UoMVM.Prop.UoMDesc.Name];
                string m_strDimensionID = this.Request.Params[DimensionVM.Prop.DimensionID.Name];

                m_lstMessage = IsSaveValid(Action, m_strUoMID, m_strUoMDesc, m_strDimensionID);
                if (m_lstMessage.Count <= 0)
                {
                    MUoM m_objMUoM = new MUoM();
                    m_objMUoM.UoMID = m_strUoMID;
                    m_objMUoMDA.Data = m_objMUoM;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objMUoMDA.Select();

                    m_objMUoM.UoMDesc = m_strUoMDesc;
                    m_objMUoM.DimensionID = m_strDimensionID;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objMUoMDA.Insert(false);
                    else
                        m_objMUoMDA.Update(false);

                    if (!m_objMUoMDA.Success || m_objMUoMDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMUoMDA.Message);
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

        public ActionResult GetUoM(string ControlUoMID, string ControlUoMDesc, string FilterUoMID, string FilterUoMDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<UoMVM>> m_dicUoMData = GetUoMData(true, FilterUoMID, FilterUoMDesc);
                KeyValuePair<int, List<UoMVM>> m_kvpUoMVM = m_dicUoMData.AsEnumerable().ToList()[0];
                if (m_kvpUoMVM.Key < 1 || (m_kvpUoMVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpUoMVM.Key > 1 && !Exact)
                    return Browse(ControlUoMID, ControlUoMDesc, FilterUoMID, FilterUoMDesc);

                m_dicUoMData = GetUoMData(false, FilterUoMID, FilterUoMDesc);
                UoMVM m_objUoMVM = m_dicUoMData[0][0];
                this.GetCmp<TextField>(ControlUoMID).Value = m_objUoMVM.UoMID;
                this.GetCmp<TextField>(ControlUoMDesc).Value = m_objUoMVM.UoMDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, string UoMID, string UoMDesc, string DimensionID)
        {
            List<string> m_lstReturn = new List<string>();

            if (UoMID == string.Empty)
                m_lstReturn.Add(UoMVM.Prop.UoMID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            //if (UoMDesc == string.Empty)
                //m_lstReturn.Add(UoMVM.Prop.UoMDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (DimensionID == string.Empty)
                m_lstReturn.Add(UoMVM.Prop.DimensionID.Desc + " " + General.EnumDesc(MessageLib.mustFill));

            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(UoMVM.Prop.UoMID.Name, parameters[UoMVM.Prop.UoMID.Name]);
            m_dicReturn.Add(UoMVM.Prop.UoMDesc.Name, parameters[UoMVM.Prop.UoMDesc.Name]);
            m_dicReturn.Add(UoMVM.Prop.DimensionID.Name, parameters[UoMVM.Prop.DimensionID.Name]);
            m_dicReturn.Add(UoMVM.Prop.DimensionDesc.Name, parameters[UoMVM.Prop.DimensionDesc.Name]);

            return m_dicReturn;
        }

        private UoMVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            UoMVM m_objUoMVM = new UoMVM();
            MUoMDA m_objMUoMDA = new MUoMDA();
            m_objMUoMDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(UoMVM.Prop.UoMID.MapAlias);
            m_lstSelect.Add(UoMVM.Prop.UoMDesc.MapAlias);
            m_lstSelect.Add(UoMVM.Prop.DimensionID.MapAlias);
            m_lstSelect.Add(UoMVM.Prop.DimensionDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objUoMVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(UoMVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMUoMDA = m_objMUoMDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMUoMDA.Message == string.Empty)
            {
                DataRow m_drMUoMDA = m_dicMUoMDA[0].Tables[0].Rows[0];
                m_objUoMVM.UoMID = m_drMUoMDA[UoMVM.Prop.UoMID.Name].ToString();
                m_objUoMVM.UoMDesc = m_drMUoMDA[UoMVM.Prop.UoMDesc.Name].ToString();
                m_objUoMVM.DimensionID = m_drMUoMDA[DimensionVM.Prop.DimensionID.Name].ToString();
                m_objUoMVM.DimensionDesc = m_drMUoMDA[DimensionVM.Prop.DimensionDesc.Name].ToString();
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMUoMDA.Message;

            return m_objUoMVM;
        }

        #endregion

        #region Public Method

        public Dictionary<int, List<UoMVM>> GetUoMData(bool isCount, string UoMID, string UoMDesc)
        {
            int m_intCount = 0;
            List<UoMVM> m_lstUoMVM = new List<ViewModels.UoMVM>();
            Dictionary<int, List<UoMVM>> m_dicReturn = new Dictionary<int, List<UoMVM>>();
            MUoMDA m_objMUoMDA = new MUoMDA();
            m_objMUoMDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(UoMVM.Prop.UoMID.MapAlias);
            m_lstSelect.Add(UoMVM.Prop.UoMDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(UoMID);
            m_objFilter.Add(UoMVM.Prop.UoMID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(UoMDesc);
            m_objFilter.Add(UoMVM.Prop.UoMDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMUoMDA = m_objMUoMDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMUoMDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpUoMBL in m_dicMUoMDA)
                    {
                        m_intCount = m_kvpUoMBL.Key;
                        break;
                    }
                else
                {
                    m_lstUoMVM = (
                        from DataRow m_drMUoMDA in m_dicMUoMDA[0].Tables[0].Rows
                        select new UoMVM()
                        {
                            UoMID = m_drMUoMDA[UoMVM.Prop.UoMID.Name].ToString(),
                            UoMDesc = m_drMUoMDA[UoMVM.Prop.UoMDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstUoMVM);
            return m_dicReturn;
        }

        #endregion
    }
}