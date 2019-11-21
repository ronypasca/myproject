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
using System.Web.Script.Serialization;
using XMVC = Ext.Net.MVC;

namespace com.SML.BIGTRONS.Controllers
{
    public class EmployeeController : BaseController
    {
        private readonly string title = "Employee";
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
            MEmployeeDA m_objMEmployeeDA = new MEmployeeDA();
            m_objMEmployeeDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcMEmployee = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMEmployee.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = EmployeeVM.Prop.Map(m_strDataIndex, false);
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
            Dictionary<int, DataSet> m_dicMEmployeeDA = m_objMEmployeeDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
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
                m_lstSelect.Add(EmployeeVM.Prop.EmployeeID.MapAlias);
                m_lstSelect.Add(EmployeeVM.Prop.EmployeeName.MapAlias);
                m_lstSelect.Add(EmployeeVM.Prop.CompanyDesc.MapAlias);
                m_lstSelect.Add(EmployeeVM.Prop.OrgDivisionDesc.MapAlias);
                m_lstSelect.Add(EmployeeVM.Prop.OrgDepartmentDesc.MapAlias);
                m_lstSelect.Add(EmployeeVM.Prop.OrgSectionDesc.MapAlias);
                m_lstSelect.Add(EmployeeVM.Prop.PositionDesc.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(EmployeeVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                m_dicMEmployeeDA = m_objMEmployeeDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objMEmployeeDA.Message == string.Empty)
                {
                    m_lstEmployeeVM = (
                        from DataRow m_drMEmployeeDA in m_dicMEmployeeDA[0].Tables[0].Rows
                        select new EmployeeVM()
                        {
                            EmployeeID = m_drMEmployeeDA[EmployeeVM.Prop.EmployeeID.Name].ToString(),
                            EmployeeName = m_drMEmployeeDA[EmployeeVM.Prop.EmployeeName.Name].ToString(),
                            CompanyDesc = m_drMEmployeeDA[EmployeeVM.Prop.CompanyDesc.Name].ToString(),
                            OrgDivisionDesc = m_drMEmployeeDA[EmployeeVM.Prop.OrgDivisionDesc.Name].ToString(),
                            OrgDepartmentDesc = m_drMEmployeeDA[EmployeeVM.Prop.OrgDepartmentDesc.Name].ToString(),
                            OrgSectionDesc = m_drMEmployeeDA[EmployeeVM.Prop.OrgSectionDesc.Name].ToString(),
                            PositionDesc = m_drMEmployeeDA[EmployeeVM.Prop.PositionDesc.Name].ToString(),
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstEmployeeVM, m_intCount);
        }

        public ActionResult ReadBrowse(StoreRequestParameters parameters,string TCTypeID, string TCTypeParentID, string FromScheduling="")
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

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(EmployeeVM.Prop.EmployeeID.MapAlias);
            m_lstSelect.Add(EmployeeVM.Prop.EmployeeName.MapAlias);
            m_lstSelect.Add(EmployeeVM.Prop.CompanyDesc.MapAlias);
            m_lstSelect.Add(EmployeeVM.Prop.OrgDivisionDesc.MapAlias);
            m_lstSelect.Add(EmployeeVM.Prop.OrgDepartmentDesc.MapAlias);
            m_lstSelect.Add(EmployeeVM.Prop.OrgSectionDesc.MapAlias);
            m_lstSelect.Add(EmployeeVM.Prop.PositionDesc.MapAlias);
            m_lstSelect.Add(EmployeeCommunicationVM.Prop.CommunicationDesc.MapAlias);

            List<string> m_lstGroup = new List<string>();
            m_lstGroup.Add(EmployeeVM.Prop.EmployeeID.Map);
            m_lstGroup.Add(EmployeeVM.Prop.EmployeeName.Map);
            m_lstGroup.Add(EmployeeVM.Prop.CompanyDesc.Map);
            m_lstGroup.Add(EmployeeVM.Prop.OrgDivisionDesc.Map);
            m_lstGroup.Add(EmployeeVM.Prop.OrgDepartmentDesc.Map);
            m_lstGroup.Add(EmployeeVM.Prop.OrgSectionDesc.Map);
            m_lstGroup.Add(EmployeeVM.Prop.PositionDesc.Map);
            m_lstGroup.Add(EmployeeCommunicationVM.Prop.CommunicationDesc.Map);

            if (!string.IsNullOrEmpty(TCTypeParentID))
            {
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(TCTypeParentID);
                m_objFilter.Add(TCTypesVM.Prop.TCTypeID.Name, m_lstFilter);
            }

            //if (!string.IsNullOrEmpty(TCTypeParentID))
            //{
            //    m_lstFilter.Add(Operator.None);
            //    m_lstFilter.Add("");
            //    m_objFilter.Add(string.Format("1=1 OR {0}='{1}'",TCTypesVM.Prop.TCTypeParentID.Name,TCTypeParentID), m_lstFilter);
            //}
            Dictionary<int, DataSet> m_dicMEmployeeDA = new Dictionary<int, DataSet>();
            if (FromScheduling.Length > 0)
                m_dicMEmployeeDA = m_objMEmployeeDA.SelectBCRecipients(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            else
               m_dicMEmployeeDA = m_objMEmployeeDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, m_lstGroup, null, null);

            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpEmployeeBL in m_dicMEmployeeDA)
            {
                m_intCount = m_kvpEmployeeBL.Key;
                break;
            }
            
            List<EmployeeVM> m_lstEmployeeVM = new List<EmployeeVM>();
            if (m_intCount > 0)
            {
                //TODO UNION
                m_boolIsCount = false;
                

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(EmployeeVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

                if (FromScheduling.Length > 0)
                {
                    m_lstSelect = new List<string>();
                    m_lstSelect.Add(EmployeeVM.Prop.EmployeeID.MapAlias);
                    m_lstSelect.Add(EmployeeVM.Prop.EmployeeName.MapAlias);
                    m_lstSelect.Add(EmployeeVM.Prop.Email.MapAlias);
                    m_lstSelect.Add(EmployeeVM.Prop.OwnerID.MapAlias);
                    m_dicMEmployeeDA = m_objMEmployeeDA.SelectBCRecipients(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, null, null);
                }
                else
                    m_dicMEmployeeDA = m_objMEmployeeDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, m_lstGroup, m_dicOrder, null);

                if (m_objMEmployeeDA.Message == string.Empty)
                {
                    if (FromScheduling.Length > 0)
                    {
                        m_lstEmployeeVM = (
                          from DataRow m_drMEmployeeDA in m_dicMEmployeeDA[0].Tables[0].Rows
                          select new EmployeeVM()
                          {
                              OwnerID = m_drMEmployeeDA[EmployeeVM.Prop.OwnerID.Name].ToString(),
                              EmployeeID = m_drMEmployeeDA[EmployeeVM.Prop.EmployeeID.Name].ToString(),
                              EmployeeName = m_drMEmployeeDA[EmployeeVM.Prop.EmployeeName.Name].ToString(),
                              Email = FromScheduling.Length > 0 ? m_drMEmployeeDA[EmployeeVM.Prop.Email.Name].ToString() : ""
                          }
                      ).ToList();
                    }
                    else
                    {
                        m_lstEmployeeVM = (
                            from DataRow m_drMEmployeeDA in m_dicMEmployeeDA[0].Tables[0].Rows
                            select new EmployeeVM()
                            {
                                EmployeeID = m_drMEmployeeDA[EmployeeVM.Prop.EmployeeID.Name].ToString(),
                                EmployeeName = m_drMEmployeeDA[EmployeeVM.Prop.EmployeeName.Name].ToString(),
                                CompanyDesc = m_drMEmployeeDA[EmployeeVM.Prop.CompanyDesc.Name].ToString(),
                                OrgDivisionDesc = m_drMEmployeeDA[EmployeeVM.Prop.OrgDivisionDesc.Name].ToString(),
                                OrgDepartmentDesc = m_drMEmployeeDA[EmployeeVM.Prop.OrgDepartmentDesc.Name].ToString(),
                                OrgSectionDesc = m_drMEmployeeDA[EmployeeVM.Prop.OrgSectionDesc.Name].ToString(),
                                PositionDesc = m_drMEmployeeDA[EmployeeVM.Prop.PositionDesc.Name].ToString(),
                                OwnerID="",
                                Email = m_drMEmployeeDA[EmployeeCommunicationVM.Prop.CommunicationDesc.Name].ToString()
                            }
                        ).ToList();
                    }
                }
            }
            return this.Store(m_lstEmployeeVM, m_intCount);
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

            EmployeeVM m_objEmployeeVM = new EmployeeVM();
            ViewDataDictionary m_vddEmployee = new ViewDataDictionary();
            m_vddEmployee.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddEmployee.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonAdd));
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
                Model = m_objEmployeeVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddEmployee,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Detail(string Caller, string Selected)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            EmployeeVM m_objEmployeeVM = new EmployeeVM();
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
            }

            if (m_dicSelectedRow.Count > 0)
                m_objEmployeeVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
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
                Model = m_objEmployeeVM,
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

            string m_strMessage = string.Empty;
            EmployeeVM m_objEmployeeVM = new EmployeeVM();
            if (m_dicSelectedRow.Count > 0)
                m_objEmployeeVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            ViewDataDictionary m_vddEmployee = new ViewDataDictionary();
            m_vddEmployee.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddEmployee.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objEmployeeVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddEmployee,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }

        public ActionResult Delete(string Selected)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<EmployeeVM> m_lstSelectedRow = new List<EmployeeVM>();
            m_lstSelectedRow = JSON.Deserialize<List<EmployeeVM>>(Selected);

            MEmployeeDA m_objMEmployeeDA = new MEmployeeDA();
            m_objMEmployeeDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (EmployeeVM m_objEmployeeVM in m_lstSelectedRow)
                {
                    List<string> m_lstKey = new List<string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifEmployeeVM = m_objEmployeeVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifEmployeeVM in m_arrPifEmployeeVM)
                    {
                        string m_strFieldName = m_pifEmployeeVM.Name;
                        object m_objFieldValue = m_pifEmployeeVM.GetValue(m_objEmployeeVM);
                        if (m_objEmployeeVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(EmployeeVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_objMEmployeeDA.DeleteBC(m_objFilter, false);
                    if (m_objMEmployeeDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMEmployeeDA.Message);
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

        public ActionResult Browse(string ControlEmployeeID, string ControlEmployeeName,string ControlEmail, string GrdScheduleRecipient, string FilterEmployeeID = "", string FilterEmployeeName = "", string FilterTCTypeID = "", string FilterTCTypeParentID = "",string RecipientTypeID="")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddEmployee = new ViewDataDictionary();
            m_vddEmployee.Add("Control" + EmployeeVM.Prop.EmployeeID.Name, ControlEmployeeID);
            m_vddEmployee.Add("Control" + EmployeeVM.Prop.EmployeeName.Name, ControlEmployeeName);
            m_vddEmployee.Add("Control" + EmployeeVM.Prop.Email.Name, ControlEmail);
            m_vddEmployee.Add(EmployeeVM.Prop.EmployeeID.Name, FilterEmployeeID);
            m_vddEmployee.Add(EmployeeVM.Prop.EmployeeName.Name, FilterEmployeeName);
            m_vddEmployee.Add(TCMembersVM.Prop.TCTypeID.Name, FilterTCTypeID);
            m_vddEmployee.Add(TCTypesVM.Prop.TCTypeParentID.Name, FilterTCTypeParentID);
            m_vddEmployee.Add(RecipientsVM.Prop.RecipientTypeID.Name, RecipientTypeID);
            if (GrdScheduleRecipient!= null)
                m_vddEmployee.Add("ControlGrdRecipientList", GrdScheduleRecipient);
            else
                m_vddEmployee.Add("ControlGrdRecipientList", "");

            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddEmployee,
                ViewName = "../Employee/_Browse"
            };
        }
       
        /// <summary>
        /// Disabled Save
        /// </summary>
        /// <param name="Action"></param>
        /// <returns></returns>
        public ActionResult Save(string Action)
        {
            /*if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            MEmployeeDA m_objMEmployeeDA = new MEmployeeDA();
            m_objMEmployeeDA.ConnectionStringName = Global.ConnStrConfigName;
            try
            {
                string m_strEmployeeID = this.Request.Params[EmployeeVM.Prop.EmployeeID.Name];
                string m_strEmployeeSubcategoryID = this.Request.Params[EmployeeSubcategoryVM.Prop.EmployeeSubcategoryID.Name];
                string m_strFirstName = this.Request.Params[EmployeeVM.Prop.FirstName.Name];
                string m_strLastName = this.Request.Params[EmployeeVM.Prop.LastName.Name];
                string m_strCity = this.Request.Params[EmployeeVM.Prop.City.Name];
                string m_strStreet = this.Request.Params[EmployeeVM.Prop.Street.Name];
                string m_strPostal = this.Request.Params[EmployeeVM.Prop.Postal.Name];
                string m_strPhone = this.Request.Params[EmployeeVM.Prop.Phone.Name];
                string m_strEmail = this.Request.Params[EmployeeVM.Prop.Email.Name];
                string m_strIDNo = this.Request.Params[EmployeeVM.Prop.IDNo.Name];
                string m_strNPWP = this.Request.Params[EmployeeVM.Prop.NPWP.Name];

                m_lstMessage = IsSaveValid(Action, m_strEmployeeID, m_strEmployeeSubcategoryID, m_strFirstName,
                    m_strLastName, m_strCity, m_strStreet, m_strPostal, m_strPhone, m_strEmail, m_strIDNo, m_strNPWP);
                if (m_lstMessage.Count <= 0)
                {
                    MEmployee m_objMEmployee = new MEmployee();
                    m_objMEmployee.EmployeeID = m_strEmployeeID;
                    m_objMEmployeeDA.Data = m_objMEmployee;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objMEmployeeDA.Select();

                    m_objMEmployee.FirstName = m_strFirstName;
                    m_objMEmployee.LastName = m_strLastName;
                    m_objMEmployee.City = m_strCity;
                    m_objMEmployee.Street = m_strStreet;
                    m_objMEmployee.Postal = m_strPostal;
                    m_objMEmployee.Phone = m_strPhone;
                    m_objMEmployee.Email = m_strEmail;
                    m_objMEmployee.IDNo = m_strIDNo;
                    m_objMEmployee.NPWP = m_strNPWP;
                    m_objMEmployee.EmployeeSubcategoryID = m_strEmployeeSubcategoryID;

                    if (Action == General.EnumDesc(Buttons.ButtonAdd))
                        m_objMEmployeeDA.Insert(false);
                    else
                        m_objMEmployeeDA.Update(false);

                    if (!m_objMEmployeeDA.Success || m_objMEmployeeDA.Message != string.Empty)
                        m_lstMessage.Add(m_objMEmployeeDA.Message);
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
            }
            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
                return Detail(General.EnumDesc(Buttons.ButtonSave), null);
            }
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            */
            return this.Direct();
        }

        #endregion

        #region Direct Method

        public ActionResult GetEmployee(string ControlEmployeeID, string ControlEmployeeName, string FilterEmployeeID, string FilterEmployeeDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<EmployeeVM>> m_dicEmployeeData = GetEmployeeData(true, FilterEmployeeID, FilterEmployeeDesc);
                KeyValuePair<int, List<EmployeeVM>> m_kvpEmployeeVM = m_dicEmployeeData.AsEnumerable().ToList()[0];
                if (m_kvpEmployeeVM.Key < 1 || (m_kvpEmployeeVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpEmployeeVM.Key > 1 && !Exact)
                    return Browse(ControlEmployeeID, ControlEmployeeName,null, FilterEmployeeID, FilterEmployeeDesc);

                m_dicEmployeeData = GetEmployeeData(false, FilterEmployeeID, FilterEmployeeDesc);
                EmployeeVM m_objEmployeeVM = m_dicEmployeeData[0][0];
                this.GetCmp<TextField>(ControlEmployeeID).Value = m_objEmployeeVM.EmployeeID;
                this.GetCmp<TextField>(ControlEmployeeName).Value = m_objEmployeeVM.EmployeeName;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method

        private List<string> IsSaveValid(string Action, string EmployeeID, string EmployeeSubcategoryID, string FirstName,
            string LastName, string City, string Street, string Postal, string Phone, string Email, string IDNo,
            string NPWP)
        {
            List<string> m_lstReturn = new List<string>();

            //if (EmployeeID == string.Empty)
            //    m_lstReturn.Add(EmployeeVM.Prop.EmployeeID.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            //if (FirstName == string.Empty)
            //    m_lstReturn.Add(EmployeeVM.Prop.FirstName.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            //if (LastName == string.Empty)
            //    m_lstReturn.Add(EmployeeVM.Prop.LastName.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            //if (EmployeeSubcategoryID == string.Empty)
            //    m_lstReturn.Add(EmployeeVM.Prop.EmployeeSubcategoryDesc.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            //if (City == string.Empty)
            //    m_lstReturn.Add(EmployeeVM.Prop.City.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            //if (Street == string.Empty)
            //    m_lstReturn.Add(EmployeeVM.Prop.Street.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            //if (Postal == string.Empty)
            //    m_lstReturn.Add(EmployeeVM.Prop.Postal.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            //if (Phone == string.Empty)
            //    m_lstReturn.Add(EmployeeVM.Prop.Phone.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            //if (Email == string.Empty)
            //    m_lstReturn.Add(EmployeeVM.Prop.Email.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            //else if (!General.IsEmailValid(Email))
            //    m_lstReturn.Add(EmployeeVM.Prop.Email.Desc + " " + General.EnumDesc(MessageLib.invalid));
            //if (IDNo == string.Empty)
            //    m_lstReturn.Add(EmployeeVM.Prop.IDNo.Desc + " " + General.EnumDesc(MessageLib.mustFill));
            //if (NPWP == string.Empty)
            //    m_lstReturn.Add(EmployeeVM.Prop.NPWP.Desc + " " + General.EnumDesc(MessageLib.mustFill));

            return m_lstReturn;
        }

        private Dictionary<string, object> GetFormData(NameValueCollection parameters)
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();

            m_dicReturn.Add(EmployeeVM.Prop.EmployeeID.Name, parameters[EmployeeVM.Prop.EmployeeID.Name]);
            m_dicReturn.Add(EmployeeVM.Prop.EmployeeName.Name, parameters[EmployeeVM.Prop.EmployeeName.Name]);
            m_dicReturn.Add(EmployeeVM.Prop.CompanyDesc.Name, parameters[EmployeeVM.Prop.CompanyDesc.Name]);
            m_dicReturn.Add(EmployeeVM.Prop.PersonnelAreaDesc.Name, parameters[EmployeeVM.Prop.PersonnelAreaDesc.Name]);
            m_dicReturn.Add(EmployeeVM.Prop.PersonnelSubareaDesc.Name, parameters[EmployeeVM.Prop.PersonnelSubareaDesc.Name]);
            m_dicReturn.Add(EmployeeVM.Prop.OrgBusinessUnitDesc.Name, parameters[EmployeeVM.Prop.OrgBusinessUnitDesc.Name]);
            m_dicReturn.Add(EmployeeVM.Prop.OrgDivisionDesc.Name, parameters[EmployeeVM.Prop.OrgDivisionDesc.Name]);
            m_dicReturn.Add(EmployeeVM.Prop.OrgDepartmentDesc.Name, parameters[EmployeeVM.Prop.OrgDepartmentDesc.Name]);
            m_dicReturn.Add(EmployeeVM.Prop.OrgSectionDesc.Name, parameters[EmployeeVM.Prop.OrgSectionDesc.Name]);
            m_dicReturn.Add(EmployeeVM.Prop.PositionDesc.Name, parameters[EmployeeVM.Prop.PositionDesc.Name]);
            m_dicReturn.Add(EmployeeVM.Prop.Email.Name, parameters[EmployeeVM.Prop.Email.Name]);

            return m_dicReturn;
        }

        private EmployeeVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            EmployeeVM m_objEmployeeVM = new EmployeeVM();
            MEmployeeDA m_objMEmployeeDA = new MEmployeeDA();
            m_objMEmployeeDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(EmployeeVM.Prop.EmployeeID.MapAlias);
            m_lstSelect.Add(EmployeeVM.Prop.EmployeeName.MapAlias);
            m_lstSelect.Add(EmployeeVM.Prop.CompanyDesc.MapAlias);
            m_lstSelect.Add(EmployeeVM.Prop.PersonnelAreaDesc.MapAlias);
            m_lstSelect.Add(EmployeeVM.Prop.PersonnelSubareaDesc.MapAlias);
            m_lstSelect.Add(EmployeeVM.Prop.OrgBusinessUnitDesc.MapAlias);
            m_lstSelect.Add(EmployeeVM.Prop.OrgDepartmentDesc.MapAlias);
            m_lstSelect.Add(EmployeeVM.Prop.OrgDivisionDesc.MapAlias);
            m_lstSelect.Add(EmployeeVM.Prop.OrgSectionDesc.MapAlias);
            m_lstSelect.Add(EmployeeCommunicationVM.Prop.CommunicationDesc.MapAlias);
            m_lstSelect.Add(EmployeeVM.Prop.PositionDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objEmployeeVM.IsKey(m_kvpSelectedRow.Key))
                {
                    m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                    List<object> m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(m_kvpSelectedRow.Value);
                    m_objFilter.Add(EmployeeVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                }
            }

            Dictionary<int, DataSet> m_dicMEmployeeDA = m_objMEmployeeDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMEmployeeDA.Message == string.Empty)
            {
                DataRow m_drMEmployeeDA = m_dicMEmployeeDA[0].Tables[0].Rows[0];
                m_objEmployeeVM.EmployeeID = m_drMEmployeeDA[EmployeeVM.Prop.EmployeeID.Name].ToString();
                m_objEmployeeVM.EmployeeName = m_drMEmployeeDA[EmployeeVM.Prop.EmployeeName.Name].ToString();
                m_objEmployeeVM.CompanyDesc = m_drMEmployeeDA[EmployeeVM.Prop.CompanyDesc.Name].ToString();
                m_objEmployeeVM.PersonnelAreaDesc = m_drMEmployeeDA[EmployeeVM.Prop.PersonnelAreaDesc.Name].ToString();
                m_objEmployeeVM.PersonnelSubareaDesc = m_drMEmployeeDA[EmployeeVM.Prop.PersonnelSubareaDesc.Name].ToString();
                m_objEmployeeVM.OrgBusinessUnitDesc = m_drMEmployeeDA[EmployeeVM.Prop.OrgBusinessUnitDesc.Name].ToString();
                m_objEmployeeVM.OrgDepartmentDesc = m_drMEmployeeDA[EmployeeVM.Prop.OrgDepartmentDesc.Name].ToString();
                m_objEmployeeVM.OrgDivisionDesc = m_drMEmployeeDA[EmployeeVM.Prop.OrgDivisionDesc.Name].ToString();
                m_objEmployeeVM.OrgSectionDesc = m_drMEmployeeDA[EmployeeVM.Prop.OrgSectionDesc.Name].ToString();
                m_objEmployeeVM.Email = m_drMEmployeeDA[EmployeeCommunicationVM.Prop.CommunicationDesc.Name].ToString();
                m_objEmployeeVM.PositionDesc = m_drMEmployeeDA[EmployeeVM.Prop.PositionDesc.Name].ToString();
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objMEmployeeDA.Message;

            return m_objEmployeeVM;
        }

        #endregion

        #region Public Method

        public Dictionary<int, List<EmployeeVM>> GetEmployeeData(bool isCount, string EmployeeID, string EmployeeDesc)
        {
            int m_intCount = 0;
            List<EmployeeVM> m_lstEmployeeVM = new List<ViewModels.EmployeeVM>();
            Dictionary<int, List<EmployeeVM>> m_dicReturn = new Dictionary<int, List<EmployeeVM>>();
            MEmployeeDA m_objMEmployeeDA = new MEmployeeDA();
            m_objMEmployeeDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(EmployeeVM.Prop.EmployeeID.MapAlias);
            m_lstSelect.Add(EmployeeVM.Prop.EmployeeName.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(EmployeeID);
            m_objFilter.Add(EmployeeVM.Prop.EmployeeID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(EmployeeDesc);
            m_objFilter.Add(EmployeeVM.Prop.EmployeeName.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMEmployeeDA = m_objMEmployeeDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMEmployeeDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpEmployeeBL in m_dicMEmployeeDA)
                    {
                        m_intCount = m_kvpEmployeeBL.Key;
                        break;
                    }
                else
                {
                    m_lstEmployeeVM = (
                        from DataRow m_drMEmployeeDA in m_dicMEmployeeDA[0].Tables[0].Rows
                        select new EmployeeVM()
                        {
                            EmployeeID = m_drMEmployeeDA[EmployeeVM.Prop.EmployeeID.Name].ToString(),
                            EmployeeName = m_drMEmployeeDA[EmployeeVM.Prop.EmployeeName.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstEmployeeVM);
            return m_dicReturn;
        }

        #endregion
    }
}