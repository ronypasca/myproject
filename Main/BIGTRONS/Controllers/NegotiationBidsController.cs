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
using com.SML.BIGTRONS.Hubs;

namespace com.SML.BIGTRONS.Controllers
{
    public class NegotiationBidsController : BaseController
    {
        private readonly string title = "Live Negotiation";
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
            if (!Global.HasAccess.Update)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            if (Session[dataSessionName] != null)
                Session[dataSessionName] = null;

            //decimal m_decpricemargin = 0;
            //m_decpricemargin = GetValuePriceTolerance();
            ViewDataDictionary m_vddFPT = new ViewDataDictionary();
            //m_vddFPT.Add("PriceMarginData", m_decpricemargin);
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 0;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.HomePage),
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddFPT,
                ViewName = "_List",
                WrapByScriptTag = false
            };
        }
        public ActionResult Home()
        {
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 0;
            return this.Direct();
        }

        public ActionResult GetPanel(string FPTID, string RoundID, /*int lastLevel,*/ decimal BValue, decimal TValue)
        {
            List<FPTNegotiationRoundVM> m_lstFPTNegotiationgetRoundVM = getNegoRound(RoundID);
            if (!m_lstFPTNegotiationgetRoundVM.Any()) return this.Direct(false);
            List<NegotiationConfigurationsVM> m_lstCNegotiationConfigurations = getNegoConfig(FPTID, General.EnumDesc(NegoConfigTypes.SubItemLevel));//todo: simplified
            UserVM m_UserVM = getCurentUser();
            int m_intSubItemLevelData = Int16.Parse(m_lstCNegotiationConfigurations.FirstOrDefault().ParameterValue);
            DateTime m_objschedule = Convert.ToDateTime(getNegoConfig(FPTID, General.EnumDesc(NegoConfigTypes.Schedule)).FirstOrDefault().ParameterValue);
            m_lstFPTNegotiationgetRoundVM.ForEach(i => i.Schedule = m_objschedule);

            List<FPTAdditionalInfoVM> m_ListFPTAdditionalInfoVM = GetListFPTAdditionalInfoVM(FPTID);
            DateTime m_dtstart = DateTime.MinValue;
            if (m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.ScheduleStart).ToString()).Any())
            {
                DateTime.TryParse(m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.ScheduleStart).ToString()).FirstOrDefault().Value, out m_dtstart);
            }
            DateTime m_dtend = DateTime.MinValue;
            if (m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.ScheduleEnd).ToString()).Any())
            {
                DateTime.TryParse(m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.ScheduleEnd).ToString()).FirstOrDefault().Value, out m_dtend);
            }

            bool m_isManual = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.ScheduleStartManual).ToString()).Any() || m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.ScheduleEndManual).ToString()).Any() || m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.DurationManual).ToString()).Any();

            string m_FPTScheduleStart = m_dtstart.ToString(Global.DefaultDateFormat);
            string m_FPTScheduleEnd = m_dtend.ToString(Global.DefaultDateFormat);
            string m_Duration = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.Duration).ToString()).Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.Duration).ToString()).FirstOrDefault().Value : string.Empty;

            if (m_isManual)
            {
                m_FPTScheduleStart = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.ScheduleStartManual).ToString()).Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.ScheduleStartManual).ToString()).FirstOrDefault().Value : string.Empty;
                m_FPTScheduleEnd = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.ScheduleEndManual).ToString()).Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.ScheduleEndManual).ToString()).FirstOrDefault().Value : string.Empty;
                m_Duration = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.DurationManual).ToString()).Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.DurationManual).ToString()).FirstOrDefault().Value : string.Empty;
            }

            m_lstFPTNegotiationgetRoundVM.ForEach(i => i.FPTScheduleEnd = m_dtstart);
            m_lstFPTNegotiationgetRoundVM.ForEach(i => i.FPTScheduleEnd = m_dtend);
            m_lstFPTNegotiationgetRoundVM.ForEach(i => i.FPTDuration = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.Duration).ToString()).Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.Duration).ToString()).FirstOrDefault().Value : null);
            m_lstFPTNegotiationgetRoundVM.ForEach(i => i.MaintenancePeriod = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.MaintenancePeriod).ToString()).Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.MaintenancePeriod).ToString()).FirstOrDefault().Value : null);
            m_lstFPTNegotiationgetRoundVM.ForEach(i => i.Guarantee = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.Guarantee).ToString()).Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.Guarantee).ToString()).FirstOrDefault().Value : null);
            m_lstFPTNegotiationgetRoundVM.ForEach(i => i.ContractType = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.ContractType).ToString()).Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.ContractType).ToString()).FirstOrDefault().Value : null);
            m_lstFPTNegotiationgetRoundVM.ForEach(i => i.PaymentMethod = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.PaymentMethod).ToString()).Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.PaymentMethod).ToString()).FirstOrDefault().Value : null);


            //TYPE PEKERJAAN
            string m_typeftp = "";
            m_typeftp = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "14").Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "14").FirstOrDefault().Value : (m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "15").Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "15").FirstOrDefault().Value : "");

            //decimal m_decpricemargin = 0;
            //m_decpricemargin = GetValuePriceTolerance();

            var sToolbarPaddingSpec = "20 0 30 0";
            var sPanelLeftPaddingSpec = "10 0 10 10";
            var sPanelRightPaddingSpec = "10 10 10 10";
            var iLabelWidth = 175;
            var iFieldWidth = 420;

            //Get All Structure
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();
            string m_strVendorID = getCurentUser().VendorID;

            List<NegotiationBidEntryVM> m_lstDNegotiationBidEntry = getNegoEntry(FPTID, m_strVendorID);
            List<NegotiationBidStructuresVM> m_lstNegotiationBidStructuresVM = getNegoStructure(FPTID, m_strVendorID);
            FPTNegotiationRoundVM m_FPTNegotiationRoundVM = m_lstFPTNegotiationgetRoundVM.FirstOrDefault();
            List<FPTNegotiationRoundVM> m_lstFPTNegotiationRoundVM = getlstNegoRound(FPTID);
            string m_strlastbidround = null;
            foreach (var item in m_lstFPTNegotiationRoundVM)
            {
                if (m_lstDNegotiationBidEntry.Where(x => x.RoundID == item.RoundID).Any())
                {
                    m_strlastbidround = item.RoundID;
                    break;
                }
            }


            foreach (var item in m_lstNegotiationBidStructuresVM)
            {
                decimal m_decbid = 0;
                decimal m_lastbid = 0;
                decimal m_lastoffer = 0;
                if (m_lstDNegotiationBidEntry.Where(x => x.RoundID == RoundID && x.NegotiationBidID == item.NegotiationBidID).Any())
                {
                    m_decbid = m_lstDNegotiationBidEntry.Where(x => x.RoundID == RoundID && x.NegotiationBidID == item.NegotiationBidID).FirstOrDefault().BidValue;
                }
                if (m_decbid != 0)
                {
                    item.Bid = m_decbid;
                }

                if (m_lstDNegotiationBidEntry.Where(x => x.NegotiationBidID == item.NegotiationBidID && string.IsNullOrEmpty(x.RoundID)).Any())
                {
                    m_lastoffer = m_lstDNegotiationBidEntry.Where(x => x.NegotiationBidID == item.NegotiationBidID && string.IsNullOrEmpty(x.RoundID)).FirstOrDefault().BidValue;
                }
                if (m_lastoffer != 0)
                {
                    item.LastOffer = m_lastoffer;
                }

                if (string.IsNullOrEmpty(m_strlastbidround))
                {
                    if (m_lstDNegotiationBidEntry.Where(x => string.IsNullOrEmpty(x.RoundID) && x.NegotiationBidID == item.NegotiationBidID).Any())
                    {
                        m_lastbid = m_lstDNegotiationBidEntry.Where(x => string.IsNullOrEmpty(x.RoundID) && x.NegotiationBidID == item.NegotiationBidID).FirstOrDefault().BidValue;
                    }
                }
                else
                {
                    if (m_lstDNegotiationBidEntry.Where(x => x.RoundID == m_strlastbidround && x.NegotiationBidID == item.NegotiationBidID).Any())
                    {
                        m_lastbid = m_lstDNegotiationBidEntry.Where(x => x.RoundID == m_strlastbidround && x.NegotiationBidID == item.NegotiationBidID).FirstOrDefault().BidValue;
                    }
                }

                if (m_lastbid != 0)
                {
                    item.LastBid = m_lastbid;
                }

            }

            //Get Project List
            List<string> ProjectList = m_lstNegotiationBidStructuresVM.Select(x => x.ProjectID).Distinct().ToList();


            if (ProjectList.Any() && ProjectList.FirstOrDefault() != string.Empty)
            {
                //Project Tab Panel
                TabPanel ProjectTabPanel = new TabPanel
                {
                    ID = "TPProject" + FPTID + RoundID,
                    ActiveTabIndex = 0,
                    Border = false
                };
                //List Panel Project
                foreach (var item in ProjectList)
                {
                    //Project Panel
                    Panel ProjectPanel = new Panel
                    {
                        ID = "P" + item + FPTID + RoundID,
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
                        string VendorParticipantID = getParticipanConfig(FPTID, m_strVendorID).Where(x => x.ParameterValue == BPitem).FirstOrDefault().FPTVendorParticipantID;
                        //BP Panel
                        Panel BPPanel = new Panel
                        {
                            ID = "P" + BPitem + "Form" + FPTID + RoundID,
                            Frame = true,
                            Title = m_lstNegotiationBidStructuresVM.Where(x => x.ParameterValue == BPitem).FirstOrDefault().BPVersionName,
                            Border = false
                        };
                        Toolbar m_BPPanelToolbar = new Toolbar();
                        TreePanel m_TPBP = generateTreePanel(m_lstNegotiationBidStructuresVM.Where(x => x.ParameterValue == BPitem).ToList(), m_intSubItemLevelData, BValue, TValue, BPitem);

                        List<Parameter> m_lstParameter = new List<Parameter>();
                        Parameter m_param;
                        m_param = new Parameter("treeStructure", "getListStructure('" + m_TPBP.ID + "')", ParameterMode.Raw, true);
                        m_lstParameter.Add(m_param);
                        m_param = new Parameter("roundID", RoundID, ParameterMode.Value, false);
                        m_lstParameter.Add(m_param);
                        m_param = new Parameter("budgetPlanID", BPitem, ParameterMode.Value, false);
                        m_lstParameter.Add(m_param);
                        m_param = new Parameter("vendorParticipantID", VendorParticipantID, ParameterMode.Value, false);
                        m_lstParameter.Add(m_param);
                        m_param = new Parameter("VendorID", m_strVendorID, ParameterMode.Value, false);
                        m_lstParameter.Add(m_param);
                        m_param = new Parameter("treeID", m_TPBP.ID, ParameterMode.Value, false);
                        m_lstParameter.Add(m_param);


                        Button m_btnBPSubmit = new Button() { ID = "BtnSubmit" + m_TPBP.ID + FPTID + RoundID, Text = "Submit" };
                        m_btnBPSubmit.DirectEvents.Click.Action = Url.Action("Submit");
                        m_btnBPSubmit.DirectEvents.Click.EventMask.ShowMask = true;
                        m_btnBPSubmit.DirectEvents.Click.ExtraParams.AddRange(m_lstParameter);
                        m_btnBPSubmit.DirectEvents.Click.After = "onSubmit";
                        m_BPPanelToolbar.Items.Add(m_btnBPSubmit);
                        BPPanel.TopBar.Add(m_BPPanelToolbar);
                        Button m_btnBPExCol = new Button() { ID = "BtnExpand" + m_TPBP.ID + FPTID + RoundID, Text = "Collapse" };
                        m_btnBPExCol.Handler = "ExColHandler";
                        m_btnBPExCol.TagString = m_TPBP.ID;
                        m_BPPanelToolbar.Items.Add(m_btnBPExCol);
                        BPPanel.Items.Add(m_TPBP);
                        BPTabPanel.Items.Add(BPPanel);
                    }

                    ProjectPanel.Items.Add(BPTabPanel);
                    ProjectTabPanel.Items.Add(ProjectPanel);
                }
                //Project Panel
                FormPanel FPTPanel = new FormPanel
                {
                    ID = "P" + "Form" + FPTID + RoundID,
                    Frame = true,
                    Border = false,
                    Padding = 10
                };
                Container FPTContainer = new Container()
                {
                    ID = "Cont" + FPTID + RoundID,
                    Layout = "Column"
                };

                //Panel Left
                Panel m_Layoutpanelleft = new Panel()
                {
                    Cls = "fullOnSmall",
                    ColumnWidth = 0.325,
                    ID = "pnlLeft" + FPTID + RoundID,
                    PaddingSpec = sPanelLeftPaddingSpec
                };
                Label LabelRemaining = new Label
                {
                    ID = "LblRemaining" + FPTID + RoundID,
                    Text = "Time Remaining 00:00:00",
                    Cls = "lblremaining",
                    PaddingSpec = sToolbarPaddingSpec
                };
                m_Layoutpanelleft.Items.Add(LabelRemaining);
                TextField TFFPTID = new TextField
                {
                    ID = "TFFPTID" + FPTID + RoundID,
                    FieldLabel = FPTNegotiationRoundVM.Prop.FPTID.Desc,
                    ReadOnly = true,
                    LabelWidth = iLabelWidth,
                    Width = iFieldWidth,
                    Hidden = true,
                    Value = FPTID
                };
                m_Layoutpanelleft.Items.Add(TFFPTID);
                TextField TFVendorName = new TextField
                {
                    ID = "TFVendorName" + FPTID + RoundID,
                    FieldLabel = "Vendor Name",
                    ReadOnly = true,
                    LabelWidth = iLabelWidth,
                    Width = iFieldWidth,
                    Value = m_UserVM.VendorDesc
                };
                m_Layoutpanelleft.Items.Add(TFVendorName);
                TextField TFSchedule = new TextField
                {
                    ID = "TFSchedule" + FPTID + RoundID,
                    FieldLabel = "Schedule",
                    ReadOnly = true,
                    LabelWidth = iLabelWidth,
                    Width = iFieldWidth,
                    Value = m_FPTNegotiationRoundVM.Schedule
                };
                m_Layoutpanelleft.Items.Add(TFSchedule);
                TextField TFFPTName = new TextField
                {
                    ID = "TFFPTName" + FPTID + RoundID,
                    FieldLabel = "FPT",
                    ReadOnly = true,
                    LabelWidth = iLabelWidth,
                    Width = iFieldWidth,
                    Value = FPTID
                };
                m_Layoutpanelleft.Items.Add(TFFPTName);
                TextField TFRoundNo = new TextField
                {
                    ID = "TFRoundNo" + FPTID + RoundID,
                    FieldLabel = "Round No",
                    ReadOnly = true,
                    LabelWidth = iLabelWidth,
                    Width = iFieldWidth,
                    Value = m_FPTNegotiationRoundVM.RoundNo
                };
                m_Layoutpanelleft.Items.Add(TFRoundNo);
                TextField TFTotalRound = new TextField
                {
                    ID = "TFTotalRound" + FPTID + RoundID,
                    FieldLabel = "Total Round",
                    ReadOnly = true,
                    LabelWidth = iLabelWidth,
                    Width = iFieldWidth,
                    Value = m_FPTNegotiationRoundVM.TotalRound
                };
                m_Layoutpanelleft.Items.Add(TFTotalRound);
                Label LabelAddInfo = new Label
                {
                    ID = "LabelAddInfo" + FPTID + RoundID,
                    Text = "Additional Info",
                    PaddingSpec = sToolbarPaddingSpec
                };
                m_Layoutpanelleft.Items.Add(LabelAddInfo);
                TextField DFFPTScheduleStart = new TextField
                {
                    ID = "DFFPTScheduleStart" + FPTID + RoundID,
                    FieldLabel = "Schedule Start",
                    ReadOnly = true,
                    LabelWidth = iLabelWidth,
                    Width = iFieldWidth,
                    Value = m_FPTScheduleStart/*m_FPTNegotiationRoundVM.FPTScheduleStart*/
                };
                m_Layoutpanelleft.Items.Add(DFFPTScheduleStart);
                TextField DFFPTScheduleEnd = new TextField
                {
                    ID = "DFFPTScheduleEnd" + FPTID + RoundID,
                    FieldLabel = "Schedule End",
                    ReadOnly = true,
                    LabelWidth = iLabelWidth,
                    Width = iFieldWidth,
                    Value = m_FPTScheduleEnd /* m_FPTNegotiationRoundVM.FPTScheduleEnd */
                };
                m_Layoutpanelleft.Items.Add(DFFPTScheduleEnd);
                TextField TFFPTDuration = new TextField
                {
                    ID = "TFFPTDuration" + FPTID + RoundID,
                    FieldLabel = "Duration",
                    ReadOnly = true,
                    LabelWidth = iLabelWidth,
                    Width = iFieldWidth,
                    Value = m_Duration /*m_FPTNegotiationRoundVM.FPTDuration*/
                };
                m_Layoutpanelleft.Items.Add(TFFPTDuration);
                TextField TFMaintenancePeriod = new TextField
                {
                    ID = "TFMaintenancePeriod" + FPTID + RoundID,
                    FieldLabel = "Maintenance Period",
                    ReadOnly = true,
                    LabelWidth = iLabelWidth,
                    Width = iFieldWidth,
                    Value = m_FPTNegotiationRoundVM.MaintenancePeriod
                };
                m_Layoutpanelleft.Items.Add(TFMaintenancePeriod);
                TextField TFGuarantee = new TextField
                {
                    ID = "TFGuarantee" + FPTID + RoundID,
                    FieldLabel = "Guarantee",
                    ReadOnly = true,
                    LabelWidth = iLabelWidth,
                    Width = iFieldWidth,
                    Value = m_FPTNegotiationRoundVM.Guarantee
                };
                m_Layoutpanelleft.Items.Add(TFGuarantee);
                TextField TFContractType = new TextField
                {
                    ID = "TFContractType" + FPTID + RoundID,
                    FieldLabel = "Contract Type",
                    ReadOnly = true,
                    LabelWidth = iLabelWidth,
                    Width = iFieldWidth,
                    Value = m_FPTNegotiationRoundVM.ContractType
                };
                m_Layoutpanelleft.Items.Add(TFContractType);
                TextArea TAPaymentMethod = new TextArea
                {
                    ID = "TAPaymentMethod" + FPTID + RoundID,
                    FieldLabel = "Payment Method",
                    ReadOnly = true,
                    LabelWidth = iLabelWidth,
                    Width = iFieldWidth,
                    Value = m_FPTNegotiationRoundVM.PaymentMethod
                };
                m_Layoutpanelleft.Items.Add(TAPaymentMethod);
                Label LabelInfo = new Label
                {
                    Text = String.Format("*{0}", m_typeftp)
                };
                m_Layoutpanelleft.Items.Add(LabelInfo);
                Label LabelInfo2 = new Label
                {
                    Text = "*Tidak ada eskalasi sampai dengan proyek selesai"
                };
                m_Layoutpanelleft.Items.Add(LabelInfo2);
                Label LabelInfo3 = new Label
                {
                    Text = "*Harga Negosiasi telah disetujui oleh vendor melalui sistem dan tidak memerlukan tanda tangan untuk keabsahannya"
                };
                m_Layoutpanelleft.Items.Add(LabelInfo3);

                //Panel Right
                Panel m_Layoutpanelright = new Panel()
                {
                    Cls = "fullOnSmall",
                    ColumnWidth = 0.675,
                    ID = "pnlRight" + FPTID + RoundID,
                    PaddingSpec = sPanelRightPaddingSpec
                };

                Label LabelLegend = new Label
                {
                    ID = "LabelLegend" + FPTID + RoundID,
                    Html = "<span class='legendbox' style='background-color:red;'>&nbsp;</span><i> Higher</i>",
                    Cls = "lblhigherinfo",
                };
                m_Layoutpanelright.Items.Add(LabelLegend);
                FormPanel FPFPBid = new FormPanel
                {
                    ID = "FPBid" + FPTID + RoundID
                };
                FPFPBid.Items.Add(ProjectTabPanel);
                m_Layoutpanelright.Items.Add(FPFPBid);

                FPTContainer.Items.Add(m_Layoutpanelleft);
                FPTContainer.Items.Add(m_Layoutpanelright);

                FPTPanel.Items.Add(FPTContainer);
                FPTPanel.Listeners.AfterRender.Fn = "resizeContainer";


                return this.ComponentConfig(FPTPanel);

            }
            else
            {
                //insert project & BP upload
                var m_lstconfig = getNegoConfig(FPTID);
                foreach (var item in m_lstNegotiationBidStructuresVM)
                {
                    item.ProjectDesc = m_lstconfig.Where(y => y.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.Round)).FirstOrDefault().ParameterValue2;
                    item.BPVersionName = m_lstconfig.Where(y => y.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.RoundTime)).FirstOrDefault().ParameterValue2;
                }
                string VendorParticipantID = getParticipanConfig(FPTID, m_strVendorID).FirstOrDefault().FPTVendorParticipantID;
                Panel BPPanel = new Panel
                {
                    ID = "P" + "Upload" + "Form" + FPTID + RoundID,
                    Frame = true,
                    Title = m_lstNegotiationBidStructuresVM.FirstOrDefault().ProjectDesc + " " + m_lstNegotiationBidStructuresVM.FirstOrDefault().BPVersionName,
                    Border = false
                };
                Toolbar m_BPPanelToolbar = new Toolbar();
                TreePanel m_TPBP = generateTreePanel(m_lstNegotiationBidStructuresVM.ToList(), m_intSubItemLevelData, BValue, TValue);

                List<Parameter> m_lstParameter = new List<Parameter>();
                Parameter m_param;
                m_param = new Parameter("treeStructure", "getListStructure('" + m_TPBP.ID + "')", ParameterMode.Raw, true);
                m_lstParameter.Add(m_param);
                m_param = new Parameter("roundID", RoundID, ParameterMode.Value, false);
                m_lstParameter.Add(m_param);
                m_param = new Parameter("budgetPlanID", m_lstconfig.Where(y => y.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.BudgetPlan)).FirstOrDefault().ParameterValue, ParameterMode.Value, false);
                m_lstParameter.Add(m_param);
                m_param = new Parameter("vendorParticipantID", VendorParticipantID, ParameterMode.Value, false);
                m_lstParameter.Add(m_param);
                m_param = new Parameter("VendorID", m_strVendorID, ParameterMode.Value, false);
                m_lstParameter.Add(m_param);

                Button m_btnBPSubmit = new Button() { ID = "BtnSubmit" + m_TPBP.ID, Text = "Submit" };
                m_btnBPSubmit.DirectEvents.Click.Action = Url.Action("Submit");
                m_btnBPSubmit.DirectEvents.Click.EventMask.ShowMask = true;
                m_btnBPSubmit.DirectEvents.Click.ExtraParams.AddRange(m_lstParameter);
                m_BPPanelToolbar.Items.Add(m_btnBPSubmit);
                BPPanel.TopBar.Add(m_BPPanelToolbar);
                Button m_btnBPExCol = new Button() { ID = "BtnExpand" + m_TPBP.ID, Text = "Collapse" };
                m_btnBPExCol.Handler = "ExColHandler";
                m_btnBPExCol.TagString = m_TPBP.ID;
                m_BPPanelToolbar.Items.Add(m_btnBPExCol);
                BPPanel.Items.Add(m_TPBP);

                Container FPTContainer = new Container()
                {
                    ID = "Cont" + FPTID + RoundID,
                    Layout = "Column"
                };

                //Panel Left
                Panel m_Layoutpanelleft = new Panel()
                {
                    Cls = "fullOnSmall",
                    ColumnWidth = 0.325,
                    ID = "pnlLeft" + FPTID + RoundID,
                    PaddingSpec = sPanelLeftPaddingSpec
                };
                Label LabelRemaining = new Label
                {
                    ID = "LblRemaining" + FPTID + RoundID,
                    Text = "Time Remaining 00:00:00",
                    Cls = "lblremaining",
                    PaddingSpec = sToolbarPaddingSpec
                };
                m_Layoutpanelleft.Items.Add(LabelRemaining);
                TextField TFFPTID = new TextField
                {
                    ID = "TFFPTID" + FPTID + RoundID,
                    FieldLabel = FPTNegotiationRoundVM.Prop.FPTID.Desc,
                    ReadOnly = true,
                    LabelWidth = iLabelWidth,
                    Width = iFieldWidth,
                    Hidden = true,
                    Value = FPTID
                };
                m_Layoutpanelleft.Items.Add(TFFPTID);
                TextField TFVendorName = new TextField
                {
                    ID = "TFVendorName" + FPTID + RoundID,
                    FieldLabel = "Vendor Name",
                    ReadOnly = true,
                    LabelWidth = iLabelWidth,
                    Width = iFieldWidth,
                    Value = m_UserVM.VendorDesc
                };
                m_Layoutpanelleft.Items.Add(TFVendorName);
                TextField TFSchedule = new TextField
                {
                    ID = "TFSchedule" + FPTID + RoundID,
                    FieldLabel = "Schedule",
                    ReadOnly = true,
                    LabelWidth = iLabelWidth,
                    Width = iFieldWidth,
                    Value = m_FPTNegotiationRoundVM.Schedule
                };
                m_Layoutpanelleft.Items.Add(TFSchedule);
                TextField TFFPTName = new TextField
                {
                    ID = "TFFPTName" + FPTID + RoundID,
                    FieldLabel = "FPT",
                    ReadOnly = true,
                    LabelWidth = iLabelWidth,
                    Width = iFieldWidth,
                    Value = FPTID
                };
                m_Layoutpanelleft.Items.Add(TFFPTName);
                TextField TFRoundNo = new TextField
                {
                    ID = "TFRoundNo" + FPTID + RoundID,
                    FieldLabel = "Round No",
                    ReadOnly = true,
                    LabelWidth = iLabelWidth,
                    Width = iFieldWidth,
                    Value = m_FPTNegotiationRoundVM.RoundNo
                };
                m_Layoutpanelleft.Items.Add(TFRoundNo);
                TextField TFTotalRound = new TextField
                {
                    ID = "TFTotalRound" + FPTID + RoundID,
                    FieldLabel = "Total Round",
                    ReadOnly = true,
                    LabelWidth = iLabelWidth,
                    Width = iFieldWidth,
                    Value = m_FPTNegotiationRoundVM.TotalRound
                };
                m_Layoutpanelleft.Items.Add(TFTotalRound);
                Label LabelAddInfo = new Label
                {
                    ID = "LabelAddInfo" + FPTID + RoundID,
                    Text = "Additional Info",
                    PaddingSpec = sToolbarPaddingSpec
                };
                m_Layoutpanelleft.Items.Add(LabelAddInfo);
                TextField DFFPTScheduleStart = new TextField
                {
                    ID = "DFFPTScheduleStart" + FPTID + RoundID,
                    FieldLabel = "Schedule Start",
                    ReadOnly = true,
                    LabelWidth = iLabelWidth,
                    Width = iFieldWidth,
                    Value = m_FPTScheduleStart/* m_FPTNegotiationRoundVM.FPTScheduleStart*/
                };
                m_Layoutpanelleft.Items.Add(DFFPTScheduleStart);
                TextField DFFPTScheduleEnd = new TextField
                {
                    ID = "DFFPTScheduleEnd" + FPTID + RoundID,
                    FieldLabel = "Schedule End",
                    ReadOnly = true,
                    LabelWidth = iLabelWidth,
                    Width = iFieldWidth,
                    Value = m_FPTScheduleEnd /* m_FPTNegotiationRoundVM.FPTScheduleEnd */
                };
                m_Layoutpanelleft.Items.Add(DFFPTScheduleEnd);
                TextField TFFPTDuration = new TextField
                {
                    ID = "TFFPTDuration" + FPTID + RoundID,
                    FieldLabel = "Duration",
                    ReadOnly = true,
                    LabelWidth = iLabelWidth,
                    Width = iFieldWidth,
                    Value = m_Duration /*m_FPTNegotiationRoundVM.FPTDuration*/
                };
                m_Layoutpanelleft.Items.Add(TFFPTDuration);
                TextField TFMaintenancePeriod = new TextField
                {
                    ID = "TFMaintenancePeriod" + FPTID + RoundID,
                    FieldLabel = "Maintenance Period",
                    ReadOnly = true,
                    LabelWidth = iLabelWidth,
                    Width = iFieldWidth,
                    Value = m_FPTNegotiationRoundVM.MaintenancePeriod
                };
                m_Layoutpanelleft.Items.Add(TFMaintenancePeriod);
                TextField TFGuarantee = new TextField
                {
                    ID = "TFGuarantee" + FPTID + RoundID,
                    FieldLabel = "Guarantee",
                    ReadOnly = true,
                    LabelWidth = iLabelWidth,
                    Width = iFieldWidth,
                    Value = m_FPTNegotiationRoundVM.Guarantee
                };
                m_Layoutpanelleft.Items.Add(TFGuarantee);
                TextField TFContractType = new TextField
                {
                    ID = "TFContractType" + FPTID + RoundID,
                    FieldLabel = "Contract Type",
                    ReadOnly = true,
                    LabelWidth = iLabelWidth,
                    Width = iFieldWidth,
                    Value = m_FPTNegotiationRoundVM.ContractType
                };
                m_Layoutpanelleft.Items.Add(TFContractType);
                TextArea TAPaymentMethod = new TextArea
                {
                    ID = "TAPaymentMethod" + FPTID + RoundID,
                    FieldLabel = "Payment Method",
                    ReadOnly = true,
                    LabelWidth = iLabelWidth,
                    Width = iFieldWidth,
                    Value = m_FPTNegotiationRoundVM.PaymentMethod
                };
                m_Layoutpanelleft.Items.Add(TAPaymentMethod);
                Label LabelInfo = new Label
                {
                    Text = String.Format("*{0}", m_typeftp)
                };
                m_Layoutpanelleft.Items.Add(LabelInfo);

                Label LabelInfo2 = new Label
                {
                    Text = "*Tidak ada eskalasi sampai dengan proyek selesai"
                };
                m_Layoutpanelleft.Items.Add(LabelInfo2);
                Label LabelInfo3 = new Label
                {
                    Text = "*Harga Negosiasi telah disetujui oleh vendor melalui sistem dan tidak memerlukan tanda tangan untuk keabsahannya"
                };
                m_Layoutpanelleft.Items.Add(LabelInfo3);
                //Panel Right
                Panel m_Layoutpanelright = new Panel()
                {
                    Cls = "fullOnSmall",
                    ColumnWidth = 0.675,
                    ID = "pnlRight" + FPTID + RoundID,
                    PaddingSpec = sPanelRightPaddingSpec
                };

                Label LabelLegend = new Label
                {
                    ID = "LabelLegend" + FPTID + RoundID,
                    Html = "<span class='legendbox' style='background-color:red;'>&nbsp;</span><i> Higher</i>",
                    Cls = "lblhigherinfo",
                };
                m_Layoutpanelright.Items.Add(LabelLegend);
                FormPanel FPFPBid = new FormPanel
                {
                    ID = "FPBid" + FPTID + RoundID
                };
                FPFPBid.Items.Add(BPPanel);
                m_Layoutpanelright.Items.Add(FPFPBid);

                FPTContainer.Items.Add(m_Layoutpanelleft);
                FPTContainer.Items.Add(m_Layoutpanelright);

                //Project Panel
                Panel FPTPanel = new Panel
                {
                    ID = "P" + "Form" + FPTID + RoundID,
                    Frame = true,
                    Border = false,

                };

                FPTPanel.Items.Add(FPTContainer);
                return this.ComponentConfig(FPTPanel);
                //return this.ComponentConfig(BPPanel);
            }
        }
        public ActionResult Bid(string Caller, string Selected, string FPTID = "", string RoundID = "")
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Update)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<FPTNegotiationRoundVM> m_lstFPTNegotiationRoundVMCheck = getCurentRound();
            if (!m_lstFPTNegotiationRoundVMCheck.Any())
            {
                return this.Direct();
            }

            string m_strFPTID;
            string m_strRoundID;
            if (FPTID != string.Empty && RoundID != string.Empty)
            {
                m_strFPTID = FPTID;
                m_strRoundID = RoundID;
            }
            else
            {
                if (string.IsNullOrEmpty(Selected))
                {
                    return this.Direct();
                }
                Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();
                m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
                m_strFPTID = m_dicSelectedRow[FPTNegotiationRoundVM.Prop.FPTID.Name].ToString();
                m_strRoundID = m_dicSelectedRow[FPTNegotiationRoundVM.Prop.RoundID.Name].ToString();
            }

            List<FPTNegotiationRoundVM> m_lstFPTNegotiationRoundVM = getNegoRound(m_strRoundID);
            List<NegotiationConfigurationsVM> m_lstCNegotiationConfigurations = getNegoConfig(m_strFPTID, General.EnumDesc(NegoConfigTypes.SubItemLevel));
            UserVM m_UserVM = getCurentUser();
            int m_intSubItemLevelData = Int16.Parse(m_lstCNegotiationConfigurations.FirstOrDefault().ParameterValue);
            DateTime m_objschedule = Convert.ToDateTime(getNegoConfig(m_strFPTID, General.EnumDesc(NegoConfigTypes.Schedule)).FirstOrDefault().ParameterValue);
            m_lstFPTNegotiationRoundVM.ForEach(i => i.Schedule = m_objschedule);

            List<FPTAdditionalInfoVM> m_ListFPTAdditionalInfoVM = GetListFPTAdditionalInfoVM(m_strFPTID);
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
            m_lstFPTNegotiationRoundVM.ForEach(i => i.FPTScheduleEnd = m_dtstart);
            m_lstFPTNegotiationRoundVM.ForEach(i => i.FPTScheduleEnd = m_dtend);
            m_lstFPTNegotiationRoundVM.ForEach(i => i.FPTDuration = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.Duration).ToString()).Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.Duration).ToString()).FirstOrDefault().Value : null);
            m_lstFPTNegotiationRoundVM.ForEach(i => i.MaintenancePeriod = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.MaintenancePeriod).ToString()).Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.MaintenancePeriod).ToString()).FirstOrDefault().Value : null);
            m_lstFPTNegotiationRoundVM.ForEach(i => i.Guarantee = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.Guarantee).ToString()).Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.Guarantee).ToString()).FirstOrDefault().Value : null);
            m_lstFPTNegotiationRoundVM.ForEach(i => i.ContractType = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.ContractType).ToString()).Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.ContractType).ToString()).FirstOrDefault().Value : null);
            m_lstFPTNegotiationRoundVM.ForEach(i => i.PaymentMethod = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.PaymentMethod).ToString()).Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == ((int)AdditionalInfoItems.PaymentMethod).ToString()).FirstOrDefault().Value : null);


            //decimal m_decpricemargin = 0;
            //m_decpricemargin = GetValuePriceTolerance();
            ViewDataDictionary m_vddFPT = new ViewDataDictionary();
            m_vddFPT.Add("FPTIDData", m_strFPTID);
            //m_vddFPT.Add("PriceMarginData", m_decpricemargin);
            m_vddFPT.Add("RoundIDData", m_strRoundID);
            m_vddFPT.Add("SubItemLevelData", m_intSubItemLevelData);
            m_vddFPT.Add("VendorNameData", m_UserVM.VendorDesc);
            m_vddFPT.Add("FPTNameData", m_strFPTID);
            m_vddFPT.Add("TimeRemaining", DateTime.Now.AddMinutes(2).ToString());
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;

            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_lstFPTNegotiationRoundVM.FirstOrDefault(),
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddFPT,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult Submit()
        {
            Dictionary<string, object>[] m_arrSctructure = JSON.Deserialize<Dictionary<string, object>[]>(Request.Params["treeStructure"]);
            string roundID = Request.Params["roundID"].ToString();
            string budgetPlandID = (Request.Params["budgetPlanID"] == null) ? string.Empty : Request.Params["budgetPlanID"].ToString();
            string m_FPTVendorParticipantID = Request.Params["vendorParticipantID"].ToString();
            string VendorID = Request.Params["VendorID"].ToString();

            List<DNegotiationBidEntry> m_lstDNegotiationBidEntry = new List<DNegotiationBidEntry>();

            foreach (Dictionary<string, object> m_dicNegotiationBidStructuresVM in m_arrSctructure)
            {

                DNegotiationBidEntry m_DNegotiationBidEntry = new DNegotiationBidEntry();
                m_DNegotiationBidEntry.NegotiationEntryID = Guid.NewGuid().ToString().Replace("-", "");

                string m_strbidtype = string.Empty;
                switch (m_dicNegotiationBidStructuresVM["sequencelevel"].ToString())
                {
                    case "7777":
                        m_strbidtype = General.EnumDesc(VendorBidTypes.GrandTotal);
                        break;
                    case "8888":
                        m_strbidtype = General.EnumDesc(VendorBidTypes.Fee);
                        break;
                    case "9999":
                        m_strbidtype = General.EnumDesc(VendorBidTypes.AfterFee);
                        break;
                    default:
                        m_strbidtype = General.EnumDesc(VendorBidTypes.SubItem);
                        break;
                }
                m_DNegotiationBidEntry.BidTypeID = m_strbidtype;
                m_DNegotiationBidEntry.NegotiationBidID = m_dicNegotiationBidStructuresVM[NegotiationBidStructuresVM.Prop.NegotiationBidID.Name.ToLower()].ToString();
                m_DNegotiationBidEntry.RoundID = roundID;
                m_DNegotiationBidEntry.FPTVendorParticipantID = m_FPTVendorParticipantID;
                m_DNegotiationBidEntry.BidValue = Convert.ToDecimal(m_dicNegotiationBidStructuresVM[NegotiationBidStructuresVM.Prop.Bid.Name.ToLower()].ToString());
                m_lstDNegotiationBidEntry.Add(m_DNegotiationBidEntry);

            }

            if (!m_lstDNegotiationBidEntry.Any())
            {
                Global.ShowInfoAlert(title, "Nothing to update");
                return this.Direct(true);
            }
            List<string> m_lstMessage = new List<string>();

            if (!isSaveValid(m_lstDNegotiationBidEntry, ref m_lstMessage))
            {
                Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
                return this.Direct(true);
            }

            DNegotiationBidEntryDA m_objDNegotiationBidEntryDA = new DNegotiationBidEntryDA();
            m_objDNegotiationBidEntryDA.ConnectionStringName = Global.ConnStrConfigName;
            string m_strTransName = "DNegotiationBidEntry";
            object m_objDBConnection = null;
            m_objDBConnection = m_objDNegotiationBidEntryDA.BeginTrans(m_strTransName);
            string m_strMessage = string.Empty;


            try
            {
                string m_strVendorID = getCurentUser().VendorID;
                //DELETE
                Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                List<object> m_lstFilter = new List<object>();

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(m_lstDNegotiationBidEntry.FirstOrDefault().RoundID);
                m_objFilter.Add(NegotiationBidEntryVM.Prop.RoundID.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(m_FPTVendorParticipantID);
                m_objFilter.Add(NegotiationBidEntryVM.Prop.FPTVendorParticipantID.Map, m_lstFilter);

                m_objDNegotiationBidEntryDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                if (m_objDNegotiationBidEntryDA.Message == General.EnumDesc(MessageLib.NotExist)) m_objDNegotiationBidEntryDA.Message = "";

                //INSERT
                foreach (DNegotiationBidEntry m_DNegotiationBidEntry in m_lstDNegotiationBidEntry)
                {
                    m_objDNegotiationBidEntryDA.Data = m_DNegotiationBidEntry;
                    m_objDNegotiationBidEntryDA.Insert(true, m_objDBConnection);
                }
                if (!m_objDNegotiationBidEntryDA.Success || m_objDNegotiationBidEntryDA.Message != string.Empty)
                    m_lstMessage.Add(m_objDNegotiationBidEntryDA.Message);

                m_objDNegotiationBidEntryDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                m_objDNegotiationBidEntryDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
            }

            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfoAlert(title, "Submited");
                NegotiationHub.BroadcastBidMonitoring(budgetPlandID, roundID, VendorID);
                return this.Direct(true);
            }
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);
        }
        public ActionResult GetActiveRound()
        {
            List<FPTNegotiationRoundVM> m_lstFPTNegotiationRoundVM = getCurentRound();
            if (m_lstFPTNegotiationRoundVM.Any())
            {
                return this.Direct(m_lstFPTNegotiationRoundVM);
            }
            return this.Direct(false);
        }
        public ActionResult IsOwnFPT(string FPTID, string RoundID)
        {
            List<FPTNegotiationRoundVM> m_lstFPTNegotiationRoundVM = getCurentRound();
            if (m_lstFPTNegotiationRoundVM.Where(x => x.FPTID == FPTID && x.RoundID == RoundID).Any())
            {
                return this.Direct(new { objresult = true });
            }
            return this.Direct(false);
        }
        #endregion

        #region Private Method
        private List<NegotiationBidStructuresVM> getNegoStructure(string FPTID, string VendorID)
        {
            //List Structure FPT
            List<NegotiationBidStructuresVM> m_lstNegotiationBidStructuresVM = new List<NegotiationBidStructuresVM>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();
            List<string> m_lstGroup = new List<string>();

            TNegotiationBidStructuresDA m_objTNegotiationBidStructuresDA = new TNegotiationBidStructuresDA();
            m_objTNegotiationBidStructuresDA.ConnectionStringName = Global.ConnStrConfigName;
            string m_strconfig = string.Join(",", getParticipanConfig(FPTID, VendorID).Select(x => x.NegotiationConfigID).ToArray());

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(m_strconfig);
            m_objFilter.Add(NegotiationBidStructuresVM.Prop.NegotiationConfigID.Map, m_lstFilter);

            //m_lstFilter = new List<object>();
            //m_lstFilter.Add(Operator.Equals);
            //m_lstFilter.Add(FPTID);
            //m_objFilter.Add(NegotiationBidStructuresVM.Prop.FPTID.Map, m_lstFilter);

            m_lstSelect = new List<string>();
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.NegotiationBidID.MapAlias);
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.NegotiationConfigID.MapAlias);
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.Sequence.MapAlias);
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.ItemParentID.MapAlias);
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.FPTID.MapAlias);
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.ParameterValue.MapAlias);
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.ProjectID.MapAlias);
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.ProjectDesc.MapAlias);
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.BudgetPlanDefaultValue.MapAlias);
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.Version.MapAlias);
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.ParentVersion.MapAlias);
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.ParentSequence.MapAlias);
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.BPVersionName.MapAlias);

            m_lstGroup = new List<string>();
            m_lstGroup.Add(NegotiationBidStructuresVM.Prop.NegotiationBidID.Map);
            m_lstGroup.Add(NegotiationBidStructuresVM.Prop.NegotiationConfigID.Map);
            m_lstGroup.Add(NegotiationBidStructuresVM.Prop.Sequence.Map);
            m_lstGroup.Add(NegotiationBidStructuresVM.Prop.ItemID.Map);
            m_lstGroup.Add(NegotiationBidStructuresVM.Prop.ItemDesc.Map);
            m_lstGroup.Add(NegotiationBidStructuresVM.Prop.ItemParentID.Map);
            m_lstGroup.Add(NegotiationBidStructuresVM.Prop.FPTID.Map);
            m_lstGroup.Add(NegotiationBidStructuresVM.Prop.ParameterValue.Map);
            m_lstGroup.Add(NegotiationBidStructuresVM.Prop.ProjectID.Map);
            m_lstGroup.Add(NegotiationBidStructuresVM.Prop.ProjectDesc.Map);
            m_lstGroup.Add(NegotiationBidStructuresVM.Prop.BudgetPlanDefaultValue.Map);
            m_lstGroup.Add(NegotiationBidStructuresVM.Prop.Version.Map);
            m_lstGroup.Add(NegotiationBidStructuresVM.Prop.ParentVersion.Map);
            m_lstGroup.Add(NegotiationBidStructuresVM.Prop.ParentSequence.Map);
            m_lstGroup.Add(NegotiationBidStructuresVM.Prop.BPVersionName.Map);

            Dictionary<int, DataSet> m_dicTNegotiationBidStructuresDA = m_objTNegotiationBidStructuresDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, m_lstGroup, null, null);

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
                    m_objNegotiationBidStructuresVM.FPTID = item[NegotiationBidStructuresVM.Prop.FPTID.Name].ToString();
                    m_objNegotiationBidStructuresVM.ParameterValue = item[NegotiationBidStructuresVM.Prop.ParameterValue.Name].ToString();
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
            return m_lstNegotiationBidStructuresVM;
        }
        private List<NegotiationBidEntryVM> getNegoEntry(string FPTID, string VendorID)
        {
            List<NegotiationBidEntryVM> m_lstNegotiationBidEntryVM = new List<NegotiationBidEntryVM>();

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();
            DNegotiationBidEntryDA m_objDNegotiationBidEntryDA = new DNegotiationBidEntryDA();
            m_objDNegotiationBidEntryDA.ConnectionStringName = Global.ConnStrConfigName;

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(VendorID);
            m_objFilter.Add(NegotiationBidEntryVM.Prop.VendorID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(FPTID);
            m_objFilter.Add(NegotiationBidEntryVM.Prop.FPTID.Map, m_lstFilter);

            m_lstSelect = new List<string>();
            m_lstSelect.Add(NegotiationBidEntryVM.Prop.NegotiationEntryID.MapAlias);
            m_lstSelect.Add(NegotiationBidEntryVM.Prop.BidTypeID.MapAlias);
            m_lstSelect.Add(NegotiationBidEntryVM.Prop.NegotiationBidID.MapAlias);
            m_lstSelect.Add(NegotiationBidEntryVM.Prop.RoundID.MapAlias);
            m_lstSelect.Add(NegotiationBidEntryVM.Prop.BidValue.MapAlias);


            Dictionary<int, DataSet> m_dicDNegotiationBidEntryDA = m_objDNegotiationBidEntryDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objDNegotiationBidEntryDA.Success)
            {
                foreach (DataRow item in m_dicDNegotiationBidEntryDA[0].Tables[0].Rows)
                {
                    NegotiationBidEntryVM m_objNegotiationBidEntryVM = new NegotiationBidEntryVM();
                    m_objNegotiationBidEntryVM.NegotiationEntryID = item[NegotiationBidEntryVM.Prop.NegotiationEntryID.Name].ToString();
                    m_objNegotiationBidEntryVM.BidTypeID = item[NegotiationBidEntryVM.Prop.BidTypeID.Name].ToString();
                    m_objNegotiationBidEntryVM.NegotiationBidID = item[NegotiationBidEntryVM.Prop.NegotiationBidID.Name].ToString();
                    m_objNegotiationBidEntryVM.RoundID = item[NegotiationBidEntryVM.Prop.RoundID.Name].ToString();
                    m_objNegotiationBidEntryVM.BidValue = Convert.ToDecimal(item[NegotiationBidEntryVM.Prop.BidValue.Name].ToString());
                    m_lstNegotiationBidEntryVM.Add(m_objNegotiationBidEntryVM);
                }
            }

            return m_lstNegotiationBidEntryVM;
        }
        private List<FPTNegotiationRoundVM> getNegoRound(string RoundID)
        {
            List<FPTNegotiationRoundVM> m_lstFPTNegotiationRoundVM = new List<FPTNegotiationRoundVM>();

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();
            MFPTDA m_objMFPTDA = new MFPTDA();
            m_objMFPTDA.ConnectionStringName = Global.ConnStrConfigName;

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(RoundID);
            m_objFilter.Add(FPTNegotiationRoundVM.Prop.RoundID.Map, m_lstFilter);

            //m_lstFilter = new List<object>();
            //m_lstFilter.Add(Operator.In);
            //m_lstFilter.Add(getVendorFPT());
            //m_objFilter.Add(FPTVM.Prop.FPTID.Map, m_lstFilter);

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
            m_lstSelect.Add(FPTNegotiationRoundVM.Prop.BottomValue.MapAlias);
            m_lstSelect.Add(FPTNegotiationRoundVM.Prop.TopValue.MapAlias);

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
                    m_objFPTNegotiationRoundVM.Duration = string.IsNullOrEmpty(m_drMFPTDA[FPTNegotiationRoundVM.Prop.Duration.Name].ToString()) ? (int?)null : int.Parse(m_drMFPTDA[FPTNegotiationRoundVM.Prop.Duration.Name].ToString());
                    m_objFPTNegotiationRoundVM.TotalRound = string.IsNullOrEmpty(m_drMFPTDA[FPTNegotiationRoundVM.Prop.Round.Name].ToString()) ? (int?)null : int.Parse(m_drMFPTDA[FPTNegotiationRoundVM.Prop.Round.Name].ToString());
                    m_objFPTNegotiationRoundVM.RoundNo = string.IsNullOrEmpty(m_drMFPTDA[FPTNegotiationRoundVM.Prop.RoundNo.Name].ToString()) ? (int?)null : int.Parse(m_drMFPTDA[FPTNegotiationRoundVM.Prop.RoundNo.Name].ToString());
                    //m_objFPTNegotiationRoundVM.Schedule = DateTime.Now;//todo:
                    m_objFPTNegotiationRoundVM.NextRoundID = m_drMFPTDA[FPTNegotiationRoundVM.Prop.NextRoundID.Name].ToString();
                    m_objFPTNegotiationRoundVM.PreviousRoundID = m_drMFPTDA[FPTNegotiationRoundVM.Prop.PreviousRoundID.Name].ToString();
                    m_objFPTNegotiationRoundVM.BottomValue = (decimal)m_drMFPTDA[FPTNegotiationRoundVM.Prop.BottomValue.Name];
                    m_objFPTNegotiationRoundVM.TopValue = (decimal)m_drMFPTDA[FPTNegotiationRoundVM.Prop.TopValue.Name];
                    m_lstFPTNegotiationRoundVM.Add(m_objFPTNegotiationRoundVM);
                }
            }
            return m_lstFPTNegotiationRoundVM;
        }

        private List<FPTNegotiationRoundVM> getlstNegoRound(string FPTID)
        {
            List<FPTNegotiationRoundVM> m_lstFPTNegotiationRoundVM = new List<FPTNegotiationRoundVM>();

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();
            DFPTNegotiationRoundsDA m_objDFPTNegotiationRoundsDA = new DFPTNegotiationRoundsDA();
            m_objDFPTNegotiationRoundsDA.ConnectionStringName = Global.ConnStrConfigName;

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(FPTID);
            m_objFilter.Add(FPTNegotiationRoundVM.Prop.FPTID.Map, m_lstFilter);

            m_lstSelect = new List<string>();
            m_lstSelect.Add(FPTNegotiationRoundVM.Prop.FPTID.MapAlias);
            m_lstSelect.Add(FPTNegotiationRoundVM.Prop.RoundID.MapAlias);
            m_lstSelect.Add(FPTNegotiationRoundVM.Prop.StartDateTimeStamp.MapAlias);
            m_lstSelect.Add(FPTNegotiationRoundVM.Prop.EndDateTimeStamp.MapAlias);
            m_lstSelect.Add(FPTNegotiationRoundVM.Prop.BottomValue.MapAlias);
            m_lstSelect.Add(FPTNegotiationRoundVM.Prop.TopValue.MapAlias);
            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(FPTNegotiationRoundVM.Prop.RoundID.Map, OrderDirection.Descending);
            Dictionary<int, DataSet> m_dicMFPTDA = m_objDFPTNegotiationRoundsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);

            if (m_objDFPTNegotiationRoundsDA.Success)
            {
                foreach (DataRow m_drMFPTDA in m_dicMFPTDA[0].Tables[0].Rows)
                {
                    FPTNegotiationRoundVM m_objFPTNegotiationRoundVM = new FPTNegotiationRoundVM();

                    m_objFPTNegotiationRoundVM.FPTID = m_drMFPTDA[FPTNegotiationRoundVM.Prop.FPTID.Name].ToString();
                    m_objFPTNegotiationRoundVM.RoundID = m_drMFPTDA[FPTNegotiationRoundVM.Prop.RoundID.Name].ToString();
                    m_objFPTNegotiationRoundVM.StartDateTimeStamp = !string.IsNullOrEmpty(m_drMFPTDA[FPTNegotiationRoundVM.Prop.StartDateTimeStamp.Name].ToString()) ? DateTime.Parse(m_drMFPTDA[FPTNegotiationRoundVM.Prop.StartDateTimeStamp.Name].ToString()) : (DateTime?)null;
                    m_objFPTNegotiationRoundVM.EndDateTimeStamp = !string.IsNullOrEmpty(m_drMFPTDA[FPTNegotiationRoundVM.Prop.EndDateTimeStamp.Name].ToString()) ? DateTime.Parse(m_drMFPTDA[FPTNegotiationRoundVM.Prop.EndDateTimeStamp.Name].ToString()) : (DateTime?)null;
                    m_objFPTNegotiationRoundVM.BottomValue = (decimal)m_drMFPTDA[FPTNegotiationRoundVM.Prop.BottomValue.Name];
                    m_objFPTNegotiationRoundVM.TopValue = (decimal)m_drMFPTDA[FPTNegotiationRoundVM.Prop.TopValue.Name];
                    m_lstFPTNegotiationRoundVM.Add(m_objFPTNegotiationRoundVM);
                }
            }
            return m_lstFPTNegotiationRoundVM;
        }
        private List<FPTNegotiationRoundVM> getCurentRound()
        {
            List<FPTNegotiationRoundVM> m_lstFPTNegotiationRoundVM = new List<FPTNegotiationRoundVM>();

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();
            DFPTNegotiationRoundsDA m_objDFPTNegotiationRoundsDA = new DFPTNegotiationRoundsDA();
            m_objDFPTNegotiationRoundsDA.ConnectionStringName = Global.ConnStrConfigName;

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.LessThanEqual);
            m_lstFilter.Add(DateTime.Now);
            m_objFilter.Add(FPTNegotiationRoundVM.Prop.StartDateTimeStamp.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.GreaterThanEqual);
            m_lstFilter.Add(DateTime.Now);
            m_objFilter.Add(FPTNegotiationRoundVM.Prop.EndDateTimeStamp.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(getVendorFPT());
            m_objFilter.Add(FPTVM.Prop.FPTID.Map, m_lstFilter);

            m_lstSelect = new List<string>();
            m_lstSelect.Add(FPTNegotiationRoundVM.Prop.RoundID.MapAlias);
            m_lstSelect.Add(FPTNegotiationRoundVM.Prop.FPTID.MapAlias);
            m_lstSelect.Add(FPTNegotiationRoundVM.Prop.BottomValue.MapAlias);
            m_lstSelect.Add(FPTNegotiationRoundVM.Prop.TopValue.MapAlias);

            Dictionary<int, DataSet> m_dicDFPTNegotiationRoundsDA = m_objDFPTNegotiationRoundsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objDFPTNegotiationRoundsDA.Success)
            {
                foreach (DataRow m_drDFPTNegotiationRoundsDA in m_dicDFPTNegotiationRoundsDA[0].Tables[0].Rows)
                {
                    FPTNegotiationRoundVM m_objFPTNegotiationRoundVM = new FPTNegotiationRoundVM();
                    m_objFPTNegotiationRoundVM.RoundID = m_drDFPTNegotiationRoundsDA[FPTNegotiationRoundVM.Prop.RoundID.Name].ToString();
                    m_objFPTNegotiationRoundVM.FPTID = m_drDFPTNegotiationRoundsDA[FPTNegotiationRoundVM.Prop.FPTID.Name].ToString();
                    m_objFPTNegotiationRoundVM.BottomValue = (decimal)m_drDFPTNegotiationRoundsDA[FPTNegotiationRoundVM.Prop.BottomValue.Name];
                    m_objFPTNegotiationRoundVM.TopValue = (decimal)m_drDFPTNegotiationRoundsDA[FPTNegotiationRoundVM.Prop.TopValue.Name];
                    m_lstFPTNegotiationRoundVM.Add(m_objFPTNegotiationRoundVM);
                }
            }
            return m_lstFPTNegotiationRoundVM;
        }
        private List<NegotiationConfigurationsVM> getNegoConfig(string FPTID, string NegotiationConfigTypeID = "")
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
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.ParameterValue2.MapAlias);
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
                    m_objNegotiationConfigurationsVM.ParameterValue2 = m_drCNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.ParameterValue2.Name].ToString();
                    m_lstNegotiationConfigurationsVM.Add(m_objNegotiationConfigurationsVM);
                }
            }
            return m_lstNegotiationConfigurationsVM;
        }
        private TreePanel generateTreePanel(List<NegotiationBidStructuresVM> ListNego, int LastLevel, decimal BVal, decimal TVal, string BPID = "")
        {

            TreePanel m_treepanel = new TreePanel
            {
                ID = "treePanel" + BPID + "structure",
                Padding = 10,
                MinHeight = 200,
                Height = 500,
                RowLines = true,
                ColumnLines = true,
                UseArrows = true,
                RootVisible = false,
                SortableColumns = false,
                FolderSort = false,
                Tag = new { lastLevel = LastLevel, bVal = BVal, tVal = TVal }
            };

            m_treepanel.ViewConfig = new TreeView() { ToggleOnDblClick = false };

            //SelectionModel
            RowSelectionModel m_rowSelectionModel = new RowSelectionModel() { Mode = SelectionMode.Multi, AllowDeselect = true };
            m_treepanel.SelectionModel.Add(m_rowSelectionModel);

            //Expand

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
            m_objModelField = new ModelField { Name = NegotiationBidStructuresVM.Prop.Bid.Name.ToLower() };
            m_objModelFieldCollection.Add(m_objModelField);
            m_objModelField = new ModelField { Name = NegotiationBidStructuresVM.Prop.LastBid.Name.ToLower() };
            m_objModelFieldCollection.Add(m_objModelField);
            m_objModelField = new ModelField { Name = NegotiationBidStructuresVM.Prop.LastOffer.Name.ToLower() };
            m_objModelFieldCollection.Add(m_objModelField);
            m_treepanel.Fields.AddRange(m_objModelFieldCollection);

            //Columns
            List<ColumnBase> m_ListColumn = new List<ColumnBase>();
            ColumnBase m_objColumn = new Column();
            NumberField m_objNumberField = new NumberField();
            m_objColumn = new Column { Text = NegotiationBidStructuresVM.Prop.Sequence.Desc, DataIndex = NegotiationBidStructuresVM.Prop.Sequence.Name.ToLower(), Flex = 1, Hidden = true };
            m_ListColumn.Add(m_objColumn);
            m_objColumn = new TreeColumn { Text = NegotiationBidStructuresVM.Prop.ItemDesc.Desc, DataIndex = NegotiationBidStructuresVM.Prop.ItemDesc.Name.ToLower(), Flex = 3 };
            m_ListColumn.Add(m_objColumn);
            m_objColumn = new NumberColumn
            {
                Text = NegotiationBidStructuresVM.Prop.LastOffer.Desc,
                DataIndex = NegotiationBidStructuresVM.Prop.LastOffer.Name.ToLower(),
                Flex = 1,
                Align = ColumnAlign.End,
                Format = Global.DefaultNumberFormat
            };
            m_objColumn.Renderer = new Renderer("renderTreeColumn");
            m_ListColumn.Add(m_objColumn);
            m_objColumn = new NumberColumn
            {
                Text = NegotiationBidStructuresVM.Prop.LastBid.Desc,
                DataIndex = NegotiationBidStructuresVM.Prop.LastBid.Name.ToLower(),
                Flex = 1,
                Align = ColumnAlign.End,
                Format = Global.DefaultNumberFormat
            };
            m_objColumn.Renderer = new Renderer("renderTreeColumn");
            m_ListColumn.Add(m_objColumn);
            m_objColumn = new NumberColumn
            {
                Text = NegotiationBidStructuresVM.Prop.Bid.Desc,
                DataIndex = NegotiationBidStructuresVM.Prop.Bid.Name.ToLower(),
                Flex = 1,
                Align = ColumnAlign.End,
                Format = Global.DefaultNumberFormat

            };
            m_objColumn.Renderer = new Renderer("renderTreeColumn");
            m_objNumberField = new NumberField()
            {
                //ID = "ColBid",
                HideTrigger = true,
                EnforceMaxLength = true,
                MinValue = 0,
                DecimalPrecision = 4,
                SpinDownEnabled = false,
                SpinUpEnabled = false
            };
            m_objColumn.Editor.Add(m_objNumberField);

            m_ListColumn.Add(m_objColumn);
            m_treepanel.ColumnModel.Columns.AddRange(m_ListColumn);

            //Root
            Node m_node = new Node() { NodeID = "root", Expanded = true };
            NodeCollection m_ncStructure = LoadNodeStructure(ListNego.Where(y => y.Sequence != 7777 && y.Sequence != 8888 && y.Sequence != 9999).OrderBy(x => x.Sequence).ToList(), "0", LastLevel, 0, 0);
            m_node.Children.AddRange(m_ncStructure);
            m_node.Children.Add(new Node() { NodeID = "######", Leaf = true, IconCls = "display: none !important;", Expandable = false });
            //Total
            var m_objGrandTotal = ListNego.Where(x => x.Sequence == 7777).FirstOrDefault(); //ListNego.OrderByDescending(x => x.Sequence).FirstOrDefault(); 
            Node m_nodeGrandTotal = new Node();
            m_nodeGrandTotal.NodeID = m_objGrandTotal.NegotiationBidID;
            m_nodeGrandTotal.Icon = Icon.Sum;
            m_nodeGrandTotal.Expanded = false;
            m_nodeGrandTotal.Leaf = true;
            m_nodeGrandTotal.AttributesObject = new
            {
                negotiationbidid = m_objGrandTotal.NegotiationBidID,
                itemid = m_objGrandTotal.ItemID,
                itemdesc = m_objGrandTotal.ItemDesc,
                itemparentid = m_objGrandTotal.ItemParentID,
                sequencelevel = 7777,
                bid = m_objGrandTotal.Bid,
                lastbid = m_objGrandTotal.LastBid,
                lastoffer = m_objGrandTotal.LastOffer,
                bpvalue = m_objGrandTotal.BudgetPlanDefaultValue
            };
            m_node.Children.Add(m_nodeGrandTotal);
            //fee
            var m_objFee = ListNego.Where(x => x.Sequence == 8888).FirstOrDefault(); //ListNego.OrderByDescending(x => x.Sequence).FirstOrDefault(); 
            Node m_nodeFee = new Node();
            m_nodeFee.NodeID = m_objFee.NegotiationBidID;
            m_nodeFee.Icon = Icon.None;
            m_nodeFee.IconCls = "display: none !important;";
            m_nodeFee.Expanded = false;
            m_nodeFee.Leaf = true;
            m_nodeFee.AttributesObject = new
            {
                negotiationbidid = m_objFee.NegotiationBidID,
                itemid = m_objFee.ItemID,
                itemdesc = m_objFee.ItemDesc,
                itemparentid = m_objFee.ItemParentID,
                sequencelevel = 8888,
                bid = m_objFee.Bid,
                lastbid = m_objFee.LastBid,
                lastoffer = m_objFee.LastOffer,
                bpvalue = m_objFee.BudgetPlanDefaultValue
            };
            m_node.Children.Add(m_nodeFee);
            //after fee
            var m_objAfterFee = ListNego.Where(x => x.Sequence == 9999).FirstOrDefault(); //ListNego.OrderByDescending(x => x.Sequence).FirstOrDefault(); 
            Node m_nodeAfterFee = new Node();
            m_nodeAfterFee.NodeID = m_objAfterFee.NegotiationBidID;
            m_nodeAfterFee.Icon = Icon.Sum;
            m_nodeAfterFee.Expanded = false;
            m_nodeAfterFee.Leaf = true;
            m_nodeAfterFee.AttributesObject = new
            {
                negotiationbidid = m_objAfterFee.NegotiationBidID,
                itemid = m_objAfterFee.ItemID,
                itemdesc = m_objAfterFee.ItemDesc,
                itemparentid = m_objAfterFee.ItemParentID,
                sequencelevel = 9999,
                bid = ((m_objFee.Bid / 100) + 1) * m_objGrandTotal.Bid,
                lastbid = ((m_objFee.LastBid / 100) + 1) * m_objGrandTotal.LastBid,
                lastoffer = ((m_objFee.LastOffer / 100) + 1) * m_objGrandTotal.LastOffer,
                bpvalue = ((m_objFee.BudgetPlanDefaultValue / 100) + 1) * m_objGrandTotal.BudgetPlanDefaultValue
            };
            m_node.Children.Add(m_nodeAfterFee);

            m_treepanel.Root.Add(m_node);

            //Listener
            m_treepanel.Listeners.ViewReady.Fn = "viewReady";

            //Plugins
            CellEditing m_objCellEditing = new CellEditing { ClicksToEdit = 1/*, ID = "cellEditorStructure"*/ };
            m_objCellEditing.Listeners.Edit.Fn = "editBidStructure";
            m_objCellEditing.Listeners.BeforeEdit.Fn = "beforeEditBidStructure";
            m_treepanel.Plugins.Add(m_objCellEditing);

            //Topbar
            Toolbar m_objToolbar = new Toolbar { PaddingSpec = "" };
            m_treepanel.TopBar.Add(m_objToolbar);

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
                        bid = item.Bid,
                        lastoffer = item.LastOffer,
                        lastbid = item.LastBid,
                        bpvalue = item.BudgetPlanDefaultValue
                    };
                    m_nodeColChild.Add(m_node);
                    m_intSequence++;
                }



            }
            return m_nodeColChild;
        }
        private string getVendorFPT()
        {
            List<string> FPTList = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();
            string m_strVendorID = getCurentUser().VendorID;
            string m_strUserID = string.Empty;

            DFPTVendorParticipantsDA m_objDFPTVendorParticipantsDA = new DFPTVendorParticipantsDA();
            m_objDFPTVendorParticipantsDA.ConnectionStringName = Global.ConnStrConfigName;
            m_objFilter = new Dictionary<string, List<object>>();

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(m_strVendorID);
            m_objFilter.Add(FPTVendorParticipantsVM.Prop.VendorID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(1);
            m_objFilter.Add(FPTVendorParticipantsVM.Prop.StatusID.Map, m_lstFilter);

            m_lstSelect = new List<string>();
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.FPTID.MapAlias);

            Dictionary<int, DataSet> m_dicDFPTVendorParticipantsDA = m_objDFPTVendorParticipantsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objDFPTVendorParticipantsDA.Success)
            {
                foreach (DataRow item in m_dicDFPTVendorParticipantsDA[0].Tables[0].Rows)
                {
                    FPTList.Add(item[FPTVendorParticipantsVM.Prop.FPTID.Name].ToString());
                }
            }

            return String.Join(",", FPTList.Distinct().ToArray());
        }
        private List<FPTVendorParticipantsVM> getParticipanConfig(string FPTID, string VendorID)
        {
            List<FPTVendorParticipantsVM> m_lstParticipanConfig = new List<FPTVendorParticipantsVM>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();

            DFPTVendorParticipantsDA m_objDFPTVendorParticipantsDA = new DFPTVendorParticipantsDA();
            m_objDFPTVendorParticipantsDA.ConnectionStringName = Global.ConnStrConfigName;
            m_objFilter = new Dictionary<string, List<object>>();

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(VendorID);
            m_objFilter.Add(FPTVendorParticipantsVM.Prop.VendorID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(FPTID);
            m_objFilter.Add(FPTVendorParticipantsVM.Prop.FPTID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(General.EnumDesc(NegoConfigTypes.BudgetPlan));
            m_objFilter.Add(FPTVendorParticipantsVM.Prop.NegotiationConfigTypeID.Map, m_lstFilter);

            m_lstSelect = new List<string>();
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.NegotiationConfigID.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.ParameterValue.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.FPTVendorParticipantID.MapAlias);

            Dictionary<int, DataSet> m_dicDFPTVendorParticipantsDA = m_objDFPTVendorParticipantsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objDFPTVendorParticipantsDA.Success)
            {
                foreach (DataRow item in m_dicDFPTVendorParticipantsDA[0].Tables[0].Rows)
                {
                    FPTVendorParticipantsVM m_objFPTVendorParticipantsVM = new FPTVendorParticipantsVM();
                    m_objFPTVendorParticipantsVM.NegotiationConfigID = item[FPTVendorParticipantsVM.Prop.NegotiationConfigID.Name].ToString();
                    m_objFPTVendorParticipantsVM.ParameterValue = item[FPTVendorParticipantsVM.Prop.ParameterValue.Name].ToString();
                    m_objFPTVendorParticipantsVM.FPTVendorParticipantID = item[FPTVendorParticipantsVM.Prop.FPTVendorParticipantID.Name].ToString();
                    m_lstParticipanConfig.Add(m_objFPTVendorParticipantsVM);
                }
            }

            return m_lstParticipanConfig;
        }
        private bool isSaveValid(List<DNegotiationBidEntry> m_lstDNegotiationBidEntry, ref List<string> message)
        {
            //todo: validation
            bool m_boolretVal = true;
            string roundID = m_lstDNegotiationBidEntry.FirstOrDefault().RoundID;
            //Check curent round
            List<FPTNegotiationRoundVM> m_lstFPTNegotiationRoundVM = new List<FPTNegotiationRoundVM>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();
            DFPTNegotiationRoundsDA m_objDFPTNegotiationRoundsDA = new DFPTNegotiationRoundsDA();
            m_objDFPTNegotiationRoundsDA.ConnectionStringName = Global.ConnStrConfigName;

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(roundID);
            m_objFilter.Add(FPTNegotiationRoundVM.Prop.RoundID.Map, m_lstFilter);

            m_lstSelect = new List<string>();
            m_lstSelect.Add(FPTNegotiationRoundVM.Prop.StartDateTimeStamp.MapAlias);
            m_lstSelect.Add(FPTNegotiationRoundVM.Prop.EndDateTimeStamp.MapAlias);

            Dictionary<int, DataSet> m_dicDFPTNegotiationRounds = m_objDFPTNegotiationRoundsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objDFPTNegotiationRoundsDA.Success)
            {
                foreach (DataRow item in m_dicDFPTNegotiationRounds[0].Tables[0].Rows)
                {
                    FPTNegotiationRoundVM m_objFPTNegotiationRoundVM = new FPTNegotiationRoundVM();
                    m_objFPTNegotiationRoundVM.StartDateTimeStamp = (DateTime)item[FPTNegotiationRoundVM.Prop.StartDateTimeStamp.Name];
                    m_objFPTNegotiationRoundVM.EndDateTimeStamp = (DateTime)item[FPTNegotiationRoundVM.Prop.EndDateTimeStamp.Name];
                    m_lstFPTNegotiationRoundVM.Add(m_objFPTNegotiationRoundVM);
                }
            }

            if (!(m_lstFPTNegotiationRoundVM.FirstOrDefault().StartDateTimeStamp <= DateTime.Now && m_lstFPTNegotiationRoundVM.FirstOrDefault().EndDateTimeStamp >= DateTime.Now))
            {
                message.Add("Round time is up");
                return false;
            }


            return m_boolretVal;
        }
       
        private List<FPTAdditionalInfoVM> GetListFPTAdditionalInfoVM(string FPTID)
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


            return m_lstFPTAdditionalInfoVM;
        }
        #endregion
    }
}