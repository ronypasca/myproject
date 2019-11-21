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
    public class ItemUploadVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string ItemID { get; set; }
        public string ItemDesc { get; set; }
        public string ItemGroupID { get; set; }
        public string ItemGroupDesc { get; set; }
        public string ItemTypeID { get; set; }
        public string ItemTypeDesc { get; set; }
        public string UoMID { get; set; }
        public string UoMDesc { get; set; }
        public bool IsActive { get; set; }
        public bool HasParameter { get; set; }
        public bool HasPrice { get; set; }
        public string ItemVersionChildID { get; set; }
        public int Version { get; set; }
        public bool HasUnitPriceAnalysis
        {
            get
            {
                return !string.IsNullOrEmpty(ItemVersionChildID);
            }
        }
        public List<ItemParameterVM> ListItemParameterVM { get; set; }
        public List<ItemPriceVM> ListItemPriceVM { get; set; }
        public List<ItemShowCaseVM> ListItemImage { get; set; }

        public string ParameterID { get; set; }
        public string Value { get; set; }
        public List<ItemDetailVM> ListItemDetailVM { get; set; }
        public string TaskID { get; set; }
        #endregion


        #region Public Field Property

        public static class Prop
        {
            public static class ItemID
            {
                public static string Desc { get { return "ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class ItemDesc
            {
                public static string Desc { get { return "Description"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class ItemGroupID
            {
                public static string Desc { get { return "Item Group ID"; } }
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

            public static class ItemTypeID
            {
                public static string Desc { get { return "Item Type ID"; } }
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
            public static class UoMID
            {
                public static string Desc { get { return "UoM ID"; } }
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
            public static class IsActive
            {
                public static string Desc { get { return "Active"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }

            }
            public static class HasParameter
            {
                public static string Desc { get { return "Has Paramter"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }

            }
            public static class HasPrice
            {
                public static string Desc { get { return "Has Price"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }

            }

            public static class ItemVersionChildID
            {
                public static string Desc { get { return "Item Version Child ID"; } }
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

            public static class ListItemParameterVM
            {
                public static string Desc { get { return "List Paramter"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }

            }
            public static class ListItemPriceVM
            {
                public static string Desc { get { return "List Price"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }

            }

            public static class TaskID
            {
                public static string Desc { get { return "TaskID"; } }
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
                MItemUpload m_objMItem = new MItemUpload();
                MItemGroup m_objMItemGroup = new MItemGroup();
                MItemType m_objMItemType = new MItemType();
                MUoM m_objMUoM = new MUoM();
                DItemVersion m_objDItemVersion = new DItemVersion();
                DItemVersionChild m_objDItemVersionChild = new DItemVersionChild();

                if (fieldName == ItemID.Name)
                    m_strReturn = m_objMItem.Name + "." + fieldName;
                else if (fieldName == ItemDesc.Name)
                    m_strReturn = m_objMItem.Name + "." + fieldName;
                else if (fieldName == ItemGroupID.Name)
                    m_strReturn = m_objMItem.Name + "." + fieldName;
                else if (fieldName == ItemGroupDesc.Name)
                    m_strReturn = m_objMItemGroup.Name + "." + fieldName;
                else if (fieldName == ItemTypeID.Name)
                    m_strReturn = m_objMItemGroup.Name + "." + fieldName;
                else if (fieldName == ItemTypeDesc.Name)
                    m_strReturn = m_objMItemType.Name + "." + fieldName;
                else if (fieldName == UoMID.Name)
                    m_strReturn = m_objMItem.Name + "." + fieldName;
                else if (fieldName == UoMDesc.Name)
                    m_strReturn = m_objMUoM.Name + "." + fieldName;
                else if (fieldName == IsActive.Name)
                    m_strReturn = m_objMItem.Name + "." + fieldName;
                else if (fieldName == HasParameter.Name)
                    m_strReturn = m_objMItemGroup.Name + "." + fieldName;
                else if (fieldName == HasPrice.Name)
                    m_strReturn = m_objMItemGroup.Name + "." + fieldName;
                else if (fieldName == Version.Name)
                    m_strReturn = m_objDItemVersion.Name + "." + fieldName;
                else if (fieldName == ItemVersionChildID.Name)
                    m_strReturn = m_objDItemVersionChild.Name + "." + fieldName;
                else if (fieldName == TaskID.Name)
                    m_strReturn = m_objMItem.Name + "." + fieldName;

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