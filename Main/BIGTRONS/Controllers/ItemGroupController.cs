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
    public class ItemGroupController : BaseController
    {
        private readonly string title = "ItemGroup";
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
            MItemGroupDA m_objMItemGroupDA = new MItemGroupDA();
            m_objMItemGroupDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMItemGroup = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMItemGroup.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = ItemGroupVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMItemGroupDA = m_objMItemGroupDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpItemGroupBL in m_dicMItemGroupDA)
            {
                m_intCount = m_kvpItemGroupBL.Key;
                break;
            }

            List<ItemGroupVM> m_lstItemGroupVM = new List<ItemGroupVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(ItemGroupVM.Prop.ItemGroupID.MapAlias);
                m_lstSelect.Add(ItemGroupVM.Prop.ItemGroupDesc.MapAlias);
                m_lstSelect.Add(ItemGroupVM.Prop.ItemTypeDesc.MapAlias);
                m_lstSelect.Add(ItemGroupVM.Prop.HasParameter.MapAlias);
                m_lstSelect.Add(ItemGroupVM.Prop.HasPrice.MapAlias);


                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(ItemGroupVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMItemGroupDA = m_objMItemGroupDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMItemGroupDA.Message == string.Empty)
                {
                    m_lstItemGroupVM = (
                        from DataRow m_drMItemGroupDA in m_dicMItemGroupDA[0].Tables[0].Rows
                        select new ItemGroupVM()
                        {
                            ItemGroupID = m_drMItemGroupDA[ItemGroupVM.Prop.ItemGroupID.Name].ToString(),
                            ItemGroupDesc = m_drMItemGroupDA[ItemGroupVM.Prop.ItemGroupDesc.Name].ToString(),
                            ItemTypeDesc = m_drMItemGroupDA[ItemGroupVM.Prop.ItemTypeDesc.Name].ToString(),
                            HasParameter = Convert.ToBoolean(m_drMItemGroupDA[ItemGroupVM.Prop.HasParameter.Name].ToString()),
                            HasPrice = Convert.ToBoolean(m_drMItemGroupDA[ItemGroupVM.Prop.HasPrice.Name].ToString())
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstItemGroupVM, m_intCount);
        }
        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            MItemGroupDA m_objMItemGroupDA = new MItemGroupDA();
            m_objMItemGroupDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMItemGroup = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMItemGroup.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = ItemGroupVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMItemGroupDA = m_objMItemGroupDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpItemGroupBL in m_dicMItemGroupDA)
            {
                m_intCount = m_kvpItemGroupBL.Key;
                break;
            }

            List<ItemGroupVM> m_lstItemGroupVM = new List<ItemGroupVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(ItemGroupVM.Prop.ItemGroupID.MapAlias);
                m_lstSelect.Add(ItemGroupVM.Prop.ItemGroupDesc.MapAlias);
                m_lstSelect.Add(ItemGroupVM.Prop.ItemTypeDesc.MapAlias);
                m_lstSelect.Add(ItemGroupVM.Prop.HasParameter.MapAlias);
                m_lstSelect.Add(ItemGroupVM.Prop.HasPrice.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(ItemGroupVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMItemGroupDA = m_objMItemGroupDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMItemGroupDA.Message == string.Empty)
                {
                    m_lstItemGroupVM = (
                        from DataRow m_drMItemGroupDA in m_dicMItemGroupDA[0].Tables[0].Rows
                        select new ItemGroupVM()
                        {
                            ItemGroupID = m_drMItemGroupDA[ItemGroupVM.Prop.ItemGroupID.Name].ToString(),
                            ItemGroupDesc = m_drMItemGroupDA[ItemGroupVM.Prop.ItemGroupDesc.Name].ToString(),
                            ItemTypeDesc = m_drMItemGroupDA[ItemGroupVM.Prop.ItemTypeDesc.Name].ToString(),
                            HasParameter = Convert.ToBoolean(m_drMItemGroupDA[ItemGroupVM.Prop.HasParameter.Name].ToString()),
                            HasPrice = Convert.ToBoolean(m_drMItemGroupDA[ItemGroupVM.Prop.HasPrice.Name].ToString())
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstItemGroupVM, m_intCount);
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
            

            ItemGroupVM m_objItemGroupVM = new ItemGroupVM();
            m_objItemGroupVM.ListItemGroupParameterVM = new List<ItemGroupParameterVM>();

            ViewDataDictionary m_vddItemGroup = new ViewDataDictionary();
            m_vddItemGroup.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddItemGroup.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
           
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            if (Caller == General.EnumDesc(Buttons.ButtonDetail))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                Dictionary<string, object> m_dicSelectedRow = GetFormData(m_nvcParams);
                Session[dataSessionName] = JSON.Serialize(m_dicSelectedRow);
            }
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objItemGroupVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddItemGroup,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult Detail(string Caller, string Selected)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();
            string m_strMessage = string.Empty;
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
            else if (Caller == General.EnumDesc(Buttons.ButtonAdd) || Caller == General.EnumDesc(Buttons.ButtonUpdate))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                m_dicSelectedRow = GetFormData(m_nvcParams);
            }

            
            ItemGroupVM m_objItemGroupVM = new ItemGroupVM();
            if (m_dicSelectedRow.Count > 0)
                m_objItemGroupVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
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
                Model = m_objItemGroupVM,
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
            if (Caller == General.EnumDesc(Buttons.ButtonList) )
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
            ItemGroupVM m_objItemGroupVM = new ItemGroupVM();
            if (m_dicSelectedRow.Count > 0)
                m_objItemGroupVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddItemGroup = new ViewDataDictionary();
            m_vddItemGroup.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddItemGroup.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objItemGroupVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddItemGroup,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<ItemGroupVM> m_lstSelectedRow = new List<ItemGroupVM>();
            m_lstSelectedRow = JSON.Deserialize<List<ItemGroupVM>>(Selected);

            MItemGroupDA m_objMItemGroupDA = new MItemGroupDA();
            DItemGroupParameterDA d_objDItemGroupParameter = new DItemGroupParameterDA();
            d_objDItemGroupParameter.ConnectionStringName = Global.ConnStrConfigName;
            m_objMItemGroupDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();
            object m_objDBConnection = null;
            string m_strTransName = "DeleteItemGroup";
            try
            {
                foreach (ItemGroupVM m_objItemGroupVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifItemGroupVM = m_objItemGroupVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifItemGroupVM in m_arrPifItemGroupVM)
                    {
                        string m_strFieldName = m_pifItemGroupVM.Name;
                        object m_objFieldValue = m_pifItemGroupVM.GetValue(m_objItemGroupVM);
                        if (m_objItemGroupVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(ItemGroupVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }


                    m_objDBConnection = m_objMItemGroupDA.BeginTrans(m_strTransName);

                    m_objMItemGroupDA.DeleteBC(m_objFilter, true, m_objDBConnection);

                    Dictionary<string, List<object>> m_objFilterParameter = new Dictionary<string, List<object>>();
                    List<object> m_lstFilterParameter = new List<object>();
                    m_lstFilterParameter.Add(Operator.Equals);
                    m_lstFilterParameter.Add(m_objItemGroupVM.ItemGroupID);

                    m_objFilterParameter.Add(ItemGroupParameterVM.Prop.Map(ItemGroupParameterVM.Prop.ItemGroupID.Name, false), m_lstFilterParameter);
                    d_objDItemGroupParameter.DeleteBC(m_objFilterParameter, true, m_objDBConnection);

                    if (m_objMItemGroupDA.Message != string.Empty && d_objDItemGroupParameter.Message != string.Empty)
                    {
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMItemGroupDA.Message);
                        m_objMItemGroupDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                    }
                    else
                        m_objMItemGroupDA.EndTrans(ref m_objDBConnection, true, m_strTransName);

                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                m_objMItemGroupDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
            }
            if (m_lstMessage.Count <= 0) {                 
                Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Deleted));
                return this.Direct(true, string.Empty);
            }
            else
                return this.Direct(false, String.Join(Global.NewLineSeparated, m_lstMessage));
                //Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));


            //return this.Direct();
        }
        public ActionResult DeleteParameter(string ItemGroupID, string ParameterDesc, string ParameterID)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            //List<ItemGroupVM> m_lstSelectedRow = new List<ItemGroupVM>();
            //m_lstSelectedRow = JSON.Deserialize<List<ItemGroupVM>>(Selected);
            bool success = false;
            if (!string.IsNullOrEmpty(ItemGroupID))
            {
                Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, "Can't delete existing Parameter"));
                success = false;
            }
            else
            {
                Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Deleted));
                success = true;
            }

            return this.Direct(success);
        }
        public ActionResult Browse(string ControlItemGroupID, string ControlItemGroupDesc, string ControlItemTypeDesc, string ControlHasParameter, string ControlHasPrice, string FilterItemGroupID = "", string FilterItemGroupDesc = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddItemGroup = new ViewDataDictionary();
            m_vddItemGroup.Add("Control" + ItemGroupVM.Prop.ItemGroupID.Name, ControlItemGroupID);
            m_vddItemGroup.Add("Control" + ItemGroupVM.Prop.ItemGroupDesc.Name, ControlItemGroupDesc);
            m_vddItemGroup.Add("Control" + ItemGroupVM.Prop.ItemTypeDesc.Name, ControlItemTypeDesc);
            m_vddItemGroup.Add("Control" + ItemGroupVM.Prop.HasParameter.Name, ControlHasParameter);
            m_vddItemGroup.Add("Control" + ItemGroupVM.Prop.HasPrice.Name, ControlHasPrice);
            m_vddItemGroup.Add(ItemGroupVM.Prop.ItemGroupID.Name, FilterItemGroupID);
            m_vddItemGroup.Add(ItemGroupVM.Prop.ItemGroupDesc.Name, FilterItemGroupDesc);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddItemGroup,
                ViewName = "../ItemGroup/_Browse"
            };
        }
        public ActionResult Save(string Action)
        {
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MItemGroupDA m_objMItemGroupDA = new MItemGroupDA();
            DItemGroupParameter d_objDItemGroupParameter = new DItemGroupParameter();
            DItemGroupParameterDA d_objDItemGroupParameterDA = new DItemGroupParameterDA();
            MItemGroup m_objMItemGroup = new MItemGroup();
            d_objDItemGroupParameterDA.ConnectionStringName = Global.ConnStrConfigName;
            m_objMItemGroupDA.ConnectionStringName = Global.ConnStrConfigName;
            object m_objDBConnection = null;
            string m_strTransName = "SaveItemGroup";
            try
            {
                string m_strItemGroupID = this.Request.Params[ItemGroupVM.Prop.ItemGroupID.Name];
                string m_strItemGroupDesc = this.Request.Params[ItemGroupVM.Prop.ItemGroupDesc.Name];
                string m_strItemTypeID = this.Request.Params[ItemTypeVM.Prop.ItemTypeID.Name];
                string m_strlstItemGroupParameterVM = this.Request.Params[ItemGroupVM.Prop.ListItemGroupParameterVM.Name];
                bool m_strHasParam = Convert.ToBoolean(this.Request.Params[ItemGroupVM.Prop.HasParameter.Name]);
                bool m_strHasPrice = Convert.ToBoolean(this.Request.Params[ItemGroupVM.Prop.HasPrice.Name]);
                List<ItemGroupParameterVM> lst_Parameter = JSON.Deserialize<List<ItemGroupParameterVM>>(m_strlstItemGroupParameterVM);

                m_lstMessage = IsSaveValid(Action, m_strItemGroupID, m_strItemGroupDesc, m_strItemTypeID, lst_Parameter);
                if (m_lstMessage.Count <= 0)
                {
                    List<string> success_status = new List<string>();

                    m_objDBConnection = m_objMItemGroupDA.BeginTrans(m_strTransName);

                    m_objMItemGroup.ItemGroupID = m_strItemGroupID;
                    m_objMItemGroupDA.Data = m_objMItemGroup;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objMItemGroupDA.Select();

                    m_objMItemGroup.ItemGroupDesc = m_strItemGroupDesc;
                    m_objMItemGroup.ItemTypeID = m_strItemTypeID;
                    m_objMItemGroup.HasParameter = m_strHasParam;
                    m_objMItemGroup.HasPrice = m_strHasPrice;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                    {
                        m_objMItemGroupDA.Insert(true, m_objDBConnection);
                        if (string.Empty == m_objMItemGroupDA.Message)
                        {
                            foreach (ItemGroupParameterVM eachlst in lst_Parameter)
                            {
                                d_objDItemGroupParameter.ItemGroupID = m_strItemGroupID;
                                d_objDItemGroupParameter.ParameterID = eachlst.ParameterID;
                                d_objDItemGroupParameterDA.Data = d_objDItemGroupParameter;

                                d_objDItemGroupParameterDA.Insert(true, m_objDBConnection);
                                if (!d_objDItemGroupParameterDA.Success || d_objDItemGroupParameterDA.Message.Any())
                                {
                                    m_lstMessage.Add(d_objDItemGroupParameterDA.Message);
                                    break;
                                }
                            }
                        }
                        else
                            m_lstMessage.Add(m_objMItemGroupDA.Message);
                    }
                    else
                    {
                        m_objMItemGroupDA.Update(true, m_objDBConnection);
                        if (string.Empty == m_objMItemGroupDA.Message)
                        {
                            foreach (ItemGroupParameterVM eachlst in lst_Parameter)
                            {
                                if (eachlst.ItemGroupID == null)
                                {
                                    d_objDItemGroupParameter.ItemGroupID = m_strItemGroupID;
                                    d_objDItemGroupParameter.ParameterID = eachlst.ParameterID;
                                    d_objDItemGroupParameterDA.Data = d_objDItemGroupParameter;

                                    d_objDItemGroupParameterDA.Insert(true, m_objDBConnection);
                                    if (!d_objDItemGroupParameterDA.Success || d_objDItemGroupParameterDA.Message.Any())
                                    {
                                        m_lstMessage.Add(d_objDItemGroupParameterDA.Message);
                                        break;
                                    }
                                }
                            }
                        }
                        else
                            m_lstMessage.Add(d_objDItemGroupParameterDA.Message);
                    }
                    if (!m_objMItemGroupDA.Success || m_objMItemGroupDA.Message != string.Empty || m_lstMessage.Any())
                    {
                        m_lstMessage.Add(m_objMItemGroupDA.Message);
                        m_objMItemGroupDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                    }
                    else
                        m_objMItemGroupDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
                }
            }
            catch (Exception ex)
            {
                m_objMItemGroupDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                m_lstMessage.Add(ex.Message);
            }
            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
                return Detail(Action, null);
            }
            else
            {
                Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
                return this.Direct(true);
            }
        }

        #endregion

        #region Direct Method

        public ActionResult GetItemGroup(string ControlItemGroupID, string ControlItemGroupDesc,string ControlItemTypeDesc,string ControlHasParameter, string ControlHasPrice, string FilterItemGroupID, string FilterItemGroupDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<ItemGroupVM>> m_dicItemGroupData = GetItemGroupData(true, FilterItemGroupID, FilterItemGroupDesc);
                KeyValuePair<int, List<ItemGroupVM>> m_kvpItemGroupVM = m_dicItemGroupData.AsEnumerable().ToList()[0];
                if (m_kvpItemGroupVM.Key < 1 || (m_kvpItemGroupVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpItemGroupVM.Key > 1 && !Exact)
                    return Browse(ControlItemGroupID, ControlItemGroupDesc,ControlItemTypeDesc,ControlHasParameter,ControlHasPrice, FilterItemGroupID, FilterItemGroupDesc);

                m_dicItemGroupData = GetItemGroupData(false, FilterItemGroupID, FilterItemGroupDesc);
                ItemGroupVM m_objItemGroupVM = m_dicItemGroupData[0][0];
                this.GetCmp<TextField>(ControlItemGroupID).Value = m_objItemGroupVM.ItemGroupID;
                this.GetCmp<TextField>(ControlItemGroupDesc).Value = m_objItemGroupVM.ItemGroupDesc;
                this.GetCmp<TextField>(ControlItemTypeDesc).Value = m_objItemGroupVM.ItemTypeDesc;
                this.GetCmp<TextField>(ControlHasParameter).Clear();
                this.GetCmp<TextField>(ControlHasParameter).Value = m_objItemGroupVM.HasParameter;
                this.GetCmp<TextField>(ControlHasPrice).Clear();
                this.GetCmp<TextField>(ControlHasPrice).Value = m_objItemGroupVM.HasPrice;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, string ItemGroupID, string ItemGroupDesc, string ItemTypeID, List<ItemGroupParameterVM> lst_Parameter)
        {
            List<string> m_lstReturn = new List<string>();

            if (string.IsNullOrEmpty(ItemGroupID))
                m_lstReturn.Add(ItemGroupVM.Prop.ItemGroupID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (ItemGroupID.Length>3)
                m_lstReturn.Add("Maximum length of "+ItemGroupVM.Prop.ItemGroupID.Desc + " " + General.EnumDesc(MessageLib.invalid));
            if (string.IsNullOrEmpty(ItemGroupDesc))
                m_lstReturn.Add(ItemGroupVM.Prop.ItemGroupDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (string.IsNullOrEmpty(ItemTypeID))
                m_lstReturn.Add(ItemGroupVM.Prop.ItemTypeID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
           // if (!lst_Parameter.Any())
           //   m_lstReturn.Add(ItemGroupVM.Prop.ListItemGroupParameterVM.Desc + " " + General.EnumDesc(MessageLib.mustFill));

            return m_lstReturn;
        }
        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(ItemGroupVM.Prop.ItemGroupID.Name, parameters[ItemGroupVM.Prop.ItemGroupID.Name]);
            m_dicReturn.Add(ItemGroupVM.Prop.ItemGroupDesc.Name, parameters[ItemGroupVM.Prop.ItemGroupDesc.Name]);
            m_dicReturn.Add(ItemGroupVM.Prop.ItemTypeID.Name, parameters[ItemGroupVM.Prop.ItemTypeID.Name]);
            m_dicReturn.Add(ItemGroupVM.Prop.ItemTypeDesc.Name, parameters[ItemGroupVM.Prop.ItemTypeDesc.Name]);

            return m_dicReturn;
        }
        private List<ItemGroupParameterVM> GetListItemGroupParameter(string ItemGroupID)
        {
            DItemGroupParameterDA m_objItemGroupParameterDA = new DItemGroupParameterDA();
            m_objItemGroupParameterDA.ConnectionStringName = Global.ConnStrConfigName;

            bool m_boolIsCount = true;

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ItemGroupID);
            m_objFilter.Add(ItemGroupParameterVM.Prop.ItemGroupID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicItemGroupParameterDA = m_objItemGroupParameterDA.SelectBC(0, null, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpParameterBL in m_dicItemGroupParameterDA)
            {
                m_intCount = m_kvpParameterBL.Key;
                break;
            }

            List<ParameterVM> m_lstParameterVM = new List<ParameterVM>();
            //if (m_intCount > 0)
           // {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(ItemGroupParameterVM.Prop.ItemGroupID.MapAlias);
                m_lstSelect.Add(ItemGroupParameterVM.Prop.ParameterID.MapAlias);
                m_lstSelect.Add(ItemGroupParameterVM.Prop.ParameterDesc.MapAlias);

                List<ItemGroupParameterVM> m_lstItemGroupParameterVM = new List<ItemGroupParameterVM>();
                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                // foreach (DataSorter m_dtsOrder in parameters.Sort)
                // m_dicOrder.Add(ParameterVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));
                // m_dicItemGroupParameterDA = m_objItemGroupParameterDA.SelectBC(0, 0, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                m_dicItemGroupParameterDA = m_objItemGroupParameterDA.SelectBC(0, 0, m_boolIsCount, m_lstSelect, m_objFilter, null, null, null, null);
                if (m_objItemGroupParameterDA.Message == string.Empty)
                {
                    m_lstItemGroupParameterVM = (
                from DataRow m_drMParameterDA in m_dicItemGroupParameterDA[0].Tables[0].Rows
                select new ItemGroupParameterVM()
                    {
                        ItemGroupID = m_drMParameterDA[ItemGroupParameterVM.Prop.ItemGroupID.Name].ToString(),
                        ParameterID = m_drMParameterDA[ItemGroupParameterVM.Prop.ParameterID.Name].ToString(),
                        ParameterDesc = m_drMParameterDA[ItemGroupParameterVM.Prop.ParameterDesc.Name].ToString()
                    }
                ).Distinct().ToList();

                    
                //}
            }
            return m_lstItemGroupParameterVM;

        }
        private ItemGroupVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            ItemGroupVM m_objItemGroupVM = new ItemGroupVM();
            MItemGroupDA m_objMItemGroupDA = new MItemGroupDA();
            m_objMItemGroupDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemGroupVM.Prop.ItemGroupID.MapAlias);
            m_lstSelect.Add(ItemGroupVM.Prop.ItemGroupDesc.MapAlias);
            m_lstSelect.Add(ItemGroupVM.Prop.ItemTypeID.MapAlias);
            m_lstSelect.Add(ItemGroupVM.Prop.ItemTypeDesc.MapAlias);
            m_lstSelect.Add(ItemGroupVM.Prop.HasParameter.MapAlias);
            m_lstSelect.Add(ItemGroupVM.Prop.HasPrice.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objItemGroupVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(ItemGroupVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMItemGroupDA = m_objMItemGroupDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMItemGroupDA.Message == string.Empty)
            {
                DataRow m_drMItemGroupDA = m_dicMItemGroupDA[0].Tables[0].Rows[0];
                m_objItemGroupVM.ItemGroupID = m_drMItemGroupDA[ItemGroupVM.Prop.ItemGroupID.Name].ToString();
                m_objItemGroupVM.ItemGroupDesc = m_drMItemGroupDA[ItemGroupVM.Prop.ItemGroupDesc.Name].ToString();
                m_objItemGroupVM.ItemTypeID = m_drMItemGroupDA[ItemTypeVM.Prop.ItemTypeID.Name].ToString();
                m_objItemGroupVM.ItemTypeDesc = m_drMItemGroupDA[ItemTypeVM.Prop.ItemTypeDesc.Name].ToString();
                m_objItemGroupVM.ListItemGroupParameterVM = GetListItemGroupParameter(m_drMItemGroupDA[ItemGroupVM.Prop.ItemGroupID.Name].ToString());
                m_objItemGroupVM.HasParameter = Convert.ToBoolean(m_drMItemGroupDA[ItemGroupVM.Prop.HasParameter.Name].ToString());
                m_objItemGroupVM.HasPrice = Convert.ToBoolean(m_drMItemGroupDA[ItemGroupVM.Prop.HasPrice.Name].ToString());
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMItemGroupDA.Message;

            return m_objItemGroupVM;
        }
        #endregion

        #region Public Method

        public Dictionary<int, List<ItemGroupVM>> GetItemGroupData(bool isCount, string ItemGroupID, string ItemGroupDesc)
        {
            int m_intCount = 0;
            List<ItemGroupVM> m_lstItemGroupVM = new List<ViewModels.ItemGroupVM>();
            Dictionary<int, List<ItemGroupVM>> m_dicReturn = new Dictionary<int, List<ItemGroupVM>>();
            MItemGroupDA m_objMItemGroupDA = new MItemGroupDA();
            m_objMItemGroupDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemGroupVM.Prop.ItemGroupID.MapAlias);
            m_lstSelect.Add(ItemGroupVM.Prop.ItemGroupDesc.MapAlias);
            m_lstSelect.Add(ItemGroupVM.Prop.ItemTypeDesc.MapAlias);
            m_lstSelect.Add(ItemGroupVM.Prop.HasParameter.MapAlias);
            m_lstSelect.Add(ItemGroupVM.Prop.HasPrice.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(ItemGroupID);
            m_objFilter.Add(ItemGroupVM.Prop.ItemGroupID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(ItemGroupDesc);
            m_objFilter.Add(ItemGroupVM.Prop.ItemGroupDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMItemGroupDA = m_objMItemGroupDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMItemGroupDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpItemGroupBL in m_dicMItemGroupDA)
                    {
                        m_intCount = m_kvpItemGroupBL.Key;
                        break;
                    }
                else
                {
                    m_lstItemGroupVM = (
                        from DataRow m_drMItemGroupDA in m_dicMItemGroupDA[0].Tables[0].Rows
                        select new ItemGroupVM()
                        {
                            ItemGroupID = m_drMItemGroupDA[ItemGroupVM.Prop.ItemGroupID.Name].ToString(),
                            ItemGroupDesc = m_drMItemGroupDA[ItemGroupVM.Prop.ItemGroupDesc.Name].ToString(),
                            ItemTypeDesc = m_drMItemGroupDA[ItemGroupVM.Prop.ItemTypeDesc.Name].ToString(),
                            HasParameter = bool.Parse(m_drMItemGroupDA[ItemGroupVM.Prop.HasParameter.Name].ToString()),
                            HasPrice = bool.Parse(m_drMItemGroupDA[ItemGroupVM.Prop.HasPrice.Name].ToString()),

                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstItemGroupVM);
            return m_dicReturn;
        }

        #endregion
    }
}