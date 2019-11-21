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
    public class ClusterController : BaseController
    {
        private readonly string title = "Cluster";
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
            MClusterDA m_objMClusterDA = new MClusterDA();
            m_objMClusterDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMCluster = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMCluster.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = ClusterVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMClusterDA = m_objMClusterDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpClusterBL in m_dicMClusterDA)
            {
                m_intCount = m_kvpClusterBL.Key;
                break;
            }

            List<ClusterVM> m_lstClusterVM = new List<ClusterVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(ClusterVM.Prop.ClusterID.MapAlias);
                m_lstSelect.Add(ClusterVM.Prop.ClusterDesc.MapAlias);
                m_lstSelect.Add(ClusterVM.Prop.ProjectID.MapAlias);
                m_lstSelect.Add(ClusterVM.Prop.ProjectDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(ClusterVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMClusterDA = m_objMClusterDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMClusterDA.Message == string.Empty)
                {
                    m_lstClusterVM = (
                        from DataRow m_drMClusterDA in m_dicMClusterDA[0].Tables[0].Rows
                        select new ClusterVM()
                        {
                            ClusterID = m_drMClusterDA[ClusterVM.Prop.ClusterID.Name].ToString(),
                            ClusterDesc = m_drMClusterDA[ClusterVM.Prop.ClusterDesc.Name].ToString(),
                            ProjectID = m_drMClusterDA[ClusterVM.Prop.ProjectID.Name].ToString(),
                            ProjectDesc = m_drMClusterDA[ClusterVM.Prop.ProjectDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstClusterVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters, string ProjectID)
        {
            MClusterDA m_objMClusterDA = new MClusterDA();
            m_objMClusterDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMCluster = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMCluster.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = ClusterVM.Prop.Map(m_strDataIndex, false);
                    
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
            if (!string.IsNullOrEmpty(ProjectID))
            {
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(ProjectID);
                m_objFilter.Add(ClusterVM.Prop.ProjectID.Map, m_lstFilter);
            }
            Dictionary<int, DataSet> m_dicMClusterDA = m_objMClusterDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpClusterBL in m_dicMClusterDA)
            {
                m_intCount = m_kvpClusterBL.Key;
                break;
            }

            List<ClusterVM> m_lstClusterVM = new List<ClusterVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(ClusterVM.Prop.ClusterID.MapAlias);
                m_lstSelect.Add(ClusterVM.Prop.ClusterDesc.MapAlias);
                m_lstSelect.Add(ClusterVM.Prop.ProjectDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(ClusterVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMClusterDA = m_objMClusterDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMClusterDA.Message == string.Empty)
                {
                    m_lstClusterVM = (
                        from DataRow m_drMClusterDA in m_dicMClusterDA[0].Tables[0].Rows
                        select new ClusterVM()
                        {
                            ClusterID = m_drMClusterDA[ClusterVM.Prop.ClusterID.Name].ToString(),
                            ClusterDesc = m_drMClusterDA[ClusterVM.Prop.ClusterDesc.Name].ToString(),
                            ProjectDesc = m_drMClusterDA[ClusterVM.Prop.ProjectDesc.Name].ToString(),
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstClusterVM, m_intCount);
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

            ClusterVM m_objClusterVM = new ClusterVM();
            ViewDataDictionary m_vddCluster = new ViewDataDictionary();
            m_vddCluster.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddCluster.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
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
                Model = m_objClusterVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddCluster,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Detail(string Caller, string Selected)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ClusterVM m_objClusterVM = new ClusterVM();
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
                m_objClusterVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
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
                Model = m_objClusterVM,
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
            ClusterVM m_objClusterVM = new ClusterVM();
            if (m_dicSelectedRow.Count > 0)
                m_objClusterVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddCluster = new ViewDataDictionary();
            m_vddCluster.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddCluster.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objClusterVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddCluster,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<ClusterVM> m_lstSelectedRow = new List<ClusterVM>();
            m_lstSelectedRow = JSON.Deserialize<List<ClusterVM>>(Selected);

            MClusterDA m_objMClusterDA = new MClusterDA();
            m_objMClusterDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (ClusterVM m_objClusterVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifClusterVM = m_objClusterVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifClusterVM in m_arrPifClusterVM)
                    {
                        string m_strFieldName = m_pifClusterVM.Name;
                        object m_objFieldValue = m_pifClusterVM.GetValue(m_objClusterVM);
                        if (m_objClusterVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(ClusterVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objMClusterDA.DeleteBC(m_objFilter, false);
                    if (m_objMClusterDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMClusterDA.Message);
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

        public ActionResult Browse(string ControlClusterID, string ControlClusterDesc, string FilterClusterID = "", string FilterClusterDesc = "", string FilterProjectID = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddCluster = new ViewDataDictionary();
            m_vddCluster.Add("Control" + ClusterVM.Prop.ClusterID.Name, ControlClusterID);
            m_vddCluster.Add("Control" + ClusterVM.Prop.ClusterDesc.Name, ControlClusterDesc);
            m_vddCluster.Add(ClusterVM.Prop.ClusterID.Name, FilterClusterID);
            m_vddCluster.Add(ClusterVM.Prop.ClusterDesc.Name, FilterClusterDesc);
            m_vddCluster.Add(ClusterVM.Prop.ProjectID.Name, FilterProjectID);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddCluster,
                ViewName = "../Cluster/_Browse"
            };
        }

        public ActionResult Save(string Action)
        {
            //action = JSON.Deserialize<string>(action);
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MClusterDA m_objMClusterDA = new MClusterDA();
            m_objMClusterDA.ConnectionStringName = Global.ConnStrConfigName;
            try
            {
                string m_strClusterID = this.Request.Params[ClusterVM.Prop.ClusterID.Name];
                string m_strClusterDesc = this.Request.Params[ClusterVM.Prop.ClusterDesc.Name];
                string m_strProjectID = this.Request.Params[ProjectVM.Prop.ProjectID.Name];

                m_lstMessage = IsSaveValid(Action, m_strClusterID, m_strClusterDesc, m_strProjectID);
                if (m_lstMessage.Count <= 0)
                {
                    MCluster m_objMCluster = new MCluster();
                    m_objMCluster.ClusterID = m_strClusterID;
                    m_objMClusterDA.Data = m_objMCluster;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objMClusterDA.Select();

                    m_objMCluster.ClusterDesc = m_strClusterDesc;
                    m_objMCluster.ProjectID = m_strProjectID;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objMClusterDA.Insert(false);
                    else
                        m_objMClusterDA.Update(false);

                    if (!m_objMClusterDA.Success || m_objMClusterDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMClusterDA.Message);
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

        public ActionResult GetCluster(string ControlClusterID, string ControlClusterDesc, string FilterClusterID, string FilterClusterDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<ClusterVM>> m_dicClusterData = GetClusterData(true, FilterClusterID, FilterClusterDesc);
                KeyValuePair<int, List<ClusterVM>> m_kvpClusterVM = m_dicClusterData.AsEnumerable().ToList()[0];
                if (m_kvpClusterVM.Key < 1 || (m_kvpClusterVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpClusterVM.Key > 1 && !Exact)
                    return Browse(ControlClusterID, ControlClusterDesc, FilterClusterID, FilterClusterDesc);

                m_dicClusterData = GetClusterData(false, FilterClusterID, FilterClusterDesc);
                ClusterVM m_objClusterVM = m_dicClusterData[0][0];
                this.GetCmp<TextField>(ControlClusterID).Value = m_objClusterVM.ClusterID;
                this.GetCmp<TextField>(ControlClusterDesc).Value = m_objClusterVM.ClusterDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, string ClusterID, string ClusterDesc, string ProjectID)
        {
            List<string> m_lstReturn = new List<string>();

            if (ClusterID == string.Empty)
                m_lstReturn.Add(ClusterVM.Prop.ClusterID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (ClusterDesc == string.Empty)
                m_lstReturn.Add(ClusterVM.Prop.ClusterDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (ProjectID == string.Empty)
                m_lstReturn.Add(ClusterVM.Prop.ProjectID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            //if (ProjectID == string.Empty)
            //    m_lstReturn.Add(ClusterVM.Prop.ProjectID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(ClusterVM.Prop.ClusterID.Name, parameters[ClusterVM.Prop.ClusterID.Name]);
            m_dicReturn.Add(ClusterVM.Prop.ClusterDesc.Name, parameters[ClusterVM.Prop.ClusterDesc.Name]);
            m_dicReturn.Add(ClusterVM.Prop.ProjectID.Name, parameters[ClusterVM.Prop.ProjectID.Name]);
            m_dicReturn.Add(ClusterVM.Prop.ProjectDesc.Name, parameters[ClusterVM.Prop.ProjectDesc.Name]);

            return m_dicReturn;
        }

        private ClusterVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            ClusterVM m_objClusterVM = new ClusterVM();
            MClusterDA m_objMClusterDA = new MClusterDA();
            m_objMClusterDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ClusterVM.Prop.ClusterID.MapAlias);
            m_lstSelect.Add(ClusterVM.Prop.ClusterDesc.MapAlias);
            m_lstSelect.Add(ClusterVM.Prop.ProjectID.MapAlias);
            m_lstSelect.Add(ClusterVM.Prop.ProjectDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objClusterVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(ClusterVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMClusterDA = m_objMClusterDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMClusterDA.Message == string.Empty)
            {
                DataRow m_drMClusterDA = m_dicMClusterDA[0].Tables[0].Rows[0];
                m_objClusterVM.ClusterID = m_drMClusterDA[ClusterVM.Prop.ClusterID.Name].ToString();
                m_objClusterVM.ClusterDesc = m_drMClusterDA[ClusterVM.Prop.ClusterDesc.Name].ToString();
                m_objClusterVM.ProjectID = m_drMClusterDA[ClusterVM.Prop.ProjectID.Name].ToString();
                m_objClusterVM.ProjectDesc = m_drMClusterDA[ClusterVM.Prop.ProjectDesc.Name].ToString();
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMClusterDA.Message;

            return m_objClusterVM;
        }

        #endregion

        #region Public Method

        public Dictionary<int, List<ClusterVM>> GetClusterData(bool isCount, string ClusterID, string ClusterDesc)
        {
            int m_intCount = 0;
            List<ClusterVM> m_lstClusterVM = new List<ViewModels.ClusterVM>();
            Dictionary<int, List<ClusterVM>> m_dicReturn = new Dictionary<int, List<ClusterVM>>();
            MClusterDA m_objMClusterDA = new MClusterDA();
            m_objMClusterDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ClusterVM.Prop.ClusterID.MapAlias);
            m_lstSelect.Add(ClusterVM.Prop.ClusterDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(ClusterID);
            m_objFilter.Add(ClusterVM.Prop.ClusterID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(ClusterDesc);
            m_objFilter.Add(ClusterVM.Prop.ClusterDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMClusterDA = m_objMClusterDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMClusterDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpClusterBL in m_dicMClusterDA)
                    {
                        m_intCount = m_kvpClusterBL.Key;
                        break;
                    }
                else
                {
                    m_lstClusterVM = (
                        from DataRow m_drMClusterDA in m_dicMClusterDA[0].Tables[0].Rows
                        select new ClusterVM()
                        {
                            ClusterID = m_drMClusterDA[ClusterVM.Prop.ClusterID.Name].ToString(),
                            ClusterDesc = m_drMClusterDA[ClusterVM.Prop.ClusterDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstClusterVM);
            return m_dicReturn;
        }

        #endregion
    }
}