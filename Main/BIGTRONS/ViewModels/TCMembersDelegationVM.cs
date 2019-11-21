using com.SML.Lib.Common;
using com.SML.BIGTRONS.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace com.SML.BIGTRONS.ViewModels
{
    public class TCMembersDelegationVM
    {

        #region Public Property

        [Key, Column(Order = 1)]
        public string TCDelegationID { get; set; }
        public string TCMemberID { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string SuperiorID { get; set; }
        public string SuperiorName { get; set; }
        public string TCTypeID { get; set; }
        public string TCTypeDesc
        {
            get
            {
                return ListTCAppliedTypesVM != null ?
                    string.Join("<\br>", ListTCAppliedTypesVM.Select(d => d.TCTypeDesc)) : string.Empty;
            }
        }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public DateTime? DelegateStartDate { get; set; }
        public DateTime? DelegateEndDate { get; set; }
        public string DelegateTo { get; set; }
        public string DelegateName { get; set; }
        public string BusinessUnitID { get; set; }
        public string BusinessUnitDesc { get; set; }
        public List<TCAppliedTypesVM> ListTCAppliedTypesVM { get; set; }
        public string TCEmail { get; set; }
        public List<TCFunctionsVM> ListTCFunctionsVM { get; set; }
        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class EmployeeID
            {
                public static string Desc { get { return "NRK"; } }
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
            public static class FirstName
            {
                public static string Desc { get { return "First Name"; } }
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
            public static class EmployeeName
            {
                public static string Desc { get { return "Employee Name"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class TCMemberID
            {
                public static string Desc { get { return "ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class SuperiorID
            {
                public static string Desc { get { return "Superior ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class SuperiorName
            {
                public static string Desc { get { return "Direct Report To"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class TCTypeID
            {
                public static string Desc { get { return "TC Type ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class TCTypeDesc
            {
                public static string Desc { get { return "Type"; } }
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
            public static class PeriodStart
            {
                public static string Desc { get { return "Period Start"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class PeriodEnd
            {
                public static string Desc { get { return "Period End"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class DelegateStartDate
            {
                public static string Desc { get { return "Start Date"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class DelegateEndDate
            {
                public static string Desc { get { return "End Date"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class DelegateTo
            {
                public static string Desc { get { return "Delegate To"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class DelegateName
            {
                public static string Desc { get { return "Delegate Name"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class TCDelegationID
            {
                public static string Desc { get { return "TC Delegation ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ListTCAppliedTypesVM
            {
                public static string Desc { get { return "ListTCAppliedTypesVM"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class TCEmail
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
                TTCMembers m_objTTCMembers = new TTCMembers();
                TTCMemberDelegations m_objTTCMemberDelegations = new TTCMemberDelegations();
                MEmployee m_objMEmployee = new MEmployee();
                MTCTypes m_objMTCTypes = new MTCTypes();

                if (fieldName == TCMemberID.Name)
                    m_strReturn = m_objTTCMemberDelegations.Name + "." + fieldName;
                else if (fieldName == EmployeeID.Name)
                    m_strReturn = m_objMEmployee.Name + "." + fieldName;
                else if (fieldName == TCEmail.Name)
                    m_strReturn = m_objMEmployee.Name + "." + TCEmail.Desc;
                else if (fieldName == DelegateName.Name)
                    m_strReturn = $"REPLACE({m_objMEmployee.Name}.{FirstName.Name} + {m_objMEmployee.Name}.{MiddleName.Name} + {m_objMEmployee.Name}.{LastName.Name},'..','')";
                else if (fieldName == TCTypeID.Name)
                    m_strReturn = m_objMTCTypes.Name + "." + fieldName;
                else if (fieldName == TCTypeDesc.Name)
                    m_strReturn = m_objMTCTypes.Name + "." + Descriptions.Name;
                else if (fieldName == SuperiorID.Name)
                    m_strReturn = "[Superior]" + "." + EmployeeID.Name;
                else if (fieldName == SuperiorName.Name)
                    m_strReturn = $"REPLACE([Superior].{FirstName.Name} + [Superior].{MiddleName.Name} + [Superior].{LastName.Name},'..','')";
                else if (fieldName == PeriodStart.Name)
                    m_strReturn = m_objTTCMembers.Name + "." + fieldName;
                else if (fieldName == PeriodEnd.Name)
                    m_strReturn = m_objTTCMembers.Name + "." + fieldName;
                else if (fieldName == DelegateStartDate.Name)
                    m_strReturn = m_objTTCMemberDelegations.Name + "." + fieldName;
                else if (fieldName == DelegateEndDate.Name)
                    m_strReturn = m_objTTCMemberDelegations.Name + "." + fieldName;
                else if (fieldName == DelegateTo.Name)
                    m_strReturn = m_objTTCMemberDelegations.Name + "." + fieldName;
                else if (fieldName == TCDelegationID.Name)
                    m_strReturn = m_objTTCMemberDelegations.Name + "." + fieldName;

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
            PropertyInfo m_pifVendorVM = this.GetType().GetProperty(fieldName);
            return m_pifVendorVM != null && Attribute.GetCustomAttribute(m_pifVendorVM, typeof(KeyAttribute)) != null;
        }
    }
}