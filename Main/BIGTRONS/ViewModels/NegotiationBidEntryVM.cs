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
    public class NegotiationBidEntryVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string NegotiationEntryID { get; set; }
        public string BidTypeID { get; set; }
        public string NegotiationBidID { get; set; }
        public string RoundID { get; set; }
        public decimal BidValue { get; set; }
        public string VendorID { get; set; }
        public string VendorDesc { get; set; }
        public string FPTID { get; set; }
        public string FPTVendorParticipantID { get; set; }
        public string ParameterValue { get; set; }
        public string ProjectID { get; set; }
        public string ProjectDesc { get; set; }
        public bool IsProposed { get; set; }
        public string Remarks { get; set; }
        public decimal BudgetPlanDefaultValue { get; set; }
        public int? Sequence { get; set; }
        public string BudgetPlanID { get; set; }

        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class NegotiationEntryID
            {
                public static string Desc { get { return "ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BidTypeID
            {
                public static string Desc { get { return "Bid Type ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class NegotiationBidID
            {
                public static string Desc { get { return "NegotiationBidID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class RoundID
            {
                public static string Desc { get { return "RoundID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BidValue
            {
                public static string Desc { get { return "BidValue"; } }
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
                public static string Desc { get { return "Vendor Desc"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            
            public static class FPTID
            {
                public static string Desc { get { return "FPTID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class FPTVendorParticipantID
            {
                public static string Desc { get { return "FPTVendorParticipantID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class ParameterValue
            {
                public static string Desc { get { return "ParameterValue"; } }
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
            public static class BudgetPlanDefaultValue
            {
                public static string Desc { get { return "Budget Plan Default Value"; } }
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
            public static class BudgetPlanID
            {
                public static string Desc { get { return "BudgetPlanID"; } }
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
                DNegotiationBidEntry m_objDNegotiationBidEntry = new DNegotiationBidEntry();
                TNegotiationBidStructures m_objTNegotiationBidStructures = new TNegotiationBidStructures();
                DFPTVendorParticipants m_objDFPTVendorParticipants = new DFPTVendorParticipants();
                CNegotiationConfigurations m_objCNegotiationConfigurations = new CNegotiationConfigurations();
                TBudgetPlan m_objTBudgetPlan = new TBudgetPlan();
                MProject m_objMProject = new MProject();
                MVendor m_objMVendor = new MVendor();

                if (fieldName == NegotiationEntryID.Name)
                    m_strReturn = m_objDNegotiationBidEntry.Name + "." + fieldName;
                else if (fieldName == BidTypeID.Name)
                    m_strReturn = m_objDNegotiationBidEntry.Name + "." + fieldName;
                else if (fieldName == NegotiationBidID.Name)
                    m_strReturn = m_objDNegotiationBidEntry.Name + "." + fieldName;
                else if (fieldName == RoundID.Name)
                    m_strReturn = m_objDNegotiationBidEntry.Name + "." + fieldName;
                else if (fieldName == BidValue.Name)
                    m_strReturn = m_objDNegotiationBidEntry.Name + "." + fieldName;
                else if (fieldName == FPTVendorParticipantID.Name)
                    m_strReturn = m_objDNegotiationBidEntry.Name + "." + fieldName;
                else if (fieldName == VendorID.Name)
                    m_strReturn = m_objDFPTVendorParticipants.Name + "." + fieldName;
                else if (fieldName == VendorDesc.Name)
                    m_strReturn = m_objMVendor.Name + ".FirstName + ' ' + " + m_objMVendor.Name + ".LastName";
                else if (fieldName == FPTID.Name)
                    m_strReturn = m_objCNegotiationConfigurations.Name + "." + fieldName;
                else if (fieldName == ParameterValue.Name)
                    m_strReturn = m_objCNegotiationConfigurations.Name + "." + fieldName;
                else if (fieldName == ProjectID.Name)
                    m_strReturn = m_objMProject.Name + "." + fieldName;
                else if (fieldName == ProjectDesc.Name)
                    m_strReturn = m_objMProject.Name + "." + fieldName;
                else if (fieldName == BudgetPlanDefaultValue.Name)
                    m_strReturn = m_objTNegotiationBidStructures.Name + "." + fieldName;
                else if (fieldName == Sequence.Name)
                    m_strReturn = m_objTNegotiationBidStructures.Name + "." + fieldName;
                else if (fieldName == BudgetPlanID.Name)
                    m_strReturn = m_objTBudgetPlan.Name + "." + fieldName;

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