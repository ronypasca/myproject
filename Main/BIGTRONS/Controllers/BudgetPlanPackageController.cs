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
    public class BudgetPlanPackageController : BaseController
    {
        private readonly string title = "Budget Plan Package";
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

            string m_strbussinesUnitid = getCurentUser().BusinessUnitID == null ? string.Empty : getCurentUser().BusinessUnitID;
            string m_strdivisionid = getCurentUser().DivisionID == null ? string.Empty : getCurentUser().DivisionID;
            string m_strprojectid = getCurentUser().ProjectID == null ? string.Empty : getCurentUser().ProjectID;

            if (!string.IsNullOrEmpty(m_strprojectid))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(m_strprojectid);
                m_objFilter.Add(PackageVM.Prop.PackageProjectID.Map, m_lstFilter);
            }
            
            if (string.IsNullOrEmpty(m_strprojectid) && !string.IsNullOrEmpty(m_strdivisionid))
            {
                List<ProjectVM> m_lstproject = GetProjectList(m_strdivisionid);
                if (m_lstproject.Any())
                {
                    m_lstuserproject = m_lstproject.Select(x => x.ProjectID).ToList();
                }
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.In);
                m_lstFilter.Add(String.Join(",", m_lstuserproject));
                m_objFilter.Add(PackageVM.Prop.PackageProjectID.Map, m_lstFilter);
            }

            if (string.IsNullOrEmpty(m_strprojectid) && string.IsNullOrEmpty(m_strdivisionid) && !string.IsNullOrEmpty(m_strbussinesUnitid))
            {

                List<DivisionVM> m_lstdivision = GetDivisionList(m_strbussinesUnitid);
                List<ProjectVM> m_lstproject = new List<ProjectVM>();

                foreach (var item in m_lstdivision)
                {
                    foreach (var prjvm in GetProjectList(item.DivisionID))
                    {
                        m_lstproject.Add(prjvm);
                    }
                }
                
                
                if (m_lstproject.Any())
                {
                    m_lstuserproject = m_lstproject.Select(x => x.ProjectID).ToList();
                }
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.In);
                m_lstFilter.Add(String.Join(",", m_lstuserproject));
                m_objFilter.Add(PackageVM.Prop.PackageProjectID.Map, m_lstFilter);
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
            string m_strdivisionid = getCurentUser().DivisionID == null ? string.Empty : getCurentUser().DivisionID;
            ViewDataDictionary m_vddWorkCenter = new ViewDataDictionary();
            m_vddWorkCenter.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
            m_vddWorkCenter.Add("divisionid", m_strdivisionid);
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
        private List<DivisionVM> GetDivisionList(string bussinesUnitID)
        {
            List<DivisionVM> m_lstProjectList = new List<DivisionVM>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            MDivisionDA m_objMDivisionDA = new MDivisionDA();
            m_objMDivisionDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(DivisionVM.Prop.DivisionID.MapAlias);
            m_lstSelect.Add(DivisionVM.Prop.DivisionDesc.MapAlias);
            m_lstSelect.Add(DivisionVM.Prop.BusinessUnitID.MapAlias);
            


            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(bussinesUnitID);
            m_objFilter.Add(DivisionVM.Prop.BusinessUnitID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMDivisionDA = m_objMDivisionDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objMDivisionDA.Message == string.Empty)
            {
                m_lstProjectList = (
                    from DataRow m_drMDivisionDA in m_dicMDivisionDA[0].Tables[0].Rows
                    select new DivisionVM()
                    {
                        DivisionID = m_drMDivisionDA[DivisionVM.Prop.DivisionID.Name].ToString(),
                        DivisionDesc = m_drMDivisionDA[DivisionVM.Prop.DivisionDesc.Name].ToString(),
                        BusinessUnitID = m_drMDivisionDA[DivisionVM.Prop.BusinessUnitID.Name].ToString()
                    }
                ).ToList();
            }

            return m_lstProjectList;
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