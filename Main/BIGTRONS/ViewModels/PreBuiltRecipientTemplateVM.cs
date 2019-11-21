using com.SML.Lib.Common;
using com.SML.BIGTRONS.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Collections.Generic;
using Ext.Net;

namespace com.SML.BIGTRONS.ViewModels
{
    public class PreBuiltRecipientTemplateVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string PreBuildRecTemplateID { get; set; }
        public string PreBuildDesc { get; set; }
        public bool IsPIC { get; set; }
        public string FunctionID { get; set; }
        public string FunctionDesc { get; set; }
        public List<PreBuiltRecipientVM> LstRecipient {get;set;}
       
        public string Recipients {
            get
            {
                string Ver = "";
                if (this.LstRecipient != null)
                    if (this.LstRecipient.Count > 0 && this.LstRecipient[0].PreBuildRecID != null)
                    {
                        {
                            Ver = string.Empty;
                            foreach (PreBuiltRecipientVM n in this.LstRecipient)
                            {
                                Ver += $"{n.RecipientTypeDesc} : {n.EmployeeName} ({ n.MailAddress.ToLower()})";
                                Ver += " </br>";
                            }
                        }
                    }
                return Ver;

            }
            set { this.Recipients = value; }
        }
        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class PreBuildRecTemplateID
            {
                public static string Desc { get { return "ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class PreBuildDesc
            {
                public static string Desc { get { return "Description"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class IsPIC
            {
                public static string Desc { get { return "PIC"; } }
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

            public static class FunctionDesc
            {
                public static string Desc { get { return "Function Desc"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class ListPreBuiltRecipient
            {
                public static string Desc { get { return "ListPreBuiltRecipient"; } }
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
                MPreBuildRecipientTemplates m_objMPreBuildRecipientTemplates = new MPreBuildRecipientTemplates();

                if (fieldName == PreBuildRecTemplateID.Name)
                    m_strReturn = m_objMPreBuildRecipientTemplates.Name + "." + fieldName;
                else if (fieldName == PreBuildDesc.Name)
                    m_strReturn = m_objMPreBuildRecipientTemplates.Name + "." + fieldName;
                else if (fieldName == IsPIC.Name)
                    m_strReturn = m_objMPreBuildRecipientTemplates.Name + "." + fieldName;
                else if (fieldName == FunctionID.Name)
                    m_strReturn = m_objMPreBuildRecipientTemplates.Name + "." + fieldName;

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