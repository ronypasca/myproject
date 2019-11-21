using com.SML.Lib.Common;
using com.SML.BIGTRONS.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace com.SML.BIGTRONS.ViewModels
{
    public class NotificationValuesVM
    {

        #region Public Property

        [Key, Column(Order = 1)]
        public string NotificationValueID { get; set; }
        public string Value { get; set; }
        public string MailNotificationID { get; set; }
        public string FieldTagID { get; set; }
        public string TagDesc { get; set; }
        public string RefTable { get; set; }
        public string RefIDColumn { get; set; }

        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class NotificationValueID
            {
                public static string Desc { get { return "Notification Value ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Value
            {
                public static string Desc { get { return "Value"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class MailNotificationID
            {
                public static string Desc { get { return "Mail Notification ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class FieldTagID
            {
                public static string Desc { get { return "Field Tag ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class TagDesc
            {
                public static string Desc { get { return "Tag Desc"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class RefTable
            {
                public static string Desc { get { return "RefTable"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class RefIDColumn
            {
                public static string Desc { get { return "RefIDColumn"; } }
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
                TNotificationValues m_objTNotificationValues = new TNotificationValues();
                MFieldTagReferences m_objFieldTags = new MFieldTagReferences();

                if (fieldName == NotificationValueID.Name)
                    m_strReturn = m_objTNotificationValues.Name + "." + fieldName;
                else if (fieldName == MailNotificationID.Name)
                    m_strReturn = m_objTNotificationValues.Name + "." + fieldName;
                else if (fieldName == FieldTagID.Name)
                    m_strReturn = m_objTNotificationValues.Name + "." + fieldName;
                else if (fieldName == TagDesc.Name)
                    m_strReturn = m_objFieldTags.Name + "." + fieldName;
                else if (fieldName == Value.Name)
                    m_strReturn = m_objTNotificationValues.Name + "." + fieldName;
                else if (fieldName == RefTable.Name)
                    m_strReturn = m_objFieldTags.Name + "." + fieldName;
                else if (fieldName == RefIDColumn.Name)
                    m_strReturn = m_objFieldTags.Name + "." + fieldName;



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