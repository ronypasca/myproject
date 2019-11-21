using com.SML.Lib.Common;
using com.SML.BIGTRONS.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace com.SML.BIGTRONS.ViewModels
{
    public class NotificationMapVM
    {
        #region Public Property
        
        [Key, Column(Order = 1)]
        public string NotifMapID { get; set; }
        public bool IsDefault { get; set; }
        public string FunctionID { get; set; }
        public string NotificationTemplateID { get; set; }

        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class FunctionID
            {
                public static string Desc { get { return "Function ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class NotificationTemplateID
            {
                public static string Desc { get { return "Notification Template ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class NotifMapID
            {
                public static string Desc { get { return "NotifMapID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class IsDefault
            {
                public static string Desc { get { return "Default"; } }
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
                MFunctions m_objMFunctions = new MFunctions();
                DNotificationMap m_objDNotificationMap = new DNotificationMap();


                if (fieldName == FunctionID.Name)
                    m_strReturn = m_objDNotificationMap.Name + "." + fieldName;
                else if (fieldName == NotificationTemplateID.Name)
                    m_strReturn = m_objDNotificationMap.Name + "." + fieldName;
                else if (fieldName == IsDefault.Name)
                    m_strReturn = m_objDNotificationMap.Name + "." + fieldName;
                else if (fieldName == NotifMapID.Name)
                    m_strReturn = m_objDNotificationMap.Name + "." + fieldName;

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