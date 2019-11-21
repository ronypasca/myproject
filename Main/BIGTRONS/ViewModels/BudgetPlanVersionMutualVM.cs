using com.SML.Lib.Common;
using com.SML.BIGTRONS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace com.SML.BIGTRONS.ViewModels
{
    public class BudgetPlanVersionMutualVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string BudgetPlanID { get; set; }
        [Key, Column(Order = 2)]
        public int BudgetPlanVersion { get; set; }
        [Key, Column(Order = 3)]
        public string VendorID { get; set; }
        [Key, Column(Order = 4)]
        public string BudgetPlanVersionStructureID { get; set; }
        public string Info { get; set; }
        public decimal? Volume { get; set; }
        public decimal? MaterialAmount { get; set; }
        public decimal? WageAmount { get; set; }
        public decimal? MiscAmount { get; set; }
        public decimal? TotalUnitPrice { get; set; }
        public decimal? Total { get; set; }
        public decimal? FeePercentage { get; set; }
        public string ItemDesc { get; set; }
        public string ItemID { get; set; }
        public int Version { get; set; }
        public string UoMDesc { get; set; }

        public int Sequence { get; set; }

        public string Specification { get; set; }
        public string Description { get; set; }
        public string BudgetPlanTemplateID { get; set; }
        public string BudgetPlanTemplateDesc { get; set; }
        public string ProjectID { get; set; }
        public string ProjectDesc { get; set; }
        public string ClusterID { get; set; }
        public string ClusterDesc { get; set; }
        public string UnitTypeID { get; set; }
        public string UnitTypeDesc { get; set; }
        public int StatusID { get; set; }
        public string StatusDesc { get; set; }
        public string BudgetPlanTypeID { get; set; }
        public string BudgetPlanTypeDesc { get; set; }
        public decimal Area { get; set; }
        public string CompanyDesc { get; set; }
        public string RegionDesc { get; set; }
        public string LocationDesc { get; set; }
        public string DivisionDesc { get; set; }
        public Ext.Net.Node ListBudgetPlanVersionStructureVM { get; set; }
        public Ext.Net.Node ListAdditionalWorkItems { get; set; }
        public List<string> FilterItemTypeID
        {
            get
            {

                List<string> m_lstItemTypeID = new List<string>();

                DataAccess.UConfigDA m_objUConfigDA = new DataAccess.UConfigDA();
                m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;


                Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                List<object> m_lstFilter = new List<object>();

                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(ConfigVM.Prop.Key3.MapAlias);

                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add("ItemTypeID");
                m_objFilter.Add(ConfigVM.Prop.Key1.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add("BudgetPlanTemplate");
                m_objFilter.Add(ConfigVM.Prop.Key2.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add("TRUE");
                m_objFilter.Add(ConfigVM.Prop.Desc1.Map, m_lstFilter);

                Dictionary<int, System.Data.DataSet> m_dicUConfigDA = m_objUConfigDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
                if (m_objUConfigDA.Message == string.Empty)
                {
                    foreach (System.Data.DataRow item in m_dicUConfigDA[0].Tables[0].Rows)
                    {
                        m_lstItemTypeID.Add(item[ConfigVM.Prop.Key3.Name].ToString());
                    }
                }
                m_lstItemTypeID = m_lstItemTypeID.Distinct().ToList();

                return m_lstItemTypeID;
            }
        }
        public string VendorIDList { get; set; }


        public string BudgetPlanVersionVendorID { get; set; }
        public string BudgetPlanVersionPeriodID { get; set; }
        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class TotalUnitPrice
            {
                public static string Desc { get { return "Total Unit Price"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class Sequence
            {
                public static string Desc { get { return "No"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class BudgetPlanVersionVendorID
            {
                public static string Desc { get { return "BudgetPlanVersionVendorID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BudgetPlanVersionPeriodID
            {
                public static string Desc { get { return "BudgetPlanVersionPeriodID"; } }
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
            public static class Total
            {
                public static string Desc { get { return "Total"; } }
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
            public static class ItemID
            {
                public static string Desc { get { return "ItemID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ItemDesc
            {
                public static string Desc { get { return "Structure"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Version
            {
                public static string Desc { get { return "Version"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class UoMDesc
            {
                public static string Desc { get { return "UoM"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Specification
            {
                public static string Desc { get { return "Specification"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BudgetPlanID
            {
                public static string Desc { get { return "ID"; } }
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
            public static class VendorID
            {
                public static string Desc { get { return "Vendor ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            //VendorIDList
            public static class VendorIDList
            {
                public static string Desc { get { return "VendorList"; } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class VendorDesc
            {
                public static string Desc { get { return "Vendor"; } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BudgetPlanVersionStructureID
            {
                public static string Desc { get { return "BudgetPlanVersionStructureID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Info
            {
                public static string Desc { get { return "Info"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Volume
            {
                public static string Desc { get { return "Volume"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class MaterialAmount
            {
                public static string Desc { get { return "Material"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class WageAmount
            {
                public static string Desc { get { return "Wage"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class MiscAmount
            {
                public static string Desc { get { return "Misc"; } }
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
                DBudgetPlanVersionAssignment m_objSBudgetPlanVersionAssignment = new DBudgetPlanVersionAssignment();
                DBudgetPlanVersionMutual m_objSBudgetPlanVersionMutual = new DBudgetPlanVersionMutual();

                DBudgetPlanVersion m_objDBudgetPlanVersion = new DBudgetPlanVersion();
                DBudgetPlanVersionPeriod m_objDBudgetPlanVersionPeriod = new DBudgetPlanVersionPeriod();
                DBudgetPlanVersionVendor m_objDBudgetPlanVersionVendor = new DBudgetPlanVersionVendor();

                if (fieldName == BudgetPlanID.Name)
                    m_strReturn = m_objDBudgetPlanVersionPeriod.Name + "." + fieldName;
                else if (fieldName == BudgetPlanVersion.Name)
                    m_strReturn = m_objDBudgetPlanVersionPeriod.Name + "." + fieldName;
                else if (fieldName == VendorID.Name)
                    m_strReturn = m_objDBudgetPlanVersionVendor.Name + "." + fieldName;
                else if (fieldName == BudgetPlanVersionPeriodID.Name)
                    m_strReturn = m_objDBudgetPlanVersionVendor.Name + "." + fieldName;
                else if (fieldName == BudgetPlanVersionStructureID.Name)
                    m_strReturn = m_objSBudgetPlanVersionMutual.Name + "." + fieldName;
                else if (fieldName == FeePercentage.Name)
                    m_strReturn = m_objSBudgetPlanVersionMutual.Name + "." + fieldName;
                else if (fieldName == Info.Name)
                    m_strReturn = m_objSBudgetPlanVersionMutual.Name + "." + fieldName;
                else if (fieldName == Volume.Name)
                    m_strReturn = m_objSBudgetPlanVersionMutual.Name + "." + fieldName;
                else if (fieldName == MaterialAmount.Name)
                    m_strReturn = m_objSBudgetPlanVersionMutual.Name + "." + fieldName;
                else if (fieldName == WageAmount.Name)
                    m_strReturn = m_objSBudgetPlanVersionMutual.Name + "." + fieldName;
                else if (fieldName == MiscAmount.Name)
                    m_strReturn = m_objSBudgetPlanVersionMutual.Name + "." + fieldName;
                else if (fieldName == StatusID.Name)
                    m_strReturn = m_objDBudgetPlanVersion.Name + "." + fieldName;
                if (fieldName == BudgetPlanVersionVendorID.Name)
                    m_strReturn = m_objSBudgetPlanVersionMutual.Name + "." + fieldName;

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
            PropertyInfo m_pifBudgetPlanVersionEntryVM = this.GetType().GetProperty(fieldName);
            return m_pifBudgetPlanVersionEntryVM != null && Attribute.GetCustomAttribute(m_pifBudgetPlanVersionEntryVM, typeof(KeyAttribute)) != null;
        }
    }
} 