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
    public class BudgetPlanVersionAssignmentController : BaseController
    {
        private readonly string title = "Assign Budget Plan";
        private readonly string dataSessionName = "FormData";

        #region Public Action
        public ActionResult Index()
        {
            base.Initialize();
            ViewBag.Title = "Budget Plan Vendor Assignment";
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
            m_lstFilter.Add("DBudgetPlanVersion");
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
        public ActionResult Read(StoreRequestParameters parameters)
        {
            TBudgetPlanDA m_objTBudgetPlanDA = new TBudgetPlanDA();
            m_objTBudgetPlanDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcTBudgetPlan = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcTBudgetPlan.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = BudgetPlanVM.Prop.Map(m_strDataIndex, false);
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
           

            List<BudgetPlanVM> m_lstBudgetPlanVM = new List<BudgetPlanVM>();
            
            m_boolIsCount = false;
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanTemplateDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanVersion.MapAlias);
            m_lstSelect.Add(string.Format("(select max(a.BudgetPlanVersion) from DBudgetPlanVersion a where a.BudgetPlanID = DBudgetPlanVersion.BudgetPlanID) as {0}", BudgetPlanVM.Prop.MaxBudgetPlanVersion.Name));
            //m_lstSelect.Add(string.Format("MAX({0}) OVER(PARTITION BY {1}) AS {2}", BudgetPlanVersionVM.Prop.BudgetPlanVersion.Map,BudgetPlanVM.Prop.BudgetPlanID.Map, BudgetPlanVM.Prop.MaxBudgetPlanVersion.Name));
            m_lstSelect.Add(BudgetPlanVM.Prop.Description.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.ProjectDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.ClusterDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.UnitTypeDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.StatusDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.StatusID.MapAlias);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            foreach (DataSorter m_dtsOrder in parameters.Sort)
                m_dicOrder.Add(BudgetPlanVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));


            Dictionary<int, DataSet> m_dicTBudgetPlanDA = m_objTBudgetPlanDA.SelectBC(0, null, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
            if (m_objTBudgetPlanDA.Message == string.Empty)
            {
                m_lstBudgetPlanVM = (
                    from DataRow m_drTBudgetPlanDA in m_dicTBudgetPlanDA[0].Tables[0].Rows
                    select new BudgetPlanVM()
                    {
                        BudgetPlanID = m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanID.Name].ToString(),
                        BudgetPlanTemplateDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanTemplateDesc.Name].ToString(),
                        BudgetPlanVersion = int.Parse(m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanVersion.Name].ToString()),
                        Description = m_drTBudgetPlanDA[BudgetPlanVM.Prop.Description.Name].ToString(),
                        ProjectDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.ProjectDesc.Name].ToString(),
                        ClusterDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.ClusterDesc.Name].ToString(),
                        UnitTypeDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.UnitTypeDesc.Name].ToString(),
                        StatusDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.StatusDesc.Name].ToString(),
                        StatusID = int.Parse(m_drTBudgetPlanDA[BudgetPlanVM.Prop.StatusID.Name].ToString()),
                        MaxBudgetPlanVersion = int.Parse(m_drTBudgetPlanDA[BudgetPlanVM.Prop.MaxBudgetPlanVersion.Name].ToString())
                    }
                ).Distinct().ToList();
            }
            

            List<BudgetPlanVM> listBudgetPlan = (m_lstBudgetPlanVM.Any() ?
                m_lstBudgetPlanVM.Where(d => d.MaxBudgetPlanVersion == d.BudgetPlanVersion).ToList() : new List<BudgetPlanVM>());
            listBudgetPlan = listBudgetPlan.Where(x => x.StatusID == 2).ToList();
            List<BudgetPlanVM> m_lstBudgetPlan = listBudgetPlan.Skip(m_intSkip).Take(m_intLength).ToList();
            
            return this.Store(m_lstBudgetPlan, listBudgetPlan.Count());
        }
        public ActionResult Detail(string Caller, string Selected, string BudgetPlanVersion, string BudgetPlanID)
        {
            Global.HasAccess = GetHasAccess();
            if (!Global.HasAccess.Read)
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));


            string m_strMessage = string.Empty;
            BudgetPlanVM m_objBudgetPlanVM = new BudgetPlanVM();
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
                m_dicSelectedRow = GetFormData(m_nvcParams, BudgetPlanID);
            }

            if (m_dicSelectedRow.Count > 0)
                m_objBudgetPlanVM = GetSelectedData(m_dicSelectedRow, BudgetPlanVersion, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            m_objBudgetPlanVM.ListBudgetPlanVersionStructureVM = new List<BudgetPlanVersionStructureVM>();

            ViewDataDictionary m_vddBudgetPlan = new ViewDataDictionary();
            m_vddBudgetPlan.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            m_vddBudgetPlan.Add(BudgetPlanVersionVM.Prop.BudgetPlanVersion.Name, m_objBudgetPlanVM.BudgetPlanVersion.ToString());
            m_vddBudgetPlan.Add(BudgetPlanVM.Prop.BudgetPlanID.Name, m_objBudgetPlanVM.BudgetPlanID.ToString());
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objBudgetPlanVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddBudgetPlan,
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
                m_dicSelectedRow = GetFormData(m_nvcParams, string.Empty);
                Session[dataSessionName] = JSON.Serialize(m_dicSelectedRow);
            }

            string m_strMessage = string.Empty;
            BudgetPlanVM m_objBudgetPlanVM = new BudgetPlanVM();

            if (m_dicSelectedRow.Count > 0)
                m_objBudgetPlanVM = GetSelectedData(m_dicSelectedRow, string.Empty, ref m_strMessage);

           
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            m_objBudgetPlanVM.ListBudgetPlanVersionStructureVM = new List<BudgetPlanVersionStructureVM>();


            ViewDataDictionary m_vddBudgetPlan = new ViewDataDictionary();
            m_vddBudgetPlan.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddBudgetPlan.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            m_vddBudgetPlan.Add(BudgetPlanVersionVM.Prop.BudgetPlanVersion.Name, m_objBudgetPlanVM.BudgetPlanVersion.ToString());
            m_vddBudgetPlan.Add(BudgetPlanVM.Prop.BudgetPlanID.Name, m_objBudgetPlanVM.BudgetPlanID.ToString());
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objBudgetPlanVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddBudgetPlan,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult Save(string Action)
        {

            if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
                : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<BudgetPlanVersionStructureVM> m_lstListBudgetPlanVersionStructureVM = JSON.Deserialize<List<BudgetPlanVersionStructureVM>>(this.Request.Params[BudgetPlanVM.Prop.ListBudgetPlanVersionStructureVM.Name]);
            List<BudgetPlanVersionAssignmentVM> m_lstListBudgetPlanVersionAssignmentVM = new List<BudgetPlanVersionAssignmentVM>();
            foreach (var m_BudgetPlanVersionStructureVM in m_lstListBudgetPlanVersionStructureVM.Where(x => x.BudgetPlanVersionStructureID != "" && x.BudgetPlanVersionStructureID != null))
            {
                m_lstListBudgetPlanVersionAssignmentVM.Add(new BudgetPlanVersionAssignmentVM() {
                    BudgetPlanVersionStructureID = m_BudgetPlanVersionStructureVM.BudgetPlanVersionStructureID,
                    VendorID = m_BudgetPlanVersionStructureVM.VendorID
                });
            }
            bool m_boolNotComplete = !IsListBudgetPlanVersionStructureComplete(m_lstListBudgetPlanVersionStructureVM);
            bool m_boolAlreadyCreated = false;
            List<string> m_lstMessage = new List<string>();

            if (m_boolNotComplete) m_lstMessage.Add("There are structure have not assigned.");
            DBudgetPlanVersionAssignmentDA m_objDBudgetPlanVersionAssignmentDA = new DBudgetPlanVersionAssignmentDA();
                        
            string m_strBudgetPlanID = string.Empty;
            string m_strBudgetPlanVersion = string.Empty;
            m_objDBudgetPlanVersionAssignmentDA.ConnectionStringName = Global.ConnStrConfigName;

            string m_strTransName = "BudgetPlanVersionAssignmen";
            object m_objDBConnection = null;
            m_objDBConnection = m_objDBudgetPlanVersionAssignmentDA.BeginTrans(m_strTransName);
            string m_strMessage = string.Empty;

            foreach (BudgetPlanVersionStructureVM m_objBudgetPlanVersionStructureVM in m_lstListBudgetPlanVersionStructureVM)
            {
                if (!string.IsNullOrEmpty(m_objBudgetPlanVersionStructureVM.BudgetPlanVersionStructureID) && !string.IsNullOrEmpty(m_objBudgetPlanVersionStructureVM.VendorID))
                {
                    if (StructureAlreadyCreated(m_objBudgetPlanVersionStructureVM.VendorID, m_objBudgetPlanVersionStructureVM.BudgetPlanVersionStructureID))
                    {
                        m_lstMessage.Add("Can't change structure vendor, some entry already created");
                        m_boolAlreadyCreated = true;
                        break;
                    }
                }
            }

            if (!m_boolNotComplete && !m_boolAlreadyCreated)
            {
            try
                {
                    m_strBudgetPlanID = this.Request.Params[BudgetPlanVM.Prop.BudgetPlanID.Name];
                    m_strBudgetPlanVersion = this.Request.Params[BudgetPlanVM.Prop.BudgetPlanVersion.Name];

                    //DELETE
                    foreach (BudgetPlanVersionAssignmentVM m_BudgetPlanVersionAssignmentVMdelete in m_lstListBudgetPlanVersionAssignmentVM)
                    {
                        Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                        List<object> m_lstFilter = new List<object>();
                        m_lstFilter.Add(Operator.Equals);
                        m_lstFilter.Add(m_BudgetPlanVersionAssignmentVMdelete.BudgetPlanVersionStructureID);
                        m_objFilter.Add(BudgetPlanVersionAssignmentVM.Prop.BudgetPlanVersionStructureID.Map, m_lstFilter);
                        m_objDBudgetPlanVersionAssignmentDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                    }

                    if (m_objDBudgetPlanVersionAssignmentDA.Message == General.EnumDesc(MessageLib.NotExist)) m_objDBudgetPlanVersionAssignmentDA.Message = "";

                    //INSERT
                    foreach (BudgetPlanVersionAssignmentVM m_BudgetPlanVersionAssignmentVMinsert in m_lstListBudgetPlanVersionAssignmentVM)
                    {
                        if (m_BudgetPlanVersionAssignmentVMinsert.BudgetPlanVersionStructureID != null && m_BudgetPlanVersionAssignmentVMinsert.BudgetPlanVersionStructureID != "" && m_BudgetPlanVersionAssignmentVMinsert.VendorID != null && m_BudgetPlanVersionAssignmentVMinsert.VendorID != "")
                        {

                            DBudgetPlanVersionAssignment m_objDBudgetPlanVersionAssignment = new DBudgetPlanVersionAssignment()
                            {
                                BudgetPlanVersionStructureID = m_BudgetPlanVersionAssignmentVMinsert.BudgetPlanVersionStructureID,
                                VendorID = m_BudgetPlanVersionAssignmentVMinsert.VendorID
                            };

                            m_objDBudgetPlanVersionAssignmentDA.Data = m_objDBudgetPlanVersionAssignment;
                            m_objDBudgetPlanVersionAssignmentDA.Insert(true, m_objDBConnection);

                        }
                    }



                    if (!m_objDBudgetPlanVersionAssignmentDA.Success || m_objDBudgetPlanVersionAssignmentDA.Message != string.Empty)
                        m_lstMessage.Add(m_objDBudgetPlanVersionAssignmentDA.Message);
                    
                        m_objDBudgetPlanVersionAssignmentDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
                }
                catch (Exception ex)
                {
                    m_lstMessage.Add(ex.Message);
                    m_objDBudgetPlanVersionAssignmentDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
                }
            }
           
            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfoAlert(title, General.EnumDesc(Action == General.EnumDesc(Buttons.ButtonAdd) ? MessageLib.Added : MessageLib.Updated));
                return Detail(General.EnumDesc(Buttons.ButtonSave), null, m_strBudgetPlanVersion, m_strBudgetPlanID);
            }
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);
        }
        public ActionResult GetNodeBudgetPlanVersionStructure(string BudgetPlanID, string BudgetPlanVersion, string BudgetPlanTemplateID, string RegionID, string ProjectID, string ClusterID, string UnitTypeID, string IsLoadTemplate, decimal? FeeContractor, decimal? Area)
        {
           
            string m_strMessage = string.Empty;

            //Dictionary<string, bool> m_dicDisplayPrice = DisplayPrice();

            NodeCollection m_nodeCollectChild = new Ext.Net.NodeCollection();

            List<BudgetPlanVersionStructureVM> listBudgetPlanVersionStructureVM = GetListBudgetPlanStructure(BudgetPlanID, Convert.ToInt32(BudgetPlanVersion), ref m_strMessage);
                if (listBudgetPlanVersionStructureVM.Any())
                {


                    List<BudgetPlanVersionStructureVM> m_listBudgetPlanVersionStructureVM = listBudgetPlanVersionStructureVM.Where(d => d.ParentItemID == "0").Distinct().OrderBy(d => d.Sequence).ToList();

                    foreach (BudgetPlanVersionStructureVM item in m_listBudgetPlanVersionStructureVM)
                    {
                        NodeCollection m_nodeChildCollection = LoadChildBPVersionStructure(new BudgetPlanVersionStructureVM() { ItemID = item.ItemID, Version = item.Version, Sequence = item.Sequence },
                            listBudgetPlanVersionStructureVM);

                       

                        Node node = new Node()
                        {
                            Expanded = m_nodeChildCollection.Count > 0,
                            Expandable = m_nodeChildCollection.Count > 0,
                            AttributesObject = new
                            {
                                itemdesc = item.ItemDesc,
                                budgetplanid = item.BudgetPlanID,
                                budgetplanversionstructureid = item.BudgetPlanVersionStructureID,
                                budgetplantemplateid = item.BudgetPlanTemplateID,
                                itemid = item.ItemID,
                                version = item.Version,
                                sequence = item.Sequence,
                                parentitemid = item.ParentItemID,
                                parentversion = item.ParentVersion,
                                parentsequence = item.ParentSequence,
                                vendorid = item.VendorID,
                                vendorname = item.VendorName,
                                specification = item.Specification,
                                itemversionchildid = item.ItemVersionChildID,
                                uomdesc = item.UoMDesc,
                                itemtypeid = item.ItemTypeID,
                                isboi = item.IsBOI,
                                isahs = item.IsAHS,
                                haschild = m_nodeChildCollection.Any(),
                                leaf = m_nodeChildCollection.Count > 0 ? false : true,
                                uomid = item.UoMID,
                                sequencedesc = ""
                                
                            },
                            Icon = item.IsBOI ? Icon.Folder : (item.IsAHS ? Icon.Table : Icon.PageWhite)
                        };
                        node.Children.AddRange(m_nodeChildCollection);
                        m_nodeCollectChild.Add(node);

                    }

                 
                }
            

            return this.Store(m_nodeCollectChild);
        }
        public ActionResult Home()
        {
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 0;
            return this.Direct();
        }
        #endregion

        public NodeCollection LoadChildBPVersionStructure(BudgetPlanVersionStructureVM dataParent, List<BudgetPlanVersionStructureVM> listBudgetPlanStructure)
        {
            
            ItemPriceVM dataItemPrice = new ItemPriceVM();
            NodeCollection m_nodeCollection = new NodeCollection();

            BudgetPlanVersionStructureVM m_objMBudgetPlanVersionStructureVM = new BudgetPlanVersionStructureVM();
            DBudgetPlanVersionStructureDA m_objMBudgetPlanVersionStructureDA = new DBudgetPlanVersionStructureDA();
            m_objMBudgetPlanVersionStructureDA.ConnectionStringName = Global.ConnStrConfigName;

            List<BudgetPlanVersionStructureVM> m_lstBudgetPlanStructure =
                listBudgetPlanStructure.Where(d => d.ParentItemID == dataParent.ItemID &&
                                                    d.ParentVersion == dataParent.Version &&
                                                    d.ParentSequence == dataParent.Sequence
                                    ).OrderBy(d => d.Sequence).ToList();


            foreach (BudgetPlanVersionStructureVM item in m_lstBudgetPlanStructure)
            {
                NodeCollection m_nodeCollectionChild = LoadChildBPVersionStructure(new BudgetPlanVersionStructureVM() { ItemID = item.ItemID, Version = item.Version, Sequence = item.Sequence }, listBudgetPlanStructure);

                
                Node node = new Node()
                {
                    Expanded = m_nodeCollectionChild.Count > 0,
                    Expandable = m_nodeCollectionChild.Count > 0,
                    AttributesObject = new
                    {
                        itemdesc = item.ItemDesc,//string.Format("{0} {1}", m_strNumbering, item.ItemDesc),
                        budgetplanid = item.BudgetPlanID,
                        budgetplanversionstructureid = item.BudgetPlanVersionStructureID,
                        budgetplantemplateid = item.BudgetPlanTemplateID,
                        itemid = item.ItemID,
                        version = item.Version,
                        sequence = item.Sequence,
                        parentitemid = item.ParentItemID,
                        parentversion = item.ParentVersion,
                        parentsequence = item.ParentSequence,
                        vendorid = item.VendorID,
                        vendorname = item.VendorName,
                        specification = item.Specification,
                        itemversionchildid = item.ItemVersionChildID,
                        uomdesc = item.UoMDesc,
                        itemtypeid = item.ItemTypeID,
                        isboi = item.IsBOI,
                        isahs = item.IsAHS,
                        haschild = m_nodeCollectionChild.Any(),
                        
                        leaf = m_nodeCollectionChild.Count > 0 ? false : true,
                        uomid = item.UoMID,
                        sequencedesc = ""
                    },
                    Icon = item.IsBOI ? Icon.Folder : (item.IsAHS ? Icon.Table : Icon.PageWhite)
                };
                node.Children.AddRange(m_nodeCollectionChild);
                m_nodeCollection.Add(node);
            }


            return m_nodeCollection;

        }

        #region Private Method 
        private bool IsListBudgetPlanVersionStructureComplete(List<BudgetPlanVersionStructureVM> ListBudgetPlanVersionStructureVM)
        {
            bool m_boolretval = true;
            foreach (BudgetPlanVersionStructureVM m_objBudgetPlanVersionStructureVM in ListBudgetPlanVersionStructureVM.Where(x => x.IsBOI && x.ParentItemID != "0"))
            {
                
                if (m_objBudgetPlanVersionStructureVM.HasChild)
                {
                    if (!hasboichild(ListBudgetPlanVersionStructureVM, m_objBudgetPlanVersionStructureVM.ParentItemID))
                    {
                        if (string.IsNullOrEmpty(m_objBudgetPlanVersionStructureVM.VendorID))return false;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(m_objBudgetPlanVersionStructureVM.VendorID)) return false;
                }
            }
            
            return m_boolretval;
        }
        private bool hasboichild(List<BudgetPlanVersionStructureVM> ListBudgetPlanVersionStructureVM, string parentitemid)
        {
            bool m_boolretval = false;
            foreach (BudgetPlanVersionStructureVM m_objBudgetPlanVersionStructureVM in ListBudgetPlanVersionStructureVM)
            {
                if (m_objBudgetPlanVersionStructureVM.ItemID == parentitemid && m_objBudgetPlanVersionStructureVM.IsBOI) return true;
            }
            return m_boolretval;
        }
        private bool StructureAlreadyCreated(string vendorID, string budgetPlanVersionStructureID)
        {
            bool m_boolretval = false;

            DBudgetPlanVersionEntryDA m_objDBudgetPlanVersionEntryDA = new DBudgetPlanVersionEntryDA();
            m_objDBudgetPlanVersionEntryDA.ConnectionStringName = Global.ConnStrConfigName;
            Dictionary<string, List<object>> m_dicFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();

            List<BudgetPlanVersionEntryVM> m_lstBudgetPlanVersionEntryVM = new List<BudgetPlanVersionEntryVM>();

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(budgetPlanVersionStructureID);
            m_dicFilter.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersionStructureID.Map, m_lstFilter);

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersionStructureID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.Info.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.MaterialAmount.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.MiscAmount.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.Volume.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionEntryVM.Prop.WageAmount.MapAlias);

            
            Dictionary<int, DataSet> m_dicDBudgetPlanVersionEntryDA = m_objDBudgetPlanVersionEntryDA.SelectBC(0, null, false, m_lstSelect, m_dicFilter, null, null, null, null);
            if (m_objDBudgetPlanVersionEntryDA.Message == string.Empty)
            {
                m_lstBudgetPlanVersionEntryVM = (
                    from DataRow m_drMBudgetPlanVersionEntryDA in m_dicDBudgetPlanVersionEntryDA[0].Tables[0].Rows
                    select new BudgetPlanVersionEntryVM()
                    {
                        BudgetPlanID = m_drMBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.BudgetPlanID.Name].ToString(),
                        BudgetPlanVersion = int.Parse(m_drMBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.BudgetPlanVersion.Name].ToString()),
                        BudgetPlanVersionStructureID = m_drMBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.BudgetPlanVersionStructureID.Name].ToString(),
                        Info = m_drMBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.Info.Name].ToString(),
                        MaterialAmount = decimal.Parse(m_drMBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.MaterialAmount.Name].ToString()),
                        MiscAmount = decimal.Parse(m_drMBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.MiscAmount.Name].ToString()),
                        VendorID = m_drMBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.VendorID.Name].ToString(),
                        Volume = decimal.Parse(m_drMBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.Volume.Name].ToString()),
                        WageAmount = decimal.Parse(m_drMBudgetPlanVersionEntryDA[BudgetPlanVersionEntryVM.Prop.WageAmount.Name].ToString())
                    }
                ).ToList();
            }

            if (m_lstBudgetPlanVersionEntryVM.Any())
            {
                if (m_lstBudgetPlanVersionEntryVM.Where(x => x.VendorID != vendorID).Any())
                {
                    return true;
                }
            }

            return m_boolretval;
        }
        private BudgetPlanVM GetSelectedData(Dictionary<string, object> selected, string budgetPlanVersion, ref string message)
        {
            BudgetPlanVM m_objBudgetPlanVM = new BudgetPlanVM();
            TBudgetPlanDA m_objTBudgetPlanDA = new TBudgetPlanDA();
            m_objTBudgetPlanDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.Description.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanTemplateDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanTemplateID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanTypeDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.CompanyDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.RegionID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.RegionDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.LocationDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.DivisionDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.ProjectID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.ProjectDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.ClusterDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.ClusterID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.UnitTypeID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.UnitTypeDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.Area.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.Unit.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVM.Prop.FeePercentage.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.CreatedDate.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.ModifiedDate.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.StatusID.MapAlias);
            m_lstSelect.Add(BudgetPlanVM.Prop.StatusDesc.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objBudgetPlanVM.IsKey(m_kvpSelectedRow.Key))
                {
                    List<object> m_lstFilter = new List<object>();


                    if (m_objBudgetPlanVM.IsKey(m_kvpSelectedRow.Key))
                    {
                        if (!string.IsNullOrEmpty(budgetPlanVersion) && m_kvpSelectedRow.Key == BudgetPlanVersionVM.Prop.BudgetPlanVersion.Name)
                        {

                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(budgetPlanVersion);
                            m_objFilter.Add(BudgetPlanVersionVM.Prop.BudgetPlanVersion.Map, m_lstFilter);
                        }
                        else
                        {
                            m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                            m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_kvpSelectedRow.Value);
                            m_objFilter.Add(BudgetPlanVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);
                        }
                    }

                }
            }


            Dictionary<int, DataSet> m_dicTBudgetPlanDA = m_objTBudgetPlanDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objTBudgetPlanDA.Message == string.Empty)
            {
                DataRow m_drTBudgetPlanDA = m_dicTBudgetPlanDA[0].Tables[0].Rows[0];
                m_objBudgetPlanVM.BudgetPlanID = m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanID.Name].ToString();
                m_objBudgetPlanVM.Description = m_drTBudgetPlanDA[BudgetPlanVM.Prop.Description.Name].ToString();
                m_objBudgetPlanVM.BudgetPlanTemplateID = m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanTemplateID.Name].ToString();
                m_objBudgetPlanVM.BudgetPlanTemplateDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanTemplateDesc.Name].ToString();
                m_objBudgetPlanVM.BudgetPlanTypeDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanTypeDesc.Name].ToString();
                m_objBudgetPlanVM.CompanyDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.CompanyDesc.Name].ToString();
                m_objBudgetPlanVM.RegionID = m_drTBudgetPlanDA[BudgetPlanVM.Prop.RegionID.Name].ToString();
                m_objBudgetPlanVM.RegionDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.RegionDesc.Name].ToString();
                m_objBudgetPlanVM.LocationDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.LocationDesc.Name].ToString();
                m_objBudgetPlanVM.DivisionDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.DivisionDesc.Name].ToString();
                m_objBudgetPlanVM.ProjectID = m_drTBudgetPlanDA[BudgetPlanVM.Prop.ProjectID.Name].ToString();
                m_objBudgetPlanVM.ProjectDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.ProjectDesc.Name].ToString();
                m_objBudgetPlanVM.ClusterID = m_drTBudgetPlanDA[BudgetPlanVM.Prop.ClusterID.Name].ToString();
                m_objBudgetPlanVM.ClusterDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.ClusterDesc.Name].ToString();
                m_objBudgetPlanVM.UnitTypeID = m_drTBudgetPlanDA[BudgetPlanVM.Prop.UnitTypeID.Name].ToString();
                m_objBudgetPlanVM.UnitTypeDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.UnitTypeDesc.Name].ToString();
                m_objBudgetPlanVM.Area = decimal.Parse(m_drTBudgetPlanDA[BudgetPlanVM.Prop.Area.Name].ToString());
                m_objBudgetPlanVM.Unit = decimal.Parse(m_drTBudgetPlanDA[BudgetPlanVM.Prop.Unit.Name].ToString());
                m_objBudgetPlanVM.FeePercentage = decimal.Parse(m_drTBudgetPlanDA[BudgetPlanVM.Prop.FeePercentage.Name].ToString());
                m_objBudgetPlanVM.BudgetPlanVersion = Convert.ToInt32(m_drTBudgetPlanDA[BudgetPlanVM.Prop.BudgetPlanVersion.Name].ToString());
                m_objBudgetPlanVM.CreatedDate = DateTime.Parse(m_drTBudgetPlanDA[BudgetPlanVM.Prop.CreatedDate.Name].ToString());
                m_objBudgetPlanVM.ModifiedDate = DateTime.Parse(m_drTBudgetPlanDA[BudgetPlanVM.Prop.ModifiedDate.Name].ToString());
                m_objBudgetPlanVM.StatusDesc = m_drTBudgetPlanDA[BudgetPlanVM.Prop.StatusDesc.Name].ToString();
                m_objBudgetPlanVM.StatusID = (int)m_drTBudgetPlanDA[BudgetPlanVM.Prop.StatusID.Name];
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objTBudgetPlanDA.Message;

            return m_objBudgetPlanVM;
        }
        private List<BudgetPlanVersionStructureVM> GetListBudgetPlanStructure(string BudgetPlanID, int BudgetPlanVersion, ref string message)
        {

            List<BudgetPlanVersionStructureVM> m_lstBudgetPlanVersionStructureVM = new List<BudgetPlanVersionStructureVM>();
            DBudgetPlanVersionStructureDA m_objDBudgetPlanVersionStructureDA = new DBudgetPlanVersionStructureDA();
            m_objDBudgetPlanVersionStructureDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersionStructureID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanTemplateID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ItemVersionChildID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Version.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Sequence.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ParentItemID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ParentItemDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ParentVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ParentSequence.MapAlias);

            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Volume.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Specification.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.MaterialAmount.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.WageAmount.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.MiscAmount.MapAlias);

            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.VendorID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.VendorName.MapAlias);

            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ItemTypeID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.UoMDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.UoMID.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Key3.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc1.MapAlias);

            m_lstSelect.Add(ItemVersionChildVM.Prop.Coefficient.MapAlias);


            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanID);
            m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanVersion);
            m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

            Dictionary<string, OrderDirection> m_objOrderBy = new Dictionary<string, OrderDirection>();
            m_objOrderBy.Add(BudgetPlanVersionStructureVM.Prop.Sequence.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicDBudgetPlanVersionStructureDA = m_objDBudgetPlanVersionStructureDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, m_objOrderBy);
            if (m_objDBudgetPlanVersionStructureDA.Message == string.Empty)
            {
                m_lstBudgetPlanVersionStructureVM = (
                from DataRow m_drDBudgetPlanVersionStructureDA in m_dicDBudgetPlanVersionStructureDA[0].Tables[0].Rows
                select new BudgetPlanVersionStructureVM()
                {
                    BudgetPlanVersionStructureID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.BudgetPlanVersionStructureID.Name].ToString(),
                    BudgetPlanTemplateID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.BudgetPlanTemplateID.Name].ToString(),
                    BudgetPlanID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.BudgetPlanID.Name].ToString(),
                    BudgetPlanVersion = int.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.Name].ToString()),
                    ItemVersionChildID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ItemVersionChildID.Name].ToString(),
                    ItemID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ItemID.Name].ToString(),
                    ItemDesc = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ItemDesc.Name].ToString(),
                    Version = int.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Version.Name].ToString()),
                    Sequence = int.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Sequence.Name].ToString()),
                    ParentItemID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ParentItemID.Name].ToString(),
                    ParentItemDesc = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ParentItemDesc.Name].ToString(),
                    ParentVersion = int.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ParentVersion.Name].ToString()),
                    ParentSequence = int.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ParentSequence.Name].ToString()),
                    IsBOI = m_drDBudgetPlanVersionStructureDA[ConfigVM.Prop.Key3.Name].ToString() == m_drDBudgetPlanVersionStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString()
                            && m_drDBudgetPlanVersionStructureDA[ConfigVM.Prop.Desc1.Name].ToString() == "TRUE",
                    IsAHS = m_drDBudgetPlanVersionStructureDA[ConfigVM.Prop.Key3.Name].ToString() == m_drDBudgetPlanVersionStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString()
                            && m_drDBudgetPlanVersionStructureDA[ConfigVM.Prop.Desc1.Name].ToString() == "FALSE",
                    Volume = ((decimal?)m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Volume.Name] == 0 ? (decimal?)null :
                                decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Volume.Name].ToString())),
                    Specification = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Specification.Name].ToString(),
                    MaterialAmount = ((decimal?)m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name] == 0 ? (decimal?)null :
                                decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name].ToString())),
                    WageAmount = ((decimal?)m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.WageAmount.Name] == 0 ? (decimal?)null :
                                decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.WageAmount.Name].ToString())),
                    MiscAmount = ((decimal?)m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.MiscAmount.Name] == 0 ? (decimal?)null :
                                decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.MiscAmount.Name].ToString())),

                    VendorID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.VendorID.Name].ToString(),
                    VendorName = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.VendorName.Name].ToString(),

                    ItemTypeID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ItemTypeID.Name].ToString(),
                    UoMDesc = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.UoMDesc.Name].ToString(),
                    UoMID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.UoMID.Name].ToString(),

                    Coefficient = string.IsNullOrEmpty(m_drDBudgetPlanVersionStructureDA[ItemVersionChildVM.Prop.Coefficient.Name].ToString()) ? 0 :
                                  (decimal?)m_drDBudgetPlanVersionStructureDA[ItemVersionChildVM.Prop.Coefficient.Name]
                }).Distinct().ToList();
            }
            else
                message = m_objDBudgetPlanVersionStructureDA.Message;

            m_lstBudgetPlanVersionStructureVM = m_lstBudgetPlanVersionStructureVM.Where(x => x.IsBOI == true).ToList();
            return m_lstBudgetPlanVersionStructureVM;

        }
        private Dictionary<string, object> GetFormData(NameValueCollection parameters, string BudgetPlanID = "")
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();
            m_dicReturn.Add(BudgetPlanVM.Prop.BudgetPlanID.Name, (parameters[BudgetPlanVM.Prop.BudgetPlanID.Name].ToString() == string.Empty ? BudgetPlanID : parameters[BudgetPlanVM.Prop.BudgetPlanID.Name]));
            m_dicReturn.Add(BudgetPlanVM.Prop.Description.Name, parameters[BudgetPlanVM.Prop.Description.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.BudgetPlanTemplateID.Name, parameters[BudgetPlanVM.Prop.BudgetPlanTemplateID.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.BudgetPlanTemplateDesc.Name, parameters[BudgetPlanVM.Prop.BudgetPlanTemplateDesc.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.BudgetPlanTypeDesc.Name, parameters[BudgetPlanVM.Prop.BudgetPlanTypeDesc.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.CompanyDesc.Name, parameters[BudgetPlanVM.Prop.CompanyDesc.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.RegionDesc.Name, parameters[BudgetPlanVM.Prop.RegionDesc.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.LocationDesc.Name, parameters[BudgetPlanVM.Prop.LocationDesc.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.DivisionDesc.Name, parameters[BudgetPlanVM.Prop.DivisionDesc.Name]);

            m_dicReturn.Add(BudgetPlanVM.Prop.ProjectID.Name, parameters[BudgetPlanVM.Prop.ProjectID.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.ProjectDesc.Name, parameters[BudgetPlanVM.Prop.ProjectDesc.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.ClusterID.Name, parameters[BudgetPlanVM.Prop.ClusterID.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.ClusterDesc.Name, parameters[BudgetPlanVM.Prop.ClusterDesc.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.UnitTypeID.Name, parameters[BudgetPlanVM.Prop.UnitTypeID.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.UnitTypeDesc.Name, parameters[BudgetPlanVM.Prop.UnitTypeDesc.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.Area.Name, parameters[BudgetPlanVM.Prop.Area.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.Unit.Name, parameters[BudgetPlanVM.Prop.Unit.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.BudgetPlanVersion.Name, parameters[BudgetPlanVM.Prop.BudgetPlanVersion.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.CreatedDate.Name, parameters[BudgetPlanVM.Prop.CreatedDate.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.ModifiedDate.Name, parameters[BudgetPlanVM.Prop.ModifiedDate.Name]);
            m_dicReturn.Add(BudgetPlanVM.Prop.StatusDesc.Name, parameters[BudgetPlanVM.Prop.StatusDesc.Name]);


            return m_dicReturn;
        }
        #endregion
    }
}