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
    public class RegionController : BaseController
    {
        private readonly string title = "Region";
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
            MRegionDA m_objMRegionDA = new MRegionDA();
            m_objMRegionDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMRegion = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMRegion.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = RegionVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMRegionDA = m_objMRegionDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpRegionBL in m_dicMRegionDA)
            {
                m_intCount = m_kvpRegionBL.Key;
                break;
            }

            List<RegionVM> m_lstRegionVM = new List<RegionVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(RegionVM.Prop.RegionID.MapAlias);
                m_lstSelect.Add(RegionVM.Prop.RegionDesc.MapAlias);
                m_lstSelect.Add(RegionVM.Prop.CountryDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(RegionVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMRegionDA = m_objMRegionDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMRegionDA.Message == string.Empty)
                {
                    m_lstRegionVM = (
                        from DataRow m_drMRegionDA in m_dicMRegionDA[0].Tables[0].Rows
                        select new RegionVM()
                        {
                            RegionID = m_drMRegionDA[RegionVM.Prop.RegionID.Name].ToString(),
                            RegionDesc = m_drMRegionDA[RegionVM.Prop.RegionDesc.Name].ToString(),
                            CountryDesc = m_drMRegionDA[RegionVM.Prop.CountryDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstRegionVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            MRegionDA m_objMRegionDA = new MRegionDA();
            m_objMRegionDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMRegion = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMRegion.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = RegionVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMRegionDA = m_objMRegionDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpRegionBL in m_dicMRegionDA)
            {
                m_intCount = m_kvpRegionBL.Key;
                break;
            }

            List<RegionVM> m_lstRegionVM = new List<RegionVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(RegionVM.Prop.RegionID.MapAlias);
                m_lstSelect.Add(RegionVM.Prop.RegionDesc.MapAlias);
                m_lstSelect.Add(RegionVM.Prop.CountryDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(RegionVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMRegionDA = m_objMRegionDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMRegionDA.Message == string.Empty)
                {
                    m_lstRegionVM = (
                        from DataRow m_drMRegionDA in m_dicMRegionDA[0].Tables[0].Rows
                        select new RegionVM()
                        {
                            RegionID = m_drMRegionDA[RegionVM.Prop.RegionID.Name].ToString(),
                            RegionDesc = m_drMRegionDA[RegionVM.Prop.RegionDesc.Name].ToString(),
                            CountryDesc = m_drMRegionDA[RegionVM.Prop.CountryDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstRegionVM, m_intCount);
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

            RegionVM m_objRegionVM = new RegionVM();
            ViewDataDictionary m_vddRegion = new ViewDataDictionary();
            m_vddRegion.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddRegion.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
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
                Model = m_objRegionVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddRegion,
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
            RegionVM m_objRegionVM = new RegionVM();
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
                m_objRegionVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddRegion = new ViewDataDictionary();
            m_vddRegion.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objRegionVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddRegion,
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
            RegionVM m_objRegionVM = new RegionVM();
            if (m_dicSelectedRow.Count > 0)
                m_objRegionVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddRegion = new ViewDataDictionary();
            m_vddRegion.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddRegion.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objRegionVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddRegion,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<RegionVM> m_lstSelectedRow = new List<RegionVM>();
            m_lstSelectedRow = JSON.Deserialize<List<RegionVM>>(Selected);

            MRegionDA m_objMRegionDA = new MRegionDA();
            m_objMRegionDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (RegionVM m_objRegionVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifRegionVM = m_objRegionVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifRegionVM in m_arrPifRegionVM)
                    {
                        string m_strFieldName = m_pifRegionVM.Name;
                        object m_objFieldValue = m_pifRegionVM.GetValue(m_objRegionVM);
                        if (m_objRegionVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(RegionVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objMRegionDA.DeleteBC(m_objFilter, false);
                    if (m_objMRegionDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMRegionDA.Message);
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

        public ActionResult Browse(string ControlRegionID, string ControlRegionDesc, string FilterRegionID = "", string FilterRegionDesc = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddRegion = new ViewDataDictionary();
            m_vddRegion.Add("Control" + RegionVM.Prop.RegionID.Name, ControlRegionID);
            m_vddRegion.Add("Control" + RegionVM.Prop.RegionDesc.Name, ControlRegionDesc);
            m_vddRegion.Add(RegionVM.Prop.RegionID.Name, FilterRegionID);
            m_vddRegion.Add(RegionVM.Prop.RegionDesc.Name, FilterRegionDesc);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddRegion,
                ViewName = "../Region/_Browse"
            };
        }

        public ActionResult Save(string Action)
        {
            //action = JSON.Deserialize<string>(action);
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MRegionDA m_objMRegionDA = new MRegionDA();
            m_objMRegionDA.ConnectionStringName = Global.ConnStrConfigName;
            try
            {
                string m_strRegionID = this.Request.Params[RegionVM.Prop.RegionID.Name];
                string m_strRegionDesc = this.Request.Params[RegionVM.Prop.RegionDesc.Name];
                string m_strCountryID = this.Request.Params[CountryVM.Prop.CountryID.Name];

                m_lstMessage = IsSaveValid(Action, m_strRegionID, m_strRegionDesc, m_strCountryID);
                if (m_lstMessage.Count <= 0)
                {
                    MRegion m_objMRegion = new MRegion();
                    m_objMRegion.RegionID = m_strRegionID;
                    m_objMRegionDA.Data = m_objMRegion;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objMRegionDA.Select();

                    m_objMRegion.RegionDesc = m_strRegionDesc;
                    m_objMRegion.CountryID = m_strCountryID;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objMRegionDA.Insert(false);
                    else
                        m_objMRegionDA.Update(false);

                    if (!m_objMRegionDA.Success || m_objMRegionDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMRegionDA.Message);
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

        public ActionResult GetRegion(string ControlRegionID, string ControlRegionDesc, string FilterRegionID, string FilterRegionDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<RegionVM>> m_dicRegionData = GetRegionData(true, FilterRegionID, FilterRegionDesc);
                KeyValuePair<int, List<RegionVM>> m_kvpRegionVM = m_dicRegionData.AsEnumerable().ToList()[0];
                if (m_kvpRegionVM.Key < 1 || (m_kvpRegionVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpRegionVM.Key > 1 && !Exact)
                    return Browse(ControlRegionID, ControlRegionDesc, FilterRegionID, FilterRegionDesc);

                m_dicRegionData = GetRegionData(false, FilterRegionID, FilterRegionDesc);
                RegionVM m_objRegionVM = m_dicRegionData[0][0];
                this.GetCmp<TextField>(ControlRegionID).Value = m_objRegionVM.RegionID;
                this.GetCmp<TextField>(ControlRegionDesc).Value = m_objRegionVM.RegionDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, string RegionID, string RegionDesc, string CountryID)
        {
            List<string> m_lstReturn = new List<string>();

            if (RegionID == string.Empty)
                m_lstReturn.Add(RegionVM.Prop.RegionID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (RegionDesc == string.Empty)
                m_lstReturn.Add(RegionVM.Prop.RegionDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (CountryID == string.Empty)
                m_lstReturn.Add(RegionVM.Prop.CountryID.Desc + " " + General.EnumDesc(MessageLib.mustFill));

            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(RegionVM.Prop.RegionID.Name, parameters[RegionVM.Prop.RegionID.Name]);
            m_dicReturn.Add(RegionVM.Prop.RegionDesc.Name, parameters[RegionVM.Prop.RegionDesc.Name]);
            m_dicReturn.Add(RegionVM.Prop.CountryID.Name, parameters[RegionVM.Prop.CountryID.Name]);
            m_dicReturn.Add(RegionVM.Prop.CountryDesc.Name, parameters[RegionVM.Prop.CountryDesc.Name]);

            return m_dicReturn;
        }

        private RegionVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            RegionVM m_objRegionVM = new RegionVM();
            MRegionDA m_objMRegionDA = new MRegionDA();
            m_objMRegionDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(RegionVM.Prop.RegionID.MapAlias);
            m_lstSelect.Add(RegionVM.Prop.RegionDesc.MapAlias);
            m_lstSelect.Add(RegionVM.Prop.CountryID.MapAlias);
            m_lstSelect.Add(RegionVM.Prop.CountryDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objRegionVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(RegionVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMRegionDA = m_objMRegionDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMRegionDA.Message == string.Empty)
            {
                DataRow m_drMRegionDA = m_dicMRegionDA[0].Tables[0].Rows[0];
                m_objRegionVM.RegionID = m_drMRegionDA[RegionVM.Prop.RegionID.Name].ToString();
                m_objRegionVM.RegionDesc = m_drMRegionDA[RegionVM.Prop.RegionDesc.Name].ToString();
                m_objRegionVM.CountryID = m_drMRegionDA[CountryVM.Prop.CountryID.Name].ToString();
                m_objRegionVM.CountryDesc = m_drMRegionDA[CountryVM.Prop.CountryDesc.Name].ToString();
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMRegionDA.Message;

            return m_objRegionVM;
        }

        #endregion

        #region Public Method

        public Dictionary<int, List<RegionVM>> GetRegionData(bool isCount, string RegionID, string RegionDesc)
        {
            int m_intCount = 0;
            List<RegionVM> m_lstRegionVM = new List<ViewModels.RegionVM>();
            Dictionary<int, List<RegionVM>> m_dicReturn = new Dictionary<int, List<RegionVM>>();
            MRegionDA m_objMRegionDA = new MRegionDA();
            m_objMRegionDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(RegionVM.Prop.RegionID.MapAlias);
            m_lstSelect.Add(RegionVM.Prop.RegionDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(RegionID);
            m_objFilter.Add(RegionVM.Prop.RegionID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(RegionDesc);
            m_objFilter.Add(RegionVM.Prop.RegionDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMRegionDA = m_objMRegionDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMRegionDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpRegionBL in m_dicMRegionDA)
                    {
                        m_intCount = m_kvpRegionBL.Key;
                        break;
                    }
                else
                {
                    m_lstRegionVM = (
                        from DataRow m_drMRegionDA in m_dicMRegionDA[0].Tables[0].Rows
                        select new RegionVM()
                        {
                            RegionID = m_drMRegionDA[RegionVM.Prop.RegionID.Name].ToString(),
                            RegionDesc = m_drMRegionDA[RegionVM.Prop.RegionDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstRegionVM);
            return m_dicReturn;
        }

        #endregion
    }
}