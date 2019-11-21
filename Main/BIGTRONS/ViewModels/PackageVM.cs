using com.SML.Lib.Common;
using com.SML.BIGTRONS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace com.SML.BIGTRONS.ViewModels
{
    public class PackageVM
    {
        #region Public Property

        private string m_strBudgetPlanListDesc = "";

        [Key, Column(Order = 1)]
        public string PackageID { get; set; }
        public string PackageDesc { get; set; }
        public int StatusID { get; set; }
        public string StatusDesc { get; set; }
        public BudgetPlanVM RABFirstVersion{ get; set; }
        public BudgetPlanVM RABLastVersion { get; set; }
        public List<VendorVM> ListVendor{ get; set; }

        public string BudgetPlanListDesc
        {
            get { return m_strBudgetPlanListDesc.Replace("&lt;br/&gt;", "<br/>"); }
            set { m_strBudgetPlanListDesc = value; }
        }
        public string PackageProjectID { get; set; }
        public decimal Area { get; set; }
        public string UnitTypeDesc { get; set; }
        public string Blok { get; set; }
        public decimal Unit { get; set; }
        public string ProjectID
        {
            get; set;
        }
        public string ProjectDesc
        {
            get; set;
        }
        public string CompanyID
        {
            get; set;
        }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<PackageListVM> BudgetPlanList
        {
            get; set;
        }
        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class PackageID
            {
                public static string Desc { get { return "ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class CreatedDate
            {
                public static string Desc { get { return "Created Date"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ModifiedDate
            {
                public static string Desc { get { return "Last Changed"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ProjectID
            {
                public static string Desc { get { return "ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class CompanyID
            {
                public static string Desc { get { return "CompanyID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class PackageDesc
            {
                public static string Desc { get { return "Description"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class StatusID
            {
                public static string Desc { get { return "Status ID"; } }
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

            public static class BudgetPlanListDesc
            {
                public static string Desc { get { return "Budget Plan"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class PackageProjectID
            {
                public static string Desc { get { return "Package Project ID"; } }
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
                TPackage m_objTPackage = new TPackage();
                MStatus m_objMStatus = new MStatus();
                DPackageList m_objDPackList = new DPackageList();

                //SUBSTRING((select distinct ','+B.tablename from mstatus as B where B.statusid=A.statusid for xml path ('')),2,1000) as ss

                if (fieldName == PackageID.Name)
                    m_strReturn = m_objTPackage.Name + "." + fieldName;
                if (fieldName == CreatedDate.Name)
                    m_strReturn = m_objTPackage.Name + "." + fieldName;
                if (fieldName == ModifiedDate.Name)
                    m_strReturn = m_objTPackage.Name + "." + fieldName;
                else if (fieldName == PackageDesc.Name)
                    m_strReturn = m_objTPackage.Name + "." + fieldName;
                else if (fieldName == StatusID.Name)
                    m_strReturn = m_objTPackage.Name + "." + fieldName;
                else if (fieldName == StatusDesc.Name)
                    m_strReturn = m_objMStatus.Name + "." + fieldName;
                else if (fieldName == BudgetPlanListDesc.Name)
                    m_strReturn = "REPLACE(SUBSTRING((SELECT distinct '<br/>'+" + PackageListVM.Prop.BudgetPlanID.Map + " FROM "
                        + m_objDPackList.Name + " WHERE " + PackageListVM.Prop.PackageID.Map + "=" + PackageID.Map
                        + " FOR XML PATH ('')),12,1000),',','<br/>')";
               else if (fieldName == PackageProjectID.Name)
                    m_strReturn = "(select top 1 TBudgetPlan.ProjectID from DPackageList join TBudgetPlan on DPackageList.BudgetPlanID = TBudgetPlan.BudgetPlanID where  DPackageList.PackageID = TPackage.PackageID)";

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
            PropertyInfo m_pifPackageVM = this.GetType().GetProperty(fieldName);
            return m_pifPackageVM != null && Attribute.GetCustomAttribute(m_pifPackageVM, typeof(KeyAttribute)) != null;
        }
    }
}