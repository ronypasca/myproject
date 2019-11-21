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
    public class TCMemberController : BaseController
    {
        private readonly string title = "TC Assignment & Delegation";
        private readonly string dataSessionName = "FormData";

        #region Public Action

        public ActionResult Index()
        {
            base.Initialize();
            ViewBag.Title = title;
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
            TTCMembersDA m_objTTCMembersDA = new TTCMembersDA();
            m_objTTCMembersDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcTTCMembers = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcTTCMembers.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = TCMembersVM.Prop.Map(m_strDataIndex, false);
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

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(TCMembersVM.Prop.TCMemberID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.EmployeeID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.EmployeeName.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.SuperiorName.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.PeriodStart.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.PeriodEnd.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.DelegateStartDate.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.DelegateEndDate.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.TCTypeDesc.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.BusinessUnitDesc.MapAlias);

            List<string> m_lstGroup = new List<string>();
            m_lstGroup.Add(TCMembersVM.Prop.TCMemberID.Map);
            m_lstGroup.Add(TCMembersVM.Prop.EmployeeID.Map);
            m_lstGroup.Add(TCMembersVM.Prop.EmployeeName.Map);
            m_lstGroup.Add(TCMembersVM.Prop.SuperiorName.Map);
            m_lstGroup.Add(TCMembersVM.Prop.PeriodStart.Map);
            m_lstGroup.Add(TCMembersVM.Prop.PeriodEnd.Map);
            m_lstGroup.Add(TCMembersVM.Prop.DelegateStartDate.Map);
            m_lstGroup.Add(TCMembersVM.Prop.DelegateEndDate.Map);
            m_lstGroup.Add(TCMembersVM.Prop.TCTypeDesc.Map);
            m_lstGroup.Add(TCMembersVM.Prop.BusinessUnitDesc.Map);
            

            Dictionary<int, DataSet> m_dicTTCMembersDA = m_objTTCMembersDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, m_lstGroup, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpTCMemberBL in m_dicTTCMembersDA)
            {
                m_intCount = m_kvpTCMemberBL.Key;
                break;
            }

            List<TCMembersVM> m_lsTCMembersVM = new List<TCMembersVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;


                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(TCMembersVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicTTCMembersDA = m_objTTCMembersDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, m_lstGroup, m_dicOrder, null);
                if (m_objTTCMembersDA.Message == string.Empty)
                {
                    m_lsTCMembersVM = (
                        from DataRow m_drTTCMembersDA in m_dicTTCMembersDA[0].Tables[0].Rows
                        select new TCMembersVM()
                        {
                            TCMemberID = m_drTTCMembersDA[TCMembersVM.Prop.TCMemberID.Name].ToString(),
                            EmployeeID = m_drTTCMembersDA[TCMembersVM.Prop.EmployeeID.Name].ToString(),
                            EmployeeName = m_drTTCMembersDA[TCMembersVM.Prop.EmployeeName.Name].ToString(),
                            SuperiorName = m_drTTCMembersDA[TCMembersVM.Prop.SuperiorName.Name].ToString(),
                            PeriodStart = DateTime.Parse(m_drTTCMembersDA[TCMembersVM.Prop.PeriodStart.Name].ToString()),
                            PeriodEnd = DateTime.Parse(m_drTTCMembersDA[TCMembersVM.Prop.PeriodEnd.Name].ToString()),
                            DelegateStartDate = !string.IsNullOrEmpty(m_drTTCMembersDA[TCMembersVM.Prop.DelegateStartDate.Name].ToString()) ? DateTime.Parse(m_drTTCMembersDA[TCMembersVM.Prop.DelegateStartDate.Name].ToString()) : (DateTime?)null,
                            DelegateEndDate = !string.IsNullOrEmpty(m_drTTCMembersDA[TCMembersVM.Prop.DelegateEndDate.Name].ToString()) ? DateTime.Parse(m_drTTCMembersDA[TCMembersVM.Prop.DelegateEndDate.Name].ToString()) : (DateTime?)null,
                            TCTypeDesc = m_drTTCMembersDA[TCMembersVM.Prop.TCTypeDesc.Name].ToString(),
                            BusinessUnitDesc = m_drTTCMembersDA[TCMembersVM.Prop.BusinessUnitDesc.Name].ToString(),
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lsTCMembersVM, m_intCount);
        }
        public ActionResult ReadTCAppliedType(StoreRequestParameters parameters, string TCMemberID)
        {
            MTCTypesDA m_objMTCTypesDA = new MTCTypesDA();
            m_objMTCTypesDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcSAction = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcSAction.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = TCAppliedTypesVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMTCTypesDA = m_objMTCTypesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpMenuBL in m_dicMTCTypesDA)
            {
                m_intCount = m_kvpMenuBL.Key;
                break;
            }

            List<TCAppliedTypesVM> m_lstTCAppliedTypesVM = new List<TCAppliedTypesVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(TCTypesVM.Prop.TCTypeDesc.MapAlias);
                m_lstSelect.Add(TCTypesVM.Prop.TCTypeID.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(TCTypesVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMTCTypesDA = m_objMTCTypesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMTCTypesDA.Message == string.Empty)
                {
                    m_lstTCAppliedTypesVM = (
                        from DataRow m_drTTCAppliedTypesDA in m_dicMTCTypesDA[0].Tables[0].Rows
                        select new TCAppliedTypesVM()
                        {
                            TCTypeDesc = m_drTTCAppliedTypesDA[TCTypesVM.Prop.TCTypeDesc.Name].ToString(),
                            TCTypeID = m_drTTCAppliedTypesDA[TCTypesVM.Prop.TCTypeID.Name].ToString(),
                            Checked = IsCheckedType(m_drTTCAppliedTypesDA[TCAppliedTypesVM.Prop.TCTypeID.Name].ToString(), TCMemberID)
                        }
                    ).Distinct().ToList();
                }
            }

            return this.Store(m_lstTCAppliedTypesVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters, string SuperiorID, string SuperiorTCMemberID, string PeriodStart, string PeriodEnd, string Schedule, string BusinessUnitID, string TCTypeID, string ValueTCMemberID)
        {
            TTCMembersDA m_objTTCMembersDA = new TTCMembersDA();
            m_objTTCMembersDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;


            FilterHeaderConditions m_fhcTTCMembers = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcTTCMembers.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = TCMembersVM.Prop.Map(m_strDataIndex, false);

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

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(TCMembersVM.Prop.TCMemberID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.EmployeeID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.EmployeeName.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.TCTypeDesc.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.TCTypeID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.PeriodStart.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.PeriodEnd.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.BusinessUnitDesc.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.Email.MapAlias);
            //m_lstSelect.Add(TCMembersVM.Prop.DelegateStartDate.MapAlias);
            //m_lstSelect.Add(TCMembersVM.Prop.DelegateEndDate.MapAlias);     

            List<string> m_lstGroup = new List<string>();
            m_lstGroup.Add(TCMembersVM.Prop.TCMemberID.Map);
            m_lstGroup.Add(TCMembersVM.Prop.EmployeeID.Map);
            m_lstGroup.Add(TCMembersVM.Prop.FirstName.Map);
            m_lstGroup.Add(TCMembersVM.Prop.MiddleName.Map);
            m_lstGroup.Add(TCMembersVM.Prop.LastName.Map);
            m_lstGroup.Add(TCMembersVM.Prop.PeriodStart.Map);
            m_lstGroup.Add(TCMembersVM.Prop.PeriodEnd.Map);
            m_lstGroup.Add(TCMembersVM.Prop.TCTypeID.Map);
            m_lstGroup.Add(TCMembersVM.Prop.TCTypeDesc.Map);
            m_lstGroup.Add(TCMembersVM.Prop.BusinessUnitDesc.Map);
            m_lstGroup.Add(TCMembersVM.Prop.Email.Map);

            if (!string.IsNullOrEmpty(TCTypeID) && !m_fhcTTCMembers.Json.Contains(TCMembersVM.Prop.TCTypeID.Name))
            {
                //m_lstFilter = new List<object>();
                //m_lstFilter.Add(Operator.Equals);
                //m_lstFilter.Add(TCTypeID);
                //m_objFilter.Add(TCMembersVM.Prop.TCTypeID.Map, m_lstFilter);
            }

            if (!string.IsNullOrEmpty(TCTypeID) && !string.IsNullOrEmpty(ValueTCMemberID))
            {

                DateTime m_dtStart = DateTime.Parse(JSON.Deserialize(PeriodStart).ToString());
                DateTime m_dtEnd = DateTime.Parse(JSON.Deserialize(PeriodEnd).ToString());

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.None);
                m_lstFilter.Add("");
                m_lstFilter.Add("NOTPARENTHESIS");
                m_objFilter.Add($@"{TCMembersVM.Prop.TCTypeID.Map} = '{TCTypeID}' AND NOT ({TCMembersVM.Prop.TCMemberID.Map} = '{ValueTCMemberID}') OR 1=1", m_lstFilter);
            }

            if (SuperiorID != string.Empty)
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(SuperiorID);
                m_objFilter.Add(TCMembersVM.Prop.SuperiorID.Map, m_lstFilter);
            }

            if (!string.IsNullOrEmpty(SuperiorTCMemberID))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(SuperiorTCMemberID);
                m_objFilter.Add(TCMembersVM.Prop.SuperiorTCMemberID.Map, m_lstFilter);
            }
            if (!string.IsNullOrEmpty(BusinessUnitID))
            {
                string[] m_arrListBUnit = JSON.Deserialize<string[]>(BusinessUnitID);
                if (m_arrListBUnit == null)
                    m_arrListBUnit = new List<string>().ToArray();

                if (m_arrListBUnit.Count() > 0)
                {
                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.In);
                    m_lstFilter.Add(string.Join(",", m_arrListBUnit));
                    m_objFilter.Add(TCMembersVM.Prop.BusinessUnitID.Map, m_lstFilter);
                }
            }
            if (!string.IsNullOrEmpty(PeriodStart) && !string.IsNullOrEmpty(PeriodEnd))
            {

                DateTime m_dtStart = DateTime.Parse(JSON.Deserialize(PeriodStart).ToString());
                DateTime m_dtEnd = DateTime.Parse(JSON.Deserialize(PeriodEnd).ToString());

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.None);
                m_lstFilter.Add("");
                m_objFilter.Add($@"(({TCMembersVM.Prop.PeriodStart.Map} <= '{m_dtEnd.ToString(Global.SqlDateFormat)}' AND {TCMembersVM.Prop.PeriodEnd.Map} >= '{m_dtEnd.ToString(Global.SqlDateFormat)}') OR 
                                    ({TCMembersVM.Prop.PeriodStart.Map} <= '{m_dtStart.ToString(Global.SqlDateFormat)}' AND {TCMembersVM.Prop.PeriodEnd.Map} >= '{m_dtStart.ToString(Global.SqlDateFormat)}')) OR 
                                    (({TCMembersVM.Prop.PeriodStart.Map} >= '{m_dtStart.ToString(Global.SqlDateFormat)}' AND {TCMembersVM.Prop.PeriodEnd.Map} >= '{m_dtStart.ToString(Global.SqlDateFormat)}') AND 
                                    ({TCMembersVM.Prop.PeriodStart.Map} <= '{m_dtEnd.ToString(Global.SqlDateFormat)}' AND {TCMembersVM.Prop.PeriodEnd.Map} <= '{m_dtEnd.ToString(Global.SqlDateFormat)}'))", m_lstFilter);

                //if (!m_dtPeriodStart.Equals(m_dtPeriodEnd))
                //{
                //    m_lstFilter = new List<object>();
                //    m_lstFilter.Add(Operator.None);
                //    m_lstFilter.Add("");
                //    m_objFilter.Add($"{TCMembersVM.Prop.PeriodStart.Map} <= '{m_dtPeriodEnd.ToString(Global.SqlDateFormat)}' AND {TCMembersVM.Prop.PeriodEnd.Map} >= '{m_dtPeriodEnd.ToString(Global.SqlDateFormat)}'", m_lstFilter); //Rony: pe changes to "less than equal"
                //}
            }

            if (!string.IsNullOrEmpty(Schedule))
            {
                DateTime FilterSchedule_ = DateTime.Parse(JSON.Deserialize(Schedule).ToString());

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.LessThanEqual);
                m_lstFilter.Add(FilterSchedule_);
                m_objFilter.Add(TCMembersVM.Prop.PeriodStart.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.GreaterThanEqual);
                m_lstFilter.Add(FilterSchedule_);
                m_objFilter.Add(TCMembersVM.Prop.PeriodEnd.Map, m_lstFilter);
            }
            
            Dictionary<int, DataSet> m_dicTTCMembersDA = m_objTTCMembersDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, m_lstGroup, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpTCMemberBL in m_dicTTCMembersDA)
            {
                m_intCount = m_kvpTCMemberBL.Key;
                break;
            }

            List<TCMembersVM> m_lsTCMembersVM = new List<TCMembersVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(TCMembersVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicTTCMembersDA = m_objTTCMembersDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, m_lstGroup, m_dicOrder, null);
                if (m_objTTCMembersDA.Message == string.Empty)
                {
                    m_lsTCMembersVM = (
                        from DataRow m_drTTCMembersDA in m_dicTTCMembersDA[0].Tables[0].Rows
                        select new TCMembersVM()
                        {
                            TCMemberID = m_drTTCMembersDA[TCMembersVM.Prop.TCMemberID.Name].ToString(),
                            EmployeeID = m_drTTCMembersDA[TCMembersVM.Prop.EmployeeID.Name].ToString(),
                            EmployeeName = m_drTTCMembersDA[TCMembersVM.Prop.EmployeeName.Name].ToString(),
                            PeriodStart = DateTime.Parse(m_drTTCMembersDA[TCMembersVM.Prop.PeriodStart.Name].ToString()),
                            PeriodEnd = DateTime.Parse(m_drTTCMembersDA[TCMembersVM.Prop.PeriodEnd.Name].ToString()),
                            TCTypeDesc = m_drTTCMembersDA[TCMembersVM.Prop.TCTypeDesc.Name].ToString(),
                            TCTypeID = m_drTTCMembersDA[TCMembersVM.Prop.TCTypeID.Name].ToString(),
                            BusinessUnitDesc = m_drTTCMembersDA[TCMembersVM.Prop.BusinessUnitDesc.Name].ToString(),
                            Email = m_drTTCMembersDA[TCMembersVM.Prop.Email.Name].ToString(),
                        }
                    ).ToList();
                }
            }

            return this.Store(m_lsTCMembersVM, m_intCount);
        }
        
        public ActionResult ReadFunction(StoreRequestParameters parameters, string TCMemberID)
        {
            MFunctionsDA m_objDFPTFunctionsDA = new MFunctionsDA();
            m_objDFPTFunctionsDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcDFPTFunctionss = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcDFPTFunctionss.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = FunctionsVM.Prop.Map(m_strDataIndex, false);
                    m_lstFilter = new List<object>();
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

            //m_lstFilter = new List<object>();
            //m_lstFilter.Add(Operator.Equals);
            //m_lstFilter.Add(NotificationTemplateID);
            //m_objFilter.Add(FunctionsVM.Prop.FunctionID.Map, m_lstFilter);

            //List<string> m_lstGroup = new List<string>();
            //m_lstGroup.Add(FunctionsVM.Prop.FunctionID.Map);
            //m_lstGroup.Add(FunctionsVM.Prop.FunctionDesc.Map);

            Dictionary<int, DataSet> m_dicDFPTFunctionsDA = m_objDFPTFunctionsDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpFPTFunctionsBL in m_dicDFPTFunctionsDA)
            {
                m_intCount = m_kvpFPTFunctionsBL.Key;
                break;
            }

            List<FunctionsVM> m_lstFunctionsVM = new List<FunctionsVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(FunctionsVM.Prop.FunctionID.MapAlias);
                m_lstSelect.Add(FunctionsVM.Prop.FunctionDesc.MapAlias);


                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(FunctionsVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicDFPTFunctionsDA = m_objDFPTFunctionsDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objDFPTFunctionsDA.Message == string.Empty)
                {
                    m_lstFunctionsVM = (
                        from DataRow m_drDFPTFunctionsDA in m_dicDFPTFunctionsDA[0].Tables[0].Rows
                        select new FunctionsVM()
                        {
                            FunctionID = m_drDFPTFunctionsDA[FunctionsVM.Prop.FunctionID.Name].ToString(),
                            FunctionDesc = m_drDFPTFunctionsDA[FunctionsVM.Prop.FunctionDesc.Name].ToString(),
                            Checked = IsChecked(m_drDFPTFunctionsDA[FunctionsVM.Prop.FunctionID.Name].ToString(), TCMemberID)
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstFunctionsVM, m_intCount);
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

            TCMembersVM m_objTCMembersVM = new TCMembersVM();
            ViewDataDictionary m_vddTCMember = new ViewDataDictionary();
            m_vddTCMember.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddTCMember.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
            if (Caller == General.EnumDesc(Buttons.ButtonDetail))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                Dictionary<string, object> m_dicSelectedRow = GetFormData(m_nvcParams);
                Session[dataSessionName] = JSON.Serialize(m_dicSelectedRow);
            }

            m_objTCMembersVM.ListTCMembersDelegationVM = new List<TCMembersDelegationVM>();
            m_objTCMembersVM.ListTCAppliedTypesVM = new List<TCAppliedTypesVM>();
            m_objTCMembersVM.ListTCFunctionsVM = new List<TCFunctionsVM>();
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objTCMembersVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddTCMember,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Detail(string Caller, string Selected, string TCMemberID)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            TCMembersVM m_objTCMembersVM = new TCMembersVM();
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
                if (m_dicSelectedRow.ContainsKey(TCMembersVM.Prop.TCMemberID.Name)
                    && string.IsNullOrEmpty(m_dicSelectedRow[TCMembersVM.Prop.TCMemberID.Name].ToString()))
                {
                    m_dicSelectedRow[TCMembersVM.Prop.TCMemberID.Name] = TCMemberID;

                }
            }

            if (m_dicSelectedRow.Count > 0)
                m_objTCMembersVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            m_objTCMembersVM.ListTCAppliedTypesVM = new List<TCAppliedTypesVM>();

            m_objTCMembersVM.ListTCMembersDelegationVM = GetListTCMembersDelegationVM(m_objTCMembersVM.TCMemberID);
            if (m_objTCMembersVM.ListTCMembersDelegationVM == null)
                m_objTCMembersVM.ListTCMembersDelegationVM = new List<TCMembersDelegationVM>();


            ViewDataDictionary m_vddWorkCenter = new ViewDataDictionary();
            m_vddWorkCenter.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objTCMembersVM,
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
            TCMembersVM m_objTCMembersVM = new TCMembersVM();
            if (m_dicSelectedRow.Count > 0)
                m_objTCMembersVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            //m_objTCMembersVM.ListTCAppliedTypesVM = GetListTCAppliedTypes(m_objTCMembersVM.TCMemberID,ref m_strMessage);
            //if (m_strMessage != string.Empty)
            //{
            //    Global.ShowErrorAlert(title, m_strMessage);
            //    return this.Direct();
            //}

            //if(!m_objTCMembersVM.ListTCAppliedTypesVM.Any())
            m_objTCMembersVM.ListTCAppliedTypesVM = new List<TCAppliedTypesVM>();
            m_objTCMembersVM.ListTCMembersDelegationVM = GetListTCMembersDelegationVM(m_objTCMembersVM.TCMemberID);
            if (m_objTCMembersVM.ListTCMembersDelegationVM == null)
                m_objTCMembersVM.ListTCMembersDelegationVM = new List<TCMembersDelegationVM>();

            ViewDataDictionary m_vddTCMember = new ViewDataDictionary();
            m_vddTCMember.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddTCMember.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objTCMembersVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddTCMember,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<TCMembersVM> m_lstSelectedRow = new List<TCMembersVM>();
            m_lstSelectedRow = JSON.Deserialize<List<TCMembersVM>>(Selected);

            TTCMembersDA m_objTTCMembersDA = new TTCMembersDA();
            m_objTTCMembersDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (TCMembersVM m_objTCMembersVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifTCMembersVM = m_objTCMembersVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifTCMembersVM in m_arrPifTCMembersVM)
                    {
                        string m_strFieldName = m_pifTCMembersVM.Name;
                        object m_objFieldValue = m_pifTCMembersVM.GetValue(m_objTCMembersVM);
                        if (m_objTCMembersVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(TCMembersVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objTTCMembersDA.DeleteBC(m_objFilter, false);
                    if (m_objTTCMembersDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objTTCMembersDA.Message);
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

        public ActionResult Browse(string ControlTCMemberID, string ControlEmployeeName, string ControlGrdTCMember,string ControlEmail,
            string FilterTCMemberID = "", string FilterEmployeeID = "", string FilterEmployeeName = "", string FilterSuperiorID = "", string FilterPeriodStart = "",
            string FilterPeriodEnd = "", string FilterSchedule = "", string FilterTCTypeID = "", string ValueTCMemberID = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddTCMember = new ViewDataDictionary();
            m_vddTCMember.Add("Control" + TCMembersVM.Prop.TCMemberID.Name, ControlTCMemberID);
            m_vddTCMember.Add("ControlGrdTCMember", ControlGrdTCMember);
            m_vddTCMember.Add("Control" + TCMembersVM.Prop.EmployeeName.Name, ControlEmployeeName);
            m_vddTCMember.Add("Control" + TCMembersVM.Prop.Email.Name, ControlEmail);
            m_vddTCMember.Add(TCMembersVM.Prop.TCMemberID.Name, FilterTCMemberID);
            m_vddTCMember.Add(TCMembersVM.Prop.EmployeeID.Name, FilterEmployeeID);
            m_vddTCMember.Add(TCMembersVM.Prop.EmployeeName.Name, FilterEmployeeName);
            m_vddTCMember.Add(TCMembersVM.Prop.SuperiorID.Name, FilterSuperiorID);
            m_vddTCMember.Add(TCMembersVM.Prop.PeriodStart.Name, FilterPeriodStart);
            m_vddTCMember.Add(TCMembersVM.Prop.PeriodEnd.Name, FilterPeriodEnd);
            m_vddTCMember.Add(FPTVM.Prop.Schedule.Name, FilterSchedule);
            m_vddTCMember.Add(TCMembersVM.Prop.TCTypeID.Name, FilterTCTypeID);
            m_vddTCMember.Add("Value" + TCMembersVM.Prop.TCMemberID.Name, ValueTCMemberID);

            string ListBusinessUnit = "";
            if (this.Request.Params["ListBusinessUnit"] != null)
            {
                ListBusinessUnit = this.Request.Params["ListBusinessUnit"];
            }
            m_vddTCMember.Add(BusinessUnitVM.Prop.BusinessUnitID.Name, ListBusinessUnit);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddTCMember,
                ViewName = "../TCMember/_Browse"
            };
        }

        public ActionResult Save(string Action)
        {
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            TTCMembersDA m_objTTCMembersDA = new TTCMembersDA();
            m_objTTCMembersDA.ConnectionStringName = Global.ConnStrConfigName;
            TTCMemberDelegationsDA m_objTTCMemberDelegationsDA = new TTCMemberDelegationsDA();
            m_objTTCMemberDelegationsDA.ConnectionStringName = Global.ConnStrConfigName;
            TTCAppliedTypesDA m_objTTCAppliedTypesDA = new TTCAppliedTypesDA();
            m_objTTCAppliedTypesDA.ConnectionStringName = Global.ConnStrConfigName;

            string m_strMessage = string.Empty;
            string m_strTCMemberID = string.Empty;
            string m_strDelegationID = string.Empty;

            string m_strTransName = "TCMember";
            object m_objDBConnection = null;
            m_objDBConnection = m_objTTCMembersDA.BeginTrans(m_strTransName);

            TCMembersVM m_objTCMembersVM = new TCMembersVM();
            List<TCFunctionsVM> m_listTCFunctionVM = new List<TCFunctionsVM>();
            try
            {
                m_strTCMemberID = this.Request.Params[TCMembersVM.Prop.TCMemberID.Name];
                string m_strEmployeeID = this.Request.Params[TCMembersVM.Prop.EmployeeID.Name];
                string m_strSuperiorID = this.Request.Params[TCMembersVM.Prop.SuperiorID.Name];
                string m_strSuperiorName = this.Request.Params[TCMembersVM.Prop.SuperiorName.Name];
                string m_strTCTypeID = this.Request.Params[TCMembersVM.Prop.TCTypeID.Name];
                string m_strBusinessUnitID = this.Request.Params[TCMembersVM.Prop.BusinessUnitID.Name];
                string m_strBusinessUnitDesc = this.Request.Params[TCMembersVM.Prop.BusinessUnitDesc.Name];
                string m_strPeriodStart = this.Request.Params[TCMembersVM.Prop.PeriodStart.Name];
                string m_strPeriodEnd = this.Request.Params[TCMembersVM.Prop.PeriodEnd.Name];

                m_strDelegationID = this.Request.Params[TCMembersVM.Prop.TCDelegationID.Name];
                string m_strDelegationTo = this.Request.Params[TCMembersVM.Prop.DelegateTo.Name];
                string m_strDelegationStartDate = this.Request.Params[TCMembersVM.Prop.DelegateStartDate.Name];
                string m_strDelegationStartEnd = this.Request.Params[TCMembersVM.Prop.DelegateEndDate.Name];
                string m_strTCEmployeeName = this.Request.Params[TCMembersVM.Prop.EmployeeName.Name];
                string m_strTCEmail = this.Request.Params[TCMembersVM.Prop.Email.Name];

                //string m_strListTCAppliedTypes = this.Request.Params[TCMembersVM.Prop.ListTCAppliedTypesVM.Name];
                string m_strListTCMemberDelegations = this.Request.Params[TCMembersVM.Prop.ListTCMembersDelegationVM.Name];

                //List<TCAppliedTypesVM> m_lstTCAppliedTypesVM = JSON.Deserialize<List<TCAppliedTypesVM>>(m_strListTCAppliedTypes);
                List<TCMembersDelegationVM> m_lstTCMembersDelegationVM = JSON.Deserialize<List<TCMembersDelegationVM>>(m_strListTCMemberDelegations);

                string m_lstFunction = this.Request.Params[TCMembersVM.Prop.ListTCFunctionVM.Name];

                List<FunctionsVM> m_lstFunctionVM = JSON.Deserialize<List<FunctionsVM>>(m_lstFunction);

                m_lstMessage = IsSaveValid(Action, m_strTCMemberID, m_strEmployeeID, m_strSuperiorID, m_strPeriodStart, m_strPeriodEnd, m_lstTCMembersDelegationVM);


                if (m_lstMessage.Count <= 0)
                {
                    if (IsDuplicateData(m_strEmployeeID, m_strTCMemberID, m_strPeriodStart, m_strPeriodEnd, ref m_strMessage))
                    {
                        Global.ShowErrorAlert(title, TCMembersVM.Prop.EmployeeName.Desc + " " + General.EnumDesc(MessageLib.exist));
                        return this.Direct();
                    }

                    TTCMembers m_objTTCMembers = new TTCMembers();
                    m_objTTCMembers.TCMemberID = m_strTCMemberID;
                    m_objTTCMembersDA.Data = m_objTTCMembers;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objTTCMembersDA.Select();

                    m_objTTCMembers.EmployeeID = m_strEmployeeID;
                    m_objTTCMembers.SuperiorID = m_strSuperiorID != string.Empty ? m_strSuperiorID : null;
                    m_objTTCMembers.PeriodStart = DateTime.Parse(m_strPeriodStart);
                    m_objTTCMembers.PeriodEnd = DateTime.Parse(m_strPeriodEnd);
                    m_objTTCMembers.TCTypeID = m_strTCTypeID;
                    m_objTTCMembers.BusinessUnitID = m_strBusinessUnitID;


                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objTTCMembersDA.Insert(true, m_objDBConnection);
                    else
                        m_objTTCMembersDA.Update(true, m_objDBConnection);

                    if (!m_objTTCMembersDA.Success || m_objTTCMembersDA.Message != string.Empty)
                    {
                        m_objTTCMembersDA.EndTrans(ref m_objDBConnection, m_strTransName);
                        m_lstMessage.Add(m_objTTCMembersDA.Message);
                    }

                    m_strTCMemberID = m_objTTCMembersDA.Data.TCMemberID;
                   

                    #region TTMemberDelegations
                    if (m_lstTCMembersDelegationVM.Any())
                    {
                        Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                        List<object> m_lstFilter = new List<object>();
                        m_lstFilter.Add(Operator.Equals);
                        m_lstFilter.Add(m_strTCMemberID);
                        m_objFilter.Add(TCMembersDelegationVM.Prop.TCMemberID.Map, m_lstFilter);

                        m_objTTCMemberDelegationsDA.DeleteBC(m_objFilter, true, m_objDBConnection);

                        foreach (TCMembersDelegationVM objTCMembersDelegationVM in m_lstTCMembersDelegationVM)
                        {
                            TTCMemberDelegations m_objTTCMemberDelegations = new TTCMemberDelegations();
                            m_objTTCMemberDelegationsDA.Data = m_objTTCMemberDelegations;

                            m_objTTCMemberDelegations.TCDelegationID = Guid.NewGuid().ToString().Replace("-", "");
                            m_objTTCMemberDelegations.TCMemberID = m_strTCMemberID;
                            m_objTTCMemberDelegations.DelegateTo = objTCMembersDelegationVM.DelegateTo;
                            m_objTTCMemberDelegations.DelegateStartDate = objTCMembersDelegationVM.DelegateStartDate.Value;
                            m_objTTCMemberDelegations.DelegateEndDate = objTCMembersDelegationVM.DelegateEndDate.Value;

                            m_objTTCMemberDelegationsDA.Insert(true, m_objDBConnection);

                            if (!m_objTTCMemberDelegationsDA.Success || m_objTTCMemberDelegationsDA.Message != string.Empty)
                            {
                                m_objTTCMembersDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                                m_lstMessage.Add(m_objTTCMemberDelegationsDA.Message);
                            }

                            objTCMembersDelegationVM.TCTypeID = m_strTCTypeID;
                            objTCMembersDelegationVM.BusinessUnitDesc = m_strBusinessUnitDesc;
                            objTCMembersDelegationVM.ListTCFunctionsVM = m_listTCFunctionVM;
                            objTCMembersDelegationVM.SuperiorName = m_strTCEmployeeName;
                        }

                        m_objTCMembersVM.ListTCMembersDelegationVM = m_lstTCMembersDelegationVM;

                    }
                    #endregion

                    #region DTCFunctions
                    DTCFunctionsDA m_objDTCFunctionsDA = new DTCFunctionsDA();
                    m_objDTCFunctionsDA.ConnectionStringName = Global.ConnStrConfigName;

                    if (Action == General.EnumDesc(Buttons.ButtonUpdate))
                    {
                        Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                        List<object> m_lstFilter = new List<object>();
                        m_lstFilter.Add(Operator.Equals);
                        m_lstFilter.Add(m_strTCMemberID);
                        m_objFilter.Add(TCFunctionsVM.Prop.TCMemberID.Map, m_lstFilter);


                        m_objDTCFunctionsDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                    }

                    foreach (FunctionsVM objFunctionsVM in m_lstFunctionVM)
                    {
                        DTCFunctions m_objDTCFunctions = new DTCFunctions();
                        m_objDTCFunctions.TCFunctionID = Guid.NewGuid().ToString().Replace("-", "");
                        m_objDTCFunctions.TCMemberID = m_strTCMemberID;
                        m_objDTCFunctions.FunctionID = objFunctionsVM.FunctionID;

                        m_listTCFunctionVM.Add(new TCFunctionsVM { FunctionDesc = objFunctionsVM.FunctionDesc });

                        m_objDTCFunctionsDA.Data = m_objDTCFunctions;
                        m_objDTCFunctionsDA.Select();

                        if (m_objDTCFunctionsDA.Message != string.Empty)
                            m_objDTCFunctionsDA.Insert(true, m_objDBConnection);

                        if (!m_objDTCFunctionsDA.Success || m_objDTCFunctionsDA.Message != string.Empty)
                        {
                            m_objDTCFunctionsDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                            return this.Direct(false, m_objDTCFunctionsDA.Message);
                        }
                    }


                    #endregion

                    if (!m_objTTCMembersDA.Success || m_objTTCMembersDA.Message != string.Empty)
                        m_lstMessage.Add(m_objTTCMembersDA.Message);
                    else 
                        m_objTTCMembersDA.EndTrans(ref m_objDBConnection, true, m_strTransName);

                    m_objTCMembersVM = new TCMembersVM()
                    {
                        TCMemberID = m_objTTCMembers.TCMemberID,
                        EmployeeName = m_strTCEmployeeName,
                        Email = m_strTCEmail,
                        EmployeeID = m_objTTCMembers.EmployeeID,
                        PeriodStart = m_objTTCMembers.PeriodStart,
                        PeriodEnd = m_objTTCMembers.PeriodEnd,
                        TCTypeID = m_objTTCMembers.TCTypeID,
                        BusinessUnitDesc = m_strBusinessUnitDesc,
                        ListTCFunctionsVM = m_listTCFunctionVM,
                        ListTCMembersDelegationVM = m_lstTCMembersDelegationVM
                    };

                }
            }
            catch (Exception ex)
            {
                m_objTTCMembersDA.EndTrans(ref m_objDBConnection, false, m_strTransName);

                m_lstMessage.Add(ex.Message);
            }
            if (m_lstMessage.Count <= 0)
            {
                try
                {
                    if(AutoEmailActive())
                        CreateMailNotif(m_objTCMembersVM, Action, ref m_strMessage);

                }
                catch (Exception ex)
                {
                }
                Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
                return Detail(General.EnumDesc(Buttons.ButtonSave), null, m_strTCMemberID);
            }
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);
        }

        public ActionResult DeleteTCMemberDelegation(string Selected)
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));


            List<TCMembersDelegationVM> m_lstSelectedRow = new List<TCMembersDelegationVM>();
            m_lstSelectedRow = JSON.Deserialize<List<TCMembersDelegationVM>>(Selected);

            TTCMemberDelegationsDA m_objTTCMemberDelegationsDA = new TTCMemberDelegationsDA();
            m_objTTCMemberDelegationsDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (TCMembersDelegationVM m_objTCMembersDelegationVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifItemPriceVendorVM = m_objTCMembersDelegationVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifItemPriceVendorVM in m_arrPifItemPriceVendorVM)
                    {
                        string m_strFieldName = m_pifItemPriceVendorVM.Name;
                        object m_objFieldValue = m_pifItemPriceVendorVM.GetValue(m_objTCMembersDelegationVM) ?? string.Empty;
                        if (m_objTCMembersDelegationVM.IsKey(m_strFieldName))
                        {

                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(TCMembersDelegationVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objTTCMemberDelegationsDA.DeleteBC(m_objFilter, false);
                    if (m_objTTCMemberDelegationsDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objTTCMemberDelegationsDA.Message);
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                return this.Direct(false, ex.Message);

            }
            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Deleted));
                return this.Direct(true, string.Empty);
            }
            else
                return this.Direct();

        }

        #endregion

        #region Direct Method

        public ActionResult GeTTCMembers(string ControlTCMemberID, string ControlTCMemberDesc, string FilterTCMemberID, string FilterTCMemberDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<TCMembersVM>> m_dicTCMemberData = GeTTCMembersData(true, FilterTCMemberID, FilterTCMemberDesc);
                KeyValuePair<int, List<TCMembersVM>> m_kvpTCMembersVM = m_dicTCMemberData.AsEnumerable().ToList()[0];
                if (m_kvpTCMembersVM.Key < 1 || (m_kvpTCMembersVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpTCMembersVM.Key > 1 && !Exact)
                    return Browse(ControlTCMemberID, ControlTCMemberDesc, "", FilterTCMemberID, FilterTCMemberDesc);

                m_dicTCMemberData = GeTTCMembersData(false, FilterTCMemberID, FilterTCMemberDesc);
                TCMembersVM m_objTCMembersVM = m_dicTCMemberData[0][0];
                this.GetCmp<TextField>(ControlTCMemberID).Value = m_objTCMembersVM.TCMemberID;
                this.GetCmp<TextField>(ControlTCMemberDesc).Value = m_objTCMembersVM.EmployeeName;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method
        private bool IsChecked(string FunctionID, string TCMemberID)
        {
            TCFunctionsVM m_objTCFunctionsVM = new TCFunctionsVM();
            DTCFunctionsDA m_objDTCFunctionsDA = new DTCFunctionsDA();
            m_objDTCFunctionsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(TCFunctionsVM.Prop.TCFunctionID.MapAlias);


            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(FunctionID);
            m_objFilter.Add(TCFunctionsVM.Prop.FunctionID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(TCMemberID);
            m_objFilter.Add(TCFunctionsVM.Prop.TCMemberID.Map, m_lstFilter);


            //Dictionary<string, OrderDirection> m_objOrderBy = new Dictionary<string, OrderDirection>();
            //m_objOrderBy.Add(NotificationMapVM.Prop.FunctionID.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicDTCFunctionsDA = m_objDTCFunctionsDA.SelectBC(0, 1, true, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpDTCFunctionsDA in m_dicDTCFunctionsDA)
            {
                m_intCount = m_kvpDTCFunctionsDA.Key;
                break;
            }
            return (m_intCount > 0 ? true : false);
        }
        private bool IsCheckedType(string TCTypeID, string TCMemberID)
        {
            TCAppliedTypesVM m_objTCAppliedTypesVM = new TCAppliedTypesVM();
            TTCAppliedTypesDA m_objTTCAppliedTypesDA = new TTCAppliedTypesDA();
            m_objTTCAppliedTypesDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(TCAppliedTypesVM.Prop.TCMemberID.MapAlias);
            m_lstSelect.Add(TCAppliedTypesVM.Prop.TCTypeID.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(TCTypeID);
            m_objFilter.Add(TCAppliedTypesVM.Prop.TCTypeID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(TCMemberID);
            m_objFilter.Add(TCAppliedTypesVM.Prop.TCMemberID.Map, m_lstFilter);

            Dictionary<string, OrderDirection> m_objOrderBy = new Dictionary<string, OrderDirection>();
            m_objOrderBy.Add(MenuVM.Prop.MenuID.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicTTCAppliedTypesDA = m_objTTCAppliedTypesDA.SelectBC(0, 1, true, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpMenuBL in m_dicTTCAppliedTypesDA)
            {
                m_intCount = m_kvpMenuBL.Key;
                break;
            }
            return (m_intCount > 0 ? true : false);
        }

        private List<string> IsSaveValid(string Action, string TCMemberID, string EmployeeID, string SuperiorID,
           string PeriodStart, string PeriodEnd, List<TCMembersDelegationVM> ListTCMembersDelegationVM)
        {
            string m_strMessage = string.Empty;
            List<string> m_lstReturn = new List<string>();

            if (string.IsNullOrEmpty(EmployeeID))
                m_lstReturn.Add(TCMembersVM.Prop.EmployeeName.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (string.IsNullOrEmpty(PeriodStart))
                m_lstReturn.Add(TCMembersVM.Prop.PeriodStart.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (string.IsNullOrEmpty(PeriodEnd))
                m_lstReturn.Add(TCMembersVM.Prop.PeriodEnd.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            //if (!ListTCAppliedTypesVM.Any())
            //{
            //    m_lstReturn.Add(TCMembersVM.Prop.TCTypeID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            //}
            if (!string.IsNullOrEmpty(PeriodStart) && !string.IsNullOrEmpty(PeriodEnd))
            {
                if (DateTime.Parse(PeriodStart) > DateTime.Parse(PeriodEnd))
                {
                    m_lstReturn.Add("Period Date Range " + General.EnumDesc(MessageLib.invalid));
                }
            }

            foreach (TCMembersDelegationVM objTCMembersDelegationVM in ListTCMembersDelegationVM)
            {


                if (!string.IsNullOrEmpty(objTCMembersDelegationVM.DelegateTo))
                {

                    if (objTCMembersDelegationVM.DelegateEndDate != null && objTCMembersDelegationVM.DelegateEndDate != null)
                    {
                        if (objTCMembersDelegationVM.DelegateStartDate > objTCMembersDelegationVM.DelegateEndDate)
                        {
                            m_lstReturn.Add("Delegate Date Range " + General.EnumDesc(MessageLib.invalid));
                        }
                    }

                    if (objTCMembersDelegationVM.DelegateEndDate == null)
                        m_lstReturn.Add(TCMembersVM.Prop.DelegateStartDate.Desc + " " + General.EnumDesc(MessageLib.mustFill));
                    else
                    {
                        if (objTCMembersDelegationVM.DelegateStartDate < DateTime.Parse(PeriodStart) || objTCMembersDelegationVM.DelegateStartDate > DateTime.Parse(PeriodEnd))
                            m_lstReturn.Add(TCMembersVM.Prop.DelegateStartDate.Desc + " must be in Period Date Range");
                    }

                    if (objTCMembersDelegationVM.DelegateEndDate == null)
                        m_lstReturn.Add(TCMembersVM.Prop.DelegateEndDate.Desc + " " + General.EnumDesc(MessageLib.mustFill));
                    else
                    {
                        if (objTCMembersDelegationVM.DelegateEndDate < DateTime.Parse(PeriodStart) || objTCMembersDelegationVM.DelegateEndDate > DateTime.Parse(PeriodEnd))
                            m_lstReturn.Add(TCMembersVM.Prop.DelegateEndDate.Desc + " must be in Period Date Range");
                    }


                    if (ListTCMembersDelegationVM.Any(d => (
                                                            (objTCMembersDelegationVM.DelegateStartDate >= d.DelegateStartDate && objTCMembersDelegationVM.DelegateStartDate < d.DelegateEndDate) ||
                                                            (objTCMembersDelegationVM.DelegateEndDate >= d.DelegateStartDate && objTCMembersDelegationVM.DelegateEndDate < d.DelegateEndDate)) &&
                                                            objTCMembersDelegationVM.DelegateTo == d.DelegateTo && (d.DelegateStartDate != objTCMembersDelegationVM.DelegateStartDate && d.DelegateEndDate != objTCMembersDelegationVM.DelegateEndDate)))
                    {
                        m_lstReturn.Add("Delegate Date Range " + General.EnumDesc(MessageLib.invalid));
                        break;
                    }

                }
            }

            if (!string.IsNullOrEmpty(SuperiorID) && SuperiorID == EmployeeID)
                m_lstReturn.Add(TCMembersVM.Prop.SuperiorName.Desc + " " + General.EnumDesc(MessageLib.invalid));



            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(TCMembersVM.Prop.TCMemberID.Name, parameters[TCMembersVM.Prop.TCMemberID.Name]);
            m_dicReturn.Add(TCMembersVM.Prop.EmployeeID.Name, parameters[TCMembersVM.Prop.EmployeeID.Name]);
            m_dicReturn.Add(TCMembersVM.Prop.EmployeeName.Name, parameters[TCMembersVM.Prop.EmployeeName.Name]);
            m_dicReturn.Add(TCMembersVM.Prop.SuperiorID.Name, parameters[TCMembersVM.Prop.SuperiorID.Name]);
            m_dicReturn.Add(TCMembersVM.Prop.SuperiorName.Name, parameters[TCMembersVM.Prop.SuperiorName.Name]);
            //m_dicReturn.Add(TCMembersVM.Prop.TCTypeID.Name, parameters[TCMembersVM.Prop.TCTypeID.Name]);
            m_dicReturn.Add(TCMembersVM.Prop.PeriodStart.Name, parameters[TCMembersVM.Prop.PeriodStart.Name]);
            m_dicReturn.Add(TCMembersVM.Prop.PeriodEnd.Name, parameters[TCMembersVM.Prop.PeriodEnd.Name]);

            return m_dicReturn;
        }

        private TCMembersVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            TCMembersVM m_objTCMembersVM = new TCMembersVM();
            TTCMembersDA m_objTTCMembersDA = new TTCMembersDA();
            m_objTTCMembersDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(TCMembersVM.Prop.TCMemberID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.EmployeeID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.EmployeeName.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.SuperiorID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.SuperiorName.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.PeriodStart.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.PeriodEnd.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.DelegateStartDate.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.DelegateEndDate.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.DelegateTo.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.TCTypeID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.BusinessUnitID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.BusinessUnitDesc.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.TCTypeParentID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.Email.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objTCMembersVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(TCMembersVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicTTCMembersDA = m_objTTCMembersDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objTTCMembersDA.Message == string.Empty)
            {
                DataRow m_drTTCMembersDA = m_dicTTCMembersDA[0].Tables[0].Rows[0];
                m_objTCMembersVM.TCMemberID = m_drTTCMembersDA[TCMembersVM.Prop.TCMemberID.Name].ToString();
                m_objTCMembersVM.EmployeeID = m_drTTCMembersDA[TCMembersVM.Prop.EmployeeID.Name].ToString();
                m_objTCMembersVM.EmployeeName = m_drTTCMembersDA[TCMembersVM.Prop.EmployeeName.Name].ToString();
                m_objTCMembersVM.SuperiorID = m_drTTCMembersDA[TCMembersVM.Prop.SuperiorID.Name].ToString();
                m_objTCMembersVM.SuperiorName = m_drTTCMembersDA[TCMembersVM.Prop.SuperiorName.Name].ToString();
                m_objTCMembersVM.PeriodStart = DateTime.Parse(m_drTTCMembersDA[TCMembersVM.Prop.PeriodStart.Name].ToString());
                m_objTCMembersVM.PeriodEnd = DateTime.Parse(m_drTTCMembersDA[TCMembersVM.Prop.PeriodEnd.Name].ToString());
                m_objTCMembersVM.DelegateStartDate = !string.IsNullOrEmpty(m_drTTCMembersDA[TCMembersVM.Prop.DelegateStartDate.Name].ToString()) ? DateTime.Parse(m_drTTCMembersDA[TCMembersVM.Prop.DelegateStartDate.Name].ToString()) : (DateTime?)null;
                m_objTCMembersVM.DelegateEndDate = !string.IsNullOrEmpty(m_drTTCMembersDA[TCMembersVM.Prop.DelegateStartDate.Name].ToString()) ? DateTime.Parse(m_drTTCMembersDA[TCMembersVM.Prop.DelegateEndDate.Name].ToString()) : (DateTime?)null;
                m_objTCMembersVM.DelegateTo = m_drTTCMembersDA[TCMembersVM.Prop.DelegateTo.Name].ToString();
                m_objTCMembersVM.TCTypeID = m_drTTCMembersDA[TCMembersVM.Prop.TCTypeID.Name].ToString();
                m_objTCMembersVM.BusinessUnitID = m_drTTCMembersDA[TCMembersVM.Prop.BusinessUnitID.Name].ToString();
                m_objTCMembersVM.BusinessUnitDesc = m_drTTCMembersDA[TCMembersVM.Prop.BusinessUnitDesc.Name].ToString();
                m_objTCMembersVM.Email = m_drTTCMembersDA[TCMembersVM.Prop.Email.Name].ToString();
                m_objTCMembersVM.TCTypeParentID = string.IsNullOrEmpty(m_drTTCMembersDA[TCMembersVM.Prop.TCTypeParentID.Name].ToString()) ? "-" : m_drTTCMembersDA[TCMembersVM.Prop.TCTypeParentID.Name].ToString();
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objTTCMembersDA.Message;

            return m_objTCMembersVM;
        }

        private bool IsDuplicateData(string EmployeeID, string TCMemberID, string PeriodStart, string PeriodEnd, ref string message)
        {
            TCMembersVM m_objTCMembersVM = new TCMembersVM();
            TTCMembersDA m_objTTCMembersDA = new TTCMembersDA();
            m_objTTCMembersDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(TCMembersVM.Prop.TCMemberID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.EmployeeID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.EmployeeName.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.SuperiorID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.SuperiorName.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.PeriodStart.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.PeriodEnd.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.DelegateStartDate.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.DelegateEndDate.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.DelegateTo.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(EmployeeID);
            m_objFilter.Add(TCMembersVM.Prop.EmployeeID.Map, m_lstFilter);

            if (!string.IsNullOrEmpty(TCMemberID))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.NotEqual);
                m_lstFilter.Add(TCMemberID);
                m_objFilter.Add(TCMembersVM.Prop.TCMemberID.Map, m_lstFilter);
            }
            //if (string.IsNullOrEmpty(SuperiorID))
            //{
            //    m_lstFilter = new List<object>();
            //    m_lstFilter.Add(Operator.None);
            //    m_lstFilter.Add("");
            //    m_objFilter.Add(TCMembersVM.Prop.SuperiorID.Map+" IS NULL", m_lstFilter);
            //}
            //else {
            //    m_lstFilter = new List<object>();
            //    m_lstFilter.Add(Operator.Equals);
            //    m_lstFilter.Add(SuperiorID);
            //    m_objFilter.Add(TCMembersVM.Prop.SuperiorID.Map, m_lstFilter);
            //}

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.None);
            m_lstFilter.Add("");
            m_objFilter.Add(string.Format("1=1 AND (('{2}' BETWEEN {0} AND {1}) OR ('{3}' BETWEEN {0} AND {1}))",
                TCMembersVM.Prop.PeriodStart.Map, TCMembersVM.Prop.PeriodEnd.Map, DateTime.Parse(PeriodStart), DateTime.Parse(PeriodEnd)), m_lstFilter);


            Dictionary<int, DataSet> m_dicTTCMembersDA = m_objTTCMembersDA.SelectBC(0, 1, true, m_lstSelect, m_objFilter, null, null, null);

            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpTCMemberBL in m_dicTTCMembersDA)
            {
                m_intCount = m_kvpTCMemberBL.Key;
                break;
            }

            return m_intCount > 0 ? true : false;
        }

        private List<TCMembersDelegationVM> GetListTCMembersDelegationVM(string TCMemberID)
        {
            List<TCMembersDelegationVM> m_listTCMembersDelegationVM = new List<TCMembersDelegationVM>();
            TTCMemberDelegationsDA m_objTTCMemberDelegationsDA = new TTCMemberDelegationsDA();
            m_objTTCMemberDelegationsDA.ConnectionStringName = Global.ConnStrConfigName;

            FilterHeaderConditions m_fhcTTCMembers = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(TCMemberID);
            m_objFilter.Add(TCMembersDelegationVM.Prop.TCMemberID.Map, m_lstFilter);

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(TCMembersDelegationVM.Prop.TCDelegationID.MapAlias);
            m_lstSelect.Add(TCMembersDelegationVM.Prop.TCMemberID.MapAlias);
            m_lstSelect.Add(TCMembersDelegationVM.Prop.DelegateTo.MapAlias);
            m_lstSelect.Add(TCMembersDelegationVM.Prop.DelegateName.MapAlias);
            m_lstSelect.Add(TCMembersDelegationVM.Prop.DelegateStartDate.MapAlias);
            m_lstSelect.Add(TCMembersDelegationVM.Prop.DelegateEndDate.MapAlias);
            m_lstSelect.Add(TCMembersDelegationVM.Prop.TCEmail.MapAlias);
            m_lstSelect.Add(TCMembersDelegationVM.Prop.TCTypeID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.BusinessUnitDesc.MapAlias);

            Dictionary<int, DataSet> m_dicTTCMemberDelegationsDA = m_objTTCMemberDelegationsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);
            if (m_objTTCMemberDelegationsDA.Message == string.Empty)
            {
                m_listTCMembersDelegationVM = (
                    from DataRow m_drTTCMembersDA in m_dicTTCMemberDelegationsDA[0].Tables[0].Rows
                    select new TCMembersDelegationVM()
                    {
                        TCDelegationID = m_drTTCMembersDA[TCMembersDelegationVM.Prop.TCDelegationID.Name].ToString(),
                        TCMemberID = m_drTTCMembersDA[TCMembersDelegationVM.Prop.TCMemberID.Name].ToString(),
                        DelegateTo = m_drTTCMembersDA[TCMembersDelegationVM.Prop.DelegateTo.Name].ToString(),
                        DelegateName = m_drTTCMembersDA[TCMembersDelegationVM.Prop.DelegateName.Name].ToString(),
                        TCEmail = m_drTTCMembersDA[TCMembersDelegationVM.Prop.TCEmail.Name].ToString(),
                        TCTypeID = m_drTTCMembersDA[TCMembersDelegationVM.Prop.TCTypeID.Name].ToString(),
                        BusinessUnitDesc = m_drTTCMembersDA[TCMembersVM.Prop.BusinessUnitDesc.Name].ToString(),
                        DelegateStartDate = !string.IsNullOrEmpty(m_drTTCMembersDA[TCMembersDelegationVM.Prop.DelegateStartDate.Name].ToString()) ? DateTime.Parse(m_drTTCMembersDA[TCMembersDelegationVM.Prop.DelegateStartDate.Name].ToString()) : (DateTime?)null,
                        DelegateEndDate = !string.IsNullOrEmpty(m_drTTCMembersDA[TCMembersDelegationVM.Prop.DelegateEndDate.Name].ToString()) ? DateTime.Parse(m_drTTCMembersDA[TCMembersDelegationVM.Prop.DelegateEndDate.Name].ToString()) : (DateTime?)null,
                    }
                ).ToList();
            }

            return m_listTCMembersDelegationVM;
        }

        private void CreateMailNotif(TCMembersVM objTCMembersVM, string Action, ref string message)
        {
            string m_strMailNotifID = string.Empty;
            string m_strFormat = string.Empty;
            string m_strCultureInfo = string.Empty;
            string m_strType = string.Empty;

            List<String> m_lstMessage = new List<string>();
            var m_lstconfig = GetConfig("FunctionID", "","TC");

            NotificationMapVM m_NotificationMapVM = GetDefaultNoticationMap(m_lstconfig.Where(x => x.Key2 == "TCAssignment").FirstOrDefault().Desc1);//todo: handling exception
            string m_strsubject = $"Tender Committee Assignment";
            MailNotificationsVM m_MailNotificationsVM = new MailNotificationsVM();
            m_MailNotificationsVM.MailNotificationID = "";
            m_MailNotificationsVM.Importance = true;
            m_MailNotificationsVM.Subject = $"{m_strsubject}";//todo
            m_MailNotificationsVM.Contents = "";
            m_MailNotificationsVM.StatusID = (int)NotificationStatus.Draft;
            //m_MailNotificationsVM.NotifMapID = m_NotificationMapVM.NotifMapID;
            m_MailNotificationsVM.FunctionID = m_NotificationMapVM.FunctionID;
            m_MailNotificationsVM.NotificationTemplateID = m_NotificationMapVM.NotificationTemplateID;

            //Recipient
            RecipientsVM m_RecipientsVM = new RecipientsVM();
            m_RecipientsVM.RecipientID = "";
            m_RecipientsVM.RecipientDesc = objTCMembersVM.EmployeeName;
            m_RecipientsVM.MailAddress = objTCMembersVM.Email;
            m_RecipientsVM.OwnerID = objTCMembersVM.EmployeeID;
            m_RecipientsVM.RecipientTypeID = ((int)RecipientTypes.TO).ToString();
            List<RecipientsVM> m_lstm_RecipientsVM = new List<RecipientsVM>();
            m_lstm_RecipientsVM.Add(m_RecipientsVM);
            m_MailNotificationsVM.RecipientsVM = m_lstm_RecipientsVM;


            //Values

            m_MailNotificationsVM.NotificationValuesVM = new List<NotificationValuesVM>();
            
            List<TemplateTagsVM> m_objListTemplateTagsVM = GetListTemplateTagsVM(m_NotificationMapVM.NotificationTemplateID,"NOT");
            PropertyInfo[] m_arrPInfoTCMembers = objTCMembersVM.GetType().GetProperties();
            foreach (TemplateTagsVM objTemplateTagsVM in m_objListTemplateTagsVM)
            {
                if (m_arrPInfoTCMembers.Any(d => d.Name.Equals(objTemplateTagsVM.RefIDColumn)))
                {
                    NotificationValuesVM objNotificationValuesVM = new NotificationValuesVM();
                    objNotificationValuesVM.NotificationValueID = Guid.NewGuid().ToString().Replace("-","");
                    objNotificationValuesVM.Value = m_arrPInfoTCMembers.FirstOrDefault(d => d.Name.Equals(objTemplateTagsVM.RefIDColumn)).GetValue(objTCMembersVM).ToString();

                    m_strType = string.Empty;
                    m_strFormat = string.Empty;
                    m_strCultureInfo = string.Empty;
                    if (objTemplateTagsVM.Config != null)
                    {
                        m_strType = objTemplateTagsVM.Config.Key2;
                        m_strFormat = objTemplateTagsVM.Config.Key3;
                        m_strCultureInfo = objTemplateTagsVM.Config.Key4;
                    }
                    if ((Type)m_arrPInfoTCMembers.FirstOrDefault(d => d.Name.Equals(objTemplateTagsVM.RefIDColumn)).PropertyType == typeof(DateTime))
                        objNotificationValuesVM.Value = DateTime.Parse(m_arrPInfoTCMembers.FirstOrDefault(d => d.Name.Equals(objTemplateTagsVM.RefIDColumn)).GetValue(objTCMembersVM).ToString()).ToString(m_strFormat, new System.Globalization.CultureInfo(m_strCultureInfo));
                    if ((Type)m_arrPInfoTCMembers.FirstOrDefault(d => d.Name.Equals(objTemplateTagsVM.RefIDColumn)).PropertyType == typeof(List<TCFunctionsVM>))
                        objNotificationValuesVM.Value = string.Join(", ", ((List<TCFunctionsVM>)(m_arrPInfoTCMembers.FirstOrDefault(d => d.Name.Equals(objTemplateTagsVM.RefIDColumn)).GetValue(objTCMembersVM))).Select(d=>d.FunctionDesc).ToList());

                    objNotificationValuesVM.FieldTagID = objTemplateTagsVM.FieldTagID;

                    m_MailNotificationsVM.NotificationValuesVM.Add(objNotificationValuesVM);
                }

            }

           
            m_MailNotificationsVM.NotificationMapVM = m_NotificationMapVM;

            if (!this.CreateMailNotification(m_MailNotificationsVM, true, ref m_lstMessage,ref m_strMailNotifID))
                Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));

            #region Delegations
            if (objTCMembersVM.ListTCMembersDelegationVM!=null) {
                foreach (TCMembersDelegationVM objTCMembersDelegationVM in objTCMembersVM.ListTCMembersDelegationVM)
                {

                    m_NotificationMapVM = GetDefaultNoticationMap(m_lstconfig.Where(x => x.Key2 == "TCDelegations").FirstOrDefault().Desc1);//todo: handling exception
                    m_strsubject = $"Tender Committee Delegations";
                    m_MailNotificationsVM = new MailNotificationsVM();
                    m_MailNotificationsVM.MailNotificationID = "";
                    m_MailNotificationsVM.Importance = true;
                    m_MailNotificationsVM.Subject = $"{m_strsubject}";//todo
                    m_MailNotificationsVM.Contents = "";
                    m_MailNotificationsVM.StatusID = (int)NotificationStatus.Draft;
                    //m_MailNotificationsVM.NotifMapID = m_NotificationMapVM.NotifMapID;
                    m_MailNotificationsVM.FunctionID = m_NotificationMapVM.FunctionID;
                    m_MailNotificationsVM.NotificationTemplateID = m_NotificationMapVM.NotificationTemplateID;

                    //Recipient TO
                    m_lstm_RecipientsVM = new List<RecipientsVM>();
                    m_RecipientsVM = new RecipientsVM();
                    m_RecipientsVM.RecipientID = "";
                    m_RecipientsVM.RecipientDesc = objTCMembersDelegationVM.DelegateName;
                    m_RecipientsVM.MailAddress = objTCMembersDelegationVM.TCEmail;
                    m_RecipientsVM.OwnerID = objTCMembersDelegationVM.EmployeeID;
                    m_RecipientsVM.RecipientTypeID = ((int)RecipientTypes.TO).ToString();
                    
                    m_lstm_RecipientsVM.Add(m_RecipientsVM);

                    ////CC
                    m_RecipientsVM = new RecipientsVM();
                    m_RecipientsVM.RecipientID = "";
                    m_RecipientsVM.RecipientDesc = objTCMembersVM.EmployeeName;
                    m_RecipientsVM.MailAddress = objTCMembersVM.Email;
                    m_RecipientsVM.OwnerID = objTCMembersVM.EmployeeID;
                    m_RecipientsVM.RecipientTypeID = ((int)RecipientTypes.CC).ToString();
                    m_lstm_RecipientsVM.Add(m_RecipientsVM);

                    m_MailNotificationsVM.RecipientsVM = m_lstm_RecipientsVM;


                    //Values

                    m_MailNotificationsVM.NotificationValuesVM = new List<NotificationValuesVM>();

                    m_objListTemplateTagsVM = GetListTemplateTagsVM(m_NotificationMapVM.NotificationTemplateID, "NOT");
                    PropertyInfo[] m_arrPInfoTCMemberDelegations = objTCMembersDelegationVM.GetType().GetProperties();
                    foreach (TemplateTagsVM objTemplateTagsVM in m_objListTemplateTagsVM)
                    {
                        if (m_arrPInfoTCMemberDelegations.Any(d => d.Name.Equals(objTemplateTagsVM.RefIDColumn)))
                        {
                            NotificationValuesVM objNotificationValuesVM = new NotificationValuesVM();
                            objNotificationValuesVM.NotificationValueID = Guid.NewGuid().ToString().Replace("-", "");
                            objNotificationValuesVM.Value = m_arrPInfoTCMemberDelegations.FirstOrDefault(d => d.Name.Equals(objTemplateTagsVM.RefIDColumn)).GetValue(objTCMembersDelegationVM).ToString();

                            m_strType = string.Empty;
                            m_strFormat = string.Empty;
                            m_strCultureInfo = string.Empty;
                            if (objTemplateTagsVM.Config != null)
                            {
                                m_strType = objTemplateTagsVM.Config.Key2;
                                m_strFormat = objTemplateTagsVM.Config.Key3;
                                m_strCultureInfo = objTemplateTagsVM.Config.Key4;
                            }
                            if ((Type)m_arrPInfoTCMemberDelegations.FirstOrDefault(d => d.Name.Equals(objTemplateTagsVM.RefIDColumn)).PropertyType == typeof(DateTime?))
                                objNotificationValuesVM.Value = DateTime.Parse(m_arrPInfoTCMemberDelegations.FirstOrDefault(d => d.Name.Equals(objTemplateTagsVM.RefIDColumn)).GetValue(objTCMembersDelegationVM).ToString()).ToString(m_strFormat, new System.Globalization.CultureInfo(m_strCultureInfo));
                            if ((Type)m_arrPInfoTCMembers.FirstOrDefault(d => d.Name.Equals(objTemplateTagsVM.RefIDColumn)).PropertyType == typeof(List<TCFunctionsVM>))
                                objNotificationValuesVM.Value = string.Join(", ", ((List<TCFunctionsVM>)(m_arrPInfoTCMemberDelegations.FirstOrDefault(d => d.Name.Equals(objTemplateTagsVM.RefIDColumn)).GetValue(objTCMembersDelegationVM))).Select(d => d.FunctionDesc).ToList());


                            objNotificationValuesVM.FieldTagID = objTemplateTagsVM.FieldTagID;
                            m_MailNotificationsVM.NotificationValuesVM.Add(objNotificationValuesVM);
                        }

                    }


                    m_MailNotificationsVM.NotificationMapVM = m_NotificationMapVM;

                    if (!this.CreateMailNotification(m_MailNotificationsVM, true, ref m_lstMessage,ref m_strMailNotifID))
                        Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
                }
            }
            #endregion


        }

        private NotificationMapVM GetDefaultNoticationMap(string FunctionID)
        {
            DNotificationMapDA m_objDNotificationMapDA = new DNotificationMapDA();
            m_objDNotificationMapDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(NotificationMapVM.Prop.NotifMapID.MapAlias);
            m_lstSelect.Add(NotificationMapVM.Prop.FunctionID.MapAlias);
            m_lstSelect.Add(NotificationMapVM.Prop.IsDefault.MapAlias);
            m_lstSelect.Add(NotificationMapVM.Prop.NotificationTemplateID.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(FunctionID);
            m_objFilter.Add(NotificationMapVM.Prop.FunctionID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(1);
            m_objFilter.Add(NotificationMapVM.Prop.IsDefault.Map, m_lstFilter);


            //Dictionary<string, OrderDirection> m_objOrderBy = new Dictionary<string, OrderDirection>();
            //m_objOrderBy.Add(NotificationMapVM.Prop.FunctionID.Map, OrderDirection.Ascending);
            NotificationMapVM m_objNotificationMapVM = new NotificationMapVM();
            Dictionary<int, DataSet> m_dicDNotificationMap = m_objDNotificationMapDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null, null);
            if (m_objDNotificationMapDA.Message == string.Empty)
            {
                DataRow m_drDNotificationMap = m_dicDNotificationMap[0].Tables[0].Rows[0];
                m_objNotificationMapVM = new NotificationMapVM
                {
                    NotifMapID = m_drDNotificationMap[NotificationMapVM.Prop.NotifMapID.Name].ToString(),
                    FunctionID = m_drDNotificationMap[NotificationMapVM.Prop.FunctionID.Name].ToString(),
                    NotificationTemplateID = m_drDNotificationMap[NotificationMapVM.Prop.NotificationTemplateID.Name].ToString(),
                    IsDefault = (bool)m_drDNotificationMap[NotificationMapVM.Prop.IsDefault.Name]
                };
            }
            return m_objNotificationMapVM;
        }

        private List<TemplateTagsVM> GetListTemplateTagsVM(string TemplateID, string TemplateType)
        {
            List<TemplateTagsVM> m_lstTemplateTagsVM = new List<TemplateTagsVM>();
            DTemplateTagsDA m_objDTemplateTagsDA = new DTemplateTagsDA();
            m_objDTemplateTagsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(TemplateTagsVM.Prop.TemplateTagID.MapAlias);
            m_lstSelect.Add(TemplateTagsVM.Prop.TemplateID.MapAlias);
            m_lstSelect.Add(TemplateTagsVM.Prop.FieldTagID.MapAlias);
            m_lstSelect.Add(TemplateTagsVM.Prop.TagDesc.MapAlias);
            m_lstSelect.Add(TemplateTagsVM.Prop.TemplateType.MapAlias);
            m_lstSelect.Add(TemplateTagsVM.Prop.RefTable.MapAlias);
            m_lstSelect.Add(TemplateTagsVM.Prop.RefIDColumn.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(TemplateID);
            m_objFilter.Add(TemplateTagsVM.Prop.TemplateID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(TemplateType);
            m_objFilter.Add(TemplateTagsVM.Prop.TemplateType.Map, m_lstFilter);


            Dictionary<int, DataSet> m_dicTNotificationValuesDA = m_objDTemplateTagsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDTemplateTagsDA.Success)
            {
                foreach (DataRow m_drTNotificationValuesDA in m_dicTNotificationValuesDA[0].Tables[0].Rows)
                {
                    TemplateTagsVM m_objTemplateTagsVM = new TemplateTagsVM();
                    m_objTemplateTagsVM.TemplateTagID = m_drTNotificationValuesDA[TemplateTagsVM.Prop.TemplateTagID.Name].ToString();
                    m_objTemplateTagsVM.TemplateID = m_drTNotificationValuesDA[TemplateTagsVM.Prop.TemplateID.Name].ToString();
                    m_objTemplateTagsVM.FieldTagID = m_drTNotificationValuesDA[TemplateTagsVM.Prop.FieldTagID.Name].ToString();
                    m_objTemplateTagsVM.TagDesc = m_drTNotificationValuesDA[TemplateTagsVM.Prop.TagDesc.Name].ToString();
                    m_objTemplateTagsVM.TemplateType = m_drTNotificationValuesDA[TemplateTagsVM.Prop.TemplateType.Name].ToString();
                    m_objTemplateTagsVM.RefIDColumn = m_drTNotificationValuesDA[TemplateTagsVM.Prop.RefIDColumn.Name].ToString();
                    m_objTemplateTagsVM.RefTable = m_drTNotificationValuesDA[TemplateTagsVM.Prop.RefTable.Name].ToString();
                    m_objTemplateTagsVM.Config = GetConfig(m_objTemplateTagsVM.FieldTagID).FirstOrDefault();
                    m_lstTemplateTagsVM.Add(m_objTemplateTagsVM);
                }
            }
            return m_lstTemplateTagsVM;
        }
        #endregion

        #region Public Method

        public Dictionary<int, List<TCMembersVM>> GeTTCMembersData(bool isCount, string TCMemberID, string TCMemberDesc)
        {
            int m_intCount = 0;
            List<TCMembersVM> m_lsTCMembersVM = new List<ViewModels.TCMembersVM>();
            Dictionary<int, List<TCMembersVM>> m_dicReturn = new Dictionary<int, List<TCMembersVM>>();
            TTCMembersDA m_objTTCMembersDA = new TTCMembersDA();
            m_objTTCMembersDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(TCMembersVM.Prop.TCMemberID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.EmployeeName.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(TCMemberID);
            m_objFilter.Add(TCMembersVM.Prop.TCMemberID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(TCMemberDesc);
            m_objFilter.Add(TCMembersVM.Prop.EmployeeName.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicTTCMembersDA = m_objTTCMembersDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objTTCMembersDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpTCMemberBL in m_dicTTCMembersDA)
                    {
                        m_intCount = m_kvpTCMemberBL.Key;
                        break;
                    }
                else
                {
                    m_lsTCMembersVM = (
                        from DataRow m_drTTCMembersDA in m_dicTTCMembersDA[0].Tables[0].Rows
                        select new TCMembersVM()
                        {
                            TCMemberID = m_drTTCMembersDA[TCMembersVM.Prop.TCMemberID.Name].ToString(),
                            EmployeeName = m_drTTCMembersDA[TCMembersVM.Prop.EmployeeName.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lsTCMembersVM);
            return m_dicReturn;
        }

        public ActionResult GetTCTypeList(StoreRequestParameters parameters)
        {
            List<TCTypesVM> m_objTCTypesVM = new List<TCTypesVM>();

            DataAccess.MTCTypesDA m_objMTCTypesDA = new DataAccess.MTCTypesDA();
            m_objMTCTypesDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(TCTypesVM.Prop.TCTypeID.MapAlias);
            m_lstSelect.Add(TCTypesVM.Prop.TCTypeDesc.MapAlias);
            m_lstSelect.Add(TCTypesVM.Prop.TCTypeParentID.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            //m_lstFilter.Add(Operator.Equals);
            //m_lstFilter.Add(typeof(DBudgetPlanBidOpening).Name);
            //m_objFilter.Add(StatusVM.Prop.TableName.Name, m_lstFilter);

            Dictionary<int, System.Data.DataSet> m_dicMTCTypesDA = m_objMTCTypesDA.SelectBC(0, null, false, m_lstSelect, null, null, null, null, null);

            if (m_objMTCTypesDA.Message == String.Empty)
            {
                for (int i = 0; i < m_dicMTCTypesDA[0].Tables[0].Rows.Count; i++)
                {
                    m_objTCTypesVM.Add(
                        new TCTypesVM()
                        {
                            TCTypeDesc = m_dicMTCTypesDA[0].Tables[0].Rows[i][TCTypesVM.Prop.TCTypeDesc.Name].ToString(),
                            TCTypeID = m_dicMTCTypesDA[0].Tables[0].Rows[i][TCTypesVM.Prop.TCTypeID.Name].ToString(),
                            TCTypeParentID = m_dicMTCTypesDA[0].Tables[0].Rows[i][TCTypesVM.Prop.TCTypeParentID.Name].ToString()
                        });
                }
            }

            return this.Store(m_objTCTypesVM);
        }

        public ActionResult GetBusinessUnitList(StoreRequestParameters parameters)
        {
            List<BusinessUnitVM> m_objBusinessUnitVM = new List<BusinessUnitVM>();

            DataAccess.MBusinessUnitDA m_objMBusinessUnitDA = new DataAccess.MBusinessUnitDA();
            m_objMBusinessUnitDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BusinessUnitVM.Prop.BusinessUnitID.MapAlias);
            m_lstSelect.Add(BusinessUnitVM.Prop.BusinessUnitDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            //m_lstFilter.Add(Operator.Equals);
            //m_lstFilter.Add(typeof(DBudgetPlanBidOpening).Name);
            //m_objFilter.Add(StatusVM.Prop.TableName.Name, m_lstFilter);

            Dictionary<int, System.Data.DataSet> m_dicMBusinessUnitDA = m_objMBusinessUnitDA.SelectBC(0, null, false, m_lstSelect, null, null, null, null, null);

            if (m_objMBusinessUnitDA.Message == String.Empty)
            {
                for (int i = 0; i < m_dicMBusinessUnitDA[0].Tables[0].Rows.Count; i++)
                {
                    m_objBusinessUnitVM.Add(new BusinessUnitVM() { BusinessUnitDesc = m_dicMBusinessUnitDA[0].Tables[0].Rows[i][BusinessUnitVM.Prop.BusinessUnitDesc.Name].ToString(), BusinessUnitID = m_dicMBusinessUnitDA[0].Tables[0].Rows[i][BusinessUnitVM.Prop.BusinessUnitID.Name].ToString() });
                }
            }

            return this.Store(m_objBusinessUnitVM);
        }
        #endregion
    }
}