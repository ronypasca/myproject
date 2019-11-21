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
    public class WBSController : BaseController
    {
        private readonly string title = "Work Breakdown Structure";
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
            MWBSDA m_objMWBSDA = new MWBSDA();
            m_objMWBSDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMWBS = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMWBS.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = WBSVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMWBSDA = m_objMWBSDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpWBSBL in m_dicMWBSDA)
            {
                m_intCount = m_kvpWBSBL.Key;
                break;
            }

            List<WBSVM> m_lstWBSVM = new List<WBSVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(WBSVM.Prop.WBSID.MapAlias);
                m_lstSelect.Add(WBSVM.Prop.WBSDesc.MapAlias);
                m_lstSelect.Add(WBSVM.Prop.ProjectDesc.MapAlias);
                m_lstSelect.Add(WBSVM.Prop.CompanyDesc.MapAlias);
                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(WBSVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMWBSDA = m_objMWBSDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMWBSDA.Message == string.Empty)
                {
                    m_lstWBSVM = (
                        from DataRow m_drMWBSDA in m_dicMWBSDA[0].Tables[0].Rows
                        select new WBSVM()
                        {
                            WBSID = m_drMWBSDA[WBSVM.Prop.WBSID.Name].ToString(),
                            WBSDesc = m_drMWBSDA[WBSVM.Prop.WBSDesc.Name].ToString(),
                            ProjectDesc = m_drMWBSDA[WBSVM.Prop.ProjectDesc.Name].ToString(),
                            CompanyDesc = m_drMWBSDA[WBSVM.Prop.CompanyDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstWBSVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            MWBSDA m_objMWBSDA = new MWBSDA();
            m_objMWBSDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMWBS = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMWBS.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = WBSVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMWBSDA = m_objMWBSDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpWBSBL in m_dicMWBSDA)
            {
                m_intCount = m_kvpWBSBL.Key;
                break;
            }

            List<WBSVM> m_lstWBSVM = new List<WBSVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(WBSVM.Prop.WBSID.MapAlias);
                m_lstSelect.Add(WBSVM.Prop.WBSDesc.MapAlias);
                m_lstSelect.Add(WBSVM.Prop.ProjectDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(WBSVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMWBSDA = m_objMWBSDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMWBSDA.Message == string.Empty)
                {
                    m_lstWBSVM = (
                        from DataRow m_drMWBSDA in m_dicMWBSDA[0].Tables[0].Rows
                        select new WBSVM()
                        {
                            WBSID = m_drMWBSDA[WBSVM.Prop.WBSID.Name].ToString(),
                            WBSDesc = m_drMWBSDA[WBSVM.Prop.WBSDesc.Name].ToString(),
                            ProjectDesc = m_drMWBSDA[WBSVM.Prop.ProjectDesc.Name].ToString(),
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstWBSVM, m_intCount);
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

            WBSVM m_objWBSVM = new WBSVM();
            ViewDataDictionary m_vddWBS = new ViewDataDictionary();
            m_vddWBS.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddWBS.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
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
                Model = m_objWBSVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddWBS,
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
            
            WBSVM m_objWBSVM = new WBSVM();
            if (m_dicSelectedRow.Count > 0)
                m_objWBSVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
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
                Model = m_objWBSVM,
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
            WBSVM m_objWBSVM = new WBSVM();
            if (m_dicSelectedRow.Count > 0)
                m_objWBSVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddWBS = new ViewDataDictionary();
            m_vddWBS.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddWBS.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objWBSVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddWBS,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<WBSVM> m_lstSelectedRow = new List<WBSVM>();
            m_lstSelectedRow = JSON.Deserialize<List<WBSVM>>(Selected);

            MWBSDA m_objMWBSDA = new MWBSDA();
            m_objMWBSDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (WBSVM m_objWBSVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifWBSVM = m_objWBSVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifWBSVM in m_arrPifWBSVM)
                    {
                        string m_strFieldName = m_pifWBSVM.Name;
                        object m_objFieldValue = m_pifWBSVM.GetValue(m_objWBSVM);
                        if (m_objWBSVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(WBSVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objMWBSDA.DeleteBC(m_objFilter, false);
                    if (m_objMWBSDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMWBSDA.Message);
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

        public ActionResult Browse(string ControlWBSID, string ControlWBSDesc, string FilterWBSID = "", string FilterWBSDesc = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddWBS = new ViewDataDictionary();
            m_vddWBS.Add("Control" + WBSVM.Prop.WBSID.Name, ControlWBSID);
            m_vddWBS.Add("Control" + WBSVM.Prop.WBSDesc.Name, ControlWBSDesc);
            m_vddWBS.Add(WBSVM.Prop.WBSID.Name, FilterWBSID);
            m_vddWBS.Add(WBSVM.Prop.WBSDesc.Name, FilterWBSDesc);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddWBS,
                ViewName = "../WBS/_Browse"
            };
        }

        public ActionResult Save(string Action)
        {
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MWBSDA m_objMWBSDA = new MWBSDA();
            m_objMWBSDA.ConnectionStringName = Global.ConnStrConfigName;
            try
            {
                string m_strWBSID = this.Request.Params[WBSVM.Prop.WBSID.Name];
                string m_strWBSDesc = this.Request.Params[WBSVM.Prop.WBSDesc.Name];
                string m_strProjectID = this.Request.Params[ProjectVM.Prop.ProjectID.Name];
                

                m_lstMessage = IsSaveValid(Action, m_strWBSID, m_strWBSDesc, m_strProjectID);
                if (m_lstMessage.Count <= 0)
                {
                    MWBS m_objMWBS = new MWBS();
                    m_objMWBS.WBSID = m_strWBSID;
                    m_objMWBSDA.Data = m_objMWBS;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objMWBSDA.Select();

                    m_objMWBS.WBSDesc = m_strWBSDesc;
                    m_objMWBS.ProjectID = m_strProjectID;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objMWBSDA.Insert(false);
                    else
                        m_objMWBSDA.Update(false);

                    if (!m_objMWBSDA.Success || m_objMWBSDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMWBSDA.Message);
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

        public ActionResult GetWBS(string ControlWBSID, string ControlWBSDesc, string FilterWBSID, string FilterWBSDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<WBSVM>> m_dicWBSData = GetWBSData(true, FilterWBSID, FilterWBSDesc);
                KeyValuePair<int, List<WBSVM>> m_kvpWBSVM = m_dicWBSData.AsEnumerable().ToList()[0];
                if (m_kvpWBSVM.Key < 1 || (m_kvpWBSVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpWBSVM.Key > 1 && !Exact)
                    return Browse(ControlWBSID, ControlWBSDesc, FilterWBSID, FilterWBSDesc);

                m_dicWBSData = GetWBSData(false, FilterWBSID, FilterWBSDesc);
                WBSVM m_objWBSVM = m_dicWBSData[0][0];
                this.GetCmp<TextField>(ControlWBSID).Value = m_objWBSVM.WBSID;
                this.GetCmp<TextField>(ControlWBSDesc).Value = m_objWBSVM.WBSDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, string WBSID, string WBSDesc, string ProjectID)
        {
            List<string> m_lstReturn = new List<string>();

            if (WBSID == string.Empty)
                m_lstReturn.Add(WBSVM.Prop.WBSID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (WBSDesc == string.Empty)
                m_lstReturn.Add(WBSVM.Prop.WBSDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (ProjectID == string.Empty)
                m_lstReturn.Add(WBSVM.Prop.ProjectID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            //if (ProjectID == string.Empty)
            //    m_lstReturn.Add(WBSVM.Prop.ProjectID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(WBSVM.Prop.WBSID.Name, parameters[WBSVM.Prop.WBSID.Name]);
            m_dicReturn.Add(WBSVM.Prop.WBSDesc.Name, parameters[WBSVM.Prop.WBSDesc.Name]);
            m_dicReturn.Add(WBSVM.Prop.ProjectID.Name, parameters[WBSVM.Prop.ProjectID.Name]);
            m_dicReturn.Add(WBSVM.Prop.ProjectDesc.Name, parameters[WBSVM.Prop.ProjectDesc.Name]);
            m_dicReturn.Add(WBSVM.Prop.CompanyDesc.Name, parameters[WBSVM.Prop.CompanyDesc.Name]);
            return m_dicReturn;
        }

        private WBSVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            WBSVM m_objWBSVM = new WBSVM();
            MWBSDA m_objMWBSDA = new MWBSDA();
            m_objMWBSDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(WBSVM.Prop.WBSID.MapAlias);
            m_lstSelect.Add(WBSVM.Prop.WBSDesc.MapAlias);
            m_lstSelect.Add(WBSVM.Prop.ProjectID.MapAlias);
            m_lstSelect.Add(WBSVM.Prop.ProjectDesc.MapAlias);
            m_lstSelect.Add(WBSVM.Prop.CompanyDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objWBSVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(WBSVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMWBSDA = m_objMWBSDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMWBSDA.Message == string.Empty)
            {
                DataRow m_drMWBSDA = m_dicMWBSDA[0].Tables[0].Rows[0];
                m_objWBSVM.WBSID = m_drMWBSDA[WBSVM.Prop.WBSID.Name].ToString();
                m_objWBSVM.WBSDesc = m_drMWBSDA[WBSVM.Prop.WBSDesc.Name].ToString();
                m_objWBSVM.ProjectID = m_drMWBSDA[WBSVM.Prop.ProjectID.Name].ToString();
                m_objWBSVM.ProjectDesc = m_drMWBSDA[WBSVM.Prop.ProjectDesc.Name].ToString();
                m_objWBSVM.CompanyDesc= m_drMWBSDA[WBSVM.Prop.CompanyDesc.Name].ToString();
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMWBSDA.Message;

            return m_objWBSVM;
        }

        #endregion

        #region Public Method

        public Dictionary<int, List<WBSVM>> GetWBSData(bool isCount, string WBSID, string WBSDesc)
        {
            int m_intCount = 0;
            List<WBSVM> m_lstWBSVM = new List<ViewModels.WBSVM>();
            Dictionary<int, List<WBSVM>> m_dicReturn = new Dictionary<int, List<WBSVM>>();
            MWBSDA m_objMWBSDA = new MWBSDA();
            m_objMWBSDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(WBSVM.Prop.WBSID.MapAlias);
            m_lstSelect.Add(WBSVM.Prop.WBSDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(WBSID);
            m_objFilter.Add(WBSVM.Prop.WBSID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(WBSDesc);
            m_objFilter.Add(WBSVM.Prop.WBSDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMWBSDA = m_objMWBSDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMWBSDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpWBSBL in m_dicMWBSDA)
                    {
                        m_intCount = m_kvpWBSBL.Key;
                        break;
                    }
                else
                {
                    m_lstWBSVM = (
                        from DataRow m_drMWBSDA in m_dicMWBSDA[0].Tables[0].Rows
                        select new WBSVM()
                        {
                            WBSID = m_drMWBSDA[WBSVM.Prop.WBSID.Name].ToString(),
                            WBSDesc = m_drMWBSDA[WBSVM.Prop.WBSDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstWBSVM);
            return m_dicReturn;
        }

        #endregion
    }
}