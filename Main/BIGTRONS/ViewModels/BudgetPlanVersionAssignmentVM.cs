using com.SML.Lib.Common;
using com.SML.BIGTRONS.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace com.SML.BIGTRONS.ViewModels
{
    public class BudgetPlanVersionAssignmentVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string BudgetPlanVersionStructureID { get; set; }
        [Key, Column(Order = 2)]
        public string VendorID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string VendorDesc {
            get { 
                    return FirstName+" "+LastName;
                }
        }
        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class BudgetPlanVersionStructureID
            {
                public static string Desc { get { return "Budget Plan Version Structure ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class VendorID
            {
                public static string Desc { get { return "Vendor ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class VendorDesc
            {
                public static string Desc { get { return "Vendor"; } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class FirstName
            {
                public static string Desc { get { return "First Name"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class LastName
            {
                public static string Desc { get { return "Last Name"; } }
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
                DBudgetPlanVersionAssignment m_objDBudgetPlanVersionAssignment = new DBudgetPlanVersionAssignment();
                
                if (fieldName == BudgetPlanVersionStructureID.Name)
                    m_strReturn = m_objDBudgetPlanVersionAssignment.Name + "." + fieldName;
                else if (fieldName == VendorID.Name)
                    m_strReturn = m_objDBudgetPlanVersionAssignment.Name + "." + fieldName;
                else if (fieldName == FirstName.Name)
                    m_strReturn = m_objDBudgetPlanVersionAssignment.Name + "." + fieldName;
                else if (fieldName == LastName.Name)
                    m_strReturn = m_objDBudgetPlanVersionAssignment.Name + "." + fieldName;

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
            PropertyInfo m_pifBudgetPlanTypeVM = this.GetType().GetProperty(fieldName);
            return m_pifBudgetPlanTypeVM != null && Attribute.GetCustomAttribute(m_pifBudgetPlanTypeVM, typeof(KeyAttribute)) != null;
        }
    }
}