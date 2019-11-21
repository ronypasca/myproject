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
    public class ItemPriceVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string ItemPriceID { get; set; }
        public string ItemID { get; set; }
        public string RegionID { get; set; }
        public string ProjectID { get; set; }
        public string ClusterID { get; set; }
        public string UnitTypeID { get; set; }
        public string PriceTypeID { get; set; }
        public string PriceTypeDesc { get; set; }
        public string RegionDesc { get; set; }
        public string ProjectDesc { get; set; }
        public string ClusterDesc { get; set; }
        public string UnitTypeDesc { get; set; }
        public List<ItemPriceVendorVM> ListItemPriceVendorVM { get; set; }
        public List<ItemPriceVendorPeriodVM> ListItemPriceVendorPeriodVM { get; set; }

        public bool StatusUpload { get; set; }

        #region Upload Support
        public string FileUpload { get; set; }
        public string VendorID { get; set; }
        public string VendorDesc { get; set; }
        public bool IsDefault { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string CurrencyID { get; set; }
        public string CurrencyDesc { get; set; }
        public decimal Amount { get; set; }
        #endregion

        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class ItemPriceID
            {
                public static string Desc { get { return "Item Price ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ItemID
            {
                public static string Desc { get { return "Item ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class PriceTypeID
            {
                public static string Desc { get { return "Price Type ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class PriceTypeDesc
            {
                public static string Desc { get { return "Price Type"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class RegionID
            {
                public static string Desc { get { return "Region ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class RegionDesc
            {
                public static string Desc { get { return "Region"; } }
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
            public static class ClusterID
            {
                public static string Desc { get { return "Cluster ID"; } }
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
            public static class UnitTypeID
            {
                public static string Desc { get { return "Unit Type ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class UnitTypeDesc
            {
                public static string Desc { get { return "Unit Type"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ListItemPriceVendorVM
            {
                public static string Desc { get { return "ListItemPriceVendorVM"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ListItemPriceVendorPeriodVM
            {
                public static string Desc { get { return "ListItemPriceVendorPeriodVM"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class FileUpload
            {
                public static string Desc { get { return "File Name"; } }
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
                DItemPrice m_objDItemPrice = new DItemPrice();
                MPriceType m_objMPriceType = new MPriceType();
                MRegion m_objMRegion = new MRegion();
                MProject m_objMProject = new MProject();
                MCluster m_objMCluster = new MCluster();
                MUnitType m_objMUnitType = new MUnitType();

                if (fieldName == ItemPriceID.Name)
                    m_strReturn = m_objDItemPrice.Name + "." + fieldName;
                else if(fieldName == ItemID.Name)
                    m_strReturn = m_objDItemPrice.Name + "." + fieldName;
                else if (fieldName == PriceTypeID.Name)
                    m_strReturn = m_objDItemPrice.Name + "." + fieldName;
                else if (fieldName == PriceTypeDesc.Name)
                    m_strReturn = m_objMPriceType.Name + "." + PriceTypeVM.Prop.PriceTypeDesc.Name;
                else if (fieldName == RegionID.Name)
                    m_strReturn = m_objDItemPrice.Name + "." + fieldName;
                else if (fieldName == RegionDesc.Name)
                    m_strReturn = m_objMRegion.Name + "." + RegionVM.Prop.RegionDesc.Name;
                else if (fieldName == ProjectID.Name)
                    m_strReturn = m_objDItemPrice.Name + "." + fieldName;
                else if (fieldName == ProjectDesc.Name)
                    m_strReturn = m_objMProject.Name + "." + ProjectVM.Prop.ProjectDesc.Name;
                else if (fieldName == ClusterID.Name)
                    m_strReturn = m_objDItemPrice.Name + "." + fieldName;
                else if (fieldName == ClusterDesc.Name)
                    m_strReturn = m_objMCluster.Name + "." + ClusterVM.Prop.ClusterDesc.Name;
                else if (fieldName == UnitTypeID.Name)
                    m_strReturn = m_objDItemPrice.Name + "." + fieldName;
                else if (fieldName == UnitTypeDesc.Name)
                    m_strReturn = m_objMUnitType.Name + "." + UnitTypeVM.Prop.UnitTypeDesc.Name;
               

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
            PropertyInfo m_pifUoMVM = this.GetType().GetProperty(fieldName);
            return m_pifUoMVM != null && Attribute.GetCustomAttribute(m_pifUoMVM, typeof(KeyAttribute)) != null;
        }
    }
}