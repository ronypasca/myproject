using com.SML.Lib.Common;
using com.SML.BIGTRONS.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace com.SML.BIGTRONS.ViewModels
{
    public class EmployeeVM
    {

        #region Public Property

        [Key, Column(Order = 1)]
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Salutation { get; set; }
        public string Gender { get; set; }
        public string LanguageID { get; set; }
        public DateTime BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public string BirthCountry { get; set; }
        public string Nationality { get; set; }
        public string MaritalStatus { get; set; }
        public string Religion { get; set; }
        public string BloodType { get; set; }
        public string Email { get; set; }
        public DateTime JoinDate { get; set; }
        public DateTime PermanentDate { get; set; }
        public DateTime ResignDate { get; set; }
        public string RemunerationType { get; set; }

        public string FPTID { get; set; }
        public string AttendeeType { get; set; }

        public string CompanyDesc { get; set; }
        public string PersonnelAreaDesc { get; set; }
        public string PersonnelSubareaDesc { get; set; }
        public string OrgBusinessUnitDesc { get; set; }
        public string OrgDivisionDesc { get; set; }
        public string OrgDepartmentDesc { get; set; }
        public string OrgSectionDesc { get; set; }
        public string OwnerID { get; set; }
        public string PositionDesc { get; set; }

        public string PersonnelAreaID { get; set; }
        public string PersonnelSubareaID { get; set; }
        public string EmployeeGroupID { get; set; }
        public string EmployeeSubgroupID { get; set; }
        public string WorkContractID { get; set; }
        public string PositionID { get; set; }
        public string Grade { get; set; }
        public int Level { get; set; }
        public string SupervisorID { get; set; }
        public bool IsAttend { get; set; }

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
            public static class EmployeeName
            {
                public static string Desc { get { return "Employee Name"; } }
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
            public static class Salutation
            {
                public static string Desc { get { return "Salutation"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Gender
            {
                public static string Desc { get { return "Gender"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class LanguageID
            {
                public static string Desc { get { return "Language ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BirthDate
            {
                public static string Desc { get { return "Birth Date"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BirthPlace
            {
                public static string Desc { get { return "Birth Place"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BirthCountry
            {
                public static string Desc { get { return "Birth Country"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Nationality
            {
                public static string Desc { get { return "Nationality"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class MaritalStatus
            {
                public static string Desc { get { return "Marital Status"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Religion
            {
                public static string Desc { get { return "Religion"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BloodType
            {
                public static string Desc { get { return "Blood Type"; } }
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
            public static class JoinDate
            {
                public static string Desc { get { return "Join Date"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class PermanentDate
            {
                public static string Desc { get { return "Permanent Date"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ResignDate
            {
                public static string Desc { get { return "Resign Date"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class OwnerID
            {
                public static string Desc { get { return "Owner ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class RemunerationType
            {
                public static string Desc { get { return "Remuneration Type"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class CompanyDesc
            {
                public static string Desc { get { return "Company"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class PersonnelAreaDesc
            {
                public static string Desc { get { return "Personnel Area"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class PersonnelSubareaDesc
            {
                public static string Desc { get { return "Personnel Subarea"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class OrgBusinessUnitDesc
            {
                public static string Desc { get { return "Business Unit"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class OrgDivisionDesc
            {
                public static string Desc { get { return "Division"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class OrgDepartmentDesc
            {
                public static string Desc { get { return "Department"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class OrgSectionDesc
            {
                public static string Desc { get { return "Section"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class PositionDesc
            {
                public static string Desc { get { return "Position"; } }
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
            public static class AttendeeType
            {
                public static string Desc { get { return "Type"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class IsAttend
            {
                public static string Desc { get { return "IsAttend"; } }
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
                MEmployee m_objMEmployee = new MEmployee();
                MCompany m_objMCompany = new MCompany();
                MPosition m_objMPosition = new MPosition();
                MPersonnelArea m_objMPersonnelArea = new MPersonnelArea();
                MPersonnelSubarea m_objMPersonnelSubarea = new MPersonnelSubarea();
                TTCMembers m_objTTCMembers = new TTCMembers();
                DEmpCommunication m_objDEmpCommunication = new DEmpCommunication();

                if (fieldName == EmployeeID.Name)
                    m_strReturn = m_objMEmployee.Name + "." + fieldName;
                else if (fieldName == FirstName.Name)
                    m_strReturn = m_objMEmployee.Name + "." + fieldName;
                else if (fieldName == EmployeeName.Name)
                    m_strReturn = $"REPLACE({m_objMEmployee.Name}.{FirstName.Name} + {m_objMEmployee.Name}.{MiddleName.Name} + {m_objMEmployee.Name}.{LastName.Name},'..','')";
                else if (fieldName == LastName.Name)
                    m_strReturn = m_objMEmployee.Name + "." + LastName.Name;
                else if (fieldName == Salutation.Name)
                    m_strReturn = m_objMEmployee.Name + "." + fieldName;
                else if (fieldName == Gender.Name)
                    m_strReturn = m_objMEmployee.Name + "." + fieldName;
                else if (fieldName == LanguageID.Name)
                    m_strReturn = m_objMEmployee.Name + "." + fieldName;
                else if (fieldName == BirthDate.Name)
                    m_strReturn = m_objMEmployee.Name + "." + fieldName;
                else if (fieldName == BirthPlace.Name)
                    m_strReturn = m_objMEmployee.Name + "." + fieldName;
                else if (fieldName == Email.Name)
                    m_strReturn = m_objMEmployee.Name + "." + fieldName;
                else if (fieldName == BirthCountry.Name)
                    m_strReturn = m_objMEmployee.Name + "." + fieldName;
                else if (fieldName == Nationality.Name)
                    m_strReturn = m_objMEmployee.Name + "." + fieldName;
                else if (fieldName == MaritalStatus.Name)
                    m_strReturn = m_objMEmployee.Name + "." + fieldName;
                else if (fieldName == Religion.Name)
                    m_strReturn = m_objMEmployee.Name + "." + fieldName;
                else if (fieldName == BloodType.Name)
                    m_strReturn = m_objMEmployee.Name + "." + fieldName;
                else if (fieldName == Email.Name)
                    m_strReturn = m_objMEmployee.Name + "." + fieldName;
                else if (fieldName == JoinDate.Name)
                    m_strReturn = m_objMEmployee.Name + "." + fieldName;
                else if (fieldName == PermanentDate.Name)
                    m_strReturn = m_objMEmployee.Name + "." + fieldName;
                else if (fieldName == ResignDate.Name)
                    m_strReturn = m_objMEmployee.Name + "." + fieldName;
                else if (fieldName == RemunerationType.Name)
                    m_strReturn = m_objMEmployee.Name + "." + fieldName;

                else if(fieldName == CompanyDesc.Name)
                    m_strReturn = m_objMCompany.Name + "." + fieldName;
                else if (fieldName == OrgDivisionDesc.Name)
                    m_strReturn = m_objMPosition.Name + "." + fieldName;
                else if (fieldName == OrgDepartmentDesc.Name)
                    m_strReturn = m_objMPosition.Name + "." + fieldName;
                else if (fieldName == OrgSectionDesc.Name)
                    m_strReturn = m_objMPosition.Name + "." + fieldName;
                else if (fieldName == OrgBusinessUnitDesc.Name)
                    m_strReturn = m_objMPosition.Name + "." + fieldName;
                else if (fieldName == PersonnelAreaDesc.Name)
                    m_strReturn = m_objMPersonnelArea.Name + "." + fieldName;
                else if (fieldName == PersonnelSubareaDesc.Name)
                    m_strReturn = m_objMPersonnelSubarea.Name + "." + fieldName;
                else if (fieldName == PositionDesc.Name)
                    m_strReturn = m_objMPosition.Name + "." + fieldName;
                else if (fieldName == OwnerID.Name)
                    m_strReturn = m_objMEmployee.Name + "." + fieldName;
                else if (fieldName == OwnerID.Name)
                    m_strReturn = m_objMEmployee.Name + "." + fieldName;
                else if (fieldName == IsAttend.Name)
                    m_strReturn = m_objMEmployee.Name + "." + fieldName;

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