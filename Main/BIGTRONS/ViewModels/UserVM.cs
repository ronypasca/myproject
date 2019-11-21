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
    public class UserVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string UserID { get; set; }
        public List<UserRoleVM> UserRole { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string ReTypeNewPassword { get; set; }
        public string VendorID { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string VendorDesc { get; set; }
        public DateTime LastLogin { get; set; }
        public string HostIP { get; set; }
        public bool IsActive { get; set; }
        public bool IsRemember { get; set; }
        public TCMembersVM TCMember { get; set; }
        public string BusinessUnitID { get; set; }
        public string BusinessUnitDesc { get; set; }
        public string DivisionID { get; set; }
        public string DivisionDesc { get; set; }
        public string ProjectID { get; set; }
        public string ProjectDesc { get; set; }
        public string ClusterID { get; set; }
        public string ClusterDesc { get; set; }
        public List<UserRoleVM> UserRoles { get; set; }
        public string UserRolesDesc
        {
            get
            {
                return UserRoles != null? UserRoles.Count > 0 ? string.Join(", ", UserRoles.Select(d => d.RoleID).ToArray()) : string.Empty : "";
            }
        }
        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class UserID
            {
                public static string Desc { get { return "ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            //public static class RoleID
            //{
            //    public static string Desc { get { return "Role ID"; } }
            //    public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
            //    public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
            //    public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            //}
            //public static class RoleDesc
            //{
            //    public static string Desc { get { return "Role"; } }
            //    public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
            //    public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
            //    public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            //}
            public static class FullName
            {
                public static string Desc { get { return "Full Name"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Password
            {
                public static string Desc { get { return "Password"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class NewPassword
            {
                public static string Desc { get { return "New Password"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class ReTypeNewPassword
            {
                public static string Desc { get { return "Re-Type New Password"; } }
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
            public static class EmployeeID
            {
                public static string Desc { get { return "Employee"; } }
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

            public static class VendorDesc
            {
                public static string Desc { get { return "Vendor"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class LastLogin
            {
                public static string Desc { get { return "Last Login"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class HostIP
            {
                public static string Desc { get { return "Host IP"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class IsActive
            {
                public static string Desc { get { return "Active"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class IsRemember
            {
                public static string Desc { get { return "Remember"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class BusinessUnitID
            {
                public static string Desc { get { return "Business Unit ID"; } }
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

            public static class DivisionID
            {
                public static string Desc { get { return "Division ID"; } }
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

            public static class ProjectID
            {
                public static string Desc { get { return "Project ID"; } }
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

            public static class ClusterID
            {
                public static string Desc { get { return "Cluster ID"; } }
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

            public static class UserRole
            {
                public static string Desc { get { return "UserRole"; } }
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
                MUser m_objUser = new MUser();
                DUserRole m_objDUserRole = new DUserRole();
                MRole m_objMRole = new MRole();
                MVendor m_objMVendor = new MVendor();
                MEmployee m_objMEmployee = new MEmployee();
                MBusinessUnit m_objMBusinessUnit = new MBusinessUnit();
                MDivision m_objMDivision = new MDivision();
                MProject m_objMProject = new MProject();
                MCluster m_objMCluster = new MCluster();

                if (fieldName == UserID.Name)
                    m_strReturn = m_objUser.Name + "." + fieldName;
                //else if (fieldName == RoleID.Name)
                //    m_strReturn = m_objDUserRole.Name + "." + fieldName;
                //else if (fieldName == RoleDesc.Name)
                //    m_strReturn = m_objMRole.Name + "." + fieldName;
                else if (fieldName == FullName.Name)
                    m_strReturn = m_objUser.Name + "." + fieldName;
                else if (fieldName == Password.Name)
                    m_strReturn = m_objUser.Name + "." + fieldName;
                else if (fieldName == VendorID.Name)
                    m_strReturn = m_objUser.Name + "." + fieldName;
                else if (fieldName == EmployeeID.Name)
                    m_strReturn = m_objUser.Name + "." + fieldName;
                else if (fieldName == VendorDesc.Name)
                    m_strReturn = m_objMVendor.Name + "." + "FirstName";//todo:
                else if (fieldName == LastLogin.Name)
                    m_strReturn = m_objUser.Name + "." + fieldName;
                else if (fieldName == HostIP.Name)
                    m_strReturn = m_objUser.Name + "." + fieldName;
                else if (fieldName == IsActive.Name)
                    m_strReturn = m_objUser.Name + "." + fieldName;

                else if (fieldName == EmployeeName.Name)
                    m_strReturn = m_objMEmployee.Name + "." + EmployeeVM.Prop.LastName.Name;
                else if (fieldName == BusinessUnitID.Name)
                    m_strReturn = m_objMBusinessUnit.Name + "." + fieldName;
                else if (fieldName == BusinessUnitDesc.Name)
                    m_strReturn = m_objMBusinessUnit.Name + "." + BusinessUnitVM.Prop.Descriptions.Name;
                else if (fieldName == DivisionID.Name)
                    m_strReturn = m_objMDivision.Name + "." + fieldName;
                else if (fieldName == DivisionDesc.Name)
                    m_strReturn = m_objMDivision.Name + "." + fieldName;
                else if (fieldName == ProjectID.Name)
                    m_strReturn = m_objMProject.Name + "." + fieldName;
                else if (fieldName == ProjectDesc.Name)
                    m_strReturn = m_objMProject.Name + "." + fieldName;
                else if (fieldName == ClusterID.Name)
                    m_strReturn = m_objMCluster.Name + "." + fieldName;
                else if (fieldName == ClusterDesc.Name)
                    m_strReturn = m_objMCluster.Name + "." + fieldName;
                else if (fieldName == CreatedDate.Name)
                    m_strReturn = m_objDUserRole.Name + "." + fieldName;


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
            PropertyInfo m_pifUserVM = this.GetType().GetProperty(fieldName);
            return m_pifUserVM != null && Attribute.GetCustomAttribute(m_pifUserVM, typeof(KeyAttribute)) != null;
        }
    }
}