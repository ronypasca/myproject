using com.SML.BIGTRONS.Controllers;
using com.SML.BIGTRONS.DataAccess;
using com.SML.BIGTRONS.Models;
using com.SML.BIGTRONS.ViewModels;
using com.SML.Lib.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Services;

namespace com.SML.BIGTRONS.Services
{
    /// <summary>
    /// Summary description for Cancellation
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Cancellation : BaseController
    {
        [WebMethod]
        public PackageWSVM GetPackage(string PackageID, string ActionID, ref string Message)
        {
            SAction Action = new SAction() { ActionID = ActionID };
            PackageWSVM m_objPackageWSVM = new PackageWSVM();

            List<object> m_lstReturn = new List<object>();

            if (isParamsValid(PackageID, Action, ref Message))
            {
                if (!isGetPackageValid(PackageID, Action, ref Message, ref m_objPackageWSVM))
                {
                    m_objPackageWSVM = new PackageWSVM();
                }
            }

            if (Message != String.Empty)
                return new PackageWSVM();
            else { Message = ""; return m_objPackageWSVM; };
        }

        [WebMethod]
        public string CancellationValid(string PackageID)
        {
            string m_strMessage = "";
            SAction m_objSAction = new SAction() { ActionID = "Create" };

            if (isParamsValid(PackageID, m_objSAction, ref m_strMessage))
            {
                PackageWSVM m_objPackageWSVM = new PackageWSVM();
                if (!isGetPackageValid(PackageID, m_objSAction, ref m_strMessage, ref m_objPackageWSVM))
                {
                    m_objPackageWSVM = new PackageWSVM();
                }
            }

            return m_strMessage;
        }

        [WebMethod]
        public string UpdatePackage(string PackageID, string ActionID)
        {
            SAction Action = new SAction() { ActionID = ActionID };
            string m_strMessage = "";

            PackageWSVM m_objPackageWSVM = new PackageWSVM();

            List<object> m_lstReturn = new List<object>();

            if (isParamsValid(PackageID, Action, ref m_strMessage))
            {
                PackageVM m_objPackageVM = GetPackageDetail(PackageID);
                if (m_objPackageVM.PackageID != null)
                {
                    if (Action.ActionID == "Create")
                    {
                        #region Create Action
                        if (m_objPackageVM.PackageID != null)
                        {
                            if (m_objPackageVM.StatusID != 2)
                            {
                                m_strMessage = "Status " + General.EnumDesc(MessageLib.Invalid) + ". [" + PackageID + "]";
                            }
                            else
                            {
                                List<PackageListVM> m_lstPackageListVM = GetPackageListDetail(PackageID);

                                List<string> m_lstBudgetPlanInvalid = new List<string>();

                                foreach (PackageListVM item in m_lstPackageListVM)
                                {
                                    List<object> m_lstBudgetPlan = new List<object>();

                                    if (item.StatusID != 2)
                                    {
                                        m_lstBudgetPlanInvalid.Add(item.BudgetPlanID);
                                        break;
                                    }
                                }

                                if (m_lstBudgetPlanInvalid.Count > 0)
                                {
                                    m_strMessage = "Some of Budget Plan's is invalid. Possibly there is Approval created for this package with status Revised. [" + String.Join(", ", m_lstBudgetPlanInvalid) + "]";
                                }
                                else
                                {
                                    List<string> m_lstBudgetPlanVersion = new List<string>();
                                    foreach (PackageListVM item in m_lstPackageListVM)
                                    {
                                        DBudgetPlanVersionEntryDA m_objDBudgetPlanVersionEntryDA = new DBudgetPlanVersionEntryDA();
                                        m_objDBudgetPlanVersionEntryDA.ConnectionStringName = Global.ConnStrConfigName;

                                        Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                                        List<object> m_lstFilter = new List<object>();
                                        m_lstFilter.Add(Operator.LessThanEqual);
                                        m_lstFilter.Add(2);
                                        m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.StatusID.Map, m_lstFilter);

                                        m_lstFilter = new List<object>();
                                        m_lstFilter.Add(Operator.Equals);
                                        m_lstFilter.Add(item.BudgetPlanID);
                                        m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanID.Map, m_lstFilter);

                                        m_lstFilter = new List<object>();
                                        m_lstFilter.Add(Operator.Equals);
                                        m_lstFilter.Add(item.BudgetPlanVersion);
                                        m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

                                        Dictionary<int, DataSet> m_dicDBudgetPlanVersionEntryDA = m_objDBudgetPlanVersionEntryDA.SelectBC(0, null, true, null, m_objFilter, null, null, null, null);
                                        int m_intCount = 0;

                                        foreach (KeyValuePair<int, DataSet> m_kvpPackageBL in m_dicDBudgetPlanVersionEntryDA)
                                        {
                                            m_intCount = m_kvpPackageBL.Key;
                                            break;
                                        }

                                        if (m_intCount > 0)
                                        {
                                            m_lstBudgetPlanVersion.Add(item.BudgetPlanID);
                                        }
                                    }

                                    if (m_lstBudgetPlanVersion.Count > 0)
                                    {
                                        m_strMessage = "There are active Budget Plan Entry. [" + String.Join(", ", m_lstBudgetPlanVersion) + "]";
                                    }
                                    else
                                    {
                                        TPackageDA m_objTPackageDA = new TPackageDA();
                                        m_objTPackageDA.ConnectionStringName = Global.ConnStrConfigName;

                                        string m_strTransaction = "ServiceActionCreate";
                                        object m_objConnection = m_objTPackageDA.BeginTrans(m_strTransaction);

                                        m_objTPackageDA.Data.PackageID = m_objPackageVM.PackageID;
                                        m_objTPackageDA.Select();
                                        m_objTPackageDA.Data.StatusID = 1;
                                        m_objTPackageDA.Update(true, m_objConnection);
                                        if (m_objTPackageDA.Success)
                                        {
                                            DBudgetPlanVersionDA m_objDBudgetPlanVersionDA = new DBudgetPlanVersionDA();
                                            m_objDBudgetPlanVersionDA.ConnectionStringName = Global.ConnStrConfigName;

                                            foreach (PackageListVM item in m_lstPackageListVM)
                                            {
                                                m_objDBudgetPlanVersionDA.Data.BudgetPlanID = item.BudgetPlanID;
                                                m_objDBudgetPlanVersionDA.Data.BudgetPlanVersion = item.BudgetPlanVersion;

                                                m_objDBudgetPlanVersionDA.Select();
                                                m_objDBudgetPlanVersionDA.Data.StatusID = 1;
                                                m_objDBudgetPlanVersionDA.Update(true, m_objConnection);
                                                if (!m_objDBudgetPlanVersionDA.Success)
                                                {
                                                    m_strMessage = "Update Package Error. [" + m_objDBudgetPlanVersionDA.Message + "]";
                                                    m_objTPackageDA.EndTrans(ref m_objConnection, false, m_strTransaction);
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            m_strMessage = "Update Package Error. [" + m_objTPackageDA.Message + "]";
                                            m_objTPackageDA.EndTrans(ref m_objConnection, false, m_strTransaction);
                                        }

                                        if (m_strMessage.Length == 0)
                                        {
                                            m_strMessage = "";
                                            m_objTPackageDA.EndTrans(ref m_objConnection, true, m_strTransaction);
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                    else if (Action.ActionID == "Approve")
                    {
                        #region Approve Action
                        if (m_objPackageVM.PackageID != null)
                        {
                            if (m_objPackageVM.StatusID != 1)
                            {
                                m_strMessage = "Status " + General.EnumDesc(MessageLib.Invalid) + ". [" + PackageID + "]";
                            }
                            else
                            {
                                List<PackageListVM> m_lstPackageListVM = GetPackageListDetail(PackageID);

                                List<string> m_lstBudgetPlanInvalid = new List<string>();

                                foreach (PackageListVM item in m_lstPackageListVM)
                                {
                                    List<object> m_lstBudgetPlan = new List<object>();

                                    if (item.StatusID != 1)
                                    {
                                        m_lstBudgetPlanInvalid.Add(item.BudgetPlanID);
                                        break;
                                    }
                                }

                                if (m_lstBudgetPlanInvalid.Count > 0)
                                {
                                    m_strMessage = "Some of Budget Plan's is invalid. Possibly there is Approval created for this package with status Revised. [" + String.Join(", ", m_lstBudgetPlanInvalid) + "]";
                                }
                                else
                                {
                                    List<string> m_lstBudgetPlanVersion = new List<string>();
                                    foreach (PackageListVM item in m_lstPackageListVM)
                                    {
                                        DBudgetPlanVersionEntryDA m_objDBudgetPlanVersionEntryDA = new DBudgetPlanVersionEntryDA();
                                        m_objDBudgetPlanVersionEntryDA.ConnectionStringName = Global.ConnStrConfigName;

                                        Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                                        List<object> m_lstFilter = new List<object>();
                                        m_lstFilter.Add(Operator.LessThanEqual);
                                        m_lstFilter.Add(2);
                                        m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.StatusID.Map, m_lstFilter);

                                        m_lstFilter = new List<object>();
                                        m_lstFilter.Add(Operator.Equals);
                                        m_lstFilter.Add(item.BudgetPlanID);
                                        m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanID.Map, m_lstFilter);

                                        m_lstFilter = new List<object>();
                                        m_lstFilter.Add(Operator.Equals);
                                        m_lstFilter.Add(item.BudgetPlanVersion);
                                        m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

                                        Dictionary<int, DataSet> m_dicDBudgetPlanVersionEntryDA = m_objDBudgetPlanVersionEntryDA.SelectBC(0, null, true, null, m_objFilter, null, null, null, null);
                                        int m_intCount = 0;

                                        foreach (KeyValuePair<int, DataSet> m_kvpPackageBL in m_dicDBudgetPlanVersionEntryDA)
                                        {
                                            m_intCount = m_kvpPackageBL.Key;
                                            break;
                                        }

                                        if (m_intCount > 0)
                                        {
                                            m_lstBudgetPlanVersion.Add(item.BudgetPlanID);
                                        }
                                    }

                                    if (m_lstBudgetPlanVersion.Count > 0)
                                    {
                                        m_strMessage = "There are active Budget Plan Entry. [" + String.Join(", ", m_lstBudgetPlanVersion) + "]";
                                    }
                                    else
                                    {
                                        DBudgetPlanVersionDA m_objDBudgetPlanVersionDA = new DBudgetPlanVersionDA();
                                        m_objDBudgetPlanVersionDA.ConnectionStringName = Global.ConnStrConfigName;
                                        string m_strTransaction = "ServiceActionApprove";
                                        object m_objConnection = m_objDBudgetPlanVersionDA.BeginTrans(m_strTransaction);

                                        bool m_boolPackageHasEntry = false;

                                        foreach (PackageListVM item in m_lstPackageListVM)
                                        {
                                            m_objDBudgetPlanVersionDA.Data.BudgetPlanID = item.BudgetPlanID;
                                            m_objDBudgetPlanVersionDA.Data.BudgetPlanVersion = item.BudgetPlanVersion;

                                            m_objDBudgetPlanVersionDA.Select();
                                            m_objDBudgetPlanVersionDA.Data.StatusID = HasBudgetPlanVersionEntry(item.BudgetPlanID, item.BudgetPlanVersion) ? 99 : 0;
                                            m_objDBudgetPlanVersionDA.Update(true, m_objConnection);
                                            if (!m_objDBudgetPlanVersionDA.Success)
                                            {
                                                m_strMessage = "Update Package Error. [" + m_objDBudgetPlanVersionDA.Message + "]";
                                                break;
                                            }

                                            if (!m_boolPackageHasEntry)
                                            {
                                                m_boolPackageHasEntry = HasBudgetPlanVersionEntry(item.BudgetPlanID, item.BudgetPlanVersion);
                                            }
                                        }

                                        if (m_objDBudgetPlanVersionDA.Success)
                                        {
                                            TPackageDA m_objTPackageDA = new TPackageDA();
                                            m_objTPackageDA.ConnectionStringName = Global.ConnStrConfigName;

                                            m_objTPackageDA.Data.PackageID = m_objPackageVM.PackageID;
                                            m_objTPackageDA.Select();
                                            m_objTPackageDA.Data.StatusID = m_boolPackageHasEntry ? 99 : 0;
                                            m_objTPackageDA.Update(true, m_objConnection);
                                            if (!m_objTPackageDA.Success)
                                            {
                                                m_strMessage = "Update Package Error. [" + m_objTPackageDA.Message + "]";
                                                m_objDBudgetPlanVersionDA.EndTrans(ref m_objConnection, false, m_strTransaction);
                                            }
                                        }
                                        else
                                        {
                                            m_strMessage = "Update Package Error. [" + m_objDBudgetPlanVersionDA.Message + "]";
                                            m_objDBudgetPlanVersionDA.EndTrans(ref m_objConnection, false, m_strTransaction);
                                        }

                                        if (m_strMessage.Length == 0)
                                        {
                                            m_strMessage = "";
                                            m_objDBudgetPlanVersionDA.EndTrans(ref m_objConnection, true, m_strTransaction);
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                    else if (Action.ActionID == "Reject")
                    {
                        #region Reject Action
                        if (m_objPackageVM.PackageID != null)
                        {
                            if (m_objPackageVM.StatusID != 1)
                            {
                                m_strMessage = "Status " + General.EnumDesc(MessageLib.Invalid) + ". [" + PackageID + "]";
                            }
                            else
                            {
                                List<PackageListVM> m_lstPackageListVM = GetPackageListDetail(PackageID);

                                List<string> m_lstBudgetPlanInvalid = new List<string>();

                                foreach (PackageListVM item in m_lstPackageListVM)
                                {
                                    List<object> m_lstBudgetPlan = new List<object>();

                                    if (item.StatusID != 1)
                                    {
                                        m_lstBudgetPlanInvalid.Add(item.BudgetPlanID);
                                        break;
                                    }
                                }

                                if (m_lstBudgetPlanInvalid.Count > 0)
                                {
                                    m_strMessage = "Some of Budget Plan's is invalid. Possibly there is Approval created for this package with status Revised. [" + String.Join(", ", m_lstBudgetPlanInvalid) + "]";
                                }
                                else
                                {
                                    List<string> m_lstBudgetPlanVersion = new List<string>();
                                    foreach (PackageListVM item in m_lstPackageListVM)
                                    {
                                        DBudgetPlanVersionEntryDA m_objDBudgetPlanVersionEntryDA = new DBudgetPlanVersionEntryDA();
                                        m_objDBudgetPlanVersionEntryDA.ConnectionStringName = Global.ConnStrConfigName;

                                        Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                                        List<object> m_lstFilter = new List<object>();
                                        m_lstFilter.Add(Operator.LessThanEqual);
                                        m_lstFilter.Add(2);
                                        m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.StatusID.Map, m_lstFilter);

                                        m_lstFilter = new List<object>();
                                        m_lstFilter.Add(Operator.Equals);
                                        m_lstFilter.Add(item.BudgetPlanID);
                                        m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanID.Map, m_lstFilter);

                                        m_lstFilter = new List<object>();
                                        m_lstFilter.Add(Operator.Equals);
                                        m_lstFilter.Add(item.BudgetPlanVersion);
                                        m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

                                        Dictionary<int, DataSet> m_dicDBudgetPlanVersionEntryDA = m_objDBudgetPlanVersionEntryDA.SelectBC(0, null, true, null, m_objFilter, null, null, null, null);
                                        int m_intCount = 0;

                                        foreach (KeyValuePair<int, DataSet> m_kvpPackageBL in m_dicDBudgetPlanVersionEntryDA)
                                        {
                                            m_intCount = m_kvpPackageBL.Key;
                                            break;
                                        }

                                        if (m_intCount > 0)
                                        {
                                            m_lstBudgetPlanVersion.Add(item.BudgetPlanID);
                                        }
                                    }

                                    if (m_lstBudgetPlanVersion.Count > 0)
                                    {
                                        m_strMessage = "There are active Budget Plan Entry. [" + String.Join(", ", m_lstBudgetPlanVersion) + "]";
                                    }
                                    else
                                    {
                                        TPackageDA m_objTPackageDA = new TPackageDA();
                                        m_objTPackageDA.ConnectionStringName = Global.ConnStrConfigName;

                                        string m_strTransaction = "ServiceActionCreate";
                                        object m_objConnection = m_objTPackageDA.BeginTrans(m_strTransaction);

                                        m_objTPackageDA.Data.PackageID = m_objPackageVM.PackageID;
                                        m_objTPackageDA.Select();
                                        m_objTPackageDA.Data.StatusID = 2;
                                        m_objTPackageDA.Update(true, m_objConnection);
                                        if (m_objTPackageDA.Success)
                                        {
                                            DBudgetPlanVersionDA m_objDBudgetPlanVersionDA = new DBudgetPlanVersionDA();
                                            m_objDBudgetPlanVersionDA.ConnectionStringName = Global.ConnStrConfigName;

                                            foreach (PackageListVM item in m_lstPackageListVM)
                                            {
                                                m_objDBudgetPlanVersionDA.Data.BudgetPlanID = item.BudgetPlanID;
                                                m_objDBudgetPlanVersionDA.Data.BudgetPlanVersion = item.BudgetPlanVersion;

                                                m_objDBudgetPlanVersionDA.Select();
                                                m_objDBudgetPlanVersionDA.Data.StatusID = 2;
                                                m_objDBudgetPlanVersionDA.Update(true, m_objConnection);
                                                if (!m_objDBudgetPlanVersionDA.Success)
                                                {
                                                    m_strMessage = "Update Package Error. [" + m_objDBudgetPlanVersionDA.Message + "]";
                                                    m_objTPackageDA.EndTrans(ref m_objConnection, false, m_strTransaction);
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            m_strMessage = "Update Package Error. [" + m_objTPackageDA.Message + "]";
                                            m_objTPackageDA.EndTrans(ref m_objConnection, false, m_strTransaction);
                                        }

                                        if (m_strMessage.Length == 0)
                                        {
                                            m_strMessage = "";
                                            m_objTPackageDA.EndTrans(ref m_objConnection, true, m_strTransaction);
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                }
                else
                {
                    m_strMessage = "Package ID " + General.EnumDesc(MessageLib.NotExist) + ". [" + PackageID + "]";
                }
            }

            if (m_strMessage.Length == 0)
            {
                m_strMessage = "";
            }

            return m_strMessage;
        }

        [WebMethod]
        public string CancellationCartValid(string CartID)
        {
            string Message = "";
            SAction Action = new SAction() { ActionID = "Create" };
            TCatalogWSVM catalogWSVM = new TCatalogWSVM();

            if (isParamsCartValid(CartID, Action, ref Message))
            {
                if (!isGetCartItemValid(CartID, Action, ref Message, ref catalogWSVM))
                {
                    catalogWSVM = new TCatalogWSVM();
                }
            }

            return Message;
        }

        [WebMethod]
        public TCatalogWSVM GetCart(string CartID, string ActionID, ref string Message)
        {
            SAction Action = new SAction() { ActionID = ActionID };
            TCatalogWSVM catalogWSVM = new TCatalogWSVM();

            if (isParamsCartValid(CartID, Action, ref Message))
            {
                if (!isGetCartItemValid(CartID, Action, ref Message, ref catalogWSVM))
                {
                    catalogWSVM = new TCatalogWSVM();
                }
            }

            return catalogWSVM;
        }

        [WebMethod]
        public string UpdateCart(string CartID, string ActionID)
        {
            SAction Action = new SAction() { ActionID = ActionID };
            string m_strMessage = "";

            if (isParamsCartValid(CartID, Action, ref m_strMessage))
            {
                if (isUpdatePackageValid(CartID, Action, ref m_strMessage))
                {
                    m_strMessage = "";
                }
            }

            return m_strMessage;
        }



        #region Private Method
        private bool isParamsValid(string PackageID, SAction Action, ref string Message)
        {
            bool m_boolValid = true;

            if (PackageID.Length == 0)
            {
                if (Message.Length == 0)
                {
                    Message = "Package ID " + General.EnumDesc(MessageLib.Invalid);
                }
                else
                {
                    Message += Environment.NewLine + "Package ID " + General.EnumDesc(MessageLib.Invalid);
                }
                m_boolValid = false;
            }

            if (Action == null)
            {
                if (Message.Length == 0)
                {
                    Message = "Action ID " + General.EnumDesc(MessageLib.Invalid);
                }
                else
                {
                    Message += Environment.NewLine + "Action ID " + General.EnumDesc(MessageLib.Invalid);
                }
                m_boolValid = false;
            }

            return m_boolValid;
        }
        private bool HasBudgetPlanVersionEntry(string BudgetPlanID, int BudgetPlanVersion)
        {
            bool m_boolReturn = false;

            DBudgetPlanVersionEntryDA m_objDBudgetPlanVersionEntryDA = new DBudgetPlanVersionEntryDA();
            m_objDBudgetPlanVersionEntryDA.ConnectionStringName = Global.ConnStrConfigName;

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.LessThanEqual);
            m_lstFilter.Add(2);
            m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.StatusID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanID);
            m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanVersion);
            m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicDBudgetPlanVersionEntryDA = m_objDBudgetPlanVersionEntryDA.SelectBC(0, null, true, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpPackageBL in m_dicDBudgetPlanVersionEntryDA)
            {
                m_intCount = m_kvpPackageBL.Key;
                break;
            }

            if (m_intCount > 0)
            {
                m_boolReturn = true;
            }

            return m_boolReturn;
        }
        private bool isGetPackageValid(string PackageID, SAction Action, ref string Message, ref PackageWSVM PackageWSVM)
        {
            bool m_boolReturn = true;

            PackageVM m_objPackageVM = GetPackageDetail(PackageID);

            if (m_objPackageVM != null)
            {
                switch (Action.ActionID)
                {
                    case "Create":
                        if (m_objPackageVM.StatusID != 2)
                        {
                            Message = PackageVM.Prop.StatusDesc.Desc + General.EnumDesc(MessageLib.invalid);
                        }
                        break;
                    default:
                        Message = "Action " + MessageLib.invalid;
                        break;
                }

                if (Message == string.Empty)
                {
                    List<string> m_lstBudgetPlanIDInvalid = new List<string>();

                    PackageWSVM.PackageID = m_objPackageVM.PackageID;
                    PackageWSVM.PackageDesc = m_objPackageVM.PackageDesc;
                    PackageWSVM.PackageList = new List<BudgetPlanWSVM>();

                    decimal m_decGrandTotal = 0;

                    List<PackageListVM> m_lstPackageListVM = GetPackageListDetail(PackageID);

                    foreach (PackageListVM item in m_lstPackageListVM)
                    {
                        if (item.StatusID != 2 && Action.ActionID == "Create")
                        {
                            m_lstBudgetPlanIDInvalid.Add(item.BudgetPlanID);
                            continue;
                        }

                        List<BudgetPlanVersionStructureWSVM> m_lstBudgetPlanVersionStructureWSVM =
                        this.GetBudgetPlanVersionStructureDetail(item.BudgetPlanID, item.BudgetPlanVersion);

                        BudgetPlanWSVM m_objBudgetPlanWSVM = new BudgetPlanWSVM();
                        m_objBudgetPlanWSVM.Structure = m_lstBudgetPlanVersionStructureWSVM;
                        m_objBudgetPlanWSVM.Area = item.Area.ToString();
                        m_objBudgetPlanWSVM.BudgetPlanDesc = item.Description;
                        m_objBudgetPlanWSVM.BudgetPlanID = item.BudgetPlanID;
                        m_objBudgetPlanWSVM.BudgetPlanType = item.BudgetPlanTypeDesc;
                        m_objBudgetPlanWSVM.Cluster = item.ClusterDesc;
                        m_objBudgetPlanWSVM.Company = item.CompanyDesc;
                        m_objBudgetPlanWSVM.Division = item.DivisionDesc;
                        m_objBudgetPlanWSVM.Location = item.LocationDesc;
                        m_objBudgetPlanWSVM.Project = item.ProjectDesc;
                        m_objBudgetPlanWSVM.Region = item.RegionDesc;
                        m_objBudgetPlanWSVM.UnitType = item.UnitTypeDesc;
                        m_objBudgetPlanWSVM.Version = item.BudgetPlanVersion;
                        m_objBudgetPlanWSVM.SummaryStructure = m_lstBudgetPlanVersionStructureWSVM.Sum(m => m.Amount);
                        PackageWSVM.PackageList.Add(m_objBudgetPlanWSVM);

                        //Add Grand Total
                        m_decGrandTotal += m_objBudgetPlanWSVM.SummaryStructure;

                    }

                    if (m_lstBudgetPlanIDInvalid.Count == 0)
                    {
                        //checking Budget Plan Version Entry
                        List<string> m_lstBudgetPlanVersion = new List<string>();
                        foreach (PackageListVM item in m_lstPackageListVM)
                        {
                            DBudgetPlanVersionEntryDA m_objDBudgetPlanVersionEntryDA = new DBudgetPlanVersionEntryDA();
                            m_objDBudgetPlanVersionEntryDA.ConnectionStringName = Global.ConnStrConfigName;

                            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.LessThanEqual);
                            m_lstFilter.Add(2);
                            m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.StatusID.Map, m_lstFilter);

                            m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(item.BudgetPlanID);
                            m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanID.Map, m_lstFilter);

                            m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(item.BudgetPlanVersion);
                            m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

                            Dictionary<int, DataSet> m_dicDBudgetPlanVersionEntryDA = m_objDBudgetPlanVersionEntryDA.SelectBC(0, null, true, null, m_objFilter, null, null, null, null);
                            int m_intCount = 0;

                            foreach (KeyValuePair<int, DataSet> m_kvpPackageBL in m_dicDBudgetPlanVersionEntryDA)
                            {
                                m_intCount = m_kvpPackageBL.Key;
                                break;
                            }

                            if (m_intCount > 0)
                            {
                                m_lstBudgetPlanVersion.Add(item.BudgetPlanID);
                            }
                        }

                        if (m_lstBudgetPlanVersion.Count > 0)
                        {
                            Message = "There are active Budget Plan Entry. [" + String.Join(", ", m_lstBudgetPlanVersion) + "]";
                        }

                    }
                    else
                    {
                        Message = "Some of Budget Plan's is invalid. [" + String.Join(", ", m_lstBudgetPlanIDInvalid) + "]";
                    }

                    PackageWSVM.GrandTotal = m_decGrandTotal;
                }
            }
            else
            {
                Message = PackageVM.Prop.PackageID.Desc + General.EnumDesc(MessageLib.notExist);
            }

            if (Message != string.Empty)
            {
                m_boolReturn = false;
            }

            return m_boolReturn;
        }
        private bool isUpdatePackageValid(string PackageID, SAction Action, ref string m_strMessage)
        {
            bool m_boolReturn = true;

            PackageVM m_objPackageVM = GetPackageDetail(PackageID);
            if (m_objPackageVM.PackageID != null)
            {
                #region Case ActionID Package
                switch (Action.ActionID)
                {
                    case "Create":
                        if (m_objPackageVM.StatusID != 2)
                        {
                            m_strMessage = "Status " + General.EnumDesc(MessageLib.Invalid) + ". [" + PackageID + "]";
                        }
                        break;
                    case "Approve":
                        if (m_objPackageVM.StatusID != 1)
                        {
                            m_strMessage = "Status " + General.EnumDesc(MessageLib.Invalid) + ". [" + PackageID + "]";
                        }
                        break;
                    case "Reject":
                        if (m_objPackageVM.StatusID != 1)
                        {
                            m_strMessage = "Status " + General.EnumDesc(MessageLib.Invalid) + ". [" + PackageID + "]";
                        }
                        break;
                    default:
                        m_strMessage = "Action " + MessageLib.invalid + ".";
                        break;
                }
                #endregion

                if (m_strMessage == string.Empty)
                {
                    List<PackageListVM> m_lstPackageListVM = GetPackageListDetail(PackageID);

                    List<string> m_lstBudgetPlanInvalid = new List<string>();

                    foreach (PackageListVM item in m_lstPackageListVM)
                    {
                        List<object> m_lstBudgetPlan = new List<object>();

                        #region Case ActionID PackageList
                        if (item.StatusID != 2 && Action.ActionID == "Create")
                        {
                            m_lstBudgetPlanInvalid.Add(item.BudgetPlanID);
                            break;
                        }
                        else if (item.StatusID != 1 && Action.ActionID == "Approve")
                        {
                            m_lstBudgetPlanInvalid.Add(item.BudgetPlanID);
                            break;
                        }
                        else if (item.StatusID != 1 && Action.ActionID == "Reject")
                        {
                            m_lstBudgetPlanInvalid.Add(item.BudgetPlanID);
                            break;
                        }
                        #endregion
                    }

                    if (m_lstBudgetPlanInvalid.Count > 0)
                    {
                        m_strMessage = "Some of Budget Plan's is invalid. Possibly there is Approval created for this package with status Revised. [" + String.Join(", ", m_lstBudgetPlanInvalid) + "]";
                    }
                    else
                    {
                        List<string> m_lstBudgetPlanVersion = new List<string>();
                        foreach (PackageListVM item in m_lstPackageListVM)
                        {
                            DBudgetPlanVersionEntryDA m_objDBudgetPlanVersionEntryDA = new DBudgetPlanVersionEntryDA();
                            m_objDBudgetPlanVersionEntryDA.ConnectionStringName = Global.ConnStrConfigName;

                            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.LessThanEqual);
                            m_lstFilter.Add(2);
                            m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.StatusID.Map, m_lstFilter);

                            m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(item.BudgetPlanID);
                            m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanID.Map, m_lstFilter);

                            m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(item.BudgetPlanVersion);
                            m_objFilter.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

                            Dictionary<int, DataSet> m_dicDBudgetPlanVersionEntryDA = m_objDBudgetPlanVersionEntryDA.SelectBC(0, null, true, null, m_objFilter, null, null, null, null);
                            int m_intCount = 0;

                            foreach (KeyValuePair<int, DataSet> m_kvpPackageBL in m_dicDBudgetPlanVersionEntryDA)
                            {
                                m_intCount = m_kvpPackageBL.Key;
                                break;
                            }

                            if (m_intCount > 0)
                            {
                                m_lstBudgetPlanVersion.Add(item.BudgetPlanID);
                            }
                        }

                        if (m_lstBudgetPlanVersion.Count > 0)
                        {
                            m_strMessage = "There are active Budget Plan Entry. [" + String.Join(", ", m_lstBudgetPlanVersion) + "]";
                        }
                        else
                        {
                            TPackageDA m_objTPackageDA = new TPackageDA();
                            m_objTPackageDA.ConnectionStringName = Global.ConnStrConfigName;

                            string m_strTransaction = "ServiceActionCreate";
                            object m_objConnection = m_objTPackageDA.BeginTrans(m_strTransaction);

                            DBudgetPlanVersionDA m_objDBudgetPlanVersionDA = new DBudgetPlanVersionDA();
                            m_objDBudgetPlanVersionDA.ConnectionStringName = Global.ConnStrConfigName;

                            bool m_boolPackageHasEntry = false;

                            foreach (PackageListVM item in m_lstPackageListVM)
                            {
                                m_objDBudgetPlanVersionDA.Data.BudgetPlanID = item.BudgetPlanID;
                                m_objDBudgetPlanVersionDA.Data.BudgetPlanVersion = item.BudgetPlanVersion;

                                m_objDBudgetPlanVersionDA.Select();

                                #region Case Action PackageList
                                switch (Action.ActionID)
                                {
                                    case "Create":
                                        m_objDBudgetPlanVersionDA.Data.StatusID = 1;
                                        break;
                                    case "Approve":
                                        m_objDBudgetPlanVersionDA.Data.StatusID = HasBudgetPlanVersionEntry(item.BudgetPlanID, item.BudgetPlanVersion) ? 99 : 0;
                                        if (!m_boolPackageHasEntry)
                                        {
                                            m_boolPackageHasEntry = HasBudgetPlanVersionEntry(item.BudgetPlanID, item.BudgetPlanVersion);
                                        }
                                        break;
                                    case "Reject":
                                        m_objDBudgetPlanVersionDA.Data.StatusID = 2;
                                        break;
                                    default:
                                        break;
                                } 
                                #endregion

                                m_objDBudgetPlanVersionDA.Update(true, m_objConnection);
                                if (!m_objDBudgetPlanVersionDA.Success)
                                {
                                    m_strMessage = "Update Package Error. [" + m_objDBudgetPlanVersionDA.Message + "]";
                                    break;
                                }
                            }

                            if (m_objDBudgetPlanVersionDA.Success)
                            {
                                m_objTPackageDA.Data.PackageID = m_objPackageVM.PackageID;
                                m_objTPackageDA.Select();

                                #region Case Action Package
                                switch (Action.ActionID)
                                {
                                    case "Create":
                                        m_objTPackageDA.Data.StatusID = 1;
                                        break;
                                    case "Approve":
                                        m_objTPackageDA.Data.StatusID = m_boolPackageHasEntry ? 99 : 0;
                                        break;
                                    case "Reject":
                                        m_objTPackageDA.Data.StatusID = 2;
                                        break;
                                    default:
                                        break;
                                } 
                                #endregion

                                m_objTPackageDA.Update(true, m_objConnection);
                                if (!m_objTPackageDA.Success)
                                {
                                    m_strMessage = "Update Package Error. [" + m_objTPackageDA.Message + "]";
                                    m_objTPackageDA.EndTrans(ref m_objConnection, false, m_strTransaction);
                                }
                            }

                            if (m_strMessage.Length == 0)
                            {
                                m_strMessage = "";
                                m_objTPackageDA.EndTrans(ref m_objConnection, true, m_strTransaction);
                            }
                        }
                    }
                }
            }
            else
            {
                m_strMessage = "Package ID " + General.EnumDesc(MessageLib.NotExist) + ". [" + PackageID + "]";
            }

            if (m_strMessage != string.Empty)
            {
                m_boolReturn = false;
            }

            return m_boolReturn;
        }

        private bool isGetCartItemValid(string CartID, SAction Action, ref string Message, ref TCatalogWSVM catalogWSVM)
        {
            bool resultStatus = true;

            try
            {
                CatalogCartVM m_objPackageVM = GetCartDetail(CartID);
                if (m_objPackageVM != null)
                {
                    switch (Action.ActionID)
                    {
                        case "Create":
                            if (m_objPackageVM.StatusID > 0)
                            {
                                Message = PackageVM.Prop.StatusDesc.Desc + General.EnumDesc(MessageLib.invalid);
                            }
                            break;
                        case "Revise":
                            if (m_objPackageVM.StatusID > 1)
                            {
                                Message = PackageVM.Prop.StatusDesc.Desc + General.EnumDesc(MessageLib.invalid);
                            }
                            break;
                        default:
                            Message = "Action " + MessageLib.invalid + ".";
                            break;
                    }

                    if (Message == string.Empty)
                    {
                        catalogWSVM = new TCatalogWSVM();
                        catalogWSVM.CatalogCartID = m_objPackageVM.CatalogCartID;
                        catalogWSVM.CatalogCartDesc = m_objPackageVM.CatalogCartDesc;
                        catalogWSVM.StatusID = m_objPackageVM.StatusID;
                        catalogWSVM.StatusDesc = m_objPackageVM.StatusDesc;



                        //GET CART DETIL LIST
                        decimal m_decGrandTotal = 0;
                        List<CartItemVM> m_lstCartItemVM = GetCartItemList(m_objPackageVM.CatalogCartID);
                        catalogWSVM.CatalogItemList = new List<CatalogCartItemsWSMV>();
                        CatalogCartItemsWSMV m_CartItemWSMV;
                        foreach (CartItemVM list in m_lstCartItemVM)
                        {
                            m_decGrandTotal += (list.Qty * list.Amount);
                            m_CartItemWSMV = new CatalogCartItemsWSMV();


                            m_CartItemWSMV.ItemID = list.ItemID;
                            m_CartItemWSMV.ItemDesc = list.ItemDesc;
                            m_CartItemWSMV.Qty = list.Qty;
                            m_CartItemWSMV.Amount = list.Amount;
                            m_CartItemWSMV.ItemPriceID = list.ItemPriceID;
                            m_CartItemWSMV.VendorID = list.VendorID;
                            m_CartItemWSMV.VendorName = list.VendorDesc;

                            catalogWSVM.CatalogItemList.Add(m_CartItemWSMV);
                        }

                        catalogWSVM.GrandTotal = m_decGrandTotal;
                    }
                }
                else
                {

                    throw new Exception(PackageVM.Prop.PackageID.Desc + General.EnumDesc(MessageLib.notExist));
                }
            }
            catch (Exception ex)
            {
                resultStatus = false;
                Message = ex.Message;
                catalogWSVM = new TCatalogWSVM();
            }

            return resultStatus;
        }
        private bool isParamsCartValid(string CartID, SAction Action, ref string Message)
        {
            bool m_boolValid = true;

            if (CartID.Length == 0)
            {
                if (Message.Length == 0)
                {
                    Message = "Cart ID " + General.EnumDesc(MessageLib.Invalid);
                }
                else
                {
                    Message += Environment.NewLine + "Cart ID " + General.EnumDesc(MessageLib.Invalid);
                }
                m_boolValid = false;
            }

            if (Action == null)
            {
                if (Message.Length == 0)
                {
                    Message = "Action ID " + General.EnumDesc(MessageLib.Invalid);
                }
                else
                {
                    Message += Environment.NewLine + "Action ID " + General.EnumDesc(MessageLib.Invalid);
                }
                m_boolValid = false;
            }

            return m_boolValid;
        }
        private bool isUpdateCartValid(string CartID, SAction Action, ref string m_strMessage)
        {
            bool m_result = true;
            try
            {
                CatalogCartVM m_objPackageVM = GetCartDetail(CartID);
                if (m_objPackageVM.CatalogCartID != null)
                {
                    #region Case ActionID Package
                    switch (Action.ActionID)
                    {
                        case "Create":
                            if (m_objPackageVM.StatusID > 0)
                            {
                                throw new Exception("Status " + General.EnumDesc(MessageLib.Invalid) + ". [" + CartID + "]");
                            }
                            break;
                        case "Resubmit":
                            if (m_objPackageVM.StatusID > 0)
                            {
                                throw new Exception("Status " + General.EnumDesc(MessageLib.Invalid) + ". [" + CartID + "]");
                            }
                            break;
                        case "Approve":
                            if (m_objPackageVM.StatusID != 1)
                            {
                                throw new Exception("Status " + General.EnumDesc(MessageLib.invalid) + ". [" + CartID + "]");
                            }
                            break;
                        case "Reject":
                            if (m_objPackageVM.StatusID != 1)
                            {
                                throw new Exception("Status " + General.EnumDesc(MessageLib.Invalid) + ". [" + CartID + "]");
                            }
                            break;
                        case "Revise":
                            if (m_objPackageVM.StatusID != 1)
                            {
                                throw new Exception("Status " + General.EnumDesc(MessageLib.Invalid) + ". [" + CartID + "]");
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                }

                if (m_strMessage == string.Empty)
                {
                    TCatalogCartDA m_objTCatalogCartDA = new TCatalogCartDA();
                    m_objTCatalogCartDA.ConnectionStringName = Global.ConnStrConfigName;

                    string m_strTransaction = "ServiceActionCreate";
                    object m_objConnection = m_objTCatalogCartDA.BeginTrans(m_strTransaction);

                    m_objTCatalogCartDA.Data.CatalogCartID = m_objPackageVM.CatalogCartID;
                    m_objTCatalogCartDA.Select();

                    #region Case Update StatusID Package
                    switch (Action.ActionID)
                    {
                        case "Create":
                            m_objTCatalogCartDA.Data.StatusID = 1;
                            break;
                        case "Resubmit":
                            m_objTCatalogCartDA.Data.StatusID = 1;
                            break;
                        case "Approve":
                            m_objTCatalogCartDA.Data.StatusID = 2;
                            break;
                        case "Reject":
                            m_objTCatalogCartDA.Data.StatusID = 99;
                            break;
                        case "Revise":
                            m_objTCatalogCartDA.Data.StatusID = 0;
                            break;
                        default:
                            break;
                    }
                    #endregion

                    m_objTCatalogCartDA.Update(true, m_objConnection);
                    if (m_objTCatalogCartDA.Success)
                    {
                        m_strMessage = "";
                    }
                    else
                    {
                        throw new Exception("Update Package Error. [" + m_objTCatalogCartDA.Message + "]");
                    }

                    m_objTCatalogCartDA.EndTrans(ref m_objConnection, true, m_strTransaction);
                }
            }
            catch (Exception ex)
            {
                m_result = false;
                m_strMessage = ex.Message;
            }
            return m_result;
        }
        #endregion
    }
}
