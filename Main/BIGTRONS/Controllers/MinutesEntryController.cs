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
using System.Net;
using System.Xml;
using System.IO;
using System.Web;
using System.Runtime.Serialization.Formatters.Binary;
using SelectPdf;

namespace com.SML.BIGTRONS.Controllers
{
	public class MinutesEntryController : BaseController
	{
		private readonly string title = "MinutesEntry";
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
			TMinuteEntriesDA m_objTMinuteEntriesDA = new TMinuteEntriesDA();
			m_objTMinuteEntriesDA.ConnectionStringName = Global.ConnStrConfigName;

			int m_intSkip = parameters.Start;
			int m_intLength = parameters.Limit;
			bool m_boolIsCount = true;

			FilterHeaderConditions m_fhcTMinuteEntries = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
			Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

			foreach (FilterHeaderCondition m_fhcFilter in m_fhcTMinuteEntries.Conditions)
			{
				string m_strDataIndex = m_fhcFilter.DataIndex;
				string m_strConditionOperator = m_fhcFilter.Operator;
				object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

				if (m_strDataIndex != string.Empty)
				{
					m_strDataIndex = MinutesEntryVM.Prop.Map(m_strDataIndex, false);
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
			Dictionary<int, DataSet> m_dicTMinuteEntriesDA = m_objTMinuteEntriesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
			int m_intCount = 0;

			foreach (KeyValuePair<int, DataSet> m_kvpSchedulesDA in m_dicTMinuteEntriesDA)
			{
				m_intCount = m_kvpSchedulesDA.Key;
				break;
			}

			List<MinutesEntryVM> m_lstMinutesEntryVM = new List<MinutesEntryVM>();
			if (m_intCount > 0)
			{
				m_boolIsCount = false;
				List<string> m_lstSelect = new List<string>();
				m_lstSelect.Add(MinutesEntryVM.Prop.MinuteEntryID.MapAlias);
				m_lstSelect.Add(MinutesEntryVM.Prop.ScheduleID.MapAlias);
				m_lstSelect.Add(MinutesEntryVM.Prop.ScheduleDesc.MapAlias);
				m_lstSelect.Add(MinutesEntryVM.Prop.TaskID.MapAlias);
				m_lstSelect.Add(MinutesEntryVM.Prop.FPTID.MapAlias);
				m_lstSelect.Add(MinutesEntryVM.Prop.FPTDesc.MapAlias);
				m_lstSelect.Add(MinutesEntryVM.Prop.StatusID.MapAlias);
				m_lstSelect.Add(MinutesEntryVM.Prop.StatusDesc.MapAlias);
				m_lstSelect.Add(MinutesEntryVM.Prop.FunctionID.MapAlias);
				m_lstSelect.Add(MinutesEntryVM.Prop.FunctionDesc.MapAlias);
				m_lstSelect.Add(MinutesEntryVM.Prop.NotificationStatusID.MapAlias);
				m_lstSelect.Add(MinutesEntryVM.Prop.NotificationTemplateID.MapAlias);
				m_lstSelect.Add(MinutesEntryVM.Prop.NotificationTemplateDesc.MapAlias);
				m_lstSelect.Add(MinutesEntryVM.Prop.MinuteTemplateID.MapAlias);
				m_lstSelect.Add(MinutesEntryVM.Prop.MinuteTemplateDescriptions.MapAlias);
				m_lstSelect.Add(MinutesEntryVM.Prop.Subject.MapAlias);
				m_lstSelect.Add(MinutesEntryVM.Prop.CreatedDate.MapAlias);
				m_lstSelect.Add(MinutesEntryVM.Prop.NotificationStatusDesc.MapAlias);
				m_lstSelect.Add(MinutesEntryVM.Prop.MailNotificationID.MapAlias);

				List<string> m_lstGroupBy = new List<string>();
				m_lstGroupBy.Add(MinutesEntryVM.Prop.MinuteEntryID.Map);
				m_lstGroupBy.Add(MinutesEntryVM.Prop.ScheduleID.Map);
				m_lstGroupBy.Add(MinutesEntryVM.Prop.ScheduleDesc.Map);
				m_lstGroupBy.Add(MinutesEntryVM.Prop.TaskID.Map);
				m_lstGroupBy.Add(MinutesEntryVM.Prop.FPTID.Map);
				m_lstGroupBy.Add(MinutesEntryVM.Prop.FPTDesc.Map);
				m_lstGroupBy.Add(MinutesEntryVM.Prop.StatusID.Map);
				m_lstGroupBy.Add(MinutesEntryVM.Prop.StatusDesc.Map);
				m_lstGroupBy.Add(MinutesEntryVM.Prop.FunctionID.Map);
				m_lstGroupBy.Add(MinutesEntryVM.Prop.FunctionDesc.Map);
				m_lstGroupBy.Add(MinutesEntryVM.Prop.NotificationStatusID.Map);
				m_lstGroupBy.Add(MinutesEntryVM.Prop.NotificationTemplateID.Map);
				m_lstGroupBy.Add(MinutesEntryVM.Prop.NotificationTemplateDesc.Map);
				m_lstGroupBy.Add(MinutesEntryVM.Prop.MinuteTemplateID.Map);
				m_lstGroupBy.Add(MinutesEntryVM.Prop.MinuteTemplateDescriptions.Map);
				m_lstGroupBy.Add(MinutesEntryVM.Prop.Subject.Map);
				m_lstGroupBy.Add(MinutesEntryVM.Prop.CreatedDate.Map);
				m_lstGroupBy.Add(MinutesEntryVM.Prop.NotificationStatusDesc.Map);
				m_lstGroupBy.Add(MinutesEntryVM.Prop.MailNotificationID.Map);

				Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
				foreach (DataSorter m_dtsOrder in parameters.Sort)
					m_dicOrder.Add(MinutesEntryVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

				m_dicTMinuteEntriesDA = m_objTMinuteEntriesDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, m_lstGroupBy, m_dicOrder, null);
				if (m_objTMinuteEntriesDA.Message == string.Empty)
				{
					m_lstMinutesEntryVM = (
						from DataRow m_drTMinuteEntriesDA in m_dicTMinuteEntriesDA[0].Tables[0].Rows
						select new MinutesEntryVM()
						{
							MinuteEntryID = m_drTMinuteEntriesDA[MinutesEntryVM.Prop.MinuteEntryID.Name].ToString(),
							ScheduleID = m_drTMinuteEntriesDA[MinutesEntryVM.Prop.ScheduleID.Name].ToString(),
							ScheduleDesc = m_drTMinuteEntriesDA[MinutesEntryVM.Prop.ScheduleDesc.Name].ToString(),
							FPTID = m_drTMinuteEntriesDA[MinutesEntryVM.Prop.FPTID.Name].ToString(),
							FPTDesc = m_drTMinuteEntriesDA[MinutesEntryVM.Prop.FPTDesc.Name].ToString(),
							TaskID = m_drTMinuteEntriesDA[MinutesEntryVM.Prop.TaskID.Name].ToString(),
							StatusID = (int)m_drTMinuteEntriesDA[MinutesEntryVM.Prop.StatusID.Name],
							StatusDesc = m_drTMinuteEntriesDA[MinutesEntryVM.Prop.StatusDesc.Name].ToString(),
							FunctionID = m_drTMinuteEntriesDA[MinutesEntryVM.Prop.FunctionID.Name].ToString(),
							FunctionDesc = m_drTMinuteEntriesDA[MinutesEntryVM.Prop.FunctionDesc.Name].ToString(),
							NotificationStatusID = m_drTMinuteEntriesDA[MinutesEntryVM.Prop.NotificationStatusID.Name].ToString(),
							NotificationTemplateID = m_drTMinuteEntriesDA[MinutesEntryVM.Prop.NotificationTemplateID.Name].ToString(),
							NotificationTemplateDesc = m_drTMinuteEntriesDA[MinutesEntryVM.Prop.NotificationTemplateDesc.Name].ToString(),
							MinuteTemplateID = m_drTMinuteEntriesDA[MinutesEntryVM.Prop.MinuteTemplateID.Name].ToString(),
							MinuteTemplateDescriptions = m_drTMinuteEntriesDA[MinutesEntryVM.Prop.MinuteTemplateDescriptions.Name].ToString(),
							NotificationStatusDesc = m_drTMinuteEntriesDA[MinutesEntryVM.Prop.NotificationStatusDesc.Name].ToString(),
							CreatedDate = (DateTime)m_drTMinuteEntriesDA[MinutesEntryVM.Prop.CreatedDate.Name],
							Subject = m_drTMinuteEntriesDA[MinutesEntryVM.Prop.Subject.Name].ToString(),
							//IsAllDay = (bool)m_drMScheduleDA[MinutesEntryVM.Prop.IsAllDay.Name],
							//Notes = m_drMScheduleDA[MinutesEntryVM.Prop.Notes.Name].ToString(),
							//Weblink = m_drMScheduleDA[MinutesEntryVM.Prop.Weblink.Name].ToString(),
							//Location = m_drMScheduleDA[MinutesEntryVM.Prop.Location.Name].ToString(),
							//Priority = (int)m_drMScheduleDA[MinutesEntryVM.Prop.Priority.Name],
							MailNotificationID = m_drTMinuteEntriesDA[MinutesEntryVM.Prop.MailNotificationID.Name].ToString()
						}
					).ToList();
				}
			}
			
			return this.Store(m_lstMinutesEntryVM, m_intCount);
		}

		public ActionResult ReadBrowse(StoreRequestParameters parameters, string BusinessUnitID)
		{
			MDivisionDA m_objMDivisionDA = new MDivisionDA();
			m_objMDivisionDA.ConnectionStringName = Global.ConnStrConfigName;

			int m_intSkip = parameters.Start;
			int m_intLength = parameters.Limit;
			bool m_boolIsCount = true;

			FilterHeaderConditions m_fhcMDivision = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
			Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
			List<object> m_lstFilter = new List<object>();
			foreach (FilterHeaderCondition m_fhcFilter in m_fhcMDivision.Conditions)
			{
				string m_strDataIndex = m_fhcFilter.DataIndex;
				string m_strConditionOperator = m_fhcFilter.Operator;
				object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

				if (m_strDataIndex != string.Empty)
				{
					m_strDataIndex = DivisionVM.Prop.Map(m_strDataIndex, false);

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
			if (!string.IsNullOrEmpty(BusinessUnitID))
			{
				m_lstFilter.Add(Operator.Equals);
				m_lstFilter.Add(BusinessUnitID);
				m_objFilter.Add(DivisionVM.Prop.BusinessUnitID.Map, m_lstFilter);
			}

			Dictionary<int, DataSet> m_dicMDivisionDA = m_objMDivisionDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
			int m_intCount = 0;

			foreach (KeyValuePair<int, DataSet> m_kvpDivisionBL in m_dicMDivisionDA)
			{
				m_intCount = m_kvpDivisionBL.Key;
				break;
			}

			List<DivisionVM> m_lstDivisionVM = new List<DivisionVM>();
			if (m_intCount > 0)
			{
				m_boolIsCount = false;
				List<string> m_lstSelect = new List<string>();
				m_lstSelect.Add(DivisionVM.Prop.DivisionID.MapAlias);
				m_lstSelect.Add(DivisionVM.Prop.DivisionDesc.MapAlias);

				Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
				foreach (DataSorter m_dtsOrder in parameters.Sort)
					m_dicOrder.Add(DivisionVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

				m_dicMDivisionDA = m_objMDivisionDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
				if (m_objMDivisionDA.Message == string.Empty)
				{
					m_lstDivisionVM = (
						from DataRow m_drMDivisionDA in m_dicMDivisionDA[0].Tables[0].Rows
						select new DivisionVM()
						{
							DivisionID = m_drMDivisionDA[DivisionVM.Prop.DivisionID.Name].ToString(),
							DivisionDesc = m_drMDivisionDA[DivisionVM.Prop.DivisionDesc.Name].ToString()
						}
					).ToList();
				}
			}
			return this.Store(m_lstDivisionVM, m_intCount);
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

			MinutesEntryVM m_objMinutesEntryVM = new MinutesEntryVM();
			ViewDataDictionary m_vddMinutesEntry = new ViewDataDictionary();
			m_vddMinutesEntry.Add(General.EnumDesc(Params.Caller), Caller);
			m_vddMinutesEntry.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
			if (Caller == General.EnumDesc(Buttons.ButtonDetail))
			{
				NameValueCollection m_nvcParams = this.Request.Params;
				Dictionary<string, object> m_dicSelectedRow = GetFormData(m_nvcParams);
				Session[dataSessionName] = JSON.Serialize(m_dicSelectedRow);
			}
			m_objMinutesEntryVM.StatusID = (int)MinutesStatus.Draft;//Check
			m_objMinutesEntryVM.ListRecipients = new List<RecipientsVM>();
			m_objMinutesEntryVM.ListMinutesValues = new List<EntryValuesVM>();
			m_objMinutesEntryVM.ListNotificationValues = new List<NotificationValuesVM>();
			//m_objMinutesEntryVM.LstNotificationAttachment = new List<NotificationAttachmentVM>();

			this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
			return new XMVC.PartialViewResult
			{
				ClearContainer = true,
				ContainerId = General.EnumName(Params.PageOne),
				Model = m_objMinutesEntryVM,
				RenderMode = RenderMode.AddTo,
				ViewData = m_vddMinutesEntry,
				ViewName = "_Form",
				WrapByScriptTag = false
			};
		}

		public ActionResult Detail(string Caller, string Selected, string MinuteEntryID = "")
		{
			//Global.HasAccess = GetHasAccess();
			//if (!Global.HasAccess.Read)
			//    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

			MinutesEntryVM m_objMinutesEntryVM = new MinutesEntryVM();
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
				if (m_dicSelectedRow.ContainsKey(MinutesEntryVM.Prop.MinuteEntryID.Name)
					&& string.IsNullOrEmpty(m_dicSelectedRow[MinutesEntryVM.Prop.MinuteEntryID.Name].ToString()))
				{
					m_dicSelectedRow[MinutesEntryVM.Prop.MinuteEntryID.Name] = MinuteEntryID;

				}
			}

			if (m_dicSelectedRow.Count > 0)
				m_objMinutesEntryVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
			if (m_strMessage != string.Empty)
			{
				Global.ShowErrorAlert(title, m_strMessage);
				return this.Direct();
			}


			ViewDataDictionary m_vddMinutesEntry = new ViewDataDictionary();
			m_vddMinutesEntry.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
			this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
			return new XMVC.PartialViewResult
			{
				ClearContainer = true,
				ContainerId = General.EnumName(Params.PageOne),
				Model = m_objMinutesEntryVM,
				RenderMode = RenderMode.AddTo,
				ViewData = m_vddMinutesEntry,
				ViewName = "_Form",
				WrapByScriptTag = false
			};
		}

		public ActionResult Update(string Caller, string Selected)
		{
			//Global.HasAccess = GetHasAccess();
			//if (!Global.HasAccess.Update)
			//    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

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
			MinutesEntryVM m_objMinutesEntryVM = new MinutesEntryVM();
			if (m_dicSelectedRow.Count > 0)
				m_objMinutesEntryVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);

			//int m_iStatusID = int.Parse(m_objMinutesEntryVM.TaskStatusID);

			switch (m_objMinutesEntryVM.StatusID)
			{
				case (int)MinutesStatus.Draft: break;
				default:
					{
						Global.ShowErrorAlert(title, MinutesEntryVM.Prop.StatusDesc.Desc + " " + General.EnumDesc(MessageLib.invalid));

						return this.Direct();
					}
			}

			if (m_strMessage != string.Empty)
			{
				Global.ShowErrorAlert(title, m_strMessage);
				return this.Direct();
			}

			ViewDataDictionary m_vddMinutesEntry = new ViewDataDictionary();
			m_vddMinutesEntry.Add(General.EnumDesc(Params.Caller), Caller);
			m_vddMinutesEntry.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
			this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
			return new XMVC.PartialViewResult
			{
				ClearContainer = true,
				ContainerId = General.EnumName(Params.PageOne),
				Model = m_objMinutesEntryVM,
				RenderMode = RenderMode.AddTo,
				ViewData = m_vddMinutesEntry,
				ViewName = "_Form",
				WrapByScriptTag = false
			};
		}

		public ActionResult Delete(string Selected)
		{
			if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
				return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

			MinutesEntryVM m_lstSelectedRow = new MinutesEntryVM();
			m_lstSelectedRow = JSON.Deserialize<MinutesEntryVM>(Selected);

			TMinuteEntriesDA m_objTMinuteEntriesDA = new TMinuteEntriesDA();
			m_objTMinuteEntriesDA.ConnectionStringName = Global.ConnStrConfigName;
			List<string> m_lstMessage = new List<string>();

			try
			{
				//Update Minutes status
				Dictionary<string, List<object>> m_dicSet = new Dictionary<string, List<object>>();
				Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
				List<object> m_lstFilter = new List<object>();
				m_lstFilter.Add(Operator.Equals);
				m_lstFilter.Add(m_lstSelectedRow.MinuteEntryID);
				m_objFilter.Add(MinutesEntryVM.Prop.MinuteEntryID.Map, m_lstFilter);

				List<object> m_lstSet = new List<object>();
				m_lstSet.Add(typeof(int));
				m_lstSet.Add((int)MinutesStatus.Deleted);
				m_dicSet.Add(MinutesEntryVM.Prop.StatusID.Map, m_lstSet);
				m_objTMinuteEntriesDA.UpdateBC(m_dicSet, m_objFilter,false);
				
				if (!m_objTMinuteEntriesDA.Success) { 
						m_lstMessage.Add(m_objTMinuteEntriesDA.Message);
				}
			}
			catch (Exception ex)
			{
				m_lstMessage.Add(ex.Message);
			}
			if (m_lstMessage.Count <= 0)
			{
				Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Deleted));
				return this.Direct(true, string.Empty);
			}
			else
				return this.Direct(false, String.Join(Global.NewLineSeparated, m_lstMessage));
			
		}

		public ActionResult ReadAttachment()
		{
			List<byte[]> lstByte = new List<byte[]>();
			string ContentType = "";
			string FileName = "";
			string StringRawData = "";
			//byte[] RawData = null;
			for (int x = 0; x < Request.Files.Count; x++)
			{
				if (Request.Files[x].ContentLength > 500000)
				{
					Global.ShowErrorAlert("Error Upload", "Maximum file size is 500KB");
					return this.Direct();
				}
				byte[] fileData = null;
				using (var binaryReader = new BinaryReader(Request.Files[x].InputStream))
					fileData = binaryReader.ReadBytes(Request.Files[x].ContentLength);
				FileName = Path.GetFileName(Server.MapPath(Request.Files[0].FileName));
				ContentType = Path.GetExtension(FileName);
				StringRawData = GetBase64String(fileData);

				//RawData = ConvertHASHStringToByte(StringRawData);
				//var t = new mailws.MailSender();
				//mailws.MailAttachment[] Attachment = new mailws.MailAttachment[1];
				//var u = new mailws.MailAttachment()
				//{
				//    FileName = FileName,
				//    Content = RawData
				//};
				//Attachment[0] = u;
				////string send = t.SendMail("bigtronstes@sinarmasland.com", "ronypasca@gmail.com", "rezaseptiandra@gmail.com", "", "Coba Kirim Attachment", "", Attachment);
				//l///stByte.Add(fileData);
			}
			NotificationAttachmentVM objs = new NotificationAttachmentVM()
			{
				Filename = FileName,
				ContentType = ContentType,
				RawData = StringRawData,
				MailNotificationID = ""

			};
			object ls = objs;
			Global.ShowInfo("Upload", "Success");
			return this.Store(ls);
		}

		public ActionResult Download(string Selected)
		{
			Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();
			m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);

			string m_strMessage = string.Empty;
			MinutesEntryVM m_objMinutesEntryVM = new MinutesEntryVM();
			if (m_dicSelectedRow.Count > 0)
				m_objMinutesEntryVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
			if (m_strMessage != string.Empty)
			{
				Global.ShowErrorAlert(title, m_strMessage);
				return this.Direct();
			}

			List<ConfigVM> m_lConfigVM = GetConfig(nameof(TMinuteEntries), MinutesEntryVM.Prop.NotificationTemplateID.Name).ToList();
			string m_strHeaderMOMID = string.Empty;
			string m_strFooterMOMID = string.Empty;

			if (m_lConfigVM.Any())
			{
				m_strHeaderMOMID = m_lConfigVM.FirstOrDefault(d => d.Key3.Equals("Header")).Desc1;
				if (m_lConfigVM.Any(d => d.Key3.Equals("Footer") && d.Key4 == m_objMinutesEntryVM.FunctionID))
					m_strFooterMOMID = m_lConfigVM.FirstOrDefault(d => d.Key3.Equals("Footer") && d.Key4 == m_objMinutesEntryVM.FunctionID).Desc1;
				if (string.IsNullOrEmpty(m_strFooterMOMID))
					m_strFooterMOMID = m_lConfigVM.FirstOrDefault(d => d.Key3.Equals("Footer") && d.Key4 == "").Desc1;
			}

			PdfPageSize pageSize = PdfPageSize.A4;
			PdfPageOrientation pdfOrientation = PdfPageOrientation.Portrait;

			int m_webPageWidth = 800;
			int m_webPageHeight = 0;
			int m_iMarginTop = 25;
			int m_iMarginBottom = 20;
			int m_iMarginLeft = 50;
			int m_iMarginRight = 50;

			// instantiate a html to pdf converter object
			HtmlToPdf m_converter = new HtmlToPdf();

			// set converter options
			m_converter.Options.PdfPageSize = pageSize;
			m_converter.Options.PdfPageOrientation = pdfOrientation;
			//m_converter.Options.WebPageFixedSize = true;
			m_converter.Options.WebPageWidth = m_webPageWidth;
			m_converter.Options.WebPageHeight = m_webPageHeight;
			m_converter.Options.MarginLeft = m_iMarginLeft;
			m_converter.Options.MarginRight = m_iMarginRight;
			m_converter.Options.MarginTop = m_iMarginTop;
			m_converter.Options.MarginBottom = m_iMarginBottom;

			// get minutes template
			MinutesTemplateVM objMinutesTemplateVM = GetMinutesTemplateVM(m_objMinutesEntryVM.MinuteTemplateID);

			// set minutes values
			Dictionary<string, string> m_dicValues = new Dictionary<string, string>();
			foreach (EntryValuesVM objEntryValuesVM in m_objMinutesEntryVM.ListMinutesValues)
			{
				m_dicValues.Add(objEntryValuesVM.FieldTagID, objEntryValuesVM.Value);
			}
			string m_strContentFile = Global.ParseParameter( objMinutesTemplateVM.Contents, m_dicValues);

			

			// header settings
			int m_iheaderHeight = 120;
			m_converter.Options.DisplayHeader = true;
			m_converter.Header.Height = m_iheaderHeight;

			NotificationTemplateVM m_NotificationTemplateVM = GetNotificationTemplateVM(m_strHeaderMOMID);
			// set minutes values - headers
			m_dicValues = new Dictionary<string, string>();
			foreach (EntryValuesVM objEntryValuesVM in m_objMinutesEntryVM.ListMinutesValues)
			{
				m_dicValues.Add(objEntryValuesVM.FieldTagID, objEntryValuesVM.Value);
			}
			string m_strHeader = m_NotificationTemplateVM != null ? m_NotificationTemplateVM.Contents : string.Empty;

			string m_strContentHeaders = string.Empty;
			if (!string.IsNullOrEmpty(m_strHeader))
				m_strContentHeaders = Global.ParseParameter(m_strHeader, m_dicValues);

			PdfHtmlSection m_pdfheaderHtml = new PdfHtmlSection(m_strContentHeaders, "");
			m_pdfheaderHtml.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;
			m_pdfheaderHtml.WebPageWidth = m_webPageWidth;


			m_converter.Header.Add(m_pdfheaderHtml);

			// footer settings
			int footerHeight = 100;
			m_converter.Options.DisplayFooter = true;
			m_converter.Footer.Height = footerHeight;

			m_NotificationTemplateVM = GetNotificationTemplateVM(m_strFooterMOMID);
			m_dicValues = new Dictionary<string, string>();
			foreach (EntryValuesVM objEntryValuesVM in m_objMinutesEntryVM.ListMinutesValues)
			{
				m_dicValues.Add(objEntryValuesVM.FieldTagID, objEntryValuesVM.Value);
			}
			string m_strFooter = m_NotificationTemplateVM!=null? m_NotificationTemplateVM.Contents : string.Empty;
			string m_strContentFooters = string.Empty;
			if (!string.IsNullOrEmpty(m_strHeader))
				m_strContentFooters = Global.ParseParameter(m_strFooter, m_dicValues);

			PdfHtmlSection footerHtml = new PdfHtmlSection(m_strContentFooters, "");
			footerHtml.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;
			footerHtml.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;
			//footerHtml.WebPageWidth = m_webPageWidth;

			m_converter.Footer.Add(footerHtml);

			// page numbers can be added using a PdfTextSection object
			PdfTextSection text = new PdfTextSection(0, 80,
				"Page: {page_number} of {total_pages}  ",
				new System.Drawing.Font("Arial", 8));
			text.HorizontalAlign = PdfTextHorizontalAlign.Right;
			m_converter.Footer.Add(text);

	
			// create a new pdf document converting an url
			PdfDocument m_pdfDoc = m_converter.ConvertHtmlString(m_strContentFile, string.Empty);

			// save pdf document
			byte[] m_bypdf = m_pdfDoc.Save();

			// close pdf document
			m_pdfDoc.Close();

			// return resulted pdf document
			return File(m_bypdf, System.Net.Mime.MediaTypeNames.Application.Pdf, $"MOM - {m_objMinutesEntryVM.FPTDesc}.pdf");

		}

		public ActionResult Browse(string ControlDivisionID, string ControlDivisionDesc, string FilterDivisionID = "", string FilterDivisionDesc = "", string FilterBusinessUnitID = "")
		{
			//if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
			//    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

			ViewDataDictionary m_vddDivision = new ViewDataDictionary();
			m_vddDivision.Add("Control" + DivisionVM.Prop.DivisionID.Name, ControlDivisionID);
			m_vddDivision.Add("Control" + DivisionVM.Prop.DivisionDesc.Name, ControlDivisionDesc);
			m_vddDivision.Add(DivisionVM.Prop.DivisionID.Name, FilterDivisionID);
			m_vddDivision.Add(DivisionVM.Prop.DivisionDesc.Name, FilterDivisionDesc);
			m_vddDivision.Add(BusinessUnitVM.Prop.BusinessUnitID.Name, FilterBusinessUnitID);

			return new XMVC.PartialViewResult
			{
				RenderMode = RenderMode.Auto,
				WrapByScriptTag = false,
				ViewData = m_vddDivision,
				ViewName = "../Division/_Browse"
			};
		}

		//BrowsePreBuildRecipients
		public ActionResult BrowsePreBuildRecipients(string ControlDivisionID, string ControlDivisionDesc, string FilterDivisionID = "", string FilterDivisionDesc = "", string FilterBusinessUnitID = "")
		{
			//if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
			//    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));


			ViewDataDictionary m_vddDivision = new ViewDataDictionary();
			m_vddDivision.Add("Control" + DivisionVM.Prop.DivisionID.Name, ControlDivisionID);
			m_vddDivision.Add("Control" + DivisionVM.Prop.DivisionDesc.Name, ControlDivisionDesc);
			m_vddDivision.Add(DivisionVM.Prop.DivisionID.Name, FilterDivisionID);
			m_vddDivision.Add(DivisionVM.Prop.DivisionDesc.Name, FilterDivisionDesc);
			m_vddDivision.Add(BusinessUnitVM.Prop.BusinessUnitID.Name, FilterBusinessUnitID);

			return new XMVC.PartialViewResult
			{
				RenderMode = RenderMode.Auto,
				WrapByScriptTag = false,
				ViewData = m_vddDivision,
				ViewName = "../Division/_Browse"
			};
		}

		[ValidateInput(false)]
		public ActionResult Save(string Action)
		{
			//action = JSON.Deserialize<string>(action);
			if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
				: HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
				return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));


			bool isUpdate = Action == General.EnumDesc(Buttons.ButtonUpdate);
			bool isCancel = Action == General.EnumDesc(Buttons.ButtonCancel);
			bool isReschedule = Action == General.EnumDesc(Buttons.ButtonChange);
			bool isAdd = (Action == General.EnumDesc(Buttons.ButtonAdd)) || isCancel || isReschedule;

			string m_strMinutesEntryID = string.Empty;
			string m_strTaskID = string.Empty;
			string m_strFPTID = string.Empty;
			string m_strFPTDesc = string.Empty;
			string m_strMailNotificationID = string.Empty;
			string m_strNotificationTemplateID = string.Empty;
			string m_strMinutesTemplateID = string.Empty;
			string m_strScheduleID = string.Empty;
			string m_strStatusID = string.Empty;
			string m_strFunctionID = string.Empty;
			string m_strNotifMapID = string.Empty;
			string m_strSubject = string.Empty;

			List<string> m_lstMessage = new List<string>();
			string m_strMessage = string.Empty;

			List<RecipientsVM> m_objListRecipientsVM = new List<RecipientsVM>();
			List<NotificationValuesVM> m_objListNotificationValuesVM = new List<NotificationValuesVM>();
			List<EntryValuesVM> m_objListEntryValuesVM = new List<EntryValuesVM>();

			MSchedulesDA m_objMSchedulesDA = new MSchedulesDA();
			m_objMSchedulesDA.ConnectionStringName = Global.ConnStrConfigName;

			TMinuteEntriesDA m_objTMinuteEntriesDA = new TMinuteEntriesDA();
			m_objTMinuteEntriesDA.ConnectionStringName = Global.ConnStrConfigName;

			TEntryValuesDA m_objTEntryValuesDA = new TEntryValuesDA();
			m_objTEntryValuesDA.ConnectionStringName = Global.ConnStrConfigName;

			DNotificationMapDA m_objNotificationMapDA = new DNotificationMapDA();
			m_objNotificationMapDA.ConnectionStringName = Global.ConnStrConfigName;

			MFieldTagReferencesDA m_objMFieldTagReferencesDA = new MFieldTagReferencesDA();
			m_objMFieldTagReferencesDA.ConnectionStringName = Global.ConnStrConfigName;

			MTasksDA m_objMTaskDA = new MTasksDA();
			m_objMTaskDA.ConnectionStringName = Global.ConnStrConfigName;

			DTaskDetailsDA m_objDTaskDetailsDA = new DTaskDetailsDA();
			m_objDTaskDetailsDA.ConnectionStringName = Global.ConnStrConfigName;

			MMailNotificationsDA m_objMMailNotifDA = new MMailNotificationsDA();
			m_objMMailNotifDA.ConnectionStringName = Global.ConnStrConfigName;

			DRecipientsDA m_objDRecipientDA = new DRecipientsDA();
			m_objDRecipientDA.ConnectionStringName = Global.ConnStrConfigName;

			TNotificationValuesDA m_objTNotificationValuesDA = new TNotificationValuesDA();
			m_objTNotificationValuesDA.ConnectionStringName = Global.ConnStrConfigName;

			TNotificationAttachmentsDA m_objTNotificationAttachmentsDA = new TNotificationAttachmentsDA();
			m_objTNotificationAttachmentsDA.ConnectionStringName = Global.ConnStrConfigName;

			object m_objDBConnection = null;
			string m_strTransName = "MinutesEntry";
			m_objDBConnection = m_objTMinuteEntriesDA.BeginTrans(m_strTransName);

			MMailNotifications m_objMMailNotifications = new MMailNotifications();

			try
			{
				m_strFPTID = this.Request.Params[MinutesEntryVM.Prop.FPTID.Name];
				m_strFPTID = m_strFPTID == string.Empty ? null : m_strFPTID;
				m_strStatusID = this.Request.Params[MinutesEntryVM.Prop.StatusID.Name];
				m_strNotificationTemplateID = this.Request.Params[MinutesEntryVM.Prop.NotificationTemplateID.Name];
				m_strFPTDesc= this.Request.Params[MinutesEntryVM.Prop.FPTDesc.Name];
				m_strMailNotificationID = this.Request.Params[MinutesEntryVM.Prop.MailNotificationID.Name];
				m_strScheduleID = this.Request.Params[MinutesEntryVM.Prop.ScheduleID.Name];
				m_strMinutesTemplateID = this.Request.Params[MinutesEntryVM.Prop.MinuteTemplateID.Name];
				m_strFunctionID = this.Request.Params[MinutesEntryVM.Prop.FunctionID.Name];
				m_strMinutesEntryID = this.Request.Params[MinutesEntryVM.Prop.MinuteEntryID.Name];
				m_strSubject = this.Request.Params[MinutesEntryVM.Prop.Subject.Name];

				List<string> m_lstSelect = new List<string>();
				m_lstSelect.Add(NotificationMapVM.Prop.NotifMapID.MapAlias);

				Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
				List<object> m_lstFilter = new List<object>();
				//m_lstFilter.Add(Operator.Equals);
				//m_lstFilter.Add(m_strNotificationTemplateID);
				//m_objFilter.Add(NotificationTemplateVM.Prop.NotificationTemplateID.Map, m_lstFilter);

				//m_lstFilter = new List<object>();
				//m_lstFilter.Add(Operator.Equals);
				//m_lstFilter.Add(m_strFunctionID);
				//m_objFilter.Add(FunctionsVM.Prop.FunctionID.Map, m_lstFilter);

				//Dictionary<int, DataSet> m_dicDNotifMapDA = m_objNotificationMapDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
				//if (m_objNotificationMapDA.Success && m_objNotificationMapDA.Message == "")
				//	m_strNotifMapID = m_dicDNotifMapDA[0].Tables[0].Rows[0][SchedulesVM.Prop.NotifMapID.Name].ToString();
				//else
				//	m_lstMessage.Add("NotifMapID not found. Please make sure the selected functions and template are exists. " + m_objNotificationMapDA.Message);

				


				
				if (this.Request.Params[MinutesEntryVM.Prop.ListNotificationValues.Name] != null)
				{
					m_objListNotificationValuesVM = JSON.Deserialize<List<NotificationValuesVM>>(this.Request.Params[MinutesEntryVM.Prop.ListNotificationValues.Name]);
					
				}

				if (this.Request.Params[MinutesEntryVM.Prop.ListRecipients.Name] != null)
				{
					m_objListRecipientsVM= JSON.Deserialize<List<RecipientsVM>>(this.Request.Params[MinutesEntryVM.Prop.ListRecipients.Name]);

				}

				if (this.Request.Params[MinutesEntryVM.Prop.ListMinutesValues.Name] != null)
				{
					m_objListEntryValuesVM = JSON.Deserialize<List<EntryValuesVM>>(this.Request.Params[MinutesEntryVM.Prop.ListMinutesValues.Name]);
				}

				//TODO
				m_lstMessage = IsSaveValid(Action, m_strFunctionID,m_strNotificationTemplateID,m_strMinutesTemplateID,m_strSubject);

				#region Task & History 
				//Mtask
				if (m_lstMessage.Count <= 0)
				{
					MTasks m_objMTask = new MTasks();
					m_objMTaskDA.Data = m_objMTask;
					if (!isAdd)
					{
						m_objMTask.TaskID = this.Request.Params[SchedulesVM.Prop.TaskID.Name];
						m_objMTaskDA.Select();
					}
					string m_strowner = "";
					string CurrentApprovalRole = GetCurrentApproval(General.EnumDesc(Enum.TaskType.MinutesOfMeeting), 0);
					m_objMTask.TaskTypeID = General.EnumDesc(TaskType.MinutesOfMeeting);
					m_objMTask.TaskDateTimeStamp = DateTime.Now;
					m_objMTask.TaskOwnerID = GetParentApproval(ref m_strowner, CurrentApprovalRole, General.EnumDesc(Enum.TaskType.MinutesOfMeeting));
					m_objMTask.StatusID = (int)TaskStatus.Draft;
					m_objMTask.CurrentApprovalLvl = 0;
					m_objMTask.Remarks = isAdd ? "New From Minutes" : "Update From Minutes";

					if (isAdd)
						m_objMTaskDA.Insert(true, m_objDBConnection);
					else
						m_objMTaskDA.Update(true, m_objDBConnection);

					if (!m_objMTaskDA.Success || m_objMTaskDA.Message != string.Empty)
						m_lstMessage.Add(m_objMTaskDA.Message);
				}
				//DTaskDetails
				if (m_objMTaskDA.Success && m_lstMessage.Count <= 0)
				{
					DTaskDetails m_obTaskDetail = new DTaskDetails();
					m_objDTaskDetailsDA.Data = m_obTaskDetail;
					m_obTaskDetail.TaskDetailID = Guid.NewGuid().ToString().Replace("-", "");
					m_obTaskDetail.TaskID = isAdd ? m_objMTaskDA.Data.TaskID : this.Request.Params[SchedulesVM.Prop.TaskID.Name];
					m_obTaskDetail.StatusID = (int)TaskStatus.Draft;
					m_obTaskDetail.Remarks = "Draft Created Minutes of meeting";
					m_objDTaskDetailsDA.Insert(true, m_objDBConnection);

					if (!m_objDTaskDetailsDA.Success || m_objDTaskDetailsDA.Message != string.Empty)
						m_lstMessage.Add(m_objDTaskDetailsDA.Message);

				}

				#endregion

				#region Notifications
				if (!m_lstMessage.Any())
				{
					m_objMMailNotifications.MailNotificationID = isAdd ? Guid.NewGuid().ToString().Replace("-", "") : m_strMailNotificationID;
					
					m_objMMailNotifDA.Data = m_objMMailNotifications;

					if (isUpdate)
						m_objMMailNotifDA.Select();

					NotificationTemplateVM obj = GetNotificationTemplateVM(m_strNotificationTemplateID);
					Dictionary<string, string> m_dicValues = new Dictionary<string, string>();
					foreach (NotificationValuesVM objNotificationValuesVM in m_objListNotificationValuesVM)
					{
						m_dicValues.Add(objNotificationValuesVM.FieldTagID, objNotificationValuesVM.Value);
					}
					m_objMMailNotifications.Importance = true;//TODO
					m_objMMailNotifications.Subject = m_strSubject ;
					m_objMMailNotifications.StatusID = (int)NotificationStatus.Draft;
					m_objMMailNotifications.FPTID = m_strFPTID;
					//m_objMMailNotifications.NotifMapID = m_strNotifMapID;
					m_objMMailNotifications.FunctionID = m_strFunctionID;
					m_objMMailNotifications.NotificationTemplateID = m_strNotificationTemplateID;
					m_objMMailNotifications.Contents = Global.ParseParameter(obj.Contents, m_dicValues,m_objListRecipientsVM,ref m_strMessage, null);
					m_objMMailNotifications.TaskID = m_objMTaskDA.Data.TaskID;

					m_strTaskID = m_objMMailNotifications.TaskID;

					m_objMMailNotifDA.Data = m_objMMailNotifications;

					if (isAdd)
						m_objMMailNotifDA.Insert(true, m_objDBConnection);
					else 
						m_objMMailNotifDA.Update(true, m_objDBConnection);

					if (!m_objMMailNotifDA.Success || m_objMMailNotifDA.Message != string.Empty)
						m_lstMessage.Add(m_objMMailNotifDA.Message);

				}
				#endregion

				#region TNotificationValues
				if (m_lstMessage.Count <= 0)
				{
					if (isUpdate)//Delete Insert
					{
						Dictionary<string, List<object>> m_dicNotificationValues = new Dictionary<string, List<object>>();
						List<object> m_lstFilterNotificationValues = new List<object>();
						m_lstFilterNotificationValues.Add(Operator.Equals);
						m_lstFilterNotificationValues.Add(m_objMMailNotifDA.Data.MailNotificationID);
						m_dicNotificationValues.Add(NotificationValuesVM.Prop.MailNotificationID.Map, m_lstFilterNotificationValues);
						m_objTNotificationValuesDA.DeleteBC(m_dicNotificationValues, true, m_objDBConnection);
					}

					foreach (NotificationValuesVM objNotValNotificationValuesVM in m_objListNotificationValuesVM)
					{
						TNotificationValues m_objTNotificationValues = new TNotificationValues();
						m_objTNotificationValues.NotificationValueID = Guid.NewGuid().ToString().Replace("-", "");
						m_objTNotificationValues.MailNotificationID = m_objMMailNotifDA.Data.MailNotificationID;
						m_objTNotificationValues.FieldTagID = objNotValNotificationValuesVM.FieldTagID;
						m_objTNotificationValues.Value = objNotValNotificationValuesVM.Value;

						m_objTNotificationValuesDA.Data = m_objTNotificationValues;

						m_objTNotificationValuesDA.Insert(true, m_objDBConnection);

						if (!m_objTNotificationValuesDA.Success || m_objTNotificationValuesDA.Message != string.Empty)
							m_lstMessage.Add(m_objTNotificationValuesDA.Message);
					}
				}
				#endregion

				#region Recipient               
				if (!m_lstMessage.Any())
				{
					if (isUpdate)//Delete Insert
					{
						Dictionary<string, List<object>> m_dicFilterNotificationRecipient = new Dictionary<string, List<object>>();
						List<object> m_lstFilterNotificationRecipient = new List<object>();
						m_lstFilterNotificationRecipient.Add(Operator.Equals);
						m_lstFilterNotificationRecipient.Add(m_objMMailNotifDA.Data.MailNotificationID);
						m_dicFilterNotificationRecipient.Add(RecipientsVM.Prop.MailNotificationID.Map, m_lstFilterNotificationRecipient);
						m_objDRecipientDA.DeleteBC(m_dicFilterNotificationRecipient, true, m_objDBConnection);
					}

					foreach (RecipientsVM objRecipientsVM in m_objListRecipientsVM)
					{
						DRecipients objDRecipient = new DRecipients();

						objDRecipient.OwnerID = objRecipientsVM.OwnerID;
						objDRecipient.RecipientID = Guid.NewGuid().ToString().Replace("-", "");
						objDRecipient.RecipientDesc = objRecipientsVM.RecipientDesc;
						objDRecipient.RecipientTypeID = objRecipientsVM.RecipientTypeID;
						objDRecipient.MailAddress = objRecipientsVM.MailAddress;
						objDRecipient.MailNotificationID = m_objMMailNotifDA.Data.MailNotificationID;
						m_objDRecipientDA.Data = objDRecipient;

						m_objDRecipientDA.Insert(true, m_objDBConnection);

						if (!m_objDRecipientDA.Success || m_objDRecipientDA.Message != string.Empty)
							m_lstMessage.Add(m_objDRecipientDA.Message);
					}
				}
				#endregion

				#region Minutes Entry
				if (m_lstMessage.Count <= 0)
				{
					TMinuteEntries m_objTMinuteEntries = new TMinuteEntries();
					m_objTMinuteEntries.MinuteEntryID = string.IsNullOrEmpty(m_strMinutesEntryID) ? Guid.NewGuid().ToString().Replace("-", ""): m_strMinutesEntryID;
					m_objTMinuteEntriesDA.Data = m_objTMinuteEntries;

					if (isUpdate)
						m_objTMinuteEntriesDA.Select();

					m_objTMinuteEntries.MinuteTemplateID = m_strMinutesTemplateID;
					m_objTMinuteEntries.Subject = m_strSubject;
					m_objTMinuteEntries.FPTID = m_strFPTID;
					m_objTMinuteEntries.TaskID = m_objMTaskDA.Data.TaskID;
					m_objTMinuteEntries.MailNotificationID = m_objMMailNotifDA.Data.MailNotificationID;
					m_objTMinuteEntries.ScheduleID = m_strScheduleID;
					m_objTMinuteEntries.StatusID = (int)MinutesStatus.Draft;


					m_objTMinuteEntriesDA.Data = m_objTMinuteEntries;

					if (isAdd)
						m_objTMinuteEntriesDA.Insert(true, m_objDBConnection);
					else
						m_objTMinuteEntriesDA.Update(true, m_objDBConnection);

					if (!m_objTMinuteEntriesDA.Success || m_objTMinuteEntriesDA.Message != string.Empty)
						m_lstMessage.Add(m_objTMinuteEntriesDA.Message);
				}
				#endregion

				#region Minutes Entry Values
				if (m_lstMessage.Count <= 0)
				{
					if (isUpdate)//Delete Insert
					{
						Dictionary<string, List<object>> m_dicFilterEntriesValues = new Dictionary<string, List<object>>();
						List<object> m_lstFilterEntriesValues = new List<object>();
						m_lstFilterEntriesValues.Add(Operator.Equals);
						m_lstFilterEntriesValues.Add(m_objTMinuteEntriesDA.Data.MinuteEntryID);
						m_dicFilterEntriesValues.Add(EntryValuesVM.Prop.MinuteEntryID.Map, m_lstFilterEntriesValues);
						m_objTEntryValuesDA.DeleteBC(m_dicFilterEntriesValues, true, m_objDBConnection);
					}

					foreach (EntryValuesVM objEntryValuesVM in m_objListEntryValuesVM)
					{
						TEntryValues m_objTEntryValues = new TEntryValues();
						m_objTEntryValues.EntryValueID = Guid.NewGuid().ToString().Replace("-", "");
						m_objTEntryValues.MinuteEntryID = m_objTMinuteEntriesDA.Data.MinuteEntryID;
						m_objTEntryValues.FieldTagID = objEntryValuesVM.FieldTagID;
						m_objTEntryValues.Value = objEntryValuesVM.Value;

						m_objTEntryValuesDA.Data = m_objTEntryValues;

						m_objTEntryValuesDA.Insert(true, m_objDBConnection);
						
						if (!m_objTEntryValuesDA.Success || m_objTEntryValuesDA.Message != string.Empty)
							m_lstMessage.Add(m_objTEntryValuesDA.Message);
					}
				}

				#region Attachment               
				if (m_lstMessage.Count <= 0)
				{
					List<ConfigVM> m_lConfigVM = GetConfig(nameof(TMinuteEntries), MinutesEntryVM.Prop.NotificationTemplateID.Name).ToList();
					string m_strHeaderMOMID = string.Empty;
					string m_strFooterMOMID = string.Empty;

					if (m_lConfigVM.Any())
					{
						m_strHeaderMOMID = m_lConfigVM.FirstOrDefault(d => d.Key3.Equals("Header")).Desc1;
						if (m_lConfigVM.Any(d => d.Key3.Equals("Footer") && d.Key4== m_strFunctionID))
						m_strFooterMOMID = m_lConfigVM.FirstOrDefault(d => d.Key3.Equals("Footer") && d.Key4 == m_strFunctionID).Desc1;
						if(string.IsNullOrEmpty(m_strFooterMOMID))
							m_strFooterMOMID = m_lConfigVM.FirstOrDefault(d => d.Key3.Equals("Footer") && d.Key4 == "").Desc1;

					}

					if (isUpdate)//Delete Insert
					{
						Dictionary<string, List<object>> m_dicFilterNotificationAttachment = new Dictionary<string, List<object>>();
						List<object> m_lstFilterNotificationAttachment = new List<object>();
						m_lstFilterNotificationAttachment.Add(Operator.Equals);
						m_lstFilterNotificationAttachment.Add(m_objMMailNotifDA.Data.MailNotificationID);
						m_dicFilterNotificationAttachment.Add(NotificationAttachmentVM.Prop.MailNotificationID.Map, m_lstFilterNotificationAttachment);
						m_objTNotificationAttachmentsDA.DeleteBC(m_dicFilterNotificationAttachment, true, m_objDBConnection);
					}

					string m_strRawData = string.Empty;
					//MemoryStream m_memoryStream = new MemoryStream();
					MinutesTemplateVM m_objMinutesTemplateVM= GetMinutesTemplateVM(m_strMinutesTemplateID);
					byte[] fileData = null;

					#region Create Pdf
					try
					{

						PdfPageSize pageSize = PdfPageSize.A4;

						PdfPageOrientation pdfOrientation = PdfPageOrientation.Portrait;

						int m_webPageWidth = 800;
						int m_webPageHeight = 0;
						int m_iMarginTop = 25;
						int m_iMarginBottom = 20;
						int m_iMarginLeft = 50;
						int m_iMarginRight = 50;

						// instantiate a html to pdf converter object
						HtmlToPdf m_pdfConverter = new HtmlToPdf();

						// set converter options
						m_pdfConverter.Options.PdfPageSize = pageSize;
						m_pdfConverter.Options.PdfPageOrientation = pdfOrientation;
						m_pdfConverter.Options.WebPageWidth = m_webPageWidth;
						m_pdfConverter.Options.WebPageHeight = m_webPageHeight;
						m_pdfConverter.Options.MarginLeft = m_iMarginLeft;
						m_pdfConverter.Options.MarginRight = m_iMarginRight;
						m_pdfConverter.Options.MarginTop = m_iMarginTop;
						m_pdfConverter.Options.MarginBottom = m_iMarginBottom;

						Dictionary<string, string> m_dicValues = new Dictionary<string, string>();
						foreach (EntryValuesVM objEntryValuesVM in m_objListEntryValuesVM)
						{
							m_dicValues.Add(objEntryValuesVM.FieldTagID, objEntryValuesVM.Value);
						}
						string m_strFileBody = Global.ParseParameter(m_objMinutesTemplateVM.Contents,m_dicValues);


						// header settings
						int m_iheaderHeight = 120;
						m_pdfConverter.Options.DisplayHeader = true;
						m_pdfConverter.Header.Height = m_iheaderHeight;

						NotificationTemplateVM m_NotificationTemplateVM = GetNotificationTemplateVM(m_strHeaderMOMID);
						// set minutes values - headers
						m_dicValues = new Dictionary<string, string>();
						foreach (EntryValuesVM objEntryValuesVM in m_objListEntryValuesVM)
						{
							m_dicValues.Add(objEntryValuesVM.FieldTagID, objEntryValuesVM.Value);
						}
						string m_strHeader = m_NotificationTemplateVM != null ? m_NotificationTemplateVM.Contents : string.Empty;

						string m_strContentHeaders = string.Empty;
						if (!string.IsNullOrEmpty(m_strHeader))
							m_strContentHeaders = Global.ParseParameter(m_strHeader, m_dicValues);

						PdfHtmlSection m_pdfheaderHtml = new PdfHtmlSection(m_strContentHeaders, "");
						m_pdfheaderHtml.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;
						m_pdfheaderHtml.WebPageWidth = m_webPageWidth;


						m_pdfConverter.Header.Add(m_pdfheaderHtml);

						// footer settings
						int footerHeight = 100;
						m_pdfConverter.Options.DisplayFooter = true;
						m_pdfConverter.Footer.Height = footerHeight;

						m_NotificationTemplateVM = GetNotificationTemplateVM(m_strFooterMOMID);
						m_dicValues = new Dictionary<string, string>();
						foreach (EntryValuesVM objEntryValuesVM in m_objListEntryValuesVM)
						{
							m_dicValues.Add(objEntryValuesVM.FieldTagID, objEntryValuesVM.Value);
						}
						string m_strFooter = m_NotificationTemplateVM != null ? m_NotificationTemplateVM.Contents : string.Empty;
						string m_strContentFooters = string.Empty;
						if (!string.IsNullOrEmpty(m_strHeader))
							m_strContentFooters = Global.ParseParameter(m_strFooter, m_dicValues);

						PdfHtmlSection footerHtml = new PdfHtmlSection(m_strContentFooters, "");
						footerHtml.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;
						footerHtml.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;

						m_pdfConverter.Footer.Add(footerHtml);

						// page numbers can be added using a PdfTextSection object
						PdfTextSection text = new PdfTextSection(0, 80,
							"Page: {page_number} of {total_pages}  ",
							new System.Drawing.Font("Arial", 8));
						text.HorizontalAlign = PdfTextHorizontalAlign.Right;
						m_pdfConverter.Footer.Add(text);

						// create a new pdf document converting an url
						PdfDocument m_pdfDocument = m_pdfConverter.ConvertHtmlString(m_strFileBody, string.Empty);


						// save pdf document
						fileData = m_pdfDocument.Save();
						//using (var binaryReader = new BinaryReader(m_memoryStream))
						//    fileData = binaryReader.ReadBytes(m_memoryStream.Capacity);

						// close pdf document
						m_pdfDocument.Close();
					}
					catch (Exception)
					{

						throw;
					}
					#endregion

					TNotificationAttachments m_objTNotificationAttachments = new TNotificationAttachments();
					m_objTNotificationAttachments.AttachmentValueID = Guid.NewGuid().ToString().Replace("-", "");
					m_objTNotificationAttachments.Filename = $"MOM - {m_strSubject}"; ;
					m_objTNotificationAttachments.ContentType = ".pdf";
					m_objTNotificationAttachments.RawData = Convert.ToBase64String(fileData); //TODO: generate
					//m_memoryStream.Flush();
					m_objTNotificationAttachments.MailNotificationID = m_objMMailNotifDA.Data.MailNotificationID;
					m_objTNotificationAttachmentsDA.Data = m_objTNotificationAttachments;

					m_objTNotificationAttachmentsDA.Insert(true, m_objDBConnection);


					if (!m_objTNotificationAttachmentsDA.Success || m_objTNotificationAttachmentsDA.Message != string.Empty)
						m_lstMessage.Add(m_objTNotificationAttachmentsDA.Message);

				}
				#endregion
				#endregion


			}
			catch (Exception ex)
			{
				m_lstMessage.Add(ex.Message);
				m_objTMinuteEntriesDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
			}
			if (m_lstMessage.Count <= 0)
			{

				m_objTMinuteEntriesDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
				Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
				if (isAdd)
					return Detail(General.EnumDesc(Buttons.ButtonSave), null, m_objTMinuteEntriesDA.Data.MinuteEntryID);
				else
					return Detail(General.EnumDesc(Buttons.ButtonSave), null);
			}
			m_objTMinuteEntriesDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
			Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
			return this.Direct(true);
		}

		public ActionResult Verify(string Selected)
		{
			////if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Verify)))
			////    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

			List<string> m_lstMessage = new List<string>();
			List<MailNotificationsVM> m_lstSelectedRow = new List<MailNotificationsVM>();
			MinutesEntryVM m_lstSelectedRowMinutesEntry = new MinutesEntryVM();
			m_lstSelectedRowMinutesEntry = JSON.Deserialize<MinutesEntryVM>(Selected);
			

			switch (m_lstSelectedRowMinutesEntry.StatusID)
			{
				case (int)MinutesStatus.Draft: break;
				default:
					{
						Global.ShowErrorAlert(title, MinutesEntryVM.Prop.StatusDesc.Desc + " " + General.EnumDesc(MessageLib.invalid));

						return this.Direct();
					}
			}

			TMinuteEntriesDA m_TMinuteEntriesDA = new TMinuteEntriesDA();
			MMailNotificationsDA m_MMailNotificationsDA = new MMailNotificationsDA();
			MTasksDA m_objMTasksDA = new MTasksDA();
			DTaskDetailsDA m_objDTaskDetailsDA = new DTaskDetailsDA();
			//todo: load data

			m_objMTasksDA.ConnectionStringName = Global.ConnStrConfigName;
			string m_strTransName = "MTasks";
			object m_objDBConnection = null;
			m_objDBConnection = m_objMTasksDA.BeginTrans(m_strTransName);
			string m_strMailNotificationID = string.Empty;
			string m_strTaskID = string.Empty;
			//insert project & BP upload
			try
			{
				//m_strMailNotificationID = m_lstSelectedRowMinutesEntry.FirstOrDefault().MailNotificationID;
				m_strTaskID = m_lstSelectedRowMinutesEntry.TaskID;
				string m_strowner = "";
				//string CurrentApprovalRole = GetCurrentApproval(General.EnumDesc(Enum.TaskType.MinutesOfMeeting), 0);
				//Insert MTasks
				MTasks m_objMTasks = new MTasks { TaskID = m_strTaskID };
				m_objMTasksDA.Data = m_objMTasks;
				m_objMTasksDA.Select();


				string m_strCurrentApprovalRole = GetCurrentApproval(General.EnumDesc(Enum.TaskType.MinutesOfMeeting), m_objMTasksDA.Data.CurrentApprovalLvl);

				m_objMTasks.TaskTypeID = General.EnumDesc(TaskType.MinutesOfMeeting);
				m_objMTasks.TaskDateTimeStamp = DateTime.Now;
				m_objMTasks.TaskOwnerID = GetParentApproval(ref m_strowner, m_strCurrentApprovalRole, General.EnumDesc(Enum.TaskType.MinutesOfMeeting));
				m_objMTasks.StatusID = (int)TaskStatus.InProgress;
				m_objMTasks.CurrentApprovalLvl = 1;
				m_objMTasks.Remarks = "Minutes of meeting";
				m_objMTasks.Summary = "";

				if (!string.IsNullOrEmpty(m_strowner)) m_lstMessage.Add(m_strowner);

				m_objMTasksDA.Data = m_objMTasks;
				m_objMTasksDA.Update(true, m_objDBConnection);
				if (!m_objMTasksDA.Success || m_objMTasksDA.Message != string.Empty)
				{
					m_objMTasksDA.EndTrans(ref m_objDBConnection, m_strTransName);
					return this.Direct(false, m_objMTasksDA.Message);
				}
				m_strTaskID = m_objMTasksDA.Data.TaskID;
				//Insert DTaskDetails
				m_objDTaskDetailsDA.ConnectionStringName = Global.ConnStrConfigName;
				DTaskDetails m_objDTaskDetails = new DTaskDetails()
				{
					TaskDetailID = Guid.NewGuid().ToString().Replace("-", ""),
					TaskID = m_strTaskID,
					StatusID = (int)TaskStatus.InProgress,
					Remarks = "Minutes of meeting",

				};
				m_objDTaskDetailsDA.Data = m_objDTaskDetails;
				//m_objDTaskDetailsDA.Select();
				m_objDTaskDetailsDA.Insert(true, m_objDBConnection);
				if (!m_objDTaskDetailsDA.Success || m_objDTaskDetailsDA.Message != string.Empty)
				{
					m_objDTaskDetailsDA.EndTrans(ref m_objDBConnection, m_strTransName);
					return this.Direct(false, m_objDTaskDetailsDA.Message);
				}
				//Update Minutes status
				Dictionary<string, List<object>> m_dicSet = new Dictionary<string, List<object>>();
				Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
				List<object> m_lstFilter = new List<object>();
				m_lstFilter.Add(Operator.Equals);
				m_lstFilter.Add(m_lstSelectedRowMinutesEntry.MinuteEntryID);
				m_objFilter.Add(MinutesEntryVM.Prop.MinuteEntryID.Map, m_lstFilter);

				List<object> m_lstSet = new List<object>();
				m_lstSet.Add(typeof(int));
				m_lstSet.Add((int)MinutesStatus.Verified);
				m_dicSet.Add(MinutesEntryVM.Prop.StatusID.Map, m_lstSet);
				m_TMinuteEntriesDA.UpdateBC(m_dicSet, m_objFilter, true, m_objDBConnection);
				if (!m_TMinuteEntriesDA.Success || m_TMinuteEntriesDA.Message != string.Empty)
				{
					m_objMTasksDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
					return this.Direct(false, m_TMinuteEntriesDA.Message);
				}
			}
			catch (Exception ex)
			{
				m_lstMessage.Add(ex.Message);
				return this.Direct(false, ex.Message);
			}
			if (m_lstMessage.Count <= 0)
			{
				string m_insmessage = string.Empty;
				Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Verified));
				m_objMTasksDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
				return this.Direct(true, string.Empty);
			}
			else
			{
				m_objMTasksDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
				return this.Direct(false, String.Join(Global.NewLineSeparated, m_lstMessage));
			}

		}

		#endregion

		#region Direct Method

		public ActionResult GetDivision(string ControlDivisionID, string ControlDivisionDesc, string FilterDivisionID, string FilterDivisionDesc, bool Exact = false)
		{
			try
			{
				Dictionary<int, List<DivisionVM>> m_dicDivisionData = GetDivisionData(true, FilterDivisionID, FilterDivisionDesc);
				KeyValuePair<int, List<DivisionVM>> m_kvpDivisionVM = m_dicDivisionData.AsEnumerable().ToList()[0];
				if (m_kvpDivisionVM.Key < 1 || (m_kvpDivisionVM.Key > 1 && Exact))
					return this.Direct(false);
				else if (m_kvpDivisionVM.Key > 1 && !Exact)
					return Browse(ControlDivisionID, ControlDivisionDesc, FilterDivisionID, FilterDivisionDesc);

				m_dicDivisionData = GetDivisionData(false, FilterDivisionID, FilterDivisionDesc);
				DivisionVM m_objDivisionVM = m_dicDivisionData[0][0];
				this.GetCmp<TextField>(ControlDivisionID).Value = m_objDivisionVM.DivisionID;
				this.GetCmp<TextField>(ControlDivisionDesc).Value = m_objDivisionVM.DivisionDesc;
				return this.Direct(true);
			}
			catch (Exception ex)
			{
				return this.Direct(false, ex.Message);
			}
		}

		public ActionResult GetListAttendanceByScheduleID(string ScheduleID)
		{
			List<RecipientsVM> m_objListScheduleRecipient = new List<RecipientsVM>();
			DRecipientsDA m_objDRecipientsDA = new DRecipientsDA();
			m_objDRecipientsDA.ConnectionStringName = Global.ConnStrConfigName;

			List<string> m_lstSelect = new List<string>();
			m_lstSelect.Add(RecipientsVM.Prop.RecipientID.MapAlias);
			m_lstSelect.Add(RecipientsVM.Prop.MailNotificationID.MapAlias);
			m_lstSelect.Add(RecipientsVM.Prop.RecipientDesc.MapAlias);
			m_lstSelect.Add(RecipientsVM.Prop.RecipientTypeID.MapAlias);
			m_lstSelect.Add(RecipientsVM.Prop.OwnerID.MapAlias);
			m_lstSelect.Add(RecipientsVM.Prop.MailAddress.MapAlias);
			m_lstSelect.Add(RecipientsVM.Prop.RecipientTypeDesc.MapAlias);

			Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
			List<object> m_lstFilter = new List<object>();
			m_lstFilter.Add(Operator.Equals);
			m_lstFilter.Add(ScheduleID);
			m_objFilter.Add(RecipientsVM.Prop.ScheduleID.Map, m_lstFilter);

			Dictionary<int, DataSet> m_dicDRecipientsDA = m_objDRecipientsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
			if (m_objDRecipientsDA.Message == string.Empty)
			{
				foreach (DataRow m_drDRecipientsDA in m_dicDRecipientsDA[0].Tables[0].Rows)
				{
					RecipientsVM objRecipient = new RecipientsVM();
					objRecipient.RecipientID = m_drDRecipientsDA[RecipientsVM.Prop.RecipientID.Name].ToString();
					objRecipient.MailNotificationID = m_drDRecipientsDA[RecipientsVM.Prop.MailNotificationID.Name].ToString();
					objRecipient.OwnerID = m_drDRecipientsDA[RecipientsVM.Prop.OwnerID.Name].ToString();
					objRecipient.RecipientDesc = m_drDRecipientsDA[RecipientsVM.Prop.RecipientDesc.Name].ToString();
					objRecipient.RecipientTypeID = m_drDRecipientsDA[RecipientsVM.Prop.RecipientTypeID.Name].ToString();
					objRecipient.RecipientTypeDesc = m_drDRecipientsDA[RecipientsVM.Prop.RecipientTypeDesc.Name].ToString();
					objRecipient.MailAddress = m_drDRecipientsDA[RecipientsVM.Prop.MailAddress.Name].ToString();

					m_objListScheduleRecipient.Add(objRecipient);
				}
			}
			return this.Store(m_objListScheduleRecipient, m_objListScheduleRecipient.Count);
		}

		public ActionResult GetListTemplateTagsVM(string TemplateID,string TemplateType,string ScheduleID, string FunctionDesc)
		{
			string m_strMessage = string.Empty;
			string m_strType = string.Empty;
			string m_strFormat = string.Empty;
			string m_strCultureInfo = string.Empty;

			SchedulesVM m_objSchedulesVM = GetScheduleData(ScheduleID,ref m_strMessage);


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

			m_lstFilter = new List<object>();
			m_lstFilter.Add(Operator.NotEqual);
			m_lstFilter.Add("System_");
			m_objFilter.Add(NotificationValuesVM.Prop.RefTable.Map, m_lstFilter);
            


            Dictionary<int, DataSet> m_dicTNotificationValuesDA = m_objDTemplateTagsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);

			PropertyInfo[] m_arrPInfoSchedule = m_objSchedulesVM.GetType().GetProperties();
			if (m_objDTemplateTagsDA.Success)
			{
				foreach (DataRow m_drTNotificationValuesDA in m_dicTNotificationValuesDA[0].Tables[0].Rows)
				{
					TemplateTagsVM m_objNotificationValuesVM = new TemplateTagsVM();
					m_objNotificationValuesVM.TemplateTagID = m_drTNotificationValuesDA[TemplateTagsVM.Prop.TemplateTagID.Name].ToString();
					m_objNotificationValuesVM.TemplateID = m_drTNotificationValuesDA[TemplateTagsVM.Prop.TemplateID.Name].ToString();
					m_objNotificationValuesVM.FieldTagID = m_drTNotificationValuesDA[TemplateTagsVM.Prop.FieldTagID.Name].ToString();
					m_objNotificationValuesVM.TagDesc = m_drTNotificationValuesDA[TemplateTagsVM.Prop.TagDesc.Name].ToString();
					m_objNotificationValuesVM.TemplateType = m_drTNotificationValuesDA[TemplateTagsVM.Prop.TemplateType.Name].ToString();

					m_objNotificationValuesVM.RefTable = m_drTNotificationValuesDA[TemplateTagsVM.Prop.RefTable.Name].ToString();
					m_objNotificationValuesVM.RefIDColumn = m_drTNotificationValuesDA[TemplateTagsVM.Prop.RefIDColumn.Name].ToString();
					m_objNotificationValuesVM.Config = GetConfig(m_objNotificationValuesVM.FieldTagID).FirstOrDefault();

					m_strType = string.Empty;
					m_strFormat = string.Empty;
					m_strCultureInfo = string.Empty;
					if (m_objNotificationValuesVM.Config != null)
					{
						m_strType = m_objNotificationValuesVM.Config.Key2;
						m_strFormat = m_objNotificationValuesVM.Config.Key3;
						m_strCultureInfo = m_objNotificationValuesVM.Config.Key4;
					}
					PropertyInfo m_proInfoSchedule = m_arrPInfoSchedule.FirstOrDefault(d => d.Name.Equals(m_objNotificationValuesVM.RefIDColumn));
					if (m_proInfoSchedule != null && !string.IsNullOrEmpty(m_objSchedulesVM.ScheduleID))
					{
						string m_strVal = m_proInfoSchedule.GetValue(m_objSchedulesVM)==null?string.Empty : m_proInfoSchedule.GetValue(m_objSchedulesVM).ToString();

						m_objNotificationValuesVM.Value = m_proInfoSchedule != null ? m_strVal : string.Empty;

						if (m_strType == nameof(DateTime))
							m_objNotificationValuesVM.Value = DateTime.Parse(m_arrPInfoSchedule.FirstOrDefault(d => d.Name.Equals(m_objNotificationValuesVM.RefIDColumn)).GetValue(m_objSchedulesVM).ToString()).ToString(m_strFormat, new System.Globalization.CultureInfo(m_strCultureInfo));
					}

                    if(!m_lstTemplateTagsVM.Any(d=>d.FieldTagID== m_objNotificationValuesVM.FieldTagID))
					m_lstTemplateTagsVM.Add(m_objNotificationValuesVM);
				}
			}

			List<string> m_FieldTagIDs = m_lstTemplateTagsVM.Select(d => d.FieldTagID).ToList();

			List<TemplateTagsVM> m_HMOMlstTemplateTagsVM = new List<TemplateTagsVM>();
			if (TemplateType == "MOM")
			{
				List<ConfigVM> m_lConfigVM = GetConfig(nameof(TMinuteEntries), MinutesEntryVM.Prop.NotificationTemplateID.Name).ToList();
				string m_strHeaderFooterMOMID = string.Empty;
				if (m_lConfigVM.Any()) {
					m_strHeaderFooterMOMID =string.Join(",",m_lConfigVM.Select(d=>d.Desc1).ToList());
				}

				m_objDTemplateTagsDA = new DTemplateTagsDA();
				m_objDTemplateTagsDA.ConnectionStringName = Global.ConnStrConfigName;

				m_lstSelect = new List<string>();
				m_lstSelect.Add(TemplateTagsVM.Prop.TemplateTagID.MapAlias);
				m_lstSelect.Add(TemplateTagsVM.Prop.TemplateID.MapAlias);
				m_lstSelect.Add(TemplateTagsVM.Prop.FieldTagID.MapAlias);
				m_lstSelect.Add(TemplateTagsVM.Prop.TagDesc.MapAlias);
				m_lstSelect.Add(TemplateTagsVM.Prop.TemplateType.MapAlias);
				m_lstSelect.Add(TemplateTagsVM.Prop.RefTable.MapAlias);
				m_lstSelect.Add(TemplateTagsVM.Prop.RefIDColumn.MapAlias);

                m_lstKey = new List<string>();
				m_objFilter = new Dictionary<string, List<object>>();
				m_lstFilter = new List<object>();
				m_lstFilter.Add(Operator.In);
				m_lstFilter.Add(m_strHeaderFooterMOMID);
				m_objFilter.Add(TemplateTagsVM.Prop.TemplateID.Map, m_lstFilter);

				m_lstFilter = new List<object>();
				m_lstFilter.Add(Operator.NotIn);
				m_lstFilter.Add(string.Join(",",m_FieldTagIDs));
				m_objFilter.Add(TemplateTagsVM.Prop.FieldTagID.Map, m_lstFilter);

				m_dicTNotificationValuesDA = m_objDTemplateTagsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);

				m_arrPInfoSchedule = m_objSchedulesVM.GetType().GetProperties();
				if (m_objDTemplateTagsDA.Success)
				{
					foreach (DataRow m_drTNotificationValuesDA in m_dicTNotificationValuesDA[0].Tables[0].Rows)
					{
						TemplateTagsVM m_objNotificationValuesVM = new TemplateTagsVM();
						m_objNotificationValuesVM.TemplateTagID = m_drTNotificationValuesDA[TemplateTagsVM.Prop.TemplateTagID.Name].ToString();
						m_objNotificationValuesVM.TemplateID = m_drTNotificationValuesDA[TemplateTagsVM.Prop.TemplateID.Name].ToString();
						m_objNotificationValuesVM.FieldTagID = m_drTNotificationValuesDA[TemplateTagsVM.Prop.FieldTagID.Name].ToString();
						m_objNotificationValuesVM.TagDesc = m_drTNotificationValuesDA[TemplateTagsVM.Prop.TagDesc.Name].ToString();
						m_objNotificationValuesVM.TemplateType = m_drTNotificationValuesDA[TemplateTagsVM.Prop.TemplateType.Name].ToString();

						m_objNotificationValuesVM.RefTable = m_drTNotificationValuesDA[TemplateTagsVM.Prop.RefTable.Name].ToString();
						m_objNotificationValuesVM.RefIDColumn = m_drTNotificationValuesDA[TemplateTagsVM.Prop.RefIDColumn.Name].ToString();
						m_objNotificationValuesVM.Config = GetConfig(m_objNotificationValuesVM.FieldTagID).FirstOrDefault();

						m_strType = string.Empty;
						m_strFormat = string.Empty;
						m_strCultureInfo = string.Empty;
						if (m_objNotificationValuesVM.Config != null)
						{
							m_strType = m_objNotificationValuesVM.Config.Key2;
							m_strFormat = m_objNotificationValuesVM.Config.Key3;
							m_strCultureInfo = m_objNotificationValuesVM.Config.Key4;
						}
						if (m_objNotificationValuesVM.RefIDColumn == MinutesEntryVM.Prop.FunctionDesc.Name)
							m_objNotificationValuesVM.Value = FunctionDesc;

						PropertyInfo m_proInfoSchedule = m_arrPInfoSchedule.FirstOrDefault(d => d.Name.Equals(m_objNotificationValuesVM.RefIDColumn));
						if (m_proInfoSchedule != null && !string.IsNullOrEmpty(m_objSchedulesVM.ScheduleID))
						{
							string m_strVal = m_proInfoSchedule.GetValue(m_objSchedulesVM) == null ? string.Empty : m_proInfoSchedule.GetValue(m_objSchedulesVM).ToString();

							m_objNotificationValuesVM.Value = m_proInfoSchedule != null ? m_strVal : string.Empty;

							if (m_strType == nameof(DateTime))
								m_objNotificationValuesVM.Value = DateTime.Parse(m_arrPInfoSchedule.FirstOrDefault(d => d.Name.Equals(m_objNotificationValuesVM.RefIDColumn)).GetValue(m_objSchedulesVM).ToString()).ToString(m_strFormat, new System.Globalization.CultureInfo(m_strCultureInfo));
						}
                        if (!m_HMOMlstTemplateTagsVM.Any(d => d.FieldTagID == m_objNotificationValuesVM.FieldTagID))
                            m_HMOMlstTemplateTagsVM.Add(m_objNotificationValuesVM);
					}
				}
			}
			m_lstTemplateTagsVM = m_lstTemplateTagsVM.Union(m_HMOMlstTemplateTagsVM).OrderBy(d=>d.FieldTagID).ToList();

			return this.Store(m_lstTemplateTagsVM);

		}
		#endregion

		#region Private Method

		private List<string> IsSaveValid(string Action,string FunctionID, string NotificationTemplateID, string MinutesTemplateID, string Subject)
		{
			//TODO
			List<string> m_lstReturn = new List<string>();

			if (FunctionID == string.Empty)
				m_lstReturn.Add(MinutesEntryVM.Prop.FunctionID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
			if (NotificationTemplateID == string.Empty)
				m_lstReturn.Add(MinutesEntryVM.Prop.NotificationTemplateID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
			if (MinutesTemplateID == string.Empty)
				m_lstReturn.Add(MinutesEntryVM.Prop.MinuteTemplateID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
			if (Subject == string.Empty)
				m_lstReturn.Add(MinutesEntryVM.Prop.Subject.Desc + " " + General.EnumDesc(MessageLib.mustFill));

			return m_lstReturn;
		}

		private Dictionary<string, object> GetFormData(NameValueCollection parameters)
		{
			Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();
			m_dicReturn.Add(MinutesEntryVM.Prop.MinuteEntryID.Name, parameters[MinutesEntryVM.Prop.MinuteEntryID.Name]);
			m_dicReturn.Add(MinutesEntryVM.Prop.ScheduleID.Name, parameters[MinutesEntryVM.Prop.ScheduleID.Name]);
			m_dicReturn.Add(MinutesEntryVM.Prop.TaskID.Name, parameters[MinutesEntryVM.Prop.TaskID.Name]);
			m_dicReturn.Add(MinutesEntryVM.Prop.FPTID.Name, parameters[MinutesEntryVM.Prop.FPTID.Name]);
			m_dicReturn.Add(MinutesEntryVM.Prop.StatusID.Name, parameters[MinutesEntryVM.Prop.StatusID.Name]);
			m_dicReturn.Add(MinutesEntryVM.Prop.FunctionID.Name, parameters[MinutesEntryVM.Prop.FunctionID.Name]);
			m_dicReturn.Add(MinutesEntryVM.Prop.NotifMapID.Name, parameters[MinutesEntryVM.Prop.NotifMapID.Name]);
			m_dicReturn.Add(MinutesEntryVM.Prop.NotificationTemplateID.Name, parameters[MinutesEntryVM.Prop.NotificationTemplateID.Name]);
			m_dicReturn.Add(MinutesEntryVM.Prop.MinuteTemplateID.Name, parameters[MinutesEntryVM.Prop.MinuteTemplateID.Name]);
			//m_dicReturn.Add(SchedulesVM.Prop.StartDate.Name, parameters[SchedulesVM.Prop.StartDate.Name]);
			//m_dicReturn.Add(SchedulesVM.Prop.EndDate.Name, parameters[SchedulesVM.Prop.EndDate.Name]);
			//m_dicReturn.Add(SchedulesVM.Prop.Subject.Name, parameters[SchedulesVM.Prop.Subject.Name]);
			//m_dicReturn.Add(SchedulesVM.Prop.IsAllDay.Name, parameters[SchedulesVM.Prop.IsAllDay.Name]);
			//m_dicReturn.Add(SchedulesVM.Prop.Notes.Name, parameters[SchedulesVM.Prop.Notes.Name]);
			//m_dicReturn.Add(SchedulesVM.Prop.Weblink.Name, parameters[SchedulesVM.Prop.Weblink.Name]);
			//m_dicReturn.Add(SchedulesVM.Prop.Priority.Name, parameters[SchedulesVM.Prop.Priority.Name]);
			//m_dicReturn.Add(SchedulesVM.Prop.Location.Name, parameters[SchedulesVM.Prop.Location.Name]);
			m_dicReturn.Add(SchedulesVM.Prop.MailNotificationID.Name, parameters[SchedulesVM.Prop.MailNotificationID.Name]);


			return m_dicReturn;
		}

		private MinutesEntryVM GetSelectedData(Dictionary<string, object> selected, ref string message, string MinuteEntryID = "")
		{
			MinutesEntryVM m_objMinutesEntryVM = new MinutesEntryVM();
			TMinuteEntriesDA m_objTMinuteEntriesDA = new TMinuteEntriesDA();
			m_objTMinuteEntriesDA.ConnectionStringName = Global.ConnStrConfigName;

			List<string> m_lstSelect = new List<string>();
			m_lstSelect.Add(MinutesEntryVM.Prop.MinuteEntryID.MapAlias);
			m_lstSelect.Add(MinutesEntryVM.Prop.TaskID.MapAlias);
			m_lstSelect.Add(MinutesEntryVM.Prop.FPTID.MapAlias);
			m_lstSelect.Add(MinutesEntryVM.Prop.FPTDesc.MapAlias);
			m_lstSelect.Add(MinutesEntryVM.Prop.StatusID.MapAlias);
			m_lstSelect.Add(MinutesEntryVM.Prop.FunctionID.MapAlias);
			m_lstSelect.Add(MinutesEntryVM.Prop.FunctionDesc.MapAlias);
			m_lstSelect.Add(MinutesEntryVM.Prop.Subject.MapAlias);
			m_lstSelect.Add(MinutesEntryVM.Prop.NotificationTemplateID.MapAlias);
			m_lstSelect.Add(MinutesEntryVM.Prop.NotificationTemplateDesc.MapAlias);
			m_lstSelect.Add(MinutesEntryVM.Prop.MinuteTemplateID.MapAlias);
			m_lstSelect.Add(MinutesEntryVM.Prop.MinuteTemplateDescriptions.MapAlias);
			m_lstSelect.Add(MinutesEntryVM.Prop.MailNotificationID.MapAlias);
			m_lstSelect.Add(MinutesEntryVM.Prop.ScheduleID.MapAlias);
			m_lstSelect.Add(MinutesEntryVM.Prop.ScheduleDesc.MapAlias);
			m_lstSelect.Add(MinutesEntryVM.Prop.TaskStatusID.MapAlias);

			List<string> m_lstKey = new List<string>();
			Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

			foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
			{
				if (m_objMinutesEntryVM.IsKey(m_kvpSelectedRow.Key))
				{

					m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
					List<object> m_lstFilter = new List<object>();
					m_lstFilter.Add(Operator.Equals);
					m_lstFilter.Add(m_kvpSelectedRow.Value);
					m_objFilter.Add(MinutesEntryVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
				}
			}


			Dictionary<int, DataSet> m_dicTMinuteEntriesDA = m_objTMinuteEntriesDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
			if (m_objTMinuteEntriesDA.Message == string.Empty)
			{
				DataRow m_drMMinutesEntryDA = m_dicTMinuteEntriesDA[0].Tables[0].Rows[0];
				m_objMinutesEntryVM.MinuteEntryID = m_drMMinutesEntryDA[MinutesEntryVM.Prop.MinuteEntryID.Name].ToString();
				m_objMinutesEntryVM.TaskID = m_drMMinutesEntryDA[MinutesEntryVM.Prop.TaskID.Name].ToString();
				m_objMinutesEntryVM.FPTID = m_drMMinutesEntryDA[MinutesEntryVM.Prop.FPTID.Name].ToString();
				m_objMinutesEntryVM.FPTDesc = m_drMMinutesEntryDA[MinutesEntryVM.Prop.FPTDesc.Name].ToString();
				m_objMinutesEntryVM.StatusID = (int)m_drMMinutesEntryDA[MinutesEntryVM.Prop.StatusID.Name];
				m_objMinutesEntryVM.FunctionID = m_drMMinutesEntryDA[MinutesEntryVM.Prop.FunctionID.Name].ToString();
				m_objMinutesEntryVM.FunctionDesc = m_drMMinutesEntryDA[MinutesEntryVM.Prop.FunctionDesc.Name].ToString();
				//m_objMinutesEntryVM.NotifMapID = m_drMMinutesEntryDA[MinutesEntryVM.Prop.NotifMapID.Name].ToString();
				m_objMinutesEntryVM.NotificationTemplateID = m_drMMinutesEntryDA[MinutesEntryVM.Prop.NotificationTemplateID.Name].ToString();
				m_objMinutesEntryVM.NotificationTemplateDesc = m_drMMinutesEntryDA[MinutesEntryVM.Prop.NotificationTemplateDesc.Name].ToString();
				m_objMinutesEntryVM.MinuteTemplateID = m_drMMinutesEntryDA[MinutesEntryVM.Prop.MinuteTemplateID.Name].ToString();
				m_objMinutesEntryVM.MinuteTemplateDescriptions = m_drMMinutesEntryDA[MinutesEntryVM.Prop.MinuteTemplateDescriptions.Name].ToString();
				m_objMinutesEntryVM.TaskStatusID = m_drMMinutesEntryDA[MinutesEntryVM.Prop.TaskStatusID.Name].ToString();
				m_objMinutesEntryVM.Subject = m_drMMinutesEntryDA[MinutesEntryVM.Prop.Subject.Name].ToString();
				m_objMinutesEntryVM.ScheduleID = m_drMMinutesEntryDA[MinutesEntryVM.Prop.ScheduleID.Name].ToString();
				m_objMinutesEntryVM.ScheduleDesc = m_drMMinutesEntryDA[MinutesEntryVM.Prop.ScheduleDesc.Name].ToString();
				//m_objMinutesEntryVM.Notes = m_drMMinutesEntryDA[MinutesEntryVM.Prop.Notes.Name].ToString();
				//m_objMinutesEntryVM.Weblink = m_drMMinutesEntryDA[MinutesEntryVM.Prop.Weblink.Name].ToString();
				//m_objMinutesEntryVM.Location = m_drMMinutesEntryDA[MinutesEntryVM.Prop.Location.Name].ToString();
				//m_objMinutesEntryVM.Priority = (int)m_drMMinutesEntryDA[MinutesEntryVM.Prop.Priority.Name];
				m_objMinutesEntryVM.MailNotificationID = m_drMMinutesEntryDA[MinutesEntryVM.Prop.MailNotificationID.Name].ToString();
				//
				m_objMinutesEntryVM.ListRecipients = GetListRecipientsVM(m_objMinutesEntryVM.MailNotificationID, ref message);
				m_objMinutesEntryVM.ListNotificationValues = GetListNotifTagValues(m_objMinutesEntryVM.MailNotificationID, m_objMinutesEntryVM.NotificationTemplateID);
				m_objMinutesEntryVM.ListMinutesValues = GetListEntryTagValues(m_objMinutesEntryVM.MinuteEntryID, m_objMinutesEntryVM.MinuteTemplateID);
			}
			else
				message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objTMinuteEntriesDA.Message;


			return m_objMinutesEntryVM;
		}

		private SchedulesVM GetScheduleData(string ScheduleID,ref string message)
		{
			SchedulesVM m_objScheduleVM = new SchedulesVM();
			m_objScheduleVM.LstRecipients = new List<RecipientsVM>();
			MSchedulesDA m_objMSchedulesDA = new MSchedulesDA();
			m_objMSchedulesDA.ConnectionStringName = Global.ConnStrConfigName;

			List<string> m_lstSelect = new List<string>();
			m_lstSelect.Add(SchedulesVM.Prop.ScheduleID.MapAlias);
			m_lstSelect.Add(SchedulesVM.Prop.TaskID.MapAlias);
			m_lstSelect.Add(SchedulesVM.Prop.FPTID.MapAlias);
			m_lstSelect.Add(SchedulesVM.Prop.FPTDescription.MapAlias);
			m_lstSelect.Add(SchedulesVM.Prop.StatusID.MapAlias);
			m_lstSelect.Add(SchedulesVM.Prop.FunctionID.MapAlias);
			m_lstSelect.Add(SchedulesVM.Prop.FunctionDescription.MapAlias);
			m_lstSelect.Add(SchedulesVM.Prop.NotificationTemplateID.MapAlias);
			m_lstSelect.Add(SchedulesVM.Prop.NotificationTemplateDesc.MapAlias);
			m_lstSelect.Add(SchedulesVM.Prop.StartDate.MapAlias);
			m_lstSelect.Add(SchedulesVM.Prop.EndDate.MapAlias);
			m_lstSelect.Add(SchedulesVM.Prop.Subject.MapAlias);
			m_lstSelect.Add(SchedulesVM.Prop.IsAllDay.MapAlias);
			m_lstSelect.Add(SchedulesVM.Prop.Notes.MapAlias);
			m_lstSelect.Add(SchedulesVM.Prop.Weblink.MapAlias);
			m_lstSelect.Add(SchedulesVM.Prop.Location.MapAlias);
			m_lstSelect.Add(SchedulesVM.Prop.Priority.MapAlias);
			m_lstSelect.Add(SchedulesVM.Prop.CreatedDate.MapAlias);
			m_lstSelect.Add(SchedulesVM.Prop.IsBatchMail.MapAlias);
			m_lstSelect.Add(SchedulesVM.Prop.MailNotificationID.MapAlias);

			List<string> m_lstKey = new List<string>();
			Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

			//if (!string.IsNullOrEmpty(ScheduleID))
			//{
				List<object> m_lstFilter = new List<object>();
				m_lstFilter.Add(Operator.Equals);
				m_lstFilter.Add(ScheduleID);
				m_objFilter.Add(SchedulesVM.Prop.ScheduleID.Map, m_lstFilter);
			//}
			


			Dictionary<int, DataSet> m_dicMDivisionDA = m_objMSchedulesDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
			if (m_objMSchedulesDA.Message == string.Empty)
			{
				DataRow m_drMSchedulesDA = m_dicMDivisionDA[0].Tables[0].Rows[0];
				m_objScheduleVM.ScheduleID = m_drMSchedulesDA[SchedulesVM.Prop.ScheduleID.Name].ToString();
				m_objScheduleVM.TaskID = m_drMSchedulesDA[SchedulesVM.Prop.TaskID.Name].ToString();
				m_objScheduleVM.FPTID = m_drMSchedulesDA[SchedulesVM.Prop.FPTID.Name].ToString();
				m_objScheduleVM.FPTDescription = m_drMSchedulesDA[SchedulesVM.Prop.FPTDescription.Name].ToString();
				m_objScheduleVM.StatusID = m_drMSchedulesDA[SchedulesVM.Prop.StatusID.Name].ToString();
				m_objScheduleVM.FunctionID = m_drMSchedulesDA[SchedulesVM.Prop.FunctionID.Name].ToString();
				m_objScheduleVM.FunctionDescription = m_drMSchedulesDA[SchedulesVM.Prop.FunctionDescription.Name].ToString();
				m_objScheduleVM.NotificationTemplateID = m_drMSchedulesDA[SchedulesVM.Prop.NotificationTemplateID.Name].ToString();
				m_objScheduleVM.NotificationTemplateDesc = m_drMSchedulesDA[SchedulesVM.Prop.NotificationTemplateDesc.Name].ToString();
				m_objScheduleVM.StartDate = (DateTime)m_drMSchedulesDA[SchedulesVM.Prop.StartDate.Name];
				m_objScheduleVM.EndDate = (DateTime)m_drMSchedulesDA[SchedulesVM.Prop.EndDate.Name];
				m_objScheduleVM.Subject = m_drMSchedulesDA[SchedulesVM.Prop.Subject.Name].ToString();
				m_objScheduleVM.IsAllDay = (bool)m_drMSchedulesDA[SchedulesVM.Prop.IsAllDay.Name];
				m_objScheduleVM.IsBatchMail = string.IsNullOrEmpty(m_drMSchedulesDA[SchedulesVM.Prop.IsBatchMail.Name].ToString()) ? false : (bool)m_drMSchedulesDA[SchedulesVM.Prop.IsBatchMail.Name];
				m_objScheduleVM.Notes = m_drMSchedulesDA[SchedulesVM.Prop.Notes.Name].ToString();
				m_objScheduleVM.Weblink = m_drMSchedulesDA[SchedulesVM.Prop.Weblink.Name].ToString();
				m_objScheduleVM.Location = m_drMSchedulesDA[SchedulesVM.Prop.Location.Name].ToString();
				m_objScheduleVM.Priority = (int)m_drMSchedulesDA[SchedulesVM.Prop.Priority.Name];
				m_objScheduleVM.CreatedDate = (DateTime)m_drMSchedulesDA[SchedulesVM.Prop.CreatedDate.Name];
				m_objScheduleVM.MailNotificationID = m_drMSchedulesDA[SchedulesVM.Prop.MailNotificationID.Name].ToString();
				m_objScheduleVM.StartTime = m_objScheduleVM.StartDateHour.ToString();
				m_objScheduleVM.EndTime = m_objScheduleVM.EndDateHour.ToString();
				//m_objScheduleVM.LstRecipients = GetListScheduleRecipiant(m_objScheduleVM.MailNotificationID, m_objScheduleVM.TaskID, m_objScheduleVM.IsBatchMail, ref message);
				//m_objScheduleVM.LstNotificationValues = GetListNotificationValues(m_objScheduleVM.MailNotificationID, m_objScheduleVM.NotificationTemplateID);
				//m_objScheduleVM.LstNotificationAttachment = GetListNotificationAttachment(m_objScheduleVM.MailNotificationID);
			}
			else
				message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMSchedulesDA.Message;


			return m_objScheduleVM;
		}
		#endregion

		#region Public Method

		public Dictionary<int, List<DivisionVM>> GetDivisionData(bool isCount, string DivisionID, string DivisionDesc)
		{
			int m_intCount = 0;
			List<DivisionVM> m_lstDivisionVM = new List<ViewModels.DivisionVM>();
			Dictionary<int, List<DivisionVM>> m_dicReturn = new Dictionary<int, List<DivisionVM>>();
			MDivisionDA m_objMDivisionDA = new MDivisionDA();
			m_objMDivisionDA.ConnectionStringName = Global.ConnStrConfigName;

			List<string> m_lstSelect = new List<string>();
			m_lstSelect.Add(DivisionVM.Prop.DivisionID.MapAlias);
			m_lstSelect.Add(DivisionVM.Prop.DivisionDesc.MapAlias);

			Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
			List<object> m_lstFilter = new List<object>();
			m_lstFilter.Add(Operator.Contains);
			m_lstFilter.Add(DivisionID);
			m_objFilter.Add(DivisionVM.Prop.DivisionID.Map, m_lstFilter);

			m_lstFilter = new List<object>();
			m_lstFilter.Add(Operator.Contains);
			m_lstFilter.Add(DivisionDesc);
			m_objFilter.Add(DivisionVM.Prop.DivisionDesc.Map, m_lstFilter);

			Dictionary<int, DataSet> m_dicMDivisionDA = m_objMDivisionDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
			if (m_objMDivisionDA.Message == string.Empty)
			{
				if (isCount)
					foreach (KeyValuePair<int, DataSet> m_kvpDivisionBL in m_dicMDivisionDA)
					{
						m_intCount = m_kvpDivisionBL.Key;
						break;
					}
				else
				{
					m_lstDivisionVM = (
						from DataRow m_drMDivisionDA in m_dicMDivisionDA[0].Tables[0].Rows
						select new DivisionVM()
						{
							DivisionID = m_drMDivisionDA[DivisionVM.Prop.DivisionID.Name].ToString(),
							DivisionDesc = m_drMDivisionDA[DivisionVM.Prop.DivisionDesc.Name].ToString()
						}
					).ToList();
				}
			}
			m_dicReturn.Add(m_intCount, m_lstDivisionVM);
			return m_dicReturn;
		}

		public List<EntryValuesVM> GetListEntryTagValues(string MinuteEntryID, string TemplateID)
		{
			List<EntryValuesVM> m_lstEntryValues = new List<EntryValuesVM>();
			TEntryValuesDA m_objTEntryValuesDA = new TEntryValuesDA();
			m_objTEntryValuesDA.ConnectionStringName = Global.ConnStrConfigName;

			List<string> m_lstSelect = new List<string>();
			m_lstSelect.Add(EntryValuesVM.Prop.EntryValueID.MapAlias);
			m_lstSelect.Add(EntryValuesVM.Prop.MinuteEntryID.MapAlias);
			m_lstSelect.Add(EntryValuesVM.Prop.FieldTagID.MapAlias);
			m_lstSelect.Add(EntryValuesVM.Prop.TagDesc.MapAlias);
			m_lstSelect.Add(EntryValuesVM.Prop.Value.MapAlias);

			Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
			List<object> m_lstFilter = new List<object>();
			m_lstFilter.Add(Operator.Equals);
			m_lstFilter.Add(MinuteEntryID);
			m_objFilter.Add(EntryValuesVM.Prop.MinuteEntryID.Map, m_lstFilter);

			//m_lstFilter = new List<object>();
			//m_lstFilter.Add(Operator.None);
			//m_lstFilter.Add(string.Empty);
			//m_objFilter.Add(string.Format("({0} IS NULL)", TemplateTagsVM.Prop.RefIDColumn.Map), m_lstFilter);

			//m_lstFilter = new List<object>();
			//m_lstFilter.Add(Operator.None);
			//m_lstFilter.Add(string.Empty);
			//m_objFilter.Add(string.Format("({0} IS NULL)", TemplateTagsVM.Prop.RefTable.Map), m_lstFilter);



			Dictionary<int, DataSet> m_dicTEntryValuesDA = m_objTEntryValuesDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
			if (m_objTEntryValuesDA.Message == string.Empty)
			{
				foreach (DataRow dr in m_dicTEntryValuesDA[0].Tables[0].Rows)
				{
					EntryValuesVM objEntryValuesVM = new EntryValuesVM();
					objEntryValuesVM.EntryValueID = dr[EntryValuesVM.Prop.EntryValueID.Name].ToString();
					objEntryValuesVM.MinuteEntryID = dr[EntryValuesVM.Prop.MinuteEntryID.Name].ToString();
					objEntryValuesVM.FieldTagID = dr[EntryValuesVM.Prop.FieldTagID.Name].ToString();
					objEntryValuesVM.TagDesc = dr[EntryValuesVM.Prop.TagDesc.Name].ToString();
					objEntryValuesVM.Value = dr[EntryValuesVM.Prop.Value.Name].ToString();

					m_lstEntryValues.Add(objEntryValuesVM);
				}
			}

			

			return m_lstEntryValues.OrderBy(d => d.FieldTagID).ToList();
		}

		public List<NotificationValuesVM> GetListNotifTagValues(string MailNotifID, string TemplateID)
		{

			List<NotificationValuesVM> m_lsNotificationValuesVM = new List<NotificationValuesVM>();
			TNotificationValuesDA m_objTNotificationValuesDA = new TNotificationValuesDA();
			m_objTNotificationValuesDA.ConnectionStringName = Global.ConnStrConfigName;

			List<string> m_lstSelect = new List<string>();
			m_lstSelect.Add(NotificationValuesVM.Prop.NotificationValueID.MapAlias);
			m_lstSelect.Add(NotificationValuesVM.Prop.MailNotificationID.MapAlias);
			m_lstSelect.Add(NotificationValuesVM.Prop.FieldTagID.MapAlias);
			m_lstSelect.Add(NotificationValuesVM.Prop.TagDesc.MapAlias);
			m_lstSelect.Add(NotificationValuesVM.Prop.Value.MapAlias);

			Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
			List<object> m_lstFilter = new List<object>();
			m_lstFilter.Add(Operator.Equals);
			m_lstFilter.Add(MailNotifID);
			m_objFilter.Add(NotificationValuesVM.Prop.MailNotificationID.Map, m_lstFilter);

			
			//m_lstFilter = new List<object>();
			//m_lstFilter.Add(Operator.None);
			//m_lstFilter.Add(string.Empty);
			//m_objFilter.Add(string.Format("({0} IS NULL)", TemplateTagsVM.Prop.RefIDColumn.Map), m_lstFilter);

			//m_lstFilter = new List<object>();
			//m_lstFilter.Add(Operator.None);
			//m_lstFilter.Add(string.Empty);
			//m_objFilter.Add(string.Format("({0} IS NULL)", TemplateTagsVM.Prop.RefTable.Map), m_lstFilter);



			Dictionary<int, DataSet> m_dicTNotificationValuesDA = m_objTNotificationValuesDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
			if (m_objTNotificationValuesDA.Message == string.Empty)
			{
				foreach (DataRow dr in m_dicTNotificationValuesDA[0].Tables[0].Rows)
				{
					NotificationValuesVM objNotificationVal = new NotificationValuesVM();
					objNotificationVal.NotificationValueID = dr[NotificationValuesVM.Prop.NotificationValueID.Name].ToString();
					objNotificationVal.MailNotificationID = dr[NotificationValuesVM.Prop.MailNotificationID.Name].ToString();
					objNotificationVal.FieldTagID = dr[NotificationValuesVM.Prop.FieldTagID.Name].ToString();
					objNotificationVal.TagDesc = dr[NotificationValuesVM.Prop.TagDesc.Name].ToString();
					objNotificationVal.Value = dr[NotificationValuesVM.Prop.Value.Name].ToString();

					m_lsNotificationValuesVM.Add(objNotificationVal);
				}
			}
			return m_lsNotificationValuesVM.OrderBy(d=>d.FieldTagID).ToList();
		}

		#endregion
	}
}