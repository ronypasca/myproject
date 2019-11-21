using com.SML.Lib.Common;
using com.SML.BIGTRONS.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace com.SML.BIGTRONS.ViewModels
{
    public class ConfigVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string Key1 { get; set; }
        [Key, Column(Order = 2)]
        public string Key2 { get; set; }
        [Key, Column(Order = 3)]
        public string Key3 { get; set; }
        [Key, Column(Order = 4)]
        public string Key4 { get; set; }
        public string Desc1 { get; set; }
        public string Desc2 { get; set; }
        public string Desc3 { get; set; }
        public string Desc4 { get; set; }

        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class Key1
            {
                public static string Desc { get { return "Key1"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class Key2
            {
                public static string Desc { get { return "Key2"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Key3
            {
                public static string Desc { get { return "Key3"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Key4
            {
                public static string Desc { get { return "Key4"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Desc1
            {
                public static string Desc { get { return "Desc1"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Desc2
            {
                public static string Desc { get { return "Desc2"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Desc3
            {
                public static string Desc { get { return "Desc3"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Desc4
            {
                public static string Desc { get { return "Desc4"; } }
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
                UConfig m_objUConfig = new UConfig();

                if (fieldName == Key1.Name)
                    m_strReturn = m_objUConfig.Name + "." + fieldName;
                else if (fieldName == Key2.Name)
                    m_strReturn = m_objUConfig.Name + "." + fieldName;
                else if (fieldName == Key3.Name)
                    m_strReturn = m_objUConfig.Name + "." + fieldName;
                else if (fieldName == Key4.Name)
                    m_strReturn = m_objUConfig.Name + "." + fieldName;
                else if (fieldName == Desc1.Name)
                    m_strReturn = m_objUConfig.Name + "." + fieldName;
                else if (fieldName == Desc2.Name)
                    m_strReturn = m_objUConfig.Name + "." + fieldName;
                else if (fieldName == Desc3.Name)
                    m_strReturn = m_objUConfig.Name + "." + fieldName;
                else if (fieldName == Desc4.Name)
                    m_strReturn = m_objUConfig.Name + "." + fieldName;

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
            PropertyInfo m_pifDimensionVM = this.GetType().GetProperty(fieldName);
            return m_pifDimensionVM != null && Attribute.GetCustomAttribute(m_pifDimensionVM, typeof(KeyAttribute)) != null;
        }
    }
}