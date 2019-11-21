using com.SML.Lib.Common;
using com.SML.BIGTRONS.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using SW = System.Web;

namespace com.SML.BIGTRONS.ViewModels
{
    public class MyTaskVM
    {
        //private string ApprovalRemarks_ { get; set; }
        #region Public Property

        [Key, Column(Order = 1)]
        public string TaskID { get; set; }
        public string TaskDesc { get; set; }
        public string TaskTypeID { get; set; }
        public DateTime TaskDateTimeStamp { get; set; }
        public string TaskOwnerID { get; set; }
        public string TaskOwnerDesc { get; set; }
        public string TaskDesciption { get; set; }
        public int StatusID { get; set; }
        public int CurrentApprovalLvl { get; set; }
        public int ConfigApprovalInterval { get; set; }
        public string StatusDesc { get; set; }
        public string Remarks { get; set; }
        public string ApprovalRemarks { get; set; }
        public string RoleID { get; set; }
        public string RoleParentID { get; set; }
        public string RoleChildID { get; set; }
        public string FPTVendorWinnerDesc { get; set; }
        public string FPTVendorWinnerFPTID { get; set; }
        public string FPTNegoConfigDescrptions { get; set; }

        //    get{
        //        if (SW.HttpContext.Current.User.Identity.Name == TaskOwnerID)
        //        {
        //            this.ApprovalRemarks_ = this.Remarks;
        //            return this.ApprovalRemarks_;
        //        }
        //        else
        //            return "";
        //    }
        //    set {
        //        this.ApprovalRemarks_ = value;
        //    }
        //}
        public string ApprovalStatusDesc { get; set; }
        public int ApprovalStatusID { get; set; }
        public string CreatedBy { get; set; }
        public string CreatorFullName { get; set; }
        public string CreatedRoleID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Summary { get; set; }
        //public string MailNotificationID { get; set; }
        public string ScheduleID { get; set; }
        public string InvitationFunctionID { get; set; }
        public string ScheduleStartDate { get; set; }
        public string Subject { get; set; }
        //public string Contents { get; set; }
        public string NotificationTemplateID { get; set; }

        #endregion

        #region Public Field Property

        public static class Prop
        {
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

            public static class TaskDescription
            {
                public static string Desc { get { return "Task Description"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class TaskDesc
            {
                public static string Desc { get { return "Description"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class InvitationFunctionID
            {
                public static string Desc { get { return "Function ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class TaskDateTimeStamp
            {
                public static string Desc { get { return "Status Date"; } }
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
            public static class TaskOwnerDesc
            {
                public static string Desc { get { return "Task Owner"; } }
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
            public static class StatusDesc
            {
                public static string Desc { get { return "Status"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            
            public static class CurrentApprovalLvl
            {
                public static string Desc { get { return "Current Approval Level"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ConfigApprovalInterval
            {
                public static string Desc { get { return "Config Approval Interval"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class Remarks
            {
                public static string Desc { get { return "Remarks"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ApprovalRemarks
            {
                public static string Desc { get { return "Approval Remarks"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ApprovalStatusID
            {
                public static string Desc { get { return "Approval ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ScheduleID
            {
                public static string Desc { get { return "Schedule ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            //public static class MailNotificationID
            //{
            //    public static string Desc { get { return "MailNotification ID"; } }
            //    public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
            //    public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
            //    public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            //}
            public static class Subject
            {
                public static string Desc { get { return "MailNotification ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            //public static class Contents
            //{
            //    public static string Desc { get { return "Content"; } }
            //    public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
            //    public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
            //    public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            //}
            public static class NotificationTemplateID
            {
                public static string Desc { get { return "Notification Template ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ApprovalStatusDesc
            {
                public static string Desc { get { return "Approval"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Summary
            {
                public static string Desc { get { return "Summary"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class CreatorFullName
            {
                public static string Desc { get { return "Created By"; } }
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
            public static class CreatedRoleID
            {
                public static string Desc { get { return "Created Role ID"; } }
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
            public static class RoleID
            {
                public static string Desc { get { return "RoleID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class RoleParentID
            {
                public static string Desc { get { return "Role Parent ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }//ScheduleStartDate
            public static class ScheduleStartDate
            {
                public static string Desc { get { return "Start Date"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class RoleChildID
            {
                public static string Desc { get { return "Row Child ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class FPTVendorWinnerDesc
            {
                public static string Desc { get { return "FPT Descriptions"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class FPTNegoConfigDescrptions
            {
                public static string Desc { get { return "FPT Descriptions"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }


            //FPTVendorWinnerFPTID
            public static class FPTVendorWinnerFPTID
            {
                public static string Desc { get { return "FPTID"; } }
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
                MTasks m_objTasks = new MTasks();
                DTaskDetails m_objTaskDetails = new DTaskDetails();
                MTaskTypes m_objMTaskTypes = new MTaskTypes();
                MStatus m_objMStatus = new MStatus();
                MRole m_objMRole = new MRole();
                MUser m_objMuser = new MUser();
                MMailNotifications m_objMMailNotif = new MMailNotifications();
                MSchedules m_objMSchedules = new MSchedules();
                if (fieldName == TaskID.Name)
                    m_strReturn = m_objTasks.Name + "." + fieldName;
                else if (fieldName == TaskTypeID.Name)
                    m_strReturn = m_objTasks.Name + "." + fieldName;
                else if (fieldName == TaskDesc.Name)
                    m_strReturn = m_objTasks.Name + "." + fieldName;
                else if (fieldName == TaskDescription.Name)
                    m_strReturn = m_objMTaskTypes.Name + "." + "Descriptions";
                else if (fieldName == TaskDateTimeStamp.Name)
                    m_strReturn = m_objTasks.Name + "." + fieldName;
                else if (fieldName == TaskOwnerID.Name)
                    m_strReturn = m_objTasks.Name + "." + fieldName;
                else if (fieldName == TaskOwnerDesc.Name)
                    m_strReturn = m_objMRole.Name + "." + RoleVM.Prop.RoleDesc.Name;
                else if (fieldName == StatusID.Name)
                    m_strReturn = m_objTasks.Name + "." + fieldName;
                else if (fieldName == StatusDesc.Name)
                    m_strReturn = m_objMStatus.Name + "." + fieldName;
                else if (fieldName == CurrentApprovalLvl.Name)
                    m_strReturn = m_objTasks.Name + "." + fieldName;
                else if (fieldName == Remarks.Name)
                    m_strReturn = m_objTasks.Name + "." + fieldName;
                else if (fieldName == Summary.Name)
                    m_strReturn = m_objTasks.Name + "." + fieldName;
                else if (fieldName == RoleID.Name)
                    m_strReturn = m_objMuser.Name + "." + fieldName;
                //else if (fieldName == MailNotificationID.Name)
                //    m_strReturn = m_objMMailNotif.Name + "." + fieldName;
                else if (fieldName == Subject.Name)
                    m_strReturn = m_objMMailNotif.Name + "." + fieldName;
                else if (fieldName == ScheduleID.Name)
                    m_strReturn = m_objMSchedules.Name + "." + fieldName;
                else if (fieldName == ScheduleStartDate.Name)
                    m_strReturn = m_objMSchedules.Name + "." + "StartDate";
                //else if (fieldName == Contents.Name)
                //    m_strReturn = m_objMMailNotif.Name + "." + fieldName;InvitationFunctionID
                else if (fieldName == NotificationTemplateID.Name)
                    m_strReturn = m_objMMailNotif.Name + "." + fieldName;
                else if (fieldName == CreatedBy.Name)
                    m_strReturn = m_objTasks.Name + "." + fieldName;
                else if (fieldName == CreatorFullName.Name)
                    m_strReturn = m_objMuser.Name + "." + "FullName";
                //else if (fieldName == CreatedRoleID.Name)ScheduleStartDate
                //m_strReturn = m_objMuser.Name + "." + UserVM.Prop.RoleID.Name;
                else if (fieldName == CreatedDate.Name)
                    m_strReturn = m_objTasks.Name + "." + fieldName;
                else if (fieldName == FPTVendorWinnerDesc.Name)
                    m_strReturn = "VendorWinnerFPTDesc.VendorWinnerDesc";
                else if (fieldName == FPTNegoConfigDescrptions.Name)
                    m_strReturn = "NegoConfDesc.NegoConfigDescrption";
                else if(fieldName == FPTVendorWinnerFPTID.Name)
                    m_strReturn = "NegoConfDesc.FPTID";
                //NegoConfigDescrption

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
            PropertyInfo m_pifTaskVM = this.GetType().GetProperty(fieldName);
            return m_pifTaskVM != null && Attribute.GetCustomAttribute(m_pifTaskVM, typeof(KeyAttribute)) != null;
        }
    }
}