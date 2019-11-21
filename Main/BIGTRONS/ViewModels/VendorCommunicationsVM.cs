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
    public class VendorCommunicationsVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string VendorCommID { get; set; }
        public string CommunicationTypeID { get; set; }
        public string VendorPICID { get; set; }
        public bool IsDefault { get; set; }
        public string CommDesc { get; set; }
        public string VendorID { get; set; }
        public string CommTypeDesc { get; set; }
        public List<CommunicationTypesVM> ListCommunicationTypes { get; set; }

        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class VendorCommID
            {
                public static string Desc { get { return "ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class CommunicationTypeID
            {
                public static string Desc { get { return "Communication Type ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class VendorPICID
            {
                public static string Desc { get { return "Vendor PIC ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class IsDefault
            {
                public static string Desc { get { return "Is Default"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class CommDesc
            {
                public static string Desc { get { return "Comm Desc"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class VendorID
            {
                public static string Desc { get { return "Vendor ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class CommTypeDesc
            {
                public static string Desc { get { return "Comm Type Desc"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class ListCommunicationTypes
            {
                public static string Desc { get { return "ListCommunicationTypes"; } }
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
                DVendorCommunications m_objDVendorCommunications = new DVendorCommunications();
                //DVendorPICs m_objDVendorPICs = new DVendorPICs();
                MVendor m_objMVendor = new MVendor();
                MCommunicationTypes m_objMCommunicationTypes = new MCommunicationTypes();

                if (fieldName == VendorCommID.Name)
                    m_strReturn = m_objDVendorCommunications.Name + "." + fieldName;
                else if (fieldName == CommunicationTypeID.Name)
                    m_strReturn = m_objDVendorCommunications.Name + "." + fieldName;
                else if (fieldName == VendorPICID.Name)
                    m_strReturn = m_objDVendorCommunications.Name + "." + fieldName;
                else if (fieldName == IsDefault.Name)
                    m_strReturn = $"ISNULL({m_objDVendorCommunications.Name}.{fieldName},0)";
                else if (fieldName == CommDesc.Name)
                    m_strReturn = m_objDVendorCommunications.Name + "." + fieldName;
                else if (fieldName == CommTypeDesc.Name)
                    m_strReturn = m_objMCommunicationTypes.Name + "." + fieldName;
                else if (fieldName == VendorID.Name)
                    m_strReturn = m_objMVendor.Name + "." + fieldName;
                

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
            PropertyInfo m_pifTaskTypesVM = this.GetType().GetProperty(fieldName);
            return m_pifTaskTypesVM != null && Attribute.GetCustomAttribute(m_pifTaskTypesVM, typeof(KeyAttribute)) != null;
        }
    }
}