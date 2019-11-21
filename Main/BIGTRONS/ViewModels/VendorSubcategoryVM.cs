using com.SML.Lib.Common;
using com.SML.BIGTRONS.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace com.SML.BIGTRONS.ViewModels
{
    public class VendorSubcategoryVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string VendorSubcategoryID { get; set; }
        public string VendorSubcategoryDesc { get; set; }
        public string VendorCategoryID { get; set; }
        public string VendorCategoryDesc { get; set; }

        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class VendorSubcategoryID
            {
                public static string Desc { get { return "ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class VendorSubcategoryDesc
            {
                public static string Desc { get { return "Description"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class VendorCategoryID
            {
                public static string Desc { get { return "VendorCategory ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class VendorCategoryDesc
            {
                public static string Desc { get { return "VendorCategory"; } }
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
                MVendorSubcategory m_objMVendorSubcategory = new MVendorSubcategory();
                MVendorCategory m_objSVendorCategory = new MVendorCategory();

                if (fieldName == VendorSubcategoryID.Name)
                    m_strReturn = m_objMVendorSubcategory.Name + "." + fieldName;
                else if (fieldName == VendorSubcategoryDesc.Name)
                    m_strReturn = m_objMVendorSubcategory.Name + "." + fieldName;
                else if (fieldName == VendorCategoryID.Name)
                    m_strReturn = m_objMVendorSubcategory.Name + "." + VendorCategoryID.Name;
                else if (fieldName == VendorCategoryDesc.Name)
                    m_strReturn = m_objSVendorCategory.Name + "." + VendorCategoryVM.Prop.VendorCategoryDesc.Name;

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
            PropertyInfo m_pifVendorSubcategoryVM = this.GetType().GetProperty(fieldName);
            return m_pifVendorSubcategoryVM != null && Attribute.GetCustomAttribute(m_pifVendorSubcategoryVM, typeof(KeyAttribute)) != null;
        }
    }
}