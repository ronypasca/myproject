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
    public class VendorController : BaseController
    {
        private readonly string title = "Vendor";
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
            MVendorDA m_objMVendorDA = new MVendorDA();
            m_objMVendorDA.ConnectionStringName = Global.ConnStrConfigName;

            string m_strMessage = string.Empty;
            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;


            VendorVM m_objVendorVM = new VendorVM();
            m_objVendorVM.ListPICVendor = new List<VendorPICsVM>();
            
            FilterHeaderConditions m_fhcMVendor = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMVendor.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = VendorVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMVendorDA = m_objMVendorDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpVendorBL in m_dicMVendorDA)
            {
                m_intCount = m_kvpVendorBL.Key;
                break;
            }

            List<VendorVM> m_lstVendorVM = new List<VendorVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(VendorVM.Prop.VendorID.MapAlias);
                m_lstSelect.Add(VendorVM.Prop.VendorDesc.MapAlias);
                m_lstSelect.Add(VendorVM.Prop.VendorCategoryDesc.MapAlias);
                m_lstSelect.Add(VendorVM.Prop.VendorSubcategoryDesc.MapAlias);
                m_lstSelect.Add(VendorVM.Prop.Address.MapAlias);
                m_lstSelect.Add(VendorVM.Prop.Phone.MapAlias);
                m_lstSelect.Add(VendorVM.Prop.Email.MapAlias);
                
                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(VendorVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMVendorDA = m_objMVendorDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMVendorDA.Message == string.Empty)
                {
                    m_lstVendorVM = (
                        from DataRow m_drMVendorDA in m_dicMVendorDA[0].Tables[0].Rows
                        select new VendorVM()
                        {
                            VendorID = m_drMVendorDA[VendorVM.Prop.VendorID.Name].ToString(),
                            VendorDesc = m_drMVendorDA[VendorVM.Prop.VendorDesc.Name].ToString(),
                            VendorCategoryDesc = m_drMVendorDA[VendorVM.Prop.VendorCategoryDesc.Name].ToString(),
                            VendorSubcategoryDesc = m_drMVendorDA[VendorVM.Prop.VendorSubcategoryDesc.Name].ToString(),
                            Address = m_drMVendorDA[VendorVM.Prop.Address.Name].ToString(),
                            Phone = m_drMVendorDA[VendorVM.Prop.Phone.Name].ToString(),
                            Email = m_drMVendorDA[VendorVM.Prop.Email.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstVendorVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            MVendorDA m_objMVendorDA = new MVendorDA();
            m_objMVendorDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMVendor = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMVendor.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = VendorVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMVendorDA = m_objMVendorDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpVendorBL in m_dicMVendorDA)
            {
                m_intCount = m_kvpVendorBL.Key;
                break;
            }

            List<VendorVM> m_lstVendorVM = new List<VendorVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(VendorVM.Prop.VendorID.MapAlias);
                m_lstSelect.Add(VendorVM.Prop.VendorDesc.MapAlias);
                m_lstSelect.Add(VendorVM.Prop.VendorCategoryDesc.MapAlias);
                m_lstSelect.Add(VendorVM.Prop.VendorSubcategoryDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(VendorVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMVendorDA = m_objMVendorDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMVendorDA.Message == string.Empty)
                {
                    m_lstVendorVM = (
                        from DataRow m_drMVendorDA in m_dicMVendorDA[0].Tables[0].Rows
                        select new VendorVM()
                        {
                            VendorID = m_drMVendorDA[VendorVM.Prop.VendorID.Name].ToString(),
                            VendorDesc = m_drMVendorDA[VendorVM.Prop.VendorDesc.Name].ToString(),
                            VendorCategoryDesc = m_drMVendorDA[VendorVM.Prop.VendorCategoryDesc.Name].ToString(),
                            VendorSubcategoryDesc = m_drMVendorDA[VendorVM.Prop.VendorSubcategoryDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstVendorVM, m_intCount);
        }

        public ActionResult ReadBrowseVendorPIC(StoreRequestParameters parameters)
        {
            DVendorPICsDA m_objDVendorPICsDA = new DVendorPICsDA();
            m_objDVendorPICsDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMVendor = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMVendor.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = VendorPICsVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicDVendorPICsDA = m_objDVendorPICsDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpVendorPICBL in m_dicDVendorPICsDA)
            {
                m_intCount = m_kvpVendorPICBL.Key;
                break;
            }

            List<VendorPICsVM> m_lstVendorPICsVM = new List<VendorPICsVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(VendorPICsVM.Prop.VendorPICID.MapAlias);
                m_lstSelect.Add(VendorPICsVM.Prop.PICName.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(VendorPICsVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicDVendorPICsDA = m_objDVendorPICsDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objDVendorPICsDA.Message == string.Empty)
                {
                    m_lstVendorPICsVM = (
                        from DataRow m_drMVendorPICsDA in m_dicDVendorPICsDA[0].Tables[0].Rows
                        select new VendorPICsVM()
                        {
                            VendorPICID = m_drMVendorPICsDA[VendorPICsVM.Prop.VendorPICID.Name].ToString(),
                            PICName = m_drMVendorPICsDA[VendorPICsVM.Prop.PICName.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstVendorPICsVM, m_intCount);
        }
        
        public ActionResult ReadBrowseCommunicationType(StoreRequestParameters parameters)
        {
            MCommunicationTypesDA m_objMCommunicationTypesDA = new MCommunicationTypesDA();
            m_objMCommunicationTypesDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcm_objMCommunicationTypesDA = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcm_objMCommunicationTypesDA.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = CommunicationTypesVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMCommunicationTypesDA = m_objMCommunicationTypesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpCommTypePICBL in m_dicMCommunicationTypesDA)
            {
                m_intCount = m_kvpCommTypePICBL.Key;
                break;
            }

            List<CommunicationTypesVM> m_lstCommunicationTypesVM = new List<CommunicationTypesVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(CommunicationTypesVM.Prop.CommunicationTypeID.MapAlias);
                m_lstSelect.Add(CommunicationTypesVM.Prop.CommTypeDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(CommunicationTypesVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMCommunicationTypesDA = m_objMCommunicationTypesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMCommunicationTypesDA.Message == string.Empty)
                {
                    m_lstCommunicationTypesVM = (
                        from DataRow m_drMCommunicationTypesDA in m_dicMCommunicationTypesDA[0].Tables[0].Rows
                        select new CommunicationTypesVM()
                        {
                            CommunicationTypeID = m_drMCommunicationTypesDA[CommunicationTypesVM.Prop.CommunicationTypeID.Name].ToString(),
                            CommTypeDesc = m_drMCommunicationTypesDA[CommunicationTypesVM.Prop.CommTypeDesc.Name].ToString(),
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstCommunicationTypesVM, m_intCount);
        }
        
        public ActionResult Home()
        {
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 0;
            return this.Direct();
        }

        public ActionResult Add(string Caller, string VendorID, string VendorPICID)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Add)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            VendorVM m_objVendorVM = new VendorVM();
            m_objVendorVM.ListPICVendor = new List<VendorPICsVM>();

            VendorCommunicationsVM m_objVendorCommunicationsVM = new VendorCommunicationsVM();
            m_objVendorCommunicationsVM.ListCommunicationTypes = new List<CommunicationTypesVM>();

            ViewDataDictionary m_vddVendor = new ViewDataDictionary();
            m_vddVendor.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddVendor.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
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
                Model = m_objVendorVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddVendor,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Detail(string Caller, string Selected)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            VendorVM m_objVendorVM = new VendorVM();
            VendorPICsVM m_objVendorPICsVM = new VendorPICsVM();
            VendorCommunicationsVM m_objVendorCommunicationsVM = new VendorCommunicationsVM();
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
            {
                m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
            }
            else if (Caller == General.EnumDesc(Buttons.ButtonSave))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                m_dicSelectedRow = GetFormData(m_nvcParams);
            }

            if (m_dicSelectedRow.Count > 0)
                m_objVendorVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }
            
            m_objVendorVM.ListPICVendor = new List<VendorPICsVM>();
            m_objVendorPICsVM.ListVendorCommunication = new List<VendorCommunicationsVM>();
            if (m_objVendorVM != null)
                m_objVendorVM.ListPICVendor = GetListVendorPIC(m_objVendorVM.VendorID, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }
            
            if (m_objVendorPICsVM != null)
                m_objVendorPICsVM.ListVendorCommunication = GetListVendorComm("", ref m_strMessage, m_objVendorVM.VendorID);
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
                Model = m_objVendorVM,
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

            string message = "";
            string m_strMessage = string.Empty;
            VendorVM m_objVendorVM = new VendorVM();
            VendorPICsVM m_objVendorPICsVM = new VendorPICsVM();
            if (m_dicSelectedRow.Count > 0)
                m_objVendorVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }
            
            m_objVendorVM.ListPICVendor = new List<VendorPICsVM>();
            if (m_objVendorVM != null)
                m_objVendorVM.ListPICVendor = GetListVendorPIC(m_objVendorVM.VendorID, ref message);
            
            ViewDataDictionary m_vddVendor = new ViewDataDictionary();
            m_vddVendor.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddVendor.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objVendorVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddVendor,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        
        private ConfigVM GetVendorConfig()
        {

            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ConfigVM.Prop.Desc1.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc2.MapAlias);

            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(VendorPICsVM.Prop.VendorPICID.Name);
            m_objFilter.Add(ConfigVM.Prop.Key1.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(VendorPICsVM.Prop.PICName.Name);
            m_objFilter.Add(ConfigVM.Prop.Key2.Map, m_lstFilter);


            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("VendorPICs");
            m_objFilter.Add(ConfigVM.Prop.Key3.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicUConfigDA = m_objUConfigDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            ConfigVM m_objConfigVM = new ConfigVM();
            if (m_objUConfigDA.Message == string.Empty)
            {
                DataRow m_drUConfigDA = m_dicUConfigDA[0].Tables[0].Rows[0];
                m_objConfigVM.Desc1 = m_drUConfigDA[ConfigVM.Prop.Desc1.Name].ToString();
                m_objConfigVM.Desc2 = m_drUConfigDA[ConfigVM.Prop.Desc2.Name].ToString();
            }
            return m_objConfigVM;

        }

        public ActionResult DeleteVendorPIC(string Selected)
        {
            string m_strVendorPICID = string.Empty;
            ConfigVM m_objConfigVM = GetVendorConfig();

            List<VendorPICsVM> m_lstSelectedRow = new List<VendorPICsVM>();
            m_lstSelectedRow = JSON.Deserialize<List<VendorPICsVM>>(Selected);

            DVendorPICsDA m_objDVendorPICsDA = new DVendorPICsDA();
            m_objDVendorPICsDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (VendorPICsVM m_objDVendorPICsVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifVendorPICsVM = m_objDVendorPICsVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifVendorPICsVM in m_arrPifVendorPICsVM)
                    {
                        string m_strFieldName = m_pifVendorPICsVM.Name;
                        object m_objFieldValue = m_pifVendorPICsVM.GetValue(m_objDVendorPICsVM) ?? string.Empty;
                        if (m_objDVendorPICsVM.IsKey(m_strFieldName))
                        {
                            if (m_strFieldName.Equals(VendorPICsVM.Prop.VendorID.Name))
                            {
                                m_strVendorPICID = (m_objConfigVM.Desc1 == m_objFieldValue.ToString() ? "" : m_objFieldValue.ToString());
                                m_objFieldValue = m_strVendorPICID;
                            }

                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(VendorPICsVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objDVendorPICsDA.DeleteBC(m_objFilter, false);
                    if (m_objDVendorPICsDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDVendorPICsDA.Message);
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                return this.Direct(false, ex.Message);

            }
            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfoAlert("PICs Vendor", General.EnumDesc(MessageLib.Deleted));
                return this.Direct(true, string.Empty);
            }
            else
                return this.Direct();

        }
        
        #region Vendor Communication
        public ActionResult DeleteVendorCommunications(string Selected)
        {
            string m_strVendorPICID = string.Empty;
            ConfigVM m_objConfigVM = GetVendorConfig();

            List<VendorCommunicationsVM> m_lstSelectedRow = new List<VendorCommunicationsVM>();
            m_lstSelectedRow = JSON.Deserialize<List<VendorCommunicationsVM>>(Selected);

            DVendorCommunicationsDA m_objDVendorCommunicationsDA = new DVendorCommunicationsDA();
            m_objDVendorCommunicationsDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (VendorCommunicationsVM m_objDVendorCommunicationsVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifVendorCommunicationsVM = m_objDVendorCommunicationsVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifVendorPICVM in m_arrPifVendorCommunicationsVM)
                    {
                        string m_strFieldName = m_pifVendorPICVM.Name;
                        object m_objFieldValue = m_pifVendorPICVM.GetValue(m_objDVendorCommunicationsVM) ?? string.Empty;
                        if (m_objDVendorCommunicationsVM.IsKey(m_strFieldName))
                        {
                            if (m_strFieldName.Equals(VendorPICsVM.Prop.VendorPICID.Name))
                            {
                                m_strVendorPICID = (m_objConfigVM.Desc1 == m_objFieldValue.ToString() ? "" : m_objFieldValue.ToString());
                                m_objFieldValue = m_strVendorPICID;
                            }

                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(VendorCommunicationsVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objDVendorCommunicationsDA.DeleteBC(m_objFilter, false);
                    if (m_objDVendorCommunicationsDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDVendorCommunicationsDA.Message);
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                return this.Direct(false, ex.Message);
            }
            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfoAlert("Item Price Vendor Period", General.EnumDesc(MessageLib.Deleted));
                return this.Direct(true, string.Empty);
            }
            else
                return this.Direct();
        }
        #endregion
        
        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<VendorVM> m_lstSelectedRow = new List<VendorVM>();
            m_lstSelectedRow = JSON.Deserialize<List<VendorVM>>(Selected);
     
            MVendorDA m_objMVendorDA = new MVendorDA();
            DVendorPICsDA d_objDVendorPICsDA = new DVendorPICsDA();

            List<VendorCommunicationsVM> d_lstVendorComm = new List<VendorCommunicationsVM>();
            DVendorCommunicationsDA d_objDVendorCommunicationDA = new DVendorCommunicationsDA();
            d_objDVendorCommunicationDA.ConnectionStringName = Global.ConnStrConfigName;

            m_objMVendorDA.ConnectionStringName = Global.ConnStrConfigName;
            d_objDVendorPICsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstMessage = new List<string>();
            string m_strMessage = string.Empty;

            try
            {
                foreach (VendorVM m_objVendorVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifVendorVM = m_objVendorVM.GetType().GetProperties();

                    string m_VendorID = m_objVendorVM.VendorID;
                    foreach (PropertyInfo m_pifVendorVM in m_arrPifVendorVM)
                    {
                        string m_strFieldName = m_pifVendorVM.Name;
                        object m_objFieldValue = m_pifVendorVM.GetValue(m_objVendorVM);

                        if (m_objVendorVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(VendorVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    List<string> m_lstSelect = new List<string>();
                    m_lstSelect.Add(VendorPICsVM.Prop.VendorPICID.MapAlias);
                    
                    Dictionary<int, DataSet> d_dicDVendorPICsDA = d_objDVendorPICsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
                    List<VendorPICsVM> d_listVendorPICsVM = new List<VendorPICsVM>();
                    if (d_objDVendorPICsDA.Success)
                    {
                        d_listVendorPICsVM = (
                        from DataRow d_drDVendorPICsDA in d_dicDVendorPICsDA[0].Tables[0].Rows
                        select new VendorPICsVM()
                        {
                            VendorPICID = d_drDVendorPICsDA[VendorPICsVM.Prop.VendorPICID.Name].ToString(),

                        }).Distinct().ToList();
                    }
                    
                    List<string> m_lstSelectVenCom = new List<string>();
                    m_lstSelectVenCom.Add(VendorCommunicationsVM.Prop.VendorPICID.MapAlias);

                    Dictionary<int, DataSet> m_dicCommType = d_objDVendorCommunicationDA.SelectBC(0, null, false, m_lstSelectVenCom, m_objFilter, null, null, null);
                    if (d_objDVendorCommunicationDA.Success)
                    {
                        d_lstVendorComm =
                        (from DataRow m_dra in m_dicCommType[0].Tables[0].Rows
                         select new VendorCommunicationsVM()
                         {
                             VendorPICID = m_dra[VendorCommunicationsVM.Prop.VendorPICID.Name].ToString(),
                         }).ToList();
                    }

                    foreach (VendorCommunicationsVM d_VenCom in d_lstVendorComm)
                    {
                        m_objFilter = new Dictionary<string, List<object>>();
                        List<object> m_lstFilter = new List<object>();
                        m_lstFilter.Add(Operator.Equals);
                        m_lstFilter.Add(d_VenCom.VendorPICID);
                        m_objFilter.Add(VendorCommunicationsVM.Prop.VendorPICID.Map, m_lstFilter);

                        d_objDVendorCommunicationDA.DeleteBC(m_objFilter, false);
                    }


                    m_objMVendorDA.DeleteBC(m_objFilter, false);
                    if (m_objMVendorDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMVendorDA.Message);
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

        public ActionResult Browse(string ControlVendorID, string ControlVendorDesc, string FilterVendorID = "", string FilterVendorDesc = "", string ControlGrid = "")
        {
            ViewDataDictionary m_vddVendor = new ViewDataDictionary();
            m_vddVendor.Add("Control" + VendorVM.Prop.VendorID.Name, ControlVendorID);
            m_vddVendor.Add("Control" + VendorVM.Prop.VendorDesc.Name, ControlVendorDesc);
            m_vddVendor.Add(VendorVM.Prop.VendorID.Name, FilterVendorID);
            m_vddVendor.Add(VendorVM.Prop.VendorDesc.Name, FilterVendorDesc);
            m_vddVendor.Add("ControlGrid", ControlGrid);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddVendor,
                ViewName = "../Vendor/_Browse"
            };
        }

        public ActionResult BrowsePIC(string ControlVendorPICID, string ControlPICName, string ControlGrid = "", string FilterVendorPICID = "", string FilterPICName = "")
        {
            ViewDataDictionary m_vddVendorPIC = new ViewDataDictionary();
            m_vddVendorPIC.Add("Control" + VendorPICsVM.Prop.VendorPICID.Name, ControlVendorPICID);
            m_vddVendorPIC.Add("Control" + VendorPICsVM.Prop.PICName.Name, ControlPICName);
            m_vddVendorPIC.Add(VendorPICsVM.Prop.VendorPICID.Name, FilterVendorPICID);
            m_vddVendorPIC.Add(VendorPICsVM.Prop.PICName.Name, FilterPICName);
            m_vddVendorPIC.Add("ControlGrid", ControlGrid);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddVendorPIC,
                ViewName = "../Vendor/_BrowsePIC"
            };
        }

        public ActionResult BrowseCOMType(string ControlCommunicationTypeID, string ControlCommTypeDesc, string ControlGridComm = "", string FilterCommunicationTypeID = "", string FilterCommDesc = "")
        {
            ViewDataDictionary m_vddCommType = new ViewDataDictionary();
            
            m_vddCommType.Add("Control" + CommunicationTypesVM.Prop.CommunicationTypeID.Name, ControlCommunicationTypeID);
            m_vddCommType.Add("Control" + CommunicationTypesVM.Prop.CommTypeDesc.Name, ControlCommTypeDesc);
            m_vddCommType.Add(CommunicationTypesVM.Prop.CommunicationTypeID.Name, FilterCommunicationTypeID);
            m_vddCommType.Add(CommunicationTypesVM.Prop.CommTypeDesc.Name, FilterCommDesc);

            m_vddCommType.Add("ControlGridComm", ControlGridComm);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddCommType,
                ViewName = "../Vendor/_BrowseCommType"
            };
        }

        public ActionResult Save(string Action)
        {
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            VendorVM m_objVendorVM = new VendorVM();
            MVendor m_objMVendor = new MVendor();
            MVendorDA m_objMVendorDA = new MVendorDA();
            DVendorPICsDA d_objDVendorPICsDA = new DVendorPICsDA();
            DVendorCommunicationsDA d_objDVendorCommunicationsDA = new DVendorCommunicationsDA();
            DVendorPICs d_objDVendorPICs = new DVendorPICs();
            DVendorCommunications d_objDVendorCommunications = new DVendorCommunications();

            d_objDVendorPICsDA.ConnectionStringName = Global.ConnStrConfigName;
            d_objDVendorCommunicationsDA.ConnectionStringName = Global.ConnStrConfigName;
            m_objMVendorDA.ConnectionStringName = Global.ConnStrConfigName;

            string m_strMessage = string.Empty;
            string m_strTransName = "Vendor";
            object m_objDBConnection = null;
            m_objDBConnection = m_objMVendorDA.BeginTrans(m_strTransName);

            try
            {
                string m_strStreet, m_strPostal, m_strPhone, m_strEmail, m_strIDNo, m_strNPWP, m_strCity, m_strVendorPICID;
                string m_strVendorID = this.Request.Params[VendorVM.Prop.VendorID.Name];
                string m_strVendorSubcategoryID = this.Request.Params[VendorVM.Prop.VendorSubcategoryID.Name];
                string m_strFirstName = this.Request.Params[VendorVM.Prop.FirstName.Name];
                string m_strLastName = this.Request.Params[VendorVM.Prop.LastName.Name];

                if (this.Request.Params[VendorVM.Prop.City.Name] == null)
                { m_strCity = "-"; }
                else
                { m_strCity = this.Request.Params[VendorVM.Prop.City.Name]; }

                if (this.Request.Params[VendorVM.Prop.Street.Name] == null)
                { m_strStreet = "-"; }
                else
                { m_strStreet = this.Request.Params[VendorVM.Prop.Street.Name]; }

                if (this.Request.Params[VendorVM.Prop.Postal.Name] == null)
                { m_strPostal = "-"; }
                else
                { m_strPostal = this.Request.Params[VendorVM.Prop.Postal.Name]; }

                if (this.Request.Params[VendorVM.Prop.Phone.Name] == null)
                { m_strPhone = "-"; }
                else
                { m_strPhone = this.Request.Params[VendorVM.Prop.Phone.Name]; }

                if (this.Request.Params[VendorVM.Prop.Email.Name] == null)
                { m_strEmail = "-"; }
                else
                { m_strEmail = this.Request.Params[VendorVM.Prop.Email.Name]; }

                if (this.Request.Params[VendorVM.Prop.IDNo.Name] == null)
                { m_strIDNo = "-"; }
                else
                { m_strIDNo = this.Request.Params[VendorVM.Prop.IDNo.Name]; }

                if (this.Request.Params[VendorVM.Prop.NPWP.Name] == null)
                { m_strNPWP = "-"; }
                else
                { m_strNPWP = this.Request.Params[VendorVM.Prop.NPWP.Name]; }

                string m_strlstVendorCommunicationVM = this.Request.Params[VendorPICsVM.Prop.ListVendorCommunication.Name];
                List<VendorCommunicationsVM> lst_VendorCommunicationsVM = JSON.Deserialize<List<VendorCommunicationsVM>>(m_strlstVendorCommunicationVM);

                string m_strlstPICVendorVM = this.Request.Params[VendorVM.Prop.ListPICVendor.Name];
                List<VendorPICsVM> lst_VendorPICs = JSON.Deserialize<List<VendorPICsVM>>(m_strlstPICVendorVM);
                
                m_objVendorVM.VendorID = m_strVendorID;
                m_objVendorVM.FirstName = m_strFirstName;
                m_objVendorVM.LastName = m_strLastName;
                m_objVendorVM.City = m_strCity;
                m_objVendorVM.Street = m_strStreet;
                m_objVendorVM.Postal = m_strPostal;
                m_objVendorVM.Phone = m_strPhone;
                m_objVendorVM.Email = m_strEmail;
                m_objVendorVM.IDNo = m_strIDNo;
                m_objVendorVM.NPWP = m_strNPWP;
                m_objVendorVM.VendorSubcategoryID = m_strVendorSubcategoryID;
                m_objVendorVM.ListPICVendor = lst_VendorPICs;
                
                m_lstMessage = IsSaveValid(Action, m_strVendorID, m_strVendorSubcategoryID, m_strFirstName,
                    m_strLastName, m_strCity, m_strStreet, m_strPostal, m_strPhone, m_strEmail, m_strIDNo, m_strNPWP, m_objVendorVM);
                if (m_lstMessage.Count <= 0)
                {
                    List<string> success_status = new List<string>();
                    
                    m_objMVendor.VendorID = m_objVendorVM.VendorID;
                    m_objMVendorDA.Data = m_objMVendor;
                    
                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objMVendorDA.Select();

                    m_objMVendor.FirstName = m_objVendorVM.FirstName;
                    m_objMVendor.LastName = m_objVendorVM.LastName;
                    m_objMVendor.City = m_objVendorVM.City;
                    m_objMVendor.Street = m_objVendorVM.Street;
                    m_objMVendor.Postal = m_objVendorVM.Postal;
                    m_objMVendor.Phone = m_objVendorVM.Phone;
                    m_objMVendor.Email = m_objVendorVM.Email;
                    m_objMVendor.IDNo = m_objVendorVM.IDNo;
                    m_objMVendor.NPWP = m_objVendorVM.NPWP;
                    m_objMVendor.VendorSubcategoryID = m_objVendorVM.VendorSubcategoryID;
                    
                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objMVendorDA.Insert(true, m_objDBConnection);
                    else
                        m_objMVendorDA.Update(true, m_objDBConnection);

                    if (!m_objMVendorDA.Success || m_objMVendorDA.Message != string.Empty)
                    {
                        m_objMVendorDA.EndTrans(ref m_objDBConnection,false, m_strTransName);
                        return this.Direct(false, m_objMVendorDA.Message);
                    }

                    if (lst_VendorPICs != null)
                    {
                        if (Action == General.EnumDesc(Buttons.ButtonUpdate))
                        {
                            List<string> m_lstSelect = new List<string>();
                            m_lstSelect.Add(VendorPICsVM.Prop.VendorPICID.MapAlias);

                            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_strVendorID);
                            m_objFilter.Add(VendorPICsVM.Prop.VendorID.Map, m_lstFilter);

                            Dictionary<int, DataSet> d_dicDVendorPICsDA = d_objDVendorPICsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
                            List<VendorPICsVM> d_listVendorPICsVM = new List<VendorPICsVM>();
                            if (d_objDVendorPICsDA.Success)
                            {
                                d_listVendorPICsVM = (
                                from DataRow d_drDVendorPICsDA in d_dicDVendorPICsDA[0].Tables[0].Rows
                                select new VendorPICsVM()
                                {
                                    VendorPICID = d_drDVendorPICsDA[VendorPICsVM.Prop.VendorPICID.Name].ToString(),

                                }).Distinct().ToList();
                            }

                            List<VendorPICsVM> m_delVendorPICsVM = d_listVendorPICsVM.ToList(); 

                            foreach (VendorCommunicationsVM d_VenCom in lst_VendorCommunicationsVM)
                            {
                                m_objFilter = new Dictionary<string, List<object>>();
                                m_lstFilter = new List<object>();
                                m_lstFilter.Add(Operator.Equals);
                                m_lstFilter.Add(d_VenCom.VendorPICID);
                                m_objFilter.Add(VendorCommunicationsVM.Prop.VendorPICID.Map, m_lstFilter);

                                d_objDVendorCommunicationsDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                            }

                            foreach (VendorPICsVM d_VendorPICsVM in m_delVendorPICsVM)
                            {
                                m_objFilter = new Dictionary<string, List<object>>();
                                m_lstFilter = new List<object>();
                                m_lstFilter.Add(Operator.Equals);
                                m_lstFilter.Add(d_VendorPICsVM.VendorPICID);
                                m_objFilter.Add(VendorPICsVM.Prop.VendorPICID.Map, m_lstFilter);

                                d_objDVendorPICsDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                            }
                        }

                        foreach (VendorPICsVM lstVendorPIC in lst_VendorPICs)
                        {
                            d_objDVendorPICs.VendorID = m_strVendorID;
                            d_objDVendorPICs.VendorPICID = lstVendorPIC.VendorPICID;
                            d_objDVendorPICs.PICName = lstVendorPIC.PICName;
                            d_objDVendorPICsDA.Data = d_objDVendorPICs;

                            d_objDVendorPICsDA.Insert(true, m_objDBConnection);
                            
                            m_strVendorPICID = d_objDVendorPICs.VendorPICID;

                            if (!d_objDVendorPICsDA.Success || d_objDVendorPICsDA.Message != string.Empty)
                            {
                                d_objDVendorPICsDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                                return this.Direct(false, d_objDVendorPICsDA.Message);
                            }
                        }
                    }

                    if (lst_VendorCommunicationsVM != null)
                    {
                        foreach (VendorCommunicationsVM objVendorCommunicationsVM in lst_VendorCommunicationsVM)
                        {
                            d_objDVendorCommunications.VendorCommID = objVendorCommunicationsVM.VendorCommID;
                            d_objDVendorCommunications.CommunicationTypeID = objVendorCommunicationsVM.CommunicationTypeID;
                            d_objDVendorCommunications.IsDefault = objVendorCommunicationsVM.IsDefault;
                            d_objDVendorCommunications.VendorPICID = objVendorCommunicationsVM.VendorPICID;
                            d_objDVendorCommunications.CommDesc = objVendorCommunicationsVM.CommDesc;

                            d_objDVendorCommunications.VendorCommID = Guid.NewGuid().ToString("N").ToUpper();

                            d_objDVendorCommunicationsDA.Data = d_objDVendorCommunications;
                            d_objDVendorCommunicationsDA.Insert(true, m_objDBConnection);

                            if (!d_objDVendorCommunicationsDA.Success)
                            {
                                d_objDVendorCommunicationsDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                                return this.Direct(false, d_objDVendorCommunicationsDA.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                m_objMVendorDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                m_lstMessage.Add(ex.Message);
            }
            if (m_lstMessage.Count <= 0)
            {
                if (m_objMVendorDA.Success && m_objMVendorDA.Message == string.Empty)
                    m_objMVendorDA.EndTrans(ref m_objDBConnection, true, m_strTransName);

                Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
                return Detail(General.EnumDesc(Buttons.ButtonSave), null);
            }
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);
        }
        
        #endregion

        #region Direct Method

        public ActionResult GetVendor(string ControlVendorID, string ControlVendorDesc, string FilterVendorID, string FilterVendorDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<VendorVM>> m_dicVendorData = GetVendorData(true, FilterVendorID, FilterVendorDesc);
                KeyValuePair<int, List<VendorVM>> m_kvpVendorVM = m_dicVendorData.AsEnumerable().ToList()[0];
                if (m_kvpVendorVM.Key < 1 || (m_kvpVendorVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpVendorVM.Key > 1 && !Exact)
                    return Browse(ControlVendorID, ControlVendorDesc, FilterVendorID, FilterVendorDesc);

                m_dicVendorData = GetVendorData(false, FilterVendorID, FilterVendorDesc);
                VendorVM m_objVendorVM = m_dicVendorData[0][0];
                this.GetCmp<TextField>(ControlVendorID).Value = m_objVendorVM.VendorID;
                this.GetCmp<TextField>(ControlVendorDesc).Value = m_objVendorVM.VendorDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method
        
        private List<VendorPICsVM> GetListVendorPIC(string VendorID, ref string message)
        {
            List<VendorPICsVM> m_lstVendorPICVM = new List<VendorPICsVM>();
            DVendorPICsDA m_objDVendorPICDA = new DVendorPICsDA();
            m_objDVendorPICDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(VendorPICsVM.Prop.VendorPICID.MapAlias);
            m_lstSelect.Add(VendorPICsVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(VendorPICsVM.Prop.PICName.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(VendorID);
            m_objFilter.Add(VendorPICsVM.Prop.VendorID.Map, m_lstFilter);

            string m_message = message;

            Dictionary<int, DataSet> m_dicDVendorPICsDA = m_objDVendorPICDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDVendorPICDA.Success)
            {
                m_lstVendorPICVM = (
               from DataRow m_drm_dicDVendorPICsDA in m_dicDVendorPICsDA[0].Tables[0].Rows
               select new VendorPICsVM()
               {
                    VendorID = m_drm_dicDVendorPICsDA[VendorPICsVM.Prop.VendorID.Name].ToString(),
                    VendorPICID = m_drm_dicDVendorPICsDA[VendorPICsVM.Prop.VendorPICID.Name].ToString(),
                    PICName = m_drm_dicDVendorPICsDA[VendorPICsVM.Prop.PICName.Name].ToString(),
                    ListVendorCommunication = GetListVendorComm(m_drm_dicDVendorPICsDA[VendorPICsVM.Prop.VendorPICID.Name].ToString(), ref m_message, ""),
                    
                }).Distinct().ToList();
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDVendorPICDA.Message;

            return m_lstVendorPICVM;

        }

        private List<VendorCommunicationsVM> GetListVendorComm(string VendorPICID, ref string message, string VendorID = "")
        {

            List<VendorCommunicationsVM> m_lstVendorComm = new List<VendorCommunicationsVM>();
            DVendorCommunicationsDA m_objDVendorCommunication = new DVendorCommunicationsDA();
            m_objDVendorCommunication.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(VendorCommunicationsVM.Prop.VendorCommID.MapAlias);
            m_lstSelect.Add(VendorCommunicationsVM.Prop.CommunicationTypeID.MapAlias);
            m_lstSelect.Add(VendorCommunicationsVM.Prop.VendorPICID.MapAlias);
            m_lstSelect.Add(VendorCommunicationsVM.Prop.IsDefault.MapAlias);
            m_lstSelect.Add(VendorCommunicationsVM.Prop.CommDesc.MapAlias);
            m_lstSelect.Add(VendorCommunicationsVM.Prop.CommTypeDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();

            string m_strMessage = string.Empty;
            string m_message = message;

            if (VendorPICID != "")
            {
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(VendorPICID);
                m_objFilter.Add(VendorCommunicationsVM.Prop.VendorPICID.Map, m_lstFilter);
            }  

            if (VendorID != "")
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(VendorID);
                m_objFilter.Add(VendorPICsVM.Prop.VendorID.Map, m_lstFilter);
            }

            Dictionary<int, DataSet> m_dicDVendorCommunicationsDA = m_objDVendorCommunication.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDVendorCommunication.Success)
            {
                m_lstVendorComm = (
                  from DataRow m_drm_dicVendorComm in m_dicDVendorCommunicationsDA[0].Tables[0].Rows
                  select new VendorCommunicationsVM()
                  {
                    VendorCommID = m_drm_dicVendorComm[VendorCommunicationsVM.Prop.VendorCommID.Name].ToString(),
                    CommunicationTypeID = m_drm_dicVendorComm[VendorCommunicationsVM.Prop.CommunicationTypeID.Name].ToString(),
                    VendorPICID = m_drm_dicVendorComm[VendorCommunicationsVM.Prop.VendorPICID.Name].ToString(),
                    CommDesc = m_drm_dicVendorComm[VendorCommunicationsVM.Prop.CommDesc.Name].ToString(),
                    CommTypeDesc = m_drm_dicVendorComm[VendorCommunicationsVM.Prop.CommTypeDesc.Name].ToString(),
                    IsDefault = bool.Parse(m_drm_dicVendorComm[VendorCommunicationsVM.Prop.IsDefault.Name].ToString()),
                    ListCommunicationTypes = GetListCommType(m_drm_dicVendorComm[VendorCommunicationsVM.Prop.CommunicationTypeID.Name].ToString(), ref m_message),
                  }).Distinct().ToList();
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDVendorCommunication.Message;

            return m_lstVendorComm;

        }

        private List<CommunicationTypesVM> GetListCommType(string CommTypeID, ref string message)
        {
            List<CommunicationTypesVM> m_lstCommunicationTypesVM = new List<CommunicationTypesVM>();
            MCommunicationTypesDA m_objMCommunicationTypesDA = new MCommunicationTypesDA();
            m_objMCommunicationTypesDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(CommunicationTypesVM.Prop.CommunicationTypeID.MapAlias);
            m_lstSelect.Add(CommunicationTypesVM.Prop.CommTypeDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(CommTypeID);
            m_objFilter.Add(CommunicationTypesVM.Prop.CommunicationTypeID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMCommunicationTypesDA = m_objMCommunicationTypesDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMCommunicationTypesDA.Success)
            {
                m_lstCommunicationTypesVM = (
                from DataRow m_objMCommunicationTypes in m_dicMCommunicationTypesDA[0].Tables[0].Rows
                select new CommunicationTypesVM()
                {
                    CommunicationTypeID = m_objMCommunicationTypes[CommunicationTypesVM.Prop.CommunicationTypeID.Name].ToString(),
                    CommTypeDesc = m_objMCommunicationTypes[CommunicationTypesVM.Prop.CommTypeDesc.Name].ToString(),

                }).Distinct().ToList();
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMCommunicationTypesDA.Message;


            return m_lstCommunicationTypesVM;
        }

        public ActionResult GetCommType(StoreRequestParameters parameters)
        {

            List<CommunicationTypesVM> lst_CommType = new List<CommunicationTypesVM>();
            MCommunicationTypesDA m_CommTypeDA = new MCommunicationTypesDA();
            m_CommTypeDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(CommunicationTypesVM.Prop.CommunicationTypeID.MapAlias);
            m_lstSelect.Add(CommunicationTypesVM.Prop.CommTypeDesc.MapAlias);

            Dictionary<int, DataSet> m_dicCommType = m_CommTypeDA.SelectBC(0, null, false, m_lstSelect, null, null, null, null);
            if (m_CommTypeDA.Success)
            {
                lst_CommType =
                (from DataRow m_dra in m_dicCommType[0].Tables[0].Rows
                 select new CommunicationTypesVM()
                 {
                     CommunicationTypeID = m_dra[CommunicationTypesVM.Prop.CommunicationTypeID.Name].ToString(),
                     CommTypeDesc = m_dra[CommunicationTypesVM.Prop.CommTypeDesc.Name].ToString()
                 }).ToList();
            }

            return this.Store(lst_CommType);
        }


        private List<string> IsSaveValid(string Action, string VendorID, string VendorSubcategoryID, string FirstName,
            string LastName, string City, string Street, string Postal, string Phone, string Email, string IDNo,
            string NPWP, VendorVM objVendorVM)//, VendorPICsVM objVendorPICsVM )
        {
            List<string> m_lstReturn = new List<string>();

            if (VendorID == string.Empty)
                m_lstReturn.Add(VendorVM.Prop.VendorID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (FirstName == string.Empty)
                m_lstReturn.Add(VendorVM.Prop.FirstName.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (LastName == string.Empty)
                m_lstReturn.Add(VendorVM.Prop.LastName.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (VendorSubcategoryID == string.Empty)
                m_lstReturn.Add(VendorVM.Prop.VendorSubcategoryDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (City == string.Empty)
                m_lstReturn.Add(VendorVM.Prop.City.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (Street == string.Empty)
                m_lstReturn.Add(VendorVM.Prop.Street.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (Postal == string.Empty)
                m_lstReturn.Add(VendorVM.Prop.Postal.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (Phone == string.Empty)
                m_lstReturn.Add(VendorVM.Prop.Phone.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (Email == string.Empty)
                m_lstReturn.Add(VendorVM.Prop.Email.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (IDNo == string.Empty)
                m_lstReturn.Add(VendorVM.Prop.IDNo.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (NPWP == string.Empty)
                m_lstReturn.Add(VendorVM.Prop.NPWP.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            
            if (!objVendorVM.ListPICVendor.Any())
                m_lstReturn.Add(VendorVM.Prop.ListPICVendor.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            else
                foreach (VendorPICsVM _objVendorPICsVM in objVendorVM.ListPICVendor)
                {
                    if (string.IsNullOrEmpty(_objVendorPICsVM.VendorPICID))
                        m_lstReturn.Add(VendorPICsVM.Prop.VendorPICID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
                    if (string.IsNullOrEmpty(_objVendorPICsVM.PICName))
                        m_lstReturn.Add("Please Fill Vendor PIC Name" );
                    return m_lstReturn;
                }
                            
            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(VendorVM.Prop.VendorID.Name, parameters[VendorVM.Prop.VendorID.Name]);
            m_dicReturn.Add(VendorVM.Prop.VendorDesc.Name, parameters[VendorVM.Prop.VendorDesc.Name]);
            m_dicReturn.Add(VendorVM.Prop.VendorSubcategoryID.Name, parameters[VendorVM.Prop.VendorSubcategoryID.Name]);
            m_dicReturn.Add(VendorVM.Prop.VendorSubcategoryDesc.Name, parameters[VendorVM.Prop.VendorSubcategoryDesc.Name]);

            return m_dicReturn;
        }

        private VendorVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            VendorVM m_objVendorVM = new VendorVM();
            MVendorDA m_objMVendorDA = new MVendorDA();
            m_objMVendorDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(VendorVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(VendorVM.Prop.LastName.MapAlias);
            m_lstSelect.Add(VendorVM.Prop.FirstName.MapAlias);
            m_lstSelect.Add(VendorVM.Prop.VendorCategoryDesc.MapAlias);
            m_lstSelect.Add(VendorVM.Prop.VendorSubcategoryID.MapAlias);
            m_lstSelect.Add(VendorVM.Prop.VendorSubcategoryDesc.MapAlias);
            m_lstSelect.Add(VendorVM.Prop.City.MapAlias);
            m_lstSelect.Add(VendorVM.Prop.Street.MapAlias);
            m_lstSelect.Add(VendorVM.Prop.Postal.MapAlias);
            m_lstSelect.Add(VendorVM.Prop.Phone.MapAlias);
            m_lstSelect.Add(VendorVM.Prop.Email.MapAlias);
            m_lstSelect.Add(VendorVM.Prop.IDNo.MapAlias);
            m_lstSelect.Add(VendorVM.Prop.NPWP.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objVendorVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(VendorVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMVendorDA = m_objMVendorDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMVendorDA.Message == string.Empty)
            {
                DataRow m_drMVendorDA = m_dicMVendorDA[0].Tables[0].Rows[0];
                m_objVendorVM.VendorID = m_drMVendorDA[VendorVM.Prop.VendorID.Name].ToString();
                m_objVendorVM.LastName = m_drMVendorDA[VendorVM.Prop.LastName.Name].ToString();
                m_objVendorVM.FirstName = m_drMVendorDA[VendorVM.Prop.FirstName.Name].ToString();
                m_objVendorVM.VendorCategoryDesc = m_drMVendorDA[VendorVM.Prop.VendorCategoryDesc.Name].ToString();
                m_objVendorVM.VendorSubcategoryID = m_drMVendorDA[VendorSubcategoryVM.Prop.VendorSubcategoryID.Name].ToString();
                m_objVendorVM.VendorSubcategoryDesc = m_drMVendorDA[VendorSubcategoryVM.Prop.VendorSubcategoryDesc.Name].ToString();
                m_objVendorVM.City = m_drMVendorDA[VendorVM.Prop.City.Name].ToString();
                m_objVendorVM.Street = m_drMVendorDA[VendorVM.Prop.Street.Name].ToString();
                m_objVendorVM.Postal = m_drMVendorDA[VendorVM.Prop.Postal.Name].ToString();
                m_objVendorVM.Phone = m_drMVendorDA[VendorVM.Prop.Phone.Name].ToString();
                m_objVendorVM.Email = m_drMVendorDA[VendorVM.Prop.Email.Name].ToString();
                m_objVendorVM.IDNo = m_drMVendorDA[VendorVM.Prop.IDNo.Name].ToString();
                m_objVendorVM.NPWP = m_drMVendorDA[VendorVM.Prop.NPWP.Name].ToString();

                m_objVendorVM.ListPICVendor = GetListVendorPIC(m_objVendorVM.VendorID, ref message);

            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMVendorDA.Message;

            return m_objVendorVM;
        }
        
        #endregion

        #region Public Method

        public Dictionary<int, List<VendorVM>> GetVendorData(bool isCount, string VendorID, string VendorDesc)
        {
            int m_intCount = 0;
            List<VendorVM> m_lstVendorVM = new List<ViewModels.VendorVM>();
            Dictionary<int, List<VendorVM>> m_dicReturn = new Dictionary<int, List<VendorVM>>();
            MVendorDA m_objMVendorDA = new MVendorDA();
            m_objMVendorDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(VendorVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(VendorVM.Prop.VendorDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(VendorID);
            m_objFilter.Add(VendorVM.Prop.VendorID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(VendorDesc);
            m_objFilter.Add(VendorVM.Prop.VendorDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMVendorDA = m_objMVendorDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMVendorDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpVendorBL in m_dicMVendorDA)
                    {
                        m_intCount = m_kvpVendorBL.Key;
                        break;
                    }
                else
                {
                    m_lstVendorVM = (
                        from DataRow m_drMVendorDA in m_dicMVendorDA[0].Tables[0].Rows
                        select new VendorVM()
                        {
                            VendorID = m_drMVendorDA[VendorVM.Prop.VendorID.Name].ToString(),
                            VendorDesc = m_drMVendorDA[VendorVM.Prop.VendorDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstVendorVM);
            return m_dicReturn;
        }


        public ActionResult GetCommType()
        {
            List<CommunicationTypesVM> lst_CommType = new List<CommunicationTypesVM>();
            MCommunicationTypesDA m_CommTypeDA = new MCommunicationTypesDA();
            m_CommTypeDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(CommunicationTypesVM.Prop.CommunicationTypeID.MapAlias);

            Dictionary<int, DataSet> m_dicCommType = m_CommTypeDA.SelectBC(0, null, false, m_lstSelect, null, null, null, null);
            if (m_CommTypeDA.Success)
            {
                lst_CommType =
                (from DataRow m_dra in m_dicCommType[0].Tables[0].Rows
                 select new CommunicationTypesVM()
                 {
                     CommunicationTypeID = m_dra[CommunicationTypesVM.Prop.CommunicationTypeID.Name].ToString()
                 }).ToList();
            }

            return this.Store(lst_CommType);
        }

        #endregion
    }
}