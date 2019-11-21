using com.SML.Lib.Common;
using com.SML.BIGTRONS.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace com.SML.BIGTRONS.ViewModels
{
    public class WBSVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string WBSID { get; set; }
        public string WBSDesc { get; set; }
        public string ProjectID { get; set; }
        public string ProjectDesc { get; set; }
        public string CompanyID { get; set; }
        public string CompanyDesc { get; set; }

        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class WBSID
            {
                public static string Desc { get { return "ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class WBSDesc
            {
                public static string Desc { get { return "Description"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ProjectID
            {
                public static string Desc { get { return "Project ID"; } }
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

            public static class CompanyID
            {
                public static string Desc { get { return "CompanyID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class CompanyDesc
            {
                public static string Desc { get { return "Company"; } }
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
                MWBS m_objWBS = new MWBS();
                MCompany m_objMcompany = new MCompany();
                MProject m_objMProject = new MProject();

                if (fieldName == WBSID.Name)
                    m_strReturn = m_objWBS.Name + "." + fieldName;
                else if (fieldName == WBSDesc.Name)
                    m_strReturn = m_objWBS.Name + "." + fieldName;
                else if (fieldName == ProjectID.Name)
                    m_strReturn = m_objWBS.Name + "." + ProjectID.Name;
                else if (fieldName == ProjectDesc.Name)
                    m_strReturn = m_objMProject.Name + "." + ProjectVM.Prop.ProjectDesc.Name;
                else if (fieldName == CompanyID.Name)
                    m_strReturn = m_objMcompany.Name + "." + CompanyVM.Prop.CompanyID.Name;
                else if (fieldName == CompanyDesc.Name)
                    m_strReturn = m_objMcompany.Name + "." + CompanyVM.Prop.CompanyDesc.Name;


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