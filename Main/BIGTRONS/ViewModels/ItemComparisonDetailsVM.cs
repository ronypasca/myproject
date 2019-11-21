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
    public class ItemComparisonDetailsVM
    {
        #region Public Property
        [Key, Column(Order = 1)]
        public string ItemComparisonDetailID { get; set; }
        public string ItemComparisonID { get; set; }
        public string ItemPriceID { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyID { get; set; }
        public string TaskID { get; set; }
        public string VendorID { get; set; }
        public string VendorDesc { get; set; }
        //Vendor
        public string FirstName { get; set; }
        public string LastName { get; set; }
        //Currency
        public string CurrencyDesc { get; set; }
        //ItemPrice
        public string ItemID { get; set; }
        public string RegionID { get; set; }
        public string ProjectID { get; set; }
        public string ClusterID { get; set; }
        public string UnitTypeID { get; set; }
        public bool IsDefault { get; set; }
        //Item
        public string ItemDesc { get; set; }
        //ItemGroup
        public bool ItemGroupDesc { get; set; }
        public bool ItemTypeID { get; set; }
        // Add some
        public bool AllowDelete { get; set; }
        public bool HasChild { get; set; }
        //public string ItemPriceVendorPeriodID { get; set; }
        public List<ItemPriceVendorPeriodVM> ListItemPriceVendorPeriod { get; set; }
        
        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class AllowDelete
            {
                public static string Desc { get { return "Allow Delete"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class HasChild
            {
                public static string Desc { get { return "Has Child"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class VendorDesc
            {
                public static string Desc { get { return "Vendor Description"; } }
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
            public static class ItemGroupDesc
            {
                public static string Desc { get { return "Item Group Description"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ItemDesc
            {
                public static string Desc { get { return "Item Description"; } }
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
            public static class UnitTypeID
            {
                public static string Desc { get { return "UnitType ID"; } }
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
            public static class ProjectID
            {
                public static string Desc { get { return "ProjectID"; } }
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
            public static class ItemID
            {
                public static string Desc { get { return "Item ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class CurrencyDesc
            {
                public static string Desc { get { return "Currency Description"; } }
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
            public static class ListItemPriceVendorPeriod
            {
                public static string Desc { get { return "ListItemPriceVendorPeriod"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ItemComparisonDetailID
            {
                public static string Desc { get { return "Item Comparison Detail ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ItemComparisonID
            {
                public static string Desc { get { return "Item Comparison ID"; } }
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
            public static class Amount
            {
                public static string Desc { get { return "Amount"; } }
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
            public static class TaskID
            {
                public static string Desc { get { return "Task ID"; } }
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
            public static class ListItemPriceVendorPeriodVM
            {
                public static string Desc { get { return "List Item Price Vendor Period"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static string Map(string fieldName, bool withAlias = false)
            {
                string m_strReturn = string.Empty;
                DItemComparisonDetails m_objDItemComparison = new DItemComparisonDetails();
                DItemPriceVendorPeriod m_objDItemPriceVendorPeriod = new DItemPriceVendorPeriod();
                DItemPriceVendor m_objDItemPriceVendor = new DItemPriceVendor();
                MVendor m_objMVendor = new MVendor();
                MCurrency m_objMCurrency = new MCurrency();
                DItemPrice m_objDItemPrice = new DItemPrice();
                MItem m_objMitem = new MItem();
                MItemGroup m_objMItemGroup = new MItemGroup();

                if (fieldName == ItemComparisonID.Name)
                    m_strReturn = m_objDItemComparison.Name + "." + fieldName;
                else if (fieldName == ItemComparisonDetailID.Name)
                    m_strReturn = m_objDItemComparison.Name + "." + fieldName;
                else if (fieldName == ItemPriceID.Name)
                    m_strReturn = m_objDItemPriceVendorPeriod.Name + "." + fieldName;
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
                else if (fieldName == VendorID.Name)
                    m_strReturn = m_objDItemComparison.Name + "." + fieldName;
                else if (fieldName == VendorDesc.Name)
                    m_strReturn = string.Format("({0} +' '+ {1})", m_objMVendor.Name + "." + VendorVM.Prop.FirstName.Name, m_objMVendor.Name + "." + VendorVM.Prop.LastName.Name, m_objMVendor.Name);
                else if (fieldName == CurrencyDesc.Name)
                    m_strReturn = m_objMCurrency.Name + "." + fieldName;
                else if (fieldName == ItemID.Name)
                    m_strReturn = m_objDItemPrice.Name + "." + fieldName;
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
                else if (fieldName == ItemDesc.Name)
                    m_strReturn = m_objMitem.Name + "." + fieldName;
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