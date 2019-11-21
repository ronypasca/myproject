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
using System.Globalization;

namespace com.SML.BIGTRONS.Controllers
{
    public class BudgetPlanPackagePeriodController : BaseController
    {
        private readonly string title = "Budget Plan Package Period";
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
            string m_strdivisionid = getCurentUser().DivisionID == null ? string.Empty : getCurentUser().DivisionID;
            string m_strbussinesUnitid = getCurentUser().BusinessUnitID == null ? string.Empty : getCurentUser().BusinessUnitID;
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
                
        public ActionResult Home()
        {
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 0;
            return this.Direct();
        }
        
        //public ActionResult Detail(string Caller, string Selected, string PackageID = "")
        //{
        //    //Global.HasAccess = GetHasAccess();
        //    //if (!Global.HasAccess.Read)
        //    //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

        //    PackageVM m_objPackageVM = new PackageVM();
        //    string m_strMessage = string.Empty;
        //    Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();
        //    if (Caller == General.EnumDesc(Buttons.ButtonCancel))
        //    {
        //        if (Session[dataSessionName] != null)
        //        {
        //            try
        //            {
        //                m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Session[dataSessionName].ToString());
        //            }
        //            catch (Exception ex)
        //            {
        //                m_strMessage = ex.Message;
        //            }
        //            Session[dataSessionName] = null;
        //        }
        //        else
        //            m_strMessage = General.EnumDesc(MessageLib.Unknown);
        //    }
        //    else if (Caller == General.EnumDesc(Buttons.ButtonList))
        //        m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
        //    else if (Caller == General.EnumDesc(Buttons.ButtonSave))
        //    {
        //        NameValueCollection m_nvcParams = this.Request.Params;
        //        m_dicSelectedRow = GetFormData(m_nvcParams, PackageID);
        //    }


        //    if (m_dicSelectedRow.Count > 0)
        //        m_objPackageVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
        //    if (m_strMessage != string.Empty)
        //    {
        //        Global.ShowErrorAlert(title, m_strMessage);
        //        return this.Direct();
        //    }
        //    string m_strdivisionid = getCurentUser().DivisionID == null ? string.Empty : getCurentUser().DivisionID;
        //    ViewDataDictionary m_vddWorkCenter = new ViewDataDictionary();
        //    m_vddWorkCenter.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
        //    m_vddWorkCenter.Add("divisionid", m_strdivisionid);
        //    this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
        //    return new XMVC.PartialViewResult
        //    {
        //        ClearContainer = true,
        //        ContainerId = General.EnumName(Params.PageOne),
        //        Model = m_objPackageVM,
        //        RenderMode = RenderMode.AddTo,
        //        ViewData = m_vddWorkCenter,
        //        ViewName = "_Form",
        //        WrapByScriptTag = false
        //    };
        //}

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

            if (m_dicSelectedRow["StatusID"].ToString() != "2")
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

        public ActionResult Save(string Action, string BudgetPlanList, string VendorList)
        {
            //action = JSON.Deserialize<string>(action);
            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));


            //todo is save valid
            string m_strStartDate = this.Request.Params["ColStartDate"];
           string m_strStartTime = this.Request.Params["ColStartTime"];
           string m_strEndtDate = this.Request.Params["ColEndDate"];
           string m_strEndTime = this.Request.Params["ColEndTime"];

            DateTime m_dtStartDate = Convert.ToDateTime(m_strStartDate) + DateTime.ParseExact(m_strStartTime, "h:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
            DateTime m_dtEndDate = Convert.ToDateTime(m_strEndtDate) + DateTime.ParseExact(m_strEndTime, "h:mm tt", CultureInfo.InvariantCulture).TimeOfDay;

            var m_lstBudgetPlanVM = JSON.Deserialize<List<PackageListVM>>(BudgetPlanList);
            var m_lstVendorList = JSON.Deserialize<List<VendorVM>>(VendorList);
                       
            List<string> m_lstMessage = new List<string>();
            List<BudgetPlanVersionVendorVM> lst_VersionVendor = new List<BudgetPlanVersionVendorVM>();
            DBudgetPlanVersionVendorDA m_objBudgetPlanVersionVendorDA = new DBudgetPlanVersionVendorDA();
            DBudgetPlanVersionVendor m_objDBudgetPlanVersionVendor = new DBudgetPlanVersionVendor();
            m_objBudgetPlanVersionVendorDA.ConnectionStringName = Global.ConnStrConfigName;
            object m_objDBConnection = null;
            string m_strTransName = "BudgetPlanPeriod";

            //todo get DBudgetPlanVersionPeriod
            List<BudgetPlanVersionPeriodVM> m_lstBudgetPlanVersionPeriodVM = new List<BudgetPlanVersionPeriodVM>();
            string m_strmsg = string.Empty;
            m_lstBudgetPlanVersionPeriodVM = GetListBudgetPlanVersionPeriod(m_lstBudgetPlanVM.Select(x => x.BudgetPlanID).ToList(), ref m_strmsg);

            //todo get DBudgetPlanVersionVendor
            List<BudgetPlanVersionVendorVM> m_lstBudgetPlanVersionVendorVM = new List<BudgetPlanVersionVendorVM>();
            m_lstBudgetPlanVersionVendorVM = GetListBudgetPlanVersionVendor(m_lstBudgetPlanVersionPeriodVM.Select(x => x.BudgetPlanVersionPeriodID).ToList(), ref m_strmsg);

            //todo check empty bp and vendor before start trans

            m_objDBConnection = m_objBudgetPlanVersionVendorDA.BeginTrans(m_strTransName);

            foreach (var budgetPlanVM in m_lstBudgetPlanVM)
            {
                string BudgetPlanVersionPeriodID = string.Empty;
                //exist DBudgetPlanVersionPeriod
                if (m_lstBudgetPlanVersionPeriodVM.Any(x => x.BudgetPlanID == budgetPlanVM.BudgetPlanID && x.BudgetPlanVersion == budgetPlanVM.MaxVersion && x.BudgetPlanPeriodID == General.EnumName(BudgetPlanPeriod.IQ)))
                {
                    BudgetPlanVersionPeriodID = m_lstBudgetPlanVersionPeriodVM.Where(x => x.BudgetPlanID == budgetPlanVM.BudgetPlanID && x.BudgetPlanVersion == budgetPlanVM.MaxVersion && x.BudgetPlanPeriodID == General.EnumName(BudgetPlanPeriod.IQ)).FirstOrDefault().BudgetPlanVersionPeriodID;
                }
                else
                {
                    BudgetPlanVersionPeriodID = createDBudgetPlanVersionPeriod(budgetPlanVM.BudgetPlanID, budgetPlanVM.MaxVersion, m_objDBConnection, ref m_lstMessage);
                }
                //fail create BudgetPlanVersionPeriod
                if (m_lstMessage.Count > 0 || string.IsNullOrEmpty(BudgetPlanVersionPeriodID))
                {
                    m_objBudgetPlanVersionVendorDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                    Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
                    return this.Direct(true);
                }

                try
                {
                    foreach (var vendorVM in m_lstVendorList)
                    {
                        //exist / update m_lstBudgetPlanVersionVendorVM
                        if (m_lstBudgetPlanVersionVendorVM.Any(x => x.BudgetPlanVersionPeriodID == BudgetPlanVersionPeriodID && x.VendorID == vendorVM.VendorID))
                        {
                            Dictionary<string, List<object>> m_dicSet = new Dictionary<string, List<object>>();
                            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                            List<object> m_lstFilter = new List<object>();

                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(BudgetPlanVersionPeriodID);
                            m_objFilter.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionPeriodID.Map, m_lstFilter);

                            m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(vendorVM.VendorID);
                            m_objFilter.Add(BudgetPlanVersionVendorVM.Prop.VendorID.Map, m_lstFilter);

                            List<object> m_lstSet = new List<object>();
                            m_lstSet.Add(typeof(DateTime));
                            m_lstSet.Add(m_dtStartDate);
                            m_dicSet.Add(BudgetPlanVersionVendorVM.Prop.StartDate.Map, m_lstSet);

                            m_lstSet = new List<object>();
                            m_lstSet.Add(typeof(DateTime));
                            m_lstSet.Add(m_dtEndDate);
                            m_dicSet.Add(BudgetPlanVersionVendorVM.Prop.EndDate.Map, m_lstSet);

                            m_objBudgetPlanVersionVendorDA.UpdateBC(m_dicSet, m_objFilter, m_objDBConnection != null, m_objDBConnection);
                        }
                        //new m_lstBudgetPlanVersionVendorVM
                        else
                        {
                            m_objDBudgetPlanVersionVendor = new DBudgetPlanVersionVendor();
                            //m_objDBudgetPlanVersionVendor.BudgetPlanVersionVendorID = objBudgetPlanVersionVendorVM.BudgetPlanVersionVendorID;
                            m_objDBudgetPlanVersionVendor.BudgetPlanVersionVendorID = Guid.NewGuid().ToString("N");
                            m_objDBudgetPlanVersionVendor.BudgetPlanVersionPeriodID = BudgetPlanVersionPeriodID;
                            m_objDBudgetPlanVersionVendor.VendorID = vendorVM.VendorID;
                            m_objDBudgetPlanVersionVendor.StatusID = 0;
                            m_objDBudgetPlanVersionVendor.StartDate = m_dtStartDate;
                            m_objDBudgetPlanVersionVendor.EndDate = m_dtEndDate;
                            m_objBudgetPlanVersionVendorDA.Data = m_objDBudgetPlanVersionVendor;
                            m_objBudgetPlanVersionVendorDA.Insert(true, m_objDBConnection);
                        }
                    }
                }
                catch (Exception ex)
                {
                    m_lstMessage.Add(ex.Message);
                }
                

            }

            if (m_lstMessage.Count <= 0)
            {
                m_objBudgetPlanVersionVendorDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
                Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Updated));
                //return Detail(General.EnumDesc(Buttons.ButtonSave), null, m_strPackageID);//todo show list
                return this.Direct(true);
            }
            m_objBudgetPlanVersionVendorDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);
            
            
        }

        #endregion

        #region Direct Method
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

        public ActionResult ReadVendor()
        {
            return null;
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
            m_lstSelect.Add(PackageListVM.Prop.MaxVersion.MapAlias);

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
                        ProjectID = m_drDPackageListDA[PackageListVM.Prop.ProjectID.Name].ToString(),
                        MaxVersion = int.Parse(m_drDPackageListDA[PackageListVM.Prop.MaxVersion.Name].ToString())
                    }
                ).ToList();
            }

            return m_lstPackageList;
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

        private string createDBudgetPlanVersionPeriod(string BudgetPlanID,int BudgetPlanVersion, object m_objDBConnection, ref List<string> m_lstMessage)
        {
            
            DBudgetPlanVersionPeriodDA m_objDBudgetPlanVersionPeriodDA = new DBudgetPlanVersionPeriodDA();
            DBudgetPlanVersionPeriod m_objDBudgetPlanVersionPeriod = new DBudgetPlanVersionPeriod();
            m_objDBudgetPlanVersionPeriodDA.ConnectionStringName = Global.ConnStrConfigName;
            //insert
            m_objDBudgetPlanVersionPeriod.BudgetPlanVersionPeriodID = Guid.NewGuid().ToString("N");
            m_objDBudgetPlanVersionPeriod.BudgetPlanID = BudgetPlanID;
            m_objDBudgetPlanVersionPeriod.BudgetPlanVersion = BudgetPlanVersion;
            m_objDBudgetPlanVersionPeriod.StatusID = (int)BudgetPlanVersionPeriodStatus.Open;
            m_objDBudgetPlanVersionPeriod.PeriodVersion = 1;
            m_objDBudgetPlanVersionPeriod.BudgetPlanPeriodID = General.EnumName(BudgetPlanPeriod.IQ);
            m_objDBudgetPlanVersionPeriodDA.Data = m_objDBudgetPlanVersionPeriod;
            try
            {
                m_objDBudgetPlanVersionPeriodDA.Insert(true, m_objDBConnection);
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
            }

            if (m_objDBudgetPlanVersionPeriodDA.Message == string.Empty)
            {
                return m_objDBudgetPlanVersionPeriodDA.Data.BudgetPlanVersionPeriodID;
            }
            else
            {
                m_lstMessage.Add(m_objDBudgetPlanVersionPeriodDA.Message);
                return string.Empty;
            }
            
        }

        private List<BudgetPlanVersionPeriodVM> GetListBudgetPlanVersionPeriod(List<string> BudgetPlanIDs, ref string message)
        {
            List<BudgetPlanVersionPeriodVM> m_lsDBudgetPlanVersionPeriodVM = new List<BudgetPlanVersionPeriodVM>();
            DBudgetPlanVersionPeriodDA m_objDBudgetPlanVersionPeriodDA = new DBudgetPlanVersionPeriodDA();
            m_objDBudgetPlanVersionPeriodDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanVersionPeriodID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanPeriodDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanPeriodID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionPeriodVM.Prop.StatusID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionPeriodVM.Prop.StatusDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionPeriodVM.Prop.PeriodVersion.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(string.Join(",", BudgetPlanIDs));
            m_objFilter.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanID.Map, m_lstFilter);

            //m_lstFilter = new List<object>();
            //m_lstFilter.Add(Operator.In);
            //m_lstFilter.Add(BudgetPlanVersion);
            //m_objFilter.Add(BudgetPlanVersionPeriodVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicDBudgetPlanVersionPeriodDA = m_objDBudgetPlanVersionPeriodDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDBudgetPlanVersionPeriodDA.Success)
            {
                foreach (DataRow m_drDBudgetPlanVersionPeriodDA in m_dicDBudgetPlanVersionPeriodDA[0].Tables[0].Rows)
                {
                    BudgetPlanVersionPeriodVM m_objBudgetPlanVM = new BudgetPlanVersionPeriodVM();
                    m_objBudgetPlanVM.BudgetPlanVersionPeriodID = m_drDBudgetPlanVersionPeriodDA[BudgetPlanVersionPeriodVM.Prop.BudgetPlanVersionPeriodID.Name].ToString();
                    m_objBudgetPlanVM.BudgetPlanID = m_drDBudgetPlanVersionPeriodDA[BudgetPlanVersionPeriodVM.Prop.BudgetPlanID.Name].ToString();
                    m_objBudgetPlanVM.BudgetPlanVersion = int.Parse(m_drDBudgetPlanVersionPeriodDA[BudgetPlanVersionPeriodVM.Prop.BudgetPlanVersion.Name].ToString());
                    m_objBudgetPlanVM.StatusID = int.Parse(m_drDBudgetPlanVersionPeriodDA[BudgetPlanVersionPeriodVM.Prop.StatusID.Name].ToString());
                    m_objBudgetPlanVM.StatusDesc = m_drDBudgetPlanVersionPeriodDA[BudgetPlanVersionPeriodVM.Prop.StatusDesc.Name].ToString();
                    m_objBudgetPlanVM.BudgetPlanPeriodID = m_drDBudgetPlanVersionPeriodDA[BudgetPlanVersionPeriodVM.Prop.BudgetPlanPeriodID.Name].ToString();
                    m_objBudgetPlanVM.BudgetPlanPeriodDesc = m_drDBudgetPlanVersionPeriodDA[BudgetPlanVersionPeriodVM.Prop.BudgetPlanPeriodDesc.Name].ToString();
                    m_objBudgetPlanVM.PeriodVersion = int.Parse(m_drDBudgetPlanVersionPeriodDA[BudgetPlanVersionPeriodVM.Prop.PeriodVersion.Name].ToString());
                    //m_objBudgetPlanVM.ListBudgetPlanVersionVendorVM = GetListBudgetPlanVersionVendor(m_drDBudgetPlanVersionPeriodDA[BudgetPlanVersionPeriodVM.Prop.BudgetPlanVersionPeriodID.Name].ToString(), ref message);
                    m_lsDBudgetPlanVersionPeriodVM.Add(m_objBudgetPlanVM);
                }
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDBudgetPlanVersionPeriodDA.Message;


            return m_lsDBudgetPlanVersionPeriodVM;

        }
        private List<BudgetPlanVersionVendorVM> GetListBudgetPlanVersionVendor(List<string> BudgetPlanVersionPeriodIDs, ref string message)
        {
            List<BudgetPlanVersionVendorVM> m_lstBudgetPlanVersionVendorVM = new List<BudgetPlanVersionVendorVM>();
            DBudgetPlanVersionVendorDA m_objDBudgetPlanVersionVendorDA = new DBudgetPlanVersionVendorDA();
            m_objDBudgetPlanVersionVendorDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionPeriodID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionVendorID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.FirstName.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.LastName.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.StartDate.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.EndDate.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.StatusVendorID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanPeriodID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.Email.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.ScheduleID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.Description.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.ProjectID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVendorVM.Prop.ClusterID.MapAlias);


            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(string.Join(",", BudgetPlanVersionPeriodIDs));
            m_objFilter.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionPeriodID.Map, m_lstFilter);

            //m_lstFilter = new List<object>();
            //m_lstFilter.Add(Operator.Equals);
            //m_lstFilter.Add(budgetPlanVersion);
            //m_objFilter.Add(BudgetPlanVersionVendorVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

            //if (periodVersion != "")
            //{
            //    m_lstFilter = new List<object>();
            //    m_lstFilter.Add(Operator.Equals);
            //    m_lstFilter.Add(periodVersion);
            //    m_objFilter.Add(BudgetPlanVersionPeriodVM.Prop.PeriodVersion.Map, m_lstFilter);
            //}

            Dictionary<int, DataSet> m_dicTBudgetPlanDA = m_objDBudgetPlanVersionVendorDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objDBudgetPlanVersionVendorDA.Success)
            {
                foreach (DataRow m_drTBudgetPlanDA in m_dicTBudgetPlanDA[0].Tables[0].Rows)
                {
                    BudgetPlanVersionVendorVM m_objBudgetPlanVM = new BudgetPlanVersionVendorVM();
                    m_objBudgetPlanVM.BudgetPlanVersionPeriodID = m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionPeriodID.Name].ToString();
                    m_objBudgetPlanVM.BudgetPlanVersionVendorID = m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.BudgetPlanVersionVendorID.Name].ToString();
                    m_objBudgetPlanVM.VendorID = m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.VendorID.Name].ToString();
                    m_objBudgetPlanVM.FirstName = m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.FirstName.Name].ToString();
                    m_objBudgetPlanVM.LastName = m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.LastName.Name].ToString();
                    m_objBudgetPlanVM.StartDate = DateTime.Parse(m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.StartDate.Name].ToString());
                    m_objBudgetPlanVM.EndDate = DateTime.Parse(m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.EndDate.Name].ToString());
                    m_objBudgetPlanVM.StatusID = (int)m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.StatusVendorID.Name];
                    //m_objBudgetPlanVM.AllowDelete = !string.IsNullOrEmpty(m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.AvailableVendorID.Name].ToString()) ? false : true;
                    m_objBudgetPlanVM.StartDateHours = DateTime.Parse(m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.StartDate.Name].ToString()).ToString(Global.ShortTimeFormat);
                    m_objBudgetPlanVM.EndDateHours = DateTime.Parse(m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.EndDate.Name].ToString()).ToString(Global.ShortTimeFormat);
                    m_objBudgetPlanVM.BudgetPlanPeriodID = m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.BudgetPlanPeriodID.Name].ToString();
                    m_objBudgetPlanVM.Email = m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.Email.Name].ToString();
                    m_objBudgetPlanVM.ScheduleID = m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.ScheduleID.Name].ToString();
                    m_objBudgetPlanVM.Description = m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.Description.Name].ToString();
                    m_objBudgetPlanVM.BudgetPlanID = m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.BudgetPlanID.Name].ToString();
                    m_objBudgetPlanVM.ProjectID = m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.ProjectID.Name].ToString();
                    m_objBudgetPlanVM.ClusterID = m_drTBudgetPlanDA[BudgetPlanVersionVendorVM.Prop.ClusterID.Name].ToString();
                    m_lstBudgetPlanVersionVendorVM.Add(m_objBudgetPlanVM);
                }
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objDBudgetPlanVersionVendorDA.Message;

            return m_lstBudgetPlanVersionVendorVM;

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