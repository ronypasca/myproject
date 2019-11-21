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
using Novacode;
using System.IO;
using System.IO.Compression;
using Spire.Doc;

namespace com.SML.BIGTRONS.Controllers
{
    public class NegotiationReportController : BaseController
    {
        private readonly string title = "Negotiation Report";
        private readonly string dataSessionName = "FormData";

        #region public Action
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

            this.GetCmp<Ext.Net.Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 0;
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

            List<string> m_lstmanualfilter = new List<string>();
            m_lstmanualfilter.Add("Projects");
            m_lstmanualfilter.Add("BudgetPlans");
            m_lstmanualfilter.Add("Vendors");
            m_lstmanualfilter.Add("LastStatus");

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

                if (!m_lstmanualfilter.Contains(m_strDataIndex))
                {

                    if (m_strDataIndex != string.Empty)
                    {
                        m_strDataIndex = FPTVM.Prop.Map(m_strDataIndex, false);

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

            }

            //m_lstFilter = new List<object>();
            //m_lstFilter.Add(Operator.None);
            //m_lstFilter.Add(string.Empty);
            //m_objFilter.Add("[FPTStatus].StatusDateTimeStamp=[DFPTStatus].StatusDateTimeStamp", m_lstFilter);
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.None);
            m_lstFilter.Add(string.Empty);
            m_objFilter.Add("(select top 1 1 from DFPTStatus where StatusID = 12 and FPTID = MFPT.FPTID) is not null", m_lstFilter);

            Dictionary<int, DataSet> m_dicMFPTDA = m_objMFPTDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpApprovalPathBL in m_dicMFPTDA)
            {
                m_intCount = m_kvpApprovalPathBL.Key;
                break;
            }

            List<FPTVM> m_lstFPTVM = new List<FPTVM>();
            string messages = "";
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(FPTVM.Prop.FPTID.MapAlias);
                m_lstSelect.Add(FPTVM.Prop.Descriptions.MapAlias);
                m_lstSelect.Add(FPTVM.Prop.Schedule.MapAlias);
                m_lstSelect.Add(FPTVM.Prop.ClusterID.MapAlias);
                m_lstSelect.Add(FPTVM.Prop.ClusterDesc.MapAlias);
                m_lstSelect.Add(FPTVM.Prop.ProjectID.MapAlias);
                m_lstSelect.Add(FPTVM.Prop.ProjectDesc.MapAlias);
                m_lstSelect.Add(FPTVM.Prop.DivisionID.MapAlias);
                m_lstSelect.Add(FPTVM.Prop.DivisionDesc.MapAlias);
                m_lstSelect.Add(FPTVM.Prop.BusinessUnitID.MapAlias);
                m_lstSelect.Add(FPTVM.Prop.BusinessUnitDesc.MapAlias);
                m_lstSelect.Add(FPTVM.Prop.CreatedDate.MapAlias);


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
                            CreatedDate = string.IsNullOrEmpty(m_drMFPTDA[FPTVM.Prop.CreatedDate.Name].ToString()) ? null : (DateTime?)m_drMFPTDA[FPTVM.Prop.CreatedDate.Name],
                            Descriptions = m_drMFPTDA[FPTVM.Prop.Descriptions.Name].ToString(),
                            ClusterID = m_drMFPTDA[FPTVM.Prop.ClusterID.Name].ToString(),
                            ClusterDesc = m_drMFPTDA[FPTVM.Prop.ClusterDesc.Name].ToString(),
                            ProjectID = m_drMFPTDA[FPTVM.Prop.ProjectID.Name].ToString(),
                            ProjectDesc = m_drMFPTDA[FPTVM.Prop.ProjectDesc.Name].ToString(),
                            DivisionID = m_drMFPTDA[FPTVM.Prop.DivisionID.Name].ToString(),
                            DivisionDesc = m_drMFPTDA[FPTVM.Prop.DivisionDesc.Name].ToString(),
                            BusinessUnitID = m_drMFPTDA[FPTVM.Prop.BusinessUnitID.Name].ToString(),
                            BusinessUnitDesc = m_drMFPTDA[FPTVM.Prop.BusinessUnitDesc.Name].ToString(),
                            Projects = GetListFPTProjectsVM(m_drMFPTDA[FPTVM.Prop.FPTID.Name].ToString(), ref messages),
                            Vendors = GetListBFPTVendorParticipantsVM(m_drMFPTDA[FPTVM.Prop.FPTID.Name].ToString(), ref messages),
                            BudgetPlans = GetListFPTBudgetPlanVM(m_drMFPTDA[FPTVM.Prop.FPTID.Name].ToString(), ref messages),
                            ListFPTStatusVM = GetListFPTStatusVM(m_drMFPTDA[FPTVM.Prop.FPTID.Name].ToString(), ref messages),
                            Schedule = m_drMFPTDA[FPTVM.Prop.Schedule.Name].ToString(),
                            ListNegotiationConfigurationsVM = GetListNegotiationConfigurationsVM(m_drMFPTDA[FPTVM.Prop.FPTID.Name].ToString(), ref messages)
                        }
                    ).ToList();
                }
            }

            return this.Store(m_lstFPTVM, m_intCount);
        }
        public ActionResult Home()
        {
            this.GetCmp<Ext.Net.Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 0;
            return this.Direct();
        }
        public ActionResult PageOne()
        {
            this.GetCmp<Ext.Net.Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return this.Direct();
        }
        public ActionResult Download(string Selected, int? type, string Lang)
        {
            string m_strLang = (Lang == "EN") ? "EN" : "ID";
            string m_strFPTID;
            string m_strFPTDesc;
            if (string.IsNullOrEmpty(Selected))
            {
                return this.Direct(false);
            }
            Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();
            m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
            m_strFPTID = m_dicSelectedRow[FPTNegotiationRoundVM.Prop.FPTID.Name].ToString();
            m_strFPTDesc = m_dicSelectedRow[FPTNegotiationRoundVM.Prop.Descriptions.Name].ToString();
            int m_intType = (type == null) ? 0 : (int)type;
            switch (m_intType)
            {
                case 1:
                    return ReportBeritaAcara(m_strFPTID, 1, m_strLang);
                case 2:
                    return ReportBeritaAcara(m_strFPTID, 2, m_strLang);
                case 3:
                    return ReportRecommendation(m_strFPTID, m_strFPTDesc, m_strLang);
                case 4:
                    return ReportSPTender(m_strFPTID, 1, m_strLang);
                case 5:
                    return ReportSPTender(m_strFPTID, 2, m_strLang);
                case 6:
                    return ReportSPTender(m_strFPTID, 3, m_strLang);
                case 7:
                    return ReportSPTender(m_strFPTID, 4, m_strLang);
                case 8:
                    return ReportThankYouLetter(m_strFPTID, m_strLang);
                default:
                    return this.Direct(false, "Please select report type");
            }


        }
        public ActionResult GenerateEmail(string Selected, int? type, string Lang)
        {
            string m_strLang = (Lang == "EN") ? "EN" : "ID";
            string m_strFPTID;
            string m_strFPTDesc;
            if (string.IsNullOrEmpty(Selected))
            {
                return this.Direct(false);
            }
            Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();
            m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
            m_strFPTID = m_dicSelectedRow[FPTNegotiationRoundVM.Prop.FPTID.Name].ToString();
            m_strFPTDesc = m_dicSelectedRow[FPTNegotiationRoundVM.Prop.Descriptions.Name].ToString();
            int m_intType = (type == null) ? 0 : (int)type;
            switch (m_intType)
            {
                case 4:
                    return GenerateSPNotification(m_strFPTID, 1, m_strLang);
                case 5:
                    return GenerateSPNotification(m_strFPTID, 2, m_strLang);
                case 6:
                    return GenerateSPNotification(m_strFPTID, 3, m_strLang);
                case 7:
                    return GenerateSPNotification(m_strFPTID, 4, m_strLang);
                case 8:
                    return GenerateTLNotification(m_strFPTID, m_strLang);
                default:
                    return this.Direct(false, "Please select report type");
            }
        }

        #endregion

        #region private Action
        private int OrderingBU(string desc)
        {
            int m_retval = 99;
            switch (desc)
            {
                case "RES":
                    m_retval = 1;
                    break;
                case "FIN":
                    m_retval = 2;
                    break;
                case "TRM":
                    m_retval = 3;
                    break;
                case "PSS":
                    m_retval = 4;
                    break;
                default:
                    break;
            }

            return m_retval;
        }
        private ActionResult ReportBeritaAcara(string FPTID, int type, string Lang)
        {
            //todo: check fpt status
            string message = string.Empty;
            List<FPTStatusVM> m_ListFPTStatusVM = GetListFPTStatusVM(FPTID, ref message);
            if (!m_ListFPTStatusVM.Any(x => x.StatusID == (int)FPTStatusTypes.DoneNegotiation))
            {
                return this.Direct(false, "Negotiation not done yet!");
            }
            //type 1 pengadaan 2 pekerjaan
            List<NegotiationConfigurationsVM> m_lstNegotiationConfigurationsVM = GetNegotiationConfigurationsVM(FPTID);
            List<FPTTCParticipantsVM> m_lsFPTTCParticipantsVM = GetListNegoTCParticipant(FPTID, ref message);
            //var m_lstTCBU = m_lstNegotiationConfigurationsVM.Where(x => x.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.TCMember)).Select(x => new { BusinessUnitIDTC = x.BusinessUnitIDTC, BusinessUnitDesc = x.BusinessUnitDesc, EmployeeName = x.EmployeeName }).Distinct().ToList();
            //var m_lstTCBU = m_lstNegotiationConfigurationsVM.Where(x => x.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.TCMember)).Select(x => new { BusinessUnitIDTC = x.BusinessUnitIDTC, BusinessUnitDesc = x.BusinessUnitDesc, OrderBussinesUnit = OrderingBU(x.BusinessUnitIDTC) }).Distinct().ToList();

            var m_lstTCBU = m_lsFPTTCParticipantsVM.Where(d=>d.StatusID).Select(x => new { BusinessUnitIDTC = x.BusinessUnitID, BusinessUnitDesc = x.BusinessUnitDesc, OrderBussinesUnit = OrderingBU(x.BusinessUnitID) }).Distinct().ToList();

            string m_strdatenego = m_lstNegotiationConfigurationsVM.Where(x => x.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.Schedule)).FirstOrDefault().ParameterValue;
            DateTime m_dtnego = DateTime.MinValue;
            if (m_strdatenego != null) DateTime.TryParse(m_strdatenego, out m_dtnego);

            if (!Directory.Exists(Server.MapPath(@"~/Temp/"))) Directory.CreateDirectory(Server.MapPath(@"~/Temp/"));
            string m_filepath = Server.MapPath($"~/Content/Template/Report/berita-acara-negosiasi-pekerjaan-{Lang}.docx");
            if (type == 1)
            {
                m_filepath = Server.MapPath($"~/Content/Template/Report/berita-acara-negosiasi-pengadaan-{Lang}.docx");
            }


            DocX m_document = null;
            try
            {
                m_document = DocX.Load(m_filepath);
            }
            catch (Exception e)
            {
                return this.Direct(false, e.Message);
            }
            string messages = string.Empty;
            List<FPTVendorWinnerVM> m_lstFPTVendorWinnerVM = GetFPTVendorWinnerVM(FPTID);
            if (!m_lstFPTVendorWinnerVM.Any()) return this.Direct(false, "No Vendor Winner");
            List<FPTAdditionalInfoVM> m_ListFPTAdditionalInfoVM = GetListFPTAdditionalInfoVM(FPTID, ref messages);
            List<string> m_lstfilename = new List<string>();

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
            string m_strstart = (m_dtstart == DateTime.MinValue) ? string.Empty : FormatDateReport(m_dtstart, Lang);
            string m_strend = (m_dtend == DateTime.MinValue) ? string.Empty : FormatDateReport(m_dtend, Lang);

            bool m_isManual = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "11").Any() || m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "12").Any() || m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "13").Any();

            string m_FPTScheduleStart = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "1").Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "1").FirstOrDefault().Value : string.Empty;
            string m_FPTScheduleEnd = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "2").Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "2").FirstOrDefault().Value : string.Empty;
            string m_Duration = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "3").Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "3").FirstOrDefault().Value : string.Empty;

            if (m_isManual)
            {
                m_FPTScheduleStart = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "11").Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "11").FirstOrDefault().Value : string.Empty;
                m_FPTScheduleEnd = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "12").Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "12").FirstOrDefault().Value : string.Empty;
                m_Duration = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "13").Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "13").FirstOrDefault().Value : string.Empty;
            }


            //foreach (var item in m_lstFPTVendorWinnerVM.Where(x => x.IsWinner == true))//todo: all or winner only
            foreach (var item in m_lstFPTVendorWinnerVM)//todo: all or winner only
            {
                item.FPTID = (item.FPTID == null) ? string.Empty : item.FPTID;
                item.BudgetPlanName = (item.BudgetPlanName == null) ? string.Empty : item.BudgetPlanName;
                item.VendorName = (item.VendorName == null) ? string.Empty : item.VendorName;

                decimal m_Bidafterfee = item.BidValue * (1 + (item.BidFee / 100));
                m_Bidafterfee = Math.Round(m_Bidafterfee);
                DocX m_documenttemp = m_document.Copy();
                string m_rnd = "Berita-acara-negosiasi " + item.FPTID + " " + item.BudgetPlanName + " " + item.VendorName;
                //m_documenttemp.ReplaceText("[budgetplan]", item.BudgetPlanName);
                m_documenttemp.ReplaceText("[budgetplan]", item.FPTDescriptions);
                m_documenttemp.ReplaceText("[negodate]", FormatDateReport((m_dtnego), Lang));
                m_documenttemp.ReplaceText("[negoplace]", "...");
                //m_documenttemp.ReplaceText("[participan]", string.Join(", ", m_lstFPTVendorWinnerVM.Where(x => x.BudgetPlanID == item.BudgetPlanID).Select(y => y.VendorName).ToArray()));
                m_documenttemp.ReplaceText("[vendorwinner]", item.VendorName);
                m_documenttemp.ReplaceText("[fptid]", FPTID);
                string m_strBid = (Lang == "ID") ? Global.GetTerbilang((long)m_Bidafterfee) : Global.GetTerbilangEN(m_Bidafterfee);
                m_strBid += " Rupiah";
                m_documenttemp.ReplaceText("[bidvalue]", $"{((long)m_Bidafterfee).ToString(Global.IntegerNumberFormat)} ({m_strBid})");
                string m_strto = Lang == "ID" ? "Sampai" : "To";
                if (m_isManual)
                {
                    if (m_FPTScheduleStart == m_FPTScheduleEnd)
                    {
                        m_documenttemp.ReplaceText("[exequtionschedule]", $"{m_FPTScheduleStart}");
                    }
                    else
                    {
                        m_documenttemp.ReplaceText("[exequtionschedule]", $"{m_FPTScheduleStart} {m_strto} {m_FPTScheduleEnd}");
                    }

                }
                else
                {
                    m_documenttemp.ReplaceText("[exequtionschedule]", $"{m_strstart} {m_strto} {m_strend}");
                }

                m_documenttemp.ReplaceText("[duration]", m_Duration);
                m_documenttemp.ReplaceText("[maintenance]", m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "4").Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "4").FirstOrDefault().Value : string.Empty);
                m_documenttemp.ReplaceText("[warranty]", m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "5").Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "5").FirstOrDefault().Value : string.Empty);
                m_documenttemp.ReplaceText("[contracttype]", m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "6").Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "6").FirstOrDefault().Value : string.Empty);
                m_documenttemp.ReplaceText("[paymenmethod]", m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "7").Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "7").FirstOrDefault().Value : string.Empty);
                m_documenttemp.ReplaceText("[Retention]", "...");

                //list
                DocX m_doclist = DocX.Create("Temp");
                Paragraph p = m_doclist.InsertParagraph();
                var l = m_doclist.AddList(listType: ListItemType.Numbered, startNumber: 1, continueNumbering: false);

                //foreach (var m_TCBU in m_lstTCBU)
                //{
                //    string m_strlst = m_TCBU.BusinessUnitDesc + " : " + m_TCBU.EmployeeName;
                //    m_doclist.AddListItem(l, m_strlst, 0, ListItemType.Numbered);
                //}

                foreach (var m_TCBU in m_lstTCBU.OrderByDescending(x => x.OrderBussinesUnit).ToList())
                {
                    string m_strlst = "Divisi " + m_TCBU.BusinessUnitDesc + " : ";
                    m_strlst += string.Join(", ", m_lsFPTTCParticipantsVM.Where(x => x.BusinessUnitID == m_TCBU.BusinessUnitIDTC).Select(y => y.StatusID ? $"{y.EmployeeName} (Hadir)" : $"{y.EmployeeName} (Tidak Hadir)").ToArray());
                    m_doclist.AddListItem(l, m_strlst, 0, ListItemType.Numbered);
                }


                m_doclist.InsertList(l);
                foreach (var paragraph in m_documenttemp.Paragraphs)
                {
                    foreach (var listParagraph in m_doclist.Paragraphs.Reverse())
                    {
                        //paragraph.InsertParagraphAfterSelf(listParagraph);
                        paragraph.FindAll("[participan]").ForEach(index => paragraph.InsertParagraphBeforeSelf(listParagraph));
                    }
                }

                //Remove tag
                try
                {
                    m_documenttemp.ReplaceText("[participan]", "");
                }
                catch (Exception e)
                {

                }
                m_doclist?.Dispose();

                m_documenttemp.SaveAs(Server.MapPath("~/Temp/" + m_rnd + ".docx"));
                m_lstfilename.Add(m_rnd);

                //Document document = new Document();
                //var m_msword = Server.MapPath("~/Temp/" + m_rnd + ".docx");
                //document.LoadFromFile(m_msword, FileFormat.Docx2013);
                //MemoryStream m_mspdf = new MemoryStream();
                //document.SaveToStream(m_mspdf, FileFormat.PDF);
                //return File(m_mspdf.ToArray(), "application/pdf", FPTID + " Berita Acara Negosisasi.pdf");
            }

            using (var memoryStream = new MemoryStream())
            {
                using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var item in m_lstfilename)
                    {
                        ziparchive.CreateEntryFromFile(Server.MapPath("~/Temp/" + item + ".docx"), item + ".docx");
                    }
                }
                foreach (var item in m_lstfilename)
                {
                    System.IO.File.Delete(Server.MapPath("~/Temp/" + item + ".docx"));
                }
                return File(memoryStream.ToArray(), "application/zip", FPTID + " Berita Acara Negosisasi.zip");
            }



        }
        private ActionResult ReportRecommendation(string FPTID, string FPTDesc, string Lang)
        {
            //todo: check fpt status
            string message = string.Empty;
            List<FPTStatusVM> m_ListFPTStatusVM = GetListFPTStatusVM(FPTID, ref message);
            List<FPTVendorRecommendationsVM> m_lstFPTVendorRecommendationsVM = getFPTVendorRecommendationsVM(FPTID);
            List<FPTTCParticipantsVM> m_lsFPTTCParticipantsVM = GetListNegoTCParticipant(FPTID, ref message);
            string m_lnum = m_lstFPTVendorRecommendationsVM.Any() ? getFPTVendorRecommendationsVM(FPTID).FirstOrDefault().LetterNumber : "";
            m_lnum = m_lnum.PadLeft(4, '0');
            if (!m_ListFPTStatusVM.Any(x => x.StatusID == (int)FPTStatusTypes.DoneNegotiation))
            {
                return this.Direct(false, "Negotiation not done yet!");
            }

            if (!Directory.Exists(Server.MapPath(@"~/Temp/"))) Directory.CreateDirectory(Server.MapPath(@"~/Temp/"));
            string m_filepath = Server.MapPath($"~/Content/Template/Report/recommendation-{Lang}.docx");
            DocX m_document = null;
            List<string> m_lstfilename = new List<string>();
            try
            {
                m_document = DocX.Load(m_filepath);
            }
            catch (Exception e)
            {
                return this.Direct(false, e.Message);
            }

            List<FPTVendorWinnerVM> m_lstFPTVendorWinnerVM = GetFPTVendorWinnerVM(FPTID);
            List<NegotiationConfigurationsVM> m_lstNegotiationConfigurationsVM = GetNegotiationConfigurationsVM(FPTID);
            List<NegotiationBidStructuresVM> m_lstNegotiationBidStructuresVM = GetNegotiationBidStructuresVM(FPTID);
            List<NegotiationBidEntryVM> m_lstNegotiationBidEntryVM = GetNegotiationBidEntryVM(FPTID);
            List<FPTAdditionalInfoVM> m_ListFPTAdditionalInfoVM = GetListFPTAdditionalInfoVM(FPTID, ref message);

            var m_lstTCBU = m_lsFPTTCParticipantsVM.Where(d => d.StatusID).Select(x => new { BusinessUnitIDTC = x.BusinessUnitID, BusinessUnitDesc = x.BusinessUnitDesc, OrderBussinesUnit = OrderingBU(x.BusinessUnitID) }).Distinct().ToList();


            List<FPTVendorParticipantsVM> m_lstFPTVendorParticipantsVM = GetListFPTVendorParticipantsVM(FPTID, ref message);
            foreach (var item in m_lstFPTVendorParticipantsVM)
            {
                item.BidFee = m_lstNegotiationBidEntryVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID && x.Sequence == 8888 && x.RoundID == item.RoundID).Any() ? m_lstNegotiationBidEntryVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID && x.Sequence == 8888 && x.RoundID == item.RoundID).OrderByDescending(x => x.RoundID).FirstOrDefault().BidValue : 0;
                item.BidAfterFee = item.BidValue * (1 + (item.BidFee / 100));
                item.BidAfterFee = Math.Round(item.BidAfterFee);
            }


            foreach (var item in m_lstNegotiationConfigurationsVM.Where(x => x.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.BudgetPlan)))
            {
                string m_strnegodate = m_lstNegotiationConfigurationsVM.Where(x => x.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.Schedule)).FirstOrDefault().ParameterValue;
                DateTime m_datenego = DateTime.MinValue;
                string[] m_hari = { "Minggu", "Senin", "Selasa", "Rabu", "Kamis", "Jumat", "Sabtu" };
                try
                {
                    m_datenego = Convert.ToDateTime(m_strnegodate);
                    m_strnegodate = FormatDateReport(m_datenego, Lang);
                }
                catch (Exception)
                {
                    m_strnegodate = "";

                }
                DocX m_documenttemp = m_document.Copy();
                string m_rnd = "Rekomendasi " + FPTID + " " + item.ParameterValue;
                decimal m_decRabValue = m_lstNegotiationBidStructuresVM.Where(x => x.NegotiationConfigID == item.NegotiationConfigID && x.Sequence == 9999).FirstOrDefault().BudgetPlanDefaultValue;
                string m_letterno = $"{m_lnum}/{ToRoman(((DateTime)m_lstFPTVendorWinnerVM.FirstOrDefault(x => x.IsWinner == true).ModifiedDate).Month)}/{((DateTime)m_lstFPTVendorWinnerVM.FirstOrDefault(x => x.IsWinner == true).ModifiedDate).ToString("yy")}";
                m_documenttemp.ReplaceText("[recommendationno]", m_letterno);
                m_documenttemp.ReplaceText("[recommendationtopic]", FPTDesc);
                m_documenttemp.ReplaceText("[dayname]", m_hari[(int)m_datenego.DayOfWeek]);
                string m_strtype = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "10").Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "10").FirstOrDefault().Value : "";
                string m_typetender = "TENDER";
                if (m_lstFPTVendorWinnerVM.FirstOrDefault().IsSync)
                {
                    m_typetender = FPTID.Split('-')[2].ToString();
                }
                m_typetender = GetTypeTender(m_typetender);

                m_documenttemp.ReplaceText("[workingtype]", m_typetender);
                m_documenttemp.ReplaceText("[negotiationdate]", m_strnegodate);
                //m_documenttemp.ReplaceText("[tcmemberlist]", string.Join(", ", m_lstNegotiationConfigurationsVM.Where(x => x.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.TCMember)).Select(y => y.EmployeeName).ToArray()));
                //m_documenttemp.ReplaceText("[recommendationlist]", "...");
                string m_strrab = (Lang == "ID") ? Global.GetTerbilang((long)m_decRabValue) : Global.GetTerbilangEN(m_decRabValue);
                //m_documenttemp.ReplaceText("[rabvalue]", $"{m_decRabValue.ToString(Global.DefaultNumberFormat)} ({m_strrab})");
                string m_strrabvalue = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "8").Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "8").FirstOrDefault().Value : "";
                if (!string.IsNullOrEmpty(m_strrabvalue))
                {
                    decimal m_decval = 0;
                    decimal.TryParse(m_strrabvalue, out m_decval);
                    m_strrabvalue = m_decval.ToString(Global.IntegerNumberFormat);
                }
                m_documenttemp.ReplaceText("[rabvalue]", m_strrabvalue);
                string m_winner = m_lstFPTVendorWinnerVM.Where(x => x.IsWinner == true && x.BudgetPlanID == item.BudgetPlanID).Any() ? string.Join(", ", GetFPTVendorWinnerVM(FPTID).Where(x => x.IsWinner == true && x.BudgetPlanID == item.BudgetPlanID).Select(y => y.VendorName).ToList()) : "";
                m_documenttemp.ReplaceText("[vendorwinner]", m_winner);
                m_documenttemp.ReplaceText("[memo]", "-");//todo:ett
                string m_reason = m_ListFPTStatusVM.Where(x => x.StatusID == (int)FPTStatusTypes.ReNegotiation).Any() ? m_ListFPTStatusVM.Where(x => x.StatusID == (int)FPTStatusTypes.ReNegotiation).FirstOrDefault().Remarks : "-";
                m_documenttemp.ReplaceText("[reason]", m_reason);

                //list
                DocX m_doclist = DocX.Create("Temp");
                Paragraph p = m_doclist.InsertParagraph();
                var l = m_doclist.AddList(listType: ListItemType.Numbered, startNumber: 1, continueNumbering: false);

                foreach (var m_TCBU in m_lstTCBU.OrderByDescending(x => x.OrderBussinesUnit).ToList())
                {
                    string m_strlst = "Divisi " + m_TCBU.BusinessUnitDesc + " : ";
                    m_strlst += string.Join(", ", m_lsFPTTCParticipantsVM.Where(x => x.BusinessUnitID == m_TCBU.BusinessUnitIDTC).Select(y => y.StatusID ? $"{y.EmployeeName} (Hadir)" : $"{y.EmployeeName} (Tidak Hadir)").ToArray());
                    m_doclist.AddListItem(l, m_strlst, 0, ListItemType.Numbered);
                }
                m_doclist.InsertList(l, new System.Drawing.FontFamily("Arial"), 11);
                foreach (var paragraph in m_documenttemp.Paragraphs)
                {
                    foreach (var listParagraph in m_doclist.Paragraphs.Reverse())
                    {
                        //paragraph.InsertParagraphAfterSelf(listParagraph);
                        paragraph.FindAll("[tcmemberlist]").ForEach(index => paragraph.InsertParagraphBeforeSelf(listParagraph));
                    }
                }
                try
                {
                    m_documenttemp.ReplaceText("[tcmemberlist]", "");
                }
                catch (Exception e)
                {

                }


                m_doclist?.Dispose();

                //table
                var t = m_documenttemp.AddTable(m_lstFPTVendorParticipantsVM.Where(x => x.BudgetPlanID == item.BudgetPlanID).Count() + 1, 3);
                t.Alignment = Novacode.Alignment.left;
                t.Design = TableDesign.None;
                int m_c = 0;
                t.Rows[m_c].Cells[0].Paragraphs.First().Append("");
                t.Rows[m_c].Cells[1].Paragraphs.First().Append("Vendor").Font(new System.Drawing.FontFamily("Arial")).FontSize(11).Bold();
                t.Rows[m_c].Cells[2].Paragraphs.First().Append("Bid").Font(new System.Drawing.FontFamily("Arial")).FontSize(11).Bold();
                m_c++;
                foreach (var m_FPTVendorParticipantsVM in m_lstFPTVendorParticipantsVM.Where(x => x.BudgetPlanID == item.BudgetPlanID))
                {
                    t.Rows[m_c].Cells[0].Paragraphs.First().Append(m_c.ToString()).Font(new System.Drawing.FontFamily("Arial")).FontSize(11).Bold(); ;
                    t.Rows[m_c].Cells[1].Paragraphs.First().Append(m_FPTVendorParticipantsVM.VendorName).Font(new System.Drawing.FontFamily("Arial")).FontSize(11).Bold(); ;
                    t.Rows[m_c].Cells[2].Paragraphs.First().Append(((long)m_FPTVendorParticipantsVM.BidAfterFee).ToString(Global.IntegerNumberFormat)).Font(new System.Drawing.FontFamily("Arial")).FontSize(11).Bold(); ;
                    t.SetColumnWidth(0, 500);
                    t.SetColumnWidth(1, 5000);
                    t.SetColumnWidth(2, 5000);
                    m_c++;
                }

                foreach (var paragraph in m_documenttemp.Paragraphs)
                {
                    paragraph.FindAll("[recommendationlist]").ForEach(index => paragraph.InsertTableAfterSelf((t)));
                }
                m_documenttemp.ReplaceText("[recommendationlist]", "");

                m_documenttemp.SaveAs(Server.MapPath("~/Temp/" + m_rnd + ".docx"));
                m_lstfilename.Add(m_rnd);
            }
            using (var memoryStream = new MemoryStream())
            {
                using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var item in m_lstfilename)
                    {
                        ziparchive.CreateEntryFromFile(Server.MapPath("~/Temp/" + item + ".docx"), item + ".docx");
                    }
                }
                foreach (var item in m_lstfilename)
                {
                    System.IO.File.Delete(Server.MapPath("~/Temp/" + item + ".docx"));
                }
                return File(memoryStream.ToArray(), "application/zip", FPTID + " Report recommendation.zip");
            }
        }
        private ActionResult ReportSPTender(string FPTID, int type, string Lang)
        {
            string message = string.Empty;
            List<FPTStatusVM> m_ListFPTStatusVM = GetListFPTStatusVM(FPTID, ref message);
            if (!m_ListFPTStatusVM.Any(x => x.StatusID == (int)FPTStatusTypes.SubmitVendorWinner))
            {
                return this.Direct(false, "Negotiation not done yet!");
            }

            if (!Directory.Exists(Server.MapPath(@"~/Temp/"))) Directory.CreateDirectory(Server.MapPath(@"~/Temp/"));
            List<FPTVendorWinnerVM> m_lstFPTVendorWinnerVM = GetFPTVendorWinnerVM(FPTID);
            List<FPTAdditionalInfoVM> m_ListFPTAdditionalInfoVM = GetListFPTAdditionalInfoVM(FPTID, ref message);
            List<NegotiationConfigurationsVM> m_lstNegotiationConfigurationsVM = GetNegotiationConfigurationsVM(FPTID);
            List<TCMembersVM> m_lsttcmember = GetTCMemberLv(ref message);
            var m_lstTCBU = m_lstNegotiationConfigurationsVM.Where(x => x.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.TCMember)).Select(x => new { BusinessUnitIDTC = x.BusinessUnitIDTC, BusinessUnitDesc = x.BusinessUnitDesc, OrderBussinesUnit = OrderingBU(x.BusinessUnitIDTC) }).Distinct().ToList();

            string m_filepath = Server.MapPath($"~/Content/Template/Report/sp-new-tender-pemasangan-{Lang}.docx");
            if (type == 1) m_filepath = Server.MapPath($"~/Content/Template/Report/sp-new-tender-pemasangan-{Lang}.docx");// 1= tender pemasangan
            if (type == 2) m_filepath = Server.MapPath($"~/Content/Template/Report/sp-new-tender-pengadaan-{Lang}.docx");// 2= tender pengadaan
            if (type == 3) m_filepath = Server.MapPath($"~/Content/Template/Report/sp-nsc-pemasangan-{Lang}.docx");// 3= nsc pemasangan
            if (type == 4) m_filepath = Server.MapPath($"~/Content/Template/Report/sp-nsc-pengadaan-{Lang}.docx");// 4= nsc pengadaan

            DocX m_document = null;

            List<string> m_lstfilename = new List<string>();
            try
            {
                m_document = DocX.Load(m_filepath);
            }
            catch (Exception e)
            {
                return this.Direct(false, e.Message);
            }

            foreach (var item in m_lstFPTVendorWinnerVM.Where(x => x.IsWinner == true))
            {
                decimal m_Bidafterfee = item.BidValue * (1 + (item.BidFee / 100));
                m_Bidafterfee = Math.Round(m_Bidafterfee);
                string m_typetender = "TENDER";
                if (item.IsSync)
                {
                    m_typetender = item.FPTID.Split('-')[2].ToString();
                }

                item.FPTID = (item.FPTID == null) ? string.Empty : item.FPTID;
                item.BudgetPlanName = (item.BudgetPlanName == null) ? string.Empty : item.BudgetPlanName;
                item.VendorName = (item.VendorName == null) ? string.Empty : item.VendorName;
                item.VendorAddress = (item.VendorAddress == null) ? string.Empty : item.VendorAddress;
                item.VendorEmail = (item.VendorEmail == null) ? string.Empty : item.VendorEmail;
                item.VendorPhone = (item.VendorPhone == null) ? string.Empty : item.VendorPhone;
                item.FPTDescriptions = (item.FPTDescriptions == null) ? string.Empty : item.FPTDescriptions;
                item.ProjectName = (item.ProjectName == null) ? string.Empty : item.ProjectName;
                item.CompanyDesc = (item.CompanyDesc == null) ? string.Empty : item.CompanyDesc;
                if (m_ListFPTAdditionalInfoVM.Any(x => x.FPTAdditionalInfoItemID == "9"))
                {
                    item.CompanyDesc = GetCompanyName(m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "9").FirstOrDefault().Value);
                }

                DocX m_documenttemp = m_document.Copy();
                string m_letterno = (item.ModifiedDate == null) ? "..." : $"{item.LetterNumber.ToString()}/TRM-{item.BusinessUnitID}-{item.DivisionID}/{m_typetender}/{ToRoman(((DateTime)item.ModifiedDate).Month)}/{((DateTime)item.ModifiedDate).ToString("yy")}";
                string m_rnd = "SP-tender " + item.FPTID + " " + item.BudgetPlanName + " " + item.VendorName;
                m_documenttemp.ReplaceText("[date]", FormatDateReport(DateTime.Now, Lang));
                m_documenttemp.ReplaceText("[letterno]", m_letterno);
                m_documenttemp.ReplaceText("[vendorname]", item.VendorName);
                m_documenttemp.ReplaceText("[vendoraddress]", item.VendorAddress);
                m_documenttemp.ReplaceText("[vendorup]", "...");
                m_documenttemp.ReplaceText("[vendoremail]", item.VendorEmail);
                m_documenttemp.ReplaceText("[vendortelp]", item.VendorPhone);
                m_documenttemp.ReplaceText("[vendorfax]", "...");
                string m_strBid = (Lang == "ID") ? Global.GetTerbilang((long)m_Bidafterfee) : Global.GetTerbilangEN(m_Bidafterfee);
                m_strBid += " Rupiah";
                m_documenttemp.ReplaceText("[bidvalue]", $"{((long)m_Bidafterfee).ToString(Global.IntegerNumberFormat)} ({m_strBid})");
                m_documenttemp.ReplaceText("[trm]", "...");
                //m_documenttemp.ReplaceText("[budgetplan]", item.BudgetPlanName);
                m_documenttemp.ReplaceText("[budgetplan]", item.FPTDescriptions);
                m_documenttemp.ReplaceText("[project]", item.ProjectName);
                bool m_boolcontainpt = item.CompanyDesc.ToLower().Contains("pt.") || item.CompanyDesc.ToLower().Contains("pt ");
                item.CompanyDesc = (m_boolcontainpt) ? item.CompanyDesc : "PT. " + item.CompanyDesc;
                m_documenttemp.ReplaceText("[company]", item.CompanyDesc);
                //m_documenttemp.ReplaceText("[cc]", "..");

                //list
                DocX m_doclist = DocX.Create("Temp");
                Paragraph p = m_doclist.InsertParagraph();
                var l = m_doclist.AddList(listType: ListItemType.Bulleted);

                foreach (var m_TCBU in m_lstTCBU.Where(y => y.BusinessUnitIDTC != "TRM").OrderByDescending(x => x.OrderBussinesUnit).ToList())
                {
                    if (m_lsttcmember.Any(x => x.BusinessUnitID == m_TCBU.BusinessUnitIDTC))
                    {
                        string m_strlst = string.Join(", ", m_lsttcmember.Where(x => x.BusinessUnitID == m_TCBU.BusinessUnitIDTC).Select(y => y.EmployeeName).ToArray());
                        m_strlst += " : Divisi " + m_TCBU.BusinessUnitDesc;
                        m_doclist.AddListItem(l, m_strlst, 0, ListItemType.Bulleted);
                    }

                }

                m_doclist.InsertList(l, 10);
                foreach (var paragraph in m_documenttemp.Paragraphs)
                {

                    foreach (var listParagraph in m_doclist.Paragraphs.Reverse())
                    {
                        listParagraph.SpacingAfter(0.25);
                        listParagraph.SpacingBefore(0.25);
                        listParagraph.FontSize(8);
                        listParagraph.Font(new System.Drawing.FontFamily("Arial"));
                        paragraph.FindAll("[cc]").ForEach(index => paragraph.InsertParagraphBeforeSelf(listParagraph));
                    }
                }

                try
                {
                    m_documenttemp.ReplaceText("[cc]", "");
                }
                catch (Exception e)
                {

                }


                m_doclist?.Dispose();

                m_documenttemp.SaveAs(Server.MapPath("~/Temp/" + m_rnd + ".docx"));
                m_lstfilename.Add(m_rnd);
            }

            using (var memoryStream = new MemoryStream())
            {
                using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var item in m_lstfilename)
                    {
                        ziparchive.CreateEntryFromFile(Server.MapPath("~/Temp/" + item + ".docx"), item + ".docx");
                    }
                }
                foreach (var item in m_lstfilename)
                {
                    System.IO.File.Delete(Server.MapPath("~/Temp/" + item + ".docx"));
                }
                return File(memoryStream.ToArray(), "application/zip", FPTID + " Report Penunjukan Tender.zip");
            }
        }
        private ActionResult ReportThankYouLetter(string FPTID, string Lang)
        {
            //todo: check fpt status
            string message = string.Empty;
            List<FPTStatusVM> m_ListFPTStatusVM = GetListFPTStatusVM(FPTID, ref message);
            if (!m_ListFPTStatusVM.Any(x => x.StatusID == (int)FPTStatusTypes.SubmitVendorWinner))
            {
                return this.Direct(false, "Negotiation not done yet!");
            }
            if (!Directory.Exists(Server.MapPath(@"~/Temp/"))) Directory.CreateDirectory(Server.MapPath(@"~/Temp/"));
            List<FPTVendorWinnerVM> m_lstFPTVendorWinnerVM = GetFPTVendorWinnerVM(FPTID);
            List<FPTAdditionalInfoVM> m_ListFPTAdditionalInfoVM = GetListFPTAdditionalInfoVM(FPTID, ref message);
            List<NegotiationConfigurationsVM> m_lstNegotiationConfigurationsVM = GetNegotiationConfigurationsVM(FPTID);
            List<TCMembersVM> m_lsttcmember = GetTCMemberLv(ref message);
            var m_lstTCBU = m_lstNegotiationConfigurationsVM.Where(x => x.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.TCMember)).Select(x => new { BusinessUnitIDTC = x.BusinessUnitIDTC, BusinessUnitDesc = x.BusinessUnitDesc, OrderBussinesUnit = OrderingBU(x.BusinessUnitIDTC) }).Distinct().ToList();


            string m_filepath = Server.MapPath($"~/Content/Template/Report/thank-you-letter-{Lang}.docx");
            DocX m_document = null;


            List<string> m_lstfilename = new List<string>();
            try
            {
                m_document = DocX.Load(m_filepath);
            }
            catch (Exception e)
            {
                return this.Direct(false, e.Message);
            }

            foreach (var item in m_lstFPTVendorWinnerVM.Where(x => x.IsWinner == false))
            {
                if (m_ListFPTAdditionalInfoVM.Any(x => x.FPTAdditionalInfoItemID == "9"))
                {
                    item.CompanyDesc = GetCompanyName(m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "9").FirstOrDefault().Value);
                }
                DocX m_documenttemp = m_document.Copy();
                string m_rnd = "FPTPPT " + item.FPTID + " " + item.BudgetPlanName + " " + item.VendorName;
                string m_letterno = (item.ModifiedDate == null) ? "..." : $"{item.LetterNumber.ToString()}/TRM-{item.BusinessUnitID}-{item.DivisionID}/TENDER/{ToRoman(((DateTime)item.ModifiedDate).Month)}/{((DateTime)item.ModifiedDate).ToString("yy")}";
                m_documenttemp.ReplaceText("[date]", FormatDateReport(DateTime.Now, Lang));
                m_documenttemp.ReplaceText("[letterno]", m_letterno);
                m_documenttemp.ReplaceText("[vendorname]", item.VendorName);
                m_documenttemp.ReplaceText("[vendoraddress]", item.VendorAddress);
                m_documenttemp.ReplaceText("[vendorup]", "...");
                m_documenttemp.ReplaceText("[vendoremail]", item.VendorEmail);
                m_documenttemp.ReplaceText("[vendortelp]", item.VendorPhone);
                m_documenttemp.ReplaceText("[vendorfax]", "...");
                m_documenttemp.ReplaceText("[vendorwinner]", string.Join(", ", m_lstFPTVendorWinnerVM.Where(x => x.IsWinner == true && x.BudgetPlanName == item.BudgetPlanName).Select(y => y.VendorName).ToArray()));//todo: use bpid
                m_documenttemp.ReplaceText("[trm]", "...");
                m_documenttemp.ReplaceText("[project]", item.ProjectName);
                //m_documenttemp.ReplaceText("[budgetplan]", item.BudgetPlanName);
                m_documenttemp.ReplaceText("[budgetplan]", item.FPTDescriptions);
                bool m_boolcontainpt = item.CompanyDesc.ToLower().Contains("pt.") || item.CompanyDesc.ToLower().Contains("pt ");
                item.CompanyDesc = (m_boolcontainpt) ? item.CompanyDesc : "PT. " + item.CompanyDesc;
                m_documenttemp.ReplaceText("[company]", item.CompanyDesc);
                //m_documenttemp.ReplaceText("[cc]", "...");
                //list
                DocX m_doclist = DocX.Create("Temp");
                Paragraph p = m_doclist.InsertParagraph();
                var l = m_doclist.AddList(listType: ListItemType.Bulleted);

                foreach (var m_TCBU in m_lstTCBU.Where(y => y.BusinessUnitIDTC != "TRM").OrderByDescending(x => x.OrderBussinesUnit).ToList())
                {
                    if (m_lsttcmember.Any(x => x.BusinessUnitID == m_TCBU.BusinessUnitIDTC))
                    {
                        string m_strlst = string.Join(", ", m_lsttcmember.Where(x => x.BusinessUnitID == m_TCBU.BusinessUnitIDTC).Select(y => y.EmployeeName).ToArray());
                        m_strlst += " : Divisi " + m_TCBU.BusinessUnitDesc;
                        m_doclist.AddListItem(l, m_strlst, 0, ListItemType.Bulleted);
                    }

                }

                m_doclist.InsertList(l, 10);
                foreach (var paragraph in m_documenttemp.Paragraphs)
                {
                    foreach (var listParagraph in m_doclist.Paragraphs.Reverse())
                    {
                        listParagraph.SpacingAfter(0.25);
                        listParagraph.SpacingBefore(0.25);
                        listParagraph.FontSize(8);
                        listParagraph.Font(new System.Drawing.FontFamily("Arial"));
                        paragraph.FindAll("[cc]").ForEach(index => paragraph.InsertParagraphBeforeSelf(listParagraph));
                    }
                }

                try
                {
                    m_documenttemp.ReplaceText("[cc]", "");
                }
                catch (Exception e)
                {

                }

                m_documenttemp.SaveAs(Server.MapPath("~/Temp/" + m_rnd + ".docx"));
                m_lstfilename.Add(m_rnd);
            }

            using (var memoryStream = new MemoryStream())
            {
                using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var item in m_lstfilename)
                    {
                        ziparchive.CreateEntryFromFile(Server.MapPath("~/Temp/" + item + ".docx"), item + ".docx");
                    }
                }
                foreach (var item in m_lstfilename)
                {
                    System.IO.File.Delete(Server.MapPath("~/Temp/" + item + ".docx"));
                }
                return File(memoryStream.ToArray(), "application/zip", FPTID + " Thank You Letter.zip");
            }
        }
        private ActionResult GenerateSPNotification(string FPTID, int type, string Lang)
        {
            string message = string.Empty;
            List<string> msg = new List<string>();
            List<FPTStatusVM> m_ListFPTStatusVM = GetListFPTStatusVM(FPTID, ref message);
            if (!m_ListFPTStatusVM.Any(x => x.StatusID == (int)FPTStatusTypes.SubmitVendorWinner))
            {
                return this.Direct(false, "Negotiation not done yet!");
            }


            var m_lstconfig = GetConfig("FunctionID");
            var m_lstconfigformat = GetConfig("FormatString");

            NotificationMapVM m_NotificationMapVMSP = GetDefaultNoticationMap(m_lstconfig.Where(x => x.Key2 == "VendorWinner").FirstOrDefault().Desc1);
            List<FieldTagReferenceVM> m_lstFieldTagReferenceVM = GetListFieldTagReferenceVM(m_NotificationMapVMSP.NotificationTemplateID);
            PropertyInfo[] m_arrPInfo = (new FPTVendorWinnerVM()).GetType().GetProperties();
            m_lstFieldTagReferenceVM = m_lstFieldTagReferenceVM.Where(y => m_arrPInfo.Select(x => x.Name).ToList().Contains(y.RefIDColumn)).ToList();

            //generate SP
            var m_lstVendorWinner = GetFPTVendorWinnerVM(FPTID).Where(x => x.IsWinner == true);
            var LstNegotiationConfigurationsVM = GetListNegoConfiguration(FPTID, null, ref message);

            foreach (var item in m_lstVendorWinner)
            {
                if (LstNegotiationConfigurationsVM.Any(x => x.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.Schedule)))
                {
                    item.NegoDate = Convert.ToDateTime(LstNegotiationConfigurationsVM.FirstOrDefault(x => x.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.Schedule)).ParameterValue);
                    item.NegoTime = Convert.ToDateTime(LstNegotiationConfigurationsVM.FirstOrDefault(x => x.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.Schedule)).ParameterValue);
                }

                string m_strsubject = m_lstconfig.Where(x => x.Key2 == "VendorWinner").Any() ? m_lstconfig.Where(x => x.Key2 == "VendorWinner").FirstOrDefault().Desc2 : "SP";
                MailNotificationsVM m_MailNotificationsVM = new MailNotificationsVM();
                m_MailNotificationsVM.MailNotificationID = "";
                m_MailNotificationsVM.Importance = true;
                m_MailNotificationsVM.Subject = $"{m_strsubject} {item.BudgetPlanName}";
                m_MailNotificationsVM.Contents = "";
                m_MailNotificationsVM.StatusID = (int)NotificationStatus.Draft;
                m_MailNotificationsVM.FPTID = item.FPTID;

                              

                // Recipient
                List<RecipientsVM> m_lstm_RecipientsVM = new List<RecipientsVM>();
                var m_vendoruserid = GetUserID(true, item.VendorID).UserID;
                foreach (var vendoremail in GetVendorDefaultEmail(item.VendorID))
                {
                    RecipientsVM m_RecipientsVM = new RecipientsVM();
                    m_RecipientsVM.RecipientID = "";
                    m_RecipientsVM.RecipientDesc = item.VendorName;
                    m_RecipientsVM.MailAddress = vendoremail;
                    m_RecipientsVM.OwnerID = m_vendoruserid;
                    m_RecipientsVM.RecipientTypeID = ((int)RecipientTypes.TO).ToString();
                    m_lstm_RecipientsVM.Add(m_RecipientsVM);
                }
                m_MailNotificationsVM.RecipientsVM = m_lstm_RecipientsVM;
                m_MailNotificationsVM.NotificationMapVM = m_NotificationMapVMSP;

                //Attachment
                NotificationAttachmentVM m_TNotificationAttachments = GetSP(item, type, Lang);

                m_MailNotificationsVM.NotificationAttachmentVM = new List<NotificationAttachmentVM>();//todo
                m_MailNotificationsVM.NotificationAttachmentVM.Add(m_TNotificationAttachments);


                //Values
                PropertyInfo[] m_arrPInfoitem = item.GetType().GetProperties();
                m_MailNotificationsVM.NotificationValuesVM = new List<NotificationValuesVM>();
                foreach (var m_FieldTagReferenceVM in m_lstFieldTagReferenceVM)
                {
                    NotificationValuesVM M_NotificationValuesVMref = new NotificationValuesVM();
                    M_NotificationValuesVMref.NotificationValueID = Guid.NewGuid().ToString("N");
                    M_NotificationValuesVMref.MailNotificationID = "";
                    M_NotificationValuesVMref.FieldTagID = m_FieldTagReferenceVM.FieldTagID;

                    var m_objvalue = m_arrPInfoitem.FirstOrDefault(d => d.Name.Equals(m_FieldTagReferenceVM.RefIDColumn)).GetValue(item);
                    string m_strvalue = string.Empty;
                    string m_strformat = string.Empty;
                    string m_strculture = string.Empty;
                    m_strformat = m_lstconfigformat.Where(x => x.Key3 == m_FieldTagReferenceVM.FieldTagID).Any() ? m_lstconfigformat.Where(x => x.Key3 == m_FieldTagReferenceVM.FieldTagID).FirstOrDefault().Desc1 : string.Empty;
                    m_strculture = m_lstconfigformat.Where(x => x.Key3 == m_FieldTagReferenceVM.FieldTagID).Any() ? m_lstconfigformat.Where(x => x.Key3 == m_FieldTagReferenceVM.FieldTagID).FirstOrDefault().Desc2 : string.Empty;

                    if (Object.ReferenceEquals(m_objvalue.GetType(), typeof(DateTime)))
                    {
                        m_strvalue = m_objvalue == null ? "" : ((DateTime)m_objvalue).ToString(m_strformat, new System.Globalization.CultureInfo(m_strculture));
                    }
                    else
                    {
                        m_strvalue = m_objvalue.ToString();
                    }

                    M_NotificationValuesVMref.Value = m_strvalue;

                    m_MailNotificationsVM.NotificationValuesVM.Add(M_NotificationValuesVMref);
                }

                string MailnotifID = string.Empty;

                if (!this.CreateMailNotification(m_MailNotificationsVM, false, ref msg, ref MailnotifID)) break;
            }
            if (msg.Any())
            {
                return this.Direct(false, string.Join(",", msg));
            }
            else
            {
                Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Added));
                return this.Direct(true);
            }

        }
        private ActionResult GenerateTLNotification(string FPTID, string Lang)
        {
            string message = string.Empty;
            List<string> msg = new List<string>();
            List<FPTStatusVM> m_ListFPTStatusVM = GetListFPTStatusVM(FPTID, ref message);
            if (!m_ListFPTStatusVM.Any(x => x.StatusID == (int)FPTStatusTypes.SubmitVendorWinner))
            {
                return this.Direct(false, "Negotiation not done yet!");
            }


            var m_lstconfig = GetConfig("FunctionID");
            var m_lstconfigformat = GetConfig("FormatString");

            NotificationMapVM m_NotificationMapVMTL = GetDefaultNoticationMap(m_lstconfig.Where(x => x.Key2 == "ThankYouLetter").FirstOrDefault().Desc1);
            List<FieldTagReferenceVM> m_lstFieldTagReferenceVM = GetListFieldTagReferenceVM(m_NotificationMapVMTL.NotificationTemplateID);
            PropertyInfo[] m_arrPInfo = (new FPTVendorWinnerVM()).GetType().GetProperties();
            m_lstFieldTagReferenceVM = m_lstFieldTagReferenceVM.Where(y => m_arrPInfo.Select(x => x.Name).ToList().Contains(y.RefIDColumn)).ToList();

            //generate TL
            var m_lstVendorWinner = GetFPTVendorWinnerVM(FPTID);
            var LstNegotiationConfigurationsVM = GetListNegoConfiguration(FPTID, null, ref message);


            foreach (var item in m_lstVendorWinner.Where(x => x.IsWinner == false))
            {
                if (LstNegotiationConfigurationsVM.Any(x => x.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.Schedule)))
                {
                    item.NegoDate = Convert.ToDateTime(LstNegotiationConfigurationsVM.FirstOrDefault(x => x.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.Schedule)).ParameterValue);
                    item.NegoTime = Convert.ToDateTime(LstNegotiationConfigurationsVM.FirstOrDefault(x => x.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.Schedule)).ParameterValue);
                }
                string m_strsubject = m_lstconfig.Where(x => x.Key2 == "ThankYouLetter").Any() ? m_lstconfig.Where(x => x.Key2 == "ThankYouLetter").FirstOrDefault().Desc2 : "TL";
                MailNotificationsVM m_MailNotificationsVM = new MailNotificationsVM();
                m_MailNotificationsVM.MailNotificationID = "";
                m_MailNotificationsVM.Importance = true;
                m_MailNotificationsVM.Subject = $"{m_strsubject} {item.BudgetPlanName} ";
                m_MailNotificationsVM.Contents = "";
                m_MailNotificationsVM.StatusID = (int)NotificationStatus.Draft;
                m_MailNotificationsVM.FPTID = item.FPTID;

                // Recipient
                List<RecipientsVM> m_lstm_RecipientsVM = new List<RecipientsVM>();
                var m_vendoruserid = GetUserID(true, item.VendorID).UserID;
                foreach (var vendoremail in GetVendorDefaultEmail(item.VendorID))
                {
                    RecipientsVM m_RecipientsVM = new RecipientsVM();
                    m_RecipientsVM.RecipientID = "";
                    m_RecipientsVM.RecipientDesc = item.VendorName;
                    m_RecipientsVM.MailAddress = vendoremail;
                    m_RecipientsVM.OwnerID = m_vendoruserid;
                    m_RecipientsVM.RecipientTypeID = ((int)RecipientTypes.TO).ToString();
                    m_lstm_RecipientsVM.Add(m_RecipientsVM);
                }
                m_MailNotificationsVM.RecipientsVM = m_lstm_RecipientsVM;             
                m_MailNotificationsVM.NotificationMapVM = m_NotificationMapVMTL;

                //Attachment
                NotificationAttachmentVM m_TNotificationAttachments = GetTL(item, string.Join(", ", m_lstVendorWinner.Where(x => x.IsWinner == true && x.BudgetPlanName == item.BudgetPlanName).Select(y => y.VendorName).ToArray()), Lang);
                m_MailNotificationsVM.NotificationAttachmentVM = new List<NotificationAttachmentVM>();//todo
                m_MailNotificationsVM.NotificationAttachmentVM.Add(m_TNotificationAttachments);

                //Values
                PropertyInfo[] m_arrPInfoitem = item.GetType().GetProperties();
                m_MailNotificationsVM.NotificationValuesVM = new List<NotificationValuesVM>();
                foreach (var m_FieldTagReferenceVM in m_lstFieldTagReferenceVM)
                {
                    NotificationValuesVM M_NotificationValuesVMref = new NotificationValuesVM();
                    M_NotificationValuesVMref.NotificationValueID = Guid.NewGuid().ToString("N");
                    M_NotificationValuesVMref.MailNotificationID = "";
                    M_NotificationValuesVMref.FieldTagID = m_FieldTagReferenceVM.FieldTagID;

                    var m_objvalue = m_arrPInfoitem.FirstOrDefault(d => d.Name.Equals(m_FieldTagReferenceVM.RefIDColumn)).GetValue(item);
                    string m_strvalue = string.Empty;
                    string m_strformat = string.Empty;
                    string m_strculture = string.Empty;
                    m_strformat = m_lstconfigformat.Where(x => x.Key3 == m_FieldTagReferenceVM.FieldTagID).Any() ? m_lstconfigformat.Where(x => x.Key3 == m_FieldTagReferenceVM.FieldTagID).FirstOrDefault().Desc1 : string.Empty;
                    m_strculture = m_lstconfigformat.Where(x => x.Key3 == m_FieldTagReferenceVM.FieldTagID).Any() ? m_lstconfigformat.Where(x => x.Key3 == m_FieldTagReferenceVM.FieldTagID).FirstOrDefault().Desc2 : string.Empty;

                    if (Object.ReferenceEquals(m_objvalue.GetType(), typeof(DateTime)))
                    {
                        m_strvalue = m_objvalue == null ? "" : ((DateTime)m_objvalue).ToString(m_strformat, new System.Globalization.CultureInfo(m_strculture));
                    }
                    else
                    {
                        m_strvalue = m_objvalue.ToString();
                    }

                    M_NotificationValuesVMref.Value = m_strvalue;
                    m_MailNotificationsVM.NotificationValuesVM.Add(M_NotificationValuesVMref);
                }

                string MailnotifID = string.Empty;
                if (!this.CreateMailNotification(m_MailNotificationsVM, false, ref msg, ref MailnotifID)) break;
            }

            if (msg.Any())
            {
                return this.Direct(false, string.Join(",", msg));
            }
            else
            {
                Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Added));
                return this.Direct(true);
            }



        }
       
        #endregion

        #region private method
        /// <summary>
        /// Todo : Create Master data?
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetTypeTender(string type)
        {
            string m_retval = type;
            Dictionary<string, string> m_listtype = new Dictionary<string, string>();
            m_listtype.Add("3CO", "3 Comparison Outsourcing");
            m_listtype.Add("ADNHR", "Addendum (High Rise)");
            m_listtype.Add("ADN", "Addendum (Reguler)");
            m_listtype.Add("DACP", "Direct Appointment (Purchasing)");
            m_listtype.Add("DN", "Direct Negotiation");
            m_listtype.Add("DNCP", "Direct Negotiation (Purchasing)");
            m_listtype.Add("EMG", "Emergency");
            m_listtype.Add("KMOU", "Konfirmasi MOU (Purchasing)");
            m_listtype.Add("KDN", "Konsultan - Direct Nego");
            m_listtype.Add("KT", "Konsultan - Tender");
            m_listtype.Add("MOU", "MOU");
            m_listtype.Add("XMOU", "MOU (Non Jabodetabek Area)");
            m_listtype.Add("MOUCP", "MOU (Purchasing)");
            m_listtype.Add("NMOU", "New MOU (Project Work)");
            m_listtype.Add("RO", "Repeat Order");
            m_listtype.Add("TR", "Tender Regular");
            m_listtype.Add("TRCP", "Tender Regular (Purchasing)");
            m_listtype.Add("TS", "Tender Special");
            m_retval = m_listtype.Any(x => x.Key == type) ? m_listtype.Where(x => x.Key == type).FirstOrDefault().Value : type;
            return m_retval;
        }

        private List<FPTVendorRecommendationsVM> getFPTVendorRecommendationsVM(string FPTID)
        {
            List<FPTVendorRecommendationsVM> m_lstFPTVendorRecommendationsVM = new List<FPTVendorRecommendationsVM>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();
            DFPTVendorRecommendationsDA m_objDFPTVendorRecommendationsDA = new DFPTVendorRecommendationsDA();
            m_objDFPTVendorRecommendationsDA.ConnectionStringName = Global.ConnStrConfigName;

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(FPTID);
            m_objFilter.Add(FPTVendorRecommendationsVM.Prop.FPTID.Map, m_lstFilter);

            m_lstSelect = new List<string>();
            m_lstSelect.Add(FPTVendorRecommendationsVM.Prop.VendorRecommendationID.MapAlias);
            m_lstSelect.Add(FPTVendorRecommendationsVM.Prop.FPTID.MapAlias);
            m_lstSelect.Add(FPTVendorRecommendationsVM.Prop.TaskID.MapAlias);
            m_lstSelect.Add(FPTVendorRecommendationsVM.Prop.TCMemberID.MapAlias);
            m_lstSelect.Add(FPTVendorRecommendationsVM.Prop.FPTVendorParticipantID.MapAlias);
            m_lstSelect.Add(FPTVendorRecommendationsVM.Prop.IsProposed.MapAlias);
            m_lstSelect.Add(FPTVendorRecommendationsVM.Prop.IsWinner.MapAlias);
            m_lstSelect.Add(FPTVendorRecommendationsVM.Prop.Remarks.MapAlias);
            m_lstSelect.Add(FPTVendorRecommendationsVM.Prop.LetterNumber.MapAlias);

            Dictionary<int, DataSet> m_dicDFPTVendorRecommendationsDA = m_objDFPTVendorRecommendationsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objDFPTVendorRecommendationsDA.Success)
            {
                foreach (DataRow item in m_dicDFPTVendorRecommendationsDA[0].Tables[0].Rows)
                {
                    FPTVendorRecommendationsVM m_objFPTVendorRecommendationsVM = new FPTVendorRecommendationsVM();
                    m_objFPTVendorRecommendationsVM.VendorRecommendationID = item[FPTVendorRecommendationsVM.Prop.VendorRecommendationID.Name].ToString();
                    m_objFPTVendorRecommendationsVM.FPTID = item[FPTVendorRecommendationsVM.Prop.FPTID.Name].ToString();
                    m_objFPTVendorRecommendationsVM.TaskID = item[FPTVendorRecommendationsVM.Prop.TaskID.Name].ToString();
                    m_objFPTVendorRecommendationsVM.TCMemberID = item[FPTVendorRecommendationsVM.Prop.TCMemberID.Name].ToString();
                    m_objFPTVendorRecommendationsVM.FPTVendorParticipantID = item[FPTVendorRecommendationsVM.Prop.FPTVendorParticipantID.Name].ToString();
                    m_objFPTVendorRecommendationsVM.IsProposed = (bool)item[FPTVendorRecommendationsVM.Prop.IsProposed.Name];
                    m_objFPTVendorRecommendationsVM.IsWinner = (bool)item[FPTVendorRecommendationsVM.Prop.IsWinner.Name];
                    m_objFPTVendorRecommendationsVM.Remarks = item[FPTVendorRecommendationsVM.Prop.Remarks.Name].ToString();
                    m_objFPTVendorRecommendationsVM.LetterNumber = item[FPTVendorRecommendationsVM.Prop.LetterNumber.Name].ToString();
                    m_lstFPTVendorRecommendationsVM.Add(m_objFPTVendorRecommendationsVM);

                }
            }
            return m_lstFPTVendorRecommendationsVM;
        }
        private List<FPTVendorWinnerVM> GetFPTVendorWinnerVM(string FPTID)
        {
            List<FPTVendorWinnerVM> m_lstFPTVendorWinnerVM = new List<FPTVendorWinnerVM>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();
            DFPTVendorWinnerDA m_objDFPTVendorWinnerDA = new DFPTVendorWinnerDA();
            m_objDFPTVendorWinnerDA.ConnectionStringName = Global.ConnStrConfigName;

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(FPTID);
            m_objFilter.Add(FPTVendorWinnerVM.Prop.FPTID.Map, m_lstFilter);

            m_lstSelect = new List<string>();
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.VendorWinnerID.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.FPTID.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.TaskID.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.FPTVendorParticipantID.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.IsWinner.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.BidValue.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.NegotiationEntryID.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.LetterNumber.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.BidFee.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.FPTDescriptions.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.IsSync.MapAlias);

            m_lstSelect.Add(FPTVendorWinnerVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.VendorName.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.VendorAddress.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.VendorPhone.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.VendorEmail.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.BudgetPlanName.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.ProjectName.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.StatusID.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.ModifiedDate.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.DivisionID.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.BusinessUnitID.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.CompanyDesc.MapAlias);

            Dictionary<int, DataSet> m_dicDFPTVendorWinnerDA = m_objDFPTVendorWinnerDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objDFPTVendorWinnerDA.Success)
            {
                foreach (DataRow item in m_dicDFPTVendorWinnerDA[0].Tables[0].Rows)
                {
                    FPTVendorWinnerVM m_objFPTVendorWinnerVM = new FPTVendorWinnerVM();
                    m_objFPTVendorWinnerVM.VendorWinnerID = item[FPTVendorWinnerVM.Prop.VendorWinnerID.Name].ToString();
                    m_objFPTVendorWinnerVM.FPTID = item[FPTVendorWinnerVM.Prop.FPTID.Name].ToString();
                    m_objFPTVendorWinnerVM.TaskID = item[FPTVendorWinnerVM.Prop.TaskID.Name].ToString();
                    m_objFPTVendorWinnerVM.FPTVendorParticipantID = item[FPTVendorWinnerVM.Prop.FPTVendorParticipantID.Name].ToString();
                    m_objFPTVendorWinnerVM.IsWinner = (bool)item[FPTVendorWinnerVM.Prop.IsWinner.Name];
                    m_objFPTVendorWinnerVM.NegotiationEntryID = item[FPTVendorWinnerVM.Prop.NegotiationEntryID.Name].ToString();
                    m_objFPTVendorWinnerVM.BidValue = (string.IsNullOrEmpty(item[FPTVendorWinnerVM.Prop.BidValue.Name].ToString())) ? 0 : (decimal)item[FPTVendorWinnerVM.Prop.BidValue.Name];
                    m_objFPTVendorWinnerVM.VendorID = item[FPTVendorWinnerVM.Prop.VendorID.Name].ToString();
                    m_objFPTVendorWinnerVM.VendorName = item[FPTVendorWinnerVM.Prop.VendorName.Name].ToString();
                    m_objFPTVendorWinnerVM.VendorAddress = item[FPTVendorWinnerVM.Prop.VendorAddress.Name].ToString();
                    m_objFPTVendorWinnerVM.BidFee = item[FPTVendorWinnerVM.Prop.BidFee.Name] == null ? 0 : Convert.ToDecimal(item[FPTVendorWinnerVM.Prop.BidFee.Name].ToString());
                    m_objFPTVendorWinnerVM.VendorPhone = item[FPTVendorWinnerVM.Prop.VendorPhone.Name].ToString();
                    m_objFPTVendorWinnerVM.VendorEmail = item[FPTVendorWinnerVM.Prop.VendorEmail.Name].ToString();
                    m_objFPTVendorWinnerVM.BudgetPlanID = item[FPTVendorWinnerVM.Prop.BudgetPlanID.Name].ToString();
                    m_objFPTVendorWinnerVM.BudgetPlanName = item[FPTVendorWinnerVM.Prop.BudgetPlanName.Name].ToString();
                    m_objFPTVendorWinnerVM.ProjectName = item[FPTVendorWinnerVM.Prop.ProjectName.Name].ToString();
                    m_objFPTVendorWinnerVM.ModifiedDate = (DateTime?)item[FPTVendorWinnerVM.Prop.ModifiedDate.Name];
                    m_objFPTVendorWinnerVM.DivisionID = item[FPTVendorWinnerVM.Prop.DivisionID.Name].ToString();
                    m_objFPTVendorWinnerVM.BusinessUnitID = item[FPTVendorWinnerVM.Prop.BusinessUnitID.Name].ToString();
                    m_objFPTVendorWinnerVM.LetterNumber = item[FPTVendorWinnerVM.Prop.LetterNumber.Name].ToString();
                    m_objFPTVendorWinnerVM.CompanyDesc = item[FPTVendorWinnerVM.Prop.CompanyDesc.Name].ToString();
                    m_objFPTVendorWinnerVM.StatusID = (string.IsNullOrEmpty(item[FPTVendorWinnerVM.Prop.StatusID.Name].ToString())) ? 4 : (int)item[FPTVendorWinnerVM.Prop.StatusID.Name];
                    m_objFPTVendorWinnerVM.FPTDescriptions = item[FPTVendorWinnerVM.Prop.FPTDescriptions.Name].ToString();
                    m_objFPTVendorWinnerVM.IsSync = (bool)item[FPTVendorWinnerVM.Prop.IsSync.Name];
                    m_lstFPTVendorWinnerVM.Add(m_objFPTVendorWinnerVM);
                }
            }

            return m_lstFPTVendorWinnerVM;
        }
        private List<NegotiationBidStructuresVM> GetNegotiationBidStructuresVM(string FPTID)
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

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add("7777, 8888, 9999");
            m_objFilter.Add(NegotiationBidStructuresVM.Prop.Sequence.Map, m_lstFilter);

            m_lstSelect = new List<string>();
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.NegotiationBidID.MapAlias);
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.NegotiationConfigID.MapAlias);
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.BudgetPlanDefaultValue.MapAlias);
            m_lstSelect.Add(NegotiationBidStructuresVM.Prop.Sequence.MapAlias);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(NegotiationBidStructuresVM.Prop.NegotiationBidID.Map, OrderDirection.Descending);
            Dictionary<int, DataSet> m_dicTNegotiationBidStructuresDA = m_objTNegotiationBidStructuresDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);

            if (m_objTNegotiationBidStructuresDA.Success)
            {
                foreach (DataRow item in m_dicTNegotiationBidStructuresDA[0].Tables[0].Rows)
                {
                    NegotiationBidStructuresVM m_objNegotiationBidStructuresVM = new NegotiationBidStructuresVM();
                    m_objNegotiationBidStructuresVM.NegotiationBidID = item[NegotiationBidStructuresVM.Prop.NegotiationBidID.Name].ToString();
                    m_objNegotiationBidStructuresVM.NegotiationConfigID = item[NegotiationBidStructuresVM.Prop.NegotiationConfigID.Name].ToString();
                    m_objNegotiationBidStructuresVM.BudgetPlanDefaultValue = (decimal)item[NegotiationBidStructuresVM.Prop.BudgetPlanDefaultValue.Name];
                    m_objNegotiationBidStructuresVM.Sequence = (int)item[NegotiationBidStructuresVM.Prop.Sequence.Name];
                    m_lstNegotiationBidStructuresVM.Add(m_objNegotiationBidStructuresVM);
                }
            }

            return m_lstNegotiationBidStructuresVM;
        }
        private List<NegotiationConfigurationsVM> GetNegotiationConfigurationsVM(string FPTID)
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

            m_lstSelect = new List<string>();
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.NegotiationConfigID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.NegotiationConfigTypeID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.TaskID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.ParameterValue.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.ParameterValue2.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.EmployeeName.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.BusinessUnitIDTC.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.BusinessUnitDesc.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.BudgetPlanDescription.MapAlias);

            Dictionary<int, DataSet> m_dicCNegotiationConfigurationsDA = m_objCNegotiationConfigurationsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objCNegotiationConfigurationsDA.Success)
            {
                foreach (DataRow item in m_dicCNegotiationConfigurationsDA[0].Tables[0].Rows)
                {
                    NegotiationConfigurationsVM m_objNegotiationConfigurationsVM = new NegotiationConfigurationsVM();
                    m_objNegotiationConfigurationsVM.NegotiationConfigID = item[NegotiationConfigurationsVM.Prop.NegotiationConfigID.Name].ToString();
                    m_objNegotiationConfigurationsVM.NegotiationConfigTypeID = item[NegotiationConfigurationsVM.Prop.NegotiationConfigTypeID.Name].ToString();
                    m_objNegotiationConfigurationsVM.TaskID = item[NegotiationConfigurationsVM.Prop.TaskID.Name].ToString();
                    m_objNegotiationConfigurationsVM.ParameterValue = item[NegotiationConfigurationsVM.Prop.ParameterValue.Name].ToString();
                    m_objNegotiationConfigurationsVM.ParameterValue2 = item[NegotiationConfigurationsVM.Prop.ParameterValue2.Name].ToString();
                    m_objNegotiationConfigurationsVM.EmployeeName = item[NegotiationConfigurationsVM.Prop.EmployeeName.Name].ToString();
                    m_objNegotiationConfigurationsVM.BudgetPlanID = item[NegotiationConfigurationsVM.Prop.BudgetPlanID.Name].ToString();
                    m_objNegotiationConfigurationsVM.BusinessUnitIDTC = item[NegotiationConfigurationsVM.Prop.BusinessUnitIDTC.Name].ToString();
                    m_objNegotiationConfigurationsVM.BusinessUnitDesc = item[NegotiationConfigurationsVM.Prop.BusinessUnitDesc.Name].ToString();
                    m_objNegotiationConfigurationsVM.BudgetPlanDescription = item[NegotiationConfigurationsVM.Prop.BudgetPlanDescription.Name].ToString();
                    m_lstNegotiationConfigurationsVM.Add(m_objNegotiationConfigurationsVM);
                }
            }
            return m_lstNegotiationConfigurationsVM;
        }
        private string GetListFPTProjectsVM(string FPTID, ref string message)
        {
            List<FPTVendorParticipantsVM> m_lstFPTVendorParticipantsVM = new List<FPTVendorParticipantsVM>();
            DFPTVendorParticipantsDA m_objDFPTVendorParticipantsDA = new DFPTVendorParticipantsDA();
            m_objDFPTVendorParticipantsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();

            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.ProjectDesc.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.FirstName.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.ProjectID.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.VendorID.MapAlias);


            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(FPTID);
            m_objFilter.Add(FPTVendorParticipantsVM.Prop.FPTID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(General.EnumDesc(NegoConfigTypes.BudgetPlan));
            m_objFilter.Add(NegotiationConfigurationsVM.Prop.NegotiationConfigTypeID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicm_objCNegotiationConfigurationsDA = m_objDFPTVendorParticipantsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDFPTVendorParticipantsDA.Success)
            {
                foreach (DataRow m_drCNegotiationConfigurationsDA in m_dicm_objCNegotiationConfigurationsDA[0].Tables[0].Rows)
                {
                    FPTVendorParticipantsVM m_objFPTVendorParticipantsVM = new FPTVendorParticipantsVM();
                    m_objFPTVendorParticipantsVM.ProjectDesc = m_drCNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.ProjectDesc.Name].ToString();
                    m_objFPTVendorParticipantsVM.BudgetPlanID = m_drCNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.BudgetPlanID.Name].ToString();
                    m_objFPTVendorParticipantsVM.FirstName = m_drCNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.FirstName.Name].ToString();
                    m_objFPTVendorParticipantsVM.ProjectID = m_drCNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.ProjectID.Name].ToString();
                    m_objFPTVendorParticipantsVM.VendorID = m_drCNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.VendorID.Name].ToString();

                    m_lstFPTVendorParticipantsVM.Add(m_objFPTVendorParticipantsVM);
                }
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDFPTVendorParticipantsDA.Message;

            string[] m_arrproject = m_lstFPTVendorParticipantsVM.OrderBy(x => x.ProjectID).ThenBy(x => x.BudgetPlanID).ThenBy(x => x.VendorID).Select(x => x.ProjectDesc).ToArray();
            List<string> m_lststr = new List<string>();
            for (int i = 0; i < m_arrproject.Length; i++)
            {
                if (m_lststr.Any(m_arrproject[i].Contains))
                {
                    m_arrproject[i] = "";
                }
                else
                {
                    m_lststr.Add(m_arrproject[i]);
                }
            }

            return string.Join("$", m_arrproject);

        }
        private List<FPTVendorParticipantsVM> GetListFPTVendorParticipantsVM(string FPTID, ref string message)
        {
            List<NegotiationBidEntryVM> m_lstNegotiationBidEntryVM = GetNegotiationBidEntryVM(FPTID);
            List<FPTVendorParticipantsVM> m_lstFPTVendorParticipantsVM = new List<FPTVendorParticipantsVM>();
            DFPTVendorParticipantsDA m_objDFPTVendorParticipantsDA = new DFPTVendorParticipantsDA();
            m_objDFPTVendorParticipantsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();

            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.FPTVendorParticipantID.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.ProjectDesc.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.FirstName.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.ProjectID.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.VendorName.MapAlias);


            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(FPTID);
            m_objFilter.Add(FPTVendorParticipantsVM.Prop.FPTID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(General.EnumDesc(NegoConfigTypes.BudgetPlan));
            m_objFilter.Add(NegotiationConfigurationsVM.Prop.NegotiationConfigTypeID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicm_objCNegotiationConfigurationsDA = m_objDFPTVendorParticipantsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDFPTVendorParticipantsDA.Success)
            {
                foreach (DataRow m_drCNegotiationConfigurationsDA in m_dicm_objCNegotiationConfigurationsDA[0].Tables[0].Rows)
                {
                    FPTVendorParticipantsVM m_objFPTVendorParticipantsVM = new FPTVendorParticipantsVM();
                    m_objFPTVendorParticipantsVM.FPTVendorParticipantID = m_drCNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.FPTVendorParticipantID.Name].ToString();
                    m_objFPTVendorParticipantsVM.ProjectDesc = m_drCNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.ProjectDesc.Name].ToString();
                    m_objFPTVendorParticipantsVM.BudgetPlanID = m_drCNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.BudgetPlanID.Name].ToString();
                    m_objFPTVendorParticipantsVM.FirstName = m_drCNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.FirstName.Name].ToString();
                    m_objFPTVendorParticipantsVM.ProjectID = m_drCNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.ProjectID.Name].ToString();
                    m_objFPTVendorParticipantsVM.VendorID = m_drCNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.VendorID.Name].ToString();
                    m_objFPTVendorParticipantsVM.VendorName = m_drCNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.VendorName.Name].ToString();
                    m_objFPTVendorParticipantsVM.RoundID = m_lstNegotiationBidEntryVM.Where(x => x.FPTVendorParticipantID == m_objFPTVendorParticipantsVM.FPTVendorParticipantID).Any() ? m_lstNegotiationBidEntryVM.Where(x => x.FPTVendorParticipantID == m_objFPTVendorParticipantsVM.FPTVendorParticipantID && x.Sequence == 7777).OrderByDescending(x => x.RoundID).FirstOrDefault().RoundID : string.Empty;
                    m_objFPTVendorParticipantsVM.BidValue = 0;
                    if (m_lstNegotiationBidEntryVM.Where(x => x.FPTVendorParticipantID == m_objFPTVendorParticipantsVM.FPTVendorParticipantID && x.RoundID == m_objFPTVendorParticipantsVM.RoundID).Any())
                    {
                        m_objFPTVendorParticipantsVM.BidValue = m_lstNegotiationBidEntryVM.Where(x => x.FPTVendorParticipantID == m_objFPTVendorParticipantsVM.FPTVendorParticipantID && x.Sequence == 7777 && x.RoundID == m_objFPTVendorParticipantsVM.RoundID).OrderByDescending(x => x.RoundID).FirstOrDefault().BidValue;
                    }
                    m_lstFPTVendorParticipantsVM.Add(m_objFPTVendorParticipantsVM);
                }
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDFPTVendorParticipantsDA.Message;
            return m_lstFPTVendorParticipantsVM;
            //return string.Join("$", m_lstFPTVendorParticipantsVM.OrderBy(x => x.ProjectID).ThenBy(x => x.BudgetPlanID).ThenBy(x => x.VendorID).Select(x => x.FirstName).ToArray());


        }
        private string GetListFPTBudgetPlanVM(string FPTID, ref string message)
        {
            List<FPTVendorParticipantsVM> m_lstFPTVendorParticipantsVM = new List<FPTVendorParticipantsVM>();
            DFPTVendorParticipantsDA m_objDFPTVendorParticipantsDA = new DFPTVendorParticipantsDA();
            m_objDFPTVendorParticipantsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();

            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.ProjectDesc.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.FirstName.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.ProjectID.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.VendorID.MapAlias);


            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(FPTID);
            m_objFilter.Add(FPTVendorParticipantsVM.Prop.FPTID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(General.EnumDesc(NegoConfigTypes.BudgetPlan));
            m_objFilter.Add(NegotiationConfigurationsVM.Prop.NegotiationConfigTypeID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicm_objCNegotiationConfigurationsDA = m_objDFPTVendorParticipantsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDFPTVendorParticipantsDA.Success)
            {
                foreach (DataRow m_drCNegotiationConfigurationsDA in m_dicm_objCNegotiationConfigurationsDA[0].Tables[0].Rows)
                {
                    FPTVendorParticipantsVM m_objFPTVendorParticipantsVM = new FPTVendorParticipantsVM();
                    m_objFPTVendorParticipantsVM.ProjectDesc = m_drCNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.ProjectDesc.Name].ToString();
                    m_objFPTVendorParticipantsVM.BudgetPlanID = m_drCNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.BudgetPlanID.Name].ToString();
                    m_objFPTVendorParticipantsVM.FirstName = m_drCNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.FirstName.Name].ToString();
                    m_objFPTVendorParticipantsVM.ProjectID = m_drCNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.ProjectID.Name].ToString();
                    m_objFPTVendorParticipantsVM.VendorID = m_drCNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.VendorID.Name].ToString();

                    m_lstFPTVendorParticipantsVM.Add(m_objFPTVendorParticipantsVM);
                }
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDFPTVendorParticipantsDA.Message;

            string[] m_arrBP = m_lstFPTVendorParticipantsVM.OrderBy(x => x.ProjectID).ThenBy(x => x.BudgetPlanID).ThenBy(x => x.VendorID).Select(x => x.BudgetPlanID).ToArray();
            List<string> m_lststr = new List<string>();
            for (int i = 0; i < m_arrBP.Length; i++)
            {
                if (m_lststr.Any(m_arrBP[i].Contains))
                {
                    m_arrBP[i] = "";
                }
                else
                {
                    m_lststr.Add(m_arrBP[i]);
                }
            }
            return string.Join("$", m_arrBP);

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
        private string GetListBFPTVendorParticipantsVM(string FPTID, ref string message)
        {
            List<FPTVendorParticipantsVM> m_lstFPTVendorParticipantsVM = new List<FPTVendorParticipantsVM>();
            DFPTVendorParticipantsDA m_objDFPTVendorParticipantsDA = new DFPTVendorParticipantsDA();
            m_objDFPTVendorParticipantsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();

            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.ProjectDesc.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.FirstName.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.ProjectID.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.VendorID.MapAlias);


            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(FPTID);
            m_objFilter.Add(FPTVendorParticipantsVM.Prop.FPTID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(General.EnumDesc(NegoConfigTypes.BudgetPlan));
            m_objFilter.Add(NegotiationConfigurationsVM.Prop.NegotiationConfigTypeID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicm_objCNegotiationConfigurationsDA = m_objDFPTVendorParticipantsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDFPTVendorParticipantsDA.Success)
            {
                foreach (DataRow m_drCNegotiationConfigurationsDA in m_dicm_objCNegotiationConfigurationsDA[0].Tables[0].Rows)
                {
                    FPTVendorParticipantsVM m_objFPTVendorParticipantsVM = new FPTVendorParticipantsVM();
                    m_objFPTVendorParticipantsVM.ProjectDesc = m_drCNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.ProjectDesc.Name].ToString();
                    m_objFPTVendorParticipantsVM.BudgetPlanID = m_drCNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.BudgetPlanID.Name].ToString();
                    m_objFPTVendorParticipantsVM.FirstName = m_drCNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.FirstName.Name].ToString();
                    m_objFPTVendorParticipantsVM.ProjectID = m_drCNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.ProjectID.Name].ToString();
                    m_objFPTVendorParticipantsVM.VendorID = m_drCNegotiationConfigurationsDA[FPTVendorParticipantsVM.Prop.VendorID.Name].ToString();

                    m_lstFPTVendorParticipantsVM.Add(m_objFPTVendorParticipantsVM);
                }
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDFPTVendorParticipantsDA.Message;

            return string.Join("$", m_lstFPTVendorParticipantsVM.OrderBy(x => x.ProjectID).ThenBy(x => x.BudgetPlanID).ThenBy(x => x.VendorID).Select(x => x.FirstName).ToArray());


        }
        private List<FPTStatusVM> GetListFPTStatusVM(string FPTID, ref string message)
        {
            List<FPTStatusVM> m_lstFPTStatusVM = new List<FPTStatusVM>();
            DFPTStatusDA m_objDFPTStatusDA = new DFPTStatusDA();
            m_objDFPTStatusDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(FPTStatusVM.Prop.FPTStatusID.MapAlias);
            m_lstSelect.Add(FPTStatusVM.Prop.FPTID.MapAlias);
            m_lstSelect.Add(FPTStatusVM.Prop.StatusDateTimeStamp.MapAlias);
            m_lstSelect.Add(FPTStatusVM.Prop.StatusID.MapAlias);
            m_lstSelect.Add(FPTStatusVM.Prop.Remarks.MapAlias);
            m_lstSelect.Add(FPTStatusVM.Prop.StatusDesc.MapAlias);
            m_lstSelect.Add(FPTStatusVM.Prop.ModifiedBy.MapAlias);
            m_lstSelect.Add(FPTStatusVM.Prop.CreatedDate.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(FPTID);
            m_objFilter.Add(FPTStatusVM.Prop.FPTID.Map, m_lstFilter);


            Dictionary<int, DataSet> m_dicDFPTStatusDA = m_objDFPTStatusDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDFPTStatusDA.Success)
            {
                foreach (DataRow m_drDFPTStatusDA in m_dicDFPTStatusDA[0].Tables[0].Rows)
                {
                    FPTStatusVM m_objFPTStatusVM = new FPTStatusVM();
                    m_objFPTStatusVM.FPTStatusID = m_drDFPTStatusDA[FPTStatusVM.Prop.FPTStatusID.Name].ToString();
                    m_objFPTStatusVM.FPTID = m_drDFPTStatusDA[FPTStatusVM.Prop.FPTID.Name].ToString();
                    m_objFPTStatusVM.StatusDateTimeStamp = Convert.ToDateTime(m_drDFPTStatusDA[FPTStatusVM.Prop.StatusDateTimeStamp.Name]);
                    m_objFPTStatusVM.StatusID = (int)m_drDFPTStatusDA[FPTStatusVM.Prop.StatusID.Name];
                    m_objFPTStatusVM.Remarks = m_drDFPTStatusDA[FPTStatusVM.Prop.Remarks.Name].ToString();
                    m_objFPTStatusVM.StatusDesc = m_drDFPTStatusDA[FPTStatusVM.Prop.StatusDesc.Name].ToString();
                    m_objFPTStatusVM.ModifiedBy = m_drDFPTStatusDA[FPTStatusVM.Prop.ModifiedBy.Name].ToString();
                    m_objFPTStatusVM.CreatedDate = (DateTime)m_drDFPTStatusDA[FPTStatusVM.Prop.CreatedDate.Name];
                    m_lstFPTStatusVM.Add(m_objFPTStatusVM);
                }
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDFPTStatusDA.Message;

            return m_lstFPTStatusVM;

        }
        private List<NegotiationConfigurationsVM> GetListNegotiationConfigurationsVM(string FPTID, ref string message)
        {
            List<NegotiationConfigurationsVM> m_lstNegotiationConfigurationsVM = new List<NegotiationConfigurationsVM>();

            CNegotiationConfigurationsDA m_objCNegotiationConfigurationsDA = new CNegotiationConfigurationsDA();
            m_objCNegotiationConfigurationsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.NegotiationConfigID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.NegotiationConfigTypeID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.TaskID.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.ParameterValue.MapAlias);
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.Descriptions.MapAlias);


            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(FPTID);
            m_objFilter.Add(NegotiationConfigurationsVM.Prop.FPTID.Map, m_lstFilter);


            Dictionary<int, DataSet> m_dicCNegotiationConfigurationsDA = m_objCNegotiationConfigurationsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objCNegotiationConfigurationsDA.Success)
            {
                foreach (DataRow m_drCNegotiationConfigurationsDA in m_dicCNegotiationConfigurationsDA[0].Tables[0].Rows)
                {
                    NegotiationConfigurationsVM m_objNegotiationConfigurationsVM = new NegotiationConfigurationsVM();
                    m_objNegotiationConfigurationsVM.NegotiationConfigID = m_drCNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.NegotiationConfigID.Name].ToString();
                    m_objNegotiationConfigurationsVM.NegotiationConfigTypeID = m_drCNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.NegotiationConfigTypeID.Name].ToString();
                    m_objNegotiationConfigurationsVM.TaskID = m_drCNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.TaskID.Name].ToString();
                    m_objNegotiationConfigurationsVM.ParameterValue = m_drCNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.ParameterValue.Name].ToString();
                    m_objNegotiationConfigurationsVM.Descriptions = m_drCNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.Descriptions.Name].ToString();

                    m_lstNegotiationConfigurationsVM.Add(m_objNegotiationConfigurationsVM);
                }
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objCNegotiationConfigurationsDA.Message;

            return m_lstNegotiationConfigurationsVM;
        }
        private List<NegotiationBidEntryVM> GetNegotiationBidEntryVM(string FPTID)
        {
            List<NegotiationBidEntryVM> m_lstNegotiationBidEntryVM = new List<NegotiationBidEntryVM>();

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();
            DNegotiationBidEntryDA m_objDNegotiationBidEntryDA = new DNegotiationBidEntryDA();
            m_objDNegotiationBidEntryDA.ConnectionStringName = Global.ConnStrConfigName;

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(FPTID);
            m_objFilter.Add(NegotiationBidEntryVM.Prop.FPTID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.NotEqual);
            m_lstFilter.Add(General.EnumDesc(VendorBidTypes.SubItem));
            m_objFilter.Add(NegotiationBidEntryVM.Prop.BidTypeID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add("7777, 8888");
            m_objFilter.Add(NegotiationBidEntryVM.Prop.Sequence.Map, m_lstFilter);

            m_lstSelect = new List<string>();
            m_lstSelect.Add(NegotiationBidEntryVM.Prop.NegotiationEntryID.MapAlias);
            m_lstSelect.Add(NegotiationBidEntryVM.Prop.NegotiationBidID.MapAlias);
            m_lstSelect.Add(NegotiationBidEntryVM.Prop.BidValue.MapAlias);
            m_lstSelect.Add(NegotiationBidEntryVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(NegotiationBidEntryVM.Prop.VendorDesc.MapAlias);
            m_lstSelect.Add(NegotiationBidEntryVM.Prop.ProjectID.MapAlias);
            m_lstSelect.Add(NegotiationBidEntryVM.Prop.ProjectDesc.MapAlias);
            m_lstSelect.Add(NegotiationBidEntryVM.Prop.ParameterValue.MapAlias);
            m_lstSelect.Add(NegotiationBidEntryVM.Prop.BidTypeID.MapAlias);
            m_lstSelect.Add(NegotiationBidEntryVM.Prop.FPTVendorParticipantID.MapAlias);
            m_lstSelect.Add(NegotiationBidEntryVM.Prop.BudgetPlanDefaultValue.MapAlias);
            m_lstSelect.Add(NegotiationBidEntryVM.Prop.Sequence.MapAlias);
            m_lstSelect.Add(NegotiationBidEntryVM.Prop.RoundID.MapAlias);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(NegotiationBidEntryVM.Prop.RoundID.Map, OrderDirection.Descending);

            Dictionary<int, DataSet> m_dicDNegotiationBidEntryDA = m_objDNegotiationBidEntryDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);

            if (m_objDNegotiationBidEntryDA.Success)
            {
                foreach (DataRow item in m_dicDNegotiationBidEntryDA[0].Tables[0].Rows)
                {
                    NegotiationBidEntryVM m_objNegotiationBidEntryVM = new NegotiationBidEntryVM();
                    m_objNegotiationBidEntryVM.NegotiationEntryID = item[NegotiationBidEntryVM.Prop.NegotiationEntryID.Name].ToString();
                    m_objNegotiationBidEntryVM.NegotiationBidID = item[NegotiationBidEntryVM.Prop.NegotiationBidID.Name].ToString();
                    m_objNegotiationBidEntryVM.BidValue = (decimal)item[NegotiationBidEntryVM.Prop.BidValue.Name];
                    m_objNegotiationBidEntryVM.VendorID = item[NegotiationBidEntryVM.Prop.VendorID.Name].ToString();
                    m_objNegotiationBidEntryVM.VendorDesc = item[NegotiationBidEntryVM.Prop.VendorDesc.Name].ToString();
                    m_objNegotiationBidEntryVM.ProjectID = item[NegotiationBidEntryVM.Prop.ProjectID.Name].ToString();
                    m_objNegotiationBidEntryVM.ProjectDesc = item[NegotiationBidEntryVM.Prop.ProjectDesc.Name].ToString();
                    m_objNegotiationBidEntryVM.ParameterValue = item[NegotiationBidEntryVM.Prop.ParameterValue.Name].ToString();
                    m_objNegotiationBidEntryVM.BidTypeID = item[NegotiationBidEntryVM.Prop.BidTypeID.Name].ToString();
                    m_objNegotiationBidEntryVM.FPTVendorParticipantID = item[NegotiationBidEntryVM.Prop.FPTVendorParticipantID.Name].ToString();
                    m_objNegotiationBidEntryVM.BudgetPlanDefaultValue = (decimal)item[NegotiationBidEntryVM.Prop.BudgetPlanDefaultValue.Name];
                    m_objNegotiationBidEntryVM.Sequence = (int?)item[NegotiationBidEntryVM.Prop.Sequence.Name];
                    m_objNegotiationBidEntryVM.RoundID = item[NegotiationBidEntryVM.Prop.RoundID.Name].ToString();
                    m_lstNegotiationBidEntryVM.Add(m_objNegotiationBidEntryVM);
                }
            }

            return m_lstNegotiationBidEntryVM;
        }
        private NotificationAttachmentVM GetSP(FPTVendorWinnerVM Model, int type, string Lang)
        {
            NotificationAttachmentVM m_NotificationAttachmentVM = new NotificationAttachmentVM();
            if (!Directory.Exists(Server.MapPath(@"~/Temp/"))) Directory.CreateDirectory(Server.MapPath(@"~/Temp/"));

            string m_filepath = Server.MapPath($"~/Content/Template/Report/sp-new-tender-pemasangan-{Lang}.docx");
            if (type == 1) m_filepath = Server.MapPath($"~/Content/Template/Report/sp-new-tender-pemasangan-{Lang}.docx");// 1= tender pemasangan
            if (type == 2) m_filepath = Server.MapPath($"~/Content/Template/Report/sp-new-tender-pengadaan-{Lang}.docx");// 2= tender pengadaan
            if (type == 3) m_filepath = Server.MapPath($"~/Content/Template/Report/sp-nsc-pemasangan-{Lang}.docx");// 3= nsc pemasangan
            if (type == 4) m_filepath = Server.MapPath($"~/Content/Template/Report/sp-new-tender-pengadaan-{Lang}.docx");// 4= nsc pengadaan
            DocX m_document = null;

            string message = string.Empty;
            List<FPTAdditionalInfoVM> m_ListFPTAdditionalInfoVM = GetListFPTAdditionalInfoVM(Model.FPTID, ref message);

            try
            {
                m_document = DocX.Load(m_filepath);
            }
            catch (Exception e)
            {
                throw e;
            }

            decimal m_Bidafterfee = Model.BidValue * (1 + (Model.BidFee / 100));
            m_Bidafterfee = Math.Round(m_Bidafterfee);
            string m_typetender = "TENDER";
            if (Model.IsSync)
            {
                m_typetender = Model.FPTID.Split('-')[2].ToString();
            }
            Model.FPTID = (Model.FPTID == null) ? string.Empty : Model.FPTID;
            Model.BudgetPlanName = (Model.BudgetPlanName == null) ? string.Empty : Model.BudgetPlanName;
            Model.VendorName = (Model.VendorName == null) ? string.Empty : Model.VendorName;
            Model.VendorAddress = (Model.VendorAddress == null) ? string.Empty : Model.VendorAddress;
            Model.VendorEmail = (Model.VendorEmail == null) ? string.Empty : Model.VendorEmail;
            Model.VendorPhone = (Model.VendorPhone == null) ? string.Empty : Model.VendorPhone;
            Model.FPTDescriptions = (Model.FPTDescriptions == null) ? string.Empty : Model.FPTDescriptions;
            Model.ProjectName = (Model.ProjectName == null) ? string.Empty : Model.ProjectName;
            Model.CompanyDesc = (Model.CompanyDesc == null) ? string.Empty : Model.CompanyDesc;
            if (m_ListFPTAdditionalInfoVM.Any(x => x.FPTAdditionalInfoItemID == "9"))
            {
                Model.CompanyDesc = GetCompanyName(m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "9").FirstOrDefault().Value);
            }

            DocX m_documenttemp = m_document.Copy();
            string m_letterno = (Model.ModifiedDate == null) ? "..." : $"{Model.LetterNumber.ToString()}/TRM-{Model.BusinessUnitID}-{Model.DivisionID}/{m_typetender}/{ToRoman(((DateTime)Model.ModifiedDate).Month)}/{((DateTime)Model.ModifiedDate).ToString("yy")}";
            string m_rnd = "SP-tender-pemasangan " + Model.FPTID + " " + Model.BudgetPlanName + " " + Model.VendorName;
            m_documenttemp.ReplaceText("[date]", FormatDateReport(DateTime.Now, Lang));
            m_documenttemp.ReplaceText("[letterno]", m_letterno);
            m_documenttemp.ReplaceText("[vendorname]", Model.VendorName);
            m_documenttemp.ReplaceText("[vendoraddress]", Model.VendorAddress);
            m_documenttemp.ReplaceText("[vendorup]", "...");
            m_documenttemp.ReplaceText("[vendoremail]", Model.VendorEmail);
            m_documenttemp.ReplaceText("[vendortelp]", Model.VendorPhone);
            m_documenttemp.ReplaceText("[vendorfax]", "...");
            string m_strBid = (Lang == "ID") ? Global.GetTerbilang((long)m_Bidafterfee) : Global.GetTerbilangEN(m_Bidafterfee);
            m_strBid += " Rupiah";
            m_documenttemp.ReplaceText("[bidvalue]", $"{((long)m_Bidafterfee).ToString(Global.IntegerNumberFormat)} ({m_strBid})");
            m_documenttemp.ReplaceText("[trm]", "...");

            m_documenttemp.ReplaceText("[budgetplan]", Model.FPTDescriptions);
            m_documenttemp.ReplaceText("[project]", Model.ProjectName);
            bool m_boolcontainpt = Model.CompanyDesc.ToLower().Contains("pt.") || Model.CompanyDesc.ToLower().Contains("pt ");
            Model.CompanyDesc = (m_boolcontainpt) ? Model.CompanyDesc : "PT. " + Model.CompanyDesc;
            m_documenttemp.ReplaceText("[company]", Model.CompanyDesc);
            m_documenttemp.ReplaceText("[cc]", "..");
            //m_documenttemp.SaveAs(Server.MapPath("~/Temp/" + m_rnd + ".docx"));

            MemoryStream m_msword = new MemoryStream();
            m_documenttemp.SaveAs(m_msword);

            m_NotificationAttachmentVM.AttachmentValueID = "";
            m_NotificationAttachmentVM.Filename = "Surat Penunjukan Tender";
            m_NotificationAttachmentVM.ContentType = ".pdf";

            Document document = new Document();
            document.LoadFromStream(m_msword, FileFormat.Docx2013);
            MemoryStream m_mspdf = new MemoryStream();
            document.SaveToStream(m_mspdf, FileFormat.PDF);

            m_NotificationAttachmentVM.RawData = Convert.ToBase64String(m_mspdf.ToArray());

            return m_NotificationAttachmentVM;
        }
        private NotificationAttachmentVM GetTL(FPTVendorWinnerVM Model, string Winner, string Lang)
        {
            NotificationAttachmentVM m_NotificationAttachmentVM = new NotificationAttachmentVM();
            m_NotificationAttachmentVM.AttachmentValueID = "";
            m_NotificationAttachmentVM.Filename = "Thank You Letter";
            m_NotificationAttachmentVM.ContentType = ".pdf";

            string message = string.Empty;
            List<FPTAdditionalInfoVM> m_ListFPTAdditionalInfoVM = GetListFPTAdditionalInfoVM(Model.FPTID, ref message);

            if (!Directory.Exists(Server.MapPath(@"~/Temp/"))) Directory.CreateDirectory(Server.MapPath(@"~/Temp/"));

            string m_filepath = Server.MapPath($"~/Content/Template/Report/thank-you-letter-{Lang}.docx");
            DocX m_document = null;


            //List<string> m_lstfilename = new List<string>();
            try
            {
                m_document = DocX.Load(m_filepath);
            }
            catch (Exception e)
            {
                throw e;
            }

            Model.FPTID = Model.FPTID == null ? string.Empty : Model.FPTID;
            Model.VendorName = Model.VendorName == null ? string.Empty : Model.VendorName;
            Model.VendorAddress = Model.VendorAddress == null ? string.Empty : Model.VendorAddress;
            Model.VendorEmail = Model.VendorEmail == null ? string.Empty : Model.VendorEmail;
            Model.VendorPhone = Model.VendorPhone == null ? string.Empty : Model.VendorPhone;
            Model.ProjectName = Model.ProjectName == null ? string.Empty : Model.ProjectName;
            Model.CompanyDesc = Model.CompanyDesc == null ? string.Empty : Model.CompanyDesc;
            Model.BudgetPlanName = Model.BudgetPlanName == null ? string.Empty : Model.BudgetPlanName;
            if (m_ListFPTAdditionalInfoVM.Any(x => x.FPTAdditionalInfoItemID == "9"))
            {
                Model.CompanyDesc = GetCompanyName(m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "9").FirstOrDefault().Value);
            }


            DocX m_documenttemp = m_document.Copy();
            string m_rnd = "FPTPPT " + Model.FPTID + " " + Model.BudgetPlanName + " " + Model.VendorName;
            string m_letterno = (Model.ModifiedDate == null) ? "..." : $"{Model.LetterNumber.ToString()}/TRM-{Model.BusinessUnitID}-{Model.DivisionID}/TENDER/{ToRoman(((DateTime)Model.ModifiedDate).Month)}/{((DateTime)Model.ModifiedDate).ToString("yy")}";
            m_documenttemp.ReplaceText("[date]", FormatDateReport(DateTime.Now, "ID"));
            m_documenttemp.ReplaceText("[letterno]", m_letterno);
            m_documenttemp.ReplaceText("[vendorname]", Model.VendorName);
            m_documenttemp.ReplaceText("[vendoraddress]", Model.VendorAddress);
            m_documenttemp.ReplaceText("[vendorup]", "...");
            m_documenttemp.ReplaceText("[vendoremail]", Model.VendorEmail);
            m_documenttemp.ReplaceText("[vendortelp]", Model.VendorPhone);
            m_documenttemp.ReplaceText("[vendorfax]", "...");
            m_documenttemp.ReplaceText("[vendorwinner]", Winner);//todo: use bpid
            m_documenttemp.ReplaceText("[trm]", "...");
            m_documenttemp.ReplaceText("[project]", Model.ProjectName);
            m_documenttemp.ReplaceText("[budgetplan]", Model.BudgetPlanName);
            bool m_boolcontainpt = Model.CompanyDesc.ToLower().Contains("pt.") || Model.CompanyDesc.ToLower().Contains("pt ");
            Model.CompanyDesc = (m_boolcontainpt) ? Model.CompanyDesc : "PT. " + Model.CompanyDesc;
            m_documenttemp.ReplaceText("[company]", Model.CompanyDesc);
            m_documenttemp.ReplaceText("[cc]", "...");

            //m_documenttemp.SaveAs(Server.MapPath("~/Temp/" + m_rnd + ".docx"));
            //m_lstfilename.Add(m_rnd);

            MemoryStream m_msword = new MemoryStream();
            m_documenttemp.SaveAs(m_msword);

            Document document = new Document();
            document.LoadFromStream(m_msword, FileFormat.Docx2013);
            MemoryStream m_mspdf = new MemoryStream();
            document.SaveToStream(m_mspdf, FileFormat.PDF);

            m_NotificationAttachmentVM.RawData = Convert.ToBase64String(m_mspdf.ToArray());

            return m_NotificationAttachmentVM;


        }

        private string GetCompanyName(string CompanyID)
        {
            MCompanyDA m_objMCompanyDA = new MCompanyDA();
            m_objMCompanyDA.ConnectionStringName = Global.ConnStrConfigName;
            string m_stretval = string.Empty;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(CompanyVM.Prop.CompanyID.MapAlias);
            m_lstSelect.Add(CompanyVM.Prop.CompanyDesc.MapAlias);
            m_lstSelect.Add(CompanyVM.Prop.CountryDesc.MapAlias);
            m_lstSelect.Add(CompanyVM.Prop.City.MapAlias);
            m_lstSelect.Add(CompanyVM.Prop.Street.MapAlias);
            m_lstSelect.Add(CompanyVM.Prop.Postal.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(CompanyID);
            m_objFilter.Add(CompanyVM.Prop.CompanyID.Map, m_lstFilter);

            List<CompanyVM> m_lstCompanyVM = new List<CompanyVM>();
            Dictionary<int, DataSet> m_dicDNotificationMap = m_objMCompanyDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);
            if (m_objMCompanyDA.Message == string.Empty && m_dicDNotificationMap[0].Tables[0].Rows.Count > 0)
            {
                var m_drMCompanyDA = m_dicDNotificationMap[0].Tables[0].Rows[0];
                m_stretval = m_drMCompanyDA[CompanyVM.Prop.CompanyDesc.Name].ToString();
            }
            return m_stretval;
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

            //m_lstFilter = new List<object>();
            //m_lstFilter.Add(Operator.Equals);
            //m_lstFilter.Add(NotificationTemplateID);
            //m_objFilter.Add(NotificationMapVM.Prop.NotificationTemplateID.Map, m_lstFilter);


            //Dictionary<string, OrderDirection> m_objOrderBy = new Dictionary<string, OrderDirection>();
            //m_objOrderBy.Add(NotificationMapVM.Prop.FunctionID.Map, OrderDirection.Ascending);
            List<NotificationMapVM> m_lstNotificationMapVM = new List<NotificationMapVM>();
            Dictionary<int, DataSet> m_dicDNotificationMap = m_objDNotificationMapDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);
            if (m_objDNotificationMapDA.Message == string.Empty)
            {
                m_lstNotificationMapVM = (
                  from DataRow m_drDNotificationMap in m_dicDNotificationMap[0].Tables[0].Rows
                  select new NotificationMapVM
                  {
                      NotifMapID = m_drDNotificationMap[NotificationMapVM.Prop.NotifMapID.Name].ToString(),
                      FunctionID = m_drDNotificationMap[NotificationMapVM.Prop.FunctionID.Name].ToString(),
                      NotificationTemplateID = m_drDNotificationMap[NotificationMapVM.Prop.NotificationTemplateID.Name].ToString(),
                      IsDefault = (bool)m_drDNotificationMap[NotificationMapVM.Prop.IsDefault.Name]
                  }
                ).ToList();
            }
            NotificationMapVM m_objNotificationMapVM = m_lstNotificationMapVM.Where(x => x.IsDefault).Any() ? m_lstNotificationMapVM.Where(x => x.IsDefault).FirstOrDefault() : null;
            return m_objNotificationMapVM;
        }
        private List<TCMembersVM> GetTCMemberLv(ref string message)
        {
            List<TCMembersVM> m_lstTCMembersVM = new List<TCMembersVM>();
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

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();


            List<object> m_lstFilter = new List<object>();


            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("TC-A");
            m_objFilter.Add(TCMembersVM.Prop.TCTypeID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.LessThanEqual);
            m_lstFilter.Add(DateTime.Now.Date);
            m_objFilter.Add(TCMembersVM.Prop.PeriodStart.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.GreaterThanEqual);
            m_lstFilter.Add(DateTime.Now.Date);
            m_objFilter.Add(TCMembersVM.Prop.PeriodEnd.Map, m_lstFilter);


            Dictionary<int, DataSet> m_dicTTCMembersDA = m_objTTCMembersDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objTTCMembersDA.Message == string.Empty)
            {
                foreach (DataRow m_drTTCMembersDA in m_dicTTCMembersDA[0].Tables[0].Rows)
                {
                    TCMembersVM m_objTCMembersVM = new TCMembersVM();
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
                    m_objTCMembersVM.ListTCAppliedTypesVM = GetListTCAppliedTypes(m_objTCMembersVM.TCMemberID, ref message);
                    m_objTCMembersVM.TCTypeID = m_drTTCMembersDA[TCMembersVM.Prop.TCTypeID.Name].ToString();
                    m_objTCMembersVM.BusinessUnitID = m_drTTCMembersDA[TCMembersVM.Prop.BusinessUnitID.Name].ToString();
                    m_objTCMembersVM.BusinessUnitDesc = m_drTTCMembersDA[TCMembersVM.Prop.BusinessUnitDesc.Name].ToString();
                    m_lstTCMembersVM.Add(m_objTCMembersVM);
                }
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objTTCMembersDA.Message;

            return m_lstTCMembersVM;
        }

        #endregion


    }
}