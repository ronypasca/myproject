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
    public class ApprovalPathController : BaseController
    {
        private readonly string title = "Approval Path";
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
            CApprovalPathDA m_objCApprovalPathDA = new CApprovalPathDA();
            m_objCApprovalPathDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcCApprovalPath = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcCApprovalPath.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = ApprovalPathVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicCApprovalPathDA = m_objCApprovalPathDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpApprovalPathBL in m_dicCApprovalPathDA)
            {
                m_intCount = m_kvpApprovalPathBL.Key;
                break;
            }

            List<ApprovalPathVM> m_lstApprovalPathVM = new List<ApprovalPathVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(ApprovalPathVM.Prop.ApprovalPathID.MapAlias);
                m_lstSelect.Add(ApprovalPathVM.Prop.RoleID.MapAlias);
                m_lstSelect.Add(ApprovalPathVM.Prop.RoleDesc.MapAlias);
                m_lstSelect.Add(ApprovalPathVM.Prop.RoleParentID.MapAlias);
                m_lstSelect.Add(ApprovalPathVM.Prop.RoleParentDesc.MapAlias);
                m_lstSelect.Add(ApprovalPathVM.Prop.RoleChildID.MapAlias);
                m_lstSelect.Add(ApprovalPathVM.Prop.TaskTypeID.MapAlias);
                m_lstSelect.Add(ApprovalPathVM.Prop.TaskTypeDesc.MapAlias);
                m_lstSelect.Add(ApprovalPathVM.Prop.RoleChildDesc.MapAlias);
                m_lstSelect.Add(ApprovalPathVM.Prop.StartDate.MapAlias);
                m_lstSelect.Add(ApprovalPathVM.Prop.EndDate.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(ApprovalPathVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicCApprovalPathDA = m_objCApprovalPathDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objCApprovalPathDA.Message == string.Empty)
                {
                    m_lstApprovalPathVM = (
                        from DataRow m_drCApprovalPathDA in m_dicCApprovalPathDA[0].Tables[0].Rows
                        select new ApprovalPathVM()
                        {
                            ApprovalPathID = m_drCApprovalPathDA[ApprovalPathVM.Prop.ApprovalPathID.Name].ToString(),
                            RoleID = m_drCApprovalPathDA[ApprovalPathVM.Prop.RoleID.Name].ToString(),
                            RoleDesc = m_drCApprovalPathDA[ApprovalPathVM.Prop.RoleDesc.Name].ToString(),
                            RoleParentID = m_drCApprovalPathDA[ApprovalPathVM.Prop.RoleParentID.Name].ToString(),
                            RoleParentDesc = m_drCApprovalPathDA[ApprovalPathVM.Prop.RoleParentDesc.Name].ToString(),
                            RoleChildID = m_drCApprovalPathDA[ApprovalPathVM.Prop.RoleChildID.Name].ToString(),
                            RoleChildDesc = m_drCApprovalPathDA[ApprovalPathVM.Prop.RoleChildDesc.Name].ToString(),
                            TaskTypeID = m_drCApprovalPathDA[ApprovalPathVM.Prop.TaskTypeID.Name].ToString(),
                            TaskTypeDesc = m_drCApprovalPathDA[ApprovalPathVM.Prop.TaskTypeDesc.Name].ToString(),
                            StartDate = (DateTime)m_drCApprovalPathDA[ApprovalPathVM.Prop.StartDate.Name],
                            EndDate = (DateTime)m_drCApprovalPathDA[ApprovalPathVM.Prop.EndDate.Name],

                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstApprovalPathVM, m_intCount);
        }
        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            CApprovalPathDA m_objCApprovalPathDA = new CApprovalPathDA();
            m_objCApprovalPathDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcCApprovalPath = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcCApprovalPath.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = ApprovalPathVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicCApprovalPathDA = m_objCApprovalPathDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpApprovalPathBL in m_dicCApprovalPathDA)
            {
                m_intCount = m_kvpApprovalPathBL.Key;
                break;
            }

            List<ApprovalPathVM> m_lstApprovalPathVM = new List<ApprovalPathVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(ApprovalPathVM.Prop.ApprovalPathID.MapAlias);
                m_lstSelect.Add(ApprovalPathVM.Prop.RoleID.MapAlias);
                m_lstSelect.Add(ApprovalPathVM.Prop.RoleParentID.MapAlias);
                m_lstSelect.Add(ApprovalPathVM.Prop.RoleChildID.MapAlias);
                m_lstSelect.Add(ApprovalPathVM.Prop.TaskTypeID.MapAlias);
                m_lstSelect.Add(ApprovalPathVM.Prop.StartDate.MapAlias);
                m_lstSelect.Add(ApprovalPathVM.Prop.EndDate.MapAlias);


                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(ApprovalPathVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicCApprovalPathDA = m_objCApprovalPathDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objCApprovalPathDA.Message == string.Empty)
                {
                    m_lstApprovalPathVM = (
                        from DataRow m_drCApprovalPathDA in m_dicCApprovalPathDA[0].Tables[0].Rows
                        select new ApprovalPathVM()
                        {
                            ApprovalPathID = m_drCApprovalPathDA[ApprovalPathVM.Prop.ApprovalPathID.Name].ToString(),
                            RoleID = m_drCApprovalPathDA[ApprovalPathVM.Prop.RoleID.Name].ToString(),
                            RoleParentID = m_drCApprovalPathDA[ApprovalPathVM.Prop.RoleParentID.Name].ToString(),
                            RoleChildID = m_drCApprovalPathDA[ApprovalPathVM.Prop.RoleChildID.Name].ToString(),
                            TaskTypeID = m_drCApprovalPathDA[ApprovalPathVM.Prop.TaskTypeID.Name].ToString(),
                            StartDate = (DateTime)m_drCApprovalPathDA[ApprovalPathVM.Prop.StartDate.Name],
                            EndDate = (DateTime)m_drCApprovalPathDA[ApprovalPathVM.Prop.EndDate.Name]
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstApprovalPathVM, m_intCount);
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

            ApprovalPathVM m_objApprovalPathVM = new ApprovalPathVM();
            ViewDataDictionary m_vddApprovalPath = new ViewDataDictionary();
            m_vddApprovalPath.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddApprovalPath.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
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
                Model = m_objApprovalPathVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddApprovalPath,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult Detail(string Caller, string Selected, string ApprovalPathID)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            string m_strMessage = string.Empty;
            ApprovalPathVM m_objApprovalPathVM = new ApprovalPathVM();
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
                m_dicSelectedRow = GetFormData(m_nvcParams, ApprovalPathID);
            }

            if (m_dicSelectedRow.Count > 0)
                m_objApprovalPathVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
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
                Model = m_objApprovalPathVM,
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
            ApprovalPathVM m_objApprovalPathVM = new ApprovalPathVM();
            if (m_dicSelectedRow.Count > 0)
                m_objApprovalPathVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddApprovalPath = new ViewDataDictionary();
            m_vddApprovalPath.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddApprovalPath.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objApprovalPathVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddApprovalPath,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<ApprovalPathVM> m_lstSelectedRow = new List<ApprovalPathVM>();
            m_lstSelectedRow = JSON.Deserialize<List<ApprovalPathVM>>(Selected);

            CApprovalPathDA m_objCApprovalPathDA = new CApprovalPathDA();
            m_objCApprovalPathDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (ApprovalPathVM m_objApprovalPathVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifApprovalPathVM = m_objApprovalPathVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifApprovalPathVM in m_arrPifApprovalPathVM)
                    {
                        string m_strFieldName = m_pifApprovalPathVM.Name;
                        object m_objFieldValue = m_pifApprovalPathVM.GetValue(m_objApprovalPathVM);
                        if (m_objApprovalPathVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(ApprovalPathVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objCApprovalPathDA.DeleteBC(m_objFilter, false);
                    if (m_objCApprovalPathDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objCApprovalPathDA.Message);
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
        public ActionResult Browse(string ControlApprovalPathID, string ControlRoleID, string ControlRoleParentID, string ControlRoleChildID, string ControlEndDate, string ControlLocationDesc, string ControlRegionID, string ControlRegionDesc, string ValueRoleChildID = "", string FilterApprovalPathID = "", string FilterRoleID = "")
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddApprovalPath = new ViewDataDictionary();
            m_vddApprovalPath.Add("Control" + ApprovalPathVM.Prop.ApprovalPathID.Name, ControlApprovalPathID);
            m_vddApprovalPath.Add("Control" + ApprovalPathVM.Prop.RoleID.Name, ControlRoleID);
            m_vddApprovalPath.Add("Control" + ApprovalPathVM.Prop.RoleParentID.Name, ControlRoleChildID);
            m_vddApprovalPath.Add("Control" + ApprovalPathVM.Prop.RoleChildID.Name, ControlRoleParentID);
            m_vddApprovalPath.Add("Control" + ApprovalPathVM.Prop.StartDate.Name, ControlEndDate);
            m_vddApprovalPath.Add("Control" + ApprovalPathVM.Prop.EndDate.Name, ControlLocationDesc);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddApprovalPath,
                ViewName = "../ApprovalPath/_Browse"
            };
        }
        public ActionResult Save(string Action)
        {
            //action = JSON.Deserialize<string>(action);
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            CApprovalPathDA m_objCApprovalPathDA = new CApprovalPathDA();
            string m_strApprovalPathID = string.Empty;
            m_objCApprovalPathDA.ConnectionStringName = Global.ConnStrConfigName;
            try
            {
                m_strApprovalPathID = this.Request.Params[ApprovalPathVM.Prop.ApprovalPathID.Name];
                string m_strRoleID = this.Request.Params[ApprovalPathVM.Prop.RoleID.Name];
                string m_strRoleParentID = this.Request.Params[ApprovalPathVM.Prop.RoleParentID.Name];
                string m_strRoleChildID = this.Request.Params[ApprovalPathVM.Prop.RoleChildID.Name];
                DateTime m_strStartDate = (!string.IsNullOrEmpty(Request.Params[ApprovalPathVM.Prop.StartDate.Name].ToString())) ? Convert.ToDateTime(Request.Params[ApprovalPathVM.Prop.StartDate.Name]) : DateTime.MinValue;
                DateTime m_strEndDate = (!string.IsNullOrEmpty(Request.Params[ApprovalPathVM.Prop.EndDate.Name].ToString())) ? Convert.ToDateTime(Request.Params[ApprovalPathVM.Prop.EndDate.Name]) : DateTime.MinValue;
                string m_strTaskTypeID = this.Request.Params[ApprovalPathVM.Prop.TaskTypeID.Name];

                m_lstMessage = IsSaveValid(Action, m_strApprovalPathID, m_strRoleID, m_strRoleParentID, m_strRoleChildID, m_strStartDate, m_strEndDate, m_strTaskTypeID);
                if (m_lstMessage.Count <= 0)
                {
                    CApprovalPath m_objCApprovalPath = new CApprovalPath();
                    m_objCApprovalPath.ApprovalPathID = m_strApprovalPathID;
                    m_objCApprovalPathDA.Data = m_objCApprovalPath;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objCApprovalPathDA.Select();

                    m_objCApprovalPath.RoleID = m_strRoleID;
                    m_objCApprovalPath.RoleParentID = m_strRoleParentID;
                    m_objCApprovalPath.RoleChildID = m_strRoleChildID;
                    m_objCApprovalPath.StartDate = m_strStartDate;
                    m_objCApprovalPath.EndDate = m_strEndDate;
                    m_objCApprovalPath.TaskTypeID = m_strTaskTypeID;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objCApprovalPathDA.Insert(false);
                    else
                        m_objCApprovalPathDA.Update(false);

                    if (!m_objCApprovalPathDA.Success || m_objCApprovalPathDA.Message != string.Empty)
                        m_lstMessage.Add(m_objCApprovalPathDA.Message);
                    m_strApprovalPathID = m_objCApprovalPathDA.Data.ApprovalPathID;
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
            }
            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
                return Detail(General.EnumDesc(Buttons.ButtonSave), null, m_strApprovalPathID);
            }
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);
        }
        #endregion

        #region Direct Method

        public ActionResult GetApprovalPath(string ControlApprovalPathID, string ControlRoleID, string ControlRoleParentID, string ControlRoleChildID, string ControlStartDate, string ControlEndDate,
            string FilterApprovalPathID, string FilterRoleID, bool Exact = false, string RoleChildID = "")
        {
            try
            {
                //Dictionary<int, List<ApprovalPathVM>> m_dicApprovalPathData = GetApprovalPathData(true, RoleChildID, FilterApprovalPathID, FilterRoleID);
                //KeyValuePair<int, List<ApprovalPathVM>> m_kvpApprovalPathVM = m_dicApprovalPathData.AsEnumerable().ToList()[0];
                //if (m_kvpApprovalPathVM.Key < 1 || (m_kvpApprovalPathVM.Key > 1 && Exact))
                //    return this.Direct(false);
                //else if (m_kvpApprovalPathVM.Key > 1 && !Exact)
                //    return Browse(ControlApprovalPathID, ControlRoleID, ControlRoleParentID, ControlRoleChildID, ControlEndDate, ControlLocationDesc, ControlRegionID, ControlRegionDesc, RoleChildID, FilterApprovalPathID, FilterRoleID);

                //m_dicApprovalPathData = GetApprovalPathData(false, RoleChildID, FilterApprovalPathID, FilterRoleID);
                //ApprovalPathVM m_objApprovalPathVM = m_dicApprovalPathData[0][0];
                //this.GetCmp<TextField>(ControlApprovalPathID).Value = m_objApprovalPathVM.ApprovalPathID;
                //this.GetCmp<TextField>(ControlRoleID).Value = m_objApprovalPathVM.RoleID;
                //if (!string.IsNullOrEmpty(ControlRoleParentID)) this.GetCmp<TextField>(ControlRoleParentID).Value = m_objApprovalPathVM.RoleParentID;
                //if (!string.IsNullOrEmpty(ControlRoleChildID)) this.GetCmp<TextField>(ControlRoleChildID).Value = m_objApprovalPathVM.RoleChildID;
                //if (!string.IsNullOrEmpty(ControlStartDate)) this.GetCmp<TextField>(ControlLocationDesc).Value = m_objApprovalPathVM.StartDate;
                //if (!string.IsNullOrEmpty(ControlEndDate)) this.GetCmp<TextField>(ControlEndDate).Value = m_objApprovalPathVM.EndDate;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method
        private List<string> IsSaveValid(string Action, string ApprovalPathID, string RoleID, string RoleParentID, string RoleChildID, DateTime StartDate, DateTime EndDate, string TaskTypeID)
        {
            List<string> m_lstReturn = new List<string>();

            if (string.IsNullOrEmpty(TaskTypeID))
                m_lstReturn.Add(ApprovalPathVM.Prop.TaskTypeID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (RoleID == string.Empty)
                m_lstReturn.Add(ApprovalPathVM.Prop.RoleID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            //if (RoleParentID == string.Empty)
            //    m_lstReturn.Add(ApprovalPathVM.Prop.RoleParentID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            //if (RoleChildID == string.Empty)
            //    m_lstReturn.Add(ApprovalPathVM.Prop.RoleChildID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (RoleChildID == string.Empty && RoleParentID == string.Empty)
                m_lstReturn.Add(ApprovalPathVM.Prop.RoleParentID.Desc + " or " + ApprovalPathVM.Prop.RoleChildID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (StartDate == DateTime.MinValue)
                m_lstReturn.Add(ApprovalPathVM.Prop.StartDate.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (EndDate == DateTime.MinValue)
                m_lstReturn.Add(ApprovalPathVM.Prop.EndDate.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (StartDate > EndDate)
                m_lstReturn.Add(ApprovalPathVM.Prop.StartDate.Desc + " " + ApprovalPathVM.Prop.EndDate.Desc + " " + General.EnumDesc(MessageLib.NotMatched));
            if (RoleID != string.Empty && RoleID == RoleParentID)
                m_lstReturn.Add("Can't assign parent role to self");

            return m_lstReturn;
        }
        private Dictionary<string, object> GetFormData(NameValueCollection parameters, string ApprovalPathID = "")
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(ApprovalPathVM.Prop.ApprovalPathID.Name, (parameters[ApprovalPathVM.Prop.ApprovalPathID.Name].ToString() == string.Empty ? ApprovalPathID : parameters[ApprovalPathVM.Prop.ApprovalPathID.Name]));
            m_dicReturn.Add(ApprovalPathVM.Prop.RoleID.Name, parameters[ApprovalPathVM.Prop.RoleID.Name]);
            m_dicReturn.Add(ApprovalPathVM.Prop.RoleParentID.Name, parameters[ApprovalPathVM.Prop.RoleParentID.Name]);
            m_dicReturn.Add(ApprovalPathVM.Prop.RoleChildID.Name, parameters[ApprovalPathVM.Prop.RoleChildID.Name]);
            m_dicReturn.Add(ApprovalPathVM.Prop.StartDate.Name, parameters[ApprovalPathVM.Prop.StartDate.Name]);
            m_dicReturn.Add(ApprovalPathVM.Prop.EndDate.Name, parameters[ApprovalPathVM.Prop.EndDate.Name]);


            return m_dicReturn;
        }
        private ApprovalPathVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            ApprovalPathVM m_objApprovalPathVM = new ApprovalPathVM();
            CApprovalPathDA m_objCApprovalPathDA = new CApprovalPathDA();
            m_objCApprovalPathDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ApprovalPathVM.Prop.ApprovalPathID.MapAlias);
            m_lstSelect.Add(ApprovalPathVM.Prop.RoleID.MapAlias);
            m_lstSelect.Add(ApprovalPathVM.Prop.RoleDesc.MapAlias);
            m_lstSelect.Add(ApprovalPathVM.Prop.RoleParentID.MapAlias);
            m_lstSelect.Add(ApprovalPathVM.Prop.RoleParentDesc.MapAlias);
            m_lstSelect.Add(ApprovalPathVM.Prop.RoleChildID.MapAlias);
            m_lstSelect.Add(ApprovalPathVM.Prop.RoleChildDesc.MapAlias);
            m_lstSelect.Add(ApprovalPathVM.Prop.StartDate.MapAlias);
            m_lstSelect.Add(ApprovalPathVM.Prop.EndDate.MapAlias);
            m_lstSelect.Add(ApprovalPathVM.Prop.TaskTypeID.MapAlias);
            m_lstSelect.Add(ApprovalPathVM.Prop.TaskTypeDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objApprovalPathVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(ApprovalPathVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicCApprovalPathDA = m_objCApprovalPathDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objCApprovalPathDA.Message == string.Empty)
            {
                DataRow m_drCApprovalPathDA = m_dicCApprovalPathDA[0].Tables[0].Rows[0];
                m_objApprovalPathVM.ApprovalPathID = m_drCApprovalPathDA[ApprovalPathVM.Prop.ApprovalPathID.Name].ToString();
                m_objApprovalPathVM.RoleID = m_drCApprovalPathDA[ApprovalPathVM.Prop.RoleID.Name].ToString();
                m_objApprovalPathVM.RoleDesc = m_drCApprovalPathDA[ApprovalPathVM.Prop.RoleDesc.Name].ToString();
                m_objApprovalPathVM.RoleParentID = m_drCApprovalPathDA[ApprovalPathVM.Prop.RoleParentID.Name].ToString();
                m_objApprovalPathVM.RoleParentDesc = m_drCApprovalPathDA[ApprovalPathVM.Prop.RoleParentDesc.Name].ToString();
                m_objApprovalPathVM.RoleChildID = m_drCApprovalPathDA[ApprovalPathVM.Prop.RoleChildID.Name].ToString();
                m_objApprovalPathVM.RoleChildDesc = m_drCApprovalPathDA[ApprovalPathVM.Prop.RoleChildDesc.Name].ToString();
                m_objApprovalPathVM.StartDate = (DateTime)m_drCApprovalPathDA[ApprovalPathVM.Prop.StartDate.Name];
                m_objApprovalPathVM.EndDate = (DateTime)m_drCApprovalPathDA[ApprovalPathVM.Prop.EndDate.Name];
                m_objApprovalPathVM.TaskTypeID = m_drCApprovalPathDA[ApprovalPathVM.Prop.TaskTypeID.Name].ToString();
                m_objApprovalPathVM.TaskTypeDesc = m_drCApprovalPathDA[ApprovalPathVM.Prop.TaskTypeDesc.Name].ToString();

            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objCApprovalPathDA.Message;

            return m_objApprovalPathVM;
        }
        #endregion

        #region Public Method
        public Dictionary<int, List<ApprovalPathVM>> GetApprovalPathData(bool isCount, string RoleChildID, string ApprovalPathID, string RoleID)
        {
            int m_intCount = 0;
            List<ApprovalPathVM> m_lstApprovalPathVM = new List<ViewModels.ApprovalPathVM>();
            Dictionary<int, List<ApprovalPathVM>> m_dicReturn = new Dictionary<int, List<ApprovalPathVM>>();
            CApprovalPathDA m_objCApprovalPathDA = new CApprovalPathDA();
            m_objCApprovalPathDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ApprovalPathVM.Prop.ApprovalPathID.MapAlias);
            m_lstSelect.Add(ApprovalPathVM.Prop.RoleID.MapAlias);
            m_lstSelect.Add(ApprovalPathVM.Prop.RoleParentID.MapAlias);
            m_lstSelect.Add(ApprovalPathVM.Prop.RoleChildID.MapAlias);
            m_lstSelect.Add(ApprovalPathVM.Prop.StartDate.MapAlias);
            m_lstSelect.Add(ApprovalPathVM.Prop.EndDate.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(ApprovalPathID);
            m_objFilter.Add(ApprovalPathVM.Prop.ApprovalPathID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(RoleID);
            m_objFilter.Add(ApprovalPathVM.Prop.RoleID.Map, m_lstFilter);

            if (!string.IsNullOrEmpty(RoleChildID))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Contains);
                m_lstFilter.Add(RoleChildID);
                m_objFilter.Add(ApprovalPathVM.Prop.RoleChildID.Map, m_lstFilter);
            }

            Dictionary<int, DataSet> m_dicCApprovalPathDA = m_objCApprovalPathDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objCApprovalPathDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpApprovalPathBL in m_dicCApprovalPathDA)
                    {
                        m_intCount = m_kvpApprovalPathBL.Key;
                        break;
                    }
                else
                {
                    m_lstApprovalPathVM = (
                        from DataRow m_drCApprovalPathDA in m_dicCApprovalPathDA[0].Tables[0].Rows
                        select new ApprovalPathVM()
                        {
                            ApprovalPathID = m_drCApprovalPathDA[ApprovalPathVM.Prop.ApprovalPathID.Name].ToString(),
                            RoleID = m_drCApprovalPathDA[ApprovalPathVM.Prop.RoleID.Name].ToString(),
                            RoleParentID = m_drCApprovalPathDA[ApprovalPathVM.Prop.RoleParentID.Name].ToString(),
                            RoleChildID = m_drCApprovalPathDA[ApprovalPathVM.Prop.RoleChildID.Name].ToString(),
                            StartDate = (DateTime)m_drCApprovalPathDA[ApprovalPathVM.Prop.StartDate.Name],
                            EndDate = (DateTime)m_drCApprovalPathDA[ApprovalPathVM.Prop.EndDate.Name]
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstApprovalPathVM);
            return m_dicReturn;
        }

        #endregion
    }
}