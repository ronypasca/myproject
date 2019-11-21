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
    public class PreBuiltRecipientController : BaseController
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
            MPreBuildRecipientTemplatesDA m_objMPreBuildRecipientTemplatesDA = new MPreBuildRecipientTemplatesDA();
            m_objMPreBuildRecipientTemplatesDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMPreBuildRecipientTemplates = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMPreBuildRecipientTemplates.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = PreBuiltRecipientTemplateVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMPreBuildRecipientTemplatesDA = m_objMPreBuildRecipientTemplatesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpNotificationTemplateBL in m_dicMPreBuildRecipientTemplatesDA)
            {
                m_intCount = m_kvpNotificationTemplateBL.Key;
                break;
            }

            List<PreBuiltRecipientTemplateVM> m_lstPreBuiltRecipientTemplateVM = new List<PreBuiltRecipientTemplateVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(PreBuiltRecipientTemplateVM.Prop.PreBuildRecTemplateID.MapAlias);
                m_lstSelect.Add(PreBuiltRecipientTemplateVM.Prop.PreBuildDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(PreBuiltRecipientTemplateVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMPreBuildRecipientTemplatesDA = m_objMPreBuildRecipientTemplatesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMPreBuildRecipientTemplatesDA.Message == string.Empty)
                {
                    m_lstPreBuiltRecipientTemplateVM = (
                        from DataRow m_drMPreBuildRecipientTemplatesDA in m_dicMPreBuildRecipientTemplatesDA[0].Tables[0].Rows
                        select new PreBuiltRecipientTemplateVM()
                        {
                            PreBuildRecTemplateID = m_drMPreBuildRecipientTemplatesDA[PreBuiltRecipientTemplateVM.Prop.PreBuildRecTemplateID.Name].ToString(),
                            PreBuildDesc = m_drMPreBuildRecipientTemplatesDA[PreBuiltRecipientTemplateVM.Prop.PreBuildDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstPreBuiltRecipientTemplateVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters, string FunctionID="")
        {
            MPreBuildRecipientTemplatesDA m_objMPreBuildRecipientTemplatesDA = new MPreBuildRecipientTemplatesDA();
            m_objMPreBuildRecipientTemplatesDA.ConnectionStringName = Global.ConnStrConfigName;

            DNotificationMapDA m_objMNotificationMapDA = new DNotificationMapDA();
            m_objMNotificationMapDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMPreBuildRecipientTemplates = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMPreBuildRecipientTemplates.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = PreBuiltRecipientTemplateVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMPreBuildRecipientTemplatesDA = m_objMPreBuildRecipientTemplatesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpNotificationTemplateBL in m_dicMPreBuildRecipientTemplatesDA)
            {
                m_intCount = m_kvpNotificationTemplateBL.Key;
                break;
            }

            List<PreBuiltRecipientTemplateVM> m_lstPreBuiltRecipientTemplateVM = new List<PreBuiltRecipientTemplateVM>();
            if (m_intCount > 0)
            {
                string message = "";
                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(FunctionID);
                if(FunctionID.Length>0)
                    m_objFilter.Add(FunctionsVM.Prop.FunctionID.Map,m_lstFilter);

                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(PreBuiltRecipientTemplateVM.Prop.PreBuildRecTemplateID.MapAlias);
                m_lstSelect.Add(PreBuiltRecipientTemplateVM.Prop.PreBuildDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(PreBuiltRecipientTemplateVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                if (FunctionID.Length > 0)
                {

                    m_lstSelect.Add(FunctionsVM.Prop.FunctionID.MapAlias);
                    List<string> m_lstGroup = new List<string>();
                    m_lstGroup.Add(PreBuiltRecipientTemplateVM.Prop.PreBuildRecTemplateID.Map);
                    m_lstGroup.Add(PreBuiltRecipientTemplateVM.Prop.PreBuildDesc.Map);
                    
                    m_lstGroup.Add(FunctionsVM.Prop.FunctionID.Map);

                    m_dicMPreBuildRecipientTemplatesDA = m_objMNotificationMapDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, m_lstGroup, m_dicOrder, null);
                    if (m_objMNotificationMapDA.Message == string.Empty)
                    {
                        m_lstPreBuiltRecipientTemplateVM = (
                            from DataRow m_drMPreBuildRecipientTemplatesDA in m_dicMPreBuildRecipientTemplatesDA[0].Tables[0].Rows
                            select new PreBuiltRecipientTemplateVM()
                            {
                                PreBuildRecTemplateID = m_drMPreBuildRecipientTemplatesDA[PreBuiltRecipientTemplateVM.Prop.PreBuildRecTemplateID.Name].ToString(),
                                PreBuildDesc = m_drMPreBuildRecipientTemplatesDA[PreBuiltRecipientTemplateVM.Prop.PreBuildDesc.Name].ToString(),
                                LstRecipient = GetListRecipientPreBuilt(m_drMPreBuildRecipientTemplatesDA[PreBuiltRecipientTemplateVM.Prop.PreBuildRecTemplateID.Name].ToString(), ref message)
                           }
                        ).ToList();
                    }
                }
                else
                {
                    m_dicMPreBuildRecipientTemplatesDA = m_objMPreBuildRecipientTemplatesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                    if (m_objMPreBuildRecipientTemplatesDA.Message == string.Empty)
                    {
                        m_lstPreBuiltRecipientTemplateVM = (
                            from DataRow m_drMPreBuildRecipientTemplatesDA in m_dicMPreBuildRecipientTemplatesDA[0].Tables[0].Rows
                            select new PreBuiltRecipientTemplateVM()
                            {
                                PreBuildRecTemplateID = m_drMPreBuildRecipientTemplatesDA[PreBuiltRecipientTemplateVM.Prop.PreBuildRecTemplateID.Name].ToString(),
                                PreBuildDesc = m_drMPreBuildRecipientTemplatesDA[PreBuiltRecipientTemplateVM.Prop.PreBuildDesc.Name].ToString(),
                                LstRecipient = GetListRecipientPreBuilt(m_drMPreBuildRecipientTemplatesDA[PreBuiltRecipientTemplateVM.Prop.PreBuildRecTemplateID.Name].ToString(), ref message)
                            }
                        ).ToList();
                    }
                }
            }
            return this.Store(m_lstPreBuiltRecipientTemplateVM, m_intCount);
        }

        public ActionResult ReadFunction(StoreRequestParameters parameters, string PreBuildRecTemplateID)
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
            //m_lstFilter.Add(PreBuildRecTemplateID);
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
                if (m_objDFPTFunctionsDA.Message == string.Empty)
                {
                    m_lstFunctionsVM = (
                        from DataRow m_drDFPTFunctionsDA in m_dicDFPTFunctionsDA[0].Tables[0].Rows
                        select new FunctionsVM()
                        {
                            FunctionID = m_drDFPTFunctionsDA[FunctionsVM.Prop.FunctionID.Name].ToString(),
                            FunctionDesc = m_drDFPTFunctionsDA[FunctionsVM.Prop.FunctionDesc.Name].ToString(),
                            Checked = IsChecked(m_drDFPTFunctionsDA[FunctionsVM.Prop.FunctionID.Name].ToString(), PreBuildRecTemplateID)
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

            //m_lstFilter = new List<object>();
            //m_lstFilter.Add(Operator.Equals);
            //m_lstFilter.Add(PreBuildRecTemplateID);
            //m_objFilter.Add(FunctionsVM.Prop.FunctionID.Map, m_lstFilter);

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

        public ActionResult Add(string Caller)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Add)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            PreBuiltRecipientTemplateVM m_objPreBuiltRecipientTemplateVM = new PreBuiltRecipientTemplateVM();
            m_objPreBuiltRecipientTemplateVM.LstRecipient = new List<PreBuiltRecipientVM>();

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
                Model = m_objPreBuiltRecipientTemplateVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddNotificationTemplate,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        
        public ActionResult Detail(string Caller, string Selected)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            string m_strMessage = string.Empty;
            PreBuiltRecipientTemplateVM m_objPreBuiltRecipientTemplateVM = new PreBuiltRecipientTemplateVM();
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
                m_objPreBuiltRecipientTemplateVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddNotificationTemplate = new ViewDataDictionary();
            m_vddNotificationTemplate.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objPreBuiltRecipientTemplateVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddNotificationTemplate,
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
            PreBuiltRecipientTemplateVM m_objPreBuiltRecipientTemplateVM = new PreBuiltRecipientTemplateVM();
            if (m_dicSelectedRow.Count > 0)
                m_objPreBuiltRecipientTemplateVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
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
                Model = m_objPreBuiltRecipientTemplateVM,
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

            List<PreBuiltRecipientTemplateVM> m_lstSelectedRow = new List<PreBuiltRecipientTemplateVM>();
            m_lstSelectedRow = JSON.Deserialize<List<PreBuiltRecipientTemplateVM>>(Selected);

            MPreBuildRecipientTemplatesDA m_objMPreBuildRecipientTemplatesDA = new MPreBuildRecipientTemplatesDA();
            m_objMPreBuildRecipientTemplatesDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (PreBuiltRecipientTemplateVM m_objPreBuiltRecipientTemplateVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifPreBuiltRecipientTemplateVM = m_objPreBuiltRecipientTemplateVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifPreBuiltRecipientTemplateVM in m_arrPifPreBuiltRecipientTemplateVM)
                    {
                        string m_strFieldName = m_pifPreBuiltRecipientTemplateVM.Name;
                        object m_objFieldValue = m_pifPreBuiltRecipientTemplateVM.GetValue(m_objPreBuiltRecipientTemplateVM);
                        if (m_objPreBuiltRecipientTemplateVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(PreBuiltRecipientTemplateVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objMPreBuildRecipientTemplatesDA.DeleteBC(m_objFilter, false);
                    if (m_objMPreBuildRecipientTemplatesDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMPreBuildRecipientTemplatesDA.Message);
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

        public ActionResult Browse(string ControlPreBuildRecTemplateID, string ControlPreBuildDesc,string GrdScheduleRecipient="",string FilterTemplateFunctionID="", string FilterPreBuildRecTemplateID = "", string FilterPreBuildDesc = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddNotificationTemplate = new ViewDataDictionary();
            m_vddNotificationTemplate.Add("Control" + PreBuiltRecipientTemplateVM.Prop.PreBuildRecTemplateID.Name, ControlPreBuildRecTemplateID);
            m_vddNotificationTemplate.Add("Control" + PreBuiltRecipientTemplateVM.Prop.PreBuildDesc.Name, ControlPreBuildDesc);
            m_vddNotificationTemplate.Add(PreBuiltRecipientTemplateVM.Prop.PreBuildRecTemplateID.Name, FilterPreBuildRecTemplateID);
            m_vddNotificationTemplate.Add(PreBuiltRecipientTemplateVM.Prop.PreBuildDesc.Name, FilterPreBuildDesc);
            m_vddNotificationTemplate.Add(FunctionsVM.Prop.FunctionID.Name, FilterTemplateFunctionID);
            if (GrdScheduleRecipient != null)
                m_vddNotificationTemplate.Add("ControlGrdRecipientList", GrdScheduleRecipient);
            else
                m_vddNotificationTemplate.Add("ControlGrdRecipientList", "");

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddNotificationTemplate,
                ViewName = "../PreBuiltRecipient/_Browse"
                
            };
        }
        
        public ActionResult Save(string Action)
        {
            //action = JSON.Deserialize<string>(action);
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MPreBuildRecipientTemplatesDA m_objMPreBuildRecipientTemplatesDA = new MPreBuildRecipientTemplatesDA();
            m_objMPreBuildRecipientTemplatesDA.ConnectionStringName = Global.ConnStrConfigName;

            try
            {
                string m_strPreBuildRecTemplateID = this.Request.Params[PreBuiltRecipientTemplateVM.Prop.PreBuildRecTemplateID.Name];
                string m_strPreBuildDesc = this.Request.Params[PreBuiltRecipientTemplateVM.Prop.PreBuildDesc.Name];
                string m_strFunctionID = this.Request.Params[PreBuiltRecipientTemplateVM.Prop.FunctionID.Name];
                string m_lstRecipient = this.Request.Params[PreBuiltRecipientTemplateVM.Prop.ListPreBuiltRecipient.Name];
                string m_isPIC = this.Request.Params[PreBuiltRecipientTemplateVM.Prop.IsPIC.Name];

                List<PreBuiltRecipientVM> m_lstPreBuiltRecipientVM = JSON.Deserialize<List<PreBuiltRecipientVM>>(m_lstRecipient);


                m_lstMessage = IsSaveValid(Action, m_strPreBuildRecTemplateID, m_strPreBuildDesc);
                if (m_lstMessage.Count <= 0)
                {
                    MPreBuildRecipientTemplates m_objMPreBuildRecipientTemplates = new MPreBuildRecipientTemplates();
                    m_objMPreBuildRecipientTemplates.PreBuildRecTemplateID = m_strPreBuildRecTemplateID;
                    m_objMPreBuildRecipientTemplatesDA.Data = m_objMPreBuildRecipientTemplates;

                    string m_strTransName = "PreBuiltRecipient";
                    object m_objDBConnection = m_objMPreBuildRecipientTemplatesDA.BeginTrans(m_strTransName);

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objMPreBuildRecipientTemplatesDA.Select();

                    m_objMPreBuildRecipientTemplates.PreBuildDesc = m_strPreBuildDesc;
                    m_objMPreBuildRecipientTemplates.FunctionID = m_strFunctionID;
                    m_objMPreBuildRecipientTemplates.IsPIC = Convert.ToBoolean(m_isPIC);

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objMPreBuildRecipientTemplatesDA.Insert(true, m_objDBConnection);
                    else
                        m_objMPreBuildRecipientTemplatesDA.Update(true, m_objDBConnection);

                    if (!m_objMPreBuildRecipientTemplatesDA.Success || m_objMPreBuildRecipientTemplatesDA.Message != string.Empty)
                    {
                        m_objMPreBuildRecipientTemplatesDA.EndTrans(ref m_objDBConnection, m_strTransName);
                        return this.Direct(false, m_objMPreBuildRecipientTemplatesDA.Message);
                    }



                    #region DPreBuiltRecipients
                    if (m_lstPreBuiltRecipientVM.Any())
                    {
                        DPreBuildRecipientsDA m_objDPreBuildRecipientsDA = new DPreBuildRecipientsDA();
                        m_objDPreBuildRecipientsDA.ConnectionStringName = Global.ConnStrConfigName;

                        if (Action == General.EnumDesc(Buttons.ButtonUpdate))
                        {
                            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_strPreBuildRecTemplateID);
                            m_objFilter.Add(PreBuiltRecipientVM.Prop.PreBuildRecTemplateID.Map, m_lstFilter);


                            m_objDPreBuildRecipientsDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                        }

                        foreach (PreBuiltRecipientVM objPreBuiltRecipientVM in m_lstPreBuiltRecipientVM)
                        {
                            DPreBuildRecipients m_objDPreBuildRecipients = new DPreBuildRecipients();
                            m_objDPreBuildRecipients.PreBuildRecID = Guid.NewGuid().ToString().Replace("-", "");
                            m_objDPreBuildRecipients.PreBuildRecTemplateID = m_strPreBuildRecTemplateID;
                            m_objDPreBuildRecipients.EmployeeID = objPreBuiltRecipientVM.EmployeeID;
                            m_objDPreBuildRecipients.RecipientTypeID = ((int)System.Enum.Parse(typeof(RecipientTypes), objPreBuiltRecipientVM.RecipientTypeDesc)).ToString();

                            m_objDPreBuildRecipientsDA.Data = m_objDPreBuildRecipients;
                            m_objDPreBuildRecipientsDA.Select();

                            if (m_objDPreBuildRecipientsDA.Message != string.Empty)
                                m_objDPreBuildRecipientsDA.Insert(true, m_objDBConnection);

                            if (!m_objDPreBuildRecipientsDA.Success || m_objDPreBuildRecipientsDA.Message != string.Empty)
                            {
                                m_objDPreBuildRecipientsDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                                return this.Direct(false, m_objDPreBuildRecipientsDA.Message);
                            }
                        }

                    }
                    #endregion

                    if (!m_objMPreBuildRecipientTemplatesDA.Success || m_objMPreBuildRecipientTemplatesDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMPreBuildRecipientTemplatesDA.Message);
                    else
                        m_objMPreBuildRecipientTemplatesDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
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

        public ActionResult GetNotificationTemplate(string ControlPreBuildRecTemplateID, string ControlPreBuildDesc, string FilterPreBuildRecTemplateID, string FilterPreBuildDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<PreBuiltRecipientTemplateVM>> m_dicNotificationTemplateData = GetNotificationTemplateData(true, FilterPreBuildRecTemplateID, FilterPreBuildDesc);
                KeyValuePair<int, List<PreBuiltRecipientTemplateVM>> m_kvpPreBuiltRecipientTemplateVM = m_dicNotificationTemplateData.AsEnumerable().ToList()[0];
                if (m_kvpPreBuiltRecipientTemplateVM.Key < 1 || (m_kvpPreBuiltRecipientTemplateVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpPreBuiltRecipientTemplateVM.Key > 1 && !Exact)
                    return Browse(ControlPreBuildRecTemplateID, ControlPreBuildDesc,null,null, FilterPreBuildRecTemplateID, FilterPreBuildDesc);

                m_dicNotificationTemplateData = GetNotificationTemplateData(false, FilterPreBuildRecTemplateID, FilterPreBuildDesc);
                PreBuiltRecipientTemplateVM m_objPreBuiltRecipientTemplateVM = m_dicNotificationTemplateData[0][0];
                this.GetCmp<TextField>(ControlPreBuildRecTemplateID).Value = m_objPreBuiltRecipientTemplateVM.PreBuildRecTemplateID;
                this.GetCmp<TextField>(ControlPreBuildDesc).Value = m_objPreBuiltRecipientTemplateVM.PreBuildDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method
        private bool IsChecked(string FunctionID, string PreBuildRecTemplateID)
        {
            /*NotificationMapVM m_objNotificationMapVM = new NotificationMapVM();
            DNotificationMapDA m_objDNotificationMapDA = new DNotificationMapDA();
            m_objDNotificationMapDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(NotificationMapVM.Prop.NotifMapID.MapAlias);


            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(FunctionID);
            m_objFilter.Add(NotificationMapVM.Prop.FunctionID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(PreBuildRecTemplateID);
            m_objFilter.Add(NotificationMapVM.Prop.pre.Map, m_lstFilter);
            

            //Dictionary<string, OrderDirection> m_objOrderBy = new Dictionary<string, OrderDirection>();
            //m_objOrderBy.Add(NotificationMapVM.Prop.FunctionID.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicDNotificationMap = m_objDNotificationMapDA.SelectBC(0, 1, true, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpMenuBL in m_dicDNotificationMap)
            {
                m_intCount = m_kvpMenuBL.Key;
                break;
            }*/
            return true;
        }

        private List<string> IsSaveValid(string Action, string PreBuildRecTemplateID, string PreBuildDesc)
        {
            List<string> m_lstReturn = new List<string>();

            if (PreBuildRecTemplateID == string.Empty)
                m_lstReturn.Add(PreBuiltRecipientTemplateVM.Prop.PreBuildRecTemplateID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (PreBuildDesc == string.Empty)
                m_lstReturn.Add(PreBuiltRecipientTemplateVM.Prop.PreBuildDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));

            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(PreBuiltRecipientTemplateVM.Prop.PreBuildRecTemplateID.Name, parameters[PreBuiltRecipientTemplateVM.Prop.PreBuildRecTemplateID.Name]);
            m_dicReturn.Add(PreBuiltRecipientTemplateVM.Prop.PreBuildDesc.Name, parameters[PreBuiltRecipientTemplateVM.Prop.PreBuildDesc.Name]);

            return m_dicReturn;
        }

        private PreBuiltRecipientTemplateVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            PreBuiltRecipientTemplateVM m_objPreBuiltRecipientTemplateVM = new PreBuiltRecipientTemplateVM();
            MPreBuildRecipientTemplatesDA m_objMPreBuildRecipientTemplatesDA = new MPreBuildRecipientTemplatesDA();
            m_objMPreBuildRecipientTemplatesDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(PreBuiltRecipientTemplateVM.Prop.PreBuildRecTemplateID.MapAlias);
            m_lstSelect.Add(PreBuiltRecipientTemplateVM.Prop.PreBuildDesc.MapAlias);
            m_lstSelect.Add(FunctionsVM.Prop.FunctionDesc.MapAlias);
            m_lstSelect.Add(PreBuiltRecipientTemplateVM.Prop.FunctionID.MapAlias);
            m_lstSelect.Add(PreBuiltRecipientTemplateVM.Prop.IsPIC.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objPreBuiltRecipientTemplateVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(PreBuiltRecipientTemplateVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMPreBuildRecipientTemplatesDA = m_objMPreBuildRecipientTemplatesDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMPreBuildRecipientTemplatesDA.Message == string.Empty)
            {
                DataRow m_drMPreBuildRecipientTemplatesDA = m_dicMPreBuildRecipientTemplatesDA[0].Tables[0].Rows[0];
                m_objPreBuiltRecipientTemplateVM.PreBuildRecTemplateID = m_drMPreBuildRecipientTemplatesDA[PreBuiltRecipientTemplateVM.Prop.PreBuildRecTemplateID.Name].ToString();
                m_objPreBuiltRecipientTemplateVM.PreBuildDesc = m_drMPreBuildRecipientTemplatesDA[PreBuiltRecipientTemplateVM.Prop.PreBuildDesc.Name].ToString();
                m_objPreBuiltRecipientTemplateVM.FunctionDesc = m_drMPreBuildRecipientTemplatesDA[PreBuiltRecipientTemplateVM.Prop.FunctionDesc.Name].ToString();
                m_objPreBuiltRecipientTemplateVM.FunctionID = m_drMPreBuildRecipientTemplatesDA[PreBuiltRecipientTemplateVM.Prop.FunctionID.Name].ToString();
                m_objPreBuiltRecipientTemplateVM.IsPIC =Convert.ToBoolean(m_drMPreBuildRecipientTemplatesDA[PreBuiltRecipientTemplateVM.Prop.IsPIC.Name].ToString());
                m_objPreBuiltRecipientTemplateVM.LstRecipient = GetListRecipientPreBuilt(m_objPreBuiltRecipientTemplateVM.PreBuildRecTemplateID,ref message);
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMPreBuildRecipientTemplatesDA.Message;

            return m_objPreBuiltRecipientTemplateVM;
        }

        private List<PreBuiltRecipientVM> GetListRecipientPreBuilt(string PreBuildRecTemplateID, ref string message)
        {
            List<PreBuiltRecipientVM> m_objPreBuiltRecipientTemplateVM = new List<PreBuiltRecipientVM>();
            DPreBuildRecipientsDA m_objMPreBuildRecipientTemplatesDA = new DPreBuildRecipientsDA();
            m_objMPreBuildRecipientTemplatesDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(PreBuiltRecipientVM.Prop.PreBuildRecID.MapAlias);
            m_lstSelect.Add(PreBuiltRecipientVM.Prop.PreBuildRecTemplateID.MapAlias);
            m_lstSelect.Add(PreBuiltRecipientVM.Prop.EmployeeID.MapAlias);
            m_lstSelect.Add(PreBuiltRecipientVM.Prop.EmployeeName.MapAlias);
            m_lstSelect.Add(PreBuiltRecipientVM.Prop.MailAddress.MapAlias);
            m_lstSelect.Add(PreBuiltRecipientVM.Prop.UserID.MapAlias);
            m_lstSelect.Add(PreBuiltRecipientVM.Prop.RecipientTypeID.MapAlias);
            m_lstSelect.Add(PreBuiltRecipientVM.Prop.RecipientTypeDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            m_lstKey.Add("");
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(PreBuildRecTemplateID);
            m_objFilter.Add(PreBuiltRecipientVM.Prop.PreBuildRecTemplateID.Map, m_lstFilter);
                

            Dictionary<int, DataSet> m_dicMPreBuildRecipientTemplatesDA = m_objMPreBuildRecipientTemplatesDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMPreBuildRecipientTemplatesDA.Message == string.Empty)
            {
                foreach (DataRow m_drMPreBuildRecipientTemplatesDA in m_dicMPreBuildRecipientTemplatesDA[0].Tables[0].Rows) {
                    PreBuiltRecipientVM obj = new PreBuiltRecipientVM();                    
                    obj.PreBuildRecTemplateID = m_drMPreBuildRecipientTemplatesDA[PreBuiltRecipientVM.Prop.PreBuildRecTemplateID.Name].ToString();
                    obj.PreBuildRecID = m_drMPreBuildRecipientTemplatesDA[PreBuiltRecipientVM.Prop.PreBuildRecID.Name].ToString();
                    obj.EmployeeName = m_drMPreBuildRecipientTemplatesDA[PreBuiltRecipientVM.Prop.EmployeeName.Name].ToString();
                    obj.EmployeeID = m_drMPreBuildRecipientTemplatesDA[PreBuiltRecipientVM.Prop.EmployeeID.Name].ToString();
                    obj.MailAddress = m_drMPreBuildRecipientTemplatesDA[PreBuiltRecipientVM.Prop.MailAddress.Name].ToString();
                    obj.UserID = m_drMPreBuildRecipientTemplatesDA[PreBuiltRecipientVM.Prop.UserID.Name].ToString();
                    obj.RecipientTypeID = m_drMPreBuildRecipientTemplatesDA[PreBuiltRecipientVM.Prop.RecipientTypeID.Name].ToString();
                    obj.RecipientTypeDesc = m_drMPreBuildRecipientTemplatesDA[PreBuiltRecipientVM.Prop.RecipientTypeDesc.Name].ToString();
                    m_objPreBuiltRecipientTemplateVM.Add(obj);
                }
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMPreBuildRecipientTemplatesDA.Message;

            return m_objPreBuiltRecipientTemplateVM;
        }
        private List<string> GetTags(string PreBuildRecTemplateID)
        {
            List<FieldTagReferenceVM> m_lstFieldTagReferenceVM = new List<FieldTagReferenceVM>();
            DTemplateTagsDA m_objDTemplateTagsDA = new DTemplateTagsDA();
            m_objDTemplateTagsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(TemplateTagsVM.Prop.FieldTagID.MapAlias);
            
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(PreBuildRecTemplateID);
            m_objFilter.Add(TemplateTagsVM.Prop.TemplateID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("NOT");
            m_objFilter.Add(TemplateTagsVM.Prop.TemplateType.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicDTemplateTagsDA = m_objDTemplateTagsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDTemplateTagsDA.Message == string.Empty)
            {
                m_lstFieldTagReferenceVM = (
                  from DataRow m_drMPreBuildRecipientTemplatesDA in m_dicDTemplateTagsDA[0].Tables[0].Rows
                  select new FieldTagReferenceVM
                  {
                      FieldTagID = m_drMPreBuildRecipientTemplatesDA[TemplateTagsVM.Prop.FieldTagID.Name].ToString()
                  }
                ).ToList();
            }
           
            return m_lstFieldTagReferenceVM.Select(d=>d.FieldTagID).ToList();
        }

        #endregion

        #region Public Method

        public Dictionary<int, List<PreBuiltRecipientTemplateVM>> GetNotificationTemplateData(bool isCount, string PreBuildRecTemplateID, string PreBuildDesc)
        {
            int m_intCount = 0;
            List<PreBuiltRecipientTemplateVM> m_lstPreBuiltRecipientTemplateVM = new List<ViewModels.PreBuiltRecipientTemplateVM>();
            Dictionary<int, List<PreBuiltRecipientTemplateVM>> m_dicReturn = new Dictionary<int, List<PreBuiltRecipientTemplateVM>>();
            MPreBuildRecipientTemplatesDA m_objMPreBuildRecipientTemplatesDA = new MPreBuildRecipientTemplatesDA();
            m_objMPreBuildRecipientTemplatesDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(PreBuiltRecipientTemplateVM.Prop.PreBuildRecTemplateID.MapAlias);
            m_lstSelect.Add(PreBuiltRecipientTemplateVM.Prop.PreBuildDesc.MapAlias);
            //m_lstSelect.Add(PreBuiltRecipientTemplateVM.Prop.Contents.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(PreBuildRecTemplateID);
            m_objFilter.Add(PreBuiltRecipientTemplateVM.Prop.PreBuildRecTemplateID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(PreBuildDesc);
            m_objFilter.Add(PreBuiltRecipientTemplateVM.Prop.PreBuildDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMPreBuildRecipientTemplatesDA = m_objMPreBuildRecipientTemplatesDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMPreBuildRecipientTemplatesDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpNotificationTemplateBL in m_dicMPreBuildRecipientTemplatesDA)
                    {
                        m_intCount = m_kvpNotificationTemplateBL.Key;
                        break;
                    }
                else
                {
                    m_lstPreBuiltRecipientTemplateVM = (
                        from DataRow m_drMPreBuildRecipientTemplatesDA in m_dicMPreBuildRecipientTemplatesDA[0].Tables[0].Rows
                        select new PreBuiltRecipientTemplateVM()
                        {
                            PreBuildRecTemplateID = m_drMPreBuildRecipientTemplatesDA[PreBuiltRecipientTemplateVM.Prop.PreBuildRecTemplateID.Name].ToString(),
                            PreBuildDesc = m_drMPreBuildRecipientTemplatesDA[PreBuiltRecipientTemplateVM.Prop.PreBuildDesc.Name].ToString(),
                            //Contents = m_drMPreBuildRecipientTemplatesDA[PreBuiltRecipientTemplateVM.Prop.Contents.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstPreBuiltRecipientTemplateVM);
            return m_dicReturn;
        }

        #endregion
    }
}