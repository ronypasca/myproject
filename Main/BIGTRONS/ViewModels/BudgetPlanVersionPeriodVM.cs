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
    public class BudgetPlanVersionPeriodVM
    {
        #region Public Property


        [Key, Column(Order = 1)]
        public string BudgetPlanVersionPeriodID { get; set; }
        public string BudgetPlanID { get; set; }
        public int BudgetPlanVersion { get; set; }
        public int PeriodVersion { get; set; }
        public string BudgetPlanPeriodID { get; set; }
        public int StatusID { get; set; }
        public string StatusDesc { get; set; }
        public string BudgetPlanPeriodDesc { get; set; }
        public List<BudgetPlanVersionVendorVM> ListBudgetPlanVersionVendorVM { get; set; }

        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class BudgetPlanVersionPeriodID
            {
                public static string Desc { get { return "Budget Plan Version Period ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

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
            public static class PeriodVersion
            {
                public static string Desc { get { return "Period"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class BudgetPlanPeriodID
            {
                public static string Desc { get { return "Budget Plan Period ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BudgetPlanPeriodDesc
            {
                public static string Desc { get { return "Info"; } }
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
                DBudgetPlanVersionPeriod m_objDBudgetPlanVersionPeriod = new DBudgetPlanVersionPeriod();
                DBudgetPlanVersion m_objDBudgetPlanVersion = new DBudgetPlanVersion();
                SBudgetPlanPeriod m_objSBudgetPlanPeriod = new SBudgetPlanPeriod();
                MStatus m_objMStatus = new MStatus();

                if (fieldName == BudgetPlanVersionPeriodID.Name)
                    m_strReturn = m_objDBudgetPlanVersionPeriod.Name + "." + fieldName;
                else if (fieldName == BudgetPlanID.Name)
                    m_strReturn = m_objDBudgetPlanVersionPeriod.Name + "." + fieldName;
                else if(fieldName == BudgetPlanVersion.Name)
                    m_strReturn = m_objDBudgetPlanVersionPeriod.Name + "." + fieldName;
                else if (fieldName == StatusID.Name)
                    m_strReturn = m_objDBudgetPlanVersionPeriod.Name + "." + fieldName;
                else if (fieldName == StatusDesc.Name)
                    m_strReturn = m_objMStatus.Name + "." + fieldName;
                else if (fieldName == BudgetPlanPeriodDesc.Name)
                    m_strReturn = m_objSBudgetPlanPeriod.Name + "." + fieldName;
                else if (fieldName == BudgetPlanPeriodID.Name)
                    m_strReturn = m_objDBudgetPlanVersionPeriod.Name + "." + fieldName;
                else if (fieldName == PeriodVersion.Name)
                    m_strReturn = m_objDBudgetPlanVersionPeriod.Name + "." + fieldName;

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