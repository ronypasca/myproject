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
    public class BudgetPlanVersionStructureVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string BudgetPlanVersionStructureID { get; set; }
        public string BudgetPlanID { get; set; }
        public int BudgetPlanVersion { get; set; }
        public string ItemID { get; set; }
        public int Version { get; set; }
        public int Sequence { get; set; }
        public string SequenceDesc { get; set; }
        public string ParentItemID { get; set; }
        public int ParentVersion { get; set; }
        public int ParentSequence { get; set; }
        public string ItemVersionChildID { get; set; }
        public decimal? Volume { get; set; }
        public decimal? MaterialAmount { get; set; }
        public decimal? WageAmount { get; set; }
        public decimal? MiscAmount { get; set; }
        public decimal? VendorMaterialAmount { get; set; }
        public decimal? VendorWageAmount { get; set; }
        public decimal? VendorMiscAmount { get; set; }
        public decimal? VendorVolume { get; set; }
        public string ItemDesc { get; set; }
        public string ParentItemDesc { get; set; }
        public string UoMID { get; set; }
        public string UoMDesc { get; set; }
        public string Specification { get; set; }
        public decimal TotalUnitPrice { get { return (decimal)((MaterialAmount ?? 0) + (WageAmount ?? 0) + (MiscAmount ?? 0)); } }
        public decimal Total { get { return (decimal)((Volume ?? 0) * TotalUnitPrice); } }

        public string BudgetPlanTemplateID { get; set; }
        public string BudgetPlanTemplateDesc { get; set; }
        public bool HasChild { get; set; }
        public string ItemTypeID { get; set; }
        public string ParentItemTypeID { get; set; }
        public bool? IsDefault { get; set; }

        public bool IsBOI { get; set; }
        public bool IsAHS { get; set; }

        public string ChildItemID { get; set; }
        public int ChildVersion { get; set; }
        public string BudgetPlanVersionVendorID { get; set; }
        public string BidBudgetPlanVersionStructureID { get; set; }
        public decimal BidMAT { get; set; }
        public decimal BidWAG { get; set; }
        public decimal BidMISC { get; set; }
        public decimal BidVolume { get; set; }
        public decimal Amount
        {
            get
            {
                return (
                        MiscAmount == null ? 0 : (decimal)MiscAmount
                        + WageAmount == null ? 0 : (decimal)WageAmount
                        + MaterialAmount == null ? 0 : (decimal)MaterialAmount
                        )
                        * Volume == null ? 0 : (decimal)Volume;
            }
        }
        public decimal? Coefficient { get; set; }
        public string VendorID { get; set; }
        public string VendorName { get; set; }
        public string Info { get; set; }
        public string ItemGroupID { get; set; }
        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class BudgetPlanVersionStructureID
            {
                public static string Desc { get { return "Budget Plan Version Structure ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BudgetPlanID
            {
                public static string Desc { get { return "Budget Plan ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BudgetPlanVersion
            {
                public static string Desc { get { return "Budget Plan Version"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ItemID
            {
                public static string Desc { get { return "ItemID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Version
            {
                public static string Desc { get { return "Version"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Sequence
            {
                public static string Desc { get { return "Sequence"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class SequenceDesc
            {
                public static string Desc { get { return "Sequence Desc"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ParentItemID
            {
                public static string Desc { get { return "ParentItemID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ParentVersion
            {
                public static string Desc { get { return "ParentVersion"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ParentSequence
            {
                public static string Desc { get { return "ParentSequence"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ItemVersionChildID
            {
                public static string Desc { get { return "ItemVersionChildID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Volume
            {
                public static string Desc { get { return "Volume"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class MaterialAmount
            {
                public static string Desc { get { return "Material"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class WageAmount
            {
                public static string Desc { get { return "Wage"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class MiscAmount
            {
                public static string Desc { get { return "Other"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class VendorMaterialAmount
            {
                public static string Desc { get { return "Material"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class VendorWageAmount
            {
                public static string Desc { get { return "Wage"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class VendorMiscAmount
            {
                public static string Desc { get { return "Other"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class VendorVolume
            {
                public static string Desc { get { return "Volume"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ItemDesc
            {
                public static string Desc { get { return "Structure"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ParentItemDesc
            {
                public static string Desc { get { return "ParentItemDesc"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class UoMID
            {
                public static string Desc { get { return "UoM"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class UoMDesc
            {
                public static string Desc { get { return "UoM Desc"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Specification
            {
                public static string Desc { get { return "Specification"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class TotalUnitPrice
            {
                public static string Desc { get { return "Total Unit Price"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Total
            {
                public static string Desc { get { return "Total"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class BudgetPlanTemplateID
            {
                public static string Desc { get { return "BudgetPlanTemplateID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BudgetPlanTemplateDesc
            {
                public static string Desc { get { return "BudgetPlanTemplateDesc"; } }
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
            public static class ItemTypeID
            {
                public static string Desc { get { return "ItemType ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ParentItemTypeID
            {
                public static string Desc { get { return "Parent ItemType ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class IsDefault
            {
                public static string Desc { get { return "IsDefault"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class EnableDefault
            {
                public static string Desc { get { return "EnableDefault"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class IsBOI
            {
                public static string Desc { get { return "IsBOI"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class IsAHS
            {
                public static string Desc { get { return "IsAHS"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BudgetPlanVersionVendorID
            {
                public static string Desc { get { return "Budget Plan Version VendorID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class VendorID
            {
                public static string Desc { get { return "Assigned Vendor ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class VendorName
            {
                public static string Desc { get { return "Assigned Vendor Name"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class Coefficient
            {
                public static string Desc { get { return "Coefficient"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ItemGroupID
            {
                public static string Desc { get { return "ItemGroupID"; } }
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
                DBudgetPlanVersionStructure m_objDBudgetPlanVersionStructure = new DBudgetPlanVersionStructure();
                TBudgetPlan m_objTBudgetPlan = new TBudgetPlan();
                MItem m_objMItem = new MItem();
                MUoM m_objMUoM = new MUoM();
                MItemGroup m_objMItemGroup = new MItemGroup();
                MVendor m_objMVendor = new MVendor();
                MBudgetPlanTemplate m_objMBudgetPlanTemplate = new MBudgetPlanTemplate();
                DBudgetPlanVersionEntry m_objDBudgetPlanVersionEntry = new DBudgetPlanVersionEntry();
                DBudgetPlanVersionAssignment m_objDBudgetPlanVersionAssignment = new DBudgetPlanVersionAssignment();

                if (fieldName == BudgetPlanVersionStructureID.Name)
                    m_strReturn = m_objDBudgetPlanVersionStructure.Name + "." + fieldName;
                else if (fieldName == BudgetPlanID.Name)
                    m_strReturn = m_objDBudgetPlanVersionStructure.Name + "." + fieldName;
                else if (fieldName == BudgetPlanTemplateID.Name)
                    m_strReturn = m_objTBudgetPlan.Name + "." + fieldName;
                else if (fieldName == BudgetPlanVersion.Name)
                    m_strReturn = m_objDBudgetPlanVersionStructure.Name + "." + fieldName;
                else if (fieldName == ItemID.Name)
                    m_strReturn = m_objDBudgetPlanVersionStructure.Name + "." + fieldName;
                else if (fieldName == Version.Name)
                    m_strReturn = m_objDBudgetPlanVersionStructure.Name + "." + fieldName;
                else if (fieldName == Sequence.Name)
                    m_strReturn = m_objDBudgetPlanVersionStructure.Name + "." + fieldName;
                else if (fieldName == ParentItemID.Name)
                    m_strReturn = m_objDBudgetPlanVersionStructure.Name + "." + fieldName;
                else if (fieldName == ParentVersion.Name)
                    m_strReturn = m_objDBudgetPlanVersionStructure.Name + "." + fieldName;
                else if (fieldName == ParentSequence.Name)
                    m_strReturn = m_objDBudgetPlanVersionStructure.Name + "." + fieldName;
                else if (fieldName == BudgetPlanTemplateDesc.Name)
                    m_strReturn = m_objMBudgetPlanTemplate.Name + "." + fieldName;
                else if (fieldName == Volume.Name)
                    m_strReturn = m_objDBudgetPlanVersionStructure.Name + "." + fieldName;
                else if (fieldName == ItemVersionChildID.Name)
                    m_strReturn = m_objDBudgetPlanVersionStructure.Name + "." + fieldName;
                else if (fieldName == MaterialAmount.Name)
                    m_strReturn = m_objDBudgetPlanVersionStructure.Name + "." + fieldName;
                else if (fieldName == WageAmount.Name)
                    m_strReturn = m_objDBudgetPlanVersionStructure.Name + "." + fieldName;
                else if (fieldName == MiscAmount.Name)
                    m_strReturn = m_objDBudgetPlanVersionStructure.Name + "." + fieldName;
                else if (fieldName == VendorMaterialAmount.Name)
                    m_strReturn = m_objDBudgetPlanVersionEntry.Name + "." + MaterialAmount.Name;
                else if (fieldName == VendorWageAmount.Name)
                    m_strReturn = m_objDBudgetPlanVersionEntry.Name + "." + WageAmount.Name;
                else if (fieldName == VendorMiscAmount.Name)
                    m_strReturn = m_objDBudgetPlanVersionEntry.Name + "." + MiscAmount.Name;
                else if (fieldName == VendorVolume.Name)
                    m_strReturn = m_objDBudgetPlanVersionEntry.Name + "." + Volume.Name;
                else if (fieldName == Specification.Name)
                    m_strReturn = m_objDBudgetPlanVersionStructure.Name + "." + fieldName;
                else if (fieldName == ItemDesc.Name)
                    m_strReturn = m_objMItem.Name + "." + fieldName;
                else if (fieldName == ParentItemDesc.Name)
                    m_strReturn = m_objMItem.Name + "." + ItemVM.Prop.ItemDesc.Name;
                else if (fieldName == UoMID.Name)
                    m_strReturn = m_objMUoM.Name + "." + fieldName;
                else if (fieldName == UoMDesc.Name)
                    m_strReturn = m_objMUoM.Name + "." + fieldName;
                else if (fieldName == ItemTypeID.Name)
                    m_strReturn = m_objMItemGroup.Name + "." + fieldName;
                else if (fieldName == BudgetPlanVersionVendorID.Name)
                    m_strReturn = m_objDBudgetPlanVersionEntry.Name + "." + fieldName;
                else if (fieldName == VendorID.Name)
                    m_strReturn = m_objDBudgetPlanVersionAssignment.Name + "." + fieldName;
                else if (fieldName == VendorName.Name)
                    m_strReturn = "(" + m_objMVendor.Name + ".FirstName + " + m_objMVendor.Name + ".LastName)";
                else if (fieldName == ItemGroupID.Name)
                    m_strReturn = m_objMItemGroup.Name + "." + fieldName;

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