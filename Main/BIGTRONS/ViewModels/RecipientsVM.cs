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
    public class RecipientsVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string RecipientID { get; set; }
        public string RecipientDesc { get; set; }
        public DateTime ScheduleDate { get; set; }
        public string MailAddress { get; set; }
        public string ScheduleID { get; set; }
        public string OwnerID { get; set; }
        public List<MailNotificationsVM> lstMailNotification { get; set; }       
        public string RecipientNRK { get; set; }
        public string RecipientName { get; set; }
        public string RecipientTypeID { get; set; }
        public string MailNotificationID { get; set; }
        public string RecipientTypeDesc { get; set; }
        public string RecipientTypeName
        {
            get
            {
                try
                {
                    int m_RecipientTypeID = Convert.ToInt16(RecipientTypeID);
                    return ((Enum.RecipientTypes)m_RecipientTypeID).ToString();
                }
                catch (Exception)
                {
                    return string.Empty;
                }
                
                
            }
        }


        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class RecipientID
            {
                public static string Desc { get { return "ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class RecipientDesc
            {
                public static string Desc { get { return "Description"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class RecipientNRK
            {
                public static string Desc { get { return "Recipient NRK"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class RecipientName
            {
                public static string Desc { get { return "Recipient Name"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class MailAddress
            {
                public static string Desc { get { return "Mail Address"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class RecipientTypeID
            {
                public static string Desc { get { return "Recipient Type ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class RecipientTypeDesc
            {
                public static string Desc { get { return "Type"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class OwnerID
            {
                public static string Desc { get { return "Owner ID"; } }
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
            public static class ScheduleID
            {
                public static string Desc { get { return "Schedule ID"; } }
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
                DRecipients m_objDRecipients = new DRecipients();
                MEmployee m_objMEmployee = new MEmployee();
                MSchedules m_objMSchedule = new MSchedules();                
                MTasks m_objMTasks = new MTasks();
                MRecipientTypes m_objMRecipientTypes = new MRecipientTypes();

                if (fieldName == RecipientID.Name)
                    m_strReturn = m_objDRecipients.Name + "." + fieldName;
                else if (fieldName == RecipientDesc.Name)
                    m_strReturn = m_objDRecipients.Name + "." + fieldName;
                else if (fieldName == OwnerID.Name)
                    m_strReturn = m_objDRecipients.Name + "." + fieldName;
                else if (fieldName == MailAddress.Name)
                    m_strReturn = m_objDRecipients.Name + "." + fieldName;
                else if (fieldName == RecipientNRK.Name)
                    m_strReturn = m_objMEmployee.Name + "." + EmployeeVM.Prop.EmployeeID.Name;
                else if (fieldName == RecipientName.Name)
                    m_strReturn = $"REPLACE({m_objMEmployee.Name}.{EmployeeVM.Prop.FirstName.Name} + {m_objMEmployee.Name}.{EmployeeVM.Prop.MiddleName.Name} + {m_objMEmployee.Name}.{EmployeeVM.Prop.LastName.Name},'..','')";
                else if (fieldName == RecipientTypeID.Name)
                    m_strReturn = m_objDRecipients.Name + "." + fieldName;
                else if (fieldName == RecipientTypeDesc.Name)
                    m_strReturn = m_objMRecipientTypes.Name + "." + "Descriptions";
                else if (fieldName == MailNotificationID.Name)
                    m_strReturn = m_objDRecipients.Name + "." + fieldName;
                else if (fieldName == ScheduleID.Name)
                    m_strReturn = m_objMSchedule.Name + "." + fieldName;
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