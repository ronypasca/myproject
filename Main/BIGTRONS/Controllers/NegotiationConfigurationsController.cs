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
using SWeb = System.Web;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using Newtonsoft.Json;

namespace com.SML.BIGTRONS.Controllers
{
    public class NegotiationConfigurationsController : BaseController
    {
        private readonly string title = "Negotiation Configurations";
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
            CNegotiationConfigurationsDA m_objnegoconfigDA = new CNegotiationConfigurationsDA();
            m_objnegoconfigDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcNegoConfig = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcNegoConfig.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = NegotiationConfigurationsVM.Prop.Map(m_strDataIndex, false);
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
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.FPTID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.FPTDesc.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.FPTStatusID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.FPTStatusDesc.MapAlias);
            m_lstSelect.Add("MAX(" + NegotiationConfigurationsVM.Prop.NegotiationConfigID.Map + ") as NegotiationConfigID");
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.StatusID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.StatusDesc.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.TaskID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.CurrentApprovalLvl.MapAlias);

            List<string> m_lstGroup = new List<string>();
            m_lstGroup.Add(NegotiationConfigurationsVM.Prop.FPTID.Map);
            m_lstGroup.Add(NegotiationConfigurationsVM.Prop.FPTDesc.Map);
            m_lstGroup.Add(NegotiationConfigurationsVM.Prop.FPTStatusID.Map);
            m_lstGroup.Add(NegotiationConfigurationsVM.Prop.FPTStatusDesc.Map);
            m_lstGroup.Add(NegotiationConfigurationsVM.Prop.TaskID.Map);
            m_lstGroup.Add(NegotiationConfigurationsVM.Prop.StatusID.Map);
            m_lstGroup.Add(NegotiationConfigurationsVM.Prop.StatusDesc.Map);
            m_lstGroup.Add(NegotiationConfigurationsVM.Prop.CurrentApprovalLvl.Map);

            Dictionary<int, DataSet> m_dicNegoConfigDA = m_objnegoconfigDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, m_lstGroup, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpNegoConfigBL in m_dicNegoConfigDA)
            {
                m_intCount = m_kvpNegoConfigBL.Key;
                break;
            }

            List<NegotiationConfigurationsVM> m_lstNegoConfigVM = new List<NegotiationConfigurationsVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                

                //Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                //foreach (DataSorter m_dtsOrder in parameters.Sort)
                //    m_dicOrder.Add(NegotiationConfigurationsVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(NegotiationConfigurationsVM.Prop.FPTID.Map, General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicNegoConfigDA = m_objnegoconfigDA.SelectBC(m_intSkip, null, m_boolIsCount, m_lstSelect, m_objFilter, null, m_lstGroup, m_dicOrder, null);
                if (m_objnegoconfigDA.Message == string.Empty)
                {
                    m_lstNegoConfigVM = (
                        from DataRow m_drNegoConfigDA in m_dicNegoConfigDA[0].Tables[0].Rows
                        select new NegotiationConfigurationsVM()
                        {
                            FPTID = m_drNegoConfigDA[NegotiationConfigurationsVM.Prop.FPTID.Name].ToString(),
                            FPTDesc = m_drNegoConfigDA[NegotiationConfigurationsVM.Prop.FPTDesc.Name].ToString(),
                            //NegotiationConfigID = m_drNegoConfigDA[NegotiationConfigurationsVM.Prop.NegotiationConfigID.Name].ToString(),
                            //NegotiationConfigTypeID = m_drNegoConfigDA[NegotiationConfigurationsVM.Prop.NegotiationConfigTypeID.Name].ToString(),
                            TaskID = m_drNegoConfigDA[NegotiationConfigurationsVM.Prop.TaskID.Name].ToString(),
                            CurrentApprovalLvl = int.Parse(m_drNegoConfigDA[NegotiationConfigurationsVM.Prop.CurrentApprovalLvl.Name].ToString()),
                            FPTStatusID = m_drNegoConfigDA[NegotiationConfigurationsVM.Prop.FPTStatusID.Name].ToString(),
                            FPTStatusDesc = m_drNegoConfigDA[NegotiationConfigurationsVM.Prop.FPTStatusDesc.Name].ToString(),
                            //ParameterValue = m_drNegoConfigDA[NegotiationConfigurationsVM.Prop.ParameterValue.Name].ToString(),
                            StatusID = m_drNegoConfigDA[NegotiationConfigurationsVM.Prop.StatusID.Name].ToString(),
                            StatusDesc = m_drNegoConfigDA[NegotiationConfigurationsVM.Prop.StatusDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstNegoConfigVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            MCompanyDA m_objMCompanyDA = new MCompanyDA();
            m_objMCompanyDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMCompany = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMCompany.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = CompanyVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMCompanyDA = m_objMCompanyDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpCompanyBL in m_dicMCompanyDA)
            {
                m_intCount = m_kvpCompanyBL.Key;
                break;
            }

            List<CompanyVM> m_lstCompanyVM = new List<CompanyVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(CompanyVM.Prop.CompanyID.MapAlias);
                m_lstSelect.Add(CompanyVM.Prop.CompanyDesc.MapAlias);
                m_lstSelect.Add(CompanyVM.Prop.CountryDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(CompanyVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMCompanyDA = m_objMCompanyDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMCompanyDA.Message == string.Empty)
                {
                    m_lstCompanyVM = (
                        from DataRow m_drMCompanyDA in m_dicMCompanyDA[0].Tables[0].Rows
                        select new CompanyVM()
                        {
                            CompanyID = m_drMCompanyDA[CompanyVM.Prop.CompanyID.Name].ToString(),
                            CompanyDesc = m_drMCompanyDA[CompanyVM.Prop.CompanyDesc.Name].ToString(),
                            CountryDesc = m_drMCompanyDA[CompanyVM.Prop.CountryDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstCompanyVM, m_intCount);
        }
        public ActionResult ReadBrowseFPT(StoreRequestParameters parameters)
        {
            MFPTDA m_objMFPTDA = new MFPTDA();
            m_objMFPTDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMFPT = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMFPT.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = FPTVM.Prop.Map(m_strDataIndex, false);
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
            string message = "";

            List<string> ListFPT = GetFPTinNegoConfig(ref message);
            List<object> m_lstFilters = new List<object>();
            m_lstFilters.Add(Operator.NotIn);
            m_lstFilters.Add("(select FPTID from CNegotiationConfigurations)");

            if (!m_objFilter.Keys.Any(x => x == FPTVM.Prop.FPTID.Map))
                m_objFilter.Add(FPTVM.Prop.FPTID.Map, m_lstFilters);
            else
            {
                List<object> m_lstFilterss = new List<object>();
                m_lstFilterss.Add(Operator.None);
                m_lstFilterss.Add(string.Empty);
                //("(select FPTID from CNegotiationConfigurations)");
                m_objFilter.Add(string.Format("{0} NOT IN (select FPTID from CNegotiationConfigurations)",  FPTVM.Prop.FPTID.Map), m_lstFilterss);
            }

            Dictionary<int, DataSet> m_dicMFPTDA = m_objMFPTDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpFPTBL in m_dicMFPTDA)
            {
                m_intCount = m_kvpFPTBL.Key;
                break;
            }
            List<FPTVM> m_lstFPTVM = new List<FPTVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(FPTVM.Prop.FPTID.MapAlias);
                m_lstSelect.Add(FPTVM.Prop.BusinessUnitID.MapAlias);
                m_lstSelect.Add(FPTVM.Prop.BusinessUnitDesc.MapAlias);
                m_lstSelect.Add(FPTVM.Prop.DivisionID.MapAlias);
                m_lstSelect.Add(FPTVM.Prop.ProjectID.MapAlias);
                m_lstSelect.Add(FPTVM.Prop.ClusterID.MapAlias);
                m_lstSelect.Add(FPTVM.Prop.ProjectDesc.MapAlias);
                m_lstSelect.Add(FPTVM.Prop.DivisionDesc.MapAlias);
                m_lstSelect.Add(FPTVM.Prop.ClusterDesc.MapAlias);
                m_lstSelect.Add(FPTVM.Prop.Descriptions.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(FPTVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMFPTDA = m_objMFPTDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMFPTDA.Message == string.Empty)
                {
                    m_lstFPTVM = (
                        from DataRow m_drMFPTDA in m_dicMFPTDA[0].Tables[0].Rows
                        select new FPTVM()
                        {
                            FPTID = m_drMFPTDA[FPTVM.Prop.FPTID.Name].ToString(),
                            ClusterID = string.IsNullOrEmpty(m_drMFPTDA[FPTVM.Prop.ClusterID.Name].ToString()) ? "" : m_drMFPTDA[FPTVM.Prop.ClusterID.Name].ToString(),
                            Descriptions = m_drMFPTDA[FPTVM.Prop.Descriptions.Name].ToString(),
                            ProjectID = m_drMFPTDA[FPTVM.Prop.ProjectID.Name].ToString(),
                            BusinessUnitID = m_drMFPTDA[FPTVM.Prop.BusinessUnitID.Name].ToString(),
                            BusinessUnitDesc = m_drMFPTDA[FPTVM.Prop.BusinessUnitDesc.Name].ToString(),
                            ProjectDesc = m_drMFPTDA[FPTVM.Prop.ProjectDesc.Name].ToString(),
                            DivisionDesc = m_drMFPTDA[FPTVM.Prop.DivisionDesc.Name].ToString(),
                            ClusterDesc= m_drMFPTDA[FPTVM.Prop.ClusterDesc.Name].ToString(),
                            DivisionID = m_drMFPTDA[FPTVM.Prop.DivisionID.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstFPTVM, m_intCount);
        }

        public ActionResult Home()
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

        public ActionResult Add(string Caller)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Add)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            FPTVM m_objFPTVM = new FPTVM();
            ViewDataDictionary m_vddCompany = new ViewDataDictionary();
            string message = "";
            m_vddCompany.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddCompany.Add("isRenegotiation", false);
            m_vddCompany.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
            if (Caller == General.EnumDesc(Buttons.ButtonDetail))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                Dictionary<string, object> m_dicSelectedRow = GetFormData(m_nvcParams);
                Session[dataSessionName] = JSON.Serialize(m_dicSelectedRow);
            }
            else
            {
                m_objFPTVM = GetForAdd(ref message);
            }

            var m_source = m_objFPTVM.ListNegotiationConfigurationsVM.Where(d => d.NegotiationConfigID == General.EnumDesc(NegoConfigTypes.Source)).Select(u => u.ParameterValue);
            m_vddCompany.Add("selectedSource", m_source.Any() ? m_source.FirstOrDefault() : ((int)NegoConfigSource.BudgetPlan).ToString());
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objFPTVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddCompany,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }


        public string ViewComparation(string FPTID)
        {

            Dictionary<string, object> result = new Dictionary<string, object>();

            try
            {
                DataSet dt = new DataSet();
                dt = GetFPTComaparation(FPTID);


                string table = "";
                table = "" +
                            "<thead>" +
                                "<tr>" +
                                    "<th style='text-align: center;'>Item ID</th>" +
                                    "<th style='text-align: center;'>Item Desc</th>" +
                                     "<th style='text-align: center;'>TRM</th>";

                for (int i = 3; i < dt.Tables[1].Columns.Count; i++)
                {
                    table += String.Format("<th style='text-align: center;'>{0}</th>", dt.Tables[1].Columns[i].ColumnName);
                }

                table += "</tr>" +
                            "</thead>";

                //body
                table += "<tbody>";

                int i_row = 0;

                if (dt.Tables[0].Rows.Count != dt.Tables[1].Rows.Count)
                {
                    throw new Exception("RAB Rows Number and Vendor Bid Rows Number not same");
                }


                for (i_row = 0; i_row < dt.Tables[0].Rows.Count; i_row++)
                {
                    table += "<tr " + (dt.Tables[0].Rows[i_row]["ItemParentID"].ToString() == "0" ? "style='font-weight:bold'" : "") + ">";
                    table += String.Format("<td style='text-align: left;'>{0}</td><td style='text-align: left;'>{1}</td><td style='text-align: right;'>{2}</td>",
                        dt.Tables[0].Rows[i_row]["ItemID"].ToString(), dt.Tables[0].Rows[i_row]["ItemDesc"].ToString(),
                        String.Format("{0:n0}", decimal.Parse(dt.Tables[0].Rows[i_row]["BudgetPlanDefaultValue"].ToString())));

                    for (int i = 3; i < dt.Tables[1].Columns.Count; i++)
                    {
                        table += String.Format("<td style='text-align: right;'>{0}</td>", String.Format("{0:n0}", decimal.Parse(dt.Tables[1].Rows[i_row][i].ToString())));
                    }

                    table += "</tr>";
                }

                table += "</tbody>";

                table += "</table>";

                result.Add("result", false);
                result.Add("message", table);
            }
            catch(Exception ex)
            {
                result.Add("result", false);
                result.Add("message", ex.Message);
            }

            return JsonConvert.SerializeObject(result);
        }

        public ActionResult Detail(string Caller, string Selected)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));


            string m_strMessage = string.Empty;
            FPTVM m_objFPTVM = new FPTVM();
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
                m_objFPTVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddFPT = new ViewDataDictionary();
            m_vddFPT.Add("isRenegotiation", false);
            m_vddFPT.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
            m_vddFPT.Add(StatusVM.Prop.StatusID.Name, m_objFPTVM.TaskStatus);
            m_vddFPT.Add(FPTVM.Prop.FPTID.Name, m_objFPTVM.FPTID.ToString().Trim());
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            var m_source = m_objFPTVM.ListNegotiationConfigurationsVM.Where(d => d.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.Source)).Select(u => u.ParameterValue);
            m_vddFPT.Add("selectedSource", m_source.Any() ? ((int)System.Enum.Parse(typeof(NegoConfigSource), m_source.FirstOrDefault())).ToString() : ((int)NegoConfigSource.BudgetPlan).ToString());

            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objFPTVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddFPT,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Verify(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Verify)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            string TaskID = "";
            string FPTID = "";
            int currentAppLevel = 0;
            List<string> m_lstMessage = new List<string>();
            List<NegotiationConfigurationsVM> m_lstSelectedRow = new List<NegotiationConfigurationsVM>();
            try
            {
                m_lstSelectedRow = JSON.Deserialize<List<NegotiationConfigurationsVM>>(Selected);
                TaskID = m_lstSelectedRow[0].TaskID;
                currentAppLevel = m_lstSelectedRow[0].CurrentApprovalLvl;
                FPTID = m_lstSelectedRow[0].FPTID;
            }
            catch (Exception e)
            {
                m_lstMessage.Add(e.Message);
            }

            string message_ = "";
            string currentApprovalRole = GetCurrentApproval(General.EnumDesc(TaskType.NegotiationConfigurations), currentAppLevel);
            string ParentApproval = GetParentApproval(ref message_, currentApprovalRole, General.EnumDesc(TaskType.NegotiationConfigurations));
            if (!string.IsNullOrEmpty(message_))
                m_lstMessage.Add(message_);

            MTasksDA m_objMTasksDA = new MTasksDA();
            MTasks m_objMTask = new MTasks();
            m_objMTasksDA.ConnectionStringName = Global.ConnStrConfigName;

            DTaskDetailsDA m_objDTaskDetailsDA = new DTaskDetailsDA();
            DTaskDetails m_obTaskDetail = new DTaskDetails();
            m_objDTaskDetailsDA.ConnectionStringName = Global.ConnStrConfigName;

            DFPTStatusDA m_objDFPTStatusDA = new DFPTStatusDA();
            DFPTStatus m_objDFPTStatus = new DFPTStatus();
            m_objDFPTStatusDA.ConnectionStringName = Global.ConnStrConfigName;

            string m_strTransactionName = "TransactionVerifyEntry";
            object m_objConnection = m_objMTasksDA.BeginTrans(m_strTransactionName);

            m_objMTasksDA.Data = m_objMTask;
            m_objMTask.TaskID = m_lstSelectedRow[0].TaskID;
            m_objMTasksDA.Select();
            m_objMTask.TaskOwnerID = ParentApproval;
            m_objMTask.TaskDateTimeStamp = DateTime.Now;
            m_objMTask.StatusID = (int)TaskStatus.InProgress;
            m_objMTask.CurrentApprovalLvl = currentAppLevel + 1;
            m_objMTask.Remarks = "Negotiation Configurations";

            if (m_lstMessage.Count <= 0)
                m_objMTasksDA.Update(true, m_objConnection);

            m_objDTaskDetailsDA.Data = m_obTaskDetail;
            m_obTaskDetail.TaskDetailID = Guid.NewGuid().ToString().Replace("-", "");
            m_obTaskDetail.TaskID = m_lstSelectedRow[0].TaskID;
            m_obTaskDetail.StatusID = (int)TaskDetailStatus.Submit;
            m_obTaskDetail.Remarks = "Submit Negotiation Configurations";

            if (string.IsNullOrEmpty(m_objMTasksDA.Message))
                m_objDTaskDetailsDA.Insert(true, m_objConnection);
            else
                m_lstMessage.Add(m_objMTasksDA.Message);

            m_objDFPTStatusDA.Data = m_objDFPTStatus;
            m_objDFPTStatus.FPTID = FPTID;
            m_objDFPTStatus.StatusDateTimeStamp = DateTime.Now;
            m_objDFPTStatus.StatusID = 9; ///Submitted Draft

            if (string.IsNullOrEmpty(m_objDTaskDetailsDA.Message))
                m_objDFPTStatusDA.Insert(true, m_objConnection);
            else
                m_lstMessage.Add(m_objDTaskDetailsDA.Message);

            if (!m_objDFPTStatusDA.Success || !m_objMTasksDA.Success || !string.IsNullOrEmpty(m_objMTasksDA.Message) || m_lstMessage.Count > 0)
            {
                m_objMTasksDA.EndTrans(ref m_objConnection, false, m_strTransactionName);
                return this.Direct(false, String.Join(Global.NewLineSeparated, m_lstMessage));
            }
            else
            {
                m_objMTasksDA.EndTrans(ref m_objConnection, true, m_strTransactionName);
                return this.Direct(true, String.Join(Global.NewLineSeparated, m_lstMessage));
            }
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
            FPTVM m_objFPTVM = new FPTVM();
            if (m_dicSelectedRow.Count > 0)
                m_objFPTVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }
            bool renego = m_objFPTVM.FPTStatusID == 13;

            ViewDataDictionary m_vddFPT = new ViewDataDictionary();
            m_vddFPT.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddFPT.Add("isRenegotiation", renego);
            var m_source = m_objFPTVM.ListNegotiationConfigurationsVM.Where(d => d.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.Source)).Select(u => u.ParameterValue);
            m_vddFPT.Add("selectedSource", m_source.Any()? ((int)System.Enum.Parse(typeof(NegoConfigSource), m_source.FirstOrDefault())).ToString() :((int)NegoConfigSource.BudgetPlan).ToString());
            m_vddFPT.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objFPTVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddFPT,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult DeleteListTCmember()
        {
            return this.Direct(true);
        }
        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<CompanyVM> m_lstSelectedRow = new List<CompanyVM>();
            m_lstSelectedRow = JSON.Deserialize<List<CompanyVM>>(Selected);

            MCompanyDA m_objMCompanyDA = new MCompanyDA();
            m_objMCompanyDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (CompanyVM m_objCompanyVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifCompanyVM = m_objCompanyVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifCompanyVM in m_arrPifCompanyVM)
                    {
                        string m_strFieldName = m_pifCompanyVM.Name;
                        object m_objFieldValue = m_pifCompanyVM.GetValue(m_objCompanyVM);
                        if (m_objCompanyVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(CompanyVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objMCompanyDA.DeleteBC(m_objFilter, false);
                    if (m_objMCompanyDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMCompanyDA.Message);
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
        public ActionResult Browse(string ControlCompanyID, string ControlCompanyDesc, string FilterCompanyID = "", string FilterCompanyDesc = "")
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddCompany = new ViewDataDictionary();
            m_vddCompany.Add("Control" + CompanyVM.Prop.CompanyID.Name, ControlCompanyID);
            m_vddCompany.Add("Control" + CompanyVM.Prop.CompanyDesc.Name, ControlCompanyDesc);
            m_vddCompany.Add(CompanyVM.Prop.CompanyID.Name, FilterCompanyID);
            m_vddCompany.Add(CompanyVM.Prop.CompanyDesc.Name, FilterCompanyDesc);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddCompany,
                ViewName = "../Company/_Browse"
            };
        }
        public ActionResult BrowseFPT(string ControlFPTID, string ControlFPTDesc, string ControlBUnit, string ControlBUnitDesc, string OldValueFPTID, string ControlDivision, string ControlProject, string ControlClusterID, string FilterFPTID = "", string FilterFPTDesc = "")
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));
            ViewDataDictionary m_vddFPT = new ViewDataDictionary();
            m_vddFPT.Add("Control" + NegotiationConfigurationsVM.Prop.FPTID.Name, ControlFPTID);
            m_vddFPT.Add("Control" + NegotiationConfigurationsVM.Prop.FPTDesc.Name, ControlFPTDesc);
            m_vddFPT.Add("ControlBUnit", ControlBUnit);
            m_vddFPT.Add("ControlBUnitDesc", ControlBUnitDesc);
            m_vddFPT.Add("ControlDivision", ControlDivision);
            m_vddFPT.Add("ControlProject", ControlProject);
            m_vddFPT.Add("OldValueFPTID", OldValueFPTID);
            m_vddFPT.Add("Control" + FPTVM.Prop.ClusterID.Name, ControlClusterID);
            m_vddFPT.Add(NegotiationConfigurationsVM.Prop.FPTID.Name, FilterFPTID);
            m_vddFPT.Add(NegotiationConfigurationsVM.Prop.FPTDesc.Name, FilterFPTDesc);
            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddFPT,
                ViewName = "../NegotiationConfigurations/_BrowseFPT"
            };
        }
        public ActionResult Save(string Action)
        {
            //action = JSON.Deserialize<string>(action);
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            #region Initialize
            List<string> m_lstMessage = new List<string>();

            CNegotiationConfigurationsDA m_objNegoConfigDA = new CNegotiationConfigurationsDA();
            CNegotiationConfigurations m_objNegoConfig = new CNegotiationConfigurations();
            m_objNegoConfigDA.ConnectionStringName = Global.ConnStrConfigName;

            MTasksDA m_objMTasksDA = new MTasksDA();
            MTasks m_objMTask = new MTasks();
            m_objMTasksDA.ConnectionStringName = Global.ConnStrConfigName;

            DTaskDetailsDA m_objDTaskDetailsDA = new DTaskDetailsDA();
            DTaskDetails m_obTaskDetail = new DTaskDetails();
            m_objDTaskDetailsDA.ConnectionStringName = Global.ConnStrConfigName;

            DFPTStatusDA m_objDFPTStatusDA = new DFPTStatusDA();
            DFPTStatus m_objDFPTStatus = new DFPTStatus();
            m_objDFPTStatusDA.ConnectionStringName = Global.ConnStrConfigName;

            DFPTVendorParticipantsDA m_objDFPTVendorParticipantsDA = new DFPTVendorParticipantsDA();
            DFPTVendorParticipants m_objDFPTVendorParticipants = new DFPTVendorParticipants();
            m_objDFPTVendorParticipantsDA.ConnectionStringName = Global.ConnStrConfigName;

            DFPTTCParticipantsDA m_objDFPTTCParticipantsDA = new DFPTTCParticipantsDA();
            DFPTTCParticipants m_objDFPTTCParticipants = new DFPTTCParticipants();
            m_objDFPTTCParticipantsDA.ConnectionStringName = Global.ConnStrConfigName;

            TNegotiationBidStructuresDA m_objTNegoBidStructuresDA = new TNegotiationBidStructuresDA();
            TNegotiationBidStructures m_objTNegoBidStructures = new TNegotiationBidStructures();
            m_objTNegoBidStructuresDA.ConnectionStringName = Global.ConnStrConfigName;

            DFPTNegotiationRoundsDA m_objDFPTNegotiationRoundsDA = new DFPTNegotiationRoundsDA();
            DFPTNegotiationRounds obj_DFPTNegotiationRounds = new DFPTNegotiationRounds();
            m_objDFPTNegotiationRoundsDA.ConnectionStringName = Global.ConnStrConfigName;

            DNegotiationBidEntryDA m_objNegoBidEntryDA = new DNegotiationBidEntryDA();
            DNegotiationBidEntry m_objNegoBidEntry = new DNegotiationBidEntry();
            m_objNegoBidEntryDA.ConnectionStringName = Global.ConnStrConfigName;

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            object m_objDBConnection = null;
            string m_strTransName = "SavingTaskAndConfig";
            m_objDBConnection = m_objNegoConfigDA.BeginTrans(m_strTransName);

            #endregion
            try
            {
                #region Populate Parameter
                string ProjectName = "";
                string DeskripsiPekerjaan = "";
                string m_strFPTID = this.Request.Params[FPTVM.Prop.FPTID.Name];
                string m_bUnitID = this.Request.Params[FPTVM.Prop.BusinessUnitID.Name];
                string m_strDescriptions = this.Request.Params[FPTVM.Prop.Descriptions.Name];
                string r_TaskID = this.Request.Params[FPTVM.Prop.TaskID.Name];
                string r_TaskStatus = this.Request.Params[FPTVM.Prop.TaskStatus.Name];
                string r_FPTStatus = this.Request.Params[FPTVM.Prop.FPTStatusID.Name];
                string r_TCType = this.Request.Params[FPTVM.Prop.TCType.Name];
                int r_CurrentApprovalLvl = int.Parse(this.Request.Params[FPTVM.Prop.CurrentApprovalLvl.Name]);
                DateTime scheduleDate = DateTime.MinValue; //= "";
                TimeSpan scheduleTime = TimeSpan.MinValue; // = "";
                string m_strSourceNego = string.Empty;

                List<string> m_strNegotiationSchedule = new List<string>();
                if (!string.IsNullOrEmpty(this.Request.Params["ScheduleDate"]))
                    scheduleDate = DateTime.Parse(this.Request.Params["ScheduleDate"]);
                //if (!string.IsNullOrEmpty(this.Request.Params["ScheduleTimeHour"]))
                //    scheduleTime = TimeSpan.Parse(this.Request.Params["ScheduleTimeHour"]);

                if (!string.IsNullOrEmpty(this.Request.Params["ScheduleDate"]) && !string.IsNullOrEmpty(this.Request.Params["ScheduleTimeHour"]))
                {
                    DateTime m_strNegotiationScheduleDate = DateTime.Parse(this.Request.Params["ScheduleDate"]);
                    DateTime m_strNegotiationScheduleDatetm = DateTime.Parse(this.Request.Params["TimeSetting"]);
                    TimeSpan m_strNegotiationScheduleTime = new TimeSpan(m_strNegotiationScheduleDatetm.Hour, m_strNegotiationScheduleDatetm.Minute, 0);
                    m_strNegotiationSchedule.Add((m_strNegotiationScheduleDate.Add(m_strNegotiationScheduleTime)).ToString(Global.SqlDateFormat));
                }

                List<string> m_strNegoLevel = new List<string>() { this.Request.Params[FPTVM.Prop.NegoLevel.Name] };
                List<string> m_strNegoRound = new List<string>() { this.Request.Params[FPTVM.Prop.NegoRound.Name] };
                List<string> m_strRoundTime = new List<string>() { this.Request.Params[FPTVM.Prop.NegoRoundTime.Name] };
                List<string> m_strNegoLeader = new List<string>() { this.Request.Params[FPTVM.Prop.TRMLead.Name] };
                List<BudgetPlanVM> listBudgetPlan = new List<BudgetPlanVM>();
                List<VendorVM> d_lstVendorParticipant = new List<VendorVM>();
                List<VendorVM> GroupedVendorParticipant = new List<VendorVM>();
                List<string> projectPlan = new List<string>();
                List<string> grpVendorParticipant = new List<string>();
                List<string> d_lstTCMembers = new List<string>();
                List<FPTVendorParticipantsVM> lstFPTVendorParticipant = new List<FPTVendorParticipantsVM>();
                List<NegotiationBidStructuresVM> lstNegoBidStructureUplaod = new List<NegotiationBidStructuresVM>();
                List<NegotiationBidStructuresVM> m_lstNegoBidStructureCart = new List<NegotiationBidStructuresVM>();
                List<NegotiationBidEntryVM> lstNegoBidEntrys = new List<NegotiationBidEntryVM>();
                List<CartItemVM> m_ListCartItem = new List<CartItemVM>();
                m_ListCartItem = JSON.Deserialize<List<CartItemVM>>(this.Request.Params["ListCartItem"]);
                if (m_ListCartItem.Any())
                {
                    m_strSourceNego = General.EnumName(Enum.NegoConfigSource.Cart);
                    int seq = 0;
                    decimal m_decVal=0, m_decsubTotal = 0;
                    foreach (CartItemVM objCartItem in m_ListCartItem)
                    {
                        seq++;
                        NegotiationBidStructuresVM itm = new NegotiationBidStructuresVM();
                        itm.Sequence = seq;
                        itm.ItemID = objCartItem.ItemID;
                        itm.ItemDesc = objCartItem.ItemDesc;
                        itm.BudgetPlanDefaultValue = objCartItem.Qty* objCartItem.Amount;
                        itm.ItemParentID = "0";
                        itm.Version = 0;
                        itm.ParentVersion = 0;
                        itm.NegotiationBidID = objCartItem.CatalogCartItemID;// Guid.NewGuid().ToString().Replace("-","");
                        itm.ParentSequence = 0;
                        itm.VendorID = objCartItem.VendorID;
                        m_decsubTotal += itm.BudgetPlanDefaultValue;
                        m_lstNegoBidStructureCart.Add(itm);
                    }

                    int[] m_sequence = { 7777, 8888, 9999 };
                    foreach (int iseq in m_sequence)
                    {
                        string m_strDesc = string.Empty;
                        if ((int)VendorBidTypes.GrandTotal == iseq)
                        {
                            m_strDesc = "Total";
                            m_decVal = m_decsubTotal;
                        }
                        else if ((int)VendorBidTypes.Fee == iseq) {
                            m_strDesc = "Fee (%)";
                            m_decVal = 0;
                        }
                        else if ((int)VendorBidTypes.AfterFee == iseq)
                        {
                            m_strDesc = "Grand Total";
                            m_decVal = 0;
                        }

                        NegotiationBidStructuresVM itm = new NegotiationBidStructuresVM();
                        itm.Sequence = iseq;
                        itm.ItemID = null;
                        itm.ItemDesc = m_strDesc;
                        itm.BudgetPlanDefaultValue = m_decVal;
                        itm.ItemParentID = null;
                        itm.Version =null;
                        itm.ParentVersion = 0;
                        itm.NegotiationBidID = Guid.NewGuid().ToString().Replace("-", "");//objCartItem.CatalogCartItemID;
                        itm.ParentSequence = 0;
                        

                        m_lstNegoBidStructureCart.Add(itm);
                    }

                }
                else if (this.Request.Params["ListProject"] != null)
                {
                    m_strSourceNego = General.EnumName(Enum.NegoConfigSource.BudgetPlan);
                    Dictionary<string, object>[] m_arrListProjectBPlan = JSON.Deserialize<Dictionary<string, object>[]>(this.Request.Params["ListProject"]);
                    if (m_arrListProjectBPlan == null)
                        m_arrListProjectBPlan = new List<Dictionary<string, object>>().ToArray();
                    foreach (Dictionary<string, object> m_dicProjectBPlan in m_arrListProjectBPlan)
                    {
                        string case_ = m_dicProjectBPlan["itemtype"].ToString();
                        switch (case_)
                        {
                            case "VendorID":
                                VendorVM vnd_ = new VendorVM();
                                vnd_.VendorID = m_dicProjectBPlan["value"].ToString();
                                GroupedVendorParticipant.Add(vnd_);
                                d_lstVendorParticipant.Add(vnd_);
                                break;
                            case "BudgetPlanID":
                                BudgetPlanVM BPVM = new BudgetPlanVM();
                                BPVM.BudgetPlanID = m_dicProjectBPlan["value"].ToString();
                                BPVM.TCProjectType = m_bUnitID;//m_dicProjectBPlan["tctype"].ToString();
                                BPVM.VendorParticipant = new List<VendorVM>();
                                BPVM.VendorParticipant = d_lstVendorParticipant;
                                d_lstVendorParticipant = new List<VendorVM>();
                                projectPlan.Add(BPVM.BudgetPlanID);
                                listBudgetPlan.Add(BPVM);
                                break;
                        }
                    }
                    var grpVendor = GroupedVendorParticipant
                                    .GroupBy(u => u.VendorID)
                                        .Select(grp => grp.ToList())
                                        .ToList();
                    foreach (var v in grpVendor)
                        grpVendorParticipant.Add(v[0].VendorID);


                    if (listBudgetPlan.Count == 1)
                        if (this.Request.Params["ListUpload"] != null && string.IsNullOrEmpty(listBudgetPlan[0].BudgetPlanID))
                        {
                            Dictionary<string, object>[] m_arrListTCmembers = JSON.Deserialize<Dictionary<string, object>[]>(this.Request.Params["ListUpload"]);
                            if (m_arrListTCmembers == null)
                                m_arrListTCmembers = new List<Dictionary<string, object>>().ToArray();
                            foreach (Dictionary<string, object> m_dicNegoConfigVM in m_arrListTCmembers)
                            {
                                if (string.IsNullOrEmpty(m_dicNegoConfigVM[ConvertFirstCharToLower(NegotiationBidStructuresVM.Prop.Sequence.Name)].ToString()))
                                { }
                                else
                                {
                                    NegotiationBidStructuresVM itm = new NegotiationBidStructuresVM();
                                    itm.Sequence = m_dicNegoConfigVM[ConvertFirstCharToLower(NegotiationBidStructuresVM.Prop.Sequence.Name)] == null ? 0 : int.Parse(m_dicNegoConfigVM[ConvertFirstCharToLower(NegotiationBidStructuresVM.Prop.Sequence.Name)].ToString());
                                    if (itm.Sequence != 99999 && itm.Sequence != 99998)
                                    {
                                        itm.ItemID = m_dicNegoConfigVM[ConvertFirstCharToLower(NegotiationBidStructuresVM.Prop.ItemID.Name)].ToString();
                                        itm.ItemDesc = m_dicNegoConfigVM[ConvertFirstCharToLower(NegotiationBidStructuresVM.Prop.ItemDesc.Name)].ToString();
                                        itm.BudgetPlanDefaultValue = decimal.Parse(m_dicNegoConfigVM[ConvertFirstCharToLower(NegotiationBidStructuresVM.Prop.BudgetPlanDefaultValue.Name)].ToString());
                                        itm.ItemParentID = m_dicNegoConfigVM[ConvertFirstCharToLower(NegotiationBidStructuresVM.Prop.ItemParentID.Name)] == null ? "0" : m_dicNegoConfigVM[ConvertFirstCharToLower(NegotiationBidStructuresVM.Prop.ItemParentID.Name)].ToString();
                                        itm.Version = 0;
                                        itm.ParentVersion = 0;
                                        itm.NegotiationBidID = m_dicNegoConfigVM[ConvertFirstCharToLower(NegotiationBidStructuresVM.Prop.NegotiationBidID.Name)].ToString();
                                        itm.ParentSequence = m_dicNegoConfigVM[ConvertFirstCharToLower(NegotiationBidStructuresVM.Prop.ParentSequence.Name)] == null ? 0 : int.Parse(m_dicNegoConfigVM[ConvertFirstCharToLower(NegotiationBidStructuresVM.Prop.ParentSequence.Name)].ToString());
                                        lstNegoBidStructureUplaod.Add(itm);
                                    }
                                    else
                                    {
                                        if (itm.Sequence == 99999)
                                            ProjectName = m_dicNegoConfigVM[ConvertFirstCharToLower(NegotiationBidStructuresVM.Prop.ItemDesc.Name)].ToString();
                                        else if (itm.Sequence == 99998)
                                            DeskripsiPekerjaan = m_dicNegoConfigVM[ConvertFirstCharToLower(NegotiationBidStructuresVM.Prop.ItemDesc.Name)].ToString();
                                    }
                                }
                            }
                        }
                }
                bool fromUpload = lstNegoBidStructureUplaod.Count > 0;
                if (fromUpload)
                {
                    m_strSourceNego = General.EnumName(Enum.NegoConfigSource.Upload);
                    if (this.Request.Params["ListBidEntry"] != null)
                    {
                        List<Dictionary<string, object>>[] m_arrListTCmembers = JSON.Deserialize<List<Dictionary<string, object>>[]>(this.Request.Params["ListBidEntry"]);
                        if (m_arrListTCmembers == null)
                            m_arrListTCmembers = new List<List<Dictionary<string, object>>>().ToArray();
                        foreach (List<Dictionary<string, object>> m_dicNegoConfigVM in m_arrListTCmembers)
                        {
                            foreach (Dictionary<string, object> objBid in m_dicNegoConfigVM)
                            {
                                NegotiationBidEntryVM obj = new NegotiationBidEntryVM();
                                obj.NegotiationBidID = objBid["negotiationBidID"].ToString();
                                obj.VendorID = objBid["vendorID"].ToString();
                                obj.BidValue = decimal.Parse(objBid["bidValue"].ToString());
                                lstNegoBidEntrys.Add(obj);
                            }
                        }
                    }
                }
                if (this.Request.Params["ListTCmembers"] != null)
                {
                    Dictionary<string, object>[] m_arrListTCmembers = JSON.Deserialize<Dictionary<string, object>[]>(this.Request.Params["ListTCMembers"]);
                    if (m_arrListTCmembers == null)
                        m_arrListTCmembers = new List<Dictionary<string, object>>().ToArray();
                    foreach (Dictionary<string, object> m_dicTCMemberVM in m_arrListTCmembers)
                    {
                        if (string.IsNullOrEmpty(m_dicTCMemberVM[TCMembersVM.Prop.TCMemberID.Name].ToString()))
                            break;
                        else
                            d_lstTCMembers.Add(m_dicTCMemberVM[TCMembersVM.Prop.TCMemberID.Name].ToString());
                    }
                }
                bool isRenego = !string.IsNullOrEmpty(r_FPTStatus) ? int.Parse(r_FPTStatus) == General.EnumOrder(FPTStatusTypes.ReNegotiation) : false;
                //TODO: validated by source data
                m_lstMessage = IsSaveValid(isRenego, Action, m_strFPTID, m_strNegoRound[0], m_strNegoLevel[0], scheduleDate, scheduleTime, m_strNegoLeader[0], m_strRoundTime[0], listBudgetPlan, d_lstTCMembers);
                Dictionary<string, List<string>> ConfigType = new Dictionary<string, List<string>>();

                if (m_lstMessage.Count <= 0)
                {
                    //SIMPLE KEY VALUE PAIR                   
                    ConfigType.Add(General.EnumDesc(NegoConfigTypes.Round), m_strNegoRound);
                    ConfigType.Add(General.EnumDesc(NegoConfigTypes.Schedule), m_strNegotiationSchedule);
                    //ConfigType.Add(General.EnumDesc(NegoConfigTypes.Project), projectPlan);
                    ConfigType.Add(General.EnumDesc(NegoConfigTypes.RoundTime), m_strRoundTime);
                    ConfigType.Add(General.EnumDesc(NegoConfigTypes.SubItemLevel), m_strNegoLevel);
                    ConfigType.Add(General.EnumDesc(NegoConfigTypes.TCMember), d_lstTCMembers);
                    ConfigType.Add(General.EnumDesc(NegoConfigTypes.TRMLead), m_strNegoLeader);
                    ConfigType.Add(General.EnumDesc(NegoConfigTypes.Vendor), grpVendorParticipant);
                    ConfigType.Add(General.EnumDesc(NegoConfigTypes.Source), new List<string> { m_strSourceNego });
                }
                #endregion

                #region Check If Re-Negotiation
                if (m_lstMessage.Count <= 0 && isRenego)
                {
                    //Update NegoConfig Here
                    if (Action == General.EnumDesc(Buttons.ButtonUpdate))
                    {
                        //Get Config ID First       
                        List<string> m_lstSelect = new List<string>();
                        List<string> m_lstNegoConfigID = new List<string>();
                        List<string> m_lstFilterFPTParticipantID = new List<string>();
                        m_lstSelect.Add(NegotiationConfigurationsVM.Prop.NegotiationConfigID.MapAlias);
                        m_lstSelect.Add(NegotiationConfigurationsVM.Prop.NegotiationConfigTypeID.MapAlias);

                        Dictionary<string, List<object>> m_objFilter_ = new Dictionary<string, List<object>>();
                        List<object> m_lstFilter_ = new List<object>();

                        m_lstFilter_.Add(Operator.Equals);
                        m_lstFilter_.Add(r_TaskID);
                        m_objFilter_.Add(NegotiationConfigurationsVM.Prop.TaskID.Map, m_lstFilter_);

                        Dictionary<int, DataSet> m_dicNegotiationConfigurationsDA = m_objNegoConfigDA.SelectBC(0, null, false, m_lstSelect, m_objFilter_, null, null, null, null);
                        if (m_objNegoConfigDA.Success)
                        {
                            m_objFilter_ = new Dictionary<string, List<object>>();
                            m_lstFilter_ = new List<object>();
                            m_lstFilter_.Add(Operator.Equals);
                            m_lstFilter_.Add(r_TaskID);
                            m_objFilter_.Add(NegotiationConfigurationsVM.Prop.TaskID.Map, m_lstFilter_);
                            m_lstFilter_ = new List<object>();
                            m_lstFilter_.Add(Operator.Equals);
                            m_lstFilter_.Add(General.EnumDesc(NegoConfigTypes.TCMember));
                            m_objFilter_.Add(NegotiationConfigurationsVM.Prop.NegotiationConfigTypeID.Map, m_lstFilter_);
                            m_objNegoConfigDA.DeleteBC(m_objFilter_, true, m_objDBConnection);

                            foreach (KeyValuePair<string, List<string>> m_kvpSelectedRow in ConfigType.Where(x => x.Key == General.EnumDesc(NegoConfigTypes.TCMember)))
                            {
                                foreach (string kvp in m_kvpSelectedRow.Value)
                                {
                                    m_objNegoConfigDA.Data = m_objNegoConfig;
                                    m_objNegoConfig.FPTID = m_strFPTID;
                                    m_objNegoConfig.NegotiationConfigTypeID = m_kvpSelectedRow.Key;
                                    m_objNegoConfig.TaskID = r_TaskID;
                                    m_objNegoConfig.ParameterValue = kvp;
                                    m_objNegoConfigDA.Insert(true, m_objDBConnection);
                                    if (!m_objNegoConfigDA.Success)
                                    {
                                        m_lstMessage.Add(m_objNegoConfigDA.Message);
                                        break;
                                    }
                                }
                            }
                            foreach (DataRow m_drNegotiationConfigurationsDA in m_dicNegotiationConfigurationsDA[0].Tables[0].Rows)
                            {
                                foreach (KeyValuePair<string, List<string>> m_kvpSelectedRow in ConfigType.Where(x => x.Key == m_drNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.NegotiationConfigTypeID.Name].ToString() &&
                                x.Key != General.EnumDesc(NegoConfigTypes.Vendor) && x.Key != General.EnumDesc(NegoConfigTypes.TCMember)))
                                {
                                    foreach (string kvp in m_kvpSelectedRow.Value)
                                    {
                                        m_objNegoConfig = new CNegotiationConfigurations();
                                        m_objNegoConfig.NegotiationConfigID = m_drNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.NegotiationConfigID.Name].ToString();
                                        m_objNegoConfigDA.Data = m_objNegoConfig;
                                        m_objNegoConfigDA.Select();
                                        m_objNegoConfig.ParameterValue = kvp;
                                        m_objNegoConfigDA.Update(true, m_objDBConnection);
                                        if (!m_objNegoConfigDA.Success)
                                        {
                                            m_lstMessage.Add(m_objNegoConfigDA.Message);
                                            break;
                                        }
                                    }
                                }
                                if (m_lstMessage.Count > 0)
                                    break;
                            }
                        }
                    }
                    else
                        m_lstMessage.Add("Cannot Add if Re-Negotiation");

                    if (m_lstMessage.Count <= 0)
                    {
                        m_objNegoConfigDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
                        Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
                        return Detail(General.EnumDesc(Buttons.ButtonSave), null);
                    }

                    m_objNegoConfigDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                    Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
                    return this.Direct(true);
                }
                #endregion

                if (m_lstMessage.Count <= 0)
                {
                    #region MTasks
                    string UserID = System.Web.HttpContext.Current.User.Identity.Name;

                    CApprovalPathDA apPathDA = new CApprovalPathDA();
                    apPathDA.ConnectionStringName = Global.ConnStrConfigName;

                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(General.EnumDesc(TaskType.NegotiationConfigurations));
                    m_objFilter.Add(ApprovalPathVM.Prop.TaskTypeID.Map, m_lstFilter);

                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(string.Empty);
                    m_objFilter.Add(ApprovalPathVM.Prop.RoleChildID.Map, m_lstFilter);

                    List<string> m_lstSelect = new List<string>();
                    m_lstSelect.Add(ApprovalPathVM.Prop.RoleID.MapAlias);

                    Dictionary<string, OrderDirection> m_dicOrderBy = new Dictionary<string, OrderDirection>();
                    // m_dicOrderBy.Add(UserRoleVM.Prop.CreatedDate.Map, OrderDirection.Ascending);

                    Dictionary<int, DataSet> m_dicCAppPathDA = apPathDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null, null);
                    string RoleIDUser = "";
                    if (string.IsNullOrEmpty(apPathDA.Message))
                        RoleIDUser = m_dicCAppPathDA[0].Tables[0].Rows[0][ApprovalPathVM.Prop.RoleID.Name].ToString();

                    string message_ = "";
                    bool isAllowedToCreateConfig = CheckMatchedRole(RoleIDUser, ref message_);
                    if (message_.Length > 0)
                        m_lstMessage.Add(message_);

                    //string ParentApproval = GetParentApproval(ref message_);

                    m_objMTasksDA.Data = m_objMTask;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd) && string.IsNullOrEmpty(message_))
                    {
                        m_objMTask.TaskTypeID = General.EnumDesc(TaskType.NegotiationConfigurations);
                        m_objMTask.TaskDateTimeStamp = DateTime.Now;
                        m_objMTask.TaskOwnerID = RoleIDUser;
                        m_objMTask.StatusID = 4;
                        m_objMTask.CurrentApprovalLvl = 0;
                        m_objMTask.Remarks = "New From Negotiation Configurations";
                        m_objMTasksDA.Insert(true, m_objDBConnection);
                    }
                    else if (Action == General.EnumDesc(Buttons.ButtonUpdate) && string.IsNullOrEmpty(message_))
                    {
                        m_objMTask.TaskID = r_TaskID;
                        m_objMTasksDA.Select();
                        m_objMTask.TaskDateTimeStamp = DateTime.Now;
                        m_objMTask.StatusID = int.Parse(r_TaskStatus);
                        m_objMTask.TaskOwnerID = RoleIDUser;
                        m_objMTask.CurrentApprovalLvl = 0;
                        m_objMTask.Remarks = "Update From Negotiation Configurations";
                        m_objMTasksDA.Update(true, m_objDBConnection);
                    }
                    else
                        m_lstMessage.Add(message_);
                    #endregion

                    #region DTaskDetails
                    if (m_objMTasksDA.Success && m_lstMessage.Count <= 0)
                    {
                        m_objDTaskDetailsDA.Data = m_obTaskDetail;
                        if (Action == General.EnumDesc(Buttons.ButtonAdd))
                            r_TaskID = m_objMTasksDA.Data.TaskID;

                        m_obTaskDetail.TaskDetailID = Guid.NewGuid().ToString().Replace("-", "");
                        m_obTaskDetail.TaskID = r_TaskID;
                        m_obTaskDetail.StatusID = 4;
                        m_obTaskDetail.Remarks = "Draft Created Negotiation Configurations";
                        m_objDTaskDetailsDA.Insert(true, m_objDBConnection);
                    }
                    #endregion

                    #region DFPTStatus
                    if (m_objDTaskDetailsDA.Success)
                    {
                        m_objDFPTStatusDA.Data = m_objDFPTStatus;
                        if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        {
                            m_objDFPTStatus.FPTID = m_strFPTID;
                            m_objDFPTStatus.StatusDateTimeStamp = DateTime.Now;
                            m_objDFPTStatus.StatusID = 8; //Draft
                            m_objDFPTStatusDA.Insert(true, m_objDBConnection);
                        }
                        else
                        {
                            m_objDFPTStatus.FPTID = m_strFPTID;
                            m_objDFPTStatus.StatusDateTimeStamp = DateTime.Now;
                            m_objDFPTStatus.StatusID = 8; ///Draft
                            m_objDFPTStatusDA.Insert(true, m_objDBConnection);
                        }

                    }
                    else
                        m_lstMessage.Add(m_objDTaskDetailsDA.Message);
                    #endregion

                    #region Nego Configurations & FPTVendorParticipants
                    //Delete Insert Method
                    if (m_objDFPTStatusDA.Success)
                    {
                        //Delete first if Update
                        if (Action == General.EnumDesc(Buttons.ButtonUpdate))
                        {
                            /*Select For Delete Filter*/
                            m_lstSelect = new List<string>();
                            List<string> m_lstNegoConfigID = new List<string>();
                            List<string> m_lstFilterFPTParticipantID = new List<string>();
                            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.NegotiationConfigID.MapAlias);
                            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.FPTVendorParticipantID.MapAlias);

                            Dictionary<string, List<object>> m_objFilter_ = new Dictionary<string, List<object>>();
                            List<object> m_lstFilter_ = new List<object>();

                            m_lstFilter_.Add(Operator.Equals);
                            m_lstFilter_.Add(r_TaskID);
                            m_objFilter_.Add(NegotiationConfigurationsVM.Prop.TaskID.Map, m_lstFilter_);
                            m_lstFilter_ = new List<object>();
                            m_lstFilter_.Add(Operator.Equals);
                            m_lstFilter_.Add(General.EnumDesc(NegoConfigTypes.BudgetPlan));
                            m_objFilter_.Add(NegotiationConfigurationsVM.Prop.NegotiationConfigTypeID.Map, m_lstFilter_);

                            Dictionary<int, DataSet> m_dicNegotiationConfigurationsDA = m_objDFPTVendorParticipantsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter_, null, null, null, null);
                            if (m_objDFPTVendorParticipantsDA.Success)
                                foreach (DataRow m_drNegotiationConfigurationsDA in m_dicNegotiationConfigurationsDA[0].Tables[0].Rows)
                                {
                                    m_lstFilterFPTParticipantID.Add(m_drNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.FPTVendorParticipantID.Name].ToString());
                                    m_lstNegoConfigID.Add(m_drNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.NegotiationConfigID.Name].ToString());
                                }


                            /*Delete NegoBidEntry First Causing Constraint to Bidstructure*/
                            Dictionary<string, List<object>> m_objFilterBidStructure = new Dictionary<string, List<object>>();
                            List<object> m_lstFilterBids = new List<object>();
                            m_lstFilterBids.Add(Operator.In);
                            m_lstFilterBids.Add(String.Join(",", m_lstFilterFPTParticipantID.Distinct()));
                            m_objFilterBidStructure.Add(NegotiationBidEntryVM.Prop.FPTVendorParticipantID.Map, m_lstFilterBids);
                            m_objNegoBidEntryDA.DeleteBC(m_objFilterBidStructure, true, m_objDBConnection);

                            if (m_objNegoBidEntryDA.Success)
                            {
                                /*Delete Bidstructure First Causing Constraint*/
                                m_objFilterBidStructure = new Dictionary<string, List<object>>();
                                m_lstFilterBids = new List<object>();
                                m_lstFilterBids.Add(Operator.In);
                                m_lstFilterBids.Add(String.Join(",", m_lstNegoConfigID.Distinct()));
                                m_objFilterBidStructure.Add(NegotiationBidStructuresVM.Prop.NegotiationConfigID.Map, m_lstFilterBids);
                                m_objTNegoBidStructuresDA.DeleteBC(m_objFilterBidStructure, true, m_objDBConnection);
                            }
                            if (m_objTNegoBidStructuresDA.Success)
                            {
                                /*Delete FPTParticipants*/
                                m_objFilter_ = new Dictionary<string, List<object>>();
                                m_lstFilter_ = new List<object>();
                                m_lstFilter_.Add(Operator.In);
                                m_lstFilter_.Add(String.Join(",", m_lstNegoConfigID.Distinct()));
                                m_objFilter_.Add(FPTVendorParticipantsVM.Prop.NegotiationConfigID.Map, m_lstFilter_);
                                m_objDFPTVendorParticipantsDA.DeleteBC(m_objFilter_, true, m_objDBConnection);
                            }
                            if (m_objTNegoBidStructuresDA.Success)
                            {
                                /*Delete FPTParticipants*/
                                m_objFilter_ = new Dictionary<string, List<object>>();
                                m_lstFilter_ = new List<object>();
                                m_lstFilter_.Add(Operator.In);
                                m_lstFilter_.Add(m_strFPTID);
                                m_objFilter_.Add(FPTTCParticipantsVM.Prop.FPTID.Map, m_lstFilter_);
                                m_objDFPTTCParticipantsDA.DeleteBC(m_objFilter_, true, m_objDBConnection);
                            }
                            if (m_objDFPTVendorParticipantsDA.Success)
                            {
                                /*Delete NegoConfig*/
                                m_objFilter_ = new Dictionary<string, List<object>>();
                                m_lstFilter_ = new List<object>();
                                m_lstFilter_.Add(Operator.Equals);
                                m_lstFilter_.Add(r_TaskID);
                                m_objFilter_.Add(NegotiationConfigurationsVM.Prop.TaskID.Map, m_lstFilter_);
                                m_objNegoConfigDA.DeleteBC(m_objFilter_, true, m_objDBConnection);
                            }
                        }

                        //Insert Wheater Update or Add
                        if (string.IsNullOrEmpty(m_objNegoConfigDA.Message))
                        {
                            //Insert for all NecoConfigType except BPlan
                            foreach (KeyValuePair<string, List<string>> m_kvpSelectedRow in ConfigType)
                            {
                                foreach (string kvp in m_kvpSelectedRow.Value)
                                {

                                    m_objNegoConfig = new CNegotiationConfigurations();
                                    m_objNegoConfigDA.Data = m_objNegoConfig;
                                    m_objNegoConfig.FPTID = m_strFPTID;
                                    m_objNegoConfig.NegotiationConfigTypeID = m_kvpSelectedRow.Key;
                                    m_objNegoConfig.TaskID = m_objMTasksDA.Data.TaskID;
                                    m_objNegoConfig.ParameterValue = kvp;
                                    if (fromUpload)
                                    {
                                        if (m_objNegoConfig.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.Round))
                                            m_objNegoConfig.ParameterValue2 = ProjectName;
                                        else if (m_kvpSelectedRow.Key == General.EnumDesc(NegoConfigTypes.RoundTime))
                                            m_objNegoConfig.ParameterValue2 = DeskripsiPekerjaan;
                                        else
                                            m_objNegoConfig.ParameterValue2 = "";
                                    }
                                    m_objNegoConfigDA.Insert(true, m_objDBConnection);
                                }
                                if (!m_objNegoConfigDA.Success)
                                    break;
                            }

                            //Insert NegoConfigType BPlan
                            int F = 0;
                            foreach (BudgetPlanVM BPVM in listBudgetPlan)
                            {
                                m_objNegoConfig.FPTID = m_strFPTID;
                                m_objNegoConfig.NegotiationConfigTypeID = General.EnumDesc(NegoConfigTypes.BudgetPlan);
                                m_objNegoConfig.TaskID = m_objMTasksDA.Data.TaskID;
                                m_objNegoConfig.ParameterValue = fromUpload ? "" : BPVM.BudgetPlanID;
                                m_objNegoConfig.ParameterValue2 = m_bUnitID;//BPVM.TCProjectType;
                                m_objNegoConfigDA.Insert(true, m_objDBConnection);
                                if (!m_objNegoConfigDA.Success)
                                    break;
                                listBudgetPlan[F].NegoConfigID = m_objNegoConfigDA.Data.NegotiationConfigID;
                                F++;

                                //Create FPTVendorParticipants
                                foreach (VendorVM VN in BPVM.VendorParticipant)
                                {
                                    FPTVendorParticipantsVM fpn = new FPTVendorParticipantsVM();
                                    m_objDFPTVendorParticipants = new DFPTVendorParticipants();

                                    m_objDFPTVendorParticipantsDA.Data = m_objDFPTVendorParticipants;
                                    m_objDFPTVendorParticipants.FPTVendorParticipantID = Guid.NewGuid().ToString().Replace("-", "");
                                    m_objDFPTVendorParticipants.NegotiationConfigID = m_objNegoConfigDA.Data.NegotiationConfigID;
                                    m_objDFPTVendorParticipants.VendorID = VN.VendorID;
                                    m_objDFPTVendorParticipants.StatusID = "1";
                                    //Active

                                    //Populating VendorParticipant
                                    fpn.FPTVendorParticipantID = m_objDFPTVendorParticipants.FPTVendorParticipantID;
                                    //fpn.NegotiationConfigID = m_objDFPTVendorParticipants.NegotiationConfigID;
                                    fpn.VendorID = VN.VendorID;
                                    fpn.BudgetPlanID = BPVM.BudgetPlanID;
                                    lstFPTVendorParticipant.Add(fpn);

                                    m_objDFPTVendorParticipantsDA.Insert(true, m_objDBConnection);
                                    if (!m_objDFPTVendorParticipantsDA.Success)
                                    {

                                        m_lstMessage.Add(m_objDFPTVendorParticipantsDA.Message);
                                        break;
                                    }
                                }
                                if (!m_objDFPTVendorParticipantsDA.Success)
                                    break;
                            }

                            //insert TC participant
                            foreach (string m_strTCMemberID in d_lstTCMembers)
                            {
                                FPTTCParticipantsVM m_objFPTTCParticipantsVM = new FPTTCParticipantsVM();
                                m_objDFPTTCParticipants = new DFPTTCParticipants();

                                m_objDFPTTCParticipantsDA.Data = m_objDFPTTCParticipants;
                                m_objDFPTTCParticipants.FPTTCParticipantID = Guid.NewGuid().ToString().Replace("-", "");
                                m_objDFPTTCParticipants.FPTID = m_strFPTID;
                                m_objDFPTTCParticipants.TCMemberID = m_strTCMemberID;
                                m_objDFPTTCParticipants.StatusID = true;
                                m_objDFPTTCParticipants.IsDelegation = false;

                                m_objDFPTTCParticipantsDA.Insert(true, m_objDBConnection);
                                if (!m_objDFPTTCParticipantsDA.Success)
                                {
                                    m_lstMessage.Add(m_objDFPTTCParticipantsDA.Message);
                                    break;
                                }

                            }

                            //from cart
                            if (m_lstNegoBidStructureCart.Any())
                            {
                                m_objNegoConfig.FPTID = m_strFPTID;
                                m_objNegoConfig.NegotiationConfigTypeID = General.EnumDesc(NegoConfigTypes.BudgetPlan);
                                m_objNegoConfig.TaskID = m_objMTasksDA.Data.TaskID;
                                m_objNegoConfig.ParameterValue = m_ListCartItem.FirstOrDefault().CatalogCartID;
                                m_objNegoConfig.ParameterValue2 = "";//BPVM.TCProjectType;
                                m_objNegoConfigDA.Insert(true, m_objDBConnection);
                                if (!m_objNegoConfigDA.Success)
                                    m_lstMessage.Add(m_objNegoConfigDA.Message);

                                m_lstNegoBidStructureCart.ForEach(d => d.NegotiationConfigID = m_objNegoConfigDA.Data.NegotiationConfigID);
                                //listBudgetPlan[F].NegoConfigID = m_objNegoConfigDA.Data.NegotiationConfigID;
                                //F++;

                                //Create FPTVendorParticipants
                                foreach (VendorVM VN in m_ListCartItem.GroupBy(d => new { d.VendorID, d.VendorDesc }).Select(d => new VendorVM { VendorID = d.Key.VendorID, VendorDesc = d.Key.VendorDesc }).ToList())
                                {
                                    FPTVendorParticipantsVM fpn = new FPTVendorParticipantsVM();
                                    m_objDFPTVendorParticipants = new DFPTVendorParticipants();

                                    m_objDFPTVendorParticipantsDA.Data = m_objDFPTVendorParticipants;
                                    m_objDFPTVendorParticipants.FPTVendorParticipantID = Guid.NewGuid().ToString().Replace("-", "");
                                    m_objDFPTVendorParticipants.NegotiationConfigID = m_objNegoConfigDA.Data.NegotiationConfigID;
                                    m_objDFPTVendorParticipants.VendorID = VN.VendorID;
                                    m_objDFPTVendorParticipants.StatusID = "1";
                                    //Active

                                    //Populating VendorParticipant
                                    fpn.FPTVendorParticipantID = m_objDFPTVendorParticipants.FPTVendorParticipantID;
                                    fpn.NegotiationConfigID = m_objDFPTVendorParticipants.NegotiationConfigID;
                                    fpn.VendorID = VN.VendorID;
                                    //fpn.BudgetPlanID = BPVM.BudgetPlanID;
                                    lstFPTVendorParticipant.Add(fpn);

                                    m_objDFPTVendorParticipantsDA.Insert(true, m_objDBConnection);
                                    if (!m_objDFPTVendorParticipantsDA.Success)
                                    {
                                        m_objDFPTVendorParticipantsDA.Message = "Insert vendor error. Please check registered vendor list or foreign key.";
                                        break;
                                    }
                                }
                                if (!m_objDFPTVendorParticipantsDA.Success)
                                    m_lstMessage.Add(m_objDFPTVendorParticipantsDA.Message); ;
                            }
                        }
                    }
                    else
                        m_lstMessage.Add(m_objDFPTStatusDA.Message);
                    #endregion

                    #region DFPTNegotiationRounds
                    if (m_objDFPTVendorParticipantsDA.Success)
                    {
                        m_objDFPTNegotiationRoundsDA.Data = obj_DFPTNegotiationRounds;
                        /*Delete First IF Update*/
                        if (Action == General.EnumDesc(Buttons.ButtonUpdate))
                        {
                            Dictionary<string, List<object>> m_objFilteDelRound = new Dictionary<string, List<object>>();
                            List<object> m_lstFilterBids = new List<object>();
                            m_lstFilterBids.Add(Operator.Equals);
                            m_lstFilterBids.Add(m_strFPTID);
                            m_objFilteDelRound.Add(FPTNegotiationRoundVM.Prop.FPTID.Map, m_lstFilterBids);
                            m_objDFPTNegotiationRoundsDA.DeleteBC(m_objFilteDelRound, true, m_objDBConnection);
                        }

                        /*Insert wheater Update or Add*/
                        for (int x = 0; x < Convert.ToInt16(m_strNegoRound[0]); x++)
                        {
                            obj_DFPTNegotiationRounds.FPTID = m_strFPTID;
                            obj_DFPTNegotiationRounds.TopValue = 0;
                            obj_DFPTNegotiationRounds.BottomValue = 0;
                            obj_DFPTNegotiationRounds.StartDateTimeStamp = DateTime.Parse(Global.MaxDate);// DateTime.Parse("9999-12-31 00:00:00.000");//TODO
                            obj_DFPTNegotiationRounds.EndDateTimeStamp = DateTime.Parse(Global.MaxDate);//DateTime.Parse("9999-12-31 00:00:00.000");
                            m_objDFPTNegotiationRoundsDA.Insert(true, m_objDBConnection);
                        }
                    }
                    else
                        m_lstMessage.Add(m_objDFPTVendorParticipantsDA.Message);

                    #endregion

                    #region TBidStructure & NegoBidEntry
                    if (m_objDFPTNegotiationRoundsDA.Success)
                    {
                        m_objTNegoBidStructuresDA.Data = m_objTNegoBidStructures;
                        m_objNegoBidEntryDA.Data = m_objNegoBidEntry;
                        /*Insert wheater Update or Add, causing if update already deleted*/
                        if (string.IsNullOrEmpty(m_objTNegoBidStructuresDA.Message))
                        {
                            foreach (BudgetPlanVM objBPVM in listBudgetPlan)
                            {
                                List<BudgetPlanVersionVendorVM> lstBVersionVendorVM = new List<BudgetPlanVersionVendorVM>();
                                List<NegotiationBidEntryVM> lstBidNegoEntryVM = new List<NegotiationBidEntryVM>();
                                List<NegotiationBidStructuresVM> ListBidStructure = new List<NegotiationBidStructuresVM>();
                                List<NegotiationBidStructuresVM> ListBidStructureForBid = new List<NegotiationBidStructuresVM>();
                                int bPlanVersion = 1;
                                ListBidStructure = fromUpload ? lstNegoBidStructureUplaod : GetTBidStructure(objBPVM, ref lstBVersionVendorVM, ref m_lstMessage, ref bPlanVersion);

                                foreach (NegotiationBidStructuresVM obj in ListBidStructure)
                                {
                                    m_objTNegoBidStructures = new TNegotiationBidStructures();
                                    m_objTNegoBidStructuresDA.Data = m_objTNegoBidStructures;
                                    m_objTNegoBidStructures.NegotiationBidID = obj.NegotiationBidID;
                                    m_objTNegoBidStructures.NegotiationConfigID = objBPVM.NegoConfigID;
                                    m_objTNegoBidStructures.ItemID = obj.ItemID;
                                    m_objTNegoBidStructures.ItemParentID = obj.ItemParentID;
                                    m_objTNegoBidStructures.ItemDesc = obj.ItemDesc;
                                    m_objTNegoBidStructures.Sequence = obj.Sequence;
                                    m_objTNegoBidStructures.ParentSequence = obj.ParentSequence;
                                    m_objTNegoBidStructures.Version = obj.Version;
                                    m_objTNegoBidStructures.ParentVersion = obj.ParentVersion;
                                    m_objTNegoBidStructures.BudgetPlanDefaultValue = obj.BudgetPlanDefaultValue;
                                    m_objTNegoBidStructuresDA.Insert(true, m_objDBConnection);
                                    if (!m_objTNegoBidStructuresDA.Success)
                                    {
                                        m_lstMessage.Add(m_objTNegoBidStructuresDA.Message);
                                        break;
                                    }
                                    ListBidStructureForBid.Add(obj);
                                }
                                if (m_lstMessage.Count > 0)
                                    break;

                                //NegotiationBidEntry
                                #region Get Vendor Bid Structure 
                                if (!fromUpload)
                                {
                                    foreach (FPTVendorParticipantsVM FPTParticipant in lstFPTVendorParticipant.Where(x => x.BudgetPlanID == objBPVM.BudgetPlanID))
                                    {
                                        string VersionVendor_ = "";
                                        foreach (BudgetPlanVersionVendorVM vnd in lstBVersionVendorVM.Where(x => x.VendorID == FPTParticipant.VendorID))
                                            VersionVendor_ = vnd.BudgetPlanVersionVendorID;
                                        if (!string.IsNullOrEmpty(VersionVendor_))
                                        {
                                            List<BudgetPlanVersionStructureVM> m_lstStructureBidEntry = new List<BudgetPlanVersionStructureVM>();
                                            DBudgetPlanVersionStructureDA m_objDBudgetPlanVersionStructureDA = new DBudgetPlanVersionStructureDA();
                                            m_objDBudgetPlanVersionStructureDA.ConnectionStringName = Global.ConnStrConfigName;

                                            if (m_lstMessage.Count <= 0)
                                            {
                                                m_lstSelect = new List<string>();
                                                m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanID.MapAlias);
                                                m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersionStructureID.MapAlias);
                                                m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.MapAlias);
                                                m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ItemID.MapAlias);
                                                m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Version.MapAlias);
                                                m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Sequence.MapAlias);
                                                m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ParentItemID.MapAlias);
                                                m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ParentItemDesc.MapAlias);
                                                m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ParentVersion.MapAlias);
                                                m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ParentSequence.MapAlias);
                                                m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Volume.MapAlias);
                                                m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Specification.MapAlias);
                                                m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.MaterialAmount.MapAlias);
                                                m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.WageAmount.MapAlias);
                                                m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.MiscAmount.MapAlias);
                                                m_lstSelect.Add("BIDENTRY.BudgetPlanVersionVendorID as VersionVendor");
                                                m_lstSelect.Add("BIDENTRY.BudgetPlanVersionStructureID as BidBudgetPlanVersionStructureID");
                                                m_lstSelect.Add("BIDENTRY.Volume as BidVolume");
                                                m_lstSelect.Add("BIDENTRY.MaterialAmount as BidMAT");
                                                m_lstSelect.Add("BIDENTRY.WageAmount as BidWAG");
                                                m_lstSelect.Add("BIDENTRY.MiscAmount as BidMISC");

                                                m_objFilter = new Dictionary<string, List<object>>();
                                                m_lstFilter = new List<object>();
                                                m_lstFilter.Add(Operator.Equals);
                                                m_lstFilter.Add(objBPVM.BudgetPlanID);
                                                m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanID.Map, m_lstFilter);

                                                m_lstFilter = new List<object>();
                                                m_lstFilter.Add(Operator.Equals);
                                                m_lstFilter.Add(bPlanVersion);
                                                m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

                                                m_lstFilter = new List<object>();
                                                m_lstFilter.Add(Operator.Equals);
                                                m_lstFilter.Add("BOI");
                                                m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.ItemTypeID.Map, m_lstFilter);

                                                m_dicOrderBy = new Dictionary<string, OrderDirection>();
                                                m_dicOrderBy.Add(BudgetPlanVersionStructureVM.Prop.Sequence.Map, OrderDirection.Ascending);

                                                Dictionary<int, DataSet> m_dicDBudgetPlanVersionStructureDA = m_objDBudgetPlanVersionStructureDA.SelectBC_BidEntry(0, null, false, VersionVendor_, m_lstSelect, m_objFilter, null, null, null);
                                                if (m_objDBudgetPlanVersionStructureDA.Message == string.Empty)
                                                {
                                                    m_lstStructureBidEntry = (
                                                    from DataRow m_drDBudgetPlanVersionStructureDA in m_dicDBudgetPlanVersionStructureDA[0].Tables[0].Rows
                                                    select new BudgetPlanVersionStructureVM()
                                                    {
                                                        BudgetPlanVersionVendorID = string.IsNullOrEmpty(m_drDBudgetPlanVersionStructureDA["VersionVendor"].ToString()) ? "" : m_drDBudgetPlanVersionStructureDA["VersionVendor"].ToString(),
                                                        BidBudgetPlanVersionStructureID = m_drDBudgetPlanVersionStructureDA["BidBudgetPlanVersionStructureID"].ToString(),
                                                        BidVolume = string.IsNullOrEmpty(m_drDBudgetPlanVersionStructureDA["BidVolume"].ToString()) ? 0 : Decimal.Parse(m_drDBudgetPlanVersionStructureDA["BidVolume"].ToString()),
                                                        BidMAT = string.IsNullOrEmpty(m_drDBudgetPlanVersionStructureDA["BidMAT"].ToString()) ? 0 : Decimal.Parse(m_drDBudgetPlanVersionStructureDA["BidMAT"].ToString()),
                                                        BidWAG = string.IsNullOrEmpty(m_drDBudgetPlanVersionStructureDA["BidWAG"].ToString()) ? 0 : Decimal.Parse(m_drDBudgetPlanVersionStructureDA["BidWAG"].ToString()),
                                                        BidMISC = string.IsNullOrEmpty(m_drDBudgetPlanVersionStructureDA["BidMISC"].ToString()) ? 0 : Decimal.Parse(m_drDBudgetPlanVersionStructureDA["BidMISC"].ToString()),
                                                        BudgetPlanVersionStructureID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.BudgetPlanVersionStructureID.Name].ToString(),
                                                        BudgetPlanID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.BudgetPlanID.Name].ToString(),
                                                        BudgetPlanVersion = int.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.Name].ToString()),
                                                        ItemID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ItemID.Name].ToString(),
                                                        Version = int.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Version.Name].ToString()),
                                                        Sequence = int.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Sequence.Name].ToString()),
                                                        ParentItemID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ParentItemID.Name].ToString(),
                                                        ParentItemDesc = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ParentItemDesc.Name].ToString(),
                                                        ParentVersion = int.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ParentVersion.Name].ToString()),
                                                        ParentSequence = int.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ParentSequence.Name].ToString()),
                                                        Volume = decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Volume.Name].ToString()),
                                                        Specification = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Specification.Name].ToString(),
                                                        MaterialAmount = decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name].ToString()),
                                                        WageAmount = decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.WageAmount.Name].ToString()),
                                                        MiscAmount = decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.MiscAmount.Name].ToString())
                                                    }).Distinct().ToList();
                                                }
                                                else
                                                    m_lstMessage.Add(m_objDBudgetPlanVersionStructureDA.Message);
                                            }

                                            //Populate NegoBidEntry
                                            List<NegotiationBidEntryVM> lstNegoBidEntry = new List<NegotiationBidEntryVM>();
                                            decimal GrandTotal = 0;
                                            foreach (BudgetPlanVersionStructureVM obj in m_lstStructureBidEntry.Where(x => x.ParentSequence == 0))
                                            {
                                                NegotiationBidEntryVM objm_objNegoBidEntry = new NegotiationBidEntryVM();
                                                foreach (NegotiationBidStructuresVM objtbid in ListBidStructureForBid.Where(x => x.VersionStructureID == obj.BudgetPlanVersionStructureID))
                                                    objm_objNegoBidEntry.NegotiationBidID = objtbid.NegotiationBidID;

                                                objm_objNegoBidEntry.NegotiationEntryID = Guid.NewGuid().ToString().Replace("-", "");
                                                objm_objNegoBidEntry.FPTVendorParticipantID = FPTParticipant.FPTVendorParticipantID;
                                                objm_objNegoBidEntry.BidTypeID = General.EnumDesc(VendorBidTypes.LastOffer);

                                                decimal bidval_ = 0;
                                                GetChildBidEntryLastOffer(m_lstStructureBidEntry, ListBidStructureForBid, obj, FPTParticipant.FPTVendorParticipantID, ref lstNegoBidEntry, ref m_lstMessage, ref bidval_);
                                                if (bidval_ == 0)
                                                    bidval_ = obj.BidVolume * (obj.BidMAT + obj.BidWAG + obj.BidMISC);

                                                objm_objNegoBidEntry.BidValue = bidval_;
                                                GrandTotal += bidval_;
                                                lstNegoBidEntry.Add(objm_objNegoBidEntry);
                                            }
                                            //Total before fee
                                            NegotiationBidEntryVM objm_NegoBidEntry = new NegotiationBidEntryVM();
                                            foreach (NegotiationBidStructuresVM objtbid in ListBidStructureForBid.Where(x => x.Sequence == 7777))
                                                objm_NegoBidEntry.NegotiationBidID = objtbid.NegotiationBidID;
                                            objm_NegoBidEntry.NegotiationEntryID = Guid.NewGuid().ToString().Replace("-", "");
                                            objm_NegoBidEntry.FPTVendorParticipantID = FPTParticipant.FPTVendorParticipantID;
                                            objm_NegoBidEntry.BidTypeID = General.EnumDesc(VendorBidTypes.LastOffer);
                                            objm_NegoBidEntry.BidValue = GrandTotal;
                                            lstNegoBidEntry.Add(objm_NegoBidEntry);

                                            //Fee
                                            objm_NegoBidEntry = new NegotiationBidEntryVM();
                                            decimal fee_ = 0;
                                            foreach (NegotiationBidStructuresVM objtbid in ListBidStructureForBid.Where(x => x.Sequence == 8888))
                                                objm_NegoBidEntry.NegotiationBidID = objtbid.NegotiationBidID;
                                            fee_ = lstBVersionVendorVM.Where(p => p.VendorID.Trim() == FPTParticipant.VendorID.Trim()).FirstOrDefault().FeePercentage??0;
                                            objm_NegoBidEntry.NegotiationEntryID = Guid.NewGuid().ToString().Replace("-", "");
                                            objm_NegoBidEntry.FPTVendorParticipantID = FPTParticipant.FPTVendorParticipantID;
                                            objm_NegoBidEntry.BidTypeID = General.EnumDesc(VendorBidTypes.Fee);
                                            objm_NegoBidEntry.BidValue = fee_;
                                            lstNegoBidEntry.Add(objm_NegoBidEntry);

                                            //Total after fee
                                            objm_NegoBidEntry = new NegotiationBidEntryVM();
                                            foreach (NegotiationBidStructuresVM objtbid in ListBidStructureForBid.Where(x => x.Sequence == 9999))
                                                objm_NegoBidEntry.NegotiationBidID = objtbid.NegotiationBidID;
                                            objm_NegoBidEntry.NegotiationEntryID = Guid.NewGuid().ToString().Replace("-", "");
                                            objm_NegoBidEntry.FPTVendorParticipantID = FPTParticipant.FPTVendorParticipantID;
                                            objm_NegoBidEntry.BidTypeID = General.EnumDesc(VendorBidTypes.AfterFee);
                                            objm_NegoBidEntry.BidValue = (fee_/100 * GrandTotal) + GrandTotal;
                                            lstNegoBidEntry.Add(objm_NegoBidEntry);

                                            //Insert NegoBidEntry
                                            foreach (NegotiationBidEntryVM obj in lstNegoBidEntry)
                                            {
                                                m_objNegoBidEntry = new DNegotiationBidEntry();
                                                m_objNegoBidEntryDA.Data = m_objNegoBidEntry;
                                                m_objNegoBidEntry.NegotiationBidID = obj.NegotiationBidID;
                                                m_objNegoBidEntry.NegotiationEntryID = obj.NegotiationEntryID;
                                                m_objNegoBidEntry.FPTVendorParticipantID = obj.FPTVendorParticipantID;
                                                m_objNegoBidEntry.BidTypeID = obj.BidTypeID;
                                                m_objNegoBidEntry.BidValue = obj.BidValue;

                                                m_objNegoBidEntryDA.Insert(true, m_objDBConnection);
                                                if (!m_objNegoBidEntryDA.Success)
                                                {
                                                    m_lstMessage.Add(m_objNegoBidEntryDA.Message);
                                                    break;
                                                }
                                            }
                                            if (m_lstMessage.Count > 0)
                                                break;
                                        }
                                        else
                                            m_lstMessage.Add("BidEntry Insert : VersionVendor Didn't match");
                                    }
                                }
                                else //IF From Upload
                                {
                                    foreach (FPTVendorParticipantsVM FPTParticipant in lstFPTVendorParticipant.Where(x => x.BudgetPlanID == objBPVM.BudgetPlanID))
                                    {
                                        foreach (NegotiationBidEntryVM objBid in lstNegoBidEntrys.Where(x => x.VendorID == FPTParticipant.VendorID))
                                        {
                                            m_objNegoBidEntry = new DNegotiationBidEntry();
                                            m_objNegoBidEntryDA.Data = m_objNegoBidEntry;
                                            m_objNegoBidEntry.NegotiationBidID = objBid.NegotiationBidID;
                                            m_objNegoBidEntry.NegotiationEntryID = Guid.NewGuid().ToString().Replace("-", "");
                                            m_objNegoBidEntry.FPTVendorParticipantID = FPTParticipant.FPTVendorParticipantID;
                                            string bidType = lstNegoBidStructureUplaod.Any(d => d.NegotiationBidID == objBid.NegotiationBidID && d.Sequence == (int)VendorBidTypes.GrandTotal) ?
                                                              General.EnumDesc(VendorBidTypes.GrandTotal) : lstNegoBidStructureUplaod.Any(d => d.NegotiationBidID == objBid.NegotiationBidID && d.Sequence == (int)VendorBidTypes.Fee) ? General.EnumDesc(VendorBidTypes.Fee) :
                                                              lstNegoBidStructureUplaod.Any(d => d.NegotiationBidID == objBid.NegotiationBidID && d.Sequence == (int)VendorBidTypes.AfterFee) ? General.EnumDesc(VendorBidTypes.AfterFee) : General.EnumDesc(VendorBidTypes.LastOffer);
                                            m_objNegoBidEntry.BidTypeID = bidType;
                                            m_objNegoBidEntry.BidValue = objBid.BidValue;

                                            m_objNegoBidEntryDA.Insert(true, m_objDBConnection);
                                            if (!m_objNegoBidEntryDA.Success)
                                            {
                                                m_lstMessage.Add(m_objNegoBidEntryDA.Message);
                                                break;
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }

                            //from cart
                            if (m_lstNegoBidStructureCart.Any())
                            {
                                List<NegotiationBidEntryVM> lstCartNegoBidEntry = new List<NegotiationBidEntryVM>();
                                foreach (NegotiationBidStructuresVM obj in m_lstNegoBidStructureCart)
                                {

                                    m_objTNegoBidStructures = new TNegotiationBidStructures();
                                    m_objTNegoBidStructuresDA.Data = m_objTNegoBidStructures;
                                    m_objTNegoBidStructures.NegotiationBidID = obj.NegotiationBidID;
                                    m_objTNegoBidStructures.NegotiationConfigID = obj.NegotiationConfigID;
                                    m_objTNegoBidStructures.ItemID = obj.ItemID;
                                    m_objTNegoBidStructures.ItemParentID = obj.ItemParentID;
                                    m_objTNegoBidStructures.ItemDesc = obj.ItemDesc;
                                    m_objTNegoBidStructures.Sequence = obj.Sequence;
                                    m_objTNegoBidStructures.ParentSequence = obj.ParentSequence;
                                    m_objTNegoBidStructures.Version = obj.Version;
                                    m_objTNegoBidStructures.ParentVersion = obj.ParentVersion;
                                    m_objTNegoBidStructures.BudgetPlanDefaultValue = 0;//obj.BudgetPlanDefaultValue;

                                    m_objTNegoBidStructuresDA.Insert(true, m_objDBConnection);
                                    if (!m_objTNegoBidStructuresDA.Success)
                                    {
                                        m_lstMessage.Add(m_objTNegoBidStructuresDA.Message);
                                        break;
                                    }
                                    if (m_lstMessage.Count > 0)
                                        break;
                                    
                                }
                                //Insert NegoBidEntry
                                foreach (FPTVendorParticipantsVM FPTParticipant in lstFPTVendorParticipant)
                                {
                                    foreach (NegotiationBidStructuresVM obj in m_lstNegoBidStructureCart.Where(d=>d.VendorID==FPTParticipant.VendorID).ToList())
                                    { 
                                        m_objNegoBidEntry = new DNegotiationBidEntry();
                                        m_objNegoBidEntryDA.Data = m_objNegoBidEntry;
                                        m_objNegoBidEntry.NegotiationBidID = obj.NegotiationBidID;
                                        m_objNegoBidEntry.NegotiationEntryID = Guid.NewGuid().ToString().Replace("-", "");
                                        m_objNegoBidEntry.FPTVendorParticipantID = FPTParticipant.FPTVendorParticipantID;
                                        m_objNegoBidEntry.BidTypeID = General.EnumDesc(VendorBidTypes.LastOffer);
                                        m_objNegoBidEntry.BidValue = obj.BudgetPlanDefaultValue;


                                        m_objNegoBidEntryDA.Insert(true, m_objDBConnection);
                                        if (!m_objNegoBidEntryDA.Success)
                                        {
                                            m_lstMessage.Add(m_objNegoBidEntryDA.Message);
                                            break;
                                        }
                                    }


                                    //Total
                                    m_objNegoBidEntry = new DNegotiationBidEntry();
                                    m_objNegoBidEntryDA.Data = m_objNegoBidEntry;
                                    m_objNegoBidEntry.NegotiationBidID = m_lstNegoBidStructureCart.Where(d=>d.Sequence==(int)VendorBidTypes.GrandTotal).Select(d=>d.NegotiationBidID).FirstOrDefault();
                                    m_objNegoBidEntry.NegotiationEntryID = Guid.NewGuid().ToString().Replace("-", "");
                                    m_objNegoBidEntry.FPTVendorParticipantID = FPTParticipant.FPTVendorParticipantID;
                                    m_objNegoBidEntry.BidTypeID = General.EnumDesc(VendorBidTypes.LastOffer);
                                    m_objNegoBidEntry.BidValue = m_lstNegoBidStructureCart.Where(d => d.VendorID == FPTParticipant.VendorID).Sum(d => d.BudgetPlanDefaultValue);

                                    m_objNegoBidEntryDA.Insert(true, m_objDBConnection);
                                    if (!m_objNegoBidEntryDA.Success)
                                    {
                                        m_lstMessage.Add(m_objNegoBidEntryDA.Message);
                                        break;
                                    }

                                    //Fee
                                    m_objNegoBidEntry = new DNegotiationBidEntry();
                                    m_objNegoBidEntryDA.Data = m_objNegoBidEntry;
                                    m_objNegoBidEntry.NegotiationBidID = m_lstNegoBidStructureCart.Where(d => d.Sequence == (int)VendorBidTypes.Fee).Select(d => d.NegotiationBidID).FirstOrDefault();
                                    m_objNegoBidEntry.NegotiationEntryID = Guid.NewGuid().ToString().Replace("-", "");
                                    m_objNegoBidEntry.FPTVendorParticipantID = FPTParticipant.FPTVendorParticipantID;
                                    m_objNegoBidEntry.BidTypeID = General.EnumDesc(VendorBidTypes.Fee);
                                    m_objNegoBidEntry.BidValue = 0;

                                    m_objNegoBidEntryDA.Insert(true, m_objDBConnection);
                                    if (!m_objNegoBidEntryDA.Success)
                                    {
                                        m_lstMessage.Add(m_objNegoBidEntryDA.Message);
                                        break;
                                    }

                                    //Grand Total
                                    m_objNegoBidEntry = new DNegotiationBidEntry();
                                    m_objNegoBidEntryDA.Data = m_objNegoBidEntry;
                                    m_objNegoBidEntry.NegotiationBidID = m_lstNegoBidStructureCart.Where(d => d.Sequence == (int)VendorBidTypes.AfterFee).Select(d => d.NegotiationBidID).FirstOrDefault();
                                    m_objNegoBidEntry.NegotiationEntryID = Guid.NewGuid().ToString().Replace("-", "");
                                    m_objNegoBidEntry.FPTVendorParticipantID = FPTParticipant.FPTVendorParticipantID;
                                    m_objNegoBidEntry.BidTypeID = General.EnumDesc(VendorBidTypes.AfterFee);
                                    m_objNegoBidEntry.BidValue = m_lstNegoBidStructureCart.Where(d => d.VendorID == FPTParticipant.VendorID).Sum(d => d.BudgetPlanDefaultValue);

                                    m_objNegoBidEntryDA.Insert(true, m_objDBConnection);
                                    if (!m_objNegoBidEntryDA.Success)
                                    {
                                        m_lstMessage.Add(m_objNegoBidEntryDA.Message);
                                        break;
                                    }
                                   
                                    
                                }
                            }
                        }
                        else
                            m_lstMessage.Add(m_objTNegoBidStructuresDA.Message);
                    }
                    else
                        m_lstMessage.Add(m_objDFPTNegotiationRoundsDA.Message);
                    #endregion

                    if (!m_objNegoConfigDA.Success || m_objNegoConfigDA.Message != string.Empty)
                        m_lstMessage.Add(m_objNegoConfigDA.Message);

                }
            }
            catch (Exception ex)
            {
                m_objNegoConfigDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                m_lstMessage.Add(ex.Message);
            }
            if (m_lstMessage.Count <= 0)
            {
                m_objNegoConfigDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
                Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
                return Detail(General.EnumDesc(Buttons.ButtonSave), null);
            }
            m_objNegoConfigDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);
        }

        #endregion

        #region Direct Method
        public Node GetNodeUpload(bool ActionhasUpload, List<NegotiationBidStructuresVM> negoBidStructure, List<List<NegotiationBidEntryVM>> negoBidVendorStructure, List<FPTVendorParticipantsVM> FPTVendorParticipants, string TCType_, string filename)
        {
            string TCType = "";
            int nm = 1;
            Node getNode = new Node();
            getNode.NodeID = "ProjectNodeUpload";
            getNode.Icon = Icon.Folder;
            if (ActionhasUpload)
            {
                //Get Existing TBidstructure Upload
                List<NegotiationBidStructuresVM> m_obListjNegoBidStructureVM = new List<NegotiationBidStructuresVM>();
                TNegotiationBidStructuresDA m_objTNegoBidStructuresDA = new TNegotiationBidStructuresDA();
                NegotiationBidStructuresVM m_objNegoBidStructureVM = new NegotiationBidStructuresVM();
                m_objTNegoBidStructuresDA.ConnectionStringName = Global.ConnStrConfigName;

                List<string> NegoConfID = new List<string>();
                foreach (FPTVendorParticipantsVM FPTParticipan in FPTVendorParticipants)
                    NegoConfID.Add(FPTParticipan.NegotiationConfigID);

                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(NegotiationBidStructuresVM.Prop.NegotiationBidID.MapAlias);
                m_lstSelect.Add(NegotiationBidStructuresVM.Prop.ItemID.MapAlias);
                m_lstSelect.Add(NegotiationBidStructuresVM.Prop.ItemParentID.MapAlias);
                m_lstSelect.Add(NegotiationBidStructuresVM.Prop.Sequence.MapAlias);
                m_lstSelect.Add(NegotiationBidStructuresVM.Prop.ParentSequence.MapAlias);
                m_lstSelect.Add(NegotiationBidStructuresVM.Prop.ItemDesc.MapAlias);
                m_lstSelect.Add(NegotiationBidStructuresVM.Prop.BudgetPlanDefaultValue.MapAlias);

                Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.In);
                m_lstFilter.Add(String.Join(",", NegoConfID));
                m_objFilter.Add(NegotiationBidStructuresVM.Prop.NegotiationConfigID.Map, m_lstFilter);

                Dictionary<int, DataSet> m_dicTBidStructure = m_objTNegoBidStructuresDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
                if (m_objTNegoBidStructuresDA.Success)
                {
                    m_obListjNegoBidStructureVM = (from DataRow m_drTbidStructureDA in m_dicTBidStructure[0].Tables[0].Rows
                                                   select new NegotiationBidStructuresVM()
                                                   {
                                                       ItemID = m_drTbidStructureDA[NegotiationBidStructuresVM.Prop.ItemID.Name].ToString(),
                                                       ItemDesc = m_drTbidStructureDA[NegotiationBidStructuresVM.Prop.ItemDesc.Name].ToString(),
                                                       BudgetPlanDefaultValue = (decimal)m_drTbidStructureDA[NegotiationBidStructuresVM.Prop.BudgetPlanDefaultValue.Name],
                                                       ItemParentID = m_drTbidStructureDA[NegotiationBidStructuresVM.Prop.ItemParentID.Name].ToString(),
                                                       NegotiationBidID = m_drTbidStructureDA[NegotiationBidStructuresVM.Prop.NegotiationBidID.Name].ToString(),
                                                       Sequence = int.Parse(m_drTbidStructureDA[NegotiationBidStructuresVM.Prop.Sequence.Name].ToString()),
                                                       ParentSequence = int.Parse(m_drTbidStructureDA[NegotiationBidStructuresVM.Prop.ParentSequence.Name].ToString())
                                                   }).ToList();
                }
                negoBidStructure = m_obListjNegoBidStructureVM;

                //Get Existing BidEntry Upload
                List<List<NegotiationBidEntryVM>> lstBidEntry = new List<List<NegotiationBidEntryVM>>();
                DNegotiationBidEntryDA m_objNegoBidEntryDA = new DNegotiationBidEntryDA();
                List<NegotiationBidEntryVM> m_obListjNegoBidSEntryVM = new List<NegotiationBidEntryVM>();
                m_objNegoBidEntryDA.ConnectionStringName = Global.ConnStrConfigName;

                foreach (FPTVendorParticipantsVM FPTParticipan in FPTVendorParticipants)
                {
                    m_lstSelect = new List<string>();
                    m_lstSelect.Add(NegotiationBidEntryVM.Prop.NegotiationBidID.MapAlias);
                    m_lstSelect.Add(NegotiationBidEntryVM.Prop.VendorID.MapAlias);
                    m_lstSelect.Add(NegotiationBidEntryVM.Prop.BidValue.MapAlias);

                    m_objFilter = new Dictionary<string, List<object>>();
                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(FPTParticipan.FPTVendorParticipantID);
                    m_objFilter.Add(NegotiationBidEntryVM.Prop.FPTVendorParticipantID.Map, m_lstFilter);

                    Dictionary<int, DataSet> m_dicTBidEntry = m_objNegoBidEntryDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
                    if (m_objNegoBidEntryDA.Success)
                    {
                        m_obListjNegoBidSEntryVM = new List<NegotiationBidEntryVM>();
                        m_obListjNegoBidSEntryVM = (from DataRow m_drTbidEntryDA in m_dicTBidEntry[0].Tables[0].Rows
                                                    select new NegotiationBidEntryVM()
                                                    {
                                                        NegotiationBidID = m_drTbidEntryDA[NegotiationBidEntryVM.Prop.NegotiationBidID.Name].ToString(),
                                                        VendorID = m_drTbidEntryDA[NegotiationBidEntryVM.Prop.VendorID.Name].ToString(),
                                                        BidValue = decimal.Parse(m_drTbidEntryDA[NegotiationBidEntryVM.Prop.BidValue.Name].ToString()),
                                                    }).ToList();
                    }
                    lstBidEntry.Add(m_obListjNegoBidSEntryVM);
                }
                negoBidVendorStructure = lstBidEntry;

                //Populating TCType                
                TCType = TCType_;
            }
            else
            {

            }

            getNode.AttributesObject = new
            {
                number = nm,
                fileuploadname = filename,
                itemtype = "BudgetPlanID",
                itemdesc = "BudgetPlan : From Upload",
                value = "",
                tctype = TCType_,
                include = FPTVendorParticipants.Count > 0 || negoBidVendorStructure.Count > 0,
                liststructure = negoBidStructure,
                listvendorstructure = negoBidVendorStructure,
            };

            //Get VendorList
            NodeCollection m_nodeCollectionVendor = new NodeCollection();
            nm++;
            //List<BudgetPlanVersionVendorVM> lstVendor = new List<BudgetPlanVersionVendorVM>();
            //lstVendor = GetVendorList(false);


            //var grpVendor = lstVendor
            //    .GroupBy(u => u.VendorID)
            //    .Select(grp => grp.ToList())
            //    .ToList();
            //foreach (var obj in grpVendor)
            //{
            //    bool incl_ = false;
            //    if (FPTVendorParticipants.Count > 0)
            //        foreach (FPTVendorParticipantsVM fptp in FPTVendorParticipants.Where(x => x.VendorID == obj[0].VendorID))
            //            incl_ = true;

            //    Node nVendor = new Node();
            //    nVendor.Icon = Icon.User;
            //    nVendor.AttributesObject = new
            //    {
            //        number = nm,
            //        itemtype = "VendorID",
            //        itemdesc = "Vendor(" + obj[0].VendorID + ") : " + obj[0].VendorDesc,
            //        value = obj[0].VendorID,
            //        tctype = "",
            //        include = incl_
            //    };
            //    nVendor.Expandable = false;
            //    nm++;
            //    m_nodeCollectionVendor.Add(nVendor);
            //}
            if (FPTVendorParticipants.Count > 0 && ActionhasUpload) //IF From Add / Update /Detail
            {
                foreach (FPTVendorParticipantsVM obj in FPTVendorParticipants)
                {
                    Node nVendor = new Node();
                    nVendor.Icon = Icon.User;
                    nVendor.AttributesObject = new
                    {
                        number = nm,
                        itemtype = "VendorID",
                        itemdesc = "Vendor(" + obj.VendorID + ") : " + obj.FirstName,
                        value = obj.VendorID,
                        tctype = "",
                        include = true
                    };
                    nVendor.Expandable = false;
                    nm++;
                    m_nodeCollectionVendor.Add(nVendor);
                }
                getNode.Children.AddRange(m_nodeCollectionVendor);
                getNode.Expandable = m_nodeCollectionVendor.Count > 0;
                getNode.Expanded = m_nodeCollectionVendor.Count > 0;
            }
            else if (negoBidStructure.Count > 0)//IF From Upload
            {
                foreach (List<NegotiationBidEntryVM> obj in negoBidVendorStructure)
                {
                    Node nVendor = new Node();
                    nVendor.Icon = Icon.User;
                    nVendor.AttributesObject = new
                    {
                        number = nm,
                        itemtype = "VendorID",
                        itemdesc = "Vendor(" + obj[0].VendorID + ") : " + obj[0].VendorDesc,
                        value = obj[0].VendorID,
                        tctype = "",
                        include = true
                    };
                    nVendor.Expandable = false;
                    nm++;
                    m_nodeCollectionVendor.Add(nVendor);
                }
                getNode.Children.AddRange(m_nodeCollectionVendor);
                getNode.Expandable = m_nodeCollectionVendor.Count > 0;
                getNode.Expanded = m_nodeCollectionVendor.Count > 0;
            }

            return getNode;
        }

        public Node TreeNegoConfig(string Action, string ClusterID, List<ProjectVM> GetobjProject)
        {
            Node getNode = new Node();
            getNode.NodeID = "ProjectNode";
            getNode.AttributesObject = new
            {
                number = 0,
                itemdesc = "Parent",
                include = false
            };

            //Get ProjectList
            NodeCollection m_nodeCollectionProject = new NodeCollection();
            int nm = 1;
            List<string> m_lstmessage = new List<string>();
            List<ProjectVM> ExistingConfiguration = new List<ProjectVM>();
            List<ProjectVM> objProject = new List<ProjectVM>();

            objProject = GetProjectList(ClusterID, ref m_lstmessage);
            //If from Action Update / Detail
            if (Action != "Add")
            {
                ExistingConfiguration = GetobjProject;
            }

            foreach (ProjectVM obj in objProject)
            {
                bool isiNclude = false;
                ProjectVM sendObj = new ProjectVM();
                sendObj.ListBudgetPlanID = new List<BudgetPlanVM>();
                if (Action != "Add" && ExistingConfiguration.Count > 0)
                {
                    foreach (ProjectVM pvm in ExistingConfiguration.Where(x => x.ProjectID == obj.ProjectID))
                    {
                        isiNclude = true;
                        sendObj = pvm;
                    }
                }
                Node nProject = new Node();
                nProject.Icon = Icon.Shape3d;
                nProject.AttributesObject = new
                {
                    number = nm,
                    itemtype = "ProjectID",
                    itemdesc = "Project(" + obj.ProjectID + ") : " + obj.ProjectDesc,
                    value = obj.ProjectID,
                    tctype = "",
                    tctypedesc = "",
                    include = isiNclude
                };
                nm++;

                //Get BPID
                NodeCollection m_nodeCollectionBIPD = new NodeCollection();
                foreach (BudgetPlanVM bpid in obj.ListBudgetPlanID)
                {
                    bool isiNcludeB = false;
                    string TCType = "";
                    string TCTypeDesc = "";
                    BudgetPlanVM sendObjB = new BudgetPlanVM();
                    sendObjB.VendorParticipant = new List<VendorVM>();
                    if (Action != General.EnumDesc(Buttons.ButtonAdd) && sendObj.ListBudgetPlanID.Count > 0)
                    {
                        foreach (BudgetPlanVM pvm in sendObj.ListBudgetPlanID.Where(x => x.BudgetPlanID == bpid.BudgetPlanID))
                        {
                            isiNcludeB = true;
                            sendObjB = pvm;
                            TCType = pvm.TCProjectType;
                            TCTypeDesc = pvm.TCTypeDesc;
                        }
                    }

                    Node nBPID = new Node();
                    nBPID.Icon = Icon.Folder;
                    nBPID.AttributesObject = new
                    {
                        number = nm,
                        itemtype = "BudgetPlanID",
                        itemdesc = "BudgetPlan : " + bpid.Description + " (" + bpid.BudgetPlanID + ")",
                        value = bpid.BudgetPlanID,
                        tctype = TCType,
                        tctypedesc = TCTypeDesc,
                        include = isiNcludeB
                    };
                    nm++;
                    //GET Vendor
                    NodeCollection m_nodeCollectionVendor = new NodeCollection();
                    foreach (VendorVM vnd in bpid.VendorParticipant)
                    {
                        bool isiNcludeV = false;
                        if (Action != "Add" && sendObjB.VendorParticipant.Count > 0)
                        {
                            foreach (VendorVM pvm in sendObjB.VendorParticipant.Where(x => x.VendorID == vnd.VendorID))
                                isiNcludeV = true;
                        }
                        Node nVendor = new Node();
                        nVendor.Icon = Icon.User;
                        nVendor.AttributesObject = new
                        {
                            number = nm,
                            itemtype = "VendorID",
                            itemdesc = "Vendor(" + vnd.VendorID + ") : " + vnd.VendorDesc,
                            value = vnd.VendorID,
                            tctype = "",
                            tctypedesc = "",
                            include = isiNcludeV
                        };
                        nVendor.Expandable = false;
                        nm++;
                        m_nodeCollectionVendor.Add(nVendor);
                    }

                    if (m_nodeCollectionVendor.Count > 0)
                        nBPID.Children.AddRange(m_nodeCollectionVendor);

                    nBPID.Expandable = (nBPID.Children.Count > 0);
                    nBPID.Expanded = (nBPID.Children.Count > 0);
                    m_nodeCollectionBIPD.Add(nBPID);
                }
                if (m_nodeCollectionBIPD.Count > 0)
                    nProject.Children.AddRange(m_nodeCollectionBIPD);

                nProject.Expandable = (nProject.Children.Count > 0);
                nProject.Expanded = (nProject.Children.Count > 0);
                m_nodeCollectionProject.Add(nProject);
            }

            getNode.Children.AddRange(m_nodeCollectionProject);
            getNode.Expandable = true;
            getNode.Expanded = true;
            return getNode;
        }

        public Node TreeNegoConfigReload(string Action, string BUnit, string Division, string Project, string ClusterID, List<ProjectVM> GetobjProject)
        {
            Node getNode = new Node();
            getNode.NodeID = "ProjectNode";
            getNode.AttributesObject = new
            {
                number = 0,
                itemdesc = "Parent",
                include = false
            };

            //Get ProjectList
            NodeCollection m_nodeCollectionProject = new NodeCollection();
            int nm = 1;
            List<string> m_lstmessage = new List<string>();
            List<ProjectVM> ExistingConfiguration = new List<ProjectVM>();
            List<ProjectVM> objProject = new List<ProjectVM>();

            objProject = GetProjectListForReloadFPT(BUnit, Division, Project, ClusterID, ref m_lstmessage);
            //If from Action Update / Detail
            if (Action != "Add")
            {
                ExistingConfiguration = GetobjProject;
            }

            foreach (ProjectVM obj in objProject)
            {
                bool isiNclude = false;
                ProjectVM sendObj = new ProjectVM();
                sendObj.ListBudgetPlanID = new List<BudgetPlanVM>();
                if (Action != "Add" && ExistingConfiguration.Count > 0)
                {
                    foreach (ProjectVM pvm in ExistingConfiguration.Where(x => x.ProjectID == obj.ProjectID))
                    {
                        isiNclude = true;
                        sendObj = pvm;
                    }
                }
                Node nProject = new Node();
                nProject.Icon = Icon.Shape3d;
                nProject.AttributesObject = new
                {
                    number = nm,
                    itemtype = "ProjectID",
                    itemdesc = "Project(" + obj.ProjectID + ") : " + obj.ProjectDesc,
                    value = obj.ProjectID,
                    tctype = "",
                    tctypedesc = "",
                    include = isiNclude
                };
                nm++;

                //Get BPID
                NodeCollection m_nodeCollectionBIPD = new NodeCollection();
                foreach (BudgetPlanVM bpid in obj.ListBudgetPlanID)
                {
                    bool isiNcludeB = false;
                    string TCType = "";
                    string TCTypeDesc = "";
                    BudgetPlanVM sendObjB = new BudgetPlanVM();
                    sendObjB.VendorParticipant = new List<VendorVM>();
                    if (Action != General.EnumDesc(Buttons.ButtonAdd) && sendObj.ListBudgetPlanID.Count > 0)
                    {
                        foreach (BudgetPlanVM pvm in sendObj.ListBudgetPlanID.Where(x => x.BudgetPlanID == bpid.BudgetPlanID))
                        {
                            isiNcludeB = true;
                            sendObjB = pvm;
                            TCType = pvm.TCProjectType;
                            TCTypeDesc = pvm.TCTypeDesc;
                        }
                    }

                    Node nBPID = new Node();
                    nBPID.Icon = Icon.Folder;
                    nBPID.AttributesObject = new
                    {
                        number = nm,
                        itemtype = "BudgetPlanID",
                        itemdesc = "BudgetPlan : " + bpid.Description + " (" + bpid.BudgetPlanID + ")",
                        value = bpid.BudgetPlanID,
                        tctype = TCType,
                        tctypedesc = TCTypeDesc,
                        include = isiNcludeB
                    };
                    nm++;
                    //GET Vendor
                    NodeCollection m_nodeCollectionVendor = new NodeCollection();
                    foreach (VendorVM vnd in bpid.VendorParticipant)
                    {
                        bool isiNcludeV = false;
                        if (Action != "Add" && sendObjB.VendorParticipant.Count > 0)
                        {
                            foreach (VendorVM pvm in sendObjB.VendorParticipant.Where(x => x.VendorID == vnd.VendorID))
                                isiNcludeV = true;
                        }
                        Node nVendor = new Node();
                        nVendor.Icon = Icon.User;
                        nVendor.AttributesObject = new
                        {
                            number = nm,
                            itemtype = "VendorID",
                            itemdesc = "Vendor(" + vnd.VendorID + ") : " + vnd.VendorDesc,
                            value = vnd.VendorID,
                            tctype = "",
                            tctypedesc = "",
                            include = isiNcludeV
                        };
                        nVendor.Expandable = false;
                        nm++;
                        m_nodeCollectionVendor.Add(nVendor);
                    }

                    if (m_nodeCollectionVendor.Count > 0)
                        nBPID.Children.AddRange(m_nodeCollectionVendor);

                    nBPID.Expandable = (nBPID.Children.Count > 0);
                    nBPID.Expanded = (nBPID.Children.Count > 0);
                    m_nodeCollectionBIPD.Add(nBPID);
                }
                if (m_nodeCollectionBIPD.Count > 0)
                    nProject.Children.AddRange(m_nodeCollectionBIPD);

                nProject.Expandable = (nProject.Children.Count > 0);
                nProject.Expanded = (nProject.Children.Count > 0);
                m_nodeCollectionProject.Add(nProject);
            }

            getNode.Children.AddRange(m_nodeCollectionProject);
            getNode.Expandable = true;
            getNode.Expanded = true;
            return getNode;
        }

        public ActionResult GetCompany(string ControlCompanyID, string ControlCompanyDesc, string FilterCompanyID, string FilterCompanyDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<CompanyVM>> m_dicCompanyData = GetCompanyData(true, FilterCompanyID, FilterCompanyDesc);
                KeyValuePair<int, List<CompanyVM>> m_kvpCompanyVM = m_dicCompanyData.AsEnumerable().ToList()[0];
                if (m_kvpCompanyVM.Key < 1 || (m_kvpCompanyVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpCompanyVM.Key > 1 && !Exact)
                    return Browse(ControlCompanyID, ControlCompanyDesc, FilterCompanyID, FilterCompanyDesc);

                m_dicCompanyData = GetCompanyData(false, FilterCompanyID, FilterCompanyDesc);
                CompanyVM m_objCompanyVM = m_dicCompanyData[0][0];
                this.GetCmp<TextField>(ControlCompanyID).Value = m_objCompanyVM.CompanyID;
                this.GetCmp<TextField>(ControlCompanyDesc).Value = m_objCompanyVM.CompanyDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<BudgetPlanVersionVendorVM> GetVendorList(bool byBudgetPlan, ref List<string> message)
        {
            List<BudgetPlanVersionVendorVM> m_lstVendorVM = new List<BudgetPlanVersionVendorVM>();

            DBudgetPlanVersionVendorDA m_objMVendorDA = new DBudgetPlanVersionVendorDA();
            m_objMVendorDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            if (byBudgetPlan)
                m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanID.MapAlias);

            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.FirstName.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.LastName.MapAlias);

            List<string> m_llstGroup = new List<string>();
            if (byBudgetPlan)
                m_llstGroup.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanID.Map);
            m_llstGroup.Add(BudgetPlanVersionVendorVM.Prop.VendorID.Map);
            m_llstGroup.Add(BudgetPlanVersionVendorVM.Prop.FirstName.Map);
            m_llstGroup.Add(BudgetPlanVersionVendorVM.Prop.LastName.Map);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(1);
            m_objFilter.Add(BudgetPlanVersionVendorVM.Prop.StatusVendorID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanPeriod.IQ);
            m_objFilter.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanPeriodID.Map, m_lstFilter);

            Dictionary<string, OrderDirection> m_dicOrderBy = new Dictionary<string, OrderDirection>();
            m_dicOrderBy.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanID.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicVersionVendor = m_objMVendorDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, m_llstGroup, m_dicOrderBy);
            if (m_objMVendorDA.Success)
            {
                foreach (DataRow m_drNegotiationConfigurationsDA in m_dicVersionVendor[0].Tables[0].Rows)
                {
                    BudgetPlanVersionVendorVM obj = new BudgetPlanVersionVendorVM();
                    if (byBudgetPlan)
                        obj.BudgetPlanID = m_drNegotiationConfigurationsDA[BudgetPlanVersionVendorVM.Prop.BudgetPlanID.Name].ToString();

                    obj.VendorID = m_drNegotiationConfigurationsDA[BudgetPlanVersionVendorVM.Prop.VendorID.Name].ToString();
                    obj.FirstName = m_drNegotiationConfigurationsDA[BudgetPlanVersionVendorVM.Prop.FirstName.Name].ToString();
                    obj.LastName = m_drNegotiationConfigurationsDA[BudgetPlanVersionVendorVM.Prop.LastName.Name].ToString();
                    //bool Exists = m_lstVendorVM.Any(item => (item.BudgetPlanID == obj.BudgetPlanID && item.VendorID == obj.VendorID ));

                    //if (!Exists)
                    m_lstVendorVM.Add(obj);
                }
                //m_lstVendorVM =
                //(from DataRow m_drNegotiationConfigurationsDA in m_dicVersionVendor[0].Tables[0].Rows
                // select new BudgetPlanVersionVendorVM()
                // {
                //     BudgetPlanID = m_drNegotiationConfigurationsDA[BudgetPlanVersionVendorVM.Prop.BudgetPlanID.Name].ToString(),
                //     VendorID = m_drNegotiationConfigurationsDA[BudgetPlanVersionVendorVM.Prop.VendorID.Name].ToString(),
                //     FirstName = m_drNegotiationConfigurationsDA[BudgetPlanVersionVendorVM.Prop.FirstName.Name].ToString(),
                //     LastName = m_drNegotiationConfigurationsDA[BudgetPlanVersionVendorVM.Prop.LastName.Name].ToString()
                // }).ToList();
            }
            else
                message.Add(m_objMVendorDA.Message);
            List<BudgetPlanVersionVendorVM> m_lstVendorVM2 = m_lstVendorVM.Select(x => new BudgetPlanVersionVendorVM() { VendorID = x.VendorID, BudgetPlanID = x.BudgetPlanID, FirstName = x.FirstName, LastName = x.LastName }).Distinct().ToList();
            //m_lstVendorVM = m_lstVendorVM.GroupBy(x => new BudgetPlanVersionVendorVM() { VendorID = x.VendorID, BudgetPlanID =  x.BudgetPlanID,FirstName = x.FirstName,LastName = x.LastName }).ToList();

            return m_lstVendorVM;
        }
        public ActionResult GetTCProjectTypeList()
        {
            List<TCTypesVM> m_lstTCTypeVM = new List<TCTypesVM>();
            MTCTypesDA m_objTCTypeDA = new MTCTypesDA();
            m_objTCTypeDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(TCTypesVM.Prop.TCTypeID.MapAlias);
            m_lstSelect.Add(TCTypesVM.Prop.TCTypeDesc.MapAlias);

            Dictionary<int, DataSet> m_dicTCTypeDA = m_objTCTypeDA.SelectBC(0, null, false, m_lstSelect, null, null, null, null);
            if (m_objTCTypeDA.Success)
            {
                m_lstTCTypeVM =
                (from DataRow m_drTCTypeDA in m_dicTCTypeDA[0].Tables[0].Rows
                 select new TCTypesVM()
                 {
                     TCTypeID = m_drTCTypeDA[TCTypesVM.Prop.TCTypeID.Name].ToString(),
                     TCTypeDesc = m_drTCTypeDA[TCTypesVM.Prop.TCTypeDesc.Name].ToString()
                 }).ToList();
            }

            return this.Store(m_lstTCTypeVM);
        }

        public ActionResult GetBusinessUnit()
        {
            List<BusinessUnitVM> m_lstBusinessUnitVM = new List<BusinessUnitVM>();
            MBusinessUnitDA m_objBusinessUnitDA = new MBusinessUnitDA();
            m_objBusinessUnitDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BusinessUnitVM.Prop.BusinessUnitID.MapAlias);
            m_lstSelect.Add(BusinessUnitVM.Prop.BusinessUnitDesc.MapAlias);

            Dictionary<int, DataSet> m_dicBusinessUnitDA = m_objBusinessUnitDA.SelectBC(0, null, false, m_lstSelect, null, null, null, null);
            if (m_objBusinessUnitDA.Success)
            {
                m_lstBusinessUnitVM =
                (from DataRow m_drBusinessUnitDA in m_dicBusinessUnitDA[0].Tables[0].Rows
                 select new BusinessUnitVM()
                 {
                     BusinessUnitID = m_drBusinessUnitDA[BusinessUnitVM.Prop.BusinessUnitID.Name].ToString(),
                     BusinessUnitDesc = m_drBusinessUnitDA[BusinessUnitVM.Prop.BusinessUnitDesc.Name].ToString()
                 }).ToList();
            }

            return this.Store(m_lstBusinessUnitVM);
        }

        public ActionResult GetTCType()
        {
            List<TCMembersVM> lst_TCType = new List<TCMembersVM>();
            MTCTypesDA m_tctypeDA = new MTCTypesDA();
            m_tctypeDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(TCTypesVM.Prop.TCTypeID.MapAlias);

            Dictionary<int, DataSet> m_dicTCType = m_tctypeDA.SelectBC(0, null, false, m_lstSelect, null, null, null, null);
            if (m_tctypeDA.Success)
            {
                lst_TCType =
                (from DataRow m_dra in m_dicTCType[0].Tables[0].Rows
                 select new TCMembersVM()
                 {
                     TCTypeID = m_dra[TCTypesVM.Prop.TCTypeID.Name].ToString()
                 }).ToList();
            }

            //lst_TCType.Add(new TCMembersVM() { TCTypeID = "TC-A" });
            //lst_TCType.Add(new TCMembersVM() { TCTypeID = "TC-B" });
            //lst_TCType.Add(new TCMembersVM() { TCTypeID = "TC-C" });
            return this.Store(lst_TCType);
        }

        public ActionResult ReloadProjectList(string Action, string BUnit, string Division, string Project, string ClusterID)
        {
            return this.Store(TreeNegoConfigReload(Action, BUnit, Division, Project, ClusterID, null));
        }
        public ActionResult ReloadTCMember(string TCTypeID, string BusinessUnitID, string ScheduleDateNego)
        {
            TTCMembersDA m_objTTCMembersDA = new TTCMembersDA();
            m_objTTCMembersDA.ConnectionStringName = Global.ConnStrConfigName;

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<TCMembersVM> m_lsTCMembersVM = new List<TCMembersVM>();
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(TCMembersVM.Prop.TCMemberID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.EmployeeID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.EmployeeName.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.TCTypeDesc.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.TCTypeID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.PeriodStart.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.PeriodEnd.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.BusinessUnitDesc.MapAlias);

            List<string> m_lstm_Group = new List<string>();
            m_lstm_Group.Add(TCMembersVM.Prop.TCMemberID.Map);
            m_lstm_Group.Add(TCMembersVM.Prop.EmployeeID.Map);
            m_lstm_Group.Add(TCMembersVM.Prop.EmployeeName.Map);
            m_lstm_Group.Add(TCMembersVM.Prop.TCTypeDesc.Map);
            m_lstm_Group.Add(TCMembersVM.Prop.TCTypeID.Map);
            m_lstm_Group.Add(TCMembersVM.Prop.PeriodStart.Map);
            m_lstm_Group.Add(TCMembersVM.Prop.PeriodEnd.Map);
            m_lstm_Group.Add(TCMembersVM.Prop.BusinessUnitDesc.Map);

            if (!string.IsNullOrEmpty(TCTypeID))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(TCTypeID);
                m_objFilter.Add(TCMembersVM.Prop.TCTypeID.Map, m_lstFilter);
            }
            if (!string.IsNullOrEmpty(BusinessUnitID))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(BusinessUnitID);
                m_objFilter.Add(TCMembersVM.Prop.BusinessUnitID.Map, m_lstFilter);
            }
            if (!string.IsNullOrEmpty(ScheduleDateNego))
            {
                DateTime FilterSchedule_ = DateTime.Parse(JSON.Deserialize(ScheduleDateNego).ToString());
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.LessThanEqual);
                m_lstFilter.Add(FilterSchedule_);
                m_objFilter.Add(TCMembersVM.Prop.PeriodStart.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.GreaterThanEqual);
                m_lstFilter.Add(FilterSchedule_);
                m_objFilter.Add(TCMembersVM.Prop.PeriodEnd.Map, m_lstFilter);
            }

            Dictionary<int, DataSet> m_dicTTCMembersDA = m_objTTCMembersDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, m_lstm_Group, null, null);
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
                        //TCTypeDesc = m_drTTCMembersDA[TCMembersVM.Prop.TCTypeDesc.Name].ToString(),
                        //TCTypeID = m_drTTCMembersDA[TCMembersVM.Prop.TCTypeID.Name].ToString(),
                        Email = "",
                        BusinessUnitDesc = m_drTTCMembersDA[TCMembersVM.Prop.BusinessUnitDesc.Name].ToString()
                    }
                ).Distinct().ToList();
            }
            return this.Store(m_lsTCMembersVM);
        }

        public ActionResult ReadFileUpload()
        {
            XSSFWorkbook xhssfwb = new XSSFWorkbook();
            HSSFWorkbook hssfwb = new HSSFWorkbook();
            DataFormatter dataFormatterz = new DataFormatter(System.Globalization.CultureInfo.CurrentCulture);

            string message = "";
            try
            {
                SWeb.HttpPostedFile m_fileUpload = X.GetCmp<FileUploadField>(ItemPriceVM.Prop.FileUpload.Name).PostedFile;
                ISheet sheet;
                string filename = Path.GetFileName(Server.MapPath(m_fileUpload.FileName));
                var fileExt = Path.GetExtension(filename);
                if (fileExt == ".xls")
                {
                    hssfwb = new HSSFWorkbook(m_fileUpload.InputStream);
                    sheet = hssfwb.GetSheetAt(0);
                }
                else
                {
                    xhssfwb = new XSSFWorkbook(m_fileUpload.InputStream);
                    sheet = xhssfwb.GetSheetAt(0);
                }
                DataTable table = new DataTable();
                IRow headerRow = sheet.GetRow(4);
                int cellCount = headerRow.LastCellNum;
                int rowCount = sheet.LastRowNum;

                #region FeePercentage
                IRow hdrtc = sheet.GetRow(3);
                ICell tcCell = hdrtc.GetCell(1);
                bool isFormatNumber = tcCell.CellType == CellType.Numeric;

                string FeePercentage = dataFormatterz.FormatCellValue(tcCell);
                if (string.IsNullOrEmpty(FeePercentage) || !isFormatNumber)
                {
                    message = "Please Fill Fee(%) with Number Format";
                    Global.ShowErrorAlert(title, message);
                    return this.Direct();
                }
                #endregion

                #region Project & Pekerjaan
                IRow rowProj = sheet.GetRow(1);
                ICell cellProj = rowProj.GetCell(1);
                string ProjectName = dataFormatterz.FormatCellValue(cellProj);
                if (string.IsNullOrEmpty(ProjectName))
                {
                    message = "Please Fill Project Name";
                    Global.ShowErrorAlert(title, message);
                    return this.Direct();
                }

                IRow rowPekerjaan = sheet.GetRow(2);
                ICell cellPekerjaan = rowPekerjaan.GetCell(1);
                string DeskripsiPekerjaan = dataFormatterz.FormatCellValue(cellPekerjaan);
                if (string.IsNullOrEmpty(DeskripsiPekerjaan))
                {
                    message = "Please Fill RAB Description";
                    Global.ShowErrorAlert(title, message);
                    return this.Direct();
                }
                #endregion

                #region Check VendorLength
                bool vendor = true;
                int startColVendorCount = 7;//Start Read Vendor
                int startRowVendorIDCount = 3;//Row Number 4 = VendorID
                int startRowVendorDescCount = 5; //Row Number 6 = VendorDesc
                List<string> VendorIDList = new List<string>();
                List<string> VendorDescList = new List<string>();
                List<List<NegotiationBidEntryVM>> MainlstNegoVendorBidStructure = new List<List<NegotiationBidEntryVM>>();


                for (int x = startColVendorCount; x < 200; x++)
                {
                    ICell cellCheckingVendorID;
                    IRow headerRowCheck = sheet.GetRow(startRowVendorIDCount);
                    cellCheckingVendorID = headerRowCheck.GetCell(startColVendorCount);
                    string valueVendorID = dataFormatterz.FormatCellValue(cellCheckingVendorID);
                    if (!string.IsNullOrEmpty(valueVendorID))
                    {
                        string res = valueVendorID.Replace(" ", string.Empty);
                        if (!string.IsNullOrEmpty(res))
                        {
                            break;
                        }
                    }
                    startColVendorCount++;
                }
                int startReadItemVendor = startColVendorCount;

                while (vendor)
                {
                    ICell cellVendor;
                    headerRow = sheet.GetRow(startRowVendorIDCount);
                    cellVendor = headerRow.GetCell(startColVendorCount);

                    string valueVendorID = cellVendor!=null? cellVendor.CellType == CellType.Formula?
                                                                        cellVendor.CachedFormulaResultType == CellType.Numeric ?
                                                                                cellVendor.NumericCellValue.ToString() :
                                                                        cellVendor.CachedFormulaResultType == CellType.String ?
                                                                                cellVendor.StringCellValue.ToString() :
                                                                        "" :
                                                                  dataFormatterz.FormatCellValue(cellVendor):
                                                               "";
                    if (string.IsNullOrEmpty(valueVendorID))
                        break;
                    else
                    {
                        VendorIDList.Add(valueVendorID);
                        IRow headerRowDesc = sheet.GetRow(startRowVendorDescCount);
                        cellVendor = headerRowDesc.GetCell(startColVendorCount);
                        string valueVendorDesc = dataFormatterz.FormatCellValue(cellVendor);

                        if (string.IsNullOrEmpty(valueVendorDesc))
                            valueVendorDesc = "(unknown)";
                        VendorDescList.Add(valueVendorDesc);
                        List<NegotiationBidEntryVM> lstBidStructureVendor = new List<NegotiationBidEntryVM>();
                        MainlstNegoVendorBidStructure.Add(lstBidStructureVendor);
                    }
                    startColVendorCount++;
                }
                #endregion

                #region Item
                bool SummaryItem = false;
                int CurrentAdditional = 1;
                int startRowItem = 7;                       //Row Number 8 = Item
                int itemNumber = 0;                         //Col A = No.
                int itemDescription = 1;                    //Col B = Description.
                int TRM = startReadItemVendor-1;            //Col F = TRM Value
                int startVendorValue = startReadItemVendor; //Col Vendor Value
                int PenandaBOI = 8;                         //Col I = Satuan

                List<NegotiationBidStructuresVM> lstBidStructure = new List<NegotiationBidStructuresVM>();
                List<NegotiationBidStructuresVM> ToUploadlstBidStructure = new List<NegotiationBidStructuresVM>();
                int sequence = 1;
                string NoSequence = string.Empty;

                for (int x = startRowItem; x <= rowCount; x++)
                {
                    headerRow = sheet.GetRow(x);
                    if (headerRow == null)
                        break;

                    ICell cell;
                    cell = headerRow.GetCell(itemNumber);
                    NegotiationBidStructuresVM BPlan = new NegotiationBidStructuresVM();
                    DataFormatter dataFormatter = new DataFormatter(System.Globalization.CultureInfo.CurrentCulture);
                    string value = dataFormatter.FormatCellValue(cell);
                    NoSequence = value;
                    //BPlan.ItemID =headerRow.GetCell(1).StringCellValue.Trim();

                    //if (string.IsNullOrEmpty(dataFormatter.FormatCellValue(headerRow.GetCell(itemDescription))) &&
                    //    string.IsNullOrEmpty(dataFormatter.FormatCellValue(headerRow.GetCell(itemNumber))) &&
                    //    string.IsNullOrEmpty(dataFormatter.FormatCellValue(headerRow.GetCell(PenandaBOI)))){
                    //    if (!SummaryItem)
                    //        SummaryItem = true;
                    //    else
                    //        break;
                    //}
                    //else
                    //{
                    if (!string.IsNullOrEmpty(NoSequence)
                        || (string.IsNullOrEmpty(NoSequence) && !string.IsNullOrEmpty(dataFormatter.FormatCellValue(headerRow.GetCell(itemDescription))))
                    )
                    {
                        //Skip jika ada Harga Satuan
                        string satuan = dataFormatter.FormatCellValue(headerRow.GetCell(PenandaBOI));
                        satuan = satuan.Replace(" ", string.Empty);
                        string itemdescr = string.IsNullOrEmpty(dataFormatter.FormatCellValue(headerRow.GetCell(itemDescription))) ? "" : dataFormatter.FormatCellValue(headerRow.GetCell(itemDescription)).ToLower();
                        itemdescr = itemdescr.Replace(" ", string.Empty);
                        bool isSubtotal = false;

                        if (itemdescr.ToLower().Contains("pembulatan")) break;

                        if (itemdescr.Contains("subtotal"))
                            isSubtotal = itemdescr.Substring(0, 8).ToLower() == "subtotal";

                        if (!isSubtotal)
                        {
                            #region ItemID
                            if (string.IsNullOrEmpty(value)) // In Case Summary Item                               
                                value = "";

                            string ItemID = value;
                            BPlan.ItemID = value;
                            #endregion

                            #region ItemDescription
                            cell = headerRow.GetCell(itemDescription);
                            value = dataFormatter.FormatCellValue(cell);


                            if (NoSequence == "7777" || NoSequence == "8888" || NoSequence == "9999")
                            {
                                BPlan.ItemID = string.Empty;
                                if (CurrentAdditional > 3)
                                    break;

                                switch (CurrentAdditional)
                                {
                                    case 1:
                                        value = "Total";
                                        break;
                                    case 2:
                                        value = "Fee (%)";
                                        break;
                                    case 3:
                                        value = "Grand Total (total + fee)";
                                        break;
                                }
                            }

                            if (string.IsNullOrEmpty(value))
                            {
                                message = "ItemDescription " + BPlan.ItemID + " cannot be empty";
                                break;
                            }
                            BPlan.ItemDesc = value;
                            #endregion

                            #region TRMValue
                            //if (SummaryItem)
                            //{
                            // value = cell.CellFormula.ToString();                       
                            cell = headerRow.GetCell(TRM);
                            if (cell.CellType == CellType.Formula)
                                value = cell.CachedFormulaResultType == CellType.Numeric ? cell.NumericCellValue.ToString() : cell.CachedFormulaResultType == CellType.String ? cell.StringCellValue.ToString() : "";
                            else
                                value = dataFormatter.FormatCellValue(cell);

                            if (string.IsNullOrEmpty(value)) value = "0";
                            //if (string.IsNullOrEmpty(value))
                            //{
                            //    message = "Standard Value " + BPlan.ItemDesc + " cannot be empty";
                            //    break;
                            //}
                            decimal number;
                            if (Decimal.TryParse(value, out number))
                                value = number.ToString();
                            else
                            {
                                message = $"\"{value}\" couldn't convert to decimal or number!";
                                break;
                            }

                            decimal val = string.IsNullOrEmpty(value) ? 0 : decimal.Parse(value);
                            val = CurrentAdditional == 2 ? decimal.Parse(FeePercentage) : val;
                            BPlan.BudgetPlanDefaultValue = val;
                            //}
                            #endregion

                            #region ParentItem & ParentSequence                        
                            string[] comp = ItemID.Split('.');
                            int lastStringLength = comp[comp.Length - 1].Length + 1;
                            if (comp.Length > 1)
                            {
                                BPlan.ItemParentID = ItemID.Substring(0, ItemID.Length - lastStringLength);
                                foreach (NegotiationBidStructuresVM n in lstBidStructure.Where(q => q.ItemID == BPlan.ItemParentID))
                                    BPlan.ParentSequence = n.Sequence;
                            }
                            else
                            {
                                BPlan.ParentSequence = 0;
                                BPlan.ItemParentID = "0";
                            }
                            #endregion

                            #region Sequence & NegoBidID
                            if (NoSequence == "7777" || NoSequence == "8888" || NoSequence == "9999")
                            {
                                if (CurrentAdditional == 1)
                                {
                                    sequence = 7777;
                                    CurrentAdditional++;
                                }
                                else if (CurrentAdditional == 2)
                                {
                                    sequence = 8888;
                                    CurrentAdditional++;
                                }
                                else if (CurrentAdditional == 3)
                                {
                                    sequence = 9999;
                                    CurrentAdditional++;
                                }
                            }
                            BPlan.Sequence = sequence;
                            BPlan.NegotiationBidID = Guid.NewGuid().ToString().Replace("-", "");
                            #endregion

                            #region VendorValue
                            //if (SummaryItem)
                            //{
                            //NegoBidEntryVendor.NegotiationBidID = BPlan.NegotiationBidID;
                            for (int y = 0; y < VendorIDList.Count; y++)
                            {
                                NegotiationBidEntryVM NegoBidEntryVendor = new NegotiationBidEntryVM();
                                NegoBidEntryVendor.NegotiationBidID = BPlan.NegotiationBidID;
                                NegoBidEntryVendor.VendorID = VendorIDList[y];
                                NegoBidEntryVendor.VendorDesc = VendorDescList[y];

                                cell = headerRow.GetCell(startVendorValue + y);
                                if (cell == null)
                                {
                                    message = "Vendor Value " + BPlan.ItemDesc + " cannot be empty";
                                    break;
                                }
                                else if (cell.CellType == CellType.Formula)
                                    value = cell.CachedFormulaResultType == CellType.Numeric ? cell.NumericCellValue.ToString() : cell.CachedFormulaResultType == CellType.String ? cell.StringCellValue.ToString() : "";
                                else
                                    value = dataFormatter.FormatCellValue(cell);
                                decimal ou = 0;
                                if (string.IsNullOrEmpty(value))
                                {
                                    value = "0";
                                    //message = "Vendor Value " + BPlan.ItemDesc + " cannot be empty";
                                    // break;
                                }
                                else if (!decimal.TryParse(value, out ou))
                                {
                                    message = "Vendor Value " + BPlan.ItemDesc + " must be decimal";
                                    break;
                                }
                                // add comment for checkin, production got stomp with elder version
                                /*
                                 * value = CurrentAdditional == 3 ? FeePercentage : value;
                                 * 
                                 * */
                                NegoBidEntryVendor.BidValue = decimal.Parse(value);
                                NegotiationBidEntryVM objNegotiationBidEntryVM = MainlstNegoVendorBidStructure[y].FirstOrDefault(d => d.NegotiationBidID == BPlan.NegotiationBidID);
                                if (objNegotiationBidEntryVM != null)
                                    MainlstNegoVendorBidStructure[y].Remove(objNegotiationBidEntryVM);
                                MainlstNegoVendorBidStructure[y].Add(NegoBidEntryVendor);

                            }
                            //}
                            #endregion
                            sequence++;
                            lstBidStructure.Add(BPlan);

                        }
                        else
                        {

                            List<NegotiationBidStructuresVM> BPlanSelect = new List<NegotiationBidStructuresVM>();
                            string itmid = itemdescr.Substring(8, (itemdescr.Length - 8));
                            BPlanSelect = lstBidStructure.Where(n => n.ItemID == itmid).ToList();
                            if (BPlanSelect.Count > 0)
                            {
                                NegotiationBidStructuresVM bpToUpload = new NegotiationBidStructuresVM();
                                bpToUpload = BPlanSelect[0];

                                #region TRMValue
                                // value = cell.CellFormula.ToString();                       
                                cell = headerRow.GetCell(TRM);
                                if (cell.CellType == CellType.Formula)
                                    value = cell.CachedFormulaResultType == CellType.Numeric ? cell.NumericCellValue.ToString() : cell.CachedFormulaResultType == CellType.String ? cell.StringCellValue.ToString() : "";
                                else
                                    value = dataFormatter.FormatCellValue(cell);

                                if (string.IsNullOrEmpty(value))
                                {
                                    value = "0";
                                    //message = "Standard Value in Subtotal " + BPlan.ItemDesc + " cannot be empty";
                                    //break;
                                }
                                decimal val = string.IsNullOrEmpty(value) ? 0 : decimal.Parse(value);
                                bpToUpload.BudgetPlanDefaultValue = val;


                                #endregion

                                #region VendorValue
                                //NegoBidEntryVendor.NegotiationBidID = BPlan.NegotiationBidID;
                                for (int y = 0; y < VendorIDList.Count; y++)
                                {
                                    NegotiationBidEntryVM NegoBidEntryVendor = new NegotiationBidEntryVM();
                                    NegoBidEntryVendor.NegotiationBidID = bpToUpload.NegotiationBidID;
                                    NegoBidEntryVendor.VendorID = VendorIDList[y];
                                    NegoBidEntryVendor.VendorDesc = VendorDescList[y];

                                    cell = headerRow.GetCell(startVendorValue + y);
                                    if (cell == null)
                                    {
                                        message = "Vendor Value in Subtotal " + bpToUpload.ItemDesc + " cannot be empty";
                                        break;
                                    }
                                    else if (cell.CellType == CellType.Formula)
                                        value = cell.CachedFormulaResultType == CellType.Numeric ? cell.NumericCellValue.ToString() : cell.CachedFormulaResultType == CellType.String ? cell.StringCellValue.ToString() : "";
                                    else
                                        value = dataFormatter.FormatCellValue(cell);
                                    decimal ou = 0;
                                    if (string.IsNullOrEmpty(value))
                                    {
                                        message = "Vendor Value in Subtotal " + bpToUpload.ItemDesc + " cannot be empty";
                                        break;
                                    }
                                    else if (!decimal.TryParse(value, out ou))
                                    {
                                        message = "Vendor Value in Subtotal " + bpToUpload.ItemDesc + " must be decimal";
                                        break;
                                    }
                                    NegoBidEntryVendor.BidValue = decimal.Parse(value);
                                    NegotiationBidEntryVM objNegotiationBidEntryVM = MainlstNegoVendorBidStructure[y].FirstOrDefault(d => d.NegotiationBidID == bpToUpload.NegotiationBidID);
                                    if(objNegotiationBidEntryVM!=null)
                                        MainlstNegoVendorBidStructure[y].Remove(objNegotiationBidEntryVM);
                                    MainlstNegoVendorBidStructure[y].Add(NegoBidEntryVendor);

                                }
                                #endregion
                            }
                            else
                            {
                                message = "Item doesn't match with subtotal";
                                Global.ShowErrorAlert(title, message);
                                return this.Direct();
                            }
                        }
                    }
                }
                #endregion

                if (string.IsNullOrEmpty(message))
                {
                    Global.ShowInfo(title, "Upload Success");
                    lstBidStructure.Add(new NegotiationBidStructuresVM() { ItemID = "ProjectName99999", Sequence = 99999, ItemDesc = ProjectName });
                    lstBidStructure.Add(new NegotiationBidStructuresVM() { ItemID = "DeskripsiPekerjaan99998", Sequence = 99998, ItemDesc = DeskripsiPekerjaan });
                    //lstBidStructure[lstBidStructure.Count - 1].ItemDesc = "Grand Total"; //Replace Grand Total itemdesc text
                    //lstBidStructure[lstBidStructure.Count - 1].Sequence = 999999999;
                    return this.Store(GetNodeUpload(false, lstBidStructure, MainlstNegoVendorBidStructure, new List<FPTVendorParticipantsVM>(), "", filename));
                }
                Global.ShowErrorAlert(title, message);
                return this.Direct();
            }
            catch (Exception ex)
            {
                Global.ShowErrorAlert(title, String.Join(Global.NewLineSeparated, ex.Message));
                return this.Direct();
            }
        }
        private void GetChildBidEntryLastOffer(List<BudgetPlanVersionStructureVM> lstBidEntry, List<NegotiationBidStructuresVM> NegoTbidStructure, BudgetPlanVersionStructureVM ParentItemBid, string FPTParticipant, ref List<NegotiationBidEntryVM> mainListNegoBidEntry, ref List<string> message, ref decimal bidval_)
        {
            foreach (BudgetPlanVersionStructureVM structure in lstBidEntry.Where(x => x.ParentSequence == ParentItemBid.Sequence
                                                                                              && x.ParentVersion == ParentItemBid.Version))
            {
                NegotiationBidEntryVM m_objNegoBidEntry = new NegotiationBidEntryVM();
                foreach (NegotiationBidStructuresVM objtbid in NegoTbidStructure.Where(x => x.VersionStructureID == structure.BudgetPlanVersionStructureID))
                    m_objNegoBidEntry.NegotiationBidID = objtbid.NegotiationBidID;
                m_objNegoBidEntry.NegotiationEntryID = Guid.NewGuid().ToString().Replace("-", "");
                m_objNegoBidEntry.FPTVendorParticipantID = FPTParticipant;
                m_objNegoBidEntry.BidTypeID = General.EnumDesc(VendorBidTypes.LastOffer);

                decimal bidval = 0;
                GetChildBidEntryLastOffer(lstBidEntry, NegoTbidStructure, structure, FPTParticipant, ref mainListNegoBidEntry, ref message, ref bidval);

                if (bidval == 0)
                    bidval = structure.BidVolume * (structure.BidMAT + structure.BidWAG + structure.BidMISC);

                bidval_ += bidval;

                m_objNegoBidEntry.BidValue = bidval;
                mainListNegoBidEntry.Add(m_objNegoBidEntry);
            }
        }
        private List<NegotiationBidStructuresVM> GetTBidStructure(BudgetPlanVM BPVMSource, ref List<BudgetPlanVersionVendorVM> lstBVersionVendorVM, ref List<string> message, ref int BplanVersion)
        {
            #region Get BPlan Version            
            BudgetPlanVersionPeriodVM BPeriodVM = new BudgetPlanVersionPeriodVM();
            DBudgetPlanVersionPeriodDA BPVPeriodDA = new DBudgetPlanVersionPeriodDA();
            BPVPeriodDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanVersionPeriodID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanVersion.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BPVMSource.BudgetPlanID);

            m_objFilter.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanPeriod.IQ);
            m_objFilter.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanPeriodID.Map, m_lstFilter);


            List<string> m_lstGroup = new List<string>();
            //m_lstGroup.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanID.Map);
            //m_lstGroup.Add(BudgetPlanVersionPeriodVM.Prop.StatusID.Map);

            Dictionary<string, OrderDirection> m_dicOrderBy = new Dictionary<string, OrderDirection>();
            m_dicOrderBy.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanVersion.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicBudgetPlanVersionDA = BPVPeriodDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, m_dicOrderBy);
            if (BPVPeriodDA.Success)
            {
                foreach (DataRow m_drBPVDA in m_dicBudgetPlanVersionDA[0].Tables[0].Rows)
                {
                    BPeriodVM.BudgetPlanID = m_drBPVDA[BudgetPlanVersionPeriodVM.Prop.BudgetPlanID.Name].ToString();
                    BPeriodVM.BudgetPlanVersion = (int)m_drBPVDA[BudgetPlanVersionPeriodVM.Prop.BudgetPlanVersion.Name];
                    BPeriodVM.BudgetPlanVersionPeriodID = m_drBPVDA[BudgetPlanVersionPeriodVM.Prop.BudgetPlanVersionPeriodID.Name].ToString();
                    BplanVersion = BPeriodVM.BudgetPlanVersion;
                }
            }
            else
                message.Add(BPVPeriodDA.Message);

            #endregion

            #region Get Version Structure 
            List<BudgetPlanVersionStructureVM> m_lstBudgetPlanVersionStructureVM = new List<BudgetPlanVersionStructureVM>();
            DBudgetPlanVersionStructureDA m_objDBudgetPlanVersionStructureDA = new DBudgetPlanVersionStructureDA();
            m_objDBudgetPlanVersionStructureDA.ConnectionStringName = Global.ConnStrConfigName;

            if (message.Count <= 0)
            {
                m_lstSelect = new List<string>();
                m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanID.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersionStructureID.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ItemVersionChildID.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ItemID.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ItemDesc.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Version.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Sequence.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ParentItemID.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ParentItemDesc.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ParentVersion.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ParentSequence.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Volume.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Specification.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.MaterialAmount.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.WageAmount.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.MiscAmount.MapAlias);
                //m_lstSelect.Add("BIDENTRY.BudgetPlanVersionVendorID as VersionVendor");

                m_objFilter = new Dictionary<string, List<object>>();
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(BPeriodVM.BudgetPlanID);
                m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanID.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(BPeriodVM.BudgetPlanVersion);
                m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add("BOI");
                m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.ItemTypeID.Map, m_lstFilter);


                m_dicOrderBy = new Dictionary<string, OrderDirection>();
                m_dicOrderBy.Add(BudgetPlanVM.Prop.BudgetPlanID.Map, OrderDirection.Ascending);


                Dictionary<int, DataSet> m_dicDBudgetPlanVersionStructureDA = m_objDBudgetPlanVersionStructureDA.SelectBC_BidEntry(0, null, false, null, m_lstSelect, m_objFilter, null, null, null);
                if (m_objDBudgetPlanVersionStructureDA.Message == string.Empty)
                {
                    m_lstBudgetPlanVersionStructureVM = (
                    from DataRow m_drDBudgetPlanVersionStructureDA in m_dicDBudgetPlanVersionStructureDA[0].Tables[0].Rows
                    select new BudgetPlanVersionStructureVM()
                    {

                        //BudgetPlanVersionVendorID = m_drDBudgetPlanVersionStructureDA["VersionVendor"].ToString(),
                        BudgetPlanVersionStructureID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.BudgetPlanVersionStructureID.Name].ToString(),
                        BudgetPlanID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.BudgetPlanID.Name].ToString(),
                        BudgetPlanVersion = int.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.Name].ToString()),
                        ItemVersionChildID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ItemVersionChildID.Name].ToString(),
                        ItemID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ItemID.Name].ToString(),
                        ItemDesc = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ItemDesc.Name].ToString(),
                        Version = int.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Version.Name].ToString()),
                        Sequence = int.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Sequence.Name].ToString()),
                        ParentItemID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ParentItemID.Name].ToString(),
                        ParentItemDesc = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ParentItemDesc.Name].ToString(),
                        ParentVersion = int.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ParentVersion.Name].ToString()),
                        ParentSequence = int.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ParentSequence.Name].ToString()),
                        Volume = decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Volume.Name].ToString()),
                        Specification = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Specification.Name].ToString(),
                        MaterialAmount = decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name].ToString()),
                        WageAmount = decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.WageAmount.Name].ToString()),
                        MiscAmount = decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.MiscAmount.Name].ToString())

                    }).Distinct().ToList();

                    m_lstBudgetPlanVersionStructureVM = m_lstBudgetPlanVersionStructureVM.OrderBy(x => x.Sequence).ToList();
                }
                else
                    message.Add(m_objDBudgetPlanVersionStructureDA.Message);
            }

            #endregion

            #region Populate Bid Version Structure
            List<NegotiationBidStructuresVM> m_lstBidBPVStructureVM = new List<NegotiationBidStructuresVM>();
            if (message.Count <= 0)
            {
                NegotiationBidStructuresVM m_objBidStructure = new NegotiationBidStructuresVM();

                int LastRowForGrandtotal = 0;
                decimal? walkingGrandTotal = 0;
                List<NegotiationBidStructuresVM> NegoBidStructure = new List<NegotiationBidStructuresVM>();
                foreach (BudgetPlanVersionStructureVM structure in m_lstBudgetPlanVersionStructureVM.Where(x => x.ParentSequence == 0))
                {
                    decimal DefaultVal = 0;
                    NegotiationBidStructuresVM itemBid = new NegotiationBidStructuresVM();
                    itemBid.NegotiationBidID = Guid.NewGuid().ToString().Replace("-", "");
                    itemBid.NegotiationConfigID = BPVMSource.NegoConfigID;
                    itemBid.Sequence = structure.Sequence;
                    itemBid.ParentSequence = structure.ParentSequence;
                    itemBid.Version = structure.Version;
                    itemBid.ParentVersion = structure.ParentVersion;
                    itemBid.ItemID = structure.ItemID;
                    itemBid.ItemParentID = structure.ParentItemID;
                    itemBid.ItemDesc = structure.ItemDesc;
                    itemBid.VersionStructureID = structure.BudgetPlanVersionStructureID;

                    //Hitung Grand Total
                    NegotiationBidStructuresVM itemBidRef = new NegotiationBidStructuresVM();
                    itemBidRef.Vol = structure.Volume == null ? 0 : (decimal)structure.Volume;
                    itemBidRef.Misc = structure.MiscAmount == null ? 0 : (decimal)structure.MiscAmount;
                    itemBidRef.Wag = structure.WageAmount == null ? 0 : (decimal)structure.WageAmount;
                    itemBidRef.Wag = structure.MaterialAmount == null ? 0 : (decimal)structure.MaterialAmount;

                    LastRowForGrandtotal = structure.Sequence + 1;

                    //GetChild
                    GetChildBidStructure(m_lstBudgetPlanVersionStructureVM, itemBid, BPVMSource.NegoConfigID, "", ref DefaultVal, ref m_lstBidBPVStructureVM);

                    itemBid.BudgetPlanDefaultValue = DefaultVal == 0 ? (itemBidRef.Vol * (itemBidRef.Misc + itemBidRef.Mat + itemBidRef.Wag)) : DefaultVal;
                    walkingGrandTotal += itemBid.BudgetPlanDefaultValue;
                    m_lstBidBPVStructureVM.Add(itemBid);
                }
                //add 1 row for Total
                NegotiationBidStructuresVM Total = new NegotiationBidStructuresVM();
                Total.NegotiationBidID = Guid.NewGuid().ToString().Replace("-", "");
                Total.NegotiationConfigID = BPVMSource.NegoConfigID;
                Total.Sequence = 7777;
                Total.ParentSequence = 0;
                Total.VersionStructureID = "Total";
                Total.ItemDesc = "Total";
                Total.BudgetPlanDefaultValue = (decimal)walkingGrandTotal;
                m_lstBidBPVStructureVM.Add(Total);

                //add 1 row for Fee
                decimal feepercentage = 0;//Get Fee Percentage
                #region Get Fee Percentage
                BudgetPlanVersionVM BPVersion = new BudgetPlanVersionVM();
                DBudgetPlanVersionDA BPVersionDA = new DBudgetPlanVersionDA();
                BPVersionDA.ConnectionStringName = Global.ConnStrConfigName;

                m_lstSelect = new List<string>();
                m_lstSelect.Add(BudgetPlanVersionVM.Prop.BudgetPlanID.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionVM.Prop.BudgetPlanVersion.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionVM.Prop.FeePercentage.MapAlias);
                // m_lstSelect.Add("MAX(BudgetPlanVersion)AS BudgetPlanVersion");

                m_objFilter = new Dictionary<string, List<object>>();
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(BPVMSource.BudgetPlanID);
                m_objFilter.Add(BudgetPlanVersionVM.Prop.BudgetPlanID.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(BPeriodVM.BudgetPlanVersion);
                m_objFilter.Add(BudgetPlanVersionVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

                Dictionary<int, DataSet> m_dicBudgetPlanVersion_DA = BPVersionDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
                if (BPVersionDA.Success)
                {
                    foreach (DataRow m_drBPVDA in m_dicBudgetPlanVersion_DA[0].Tables[0].Rows)
                    {
                        BPVersion.BudgetPlanID = m_drBPVDA[BudgetPlanVersionVM.Prop.BudgetPlanID.Name].ToString();
                        BPVersion.FeePercentage = decimal.Parse(m_drBPVDA[BudgetPlanVersionVM.Prop.FeePercentage.Name].ToString());
                        feepercentage = BPVersion.FeePercentage;
                    }
                }
                else
                    message.Add(BPVersionDA.Message);
                #endregion
                NegotiationBidStructuresVM Fee = new NegotiationBidStructuresVM();
                Fee.NegotiationBidID = Guid.NewGuid().ToString().Replace("-", "");
                Fee.NegotiationConfigID = BPVMSource.NegoConfigID;
                Fee.Sequence = 8888;
                Fee.ParentSequence = 0;
                Fee.VersionStructureID = "Fee(%)";
                Fee.ItemDesc = "Fee (%)";
                Fee.BudgetPlanDefaultValue = feepercentage;///100 * Total.BudgetPlanDefaultValue;
                m_lstBidBPVStructureVM.Add(Fee);

                //add 1 row for GrandTotal
                NegotiationBidStructuresVM GrandTotalItem = new NegotiationBidStructuresVM();
                GrandTotalItem.NegotiationBidID = Guid.NewGuid().ToString().Replace("-", "");
                GrandTotalItem.NegotiationConfigID = BPVMSource.NegoConfigID;
                GrandTotalItem.Sequence = 9999;
                GrandTotalItem.ParentSequence = 0;
                GrandTotalItem.VersionStructureID = "GrandTotal";
                GrandTotalItem.ItemDesc = "Grand Total (total+fee)";
                GrandTotalItem.BudgetPlanDefaultValue = Total.BudgetPlanDefaultValue + (Fee.BudgetPlanDefaultValue * Total.BudgetPlanDefaultValue / 100);
                m_lstBidBPVStructureVM.Add(GrandTotalItem);


            }
            #endregion

            #region Get List Of Vendor (VendorVersionID)
            DBudgetPlanVersionVendorDA BVersionVendorDA = new DBudgetPlanVersionVendorDA();
            BVersionVendorDA.ConnectionStringName = Global.ConnStrConfigName;
            //List<BudgetPlanVersionVendorVM> lstBVersionVendorVM = new List<BudgetPlanVersionVendorVM>();
            if (message.Count <= 0)
            {
                List<string> VendorIDList = new List<string>();
                m_lstSelect = new List<string>();
                m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionPeriodID.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionVendorID.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.VendorID.MapAlias);
                m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.FeePercentage.MapAlias);

                m_objFilter = new Dictionary<string, List<object>>();
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(BPeriodVM.BudgetPlanVersionPeriodID);
                m_objFilter.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionPeriodID.Map, m_lstFilter);

                foreach (VendorVM vList in BPVMSource.VendorParticipant)
                    VendorIDList.Add(vList.VendorID);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.In);
                m_lstFilter.Add(String.Join(",", VendorIDList));
                m_objFilter.Add(BudgetPlanVersionVendorVM.Prop.VendorID.Map, m_lstFilter);

                Dictionary<int, DataSet> m_dicBudgetPlanVersionVendorDA = BVersionVendorDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
                if (BVersionVendorDA.Success)
                {
                    foreach (DataRow m_drBPVersionVendorDA in m_dicBudgetPlanVersionVendorDA[0].Tables[0].Rows)
                    {
                        BudgetPlanVersionVendorVM BVersionVendor = new BudgetPlanVersionVendorVM();
                        BVersionVendor.BudgetPlanVersionPeriodID = m_drBPVersionVendorDA[BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionPeriodID.Name].ToString();
                        BVersionVendor.BudgetPlanVersionVendorID = m_drBPVersionVendorDA[BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionVendorID.Name].ToString();
                        BVersionVendor.VendorID = m_drBPVersionVendorDA[BudgetPlanVersionVendorVM.Prop.VendorID.Name].ToString();
                        //set fee percentage
                        BVersionVendor.FeePercentage = decimal.Parse(m_drBPVersionVendorDA[BudgetPlanVersionVendorVM.Prop.FeePercentage.Name].ToString());
                        lstBVersionVendorVM.Add(BVersionVendor);
                    }
                }
                else
                    message.Add(BVersionVendorDA.Message);
            }
            #endregion

            return m_lstBidBPVStructureVM;
        }
        private void GetChildBidStructure(List<BudgetPlanVersionStructureVM> BudgetVersionStructureList, NegotiationBidStructuresVM ParentItemBid, string NegoConfigID, string VendorID, ref decimal DefaultValRef, ref List<NegotiationBidStructuresVM> MainBidStructure)
        {
            //List<NegotiationBidStructuresVM> NegoBidStructure = new List<NegotiationBidStructuresVM>();
            foreach (BudgetPlanVersionStructureVM structure in BudgetVersionStructureList.Where(x => x.ParentSequence == ParentItemBid.Sequence
                                                                                               && x.ParentItemID == ParentItemBid.ItemID
                                                                                               && x.ParentVersion == ParentItemBid.Version))
            {
                decimal Default = 0;
                NegotiationBidStructuresVM itemBid = new NegotiationBidStructuresVM();
                itemBid.NegotiationBidID = Guid.NewGuid().ToString().Replace("-", "");
                itemBid.NegotiationConfigID = NegoConfigID;
                itemBid.Sequence = structure.Sequence;
                itemBid.ParentSequence = structure.ParentSequence;
                itemBid.Version = structure.Version;
                itemBid.ParentVersion = structure.ParentVersion;
                itemBid.ItemID = structure.ItemID;
                itemBid.ItemParentID = structure.ParentItemID;
                itemBid.ItemDesc = structure.ItemDesc;
                itemBid.VendorID = VendorID;
                MainBidStructure.Add(itemBid);
                itemBid.VersionStructureID = structure.BudgetPlanVersionStructureID;

                NegotiationBidStructuresVM itemBidRef = new NegotiationBidStructuresVM();
                itemBidRef.Vol = structure.Volume == null ? 0 : (decimal)structure.Volume;
                itemBidRef.Misc = structure.MiscAmount == null ? 0 : (decimal)structure.MiscAmount;
                itemBidRef.Wag = structure.WageAmount == null ? 0 : (decimal)structure.WageAmount;
                itemBidRef.Mat = structure.MaterialAmount == null ? 0 : (decimal)structure.MaterialAmount;


                GetChildBidStructure(BudgetVersionStructureList, itemBid, NegoConfigID, VendorID, ref Default, ref MainBidStructure);
                itemBid.BudgetPlanDefaultValue = Default == 0 ? (itemBidRef.Vol * (itemBidRef.Misc + itemBidRef.Mat + itemBidRef.Wag)) : Default;
                DefaultValRef += itemBid.BudgetPlanDefaultValue;
            }
        }
        private List<FPTVendorParticipantsVM> GetFPTarticipants(List<string> lst_NegoConfigID, ref string message)
        {

            List<FPTVendorParticipantsVM> m_lstVendorFPTParticipantVM = new List<FPTVendorParticipantsVM>();
            DFPTVendorParticipantsDA m_objFPTParticipantsDA = new DFPTVendorParticipantsDA();
            m_objFPTParticipantsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.FPTVendorParticipantID.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.NegotiationConfigID.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.FirstName.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(String.Join(",", lst_NegoConfigID));
            m_objFilter.Add(FPTVendorParticipantsVM.Prop.NegotiationConfigID.Map, m_lstFilter);

            Dictionary<string, OrderDirection> m_dicOrderBy = new Dictionary<string, OrderDirection>();
            m_dicOrderBy.Add(FPTVendorParticipantsVM.Prop.NegotiationConfigID.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicVendorParticipantsDA = m_objFPTParticipantsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, m_dicOrderBy);
            if (m_objFPTParticipantsDA.Success)
            {
                m_lstVendorFPTParticipantVM =
                (from DataRow m_drFPTParticipantsDA in m_dicVendorParticipantsDA[0].Tables[0].Rows
                 select new FPTVendorParticipantsVM()
                 {
                     FPTVendorParticipantID = m_drFPTParticipantsDA[FPTVendorParticipantsVM.Prop.FPTVendorParticipantID.Name].ToString(),
                     NegotiationConfigID = m_drFPTParticipantsDA[FPTVendorParticipantsVM.Prop.NegotiationConfigID.Name].ToString(),
                     VendorID = m_drFPTParticipantsDA[FPTVendorParticipantsVM.Prop.VendorID.Name].ToString(),
                     FirstName = m_drFPTParticipantsDA[FPTVendorParticipantsVM.Prop.FirstName.Name].ToString()

                 }).ToList();
            }
            else
                message = m_objFPTParticipantsDA.Message;

            return m_lstVendorFPTParticipantVM;
        }

        private List<ProjectVM> GetProjectList(string ClusterID, ref List<string> m_lstMessage)
        {
            List<ProjectVM> m_lstProjectVM = new List<ProjectVM>();
            List<ProjectVM> m_lstProjectVMReturn = new List<ProjectVM>();
            TBudgetPlanDA m_objTbudgetPlanDA = new TBudgetPlanDA();
            m_objTbudgetPlanDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.Description.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.ProjectID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.ProjectDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.StatusID.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(2);
            m_objFilter.Add(BudgetPlanVM.Prop.StatusID.Map, m_lstFilter);
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(1);
            m_objFilter.Add(BudgetPlanVM.Prop.IsBidOpen.Map, m_lstFilter);
            if (!string.IsNullOrEmpty(ClusterID))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(ClusterID);
                m_objFilter.Add(BudgetPlanVM.Prop.ClusterID.Map, m_lstFilter);
            }

            List<string> m_lstGroup = new List<string>();
            m_lstGroup.Add(BudgetPlanVM.Prop.BudgetPlanID.Map);
            m_lstGroup.Add(BudgetPlanVM.Prop.Description.Map);
            m_lstGroup.Add(BudgetPlanVM.Prop.ProjectID.Map);
            m_lstGroup.Add(BudgetPlanVM.Prop.ProjectDesc.Map);
            m_lstGroup.Add(BudgetPlanVM.Prop.StatusID.Map);

            Dictionary<string, OrderDirection> m_dicOrderBy = new Dictionary<string, OrderDirection>();
            m_dicOrderBy.Add(BudgetPlanVM.Prop.ProjectID.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicBudgetPlanDA = m_objTbudgetPlanDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, m_lstGroup, m_dicOrderBy);
            if (m_objTbudgetPlanDA.Success)
            {
                m_lstProjectVM = (from DataRow m_drNegotiationConfigurationsDA in m_dicBudgetPlanDA[0].Tables[0].Rows
                                  select new ProjectVM()
                                  {
                                      BudgetPlanID = m_drNegotiationConfigurationsDA[BudgetPlanVM.Prop.BudgetPlanID.Name].ToString(),
                                      BudgetPlanDescription = m_drNegotiationConfigurationsDA[BudgetPlanVM.Prop.Description.Name].ToString(),
                                      ProjectID = m_drNegotiationConfigurationsDA[BudgetPlanVM.Prop.ProjectID.Name].ToString(),
                                      ProjectDesc = m_drNegotiationConfigurationsDA[BudgetPlanVM.Prop.ProjectDesc.Name].ToString()
                                  }).ToList();

                List<BudgetPlanVersionVendorVM> lstVendor = new List<BudgetPlanVersionVendorVM>();
                //Get List Approved Vendor
                lstVendor = GetVendorList(true, ref m_lstMessage);
                if (m_lstMessage.Count <= 0)
                {
                    var grpPoject = m_lstProjectVM
                        .GroupBy(u => u.ProjectID)
                        .Select(grp => grp.ToList())
                        .ToList();

                    foreach (var s in grpPoject)
                    {
                        ProjectVM objProject = new ProjectVM();
                        objProject.ProjectID = s[0].ProjectID;
                        objProject.ProjectDesc = s[0].ProjectDesc;
                        objProject.ListBudgetPlanID = new List<BudgetPlanVM>();

                        foreach (ProjectVM n in m_lstProjectVM.Where(x => x.ProjectID == objProject.ProjectID))
                        {
                            BudgetPlanVM bpvm = new BudgetPlanVM();
                            bpvm.BudgetPlanID = n.BudgetPlanID;
                            bpvm.Description = n.BudgetPlanDescription;
                            bpvm.VendorParticipant = new List<VendorVM>();
                            foreach (BudgetPlanVersionVendorVM vmn in lstVendor.Where(x => x.BudgetPlanID == n.BudgetPlanID))
                            {
                                VendorVM vndr = new VendorVM();
                                vndr.VendorID = vmn.VendorID;
                                vndr.VendorDesc = vmn.VendorDesc;
                                bpvm.VendorParticipant.Add(vndr);
                            }
                            objProject.ListBudgetPlanID.Add(bpvm);
                        }
                        m_lstProjectVMReturn.Add(objProject);
                    }
                }
            }
            else
                m_lstMessage.Add(m_objTbudgetPlanDA.Message);

            return m_lstProjectVMReturn;
        }
        private List<ProjectVM> GetProjectListForReloadFPT(string BUnit, string Division, string Project, string ClusterID, ref List<string> m_lstMessage)
        {
            List<ProjectVM> m_lstProjectVM = new List<ProjectVM>();
            List<ProjectVM> m_lstProjectVMReturn = new List<ProjectVM>();
            TBudgetPlanDA m_objTbudgetPlanDA = new TBudgetPlanDA();
            m_objTbudgetPlanDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add("MAX(DBudgetPlanVersion.BudgetPlanVersion) OVER (PARTITION by TBudgetPlan.BudgetPlanID) AS BudgetPlanVersion");
            m_lstSelect.Add(BudgetPlanVM.Prop.Description.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.ProjectID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.ProjectDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.StatusID.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(2);
            m_objFilter.Add(BudgetPlanVM.Prop.StatusID.Map, m_lstFilter);
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(1);
            m_objFilter.Add(BudgetPlanVM.Prop.IsBidOpen.Map, m_lstFilter);

            if (!string.IsNullOrEmpty(ClusterID))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(ClusterID);
                m_objFilter.Add(BudgetPlanVM.Prop.ClusterID.Map, m_lstFilter);
            }
            else if (!string.IsNullOrEmpty(Project))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(Project);
                m_objFilter.Add(BudgetPlanVM.Prop.ProjectID.Map, m_lstFilter);
            }
            else if (!string.IsNullOrEmpty(Division))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(Division);
                m_objFilter.Add(BudgetPlanVM.Prop.DivisionID.Map, m_lstFilter);
            }
            else if (!string.IsNullOrEmpty(BUnit))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(BUnit);
                m_objFilter.Add(BudgetPlanVM.Prop.BusinessUnitID.Map, m_lstFilter);
            }

            List<string> m_lstGroup = new List<string>();
            m_lstGroup.Add(BudgetPlanVM.Prop.BudgetPlanID.Map);
            m_lstGroup.Add(BudgetPlanVM.Prop.BudgetPlanVersion.Map);
            m_lstGroup.Add(BudgetPlanVM.Prop.Description.Map);
            m_lstGroup.Add(BudgetPlanVM.Prop.ProjectID.Map);
            m_lstGroup.Add(BudgetPlanVM.Prop.ProjectDesc.Map);
            m_lstGroup.Add(BudgetPlanVM.Prop.StatusID.Map);

            Dictionary<string, OrderDirection> m_dicOrderBy = new Dictionary<string, OrderDirection>();
            m_dicOrderBy.Add(BudgetPlanVM.Prop.ProjectID.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicBudgetPlanDA = m_objTbudgetPlanDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, m_lstGroup, m_dicOrderBy);
            if (m_objTbudgetPlanDA.Success)
            {
                m_lstProjectVM = (from DataRow m_drNegotiationConfigurationsDA in m_dicBudgetPlanDA[0].Tables[0].Rows
                                  select new ProjectVM()
                                  {
                                      BudgetPlanID = m_drNegotiationConfigurationsDA[BudgetPlanVM.Prop.BudgetPlanID.Name].ToString(),
                                      BudgetPlanDescription = m_drNegotiationConfigurationsDA[BudgetPlanVM.Prop.Description.Name].ToString(),
                                      ProjectID = m_drNegotiationConfigurationsDA[BudgetPlanVM.Prop.ProjectID.Name].ToString(),
                                      ProjectDesc = m_drNegotiationConfigurationsDA[BudgetPlanVM.Prop.ProjectDesc.Name].ToString()
                                  }).ToList();

                List<BudgetPlanVersionVendorVM> lstVendor = new List<BudgetPlanVersionVendorVM>();
                //Get List Approved Vendor
                m_lstProjectVM = m_lstProjectVM.Distinct().ToList();
                lstVendor = GetVendorList(true, ref m_lstMessage);
                if (m_lstMessage.Count <= 0)
                {
                    var grpPoject = m_lstProjectVM
                        .GroupBy(u => u.ProjectID)
                        .Select(grp => grp.ToList())
                        .ToList();

                    foreach (var s in grpPoject)
                    {
                        ProjectVM objProject = new ProjectVM();
                        objProject.ProjectID = s[0].ProjectID;
                        objProject.ProjectDesc = s[0].ProjectDesc;
                        objProject.ListBudgetPlanID = new List<BudgetPlanVM>();

                        foreach (ProjectVM n in m_lstProjectVM.Where(x => x.ProjectID == objProject.ProjectID))
                        {
                            BudgetPlanVM bpvm = new BudgetPlanVM();
                            bpvm.BudgetPlanID = n.BudgetPlanID;
                            bpvm.Description = n.BudgetPlanDescription;
                            bpvm.VendorParticipant = new List<VendorVM>();
                            foreach (BudgetPlanVersionVendorVM vmn in lstVendor.Where(x => x.BudgetPlanID == n.BudgetPlanID))
                            {
                                VendorVM vndr = new VendorVM();
                                vndr.VendorID = vmn.VendorID;
                                vndr.VendorDesc = vmn.VendorDesc;
                                bpvm.VendorParticipant.Add(vndr);
                            }
                            objProject.ListBudgetPlanID.Add(bpvm);
                        }
                        m_lstProjectVMReturn.Add(objProject);
                    }
                }
            }
            else
                m_lstMessage.Add(m_objTbudgetPlanDA.Message);

            return m_lstProjectVMReturn;
        }
        protected List<string> GetFPTinNegoConfig(ref string message)
        {
            List<string> returnthis = new List<string>();
            List<NegotiationConfigurationsVM> m_lstNegotiationConfigurationsVM = new List<NegotiationConfigurationsVM>();
            CNegotiationConfigurationsDA m_objCNegotiationConfigurationsDA = new CNegotiationConfigurationsDA();
            m_objCNegotiationConfigurationsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.FPTID.MapAlias);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.NotEqual);
            m_lstFilter.Add(3);
            m_objFilter.Add(NegotiationConfigurationsVM.Prop.StatusID.Map, m_lstFilter);

            List<string> m_lstGroup = new List<string>();
            m_lstGroup.Add(NegotiationConfigurationsVM.Prop.FPTID.Map);

            Dictionary<string, OrderDirection> m_dicOrderBy = new Dictionary<string, OrderDirection>();
            m_dicOrderBy.Add(NegotiationConfigurationsVM.Prop.FPTID.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicNegotiationConfigurationsDA = m_objCNegotiationConfigurationsDA.SelectBC(0, null, false, m_lstSelect, null, null, m_lstGroup, m_dicOrderBy);
            if (m_objCNegotiationConfigurationsDA.Success)
            {
                m_lstNegotiationConfigurationsVM =
                (from DataRow m_drNegotiationConfigurationsDA in m_dicNegotiationConfigurationsDA[0].Tables[0].Rows
                 select new NegotiationConfigurationsVM()
                 {
                     FPTID = m_drNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.FPTID.Name].ToString(),
                 }).ToList();
            }
            else
                message = m_objCNegotiationConfigurationsDA.Message;

            foreach (NegotiationConfigurationsVM n in m_lstNegotiationConfigurationsVM)
                returnthis.Add(n.FPTID);


            return returnthis;

        }
        private List<string> IsSaveValid(bool isRenego, string Action, string FPTID, string Round, string NegoLevel, DateTime NegoSchedule, TimeSpan NegoScheduleTime, string TRMLead, string RoundTime, List<BudgetPlanVM> BPVM, List<string> ListTCMembers)
        {
            int out_ = 0;
            List<string> m_lstReturn = new List<string>();
            if (FPTID == string.Empty)
                m_lstReturn.Add(FPTVM.Prop.FPTID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            else if (FPTID != string.Empty && Action == "Add")
            {
                CNegotiationConfigurationsDA m_objNegoConfigDA = new CNegotiationConfigurationsDA();
                CNegotiationConfigurations m_objNegoConfig = new CNegotiationConfigurations();
                m_objNegoConfigDA.ConnectionStringName = Global.ConnStrConfigName;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(NegotiationConfigurationsVM.Prop.FPTID.MapAlias);
                Dictionary<string, List<object>> m_objFilter_ = new Dictionary<string, List<object>>();
                List<object> m_lstFilter_ = new List<object>();
                m_lstFilter_.Add(Operator.Equals);
                m_lstFilter_.Add(FPTID);
                m_objFilter_.Add(NegotiationConfigurationsVM.Prop.FPTID.Map, m_lstFilter_);
                Dictionary<int, DataSet> m_dicNegotiationConfigurationsDA = m_objNegoConfigDA.SelectBC(0, null, false, m_lstSelect, m_objFilter_, null, null, null, null);
                if (m_objNegoConfigDA.Success && m_objNegoConfigDA.AffectedRows > 0)
                    m_lstReturn.Add(FPTVM.Prop.FPTID.Desc + " " + General.EnumDesc(MessageLib.Exist));
            }
            if (Round == string.Empty)
                m_lstReturn.Add(FPTVM.Prop.NegoRound.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            else if (int.Parse(Round) > 99)
                m_lstReturn.Add(FPTVM.Prop.NegoRound.Desc + " Maximum Length is 99");

            if (NegoLevel == string.Empty)
                m_lstReturn.Add(FPTVM.Prop.NegoLevel.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            else if (int.Parse(NegoLevel) > 99)
                m_lstReturn.Add(FPTVM.Prop.NegoLevel.Desc + " Maximum Length is 99");
            //if (NegoSchedule == string.Empty)
            //    m_lstReturn.Add(FPTVM.Prop.Schedule.Desc + "Date " + General.EnumDesc(MessageLib.mustFill));
            //if (NegoSchedule == string.Empty)
            //    m_lstReturn.Add(FPTVM.Prop.Schedule.Desc + "Time " + General.EnumDesc(MessageLib.mustFill));
            //else 
            if (TRMLead == string.Empty)
                m_lstReturn.Add(FPTVM.Prop.TRMLead.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (RoundTime == string.Empty)
                m_lstReturn.Add(FPTVM.Prop.NegoRoundTime.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            else if (int.Parse(RoundTime) > 99)
                m_lstReturn.Add(FPTVM.Prop.NegoRoundTime.Desc + " Maximum Length is 120");
            if (ListTCMembers.Count <= 0)
                m_lstReturn.Add(TCMembersVM.Prop.TCMemberID.Desc + " Must fill at least 2 TC Members");
            else
            {
                TTCMembersDA m_objTTCmembersDA = new TTCMembersDA();
                m_objTTCmembersDA.ConnectionStringName = Global.ConnStrConfigName;
                foreach (string tcmember in ListTCMembers)
                {
                    TTCMembers m_objTTCmembers = new TTCMembers();
                    m_objTTCmembers.TCMemberID = tcmember;
                    m_objTTCmembersDA.Data = m_objTTCmembers;
                    m_objTTCmembersDA.Select();
                    if (m_objTTCmembersDA.Success)
                    {
                        if (NegoSchedule > m_objTTCmembersDA.Data.PeriodEnd ||
                            NegoSchedule < m_objTTCmembersDA.Data.PeriodStart)
                        {
                            m_lstReturn.Add("TC Member : " + tcmember + " didn't match the ScheduleTime");
                            break;
                        }
                    }
                    else
                    {
                        m_lstReturn.Add("Error get TC Member Period (" + tcmember + ")");
                        break;
                    }

                }
            }

            if (!isRenego)
            {
                if (BPVM.Count <= 0)
                    m_lstReturn.Add("Please select at least 1 Budget Plan");
                else
                    foreach (BudgetPlanVM BV in BPVM)
                    {
                        //Revisi jadi sementara BUnit ga ditampilin
                        //if (string.IsNullOrEmpty(BV.TCProjectType))
                        //m_lstReturn.Add(TCMembersVM.Prop.TCTypeDesc.Desc + " at " + BV.BudgetPlanID + " " + General.EnumDesc(MessageLib.mustFill));
                    }
            }
            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(FPTVM.Prop.FPTID.Name, parameters[FPTVM.Prop.FPTID.Name]);
            m_dicReturn.Add(FPTVM.Prop.Descriptions.Name, parameters[FPTVM.Prop.Descriptions.Name]);
            m_dicReturn.Add(FPTVM.Prop.Schedule.Name, parameters[FPTVM.Prop.Schedule.Name]);
            m_dicReturn.Add(FPTVM.Prop.NegoLevel.Name, parameters[FPTVM.Prop.NegoLevel.Name]);
            m_dicReturn.Add(FPTVM.Prop.NegoRound.Name, parameters[FPTVM.Prop.NegoRound.Name]);
            m_dicReturn.Add(FPTVM.Prop.NegoRoundTime.Name, parameters[FPTVM.Prop.NegoRoundTime.Name]);
            m_dicReturn.Add(FPTVM.Prop.TRMLead.Name, parameters[FPTVM.Prop.TRMLead.Name]);
            m_dicReturn.Add(FPTVM.Prop.TaskID.Name, parameters[FPTVM.Prop.TaskID.Name]);

            return m_dicReturn;
        }

        private FPTVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            FPTVM m_objFPTVM = new FPTVM();
            MFPTDA m_objMFPTDA = new MFPTDA();
            m_objMFPTDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(FPTVM.Prop.FPTID.MapAlias);
            m_lstSelect.Add(FPTVM.Prop.Descriptions.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objFPTVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(FPTVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }
            //

            Dictionary<int, DataSet> m_dicMFPTDA = m_objMFPTDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMFPTDA.Message == string.Empty)
            {
                DataRow m_drMFPTDA = m_dicMFPTDA[0].Tables[0].Rows[0];
                m_objFPTVM.FPTID = m_drMFPTDA[FPTVM.Prop.FPTID.Name].ToString();
                m_objFPTVM.Descriptions = m_drMFPTDA[FPTVM.Prop.Descriptions.Name].ToString();
                m_objFPTVM.ByUpload = false;
                m_objFPTVM.ListNegotiationConfigurationsVM = GetListNegoConfiguration(m_objFPTVM.FPTID, null, ref message);
                m_objFPTVM.ListTCMembers = new List<TCMembersVM>();
                m_objFPTVM.ListProject = new List<ProjectVM>();
                m_objFPTVM.ListBFPTVendorParticipantsVM = new List<FPTVendorParticipantsVM>();
                var NegoConfID = m_objFPTVM.ListNegotiationConfigurationsVM
                                                     .Where(u => u.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.BudgetPlan))
                                                     .GroupBy(u => u.BudgetPlanID)
                                                     .Select(grp => grp.ToList())
                                                     .ToList();

                List<string> negoConf = new List<string>();
                foreach (var j in NegoConfID)
                    negoConf.Add(j[0].NegotiationConfigID);

                m_objFPTVM.ListBFPTVendorParticipantsVM = GetFPTarticipants(negoConf, ref message);

                if (m_objFPTVM.ListNegotiationConfigurationsVM != null)
                    if (m_objFPTVM.ListNegotiationConfigurationsVM.Count > 0)
                    {
                        {
                            foreach (NegotiationConfigurationsVM n in m_objFPTVM.ListNegotiationConfigurationsVM.Where(x => x.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.TRMLead)))
                            {
                                m_objFPTVM.TRMLead = n.ParameterValue;
                                m_objFPTVM.TRMLeadDesc = n.TRMLeadDesc;
                            }
                            foreach (NegotiationConfigurationsVM n in m_objFPTVM.ListNegotiationConfigurationsVM.Where(x => x.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.SubItemLevel)))
                                m_objFPTVM.NegoLevel = n.ParameterValue;
                            foreach (NegotiationConfigurationsVM n in m_objFPTVM.ListNegotiationConfigurationsVM.Where(x => x.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.Round)))
                                m_objFPTVM.NegoRound = n.ParameterValue;
                            foreach (NegotiationConfigurationsVM n in m_objFPTVM.ListNegotiationConfigurationsVM.Where(x => x.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.RoundTime)))
                                m_objFPTVM.NegoRoundTime = n.ParameterValue;
                            foreach (NegotiationConfigurationsVM n in m_objFPTVM.ListNegotiationConfigurationsVM.Where(x => x.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.Schedule)))
                                m_objFPTVM.Schedule = n.ParameterValue;


                            var grpPoject = m_objFPTVM.ListNegotiationConfigurationsVM
                                                      .Where(u => u.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.BudgetPlan))
                                                      .GroupBy(u => u.ProjectID)
                                                      .Select(grp => grp.ToList())
                                                      .ToList();
                            string bUnit = "";
                            foreach (var s in grpPoject)
                            {
                                ProjectVM objProject = new ProjectVM();
                                objProject.ProjectID = s[0].ProjectID;
                                objProject.ProjectDesc = s[0].ProjectDesc;
                                objProject.ListBudgetPlanID = new List<BudgetPlanVM>();
                                //objProject.ListVendorParticipants = new List<FPTVendorParticipantsVM>();

                                foreach (NegotiationConfigurationsVM n in m_objFPTVM.ListNegotiationConfigurationsVM.Where(x => x.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.BudgetPlan) && x.ProjectID == objProject.ProjectID))
                                {
                                    BudgetPlanVM BPID = new BudgetPlanVM();
                                    BPID.VendorParticipant = new List<VendorVM>();
                                    foreach (FPTVendorParticipantsVM ns in m_objFPTVM.ListBFPTVendorParticipantsVM.Where(x => x.NegotiationConfigID == n.NegotiationConfigID))
                                    {
                                        VendorVM vndrParticipants = new VendorVM();
                                        vndrParticipants.VendorID = ns.VendorID;
                                        vndrParticipants.VendorDesc = ns.FirstName;
                                        BPID.VendorParticipant.Add(vndrParticipants);
                                    }
                                    BPID.BudgetPlanID = n.BudgetPlanID;
                                    BPID.TCProjectType = n.ParameterValue2;
                                    bUnit = n.ParameterValue2;
                                    BPID.TCTypeDesc = n.TCTypeDesc;
                                    objProject.ListBudgetPlanID.Add(BPID);
                                    m_objFPTVM.NegoConfigBUnitDesc = n.NegoBUnitDesc;
                                }


                                m_objFPTVM.ListProject.Add(objProject);

                            }


                            //foreach (NegotiationConfigurationsVM n in m_objFPTVM.ListNegotiationConfigurationsVM.Where(x => x.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.Vendor)))
                            //{
                            //    FPTVendorParticipantsVM objVendorParticipants = new FPTVendorParticipantsVM();
                            //    objVendorParticipants.VendorID = n.VendorID;
                            //    objVendorParticipants.VendorName = n.FirstName;
                            //    m_objFPTVM.ListBFPTVendorParticipantsVM.Add(objVendorParticipants);
                            //}

                            foreach (NegotiationConfigurationsVM n in m_objFPTVM.ListNegotiationConfigurationsVM.Where(x => x.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.TCMember)))
                            {
                                TCMembersVM objTCMember = new TCMembersVM();
                                objTCMember.TCMemberID = n.TCMemberID;
                                objTCMember.EmployeeID = n.EmployeeID;
                                objTCMember.TCTypeID = n.TCTypeID;
                                objTCMember.EmployeeName = n.EmployeeName;
                                objTCMember.BusinessUnitDesc = n.BusinessUnitDesc;
                                objTCMember.Email = "";
                                m_objFPTVM.ListTCMembers.Add(objTCMember);
                            }

                            foreach (NegotiationConfigurationsVM n in m_objFPTVM.ListNegotiationConfigurationsVM.Where(x => x.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.BudgetPlan)))
                            {
                                m_objFPTVM.ByUpload = string.IsNullOrEmpty(n.ParameterValue);
                                if (m_objFPTVM.ByUpload)
                                    m_objFPTVM.TCTypeUpload = n.ParameterValue2;
                            }
                            m_objFPTVM.ScheduleDate = DateTime.Parse(m_objFPTVM.Schedule);
                            //m_objFPTVM.ScheduleTimeHour = DateTime.Parse(m_objFPTVM.Schedule).ToString(Global.ShortTimeFormat);
                            m_objFPTVM.FPTStatusID = int.Parse(m_objFPTVM.ListNegotiationConfigurationsVM[0].FPTStatusID);
                            m_objFPTVM.TaskID = m_objFPTVM.ListNegotiationConfigurationsVM[0].TaskID;
                            m_objFPTVM.TaskStatus = m_objFPTVM.ListNegotiationConfigurationsVM[0].StatusID;
                            m_objFPTVM.TCType = m_objFPTVM.ListTCMembers[0].TCTypeID;
                            m_objFPTVM.BusinessUnitID = bUnit;

                            m_objFPTVM.CurrentApprovalLvl = m_objFPTVM.ListNegotiationConfigurationsVM[0].CurrentApprovalLvl;
                        }
                    }

            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMFPTDA.Message;

            return m_objFPTVM;
        }
        private FPTVM GetForAdd(ref string message)
        {
            string NegoLevel = "";
            string NegoRound = "";
            string NegoRoundTime = "";

            UConfigDA uConfigDA = new UConfigDA();
            uConfigDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ConfigVM.Prop.Key3.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Key2.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add("NegoConfig");
            m_objFilter.Add(ConfigVM.Prop.Key1.Map, m_lstFilter);

            Dictionary<int, DataSet> dicuConfigDA = uConfigDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (uConfigDA.Message == string.Empty)
            {
                foreach (DataRow dr_Config in dicuConfigDA[0].Tables[0].Rows)
                    if (!string.IsNullOrEmpty(dr_Config[ConfigVM.Prop.Key3.Name].ToString()))
                    {
                        if (dr_Config[ConfigVM.Prop.Key2.Name].ToString() == "DefaultNegoLevel")
                        {
                            NegoLevel = dr_Config[ConfigVM.Prop.Key3.Name].ToString();
                        }
                        else if (dr_Config[ConfigVM.Prop.Key2.Name].ToString() == "DefaultNegoRound")
                        {
                            NegoRound = dr_Config[ConfigVM.Prop.Key3.Name].ToString();
                        }
                        else if (dr_Config[ConfigVM.Prop.Key2.Name].ToString() == "DefaultNegoRoundTime")
                        {
                            NegoRoundTime = dr_Config[ConfigVM.Prop.Key3.Name].ToString();
                        }
                    }
            }
            else
            {
                message = "Error while getting Configuration DefaultValue: " + uConfigDA.Message;
                return new FPTVM();
            }

            FPTVM m_objFPTVM = new FPTVM();
            m_objFPTVM.FPTID = string.Empty;
            m_objFPTVM.Descriptions = string.Empty;
            m_objFPTVM.ScheduleDate = DateTime.Now;
            m_objFPTVM.ListNegotiationConfigurationsVM = new List<NegotiationConfigurationsVM>();
            m_objFPTVM.ListTCMembers = new List<TCMembersVM>();
            m_objFPTVM.ListProject = new List<ProjectVM>();
            m_objFPTVM.ListBFPTVendorParticipantsVM = new List<FPTVendorParticipantsVM>();
            m_objFPTVM.NegoLevel = NegoLevel;
            m_objFPTVM.NegoRound = NegoRound;
            m_objFPTVM.NegoRoundTime = NegoRoundTime;
            m_objFPTVM.TCType = string.Empty;
            m_objFPTVM.BusinessUnitID = string.Empty;
            m_objFPTVM.FPTStatusID = General.EnumOrder(FPTStatusTypes.NegotiationConfiguration);
            m_objFPTVM.TaskStatus = string.Empty;

            return m_objFPTVM;
        }


        private DataSet GetFPTComaparation(string FPTID)
        {
            DataSet dt = new DataSet();
            List<string> m_lstSelect = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();

            DFPTVendorParticipantsDA m_objVendorParticipants = new DFPTVendorParticipantsDA();
            m_objVendorParticipants.ConnectionStringName = Global.ConnStrConfigName;

            m_lstFilter = new List<object>();
            m_objFilter = new Dictionary<string, List<object>>();

            m_lstFilter.Add(Operator.None);
            m_lstFilter.Add(String.Empty);
            m_objFilter.Add(String.Format("{0} IN (select NegotiationConfigID from CNegotiationConfigurations where FPTID = '{1}')", FPTVendorParticipantsVM.Prop.NegotiationConfigID.Map, FPTID), m_lstFilter);


            Dictionary<int, DataSet> m_dicVendorParticipantsDA = m_objVendorParticipants.SelectBC(FPTID, m_objFilter);
            dt = m_dicVendorParticipantsDA[0];

            return dt;
        }

        #endregion


        #region Public Method

        public Dictionary<int, List<CompanyVM>> GetCompanyData(bool isCount, string CompanyID, string CompanyDesc)
        {
            int m_intCount = 0;
            List<CompanyVM> m_lstCompanyVM = new List<ViewModels.CompanyVM>();
            Dictionary<int, List<CompanyVM>> m_dicReturn = new Dictionary<int, List<CompanyVM>>();
            MCompanyDA m_objMCompanyDA = new MCompanyDA();
            m_objMCompanyDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(CompanyVM.Prop.CompanyID.MapAlias);
            m_lstSelect.Add(CompanyVM.Prop.CompanyDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(CompanyID);
            m_objFilter.Add(CompanyVM.Prop.CompanyID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(CompanyDesc);
            m_objFilter.Add(CompanyVM.Prop.CompanyDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMCompanyDA = m_objMCompanyDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMCompanyDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpCompanyBL in m_dicMCompanyDA)
                    {
                        m_intCount = m_kvpCompanyBL.Key;
                        break;
                    }
                else
                {
                    m_lstCompanyVM = (
                        from DataRow m_drMCompanyDA in m_dicMCompanyDA[0].Tables[0].Rows
                        select new CompanyVM()
                        {
                            CompanyID = m_drMCompanyDA[CompanyVM.Prop.CompanyID.Name].ToString(),
                            CompanyDesc = m_drMCompanyDA[CompanyVM.Prop.CompanyDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstCompanyVM);
            return m_dicReturn;
        }

        #endregion
    }
}