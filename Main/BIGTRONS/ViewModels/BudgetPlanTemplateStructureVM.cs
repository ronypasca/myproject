using com.SML.Lib.Common;
using com.SML.BIGTRONS.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace com.SML.BIGTRONS.ViewModels
{
    public class BudgetPlanTemplateStructureVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string BudgetPlanTemplateID { get; set; }
        [Key, Column(Order = 2)]
        public string ItemID { get; set; }
        [Key, Column(Order = 3)]
        public int Version { get; set; }
        [Key, Column(Order = 4)]
        public int Sequence { get; set; }
        public string ParentItemID { get; set; }
        public string ParentItemTypeID { get; set; }
        public int ParentVersion { get; set; }
        public int ParentSequence { get; set; }
        public bool AllowDelete { get; set; }
        public bool HasChild { get; set; }
        public string ItemGroupDesc { get; set; }
        public string ItemTypeID { get; set; }
        public string ItemDesc { get; set; }
        public Nullable<bool> IsDefault { get; set; }
        public bool EnableDefault { get; set; }
        public string ItemVersionChildID { get; set; }
        public string UoMID { get; set; }
        public string UoMDesc { get; set; }
        public bool IsBOI { get; set; }
        public bool IsAHS { get; set; }
        public decimal? MaterialAmount { get; set; }
        public decimal? WageAmount { get; set; }
        public decimal? MiscAmount { get; set; }
        public decimal? Coefficient { get; set; }
        public string SequenceDesc { get; set; }
        public string Formula { get; set; }
        public string ItemGroupID { get; set; }

        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class BudgetPlanTemplateID
            {
                public static string Desc { get { return "ID"; } }
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

            public static class ParentItemID
            {
                public static string Desc { get { return "ParentItem ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class ParentVersion
            {
                public static string Desc { get { return "Parent Version"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class ParentSequence
            {
                public static string Desc { get { return "Parent Sequence"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
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
            public static class ItemGroupDesc
            {
                public static string Desc { get { return "ItemGroup Desc"; } }
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
            public static class ItemDesc
            {
                public static string Desc { get { return "Item"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ParentItemTypeID
            {
                public static string Desc { get { return "ParentItemType ID"; } }
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
            public static class EnableDefault
            {
                public static string Desc { get { return "EnableDefault"; } }
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
            public static class ItemVersionChildID
            {
                public static string Desc { get { return "ItemVersionChildID"; } }
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

            public static class SequenceDesc
            {
                public static string Desc { get { return "No"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class Formula
            {
                public static string Desc { get { return "Formula"; } }
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
                DBudgetPlanTemplateStructure m_objDbudgetPlanTemplateStructure = new DBudgetPlanTemplateStructure();
                MBudgetPlanTemplate m_objMBudgetPlanTemplate = new MBudgetPlanTemplate();
                MItemGroup m_objMItemGroup = new MItemGroup();
                MItem m_objMitem = new MItem();

                DItemVersionChild m_objDItemVersionChild = new DItemVersionChild();
                DItemVersionChildFormula m_objDItemVersionChildFormula = new DItemVersionChildFormula();
                MUoM m_objMUoM = new MUoM();

                if (fieldName == BudgetPlanTemplateID.Name)
                    m_strReturn = m_objDbudgetPlanTemplateStructure.Name + "." + BudgetPlanTemplateID.Name;
                else if (fieldName == ItemID.Name)
                    m_strReturn = m_objDbudgetPlanTemplateStructure.Name + "." + ItemID.Name;
                else if (fieldName == Version.Name)
                    m_strReturn = m_objDbudgetPlanTemplateStructure.Name + "." + ItemVersionVM.Prop.Version.Name;
                else if (fieldName == Sequence.Name)
                    m_strReturn = m_objDbudgetPlanTemplateStructure.Name + "." + fieldName;
                else if (fieldName == ParentItemID.Name)
                    m_strReturn = m_objDbudgetPlanTemplateStructure.Name + "." + ParentItemID.Name;
                else if (fieldName == ParentVersion.Name)
                    m_strReturn = m_objDbudgetPlanTemplateStructure.Name + "." + ParentVersion.Name;
                else if (fieldName == ParentSequence.Name)
                    m_strReturn = m_objDbudgetPlanTemplateStructure.Name + "." + ParentSequence.Name;
                else if (fieldName == Coefficient.Name)
                    m_strReturn = m_objDItemVersionChildFormula.Name + "." + Coefficient.Name;
                else if (fieldName == ItemGroupDesc.Name)
                    m_strReturn = m_objMItemGroup.Name + "." + ItemGroupDesc.Name;
                else if (fieldName == ItemGroupID.Name)
                    m_strReturn = m_objMItemGroup.Name + "." + ItemGroupID.Name;
                else if (fieldName == ItemTypeID.Name)
                    m_strReturn = m_objMItemGroup.Name + "." + ItemTypeID.Name;
                else if (fieldName == ItemDesc.Name)
                    m_strReturn = m_objMitem.Name + "." + ItemDesc.Name;
                else if (fieldName == ParentItemTypeID.Name)
                    m_strReturn = "ParentItemGroup" + "." + ParentItemTypeID.Name;
                else if (fieldName == IsDefault.Name)
                    m_strReturn = m_objDbudgetPlanTemplateStructure.Name + "." + fieldName;
                else if (fieldName == ItemVersionChildID.Name)
                    m_strReturn = m_objDItemVersionChild.Name + "." + fieldName;
                else if (fieldName == UoMDesc.Name)
                    m_strReturn = m_objMUoM.Name + "." + fieldName;
                else if (fieldName == UoMID.Name)
                    m_strReturn = m_objMUoM.Name + "." + fieldName;
                //else if (fieldName == AllowDelete.Name)
                //m_strReturn = m_objDbudgetPlanTemplateStructure.Name + "." + fieldName;

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
            PropertyInfo m_pifBudgetPlanTemplateStructureVM = this.GetType().GetProperty(fieldName);
            return m_pifBudgetPlanTemplateStructureVM != null && Attribute.GetCustomAttribute(m_pifBudgetPlanTemplateStructureVM, typeof(KeyAttribute)) != null;
        }
    }
}