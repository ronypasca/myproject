using com.SML.Lib.Common;
using com.SML.BIGTRONS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;

namespace com.SML.BIGTRONS.ViewModels
{
    public class MinutesEntryVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string MinuteEntryID { get; set; }
        public string FPTID { get; set; }
        public string FPTDesc { get; set; }

        public string FunctionID { get; set; }
        public string FunctionDesc { get; set; }
        public string MinuteTemplateID { get; set; }
        public string MinuteTemplateDescriptions { get; set; }
        public int StatusID { get; set; }
        public string StatusDesc { get; set; }
        public string TaskID { get; set; }
        public string MailNotificationID { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string NotificationTemplateID { get; set; }
        public string NotificationTemplateDesc { get; set; }
        public string ScheduleID { get; set; }
        public string ScheduleDesc { get; set; }
        public DateTime CreatedDate { get; set; }
        public string TaskStatusID { get; set; }
        public string TaskStatusDesc { get; set; }
        public string NotificationStatusID { get; set; }
        public string NotificationStatusDesc { get; set; }
        public List<RecipientsVM> ListRecipients { get; set; }
        public List<EntryValuesVM> ListMinutesValues { get; set; }
        public List<NotificationValuesVM> ListNotificationValues { get; set; }
        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class MinuteEntryID
            {
                public static string Desc { get { return "MinuteEntryID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class MailNotificationID
            {
                public static string Desc { get { return "MailNotificationID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class FPTID
            {
                public static string Desc { get { return "FPTID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class FPTDesc
            {
                public static string Desc { get { return "FPT Description"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class MinuteTemplateID
            {
                public static string Desc { get { return "MinuteTemplateID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class MinuteTemplateDescriptions
            {
                public static string Desc { get { return "MinuteTemplateDesc"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class TaskID
            {
                public static string Desc { get { return "TaskID"; } }
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
            public static class NotificationTemplateDesc
            {
                public static string Desc { get { return "NotificationTemplateDesc"; } }
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
            public static class PFTDesc
            {
                public static string Desc { get { return "PFT Desc"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            
            public static class FunctionDesc
            {
                public static string Desc { get { return "Function"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class TaskStatusID
            {
                public static string Desc { get { return "Approval Status ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class TaskStatusDesc
            {
                public static string Desc { get { return "Approval Status"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class NotificationStatusID
            {
                public static string Desc { get { return "Mail Status ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class NotificationStatusDesc
            {
                public static string Desc { get { return "Mail Status"; } }
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

            public static class Contents
            {
                public static string Desc { get { return "Contents"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class NotificationTemplateID
            {
                public static string Desc { get { return "NotificationTemplateID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ScheduleID
            {
                public static string Desc { get { return "Schedule"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ScheduleDesc
            {
                public static string Desc { get { return "Schedule"; } }
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

            public static class CreatedDate
            {
                public static string Desc { get { return "Created Date"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class ListRecipients
            {
                public static string Desc { get { return "ListRecipients"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class ListMinutesValues
            {
                public static string Desc { get { return "ListMinutesValues"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class ListNotificationValues
            {
                public static string Desc { get { return "ListMinutesValues"; } }
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
                MMailNotifications m_objMMailNotifications = new MMailNotifications();
                MFPT m_objMFPT = new MFPT();
                DNotificationMap m_objDNotificationMap = new DNotificationMap();
                MFunctions m_objMFunctions = new MFunctions();
                MTasks m_objMTasks = new MTasks();
                MStatus m_objMStatus  = new MStatus();
                MNotificationTemplates m_objMNotificationTemplates = new MNotificationTemplates();
                MMinuteTemplates m_objMMinuteTemplates = new MMinuteTemplates();
                TMinuteEntries m_objTMinuteEntries = new TMinuteEntries();
                MSchedules m_objMSchedules = new MSchedules();

                if (fieldName == MailNotificationID.Name)
                    m_strReturn = m_objTMinuteEntries.Name + "." + fieldName;
                else if (fieldName == MinuteEntryID.Name)
                    m_strReturn = m_objTMinuteEntries.Name + "." + fieldName;
                else if (fieldName == CreatedDate.Name)
                    m_strReturn = m_objTMinuteEntries.Name + "." + fieldName;
                else if (fieldName == ScheduleID.Name)
                    m_strReturn = m_objTMinuteEntries.Name + "." + fieldName;
                else if (fieldName == ScheduleDesc.Name)
                    m_strReturn = m_objMSchedules.Name + "." + Subject.Name;
                else if (fieldName == TaskID.Name)
                    m_strReturn = m_objTMinuteEntries.Name + "." + fieldName;
                else if (fieldName == Subject.Name)
                    m_strReturn = m_objMMailNotifications.Name + "." + fieldName;
                else if (fieldName == Contents.Name)
                    m_strReturn = m_objMMailNotifications.Name + "." + fieldName;
                else if (fieldName == StatusID.Name)
                    m_strReturn = m_objTMinuteEntries.Name + "." + fieldName;
                else if (fieldName == NotifMapID.Name)
                    m_strReturn = m_objDNotificationMap.Name + "." + fieldName;
                else if (fieldName == TaskID.Name)
                    m_strReturn = m_objMMailNotifications.Name + "." + fieldName;
                else if (fieldName == FPTID.Name)
                    m_strReturn = m_objMMailNotifications.Name + "." + fieldName;
                else if (fieldName == FunctionID.Name)
                    m_strReturn = m_objMFunctions.Name + "." + fieldName;
                else if (fieldName == FunctionDesc.Name)
                    m_strReturn = m_objMFunctions.Name + "." + fieldName;
                else if (fieldName == NotificationTemplateID.Name)
                    m_strReturn = m_objMNotificationTemplates.Name + "." + fieldName;
                else if (fieldName == NotificationTemplateDesc.Name)
                    m_strReturn = m_objMNotificationTemplates.Name + "." + fieldName;
                else if (fieldName == MinuteTemplateID.Name)
                    m_strReturn = m_objMMinuteTemplates.Name + "." + fieldName;
                else if (fieldName == MinuteTemplateDescriptions.Name)
                    m_strReturn = m_objMMinuteTemplates.Name + "." + fieldName;
                else if (fieldName == StatusDesc.Name)
                    m_strReturn = m_objMStatus.Name + "." + fieldName;
                else if (fieldName == FPTDesc.Name)
                    m_strReturn = m_objMFPT.Name + ".Descriptions";
                else if (fieldName == TaskStatusDesc.Name)
                    m_strReturn = "MStatusMTasks.StatusDesc";
                else if (fieldName == TaskStatusID.Name)
                    m_strReturn = "MStatusMTasks.StatusID";
                else if (fieldName == NotificationStatusDesc.Name)
                    m_strReturn = "MStatusMailNotifications.StatusDesc";
                else if (fieldName == NotificationStatusID.Name)
                    m_strReturn = "MStatusMailNotifications.StatusID";
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
            PropertyInfo m_pifBudgetPlanTypeVM = this.GetType().GetProperty(fieldName);
            return m_pifBudgetPlanTypeVM != null && Attribute.GetCustomAttribute(m_pifBudgetPlanTypeVM, typeof(KeyAttribute)) != null;
        }
    }
}