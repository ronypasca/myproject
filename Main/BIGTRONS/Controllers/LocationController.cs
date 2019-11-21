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
    public class LocationController : BaseController
    {
        private readonly string title = "Location";
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
            MLocationDA m_objMLocationDA = new MLocationDA();
            m_objMLocationDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMLocation = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMLocation.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = LocationVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMLocationDA = m_objMLocationDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpLocationBL in m_dicMLocationDA)
            {
                m_intCount = m_kvpLocationBL.Key;
                break;
            }

            List<LocationVM> m_lstLocationVM = new List<LocationVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(LocationVM.Prop.LocationID.MapAlias);
                m_lstSelect.Add(LocationVM.Prop.LocationDesc.MapAlias);
                m_lstSelect.Add(LocationVM.Prop.RegionDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(LocationVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMLocationDA = m_objMLocationDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMLocationDA.Message == string.Empty)
                {
                    m_lstLocationVM = (
                        from DataRow m_drMLocationDA in m_dicMLocationDA[0].Tables[0].Rows
                        select new LocationVM()
                        {
                            LocationID = m_drMLocationDA[LocationVM.Prop.LocationID.Name].ToString(),
                            LocationDesc = m_drMLocationDA[LocationVM.Prop.LocationDesc.Name].ToString(),
                            RegionDesc = m_drMLocationDA[LocationVM.Prop.RegionDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstLocationVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            MLocationDA m_objMLocationDA = new MLocationDA();
            m_objMLocationDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMLocation = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMLocation.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = LocationVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMLocationDA = m_objMLocationDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpLocationBL in m_dicMLocationDA)
            {
                m_intCount = m_kvpLocationBL.Key;
                break;
            }

            List<LocationVM> m_lstLocationVM = new List<LocationVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(LocationVM.Prop.LocationID.MapAlias);
                m_lstSelect.Add(LocationVM.Prop.LocationDesc.MapAlias);
                m_lstSelect.Add(LocationVM.Prop.RegionDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(LocationVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMLocationDA = m_objMLocationDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMLocationDA.Message == string.Empty)
                {
                    m_lstLocationVM = (
                        from DataRow m_drMLocationDA in m_dicMLocationDA[0].Tables[0].Rows
                        select new LocationVM()
                        {
                            LocationID = m_drMLocationDA[LocationVM.Prop.LocationID.Name].ToString(),
                            LocationDesc = m_drMLocationDA[LocationVM.Prop.LocationDesc.Name].ToString(),
                            RegionDesc = m_drMLocationDA[LocationVM.Prop.RegionDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstLocationVM, m_intCount);
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

            LocationVM m_objLocationVM = new LocationVM();
            ViewDataDictionary m_vddLocation = new ViewDataDictionary();
            m_vddLocation.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddLocation.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
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
                Model = m_objLocationVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddLocation,
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
            LocationVM m_objLocationVM = new LocationVM();
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
                m_objLocationVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddLocation = new ViewDataDictionary();
            m_vddLocation.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objLocationVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddLocation,
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
            LocationVM m_objLocationVM = new LocationVM();
            if (m_dicSelectedRow.Count > 0)
                m_objLocationVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddLocation = new ViewDataDictionary();
            m_vddLocation.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddLocation.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objLocationVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddLocation,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<LocationVM> m_lstSelectedRow = new List<LocationVM>();
            m_lstSelectedRow = JSON.Deserialize<List<LocationVM>>(Selected);

            MLocationDA m_objMLocationDA = new MLocationDA();
            m_objMLocationDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (LocationVM m_objLocationVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifLocationVM = m_objLocationVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifLocationVM in m_arrPifLocationVM)
                    {
                        string m_strFieldName = m_pifLocationVM.Name;
                        object m_objFieldValue = m_pifLocationVM.GetValue(m_objLocationVM);
                        if (m_objLocationVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(LocationVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objMLocationDA.DeleteBC(m_objFilter, false);
                    if (m_objMLocationDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMLocationDA.Message);
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

        public ActionResult Browse(string ControlLocationID, string ControlLocationDesc, string FilterLocationID = "", string FilterLocationDesc = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddLocation = new ViewDataDictionary();
            m_vddLocation.Add("Control" + LocationVM.Prop.LocationID.Name, ControlLocationID);
            m_vddLocation.Add("Control" + LocationVM.Prop.LocationDesc.Name, ControlLocationDesc);
            m_vddLocation.Add(LocationVM.Prop.LocationID.Name, FilterLocationID);
            m_vddLocation.Add(LocationVM.Prop.LocationDesc.Name, FilterLocationDesc);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddLocation,
                ViewName = "../Location/_Browse"
            };
        }

        public ActionResult Save(string Action)
        {
            //action = JSON.Deserialize<string>(action);
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MLocationDA m_objMLocationDA = new MLocationDA();
            m_objMLocationDA.ConnectionStringName = Global.ConnStrConfigName;
            try
            {
                string m_strLocationID = this.Request.Params[LocationVM.Prop.LocationID.Name];
                string m_strLocationDesc = this.Request.Params[LocationVM.Prop.LocationDesc.Name];
                string m_strRegionID = this.Request.Params[RegionVM.Prop.RegionID.Name];

                m_lstMessage = IsSaveValid(Action, m_strLocationID, m_strLocationDesc, m_strRegionID);
                if (m_lstMessage.Count <= 0)
                {
                    MLocation m_objMLocation = new MLocation();
                    m_objMLocation.LocationID = m_strLocationID;
                    m_objMLocationDA.Data = m_objMLocation;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objMLocationDA.Select();

                    m_objMLocation.LocationDesc = m_strLocationDesc;
                    m_objMLocation.RegionID = m_strRegionID;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objMLocationDA.Insert(false);
                    else
                        m_objMLocationDA.Update(false);

                    if (!m_objMLocationDA.Success || m_objMLocationDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMLocationDA.Message);
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

        public ActionResult GetLocation(string ControlLocationID, string ControlLocationDesc, string FilterLocationID, string FilterLocationDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<LocationVM>> m_dicLocationData = GetLocationData(true, FilterLocationID, FilterLocationDesc);
                KeyValuePair<int, List<LocationVM>> m_kvpLocationVM = m_dicLocationData.AsEnumerable().ToList()[0];
                if (m_kvpLocationVM.Key < 1 || (m_kvpLocationVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpLocationVM.Key > 1 && !Exact)
                    return Browse(ControlLocationID, ControlLocationDesc, FilterLocationID, FilterLocationDesc);

                m_dicLocationData = GetLocationData(false, FilterLocationID, FilterLocationDesc);
                LocationVM m_objLocationVM = m_dicLocationData[0][0];
                this.GetCmp<TextField>(ControlLocationID).Value = m_objLocationVM.LocationID;
                this.GetCmp<TextField>(ControlLocationDesc).Value = m_objLocationVM.LocationDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, string LocationID, string LocationDesc, string RegionID)
        {
            List<string> m_lstReturn = new List<string>();

            if (LocationID == string.Empty)
                m_lstReturn.Add(LocationVM.Prop.LocationID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (LocationDesc == string.Empty)
                m_lstReturn.Add(LocationVM.Prop.LocationDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (RegionID == string.Empty)
                m_lstReturn.Add(LocationVM.Prop.RegionID.Desc + " " + General.EnumDesc(MessageLib.mustFill));

            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(LocationVM.Prop.LocationID.Name, parameters[LocationVM.Prop.LocationID.Name]);
            m_dicReturn.Add(LocationVM.Prop.LocationDesc.Name, parameters[LocationVM.Prop.LocationDesc.Name]);
            m_dicReturn.Add(LocationVM.Prop.RegionID.Name, parameters[LocationVM.Prop.RegionID.Name]);
            m_dicReturn.Add(LocationVM.Prop.RegionDesc.Name, parameters[LocationVM.Prop.RegionDesc.Name]);

            return m_dicReturn;
        }

        private LocationVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            LocationVM m_objLocationVM = new LocationVM();
            MLocationDA m_objMLocationDA = new MLocationDA();
            m_objMLocationDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(LocationVM.Prop.LocationID.MapAlias);
            m_lstSelect.Add(LocationVM.Prop.LocationDesc.MapAlias);
            m_lstSelect.Add(LocationVM.Prop.RegionID.MapAlias);
            m_lstSelect.Add(LocationVM.Prop.RegionDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objLocationVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(LocationVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMLocationDA = m_objMLocationDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMLocationDA.Message == string.Empty)
            {
                DataRow m_drMLocationDA = m_dicMLocationDA[0].Tables[0].Rows[0];
                m_objLocationVM.LocationID = m_drMLocationDA[LocationVM.Prop.LocationID.Name].ToString();
                m_objLocationVM.LocationDesc = m_drMLocationDA[LocationVM.Prop.LocationDesc.Name].ToString();
                m_objLocationVM.RegionID = m_drMLocationDA[RegionVM.Prop.RegionID.Name].ToString();
                m_objLocationVM.RegionDesc = m_drMLocationDA[RegionVM.Prop.RegionDesc.Name].ToString();
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMLocationDA.Message;

            return m_objLocationVM;
        }

        #endregion

        #region Public Method

        public Dictionary<int, List<LocationVM>> GetLocationData(bool isCount, string LocationID, string LocationDesc)
        {
            int m_intCount = 0;
            List<LocationVM> m_lstLocationVM = new List<ViewModels.LocationVM>();
            Dictionary<int, List<LocationVM>> m_dicReturn = new Dictionary<int, List<LocationVM>>();
            MLocationDA m_objMLocationDA = new MLocationDA();
            m_objMLocationDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(LocationVM.Prop.LocationID.MapAlias);
            m_lstSelect.Add(LocationVM.Prop.LocationDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(LocationID);
            m_objFilter.Add(LocationVM.Prop.LocationID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(LocationDesc);
            m_objFilter.Add(LocationVM.Prop.LocationDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMLocationDA = m_objMLocationDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMLocationDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpLocationBL in m_dicMLocationDA)
                    {
                        m_intCount = m_kvpLocationBL.Key;
                        break;
                    }
                else
                {
                    m_lstLocationVM = (
                        from DataRow m_drMLocationDA in m_dicMLocationDA[0].Tables[0].Rows
                        select new LocationVM()
                        {
                            LocationID = m_drMLocationDA[LocationVM.Prop.LocationID.Name].ToString(),
                            LocationDesc = m_drMLocationDA[LocationVM.Prop.LocationDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstLocationVM);
            return m_dicReturn;
        }

        #endregion
    }
}