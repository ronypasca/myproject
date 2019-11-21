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
    public class FPTStatusVM 
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string FPTStatusID { get; set; }
        [Key, Column(Order = 2)]
        public string FPTID { get; set; }
        public DateTime StatusDateTimeStamp { get; set; }
        public int StatusID { get; set; }
        public string Remarks { get; set; }
        public string StatusDesc { get; set; }
        public string ModifiedBy { get; set; }
        public string TaskStatusID { get; set; }
        public DateTime CreatedDate { get; set; }
        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class FPTStatusID
            {
                public static string Desc { get { return "ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class FPTID
            {
                public static string Desc { get { return "FPT Ref.ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class StatusDateTimeStamp
            {
                public static string Desc { get { return "Status Date Time Stamp"; } }
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
            public static class Remarks
            {
                public static string Desc { get { return "Remarks"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class TaskStatusID
            {
                public static string Desc { get { return "Task Status ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class StatusDesc
            {
                public static string Desc { get { return "Descriptions"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ModifiedBy
            {
                public static string Desc { get { return "Updated By"; } }
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
                DFPTStatus m_objDFPTStatus = new DFPTStatus();
                MStatus m_objMStatus = new MStatus();
                MTasks m_objMTasks = new MTasks();

                if (fieldName == FPTStatusID.Name)
                    m_strReturn = m_objDFPTStatus.Name + "." + fieldName;
                else if (fieldName == FPTID.Name)
                    m_strReturn = m_objDFPTStatus.Name + "." + fieldName;
                else if (fieldName == StatusDateTimeStamp.Name)
                    m_strReturn = m_objDFPTStatus.Name + "." + fieldName;
                else if (fieldName == StatusID.Name)
                    m_strReturn = m_objDFPTStatus.Name + "." + fieldName;
                else if (fieldName == Remarks.Name)
                    m_strReturn = m_objDFPTStatus.Name + "." + fieldName;
                else if (fieldName == TaskStatusID.Name)
                    m_strReturn = m_objMTasks.Name + "." + "StatusID";
                else if (fieldName == StatusDesc.Name)
                    m_strReturn = m_objMStatus.Name + "." + fieldName;
                else if (fieldName == CreatedDate.Name)
                    m_strReturn = m_objDFPTStatus.Name + "." + fieldName;
                else if (fieldName == ModifiedBy.Name)
                    m_strReturn = m_objDFPTStatus.Name + "." + fieldName;

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