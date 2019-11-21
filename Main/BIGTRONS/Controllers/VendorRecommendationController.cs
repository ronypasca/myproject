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
    public class VendorRecommendationController : BaseController
    {
        private readonly string title = "Vendor Recommendation";
        private readonly string dataSessionName = "FormData";

        #region Public Action
        public ActionResult Index()
        {
            base.Initialize();
            ViewBag.Title = title;
            return View();
        }
        public ActionResult Update(string Caller, string Selected, string FPTID = "", string RoundID = "")
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Update)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            string m_strFPTID;
            string m_strRoundID;
            string m_strTCMemberID;
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
            }
            string m_strMessage = string.Empty;
            string m_strBusinessUnitIDs = "";
            UserVM userVM = getCurentUser();
            m_strTCMemberID = userVM.TCMember.TCMemberID; //getTCMemberID(m_strFPTID);
            //Get BusinessUnit
            if (userVM.TCMember != null)
                m_strBusinessUnitIDs = userVM.TCMember.BusinessUnitID;

            if (!ValidateTCAccess(m_strFPTID, ref m_strMessage))
            {
                Global.ShowErrorAlert(title, General.EnumDesc(MessageLib.NotAuthorized));
                return this.Direct();
            }

            

            ViewDataDictionary m_vddFPT = new ViewDataDictionary();
            m_vddFPT.Add("FPTIDData", m_strFPTID);
            m_vddFPT.Add("TCMemberIDData", m_strTCMemberID);
            m_vddFPT.Add("RoundIDData", m_strRoundID);
            m_vddFPT.Add("FPTNameData", m_strFPTID);
            //m_vddFPT.Add("TCTypes", string.Join(",", m_strTypes));
            m_vddFPT.Add("BusinessUnitIDs", m_strBusinessUnitIDs);
            //m_vddFPT.Add("PriceMarginTopData", m_decMarginTop);
            //m_vddFPT.Add("PriceMarginBottomData", m_decMarginBottom);
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;

            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = null,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddFPT,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
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

        private int GenerateNumber(ref bool createnew)
        {
            int m_intretval = 0;
                       
             //Get Last TNumber 
            List<TNumbering> m_lstTNumbering = new List<TNumbering>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();
            TNumberingDA m_objTNumberingDA = new TNumberingDA();
            DFPTVendorWinnerDA m_objDFPTVendorWinnerDA = new DFPTVendorWinnerDA();


            m_objTNumberingDA.ConnectionStringName = Global.ConnStrConfigName;
            m_objDFPTVendorWinnerDA.ConnectionStringName = Global.ConnStrConfigName;
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("VR");
            m_objFilter.Add(nameof(TNumbering.Header), m_lstFilter);
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(DateTime.Now.Year.ToString());//todo:
            m_objFilter.Add(nameof(TNumbering.Year), m_lstFilter);

            m_lstSelect = new List<string>();
            m_lstSelect.Add(nameof(TNumbering.LastNo));

            Dictionary<int, DataSet> m_dicTNumberingDA = m_objTNumberingDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null, null);
            if (m_objTNumberingDA.Success)
            {
                foreach (DataRow item in m_dicTNumberingDA[0].Tables[0].Rows)
                {
                    TNumbering m_objTNumbering = new TNumbering();
                    m_objTNumbering.LastNo = (int)item[nameof(TNumbering.LastNo)];
                    m_lstTNumbering.Add(m_objTNumbering);
                }
            }
            if (m_lstTNumbering.Any())
            {
                createnew = false;
                m_intretval = m_lstTNumbering.FirstOrDefault().LastNo;
            }
            else
            {
                createnew = true;
            }
            


            return m_intretval + 1;
        }

        public ActionResult Save()
        {
            Dictionary<string, object>[] m_arrList = JSON.Deserialize<Dictionary<string, object>[]>(Request.Params["GridList"]);
            string FPTID = Request.Params["FPTID"].ToString();
            string TCMemberID = Request.Params["TCMemberID"].ToString();
            List<FPTVendorRecommendationsVM> m_lstRecom = getFPTVendorRecommendationsVM(FPTID, string.Empty);
            List<DFPTVendorRecommendations> m_lstDFPTVendorRecommendations = new List<DFPTVendorRecommendations>();
            int m_curnum = 0;
            bool m_isnewnum = !m_lstRecom.Any(x => !string.IsNullOrEmpty(x.LetterNumber));
            bool m_insertnum = false;
            if (m_isnewnum)
            {
                m_curnum = GenerateNumber(ref m_insertnum);
            }
            else
            {
                m_curnum = Convert.ToInt32(m_lstRecom.Where(x => !string.IsNullOrEmpty(x.LetterNumber)).FirstOrDefault().LetterNumber);
            }
            
            m_lstDFPTVendorRecommendations = (
                            from Dictionary<string, object> m_dicDFPTVendorRecommendations in m_arrList
                            select new DFPTVendorRecommendations()
                            {
                                VendorRecommendationID = Guid.NewGuid().ToString().Replace("-", ""),
                                FPTID = FPTID,
                                TCMemberID = TCMemberID,
                                FPTVendorParticipantID = (m_dicDFPTVendorRecommendations[FPTVendorRecommendationsVM.Prop.FPTVendorParticipantID.Name] == null) ? string.Empty : m_dicDFPTVendorRecommendations[FPTVendorRecommendationsVM.Prop.FPTVendorParticipantID.Name].ToString(),
                                IsProposed = (m_dicDFPTVendorRecommendations[FPTVendorRecommendationsVM.Prop.IsProposed.Name] == null) ? false : (bool)m_dicDFPTVendorRecommendations[FPTVendorRecommendationsVM.Prop.IsProposed.Name],
                                Remarks = (m_dicDFPTVendorRecommendations["RecommendationRemark"] == null) ? string.Empty : m_dicDFPTVendorRecommendations["RecommendationRemark"].ToString(),
                                LetterNumber = m_curnum.ToString()
                            }).ToList();
            List<string> m_lstMessage = new List<string>();

            if (!isSaveValid(m_lstDFPTVendorRecommendations, ref m_lstMessage))
            {
                Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
                return this.Direct(true);
            }

            DFPTVendorRecommendationsDA m_objDFPTVendorRecommendationsDA = new DFPTVendorRecommendationsDA();
            TNumberingDA m_objTNumberingDA = new TNumberingDA();
            m_objDFPTVendorRecommendationsDA.ConnectionStringName = Global.ConnStrConfigName;
            string m_strTransName = "DFPTVendorRecommendationsDA";
            object m_objDBConnection = null;
            m_objDBConnection = m_objDFPTVendorRecommendationsDA.BeginTrans(m_strTransName);
            string m_strMessage = string.Empty;

            try
            {
                //DELETE
                Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.In);
                m_lstFilter.Add(string.Join(",", m_lstDFPTVendorRecommendations.Select(x => x.FPTVendorParticipantID).ToArray()));
                m_objFilter.Add(FPTVendorRecommendationsVM.Prop.FPTVendorParticipantID.Map, m_lstFilter);
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(FPTID);
                m_objFilter.Add(FPTVendorRecommendationsVM.Prop.FPTID.Map, m_lstFilter);
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(TCMemberID);
                m_objFilter.Add(FPTVendorRecommendationsVM.Prop.TCMemberID.Map, m_lstFilter);


                m_objDFPTVendorRecommendationsDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                if (m_objDFPTVendorRecommendationsDA.Message == General.EnumDesc(MessageLib.NotExist)) m_objDFPTVendorRecommendationsDA.Message = "";

                //INSERT
                foreach (DFPTVendorRecommendations m_DFPTVendorRecommendations in m_lstDFPTVendorRecommendations)
                {
                    m_objDFPTVendorRecommendationsDA.Data = m_DFPTVendorRecommendations;
                    m_objDFPTVendorRecommendationsDA.Insert(true, m_objDBConnection);
                }
                if (!m_objDFPTVendorRecommendationsDA.Success || m_objDFPTVendorRecommendationsDA.Message != string.Empty)
                    m_lstMessage.Add(m_objDFPTVendorRecommendationsDA.Message);

                //++NUM
                if (m_isnewnum)
                {
                    if (m_insertnum)
                    {
                        //insert tnumbering
                        TNumbering m_objTNumberinginsert = new TNumbering();
                        m_objTNumberinginsert.Header = "VR";
                        m_objTNumberinginsert.Year = DateTime.Now.Year.ToString();
                        m_objTNumberinginsert.Month = string.Empty;
                        m_objTNumberinginsert.CompanyID = string.Empty;
                        m_objTNumberinginsert.ProjectID = string.Empty;
                        m_objTNumberinginsert.LastNo = m_curnum;

                        m_objTNumberingDA.Data = m_objTNumberinginsert;
                        m_objTNumberingDA.Insert(true, m_objDBConnection);
                        if (!m_objTNumberingDA.Success || m_objTNumberingDA.Message != string.Empty)
                        {
                            m_lstMessage.Add(m_objTNumberingDA.Message);
                            m_objDFPTVendorRecommendationsDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                        }
                    }
                    else
                    {
                        //update tnumbering
                        
                        Dictionary<string, List<object>> m_dicSet = new Dictionary<string, List<object>>();
                        m_objFilter = new Dictionary<string, List<object>>();

                        m_lstFilter = new List<object>();
                        m_lstFilter.Add(Operator.Equals);
                        m_lstFilter.Add("VR");
                        m_objFilter.Add("Header", m_lstFilter);

                        m_lstFilter = new List<object>();
                        m_lstFilter.Add(Operator.Equals);
                        m_lstFilter.Add(DateTime.Now.Year.ToString());
                        m_objFilter.Add("Year", m_lstFilter);

                        List<object> m_lstSet = new List<object>();
                        m_lstSet.Add(typeof(string));
                        m_lstSet.Add(m_curnum);
                        m_dicSet.Add("LastNo", m_lstSet);
                        m_objTNumberingDA.UpdateBC(m_dicSet, m_objFilter, true, m_objDBConnection);

                        if (!m_objTNumberingDA.Success || m_objTNumberingDA.Message != string.Empty)
                        {
                            m_lstMessage.Add(m_objTNumberingDA.Message);
                            m_objDFPTVendorRecommendationsDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                        }
                    }
                    
                }

                m_objDFPTVendorRecommendationsDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                m_objDFPTVendorRecommendationsDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
            }

            if (m_lstMessage.Count <= 0)
            {
                string m_insmessage = string.Empty;
                InsertDFPTStatus(FPTID, (int)FPTStatusTypes.VendorRecommendation, DateTime.Now, ref m_insmessage);
                Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Updated));
                return this.Direct();
            }
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);

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
                    m_strDataIndex = FPTVM.Prop.Map(m_strDataIndex, false);
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

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.None);
            m_lstFilter.Add(string.Empty);
            m_objFilter.Add($"{FPTNegotiationRoundVM.Prop.Round.Map} - {FPTNegotiationRoundVM.Prop.RoundNo.Map} = 0", m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.LessThan);
            m_lstFilter.Add(DateTime.Now);
            m_objFilter.Add(FPTNegotiationRoundVM.Prop.EndDateTimeStamp.Map, m_lstFilter);


            Dictionary<int, DataSet> m_dicMFPTDA = m_objMFPTDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
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


                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(FPTVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

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
                        m_lstFPTNegotiationRoundVM.Add(m_objFPTNegotiationRoundVM);
                    }
                }
            }

            return this.Store(m_lstFPTNegotiationRoundVM, m_intCount);
        }
        public ActionResult GetPanel(string FPTID, string TCMemberID, string BusinessUnitIDs)
        {
            //Get Last Round
            //string m_strRoundID = getLastRoundID(FPTID);
            //Get All Structure
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();

            List<NegotiationBidEntryVM> m_lstNegotiationBidEntryVM = getNegotiationBidEntryVM(FPTID);
            List<FPTVendorRecommendationsVM> m_lstDFPTVendorRecommendations = getFPTVendorRecommendationsVM(FPTID, TCMemberID);
            List<FPTVendorParticipantsVM> m_lstFPTVendorParticipantsVM = getFPTVendorParticipantsVM(FPTID, BusinessUnitIDs);
            List<FPTNegotiationRoundVM> m_lstFPTNegotiationRoundVM = getlstNegoRound(FPTID);

            foreach (var item in m_lstFPTVendorParticipantsVM)
            {
                //TC Recomendation
                if (m_lstDFPTVendorRecommendations.Where(x => x.TCMemberID == TCMemberID && x.FPTVendorParticipantID == item.FPTVendorParticipantID).Any())
                {
                    item.IsProposed = m_lstDFPTVendorRecommendations.Where(x => x.TCMemberID == TCMemberID && x.FPTVendorParticipantID == item.FPTVendorParticipantID).FirstOrDefault().IsProposed;
                    item.RecommendationRemark = m_lstDFPTVendorRecommendations.Where(x => x.TCMemberID == TCMemberID && x.FPTVendorParticipantID == item.FPTVendorParticipantID).FirstOrDefault().Remarks;
                }
                //Bid Entry, Bid Fee, after fee
                item.BidValue = 0;
                item.BidFee = 0;
                if (m_lstNegotiationBidEntryVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID).Any())
                {
                    string m_roundid = m_lstNegotiationBidEntryVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID && x.Sequence == 7777).OrderByDescending(x => x.RoundID).FirstOrDefault().RoundID;
                    item.BidValue = m_lstNegotiationBidEntryVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID && x.Sequence == 7777).OrderByDescending(x => x.RoundID).FirstOrDefault().BidValue;
                    item.BudgetPlanDefaultValue = m_lstNegotiationBidEntryVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID && x.Sequence == 7777).FirstOrDefault().BudgetPlanDefaultValue;
                    item.BidFee = m_lstNegotiationBidEntryVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID && x.Sequence == 8888 && x.RoundID == m_roundid).Any() ? m_lstNegotiationBidEntryVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID && x.Sequence == 8888 && x.RoundID == m_roundid).OrderByDescending(x => x.RoundID).FirstOrDefault().BidValue : 0;
                    item.BudgetPlanDefaultFee = m_lstNegotiationBidEntryVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID && x.Sequence == 8888).Any() ? m_lstNegotiationBidEntryVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID && x.Sequence == 8888).OrderByDescending(x => x.RoundID).FirstOrDefault().BudgetPlanDefaultValue : 0;
                    item.BidAfterFee = item.BidValue * (1 + (item.BidFee / 100));//m_lstNegotiationBidEntryVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID && x.Sequence == 9999).Any() ? m_lstNegotiationBidEntryVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID && x.Sequence == 9999).OrderByDescending(x => x.RoundID).FirstOrDefault().BidValue : 0;
                    item.BudgetPlanDefaultValueAfterFee = item.BudgetPlanDefaultValue * (1 + (item.BudgetPlanDefaultFee / 100));//m_lstNegotiationBidEntryVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID && x.Sequence == 9999).Any() ? m_lstNegotiationBidEntryVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID && x.Sequence == 9999).OrderByDescending(x => x.RoundID).FirstOrDefault().BudgetPlanDefaultValue : 0;
                }

            }


            //Get Project List
            List<string> ProjectList = m_lstFPTVendorParticipantsVM.Select(x => x.ProjectID).Distinct().ToList();
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
                        ID = "P" + item + "Form",
                        Frame = true,
                        Title = m_lstFPTVendorParticipantsVM.Where(x => x.ProjectID == item).FirstOrDefault().ProjectDesc,
                        Border = false
                    };
                    //BP Tab Panel
                    TabPanel BPTabPanel = new TabPanel
                    {
                        ActiveTabIndex = 0,
                        Border = false
                    };
                    //Get BP List
                    List<string> BPList = m_lstFPTVendorParticipantsVM.Where(y => y.ProjectID == item).Select(x => x.ParameterValue).Distinct().ToList();
                    foreach (var BPitem in BPList)
                    {
                        //BP Panel
                        Panel BPPanel = new Panel
                        {
                            ID = "P" + BPitem + "Form",
                            Frame = true,
                            Title = m_lstFPTVendorParticipantsVM.Where(x => x.ParameterValue == BPitem).FirstOrDefault().BPVersionName,
                            Border = false
                        };
                        Toolbar m_BPPanelToolbar = new Toolbar();
                        GridPanel m_GPBP = generateGridPanel(m_lstFPTVendorParticipantsVM.Where(x => x.ParameterValue == BPitem).OrderBy(x => x.BidValue).ToList(), m_lstFPTNegotiationRoundVM.FirstOrDefault().BottomValue, m_lstFPTNegotiationRoundVM.FirstOrDefault().TopValue, BPitem);

                        List<Parameter> m_lstParameter = new List<Parameter>();
                        Parameter m_param;
                        m_param = new Parameter("GridList", "getGridList('" + m_GPBP.ID + "')", ParameterMode.Raw, true);
                        m_lstParameter.Add(m_param);
                        m_param = new Parameter("FPTID", FPTID, ParameterMode.Value, false);
                        m_lstParameter.Add(m_param);
                        m_param = new Parameter("TCMemberID", TCMemberID, ParameterMode.Value, false);
                        m_lstParameter.Add(m_param);
                        Button m_btnBPSubmit = new Button() { ID = "BtnSubmit" + m_GPBP.ID, Text = "Save" };
                        m_btnBPSubmit.DirectEvents.Click.Action = Url.Action("Save");
                        m_btnBPSubmit.DirectEvents.Click.EventMask.ShowMask = true;
                        m_btnBPSubmit.DirectEvents.Click.ExtraParams.AddRange(m_lstParameter);
                        m_BPPanelToolbar.Items.Add(m_btnBPSubmit);

                        BPPanel.TopBar.Add(m_BPPanelToolbar);
                        BPPanel.Items.Add(m_GPBP);
                        BPTabPanel.Items.Add(BPPanel);
                    }

                    ProjectPanel.Items.Add(BPTabPanel);
                    ProjectTabPanel.Items.Add(ProjectPanel);
                }
                return this.ComponentConfig(ProjectTabPanel);

            }
            else
            {
                Panel BPPanel = new Panel
                {
                    ID = "P" + "Upload" + "Form",//Todo:
                    Frame = true,
                    Title = "Upload",//Todo:
                    Border = false
                };
                Toolbar m_BPPanelToolbar = new Toolbar();
                GridPanel m_GPBP = generateGridPanel(m_lstFPTVendorParticipantsVM.OrderBy(x => x.BidValue).ToList(), m_lstFPTNegotiationRoundVM.FirstOrDefault().BottomValue, m_lstFPTNegotiationRoundVM.FirstOrDefault().TopValue);
                List<Parameter> m_lstParameter = new List<Parameter>();
                Parameter m_param;
                m_param = new Parameter("GridList", "getGridList('" + m_GPBP.ID + "')", ParameterMode.Raw, true);
                m_lstParameter.Add(m_param);
                m_param = new Parameter("FPTID", FPTID, ParameterMode.Value, false);
                m_lstParameter.Add(m_param);
                m_param = new Parameter("TCMemberID", TCMemberID, ParameterMode.Value, false);
                m_lstParameter.Add(m_param);
                Button m_btnBPSubmit = new Button() { ID = "BtnSubmit" + m_GPBP.ID, Text = "Save" };
                m_btnBPSubmit.DirectEvents.Click.Action = Url.Action("Save");
                m_btnBPSubmit.DirectEvents.Click.EventMask.ShowMask = true;
                m_btnBPSubmit.DirectEvents.Click.ExtraParams.AddRange(m_lstParameter);
                m_BPPanelToolbar.Items.Add(m_btnBPSubmit);

                BPPanel.TopBar.Add(m_BPPanelToolbar);
                BPPanel.Items.Add(m_GPBP);
                return this.ComponentConfig(BPPanel);
            }

        }
        public ActionResult Home()
        {
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 0;
            return this.Direct();
        }
        #endregion

        #region Direct Method

        #endregion

        #region Private Method
        private List<FPTVendorParticipantsVM> getFPTVendorParticipantsVM(string FPTID, string BusinessUnitIDs)
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

            //m_lstFilter = new List<object>();
            //m_lstFilter.Add(Operator.In);
            //m_lstFilter.Add(BusinessUnitIDs);
            //m_objFilter.Add(NegotiationBidStructuresVM.Prop.ParameterValue2.Map, m_lstFilter);

            //m_lstFilter = new List<object>();
            //m_lstFilter.Add(Operator.In);
            //m_lstFilter.Add(TCTypes);
            //m_objFilter.Add(FPTVendorParticipantsVM.Prop.ParameterValue2.Map, m_lstFilter);

            m_lstSelect = new List<string>();
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.FPTVendorParticipantID.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.VendorName.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.FPTID.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.StatusID.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.NegotiationConfigID.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.ParameterValue.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.ProjectID.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.ProjectDesc.MapAlias);
            m_lstSelect.Add(FPTVendorParticipantsVM.Prop.BPVersionName.MapAlias);

            Dictionary<int, DataSet> m_dicDNegotiationBidEntryDA = m_objDFPTVendorParticipantsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objDFPTVendorParticipantsDA.Success)
            {
                foreach (DataRow item in m_dicDNegotiationBidEntryDA[0].Tables[0].Rows)
                {
                    FPTVendorParticipantsVM m_objNFPTVendorParticipantsVM = new FPTVendorParticipantsVM();
                    m_objNFPTVendorParticipantsVM.FPTVendorParticipantID = item[FPTVendorParticipantsVM.Prop.FPTVendorParticipantID.Name].ToString();
                    m_objNFPTVendorParticipantsVM.VendorID = item[FPTVendorParticipantsVM.Prop.VendorID.Name].ToString();
                    m_objNFPTVendorParticipantsVM.VendorName = item[FPTVendorParticipantsVM.Prop.VendorName.Name].ToString();
                    m_objNFPTVendorParticipantsVM.FPTID = item[FPTVendorParticipantsVM.Prop.FPTID.Name].ToString();
                    m_objNFPTVendorParticipantsVM.StatusID = item[FPTVendorParticipantsVM.Prop.StatusID.Name].ToString();
                    m_objNFPTVendorParticipantsVM.NegotiationConfigID = item[FPTVendorParticipantsVM.Prop.NegotiationConfigID.Name].ToString();
                    m_objNFPTVendorParticipantsVM.ParameterValue = item[FPTVendorParticipantsVM.Prop.ParameterValue.Name].ToString();
                    m_objNFPTVendorParticipantsVM.ProjectID = item[FPTVendorParticipantsVM.Prop.ProjectID.Name].ToString();
                    m_objNFPTVendorParticipantsVM.ProjectDesc = item[FPTVendorParticipantsVM.Prop.ProjectDesc.Name].ToString();
                    m_objNFPTVendorParticipantsVM.BPVersionName = item[FPTVendorParticipantsVM.Prop.BPVersionName.Name].ToString();
                    m_lstFPTVendorParticipantsVM.Add(m_objNFPTVendorParticipantsVM);
                }
            }

            return m_lstFPTVendorParticipantsVM;
        }
        private List<FPTVendorRecommendationsVM> getFPTVendorRecommendationsVM(string FPTID, string TCMemberID)
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
            if (!string.IsNullOrEmpty(TCMemberID))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(TCMemberID);
                m_objFilter.Add(FPTVendorRecommendationsVM.Prop.TCMemberID.Map, m_lstFilter);
            }
            
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
        private List<FPTVendorWinnerVM> getFPTVendorWinnerVM(string FPTID)
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

            m_lstSelect.Add(FPTVendorWinnerVM.Prop.VendorName.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.VendorAddress.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.VendorPhone.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.VendorEmail.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.BudgetPlanName.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.ProjectName.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.StatusID.MapAlias);

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
                    m_objFPTVendorWinnerVM.VendorName = item[FPTVendorWinnerVM.Prop.VendorName.Name].ToString();
                    m_objFPTVendorWinnerVM.VendorAddress = item[FPTVendorWinnerVM.Prop.VendorAddress.Name].ToString();
                    m_objFPTVendorWinnerVM.VendorPhone = item[FPTVendorWinnerVM.Prop.VendorPhone.Name].ToString();
                    m_objFPTVendorWinnerVM.VendorEmail = item[FPTVendorWinnerVM.Prop.VendorEmail.Name].ToString();
                    m_objFPTVendorWinnerVM.BudgetPlanName = item[FPTVendorWinnerVM.Prop.BudgetPlanName.Name].ToString();
                    m_objFPTVendorWinnerVM.ProjectName = item[FPTVendorWinnerVM.Prop.ProjectName.Name].ToString();
                    m_objFPTVendorWinnerVM.StatusID = string.IsNullOrEmpty(item[FPTVendorWinnerVM.Prop.StatusID.Name].ToString()) ? 4 : (int)item[FPTVendorWinnerVM.Prop.StatusID.Name];
                    m_lstFPTVendorWinnerVM.Add(m_objFPTVendorWinnerVM);
                }
            }
            return m_lstFPTVendorWinnerVM;
        }
        private List<NegotiationBidEntryVM> getNegotiationBidEntryVM(string FPTID)
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
        private string getLastRoundID(string FPTID)
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
            m_lstSelect.Add(FPTNegotiationRoundVM.Prop.RoundID.MapAlias);
            m_lstSelect.Add(FPTNegotiationRoundVM.Prop.EndDateTimeStamp.MapAlias);


            Dictionary<int, DataSet> m_dicDFPTNegotiationRoundsDA = m_objDFPTNegotiationRoundsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objDFPTNegotiationRoundsDA.Success)
            {
                foreach (DataRow item in m_dicDFPTNegotiationRoundsDA[0].Tables[0].Rows)
                {
                    FPTNegotiationRoundVM m_objFPTNegotiationRoundVM = new FPTNegotiationRoundVM();
                    m_objFPTNegotiationRoundVM.RoundID = item[FPTNegotiationRoundVM.Prop.RoundID.Name].ToString();
                    m_objFPTNegotiationRoundVM.EndDateTimeStamp = (DateTime)item[FPTNegotiationRoundVM.Prop.EndDateTimeStamp.Name];

                    m_lstFPTNegotiationRoundVM.Add(m_objFPTNegotiationRoundVM);
                }
            }
            DateTime m_datetimenull = DateTime.Parse("9999-12-31 00:00:00.000");
            return m_lstFPTNegotiationRoundVM.Where(x => x.EndDateTimeStamp != m_datetimenull).OrderByDescending(x => x.EndDateTimeStamp).FirstOrDefault().RoundID;
        }
        private string getTCMemberID(string FPTID)
        {
            string m_strretVal = string.Empty;
            List<NegotiationConfigurationsVM> m_lstNegotiationConfigurationsVM = new List<NegotiationConfigurationsVM>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();
            CNegotiationConfigurationsDA m_objCNegotiationConfigurationsDA = new CNegotiationConfigurationsDA();
            m_objCNegotiationConfigurationsDA.ConnectionStringName = Global.ConnStrConfigName;

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(General.EnumDesc(NegoConfigTypes.TCMember));
            m_objFilter.Add(NegotiationConfigurationsVM.Prop.NegotiationConfigTypeID.Map, m_lstFilter);
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(FPTID);
            m_objFilter.Add(NegotiationConfigurationsVM.Prop.FPTID.Map, m_lstFilter);
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(HttpContext.User.Identity.Name);
            m_objFilter.Add(NegotiationConfigurationsVM.Prop.UserID.Map, m_lstFilter);


            m_lstSelect = new List<string>();
            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.ParameterValue.MapAlias);

            Dictionary<int, DataSet> m_dicCNegotiationConfigurationsDA = m_objCNegotiationConfigurationsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objCNegotiationConfigurationsDA.Success)
            {
                foreach (DataRow item in m_dicCNegotiationConfigurationsDA[0].Tables[0].Rows)
                {
                    NegotiationConfigurationsVM m_objNegotiationConfigurationsVM = new NegotiationConfigurationsVM();
                    m_objNegotiationConfigurationsVM.ParameterValue = item[NegotiationConfigurationsVM.Prop.ParameterValue.Name].ToString();
                    m_lstNegotiationConfigurationsVM.Add(m_objNegotiationConfigurationsVM);
                }
            }

            m_strretVal = (m_lstNegotiationConfigurationsVM.Any()) ? m_lstNegotiationConfigurationsVM.FirstOrDefault().ParameterValue : string.Empty;
            return m_strretVal;
        }
        
        private GridPanel generateGridPanel(List<FPTVendorParticipantsVM> ListNego, decimal BVal, decimal TVal, string BPID = "")
        {
            GridPanel m_gridpanel = new GridPanel
            {
                ID = "gridPanel" + BPID + "structure",
                Padding = 10,
                MinHeight = 200,
                Tag = new { bVal = BVal, tVal = TVal }
            };

            //SelectionModel
            RowSelectionModel m_rowSelectionModel = new RowSelectionModel() { Mode = SelectionMode.Multi, AllowDeselect = true };
            m_gridpanel.SelectionModel.Add(m_rowSelectionModel);

            //Store         
            Store m_store = new Store();
            Model m_model = new Model();
            ModelField m_ModelField = new ModelField();
            m_ModelField = new ModelField(FPTVendorParticipantsVM.Prop.FPTVendorParticipantID.Name);
            m_model.Fields.Add(m_ModelField);
            m_ModelField = new ModelField(FPTVendorParticipantsVM.Prop.VendorName.Name);
            m_model.Fields.Add(m_ModelField);
            m_ModelField = new ModelField(nameof(FPTVendorParticipantsVM.BidValue));
            m_model.Fields.Add(m_ModelField);
            m_ModelField = new ModelField(nameof(FPTVendorParticipantsVM.BidFee));
            m_model.Fields.Add(m_ModelField);
            m_ModelField = new ModelField(nameof(FPTVendorParticipantsVM.BidAfterFee));
            m_model.Fields.Add(m_ModelField);
            m_ModelField = new ModelField(FPTVendorRecommendationsVM.Prop.IsProposed.Name);
            m_model.Fields.Add(m_ModelField);
            m_ModelField = new ModelField(nameof(FPTVendorParticipantsVM.RecommendationRemark));
            m_model.Fields.Add(m_ModelField);
            m_ModelField = new ModelField(nameof(FPTVendorParticipantsVM.BudgetPlanDefaultValue));
            m_model.Fields.Add(m_ModelField);
            m_ModelField = new ModelField(nameof(FPTVendorParticipantsVM.BudgetPlanDefaultValueAfterFee));
            m_model.Fields.Add(m_ModelField);
            m_store.Model.Add(m_model);
            m_store.DataSource = ListNego;
            m_gridpanel.Store.Add(m_store);

            //Columns
            List<ColumnBase> m_ListColumn = new List<ColumnBase>();
            Checkbox m_objCheckbox = new Checkbox();
            ColumnBase m_objColumn = new Column();
            m_objColumn = new Column { Text = FPTVendorParticipantsVM.Prop.FPTVendorParticipantID.Desc, DataIndex = "FPTVendorParticipantID", Flex = 1, Hidden = true };
            m_ListColumn.Add(m_objColumn);
            m_objColumn = new Column { Text = FPTVendorParticipantsVM.Prop.VendorName.Desc, DataIndex = "VendorName", Flex = 2 };
            m_ListColumn.Add(m_objColumn);
            //m_objColumn = new Column { Text = "Bid", DataIndex = "BidValue", Flex = 1 };
            //m_objColumn.Renderer = new Renderer("renderGridColumn");
            //m_ListColumn.Add(m_objColumn);
            m_objColumn = new Column { Text = "Fee", DataIndex = "BidFee", Flex = 1 };
            //.Renderer = new Renderer("renderGridColumnFee");
            m_ListColumn.Add(m_objColumn);
            m_objColumn = new Column { Text = "Bid", DataIndex = "BidAfterFee", Flex = 1 };
            m_objColumn.Renderer = new Renderer("renderGridColumnAfterFee");
            m_ListColumn.Add(m_objColumn);
            m_objColumn = new Column { Text = "Remark", DataIndex = "RecommendationRemark", Flex = 3 };
            TextField m_objtextField = new TextField() { SubmitValue = true, HideTrigger = true };
            m_objColumn.Editor.Add(m_objtextField);
            m_ListColumn.Add(m_objColumn);
            m_objColumn = new CheckColumn { Text = "Proposed", DataIndex = "IsProposed", Flex = 1, Editable = true };
            m_ListColumn.Add(m_objColumn);
            m_gridpanel.ColumnModel.Columns.AddRange(m_ListColumn);

            //Plugin
            CellEditing m_objCellEditing = new CellEditing() { ClicksToEdit = 1 };
            m_gridpanel.Plugins.Add(m_objCellEditing);



            return m_gridpanel;
        }
        private bool isSaveValid(List<DFPTVendorRecommendations> m_lstDFPTVendorRecommendations, ref List<string> message)
        {
            bool m_boolretVal = true;
            foreach (var item in m_lstDFPTVendorRecommendations)
            {
                if (string.IsNullOrWhiteSpace(item.Remarks))
                {
                    message.Add("Remark cannot be blank");
                    return false;
                }
            }
            //vendorwinner already created
            List<FPTVendorWinnerVM> m_lstFPTVendorWinnerVM = getFPTVendorWinnerVM(m_lstDFPTVendorRecommendations.FirstOrDefault().FPTID);
            if (m_lstFPTVendorWinnerVM.Any())
            {
                if (m_lstFPTVendorWinnerVM.Where(x => x.StatusID != (int)TaskStatus.Draft).Any())
                {
                    message.Add("Vendor winner already created!");
                    m_boolretVal = false;
                }
            }

            return m_boolretVal;
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
        private List<TCMembersVM> GetListTCMemberByFPT(string FPTID, ref string message)
        {
            List<TCMembersVM> m_lstTCMembersVM = new List<TCMembersVM>();
            TTCMembersDA m_objTTCMembersDA = new TTCMembersDA();
            m_objTTCMembersDA.ConnectionStringName = Global.ConnStrConfigName;
            CNegotiationConfigurationsDA m_objCNegotiationConfigurationsDA = new CNegotiationConfigurationsDA();
            m_objCNegotiationConfigurationsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(TCMembersVM.Prop.TCMemberID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.EmployeeID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.EmployeeName.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.SuperiorID.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.SuperiorName.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.PeriodStart.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.PeriodEnd.MapAlias);
            m_lstSelect.Add(TCMembersVM.Prop.BusinessUnitID.MapAlias);
            //m_lstSelect.Add(TCMembersVM.Prop.DelegateStartDate.MapAlias);
            //m_lstSelect.Add(TCMembersVM.Prop.DelegateEndDate.MapAlias);
            //m_lstSelect.Add(TCMembersVM.Prop.DelegateTo.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(FPTID);
            m_objFilter.Add(NegotiationConfigurationsVM.Prop.FPTID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(General.EnumDesc(NegoConfigTypes.TCMember));
            m_objFilter.Add(NegotiationConfigurationsVM.Prop.NegotiationConfigTypeID.Map, m_lstFilter);


            Dictionary<int, DataSet> m_dicCNegotiationConfigurationsDA = m_objCNegotiationConfigurationsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objTTCMembersDA.Message == string.Empty)
            {
                foreach (DataRow m_drTTCMembersDA in m_dicCNegotiationConfigurationsDA[0].Tables[0].Rows)
                {
                    TCMembersVM m_objTCMembersVM = new TCMembersVM();
                    m_objTCMembersVM.TCMemberID = m_drTTCMembersDA[TCMembersVM.Prop.TCMemberID.Name].ToString();
                    m_objTCMembersVM.EmployeeID = m_drTTCMembersDA[TCMembersVM.Prop.EmployeeID.Name].ToString();
                    m_objTCMembersVM.EmployeeName = m_drTTCMembersDA[TCMembersVM.Prop.EmployeeName.Name].ToString();
                    m_objTCMembersVM.SuperiorID = m_drTTCMembersDA[TCMembersVM.Prop.SuperiorID.Name].ToString();
                    m_objTCMembersVM.SuperiorName = m_drTTCMembersDA[TCMembersVM.Prop.SuperiorName.Name].ToString();
                    m_objTCMembersVM.PeriodStart = DateTime.Parse(m_drTTCMembersDA[TCMembersVM.Prop.PeriodStart.Name].ToString());
                    m_objTCMembersVM.PeriodEnd = DateTime.Parse(m_drTTCMembersDA[TCMembersVM.Prop.PeriodEnd.Name].ToString());
                    m_objTCMembersVM.BusinessUnitID = m_drTTCMembersDA[TCMembersVM.Prop.BusinessUnitID.Name].ToString();
                    //m_objTCMembersVM.DelegateStartDate = !string.IsNullOrEmpty(m_drTTCMembersDA[TCMembersVM.Prop.DelegateStartDate.Name].ToString()) ? DateTime.Parse(m_drTTCMembersDA[TCMembersVM.Prop.DelegateStartDate.Name].ToString()) : (DateTime?)null;
                    //m_objTCMembersVM.DelegateEndDate = !string.IsNullOrEmpty(m_drTTCMembersDA[TCMembersVM.Prop.DelegateStartDate.Name].ToString()) ? DateTime.Parse(m_drTTCMembersDA[TCMembersVM.Prop.DelegateEndDate.Name].ToString()) : (DateTime?)null;
                    //m_objTCMembersVM.DelegateTo = m_drTTCMembersDA[TCMembersVM.Prop.DelegateTo.Name].ToString();
                    //m_objTCMembersVM.ListTCAppliedTypesVM = GetListTCAppliedTypes(m_objTCMembersVM.TCMemberID, ref message);
                    m_objTCMembersVM.ListTCMembersDelegationVM = GetListTCMembersDelegationVM(m_objTCMembersVM.TCMemberID).Where(x => x.DelegateStartDate >= DateTime.Now && x.DelegateEndDate <= DateTime.Now).ToList();
                    m_lstTCMembersVM.Add(m_objTCMembersVM);
                }
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objTTCMembersDA.Message;

            return m_lstTCMembersVM;
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
                        DelegateStartDate = !string.IsNullOrEmpty(m_drTTCMembersDA[TCMembersDelegationVM.Prop.DelegateStartDate.Name].ToString()) ? DateTime.Parse(m_drTTCMembersDA[TCMembersDelegationVM.Prop.DelegateStartDate.Name].ToString()) : (DateTime?)null,
                        DelegateEndDate = !string.IsNullOrEmpty(m_drTTCMembersDA[TCMembersDelegationVM.Prop.DelegateEndDate.Name].ToString()) ? DateTime.Parse(m_drTTCMembersDA[TCMembersDelegationVM.Prop.DelegateEndDate.Name].ToString()) : (DateTime?)null,
                    }
                ).ToList();
            }

            return m_listTCMembersDelegationVM;
        }
        #endregion


    }
}