using com.SML.Lib.Common;
using com.SML.BIGTRONS.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace com.SML.BIGTRONS.ViewModels
{
    public class RoleMenuActionVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string RoleID { get; set; }
        [Key, Column(Order = 2)]
        public string MenuID { get; set; }
        [Key, Column(Order = 3)]
        public string ActionID { get; set; }
        public string MenuUrl { get; set; }

        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class RoleID
            {
                public static string Desc { get { return "RoleID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class MenuUrl
            {
                public static string Desc { get { return "Menu Url"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class MenuID
            {
                public static string Desc { get { return "Menu"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ActionID
            {
                public static string Desc { get { return "Action"; } }
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
                DRoleMenuAction m_objDRoleMenuAction = new DRoleMenuAction();
                SMenu m_objSMenu = new SMenu();

                if (fieldName == RoleID.Name)
                    m_strReturn = m_objDRoleMenuAction.Name + "." + fieldName;
                else if (fieldName == MenuID.Name)
                    m_strReturn = m_objDRoleMenuAction.Name + "." + fieldName;
                else if (fieldName == ActionID.Name)
                    m_strReturn = m_objDRoleMenuAction.Name + "." + fieldName;
                else if (fieldName == MenuUrl.Name)
                    m_strReturn = m_objSMenu.Name + "." + fieldName;

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
            PropertyInfo m_pifRoleVM = this.GetType().GetProperty(fieldName);
            return m_pifRoleVM != null && Attribute.GetCustomAttribute(m_pifRoleVM, typeof(KeyAttribute)) != null;
        }
    }
}