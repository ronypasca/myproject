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
    public class ItemVersionChildFormulaVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string ItemVersionChildID { get; set; }
        public string ItemID { get; set; }
        public string ItemDesc { get; set; }
        public string VersionDesc { get; set; }
        public string ItemTypeID { get; set; }
        public string ItemTypeParentID { get; set; }

        public string ItemTypeDesc { get; set; }
        public string ItemGroupDesc { get; set; }
        public string UoMDesc { get; set; }
        public int Version { get; set; }
        public string ChildItemID { get; set; }
        public string ChildItemDesc { get; set; }
        public int ChildVersion { get; set; }
        public decimal Coefficient { get; set; }
        public string Formula { get; set; }
        public string FormulaDesc { get; set; }
        public string NodeID { get; set; }
        public int Sequence { get; set; }
        public string SequenceDesc { get; set; }
        public Ext.Net.Node FormulaItem { get; set; }
        public Ext.Net.Node ChildStructure { get; set; }
        public List<ItemVersionVM> AlternativeItem { get; set; }
        public string AlternativerItemStr { get; set; }
        public ConfigAHSChildVM FormulaBehavior { get; set; }

        public bool IsBOI { get; set; }
        public bool IsAHS { get; set; }
        public bool HasChild { get; set; }
        public decimal? MaterialAmount { get; set; }
        public decimal? WageAmount { get; set; }
        public decimal? MiscAmount { get; set; }
        public string ChildItemTypeID { get; set; }
        public bool? IsDefault { get; set; }
        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class AlternativeItem
            {
                public static string Desc { get { return "AlternativeItem"; } }
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
            public static class ItemID
            {
                public static string Desc { get { return "ID"; } }
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
            public static class Version
            {
                public static string Desc { get { return "Version"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class VersionDesc
            {
                public static string Desc { get { return "Description"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ItemTypeDesc
            {
                public static string Desc { get { return "Item Type"; } }
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
            public static class ItemTypeParentID
            {
                public static string Desc { get { return "ItemTypeParentID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ItemGroupDesc
            {
                public static string Desc { get { return "Item Group"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ChildItemID
            {
                public static string Desc { get { return "Child Item ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class ChildItemDesc
            {
                public static string Desc { get { return "Child Item Desc"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ChildVersion
            {
                public static string Desc { get { return "Version"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class UoMID
            {
                public static string Desc { get { return "UoMID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class UoMDesc
            {
                public static string Desc { get { return "UoM"; } }
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
            public static class Formula
            {
                public static string Desc { get { return "Formula"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class FormulaDesc
            {
                public static string Desc { get { return "Formula"; } }
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
                public static string Desc { get { return "Sequence"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ChildItemTypeID
            {
                public static string Desc { get { return "ChlildItemTypeID"; } }
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
                MItem m_objMItem = new MItem();
                MItemGroup m_objMItemGroup = new MItemGroup();
                MItemType m_objMItemType = new MItemType();
                DItemVersion m_objDItemVersion = new DItemVersion();
                DItemVersionChild m_objDItemVersionChild = new DItemVersionChild();
                DItemVersionChildFormula m_objDItemVersionChildFormula = new DItemVersionChildFormula();
                DItemVersionChildAlt m_objDItemVersionChildAlt = new DItemVersionChildAlt();
                MUoM m_objMUoM = new MUoM();

                if (fieldName == ItemVersionChildID.Name)
                    m_strReturn = m_objDItemVersionChildFormula.Name + "." + fieldName;
                else if (fieldName == ItemID.Name)
                    m_strReturn = m_objDItemVersionChild.Name + "." + fieldName;
                else if (fieldName == ItemDesc.Name)
                    m_strReturn = m_objMItem.Name + "." + fieldName;
                else if (fieldName == VersionDesc.Name)
                    m_strReturn = m_objDItemVersion.Name + "." + fieldName;
                else if (fieldName == Version.Name)
                    m_strReturn = m_objDItemVersionChild.Name + "." + fieldName;
                else if (fieldName == ChildItemID.Name)
                    m_strReturn = m_objDItemVersionChild.Name + "." + fieldName;
                else if (fieldName == ChildItemDesc.Name)
                    m_strReturn = m_objMItem.Name + "." + ItemVM.Prop.ItemDesc.Name;
                else if (fieldName == ChildVersion.Name)
                    m_strReturn = m_objDItemVersionChild.Name + "." + fieldName;
                else if (fieldName == Formula.Name)
                    m_strReturn = m_objDItemVersionChildFormula.Name + "." + fieldName;
                else if (fieldName == UoMDesc.Name)
                    m_strReturn = m_objMUoM.Name + "." + fieldName;
                else if (fieldName == Coefficient.Name)
                    m_strReturn = m_objDItemVersionChildFormula.Name + "." + fieldName;
                else if (fieldName == SequenceDesc.Name)
                    m_strReturn = m_objDItemVersionChild.Name + "." + fieldName;
                else if (fieldName == Sequence.Name)
                    m_strReturn = m_objDItemVersionChild.Name + "." + fieldName;
                else if (fieldName == ItemTypeID.Name)
                    m_strReturn = m_objMItemGroup.Name + "." + fieldName;
                else if (fieldName == ItemTypeParentID.Name)
                    m_strReturn = m_objMItemType.Name + "." + fieldName;
                else if (fieldName == ItemTypeDesc.Name)
                    m_strReturn = m_objMItemType.Name + "." + fieldName;
                else if (fieldName == ItemGroupDesc.Name)
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
            PropertyInfo m_pifItemVersionVM = this.GetType().GetProperty(fieldName);
            return m_pifItemVersionVM != null && Attribute.GetCustomAttribute(m_pifItemVersionVM, typeof(KeyAttribute)) != null;
        }
    }
}