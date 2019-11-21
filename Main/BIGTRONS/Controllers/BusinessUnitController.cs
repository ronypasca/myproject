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
    public class BusinessUnitController : BaseController
    {
        private readonly string title = "BusinessUnit";
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
            MBusinessUnitDA m_objMBusinessUnitDA = new MBusinessUnitDA();
            m_objMBusinessUnitDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMBusinessUnit = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMBusinessUnit.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = BusinessUnitVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMBusinessUnitDA = m_objMBusinessUnitDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpBusinessUnitBL in m_dicMBusinessUnitDA)
            {
                m_intCount = m_kvpBusinessUnitBL.Key;
                break;
            }

            List<BusinessUnitVM> m_lstBusinessUnitVM = new List<BusinessUnitVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(BusinessUnitVM.Prop.BusinessUnitID.MapAlias);
                m_lstSelect.Add(BusinessUnitVM.Prop.BusinessUnitDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(BusinessUnitVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMBusinessUnitDA = m_objMBusinessUnitDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMBusinessUnitDA.Message == string.Empty)
                {
                    m_lstBusinessUnitVM = (
                        from DataRow m_drMBusinessUnitDA in m_dicMBusinessUnitDA[0].Tables[0].Rows
                        select new BusinessUnitVM()
                        {
                            BusinessUnitID = m_drMBusinessUnitDA[BusinessUnitVM.Prop.BusinessUnitID.Name].ToString(),
                            BusinessUnitDesc = m_drMBusinessUnitDA[BusinessUnitVM.Prop.BusinessUnitDesc.Name].ToString(),
                            
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstBusinessUnitVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            MBusinessUnitDA m_objMBusinessUnitDA = new MBusinessUnitDA();
            m_objMBusinessUnitDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMBusinessUnit = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMBusinessUnit.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = BusinessUnitVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMBusinessUnitDA = m_objMBusinessUnitDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpBusinessUnitBL in m_dicMBusinessUnitDA)
            {
                m_intCount = m_kvpBusinessUnitBL.Key;
                break;
            }

            List<BusinessUnitVM> m_lstBusinessUnitVM = new List<BusinessUnitVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(BusinessUnitVM.Prop.BusinessUnitID.MapAlias);
                m_lstSelect.Add(BusinessUnitVM.Prop.BusinessUnitDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(BusinessUnitVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMBusinessUnitDA = m_objMBusinessUnitDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMBusinessUnitDA.Message == string.Empty)
                {
                    m_lstBusinessUnitVM = (
                        from DataRow m_drMBusinessUnitDA in m_dicMBusinessUnitDA[0].Tables[0].Rows
                        select new BusinessUnitVM()
                        {
                            BusinessUnitID = m_drMBusinessUnitDA[BusinessUnitVM.Prop.BusinessUnitID.Name].ToString(),
                            BusinessUnitDesc = m_drMBusinessUnitDA[BusinessUnitVM.Prop.BusinessUnitDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstBusinessUnitVM, m_intCount);
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

            BusinessUnitVM m_objBusinessUnitVM = new BusinessUnitVM();
            ViewDataDictionary m_vddBusinessUnit = new ViewDataDictionary();
            m_vddBusinessUnit.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddBusinessUnit.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
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
                Model = m_objBusinessUnitVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddBusinessUnit,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Detail(string Caller, string Selected)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            BusinessUnitVM m_objBusinessUnitVM = new BusinessUnitVM();
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
                m_objBusinessUnitVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
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
                Model = m_objBusinessUnitVM,
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
            BusinessUnitVM m_objBusinessUnitVM = new BusinessUnitVM();
            if (m_dicSelectedRow.Count > 0)
                m_objBusinessUnitVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddBusinessUnit = new ViewDataDictionary();
            m_vddBusinessUnit.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddBusinessUnit.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objBusinessUnitVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddBusinessUnit,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<BusinessUnitVM> m_lstSelectedRow = new List<BusinessUnitVM>();
            m_lstSelectedRow = JSON.Deserialize<List<BusinessUnitVM>>(Selected);

            MBusinessUnitDA m_objMBusinessUnitDA = new MBusinessUnitDA();
            m_objMBusinessUnitDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (BusinessUnitVM m_objBusinessUnitVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifBusinessUnitVM = m_objBusinessUnitVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifBusinessUnitVM in m_arrPifBusinessUnitVM)
                    {
                        string m_strFieldName = m_pifBusinessUnitVM.Name;
                        object m_objFieldValue = m_pifBusinessUnitVM.GetValue(m_objBusinessUnitVM);
                        if (m_objBusinessUnitVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(BusinessUnitVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objMBusinessUnitDA.DeleteBC(m_objFilter, false);
                    if (m_objMBusinessUnitDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMBusinessUnitDA.Message);
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

        public ActionResult Browse(string ControlBusinessUnitID, string ControlBusinessUnitDesc, string FilterBusinessUnitID = "", string FilterBusinessUnitDesc = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddBusinessUnit = new ViewDataDictionary();
            m_vddBusinessUnit.Add("Control" + BusinessUnitVM.Prop.BusinessUnitID.Name, ControlBusinessUnitID);
            m_vddBusinessUnit.Add("Control" + BusinessUnitVM.Prop.BusinessUnitDesc.Name, ControlBusinessUnitDesc);
            m_vddBusinessUnit.Add(BusinessUnitVM.Prop.BusinessUnitID.Name, FilterBusinessUnitID);
            m_vddBusinessUnit.Add(BusinessUnitVM.Prop.BusinessUnitDesc.Name, FilterBusinessUnitDesc);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddBusinessUnit,
                ViewName = "../BusinessUnit/_Browse"
            };
        }

        public ActionResult Save(string Action)
        {
            //action = JSON.Deserialize<string>(action);
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MBusinessUnitDA m_objMBusinessUnitDA = new MBusinessUnitDA();
            m_objMBusinessUnitDA.ConnectionStringName = Global.ConnStrConfigName;
            try
            {
                string m_strBusinessUnitID = this.Request.Params[BusinessUnitVM.Prop.BusinessUnitID.Name];
                string m_strBusinessUnitDesc = this.Request.Params[BusinessUnitVM.Prop.BusinessUnitDesc.Name];
                string m_strDimensionID = this.Request.Params[DimensionVM.Prop.DimensionID.Name];

                m_lstMessage = IsSaveValid(Action, m_strBusinessUnitID, m_strBusinessUnitDesc, m_strDimensionID);
                if (m_lstMessage.Count <= 0)
                {
                    MBusinessUnit m_objMBusinessUnit = new MBusinessUnit();
                    m_objMBusinessUnit.BusinessUnitID = m_strBusinessUnitID;
                    m_objMBusinessUnitDA.Data = m_objMBusinessUnit;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objMBusinessUnitDA.Select();

                    m_objMBusinessUnit.Descriptions = m_strBusinessUnitDesc;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objMBusinessUnitDA.Insert(false);
                    else
                        m_objMBusinessUnitDA.Update(false);

                    if (!m_objMBusinessUnitDA.Success || m_objMBusinessUnitDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMBusinessUnitDA.Message);
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

        public ActionResult GetBusinessUnit(string ControlBusinessUnitID, string ControlBusinessUnitDesc, string FilterBusinessUnitID, string FilterBusinessUnitDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<BusinessUnitVM>> m_dicBusinessUnitData = GetBusinessUnitData(true, FilterBusinessUnitID, FilterBusinessUnitDesc);
                KeyValuePair<int, List<BusinessUnitVM>> m_kvpBusinessUnitVM = m_dicBusinessUnitData.AsEnumerable().ToList()[0];
                if (m_kvpBusinessUnitVM.Key < 1 || (m_kvpBusinessUnitVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpBusinessUnitVM.Key > 1 && !Exact)
                    return Browse(ControlBusinessUnitID, ControlBusinessUnitDesc, FilterBusinessUnitID, FilterBusinessUnitDesc);

                m_dicBusinessUnitData = GetBusinessUnitData(false, FilterBusinessUnitID, FilterBusinessUnitDesc);
                BusinessUnitVM m_objBusinessUnitVM = m_dicBusinessUnitData[0][0];
                this.GetCmp<TextField>(ControlBusinessUnitID).Value = m_objBusinessUnitVM.BusinessUnitID;
                this.GetCmp<TextField>(ControlBusinessUnitDesc).Value = m_objBusinessUnitVM.BusinessUnitDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, string BusinessUnitID, string BusinessUnitDesc, string DimensionID)
        {
            List<string> m_lstReturn = new List<string>();

            if (BusinessUnitID == string.Empty)
                m_lstReturn.Add(BusinessUnitVM.Prop.BusinessUnitID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            //if (BusinessUnitDesc == string.Empty)
                //m_lstReturn.Add(BusinessUnitVM.Prop.BusinessUnitDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            
            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(BusinessUnitVM.Prop.BusinessUnitID.Name, parameters[BusinessUnitVM.Prop.BusinessUnitID.Name]);
            m_dicReturn.Add(BusinessUnitVM.Prop.BusinessUnitDesc.Name, parameters[BusinessUnitVM.Prop.BusinessUnitDesc.Name]);
            
            return m_dicReturn;
        }

        private BusinessUnitVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            BusinessUnitVM m_objBusinessUnitVM = new BusinessUnitVM();
            MBusinessUnitDA m_objMBusinessUnitDA = new MBusinessUnitDA();
            m_objMBusinessUnitDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BusinessUnitVM.Prop.BusinessUnitID.MapAlias);
            m_lstSelect.Add(BusinessUnitVM.Prop.BusinessUnitDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objBusinessUnitVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(BusinessUnitVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMBusinessUnitDA = m_objMBusinessUnitDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMBusinessUnitDA.Message == string.Empty)
            {
                DataRow m_drMBusinessUnitDA = m_dicMBusinessUnitDA[0].Tables[0].Rows[0];
                m_objBusinessUnitVM.BusinessUnitID = m_drMBusinessUnitDA[BusinessUnitVM.Prop.BusinessUnitID.Name].ToString();
                m_objBusinessUnitVM.BusinessUnitDesc = m_drMBusinessUnitDA[BusinessUnitVM.Prop.BusinessUnitDesc.Name].ToString();
          }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMBusinessUnitDA.Message;

            return m_objBusinessUnitVM;
        }

        #endregion

        #region Public Method

        public Dictionary<int, List<BusinessUnitVM>> GetBusinessUnitData(bool isCount, string BusinessUnitID, string BusinessUnitDesc)
        {
            int m_intCount = 0;
            List<BusinessUnitVM> m_lstBusinessUnitVM = new List<ViewModels.BusinessUnitVM>();
            Dictionary<int, List<BusinessUnitVM>> m_dicReturn = new Dictionary<int, List<BusinessUnitVM>>();
            MBusinessUnitDA m_objMBusinessUnitDA = new MBusinessUnitDA();
            m_objMBusinessUnitDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BusinessUnitVM.Prop.BusinessUnitID.MapAlias);
            m_lstSelect.Add(BusinessUnitVM.Prop.BusinessUnitDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(BusinessUnitID);
            m_objFilter.Add(BusinessUnitVM.Prop.BusinessUnitID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(BusinessUnitDesc);
            m_objFilter.Add(BusinessUnitVM.Prop.BusinessUnitDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMBusinessUnitDA = m_objMBusinessUnitDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMBusinessUnitDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpBusinessUnitBL in m_dicMBusinessUnitDA)
                    {
                        m_intCount = m_kvpBusinessUnitBL.Key;
                        break;
                    }
                else
                {
                    m_lstBusinessUnitVM = (
                        from DataRow m_drMBusinessUnitDA in m_dicMBusinessUnitDA[0].Tables[0].Rows
                        select new BusinessUnitVM()
                        {
                            BusinessUnitID = m_drMBusinessUnitDA[BusinessUnitVM.Prop.BusinessUnitID.Name].ToString(),
                            BusinessUnitDesc = m_drMBusinessUnitDA[BusinessUnitVM.Prop.BusinessUnitDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstBusinessUnitVM);
            return m_dicReturn;
        }

        #endregion
    }
}