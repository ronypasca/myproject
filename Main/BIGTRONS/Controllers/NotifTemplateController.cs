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
using RazorEngine;
using RazorEngine.Templating;
using RazorEngine.Configuration;
using System.Security.Policy;
using System.Security;
using System.Security.Permissions;

namespace com.SML.BIGTRONS.Controllers
{
    public class NotifTemplateController : BaseController
    {
        private readonly string title = "Notification Template";
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
            MNotificationTemplatesDA m_objMNotificationTemplatesDA = new MNotificationTemplatesDA();
            m_objMNotificationTemplatesDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            List<string> AllRoleID = new List<string>();
            string message = "";
            AllRoleID = GetAllRoleID(ref message);
            bool AllowedSystemTemplate = AllRoleID.Any(x => x == "PWR");

            FilterHeaderConditions m_fhcMNotificationTemplates = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMNotificationTemplates.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = NotificationTemplateVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMNotificationTemplatesDA = m_objMNotificationTemplatesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpNotificationTemplateBL in m_dicMNotificationTemplatesDA)
            {
                m_intCount = m_kvpNotificationTemplateBL.Key;
                break;
            }

            List<NotificationTemplateVM> m_lstNotificationTemplateVM = new List<NotificationTemplateVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(NotificationTemplateVM.Prop.NotificationTemplateID.MapAlias);
                m_lstSelect.Add(NotificationTemplateVM.Prop.NotificationTemplateDesc.MapAlias);

                if (!AllowedSystemTemplate)
                {
                    List<string> lst_AvoidTemplateID = new List<string>();
                    List<ConfigVM> lstConf = GetConfig("NotificationTemplate", null, "Disabled", null);
                    if (lstConf.Any())
                        foreach (ConfigVM obj in lstConf)
                            lst_AvoidTemplateID.Add(obj.Key2);


                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.NotIn);
                    m_lstFilter.Add(string.Join(",", lst_AvoidTemplateID));

                    if (!m_objFilter.Keys.Any(x => x == NotificationTemplateVM.Prop.NotificationTemplateID.Map))
                        m_objFilter.Add(NotificationTemplateVM.Prop.NotificationTemplateID.Map, m_lstFilter);
                    else
                    {
                        List<object> m_lstFilterss = new List<object>();
                        m_lstFilterss.Add(Operator.None);
                        m_lstFilterss.Add(string.Empty);
                        m_objFilter.Add(string.Format("{1} NOT IN ('{0}')", string.Join("','", lst_AvoidTemplateID), NotificationTemplateVM.Prop.NotificationTemplateID.Map), m_lstFilterss);
                    }
                }

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(NotificationTemplateVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMNotificationTemplatesDA = m_objMNotificationTemplatesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMNotificationTemplatesDA.Message == string.Empty)
                {
                    m_lstNotificationTemplateVM = (
                        from DataRow m_drMNotificationTemplatesDA in m_dicMNotificationTemplatesDA[0].Tables[0].Rows
                        select new NotificationTemplateVM()
                        {
                            NotificationTemplateID = m_drMNotificationTemplatesDA[NotificationTemplateVM.Prop.NotificationTemplateID.Name].ToString(),
                            NotificationTemplateDesc = m_drMNotificationTemplatesDA[NotificationTemplateVM.Prop.NotificationTemplateDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstNotificationTemplateVM, m_lstNotificationTemplateVM.Count);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters, string FunctionID = "")
        {
            MNotificationTemplatesDA m_objMNotificationTemplatesDA = new MNotificationTemplatesDA();
            m_objMNotificationTemplatesDA.ConnectionStringName = Global.ConnStrConfigName;

            DNotificationMapDA m_objMNotificationMapDA = new DNotificationMapDA();
            m_objMNotificationMapDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMNotificationTemplates = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMNotificationTemplates.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = NotificationTemplateVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMNotificationTemplatesDA = m_objMNotificationTemplatesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpNotificationTemplateBL in m_dicMNotificationTemplatesDA)
            {
                m_intCount = m_kvpNotificationTemplateBL.Key;
                break;
            }

            List<NotificationTemplateVM> m_lstNotificationTemplateVM = new List<NotificationTemplateVM>();
            if (m_intCount > 0)
            {
                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(FunctionID);
                if (FunctionID.Length > 0)
                    m_objFilter.Add(FunctionsVM.Prop.FunctionID.Map, m_lstFilter);

                
                List<string> lst_AvoidTemplateID = new List<string>();
                List<ConfigVM> lstConf = GetConfig("NotificationTemplate", null, "Disabled", null);
                if (lstConf.Any())
                    foreach (ConfigVM obj in lstConf)
                        lst_AvoidTemplateID.Add(obj.Key2);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.NotIn);
                m_lstFilter.Add(string.Join(",", lst_AvoidTemplateID));

                if (!m_objFilter.Keys.Any(x => x == NotificationTemplateVM.Prop.NotificationTemplateID.Map))
                    m_objFilter.Add(NotificationTemplateVM.Prop.NotificationTemplateID.Map, m_lstFilter);
                else
                {
                    List<object> m_lstFilterss = new List<object>();
                    m_lstFilterss.Add(Operator.None);
                    m_lstFilterss.Add(string.Empty);
                    m_objFilter.Add(string.Format("{1} NOT IN ('{0}')", string.Join("','", lst_AvoidTemplateID), NotificationTemplateVM.Prop.NotificationTemplateID.Map), m_lstFilterss);
                }
                

                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(NotificationTemplateVM.Prop.NotificationTemplateID.MapAlias);
                m_lstSelect.Add(NotificationTemplateVM.Prop.NotificationTemplateDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(NotificationTemplateVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                if (FunctionID.Length > 0)
                {

                    m_lstSelect.Add(FunctionsVM.Prop.FunctionID.MapAlias);
                    List<string> m_lstGroup = new List<string>();
                    m_lstGroup.Add(NotificationTemplateVM.Prop.NotificationTemplateID.Map);
                    m_lstGroup.Add(NotificationTemplateVM.Prop.NotificationTemplateDesc.Map);
                    m_lstGroup.Add(FunctionsVM.Prop.FunctionID.Map);

                    m_dicMNotificationTemplatesDA = m_objMNotificationMapDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, m_lstGroup, m_dicOrder, null);
                    if (m_objMNotificationMapDA.Message == string.Empty)
                    {
                        m_lstNotificationTemplateVM = (
                            from DataRow m_drMNotificationTemplatesDA in m_dicMNotificationTemplatesDA[0].Tables[0].Rows
                            select new NotificationTemplateVM()
                            {
                                NotificationTemplateID = m_drMNotificationTemplatesDA[NotificationTemplateVM.Prop.NotificationTemplateID.Name].ToString(),
                                NotificationTemplateDesc = m_drMNotificationTemplatesDA[NotificationTemplateVM.Prop.NotificationTemplateDesc.Name].ToString()
                            }
                        ).ToList();
                    }
                }
                else
                {
                    m_dicMNotificationTemplatesDA = m_objMNotificationTemplatesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                    if (m_objMNotificationTemplatesDA.Message == string.Empty)
                    {
                        m_lstNotificationTemplateVM = (
                            from DataRow m_drMNotificationTemplatesDA in m_dicMNotificationTemplatesDA[0].Tables[0].Rows
                            select new NotificationTemplateVM()
                            {
                                NotificationTemplateID = m_drMNotificationTemplatesDA[NotificationTemplateVM.Prop.NotificationTemplateID.Name].ToString(),
                                NotificationTemplateDesc = m_drMNotificationTemplatesDA[NotificationTemplateVM.Prop.NotificationTemplateDesc.Name].ToString()
                            }
                        ).ToList();
                    }
                }
            }
            return this.Store(m_lstNotificationTemplateVM, m_intCount);
        }

        public ActionResult ReadFunction(StoreRequestParameters parameters, string NotificationTemplateID)
        {
            MFunctionsDA m_objDFPTFunctionsDA = new MFunctionsDA();
            m_objDFPTFunctionsDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcDFPTFunctionss = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcDFPTFunctionss.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = FunctionsVM.Prop.Map(m_strDataIndex, false);
                    m_lstFilter = new List<object>();
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

            //m_lstFilter = new List<object>();
            //m_lstFilter.Add(Operator.Equals);
            //m_lstFilter.Add(NotificationTemplateID);
            //m_objFilter.Add(FunctionsVM.Prop.FunctionID.Map, m_lstFilter);

            //List<string> m_lstGroup = new List<string>();
            //m_lstGroup.Add(FunctionsVM.Prop.FunctionID.Map);
            //m_lstGroup.Add(FunctionsVM.Prop.FunctionDesc.Map);

            Dictionary<int, DataSet> m_dicDFPTFunctionsDA = m_objDFPTFunctionsDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpFPTFunctionsBL in m_dicDFPTFunctionsDA)
            {
                m_intCount = m_kvpFPTFunctionsBL.Key;
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

                m_dicDFPTFunctionsDA = m_objDFPTFunctionsDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                List<NotificationMapVM> m_lstNotificationMapVM = GetListNoticationMap(NotificationTemplateID);
                if (m_objDFPTFunctionsDA.Message == string.Empty)
                {
                    m_lstFunctionsVM = (
                        from DataRow m_drDFPTFunctionsDA in m_dicDFPTFunctionsDA[0].Tables[0].Rows
                        join lstnotifmap in m_lstNotificationMapVM on m_drDFPTFunctionsDA[FunctionsVM.Prop.FunctionID.Name].ToString() equals lstnotifmap.FunctionID into v
                        from lstnotifmap in v.DefaultIfEmpty()
                        select new FunctionsVM()
                        {
                            FunctionID = m_drDFPTFunctionsDA[FunctionsVM.Prop.FunctionID.Name].ToString(),
                            FunctionDesc = m_drDFPTFunctionsDA[FunctionsVM.Prop.FunctionDesc.Name].ToString(),
                            Checked = lstnotifmap==null? false: true,
                            IsDefault = lstnotifmap==null? false: lstnotifmap.IsDefault

                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstFunctionsVM, m_intCount);
        }

        public ActionResult GetFieldTags(StoreRequestParameters parameters)
        {
            MFieldTagReferencesDA m_objMFieldTagReferencesDA = new MFieldTagReferencesDA();
            m_objMFieldTagReferencesDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcDFPTFunctionss = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcDFPTFunctionss.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = FunctionsVM.Prop.Map(m_strDataIndex, false);
                    m_lstFilter = new List<object>();
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

            if(!string.IsNullOrEmpty(parameters.Query))
            {
                string m_strQuery = parameters.Query.Replace("@Model.", "");
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Contains);
                m_lstFilter.Add(m_strQuery);
                m_objFilter.Add(FieldTagReferenceVM.Prop.FieldTagID.Map, m_lstFilter);
            }
            //List<string> m_lstGroup = new List<string>();
            //m_lstGroup.Add(FunctionsVM.Prop.FunctionID.Map);
            //m_lstGroup.Add(FunctionsVM.Prop.FunctionDesc.Map);

            Dictionary<int, DataSet> m_dicMFieldTagReferencesDA = m_objMFieldTagReferencesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpFPTFunctionsBL in m_dicMFieldTagReferencesDA)
            {
                m_intCount = m_kvpFPTFunctionsBL.Key;
                break;
            }

            List<FieldTagReferenceVM> m_lstFieldTagReferenceVM = new List<FieldTagReferenceVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(FieldTagReferenceVM.Prop.FieldTagID.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(FunctionsVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMFieldTagReferencesDA = m_objMFieldTagReferencesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMFieldTagReferencesDA.Message == string.Empty)
                {
                    m_lstFieldTagReferenceVM = (
                        from DataRow m_drDFPTFunctionsDA in m_dicMFieldTagReferencesDA[0].Tables[0].Rows
                        select new FieldTagReferenceVM()
                        {
                            FieldTagID = m_drDFPTFunctionsDA[FieldTagReferenceVM.Prop.FieldTagID.Name].ToString(),
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstFieldTagReferenceVM);
        }

        public ActionResult Home()
        {
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 0;
            return this.Direct();
        }

        [ValidateInput(false)]
        public ActionResult Add(string Caller)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Add)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            NotificationTemplateVM m_objNotificationTemplateVM = new NotificationTemplateVM();
            ViewDataDictionary m_vddNotificationTemplate = new ViewDataDictionary();
            m_vddNotificationTemplate.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddNotificationTemplate.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
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
                Model = m_objNotificationTemplateVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddNotificationTemplate,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        [ValidateInput(false)]
        public ActionResult Detail(string Caller, string Selected)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            string m_strMessage = string.Empty;
            NotificationTemplateVM m_objNotificationTemplateVM = new NotificationTemplateVM();
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
                m_objNotificationTemplateVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }
            
            //Dictionary<string,object> viewBags = new Dictionary<string, object>();
            //foreach (var item in m_objNotificationTemplateVM.Tags)
            //{
            //    viewBags.Add(item, "test-"+item);
            //}
            
            //string body = Global.GenerateUsingRazor(m_objNotificationTemplateVM.NotificationTemplateID, m_objNotificationTemplateVM.Contents, new DynamicViewBag(viewBags));

            ViewDataDictionary m_vddNotificationTemplate = new ViewDataDictionary();
            m_vddNotificationTemplate.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objNotificationTemplateVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddNotificationTemplate,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        [ValidateInput(false)]
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
            NotificationTemplateVM m_objNotificationTemplateVM = new NotificationTemplateVM();
            if (m_dicSelectedRow.Count > 0)
                m_objNotificationTemplateVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddNotificationTemplate = new ViewDataDictionary();
            m_vddNotificationTemplate.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddNotificationTemplate.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objNotificationTemplateVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddNotificationTemplate,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<NotificationTemplateVM> m_lstSelectedRow = new List<NotificationTemplateVM>();
            m_lstSelectedRow = JSON.Deserialize<List<NotificationTemplateVM>>(Selected);

            MNotificationTemplatesDA m_objMNotificationTemplatesDA = new MNotificationTemplatesDA();
            m_objMNotificationTemplatesDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (NotificationTemplateVM m_objNotificationTemplateVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifNotificationTemplateVM = m_objNotificationTemplateVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifNotificationTemplateVM in m_arrPifNotificationTemplateVM)
                    {
                        string m_strFieldName = m_pifNotificationTemplateVM.Name;
                        object m_objFieldValue = m_pifNotificationTemplateVM.GetValue(m_objNotificationTemplateVM);
                        if (m_objNotificationTemplateVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(NotificationTemplateVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objMNotificationTemplatesDA.DeleteBC(m_objFilter, false);
                    if (m_objMNotificationTemplatesDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMNotificationTemplatesDA.Message);
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

        public ActionResult Browse(string ControlNotificationTemplateID, string ControlNotificationTemplateDesc, string FilterTemplateFunctionID, string FilterNotificationTemplateID = "", string FilterNotificationTemplateDesc = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddNotificationTemplate = new ViewDataDictionary();
            m_vddNotificationTemplate.Add("Control" + NotificationTemplateVM.Prop.NotificationTemplateID.Name, ControlNotificationTemplateID);
            m_vddNotificationTemplate.Add("Control" + NotificationTemplateVM.Prop.NotificationTemplateDesc.Name, ControlNotificationTemplateDesc);
            m_vddNotificationTemplate.Add(NotificationTemplateVM.Prop.NotificationTemplateID.Name, FilterNotificationTemplateID);
            m_vddNotificationTemplate.Add(NotificationTemplateVM.Prop.NotificationTemplateDesc.Name, FilterNotificationTemplateDesc);
            m_vddNotificationTemplate.Add(FunctionsVM.Prop.FunctionID.Name, FilterTemplateFunctionID);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddNotificationTemplate,
                ViewName = "../NotifTemplate/_Browse"
            };
        }

        [ValidateInput(false)]
        public ActionResult Save(string Action)
        {
            //action = JSON.Deserialize<string>(action);
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            MNotificationTemplates m_objMNotificationTemplates = new MNotificationTemplates();
            List<string> m_lstMessage = new List<string>();
            MNotificationTemplatesDA m_objMNotificationTemplatesDA = new MNotificationTemplatesDA();
            m_objMNotificationTemplatesDA.ConnectionStringName = Global.ConnStrConfigName;

            try
            {
                string m_strNotificationTemplateID = this.Request.Params[NotificationTemplateVM.Prop.NotificationTemplateID.Name];
                string m_strNotificationTemplateDesc = this.Request.Params[NotificationTemplateVM.Prop.NotificationTemplateDesc.Name];
                string m_strContent = this.Request.Params["CKEDITOR" + NotificationTemplateVM.Prop.Contents.Name];

                string m_lstFunction = this.Request.Params[NotificationTemplateVM.Prop.ListFunctionsVM.Name];

                List<FunctionsVM> m_lstFunctionVM = JSON.Deserialize<List<FunctionsVM>>(m_lstFunction);

                string m_lstFieldTags = this.Request.Params[NotificationTemplateVM.Prop.ListFieldTagReferenceVM.Name];

                //List<FieldTagReferenceVM> m_lstFieldTagsVM = JSON.Deserialize<List<FieldTagReferenceVM>>(m_lstFieldTags);
                
                m_lstMessage = IsSaveValid(Action, m_strNotificationTemplateID, m_strNotificationTemplateDesc);
                if (m_lstMessage.Count <= 0)
                {
                   
                    m_objMNotificationTemplates.NotificationTemplateID = m_strNotificationTemplateID;
                    m_objMNotificationTemplatesDA.Data = m_objMNotificationTemplates;

                    string m_strTransName = "NotifTemplate";
                    object m_objDBConnection = m_objMNotificationTemplatesDA.BeginTrans(m_strTransName);

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objMNotificationTemplatesDA.Select();

                    m_objMNotificationTemplates.NotificationTemplateDesc = m_strNotificationTemplateDesc;
                    m_objMNotificationTemplates.Contents = m_strContent;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objMNotificationTemplatesDA.Insert(true, m_objDBConnection);
                    else
                        m_objMNotificationTemplatesDA.Update(true, m_objDBConnection);

                    if (!m_objMNotificationTemplatesDA.Success || m_objMNotificationTemplatesDA.Message != string.Empty)
                    {
                        m_objMNotificationTemplatesDA.EndTrans(ref m_objDBConnection, m_strTransName);
                        return this.Direct(false, m_objMNotificationTemplatesDA.Message);
                    }

                    System.Text.RegularExpressions.MatchCollection m_match = System.Text.RegularExpressions.Regex.Matches(m_strContent, @"\[\[(.+?)\]\]");
                    List<string> m_lsTags = new List<string>();
                    foreach (System.Text.RegularExpressions.Match item in m_match)
                    {
                        m_lsTags.Add(item.Groups[1].Value);
                    }

                    #region DNotificationMap

                    DNotificationMapDA m_objDNotificationMapDA = new DNotificationMapDA();
                    m_objDNotificationMapDA.ConnectionStringName = Global.ConnStrConfigName;

                    if (Action == General.EnumDesc(Buttons.ButtonUpdate))
                    {
                        Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                        List<object> m_lstFilter = new List<object>();
                        m_lstFilter.Add(Operator.Equals);
                        m_lstFilter.Add(m_strNotificationTemplateID);
                        m_objFilter.Add(NotificationMapVM.Prop.NotificationTemplateID.Map, m_lstFilter);

                        m_objDNotificationMapDA.DeleteBC(m_objFilter, true, m_objDBConnection);

                        if (!m_objDNotificationMapDA.Success)
                        {
                            m_objDNotificationMapDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                            return this.Direct(false, m_objDNotificationMapDA.Message);
                        }
                    }

                    foreach (FunctionsVM objFunctionsVM in m_lstFunctionVM)
                    {   //UPDATE: if IsDefault is true
                        if (objFunctionsVM.IsDefault)
                        {
                            Dictionary<string, List<object>> m_dicSet = new Dictionary<string, List<object>>();
                            Dictionary<string, List<object>> m_dicFilter = new Dictionary<string, List<object>>();
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(objFunctionsVM.FunctionID);
                            m_dicFilter.Add(NotificationMapVM.Prop.FunctionID.Map, m_lstFilter);

                            List<object> m_lstSet = new List<object>();
                            m_lstSet.Add(typeof(int));
                            m_lstSet.Add(0);
                            m_dicSet.Add(NotificationMapVM.Prop.IsDefault.Map, m_lstSet);

                            m_objDNotificationMapDA.UpdateBC(m_dicSet, m_dicFilter, true, m_objDBConnection);
                            if (!m_objDNotificationMapDA.Success)
                            {
                                m_lstMessage.Add(m_objDNotificationMapDA.Message);
                                m_objDNotificationMapDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                            }
                        }

                        DNotificationMap m_objDNotificationMap = new DNotificationMap();
                        m_objDNotificationMap.NotifMapID = Guid.NewGuid().ToString().Replace("-", "");

                        m_objDNotificationMapDA.Data = m_objDNotificationMap;
                        m_objDNotificationMapDA.Select();

                        m_objDNotificationMap.NotificationTemplateID = m_strNotificationTemplateID;
                        m_objDNotificationMap.FunctionID = objFunctionsVM.FunctionID;
                        m_objDNotificationMap.IsDefault = objFunctionsVM.IsDefault;

                        

                        if (m_objDNotificationMapDA.Message != string.Empty)
                            m_objDNotificationMapDA.Insert(true, m_objDBConnection);

                        if (!m_objDNotificationMapDA.Success || m_objDNotificationMapDA.Message != string.Empty)
                        {
                            m_objDNotificationMapDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                            return this.Direct(false, m_objDNotificationMapDA.Message);
                        }
                    }


                    #endregion

                    #region DTemplateTags
                    m_lstFieldTags = string.Join(",", m_lsTags);
                    if (!string.IsNullOrEmpty(m_lstFieldTags) && m_lstFieldTags.Split(',').Length > 0)
                    {
                        DTemplateTagsDA m_objDTemplateTagsDA = new DTemplateTagsDA();
                        m_objDTemplateTagsDA.ConnectionStringName = Global.ConnStrConfigName;

                        if (Action == General.EnumDesc(Buttons.ButtonUpdate))
                        {
                            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_strNotificationTemplateID);
                            m_objFilter.Add(TemplateTagsVM.Prop.TemplateID.Map, m_lstFilter);

                            m_objDTemplateTagsDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                        }

                        foreach (string m_strFieldTagReferenceVM in m_lstFieldTags.Split(','))
                        {
                            DTemplateTags m_objDTemplateTags = new DTemplateTags();
                            m_objDTemplateTags.TemplateTagID = Guid.NewGuid().ToString().Replace("-", "");
                            m_objDTemplateTags.TemplateID = m_strNotificationTemplateID;
                            m_objDTemplateTags.FieldTagID = m_strFieldTagReferenceVM;
                            m_objDTemplateTags.TemplateType = "NOT";

                            m_objDTemplateTagsDA.Data = m_objDTemplateTags;
                            m_objDTemplateTagsDA.Select();

                            if (m_objDTemplateTagsDA.Message != string.Empty)
                                m_objDTemplateTagsDA.Insert(true, m_objDBConnection);

                            if (!m_objDTemplateTagsDA.Success || m_objDTemplateTagsDA.Message != string.Empty)
                            {
                                m_objDTemplateTagsDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                                return this.Direct(false, m_objDTemplateTagsDA.Message);
                            }
                        }

                    }
                    #endregion


                    if (!m_objMNotificationTemplatesDA.Success || m_objMNotificationTemplatesDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMNotificationTemplatesDA.Message);
                    else
                        m_objMNotificationTemplatesDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
            }
            if (m_lstMessage.Count <= 0)
            {
                //try
                //{
                //    //if (AppDomain.CurrentDomain.IsDefaultAppDomain())
                //    //{
                //    //    // RazorEngine cannot clean up from the default appdomain...
                //    //    //Console.WriteLine("Switching to secound AppDomain, for RazorEngine...");
                //    //    AppDomainSetup adSetup = new AppDomainSetup();
                //    //    adSetup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                //    //    var current = AppDomain.CurrentDomain;
                //    //    // You only need to add strongnames when your appdomain is not a full trust environment.
                //    //    var strongNames = new StrongName[0];

                //    //    var domain = AppDomain.CreateDomain(
                //    //        "MyMainDomain", null,
                //    //        current.SetupInformation, new PermissionSet(PermissionState.Unrestricted),
                //    //        strongNames);
                //    //    var exitCode = domain.ExecuteAssembly(Assembly.GetExecutingAssembly().Location);
                //    //    // RazorEngine will cleanup. 
                //    //    AppDomain.Unload(domain);
                //    //}

                //    //var config = new TemplateServiceConfiguration();
                //    //config.DisableTempFileLocking = true;
                //    //config.CachingProvider = new DefaultCachingProvider(t => { });
                //    //Engine.Razor = RazorEngineService.Create(config);

                //    //Engine.Razor.AddTemplate(m_objMNotificationTemplates.NotificationTemplateID, m_objMNotificationTemplates.Contents);
                //    //Engine.Razor.Compile(m_objMNotificationTemplates.NotificationTemplateID, null);
                //}
                //catch 
                //{
                //    Global.ShowErrorAlert(title, "Invalid Template!");
                //    return this.Direct(true);
                //}
                Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
                return Detail(General.EnumDesc(Buttons.ButtonSave), null);
            }
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);
        }

        #endregion

        #region Direct Method

        public ActionResult GetNotificationTemplate(string ControlNotificationTemplateID, string ControlNotificationTemplateDesc, string FilterNotificationTemplateID, string FilterNotificationTemplateDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<NotificationTemplateVM>> m_dicNotificationTemplateData = GetNotificationTemplateData(true, FilterNotificationTemplateID, FilterNotificationTemplateDesc);
                KeyValuePair<int, List<NotificationTemplateVM>> m_kvpNotificationTemplateVM = m_dicNotificationTemplateData.AsEnumerable().ToList()[0];
                if (m_kvpNotificationTemplateVM.Key < 1 || (m_kvpNotificationTemplateVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpNotificationTemplateVM.Key > 1 && !Exact)
                    return Browse(ControlNotificationTemplateID, ControlNotificationTemplateDesc, null, FilterNotificationTemplateID, FilterNotificationTemplateDesc);

                m_dicNotificationTemplateData = GetNotificationTemplateData(false, FilterNotificationTemplateID, FilterNotificationTemplateDesc);
                NotificationTemplateVM m_objNotificationTemplateVM = m_dicNotificationTemplateData[0][0];
                this.GetCmp<TextField>(ControlNotificationTemplateID).Value = m_objNotificationTemplateVM.NotificationTemplateID;
                this.GetCmp<TextField>(ControlNotificationTemplateDesc).Value = m_objNotificationTemplateVM.NotificationTemplateDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<NotificationMapVM> GetListNoticationMap(string NotificationTemplateID)
        {
            NotificationMapVM m_objNotificationMapVM = new NotificationMapVM();
            DNotificationMapDA m_objDNotificationMapDA = new DNotificationMapDA();
            m_objDNotificationMapDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(NotificationMapVM.Prop.NotifMapID.MapAlias);
            m_lstSelect.Add(NotificationMapVM.Prop.FunctionID.MapAlias);
            m_lstSelect.Add(NotificationMapVM.Prop.IsDefault.MapAlias);
            m_lstSelect.Add(NotificationMapVM.Prop.NotificationTemplateID.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            //m_lstFilter.Add(Operator.Equals);
            //m_lstFilter.Add(FunctionID);
            //m_objFilter.Add(NotificationMapVM.Prop.FunctionID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(NotificationTemplateID);
            m_objFilter.Add(NotificationMapVM.Prop.NotificationTemplateID.Map, m_lstFilter);


            //Dictionary<string, OrderDirection> m_objOrderBy = new Dictionary<string, OrderDirection>();
            //m_objOrderBy.Add(NotificationMapVM.Prop.FunctionID.Map, OrderDirection.Ascending);
            List<NotificationMapVM> m_lstNotificationMapVM = new List<NotificationMapVM>();
            Dictionary<int, DataSet> m_dicDNotificationMap = m_objDNotificationMapDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);
            if (m_objDNotificationMapDA.Message == string.Empty)
            {
                m_lstNotificationMapVM = (
                  from DataRow m_drDNotificationMap in m_dicDNotificationMap[0].Tables[0].Rows
                  select new NotificationMapVM
                  {
                      NotifMapID = m_drDNotificationMap[NotificationMapVM.Prop.NotifMapID.Name].ToString(),
                      FunctionID = m_drDNotificationMap[NotificationMapVM.Prop.FunctionID.Name].ToString(),
                      NotificationTemplateID = m_drDNotificationMap[NotificationMapVM.Prop.NotificationTemplateID.Name].ToString(),
                      IsDefault = (bool)m_drDNotificationMap[NotificationMapVM.Prop.IsDefault.Name]
                  }
                ).ToList();
            }
            return m_lstNotificationMapVM;
        }

        private List<string> IsSaveValid(string Action, string NotificationTemplateID, string NotificationTemplateDesc)
        {
            List<string> m_lstReturn = new List<string>();

            if (NotificationTemplateID == string.Empty)
                m_lstReturn.Add(NotificationTemplateVM.Prop.NotificationTemplateID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (NotificationTemplateDesc == string.Empty)
                m_lstReturn.Add(NotificationTemplateVM.Prop.NotificationTemplateDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));

            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(NotificationTemplateVM.Prop.NotificationTemplateID.Name, parameters[NotificationTemplateVM.Prop.NotificationTemplateID.Name]);
            m_dicReturn.Add(NotificationTemplateVM.Prop.NotificationTemplateDesc.Name, parameters[NotificationTemplateVM.Prop.NotificationTemplateDesc.Name]);

            return m_dicReturn;
        }

        private NotificationTemplateVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            NotificationTemplateVM m_objNotificationTemplateVM = new NotificationTemplateVM();
            MNotificationTemplatesDA m_objMNotificationTemplatesDA = new MNotificationTemplatesDA();
            m_objMNotificationTemplatesDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(NotificationTemplateVM.Prop.NotificationTemplateID.MapAlias);
            m_lstSelect.Add(NotificationTemplateVM.Prop.NotificationTemplateDesc.MapAlias);
            m_lstSelect.Add(NotificationTemplateVM.Prop.Contents.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objNotificationTemplateVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(NotificationTemplateVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMNotificationTemplatesDA = m_objMNotificationTemplatesDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMNotificationTemplatesDA.Message == string.Empty)
            {
                DataRow m_drMNotificationTemplatesDA = m_dicMNotificationTemplatesDA[0].Tables[0].Rows[0];
                m_objNotificationTemplateVM.NotificationTemplateID = m_drMNotificationTemplatesDA[NotificationTemplateVM.Prop.NotificationTemplateID.Name].ToString();
                m_objNotificationTemplateVM.NotificationTemplateDesc = m_drMNotificationTemplatesDA[NotificationTemplateVM.Prop.NotificationTemplateDesc.Name].ToString();
                m_objNotificationTemplateVM.Contents = m_drMNotificationTemplatesDA[NotificationTemplateVM.Prop.Contents.Name].ToString();
                m_objNotificationTemplateVM.Tags = GetTags(m_objNotificationTemplateVM.NotificationTemplateID);
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMNotificationTemplatesDA.Message;

            return m_objNotificationTemplateVM;
        }

        private List<string> GetTags(string NotificationTemplateID)
        {
            List<FieldTagReferenceVM> m_lstFieldTagReferenceVM = new List<FieldTagReferenceVM>();
            DTemplateTagsDA m_objDTemplateTagsDA = new DTemplateTagsDA();
            m_objDTemplateTagsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(TemplateTagsVM.Prop.FieldTagID.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(NotificationTemplateID);
            m_objFilter.Add(TemplateTagsVM.Prop.TemplateID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("NOT");
            m_objFilter.Add(TemplateTagsVM.Prop.TemplateType.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicDTemplateTagsDA = m_objDTemplateTagsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDTemplateTagsDA.Message == string.Empty)
            {
                m_lstFieldTagReferenceVM = (
                  from DataRow m_drMNotificationTemplatesDA in m_dicDTemplateTagsDA[0].Tables[0].Rows
                  select new FieldTagReferenceVM
                  {
                      FieldTagID = m_drMNotificationTemplatesDA[TemplateTagsVM.Prop.FieldTagID.Name].ToString()
                  }
                ).ToList();
            }

            return m_lstFieldTagReferenceVM.Select(d => d.FieldTagID).ToList();
        }

        #endregion

        #region Public Method

        public Dictionary<int, List<NotificationTemplateVM>> GetNotificationTemplateData(bool isCount, string NotificationTemplateID, string NotificationTemplateDesc)
        {
            int m_intCount = 0;
            List<NotificationTemplateVM> m_lstNotificationTemplateVM = new List<ViewModels.NotificationTemplateVM>();
            Dictionary<int, List<NotificationTemplateVM>> m_dicReturn = new Dictionary<int, List<NotificationTemplateVM>>();
            MNotificationTemplatesDA m_objMNotificationTemplatesDA = new MNotificationTemplatesDA();
            m_objMNotificationTemplatesDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(NotificationTemplateVM.Prop.NotificationTemplateID.MapAlias);
            m_lstSelect.Add(NotificationTemplateVM.Prop.NotificationTemplateDesc.MapAlias);
            m_lstSelect.Add(NotificationTemplateVM.Prop.Contents.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(NotificationTemplateID);
            m_objFilter.Add(NotificationTemplateVM.Prop.NotificationTemplateID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(NotificationTemplateDesc);
            m_objFilter.Add(NotificationTemplateVM.Prop.NotificationTemplateDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMNotificationTemplatesDA = m_objMNotificationTemplatesDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMNotificationTemplatesDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpNotificationTemplateBL in m_dicMNotificationTemplatesDA)
                    {
                        m_intCount = m_kvpNotificationTemplateBL.Key;
                        break;
                    }
                else
                {
                    m_lstNotificationTemplateVM = (
                        from DataRow m_drMNotificationTemplatesDA in m_dicMNotificationTemplatesDA[0].Tables[0].Rows
                        select new NotificationTemplateVM()
                        {
                            NotificationTemplateID = m_drMNotificationTemplatesDA[NotificationTemplateVM.Prop.NotificationTemplateID.Name].ToString(),
                            NotificationTemplateDesc = m_drMNotificationTemplatesDA[NotificationTemplateVM.Prop.NotificationTemplateDesc.Name].ToString(),
                            Contents = m_drMNotificationTemplatesDA[NotificationTemplateVM.Prop.Contents.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstNotificationTemplateVM);
            return m_dicReturn;
        }

        #endregion
    }
}