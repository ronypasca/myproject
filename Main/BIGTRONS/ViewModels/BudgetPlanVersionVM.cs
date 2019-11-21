using com.SML.Lib.Common;
using com.SML.BIGTRONS.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace com.SML.BIGTRONS.ViewModels
{
    public class BudgetPlanVersionVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string BudgetPlanID { get; set; }
        [Key, Column(Order = 2)]
        public int BudgetPlanVersion { get; set; }
        public string Description { get; set; }
        public decimal Area { get; set; }
        public decimal Unit { get; set; }
        public int StatusID { get; set; }
        public int StatusDesc { get; set; }
        public string BudgetPlanTemplateID { get; set; }
        public decimal FeePercentage { get; set; }
        public bool IsBidOpen { get; set; }
        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class BudgetPlanID
            {
                public static string Desc { get { return "Budget Plan ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class BudgetPlanVersion
            {
                public static string Desc { get { return "Version"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Description
            {
                public static string Desc { get { return "Description"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Area
            {
                public static string Desc { get { return "Area"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Unit
            {
                public static string Desc { get { return "Unit"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class StatusID
            {
                public static string Desc { get { return "StatusID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class StatusDesc
            {
                public static string Desc { get { return "Status"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BudgetPlanTemplateID
            {
                public static string Desc { get { return "Template ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class FeePercentage
            {
                public static string Desc { get { return "Fee Percentage"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class IsBidOpen
            {
                public static string Desc { get { return "IsBidOpen"; } }
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
                DBudgetPlanVersion m_objDBudgetPlanVersion = new DBudgetPlanVersion();
                TBudgetPlan m_objTBudgetPlan = new TBudgetPlan();
                MStatus m_objMStatus = new MStatus();  
                if (fieldName == BudgetPlanID.Name)
                    m_strReturn = m_objDBudgetPlanVersion.Name + "." + fieldName;
                else if (fieldName == BudgetPlanVersion.Name)
                    m_strReturn = m_objDBudgetPlanVersion.Name + "." + fieldName;
                else if (fieldName == Description.Name)
                    m_strReturn = m_objDBudgetPlanVersion.Name + "." + fieldName;
                else if (fieldName == Area.Name)
                    m_strReturn = m_objDBudgetPlanVersion.Name + "." + fieldName;
                else if (fieldName == Unit.Name)
                    m_strReturn = m_objDBudgetPlanVersion.Name + "." + fieldName;
                else if (fieldName == StatusID.Name)
                    m_strReturn = m_objDBudgetPlanVersion.Name + "." + fieldName;
                else if (fieldName == StatusDesc.Name)
                    m_strReturn = m_objMStatus.Name + "." + fieldName;
                else if (fieldName == BudgetPlanTemplateID.Name)
                    m_strReturn = m_objTBudgetPlan.Name + "." + fieldName;
                else if (fieldName == FeePercentage.Name)
                    m_strReturn = m_objDBudgetPlanVersion.Name + "." + fieldName;
                else if (fieldName == IsBidOpen.Name)
                    m_strReturn = m_objDBudgetPlanVersion.Name + "." + fieldName;

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