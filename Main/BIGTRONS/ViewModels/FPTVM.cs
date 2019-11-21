using com.SML.Lib.Common;
using com.SML.BIGTRONS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;

namespace com.SML.BIGTRONS.ViewModels
{
    public class FPTVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string FPTID { get; set; }
        public string Descriptions { get; set; }
        public string ClusterID { get; set; }
        public string ProjectID { get; set; }
        public string DivisionID { get; set; }
        public string BusinessUnitID { get; set; }
        public bool IsSync { get; set; }
        public string NegoConfigBUnitDesc { get; set; }
        public string ClusterDesc { get; set; }
        public string ProjectDesc { get; set; }
        public string DivisionDesc { get; set; }
        public string BusinessUnitDesc { get; set; }
        public string Projects { get; set; }
        public string BudgetPlans { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int AdditionalInfo1 { get; set; }
        public string AdditionalInfo1Desc { get; set; }

        public List<FPTVendorParticipantsVM> ListBFPTVendorParticipantsVM { get; set; }
        public List<NegotiationBidStructuresVM> ListNegotiationBidStructuresVM { get; set; }
        public string Vendors { get; set; }
        public FPTDeviationsVM FPTDeviationsVM { get; set; }
        public bool DocumentComplete { get; set; }
        public bool ByUpload { get; set; }
        public string TCTypeUpload { get; set; }
        public string TCType { get; set; }
        public List<ProjectVM> ListProject { get; set; }
        public List<NegotiationConfigurationsVM> ListNegotiationConfigurationsVM { get; set; }
        public string Schedule { get; set; }
        public DateTime ScheduleDate { get; set; }
        public int FPTStatusID { get; set; }
        public DateTime? ScheduleDateFPT
        {
            get
            {
                if (!string.IsNullOrEmpty(Schedule))
                {
                    try
                    {
                        DateTime theDate = DateTime.Parse(Schedule);
                        return DateTime.Parse(Schedule);
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public TimeSpan ScheduleTimeHour
        {
            get
            {
                if (!string.IsNullOrEmpty(Schedule))
                {
                    DateTime theDate = DateTime.Parse(this.Schedule);
                    return TimeSpan.Parse(theDate.ToString(Global.ShortTimeFormat));
                }
                return new TimeSpan(0, 0, 0);
            }
            //set
            //{
            //DateTime theDate = DateTime.Now;
            //if (!string.IsNullOrEmpty(Schedule))                   
            //    theDate = DateTime.Parse(this.Schedule);                
            //string ret = theDate.ToString(Global.DefaultDateFormat);
            //this.Schedule = ret + " " + value.ToString(Global.ShortTimeFormat);
            //}            
        }

        public DateTime FPTScheduleStart { get; set; }
        public DateTime FPTScheduleEnd { get; set; }
        public string FPTScheduleStartManual { get; set; }
        public string FPTScheduleEndManual { get; set; }
        public string Duration { get; set; }
        public string DurationManual { get; set; }
        public string MaintenancePeriod { get; set; }
        public string Guarantee { get; set; }
        public string ContractType { get; set; }
        public string PaymentMethod { get; set; }
        public decimal P3Value { get; set; }
        public string NegoLevel { get; set; }
        public string NegoRound { get; set; }
        public string TRMLead { get; set; }
        public string TRMLeadDesc { get; set; }
        public string NegoRoundTime { get; set; }
        public List<FPTStatusVM> ListFPTStatusVM { get; set; }
        public List<TCMembersVM> ListTCMembers { get; set; }
        public string TaskID { get; set; }
        public int CurrentApprovalLvl { get; set; }
        public string TaskStatus { get; set; }
        public string LastStatus
        {
            get
            {
                string retVal = "";
                if (ListFPTStatusVM != null && ListFPTStatusVM.Any())
                {
                    retVal = ListFPTStatusVM.OrderByDescending(x => x.CreatedDate).FirstOrDefault().StatusDesc;
                }

                return retVal;
            }
        }

        public string VendorName { get; set; }
        public string TCName { get; set; }
        public string ListVendor { get; set; }
        public string Source { get; set; }



        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class FPTID
            {
                public static string Desc { get { return "FPT Ref.ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class TaskID
            {
                public static string Desc { get { return "TaskID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class CreatedDate
            {
                public static string Desc { get { return "CreatedDate"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class CurrentApprovalLvl
            {
                public static string Desc { get { return "Current Approval Level"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Descriptions
            {
                public static string Desc { get { return "Descriptions"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ClusterID
            {
                public static string Desc { get { return "ClusterID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ClusterDesc
            {
                public static string Desc { get { return "Cluster"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ProjectID
            {
                public static string Desc { get { return "ProjectID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ProjectDesc
            {
                public static string Desc { get { return "Project"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class DivisionID
            {
                public static string Desc { get { return "DivisionID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class DivisionDesc
            {
                public static string Desc { get { return "Division"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BusinessUnitID
            {
                public static string Desc { get { return "BusinessUnitID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class IsSync
            {
                public static string Desc { get { return "Is Sync"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BusinessUnitDesc
            {
                public static string Desc { get { return "Business Unit"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Projects
            {
                public static string Desc { get { return "Projects"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BudgetPlans
            {
                public static string Desc { get { return "Budget Plan"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ListBFPTVendorParticipantsVM
            {
                public static string Desc { get { return "ListBFPTVendorParticipantsVM"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Vendors
            {
                public static string Desc { get { return "Vendor"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ListNegotiationConfigurationsVM
            {
                public static string Desc { get { return "List Negotiation ConfigurationsVM"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Schedule
            {
                public static string Desc { get { return "Negotiation Schedule"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class NegoLevel
            {
                public static string Desc { get { return "Negotiation Level"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class NegoConfigBUnitDesc
            {
                public static string Desc { get { return "Business Unit"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class NegoRound
            {
                public static string Desc { get { return "Negotiation Round"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class NegoRoundTime
            {
                public static string Desc { get { return "Round Time"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class TRMLead
            {
                public static string Desc { get { return "TRM Negotiation Lead ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class TRMLeadDesc
            {
                public static string Desc { get { return "TRM Negotiation Lead"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ListFPTStatusVM
            {
                public static string Desc { get { return "ListFPTStatusVM"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class LastStatus
            {
                public static string Desc { get { return "Last Status"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class FPTDeviationsVM
            {
                public static string Desc { get { return "FPT Deviations"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class DocumentComplete
            {
                public static string Desc { get { return "Document Complete"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class TCType
            {
                public static string Desc { get { return "TC Type"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class FPTStatusID
            {
                public static string Desc { get { return "FPTStatusID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class TaskStatus
            {
                public static string Desc { get { return "TaskStatus"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class AdditionalInfo1
            {
                public static string Desc { get { return "Additional Info"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class AdditionalInfo1Desc
            {
                public static string Desc { get { return "Additional Desc"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            /// <summary>
            /// Function for mapping ViewModel Field with its Model Field
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="variable">ViewModel Field to map</param>
            /// <param name="withAlias">Whether to set its alias, usually for select list</param>
            /// <returns>Mapped Field Name</returns>
            private static string Mapping<T>(Expression<Func<T>> variable, bool withAlias = false)
            {
                string m_strFieldName = General.GetVariableName(variable);
                return Map(m_strFieldName, withAlias);
            }

            /// <summary>
            /// Function for mapping ViewModel Field with its Model Field (by ViewModel Field Name)
            /// </summary>
            /// <param name="fieldName">ViewModel Field Name to map</param>
            /// <param name="withAlias">Whether to set its alias, usually for select list</param>
            /// <returns>Mapped Field Name or empty string if not found</returns>
            public static string Map(string fieldName, bool withAlias = false)
            {
                string m_strReturn = string.Empty;
                MFPT m_objMFPT = new MFPT();
                MTasks m_objMTasks = new MTasks();
                MEmployee m_objMEmployee = new MEmployee();
                CNegotiationConfigurations m_objCNegotiationConfigurations = new CNegotiationConfigurations();
                MCluster m_objMCluster = new MCluster();
                MProject m_objMProject = new MProject();
                MDivision m_objMDivision = new MDivision();
                MBusinessUnit m_objMBusinessUnit = new MBusinessUnit();

                if (fieldName == FPTID.Name)
                    m_strReturn = m_objMFPT.Name + "." + fieldName;
                else if (fieldName == TaskID.Name)
                    m_strReturn = m_objMTasks.Name + "." + fieldName;
                else if (fieldName == CreatedDate.Name)
                    m_strReturn = m_objMFPT.Name + "." + fieldName;
                else if (fieldName == Descriptions.Name)
                    m_strReturn = m_objMFPT.Name + "." + fieldName;
                else if (fieldName == ClusterID.Name)
                    m_strReturn = m_objMCluster.Name + "." + fieldName;
                else if (fieldName == ClusterDesc.Name)
                    m_strReturn = m_objMCluster.Name + "." + fieldName;
                else if (fieldName == ProjectID.Name)
                    m_strReturn = m_objMProject.Name + "." + fieldName;
                else if (fieldName == ProjectDesc.Name)
                    m_strReturn = m_objMProject.Name + "." + fieldName;
                else if (fieldName == DivisionID.Name)
                    m_strReturn = m_objMDivision.Name + "." + fieldName;
                else if (fieldName == DivisionDesc.Name)
                    m_strReturn = m_objMDivision.Name + "." + fieldName;
                else if (fieldName == BusinessUnitID.Name)
                    m_strReturn = m_objMBusinessUnit.Name + "." + fieldName;
                else if (fieldName == IsSync.Name)
                    m_strReturn = m_objMFPT.Name + "." + fieldName;
                else if (fieldName == BusinessUnitDesc.Name)
                    m_strReturn = m_objMBusinessUnit.Name + "." + "Descriptions";
                else if (fieldName == Schedule.Name)
                    m_strReturn = m_objCNegotiationConfigurations.Name + "." + fieldName;
                else if (fieldName == AdditionalInfo1.Name)
                    m_strReturn = "[DFPTAdditionalInfo].FPTAdditionalInfoItemID";
                else if (fieldName == AdditionalInfo1Desc.Name)
                    m_strReturn = "[DFPTAdditionalInfo].FPTAdditionalInfoItemDesc";
                m_strReturn += (withAlias ? " AS [" + fieldName + "]" : "");
                return m_strReturn;
            }
        }

        #endregion

        /// <summary>
        /// Function for checking if Field is Key
        /// </summary>
        /// <param name="FieldName">Name of Field to check</param>
        public bool IsKey(string fieldName)
        {
            PropertyInfo m_pifBudgetPlanTypeVM = this.GetType().GetProperty(fieldName);
            return m_pifBudgetPlanTypeVM != null && Attribute.GetCustomAttribute(m_pifBudgetPlanTypeVM, typeof(KeyAttribute)) != null;
        }
    }
}