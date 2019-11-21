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
using System.Net;
using System.Xml;

namespace com.SML.BIGTRONS.Controllers
{
	public class RoundMonitoringController : BaseController
	{
		private readonly string title = "Round Monitoring";
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

		public ActionResult ReadVendorParticipant(StoreRequestParameters parameters, string FPTID)
		{
			DFPTVendorParticipantsDA m_objDFPTVendorParticipantsDA = new DFPTVendorParticipantsDA();
			m_objDFPTVendorParticipantsDA.ConnectionStringName = Global.ConnStrConfigName;

			int m_intSkip = parameters.Start;
			int m_intLength = parameters.Limit;
			bool m_boolIsCount = true;

			FilterHeaderConditions m_fhcDFPTVendorParticipantss = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
			Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
			List<object> m_lstFilter = new List<object>();

			foreach (FilterHeaderCondition m_fhcFilter in m_fhcDFPTVendorParticipantss.Conditions)
			{
				string m_strDataIndex = m_fhcFilter.DataIndex;
				string m_strConditionOperator = m_fhcFilter.Operator;
				object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

				if (m_strDataIndex != string.Empty)
				{
					m_strDataIndex = FPTVendorParticipantsVM.Prop.Map(m_strDataIndex, false);
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
			m_lstFilter.Add(Operator.Equals);
			m_lstFilter.Add(FPTID);
			m_objFilter.Add(FPTVendorParticipantsVM.Prop.FPTID.Map, m_lstFilter);

			List<string> m_lstGroup = new List<string>();
			m_lstGroup.Add(FPTVendorParticipantsVM.Prop.VendorID.Map);
			m_lstGroup.Add(FPTVendorParticipantsVM.Prop.VendorName.Map);

			Dictionary<int, DataSet> m_dicDFPTVendorParticipantsDA = m_objDFPTVendorParticipantsDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, m_lstGroup, null, null);
			int m_intCount = 0;

			foreach (KeyValuePair<int, DataSet> m_kvpFPTVendorParticipantsBL in m_dicDFPTVendorParticipantsDA)
			{
				m_intCount = m_kvpFPTVendorParticipantsBL.Key;
				break;
			}

			List<FPTVendorParticipantsVM> m_lstFPTVendorParticipantsVM = new List<FPTVendorParticipantsVM>();
			if (m_intCount > 0)
			{
				m_boolIsCount = false;
				List<string> m_lstSelect = new List<string>();
				m_lstSelect.Add(FPTVendorParticipantsVM.Prop.VendorID.MapAlias);
				m_lstSelect.Add(FPTVendorParticipantsVM.Prop.VendorName.MapAlias);


				Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
				foreach (DataSorter m_dtsOrder in parameters.Sort)
					m_dicOrder.Add(FPTVendorParticipantsVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

				m_dicDFPTVendorParticipantsDA = m_objDFPTVendorParticipantsDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, m_lstGroup, m_dicOrder, null);
				if (m_objDFPTVendorParticipantsDA.Message == string.Empty)
				{
					m_lstFPTVendorParticipantsVM = (
						from DataRow m_drDFPTVendorParticipantsDA in m_dicDFPTVendorParticipantsDA[0].Tables[0].Rows
						select new FPTVendorParticipantsVM()
						{
							VendorID = m_drDFPTVendorParticipantsDA[FPTVendorParticipantsVM.Prop.VendorID.Name].ToString(),
							VendorName = m_drDFPTVendorParticipantsDA[FPTVendorParticipantsVM.Prop.VendorName.Name].ToString(),
							Checked = IsChecked(FPTID,"",m_drDFPTVendorParticipantsDA[FPTVendorParticipantsVM.Prop.VendorID.Name].ToString())
						}
					).ToList();
				}
			}
			return this.Store(m_lstFPTVendorParticipantsVM, m_intCount);
		}
        public ActionResult ReadTCParticipant(StoreRequestParameters parameters, string FPTID)
        {
            DFPTTCParticipantsDA m_objDFPTTCParticipantsDA = new DFPTTCParticipantsDA();
            m_objDFPTTCParticipantsDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcDFPTVendorParticipantss = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcDFPTVendorParticipantss.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = FPTTCParticipantsVM.Prop.Map(m_strDataIndex, false);
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
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(FPTID);
            m_objFilter.Add(FPTTCParticipantsVM.Prop.FPTID.Map, m_lstFilter);

            List<string> m_lstGroup = new List<string>();
            m_lstGroup.Add(FPTTCParticipantsVM.Prop.TCMemberID.Map);
            m_lstGroup.Add(FPTTCParticipantsVM.Prop.EmployeeName.Map);
            m_lstGroup.Add(FPTTCParticipantsVM.Prop.FPTID.Map);
            m_lstGroup.Add(FPTTCParticipantsVM.Prop.FPTTCParticipantID.Map);

            Dictionary<int, DataSet> m_dicDFPTTCParticipantsDA = m_objDFPTTCParticipantsDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, m_lstGroup, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpFPTTCParticipantsBL in m_dicDFPTTCParticipantsDA)
            {
                m_intCount = m_kvpFPTTCParticipantsBL.Key;
                break;
            }

            List<FPTTCParticipantsVM> m_lstFPTTCParticipantsVM = new List<FPTTCParticipantsVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(FPTTCParticipantsVM.Prop.TCMemberID.MapAlias);
                m_lstSelect.Add(FPTTCParticipantsVM.Prop.EmployeeName.MapAlias);
                m_lstSelect.Add(FPTTCParticipantsVM.Prop.FPTID.MapAlias);
                m_lstSelect.Add(FPTTCParticipantsVM.Prop.FPTTCParticipantID.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(FPTTCParticipantsVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicDFPTTCParticipantsDA = m_objDFPTTCParticipantsDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, m_lstGroup, m_dicOrder, null);
                if (m_objDFPTTCParticipantsDA.Message == string.Empty)
                {
                    m_lstFPTTCParticipantsVM = (
                        from DataRow m_drDFPTTCParticipantsDA in m_dicDFPTTCParticipantsDA[0].Tables[0].Rows
                        select new FPTTCParticipantsVM()
                        {
                            FPTTCParticipantID = m_drDFPTTCParticipantsDA[FPTTCParticipantsVM.Prop.FPTTCParticipantID.Name].ToString(),
                            FPTID = m_drDFPTTCParticipantsDA[FPTTCParticipantsVM.Prop.FPTID.Name].ToString(),
                            TCMemberID = m_drDFPTTCParticipantsDA[FPTTCParticipantsVM.Prop.TCMemberID.Name].ToString(),
                            EmployeeName = m_drDFPTTCParticipantsDA[FPTTCParticipantsVM.Prop.EmployeeName.Name].ToString(),
                            Checked = IsChecked(FPTID, m_drDFPTTCParticipantsDA[FPTTCParticipantsVM.Prop.TCMemberID.Name].ToString(),"")
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstFPTTCParticipantsVM, m_intCount);
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
			m_lstFilter.Add(Operator.Equals);
			m_lstFilter.Add(DateTime.Now.Date);
			m_objFilter.Add(string.Format("CAST({0} AS DATE)", FPTNegotiationRoundVM.Prop.Schedule.Map), m_lstFilter);

			m_lstFilter = new List<object>();
			m_lstFilter.Add(Operator.Equals);
			m_lstFilter.Add((int)TaskStatus.Approved);
			m_objFilter.Add(FPTNegotiationRoundVM.Prop.TaskStatusID.Map, m_lstFilter);


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
				m_lstSelect.Add(FPTNegotiationRoundVM.Prop.Schedule.MapAlias);
				m_lstSelect.Add(string.Format("CASE WHEN TotalRoundID > 0 THEN 'Active' ELSE 'Non Active' END AS {0}", FPTNegotiationRoundVM.Prop.Status.Name));

				Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
				foreach (DataSorter m_dtsOrder in parameters.Sort)
					m_dicOrder.Add(FPTVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

				m_dicMFPTDA = m_objMFPTDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
				if (m_objMFPTDA.Message == string.Empty)
				{
					// m_lstFPTNegotiationRoundVM = (
					// DataRow m_drMFPTDA = m_dicMFPTDA[0].Tables[0].Rows;
					foreach (DataRow m_drMFPTDA in m_dicMFPTDA[0].Tables[0].Rows)
					{
						FPTNegotiationRoundVM m_objFPTNegotiationRoundVM = new FPTNegotiationRoundVM();

						m_objFPTNegotiationRoundVM.FPTID = m_drMFPTDA[FPTNegotiationRoundVM.Prop.FPTID.Name].ToString();
						m_objFPTNegotiationRoundVM.FPTDesc = m_drMFPTDA[FPTNegotiationRoundVM.Prop.Descriptions.Name].ToString();
						m_objFPTNegotiationRoundVM.TotalVendors = (!string.IsNullOrEmpty(m_drMFPTDA[FPTNegotiationRoundVM.Prop.TotalVendors.Name].ToString()) ? int.Parse(m_drMFPTDA[FPTNegotiationRoundVM.Prop.TotalVendors.Name].ToString()) : (int?)null);
						m_objFPTNegotiationRoundVM.StartDateTimeStamp = !string.IsNullOrEmpty(m_drMFPTDA[FPTNegotiationRoundVM.Prop.StartDateTimeStamp.Name].ToString()) ? DateTime.Parse(m_drMFPTDA[FPTNegotiationRoundVM.Prop.StartDateTimeStamp.Name].ToString()) : (DateTime?)null;
						m_objFPTNegotiationRoundVM.EndDateTimeStamp = !string.IsNullOrEmpty(m_drMFPTDA[FPTNegotiationRoundVM.Prop.EndDateTimeStamp.Name].ToString()) ? DateTime.Parse(m_drMFPTDA[FPTNegotiationRoundVM.Prop.EndDateTimeStamp.Name].ToString()) : (DateTime?)null;
						m_objFPTNegotiationRoundVM.Schedule = !string.IsNullOrEmpty(m_drMFPTDA[FPTNegotiationRoundVM.Prop.Schedule.Name].ToString()) ? DateTime.Parse(m_drMFPTDA[FPTNegotiationRoundVM.Prop.Schedule.Name].ToString()) : (DateTime?)null;
						m_objFPTNegotiationRoundVM.Duration = string.IsNullOrEmpty(m_drMFPTDA[FPTNegotiationRoundVM.Prop.Duration.Name].ToString()) ? (int?)null : int.Parse(m_drMFPTDA[FPTNegotiationRoundVM.Prop.Duration.Name].ToString());
						m_objFPTNegotiationRoundVM.TotalRound = string.IsNullOrEmpty(m_drMFPTDA[FPTNegotiationRoundVM.Prop.Round.Name].ToString()) ? (int?)null : int.Parse(m_drMFPTDA[FPTNegotiationRoundVM.Prop.Round.Name].ToString());
						m_objFPTNegotiationRoundVM.RoundNo = string.IsNullOrEmpty(m_drMFPTDA[FPTNegotiationRoundVM.Prop.RoundNo.Name].ToString()) ? (int?)null : int.Parse(m_drMFPTDA[FPTNegotiationRoundVM.Prop.RoundNo.Name].ToString());
						m_objFPTNegotiationRoundVM.Status = m_drMFPTDA[FPTNegotiationRoundVM.Prop.Status.Name].ToString();
						m_lstFPTNegotiationRoundVM.Add(m_objFPTNegotiationRoundVM);
					}
				}
			}
			return this.Store(m_lstFPTNegotiationRoundVM, m_intCount);
		}

		public ActionResult ReadBrowse(StoreRequestParameters parameters)
		{
			DFPTNegotiationRoundsDA m_objDFPTNegotiationRoundsDA = new DFPTNegotiationRoundsDA();
			m_objDFPTNegotiationRoundsDA.ConnectionStringName = Global.ConnStrConfigName;

			int m_intSkip = parameters.Start;
			int m_intLength = parameters.Limit;
			bool m_boolIsCount = true;

			FilterHeaderConditions m_fhcDFPTNegotiationRounds = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
			Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

			foreach (FilterHeaderCondition m_fhcFilter in m_fhcDFPTNegotiationRounds.Conditions)
			{
				string m_strDataIndex = m_fhcFilter.DataIndex;
				string m_strConditionOperator = m_fhcFilter.Operator;
				object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

				if (m_strDataIndex != string.Empty)
				{
					m_strDataIndex = FPTNegotiationRoundVM.Prop.Map(m_strDataIndex, false);
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
			Dictionary<int, DataSet> m_dicDFPTNegotiationRoundsDA = m_objDFPTNegotiationRoundsDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
			int m_intCount = 0;

			foreach (KeyValuePair<int, DataSet> m_kvpFPTNegotiationRoundBL in m_dicDFPTNegotiationRoundsDA)
			{
				m_intCount = m_kvpFPTNegotiationRoundBL.Key;
				break;
			}

			List<FPTNegotiationRoundVM> m_lstFPTNegotiationRoundVM = new List<FPTNegotiationRoundVM>();
			if (m_intCount > 0)
			{
				m_boolIsCount = false;
				List<string> m_lstSelect = new List<string>();
				m_lstSelect.Add(FPTNegotiationRoundVM.Prop.RoundID.MapAlias);
				m_lstSelect.Add(FPTNegotiationRoundVM.Prop.FPTID.MapAlias);
				m_lstSelect.Add(FPTNegotiationRoundVM.Prop.StartDateTimeStamp.MapAlias);
				m_lstSelect.Add(FPTNegotiationRoundVM.Prop.EndDateTimeStamp.MapAlias);

				Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
				foreach (DataSorter m_dtsOrder in parameters.Sort)
					m_dicOrder.Add(FPTNegotiationRoundVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

				m_dicDFPTNegotiationRoundsDA = m_objDFPTNegotiationRoundsDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
				if (m_objDFPTNegotiationRoundsDA.Message == string.Empty)
				{
					m_lstFPTNegotiationRoundVM = (
						from DataRow m_drDFPTNegotiationRoundsDA in m_dicDFPTNegotiationRoundsDA[0].Tables[0].Rows
						select new FPTNegotiationRoundVM()
						{
							RoundID = m_drDFPTNegotiationRoundsDA[FPTNegotiationRoundVM.Prop.RoundID.Name].ToString(),
							FPTID = m_drDFPTNegotiationRoundsDA[FPTNegotiationRoundVM.Prop.FPTID.Name].ToString(),
							StartDateTimeStamp = Convert.ToDateTime(m_drDFPTNegotiationRoundsDA[FPTNegotiationRoundVM.Prop.StartDateTimeStamp.Name].ToString()),
							EndDateTimeStamp = Convert.ToDateTime(m_drDFPTNegotiationRoundsDA[FPTNegotiationRoundVM.Prop.EndDateTimeStamp.Name].ToString())
						}
					).ToList();
				}
			}
			return this.Store(m_lstFPTNegotiationRoundVM, m_intCount);
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


			FPTNegotiationRoundVM m_objFPTNegotiationRoundVM = new FPTNegotiationRoundVM();
			//m_objFPTNegotiationRoundVM.ListFPTNegotiationRoundVM = new List<FPTNegotiationRoundVM>();

			ViewDataDictionary m_vddFPTNegotiationRound = new ViewDataDictionary();
			m_vddFPTNegotiationRound.Add(General.EnumDesc(Params.Caller), Caller);
			m_vddFPTNegotiationRound.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));

			this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
			if (Caller == General.EnumDesc(Buttons.ButtonDetail))
			{
				NameValueCollection m_nvcParams = this.Request.Params;
				Dictionary<string, object> m_dicSelectedRow = GetFormData(m_nvcParams);
				Session[dataSessionName] = JSON.Serialize(m_dicSelectedRow);
			}
			return new XMVC.PartialViewResult
			{
				ClearContainer = true,
				ContainerId = General.EnumName(Params.PageOne),
				Model = m_objFPTNegotiationRoundVM,
				RenderMode = RenderMode.AddTo,
				ViewData = m_vddFPTNegotiationRound,
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
			else if (Caller == General.EnumDesc(Buttons.ButtonAdd) || Caller == General.EnumDesc(Buttons.ButtonUpdate))
			{
				NameValueCollection m_nvcParams = this.Request.Params;
				m_dicSelectedRow = GetFormData(m_nvcParams);
			}


			FPTNegotiationRoundVM m_objFPTNegotiationRoundVM = new FPTNegotiationRoundVM();
			if (m_dicSelectedRow.Count > 0)
				m_objFPTNegotiationRoundVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
			if (m_strMessage != string.Empty)
			{
				Global.ShowErrorAlert(title, m_strMessage);
				return this.Direct();
			}



			m_objFPTNegotiationRoundVM.ListNegotiationRoundsVM = GetListFPTNegotiationRound(m_objFPTNegotiationRoundVM.FPTID, ref m_strMessage);
			if (m_strMessage != string.Empty)
			{
				m_objFPTNegotiationRoundVM.ListNegotiationRoundsVM = new List<FPTNegotiationRoundVM>();
				//Global.ShowErrorAlert(title, m_strMessage);
				//return this.Direct();
			}

			int m_iIcon = (int)Icon.PlayGreen;
			string m_strButton = "Start";
			string m_strFuncJS = "beforeStartConfirm";

			if (m_objFPTNegotiationRoundVM.ListNegotiationRoundsVM.Any(d => d.Status.Contains("Running")))
			{
				m_strButton = "Stop";
				m_iIcon = (int)Icon.StopRed;
				m_strFuncJS = "beforeStopConfirm";
			}


			ViewDataDictionary m_vddFPTNegotiationRound = new ViewDataDictionary();
			m_vddFPTNegotiationRound.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
			m_vddFPTNegotiationRound.Add("Button", m_strButton);
			m_vddFPTNegotiationRound.Add("Icon", m_iIcon);
			m_vddFPTNegotiationRound.Add("Fn", m_strFuncJS);
			List<ConfigVM> m_lstConfig = GetConfig("Negotiation");
			if (m_lstConfig.Any())
			{
				m_vddFPTNegotiationRound.Add(FPTNegotiationRoundVM.Prop.TopValue.Name, m_lstConfig.FirstOrDefault(d => d.Key2.Equals(FPTNegotiationRoundVM.Prop.TopValue.Name)).Desc1);
				m_vddFPTNegotiationRound.Add(FPTNegotiationRoundVM.Prop.BottomValue.Name, m_lstConfig.FirstOrDefault(d => d.Key2.Equals(FPTNegotiationRoundVM.Prop.BottomValue.Name)).Desc1);
			}
			this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
			return new XMVC.PartialViewResult
			{
				ClearContainer = true,
				ContainerId = General.EnumName(Params.PageOne),
				Model = m_objFPTNegotiationRoundVM,
				RenderMode = RenderMode.AddTo,
				ViewData = m_vddFPTNegotiationRound,
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
			FPTNegotiationRoundVM m_objFPTNegotiationRoundVM = new FPTNegotiationRoundVM();
			if (m_dicSelectedRow.Count > 0)
				m_objFPTNegotiationRoundVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
			if (m_strMessage != string.Empty)
			{
				Global.ShowErrorAlert(title, m_strMessage);
				return this.Direct();
			}



			m_objFPTNegotiationRoundVM.ListNegotiationRoundsVM = GetListFPTNegotiationRound(m_objFPTNegotiationRoundVM.FPTID, ref m_strMessage);
			if (m_strMessage != string.Empty)
			{
				m_objFPTNegotiationRoundVM.ListNegotiationRoundsVM = new List<FPTNegotiationRoundVM>();
				//Global.ShowErrorAlert(title, m_strMessage);
				//return this.Direct();
			}

			int m_iIcon = (int)Icon.PlayGreen;
			string m_strButton = "Start";
			string m_strFuncJS = "beforeStartConfirm";

			if (m_objFPTNegotiationRoundVM.ListNegotiationRoundsVM.Any(d => d.Status.Contains("Running")))
			{
				m_strButton = "Stop";
				m_iIcon = (int)Icon.StopRed;
				m_strFuncJS = "beforeStopConfirm";
			}

			ViewDataDictionary m_vddFPTNegotiationRound = new ViewDataDictionary();
			m_vddFPTNegotiationRound.Add(General.EnumDesc(Params.Caller), Caller);
			m_vddFPTNegotiationRound.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
			m_vddFPTNegotiationRound.Add("Button", m_strButton);
			m_vddFPTNegotiationRound.Add("Icon", m_iIcon);
			m_vddFPTNegotiationRound.Add("Fn", m_strFuncJS);
			List<ConfigVM> m_lstConfig = GetConfig("Negotiation");
			if (m_lstConfig.Any())
			{
				m_vddFPTNegotiationRound.Add(FPTNegotiationRoundVM.Prop.TopValue.Name, m_lstConfig.FirstOrDefault(d => d.Key2.Equals(FPTNegotiationRoundVM.Prop.TopValue.Name)).Desc1);
				m_vddFPTNegotiationRound.Add(FPTNegotiationRoundVM.Prop.BottomValue.Name, m_lstConfig.FirstOrDefault(d => d.Key2.Equals(FPTNegotiationRoundVM.Prop.BottomValue.Name)).Desc1);
			}
			this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;

			return new XMVC.PartialViewResult
			{
				ClearContainer = true,
				ContainerId = General.EnumName(Params.PageOne),
				Model = m_objFPTNegotiationRoundVM,
				RenderMode = RenderMode.AddTo,
				ViewData = m_vddFPTNegotiationRound,
				ViewName = "_Form",
				WrapByScriptTag = false
			};
		}

		//public ActionResult Delete(string Selected)
		//{
		//    if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
		//        return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

		//    List<FPTNegotiationRoundVM> m_lstSelectedRow = new List<FPTNegotiationRoundVM>();
		//    m_lstSelectedRow = JSON.Deserialize<List<FPTNegotiationRoundVM>>(Selected);

		//    DFPTNegotiationRoundsDA m_objDFPTNegotiationRoundsDA = new DFPTNegotiationRoundsDA();
		//    //DFPTNegotiationRoundsDA d_objDFPTNegotiationRoundParameter = new DFPTNegotiationRoundsDA();
		//    //d_objDFPTNegotiationRoundParameter.ConnectionStringName = Global.ConnStrConfigName;
		//    m_objDFPTNegotiationRoundsDA.ConnectionStringName = Global.ConnStrConfigName;
		//    List<string> m_lstMessage = new List<string>();
		//    object m_objDBConnection = null;
		//    string m_strTransName = "DeleteFPTNegotiationRound";
		//    try
		//    {
		//        foreach (FPTNegotiationRoundVM m_objFPTNegotiationRoundVM in m_lstSelectedRow)
		//        {
		//            List<string> m_lstKey = new List<string>();
		//            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
		//            PropertyInfo[] m_arrPifFPTNegotiationRoundVM = m_objFPTNegotiationRoundVM.GetType().GetProperties();

		//            foreach (PropertyInfo m_pifFPTNegotiationRoundVM in m_arrPifFPTNegotiationRoundVM)
		//            {
		//                string m_strFieldName = m_pifFPTNegotiationRoundVM.Name;
		//                object m_objFieldValue = m_pifFPTNegotiationRoundVM.GetValue(m_objFPTNegotiationRoundVM);
		//                if (m_objFPTNegotiationRoundVM.IsKey(m_strFieldName))
		//                {
		//                    m_lstKey.Add(m_objFieldValue.ToString());
		//                    List<object> m_lstFilter = new List<object>();
		//                    m_lstFilter.Add(Operator.Equals);
		//                    m_lstFilter.Add(m_objFieldValue);
		//                    m_objFilter.Add(FPTNegotiationRoundVM.Prop.Map(m_strFieldName, false), m_lstFilter);
		//                }
		//                else break;
		//            }


		//            m_objDBConnection = m_objDFPTNegotiationRoundsDA.BeginTrans(m_strTransName);

		//            m_objDFPTNegotiationRoundsDA.DeleteBC(m_objFilter, true, m_objDBConnection);

		//            Dictionary<string, List<object>> m_objFilterParameter = new Dictionary<string, List<object>>();
		//            List<object> m_lstFilterParameter = new List<object>();
		//            m_lstFilterParameter.Add(Operator.Equals);
		//            //m_lstFilterParameter.Add(m_objFPTNegotiationRoundVM.FPTNegotiationRoundID);

		//           // m_objFilterParameter.Add(FPTNegotiationRoundVM.Prop.Map(FPTNegotiationRoundVM.Prop.FPTNegotiationRoundID.Name, false), m_lstFilterParameter);
		//            //d_objDFPTNegotiationRoundParameter.DeleteBC(m_objFilterParameter, true, m_objDBConnection);

		//            //if (m_objDFPTNegotiationRoundsDA.Message != string.Empty && d_objDFPTNegotiationRoundParameter.Message != string.Empty)
		//            //{
		//            //    m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDFPTNegotiationRoundsDA.Message);
		//           //     m_objDFPTNegotiationRoundsDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
		//           // }
		//           // else
		//            //    m_objDFPTNegotiationRoundsDA.EndTrans(ref m_objDBConnection, true, m_strTransName);

		//        }
		//    }
		//    catch (Exception ex)
		//    {
		//        m_lstMessage.Add(ex.Message);
		//        m_objDFPTNegotiationRoundsDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
		//    }
		//    if (m_lstMessage.Count <= 0) {                 
		//        Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Deleted));
		//        return this.Direct(true, string.Empty);
		//    }
		//    else
		//        return this.Direct(false, String.Join(Global.NewLineSeparated, m_lstMessage));
		//        //Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));


		//    //return this.Direct();
		//}
		//public ActionResult DeleteParameter(string FPTNegotiationRoundID, string ParameterDesc, string ParameterID)
		//{
		//    if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
		//        return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

		//    //List<FPTNegotiationRoundVM> m_lstSelectedRow = new List<FPTNegotiationRoundVM>();
		//    //m_lstSelectedRow = JSON.Deserialize<List<FPTNegotiationRoundVM>>(Selected);
		//    bool success = false;
		//    if (!string.IsNullOrEmpty(FPTNegotiationRoundID))
		//    {
		//        Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, "Can't delete existing Parameter"));
		//        success = false;
		//    }
		//    else
		//    {
		//        Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Deleted));
		//        success = true;
		//    }

		//    return this.Direct(success);
		//}
		public ActionResult Browse(string ControlFPTNegotiationRoundID, string ControlFPTNegotiationRoundDesc, string FilterFPTNegotiationRoundID = "", string FilterFPTNegotiationRoundDesc = "")
		{
			if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
				return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

			ViewDataDictionary m_vddFPTNegotiationRound = new ViewDataDictionary();
			m_vddFPTNegotiationRound.Add("Control" + FPTNegotiationRoundVM.Prop.RoundID.Name, ControlFPTNegotiationRoundID);
			m_vddFPTNegotiationRound.Add("Control" + FPTNegotiationRoundVM.Prop.FPTID.Name, ControlFPTNegotiationRoundDesc);
			m_vddFPTNegotiationRound.Add(FPTNegotiationRoundVM.Prop.RoundID.Name, FilterFPTNegotiationRoundID);
			m_vddFPTNegotiationRound.Add(FPTNegotiationRoundVM.Prop.FPTID.Name, FilterFPTNegotiationRoundDesc);

			return new XMVC.PartialViewResult
			{
				RenderMode = RenderMode.Auto,
				WrapByScriptTag = false,
				ViewData = m_vddFPTNegotiationRound,
				ViewName = "../FPTNegotiationRound/_Browse"
			};
		}

		public ActionResult Start(string Action)
		{
			if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
				: HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
				return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

			List<string> m_lstMessage = new List<string>();
			DFPTNegotiationRoundsDA m_objDFPTNegotiationRoundsDA = new DFPTNegotiationRoundsDA();
			DFPTVendorParticipantsDA m_objDFPTVendorParticipantsDA = new DFPTVendorParticipantsDA();

            DFPTTCParticipantsDA m_objDFPTTCParticipantsDA = new DFPTTCParticipantsDA();
            DFPTNegotiationRounds m_objDFPTNegotiationRounds = new DFPTNegotiationRounds();
			m_objDFPTNegotiationRoundsDA.ConnectionStringName = Global.ConnStrConfigName;
			m_objDFPTVendorParticipantsDA.ConnectionStringName = Global.ConnStrConfigName;
            m_objDFPTTCParticipantsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<FPTNegotiationRoundVM> m_lstFPTNegotiationRoundVM = new List<FPTNegotiationRoundVM>();

            string m_strFPTID = string.Empty;
			string m_strDuration = string.Empty;
			string m_RoundID = string.Empty;
			string m_lstVendorParticipant = string.Empty;
            string m_lstTCParticipant = string.Empty;
            double m_RoundTime = 0;
			decimal m_decTopValue = 0;
			decimal m_decBottomValue = 0;

			string m_strTransName = "Round";
			object m_objDBConnection = null;
			m_objDBConnection = m_objDFPTNegotiationRoundsDA.BeginTrans(m_strTransName);

			try
			{
				m_strFPTID = this.Request.Params[FPTNegotiationRoundVM.Prop.FPTID.Name];
				m_strDuration = this.Request.Params[FPTNegotiationRoundVM.Prop.Duration.Name];
				m_lstVendorParticipant = this.Request.Params[FPTNegotiationRoundVM.Prop.ListVendorParticipant.Name];
                m_lstTCParticipant = this.Request.Params[FPTNegotiationRoundVM.Prop.ListTCParticipant.Name];

                m_decTopValue = decimal.Parse(string.IsNullOrEmpty(this.Request.Params[FPTNegotiationRoundVM.Prop.TopValue.Name].ToString()) ? "0" : this.Request.Params[FPTNegotiationRoundVM.Prop.TopValue.Name].ToString());
				m_decBottomValue = decimal.Parse(string.IsNullOrEmpty(this.Request.Params[FPTNegotiationRoundVM.Prop.BottomValue.Name].ToString()) ? "0" : this.Request.Params[FPTNegotiationRoundVM.Prop.BottomValue.Name].ToString());

				List<FPTVendorParticipantsVM> m_lstFPTVendorParticipantsVM = JSON.Deserialize<List<FPTVendorParticipantsVM>>(m_lstVendorParticipant);
                List<FPTTCParticipantsVM> m_lstFPTTCParticipantsVM = JSON.Deserialize<List<FPTTCParticipantsVM>>(m_lstTCParticipant);

                m_lstMessage = IsSaveValid(Action, m_lstFPTVendorParticipantsVM);
				if (m_lstMessage.Count <= 0)
				{
					string m_strMessage = string.Empty;
					DateTime m_dtInvalid = new DateTime(9999, 12, 31, 0, 0, 0);
					DateTime m_currTime = DateTime.Now;

					m_lstFPTNegotiationRoundVM = GetListFPTNegotiationRound(m_strFPTID, ref m_strMessage);
					if (m_strMessage != string.Empty)
					{
						Global.ShowErrorAlert(title, m_strMessage);
						return this.Direct(true);
					}

					List<string> success_status = new List<string>();
					int m_iRoundToUpdate = 0;
					if (m_lstFPTNegotiationRoundVM.Any(d => d.StartDateTimeStamp == m_dtInvalid && d.EndDateTimeStamp == m_dtInvalid))
					{
						foreach (FPTNegotiationRoundVM objFPTNegotiationRoundVM in m_lstFPTNegotiationRoundVM)
						{

							m_objDFPTNegotiationRounds.RoundID = objFPTNegotiationRoundVM.RoundID;
							m_objDFPTNegotiationRoundsDA.Data = m_objDFPTNegotiationRounds;
							if (Action != General.EnumDesc(Buttons.ButtonAdd))
								m_objDFPTNegotiationRoundsDA.Select();

							m_objDFPTNegotiationRounds.TopValue = m_decTopValue;
							m_objDFPTNegotiationRounds.BottomValue = m_decBottomValue;
							if (objFPTNegotiationRoundVM.StartDateTimeStamp == m_dtInvalid && objFPTNegotiationRoundVM.EndDateTimeStamp == m_dtInvalid && m_iRoundToUpdate == 0)
							{
								m_objDFPTNegotiationRounds.FPTID = m_strFPTID;
								m_objDFPTNegotiationRounds.StartDateTimeStamp = DateTime.Now;
								m_objDFPTNegotiationRounds.EndDateTimeStamp = m_objDFPTNegotiationRounds.StartDateTimeStamp.AddMinutes(Convert.ToDouble(m_strDuration)).AddSeconds(2);

								m_RoundID = objFPTNegotiationRoundVM.RoundID;
								m_RoundTime = Convert.ToDouble(m_strDuration) * 60;// (m_objDFPTNegotiationRounds.EndDateTimeStamp- DateTime.Now).TotalSeconds;
								m_iRoundToUpdate++;
							}

							m_objDFPTNegotiationRoundsDA.Update(true, m_objDBConnection);


							if (!m_objDFPTNegotiationRoundsDA.Success || m_objDFPTNegotiationRoundsDA.Message != string.Empty || m_lstMessage.Any())
							{
								m_lstMessage.Add(m_objDFPTNegotiationRoundsDA.Message);
								m_objDFPTNegotiationRoundsDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
							}


						}

						#region DFPTVendorParticipant
						if (m_lstFPTVendorParticipantsVM.Any())
						{
							if (Action == General.EnumDesc(Buttons.ButtonUpdate))
							{
								List<NegotiationConfigurationsVM> m_lstNegotiationConfigurationsVM = GetListNegoConfiguration(m_strFPTID, General.EnumDesc(NegoConfigTypes.BudgetPlan), ref m_strMessage);

								Dictionary<string, List<object>> m_dicSet = new Dictionary<string, List<object>>();
								Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
								List<object> m_lstFilter = new List<object>();
								m_lstFilter.Add(Operator.In);
								m_lstFilter.Add(string.Join(",", m_lstNegotiationConfigurationsVM.Select(d => d.NegotiationConfigID)));
								m_objFilter.Add(FPTVendorParticipantsVM.Prop.NegotiationConfigID.Map, m_lstFilter);

								string[] arr = m_lstFPTVendorParticipantsVM
									 .Select(x => x.VendorID)
									 .ToArray();
								m_lstFilter = new List<object>();
								m_lstFilter.Add(Operator.NotIn);
								m_lstFilter.Add(string.Join(",", arr));
								m_objFilter.Add(FPTVendorParticipantsVM.Prop.VendorID.Map, m_lstFilter);

								List<object> m_lstSet = new List<object>();
								m_lstSet.Add(typeof(int));
								m_lstSet.Add(0);
								m_dicSet.Add(FPTVendorParticipantsVM.Prop.StatusID.Map, m_lstSet);

								m_objDFPTVendorParticipantsDA.UpdateBC(m_dicSet, m_objFilter, true, m_objDBConnection);
								if (!m_objDFPTVendorParticipantsDA.Success)
								{
									m_lstMessage.Add(m_objDFPTVendorParticipantsDA.Message);
									m_objDFPTNegotiationRoundsDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
								}

								m_dicSet = new Dictionary<string, List<object>>();
								m_lstFilter = new List<object>();
								m_objFilter = new Dictionary<string, List<object>>();

								m_lstFilter.Add(Operator.Equals);
								m_lstFilter.Add(Operator.In);
								m_lstFilter.Add(string.Join(",", m_lstNegotiationConfigurationsVM.Select(d => d.NegotiationConfigID)));

								arr = m_lstFPTVendorParticipantsVM
									.Select(x => x.VendorID)
									.ToArray();
								m_lstFilter = new List<object>();
								m_lstFilter.Add(Operator.In);
								m_lstFilter.Add(string.Join(",", arr));
								m_objFilter.Add(FPTVendorParticipantsVM.Prop.VendorID.Map, m_lstFilter);

								m_lstFilter = new List<object>();
								m_lstFilter.Add(Operator.In);
								m_lstFilter.Add(string.Join(",", m_lstNegotiationConfigurationsVM.Select(d => d.NegotiationConfigID)));
								m_objFilter.Add(FPTVendorParticipantsVM.Prop.NegotiationConfigID.Map, m_lstFilter);

								m_lstSet = new List<object>();
								m_lstSet.Add(typeof(int));
								m_lstSet.Add(1);
								m_dicSet.Add(FPTVendorParticipantsVM.Prop.StatusID.Map, m_lstSet);

								m_objDFPTVendorParticipantsDA.UpdateBC(m_dicSet, m_objFilter, true, m_objDBConnection);
								if (!m_objDFPTVendorParticipantsDA.Success)
								{
									m_lstMessage.Add(m_objDFPTVendorParticipantsDA.Message);
									m_objDFPTNegotiationRoundsDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
								}

							}



						}
                        #endregion

                        #region DFPTTCParticipant
                        if (m_lstFPTTCParticipantsVM.Any())
                        {
                            if (Action == General.EnumDesc(Buttons.ButtonUpdate))
                            {
                                List<NegotiationConfigurationsVM> m_lstNegotiationConfigurationsVM = GetListNegoConfiguration(m_strFPTID, General.EnumDesc(NegoConfigTypes.BudgetPlan), ref m_strMessage);

                                Dictionary<string, List<object>> m_dicSet = new Dictionary<string, List<object>>();
                                Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                                List<object> m_lstFilter = new List<object>();
                                m_lstFilter.Add(Operator.In);
                                m_lstFilter.Add(m_strFPTID);
                                m_objFilter.Add(FPTTCParticipantsVM.Prop.FPTID.Map, m_lstFilter);

                                string[] arr = m_lstFPTTCParticipantsVM
                                     .Select(x => x.TCMemberID)
                                     .ToArray();
                                m_lstFilter = new List<object>();
                                m_lstFilter.Add(Operator.NotIn);
                                m_lstFilter.Add(string.Join(",", arr));
                                m_objFilter.Add(FPTTCParticipantsVM.Prop.TCMemberID.Map, m_lstFilter);

                                List<object> m_lstSet = new List<object>();
                                m_lstSet.Add(typeof(int));
                                m_lstSet.Add(0);
                                m_dicSet.Add(FPTTCParticipantsVM.Prop.StatusID.Map, m_lstSet);

                                m_objDFPTTCParticipantsDA.UpdateBC(m_dicSet, m_objFilter, true, m_objDBConnection);
                                if (!m_objDFPTTCParticipantsDA.Success)
                                {
                                    m_lstMessage.Add(m_objDFPTTCParticipantsDA.Message);
                                    m_objDFPTNegotiationRoundsDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                                }

                                m_dicSet = new Dictionary<string, List<object>>();
                                m_lstFilter = new List<object>();
                                m_objFilter = new Dictionary<string, List<object>>();

                                m_lstFilter.Add(Operator.Equals);
                                m_lstFilter.Add(Operator.In);
                                m_lstFilter.Add(m_strFPTID);

                                arr = m_lstFPTTCParticipantsVM
                                    .Select(x => x.TCMemberID)
                                    .ToArray();
                                m_lstFilter = new List<object>();
                                m_lstFilter.Add(Operator.In);
                                m_lstFilter.Add(string.Join(",", arr));
                                m_objFilter.Add(FPTTCParticipantsVM.Prop.TCMemberID.Map, m_lstFilter);

                                m_lstFilter = new List<object>();
                                m_lstFilter.Add(Operator.In);
                                m_lstFilter.Add(m_strFPTID);
                                m_objFilter.Add(FPTTCParticipantsVM.Prop.FPTID.Map, m_lstFilter);

                                m_lstSet = new List<object>();
                                m_lstSet.Add(typeof(int));
                                m_lstSet.Add(1);
                                m_dicSet.Add(FPTTCParticipantsVM.Prop.StatusID.Map, m_lstSet);

                                m_objDFPTTCParticipantsDA.UpdateBC(m_dicSet, m_objFilter, true, m_objDBConnection);
                                if (!m_objDFPTTCParticipantsDA.Success)
                                {
                                    m_lstMessage.Add(m_objDFPTTCParticipantsDA.Message);
                                    m_objDFPTNegotiationRoundsDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                                }

                            }



                        }
                        #endregion

                        if (!m_objDFPTNegotiationRoundsDA.Success || m_objDFPTNegotiationRoundsDA.Message != string.Empty || m_lstMessage.Any())
							m_lstMessage.Add(m_objDFPTNegotiationRoundsDA.Message);
						else
							m_objDFPTNegotiationRoundsDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
					}
					else
					{
						Global.ShowErrorAlert(title, "Sorry, there aren't an active round to be started");
						return this.Direct(true);
					}

				}
			}
			catch (Exception ex)
			{
				m_lstMessage.Add(ex.Message);
				m_objDFPTNegotiationRoundsDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
			}
			if (m_lstMessage.Count <= 0)
			{
				Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
				NegotiationHub.BrodcastActivatedRound(m_strFPTID, m_RoundID, m_RoundTime, m_decTopValue, m_decBottomValue);
				return Detail(Action, null);
			}
			else
			{
				Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
				return this.Direct(true);
			}
		}

		public ActionResult Stop(string Action)
		{
			if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
				: HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
				return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

			List<string> m_lstMessage = new List<string>();
			DFPTNegotiationRoundsDA m_objDFPTNegotiationRoundsDA = new DFPTNegotiationRoundsDA();
			DFPTVendorParticipantsDA m_objDFPTVendorParticipantsDA = new DFPTVendorParticipantsDA();
			DFPTNegotiationRounds m_objDFPTNegotiationRounds = new DFPTNegotiationRounds();
			m_objDFPTNegotiationRoundsDA.ConnectionStringName = Global.ConnStrConfigName;
			m_objDFPTVendorParticipantsDA.ConnectionStringName = Global.ConnStrConfigName;

			List<FPTNegotiationRoundVM> m_lstFPTNegotiationRoundVM = new List<FPTNegotiationRoundVM>();
			string m_strFPTID = string.Empty;
			string m_strDuration = string.Empty;
			string m_RoundID = string.Empty;
			string m_lstVendorParticipant = string.Empty;
			double m_RoundTime = 0;
			string m_Remarks = string.Empty;

			string m_strTransName = "Round";
			object m_objDBConnection = null;
			m_objDBConnection = m_objDFPTNegotiationRoundsDA.BeginTrans(m_strTransName);

			try
			{
				m_strFPTID = this.Request.Params[FPTNegotiationRoundVM.Prop.FPTID.Name];
				m_strDuration = this.Request.Params[FPTNegotiationRoundVM.Prop.Duration.Name];
				m_lstVendorParticipant = this.Request.Params[FPTNegotiationRoundVM.Prop.ListVendorParticipant.Name];
				m_Remarks = this.Request.Params[FPTNegotiationRoundVM.Prop.Remarks.Name];
				List<FPTVendorParticipantsVM> m_lstFPTVendorParticipantsVM = JSON.Deserialize<List<FPTVendorParticipantsVM>>(m_lstVendorParticipant);

				m_lstMessage = IsSaveValid(Action, m_lstFPTVendorParticipantsVM);
				if (m_lstMessage.Count <= 0)
				{
					string m_strMessage = string.Empty;
					DateTime m_dtInvalid = new DateTime(9999, 12, 31, 0, 0, 0);
					DateTime m_currTime = DateTime.Now;

					m_lstFPTNegotiationRoundVM = GetListFPTNegotiationRound(m_strFPTID, ref m_strMessage);
					if (m_strMessage != string.Empty)
					{
						Global.ShowErrorAlert(title, m_strMessage);
						return this.Direct(true);
					}

					List<string> success_status = new List<string>();

					if (m_lstFPTNegotiationRoundVM.Any(d => d.Status.Contains("Running")))
					{
						FPTNegotiationRoundVM objFPTNegotiationRoundVM = m_lstFPTNegotiationRoundVM.FirstOrDefault(d => d.Status.Contains("Running"));
						//foreach (FPTNegotiationRoundVM objFPTNegotiationRoundVM in m_lstFPTNegotiationRoundVM)
						//{//
						//if (objFPTNegotiationRoundVM.StartDateTimeStamp <= m_currTime && objFPTNegotiationRoundVM.EndDateTimeStamp >= m_currTime)
						//{
						//    Global.ShowErrorAlert(title, "Cannot start the next round because the Round has been activated");
						//    return this.Direct(true);
						//}


						if (objFPTNegotiationRoundVM.StartDateTimeStamp != m_dtInvalid && objFPTNegotiationRoundVM.EndDateTimeStamp != m_dtInvalid)
						{

							m_objDFPTNegotiationRounds.RoundID = objFPTNegotiationRoundVM.RoundID;
							m_objDFPTNegotiationRoundsDA.Data = m_objDFPTNegotiationRounds;

							if (Action != General.EnumDesc(Buttons.ButtonAdd))
								m_objDFPTNegotiationRoundsDA.Select();

							m_objDFPTNegotiationRounds.FPTID = m_strFPTID;
							m_objDFPTNegotiationRounds.Remarks = m_Remarks;
							m_objDFPTNegotiationRounds.EndDateTimeStamp = DateTime.Now;

							m_objDFPTNegotiationRoundsDA.Update(true, m_objDBConnection);


							if (!m_objDFPTNegotiationRoundsDA.Success || m_objDFPTNegotiationRoundsDA.Message != string.Empty || m_lstMessage.Any())
							{
								m_lstMessage.Add(m_objDFPTNegotiationRoundsDA.Message);
								m_objDFPTNegotiationRoundsDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
							}

							m_RoundID = objFPTNegotiationRoundVM.RoundID;
							m_RoundTime = 1;// (m_objDFPTNegotiationRounds.EndDateTimeStamp- DateTime.Now).TotalSeconds;

							//break;
						}

						//}



						if (!m_objDFPTNegotiationRoundsDA.Success || m_objDFPTNegotiationRoundsDA.Message != string.Empty || m_lstMessage.Any())
							m_lstMessage.Add(m_objDFPTNegotiationRoundsDA.Message);
						else
							m_objDFPTNegotiationRoundsDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
					}
					else
					{
						Global.ShowErrorAlert(title, "Sorry, there aren't an active round to be stopped");
						return this.Direct(true);
					}

				}
			}
			catch (Exception ex)
			{
				m_lstMessage.Add(ex.Message);
				m_objDFPTNegotiationRoundsDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
			}
			if (m_lstMessage.Count <= 0)
			{
				Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
				NegotiationHub.BrodcastDeactivatedRound(m_strFPTID, m_RoundID, m_RoundTime);
				return Detail(Action, null);
			}
			else
			{
				Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
				return this.Direct(true);
			}
		}

		public ActionResult CloseRemainingRound(string Action)
		{
			if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
				: HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
				return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

			List<string> m_lstMessage = new List<string>();
			DFPTNegotiationRoundsDA m_objDFPTNegotiationRoundsDA = new DFPTNegotiationRoundsDA();
			DFPTVendorParticipantsDA m_objDFPTVendorParticipantsDA = new DFPTVendorParticipantsDA();
			DFPTNegotiationRounds m_objDFPTNegotiationRounds = new DFPTNegotiationRounds();
			m_objDFPTNegotiationRoundsDA.ConnectionStringName = Global.ConnStrConfigName;
			m_objDFPTVendorParticipantsDA.ConnectionStringName = Global.ConnStrConfigName;

			List<FPTNegotiationRoundVM> m_lstFPTNegotiationRoundVM = new List<FPTNegotiationRoundVM>();
			string m_strFPTID = string.Empty;
			string m_strDuration = string.Empty;
			string m_RoundID = string.Empty;
			string m_lstVendorParticipant = string.Empty;
			double m_RoundTime = 0;
			string m_Remarks = string.Empty;

			string m_strTransName = "Round";
			object m_objDBConnection = null;
			m_objDBConnection = m_objDFPTNegotiationRoundsDA.BeginTrans(m_strTransName);

			try
			{
				m_strFPTID = this.Request.Params[FPTNegotiationRoundVM.Prop.FPTID.Name];
				m_strDuration = this.Request.Params[FPTNegotiationRoundVM.Prop.Duration.Name];
				m_lstVendorParticipant = this.Request.Params[FPTNegotiationRoundVM.Prop.ListVendorParticipant.Name];
				m_Remarks = this.Request.Params[FPTNegotiationRoundVM.Prop.Remarks.Name];
				List<FPTVendorParticipantsVM> m_lstFPTVendorParticipantsVM = JSON.Deserialize<List<FPTVendorParticipantsVM>>(m_lstVendorParticipant);

				m_lstMessage = IsSaveValid(Action, m_lstFPTVendorParticipantsVM);
				if (m_lstMessage.Count <= 0)
				{
					string m_strMessage = string.Empty;
					DateTime m_dtInvalid = new DateTime(9999, 12, 31, 0, 0, 0);
					DateTime m_currTime = DateTime.Now;

					m_lstFPTNegotiationRoundVM = GetListFPTNegotiationRound(m_strFPTID, ref m_strMessage);
					if (m_strMessage != string.Empty)
					{
						Global.ShowErrorAlert(title, m_strMessage);
						return this.Direct(true);
					}

					List<string> success_status = new List<string>();


					if (m_lstFPTNegotiationRoundVM.Any(d => d.Status.Contains("Running")))
					{
						Global.ShowErrorAlert(title, "Please stop the active/running round!");
						return this.Direct(true);
					}
					int m_intSecond = 1;
					//FPTNegotiationRoundVM objFPTNegotiationRoundVM = m_lstFPTNegotiationRoundVM.FirstOrDefault(d => d.Status.Contains("Running"));
					if (m_lstFPTNegotiationRoundVM.Any(d => d.StartDateTimeStamp != m_dtInvalid))
					{

						foreach (FPTNegotiationRoundVM objFPTNegotiationRoundVM in m_lstFPTNegotiationRoundVM)
						{


							if (objFPTNegotiationRoundVM.StartDateTimeStamp == m_dtInvalid && objFPTNegotiationRoundVM.EndDateTimeStamp == m_dtInvalid)
							{

								m_objDFPTNegotiationRounds.RoundID = objFPTNegotiationRoundVM.RoundID;
								m_objDFPTNegotiationRoundsDA.Data = m_objDFPTNegotiationRounds;

								if (Action != General.EnumDesc(Buttons.ButtonAdd))
									m_objDFPTNegotiationRoundsDA.Select();

								m_objDFPTNegotiationRounds.FPTID = m_strFPTID;
								m_objDFPTNegotiationRounds.Remarks = m_Remarks;
								m_objDFPTNegotiationRounds.StartDateTimeStamp = DateTime.Now.AddSeconds(m_intSecond);
                                m_intSecond++;
                                m_objDFPTNegotiationRounds.EndDateTimeStamp = DateTime.Now.AddSeconds(m_intSecond);
								m_intSecond++;

								m_objDFPTNegotiationRoundsDA.Update(true, m_objDBConnection);


								if (!m_objDFPTNegotiationRoundsDA.Success || m_objDFPTNegotiationRoundsDA.Message != string.Empty || m_lstMessage.Any())
								{
									m_lstMessage.Add(m_objDFPTNegotiationRoundsDA.Message);
									m_objDFPTNegotiationRoundsDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
								}

								//m_RoundID = objFPTNegotiationRoundVM.RoundID;
								//m_RoundTime = 1;
							}
						}
					}
					else
					{
						Global.ShowErrorAlert(title, "Minimum a round was activated!");
						return this.Direct(true);
					}

					if (!m_objDFPTNegotiationRoundsDA.Success || m_objDFPTNegotiationRoundsDA.Message != string.Empty || m_lstMessage.Any())
						m_lstMessage.Add(m_objDFPTNegotiationRoundsDA.Message);
					else
						m_objDFPTNegotiationRoundsDA.EndTrans(ref m_objDBConnection, true, m_strTransName);



				}
			}
			catch (Exception ex)
			{
				m_lstMessage.Add(ex.Message);
				m_objDFPTNegotiationRoundsDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
			}
			if (m_lstMessage.Count <= 0)
			{
				//update status FPT
				string msg = string.Empty;
				InsertDFPTStatus(m_strFPTID, (int)FPTStatusTypes.DoneNegotiation, DateTime.Now, ref msg);
				SyncNegoPrice(m_strFPTID, ref m_lstMessage);
				SyncTRMPrice(m_strFPTID, ref m_lstMessage);

				Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
				//NegotiationHub.BrodcastDeactivatedRound(m_strFPTID, m_RoundID, m_RoundTime);
				return Detail(Action, null);
			}
			else
			{
				Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
				return this.Direct(true);
			}
		}

        public ActionResult GetDelegation(string Selected, string FPTID)
        {
            DFPTTCParticipantsDA m_objDFPTTCParticipantsDA = new DFPTTCParticipantsDA();
            m_objDFPTTCParticipantsDA.ConnectionStringName = Global.ConnStrConfigName;

            TTCMembersDA m_objTTCMembersDA = new TTCMembersDA();
            m_objTTCMembersDA.ConnectionStringName = Global.ConnStrConfigName;

            List<FPTTCParticipantsVM> m_lsFPTTCParticipantsVM = new List<FPTTCParticipantsVM>();
            m_lsFPTTCParticipantsVM = JSON.Deserialize<List<FPTTCParticipantsVM>>(Selected);

            DateTime m_dtNow = DateTime.Now.Date;
            List<TCMembersDelegationVM> m_lsTCMembersDelegationVM = GetListTCMembersDelegation(string.Join(",", m_lsFPTTCParticipantsVM.Select(d => d.TCMemberID).ToArray()));
            m_lsTCMembersDelegationVM = m_lsTCMembersDelegationVM.Where(d => d.DelegateStartDate <= m_dtNow && d.DelegateEndDate >= m_dtNow).ToList();
            FPTTCParticipantsVM m_objFPTTCParticipantsVM = new FPTTCParticipantsVM();
            DFPTTCParticipants m_objDFPTTCParticipants = new DFPTTCParticipants();

            foreach (TCMembersDelegationVM m_TCMembersDelegationVM in m_lsTCMembersDelegationVM)
            {

                if (!m_lsFPTTCParticipantsVM.Any(d => d.TCMemberID == m_TCMembersDelegationVM.DelegateTo))
                {
                    m_objDFPTTCParticipantsDA.Data = m_objDFPTTCParticipants;
                    m_objDFPTTCParticipants.FPTTCParticipantID = Guid.NewGuid().ToString().Replace("-", "");
                    m_objDFPTTCParticipants.FPTID = FPTID;
                    m_objDFPTTCParticipants.TCMemberID = m_TCMembersDelegationVM.DelegateTo;
                    m_objDFPTTCParticipants.StatusID = true;
                    m_objDFPTTCParticipants.IsDelegation = true;

                    m_objDFPTTCParticipantsDA.Insert(false);
                    if (!m_objDFPTTCParticipantsDA.Success && m_objDFPTTCParticipantsDA.Message != string.Empty)
                    {
                        return this.Direct(false, m_objDFPTTCParticipantsDA.Message);
                    }

                    m_lsFPTTCParticipantsVM.Add(new FPTTCParticipantsVM { FPTTCParticipantID = m_objDFPTTCParticipants.FPTTCParticipantID, FPTID = FPTID, TCMemberID = m_TCMembersDelegationVM.DelegateTo, StatusID = true, IsDelegation = true });
                }
            }

            return this.Store(m_lsFPTTCParticipantsVM, m_lsFPTTCParticipantsVM.Count);
        }
        #endregion

        #region Direct Method

        public ActionResult GetFPTNegotiationRound(string ControlFPTNegotiationRoundID, string ControlFPTNegotiationRoundDesc, string FilterFPTNegotiationRoundID, string FilterFPTNegotiationRoundDesc, bool Exact = false)
		{
			try
			{
				Dictionary<int, List<FPTNegotiationRoundVM>> m_dicFPTNegotiationRoundData = GetFPTNegotiationRoundData(true, FilterFPTNegotiationRoundID, FilterFPTNegotiationRoundDesc);
				KeyValuePair<int, List<FPTNegotiationRoundVM>> m_kvpFPTNegotiationRoundVM = m_dicFPTNegotiationRoundData.AsEnumerable().ToList()[0];
				if (m_kvpFPTNegotiationRoundVM.Key < 1 || (m_kvpFPTNegotiationRoundVM.Key > 1 && Exact))
					return this.Direct(false);
				else if (m_kvpFPTNegotiationRoundVM.Key > 1 && !Exact)
					return Browse(ControlFPTNegotiationRoundID, ControlFPTNegotiationRoundDesc, FilterFPTNegotiationRoundID, FilterFPTNegotiationRoundDesc);

				m_dicFPTNegotiationRoundData = GetFPTNegotiationRoundData(false, FilterFPTNegotiationRoundID, FilterFPTNegotiationRoundDesc);
				FPTNegotiationRoundVM m_objFPTNegotiationRoundVM = m_dicFPTNegotiationRoundData[0][0];
				this.GetCmp<TextField>(ControlFPTNegotiationRoundID).Value = m_objFPTNegotiationRoundVM.RoundID;
				this.GetCmp<TextField>(ControlFPTNegotiationRoundDesc).Value = m_objFPTNegotiationRoundVM.FPTID;
				return this.Direct(true);
			}
			catch (Exception ex)
			{
				return this.Direct(false, ex.Message);
			}
		}

		public ActionResult GetListRound(string FPTID)
		{
			string m_strMessage = string.Empty;

			List<FPTNegotiationRoundVM> m_lstFPTNegotiationRoundVM = GetListFPTNegotiationRound(FPTID, ref m_strMessage);

			//update status FPT
			DateTime m_dtInvalid = new DateTime(9999, 12, 31);
			if (!m_lstFPTNegotiationRoundVM.Any(d => d.StartDateTimeStamp == m_dtInvalid && d.EndDateTimeStamp == m_dtInvalid))
			{
				string m_insmessage = string.Empty;
				bool m_boolSuccess = InsertDFPTStatus(FPTID, (int)FPTStatusTypes.DoneNegotiation, DateTime.Now, ref m_insmessage);
			}

			List<string> m_lstMessage = new List<string>();
			SyncNegoPrice(FPTID, ref m_lstMessage);
			SyncTRMPrice(FPTID, ref m_lstMessage);

			if (m_strMessage != string.Empty)
			{
				return this.Direct(false, m_strMessage);
			}
			return this.Direct(m_lstFPTNegotiationRoundVM);
		}
        #endregion

        #region Private Method
        private List<TCMembersDelegationVM> GetListTCMembersDelegation(string TCMemberIDs)
        {
            List<TCMembersDelegationVM> m_listTCMembersDelegationVM = new List<TCMembersDelegationVM>();
            TTCMemberDelegationsDA m_objTTCMemberDelegationsDA = new TTCMemberDelegationsDA();
            m_objTTCMemberDelegationsDA.ConnectionStringName = Global.ConnStrConfigName;

            FilterHeaderConditions m_fhcTTCMembers = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(TCMemberIDs);
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
        private List<FPTVendorParticipantsVM> getFPTVendorParticipantsVM(string FPTID)
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
			m_lstFilter.Add("7777, 8888, 9999");
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
			m_lstSelect.Add(NegotiationBidEntryVM.Prop.BudgetPlanID.MapAlias);
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
		private List<NegotiationBidStructuresVM> getNegoStructure(string FPTID)
		{
			//List Structure FPT
			List<NegotiationBidStructuresVM> m_lstNegotiationBidStructuresVM = new List<NegotiationBidStructuresVM>();
			Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
			List<object> m_lstFilter = new List<object>();
			List<string> m_lstSelect = new List<string>();


			TNegotiationBidStructuresDA m_objTNegotiationBidStructuresDA = new TNegotiationBidStructuresDA();
			m_objTNegotiationBidStructuresDA.ConnectionStringName = Global.ConnStrConfigName;


			//m_lstFilter = new List<object>();
			//m_lstFilter.Add(Operator.In);
			//m_lstFilter.Add(m_strconfig);
			//m_objFilter.Add(NegotiationBidStructuresVM.Prop.NegotiationConfigID.Map, m_lstFilter);

			m_lstFilter = new List<object>();
			m_lstFilter.Add(Operator.Equals);
			m_lstFilter.Add(FPTID);
			m_objFilter.Add(NegotiationBidStructuresVM.Prop.FPTID.Map, m_lstFilter);

			m_lstFilter = new List<object>();
			m_lstFilter.Add(Operator.Equals);
			m_lstFilter.Add((int)VendorBidTypes.AfterFee);
			m_objFilter.Add(NegotiationBidStructuresVM.Prop.Sequence.Map, m_lstFilter);

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
		private bool SyncNegoPrice(string FPTID, ref List<string> message)
		{
			List<FPTVendorParticipantsVM> m_lstFPTVendorParticipantsVM = getFPTVendorParticipantsVM(FPTID);
			//List<FPTVendorRecommendationsVM> m_lstFPTVendorRecommendationsVM = getFPTVendorRecommendationsVM(FPTID);
			//List<FPTVendorWinnerVM> m_lstFPTVendorWinnerVM = getFPTVendorWinnerVM(FPTID);
			List<NegotiationBidEntryVM> m_lstNegotiationBidEntryVM = getNegotiationBidEntryVM(FPTID);


			foreach (var item in m_lstFPTVendorParticipantsVM)
			{
				//item.TCName = string.Join("$", m_lstFPTVendorRecommendationsVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID).Select(x => x.TCMemberName).ToArray());
				//item.RecommendationRemark = string.Join("$", m_lstFPTVendorRecommendationsVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID).Select(x => x.Remarks).ToArray());
				//item.IsProposedWinner = string.Join("$", m_lstFPTVendorRecommendationsVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID).Select(x => x.IsProposed.ToString()).ToArray());
				//item.IsWinner = (m_lstFPTVendorWinnerVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID).Any()) ? m_lstFPTVendorWinnerVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID).FirstOrDefault().IsWinner : false;

				item.BidValue = 0;
				item.BidFee = 0;

				if (m_lstNegotiationBidEntryVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID).Any())
				{
					string m_roundid = m_lstNegotiationBidEntryVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID && x.Sequence == 7777).OrderByDescending(x => x.RoundID).FirstOrDefault().RoundID;
					item.NegotiationEntryID = m_lstNegotiationBidEntryVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID && x.Sequence == 7777).FirstOrDefault().NegotiationEntryID;
					item.BidValue = m_lstNegotiationBidEntryVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID && x.Sequence == 7777).OrderByDescending(x => x.RoundID).FirstOrDefault().BidValue;
					item.BudgetPlanDefaultValue = m_lstNegotiationBidEntryVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID && x.Sequence == 7777).FirstOrDefault().BudgetPlanDefaultValue;
					item.BidFee = m_lstNegotiationBidEntryVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID && x.Sequence == 8888 && x.RoundID == m_roundid).Any() ? m_lstNegotiationBidEntryVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID && x.Sequence == 8888 && x.RoundID == m_roundid).OrderByDescending(x => x.RoundID).FirstOrDefault().BidValue : 0;
					item.BudgetPlanDefaultFee = m_lstNegotiationBidEntryVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID && x.Sequence == 8888).Any() ? m_lstNegotiationBidEntryVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID && x.Sequence == 8888).OrderByDescending(x => x.RoundID).FirstOrDefault().BudgetPlanDefaultValue : 0;
					item.BidAfterFee = item.BidValue * (1 + (item.BidFee / 100));//m_lstNegotiationBidEntryVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID && x.Sequence == 9999).Any() ? m_lstNegotiationBidEntryVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID && x.Sequence == 9999).OrderByDescending(x => x.RoundID).FirstOrDefault().BidValue : 0;
					item.BudgetPlanDefaultValueAfterFee = item.BudgetPlanDefaultValue * (1 + (item.BudgetPlanDefaultFee / 100));//m_lstNegotiationBidEntryVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID && x.Sequence == 9999).Any() ? m_lstNegotiationBidEntryVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID && x.Sequence == 9999).OrderByDescending(x => x.RoundID).FirstOrDefault().BudgetPlanDefaultValue : 0;
					item.LastOffer = m_lstNegotiationBidEntryVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID && x.Sequence == 7777 && string.IsNullOrEmpty(x.RoundID)).Any() ? m_lstNegotiationBidEntryVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID && x.Sequence == 7777 && string.IsNullOrEmpty(x.RoundID)).FirstOrDefault().BidValue : 0;
				}

			}
			string m_lstvendor = string.Empty;
			string m_lstpricebefore = string.Empty;
			string m_lstpriceafter = string.Empty;
			string m_lsttype = string.Empty;

			foreach (var item in m_lstFPTVendorParticipantsVM)
			{
				m_lstvendor += $" <string>{item.VendorName}</string> ";
				m_lstpricebefore += $" <string>{item.LastOffer}</string> ";
				m_lstpriceafter += $" <string>{item.BidAfterFee}</string> ";
				m_lsttype += $" <string>{item.BudgetPlanID}</string> ";
			}


			string m_soapenv = $@"<?xml version='1.0' encoding='utf-8'?>
			<soap:Envelope xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/'>
			  <soap:Body>
				<UpdateHargaNego xmlns='http://tempuri.org/'>
				  <prmRequestNo>{FPTID}</prmRequestNo>
				  <prmNamaVendor>
					{m_lstvendor}
				  </prmNamaVendor>
				  <prmType>
					{m_lsttype}
				  </prmType>
				  <prmPriceBeforeNego>
					{m_lstpricebefore}
				  </prmPriceBeforeNego>
				  <prmPriceAfterNego>
					{m_lstpriceafter}
				  </prmPriceAfterNego>
				  <prmUpdateOnly>0</prmUpdateOnly>
				</UpdateHargaNego>
			  </soap:Body>
			</soap:Envelope>";


			var m_lstconfig = GetConfig("WS", null, "ETT");
			if (!m_lstconfig.Any())
			{
				message.Add("Fail Sync ETT");
				return false;
			}
			string m_struser = m_lstconfig.Any(x => x.Key2 == "Credential" && x.Key4 == "User") ? m_lstconfig.Where(x => x.Key2 == "Credential" && x.Key4 == "User").FirstOrDefault().Desc1 : string.Empty;
			string m_strpass = m_lstconfig.Any(x => x.Key2 == "Credential" && x.Key4 == "Password") ? m_lstconfig.Where(x => x.Key2 == "Credential" && x.Key4 == "Password").FirstOrDefault().Desc1 : string.Empty;
			string m_strdomain = m_lstconfig.Any(x => x.Key2 == "Credential" && x.Key4 == "Domain") ? m_lstconfig.Where(x => x.Key2 == "Credential" && x.Key4 == "Domain").FirstOrDefault().Desc1 : string.Empty;
			string m_strurl = m_lstconfig.Any(x => x.Key2 == "URL" && x.Key4 == "Division") ? m_lstconfig.Where(x => x.Key2 == "URL" && x.Key4 == "Division").FirstOrDefault().Desc1 : string.Empty;
			NetworkCredential m_credential = new NetworkCredential(m_struser, m_strpass, m_strdomain);
			string m_strsoapresult = GetSOAPString(m_soapenv, m_strurl, m_credential);
			if (string.IsNullOrEmpty(m_strsoapresult))
			{
				return false;
			}

			System.Xml.XmlDocument document = new XmlDocument();
			document.LoadXml(m_strsoapresult);
			XmlNamespaceManager manager = new XmlNamespaceManager(document.NameTable);

			XmlNodeList xnList = document.GetElementsByTagName("UpdateHargaNegoResult");
			foreach (XmlNode item in xnList)
			{
				if (item.Name == "UpdateHargaNegoResult" && item.InnerText == "")
				{
					return true;
				}
				else
				{
					message.Add("Fail Sync ETT");
					return false;
				}
			}
			message.Add("Fail Sync ETT");
			return false;

		}
		private bool SyncTRMPrice(string FPTID, ref List<string> message)
		{
			List<NegotiationBidStructuresVM> m_structure = getNegoStructure(FPTID);
			if (!m_structure.Any())
			{
				return false;
			}
			string m_lstprice = string.Empty;
			string m_lstBP = string.Empty;
			foreach (var item in m_structure)
			{
				m_lstprice += $"<string>{item.BudgetPlanDefaultValue}</string> ";
				m_lstBP += $"<string>{item.ParameterValue}</string> ";
			}
			//m_lstprice += "<string>6969</string> ";
			//m_lstBP += "<string>Bptes1</string> ";
			//m_lstprice += "<string>123456</string> ";
			//m_lstBP += "<string>Bptes2</string> ";

			string m_soapenv = $@"<?xml version='1.0' encoding='utf-8'?>
				<soap:Envelope xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/'>
			  <soap:Body>
				<UpdateHargaTRM xmlns='http://tempuri.org/'>
				  <prmRequestNo>{FPTID}</prmRequestNo>
				  <prmHargaTRM>
					{m_lstprice}
				  </prmHargaTRM>
				  <prmDescription>
					{m_lstBP}
				  </prmDescription>
				  <prmUpdateOnly>0</prmUpdateOnly>
				</UpdateHargaTRM>
			  </soap:Body>
			</soap:Envelope>";


			var m_lstconfig = GetConfig("WS", null, "ETT");
			if (!m_lstconfig.Any())
			{
				message.Add("Fail Sync ETT");
				return false;
			}
			string m_struser = m_lstconfig.Any(x => x.Key2 == "Credential" && x.Key4 == "User") ? m_lstconfig.Where(x => x.Key2 == "Credential" && x.Key4 == "User").FirstOrDefault().Desc1 : string.Empty;
			string m_strpass = m_lstconfig.Any(x => x.Key2 == "Credential" && x.Key4 == "Password") ? m_lstconfig.Where(x => x.Key2 == "Credential" && x.Key4 == "Password").FirstOrDefault().Desc1 : string.Empty;
			string m_strdomain = m_lstconfig.Any(x => x.Key2 == "Credential" && x.Key4 == "Domain") ? m_lstconfig.Where(x => x.Key2 == "Credential" && x.Key4 == "Domain").FirstOrDefault().Desc1 : string.Empty;
			string m_strurl = m_lstconfig.Any(x => x.Key2 == "URL" && x.Key4 == "Division") ? m_lstconfig.Where(x => x.Key2 == "URL" && x.Key4 == "Division").FirstOrDefault().Desc1 : string.Empty;
			NetworkCredential m_credential = new NetworkCredential(m_struser, m_strpass, m_strdomain);
			string m_strsoapresult = GetSOAPString(m_soapenv, m_strurl, m_credential);
			if (string.IsNullOrEmpty(m_strsoapresult))
			{
				return false;
			}

			System.Xml.XmlDocument document = new XmlDocument();
			document.LoadXml(m_strsoapresult);
			XmlNamespaceManager manager = new XmlNamespaceManager(document.NameTable);

			XmlNodeList xnList = document.GetElementsByTagName("UpdateHargaTRMResult");

			foreach (XmlNode item in xnList)
			{
				if (item.Name == "UpdateHargaTRMResult" && item.InnerText == "")
				{
					return true;
				}
				else
				{
					message.Add("Fail Sync ETT");
					return false;
				}
			}
			message.Add("Fail Sync ETT");
			return false;

		}


        private bool IsChecked(string FPTID, string TCMemberID = "", string VendorID = "")
        {
            List<string> m_lstSelect = new List<string>();
            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            Dictionary<string, OrderDirection> m_objOrderBy = new Dictionary<string, OrderDirection>();
            int m_intCount = 0;

            if (!string.IsNullOrEmpty(TCMemberID))
            {
                FPTTCParticipantsVM m_objFPTTCParticipantsVM = new FPTTCParticipantsVM();
                DFPTTCParticipantsDA m_objDFPTTCParticipantsDA = new DFPTTCParticipantsDA();
                m_objDFPTTCParticipantsDA.ConnectionStringName = Global.ConnStrConfigName;


                m_lstSelect.Add(FPTTCParticipantsVM.Prop.TCMemberID.MapAlias);


                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(TCMemberID);
                m_objFilter.Add(FPTTCParticipantsVM.Prop.TCMemberID.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(FPTID);
                m_objFilter.Add(FPTTCParticipantsVM.Prop.FPTID.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.In);
                m_lstFilter.Add(1);
                m_objFilter.Add(FPTTCParticipantsVM.Prop.StatusID.Map, m_lstFilter);

                m_objOrderBy.Add(MenuVM.Prop.MenuID.Map, OrderDirection.Ascending);

                Dictionary<int, DataSet> m_dicDFPTTCParticipantsDA = m_objDFPTTCParticipantsDA.SelectBC(0, 1, true, null, m_objFilter, null, null, null, null);


                foreach (KeyValuePair<int, DataSet> m_kvpTCParticipantDA in m_dicDFPTTCParticipantsDA)
                {
                    m_intCount = m_kvpTCParticipantDA.Key;
                    break;
                }
            }
            if (!string.IsNullOrEmpty(VendorID))
            {
                FPTVendorParticipantsVM m_objFPTVendorParticipantsVM = new FPTVendorParticipantsVM();
                DFPTVendorParticipantsDA m_objDFPTVendorParticipantsDA = new DFPTVendorParticipantsDA();
                m_objDFPTVendorParticipantsDA.ConnectionStringName = Global.ConnStrConfigName;

                m_lstSelect.Add(FPTVendorParticipantsVM.Prop.VendorID.MapAlias);

                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(VendorID);
                m_objFilter.Add(FPTVendorParticipantsVM.Prop.VendorID.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(FPTID);
                m_objFilter.Add(FPTVendorParticipantsVM.Prop.FPTID.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.In);
                m_lstFilter.Add(1);
                m_objFilter.Add(FPTVendorParticipantsVM.Prop.StatusID.Map, m_lstFilter);

                m_objOrderBy.Add(MenuVM.Prop.MenuID.Map, OrderDirection.Ascending);

                Dictionary<int, DataSet> m_dicDFPTVendorParticipantsDA = m_objDFPTVendorParticipantsDA.SelectBC(0, 1, true, null, m_objFilter, null, null, null, null);


                foreach (KeyValuePair<int, DataSet> m_kvpMenuBL in m_dicDFPTVendorParticipantsDA)
                {
                    m_intCount = m_kvpMenuBL.Key;
                    break;
                }
            }

            return (m_intCount > 0 ? true : false);
        }

		private List<string> IsSaveValid(string Action, List<FPTVendorParticipantsVM> m_lstFPTVendorParticipantsVM)
		{
			List<string> m_lstReturn = new List<string>();

			if (!m_lstFPTVendorParticipantsVM.Any())
				m_lstReturn.Add(FPTNegotiationRoundVM.Prop.ListVendorParticipant.Desc + " " + General.EnumDesc(MessageLib.mustFill));
			//if (string.IsNullOrEmpty(FPTNegotiationRoundID))
			//    m_lstReturn.Add(FPTNegotiationRoundVM.Prop.FPTNegotiationRoundID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
			//if (FPTNegotiationRoundID.Length>3)
			//    m_lstReturn.Add("Maximum length of "+FPTNegotiationRoundVM.Prop.FPTNegotiationRoundID.Desc + " " + General.EnumDesc(MessageLib.invalid));
			//if (string.IsNullOrEmpty(FPTNegotiationRoundDesc))
			//    m_lstReturn.Add(FPTNegotiationRoundVM.Prop.FPTNegotiationRoundDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));
			//if (string.IsNullOrEmpty(ItemTypeID))
			//    m_lstReturn.Add(FPTNegotiationRoundVM.Prop.ItemTypeID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
			// if (!lst_Parameter.Any())
			//   m_lstReturn.Add(FPTNegotiationRoundVM.Prop.ListFPTNegotiationRoundVM.Desc + " " + General.EnumDesc(MessageLib.mustFill));

			return m_lstReturn;
		}

		private Dictionary<string, object> GetFormData(NameValueCollection parameters)
		{
			Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

			m_dicReturn.Add(FPTNegotiationRoundVM.Prop.RoundID.Name, parameters[FPTNegotiationRoundVM.Prop.RoundID.Name]);
			m_dicReturn.Add(FPTNegotiationRoundVM.Prop.FPTID.Name, parameters[FPTNegotiationRoundVM.Prop.FPTID.Name]);
			m_dicReturn.Add(FPTNegotiationRoundVM.Prop.StartDateTimeStamp.Name, parameters[FPTNegotiationRoundVM.Prop.StartDateTimeStamp.Name]);
			m_dicReturn.Add(FPTNegotiationRoundVM.Prop.EndDateTimeStamp.Name, parameters[FPTNegotiationRoundVM.Prop.EndDateTimeStamp.Name]);

			return m_dicReturn;
		}

		private List<FPTNegotiationRoundVM> GetListFPTNegotiationRound(string FPTID, ref string message)
		{
			DFPTNegotiationRoundsDA m_objFPTNegotiationRoundDA = new DFPTNegotiationRoundsDA();
			m_objFPTNegotiationRoundDA.ConnectionStringName = Global.ConnStrConfigName;

			List<string> m_lstSelect = new List<string>();
			m_lstSelect.Add(FPTNegotiationRoundVM.Prop.RoundID.MapAlias);
			m_lstSelect.Add(FPTNegotiationRoundVM.Prop.StartDateTimeStamp.MapAlias);
			m_lstSelect.Add(FPTNegotiationRoundVM.Prop.EndDateTimeStamp.MapAlias);
			m_lstSelect.Add(FPTNegotiationRoundVM.Prop.Remarks.MapAlias);


			Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
			List<object> m_lstFilter = new List<object>();
			m_lstFilter.Add(Operator.Equals);
			m_lstFilter.Add(FPTID);
			m_objFilter.Add(FPTNegotiationRoundVM.Prop.FPTID.Map, m_lstFilter);


			List<FPTNegotiationRoundVM> m_lstFPTNegotiationRoundVM = new List<FPTNegotiationRoundVM>();
			Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();

			DateTime InvalidDate = new DateTime(9999, 12, 31, 0, 0, 0);

			Dictionary<int, DataSet> m_dicFPTNegotiationRoundDA = m_objFPTNegotiationRoundDA.SelectBC(0, 0, false, m_lstSelect, m_objFilter, null, null, null, null);
			if (m_objFPTNegotiationRoundDA.Message == string.Empty)
			{
				m_lstFPTNegotiationRoundVM = (
					from DataRow m_drFPTNegotiationRoundDA in m_dicFPTNegotiationRoundDA[0].Tables[0].Rows
					select new FPTNegotiationRoundVM()
					{
						RoundID = m_drFPTNegotiationRoundDA[FPTNegotiationRoundVM.Prop.RoundID.Name].ToString(),
						StartDateTimeStamp = DateTime.Parse(m_drFPTNegotiationRoundDA[FPTNegotiationRoundVM.Prop.StartDateTimeStamp.Name].ToString()),
						EndDateTimeStamp = DateTime.Parse(m_drFPTNegotiationRoundDA[FPTNegotiationRoundVM.Prop.EndDateTimeStamp.Name].ToString()),
						Status = ((DateTime.Now >= DateTime.Parse(m_drFPTNegotiationRoundDA[FPTNegotiationRoundVM.Prop.StartDateTimeStamp.Name].ToString()) &&
								 DateTime.Now <= DateTime.Parse(m_drFPTNegotiationRoundDA[FPTNegotiationRoundVM.Prop.EndDateTimeStamp.Name].ToString())) ? "Running" :
								 DateTime.Parse(m_drFPTNegotiationRoundDA[FPTNegotiationRoundVM.Prop.StartDateTimeStamp.Name].ToString()) == InvalidDate ||
								 DateTime.Parse(m_drFPTNegotiationRoundDA[FPTNegotiationRoundVM.Prop.EndDateTimeStamp.Name].ToString()) == InvalidDate ? "Not Started" :
								 "End"),
						Remarks = m_drFPTNegotiationRoundDA[FPTNegotiationRoundVM.Prop.Remarks.Name].ToString(),
					}).Distinct().ToList();
			}
			else
			{
				message = m_objFPTNegotiationRoundDA.Message;
			}

			return m_lstFPTNegotiationRoundVM;

		}

		private FPTNegotiationRoundVM GetSelectedData(Dictionary<string, object> selected, ref string message)
		{
			FPTNegotiationRoundVM m_objFPTNegotiationRoundVM = new FPTNegotiationRoundVM();
			DFPTNegotiationRoundsDA m_objDFPTNegotiationRoundsDA = new DFPTNegotiationRoundsDA();
			m_objDFPTNegotiationRoundsDA.ConnectionStringName = Global.ConnStrConfigName;

			MFPTDA m_objMFPTDA = new MFPTDA();
			m_objMFPTDA.ConnectionStringName = Global.ConnStrConfigName;

			List<string> m_lstSelect = new List<string>();
			m_lstSelect.Add(FPTVM.Prop.FPTID.MapAlias);
			m_lstSelect.Add(FPTVM.Prop.Descriptions.MapAlias);
			m_lstSelect.Add(FPTNegotiationRoundVM.Prop.StartDateTimeStamp.MapAlias);
			m_lstSelect.Add(FPTNegotiationRoundVM.Prop.EndDateTimeStamp.MapAlias);
			m_lstSelect.Add(FPTNegotiationRoundVM.Prop.TotalVendors.MapAlias);
			m_lstSelect.Add(FPTNegotiationRoundVM.Prop.Duration.MapAlias);
			m_lstSelect.Add(FPTNegotiationRoundVM.Prop.Round.MapAlias);
			m_lstSelect.Add(FPTNegotiationRoundVM.Prop.RoundNo.MapAlias);
			m_lstSelect.Add(FPTNegotiationRoundVM.Prop.Schedule.MapAlias);
			m_lstSelect.Add(FPTNegotiationRoundVM.Prop.RoundID.MapAlias);
			m_lstSelect.Add(FPTNegotiationRoundVM.Prop.TopValue.MapAlias);
			m_lstSelect.Add(FPTNegotiationRoundVM.Prop.BottomValue.MapAlias);
			m_lstSelect.Add(string.Format("CASE WHEN TotalRoundID > 0 THEN 'Active' ELSE 'Non Active' END AS {0}", FPTNegotiationRoundVM.Prop.Status.Name));

			List<string> m_lstKey = new List<string>();
			List<object> m_lstFilter = new List<object>();
			Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
			foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
			{
				if (m_objFPTNegotiationRoundVM.IsKey(m_kvpSelectedRow.Key))
				{
					if (m_kvpSelectedRow.Value != null && !string.IsNullOrEmpty(m_kvpSelectedRow.Value.ToString()))
					{
						m_lstKey.Add(m_kvpSelectedRow.Value.ToString());

						m_lstFilter.Add(Operator.Equals);
						m_lstFilter.Add(m_kvpSelectedRow.Value);
						m_objFilter.Add(FPTVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
					}
				}
			}

			//m_lstFilter = new List<object>();
			//m_lstFilter.Add(Operator.None);
			//m_lstFilter.Add(string.Empty);
			//m_objFilter.Add("[FPTStatus].StatusDateTimeStamp=[DFPTStatus].StatusDateTimeStamp", m_lstFilter);

			Dictionary<int, DataSet> m_dicDFPTNegotiationRoundsDA = m_objMFPTDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
			if (m_objDFPTNegotiationRoundsDA.Message == string.Empty)
			{
				DataRow m_drDFPTNegotiationRoundsDA = m_dicDFPTNegotiationRoundsDA[0].Tables[0].Rows[0];

				m_objFPTNegotiationRoundVM.FPTID = m_drDFPTNegotiationRoundsDA[FPTNegotiationRoundVM.Prop.FPTID.Name].ToString();
				m_objFPTNegotiationRoundVM.FPTDesc = m_drDFPTNegotiationRoundsDA[FPTNegotiationRoundVM.Prop.Descriptions.Name].ToString();
				m_objFPTNegotiationRoundVM.TotalVendors = (!string.IsNullOrEmpty(m_drDFPTNegotiationRoundsDA[FPTNegotiationRoundVM.Prop.TotalVendors.Name].ToString()) ? int.Parse(m_drDFPTNegotiationRoundsDA[FPTNegotiationRoundVM.Prop.TotalVendors.Name].ToString()) : (int?)null);
				m_objFPTNegotiationRoundVM.StartDateTimeStamp = !string.IsNullOrEmpty(m_drDFPTNegotiationRoundsDA[FPTNegotiationRoundVM.Prop.StartDateTimeStamp.Name].ToString()) ? DateTime.Parse(m_drDFPTNegotiationRoundsDA[FPTNegotiationRoundVM.Prop.StartDateTimeStamp.Name].ToString()) : (DateTime?)null;
				m_objFPTNegotiationRoundVM.EndDateTimeStamp = !string.IsNullOrEmpty(m_drDFPTNegotiationRoundsDA[FPTNegotiationRoundVM.Prop.EndDateTimeStamp.Name].ToString()) ? DateTime.Parse(m_drDFPTNegotiationRoundsDA[FPTNegotiationRoundVM.Prop.EndDateTimeStamp.Name].ToString()) : (DateTime?)null;
				m_objFPTNegotiationRoundVM.Schedule = DateTime.Parse(m_drDFPTNegotiationRoundsDA[FPTNegotiationRoundVM.Prop.Schedule.Name].ToString());
				m_objFPTNegotiationRoundVM.Duration = string.IsNullOrEmpty(m_drDFPTNegotiationRoundsDA[FPTNegotiationRoundVM.Prop.Duration.Name].ToString()) ? (int?)null : int.Parse(m_drDFPTNegotiationRoundsDA[FPTNegotiationRoundVM.Prop.Duration.Name].ToString());
				m_objFPTNegotiationRoundVM.TotalRound = string.IsNullOrEmpty(m_drDFPTNegotiationRoundsDA[FPTNegotiationRoundVM.Prop.Round.Name].ToString()) ? (int?)null : int.Parse(m_drDFPTNegotiationRoundsDA[FPTNegotiationRoundVM.Prop.Round.Name].ToString());
				m_objFPTNegotiationRoundVM.RoundNo = string.IsNullOrEmpty(m_drDFPTNegotiationRoundsDA[FPTNegotiationRoundVM.Prop.RoundNo.Name].ToString()) ? (int?)null : int.Parse(m_drDFPTNegotiationRoundsDA[FPTNegotiationRoundVM.Prop.RoundNo.Name].ToString());
				m_objFPTNegotiationRoundVM.Status = m_drDFPTNegotiationRoundsDA[FPTNegotiationRoundVM.Prop.Status.Name].ToString();
				m_objFPTNegotiationRoundVM.TopValue = decimal.Parse(string.IsNullOrEmpty(m_drDFPTNegotiationRoundsDA[FPTNegotiationRoundVM.Prop.TopValue.Name].ToString()) ? "0" : m_drDFPTNegotiationRoundsDA[FPTNegotiationRoundVM.Prop.TopValue.Name].ToString());
				m_objFPTNegotiationRoundVM.BottomValue = decimal.Parse(string.IsNullOrEmpty(m_drDFPTNegotiationRoundsDA[FPTNegotiationRoundVM.Prop.BottomValue.Name].ToString()) ? "0" : m_drDFPTNegotiationRoundsDA[FPTNegotiationRoundVM.Prop.BottomValue.Name].ToString());
				m_objFPTNegotiationRoundVM.RoundID = m_drDFPTNegotiationRoundsDA[FPTNegotiationRoundVM.Prop.RoundID.Name].ToString();

			}
			else
				message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDFPTNegotiationRoundsDA.Message;

			return m_objFPTNegotiationRoundVM;
		}
		#endregion

		#region Public Method

		public Dictionary<int, List<FPTNegotiationRoundVM>> GetFPTNegotiationRoundData(bool isCount, string FPTNegotiationRoundID, string FPTNegotiationRoundDesc)
		{
			int m_intCount = 0;
			List<FPTNegotiationRoundVM> m_lstFPTNegotiationRoundVM = new List<ViewModels.FPTNegotiationRoundVM>();
			Dictionary<int, List<FPTNegotiationRoundVM>> m_dicReturn = new Dictionary<int, List<FPTNegotiationRoundVM>>();
			DFPTNegotiationRoundsDA m_objDFPTNegotiationRoundsDA = new DFPTNegotiationRoundsDA();
			m_objDFPTNegotiationRoundsDA.ConnectionStringName = Global.ConnStrConfigName;

			List<string> m_lstSelect = new List<string>();
			m_lstSelect.Add(FPTNegotiationRoundVM.Prop.RoundID.MapAlias);
			m_lstSelect.Add(FPTNegotiationRoundVM.Prop.FPTID.MapAlias);
			m_lstSelect.Add(FPTNegotiationRoundVM.Prop.StartDateTimeStamp.MapAlias);
			m_lstSelect.Add(FPTNegotiationRoundVM.Prop.EndDateTimeStamp.MapAlias);

			Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
			List<object> m_lstFilter = new List<object>();
			m_lstFilter.Add(Operator.Contains);
			m_lstFilter.Add(FPTNegotiationRoundID);
			m_objFilter.Add(FPTNegotiationRoundVM.Prop.RoundID.Map, m_lstFilter);

			m_lstFilter = new List<object>();
			m_lstFilter.Add(Operator.Contains);
			m_lstFilter.Add(FPTNegotiationRoundDesc);
			m_objFilter.Add(FPTNegotiationRoundVM.Prop.FPTID.Map, m_lstFilter);

			Dictionary<int, DataSet> m_dicDFPTNegotiationRoundsDA = m_objDFPTNegotiationRoundsDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
			if (m_objDFPTNegotiationRoundsDA.Message == string.Empty)
			{
				if (isCount)
					foreach (KeyValuePair<int, DataSet> m_kvpFPTNegotiationRoundBL in m_dicDFPTNegotiationRoundsDA)
					{
						m_intCount = m_kvpFPTNegotiationRoundBL.Key;
						break;
					}
				else
				{
					m_lstFPTNegotiationRoundVM = (
						from DataRow m_drDFPTNegotiationRoundsDA in m_dicDFPTNegotiationRoundsDA[0].Tables[0].Rows
						select new FPTNegotiationRoundVM()
						{
							RoundID = m_drDFPTNegotiationRoundsDA[FPTNegotiationRoundVM.Prop.RoundID.Name].ToString(),
							FPTID = m_drDFPTNegotiationRoundsDA[FPTNegotiationRoundVM.Prop.FPTID.Name].ToString(),
							StartDateTimeStamp = DateTime.Parse(m_drDFPTNegotiationRoundsDA[FPTNegotiationRoundVM.Prop.StartDateTimeStamp.Name].ToString()),
							EndDateTimeStamp = DateTime.Parse(m_drDFPTNegotiationRoundsDA[FPTNegotiationRoundVM.Prop.EndDateTimeStamp.Name].ToString()),

						}
					).ToList();
				}
			}
			m_dicReturn.Add(m_intCount, m_lstFPTNegotiationRoundVM);
			return m_dicReturn;
		}

		#endregion
	}
}