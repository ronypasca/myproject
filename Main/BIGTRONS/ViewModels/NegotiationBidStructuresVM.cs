using com.SML.Lib.Common;
using com.SML.BIGTRONS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;

namespace com.SML.BIGTRONS.ViewModels
{
    public class NegotiationBidStructuresVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string NegotiationBidID { get; set; }
        public int Sequence { get; set; }
        public string VersionStructureID { get; set; }
        public string ItemID { get; set; }
        public string ItemDesc { get; set; }
        public string ItemParentID { get; set; }
        public decimal BudgetPlanDefaultValue { get; set; }
        public int? Version { get; set; }
        public int? ParentVersion { get; set; }
        public int? ParentSequence { get; set; }
        //public string VendorID { get; set; }
        public string VendorDesc { get; set; }
        public string FPTID { get; set; }
        public string ParameterValue { get; set; }
        public string ProjectID { get; set; }
        public string ProjectDesc { get; set; }
        public decimal LastBid { get; set; }
        public decimal Bid { get; set; }
        public decimal LastOffer { get; set; }
        public decimal Mat { get; set; }
        public decimal Wag { get; set; }
        public decimal Misc { get; set; }
        public decimal Vol { get; set; }

        public bool IsProposed { get; set; }
        public string Remarks { get; set; }
        public string NegotiationConfigID { get; set; }
        public string FPTVendorParticipantID { get; set; }
        public string ParameterValue2 { get; set; }
        public string VendorID { get; set; }
        public List<NegotiationBidEntryVM> VendorDefaultEntry { get; set; }
        public string BPVersionName { get; set; }

        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class NegotiationBidID
            {
                public static string Desc { get { return "ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Sequence
            {
                public static string Desc { get { return "Sequence"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ItemID
            {
                public static string Desc { get { return "Item ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ItemDesc
            {
                public static string Desc { get { return "Sub Item"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ItemParentID
            {
                public static string Desc { get { return "Item Parent ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BudgetPlanDefaultValue
            {
                public static string Desc { get { return "RAB"; } }
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
            public static class ParentVersion
            {
                public static string Desc { get { return "Parent Version"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ParentSequence
            {
                public static string Desc { get { return "Parent Sequence"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            //public static class VendorID
            //{
            //    public static string Desc { get { return "Vendor ID"; } }
            //    public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
            //    public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
            //    public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            //}
            //public static class VendorDesc
            //{
            //    public static string Desc { get { return "Vendor Name"; } }
            //    public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
            //    public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
            //    public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            //}
            public static class FPTID
            {
                public static string Desc { get { return "FPT ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ParameterValue
            {
                public static string Desc { get { return "Parameter Value"; } }
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
                public static string Desc { get { return "Project Desc"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class LastBid
            {
                public static string Desc { get { return "Last Bid"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class LastOffer
            {
                public static string Desc { get { return "Last Offer"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class Bid
            {
                public static string Desc { get { return "Bid"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class NegotiationConfigID
            {
                public static string Desc { get { return "NegotiationConfigID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            //public static class FPTVendorParticipantID
            //{
            //    public static string Desc { get { return "FPTVendorParticipantID"; } }
            //    public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
            //    public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
            //    public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            //}
            public static class ParameterValue2
            {
                public static string Desc { get { return "Parameter Value"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BPVersionName
            {
                public static string Desc { get { return "BP Name"; } }
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
                TNegotiationBidStructures m_objTNegotiationBidStructures = new TNegotiationBidStructures();
                CNegotiationConfigurations m_objCNegotiationConfigurations = new CNegotiationConfigurations();
                //DFPTVendorParticipants m_objDFPTVendorParticipants = new DFPTVendorParticipants();
                TBudgetPlan m_objTBudgetPlan = new TBudgetPlan();
                MProject m_objMProject = new MProject();
                DBudgetPlanVersion m_objDBudgetPlanVersion = new DBudgetPlanVersion();

                if (fieldName == NegotiationBidID.Name)
                    m_strReturn = m_objTNegotiationBidStructures.Name + "." + fieldName;
                else if (fieldName == NegotiationConfigID.Name)
                    m_strReturn = m_objTNegotiationBidStructures.Name + "." + fieldName;
                else if (fieldName == Sequence.Name)
                    m_strReturn = m_objTNegotiationBidStructures.Name + "." + fieldName;
                else if (fieldName == ItemID.Name)
                    m_strReturn = m_objTNegotiationBidStructures.Name + "." + fieldName;
                else if (fieldName == ItemDesc.Name)
                    m_strReturn = m_objTNegotiationBidStructures.Name + "." + fieldName;
                else if (fieldName == ItemParentID.Name)
                    m_strReturn = m_objTNegotiationBidStructures.Name + "." + fieldName;
                else if (fieldName == BudgetPlanDefaultValue.Name)
                    m_strReturn = m_objTNegotiationBidStructures.Name + "." + fieldName;
                else if (fieldName == Version.Name)
                    m_strReturn = m_objTNegotiationBidStructures.Name + "." + fieldName;
                else if (fieldName == ParentVersion.Name)
                    m_strReturn = m_objTNegotiationBidStructures.Name + "." + fieldName;
                else if (fieldName == ParentSequence.Name)
                    m_strReturn = m_objTNegotiationBidStructures.Name + "." + fieldName;
                else if (fieldName == FPTID.Name)
                    m_strReturn = m_objCNegotiationConfigurations.Name + "." + fieldName;
                else if (fieldName == ParameterValue.Name)
                    m_strReturn = m_objCNegotiationConfigurations.Name + "." + fieldName;
                else if (fieldName == ProjectID.Name)
                    m_strReturn = m_objMProject.Name + "." + fieldName;
                else if (fieldName == ProjectDesc.Name)
                    m_strReturn = m_objMProject.Name + "." + fieldName;
                //else if (fieldName == FPTVendorParticipantID.Name)
                //    m_strReturn = m_objDFPTVendorParticipants.Name + "." + fieldName;
                else if (fieldName == ParameterValue2.Name)
                    m_strReturn = m_objCNegotiationConfigurations.Name + "." + fieldName;
                else if (fieldName == BPVersionName.Name)
                    m_strReturn = m_objDBudgetPlanVersion.Name + ".Description";
                

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