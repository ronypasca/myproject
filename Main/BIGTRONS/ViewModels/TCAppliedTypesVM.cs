using com.SML.Lib.Common;
using com.SML.BIGTRONS.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace com.SML.BIGTRONS.ViewModels
{
    public class TCAppliedTypesVM
    {

        #region Public Property

        [Key, Column(Order = 1)]
        public string TCAppliedID { get; set; }
        public string TCMemberID { get; set; }
        public string TCTypeID { get; set; }
        public string TCTypeDesc { get; set; }
        public bool Checked { get; set; }
        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class TCAppliedID
            {
                public static string Desc { get { return "TCAppliedID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class TCMemberID
            {
                public static string Desc { get { return "TCMemberID"; } }
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

            public static class TCTypeDesc
            {
                public static string Desc { get { return "Type"; } }
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
                TTCAppliedTypes m_objTTCAppliedTypes = new TTCAppliedTypes();
                MEmployee m_objMEmployee = new MEmployee();
                MTCTypes m_objMTCTypes = new MTCTypes();

                if (fieldName == TCMemberID.Name)
                    m_strReturn = m_objTTCAppliedTypes.Name + "." + fieldName;
                else if (fieldName == TCAppliedID.Name)
                    m_strReturn = m_objTTCAppliedTypes.Name + "." + fieldName;
                else if (fieldName == TCTypeID.Name)
                    m_strReturn = m_objTTCAppliedTypes.Name + "." + fieldName;
                else if (fieldName == TCTypeDesc.Name)
                    m_strReturn = m_objMTCTypes.Name + ".Description";

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