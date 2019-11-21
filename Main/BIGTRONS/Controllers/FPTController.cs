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

namespace com.SML.BIGTRONS.Controllers
{
    public class FPTController : BaseController
    {
        private readonly string title = "Form Pengajuan Tender";
        private readonly string dataSessionName = "FormData";

        private List<int> manualStatusList = new List<int>();
        public FPTController()
        {
            manualStatusList.Add((int)FPTStatusTypes.PreTender);
            manualStatusList.Add((int)FPTStatusTypes.ManageVendor);
            manualStatusList.Add((int)FPTStatusTypes.Aanwijzing);
            manualStatusList.Add((int)FPTStatusTypes.VendorEntry);
            manualStatusList.Add((int)FPTStatusTypes.Clarification);
            manualStatusList.Add((int)FPTStatusTypes.ReNegotiation);
            manualStatusList.Add((int)FPTStatusTypes.StopNegotiation);
            manualStatusList.Add((int)FPTStatusTypes.ReTender);
            manualStatusList.Add((int)FPTStatusTypes.VendorUnverified);
            manualStatusList.Add((int)FPTStatusTypes.VendorVerified);
            manualStatusList.Add((int)FPTStatusTypes.DoneNegotiation);
            manualStatusList.Add((int)FPTStatusTypes.FPTPark);
            manualStatusList.Add((int)FPTStatusTypes.FPTUnpark);
        }

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
                m_lstSelect.Add(FPTVM.Prop.AdditionalInfo1.MapAlias);
                m_lstSelect.Add(FPTVM.Prop.AdditionalInfo1Desc.MapAlias);
                m_lstSelect.Add(FPTVM.Prop.IsSync.MapAlias);

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
                            IsSync = (bool)m_drMFPTDA[FPTVM.Prop.IsSync.Name],
                            AdditionalInfo1 = (m_drMFPTDA[FPTVM.Prop.AdditionalInfo1.Name].ToString() == string.Empty ? 0 : int.Parse(m_drMFPTDA[FPTVM.Prop.AdditionalInfo1.Name].ToString())),
                            AdditionalInfo1Desc = m_drMFPTDA[FPTVM.Prop.AdditionalInfo1Desc.Name].ToString(),
                            ListNegotiationConfigurationsVM = GetListNegotiationConfigurationsVM(m_drMFPTDA[FPTVM.Prop.FPTID.Name].ToString(), ref messages)
                        }
                    ).ToList();
                }
            }

            return this.Store(m_lstFPTVM, m_intCount);
        }
        public ActionResult Home()
        {
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 0;
            return this.Direct();
        }
        public ActionResult PageOne()
        {
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return this.Direct();
        }
        public ActionResult Add(string Caller)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Add)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            FPTVM m_objFPTVM = new FPTVM();
            m_objFPTVM.ListFPTStatusVM = new List<FPTStatusVM>();
            ViewDataDictionary m_vddFPT = new ViewDataDictionary();
            m_vddFPT.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddFPT.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
            if (Caller == General.EnumDesc(Buttons.ButtonDetail))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                //Dictionary<string, object> m_dicSelectedRow = GetFormData(m_nvcParams);
                //Session[dataSessionName] = JSON.Serialize(m_dicSelectedRow);
            }


            //additional info 1
            int m_AdditionalInfo1 = 0;
            string m_AdditionalInfo1Desc = string.Empty;


            m_vddFPT.Add(FPTVM.Prop.AdditionalInfo1.Name, m_AdditionalInfo1.ToString());
            m_vddFPT.Add(FPTVM.Prop.AdditionalInfo1Desc.Name, m_AdditionalInfo1.ToString());

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
        public ActionResult AddStatus(string Caller, string FPTID)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Add)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            FPTStatusVM m_objFPTStatusVM = new FPTStatusVM();
            m_objFPTStatusVM.FPTID = FPTID;
            ViewDataDictionary m_vddFPTStatusVM = new ViewDataDictionary();
            m_vddFPTStatusVM.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddFPTStatusVM.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
            if (Caller == General.EnumDesc(Buttons.ButtonDetail))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                //Dictionary<string, object> m_dicSelectedRow = GetFormData(m_nvcParams);
                //Session[dataSessionName] = JSON.Serialize(m_dicSelectedRow);
            }

            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 2;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageTwo),
                Model = m_objFPTStatusVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddFPTStatusVM,
                ViewName = "Status/Entry/_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult Detail(string Caller, string Selected, string FPTID)
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
                m_dicSelectedRow = GetFormData(m_nvcParams, FPTID);
            }
            else
                m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);

            if (m_dicSelectedRow.Count > 0)
                m_objFPTVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }


            ViewDataDictionary m_vddFPT = new ViewDataDictionary();
            m_vddFPT.Add("StatusData", m_objFPTVM.LastStatus);
            m_vddFPT.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
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
        public ActionResult StatusTracking(string Caller, string Selected, string FPTID)
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
                m_dicSelectedRow = GetFormData(m_nvcParams, FPTID);
            }
            else
                m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);

            if (m_dicSelectedRow.Count > 0)
                m_objFPTVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }


            ViewDataDictionary m_vddFPT = new ViewDataDictionary();

            m_vddFPT.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
            m_vddFPT.Add(FPTStatusVM.Prop.FPTID.Name, m_objFPTVM.FPTID);
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;

            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objFPTVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddFPT,
                ViewName = "Status/_Form",
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
            FPTVM m_objFPTVM = new FPTVM();
            if (m_dicSelectedRow.Count > 0)
                m_objFPTVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddFPT = new ViewDataDictionary();
            m_vddFPT.Add(General.EnumDesc(Params.Caller), Caller);
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
        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<FPTVM> m_lstSelectedRow = new List<FPTVM>();
            m_lstSelectedRow = JSON.Deserialize<List<FPTVM>>(Selected);

            DFPTStatusDA m_objDFPTStatusDA = new DFPTStatusDA();
            m_objDFPTStatusDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (FPTVM m_objFPTVM in m_lstSelectedRow)
                {
                    DFPTStatus m_objDFPTStatus = new DFPTStatus();


                    m_objDFPTStatus.FPTStatusID = string.Empty;
                    m_objDFPTStatus.FPTID = m_objFPTVM.FPTID;
                    m_objDFPTStatus.StatusDateTimeStamp = DateTime.Now;
                    m_objDFPTStatus.StatusID = (int)FPTStatusTypes.Deleted;
                    m_objDFPTStatusDA.Data = m_objDFPTStatus;
                    m_objDFPTStatusDA.Select();

                    m_objDFPTStatusDA.Insert(false);
                    if (!m_objDFPTStatusDA.Success || m_objDFPTStatusDA.Message != string.Empty)
                    {
                        m_lstMessage.Add(m_objDFPTStatusDA.Message);
                    }


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
        public ActionResult Save(string Action)
        {

            Dictionary<string, List<object>> m_objFilter;
            List<object> m_lstFilter ;

            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            bool m_bissync = false;
            List<string> m_lstMessage = new List<string>();
            MFPTDA m_objMFPTDA = new MFPTDA();
            FPTVM m_objFPTVM = new FPTVM();
            DFPTStatusDA m_objDFPTStatusDA = new DFPTStatusDA();
            DFPTDeviationsDA m_objDFPTDeviationsDA = new DFPTDeviationsDA();
            DFPTAdditionalInfoDA m_objDFPTAdditionalInfoDA = new DFPTAdditionalInfoDA();

            m_objMFPTDA.ConnectionStringName = Global.ConnStrConfigName;
            string m_strTransName = "MFPT";
            object m_objDBConnection = null;
            m_objDBConnection = m_objMFPTDA.BeginTrans(m_strTransName);
            string m_strFPTID = string.Empty;
            try
            {
                m_strFPTID = Request.Params[FPTVM.Prop.FPTID.Name];
                m_bissync = !(Action == General.EnumDesc(Buttons.ButtonAdd) && string.IsNullOrEmpty(m_strFPTID));//todo:
                bool m_boolDocumentComplete = Request.Params[FPTVM.Prop.DocumentComplete.Name].ToString().ToLower() == "true";
                bool m_boolIsSync = Request.Params[FPTVM.Prop.IsSync.Name].ToString().ToLower() == "true";
                FPTDeviationsVM m_FPTDeviationsVM = new FPTDeviationsVM();
                if (!m_boolDocumentComplete)
                {
                    m_FPTDeviationsVM.RefDate = (string.IsNullOrEmpty(Request.Params["FPTDeviationsVM.RefDate"].ToString())) ? DateTime.MinValue : Convert.ToDateTime(Request.Params[nameof(FPTDeviationsVM) + "." + nameof(FPTDeviationsVM.RefDate)]);
                    m_FPTDeviationsVM.RefNumber = Request.Params[nameof(FPTDeviationsVM) + "." + nameof(FPTDeviationsVM.RefNumber)];
                    m_FPTDeviationsVM.RefTitle = Request.Params[nameof(FPTDeviationsVM) + "." + nameof(FPTDeviationsVM.RefTitle)];
                }

                m_objFPTVM.FPTID = m_strFPTID;
                m_objFPTVM.Descriptions = Request.Params[FPTVM.Prop.Descriptions.Name];
                m_objFPTVM.ClusterID = string.IsNullOrEmpty(Request.Params[FPTVM.Prop.ClusterID.Name].ToString()) ? null : Request.Params[FPTVM.Prop.ClusterID.Name];
                m_objFPTVM.ProjectID = string.IsNullOrEmpty(Request.Params[FPTVM.Prop.ProjectID.Name].ToString()) ? null : Request.Params[FPTVM.Prop.ProjectID.Name];
                m_objFPTVM.DivisionID = string.IsNullOrEmpty(Request.Params[FPTVM.Prop.DivisionID.Name].ToString()) ? null : Request.Params[FPTVM.Prop.DivisionID.Name];
                m_objFPTVM.BusinessUnitID = string.IsNullOrEmpty(Request.Params[FPTVM.Prop.BusinessUnitID.Name].ToString()) ? null : Request.Params[FPTVM.Prop.BusinessUnitID.Name];
                m_objFPTVM.DocumentComplete = m_boolDocumentComplete;
                m_objFPTVM.FPTDeviationsVM = m_FPTDeviationsVM;
                m_objFPTVM.FPTScheduleStart = (string.IsNullOrEmpty(Request.Params[nameof(FPTVM.FPTScheduleStart)].ToString())) ? DateTime.MinValue : Convert.ToDateTime(Request.Params[nameof(FPTVM.FPTScheduleStart)]);
                m_objFPTVM.FPTScheduleEnd = (string.IsNullOrEmpty(Request.Params[nameof(FPTVM.FPTScheduleEnd)].ToString())) ? DateTime.MinValue : Convert.ToDateTime(Request.Params[nameof(FPTVM.FPTScheduleEnd)]);
                DateDifference dateDifference = new DateDifference(m_objFPTVM.FPTScheduleEnd, m_objFPTVM.FPTScheduleStart);
                m_objFPTVM.Duration = dateDifference.ToString(); //string.IsNullOrEmpty(Request.Params[nameof(FPTVM.Duration)].ToString()) ? null : Request.Params[nameof(FPTVM.Duration)];
                m_objFPTVM.MaintenancePeriod = string.IsNullOrEmpty(Request.Params[nameof(FPTVM.MaintenancePeriod)].ToString()) ? null : Request.Params[nameof(FPTVM.MaintenancePeriod)];
                m_objFPTVM.Guarantee = string.IsNullOrEmpty(Request.Params[nameof(FPTVM.Guarantee)].ToString()) ? null : Request.Params[nameof(FPTVM.Guarantee)];
                m_objFPTVM.ContractType = string.IsNullOrEmpty(Request.Params[nameof(FPTVM.ContractType)].ToString()) ? null : Request.Params[nameof(FPTVM.ContractType)];
                m_objFPTVM.PaymentMethod = string.IsNullOrEmpty(Request.Params[nameof(FPTVM.PaymentMethod)].ToString()) ? null : Request.Params[nameof(FPTVM.PaymentMethod)];

                m_objFPTVM.FPTScheduleStartManual = string.IsNullOrEmpty(Request.Params[nameof(FPTVM.FPTScheduleStartManual)].ToString()) ? null : Request.Params[nameof(FPTVM.FPTScheduleStartManual)];
                m_objFPTVM.FPTScheduleEndManual = string.IsNullOrEmpty(Request.Params[nameof(FPTVM.FPTScheduleEndManual)].ToString()) ? null : Request.Params[nameof(FPTVM.FPTScheduleEndManual)];
                m_objFPTVM.DurationManual = string.IsNullOrEmpty(Request.Params[nameof(FPTVM.DurationManual)].ToString()) ? null : Request.Params[nameof(FPTVM.DurationManual)];

                m_objFPTVM.IsSync = m_boolIsSync;


                m_lstMessage = IsSaveValid(Action, m_objFPTVM);
                if (m_lstMessage.Count <= 0)
                {
                    #region MFPT
                    MFPT m_objMFPT = new MFPT();
                    m_objMFPT.FPTID = m_strFPTID;
                    m_objMFPTDA.Data = m_objMFPT;
                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objMFPTDA.Select();

                    m_objMFPT.FPTID = m_strFPTID;
                    m_objMFPT.Descriptions = m_objFPTVM.Descriptions;
                    m_objMFPT.ClusterID = m_objFPTVM.ClusterID;
                    m_objMFPT.ProjectID = m_objFPTVM.ProjectID;
                    m_objMFPT.DivisionID = m_objFPTVM.DivisionID;
                    m_objMFPT.IsSync = m_objFPTVM.IsSync;
                    m_objMFPT.BusinessUnitID = m_objFPTVM.BusinessUnitID;

                    m_objDBConnection = m_objMFPTDA.BeginTrans(m_strTransName);
                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objMFPTDA.Insert(true, m_objDBConnection);
                    else
                        m_objMFPTDA.Update(true, m_objDBConnection);


                    if (!m_objMFPTDA.Success || m_objMFPTDA.Message != string.Empty)
                    {
                        m_objMFPTDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                        return this.Direct(false, m_objMFPTDA.Message);
                    }
                    m_strFPTID = m_objMFPTDA.Data.FPTID;

                    #endregion

                    #region DFPTStatus
                    m_objDFPTStatusDA.ConnectionStringName = Global.ConnStrConfigName;
                    DFPTStatus m_objDFPTStatus = new DFPTStatus();
                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                    {
                        string m_insmessage = string.Empty;
                        //New Status
                        if (!InsertDFPTStatus(m_strFPTID, (int)FPTStatusTypes.New, DateTime.Now, ref m_insmessage, null, m_objDBConnection))
                        {
                            m_objDFPTStatusDA.EndTrans(ref m_objDBConnection, m_strTransName);
                            return this.Direct(false, m_objDFPTStatusDA.Message);
                        }

                        //Deviation Status
                        int m_intDeviationStatusID = (m_boolDocumentComplete) ? (int)FPTStatusTypes.FPTVerified : (int)FPTStatusTypes.FPTUnverified;

                        m_objDFPTStatus.FPTStatusID = string.Empty;
                        m_objDFPTStatus.FPTID = m_strFPTID;
                        m_objDFPTStatus.StatusDateTimeStamp = DateTime.Now.AddMilliseconds(100);
                        m_objDFPTStatus.StatusID = m_intDeviationStatusID;
                        m_objDFPTStatusDA.Data = m_objDFPTStatus;
                        //m_objDFPTStatusDA.Select();
                        m_objDFPTStatusDA.Insert(true, m_objDBConnection);
                        if (!m_objDFPTStatusDA.Success || m_objDFPTStatusDA.Message != string.Empty)
                        {
                            m_objDFPTStatusDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                            return this.Direct(false, m_objDFPTStatusDA.Message);
                        }
                    }
                    else
                    {
                        if (isDeviationChange(m_strFPTID))
                        {
                            //Deviation Status
                            int m_intDeviationStatusID = (m_boolDocumentComplete) ? (int)FPTStatusTypes.FPTVerified : (int)FPTStatusTypes.FPTUnverified;

                            m_objDFPTStatus.FPTStatusID = string.Empty;
                            m_objDFPTStatus.FPTID = m_strFPTID;
                            m_objDFPTStatus.StatusDateTimeStamp = DateTime.Now;
                            m_objDFPTStatus.StatusID = m_intDeviationStatusID;
                            m_objDFPTStatusDA.Data = m_objDFPTStatus;
                                                        
                            m_objDFPTStatusDA.Insert(true, m_objDBConnection);
                            if (!m_objDFPTStatusDA.Success || m_objDFPTStatusDA.Message != string.Empty)
                            {
                                m_objDFPTStatusDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                                return this.Direct(false, m_objDFPTStatusDA.Message);
                            }
                        }

                    }

                    #endregion

                    #region DFPTDeviations
                    if (!m_boolDocumentComplete)
                    {
                        m_objDFPTDeviationsDA.ConnectionStringName = Global.ConnStrConfigName;
                        DFPTDeviations m_objDFPTDeviations = new DFPTDeviations();
                        m_objDFPTDeviations.FPTDeviationID = string.Empty;
                        m_objDFPTDeviationsDA.Data = m_objDFPTDeviations;
                        if (Action != General.EnumDesc(Buttons.ButtonAdd))
                            m_objDFPTDeviationsDA.Select();

                        m_objDFPTDeviations.FPTDeviationID = (Request.Params["FPTDeviationsVM.FPTDeviationID"] == null) ? string.Empty : Request.Params["FPTDeviationsVM.FPTDeviationID"];
                        m_objDFPTDeviations.FPTID = m_strFPTID;
                        m_objDFPTDeviations.DeviationTypeID = General.EnumDesc(DeviationTypes.FPTVerification);
                        m_objDFPTDeviations.RefNumber = m_FPTDeviationsVM.RefNumber;
                        m_objDFPTDeviations.RefTitle = m_FPTDeviationsVM.RefTitle;
                        m_objDFPTDeviations.RefDate = m_FPTDeviationsVM.RefDate;
                        //m_objDBConnection = m_objDFPTDeviationsDA.BeginTrans(m_strTransName);

                        if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        {
                            m_objDFPTDeviationsDA.Insert(true, m_objDBConnection);
                        }
                        else
                        {
                            if (isDeviationExist(m_strFPTID))
                                m_objDFPTDeviationsDA.Update(true, m_objDBConnection);
                            else
                                m_objDFPTDeviationsDA.Insert(true, m_objDBConnection);
                        }



                        if (!m_objDFPTDeviationsDA.Success || m_objDFPTDeviationsDA.Message != string.Empty)
                        {
                            m_objDFPTDeviationsDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                            return this.Direct(false, m_objDFPTDeviationsDA.Message);
                        }
                        //m_strFPTID = m_objDFPTDeviationsDA.Data.FPTID;
                    }
                    else
                    {

                        //DELETE deviation
                        m_objFilter = new Dictionary<string, List<object>>();
                        m_lstFilter = new List<object>();

                        m_lstFilter = new List<object>();
                        m_lstFilter.Add(Operator.Equals);
                        m_lstFilter.Add(m_strFPTID);
                        m_objFilter.Add(FPTDeviationsVM.Prop.FPTID.Map, m_lstFilter);

                        m_objDFPTDeviationsDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                        if (m_objDFPTDeviationsDA.Message == General.EnumDesc(MessageLib.NotExist)) m_objDFPTDeviationsDA.Message = "";
                    }
                    #endregion

                    #region DFPTAdditionalInfo
                    m_objDFPTAdditionalInfoDA.ConnectionStringName = Global.ConnStrConfigName;
                    DFPTAdditionalInfo m_objDFPTAdditionalInfo = new DFPTAdditionalInfo();
                    if (Action == General.EnumDesc(Buttons.ButtonUpdate))
                    {
                        //DELETE DFPTAdditionalInfo
                        m_objFilter = new Dictionary<string, List<object>>();
                        m_lstFilter = new List<object>();

                        m_lstFilter = new List<object>();
                        m_lstFilter.Add(Operator.Equals);
                        m_lstFilter.Add(m_strFPTID);
                        m_objFilter.Add(FPTAdditionalInfoVM.Prop.FPTID.Map, m_lstFilter);
                        m_lstFilter = new List<object>();
                        m_lstFilter.Add(Operator.In);
                        m_lstFilter.Add("1,2,3,4,5,6,7,11,12,13,14,15");
                        m_objFilter.Add(FPTAdditionalInfoVM.Prop.FPTAdditionalInfoItemID.Map, m_lstFilter);

                        m_objDFPTAdditionalInfoDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                        if (m_objDFPTAdditionalInfoDA.Message == General.EnumDesc(MessageLib.NotExist)) m_objDFPTAdditionalInfoDA.Message = "";
                    }
                    //FPTScheduleStart
                    if (m_objFPTVM.FPTScheduleStart != DateTime.MinValue)
                    {
                        m_objDFPTAdditionalInfo = new DFPTAdditionalInfo() { FPTAdditionalInfoID = Guid.NewGuid().ToString().Replace("-", ""), FPTID = m_strFPTID, FPTAdditionalInfoItemID = ((int)AdditionalInfoItems.ScheduleStart).ToString(), Value = m_objFPTVM.FPTScheduleStart.ToString(Global.SqlDateFormat) };
                        m_objDFPTAdditionalInfoDA.Data = m_objDFPTAdditionalInfo;
                        m_objDFPTAdditionalInfoDA.Insert(true, m_objDBConnection);
                        if (!m_objDFPTAdditionalInfoDA.Success || m_objDFPTAdditionalInfoDA.Message != string.Empty)
                        {
                            m_objDFPTAdditionalInfoDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                            return this.Direct(false, m_objDFPTAdditionalInfoDA.Message);
                        }
                    }
                    //FPTScheduleEnd
                    if (m_objFPTVM.FPTScheduleEnd != DateTime.MinValue)
                    {
                        m_objDFPTAdditionalInfo = new DFPTAdditionalInfo() { FPTAdditionalInfoID = Guid.NewGuid().ToString().Replace("-", ""), FPTID = m_strFPTID, FPTAdditionalInfoItemID = ((int)AdditionalInfoItems.ScheduleEnd).ToString(), Value = m_objFPTVM.FPTScheduleEnd.ToString(Global.SqlDateFormat) };
                        m_objDFPTAdditionalInfoDA.Data = m_objDFPTAdditionalInfo;
                        m_objDFPTAdditionalInfoDA.Insert(true, m_objDBConnection);
                        if (!m_objDFPTAdditionalInfoDA.Success || m_objDFPTAdditionalInfoDA.Message != string.Empty)
                        {
                            m_objDFPTAdditionalInfoDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                            return this.Direct(false, m_objDFPTAdditionalInfoDA.Message);
                        }
                    }
                    //Duration
                    if (!string.IsNullOrEmpty(m_objFPTVM.Duration))
                    {
                        m_objDFPTAdditionalInfo = new DFPTAdditionalInfo() { FPTAdditionalInfoID = Guid.NewGuid().ToString().Replace("-", ""), FPTID = m_strFPTID, FPTAdditionalInfoItemID = ((int)AdditionalInfoItems.Duration).ToString(), Value = m_objFPTVM.Duration };
                        m_objDFPTAdditionalInfoDA.Data = m_objDFPTAdditionalInfo;
                        m_objDFPTAdditionalInfoDA.Insert(true, m_objDBConnection);
                        if (!m_objDFPTAdditionalInfoDA.Success || m_objDFPTAdditionalInfoDA.Message != string.Empty)
                        {
                            m_objDFPTAdditionalInfoDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                            return this.Direct(false, m_objDFPTAdditionalInfoDA.Message);
                        }
                    }
                    //MaintenancePeriod
                    if (!string.IsNullOrEmpty(m_objFPTVM.MaintenancePeriod))
                    {
                        m_objDFPTAdditionalInfo = new DFPTAdditionalInfo() { FPTAdditionalInfoID = Guid.NewGuid().ToString().Replace("-", ""), FPTID = m_strFPTID, FPTAdditionalInfoItemID = ((int)AdditionalInfoItems.MaintenancePeriod).ToString(), Value = m_objFPTVM.MaintenancePeriod };
                        m_objDFPTAdditionalInfoDA.Data = m_objDFPTAdditionalInfo;
                        m_objDFPTAdditionalInfoDA.Insert(true, m_objDBConnection);
                        if (!m_objDFPTAdditionalInfoDA.Success || m_objDFPTAdditionalInfoDA.Message != string.Empty)
                        {
                            m_objDFPTAdditionalInfoDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                            return this.Direct(false, m_objDFPTAdditionalInfoDA.Message);
                        }
                    }
                    //Guarantee
                    if (!string.IsNullOrEmpty(m_objFPTVM.Guarantee))
                    {
                        m_objDFPTAdditionalInfo = new DFPTAdditionalInfo() { FPTAdditionalInfoID = Guid.NewGuid().ToString().Replace("-", ""), FPTID = m_strFPTID, FPTAdditionalInfoItemID = ((int)AdditionalInfoItems.Guarantee).ToString(), Value = m_objFPTVM.Guarantee };
                        m_objDFPTAdditionalInfoDA.Data = m_objDFPTAdditionalInfo;
                        m_objDFPTAdditionalInfoDA.Insert(true, m_objDBConnection);
                        if (!m_objDFPTAdditionalInfoDA.Success || m_objDFPTAdditionalInfoDA.Message != string.Empty)
                        {
                            m_objDFPTAdditionalInfoDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                            return this.Direct(false, m_objDFPTAdditionalInfoDA.Message);
                        }
                    }
                    //ContractType
                    if (!string.IsNullOrEmpty(m_objFPTVM.ContractType))
                    {
                        m_objDFPTAdditionalInfo = new DFPTAdditionalInfo() { FPTAdditionalInfoID = Guid.NewGuid().ToString().Replace("-", ""), FPTID = m_strFPTID, FPTAdditionalInfoItemID = ((int)AdditionalInfoItems.ContractType).ToString(), Value = m_objFPTVM.ContractType };
                        m_objDFPTAdditionalInfoDA.Data = m_objDFPTAdditionalInfo;
                        m_objDFPTAdditionalInfoDA.Insert(true, m_objDBConnection);
                        if (!m_objDFPTAdditionalInfoDA.Success || m_objDFPTAdditionalInfoDA.Message != string.Empty)
                        {
                            m_objDFPTAdditionalInfoDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                            return this.Direct(false, m_objDFPTAdditionalInfoDA.Message);
                        }
                    }
                    //PaymentMethod
                    if (!string.IsNullOrEmpty(m_objFPTVM.PaymentMethod))
                    {
                        m_objDFPTAdditionalInfo = new DFPTAdditionalInfo() { FPTAdditionalInfoID = Guid.NewGuid().ToString().Replace("-", ""), FPTID = m_strFPTID, FPTAdditionalInfoItemID = ((int)AdditionalInfoItems.PaymentMethod).ToString(), Value = m_objFPTVM.PaymentMethod };
                        m_objDFPTAdditionalInfoDA.Data = m_objDFPTAdditionalInfo;
                        m_objDFPTAdditionalInfoDA.Insert(true, m_objDBConnection);
                        if (!m_objDFPTAdditionalInfoDA.Success || m_objDFPTAdditionalInfoDA.Message != string.Empty)
                        {
                            m_objDFPTAdditionalInfoDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                            return this.Direct(false, m_objDFPTAdditionalInfoDA.Message);
                        }
                    }

                    //FPTScheduleStartManual
                    if (!string.IsNullOrEmpty(m_objFPTVM.FPTScheduleStartManual))
                    {
                        m_objDFPTAdditionalInfo = new DFPTAdditionalInfo() { FPTAdditionalInfoID = Guid.NewGuid().ToString().Replace("-", ""), FPTID = m_strFPTID, FPTAdditionalInfoItemID = ((int)AdditionalInfoItems.ScheduleStartManual).ToString(), Value = m_objFPTVM.FPTScheduleStartManual };
                        m_objDFPTAdditionalInfoDA.Data = m_objDFPTAdditionalInfo;
                        m_objDFPTAdditionalInfoDA.Insert(true, m_objDBConnection);
                        if (!m_objDFPTAdditionalInfoDA.Success || m_objDFPTAdditionalInfoDA.Message != string.Empty)
                        {
                            m_objDFPTAdditionalInfoDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                            return this.Direct(false, m_objDFPTAdditionalInfoDA.Message);
                        }
                    }

                    //FPTScheduleEndManual
                    if (!string.IsNullOrEmpty(m_objFPTVM.FPTScheduleEndManual))
                    {
                        m_objDFPTAdditionalInfo = new DFPTAdditionalInfo() { FPTAdditionalInfoID = Guid.NewGuid().ToString().Replace("-", ""), FPTID = m_strFPTID, FPTAdditionalInfoItemID = ((int)AdditionalInfoItems.ScheduleEndManual).ToString(), Value = m_objFPTVM.FPTScheduleEndManual };
                        m_objDFPTAdditionalInfoDA.Data = m_objDFPTAdditionalInfo;
                        m_objDFPTAdditionalInfoDA.Insert(true, m_objDBConnection);
                        if (!m_objDFPTAdditionalInfoDA.Success || m_objDFPTAdditionalInfoDA.Message != string.Empty)
                        {
                            m_objDFPTAdditionalInfoDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                            return this.Direct(false, m_objDFPTAdditionalInfoDA.Message);
                        }
                    }

                    //DurationManual
                    if (!string.IsNullOrEmpty(m_objFPTVM.DurationManual))
                    {
                        m_objDFPTAdditionalInfo = new DFPTAdditionalInfo() { FPTAdditionalInfoID = Guid.NewGuid().ToString().Replace("-", ""), FPTID = m_strFPTID, FPTAdditionalInfoItemID = ((int)AdditionalInfoItems.DurationManual).ToString(), Value = m_objFPTVM.DurationManual };
                        m_objDFPTAdditionalInfoDA.Data = m_objDFPTAdditionalInfo;
                        m_objDFPTAdditionalInfoDA.Insert(true, m_objDBConnection);
                        if (!m_objDFPTAdditionalInfoDA.Success || m_objDFPTAdditionalInfoDA.Message != string.Empty)
                        {
                            m_objDFPTAdditionalInfoDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                            return this.Direct(false, m_objDFPTAdditionalInfoDA.Message);
                        }
                    }

                    #endregion

                    #region Additional Info tipe pekerjaan
                    m_lstFilter = new List<object>();
                    m_objFilter = new Dictionary<string, List<object>>();

                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(Request.Params[nameof(FPTVM.AdditionalInfo1)].ToString());
                    m_objFilter.Add("FPTAdditionalInfoItemID", m_lstFilter);

                    //select
                    List<string> m_lstSelect = new List<string>();
                    m_lstSelect.Add("FPTAdditionalInfoItemID");
                    m_lstSelect.Add("Descriptions");

                    MAdditionalInfoItemsDA m_objAddInfoDA = new MAdditionalInfoItemsDA();
                    m_objAddInfoDA.ConnectionStringName = Global.ConnStrConfigName;
                    Dictionary<int, DataSet> m_FTPAdditionalInfoDA = m_objAddInfoDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
                    string addtionalInfoDesc = "";
                    if (m_objAddInfoDA.Message == string.Empty)
                    {
                        try
                        {
                            addtionalInfoDesc = m_FTPAdditionalInfoDA[0].Tables[0].Rows[0]["Descriptions"].ToString();

                            //FPTScheduleEndManual
                            m_objDFPTAdditionalInfo = new DFPTAdditionalInfo() { FPTAdditionalInfoID = Guid.NewGuid().ToString().Replace("-", ""), FPTID = m_strFPTID, FPTAdditionalInfoItemID = Request.Params[nameof(FPTVM.AdditionalInfo1)].ToString(), Value = addtionalInfoDesc };
                            m_objDFPTAdditionalInfoDA.Data = m_objDFPTAdditionalInfo;
                            m_objDFPTAdditionalInfoDA.Insert(true, m_objDBConnection);
                            if (!m_objDFPTAdditionalInfoDA.Success || m_objDFPTAdditionalInfoDA.Message != string.Empty)
                            {
                                m_objDFPTAdditionalInfoDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                                return this.Direct(false, m_objDFPTAdditionalInfoDA.Message);
                            }
                        }
                        catch(Exception ex)
                        {
                            throw new Exception(String.Format("Failed Get Additional Info Description : {0}", ex.Message));
                        }
                    }

                    #endregion

                        if (m_bissync) Sync(m_objFPTVM, ref m_lstMessage);
                    if (!m_objMFPTDA.Success || m_objMFPTDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMFPTDA.Message);
                    else
                        m_objMFPTDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                m_objMFPTDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
            }
            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
                return Detail(General.EnumDesc(Buttons.ButtonSave), null, m_strFPTID);
                //return Detail(General.EnumDesc(Buttons.ButtonSave), null, m_strBudgetPlanVersion, m_strBudgetPlanID);

            }
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);
        }
        public ActionResult SaveStatus(string Action)
        {
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();

            DFPTStatusDA m_objDFPTStatusDA = new DFPTStatusDA();
            MTasksDA m_objMTasksDA = new MTasksDA();
            DTaskDetailsDA m_objDTaskDetailsDA = new DTaskDetailsDA();

            m_objDFPTStatusDA.ConnectionStringName = Global.ConnStrConfigName;
            string m_strTransName = "DFPTStatus";
            object m_objDBConnection = null;
            m_objDBConnection = m_objDFPTStatusDA.BeginTrans(m_strTransName);

            int m_intStatusID = 0;
            string m_strDescriptions = string.Empty;
            string m_strFPTID = string.Empty;
            string m_strRemarks = string.Empty;
            string m_strTaskID = string.Empty;

            try
            {
                m_intStatusID = int.Parse(this.Request.Params[FPTStatusVM.Prop.StatusID.Name]);
                m_strDescriptions = this.Request.Params[FPTStatusVM.Prop.StatusDesc.Name];
                m_strFPTID = this.Request.Params[FPTStatusVM.Prop.FPTID.Name];
                m_strRemarks = this.Request.Params[FPTStatusVM.Prop.Remarks.Name];
                m_strTaskID = GetTaskID(m_strFPTID);
                m_lstMessage = IsSaveValidStatus(Action, m_strFPTID, m_intStatusID, m_strTaskID);
                if (m_lstMessage.Count <= 0)
                {
                    string m_insmessage = string.Empty;
                    //FPTStatus
                    if (!InsertDFPTStatus(m_strFPTID, m_intStatusID, DateTime.Now, ref m_insmessage, m_strRemarks, m_objDBConnection))
                    {
                        m_objDFPTStatusDA.EndTrans(ref m_objDBConnection, m_strTransName);
                        return this.Direct(false, m_objDFPTStatusDA.Message);
                    }

                    if (m_intStatusID == (int)FPTStatusTypes.ReNegotiation && m_strTaskID != string.Empty)
                    {
                        //MTask
                        m_objMTasksDA.ConnectionStringName = Global.ConnStrConfigName;
                        MTasks m_objMTasks = new MTasks();
                        m_objMTasks.TaskID = m_strTaskID;
                        m_objMTasksDA.Data = m_objMTasks;
                        m_objMTasksDA.Select();
                        m_objMTasks.StatusID = 4;
                        m_objMTasksDA.Update(true, m_objDBConnection);

                        if (!m_objMTasksDA.Success || m_objMTasksDA.Message != string.Empty)
                        {
                            m_objDFPTStatusDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                            return this.Direct(false, m_objMTasksDA.Message);
                        }
                        //DTaskDetail
                        m_objDTaskDetailsDA.ConnectionStringName = Global.ConnStrConfigName;
                        DTaskDetails m_objDTaskDetails = new DTaskDetails()
                        {
                            TaskDetailID = Guid.NewGuid().ToString().Replace("-", ""),
                            TaskID = m_strTaskID,
                            StatusID = 4,
                            Remarks = "Re Negotiation",

                        };
                        m_objDTaskDetailsDA.Data = m_objDTaskDetails;
                        m_objDTaskDetailsDA.Insert(true, m_objDBConnection);
                        if (!m_objDTaskDetailsDA.Success || m_objDTaskDetailsDA.Message != string.Empty)
                        {
                            m_objDFPTStatusDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                            return this.Direct(false, m_objDTaskDetailsDA.Message);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                m_objDFPTStatusDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
            }
            if (m_lstMessage.Count <= 0)
            {
                m_objDFPTStatusDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
                Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
                return StatusTracking(General.EnumDesc(Buttons.ButtonSave), null, m_strFPTID);
            }
            m_objDFPTStatusDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);


        }
        public ActionResult BrowseStatus(string ControlStatusID, string ControlStatusDesc, string FilterStatusID = "", string FilterStatusDesc = "")
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddStatus = new ViewDataDictionary();
            m_vddStatus.Add("Control" + FPTStatusVM.Prop.StatusID.Name, ControlStatusID);
            m_vddStatus.Add("Control" + FPTStatusVM.Prop.StatusDesc.Name, ControlStatusDesc);
            m_vddStatus.Add(FPTStatusVM.Prop.StatusID.Name, FilterStatusID);
            m_vddStatus.Add(FPTStatusVM.Prop.StatusDesc.Name, FilterStatusDesc);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddStatus,
                ViewName = "Status/_Browse"
            };
        }
        public ActionResult GetStatus(string ControlStatusID, string ControlStatusDesc, string FilterStatusID = "", string FilterStatusDesc = "", bool Exact = false)
        {
            try
            {
                Dictionary<int, List<FPTStatusVM>> m_dicStatusData = GetStatusData(true, FilterStatusID, FilterStatusDesc);
                KeyValuePair<int, List<FPTStatusVM>> m_kvpStatusVM = m_dicStatusData.AsEnumerable().ToList()[0];
                if (m_kvpStatusVM.Key < 1 || (m_kvpStatusVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpStatusVM.Key > 1 && !Exact)
                    return BrowseStatus(ControlStatusID, ControlStatusDesc, FilterStatusID, FilterStatusDesc);

                m_dicStatusData = GetStatusData(false, FilterStatusID, FilterStatusDesc);
                FPTStatusVM m_objStatusVM = m_dicStatusData[0][0];
                this.GetCmp<TextField>(ControlStatusID).Value = m_objStatusVM.StatusID;
                this.GetCmp<TextField>(ControlStatusDesc).Value = m_objStatusVM.StatusDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }
        public ActionResult Browse(string ControlFPTID, string ControlDescriptions, string FilterFPTID = "", string FilterDescriptions = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddFPT = new ViewDataDictionary();
            m_vddFPT.Add("Control" + FPTVM.Prop.FPTID.Name, ControlFPTID);
            m_vddFPT.Add("Control" + FPTVM.Prop.Descriptions.Name, ControlDescriptions);
            m_vddFPT.Add(FPTVM.Prop.FPTID.Name, FilterFPTID);
            m_vddFPT.Add(FPTVM.Prop.Descriptions.Name, FilterDescriptions);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddFPT,
                ViewName = "../FPT/_Browse"
            };
        }
        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            MFPTDA m_objMFPTDA = new MFPTDA();
            m_objMFPTDA.ConnectionStringName = Global.ConnStrConfigName;
            
            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMUoM = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMUoM.Conditions)
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
            Dictionary<int, DataSet> m_dicFPTDA = m_objMFPTDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpFPTBL in m_dicFPTDA)
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

                m_dicFPTDA = m_objMFPTDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMFPTDA.Message == string.Empty)
                {
                    m_lstFPTVM = (
                        from DataRow m_drMFPTDA in m_dicFPTDA[0].Tables[0].Rows
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
                            ClusterDesc = m_drMFPTDA[FPTVM.Prop.ClusterDesc.Name].ToString(),
                            DivisionID = m_drMFPTDA[FPTVM.Prop.DivisionID.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstFPTVM, m_intCount);
        }



        public ActionResult ReadStatus(StoreRequestParameters parameters, string FPTID)
        {
            DFPTStatusDA m_objDFPTStatusDA = new DFPTStatusDA();
            m_objDFPTStatusDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMStatus = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMStatus.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = StatusVM.Prop.Map(m_strDataIndex, false);
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

            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(FPTID);
            m_objFilter.Add(FPTStatusVM.Prop.FPTID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMStatusDA = m_objDFPTStatusDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpStatusBL in m_dicMStatusDA)
            {
                m_intCount = m_kvpStatusBL.Key;
                break;
            }

            List<FPTStatusVM> m_lstFPTStatusVM = new List<FPTStatusVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(FPTStatusVM.Prop.FPTStatusID.MapAlias);
                m_lstSelect.Add(FPTStatusVM.Prop.FPTID.MapAlias);
                m_lstSelect.Add(FPTStatusVM.Prop.StatusDateTimeStamp.MapAlias);
                m_lstSelect.Add(FPTStatusVM.Prop.StatusID.MapAlias);
                m_lstSelect.Add(FPTStatusVM.Prop.Remarks.MapAlias);
                m_lstSelect.Add(FPTStatusVM.Prop.StatusDesc.MapAlias);
                m_lstSelect.Add(FPTStatusVM.Prop.ModifiedBy.MapAlias);
                m_lstSelect.Add(FPTStatusVM.Prop.CreatedDate.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(StatusVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMStatusDA = m_objDFPTStatusDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objDFPTStatusDA.Message == string.Empty)
                {
                    m_lstFPTStatusVM = (
                        from DataRow m_drMStatusDA in m_dicMStatusDA[0].Tables[0].Rows
                        select new FPTStatusVM()
                        {
                            FPTStatusID = m_drMStatusDA[FPTStatusVM.Prop.FPTStatusID.Name].ToString(),
                    FPTID = m_drMStatusDA[FPTStatusVM.Prop.FPTID.Name].ToString(),
                    StatusDateTimeStamp = Convert.ToDateTime(m_drMStatusDA[FPTStatusVM.Prop.StatusDateTimeStamp.Name]),
                    StatusID = (int)m_drMStatusDA[FPTStatusVM.Prop.StatusID.Name],
                    Remarks = m_drMStatusDA[FPTStatusVM.Prop.Remarks.Name].ToString(),
                    StatusDesc = m_drMStatusDA[FPTStatusVM.Prop.StatusDesc.Name].ToString(),
                    ModifiedBy = m_drMStatusDA[FPTStatusVM.Prop.ModifiedBy.Name].ToString(),
                    CreatedDate = (DateTime)m_drMStatusDA[FPTStatusVM.Prop.CreatedDate.Name]

                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstFPTStatusVM, m_intCount);
        }
        public ActionResult ReadBrowseStatus(StoreRequestParameters parameters)
        {
            MStatusDA m_objMStatusDA = new MStatusDA();
            m_objMStatusDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMStatus = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMStatus.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = StatusVM.Prop.Map(m_strDataIndex, false);
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

            //List<string> m_lstKey = new List<string>();
            //Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            //List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("DFPTStatus");
            m_objFilter.Add(StatusVM.Prop.TableName.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(string.Join(",", manualStatusList.ToArray()));
            m_objFilter.Add(StatusVM.Prop.StatusID.Map, m_lstFilter);


            Dictionary<int, DataSet> m_dicMStatusDA = m_objMStatusDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpStatusBL in m_dicMStatusDA)
            {
                m_intCount = m_kvpStatusBL.Key;
                break;
            }

            List<StatusVM> m_lstStatusVM = new List<StatusVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(StatusVM.Prop.StatusID.MapAlias);
                m_lstSelect.Add(StatusVM.Prop.StatusDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(StatusVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMStatusDA = m_objMStatusDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMStatusDA.Message == string.Empty)
                {
                    m_lstStatusVM = (
                        from DataRow m_drMStatusDA in m_dicMStatusDA[0].Tables[0].Rows
                        select new StatusVM()
                        {
                            StatusID = Convert.ToInt32(m_drMStatusDA[StatusVM.Prop.StatusID.Name].ToString()),
                            StatusDesc = m_drMStatusDA[StatusVM.Prop.StatusDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstStatusVM, m_intCount);
        }
        public ActionResult GetAdditionalInfo(StoreRequestParameters parameters, string AdditionalInfo1)
        {
            List<UConfig> listUConfig = new List<UConfig>();
            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;

            //select
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add("Key1");
            m_lstSelect.Add("Key2");
            m_lstSelect.Add("Key3");

            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("FTPFormAdditionalInfo1");
            m_objFilter.Add("Key1", m_lstFilter);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add("Key2", OrderDirection.Ascending);

            Dictionary<int, DataSet> m_FTPAdditionalInfoDA = m_objUConfigDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, m_dicOrder);

            List<FPTVM> _listFTPVM = new List<FPTVM>();

            if (m_objUConfigDA.Message == string.Empty)
            {
                _listFTPVM = (
                from DataRow drm_FTPAdditionalInfoDA in m_FTPAdditionalInfoDA[0].Tables[0].Rows
                select new FPTVM
                {
                    AdditionalInfo1 = int.Parse(drm_FTPAdditionalInfoDA["Key2"].ToString()),
                    AdditionalInfo1Desc = drm_FTPAdditionalInfoDA["Key3"].ToString()
                }).Distinct().ToList();
            }

               
            return this.Store(_listFTPVM);
        }
            #endregion

            #region Private Method
            private bool Sync(FPTVM Model, ref List<string> message)
        {
            string m_duration = string.Empty;
            DateDifference dateDifference = new DateDifference(Model.FPTScheduleEnd, Model.FPTScheduleStart);
            m_duration = dateDifference.ToString();


            string m_FPTScheduleStart = Model.FPTScheduleStart.ToString("dd.MM.yyyy");
            string m_FPTScheduleEnd = Model.FPTScheduleEnd.ToString("dd.MM.yyyy");
            string m_Duration = m_duration;
            bool m_isManual = !string.IsNullOrEmpty(Model.FPTScheduleStartManual) || !string.IsNullOrEmpty(Model.FPTScheduleEndManual) || !string.IsNullOrEmpty(Model.DurationManual);
            if (m_isManual)
            {
                m_FPTScheduleStart = string.Empty;
                m_FPTScheduleEnd = string.Empty;
                m_Duration = Model.DurationManual;
            }



            string m_soapenv = $@"<?xml version='1.0' encoding='utf-8'?>
                            <soap:Envelope xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/'>
                              <soap:Body>
                                <UpdateImplementation xmlns='http://tempuri.org/'>
                                  <prmRequestNo>{Model.FPTID}</prmRequestNo>
                                  <prmSLATRM></prmSLATRM>
                                  <prmIsActual>0</prmIsActual>
                                  <prmKeterangan>-</prmKeterangan>
                                  <prmFPTCategory></prmFPTCategory>
                                  <prmDtTarget>{m_FPTScheduleStart}</prmDtTarget>
                                  <prmDtActual>{m_FPTScheduleEnd}</prmDtActual>
                                  <prmDurasiPelaksanaan>{m_Duration}</prmDurasiPelaksanaan>
                                  <prmMaintenancePeriod>{Model.MaintenancePeriod}</prmMaintenancePeriod>
                                  <prmGuarantee>{Model.Guarantee}</prmGuarantee>
                                  <prmContractType>{Model.ContractType}</prmContractType>
                                  <prmPaymentMethod>{Model.PaymentMethod}</prmPaymentMethod>
                                  <prmUpdateOnly>0</prmUpdateOnly>
                                </UpdateImplementation>
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

            XmlNodeList xnList = document.GetElementsByTagName("UpdateImplementationResult");

            foreach (XmlNode item in xnList)
            {
                if (item.Name == "UpdateImplementationResult" && item.InnerText == "")
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
        private string GetTaskID(string FPTID)
        {
            List<NegotiationConfigurationsVM> m_lstNegotiationConfigurationsVM = new List<NegotiationConfigurationsVM>();
            CNegotiationConfigurationsDA m_objCNegotiationConfigurationsDA = new CNegotiationConfigurationsDA();
            m_objCNegotiationConfigurationsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();

            m_lstSelect.Add(NegotiationConfigurationsVM.Prop.TaskID.MapAlias);

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
                    m_objNegotiationConfigurationsVM.TaskID = m_drCNegotiationConfigurationsDA[NegotiationConfigurationsVM.Prop.TaskID.Name].ToString();
                    m_lstNegotiationConfigurationsVM.Add(m_objNegotiationConfigurationsVM);
                }
            }
            else
            {
                return string.Empty;
            }
            if (m_lstNegotiationConfigurationsVM.Any())
            {
                return m_lstNegotiationConfigurationsVM.FirstOrDefault().TaskID;
            }
            else
            {
                return string.Empty;
            }


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
        private List<FPTDeviationsVM> GetListFPTDeviationsVM(string FPTID, ref string message)
        {
            List<FPTDeviationsVM> m_lstFPTDeviationsVM = new List<FPTDeviationsVM>();

            DFPTDeviationsDA m_objDFPTDeviationsDA = new DFPTDeviationsDA();
            m_objDFPTDeviationsDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(FPTDeviationsVM.Prop.FPTDeviationID.MapAlias);
            m_lstSelect.Add(FPTDeviationsVM.Prop.FPTID.MapAlias);
            m_lstSelect.Add(FPTDeviationsVM.Prop.RefNumber.MapAlias);
            m_lstSelect.Add(FPTDeviationsVM.Prop.RefTitle.MapAlias);
            m_lstSelect.Add(FPTDeviationsVM.Prop.RefDate.MapAlias);


            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(FPTID);
            m_objFilter.Add(FPTDeviationsVM.Prop.FPTID.Map, m_lstFilter);


            Dictionary<int, DataSet> m_dicDFPTDeviationsDA = m_objDFPTDeviationsDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDFPTDeviationsDA.Success)
            {
                foreach (DataRow m_drDFPTDeviationsDA in m_dicDFPTDeviationsDA[0].Tables[0].Rows)
                {
                    FPTDeviationsVM m_objFPTDeviationsVM = new FPTDeviationsVM();
                    m_objFPTDeviationsVM.FPTDeviationID = m_drDFPTDeviationsDA[FPTDeviationsVM.Prop.FPTDeviationID.Name].ToString();
                    m_objFPTDeviationsVM.FPTID = m_drDFPTDeviationsDA[FPTDeviationsVM.Prop.FPTID.Name].ToString();
                    m_objFPTDeviationsVM.RefNumber = m_drDFPTDeviationsDA[FPTDeviationsVM.Prop.RefNumber.Name].ToString();
                    m_objFPTDeviationsVM.RefTitle = m_drDFPTDeviationsDA[FPTDeviationsVM.Prop.RefTitle.Name].ToString();
                    m_objFPTDeviationsVM.RefDate = Convert.ToDateTime(m_drDFPTDeviationsDA[FPTDeviationsVM.Prop.RefDate.Name]);
                    m_lstFPTDeviationsVM.Add(m_objFPTDeviationsVM);
                }
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDFPTDeviationsDA.Message;

            return m_lstFPTDeviationsVM;
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
        private Dictionary<string, object> GetFormData(NameValueCollection parameters, string FPTID = "")
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(FPTVM.Prop.FPTID.Name, (parameters[FPTVM.Prop.FPTID.Name].ToString() == string.Empty ? FPTID : parameters[FPTVM.Prop.FPTID.Name]));
            m_dicReturn.Add(FPTVM.Prop.Descriptions.Name, parameters[FPTVM.Prop.Descriptions.Name]);
            m_dicReturn.Add(FPTVM.Prop.DocumentComplete.Name, parameters[FPTVM.Prop.DocumentComplete.Name]);
            m_dicReturn.Add(FPTVM.Prop.FPTDeviationsVM.Name, parameters[FPTVM.Prop.FPTDeviationsVM.Name]);

            return m_dicReturn;
        }
        private FPTVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            FPTVM m_objFPTVM = new FPTVM();
            MFPTDA m_objMFPTVMDA = new MFPTDA();
            m_objMFPTVMDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(FPTVM.Prop.FPTID.MapAlias);
            m_lstSelect.Add(FPTVM.Prop.Descriptions.MapAlias);
            m_lstSelect.Add(FPTVM.Prop.ClusterID.MapAlias);
            m_lstSelect.Add(FPTVM.Prop.ClusterDesc.MapAlias);
            m_lstSelect.Add(FPTVM.Prop.ProjectID.MapAlias);
            m_lstSelect.Add(FPTVM.Prop.ProjectDesc.MapAlias);
            m_lstSelect.Add(FPTVM.Prop.DivisionID.MapAlias);
            m_lstSelect.Add(FPTVM.Prop.DivisionDesc.MapAlias);
            m_lstSelect.Add(FPTVM.Prop.BusinessUnitID.MapAlias);
            m_lstSelect.Add(FPTVM.Prop.BusinessUnitDesc.MapAlias);
            m_lstSelect.Add(FPTVM.Prop.AdditionalInfo1.MapAlias);
            m_lstSelect.Add(FPTVM.Prop.AdditionalInfo1Desc.MapAlias);
            m_lstSelect.Add(FPTVM.Prop.IsSync.MapAlias);
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

            Dictionary<int, DataSet> m_dicMFPTDA = m_objMFPTVMDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMFPTVMDA.Message == string.Empty)
            {

                DataRow m_drMFPTDA = m_dicMFPTDA[0].Tables[0].Rows[0];
                var dev = GetListFPTDeviationsVM(m_drMFPTDA[FPTVM.Prop.FPTID.Name].ToString(), ref message);
                m_objFPTVM.FPTID = m_drMFPTDA[FPTVM.Prop.FPTID.Name].ToString();
                m_objFPTVM.Descriptions = m_drMFPTDA[FPTVM.Prop.Descriptions.Name].ToString();
                m_objFPTVM.ClusterID = m_drMFPTDA[FPTVM.Prop.ClusterID.Name].ToString();
                m_objFPTVM.ClusterDesc = m_drMFPTDA[FPTVM.Prop.ClusterDesc.Name].ToString();
                m_objFPTVM.ProjectID = m_drMFPTDA[FPTVM.Prop.ProjectID.Name].ToString();
                m_objFPTVM.ProjectDesc = m_drMFPTDA[FPTVM.Prop.ProjectDesc.Name].ToString();
                m_objFPTVM.DivisionID = m_drMFPTDA[FPTVM.Prop.DivisionID.Name].ToString();
                m_objFPTVM.DivisionDesc = m_drMFPTDA[FPTVM.Prop.DivisionDesc.Name].ToString();
                m_objFPTVM.BusinessUnitID = m_drMFPTDA[FPTVM.Prop.BusinessUnitID.Name].ToString();
                m_objFPTVM.BusinessUnitDesc = m_drMFPTDA[FPTVM.Prop.BusinessUnitDesc.Name].ToString();
                m_objFPTVM.AdditionalInfo1 = (m_drMFPTDA[FPTVM.Prop.AdditionalInfo1.Name].ToString() == string.Empty ? 0 : int.Parse(m_drMFPTDA[FPTVM.Prop.AdditionalInfo1.Name].ToString()));
                m_objFPTVM.AdditionalInfo1Desc = m_drMFPTDA[FPTVM.Prop.AdditionalInfo1Desc.Name].ToString();
                m_objFPTVM.IsSync = (bool)m_drMFPTDA[FPTVM.Prop.IsSync.Name];
                m_objFPTVM.FPTDeviationsVM = (dev.Any()) ? dev.FirstOrDefault() : null;
                m_objFPTVM.DocumentComplete = !dev.Any();
                m_objFPTVM.ListFPTStatusVM = GetListFPTStatusVM(m_drMFPTDA[FPTVM.Prop.FPTID.Name].ToString(), ref message);
                string m_strmsg = string.Empty;
                List<FPTAdditionalInfoVM> m_ListFPTAdditionalInfoVM = GetListFPTAdditionalInfoVM(m_objFPTVM.FPTID, ref m_strmsg);
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

                m_objFPTVM.FPTScheduleStart = m_dtstart;
                m_objFPTVM.FPTScheduleEnd = m_dtend;
                m_objFPTVM.FPTScheduleStartManual = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "11").Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "11").FirstOrDefault().Value : null;
                m_objFPTVM.FPTScheduleEndManual = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "12").Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "12").FirstOrDefault().Value : null;
                m_objFPTVM.Duration = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "3").Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "3").FirstOrDefault().Value : null;
                m_objFPTVM.DurationManual = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "13").Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "13").FirstOrDefault().Value : null;
                m_objFPTVM.MaintenancePeriod = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "4").Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "4").FirstOrDefault().Value : null;
                m_objFPTVM.Guarantee = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "5").Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "5").FirstOrDefault().Value : null;
                m_objFPTVM.ContractType = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "6").Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "6").FirstOrDefault().Value : null;
                m_objFPTVM.PaymentMethod = m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "7").Any() ? m_ListFPTAdditionalInfoVM.Where(x => x.FPTAdditionalInfoItemID == "7").FirstOrDefault().Value : null;
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMFPTVMDA.Message;

            return m_objFPTVM;
        }
        private List<string> IsSaveValid(string Action, FPTVM objFPTVM)
        {
            List<string> m_lstReturn = new List<string>();
            //FPT
            if (string.IsNullOrEmpty(objFPTVM.Descriptions))
                m_lstReturn.Add(FPTVM.Prop.Descriptions.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            //FPTDeviations
            if (!objFPTVM.DocumentComplete)
            {
                if (string.IsNullOrEmpty(objFPTVM.FPTDeviationsVM.RefNumber))
                    m_lstReturn.Add(FPTDeviationsVM.Prop.RefNumber.Desc + " " + General.EnumDesc(MessageLib.mustFill));
                if (objFPTVM.FPTDeviationsVM.RefDate == null)
                    m_lstReturn.Add(FPTDeviationsVM.Prop.RefDate.Desc + " " + General.EnumDesc(MessageLib.mustFill));
                if (string.IsNullOrEmpty(objFPTVM.FPTDeviationsVM.RefTitle))
                    m_lstReturn.Add(FPTDeviationsVM.Prop.RefTitle.Desc + " " + General.EnumDesc(MessageLib.mustFill));
                if (objFPTVM.FPTDeviationsVM.RefDate == DateTime.MinValue)
                    m_lstReturn.Add(FPTDeviationsVM.Prop.RefDate.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            }

            return m_lstReturn;
        }

        private List<string> IsSaveValidStatus(string Action, string m_strFPTID, int m_intStatusID, string m_strTaskID)
        {
            List<string> m_lstReturn = new List<string>();
            //FPT
            if (string.IsNullOrEmpty(m_strFPTID))
                m_lstReturn.Add(FPTStatusVM.Prop.FPTID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            if (!manualStatusList.Contains(m_intStatusID))
                m_lstReturn.Add("Only manual status");
            if (m_intStatusID == (int)FPTStatusTypes.ReNegotiation)
            {
                string m_strmsg = string.Empty;
                List<FPTStatusVM> m_ListFPTStatusVM = GetListFPTStatusVM(m_strFPTID, ref m_strmsg);
                if (!m_ListFPTStatusVM.Any(x => x.StatusID == (int)FPTStatusTypes.DoneNegotiation))
                {
                    m_lstReturn.Add("FPT Not Done!");
                }
            }
            //Todo: Check Exist?


            return m_lstReturn;
        }
        private bool isDeviationExist(string FPTID)
        {
            bool m_boolRetval = false;


            return m_boolRetval;
        }
        private bool isDeviationChange(string FPTID)
        {
            bool m_boolRetval = true;


            return m_boolRetval;
        }

        #endregion

        #region Public Method
        public Dictionary<int, List<FPTStatusVM>> GetStatusData(bool isCount, string StatusID, string StatusDesc)
        {
            int m_intCount = 0;
            List<FPTStatusVM> m_lstFPTStatusVM = new List<FPTStatusVM>();
            Dictionary<int, List<FPTStatusVM>> m_dicReturn = new Dictionary<int, List<FPTStatusVM>>();
            MStatusDA m_objMStatusDA = new MStatusDA();
            m_objMStatusDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(StatusVM.Prop.StatusID.MapAlias);
            m_lstSelect.Add(StatusVM.Prop.StatusDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(StatusID);
            m_objFilter.Add(StatusVM.Prop.StatusID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(StatusDesc);
            m_objFilter.Add(StatusVM.Prop.StatusDesc.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(typeof(DFPTStatus).Name);
            m_objFilter.Add(StatusVM.Prop.TableName.Name, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(string.Join(",", manualStatusList.ToArray()));
            m_objFilter.Add(StatusVM.Prop.StatusID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMStatusDA = m_objMStatusDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMStatusDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpStatusBL in m_dicMStatusDA)
                    {
                        m_intCount = m_kvpStatusBL.Key;
                        break;
                    }
                else
                {
                    m_lstFPTStatusVM = (
                        from DataRow m_drMStatusDA in m_dicMStatusDA[0].Tables[0].Rows
                        select new FPTStatusVM()
                        {
                            StatusID = (int)m_drMStatusDA[FPTStatusVM.Prop.StatusID.Name],
                            StatusDesc = m_drMStatusDA[FPTStatusVM.Prop.StatusDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstFPTStatusVM);
            return m_dicReturn;
        }
        #endregion

        #region Nested
        public class DateDifference
        {
            /// <summary>
            /// defining Number of days in month; index 0=> january and 11=> December
            /// february contain either 28 or 29 days, that's why here value is -1
            /// which wil be calculate later.
            /// </summary>
            private int[] monthDay = new int[12] { 31, -1, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

            /// <summary>
            /// contain from date
            /// </summary>
            private DateTime fromDate;

            /// <summary>
            /// contain To Date
            /// </summary>
            private DateTime toDate;

            /// <summary>
            /// this three variable for output representation..
            /// </summary>
            private int year;
            private int month;
            private int day;

            public DateDifference(DateTime d1, DateTime d2)
            {
                int increment;

                if (d1 > d2)
                {
                    this.fromDate = d2;
                    this.toDate = d1;
                }
                else
                {
                    this.fromDate = d1;
                    this.toDate = d2;
                }

                /// 
                /// Day Calculation
                /// 
                increment = 0;

                if (this.fromDate.Day > this.toDate.Day)
                {
                    increment = this.monthDay[this.fromDate.Month - 1];

                }
                /// if it is february month
                /// if it's to day is less then from day
                if (increment == -1)
                {
                    if (DateTime.IsLeapYear(this.fromDate.Year))
                    {
                        // leap year february contain 29 days
                        increment = 29;
                    }
                    else
                    {
                        increment = 28;
                    }
                }
                if (increment != 0)
                {
                    day = (this.toDate.Day + increment) - this.fromDate.Day;
                    increment = 1;
                }
                else
                {
                    day = this.toDate.Day - this.fromDate.Day;
                }

                ///
                ///month calculation
                ///
                if ((this.fromDate.Month + increment) > this.toDate.Month)
                {
                    this.month = (this.toDate.Month + 12) - (this.fromDate.Month + increment);
                    increment = 1;
                }
                else
                {
                    this.month = (this.toDate.Month) - (this.fromDate.Month + increment);
                    increment = 0;
                }

                ///
                /// year calculation
                ///
                this.year = this.toDate.Year - (this.fromDate.Year + increment);

            }

            public override string ToString()
            {
                int m_year = this.year;
                int m_month = this.month;
                int m_day = this.day + 1;

                if (m_day > 29)
                {
                    m_month += 1;
                    m_day = 0;
                }

                string m_return = string.Empty;
                if (m_year > 0) m_return += m_year + " Tahun ";
                if (m_month > 0) m_return += m_month + " Bulan ";
                if (m_day > 0) m_return += m_day + " Hari ";
                return m_return;
            }

            public int Years
            {
                get
                {
                    return this.year;
                }
            }

            public int Months
            {
                get
                {
                    return this.month;
                }
            }

            public int Days
            {
                get
                {
                    return this.day;
                }
            }

        }

        #endregion
    }
}