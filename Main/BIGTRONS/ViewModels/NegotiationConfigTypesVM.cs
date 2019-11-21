using com.SML.Lib.Common;
using com.SML.BIGTRONS.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace com.SML.BIGTRONS.ViewModels
{
    public class NegotiationConfigTypesVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string NegotiationConfigTypeID { get; set; }
        public string Descriptions { get; set; }

        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class NegotiationConfigTypeID
            {
                public static string Desc { get { return "ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class Descriptions
            {
                public static string Desc { get { return "Description"; } }
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
                SNegotiationConfigTypes m_objSNegotiationConfigTypes = new SNegotiationConfigTypes();

                if (fieldName == NegotiationConfigTypeID.Name)
                    m_strReturn = m_objSNegotiationConfigTypes.Name + "." + fieldName;
                else if (fieldName == Descriptions.Name)
                    m_strReturn = m_objSNegotiationConfigTypes.Name + "." + fieldName;

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
            PropertyInfo m_pifTaskTypesVM = this.GetType().GetProperty(fieldName);
            return m_pifTaskTypesVM != null && Attribute.GetCustomAttribute(m_pifTaskTypesVM, typeof(KeyAttribute)) != null;
        }
    }
}