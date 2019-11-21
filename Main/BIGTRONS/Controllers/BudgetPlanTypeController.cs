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
    public class BudgetPlanTypeController : BaseController
    {
        private readonly string title = "Budget Plan Type";
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
            MBudgetPlanTypeDA m_objSBudgetPlanTypeDA = new MBudgetPlanTypeDA();
            m_objSBudgetPlanTypeDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMBudgetPlanType = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMBudgetPlanType.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = BudgetPlanTypeVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicSBudgetPlanTypeDA = m_objSBudgetPlanTypeDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpBudgetPlanTypeBL in m_dicSBudgetPlanTypeDA)
            {
                m_intCount = m_kvpBudgetPlanTypeBL.Key;
                break;
            }

            List<BudgetPlanTypeVM> m_lstBudgetPlanTypeVM = new List<BudgetPlanTypeVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(BudgetPlanTypeVM.Prop.BudgetPlanTypeID.MapAlias);
                m_lstSelect.Add(BudgetPlanTypeVM.Prop.BudgetPlanTypeDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(BudgetPlanTypeVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicSBudgetPlanTypeDA = m_objSBudgetPlanTypeDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objSBudgetPlanTypeDA.Message == string.Empty)
                {
                    m_lstBudgetPlanTypeVM = (
                        from DataRow m_drSBudgetPlanTypeDA in m_dicSBudgetPlanTypeDA[0].Tables[0].Rows
                        select new BudgetPlanTypeVM()
                        {
                            BudgetPlanTypeID = m_drSBudgetPlanTypeDA[BudgetPlanTypeVM.Prop.BudgetPlanTypeID.Name].ToString(),
                            BudgetPlanTypeDesc = m_drSBudgetPlanTypeDA[BudgetPlanTypeVM.Prop.BudgetPlanTypeDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstBudgetPlanTypeVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            MBudgetPlanTypeDA m_objSBudgetPlanTypeDA = new MBudgetPlanTypeDA();
            m_objSBudgetPlanTypeDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMBudgetPlanType = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMBudgetPlanType.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = BudgetPlanTypeVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicSBudgetPlanTypeDA = m_objSBudgetPlanTypeDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpBudgetPlanTypeBL in m_dicSBudgetPlanTypeDA)
            {
                m_intCount = m_kvpBudgetPlanTypeBL.Key;
                break;
            }

            List<BudgetPlanTypeVM> m_lstBudgetPlanTypeVM = new List<BudgetPlanTypeVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(BudgetPlanTypeVM.Prop.BudgetPlanTypeID.MapAlias);
                m_lstSelect.Add(BudgetPlanTypeVM.Prop.BudgetPlanTypeDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(BudgetPlanTypeVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicSBudgetPlanTypeDA = m_objSBudgetPlanTypeDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objSBudgetPlanTypeDA.Message == string.Empty)
                {
                    m_lstBudgetPlanTypeVM = (
                        from DataRow m_drSBudgetPlanTypeDA in m_dicSBudgetPlanTypeDA[0].Tables[0].Rows
                        select new BudgetPlanTypeVM()
                        {
                            BudgetPlanTypeID = m_drSBudgetPlanTypeDA[BudgetPlanTypeVM.Prop.BudgetPlanTypeID.Name].ToString(),
                            BudgetPlanTypeDesc = m_drSBudgetPlanTypeDA[BudgetPlanTypeVM.Prop.BudgetPlanTypeDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstBudgetPlanTypeVM, m_intCount);
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

            BudgetPlanTypeVM m_objBudgetPlanTypeVM = new BudgetPlanTypeVM();
            ViewDataDictionary m_vddBudgetPlanType = new ViewDataDictionary();
            m_vddBudgetPlanType.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddBudgetPlanType.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
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
                Model = m_objBudgetPlanTypeVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddBudgetPlanType,
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

            BudgetPlanTypeVM m_objBudgetPlanTypeVM = new BudgetPlanTypeVM();
            if (m_dicSelectedRow.Count > 0)
                m_objBudgetPlanTypeVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
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
                Model = m_objBudgetPlanTypeVM,
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
            BudgetPlanTypeVM m_objBudgetPlanTypeVM = new BudgetPlanTypeVM();
            if (m_dicSelectedRow.Count > 0)
                m_objBudgetPlanTypeVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddBudgetPlanType = new ViewDataDictionary();
            m_vddBudgetPlanType.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddBudgetPlanType.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objBudgetPlanTypeVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddBudgetPlanType,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<BudgetPlanTypeVM> m_lstSelectedRow = new List<BudgetPlanTypeVM>();
            m_lstSelectedRow = JSON.Deserialize<List<BudgetPlanTypeVM>>(Selected);

            MBudgetPlanTypeDA m_objSBudgetPlanTypeDA = new MBudgetPlanTypeDA();
            m_objSBudgetPlanTypeDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (BudgetPlanTypeVM m_objBudgetPlanTypeVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifBudgetPlanTypeVM = m_objBudgetPlanTypeVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifBudgetPlanTypeVM in m_arrPifBudgetPlanTypeVM)
                    {
                        string m_strFieldName = m_pifBudgetPlanTypeVM.Name;
                        object m_objFieldValue = m_pifBudgetPlanTypeVM.GetValue(m_objBudgetPlanTypeVM);
                        if (m_objBudgetPlanTypeVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(BudgetPlanTypeVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objSBudgetPlanTypeDA.DeleteBC(m_objFilter, false);
                    if (m_objSBudgetPlanTypeDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objSBudgetPlanTypeDA.Message);
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

        public ActionResult Browse(string ControlBudgetPlanTypeID, string ControlBudgetPlanTypeDesc, string FilterBudgetPlanTypeID = "", string FilterBudgetPlanTypeDesc = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddBudgetPlanType = new ViewDataDictionary();
            m_vddBudgetPlanType.Add("Control" + BudgetPlanTypeVM.Prop.BudgetPlanTypeID.Name, ControlBudgetPlanTypeID);
            m_vddBudgetPlanType.Add("Control" + BudgetPlanTypeVM.Prop.BudgetPlanTypeDesc.Name, ControlBudgetPlanTypeDesc);
            m_vddBudgetPlanType.Add(BudgetPlanTypeVM.Prop.BudgetPlanTypeID.Name, FilterBudgetPlanTypeID);
            m_vddBudgetPlanType.Add(BudgetPlanTypeVM.Prop.BudgetPlanTypeDesc.Name, FilterBudgetPlanTypeDesc);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddBudgetPlanType,
                ViewName = "../BudgetPlanType/_Browse"
            };
        }

        public ActionResult Save(string Action)
        {
            //action = JSON.Deserialize<string>(action);
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MBudgetPlanTypeDA m_objSBudgetPlanTypeDA = new MBudgetPlanTypeDA();
            m_objSBudgetPlanTypeDA.ConnectionStringName = Global.ConnStrConfigName;
            try
            {
                string m_strBudgetPlanTypeID = this.Request.Params[BudgetPlanTypeVM.Prop.BudgetPlanTypeID.Name];
                string m_strBudgetPlanTypeDesc = this.Request.Params[BudgetPlanTypeVM.Prop.BudgetPlanTypeDesc.Name];

                m_lstMessage = IsSaveValid(Action, m_strBudgetPlanTypeID, m_strBudgetPlanTypeDesc);
                if (m_lstMessage.Count <= 0)
                {
                    MBudgetPlanType m_objSBudgetPlanType = new MBudgetPlanType();
                    m_objSBudgetPlanType.BudgetPlanTypeID = m_strBudgetPlanTypeID;
                    m_objSBudgetPlanTypeDA.Data = m_objSBudgetPlanType;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objSBudgetPlanTypeDA.Select();

                    m_objSBudgetPlanType.BudgetPlanTypeDesc = m_strBudgetPlanTypeDesc;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objSBudgetPlanTypeDA.Insert(false);
                    else
                        m_objSBudgetPlanTypeDA.Update(false);

                    if (!m_objSBudgetPlanTypeDA.Success || m_objSBudgetPlanTypeDA.Message != string.Empty)
                        m_lstMessage.Add(m_objSBudgetPlanTypeDA.Message);
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

        public ActionResult GetBudgetPlanType(string ControlBudgetPlanTypeID, string ControlBudgetPlanTypeDesc, string FilterBudgetPlanTypeID, string FilterBudgetPlanTypeDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<BudgetPlanTypeVM>> m_dicBudgetPlanTypeData = GetBudgetPlanTypeData(true, FilterBudgetPlanTypeID, FilterBudgetPlanTypeDesc);
                KeyValuePair<int, List<BudgetPlanTypeVM>> m_kvpBudgetPlanTypeVM = m_dicBudgetPlanTypeData.AsEnumerable().ToList()[0];
                if (m_kvpBudgetPlanTypeVM.Key < 1 || (m_kvpBudgetPlanTypeVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpBudgetPlanTypeVM.Key > 1 && !Exact)
                    return Browse(ControlBudgetPlanTypeID, ControlBudgetPlanTypeDesc, FilterBudgetPlanTypeID, FilterBudgetPlanTypeDesc);

                m_dicBudgetPlanTypeData = GetBudgetPlanTypeData(false, FilterBudgetPlanTypeID, FilterBudgetPlanTypeDesc);
                BudgetPlanTypeVM m_objBudgetPlanTypeVM = m_dicBudgetPlanTypeData[0][0];
                this.GetCmp<TextField>(ControlBudgetPlanTypeID).Value = m_objBudgetPlanTypeVM.BudgetPlanTypeID;
                this.GetCmp<TextField>(ControlBudgetPlanTypeDesc).Value = m_objBudgetPlanTypeVM.BudgetPlanTypeDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, string BudgetPlanTypeID, string BudgetPlanTypeDesc)
        {
            List<string> m_lstReturn = new List<string>();

            if (BudgetPlanTypeID == string.Empty)
                m_lstReturn.Add(BudgetPlanTypeVM.Prop.BudgetPlanTypeID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (BudgetPlanTypeDesc == string.Empty)
                m_lstReturn.Add(BudgetPlanTypeVM.Prop.BudgetPlanTypeDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));

            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(BudgetPlanTypeVM.Prop.BudgetPlanTypeID.Name, parameters[BudgetPlanTypeVM.Prop.BudgetPlanTypeID.Name]);
            m_dicReturn.Add(BudgetPlanTypeVM.Prop.BudgetPlanTypeDesc.Name, parameters[BudgetPlanTypeVM.Prop.BudgetPlanTypeDesc.Name]);

            return m_dicReturn;
        }

        private BudgetPlanTypeVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            BudgetPlanTypeVM m_objBudgetPlanTypeVM = new BudgetPlanTypeVM();
            MBudgetPlanTypeDA m_objSBudgetPlanTypeDA = new MBudgetPlanTypeDA();
            m_objSBudgetPlanTypeDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanTypeVM.Prop.BudgetPlanTypeID.MapAlias);
            m_lstSelect.Add(BudgetPlanTypeVM.Prop.BudgetPlanTypeDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objBudgetPlanTypeVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(BudgetPlanTypeVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicSBudgetPlanTypeDA = m_objSBudgetPlanTypeDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objSBudgetPlanTypeDA.Message == string.Empty)
            {
                DataRow m_drSBudgetPlanTypeDA = m_dicSBudgetPlanTypeDA[0].Tables[0].Rows[0];
                m_objBudgetPlanTypeVM.BudgetPlanTypeID = m_drSBudgetPlanTypeDA[BudgetPlanTypeVM.Prop.BudgetPlanTypeID.Name].ToString();
                m_objBudgetPlanTypeVM.BudgetPlanTypeDesc = m_drSBudgetPlanTypeDA[BudgetPlanTypeVM.Prop.BudgetPlanTypeDesc.Name].ToString();
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objSBudgetPlanTypeDA.Message;

            return m_objBudgetPlanTypeVM;
        }

        #endregion

        #region Public Method

        public Dictionary<int, List<BudgetPlanTypeVM>> GetBudgetPlanTypeData(bool isCount, string BudgetPlanTypeID, string BudgetPlanTypeDesc)
        {
            int m_intCount = 0;
            List<BudgetPlanTypeVM> m_lstBudgetPlanTypeVM = new List<ViewModels.BudgetPlanTypeVM>();
            Dictionary<int, List<BudgetPlanTypeVM>> m_dicReturn = new Dictionary<int, List<BudgetPlanTypeVM>>();
            MBudgetPlanTypeDA m_objSBudgetPlanTypeDA = new MBudgetPlanTypeDA();
            m_objSBudgetPlanTypeDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanTypeVM.Prop.BudgetPlanTypeID.MapAlias);
            m_lstSelect.Add(BudgetPlanTypeVM.Prop.BudgetPlanTypeDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(BudgetPlanTypeID);
            m_objFilter.Add(BudgetPlanTypeVM.Prop.BudgetPlanTypeID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(BudgetPlanTypeDesc);
            m_objFilter.Add(BudgetPlanTypeVM.Prop.BudgetPlanTypeDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicSBudgetPlanTypeDA = m_objSBudgetPlanTypeDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objSBudgetPlanTypeDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpBudgetPlanTypeBL in m_dicSBudgetPlanTypeDA)
                    {
                        m_intCount = m_kvpBudgetPlanTypeBL.Key;
                        break;
                    }
                else
                {
                    m_lstBudgetPlanTypeVM = (
                        from DataRow m_drSBudgetPlanTypeDA in m_dicSBudgetPlanTypeDA[0].Tables[0].Rows
                        select new BudgetPlanTypeVM()
                        {
                            BudgetPlanTypeID = m_drSBudgetPlanTypeDA[BudgetPlanTypeVM.Prop.BudgetPlanTypeID.Name].ToString(),
                            BudgetPlanTypeDesc = m_drSBudgetPlanTypeDA[BudgetPlanTypeVM.Prop.BudgetPlanTypeDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstBudgetPlanTypeVM);
            return m_dicReturn;
        }

        #endregion
    }
}