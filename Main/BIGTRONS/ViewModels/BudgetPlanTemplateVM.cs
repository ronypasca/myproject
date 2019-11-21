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
    public class BudgetPlanTemplateVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string BudgetPlanTemplateID { get; set; }
        public string BudgetPlanTemplateDesc { get; set; }
        public string BudgetPlanTypeID { get; set; }
        public string BudgetPlanTypeDesc { get; set; }

        public List<BudgetPlanTemplateStructureVM> ListBudgetPlanTemplateStructureVM { get; set; }

        public List<UserVM> lstUser { get; set; }
        public List<UserBudgetPlanAccessVM> lstUserBudgetPlanAccessVM { get; set; }
        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class BudgetPlanTemplateID
            {
                public static string Desc { get { return "ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class BudgetPlanTemplateDesc
            {
                public static string Desc { get { return "Description"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BudgetPlanTypeID
            {
                public static string Desc { get { return "BudgetPlanType ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
        
            public static class BudgetPlanTypeDesc
            {
                public static string Desc { get { return "Budget Plan Type"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ListBudgetPlanTemplateStructureVM
            {
                public static string Desc { get { return "List Budget Structure"; } }
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
                MBudgetPlanTemplate m_objBudgetPlanTemplate = new MBudgetPlanTemplate();
                MBudgetPlanType m_objMBudgetPlanType = new MBudgetPlanType();

                if (fieldName == BudgetPlanTemplateID.Name)
                    m_strReturn = m_objBudgetPlanTemplate.Name + "." + fieldName;
                else if (fieldName == BudgetPlanTemplateDesc.Name)
                    m_strReturn = m_objBudgetPlanTemplate.Name + "." + fieldName;
                else if (fieldName == BudgetPlanTypeID.Name)
                    m_strReturn = m_objBudgetPlanTemplate.Name + "." + BudgetPlanTypeID.Name;
                else if (fieldName == BudgetPlanTypeDesc.Name)
                    m_strReturn = m_objMBudgetPlanType.Name + "." + BudgetPlanTypeVM.Prop.BudgetPlanTypeDesc.Name;


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
            PropertyInfo m_pifBudgetPlanTemplateVM = this.GetType().GetProperty(fieldName);
            return m_pifBudgetPlanTemplateVM != null && Attribute.GetCustomAttribute(m_pifBudgetPlanTemplateVM, typeof(KeyAttribute)) != null;
        }
    }
}