using com.SML.Lib.Common;
using com.SML.BIGTRONS.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Collections.Generic;

namespace com.SML.BIGTRONS.ViewModels
{
    public class NotificationTemplateVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string NotificationTemplateID { get; set; }
        public string NotificationTemplateDesc { get; set; }
        [AllowHtml]
        public string Contents { get; set; }

        public List<FunctionsVM> ListFunctionsVM { get; set; }
        public List<FieldTagReferenceVM> ListFieldTagReferenceVM { get; set; }
        public List<string> Tags { get; set; }
        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class NotificationTemplateID
            {
                public static string Desc { get { return "ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class NotificationTemplateDesc
            {
                public static string Desc { get { return "Description"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class Contents
            {
                public static string Desc { get { return "Contents"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class ListFunctionsVM
            {
                public static string Desc { get { return "ListFunctionsVM"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class ListFieldTagReferenceVM
            {
                public static string Desc { get { return "Tags"; } }
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
                MNotificationTemplates m_objMNotificationTemplates = new MNotificationTemplates();

                if (fieldName == NotificationTemplateID.Name)
                    m_strReturn = m_objMNotificationTemplates.Name + "." + fieldName;
                else if (fieldName == NotificationTemplateDesc.Name)
                    m_strReturn = m_objMNotificationTemplates.Name + "." + fieldName;
                else if (fieldName == Contents.Name)
                    m_strReturn = m_objMNotificationTemplates.Name + "." + fieldName;

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
            PropertyInfo m_pifCountryVM = this.GetType().GetProperty(fieldName);
            return m_pifCountryVM != null && Attribute.GetCustomAttribute(m_pifCountryVM, typeof(KeyAttribute)) != null;
        }
    }
}