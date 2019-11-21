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

namespace com.SML.BIGTRONS.Controllers
{
    public class MinutesTemplateController : BaseController
    {
        private readonly string title = "Minutes of Meeting Template";
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
            MMinuteTemplatesDA m_objMMinuteTemplatesDA = new MMinuteTemplatesDA();
            m_objMMinuteTemplatesDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;            

            FilterHeaderConditions m_fhcMMinuteTemplates = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMMinuteTemplates.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = MinutesTemplateVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMMinuteTemplatesDA = m_objMMinuteTemplatesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpMMinuteTemplatesDA in m_dicMMinuteTemplatesDA)
            {
                m_intCount = m_kvpMMinuteTemplatesDA.Key;
                break;
            }

            List<MinutesTemplateVM> m_lstMinutesTemplateVM = new List<MinutesTemplateVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(MinutesTemplateVM.Prop.MinuteTemplateID.MapAlias);
                m_lstSelect.Add(MinutesTemplateVM.Prop.MinuteTemplateDescriptions.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(MinutesTemplateVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMMinuteTemplatesDA = m_objMMinuteTemplatesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMMinuteTemplatesDA.Message == string.Empty)
                {
                    m_lstMinutesTemplateVM = (
                        from DataRow m_drMMinuteTemplatesDA in m_dicMMinuteTemplatesDA[0].Tables[0].Rows
                        select new MinutesTemplateVM()
                        {
                            MinuteTemplateID = m_drMMinuteTemplatesDA[MinutesTemplateVM.Prop.MinuteTemplateID.Name].ToString(),
                            MinuteTemplateDescriptions = m_drMMinuteTemplatesDA[MinutesTemplateVM.Prop.MinuteTemplateDescriptions.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstMinutesTemplateVM, m_lstMinutesTemplateVM.Count);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters, string FunctionID = "")
        {
            MMinuteTemplatesDA m_objMMinuteTemplatesDA = new MMinuteTemplatesDA();
            m_objMMinuteTemplatesDA.ConnectionStringName = Global.ConnStrConfigName;

            //DNotificationMapDA m_objMNotificationMapDA = new DNotificationMapDA();
            //m_objMNotificationMapDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMMinuteTemplates = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMMinuteTemplates.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = MinutesTemplateVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMMinuteTemplatesDA = m_objMMinuteTemplatesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpMinutesTemplateBL in m_dicMMinuteTemplatesDA)
            {
                m_intCount = m_kvpMinutesTemplateBL.Key;
                break;
            }

            List<MinutesTemplateVM> m_lstMinutesTemplateVM = new List<MinutesTemplateVM>();
            if (m_intCount > 0)
            {
                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(FunctionID);
                if (FunctionID.Length > 0)
                    m_objFilter.Add(FunctionsVM.Prop.FunctionID.Map, m_lstFilter);

                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(MinutesTemplateVM.Prop.MinuteTemplateID.MapAlias);
                m_lstSelect.Add(MinutesTemplateVM.Prop.MinuteTemplateDescriptions.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(MinutesTemplateVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));
                
                m_dicMMinuteTemplatesDA = m_objMMinuteTemplatesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMMinuteTemplatesDA.Message == string.Empty)
                {
                    m_lstMinutesTemplateVM = (
                        from DataRow m_drMMinuteTemplatesDA in m_dicMMinuteTemplatesDA[0].Tables[0].Rows
                        select new MinutesTemplateVM()
                        {
                            MinuteTemplateID = m_drMMinuteTemplatesDA[MinutesTemplateVM.Prop.MinuteTemplateID.Name].ToString(),
                            MinuteTemplateDescriptions = m_drMMinuteTemplatesDA[MinutesTemplateVM.Prop.MinuteTemplateDescriptions.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstMinutesTemplateVM, m_intCount);
        }

        public ActionResult ReadFunction(StoreRequestParameters parameters, string MinuteTemplateID)
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
            //m_lstFilter.Add(MinuteTemplateID);
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
                //List<MinutesTemplateVM> m_lstMinutesTemplateVM = GetListNoticationMap(MinuteTemplateID);
                if (m_objDFPTFunctionsDA.Message == string.Empty)
                {
                    m_lstFunctionsVM = (
                        from DataRow m_drDFPTFunctionsDA in m_dicDFPTFunctionsDA[0].Tables[0].Rows
                        //join lstnotifmap in m_lstMinutesTemplateVM on m_drDFPTFunctionsDA[FunctionsVM.Prop.FunctionID.Name].ToString() equals lstnotifmap.FunctionID into v
                        //from lstnotifmap in v.DefaultIfEmpty()
                        select new FunctionsVM()
                        {
                            FunctionID = m_drDFPTFunctionsDA[FunctionsVM.Prop.FunctionID.Name].ToString(),
                            FunctionDesc = m_drDFPTFunctionsDA[FunctionsVM.Prop.FunctionDesc.Name].ToString(),
                            //Checked = lstnotifmap==null? false: true//,
                            //IsDefault = lstnotifmap==null? false: lstnotifmap.IsDefault

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
            //m_lstFilter.Add(MinuteTemplateID);
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
                m_lstSelect.Add(FieldTagReferenceVM.Prop.TagDesc.MapAlias);

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
                            TagDesc = m_drDFPTFunctionsDA[FieldTagReferenceVM.Prop.TagDesc.Name].ToString()
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

            MinutesTemplateVM m_objMinutesTemplateVM = new MinutesTemplateVM();
            ViewDataDictionary m_vddMinutesTemplate = new ViewDataDictionary();
            m_vddMinutesTemplate.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddMinutesTemplate.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
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
                Model = m_objMinutesTemplateVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddMinutesTemplate,
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
            MinutesTemplateVM m_objMinutesTemplateVM = new MinutesTemplateVM();
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
                m_objMinutesTemplateVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddMinutesTemplate = new ViewDataDictionary();
            m_vddMinutesTemplate.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objMinutesTemplateVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddMinutesTemplate,
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
            MinutesTemplateVM m_objMinutesTemplateVM = new MinutesTemplateVM();
            if (m_dicSelectedRow.Count > 0)
                m_objMinutesTemplateVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddMinutesTemplate = new ViewDataDictionary();
            m_vddMinutesTemplate.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddMinutesTemplate.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objMinutesTemplateVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddMinutesTemplate,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<MinutesTemplateVM> m_lstSelectedRow = new List<MinutesTemplateVM>();
            m_lstSelectedRow = JSON.Deserialize<List<MinutesTemplateVM>>(Selected);

            MMinuteTemplatesDA m_objMMinuteTemplatesDA = new MMinuteTemplatesDA();
            m_objMMinuteTemplatesDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (MinutesTemplateVM m_objMinutesTemplateVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifMinutesTemplateVM = m_objMinutesTemplateVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifMinutesTemplateVM in m_arrPifMinutesTemplateVM)
                    {
                        string m_strFieldName = m_pifMinutesTemplateVM.Name;
                        object m_objFieldValue = m_pifMinutesTemplateVM.GetValue(m_objMinutesTemplateVM);
                        if (m_objMinutesTemplateVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(MinutesTemplateVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objMMinuteTemplatesDA.DeleteBC(m_objFilter, false);
                    if (m_objMMinuteTemplatesDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMMinuteTemplatesDA.Message);
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

        public ActionResult Browse(string ControlMinuteTemplateID, string ControlMinuteTemplateDescriptions, string FilterTemplateFunctionID, string FilterMinuteTemplateID = "", string FilterMinuteTemplateDescriptions = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddMinutesTemplate = new ViewDataDictionary();
            m_vddMinutesTemplate.Add("Control" + MinutesTemplateVM.Prop.MinuteTemplateID.Name, ControlMinuteTemplateID);
            m_vddMinutesTemplate.Add("Control" + MinutesTemplateVM.Prop.MinuteTemplateDescriptions.Name, ControlMinuteTemplateDescriptions);
            m_vddMinutesTemplate.Add(MinutesTemplateVM.Prop.MinuteTemplateID.Name, FilterMinuteTemplateID);
            m_vddMinutesTemplate.Add(MinutesTemplateVM.Prop.MinuteTemplateDescriptions.Name, FilterMinuteTemplateDescriptions);
            m_vddMinutesTemplate.Add(FunctionsVM.Prop.FunctionID.Name, FilterTemplateFunctionID);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddMinutesTemplate,
                ViewName = "../MinutesTemplate/_Browse"
            };
        }

        [ValidateInput(false)]
        public ActionResult Save(string Action)
        {
            //action = JSON.Deserialize<string>(action);
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MMinuteTemplates m_objMMinuteTemplates = new MMinuteTemplates();
            MMinuteTemplatesDA m_objMMinuteTemplatesDA = new MMinuteTemplatesDA();
            m_objMMinuteTemplatesDA.ConnectionStringName = Global.ConnStrConfigName;

            try
            {
                string m_strMinuteTemplateID = this.Request.Params[MinutesTemplateVM.Prop.MinuteTemplateID.Name];
                string m_strMinuteTemplateDescriptions = this.Request.Params[MinutesTemplateVM.Prop.MinuteTemplateDescriptions.Name];
                string m_strFunctionID = this.Request.Params[MinutesTemplateVM.Prop.FunctionID.Name];
                string m_strContent = this.Request.Params["CKEDITOR"+MinutesTemplateVM.Prop.Contents.Name];

                //string m_lstFunction = this.Request.Params[MinutesTemplateVM.Prop.ListFunctionsVM.Name];

                //List<FunctionsVM> m_lstFunctionVM = JSON.Deserialize<List<FunctionsVM>>(m_lstFunction);

                string m_lstFieldTags = this.Request.Params[MinutesTemplateVM.Prop.ListFieldTagReferenceVM.Name];


                m_lstMessage = IsSaveValid(Action, m_strMinuteTemplateID, m_strMinuteTemplateDescriptions);
                if (m_lstMessage.Count <= 0)
                {
                    m_objMMinuteTemplates.MinuteTemplateID = m_strMinuteTemplateID;
                    m_objMMinuteTemplatesDA.Data = m_objMMinuteTemplates;

                    string m_strTransName = "Template";
                    object m_objDBConnection = m_objMMinuteTemplatesDA.BeginTrans(m_strTransName);

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objMMinuteTemplatesDA.Select();

                    m_objMMinuteTemplates.MinuteTemplateDescriptions = m_strMinuteTemplateDescriptions;
                    m_objMMinuteTemplates.FunctionID = m_strFunctionID;
                    m_objMMinuteTemplates.Contents = m_strContent;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objMMinuteTemplatesDA.Insert(true, m_objDBConnection);
                    else
                        m_objMMinuteTemplatesDA.Update(true, m_objDBConnection);

                    if (!m_objMMinuteTemplatesDA.Success || m_objMMinuteTemplatesDA.Message != string.Empty)
                    {
                        m_objMMinuteTemplatesDA.EndTrans(ref m_objDBConnection, m_strTransName);
                        return this.Direct(false, m_objMMinuteTemplatesDA.Message);
                    }

                    System.Text.RegularExpressions.MatchCollection m_match = System.Text.RegularExpressions.Regex.Matches(m_strContent, @"\[\[(.+?)\]\]");
                    List<string> m_lsTags = new List<string>();
                    foreach (System.Text.RegularExpressions.Match item in m_match)
                    {
                        m_lsTags.Add(item.Groups[1].Value);
                    }

                    #region DTemplateTags

                    m_lstFieldTags = string.Join(",", m_lsTags);

                    if (m_lstFieldTags.Split(',').Length > 0 && !string.IsNullOrEmpty(m_lstFieldTags))
                    {
                        DTemplateTagsDA m_objDTemplateTagsDA = new DTemplateTagsDA();
                        m_objDTemplateTagsDA.ConnectionStringName = Global.ConnStrConfigName;

                        if (Action == General.EnumDesc(Buttons.ButtonUpdate))
                        {
                            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_strMinuteTemplateID);
                            m_objFilter.Add(TemplateTagsVM.Prop.TemplateID.Map, m_lstFilter);

                            m_objDTemplateTagsDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                        }

                        foreach (string m_strFieldTagReferenceVM in m_lstFieldTags.Split(','))
                        {
                            DTemplateTags m_objDTemplateTags = new DTemplateTags();
                            m_objDTemplateTags.TemplateTagID = Guid.NewGuid().ToString().Replace("-", "");
                            m_objDTemplateTags.TemplateID = m_strMinuteTemplateID;
                            m_objDTemplateTags.FieldTagID = m_strFieldTagReferenceVM;
                            m_objDTemplateTags.TemplateType = "MOM";

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

                    //#region DNotificationMap

                    //DNotificationMapDA m_objDNotificationMapDA = new DNotificationMapDA();
                    //m_objDNotificationMapDA.ConnectionStringName = Global.ConnStrConfigName;

                    //if (Action == General.EnumDesc(Buttons.ButtonUpdate))
                    //{
                    //    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    //    List<object> m_lstFilter = new List<object>();
                    //    m_lstFilter.Add(Operator.Equals);
                    //    m_lstFilter.Add(m_strMinuteTemplateID);
                    //    m_objFilter.Add(MinutesTemplateVM.Prop.MinuteTemplateID.Map, m_lstFilter);

                    //    m_objDNotificationMapDA.DeleteBC(m_objFilter, true, m_objDBConnection);

                    //    if (!m_objDNotificationMapDA.Success)
                    //    {
                    //        m_objDNotificationMapDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                    //        return this.Direct(false, m_objDNotificationMapDA.Message);
                    //    }
                    //}

                    //foreach (FunctionsVM objFunctionsVM in m_lstFunctionVM)
                    //{   //UPDATE: if IsDefault is true
                    //    if (objFunctionsVM.IsDefault)
                    //    {
                    //        Dictionary<string, List<object>> m_dicSet = new Dictionary<string, List<object>>();
                    //        Dictionary<string, List<object>> m_dicFilter = new Dictionary<string, List<object>>();
                    //        List<object> m_lstFilter = new List<object>();
                    //        m_lstFilter.Add(Operator.Equals);
                    //        m_lstFilter.Add(objFunctionsVM.FunctionID);
                    //        m_dicFilter.Add(MinutesTemplateVM.Prop.FunctionID.Map, m_lstFilter);

                    //        List<object> m_lstSet = new List<object>();
                    //        m_lstSet.Add(typeof(int));
                    //        m_lstSet.Add(0);
                    //        m_dicSet.Add(MinutesTemplateVM.Prop.IsDefault.Map, m_lstSet);

                    //        m_objDNotificationMapDA.UpdateBC(m_dicSet, m_dicFilter, true, m_objDBConnection);
                    //        if (!m_objDNotificationMapDA.Success)
                    //        {
                    //            m_lstMessage.Add(m_objDNotificationMapDA.Message);
                    //            m_objDNotificationMapDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                    //        }
                    //    }

                    //    DNotificationMap m_objDNotificationMap = new DNotificationMap();
                    //    m_objDNotificationMap.NotifMapID = Guid.NewGuid().ToString().Replace("-", "");

                    //    m_objDNotificationMapDA.Data = m_objDNotificationMap;
                    //    m_objDNotificationMapDA.Select();

                    //    m_objDNotificationMap.MinuteTemplateID = m_strMinuteTemplateID;
                    //    m_objDNotificationMap.FunctionID = objFunctionsVM.FunctionID;
                    //    m_objDNotificationMap.IsDefault = objFunctionsVM.IsDefault;



                    //    if (m_objDNotificationMapDA.Message != string.Empty)
                    //        m_objDNotificationMapDA.Insert(true, m_objDBConnection);

                    //    if (!m_objDNotificationMapDA.Success || m_objDNotificationMapDA.Message != string.Empty)
                    //    {
                    //        m_objDNotificationMapDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                    //        return this.Direct(false, m_objDNotificationMapDA.Message);
                    //    }
                    //}


                    //#endregion



                    if (!m_objMMinuteTemplatesDA.Success || m_objMMinuteTemplatesDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMMinuteTemplatesDA.Message);
                    else
                        m_objMMinuteTemplatesDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
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
                //    /*if (AppDomain.CurrentDomain.IsDefaultAppDomain())
                //    {
                //        // RazorEngine cannot clean up from the default appdomain...
                //        AppDomainSetup adSetup = new AppDomainSetup();
                //        adSetup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                //        var current = AppDomain.CurrentDomain;
                //        // You only need to add strongnames when your appdomain is not a full trust environment.
                //        var strongNames = new System.Security.Policy.StrongName[0];

                //        var domain = AppDomain.CreateDomain(
                //            "MyMainDomain", null,
                //            current.SetupInformation, new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted),
                //            strongNames);
                //        var exitCode = domain.ExecuteAssembly(Assembly.GetExecutingAssembly().Location);
                //        // RazorEngine will cleanup. 
                //        AppDomain.Unload(domain);
                //    }*/

                //    /*var config = new TemplateServiceConfiguration();
                //    config.DisableTempFileLocking = true;
                //    config.CachingProvider = new DefaultCachingProvider(t => { });
                //    Engine.Razor = RazorEngineService.Create(config);

                //    Engine.Razor.AddTemplate(m_objMMinuteTemplates.MinuteTemplateID, m_objMMinuteTemplates.Contents);
                //    Engine.Razor.Compile(m_objMMinuteTemplates.MinuteTemplateID, null);*/
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

        public ActionResult GetMinutesTemplate(string ControlMinuteTemplateID, string ControlMinuteTemplateDescriptions, string FilterMinuteTemplateID, string FilterMinuteTemplateDescriptions, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<MinutesTemplateVM>> m_dicMinutesTemplateData = GetMinutesTemplateData(true, FilterMinuteTemplateID, FilterMinuteTemplateDescriptions);
                KeyValuePair<int, List<MinutesTemplateVM>> m_kvpMinutesTemplateVM = m_dicMinutesTemplateData.AsEnumerable().ToList()[0];
                if (m_kvpMinutesTemplateVM.Key < 1 || (m_kvpMinutesTemplateVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpMinutesTemplateVM.Key > 1 && !Exact)
                    return Browse(ControlMinuteTemplateID, ControlMinuteTemplateDescriptions, null, FilterMinuteTemplateID, FilterMinuteTemplateDescriptions);

                m_dicMinutesTemplateData = GetMinutesTemplateData(false, FilterMinuteTemplateID, FilterMinuteTemplateDescriptions);
                MinutesTemplateVM m_objMinutesTemplateVM = m_dicMinutesTemplateData[0][0];
                this.GetCmp<TextField>(ControlMinuteTemplateID).Value = m_objMinutesTemplateVM.MinuteTemplateID;
                this.GetCmp<TextField>(ControlMinuteTemplateDescriptions).Value = m_objMinutesTemplateVM.MinuteTemplateDescriptions;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, string MinuteTemplateID, string MinuteTemplateDescriptions)
        {
            List<string> m_lstReturn = new List<string>();

            if (MinuteTemplateID == string.Empty)
                m_lstReturn.Add(MinutesTemplateVM.Prop.MinuteTemplateID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (MinuteTemplateDescriptions == string.Empty)
                m_lstReturn.Add(MinutesTemplateVM.Prop.MinuteTemplateDescriptions.Desc + " " + General.EnumDesc(MessageLib.mustFill));

            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(MinutesTemplateVM.Prop.MinuteTemplateID.Name, parameters[MinutesTemplateVM.Prop.MinuteTemplateID.Name]);
            m_dicReturn.Add(MinutesTemplateVM.Prop.MinuteTemplateDescriptions.Name, parameters[MinutesTemplateVM.Prop.MinuteTemplateDescriptions.Name]);

            return m_dicReturn;
        }

        private MinutesTemplateVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            MinutesTemplateVM m_objMinutesTemplateVM = new MinutesTemplateVM();
            MMinuteTemplatesDA m_objMMinuteTemplatesDA = new MMinuteTemplatesDA();
            m_objMMinuteTemplatesDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(MinutesTemplateVM.Prop.MinuteTemplateID.MapAlias);
            m_lstSelect.Add(MinutesTemplateVM.Prop.MinuteTemplateDescriptions.MapAlias);
            m_lstSelect.Add(MinutesTemplateVM.Prop.Contents.MapAlias);
            m_lstSelect.Add(MinutesTemplateVM.Prop.FunctionID.MapAlias);
            m_lstSelect.Add(FunctionsVM.Prop.FunctionDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objMinutesTemplateVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(MinutesTemplateVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMMinuteTemplatesDA = m_objMMinuteTemplatesDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMMinuteTemplatesDA.Message == string.Empty)
            {
                DataRow m_drMMinuteTemplatesDA = m_dicMMinuteTemplatesDA[0].Tables[0].Rows[0];
                m_objMinutesTemplateVM.MinuteTemplateID = m_drMMinuteTemplatesDA[MinutesTemplateVM.Prop.MinuteTemplateID.Name].ToString();
                m_objMinutesTemplateVM.MinuteTemplateDescriptions = m_drMMinuteTemplatesDA[MinutesTemplateVM.Prop.MinuteTemplateDescriptions.Name].ToString();
                m_objMinutesTemplateVM.Contents = m_drMMinuteTemplatesDA[MinutesTemplateVM.Prop.Contents.Name].ToString();
                m_objMinutesTemplateVM.FunctionID = m_drMMinuteTemplatesDA[MinutesTemplateVM.Prop.FunctionID.Name].ToString();
                m_objMinutesTemplateVM.FunctionDesc = m_drMMinuteTemplatesDA[MinutesTemplateVM.Prop.FunctionDesc.Name].ToString();
                m_objMinutesTemplateVM.Tags = GetTags(m_objMinutesTemplateVM.MinuteTemplateID);
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMMinuteTemplatesDA.Message;

            return m_objMinutesTemplateVM;
        }

        private List<string> GetTags(string MinuteTemplateID)
        {
            List<FieldTagReferenceVM> m_lstFieldTagReferenceVM = new List<FieldTagReferenceVM>();
            DTemplateTagsDA m_objDTemplateTagsDA = new DTemplateTagsDA();
            m_objDTemplateTagsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(TemplateTagsVM.Prop.FieldTagID.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(MinuteTemplateID);
            m_objFilter.Add(TemplateTagsVM.Prop.TemplateID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("MOM");
            m_objFilter.Add(TemplateTagsVM.Prop.TemplateType.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicDTemplateTagsDA = m_objDTemplateTagsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDTemplateTagsDA.Message == string.Empty)
            {
                m_lstFieldTagReferenceVM = (
                  from DataRow m_drMMinuteTemplatesDA in m_dicDTemplateTagsDA[0].Tables[0].Rows
                  select new FieldTagReferenceVM
                  {
                      FieldTagID = m_drMMinuteTemplatesDA[TemplateTagsVM.Prop.FieldTagID.Name].ToString()
                  }
                ).ToList();
            }

            return m_lstFieldTagReferenceVM.Select(d => d.FieldTagID).ToList();
        }

        #endregion

        #region Public Method

        public Dictionary<int, List<MinutesTemplateVM>> GetMinutesTemplateData(bool isCount, string MinuteTemplateID, string MinuteTemplateDescriptions)
        {
            int m_intCount = 0;
            List<MinutesTemplateVM> m_lstMinutesTemplateVM = new List<ViewModels.MinutesTemplateVM>();
            Dictionary<int, List<MinutesTemplateVM>> m_dicReturn = new Dictionary<int, List<MinutesTemplateVM>>();
            MMinuteTemplatesDA m_objMMinuteTemplatesDA = new MMinuteTemplatesDA();
            m_objMMinuteTemplatesDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(MinutesTemplateVM.Prop.MinuteTemplateID.MapAlias);
            m_lstSelect.Add(MinutesTemplateVM.Prop.MinuteTemplateDescriptions.MapAlias);
            m_lstSelect.Add(MinutesTemplateVM.Prop.Contents.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(MinuteTemplateID);
            m_objFilter.Add(MinutesTemplateVM.Prop.MinuteTemplateID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(MinuteTemplateDescriptions);
            m_objFilter.Add(MinutesTemplateVM.Prop.MinuteTemplateDescriptions.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMMinuteTemplatesDA = m_objMMinuteTemplatesDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMMinuteTemplatesDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpMinutesTemplateBL in m_dicMMinuteTemplatesDA)
                    {
                        m_intCount = m_kvpMinutesTemplateBL.Key;
                        break;
                    }
                else
                {
                    m_lstMinutesTemplateVM = (
                        from DataRow m_drMMinuteTemplatesDA in m_dicMMinuteTemplatesDA[0].Tables[0].Rows
                        select new MinutesTemplateVM()
                        {
                            MinuteTemplateID = m_drMMinuteTemplatesDA[MinutesTemplateVM.Prop.MinuteTemplateID.Name].ToString(),
                            MinuteTemplateDescriptions = m_drMMinuteTemplatesDA[MinutesTemplateVM.Prop.MinuteTemplateDescriptions.Name].ToString(),
                            Contents = m_drMMinuteTemplatesDA[MinutesTemplateVM.Prop.Contents.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstMinutesTemplateVM);
            return m_dicReturn;
        }

        #endregion
    }
}