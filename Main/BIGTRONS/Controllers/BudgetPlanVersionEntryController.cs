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
using System.Web.Script.Serialization;
using XMVC = Ext.Net.MVC;
using NPOI.HSSF.UserModel;
using System.IO;
using NPOI.SS.UserModel;
namespace com.SML.BIGTRONS.Controllers
{
    public class BudgetPlanVersionEntryController : BaseController
    {
        private readonly string title = "Budget Plan Version Entry";
        private readonly string dataSessionName = "FormData";
        //private string VendorID;

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
            //HttpContext.User.Identity.Name

            List<string> m_lstMessage = new List<string>();

            string m_strVendorID = "";
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                m_strVendorID = HttpContext.User.Identity.Name;

                MUserDA m_objMUserDA = new MUserDA();
                m_objMUserDA.ConnectionStringName = Global.ConnStrConfigName;

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(m_strVendorID);
                m_objFilter.Add(UserVM.Prop.UserID.Map, m_lstFilter);

                m_lstSelect = new List<string>();
                m_lstSelect.Add(UserVM.Prop.VendorID.MapAlias);

                Dictionary<int, DataSet> m_dicMUserDA = m_objMUserDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null, null);

                if (m_objMUserDA.Success)
                {
                    m_strVendorID = m_dicMUserDA[0].Tables[0].Rows[0][UserVM.Prop.VendorID.Name].ToString();
                    //VendorID = m_strVendorID;
                }
            }
            else
            {
                return this.Store(new List<BudgetPlanVersionVendorVM>(), 0);
            }

            DBudgetPlanVersionVendorDA m_objDBudgetPlanVersionVendorDA = new DBudgetPlanVersionVendorDA();
            m_objDBudgetPlanVersionVendorDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMBudgetPlanVersionEntry = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            m_objFilter = new Dictionary<string, List<object>>();
            m_lstFilter = new List<object>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMBudgetPlanVersionEntry.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = BudgetPlanVersionVendorVM.Prop.Map(m_strDataIndex, false);
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
                        if (m_strDataIndex.Contains("Date"))
                        {
                            m_lstFilter.Add(m_objStart != null ? m_objStart + " 00:00:00" : "");
                            m_lstFilter.Add(m_objEnd != null ? m_objEnd + " 23:59:59.999" : "");
                            m_objFilter.Add(m_strDataIndex, m_lstFilter);
                        }
                        else
                        {
                            m_lstFilter.Add(m_objStart != null ? m_objStart : "");
                            m_lstFilter.Add(m_objEnd != null ? m_objEnd : "");
                            m_objFilter.Add(m_strDataIndex, m_lstFilter);
                        }

                    }
                }
            }

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(m_strVendorID);
            m_objFilter.Add(BudgetPlanVersionVendorVM.Prop.VendorID.Map, m_lstFilter);

            ////m_lstFilter = new List<object>();
            ////m_lstFilter.Add(Operator.None);
            ////m_objFilter.Add("'" + DateTime.Now.ToString(Global.SqlDateFormat) + "' BETWEEN " + BudgetPlanVersionVendorVM.Prop.StartDate.Map + " AND " + BudgetPlanVersionVendorVM.Prop.EndDate.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(2);
            m_objFilter.Add(BudgetPlanVersionVendorVM.Prop.StatusID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(0);
            m_objFilter.Add(BudgetPlanVersionVendorVM.Prop.StatusIDPeriod.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicDBudgetPlanVersionVendorDA = m_objDBudgetPlanVersionVendorDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpBudgetPlanVersionVendorBL in m_dicDBudgetPlanVersionVendorDA)
            {
                m_intCount = m_kvpBudgetPlanVersionVendorBL.Key;
                break;
            }

            List<BudgetPlanVersionVendorVM> m_lstBudgetPlanVersionVendorVM = new List<BudgetPlanVersionVendorVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                m_lstSelect = new List<string>();
                m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanID.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionVendorID.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionPeriodID.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanVersion.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.Description.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanTemplateDesc.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.ProjectDesc.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.ClusterDesc.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.UnitTypeDesc.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.StartDate.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.EndDate.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.StatusID.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.StatusDesc.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.FeePercentage.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(BudgetPlanVersionVendorVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                //m_dicDBudgetPlanVersionVendorDA = m_objDBudgetPlanVersionVendorDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                m_dicDBudgetPlanVersionVendorDA = m_objDBudgetPlanVersionVendorDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objDBudgetPlanVersionVendorDA.Message == string.Empty)
                {
                    m_lstBudgetPlanVersionVendorVM = (
                        from DataRow m_drMBudgetPlanVersionVendorDA in m_dicDBudgetPlanVersionVendorDA[0].Tables[0].Rows
                        select new BudgetPlanVersionVendorVM()
                        {
                            FeePercentage = m_drMBudgetPlanVersionVendorDA[BudgetPlanVersionVendorVM.Prop.FeePercentage.Name].ToString().Length == 0 ? null : (decimal?)decimal.Parse(m_drMBudgetPlanVersionVendorDA[BudgetPlanVersionVendorVM.Prop.FeePercentage.Name].ToString()),
                            BudgetPlanID = m_drMBudgetPlanVersionVendorDA[BudgetPlanVersionVendorVM.Prop.BudgetPlanID.Name].ToString(),
                            BudgetPlanVersionVendorID = m_drMBudgetPlanVersionVendorDA[BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionVendorID.Name].ToString(),
                            BudgetPlanVersionPeriodID = m_drMBudgetPlanVersionVendorDA[BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionPeriodID.Name].ToString(),
                            BudgetPlanVersion = int.Parse(m_drMBudgetPlanVersionVendorDA[BudgetPlanVersionVendorVM.Prop.BudgetPlanVersion.Name].ToString()),
                            Description = m_drMBudgetPlanVersionVendorDA[BudgetPlanVersionVendorVM.Prop.Description.Name].ToString(),
                            BudgetPlanTemplateDesc = m_drMBudgetPlanVersionVendorDA[BudgetPlanVersionVendorVM.Prop.BudgetPlanTemplateDesc.Name].ToString(),
                            ProjectDesc = m_drMBudgetPlanVersionVendorDA[BudgetPlanVersionVendorVM.Prop.ProjectDesc.Name].ToString(),
                            ClusterDesc = m_drMBudgetPlanVersionVendorDA[BudgetPlanVersionVendorVM.Prop.ClusterDesc.Name].ToString(),
                            UnitTypeDesc = m_drMBudgetPlanVersionVendorDA[BudgetPlanVersionVendorVM.Prop.UnitTypeDesc.Name].ToString(),
                            StartDate = DateTime.Parse(m_drMBudgetPlanVersionVendorDA[BudgetPlanVersionVendorVM.Prop.StartDate.Name].ToString()),
                            EndDate = DateTime.Parse(m_drMBudgetPlanVersionVendorDA[BudgetPlanVersionVendorVM.Prop.EndDate.Name].ToString()),
                            StatusDesc = m_drMBudgetPlanVersionVendorDA[BudgetPlanVersionVendorVM.Prop.StatusDesc.Name].ToString()
                        }
                    ).ToList();

                    List<BudgetPlanVersionVendorVM> m_lstBudgetPlanVersionVendorVMNew = new List<BudgetPlanVersionVendorVM>();
                    foreach (string item in m_lstBudgetPlanVersionVendorVM.Select(m => m.BudgetPlanID).Distinct().ToList())
                    {
                        m_lstBudgetPlanVersionVendorVMNew.Add(m_lstBudgetPlanVersionVendorVM.OrderByDescending(m => m.BudgetPlanVersion).FirstOrDefault(m => m.BudgetPlanID == item));
                    }

                    m_intCount = m_lstBudgetPlanVersionVendorVMNew.Count;
                    m_lstBudgetPlanVersionVendorVM = m_lstBudgetPlanVersionVendorVMNew.Skip(m_intSkip).Take(m_intLength).ToList();
                }
            }
            return this.Store(m_lstBudgetPlanVersionVendorVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            DBudgetPlanVersionEntryDA m_objDBudgetPlanVersionEntryDA = new DBudgetPlanVersionEntryDA();
            m_objDBudgetPlanVersionEntryDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcDBudgetPlanVersionEntry = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcDBudgetPlanVersionEntry.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = BudgetPlanVersionEntryVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicDBudgetPlanVersionEntryDA = m_objDBudgetPlanVersionEntryDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpBudgetPlanVersionEntryBL in m_dicDBudgetPlanVersionEntryDA)
            {
                m_intCount = m_kvpBudgetPlanVersionEntryBL.Key;
                break;
            }

            List<BudgetPlanVersionEntryVM> m_lstBudgetPlanVersionEntryVM = new List<BudgetPlanVersionEntryVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanID.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersion.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersionStructureID.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.Info.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.MaterialAmount.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.MiscAmount.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.VendorID.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.Volume.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.WageAmount.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(BudgetPlanVersionEntryVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicDBudgetPlanVersionEntryDA = m_objDBudgetPlanVersionEntryDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objDBudgetPlanVersionEntryDA.Message == string.Empty)
                {
                    m_lstBudgetPlanVersionEntryVM = (
                        from DataRow m_drMBudgetPlanVersionEntryDA in m_dicDBudgetPlanVersionEntryDA[0].Tables[0].Rows
                        select new BudgetPlanVersionEntryVM()
                        {
                            BudgetPlanID = m_drMBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.BudgetPlanID.Name].ToString(),
                            BudgetPlanVersion = int.Parse(m_drMBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.BudgetPlanVersion.Name].ToString()),
                            BudgetPlanVersionStructureID = m_drMBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.BudgetPlanVersionStructureID.Name].ToString(),
                            Info = m_drMBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.Info.Name].ToString(),
                            MaterialAmount = decimal.Parse(m_drMBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.MaterialAmount.Name].ToString()),
                            MiscAmount = decimal.Parse(m_drMBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.MiscAmount.Name].ToString()),
                            VendorID = m_drMBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.VendorID.Name].ToString(),
                            Volume = decimal.Parse(m_drMBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.Volume.Name].ToString()),
                            WageAmount = decimal.Parse(m_drMBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.WageAmount.Name].ToString())
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstBudgetPlanVersionEntryVM, m_intCount);
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

            BudgetPlanVersionEntryVM m_objBudgetPlanVersionEntryVM = new BudgetPlanVersionEntryVM();
            ViewDataDictionary m_vddBudgetPlanVersionEntry = new ViewDataDictionary();
            m_vddBudgetPlanVersionEntry.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddBudgetPlanVersionEntry.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
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
                Model = m_objBudgetPlanVersionEntryVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddBudgetPlanVersionEntry,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Detail(string Caller, string Selected)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            BudgetPlanVersionEntryVM m_objBudgetPlanVersionEntryVM = new BudgetPlanVersionEntryVM();
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
                m_objBudgetPlanVersionEntryVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage,null,false);
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
                Model = m_objBudgetPlanVersionEntryVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddWorkCenter,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Update(string Caller, string Selected,string SelectedCopy)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Update)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            bool IsCallerFromGetData = false;
            Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();
            Dictionary<string, object> m_dicSelectedCopyRow = new Dictionary<string, object>();
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
            else if (Caller == "GetData")
            {
                IsCallerFromGetData = true;
                m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
                
                m_dicSelectedCopyRow = JSON.Deserialize<Dictionary<string, object>>(SelectedCopy);
                if (string.IsNullOrEmpty(m_dicSelectedCopyRow["CFP"+BudgetPlanVersionVM.Prop.BudgetPlanID.Name].ToString()))
                {
                    Global.ShowErrorAlert(title, BudgetPlanVersionVM.Prop.BudgetPlanID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
                    return this.Direct();
                }
            }

            string m_strMessage = string.Empty;
            BudgetPlanVersionEntryVM m_objBudgetPlanVersionEntryVM = new BudgetPlanVersionEntryVM();
            if (m_dicSelectedRow.Count > 0)
                m_objBudgetPlanVersionEntryVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage, m_dicSelectedCopyRow,IsCallerFromGetData);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            List<ConfigVM> m_uConfigVM = GetConfig("BudgetPlan", ItemGroupVM.Prop.ItemGroupID.Name, "Validate");
            string strItemGroupID = m_uConfigVM != null ? string.Join(",", m_uConfigVM.Select(d => d.Key4).ToList()) : string.Empty;
            ViewDataDictionary m_vddBudgetPlanVersionEntry = new ViewDataDictionary();
            m_vddBudgetPlanVersionEntry.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddBudgetPlanVersionEntry.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            m_vddBudgetPlanVersionEntry.Add(ItemGroupVM.Prop.ItemGroupID.Name, strItemGroupID);
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objBudgetPlanVersionEntryVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddBudgetPlanVersionEntry,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<BudgetPlanVersionEntryVM> m_lstSelectedRow = new List<BudgetPlanVersionEntryVM>();
            m_lstSelectedRow = JSON.Deserialize<List<BudgetPlanVersionEntryVM>>(Selected);

            DBudgetPlanVersionEntryDA m_objDBudgetPlanVersionEntryDA = new DBudgetPlanVersionEntryDA();
            m_objDBudgetPlanVersionEntryDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (BudgetPlanVersionEntryVM m_objBudgetPlanVersionEntryVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifBudgetPlanVersionEntryVM = m_objBudgetPlanVersionEntryVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifBudgetPlanVersionEntryVM in m_arrPifBudgetPlanVersionEntryVM)
                    {
                        string m_strFieldName = m_pifBudgetPlanVersionEntryVM.Name;
                        object m_objFieldValue = m_pifBudgetPlanVersionEntryVM.GetValue(m_objBudgetPlanVersionEntryVM);
                        if (m_objBudgetPlanVersionEntryVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objDBudgetPlanVersionEntryDA.DeleteBC(m_objFilter, false);
                    if (m_objDBudgetPlanVersionEntryDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDBudgetPlanVersionEntryDA.Message);
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
        public ActionResult ClearExcelFile(string filename)
        {
            string fullPath = Request.MapPath("~/Content/" + filename);
            //if (System.IO.File.Exists(fullPath))
            //    System.IO.File.Delete(fullPath);
            return this.Direct();
        }
        public ActionResult ExportExcelReturnDirect(string Selected, string GridStructure)
        {
            string m_strMessage = string.Empty;
            //Dictionary<string, object> m_dicBudgetPlan = new Dictionary<string, object>();
            //m_dicBudgetPlan.Add(BudgetPlanVM.Prop.BudgetPlanID.Name, BPID);
            BudgetPlanVM m_objBudgetPlanVM = JSON.Deserialize<BudgetPlanVM>(Selected);

            string m_strfilename = $"BUDGET PLAN - [{m_objBudgetPlanVM.Description}].xls";
            if (this.Request.Params["GridStructure"] != null)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("No.", typeof(string)));
                dt.Columns.Add(new DataColumn(BudgetPlanVersionStructureVM.Prop.ItemDesc.Desc, typeof(string)));
                dt.Columns.Add(new DataColumn(BudgetPlanVersionStructureVM.Prop.Specification.Desc, typeof(string)));
                dt.Columns.Add(new DataColumn(BudgetPlanVersionStructureVM.Prop.UoMDesc.Desc, typeof(string)));
                dt.Columns.Add(new DataColumn(BudgetPlanVersionStructureVM.Prop.Volume.Desc, typeof(string)));
                dt.Columns.Add(new DataColumn("MAT", typeof(string)));
                dt.Columns.Add(new DataColumn("WAG", typeof(string)));
                dt.Columns.Add(new DataColumn("MISC", typeof(string)));
                //dt.Columns.Add(new DataColumn(BudgetPlanVersionStructureVM.Prop.WageAmount.Desc, typeof(string)));
                //dt.Columns.Add(new DataColumn(BudgetPlanVersionStructureVM.Prop.MiscAmount.Desc, typeof(string)));
                dt.Columns.Add(new DataColumn(BudgetPlanVersionStructureVM.Prop.TotalUnitPrice.Desc, typeof(string)));
                dt.Columns.Add(new DataColumn(BudgetPlanVersionStructureVM.Prop.Total.Desc, typeof(string)));

                Dictionary<string, object>[] m_arrBudgetStructureChild = JSON.Deserialize<Dictionary<string, object>[]>(this.Request.Params["GridStructure"]);
                if (m_arrBudgetStructureChild == null)
                    m_arrBudgetStructureChild = new List<Dictionary<string, object>>().ToArray();

                foreach (Dictionary<string, object> m_dicBudgetVersionStructureVM in m_arrBudgetStructureChild)
                {
                    if (m_dicBudgetVersionStructureVM.ContainsKey("itemdesc"))
                    {
                        //m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.ItemDesc.Name.ToLower()].ToString();
                        string No = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.Sequence.Name.ToLower()] == null ? "" : m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.Sequence.Name.ToLower()].ToString();
                        string ItemDesc = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.ItemDesc.Name.ToLower()] == null ? "" : m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.ItemDesc.Name.ToLower()].ToString();
                        string Specification = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.Specification.Name.ToLower()] == null ? "" : m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.Specification.Name.ToLower()].ToString();
                        string UoM = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.UoMDesc.Name.ToLower()] == null ? "" : m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.UoMDesc.Name.ToLower()].ToString();
                        string Volume = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.Volume.Name.ToLower()] == null ? "" : string.IsNullOrEmpty(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.Volume.Name.ToLower()].ToString()) ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.Volume.Name.ToLower()]).ToString("#,#0.00#");
                        string MAG = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name.ToLower()] == null ? "" : string.IsNullOrEmpty(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name.ToLower()].ToString()) ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name.ToLower()]).ToString("#,#");
                        string WAG = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.WageAmount.Name.ToLower()] == null ? "" : string.IsNullOrEmpty(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.WageAmount.Name.ToLower()].ToString()) ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.WageAmount.Name.ToLower()]).ToString("#,#");
                        string MISC = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.MiscAmount.Name.ToLower()] == null ? "" : string.IsNullOrEmpty(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.MiscAmount.Name.ToLower()].ToString()) ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.MiscAmount.Name.ToLower()]).ToString("#,#");
                        string TotalUP = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.TotalUnitPrice.Name.ToLower()] == null ? "" : string.IsNullOrEmpty(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.TotalUnitPrice.Name.ToLower()].ToString()) ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.TotalUnitPrice.Name.ToLower()]).ToString("#,#");
                        string Total = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.Total.Name.ToLower()] == null ? "" : string.IsNullOrEmpty(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.Total.Name.ToLower()].ToString()) ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.Total.Name.ToLower()]).ToString("#,#");
                        //string itemtypeid = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.ItemTypeID.Name.ToLower()] == null ? "" : m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.ItemTypeID.Name.ToLower()].ToString();


                        if (No.Split('.').ToList().Count() == 1 && No != string.Empty && No != "1")
                        {
                            dt.Rows.Add("", "", "", "", "", "", "", "", "", "");
                        }
                        //if (itemtypeid == "BOI")
                        dt.Rows.Add(No, ItemDesc, Specification, UoM, Volume, MAG, WAG, MISC, TotalUP, Total);
                        //else
                        //  dt.Rows.Add(No, ItemDesc, Specification, UoM, "", "", "", "", "", "");
                    }
                    else
                        dt.Rows.Add("", "", "", "", "", "", "", "", "", "");
                }

                m_strfilename = Global.LoggedInFullName.ToUpperInvariant() + " - " + m_strfilename;
                ExportExcel(dt, m_strfilename);

            }
            return this.Direct(m_strfilename);
        }
        public void ExportExcel(DataTable sourceTable, string fileName)
        {
            HSSFWorkbook m_objWorkbook = new HSSFWorkbook();
            MemoryStream m_objmemoryStream = new MemoryStream();
            HSSFSheet m_objsheet = (HSSFSheet)m_objWorkbook.CreateSheet("BPL");
            HSSFRow m_objTitleRow = (HSSFRow)m_objsheet.CreateRow(0);
            HSSFRow m_objheaderRow = (HSSFRow)m_objsheet.CreateRow(1);

            var FontReg = m_objWorkbook.CreateFont();
            FontReg.FontHeightInPoints = 11;
            FontReg.FontName = "Calibri";
            FontReg.IsBold = false;

            var FontBold = m_objWorkbook.CreateFont();
            FontBold.FontHeightInPoints = 11;
            FontBold.FontName = "Calibri";
            FontBold.IsBold = true;

            #region Title
            m_objTitleRow.HeightInPoints = 26;

            var m_objTitleFont = m_objWorkbook.CreateFont();
            m_objTitleFont.FontHeightInPoints = 20;
            m_objTitleFont.FontName = "Calibri";
            m_objTitleFont.IsBold = true;

            HSSFCell m_objtitleCell = (HSSFCell)m_objTitleRow.CreateCell(0);
            m_objtitleCell.SetCellValue(fileName.Replace(".xls", ""));

            var mergedTitlecell = new NPOI.SS.Util.CellRangeAddress(m_objTitleRow.RowNum, m_objTitleRow.RowNum, 0, sourceTable.Columns.Count - 1);
            m_objsheet.AddMergedRegion(mergedTitlecell);

            HSSFCellStyle titleCellStyle = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            titleCellStyle.Alignment = HorizontalAlignment.Center;
            titleCellStyle.SetFont(m_objTitleFont);
            titleCellStyle.WrapText = true;
            titleCellStyle.VerticalAlignment = VerticalAlignment.Center;
            titleCellStyle.ShrinkToFit = true;
            m_objtitleCell.CellStyle = titleCellStyle;
            #endregion

            #region Header
            // Create Style
            HSSFCellStyle headerCellStyle = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            //headerCellStyle.FillForegroundColor = HSSFColor.Aqua.Index;
            headerCellStyle.FillPattern = FillPattern.SparseDots;
            headerCellStyle.VerticalAlignment = VerticalAlignment.Center;
            headerCellStyle.Alignment = HorizontalAlignment.Center;
            headerCellStyle.BorderBottom = BorderStyle.Thin;
            headerCellStyle.BorderTop = BorderStyle.Thin;
            headerCellStyle.BorderRight = BorderStyle.Thin;
            headerCellStyle.SetFont(FontBold);
            headerCellStyle.WrapText = true;

            // Handling header..
            int columnum = 0;
            foreach (DataColumn column in sourceTable.Columns)
            {
                // Create New Cell

                m_objheaderRow.Height = 700;
                HSSFCell headerCell = (HSSFCell)m_objheaderRow.CreateCell(column.Ordinal);
                // Set Cell Value
                if (columnum == 5)
                    headerCell.SetCellValue("Unit Price");
                else if (columnum == 6)
                    headerCell.SetCellValue("");
                else if (columnum == 7)
                    headerCell.SetCellValue("");
                else
                    headerCell.SetCellValue(column.ColumnName);

                headerCell.CellStyle = headerCellStyle;

                if (columnum == 5 || columnum == 6 || columnum == 7)
                {
                    if (columnum == 5)
                    {
                        var mergedcell = new NPOI.SS.Util.CellRangeAddress(m_objheaderRow.RowNum, m_objheaderRow.RowNum, columnum, (columnum + 2));
                        m_objsheet.AddMergedRegion(mergedcell);
                    }
                }
                else
                {
                    var mergedcell = new NPOI.SS.Util.CellRangeAddress(m_objheaderRow.RowNum,2, columnum, columnum);
                    m_objsheet.AddMergedRegion(mergedcell);
                }
                columnum++;
            }

            //Margin Row
            HSSFRow m_objdataRowMargin = (HSSFRow)m_objsheet.CreateRow(2);
            int NumberColumnMargin = 0;
            foreach (DataColumn column in sourceTable.Columns)
            {
                // Set Cell With Style
                if (column.Ordinal == 5)
                {
                    HSSFCell itemCell = (HSSFCell)m_objdataRowMargin.CreateCell(column.Ordinal);
                    itemCell.SetCellValue("Material");
                    itemCell.CellStyle = headerCellStyle;
                }
                else if (column.Ordinal == 6)
                {
                    HSSFCell itemCell = (HSSFCell)m_objdataRowMargin.CreateCell(column.Ordinal);
                    itemCell.SetCellValue("Wage");
                    itemCell.CellStyle = headerCellStyle;
                }
                else if (column.Ordinal == 7)
                {
                    HSSFCell itemCell = (HSSFCell)m_objdataRowMargin.CreateCell(column.Ordinal);
                    itemCell.SetCellValue("Other");
                    itemCell.CellStyle = headerCellStyle;
                }
                else
                {
                    HSSFCell itemCell = (HSSFCell)m_objdataRowMargin.CreateCell(column.Ordinal);
                    itemCell.SetCellValue("");
                    itemCell.CellStyle = headerCellStyle;
                }
                NumberColumnMargin++;
            }

            #endregion

            #region Data
            //Create Style for col 1
            HSSFCellStyle itemCellStyleCol1FontReg = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellStyleCol1FontReg.BorderBottom = BorderStyle.Thin;
            itemCellStyleCol1FontReg.BorderRight = BorderStyle.Thin;
            itemCellStyleCol1FontReg.WrapText = true;
            itemCellStyleCol1FontReg.VerticalAlignment = VerticalAlignment.Top;
            itemCellStyleCol1FontReg.Alignment = HorizontalAlignment.Right;
            itemCellStyleCol1FontReg.SetFont(FontReg);

            HSSFCellStyle itemCellStyleCol1FontBold = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellStyleCol1FontBold.BorderBottom = BorderStyle.Thin;
            itemCellStyleCol1FontBold.BorderRight = BorderStyle.Thin;
            itemCellStyleCol1FontBold.WrapText = true;
            itemCellStyleCol1FontBold.VerticalAlignment = VerticalAlignment.Top;
            itemCellStyleCol1FontBold.Alignment = HorizontalAlignment.Right;
            itemCellStyleCol1FontBold.SetFont(FontBold);

            //Create Style for col 2
            List<HSSFCellStyle> lst_indentStyle = new List<HSSFCellStyle>();

            HSSFCellStyle indentStyle = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            indentStyle.BorderBottom = BorderStyle.Thin;
            indentStyle.BorderRight = BorderStyle.Thin;
            indentStyle.WrapText = true;
            indentStyle.VerticalAlignment = VerticalAlignment.Top;
            indentStyle.Alignment = HorizontalAlignment.Left;
            indentStyle.SetFont(FontBold);
            lst_indentStyle.Add(indentStyle);

            //Create Style for col 3
            HSSFCellStyle itemCellStyleCol3Bold = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellStyleCol3Bold.BorderBottom = BorderStyle.Thin;
            itemCellStyleCol3Bold.BorderRight = BorderStyle.Thin;
            itemCellStyleCol3Bold.WrapText = true;
            itemCellStyleCol3Bold.VerticalAlignment = VerticalAlignment.Top;
            itemCellStyleCol3Bold.Alignment = HorizontalAlignment.Left;
            itemCellStyleCol3Bold.SetFont(FontBold);

            HSSFCellStyle itemCellStyleCol3 = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellStyleCol3.BorderBottom = BorderStyle.Thin;
            itemCellStyleCol3.BorderRight = BorderStyle.Thin;
            itemCellStyleCol3.WrapText = true;
            itemCellStyleCol3.VerticalAlignment = VerticalAlignment.Top;
            itemCellStyleCol3.Alignment = HorizontalAlignment.Left;
            itemCellStyleCol3.SetFont(FontReg);

            //Create Style for col 4
            HSSFCellStyle itemCellStyleCol4 = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellStyleCol4.BorderBottom = BorderStyle.Thin;
            itemCellStyleCol4.BorderRight = BorderStyle.Thin;
            itemCellStyleCol4.WrapText = true;
            itemCellStyleCol4.VerticalAlignment = VerticalAlignment.Top;
            itemCellStyleCol4.Alignment = HorizontalAlignment.Center;

            // Create Style for Default Value
            HSSFCellStyle itemCellStyleDefaultValue = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellStyleDefaultValue.BorderBottom = BorderStyle.Thin;
            itemCellStyleDefaultValue.BorderRight = BorderStyle.Thin;
            itemCellStyleDefaultValue.WrapText = true;
            itemCellStyleDefaultValue.VerticalAlignment = VerticalAlignment.Top;
            itemCellStyleDefaultValue.Alignment = HorizontalAlignment.Right;
            itemCellStyleDefaultValue.SetFont(FontReg);

            HSSFCellStyle itemCellStyleDefaultValueBold = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellStyleDefaultValueBold.BorderBottom = BorderStyle.Thin;
            itemCellStyleDefaultValueBold.BorderRight = BorderStyle.Thin;
            itemCellStyleDefaultValueBold.WrapText = true;
            itemCellStyleDefaultValueBold.VerticalAlignment = VerticalAlignment.Top;
            itemCellStyleDefaultValueBold.Alignment = HorizontalAlignment.Right;
            itemCellStyleDefaultValueBold.SetFont(FontBold);

            // Create Style for Summary
            HSSFCellStyle itemCellSrtyleSummary = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellSrtyleSummary.BorderBottom = BorderStyle.Thin;
            itemCellSrtyleSummary.BorderRight = BorderStyle.Thin;
            itemCellSrtyleSummary.WrapText = true;
            itemCellSrtyleSummary.VerticalAlignment = VerticalAlignment.Top;
            itemCellSrtyleSummary.Alignment = HorizontalAlignment.Left;
            itemCellSrtyleSummary.Indention = 2;
            itemCellSrtyleSummary.SetFont(FontBold);

            HSSFCellStyle itemCellSrtyleSummaryIndent = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellSrtyleSummaryIndent.BorderBottom = BorderStyle.Thin;
            itemCellSrtyleSummaryIndent.BorderRight = BorderStyle.Thin;
            itemCellSrtyleSummaryIndent.WrapText = true;
            itemCellSrtyleSummaryIndent.VerticalAlignment = VerticalAlignment.Top;
            itemCellSrtyleSummaryIndent.Alignment = HorizontalAlignment.Left;
            itemCellSrtyleSummaryIndent.Indention = 4;
            itemCellSrtyleSummaryIndent.SetFont(FontReg);

            HSSFCellStyle itemCellSrtyleSummaryIndentBold = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellSrtyleSummaryIndentBold.BorderBottom = BorderStyle.Thin;
            itemCellSrtyleSummaryIndentBold.BorderRight = BorderStyle.Thin;
            itemCellSrtyleSummaryIndentBold.WrapText = true;
            itemCellSrtyleSummaryIndentBold.VerticalAlignment = VerticalAlignment.Top;
            itemCellSrtyleSummaryIndentBold.Alignment = HorizontalAlignment.Left;
            itemCellSrtyleSummaryIndentBold.Indention = 4;
            itemCellSrtyleSummaryIndentBold.SetFont(FontBold);


            //Create Row Data
            int rowIndex = 3;
            foreach (DataRow row in sourceTable.Rows)
            {
                //Style Setting
                HSSFRow m_objdataRow = (HSSFRow)m_objsheet.CreateRow(rowIndex);
                int NumberColumn = 0;
                int indent = 0;
                bool isHiLevelBOI = false;
                bool firstCellisEmpty = false;
                bool isSummaryValueToBold = false;
                foreach (DataColumn column in sourceTable.Columns)
                {
                    // Set Cell With Style
                    if (NumberColumn == 0)
                    {
                        indent = row[column].ToString().Split('.').ToList().Count();
                        isHiLevelBOI = row[column].ToString() != string.Empty && indent < 2 ? true : false;
                        firstCellisEmpty = row[column].ToString() == string.Empty;
                        HSSFCell itemCell = (HSSFCell)m_objdataRow.CreateCell(column.Ordinal);
                        itemCell.SetCellValue(row[column].ToString());
                        if (indent == 1 && isHiLevelBOI == true)
                            itemCell.CellStyle = itemCellStyleCol1FontBold;
                        else
                            itemCell.CellStyle = itemCellStyleCol1FontReg;
                    }
                    else if (NumberColumn == 1)
                    {
                        HSSFCell itemCell = (HSSFCell)m_objdataRow.CreateCell(column.Ordinal);
                        itemCell.SetCellValue(row[column].ToString());
                        string containingValue = row[column].ToString().ToLower();
                        if (firstCellisEmpty)
                        {
                            if (containingValue.Contains("budget plan total"))
                            {
                                itemCell.SetCellValue("∑  Sub Total");
                                itemCell.CellStyle = itemCellSrtyleSummary;
                                isSummaryValueToBold = true;
                            }
                            else if (containingValue.Contains("pembulatan"))
                            {
                                itemCell.CellStyle = itemCellSrtyleSummaryIndentBold;
                                isSummaryValueToBold = true;
                            }
                            else
                                itemCell.CellStyle = itemCellSrtyleSummaryIndent;
                        }
                        else
                        {
                            if (indent > 1)
                            {
                                if (lst_indentStyle.Count() < indent)
                                {
                                    HSSFCellStyle indentStyleAdditional = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
                                    indentStyleAdditional.BorderBottom = BorderStyle.Thin;
                                    indentStyleAdditional.BorderRight = BorderStyle.Thin;
                                    indentStyleAdditional.WrapText = true;
                                    indentStyleAdditional.VerticalAlignment = VerticalAlignment.Top;
                                    indentStyleAdditional.Alignment = HorizontalAlignment.Left;
                                    indentStyleAdditional.Indention = (short)(indent - 1);
                                    lst_indentStyle.Add(indentStyleAdditional);
                                }
                            }
                            itemCell.CellStyle = lst_indentStyle[indent - 1];
                        }
                    }
                    else if (NumberColumn == 2)
                    {
                        HSSFCell itemCell = (HSSFCell)m_objdataRow.CreateCell(column.Ordinal);
                        itemCell.SetCellValue(row[column].ToString());
                        itemCell.CellStyle = indent == 1 && isHiLevelBOI == true ? itemCellStyleCol3Bold : itemCellStyleCol3;
                    }
                    else if (NumberColumn == 3)
                    {
                        HSSFCell itemCell = (HSSFCell)m_objdataRow.CreateCell(column.Ordinal);
                        itemCell.SetCellValue(row[column].ToString());
                        itemCell.CellStyle = itemCellStyleCol4;
                    }
                    else
                    {
                        HSSFCell itemCell = (HSSFCell)m_objdataRow.CreateCell(column.Ordinal);
                        itemCell.SetCellValue(row[column].ToString());
                        var builtinFormatId = HSSFDataFormat.GetBuiltinFormat("#,##0.00");
                        itemCell.CellStyle = indent == 1 && (isHiLevelBOI == true || isSummaryValueToBold == true) ? itemCellStyleDefaultValueBold : itemCellStyleDefaultValue;
                    }
                    NumberColumn++;
                }
                rowIndex++;
            }
            #endregion

            m_objsheet.AutoSizeColumn(1);
            m_objsheet.AutoSizeColumn(3);
            m_objsheet.AutoSizeColumn(5);
            m_objsheet.AutoSizeColumn(6);
            m_objsheet.AutoSizeColumn(7);
            m_objsheet.AutoSizeColumn(8);
            m_objsheet.AutoSizeColumn(9);
            m_objsheet.SetColumnWidth(2, 10000);

            m_objWorkbook.Write(m_objmemoryStream);
            DirectoryInfo di = new DirectoryInfo(Request.MapPath("~/Content/"));
            foreach (FileInfo filez in di.GetFiles())
            {
                string ext = Path.GetExtension(filez.Extension);
                if (ext == ".xls")
                    filez.Delete();
            }
            //Save to server before download
            FileStream file = new FileStream(Server.MapPath("~/Content/" + fileName), FileMode.Create, FileAccess.Write);
            m_objmemoryStream.WriteTo(file);
            file.Close();
            m_objmemoryStream.Close();
            m_objmemoryStream.Flush();
        }


        public ActionResult Browse(string ControlBudgetPlanVersionEntryID, string ControlBudgetPlanVersionEntryDesc, string FilterBudgetPlanVersionEntryID = "", string FilterBudgetPlanVersionEntryDesc = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddBudgetPlanVersionEntry = new ViewDataDictionary();
            m_vddBudgetPlanVersionEntry.Add("Control" + BudgetPlanVersionEntryVM.Prop.BudgetPlanID.Name, ControlBudgetPlanVersionEntryID);
            m_vddBudgetPlanVersionEntry.Add("Control" + BudgetPlanVersionEntryVM.Prop.BudgetPlanVersion.Name, ControlBudgetPlanVersionEntryDesc);
            m_vddBudgetPlanVersionEntry.Add("Control" + BudgetPlanVersionEntryVM.Prop.VendorID.Name, ControlBudgetPlanVersionEntryDesc);
            m_vddBudgetPlanVersionEntry.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanID.Name, FilterBudgetPlanVersionEntryID);
            m_vddBudgetPlanVersionEntry.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersion.Name, FilterBudgetPlanVersionEntryDesc);
            m_vddBudgetPlanVersionEntry.Add(BudgetPlanVersionEntryVM.Prop.VendorID.Name, FilterBudgetPlanVersionEntryDesc);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddBudgetPlanVersionEntry,
                ViewName = "../BudgetPlanVersionEntry/_Browse"
            };
        }

        public ActionResult Save(string Action)
        {
            //action = JSON.Deserialize<string>(action);
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            DBudgetPlanVersionEntryDA m_objDBudgetPlanVersionEntryDA = new DBudgetPlanVersionEntryDA();
            m_objDBudgetPlanVersionEntryDA.ConnectionStringName = Global.ConnStrConfigName;

            string m_strVendorID = GetVendorID();

            string m_strTransactionName = "BudgetPlanEntry_Trans";
            object m_objDBConnection = m_objDBudgetPlanVersionEntryDA.BeginTrans(m_strTransactionName);
            try
            {
                string m_strBudgetPlanID = this.Request.Params[BudgetPlanVersionEntryVM.Prop.BudgetPlanID.Name];
                string m_strBudgetPlanVersionVendorID = this.Request.Params[BudgetPlanVersionEntryVM.Prop.BudgetPlanVersionVendorID.Name];
                decimal m_decFeePercentage = this.Request.Params[BudgetPlanVersionEntryVM.Prop.FeePercentage.Name].ToString().Length == 0 ? 0 : decimal.Parse(this.Request.Params[BudgetPlanVersionEntryVM.Prop.FeePercentage.Name].ToString());
                string m_strBudgetPlanVersionPeriodID = this.Request.Params[BudgetPlanVersionEntryVM.Prop.BudgetPlanVersionPeriodID.Name];
                int m_intBudgetPlanVersion = int.Parse(this.Request.Params[BudgetPlanVersionEntryVM.Prop.BudgetPlanVersion.Name]);

                Dictionary<string, object>[] m_arrSctructure = JSON.Deserialize<Dictionary<string, object>[]>(this.Request.Params["treeStructure"]);
                Dictionary<string, object>[] m_arrAdditional = JSON.Deserialize<Dictionary<string, object>[]>(this.Request.Params["treeAdditional"]);

                m_lstMessage = IsSaveValid(Action, m_strBudgetPlanID, m_intBudgetPlanVersion, m_strBudgetPlanVersionPeriodID, m_strBudgetPlanVersionVendorID);
                if (m_lstMessage.Count <= 0)
                {
                    Dictionary<string, List<object>> m_objFilterChild = new Dictionary<string, List<object>>();
                    m_objFilterChild.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersionVendorID.Name, new List<object> { Operator.Equals, m_strBudgetPlanVersionVendorID });
                    m_objDBudgetPlanVersionEntryDA.DeleteBC(m_objFilterChild, true, m_objDBConnection);

                    if (m_objDBudgetPlanVersionEntryDA.Success)
                    {
                        List<DBudgetPlanVersionEntry> m_lstDBudgetPlanVersionEntry = new List<DBudgetPlanVersionEntry>();

                        m_lstDBudgetPlanVersionEntry = (
                            from Dictionary<string, object> m_dicBudgetPlanVersionEntry in m_arrSctructure
                            select new DBudgetPlanVersionEntry()
                            {
                                BudgetPlanVersionVendorID = m_strBudgetPlanVersionVendorID,
                                BudgetPlanVersionStructureID = m_dicBudgetPlanVersionEntry[BudgetPlanVersionEntryVM.Prop.BudgetPlanVersionStructureID.Name.ToLower()].ToString(),
                                Info = m_dicBudgetPlanVersionEntry[BudgetPlanVersionEntryVM.Prop.Info.Name.ToLower()].ToString(),
                                MaterialAmount = decimal.Parse(m_dicBudgetPlanVersionEntry[BudgetPlanVersionEntryVM.Prop.MaterialAmount.Name.ToLower()] != null ? m_dicBudgetPlanVersionEntry[BudgetPlanVersionEntryVM.Prop.MaterialAmount.Name.ToLower()].ToString() : "0"),
                                MiscAmount = decimal.Parse(m_dicBudgetPlanVersionEntry[BudgetPlanVersionEntryVM.Prop.MiscAmount.Name.ToLower()] != null ? m_dicBudgetPlanVersionEntry[BudgetPlanVersionEntryVM.Prop.MiscAmount.Name.ToLower()].ToString() : "0"),
                                //VendorID = GetVendorID(),
                                Volume = decimal.Parse(m_dicBudgetPlanVersionEntry[BudgetPlanVersionEntryVM.Prop.Volume.Name.ToLower()] != null ? m_dicBudgetPlanVersionEntry[BudgetPlanVersionEntryVM.Prop.Volume.Name.ToLower()].ToString() : "0"),
                                WageAmount = decimal.Parse(m_dicBudgetPlanVersionEntry[BudgetPlanVersionEntryVM.Prop.WageAmount.Name.ToLower()] != null ? m_dicBudgetPlanVersionEntry[BudgetPlanVersionEntryVM.Prop.WageAmount.Name.ToLower()].ToString() : "0")
                            }).ToList();

                        foreach (DBudgetPlanVersionEntry item in m_lstDBudgetPlanVersionEntry)
                        {
                            m_objDBudgetPlanVersionEntryDA.Data = item;
                            m_objDBudgetPlanVersionEntryDA.Insert(true, m_objDBConnection);
                            if (!m_objDBudgetPlanVersionEntryDA.Success)
                            {
                                m_lstMessage.Add(m_objDBudgetPlanVersionEntryDA.Message);
                                break;
                            }
                        }

                        if (m_objDBudgetPlanVersionEntryDA.Success)
                        {
                            DBudgetPlanVersionAdditionalDA m_objDBudgetPlanVersionAdditionalDA = new DBudgetPlanVersionAdditionalDA();
                            m_objFilterChild = new Dictionary<string, List<object>>();
                            m_objFilterChild.Add(BudgetPlanVersionAdditionalVM.Prop.BudgetPlanVersionVendorID.Name, new List<object> { Operator.Equals, m_strBudgetPlanVersionVendorID });
                            m_objDBudgetPlanVersionAdditionalDA.DeleteBC(m_objFilterChild, true, m_objDBConnection);

                            if (m_objDBudgetPlanVersionAdditionalDA.Success)
                            {
                                List<DBudgetPlanVersionAdditional> m_lstDBudgetPlanVersionAdditional = new List<DBudgetPlanVersionAdditional>();

                                m_lstDBudgetPlanVersionAdditional = (
                                    from Dictionary<string, object> m_dicBudgetPlanVersionAdditional in m_arrAdditional
                                    select new DBudgetPlanVersionAdditional()
                                    {
                                        Info = m_dicBudgetPlanVersionAdditional[BudgetPlanVersionAdditionalVM.Prop.Info.Name.ToLower()].ToString(),
                                        BudgetPlanVersionVendorID = m_strBudgetPlanVersionVendorID,
                                        Volume = m_dicBudgetPlanVersionAdditional[BudgetPlanVersionAdditionalVM.Prop.Volume.Name.ToLower()] == null ? 0 : decimal.Parse(m_dicBudgetPlanVersionAdditional[BudgetPlanVersionAdditionalVM.Prop.Volume.Name.ToLower()].ToString()),
                                        ItemID = m_dicBudgetPlanVersionAdditional[BudgetPlanVersionAdditionalVM.Prop.ItemID.Name.ToLower()].ToString(),
                                        Version = int.Parse(m_dicBudgetPlanVersionAdditional[BudgetPlanVersionAdditionalVM.Prop.Version.Name.ToLower()].ToString()),
                                        Sequence = int.Parse(m_dicBudgetPlanVersionAdditional[BudgetPlanVersionAdditionalVM.Prop.Sequence.Name.ToLower()].ToString()),
                                        ParentItemID = m_dicBudgetPlanVersionAdditional[BudgetPlanVersionAdditionalVM.Prop.ParentItemID.Name.ToLower()].ToString(),
                                        ParentVersion = int.Parse(m_dicBudgetPlanVersionAdditional[BudgetPlanVersionAdditionalVM.Prop.ParentVersion.Name.ToLower()].ToString()),
                                        ParentSequence = int.Parse(m_dicBudgetPlanVersionAdditional[BudgetPlanVersionAdditionalVM.Prop.ParentSequence.Name.ToLower()].ToString())
                                    }).ToList();

                                foreach (DBudgetPlanVersionAdditional item in m_lstDBudgetPlanVersionAdditional)
                                {
                                    m_objDBudgetPlanVersionAdditionalDA.Data = item;
                                    m_objDBudgetPlanVersionAdditionalDA.Data.BudgetPlanVersionAdditionalID = Guid.NewGuid().ToString().Replace("-", "");
                                    m_objDBudgetPlanVersionAdditionalDA.Insert(true, m_objDBConnection);
                                    if (!m_objDBudgetPlanVersionAdditionalDA.Success)
                                    {
                                        m_lstMessage.Add(m_objDBudgetPlanVersionAdditionalDA.Message);
                                        break;
                                    }
                                }

                                if (m_objDBudgetPlanVersionAdditionalDA.Success)
                                {
                                    DBudgetPlanVersionVendorDA m_objDBudgetPlanVersionVendorDA = new DBudgetPlanVersionVendorDA();
                                    m_objDBudgetPlanVersionVendorDA.ConnectionStringName = Global.ConnStrConfigName;

                                    m_objDBudgetPlanVersionVendorDA.Data.BudgetPlanVersionVendorID = m_strBudgetPlanVersionVendorID;
                                    m_objDBudgetPlanVersionVendorDA.Select();

                                    m_objDBudgetPlanVersionVendorDA.Data.FeePercentage = m_decFeePercentage;
                                    m_objDBudgetPlanVersionVendorDA.Update(true, m_objDBConnection);
                                    if (!m_objDBudgetPlanVersionVendorDA.Success)
                                    {
                                        m_lstMessage.Add(m_objDBudgetPlanVersionVendorDA.Message);
                                    }
                                }
                            }
                            else
                            {
                                m_lstMessage.Add(m_objDBudgetPlanVersionAdditionalDA.Message);
                            }
                        }

                    }
                    else
                    {
                        m_lstMessage.Add(m_objDBudgetPlanVersionEntryDA.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
            }
            if (m_lstMessage.Count <= 0)
            {
                m_objDBudgetPlanVersionEntryDA.EndTrans(ref m_objDBConnection, true, m_strTransactionName);
                Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
                return Detail(General.EnumDesc(Buttons.ButtonSave), null);
            }
            m_objDBudgetPlanVersionEntryDA.EndTrans(ref m_objDBConnection, false, m_strTransactionName);
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);
        }

        public ActionResult Verify(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Verify)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<BudgetPlanVersionVendorVM> m_lstSelectedRow = new List<BudgetPlanVersionVendorVM>();
            m_lstSelectedRow = JSON.Deserialize<List<BudgetPlanVersionVendorVM>>(Selected);
            List<string> m_lstMessage = new List<string>();

            DBudgetPlanVersionVendorDA m_objDBudgetPlanVersionVendorDA = new DBudgetPlanVersionVendorDA();
            m_objDBudgetPlanVersionVendorDA.ConnectionStringName = Global.ConnStrConfigName;
            string m_strTransactionName = "TransactionVerifyEntry";
            object m_objConnection = m_objDBudgetPlanVersionVendorDA.BeginTrans(m_strTransactionName);

            string m_strVendorID = GetVendorID();

            try
            {
                List<string> m_lstBudgetPlanVersionPeriodClosed = new List<string>();
                List<string> m_lstBudgetPlanIDInvalidStatusID = new List<string>();
                List<string> m_lstBudgetPlanIDNotSubmitedYet = new List<string>();

                foreach (BudgetPlanVersionVendorVM item in m_lstSelectedRow)
                {
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    List<object> m_lstFilter = new List<object>();
                    List<string> m_lstSelect = new List<string>();

                    #region Checking Status Vendor == 0

                    if (!IsBudgetPlanVersionPeriodIDOpen(item.BudgetPlanVersionPeriodID, m_strVendorID,
                        item.BudgetPlanVersionVendorID))
                    {
                        m_lstBudgetPlanVersionPeriodClosed.Add(item.BudgetPlanID);
                    }
                    else
                    {
                        if (!IsBudgetPlanVersionStatusIDValid(item.BudgetPlanID, item.BudgetPlanVersion))
                        {
                            m_lstBudgetPlanIDInvalidStatusID.Add(item.BudgetPlanID);
                        }
                        else
                        {

                            m_objDBudgetPlanVersionVendorDA.Data.BudgetPlanVersionVendorID = item.BudgetPlanVersionVendorID;
                            m_objDBudgetPlanVersionVendorDA.Select();

                            //if (!(m_objDBudgetPlanVersionVendorDA.Data.FeePercentage > 0))
                            //{
                            //    m_lstMessage.Add("Fee Percentage " + MessageLib.invalid + " [" + item.BudgetPlanID + "]");
                            //}
                            //else
                            //{

                            #region Checking StructureID > 0

                            BudgetPlanVersionEntryVM m_objBudgetPlanVersionEntryVM = new BudgetPlanVersionEntryVM();
                                TBudgetPlanDA m_objTBudgetPlanDA = new TBudgetPlanDA();
                                m_objTBudgetPlanDA.ConnectionStringName = Global.ConnStrConfigName;

                                m_lstSelect = new List<string>();
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

                                m_objFilter = new Dictionary<string, List<object>>();
                                m_lstFilter = new List<object>();
                                m_lstFilter.Add(Operator.Equals);
                                m_lstFilter.Add(item.BudgetPlanID);
                                m_objFilter.Add(BudgetPlanVM.Prop.BudgetPlanID.Map, m_lstFilter);

                                Dictionary<int, DataSet> m_dicTBudgetPlanDA = m_objTBudgetPlanDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
                                if (m_objTBudgetPlanDA.Message == string.Empty)
                                {
                                    DataRow m_drTBudgetPlanDA = m_dicTBudgetPlanDA[0].Tables[0].Rows[0];
                                    m_objBudgetPlanVersionEntryVM.BudgetPlanID = m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanID.Name].ToString();
                                    m_objBudgetPlanVersionEntryVM.Description = m_drTBudgetPlanDA[BudgetPlanVM.Prop.Description.Name].ToString();
                                    m_objBudgetPlanVersionEntryVM.BudgetPlanTemplateID = m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanTemplateID.Name].ToString();
                                    m_objBudgetPlanVersionEntryVM.BudgetPlanTemplateDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanTemplateDesc.Name].ToString();
                                    m_objBudgetPlanVersionEntryVM.BudgetPlanTypeDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanTypeDesc.Name].ToString();
                                    m_objBudgetPlanVersionEntryVM.CompanyDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.CompanyDesc.Name].ToString();
                                    m_objBudgetPlanVersionEntryVM.RegionDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.RegionDesc.Name].ToString();
                                    m_objBudgetPlanVersionEntryVM.LocationDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.LocationDesc.Name].ToString();
                                    m_objBudgetPlanVersionEntryVM.DivisionDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.DivisionDesc.Name].ToString();
                                    m_objBudgetPlanVersionEntryVM.ProjectID = m_drTBudgetPlanDA[BudgetPlanVM.Prop.ProjectID.Name].ToString();
                                    m_objBudgetPlanVersionEntryVM.ProjectDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.ProjectDesc.Name].ToString();
                                    m_objBudgetPlanVersionEntryVM.ClusterID = m_drTBudgetPlanDA[BudgetPlanVM.Prop.ClusterID.Name].ToString();
                                    m_objBudgetPlanVersionEntryVM.ClusterDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.ClusterDesc.Name].ToString();
                                    m_objBudgetPlanVersionEntryVM.UnitTypeID = m_drTBudgetPlanDA[BudgetPlanVM.Prop.UnitTypeID.Name].ToString();
                                    m_objBudgetPlanVersionEntryVM.UnitTypeDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.UnitTypeDesc.Name].ToString();
                                    m_objBudgetPlanVersionEntryVM.Area = decimal.Parse(m_drTBudgetPlanDA[BudgetPlanVM.Prop.Area.Name].ToString());
                                    m_objBudgetPlanVersionEntryVM.StatusDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.StatusDesc.Name].ToString();
                                    m_objBudgetPlanVersionEntryVM.VendorID = m_strVendorID;
                                    m_objBudgetPlanVersionEntryVM.BudgetPlanVersionVendorID = item.BudgetPlanVersionVendorID;
                                    m_objBudgetPlanVersionEntryVM.BudgetPlanVersion = item.BudgetPlanVersion;

                                    Node m_node = new Node() { NodeID = "root", Expanded = true };
                                    m_node.Children.AddRange(CheckMyChild(m_objBudgetPlanVersionEntryVM.BudgetPlanID,
                                        m_objBudgetPlanVersionEntryVM.BudgetPlanVersion,
                                        m_objBudgetPlanVersionEntryVM.FilterItemTypeID,
                                        item.BudgetPlanVersionVendorID, ref m_lstBudgetPlanIDNotSubmitedYet, m_strVendorID));

                                }

                                else
                                    m_lstMessage.Add("[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objTBudgetPlanDA.Message);

                                #endregion

                            //}
                        }
                    }

                    #endregion

                    if (m_lstBudgetPlanVersionPeriodClosed.Count == 0 && m_lstBudgetPlanIDInvalidStatusID.Count == 0 &&
                        m_lstBudgetPlanIDNotSubmitedYet.Count == 0 && m_lstMessage.Count == 0)
                    {
                        m_objDBudgetPlanVersionVendorDA.Data.BudgetPlanVersionVendorID = item.BudgetPlanVersionVendorID;
                        m_objDBudgetPlanVersionVendorDA.Select();

                        m_objDBudgetPlanVersionVendorDA.Data.StatusID = 1;
                        m_objDBudgetPlanVersionVendorDA.Update(true, m_objConnection);
                        if (!m_objDBudgetPlanVersionVendorDA.Success)
                        {
                            m_lstMessage.Add(m_objDBudgetPlanVersionVendorDA.Message);
                            break;
                        }
                    }
                }

                if (m_lstBudgetPlanIDInvalidStatusID.Count > 0)
                {
                    m_lstMessage.Add("Budget Plan Entry Status " + MessageLib.invalid + " [" + String.Join(Global.OneLineSeparated, m_lstBudgetPlanIDInvalidStatusID) + "]");
                }

                if (m_lstBudgetPlanIDNotSubmitedYet.Count > 0)
                {
                    m_lstMessage.Add("Budget Plan Entry is not yet completed");
                    //m_lstMessage.Add("Budget Plan Entry is not entry yet [" + String.Join(Global.OneLineSeparated, m_lstBudgetPlanIDNotSubmitedYet) + "]");
                }

                if (m_lstBudgetPlanVersionPeriodClosed.Count > 0)
                {
                    m_lstMessage.Add("Budget Plan Entry period is closed [" + String.Join(Global.OneLineSeparated, m_lstBudgetPlanVersionPeriodClosed) + "]");
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                return this.Direct(false, ex.Message);
            }

            if (m_lstMessage.Count <= 0)
            {
                m_objDBudgetPlanVersionVendorDA.EndTrans(ref m_objConnection, true, m_strTransactionName);
                Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Verified));
                return this.Direct(true, string.Empty);
            }
            else
            {
                m_objDBudgetPlanVersionVendorDA.EndTrans(ref m_objConnection, false, m_strTransactionName);
                return this.Direct(false, String.Join(Global.NewLineSeparated, m_lstMessage));
            }

        }

        public ActionResult Unverify(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Unverify)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<BudgetPlanVersionVendorVM> m_lstSelectedRow = new List<BudgetPlanVersionVendorVM>();
            m_lstSelectedRow = JSON.Deserialize<List<BudgetPlanVersionVendorVM>>(Selected);

            DBudgetPlanVersionVendorDA m_objDBudgetPlanVersionVendorDA = new DBudgetPlanVersionVendorDA();
            m_objDBudgetPlanVersionVendorDA.ConnectionStringName = Global.ConnStrConfigName;
            string m_strTransactionName = "TransactionUnverifyEntry";
            object m_objConnection = m_objDBudgetPlanVersionVendorDA.BeginTrans(m_strTransactionName);

            List<string> m_lstMessage = new List<string>();
            List<string> m_lstMessageStatusBudgetPlanVersionInvalid = new List<string>();
            List<string> m_lstMessageNewBudgetPlanVersionCreated = new List<string>();
            List<string> m_lstMessageBudgetPlanEntryNotSubmited = new List<string>();
            List<string> m_lstMessageBudgetPlanEntryHasOpened = new List<string>();

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();

            try
            {
                foreach (BudgetPlanVersionVendorVM item in m_lstSelectedRow)
                {
                    int m_intBudgetPlanVersion = item.BudgetPlanVersion;
                    string m_strBudgetPlanID = item.BudgetPlanID;

                    if (!IsBudgetPlanVersionStatusIDValid(item.BudgetPlanID, item.BudgetPlanVersion))
                    {
                        m_lstMessageStatusBudgetPlanVersionInvalid.Add(item.BudgetPlanID);
                    }
                    else
                    {
                        if (!IsBudgetPlanVersionValid(item.BudgetPlanID, item.BudgetPlanVersion))
                        {
                            m_lstMessageNewBudgetPlanVersionCreated.Add(item.BudgetPlanID);
                        }
                        else
                        {
                            m_objFilter = new Dictionary<string, List<object>>();

                            m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(item.BudgetPlanVersionVendorID);
                            m_objFilter.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionVendorID.Map, m_lstFilter);

                            m_lstSelect = new List<string>();
                            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.StatusIDVendor.MapAlias);

                            BudgetPlanVersionVendorVM m_objBudgetPlanVersionVendorVM = new BudgetPlanVersionVendorVM();

                            Dictionary<int, DataSet> m_dicDBudgetPlanVersionVendorDA = m_objDBudgetPlanVersionVendorDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);
                            if (m_objDBudgetPlanVersionVendorDA.Success && m_objDBudgetPlanVersionVendorDA.Message == string.Empty)
                            {
                                m_objBudgetPlanVersionVendorVM.StatusID = int.Parse(m_dicDBudgetPlanVersionVendorDA[0].Tables[0].Rows[0][BudgetPlanVersionVendorVM.Prop.StatusIDVendor.Name].ToString());
                            }

                            if (m_objBudgetPlanVersionVendorVM.StatusID == 0)
                            {
                                m_lstMessageBudgetPlanEntryNotSubmited.Add(item.BudgetPlanID);
                            }
                            else if (m_objBudgetPlanVersionVendorVM.StatusID > 1)
                            {
                                m_lstMessageBudgetPlanEntryHasOpened.Add(item.BudgetPlanID);
                            }
                            else
                            {
                                #region Update StatusID DBudgetPlanVersionVendor
                                if (m_lstMessageBudgetPlanEntryHasOpened.Count == 0 &&
                                    m_lstMessageBudgetPlanEntryNotSubmited.Count == 0 &&
                                    m_lstMessageNewBudgetPlanVersionCreated.Count == 0 &&
                                    m_lstMessageStatusBudgetPlanVersionInvalid.Count == 0)
                                {
                                    m_objDBudgetPlanVersionVendorDA.Data.BudgetPlanVersionVendorID = item.BudgetPlanVersionVendorID;
                                    m_objDBudgetPlanVersionVendorDA.Select();

                                    m_objDBudgetPlanVersionVendorDA.Data.StatusID = 0;
                                    m_objDBudgetPlanVersionVendorDA.Update(true, m_objConnection);
                                    if (!m_objDBudgetPlanVersionVendorDA.Success)
                                    {
                                        m_lstMessage.Add(m_objDBudgetPlanVersionVendorDA.Message);
                                        break;
                                    }
                                    else
                                    {

                                    }
                                }
                                #endregion
                            }
                        }
                    }
                }

                if (m_lstMessageBudgetPlanEntryHasOpened.Count > 0)
                {
                    m_lstMessage.Add("Budget Plan Entry has been openned for comparison [" + String.Join(", ", m_lstMessageBudgetPlanEntryHasOpened) + "]");
                }

                if (m_lstMessageBudgetPlanEntryNotSubmited.Count > 0)
                {
                    m_lstMessage.Add("Budget Plan Entry is not submitted yet [" + String.Join(", ", m_lstMessageBudgetPlanEntryNotSubmited) + "]");
                }

                if (m_lstMessageNewBudgetPlanVersionCreated.Count > 0)
                {
                    m_lstMessage.Add("New Budget Plan version may has been created [" + String.Join(", ", m_lstMessageNewBudgetPlanVersionCreated) + "]");
                }

                if (m_lstMessageStatusBudgetPlanVersionInvalid.Count > 0)
                {
                    m_lstMessage.Add("Budget Plan Status is invalid [" + String.Join(", ", m_lstMessageStatusBudgetPlanVersionInvalid) + "]");
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                return this.Direct(false, ex.Message);
            }

            if (m_lstMessage.Count <= 0)
            {
                m_objDBudgetPlanVersionVendorDA.EndTrans(ref m_objConnection, true, m_strTransactionName);
                Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Unverified));
                return this.Direct(true, string.Empty);
            }
            else
            {
                m_objDBudgetPlanVersionVendorDA.EndTrans(ref m_objConnection, false, m_strTransactionName);
                return this.Direct(false, String.Join(Global.NewLineSeparated, m_lstMessage));
            }
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

        public ActionResult GetBudgetPlanVersionEntry(string ControlBudgetPlanVersionEntryID, string ControlBudgetPlanVersionEntryDesc, string FilterBudgetPlanVersionEntryID, string FilterBudgetPlanVersionEntryDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<BudgetPlanVersionEntryVM>> m_dicBudgetPlanVersionEntryData = GetBudgetPlanVersionEntryData(true, FilterBudgetPlanVersionEntryID, FilterBudgetPlanVersionEntryDesc);
                KeyValuePair<int, List<BudgetPlanVersionEntryVM>> m_kvpBudgetPlanVersionEntryVM = m_dicBudgetPlanVersionEntryData.AsEnumerable().ToList()[0];
                if (m_kvpBudgetPlanVersionEntryVM.Key < 1 || (m_kvpBudgetPlanVersionEntryVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpBudgetPlanVersionEntryVM.Key > 1 && !Exact)
                    return Browse(ControlBudgetPlanVersionEntryID, ControlBudgetPlanVersionEntryDesc, FilterBudgetPlanVersionEntryID, FilterBudgetPlanVersionEntryDesc);

                m_dicBudgetPlanVersionEntryData = GetBudgetPlanVersionEntryData(false, FilterBudgetPlanVersionEntryID, FilterBudgetPlanVersionEntryDesc);
                BudgetPlanVersionEntryVM m_objBudgetPlanVersionEntryVM = m_dicBudgetPlanVersionEntryData[0][0];
                this.GetCmp<TextField>(ControlBudgetPlanVersionEntryID).Value = m_objBudgetPlanVersionEntryVM.BudgetPlanID;
                this.GetCmp<TextField>(ControlBudgetPlanVersionEntryDesc).Value = m_objBudgetPlanVersionEntryVM.BudgetPlanTemplateDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, string BudgetPlanID, int BudgetPlanVersion,
            string BudgetPlanVersionPeriodID, string BudgetPlanVersionVendorID)
        {
            List<string> m_lstReturn = new List<string>();

            if (BudgetPlanID == string.Empty)
                m_lstReturn.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (BudgetPlanVersion == 0)
                m_lstReturn.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersion.Desc + " " + General.EnumDesc(MessageLib.NotExist));

            if (!IsBudgetPlanVersionStatusIDValid(BudgetPlanID, BudgetPlanVersion))
            {
                m_lstReturn.Add("Budget Plan Status " + General.EnumDesc(MessageLib.invalid));
            }

            if (!IsBudgetPlanVersionPeriodIDOpen(BudgetPlanVersionPeriodID, GetVendorID(), BudgetPlanVersionVendorID))
            {
                m_lstReturn.Add("Budget Plan Version Period Status id Closed");
            }

            if (!IsBudgetPlanVersionValid(BudgetPlanID, BudgetPlanVersion))
            {
                m_lstReturn.Add("New Budget Plan Version may has been created.");
            }

            return m_lstReturn;
        }

        private bool IsBudgetPlanVersionPeriodIDOpen(string BudgetPlanVersionPeriodID, string VendorID, string BudgetPlanVersionVendorID)
        {
            bool m_boolReturn = false;

            DBudgetPlanVersionVendorDA m_objDBudgetPlanVersionPeriodDA = new DBudgetPlanVersionVendorDA();
            m_objDBudgetPlanVersionPeriodDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionVendorID.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanVersionPeriodID);
            m_objFilter.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionPeriodID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanVersionVendorID);
            m_objFilter.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionVendorID.Map, m_lstFilter);

            //m_lstFilter = new List<object>();
            //m_lstFilter.Add(Operator.Equals);
            //m_lstFilter.Add(VendorID);
            //m_objFilter.Add(BudgetPlanVersionVendorVM.Prop.VendorID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(0);
            m_objFilter.Add(BudgetPlanVersionVendorVM.Prop.StatusIDPeriod.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.None);
            m_objFilter.Add("'" + DateTime.Now.ToString(Global.SqlDateFormat) + "' BETWEEN " + BudgetPlanVersionVendorVM.Prop.StartDate.Map
                + " AND " + BudgetPlanVersionVendorVM.Prop.EndDate.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicDBudgetPlanVersionVendorDA = m_objDBudgetPlanVersionPeriodDA.SelectBC(0, null, true, m_lstSelect, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpBudgetPlanVersionVendorBL in m_dicDBudgetPlanVersionVendorDA)
            {
                m_intCount = m_kvpBudgetPlanVersionVendorBL.Key;
                break;
            }

            m_boolReturn = m_intCount == 1;

            return m_boolReturn;
        }

        private bool IsBudgetPlanVersionStatusIDValid(string BudgetPlanID, int BudgetPlanVersion)
        {
            bool m_boolReturn = false;

            DBudgetPlanVersionDA m_objDBudgetPlanVersionDA = new DBudgetPlanVersionDA();
            m_objDBudgetPlanVersionDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVersionVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVM.Prop.BudgetPlanVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVM.Prop.StatusID.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanID);
            m_objFilter.Add(BudgetPlanVersionVM.Prop.BudgetPlanID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanVersion);
            m_objFilter.Add(BudgetPlanVersionVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(2);
            m_objFilter.Add(BudgetPlanVersionVendorVM.Prop.StatusID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicDBudgetPlanVersionVendorDA = m_objDBudgetPlanVersionDA.SelectBC(0, null, true, m_lstSelect, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpBudgetPlanVersionVendorBL in m_dicDBudgetPlanVersionVendorDA)
            {
                m_intCount = m_kvpBudgetPlanVersionVendorBL.Key;
                break;
            }

            m_boolReturn = m_intCount == 1;

            return m_boolReturn;
        }

        private bool IsBudgetPlanVersionValid(string BudgetPlanID, int BudgetPlanVersion)
        {
            bool m_boolReturn = false;

            DBudgetPlanVersionDA m_objDBudgetPlanVersionDA = new DBudgetPlanVersionDA();
            m_objDBudgetPlanVersionDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVersionVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVM.Prop.BudgetPlanVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVM.Prop.StatusID.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanID);
            m_objFilter.Add(BudgetPlanVersionVM.Prop.BudgetPlanID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.GreaterThanEqual);
            m_lstFilter.Add(BudgetPlanVersion);
            m_objFilter.Add(BudgetPlanVersionVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(2);
            m_objFilter.Add(BudgetPlanVersionVendorVM.Prop.StatusID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicDBudgetPlanVersionVendorDA = m_objDBudgetPlanVersionDA.SelectBC(0, null, true, m_lstSelect, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpBudgetPlanVersionVendorBL in m_dicDBudgetPlanVersionVendorDA)
            {
                m_intCount = m_kvpBudgetPlanVersionVendorBL.Key;
                break;
            }

            m_boolReturn = m_intCount == 1;

            return m_boolReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanID.Name, parameters[BudgetPlanVersionEntryVM.Prop.BudgetPlanID.Name]);
            m_dicReturn.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersion.Name, parameters[BudgetPlanVersionEntryVM.Prop.BudgetPlanVersion.Name]);
            m_dicReturn.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersionVendorID.Name, parameters[BudgetPlanVersionEntryVM.Prop.BudgetPlanVersionVendorID.Name]);
            m_dicReturn.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersionPeriodID.Name, parameters[BudgetPlanVersionEntryVM.Prop.BudgetPlanVersionPeriodID.Name]);
            m_dicReturn.Add(BudgetPlanVersionEntryVM.Prop.FeePercentage.Name, parameters[BudgetPlanVersionEntryVM.Prop.FeePercentage.Name]);
            return m_dicReturn;
        }

        private BudgetPlanVersionEntryVM GetSelectedData(Dictionary<string, object> selected, ref string message, Dictionary<string, object> selectedCopy, bool isCopyFromPrevious)
        {
            string m_strBudgetPlanID = selected[BudgetPlanVersionEntryVM.Prop.BudgetPlanID.Name].ToString();
            string m_strBudgetPlanVersionVendorID = selected[BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionVendorID.Name].ToString();
            string m_strBudgetPlanVersionPeriodID = selected[BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionPeriodID.Name].ToString();
            decimal? m_decFeePercentage = selected[BudgetPlanVersionVendorVM.Prop.FeePercentage.Name].ToString().Length == 0 ? null : (decimal?)decimal.Parse(selected[BudgetPlanVersionEntryVM.Prop.FeePercentage.Name].ToString());
            int m_intBudgetPlanVersion = int.Parse(selected[BudgetPlanVersionVendorVM.Prop.BudgetPlanVersion.Name].ToString());

            BudgetPlanVersionEntryVM m_budgetPlanVersionEntry = new BudgetPlanVersionEntryVM();
             if (isCopyFromPrevious)
            {
                string m_strCFPBudgetPlanID = selectedCopy["CFP" + BudgetPlanVersionEntryVM.Prop.BudgetPlanID.Name].ToString();
                string m_strCFPBudgetPlanVersionVendorID = selectedCopy["CFP" + BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionVendorID.Name].ToString();
                string m_strCFPBudgetPlanVersionPeriodID = selectedCopy["CFP" + BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionPeriodID.Name].ToString();
                decimal? m_decCFPFeePercentage = selectedCopy["CFP" + BudgetPlanVersionVendorVM.Prop.FeePercentage.Name].ToString().Length == 0 ? null : (decimal?)decimal.Parse(selectedCopy["CFP" + BudgetPlanVersionEntryVM.Prop.FeePercentage.Name].ToString());
                int m_intCFPBudgetPlanVersion = int.Parse(selectedCopy["CFP" + BudgetPlanVersionVendorVM.Prop.BudgetPlanVersion.Name].ToString());

                m_budgetPlanVersionEntry.BudgetPlanID = m_strCFPBudgetPlanID;
                m_budgetPlanVersionEntry.BudgetPlanVersion = (int)m_intCFPBudgetPlanVersion;
                m_budgetPlanVersionEntry.BudgetPlanVersionPeriodID = m_strCFPBudgetPlanVersionPeriodID;
                m_budgetPlanVersionEntry.BudgetPlanVersionVendorID = m_strCFPBudgetPlanVersionVendorID;
            }

            string m_strVendorID = GetVendorID();

            if (!IsBudgetPlanVersionValid(m_strBudgetPlanID, m_intBudgetPlanVersion))
            {
                message = "New Budget Plan Version may has been created [" + m_strBudgetPlanID + "]";
                return new BudgetPlanVersionEntryVM();
            }

            BudgetPlanVersionEntryVM m_objBudgetPlanVersionEntryVM = new BudgetPlanVersionEntryVM();
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
            m_lstSelect.Add(BudgetPlanVM.Prop.Unit.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.CreatedDate.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.ModifiedDate.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.StatusID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.StatusDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(m_strBudgetPlanID);
            m_objFilter.Add(BudgetPlanVM.Prop.BudgetPlanID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(m_intBudgetPlanVersion);
            m_objFilter.Add(BudgetPlanVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicTBudgetPlanDA = m_objTBudgetPlanDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objTBudgetPlanDA.Message == string.Empty)
            {
                DataRow m_drTBudgetPlanDA = m_dicTBudgetPlanDA[0].Tables[0].Rows[0];
                m_objBudgetPlanVersionEntryVM.BudgetPlanID = m_strBudgetPlanID;
                m_objBudgetPlanVersionEntryVM.Description = m_drTBudgetPlanDA[BudgetPlanVM.Prop.Description.Name].ToString();
                m_objBudgetPlanVersionEntryVM.BudgetPlanTemplateID = m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanTemplateID.Name].ToString();
                m_objBudgetPlanVersionEntryVM.BudgetPlanTemplateDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanTemplateDesc.Name].ToString();
                m_objBudgetPlanVersionEntryVM.BudgetPlanTypeDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanTypeDesc.Name].ToString();
                m_objBudgetPlanVersionEntryVM.CompanyDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.CompanyDesc.Name].ToString();
                m_objBudgetPlanVersionEntryVM.RegionDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.RegionDesc.Name].ToString();
                m_objBudgetPlanVersionEntryVM.LocationDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.LocationDesc.Name].ToString();
                m_objBudgetPlanVersionEntryVM.DivisionDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.DivisionDesc.Name].ToString();
                m_objBudgetPlanVersionEntryVM.ProjectID = m_drTBudgetPlanDA[BudgetPlanVM.Prop.ProjectID.Name].ToString();
                m_objBudgetPlanVersionEntryVM.ProjectDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.ProjectDesc.Name].ToString();
                m_objBudgetPlanVersionEntryVM.ClusterID = m_drTBudgetPlanDA[BudgetPlanVM.Prop.ClusterID.Name].ToString();
                m_objBudgetPlanVersionEntryVM.ClusterDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.ClusterDesc.Name].ToString();
                m_objBudgetPlanVersionEntryVM.UnitTypeID = m_drTBudgetPlanDA[BudgetPlanVM.Prop.UnitTypeID.Name].ToString();
                m_objBudgetPlanVersionEntryVM.UnitTypeDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.UnitTypeDesc.Name].ToString();
                m_objBudgetPlanVersionEntryVM.Area = decimal.Parse(m_drTBudgetPlanDA[BudgetPlanVM.Prop.Area.Name].ToString());
                m_objBudgetPlanVersionEntryVM.Unit = decimal.Parse(m_drTBudgetPlanDA[BudgetPlanVM.Prop.Unit.Name].ToString());
                m_objBudgetPlanVersionEntryVM.BudgetPlanVersion = m_intBudgetPlanVersion;
                m_objBudgetPlanVersionEntryVM.StatusDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.StatusDesc.Name].ToString();
                m_objBudgetPlanVersionEntryVM.VendorID = m_strVendorID;
                m_objBudgetPlanVersionEntryVM.BudgetPlanVersionVendorID = m_strBudgetPlanVersionVendorID;
                m_objBudgetPlanVersionEntryVM.BudgetPlanVersion = m_intBudgetPlanVersion;
                m_objBudgetPlanVersionEntryVM.FeePercentage = m_decFeePercentage;
                m_objBudgetPlanVersionEntryVM.BudgetPlanVersionPeriodID = m_strBudgetPlanVersionPeriodID;

                decimal? m_decBudgetPlanTotal = 0;

                Node m_node = new Node() { NodeID = "root", Expanded = true };
                NodeCollection m_ncStructure = LoadMyChild(m_objBudgetPlanVersionEntryVM.BudgetPlanID, m_objBudgetPlanVersionEntryVM.BudgetPlanVersion, m_objBudgetPlanVersionEntryVM.FilterItemTypeID, m_strBudgetPlanVersionVendorID, m_strVendorID,"0",0,0,"",isCopyFromPrevious, m_budgetPlanVersionEntry);

                foreach (Node itemChild in m_ncStructure)
                {
                    m_decBudgetPlanTotal += (decimal?)itemChild.AttributesObject.GetType().GetProperties()[13].GetValue(itemChild.AttributesObject, null) == null ? 0
                        : (decimal?)itemChild.AttributesObject.GetType().GetProperties()[13].GetValue(itemChild.AttributesObject, null);
                }

                #region Grand total
                m_ncStructure.Add(new Node() { NodeID = "######", Leaf = true, IconCls = "display: none !important;", Expandable = false });

                Node m_nodeGrandTotal = new Node();
                m_nodeGrandTotal.NodeID = BudgetPlanVM.Prop.Subtotal.Name;
                m_nodeGrandTotal.Expanded = true;
                m_nodeGrandTotal.Icon = Icon.Sum;
                m_nodeGrandTotal.AttributesObject = new
                {
                    sequence = "",
                    itemdesc = BudgetPlanVM.Prop.Subtotal.Desc,
                    budgetplanid = "",
                    budgetplanversion = "",
                    budgetplanversionstructureid = "",
                    specification = "",
                    info = "",
                    uomdesc = "",
                    volume = "",
                    materialamount = "",
                    wageamount = "",
                    miscamount = "",
                    totalunitprice = "",
                    total = m_decBudgetPlanTotal,
                    bold = true,
                    additional = true
                };
                m_nodeGrandTotal.Children.Add(new Node()
                {
                    NodeID = BudgetPlanVM.Prop.ContractorFee.Name,
                    Leaf = true,
                    AttributesObject = new
                    {
                        sequence = "",
                        itemdesc = BudgetPlanVM.Prop.ContractorFee.Desc,
                        budgetplanid = "",
                        budgetplanversion = "",
                        budgetplanversionstructureid = "",
                        specification = "",
                        info = "",
                        uomdesc = "",
                        volume = "",
                        materialamount = "",
                        wageamount = "",
                        miscamount = "",
                        totalunitprice = m_decFeePercentage,
                        total = m_decBudgetPlanTotal * m_decFeePercentage / 100,
                        bold = false,
                        additional = true
                    },
                    IconCls = "display: none !important;"
                });
                m_nodeGrandTotal.Children.Add(new Node()
                {
                    NodeID = BudgetPlanVM.Prop.Total.Desc,
                    Leaf = true,
                    AttributesObject = new
                    {
                        sequence = "",
                        itemdesc = BudgetPlanVM.Prop.Total.Desc,
                        budgetplanid = "",
                        budgetplanversion = "",
                        budgetplanversionstructureid = "",
                        specification = "",
                        info = "",
                        uomdesc = "",
                        volume = "",
                        materialamount = "",
                        wageamount = "",
                        miscamount = "",
                        totalunitprice = "",
                        total = m_decBudgetPlanTotal * m_decFeePercentage / 100 + m_decBudgetPlanTotal,
                        bold = false,
                        additional = true
                    },
                    IconCls = "display: none !important;"
                });

                decimal m_decRounding = Math.Round(((decimal)m_decBudgetPlanTotal * (decimal)m_decFeePercentage / (decimal)100 + (decimal)m_decBudgetPlanTotal) / 1000, 0) * 1000;
                m_nodeGrandTotal.Children.Add(new Node()
                {
                    NodeID = BudgetPlanVM.Prop.Rounding.Name,
                    Leaf = true,
                    AttributesObject = new
                    {
                        sequence = "",
                        itemdesc = BudgetPlanVM.Prop.Rounding.Desc,
                        budgetplanid = "",
                        budgetplanversion = "",
                        budgetplanversionstructureid = "",
                        specification = "",
                        info = "",
                        uomdesc = "",
                        volume = "",
                        materialamount = "",
                        wageamount = "",
                        miscamount = "",
                        totalunitprice = "",
                        total = m_decRounding,
                        bold = false,
                        additional = true
                    },
                    IconCls = "display: none !important;"
                });

                m_nodeGrandTotal.Children.Add(new Node()
                {
                    NodeID = BudgetPlanVM.Prop.GrandTotalExcPPN.Name,
                    Leaf = true,
                    AttributesObject = new
                    {
                        sequence = "",
                        itemdesc = BudgetPlanVM.Prop.GrandTotalExcPPN.Desc,
                        budgetplanid = "",
                        budgetplanversion = "",
                        budgetplanversionstructureid = "",
                        specification = "",
                        info = "",
                        uomdesc = "",
                        volume = "",
                        materialamount = "",
                        wageamount = "",
                        miscamount = "",
                        totalunitprice = "",
                        total = m_decRounding * m_objBudgetPlanVersionEntryVM.Unit ,
                        bold = false,
                        additional = true
                    },
                    IconCls = "display: none !important;"
                });
                decimal m_decTax = m_decRounding * m_objBudgetPlanVersionEntryVM.Unit *(decimal)0.1;
                m_nodeGrandTotal.Children.Add(new Node()
                {
                    NodeID = BudgetPlanVM.Prop.Tax.Name,
                    Leaf = true,
                    AttributesObject = new
                    {
                        sequence = "",
                        itemdesc = BudgetPlanVM.Prop.Tax.Desc,
                        budgetplanid = "",
                        budgetplanversion = "",
                        budgetplanversionstructureid = "",
                        specification = "",
                        info = "",
                        uomdesc = "",
                        volume = "",
                        materialamount = "",
                        wageamount = "",
                        miscamount = "",
                        totalunitprice = "",
                        total = m_decTax,
                        bold = false,
                        additional = true
                    },
                    IconCls = "display: none !important;"
                });
                m_nodeGrandTotal.Children.Add(new Node()
                {
                    NodeID = BudgetPlanVM.Prop.GrandTotalIncPPN.Name,
                    Leaf = true,
                    AttributesObject = new
                    {
                        sequence = "",
                        itemdesc = BudgetPlanVM.Prop.GrandTotalIncPPN.Desc,
                        budgetplanid = "",
                        budgetplanversion = "",
                        budgetplanversionstructureid = "",
                        specification = "",
                        info = "",
                        uomdesc = "",
                        volume = "",
                        materialamount = "",
                        wageamount = "",
                        miscamount = "",
                        totalunitprice = "",
                        total = (m_decRounding * m_objBudgetPlanVersionEntryVM.Unit)+m_decTax,
                        bold = false,
                        additional = true
                    },
                    IconCls = "display: none !important;"
                });
                if (m_objBudgetPlanVersionEntryVM.Area > 1)
                {
                    m_nodeGrandTotal.Children.Add(new Node()
                    {
                        NodeID = BudgetPlanVM.Prop.AreaSize.Name,
                        Leaf = true,
                        AttributesObject = new
                        {
                            sequence = "",
                            itemdesc = BudgetPlanVM.Prop.AreaSize.Desc,
                            budgetplanid = "",
                            budgetplanversion = "",
                            budgetplanversionstructureid = "",
                            specification = "",
                            info = "",
                            uomdesc = "",
                            volume = "",
                            materialamount = "",
                            wageamount = "",
                            miscamount = "",
                            totalunitprice = "",
                            total = m_objBudgetPlanVersionEntryVM.Area,
                            bold = false,
                            additional = true
                        },
                        IconCls = "display: none !important;"
                    });
                    m_nodeGrandTotal.Children.Add(new Node()
                    {
                        NodeID = BudgetPlanVM.Prop.BasicPriceI.Name,
                        Leaf = true,
                        AttributesObject = new
                        {
                            sequence = "",
                            itemdesc = BudgetPlanVM.Prop.BasicPriceI.Desc,
                            budgetplanid = "",
                            budgetplanversion = "",
                            budgetplanversionstructureid = "",
                            specification = "",
                            info = "",
                            uomdesc = "",
                            volume = "",
                            materialamount = "",
                            wageamount = "",
                            miscamount = "",
                            totalunitprice = "",
                            total = m_decBudgetPlanTotal / m_objBudgetPlanVersionEntryVM.Area,
                            bold = false,
                            additional = true
                        },
                        IconCls = "display: none !important;"
                    });
                    m_nodeGrandTotal.Children.Add(new Node()
                    {
                        NodeID = BudgetPlanVM.Prop.BasicPriceII.Name,
                        Leaf = true,
                        AttributesObject = new
                        {
                            sequence = "",
                            itemdesc = BudgetPlanVM.Prop.BasicPriceII.Desc,
                            budgetplanid = "",
                            budgetplanversion = "",
                            budgetplanversionstructureid = "",
                            specification = "",
                            info = "",
                            uomdesc = "",
                            volume = "",
                            materialamount = "",
                            wageamount = "",
                            miscamount = "",
                            totalunitprice = "",
                            total = (m_decBudgetPlanTotal * m_decFeePercentage / 100 + m_decBudgetPlanTotal) / m_objBudgetPlanVersionEntryVM.Area,
                            bold = false,
                            additional = true
                        },
                        IconCls = "display: none !important;"
                    });
                    m_nodeGrandTotal.Children.Add(new Node()
                    {
                        NodeID = BudgetPlanVM.Prop.BasicPriceIII.Name,
                        Leaf = true,
                        AttributesObject = new
                        {
                            sequence = "",
                            itemdesc = BudgetPlanVM.Prop.BasicPriceIII.Desc,
                            budgetplanid = "",
                            budgetplanversion = "",
                            budgetplanversionstructureid = "",
                            specification = "",
                            info = "",
                            uomdesc = "",
                            volume = "",
                            materialamount = "",
                            wageamount = "",
                            miscamount = "",
                            totalunitprice = "",
                            total = ((Math.Round(((decimal)m_decBudgetPlanTotal * (decimal)m_decFeePercentage / (decimal)100 + (decimal)m_decBudgetPlanTotal) / 1000, 0) * 1000) + (Math.Round(((decimal)m_decBudgetPlanTotal * (decimal)m_decFeePercentage / (decimal)100 + (decimal)m_decBudgetPlanTotal) / 1000, 0) * 100)) / m_objBudgetPlanVersionEntryVM.Area,
                            bold = false,
                            additional = true
                        },
                        IconCls = "display: none !important;"
                    });
                }
                m_ncStructure.Add(m_nodeGrandTotal);
                #endregion

                m_node.Children.AddRange(m_ncStructure);

                m_objBudgetPlanVersionEntryVM.ListBudgetPlanVersionStructureVM = m_node;

                m_node = new Node() { NodeID = "root", Expanded = true };
                m_node.Children.AddRange(LoadMyChildAdditional(m_objBudgetPlanVersionEntryVM.BudgetPlanID, m_objBudgetPlanVersionEntryVM.BudgetPlanVersion, m_strVendorID));
                m_objBudgetPlanVersionEntryVM.ListAdditionalWorkItems = m_node;
            }

            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objTBudgetPlanDA.Message;

            return m_objBudgetPlanVersionEntryVM;
        }

        private NodeCollection LoadMyChild(string budgetPlanID, int budgetPlanVersion, List<string> filterItemTypeID, string BudgetPlanVersionVendorID, string VendorID = "",
            string ParentItemID = "0", int ParentVersion = 0, int ParentSequence = 0, string SequenceDesc = "",bool IsCopyFromPrevious = false,BudgetPlanVersionEntryVM budgetPlanVersionEntryVM=null)
        {
            NodeCollection m_nodeColChild = new NodeCollection();

            DBudgetPlanVersionStructureDA m_objDBudgetPlanVersionStructureDA = new DBudgetPlanVersionStructureDA();
            m_objDBudgetPlanVersionStructureDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();

            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersionStructureID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Volume.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.MaterialAmount.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.WageAmount.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.MiscAmount.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.UoMDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Specification.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Sequence.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Version.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ParentItemID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ItemGroupID.MapAlias);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(budgetPlanID);
            m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(budgetPlanVersion);
            m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(String.Join(",", filterItemTypeID));
            m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.ItemTypeID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ParentItemID);
            m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.ParentItemID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ParentSequence);
            m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.ParentSequence.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ParentVersion);
            m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.ParentVersion.Map, m_lstFilter);

            m_dicOrder.Add(BudgetPlanVersionStructureVM.Prop.Sequence.Map, OrderDirection.Ascending);

            List<BudgetPlanVersionEntryVM> m_lstBudgetPlanVersionEntryVM = new List<BudgetPlanVersionEntryVM>();

            Dictionary<int, DataSet> m_dicDBudgetPlanVersionStructureDA = m_objDBudgetPlanVersionStructureDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
            if (m_objDBudgetPlanVersionStructureDA.Message == string.Empty)
            {
                m_lstBudgetPlanVersionEntryVM = (
                    from DataRow m_drDBudgetPlanVersionStructureDA in m_dicDBudgetPlanVersionStructureDA[0].Tables[0].Rows
                    select new BudgetPlanVersionEntryVM()
                    {
                        Sequence = int.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Sequence.Name].ToString()),
                        Version = int.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Version.Name].ToString()),
                        ItemID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ItemID.Name].ToString(),
                        ParentItemID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ParentItemID.Name].ToString(),
                        Specification = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Specification.Name].ToString(),
                        //Info = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionEntryVM.Prop.Info.Name].ToString(),
                        ItemDesc = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ItemDesc.Name].ToString(),
                        BudgetPlanVersionStructureID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.BudgetPlanVersionStructureID.Name].ToString(),
                        BudgetPlanID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.BudgetPlanID.Name].ToString(),
                        ItemGroupID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ItemGroupID.Name].ToString(),
                        BudgetPlanVersion = int.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.Name].ToString()),
                        Volume = decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Volume.Name].ToString()) == 0 ? (decimal?)null : decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Volume.Name].ToString()),
                        /*MaterialAmount = decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name].ToString()) == 0 ? (decimal?)null : decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name].ToString()),
                        WageAmount = decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.WageAmount.Name].ToString()) == 0 ? (decimal?)null : decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.WageAmount.Name].ToString()),
                        MiscAmount = decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.MiscAmount.Name].ToString()) == 0 ? (decimal?)null : decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.MiscAmount.Name].ToString()),
                        Volume = null,*/
                        MaterialAmount = null,
                        WageAmount = null,
                        MiscAmount = null,
                        UoMDesc = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.UoMDesc.Name].ToString(),
                        VendorID = VendorID,
                        Info = "",
                        TotalUnitPrice = null,
                        Total = null
                    }
                ).ToList();
            }
            m_lstBudgetPlanVersionEntryVM = m_lstBudgetPlanVersionEntryVM.OrderBy(m => m.Sequence).ToList();
            for (int i = 0; i < m_lstBudgetPlanVersionEntryVM.Count; i++)
            {
                DBudgetPlanVersionEntryDA m_objDBudgetPlanVersionEntryDA = new DBudgetPlanVersionEntryDA();
                m_objDBudgetPlanVersionEntryDA.ConnectionStringName = Global.ConnStrConfigName;

                m_lstSelect = new List<string>();
                m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.Info.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.Volume.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.MaterialAmount.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.WageAmount.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.MiscAmount.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.VendorID.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ItemGroupID.MapAlias);

                m_objFilter = new Dictionary<string, List<object>>();

                if (IsCopyFromPrevious)
                {
                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(budgetPlanVersionEntryVM.BudgetPlanID);
                    m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanID.Map, m_lstFilter);

                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(budgetPlanVersionEntryVM.BudgetPlanVersion);
                    m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(budgetPlanVersionEntryVM.BudgetPlanVersionVendorID);
                    m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersionVendorID.Map, m_lstFilter);

                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_lstBudgetPlanVersionEntryVM[i].ItemID);
                    m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.ItemID.Map, m_lstFilter);

                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_lstBudgetPlanVersionEntryVM[i].ParentItemID??string.Empty);
                    m_objFilter.Add($"ISNULL({BudgetPlanVersionEntryVM.Prop.ParentItemID.Map},'')", m_lstFilter);

                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_lstBudgetPlanVersionEntryVM[i].Specification ?? string.Empty);
                    m_objFilter.Add($"ISNULL({BudgetPlanVersionStructureVM.Prop.Specification.Map},'')", m_lstFilter);

                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.None);
                    m_lstFilter.Add("");
                    m_objFilter.Add($"{BudgetPlanVersionEntryVM.Prop.Volume.Map} IS NOT NULL OR {BudgetPlanVersionEntryVM.Prop.Volume.Map} != 0", m_lstFilter);

                }
                else
                {
                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_lstBudgetPlanVersionEntryVM[i].BudgetPlanID);
                    m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanID.Map, m_lstFilter);

                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_lstBudgetPlanVersionEntryVM[i].BudgetPlanVersion);
                    m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(BudgetPlanVersionVendorID);
                    m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersionVendorID.Map, m_lstFilter);

                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_lstBudgetPlanVersionEntryVM[i].BudgetPlanVersionStructureID);
                    m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersionStructureID.Map, m_lstFilter);

                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(VendorID);
                    m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.VendorID.Map, m_lstFilter);

                }
                Dictionary<int, DataSet> m_dicDBudgetPlanVersionEntryDA = m_objDBudgetPlanVersionEntryDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);
                if (m_objDBudgetPlanVersionEntryDA.Message == string.Empty)
                {
                    DataRow m_drDBudgetPlanVersionEntryDA = m_dicDBudgetPlanVersionEntryDA[0].Tables[0].Rows[0];
                    if(!IsCopyFromPrevious) m_lstBudgetPlanVersionEntryVM[i].Volume = decimal.Parse(m_drDBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.Volume.Name].ToString()) == 0 ? (decimal?)null : decimal.Parse(m_drDBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.Volume.Name].ToString());
                    m_lstBudgetPlanVersionEntryVM[i].MaterialAmount = decimal.Parse(m_drDBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.MaterialAmount.Name].ToString()) == 0 ? (decimal?)null : decimal.Parse(m_drDBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.MaterialAmount.Name].ToString());
                    m_lstBudgetPlanVersionEntryVM[i].WageAmount = decimal.Parse(m_drDBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.WageAmount.Name].ToString()) == 0 ? (decimal?)null : decimal.Parse(m_drDBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.WageAmount.Name].ToString());
                    m_lstBudgetPlanVersionEntryVM[i].MiscAmount = decimal.Parse(m_drDBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.MiscAmount.Name].ToString()) == 0 ? (decimal?)null : decimal.Parse(m_drDBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.MiscAmount.Name].ToString());
                    m_lstBudgetPlanVersionEntryVM[i].Info = m_drDBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.Info.Name].ToString();
                    m_lstBudgetPlanVersionEntryVM[i].ItemGroupID = m_drDBudgetPlanVersionEntryDA[BudgetPlanVersionStructureVM.Prop.ItemGroupID.Name].ToString();
                }
            }

            int m_intSequence = 1;

            foreach (BudgetPlanVersionEntryVM item in m_lstBudgetPlanVersionEntryVM)
            {
                string m_strSequenceDesc = SequenceDesc.Length > 0 ? SequenceDesc + "." + m_intSequence.ToString() : m_intSequence.ToString();
                Node m_node = new Node();
                m_node.NodeID = item.BudgetPlanVersionStructureID;
                m_node.Children.AddRange(LoadMyChild(budgetPlanID, budgetPlanVersion, filterItemTypeID, BudgetPlanVersionVendorID, VendorID, item.ItemID, item.Version, item.Sequence, m_strSequenceDesc,IsCopyFromPrevious, budgetPlanVersionEntryVM));
                m_node.Icon = Icon.Folder;
                m_node.Expanded = m_node.Children.Count > 0 ? true : false;
                m_node.Leaf = m_node.Children.Count > 0 ? false : true;

                decimal? m_decimalTotalUnitPrice = ((item.MaterialAmount == null ? 0 : item.MaterialAmount) + (item.WageAmount == null ? 0 : item.WageAmount) + (item.MiscAmount == null ? 0 : item.MiscAmount)) == 0 ? (decimal?)null : ((item.MaterialAmount == null ? 0 : item.MaterialAmount) + (item.WageAmount == null ? 0 : item.WageAmount) + (item.MiscAmount == null ? 0 : item.MiscAmount));
                decimal? m_decimalTotal = (((item.MaterialAmount == null ? 0 : item.MaterialAmount) + (item.WageAmount == null ? 0 : item.WageAmount) + (item.MiscAmount == null ? 0 : item.MiscAmount)) * (item.Volume == null ? 0 : item.Volume)) == 0 ? (decimal?)null : (((item.MaterialAmount == null ? 0 : item.MaterialAmount) + (item.WageAmount == null ? 0 : item.WageAmount) + (item.MiscAmount == null ? 0 : item.MiscAmount)) * (item.Volume == null ? 0 : item.Volume));

                if (!m_node.Leaf)
                {
                    m_decimalTotal = 0;

                    foreach (Node itemChild in m_node.Children)
                    {
                        m_decimalTotal += (decimal?)itemChild.AttributesObject.GetType().GetProperties()[13].GetValue(itemChild.AttributesObject, null) == null ? 0
                            : (decimal?)itemChild.AttributesObject.GetType().GetProperties()[13].GetValue(itemChild.AttributesObject, null);
                    }
                }

                m_decimalTotal = m_decimalTotal == 0 ? null : (decimal?)m_decimalTotal;

                //BudgetPlanTotal += m_decimalTotal == null ? 0 : (decimal)m_decimalTotal;

                m_node.AttributesObject = new
                {
                    sequence = m_strSequenceDesc,
                    itemdesc = item.ItemDesc,
                    budgetplanid = item.BudgetPlanID,
                    budgetplanversion = item.BudgetPlanVersion,
                    budgetplanversionstructureid = item.BudgetPlanVersionStructureID,
                    specification = item.Specification,
                    info = item.Info.Length == 0 ? item.Specification : item.Info,
                    uomdesc = item.UoMDesc,
                    volume = item.Volume,
                    materialamount = item.MaterialAmount,
                    wageamount = item.WageAmount,
                    miscamount = item.MiscAmount,
                    totalunitprice = m_decimalTotalUnitPrice,
                    total = m_decimalTotal,
                    bold = false,
                    additional = false,
                    itemid = item.ItemID,
                    parentitemid = item.ParentItemID,
                    itemgroupid = item.ItemGroupID
                };
                m_nodeColChild.Add(m_node);

                m_intSequence++;
            }

            return m_nodeColChild;
        }

        private NodeCollection CheckMyChild(string budgetPlanID, int budgetPlanVersion, List<string> filterItemTypeID, string BudgetPlanVersionVendorID, ref List<string> Message, string VendorID = "",
            string ParentItemID = "0", int ParentVersion = 0, int ParentSequence = 0)
        {
            bool isGroupValid = false;
            List<ConfigVM> m_uConfigVM = GetConfig("BudgetPlan", ItemGroupVM.Prop.ItemGroupID.Name, "Validate");

            NodeCollection m_nodeColChild = new NodeCollection();

            DBudgetPlanVersionStructureDA m_objDBudgetPlanVersionStructureDA = new DBudgetPlanVersionStructureDA();
            m_objDBudgetPlanVersionStructureDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();

            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersionStructureID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Volume.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.MaterialAmount.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.WageAmount.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.MiscAmount.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.UoMDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Specification.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Sequence.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Version.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ItemGroupID.MapAlias);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(budgetPlanID);
            m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(budgetPlanVersion);
            m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(String.Join(",", filterItemTypeID));
            m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.ItemTypeID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ParentItemID);
            m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.ParentItemID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ParentSequence);
            m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.ParentSequence.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ParentVersion);
            m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.ParentVersion.Map, m_lstFilter);

            m_dicOrder.Add(BudgetPlanVersionStructureVM.Prop.Sequence.Map, OrderDirection.Ascending);

            List<BudgetPlanVersionEntryVM> m_lstBudgetPlanVersionEntryVM = new List<BudgetPlanVersionEntryVM>();

            Dictionary<int, DataSet> m_dicDBudgetPlanVersionStructureDA = m_objDBudgetPlanVersionStructureDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
            if (m_objDBudgetPlanVersionStructureDA.Message == string.Empty)
            {
                m_lstBudgetPlanVersionEntryVM = (
                    from DataRow m_drDBudgetPlanVersionStructureDA in m_dicDBudgetPlanVersionStructureDA[0].Tables[0].Rows
                    select new BudgetPlanVersionEntryVM()
                    {
                        Sequence = int.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Sequence.Name].ToString()),
                        Version = int.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Version.Name].ToString()),
                        ItemID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ItemID.Name].ToString(),
                        Specification = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Specification.Name].ToString(),
                        ItemDesc = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ItemDesc.Name].ToString(),
                        BudgetPlanVersionStructureID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.BudgetPlanVersionStructureID.Name].ToString(),
                        BudgetPlanID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.BudgetPlanID.Name].ToString(),
                        ItemGroupID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ItemGroupID.Name].ToString(),
                        BudgetPlanVersion = int.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.Name].ToString()),
                        Volume = decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Volume.Name].ToString()) == 0 ? (decimal?)null : decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Volume.Name].ToString()),
                        /*MaterialAmount = decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name].ToString()) == 0 ? (decimal?)null : decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name].ToString()),
                        WageAmount = decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.WageAmount.Name].ToString()) == 0 ? (decimal?)null : decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.WageAmount.Name].ToString()),
                        MiscAmount = decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.MiscAmount.Name].ToString()) == 0 ? (decimal?)null : decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.MiscAmount.Name].ToString()),
                        Volume = null,*/
                        MaterialAmount = null,
                        WageAmount = null,
                        MiscAmount = null,
                        UoMDesc = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.UoMDesc.Name].ToString(),
                        VendorID = VendorID,
                        Info = "",
                        TotalUnitPrice = null,
                        Total = null
                    }
                ).ToList();
            }
            m_lstBudgetPlanVersionEntryVM = m_lstBudgetPlanVersionEntryVM.OrderBy(m => m.Sequence).ToList();
            for (int i = 0; i < m_lstBudgetPlanVersionEntryVM.Count; i++)
            {
                DBudgetPlanVersionEntryDA m_objDBudgetPlanVersionEntryDA = new DBudgetPlanVersionEntryDA();
                m_objDBudgetPlanVersionEntryDA.ConnectionStringName = Global.ConnStrConfigName;

                m_lstSelect = new List<string>();
                m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.Info.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.Volume.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.MaterialAmount.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.WageAmount.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.MiscAmount.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.VendorID.MapAlias);

                m_objFilter = new Dictionary<string, List<object>>();

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(m_lstBudgetPlanVersionEntryVM[i].BudgetPlanID);
                m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanID.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(m_lstBudgetPlanVersionEntryVM[i].BudgetPlanVersion);
                m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(BudgetPlanVersionVendorID);
                m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersionVendorID.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(m_lstBudgetPlanVersionEntryVM[i].BudgetPlanVersionStructureID);
                m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersionStructureID.Map, m_lstFilter);

                //m_lstFilter = new List<object>();
                //m_lstFilter.Add(Operator.Equals);
                //m_lstFilter.Add(VendorID);
                //m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.VendorID.Map, m_lstFilter);

                Dictionary<int, DataSet> m_dicDBudgetPlanVersionEntryDA = m_objDBudgetPlanVersionEntryDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);
                if (m_objDBudgetPlanVersionEntryDA.Message == string.Empty)
                {
                    DataRow m_drDBudgetPlanVersionEntryDA = m_dicDBudgetPlanVersionEntryDA[0].Tables[0].Rows[0];
                    m_lstBudgetPlanVersionEntryVM[i].Volume = decimal.Parse(m_drDBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.Volume.Name].ToString()) == 0 ? (decimal?)null : decimal.Parse(m_drDBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.Volume.Name].ToString());
                    m_lstBudgetPlanVersionEntryVM[i].MaterialAmount = decimal.Parse(m_drDBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.MaterialAmount.Name].ToString()) == 0 ? (decimal?)null : decimal.Parse(m_drDBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.MaterialAmount.Name].ToString());
                    m_lstBudgetPlanVersionEntryVM[i].WageAmount = decimal.Parse(m_drDBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.WageAmount.Name].ToString()) == 0 ? (decimal?)null : decimal.Parse(m_drDBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.WageAmount.Name].ToString());
                    m_lstBudgetPlanVersionEntryVM[i].MiscAmount = decimal.Parse(m_drDBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.MiscAmount.Name].ToString()) == 0 ? (decimal?)null : decimal.Parse(m_drDBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.MiscAmount.Name].ToString());
                    m_lstBudgetPlanVersionEntryVM[i].Info = m_drDBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.Info.Name].ToString();
                }
            }

            foreach (BudgetPlanVersionEntryVM item in m_lstBudgetPlanVersionEntryVM)
            {
                Node m_node = new Node();
                m_node.NodeID = item.BudgetPlanVersionStructureID;
                m_node.Children.AddRange(CheckMyChild(budgetPlanID, budgetPlanVersion, filterItemTypeID, BudgetPlanVersionVendorID, ref Message, VendorID, item.ItemID, item.Version, item.Sequence));
                m_node.Icon = Icon.Folder;
                m_node.Expanded = m_node.Children.Count > 0 ? true : false;
                m_node.Leaf = m_node.Children.Count > 0 ? false : true;

                decimal? m_decimalTotalUnitPrice = ((item.MaterialAmount == null ? 0 : item.MaterialAmount) + (item.WageAmount == null ? 0 : item.WageAmount) + (item.MiscAmount == null ? 0 : item.MiscAmount)) == 0 ? (decimal?)null : ((item.MaterialAmount == null ? 0 : item.MaterialAmount) + (item.WageAmount == null ? 0 : item.WageAmount) + (item.MiscAmount == null ? 0 : item.MiscAmount));
                decimal? m_decimalTotal = (((item.MaterialAmount == null ? 0 : item.MaterialAmount) + (item.WageAmount == null ? 0 : item.WageAmount) + (item.MiscAmount == null ? 0 : item.MiscAmount)) * (item.Volume == null ? 0 : item.Volume)) == 0 ? (decimal?)null : (((item.MaterialAmount == null ? 0 : item.MaterialAmount) + (item.WageAmount == null ? 0 : item.WageAmount) + (item.MiscAmount == null ? 0 : item.MiscAmount)) * (item.Volume == null ? 0 : item.Volume));

                if (!m_node.Leaf)
                {
                    m_decimalTotal = 0;

                    foreach (Node itemChild in m_node.Children)
                    {
                        m_decimalTotal += (decimal?)itemChild.AttributesObject.GetType().GetProperties()[12].GetValue(itemChild.AttributesObject, null) == null ? 0 : (decimal?)itemChild.AttributesObject.GetType().GetProperties()[12].GetValue(itemChild.AttributesObject, null);
                    }
                }

                
                if (m_uConfigVM != null)
                    if (m_uConfigVM.Any(d => d.Key4 == item.ItemGroupID))
                    {
                        isGroupValid = true;
                    }

                m_decimalTotal = m_decimalTotal == 0 ? null : (decimal?)m_decimalTotal;
                
                if (m_decimalTotal == null && !isGroupValid)
                {
                    Message.Add(item.ItemDesc);
                    break;
                }

                m_node.AttributesObject = new
                {
                    itemdesc = item.ItemDesc,
                    budgetplanid = item.BudgetPlanID,
                    budgetplanversion = item.BudgetPlanVersion,
                    budgetplanversionstructureid = item.BudgetPlanVersionStructureID,
                    specification = item.Specification,
                    info = item.Info.Length == 0 ? item.Specification : item.Info,
                    uomdesc = item.UoMDesc,
                    volume = item.Volume,
                    materialamount = item.MaterialAmount,
                    wageamount = item.WageAmount,
                    miscamount = item.MiscAmount,
                    totalunitprice = m_decimalTotalUnitPrice,
                    total = m_decimalTotal,
                    bold = false,
                    additional = false
                };
                m_nodeColChild.Add(m_node);
            }

            return m_nodeColChild;
        }

        private NodeCollection LoadMyChildAdditional(string budgetPlanID, int budgetPlanVersion, string VendorID = "",
            string ParentItemID = "0", int ParentVersion = 0, int ParentSequence = 0)
        {
            NodeCollection m_nodeColChild = new NodeCollection();

            DBudgetPlanVersionAdditionalDA m_objDBudgetPlanVersionAdditionalDA = new DBudgetPlanVersionAdditionalDA();
            m_objDBudgetPlanVersionAdditionalDA.ConnectionStringName = Global.ConnStrConfigName;
            object m_objConnection = m_objDBudgetPlanVersionAdditionalDA.BeginConnection();

            List<string> m_lstSelect = new List<string>();
            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();

            m_lstSelect.Add(BudgetPlanVersionAdditionalVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionAdditionalVM.Prop.BudgetPlanVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionAdditionalVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionAdditionalVM.Prop.Version.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionAdditionalVM.Prop.Sequence.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionAdditionalVM.Prop.Info.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionAdditionalVM.Prop.Volume.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionAdditionalVM.Prop.UoMDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionAdditionalVM.Prop.ItemDesc.MapAlias);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(budgetPlanID);
            m_objFilter.Add(BudgetPlanVersionAdditionalVM.Prop.BudgetPlanID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(budgetPlanVersion);
            m_objFilter.Add(BudgetPlanVersionAdditionalVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(VendorID);
            m_objFilter.Add(BudgetPlanVersionAdditionalVM.Prop.VendorID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ParentItemID);
            m_objFilter.Add(BudgetPlanVersionAdditionalVM.Prop.ParentItemID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ParentSequence);
            m_objFilter.Add(BudgetPlanVersionAdditionalVM.Prop.ParentSequence.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ParentVersion);
            m_objFilter.Add(BudgetPlanVersionAdditionalVM.Prop.ParentVersion.Map, m_lstFilter);

            m_dicOrder.Add(BudgetPlanVersionAdditionalVM.Prop.Sequence.Map, OrderDirection.Ascending);

            List<BudgetPlanVersionAdditionalVM> m_lstBudgetPlanVersionAdditionalVM = new List<BudgetPlanVersionAdditionalVM>();

            Dictionary<int, DataSet> m_dicDBudgetPlanVersionAdditionalDA = m_objDBudgetPlanVersionAdditionalDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
            if (m_objDBudgetPlanVersionAdditionalDA.Message == string.Empty)
            {
                m_lstBudgetPlanVersionAdditionalVM = (
                    from DataRow m_drDBudgetPlanVersionAdditionalDA in m_dicDBudgetPlanVersionAdditionalDA[0].Tables[0].Rows
                    select new BudgetPlanVersionAdditionalVM()
                    {
                        Sequence = int.Parse(m_drDBudgetPlanVersionAdditionalDA[BudgetPlanVersionAdditionalVM.Prop.Sequence.Name].ToString()),
                        Version = int.Parse(m_drDBudgetPlanVersionAdditionalDA[BudgetPlanVersionAdditionalVM.Prop.Version.Name].ToString()),
                        ItemID = m_drDBudgetPlanVersionAdditionalDA[BudgetPlanVersionAdditionalVM.Prop.ItemID.Name].ToString(),
                        ItemDesc = m_drDBudgetPlanVersionAdditionalDA[BudgetPlanVersionAdditionalVM.Prop.ItemDesc.Name].ToString(),
                        BudgetPlanID = m_drDBudgetPlanVersionAdditionalDA[BudgetPlanVersionAdditionalVM.Prop.BudgetPlanID.Name].ToString(),
                        BudgetPlanVersion = int.Parse(m_drDBudgetPlanVersionAdditionalDA[BudgetPlanVersionAdditionalVM.Prop.BudgetPlanVersion.Name].ToString()),
                        Volume = decimal.Parse(m_drDBudgetPlanVersionAdditionalDA[BudgetPlanVersionAdditionalVM.Prop.Volume.Name].ToString()) == 0 ? (decimal?)null : decimal.Parse(m_drDBudgetPlanVersionAdditionalDA[BudgetPlanVersionAdditionalVM.Prop.Volume.Name].ToString()),
                        UoMDesc = m_drDBudgetPlanVersionAdditionalDA[BudgetPlanVersionAdditionalVM.Prop.UoMDesc.Name].ToString(),
                        Info = m_drDBudgetPlanVersionAdditionalDA[BudgetPlanVersionAdditionalVM.Prop.Info.Name].ToString()
                    }
                ).ToList();
            }

            m_objDBudgetPlanVersionAdditionalDA.EndConnection(ref m_objConnection);

            foreach (BudgetPlanVersionAdditionalVM item in m_lstBudgetPlanVersionAdditionalVM)
            {
                Node m_node = new Node();
                m_node.Children.AddRange(LoadMyChildAdditional(budgetPlanID, budgetPlanVersion, VendorID, item.ItemID, item.Version, item.Sequence));
                m_node.Icon = Icon.Folder;
                m_node.Expanded = m_node.Children.Count > 0 ? true : false;
                m_node.Leaf = m_node.Children.Count > 0 ? false : true;

                m_node.AttributesObject = new
                {
                    budgetplanid = item.BudgetPlanID,
                    budgetplanversion = item.BudgetPlanVersion,
                    itemid = item.ItemID,
                    version = item.Version,
                    sequence = "",
                    itemdesc = item.ItemDesc,
                    uomdesc = item.UoMDesc,
                    info = item.Info,
                    volume = item.Volume,
                    parentitemid = item.ItemID,
                    parentversion = item.Version,
                    parentsequence = ""
                };
                m_nodeColChild.Add(m_node);
            }

            return m_nodeColChild;
        }

        public string GetVendorID()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                string m_strVendorID = HttpContext.User.Identity.Name;

                Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                List<object> m_lstFilter = new List<object>();
                List<string> m_lstSelect = new List<string>();

                MUserDA m_objMUserDA = new MUserDA();
                m_objMUserDA.ConnectionStringName = Global.ConnStrConfigName;

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(m_strVendorID);
                m_objFilter.Add(UserVM.Prop.UserID.Map, m_lstFilter);

                m_lstSelect = new List<string>();
                m_lstSelect.Add(UserVM.Prop.VendorID.MapAlias);

                Dictionary<int, DataSet> m_dicMUserDA = m_objMUserDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null, null);

                if (m_objMUserDA.Success)
                {
                    m_strVendorID = m_dicMUserDA[0].Tables[0].Rows[0][UserVM.Prop.VendorID.Name].ToString();
                }

                return m_strVendorID;
            }
            else
            {
                return "";
            }
        }

        #endregion

        #region Public Method

        public Dictionary<int, List<BudgetPlanVersionEntryVM>> GetBudgetPlanVersionEntryData(bool isCount, string BudgetPlanVersionEntryID, string BudgetPlanVersionEntryDesc)
        {
            int m_intCount = 0;
            List<BudgetPlanVersionEntryVM> m_lstBudgetPlanVersionEntryVM = new List<ViewModels.BudgetPlanVersionEntryVM>();
            Dictionary<int, List<BudgetPlanVersionEntryVM>> m_dicReturn = new Dictionary<int, List<BudgetPlanVersionEntryVM>>();
            DBudgetPlanVersionEntryDA m_objDBudgetPlanVersionEntryDA = new DBudgetPlanVersionEntryDA();
            m_objDBudgetPlanVersionEntryDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersion.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(BudgetPlanVersionEntryID);
            m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(BudgetPlanVersionEntryDesc);
            m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMBudgetPlanVersionEntryDA = m_objDBudgetPlanVersionEntryDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDBudgetPlanVersionEntryDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpBudgetPlanVersionEntryBL in m_dicMBudgetPlanVersionEntryDA)
                    {
                        m_intCount = m_kvpBudgetPlanVersionEntryBL.Key;
                        break;
                    }
                else
                {
                    m_lstBudgetPlanVersionEntryVM = (
                        from DataRow m_drMBudgetPlanVersionEntryDA in m_dicMBudgetPlanVersionEntryDA[0].Tables[0].Rows
                        select new BudgetPlanVersionEntryVM()
                        {
                            BudgetPlanID = m_drMBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.BudgetPlanID.Name].ToString(),
                            BudgetPlanVersion = int.Parse(m_drMBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.BudgetPlanVersion.Name].ToString())
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstBudgetPlanVersionEntryVM);
            return m_dicReturn;
        }

        #endregion
    }
}