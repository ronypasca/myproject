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
    public class MenuVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string MenuID { get; set; }
        public string MenuHierarchy { get; set; }
        public string MenuDesc { get; set; }
        public string MenuIcon { get; set; }
        public string MenuUrl { get; set; }
        public bool MenuVisible { get; set; }
        public MenuVM MenuParentVM { get; set; }
        public string MenuParentHierarchy { get; set; }
        public MenuVM MenuLeftVM { get; set; }
        public string MenuLeftHierarchy { get; set; }
        public List<MenuActionVM> ListMenuActionVM { get; set; }
        public List<MenuObjectVM> ListMenuObjectVM { get; set; }
        public string MenuParent
        {
            get
            {
                return MenuHierarchy == null ? String.Empty : MenuHierarchy.Length > 2 ? MenuHierarchy.Substring(0, MenuHierarchy.Length - 2) : String.Empty;
            }
        }
        public string RoleID { get; set; }
        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class MenuID
            {
                public static string Desc { get { return "ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class MenuHierarchy
            {
                public static string Desc { get { return "Hierarchy"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class MenuDesc
            {
                public static string Desc { get { return "Description"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class MenuIcon
            {
                public static string Desc { get { return "Icon"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class MenuUrl
            {
                public static string Desc { get { return "Url"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class MenuVisible
            {
                public static string Desc { get { return "Visible"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class MenuParentVM
            {
                public static string Desc { get { return "Parent"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class MenuLeftHierarchy
            {
                public static string Desc { get { return "Left"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class MenuParentHierarchy
            {
                public static string Desc { get { return "Parent"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class MenuLeftVM
            {
                public static string Desc { get { return "Left"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ListMenuActionVM
            {
                public static string Desc { get { return "ListMenuActionVM"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class ListMenuObjectVM
            {
                public static string Desc { get { return "ListMenuObjectVM"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class RoleID
            {
                public static string Desc { get { return "Role ID"; } }
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
                SMenu m_objSMenu = new SMenu();
                DRoleMenuAction m_objDRoleMenuAction = new DRoleMenuAction();

                if (fieldName == MenuID.Name)
                    m_strReturn = m_objSMenu.Name + "." + fieldName;
                else if (fieldName == MenuHierarchy.Name)
                    m_strReturn = m_objSMenu.Name + "." + fieldName;
                else if (fieldName == MenuDesc.Name)
                    m_strReturn = m_objSMenu.Name + "." + fieldName;
                else if (fieldName == MenuIcon.Name)
                    m_strReturn = m_objSMenu.Name + "." + fieldName;
                else if (fieldName == MenuUrl.Name)
                    m_strReturn = m_objSMenu.Name + "." + fieldName;
                else if (fieldName == MenuVisible.Name)
                    m_strReturn = m_objSMenu.Name + "." + fieldName;
                else if (fieldName == RoleID.Name)
                    m_strReturn = m_objDRoleMenuAction.Name + "." + RoleID.Name;
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