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
    public class HolidayController : BaseController
    {
        private readonly string title = "Holiday";
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
            MHolidaysDA m_objMHolidaysDA = new MHolidaysDA();
            m_objMHolidaysDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMHolidays = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMHolidays.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = HolidayVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMHolidaysDA = m_objMHolidaysDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpHolidayBL in m_dicMHolidaysDA)
            {
                m_intCount = m_kvpHolidayBL.Key;
                break;
            }

            List<HolidayVM> m_lstHolidayVM = new List<HolidayVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(HolidayVM.Prop.HolidayID.MapAlias);
                m_lstSelect.Add(HolidayVM.Prop.Descriptions.MapAlias);
                m_lstSelect.Add(HolidayVM.Prop.HolidayDate.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(HolidayVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMHolidaysDA = m_objMHolidaysDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMHolidaysDA.Message == string.Empty)
                {
                    m_lstHolidayVM = (
                        from DataRow m_drMHolidaysDA in m_dicMHolidaysDA[0].Tables[0].Rows
                        select new HolidayVM()
                        {
                            HolidayID = m_drMHolidaysDA[HolidayVM.Prop.HolidayID.Name].ToString(),
                            Descriptions = m_drMHolidaysDA[HolidayVM.Prop.Descriptions.Name].ToString(),
                            HolidayDate = DateTime.Parse(m_drMHolidaysDA[HolidayVM.Prop.HolidayDate.Name].ToString())

                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstHolidayVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            MHolidaysDA m_objMHolidaysDA = new MHolidaysDA();
            m_objMHolidaysDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMHolidays = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMHolidays.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = HolidayVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMHolidaysDA = m_objMHolidaysDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpHolidayBL in m_dicMHolidaysDA)
            {
                m_intCount = m_kvpHolidayBL.Key;
                break;
            }

            List<HolidayVM> m_lstHolidayVM = new List<HolidayVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(HolidayVM.Prop.HolidayID.MapAlias);
                m_lstSelect.Add(HolidayVM.Prop.Descriptions.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(HolidayVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMHolidaysDA = m_objMHolidaysDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMHolidaysDA.Message == string.Empty)
                {
                    m_lstHolidayVM = (
                        from DataRow m_drMHolidaysDA in m_dicMHolidaysDA[0].Tables[0].Rows
                        select new HolidayVM()
                        {
                            HolidayID = m_drMHolidaysDA[HolidayVM.Prop.HolidayID.Name].ToString(),
                            Descriptions = m_drMHolidaysDA[HolidayVM.Prop.Descriptions.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstHolidayVM, m_intCount);
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

            HolidayVM m_objHolidayVM = new HolidayVM();
            ViewDataDictionary m_vddHoliday = new ViewDataDictionary();
            m_vddHoliday.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddHoliday.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
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
                Model = m_objHolidayVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddHoliday,
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
            HolidayVM m_objHolidayVM = new HolidayVM();
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
            if (Caller == General.EnumDesc(Buttons.ButtonList))
            {
                m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
            }
            else if (Caller == General.EnumDesc(Buttons.ButtonSave))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                m_dicSelectedRow = GetFormData(m_nvcParams);
            }

            if (m_dicSelectedRow.Count > 0)
                m_objHolidayVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddHoliday = new ViewDataDictionary();
            m_vddHoliday.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objHolidayVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddHoliday,
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
            HolidayVM m_objHolidayVM = new HolidayVM();
            if (m_dicSelectedRow.Count > 0)
                m_objHolidayVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddHoliday = new ViewDataDictionary();
            m_vddHoliday.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddHoliday.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objHolidayVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddHoliday,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<HolidayVM> m_lstSelectedRow = new List<HolidayVM>();
            m_lstSelectedRow = JSON.Deserialize<List<HolidayVM>>(Selected);

            MHolidaysDA m_objMHolidaysDA = new MHolidaysDA();
            m_objMHolidaysDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (HolidayVM m_objHolidayVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifHolidayVM = m_objHolidayVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifHolidayVM in m_arrPifHolidayVM)
                    {
                        string m_strFieldName = m_pifHolidayVM.Name;
                        object m_objFieldValue = m_pifHolidayVM.GetValue(m_objHolidayVM);
                        if (m_objHolidayVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(HolidayVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objMHolidaysDA.DeleteBC(m_objFilter, false);
                    if (m_objMHolidaysDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMHolidaysDA.Message);
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

        public ActionResult Browse(string ControlHolidayID, string ControlDescriptions, string FilterHolidayID = "", string FilterDescriptions = "")
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddHoliday = new ViewDataDictionary();
            m_vddHoliday.Add("Control" + HolidayVM.Prop.HolidayID.Name, ControlHolidayID);
            m_vddHoliday.Add("Control" + HolidayVM.Prop.Descriptions.Name, ControlDescriptions);
            m_vddHoliday.Add(HolidayVM.Prop.HolidayID.Name, FilterHolidayID);
            m_vddHoliday.Add(HolidayVM.Prop.Descriptions.Name, FilterDescriptions);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddHoliday,
                ViewName = "../Holiday/_Browse"
            };
        }

        public ActionResult Save(string Action)
        {
            //action = JSON.Deserialize<string>(action);
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MHolidaysDA m_objMHolidaysDA = new MHolidaysDA();
            m_objMHolidaysDA.ConnectionStringName = Global.ConnStrConfigName;
            try
            {
                string m_strHolidayID = this.Request.Params[HolidayVM.Prop.HolidayID.Name];
                string m_strDescriptions = this.Request.Params[HolidayVM.Prop.Descriptions.Name];
                string m_strHolidayDate = this.Request.Params[HolidayVM.Prop.HolidayDate.Name];

                m_lstMessage = IsSaveValid(Action, m_strHolidayID, m_strDescriptions);
                if (m_lstMessage.Count <= 0)
                {
                    MHolidays m_objMHolidays = new MHolidays();
                    m_objMHolidays.HolidayID = m_strHolidayID;
                    m_objMHolidaysDA.Data = m_objMHolidays;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objMHolidaysDA.Select();

                    m_objMHolidays.Descriptions = m_strDescriptions;
                    m_objMHolidays.HolidayDate = DateTime.Parse(m_strHolidayDate);

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objMHolidaysDA.Insert(false);
                    else
                        m_objMHolidaysDA.Update(false);

                    if (!m_objMHolidaysDA.Success || m_objMHolidaysDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMHolidaysDA.Message);
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

        public ActionResult GetHoliday(string ControlHolidayID, string ControlDescriptions, string FilterHolidayID, string FilterDescriptions, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<HolidayVM>> m_dicHolidayData = GetHolidayData(true, FilterHolidayID, FilterDescriptions);
                KeyValuePair<int, List<HolidayVM>> m_kvpHolidayVM = m_dicHolidayData.AsEnumerable().ToList()[0];
                if (m_kvpHolidayVM.Key < 1 || (m_kvpHolidayVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpHolidayVM.Key > 1 && !Exact)
                    return Browse(ControlHolidayID, ControlDescriptions, FilterHolidayID, FilterDescriptions);

                m_dicHolidayData = GetHolidayData(false, FilterHolidayID, FilterDescriptions);
                HolidayVM m_objHolidayVM = m_dicHolidayData[0][0];
                this.GetCmp<TextField>(ControlHolidayID).Value = m_objHolidayVM.HolidayID;
                this.GetCmp<TextField>(ControlDescriptions).Value = m_objHolidayVM.Descriptions;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, string HolidayID, string Descriptions)
        {
            List<string> m_lstReturn = new List<string>();

            if (HolidayID == string.Empty)
                m_lstReturn.Add(HolidayVM.Prop.HolidayID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (Descriptions == string.Empty)
                m_lstReturn.Add(HolidayVM.Prop.Descriptions.Desc + " " + General.EnumDesc(MessageLib.mustFill));

            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(HolidayVM.Prop.HolidayID.Name, parameters[HolidayVM.Prop.HolidayID.Name]);
            m_dicReturn.Add(HolidayVM.Prop.Descriptions.Name, parameters[HolidayVM.Prop.Descriptions.Name]);

            return m_dicReturn;
        }

        private HolidayVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            HolidayVM m_objHolidayVM = new HolidayVM();
            MHolidaysDA m_objMHolidaysDA = new MHolidaysDA();
            m_objMHolidaysDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(HolidayVM.Prop.HolidayID.MapAlias);
            m_lstSelect.Add(HolidayVM.Prop.Descriptions.MapAlias);
            m_lstSelect.Add(HolidayVM.Prop.HolidayDate.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objHolidayVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(HolidayVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMHolidaysDA = m_objMHolidaysDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMHolidaysDA.Message == string.Empty)
            {
                DataRow m_drMHolidaysDA = m_dicMHolidaysDA[0].Tables[0].Rows[0];
                m_objHolidayVM.HolidayID = m_drMHolidaysDA[HolidayVM.Prop.HolidayID.Name].ToString();
                m_objHolidayVM.Descriptions = m_drMHolidaysDA[HolidayVM.Prop.Descriptions.Name].ToString();
                m_objHolidayVM.HolidayDate = DateTime.Parse(m_drMHolidaysDA[HolidayVM.Prop.HolidayDate.Name].ToString());
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMHolidaysDA.Message;

            return m_objHolidayVM;
        }

        #endregion

        #region Public Method

        public Dictionary<int, List<HolidayVM>> GetHolidayData(bool isCount, string HolidayID, string Descriptions)
        {
            int m_intCount = 0;
            List<HolidayVM> m_lstHolidayVM = new List<ViewModels.HolidayVM>();
            Dictionary<int, List<HolidayVM>> m_dicReturn = new Dictionary<int, List<HolidayVM>>();
            MHolidaysDA m_objMHolidaysDA = new MHolidaysDA();
            m_objMHolidaysDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(HolidayVM.Prop.HolidayID.MapAlias);
            m_lstSelect.Add(HolidayVM.Prop.Descriptions.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(HolidayID);
            m_objFilter.Add(HolidayVM.Prop.HolidayID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(Descriptions);
            m_objFilter.Add(HolidayVM.Prop.Descriptions.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMHolidaysDA = m_objMHolidaysDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMHolidaysDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpHolidayBL in m_dicMHolidaysDA)
                    {
                        m_intCount = m_kvpHolidayBL.Key;
                        break;
                    }
                else
                {
                    m_lstHolidayVM = (
                        from DataRow m_drMHolidaysDA in m_dicMHolidaysDA[0].Tables[0].Rows
                        select new HolidayVM()
                        {
                            HolidayID = m_drMHolidaysDA[HolidayVM.Prop.HolidayID.Name].ToString(),
                            Descriptions = m_drMHolidaysDA[HolidayVM.Prop.Descriptions.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstHolidayVM);
            return m_dicReturn;
        }

        #endregion
    }
}