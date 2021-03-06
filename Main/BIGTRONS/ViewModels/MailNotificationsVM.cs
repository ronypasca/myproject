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
    public class MailNotificationsVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string MailNotificationID { get; set; }
        public bool Importance { get; set; }
        public string FunctionID { get; set; }
        public string NotificationTemplateID { get; set; }
        public string Subject { get; set; }
        public string Contents { get; set; }
        public int StatusID { get; set; }
        public string ScheduleID { get; set; }
        public string ScheduleStartDate { get; set; }
        public string ScheduleStartTime { get; set; }

        public string TaskID { get; set; }
        public string FPTID { get; set; }
        public DateTime? CreatedDate { get; set; }

        public string NotificationTemplateDesc { get; set; }
        public string FPTDesc { get; set; }
        public string StatusDesc { get; set; }
        public string FunctionDesc { get; set; }
        public string TaskStatusDesc { get; set; }
        public int? TaskStatusID { get; set; }
        
        public string RecipientsTO { get; set; }
        public string RecipientsCC { get; set; }
        public string RecipientsBCC { get; set; }
        public List<RecipientsVM> RecipientsVM { get; set; }
        public List<TemplateTagsVM> TemplateTagsVM { get; set; }
        public List<NotificationValuesVM> NotificationValuesVM { get; set; }
        public List<NotificationAttachmentVM> NotificationAttachmentVM { get; set; }
        public NotificationMapVM NotificationMapVM { get; set; }
        public string NotificationAttachmentName { get; set; }



        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class MailNotificationID
            {
                public static string Desc { get { return "ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Importance
            {
                public static string Desc { get { return "Importance"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ScheduleStartDate
            {
                public static string Desc { get { return "ScheduleStartDate"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }//
            public static class ScheduleID
            {
                public static string Desc { get { return "ScheduleID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }//ScheduleStartDate
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
            public static class StatusID
            {
                public static string Desc { get { return "StatusID"; } }
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
            public static class FPTID
            {
                public static string Desc { get { return "FPTID"; } }
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

            public static class NotificationTemplateID
            {
                public static string Desc { get { return "NotificationTemplateID"; } }
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
            public static class FPTDesc
            {
                public static string Desc { get { return "FPT Desc"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class StatusDesc
            {
                public static string Desc { get { return "Status Desc"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class FunctionDesc
            {
                public static string Desc { get { return "Function Desc"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class TaskStatusDesc
            {
                public static string Desc { get { return "Task Status Desc"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class TaskStatusID
            {
                public static string Desc { get { return "TaskStatusID"; } }
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
                MSchedules m_objMSchedules = new MSchedules();
                MFunctions m_objMFunctions = new MFunctions();
                MTasks m_objMTasks = new MTasks();
                MStatus m_objMStatus  = new MStatus();
                MNotificationTemplates m_objMNotificationTemplates = new MNotificationTemplates();
                

                if (fieldName == MailNotificationID.Name)
                    m_strReturn = m_objMMailNotifications.Name + "." + fieldName;
                else if (fieldName == Importance.Name)
                    m_strReturn = m_objMMailNotifications.Name + "." + fieldName;
                else if (fieldName == TaskID.Name)
                    m_strReturn = m_objMMailNotifications.Name + "." + fieldName;
                else if (fieldName == Subject.Name)
                    m_strReturn = m_objMMailNotifications.Name + "." + fieldName;
                else if (fieldName == Contents.Name)
                    m_strReturn = m_objMMailNotifications.Name + "." + fieldName;
                else if (fieldName == StatusID.Name)
                    m_strReturn = m_objMMailNotifications.Name + "." + fieldName;
                else if (fieldName == TaskStatusID.Name)
                    m_strReturn = m_objMTasks.Name + "." + "StatusID";
                else if (fieldName == TaskID.Name)
                    m_strReturn = m_objMMailNotifications.Name + "." + fieldName;
                else if (fieldName == FPTID.Name)
                    m_strReturn = m_objMMailNotifications.Name + "." + fieldName;
                else if (fieldName == CreatedDate.Name)
                    m_strReturn = m_objMMailNotifications.Name + "." + fieldName;
                else if (fieldName == FunctionID.Name)
                    m_strReturn = m_objMFunctions.Name + "." + fieldName;
                else if (fieldName == FunctionDesc.Name)
                    m_strReturn = m_objMFunctions.Name + "." + fieldName;
                else if (fieldName == NotificationTemplateID.Name)
                    m_strReturn = m_objMNotificationTemplates.Name + "." + fieldName;
                else if (fieldName == NotificationTemplateDesc.Name)
                    m_strReturn = m_objMNotificationTemplates.Name + "." + fieldName;
                else if (fieldName == StatusDesc.Name)
                    m_strReturn = m_objMStatus.Name + "." + fieldName;
                else if (fieldName == FPTDesc.Name)
                    m_strReturn = m_objMFPT.Name + ".Descriptions";
                else if (fieldName == TaskStatusDesc.Name)
                    m_strReturn = "MStatusMTasks.StatusDesc";
                else if (fieldName == ScheduleID.Name)
                    m_strReturn = m_objMSchedules.Name + "." + fieldName;
                else if (fieldName == ScheduleStartDate.Name)
                    m_strReturn = m_objMSchedules.Name + "." + "StartDate";
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