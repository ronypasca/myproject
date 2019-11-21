using com.SML.Lib.Common;
using com.SML.BIGTRONS.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;

namespace com.SML.BIGTRONS.ViewModels
{
    public class ApprovalDelegationVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string ApprovalDelegateID { get; set; }
        public string UserID { get; set; }
        public string OwnerName { get; set; } 
        public string TaskTypeID { get; set; }
        public string TaskTypeDesc { get; set; }
        public string DelegateUserID { get; set; }
        public string DelegateTo { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public string TaskGroupID { get; set; }
        public string TaskGroupDesc { get; set; }
        public List<string> ListApprovalDelegateID { get; set; }
        public List<ApprovalDelegationUserVM> lstApprovalDelegationUser { get; set; }
        public string ListApproval_asString { get {
                List<string> ListName = new List<string>();
                if (lstApprovalDelegationUser != null) 
                    foreach (ApprovalDelegationUserVM Name in lstApprovalDelegationUser)
                        ListName.Add(Name.DelegateTo);
                return string.Join(Global.NewLineSeparated,ListName);
            } }
        public List<ApprovalDelegationVM> lstTaskType{ get; set; }
        public string lstTaskType_asString
        {
            get
            {
                List<string> ListTaskType = new List<string>();
                if (lstTaskType != null)
                    foreach (ApprovalDelegationVM objTaskType in lstTaskType) 
                        ListTaskType.Add(objTaskType.TaskTypeDesc);
                return string.Join(Global.NewLineSeparated, ListTaskType);
            }
        }

        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class ApprovalDelegateID
            {
                public static string Desc { get { return "ID"; } }
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

            public static class OwnerName
            {
                public static string Desc { get { return "Owner"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class TaskTypeID
            {
                public static string Desc { get { return "TaskType ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class TaskTypeDesc
            {
                public static string Desc { get { return "Task Type"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class TaskGroupDesc
            {
                public static string Desc { get { return "Task Group"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class TaskGroupID
            {
                public static string Desc { get { return "Task Group ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class DelegateUserID
            {
                public static string Desc { get { return "DelegateUserId "; } }
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
                MApprovalDelegation m_objApprovalDelegation = new MApprovalDelegation();
                MTaskTypes m_objMTaskType = new MTaskTypes();
                DApprovalDelegationUser m_objDelegationUser = new DApprovalDelegationUser();
                
                if (fieldName == ApprovalDelegateID.Name)
                    m_strReturn = m_objApprovalDelegation.Name + "." + fieldName;
                else if (fieldName == UserID.Name)
                    m_strReturn = m_objApprovalDelegation.Name + "." + fieldName;
                else if (fieldName == TaskTypeID.Name)
                    m_strReturn = m_objApprovalDelegation.Name + "." + fieldName;
                else if (fieldName == TaskTypeDesc.Name)
                    m_strReturn = TaskTypesVM.Prop.Descriptions.Map;
                else if (fieldName == TaskGroupDesc.Name)
                    m_strReturn = m_objMTaskType.Name + "." + fieldName;
                else if (fieldName == TaskGroupID.Name)
                    m_strReturn = m_objMTaskType.Name + "." + fieldName;
                else if (fieldName == OwnerName.Name)
                    m_strReturn = m_objApprovalDelegation.Name + "." + fieldName;
                else if (fieldName == DelegateUserID.Name)
                    m_strReturn = m_objDelegationUser.Name + "." + fieldName;
                else if (fieldName == DelegateTo.Name)
                    m_strReturn = m_objDelegationUser.Name + "." + fieldName;
                else if (fieldName == PeriodStart.Name)
                    m_strReturn = m_objApprovalDelegation.Name + "." + fieldName;
                else if (fieldName == PeriodEnd.Name)
                    m_strReturn = m_objApprovalDelegation.Name + "." + fieldName;

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
            PropertyInfo m_pifApprovalDelegationVM = this.GetType().GetProperty(fieldName);
            return m_pifApprovalDelegationVM != null && Attribute.GetCustomAttribute(m_pifApprovalDelegationVM, typeof(KeyAttribute)) != null;
        }
    }
}