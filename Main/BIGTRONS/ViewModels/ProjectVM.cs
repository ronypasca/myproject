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
    public class ProjectVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string ProjectID { get; set; }
        public string ProjectDesc { get; set; }
        public List<BudgetPlanVM> ListBudgetPlanID { get; set; }
        public string BudgetPlanID { get; set; }
        public string BudgetPlanDescription { get; set; }
        public string CompanyID { get; set; }
        public string CompanyDesc { get; set; }
        public string DivisionID { get; set; }
        public string DivisionDesc { get; set; }
        public string LocationID { get; set; }
        public string LocationDesc { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Postal { get; set; }

        public string RegionDesc { get; set; }
        public string RegionID { get; set; }
        public bool include { get; set; }

        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class ProjectID
            {
                public static string Desc { get { return "ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class ProjectDesc
            {
                public static string Desc { get { return "Description"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class CompanyID
            {
                public static string Desc { get { return "Company ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class CompanyDesc
            {
                public static string Desc { get { return "Company"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class DivisionID
            {
                public static string Desc { get { return "Division ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class DivisionDesc
            {
                public static string Desc { get { return "Division"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class LocationID
            {
                public static string Desc { get { return "Location ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class LocationDesc
            {
                public static string Desc { get { return "Location"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class City
            {
                public static string Desc { get { return "City"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Street
            {
                public static string Desc { get { return "Street"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Postal
            {
                public static string Desc { get { return "Postal"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class RegionID
            {
                public static string Desc { get { return "Region ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class RegionDesc
            {
                public static string Desc { get { return "Region"; } }
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
                MProject m_objProject = new MProject();
                MCompany m_objMCompany = new MCompany();
                MDivision m_objMDivision = new MDivision();
                MLocation m_objMLocation = new MLocation();
                MRegion m_objMRegion = new MRegion();

                if (fieldName == ProjectID.Name)
                    m_strReturn = m_objProject.Name + "." + fieldName;
                else if (fieldName == ProjectDesc.Name)
                    m_strReturn = m_objProject.Name + "." + fieldName;
                else if (fieldName == CompanyID.Name)
                    m_strReturn = m_objProject.Name + "." + fieldName;
                else if (fieldName == CompanyDesc.Name)
                    m_strReturn = m_objMCompany.Name + "." + CompanyVM.Prop.CompanyDesc.Name;
                else if (fieldName == DivisionID.Name)
                    m_strReturn = m_objMDivision.Name + "." + fieldName;
                else if (fieldName == DivisionDesc.Name)
                    m_strReturn = m_objMDivision.Name + "." + DivisionVM.Prop.DivisionDesc.Name;
                else if (fieldName == LocationID.Name)
                    m_strReturn = m_objProject.Name + "." + fieldName;
                else if (fieldName == LocationDesc.Name)
                    m_strReturn = m_objMLocation.Name + "." + LocationVM.Prop.LocationDesc.Name;
                else if (fieldName == City.Name)
                    m_strReturn = m_objProject.Name + "." + fieldName;
                else if (fieldName == Street.Name)
                    m_strReturn = m_objProject.Name + "." + fieldName;
                else if (fieldName == Postal.Name)
                    m_strReturn = m_objProject.Name + "." + fieldName;
                else if (fieldName == RegionDesc.Name)
                    m_strReturn = m_objMRegion.Name + "." + fieldName;
                else if (fieldName == RegionID.Name)
                    m_strReturn = m_objMRegion.Name + "." + fieldName;

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