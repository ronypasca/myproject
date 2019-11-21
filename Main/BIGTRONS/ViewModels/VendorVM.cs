using com.SML.Lib.Common;
using com.SML.BIGTRONS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace com.SML.BIGTRONS.ViewModels
{
    public class VendorVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string VendorID { get; set; }
        public string VendorDesc { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string VendorSubcategoryID { get; set; }
        public string VendorSubcategoryDesc { get; set; }
        public string VendorCategoryDesc { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Postal { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string IDNo { get; set; }
        public string NPWP { get; set; }
        public string Address { get; set; }
        public bool include { get; set; }
        public decimal VendorP3Value { get; set; }
        public decimal VendorSubTotal { get; set; }
        public decimal VendorTotalPackage { get; set; }
        public List<VendorPICsVM> ListPICVendor { get; set; }
        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class VendorID
            {
                public static string Desc { get { return "ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class VendorDesc
            {
                public static string Desc { get { return "Description"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class FirstName
            {
                public static string Desc { get { return "First Name"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class LastName
            {
                public static string Desc { get { return "Last Name"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class VendorSubcategoryID
            {
                public static string Desc { get { return "Vendor Subcategory ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class VendorSubcategoryDesc
            {
                public static string Desc { get { return "Subcategory"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class VendorCategoryDesc
            {
                public static string Desc { get { return "Category"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class City
            {
                public static string Desc { get { return "City"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class Street
            {
                public static string Desc { get { return "Street"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Postal
            {
                public static string Desc { get { return "Postal"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Phone
            {
                public static string Desc { get { return "Phone"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Email
            {
                public static string Desc { get { return "Email"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class IDNo
            {
                public static string Desc { get { return "IDNo"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class NPWP
            {
                public static string Desc { get { return "NPWP"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Address
            {
                public static string Desc { get { return "Address"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ListPICVendor
            {
                public static string Desc { get { return "ListPICVendor"; } }
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
                MVendor m_objMVendor = new MVendor();
                DVendorPICs m_objDVendorPICs = new DVendorPICs();
                MVendorCategory m_objSVendorCategory = new MVendorCategory();
                MVendorSubcategory m_objSVendorSubcategory = new MVendorSubcategory();

                if (fieldName == VendorID.Name)
                    m_strReturn = m_objMVendor.Name + "." + fieldName;
                else if (fieldName == FirstName.Name)
                    m_strReturn = m_objMVendor.Name + "." + fieldName;
                else if (fieldName == VendorDesc.Name)
                    m_strReturn = m_objMVendor.Name + "." + FirstName.Name + " + ' ' + " + m_objMVendor.Name + "." + LastName.Name;
                else if (fieldName == Address.Name)
                    m_strReturn = m_objMVendor.Name + "." + City.Name + " + ', ' +" + m_objMVendor.Name + "." + Street.Name + " + ' - ' +" + m_objMVendor.Name + "." + Postal.Name;
                else if (fieldName == LastName.Name)
                    m_strReturn = m_objMVendor.Name + "." + fieldName;
                else if (fieldName == City.Name)
                    m_strReturn = m_objMVendor.Name + "." + fieldName;
                else if (fieldName == Street.Name)
                    m_strReturn = m_objMVendor.Name + "." + fieldName;
                else if (fieldName == Postal.Name)
                    m_strReturn = m_objMVendor.Name + "." + fieldName;
                else if (fieldName == Phone.Name)
                    m_strReturn = m_objMVendor.Name + "." + fieldName;
                else if (fieldName == Email.Name)
                    m_strReturn = m_objMVendor.Name + "." + fieldName;
                else if (fieldName == IDNo.Name)
                    m_strReturn = m_objMVendor.Name + "." + fieldName;
                else if (fieldName == NPWP.Name)
                    m_strReturn = m_objMVendor.Name + "." + fieldName;
                else if (fieldName == VendorSubcategoryDesc.Name)
                    m_strReturn = m_objSVendorSubcategory.Name + "." + VendorSubcategoryVM.Prop.VendorSubcategoryDesc.Name;
                else if (fieldName == VendorSubcategoryID.Name)
                    m_strReturn = m_objSVendorSubcategory.Name + "." + VendorSubcategoryVM.Prop.VendorSubcategoryID.Name;
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
            PropertyInfo m_pifVendorVM = this.GetType().GetProperty(fieldName);
            return m_pifVendorVM != null && Attribute.GetCustomAttribute(m_pifVendorVM, typeof(KeyAttribute)) != null;
        }
    }
}