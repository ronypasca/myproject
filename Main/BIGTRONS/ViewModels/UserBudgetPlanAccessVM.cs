using com.SML.Lib.Common;
using com.SML.BIGTRONS.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace com.SML.BIGTRONS.ViewModels
{
    public class UserBudgetPlanAccessVM
    {
        #region Public Property
        [Key, Column(Order = 1)]
        public string UserID { get; set; }
        [Key, Column(Order = 2)]
        public string BudgetPlanTemplateID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public DateTime StartHours { get; set; }
        public DateTime EndHours { get; set; }
        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class UserID
            {
                public static string Desc { get { return "UserID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class BudgetPlanTemplateID
            {
                public static string Desc { get { return "BudgetPlanTemplateID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class StartDate
            {
                public static string Desc { get { return "StartDate"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class EndDate
            {
                public static string Desc { get { return "EndDate"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class startHours
            {
                public static string Desc { get { return "startHours"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class EndHours
            {
                public static string Desc { get { return "EndHours"; } }
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
                DUserBudgetPlanAccess m_objDUserBudgetPlanAccess = new DUserBudgetPlanAccess();

                if (fieldName == UserID.Name)
                    m_strReturn = m_objDUserBudgetPlanAccess.Name + "." + fieldName;
                else if (fieldName == BudgetPlanTemplateID.Name)
                    m_strReturn = m_objDUserBudgetPlanAccess.Name + "." + fieldName;
                else if (fieldName == StartDate.Name)
                    m_strReturn = m_objDUserBudgetPlanAccess.Name + "." + fieldName;
                else if (fieldName == EndDate.Name)
                    m_strReturn = m_objDUserBudgetPlanAccess.Name + "." + fieldName;
                else if (fieldName == startHours.Name)
                    m_strReturn = m_objDUserBudgetPlanAccess.Name + "." + fieldName;
                else if (fieldName == EndHours.Name)
                    m_strReturn = m_objDUserBudgetPlanAccess.Name + "." + fieldName;

                m_strReturn += (withAlias ? " AS [" + fieldName + "]" : "");
                return m_strReturn;
            }
        }

        #endregion
    }
}