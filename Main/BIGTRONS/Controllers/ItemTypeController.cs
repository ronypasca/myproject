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
    public class ItemTypeController : BaseController
    {
        private readonly string title = "Item Type";
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
            MItemTypeDA m_objMItemTypeDA = new MItemTypeDA();
            m_objMItemTypeDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMItemType = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMItemType.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = ItemTypeVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMItemTypeDA = m_objMItemTypeDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpItemTypeBL in m_dicMItemTypeDA)
            {
                m_intCount = m_kvpItemTypeBL.Key;
                break;
            }

            List<ItemTypeVM> m_lstItemTypeVM = new List<ItemTypeVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(ItemTypeVM.Prop.ItemTypeID.MapAlias);
                m_lstSelect.Add(ItemTypeVM.Prop.ItemTypeDesc.MapAlias);
                m_lstSelect.Add(ItemTypeVM.Prop.ItemTypeParentDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(ItemTypeVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMItemTypeDA = m_objMItemTypeDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMItemTypeDA.Message == string.Empty)
                {
                    m_lstItemTypeVM = (
                        from DataRow m_drMItemTypeDA in m_dicMItemTypeDA[0].Tables[0].Rows
                        select new ItemTypeVM()
                        {
                            ItemTypeID = m_drMItemTypeDA[ItemTypeVM.Prop.ItemTypeID.Name].ToString(),
                            ItemTypeDesc = m_drMItemTypeDA[ItemTypeVM.Prop.ItemTypeDesc.Name].ToString(),
                            ItemTypeParentDesc = m_drMItemTypeDA[ItemTypeVM.Prop.ItemTypeParentDesc.Name].ToString()

                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstItemTypeVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            MItemTypeDA m_objMItemTypeDA = new MItemTypeDA();
            m_objMItemTypeDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMItemType = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMItemType.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = ItemTypeVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMItemTypeDA = m_objMItemTypeDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpItemTypeBL in m_dicMItemTypeDA)
            {
                m_intCount = m_kvpItemTypeBL.Key;
                break;
            }

            List<ItemTypeVM> m_lstItemTypeVM = new List<ItemTypeVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(ItemTypeVM.Prop.ItemTypeID.MapAlias);
                m_lstSelect.Add(ItemTypeVM.Prop.ItemTypeDesc.MapAlias);
                m_lstSelect.Add(ItemTypeVM.Prop.ItemTypeParentDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(ItemTypeVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMItemTypeDA = m_objMItemTypeDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMItemTypeDA.Message == string.Empty)
                {
                    m_lstItemTypeVM = (
                        from DataRow m_drMItemTypeDA in m_dicMItemTypeDA[0].Tables[0].Rows
                        select new ItemTypeVM()
                        {
                            ItemTypeID = m_drMItemTypeDA[ItemTypeVM.Prop.ItemTypeID.Name].ToString(),
                            ItemTypeDesc = m_drMItemTypeDA[ItemTypeVM.Prop.ItemTypeDesc.Name].ToString(),
                            ItemTypeParentDesc = m_drMItemTypeDA[ItemTypeVM.Prop.ItemTypeParentDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstItemTypeVM, m_intCount);
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

            ItemTypeVM m_objItemTypeVM = new ItemTypeVM();
            ViewDataDictionary m_vddItemType = new ViewDataDictionary();
            m_vddItemType.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddItemType.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
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
                Model = m_objItemTypeVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddItemType,
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

            ItemTypeVM m_objItemTypeVM = new ItemTypeVM();
            if (m_dicSelectedRow.Count > 0)
                m_objItemTypeVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
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
                Model = m_objItemTypeVM,
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
            ItemTypeVM m_objItemTypeVM = new ItemTypeVM();
            if (m_dicSelectedRow.Count > 0)
                m_objItemTypeVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddItemType = new ViewDataDictionary();
            m_vddItemType.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddItemType.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objItemTypeVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddItemType,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<ItemTypeVM> m_lstSelectedRow = new List<ItemTypeVM>();
            m_lstSelectedRow = JSON.Deserialize<List<ItemTypeVM>>(Selected);

            MItemTypeDA m_objMItemTypeDA = new MItemTypeDA();
            m_objMItemTypeDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (ItemTypeVM m_objItemTypeVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifItemTypeVM = m_objItemTypeVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifItemTypeVM in m_arrPifItemTypeVM)
                    {
                        string m_strFieldName = m_pifItemTypeVM.Name;
                        object m_objFieldValue = m_pifItemTypeVM.GetValue(m_objItemTypeVM);
                        if (m_objItemTypeVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(ItemTypeVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objMItemTypeDA.DeleteBC(m_objFilter, false);
                    if (m_objMItemTypeDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMItemTypeDA.Message);
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

        public ActionResult Browse(string ControlItemTypeID, string ControlItemTypeDesc, string FilterItemTypeID = "", string FilterItemTypeDesc = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddItemType = new ViewDataDictionary();
            m_vddItemType.Add("Control" + ItemTypeVM.Prop.ItemTypeID.Name, ControlItemTypeID);
            m_vddItemType.Add("Control" + ItemTypeVM.Prop.ItemTypeDesc.Name, ControlItemTypeDesc);
            m_vddItemType.Add(ItemTypeVM.Prop.ItemTypeID.Name, FilterItemTypeID);
            m_vddItemType.Add(ItemTypeVM.Prop.ItemTypeDesc.Name, FilterItemTypeDesc);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddItemType,
                ViewName = "../ItemType/_Browse"
            };
        }

        public ActionResult Save(string Action)
        {
            //action = JSON.Deserialize<string>(action);
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MItemTypeDA m_objMItemTypeDA = new MItemTypeDA();
            m_objMItemTypeDA.ConnectionStringName = Global.ConnStrConfigName;
            try
            {
                string m_strItemTypeID = this.Request.Params[ItemTypeVM.Prop.ItemTypeID.Name];
                string m_strItemTypeDesc = this.Request.Params[ItemTypeVM.Prop.ItemTypeDesc.Name];
                string m_strItemTypeParentID = (this.Request.Params[ItemTypeVM.Prop.ItemTypeParentID.Name]) == string.Empty ? null : this.Request.Params[ItemTypeVM.Prop.ItemTypeParentID.Name];


                m_lstMessage = IsSaveValid(Action, m_strItemTypeID, m_strItemTypeDesc, m_strItemTypeParentID);
                if (m_lstMessage.Count <= 0)
                {
                    MItemType m_objMItemType = new MItemType();
                    m_objMItemType.ItemTypeID = m_strItemTypeID;
                    m_objMItemTypeDA.Data = m_objMItemType;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objMItemTypeDA.Select();


                    m_objMItemType.ItemTypeDesc = m_strItemTypeDesc;
                    m_objMItemType.ItemTypeParentID = m_strItemTypeParentID;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objMItemTypeDA.Insert(false);
                    else
                        m_objMItemTypeDA.Update(false);

                    if (!m_objMItemTypeDA.Success || m_objMItemTypeDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMItemTypeDA.Message);
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
            }
            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
                return Detail(Action, null);
            }
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);
        }

        #endregion

        #region Direct Method

        public ActionResult GetItemType(string ControlItemTypeParentID, string ControlItemTypeParentDesc, string FilterItemTypeParentID, string FilterItemTypeParentDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<ItemTypeVM>> m_dicItemTypeData = GetItemTypeData(true, FilterItemTypeParentID, FilterItemTypeParentDesc);
                KeyValuePair<int, List<ItemTypeVM>> m_kvpItemTypeVM = m_dicItemTypeData.AsEnumerable().ToList()[0];
                if (m_kvpItemTypeVM.Key < 1 || (m_kvpItemTypeVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpItemTypeVM.Key > 1 && !Exact)
                    return Browse(ControlItemTypeParentID, ControlItemTypeParentDesc, FilterItemTypeParentID, FilterItemTypeParentDesc);

                m_dicItemTypeData = GetItemTypeData(false, FilterItemTypeParentID, FilterItemTypeParentDesc);
                ItemTypeVM m_objItemTypeVM = m_dicItemTypeData[0][0];
                this.GetCmp<TextField>(ControlItemTypeParentID).Value = m_objItemTypeVM.ItemTypeID;
                this.GetCmp<TextField>(ControlItemTypeParentDesc).Value = m_objItemTypeVM.ItemTypeDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }
        public ActionResult GetItemTypeMaster(string ControlItemTypeID, string ControlItemTypeDesc, string FilterItemTypeID, string FilterItemTypeDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<ItemTypeVM>> m_dicItemTypeData = GetItemTypeData(true, FilterItemTypeID, FilterItemTypeDesc);
                KeyValuePair<int, List<ItemTypeVM>> m_kvpItemTypeVM = m_dicItemTypeData.AsEnumerable().ToList()[0];
                if (m_kvpItemTypeVM.Key < 1 || (m_kvpItemTypeVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpItemTypeVM.Key > 1 && !Exact)
                    return Browse(ControlItemTypeID, ControlItemTypeDesc, FilterItemTypeID, FilterItemTypeDesc);

                m_dicItemTypeData = GetItemTypeData(false, FilterItemTypeID, FilterItemTypeDesc);
                ItemTypeVM m_objItemTypeVM = m_dicItemTypeData[0][0];
                this.GetCmp<TextField>(ControlItemTypeID).Value = m_objItemTypeVM.ItemTypeID;
                this.GetCmp<TextField>(ControlItemTypeDesc).Value = m_objItemTypeVM.ItemTypeDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }
        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, string ItemTypeID, string ItemTypeDesc, string ItemTypeParentID)
        {
            List<string> m_lstReturn = new List<string>();

            if (ItemTypeID == string.Empty)
                m_lstReturn.Add(ItemTypeVM.Prop.ItemTypeID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (ItemTypeDesc == string.Empty)
                m_lstReturn.Add(ItemTypeVM.Prop.ItemTypeDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (ItemTypeParentID == ItemTypeID)
                m_lstReturn.Add(ItemTypeVM.Prop.ItemTypeParentID.Desc + " Circular Refrence");

            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(ItemTypeVM.Prop.ItemTypeID.Name, parameters[ItemTypeVM.Prop.ItemTypeID.Name]);
            m_dicReturn.Add(ItemTypeVM.Prop.ItemTypeDesc.Name, parameters[ItemTypeVM.Prop.ItemTypeDesc.Name]);
            m_dicReturn.Add(ItemTypeVM.Prop.ItemTypeParentID.Name, parameters[ItemTypeVM.Prop.ItemTypeParentID.Name]);
            m_dicReturn.Add(ItemTypeVM.Prop.ItemTypeParentDesc.Name, parameters[ItemTypeVM.Prop.ItemTypeParentDesc.Name]);


            return m_dicReturn;
        }

        private ItemTypeVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            ItemTypeVM m_objItemTypeVM = new ItemTypeVM();
            MItemTypeDA m_objMItemTypeDA = new MItemTypeDA();
            m_objMItemTypeDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemTypeVM.Prop.ItemTypeID.MapAlias);
            m_lstSelect.Add(ItemTypeVM.Prop.ItemTypeDesc.MapAlias);
            m_lstSelect.Add(ItemTypeVM.Prop.ItemTypeParentID.MapAlias);
            m_lstSelect.Add(ItemTypeVM.Prop.ItemTypeParentDesc.MapAlias);



            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objItemTypeVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(ItemTypeVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMItemTypeDA = m_objMItemTypeDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMItemTypeDA.Message == string.Empty)
            {
                DataRow m_drMItemTypeDA = m_dicMItemTypeDA[0].Tables[0].Rows[0];
                m_objItemTypeVM.ItemTypeID = m_drMItemTypeDA[ItemTypeVM.Prop.ItemTypeID.Name].ToString();
                m_objItemTypeVM.ItemTypeDesc = m_drMItemTypeDA[ItemTypeVM.Prop.ItemTypeDesc.Name].ToString();
                m_objItemTypeVM.ItemTypeParentID = m_drMItemTypeDA[ItemTypeVM.Prop.ItemTypeParentID.Name].ToString();
                m_objItemTypeVM.ItemTypeParentDesc = m_drMItemTypeDA[ItemTypeVM.Prop.ItemTypeParentDesc.Name].ToString();

            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMItemTypeDA.Message;

            return m_objItemTypeVM;
        }

        #endregion

        #region Public Method

        public Dictionary<int, List<ItemTypeVM>> GetItemTypeData(bool isCount, string ItemTypeID, string ItemTypeDesc)
        {
            int m_intCount = 0;
            List<ItemTypeVM> m_lstItemTypeVM = new List<ViewModels.ItemTypeVM>();
            Dictionary<int, List<ItemTypeVM>> m_dicReturn = new Dictionary<int, List<ItemTypeVM>>();
            MItemTypeDA m_objMItemTypeDA = new MItemTypeDA();
            m_objMItemTypeDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemTypeVM.Prop.ItemTypeID.MapAlias);
            m_lstSelect.Add(ItemTypeVM.Prop.ItemTypeDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(ItemTypeID);
            m_objFilter.Add(ItemTypeVM.Prop.ItemTypeID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(ItemTypeDesc);
            m_objFilter.Add(ItemTypeVM.Prop.ItemTypeDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMItemTypeDA = m_objMItemTypeDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMItemTypeDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpItemTypeBL in m_dicMItemTypeDA)
                    {
                        m_intCount = m_kvpItemTypeBL.Key;
                        break;
                    }
                else
                {
                    m_lstItemTypeVM = (
                        from DataRow m_drMItemTypeDA in m_dicMItemTypeDA[0].Tables[0].Rows
                        select new ItemTypeVM()
                        {
                            ItemTypeID = m_drMItemTypeDA[ItemTypeVM.Prop.ItemTypeID.Name].ToString(),
                            ItemTypeDesc = m_drMItemTypeDA[ItemTypeVM.Prop.ItemTypeDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstItemTypeVM);
            return m_dicReturn;
        }

        #endregion
    }
}