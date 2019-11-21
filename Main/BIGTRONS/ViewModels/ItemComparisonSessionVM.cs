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
    public class ItemComparisonSessionVM
    {
        #region public property
        [Key, Column(Order = 1)]

        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemSecondaryName { get; set; }
        public string SessionID { get; set; }
        public string GroupGUID { get; set; }
        public string ComparisonDetilID { get; set; }
        #endregion

        #region Public Field Property
        public static class Prop
        {
            public static class ItemID
            {
                public static string Desc { get { return "ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ItemName
            {
                public static string Desc { get { return "Item Name"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ItemSecondaryName
            {
                public static string Desc { get { return "Item Secondary Name"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class SessionID
            {
                public static string Desc { get { return "Session ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class GroupGUID
            {
                public static string Desc { get { return "Goup GUID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ComparisonDetilID
            {
                public static string Desc { get { return "Comparison Detil ID"; } }
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

                DItemComparisonSession m_objMItemComparisonDetailSession = new DItemComparisonSession();

                if (fieldName == ItemID.Name)
                    m_strReturn = m_objMItemComparisonDetailSession.Name + "." + fieldName;
                else if (fieldName == ItemName.Name)
                    m_strReturn = m_objMItemComparisonDetailSession.Name + "." + fieldName;
                else if (fieldName == ItemSecondaryName.Name)
                    m_strReturn = m_objMItemComparisonDetailSession.Name + "." + fieldName;
                else if (fieldName == SessionID.Name)
                    m_strReturn = m_objMItemComparisonDetailSession.Name + "." + fieldName;
                else if (fieldName == GroupGUID.Name)
                    m_strReturn = m_objMItemComparisonDetailSession.Name + "." + fieldName;
                else if (fieldName == ComparisonDetilID.Name)
                    m_strReturn = m_objMItemComparisonDetailSession.Name + "." + fieldName;

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
            PropertyInfo m_pifUoMVM = this.GetType().GetProperty(fieldName);
            return m_pifUoMVM != null && Attribute.GetCustomAttribute(m_pifUoMVM, typeof(KeyAttribute)) != null;
        }
    }
}