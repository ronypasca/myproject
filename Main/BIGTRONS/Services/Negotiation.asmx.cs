using com.SML.BIGTRONS.Controllers;
using com.SML.BIGTRONS.DataAccess;
using com.SML.BIGTRONS.Enum;
using com.SML.BIGTRONS.Models;
using com.SML.BIGTRONS.ViewModels;
using com.SML.Lib.Common;
using Ext.Net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace com.SML.BIGTRONS.Services
{
    /// <summary>
    /// Summary description for Negotiation
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Negotiation : BaseController
    {

        [WebMethod]
        public string SendFPT(FPTWSVM FPT)
        {
            List<string> m_lstmsg = new List<string>();
            SaveFPT(FPT, ref m_lstmsg);
            return !m_lstmsg.Any() ? string.Empty : string.Join(", ", m_lstmsg);
        }
        [WebMethod]
        public List<FPTStatusWSVM> GetFPTStatus(string FPTID)
        {
            List<FPTStatusWSVM> m_lstFPTStatusWSVM = new List<FPTStatusWSVM>();
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
                    FPTStatusWSVM m_objFPTStatusWSVM = new FPTStatusWSVM();
                    m_objFPTStatusWSVM.FPTStatusID = m_drDFPTStatusDA[FPTStatusVM.Prop.FPTStatusID.Name].ToString();
                    m_objFPTStatusWSVM.FPTID = m_drDFPTStatusDA[FPTStatusVM.Prop.FPTID.Name].ToString();
                    m_objFPTStatusWSVM.StatusDateTimeStamp = Convert.ToDateTime(m_drDFPTStatusDA[FPTStatusVM.Prop.StatusDateTimeStamp.Name]);
                    m_objFPTStatusWSVM.StatusID = (int)m_drDFPTStatusDA[FPTStatusVM.Prop.StatusID.Name];
                    m_objFPTStatusWSVM.Remarks = m_drDFPTStatusDA[FPTStatusVM.Prop.Remarks.Name].ToString();
                    m_objFPTStatusWSVM.StatusDesc = m_drDFPTStatusDA[FPTStatusVM.Prop.StatusDesc.Name].ToString();
                    m_lstFPTStatusWSVM.Add(m_objFPTStatusWSVM);
                }
            }

            return m_lstFPTStatusWSVM;
        }

        [WebMethod]
        public string UpdateFPTStatus(FPTStatusUpdateWSVM Status)
        {
            string m_message = string.Empty;
            TimeSpan ts = new TimeSpan(0, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
            Status.StatusDateTimeStamp = Status.StatusDateTimeStamp.Date + ts;
            InsertDFPTStatus(Status.FPTID, Status.StatusID, Status.StatusDateTimeStamp, ref m_message, Status.Remarks);
            return m_message;
        }

        private bool SaveFPT(FPTWSVM FPT, ref List<string> m_lstmsg)
        {

            MFPTDA m_objMFPTDA = new MFPTDA();
            FPTVM m_objFPTVM = new FPTVM();
            DFPTStatusDA m_objDFPTStatusDA = new DFPTStatusDA();
            DFPTDeviationsDA m_objDFPTDeviationsDA = new DFPTDeviationsDA();
            DFPTAdditionalInfoDA m_objDFPTAdditionalInfoDA = new DFPTAdditionalInfoDA();

            m_objMFPTDA.ConnectionStringName = Global.ConnStrConfigName;
            string m_strTransName = "MFPT";
            object m_objDBConnection = null;
            m_objDBConnection = m_objMFPTDA.BeginTrans(m_strTransName);
            try
            {
                string m_strBU = string.IsNullOrEmpty(FPT.BusinessUnitID) ? null : FPT.BusinessUnitID.Substring(FPT.BusinessUnitID.LastIndexOf('-') + 1);
                string m_idBU = null;
                if (m_strBU != null)
                {
                    m_idBU = getByDesc(m_strBU.Trim()).Any() ? getByDesc(m_strBU.Trim()).ToList().FirstOrDefault().BusinessUnitID : null;
                }

                FPTDeviationsVM m_FPTDeviationsVM = new FPTDeviationsVM();

                m_objFPTVM.FPTID = FPT.FPTID;
                m_objFPTVM.Descriptions = FPT.Descriptions;
                //m_objFPTVM.ClusterID = null;
                m_objFPTVM.ProjectID = string.IsNullOrEmpty(FPT.ProjectID) ? null : FPT.ProjectID;
                m_objFPTVM.DivisionID = string.IsNullOrEmpty(FPT.DivisionID) ? null : FPT.DivisionID;
                m_objFPTVM.BusinessUnitID = m_idBU;// string.IsNullOrEmpty(FPT.BusinessUnitID) ? null : FPT.BusinessUnitID;
                m_objFPTVM.P3Value = FPT.P3Value;
                m_objFPTVM.DocumentComplete = false;
                
                if (!IsSaveValid(m_objFPTVM, ref m_lstmsg))
                {
                    return false;
                }

                #region MFPT
                MFPT m_objMFPT = new MFPT();
                m_objMFPT.FPTID = FPT.FPTID;
                m_objMFPTDA.Data = m_objMFPT;
                if (FPT.Action != "Add") m_objMFPTDA.Select();

                m_objMFPT.FPTID = FPT.FPTID;
                m_objMFPT.Descriptions = m_objFPTVM.Descriptions;
                m_objMFPT.ClusterID = m_objFPTVM.ClusterID;
                m_objMFPT.ProjectID = m_objFPTVM.ProjectID;
                m_objMFPT.DivisionID = m_objFPTVM.DivisionID;
                m_objMFPT.BusinessUnitID = m_objFPTVM.BusinessUnitID;
                m_objMFPT.IsSync = true;
                m_objMFPT.CreatedBy = "ETT";


                m_objDBConnection = m_objMFPTDA.BeginTrans(m_strTransName);
                if (FPT.Action == "Add")
                    m_objMFPTDA.Insert(true, m_objDBConnection);
                else
                    m_objMFPTDA.Update(true, m_objDBConnection);

                if (!m_objMFPTDA.Success || m_objMFPTDA.Message != string.Empty)
                {
                    m_objMFPTDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                    m_lstmsg.Add(m_objMFPTDA.Message);
                    return false;
                }

                #endregion

                #region DFPTStatus
                m_objDFPTStatusDA.ConnectionStringName = Global.ConnStrConfigName;
                DFPTStatus m_objDFPTStatus = new DFPTStatus();
                if (FPT.Action == "Add")
                {
                    //New Status
                    string m_insmessage = string.Empty;
                    if (!InsertDFPTStatus(FPT.FPTID, (int)FPTStatusTypes.New, DateTime.Now, ref m_insmessage, null, m_objDBConnection))
                    {
                        m_objDFPTStatusDA.EndTrans(ref m_objDBConnection, m_strTransName);
                        m_lstmsg.Add(m_insmessage);
                        return false;
                    }

                    //Deviation Status
                    int m_intDeviationStatusID = (int)FPTStatusTypes.FPTUnverified;

                    m_objDFPTStatus.FPTStatusID = string.Empty;
                    m_objDFPTStatus.FPTID = FPT.FPTID;
                    m_objDFPTStatus.StatusDateTimeStamp = DateTime.Now.AddMilliseconds(100);
                    m_objDFPTStatus.StatusID = m_intDeviationStatusID;
                    m_objDFPTStatusDA.Data = m_objDFPTStatus;
                    m_objDFPTStatusDA.Insert(true, m_objDBConnection);
                    if (!m_objDFPTStatusDA.Success || m_objDFPTStatusDA.Message != string.Empty)
                    {
                        m_objDFPTStatusDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                        m_lstmsg.Add("DFPTStatus");
                        m_lstmsg.Add(m_objDFPTStatusDA.Message);
                        return false;
                    }
                }

                #endregion

                #region DFPTDeviations
                m_objDFPTDeviationsDA.ConnectionStringName = Global.ConnStrConfigName;
                DFPTDeviations m_objDFPTDeviations = new DFPTDeviations();
                m_objDFPTDeviations.FPTDeviationID = string.Empty;
                m_objDFPTDeviationsDA.Data = m_objDFPTDeviations;
                m_objDFPTDeviations.FPTDeviationID = string.Empty;
                m_objDFPTDeviations.FPTID = FPT.FPTID;
                m_objDFPTDeviations.DeviationTypeID = General.EnumDesc(DeviationTypes.FPTVerification);
                m_objDFPTDeviations.RefNumber = "-";
                m_objDFPTDeviations.RefTitle = "Sync ETT";
                m_objDFPTDeviations.RefDate = DateTime.Now;
                m_objDFPTDeviationsDA.Insert(true, m_objDBConnection);
                if (!m_objDFPTDeviationsDA.Success || m_objDFPTDeviationsDA.Message != string.Empty)
                {
                    m_objDFPTDeviationsDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                    m_lstmsg.Add("DFPTDeviations");
                    m_lstmsg.Add(m_objDFPTDeviationsDA.Message);
                    return false;
                }
                //m_strFPTID = m_objDFPTDeviationsDA.Data.FPTID;

                #endregion

                #region DFPTAdditionalInfo
                m_objDFPTAdditionalInfoDA.ConnectionStringName = Global.ConnStrConfigName;
                DFPTAdditionalInfo m_objDFPTAdditionalInfo = new DFPTAdditionalInfo();
                if (FPT.Action == "Add")
                {
                    //DELETE DFPTAdditionalInfo
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    List<object> m_lstFilter = new List<object>();

                    m_lstFilter = new List<object>();
                    m_lstFilter.Add(Operator.Equals);
                    m_lstFilter.Add(FPT.FPTID);
                    m_objFilter.Add(FPTAdditionalInfoVM.Prop.FPTID.Map, m_lstFilter);

                    m_objDFPTAdditionalInfoDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                    if (m_objDFPTAdditionalInfoDA.Message == General.EnumDesc(MessageLib.NotExist)) m_objDFPTAdditionalInfoDA.Message = "";
                }
                //Add P3
                m_objDFPTAdditionalInfo = new DFPTAdditionalInfo() { FPTAdditionalInfoID = Guid.NewGuid().ToString().Replace("-", ""), FPTID = FPT.FPTID, FPTAdditionalInfoItemID = "8", Value = FPT.P3Value.ToString() };
                m_objDFPTAdditionalInfoDA.Data = m_objDFPTAdditionalInfo;
                m_objDFPTAdditionalInfoDA.Insert(true, m_objDBConnection);
                if (!m_objDFPTAdditionalInfoDA.Success || m_objDFPTAdditionalInfoDA.Message != string.Empty)
                {
                    m_objDFPTAdditionalInfoDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                    m_lstmsg.Add("AdditionalInfo p3");
                    m_lstmsg.Add(m_objDFPTAdditionalInfoDA.Message);
                    return false;
                }
                //Add Company
                m_objDFPTAdditionalInfo = new DFPTAdditionalInfo() { FPTAdditionalInfoID = Guid.NewGuid().ToString().Replace("-", ""), FPTID = FPT.FPTID, FPTAdditionalInfoItemID = "9", Value = FPT.CompanyID.ToString() };
                m_objDFPTAdditionalInfoDA.Data = m_objDFPTAdditionalInfo;
                m_objDFPTAdditionalInfoDA.Insert(true, m_objDBConnection);
                if (!m_objDFPTAdditionalInfoDA.Success || m_objDFPTAdditionalInfoDA.Message != string.Empty)
                {
                    m_objDFPTAdditionalInfoDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                    m_lstmsg.Add("AdditionalInfo Company");
                    m_lstmsg.Add(m_objDFPTAdditionalInfoDA.Message);
                    return false;
                }
                //Add TenderType
                m_objDFPTAdditionalInfo = new DFPTAdditionalInfo() { FPTAdditionalInfoID = Guid.NewGuid().ToString().Replace("-", ""), FPTID = FPT.FPTID, FPTAdditionalInfoItemID = "10", Value = FPT.TenderType.ToString() };
                m_objDFPTAdditionalInfoDA.Data = m_objDFPTAdditionalInfo;
                m_objDFPTAdditionalInfoDA.Insert(true, m_objDBConnection);
                if (!m_objDFPTAdditionalInfoDA.Success || m_objDFPTAdditionalInfoDA.Message != string.Empty)
                {
                    m_objDFPTAdditionalInfoDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                    m_lstmsg.Add("AdditionalInfo TenderType");
                    m_lstmsg.Add(m_objDFPTAdditionalInfoDA.Message);
                    return false;
                }

                #endregion

                if (!m_objMFPTDA.Success || m_objMFPTDA.Message != string.Empty)
                    m_lstmsg.Add(m_objMFPTDA.Message);
                else
                    m_objMFPTDA.EndTrans(ref m_objDBConnection, true, m_strTransName);

                LogFile(JSON.Serialize(FPT));
            }
            catch (Exception ex)
            {
                m_lstmsg.Add(ex.Message);
                m_objMFPTDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
            }
            if (m_lstmsg.Count <= 0)
            {
                return true;
            }
            return false;
        }

        private bool IsSaveValid(FPTVM FPTVM, ref List<string> m_lstmsg)
        {
            if (!string.IsNullOrEmpty(FPTVM.Descriptions) && FPTVM.Descriptions.Length > 254)
            {
                m_lstmsg.Add("Descriptions Too Long!");
                return false;
            }

            if (!string.IsNullOrEmpty(FPTVM.ClusterID) && !IsExist("MCluster", FPTVM.ClusterID))
            {
                m_lstmsg.Add("Cluster ID Invalid!");
                return false;
            }
            if (!string.IsNullOrEmpty(FPTVM.ProjectID) && !IsExist("MProject", FPTVM.ProjectID))
            {
                m_lstmsg.Add("Project ID Invalid!");
                return false;
            }
            if (!string.IsNullOrEmpty(FPTVM.DivisionID) && !IsExist("MDivision", FPTVM.DivisionID))
            {
                m_lstmsg.Add("DivisionID ID Invalid!");
                return false;
            }
            if (!string.IsNullOrEmpty(FPTVM.BusinessUnitID) && !IsExist("MBusinessUnit", FPTVM.BusinessUnitID))
            {
                m_lstmsg.Add("BusinessUnitID ID Invalid!");
                return false;
            }
            return true;
        }

        private bool IsExist(string type, string ID)
        {
            object m_objDA = new object();
            switch (type)
            {
                case  "MProject" :
                    m_objDA = new MProjectDA();
                    ((MProjectDA)(m_objDA)).ConnectionStringName = Global.ConnStrConfigName;
                    break;
                case "MCluster":
                    m_objDA = new MClusterDA();
                    ((MClusterDA)(m_objDA)).ConnectionStringName = Global.ConnStrConfigName;
                    break;
                case "MDivision":
                    m_objDA = new MDivisionDA();
                    ((MDivisionDA)(m_objDA)).ConnectionStringName = Global.ConnStrConfigName;
                    break;
                case "MBusinessUnit":
                    m_objDA = new MBusinessUnitDA();
                    ((MBusinessUnitDA)(m_objDA)).ConnectionStringName = Global.ConnStrConfigName;
                    break;
                default:
                    return false;
            }
                       

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ID);
            
            switch (type)
            {
                case "MProject":
                    m_objFilter.Add(ProjectVM.Prop.ProjectID.Map, m_lstFilter);
                    break;
                case "MCluster":
                    m_objFilter.Add(ClusterVM.Prop.ClusterID.Map, m_lstFilter);
                    break;
                case "MDivision":
                    m_objFilter.Add(DivisionVM.Prop.DivisionID.Map, m_lstFilter);
                    break;
                case "MBusinessUnit":
                    m_objFilter.Add(BusinessUnitVM.Prop.BusinessUnitID.Map, m_lstFilter);
                    break;
                default:
                    return false;
            }


            Dictionary<int, DataSet> m_dicDA = new Dictionary<int, DataSet>();
            switch (type)
            {
                case "MProject":
                    m_dicDA = ((MProjectDA)(m_objDA)).SelectBC(0, 0, true, null, m_objFilter, null, null, null, null);
                    break;
                case "MCluster":
                    m_dicDA = ((MClusterDA)(m_objDA)).SelectBC(0, 0, true, null, m_objFilter, null, null, null, null);
                    break;
                case "MDivision":
                    m_dicDA = ((MDivisionDA)(m_objDA)).SelectBC(0, 0, true, null, m_objFilter, null, null, null, null);
                    break;
                case "MBusinessUnit":
                    m_dicDA = ((MBusinessUnitDA)(m_objDA)).SelectBC(0, 0, true, null, m_objFilter, null, null, null, null);
                    break;
                default:
                    return false;
            }

            int m_intCount = 0;
            foreach (KeyValuePair<int, DataSet> m_kvpBL in m_dicDA)
            {
                m_intCount = m_kvpBL.Key;
                break;
            }
            
            
            return m_intCount > 0;
        }

        private List<BusinessUnitVM> getByDesc(string desc)
        {

            List<BusinessUnitVM> m_lstBusinessUnitVM = new List<BusinessUnitVM>();
            MBusinessUnitDA m_objBusinessUnitDA = new MBusinessUnitDA();
            m_objBusinessUnitDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BusinessUnitVM.Prop.BusinessUnitID.MapAlias);
            m_lstSelect.Add(BusinessUnitVM.Prop.BusinessUnitDesc.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(desc);
            m_objFilter.Add(BusinessUnitVM.Prop.BusinessUnitDesc.Map, m_lstFilter);


            Dictionary<int, DataSet> m_dicBusinessUnitDA = m_objBusinessUnitDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objBusinessUnitDA.Success)
            {
                m_lstBusinessUnitVM =
                (from DataRow m_drBusinessUnitDA in m_dicBusinessUnitDA[0].Tables[0].Rows
                 select new BusinessUnitVM()
                 {
                     BusinessUnitID = m_drBusinessUnitDA[BusinessUnitVM.Prop.BusinessUnitID.Name].ToString(),
                     BusinessUnitDesc = m_drBusinessUnitDA[BusinessUnitVM.Prop.BusinessUnitDesc.Name].ToString()
                 }).ToList();
            }
            return m_lstBusinessUnitVM;

        }

    }
}
