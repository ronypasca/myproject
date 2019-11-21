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
    public class ItemGroupVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string ItemGroupID { get; set; }
        public string ItemGroupDesc { get; set; }
        public string ItemTypeID { get; set; }
        public string ItemTypeDesc { get; set; }
        private List<ItemGroupParameterVM> ListItemGroupParameterVM_ = new List<ItemGroupParameterVM>();
        //public List<ItemGroupParameterVM> ListItemGroupParameterVM {get{return this.ListItemGroupParameterVM_;} set{this.ListItemGroupParameterVM_ = value;}}
        public List<ItemGroupParameterVM> ListItemGroupParameterVM { get; set; }

        public bool HasParameter { get; set; }
        public bool HasPrice { get; set; }

        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class ItemGroupID
            {
                public static string Desc { get { return "ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class ItemGroupDesc
            {
                public static string Desc { get { return "Description"; } }
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

            public static class ItemTypeDesc
            {
                public static string Desc { get { return "ItemType Description"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ListItemGroupParameterVM
            {
                public static string Desc { get { return "List ItemGroup Parameter"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }


            }
            public static class HasParameter
            {
                public static string Desc { get { return "Has Parameter"; } }
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
                MItemGroup m_objMItemGroup = new MItemGroup();
                MItemType m_objMItemType = new MItemType();

                if (fieldName == ItemGroupID.Name)
                    m_strReturn = m_objMItemGroup.Name + "." + fieldName;
                else if (fieldName == ItemGroupDesc.Name)
                    m_strReturn = m_objMItemGroup.Name + "." + fieldName;
                else if (fieldName == ItemTypeID.Name)
                    m_strReturn = m_objMItemGroup.Name + "." + ItemTypeID.Name;
                else if (fieldName == ItemTypeDesc.Name)
                    m_strReturn = m_objMItemType.Name + "." + ItemTypeVM.Prop.ItemTypeDesc.Name;
                else if (fieldName == HasParameter.Name)
                    m_strReturn = m_objMItemGroup.Name + "." + fieldName;
                else if (fieldName == HasPrice.Name)
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
            PropertyInfo m_pifUoMVM = this.GetType().GetProperty(fieldName);
            return m_pifUoMVM != null && Attribute.GetCustomAttribute(m_pifUoMVM, typeof(KeyAttribute)) != null;
        }
    }
}