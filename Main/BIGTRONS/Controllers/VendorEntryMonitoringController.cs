﻿using com.SML.Lib.Common;
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
using com.SML.BIGTRONS.Enum;

namespace com.SML.BIGTRONS.Controllers
{
    public class VendorEntryMonitoringController : BaseController
    {
        private readonly string title = "Vendor Entry Monitoring";
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
            TBudgetPlanDA m_objTBudgetPlanDA = new TBudgetPlanDA();
            m_objTBudgetPlanDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcTBudgetPlan = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcTBudgetPlan.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = BudgetPlanVM.Prop.Map(m_strDataIndex, false);
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
            /*Dictionary<int, DataSet> m_dicTBudgetPlanDA = m_objTBudgetPlanDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpItemBL in m_dicTBudgetPlanDA)
            {
                m_intCount = m_kvpItemBL.Key;
                break;
            }*/

            List<BudgetPlanVM> m_lstBudgetPlanVM = new List<BudgetPlanVM>();
            m_boolIsCount = false;
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanTemplateDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanVersion.MapAlias);
            m_lstSelect.Add(string.Format("(select max(a.BudgetPlanVersion) from DBudgetPlanVersion a where a.BudgetPlanID = DBudgetPlanVersion.BudgetPlanID) as {0}", BudgetPlanVM.Prop.MaxBudgetPlanVersion.Name));
            //m_lstSelect.Add(string.Format("MAX({0}) OVER(PARTITION BY {1}) AS {2}", BudgetPlanVersionVM.Prop.BudgetPlanVersion.Map,BudgetPlanVM.Prop.BudgetPlanID.Map, BudgetPlanVM.Prop.MaxBudgetPlanVersion.Name));
            m_lstSelect.Add(BudgetPlanVM.Prop.Description.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.ProjectDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.ClusterDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.UnitTypeDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.StatusDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.StatusID.MapAlias);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            foreach (DataSorter m_dtsOrder in parameters.Sort)
                m_dicOrder.Add(BudgetPlanVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));


            Dictionary<int, DataSet> m_dicTBudgetPlanDA = m_objTBudgetPlanDA.SelectBC(0, null, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
            if (m_objTBudgetPlanDA.Message == string.Empty)
            {
                m_lstBudgetPlanVM = (
                    from DataRow m_drTBudgetPlanDA in m_dicTBudgetPlanDA[0].Tables[0].Rows
                    select new BudgetPlanVM()
                    {
                        BudgetPlanID = m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanID.Name].ToString(),
                        BudgetPlanTemplateDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanTemplateDesc.Name].ToString(),
                        BudgetPlanVersion = int.Parse(m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanVersion.Name].ToString()),
                        Description = m_drTBudgetPlanDA[BudgetPlanVM.Prop.Description.Name].ToString(),
                        ProjectDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.ProjectDesc.Name].ToString(),
                        ClusterDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.ClusterDesc.Name].ToString(),
                        UnitTypeDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.UnitTypeDesc.Name].ToString(),
                        StatusDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.StatusDesc.Name].ToString(),
                        StatusID = int.Parse(m_drTBudgetPlanDA[BudgetPlanVM.Prop.StatusID.Name].ToString()),
                        MaxBudgetPlanVersion = int.Parse(m_drTBudgetPlanDA[BudgetPlanVM.Prop.MaxBudgetPlanVersion.Name].ToString())
                    }
                ).Distinct().ToList();
            }


            List<BudgetPlanVM> listBudgetPlan = (m_lstBudgetPlanVM.Any() ?
                m_lstBudgetPlanVM.Where(d => d.MaxBudgetPlanVersion == d.BudgetPlanVersion).ToList() : new List<BudgetPlanVM>());
            string messages = "";
            List<BudgetPlanPeriodVM> m_lstBudgetPlanPeriodVM = new List<BudgetPlanPeriodVM>();

            m_lstBudgetPlanPeriodVM = (
                        from BudgetPlanVM pr in listBudgetPlan
                        select new BudgetPlanPeriodVM()
                        {
                            BudgetPlanID = pr.BudgetPlanID,
                            Description = pr.Description,
                            BudgetPlanTemplateDesc = pr.BudgetPlanTemplateDesc,
                            BudgetPlanTypeID = pr.BudgetPlanTypeID,
                            ProjectDesc = pr.ProjectDesc,
                            ClusterDesc = pr.ClusterDesc,
                            UnitTypeDesc = pr.UnitTypeDesc,
                            BudgetPlanVersion = pr.BudgetPlanVersion.ToString(),
                            StatusDesc = pr.StatusDesc,
                            StatusID = pr.StatusID.ToString(),
                            ListBudgetPlanVersionVendorVM = GetListBudgetPlanVersionVendorLast(pr.BudgetPlanID, pr.BudgetPlanVersion.ToString(), ref messages)
                        }
                    ).ToList();

            return this.Store(m_lstBudgetPlanPeriodVM, m_lstBudgetPlanPeriodVM.Count());

        }
        public ActionResult BrowseVendor(string ControlVendorID, string ControlVendorDesc, string FilterVendorID = "", string FilterVendorDesc = "", string FilterBudgetPlanID = "", string FilterBudgetPlanVersion = "")
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddVendor = new ViewDataDictionary();
            m_vddVendor.Add("Control" + BudgetPlanVersionVendorVM.Prop.VendorID.Name, ControlVendorID);
            m_vddVendor.Add("Control" + BudgetPlanVersionVendorVM.Prop.VendorDesc.Name, ControlVendorDesc);
            m_vddVendor.Add(BudgetPlanVersionVendorVM.Prop.VendorID.Name, FilterVendorID);
            m_vddVendor.Add(BudgetPlanVersionVendorVM.Prop.VendorDesc.Name, FilterVendorDesc);

            m_vddVendor.Add("Filter" + BudgetPlanVM.Prop.BudgetPlanID.Name, FilterBudgetPlanID);
            m_vddVendor.Add("Filter" + BudgetPlanVM.Prop.BudgetPlanVersion.Name, FilterBudgetPlanVersion);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddVendor,
                ViewName = "../BudgetPlanPeriod/Vendor/_Browse"
            };
        }
        public ActionResult ReadBrowseVendor(StoreRequestParameters parameters, string FilterBudgetPlanID = "", string FilterBudgetPlanVersion = "")
        {
            DBudgetPlanVersionVendorDA m_objDBudgetPlanVersionVendorDA = new DBudgetPlanVersionVendorDA();
            m_objDBudgetPlanVersionVendorDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMVendor = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMVendor.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = BudgetPlanVersionVendorVM.Prop.Map(m_strDataIndex, false);

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
            if (FilterBudgetPlanID.Length > 0)
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(FilterBudgetPlanID);
                m_objFilter.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanID.Map, m_lstFilter);
            }

            if (FilterBudgetPlanVersion.Length > 0)
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(FilterBudgetPlanVersion);
                m_objFilter.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanVersion.Map, m_lstFilter);
            }

            //status
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(Enum.BudgetPlanPeriod.FQ.ToString());
            m_objFilter.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanPeriodID.Map, m_lstFilter);

            //date
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.None);
            m_objFilter.Add("'" + DateTime.Now.ToString(Global.SqlDateFormat) + "' BETWEEN " + BudgetPlanVersionVendorVM.Prop.StartDate.Map
                + " AND " + BudgetPlanVersionVendorVM.Prop.EndDate.Map, m_lstFilter);


            Dictionary<int, DataSet> m_dicDBudgetPlanVersionVendorDA = m_objDBudgetPlanVersionVendorDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpVendorBL in m_dicDBudgetPlanVersionVendorDA)
            {
                m_intCount = m_kvpVendorBL.Key;
                break;
            }

            List<BudgetPlanVersionVendorVM> m_lstBudgetPlanVersionVendorVM = new List<BudgetPlanVersionVendorVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.VendorID.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.FirstName.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.LastName.MapAlias);
                //m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.VendorDesc.MapAlias);
                //m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.VendorCategoryDesc.MapAlias);
                //m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.VendorSubcategoryDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(BudgetPlanVersionVendorVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicDBudgetPlanVersionVendorDA = m_objDBudgetPlanVersionVendorDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objDBudgetPlanVersionVendorDA.Message == string.Empty)
                {
                    m_lstBudgetPlanVersionVendorVM = (
                        from DataRow m_drDBudgetPlanVersionVendorDA in m_dicDBudgetPlanVersionVendorDA[0].Tables[0].Rows
                        select new BudgetPlanVersionVendorVM()
                        {
                            VendorID = m_drDBudgetPlanVersionVendorDA[BudgetPlanVersionVendorVM.Prop.VendorID.Name].ToString(),
                            FirstName = m_drDBudgetPlanVersionVendorDA[BudgetPlanVersionVendorVM.Prop.FirstName.Name].ToString(),
                            LastName = m_drDBudgetPlanVersionVendorDA[BudgetPlanVersionVendorVM.Prop.LastName.Name].ToString()
                            //VendorCategoryDesc = m_drDBudgetPlanVersionVendorDA[VendorVM.Prop.VendorCategoryDesc.Name].ToString(),
                            //VendorSubcategoryDesc = m_drDBudgetPlanVersionVendorDA[VendorVM.Prop.VendorSubcategoryDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstBudgetPlanVersionVendorVM, m_intCount);
        }
        public ActionResult Home()
        {
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 0;
            return this.Direct();
        }
        public ActionResult Add(string Caller, string BudgetPlanID, string BudgetPlanVersion)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Add)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            DBudgetPlanVersionPeriodDA m_objDBudgetPlanVersionPeriodDA = new DBudgetPlanVersionPeriodDA();
            DBudgetPlanVersionPeriod m_objDBudgetPlanVersionPeriod = new DBudgetPlanVersionPeriod();
            m_objDBudgetPlanVersionPeriodDA.ConnectionStringName = Global.ConnStrConfigName;

            string m_strMessage = string.Empty;
            List<string> m_lstMessage = new List<string>();
            BudgetPlanVersionPeriodVM m_objBudgetPlanVersionPeriodVM = new BudgetPlanVersionPeriodVM();

            try
            {
                List<BudgetPlanVersionPeriodVM> m_lstBudgetPlanVersionPeriodVM = GetListBudgetPlanVersionPeriod(BudgetPlanID, BudgetPlanVersion, ref m_strMessage);

                m_objBudgetPlanVersionPeriodVM = m_lstBudgetPlanVersionPeriodVM.Where(p => p.StatusID == 0).FirstOrDefault();

                if (m_lstBudgetPlanVersionPeriodVM.Where(p => p.StatusID == 0 && p.BudgetPlanPeriodID == General.EnumName(BudgetPlanPeriod.FQ)).Any())
                {
                    Global.ShowErrorAlert("Budget Plan Version Period", "This process has been finished!");
                    return this.Direct();
                }

                if (m_lstBudgetPlanVersionPeriodVM.Where(p => p.StatusID == 0).Any())
                {
                    //Global.ShowErrorAlert("Budget Plan Version Period", "Previous process has not finished yet!");
                    //return this.Direct();
                    m_objDBudgetPlanVersionPeriod.BudgetPlanVersionPeriodID = m_objBudgetPlanVersionPeriodVM.BudgetPlanVersionPeriodID;
                    m_objDBudgetPlanVersionPeriodDA.Data = m_objDBudgetPlanVersionPeriod;
                    m_objDBudgetPlanVersionPeriodDA.Select();
                    if (m_objDBudgetPlanVersionPeriodDA.Message == string.Empty)
                    {

                        m_objDBudgetPlanVersionPeriod.StatusID = int.Parse(General.EnumDesc(BudgetPlanVersionPeriodStatus.Close));
                        m_objDBudgetPlanVersionPeriodDA.Update(false);

                        if (!m_objDBudgetPlanVersionPeriodDA.Success || m_objDBudgetPlanVersionPeriodDA.Message != string.Empty)
                        {
                            return this.Direct(false, m_objDBudgetPlanVersionPeriodDA.Message);
                        }
                        else
                        {
                            //insert
                            m_objDBudgetPlanVersionPeriodDA.Data = m_objDBudgetPlanVersionPeriod;
                            m_objDBudgetPlanVersionPeriod.BudgetPlanVersionPeriodID = Guid.NewGuid().ToString().Replace("-", "");
                            m_objDBudgetPlanVersionPeriod.BudgetPlanID = BudgetPlanID;
                            m_objDBudgetPlanVersionPeriod.BudgetPlanVersion = int.Parse(BudgetPlanVersion);
                            m_objDBudgetPlanVersionPeriod.StatusID = int.Parse(General.EnumDesc(BudgetPlanVersionPeriodStatus.Open));
                            if (m_lstBudgetPlanVersionPeriodVM.LastOrDefault() != null)
                                m_objDBudgetPlanVersionPeriod.PeriodVersion = m_lstBudgetPlanVersionPeriodVM.LastOrDefault().PeriodVersion + 1;
                            else
                                m_objDBudgetPlanVersionPeriod.PeriodVersion = 1;

                            if (m_objDBudgetPlanVersionPeriod.PeriodVersion == 1)
                                m_objDBudgetPlanVersionPeriod.BudgetPlanPeriodID = General.EnumName(BudgetPlanPeriod.IQ);
                            else
                                m_objDBudgetPlanVersionPeriod.BudgetPlanPeriodID = General.EnumName(BudgetPlanPeriod.FQ);

                            m_objDBudgetPlanVersionPeriodDA.Insert(false);
                            if (m_objDBudgetPlanVersionPeriodDA.Message == string.Empty)
                            {
                                m_objBudgetPlanVersionPeriodVM = new BudgetPlanVersionPeriodVM()
                                {
                                    BudgetPlanID = m_objDBudgetPlanVersionPeriod.BudgetPlanID,
                                    BudgetPlanVersionPeriodID = m_objDBudgetPlanVersionPeriod.BudgetPlanVersionPeriodID,
                                    PeriodVersion = m_objDBudgetPlanVersionPeriod.PeriodVersion,
                                    BudgetPlanPeriodDesc = (General.EnumName(BudgetPlanPeriod.IQ) == m_objDBudgetPlanVersionPeriod.BudgetPlanPeriodID ? General.EnumDesc(BudgetPlanPeriod.IQ) : General.EnumDesc(BudgetPlanPeriod.FQ)),
                                    BudgetPlanPeriodID = m_objDBudgetPlanVersionPeriod.BudgetPlanPeriodID,
                                    BudgetPlanVersion = m_objDBudgetPlanVersionPeriod.BudgetPlanVersion,
                                    StatusDesc = (int.Parse(General.EnumDesc(BudgetPlanVersionPeriodStatus.Open)) == m_objDBudgetPlanVersionPeriod.StatusID ? General.EnumName(BudgetPlanVersionPeriodStatus.Open) : General.EnumName(BudgetPlanVersionPeriodStatus.Close)),
                                    StatusID = m_objDBudgetPlanVersionPeriod.StatusID
                                };
                            }

                        }
                    }

                }
                else
                {
                    m_objDBudgetPlanVersionPeriodDA.Data = m_objDBudgetPlanVersionPeriod;
                    m_objDBudgetPlanVersionPeriod.BudgetPlanVersionPeriodID = Guid.NewGuid().ToString().Replace("-", "");
                    m_objDBudgetPlanVersionPeriod.BudgetPlanID = BudgetPlanID;
                    m_objDBudgetPlanVersionPeriod.BudgetPlanVersion = int.Parse(BudgetPlanVersion);
                    m_objDBudgetPlanVersionPeriod.StatusID = int.Parse(General.EnumDesc(BudgetPlanVersionPeriodStatus.Open));
                    if (m_lstBudgetPlanVersionPeriodVM.LastOrDefault() != null)
                        m_objDBudgetPlanVersionPeriod.PeriodVersion = m_lstBudgetPlanVersionPeriodVM.LastOrDefault().PeriodVersion + 1;
                    else
                        m_objDBudgetPlanVersionPeriod.PeriodVersion = 1;

                    if (m_objDBudgetPlanVersionPeriod.PeriodVersion == 1)
                        m_objDBudgetPlanVersionPeriod.BudgetPlanPeriodID = General.EnumName(BudgetPlanPeriod.IQ);
                    else
                        m_objDBudgetPlanVersionPeriod.BudgetPlanPeriodID = General.EnumName(BudgetPlanPeriod.FQ);

                    m_objDBudgetPlanVersionPeriodDA.Insert(false);

                    if (!m_objDBudgetPlanVersionPeriodDA.Success || m_objDBudgetPlanVersionPeriodDA.Message != string.Empty)
                    {
                        return this.Direct(false, m_objDBudgetPlanVersionPeriodDA.Message);
                    }
                    else
                    {
                        m_objBudgetPlanVersionPeriodVM = new BudgetPlanVersionPeriodVM()
                        {
                            BudgetPlanID = m_objDBudgetPlanVersionPeriod.BudgetPlanID,
                            BudgetPlanVersionPeriodID = m_objDBudgetPlanVersionPeriod.BudgetPlanVersionPeriodID,
                            PeriodVersion = m_objDBudgetPlanVersionPeriod.PeriodVersion,
                            BudgetPlanPeriodDesc = (General.EnumName(BudgetPlanPeriod.IQ) == m_objDBudgetPlanVersionPeriod.BudgetPlanPeriodID ? General.EnumDesc(BudgetPlanPeriod.IQ) : General.EnumDesc(BudgetPlanPeriod.FQ)),
                            BudgetPlanPeriodID = m_objDBudgetPlanVersionPeriod.BudgetPlanPeriodID,
                            BudgetPlanVersion = m_objDBudgetPlanVersionPeriod.BudgetPlanVersion,
                            StatusDesc = (int.Parse(General.EnumDesc(BudgetPlanVersionPeriodStatus.Open)) == m_objDBudgetPlanVersionPeriod.StatusID ? General.EnumName(BudgetPlanVersionPeriodStatus.Open) : General.EnumName(BudgetPlanVersionPeriodStatus.Close)),
                            StatusID = m_objDBudgetPlanVersionPeriod.StatusID
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
                return this.Direct();
            }
            //return this.Direct(m_objBudgetPlanVersionPeriodVM);
            return Update("Detail", null);

        }
        public ActionResult Detail(string Caller, string Selected, BudgetPlanVM BudgetPlanVM)
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
            else if (Caller == "ComboBoxVersion")
            {
                if (!string.IsNullOrEmpty(Selected))
                {
                    m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
                }
                else
                {
                    NameValueCollection m_nvcParams = this.Request.Params;
                    m_dicSelectedRow = GetFormData(m_nvcParams, BudgetPlanVM.BudgetPlanID);
                }
            }


            BudgetPlanPeriodVM m_objBudgetPlanPeriodVM = new BudgetPlanPeriodVM();
            if (m_dicSelectedRow.Count > 0)
                m_objBudgetPlanPeriodVM = GetSelectedData(m_dicSelectedRow, null, string.Empty, ref m_strMessage);
            else if (Caller == General.EnumDesc(Buttons.ButtonUpdate))
            {
                m_objBudgetPlanPeriodVM = GetSelectedData(null, BudgetPlanVM, string.Empty, ref m_strMessage);
            }
            else
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            m_objBudgetPlanPeriodVM.ListBudgetPlanVersionPeriodVM = new List<BudgetPlanVersionPeriodVM>();
            m_objBudgetPlanPeriodVM.ListBudgetPlanVersionVendorVM = new List<BudgetPlanVersionVendorVM>();
            if (m_objBudgetPlanPeriodVM != null)
            {
                m_objBudgetPlanPeriodVM.ListBudgetPlanVersionPeriodVM = GetListBudgetPlanVersionPeriod(m_objBudgetPlanPeriodVM.BudgetPlanID, m_objBudgetPlanPeriodVM.BudgetPlanVersion, ref m_strMessage);
                if (m_strMessage != string.Empty)
                {
                    Global.ShowErrorAlert(title, m_strMessage);
                    return this.Direct();
                }
                m_objBudgetPlanPeriodVM.ListBudgetPlanVersionVendorVM = GetListBudgetPlanVersionVendor(m_objBudgetPlanPeriodVM.BudgetPlanID, m_objBudgetPlanPeriodVM.BudgetPlanVersion, ref m_strMessage);
                if (m_strMessage != string.Empty)
                {
                    Global.ShowErrorAlert(title, m_strMessage);
                    return this.Direct();
                }
            }



            ViewDataDictionary m_vddItemGroup = new ViewDataDictionary();
            m_vddItemGroup.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddItemGroup.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objBudgetPlanPeriodVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddItemGroup,
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
                m_dicSelectedRow = GetFormData(m_nvcParams, string.Empty);
                Session[dataSessionName] = JSON.Serialize(m_dicSelectedRow);
            }

            string m_strMessage = string.Empty;
            BudgetPlanPeriodVM m_objBudgetPlanPeriodVM = new BudgetPlanPeriodVM();
            if (m_dicSelectedRow.Count > 0)
                m_objBudgetPlanPeriodVM = GetSelectedData(m_dicSelectedRow, null, string.Empty, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            m_objBudgetPlanPeriodVM.ListBudgetPlanVersionPeriodVM = new List<BudgetPlanVersionPeriodVM>();
            m_objBudgetPlanPeriodVM.ListBudgetPlanVersionVendorVM = new List<BudgetPlanVersionVendorVM>();
            if (m_objBudgetPlanPeriodVM != null)
            {
                m_objBudgetPlanPeriodVM.ListBudgetPlanVersionPeriodVM = GetListBudgetPlanVersionPeriod(m_objBudgetPlanPeriodVM.BudgetPlanID, m_objBudgetPlanPeriodVM.BudgetPlanVersion, ref m_strMessage);
                if (m_strMessage != string.Empty)
                {
                    Global.ShowErrorAlert(title, m_strMessage);
                    return this.Direct();
                }
                m_objBudgetPlanPeriodVM.ListBudgetPlanVersionVendorVM = GetListBudgetPlanVersionVendor(m_objBudgetPlanPeriodVM.BudgetPlanID, m_objBudgetPlanPeriodVM.BudgetPlanVersion, ref m_strMessage);
                if (m_strMessage != string.Empty)
                {
                    Global.ShowErrorAlert(title, m_strMessage);
                    return this.Direct();
                }
            }


            ViewDataDictionary m_vddItemGroup = new ViewDataDictionary();
            m_vddItemGroup.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddItemGroup.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objBudgetPlanPeriodVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddItemGroup,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult Delete(string AvailableVendorID, string Selected, string ListBudgetPlanVersionPeriod)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));
            string m_strMessage = string.Empty;
            //AvailableVendorID = JSON.Deserialize<string>(AvailableVendorID);
            BudgetPlanVersionPeriodVM m_BudgetPlanVersionPeriodVM = JSON.Deserialize<BudgetPlanVersionPeriodVM>(Selected);
            m_BudgetPlanVersionPeriodVM.ListBudgetPlanVersionVendorVM = GetListBudgetPlanVersionVendor(m_BudgetPlanVersionPeriodVM.BudgetPlanID, m_BudgetPlanVersionPeriodVM.BudgetPlanVersion.ToString(), ref m_strMessage, m_BudgetPlanVersionPeriodVM.PeriodVersion.ToString());

            DBudgetPlanVersionEntryDA m_objDBudgetPlanVersionEntryDA = new DBudgetPlanVersionEntryDA();
            m_objDBudgetPlanVersionEntryDA.ConnectionStringName = Global.ConnStrConfigName;

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(string.Join(",", m_BudgetPlanVersionPeriodVM.ListBudgetPlanVersionVendorVM.Select(x => x.BudgetPlanVersionVendorID)));
            m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersionVendorID.Map, m_lstFilter);
            Dictionary<int, DataSet> m_dicDBudgetPlanVersionEntryDA = m_objDBudgetPlanVersionEntryDA.SelectBC(0, 1, true, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpItemBL in m_dicDBudgetPlanVersionEntryDA)
            {
                m_intCount = m_kvpItemBL.Key;
                break;
            }
            if (m_intCount > 0)
            {
                Global.ShowInfoAlert(title, "Is Used by VersionEntry");
                return this.Direct("fail");
            }

            //Delete
            DBudgetPlanVersionPeriodDA m_objDBudgetPlanVersionPeriodDA = new DBudgetPlanVersionPeriodDA();
            m_objDBudgetPlanVersionPeriodDA.ConnectionStringName = Global.ConnStrConfigName;
            string m_strTransName = "DBudgetPlanVersionPeriod";
            object m_objDBConnection = null;
            m_objDBConnection = m_objDBudgetPlanVersionPeriodDA.BeginTrans(m_strTransName);
            List<string> m_lstMessage = new List<string>();

            try
            {
                m_objFilter = new Dictionary<string, List<object>>();
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(m_BudgetPlanVersionPeriodVM.BudgetPlanVersionPeriodID);
                m_objFilter.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanVersionPeriodID.Map, m_lstFilter);

                m_objDBudgetPlanVersionPeriodDA.DeleteBC(m_objFilter, true, m_objDBConnection);


                if (m_BudgetPlanVersionPeriodVM.BudgetPlanPeriodID == General.EnumName(BudgetPlanPeriod.FQ))
                {
                    List<BudgetPlanVersionPeriodVM> m_ListBudgetPlanVersionPeriodVM = JSON.Deserialize<List<BudgetPlanVersionPeriodVM>>(ListBudgetPlanVersionPeriod);

                    //Update
                    DBudgetPlanVersionPeriod m_objDBudgetPlanVersionPeriod = new DBudgetPlanVersionPeriod();
                    m_objDBudgetPlanVersionPeriod.BudgetPlanVersionPeriodID = m_ListBudgetPlanVersionPeriodVM.Where(x => x.BudgetPlanPeriodID == General.EnumName(BudgetPlanPeriod.IQ)).FirstOrDefault().BudgetPlanVersionPeriodID;
                    m_objDBudgetPlanVersionPeriodDA.Data = m_objDBudgetPlanVersionPeriod;
                    m_objDBudgetPlanVersionPeriodDA.Select();
                    m_objDBudgetPlanVersionPeriod.StatusID = 0;
                    m_objDBudgetPlanVersionPeriodDA.Update(true, m_objDBConnection);
                }

                if (!m_objDBudgetPlanVersionPeriodDA.Success || m_objDBudgetPlanVersionPeriodDA.Message != string.Empty) m_lstMessage.Add(m_objDBudgetPlanVersionPeriodDA.Message);

                m_objDBudgetPlanVersionPeriodDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                m_objDBudgetPlanVersionPeriodDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
            }

            if (m_lstMessage.Count <= 0)
            {
                //return this.Direct();
                return Update("Detail", null);
            }
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct("fail");

            //if (!string.IsNullOrEmpty(AvailableVendorID))
            //{
            //    AvailableVendorID = AvailableVendorID.ToString();
            //    if (AvailableVendorID == "true" || AvailableVendorID == "null")
            //        Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Deleted));
            //    else
            //        Global.ShowErrorAlert(title, "Is Used by VersionEntry");
            //}
            //return this.Direct();
        }
        public ActionResult DeleteVendorItem(string VendorID, string BudgetPlanID, string Selected)
        {
            BudgetPlanVersionVendorVM m_BudgetPlanVersionVendorVM = JSON.Deserialize<BudgetPlanVersionVendorVM>(Selected);

            DBudgetPlanVersionEntryDA m_objDBudgetPlanVersionEntryDA = new DBudgetPlanVersionEntryDA();
            m_objDBudgetPlanVersionEntryDA.ConnectionStringName = Global.ConnStrConfigName;

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(m_BudgetPlanVersionVendorVM.BudgetPlanVersionVendorID);
            m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersionVendorID.Map, m_lstFilter);
            Dictionary<int, DataSet> m_dicDBudgetPlanVersionEntryDA = m_objDBudgetPlanVersionEntryDA.SelectBC(0, 1, true, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpItemBL in m_dicDBudgetPlanVersionEntryDA)
            {
                m_intCount = m_kvpItemBL.Key;
                break;
            }
            if (m_intCount > 0)
            {
                Global.ShowInfoAlert(title, "Can't delete this");
                return this.Direct("fail");
            }

            //DELETE
            DBudgetPlanVersionVendorDA m_objDBudgetPlanVersionVendorDA = new DBudgetPlanVersionVendorDA();
            m_objDBudgetPlanVersionVendorDA.ConnectionStringName = Global.ConnStrConfigName;
            string m_strTransName = "DBudgetPlanVersionVendorDA";
            object m_objDBConnection = null;
            m_objDBConnection = m_objDBudgetPlanVersionVendorDA.BeginTrans(m_strTransName);
            List<string> m_lstMessage = new List<string>();
            try
            {
                m_objFilter = new Dictionary<string, List<object>>();
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(m_BudgetPlanVersionVendorVM.BudgetPlanVersionVendorID);
                m_objFilter.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionVendorID.Map, m_lstFilter);

                m_objDBudgetPlanVersionVendorDA.DeleteBC(m_objFilter, true, m_objDBConnection);

                if (!m_objDBudgetPlanVersionVendorDA.Success || m_objDBudgetPlanVersionVendorDA.Message != string.Empty) m_lstMessage.Add(m_objDBudgetPlanVersionVendorDA.Message);

                m_objDBudgetPlanVersionVendorDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                m_objDBudgetPlanVersionVendorDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
            }

            if (m_lstMessage.Count <= 0)
            {
                return this.Direct();
            }
            Global.ShowInfoAlert(title, "Can't delete this");
            return this.Direct("fail");



            //bool success = false;
            //if (!string.IsNullOrEmpty(BudgetPlanID))
            //{
            //    Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, "Can't delete existing Parameter"));
            //    success = false;
            //}
            //else
            //{
            //    Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Deleted));
            //    success = true;
            //}

            //return this.Direct(success);
        }
        public ActionResult Save(string Action)
        {
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            DBudgetPlanVersionVendorDA m_objBudgetPlanVersionVendorDA = new DBudgetPlanVersionVendorDA();
            DBudgetPlanVersionVendor m_objBudgetPlanVersionVendor = new DBudgetPlanVersionVendor();
            string m_strItemID = string.Empty;
            m_objBudgetPlanVersionVendorDA.ConnectionStringName = Global.ConnStrConfigName;


            string m_strBudgetPlanID = "";
            string m_strBudgetPlanVersion = "";
            object m_objDBConnection = null;
            string m_strTransName = "BudgetPlanPeriod";

            try
            {
                m_strBudgetPlanID = this.Request.Params[BudgetPlanPeriodVM.Prop.BudgetPlanID.Name];
                m_strBudgetPlanVersion = this.Request.Params[BudgetPlanPeriodVM.Prop.BudgetPlanVersion.Name];
                m_objDBConnection = m_objBudgetPlanVersionVendorDA.BeginTrans(m_strTransName);
                string m_strlstBudgetPlanVersionVendorVMs = this.Request.Params[BudgetPlanPeriodVM.Prop.ListBudgetPlanVersionVendorVM.Name];

                List<BudgetPlanVersionVendorVM> lst_BudgetPlanVersionVendor = JSON.Deserialize<List<BudgetPlanVersionVendorVM>>(m_strlstBudgetPlanVersionVendorVMs);

                List<string> success_status = new List<string>();

                m_lstMessage = IsSaveValid(lst_BudgetPlanVersionVendor);


                if (m_lstMessage.Count <= 0)
                {
                    DBudgetPlanVersionVendor m_objDBudgetPlanVersionVendor = new DBudgetPlanVersionVendor();
                    foreach (BudgetPlanVersionVendorVM objBudgetPlanVersionVendorVM in lst_BudgetPlanVersionVendor)
                    {
                        m_objDBudgetPlanVersionVendor.BudgetPlanVersionVendorID = objBudgetPlanVersionVendorVM.BudgetPlanVersionVendorID;
                        m_objBudgetPlanVersionVendorDA.Data = m_objDBudgetPlanVersionVendor;
                        m_objBudgetPlanVersionVendorDA.Select();

                        if (string.IsNullOrEmpty(m_objBudgetPlanVersionVendorDA.Data.BudgetPlanVersionVendorID))
                            m_objDBudgetPlanVersionVendor.BudgetPlanVersionVendorID = Guid.NewGuid().ToString().Replace("-", "");

                        m_objDBudgetPlanVersionVendor.BudgetPlanVersionPeriodID = objBudgetPlanVersionVendorVM.BudgetPlanVersionPeriodID;
                        m_objDBudgetPlanVersionVendor.VendorID = objBudgetPlanVersionVendorVM.VendorID;
                        m_objDBudgetPlanVersionVendor.StatusID = 0;
                        m_objDBudgetPlanVersionVendor.StartDate = objBudgetPlanVersionVendorVM.StartDate.Date + TimeSpan.Parse(objBudgetPlanVersionVendorVM.StartDateHours);
                        m_objDBudgetPlanVersionVendor.EndDate = objBudgetPlanVersionVendorVM.EndDate.Date + TimeSpan.Parse(objBudgetPlanVersionVendorVM.EndDateHours);

                        if (string.IsNullOrEmpty(objBudgetPlanVersionVendorVM.BudgetPlanVersionVendorID))
                            m_objBudgetPlanVersionVendorDA.Insert(true, m_objDBConnection);
                        else
                            m_objBudgetPlanVersionVendorDA.Update(true, m_objDBConnection);

                    }

                }
                if (m_lstMessage.Count > 0 || !m_objBudgetPlanVersionVendorDA.Success)
                {
                    m_lstMessage.Add(m_objBudgetPlanVersionVendorDA.Message);
                    m_objBudgetPlanVersionVendorDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                }
                else if (m_objBudgetPlanVersionVendorDA.Success || m_objBudgetPlanVersionVendorDA.Message == string.Empty || !m_lstMessage.Any())
                    m_objBudgetPlanVersionVendorDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
                else
                    m_objBudgetPlanVersionVendorDA.EndTrans(ref m_objDBConnection, false, m_strTransName);


            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                m_objBudgetPlanVersionVendorDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
                return this.Direct();
            }
            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
                return Detail(Action, null, new BudgetPlanVM() { BudgetPlanID = m_strBudgetPlanID, BudgetPlanVersion = Convert.ToInt16(m_strBudgetPlanVersion) });

            }
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);
        }

        #endregion

        private List<string> IsSaveValid(List<BudgetPlanVersionVendorVM> m_lstBudgetPlanVersionVendorVM)
        {
            List<string> m_lstReturn = new List<string>();
            foreach (BudgetPlanVersionVendorVM m_objBudgetPlanVersionVendorVM in m_lstBudgetPlanVersionVendorVM)
            {
                if (string.IsNullOrEmpty(m_objBudgetPlanVersionVendorVM.VendorID))
                    m_lstReturn.Add(BudgetPlanVersionVendorVM.Prop.VendorID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
                if (string.IsNullOrEmpty(m_objBudgetPlanVersionVendorVM.StartDate.ToString()))
                    m_lstReturn.Add(BudgetPlanVersionVendorVM.Prop.StartDate.Desc + " " + General.EnumDesc(MessageLib.mustFill));
                if (string.IsNullOrEmpty(m_objBudgetPlanVersionVendorVM.EndDate.ToString()))
                    m_lstReturn.Add(BudgetPlanVersionVendorVM.Prop.EndDate.Desc + " " + General.EnumDesc(MessageLib.mustFill));
                if (string.IsNullOrEmpty(m_objBudgetPlanVersionVendorVM.StartDateHours.ToString()))
                    m_lstReturn.Add(BudgetPlanVersionVendorVM.Prop.StartDate.Desc + "Hour " + General.EnumDesc(MessageLib.mustFill));
                if (string.IsNullOrEmpty(m_objBudgetPlanVersionVendorVM.EndDateHours.ToString()))
                    m_lstReturn.Add(BudgetPlanVersionVendorVM.Prop.EndDate.Desc + "Hour " + General.EnumDesc(MessageLib.mustFill));
            }
            return m_lstReturn;
        }
        private Dictionary<string, object> GetFormData(NameValueCollection parameters, string BudgetPlanID)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();
            m_dicReturn.Add(BudgetPlanVM.Prop.BudgetPlanID.Name, (parameters[BudgetPlanVM.Prop.BudgetPlanID.Name].ToString() == string.Empty ? BudgetPlanID : parameters[BudgetPlanVM.Prop.BudgetPlanID.Name].ToString().Split(',')[0]));
            m_dicReturn.Add(BudgetPlanVM.Prop.Description.Name, parameters[BudgetPlanVM.Prop.Description.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.BudgetPlanTemplateID.Name, parameters[BudgetPlanVM.Prop.BudgetPlanTemplateID.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.BudgetPlanTemplateDesc.Name, parameters[BudgetPlanVM.Prop.BudgetPlanTemplateDesc.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.BudgetPlanTypeDesc.Name, parameters[BudgetPlanVM.Prop.BudgetPlanTypeDesc.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.CompanyDesc.Name, parameters[BudgetPlanVM.Prop.CompanyDesc.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.RegionDesc.Name, parameters[BudgetPlanVM.Prop.RegionDesc.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.LocationDesc.Name, parameters[BudgetPlanVM.Prop.LocationDesc.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.DivisionDesc.Name, parameters[BudgetPlanVM.Prop.DivisionDesc.Name]);

            m_dicReturn.Add(BudgetPlanVM.Prop.ProjectID.Name, parameters[BudgetPlanVM.Prop.ProjectID.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.ProjectDesc.Name, parameters[BudgetPlanVM.Prop.ProjectDesc.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.ClusterID.Name, parameters[BudgetPlanVM.Prop.ClusterID.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.ClusterDesc.Name, parameters[BudgetPlanVM.Prop.ClusterDesc.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.UnitTypeID.Name, parameters[BudgetPlanVM.Prop.UnitTypeID.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.UnitTypeDesc.Name, parameters[BudgetPlanVM.Prop.UnitTypeDesc.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.Area.Name, parameters[BudgetPlanVM.Prop.Area.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.BudgetPlanVersion.Name, parameters[BudgetPlanVM.Prop.BudgetPlanVersion.Name].ToString().Split(',')[0]);
            m_dicReturn.Add(BudgetPlanVM.Prop.CreatedDate.Name, parameters[BudgetPlanVM.Prop.CreatedDate.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.ModifiedDate.Name, parameters[BudgetPlanVM.Prop.ModifiedDate.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.StatusDesc.Name, parameters[BudgetPlanVM.Prop.StatusDesc.Name]);

            return m_dicReturn;
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
            m_lstFilter.Add("DBudgetPlanVersion");
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
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.StatusIDVendor.MapAlias);
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
                    m_objBudgetPlanVM.StatusID = int.Parse(m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.StatusIDVendor.Name].ToString());
                    m_lstBudgetPlanVersionVendorVM.Add(m_objBudgetPlanVM);
                }
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDBudgetPlanVersionVendorDA.Message;

            //List<BudgetPlanVM> listBudgetPlan = (m_lstBudgetPlanVM.Any() ? m_lstBudgetPlanVM.Where(d => d.MaxBudgetPlanVersion == d.BudgetPlanVersion).ToList() : new List<BudgetPlanVM>());
            return m_lstBudgetPlanVersionVendorVM.Where(x => x.MaxPeriodVersion == x.PeriodVersion).ToList();

        }
        private List<BudgetPlanVersionVendorVM> GetListBudgetPlanVersionVendor(string budgetPlanID, string budgetPlanVersion, ref string message, string periodVersion = "")
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
            //m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.AvailableVendorID.MapAlias);

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

            if (periodVersion != "")
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(periodVersion);
                m_objFilter.Add(BudgetPlanVersionPeriodVM.Prop.PeriodVersion.Map, m_lstFilter);
            }

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
                    //m_objBudgetPlanVM.AllowDelete = !string.IsNullOrEmpty(m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.AvailableVendorID.Name].ToString()) ? false : true;
                    m_objBudgetPlanVM.StartDateHours = DateTime.Parse(m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.StartDate.Name].ToString()).ToString(Global.ShortTimeFormat);
                    m_objBudgetPlanVM.EndDateHours = DateTime.Parse(m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.EndDate.Name].ToString()).ToString(Global.ShortTimeFormat);
                    m_lstBudgetPlanVersionVendorVM.Add(m_objBudgetPlanVM);
                }
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDBudgetPlanVersionVendorDA.Message;

            return m_lstBudgetPlanVersionVendorVM;

        }
        private List<BudgetPlanVersionPeriodVM> GetListBudgetPlanVersionPeriod(string BudgetPlanID, string BudgetPlanVersion, ref string message)
        {
            List<BudgetPlanVersionPeriodVM> m_lsDBudgetPlanVersionPeriodVM = new List<BudgetPlanVersionPeriodVM>();
            DBudgetPlanVersionPeriodDA m_objDBudgetPlanVersionPeriodDA = new DBudgetPlanVersionPeriodDA();
            m_objDBudgetPlanVersionPeriodDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanVersionPeriodID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanPeriodDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanPeriodID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionPeriodVM.Prop.StatusID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionPeriodVM.Prop.StatusDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionPeriodVM.Prop.PeriodVersion.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanID);
            m_objFilter.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(BudgetPlanVersion);
            m_objFilter.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicDBudgetPlanVersionPeriodDA = m_objDBudgetPlanVersionPeriodDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDBudgetPlanVersionPeriodDA.Success)
            {
                foreach (DataRow m_drDBudgetPlanVersionPeriodDA in m_dicDBudgetPlanVersionPeriodDA[0].Tables[0].Rows)
                {
                    BudgetPlanVersionPeriodVM m_objBudgetPlanVM = new BudgetPlanVersionPeriodVM();
                    m_objBudgetPlanVM.BudgetPlanVersionPeriodID = m_drDBudgetPlanVersionPeriodDA[BudgetPlanVersionPeriodVM.Prop.BudgetPlanVersionPeriodID.Name].ToString();
                    m_objBudgetPlanVM.BudgetPlanID = m_drDBudgetPlanVersionPeriodDA[BudgetPlanVersionPeriodVM.Prop.BudgetPlanID.Name].ToString();
                    m_objBudgetPlanVM.BudgetPlanVersion = int.Parse(m_drDBudgetPlanVersionPeriodDA[BudgetPlanVersionPeriodVM.Prop.BudgetPlanVersion.Name].ToString());
                    m_objBudgetPlanVM.StatusID = int.Parse(m_drDBudgetPlanVersionPeriodDA[BudgetPlanVersionPeriodVM.Prop.StatusID.Name].ToString());
                    m_objBudgetPlanVM.StatusDesc = m_drDBudgetPlanVersionPeriodDA[BudgetPlanVersionPeriodVM.Prop.StatusDesc.Name].ToString();
                    m_objBudgetPlanVM.BudgetPlanPeriodID = m_drDBudgetPlanVersionPeriodDA[BudgetPlanVersionPeriodVM.Prop.BudgetPlanPeriodID.Name].ToString();
                    m_objBudgetPlanVM.BudgetPlanPeriodDesc = m_drDBudgetPlanVersionPeriodDA[BudgetPlanVersionPeriodVM.Prop.BudgetPlanPeriodDesc.Name].ToString();
                    m_objBudgetPlanVM.PeriodVersion = int.Parse(m_drDBudgetPlanVersionPeriodDA[BudgetPlanVersionPeriodVM.Prop.PeriodVersion.Name].ToString());
                    //m_objBudgetPlanVM.ListBudgetPlanVersionVendorVM = GetListBudgetPlanVersionVendor(m_drDBudgetPlanVersionPeriodDA[BudgetPlanVersionPeriodVM.Prop.BudgetPlanVersionPeriodID.Name].ToString(), ref message);
                    m_lsDBudgetPlanVersionPeriodVM.Add(m_objBudgetPlanVM);
                }
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDBudgetPlanVersionPeriodDA.Message;


            return m_lsDBudgetPlanVersionPeriodVM;

        }
        private BudgetPlanPeriodVM GetBudgetPlanPeriod(string BudgetPlanID, int Version, ref string message)
        {
            BudgetPlanPeriodVM m_BudgetPlanVersionVendorVM = new BudgetPlanPeriodVM();
            TBudgetPlanDA m_objTBudgetPlanDA = new TBudgetPlanDA();
            m_objTBudgetPlanDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanPeriodVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanPeriodVM.Prop.Description.MapAlias);
            m_lstSelect.Add(BudgetPlanPeriodVM.Prop.BudgetPlanTemplateDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanPeriodVM.Prop.BudgetPlanTypeID.MapAlias);
            m_lstSelect.Add(BudgetPlanPeriodVM.Prop.CompanyDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanPeriodVM.Prop.RegionDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanPeriodVM.Prop.LocationDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanPeriodVM.Prop.DivisionDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanPeriodVM.Prop.ProjectDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanPeriodVM.Prop.ClusterDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanPeriodVM.Prop.UnitTypeDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanPeriodVM.Prop.Area.MapAlias);
            m_lstSelect.Add(BudgetPlanPeriodVM.Prop.BudgetPlanVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanPeriodVM.Prop.CreatedDate.MapAlias);
            m_lstSelect.Add(BudgetPlanPeriodVM.Prop.ModifiedDate.MapAlias);
            m_lstSelect.Add(BudgetPlanPeriodVM.Prop.StatusDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanID);
            m_objFilter.Add(BudgetPlanPeriodVM.Prop.BudgetPlanID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(Version);
            m_objFilter.Add(BudgetPlanPeriodVM.Prop.BudgetPlanVersion.Map, m_lstFilter);
            BudgetPlanPeriodVM m_objBudgetPlanVM = new BudgetPlanPeriodVM();
            Dictionary<int, DataSet> m_dicTBudgetPlanDA = m_objTBudgetPlanDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objTBudgetPlanDA.Message == string.Empty)
            {
                string messages_ = "";
                DataRow m_drTBudgetPlanDA = m_dicTBudgetPlanDA[0].Tables[0].Rows[0];

                m_objBudgetPlanVM.BudgetPlanID = m_drTBudgetPlanDA[BudgetPlanPeriodVM.Prop.BudgetPlanID.Name].ToString();
                m_objBudgetPlanVM.Description = m_drTBudgetPlanDA[BudgetPlanPeriodVM.Prop.Description.Name].ToString();
                m_objBudgetPlanVM.BudgetPlanTemplateDesc = m_drTBudgetPlanDA[BudgetPlanPeriodVM.Prop.BudgetPlanTemplateDesc.Name].ToString();
                m_objBudgetPlanVM.BudgetPlanTypeID = m_drTBudgetPlanDA[BudgetPlanPeriodVM.Prop.BudgetPlanTypeID.Name].ToString();
                m_objBudgetPlanVM.CompanyDesc = m_drTBudgetPlanDA[BudgetPlanPeriodVM.Prop.CompanyDesc.Name].ToString();
                m_objBudgetPlanVM.RegionDesc = m_drTBudgetPlanDA[BudgetPlanPeriodVM.Prop.RegionDesc.Name].ToString();
                m_objBudgetPlanVM.LocationDesc = m_drTBudgetPlanDA[BudgetPlanPeriodVM.Prop.LocationDesc.Name].ToString();
                m_objBudgetPlanVM.DivisionDesc = m_drTBudgetPlanDA[BudgetPlanPeriodVM.Prop.DivisionDesc.Name].ToString();
                m_objBudgetPlanVM.ProjectDesc = m_drTBudgetPlanDA[BudgetPlanPeriodVM.Prop.ProjectDesc.Name].ToString();
                m_objBudgetPlanVM.ClusterDesc = m_drTBudgetPlanDA[BudgetPlanPeriodVM.Prop.ClusterDesc.Name].ToString();
                m_objBudgetPlanVM.UnitTypeDesc = m_drTBudgetPlanDA[BudgetPlanPeriodVM.Prop.UnitTypeDesc.Name].ToString();
                m_objBudgetPlanVM.Area = decimal.Parse(m_drTBudgetPlanDA[BudgetPlanPeriodVM.Prop.Area.Name].ToString());
                m_objBudgetPlanVM.BudgetPlanVersion = m_drTBudgetPlanDA[BudgetPlanPeriodVM.Prop.BudgetPlanVersion.Name].ToString();
                m_objBudgetPlanVM.CreatedDate = DateTime.Parse(m_drTBudgetPlanDA[BudgetPlanPeriodVM.Prop.CreatedDate.Name].ToString());
                m_objBudgetPlanVM.ModifiedDate = DateTime.Parse(m_drTBudgetPlanDA[BudgetPlanPeriodVM.Prop.ModifiedDate.Name].ToString());
                m_objBudgetPlanVM.StatusDesc = m_drTBudgetPlanDA[BudgetPlanPeriodVM.Prop.StatusDesc.Name].ToString();
                m_objBudgetPlanVM.ListBudgetPlanVersionPeriodVM = GetListBudgetPlanVersionPeriod(m_objBudgetPlanVM.BudgetPlanID, m_objBudgetPlanVM.BudgetPlanVersion, ref messages_);

            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objTBudgetPlanDA.Message;


            return m_objBudgetPlanVM;

        }
        private BudgetPlanVersionPeriodVM GetBudgetPlanVersionPeriod(string BudgetPlanID, int BudgetPlanVersion, string StatusID, ref string message)
        {
            BudgetPlanVersionPeriodVM m_objDBudgetPlanVersionPeriodVM = new BudgetPlanVersionPeriodVM();
            DBudgetPlanVersionPeriodDA m_objDBudgetPlanVersionPeriodDA = new DBudgetPlanVersionPeriodDA();
            m_objDBudgetPlanVersionPeriodDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanVersionPeriodID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanPeriodDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanPeriodID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionPeriodVM.Prop.StatusID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionPeriodVM.Prop.StatusDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanID);
            m_objFilter.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanVersion);
            m_objFilter.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

            if (string.IsNullOrEmpty(StatusID))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(BudgetPlanVersion);
                m_objFilter.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanVersion.Map, m_lstFilter);
            }

            Dictionary<int, DataSet> m_dicDBudgetPlanVersionPeriodDA = m_objDBudgetPlanVersionPeriodDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDBudgetPlanVersionPeriodDA.Message == string.Empty)
            {
                DataRow m_drDBudgetPlanVersionPeriodDA = m_dicDBudgetPlanVersionPeriodDA[0].Tables[0].Rows[0];
                m_objDBudgetPlanVersionPeriodVM.BudgetPlanVersionPeriodID = m_drDBudgetPlanVersionPeriodDA[BudgetPlanVersionPeriodVM.Prop.BudgetPlanVersionPeriodID.Name].ToString();
                m_objDBudgetPlanVersionPeriodVM.BudgetPlanID = m_drDBudgetPlanVersionPeriodDA[BudgetPlanVersionPeriodVM.Prop.BudgetPlanID.Name].ToString();
                m_objDBudgetPlanVersionPeriodVM.BudgetPlanVersion = int.Parse(m_drDBudgetPlanVersionPeriodDA[BudgetPlanVersionPeriodVM.Prop.BudgetPlanVersion.Name].ToString());
                m_objDBudgetPlanVersionPeriodVM.StatusID = int.Parse(m_drDBudgetPlanVersionPeriodDA[BudgetPlanVersionPeriodVM.Prop.StatusID.Name].ToString());
                m_objDBudgetPlanVersionPeriodVM.StatusDesc = m_drDBudgetPlanVersionPeriodDA[BudgetPlanVersionPeriodVM.Prop.StatusDesc.Name].ToString();
                m_objDBudgetPlanVersionPeriodVM.BudgetPlanPeriodID = m_drDBudgetPlanVersionPeriodDA[BudgetPlanVersionPeriodVM.Prop.BudgetPlanPeriodID.Name].ToString();
                m_objDBudgetPlanVersionPeriodVM.BudgetPlanPeriodDesc = m_drDBudgetPlanVersionPeriodDA[BudgetPlanVersionPeriodVM.Prop.BudgetPlanPeriodDesc.Name].ToString();

            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDBudgetPlanVersionPeriodDA.Message;

            return m_objDBudgetPlanVersionPeriodVM;

        }
        public ActionResult GetApprovalStatus(StoreRequestParameters parameters)
        {


            List<string> m_Status = new List<string>();
            m_Status.Add("Approved");
            m_Status.Add("Draft");
            m_Status.Add("Cancelled");

            return this.Store(m_Status);
        }
        private BudgetPlanPeriodVM GetSelectedData(Dictionary<string, object> selected, BudgetPlanVM datafilter, string budgetPlanVersion, ref string message)
        {
            BudgetPlanPeriodVM m_objBudgetPlanVM = new BudgetPlanPeriodVM();
            TBudgetPlanDA m_objTBudgetPlanDA = new TBudgetPlanDA();
            m_objTBudgetPlanDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.Description.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanTemplateDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanTemplateID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanTypeDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.CompanyDesc.MapAlias);
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
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.CreatedDate.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.ModifiedDate.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.StatusID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.StatusDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            if (datafilter != null)
            {
                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(datafilter.BudgetPlanID);
                m_objFilter.Add(BudgetPlanVM.Prop.BudgetPlanID.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(datafilter.BudgetPlanVersion);
                m_objFilter.Add(BudgetPlanVM.Prop.BudgetPlanVersion.Map, m_lstFilter);
            }
            else
            {
                foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
                {
                    if (m_kvpSelectedRow.Key == BudgetPlanVersionVM.Prop.BudgetPlanVersion.Name)
                    {
                        List<object> m_lstFilter = new List<object>();
                        if (!string.IsNullOrEmpty(budgetPlanVersion))
                        {
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(budgetPlanVersion);
                            m_objFilter.Add(BudgetPlanVersionVM.Prop.BudgetPlanVersion.Map, m_lstFilter);
                        }
                        else
                        {
                            m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                            m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_kvpSelectedRow.Value);
                            m_objFilter.Add(BudgetPlanVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                        }

                    }
                    if (m_kvpSelectedRow.Key == BudgetPlanVersionVM.Prop.BudgetPlanID.Name)
                    {
                        List<object> m_lstFilter = new List<object>();
                        m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                        m_lstFilter = new List<object>();
                        m_lstFilter.Add(Operator.Equals);
                        m_lstFilter.Add(m_kvpSelectedRow.Value);
                        m_objFilter.Add(BudgetPlanVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                    }
                }
            }

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
                m_objBudgetPlanVM.BudgetPlanVersion = m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanVersion.Name].ToString();
                m_objBudgetPlanVM.CreatedDate = DateTime.Parse(m_drTBudgetPlanDA[BudgetPlanVM.Prop.CreatedDate.Name].ToString());
                m_objBudgetPlanVM.ModifiedDate = DateTime.Parse(m_drTBudgetPlanDA[BudgetPlanVM.Prop.ModifiedDate.Name].ToString());
                m_objBudgetPlanVM.StatusDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.StatusDesc.Name].ToString();

            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objTBudgetPlanDA.Message;

            return m_objBudgetPlanVM;

        }

    }
}