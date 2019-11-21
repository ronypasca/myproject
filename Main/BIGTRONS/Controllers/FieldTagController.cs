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
    public class FieldTagController : BaseController
    {
        private readonly string title = "FieldTag";
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
            MFieldTagReferencesDA m_objMFieldTagReferencesDA = new MFieldTagReferencesDA();
            m_objMFieldTagReferencesDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMFieldTags = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMFieldTags.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = FieldTagReferenceVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMFieldTagReferencesDA = m_objMFieldTagReferencesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpFieldTagBL in m_dicMFieldTagReferencesDA)
            {
                m_intCount = m_kvpFieldTagBL.Key;
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
                    m_dicOrder.Add(FieldTagReferenceVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMFieldTagReferencesDA = m_objMFieldTagReferencesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMFieldTagReferencesDA.Message == string.Empty)
                {
                    m_lstFieldTagReferenceVM = (
                        from DataRow m_drMFieldTagReferencesDA in m_dicMFieldTagReferencesDA[0].Tables[0].Rows
                        select new FieldTagReferenceVM()
                        {
                            FieldTagID = m_drMFieldTagReferencesDA[FieldTagReferenceVM.Prop.FieldTagID.Name].ToString(),
                            TagDesc = m_drMFieldTagReferencesDA[FieldTagReferenceVM.Prop.TagDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstFieldTagReferenceVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            MFieldTagReferencesDA m_objMFieldTagReferencesDA = new MFieldTagReferencesDA();
            m_objMFieldTagReferencesDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMFieldTags = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMFieldTags.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = FieldTagReferenceVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMFieldTagReferencesDA = m_objMFieldTagReferencesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpFieldTagBL in m_dicMFieldTagReferencesDA)
            {
                m_intCount = m_kvpFieldTagBL.Key;
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
                    m_dicOrder.Add(FieldTagReferenceVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMFieldTagReferencesDA = m_objMFieldTagReferencesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMFieldTagReferencesDA.Message == string.Empty)
                {
                    m_lstFieldTagReferenceVM = (
                        from DataRow m_drMFieldTagReferencesDA in m_dicMFieldTagReferencesDA[0].Tables[0].Rows
                        select new FieldTagReferenceVM()
                        {
                            FieldTagID = m_drMFieldTagReferencesDA[FieldTagReferenceVM.Prop.FieldTagID.Name].ToString(),
                            TagDesc = m_drMFieldTagReferencesDA[FieldTagReferenceVM.Prop.TagDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstFieldTagReferenceVM, m_intCount);
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

            FieldTagReferenceVM m_objFieldTagReferenceVM = new FieldTagReferenceVM();
            ViewDataDictionary m_vddFieldTag = new ViewDataDictionary();
            m_vddFieldTag.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddFieldTag.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
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
                Model = m_objFieldTagReferenceVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddFieldTag,
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
            FieldTagReferenceVM m_objFieldTagReferenceVM = new FieldTagReferenceVM();
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
                m_objFieldTagReferenceVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddFieldTag = new ViewDataDictionary();
            m_vddFieldTag.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objFieldTagReferenceVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddFieldTag,
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
            FieldTagReferenceVM m_objFieldTagReferenceVM = new FieldTagReferenceVM();
            if (m_dicSelectedRow.Count > 0)
                m_objFieldTagReferenceVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddFieldTag = new ViewDataDictionary();
            m_vddFieldTag.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddFieldTag.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objFieldTagReferenceVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddFieldTag,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<FieldTagReferenceVM> m_lstSelectedRow = new List<FieldTagReferenceVM>();
            m_lstSelectedRow = JSON.Deserialize<List<FieldTagReferenceVM>>(Selected);

            MFieldTagReferencesDA m_objMFieldTagReferencesDA = new MFieldTagReferencesDA();
            m_objMFieldTagReferencesDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (FieldTagReferenceVM m_objFieldTagReferenceVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifFieldTagReferenceVM = m_objFieldTagReferenceVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifFieldTagReferenceVM in m_arrPifFieldTagReferenceVM)
                    {
                        string m_strFieldName = m_pifFieldTagReferenceVM.Name;
                        object m_objFieldValue = m_pifFieldTagReferenceVM.GetValue(m_objFieldTagReferenceVM);
                        if (m_objFieldTagReferenceVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(FieldTagReferenceVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objMFieldTagReferencesDA.DeleteBC(m_objFilter, false);
                    if (m_objMFieldTagReferencesDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMFieldTagReferencesDA.Message);
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

        public ActionResult Browse(string ControlFieldTagID, string ControlTagDesc, string FilterFieldTagID = "", string FilterTagDesc = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddFieldTag = new ViewDataDictionary();
            m_vddFieldTag.Add("Control" + FieldTagReferenceVM.Prop.FieldTagID.Name, ControlFieldTagID);
            m_vddFieldTag.Add("Control" + FieldTagReferenceVM.Prop.TagDesc.Name, ControlTagDesc);
            m_vddFieldTag.Add(FieldTagReferenceVM.Prop.FieldTagID.Name, FilterFieldTagID);
            m_vddFieldTag.Add(FieldTagReferenceVM.Prop.TagDesc.Name, FilterTagDesc);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddFieldTag,
                ViewName = "../FieldTag/_Browse"
            };
        }

        public ActionResult Save(string Action)
        {
            //action = JSON.Deserialize<string>(action);
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MFieldTagReferencesDA m_objMFieldTagReferencesDA = new MFieldTagReferencesDA();
            m_objMFieldTagReferencesDA.ConnectionStringName = Global.ConnStrConfigName;
            try
            {
                string m_strFieldTagID = this.Request.Params[FieldTagReferenceVM.Prop.FieldTagID.Name];
                string m_strTagDesc = this.Request.Params[FieldTagReferenceVM.Prop.TagDesc.Name];
                string m_strRefColumnID = this.Request.Params[FieldTagReferenceVM.Prop.RefIDColumn.Name];
                string m_strRefTableID = this.Request.Params[FieldTagReferenceVM.Prop.RefTable.Name];

                m_lstMessage = IsSaveValid(Action, m_strFieldTagID, m_strTagDesc);
                if (m_lstMessage.Count <= 0)
                {
                    MFieldTagReferences m_objMFieldTags = new MFieldTagReferences();
                    m_objMFieldTags.FieldTagID = m_strFieldTagID;
                    m_objMFieldTagReferencesDA.Data = m_objMFieldTags;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objMFieldTagReferencesDA.Select();

                    m_objMFieldTags.TagDesc = m_strTagDesc;
                    m_objMFieldTags.RefIDColumn = m_strRefColumnID;
                    m_objMFieldTags.RefTable = m_strRefTableID;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objMFieldTagReferencesDA.Insert(false);
                    else
                        m_objMFieldTagReferencesDA.Update(false);

                    if (!m_objMFieldTagReferencesDA.Success || m_objMFieldTagReferencesDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMFieldTagReferencesDA.Message);
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

        public ActionResult GetFieldTag(string ControlFieldTagID, string ControlTagDesc, string FilterFieldTagID, string FilterTagDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<FieldTagReferenceVM>> m_dicFieldTagData = GetFieldTagData(true, FilterFieldTagID, FilterTagDesc);
                KeyValuePair<int, List<FieldTagReferenceVM>> m_kvpFieldTagReferenceVM = m_dicFieldTagData.AsEnumerable().ToList()[0];
                if (m_kvpFieldTagReferenceVM.Key < 1 || (m_kvpFieldTagReferenceVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpFieldTagReferenceVM.Key > 1 && !Exact)
                    return Browse(ControlFieldTagID, ControlTagDesc, FilterFieldTagID, FilterTagDesc);

                m_dicFieldTagData = GetFieldTagData(false, FilterFieldTagID, FilterTagDesc);
                FieldTagReferenceVM m_objFieldTagReferenceVM = m_dicFieldTagData[0][0];
                this.GetCmp<TextField>(ControlFieldTagID).Value = m_objFieldTagReferenceVM.FieldTagID;
                this.GetCmp<TextField>(ControlTagDesc).Value = m_objFieldTagReferenceVM.TagDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, string FieldTagID, string TagDesc)
        {
            List<string> m_lstReturn = new List<string>();

            if (FieldTagID == string.Empty)
                m_lstReturn.Add(FieldTagReferenceVM.Prop.FieldTagID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (TagDesc == string.Empty)
                m_lstReturn.Add(FieldTagReferenceVM.Prop.TagDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));

            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(FieldTagReferenceVM.Prop.FieldTagID.Name, parameters[FieldTagReferenceVM.Prop.FieldTagID.Name]);
            m_dicReturn.Add(FieldTagReferenceVM.Prop.TagDesc.Name, parameters[FieldTagReferenceVM.Prop.TagDesc.Name]);

            return m_dicReturn;
        }

        private FieldTagReferenceVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            FieldTagReferenceVM m_objFieldTagReferenceVM = new FieldTagReferenceVM();
            MFieldTagReferencesDA m_objMFieldTagReferencesDA = new MFieldTagReferencesDA();
            m_objMFieldTagReferencesDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(FieldTagReferenceVM.Prop.FieldTagID.MapAlias);
            m_lstSelect.Add(FieldTagReferenceVM.Prop.TagDesc.MapAlias);
            m_lstSelect.Add(FieldTagReferenceVM.Prop.RefTable.MapAlias);
            m_lstSelect.Add(FieldTagReferenceVM.Prop.RefIDColumn.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objFieldTagReferenceVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(FieldTagReferenceVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMFieldTagReferencesDA = m_objMFieldTagReferencesDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMFieldTagReferencesDA.Message == string.Empty)
            {
                DataRow m_drMFieldTagReferencesDA = m_dicMFieldTagReferencesDA[0].Tables[0].Rows[0];
                m_objFieldTagReferenceVM.FieldTagID = m_drMFieldTagReferencesDA[FieldTagReferenceVM.Prop.FieldTagID.Name].ToString();
                m_objFieldTagReferenceVM.TagDesc = m_drMFieldTagReferencesDA[FieldTagReferenceVM.Prop.TagDesc.Name].ToString();
                m_objFieldTagReferenceVM.RefTable = m_drMFieldTagReferencesDA[FieldTagReferenceVM.Prop.RefTable.Name].ToString();
                m_objFieldTagReferenceVM.RefIDColumn = m_drMFieldTagReferencesDA[FieldTagReferenceVM.Prop.RefIDColumn.Name].ToString();
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMFieldTagReferencesDA.Message;

            return m_objFieldTagReferenceVM;
        }

        #endregion

        #region Public Method

        public Dictionary<int, List<FieldTagReferenceVM>> GetFieldTagData(bool isCount, string FieldTagID, string TagDesc)
        {
            int m_intCount = 0;
            List<FieldTagReferenceVM> m_lstFieldTagReferenceVM = new List<ViewModels.FieldTagReferenceVM>();
            Dictionary<int, List<FieldTagReferenceVM>> m_dicReturn = new Dictionary<int, List<FieldTagReferenceVM>>();
            MFieldTagReferencesDA m_objMFieldTagReferencesDA = new MFieldTagReferencesDA();
            m_objMFieldTagReferencesDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(FieldTagReferenceVM.Prop.FieldTagID.MapAlias);
            m_lstSelect.Add(FieldTagReferenceVM.Prop.TagDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(FieldTagID);
            m_objFilter.Add(FieldTagReferenceVM.Prop.FieldTagID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(TagDesc);
            m_objFilter.Add(FieldTagReferenceVM.Prop.TagDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMFieldTagReferencesDA = m_objMFieldTagReferencesDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMFieldTagReferencesDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpFieldTagBL in m_dicMFieldTagReferencesDA)
                    {
                        m_intCount = m_kvpFieldTagBL.Key;
                        break;
                    }
                else
                {
                    m_lstFieldTagReferenceVM = (
                        from DataRow m_drMFieldTagReferencesDA in m_dicMFieldTagReferencesDA[0].Tables[0].Rows
                        select new FieldTagReferenceVM()
                        {
                            FieldTagID = m_drMFieldTagReferencesDA[FieldTagReferenceVM.Prop.FieldTagID.Name].ToString(),
                            TagDesc = m_drMFieldTagReferencesDA[FieldTagReferenceVM.Prop.TagDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstFieldTagReferenceVM);
            return m_dicReturn;
        }

        #endregion
    }
}