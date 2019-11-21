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
using System.Web.Script.Serialization;
using XMVC = Ext.Net.MVC;

namespace com.SML.BIGTRONS.Controllers
{
    public class VendorCategoryController : BaseController
    {
        private readonly string title = "Vendor Category";
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
            MVendorCategoryDA m_objMVendorCategoryDA = new MVendorCategoryDA();
            m_objMVendorCategoryDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMVendorCategory = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMVendorCategory.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = VendorCategoryVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMVendorCategoryDA = m_objMVendorCategoryDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpVendorCategoryBL in m_dicMVendorCategoryDA)
            {
                m_intCount = m_kvpVendorCategoryBL.Key;
                break;
            }

            List<VendorCategoryVM> m_lstVendorCategoryVM = new List<VendorCategoryVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(VendorCategoryVM.Prop.VendorCategoryID.MapAlias);
                m_lstSelect.Add(VendorCategoryVM.Prop.VendorCategoryDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(VendorCategoryVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMVendorCategoryDA = m_objMVendorCategoryDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMVendorCategoryDA.Message == string.Empty)
                {
                    m_lstVendorCategoryVM = (
                        from DataRow m_drMVendorCategoryDA in m_dicMVendorCategoryDA[0].Tables[0].Rows
                        select new VendorCategoryVM()
                        {
                            VendorCategoryID = m_drMVendorCategoryDA[VendorCategoryVM.Prop.VendorCategoryID.Name].ToString(),
                            VendorCategoryDesc = m_drMVendorCategoryDA[VendorCategoryVM.Prop.VendorCategoryDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstVendorCategoryVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            MVendorCategoryDA m_objMVendorCategoryDA = new MVendorCategoryDA();
            m_objMVendorCategoryDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMVendorCategory = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMVendorCategory.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = VendorCategoryVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMVendorCategoryDA = m_objMVendorCategoryDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpVendorCategoryBL in m_dicMVendorCategoryDA)
            {
                m_intCount = m_kvpVendorCategoryBL.Key;
                break;
            }

            List<VendorCategoryVM> m_lstVendorCategoryVM = new List<VendorCategoryVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(VendorCategoryVM.Prop.VendorCategoryID.MapAlias);
                m_lstSelect.Add(VendorCategoryVM.Prop.VendorCategoryDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(VendorCategoryVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMVendorCategoryDA = m_objMVendorCategoryDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMVendorCategoryDA.Message == string.Empty)
                {
                    m_lstVendorCategoryVM = (
                        from DataRow m_drMVendorCategoryDA in m_dicMVendorCategoryDA[0].Tables[0].Rows
                        select new VendorCategoryVM()
                        {
                            VendorCategoryID = m_drMVendorCategoryDA[VendorCategoryVM.Prop.VendorCategoryID.Name].ToString(),
                            VendorCategoryDesc = m_drMVendorCategoryDA[VendorCategoryVM.Prop.VendorCategoryDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstVendorCategoryVM, m_intCount);
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

            VendorCategoryVM m_objVendorCategoryVM = new VendorCategoryVM();
            ViewDataDictionary m_vddVendorCategory = new ViewDataDictionary();
            m_vddVendorCategory.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddVendorCategory.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
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
                Model = m_objVendorCategoryVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddVendorCategory,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Detail(string Caller, string Selected)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            VendorCategoryVM m_objVendorCategoryVM = new VendorCategoryVM();
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
                m_objVendorCategoryVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
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
                Model = m_objVendorCategoryVM,
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
            VendorCategoryVM m_objVendorCategoryVM = new VendorCategoryVM();
            if (m_dicSelectedRow.Count > 0)
                m_objVendorCategoryVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddVendorCategory = new ViewDataDictionary();
            m_vddVendorCategory.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddVendorCategory.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objVendorCategoryVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddVendorCategory,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<VendorCategoryVM> m_lstSelectedRow = new List<VendorCategoryVM>();
            m_lstSelectedRow = JSON.Deserialize<List<VendorCategoryVM>>(Selected);

            MVendorCategoryDA m_objMVendorCategoryDA = new MVendorCategoryDA();
            m_objMVendorCategoryDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (VendorCategoryVM m_objVendorCategoryVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifVendorCategoryVM = m_objVendorCategoryVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifVendorCategoryVM in m_arrPifVendorCategoryVM)
                    {
                        string m_strFieldName = m_pifVendorCategoryVM.Name;
                        object m_objFieldValue = m_pifVendorCategoryVM.GetValue(m_objVendorCategoryVM);
                        if (m_objVendorCategoryVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(VendorCategoryVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objMVendorCategoryDA.DeleteBC(m_objFilter, false);
                    if (m_objMVendorCategoryDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMVendorCategoryDA.Message);
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

        public ActionResult Browse(string ControlVendorCategoryID, string ControlVendorCategoryDesc, string FilterVendorCategoryID = "", string FilterVendorCategoryDesc = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddVendorCategory = new ViewDataDictionary();
            m_vddVendorCategory.Add("Control" + VendorCategoryVM.Prop.VendorCategoryID.Name, ControlVendorCategoryID);
            m_vddVendorCategory.Add("Control" + VendorCategoryVM.Prop.VendorCategoryDesc.Name, ControlVendorCategoryDesc);
            m_vddVendorCategory.Add(VendorCategoryVM.Prop.VendorCategoryID.Name, FilterVendorCategoryID);
            m_vddVendorCategory.Add(VendorCategoryVM.Prop.VendorCategoryDesc.Name, FilterVendorCategoryDesc);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddVendorCategory,
                ViewName = "../VendorCategory/_Browse"
            };
        }

        public ActionResult Save(string Action)
        {
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MVendorCategoryDA m_objMVendorCategoryDA = new MVendorCategoryDA();
            m_objMVendorCategoryDA.ConnectionStringName = Global.ConnStrConfigName;
            try
            {
                string m_strVendorCategoryID = this.Request.Params[VendorCategoryVM.Prop.VendorCategoryID.Name];
                string m_strVendorCategoryDesc = this.Request.Params[VendorCategoryVM.Prop.VendorCategoryDesc.Name];

                m_lstMessage = IsSaveValid(Action, m_strVendorCategoryID, m_strVendorCategoryDesc);
                if (m_lstMessage.Count <= 0)
                {
                    MVendorCategory m_objMVendorCategory = new MVendorCategory();
                    m_objMVendorCategory.VendorCategoryID = m_strVendorCategoryID;
                    m_objMVendorCategoryDA.Data = m_objMVendorCategory;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objMVendorCategoryDA.Select();

                    m_objMVendorCategory.VendorCategoryDesc = m_strVendorCategoryDesc;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objMVendorCategoryDA.Insert(false);
                    else
                        m_objMVendorCategoryDA.Update(false);

                    if (!m_objMVendorCategoryDA.Success || m_objMVendorCategoryDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMVendorCategoryDA.Message);
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

        public ActionResult GetVendorCategory(string ControlVendorCategoryID, string ControlVendorCategoryDesc, string FilterVendorCategoryID, string FilterVendorCategoryDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<VendorCategoryVM>> m_dicVendorCategoryData = GetVendorCategoryData(true, FilterVendorCategoryID, FilterVendorCategoryDesc);
                KeyValuePair<int, List<VendorCategoryVM>> m_kvpVendorCategoryVM = m_dicVendorCategoryData.AsEnumerable().ToList()[0];
                if (m_kvpVendorCategoryVM.Key < 1 || (m_kvpVendorCategoryVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpVendorCategoryVM.Key > 1 && !Exact)
                    return Browse(ControlVendorCategoryID, ControlVendorCategoryDesc, FilterVendorCategoryID, FilterVendorCategoryDesc);

                m_dicVendorCategoryData = GetVendorCategoryData(false, FilterVendorCategoryID, FilterVendorCategoryDesc);
                VendorCategoryVM m_objVendorCategoryVM = m_dicVendorCategoryData[0][0];
                this.GetCmp<TextField>(ControlVendorCategoryID).Value = m_objVendorCategoryVM.VendorCategoryID;
                this.GetCmp<TextField>(ControlVendorCategoryDesc).Value = m_objVendorCategoryVM.VendorCategoryDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, string VendorCategoryID, string VendorCategoryDesc)
        {
            List<string> m_lstReturn = new List<string>();

            if (VendorCategoryID == string.Empty)
                m_lstReturn.Add(VendorCategoryVM.Prop.VendorCategoryID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (VendorCategoryDesc == string.Empty)
                m_lstReturn.Add(VendorCategoryVM.Prop.VendorCategoryDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));

            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(VendorCategoryVM.Prop.VendorCategoryID.Name, parameters[VendorCategoryVM.Prop.VendorCategoryID.Name]);
            m_dicReturn.Add(VendorCategoryVM.Prop.VendorCategoryDesc.Name, parameters[VendorCategoryVM.Prop.VendorCategoryDesc.Name]);

            return m_dicReturn;
        }

        private VendorCategoryVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            VendorCategoryVM m_objVendorCategoryVM = new VendorCategoryVM();
            MVendorCategoryDA m_objMVendorCategoryDA = new MVendorCategoryDA();
            m_objMVendorCategoryDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(VendorCategoryVM.Prop.VendorCategoryID.MapAlias);
            m_lstSelect.Add(VendorCategoryVM.Prop.VendorCategoryDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objVendorCategoryVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(VendorCategoryVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMVendorCategoryDA = m_objMVendorCategoryDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMVendorCategoryDA.Message == string.Empty)
            {
                DataRow m_drMVendorCategoryDA = m_dicMVendorCategoryDA[0].Tables[0].Rows[0];
                m_objVendorCategoryVM.VendorCategoryID = m_drMVendorCategoryDA[VendorCategoryVM.Prop.VendorCategoryID.Name].ToString();
                m_objVendorCategoryVM.VendorCategoryDesc = m_drMVendorCategoryDA[VendorCategoryVM.Prop.VendorCategoryDesc.Name].ToString();
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMVendorCategoryDA.Message;

            return m_objVendorCategoryVM;
        }

        #endregion

        #region Public Method

        public Dictionary<int, List<VendorCategoryVM>> GetVendorCategoryData(bool isCount, string VendorCategoryID, string VendorCategoryDesc)
        {
            int m_intCount = 0;
            List<VendorCategoryVM> m_lstVendorCategoryVM = new List<ViewModels.VendorCategoryVM>();
            Dictionary<int, List<VendorCategoryVM>> m_dicReturn = new Dictionary<int, List<VendorCategoryVM>>();
            MVendorCategoryDA m_objMVendorCategoryDA = new MVendorCategoryDA();
            m_objMVendorCategoryDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(VendorCategoryVM.Prop.VendorCategoryID.MapAlias);
            m_lstSelect.Add(VendorCategoryVM.Prop.VendorCategoryDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(VendorCategoryID);
            m_objFilter.Add(VendorCategoryVM.Prop.VendorCategoryID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(VendorCategoryDesc);
            m_objFilter.Add(VendorCategoryVM.Prop.VendorCategoryDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMVendorCategoryDA = m_objMVendorCategoryDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMVendorCategoryDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpVendorCategoryBL in m_dicMVendorCategoryDA)
                    {
                        m_intCount = m_kvpVendorCategoryBL.Key;
                        break;
                    }
                else
                {
                    m_lstVendorCategoryVM = (
                        from DataRow m_drMVendorCategoryDA in m_dicMVendorCategoryDA[0].Tables[0].Rows
                        select new VendorCategoryVM()
                        {
                            VendorCategoryID = m_drMVendorCategoryDA[VendorCategoryVM.Prop.VendorCategoryID.Name].ToString(),
                            VendorCategoryDesc = m_drMVendorCategoryDA[VendorCategoryVM.Prop.VendorCategoryDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstVendorCategoryVM);
            return m_dicReturn;
        }

        #endregion
    }
}