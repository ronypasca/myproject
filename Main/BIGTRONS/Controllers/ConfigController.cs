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
    public class ConfigController : BaseController
    {
        private readonly string title = "Cluster";
        private readonly string _strBoolConfig = "Bool";


        #region Public Action
        public ActionResult Index()
        {
            base.Initialize();
            return View();
        }

        public ActionResult List()
        {
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
            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcUConfig = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcUConfig.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = ConfigVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicUConfigDA = m_objUConfigDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpConfigBL in m_dicUConfigDA)
            {
                m_intCount = m_kvpConfigBL.Key;
                break;
            }

            List<ConfigVM> m_lstConfigVM = new List<ConfigVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(ConfigVM.Prop.Key1.MapAlias);
                m_lstSelect.Add(ConfigVM.Prop.Key2.MapAlias);
                m_lstSelect.Add(ConfigVM.Prop.Key3.MapAlias);
                m_lstSelect.Add(ConfigVM.Prop.Key4.MapAlias);
                m_lstSelect.Add(ConfigVM.Prop.Desc1.MapAlias);
                m_lstSelect.Add(ConfigVM.Prop.Desc2.MapAlias);
                m_lstSelect.Add(ConfigVM.Prop.Desc3.MapAlias);
                m_lstSelect.Add(ConfigVM.Prop.Desc4.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(ConfigVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicUConfigDA = m_objUConfigDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objUConfigDA.Message == string.Empty)
                {
                    m_lstConfigVM = (
                        from DataRow m_drUConfigDA in m_dicUConfigDA[0].Tables[0].Rows
                        select new ConfigVM()
                        {
                            Key1 = m_drUConfigDA[ConfigVM.Prop.Key1.Name].ToString(),
                            Key2 = m_drUConfigDA[ConfigVM.Prop.Key2.Name].ToString(),
                            Key3 = m_drUConfigDA[ConfigVM.Prop.Key3.Name].ToString(),
                            Key4 = m_drUConfigDA[ConfigVM.Prop.Key4.Name].ToString(),
                            Desc1 = m_drUConfigDA[ConfigVM.Prop.Desc1.Name].ToString(),
                            Desc2 = m_drUConfigDA[ConfigVM.Prop.Desc2.Name].ToString(),
                            Desc3 = m_drUConfigDA[ConfigVM.Prop.Desc3.Name].ToString(),
                            Desc4 = m_drUConfigDA[ConfigVM.Prop.Desc4.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstConfigVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            UConfig m_objUConfig = new UConfig();
            m_objUConfigDA.Data = m_objUConfig;
            ConfigVM m_objConfigVM = new ConfigVM();
            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMUoM = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMUoM.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = UoMVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicUConfigDA = m_objUConfigDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpConfigBL in m_dicUConfigDA)
            {
                m_intCount = m_kvpConfigBL.Key;
                break;
            }

            List<ConfigVM> m_lstConfigVM = new List<ConfigVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(ConfigVM.Prop.Key1.MapAlias);
                m_lstSelect.Add(ConfigVM.Prop.Key2.MapAlias);
                m_lstSelect.Add(ConfigVM.Prop.Key3.MapAlias);
                m_lstSelect.Add(ConfigVM.Prop.Key4.MapAlias);
                m_lstSelect.Add(ConfigVM.Prop.Desc1.MapAlias);
                m_lstSelect.Add(ConfigVM.Prop.Desc2.MapAlias);
                m_lstSelect.Add(ConfigVM.Prop.Desc3.MapAlias);
                m_lstSelect.Add(ConfigVM.Prop.Desc4.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicUConfigDA = m_objUConfigDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objUConfigDA.Message == string.Empty)
                {
                    m_lstConfigVM = (
                        from DataRow m_drUConfigDA in m_dicUConfigDA[0].Tables[0].Rows
                        select new ConfigVM()
                        {
                            Key1 = m_drUConfigDA[ConfigVM.Prop.Key1.Name].ToString(),
                            Key2 = m_drUConfigDA[ConfigVM.Prop.Key2.Name].ToString(),
                            Key3 = m_drUConfigDA[ConfigVM.Prop.Key3.Name].ToString(),
                            Key4 = m_drUConfigDA[ConfigVM.Prop.Key4.Name].ToString(),
                            Desc1 = m_drUConfigDA[ConfigVM.Prop.Desc1.Name].ToString(),
                            Desc2 = m_drUConfigDA[ConfigVM.Prop.Desc2.Name].ToString(),
                            Desc3 = m_drUConfigDA[ConfigVM.Prop.Desc3.Name].ToString(),
                            Desc4 = m_drUConfigDA[ConfigVM.Prop.Desc4.Name].ToString()
                        }).ToList();
                }
            }
            return this.Store(m_lstConfigVM, m_intCount);
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

            ConfigVM m_objConfigVM = new ConfigVM();
            ViewDataDictionary m_vddConfig = new ViewDataDictionary();
            m_vddConfig.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddConfig.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objConfigVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddConfig,
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
            if (Caller == General.EnumDesc(Buttons.ButtonList))
            {
                m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
            }
            else if (Caller == General.EnumDesc(Buttons.ButtonAdd) || Caller == General.EnumDesc(Buttons.ButtonUpdate))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                m_dicSelectedRow = GetFormData(m_nvcParams);
            }

            string m_strMessage = string.Empty;
            ConfigVM m_objConfigVM = new ConfigVM();
            if (m_dicSelectedRow.Count > 0)
                m_objConfigVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
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
                Model = m_objConfigVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddWorkCenter,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Update(string Caller, string Selected)
        {
            //Global.HasAccess = GetHasAccess();
            //if (!Global.HasAccess.Update)
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();
            if (Caller == General.EnumDesc(Buttons.ButtonList))
            {
                m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
            }
            else if (Caller == General.EnumDesc(Buttons.ButtonDetail))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                m_dicSelectedRow = GetFormData(m_nvcParams);
            }

            string m_strMessage = string.Empty;
            ConfigVM m_objConfigVM = new ConfigVM();
            if (m_dicSelectedRow.Count > 0)
                m_objConfigVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddConfig = new ViewDataDictionary();
            m_vddConfig.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddConfig.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objConfigVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddConfig,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<ConfigVM> m_lstSelectedRow = new List<ConfigVM>();
            m_lstSelectedRow = JSON.Deserialize<List<ConfigVM>>(Selected);

            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (ConfigVM m_objConfigVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifConfigVM = m_objConfigVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifConfigVM in m_arrPifConfigVM)
                    {
                        string m_strFieldName = m_pifConfigVM.Name;
                        object m_objFieldValue = m_pifConfigVM.GetValue(m_objConfigVM);
                        if (m_objConfigVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(ConfigVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objUConfigDA.DeleteBC(m_objFilter, false);
                    if (m_objUConfigDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objUConfigDA.Message);
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

        public ActionResult Save(string Action)
        {
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            try
            {
                string m_strKey1 = this.Request.Params[ConfigVM.Prop.Key1.Name];
                string m_strKey2 = this.Request.Params[ConfigVM.Prop.Key2.Name];
                string m_strKey3 = this.Request.Params[ConfigVM.Prop.Key3.Name];
                string m_strKey4 = this.Request.Params[ConfigVM.Prop.Key4.Name];
                string m_strDesc1 = this.Request.Params[ConfigVM.Prop.Desc1.Name];
                string m_strDesc2 = this.Request.Params[ConfigVM.Prop.Desc2.Name];
                string m_strDesc3 = this.Request.Params[ConfigVM.Prop.Desc3.Name];
                string m_strDesc4 = this.Request.Params[ConfigVM.Prop.Desc4.Name];

                m_lstMessage = IsSaveValid(Action, m_strKey1, m_strDesc1);
                if (m_lstMessage.Count <= 0)
                {
                    UConfig m_objUConfig = new UConfig();
                    m_objUConfig.Key1 = m_strKey1;
                    m_objUConfig.Key2 = m_strKey2;
                    m_objUConfig.Key3 = m_strKey3;
                    m_objUConfig.Key4 = m_strKey4;
                    m_objUConfigDA.Data = m_objUConfig;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objUConfigDA.Select();

                    m_objUConfig.Desc1 = m_strDesc1;
                    m_objUConfig.Desc2 = m_strDesc2;
                    m_objUConfig.Desc3 = m_strDesc3;
                    m_objUConfig.Desc4 = m_strDesc4;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objUConfigDA.Insert(false);
                    else
                        m_objUConfigDA.Update(false);

                    if (!m_objUConfigDA.Success || m_objUConfigDA.Message != string.Empty)
                        m_lstMessage.Add(m_objUConfigDA.Message);
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

        #region Private Method
        private List<string> IsSaveValid(string Action, string Key1, string Desc1)
        {
            List<string> m_lstReturn = new List<string>();

            if (Key1 == string.Empty)
                m_lstReturn.Add(ConfigVM.Prop.Key1.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (Desc1 == string.Empty)
                m_lstReturn.Add(ConfigVM.Prop.Desc1.Desc + " " + General.EnumDesc(MessageLib.mustFill));
           
            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(ConfigVM.Prop.Key1.Name, parameters[ConfigVM.Prop.Key1.Name]);
            m_dicReturn.Add(ConfigVM.Prop.Key2.Name, parameters[ConfigVM.Prop.Key2.Name]);
            m_dicReturn.Add(ConfigVM.Prop.Key3.Name, parameters[ConfigVM.Prop.Key3.Name]);
            m_dicReturn.Add(ConfigVM.Prop.Key4.Name, parameters[ConfigVM.Prop.Key4.Name]);

            m_dicReturn.Add(ConfigVM.Prop.Desc1.Name, parameters[ConfigVM.Prop.Desc1.Name]);
            m_dicReturn.Add(ConfigVM.Prop.Desc2.Name, parameters[ConfigVM.Prop.Desc2.Name]);
            m_dicReturn.Add(ConfigVM.Prop.Desc3.Name, parameters[ConfigVM.Prop.Desc3.Name]);
            m_dicReturn.Add(ConfigVM.Prop.Desc4.Name, parameters[ConfigVM.Prop.Desc4.Name]);

            return m_dicReturn;
        }

        private ConfigVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            ConfigVM m_objConfigVM = new ConfigVM();
            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ConfigVM.Prop.Key1.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Key2.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Key3.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Key4.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc1.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc2.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc3.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc4.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objConfigVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(ConfigVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicUConfigDA = m_objUConfigDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objUConfigDA.Message == string.Empty)
            {
                DataRow m_drUConfigDA = m_dicUConfigDA[0].Tables[0].Rows[0];
                m_objConfigVM.Key1 = m_drUConfigDA[ConfigVM.Prop.Key1.Name].ToString();
                m_objConfigVM.Key2 = m_drUConfigDA[ConfigVM.Prop.Key2.Name].ToString();
                m_objConfigVM.Key3 = m_drUConfigDA[ConfigVM.Prop.Key3.Name].ToString();
                m_objConfigVM.Key4 = m_drUConfigDA[ConfigVM.Prop.Key4.Name].ToString();
                m_objConfigVM.Desc1 = m_drUConfigDA[ConfigVM.Prop.Desc1.Name].ToString();
                m_objConfigVM.Desc2 = m_drUConfigDA[ConfigVM.Prop.Desc2.Name].ToString();
                m_objConfigVM.Desc3 = m_drUConfigDA[ConfigVM.Prop.Desc3.Name].ToString();
                m_objConfigVM.Desc4 = m_drUConfigDA[ConfigVM.Prop.Desc4.Name].ToString();
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objUConfigDA.Message;

            return m_objConfigVM;
        }
        #endregion

        #region Public Method
        public string GetLDAPPath()
        {
            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            UConfig m_objUConfig = new UConfig();
            m_objUConfigDA.Data = m_objUConfig;
            ConfigVM m_objConfigVM = new ConfigVM();

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ConfigVM.Prop.Desc1.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("LDAPPath");
            m_objFilter.Add(ConfigVM.Prop.Key1.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicUConfig = m_objUConfigDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null,null);
            string m_strLDAPPath = (
                from DataRow m_drUConfig in m_dicUConfig[0].Tables[0].Rows
                select m_drUConfig[General.GetVariableName(() => m_objUConfig.Desc1)].ToString()).ToList()[0];
            return m_strLDAPPath;
        }
        #endregion
        public ActionResult GetBoolList(StoreRequestParameters parameters)
        {
            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            //UConfig m_objUConfig = new UConfig();
            //m_objUConfigDA.UConfigData = m_objUConfig;
            ConfigBoolVM m_objConfigBoolVM = new ConfigBoolVM();

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ConfigBoolVM.Prop.ID.MapAlias);
            m_lstSelect.Add(ConfigBoolVM.Prop.Description.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(_strBoolConfig);
            m_objFilter.Add(ConfigBoolVM.Prop.FilterKey.Map, m_lstFilter);

            try
            {
                Dictionary<int, DataSet> m_dicUConfigDA = m_objUConfigDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null,null);
                List<ConfigBoolVM> m_lstBool = (
                    from DataRow m_drUConfig in m_dicUConfigDA[0].Tables[0].Rows
                    select new ConfigBoolVM()
                    {
                        ID = m_drUConfig[ConfigBoolVM.Prop.ID.Name].ToString(),
                        Description = m_drUConfig[ConfigBoolVM.Prop.Description.Name].ToString()
                    }).ToList();
                return this.Store(m_lstBool);
            }
            catch (Exception ex)
            {
                Global.ShowErrorAlert(title, ex.Message);
                return this.Direct();
            }
        }

        /*
        public ActionResult GetProductForReceiptReport(StoreRequestParameters parameters, string Key1, string Key2)
        {
            ConfigVM m_objConfigVM = new ConfigVM();
            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            UConfig m_objUConfig = new UConfig();

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(m_objConfigVM.MapField(() => m_objConfigVM.Key1, true));
            m_lstSelect.Add(m_objConfigVM.MapField(() => m_objConfigVM.Key2, true));
            m_lstSelect.Add(m_objConfigVM.MapField(() => m_objConfigVM.Key3, true));
            m_lstSelect.Add(m_objConfigVM.MapField(() => m_objConfigVM.Key4, true));
            m_lstSelect.Add(m_objConfigVM.MapField(() => m_objConfigVM.Desc1, true));
            m_lstSelect.Add(m_objConfigVM.MapField(() => m_objConfigVM.Desc2, true));
            m_lstSelect.Add(m_objConfigVM.MapField(() => m_objConfigVM.Desc3, true));
            m_lstSelect.Add(m_objConfigVM.MapField(() => m_objConfigVM.Desc4, true));

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(Key1);
            m_objFilter.Add(m_objConfigVM.MapField(() => m_objConfigVM.Key1, false), m_lstFilter);
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(Key2);
            m_objFilter.Add(m_objConfigVM.MapField(() => m_objConfigVM.Key2, false), m_lstFilter);

            try
            {
                List<ConfigVM> m_lstConfigVM = new List<ConfigVM>();
                Dictionary<int, DataSet> m_dicUConfigDA = m_objUConfigDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null);
                if (m_objUConfigDA.Message == string.Empty)
                {
                    m_lstConfigVM = (
                        from DataRow m_drUConfig in m_dicUConfigDA[0].Tables[0].Rows
                        select new ConfigVM()
                        {
                            Key1 = m_drUConfig[General.GetVariableName(() => m_objConfigVM.Key1)].ToString(),
                            Key2 = m_drUConfig[General.GetVariableName(() => m_objConfigVM.Key2)].ToString(),
                            Key3 = m_drUConfig[General.GetVariableName(() => m_objConfigVM.Key3)].ToString(),
                            Key4 = m_drUConfig[General.GetVariableName(() => m_objConfigVM.Key4)].ToString(),
                            Desc1 = m_drUConfig[General.GetVariableName(() => m_objConfigVM.Desc1)].ToString(),
                            Desc2 = m_drUConfig[General.GetVariableName(() => m_objConfigVM.Desc2)].ToString(),
                            Desc3 = m_drUConfig[General.GetVariableName(() => m_objConfigVM.Desc3)].ToString(),
                            Desc4 = m_drUConfig[General.GetVariableName(() => m_objConfigVM.Desc4)].ToString()
                        }).ToList();
                }
                else
                    Global.ShowErrorAlert(title, m_objUConfigDA.Message);
                return this.Store(m_lstConfigVM);
            }
            catch (Exception ex)
            {
                Global.ShowErrorAlert(title, ex.Message);
                return this.Direct();
            }
        }

        public List<ConfigVM> GetListProductForReport(ref string m_strMessage)
        {
            List<ConfigVM> m_lstConfigVM = new List<ConfigVM>();
            ConfigVM m_objConfigVM = new ConfigVM();
            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            UConfig m_objUConfig = new UConfig();

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(m_objConfigVM.MapField(() => m_objConfigVM.Key1, true));
            m_lstSelect.Add(m_objConfigVM.MapField(() => m_objConfigVM.Key2, true));
            m_lstSelect.Add(m_objConfigVM.MapField(() => m_objConfigVM.Key3, true));
            m_lstSelect.Add(m_objConfigVM.MapField(() => m_objConfigVM.Key4, true));
            m_lstSelect.Add(m_objConfigVM.MapField(() => m_objConfigVM.Desc1, true));
            m_lstSelect.Add(m_objConfigVM.MapField(() => m_objConfigVM.Desc2, true));
            m_lstSelect.Add(m_objConfigVM.MapField(() => m_objConfigVM.Desc3, true));
            m_lstSelect.Add(m_objConfigVM.MapField(() => m_objConfigVM.Desc4, true));

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("ReportFilter");
            m_objFilter.Add(m_objConfigVM.MapField(() => m_objConfigVM.Key1, false), m_lstFilter);
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("ProductID");
            m_objFilter.Add(m_objConfigVM.MapField(() => m_objConfigVM.Key2, false), m_lstFilter);

            try
            {
                Dictionary<int, DataSet> m_dicUConfigDA = m_objUConfigDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null);
                if (m_objUConfigDA.Message == string.Empty)
                {
                    m_lstConfigVM = (
                        from DataRow m_drUConfig in m_dicUConfigDA[0].Tables[0].Rows
                        select new ConfigVM()
                        {
                            Key1 = m_drUConfig[General.GetVariableName(() => m_objConfigVM.Key1)].ToString(),
                            Key2 = m_drUConfig[General.GetVariableName(() => m_objConfigVM.Key2)].ToString(),
                            Key3 = m_drUConfig[General.GetVariableName(() => m_objConfigVM.Key3)].ToString(),
                            Key4 = m_drUConfig[General.GetVariableName(() => m_objConfigVM.Key4)].ToString(),
                            Desc1 = m_drUConfig[General.GetVariableName(() => m_objConfigVM.Desc1)].ToString(),
                            Desc2 = m_drUConfig[General.GetVariableName(() => m_objConfigVM.Desc2)].ToString(),
                            Desc3 = m_drUConfig[General.GetVariableName(() => m_objConfigVM.Desc3)].ToString(),
                            Desc4 = m_drUConfig[General.GetVariableName(() => m_objConfigVM.Desc4)].ToString()
                        }).ToList();
                }
                else
                    m_strMessage = m_objUConfigDA.Message;
            }
            catch (Exception ex)
            {
                m_strMessage = ex.Message;
                Global.ShowErrorAlert(title, ex.Message);
            }
            return m_lstConfigVM;
        }

        public ActionResult GetProductForReport(StoreRequestParameters parameters)
        {
            string m_strMessage = string.Empty;
            List<ConfigVM> m_lstConfigVM = GetListProductForReport(ref m_strMessage);
            if (m_strMessage == string.Empty)
            {
                return this.Store(m_lstConfigVM);
            }
            else
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }
        }

        public ActionResult GetDefaultValueList(StoreRequestParameters parameters, string key = "")
        {
            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            UConfig m_objUConfig = new UConfig();
            m_objUConfigDA.Data = m_objUConfig;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(General.GetVariableName(() => m_objUConfig.Key2));
            m_lstSelect.Add(General.GetVariableName(() => m_objUConfig.Desc1));

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(_strDefaultValueConfig);
            m_objFilter.Add(General.GetVariableName(() => m_objUConfig.Key1), m_lstFilter);

            if (key != string.Empty)
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(key);
                m_objFilter.Add(General.GetVariableName(() => m_objUConfig.Key2), m_lstFilter);
            }

            Dictionary<int, DataSet> m_dicUConfig = m_objUConfigDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null);
            List<ConfigDefaultValueVM> m_lstDefaultValue = (
                from DataRow m_drUConfig in m_dicUConfig[0].Tables[0].Rows
                select new ConfigDefaultValueVM()
                {
                    ID = m_drUConfig[General.GetVariableName(() => m_objUConfig.Key2)].ToString(),
                    Desc = m_drUConfig[General.GetVariableName(() => m_objUConfig.Desc1)].ToString()
                }).ToList();
            return this.Store(m_lstDefaultValue);
        }

        public ConfigVM GetWebServiceUserPassword(string key)
        {
            ConfigVM m_objConfigVM = new ConfigVM();

            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            UConfig m_objUConfig = new UConfig();
            m_objUConfigDA.Data = m_objUConfig;
            m_objUConfig.Key1 = _strWebServiceConfig;
            m_objUConfig.Key2 = key;
            m_objUConfig.Key3 = string.Empty;
            m_objUConfig.Key4 = string.Empty;
            m_objUConfigDA.Select();
            if (m_objUConfigDA.Message == "")
            {
                string m_strUserName = string.Empty;
                string m_strPassword = string.Empty;

                string m_strEncrypted = m_objUConfigDA.Data.Desc1; // UserName
                m_strUserName = Encryption.DefaultDecrypt(m_strEncrypted);
                m_strEncrypted = m_objUConfigDA.Data.Desc2; // Password
                m_strPassword = Encryption.DefaultDecrypt(m_strEncrypted);

                m_objConfigVM.Key1 = m_objUConfigDA.Data.Key1;
                m_objConfigVM.Key2 = m_objUConfigDA.Data.Key2;
                m_objConfigVM.Key3 = m_objUConfigDA.Data.Key3;
                m_objConfigVM.Key4 = m_objUConfigDA.Data.Key4;
                m_objConfigVM.Desc1 = m_strUserName;
                m_objConfigVM.Desc2 = m_strPassword;
                m_objConfigVM.Desc3 = m_objUConfigDA.Data.Desc3;
                m_objConfigVM.Desc4 = m_objUConfigDA.Data.Desc4;

                // Re-encrypt
                m_objUConfig.Desc1 = Encryption.DefaultEncrypt(m_strUserName);
                m_objUConfig.Desc2 = Encryption.DefaultEncrypt(m_strPassword);
                m_objUConfigDA.Update(false);
            }

            return m_objConfigVM;
        }

        public List<ConfigVM> GetAdminFeeList()
        {
            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            UConfig m_objUConfig = new UConfig();
            m_objUConfigDA.Data = m_objUConfig;
            ConfigVM m_objConfigVM = new ConfigVM();
            MProduct m_objMProduct = new MProduct();

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(m_objConfigVM.MapField(() => m_objUConfig.Key2, false));
            m_lstSelect.Add(m_objConfigVM.MapField(() => m_objUConfig.Desc1, false));
            m_lstSelect.Add(m_objConfigVM.MapField(() => m_objUConfig.Desc2, false));
            m_lstSelect.Add(m_objConfigVM.MapField(() => m_objUConfig.Desc3, false));
            m_lstSelect.Add(General.GetVariableName(() => m_objMProduct.ProductID));
            m_lstSelect.Add(General.GetVariableName(() => m_objMProduct.ProductDesc));

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(_strAdminFeeConfig);
            m_objFilter.Add(m_objConfigVM.MapField(() => m_objUConfig.Key1, false), m_lstFilter);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(m_objConfigVM.MapField(() => m_objUConfig.Key2, false), OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicUConfig = m_objUConfigDA.SelectAdminStampDuty(0, null, false, m_lstSelect, m_objFilter, m_dicOrder, null);
            List<ConfigVM> m_lstAdminFee = (
                from DataRow m_drUConfig in m_dicUConfig[0].Tables[0].Rows
                select new ConfigVM()
                {
                    Key1 = string.Empty,
                    Key2 = m_drUConfig[General.GetVariableName(() => m_objUConfig.Key2)].ToString(),
                    Key3 = m_drUConfig[General.GetVariableName(() => m_objMProduct.ProductID)].ToString(),
                    Key4 = m_drUConfig[General.GetVariableName(() => m_objMProduct.ProductDesc)].ToString(),
                    Desc1 = m_drUConfig[General.GetVariableName(() => m_objUConfig.Desc1)].ToString(),
                    Desc2 = m_drUConfig[General.GetVariableName(() => m_objUConfig.Desc2)].ToString(),
                    Desc3 = m_drUConfig[General.GetVariableName(() => m_objUConfig.Desc3)].ToString(),
                    Desc4 = string.Empty
                }).ToList();
            return m_lstAdminFee;
        }

        public List<ConfigVM> GetStampDutyList()
        {
            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            UConfig m_objUConfig = new UConfig();
            m_objUConfigDA.Data = m_objUConfig;
            ConfigVM m_objConfigVM = new ConfigVM();
            MProduct m_objMProduct = new MProduct();

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(m_objConfigVM.MapField(() => m_objUConfig.Key2, false));
            m_lstSelect.Add(m_objConfigVM.MapField(() => m_objUConfig.Desc1, false));
            m_lstSelect.Add(m_objConfigVM.MapField(() => m_objUConfig.Desc2, false));
            m_lstSelect.Add(m_objConfigVM.MapField(() => m_objUConfig.Desc3, false));
            m_lstSelect.Add(General.GetVariableName(() => m_objMProduct.ProductID));
            m_lstSelect.Add(General.GetVariableName(() => m_objMProduct.ProductDesc));

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(_strStampDutyConfig);
            m_objFilter.Add(m_objConfigVM.MapField(() => m_objUConfig.Key1, false), m_lstFilter);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add("CONVERT(DECIMAL, " + m_objConfigVM.MapField(() => m_objUConfig.Key2, false) + ")", OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicUConfig = m_objUConfigDA.SelectAdminStampDuty(0, null, false, m_lstSelect, m_objFilter, m_dicOrder, null);
            List<ConfigVM> m_lstStampDuty = (
                from DataRow m_drUConfig in m_dicUConfig[0].Tables[0].Rows
                select new ConfigVM()
                {
                    Key1 = string.Empty,
                    Key2 = m_drUConfig[General.GetVariableName(() => m_objUConfig.Key2)].ToString(),
                    Key3 = m_drUConfig[General.GetVariableName(() => m_objMProduct.ProductID)].ToString(),
                    Key4 = m_drUConfig[General.GetVariableName(() => m_objMProduct.ProductDesc)].ToString(),
                    Desc1 = m_drUConfig[General.GetVariableName(() => m_objUConfig.Desc1)].ToString(),
                    Desc2 = m_drUConfig[General.GetVariableName(() => m_objUConfig.Desc2)].ToString(),
                    Desc3 = m_drUConfig[General.GetVariableName(() => m_objUConfig.Desc3)].ToString(),
                    Desc4 = string.Empty
                }).ToList();
            return m_lstStampDuty;
        }

        public List<ConfigVM> GetMappingProductList(string productDesc = "")
        {
            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            UConfig m_objUConfig = new UConfig();
            m_objUConfigDA.Data = m_objUConfig;
            ConfigVM m_objConfigVM = new ConfigVM();
            MProduct m_objMProduct = new MProduct();

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(General.GetVariableName(() => m_objUConfig.Key3));
            m_lstSelect.Add(General.GetVariableName(() => m_objUConfig.Desc1));
            m_lstSelect.Add(General.GetVariableName(() => m_objMProduct.ProductDesc));

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(_strMappingConfig);
            m_objFilter.Add(General.GetVariableName(() => m_objUConfig.Key1), m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("ProductID");
            m_objFilter.Add(General.GetVariableName(() => m_objUConfig.Key2), m_lstFilter);

            if (productDesc != string.Empty)
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(productDesc);
                m_objFilter.Add(General.GetVariableName(() => m_objUConfig.Key3), m_lstFilter);
            }

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(General.GetVariableName(() => m_objConfigVM.Key3), OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicUConfig = m_objUConfigDA.SelectMappingProduct(0, null, false, m_lstSelect, m_objFilter, m_dicOrder, null);
            List<ConfigVM> m_lstStampDuty = (
                from DataRow m_drUConfig in m_dicUConfig[0].Tables[0].Rows
                select new ConfigVM()
                {
                    Key1 = string.Empty,
                    Key2 = string.Empty,
                    Key3 = m_drUConfig[General.GetVariableName(() => m_objUConfig.Key3)].ToString(),
                    Key4 = string.Empty,
                    Desc1 = m_drUConfig[General.GetVariableName(() => m_objUConfig.Desc1)].ToString(),
                    Desc2 = m_drUConfig[General.GetVariableName(() => m_objMProduct.ProductDesc)].ToString(),
                    Desc3 = string.Empty,
                    Desc4 = string.Empty
                }).ToList();
            return m_lstStampDuty;
        }

        public List<ConfigVM> GetMappingParameterList(string parameterDesc = "")
        {
            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            UConfig m_objUConfig = new UConfig();
            m_objUConfigDA.Data = m_objUConfig;
            ConfigVM m_objConfigVM = new ConfigVM();

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(General.GetVariableName(() => m_objUConfig.Key3));
            m_lstSelect.Add(General.GetVariableName(() => m_objUConfig.Desc1));

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(_strMappingConfig);
            m_objFilter.Add(General.GetVariableName(() => m_objUConfig.Key1), m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("ParameterID");
            m_objFilter.Add(General.GetVariableName(() => m_objUConfig.Key2), m_lstFilter);

            if (parameterDesc != string.Empty)
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(parameterDesc);
                m_objFilter.Add(General.GetVariableName(() => m_objUConfig.Key3), m_lstFilter);
            }

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(General.GetVariableName(() => m_objConfigVM.Key3), OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicUConfig = m_objUConfigDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, m_dicOrder, null);
            List<ConfigVM> m_lstStampDuty = (
                from DataRow m_drUConfig in m_dicUConfig[0].Tables[0].Rows
                select new ConfigVM()
                {
                    Key1 = string.Empty,
                    Key2 = string.Empty,
                    Key3 = m_drUConfig[General.GetVariableName(() => m_objUConfig.Key3)].ToString(),
                    Key4 = string.Empty,
                    Desc1 = m_drUConfig[General.GetVariableName(() => m_objUConfig.Desc1)].ToString(),
                    Desc2 = string.Empty,
                    Desc3 = string.Empty,
                    Desc4 = string.Empty
                }).ToList();
            return m_lstStampDuty;
        }

        public List<ConfigVM> GetKeyAccountReceiptOnBillList()
        {
            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            UConfig m_objUConfig = new UConfig();
            m_objUConfigDA.Data = m_objUConfig;
            ConfigVM m_objConfigVM = new ConfigVM();

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(General.GetVariableName(() => m_objUConfig.Key3));

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("GenerateReceipt");
            m_objFilter.Add(General.GetVariableName(() => m_objUConfig.Key1), m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("Billing");
            m_objFilter.Add(General.GetVariableName(() => m_objUConfig.Key2), m_lstFilter);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(General.GetVariableName(() => m_objConfigVM.Key3), OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicUConfig = m_objUConfigDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, m_dicOrder, null);
            List<ConfigVM> m_lstStampDuty = (
                from DataRow m_drUConfig in m_dicUConfig[0].Tables[0].Rows
                select new ConfigVM()
                {
                    Key1 = string.Empty,
                    Key2 = string.Empty,
                    Key3 = m_drUConfig[General.GetVariableName(() => m_objUConfig.Key3)].ToString(),
                    Key4 = string.Empty,
                    Desc1 = string.Empty,
                    Desc2 = string.Empty,
                    Desc3 = string.Empty,
                    Desc4 = string.Empty
                }).ToList();
            return m_lstStampDuty;
        }

        public string GetLDAPPath()
        {
            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            UConfig m_objUConfig = new UConfig();
            m_objUConfigDA.Data = m_objUConfig;
            ConfigVM m_objConfigVM = new ConfigVM();

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(General.GetVariableName(() => m_objUConfig.Desc1));

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("LDAPPath");
            m_objFilter.Add(General.GetVariableName(() => m_objUConfig.Key1), m_lstFilter);

            Dictionary<int, DataSet> m_dicUConfig = m_objUConfigDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null);
            string m_strLDAPPath = (
                from DataRow m_drUConfig in m_dicUConfig[0].Tables[0].Rows
                select m_drUConfig[General.GetVariableName(() => m_objUConfig.Desc1)].ToString()).ToList()[0];
            return m_strLDAPPath;
        }

        public ConfigVM GetMeterReadingDeviation()
        {
            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            UConfig m_objUConfig = new UConfig();
            m_objUConfigDA.Data = m_objUConfig;
            ConfigVM m_objConfigVM = new ConfigVM();

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(General.GetVariableName(() => m_objUConfig.Desc1));
            m_lstSelect.Add(General.GetVariableName(() => m_objUConfig.Desc2));
            m_lstSelect.Add(General.GetVariableName(() => m_objUConfig.Desc3));

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("Default");
            m_objFilter.Add(General.GetVariableName(() => m_objUConfig.Key1), m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("MeterReadingDeviation");
            m_objFilter.Add(General.GetVariableName(() => m_objUConfig.Key2), m_lstFilter);

            Dictionary<int, DataSet> m_dicUConfig = m_objUConfigDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null);
            ConfigVM m_objMeterReadingDeviation = (
                from DataRow m_drUConfig in m_dicUConfig[0].Tables[0].Rows
                select new ConfigVM()
                {
                    Key1 = string.Empty,
                    Key2 = string.Empty,
                    Key3 = string.Empty,
                    Key4 = string.Empty,
                    Desc1 = m_drUConfig[General.GetVariableName(() => m_objUConfig.Desc1)].ToString(),
                    Desc2 = m_drUConfig[General.GetVariableName(() => m_objUConfig.Desc2)].ToString(),
                    Desc3 = m_drUConfig[General.GetVariableName(() => m_objUConfig.Desc3)].ToString(),
                    Desc4 = string.Empty
                }).ToList()[0];
            return m_objMeterReadingDeviation;
        }

        public bool IsPaymentDateLocked()
        {
            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            UConfig m_objUConfig = new UConfig();
            m_objUConfigDA.Data = m_objUConfig;
            m_objUConfig.Key1 = "Default";
            m_objUConfig.Key2 = "PaymentDateLocked";
            m_objUConfig.Key3 = "";
            m_objUConfig.Key4 = "";
            m_objUConfigDA.Select();
            if (m_objUConfigDA.Success)
                return bool.Parse(m_objUConfigDA.Data.Desc1);
            return false;
        }

        public bool IsAdjustmentDateLocked()
        {
            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            UConfig m_objUConfig = new UConfig();
            m_objUConfigDA.Data = m_objUConfig;
            m_objUConfig.Key1 = "Default";
            m_objUConfig.Key2 = "AdjustmentLocked";
            m_objUConfig.Key3 = "";
            m_objUConfig.Key4 = "";
            m_objUConfigDA.Select();
            if (m_objUConfigDA.Success)
                return bool.Parse(m_objUConfigDA.Data.Desc1);
            return false;
        }

        public bool IsPostingBankDateLocked()
        {
            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            UConfig m_objUConfig = new UConfig();
            m_objUConfigDA.Data = m_objUConfig;
            m_objUConfig.Key1 = "Default";
            m_objUConfig.Key2 = "PostingBankDateLocked";
            m_objUConfig.Key3 = "";
            m_objUConfig.Key4 = "";
            m_objUConfigDA.Select();
            if (m_objUConfigDA.Success)
                return bool.Parse(m_objUConfigDA.Data.Desc1);
            return false;
        }


        public List<ConfigVM> GetListProductGroupForReport(ref string m_strMessage)
        {
            List<ConfigVM> m_lstConfigVM = new List<ConfigVM>();
            ConfigVM m_objConfigVM = new ConfigVM();
            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            UConfig m_objUConfig = new UConfig();

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(m_objConfigVM.MapField(() => m_objConfigVM.Key1, true));
            m_lstSelect.Add(m_objConfigVM.MapField(() => m_objConfigVM.Key2, true));
            m_lstSelect.Add(m_objConfigVM.MapField(() => m_objConfigVM.Key3, true));
            m_lstSelect.Add(m_objConfigVM.MapField(() => m_objConfigVM.Key4, true));
            m_lstSelect.Add(m_objConfigVM.MapField(() => m_objConfigVM.Desc1, true));
            m_lstSelect.Add(m_objConfigVM.MapField(() => m_objConfigVM.Desc2, true));
            m_lstSelect.Add(m_objConfigVM.MapField(() => m_objConfigVM.Desc3, true));
            m_lstSelect.Add(m_objConfigVM.MapField(() => m_objConfigVM.Desc4, true));

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("ReportFilter");
            m_objFilter.Add(m_objConfigVM.MapField(() => m_objConfigVM.Key1, false), m_lstFilter);
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("ProductGroupID");
            m_objFilter.Add(m_objConfigVM.MapField(() => m_objConfigVM.Key2, false), m_lstFilter);

            try
            {
                Dictionary<int, DataSet> m_dicUConfigDA = m_objUConfigDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null);
                if (m_objUConfigDA.Message == string.Empty)
                {
                    m_lstConfigVM = (
                        from DataRow m_drUConfig in m_dicUConfigDA[0].Tables[0].Rows
                        select new ConfigVM()
                        {
                            Key1 = m_drUConfig[General.GetVariableName(() => m_objConfigVM.Key1)].ToString(),
                            Key2 = m_drUConfig[General.GetVariableName(() => m_objConfigVM.Key2)].ToString(),
                            Key3 = m_drUConfig[General.GetVariableName(() => m_objConfigVM.Key3)].ToString(),
                            Key4 = m_drUConfig[General.GetVariableName(() => m_objConfigVM.Key4)].ToString(),
                            Desc1 = m_drUConfig[General.GetVariableName(() => m_objConfigVM.Desc1)].ToString(),
                            Desc2 = m_drUConfig[General.GetVariableName(() => m_objConfigVM.Desc2)].ToString(),
                            Desc3 = m_drUConfig[General.GetVariableName(() => m_objConfigVM.Desc3)].ToString(),
                            Desc4 = m_drUConfig[General.GetVariableName(() => m_objConfigVM.Desc4)].ToString()
                        }).ToList();
                }
                else
                    m_strMessage = m_objUConfigDA.Message;
            }
            catch (Exception ex)
            {
                m_strMessage = ex.Message;
                Global.ShowErrorAlert(title, ex.Message);
            }
            return m_lstConfigVM;
        }

        public ActionResult GetProductGroupForReport(StoreRequestParameters parameters)
        {
            string m_strMessage = string.Empty;
            List<ConfigVM> m_lstConfigVM = GetListProductGroupForReport(ref m_strMessage);
            if (m_strMessage == string.Empty)
            {
                return this.Store(m_lstConfigVM);
            }
            else
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }
        }
        */
    }
}