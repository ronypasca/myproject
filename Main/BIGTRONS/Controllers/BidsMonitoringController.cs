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
    public class BidsMonitoringController : BaseController
    {
        private readonly string title = "Bid Monitoring";
        private readonly string dataSessionName = "FormData";

        #region Public Action

        public ActionResult Index()
        {
            Initialize();
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
            MFPTDA m_objMFPTDA = new MFPTDA();
            m_objMFPTDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcCApprovalPath = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            foreach (FilterHeaderCondition m_fhcFilter in m_fhcCApprovalPath.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);


                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = FPTNegotiationRoundVM.Prop.Map(m_strDataIndex, false);

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
            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            foreach (DataSorter m_dtsOrder in parameters.Sort)
                m_dicOrder.Add(FPTNegotiationRoundVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));


            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.NotEqual);
            m_lstFilter.Add(new DateTime(9999, 12, 31));
            m_objFilter.Add(FPTNegotiationRoundVM.Prop.StartDateTimeStamp.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.NotEqual);
            m_lstFilter.Add(new DateTime(9999, 12, 31));
            m_objFilter.Add(FPTNegotiationRoundVM.Prop.EndDateTimeStamp.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.None);
            m_lstFilter.Add(string.Empty);
            m_objFilter.Add(FPTNegotiationRoundVM.Prop.EndDateTimeStamp.Map + " IS NOT  NULL", m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.None);
            m_lstFilter.Add(string.Empty);
            m_objFilter.Add(FPTNegotiationRoundVM.Prop.StartDateTimeStamp.Map + " IS NOT NULL", m_lstFilter);

            Dictionary<int, DataSet> m_dicMFPTDA = m_objMFPTDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, m_dicOrder, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpApprovalPathBL in m_dicMFPTDA)
            {
                m_intCount = m_kvpApprovalPathBL.Key;
                break;
            }

            List<FPTNegotiationRoundVM> m_lstFPTNegotiationRoundVM = new List<FPTNegotiationRoundVM>();

            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(FPTVM.Prop.FPTID.MapAlias);
                m_lstSelect.Add(FPTVM.Prop.Descriptions.MapAlias);
                m_lstSelect.Add(FPTNegotiationRoundVM.Prop.StartDateTimeStamp.MapAlias);
                m_lstSelect.Add(FPTNegotiationRoundVM.Prop.EndDateTimeStamp.MapAlias);
                m_lstSelect.Add(FPTNegotiationRoundVM.Prop.TotalVendors.MapAlias);
                m_lstSelect.Add(FPTNegotiationRoundVM.Prop.Duration.MapAlias);
                m_lstSelect.Add(FPTNegotiationRoundVM.Prop.Round.MapAlias);
                m_lstSelect.Add(FPTNegotiationRoundVM.Prop.RoundNo.MapAlias);
                m_lstSelect.Add(FPTNegotiationRoundVM.Prop.RoundID.MapAlias);
                m_lstSelect.Add(FPTNegotiationRoundVM.Prop.TopValue.MapAlias);
                m_lstSelect.Add(FPTNegotiationRoundVM.Prop.BottomValue.MapAlias);
                m_lstSelect.Add(FPTVM.Prop.AdditionalInfo1.MapAlias);
                m_lstSelect.Add(FPTVM.Prop.AdditionalInfo1Desc.MapAlias);

                m_dicMFPTDA = m_objMFPTDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMFPTDA.Message == string.Empty)
                {
                    DateTime InvalidDate = new DateTime(9999, 12, 31, 0, 0, 0);

                    foreach (DataRow m_drMFPTDA in m_dicMFPTDA[0].Tables[0].Rows)
                    {
                        FPTNegotiationRoundVM m_objFPTNegotiationRoundVM = new FPTNegotiationRoundVM();

                        m_objFPTNegotiationRoundVM.FPTID = m_drMFPTDA[FPTNegotiationRoundVM.Prop.FPTID.Name].ToString();
                        m_objFPTNegotiationRoundVM.FPTDesc = m_drMFPTDA[FPTNegotiationRoundVM.Prop.Descriptions.Name].ToString();
                        m_objFPTNegotiationRoundVM.TotalVendors = (!string.IsNullOrEmpty(m_drMFPTDA[FPTNegotiationRoundVM.Prop.TotalVendors.Name].ToString()) ? int.Parse(m_drMFPTDA[FPTNegotiationRoundVM.Prop.TotalVendors.Name].ToString()) : (int?)null);
                        m_objFPTNegotiationRoundVM.StartDateTimeStamp = !string.IsNullOrEmpty(m_drMFPTDA[FPTNegotiationRoundVM.Prop.StartDateTimeStamp.Name].ToString()) ? DateTime.Parse(m_drMFPTDA[FPTNegotiationRoundVM.Prop.StartDateTimeStamp.Name].ToString()) : (DateTime?)null;
                        m_objFPTNegotiationRoundVM.EndDateTimeStamp = !string.IsNullOrEmpty(m_drMFPTDA[FPTNegotiationRoundVM.Prop.EndDateTimeStamp.Name].ToString()) ? DateTime.Parse(m_drMFPTDA[FPTNegotiationRoundVM.Prop.EndDateTimeStamp.Name].ToString()) : (DateTime?)null;
                        m_objFPTNegotiationRoundVM.RoundID = m_drMFPTDA[FPTNegotiationRoundVM.Prop.RoundID.Name].ToString();
                        m_objFPTNegotiationRoundVM.Duration = string.IsNullOrEmpty(m_drMFPTDA[FPTNegotiationRoundVM.Prop.Duration.Name].ToString()) ? (int?)null : int.Parse(m_drMFPTDA[FPTNegotiationRoundVM.Prop.Duration.Name].ToString());
                        m_objFPTNegotiationRoundVM.TotalRound = string.IsNullOrEmpty(m_drMFPTDA[FPTNegotiationRoundVM.Prop.Round.Name].ToString()) ? (int?)null : int.Parse(m_drMFPTDA[FPTNegotiationRoundVM.Prop.Round.Name].ToString());
                        m_objFPTNegotiationRoundVM.RoundNo = string.IsNullOrEmpty(m_drMFPTDA[FPTNegotiationRoundVM.Prop.RoundNo.Name].ToString()) ? (int?)null : int.Parse(m_drMFPTDA[FPTNegotiationRoundVM.Prop.RoundNo.Name].ToString());
                        m_objFPTNegotiationRoundVM.TopValue = string.IsNullOrEmpty(m_drMFPTDA[FPTNegotiationRoundVM.Prop.TopValue.Name].ToString()) ? 0 : decimal.Parse(m_drMFPTDA[FPTNegotiationRoundVM.Prop.TopValue.Name].ToString());
                        m_objFPTNegotiationRoundVM.BottomValue = string.IsNullOrEmpty(m_drMFPTDA[FPTNegotiationRoundVM.Prop.BottomValue.Name].ToString()) ? 0 : decimal.Parse(m_drMFPTDA[FPTNegotiationRoundVM.Prop.BottomValue.Name].ToString());
                        m_objFPTNegotiationRoundVM.AdditionalInfo1Desc = m_drMFPTDA[FPTVM.Prop.AdditionalInfo1Desc.Name].ToString();
                        m_lstFPTNegotiationRoundVM.Add(m_objFPTNegotiationRoundVM);
                    }
                }
            }
            return this.Store(m_lstFPTNegotiationRoundVM, m_intCount);
        }

        public ActionResult Home()
        {
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 0;
            return this.Direct();
        }

        public ActionResult GetPanel(string FPTID, string RoundID, int lastLevel)
        {
            string m_strMessage = string.Empty;

            //Get All Structure
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();

            List<NegotiationBidStructuresVM> m_lstNegotiationBidStructuresVM = GetNegoStructure(FPTID);
            List<FPTNegotiationRoundVM> m_lstFPTNegotiationRoundVM = GetListNegoRound(FPTID);
            List<NegotiationConfigurationsVM> m_lstNegotiationConfigurationsVM = GetNegoConfig(FPTID, General.EnumDesc(NegoConfigTypes.BudgetPlan));
            List<FPTVendorParticipantsVM> m_lstFPTVendorParticipantsVM = GetVendorParticipant(FPTID);
         
            //Get Project List
            List<string> ProjectList = m_lstNegotiationBidStructuresVM.Where(d => !string.IsNullOrEmpty(d.ProjectID)).Select(x => x.ProjectID).Distinct().ToList();

            //if (!m_lstNegotiationBidStructuresVM.Any())
            //{
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));
            //}

            if (ProjectList.Any() && ProjectList.FirstOrDefault() != string.Empty)
            {
                //Project Tab Panel
                TabPanel ProjectTabPanel = new TabPanel
                {
                    ID = "TPProject",
                    ActiveTabIndex = 0,
                    Border = false
                };
                //List Panel Project
                foreach (var item in ProjectList)
                {
                    //Project Panel
                    Panel ProjectPanel = new Panel
                    {
                        //ID = "P" + item + "Form",
                        Frame = true,
                        Title = m_lstNegotiationBidStructuresVM.Where(x => x.ProjectID == item).FirstOrDefault().ProjectDesc,
                        Border = false
                    };
                    //BP Tab Panel
                    TabPanel BPTabPanel = new TabPanel
                    {
                        ActiveTabIndex = 0,
                        Border = false
                    };
                    //Get BP List
                    List<string> BPList = m_lstNegotiationBidStructuresVM.Where(y => y.ProjectID == item).Select(x => x.ParameterValue).Distinct().ToList();
                    foreach (var BPitem in BPList)
                    {
                        //BP Panel
                        Panel BPPanel = new Panel
                        {
                            //ID = "P" + BPitem + "Form",
                            Frame = true,
                            Title = m_lstNegotiationBidStructuresVM.FirstOrDefault(y => y.ParameterValue == BPitem).BPVersionName,
                            Border = false
                        };
                        Toolbar m_BPPanelToolbar = new Toolbar();
                        TreePanel m_TPBP = GenerateTreePanel(m_lstFPTVendorParticipantsVM, m_lstFPTNegotiationRoundVM, m_lstNegotiationBidStructuresVM.Where(x => x.ParameterValue == BPitem).ToList(), lastLevel, BPitem, RoundID);


                        BPPanel.TopBar.Add(m_BPPanelToolbar);
                        Button m_btnBPExCol = new Button() { ID = "BtnExpand" + m_TPBP.ID, Text = "Collapse" };
                        m_btnBPExCol.Handler = "ExColHandler";
                        m_btnBPExCol.TagString = m_TPBP.ID;
                        m_BPPanelToolbar.Items.Add(m_btnBPExCol);

                        Button m_btnBPViewLastOffer = new Button() { EnableToggle = true, ToggleHandler = "handlerLoadOffer", ID = "BtnViewLastOffer" + m_TPBP.ID, Text = "Load Last Offer" };
                        m_btnBPViewLastOffer.TagString = BPitem;
                        m_BPPanelToolbar.Items.Add(m_btnBPViewLastOffer);

                        BPPanel.Items.Add(m_TPBP);
                        BPTabPanel.Items.Add(BPPanel);
                    }

                    ProjectPanel.Items.Add(BPTabPanel);
                    ProjectTabPanel.Items.Add(ProjectPanel);
                }
                return this.ComponentConfig(ProjectTabPanel);

            }
            else
            {
                string m_strValueID = m_lstNegotiationConfigurationsVM.FirstOrDefault().ParameterValue;

                Panel BPPanel = new Panel
                {
                    ID = "P" + "Upload" + "Form",//todo
                    Frame = true,
                    Title = string.IsNullOrEmpty(m_strValueID) ? "Upload" : m_strValueID,//todo
                    Border = false
                };

                m_lstFPTVendorParticipantsVM.ForEach(d => d.BudgetPlanID = m_lstNegotiationConfigurationsVM.FirstOrDefault().ParameterValue);


                Toolbar m_BPPanelToolbar = new Toolbar();
                TreePanel m_TPBP = GenerateTreePanel(m_lstFPTVendorParticipantsVM, m_lstFPTNegotiationRoundVM, m_lstNegotiationBidStructuresVM.ToList(), lastLevel, m_strValueID, RoundID);

                List<Parameter> m_lstParameter = new List<Parameter>();

                BPPanel.TopBar.Add(m_BPPanelToolbar);
                Button m_btnBPExCol = new Button() { ID = "BtnExpand" + m_TPBP.ID, Text = "Collapse" };
                m_btnBPExCol.Handler = "ExColHandler";
                m_btnBPExCol.TagString = m_TPBP.ID;
                m_BPPanelToolbar.Items.Add(m_btnBPExCol);

                Button m_btnBPViewLastOffer = new Button() { EnableToggle=true, ToggleHandler= "handlerLoadOffer", ID = "BtnViewLastOffer" + m_TPBP.ID, Text = "Load Last Offer" };
                
                m_btnBPViewLastOffer.TagString = m_strValueID;

                m_BPPanelToolbar.Items.Add(m_btnBPViewLastOffer);
                BPPanel.Items.Add(m_TPBP);
                return this.ComponentConfig(BPPanel);
            }

        }

        public ActionResult Monitoring(string Caller, string Selected, string FPTID = "", string RoundID = "")
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Update)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));



            string m_strFPTID;
            string m_strRoundID;
            decimal m_decUpperLimit = 0;
            decimal m_decLowerLimit = 0;
            if (FPTID != string.Empty && RoundID != string.Empty)
            {
                m_strFPTID = FPTID;
                m_strRoundID = RoundID;
            }
            else
            {
                if (string.IsNullOrEmpty(Selected))
                {
                    return this.Direct(false);
                }
                Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();
                m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
                m_strFPTID = m_dicSelectedRow[FPTNegotiationRoundVM.Prop.FPTID.Name].ToString();
                m_strRoundID = m_dicSelectedRow[FPTNegotiationRoundVM.Prop.RoundID.Name].ToString();
                m_decUpperLimit = decimal.Parse(m_dicSelectedRow[FPTNegotiationRoundVM.Prop.TopValue.Name].ToString());
                m_decLowerLimit = decimal.Parse(m_dicSelectedRow[FPTNegotiationRoundVM.Prop.BottomValue.Name].ToString());
            }

            string m_strMessage = string.Empty;

            if (!ValidateTCAccess(m_strFPTID, ref m_strMessage))
            {
                bool hasAccess = IsByPassToAccess();
                if (!hasAccess)
                {
                    Global.ShowErrorAlert(title, General.EnumDesc(MessageLib.NotAuthorized));
                    return this.Direct();
                }
            }



            List<FPTNegotiationRoundVM> m_lstFPTNegotiationRoundVM = GetNegoRound(m_strRoundID);


            //todo: check if empty configuration
            List<NegotiationConfigurationsVM> m_lstCNegotiationConfigurations = GetNegoConfig(m_strFPTID, General.EnumDesc(NegoConfigTypes.SubItemLevel));
            int m_intSubItemLevelData = Int16.Parse(m_lstCNegotiationConfigurations.FirstOrDefault().ParameterValue);
            DateTime m_objschedule = Convert.ToDateTime(GetNegoConfig(m_strFPTID, General.EnumDesc(NegoConfigTypes.Schedule)).FirstOrDefault().ParameterValue);
            m_lstFPTNegotiationRoundVM.ForEach(i => i.Schedule = m_objschedule);

            List<ConfigVM> m_lstConfigVM = GetValuePriceTolerance();
            decimal m_decMarginTop = m_lstConfigVM.Where(x => x.Key2 == "TOP").Any() ? Convert.ToDecimal(m_lstConfigVM.Where(x => x.Key2 == "TOP").FirstOrDefault().Desc1) : 0;
            decimal m_decMarginBottom = m_lstConfigVM.Where(x => x.Key2 == "BOTTOM").Any() ? Convert.ToDecimal(m_lstConfigVM.Where(x => x.Key2 == "TOP").FirstOrDefault().Desc1) : 0;

            FPTNegotiationRoundVM m_objFPTNegotiationRoundVM = m_lstFPTNegotiationRoundVM.FirstOrDefault();
            List<FPTAdditionalInfoVM> m_ListFPTAdditionalInfoVM = GetListFPTAdditionalInfoVM(m_objFPTNegotiationRoundVM.FPTID, ref m_strMessage);
            DateTime m_dtstart = DateTime.MinValue;
            if (m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "1").Any())
            {
                DateTime.TryParse(m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "1").FirstOrDefault().Value, out m_dtstart);
            }
            DateTime m_dtend = DateTime.MinValue;
            if (m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "2").Any())
            {
                DateTime.TryParse(m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "2").FirstOrDefault().Value, out m_dtend);
            }

            bool m_isManual = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "11").Any() || m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "12").Any() || m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "13").Any();

            string m_FPTScheduleStart = m_dtstart.ToString(Global.DefaultDateFormat);
            string m_FPTScheduleEnd = m_dtend.ToString(Global.DefaultDateFormat);
            string m_Duration = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "3").Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "3").FirstOrDefault().Value : string.Empty;

            if (m_isManual)
            {
                m_FPTScheduleStart = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "11").Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "11").FirstOrDefault().Value : string.Empty;
                m_FPTScheduleEnd = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "12").Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "12").FirstOrDefault().Value : string.Empty;
                m_Duration = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "13").Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "13").FirstOrDefault().Value : string.Empty;
            }

            m_objFPTNegotiationRoundVM.FPTScheduleStart = m_dtstart;
            m_objFPTNegotiationRoundVM.FPTScheduleEnd = m_dtend;
            m_objFPTNegotiationRoundVM.FPTDuration = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "3").Any() ? m_ListFPTAdditionalInfoVM.FirstOrDefault(x => x.FPTAdditionalInfoItemID == "3").Value : null;
            m_objFPTNegotiationRoundVM.MaintenancePeriod = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "4").Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "4").FirstOrDefault().Value : null;
            m_objFPTNegotiationRoundVM.Guarantee = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "5").Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "5").FirstOrDefault().Value : null;
            m_objFPTNegotiationRoundVM.ContractType = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "6").Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "6").FirstOrDefault().Value : null;
            m_objFPTNegotiationRoundVM.PaymentMethod = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "7").Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "7").FirstOrDefault().Value : null;

            string m_typeftp = "";
            m_typeftp = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "14").Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "14").FirstOrDefault().Value : (m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "15").Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "15").FirstOrDefault().Value : "");


            ViewDataDictionary m_vddBidMonitoring = new ViewDataDictionary();
            m_vddBidMonitoring.Add("FPTIDData", m_strFPTID);
            m_vddBidMonitoring.Add("RoundIDData", m_strRoundID);
            m_vddBidMonitoring.Add("SubItemLevelData", m_intSubItemLevelData);
            m_vddBidMonitoring.Add("FPTNameData", m_strFPTID);
            //m_vddBidMonitoring.Add("BusinessUnitIDs", m_strBusinessUnitIDs);
            m_vddBidMonitoring.Add("PriceMarginTopData", m_decUpperLimit);
            m_vddBidMonitoring.Add("PriceMarginBottomData", m_decLowerLimit);
            m_vddBidMonitoring.Add("ScheduleStart", m_FPTScheduleStart);
            m_vddBidMonitoring.Add("ScheduleEnd", m_FPTScheduleEnd);
            m_vddBidMonitoring.Add("Duration", m_Duration);
            m_vddBidMonitoring.Add(FPTVM.Prop.AdditionalInfo1Desc.MapAlias, m_typeftp);
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;

            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objFPTNegotiationRoundVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddBidMonitoring,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult GetBids(string BudgetPlanID, string RoundID, string FPTID)
        {
            string m_strMessage = string.Empty;

            List<NegotiationBidEntryVM> m_lstNegotiationBidEntryVM = GetNegoEntry(BudgetPlanID, RoundID, FPTID, true);

            //if (m_strMessage != string.Empty)
            //{
            //    return this.Direct(false, m_strMessage);
            //}
            return this.Direct(m_lstNegotiationBidEntryVM);
        }

        public ActionResult GetLastOffer(string BudgetPlanID, string RoundID)
        {
            string m_strMessage = string.Empty;

            List<NegotiationBidEntryVM> m_lstNegotiationBidEntryVM = GetNegoEntry(BudgetPlanID, RoundID);

            //if (m_strMessage != string.Empty)
            //{
            //    return this.Direct(false, m_strMessage);
            //}
            return this.Direct(m_lstNegotiationBidEntryVM);
        }

        public ActionResult GetTotalRounds(string BudgetPlanID)
        {
            string m_strMessage = string.Empty;

            List<NegotiationBidEntryVM> m_lstNegotiationBidEntryVM = GetNegoEntry(BudgetPlanID, "-");

            //var m_objHistoryTotalAllRound = m_lstNegotiationBidEntryVM.Where(d=>d.RoundID!="").GroupBy(d => new { d.RoundID,d.VendorID }).Select(e=> new { RoundID = e.Key.RoundID,VendorID = e.Key.VendorID, BidValue = e.Sum(p => p.BidValue) }).OrderBy(d=>d.VendorID).ToList();
            VendorBidTypes[] bidType = (VendorBidTypes[])System.Enum.GetValues(typeof(VendorBidTypes)).Cast<VendorBidTypes>();
            var m_objHistoryTotalAllRound = m_lstNegotiationBidEntryVM.Where(d => d.RoundID != "" && bidType.Select(x => (int)x).ToList().Contains(d.Sequence.Value)).OrderBy(d => d.VendorID).ToList();

            return this.Direct(m_objHistoryTotalAllRound);
        }
        #endregion

        private List<NegotiationBidStructuresVM> GetNegoStructure(string FPTID)
        {
            List<NegotiationBidStructuresVM> m_lstNegotiationBidStructuresVM = new List<NegotiationBidStructuresVM>();


            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();
            TNegotiationBidStructuresDA m_objTNegotiationBidStructuresDA = new TNegotiationBidStructuresDA();
            m_objTNegotiationBidStructuresDA.ConnectionStringName = Global.ConnStrConfigName;


            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(FPTID);
            m_objFilter.Add(NegotiationBidStructuresVM.Prop.FPTID.Map, m_lstFilter);

            m_lstSelect = new List<string>();
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.NegotiationBidID.MapAlias);
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.NegotiationConfigID.MapAlias);
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.Sequence.MapAlias);
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.ItemParentID.MapAlias);
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.ParameterValue2.MapAlias);
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.BPVersionName.MapAlias);
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.FPTID.MapAlias);
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.ParameterValue.MapAlias);
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.ProjectID.MapAlias);
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.ProjectDesc.MapAlias);
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.BudgetPlanDefaultValue.MapAlias);
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.Version.MapAlias);
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.ParentVersion.MapAlias);
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.ParentSequence.MapAlias);


            Dictionary<int, DataSet> m_dicTNegotiationBidStructuresDA = m_objTNegotiationBidStructuresDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objTNegotiationBidStructuresDA.Success)
            {
                foreach (DataRow item in m_dicTNegotiationBidStructuresDA[0].Tables[0].Rows)
                {
                    NegotiationBidStructuresVM m_objNegotiationBidStructuresVM = new NegotiationBidStructuresVM();
                    m_objNegotiationBidStructuresVM.NegotiationBidID = item[NegotiationBidStructuresVM.Prop.NegotiationBidID.Name].ToString();
                    m_objNegotiationBidStructuresVM.NegotiationConfigID = item[NegotiationBidStructuresVM.Prop.NegotiationConfigID.Name].ToString();
                    m_objNegotiationBidStructuresVM.Sequence = Convert.ToInt32(item[NegotiationBidStructuresVM.Prop.Sequence.Name].ToString());
                    m_objNegotiationBidStructuresVM.ItemID = item[NegotiationBidStructuresVM.Prop.ItemID.Name].ToString();
                    m_objNegotiationBidStructuresVM.ItemDesc = item[NegotiationBidStructuresVM.Prop.ItemDesc.Name].ToString();
                    m_objNegotiationBidStructuresVM.ItemParentID = item[NegotiationBidStructuresVM.Prop.ItemParentID.Name].ToString();
                    //m_objNegotiationBidStructuresVM.VendorID = item[NegotiationBidStructuresVM.Prop.VendorID.Name].ToString();
                    //m_objNegotiationBidStructuresVM.VendorDesc = item[NegotiationBidStructuresVM.Prop.VendorDesc.Name].ToString();
                    m_objNegotiationBidStructuresVM.FPTID = item[NegotiationBidStructuresVM.Prop.FPTID.Name].ToString();
                    m_objNegotiationBidStructuresVM.ParameterValue = item[NegotiationBidStructuresVM.Prop.ParameterValue.Name].ToString();
                    m_objNegotiationBidStructuresVM.ParameterValue2 = item[NegotiationBidStructuresVM.Prop.ParameterValue2.Name].ToString();
                    m_objNegotiationBidStructuresVM.ProjectID = item[NegotiationBidStructuresVM.Prop.ProjectID.Name].ToString();
                    m_objNegotiationBidStructuresVM.ProjectDesc = item[NegotiationBidStructuresVM.Prop.ProjectDesc.Name].ToString();
                    m_objNegotiationBidStructuresVM.BudgetPlanDefaultValue = (decimal)item[NegotiationBidStructuresVM.Prop.BudgetPlanDefaultValue.Name];
                    m_objNegotiationBidStructuresVM.Version = (string.IsNullOrEmpty(item[NegotiationBidStructuresVM.Prop.Version.Name].ToString())) ? null : (int?)item[NegotiationBidStructuresVM.Prop.Version.Name];
                    m_objNegotiationBidStructuresVM.ParentVersion = (string.IsNullOrEmpty(item[NegotiationBidStructuresVM.Prop.ParentVersion.Name].ToString())) ? null : (int?)item[NegotiationBidStructuresVM.Prop.ParentVersion.Name];
                    m_objNegotiationBidStructuresVM.ParentSequence = (string.IsNullOrEmpty(item[NegotiationBidStructuresVM.Prop.ParentSequence.Name].ToString())) ? null : (int?)item[NegotiationBidStructuresVM.Prop.ParentSequence.Name];
                    m_objNegotiationBidStructuresVM.BPVersionName = item[NegotiationBidStructuresVM.Prop.BPVersionName.Name].ToString();
                    m_lstNegotiationBidStructuresVM.Add(m_objNegotiationBidStructuresVM);

                }
            }

            return m_lstNegotiationBidStructuresVM.Distinct().ToList();
        }

        private List<NegotiationBidEntryVM> GetNegoEntry(string BudgetPlanID, string RoundID,string FPTID="",bool IsLastBid=false)
        {
            List<NegotiationBidEntryVM> m_lstNegotiationBidEntryVM = new List<NegotiationBidEntryVM>();

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();
            DNegotiationBidEntryDA m_objDNegotiationBidEntryDA = new DNegotiationBidEntryDA();
            m_objDNegotiationBidEntryDA.ConnectionStringName = Global.ConnStrConfigName;


            //m_lstFilter = new List<object>();
            //m_lstFilter.Add(Operator.Equals);
            //m_lstFilter.Add(FPTID);
            //m_objFilter.Add(NegotiationBidEntryVM.Prop.FPTID.Map, m_lstFilter);
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanID);
            m_objFilter.Add(NegotiationBidEntryVM.Prop.ParameterValue.Map, m_lstFilter);

            if (RoundID != "-")
                if (RoundID != string.Empty)
                {
                    if (IsLastBid)
                    {
                        m_lstFilter = new List<object>();
                        m_lstFilter.Add(Operator.None);
                        m_lstFilter.Add("");
                        m_objFilter.Add($"{NegotiationBidEntryVM.Prop.RoundID.Map} IN ( SELECT TOP 1 DFPTNegotiationRounds.RoundID FROM DFPTNegotiationRounds  JOIN DNegotiationBidEntry ON DNegotiationBidEntry.RoundID=DFPTNegotiationRounds.RoundID WHERE StartDateTimeStamp != '9999-12-31 00:00:00.000' AND  EndDateTimeStamp != '9999-12-31 00:00:00.000' AND DFPTNegotiationRounds.FPTID='{FPTID}' ORDER BY DFPTNegotiationRounds.RoundID DESC )", m_lstFilter);
                    }
                    else {
                        m_lstFilter = new List<object>();
                        m_lstFilter.Add(Operator.Equals);
                        m_lstFilter.Add(RoundID);
                        m_objFilter.Add(NegotiationBidEntryVM.Prop.RoundID.Map, m_lstFilter);
                    }
                }
                else
                {
                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.None);
                    m_lstFilter.Add("");
                    m_objFilter.Add($"{NegotiationBidEntryVM.Prop.RoundID.Map} IS NULL OR {NegotiationBidEntryVM.Prop.RoundID.Map} = ''  ", m_lstFilter);
                }


            m_lstSelect = new List<string>();
            m_lstSelect.Add(NegotiationBidEntryVM.Prop.FPTVendorParticipantID.MapAlias);
            m_lstSelect.Add(NegotiationBidEntryVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(NegotiationBidEntryVM.Prop.NegotiationEntryID.MapAlias);
            m_lstSelect.Add(NegotiationBidEntryVM.Prop.BidTypeID.MapAlias);
            m_lstSelect.Add(NegotiationBidEntryVM.Prop.NegotiationBidID.MapAlias);
            m_lstSelect.Add(NegotiationBidEntryVM.Prop.RoundID.MapAlias);
            m_lstSelect.Add(NegotiationBidEntryVM.Prop.BidValue.MapAlias);
            m_lstSelect.Add(NegotiationBidEntryVM.Prop.Sequence.MapAlias);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();

            m_dicOrder.Add(NegotiationBidEntryVM.Prop.NegotiationBidID.Map, OrderDirection.Ascending);


            Dictionary<int, DataSet> m_dicDNegotiationBidEntryDA = m_objDNegotiationBidEntryDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);

            if (m_objDNegotiationBidEntryDA.Success)
            {
                foreach (DataRow item in m_dicDNegotiationBidEntryDA[0].Tables[0].Rows)
                {
                    NegotiationBidEntryVM m_objNegotiationBidEntryVM = new NegotiationBidEntryVM();
                    m_objNegotiationBidEntryVM.FPTVendorParticipantID = item[NegotiationBidEntryVM.Prop.FPTVendorParticipantID.Name].ToString();
                    m_objNegotiationBidEntryVM.VendorID = item[NegotiationBidEntryVM.Prop.VendorID.Name].ToString();
                    m_objNegotiationBidEntryVM.NegotiationEntryID = item[NegotiationBidEntryVM.Prop.NegotiationEntryID.Name].ToString();
                    m_objNegotiationBidEntryVM.BidTypeID = item[NegotiationBidEntryVM.Prop.BidTypeID.Name].ToString();
                    m_objNegotiationBidEntryVM.NegotiationBidID = item[NegotiationBidEntryVM.Prop.NegotiationBidID.Name].ToString();
                    m_objNegotiationBidEntryVM.RoundID = item[NegotiationBidEntryVM.Prop.RoundID.Name].ToString();
                    m_objNegotiationBidEntryVM.BidValue = Convert.ToDecimal(item[NegotiationBidEntryVM.Prop.BidValue.Name].ToString());
                    m_objNegotiationBidEntryVM.Sequence = Convert.ToInt32(item[NegotiationBidEntryVM.Prop.Sequence.Name].ToString());
                    m_lstNegotiationBidEntryVM.Add(m_objNegotiationBidEntryVM);
                }
            }

            return m_lstNegotiationBidEntryVM;
        }

        private List<FPTNegotiationRoundVM> GetNegoRound(string RoundID, string FPTID = "")
        {
            List<FPTNegotiationRoundVM> m_lstFPTNegotiationRoundVM = new List<FPTNegotiationRoundVM>();

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();
            MFPTDA m_objMFPTDA = new MFPTDA();
            m_objMFPTDA.ConnectionStringName = Global.ConnStrConfigName;

            if (RoundID != string.Empty)
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(RoundID);
                m_objFilter.Add(FPTNegotiationRoundVM.Prop.RoundID.Map, m_lstFilter);
            }

            if (FPTID != string.Empty)
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(FPTID);
                m_objFilter.Add(FPTNegotiationRoundVM.Prop.FPTID.Map, m_lstFilter);
            }

            m_lstSelect = new List<string>();
            m_lstSelect.Add(FPTVM.Prop.FPTID.MapAlias);
            m_lstSelect.Add(FPTVM.Prop.Descriptions.MapAlias);
            m_lstSelect.Add(FPTNegotiationRoundVM.Prop.StartDateTimeStamp.MapAlias);
            m_lstSelect.Add(FPTNegotiationRoundVM.Prop.EndDateTimeStamp.MapAlias);
            m_lstSelect.Add(FPTNegotiationRoundVM.Prop.TotalVendors.MapAlias);
            m_lstSelect.Add(FPTNegotiationRoundVM.Prop.Duration.MapAlias);
            m_lstSelect.Add(FPTNegotiationRoundVM.Prop.Round.MapAlias);
            m_lstSelect.Add(FPTNegotiationRoundVM.Prop.RoundNo.MapAlias);
            m_lstSelect.Add(FPTNegotiationRoundVM.Prop.RoundID.MapAlias);
            m_lstSelect.Add(FPTNegotiationRoundVM.Prop.NextRoundID.MapAlias);
            m_lstSelect.Add(FPTNegotiationRoundVM.Prop.PreviousRoundID.MapAlias);

            Dictionary<int, DataSet> m_dicMFPTDA = m_objMFPTDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objMFPTDA.Success)
            {
                foreach (DataRow m_drMFPTDA in m_dicMFPTDA[0].Tables[0].Rows)
                {
                    FPTNegotiationRoundVM m_objFPTNegotiationRoundVM = new FPTNegotiationRoundVM();

                    m_objFPTNegotiationRoundVM.FPTID = m_drMFPTDA[FPTNegotiationRoundVM.Prop.FPTID.Name].ToString();
                    m_objFPTNegotiationRoundVM.FPTDesc = m_drMFPTDA[FPTNegotiationRoundVM.Prop.Descriptions.Name].ToString();
                    m_objFPTNegotiationRoundVM.TotalVendors = (!string.IsNullOrEmpty(m_drMFPTDA[FPTNegotiationRoundVM.Prop.TotalVendors.Name].ToString()) ? int.Parse(m_drMFPTDA[FPTNegotiationRoundVM.Prop.TotalVendors.Name].ToString()) : (int?)null);
                    m_objFPTNegotiationRoundVM.StartDateTimeStamp = !string.IsNullOrEmpty(m_drMFPTDA[FPTNegotiationRoundVM.Prop.StartDateTimeStamp.Name].ToString()) ? DateTime.Parse(m_drMFPTDA[FPTNegotiationRoundVM.Prop.StartDateTimeStamp.Name].ToString()) : (DateTime?)null;
                    m_objFPTNegotiationRoundVM.EndDateTimeStamp = !string.IsNullOrEmpty(m_drMFPTDA[FPTNegotiationRoundVM.Prop.EndDateTimeStamp.Name].ToString()) ? DateTime.Parse(m_drMFPTDA[FPTNegotiationRoundVM.Prop.EndDateTimeStamp.Name].ToString()) : (DateTime?)null;
                    m_objFPTNegotiationRoundVM.RoundID = m_drMFPTDA[FPTNegotiationRoundVM.Prop.RoundID.Name].ToString();
                    m_objFPTNegotiationRoundVM.FPTDuration = string.IsNullOrEmpty(m_drMFPTDA[FPTNegotiationRoundVM.Prop.Duration.Name].ToString()) ? null : m_drMFPTDA[FPTNegotiationRoundVM.Prop.Duration.Name].ToString();
                    m_objFPTNegotiationRoundVM.TotalRound = string.IsNullOrEmpty(m_drMFPTDA[FPTNegotiationRoundVM.Prop.Round.Name].ToString()) ? (int?)null : int.Parse(m_drMFPTDA[FPTNegotiationRoundVM.Prop.Round.Name].ToString());
                    m_objFPTNegotiationRoundVM.RoundNo = string.IsNullOrEmpty(m_drMFPTDA[FPTNegotiationRoundVM.Prop.RoundNo.Name].ToString()) ? (int?)null : int.Parse(m_drMFPTDA[FPTNegotiationRoundVM.Prop.RoundNo.Name].ToString());
                    //m_objFPTNegotiationRoundVM.Schedule = DateTime.Now;//todo:
                    m_objFPTNegotiationRoundVM.NextRoundID = m_drMFPTDA[FPTNegotiationRoundVM.Prop.NextRoundID.Name].ToString();
                    m_objFPTNegotiationRoundVM.PreviousRoundID = m_drMFPTDA[FPTNegotiationRoundVM.Prop.PreviousRoundID.Name].ToString();
                    m_lstFPTNegotiationRoundVM.Add(m_objFPTNegotiationRoundVM);
                }
            }
            return m_lstFPTNegotiationRoundVM;
        }

        private List<FPTNegotiationRoundVM> GetListNegoRound(string FPTID)
        {
            List<FPTNegotiationRoundVM> m_lstFPTNegotiationRoundVM = new List<FPTNegotiationRoundVM>();

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();
            DFPTNegotiationRoundsDA m_objDFPTNegotiationRoundsDA = new DFPTNegotiationRoundsDA();
            m_objDFPTNegotiationRoundsDA.ConnectionStringName = Global.ConnStrConfigName;

            if (FPTID != string.Empty)
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(FPTID);
                m_objFilter.Add(FPTNegotiationRoundVM.Prop.FPTID.Map, m_lstFilter);
            }

            m_lstSelect = new List<string>();
            m_lstSelect.Add(FPTNegotiationRoundVM.Prop.StartDateTimeStamp.MapAlias);
            m_lstSelect.Add(FPTNegotiationRoundVM.Prop.EndDateTimeStamp.MapAlias);
            m_lstSelect.Add(FPTNegotiationRoundVM.Prop.RoundID.MapAlias);

            Dictionary<int, DataSet> m_dicDFPTNegotiationRoundsDA = m_objDFPTNegotiationRoundsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objDFPTNegotiationRoundsDA.Success)
            {
                foreach (DataRow m_drMFPTDA in m_dicDFPTNegotiationRoundsDA[0].Tables[0].Rows)
                {
                    FPTNegotiationRoundVM m_objFPTNegotiationRoundVM = new FPTNegotiationRoundVM();
                    m_objFPTNegotiationRoundVM.StartDateTimeStamp = !string.IsNullOrEmpty(m_drMFPTDA[FPTNegotiationRoundVM.Prop.StartDateTimeStamp.Name].ToString()) ? DateTime.Parse(m_drMFPTDA[FPTNegotiationRoundVM.Prop.StartDateTimeStamp.Name].ToString()) : (DateTime?)null;
                    m_objFPTNegotiationRoundVM.EndDateTimeStamp = !string.IsNullOrEmpty(m_drMFPTDA[FPTNegotiationRoundVM.Prop.EndDateTimeStamp.Name].ToString()) ? DateTime.Parse(m_drMFPTDA[FPTNegotiationRoundVM.Prop.EndDateTimeStamp.Name].ToString()) : (DateTime?)null;
                    m_objFPTNegotiationRoundVM.RoundID = m_drMFPTDA[FPTNegotiationRoundVM.Prop.RoundID.Name].ToString();
                    m_lstFPTNegotiationRoundVM.Add(m_objFPTNegotiationRoundVM);
                }
            }
            return m_lstFPTNegotiationRoundVM;
        }

        private TreePanel GenerateTreePanel(List<FPTVendorParticipantsVM> ListVendor, List<FPTNegotiationRoundVM> ListNegoRound, List<NegotiationBidStructuresVM> ListNego, int lastLevel, string BPID = "", string RoundID = "")
        {

            TreePanel m_treepanel = new TreePanel
            {
                ID = "treePanel" + BPID + "structure",
                Padding = 10,
                MinHeight = 200,
                RowLines = true,
                ColumnLines = true,
                UseArrows = true,
                RootVisible = false,
                SortableColumns = false,
                FolderSort = false,
                AutoLoad = false
            };

            m_treepanel.Listeners.ViewReady.Handler = $"viewReady('treePanel{BPID}structure','{BPID}','{RoundID}');";
            m_treepanel.Listeners.AfterRender.Handler = $"btn = Ext.getCmp('BtnExpandtreePanel{BPID}structure'); btn.click();";

            //SelectionModel
            RowSelectionModel m_rowSelectionModel = new RowSelectionModel() { Mode = SelectionMode.Multi, AllowDeselect = true };
            m_treepanel.SelectionModel.Add(m_rowSelectionModel);

            //Fields
            ModelField m_objModelField = new ModelField();
            ModelFieldCollection m_objModelFieldCollection = new ModelFieldCollection();

            m_objModelField = new ModelField { Name = NegotiationBidStructuresVM.Prop.NegotiationBidID.Name.ToLower() };
            m_objModelFieldCollection.Add(m_objModelField);
            m_objModelField = new ModelField { Name = NegotiationBidStructuresVM.Prop.Sequence.Name.ToLower() };
            m_objModelFieldCollection.Add(m_objModelField);
            m_objModelField = new ModelField { Name = NegotiationBidStructuresVM.Prop.ItemID.Name.ToLower() };
            m_objModelFieldCollection.Add(m_objModelField);
            m_objModelField = new ModelField { Name = NegotiationBidStructuresVM.Prop.ItemDesc.Name.ToLower() };
            m_objModelFieldCollection.Add(m_objModelField);
            m_objModelField = new ModelField { Name = NegotiationBidStructuresVM.Prop.ItemParentID.Name.ToLower() };
            m_objModelFieldCollection.Add(m_objModelField);
            m_objModelField = new ModelField { Name = NegotiationBidStructuresVM.Prop.BudgetPlanDefaultValue.Name.ToLower() };
            m_objModelFieldCollection.Add(m_objModelField);


            //Columns
            List<ColumnBase> m_ListColumn = new List<ColumnBase>();
            ColumnBase m_objColumn = new Column();
            m_objColumn = new Column { Text = NegotiationBidStructuresVM.Prop.Sequence.Desc, DataIndex = NegotiationBidStructuresVM.Prop.Sequence.Name.ToLower(), Flex = 1, Hidden = true };
            m_ListColumn.Add(m_objColumn);
            m_objColumn = new TreeColumn { Text = NegotiationBidStructuresVM.Prop.ItemDesc.Desc, DataIndex = NegotiationBidStructuresVM.Prop.ItemDesc.Name.ToLower(), Width = 400 };
            m_ListColumn.Add(m_objColumn);
            m_objColumn = new NumberColumn { Text = NegotiationBidStructuresVM.Prop.BudgetPlanDefaultValue.Desc, DataIndex = NegotiationBidStructuresVM.Prop.BudgetPlanDefaultValue.Name.ToLower(), Align = ColumnAlign.End, Format = Global.IntegerNumberFormat, Width = 100, Locked = true };
            m_ListColumn.Add(m_objColumn);



            //Root
            Node m_node = new Node() { NodeID = "root", Expanded = true };
            NodeCollection m_ncStructure = LoadNodeStructure(ListNego.OrderBy(x => x.Sequence).ToList(), "0", lastLevel, 0, 0);
            m_node.Children.AddRange(m_ncStructure);
            m_node.Children.Add(new Node() { Leaf = true, IconCls = "display: none !important;", Expandable = false });

            var m_objFooterInfo = ListNego.Where(x => string.IsNullOrEmpty(x.ItemID)).OrderBy(d => d.Sequence).ToList();

            decimal m_decRAB = 0;
            decimal m_decFee = 0;

            foreach (var data in m_objFooterInfo)
            {
                //var m_objGrandTotal = ListNego.Where(x => x.ItemDesc == "Grand Total").FirstOrDefault();
                Node m_nodeFooterInfo = new Node();
                m_nodeFooterInfo.NodeID = data.NegotiationBidID;
                m_nodeFooterInfo.Expanded = false;
                m_nodeFooterInfo.Leaf = true;
                m_nodeFooterInfo.AttributesObject = new
                {
                    negotiationbidid = data.NegotiationBidID,
                    itemid = data.ItemID,
                    itemdesc = data.ItemDesc,
                    itemparentid = data.ItemParentID,
                    sequencelevel = data.Sequence,
                    budgetplandefaultvalue = data.BudgetPlanDefaultValue

                };
                if (data.Sequence == (int)VendorBidTypes.Fee)
                {
                    m_nodeFooterInfo.IconCls = "display: none !important;";
                    m_decFee = data.BudgetPlanDefaultValue;
                }
                else
                {
                    m_nodeFooterInfo.Icon = Icon.Sum;
                }
                m_node.Children.Add(m_nodeFooterInfo);
                m_decRAB = data.BudgetPlanDefaultValue;
            }

            m_node.Children.Add(new Node { IconCls = "display: none !important;", Text = "", Leaf = true, Expanded = false });


            for (int i = 1; i <= ListNegoRound.Count; i++)
            {
                m_node.Children.Add(new Node
                {
                    NodeID = m_objFooterInfo.FirstOrDefault(d => d.Sequence == (int)VendorBidTypes.AfterFee).NegotiationBidID + ListNegoRound[i - 1].RoundID,
                    Icon = Icon.Sum,
                    Leaf = true,
                    Expanded = false,
                    AttributesObject = new
                    {
                        itemid = "",
                        itemdesc = $"Grand Total - Round {i}",
                        itemparentid = "",
                        sequencelevel = (int)VendorBidTypes.AfterFee,
                        roundid = ListNegoRound[i - 1].RoundID,
                        budgetplandefaultvalue = m_decRAB

                    }
                });
                m_node.Children.Add(new Node
                {
                    NodeID = m_objFooterInfo.FirstOrDefault(d => d.Sequence == (int)VendorBidTypes.Fee).NegotiationBidID + ListNegoRound[i - 1].RoundID,
                    IconCls = "display: none !important;",
                    Leaf = true,
                    Expanded = false,
                    AttributesObject = new
                    {
                        itemid = "",
                        itemdesc = "Fee",
                        itemparentid = "",
                        sequencelevel = (int)VendorBidTypes.Fee,
                        roundid = ListNegoRound[i - 1].RoundID,
                        budgetplandefaultvalue = m_decFee

                    }
                });
                m_node.Children.Add(new Node { IconCls = "display: none !important;", Text = "", Leaf = true, Expanded = false });
            }


            m_treepanel.Root.Add(m_node);


            foreach (var item in ListVendor.Where(x => x.BudgetPlanID == BPID))
            {
                m_objModelField = new ModelField { Name = (item.VendorID).ToLower() };
                m_objModelFieldCollection.Add(m_objModelField);

                Column m_objColumnBid = new Column { Text = "Bid", DataIndex = item.VendorID, Align = ColumnAlign.End, Renderer = new Renderer("renderGridColumn") };
                Column m_objColumnOffer = new Column { Text = "Last Offer", DataIndex = "offer" + item.VendorID, Align = ColumnAlign.End, Renderer = new Renderer("return Ext.util.Format.number(record.get('offer" + item.VendorID + "'), integerNumberFormat)") };

                m_objColumn = new Column { Text = item.VendorName, Flex = 1 };
                m_objColumn.Columns.Add(m_objColumnOffer);
                m_objColumn.Columns.Add(m_objColumnBid);
                m_ListColumn.Add(m_objColumn);
            }


            //m_treepanel.Fields.AddRange(m_objModelFieldCollection);
            m_treepanel.ColumnModel.Columns.AddRange(m_ListColumn);

            return m_treepanel;
        }

        private NodeCollection LoadNodeStructure(List<NegotiationBidStructuresVM> ListNego, string parentID, int lastLevel, int parentVersion, int parentSequence, string SequenceDesc = "")
        {
            NodeCollection m_nodeColChild = new NodeCollection();

            int m_intSequence = 1;

            foreach (var item in ListNego.Where(x => x.ItemParentID == parentID && x.ParentSequence == parentSequence && x.ParentVersion == parentVersion))
            {

                string m_strSequenceDesc = SequenceDesc.Length > 0 ? SequenceDesc + "." + m_intSequence.ToString() : m_intSequence.ToString();
                int m_intseqLevel = m_strSequenceDesc.Count(f => f == '.') + 1;
                if (m_intseqLevel <= lastLevel)
                {
                    Node m_node = new Node();
                    m_node.NodeID = item.NegotiationBidID;
                    m_node.Children.AddRange(LoadNodeStructure(ListNego, item.ItemID, lastLevel, (int)item.Version, (int)item.Sequence, m_strSequenceDesc));
                    m_node.Icon = Icon.Folder;
                    m_node.Expanded = m_node.Children.Count > 0 ? true : false;
                    m_node.Leaf = m_node.Children.Count > 0 ? false : true;
                    m_node.AttributesObject = new
                    {
                        negotiationbidid = item.NegotiationBidID,
                        sequence = m_strSequenceDesc,
                        itemid = item.ItemID,
                        itemdesc = item.ItemDesc,
                        itemparentid = item.ItemParentID,
                        sequencelevel = m_strSequenceDesc.Count(f => f == '.') + 1,
                        budgetplandefaultvalue = item.BudgetPlanDefaultValue
                    };
                    m_nodeColChild.Add(m_node);
                    m_intSequence++;
                }
            }
            return m_nodeColChild;
        }

        private List<NegotiationConfigurationsVM> GetNegoConfig(string FPTID, string NegotiationConfigTypeID = "")
        {
            List<NegotiationConfigurationsVM> m_lstNegotiationConfigurationsVM = new List<NegotiationConfigurationsVM>();

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();
            CNegotiationConfigurationsDA m_objCNegotiationConfigurationsDA = new CNegotiationConfigurationsDA();
            m_objCNegotiationConfigurationsDA.ConnectionStringName = Global.ConnStrConfigName;

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(FPTID);
            m_objFilter.Add(NegotiationConfigurationsVM.Prop.FPTID.Map, m_lstFilter);

            if (NegotiationConfigTypeID != string.Empty)
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(NegotiationConfigTypeID);
                m_objFilter.Add(NegotiationConfigurationsVM.Prop.NegotiationConfigTypeID.Map, m_lstFilter);
            }

            m_lstSelect = new List<string>();
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.NegotiationConfigID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.NegotiationConfigTypeID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.FPTID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.TaskID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.ParameterValue.MapAlias);

            Dictionary<int, DataSet> m_dicCNegotiationConfigurationsDA = m_objCNegotiationConfigurationsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objCNegotiationConfigurationsDA.Success)
            {
                foreach (DataRow m_drCNegotiationConfigurationsDA in m_dicCNegotiationConfigurationsDA[0].Tables[0].Rows)
                {
                    NegotiationConfigurationsVM m_objNegotiationConfigurationsVM = new NegotiationConfigurationsVM();

                    m_objNegotiationConfigurationsVM.NegotiationConfigID = m_drCNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.NegotiationConfigID.Name].ToString();
                    m_objNegotiationConfigurationsVM.NegotiationConfigTypeID = m_drCNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.NegotiationConfigTypeID.Name].ToString();
                    m_objNegotiationConfigurationsVM.FPTID = m_drCNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.FPTID.Name].ToString();
                    m_objNegotiationConfigurationsVM.TaskID = m_drCNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.TaskID.Name].ToString();
                    m_objNegotiationConfigurationsVM.ParameterValue = m_drCNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.ParameterValue.Name].ToString();
                    m_lstNegotiationConfigurationsVM.Add(m_objNegotiationConfigurationsVM);
                }
            }
            return m_lstNegotiationConfigurationsVM;
        }

        private List<FPTVendorParticipantsVM> GetVendorParticipant(string FPTID)
        {
            List<FPTVendorParticipantsVM> m_lstFPTVendorParticipantsVM = new List<FPTVendorParticipantsVM>();

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();
            DFPTVendorParticipantsDA m_objDFPTVendorParticipantsDA = new DFPTVendorParticipantsDA();
            m_objDFPTVendorParticipantsDA.ConnectionStringName = Global.ConnStrConfigName;

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(FPTID);
            m_objFilter.Add(FPTVendorParticipantsVM.Prop.FPTID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(1);
            m_objFilter.Add(FPTVendorParticipantsVM.Prop.StatusID.Map, m_lstFilter);


            m_lstSelect = new List<string>();
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.VendorName.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.FPTVendorParticipantID.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.NegotiationConfigID.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(NegotiationBidEntryVM.Prop.BidValue.MapAlias);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(NegotiationBidEntryVM.Prop.BidValue.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicDFPTVendorParticipantsDA = m_objDFPTVendorParticipantsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);

            if (m_objDFPTVendorParticipantsDA.Success)
            {
                foreach (DataRow m_drDFPTVendorParticipantsDA in m_dicDFPTVendorParticipantsDA[0].Tables[0].Rows)
                {
                    FPTVendorParticipantsVM m_objNegotiationConfigurationsVM = new FPTVendorParticipantsVM();
                    m_objNegotiationConfigurationsVM.NegotiationConfigID = m_drDFPTVendorParticipantsDA[FPTVendorParticipantsVM.Prop.NegotiationConfigID.Name].ToString();
                    m_objNegotiationConfigurationsVM.FPTVendorParticipantID = m_drDFPTVendorParticipantsDA[FPTVendorParticipantsVM.Prop.FPTVendorParticipantID.Name].ToString();
                    m_objNegotiationConfigurationsVM.VendorID = m_drDFPTVendorParticipantsDA[FPTVendorParticipantsVM.Prop.VendorID.Name].ToString();
                    m_objNegotiationConfigurationsVM.VendorName = m_drDFPTVendorParticipantsDA[FPTVendorParticipantsVM.Prop.VendorName.Name].ToString();
                    m_objNegotiationConfigurationsVM.BudgetPlanID = m_drDFPTVendorParticipantsDA[FPTVendorParticipantsVM.Prop.BudgetPlanID.Name].ToString();
                    m_lstFPTVendorParticipantsVM.Add(m_objNegotiationConfigurationsVM);
                }
            }
            return m_lstFPTVendorParticipantsVM;
        }

        private List<ConfigVM> GetValuePriceTolerance()
        {
            List<ConfigVM> m_lstConfigVM = new List<ConfigVM>();

            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilteru = new Dictionary<string, List<object>>();

            List<string> m_lstSelectb = new List<string>();
            m_lstSelectb.Add(ConfigVM.Prop.Desc1.MapAlias);
            m_lstSelectb.Add(ConfigVM.Prop.Key2.MapAlias);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("ComparisonPriceMargin");
            m_objFilteru.Add(ConfigVM.Prop.Key1.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicUConfigDA = m_objUConfigDA.SelectBC(0, null, false, m_lstSelectb, m_objFilteru, null, null, null);

            if (m_objUConfigDA.Success)
            {
                foreach (DataRow item in m_dicUConfigDA[0].Tables[0].Rows)
                {
                    ConfigVM m_objConfigVM = new ConfigVM();
                    m_objConfigVM.Desc1 = item[ConfigVM.Prop.Desc1.Name].ToString();
                    m_objConfigVM.Key2 = item[ConfigVM.Prop.Key2.Name].ToString();
                    m_lstConfigVM.Add(m_objConfigVM);
                }
            }
            return m_lstConfigVM;
        }

        private List<FPTAdditionalInfoVM> GetListFPTAdditionalInfoVM(string FPTID, ref string message)
        {
            List<FPTAdditionalInfoVM> m_lstFPTAdditionalInfoVM = new List<FPTAdditionalInfoVM>();

            DFPTAdditionalInfoDA m_objDFPTAdditionalInfoDA = new DFPTAdditionalInfoDA();
            m_objDFPTAdditionalInfoDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(FPTAdditionalInfoVM.Prop.FPTAdditionalInfoID.MapAlias);
            m_lstSelect.Add(FPTAdditionalInfoVM.Prop.FPTID.MapAlias);
            m_lstSelect.Add(FPTAdditionalInfoVM.Prop.FPTAdditionalInfoItemID.MapAlias);
            m_lstSelect.Add(FPTAdditionalInfoVM.Prop.Value.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(FPTID);
            m_objFilter.Add(FPTAdditionalInfoVM.Prop.FPTID.Map, m_lstFilter);


            Dictionary<int, DataSet> m_dicDFPTAdditionalInfoDA = m_objDFPTAdditionalInfoDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDFPTAdditionalInfoDA.Success)
            {
                foreach (DataRow m_drDFPTAdditionalInfoDA in m_dicDFPTAdditionalInfoDA[0].Tables[0].Rows)
                {
                    FPTAdditionalInfoVM m_objFPTAdditionalInfoVM = new FPTAdditionalInfoVM();
                    m_objFPTAdditionalInfoVM.FPTAdditionalInfoID = m_drDFPTAdditionalInfoDA[FPTAdditionalInfoVM.Prop.FPTAdditionalInfoID.Name].ToString();
                    m_objFPTAdditionalInfoVM.FPTID = m_drDFPTAdditionalInfoDA[FPTAdditionalInfoVM.Prop.FPTID.Name].ToString();
                    m_objFPTAdditionalInfoVM.FPTAdditionalInfoItemID = m_drDFPTAdditionalInfoDA[FPTAdditionalInfoVM.Prop.FPTAdditionalInfoItemID.Name].ToString();
                    m_objFPTAdditionalInfoVM.Value = m_drDFPTAdditionalInfoDA[FPTAdditionalInfoVM.Prop.Value.Name].ToString();
                    m_lstFPTAdditionalInfoVM.Add(m_objFPTAdditionalInfoVM);
                }
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDFPTAdditionalInfoDA.Message;

            return m_lstFPTAdditionalInfoVM;
        }

        private bool IsByPassToAccess()
        {
            string m_strResult = GetMenuObject("IsByPass");
            return Convert.ToBoolean(string.IsNullOrEmpty(m_strResult) ?"false": m_strResult);
        }

    }
}