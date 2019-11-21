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
    public class VendorSubcategoryController : BaseController
    {
        private readonly string title = "Vendor Subcategory";
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
            MVendorSubcategoryDA m_objMVendorSubcategoryDA = new MVendorSubcategoryDA();
            m_objMVendorSubcategoryDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMVendorSubcategory = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMVendorSubcategory.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = VendorSubcategoryVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMVendorSubcategoryDA = m_objMVendorSubcategoryDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpVendorSubcategoryBL in m_dicMVendorSubcategoryDA)
            {
                m_intCount = m_kvpVendorSubcategoryBL.Key;
                break;
            }

            List<VendorSubcategoryVM> m_lstVendorSubcategoryVM = new List<VendorSubcategoryVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(VendorSubcategoryVM.Prop.VendorSubcategoryID.MapAlias);
                m_lstSelect.Add(VendorSubcategoryVM.Prop.VendorSubcategoryDesc.MapAlias);
                m_lstSelect.Add(VendorSubcategoryVM.Prop.VendorCategoryDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(VendorSubcategoryVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMVendorSubcategoryDA = m_objMVendorSubcategoryDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMVendorSubcategoryDA.Message == string.Empty)
                {
                    m_lstVendorSubcategoryVM = (
                        from DataRow m_drMVendorSubcategoryDA in m_dicMVendorSubcategoryDA[0].Tables[0].Rows
                        select new VendorSubcategoryVM()
                        {
                            VendorSubcategoryID = m_drMVendorSubcategoryDA[VendorSubcategoryVM.Prop.VendorSubcategoryID.Name].ToString(),
                            VendorSubcategoryDesc = m_drMVendorSubcategoryDA[VendorSubcategoryVM.Prop.VendorSubcategoryDesc.Name].ToString(),
                            VendorCategoryDesc = m_drMVendorSubcategoryDA[VendorSubcategoryVM.Prop.VendorCategoryDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstVendorSubcategoryVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            MVendorSubcategoryDA m_objMVendorSubcategoryDA = new MVendorSubcategoryDA();
            m_objMVendorSubcategoryDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMVendorSubcategory = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMVendorSubcategory.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = VendorSubcategoryVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMVendorSubcategoryDA = m_objMVendorSubcategoryDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpVendorSubcategoryBL in m_dicMVendorSubcategoryDA)
            {
                m_intCount = m_kvpVendorSubcategoryBL.Key;
                break;
            }

            List<VendorSubcategoryVM> m_lstVendorSubcategoryVM = new List<VendorSubcategoryVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(VendorSubcategoryVM.Prop.VendorSubcategoryID.MapAlias);
                m_lstSelect.Add(VendorSubcategoryVM.Prop.VendorSubcategoryDesc.MapAlias);
                m_lstSelect.Add(VendorSubcategoryVM.Prop.VendorCategoryDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(VendorSubcategoryVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMVendorSubcategoryDA = m_objMVendorSubcategoryDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMVendorSubcategoryDA.Message == string.Empty)
                {
                    m_lstVendorSubcategoryVM = (
                        from DataRow m_drMVendorSubcategoryDA in m_dicMVendorSubcategoryDA[0].Tables[0].Rows
                        select new VendorSubcategoryVM()
                        {
                            VendorSubcategoryID = m_drMVendorSubcategoryDA[VendorSubcategoryVM.Prop.VendorSubcategoryID.Name].ToString(),
                            VendorSubcategoryDesc = m_drMVendorSubcategoryDA[VendorSubcategoryVM.Prop.VendorSubcategoryDesc.Name].ToString(),
                            VendorCategoryDesc = m_drMVendorSubcategoryDA[VendorSubcategoryVM.Prop.VendorCategoryDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstVendorSubcategoryVM, m_intCount);
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

            VendorSubcategoryVM m_objVendorSubcategoryVM = new VendorSubcategoryVM();
            ViewDataDictionary m_vddVendorSubcategory = new ViewDataDictionary();
            m_vddVendorSubcategory.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddVendorSubcategory.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
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
                Model = m_objVendorSubcategoryVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddVendorSubcategory,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Detail(string Caller, string Selected)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            VendorSubcategoryVM m_objVendorSubcategoryVM = new VendorSubcategoryVM();
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
                m_objVendorSubcategoryVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
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
                Model = m_objVendorSubcategoryVM,
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
            VendorSubcategoryVM m_objVendorSubcategoryVM = new VendorSubcategoryVM();
            if (m_dicSelectedRow.Count > 0)
                m_objVendorSubcategoryVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddVendorSubcategory = new ViewDataDictionary();
            m_vddVendorSubcategory.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddVendorSubcategory.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objVendorSubcategoryVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddVendorSubcategory,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<VendorSubcategoryVM> m_lstSelectedRow = new List<VendorSubcategoryVM>();
            m_lstSelectedRow = JSON.Deserialize<List<VendorSubcategoryVM>>(Selected);

            MVendorSubcategoryDA m_objMVendorSubcategoryDA = new MVendorSubcategoryDA();
            m_objMVendorSubcategoryDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (VendorSubcategoryVM m_objVendorSubcategoryVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifVendorSubcategoryVM = m_objVendorSubcategoryVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifVendorSubcategoryVM in m_arrPifVendorSubcategoryVM)
                    {
                        string m_strFieldName = m_pifVendorSubcategoryVM.Name;
                        object m_objFieldValue = m_pifVendorSubcategoryVM.GetValue(m_objVendorSubcategoryVM);
                        if (m_objVendorSubcategoryVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(VendorSubcategoryVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objMVendorSubcategoryDA.DeleteBC(m_objFilter, false);
                    if (m_objMVendorSubcategoryDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMVendorSubcategoryDA.Message);
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

        public ActionResult Browse(string ControlVendorSubcategoryID, string ControlVendorSubcategoryDesc, string ControlVendorCategoryDesc,
            string FilterVendorSubcategoryID = "", string FilterVendorSubcategoryDesc = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddVendorSubcategory = new ViewDataDictionary();
            m_vddVendorSubcategory.Add("Control" + VendorSubcategoryVM.Prop.VendorSubcategoryID.Name, ControlVendorSubcategoryID);
            m_vddVendorSubcategory.Add("Control" + VendorSubcategoryVM.Prop.VendorSubcategoryDesc.Name, ControlVendorSubcategoryDesc);
            m_vddVendorSubcategory.Add("Control" + VendorSubcategoryVM.Prop.VendorCategoryDesc.Name, ControlVendorCategoryDesc);
            m_vddVendorSubcategory.Add(VendorSubcategoryVM.Prop.VendorSubcategoryID.Name, FilterVendorSubcategoryID);
            m_vddVendorSubcategory.Add(VendorSubcategoryVM.Prop.VendorSubcategoryDesc.Name, FilterVendorSubcategoryDesc);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddVendorSubcategory,
                ViewName = "../VendorSubcategory/_Browse"
            };
        }

        public ActionResult Save(string Action)
        {
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MVendorSubcategoryDA m_objMVendorSubcategoryDA = new MVendorSubcategoryDA();
            m_objMVendorSubcategoryDA.ConnectionStringName = Global.ConnStrConfigName;
            try
            {
                string m_strVendorSubcategoryID = this.Request.Params[VendorSubcategoryVM.Prop.VendorSubcategoryID.Name];
                string m_strVendorSubcategoryDesc = this.Request.Params[VendorSubcategoryVM.Prop.VendorSubcategoryDesc.Name];
                string m_strVendorCategoryID = this.Request.Params[VendorCategoryVM.Prop.VendorCategoryID.Name];

                m_lstMessage = IsSaveValid(Action, m_strVendorSubcategoryID, m_strVendorSubcategoryDesc, m_strVendorCategoryID);
                if (m_lstMessage.Count <= 0)
                {
                    MVendorSubcategory m_objMVendorSubcategory = new MVendorSubcategory();
                    m_objMVendorSubcategory.VendorSubcategoryID = m_strVendorSubcategoryID;
                    m_objMVendorSubcategoryDA.Data = m_objMVendorSubcategory;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objMVendorSubcategoryDA.Select();

                    m_objMVendorSubcategory.VendorSubcategoryDesc = m_strVendorSubcategoryDesc;
                    m_objMVendorSubcategory.VendorCategoryID = m_strVendorCategoryID;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objMVendorSubcategoryDA.Insert(false);
                    else
                        m_objMVendorSubcategoryDA.Update(false);

                    if (!m_objMVendorSubcategoryDA.Success || m_objMVendorSubcategoryDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMVendorSubcategoryDA.Message);
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

        public ActionResult GetVendorSubcategory(string ControlVendorSubcategoryID, string ControlVendorSubcategoryDesc, string FilterVendorSubcategoryID, string FilterVendorSubcategoryDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<VendorSubcategoryVM>> m_dicVendorSubcategoryData = GetVendorSubcategoryData(true, FilterVendorSubcategoryID, FilterVendorSubcategoryDesc);
                KeyValuePair<int, List<VendorSubcategoryVM>> m_kvpVendorSubcategoryVM = m_dicVendorSubcategoryData.AsEnumerable().ToList()[0];
                if (m_kvpVendorSubcategoryVM.Key < 1 || (m_kvpVendorSubcategoryVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpVendorSubcategoryVM.Key > 1 && !Exact)
                    return Browse(ControlVendorSubcategoryID, ControlVendorSubcategoryDesc, "", FilterVendorSubcategoryID, FilterVendorSubcategoryDesc);

                m_dicVendorSubcategoryData = GetVendorSubcategoryData(false, FilterVendorSubcategoryID, FilterVendorSubcategoryDesc);
                VendorSubcategoryVM m_objVendorSubcategoryVM = m_dicVendorSubcategoryData[0][0];
                this.GetCmp<TextField>(ControlVendorSubcategoryID).Value = m_objVendorSubcategoryVM.VendorSubcategoryID;
                this.GetCmp<TextField>(ControlVendorSubcategoryDesc).Value = m_objVendorSubcategoryVM.VendorSubcategoryDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, string VendorSubcategoryID, string VendorSubcategoryDesc, string VendorCategoryID)
        {
            List<string> m_lstReturn = new List<string>();

            if (VendorSubcategoryID == string.Empty)
                m_lstReturn.Add(VendorSubcategoryVM.Prop.VendorSubcategoryID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (VendorSubcategoryDesc == string.Empty)
                m_lstReturn.Add(VendorSubcategoryVM.Prop.VendorSubcategoryDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (VendorCategoryID == string.Empty)
                m_lstReturn.Add(VendorSubcategoryVM.Prop.VendorCategoryID.Desc + " " + General.EnumDesc(MessageLib.mustFill));

            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(VendorSubcategoryVM.Prop.VendorSubcategoryID.Name, parameters[VendorSubcategoryVM.Prop.VendorSubcategoryID.Name]);
            m_dicReturn.Add(VendorSubcategoryVM.Prop.VendorSubcategoryDesc.Name, parameters[VendorSubcategoryVM.Prop.VendorSubcategoryDesc.Name]);
            m_dicReturn.Add(VendorSubcategoryVM.Prop.VendorCategoryID.Name, parameters[VendorSubcategoryVM.Prop.VendorCategoryID.Name]);
            m_dicReturn.Add(VendorSubcategoryVM.Prop.VendorCategoryDesc.Name, parameters[VendorSubcategoryVM.Prop.VendorCategoryDesc.Name]);

            return m_dicReturn;
        }

        private VendorSubcategoryVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            VendorSubcategoryVM m_objVendorSubcategoryVM = new VendorSubcategoryVM();
            MVendorSubcategoryDA m_objMVendorSubcategoryDA = new MVendorSubcategoryDA();
            m_objMVendorSubcategoryDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(VendorSubcategoryVM.Prop.VendorSubcategoryID.MapAlias);
            m_lstSelect.Add(VendorSubcategoryVM.Prop.VendorSubcategoryDesc.MapAlias);
            m_lstSelect.Add(VendorSubcategoryVM.Prop.VendorCategoryID.MapAlias);
            m_lstSelect.Add(VendorSubcategoryVM.Prop.VendorCategoryDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objVendorSubcategoryVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(VendorSubcategoryVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMVendorSubcategoryDA = m_objMVendorSubcategoryDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMVendorSubcategoryDA.Message == string.Empty)
            {
                DataRow m_drMVendorSubcategoryDA = m_dicMVendorSubcategoryDA[0].Tables[0].Rows[0];
                m_objVendorSubcategoryVM.VendorSubcategoryID = m_drMVendorSubcategoryDA[VendorSubcategoryVM.Prop.VendorSubcategoryID.Name].ToString();
                m_objVendorSubcategoryVM.VendorSubcategoryDesc = m_drMVendorSubcategoryDA[VendorSubcategoryVM.Prop.VendorSubcategoryDesc.Name].ToString();
                m_objVendorSubcategoryVM.VendorCategoryID = m_drMVendorSubcategoryDA[VendorCategoryVM.Prop.VendorCategoryID.Name].ToString();
                m_objVendorSubcategoryVM.VendorCategoryDesc = m_drMVendorSubcategoryDA[VendorCategoryVM.Prop.VendorCategoryDesc.Name].ToString();
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMVendorSubcategoryDA.Message;

            return m_objVendorSubcategoryVM;
        }

        #endregion

        #region Public Method

        public Dictionary<int, List<VendorSubcategoryVM>> GetVendorSubcategoryData(bool isCount, string VendorSubcategoryID, string VendorSubcategoryDesc)
        {
            int m_intCount = 0;
            List<VendorSubcategoryVM> m_lstVendorSubcategoryVM = new List<ViewModels.VendorSubcategoryVM>();
            Dictionary<int, List<VendorSubcategoryVM>> m_dicReturn = new Dictionary<int, List<VendorSubcategoryVM>>();
            MVendorSubcategoryDA m_objMVendorSubcategoryDA = new MVendorSubcategoryDA();
            m_objMVendorSubcategoryDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(VendorSubcategoryVM.Prop.VendorSubcategoryID.MapAlias);
            m_lstSelect.Add(VendorSubcategoryVM.Prop.VendorSubcategoryDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(VendorSubcategoryID);
            m_objFilter.Add(VendorSubcategoryVM.Prop.VendorSubcategoryID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(VendorSubcategoryDesc);
            m_objFilter.Add(VendorSubcategoryVM.Prop.VendorSubcategoryDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMVendorSubcategoryDA = m_objMVendorSubcategoryDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMVendorSubcategoryDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpVendorSubcategoryBL in m_dicMVendorSubcategoryDA)
                    {
                        m_intCount = m_kvpVendorSubcategoryBL.Key;
                        break;
                    }
                else
                {
                    m_lstVendorSubcategoryVM = (
                        from DataRow m_drMVendorSubcategoryDA in m_dicMVendorSubcategoryDA[0].Tables[0].Rows
                        select new VendorSubcategoryVM()
                        {
                            VendorSubcategoryID = m_drMVendorSubcategoryDA[VendorSubcategoryVM.Prop.VendorSubcategoryID.Name].ToString(),
                            VendorSubcategoryDesc = m_drMVendorSubcategoryDA[VendorSubcategoryVM.Prop.VendorSubcategoryDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstVendorSubcategoryVM);
            return m_dicReturn;
        }

        #endregion
    }
}