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
    public class TCFunctionsVM
    {

        #region Public Property

        [Key, Column(Order = 1)]
        public string TCFunctionID { get; set; }
        public string TCMemberID { get; set; }
        public string FunctionID { get; set; }
        public string UserID { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string FunctionDesc { get; set; }
        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class TCFunctionID
            {
                public static string Desc { get { return "TCFunctionID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class TCMemberID
            {
                public static string Desc { get { return "TC Member ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class FunctionID
            {
                public static string Desc { get { return "Function ID"; } }
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
            public static class FirstName
            {
                public static string Desc { get { return "First Name"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Email
            {
                public static string Desc { get { return "Mail Address"; } }
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
                DTCFunctions m_objDTCFunctions = new DTCFunctions();
                MEmployee m_objEmployee = new MEmployee();
                MUser m_objMuser = new MUser();

                if (fieldName == TCMemberID.Name)
                    m_strReturn = m_objDTCFunctions.Name + "." + fieldName;
                else if (fieldName == FunctionID.Name)
                    m_strReturn = m_objDTCFunctions.Name + "." + fieldName;
                else if (fieldName == TCFunctionID.Name)
                    m_strReturn = m_objDTCFunctions.Name + "." + fieldName;
                else if (fieldName == UserID.Name)
                    m_strReturn = m_objMuser.Name + "." + fieldName;
                else if (fieldName == FirstName.Name)
                    m_strReturn = m_objEmployee.Name + "." + fieldName;
                else if (fieldName == Email.Name)
                    m_strReturn = m_objEmployee.Name + "." + fieldName;

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