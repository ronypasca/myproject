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
//using Novacode;
using System.IO;
using System.IO.Compression;
//using Spire.Doc;
using System.Net;
using System.Xml;

namespace com.SML.BIGTRONS.Controllers
{
    public class VendorWinnerController : BaseController
    {
        private readonly string title = "Vendor Winner";
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

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcCApprovalPath = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            foreach (FilterHeaderCondition m_fhcFilter in m_fhcCApprovalPath.Conditions)
            {
                string m_strDataIndex = (m_fhcFilter.DataIndex == "FPTDesc") ? "Descriptions" : m_fhcFilter.DataIndex;
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

            foreach (var item in m_lstFPTNegotiationRoundVM)
            {
                List<FPTVendorWinnerVM> m_lstFPTVendorWinnerVM = getFPTVendorWinnerVM(item.FPTID);

                if (m_lstFPTVendorWinnerVM.Any())
                {
                    int m_intstatus = m_lstFPTVendorWinnerVM.FirstOrDefault().StatusID;
                    switch (m_intstatus)
                    {
                        case 0:
                            item.FPTWinnerStatus = General.EnumDesc(TaskStatus.InProgress);
                            break;
                        case 1:
                            item.FPTWinnerStatus = General.EnumDesc(TaskStatus.Revise);
                            break;
                        case 2:
                            item.FPTWinnerStatus = General.EnumDesc(TaskStatus.Approved);
                            break;
                        case 3:
                            item.FPTWinnerStatus = General.EnumDesc(TaskStatus.Rejected);
                            break;
                        case 4:
                            item.FPTWinnerStatus = General.EnumDesc(TaskStatus.Draft);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    item.FPTWinnerStatus = "New";
                }

            }

            return this.Store(m_lstFPTNegotiationRoundVM, m_intCount);
        }
        public ActionResult Home()
        {
            this.GetCmp<Ext.Net.Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 0;
            return this.Direct();
        }
        public ActionResult Update(string Caller, string Selected)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Update)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));
            string m_strFPTID;
            string m_strRoundID;
            //string m_strTCMemberID;

            //Todo: Check if already verify
            if (string.IsNullOrEmpty(Selected) && Caller == General.EnumDesc(Buttons.ButtonUpdate))
            {
                return this.Direct(false);
            }
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

            m_strFPTID = m_dicSelectedRow[FPTNegotiationRoundVM.Prop.FPTID.Name].ToString();
            m_strRoundID = m_dicSelectedRow[FPTNegotiationRoundVM.Prop.RoundID.Name].ToString();

            //List<ConfigVM> m_lstConfigVM = GetValuePriceTolerance();
            //decimal m_decMarginTop = m_lstConfigVM.Where(x => x.Key2 == "TOP").Any() ? Convert.ToDecimal(m_lstConfigVM.Where(x => x.Key2 == "TOP").FirstOrDefault().Desc1) : 0;
            //decimal m_decMarginBottom = m_lstConfigVM.Where(x => x.Key2 == "BOTTOM").Any() ? Convert.ToDecimal(m_lstConfigVM.Where(x => x.Key2 == "TOP").FirstOrDefault().Desc1) : 0;

            ViewDataDictionary m_vddFPT = new ViewDataDictionary();
            m_vddFPT.Add("FPTIDData", m_strFPTID);
            //m_vddFPT.Add("TCMemberIDData", m_strTCMemberID);
            m_vddFPT.Add("RoundIDData", m_strRoundID);
            m_vddFPT.Add("FPTNameData", m_strFPTID);
            //m_vddFPT.Add("PriceMarginTopData", m_decMarginTop);
            //m_vddFPT.Add("PriceMarginBottomData", m_decMarginBottom);
            m_vddFPT.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddFPT.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Ext.Net.Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;

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
        public ActionResult Detail(string Caller, string Selected)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Update)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            string m_strFPTID;
            string m_strRoundID;
            string m_strStatus;
            if (string.IsNullOrEmpty(Selected))
            {
                return this.Direct(false);
            }
            Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();
            m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
            m_strFPTID = m_dicSelectedRow[FPTNegotiationRoundVM.Prop.FPTID.Name].ToString();
            m_strRoundID = m_dicSelectedRow[FPTNegotiationRoundVM.Prop.RoundID.Name].ToString();
            m_strStatus = m_dicSelectedRow["FPTWinnerStatus"].ToString();
            //List<ConfigVM> m_lstConfigVM = GetValuePriceTolerance();
            //decimal m_decMarginTop = m_lstConfigVM.Where(x => x.Key2 == "TOP").Any() ? Convert.ToDecimal(m_lstConfigVM.Where(x => x.Key2 == "TOP").FirstOrDefault().Desc1) : 0;
            //decimal m_decMarginBottom = m_lstConfigVM.Where(x => x.Key2 == "BOTTOM").Any() ? Convert.ToDecimal(m_lstConfigVM.Where(x => x.Key2 == "TOP").FirstOrDefault().Desc1) : 0;

            ViewDataDictionary m_vddFPT = new ViewDataDictionary();
            m_vddFPT.Add("FPTIDData", m_strFPTID);
            m_vddFPT.Add("RoundIDData", m_strRoundID);
            m_vddFPT.Add("FPTNameData", m_strFPTID);
            m_vddFPT.Add("StatusData", m_strStatus);
            //m_vddFPT.Add("PriceMarginTopData", m_decMarginTop);
            //m_vddFPT.Add("PriceMarginBottomData", m_decMarginBottom);
            m_vddFPT.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
            this.GetCmp<Ext.Net.Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;

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
        
        public ActionResult BrowseAtt(string ControlEmployeeID, string ControlEmployeeName, string FilterEmployeeID = "", string FilterEmployeeName = "", string FilterFPTID = "")
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddEmployee = new ViewDataDictionary();
            m_vddEmployee.Add("Control" + EmployeeVM.Prop.EmployeeID.Name, ControlEmployeeID);
            m_vddEmployee.Add("Control" + EmployeeVM.Prop.EmployeeName.Name, ControlEmployeeName);
            m_vddEmployee.Add(EmployeeVM.Prop.EmployeeID.Name, FilterEmployeeID);
            m_vddEmployee.Add(EmployeeVM.Prop.EmployeeName.Name, FilterEmployeeName);
            m_vddEmployee.Add("Filter" + FPTVM.Prop.FPTID.Name, FilterFPTID);


            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddEmployee,
                ViewName = "../VendorWinner/_Browse"
            };
        }
        public ActionResult ReadBrowseAtt(StoreRequestParameters parameters, string FilterFPTID = "")
        {
            
            MEmployeeDA m_objMEmployeeDA = new MEmployeeDA();
            m_objMEmployeeDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMEmployee = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMEmployee.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);
                
                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = EmployeeVM.Prop.Map(m_strDataIndex, false);
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
            m_lstFilter.Add(FilterFPTID);
            m_objFilter.Add(EmployeeVM.Prop.FPTID.Name, m_lstFilter);

            Dictionary<int, DataSet> m_dicMEmployeeDA = m_objMEmployeeDA.SelectBCAtt(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpEmployeeBL in m_dicMEmployeeDA)
            {
                m_intCount = m_kvpEmployeeBL.Key;
                break;
            }

            List<EmployeeVM> m_lstEmployeeVM = new List<EmployeeVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add("AttendeeType");
                m_lstSelect.Add(EmployeeVM.Prop.EmployeeID.MapAlias);
                m_lstSelect.Add(EmployeeVM.Prop.EmployeeName.MapAlias);
                m_lstSelect.Add(EmployeeVM.Prop.FirstName.MapAlias);
                m_lstSelect.Add("MiddleName");
                m_lstSelect.Add(EmployeeVM.Prop.LastName.MapAlias);
                m_lstSelect.Add("FPTID");
                
                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(EmployeeVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMEmployeeDA = m_objMEmployeeDA.SelectBCAtt(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMEmployeeDA.Message == string.Empty)
                {
                    m_lstEmployeeVM = (
                        from DataRow m_drMEmployeeDA in m_dicMEmployeeDA[0].Tables[0].Rows
                        select new EmployeeVM()
                        {
                            EmployeeName = m_drMEmployeeDA[EmployeeVM.Prop.EmployeeName.Name].ToString(),
                            AttendeeType = m_drMEmployeeDA[EmployeeVM.Prop.AttendeeType.Name].ToString(),
                            EmployeeID = m_drMEmployeeDA[EmployeeVM.Prop.EmployeeID.Name].ToString(),
                            FirstName = m_drMEmployeeDA[EmployeeVM.Prop.FirstName.Name].ToString(),
                            MiddleName = m_drMEmployeeDA[EmployeeVM.Prop.MiddleName.Name].ToString(),
                            LastName = m_drMEmployeeDA[EmployeeVM.Prop.LastName.Name].ToString(),
                            FPTID = m_drMEmployeeDA[EmployeeVM.Prop.FPTID.Name].ToString(),
                            
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstEmployeeVM, m_intCount);
        }



        public ActionResult GetPanelAtt(string FPTID)
        {

            List<FPTAttendancesVM> m_lstFPTAttendancesVM = getFPTAttendancesVM(FPTID);

            Panel BPPanel = new Panel
            {
                ID = "P" + "Att" + "Form",
                Frame = true,
                Border = false
            };
            Toolbar m_BPPanelToolbar = new Toolbar();

            GridPanel m_GPBP = new GridPanel
            {
                ID = "gridPanelAttstructure",
                Padding = 10,
                MinHeight = 200
            };

            //SelectionModel
            RowSelectionModel m_rowSelectionModel = new RowSelectionModel() { Mode = SelectionMode.Multi, AllowDeselect = true };
            m_GPBP.SelectionModel.Add(m_rowSelectionModel);

            //Store         
            Store m_store = new Store();
            Model m_model = new Model();
            ModelField m_ModelField = new ModelField();
            m_ModelField = new ModelField(FPTAttendancesVM.Prop.FPTAttendanceID.Name);
            m_model.Fields.Add(m_ModelField);
            m_ModelField = new ModelField(FPTAttendancesVM.Prop.FPTID.Name);
            m_model.Fields.Add(m_ModelField);
            m_ModelField = new ModelField(FPTAttendancesVM.Prop.AttendeeType.Name);
            m_model.Fields.Add(m_ModelField);
            m_ModelField = new ModelField(FPTAttendancesVM.Prop.IDAttendee.Name);
            m_model.Fields.Add(m_ModelField);
            m_ModelField = new ModelField(FPTAttendancesVM.Prop.IsAttend.Name);
            m_model.Fields.Add(m_ModelField);
            m_ModelField = new ModelField(FPTAttendancesVM.Prop.AttendanceDesc.Name);
            m_model.Fields.Add(m_ModelField);
            m_ModelField = new ModelField("AttendanceName");
            m_model.Fields.Add(m_ModelField);

            m_store.Model.Add(m_model);
            m_store.DataSource = m_lstFPTAttendancesVM.OrderBy(x => x.AttendeeType);
            m_GPBP.Store.Add(m_store);

            //Columns
            List<ColumnBase> m_ListColumn = new List<ColumnBase>();
            Checkbox m_objCheckbox = new Checkbox();
            ColumnBase m_objColumn = new Ext.Net.Column();
            m_objColumn = new Ext.Net.Column { Text = FPTAttendancesVM.Prop.FPTAttendanceID.Desc, DataIndex = "FPTAttendanceID", Flex = 1, Hidden = true };
            m_ListColumn.Add(m_objColumn);
            m_objColumn = new Ext.Net.Column { Text = FPTAttendancesVM.Prop.FPTID.Desc, DataIndex = "FPTID", Flex = 1, Hidden = true };
            m_ListColumn.Add(m_objColumn);
            m_objColumn = new Ext.Net.Column { Text = FPTAttendancesVM.Prop.IDAttendee.Desc, DataIndex = "IDAttendee", Flex = 1, Hidden = true };
            m_ListColumn.Add(m_objColumn);
            m_objColumn = new CheckColumn { Text = "Is Attend", DataIndex = "IsAttend", Editable = true };
            m_ListColumn.Add(m_objColumn);
            m_objColumn = new Ext.Net.Column { Text = FPTAttendancesVM.Prop.AttendeeType.Desc, DataIndex = "AttendeeType", Hidden = false };
            m_ListColumn.Add(m_objColumn);
            m_objColumn = new Column { Text = "Name", DataIndex = "AttendanceName", Flex = 1, Hidden = false };
            var m_objtextfield = new TextField();
            m_objtextfield.ReadOnly = true;
            m_objtextfield.HideTrigger = true;
            m_objtextfield.AllowBlank = false;
            m_objtextfield.RightButtonsShowMode = ButtonsShowMode.MouseOverOrFocus;
            Button m_btnBrowse = new Button() { ID = "BtnBrowseAtt", Icon = Icon.Find };//todo
            m_btnBrowse.DirectEvents.Click.Action = "../vendorwinner/BrowseAtt";
            m_btnBrowse.DirectEvents.Click.EventMask.ShowMask = true;
            m_btnBrowse.DirectEvents.Click.ExtraParams.Add(new Parameter("FilterFPTID", FPTID, ParameterMode.Value, false));
            m_btnBrowse.DirectEvents.Click.ExtraParams.Add(new Parameter("ControlEmployeeID", "App.gridPanelAttstructure.getSelectionModel().getSelected().items[0].id", ParameterMode.Raw, false));

            Button m_btnErase = new Button() { ID = "BtnEraseAtt", Icon = Icon.Erase };//todo
            m_objtextfield.RightButtons.Add(m_btnBrowse);
            m_objtextfield.RightButtons.Add(m_btnErase);
            m_objColumn.Editor.Add(m_objtextfield);
            m_ListColumn.Add(m_objColumn);
            m_objColumn = new Ext.Net.Column { Text = "On Behalf", DataIndex = "AttendanceDesc", Flex = 1, Hidden = false };
            m_objtextfield = new TextField();
            m_objtextfield.HideTrigger = true;
            m_objColumn.Editor.Add(m_objtextfield);
            m_ListColumn.Add(m_objColumn);
            m_GPBP.ColumnModel.Columns.AddRange(m_ListColumn);

            //Plugin
            CellEditing m_objCellEditing = new CellEditing() { ClicksToEdit = 1 };
            m_GPBP.Plugins.Add(m_objCellEditing);


            Button m_btnAdd = new Button() { ID = "BtnAddAtt", Text = "Add", Icon = Icon.Add };
            m_btnAdd.Handler = "addatt";
            m_btnAdd.Hidden = false;
            m_BPPanelToolbar.Items.Add(m_btnAdd);

            Button m_btnDelete = new Button() { ID = "BtnDeleteAtt", Text = "Delete", Icon = Icon.Delete };
            m_btnDelete.Handler = "deleteatt";
            m_btnDelete.Hidden = false;
            m_BPPanelToolbar.Items.Add(m_btnDelete);

            BPPanel.TopBar.Add(m_BPPanelToolbar);
            BPPanel.Items.Add(m_GPBP);
            return this.ComponentConfig(BPPanel);

        }
        public ActionResult GetPanel(string FPTID)
        {

            //Get All Structure
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();
            string m_strMessage = string.Empty;

            List<FPTVendorParticipantsVM> m_lstFPTVendorParticipantsVM = getFPTVendorParticipantsVM(FPTID);
            List<FPTTCParticipantsVM> m_lstFPTTCParticipantsVM = GetListNegoTCParticipant(FPTID,ref m_strMessage);
            List<FPTVendorRecommendationsVM> m_lstFPTVendorRecommendationsVM = getFPTVendorRecommendationsVM(FPTID);
            List<FPTVendorWinnerVM> m_lstFPTVendorWinnerVM = getFPTVendorWinnerVM(FPTID);
            List<NegotiationBidEntryVM> m_lstNegotiationBidEntryVM = getNegotiationBidEntryVM(FPTID);
            List<FPTNegotiationRoundVM> m_lstFPTNegotiationRoundVM = getlstNegoRound(FPTID);

            foreach (var item in m_lstFPTTCParticipantsVM)
            {

                if (!m_lstFPTVendorRecommendationsVM.Any(d => d.TCMemberID == item.TCMemberID))
                {
                    foreach (var objVendorParticipant in m_lstFPTVendorParticipantsVM)
                        m_lstFPTVendorRecommendationsVM.Add(new FPTVendorRecommendationsVM { FPTVendorParticipantID = objVendorParticipant.FPTVendorParticipantID, TCMemberID = item.TCMemberID, TCMemberName = item.EmployeeName, Remarks = "", IsProposed = false });
                }
            }

            foreach (var item in m_lstFPTVendorParticipantsVM)
            {
                item.TCName = string.Join("$", m_lstFPTVendorRecommendationsVM.OrderBy(d => d.TCMemberID).Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID).Select(x => x.TCMemberName).ToArray());
                item.RecommendationRemark = string.Join("$", m_lstFPTVendorRecommendationsVM.OrderBy(d => d.TCMemberID).Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID).Select(x => x.Remarks).ToArray());
                item.IsProposedWinner = string.Join("$", m_lstFPTVendorRecommendationsVM.OrderBy(d => d.TCMemberID).Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID).Select(x => x.IsProposed.ToString()).ToArray());
                item.IsWinner = (m_lstFPTVendorWinnerVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID).Any()) ? m_lstFPTVendorWinnerVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID).FirstOrDefault().IsWinner : false;
                item.IsAttend = string.Join("$", m_lstFPTTCParticipantsVM.OrderBy(d => d.TCMemberID).Select(x => x.StatusID.ToString()).ToArray());
                //List<string> lstrecom = new List<string>();
                //foreach (var itemFPTVendorRecommendationsVM in m_lstFPTVendorRecommendationsVM.Where(x => x.FPTVendorParticipantID == item.FPTVendorParticipantID).ToList())
                //{
                //    string m_proposed = itemFPTVendorRecommendationsVM.IsProposed ? "<span class='x-grid-checkcolumn x-grid-checkcolumn-checked' role='button'></span>" : "<span class='x-grid-checkcolumn' role='button'></span>";
                //    lstrecom.Add($"{m_proposed} {itemFPTVendorRecommendationsVM.TCMemberName} Remarks : {itemFPTVendorRecommendationsVM.Remarks}");
                //}
                //item.IsProposedWinner = string.Join("<br>", lstrecom);

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
                        //m_param = new Parameter("TCMemberID", TCMemberID, ParameterMode.Value, false);
                        //m_lstParameter.Add(m_param);
                        Button m_btnBPSubmit = new Button() { ID = "BtnSubmit" + m_GPBP.ID, Text = "Save" };
                        m_btnBPSubmit.DirectEvents.Click.Action = Url.Action("Save");
                        m_btnBPSubmit.DirectEvents.Click.EventMask.ShowMask = true;
                        m_btnBPSubmit.DirectEvents.Click.ExtraParams.AddRange(m_lstParameter);
                        m_btnBPSubmit.Hidden = true;
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
                    ID = "P" + "Upload" + "Form",//todo
                    Frame = true,
                    Title = "Upload",//todo
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
                //m_param = new Parameter("TCMemberID", TCMemberID, ParameterMode.Value, false);
                //m_lstParameter.Add(m_param);
                Button m_btnBPSubmit = new Button() { ID = "BtnSubmit" + m_GPBP.ID, Text = "Save" };
                m_btnBPSubmit.DirectEvents.Click.Action = Url.Action("Save");
                m_btnBPSubmit.DirectEvents.Click.EventMask.ShowMask = true;
                m_btnBPSubmit.DirectEvents.Click.ExtraParams.AddRange(m_lstParameter);
                m_btnBPSubmit.Hidden = true;
                m_BPPanelToolbar.Items.Add(m_btnBPSubmit);

                BPPanel.TopBar.Add(m_BPPanelToolbar);
                BPPanel.Items.Add(m_GPBP);
                return this.ComponentConfig(BPPanel);
            }

        }
        public ActionResult Save()
        {
            Dictionary<string, object>[] m_arrList = JSON.Deserialize<Dictionary<string, object>[]>(Request.Params["GridList"]);
            string FPTID = Request.Params["FPTID"].ToString();
            //string TCMemberID = Request.Params["TCMemberID"].ToString();

            List<DFPTVendorWinner> m_lstDFPTVendorWinner = new List<DFPTVendorWinner>();

            m_lstDFPTVendorWinner = (
                            from Dictionary<string, object> m_dicDFPTVendorRecommendations in m_arrList
                            select new DFPTVendorWinner()
                            {
                                VendorWinnerID = Guid.NewGuid().ToString().Replace("-", ""),
                                FPTID = FPTID,
                                FPTVendorParticipantID = m_dicDFPTVendorRecommendations.ContainsKey(nameof(FPTVendorWinnerVM.FPTVendorParticipantID)) ? (m_dicDFPTVendorRecommendations[nameof(FPTVendorWinnerVM.FPTVendorParticipantID)] == null) ? null : m_dicDFPTVendorRecommendations[nameof(FPTVendorWinnerVM.FPTVendorParticipantID)].ToString() : null,
                                IsWinner = m_dicDFPTVendorRecommendations.ContainsKey(nameof(FPTVendorWinnerVM.IsWinner)) ? (m_dicDFPTVendorRecommendations[nameof(FPTVendorWinnerVM.IsWinner)] == null) ? false : (bool)m_dicDFPTVendorRecommendations[nameof(FPTVendorWinnerVM.IsWinner)] : false,
                                NegotiationEntryID = m_dicDFPTVendorRecommendations.ContainsKey(nameof(FPTVendorWinnerVM.NegotiationEntryID)) ? (m_dicDFPTVendorRecommendations[nameof(FPTVendorWinnerVM.NegotiationEntryID)] == null) ? null : m_dicDFPTVendorRecommendations[nameof(FPTVendorWinnerVM.NegotiationEntryID)].ToString() : null,
                                BidFee = m_dicDFPTVendorRecommendations.ContainsKey(nameof(FPTVendorWinnerVM.BidFee)) ? (m_dicDFPTVendorRecommendations[nameof(FPTVendorWinnerVM.BidFee)] == null) ? 0 : Convert.ToDecimal(m_dicDFPTVendorRecommendations[nameof(FPTVendorWinnerVM.BidFee)].ToString()) : 0,

                            }).ToList();
            List<string> m_lstMessage = new List<string>();

            if (!isSaveValid(m_lstDFPTVendorWinner, ref m_lstMessage))
            {
                Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
                return this.Direct(true);
            }

            DFPTVendorWinnerDA m_objDFPTVendorWinnerDA = new DFPTVendorWinnerDA();
            m_objDFPTVendorWinnerDA.ConnectionStringName = Global.ConnStrConfigName;
            string m_strTransName = "DFPTVendorWinnerDA";
            object m_objDBConnection = null;
            m_objDBConnection = m_objDFPTVendorWinnerDA.BeginTrans(m_strTransName);
            string m_strMessage = string.Empty;

            try
            {
                //DELETE
                Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.In);
                m_lstFilter.Add(string.Join(",", m_lstDFPTVendorWinner.Select(x => x.FPTVendorParticipantID).ToArray()));
                m_objFilter.Add(FPTVendorWinnerVM.Prop.FPTVendorParticipantID.Map, m_lstFilter);
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(FPTID);
                m_objFilter.Add(FPTVendorWinnerVM.Prop.FPTID.Map, m_lstFilter);
                //m_lstFilter = new List<object>();
                //m_lstFilter.Add(Operator.Equals);
                //m_lstFilter.Add(TCMemberID);
                //m_objFilter.Add(FPTVendorRecommendationsVM.Prop.TCMemberID.Map, m_lstFilter);


                m_objDFPTVendorWinnerDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                if (m_objDFPTVendorWinnerDA.Message == General.EnumDesc(MessageLib.NotExist)) m_objDFPTVendorWinnerDA.Message = "";

                //INSERT
                foreach (DFPTVendorWinner m_DFPTVendorWinner in m_lstDFPTVendorWinner)
                {
                    m_objDFPTVendorWinnerDA.Data = m_DFPTVendorWinner;
                    m_objDFPTVendorWinnerDA.Insert(true, m_objDBConnection);
                }
                if (!m_objDFPTVendorWinnerDA.Success || m_objDFPTVendorWinnerDA.Message != string.Empty)
                {
                    m_lstMessage.Add(m_objDFPTVendorWinnerDA.Message);
                }

            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                m_objDFPTVendorWinnerDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
            }

            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Updated));
                m_objDFPTVendorWinnerDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
                return this.Direct();
            }
            m_objDFPTVendorWinnerDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);

        }

        public ActionResult SaveAttend()
        {
            Dictionary<string, object>[] m_arrList = JSON.Deserialize<Dictionary<string, object>[]>(Request.Params["GridList"]);
            string FPTID = Request.Params["FPTID"].ToString();
            List<TFPTAttendances> m_lstTFPTAttendances = new List<TFPTAttendances>();

            m_lstTFPTAttendances = (
                            from Dictionary<string, object> m_dicm_lstTFPTAttendances in m_arrList
                            select new TFPTAttendances()
                            {
                                FPTAttendanceID = Guid.NewGuid().ToString("N"),
                                FPTID = FPTID,
                                AttendeeType = (m_dicm_lstTFPTAttendances[nameof(FPTAttendancesVM.AttendeeType)] == null) ? null : m_dicm_lstTFPTAttendances[nameof(FPTAttendancesVM.AttendeeType)].ToString(),
                                IDAttendee = (m_dicm_lstTFPTAttendances[nameof(FPTAttendancesVM.IDAttendee)] == null) ? null : m_dicm_lstTFPTAttendances[nameof(FPTAttendancesVM.IDAttendee)].ToString(),
                                IsAttend = (m_dicm_lstTFPTAttendances[nameof(FPTAttendancesVM.IsAttend)] == null) ? false : (bool)m_dicm_lstTFPTAttendances[nameof(FPTAttendancesVM.IsAttend)],
                                AttendanceDesc = (m_dicm_lstTFPTAttendances[nameof(FPTAttendancesVM.AttendanceDesc)] == null) ? null : m_dicm_lstTFPTAttendances[nameof(FPTAttendancesVM.AttendanceDesc)].ToString(),
                            }).ToList();
            List<string> m_lstMessage = new List<string>();

            if (!isSaveValidAtt(m_lstTFPTAttendances, ref m_lstMessage))//todo
            {
                Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
                return this.Direct(true);
            }

            TFPTAttendancesDA m_objTFPTAttendancesDA = new TFPTAttendancesDA();
            m_objTFPTAttendancesDA.ConnectionStringName = Global.ConnStrConfigName;
            string m_strTransName = "TFPTAttendancesDA";
            object m_objDBConnection = null;
            m_objDBConnection = m_objTFPTAttendancesDA.BeginTrans(m_strTransName);
            string m_strMessage = string.Empty;

            try
            {
                //DELETE
                Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                List<object> m_lstFilter = new List<object>();
                
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(FPTID);
                m_objFilter.Add(FPTAttendancesVM.Prop.FPTID.Map, m_lstFilter);

                m_objTFPTAttendancesDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                if (m_objTFPTAttendancesDA.Message == General.EnumDesc(MessageLib.NotExist)) m_objTFPTAttendancesDA.Message = "";

                //INSERT
                foreach (TFPTAttendances m_TFPTAttendances in m_lstTFPTAttendances)
                {
                    m_objTFPTAttendancesDA.Data = m_TFPTAttendances;
                    m_objTFPTAttendancesDA.Insert(true, m_objDBConnection);
                }
                if (!m_objTFPTAttendancesDA.Success || m_objTFPTAttendancesDA.Message != string.Empty)
                {
                    m_lstMessage.Add(m_objTFPTAttendancesDA.Message);
                }

            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                m_objTFPTAttendancesDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
            }

            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Updated));
                m_objTFPTAttendancesDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
                return this.Direct();
            }
            m_objTFPTAttendancesDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);
        }
        public ActionResult Verify(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Verify)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            List<FPTNegotiationRoundVM> m_lstSelectedRow = new List<FPTNegotiationRoundVM>();
            m_lstSelectedRow = JSON.Deserialize<List<FPTNegotiationRoundVM>>(Selected);
            DFPTVendorWinnerDA m_objDFPTVendorWinnerDA = new DFPTVendorWinnerDA();
            MTasksDA m_objMTasksDA = new MTasksDA();
            DTaskDetailsDA m_objDTaskDetailsDA = new DTaskDetailsDA();
            List<FPTVendorWinnerVM> m_lstFPTVendorWinnerVM = getFPTVendorWinnerVM(m_lstSelectedRow.FirstOrDefault().FPTID);
            if (!isFPTValid(m_lstFPTVendorWinnerVM, m_lstSelectedRow.FirstOrDefault().FPTID, ref m_lstMessage))
            {
                return this.Direct(false, String.Join(Global.NewLineSeparated, m_lstMessage));
            }
            m_objDFPTVendorWinnerDA.ConnectionStringName = Global.ConnStrConfigName;
            string m_strTransName = "DFPTVendorWinner";
            object m_objDBConnection = null;
            m_objDBConnection = m_objDFPTVendorWinnerDA.BeginTrans(m_strTransName);
            string m_strFPTID = string.Empty;

            //insert project & BP upload
            var m_lstconfig = getNegoConfig(m_lstSelectedRow.FirstOrDefault().FPTID);
            if (m_lstFPTVendorWinnerVM.Where(x => string.IsNullOrEmpty(x.BudgetPlanName)).Any())
            {
                foreach (var item in m_lstFPTVendorWinnerVM)
                {
                    item.BudgetPlanName = m_lstconfig.Where(y => y.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.Round)).FirstOrDefault().ParameterValue2;
                    item.ProjectName = m_lstconfig.Where(y => y.NegotiationConfigTypeID == General.EnumDesc(NegoConfigTypes.RoundTime)).FirstOrDefault().ParameterValue2;
                }
            }

            List<NegotiationBidEntryVM> m_lstNegotiationBidEntryVM = getNegotiationBidEntryVM(m_lstSelectedRow.FirstOrDefault().FPTID);
            string m_strSummary = "FPT " + m_lstSelectedRow.FirstOrDefault().FPTID;
            //winner
            m_strSummary += "|Vendor Winner : |";

            foreach (var bpname in m_lstFPTVendorWinnerVM.Select(x => x.BudgetPlanName).ToArray().Distinct())
            {
                m_strSummary += "|" + bpname + ": |";
                //rab
                decimal m_dectotal = m_lstNegotiationBidEntryVM.Where(x => x.Sequence == 7777 && x.BudgetPlanID == m_lstFPTVendorWinnerVM.Where(o => o.BudgetPlanName == bpname).FirstOrDefault().BudgetPlanID).FirstOrDefault().BudgetPlanDefaultValue;
                decimal m_decfee = m_lstNegotiationBidEntryVM.Where(x => x.Sequence == 8888 && x.BudgetPlanID == m_lstFPTVendorWinnerVM.Where(o => o.BudgetPlanName == bpname).FirstOrDefault().BudgetPlanID).FirstOrDefault().BudgetPlanDefaultValue;
                m_strSummary += "|RAB : " + (m_dectotal * (1 + (m_decfee / 100))).ToString(Global.DefaultNumberFormat) + "|";

                foreach (var vwinner in m_lstFPTVendorWinnerVM.Where(x => x.IsWinner == true && x.BudgetPlanName == bpname))
                {
                    decimal m_decwin = vwinner.BidValue * (1 + (vwinner.BidFee / 100));
                    m_strSummary += "-" + vwinner.VendorName + " (" + m_decwin.ToString(Global.DefaultNumberFormat) + ") |";
                }
            }

            m_strSummary += "|Vendor Participant : | ";
            foreach (var bpname in m_lstFPTVendorWinnerVM.Select(x => x.BudgetPlanName).ToArray().Distinct())
            {
                m_strSummary += bpname + ": |";
                foreach (var vwinner in m_lstFPTVendorWinnerVM.Where(x => x.BudgetPlanName == bpname && x.IsWinner != true))
                {
                    decimal m_decwin = vwinner.BidValue * (1 + (vwinner.BidFee / 100));
                    m_strSummary += "-" + vwinner.VendorName + " (" + m_decwin.ToString(Global.DefaultNumberFormat) + ") |";
                }
            }

            //m_strSummary += string.Join("$", m_lstFPTVendorWinnerVM.Where(x => x.IsWinner == true).Select(y => y.VendorName).ToArray());
            //m_strSummary += "|";
            //m_strSummary += string.Join("$", m_lstFPTVendorWinnerVM.Select(y => y.VendorName).ToArray());
            try
            {
                m_strFPTID = m_lstSelectedRow.FirstOrDefault().FPTID;
                string m_strTaskID = "";
                string m_strowner = "";
                string CurrentApprovalRole = GetCurrentApproval(General.EnumDesc(Enum.TaskType.VendorWinner), 0);
                //Insert MTasks
                m_objMTasksDA.ConnectionStringName = Global.ConnStrConfigName;
                MTasks m_objMTasks = new MTasks()
                {
                    TaskTypeID = General.EnumDesc(TaskType.VendorWinner),
                    TaskDateTimeStamp = DateTime.Now,
                    TaskOwnerID = GetParentApproval(ref m_strowner, CurrentApprovalRole, General.EnumDesc(Enum.TaskType.VendorWinner)),
                    StatusID = 0,
                    CurrentApprovalLvl = 1,
                    Remarks = "Vendor Winner",
                    Summary = m_strSummary
                };

                if (!string.IsNullOrEmpty(m_strowner)) m_lstMessage.Add(m_strowner);

                m_objMTasksDA.Data = m_objMTasks;
                //m_objMTasksDA.Select();//todo:
                m_objMTasksDA.Insert(true, m_objDBConnection);
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
                    StatusID = 5,
                    Remarks = "Vendor Winner",

                };
                m_objDTaskDetailsDA.Data = m_objDTaskDetails;
                m_objDTaskDetailsDA.Select();
                m_objDTaskDetailsDA.Insert(true, m_objDBConnection);
                if (!m_objDTaskDetailsDA.Success || m_objDTaskDetailsDA.Message != string.Empty)
                {
                    m_objDTaskDetailsDA.EndTrans(ref m_objDBConnection, m_strTransName);
                    return this.Direct(false, m_objDTaskDetailsDA.Message);
                }
                //Update DFPTVendorWinner
                Dictionary<string, List<object>> m_dicSet = new Dictionary<string, List<object>>();
                Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(m_strFPTID);
                m_objFilter.Add(FPTVendorWinnerVM.Prop.FPTID.Map, m_lstFilter);

                List<object> m_lstSet = new List<object>();
                m_lstSet.Add(typeof(string));
                m_lstSet.Add(m_strTaskID);
                m_dicSet.Add(FPTVendorWinnerVM.Prop.TaskID.Map, m_lstSet);
                m_objDFPTVendorWinnerDA.UpdateBC(m_dicSet, m_objFilter, true, m_objDBConnection);
                if (!m_objDFPTVendorWinnerDA.Success || m_objDTaskDetailsDA.Message != string.Empty)
                {
                    m_objDFPTVendorWinnerDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                    return this.Direct(false, m_objDFPTVendorWinnerDA.Message);
                }

                //SyncTRMPrice(m_strFPTID, ref m_lstMessage);
                //SyncNegoPrice(m_strFPTID, ref m_lstMessage);
                //SyncRecommendation(m_strFPTID, ref m_lstMessage);
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                return this.Direct(false, ex.Message);

            }
            if (m_lstMessage.Count <= 0)
            {
                string m_insmessage = string.Empty;
                InsertDFPTStatus(m_strFPTID, (int)FPTStatusTypes.SubmitVendorWinner, DateTime.Now, ref m_insmessage);
                Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Verified));
                m_objDFPTVendorWinnerDA.EndTrans(ref m_objDBConnection, true, m_strTransName);

                if (m_lstFPTVendorWinnerVM.FirstOrDefault().IsSync)
                {
                    SyncTRMPrice(m_strFPTID, ref m_lstMessage);
                    SyncNegoPrice(m_strFPTID, ref m_lstMessage);
                    SyncRecommendation(m_strFPTID, ref m_lstMessage);
                }

                return this.Direct(true, string.Empty);
            }
            else
            {
                m_objDFPTVendorWinnerDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                return this.Direct(false, String.Join(Global.NewLineSeparated, m_lstMessage));
            }

        }


        #endregion

        #region Direct Method

        #endregion

        #region Private Method

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
        private bool SyncRecommendation(string FPTID, ref List<string> message)
        {
            List<FPTVendorWinnerVM> m_lstFPTVendorWinnerVM = getFPTVendorWinnerVM(FPTID);
            List<NegotiationBidEntryVM> m_lstNegotiationBidEntryVM = getNegotiationBidEntryVM(FPTID);

            string m_vendorwinner = string.Empty;
            string m_tcremark = string.Empty;
            string m_listvendor = string.Empty;
            string m_listvendorprice = string.Empty;
            string m_listdesc = string.Empty;
            m_vendorwinner = string.Join(",", m_lstFPTVendorWinnerVM.Where(x => x.IsWinner == true).Select(x => x.VendorName).Distinct());
            foreach (var item in m_lstFPTVendorWinnerVM.Where(x => x.IsWinner == true))
            {
                m_listvendor += $" <string>{item.VendorName}</string> ";
                m_listvendorprice += $" <string>{item.BidValue * (1 + (item.BidFee / 100))}</string> ";
                m_listdesc += $" <string> </string> ";
            }

            List<FPTVendorRecommendationsVM> m_lstFPTVendorRecommendationsVM = getFPTVendorRecommendationsVM(FPTID);
            string m_lnum = m_lstFPTVendorRecommendationsVM.Any() ? m_lstFPTVendorRecommendationsVM.FirstOrDefault().LetterNumber : "";
            string m_letterno =  $"{m_lnum}/{ToRoman(m_lstFPTVendorRecommendationsVM.FirstOrDefault().CreatedDate.Month)}/{m_lstFPTVendorRecommendationsVM.FirstOrDefault().CreatedDate.ToString("yy")}";
            //string m_letterno = (m_lstFPTVendorWinnerVM.FirstOrDefault(x => x.IsWinner == true).ModifiedDate == null) ? "..." : $"{m_lstFPTVendorWinnerVM.FirstOrDefault(x => x.IsWinner == true).LetterNumber.ToString()}/{ToRoman(((DateTime)m_lstFPTVendorWinnerVM.FirstOrDefault(x => x.IsWinner == true).ModifiedDate).Month)}/{((DateTime)m_lstFPTVendorWinnerVM.FirstOrDefault(x => x.IsWinner == true).ModifiedDate).ToString("yy")}";
            //todo: change date format
            string m_soapenv = $@"<?xml version='1.0' encoding='utf-8'?>
            <soap:Envelope xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/'>
              <soap:Body>
                <UpdateRekomendasi xmlns='http://tempuri.org/'>
                  <prmRequestNo>{FPTID}</prmRequestNo>
                  <prmDtMeetingNego>{DateTime.Now.ToString("dd.MM.yyyy")}</prmDtMeetingNego>
                  <prmNoRekomendasi>{m_letterno}</prmNoRekomendasi>
                  <prmVendorDiTunjuk>{m_vendorwinner}</prmVendorDiTunjuk>
                  <prmdtNegoRemark>{m_tcremark}</prmdtNegoRemark>
                  <prmDtCreate>{DateTime.Now.ToString("dd.MM.yyyy")}</prmDtCreate>
                  <prmDtFinish>{DateTime.Now.ToString("dd.MM.yyyy")}</prmDtFinish>
                  <prmVendor>
                    {m_listvendor}
                  </prmVendor>
                  <prmHargaNego>
                    {m_listvendorprice}
                  </prmHargaNego>
                  <prmKeterangan>
                    {m_listdesc}
                  </prmKeterangan>
                  <prmUpdateOnly>0</prmUpdateOnly>
                </UpdateRekomendasi>
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

            XmlNodeList xnList = document.GetElementsByTagName("UpdateRekomendasiResult");

            foreach (XmlNode item in xnList)
            {
                if (item.Name == "UpdateRekomendasiResult" && item.InnerText == "")
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
        private List<NegotiationConfigurationsVM> GetListNegotiationConfigurationsVM(string FPTID)
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

            return m_lstNegotiationConfigurationsVM;
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
            m_lstSelect.Add(FPTVendorRecommendationsVM.Prop.TCMemberName.MapAlias);
            m_lstSelect.Add(FPTVendorRecommendationsVM.Prop.FPTVendorParticipantID.MapAlias);
            m_lstSelect.Add(FPTVendorRecommendationsVM.Prop.IsProposed.MapAlias);
            m_lstSelect.Add(FPTVendorRecommendationsVM.Prop.IsWinner.MapAlias);
            m_lstSelect.Add(FPTVendorRecommendationsVM.Prop.Remarks.MapAlias);
            m_lstSelect.Add(FPTVendorRecommendationsVM.Prop.LetterNumber.MapAlias);
            m_lstSelect.Add(FPTVendorRecommendationsVM.Prop.CreatedDate.MapAlias);

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
                    m_objFPTVendorRecommendationsVM.TCMemberName = item[FPTVendorRecommendationsVM.Prop.TCMemberName.Name].ToString();
                    m_objFPTVendorRecommendationsVM.FPTVendorParticipantID = item[FPTVendorRecommendationsVM.Prop.FPTVendorParticipantID.Name].ToString();
                    m_objFPTVendorRecommendationsVM.IsProposed = (bool)item[FPTVendorRecommendationsVM.Prop.IsProposed.Name];
                    m_objFPTVendorRecommendationsVM.IsWinner = (bool)item[FPTVendorRecommendationsVM.Prop.IsWinner.Name];
                    m_objFPTVendorRecommendationsVM.Remarks = item[FPTVendorRecommendationsVM.Prop.Remarks.Name].ToString();
                    m_objFPTVendorRecommendationsVM.LetterNumber = item[FPTVendorRecommendationsVM.Prop.LetterNumber.Name].ToString();
                    m_objFPTVendorRecommendationsVM.CreatedDate = (DateTime)item[FPTVendorRecommendationsVM.Prop.CreatedDate.Name];
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
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.NegotiationEntryID.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.BidValue.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.BidFee.MapAlias);

            m_lstSelect.Add(FPTVendorWinnerVM.Prop.VendorName.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.VendorAddress.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.VendorPhone.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.VendorEmail.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.BudgetPlanName.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.ProjectName.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.StatusID.MapAlias);
            m_lstSelect.Add(FPTVendorWinnerVM.Prop.IsSync.MapAlias);

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
                    m_objFPTVendorWinnerVM.VendorName = item[FPTVendorWinnerVM.Prop.VendorName.Name].ToString();
                    m_objFPTVendorWinnerVM.VendorAddress = item[FPTVendorWinnerVM.Prop.VendorAddress.Name].ToString();
                    m_objFPTVendorWinnerVM.VendorPhone = item[FPTVendorWinnerVM.Prop.VendorPhone.Name].ToString();
                    m_objFPTVendorWinnerVM.VendorEmail = item[FPTVendorWinnerVM.Prop.VendorEmail.Name].ToString();
                    m_objFPTVendorWinnerVM.BudgetPlanName = item[FPTVendorWinnerVM.Prop.BudgetPlanName.Name].ToString();
                    m_objFPTVendorWinnerVM.BudgetPlanID = item[FPTVendorWinnerVM.Prop.BudgetPlanID.Name].ToString();
                    m_objFPTVendorWinnerVM.ProjectName = item[FPTVendorWinnerVM.Prop.ProjectName.Name].ToString();
                    m_objFPTVendorWinnerVM.BidFee = (string.IsNullOrEmpty(item[FPTVendorWinnerVM.Prop.BidFee.Name].ToString())) ? 0 : (decimal)item[FPTVendorWinnerVM.Prop.BidFee.Name];
                    m_objFPTVendorWinnerVM.StatusID = (string.IsNullOrEmpty(item[FPTVendorWinnerVM.Prop.StatusID.Name].ToString())) ? 4 : (int)item[FPTVendorWinnerVM.Prop.StatusID.Name];
                    m_objFPTVendorWinnerVM.IsSync = (bool)item[FPTVendorWinnerVM.Prop.IsSync.Name];
                    m_lstFPTVendorWinnerVM.Add(m_objFPTVendorWinnerVM);
                }
            }
            return m_lstFPTVendorWinnerVM;
        }
        private List<FPTAttendancesVM> getFPTAttendancesVM(string FPTID)
        {
            List<FPTAttendancesVM> m_lstFPTAttendancesVM = new List<FPTAttendancesVM>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();
            TFPTAttendancesDA m_objTFPTAttendancesDA = new TFPTAttendancesDA();
            m_objTFPTAttendancesDA.ConnectionStringName = Global.ConnStrConfigName;

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(FPTID);
            m_objFilter.Add(FPTAttendancesVM.Prop.FPTID.Map, m_lstFilter);

            m_lstSelect = new List<string>();
            m_lstSelect.Add(FPTAttendancesVM.Prop.FPTAttendanceID.MapAlias);
            m_lstSelect.Add(FPTAttendancesVM.Prop.FPTID.MapAlias);
            m_lstSelect.Add(FPTAttendancesVM.Prop.AttendeeType.MapAlias);
            m_lstSelect.Add(FPTAttendancesVM.Prop.IDAttendee.MapAlias);
            m_lstSelect.Add(FPTAttendancesVM.Prop.IsAttend.MapAlias);
            m_lstSelect.Add(FPTAttendancesVM.Prop.AttendanceDesc.MapAlias);
            m_lstSelect.Add(FPTAttendancesVM.Prop.AttendanceName.MapAlias);


            Dictionary<int, DataSet> m_dicTFPTAttendancesDA = m_objTFPTAttendancesDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objTFPTAttendancesDA.Success)
            {
                foreach (DataRow item in m_dicTFPTAttendancesDA[0].Tables[0].Rows)
                {
                    FPTAttendancesVM m_objFPTAttendancesVM = new FPTAttendancesVM();

                    m_objFPTAttendancesVM.FPTAttendanceID = item[FPTAttendancesVM.Prop.FPTAttendanceID.Name].ToString();
                    m_objFPTAttendancesVM.FPTID = item[FPTAttendancesVM.Prop.FPTID.Name].ToString();
                    m_objFPTAttendancesVM.AttendeeType = item[FPTAttendancesVM.Prop.AttendeeType.Name].ToString();
                    m_objFPTAttendancesVM.IDAttendee = item[FPTAttendancesVM.Prop.IDAttendee.Name].ToString();
                    m_objFPTAttendancesVM.IsAttend = (bool)item[FPTAttendancesVM.Prop.IsAttend.Name];
                    m_objFPTAttendancesVM.AttendanceDesc = item[FPTAttendancesVM.Prop.AttendanceDesc.Name].ToString();
                    m_objFPTAttendancesVM.AttendanceName = item[FPTAttendancesVM.Prop.AttendanceName.Name].ToString();
                    m_lstFPTAttendancesVM.Add(m_objFPTAttendancesVM);
                }
            }
            
            if (!m_lstFPTAttendancesVM.Any())
            {
                string message = string.Empty;
                
                var m_lstemployee = getListEmployeeVM(FPTID);
                foreach (var item in m_lstemployee)
                {
                    FPTAttendancesVM attendancevm = new FPTAttendancesVM();
                    attendancevm.AttendeeType = item.AttendeeType;
                    attendancevm.IDAttendee = item.EmployeeID;
                    attendancevm.AttendanceName = item.EmployeeName;
                    attendancevm.IsAttend = item.IsAttend;
                    m_lstFPTAttendancesVM.Add(attendancevm);
                }
            }
            
            return m_lstFPTAttendancesVM;
        }

        private List<EmployeeVM> getListEmployeeVM(string FPTID)
        {
            MEmployeeDA m_objMEmployeeDA = new MEmployeeDA();
            m_objMEmployeeDA.ConnectionStringName = Global.ConnStrConfigName;

            

            FilterHeaderConditions m_fhcMEmployee = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(FPTID);
            m_objFilter.Add(EmployeeVM.Prop.FPTID.Name, m_lstFilter);

            List<EmployeeVM> m_lstEmployeeVM = new List<EmployeeVM>();
            
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add("AttendeeType");
            m_lstSelect.Add(EmployeeVM.Prop.EmployeeID.MapAlias);
            m_lstSelect.Add(EmployeeVM.Prop.EmployeeName.MapAlias);
            m_lstSelect.Add(EmployeeVM.Prop.FirstName.MapAlias);
            m_lstSelect.Add("MiddleName");
            m_lstSelect.Add(EmployeeVM.Prop.LastName.MapAlias);
            m_lstSelect.Add("FPTID");
            m_lstSelect.Add(EmployeeVM.Prop.IsAttend.MapAlias);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            
            Dictionary<int, DataSet> m_dicMEmployeeDA = m_objMEmployeeDA.SelectBCAtt(0, null, false, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
            if (m_objMEmployeeDA.Message == string.Empty)
            {
                m_lstEmployeeVM = (
                    from DataRow m_drMEmployeeDA in m_dicMEmployeeDA[0].Tables[0].Rows
                    select new EmployeeVM()
                    {
                        EmployeeName = m_drMEmployeeDA[EmployeeVM.Prop.EmployeeName.Name].ToString(),
                        AttendeeType = m_drMEmployeeDA[EmployeeVM.Prop.AttendeeType.Name].ToString(),
                        EmployeeID = m_drMEmployeeDA[EmployeeVM.Prop.EmployeeID.Name].ToString(),
                        FirstName = m_drMEmployeeDA[EmployeeVM.Prop.FirstName.Name].ToString(),
                        MiddleName = m_drMEmployeeDA[EmployeeVM.Prop.MiddleName.Name].ToString(),
                        LastName = m_drMEmployeeDA[EmployeeVM.Prop.LastName.Name].ToString(),
                        FPTID = m_drMEmployeeDA[EmployeeVM.Prop.FPTID.Name].ToString(),
                        IsAttend = bool.Parse(m_drMEmployeeDA[EmployeeVM.Prop.IsAttend.Name].ToString())
                    }
                ).ToList();
            }
            return m_lstEmployeeVM;

        }

        private List<FPTStatusVM> getFPTStatusVM(string FPTID)
        {
            List<FPTStatusVM> m_lstFPTStatusVM = new List<FPTStatusVM>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();
            DFPTStatusDA m_objDFPTStatusDA = new DFPTStatusDA();
            m_objDFPTStatusDA.ConnectionStringName = Global.ConnStrConfigName;

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(FPTID);
            m_objFilter.Add(FPTStatusVM.Prop.FPTID.Map, m_lstFilter);

            m_lstSelect = new List<string>();
            m_lstSelect.Add(FPTStatusVM.Prop.FPTStatusID.MapAlias);
            m_lstSelect.Add(FPTStatusVM.Prop.FPTID.MapAlias);
            m_lstSelect.Add(FPTStatusVM.Prop.StatusDateTimeStamp.MapAlias);
            m_lstSelect.Add(FPTStatusVM.Prop.StatusID.MapAlias);


            Dictionary<int, DataSet> m_dicDFPTStatusDA = m_objDFPTStatusDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objDFPTStatusDA.Success)
            {
                foreach (DataRow item in m_dicDFPTStatusDA[0].Tables[0].Rows)
                {
                    FPTStatusVM m_objFPTStatusVM = new FPTStatusVM();
                    m_objFPTStatusVM.FPTStatusID = item[FPTStatusVM.Prop.FPTStatusID.Name].ToString();
                    m_objFPTStatusVM.FPTID = item[FPTStatusVM.Prop.FPTID.Name].ToString();
                    m_objFPTStatusVM.StatusDateTimeStamp = (DateTime)item[FPTStatusVM.Prop.StatusDateTimeStamp.Name];
                    m_objFPTStatusVM.StatusID = (int)item[FPTStatusVM.Prop.StatusID.Name];
                    m_lstFPTStatusVM.Add(m_objFPTStatusVM);
                }
            }
            return m_lstFPTStatusVM;
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
                    m_objNegotiationBidEntryVM.BudgetPlanID = item[NegotiationBidEntryVM.Prop.BudgetPlanID.Name].ToString();
                    m_objNegotiationBidEntryVM.RoundID = item[NegotiationBidEntryVM.Prop.RoundID.Name].ToString();
                    m_lstNegotiationBidEntryVM.Add(m_objNegotiationBidEntryVM);
                }
            }

            return m_lstNegotiationBidEntryVM;
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
            m_ModelField = new ModelField(NegotiationBidEntryVM.Prop.NegotiationEntryID.Name);
            m_model.Fields.Add(m_ModelField);
            m_ModelField = new ModelField(nameof(FPTVendorParticipantsVM.TCName));
            m_model.Fields.Add(m_ModelField);
            m_ModelField = new ModelField(nameof(FPTVendorParticipantsVM.RecommendationRemark));
            m_model.Fields.Add(m_ModelField);
            m_ModelField = new ModelField(nameof(FPTVendorParticipantsVM.IsProposedWinner));
            m_model.Fields.Add(m_ModelField);
            m_ModelField = new ModelField(FPTVendorWinnerVM.Prop.IsWinner.Name);
            m_model.Fields.Add(m_ModelField);
            m_ModelField = new ModelField(nameof(FPTVendorParticipantsVM.BudgetPlanDefaultValue));
            m_model.Fields.Add(m_ModelField);
            m_ModelField = new ModelField(nameof(FPTVendorParticipantsVM.BudgetPlanDefaultValueAfterFee));
            m_model.Fields.Add(m_ModelField);
            m_ModelField = new ModelField(nameof(FPTVendorParticipantsVM.IsAttend));
            m_model.Fields.Add(m_ModelField);
            m_store.Model.Add(m_model);
            m_store.DataSource = ListNego.OrderBy(x => x.BidAfterFee).ToList();
            m_gridpanel.Store.Add(m_store);

            //Columns
            List<ColumnBase> m_ListColumn = new List<ColumnBase>();
            Checkbox m_objCheckbox = new Checkbox();
            ColumnBase m_objColumn = new Ext.Net.Column();
            m_objColumn = new Ext.Net.Column { Text = FPTVendorParticipantsVM.Prop.FPTVendorParticipantID.Desc, DataIndex = "FPTVendorParticipantID", Flex = 1, Hidden = true };
            m_ListColumn.Add(m_objColumn);
            m_objColumn = new Ext.Net.Column { Text = "NegotiationEntryID", DataIndex = "NegotiationEntryID", Flex = 1, Hidden = true };
            m_ListColumn.Add(m_objColumn);
            m_objColumn = new Ext.Net.Column { Text = FPTVendorParticipantsVM.Prop.VendorName.Desc, DataIndex = "VendorName", Flex = 2 };
            m_ListColumn.Add(m_objColumn);
            m_objColumn = new Ext.Net.Column { Text = "Fee", DataIndex = "BidFee", Flex = 1 };
            m_ListColumn.Add(m_objColumn);
            m_objColumn = new Ext.Net.Column { Text = "Bid", DataIndex = "BidAfterFee", Flex = 1 };
            m_objColumn.Renderer = new Renderer("renderGridColumnAfterFee");
            m_ListColumn.Add(m_objColumn);
            //m_objColumn = new Ext.Net.Column { Text = "Bid", DataIndex = "BidValue", Flex = 1 };
            //m_objColumn.Renderer = new Renderer("renderGridColumn");
            //m_ListColumn.Add(m_objColumn);
            m_objColumn = new Ext.Net.Column { Text = "Is Attend", DataIndex = "IsAttend", Flex = 1, Align = ColumnAlign.End };
            m_objColumn.Renderer = new Renderer("RendererBR");
            m_ListColumn.Add(m_objColumn);
            m_objColumn = new Ext.Net.Column { Text = "TC Name", DataIndex = "TCName", Flex = 2 };
            m_objColumn.Renderer = new Renderer("RendererBR");
            m_ListColumn.Add(m_objColumn);
            m_objColumn = new Ext.Net.Column { Text = "Recommendation Remark", DataIndex = "RecommendationRemark", Flex = 2 };
            m_objColumn.Renderer = new Renderer("RendererBR");
            m_ListColumn.Add(m_objColumn);
            m_objColumn = new Ext.Net.Column { Text = "Is Proposed", DataIndex = "IsProposedWinner", Flex = 2 };
            m_objColumn.Renderer = new Renderer("RendererBR");
            m_ListColumn.Add(m_objColumn);
            m_objColumn = new CheckColumn { Text = "Is Winner", DataIndex = "IsWinner", Flex = 1, Editable = true };
            m_ListColumn.Add(m_objColumn);
            m_gridpanel.ColumnModel.Columns.AddRange(m_ListColumn);

            //Plugin
            CellEditing m_objCellEditing = new CellEditing() { ClicksToEdit = 1 };
            m_gridpanel.Plugins.Add(m_objCellEditing);



            return m_gridpanel;
        }
        private bool isSaveValid(List<DFPTVendorWinner> m_lstDFPTVendorWinner, ref List<string> message)
        {
            //todo: validation
            bool m_boolretVal = true;

            return m_boolretVal;
        }

        private bool isSaveValidAtt(List<TFPTAttendances> m_lstTFPTAttendances, ref List<string> message)
        {
            //todo: validation
            bool m_boolretVal = true;
            foreach (var item in m_lstTFPTAttendances)
            {
                if(string.IsNullOrEmpty(item.IDAttendee) || string.IsNullOrEmpty(item.AttendanceDesc))
                {
                    message.Add("There is empty Attendance");
                    return false;
                }
            }
            return m_boolretVal;
        }

        private bool isFPTValid(List<FPTVendorWinnerVM> m_lstFPTVendorWinnerVM, string FPTID, ref List<string> message)
        {
            bool m_boolretVal = true;
            List<FPTStatusVM> m_lstFPTStatusVM = getFPTStatusVM(FPTID);
            foreach (var item in m_lstFPTStatusVM.OrderByDescending(x => x.StatusDateTimeStamp))
            {
                if (item.StatusID == (int)FPTStatusTypes.FPTUnverified || item.StatusID == (int)FPTStatusTypes.FPTVerified)
                {
                    if (item.StatusID == (int)FPTStatusTypes.FPTUnverified)
                    {
                        message.Add("FPT Unverified!");
                        return false;
                    }
                    if (item.StatusID == (int)FPTStatusTypes.FPTVerified)
                    {
                        break;
                    }
                }
            }
            if (!m_lstFPTVendorWinnerVM.Where(x => x.IsWinner == true).Any())
            {
                message.Add("No Winner Selected!");
                return false;
            }
            if (!getFPTAttendancesVM(FPTID).Any())
            {
                message.Add("Attendances List is Empty!");
                return false;
            }
            //if (!m_lstFPTVendorWinnerVM.Where(x => string.IsNullOrEmpty(x.TaskID)).Any())
            //{
            //    message.Add("Already Verified!");
            //    return false;
            //}
            //todo: check if already verified
            return m_boolretVal;
        }
        private Dictionary<string, object> GetFormData(NameValueCollection parameters, string FPTID = "")
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            //todo:
            m_dicReturn.Add(FPTNegotiationRoundVM.Prop.FPTID.Name, parameters[FPTNegotiationRoundVM.Prop.FPTID.Name]);
            m_dicReturn.Add(FPTNegotiationRoundVM.Prop.RoundID.Name, parameters[FPTNegotiationRoundVM.Prop.RoundID.Name]);

            return m_dicReturn;
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

        #endregion

    }
}