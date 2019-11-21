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
    public class BudgetPlanVersionVendorVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string BudgetPlanVersionVendorID { get; set; }
        public string BudgetPlanVersionPeriodID { get; set; }
        public string BudgetPlanID { get; set; }
        public string ScheduleID { get; set; }
        public int BudgetPlanVersion { get; set; }
        public string BudgetPlanPeriodID { get; set; }
        public string VendorID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string VendorDesc
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
        public string PackageID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime StartHours { get; set; }
        public DateTime EndHours { get; set; }
        public string StartDateHours { get; set; }
        public string EndDateHours { get; set; }
        public string Description { get; set; }
        public string BudgetPlanTemplateDesc { get; set; }
        public string ProjectDesc { get; set; }
        public string ProjectID { get; set; }
        public string ClusterDesc { get; set; }
        public string ClusterID { get; set; }
        public string UnitTypeDesc { get; set; }
        public int StatusID { get; set; }
        public int StatusIDVendor { get; set; }
        public int StatusIDPeriod { get; set; }
        public string AvailableVendorID { get; set; }
        public bool AllowDelete { get; set; }
        public string StatusDesc { get; set; }
        public decimal? FeePercentage { get; set; }
        public decimal RoundedTotal { get; set; }
        public int LastPeriodVersion { get; set; }
        public List<BudgetPlanVersionAssignmentVM> ListBudgetPlanVersionAssignmentVM { get; set; }
        public List<string> lst_Vendor
        {
            get
            {
                List<string> bl = new List<string>();
                if (this.ListBudgetPlanVersionAssignmentVM != null)
                {
                    if (this.ListBudgetPlanVersionAssignmentVM.Count > 0)
                    {
                        foreach (BudgetPlanVersionAssignmentVM n in this.ListBudgetPlanVersionAssignmentVM)
                        {
                            bl.Add(n.VendorID);
                        }
                    }
                }
                return bl;
            }
        }
        private string Vendor_;
        public string Vendor
        {
            get
            {
                string Ver = "";
                if (this.ListBudgetPlanVersionAssignmentVM != null)
                    if (this.ListBudgetPlanVersionAssignmentVM.Count > 0 && this.ListBudgetPlanVersionAssignmentVM[0].VendorID != null)
                    {
                        {
                            Ver = string.Empty;
                            foreach (BudgetPlanVersionAssignmentVM n in this.ListBudgetPlanVersionAssignmentVM)
                            {
                                Ver += n.VendorID;
                                Ver += " </br>";
                            }
                        }
                    }
                return Ver;

            }
            set { this.Vendor_ = value; }
        }
        public int PeriodVersion { get; set; }
        public int MaxPeriodVersion { get; set; }

        public bool InPeriod
        {
            get
            {
                if (StartDate <= DateTime.Now && EndDate >= DateTime.Now)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class LastPeriodVersion
            {
                public static string Desc { get { return "Last Period Version"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BudgetPlanVersionVendorID
            {
                public static string Desc { get { return "Budget Plan Version Vendor ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class BudgetPlanVersionPeriodID
            {
                public static string Desc { get { return "Budget Plan Version Period ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class FeePercentage
            {
                public static string Desc { get { return "Fee (%)"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ScheduleID
            {
                public static string Desc { get { return "ScheduleID"; } }
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
            public static class BudgetPlanPeriodID
            {
                public static string Desc { get { return "BudgetPlanPeriodID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class VendorID
            {
                public static string Desc { get { return "VendorID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class VendorDesc
            {
                public static string Desc { get { return "Vendor"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
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

            public static class Email
            {
                public static string Desc { get { return "Email"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class StartDate
            {
                public static string Desc { get { return "Start"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class EndDate
            {
                public static string Desc { get { return "End"; } }
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

            public static class Description
            {
                public static string Desc { get { return "Description"; } }
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

            public static class BudgetPlanTemplateDesc
            {
                public static string Desc { get { return "Template"; } }
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

            public static class StatusID
            {
                public static string Desc { get { return "StatusID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class StatusIDVendor
            {
                public static string Desc { get { return "StatusIDVendor"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class StatusIDPeriod
            {
                public static string Desc { get { return "StatusIDPeriod"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class StatusVendorID
            {
                public static string Desc { get { return "StatusVendorID"; } }
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

            public static class TestTime
            {
                public static string Desc { get { return "TestTime"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class AvailableVendorID
            {
                public static string Desc { get { return "AvailableVendorID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class MaxPeriodVersion
            {
                public static string Desc { get { return "Max Period Version"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class PackageID
            {
                public static string Desc { get { return "Package ID"; } }
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
                DBudgetPlanVersionVendor m_objDBudgetPlanVersionVendor = new DBudgetPlanVersionVendor();
                DBudgetPlanVersion m_objDBudgetPlanVersion = new DBudgetPlanVersion();
                MVendor m_objMVendor = new MVendor();
                MStatus m_objMStatus = new MStatus();
                MProject m_objMProject = new MProject();
                MCluster m_objMCluster = new MCluster();
                MUnitType m_objMUnitType = new MUnitType();
                MBudgetPlanTemplate m_objMBudgetPlanTemplate = new MBudgetPlanTemplate();
                DBudgetPlanVersionEntry m_objVersionEntry = new DBudgetPlanVersionEntry();
                DPackageList m_objPackageList = new DPackageList();

                if (fieldName == BudgetPlanVersionPeriodID.Name)
                    m_strReturn = m_objDBudgetPlanVersionVendor.Name + "." + fieldName;
                else if (fieldName == BudgetPlanVersionVendorID.Name)
                    m_strReturn = m_objDBudgetPlanVersionVendor.Name + "." + fieldName;
                else if (fieldName == FeePercentage.Name)
                    m_strReturn = m_objDBudgetPlanVersionVendor.Name + "." + fieldName;
                else if (fieldName == ScheduleID.Name)
                    m_strReturn = m_objDBudgetPlanVersionVendor.Name + "." + fieldName;
                else if (fieldName == VendorID.Name)
                    m_strReturn = m_objDBudgetPlanVersionVendor.Name + "." + fieldName;
                else if (fieldName == VendorDesc.Name)
                    m_strReturn = "MVendor.FirstName + MVendor.LastName";
                else if (fieldName == StatusVendorID.Name)
                    m_strReturn = m_objDBudgetPlanVersionVendor.Name + ".StatusID";
                else if (fieldName == StatusIDVendor.Name)
                    m_strReturn = m_objDBudgetPlanVersionVendor.Name + ".StatusID";
                else if (fieldName == FirstName.Name)
                    m_strReturn = m_objMVendor.Name + "." + fieldName;
                else if (fieldName == LastName.Name)
                    m_strReturn = m_objMVendor.Name + "." + fieldName;
                else if (fieldName == StartDate.Name)
                    m_strReturn = m_objDBudgetPlanVersionVendor.Name + "." + fieldName;
                else if (fieldName == EndDate.Name)
                    m_strReturn = m_objDBudgetPlanVersionVendor.Name + "." + fieldName;
                else if (fieldName == BudgetPlanTemplateDesc.Name)
                    m_strReturn = m_objMBudgetPlanTemplate.Name + "." + fieldName;
                else if (fieldName == Description.Name)
                    m_strReturn = m_objDBudgetPlanVersion.Name + "." + fieldName;
                else if (fieldName == ProjectDesc.Name)
                    m_strReturn = m_objMProject.Name + "." + fieldName;
                else if (fieldName == ProjectID.Name)
                    m_strReturn = m_objMProject.Name + "." + fieldName;
                else if (fieldName == ClusterDesc.Name)
                    m_strReturn = m_objMCluster.Name + "." + fieldName;
                else if (fieldName == ClusterID.Name)
                    m_strReturn = m_objMCluster.Name + "." + fieldName;
                else if (fieldName == UnitTypeDesc.Name)
                    m_strReturn = m_objMUnitType.Name + "." + fieldName;
                else if (fieldName == UnitTypeDesc.Name)
                    m_strReturn = m_objMUnitType.Name + "." + fieldName;
                else if (fieldName == StatusID.Name)
                    m_strReturn = m_objDBudgetPlanVersion.Name + "." + fieldName;
                else if (fieldName == AvailableVendorID.Name)
                    m_strReturn = m_objVersionEntry.Name + "." + "AvailableVendor";
                else if (fieldName == BudgetPlanID.Name)
                    m_strReturn = m_objDBudgetPlanVersionPeriod.Name + "." + fieldName;
                else if (fieldName == BudgetPlanVersion.Name)
                    m_strReturn = m_objDBudgetPlanVersionPeriod.Name + "." + fieldName;
                else if (fieldName == PackageID.Name)
                    m_strReturn = m_objPackageList.Name + "." + fieldName;
                else if (fieldName == StatusIDPeriod.Name)
                    m_strReturn = m_objDBudgetPlanVersionPeriod.Name + ".StatusID";
                else if (fieldName == StatusDesc.Name)
                    m_strReturn = m_objMStatus.Name + "." + fieldName;
                else if (fieldName == BudgetPlanPeriodID.Name)
                    m_strReturn = m_objDBudgetPlanVersionPeriod.Name + "." + fieldName;
                else if (fieldName == Email.Name)
                    m_strReturn = m_objMVendor.Name + "." + fieldName;
                else if (fieldName == LastPeriodVersion.Name)
                    m_strReturn = "LASTPERIOD.MaxPeriodVersion";

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