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
using System.IO;
using SW = System.Web;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;

namespace com.SML.BIGTRONS.Controllers
{
    public class ItemComparisonController : BaseController
    {
        private readonly string title = "Item Comparison";
        private readonly string dataSessionName = "ComparisonDetail";
        private readonly string dataSessionDetail = "BtnComparisonDetail";
        private readonly string dataSessionGroup = "dataSessionGroup";
        private readonly string dataUpdate = "DataUpdate";
        private readonly string detaildesc = "DetailDesc";
        private readonly string detailCartSessionName = "DetailCart";
        private readonly int countJSON = 5;
        private readonly string errNullVendor = "Vendor NULL.";
        private readonly string errZeroAmmount = "the Vendor does not contain any item.";
        private readonly string item_id = "ItemID";
        private readonly string item_desc = "ItemDesc";
        private readonly string item_price_id = "ItemPriceID";
        private readonly string vendor_id = "VendorID";
        private readonly string vendor_desc = "VendorDesc";
        private readonly string uom_id = "UoMID";
        private readonly string uom_desc = "UoMDesc";
        private readonly string amount = "Amount";
        #region Public Action

        public ActionResult Index()
        {
            //base.Initialize();
            return View();
        }
        public ActionResult List()
        {
            Global.HasAccess = GetHasAccess();
            //if (!Global.HasAccess.Read)
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            //if (Session[dataSessionName] != null)
            Session[dataSessionName] = null;
            Session[dataUpdate] = null;

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
            MItemComparisonDA m_objMItemComparisonDA = new MItemComparisonDA();
            m_objMItemComparisonDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMItemComparison = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMItemComparison.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = ItemComparisonVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMItemComparisonDA = m_objMItemComparisonDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpItemComparisonBL in m_dicMItemComparisonDA)
            {
                m_intCount = m_kvpItemComparisonBL.Key;
                break;
            }

            List<ItemComparisonVM> m_lstItemComparisonVM = new List<ItemComparisonVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(ItemComparisonVM.Prop.ItemComparisonID.MapAlias);
                m_lstSelect.Add(ItemComparisonVM.Prop.ItemComparisonDesc.MapAlias);
                m_lstSelect.Add(ItemComparisonVM.Prop.UserID.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(ItemComparisonVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMItemComparisonDA = m_objMItemComparisonDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMItemComparisonDA.Message == string.Empty)
                {
                    m_lstItemComparisonVM = (
                        from DataRow m_drMItemComparisonDA in m_dicMItemComparisonDA[0].Tables[0].Rows
                        select new ItemComparisonVM()
                        {
                            ItemComparisonID = m_drMItemComparisonDA[ItemComparisonVM.Prop.ItemComparisonID.Name].ToString(),
                            ItemComparisonDesc = m_drMItemComparisonDA[ItemComparisonVM.Prop.ItemComparisonDesc.Name].ToString(),
                            UserID = m_drMItemComparisonDA[ItemComparisonVM.Prop.UserID.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstItemComparisonVM, m_intCount);
        }
        public ActionResult Home()
        {
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 0;
            return this.Direct();
        }
        public ActionResult Add(string Caller, string Selected, string ItemComparisonVersion)
        {
            Global.HasAccess = GetHasAccess();
            //if (!Global.HasAccess.Add)
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ItemComparisonVM m_objItemComparisonVM = new ItemComparisonVM();
            m_objItemComparisonVM.ListItemComparisonDetailsVM = new List<ItemComparisonDetailsVM>();

            ItemComparisonDetailsVM m_objItemComparisonDetailsVM_ = new ItemComparisonDetailsVM();
            m_objItemComparisonDetailsVM_.ItemComparisonDetailID = " ";
            m_objItemComparisonDetailsVM_.ItemComparisonID = "empty";
            m_objItemComparisonVM.ListItemComparisonDetailsVM.Add(m_objItemComparisonDetailsVM_);

            m_objItemComparisonVM.ListVendorVM = new List<VendorVM>();

            ViewDataDictionary m_vddItemComparison = new ViewDataDictionary();
            m_vddItemComparison.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddItemComparison.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            

            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objItemComparisonVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddItemComparison,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult Checkout(string Caller, string Selected)
        {
            //Global.HasAccess = GetHasAccess();
            //if (!Global.HasAccess.Add)
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();
            List<ItemComparisonDetailsVM> m_objCartItemVM = new List<ItemComparisonDetailsVM>();
            if (Session[dataSessionName] != null)
                m_objCartItemVM = JSON.Deserialize<List<ItemComparisonDetailsVM>>(Session[dataSessionName].ToString());

            ItemComparisonVM m_objCatalogCartVM = new ItemComparisonVM();
            m_objCatalogCartVM.ListItemComparisonDetailsVM = m_objCartItemVM;

            ViewDataDictionary m_vddCartItem = new ViewDataDictionary();
            m_vddCartItem.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddCartItem.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objCatalogCartVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddCartItem,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult AddToComparison(string Caller, string Selected)
        {
            //Global.HasAccess = GetHasAccess();
            //if (!Global.HasAccess.Add)
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            if (Caller == General.EnumDesc(Buttons.ButtonList))
            {
                List<ItemComparisonDetailsVM> m_objSelectedRows = JSON.Deserialize<List<ItemComparisonDetailsVM>>(Selected);
                if (m_objSelectedRows.Count() > 0)
                {
                    if (Session[dataSessionName] != null)
                    {
                        List<ItemComparisonDetailsVM> m_objSessionData = JSON.Deserialize<List<ItemComparisonDetailsVM>>(Session[dataSessionName].ToString());
                        foreach (var item in m_objSelectedRows)
                        {
                            ItemComparisonDetailsVM objSession = m_objSessionData.Where(d => d.ItemID == item.ItemID && d.VendorID == item.VendorID).FirstOrDefault();
                            if (objSession == null)
                            {
                                m_objSessionData.Add(item);
                            }
                        }
                        Session[dataSessionName] = JSON.Serialize(m_objSessionData);
                    }
                    else
                    {
                        Session[dataSessionName] = JSON.Serialize(m_objSelectedRows);
                    }
                }
                Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Added));
            }
            return this.Direct();
        }
        public ActionResult Save()
        {
            //Global.HasAccess = GetHasAccess();
            //if (!Global.HasAccess.Add)
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            if (Session[dataSessionName] != null)
            {
                string itemComparison_desc = Request.Params["m_txtItemComparisonDesc"].ToString();

                List<ItemComparisonDetailsVM> m_objSessionData = JSON.Deserialize<List<ItemComparisonDetailsVM>>(Session[dataSessionName].ToString());

                UserVM userVM = getCurentUser();

                MItemComparisonDA m_objItemComparisonDA = new MItemComparisonDA();
                DItemComparisonDetailsDA m_objItemComparisonDetailsDA = new DItemComparisonDetailsDA();
                DItemComparisonSessionDA m_objItemComparisonSessionDA = new DItemComparisonSessionDA();

                ItemComparisonVM m_itemComparisonVM = new ItemComparisonVM();
                ItemComparisonDetailsVM m_itemComparisonDetailsVM = new ItemComparisonDetailsVM();
                ItemComparisonSessionVM m_objItemComparisonSessionVM = new ItemComparisonSessionVM();

                m_objItemComparisonDA.ConnectionStringName = Global.ConnStrConfigName;
                m_objItemComparisonDetailsDA.ConnectionStringName = Global.ConnStrConfigName;
                m_objItemComparisonSessionDA.ConnectionStringName = Global.ConnStrConfigName;


                object m_objDBConnection = null;

                if (!IsSaveValid())
                {
                    return this.Direct(false, General.EnumDesc(MessageLib.ErrorSave));
                }

                MItemComparison m_objItemComparison = new MItemComparison();

                m_objItemComparison.ItemComparisonID = Guid.NewGuid().ToString().Replace("-", "");
                m_objItemComparison.ItemComparisonDesc = itemComparison_desc;
                m_objItemComparison.UserID = getCurentUser().UserID;
                m_objItemComparisonDA.Data = m_objItemComparison;

                if (Session[dataUpdate] == null)
                {
                    m_objItemComparisonDA.Insert(false, m_objDBConnection);
                }

                if ((!m_objItemComparisonDA.Success || m_objItemComparisonDA.Message != string.Empty) && Session[dataUpdate] == null)
                {
                    return this.Direct(false, General.EnumDesc(MessageLib.ErrorSave));
                }

                DItemComparisonDetails m_objItemComparisonDetails = new DItemComparisonDetails();

                foreach (var item in m_objSessionData)
                {
                    if (item.ItemPriceID != null)
                    {
                        m_objItemComparisonDetails.ItemComparisonDetailID = Guid.NewGuid().ToString().Replace("-", "");
                        m_objItemComparisonDetails.ItemPriceID = item.ItemPriceID;
                        m_objItemComparisonDetails.VendorID = item.VendorID;
                        m_objItemComparisonDetails.ValidFrom = DateTime.Parse(item.ValidFrom.ToString());
                        m_objItemComparisonDetailsDA.Data = m_objItemComparisonDetails;

                        if (Session[dataUpdate] == null)
                        {
                            //Save
                            m_objItemComparisonDetails.ItemComparisonID = m_objItemComparison.ItemComparisonID;
                        }
                        else
                        {
                            //UPDATE
                            if (!string.IsNullOrEmpty(item.ItemComparisonID))
                            {
                                m_objItemComparisonDetails.ItemComparisonID = item.ItemComparisonID;
                                m_objItemComparison.ItemComparisonID = item.ItemComparisonID;
                                m_objItemComparison.ItemComparisonDesc = itemComparison_desc;
                                m_objItemComparisonDA.Update(false, m_objDBConnection);
                            }
                        }

                        if (!IsDuplicate(item.ItemComparisonDetailID))
                        {
                            m_objItemComparisonDetailsDA.Insert(false, m_objDBConnection);

                            if (!m_objItemComparisonDetailsDA.Success || m_objItemComparisonDetailsDA.Message != string.Empty)
                            {
                                return this.Direct(false, General.EnumDesc(MessageLib.ErrorSave));
                            }
                        }

                        if (IsComparisonSessionExists(Session.SessionID))
                        {
                            DItemComparisonSession m_objDItemComparisonSession = new DItemComparisonSession();

                            Dictionary<string, List<object>> m_dicSet = new Dictionary<string, List<object>>();
                            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(Session.SessionID.ToString());
                            m_objFilter.Add(ItemComparisonSessionVM.Prop.SessionID.Map, m_lstFilter);

                            List<object> m_lstSet = new List<object>();
                            m_lstSet.Add(typeof(string));
                            m_lstSet.Add(m_objItemComparison.ItemComparisonID);
                            m_dicSet.Add(ItemComparisonSessionVM.Prop.ComparisonDetilID.Map, m_lstSet);

                            m_objItemComparisonSessionDA.Data = m_objDItemComparisonSession;

                            m_objItemComparisonSessionDA.UpdateBC(m_dicSet, m_objFilter, false, m_objDBConnection);

                            if (!m_objItemComparisonSessionDA.Success || m_objItemComparisonSessionDA.Message != string.Empty)
                            {
                                return this.Direct(false, General.EnumDesc(MessageLib.ErrorSave));
                            }
                        }
                    }
                }
                Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.saved));
                Session[dataSessionName] = null;
            }
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = null,
                RenderMode = RenderMode.AddTo,
                ViewData = null,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        [ValidateInput(false)]
        public ActionResult DeleteDetail(string Selected)
        {
            //Global.HasAccess = GetHasAccess();
            //if (!Global.HasAccess.Add)
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<ItemComparisonDetailsVM> m_objSelectedRows = JSON.Deserialize<List<ItemComparisonDetailsVM>>(Selected);
            if (m_objSelectedRows.Count() > 0)
            {
                if (Session[dataSessionName] != null)
                {
                    List<ItemComparisonDetailsVM> m_objSessionData = JSON.Deserialize<List<ItemComparisonDetailsVM>>(Session[dataSessionName].ToString());
                    foreach (var item in m_objSelectedRows)
                    {
                        if (item.ItemID.ToString().Contains(','))
                        {
                            DItemComparisonSessionDA m_objItemComparisonDA = new DItemComparisonSessionDA();
                            ItemComparisonSessionVM m_objItemComparisonVM = new ItemComparisonSessionVM();

                            m_objItemComparisonDA.ConnectionStringName = Global.ConnStrConfigName;
                            string m_strTransName = "ItemComparisonSession";
                            object m_objDBConnection = null;
                            m_objDBConnection = m_objItemComparisonDA.BeginTrans(m_strTransName);

                            DItemComparisonSession m_objDItemComparisonSession = new DItemComparisonSession();
                            m_objDItemComparisonSession.SessionID = Session.SessionID.ToString();

                            List<string> m_lstKey = new List<string>();
                            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

                            var item_arr = item.ItemID.ToString().Split(',');
                            foreach (var obj_itemArr in item_arr)
                            {
                                m_objSessionData.RemoveAll(i => i.ItemID == obj_itemArr);

                                List<object> m_lstFilter = new List<object>();
                                m_lstFilter.Add(Operator.In);
                                m_lstFilter.Add(obj_itemArr);
                                m_objFilter.Add(ItemComparisonSessionVM.Prop.ItemID.Map, m_lstFilter);

                                m_objDItemComparisonSession.SessionID = Session.SessionID.ToString();
                                m_objItemComparisonDA.Data = m_objDItemComparisonSession;
                                m_objItemComparisonDA.DeleteBC(m_objFilter, true, m_objDBConnection);

                                if (!m_objItemComparisonDA.Success || m_objItemComparisonDA.Message != string.Empty)
                                {
                                    m_objItemComparisonDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                                    Global.ShowErrorAlert(title, General.EnumDesc(MessageLib.ErrorDelete));
                                    return this.Direct();
                                }
                                m_objFilter.Clear();
                            }
                            m_objItemComparisonDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
                        }
                        else
                        {
                            m_objSessionData.RemoveAll(i => i.ItemID == item.ItemID);
                        }
                    }
                    if (m_objSessionData.Count() == 0)
                    {
                        Session[dataSessionName] = null;
                    }
                    else
                    {
                        Session[dataSessionName] = JSON.Serialize(m_objSessionData);
                    }
                }
                Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Deleted));
            }
            
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = null,
                RenderMode = RenderMode.AddTo,
                ViewData = null,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult Delete(string caller, string selected)
        {
            //Global.HasAccess = GetHasAccess();
            //if (!Global.HasAccess.Add)
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();
            m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(selected.Replace("[", "").Replace("]",""));
            string m_strMessage = string.Empty;
            string m_comparisonId = string.Empty;

            List<ItemComparisonDetailsVM> m_objItemComparisonVM = new List<ItemComparisonDetailsVM>();

            MItemComparisonDA m_objItemComparisonDA = new MItemComparisonDA();
            DItemComparisonDetailsDA m_objItemComparisonDetailsDA = new DItemComparisonDetailsDA();

            ItemComparisonVM m_itemComparisonVM = new ItemComparisonVM();

            DItemComparisonDetails m_objItemComparisonDetails = new DItemComparisonDetails();
            MItemComparison m_objItemComparison = new MItemComparison();

            m_objItemComparisonDetailsDA.ConnectionStringName = Global.ConnStrConfigName;
            m_objItemComparisonDA.ConnectionStringName = Global.ConnStrConfigName;

            object m_objDBConnection = null;

            m_objItemComparisonVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in m_dicSelectedRow)
            {
                if (m_itemComparisonVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(ItemComparisonDetailsVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);

                    m_objItemComparison.ItemComparisonID = m_kvpSelectedRow.Value.ToString();
                }
            }

            foreach (var item in m_objItemComparisonVM)
            {
                m_objItemComparisonDetails.ItemComparisonID = item.ItemComparisonID.ToString();
                m_objItemComparisonDetails.ItemComparisonDetailID = item.ItemComparisonDetailID.ToString();
                m_objItemComparisonDetailsDA.Data = m_objItemComparisonDetails;
                m_objItemComparisonDetailsDA.DeleteBC(m_objFilter, false, m_objDBConnection);
                
            }

            m_objItemComparisonDA.Data = m_objItemComparison;
            m_objItemComparisonDA.Delete(false, m_objDBConnection);

            if ((!m_objItemComparisonDA.Success || m_objItemComparisonDA.Message != string.Empty))
            {
                return this.Direct(false, General.EnumDesc(MessageLib.ErrorDelete));
            }

            Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.deleted));

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
        [ValidateInput(false)]
        public ActionResult Group(string Selected)
        {
            DItemComparisonSessionDA m_objItemComparisonDA = new DItemComparisonSessionDA();
            ItemComparisonSessionVM m_objItemComparisonVM = new ItemComparisonSessionVM();

            m_objItemComparisonDA.ConnectionStringName = Global.ConnStrConfigName;
            string m_strTransName = "ItemComparisonSession";
            object m_objDBConnection = null;
            m_objDBConnection = m_objItemComparisonDA.BeginTrans(m_strTransName);

            DItemComparisonSession m_objDItemComparisonSession = new DItemComparisonSession();

            m_objDItemComparisonSession.GroupGUID = Guid.NewGuid().ToString().Replace("-", "");
            m_objDItemComparisonSession.SessionID = Session.SessionID.ToString();
            m_objDItemComparisonSession.ComparisonDetilID = null;
            m_objDItemComparisonSession.CreatedBy = getCurentUser().UserID;

            int j_index = 1;
            JArray j_arr_count = JArray.Parse(Selected);
            List<string> list_itemDesc = new List<string>();

            foreach (var objItemDesc in j_arr_count.Children<JObject>())
            {
                list_itemDesc.Add(objItemDesc[uom_desc].ToString());
            }

            if (list_itemDesc.Distinct().Count() > 1)
            {
                Global.ShowErrorAlert(title, General.EnumDesc(MessageLib.NotMatched));
                return this.Direct();
            }

            if (!string.IsNullOrEmpty(Selected))
            {
                JArray j_arr = JArray.Parse(Selected);
                Dictionary<string, object> d_selected = new Dictionary<string, object>();
                string secondary_name = "";

                foreach (JObject j_obj in j_arr.Children<JObject>())
                {
                    foreach (JProperty j_prop in j_obj.Properties())
                    {
                        string j_name = j_prop.Name;
                        string j_value = (string)j_prop.Value;

                        if (j_name == item_desc)
                        {
                            if (secondary_name != "")
                                secondary_name += "<br />";
                            secondary_name += j_value;
                        }
                    }
                }


                foreach (JObject j_obj in j_arr.Children<JObject>())
                {
                    foreach (JProperty j_prop in j_obj.Properties())
                    {
                        string j_name = j_prop.Name;
                        string j_value = (string)j_prop.Value;

                        if (j_name == item_id)
                        {
                            m_objDItemComparisonSession.ItemID = j_value;
                            if (j_value.Contains(","))
                            {
                                Global.ShowErrorAlert(title, General.EnumDesc(MessageLib.NotMatched));
                                return this.Direct();
                            }
                        }

                        if (j_name == item_desc)
                        {
                            m_objDItemComparisonSession.ItemName = j_value;
                            m_objDItemComparisonSession.ItemSecondaryName = secondary_name;
                        }
                    }
                    m_objItemComparisonDA.Data = m_objDItemComparisonSession;
                    m_objItemComparisonDA.Insert(true, m_objDBConnection);

                    if (!m_objItemComparisonDA.Success || m_objItemComparisonDA.Message != string.Empty)
                    {
                        m_objItemComparisonDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                        return this.Direct(false, General.EnumDesc(MessageLib.ErrorSave));
                    }

                    j_index++;

                }
                m_objItemComparisonDA.EndTrans(ref m_objDBConnection, true, m_strTransName);

                Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Updated));
                Session[dataSessionGroup] = d_selected;
            }
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = null,
                RenderMode = RenderMode.AddTo,
                ViewData = null,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        [ValidateInput(false)]
        public ActionResult Ungroup(string Selected)
        {
            DItemComparisonSessionDA m_objItemComparisonDA = new DItemComparisonSessionDA();
            ItemComparisonSessionVM m_objItemComparisonVM = new ItemComparisonSessionVM();

            m_objItemComparisonDA.ConnectionStringName = Global.ConnStrConfigName;
            string m_strTransName = "ItemComparisonSession";
            object m_objDBConnection = null;
            m_objDBConnection = m_objItemComparisonDA.BeginTrans(m_strTransName);

            DItemComparisonSession m_objDItemComparisonSession = new DItemComparisonSession();
            m_objDItemComparisonSession.SessionID = Session.SessionID.ToString();

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            if (!string.IsNullOrEmpty(Selected))
            {
                JArray j_arr = JArray.Parse(Selected);
                Dictionary<string, object> d_selected = new Dictionary<string, object>();
                int selected_count = j_arr.Children<JObject>().Count();

                if (selected_count != 1)
                {
                    Global.ShowErrorAlert(title, General.EnumDesc(MessageLib.Invalid));
                    return this.Direct();
                }
                
                foreach (JObject j_obj in j_arr.Children<JObject>())
                {
                    foreach (JProperty j_prop in j_obj.Properties())
                    {
                        string j_name = j_prop.Name;
                        string j_value = (string)j_prop.Value;

                        if (j_name == item_id)
                        {
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.In);
                            m_lstFilter.Add(j_value.ToString());
                            m_objFilter.Add(ItemComparisonSessionVM.Prop.ItemID.Map, m_lstFilter);

                            m_objDItemComparisonSession.SessionID = Session.SessionID.ToString();
                            m_objItemComparisonDA.Data = m_objDItemComparisonSession;
                            m_objItemComparisonDA.DeleteBC(m_objFilter, true, m_objDBConnection);

                            if (!m_objItemComparisonDA.Success || m_objItemComparisonDA.Message != string.Empty)
                            {
                                m_objItemComparisonDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                                Global.ShowErrorAlert(title, General.EnumDesc(MessageLib.ErrorDelete));
                                return this.Direct();
                            }
                            m_objFilter.Clear();
                        }
                    }
                }
                m_objItemComparisonDA.EndTrans(ref m_objDBConnection, true, m_strTransName);

                Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Updated));
                Session[dataSessionGroup] = d_selected;
            }
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = null,
                RenderMode = RenderMode.AddTo,
                ViewData = null,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult Update(string Caller, string Selected)
        {
            Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();


            if (Session[dataSessionDetail] == null && Selected != null)
            {
                m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
                string m_strDesc = m_dicSelectedRow["ItemComparisonDesc"].ToString();
                string m_strMessage = string.Empty;

                Session[detaildesc] = m_strDesc;

                List<ItemComparisonDetailsVM> m_objItemComparisonVM = new List<ItemComparisonDetailsVM>();

                m_objItemComparisonVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
                Session[dataSessionName] = JSON.Serialize(m_objItemComparisonVM);
            }
            else
            {
                Session[detaildesc] = string.IsNullOrEmpty(Request.Params["m_txtItemComparisonDesc"].ToString()) ? "": Request.Params["m_txtItemComparisonDesc"].ToString();
            }

            Session[dataSessionDetail] = null;
            Session[dataUpdate] = "Update";

            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = null,
                RenderMode = RenderMode.AddTo,
                ViewData = null,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public bool IsSaveValid()
        {
            bool is_bool = true;
            if (Session[dataSessionName] == null)
            {
                is_bool = false;
            }
            return is_bool;
        }
        public bool IsCartValid(string Selected)
        {
            JArray j_arr = JArray.Parse(Selected);
            bool is_bool;
            int check_null = 0;
            int j_index;
            is_bool = false;

            foreach (JObject j_obj in j_arr.Children<JObject>())
            {
                j_index = 1;

                foreach (JProperty j_prop in j_obj.Properties())
                {
                    string j_name = j_prop.Name;
                    string j_value = (string)j_prop.Value;
                    
                    if (j_index >= countJSON)
                    {
                        if (j_index % 2 == 1)
                        {
                            if (j_value == "True")
                            {
                                check_null += 1;
                            }
                        }
                    }
                    j_index++;
                }
                is_bool = false;

                if (check_null > 0)
                {
                    is_bool = true;
                    check_null = 0;
                }
            }

            return is_bool;
        }
        [ValidateInput(false)]
        public ActionResult ToCart(string Caller, string Selected)
        {
            JArray j_arr = JArray.Parse(Selected);
            Dictionary<string, object> d_selected = new Dictionary<string, object>();
            
            int j_index;
            bool is_next;

            if (!IsCartValid(Selected))
            {
                string message = "";
                message = this.errNullVendor;

                Global.ShowInfoAlert(title, message);
                return this.Direct();
            }

            foreach (JObject j_obj in j_arr.Children<JObject>())
            {
                j_index = 1;
                is_next = false;
                
                foreach (JProperty j_prop in j_obj.Properties())
                {
                    string j_name = j_prop.Name;
                    string j_value = (string)j_prop.Value;
                    
                    if (j_index >= countJSON)
                    {
                        if (j_index % 2 == 1)
                        {
                            if (j_value == "True")
                            {
                                if (d_selected.ContainsKey(vendor_id))
                                {
                                    d_selected.Remove(vendor_id);
                                }
                                d_selected.Add(vendor_id, j_name.Substring(2));
                                is_next = true;
                            }
                        }
                        else
                        {
                            if (is_next)
                            {
                                if (d_selected[item_id].ToString().Contains(','))
                                {
                                    d_selected[item_id] = d_selected[item_id].ToString().Split(',')[0];
                                }

                                d_selected[item_desc] = d_selected[item_desc].ToString().Replace("<br />", ",");
                                if (d_selected[item_desc].ToString().Contains(','))
                                {
                                    d_selected[item_desc] = d_selected[item_desc].ToString().Replace("<br />", ",");
                                    d_selected[item_desc] = d_selected[item_desc].ToString().Split(',')[0];
                                }

                                if (d_selected.ContainsKey(vendor_desc))
                                {
                                    d_selected.Remove(vendor_desc);
                                }

                                d_selected.Add(vendor_desc, j_name);

                                if (d_selected.ContainsKey(amount))
                                {
                                    d_selected.Remove(amount);
                                }
                                d_selected.Add(amount, j_value);

                                if (j_value == "0")
                                {
                                    Global.ShowInfoAlert(title, errZeroAmmount);
                                    return this.Direct();
                                }

                                string j_selected = JsonConvert.SerializeObject(d_selected, Formatting.Indented);
                                j_selected = "[" + j_selected.Replace(System.Environment.NewLine, string.Empty) + "]";

                                Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();
                                List<CartItemVM> m_objSelectedRows = JSON.Deserialize<List<CartItemVM>>(j_selected);

                                if (Session[detailCartSessionName] != null)
                                {
                                    List<CartItemVM> m_objSessionData = JSON.Deserialize<List<CartItemVM>>(Session[detailCartSessionName].ToString());
                                    foreach (var item in m_objSelectedRows)
                                    {
                                        CartItemVM objSession = m_objSessionData.Where(d => d.ItemID == item.ItemID && d.VendorID == item.VendorID).FirstOrDefault();
                                        if (objSession == null)
                                        {
                                            item.Qty = 1;
                                            m_objSessionData.Add(item);
                                        }
                                        else
                                        {
                                            if (item.Qty <= 0) item.Qty = 1;
                                            objSession.Qty = objSession.Qty + item.Qty;
                                        }
                                    }
                                    Session[detailCartSessionName] = JSON.Serialize(m_objSessionData);
                                }
                                else
                                {
                                    List<CartItemVM> m_lsobjCartItemVM = new List<CartItemVM>();
                                    foreach (var item in m_objSelectedRows)
                                    {
                                        item.Qty = 1;
                                        if (!m_lsobjCartItemVM.Any(x => x.ItemID.Equals(item.ItemID)))
                                            m_lsobjCartItemVM.Add(item);
                                    }
                                    Session[detailCartSessionName] = JSON.Serialize(m_lsobjCartItemVM);
                                }

                                is_next = false;
                            }
                        }
                    }
                    else
                    {
                        if (d_selected.ContainsKey(j_name))
                        {
                            d_selected.Remove(j_name);
                        }
                        d_selected.Add(j_name, j_value);
                    }
                    j_index++;
                }
            }
            
            Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Added));
            return this.Direct();
        }
        public bool IsDuplicate(string itemComparationDetailID)
        {
            bool is_bool = true;

            DItemComparisonDetailsDA m_objDItemComparisonDetails = new DItemComparisonDetailsDA();

            m_objDItemComparisonDetails.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemComparisonDetailsVM.Prop.ItemComparisonDetailID.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(itemComparationDetailID);
            m_objFilter.Add(ItemComparisonDetailsVM.Prop.ItemComparisonDetailID.Map, m_lstFilter);

            if (itemComparationDetailID == null)
            {
                is_bool = false;
            }
            else
            {
                Dictionary<int, DataSet> m_dicDItemComparisonDetails = m_objDItemComparisonDetails.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
                if (m_dicDItemComparisonDetails[0].Tables[0].Rows.Count == 0)
                {
                    is_bool = false;
                }
            }
            
            return is_bool;
        }
        public ActionResult Detail(string Caller, string Selected)
        {
            Session[dataSessionDetail] = "Detail";

            Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();
            m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
            string m_strMessage = string.Empty;
            string m_strDesc = m_dicSelectedRow["ItemComparisonDesc"].ToString();

            List< ItemComparisonDetailsVM> m_objItemComparisonVM = new List<ItemComparisonDetailsVM>();
            
            m_objItemComparisonVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            Session[dataSessionName] = JSON.Serialize(m_objItemComparisonVM);
            Session[detaildesc] = m_strDesc;

            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objItemComparisonVM,
                RenderMode = RenderMode.AddTo,
                ViewData = null,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult GetPanel() 
        {
            Panel BPPanel = new Panel
            {
                ID = "P" + "Item" + "Comparation",
                Frame = true,
                Title = "Item Comparation",
                Border = false
            };
            Toolbar m_BPPanelToolbar = new Toolbar();
            //Search Item
            Button m_btnSearch = new Button() { ID = "BtnSearch", Text = "Search Item", Icon = Icon.CartMagnify };
            m_btnSearch.DirectEvents.Click.Action = Url.Action("list", "catalog");
            m_btnSearch.DirectEvents.Click.EventMask.ShowMask = true;
            m_btnSearch.Visible = Session[dataSessionDetail] == null ? true : false;   
            m_BPPanelToolbar.Items.Add(m_btnSearch);

            //Submit To Cart
            List<Parameter> m_lstParameter = new List<Parameter>();
            Parameter m_param;
            m_param = new Parameter("Selected", "App.grdItemComparison.getRowsValues({selectedOnly:true})", ParameterMode.Raw, true);
            m_lstParameter.Add(m_param);

            Button m_btnSubmit = new Button() { ID = "BtnSubmit", Text = "Submit To Cart", Icon = Icon.CartAdd };
            m_btnSubmit.DirectEvents.Click.Action = Url.Action("ToCart", "itemcomparison");
            m_btnSubmit.DirectEvents.Click.EventMask.ShowMask = true;
            m_btnSubmit.Disabled = true;
            m_btnSubmit.DirectEvents.Click.ExtraParams.AddRange(m_lstParameter);
            m_btnSubmit.Visible = Session[dataSessionDetail] == null ? true : false;
            m_BPPanelToolbar.Items.Add(m_btnSubmit);

            //View Cart
            Button m_btnView = new Button() { ID = "BtnView", Text = "View Cart", Icon = Icon.CartGo };
            m_btnView.DirectEvents.Click.Action = Url.Action("checkout", "Cart");
            m_btnView.DirectEvents.Click.EventMask.ShowMask = true;
            m_btnView.Visible = Session[dataSessionDetail] == null ? true : false;
            m_BPPanelToolbar.Items.Add(m_btnView);

            //Save
            Button m_btnSave = new Button() { ID = "BtnSave", Text = "Save", Icon = Icon.DatabaseSave };
            m_btnSave.DirectEvents.Click.Action = Url.Action("Save", "itemcomparison");
            m_btnSave.DirectEvents.Click.EventMask.ShowMask = true;
            m_btnSave.Visible = Session[dataSessionDetail] == null ? true : false;
            m_BPPanelToolbar.Items.Add(m_btnSave);

            //Delete
            Button m_btnDelete = new Button() { ID = "BtnDelete", Text = "Delete", Icon = Icon.DatabaseDelete };
            m_btnDelete.DirectEvents.Click.Action = Url.Action("DeleteDetail", "itemcomparison");
            m_btnDelete.DirectEvents.Click.EventMask.ShowMask = true;
            m_btnDelete.Disabled = true;
            m_btnDelete.DirectEvents.Click.ExtraParams.AddRange(m_lstParameter);
            m_btnDelete.Visible = Session[dataSessionDetail] == null ? true : false;
            m_BPPanelToolbar.Items.Add(m_btnDelete);

            //Update
            Button m_btnUpdate = new Button() { ID = "BtnUpdate", Text = "Update", Icon = Icon.DatabaseEdit };
            m_btnUpdate.DirectEvents.Click.Action = Url.Action("Update", "itemcomparison");
            m_btnUpdate.DirectEvents.Click.EventMask.ShowMask = true;
            m_btnUpdate.Visible = Session[dataSessionDetail] == null ? false : true;
            m_BPPanelToolbar.Items.Add(m_btnUpdate);

            //Group
            Button m_btnGroup = new Button() { ID = "BtnGroup", Text = "Group", Icon = Icon.Group };
            m_btnGroup.DirectEvents.Click.Action = Url.Action("Group", "ItemComparison");
            m_btnGroup.DirectEvents.Click.EventMask.ShowMask = true;
            m_btnGroup.Disabled = true;
            m_btnGroup.Visible = Session[dataSessionDetail] == null ? true : false;
            m_btnGroup.DirectEvents.Click.ExtraParams.AddRange(m_lstParameter);
            m_BPPanelToolbar.Items.Add(m_btnGroup);

            //Ungroup
            Button m_btnUngroup = new Button() { ID = "BtnUngroup", Text = "Ungroup", Icon = Icon.ShapeUngroup };
            m_btnUngroup.DirectEvents.Click.Action = Url.Action("Ungroup", "ItemComparison");
            m_btnUngroup.DirectEvents.Click.EventMask.ShowMask = true;
            m_btnUngroup.Disabled = true;
            m_btnUngroup.Visible = Session[dataSessionDetail] == null ? true : false;
            m_btnUngroup.DirectEvents.Click.ExtraParams.AddRange(m_lstParameter);
            m_BPPanelToolbar.Items.Add(m_btnUngroup);

            //List
            Button m_btnList = new Button() { ID = "BtnList", Text = "List", Icon = Icon.ApplicationViewList };
            m_btnList.DirectEvents.Click.Action = Url.Action("List", "itemcomparison");
            m_btnList.DirectEvents.Click.EventMask.ShowMask = true;
            m_BPPanelToolbar.Items.Add(m_btnList);
            
            TextField m_textDesc = new TextField();
            m_textDesc.ID = "m_txtItemComparisonDesc";
            m_textDesc.FieldLabel = "Description";
            m_textDesc.Padding = 5;

            m_textDesc.Text = string.Empty;

            if (Session[detaildesc] != null)
            {
                m_textDesc.Text = Session[detaildesc].ToString();
                Session[detaildesc] = null;
            }
           
            BPPanel.Items.Add(m_textDesc);

            if (Session[dataSessionName] != null)
            {
                List<ItemComparisonDetailsVM> m_objSessionData = JSON.Deserialize<List<ItemComparisonDetailsVM>>(Session[dataSessionName].ToString());
                var vendorList = new List<string>();
                
                string vendorId;

                foreach (var item in m_objSessionData)
                {
                    vendorList.Add(item.VendorID);
                }

                vendorId = string.Join(",", vendorList.ToArray());

                GridPanel m_GPBP = generateGridPanel(GetComparation(vendorId));
                BPPanel.Items.Add(m_GPBP);
            }
            BPPanel.TopBar.Add(m_BPPanelToolbar);
            Session[dataSessionDetail] = null;
            return this.ComponentConfig(BPPanel);
        }
        #endregion

        #region Direct Method

        public ActionResult GetStatusList(StoreRequestParameters parameters)
        {
            List<StatusVM> m_objStatusVM = new List<StatusVM>();

            DataAccess.MStatusDA m_objMStatusDA = new DataAccess.MStatusDA();
            m_objMStatusDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(StatusVM.Prop.StatusDesc.MapAlias);
            m_lstSelect.Add(StatusVM.Prop.StatusID.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("DItemComparisonDetail");
            m_objFilter.Add(StatusVM.Prop.TableName.Name, m_lstFilter);

            Dictionary<int, System.Data.DataSet> m_dicMStatusDA = m_objMStatusDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objMStatusDA.Message == String.Empty)
            {
                for (int i = 0; i < m_dicMStatusDA[0].Tables[0].Rows.Count; i++)
                {
                    m_objStatusVM.Add(new StatusVM() { StatusDesc = m_dicMStatusDA[0].Tables[0].Rows[i][StatusVM.Prop.StatusDesc.Name].ToString(), StatusID = int.Parse(m_dicMStatusDA[0].Tables[0].Rows[i][StatusVM.Prop.StatusID.Name].ToString()) });
                }
            }

            return this.Store(m_objStatusVM);
        }

        #endregion

        #region Private Method
        private List<ItemComparisonDetailsVM> GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            ItemComparisonVM m_objItemComparisonVM = new ItemComparisonVM();
            ItemComparisonDetailsVM m_objItemComparisonDetails = new ItemComparisonDetailsVM();

            DItemComparisonDetailsDA m_objDItemComparisonDetails = new DItemComparisonDetailsDA();
            m_objDItemComparisonDetails.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemComparisonDetailsVM.Prop.ItemComparisonDetailID.MapAlias);
            m_lstSelect.Add(ItemComparisonDetailsVM.Prop.ItemComparisonID.MapAlias);
            m_lstSelect.Add(ItemComparisonDetailsVM.Prop.ItemPriceID.MapAlias);
            m_lstSelect.Add(ItemComparisonDetailsVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(ItemComparisonDetailsVM.Prop.ValidFrom.MapAlias);
            m_lstSelect.Add(ItemVM.Prop.ItemID.MapAlias);


            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objItemComparisonVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(ItemComparisonDetailsVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }
            List<ItemComparisonDetailsVM> listItemComparison = new List<ItemComparisonDetailsVM>();
            Dictionary<int, DataSet> m_dicDItemComparisonDetails = m_objDItemComparisonDetails.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);

            ItemComparisonDetailsVM m_itemComparisonVM;
           

            foreach (DataRow d_itemComparisonDetails in m_dicDItemComparisonDetails[0].Tables[0].Rows)
            {
                m_itemComparisonVM = new ItemComparisonDetailsVM();
                m_itemComparisonVM.ItemComparisonDetailID = d_itemComparisonDetails[ItemComparisonDetailsVM.Prop.ItemComparisonDetailID.Name].ToString();
                m_itemComparisonVM.ItemComparisonID = d_itemComparisonDetails[ItemComparisonDetailsVM.Prop.ItemComparisonID.Name].ToString();
                m_itemComparisonVM.ItemPriceID = d_itemComparisonDetails[ItemComparisonDetailsVM.Prop.ItemPriceID.Name].ToString();
                m_itemComparisonVM.VendorID = d_itemComparisonDetails[ItemComparisonDetailsVM.Prop.VendorID.Name].ToString();
                m_itemComparisonVM.ItemID = d_itemComparisonDetails[ItemVM.Prop.ItemID.Name].ToString();
                m_itemComparisonVM.ValidFrom = DateTime.Parse(d_itemComparisonDetails[ItemPriceVendorPeriodVM.Prop.ValidFrom.Name].ToString());
                listItemComparison.Add(m_itemComparisonVM);
            }
            
            return listItemComparison;
        }
        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(ItemComparisonVM.Prop.ItemComparisonID.Name, parameters[ItemComparisonVM.Prop.ItemComparisonID.Name]);
            m_dicReturn.Add(ItemComparisonVM.Prop.ItemComparisonDesc.Name, parameters[ItemComparisonVM.Prop.ItemComparisonDesc.Name]);
            m_dicReturn.Add(ItemComparisonVM.Prop.UserID.Name, parameters[ItemComparisonVM.Prop.UserID.Name]);

            return m_dicReturn;
        }
        private GridPanel generateGridPanel(DataTable ListItemComparation)
        {
            GridPanel m_gridpanel = new GridPanel
            {
                ID = "grdItemComparison",
                Padding = 10,
                MinHeight = 200
            };

            //SelectionModel
            RowSelectionModel m_rowSelectionModel = new RowSelectionModel() { Mode = SelectionMode.Simple, AllowDeselect = true };
            m_gridpanel.SelectionModel.Add(m_rowSelectionModel);

            //Store         
            Store m_store = new Store();
            Model m_model = new Model();
            ModelField m_ModelField;
            
            List<ColumnBase> m_ListColumn = new List<ColumnBase>();;
            ColumnBase m_objColumn;
            CheckColumn c_objColumn;

            int index_c = 0;

            foreach (DataColumn dc in ListItemComparation.Columns)
            {
                m_ModelField = new ModelField(dc.ColumnName);
                m_model.Fields.Add(m_ModelField);

                if (index_c >= 4)
                {
                    if (index_c % 2 == 0)
                    {
                        c_objColumn = new CheckColumn { ID = dc.ColumnName, DataIndex = dc.ColumnName, Editable = true, Width = 20 };
                        m_ListColumn.Add(c_objColumn);
                    }
                    else
                    {
                        m_objColumn = new Column { Text = dc.ColumnName, DataIndex = dc.ColumnName, Flex = 1 };
                        m_objColumn.Renderer.Fn = "Ext.util.Format.numberRenderer('0,0')";
                        m_ListColumn.Add(m_objColumn);
                    }
                }
                else
                {
                    if (dc.ColumnName.ToString() == uom_id)
                    {
                        m_objColumn = new Column { Text = dc.ColumnName, DataIndex = dc.ColumnName, Flex = 1, Visible = false };
                        m_ListColumn.Add(m_objColumn);
                    }
                    else if (dc.ColumnName.ToString() == item_price_id)
                    {
                        m_objColumn = new Column { Text = dc.ColumnName, DataIndex = dc.ColumnName, Flex = 1, Visible = false };
                        m_ListColumn.Add(m_objColumn);
                    }
                    else
                    {
                        m_objColumn = new Column { Text = dc.ColumnName, DataIndex = dc.ColumnName, Flex = 1 };
                        m_ListColumn.Add(m_objColumn);
                    }
                }

                index_c++;
            }
            
            m_store.Model.Add(m_model);
            m_store.DataSource = ListItemComparation;
            m_gridpanel.Store.Add(m_store);
            
            m_gridpanel.ColumnModel.Columns.AddRange(m_ListColumn);
            m_gridpanel.Listeners.Select.Handler = "App.BtnSubmit.setDisabled(0);App.BtnSave.setDisabled(0);App.BtnGroup.setDisabled(0);App.BtnUngroup.setDisabled(0);App.BtnDelete.setDisabled(0)";
            
            //Plugin
            CellEditing m_objCellEditing = new CellEditing() { ClicksToEdit = 1 };
            m_objCellEditing.Listeners.BeforeEdit.Fn = "function (editor, e){ App.grdItemComparison.getSelectionModel().select(e.record); }";
            m_gridpanel.Plugins.Add(m_objCellEditing);
            
            return m_gridpanel;
        }
        private DataTable GetComparation(string vendorID)
        {
            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            Dictionary<string, List<object>> m_objFilter_item = new Dictionary<string, List<object>>();
            
            DItemComparisonDetailsDA m_objDItemComparisonDetailsDA = new DItemComparisonDetailsDA();
            m_objDItemComparisonDetailsDA.ConnectionStringName = Global.ConnStrConfigName;
            
            m_lstFilter = new List<object>();
            m_objFilter = new Dictionary<string, List<object>>();

            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(vendorID);
            m_objFilter.Add(VendorVM.Prop.VendorID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_objFilter_item = new Dictionary<string, List<object>>();

            List<ItemComparisonDetailsVM> m_objSessionData = JSON.Deserialize<List<ItemComparisonDetailsVM>>(Session[dataSessionName].ToString());

            string where = "";

            foreach (var item in m_objSessionData)
            {
                if (where != "")
                    where += " OR ";

                where += String.Format("(DItemPrice.ItemPriceID = '{0}' AND MVendor.VendorID = '{1}') ", item.ItemPriceID.ToString(), item.VendorID.ToString());
            }

            if (IsComparisonSessionExists(Session.SessionID.ToString()))
            {
                where += String.Format(" OR ( DItemComparisonSession.SessionID = '{0}') ", Session.SessionID.ToString());
            }

            m_lstFilter.Add(Operator.None);
            m_lstFilter.Add(String.Empty);
            m_objFilter_item.Add(where, m_lstFilter);

            Dictionary<int, DataSet> DItemComparisonDetailsDA = m_objDItemComparisonDetailsDA.SelectBC(m_objFilter, m_objFilter_item, null);

            return DItemComparisonDetailsDA[0].Tables[0];
        }
        private bool IsComparisonSessionExists(string sessionID)
        {
            bool isExists;

            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            DItemComparisonSessionDA m_objDItemComparisonSessionDA = new DItemComparisonSessionDA();
            m_objDItemComparisonSessionDA.ConnectionStringName = Global.ConnStrConfigName;

            m_lstFilter = new List<object>();
            m_objFilter = new Dictionary<string, List<object>>();

            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(sessionID);
            m_objFilter.Add(ItemComparisonSessionVM.Prop.SessionID.Map, m_lstFilter);

            Dictionary<int, DataSet> DItemComparisonSessionDA = m_objDItemComparisonSessionDA.SelectBC(0, null, false, null, m_objFilter,null, null, null, null);
            
            if (DItemComparisonSessionDA[0].Tables[0].Rows.Count > 0)
            {
                isExists = true;
            }
            else
            {
                isExists = false;
            }

            return isExists;
        }
        #endregion

        #region Public Method
        public ActionResult GetNodeAppend(string a, string b, string c, string d)
        {
            string thisItemDesc = string.Empty;
            string thisItem = string.Empty;
            string ItemComparisonDetailID = string.Empty;
            Node m_node = new Node();

            List<string> m_lstSelectedRowss = new List<string>();
            m_lstSelectedRowss = JSON.Deserialize<List<string>>(this.Request.Params["ItemID"]);
            
            FilterHeaderConditions m_fhcMBudgetPlanTemplate = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            
            ItemPriceVendorPeriodVM m_objItemPriceVendorPeriod = new ItemPriceVendorPeriodVM();
            Dictionary<string, List<object>> m_objFiltersFirst = new Dictionary<string, List<object>>();
            //DItemVersionDA m_objVersion = new DItemVersionDA();
            MItemDA m_objMItem = new MItemDA();
            m_objMItem.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstSelectsFirst = new List<string>();
            m_lstSelectsFirst.Add(ItemVM.Prop.ItemDesc.Map);
            m_lstSelectsFirst.Add(ItemVM.Prop.ItemID.Map);

            List<object> m_lstFilterss = new List<object>();
            m_lstFilterss.Add(Operator.Equals);
            m_lstFilterss.Add(thisItem);
            m_objFiltersFirst.Add(ItemVM.Prop.ItemID.Map, m_lstFilterss);


            Dictionary<int, DataSet> m_dicMBudgetPlanTemplateDA = m_objMItem.SelectBC(0, null, false, m_lstSelectsFirst, m_objFiltersFirst, null, null, null, null);
            if (m_objMItem.Message == string.Empty)
            {
                DataRow m_dataRow = m_dicMBudgetPlanTemplateDA[0].Tables[0].Rows[0];

                ItemVM m_objItem = new ItemVM();
                m_objItem.ItemDesc = m_dataRow[ItemVM.Prop.ItemDesc.Name].ToString();
                m_objItem.ItemID = m_dataRow[ItemVM.Prop.ItemID.Name].ToString();
                //m_objItem.Version = Convert.ToInt16(m_dataRow[ItemVersionVM.Prop.Version.Name].ToString());
                m_objItem.ItemTypeID = m_dataRow[ItemVersionVM.Prop.ItemTypeID.Name].ToString();
                m_objItem.ItemGroupDesc = m_dataRow[ItemVersionVM.Prop.ItemGroupDesc.Name].ToString();


                m_objItemPriceVendorPeriod.ItemDesc = m_objItem.ItemDesc;
                m_objItemPriceVendorPeriod.ItemID = m_objItem.ItemID;
                m_objItemPriceVendorPeriod.ItemTypeID = m_objItem.ItemTypeID;
                m_objItemPriceVendorPeriod.ItemGroupDesc = m_objItem.ItemGroupDesc;
                m_objItemPriceVendorPeriod.ItemPriceID = ItemComparisonDetailID;
                
                m_node = createNodeVersion(m_objItemPriceVendorPeriod, false);//, isdefaultCheckval);

                #region CheckChildren
                //CheckChildren
                m_node = CheckChildren(m_objItemPriceVendorPeriod.ItemID, m_objItemPriceVendorPeriod.ItemTypeID, ItemComparisonDetailID, m_node);
                if (m_node.Children.Count > 0)
                    m_node.Expandable = true;
                else
                    m_node.Expandable = false;
                #endregion
            }
            else
            {
                m_node = null;
            }
            return this.Store(m_node);
        }
        public ActionResult GetNodeAppendNew()
        {
            string thisItemDesc = string.Empty;
            string thisItem = string.Empty;
            //string thisItemTypeId = string.Empty;
            //string ParentItem = string.Empty;
            //int thisVersion=0;
            //string ParentItemTypeID = string.Empty;
            //int ParentVersion=0; int ParentSequence=0; int Sequence=0;
            //string BudgetTemplateID = string.Empty;
            //bool isdefaultCheck=true;
            Node m_node = new Node();

            bool isdefaultCheckval = false;
            bool isEnableDefault = false;


            List<string> m_lstSelectedRow = new List<string>();
            m_lstSelectedRow = JSON.Deserialize<List<string>>(this.Request.Params["ItemID"]);

            List<string> m_lstMessage = new List<string>();
            if (!m_lstSelectedRow.Any())
                m_lstMessage.Add("Some of Item Can't be Found");

            ItemPriceVendorPeriodVM m_objParentItemPriceVendorPeriod = new ItemPriceVendorPeriodVM();
            Dictionary<string, List<object>> m_objFiltersFirst = new Dictionary<string, List<object>>();
            MItemDA m_objVersion = new MItemDA();
            m_objVersion.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstSelectsFirst = new List<string>();
            m_lstSelectsFirst.Add(ItemVM.Prop.ItemDesc.Map);
            m_lstSelectsFirst.Add(ItemVM.Prop.ItemID.Map);
            m_lstSelectsFirst.Add(ItemVM.Prop.Version.Map);
            m_lstSelectsFirst.Add(ItemVM.Prop.ItemTypeID.Map);
            m_lstSelectsFirst.Add(ItemVM.Prop.ItemGroupDesc.Map);

            List<object> m_lstFilterss = new List<object>();
            m_lstFilterss.Add(Operator.Equals);
            m_lstFilterss.Add(thisItem);
            m_objFiltersFirst.Add(ItemVM.Prop.ItemID.Map, m_lstFilterss);

            //List<object> m_lstFiltersv = new List<object>();
            //m_lstFiltersv.Add(Operator.Equals);
            //m_lstFiltersv.Add(thisVersion);
            //m_objFiltersFirst.Add(ItemVM.Prop.Version.Map, m_lstFiltersv);

            Dictionary<int, DataSet> m_dicMBudgetPlanTemplateDA = m_objVersion.SelectBC(0, null, false, m_lstSelectsFirst, m_objFiltersFirst, null, null, null, null);
            if (m_objVersion.Message == string.Empty)
            {
                DataRow m_dataRow = m_dicMBudgetPlanTemplateDA[0].Tables[0].Rows[0];

                ItemVM m_objitemVersion = new ItemVM();
                m_objitemVersion.ItemDesc = m_dataRow[ItemVM.Prop.ItemDesc.Name].ToString();
                m_objitemVersion.ItemID = m_dataRow[ItemVM.Prop.ItemID.Name].ToString();
                m_objitemVersion.Version = Convert.ToInt16(m_dataRow[ItemVM.Prop.Version.Name].ToString());
                m_objitemVersion.ItemTypeID = m_dataRow[ItemVM.Prop.ItemTypeID.Name].ToString();
                m_objitemVersion.ItemGroupDesc = m_dataRow[ItemVM.Prop.ItemGroupDesc.Name].ToString();

                m_objParentItemPriceVendorPeriod.ItemDesc = m_objitemVersion.ItemDesc;
                m_objParentItemPriceVendorPeriod.ItemTypeID = m_objitemVersion.ItemTypeID;
                m_objParentItemPriceVendorPeriod.ItemGroupDesc = m_objitemVersion.ItemGroupDesc;
                m_objParentItemPriceVendorPeriod.ItemID = m_objitemVersion.ItemID;
                //m_objParentItemComparisonDetails.Version = m_objitemVersion.Version;

                //public string BudgetPlanTemplateID = ItemPriceID
                //public string ItemID = VendorID
                //public int Version = TaskID
                //public int Sequence = StatusID

                //m_objParentItemComparisonDetails.ItemDesc = m_objitemVersion.ItemDesc;
                //m_objParentItemComparisonDetails.ItemTypeID = m_objitemVersion.ItemTypeID;
                //m_objParentItemComparisonDetails.ItemGroupDesc = m_objitemVersion.ItemGroupDesc;
                //m_objParentItemComparisonDetails.ItemID = m_objitemVersion.ItemID;
                //m_objParentItemComparisonDetails.Version = m_objitemVersion.Version;
                //m_objParentItemComparisonDetails.BudgetPlanTemplateID = BudgetTemplateID;
                //m_objParentItemComparisonDetails.Sequence = Sequence + 1;
                //m_objParentItemPriceVendorPeriod.ParentItemID = ParentItem;
                //m_objParentItemPriceVendorPeriod.ParentVersion = ParentVersion;
                //m_objParentItemPriceVendorPeriod.ParentSequence = ParentSequence;
                //m_objParentItemPriceVendorPeriod.ParentItemTypeID = ParentItemTypeID;


                //m_objParentItemComparisonDetails.EnableDefault = isEnableDefault;
                //Sequence++;
                m_node = createNodeVersion(m_objParentItemPriceVendorPeriod, false);//, isdefaultCheckval);

                #region CheckChildren
                ////CheckChildren
                ////m_node = CheckChildren(m_objParentItemPriceVendorPeriod.ItemID, m_objParentItemPriceVendorPeriod.ItemTypeID, m_objParentItemPriceVendorPeriod.Version, ref Sequence, BudgetTemplateID, m_node, m_lstUPA, m_lstUPAAHS, ref isEnableDefault, ref isdefaultCheck);
                ////private Node CheckChildren(string ParentItemID, string ParentItemTypeID, ref int Sequence, string BudgetTemplateID, Node ParentNode, ref bool isEnableDefault, ref bool isdefaultcheck)
                //m_node = CheckChildren(m_objParentItemPriceVendorPeriod.ItemID, m_objParentItemPriceVendorPeriod.ItemTypeID, m_node, ref isEnableDefault);//, ref Sequence, BudgetTemplateID, ref isdefaultCheck);
                //if (m_node.Children.Count > 0)
                //    m_node.Expandable = true;
                //else
                //    m_node.Expandable = false;
                #endregion
            }
            else
            {
                m_node = null;
            }

            return this.Store(m_node);
        }
        private Node createNodeVersion(ItemPriceVendorPeriodVM parent, bool isFromSelect)//, bool isdefaultCheck)
        {
            Node m_node = new Node();

            m_node.Icon = Icon.Folder;
            m_node.Text = parent.ItemID;
            m_node.AttributesObject = new
            {
                number = "",
                itemdesc = parent.ItemDesc,
                itemid = parent.ItemID,
                itemgroupdesc = parent.ItemGroupDesc,
                itemtypeid = parent.ItemTypeID
            };

            return m_node;
        }
        private Node CheckChildren(string ParentItemID, string ParentItemTypeID, string ItemComparisonDetailID, Node ParentNode)//, ref bool isEnableDefault, ref int Sequence, string BudgetTemplateID, ref bool isdefaultcheck)
        {
            bool isdefaultval = false;

            ItemPriceVendorPeriodVM m_objParentItemPriceVendorPeriod = new ItemPriceVendorPeriodVM();
            Node m_node = new Node();
            Dictionary<string, List<object>> m_objFiltersFirst = new Dictionary<string, List<object>>();
            List<string> m_lstSelectsFirst = new List<string>();

            m_lstSelectsFirst.Add(ItemVM.Prop.ItemDesc.Map);
            m_lstSelectsFirst.Add(ItemVM.Prop.ItemID.Map);
            m_lstSelectsFirst.Add(ItemVM.Prop.Version.Map);
            m_lstSelectsFirst.Add(ItemVM.Prop.ItemTypeID.Map);
            m_lstSelectsFirst.Add(ItemVM.Prop.ItemGroupDesc.Map);

            List<object> m_lstFilterss = new List<object>();
            m_lstFilterss.Add(Operator.Equals);
            m_lstFilterss.Add(ParentItemID);
            m_objFiltersFirst.Add(ItemVersionChildVM.Prop.ItemID.Map, m_lstFilterss);

            DItemVersionChildDA m_objVersionChild = new DItemVersionChildDA();
            m_objVersionChild.ConnectionStringName = Global.ConnStrConfigName;

            Dictionary<int, DataSet> m_dicMBudgetPlanTemplateChildDA = m_objVersionChild.SelectBC(0, null, false, m_lstSelectsFirst, m_objFiltersFirst, null, null, null, null);
            if (m_objVersionChild.Message == string.Empty && m_objVersionChild.AffectedRows > 0)
            {

                foreach (DataRow m_dataRow in m_dicMBudgetPlanTemplateChildDA[0].Tables[0].Rows)
                {
                    //bool enabledefaultForThis = false;
                    ItemVersionVM m_objitemVersion = new ItemVersionVM();
                    m_objitemVersion.ItemDesc = m_dataRow[ItemVersionChildVM.Prop.ItemDesc.Name].ToString();
                    m_objitemVersion.ItemID = m_dataRow[ItemVersionChildVM.Prop.ChildItemID.Name].ToString();
                    m_objitemVersion.Version = Convert.ToInt16(m_dataRow[ItemVersionChildVM.Prop.ChildVersion.Name].ToString());
                    m_objitemVersion.ItemTypeID = m_dataRow[ItemVersionChildVM.Prop.ItemTypeID.Name].ToString();
                    m_objitemVersion.ItemGroupDesc = m_dataRow[ItemVersionChildVM.Prop.ItemGroupDesc.Name].ToString();

                    m_objParentItemPriceVendorPeriod.ItemDesc = m_objitemVersion.ItemDesc;
                    m_objParentItemPriceVendorPeriod.ItemID = m_objitemVersion.ItemID;
                    m_objParentItemPriceVendorPeriod.ItemTypeID = m_objitemVersion.ItemTypeID;
                    m_objParentItemPriceVendorPeriod.ItemGroupDesc = m_objitemVersion.ItemGroupDesc;

                    m_objParentItemPriceVendorPeriod.IsDefault = false;
                    //Sequence++;
                    m_node = createNodeVersion(m_objParentItemPriceVendorPeriod, false);//, isdefaultval);

                    #region CheckChild_Loop
                    m_node = CheckChildren(m_objParentItemPriceVendorPeriod.ItemID, m_objParentItemPriceVendorPeriod.ItemTypeID, ItemComparisonDetailID, m_node);//, m_node, ref isEnableDefault);//, ref Sequence, BudgetTemplateID, ref isdefaultcheck);
                    ParentNode.Children.Add(m_node);
                    ParentNode.Expanded = true;
                    #endregion

                }
            }
            else
            {
                //ParentNode.Leaf = true;
            }
            return ParentNode;
        }
        
        #endregion

    }
}