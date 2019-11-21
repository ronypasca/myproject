using com.SML.Lib.Common;
using com.SML.BIGTRONS.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;

namespace com.SML.BIGTRONS.ViewModels
{
   
    
    public class SchedulesVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string ScheduleID { get; set; }
        public string TaskID { get; set; }
        public string TaskOwnerID { get; set; }
        public string TaskTypeID { get; set; }
        public string StatusID { get; set; }
        public string StatusDesc { get; set; }
        public string FPTID { get; set; }
        public string FPTDescription { get; set; }
        public string FunctionID { get; set; }
        public string FunctionDescription { get; set; }
        public string NotificationTemplateID { get; set; }
        public string NotificationTemplateDesc { get; set; }
        public int CurrentApprovalLvl { get; set; }
        public DateTime StartDate { get; set; }
        public TimeSpan StartDateHour
        {
            get
            {
                if (StartDate != null)                
                    return TimeSpan.Parse(StartDate.ToString(Global.ShortTimeFormat));
                
                return new TimeSpan(0, 0, 0);
            }            
        }
        public DateTime EndDate { get; set; }
        public TimeSpan EndDateHour
        {
            get
            {
                if (EndDate != null)
                    return TimeSpan.Parse(EndDate.ToString(Global.ShortTimeFormat));

                return new TimeSpan(0, 0, 0);
            }
        }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Subject { get; set; }
        public string ProjectID { get; set; }
        public string ProjectDesc { get; set; }
        public string ClusterDesc { get; set; }
        public string ClusterID { get; set; }
        public string Contents { get; set; }
        public string Notes { get; set; }
        public string Weblink { get; set; }
        public string Location { get; set; }
        public int Priority { get; set; }
        public bool IsAllDay { get; set; }
        public bool IsBatchMail{ get; set; }
        public bool IsKeepTask { get; set; }
        public string MailNotificationID { get; set; }
        public string OwnerID { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public List<RecipientsVM> LstRecipients { get; set; }

        public List<NotificationValuesVM> LstNotificationValues { get; set; }

        public List<NotificationAttachmentVM> LstNotificationAttachment { get; set; }
        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class ScheduleID
            {
                public static string Desc { get { return "Schedule ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class TaskID
            {
                public static string Desc { get { return "Task ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class TaskTypeID
            {
                public static string Desc { get { return "Task Type ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class TaskOwnerID
            {
                public static string Desc { get { return "Task Owner ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class StatusID
            {
                public static string Desc { get { return "Status ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class FPTID
            {
                public static string Desc { get { return "FPT"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class FPTDescription
            {
                public static string Desc { get { return "FPT Desciption"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class FunctionID
            {
                public static string Desc { get { return "Functions"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class ProjectDesc
            {
                public static string Desc { get { return "Project"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class ClusterDesc
            {
                public static string Desc { get { return "Cluster"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class ProjectID
            {
                public static string Desc { get { return "ProjectID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class ClusterID
            {
                public static string Desc { get { return "ClusterID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class FunctionDescription
            {
                public static string Desc { get { return "Function Description"; } }
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

            public static class IsBatchMail
            {
                public static string Desc { get { return "as Private"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class NotificationTemplateID
            {
                public static string Desc { get { return "Template"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class NotificationTemplateDesc
            {
                public static string Desc { get { return "Template Desc"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }


            public static class StartDate
            {
                public static string Desc { get { return "Start"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            //public DateTime StartDate { get; set; }

            public static class EndDate
            {
                public static string Desc { get { return "End"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            //public DateTime EndDate { get; set; }

            public static class Subject
            {
                public static string Desc { get { return "Subject"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            //public string Subject { get; set; }

            //public static class Contents
            //{
            //    public static string Desc { get { return "Content"; } }
            //    public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
            //    public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
            //    public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            //}
           

            public static class Notes
            {
                public static string Desc { get { return "Notes"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            //public string Notes { get; set; }

            public static class Weblink
            {
                public static string Desc { get { return "Weblink"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            //public string Weblink { get; set; }

            public static class Location
            {
                public static string Desc { get { return "Location"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            //public string Location { get; set; }

            public static class Priority
            {
                public static string Desc { get { return "Priority"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            //public int Priority { get; set; }

            public static class IsAllDay
            {
                public static string Desc { get { return "All Day"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            //public int Priority { get; set; }

            public static class StatusDesc
            {
                public static string Desc { get { return "Schedule Status"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class OwnerID {
                public static string Desc { get { return "OwnerID"; } }
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

            public static class CreatedBy
            {
                public static string Desc { get { return "Created By"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            //public bool IsAllDay { get; set; }
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
                MSchedules m_objSchedules = new MSchedules();
                MFunctions m_objFunctions = new MFunctions();
                MNotificationTemplates m_objNotificationTemplates = new MNotificationTemplates();
                //MMailNotifications m_objMailNotification = new MMailNotifications();
                MTasks m_objMTask = new MTasks();
                MFPT m_objFPT = new MFPT();
                MCluster m_objCluster = new MCluster();
                MProject m_objProject = new MProject();
                MStatus m_objMStatus = new MStatus();
                DRecipients m_objDRecipient = new DRecipients();
                if (fieldName == ScheduleID.Name)
                    m_strReturn = m_objSchedules.Name + "." + fieldName;
                else if (fieldName == TaskID.Name)
                    m_strReturn = m_objSchedules.Name + "." + fieldName;
                else if (fieldName == TaskTypeID.Name)
                    m_strReturn = m_objMTask.Name + "." + fieldName;
                else if (fieldName == TaskOwnerID.Name)
                    m_strReturn = m_objMTask.Name + "." + fieldName;
                else if (fieldName == StatusID.Name)
                    m_strReturn = m_objSchedules.Name + "." + fieldName;
                else if (fieldName == StatusDesc.Name)
                    m_strReturn = m_objMStatus.Name + "." + fieldName;
                else if (fieldName == FPTID.Name)
                    m_strReturn = m_objSchedules.Name + "." + fieldName;
                else if (fieldName == FPTDescription.Name)
                    m_strReturn = m_objFPT.Name + "." + "Descriptions";
                else if (fieldName == FunctionID.Name)
                    m_strReturn = m_objFunctions.Name + "." + fieldName;
                else if (fieldName == FunctionDescription.Name)
                    m_strReturn = m_objFunctions.Name + "." + "FunctionDesc";
                else if (fieldName == NotificationTemplateID.Name)
                    m_strReturn = m_objNotificationTemplates.Name + "." + fieldName;
                else if (fieldName == NotificationTemplateDesc.Name)
                    m_strReturn = m_objNotificationTemplates.Name + "." + fieldName;
                else if (fieldName == StartDate.Name)
                    m_strReturn = m_objSchedules.Name + "." + fieldName;
                else if (fieldName == EndDate.Name)
                    m_strReturn = m_objSchedules.Name + "." + fieldName;
                else if (fieldName == Subject.Name)
                    m_strReturn = m_objSchedules.Name + "." + fieldName;
                else if (fieldName == Notes.Name)
                    m_strReturn = m_objSchedules.Name + "." + fieldName;
                else if (fieldName == Weblink.Name)
                    m_strReturn = m_objSchedules.Name + "." + fieldName;
                else if (fieldName == Location.Name)
                    m_strReturn = m_objSchedules.Name + "." + fieldName;
                else if (fieldName == Priority.Name)
                    m_strReturn = m_objSchedules.Name + "." + fieldName;
                else if (fieldName == OwnerID.Name)
                    m_strReturn = m_objDRecipient.Name + "." + fieldName;
                else if (fieldName == ProjectID.Name)
                    m_strReturn = m_objSchedules.Name + "." + fieldName;
                else if (fieldName == ClusterID.Name)
                    m_strReturn = m_objSchedules.Name + "." + fieldName;
                else if (fieldName == ProjectDesc.Name)
                    m_strReturn = m_objProject.Name + "." + fieldName;
                else if (fieldName == ClusterDesc.Name)
                    m_strReturn = m_objCluster.Name + "." + fieldName;
                else if (fieldName == IsAllDay.Name)
                    m_strReturn = m_objSchedules.Name + "." + fieldName;
                else if (fieldName == CreatedDate.Name)
                    m_strReturn = m_objSchedules.Name + "." + fieldName;
                else if (fieldName == CreatedBy.Name)
                    m_strReturn = m_objSchedules.Name + "." + fieldName;
                else if (fieldName == IsBatchMail.Name)
                    m_strReturn = m_objSchedules.Name + "." + fieldName;
                else if (fieldName == MailNotificationID.Name)
                    m_strReturn = m_objSchedules.Name + "." + fieldName;
                //else if (fieldName == Contents.Name)
                //    m_strReturn = m_objMailNotification.Name + "." + fieldName;
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
            PropertyInfo m_pifSchedulesVM = this.GetType().GetProperty(fieldName);
            return m_pifSchedulesVM != null && Attribute.GetCustomAttribute(m_pifSchedulesVM, typeof(KeyAttribute)) != null;
        }
    }
}