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
    public class ItemDetailVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string ItemDetailID { get; set; }
        public string ItemDetailDesc { get; set; }
        public string ItemDetailInfo { get; set; }
        public string ItemDetailTypeID { get; set; }
        public string ItemDetailTypeDesc { get; set; }
        public bool HasParameter { get; set; }
        public bool HasPrice { get; set; }
        public string ItemID { get; set; }
        public string VendorID { get; set; }
        public string TaskID { get; set; }
        public bool Visible { get; set; }
        public string ItemDesc { get; set; }
        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class ItemDetailID
            {
                public static string Desc { get { return "ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class ItemDetailDesc
            {
                public static string Desc { get { return "Description"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class ItemDetailInfo
            {
                public static string Desc { get { return "Detail Info"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class ItemDetailTypeID
            {
                public static string Desc { get { return "ItemDetailType ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class ItemDetailTypeDesc
            {
                public static string Desc { get { return "ItemDetailType Description"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ListItemDetailParameterVM
            {
                public static string Desc { get { return "List ItemDetail Parameter"; } }
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

            public static class ItemID
            {
                public static string Desc { get { return "ItemID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }

            }
            public static class VendorID
            {
                public static string Desc { get { return "VendorID"; } }
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
            public static class Visible
            {
                public static string Desc { get { return "Visible"; } }
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
                DItemDetails m_objDItemDetails = new DItemDetails();
                MItemDetailTypes m_objMItemDetailTypes = new MItemDetailTypes();

                if (fieldName == ItemDetailID.Name)
                    m_strReturn = m_objDItemDetails.Name + "." + fieldName;
                else if (fieldName == ItemDetailDesc.Name)
                    m_strReturn = m_objDItemDetails.Name + "." + fieldName;
                else if (fieldName == ItemDetailTypeID.Name)
                    m_strReturn = m_objDItemDetails.Name + "." + ItemDetailTypeID.Name;
                else if (fieldName == ItemDetailTypeDesc.Name)
                    m_strReturn = m_objMItemDetailTypes.Name + "." + fieldName;
                else if (fieldName == VendorID.Name)
                    m_strReturn = m_objDItemDetails.Name + "." + fieldName;
                else if (fieldName == ItemID.Name)
                    m_strReturn = m_objDItemDetails.Name + "." + fieldName;
                else if (fieldName == TaskID.Name)
                    m_strReturn = m_objDItemDetails.Name + "." + fieldName;
                else if (fieldName == Visible.Name)
                    m_strReturn = m_objDItemDetails.Name + "." + fieldName;
                //else if (fieldName == HasPrice.Name)
                //    m_strReturn = m_objMItemDetail.Name + "." + fieldName;

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