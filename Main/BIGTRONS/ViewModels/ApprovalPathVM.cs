using com.SML.Lib.Common;
using com.SML.BIGTRONS.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace com.SML.BIGTRONS.ViewModels
{
    public class ApprovalPathVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string ApprovalPathID { get; set; }
        public string RoleID { get; set; }
        public string RoleDesc { get; set; }
        public string RoleParentID { get; set; }
        public string RoleParentDesc { get; set; }
        public string RoleChildID { get; set; }
        public string RoleChildDesc { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TaskTypeID { get; set; }
        public string TaskTypeDesc { get; set; }

        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class ApprovalPathID
            {
                public static string Desc { get { return "ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class RoleID
            {
                public static string Desc { get { return "Role ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class RoleDesc
            {
                public static string Desc { get { return "Applied to User Role"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class RoleParentID
            {
                public static string Desc { get { return "Role Parent ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class RoleParentDesc
            {
                public static string Desc { get { return "Parent Role"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class RoleChildID
            {
                public static string Desc { get { return "Role Child ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class RoleChildDesc
            {
                public static string Desc { get { return "Child Role"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class StartDate
            {
                public static string Desc { get { return "Start Date"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class EndDate
            {
                public static string Desc { get { return "End Date"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class TaskTypeID
            {
                public static string Desc { get { return "Task Type ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class TaskTypeDesc
            {
                public static string Desc { get { return "Descriptions"; } }
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
                CApprovalPath m_objCApprovalPath = new CApprovalPath();
                MRole m_objRMRole = new MRole();
                MTaskTypes m_objMTaskTypes = new MTaskTypes();

                if (fieldName == ApprovalPathID.Name)
                    m_strReturn = m_objCApprovalPath.Name + "." + fieldName;
                else if (fieldName == RoleID.Name)
                    m_strReturn = m_objCApprovalPath.Name + "." + fieldName;
                else if (fieldName == RoleDesc.Name)
                    m_strReturn = m_objRMRole.Name + "." + fieldName;
                else if (fieldName == RoleParentID.Name)
                    m_strReturn = m_objCApprovalPath.Name + "." + fieldName;
                else if (fieldName == RoleParentDesc.Name)
                    m_strReturn = "MRoleParent" + "." + RoleDesc.Name;
                else if (fieldName == RoleChildID.Name)
                    m_strReturn = m_objCApprovalPath.Name + "." + fieldName;
                else if (fieldName == RoleChildDesc.Name)
                    m_strReturn = "MRoleChild" + "." + RoleDesc.Name;
                else if (fieldName == StartDate.Name)
                    m_strReturn = m_objCApprovalPath.Name + "." + fieldName;
                else if (fieldName == EndDate.Name)
                    m_strReturn = m_objCApprovalPath.Name + "." + fieldName;
                else if (fieldName == TaskTypeID.Name)
                    m_strReturn = m_objCApprovalPath.Name + "." + fieldName;
                else if (fieldName == TaskTypeDesc.Name)
                    m_strReturn = m_objMTaskTypes.Name + "." + "Descriptions";//todo:
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
            PropertyInfo m_pifDimensionVM = this.GetType().GetProperty(fieldName);
            return m_pifDimensionVM != null && Attribute.GetCustomAttribute(m_pifDimensionVM, typeof(KeyAttribute)) != null;
        }
    }
}