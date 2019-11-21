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
    public class FPTVendorParticipantsVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string FPTVendorParticipantID { get; set; }
        public string VendorID { get; set; }
        public string VendorName { get; set; }
        public string FPTID { get; set; }
        public string StatusID { get; set; }

        public string ProjectDesc { get; set; }
        public string BudgetPlanID { get; set; }
        public string FirstName { get; set; }
        public string ProjectID { get; set; }
        public string NegotiationConfigID { get; set; }
        public bool Checked { get; set; }
        public string ParameterValue { get; set; }
        public decimal BidValue { get; set; }
        public decimal BidFee { get; set; }
        public decimal BidAfterFee { get; set; }
        public string NegotiationEntryID { get; set; }
        public decimal BudgetPlanDefaultValue { get; set; }
        public decimal BudgetPlanDefaultFee { get; set; }
        public decimal BudgetPlanDefaultValueAfterFee { get; set; }
        public decimal LastOffer { get; set; }

        public bool IsProposed { get; set; }
        public string IsProposedWinner { get; set; }
        public string TCName { get; set; }
        public string RecommendationRemark { get; set; }
        public bool IsWinner { get; set; }
        public string BPVersionName { get; set; }
        public string RoundID { get; set; }
        public string VendorEmail { get; set; }
        public string IsAttend { get; set; }

        public DateTime NegoDate { get; set; }
        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class FPTVendorParticipantID
            {
                public static string Desc { get { return "ID"; } }
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
            public static class VendorName
            {
                public static string Desc { get { return "Vendor Name"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class FPTID
            {
                public static string Desc { get { return "FPT ID"; } }
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

            public static class ProjectDesc
            {
                public static string Desc { get { return "Project Desc"; } }
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
            public static class ProjectID
            {
                public static string Desc { get { return "Project ID"; } }
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

            public static class NegotiationConfigID
            {
                public static string Desc { get { return "NegotiationConfigID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class NegotiationConfigTypeID
            {
                public static string Desc { get { return "NegotiationConfigTypeID"; } }
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
            public static class ParameterValue2
            {
                public static string Desc { get { return "ParameterValue"; } }
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
            public static class VendorEmail
            {
                public static string Desc { get { return "Vendor Email"; } }
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
                DFPTVendorParticipants m_objDFPTVendorParticipants = new DFPTVendorParticipants();
                CNegotiationConfigurations m_objCNegotiationConfigurations = new CNegotiationConfigurations();
                MVendor m_objMVendor = new MVendor();
                TBudgetPlan m_objTBudgetPlan = new TBudgetPlan();
                MProject m_objMProject = new MProject();
                DBudgetPlanVersion m_objDBudgetPlanVersion = new DBudgetPlanVersion();

                if (fieldName == FPTVendorParticipantID.Name)
                    m_strReturn = m_objDFPTVendorParticipants.Name + "." + fieldName;
                else if (fieldName == VendorID.Name)
                    m_strReturn = m_objDFPTVendorParticipants.Name + "." + fieldName;
                else if (fieldName == VendorName.Name)
                    m_strReturn = m_objMVendor.Name + "." + "FirstName + ' ' + " + m_objMVendor.Name + "." + "LastName";
                else if (fieldName == FPTID.Name)
                    m_strReturn = m_objCNegotiationConfigurations.Name + "." + fieldName;
                else if (fieldName == StatusID.Name)
                    m_strReturn = m_objDFPTVendorParticipants.Name + "." + fieldName;
                else if (fieldName == ProjectDesc.Name)
                    m_strReturn = m_objMProject.Name + "." + fieldName;
                else if (fieldName == FirstName.Name)
                    m_strReturn = m_objMVendor.Name + "." + fieldName;
                else if (fieldName == VendorEmail.Name)
                    m_strReturn = m_objMVendor.Name + "." + "Email";
                else if (fieldName == StatusID.Name)
                    m_strReturn = m_objDFPTVendorParticipants.Name + "." + fieldName;
                else if (fieldName == ProjectID.Name)
                    m_strReturn = m_objMProject.Name + "." + fieldName;
                else if (fieldName == BudgetPlanID.Name)
                    m_strReturn = m_objTBudgetPlan.Name + "." + fieldName;
                else if (fieldName == NegotiationConfigID.Name)
                    m_strReturn = m_objDFPTVendorParticipants.Name + "." + fieldName;
                else if (fieldName == NegotiationConfigTypeID.Name)
                    m_strReturn = m_objCNegotiationConfigurations.Name + "." + fieldName;
                else if (fieldName == ParameterValue.Name)
                    m_strReturn = m_objCNegotiationConfigurations.Name + "." + fieldName;
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
            PropertyInfo m_pifTaskTypesVM = this.GetType().GetProperty(fieldName);
            return m_pifTaskTypesVM != null && Attribute.GetCustomAttribute(m_pifTaskTypesVM, typeof(KeyAttribute)) != null;
        }
    }
}