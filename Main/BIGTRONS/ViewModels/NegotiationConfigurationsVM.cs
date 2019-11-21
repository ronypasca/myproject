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
    public class NegotiationConfigurationsVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string NegotiationConfigID { get; set; }
        public string NegotiationConfigTypeID { get; set; }
        //public string FPTProjectID { get; set; }
        public string TaskID { get; set; }
        public string ParameterValue { get; set; }
        public string ParameterValue2 { get; set; }
        public string FPTDesc { get; set; }
        public string Descriptions { get; set; }
        public string FPTID { get; set; } 
        public string FPTStatusID { get; set; }
        public string FPTStatusDesc { get; set; }
        public string StatusID { get; set; }
        public string StatusDesc { get; set; }
        public string TRMLeadDesc { get; set; }
        public string TCLeadName { get; set; }
        public string TCTypeID { get; set; }
        public string TCTypeDesc { get; set; }
        public string ProjectDesc { get; set; }
        public string BudgetPlanID { get; set; }
        public string FirstName { get; set; }
        public string ProjectID { get; set; }
        public string VendorID { get; set; }
        public string VendorName { get; set; }
        public string BudgetPlan { get; set; }
        public string BudgetPlanDescription { get; set; }

        public string NegoBUnitDesc { get; set; }
        public string TCMemberID{ get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string BusinessUnitDesc { get; set; }
        public int CurrentApprovalLvl { get; set; }
        public string BusinessUnitIDTC { get; set; }
        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class NegotiationConfigID
            {
                public static string Desc { get { return "ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class NegotiationConfigTypeID
            {
                public static string Desc { get { return "Negotiation Config Type ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class TaskID
            {
                public static string Desc { get { return "Task ID"; } }
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
            public static class ParameterValue
            {
                public static string Desc { get { return "Parameter Value"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ParameterValue2
            {
                public static string Desc { get { return "Parameter Value 2"; } }
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
            public static class FPTID
            {
                public static string Desc { get { return "FPTID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class FPTDesc
            {
                public static string Desc { get { return "FPT Descriptions"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class FPTStatusID
            {
                public static string Desc { get { return "FPT Status ID"; } }
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
            public static class StatusDesc
            {
                public static string Desc { get { return "Task Status"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class FPTStatusDesc
            {
                public static string Desc { get { return "FPT Status"; } }
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
            public static class BudgetPlanDescription
            {
                public static string Desc { get { return "BudgetPlan Description"; } }
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
            public static class VendorID
            {
                public static string Desc { get { return "Vendor ID"; } }
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
            public static class NegoBUnitDesc
            {
                public static string Desc { get { return "Business Unit"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class VendorName
            {
                public static string Desc { get { return "Vendor Name"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class TCTypeDesc
            {
                public static string Desc { get { return "TCTypeDesc"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class TCTypeID
            {
                public static string Desc { get { return "TCTypeID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class TCLeadName
            {
                public static string Desc { get { return "TCLeadName"; } }
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

            public static class BusinessUnitIDTC
            {
                public static string Desc { get { return "BusinessUnitIDTC"; } }
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

            public static class UserID
            {
                public static string Desc { get { return "User ID"; } }
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
                CNegotiationConfigurations m_objCNegotiationConfigurations = new CNegotiationConfigurations();
                SNegotiationConfigTypes m_objSNegotiationConfigTypes = new SNegotiationConfigTypes();
               
                DFPTStatus m_objDFPTStatus = new DFPTStatus();
                MStatus m_objMStatus = new MStatus();
                MFPT m_objMFPT = new MFPT();
                MTasks m_objMTasks = new MTasks();
                TBudgetPlan m_objTBudgetPlan = new TBudgetPlan();
                MProject m_objMProject = new MProject();
                MVendor m_objMVendor = new MVendor();
                MEmployee m_objEmployee = new MEmployee();
                TTCMembers m_objTcMember = new TTCMembers();
                MTCTypes m_objTCType = new MTCTypes();
                MUser m_objMUser = new MUser();
                MBusinessUnit m_objBusinessUnit = new MBusinessUnit();

                if (fieldName == NegotiationConfigID.Name)
                    m_strReturn = m_objCNegotiationConfigurations.Name + "." + fieldName;
                else if (fieldName == NegotiationConfigTypeID.Name)
                    m_strReturn = m_objCNegotiationConfigurations.Name + "." + fieldName;
                else if (fieldName == TaskID.Name)
                    m_strReturn = m_objCNegotiationConfigurations.Name + "." + fieldName;
               else if (fieldName == ParameterValue.Name)
                    m_strReturn = m_objCNegotiationConfigurations.Name + "." + fieldName;
                else if (fieldName == ParameterValue2.Name)
                    m_strReturn = m_objCNegotiationConfigurations.Name + "." + fieldName;
                else if (fieldName == Descriptions.Name)
                    m_strReturn = m_objSNegotiationConfigTypes.Name + "." + fieldName;
                else if (fieldName == FPTID.Name)
                    m_strReturn = m_objCNegotiationConfigurations.Name + "." + fieldName;
                else if (fieldName == FPTDesc.Name)
                    m_strReturn = m_objMFPT.Name + "." + FPTVM.Prop.Descriptions.Name;
                else if (fieldName == FPTStatusID.Name)
                    m_strReturn = m_objDFPTStatus.Name + "." + FPTStatusVM.Prop.StatusID.Name;
                else if (fieldName == FPTStatusDesc.Name)
                    m_strReturn = m_objDFPTStatus.Name + "." + FPTStatusVM.Prop.StatusDesc.Name;
                else if (fieldName == StatusID.Name)
                    m_strReturn = m_objMStatus.Name + "." + fieldName;
                else if (fieldName == StatusDesc.Name)
                    m_strReturn = m_objMStatus.Name + "." + fieldName;
                else if (fieldName == TaskID.Name)
                    m_strReturn = m_objMTasks.Name + "." + fieldName;
                else if (fieldName == CurrentApprovalLvl.Name)
                    m_strReturn = m_objMTasks.Name + "." + fieldName;
                else if (fieldName == ProjectID.Name)
                    m_strReturn = m_objTBudgetPlan.Name + "." + fieldName;
                else if (fieldName == ProjectDesc.Name)
                    m_strReturn = m_objMProject.Name + "." + fieldName;
                else if (fieldName == BudgetPlanID.Name)
                    m_strReturn = m_objTBudgetPlan.Name + "." + fieldName;
                else if (fieldName == BudgetPlanDescription.Name)
                    m_strReturn = "BPVersion.[Description]";
                else if (fieldName == FirstName.Name)
                    m_strReturn = m_objMVendor.Name + "." + fieldName;
                else if (fieldName == VendorID.Name)
                    m_strReturn = m_objMVendor.Name + "." + fieldName;
                else if (fieldName == VendorName.Name)
                    m_strReturn = "MVendor.FirstName + ' ' + MVendor.LastName";//TCTypeDesc
                else if (fieldName == TCMemberID.Name)
                    m_strReturn = m_objTcMember.Name + "." + fieldName;
                else if (fieldName == TCTypeID.Name)
                    m_strReturn = m_objTCType.Name + "." + fieldName;
                else if (fieldName == BusinessUnitDesc.Name)
                    m_strReturn = m_objBusinessUnit.Name + "." + "Descriptions";
                else if (fieldName == TCLeadName.Name)
                    m_strReturn = "TCLeadName.FullName";
                else if (fieldName == TCTypeDesc.Name)
                    m_strReturn = m_objTCType.Name + "." + "Descriptions";
                else if (fieldName == EmployeeID.Name)
                    m_strReturn = m_objEmployee.Name + "." + fieldName;
                else if (fieldName == UserID.Name)
                    m_strReturn = m_objMUser.Name + "." + fieldName;
                else if (fieldName == NegoBUnitDesc.Name)
                    m_strReturn = "BUnitDescriptions.[Descriptions]";
                else if (fieldName == EmployeeName.Name)
                    m_strReturn = m_objEmployee.Name + ".LastName";
                else if (fieldName == BusinessUnitIDTC.Name)
                    m_strReturn = m_objTcMember.Name + "." + "BusinessUnitID";
                ///BUnitDescriptions
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