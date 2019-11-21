using com.SML.Lib.Common;
using com.SML.BIGTRONS.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using SW = System.Web;

namespace com.SML.BIGTRONS.ViewModels
{
    public class TaskDetailsVM
    {

        #region Public Property

        [Key, Column(Order = 1)]
        public string TaskDetailID { get; set; }
        public string TaskID { get; set; }
        public int StatusID { get; set; }
        public string StatusDesc { get; set; }
        public string Remarks { get; set; }
        public string CreatorFullName { get; set; }
        public DateTime CreatedDate { get; set; }

        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class TaskDetailID
            {
                public static string Desc { get { return "Task Detail ID"; } }
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
            public static class StatusID
            {
                public static string Desc { get { return "StatusID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class StatusDesc
            {
                public static string Desc { get { return "Status"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Remarks
            {
                public static string Desc { get { return "Remarks"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class CreatedDate
            {
                public static string Desc { get { return "Date"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class CreatorFullName
            {
                public static string Desc { get { return "Created By"; } }
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
                DTaskDetails m_objTaskDetails = new DTaskDetails();               
                MUser m_objMuser = new MUser();
                MStatus m_objStatus = new MStatus();

                if (fieldName == TaskID.Name)
                    m_strReturn = m_objTaskDetails.Name + "." + fieldName;
                else if (fieldName == TaskDetailID.Name)
                    m_strReturn = m_objTaskDetails.Name + "." + fieldName;
                else if (fieldName == StatusID.Name)
                    m_strReturn = m_objTaskDetails.Name + "." + fieldName;
                else if (fieldName == StatusDesc.Name)
                    m_strReturn = m_objStatus.Name + "." + fieldName;
                else if (fieldName == Remarks.Name)
                    m_strReturn = m_objTaskDetails.Name + "." + fieldName;
                else if (fieldName == CreatorFullName.Name)
                    m_strReturn = m_objMuser.Name + "." + "FullName";
                else if (fieldName == CreatedDate.Name)
                    m_strReturn = m_objTaskDetails.Name + "." + fieldName;

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
            PropertyInfo m_pifTaskVM = this.GetType().GetProperty(fieldName);
            return m_pifTaskVM != null && Attribute.GetCustomAttribute(m_pifTaskVM, typeof(KeyAttribute)) != null;
        }
    }
}