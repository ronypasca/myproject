using com.SML.Lib.Common;
using com.SML.BIGTRONS.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace com.SML.BIGTRONS.ViewModels
{
    public class NotificationAttachmentVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string AttachmentValueID { get; set; }
        public string No { get; set; }
        public string Filename { get; set; }
        public string ContentType { get; set; }
        public string RawData { get; set; }
        public string ByteRawData { get; set; }
        public string MailNotificationID { get; set; }
        public string Subject { get; set; }
        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class AttachmentValueID
            {
                public static string Desc { get { return "AttachmentValue ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class Filename
            {
                public static string Desc { get { return "Filename"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class ContentType
            {
                public static string Desc { get { return "Content Type"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class RawData
            {
                public static string Desc { get { return "Raw Data"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class MailNotificationID
            {
                public static string Desc { get { return "MailNotification ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class Subject
            {
                public static string Desc { get { return "Subject"; } }
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
                TNotificationAttachments m_objTNotificationAttachment = new TNotificationAttachments();
                MMailNotifications m_objMailNotif = new MMailNotifications();

                if (fieldName == AttachmentValueID.Name)
                    m_strReturn = m_objTNotificationAttachment.Name + "." + fieldName;
                else if (fieldName == Filename.Name)
                    m_strReturn = m_objTNotificationAttachment.Name + "." + fieldName;
                else if (fieldName == ContentType.Name)
                    m_strReturn = m_objTNotificationAttachment.Name + "." + fieldName;
                else if (fieldName == RawData.Name)
                    m_strReturn = m_objTNotificationAttachment.Name + "." + fieldName;
                else if (fieldName == MailNotificationID.Name)
                    m_strReturn = m_objTNotificationAttachment.Name + "." + fieldName;
                else if (fieldName == Subject.Name)
                    m_strReturn = m_objMailNotif.Name + "." + fieldName;

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
            PropertyInfo m_pifDimensionVM = this.GetType().GetProperty(fieldName);
            return m_pifDimensionVM != null && Attribute.GetCustomAttribute(m_pifDimensionVM, typeof(KeyAttribute)) != null;
        }
    }
}