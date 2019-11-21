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
    public class BudgetPlanVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string BudgetPlanID { get; set; }
        public string BudgetPlanTemplateID { get; set; }
        public string BudgetPlanTemplateDesc { get; set; }
        public string TCProjectType { get; set; }
        public string TCTypeDesc { get; set; }
        public List<VendorVM> VendorParticipant { get; set; }
        public string RegionID { get; set; }
        public string ProjectID { get; set; }
        public string ProjectDesc { get; set; }
        public string ClusterID { get; set; }
        public string ClusterDesc { get; set; }
        public string UnitTypeID { get; set; }
        public string Blok { get; set; }

        public string UnitTypeDesc { get; set; }
        public int StatusID { get; set; }
        public string StatusDesc { get; set; }
        public decimal Area { get; set; }
        public decimal Unit { get; set; }
        [Key, Column(Order = 2)]
        public int BudgetPlanVersion { get; set; }
        public int MaxBudgetPlanVersion { get; set; }
        public string Description { get; set; }
        public string BudgetPlanTypeID { get; set; }
        public string BudgetPlanTypeDesc { get; set; }
        public string NegoConfigID { get; set; }
        public decimal P3value { get; set; }
        public decimal RABSubtotal { get; set; }
        public string CompanyDesc { get; set; }
        public string RegionDesc { get; set; }
        public string LocationDesc { get; set; }
        public string BusinessUnitID { get; set; }
        public string DivisionID { get; set; }
        public string DivisionDesc { get; set; }
        public bool include { get; set; }
        public bool IsBidOpen { get; set; }
        public string BlockNo { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Ext.Net.Node Structure { get; set; }
        public BudgetPlanVersionStructureVM StructureModel { get; set; }
        public List<BudgetPlanVersionStructureVM> ListBudgetPlanVersionStructureVM { get; set; }
        public List<BudgetPlanVersionVendorVM> ListBudgetPlanVersionVendorVM { get; set; }
        private string Vendor_;
        public string Vendor
        {
            get
            {
                string Ver = "";
                if (this.ListBudgetPlanVersionVendorVM != null)
                    if (this.ListBudgetPlanVersionVendorVM.Count > 0 && this.ListBudgetPlanVersionVendorVM[0].BudgetPlanVersionVendorID != null)
                    {
                        {
                            Ver = string.Empty;
                            foreach (BudgetPlanVersionVendorVM n in this.ListBudgetPlanVersionVendorVM)
                            {
                                Ver += n.VendorDesc;
                                Ver += " </br>";
                            }
                        }
                    }
                return Ver;

            }
            set { this.Vendor_ = value; }
        }
        public decimal FeePercentage { get; set; }
        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class BudgetPlanID
            {
                public static string Desc { get { return "ID"; } }
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
            public static class BudgetPlanTemplateDesc
            {
                public static string Desc { get { return "Template"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BudgetPlanTypeID
            {
                public static string Desc { get { return "Type ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BudgetPlanTypeDesc
            {
                public static string Desc { get { return "Budget Type"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ProjectID
            {
                public static string Desc { get { return "Project ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ProjectDesc
            {
                public static string Desc { get { return "Project"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Blok
            {
                public static string Desc { get { return "Blok"; } }
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
            public static class UnitTypeID
            {
                public static string Desc { get { return "UnitType ID"; } }
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
            public static class Vendor
            {
                public static string Desc { get { return "Vendor"; } }
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

            public static class BudgetPlanVersion
            {
                public static string Desc { get { return "Version"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class MaxBudgetPlanVersion
            {
                public static string Desc { get { return "Max Version"; } }
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
            public static class ListBudgetPlanVersionStructureVM
            {
                public static string Desc { get { return "List of Budget Plan Structure"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ListBudgetPlanVersionVendorVM
            {
                public static string Desc { get { return "ListBudgetPlanVersionVendorVM"; } }
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
            public static class BusinessUnitID
            {
                public static string Desc { get { return "BusinessUnitID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class DivisionID
            {
                public static string Desc { get { return "DivisionID"; } }
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

            public static class CompanyDesc
            {
                public static string Desc { get { return "Company"; } }
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
            public static class Subtotal
            {
                public static string Desc { get { return "Sub Total "; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ContractorFee
            {
                public static string Desc { get { return "Fee (%)"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Total
            {
                public static string Desc { get { return "Total"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Rounding
            {
                public static string Desc { get { return "Pembulatan"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class PersentageByRAB
            {
                public static string Desc { get { return "Persentase BudgetPlan"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Tax
            {
                public static string Desc { get { return "PPN 10%"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class GrandTotalExcPPN
            {
                public static string Desc { get { return "Grand Total (Excl. PPN)"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class GrandTotalIncPPN
            {
                public static string Desc { get { return "Grand Total (Incl. PPN)"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class AreaSize
            {
                public static string Desc { get { return "Luas (M²)"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BasicPriceI
            {
                public static string Desc { get { return "Harga Dasar Per M²"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BasicPriceII
            {
                public static string Desc { get { return "Harga Dasar + Fee 10% Per M²"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BasicPriceIII
            {
                public static string Desc { get { return "Harga Dasar + Fee 10% + PPN 10% Per M²"; } }
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
                public static string Desc { get { return "Is Bid Open"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class BlockNo
            {
                public static string Desc { get { return "Block No"; } }
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
                TBudgetPlan m_objTBudgetPlan = new TBudgetPlan();
                DBudgetPlanVersionStructure m_objDBudgetPlanVersionStructure = new DBudgetPlanVersionStructure();
                DBudgetPlanVersion m_objDBudgetPlanVersion = new DBudgetPlanVersion();
                DBudgetPlanTemplateStructure m_objDBudgetPlanTemplateStructure = new DBudgetPlanTemplateStructure();
                MBudgetPlanTemplate m_objMBudgetPlanTemplate = new MBudgetPlanTemplate();
                MStatus m_objMStatus = new MStatus();
                MBudgetPlanType m_objMBudgetPlanType = new MBudgetPlanType();
                MCompany m_objMCompany = new MCompany();
                MProject m_objMProject = new MProject();
                MCluster m_objMCluster = new MCluster();
                MUnitType m_objMUnitType = new MUnitType();
                MRegion m_objMRegion = new MRegion();
                MLocation m_objMLocation = new MLocation();
                MDivision m_objMDivision = new MDivision();

                if (fieldName == BudgetPlanID.Name)
                    m_strReturn = m_objTBudgetPlan.Name + "." + fieldName;
                else if (fieldName == BudgetPlanTemplateID.Name)
                    m_strReturn = m_objTBudgetPlan.Name + "." + fieldName;
                else if (fieldName == ProjectID.Name)
                    m_strReturn = m_objTBudgetPlan.Name + "." + fieldName;
                else if (fieldName == ClusterID.Name)
                    m_strReturn = m_objTBudgetPlan.Name + "." + fieldName;
                else if (fieldName == UnitTypeID.Name)
                    m_strReturn = m_objTBudgetPlan.Name + "." + fieldName;
                else if (fieldName == BudgetPlanTemplateDesc.Name)
                    m_strReturn = m_objMBudgetPlanTemplate.Name + "." + fieldName;
                else if (fieldName == BudgetPlanVersion.Name)
                    m_strReturn = m_objDBudgetPlanVersion.Name + "." + fieldName;
                else if (fieldName == Area.Name)
                    m_strReturn = m_objDBudgetPlanVersion.Name + "." + fieldName;
                else if (fieldName == Unit.Name)
                    m_strReturn = m_objDBudgetPlanVersion.Name + "." + fieldName;
                else if (fieldName == IsBidOpen.Name)
                    m_strReturn = m_objDBudgetPlanVersion.Name + "." + fieldName;
                else if (fieldName == Description.Name)
                    m_strReturn = m_objDBudgetPlanVersion.Name + "." + fieldName;
                else if (fieldName == StatusID.Name)
                    m_strReturn = m_objDBudgetPlanVersion.Name + "." + fieldName;
                else if (fieldName == StatusDesc.Name)
                    m_strReturn = m_objMStatus.Name + "." + fieldName;
                else if (fieldName == BudgetPlanTypeID.Name)
                    m_strReturn = m_objMBudgetPlanTemplate.Name + "." + fieldName;
                else if (fieldName == BudgetPlanTypeDesc.Name)
                    m_strReturn = m_objMBudgetPlanType.Name + "." + fieldName;

                else if (fieldName == CompanyDesc.Name)
                    m_strReturn = m_objMCompany.Name + "." + fieldName;
                else if (fieldName == RegionDesc.Name)
                    m_strReturn = m_objMRegion.Name + "." + fieldName;
                else if (fieldName == RegionID.Name)
                    m_strReturn = m_objMRegion.Name + "." + fieldName;
                else if (fieldName == LocationDesc.Name)
                    m_strReturn = m_objMLocation.Name + "." + fieldName;
                else if (fieldName == BusinessUnitID.Name)
                    m_strReturn = m_objMDivision.Name + "." + fieldName;
                else if (fieldName == DivisionID.Name)
                    m_strReturn = m_objMDivision.Name + "." + fieldName;
                else if (fieldName == DivisionDesc.Name)
                    m_strReturn = m_objMDivision.Name + "." + fieldName;
                else if (fieldName == ProjectDesc.Name)
                    m_strReturn = m_objMProject.Name + "." + fieldName;
                else if (fieldName == ClusterDesc.Name)
                    m_strReturn = m_objMCluster.Name + "." + fieldName;
                else if (fieldName == UnitTypeDesc.Name)
                    m_strReturn = m_objMUnitType.Name + "." + fieldName;

                else if (fieldName == CreatedDate.Name)
                    m_strReturn = m_objTBudgetPlan.Name + "." + fieldName;
                else if (fieldName == ModifiedDate.Name)
                    m_strReturn = m_objTBudgetPlan.Name + "." + fieldName;

                else if (fieldName == MaxBudgetPlanVersion.Name)
                    m_strReturn = "XBudgetPlanVersion." + fieldName;

                else if (fieldName == BlockNo.Name)
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