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
    public class ItemVersionVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string ItemID { get; set; }
        [Key, Column(Order = 2)]
        public int Version { get; set; }
        public string ChildItemID { get; set; }        
        public int ChildVersion { get; set; }
        public string VersionDesc { get; set; }
        public string ItemDesc { get; set; }
        public string ItemTypeDesc { get; set; }
        public string ItemTypeID { get; set; }
        public string ItemTypeParentID { get; set; }
        public string UoMDesc { get; set; }
        public string ItemGroupID { get; set; }
        public string ItemGroupDesc { get; set; }
        public string RegionID { get; set; }
        public string RegionDesc { get; set; }
        public string ProjectID { get; set; }
        public string ProjectDesc { get; set; }
        public string ClusterID { get; set; }
        public string ClusterDesc { get; set; }
        public string UnitTypeID { get; set; }
        public string UnitTypeDesc { get; set; }
        public Ext.Net.Node Structure { get; set; }

        public ItemVersionChildVM StructureModel { get; set; }
        public List<ItemVersionChildVM> ListItemVersionChildVM { get; set; }

        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class ItemID
            {
                public static string Desc { get { return "Item"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ChildItemID
            {
                public static string Desc { get { return "Item"; } }
                
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
            public static class UoMDesc
            {
                public static string Desc { get { return "UoM"; } }
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
            public static class Version
            {
                public static string Desc { get { return "Version"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ChildVersion
            {
                public static string Desc { get { return "Child Version"; } }
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
                public static string Desc { get { return "Item Type"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class ItemTypeParentID
            {
                public static string Desc { get { return "Item Type Parent ID"; } }
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
                DItemVersion m_objMItemVersion = new DItemVersion();
                MItemType m_objSItemType = new MItemType();
                MItemGroup m_objSItemGroup = new MItemGroup();
                MItem m_objMItem = new MItem();
                MUoM m_objMUoM = new MUoM();

                if (fieldName == ItemID.Name)
                    m_strReturn = m_objMItemVersion.Name + "." + fieldName;
                else if (fieldName == ItemDesc.Name)
                    m_strReturn = m_objMItem.Name + "." + fieldName;
                else if (fieldName == VersionDesc.Name)
                    m_strReturn = m_objMItemVersion.Name + "." + fieldName;
                else if (fieldName == Version.Name)
                    m_strReturn = m_objMItemVersion.Name + "." + fieldName;
                else if (fieldName == ItemGroupDesc.Name)
                    m_strReturn = m_objSItemGroup.Name + "." + fieldName;
                else if (fieldName == ItemTypeDesc.Name)
                    m_strReturn = m_objSItemType.Name + "." + fieldName;
                else if (fieldName == ItemTypeID.Name)
                    m_strReturn = m_objSItemType.Name + "." + fieldName;
                else if (fieldName == ItemTypeParentID.Name)
                    m_strReturn = m_objSItemType.Name + "." + fieldName;
                else if (fieldName == UoMDesc.Name)
                    m_strReturn = m_objMUoM.Name + "." + fieldName;

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