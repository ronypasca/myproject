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
    public class PreBuiltRecipientVM
    {
        #region Public Property
        [Key, Column(Order = 1)]
        public string PreBuildRecID { get; set; }
        public string PreBuildRecTemplateID { get; set; }
        public string EmployeeID { get; set; }
        public string RecipientTypeID { get; set; }
        public string EmployeeName { get; set; }
        public string MailAddress { get; set; }
        public string UserID { get; set; }
        public string RecipientTypeDesc { get; set; }
        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class PreBuildRecID
            {
                public static string Desc { get { return "PreBuildRecID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class PreBuildRecTemplateID
            {
                public static string Desc { get { return "PreBuildRecTemplateID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class EmployeeID
            {
                public static string Desc { get { return "EmployeeID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class UserID
            {
                public static string Desc { get { return "UserID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class EmployeeName
            {
                public static string Desc { get { return "EmployeeName"; } }
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
                public static string Desc { get { return "Type ID"; } }
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
                DPreBuildRecipients m_objDPreBuildRecipients = new DPreBuildRecipients();
                MRecipientTypes m_objMRecipientTypes = new MRecipientTypes();
                MEmployee m_objMEmployee = new MEmployee();
                MUser m_objMUser = new MUser();
                DEmpCommunication m_objDEmpCommunication = new DEmpCommunication();

                if (fieldName == PreBuildRecTemplateID.Name)
                    m_strReturn = m_objDPreBuildRecipients.Name + "." + fieldName;
                else if (fieldName == PreBuildRecID.Name)
                    m_strReturn = m_objDPreBuildRecipients.Name + "." + fieldName;
                else if (fieldName == EmployeeID.Name)
                    m_strReturn = m_objDPreBuildRecipients.Name + "." + fieldName;
                else if (fieldName == EmployeeName.Name)
                    m_strReturn = $"REPLACE({m_objMEmployee.Name}.{EmployeeVM.Prop.FirstName.Name} + {m_objMEmployee.Name}.{EmployeeVM.Prop.MiddleName.Name} + {m_objMEmployee.Name}.{EmployeeVM.Prop.LastName.Name},'..','')";
                else if (fieldName == MailAddress.Name)
                    m_strReturn = m_objDEmpCommunication.Name + "." + EmployeeCommunicationVM.Prop.CommunicationDesc.Name;
                else if (fieldName == UserID.Name)
                    m_strReturn = m_objMUser.Name + "." + fieldName;
                else if (fieldName == RecipientTypeID.Name)
                    m_strReturn = m_objDPreBuildRecipients.Name + "." + fieldName;
                else if (fieldName == RecipientTypeDesc.Name)
                    m_strReturn = m_objMRecipientTypes.Name + "." + "Descriptions";

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