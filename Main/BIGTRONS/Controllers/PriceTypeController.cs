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
    public class PriceTypeController : BaseController
    {
        private readonly string title = "Price Type";
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
            MPriceTypeDA m_objSPriceTypeDA = new MPriceTypeDA();
            m_objSPriceTypeDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMPriceType = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMPriceType.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = PriceTypeVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicSPriceTypeDA = m_objSPriceTypeDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpPriceTypeBL in m_dicSPriceTypeDA)
            {
                m_intCount = m_kvpPriceTypeBL.Key;
                break;
            }

            List<PriceTypeVM> m_lstPriceTypeVM = new List<PriceTypeVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(PriceTypeVM.Prop.PriceTypeID.MapAlias);
                m_lstSelect.Add(PriceTypeVM.Prop.PriceTypeDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(PriceTypeVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicSPriceTypeDA = m_objSPriceTypeDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objSPriceTypeDA.Message == string.Empty)
                {
                    m_lstPriceTypeVM = (
                        from DataRow m_drMPriceTypeDA in m_dicSPriceTypeDA[0].Tables[0].Rows
                        select new PriceTypeVM()
                        {
                            PriceTypeID = m_drMPriceTypeDA[PriceTypeVM.Prop.PriceTypeID.Name].ToString(),
                            PriceTypeDesc = m_drMPriceTypeDA[PriceTypeVM.Prop.PriceTypeDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstPriceTypeVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            MPriceTypeDA m_objMPriceTypeDA = new MPriceTypeDA();
            m_objMPriceTypeDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMPriceType = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMPriceType.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = PriceTypeVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMPriceTypeDA = m_objMPriceTypeDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpPriceTypeBL in m_dicMPriceTypeDA)
            {
                m_intCount = m_kvpPriceTypeBL.Key;
                break;
            }

            List<PriceTypeVM> m_lstPriceTypeVM = new List<PriceTypeVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(PriceTypeVM.Prop.PriceTypeID.MapAlias);
                m_lstSelect.Add(PriceTypeVM.Prop.PriceTypeDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(PriceTypeVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMPriceTypeDA = m_objMPriceTypeDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMPriceTypeDA.Message == string.Empty)
                {
                    m_lstPriceTypeVM = (
                        from DataRow m_drMPriceTypeDA in m_dicMPriceTypeDA[0].Tables[0].Rows
                        select new PriceTypeVM()
                        {
                            PriceTypeID = m_drMPriceTypeDA[PriceTypeVM.Prop.PriceTypeID.Name].ToString(),
                            PriceTypeDesc = m_drMPriceTypeDA[PriceTypeVM.Prop.PriceTypeDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstPriceTypeVM, m_intCount);
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

            PriceTypeVM m_objPriceTypeVM = new PriceTypeVM();
            ViewDataDictionary m_vddPriceType = new ViewDataDictionary();
            m_vddPriceType.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddPriceType.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
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
                Model = m_objPriceTypeVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddPriceType,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Detail(string Caller, string Selected)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            PriceTypeVM m_objPriceTypeVM = new PriceTypeVM();
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
                m_objPriceTypeVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
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
                Model = m_objPriceTypeVM,
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
            PriceTypeVM m_objPriceTypeVM = new PriceTypeVM();
            if (m_dicSelectedRow.Count > 0)
                m_objPriceTypeVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddPriceType = new ViewDataDictionary();
            m_vddPriceType.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddPriceType.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objPriceTypeVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddPriceType,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<PriceTypeVM> m_lstSelectedRow = new List<PriceTypeVM>();
            m_lstSelectedRow = JSON.Deserialize<List<PriceTypeVM>>(Selected);

            MPriceTypeDA m_objMPriceTypeDA = new MPriceTypeDA();
            m_objMPriceTypeDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (PriceTypeVM m_objPriceTypeVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifPriceTypeVM = m_objPriceTypeVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifPriceTypeVM in m_arrPifPriceTypeVM)
                    {
                        string m_strFieldName = m_pifPriceTypeVM.Name;
                        object m_objFieldValue = m_pifPriceTypeVM.GetValue(m_objPriceTypeVM);
                        if (m_objPriceTypeVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(PriceTypeVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objMPriceTypeDA.DeleteBC(m_objFilter, false);
                    if (m_objMPriceTypeDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMPriceTypeDA.Message);
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

        public ActionResult Browse(string ControlPriceTypeID, string ControlPriceTypeDesc, string FilterPriceTypeID = "", string FilterPriceTypeDesc = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddPriceType = new ViewDataDictionary();
            m_vddPriceType.Add("Control" + PriceTypeVM.Prop.PriceTypeID.Name, ControlPriceTypeID);
            m_vddPriceType.Add("Control" + PriceTypeVM.Prop.PriceTypeDesc.Name, ControlPriceTypeDesc);
            m_vddPriceType.Add(PriceTypeVM.Prop.PriceTypeID.Name, FilterPriceTypeID);
            m_vddPriceType.Add(PriceTypeVM.Prop.PriceTypeDesc.Name, FilterPriceTypeDesc);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddPriceType,
                ViewName = "../PriceType/_Browse"
            };
        }

        public ActionResult Save(string Action)
        {
            //action = JSON.Deserialize<string>(action);
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MPriceTypeDA m_objMPriceTypeDA = new MPriceTypeDA();
            m_objMPriceTypeDA.ConnectionStringName = Global.ConnStrConfigName;
            try
            {
                string m_strPriceTypeID = this.Request.Params[PriceTypeVM.Prop.PriceTypeID.Name];
                string m_strPriceTypeDesc = this.Request.Params[PriceTypeVM.Prop.PriceTypeDesc.Name];

                m_lstMessage = IsSaveValid(Action, m_strPriceTypeID, m_strPriceTypeDesc);
                if (m_lstMessage.Count <= 0)
                {
                    MPriceType m_objMPriceType = new MPriceType();
                    m_objMPriceType.PriceTypeID = m_strPriceTypeID;
                    m_objMPriceTypeDA.Data = m_objMPriceType;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objMPriceTypeDA.Select();

                    m_objMPriceType.PriceTypeDesc = m_strPriceTypeDesc;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objMPriceTypeDA.Insert(false);
                    else
                        m_objMPriceTypeDA.Update(false);

                    if (!m_objMPriceTypeDA.Success || m_objMPriceTypeDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMPriceTypeDA.Message);
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

        public ActionResult GetPriceType(string ControlPriceTypeID, string ControlPriceTypeDesc, string FilterPriceTypeID, string FilterPriceTypeDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<PriceTypeVM>> m_dicPriceTypeData = GetPriceTypeData(true, FilterPriceTypeID, FilterPriceTypeDesc);
                KeyValuePair<int, List<PriceTypeVM>> m_kvpPriceTypeVM = m_dicPriceTypeData.AsEnumerable().ToList()[0];
                if (m_kvpPriceTypeVM.Key < 1 || (m_kvpPriceTypeVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpPriceTypeVM.Key > 1 && !Exact)
                    return Browse(ControlPriceTypeID, ControlPriceTypeDesc, FilterPriceTypeID, FilterPriceTypeDesc);

                m_dicPriceTypeData = GetPriceTypeData(false, FilterPriceTypeID, FilterPriceTypeDesc);
                PriceTypeVM m_objPriceTypeVM = m_dicPriceTypeData[0][0];
                this.GetCmp<TextField>(ControlPriceTypeID).Value = m_objPriceTypeVM.PriceTypeID;
                this.GetCmp<TextField>(ControlPriceTypeDesc).Value = m_objPriceTypeVM.PriceTypeDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, string PriceTypeID, string PriceTypeDesc)
        {
            List<string> m_lstReturn = new List<string>();

            if (PriceTypeID == string.Empty)
                m_lstReturn.Add(PriceTypeVM.Prop.PriceTypeID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (PriceTypeDesc == string.Empty)
                m_lstReturn.Add(PriceTypeVM.Prop.PriceTypeDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));

            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(PriceTypeVM.Prop.PriceTypeID.Name, parameters[PriceTypeVM.Prop.PriceTypeID.Name]);
            m_dicReturn.Add(PriceTypeVM.Prop.PriceTypeDesc.Name, parameters[PriceTypeVM.Prop.PriceTypeDesc.Name]);

            return m_dicReturn;
        }

        private PriceTypeVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            PriceTypeVM m_objPriceTypeVM = new PriceTypeVM();
            MPriceTypeDA m_objMPriceTypeDA = new MPriceTypeDA();
            m_objMPriceTypeDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(PriceTypeVM.Prop.PriceTypeID.MapAlias);
            m_lstSelect.Add(PriceTypeVM.Prop.PriceTypeDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objPriceTypeVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(PriceTypeVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMPriceTypeDA = m_objMPriceTypeDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMPriceTypeDA.Message == string.Empty)
            {
                DataRow m_drMPriceTypeDA = m_dicMPriceTypeDA[0].Tables[0].Rows[0];
                m_objPriceTypeVM.PriceTypeID = m_drMPriceTypeDA[PriceTypeVM.Prop.PriceTypeID.Name].ToString();
                m_objPriceTypeVM.PriceTypeDesc = m_drMPriceTypeDA[PriceTypeVM.Prop.PriceTypeDesc.Name].ToString();
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMPriceTypeDA.Message;

            return m_objPriceTypeVM;
        }

        #endregion

        #region Public Method

        public Dictionary<int, List<PriceTypeVM>> GetPriceTypeData(bool isCount, string PriceTypeID, string PriceTypeDesc)
        {
            int m_intCount = 0;
            List<PriceTypeVM> m_lstPriceTypeVM = new List<ViewModels.PriceTypeVM>();
            Dictionary<int, List<PriceTypeVM>> m_dicReturn = new Dictionary<int, List<PriceTypeVM>>();
            MPriceTypeDA m_objMPriceTypeDA = new MPriceTypeDA();
            m_objMPriceTypeDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(PriceTypeVM.Prop.PriceTypeID.MapAlias);
            m_lstSelect.Add(PriceTypeVM.Prop.PriceTypeDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(PriceTypeID);
            m_objFilter.Add(PriceTypeVM.Prop.PriceTypeID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(PriceTypeDesc);
            m_objFilter.Add(PriceTypeVM.Prop.PriceTypeDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMPriceTypeDA = m_objMPriceTypeDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMPriceTypeDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpPriceTypeBL in m_dicMPriceTypeDA)
                    {
                        m_intCount = m_kvpPriceTypeBL.Key;
                        break;
                    }
                else
                {
                    m_lstPriceTypeVM = (
                        from DataRow m_drMPriceTypeDA in m_dicMPriceTypeDA[0].Tables[0].Rows
                        select new PriceTypeVM()
                        {
                            PriceTypeID = m_drMPriceTypeDA[PriceTypeVM.Prop.PriceTypeID.Name].ToString(),
                            PriceTypeDesc = m_drMPriceTypeDA[PriceTypeVM.Prop.PriceTypeDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstPriceTypeVM);
            return m_dicReturn;
        }

        #endregion
    }
}