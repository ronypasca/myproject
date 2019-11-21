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
    public class DimensionController : BaseController
    {
        private readonly string title = "Dimension";
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
            SDimensionDA m_objSDimensionDA = new SDimensionDA();
            m_objSDimensionDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMDimension = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMDimension.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = DimensionVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicSDimensionDA = m_objSDimensionDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpDimensionBL in m_dicSDimensionDA)
            {
                m_intCount = m_kvpDimensionBL.Key;
                break;
            }

            List<DimensionVM> m_lstDimensionVM = new List<DimensionVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(DimensionVM.Prop.DimensionID.MapAlias);
                m_lstSelect.Add(DimensionVM.Prop.DimensionDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(DimensionVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicSDimensionDA = m_objSDimensionDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objSDimensionDA.Message == string.Empty)
                {
                    m_lstDimensionVM = (
                        from DataRow m_drSDimensionDA in m_dicSDimensionDA[0].Tables[0].Rows
                        select new DimensionVM()
                        {
                            DimensionID = m_drSDimensionDA[DimensionVM.Prop.DimensionID.Name].ToString(),
                            DimensionDesc = m_drSDimensionDA[DimensionVM.Prop.DimensionDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstDimensionVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            SDimensionDA m_objSDimensionDA = new SDimensionDA();
            m_objSDimensionDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMDimension = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMDimension.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = DimensionVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicSDimensionDA = m_objSDimensionDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpDimensionBL in m_dicSDimensionDA)
            {
                m_intCount = m_kvpDimensionBL.Key;
                break;
            }

            List<DimensionVM> m_lstDimensionVM = new List<DimensionVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(DimensionVM.Prop.DimensionID.MapAlias);
                m_lstSelect.Add(DimensionVM.Prop.DimensionDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(DimensionVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicSDimensionDA = m_objSDimensionDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objSDimensionDA.Message == string.Empty)
                {
                    m_lstDimensionVM = (
                        from DataRow m_drSDimensionDA in m_dicSDimensionDA[0].Tables[0].Rows
                        select new DimensionVM()
                        {
                            DimensionID = m_drSDimensionDA[DimensionVM.Prop.DimensionID.Name].ToString(),
                            DimensionDesc = m_drSDimensionDA[DimensionVM.Prop.DimensionDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstDimensionVM, m_intCount);
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

            DimensionVM m_objDimensionVM = new DimensionVM();
            ViewDataDictionary m_vddDimension = new ViewDataDictionary();
            m_vddDimension.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddDimension.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
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
                Model = m_objDimensionVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddDimension,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Detail(string Caller, string Selected)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            DimensionVM m_objDimensionVM = new DimensionVM();
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
                m_objDimensionVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
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
                Model = m_objDimensionVM,
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
            DimensionVM m_objDimensionVM = new DimensionVM();
            if (m_dicSelectedRow.Count > 0)
                m_objDimensionVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddDimension = new ViewDataDictionary();
            m_vddDimension.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddDimension.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objDimensionVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddDimension,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<DimensionVM> m_lstSelectedRow = new List<DimensionVM>();
            m_lstSelectedRow = JSON.Deserialize<List<DimensionVM>>(Selected);

            SDimensionDA m_objSDimensionDA = new SDimensionDA();
            m_objSDimensionDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (DimensionVM m_objDimensionVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifDimensionVM = m_objDimensionVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifDimensionVM in m_arrPifDimensionVM)
                    {
                        string m_strFieldName = m_pifDimensionVM.Name;
                        object m_objFieldValue = m_pifDimensionVM.GetValue(m_objDimensionVM);
                        if (m_objDimensionVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(DimensionVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objSDimensionDA.DeleteBC(m_objFilter, false);
                    if (m_objSDimensionDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objSDimensionDA.Message);
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

        public ActionResult Browse(string ControlDimensionID, string ControlDimensionDesc, string FilterDimensionID = "", string FilterDimensionDesc = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddDimension = new ViewDataDictionary();
            m_vddDimension.Add("Control" + DimensionVM.Prop.DimensionID.Name, ControlDimensionID);
            m_vddDimension.Add("Control" + DimensionVM.Prop.DimensionDesc.Name, ControlDimensionDesc);
            m_vddDimension.Add(DimensionVM.Prop.DimensionID.Name, FilterDimensionID);
            m_vddDimension.Add(DimensionVM.Prop.DimensionDesc.Name, FilterDimensionDesc);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddDimension,
                ViewName = "../Dimension/_Browse"
            };
        }

        public ActionResult Save(string Action)
        {
            //action = JSON.Deserialize<string>(action);
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            SDimensionDA m_objSDimensionDA = new SDimensionDA();
            m_objSDimensionDA.ConnectionStringName = Global.ConnStrConfigName;
            try
            {
                string m_strDimensionID = this.Request.Params[DimensionVM.Prop.DimensionID.Name];
                string m_strDimensionDesc = this.Request.Params[DimensionVM.Prop.DimensionDesc.Name];

                m_lstMessage = IsSaveValid(Action, m_strDimensionID, m_strDimensionDesc);
                if (m_lstMessage.Count <= 0)
                {
                    SDimension m_objSDimension = new SDimension();
                    m_objSDimension.DimensionID = m_strDimensionID;
                    m_objSDimensionDA.Data = m_objSDimension;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objSDimensionDA.Select();

                    m_objSDimension.DimensionDesc = m_strDimensionDesc;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objSDimensionDA.Insert(false);
                    else
                        m_objSDimensionDA.Update(false);

                    if (!m_objSDimensionDA.Success || m_objSDimensionDA.Message != string.Empty)
                        m_lstMessage.Add(m_objSDimensionDA.Message);
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

        public ActionResult GetDimension(string ControlDimensionID, string ControlDimensionDesc, string FilterDimensionID, string FilterDimensionDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<DimensionVM>> m_dicDimensionData = GetDimensionData(true, FilterDimensionID, FilterDimensionDesc);
                KeyValuePair<int, List<DimensionVM>> m_kvpDimensionVM = m_dicDimensionData.AsEnumerable().ToList()[0];
                if (m_kvpDimensionVM.Key < 1 || (m_kvpDimensionVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpDimensionVM.Key > 1 && !Exact)
                    return Browse(ControlDimensionID, ControlDimensionDesc, FilterDimensionID, FilterDimensionDesc);

                m_dicDimensionData = GetDimensionData(false, FilterDimensionID, FilterDimensionDesc);
                DimensionVM m_objDimensionVM = m_dicDimensionData[0][0];
                this.GetCmp<TextField>(ControlDimensionID).Value = m_objDimensionVM.DimensionID;
                this.GetCmp<TextField>(ControlDimensionDesc).Value = m_objDimensionVM.DimensionDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, string DimensionID, string DimensionDesc)
        {
            List<string> m_lstReturn = new List<string>();

            if (DimensionID == string.Empty)
                m_lstReturn.Add(DimensionVM.Prop.DimensionID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (DimensionDesc == string.Empty)
                m_lstReturn.Add(DimensionVM.Prop.DimensionDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));

            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(DimensionVM.Prop.DimensionID.Name, parameters[DimensionVM.Prop.DimensionID.Name]);
            m_dicReturn.Add(DimensionVM.Prop.DimensionDesc.Name, parameters[DimensionVM.Prop.DimensionDesc.Name]);

            return m_dicReturn;
        }

        private DimensionVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            DimensionVM m_objDimensionVM = new DimensionVM();
            SDimensionDA m_objSDimensionDA = new SDimensionDA();
            m_objSDimensionDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(DimensionVM.Prop.DimensionID.MapAlias);
            m_lstSelect.Add(DimensionVM.Prop.DimensionDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objDimensionVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(DimensionVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicSDimensionDA = m_objSDimensionDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objSDimensionDA.Message == string.Empty)
            {
                DataRow m_drSDimensionDA = m_dicSDimensionDA[0].Tables[0].Rows[0];
                m_objDimensionVM.DimensionID = m_drSDimensionDA[DimensionVM.Prop.DimensionID.Name].ToString();
                m_objDimensionVM.DimensionDesc = m_drSDimensionDA[DimensionVM.Prop.DimensionDesc.Name].ToString();
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objSDimensionDA.Message;

            return m_objDimensionVM;
        }

        #endregion

        #region Public Method

        public Dictionary<int, List<DimensionVM>> GetDimensionData(bool isCount, string DimensionID, string DimensionDesc)
        {
            int m_intCount = 0;
            List<DimensionVM> m_lstDimensionVM = new List<ViewModels.DimensionVM>();
            Dictionary<int, List<DimensionVM>> m_dicReturn = new Dictionary<int, List<DimensionVM>>();
            SDimensionDA m_objSDimensionDA = new SDimensionDA();
            m_objSDimensionDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(DimensionVM.Prop.DimensionID.MapAlias);
            m_lstSelect.Add(DimensionVM.Prop.DimensionDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(DimensionID);
            m_objFilter.Add(DimensionVM.Prop.DimensionID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(DimensionDesc);
            m_objFilter.Add(DimensionVM.Prop.DimensionDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicSDimensionDA = m_objSDimensionDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objSDimensionDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpDimensionBL in m_dicSDimensionDA)
                    {
                        m_intCount = m_kvpDimensionBL.Key;
                        break;
                    }
                else
                {
                    m_lstDimensionVM = (
                        from DataRow m_drSDimensionDA in m_dicSDimensionDA[0].Tables[0].Rows
                        select new DimensionVM()
                        {
                            DimensionID = m_drSDimensionDA[DimensionVM.Prop.DimensionID.Name].ToString(),
                            DimensionDesc = m_drSDimensionDA[DimensionVM.Prop.DimensionDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstDimensionVM);
            return m_dicReturn;
        }

        #endregion
    }
}