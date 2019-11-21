using com.SML.Lib.Common;
using com.SML.BIGTRONS.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace com.SML.BIGTRONS.ViewModels
{
    public class FPTAttendancesVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string FPTAttendanceID { get; set; }
        public string FPTID { get; set; }
        public string AttendeeType { get; set; }
        public string IDAttendee { get; set; }
        public bool IsAttend { get; set; }
        public string AttendanceDesc { get; set; }

        public string AttendanceName { get; set; }
        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class FPTAttendanceID
            {
                public static string Desc { get { return "ID"; } }
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
            public static class AttendeeType
            {
                public static string Desc { get { return "Attendee Type"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class IDAttendee
            {
                public static string Desc { get { return "ID Attendee"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class IsAttend
            {
                public static string Desc { get { return "Is Attend"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class AttendanceDesc
            {
                public static string Desc { get { return "Attendance Desc"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class AttendanceName
            {
                public static string Desc { get { return "Attendance Name"; } }
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
                TFPTAttendances m_objTFPTAttendances = new TFPTAttendances();
                MEmployee m_objMEmployee = new MEmployee();
                MVendor m_objMVendor = new MVendor();

                if (fieldName == FPTAttendanceID.Name)
                    m_strReturn = m_objTFPTAttendances.Name + "." + fieldName;
                else if (fieldName == FPTID.Name)
                    m_strReturn = m_objTFPTAttendances.Name + "." + fieldName;
                else if (fieldName == AttendeeType.Name)
                    m_strReturn = m_objTFPTAttendances.Name + "." + fieldName;
                else if (fieldName == IDAttendee.Name)
                    m_strReturn = m_objTFPTAttendances.Name + "." + fieldName;
                else if (fieldName == IsAttend.Name)
                    m_strReturn = m_objTFPTAttendances.Name + "." + fieldName;
                else if (fieldName == AttendanceDesc.Name)
                    m_strReturn = m_objTFPTAttendances.Name + "." + fieldName;
                else if (fieldName == AttendanceName.Name)
                    m_strReturn = $"(ISNULL({m_objMEmployee.Name}.FirstName,'') + ISNULL({m_objMVendor.Name}.FirstName,'') + ISNULL({m_objMEmployee.Name}.LastName,'') + ISNULL({m_objMVendor.Name}.LastName,''))";
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
            PropertyInfo m_pifFPTAttendancesVM = this.GetType().GetProperty(fieldName);
            return m_pifFPTAttendancesVM != null && Attribute.GetCustomAttribute(m_pifFPTAttendancesVM, typeof(KeyAttribute)) != null;
        }
    }
}