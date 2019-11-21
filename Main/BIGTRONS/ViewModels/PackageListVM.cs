using com.SML.Lib.Common;
using com.SML.BIGTRONS.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace com.SML.BIGTRONS.ViewModels
{
    public class PackageListVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string PackageID { get; set; }
        [Key, Column(Order = 2)]
        public string BudgetPlanID { get; set; }
        [Key, Column(Order = 3)]
        public int BudgetPlanVersion { get; set; }
        public int MaxVersion { get; set; }
        public string BudgetPlanTypeDesc { get; set; }
        public string Description { get; set; }
        public string StatusDesc { get; set; }
        public int StatusID { get; set; }
        public string ProjectID { get; set; }
        public string ProjectDesc { get; set; }
        public string CompanyID { get; set; }
        public string CompanyDesc { get; set; }
        public int PackageStatusID { get; set; }
        public string BudgetPlanTemplateID { get; set; }
        public string BudgetPlanTemplateDesc { get; set; }
        public string ClusterID { get; set; }
        public string ClusterDesc { get; set; }
        public string RegionDesc { get; set; }
        public string DivisionDesc { get; set; }
        public string UnitTypeDesc { get; set; }
        public decimal Area { get; set; }
        public string BudgetPlanVersionVendorID { get; set; }
        public decimal LastApprovedVersionArea { get; set; }
        public string BlokNo { get; set; }
        public string LastApprovedVersionBlokNo { get; set; }
        public decimal Unit { get; set; }
        public decimal LastApprovedVersionUnit { get; set; }
        public string LocationDesc { get; set; }
        public decimal FeePercentage { get; set; }
        public decimal FeePercentageVendorEntry { get; set; }
        public decimal LastApprovedVersionFeePercentage { get; set; }

        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class LocationDesc
            {
                public static string Desc { get { return "Location"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class UnitTypeDesc
            {
                public static string Desc { get { return "Unit Type"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class PackageID
            {
                public static string Desc { get { return "ID"; } }
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
            public static class BlockNo
            {
                public static string Desc { get { return "Blok No. Rumah"; } }
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
            public static class LastApprovedVersionArea
            {
                public static string Desc { get { return "Area"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class LastApprovedVersionBlockNo
            {
                public static string Desc { get { return "Blok No. Rumah"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class LastApprovedVersionUnit
            {
                public static string Desc { get { return "Unit"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class CompanyDesc
            {
                public static string Desc { get { return "ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class DivisionDesc
            {
                public static string Desc { get { return "ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class RegionDesc
            {
                public static string Desc { get { return "ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BudgetPlanTemplateID
            {
                public static string Desc { get { return "Budget Plan Template ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ClusterID
            {
                public static string Desc { get { return "Cluster ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ClusterDesc
            {
                public static string Desc { get { return "Cluster"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BudgetPlanTemplateDesc
            {
                public static string Desc { get { return "Budget Plan Template"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ProjectID
            {
                public static string Desc { get { return "ProjectID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ProjectDesc
            {
                public static string Desc { get { return "ProjectDesc"; } }
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
            public static class BudgetPlanTypeDesc
            {
                public static string Desc { get { return "BudgetPlanTypeDesc"; } }
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
            public static class StatusDesc
            {
                public static string Desc { get { return "StatusDesc"; } }
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
            public static class PackageStatusID
            {
                public static string Desc { get { return "PackageStatusID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BudgetPlanID
            {
                public static string Desc { get { return "BudgetPlanID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BudgetPlanVersion
            {
                public static string Desc { get { return "BudgetPlanVersion"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class MaxVersion
            {
                public static string Desc { get { return "Max Version"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            
            public static class FeePercentage
            {
                public static string Desc { get { return "FeePercentage"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class FeePercentageVendorEntry
            {
                public static string Desc { get { return "FeePercentage"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            //BudgetPlanVersionVendorID
            public static class BudgetPlanVersionVendorID
            {
                public static string Desc { get { return "Version Vendor ID"; } }
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
            public static class LastApprovedVersionFeePercentage
            {
                public static string Desc { get { return "FeePercentage"; } }
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
                DPackageList m_objDPackageList = new DPackageList();
                TPackage m_objTPackage = new TPackage();
                MStatus m_objMStatus = new MStatus();
                DBudgetPlanVersion m_objDBudgetPlanVersion = new DBudgetPlanVersion();
                MBudgetPlanType m_objMBudgetPlanType = new MBudgetPlanType();
                MProject m_objMProject = new MProject();
                MRegion m_objMRegion = new MRegion();
                MCluster m_objMCluster = new MCluster();
                MCompany m_objMCompany = new MCompany();
                MDivision m_objMDivision = new MDivision();
                MUnitType m_objMUnitType = new MUnitType();
                MLocation m_objMLocation = new MLocation();
                TBudgetPlan m_objTBudgetPlan = new TBudgetPlan();
                MBudgetPlanTemplate m_objMBudgetPlanTemplate = new MBudgetPlanTemplate();
                DBudgetPlanVersionVendor m_objVersionVendor = new DBudgetPlanVersionVendor();
                //SUBSTRING((select distinct ','+B.tablename from mstatus as B where B.statusid=A.statusid for xml path ('')),2,1000) as ss

                if (fieldName == PackageID.Name)
                    m_strReturn = m_objDPackageList.Name + "." + fieldName;
                else if (fieldName == BudgetPlanID.Name)
                    m_strReturn = m_objDPackageList.Name + "." + fieldName;
                else if (fieldName == BudgetPlanVersion.Name)
                    m_strReturn = m_objDPackageList.Name + "." + fieldName;
                else if (fieldName == MaxVersion.Name)
                    m_strReturn = "MaxVersionRAB." + fieldName;
                else if (fieldName == StatusDesc.Name)
                    m_strReturn = m_objMStatus.Name + "." + fieldName;
                else if (fieldName == StatusID.Name)
                    m_strReturn = m_objDBudgetPlanVersion.Name + "." + fieldName;
                else if (fieldName == Description.Name)
                    m_strReturn = m_objDBudgetPlanVersion.Name + "." + fieldName;
                else if (fieldName == BudgetPlanTypeDesc.Name)
                    m_strReturn = m_objMBudgetPlanType.Name + "." + fieldName;
                else if (fieldName == ProjectID.Name)
                    m_strReturn = m_objMProject.Name + "." + fieldName;
                else if (fieldName == ProjectDesc.Name)
                    m_strReturn = m_objMProject.Name + "." + fieldName;
                else if (fieldName == CompanyID.Name)
                    m_strReturn = m_objMProject.Name + "." + fieldName;
                else if (fieldName == CompanyDesc.Name)
                    m_strReturn = m_objMCompany.Name + "." + fieldName;
                else if (fieldName == PackageStatusID.Name)
                    m_strReturn = m_objTPackage.Name + "." + "StatusID";
                else if (fieldName == BudgetPlanTemplateID.Name)
                    m_strReturn = m_objMBudgetPlanTemplate.Name + "." + fieldName;
                else if (fieldName == BudgetPlanTemplateDesc.Name)
                    m_strReturn = m_objMBudgetPlanTemplate.Name + "." + fieldName;
                else if (fieldName == ClusterDesc.Name)
                    m_strReturn = m_objMCluster.Name + "." + fieldName;
                else if (fieldName == ClusterID.Name)
                    m_strReturn = m_objMCluster.Name + "." + fieldName;
                else if (fieldName == CompanyID.Name)
                    m_strReturn = m_objMProject.Name + "." + fieldName;
                else if (fieldName == CompanyDesc.Name)
                    m_strReturn = m_objMCompany.Name + "." + fieldName;
                else if (fieldName == RegionDesc.Name)
                    m_strReturn = m_objMRegion.Name + "." + fieldName;
                else if (fieldName == DivisionDesc.Name)
                    m_strReturn = m_objMDivision.Name + "." + fieldName;
                else if (fieldName == UnitTypeDesc.Name)
                    m_strReturn = m_objMUnitType.Name + "." + fieldName;
                else if (fieldName == BlockNo.Name)
                    m_strReturn = m_objDBudgetPlanVersion.Name + "." + fieldName;
                else if (fieldName == Area.Name)
                    m_strReturn = m_objDBudgetPlanVersion.Name + "." + fieldName;
                else if (fieldName == VendorID.Name)
                    m_strReturn = m_objVersionVendor.Name + "." + fieldName;
                else if (fieldName == BudgetPlanVersionVendorID.Name)
                    m_strReturn = m_objVersionVendor.Name + "." + fieldName;
                else if (fieldName == FeePercentageVendorEntry.Name)
                    m_strReturn = m_objVersionVendor.Name + "." + "FeePercentage";
                else if (fieldName == Unit.Name)
                    m_strReturn = m_objDBudgetPlanVersion.Name + "." + fieldName;
                else if (fieldName == LastApprovedVersionArea.Name)
                    m_strReturn = "MaxVersionRAB." + "Area";
                else if (fieldName == LastApprovedVersionBlockNo.Name)
                    m_strReturn = "MaxVersionRAB." + "BlockNo";
                else if (fieldName == LastApprovedVersionUnit.Name)
                    m_strReturn = "MaxVersionRAB." + "Unit";
                else if (fieldName == LocationDesc.Name)
                    m_strReturn = m_objMLocation.Name + "." + fieldName;
                else if (fieldName == FeePercentage.Name)
                    m_strReturn = m_objDBudgetPlanVersion.Name + "." + fieldName;
                else if (fieldName == LastApprovedVersionFeePercentage.Name)
                    m_strReturn = "MaxVersionRAB." + "FeePercentage";

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
            PropertyInfo m_pifPackageListVM = this.GetType().GetProperty(fieldName);
            return m_pifPackageListVM != null && Attribute.GetCustomAttribute(m_pifPackageListVM, typeof(KeyAttribute)) != null;
        }
    }
}