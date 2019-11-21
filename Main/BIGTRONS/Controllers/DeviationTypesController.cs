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
    public class DeviationTypesController : BaseController
    {
        private readonly string title = "Deviation Types";
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
            MDeviationTypesDA m_objMDeviationTypesDA = new MDeviationTypesDA();
            m_objMDeviationTypesDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMDeviationTypes = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMDeviationTypes.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = DeviationTypesVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMDeviationTypesDA = m_objMDeviationTypesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpDeviationTypesBL in m_dicMDeviationTypesDA)
            {
                m_intCount = m_kvpDeviationTypesBL.Key;
                break;
            }

            List<DeviationTypesVM> m_lstDeviationTypesVM = new List<DeviationTypesVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(DeviationTypesVM.Prop.DeviationTypeID.MapAlias);
                m_lstSelect.Add(DeviationTypesVM.Prop.Descriptions.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(DeviationTypesVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMDeviationTypesDA = m_objMDeviationTypesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMDeviationTypesDA.Message == string.Empty)
                {
                    m_lstDeviationTypesVM = (
                        from DataRow m_drMDeviationTypesDA in m_dicMDeviationTypesDA[0].Tables[0].Rows
                        select new DeviationTypesVM()
                        {
                            DeviationTypeID = m_drMDeviationTypesDA[DeviationTypesVM.Prop.DeviationTypeID.Name].ToString(),
                            Descriptions = m_drMDeviationTypesDA[DeviationTypesVM.Prop.Descriptions.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstDeviationTypesVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            MDeviationTypesDA m_objMDeviationTypesDA = new MDeviationTypesDA();
            m_objMDeviationTypesDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMDeviationTypes = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMDeviationTypes.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = DeviationTypesVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMDeviationTypesDA = m_objMDeviationTypesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpDeviationTypesBL in m_dicMDeviationTypesDA)
            {
                m_intCount = m_kvpDeviationTypesBL.Key;
                break;
            }

            List<DeviationTypesVM> m_lstDeviationTypesVM = new List<DeviationTypesVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(DeviationTypesVM.Prop.DeviationTypeID.MapAlias);
                m_lstSelect.Add(DeviationTypesVM.Prop.Descriptions.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(DeviationTypesVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMDeviationTypesDA = m_objMDeviationTypesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMDeviationTypesDA.Message == string.Empty)
                {
                    m_lstDeviationTypesVM = (
                        from DataRow m_drMDeviationTypesDA in m_dicMDeviationTypesDA[0].Tables[0].Rows
                        select new DeviationTypesVM()
                        {
                            DeviationTypeID = m_drMDeviationTypesDA[DeviationTypesVM.Prop.DeviationTypeID.Name].ToString(),
                            Descriptions = m_drMDeviationTypesDA[DeviationTypesVM.Prop.Descriptions.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstDeviationTypesVM, m_intCount);
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

            DeviationTypesVM m_objDeviationTypesVM = new DeviationTypesVM();
            ViewDataDictionary m_vddDeviationTypes = new ViewDataDictionary();
            m_vddDeviationTypes.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddDeviationTypes.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
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
                Model = m_objDeviationTypesVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddDeviationTypes,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Detail(string Caller, string Selected, string DeviationTypeID)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            DeviationTypesVM m_objDeviationTypesVM = new DeviationTypesVM();
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
                m_dicSelectedRow = GetFormData(m_nvcParams, DeviationTypeID);
            }

            if (m_dicSelectedRow.Count > 0)
                m_objDeviationTypesVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
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
                Model = m_objDeviationTypesVM,
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
            DeviationTypesVM m_objDeviationTypesVM = new DeviationTypesVM();
            if (m_dicSelectedRow.Count > 0)
                m_objDeviationTypesVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddDeviationTypes = new ViewDataDictionary();
            m_vddDeviationTypes.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddDeviationTypes.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objDeviationTypesVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddDeviationTypes,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<DeviationTypesVM> m_lstSelectedRow = new List<DeviationTypesVM>();
            m_lstSelectedRow = JSON.Deserialize<List<DeviationTypesVM>>(Selected);

            MDeviationTypesDA m_objMDeviationTypesDA = new MDeviationTypesDA();
            m_objMDeviationTypesDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (DeviationTypesVM m_objDeviationTypesVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifDeviationTypesVM = m_objDeviationTypesVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifDeviationTypesVM in m_arrPifDeviationTypesVM)
                    {
                        string m_strFieldName = m_pifDeviationTypesVM.Name;
                        object m_objFieldValue = m_pifDeviationTypesVM.GetValue(m_objDeviationTypesVM);
                        if (m_objDeviationTypesVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(DeviationTypesVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objMDeviationTypesDA.DeleteBC(m_objFilter, false);
                    if (m_objMDeviationTypesDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMDeviationTypesDA.Message);
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

        public ActionResult Browse(string ControlDeviationTypeID, string ControlDescriptions, string FilterDeviationTypeID = "", string FilterDescriptions = "")
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddDeviationTypes = new ViewDataDictionary();
            m_vddDeviationTypes.Add("Control" + DeviationTypesVM.Prop.DeviationTypeID.Name, ControlDeviationTypeID);
            m_vddDeviationTypes.Add("Control" + DeviationTypesVM.Prop.Descriptions.Name, ControlDescriptions);
            m_vddDeviationTypes.Add(DeviationTypesVM.Prop.DeviationTypeID.Name, FilterDeviationTypeID);
            m_vddDeviationTypes.Add(DeviationTypesVM.Prop.Descriptions.Name, FilterDescriptions);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddDeviationTypes,
                ViewName = "../DeviationTypes/_Browse"
            };
        }

        public ActionResult Save(string Action)
        {
            //action = JSON.Deserialize<string>(action);
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MDeviationTypesDA m_objMDeviationTypesDA = new MDeviationTypesDA();
            string m_strDeviationTypeID = string.Empty;
            m_objMDeviationTypesDA.ConnectionStringName = Global.ConnStrConfigName;
            try
            {
                m_strDeviationTypeID = this.Request.Params[DeviationTypesVM.Prop.DeviationTypeID.Name];
                string m_strDescriptions = this.Request.Params[DeviationTypesVM.Prop.Descriptions.Name];

                m_lstMessage = IsSaveValid(Action, m_strDeviationTypeID, m_strDescriptions);
                if (m_lstMessage.Count <= 0)
                {
                    MDeviationTypes m_objMDeviationTypes = new MDeviationTypes();
                    m_objMDeviationTypes.DeviationTypeID = m_strDeviationTypeID;
                    m_objMDeviationTypesDA.Data = m_objMDeviationTypes;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objMDeviationTypesDA.Select();

                    m_objMDeviationTypes.Descriptions = m_strDescriptions;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objMDeviationTypesDA.Insert(false);
                    else
                        m_objMDeviationTypesDA.Update(false);

                    if (!m_objMDeviationTypesDA.Success || m_objMDeviationTypesDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMDeviationTypesDA.Message);
                    m_strDeviationTypeID = m_objMDeviationTypesDA.Data.DeviationTypeID;
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
            }
            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
                return Detail(General.EnumDesc(Buttons.ButtonSave), null, m_strDeviationTypeID);
            }
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);
        }

        #endregion

        #region Direct Method

        public ActionResult GetDeviationTypes(string ControlDeviationTypeID, string ControlDescriptions, string FilterDeviationTypeID, string FilterDescriptions, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<DeviationTypesVM>> m_dicDeviationTypesData = GetDeviationTypesData(true, FilterDeviationTypeID, FilterDescriptions);
                KeyValuePair<int, List<DeviationTypesVM>> m_kvpDeviationTypesVM = m_dicDeviationTypesData.AsEnumerable().ToList()[0];
                if (m_kvpDeviationTypesVM.Key < 1 || (m_kvpDeviationTypesVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpDeviationTypesVM.Key > 1 && !Exact)
                    return Browse(ControlDeviationTypeID, ControlDescriptions, FilterDeviationTypeID, FilterDescriptions);

                m_dicDeviationTypesData = GetDeviationTypesData(false, FilterDeviationTypeID, FilterDescriptions);
                DeviationTypesVM m_objDeviationTypesVM = m_dicDeviationTypesData[0][0];
                this.GetCmp<TextField>(ControlDeviationTypeID).Value = m_objDeviationTypesVM.DeviationTypeID;
                this.GetCmp<TextField>(ControlDescriptions).Value = m_objDeviationTypesVM.Descriptions;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, string DeviationTypeID, string Descriptions)
        {
            List<string> m_lstReturn = new List<string>();

            //if (DeviationTypeID == string.Empty)
            //    m_lstReturn.Add(DeviationTypesVM.Prop.DeviationTypeID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (Descriptions == string.Empty)
                m_lstReturn.Add(DeviationTypesVM.Prop.Descriptions.Desc + " " + General.EnumDesc(MessageLib.mustFill));

            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters, string DeviationTypeID = "")
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();
            m_dicReturn.Add(DeviationTypesVM.Prop.DeviationTypeID.Name, (parameters[DeviationTypesVM.Prop.DeviationTypeID.Name].ToString() == string.Empty ? DeviationTypeID : parameters[DeviationTypesVM.Prop.DeviationTypeID.Name]));
            m_dicReturn.Add(DeviationTypesVM.Prop.Descriptions.Name, parameters[DeviationTypesVM.Prop.Descriptions.Name]);

            return m_dicReturn;
        }

        private DeviationTypesVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            DeviationTypesVM m_objDeviationTypesVM = new DeviationTypesVM();
            MDeviationTypesDA m_objMDeviationTypesDA = new MDeviationTypesDA();
            m_objMDeviationTypesDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(DeviationTypesVM.Prop.DeviationTypeID.MapAlias);
            m_lstSelect.Add(DeviationTypesVM.Prop.Descriptions.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objDeviationTypesVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(DeviationTypesVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMDeviationTypesDA = m_objMDeviationTypesDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMDeviationTypesDA.Message == string.Empty)
            {
                DataRow m_drMDeviationTypesDA = m_dicMDeviationTypesDA[0].Tables[0].Rows[0];
                m_objDeviationTypesVM.DeviationTypeID = m_drMDeviationTypesDA[DeviationTypesVM.Prop.DeviationTypeID.Name].ToString();
                m_objDeviationTypesVM.Descriptions = m_drMDeviationTypesDA[DeviationTypesVM.Prop.Descriptions.Name].ToString();
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMDeviationTypesDA.Message;

            return m_objDeviationTypesVM;
        }

        #endregion

        #region Public Method

        public Dictionary<int, List<DeviationTypesVM>> GetDeviationTypesData(bool isCount, string DeviationTypeID, string Descriptions)
        {
            int m_intCount = 0;
            List<DeviationTypesVM> m_lstDeviationTypesVM = new List<ViewModels.DeviationTypesVM>();
            Dictionary<int, List<DeviationTypesVM>> m_dicReturn = new Dictionary<int, List<DeviationTypesVM>>();
            MDeviationTypesDA m_objMDeviationTypesDA = new MDeviationTypesDA();
            m_objMDeviationTypesDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(DeviationTypesVM.Prop.DeviationTypeID.MapAlias);
            m_lstSelect.Add(DeviationTypesVM.Prop.Descriptions.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(DeviationTypeID);
            m_objFilter.Add(DeviationTypesVM.Prop.DeviationTypeID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(Descriptions);
            m_objFilter.Add(DeviationTypesVM.Prop.Descriptions.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMDeviationTypesDA = m_objMDeviationTypesDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMDeviationTypesDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpDeviationTypesBL in m_dicMDeviationTypesDA)
                    {
                        m_intCount = m_kvpDeviationTypesBL.Key;
                        break;
                    }
                else
                {
                    m_lstDeviationTypesVM = (
                        from DataRow m_drMDeviationTypesDA in m_dicMDeviationTypesDA[0].Tables[0].Rows
                        select new DeviationTypesVM()
                        {
                            DeviationTypeID = m_drMDeviationTypesDA[DeviationTypesVM.Prop.DeviationTypeID.Name].ToString(),
                            Descriptions = m_drMDeviationTypesDA[DeviationTypesVM.Prop.Descriptions.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstDeviationTypesVM);
            return m_dicReturn;
        }

        #endregion

    }
}