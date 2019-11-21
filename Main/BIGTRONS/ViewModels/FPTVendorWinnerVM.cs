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
    public class FPTVendorWinnerVM
    {
        #region Public Property

        [Key, Column(Order = 1)]
        public string VendorWinnerID { get; set; }
        public string FPTID { get; set; }
        public string FPTDescriptions { get; set; }

        public string TaskID { get; set; }
        public string FPTVendorParticipantID { get; set; }
        public string NegotiationEntryID { get; set; }
        public bool IsWinner { get; set; }
        public string LetterNumber { get; set; }
        public decimal BidFee { get; set; }

        public string VendorID { get; set; }
        public string VendorName { get; set; }
        public string VendorAddress { get; set; }
        public string VendorPhone { get; set; }
        public string VendorEmail { get; set; }
        public string BudgetPlanName { get; set; }
        public string BudgetPlanID { get; set; }
        public string ProjectName { get; set; }

        public int StatusID { get; set; }
        public decimal BidValue { get; set; }
        //public decimal BidFeeValue { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string DivisionID { get; set; }
        public string BusinessUnitID { get; set; }
        public string CompanyDesc { get; set; }
        public bool IsSync { get; set; }
        
        public DateTime NegoDate { get; set; }
        public DateTime NegoTime { get; set; }

        #endregion

        #region Public Field Property

        public static class Prop
        {
            public static class VendorWinnerID
            {
                public static string Desc { get { return "ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class FPTID
            {
                public static string Desc { get { return "FPT Ref.ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class IsSync
            {
                public static string Desc { get { return "Is Sync"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }

            public static class FPTDescriptions
            {
                public static string Desc { get { return "FPT Desctription"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class TaskID
            {
                public static string Desc { get { return "Task ID"; } }
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
            public static class ModifiedDate
            {
                public static string Desc { get { return "ModifiedDate"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class IsWinner
            {
                public static string Desc { get { return "IsWinner"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class LetterNumber
            {
                public static string Desc { get { return "LetterNumber"; } }
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
            public static class BusinessUnitID
            {
                public static string Desc { get { return "BusinessUnitID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class CompanyDesc
            {
                public static string Desc { get { return "CompanyDesc"; } }
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
            public static class VendorAddress
            {
                public static string Desc { get { return "Vendor Address"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class VendorPhone
            {
                public static string Desc { get { return "Vendor Phone"; } }
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
            public static class BudgetPlanName
            {
                public static string Desc { get { return "Budget Plan Name"; } }
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
            public static class ProjectName
            {
                public static string Desc { get { return "Project Name"; } }
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
            public static class NegotiationEntryID
            {
                public static string Desc { get { return "Negotiation Entry ID"; } }
                public static string Map { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name); } }
                public static string MapAlias { get { return Map(MethodBase.GetCurrentMethod().DeclaringType.Name, true); } }
                public static string Name { get { return MethodBase.GetCurrentMethod().DeclaringType.Name; } }
            }
            public static class BidFee
            {
                public static string Desc { get { return "Fee"; } }
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
                DFPTVendorWinner m_objDFPTVendorWinner = new DFPTVendorWinner();
                MFPT m_objMFPT = new MFPT();
                MDivision m_objMDivision = new MDivision();
                MBusinessUnit m_objMBusinessUnit = new MBusinessUnit();
                DFPTVendorParticipants m_objDFPTVendorParticipants = new DFPTVendorParticipants();
                CNegotiationConfigurations m_objCNegotiationConfigurations = new CNegotiationConfigurations();
                MVendor m_objMVendor = new MVendor();
                TBudgetPlan m_objTBudgetPlan = new TBudgetPlan();
                MProject m_objMProject = new MProject();
                DBudgetPlanVersion m_objDBudgetPlanVersion = new DBudgetPlanVersion();
                MTasks m_objMTasks = new MTasks();
                DNegotiationBidEntry m_objDNegotiationBidEntry = new DNegotiationBidEntry();
                MCompany m_objMCompany = new MCompany();

                if (fieldName == VendorWinnerID.Name)
                    m_strReturn = m_objDFPTVendorWinner.Name + "." + fieldName;
                else if (fieldName == FPTID.Name)
                    m_strReturn = m_objDFPTVendorWinner.Name + "." + fieldName;
                else if (fieldName == FPTDescriptions.Name)
                    m_strReturn = m_objMFPT.Name + ".Descriptions";
                else if (fieldName == IsSync.Name)
                    m_strReturn = m_objMFPT.Name + "." + fieldName;
                else if (fieldName == TaskID.Name)
                    m_strReturn = m_objDFPTVendorWinner.Name + "." + fieldName;
                else if (fieldName == FPTVendorParticipantID.Name)
                    m_strReturn = m_objDFPTVendorWinner.Name + "." + fieldName;
                else if (fieldName == BidFee.Name)
                    m_strReturn = m_objDFPTVendorWinner.Name + "." + fieldName;
                else if (fieldName == IsWinner.Name)
                    m_strReturn = m_objDFPTVendorWinner.Name + "." + fieldName;
                else if (fieldName == LetterNumber.Name)
                    m_strReturn = m_objDFPTVendorWinner.Name + "." + fieldName;
                else if (fieldName == VendorID.Name)
                    m_strReturn = m_objMVendor.Name + "." + fieldName;
                else if (fieldName == VendorName.Name)
                    m_strReturn = m_objMVendor.Name + "." + "FirstName + ' ' + " + m_objMVendor.Name + "." + "LastName";
                else if (fieldName == VendorAddress.Name)
                    m_strReturn = m_objMVendor.Name + "." + "Street + ' ' + " + m_objMVendor.Name + "." + "Postal";
                else if (fieldName == VendorPhone.Name)
                    m_strReturn = m_objMVendor.Name + "." + "Phone";
                else if (fieldName == VendorEmail.Name)
                    m_strReturn = m_objMVendor.Name + "." + "Email";
                else if (fieldName == BudgetPlanID.Name)
                    m_strReturn = m_objDBudgetPlanVersion.Name + "." + fieldName;
                else if (fieldName == BudgetPlanName.Name)
                    m_strReturn = m_objDBudgetPlanVersion.Name + "." + "Description";
                else if (fieldName == ProjectName.Name)
                    m_strReturn = m_objMProject.Name + "." + "ProjectDesc";
                else if (fieldName == StatusID.Name)
                    m_strReturn = m_objMTasks.Name + "." + fieldName;
                else if (fieldName == NegotiationEntryID.Name)
                    m_strReturn = m_objDNegotiationBidEntry.Name + "." + fieldName;
                else if (fieldName == BidValue.Name)
                    m_strReturn = m_objDNegotiationBidEntry.Name + "." + fieldName;
                else if (fieldName == ModifiedDate.Name)
                    m_strReturn = m_objDFPTVendorWinner.Name + "." + fieldName;
                else if (fieldName == DivisionID.Name)
                    m_strReturn = m_objMDivision.Name + "." + fieldName;
                else if (fieldName == BusinessUnitID.Name)
                    m_strReturn = m_objMBusinessUnit.Name + "." + fieldName;
                else if (fieldName == CompanyDesc.Name)
                    m_strReturn = m_objMCompany.Name + "." + fieldName;


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