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
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using System.IO;
using NPOI.SS.UserModel;

namespace com.SML.BIGTRONS.Controllers
{
    public class ReportComparisonController : BaseController
    {
        private readonly string title = "Report";
        private readonly string dataSessionName = "FormData";

        #region Public Action

        public ActionResult Index()
        {
            base.Initialize();
            return View("~/Views/Report/Comparison/Index.cshtml");
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
                ViewName = "~/Views/Report/Comparison/_List.cshtml",
                WrapByScriptTag = false
            };
        }

        public ActionResult Read(StoreRequestParameters parameters)
        {
            TPackageDA m_objSPackageDA = new TPackageDA();
            m_objSPackageDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMPackage = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();

            //string m_strFilterBudgetPlan = "";

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMPackage.Conditions)
            {
                //if (m_fhcFilter.DataIndex == "BudgetPlanListDesc")
                //{
                //    m_strFilterBudgetPlan = Global.GetFilterConditionValue(m_fhcFilter).ToString();
                //    continue;
                //}
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = PackageVM.Prop.Map(m_strDataIndex, false);
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
                                    m_objEnd = m_fhcFilter.Type.ToString() == "Date" ? m_kvpFilterDetail.Value.ToString() + " 23:59:59" : m_kvpFilterDetail.Value;
                                    break;
                                case Global.OpGreaterThan:
                                case Global.OpGreaterThanEqual:
                                    m_objStart = m_fhcFilter.Type.ToString() == "Date" ? m_kvpFilterDetail.Value.ToString() + " 00:00:00" : m_kvpFilterDetail.Value;
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
            List<string> m_lstuserproject = new List<string>();
            string m_strdivisionid = getCurentUser().DivisionID == null ? string.Empty : getCurentUser().DivisionID;
            List<ProjectVM> m_lstproject = GetProjectList(m_strdivisionid);
            if (m_lstproject.Any())
            {
                m_lstuserproject = m_lstproject.Select(x => x.ProjectID).ToList();
            }
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(String.Join(",", m_lstuserproject));
            m_objFilter.Add(PackageVM.Prop.PackageProjectID.Map, m_lstFilter);
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(General.EnumDesc(BudgetPlanVersionStatus.Approved));
            m_objFilter.Add(PackageVM.Prop.StatusID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicSPackageDA = m_objSPackageDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpPackageBL in m_dicSPackageDA)
            {
                m_intCount = m_kvpPackageBL.Key;
                break;
            }

            List<PackageVM> m_lstPackageVM = new List<PackageVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(PackageVM.Prop.PackageID.MapAlias);
                m_lstSelect.Add(PackageVM.Prop.PackageDesc.MapAlias);
                m_lstSelect.Add(PackageVM.Prop.StatusID.MapAlias);
                m_lstSelect.Add(PackageVM.Prop.StatusDesc.MapAlias);
                m_lstSelect.Add(PackageVM.Prop.CreatedDate.MapAlias);
                m_lstSelect.Add(PackageVM.Prop.ModifiedDate.MapAlias);
                m_lstSelect.Add(PackageVM.Prop.BudgetPlanListDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(PackageVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicSPackageDA = m_objSPackageDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objSPackageDA.Message == string.Empty)
                {
                    m_lstPackageVM = (
                        from DataRow m_drSPackageDA in m_dicSPackageDA[0].Tables[0].Rows
                        select new PackageVM()
                        {
                            PackageID = m_drSPackageDA[PackageVM.Prop.PackageID.Name].ToString(),
                            PackageDesc = m_drSPackageDA[PackageVM.Prop.PackageDesc.Name].ToString(),
                            StatusID = int.Parse(m_drSPackageDA[PackageVM.Prop.StatusID.Name].ToString()),
                            StatusDesc = m_drSPackageDA[PackageVM.Prop.StatusDesc.Name].ToString(),
                            CreatedDate = (DateTime?)m_drSPackageDA[PackageVM.Prop.CreatedDate.Name],
                            ModifiedDate = (DateTime?)m_drSPackageDA[PackageVM.Prop.ModifiedDate.Name],
                            BudgetPlanListDesc = m_drSPackageDA[PackageVM.Prop.BudgetPlanListDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }

            //if (m_strFilterBudgetPlan.Length > 0)
            //{
            //    return this.Store(m_lstPackageVM.Where(m => m.BudgetPlanListDesc.IndexOf(m_strFilterBudgetPlan) >= 0).ToList<PackageVM>(), m_intCount);
            //}
            return this.Store(m_lstPackageVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            TPackageDA m_objSPackageDA = new TPackageDA();
            m_objSPackageDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMPackage = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMPackage.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = PackageVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicSPackageDA = m_objSPackageDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpPackageBL in m_dicSPackageDA)
            {
                m_intCount = m_kvpPackageBL.Key;
                break;
            }

            List<PackageVM> m_lstPackageVM = new List<PackageVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(PackageVM.Prop.PackageID.MapAlias);
                m_lstSelect.Add(PackageVM.Prop.PackageDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(PackageVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicSPackageDA = m_objSPackageDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objSPackageDA.Message == string.Empty)
                {
                    m_lstPackageVM = (
                        from DataRow m_drSPackageDA in m_dicSPackageDA[0].Tables[0].Rows
                        select new PackageVM()
                        {
                            PackageID = m_drSPackageDA[PackageVM.Prop.PackageID.Name].ToString(),
                            PackageDesc = m_drSPackageDA[PackageVM.Prop.PackageDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstPackageVM, m_intCount);
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

            PackageVM m_objPackageVM = new PackageVM();
            m_objPackageVM.StatusDesc = "";
            m_objPackageVM.BudgetPlanList = new List<PackageListVM>();
            ViewDataDictionary m_vddPackage = new ViewDataDictionary();
            m_vddPackage.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddPackage.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
            string m_strdivisionid = getCurentUser().DivisionID == null ? string.Empty : getCurentUser().DivisionID;
            m_vddPackage.Add("divisionid", m_strdivisionid);

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
                Model = m_objPackageVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddPackage,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Detail(string Caller, string Selected, string PackageID = "")
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            PackageVM m_objPackageVM = new PackageVM();
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
                m_dicSelectedRow = GetFormData(m_nvcParams, PackageID);
            }


            if (m_dicSelectedRow.Count > 0)
                m_objPackageVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
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
                Model = m_objPackageVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddWorkCenter,
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

            if (m_dicSelectedRow["StatusID"].ToString() != "0")
            {
                Global.ShowErrorAlert(title, "Status is " + MessageLib.invalid);
                return this.Direct();
            }

            string m_strMessage = string.Empty;
            PackageVM m_objPackageVM = new PackageVM();
            if (m_dicSelectedRow.Count > 0)
                m_objPackageVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddPackage = new ViewDataDictionary();
            m_vddPackage.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddPackage.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            string m_strdivisionid = getCurentUser().DivisionID == null ? string.Empty : getCurentUser().DivisionID;
            m_vddPackage.Add("divisionid", m_strdivisionid);
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objPackageVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddPackage,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<PackageVM> m_lstSelectedRow = new List<PackageVM>();
            m_lstSelectedRow = JSON.Deserialize<List<PackageVM>>(Selected);

            TPackageDA m_objTPackageDA = new TPackageDA();
            m_objTPackageDA.ConnectionStringName = Global.ConnStrConfigName;
            string m_strTransaction = "BudgetPlanPackageDelete";
            object m_objConnection = m_objTPackageDA.BeginTrans(m_strTransaction);

            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (PackageVM m_objPackageVM in m_lstSelectedRow)
                {
                    m_objTPackageDA.Data.PackageID = m_objPackageVM.PackageID;
                    m_objTPackageDA.Select();
                    if (m_objPackageVM.StatusID > 0)
                    {
                        m_lstMessage.Add("Package [" + m_objTPackageDA.Data.PackageID.ToString() + "] Status is " + MessageLib.invalid);
                        break;
                    }
                    else
                    {
                        m_objTPackageDA.Data.StatusID = 99;
                        m_objTPackageDA.Update(true, m_objConnection);
                        if (m_objTPackageDA.Success)
                        {
                            m_objPackageVM.BudgetPlanList = GetPackageList(m_objPackageVM.PackageID);
                            foreach (PackageListVM m_objPackageListVM in m_objPackageVM.BudgetPlanList)
                            {
                                DBudgetPlanVersionDA m_objDBudgetPlanVersionDA = new DBudgetPlanVersionDA();
                                m_objDBudgetPlanVersionDA.ConnectionStringName = Global.ConnStrConfigName;

                                m_objDBudgetPlanVersionDA.Data.BudgetPlanID = m_objPackageListVM.BudgetPlanID;
                                m_objDBudgetPlanVersionDA.Data.BudgetPlanVersion = m_objPackageListVM.BudgetPlanVersion;
                                m_objDBudgetPlanVersionDA.Select();

                                m_objDBudgetPlanVersionDA.Data.StatusID = 0;
                                m_objDBudgetPlanVersionDA.Update(true, m_objConnection);
                            }
                        }
                        else
                        {
                            m_lstMessage.Add("Package [" + m_objTPackageDA.Data.StatusID.ToString() + "] " + m_objTPackageDA.Message);
                            break;
                        }
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


            if (m_lstMessage.Count > 0)
            {
                m_objTPackageDA.EndTrans(ref m_objConnection, false, m_strTransaction);
            }
            else
            {
                m_objTPackageDA.EndTrans(ref m_objConnection, true, m_strTransaction);
            }

            return this.Direct();
        }

        public ActionResult Browse(string ControlPackageID, string ControlPackageDesc, string FilterPackageID = "", string FilterPackageDesc = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddPackage = new ViewDataDictionary();
            m_vddPackage.Add("Control" + PackageVM.Prop.PackageID.Name, ControlPackageID);
            m_vddPackage.Add("Control" + PackageVM.Prop.PackageDesc.Name, ControlPackageDesc);
            m_vddPackage.Add(PackageVM.Prop.PackageID.Name, FilterPackageID);
            m_vddPackage.Add(PackageVM.Prop.PackageDesc.Name, FilterPackageDesc);

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddPackage,
                ViewName = "../Package/_Browse"
            };
        }

        public ActionResult Save(string Action, string BudgetPlanList)
        {
            //action = JSON.Deserialize<string>(action);
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            string m_strPackageID = this.Request.Params[PackageVM.Prop.PackageID.Name];

            List<string> m_lstMessage = new List<string>();
            TPackageDA m_objTPackageDA = new TPackageDA();
            m_objTPackageDA.ConnectionStringName = Global.ConnStrConfigName;

            string m_strTransName = "BudgetPlanPackage";
            object m_objConnection;

            m_objConnection = m_objTPackageDA.BeginTrans(m_strTransName);
            try
            {
                string m_strPackageDesc = this.Request.Params[PackageVM.Prop.PackageDesc.Name];
                string m_strProjectID = this.Request.Params[PackageVM.Prop.ProjectID.Name];
                string m_strCompanyID = this.Request.Params[PackageVM.Prop.CompanyID.Name];

                Dictionary<string, object>[] m_arrBudgetPlanList = JSON.Deserialize<Dictionary<string, object>[]>(BudgetPlanList);
                if (m_arrBudgetPlanList == null)
                    m_arrBudgetPlanList = new List<Dictionary<string, object>>().ToArray();

                List<PackageListVM> m_lstPackageListVM = new List<PackageListVM>();
                m_lstPackageListVM = (
                    from Dictionary<string, object> m_dicPackageList in m_arrBudgetPlanList
                    select new PackageListVM()
                    {
                        PackageID = m_objTPackageDA.Data.PackageID,
                        BudgetPlanID = m_dicPackageList[PackageListVM.Prop.BudgetPlanID.Name].ToString(),
                        BudgetPlanVersion = int.Parse(m_dicPackageList[PackageListVM.Prop.BudgetPlanVersion.Name].ToString())
                    }).ToList();

                m_lstMessage = IsSaveValid(Action, m_strPackageID, m_strPackageDesc, m_lstPackageListVM);
                if (m_lstMessage.Count <= 0)
                {
                    TPackage m_objTPackage = new TPackage();
                    m_objTPackage.PackageID = m_strPackageID;
                    m_objTPackageDA.Data = m_objTPackage;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objTPackageDA.Select();

                    m_objTPackage.PackageDesc = m_strPackageDesc;
                    m_objTPackage.CompanyID = m_strCompanyID;
                    m_objTPackage.ProjectID = m_strProjectID;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objTPackageDA.Insert(true, m_objConnection);
                    else
                        m_objTPackageDA.Update(true, m_objConnection);

                    if (!m_objTPackageDA.Success || m_objTPackageDA.Message != string.Empty)
                        m_lstMessage.Add(m_objTPackageDA.Message);

                    m_strPackageID = m_objTPackage.PackageID;

                    if (m_lstMessage.Count == 0)
                    {
                        DPackageListDA m_objDPackageListDA = new DPackageListDA();

                        Dictionary<string, List<object>> m_objFilterPackageList = new Dictionary<string, List<object>>();
                        List<object> m_lstFilter = new List<object>();
                        m_lstFilter.Add(Operator.Equals);
                        m_lstFilter.Add(m_objTPackageDA.Data.PackageID);
                        m_objFilterPackageList.Add(PackageListVM.Prop.PackageID.Map, m_lstFilter);

                        m_objDPackageListDA.DeleteBC(m_objFilterPackageList, true, m_objConnection);

                        foreach (PackageListVM item in m_lstPackageListVM)
                        {
                            m_objDPackageListDA.Data.PackageID = m_objTPackage.PackageID;
                            m_objDPackageListDA.Data.BudgetPlanID = item.BudgetPlanID;
                            m_objDPackageListDA.Data.BudgetPlanVersion = item.BudgetPlanVersion;

                            m_objDPackageListDA.Insert(true, m_objConnection);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
            }
            if (m_lstMessage.Count <= 0)
            {
                m_objTPackageDA.EndTrans(ref m_objConnection, true, m_strTransName);
                Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
                return Detail(General.EnumDesc(Buttons.ButtonSave), null, m_strPackageID);
            }
            m_objTPackageDA.EndTrans(ref m_objConnection, false, m_strTransName);
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);
        }
        public ActionResult ClearExcelFile(string filename)
        {
            string fullPath = Request.MapPath("~/Content/" + filename);
            //if (System.IO.File.Exists(fullPath))
            //    System.IO.File.Delete(fullPath);
            return this.Direct();
        }
        public ActionResult ExportReportToExcel(string BudgetPlanPackageID,string Descriptions, string GridStructure, string VendorCount, string BudgetPlanID)
        {


            #region Get List Package
            DPackageListDA m_objDPackageDA = new DPackageListDA();
            m_objDPackageDA.ConnectionStringName = Global.ConnStrConfigName;
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            List<string> m_lstSelect = new List<string>();
            #region Get PackageID
            if (!string.IsNullOrEmpty(BudgetPlanID))
            {
                m_objFilter = new Dictionary<string, List<object>>();
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(BudgetPlanID);
                m_objFilter.Add(PackageListVM.Prop.BudgetPlanID.Map, m_lstFilter);
                m_lstSelect = new List<string>();
                m_lstSelect.Add(PackageListVM.Prop.PackageID.MapAlias);
                m_lstSelect.Add(PackageListVM.Prop.BudgetPlanID.MapAlias);
                m_lstSelect.Add(PackageListVM.Prop.Description.MapAlias);
                Dictionary<int, DataSet> m_dicSPackageDAs = m_objDPackageDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);
                if (m_objDPackageDA.Message == string.Empty)
                {
                    Descriptions = m_dicSPackageDAs[0].Tables[0].Rows[0][PackageListVM.Prop.Description.Name].ToString();
                    BudgetPlanPackageID = m_dicSPackageDAs[0].Tables[0].Rows[0][PackageListVM.Prop.PackageID.Name].ToString();
                }
            }
            #endregion

            List<PackageListVM> m_lstPackageVM = new List<PackageListVM>();
            if (!string.IsNullOrEmpty(BudgetPlanPackageID))
            {               
                m_objFilter = new Dictionary<string, List<object>>();
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);                
                m_lstFilter.Add(BudgetPlanPackageID);
                m_objFilter.Add(PackageListVM.Prop.PackageID.Map, m_lstFilter);   

                m_lstSelect = new List<string>();
                m_lstSelect.Add(PackageListVM.Prop.PackageID.MapAlias);
                m_lstSelect.Add(PackageListVM.Prop.BudgetPlanID.MapAlias);
                m_lstSelect.Add(PackageListVM.Prop.MaxVersion.MapAlias);
                m_lstSelect.Add(PackageListVM.Prop.UnitTypeDesc.MapAlias);
                m_lstSelect.Add(PackageListVM.Prop.LastApprovedVersionArea.MapAlias);
                m_lstSelect.Add(PackageListVM.Prop.LastApprovedVersionBlockNo.MapAlias);
                m_lstSelect.Add(PackageListVM.Prop.LastApprovedVersionUnit.MapAlias);
                m_lstSelect.Add(PackageListVM.Prop.LastApprovedVersionFeePercentage.MapAlias);

                Dictionary<int, DataSet> m_dicSPackageDA = m_objDPackageDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);
                if (m_objDPackageDA.Message == string.Empty)
                {
                    m_lstPackageVM = (
                        from DataRow m_drSPackageDA in m_dicSPackageDA[0].Tables[0].Rows
                        select new PackageListVM()
                        {
                            PackageID = m_drSPackageDA[PackageListVM.Prop.PackageID.Name].ToString(),
                            BudgetPlanID = m_drSPackageDA[PackageListVM.Prop.BudgetPlanID.Name].ToString(),
                            MaxVersion = int.Parse(m_drSPackageDA[PackageListVM.Prop.MaxVersion.Name].ToString()),
                            UnitTypeDesc = m_drSPackageDA[PackageListVM.Prop.UnitTypeDesc.Name].ToString(),
                            Area = (decimal)m_drSPackageDA[PackageListVM.Prop.LastApprovedVersionArea.Name],
                            FeePercentage = (decimal)m_drSPackageDA[PackageListVM.Prop.LastApprovedVersionFeePercentage.Name],
                            BlokNo = m_drSPackageDA[PackageListVM.Prop.LastApprovedVersionBlockNo.Name].ToString(),
                            Unit = (decimal)m_drSPackageDA[PackageListVM.Prop.LastApprovedVersionUnit.Name]
                        }
                    ).ToList();
                }
            }

            List<PackageVM> TotalVendorAndRABVersion = new List<PackageVM>();
            List<VendorVM> ListVendor_ = new List<VendorVM>();
            string message = "";
            TotalVendorAndRABVersion = GetRABVersionAndTotalVendor(m_lstPackageVM,ref ListVendor_,ref message);

            foreach (var vendor in ListVendor_)
            {
                foreach (PackageVM PVM in TotalVendorAndRABVersion)
                {
                    foreach (VendorVM v in PVM.ListVendor.Where(q => q.VendorID == vendor.VendorID))
                    {
                        vendor.VendorTotalPackage += v.VendorP3Value * PVM.Unit;
                    }

                }

            }
            

            ListVendor_ = ListVendor_.OrderBy(x => x.VendorTotalPackage).ToList();

            if (!string.IsNullOrEmpty(message))
            {
                Global.ShowErrorAlert(title, message);
                return this.Direct();
            }
            DataTable dt = new DataTable();
            string filename = "Report Komparasi.xls";
            int TotalRABVersion = 2;
            int TotalVendor = ListVendor_.Count;
            int lengthCol = 7 + (2 * TotalRABVersion) + (2 * TotalVendor);
            object[] arrayRow = new object[lengthCol];
            #endregion

            #region Column
            dt.Columns.Add(new DataColumn("No.", typeof(string)));
            dt.Columns.Add(new DataColumn("Type", typeof(string)));
            dt.Columns.Add(new DataColumn("Pondasi.", typeof(string)));
            dt.Columns.Add(new DataColumn("LuasRAB", typeof(string)));
            dt.Columns.Add(new DataColumn("Blok", typeof(string)));
            dt.Columns.Add(new DataColumn("JumlahUnit", typeof(string)));
            dt.Columns.Add(new DataColumn("Rekapitulasi", typeof(string)));
            dt.Columns.Add(new DataColumn("RABUnitPriceFirst", typeof(string)));
            dt.Columns.Add(new DataColumn("RABPricePerMetersFirst", typeof(string)));
            dt.Columns.Add(new DataColumn("RABUnitPriceLast", typeof(string)));
            dt.Columns.Add(new DataColumn("RABPricePerMetersLast", typeof(string)));

            for (int x = 0; x < TotalVendor; x++)
            {
                dt.Columns.Add(new DataColumn("VendorUnitPrice" + x.ToString(), typeof(string)));
                dt.Columns.Add(new DataColumn("VendorPricePerMeters" + x.ToString(), typeof(string)));
            }
            #endregion

            #region Row Header
            arrayRow = new object[lengthCol];
            int StartRangeRABVersion = 7;
            int EndRangeRABVersion = 7 + (TotalRABVersion * 2);
            int RABVersionCount = 1;
            int VendorCounts = 0;
            for (int row = 0; row < 3; row++)
            {
                bool PricePerUnitOrArea = true; //true Perunit, false Area
                arrayRow = new object[lengthCol];
                for (int x = 0; x < lengthCol; x++)
                {
                    if (x < 7)
                    {
                        if (row != 0)
                            arrayRow[x] = "";
                        else
                        {
                            switch (x)
                            {
                                case 0:
                                    arrayRow[x] = "no";
                                    break;
                                case 1:
                                    arrayRow[x] = "Type";
                                    break;
                                case 2:
                                    arrayRow[x] = "Pondasi";
                                    break;
                                case 3:
                                    arrayRow[x] = "Luas (m2) RAB";
                                    break;
                                case 4:
                                    arrayRow[x] = "Blok";
                                    break;
                                case 5:
                                    arrayRow[x] = "Jumlah Unit";
                                    break;
                                case 6:
                                    arrayRow[x] = "Rekapitulasi";
                                    break;
                            }
                        }
                    }
                    else
                    {
                        if (x >= StartRangeRABVersion && x < EndRangeRABVersion)
                        {
                            if (PricePerUnitOrArea)
                            {
                                if (row == 0)
                                    arrayRow[x] = RABVersionCount==1?"RAB (dari Budget Plan Version 1)" : "RAB (dari Budget Plan Last Version)";
                                else if (row == 1)
                                    arrayRow[x] = "Per Unit";
                                else
                                    arrayRow[x] = "";

                                RABVersionCount++;
                            }
                            else
                            {
                                if (row == 1)
                                    arrayRow[x] = "Harga / m2 RAB";
                                else
                                    arrayRow[x] = "";
                            }
                            PricePerUnitOrArea = !PricePerUnitOrArea;
                        }
                        else
                        {
                            if (PricePerUnitOrArea)
                            {
                                if (row == 0)
                                    arrayRow[x] = ListVendor_[VendorCounts].VendorDesc;
                                else if (row == 1)
                                    arrayRow[x] = "Per Unit";
                                else
                                    arrayRow[x] = "";

                                VendorCounts++;
                            }
                            else
                            {
                                if (row == 1)
                                    arrayRow[x] = "Harga / m2 RAB";
                                else
                                    arrayRow[x] = "";
                            }
                            PricePerUnitOrArea = !PricePerUnitOrArea;
                        }
                    }
                }

                dt.Rows.Add(arrayRow);
            }
            //Add Space
            arrayRow = new object[lengthCol];
            dt.Rows.Add(arrayRow);
            #endregion
            
            #region Row Data
            int No = 1;
            foreach (PackageVM PVM in TotalVendorAndRABVersion)
            {
                bool PricePerUnitOrArea = true;
                VendorCounts = 0;
                arrayRow = new object[lengthCol];
                for (int x = 0; x < 11; x++)
                {
                    switch (x)
                    {
                        case 0:
                            arrayRow[x] = No.ToString();
                            break;
                        case 1:
                            arrayRow[x] = PVM.UnitTypeDesc;
                            break;
                        case 2:
                            arrayRow[x] = "";
                            break;
                        case 3:
                            arrayRow[x] = PVM.Area.ToString("#,##0.00");
                            break;
                        case 4:
                            arrayRow[x] = PVM.Blok;
                            break;
                        case 5:
                            arrayRow[x] = PVM.Unit.ToString("#,##0.00");
                            break;
                        case 6:
                            arrayRow[x] = "";
                            break;
                        case 7:
                            arrayRow[x] = PVM.RABFirstVersion.P3value.ToString("#,##0.00");
                            break;
                        case 8:
                            arrayRow[x] = PVM.RABFirstVersion.RABSubtotal.ToString("#,##0.00");
                            break;
                        case 9:
                            arrayRow[x] = PVM.RABLastVersion.P3value.ToString("#,##0.00");
                            break;
                        case 10:
                            arrayRow[x] = PVM.RABLastVersion.RABSubtotal.ToString("#,##0.00");
                            break;
                    }
                }

                int StartVendor = 11;
                foreach (VendorVM Vnd in ListVendor_)
                {                    
                    arrayRow[StartVendor] = "";
                    foreach (VendorVM v in PVM.ListVendor.Where(x=>x.VendorID==Vnd.VendorID))
                        arrayRow[StartVendor] = v.VendorP3Value.ToString("#,##0.00");                        
                    arrayRow[StartVendor+1] = "";
                    foreach (VendorVM v in PVM.ListVendor.Where(x => x.VendorID == Vnd.VendorID))
                        arrayRow[StartVendor+1] = v.VendorSubTotal.ToString("#,##0.00");                    
                    StartVendor += 2;                    
                }                
                dt.Rows.Add(arrayRow);
                //Add Space
                arrayRow = new object[lengthCol];
                dt.Rows.Add(arrayRow);
            }
            #endregion

            #region Total and Percentage
            //Add Total
            decimal TotalUnitLastVersion = 0;
            arrayRow = new object[lengthCol];
            int VendorPosition = 0;
            for(int x = 1; x < lengthCol; x++)
            {
                if (x == 1)
                    arrayRow[x] = "TOTAL";
                else if (x == 5)//Unit
                {
                    decimal TotalUnit = 0;
                    foreach (PackageVM PVM in TotalVendorAndRABVersion)
                        TotalUnit += PVM.Unit;

                    arrayRow[x] = TotalUnit.ToString("#,##0.00");
                }
                else if (x == 7)//RAB First Version
                {
                    decimal TotalUnit = 0;
                    foreach (PackageVM PVM in TotalVendorAndRABVersion)
                    {
                        TotalUnit += PVM.RABFirstVersion.P3value * PVM.Unit;
                    }
                        
                    arrayRow[x] = TotalUnit.ToString("#,##0.00");
                }
                else if (x == 9)//RAB Last Version
                {
                    decimal TotalUnit = 0;
                    foreach (PackageVM PVM in TotalVendorAndRABVersion)
                    {
                        TotalUnit += PVM.RABLastVersion.P3value * PVM.Unit;
                    }
                        
                    TotalUnitLastVersion = TotalUnit;
                    arrayRow[x] = TotalUnit.ToString("#,##0.00");
                }
                else if (x > 10 && x < lengthCol && x%2!=0)
                {
                    decimal TotalUnit = 0;
                    foreach (PackageVM PVM in TotalVendorAndRABVersion) {
                        decimal p3Vendor = 0;
                        foreach (VendorVM v in PVM.ListVendor.Where(q => q.VendorID == ListVendor_[VendorPosition].VendorID))
                        {
                            p3Vendor = v.VendorP3Value * PVM.Unit;
                        }
                        TotalUnit += p3Vendor;
                    }

                    arrayRow[x] = TotalUnit.ToString("#,##0.00");
                    VendorPosition++;
                }
                else
                    arrayRow[x] = "";
            }
            dt.Rows.Add(arrayRow);

            //Add Space
            arrayRow = new object[lengthCol];
            dt.Rows.Add(arrayRow);
            
            //Add Keterangan Percentage to RAB
            arrayRow = new object[lengthCol];
            VendorPosition = 0;
            for (int x = 4; x < lengthCol; x++)
            {
                if (x == 4)
                    arrayRow[x] = "% thd RAB";
                else if (x == 7)//RAB First Version
                {
                    decimal TotalUnitFirstVerion = 0;
                    
                    foreach (PackageVM PVM in TotalVendorAndRABVersion)
                        TotalUnitFirstVerion += PVM.RABFirstVersion.P3value * PVM.Unit;
                    decimal percentage = 0;

                    if (TotalUnitLastVersion>0)
                        percentage = (TotalUnitFirstVerion - TotalUnitLastVersion) / TotalUnitLastVersion * 100;

                    arrayRow[x] = percentage.ToString("#,##0.00")+"%";
                }
                else if (x > 10 && x < lengthCol && x % 2 != 0)
                {
                    decimal TotalUnit = 0;
                    foreach (PackageVM PVM in TotalVendorAndRABVersion)
                    {
                        decimal p3Vendor = 0;
                        foreach (VendorVM v in PVM.ListVendor.Where(q => q.VendorID == ListVendor_[VendorPosition].VendorID))
                           p3Vendor = v.VendorP3Value * PVM.Unit;
                        
                        TotalUnit += p3Vendor;
                    }
                    decimal percentage = 0;

                    if (TotalUnitLastVersion > 0)
                        percentage = (TotalUnit - TotalUnitLastVersion) / TotalUnitLastVersion * 100;
                    arrayRow[x] = percentage.ToString("#,##0.00") +"%";
                    VendorPosition++;
                }
                else
                    arrayRow[x] = "";
            }
            dt.Rows.Add(arrayRow);
            #endregion
            
            CreateExcel(dt,Descriptions, filename, TotalVendorAndRABVersion,ListVendor_);            
            return this.Direct(filename);
        }

        public void CreateExcel(DataTable sourceTable, string Descriptions,string fileName, List<PackageVM> Package, List<VendorVM> lstVendors)
        {
            HSSFWorkbook m_objWorkbook = new HSSFWorkbook();
            MemoryStream m_objmemoryStream = new MemoryStream();
            HSSFSheet m_objsheet = (HSSFSheet)m_objWorkbook.CreateSheet("BPL");
            
            int FirstRowData = 3;
            HSSFRow m_objheaderRow2 = (HSSFRow)m_objsheet.CreateRow(1);
            int startRangeRABVersion = 7;
            #region Style
            var FontReg = m_objWorkbook.CreateFont();
            FontReg.FontHeightInPoints = 11;
            FontReg.FontName = "Arial";
            FontReg.IsBold = false;

            var FontBold = m_objWorkbook.CreateFont();
            FontBold.FontHeightInPoints = 11;
            FontBold.FontName = "Arial";
            FontBold.IsBold = true;

            var FontBoldTitle = m_objWorkbook.CreateFont();
            FontBoldTitle.FontHeightInPoints = 32;
            FontBoldTitle.FontName = "Arial";
            FontBoldTitle.IsBold = true;

            HSSFCellStyle StyleTitle = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            StyleTitle.VerticalAlignment = VerticalAlignment.Center;
            StyleTitle.Alignment = HorizontalAlignment.Left;
            StyleTitle.SetFont(FontBoldTitle);

            HSSFCellStyle headerCellStyle = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            headerCellStyle.VerticalAlignment = VerticalAlignment.Center;
            headerCellStyle.Alignment = HorizontalAlignment.Center;
            headerCellStyle.BorderBottom = BorderStyle.Thin;
            headerCellStyle.BorderTop = BorderStyle.Thin;
            headerCellStyle.BorderRight = BorderStyle.Thin;
            headerCellStyle.FillForegroundColor = HSSFColor.LightTurquoise.Index;
            headerCellStyle.FillPattern = FillPattern.SolidForeground;
            headerCellStyle.SetFont(FontBold);
            headerCellStyle.WrapText = true;

            HSSFCellStyle headerCellStyleReg = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            headerCellStyleReg.VerticalAlignment = VerticalAlignment.Center;
            headerCellStyleReg.Alignment = HorizontalAlignment.Center;
            headerCellStyleReg.BorderBottom = BorderStyle.Thin;
            headerCellStyleReg.BorderTop = BorderStyle.Thin;
            headerCellStyleReg.BorderRight = BorderStyle.Thin;
            headerCellStyleReg.WrapText = true;

            HSSFCellStyle headerCellStyleRegBottom = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            headerCellStyleRegBottom.VerticalAlignment = VerticalAlignment.Center;
            headerCellStyleRegBottom.Alignment = HorizontalAlignment.Center;
            headerCellStyleRegBottom.BorderBottom = BorderStyle.Thin;
            headerCellStyleRegBottom.BorderTop = BorderStyle.Thin;
            headerCellStyleRegBottom.BorderRight = BorderStyle.Thin;
            headerCellStyleRegBottom.SetFont(FontBold);

            #endregion

            #region Title
            HSSFRow m_objheaderRowTitle = null;
            for (int x = 0; x < 2; x++)
            {
                m_objheaderRowTitle = (HSSFRow)m_objsheet.CreateRow(x);
                m_objheaderRowTitle.Height = 750;
                HSSFCell CellTitle = (HSSFCell)m_objheaderRowTitle.CreateCell(0);
                //headerCell.SetCellValue(column.ColumnName);
                CellTitle.SetCellValue(x==0?"REKAPITULASI PENAWARAN": Descriptions);
                CellTitle.CellStyle = StyleTitle;
            }

            //Descriptions
            #endregion

            #region Header  & Data
            int RowIndex = FirstRowData;
            foreach (DataRow row in sourceTable.Rows)
            {
                HSSFRow m_objheaderRow = (HSSFRow)m_objsheet.CreateRow(RowIndex);
                m_objheaderRow.Height = 550;
                int columnCount = -1;
                bool oddEven = true;
                if (RowIndex < FirstRowData + 3)
                {
                    bool firstCol = true;
                    foreach (DataColumn column in sourceTable.Columns)
                    {
                        if (firstCol)
                            firstCol = !firstCol;
                        else
                        {
                            HSSFCell headerCell = (HSSFCell)m_objheaderRow.CreateCell(columnCount);
                            //headerCell.SetCellValue(column.ColumnName);
                            headerCell.SetCellValue(row[column].ToString());
                            headerCell.CellStyle = headerCellStyle;
                            if (RowIndex == FirstRowData && columnCount >= startRangeRABVersion)//TODO
                            {
                                if (oddEven)
                                {
                                    var mergedcell = new NPOI.SS.Util.CellRangeAddress(FirstRowData, FirstRowData, columnCount - 1, columnCount);
                                    m_objsheet.AddMergedRegion(mergedcell);
                                }
                                oddEven = !oddEven;
                            }
                            else if (RowIndex == FirstRowData && columnCount < startRangeRABVersion - 1)
                            {
                                var mergedcell = new NPOI.SS.Util.CellRangeAddress(FirstRowData, FirstRowData + 2, columnCount, columnCount);
                                m_objsheet.AddMergedRegion(mergedcell);
                            }
                            else if (RowIndex == FirstRowData + 1 && columnCount >= startRangeRABVersion - 1)
                            {
                                var mergedcell = new NPOI.SS.Util.CellRangeAddress(FirstRowData + 1, FirstRowData + 2, columnCount, columnCount);
                                m_objsheet.AddMergedRegion(mergedcell);
                            }
                        }
                        columnCount++;
                    }
                }
                else
                {
                    bool isBottomRow = RowIndex < sourceTable.Rows.Count+FirstRowData && RowIndex >= sourceTable.Rows.Count - 3+FirstRowData;
                    bool isSpaceBottom = RowIndex == sourceTable.Rows.Count + FirstRowData - 2;
                    columnCount = -1;
                    m_objheaderRow.Height = 550;
                    bool firstCol = true;
                    foreach (DataColumn column in sourceTable.Columns)
                    {
                        if (firstCol)
                            firstCol = !firstCol;
                        else
                        {
                            if(isSpaceBottom)
                                m_objheaderRow.Height = 100 ;
                            else
                                m_objheaderRow.Height = 550; 

                            HSSFCell headerCell = (HSSFCell)m_objheaderRow.CreateCell(columnCount);
                            headerCell.SetCellValue(row[column].ToString());
                            headerCell.CellStyle = isBottomRow? headerCellStyleRegBottom: headerCellStyleReg;

                            var b = HSSFDataFormat.GetBuiltinFormat("#,##0.00");
                        }
                        columnCount++;
                    }
                }
                RowIndex++;
            }
            #endregion
            
            int cols = 0;
            foreach (DataColumn column in sourceTable.Columns)
            {
                if (cols>5)
                    m_objsheet.SetColumnWidth(cols, 5000);
                cols++;
            }

            m_objsheet.SetColumnWidth(0, 3500);
            m_objsheet.SetColumnWidth(2, 3000);
            m_objsheet.SetColumnWidth(3, 10000);
            m_objsheet.SetColumnWidth(5, 4000);
            m_objWorkbook.Write(m_objmemoryStream);
            //Save to server before download
            //delete first
            DirectoryInfo di = new DirectoryInfo(Request.MapPath("~/Content/"));
            foreach (FileInfo filez in di.GetFiles())
            {
                string ext = Path.GetExtension(filez.Extension);
                if (ext == ".xls")
                    filez.Delete();
            }
            FileStream file = new FileStream(Server.MapPath("~/Content/" + fileName), FileMode.Create, FileAccess.Write);
            m_objmemoryStream.WriteTo(file);
            file.Close();
            m_objmemoryStream.Close();
            m_objmemoryStream.Flush();
        }       
        #endregion

        #region Direct Method

        public ActionResult GetPackage(string ControlPackageID, string ControlPackageDesc, string FilterPackageID, string FilterPackageDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<PackageVM>> m_dicPackageData = GetPackageData(true, FilterPackageID, FilterPackageDesc);
                KeyValuePair<int, List<PackageVM>> m_kvpPackageVM = m_dicPackageData.AsEnumerable().ToList()[0];
                if (m_kvpPackageVM.Key < 1 || (m_kvpPackageVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpPackageVM.Key > 1 && !Exact)
                    return Browse(ControlPackageID, ControlPackageDesc, FilterPackageID, FilterPackageDesc);

                m_dicPackageData = GetPackageData(false, FilterPackageID, FilterPackageDesc);
                PackageVM m_objPackageVM = m_dicPackageData[0][0];
                this.GetCmp<TextField>(ControlPackageID).Value = m_objPackageVM.PackageID;
                this.GetCmp<TextField>(ControlPackageDesc).Value = m_objPackageVM.PackageDesc;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

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
            m_lstFilter.Add("TPackage");
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

        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, string PackageID, string PackageDesc, List<PackageListVM> m_lstPackageListVM)
        {
            List<string> m_lstReturn = new List<string>();

            if (PackageDesc == string.Empty)
                m_lstReturn.Add(PackageVM.Prop.PackageDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));

            if (m_lstPackageListVM.Count == 0)
            {
                m_lstReturn.Add(PackageVM.Prop.BudgetPlanListDesc.Desc + " " + General.EnumDesc(MessageLib.NotExist));
            }
            else
            {
                DPackageListDA m_objDPackageListDA = new DPackageListDA();
                m_objDPackageListDA.ConnectionStringName = Global.ConnStrConfigName;
                Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(PackageListVM.Prop.PackageID.MapAlias);
                m_lstSelect.Add(PackageListVM.Prop.PackageStatusID.MapAlias);

                foreach (PackageListVM item in m_lstPackageListVM)
                {
                    m_objFilter = new Dictionary<string, List<object>>();

                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(item.BudgetPlanID);
                    m_objFilter.Add(PackageListVM.Prop.BudgetPlanID.Map, m_lstFilter);

                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(item.BudgetPlanVersion);
                    m_objFilter.Add(PackageListVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.LessThan);
                    m_lstFilter.Add(3);
                    m_objFilter.Add(PackageListVM.Prop.PackageStatusID.Map, m_lstFilter);

                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.NotEqual);
                    m_lstFilter.Add(PackageID);
                    m_objFilter.Add(PackageListVM.Prop.PackageID.Map, m_lstFilter);

                    Dictionary<int, DataSet> m_dicDPackageListDA = m_objDPackageListDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);
                    List<PackageListVM> m_PackageListVM = new List<PackageListVM>();

                    if (m_objDPackageListDA.Message == string.Empty)
                    {
                        m_PackageListVM = (
                            from DataRow m_drSPackageDA in m_dicDPackageListDA[0].Tables[0].Rows
                            select new PackageListVM()
                            {
                                PackageID = m_drSPackageDA[PackageListVM.Prop.PackageID.Name].ToString(),
                                PackageStatusID = int.Parse(m_drSPackageDA[PackageListVM.Prop.PackageStatusID.Name].ToString())
                            }
                        ).Distinct().ToList();
                    }

                    foreach (PackageListVM itemChild in m_PackageListVM)
                    {
                        m_lstReturn.Add(PackageVM.Prop.BudgetPlanListDesc.Desc + " [" + item.BudgetPlanID
                            + "] Version [" + item.BudgetPlanVersion.ToString() + "] " + General.EnumDesc(MessageLib.Exist) + " On Active Package [" + itemChild.PackageID + "]");
                    }
                }
            }

            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters, string PackageID = "")
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            if (PackageID.Length > 0)
                m_dicReturn.Add(PackageVM.Prop.PackageID.Name, PackageID);
            else
                m_dicReturn.Add(PackageVM.Prop.PackageID.Name, parameters[PackageVM.Prop.PackageID.Name]);

            m_dicReturn.Add(PackageVM.Prop.PackageDesc.Name, parameters[PackageVM.Prop.PackageDesc.Name]);
            m_dicReturn.Add(PackageVM.Prop.StatusID.Name, parameters[PackageVM.Prop.StatusID.Name]);

            return m_dicReturn;
        }

        private PackageVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            PackageVM m_objPackageVM = new PackageVM();
            TPackageDA m_objTPackageDA = new TPackageDA();
            m_objTPackageDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(PackageVM.Prop.PackageID.MapAlias);
            m_lstSelect.Add(PackageVM.Prop.PackageDesc.MapAlias);
            m_lstSelect.Add(PackageVM.Prop.StatusID.MapAlias);
            m_lstSelect.Add(PackageVM.Prop.StatusDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objPackageVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(PackageVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicSPackageDA = m_objTPackageDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objTPackageDA.Message == string.Empty)
            {
                DataRow m_drSPackageDA = m_dicSPackageDA[0].Tables[0].Rows[0];
                m_objPackageVM.PackageID = m_drSPackageDA[PackageVM.Prop.PackageID.Name].ToString();
                m_objPackageVM.PackageDesc = m_drSPackageDA[PackageVM.Prop.PackageDesc.Name].ToString();
                m_objPackageVM.StatusDesc = m_drSPackageDA[PackageVM.Prop.StatusDesc.Name].ToString();
                m_objPackageVM.StatusID = int.Parse(m_drSPackageDA[PackageVM.Prop.StatusID.Name].ToString());

                m_objPackageVM.BudgetPlanList = GetPackageList(m_objPackageVM.PackageID);

                if (m_objPackageVM.BudgetPlanList.Count > 0)
                {
                    m_objPackageVM.ProjectID = m_objPackageVM.BudgetPlanList[0].ProjectID;
                    m_objPackageVM.ProjectDesc = m_objPackageVM.BudgetPlanList[0].ProjectDesc;
                    m_objPackageVM.CompanyID = m_objPackageVM.BudgetPlanList[0].CompanyID;
                }
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objTPackageDA.Message;

            return m_objPackageVM;
        }

        private List<PackageListVM> GetPackageList(string packageID)
        {
            List<PackageListVM> m_lstPackageList = new List<PackageListVM>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            DataAccess.DPackageListDA m_objDPackageListDA = new DPackageListDA();
            m_objDPackageListDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(PackageListVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(PackageListVM.Prop.BudgetPlanVersion.MapAlias);
            m_lstSelect.Add(PackageListVM.Prop.BudgetPlanTypeDesc.MapAlias);
            m_lstSelect.Add(PackageListVM.Prop.StatusDesc.MapAlias);
            m_lstSelect.Add(PackageListVM.Prop.StatusID.MapAlias);
            m_lstSelect.Add(PackageListVM.Prop.Description.MapAlias);
            m_lstSelect.Add(PackageListVM.Prop.ProjectID.MapAlias);
            m_lstSelect.Add(PackageListVM.Prop.ProjectDesc.MapAlias);
            m_lstSelect.Add(PackageListVM.Prop.CompanyID.MapAlias);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(packageID);
            m_objFilter.Add(PackageListVM.Prop.PackageID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicDPackageDA = m_objDPackageListDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objDPackageListDA.Message == string.Empty)
            {
                m_lstPackageList = (
                    from DataRow m_drDPackageListDA in m_dicDPackageDA[0].Tables[0].Rows
                    select new PackageListVM()
                    {
                        PackageID = packageID,
                        BudgetPlanID = m_drDPackageListDA[PackageListVM.Prop.BudgetPlanID.Name].ToString(),
                        BudgetPlanVersion = int.Parse(m_drDPackageListDA[PackageListVM.Prop.BudgetPlanVersion.Name].ToString()),
                        Description = m_drDPackageListDA[PackageListVM.Prop.Description.Name].ToString(),
                        StatusDesc = m_drDPackageListDA[PackageListVM.Prop.StatusDesc.Name].ToString(),
                        BudgetPlanTypeDesc = m_drDPackageListDA[PackageListVM.Prop.BudgetPlanTypeDesc.Name].ToString(),
                        StatusID = int.Parse(m_drDPackageListDA[PackageListVM.Prop.StatusID.Name].ToString()),
                        CompanyID = m_drDPackageListDA[PackageListVM.Prop.CompanyID.Name].ToString(),
                        ProjectDesc = m_drDPackageListDA[PackageListVM.Prop.ProjectDesc.Name].ToString(),
                        ProjectID = m_drDPackageListDA[PackageListVM.Prop.ProjectID.Name].ToString()
                    }
                ).ToList();
            }

            return m_lstPackageList;
        }
        private List<PackageVM> GetRABVersionAndTotalVendor(List<PackageListVM> lstPackage, ref List<VendorVM> lstVendorGeneral, ref string message)
        {
            List<PackageVM> lstPackageReturn = new List<PackageVM>();
            foreach (PackageListVM package in lstPackage)
            {
                PackageVM pckRet = new PackageVM();
                pckRet.Area = package.Area;
                pckRet.Unit = package.Unit;
                pckRet.UnitTypeDesc = package.UnitTypeDesc;
                pckRet.Blok = package.BlokNo;
                pckRet.ListVendor = new List<VendorVM>();
                decimal Area = package.Area;
                decimal FeePercentage = package.FeePercentage;

                #region Get RAB First Version
                if (string.IsNullOrEmpty(message))
                {
                    BudgetPlanVM bPlan = new BudgetPlanVM();                    
                    bPlan.P3value = GetRABSubtotal(package.BudgetPlanID, FeePercentage, 1);
                    bPlan.RABSubtotal = bPlan.P3value/Area;
                    pckRet.RABFirstVersion = bPlan;
                }
                #endregion

                #region Get RAB Last Version 
                if (string.IsNullOrEmpty(message))
                {
                    BudgetPlanVM bPlan = new BudgetPlanVM();
                    bPlan.P3value = package.MaxVersion == 1? pckRet.RABFirstVersion.P3value : GetRABSubtotal(package.BudgetPlanID,FeePercentage,package.MaxVersion);
                    bPlan.RABSubtotal = bPlan.P3value/Area;
                    pckRet.RABLastVersion = bPlan;
                }
                #endregion

                #region GetVendorValue
                if (string.IsNullOrEmpty(message))
                {
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    List<object> m_lstFilter = new List<object>();
                    List<BudgetPlanVersionPeriodVM> m_lstBudgetPlanPeriod = new List<BudgetPlanVersionPeriodVM>();
                    List<BudgetPlanVersionVendorVM> m_lstBudgetPlanVersionVendor = new List<BudgetPlanVersionVendorVM>();
                    DBudgetPlanVersionVendorDA m_objDBudgetPlanVersionVendorDA = new DBudgetPlanVersionVendorDA();
                    m_objDBudgetPlanVersionVendorDA.ConnectionStringName = Global.ConnStrConfigName;
                    DBudgetPlanVersionPeriodDA m_objDBudgetPlanVersionperiodDA = new DBudgetPlanVersionPeriodDA();
                    m_objDBudgetPlanVersionperiodDA.ConnectionStringName = Global.ConnStrConfigName;

                    List<string> m_lstSelect = new List<string>();
                    m_lstSelect.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanVersionPeriodID.MapAlias);
                    m_lstSelect.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanVersion.MapAlias);
                    m_lstSelect.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanID.MapAlias);
                    m_lstSelect.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanPeriodID.MapAlias);

                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(BudgetPlanPeriod.IQ);
                    m_objFilter.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanPeriodID.Map, m_lstFilter);

                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(package.MaxVersion);
                    m_objFilter.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(package.BudgetPlanID);
                    m_objFilter.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanID.Map, m_lstFilter);

                    Dictionary<int, DataSet> m_dicDBudgetPlanVersionPeriod = m_objDBudgetPlanVersionperiodDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

                    if (m_objDBudgetPlanVersionperiodDA.Message == string.Empty)
                    {
                        m_lstBudgetPlanPeriod = (
                            from DataRow m_dBudgetPlanPeriodDA in m_dicDBudgetPlanVersionPeriod[0].Tables[0].Rows
                            select new BudgetPlanVersionPeriodVM()
                            {
                                BudgetPlanVersionPeriodID = m_dBudgetPlanPeriodDA[BudgetPlanVersionPeriodVM.Prop.BudgetPlanVersionPeriodID.Name].ToString(),
                                BudgetPlanVersion = (int)m_dBudgetPlanPeriodDA[BudgetPlanVersionPeriodVM.Prop.BudgetPlanVersion.Name],
                                BudgetPlanID = m_dBudgetPlanPeriodDA[BudgetPlanVersionPeriodVM.Prop.BudgetPlanID.Name].ToString()
                            }
                        ).ToList();

                        foreach (BudgetPlanVersionPeriodVM BPPeriod in m_lstBudgetPlanPeriod)
                        {
                            m_lstSelect = new List<string>();
                            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionPeriodID.MapAlias);
                            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionVendorID.MapAlias);
                            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanID.MapAlias);
                            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.VendorID.MapAlias);
                            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.VendorDesc.MapAlias);
                            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.FeePercentage.MapAlias);

                            m_objFilter = new Dictionary<string, List<object>>();
                            m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(BPPeriod.BudgetPlanVersionPeriodID);
                            m_objFilter.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionPeriodID.Map, m_lstFilter);

                            Dictionary<int, DataSet> m_dicDBudgetPlanVersionVendor = m_objDBudgetPlanVersionVendorDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

                            if (m_objDBudgetPlanVersionVendorDA.Message == string.Empty)
                            {
                                m_lstBudgetPlanVersionVendor = (
                                from DataRow m_dBudgetPlanPeriodDA in m_dicDBudgetPlanVersionVendor[0].Tables[0].Rows
                                select new BudgetPlanVersionVendorVM()
                                {
                                    BudgetPlanVersionPeriodID = m_dBudgetPlanPeriodDA[BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionPeriodID.Name].ToString(),
                                    BudgetPlanVersionVendorID = m_dBudgetPlanPeriodDA[BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionVendorID.Name].ToString(),
                                    VendorID = m_dBudgetPlanPeriodDA[BudgetPlanVersionVendorVM.Prop.VendorID.Name].ToString(),
                                    FirstName = m_dBudgetPlanPeriodDA[BudgetPlanVersionVendorVM.Prop.VendorDesc.Name].ToString(),
                                    FeePercentage = m_dBudgetPlanPeriodDA[BudgetPlanVersionVendorVM.Prop.FeePercentage.Name] == null? 0:(decimal)m_dBudgetPlanPeriodDA[BudgetPlanVersionVendorVM.Prop.FeePercentage.Name]
                                }).ToList();

                                foreach (BudgetPlanVersionVendorVM VersionVendor in m_lstBudgetPlanVersionVendor)
                                {
                                    VendorVM NilaiRABVendor = new VendorVM();
                                    NilaiRABVendor.VendorP3Value = GetVendorSubtotal(VersionVendor.BudgetPlanVersionVendorID, (decimal)VersionVendor.FeePercentage, ref message);
                                    NilaiRABVendor.VendorSubTotal = NilaiRABVendor.VendorP3Value/Area;
                                    NilaiRABVendor.VendorID = VersionVendor.VendorID;
                                    NilaiRABVendor.VendorDesc = VersionVendor.FirstName;

                                    if (!string.IsNullOrEmpty(message))
                                        return new List<PackageVM>();

                                    pckRet.ListVendor.Add(NilaiRABVendor);
                                    bool containing = false;
                                    foreach (VendorVM v in lstVendorGeneral.Where(x => x.VendorID == NilaiRABVendor.VendorID))
                                    {
                                        containing = true;
                                        break;
                                    }
                                    if (!containing)
                                        lstVendorGeneral.Add(NilaiRABVendor);
                                }
                            }
                        }
                    }
                }
                #endregion

                lstPackageReturn.Add(pckRet);
            }
            return lstPackageReturn;
        }

        private List<ProjectVM> GetProjectList(string divisionID)
        {
            List<ProjectVM> m_lstProjectList = new List<ProjectVM>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            MProjectDA m_objMProjectListDA = new MProjectDA();
            m_objMProjectListDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ProjectVM.Prop.ProjectID.MapAlias);
            m_lstSelect.Add(ProjectVM.Prop.ProjectDesc.MapAlias);
            m_lstSelect.Add(ProjectVM.Prop.CompanyID.MapAlias);
            m_lstSelect.Add(ProjectVM.Prop.DivisionID.MapAlias);
            m_lstSelect.Add(ProjectVM.Prop.LocationID.MapAlias);
            m_lstSelect.Add(ProjectVM.Prop.City.MapAlias);
            m_lstSelect.Add(ProjectVM.Prop.Street.MapAlias);
            m_lstSelect.Add(ProjectVM.Prop.Postal.MapAlias);


            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(divisionID);
            m_objFilter.Add(ProjectVM.Prop.DivisionID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMProjectDA = m_objMProjectListDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objMProjectListDA.Message == string.Empty)
            {
                m_lstProjectList = (
                    from DataRow m_drMProjectListDA in m_dicMProjectDA[0].Tables[0].Rows
                    select new ProjectVM()
                    {
                        ProjectID = m_drMProjectListDA[ProjectVM.Prop.ProjectID.Name].ToString(),
                        ProjectDesc = m_drMProjectListDA[ProjectVM.Prop.ProjectDesc.Name].ToString(),
                        CompanyID = m_drMProjectListDA[ProjectVM.Prop.CompanyID.Name].ToString(),
                        DivisionID = m_drMProjectListDA[ProjectVM.Prop.DivisionID.Name].ToString(),
                        LocationID = m_drMProjectListDA[ProjectVM.Prop.LocationID.Name].ToString(),
                        City = m_drMProjectListDA[ProjectVM.Prop.City.Name].ToString(),
                        Street = m_drMProjectListDA[ProjectVM.Prop.Street.Name].ToString(),
                        Postal = m_drMProjectListDA[ProjectVM.Prop.Postal.Name].ToString()
                    }
                ).ToList();
            }

            return m_lstProjectList;
        }
        #endregion

        #region Public Method

        public Dictionary<int, List<PackageVM>> GetPackageData(bool isCount, string PackageID, string PackageDesc)
        {
            int m_intCount = 0;
            List<PackageVM> m_lstPackageVM = new List<ViewModels.PackageVM>();
            Dictionary<int, List<PackageVM>> m_dicReturn = new Dictionary<int, List<PackageVM>>();
            TPackageDA m_objTPackageDA = new TPackageDA();
            m_objTPackageDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(PackageVM.Prop.PackageID.MapAlias);
            m_lstSelect.Add(PackageVM.Prop.PackageDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(PackageID);
            m_objFilter.Add(PackageVM.Prop.PackageID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(PackageDesc);
            m_objFilter.Add(PackageVM.Prop.PackageDesc.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicSPackageDA = m_objTPackageDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objTPackageDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpPackageBL in m_dicSPackageDA)
                    {
                        m_intCount = m_kvpPackageBL.Key;
                        break;
                    }
                else
                {
                    m_lstPackageVM = (
                        from DataRow m_drSPackageDA in m_dicSPackageDA[0].Tables[0].Rows
                        select new PackageVM()
                        {
                            PackageID = m_drSPackageDA[PackageVM.Prop.PackageID.Name].ToString(),
                            PackageDesc = m_drSPackageDA[PackageVM.Prop.PackageDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstPackageVM);
            return m_dicReturn;
        }

        #endregion
    }
}