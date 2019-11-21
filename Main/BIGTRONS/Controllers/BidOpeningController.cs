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
    public class BidOpeningController : BaseController
    {
        private readonly string title = "Bid Opening";
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
            DBudgetPlanBidOpeningDA m_objDBudgetPlanBidOpeningDA = new DBudgetPlanBidOpeningDA();
            m_objDBudgetPlanBidOpeningDA.ConnectionStringName = Global.ConnStrConfigName;

            string m_strMessage = string.Empty;
            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;
            string m_strFilterVendor = string.Empty;

            FilterHeaderConditions m_fhcDBudgetPlanBidOpening = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcDBudgetPlanBidOpening.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (BudgetPlanBidOpeningVM.Prop.Vendors.Name == m_strDataIndex)
                {
                    m_strFilterVendor = m_objValue.ToString();
                }

                if (m_strDataIndex != string.Empty && m_strDataIndex!= BudgetPlanBidOpeningVM.Prop.Vendors.Name)
                {
                    

                    m_strDataIndex = BudgetPlanBidOpeningVM.Prop.Map(m_strDataIndex, false);


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
            m_lstSelect.Add(BudgetPlanBidOpeningVM.Prop.BPBidOpeningID.MapAlias);
            m_lstSelect.Add(BudgetPlanBidOpeningVM.Prop.BudgetPlanVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanBidOpeningVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanBidOpeningVM.Prop.StatusDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanBidOpeningVM.Prop.PeriodStart.MapAlias);
            m_lstSelect.Add(BudgetPlanBidOpeningVM.Prop.PeriodEnd.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.Description.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.MaxBudgetPlanVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVM.Prop.IsBidOpen.MapAlias);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.None);
            m_lstFilter.Add(string.Empty);
            m_objFilter.Add(string.Format("{0} = {1}", BudgetPlanVM.Prop.BudgetPlanVersion.Map, BudgetPlanVM.Prop.MaxBudgetPlanVersion.Map), m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add((int)BudgetPlanVersionStatus.Approved);
            m_objFilter.Add(BudgetPlanVM.Prop.StatusID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add((int)BudgetPlanVersionPeriodStatus.Open);
            m_objFilter.Add(BudgetPlanVersionPeriodVM.Prop.StatusID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanPeriod.IQ.ToString());
            m_objFilter.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanPeriodID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicDBudgetPlanBidOpeningDA = m_objDBudgetPlanBidOpeningDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpDBudgetPlanBidOpeningDA in m_dicDBudgetPlanBidOpeningDA)
            {
                m_intCount = m_kvpDBudgetPlanBidOpeningDA.Key;
                break;
            }

            List<BudgetPlanBidOpeningVM> m_lsBudgetPlanBidOpeningVM = new List<BudgetPlanBidOpeningVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;

                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(BudgetPlanBidOpeningVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicDBudgetPlanBidOpeningDA = m_objDBudgetPlanBidOpeningDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objDBudgetPlanBidOpeningDA.Message == string.Empty)
                {
                    m_lsBudgetPlanBidOpeningVM = (
                        from DataRow m_drDBudgetPlanBidOpeningDA in m_dicDBudgetPlanBidOpeningDA[0].Tables[0].Rows
                        select new BudgetPlanBidOpeningVM()
                        {
                            BPBidOpeningID = m_drDBudgetPlanBidOpeningDA[BudgetPlanBidOpeningVM.Prop.BPBidOpeningID.Name].ToString(),
                            BudgetPlanID = m_drDBudgetPlanBidOpeningDA[BudgetPlanBidOpeningVM.Prop.BudgetPlanID.Name].ToString(),
                            Description = m_drDBudgetPlanBidOpeningDA[BudgetPlanBidOpeningVM.Prop.Description.Name].ToString(),
                            BudgetPlanVersion = (int)m_drDBudgetPlanBidOpeningDA[BudgetPlanBidOpeningVM.Prop.BudgetPlanVersion.Name],
                            ListBudgetPlanVersionVendorVM = GetListBudgetPlanVersionVendorLast(m_drDBudgetPlanBidOpeningDA[BudgetPlanBidOpeningVM.Prop.BudgetPlanID.Name].ToString(), m_drDBudgetPlanBidOpeningDA[BudgetPlanBidOpeningVM.Prop.BudgetPlanVersion.Name].ToString(), ref m_strMessage),
                            StatusDesc = string.IsNullOrEmpty(m_drDBudgetPlanBidOpeningDA[BudgetPlanBidOpeningVM.Prop.StatusDesc.Name].ToString()) ? "Locked" : m_drDBudgetPlanBidOpeningDA[BudgetPlanBidOpeningVM.Prop.StatusDesc.Name].ToString(),
                            PeriodStart = string.IsNullOrEmpty(m_drDBudgetPlanBidOpeningDA[BudgetPlanBidOpeningVM.Prop.PeriodStart.Name].ToString()) ? (DateTime?)null : DateTime.Parse(m_drDBudgetPlanBidOpeningDA[BudgetPlanBidOpeningVM.Prop.PeriodStart.Name].ToString()),
                            PeriodEnd = string.IsNullOrEmpty(m_drDBudgetPlanBidOpeningDA[BudgetPlanBidOpeningVM.Prop.PeriodEnd.Name].ToString()) ? (DateTime?)null : DateTime.Parse(m_drDBudgetPlanBidOpeningDA[BudgetPlanBidOpeningVM.Prop.PeriodEnd.Name].ToString()),

                        }
                    ).ToList();
                }
            }

            if (!string.IsNullOrEmpty(m_strFilterVendor)) {
                m_lsBudgetPlanBidOpeningVM = m_lsBudgetPlanBidOpeningVM.Where(d => d.Vendors.Contains(m_strFilterVendor)).ToList();
                return this.Store(m_lsBudgetPlanBidOpeningVM, m_lsBudgetPlanBidOpeningVM.Count());
            }
            else
            return this.Store(m_lsBudgetPlanBidOpeningVM, m_intCount);
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

        public ActionResult ReadBrowse(StoreRequestParameters parameters, string SuperiorID, string SuperiorTCMemberID, string PeriodStart, string PeriodEnd)
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

            if (!string.IsNullOrEmpty(PeriodStart))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(PeriodStart);
                m_objFilter.Add(TCMembersVM.Prop.PeriodStart.Map, m_lstFilter);
            }

            if (!string.IsNullOrEmpty(PeriodEnd))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(PeriodEnd);
                m_objFilter.Add(TCMembersVM.Prop.PeriodEnd.Map, m_lstFilter);
            }

            Dictionary<int, DataSet> m_dicTTCMembersDA = m_objTTCMembersDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
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
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(TCMembersVM.Prop.TCMemberID.MapAlias);
                m_lstSelect.Add(TCMembersVM.Prop.EmployeeID.MapAlias);
                m_lstSelect.Add(TCMembersVM.Prop.EmployeeName.MapAlias);
                //m_lstSelect.Add(TCMembersVM.Prop.TCTypeDesc.MapAlias);
                //m_lstSelect.Add(TCMembersVM.Prop.SuperiorName.MapAlias);
                m_lstSelect.Add(TCMembersVM.Prop.PeriodStart.MapAlias);
                m_lstSelect.Add(TCMembersVM.Prop.PeriodEnd.MapAlias);
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
                            PeriodEnd = DateTime.Parse(m_drTTCMembersDA[TCMembersVM.Prop.PeriodEnd.Name].ToString())
                            //TCTypeDesc = m_drTTCMembersDA[TCMembersVM.Prop.TCTypeDesc.Name].ToString(),
                            //SuperiorName = m_drTTCMembersDA[TCMembersVM.Prop.SuperiorName.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lsTCMembersVM, m_intCount);
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

            BudgetPlanBidOpeningVM m_objBudgetPlanBidOpeningVM = new BudgetPlanBidOpeningVM();
            ViewDataDictionary m_vddTCMember = new ViewDataDictionary();
            m_vddTCMember.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddTCMember.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
            if (Caller == General.EnumDesc(Buttons.ButtonDetail))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                Dictionary<string, object> m_dicSelectedRow = GetFormData(m_nvcParams);
                Session[dataSessionName] = JSON.Serialize(m_dicSelectedRow);
            }

            m_objBudgetPlanBidOpeningVM.ListTCMembersVM = new List<TCMembersVM>();

            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objBudgetPlanBidOpeningVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddTCMember,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Detail(string Caller, string Selected, string BidOpeningID)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            BudgetPlanBidOpeningVM m_objBudgetPlanBidOpeningVM = new BudgetPlanBidOpeningVM();
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
            {
                if (m_dicSelectedRow.ContainsKey(BudgetPlanBidOpeningVM.Prop.BPBidOpeningID.Name)
                   && string.IsNullOrEmpty(m_dicSelectedRow[BudgetPlanBidOpeningVM.Prop.BPBidOpeningID.Name].ToString()))
                    m_dicSelectedRow[BudgetPlanBidOpeningVM.Prop.BPBidOpeningID.Name] = BidOpeningID;

                m_objBudgetPlanBidOpeningVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            }
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }


            m_objBudgetPlanBidOpeningVM.ListTCMembersVM = GetListTCBidOpeningVM(m_objBudgetPlanBidOpeningVM.BPBidOpeningID);
            if (m_objBudgetPlanBidOpeningVM.ListTCMembersVM == null)
                m_objBudgetPlanBidOpeningVM.ListTCMembersVM = new List<TCMembersVM>();

            ViewDataDictionary m_vddBidOpening = new ViewDataDictionary();
            m_vddBidOpening.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objBudgetPlanBidOpeningVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddBidOpening,
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
            BudgetPlanBidOpeningVM m_objBudgetPlanBidOpeningVM = new BudgetPlanBidOpeningVM();
            if (m_dicSelectedRow.Count > 0)
            {
                if (m_dicSelectedRow.ContainsKey(BudgetPlanBidOpeningVM.Prop.BPBidOpeningID.Name)
                    && !string.IsNullOrEmpty(m_dicSelectedRow[BudgetPlanBidOpeningVM.Prop.BPBidOpeningID.Name].ToString()))
                    m_objBudgetPlanBidOpeningVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
                else
                    m_objBudgetPlanBidOpeningVM = new BudgetPlanBidOpeningVM
                    {
                        BudgetPlanID = m_dicSelectedRow[BudgetPlanBidOpeningVM.Prop.BudgetPlanID.Name].ToString(),
                        BudgetPlanVersion = int.Parse(m_dicSelectedRow[BudgetPlanBidOpeningVM.Prop.BudgetPlanVersion.Name].ToString()),
                        Description = m_dicSelectedRow[BudgetPlanBidOpeningVM.Prop.Description.Name].ToString()
                    };
            }
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            m_objBudgetPlanBidOpeningVM.ListTCMembersVM = new List<TCMembersVM>();
            if (m_objBudgetPlanBidOpeningVM.BPBidOpeningID != null)
                m_objBudgetPlanBidOpeningVM.ListTCMembersVM = GetListTCBidOpeningVM(m_objBudgetPlanBidOpeningVM.BPBidOpeningID);
            if (!m_objBudgetPlanBidOpeningVM.ListTCMembersVM.Any())
                m_objBudgetPlanBidOpeningVM.ListTCMembersVM = new List<TCMembersVM>();

            ViewDataDictionary m_vddTCMember = new ViewDataDictionary();
            m_vddTCMember.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddTCMember.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objBudgetPlanBidOpeningVM,
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

            List<BudgetPlanBidOpeningVM> m_lstSelectedRow = new List<BudgetPlanBidOpeningVM>();
            m_lstSelectedRow = JSON.Deserialize<List<BudgetPlanBidOpeningVM>>(Selected);

            DBudgetPlanBidOpeningDA m_objDBudgetPlanBidOpeningDA = new DBudgetPlanBidOpeningDA();
            m_objDBudgetPlanBidOpeningDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (BudgetPlanBidOpeningVM m_objBudgetPlanBidOpeningVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifBudgetPlanBidOpeningVM = m_objBudgetPlanBidOpeningVM.GetType().GetProperties();
                    List<object> m_lstFilter = new List<object>();

                    foreach (PropertyInfo m_pifBudgetPlanBidOpeningVM in m_arrPifBudgetPlanBidOpeningVM)
                    {
                        string m_strFieldName = m_pifBudgetPlanBidOpeningVM.Name;
                        object m_objFieldValue = m_pifBudgetPlanBidOpeningVM.GetValue(m_objBudgetPlanBidOpeningVM);
                        if (m_objBudgetPlanBidOpeningVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                           
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(BudgetPlanBidOpeningVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objDBudgetPlanBidOpeningDA.DeleteBC(m_objFilter, false);
                    if (!m_objDBudgetPlanBidOpeningDA.Success)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDBudgetPlanBidOpeningDA.Message);

                    #region Update DBudgetPlanVersion
                    DBudgetPlanVersionDA m_objDBudgetPlanVersionDA = new DBudgetPlanVersionDA();
                    m_objDBudgetPlanVersionDA.ConnectionStringName = Global.ConnStrConfigName;

                    m_objFilter = new Dictionary<string, List<object>>();
                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_objBudgetPlanBidOpeningVM.BudgetPlanID);
                    m_objFilter.Add(BudgetPlanVersionVM.Prop.BudgetPlanID.Map, m_lstFilter);

                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_objBudgetPlanBidOpeningVM.BudgetPlanVersion);
                    m_objFilter.Add(BudgetPlanVersionVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

                    Dictionary<string, List<object>> m_dicSet = new Dictionary<string, List<object>>();
                    List<object> m_lstSet = new List<object>();
                    m_lstSet.Add(typeof(int));
                    m_lstSet.Add(0);
                    m_dicSet.Add(BudgetPlanVersionVM.Prop.IsBidOpen.Map, m_lstSet);

                    m_objDBudgetPlanVersionDA.UpdateBC(m_dicSet, m_objFilter, false);
                    if (!m_objDBudgetPlanVersionDA.Success)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDBudgetPlanVersionDA.Message);


                    #endregion

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

        public ActionResult Browse(string ControlTCMemberID, string ControlEmployeeName, string ControlGrdTCMember, string FilterTCMemberID = "", string FilterEmployeeID = "", string FilterEmployeeName = "", string FilterSuperiorID = "")
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddTCMember = new ViewDataDictionary();
            m_vddTCMember.Add("Control" + TCMembersVM.Prop.TCMemberID.Name, ControlTCMemberID);
            m_vddTCMember.Add("ControlGrdTCMember", ControlGrdTCMember);
            m_vddTCMember.Add("Control" + TCMembersVM.Prop.EmployeeName.Name, ControlEmployeeName);
            m_vddTCMember.Add(TCMembersVM.Prop.TCMemberID.Name, FilterTCMemberID);
            m_vddTCMember.Add(TCMembersVM.Prop.EmployeeID.Name, FilterEmployeeID);
            m_vddTCMember.Add(TCMembersVM.Prop.EmployeeName.Name, FilterEmployeeName);
            m_vddTCMember.Add(TCMembersVM.Prop.SuperiorID.Name, FilterSuperiorID);
            //m_vddTCMember.Add(TCMembersVM.Prop.SuperiorTCMemberID.Name, FilterSuperiorTCMemberID);

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
            DBudgetPlanBidOpeningDA m_objDBudgetPlanBidOpeningDA = new DBudgetPlanBidOpeningDA();
            m_objDBudgetPlanBidOpeningDA.ConnectionStringName = Global.ConnStrConfigName;
            DBudgetPlanTCBidOpeningDA m_objDBudgetPlanTCBidOpeningDA = new DBudgetPlanTCBidOpeningDA();
            m_objDBudgetPlanTCBidOpeningDA.ConnectionStringName = Global.ConnStrConfigName;
            DBudgetPlanVersionDA m_objDBudgetPlanVersionDA = new DBudgetPlanVersionDA();
            m_objDBudgetPlanVersionDA.ConnectionStringName = Global.ConnStrConfigName;

            string m_strMessage = string.Empty;
            string m_strBudgetPlanBidOpeningID = string.Empty;
            string m_strDelegationID = string.Empty;

            string m_strTransName = "BidOpening";
            object m_objDBConnection = null;
            m_objDBConnection = m_objDBudgetPlanTCBidOpeningDA.BeginTrans(m_strTransName);


            try
            {
                m_strBudgetPlanBidOpeningID = this.Request.Params[BudgetPlanBidOpeningVM.Prop.BPBidOpeningID.Name];
                string m_strBudgetPlanID = this.Request.Params[BudgetPlanBidOpeningVM.Prop.BudgetPlanID.Name];
                string m_strBudgetPlanVersion = this.Request.Params[BudgetPlanBidOpeningVM.Prop.BudgetPlanVersion.Name];
                string m_strPeriodStart = this.Request.Params[BudgetPlanBidOpeningVM.Prop.PeriodStart.Name];
                string m_strPeriodEnd = this.Request.Params[BudgetPlanBidOpeningVM.Prop.PeriodEnd.Name];
                string m_strPeriodStartTime = this.Request.Params[BudgetPlanBidOpeningVM.Prop.PeriodStartTime.Name];
                string m_strPeriodEndTime = this.Request.Params[BudgetPlanBidOpeningVM.Prop.PeriodEndTime.Name];
                string m_strStatusID = this.Request.Params[BudgetPlanBidOpeningVM.Prop.StatusID.Name];

                string m_strListTCMember = this.Request.Params[BudgetPlanBidOpeningVM.Prop.ListTCMembers.Name];

                List<TCMembersVM> m_lstTCMembersVM = JSON.Deserialize<List<TCMembersVM>>(m_strListTCMember);

                m_lstMessage = IsSaveValid(Action, m_strBudgetPlanBidOpeningID, m_strBudgetPlanID, m_strBudgetPlanVersion, m_strPeriodStart, m_strPeriodEnd, m_strStatusID, m_lstTCMembersVM);
                
                if (m_lstMessage.Count <= 0)
                {

                    DBudgetPlanBidOpening m_objDBudgetPlanBidOpening = new DBudgetPlanBidOpening();
                    m_objDBudgetPlanBidOpening.BPBidOpeningID = m_strBudgetPlanBidOpeningID;
                    m_objDBudgetPlanBidOpeningDA.Data = m_objDBudgetPlanBidOpening;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objDBudgetPlanBidOpeningDA.Select();

                    m_objDBudgetPlanBidOpening.BudgetPlanID = m_strBudgetPlanID;
                    m_objDBudgetPlanBidOpening.BudgetPlanVersion = int.Parse(m_strBudgetPlanVersion);
                    m_objDBudgetPlanBidOpening.PeriodStart = DateTime.Parse(m_strPeriodStart +" " + m_strPeriodStartTime);
                    m_objDBudgetPlanBidOpening.PeriodEnd = DateTime.Parse(m_strPeriodEnd + " " + m_strPeriodEndTime);
                    m_objDBudgetPlanBidOpening.StatusID = int.Parse(m_strStatusID);

                    if (string.IsNullOrEmpty(m_objDBudgetPlanBidOpeningDA.Data.BPBidOpeningID))
                    {
                        m_objDBudgetPlanBidOpening.BPBidOpeningID = Guid.NewGuid().ToString().Replace("-", "");
                        m_objDBudgetPlanBidOpeningDA.Insert(true, m_objDBConnection);
                    }
                    else
                        m_objDBudgetPlanBidOpeningDA.Update(true, m_objDBConnection);

                    if (!m_objDBudgetPlanBidOpeningDA.Success || m_objDBudgetPlanBidOpeningDA.Message != string.Empty)
                    {
                        m_objDBudgetPlanBidOpeningDA.EndTrans(ref m_objDBConnection, m_strTransName);
                        m_lstMessage.Add(m_objDBudgetPlanBidOpeningDA.Message);
                    }

                    m_strBudgetPlanBidOpeningID = m_objDBudgetPlanBidOpeningDA.Data.BPBidOpeningID;

                    #region DBudgetPlanVersion

                    DBudgetPlanVersion m_objDBudgetPlanVersion = new DBudgetPlanVersion();
                    m_objDBudgetPlanVersion.BudgetPlanID = m_objDBudgetPlanBidOpening.BudgetPlanID;
                    m_objDBudgetPlanVersion.BudgetPlanVersion = m_objDBudgetPlanBidOpening.BudgetPlanVersion;
                    m_objDBudgetPlanVersionDA.Data = m_objDBudgetPlanVersion;

                    m_objDBudgetPlanVersionDA.Select();

                    m_objDBudgetPlanVersion.IsBidOpen = false;
                    if (m_objDBudgetPlanBidOpening.StatusID == 0) //Open
                        m_objDBudgetPlanVersion.IsBidOpen = true;

                    m_objDBudgetPlanVersionDA.Update(true, m_objDBConnection);

                    if (!m_objDBudgetPlanVersionDA.Success || m_objDBudgetPlanVersionDA.Message != string.Empty)
                    {
                        m_objDBudgetPlanVersionDA.EndTrans(ref m_objDBConnection, m_strTransName);
                        m_lstMessage.Add(m_objDBudgetPlanVersionDA.Message);
                    }
                    #endregion

                    #region DBudgetPlanTCBidOpening
                    if (m_lstTCMembersVM.Any())
                    {
                        Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                        List<object> m_lstFilter = new List<object>();
                        m_lstFilter.Add(Operator.Equals);
                        m_lstFilter.Add(m_strBudgetPlanBidOpeningID);
                        m_objFilter.Add(BudgetPlanTCBidOpeningVM.Prop.BPBidOpeningID.Map, m_lstFilter);

                        m_objDBudgetPlanTCBidOpeningDA.DeleteBC(m_objFilter, true, m_objDBConnection);

                        foreach (TCMembersVM objTCMember in m_lstTCMembersVM)
                        {
                            DBudgetPlanTCBidOpening m_objDBudgetPlanTCBidOpening = new DBudgetPlanTCBidOpening();
                            m_objDBudgetPlanTCBidOpeningDA.Data = m_objDBudgetPlanTCBidOpening;

                            m_objDBudgetPlanTCBidOpening.BPTCBidOpeningID = Guid.NewGuid().ToString().Replace("-", "");
                            m_objDBudgetPlanTCBidOpening.BPBidOpeningID = m_strBudgetPlanBidOpeningID;
                            m_objDBudgetPlanTCBidOpening.TCMemberID = objTCMember.TCMemberID;

                            m_objDBudgetPlanTCBidOpeningDA.Insert(true, m_objDBConnection);

                            if (!m_objDBudgetPlanTCBidOpeningDA.Success || m_objDBudgetPlanTCBidOpeningDA.Message != string.Empty)
                            {
                                m_objDBudgetPlanTCBidOpeningDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                                m_lstMessage.Add(m_objDBudgetPlanTCBidOpeningDA.Message);
                            }

                        }

                    }
                    #endregion


                    if (!m_objDBudgetPlanBidOpeningDA.Success || m_objDBudgetPlanBidOpeningDA.Message != string.Empty)
                        m_lstMessage.Add(m_objDBudgetPlanBidOpeningDA.Message);
                    else
                        m_objDBudgetPlanBidOpeningDA.EndTrans(ref m_objDBConnection, true, m_strTransName);

                }
            }
            catch (Exception ex)
            {
                m_objDBudgetPlanBidOpeningDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                m_lstMessage.Add(ex.Message);
            }
            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
                return Detail(General.EnumDesc(Buttons.ButtonSave), null, m_strBudgetPlanBidOpeningID);
            }
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);
        }

        public ActionResult DeleteTCMemberBidOpening(string Selected)
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));


            List<BudgetPlanTCBidOpeningVM> m_lstSelectedRow = new List<BudgetPlanTCBidOpeningVM>();
            m_lstSelectedRow = JSON.Deserialize<List<BudgetPlanTCBidOpeningVM>>(Selected);

            DBudgetPlanTCBidOpeningDA m_objDBudgetPlanTCBidOpeningDA = new DBudgetPlanTCBidOpeningDA();
            m_objDBudgetPlanTCBidOpeningDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (BudgetPlanTCBidOpeningVM m_objBudgetPlanTCBidOpeningVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifBudgetPlanTCBidOpeningVM = m_objBudgetPlanTCBidOpeningVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifBudgetPlanTCBidOpeningVM in m_arrPifBudgetPlanTCBidOpeningVM)
                    {
                        string m_strFieldName = m_pifBudgetPlanTCBidOpeningVM.Name;
                        object m_objFieldValue = m_pifBudgetPlanTCBidOpeningVM.GetValue(m_objBudgetPlanTCBidOpeningVM) ?? string.Empty;
                        if (m_objBudgetPlanTCBidOpeningVM.IsKey(m_strFieldName))
                        {

                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(BudgetPlanTCBidOpeningVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objDBudgetPlanTCBidOpeningDA.DeleteBC(m_objFilter, false);
                    if (m_objDBudgetPlanTCBidOpeningDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDBudgetPlanTCBidOpeningDA.Message);
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
            m_lstFilter.Add(typeof(DBudgetPlanBidOpening).Name);
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

        public ActionResult Preview(string Caller, string Selected)
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
            BudgetPlanBidOpeningVM m_objBudgetPlanBidOpeningVM = new BudgetPlanBidOpeningVM();
            if (m_dicSelectedRow.Count > 0)
            {
                if (m_dicSelectedRow.ContainsKey(BudgetPlanBidOpeningVM.Prop.BPBidOpeningID.Name)
                    && !string.IsNullOrEmpty(m_dicSelectedRow[BudgetPlanBidOpeningVM.Prop.BPBidOpeningID.Name].ToString()))
                {
                    //m_objBudgetPlanBidOpeningVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
                    m_objBudgetPlanBidOpeningVM.BudgetPlanVM = GetBudgetPlan(m_dicSelectedRow[BudgetPlanBidOpeningVM.Prop.BudgetPlanID.Name].ToString(), m_dicSelectedRow[BudgetPlanBidOpeningVM.Prop.BudgetPlanVersion.Name].ToString(), ref m_strMessage);
                }
                else
                    m_objBudgetPlanBidOpeningVM = new BudgetPlanBidOpeningVM
                    {
                        BudgetPlanID = m_dicSelectedRow[BudgetPlanBidOpeningVM.Prop.BudgetPlanID.Name].ToString(),
                        BudgetPlanVersion = int.Parse(m_dicSelectedRow[BudgetPlanBidOpeningVM.Prop.BudgetPlanVersion.Name].ToString()),
                        Description = m_dicSelectedRow[BudgetPlanBidOpeningVM.Prop.Description.Name].ToString()
                    };
            }
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            //m_objBudgetPlanBidOpeningVM.ListTCMembersVM = new List<TCMembersVM>();
            //if (m_objBudgetPlanBidOpeningVM.BPBidOpeningID != null)
            //    m_objBudgetPlanBidOpeningVM.ListTCMembersVM = GetListTCBidOpeningVM(m_objBudgetPlanBidOpeningVM.BPBidOpeningID);
            //if (!m_objBudgetPlanBidOpeningVM.ListTCMembersVM.Any())
            //    m_objBudgetPlanBidOpeningVM.ListTCMembersVM = new List<TCMembersVM>();

            ViewDataDictionary m_vddBidOpening = new ViewDataDictionary();
            m_vddBidOpening.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddBidOpening.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            m_vddBidOpening.Add(BudgetPlanBidOpeningVM.Prop.Vendors.Name, m_dicSelectedRow[BudgetPlanBidOpeningVM.Prop.Vendors.Name].ToString());
            m_vddBidOpening.Add(BudgetPlanBidOpeningVM.Prop.BudgetPlanID.Name, m_dicSelectedRow[BudgetPlanBidOpeningVM.Prop.BudgetPlanID.Name].ToString());
            m_vddBidOpening.Add(BudgetPlanBidOpeningVM.Prop.BudgetPlanVersion.Name, m_dicSelectedRow[BudgetPlanBidOpeningVM.Prop.BudgetPlanVersion.Name].ToString());
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objBudgetPlanBidOpeningVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddBidOpening,
                ViewName = "TRMLead/_Form",//"TRMLead/_Form"
                WrapByScriptTag = false
            };
        }

        public ActionResult GetVendorsEntry(StoreRequestParameters parameters, string BudgetPlanID,string BudgetPlanVersion) {
            string m_strMessage = string.Empty;
            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;

            List<BudgetPlanVersionEntryVM> m_objListBudgetPlanVersionEntryVM = GetListBudgetPlanVersionEntry(BudgetPlanID, BudgetPlanVersion, ref m_strMessage);
            
            //data
            //object[] m_obDataSource = new object[m_objListBudgetPlanVersionEntryVM.Count];
            //int m_intRow = 0;
            ////List Panel Vendor
            //foreach (var item in m_objListBudgetPlanVersionEntryVM)
            //{
            //    int m_intColoumn = 0;
            //    object[] m_obRowSource = new object[3];
            //    m_obRowSource[m_intColoumn] = item.VendorDesc;
            //    m_intColoumn++;
            //    m_obRowSource[m_intColoumn] = item.FeePercentage;
            //    m_intColoumn++;
            //    m_obRowSource[m_intColoumn] = item.Total;
            //    m_intColoumn++;
            //    m_obDataSource[m_intRow] = m_obRowSource;
            //    m_intRow++;
            //}


            return this.Store(m_objListBudgetPlanVersionEntryVM.Skip(m_intSkip).Take(m_intLength), m_objListBudgetPlanVersionEntryVM.Count);
        }

        public ActionResult GetPanelTC(string BudgetPlanID, string BudgetPlanVersion, string Vendors)
        {
            string m_strMessage = string.Empty;
            int iLabelWidth = 175;
            int iFieldWidth = 420;
            int iBodyPadding = 10;

            //Vendor Tab Panel
            FormPanel m_frmVendorPanel = new FormPanel
            {
                ID = "FVendorEntry",
                Border = false,
                BodyPadding = iBodyPadding,
                Title = "Vendor Entry"
            };


            List<BudgetPlanVersionEntryVM> m_objListBudgetPlanVersionEntryVM = GetListBudgetPlanVersionEntry(BudgetPlanID, BudgetPlanVersion, ref m_strMessage);

            if (m_strMessage != string.Empty)
            {
                return this.Direct();
            }

            //Get Vendor List
            //List<string> VendorList = m_objListBudgetPlanVersionEntryVM.Select(d=>d.VendorDesc).ToList();

            if (m_objListBudgetPlanVersionEntryVM.Any())
            {

                

                //Vendor Panel

                TextField m_txtVendorDesc = new TextField
                {
                    //Value = item.VendorDesc,
                    ID = BudgetPlanVersionEntryVM.Prop.VendorDesc.Name,
                    FieldLabel = BudgetPlanVersionEntryVM.Prop.VendorDesc.Desc,
                    LabelWidth = iLabelWidth,
                    Width = iFieldWidth,
                    ReadOnly = true
                };

                NumberField m_txtFeePercentageField = new NumberField
                {
                    //Value = item.FeePercentage,
                    ID = BudgetPlanVersionEntryVM.Prop.FeePercentage.Name,
                    FieldLabel = BudgetPlanVersionEntryVM.Prop.FeePercentage.Desc,
                    LabelWidth = iLabelWidth,
                    Width = iFieldWidth,
                    ReadOnly = true,
                    FieldStyle = "text-align: right;"
                };

                NumberField m_txtTotalField = new NumberField
                {
                    //Value = item.Total.Value.ToString(Global.DefaultNumberFormat),
                    ID = BudgetPlanVersionEntryVM.Prop.Total.Name,
                    FieldLabel = BudgetPlanVersionEntryVM.Prop.Total.Desc,
                    LabelWidth = iLabelWidth,
                    Width = iFieldWidth,
                    ReadOnly = true,
                    FieldStyle = "text-align: right;",
                    //FieldCls ="numField"
                };
                //m_txtTotalField.Listeners.AfterRender.Handler = "formatNumber(this, false);";
                //m_txtTotalField.Listeners.Blur.Handler = "formatNumber(this, false);";


                m_frmVendorPanel.Items.Add(m_txtVendorDesc);
                m_frmVendorPanel.Items.Add(m_txtFeePercentageField);
                m_frmVendorPanel.Items.Add(m_txtTotalField);
                m_frmVendorPanel.Listeners.AfterRender.Fn = "afterRender";

            }

            return this.ComponentConfig(m_frmVendorPanel);
        }

        public ActionResult GetPanel(string BudgetPlanID, string BudgetPlanVersion, string Vendors)
        {
            string m_strMessage = string.Empty;
            //int iLabelWidth = 175;
            //int iFieldWidth = 420;
            //int iBodyPadding = 10;


            FormPanel m_frmVendorPanel = new FormPanel
            {
                Collapsible = true,
                Title = "Version Entry",
                //BodyPadding = iBodyPadding
            };

            List<BudgetPlanVersionEntryVM> m_objListBudgetPlanVersionEntryVM = GetListBudgetPlanVersionEntry(BudgetPlanID, BudgetPlanVersion, ref m_strMessage);

            if (m_strMessage != string.Empty)
            {
                return this.Direct();
            }

            //Get Vendor List
            //List<string> VendorList = m_objListBudgetPlanVersionEntryVM.Select(d=>d.VendorDesc).ToList();

            if (m_objListBudgetPlanVersionEntryVM.Any())
            {

                GridPanel m_grdPanel = GenerateGridPanel(m_objListBudgetPlanVersionEntryVM,BudgetPlanID, BudgetPlanVersion);
                m_frmVendorPanel.Items.Add(m_grdPanel);

            }

            return this.ComponentConfig(m_frmVendorPanel);
        }

        private GridPanel GenerateGridPanel(List<BudgetPlanVersionEntryVM> listBudgetPlanVersionEntryVM, string budgetPlanID, string budgetPlanVersion)
        {

            //m_gridPanel.Listeners.ViewReady.Handler = $"viewReady('{budgetPlanID}','{budgetPlanVersion.ToString()}');";

            //SelectionModel
            //RowSelectionModel m_rowSelectionModel = new RowSelectionModel() { Mode = SelectionMode.Multi, AllowDeselect = true };
            //m_treepanel.SelectionModel.Add(m_rowSelectionModel);

            //Fields
            ModelField m_objModelField = new ModelField();
            ModelFieldCollection m_objModelFieldCollection = new ModelFieldCollection();
            m_objModelField = new ModelField { Name = BudgetPlanVersionEntryVM.Prop.ItemDesc.Name.ToLower() };
            m_objModelFieldCollection.Add(m_objModelField);

            //Columns
            List<ColumnBase> m_ListColumn = new List<ColumnBase>();
            ColumnBase m_objColumn = new Column();
            m_objColumn = new Column { Text = NegotiationBidStructuresVM.Prop.ItemDesc.Desc, DataIndex = NegotiationBidStructuresVM.Prop.ItemDesc.Name.ToLower(), Width = 400, Locked = true };
            m_ListColumn.Add(m_objColumn);
            

            Dictionary<string, string> m_dicItem = new Dictionary<string, string>();
            m_dicItem.Add(BudgetPlanVersionEntryVM.Prop.FeePercentage.Name, BudgetPlanVersionEntryVM.Prop.FeePercentage.Desc);
            m_dicItem.Add(BudgetPlanVersionEntryVM.Prop.Total.Name, BudgetPlanVersionEntryVM.Prop.Total.Desc);

            object[] m_objDataSource = new object[m_dicItem.Count];

            
            for (int m_intRow = 0; m_intRow < m_dicItem.Count; m_intRow++)
            {
                int m_intColumn = 0;
                object[] m_rowVendorEntry = new object[listBudgetPlanVersionEntryVM.Count+1];
                m_rowVendorEntry[m_intColumn] = m_dicItem.ElementAt(m_intRow).Value;
                m_intColumn++;

                foreach (BudgetPlanVersionEntryVM item in listBudgetPlanVersionEntryVM)
                {
                    m_rowVendorEntry[m_intColumn] = item.GetType().GetProperty(m_dicItem.ElementAt(m_intRow).Key).GetValue(item, null);
                    m_intColumn++;
                }

                m_objDataSource[m_intRow] = m_rowVendorEntry;
            }

            foreach (BudgetPlanVersionEntryVM item in listBudgetPlanVersionEntryVM)
            {
                m_objModelField = new ModelField { Name = (item.VendorID).ToLower() };
                m_objModelFieldCollection.Add(m_objModelField);

                m_objColumn = new NumberColumn { Text = item.VendorDesc, Flex = 1, DataIndex = item.VendorID, Align = ColumnAlign.End, Lockable = false,  Format = Global.IntegerNumberFormat }; //Renderer = new Renderer("renderGridColumn")
                m_ListColumn.Add(m_objColumn);
            }

            Store m_store = this.BuildStore(m_objModelFieldCollection, m_objDataSource,"SVendorEntry");

            GridPanel m_gridPanel = new GridPanel
            {
                MinHeight = 200,
                RowLines = true,
                ColumnLines = true,
                SortableColumns = false,
                AutoLoad = false,
                Store = { m_store },
                
            };
            
            m_gridPanel.ColumnModel.Columns.AddRange(m_ListColumn);
            return m_gridPanel;
        }

        private Store BuildStore(ModelFieldCollection modelFieldCollection,object data,string ID="")
        {
            Model model = new Model();
            model.Fields.AddRange(modelFieldCollection);
            Store store = new Store
            {
                ID = ID,
                Model = { model }
            };

            store.DataSource = data;

            return store;
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

        private List<string> IsSaveValid(string Action, string BudgetPlanTCBidOpening, string BudgetPlanID, string BudgetPlanVersion,
           string PeriodStart, string PeriodEnd, string StatusID, List<TCMembersVM> ListTCMembersVM)
        {
            string m_strMessage = string.Empty;
            List<string> m_lstReturn = new List<string>();

            if (string.IsNullOrEmpty(StatusID))
                m_lstReturn.Add(BudgetPlanBidOpeningVM.Prop.StatusDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (string.IsNullOrEmpty(BudgetPlanID))
                m_lstReturn.Add(TCMembersVM.Prop.EmployeeName.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (string.IsNullOrEmpty(PeriodStart))
                m_lstReturn.Add(TCMembersVM.Prop.PeriodStart.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (string.IsNullOrEmpty(PeriodEnd))
                m_lstReturn.Add(TCMembersVM.Prop.PeriodEnd.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (!ListTCMembersVM.Any())
            {
                m_lstReturn.Add(BudgetPlanBidOpeningVM.Prop.TCMember.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            }
            if (!string.IsNullOrEmpty(PeriodStart) && !string.IsNullOrEmpty(PeriodEnd))
            {
                if (DateTime.Parse(PeriodStart) > DateTime.Parse(PeriodEnd))
                {
                    m_lstReturn.Add("Period Date Range " + General.EnumDesc(MessageLib.invalid));
                }
                else
                {
                    if (!ListTCMembersVM.Any(d => ((DateTime.Parse(PeriodStart) >= d.PeriodStart && DateTime.Parse(PeriodStart) <= d.PeriodEnd) &&
                                            DateTime.Parse(PeriodEnd) >= d.PeriodStart && DateTime.Parse(PeriodEnd) <= d.PeriodEnd)))
                    {
                        m_lstReturn.Add("Period Date Range " + General.EnumDesc(MessageLib.invalid));
                    }
                }

            }

            if (IsExistBidOpening(BudgetPlanID, BudgetPlanVersion)) {
                m_lstReturn.Add(BudgetPlanBidOpeningVM.Prop.BudgetPlanID.Desc + " " + General.EnumDesc(MessageLib.exist));
            }





            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(BudgetPlanBidOpeningVM.Prop.BPBidOpeningID.Name, parameters[BudgetPlanBidOpeningVM.Prop.BPBidOpeningID.Name]);
            m_dicReturn.Add(BudgetPlanBidOpeningVM.Prop.BudgetPlanID.Name, parameters[BudgetPlanBidOpeningVM.Prop.BudgetPlanID.Name]);
            m_dicReturn.Add(BudgetPlanBidOpeningVM.Prop.BudgetPlanVersion.Name, parameters[BudgetPlanBidOpeningVM.Prop.BudgetPlanVersion.Name]);
            m_dicReturn.Add(BudgetPlanBidOpeningVM.Prop.PeriodStart.Name, parameters[BudgetPlanBidOpeningVM.Prop.PeriodStart.Name]);
            m_dicReturn.Add(BudgetPlanBidOpeningVM.Prop.PeriodEnd.Name, parameters[BudgetPlanBidOpeningVM.Prop.PeriodEnd.Name]);

            return m_dicReturn;
        }

        private BudgetPlanBidOpeningVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            BudgetPlanBidOpeningVM m_objBudgetPlanBidOpeningVM = new BudgetPlanBidOpeningVM();
            DBudgetPlanBidOpeningDA m_objTBudgetPlanBidOpeningDA = new DBudgetPlanBidOpeningDA();
            m_objTBudgetPlanBidOpeningDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanBidOpeningVM.Prop.BPBidOpeningID.MapAlias);
            m_lstSelect.Add(BudgetPlanBidOpeningVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanBidOpeningVM.Prop.BudgetPlanVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.Description.MapAlias);
            m_lstSelect.Add(BudgetPlanBidOpeningVM.Prop.PeriodStart.MapAlias);
            m_lstSelect.Add(BudgetPlanBidOpeningVM.Prop.PeriodEnd.MapAlias);
            m_lstSelect.Add(BudgetPlanBidOpeningVM.Prop.StatusDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanBidOpeningVM.Prop.StatusID.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objBudgetPlanBidOpeningVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(BudgetPlanBidOpeningVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicTBudgetPlanBidOpeningDA = m_objTBudgetPlanBidOpeningDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objTBudgetPlanBidOpeningDA.Message == string.Empty)
            {
                DataRow m_drTBudgetPlanBidOpeningDA = m_dicTBudgetPlanBidOpeningDA[0].Tables[0].Rows[0];
                m_objBudgetPlanBidOpeningVM.BPBidOpeningID = m_drTBudgetPlanBidOpeningDA[BudgetPlanBidOpeningVM.Prop.BPBidOpeningID.Name].ToString();
                m_objBudgetPlanBidOpeningVM.BudgetPlanID = m_drTBudgetPlanBidOpeningDA[BudgetPlanBidOpeningVM.Prop.BudgetPlanID.Name].ToString();
                m_objBudgetPlanBidOpeningVM.BudgetPlanVersion = (int)m_drTBudgetPlanBidOpeningDA[BudgetPlanBidOpeningVM.Prop.BudgetPlanVersion.Name];
                m_objBudgetPlanBidOpeningVM.Description = m_drTBudgetPlanBidOpeningDA[BudgetPlanBidOpeningVM.Prop.Description.Name].ToString();
                m_objBudgetPlanBidOpeningVM.StatusDesc = m_drTBudgetPlanBidOpeningDA[BudgetPlanBidOpeningVM.Prop.StatusDesc.Name].ToString();
                m_objBudgetPlanBidOpeningVM.StatusID = (int)m_drTBudgetPlanBidOpeningDA[BudgetPlanBidOpeningVM.Prop.StatusID.Name];
                m_objBudgetPlanBidOpeningVM.PeriodStart = DateTime.Parse(m_drTBudgetPlanBidOpeningDA[BudgetPlanBidOpeningVM.Prop.PeriodStart.Name].ToString());
                m_objBudgetPlanBidOpeningVM.PeriodEnd = DateTime.Parse(m_drTBudgetPlanBidOpeningDA[BudgetPlanBidOpeningVM.Prop.PeriodEnd.Name].ToString());
                m_objBudgetPlanBidOpeningVM.PeriodStartTime = TimeSpan.Parse(m_objBudgetPlanBidOpeningVM.PeriodStart.Value.ToString(Global.ShortTimeFormat));//  TimeSpan.Parse(m_drTBudgetPlanBidOpeningDA[BudgetPlanBidOpeningVM.Prop.PeriodStart.Name].ToString());
                m_objBudgetPlanBidOpeningVM.PeriodEndTime = TimeSpan.Parse(m_objBudgetPlanBidOpeningVM.PeriodEnd.Value.ToString(Global.ShortTimeFormat));
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objTBudgetPlanBidOpeningDA.Message;

            return m_objBudgetPlanBidOpeningVM;
        }

        private bool IsExistBidOpening(string budgetPlanID, string budgetPlanVersion)
        {
            BudgetPlanBidOpeningVM m_objBudgetPlanBidOpeningVM = new BudgetPlanBidOpeningVM();
            DBudgetPlanBidOpeningDA m_objTBudgetPlanBidOpeningDA = new DBudgetPlanBidOpeningDA();
            m_objTBudgetPlanBidOpeningDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanBidOpeningVM.Prop.BPBidOpeningID.MapAlias);
            m_lstSelect.Add(BudgetPlanBidOpeningVM.Prop.BPBidOpeningID.MapAlias);
            m_lstSelect.Add(BudgetPlanBidOpeningVM.Prop.BudgetPlanVersion.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(budgetPlanID);
            m_objFilter.Add(BudgetPlanBidOpeningVM.Prop.BudgetPlanID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(budgetPlanVersion);
            m_objFilter.Add(BudgetPlanBidOpeningVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.None);
            m_lstFilter.Add("");
            m_objFilter.Add(BudgetPlanBidOpeningVM.Prop.BPBidOpeningID.Map + " IS NOT NULL", m_lstFilter);

            Dictionary<int, DataSet> m_dicDBudgetPlanBidOpeningDA = m_objTBudgetPlanBidOpeningDA.SelectBC(0, null, true, null, m_objFilter, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpDBudgetPlanBidOpeningDA in m_dicDBudgetPlanBidOpeningDA)
            {
                m_intCount = m_kvpDBudgetPlanBidOpeningDA.Key;
                break;
            }
            return (m_intCount > 0 ? true:false);
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

        private List<TCMembersVM> GetListTCBidOpeningVM(string BidOpeningID)
        {
            List<TCMembersVM> m_listTCMemberBidOpeningVM = new List<TCMembersVM>();
            DBudgetPlanTCBidOpeningDA m_objDBudgetPlanTCBidOpeningDA = new DBudgetPlanTCBidOpeningDA();
            m_objDBudgetPlanTCBidOpeningDA.ConnectionStringName = Global.ConnStrConfigName;

            FilterHeaderConditions m_fhcTTCMembers = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BidOpeningID);
            m_objFilter.Add(BudgetPlanTCBidOpeningVM.Prop.BPBidOpeningID.Map, m_lstFilter);

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanTCBidOpeningVM.Prop.BPTCBidOpeningID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.TCMemberID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.EmployeeID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.EmployeeName.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.PeriodStart.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.PeriodEnd.MapAlias);


            Dictionary<int, DataSet> m_dicDBudgetPlanTCBidOpeningDA = m_objDBudgetPlanTCBidOpeningDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);
            if (m_objDBudgetPlanTCBidOpeningDA.Message == string.Empty)
            {
                m_listTCMemberBidOpeningVM = (
                    from DataRow m_drTTCMembersDA in m_dicDBudgetPlanTCBidOpeningDA[0].Tables[0].Rows
                    select new TCMembersVM()
                    {
                        BPTCBidOpeningID = m_drTTCMembersDA[BudgetPlanTCBidOpeningVM.Prop.BPTCBidOpeningID.Name].ToString(),
                        TCMemberID = m_drTTCMembersDA[TCMembersVM.Prop.TCMemberID.Name].ToString(),
                        EmployeeID = m_drTTCMembersDA[TCMembersVM.Prop.EmployeeID.Name].ToString(),
                        EmployeeName = m_drTTCMembersDA[TCMembersVM.Prop.EmployeeName.Name].ToString(),
                        PeriodStart = DateTime.Parse(m_drTTCMembersDA[TCMembersVM.Prop.PeriodStart.Name].ToString()),
                        PeriodEnd = DateTime.Parse(m_drTTCMembersDA[TCMembersVM.Prop.PeriodEnd.Name].ToString())
                    }
                ).ToList();
            }

            return m_listTCMemberBidOpeningVM;
        }

        private List<BudgetPlanVersionVendorVM> GetListBudgetPlanVersionVendorLast(string budgetPlanID, string budgetPlanVersion, ref string message)
        {
            List<BudgetPlanVersionVendorVM> m_lstBudgetPlanVersionVendorVM = new List<BudgetPlanVersionVendorVM>();
            DBudgetPlanVersionVendorDA m_objDBudgetPlanVersionVendorDA = new DBudgetPlanVersionVendorDA();
            m_objDBudgetPlanVersionVendorDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionPeriodID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionVendorID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.FirstName.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.LastName.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.StartDate.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.EndDate.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionPeriodVM.Prop.PeriodVersion.MapAlias);
            //m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.AvailableVendorID.MapAlias);
            m_lstSelect.Add(string.Format("(select max(a.PeriodVersion) from DBudgetPlanVersionPeriod a where a.BudgetPlanID = DBudgetPlanVersion.BudgetPlanID) as {0}", BudgetPlanVersionVendorVM.Prop.MaxPeriodVersion.Name));

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(budgetPlanID);
            m_objFilter.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(budgetPlanVersion);
            m_objFilter.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanVersion.Map, m_lstFilter);



            Dictionary<int, DataSet> m_dicTBudgetPlanDA = m_objDBudgetPlanVersionVendorDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDBudgetPlanVersionVendorDA.Success)
            {
                foreach (DataRow m_drTBudgetPlanDA in m_dicTBudgetPlanDA[0].Tables[0].Rows)
                {
                    BudgetPlanVersionVendorVM m_objBudgetPlanVM = new BudgetPlanVersionVendorVM();
                    m_objBudgetPlanVM.BudgetPlanVersionPeriodID = m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionPeriodID.Name].ToString();
                    m_objBudgetPlanVM.BudgetPlanVersionVendorID = m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionVendorID.Name].ToString();
                    m_objBudgetPlanVM.VendorID = m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.VendorID.Name].ToString();
                    m_objBudgetPlanVM.FirstName = m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.FirstName.Name].ToString();
                    m_objBudgetPlanVM.LastName = m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.LastName.Name].ToString();
                    m_objBudgetPlanVM.StartDate = DateTime.Parse(m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.StartDate.Name].ToString());
                    m_objBudgetPlanVM.EndDate = DateTime.Parse(m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.EndDate.Name].ToString());
                    m_objBudgetPlanVM.PeriodVersion = int.Parse(m_drTBudgetPlanDA[BudgetPlanVersionPeriodVM.Prop.PeriodVersion.Name].ToString());
                    m_objBudgetPlanVM.MaxPeriodVersion = int.Parse(m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.MaxPeriodVersion.Name].ToString());
                    //m_objBudgetPlanVM.AllowDelete = !string.IsNullOrEmpty(m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.AvailableVendorID.Name].ToString()) ? false : true;
                    m_objBudgetPlanVM.StartDateHours = DateTime.Parse(m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.StartDate.Name].ToString()).ToString(Global.ShortTimeFormat);
                    m_objBudgetPlanVM.EndDateHours = DateTime.Parse(m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.EndDate.Name].ToString()).ToString(Global.ShortTimeFormat);
                    m_lstBudgetPlanVersionVendorVM.Add(m_objBudgetPlanVM);
                }
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDBudgetPlanVersionVendorDA.Message;

            //List<BudgetPlanVM> listBudgetPlan = (m_lstBudgetPlanVM.Any() ? m_lstBudgetPlanVM.Where(d => d.MaxBudgetPlanVersion == d.BudgetPlanVersion).ToList() : new List<BudgetPlanVM>());
            return m_lstBudgetPlanVersionVendorVM.Where(x => x.MaxPeriodVersion == x.PeriodVersion).ToList();

        }

        private List<BudgetPlanVersionEntryVM> GetListBudgetPlanVersionEntry(string budgetPlanID, string budgetPlanVersion, ref string message)
        {
            List<BudgetPlanVersionEntryVM> m_objListBudgetPlanVersionEntryVM = new List<BudgetPlanVersionEntryVM>();
            BudgetPlanVersionEntryVM m_objBudgetPlanVersionEntryVM = new BudgetPlanVersionEntryVM();
            TBudgetPlanDA m_objTBudgetPlanDA = new TBudgetPlanDA();
            m_objTBudgetPlanDA.ConnectionStringName = Global.ConnStrConfigName;

            DBudgetPlanVersionEntryDA m_objDBudgetPlanVersionEntryDA = new DBudgetPlanVersionEntryDA();
            m_objDBudgetPlanVersionEntryDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVersionVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVM.Prop.Description.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.VendorDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.Total.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.FeePercentage.MapAlias);


            List<string> m_lstGroupBy = new List<string>();
            m_lstGroupBy.Add(BudgetPlanVersionVM.Prop.BudgetPlanID.Map);
            m_lstGroupBy.Add(BudgetPlanVersionVM.Prop.Description.Map);
            m_lstGroupBy.Add(BudgetPlanVersionEntryVM.Prop.FeePercentage.Map);
            m_lstGroupBy.Add(BudgetPlanVersionEntryVM.Prop.VendorID.Map);
            m_lstGroupBy.Add(BudgetPlanVersionVendorVM.Prop.FirstName.Map);
            m_lstGroupBy.Add(BudgetPlanVersionVendorVM.Prop.LastName.Map);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(budgetPlanID);
            m_objFilter.Add(BudgetPlanVersionVM.Prop.BudgetPlanID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(budgetPlanVersion);
            m_objFilter.Add(BudgetPlanVersionVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(BudgetPlanVersionEntryVM.Prop.Total.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicDBudgetPlanVersionEntryDA = m_objDBudgetPlanVersionEntryDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, m_lstGroupBy, m_dicOrder);
            if (m_objDBudgetPlanVersionEntryDA.Message == string.Empty)
            {
                //DataRow m_drTBudgetPlanDA = m_dicDBudgetPlanVersionEntryDA[0].Tables[0].Rows[0];
                foreach (DataRow m_drTBudgetPlanDA in m_dicDBudgetPlanVersionEntryDA[0].Tables[0].Rows)
                {
                    m_objBudgetPlanVersionEntryVM = new BudgetPlanVersionEntryVM();
                    m_objBudgetPlanVersionEntryVM.BudgetPlanID = m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanID.Name].ToString(); ;
                    m_objBudgetPlanVersionEntryVM.Description = m_drTBudgetPlanDA[BudgetPlanVM.Prop.Description.Name].ToString();
                    m_objBudgetPlanVersionEntryVM.FeePercentage = decimal.Parse(m_drTBudgetPlanDA[BudgetPlanVersionEntryVM.Prop.FeePercentage.Name].ToString());
                    m_objBudgetPlanVersionEntryVM.VendorID = m_drTBudgetPlanDA[BudgetPlanVersionEntryVM.Prop.VendorID.Name].ToString();
                    m_objBudgetPlanVersionEntryVM.VendorDesc = m_drTBudgetPlanDA[BudgetPlanVersionEntryVM.Prop.VendorDesc.Name].ToString();
                    m_objBudgetPlanVersionEntryVM.Total = decimal.Parse(m_drTBudgetPlanDA[BudgetPlanVersionEntryVM.Prop.Total.Name].ToString());

                    m_objListBudgetPlanVersionEntryVM.Add(m_objBudgetPlanVersionEntryVM);
                }
            }

            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objTBudgetPlanDA.Message;

            return m_objListBudgetPlanVersionEntryVM;
        }

        private BudgetPlanVM GetBudgetPlan(string budgetPlanID, string budgetPlanVersion, ref string message)
        {
            BudgetPlanVM m_objBudgetPlanVM = new BudgetPlanVM();
            TBudgetPlanDA m_objTBudgetPlanDA = new TBudgetPlanDA();
            m_objTBudgetPlanDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.Description.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanTemplateDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanTemplateID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanTypeDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.CompanyDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.RegionID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.RegionDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.LocationDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.DivisionDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.ProjectID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.ProjectDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.ClusterDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.ClusterID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.UnitTypeID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.UnitTypeDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.Area.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.Unit.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVM.Prop.FeePercentage.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.CreatedDate.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.ModifiedDate.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.StatusID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.StatusDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(budgetPlanID);
            m_objFilter.Add(BudgetPlanVersionVM.Prop.BudgetPlanID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(budgetPlanVersion);
            m_objFilter.Add(BudgetPlanVersionVM.Prop.BudgetPlanVersion.Map, m_lstFilter);


            Dictionary<int, DataSet> m_dicTBudgetPlanDA = m_objTBudgetPlanDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objTBudgetPlanDA.Message == string.Empty)
            {
                DataRow m_drTBudgetPlanDA = m_dicTBudgetPlanDA[0].Tables[0].Rows[0];
                m_objBudgetPlanVM.BudgetPlanID = m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanID.Name].ToString();
                m_objBudgetPlanVM.Description = m_drTBudgetPlanDA[BudgetPlanVM.Prop.Description.Name].ToString();
                m_objBudgetPlanVM.BudgetPlanTemplateID = m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanTemplateID.Name].ToString();
                m_objBudgetPlanVM.BudgetPlanTemplateDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanTemplateDesc.Name].ToString();
                m_objBudgetPlanVM.BudgetPlanTypeDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanTypeDesc.Name].ToString();
                m_objBudgetPlanVM.CompanyDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.CompanyDesc.Name].ToString();
                m_objBudgetPlanVM.RegionID = m_drTBudgetPlanDA[BudgetPlanVM.Prop.RegionID.Name].ToString();
                m_objBudgetPlanVM.RegionDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.RegionDesc.Name].ToString();
                m_objBudgetPlanVM.LocationDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.LocationDesc.Name].ToString();
                m_objBudgetPlanVM.DivisionDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.DivisionDesc.Name].ToString();
                m_objBudgetPlanVM.ProjectID = m_drTBudgetPlanDA[BudgetPlanVM.Prop.ProjectID.Name].ToString();
                m_objBudgetPlanVM.ProjectDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.ProjectDesc.Name].ToString();
                m_objBudgetPlanVM.ClusterID = m_drTBudgetPlanDA[BudgetPlanVM.Prop.ClusterID.Name].ToString();
                m_objBudgetPlanVM.ClusterDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.ClusterDesc.Name].ToString();
                m_objBudgetPlanVM.UnitTypeID = m_drTBudgetPlanDA[BudgetPlanVM.Prop.UnitTypeID.Name].ToString();
                m_objBudgetPlanVM.UnitTypeDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.UnitTypeDesc.Name].ToString();
                m_objBudgetPlanVM.Area = decimal.Parse(m_drTBudgetPlanDA[BudgetPlanVM.Prop.Area.Name].ToString());
                m_objBudgetPlanVM.Unit = decimal.Parse(m_drTBudgetPlanDA[BudgetPlanVM.Prop.Unit.Name].ToString());
                m_objBudgetPlanVM.FeePercentage = decimal.Parse(m_drTBudgetPlanDA[BudgetPlanVM.Prop.FeePercentage.Name].ToString());
                m_objBudgetPlanVM.BudgetPlanVersion = Convert.ToInt32(m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanVersion.Name].ToString());
                m_objBudgetPlanVM.CreatedDate = DateTime.Parse(m_drTBudgetPlanDA[BudgetPlanVM.Prop.CreatedDate.Name].ToString());
                m_objBudgetPlanVM.ModifiedDate = DateTime.Parse(m_drTBudgetPlanDA[BudgetPlanVM.Prop.ModifiedDate.Name].ToString());
                m_objBudgetPlanVM.StatusDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.StatusDesc.Name].ToString();
                m_objBudgetPlanVM.StatusID = (int)m_drTBudgetPlanDA[BudgetPlanVM.Prop.StatusID.Name];
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objTBudgetPlanDA.Message;

            return m_objBudgetPlanVM;
        }

        //private Dictionary<string, bool> DisplayButton()
        //{
        //    Dictionary<string, bool> m_dicItemTypeForDisplayPrice = new Dictionary<string, bool>();

        //    List<ConfigVM> m_lstConfig = GetItemTypeDisplayPriceConfig();
        //    bool m_boolValueObject = Convert.ToBoolean(GetMenuObject("DisplayButton"));

        //    foreach (var item in m_lstConfig)
        //    {
        //        m_dicItemTypeForDisplayPrice.Add(item.Key2, m_boolValueObject);
        //    }
        //    return m_dicItemTypeForDisplayPrice;
        //}

        private List<ConfigVM> GetRoleDisplayButtonConfig()
        {

            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();


            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ConfigVM.Prop.Key2.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc1.MapAlias);

            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("BidOpening");
            m_objFilter.Add(ConfigVM.Prop.Key1.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("DisplayButton");
            m_objFilter.Add(ConfigVM.Prop.Key3.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicUConfigDA = m_objUConfigDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            List<ConfigVM> m_lstConfigVM = new List<ConfigVM>();
            if (m_objUConfigDA.Message == string.Empty)
            {
                m_lstConfigVM = (
                        from DataRow m_drUConfigDA in m_dicUConfigDA[0].Tables[0].Rows
                        select new ConfigVM()
                        {
                            Key2 = m_drUConfigDA[ConfigVM.Prop.Key2.Name].ToString(),
                            Desc1 = m_drUConfigDA[ConfigVM.Prop.Desc1.Name].ToString()
                        }
                    ).ToList();
            }
            return m_lstConfigVM;

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

        #endregion
    }
}