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
    public class ItemPriceVendorPeriodVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string ItemPriceID { get; set; }
        [Key, Column(Order = 2)]
        public string VendorID { get; set; }
        [Key, Column(Order = 3)]
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string CurrencyID { get; set; }
        public decimal Amount { get; set; }
        public List<ItemPriceVM> ListItemPrice { get; set; }
        public List<VendorVM> ListVendor { get; set; }
        public List<MyTaskVM> ListMyTask { get; set; }
        public string CurrencyDesc { get; set; }
        public string VendorDesc { get; set; }
        public string ItemID { get; set; }
        public string ItemDesc { get; set; }
        public bool IsDefault { get; set; }
        public string RegionID { get; set; }
        public string RegionDesc { get; set; }
        public string ProjectID { get; set; }
        public string ClusterID { get; set; }
        public string UnitTypeID { get; set; }
        public string PriceTypeID { get; set; }
        public bool DisplayPrice { get; set; }
        public string ItemGroupDesc { get; set; }
        public string ItemTypeID { get; set; }
        public string TaskID { get; set; }
        public int RowNo { get; set; }
        public string UoMID { get; set; }
        public string UoMDesc { get; set; }
        public string Url { get; set; }
        public bool? Visible { get; set; }
        public string Price { get { return Amount.ToString(Global.IntegerNumberFormat); } }
        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class ListItemPrice
            {
                public static string Desc { get { return "ListItemPrice"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ListVendor
            {
                public static string Desc { get { return "ListVendor"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ListMyTask
            {
                public static string Desc { get { return "ListMyTask"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ItemGroupDesc
            {
                public static string Desc { get { return "Item Group Description"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ItemTypeID
            {
                public static string Desc { get { return "Item Type ID"; } }
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
            public static class ItemPriceID
            {
                public static string Desc { get { return "Item Price ID"; } }
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
            public static class VendorDesc
            {
                public static string Desc { get { return "Vendor"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ValidFrom
            {
                public static string Desc { get { return "Start"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ValidTo
            {
                public static string Desc { get { return "End"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class CurrencyID
            {
                public static string Desc { get { return "Currency ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class CurrencyDesc
            {
                public static string Desc { get { return "Currency"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Amount
            {
                public static string Desc { get { return "Amount"; } }
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
            public static class ItemDesc
            {
                public static string Desc { get { return "Item Desc"; } }
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

            public static class ProjectID
            {
                public static string Desc { get { return "Project ID"; } }
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

            public static class UnitTypeID
            {
                public static string Desc { get { return "Unit Type ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class IsDefault
            {
                public static string Desc { get { return "Default"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class DisplayPrice
            {
                public static string Desc { get { return "Display Price"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class RowNo
            {
                public static string Desc { get { return "RowNo"; } }
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
                DItemPriceVendor m_objDItemPriceVendor = new DItemPriceVendor();
                DItemPriceVendorPeriod m_objDItemPriceVendorPeriod = new DItemPriceVendorPeriod ();
                MVendor m_objMVendor = new MVendor();
                MCurrency m_objMCurrency = new MCurrency();
                DItemPrice m_objDItemPrice = new DItemPrice();
                MItem m_objMitem = new MItem();
                MItemGroup m_objMItemGroup = new MItemGroup();

                if (fieldName == ItemPriceID.Name)
                    m_strReturn = m_objDItemPriceVendorPeriod.Name + "." + fieldName;
                else if (fieldName == VendorID.Name)
                    m_strReturn = m_objDItemPriceVendorPeriod.Name + "." + fieldName;
                else if (fieldName == VendorDesc.Name)
                    m_strReturn = string.Format("({0} +' '+ {1})", m_objMVendor.Name + "." + VendorVM.Prop.FirstName.Name, m_objMVendor.Name + "." + VendorVM.Prop.LastName.Name, m_objMVendor.Name);
                else if (fieldName == ValidFrom.Name)
                    m_strReturn = m_objDItemPriceVendorPeriod.Name + "." + fieldName;
                else if (fieldName == ValidTo.Name)
                    m_strReturn = m_objDItemPriceVendorPeriod.Name + "." + fieldName;
                else if (fieldName == Amount.Name)
                    m_strReturn = m_objDItemPriceVendorPeriod.Name + "." + fieldName;
                else if (fieldName == CurrencyID.Name)
                    m_strReturn = m_objDItemPriceVendorPeriod.Name + "." + fieldName;
                else if (fieldName == TaskID.Name)
                    m_strReturn = m_objDItemPriceVendorPeriod.Name + "." + fieldName;
                else if (fieldName == CurrencyDesc.Name)
                    m_strReturn = m_objMCurrency.Name + "." + fieldName;
                else if (fieldName == ItemID.Name)
                    m_strReturn = m_objDItemPrice.Name + "." + fieldName;
                else if (fieldName == ItemDesc.Name)
                    m_strReturn = m_objMitem.Name + "." + fieldName;
                else if (fieldName == RegionID.Name)
                    m_strReturn = m_objDItemPrice.Name + "." + fieldName;
                else if (fieldName == ProjectID.Name)
                    m_strReturn = m_objDItemPrice.Name + "." + fieldName;
                else if (fieldName == ClusterID.Name)
                    m_strReturn = m_objDItemPrice.Name + "." + fieldName;
                else if (fieldName == UnitTypeID.Name)
                    m_strReturn = m_objDItemPrice.Name + "." + fieldName;
                else if (fieldName == IsDefault.Name)
                    m_strReturn = m_objDItemPriceVendor.Name + "." + fieldName;
                else if (fieldName == ItemGroupDesc.Name)
                    m_strReturn = m_objMItemGroup.Name + "." + ItemGroupDesc.Name;
                else if (fieldName == ItemTypeID.Name)
                    m_strReturn = m_objMItemGroup.Name + "." + ItemTypeID.Name;

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