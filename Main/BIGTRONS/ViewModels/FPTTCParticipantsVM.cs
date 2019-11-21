using com.SML.Lib.Common;
using com.SML.BIGTRONS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace com.SML.BIGTRONS.ViewModels
{
    public class FPTTCParticipantsVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string FPTTCParticipantID { get; set; }
        public string TCMemberID { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string FPTID { get; set; }
        public bool StatusID { get; set; }
        public bool IsDelegation { get; set; }

        public string ProjectDesc { get; set; }
        public string BudgetPlanID { get; set; }
        public string FirstName { get; set; }
        public string ProjectID { get; set; }
        public string NegotiationConfigID { get; set; }
        public bool Checked { get; set; }
        public string ParameterValue { get; set; }
        public decimal BidValue { get; set; }
        public decimal BidFee { get; set; }
        public decimal BidAfterFee { get; set; }
        public string NegotiationEntryID { get; set; }
        public decimal BudgetPlanDefaultValue { get; set; }
        public decimal BudgetPlanDefaultFee { get; set; }
        public decimal BudgetPlanDefaultValueAfterFee { get; set; }
        public decimal LastOffer { get; set; }

        public bool IsProposed { get; set; }
        public string IsProposedWinner { get; set; }
        public string TCName { get; set; }
        public string RecommendationRemark { get; set; }
        public bool IsWinner { get; set; }
        public string BPVersionName { get; set; }
        public string RoundID { get; set; }
        public string VendorEmail { get; set; }

        public DateTime NegoDate { get; set; }

        public string BusinessUnitID { get; set; }
        public string BusinessUnitDesc { get; set; }

        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class FPTTCParticipantID
            {
                public static string Desc { get { return "ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class TCMemberID
            {
                public static string Desc { get { return "TCMember ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class EmployeeID
            {
                public static string Desc { get { return "Employee ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class EmployeeName
            {
                public static string Desc { get { return "Employee Name"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class FPTID
            {
                public static string Desc { get { return "FPT ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class StatusID
            {
                public static string Desc { get { return "Status ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class IsDelegation
            {
                public static string Desc { get { return "Is Delegation"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class ProjectDesc
            {
                public static string Desc { get { return "Project Desc"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class FirstName
            {
                public static string Desc { get { return "First Name"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class MiddleName
            {
                public static string Desc { get { return "Middle Name"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class LastName
            {
                public static string Desc { get { return "Last Name"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ProjectID
            {
                public static string Desc { get { return "Project ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BudgetPlanID
            {
                public static string Desc { get { return "Budget Plan ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class NegotiationConfigID
            {
                public static string Desc { get { return "NegotiationConfigID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class NegotiationConfigTypeID
            {
                public static string Desc { get { return "NegotiationConfigTypeID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ParameterValue
            {
                public static string Desc { get { return "ParameterValue"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ParameterValue2
            {
                public static string Desc { get { return "ParameterValue"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BPVersionName
            {
                public static string Desc { get { return "BP Name"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Email
            {
                public static string Desc { get { return "Email"; } }
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
                DFPTTCParticipants m_objDFPTTCParticipants = new DFPTTCParticipants();
                CNegotiationConfigurations m_objCNegotiationConfigurations = new CNegotiationConfigurations();
                MEmployee m_objMEmployee = new MEmployee();
                TBudgetPlan m_objTBudgetPlan = new TBudgetPlan();
                MProject m_objMProject = new MProject();
                DBudgetPlanVersion m_objDBudgetPlanVersion = new DBudgetPlanVersion();

                if (fieldName == FPTTCParticipantID.Name)
                    m_strReturn = m_objDFPTTCParticipants.Name + "." + fieldName;
                else if (fieldName == TCMemberID.Name)
                    m_strReturn = m_objDFPTTCParticipants.Name + "." + fieldName;
                else if (fieldName == EmployeeID.Name)
                    m_strReturn = m_objMEmployee.Name + "." + fieldName;
                else if (fieldName == EmployeeName.Name)
                    m_strReturn = $"REPLACE({m_objMEmployee.Name}.{FirstName.Name} + {m_objMEmployee.Name}.{MiddleName.Name} + {m_objMEmployee.Name}.{LastName.Name},'..','')";
                else if (fieldName == FPTID.Name)
                    m_strReturn = m_objDFPTTCParticipants.Name + "." + fieldName;
                else if (fieldName == StatusID.Name)
                    m_strReturn = m_objDFPTTCParticipants.Name + "." + fieldName;
                else if (fieldName == ProjectDesc.Name)
                    m_strReturn = m_objMProject.Name + "." + fieldName;
                else if (fieldName == FirstName.Name)
                    m_strReturn = m_objMEmployee.Name + "." + fieldName;
                else if (fieldName == Email.Name)
                    m_strReturn = m_objMEmployee.Name + "." + "Email";
                else if (fieldName == StatusID.Name)
                    m_strReturn = m_objDFPTTCParticipants.Name + "." + fieldName;
                else if (fieldName == IsDelegation.Name)
                    m_strReturn = m_objDFPTTCParticipants.Name + "." + fieldName;
                else if (fieldName == ProjectID.Name)
                    m_strReturn = m_objMProject.Name + "." + fieldName;
                else if (fieldName == BudgetPlanID.Name)
                    m_strReturn = m_objTBudgetPlan.Name + "." + fieldName;
                else if (fieldName == NegotiationConfigID.Name)
                    m_strReturn = m_objDFPTTCParticipants.Name + "." + fieldName;
                else if (fieldName == NegotiationConfigTypeID.Name)
                    m_strReturn = m_objCNegotiationConfigurations.Name + "." + fieldName;
                else if (fieldName == ParameterValue.Name)
                    m_strReturn = m_objCNegotiationConfigurations.Name + "." + fieldName;
                else if (fieldName == ParameterValue2.Name)
                    m_strReturn = m_objCNegotiationConfigurations.Name + "." + fieldName;
                else if (fieldName == BPVersionName.Name)
                    m_strReturn = m_objDBudgetPlanVersion.Name + ".Description";



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
            PropertyInfo m_pifTaskTypesVM = this.GetType().GetProperty(fieldName);
            return m_pifTaskTypesVM != null && Attribute.GetCustomAttribute(m_pifTaskTypesVM, typeof(KeyAttribute)) != null;
        }
    }
}