using com.SML.BIGTRONS.Models;
using com.SML.BIGTRONS.ViewModels;
using com.SML.Lib.Common;
using com.SML.BIGTRONS.Controllers;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using CrystalDecisions.Web;
using System.Web;
using System.Web.Services;
using System.Drawing.Printing;
using System.Web.UI.WebControls;
using com.SML.BIGTRONS.Enum;
using com.SML.BIGTRONS;

namespace com.SML.BIGTRONS.Reports
{
    public partial class Viewer : System.Web.UI.Page
    {
        private readonly string _strReportMaterialGroupAvailability = "/MaterialGroup/Availability.rpt";
        private readonly string _strReportMaterialGroupOccupancy = "/MaterialGroup/Occupancy.rpt";
        private readonly string _strReportMaterialGroupStockSummary = "/MaterialGroup/StockSummary.rpt";
        private readonly string _strReportReceiptPenerimaanSewa = "/Receipt/DReceipt_Penerimaan_Sewa.rpt";
        private readonly string _strReportReceiptPenerimaanBulanan = "/Receipt/DReceipt_Penerimaan_Bulanan.rpt";
        private readonly string _strReportReceiptSummaryElectricity = "/Receipt/ReceiptElectricity.rpt";

        private string _strReportUID = string.Empty;

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Request.UrlReferrer == null)
                Response.Redirect("~/ReportViewer");

            _strReportUID = Request.QueryString["ReportUID"];
            if (!IsPostBack)
            {
                //if (Session["rpt" + _strReportUID] != null)
                //{
                //    ((ReportDocument)Session["rpt" + _strReportUID]).Dispose();
                //    Session["rpt" + _strReportUID] = null;
                //    GC.Collect();
                //    GC.WaitForPendingFinalizers();
                //}
                LoadReport();
            }

            ReportViewer.ReportSource = (ReportDocument)Session["rpt" + _strReportUID];
            ReportViewer.RefreshReport();
            //ReportViewer.DataBind();
        }

        //protected void Page_Unload(object sender, EventArgs e)
        //{
        //    ReportDocument _rpdReport = (ReportDocument)Session["rpt" + _strReportUID];
        //    _rpdReport.Close();
        //    _rpdReport.Dispose();
        //    ReportViewer.Dispose();
        //}

        //public string DoPrint(string printerName)
        //{
        //    string m_strReturn = string.Empty;
        //    try
        //    {
        //        ReportDocument _rpdReport = (ReportDocument)Session["rpt" + _strReportUID];
        //        _rpdReport.PrintOptions.PrinterName = printerName;
        //        _rpdReport.PrintToPrinter(1, false, 0, 0);
        //    }
        //    catch (Exception ex)
        //    {
        //        m_strReturn = ex.Message;
        //    }
        //    return m_strReturn;
        //}

        private void LoadReport()
        {
            DateTime m_dteDate, m_dtePeriodDate, m_dtStarDate, m_dtEndDate;
            string m_strOnlyInPeriod = string.Empty;
            string m_strPeriod = string.Empty;
            string m_strPlantID = string.Empty;
            string m_strCustomerID;
            string m_strProductID;
            string m_strMaterialID;
            string m_strDebtWarningID;
            string m_strPurchasingDocument;
            string m_strMaterialDocument;
            string m_strReservationDocument;
            string m_strVendorID;
            string m_strCompanyID;
            string m_strPaymentID;
            string m_strMessage = string.Empty;
            string m_strKeyAccount = string.Empty;
            string m_strStatus = string.Empty;
            string m_strMaterialTypeID = string.Empty;
            string m_strReceiptTypeID = string.Empty;
            string m_strParameterID = string.Empty;
            ConfigController m_objConfigController = new ConfigController();
            List<ConfigVM> m_lstConfigVM = new List<ConfigVM>();
            string m_strContractID;
            string m_strProductGroupID = string.Empty;
            string m_strReportID = Request.QueryString["ReportID"];

            Session["rpt" + _strReportUID] = new ReportDocument();
            switch (m_strReportID)
            {
                //case "PrintMaterialGroupAvailability":
                //    m_strPeriod = Request.QueryString["Period"];
                //    m_strPlantID = Request.QueryString["PlantID"];
                //    MaterialGroupAvailability(m_strPeriod, m_strPlantID);
                //    break;
                //case "PrintMaterialGroupOccupancy":
                //    m_strPeriod = Request.QueryString["Period"];
                //    MaterialGroupOccupancy(m_strPeriod);
                //    break;
                //case "PrintMaterialGroupStockSummary":
                //    m_strPeriod = Request.QueryString["Period"];
                //    m_strPlantID = Request.QueryString["PlantID"];
                //    MaterialGroupStockSummary(m_strPeriod, m_strPlantID);
                //    break;
                //case "PrintMasaBerakhirSewa":
                //    m_strCustomerID = Request.QueryString["CustomerID"];
                //    m_strPeriod = Request.QueryString["BillDate"];
                //    m_strPlantID = Request.QueryString["PlantID"];
                //    MasaBerakhirSewa(m_strPeriod, m_strPlantID, m_strCustomerID);
                //    break;
                //case "PrintRentDetail":
                //    m_strCustomerID = Request.QueryString["CustomerID"];
                //    m_strPeriod = Request.QueryString["BillDate"];
                //    m_strPlantID = Request.QueryString["PlantID"];
                //    RentalDetail(m_strPeriod, m_strPlantID, m_strCustomerID);
                //    break;
                //case "PrintBillFine":
                //    m_strCustomerID = Request.QueryString["CustomerID"];
                //    m_strPeriod = Request.QueryString["BillDate"];
                //    m_strPlantID = Request.QueryString["PlantID"];
                //    bool m_bolBillType = bool.Parse(Request.QueryString["BillType"]);
                //    if (m_bolBillType)
                //        BillTypeRent(m_strPeriod, m_strPlantID, m_strCustomerID);
                //    else
                //        BillTypeNonRent(m_strPeriod, m_strPlantID, m_strCustomerID);
                //    break;
                //case "PrintDailyReconciliation":
                //    m_dtStarDate = DateTime.Parse(Request.QueryString["StartDate"]);
                //    m_dtEndDate = DateTime.Parse(Request.QueryString["EndDate"]);
                //    m_strPlantID = Request.QueryString["PlantID"];
                //    m_strProductGroupID = Request.QueryString["ProductGroupID"];
                //    DailyReconciliation(m_strPlantID, m_dtStarDate, m_dtEndDate, m_strProductGroupID);

                //    break;
                //case "PrintRekapMeterReading":
                //    m_dtePeriodDate = DateTime.Parse(Request.QueryString["Period"]);
                //    m_strPlantID = Request.QueryString["PlantID"];
                //    m_strMaterialTypeID = Request.QueryString["ProductGroupID"];
                //    RekapMeterReading(m_strPlantID, m_dtePeriodDate, m_strMaterialTypeID);

                //    break;

                //case "PrintBillOutstanding":
                //    DateTime m_dtePaymentDate;
                //    m_dtePeriodDate = DateTime.Parse(Request.QueryString["Period"]);
                //    m_dtePaymentDate = DateTime.Parse(Request.QueryString["Date"]);
                //    m_strPlantID = Request.QueryString["PlantID"];
                //    m_strMaterialTypeID = Request.QueryString["MaterialTypeID"];
                //    BillOutstanding(m_dtePeriodDate, m_dtePaymentDate, m_strPlantID, m_strMaterialTypeID);
                //    break;

                //case "PrintBillOutstandingReceipt":
                //    DateTime m_dtePaymentDateREceipt;
                //    m_dtePeriodDate = DateTime.Parse(Request.QueryString["Period"]);
                //    m_dtePaymentDateREceipt = DateTime.Parse(Request.QueryString["Date"]);
                //    m_strPlantID = Request.QueryString["PlantID"];
                //    m_strMaterialTypeID = Request.QueryString["MaterialTypeID"];
                //    BillOutstanding(m_dtePeriodDate, m_dtePaymentDateREceipt, m_strPlantID, m_strMaterialTypeID);
                //    break;

                //case "PrintBillOutstandingRecap":
                //    DateTime m_dtePaymentDateRecap;
                //    m_dtePeriodDate = DateTime.Parse(Request.QueryString["Period"]);
                //    m_dtePaymentDateRecap = DateTime.Parse(Request.QueryString["Date"]);
                //    m_strPlantID = Request.QueryString["PlantID"];
                //    m_strMaterialTypeID = Request.QueryString["MaterialTypeID"];
                //    BillOutstandingRecap(m_dtePeriodDate, m_dtePaymentDateRecap, m_strPlantID, m_strMaterialTypeID);
                //    break;
                //case "PrintBillOutstandingRecapReceipt":
                //    DateTime m_dtePaymentDateRecapReceipt;
                //    m_dtePeriodDate = DateTime.Parse(Request.QueryString["Period"]);
                //    m_dtePaymentDateRecapReceipt = DateTime.Parse(Request.QueryString["Date"]);
                //    m_strPlantID = Request.QueryString["PlantID"];
                //    m_strMaterialTypeID = Request.QueryString["MaterialTypeID"];
                //    //BillOutstandingRecapReceipt(m_dtePeriodDate, m_dtePaymentDateRecapReceipt, m_strPlantID, m_strMaterialTypeID);
                //    BillOutstandingRecap(m_dtePeriodDate, m_dtePaymentDateRecapReceipt, m_strPlantID, m_strMaterialTypeID);
                //    break;
                //case "PrintBillCreditDetail":
                //    m_dteDate = DateTime.Parse(Request.QueryString["Date"]);
                //    m_strCustomerID = Request.QueryString["CustomerID"];
                //    m_strMaterialID = Request.QueryString["MaterialID"];
                //    BillCreditDetail(m_dteDate, m_strCustomerID, m_strMaterialID);
                //    break;
                //case "PrintBillCreditSummary":
                //    m_dteDate = DateTime.Parse(Request.QueryString["Date"]);
                //    m_strCustomerID = Request.QueryString["CustomerID"];
                //    m_strMaterialID = Request.QueryString["MaterialID"];
                //    BillCreditSummary(m_dteDate, m_strCustomerID, m_strMaterialID);
                //    break;
                //case "PrintDebtWarning":
                //    string m_strRealReportID = Request.QueryString["RealReportID"];
                //    m_strCustomerID = Request.QueryString["CustomerID"];
                //    m_strPlantID = Request.QueryString["PlantID"];
                //    m_strDebtWarningID = Request.QueryString["DebtWarningID"];
                //    BillDebtWarning(m_strRealReportID, m_strCustomerID, m_strPlantID, m_strDebtWarningID);
                //    break;
                //case "PrintPurchaseOrder":
                //    m_strPurchasingDocument = Request.QueryString["PurchasingDocument"];
                //    m_strVendorID = Request.QueryString["VendorID"];
                //    m_strCompanyID = Request.QueryString["CompanyID"];
                //    m_strPlantID = Request.QueryString["PlantID"];
                //    PurchaseOrder(m_strPurchasingDocument, m_strVendorID, m_strCompanyID, m_strPlantID);
                //    break;
                //case "PrintReservationDocument":
                //    m_strReservationDocument = Request.QueryString["ReservationDocument"];
                //    ReservationDocument(m_strReservationDocument);
                //    break;
                //case "PrintPurchaseRequisitionDocument":
                //    m_strReservationDocument = Request.QueryString["PurchaseRequisition"];
                //    PurchaseRequisitionDocument(m_strReservationDocument);
                //    break;
                //case "PrintOutlineAgreement":
                //    m_strPurchasingDocument = Request.QueryString["PurchasingDocument"];
                //    m_strVendorID = Request.QueryString["VendorID"];
                //    m_strCompanyID = Request.QueryString["CompanyID"];
                //    m_strPlantID = Request.QueryString["PlantID"];
                //    OutlineAgreement(m_strPurchasingDocument, m_strVendorID, m_strCompanyID, m_strPlantID);
                //    break;
                //case "PrintGR":
                //    m_strMaterialDocument = Request.QueryString["MaterialDocument"];
                //    MaterialDocument(m_strMaterialDocument);
                //    break;
                //case "PrintMatDocList":
                //    m_strMaterialDocument = Request.QueryString["MaterialDocument"];
                //    MaterialDocument(m_strMaterialDocument);
                //    break;
                //case "PrintRFQ":
                //    m_strPurchasingDocument = Request.QueryString["PurchasingDocument"];
                //    m_strVendorID = Request.QueryString["VendorID"];
                //    m_strCompanyID = Request.QueryString["CompanyID"];
                //    m_strPlantID = Request.QueryString["PlantID"];
                //    RequestForQuotation(m_strPurchasingDocument, m_strVendorID, m_strCompanyID, m_strPlantID);
                //    break;
                //case "PrintRentalAdmission":
                //    m_strPeriod = Request.QueryString["Period"];
                //    m_strPlantID = Request.QueryString["PlantID"];
                //    m_strCustomerID = Request.QueryString["CustomerID"];
                //    m_strOnlyInPeriod = Request.QueryString["PeriodRange"];
                //    RentalAdmission(m_strPeriod, m_strPlantID, m_strCustomerID, m_strOnlyInPeriod);
                //    break;
                //case "PrintRentalAdmissionPerBlock":
                //    m_strPeriod = Request.QueryString["Period"];
                //    m_strPlantID = Request.QueryString["PlantID"];
                //    m_strOnlyInPeriod = Request.QueryString["PeriodRange"];
                //    RentalAdmissionPerBlock(m_strPeriod, m_strPlantID, m_strOnlyInPeriod);
                //    break;
                //case "PrintRentalCardReceivables":
                //    m_strPlantID = Request.QueryString["PlantID"];
                //    m_strContractID = Request.QueryString["ContractID"];
                //    m_strCustomerID = Request.QueryString["CustomerID"];
                //    RentalCardReceivables(m_strPlantID, m_strContractID, m_strCustomerID);
                //    break;
                //case "PrintBillSchedule":
                //    DateTime m_dtStart = String.IsNullOrEmpty(Request.QueryString["StartDate"]) ? DateTime.MinValue : DateTime.Parse(Request.QueryString["StartDate"]);
                //    DateTime m_dtEnd = String.IsNullOrEmpty(Request.QueryString["EndDate"]) ? DateTime.MinValue : DateTime.Parse(Request.QueryString["EndDate"]);
                //    m_strPlantID = Request.QueryString["PlantID"];
                //    BillSchedule(m_dtStart, m_dtEnd, m_strPlantID);
                //    break;
                //case "PrintSuratPesananSewa":
                //    string m_strSessionContractID = Request.QueryString["SessionContractID"];
                //    List<string> m_lstContractIDList = new List<string>();
                //    m_lstContractIDList = (List<string>)Session[m_strSessionContractID];
                //    SuratPesananSewa(m_lstContractIDList);
                //    break;
                //case "PrintOffering":
                //    string m_strSessionOfferingID = Request.QueryString["SessionOfferingID"];
                //    List<string> m_lstOfferingIDList = new List<string>();
                //    m_lstOfferingIDList = (List<string>)Session[m_strSessionOfferingID];
                //    Offering(m_lstOfferingIDList);
                //    break;
                //case "PrintPhysicalInventoryDocument":
                //    string m_strPIDocument = Request.QueryString["PIDocument"];
                //    PhysicalInventoryDocument(m_strPIDocument);
                //    break;
            }
            //DBConfigBL m_objDBConfigBL = new DBConfigBL();
            //m_objDBConfigBL.ConnectionStringName = Global.ConnStrConfigName;
            //SetDatabaseLogon(m_objDBConfigBL.UserID, m_objDBConfigBL.Password, m_objDBConfigBL.ServerName, m_objDBConfigBL.DatabaseName);
            //return _rpdReport;
        }

        private void SetDatabaseLogon(string userID, string password, string serverName, string dbName)
        {
            ((ReportDocument)Session["rpt" + _strReportUID]).SetDatabaseLogon(userID, password, serverName, dbName, false);
            foreach (ReportDocument m_rpdSubReport in ((ReportDocument)Session["rpt" + _strReportUID]).Subreports)
                m_rpdSubReport.SetDatabaseLogon(userID, password, serverName, dbName, false);
        }

        //#region AJAX Method
        [WebMethod(EnableSession = true)]
        public static void UnloadReport(string reportUID)
        {
            if (HttpContext.Current.Session["rpt" + reportUID] != null)
            {
                ((ReportDocument)HttpContext.Current.Session["rpt" + reportUID]).Dispose();
                HttpContext.Current.Session["rpt" + reportUID] = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        //[WebMethod()]
        //public static Dictionary<string, string> GetPrinterList()
        //{
        //    Dictionary<string, string> m_dicReturn = new Dictionary<string, string>();
        //    ManagementScope m_mgsPrinter = new ManagementScope();
        //    m_mgsPrinter.Connect();
        //    SelectQuery m_objQuery = new SelectQuery("SELECT * FROM Win32_Printer");
        //    ManagementObjectSearcher m_mosPrinter = new ManagementObjectSearcher(m_mgsPrinter, m_objQuery);
        //    ManagementObjectCollection m_mocPrinter = m_mosPrinter.Get();

        //    foreach (ManagementObject m_mgoPrinter in m_mocPrinter)
        //    {
        //        string m_strPrinterName = m_mgoPrinter["Name"].ToString();
        //        if (m_strPrinterName.Contains("\\"))
        //            m_strPrinterName = m_strPrinterName.Substring(m_strPrinterName.LastIndexOf("\\") + 1);
        //        m_dicReturn.Add(m_strPrinterName, m_strPrinterName);
        //    }
        //    return m_dicReturn;
        //}

        //[WebMethod()]
        //public static string PrintReport(string printerName)
        //{
        //    Viewer m_objViewer = new Viewer();
        //    return m_objViewer.DoPrint(printerName);
        //}
        //#endregion
        

        //#region Report Bill
        //private void BillTypeRent(string period, string plantID, string customerID)
        //{
        //    string reportPath = Server.MapPath("~/" + Global.ReportFolder + _strReportBillRentSell);
        //    ((ReportDocument)Session["rpt" + _strReportUID]).Load(reportPath);

        //    Dictionary<string, List<object>> m_objFilter = FilterBillTypeRent(period, plantID, customerID);
        //    TBillBL m_objTBillBL = new TBillBL();
        //    m_objTBillBL.ConnectionStringName = Global.ConnStrConfigName;
        //    Dictionary<int, DataSet> m_dicTBillBL = m_objTBillBL.SelectBillRentAndSellReport(m_objFilter, null);
        //    DataTable m_dtTBillRent;
        //    if (m_objTBillBL.Success)
        //    {
        //        m_dtTBillRent = m_dicTBillBL[0].Tables[0];
        //        ((ReportDocument)Session["rpt" + _strReportUID]).Database.Tables[0].SetDataSource(m_dtTBillRent);
        //    }
        //}

        //private void BillTypeNonRent(string period, string plantID, string customerID)
        //{
        //    //using (ReportDocument m_rpdReport = new ReportDocument())
        //    //{
        //    string reportPath = Server.MapPath("~/" + Global.ReportFolder + _strReportBillNonRent);
        //    ((ReportDocument)Session["rpt" + _strReportUID]).Load(reportPath);

        //    Dictionary<int, DataSet> m_dicTBillBL = GetBillReportData(period, plantID, customerID);
        //    if (m_dicTBillBL[0].Tables.Count > 0)
        //    {
        //        DataTable m_dtTBillBL = m_dicTBillBL[0].Tables[0];
        //        m_dtTBillBL.TableName = ((ReportDocument)Session["rpt" + _strReportUID]).Database.Tables[0].Name;
        //        ((ReportDocument)Session["rpt" + _strReportUID]).SetDataSource(m_dtTBillBL);

        //        //Dictionary<int, DataSet> m_dicTBillVA = GetBillReportVA(period, plantID, customerID);
        //        //if (m_dicTBillVA[0].Tables.Count > 0)
        //        //{
        //        //    DataTable m_dtTBillVA = m_dicTBillVA[0].Tables[0];
        //        //    m_dtTBillVA.TableName = ((ReportDocument)Session["rpt" + _strReportUID]).Subreports[0].Database.Tables[0].Name;
        //        //    ((ReportDocument)Session["rpt" + _strReportUID]).Subreports[0].SetDataSource(m_dtTBillVA);
        //        //}
        //    }
        //}

        //private void RekapBill(string ProductDesc, DateTime BillDate, string PlantID, string MaterialTypeID, string ParameterID)
        //{
        //    BaseController m_objBaseController = new BaseController();
        //    string reportPath = string.Empty;
        //    string strMessage = string.Empty;
        //    decimal m_decBebanPemakaian = 0;
        //    switch (ProductDesc)
        //    {
        //        case "Electricity": reportPath = Server.MapPath("~/" + Global.ReportFolder + _strReportRekapBillElectricity); break;
        //        case "ServiceCharge": reportPath = Server.MapPath("~/" + Global.ReportFolder + _strReportRekapBillServiceCharge); break;
        //        case "Water": reportPath = Server.MapPath("~/" + Global.ReportFolder + _strReportRekapBillWater); break;
        //    }
        //    ((ReportDocument)Session["rpt" + _strReportUID]).Load(reportPath);
        //    TBillBL m_objTBillBL = new TBillBL();
        //    m_objTBillBL.ConnectionStringName = Global.ConnStrConfigName;
        //    Dictionary<int, DataSet> m_dicTBillBL = new Dictionary<int, DataSet>();
        //    switch (ProductDesc)
        //    {
        //        case "Electricity": m_dicTBillBL = m_objTBillBL.SelectBill_RekapElectricityReport(BillDate, PlantID, MaterialTypeID, ParameterID, null); break;
        //        case "ServiceCharge": m_dicTBillBL = m_objTBillBL.SelectBill_RekapServiceChargeReport(BillDate, PlantID, MaterialTypeID, ParameterID, null); break;
        //        case "Water": m_dicTBillBL = m_objTBillBL.SelectBill_RekapWaterReport(BillDate, PlantID, MaterialTypeID, ParameterID, null); break;
        //    }
        //    //Calculate BebanPemakaian dengan menggunakan fungsi ProporsionalPeriod
        //    DataTable dtQueryResult = m_dicTBillBL[0].Tables[0];
        //    DataTable dtBillgenBC = BillGenBC(ref strMessage);

        //    if (ProductDesc.ToLower() == "electricity")
        //    {

        //        foreach (DataRow dr in dtQueryResult.Rows)
        //        {
        //            string BillPlanID = dr["BillPlanID"].ToString();
        //            m_decBebanPemakaian = decimal.Parse(dr["BebanPemakaian"].ToString());

        //            //filter datatable billgenBC base on billplanID
        //            DataTable dtFilteredDataTable = dtBillgenBC.AsEnumerable().Where(row => row.Field<String>("BillPlanID") == BillPlanID).CopyToDataTable();

        //            string m_strPricingPeriod = dtFilteredDataTable.Rows[0]["PricingPeriodID"].ToString();
        //            string m_strBillCycleID = dtFilteredDataTable.Rows[0]["BillCycleID"].ToString();

        //            string m_strBillPeriodStart = dtFilteredDataTable.Rows[0]["BillPeriodStart"].ToString();

        //            string m_strBillPeriodEnd = dtFilteredDataTable.Rows[0]["BillPeriodEnd"].ToString();
        //            string m_strStartDate = dtFilteredDataTable.Rows[0]["StartDate"].ToString();
        //            string m_strEndDate = dtFilteredDataTable.Rows[0]["EndDate"].ToString();
        //            string m_strStartBillMethod = dtFilteredDataTable.Rows[0]["StartBillMethod"].ToString();
        //            string m_strEndBillMethod = dtFilteredDataTable.Rows[0]["EndBillMethod"].ToString();


        //            decimal m_decTotalPeriod = m_objBaseController.ProportionPeriod(int.Parse(m_strPricingPeriod), m_strBillCycleID,
        //                                                                            DateTime.Parse(m_strBillPeriodStart),
        //                                                                            DateTime.Parse(m_strBillPeriodEnd),
        //                                                                            DateTime.Parse(m_strStartDate),
        //                                                                            DateTime.Parse(m_strEndDate),
        //                                                                            m_strStartBillMethod.ToUpper() == "FUL",
        //                                                                            m_strEndBillMethod.ToUpper() == "FUL");
        //            m_decBebanPemakaian *= m_decTotalPeriod;

        //            dr["BebanPemakaian"] = m_decBebanPemakaian;
        //        }
        //    }
        //    //

        //    if (m_objTBillBL.Success)
        //        //((ReportDocument)Session["rpt" + _strReportUID]).Database.Tables[0].SetDataSource(m_dicTBillBL[0].Tables[0]);
        //        ((ReportDocument)Session["rpt" + _strReportUID]).Database.Tables[0].SetDataSource(dtQueryResult);
        //}

        //public DataTable BillGenBC(ref string strMessage)
        //{
        //    DataTable BillGenDataTable = new DataTable();
        //    TBillBL m_objTBillBL = new TBillBL();
        //    m_objTBillBL.ConnectionStringName = Global.ConnStrConfigName;
        //    BillGenVM m_objBillGenVM = new BillGenVM();
        //    bool isOffering = false;
        //    bool isContract = true;
        //    List<string> m_lstSelect = new List<string>();
        //    if (isOffering)
        //    {
        //        m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.OfferingID, true, isOffering));
        //        //m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.PaymentTermID, true, isOffering));
        //        //m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.FormulaID, true, isOffering));
        //    }
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.BillBundlingID, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.Period, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.BillPlanID, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.ContractUnitID, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.BillPeriodStart, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.PaymentPeriodStart, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.BillPeriodEnd, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.BillDate, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.DueDate, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.BillPortion, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.IsLockPrice, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.Amount, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.ContractID, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.PricingID, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.ProductID, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.PricingPeriodID, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.PlantID, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.PlantDesc, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.HasAdminFee, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.HasStampDuty, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.TaxPayeeID, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.MaterialID, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.StartDate, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.EndDate, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.BillTo, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.BillToDesc, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.IsBilled, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.PricingGroupID, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.PricingTypeID, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.HasMinimum, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.HasMaximum, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.MinMaxType, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.TableName, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.ComparerField, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.MultiplierField, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.FilterKey, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.ParameterID, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.Minimum, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.Maximum, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.Value, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.TaxPercentage, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.Multiplier, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.ReferenceID, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.WithholdingPercentage, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.ValueField, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.MeterID, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.Formula, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.StartBillMethod, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.EndBillMethod, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.BillCycleID, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.CalculationBaseID, true, isOffering));
        //    m_lstSelect.Add(m_objBillGenVM.MapField(() => m_objBillGenVM.Wattage, true, isOffering));

        //    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
        //    List<object> m_lstFilter = new List<object>();


        //    List<BillGenVM> m_lstBillGenVM = new List<BillGenVM>();
        //    Dictionary<int, DataSet> m_dicTBillBL = m_objTBillBL.SelectBillGenBC(0, null, false, m_lstSelect, m_objFilter, null, isOffering, isContract);
        //    if (m_objTBillBL.Message != string.Empty)
        //    {
        //        strMessage = "Error get bill to calculate. " + m_objTBillBL.Message;
        //        return null;
        //    }

        //    BillGenDataTable = m_dicTBillBL[0].Tables[0];
        //    return BillGenDataTable;
        //}

        //private void BillOutstanding(DateTime periodDate, DateTime paymentDate, string plantID, string materialTypeID)
        //{
        //    string reportPath = Server.MapPath("~/" + Global.ReportFolder + _strReportBillOutstanding);
        //    ((ReportDocument)Session["rpt" + _strReportUID]).Load(reportPath);

        //    TBillBL m_objTBillBL = new TBillBL();
        //    m_objTBillBL.ConnectionStringName = Global.ConnStrConfigName;
        //    Dictionary<int, DataSet> m_dicTBillBL = m_objTBillBL.SelectOutstanding(paymentDate, periodDate, plantID, materialTypeID, null, null);

        //    if (m_objTBillBL.Success)
        //        ((ReportDocument)Session["rpt" + _strReportUID]).Database.Tables[0].SetDataSource(m_dicTBillBL[0].Tables[0]);
        //}

        //private void BillOutstandingReceipt(DateTime periodDate, DateTime paymentDate, string plantID, string materialTypeID)
        //{
        //    string reportPath = Server.MapPath("~/" + Global.ReportFolder + _strReportBillOutstandingReceipt);
        //    ((ReportDocument)Session["rpt" + _strReportUID]).Load(reportPath);

        //    TBillBL m_objTBillBL = new TBillBL();
        //    m_objTBillBL.ConnectionStringName = Global.ConnStrConfigName;
        //    Dictionary<int, DataSet> m_dicTBillBL = m_objTBillBL.SelectOutstandingReceipt(paymentDate, periodDate, plantID, materialTypeID, null, null);

        //    if (m_objTBillBL.Success)
        //        ((ReportDocument)Session["rpt" + _strReportUID]).Database.Tables[0].SetDataSource(m_dicTBillBL[0].Tables[0]);
        //}

        //private void BillOutstandingRecapReceipt(DateTime periodDate, DateTime paymentDate, string plantID, string MaterialTypeID)
        //{
        //    string reportPath = Server.MapPath("~/" + Global.ReportFolder + _strReportBillOutstandingRecapReceipt);
        //    ((ReportDocument)Session["rpt" + _strReportUID]).Load(reportPath);

        //    string m_strMessage = string.Empty;
        //    TBillBL m_objTBillBL = new TBillBL();
        //    m_objTBillBL.ConnectionStringName = Global.ConnStrConfigName;
        //    Dictionary<int, DataSet> m_dicTBillBL = m_objTBillBL.SelectRekapOutstandingReceipt(periodDate, paymentDate, plantID, MaterialTypeID, null, null);
        //    if (m_objTBillBL.Success)
        //    {

        //        PlantController m_objPlantController = new PlantController();
        //        Dictionary<string, object> m_dicData = new Dictionary<string, object>();
        //        PlantVM m_objPlantVM = new PlantVM();
        //        m_dicData.Add(General.GetVariableName(() => m_objPlantVM.PlantID), plantID);
        //        m_objPlantVM = m_objPlantController.GetPlantVM(m_dicData, ref m_strMessage);

        //        ((ReportDocument)Session["rpt" + _strReportUID]).Database.Tables[0].SetDataSource(m_dicTBillBL[0].Tables[0]);
        //        ((ReportDocument)Session["rpt" + _strReportUID]).DataDefinition.FormulaFields["FCompanyDesc"].Text = "'" + m_objPlantVM.CompanyDesc + "'";
        //        ((ReportDocument)Session["rpt" + _strReportUID]).DataDefinition.FormulaFields["FPlantDesc"].Text = "'" + m_objPlantVM.PlantDesc + "'";
        //        ((ReportDocument)Session["rpt" + _strReportUID]).DataDefinition.FormulaFields["FThisYear"].Text = "'" + periodDate.Year + "'";
        //    }
        //}

        //private void BillOutstandingRecap(DateTime periodDate, DateTime paymentDate, string plantID, string MaterialTypeID)
        //{
        //    string reportPath = Server.MapPath("~/" + Global.ReportFolder + _strReportBillOutstandingRecap);
        //    ((ReportDocument)Session["rpt" + _strReportUID]).Load(reportPath);

        //    string m_strMessage = string.Empty;
        //    TBillBL m_objTBillBL = new TBillBL();
        //    m_objTBillBL.ConnectionStringName = Global.ConnStrConfigName;
        //    Dictionary<int, DataSet> m_dicTBillBL = m_objTBillBL.SelectRekapOutstanding(periodDate, paymentDate, plantID, MaterialTypeID, null, null);


        //    if (m_objTBillBL.Success)
        //    {
        //        //Perubahan Query karena Proses lama maka query dipecah dua kemudian di gabung di code
        //        DataTable dtSourceAmountData = new DataTable();
        //        DataTable dtMaterialParam = new DataTable();
        //        DataTable dtFilteredData = new DataTable();
        //        dtSourceAmountData = m_dicTBillBL[0].Tables[0];
        //        if (!string.IsNullOrEmpty(MaterialTypeID))
        //        {
        //            int numberOfRows = m_dicTBillBL[0].Tables[0].AsEnumerable().Where(row => row.Field<String>("MaterialTypeID") == MaterialTypeID).Count();
        //            if (numberOfRows > 0)
        //            {
        //                dtSourceAmountData = m_dicTBillBL[0].Tables[0].AsEnumerable().Where(row => row.Field<String>("MaterialTypeID") == MaterialTypeID).CopyToDataTable();
        //            }

        //        }
        //        dtMaterialParam = m_dicTBillBL[1].Tables[0];

        //        //dtSourceAmountData.Columns.Add("CustomerName", typeof(String));
        //        //dtSourceAmountData.Columns.Add("MaterialTypeID", typeof(String));
        //        //dtSourceAmountData.Columns.Add("FLOOR", typeof(String));
        //        //dtSourceAmountData.Columns.Add("Blok", typeof(String));
        //        //dtSourceAmountData.Columns.Add("MaterialGroupDesc", typeof(String));
        //        int testCounter = 0;
        //        if (dtSourceAmountData.Rows.Count > 0)
        //        {
        //            foreach (DataRow dr in dtSourceAmountData.Rows)
        //            {
        //                string m_strCustomerID = string.Empty;
        //                string m_strMaterialID = string.Empty;
        //                m_strCustomerID = dr["BillTo"].ToString();
        //                m_strMaterialID = dr["MaterialID"].ToString();
        //                if (!string.IsNullOrEmpty(m_strMaterialID))
        //                {
        //                    int countResult = dtMaterialParam.AsEnumerable().Where(row => row.Field<String>("CustomerID") == m_strCustomerID && row.Field<String>("MaterialID").Contains(m_strMaterialID)).Count();
        //                    if (countResult == 0)
        //                    {
        //                        testCounter++;
        //                    }
        //                    else
        //                    {
        //                        dtFilteredData = dtMaterialParam.AsEnumerable().Where(row => row.Field<String>("CustomerID") == m_strCustomerID && row.Field<String>("MaterialID").Contains(m_strMaterialID)).CopyToDataTable();
        //                        dr["CustomerName"] = dtFilteredData.Rows[0]["FullName"];
        //                        dr["MaterialTypeID"] = dtFilteredData.Rows[0]["MaterialTypeID"];
        //                        dr["FLOOR"] = dtFilteredData.Rows[0]["Lantai"];
        //                        dr["Blok"] = dtFilteredData.Rows[0]["KodeUnit"];
        //                        dr["MaterialGroupDesc"] = dtFilteredData.Rows[0]["MaterialGroupDesc"];
        //                    }
        //                }
        //            }
        //        }






        //        PlantController m_objPlantController = new PlantController();
        //        Dictionary<string, object> m_dicData = new Dictionary<string, object>();
        //        PlantVM m_objPlantVM = new PlantVM();
        //        m_dicData.Add(General.GetVariableName(() => m_objPlantVM.PlantID), plantID);
        //        m_objPlantVM = m_objPlantController.GetPlantVM(m_dicData, ref m_strMessage);

        //        ((ReportDocument)Session["rpt" + _strReportUID]).Database.Tables[0].SetDataSource(dtSourceAmountData);
        //        ((ReportDocument)Session["rpt" + _strReportUID]).DataDefinition.FormulaFields["FCompanyDesc"].Text = "'" + m_objPlantVM.CompanyDesc + "'";
        //        ((ReportDocument)Session["rpt" + _strReportUID]).DataDefinition.FormulaFields["FPlantDesc"].Text = "'" + m_objPlantVM.PlantDesc + "'";
        //        ((ReportDocument)Session["rpt" + _strReportUID]).DataDefinition.FormulaFields["FThisYear"].Text = "'" + periodDate.Year + "'";
        //    }
        //}

        //private void BillCreditDetail(DateTime date, string customerID, string materialID)
        //{
        //    string reportPath = Server.MapPath("~/" + Global.ReportFolder + _strReportBillCreditDetail);
        //    ((ReportDocument)Session["rpt" + _strReportUID]).Load(reportPath);
        //    string labelWT = "";
        //    string labelEL = "";
        //    string labelSC = "";
        //    List<string> m_lstMessage = new List<string>();
        //    List<BillCreditDetailVM> m_lstBillCreditDetailVM = this.GenerateBillCreditDetailVM(date, customerID, materialID, ref labelWT, ref labelEL, ref labelSC);

        //    List<string> m_lstBillID = new List<string>();
        //    m_lstBillID = m_lstBillCreditDetailVM.Select(x => x.BillID).Distinct().ToList();

        //    List<string> m_lstMaterialDesc = this.GetListMaterial(m_lstBillID, ref m_lstMessage);
        //    string m_strAllMaterialDesc = String.Join(",", m_lstMaterialDesc);

        //    DataTable m_dtBill = OfferingController.ToDataTableNoConverting(m_lstBillCreditDetailVM);
        //    ((ReportDocument)Session["rpt" + _strReportUID]).SetDataSource(m_dtBill);
        //    ((ReportDocument)Session["rpt" + _strReportUID]).DataDefinition.FormulaFields["FUnit"].Text = "'" + m_strAllMaterialDesc + "'";
        //    ((ReportDocument)Session["rpt" + _strReportUID]).DataDefinition.FormulaFields["FPeriod"].Text = "'" + date.ToString(Global.DefaultDateFormat) + "'";
        //    ((ReportDocument)Session["rpt" + _strReportUID]).DataDefinition.FormulaFields["FLabelSC"].Text = "'" + labelSC + "'";
        //    ((ReportDocument)Session["rpt" + _strReportUID]).DataDefinition.FormulaFields["FLabelWT"].Text = "'" + labelWT + "'";
        //    ((ReportDocument)Session["rpt" + _strReportUID]).DataDefinition.FormulaFields["FLabelEL"].Text = "'" + labelEL + "'";
        //}

        //private List<BillCreditDetailVM> GenerateBillCreditDetailVM(DateTime date, string customerID, string materialID, ref string labelWT, ref string labelEL, ref string labelSC)
        //{
        //    List<BillCreditDetailVM> m_lstBillForSummary = new List<BillCreditDetailVM>();
        //    List<BillCreditDetailVM> m_lstBillForDetail = new List<BillCreditDetailVM>();
        //    BillCreditDetailVM m_objBillCreditDetailVM = new BillCreditDetailVM();
        //    TBillBL m_objTBillBL = new TBillBL();
        //    m_objTBillBL.ConnectionStringName = Global.ConnStrConfigName;
        //    string m_strMessage = "";
        //    string m_strDateFormat = "dd-MMM-yyyy";
        //    //Select from Summary
        //    Dictionary<string, List<object>> m_objFilter = FilterBillCredit(date, customerID, Operator.NotEqual, "Utility");
        //    Dictionary<string, OrderDirection> m_dicOrder = OrderBillCredit();
        //    Dictionary<int, DataSet> m_dicTBillBL = m_objTBillBL.SelectCreditSummary(date, materialID, m_objFilter, m_dicOrder);
        //    if (m_objTBillBL.Message == string.Empty)
        //        m_lstBillForSummary = (
        //            from DataRow m_drDNumberRangeBL in m_dicTBillBL[0].Tables[0].Rows
        //            select new BillCreditDetailVM()
        //            {
        //                BillID = m_drDNumberRangeBL[General.GetVariableName(() => m_objBillCreditDetailVM.BillID)].ToString(),
        //                BillDate = DateTime.Parse(m_drDNumberRangeBL[General.GetVariableName(() => m_objBillCreditDetailVM.BillDate)].ToString()),
        //                CustomerName = m_drDNumberRangeBL[General.GetVariableName(() => m_objBillCreditDetailVM.CustomerName)].ToString(),
        //                BillMonth = m_drDNumberRangeBL[General.GetVariableName(() => m_objBillCreditDetailVM.BillMonth)].ToString(),
        //                BillType = m_drDNumberRangeBL[General.GetVariableName(() => m_objBillCreditDetailVM.BillType)].ToString(),
        //                TTSDate = m_drDNumberRangeBL[General.GetVariableName(() => m_objBillCreditDetailVM.TTSDate)].ToString() == String.Empty ?
        //                    String.Empty : DateTime.Parse(m_drDNumberRangeBL[General.GetVariableName(() => m_objBillCreditDetailVM.TTSDate)].ToString()).ToString(m_strDateFormat),
        //                TTSNumber = m_drDNumberRangeBL[General.GetVariableName(() => m_objBillCreditDetailVM.TTSNumber)].ToString(),
        //                DebitOther = Convert.ToDecimal(m_drDNumberRangeBL[General.GetVariableName(() => m_objBillCreditDetailVM.Debit)].ToString()),
        //                CreditOther = Convert.ToDecimal(m_drDNumberRangeBL[General.GetVariableName(() => m_objBillCreditDetailVM.Credit)].ToString()),
        //                SaldoOther = Convert.ToDecimal(m_drDNumberRangeBL[General.GetVariableName(() => m_objBillCreditDetailVM.Saldo)].ToString())
        //            }).ToList();

        //    //Select from Detail
        //    m_objFilter = FilterBillCredit(date, customerID, Operator.Equals, "Utility");
        //    m_dicTBillBL = m_objTBillBL.SelectCreditDetail(date, materialID, m_objFilter, m_dicOrder);
        //    if (m_objTBillBL.Message == string.Empty)
        //        m_lstBillForDetail = (
        //            from DataRow m_drDNumberRangeBL in m_dicTBillBL[0].Tables[0].Rows
        //            select new BillCreditDetailVM()
        //            {
        //                BillID = m_drDNumberRangeBL[General.GetVariableName(() => m_objBillCreditDetailVM.BillID)].ToString(),
        //                BillDate = DateTime.Parse(m_drDNumberRangeBL[General.GetVariableName(() => m_objBillCreditDetailVM.BillDate)].ToString()),
        //                CustomerName = m_drDNumberRangeBL[General.GetVariableName(() => m_objBillCreditDetailVM.CustomerName)].ToString(),
        //                BillMonth = m_drDNumberRangeBL[General.GetVariableName(() => m_objBillCreditDetailVM.BillMonth)].ToString(),
        //                BillType = m_drDNumberRangeBL[General.GetVariableName(() => m_objBillCreditDetailVM.BillType)].ToString(),
        //                ProductID = m_drDNumberRangeBL[General.GetVariableName(() => m_objBillCreditDetailVM.ProductID)].ToString(),
        //                TTSDate = m_drDNumberRangeBL[General.GetVariableName(() => m_objBillCreditDetailVM.TTSDate)].ToString() == "" ?
        //                    String.Empty : DateTime.Parse(m_drDNumberRangeBL[General.GetVariableName(() => m_objBillCreditDetailVM.TTSDate)].ToString()).ToString(m_strDateFormat),
        //                TTSNumber = m_drDNumberRangeBL[General.GetVariableName(() => m_objBillCreditDetailVM.TTSNumber)].ToString(),
        //                Debit = Convert.ToDecimal(m_drDNumberRangeBL[General.GetVariableName(() => m_objBillCreditDetailVM.Debit)].ToString()),
        //                Credit = Convert.ToDecimal(m_drDNumberRangeBL[General.GetVariableName(() => m_objBillCreditDetailVM.Credit)].ToString()),
        //                Saldo = Convert.ToDecimal(m_drDNumberRangeBL[General.GetVariableName(() => m_objBillCreditDetailVM.Saldo)].ToString())
        //            }).ToList();

        //    List<string> m_lstBillID = m_lstBillForDetail.Select(x => x.BillID).Distinct().ToList();
        //    List<ConfigVM> m_lstConfigProduct = new ConfigController().GetMappingProductList();
        //    ConfigVM m_objProductSC = m_lstConfigProduct.Where(x => x.Key3 == "ServiceCharge").FirstOrDefault();
        //    ConfigVM m_objProductEL = m_lstConfigProduct.Where(x => x.Key3 == "Electricity").FirstOrDefault();
        //    ConfigVM m_objProductWT = m_lstConfigProduct.Where(x => x.Key3 == "Water").FirstOrDefault();

        //    foreach (string billID in m_lstBillID)
        //    {
        //        List<string> m_lstTTSNumber = m_lstBillForDetail.Where(x => x.BillID == billID).Select(x => x.TTSNumber).Distinct().ToList();
        //        foreach (string ttsNumber in m_lstTTSNumber)
        //        {
        //            List<BillCreditDetailVM> m_lstFound = m_lstBillForDetail.Where(x => x.BillID == billID && x.TTSNumber == ttsNumber).ToList();
        //            BillCreditDetailVM m_objFirst = m_lstFound.FirstOrDefault();
        //            BillCreditDetailVM m_objNewDetail = new BillCreditDetailVM();
        //            m_objNewDetail.BillID = m_objFirst.BillID;
        //            m_objNewDetail.BillDate = m_objFirst.BillDate;
        //            m_objNewDetail.CustomerName = m_objFirst.CustomerName;
        //            m_objNewDetail.BillMonth = m_objFirst.BillMonth;
        //            m_objNewDetail.BillType = m_objFirst.BillType;
        //            m_objNewDetail.TTSDate = m_objFirst.TTSDate;
        //            m_objNewDetail.TTSNumber = m_objFirst.TTSNumber;

        //            ProductVM m_objProductVM = new ProductVM();
        //            ProductController m_objProductController = new ProductController();
        //            Dictionary<string, object> m_dicProduct = new Dictionary<string, object>();
        //            m_dicProduct.Add(General.GetVariableName(() => m_objProductVM.ProductID), m_objProductSC.Desc1);
        //            ProductVM m_objProductVMSC = m_objProductController.GetProductVM(m_dicProduct, ref m_strMessage);
        //            labelSC = m_objProductVMSC.ProductDesc;

        //            m_dicProduct = new Dictionary<string, object>();
        //            m_dicProduct.Add(General.GetVariableName(() => m_objProductVM.ProductID), m_objProductEL.Desc1);
        //            ProductVM m_objProductVMEL = m_objProductController.GetProductVM(m_dicProduct, ref m_strMessage);
        //            labelEL = m_objProductVMEL.ProductDesc;

        //            m_dicProduct = new Dictionary<string, object>();
        //            m_dicProduct.Add(General.GetVariableName(() => m_objProductVM.ProductID), m_objProductWT.Desc1);
        //            ProductVM m_objProductVMWT = m_objProductController.GetProductVM(m_dicProduct, ref m_strMessage);
        //            labelWT = m_objProductVMWT.ProductDesc;

        //            foreach (BillCreditDetailVM perProduct in m_lstFound)
        //            {
        //                if (perProduct.ProductID == m_objProductSC.Desc1)
        //                {
        //                    m_objNewDetail.DebitSC += perProduct.Debit;
        //                    m_objNewDetail.CreditSC += perProduct.Credit;
        //                    m_objNewDetail.SaldoSC += perProduct.Saldo;
        //                }
        //                else if (perProduct.ProductID == m_objProductEL.Desc1)
        //                {
        //                    m_objNewDetail.DebitEL += perProduct.Debit;
        //                    m_objNewDetail.CreditEL += perProduct.Credit;
        //                    m_objNewDetail.SaldoEL += perProduct.Saldo;
        //                }
        //                else if (perProduct.ProductID == m_objProductWT.Desc1)
        //                {
        //                    m_objNewDetail.DebitWT += perProduct.Debit;
        //                    m_objNewDetail.CreditWT += perProduct.Credit;
        //                    m_objNewDetail.SaldoWT += perProduct.Saldo;
        //                }
        //                else
        //                {
        //                    m_objNewDetail.DebitOther += perProduct.Debit;
        //                    m_objNewDetail.CreditOther += perProduct.Credit;
        //                    m_objNewDetail.SaldoOther += perProduct.Saldo;
        //                }
        //            }
        //            m_lstBillForSummary.Add(m_objNewDetail);
        //        }
        //    }
        //    return m_lstBillForSummary;
        //}

        //private void BillCreditSummary(DateTime date, string customerID, string materialID)
        //{
        //    string reportPath = Server.MapPath("~/" + Global.ReportFolder + _strReportBillCreditSummary);
        //    ((ReportDocument)Session["rpt" + _strReportUID]).Load(reportPath);

        //    string m_strMessage = string.Empty;
        //    List<string> m_lstMessage = new List<string>();
        //    TBillBL m_objTBillBL = new TBillBL();
        //    BillVM m_objBillVM = new BillVM();
        //    m_objTBillBL.ConnectionStringName = Global.ConnStrConfigName;
        //    Dictionary<string, List<object>> m_objFilter = FilterBillCredit(date, customerID);
        //    Dictionary<string, OrderDirection> m_dicOrder = OrderBillCredit();

        //    Dictionary<int, DataSet> m_dicTBillBL = m_objTBillBL.SelectCreditSummary(date, materialID, m_objFilter, m_dicOrder);

        //    List<string> m_lstBillID = new List<string>();
        //    foreach (DataRow item in m_dicTBillBL[0].Tables[0].Rows)
        //    {
        //        m_lstBillID.Add(item[General.GetVariableName(() => m_objBillVM.BillID)].ToString());
        //    }
        //    m_lstBillID = m_lstBillID.Distinct().ToList();

        //    List<string> m_lstMaterialDesc = this.GetListMaterial(m_lstBillID, ref m_lstMessage);
        //    string m_strAllMaterialDesc = String.Join(",", m_lstMaterialDesc);
        //    m_strMessage = string.IsNullOrEmpty(m_strMessage) ? m_objTBillBL.Message : m_strMessage;
        //    if (m_objTBillBL.Success)
        //    {
        //        ((ReportDocument)Session["rpt" + _strReportUID]).Database.Tables[0].SetDataSource(m_dicTBillBL[0].Tables[0]);
        //        ((ReportDocument)Session["rpt" + _strReportUID]).DataDefinition.FormulaFields["FUnit"].Text = "'" + m_strAllMaterialDesc + "'";
        //        ((ReportDocument)Session["rpt" + _strReportUID]).DataDefinition.FormulaFields["FPeriod"].Text = "'" + date.ToString(Global.DefaultDateFormat) + "'";
        //    }
        //}

        //private void BillDebtWarning(string ReportID, string CustomerID, string PlantID, string DebtWarningID)
        //{
        //    string reportPath = Server.MapPath("~/" + Global.ReportFolder + _strReportBillDebtWarning);
        //    ((ReportDocument)Session["rpt" + _strReportUID]).Load(reportPath);

        //    string m_strMessage = string.Empty;
        //    List<string> m_strBlok = new List<string>();
        //    List<string> m_strFloor = new List<string>();
        //    List<string> m_lstMessage = new List<string>();
        //    TDebtWarningBL m_objTDebtWarningBL = new TDebtWarningBL();
        //    DebtWarningRptVM m_objDebtWarningListVM = new DebtWarningRptVM();
        //    m_objTDebtWarningBL.ConnectionStringName = Global.ConnStrConfigName;
        //    Dictionary<string, List<object>> m_objFilter = FilterDebtWarning(CustomerID, PlantID, DebtWarningID);

        //    Dictionary<int, DataSet> m_dicTDebtWarningBL = m_objTDebtWarningBL.SelectDebtWarningReport(ReportID, DebtWarningID, m_objFilter);
        //    m_objTDebtWarningBL.ConnectionStringName = Global.ConnStrConfigName;


        //    //Get ReportFileName


        //    string m_strReportFileName = getReportFileName(DebtWarningID);


        //    if (m_strReportFileName != string.Empty)
        //    {
        //        m_strReportFileName = m_strReportFileName.Substring(m_strReportFileName.Length - 1, 1);
        //    }
        //    else
        //    {
        //        m_strReportFileName = "1";
        //    }

        //    if (m_objTDebtWarningBL.Success)
        //    {
        //        for (int i = 0; i < m_dicTDebtWarningBL[0].Tables[0].Rows.Count; i++)
        //        {
        //            m_strBlok.Add(m_dicTDebtWarningBL[0].Tables[0].Rows[i]["Block"].ToString());
        //            m_strFloor.Add(m_dicTDebtWarningBL[0].Tables[0].Rows[i]["Floor"].ToString());
        //        }
        //        m_strBlok = m_strBlok.Distinct().ToList();
        //        m_strFloor = m_strFloor.Distinct().ToList();

        //        ((ReportDocument)Session["rpt" + _strReportUID]).DataDefinition.FormulaFields["FReportFileName"].Text = "'" + m_strReportFileName + "'";
        //        ((ReportDocument)Session["rpt" + _strReportUID]).DataDefinition.FormulaFields["FBlock"].Text = "'" + string.Join(Global.OneLineSeparated, m_strBlok) + "'";
        //        ((ReportDocument)Session["rpt" + _strReportUID]).DataDefinition.FormulaFields["FFloor"].Text = "'" + string.Join(Global.OneLineSeparated, m_strFloor) + "'";
        //        ((ReportDocument)Session["rpt" + _strReportUID]).Database.Tables[0].SetDataSource(m_dicTDebtWarningBL[0].Tables[0]);

        //    }
        //}

        //private void RequestForQuotation(string PurchasingDocument, string VendorID, string CompanyID, string PlantID)
        //{
        //    string reportPath = Server.MapPath("~/" + Global.ReportFolder + _strReportRequestForQuotation);
        //    ((ReportDocument)Session["rpt" + _strReportUID]).Load(reportPath);

        //    string m_strMessage = string.Empty;
        //    List<string> m_lstMessage = new List<string>();
        //    TRequestForQuotationBL m_objTRequestForQuotationBL = new TRequestForQuotationBL();
        //    RequestForQuotationRptVM m_objRequestForQuotationVM = new RequestForQuotationRptVM();
        //    m_objTRequestForQuotationBL.ConnectionStringName = Global.ConnStrConfigName;

        //    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
        //    List<object> m_lstFilter = new List<object>();

        //    if (!string.IsNullOrEmpty(PurchasingDocument))
        //    {
        //        m_lstFilter = new List<object>();
        //        m_lstFilter.Add(Operator.In);
        //        m_lstFilter.Add(PurchasingDocument);
        //        m_objFilter.Add(m_objRequestForQuotationVM.MapField(() => m_objRequestForQuotationVM.PurchasingDocument, false), m_lstFilter);

        //        m_lstFilter = new List<object>();
        //        m_lstFilter.Add(Operator.In);
        //        m_lstFilter.Add(VendorID);
        //        m_objFilter.Add(m_objRequestForQuotationVM.MapField(() => m_objRequestForQuotationVM.VendorID, false), m_lstFilter);

        //        m_lstFilter = new List<object>();
        //        m_lstFilter.Add(Operator.In);
        //        m_lstFilter.Add(CompanyID);
        //        m_objFilter.Add(m_objRequestForQuotationVM.MapField(() => m_objRequestForQuotationVM.CompanyID, false), m_lstFilter);

        //        m_lstFilter = new List<object>();
        //        m_lstFilter.Add(Operator.In);
        //        m_lstFilter.Add(PlantID);
        //        m_objFilter.Add(m_objRequestForQuotationVM.MapField(() => m_objRequestForQuotationVM.PlantID, false), m_lstFilter);
        //    }

        //    Dictionary<int, DataSet> m_dicTRequestForQuotationBL = m_objTRequestForQuotationBL.SelectRequestForQuotationReport(0, null, false, null, m_objFilter, null, null);
        //    m_objTRequestForQuotationBL.ConnectionStringName = Global.ConnStrConfigName;

        //    if (m_objTRequestForQuotationBL.Success)
        //        ((ReportDocument)Session["rpt" + _strReportUID]).Database.Tables[0].SetDataSource(m_dicTRequestForQuotationBL[0].Tables[0]);
        //}

        //private void OutlineAgreement(string PurchasingDocument, string VendorID, string CompanyID, string PlantID)
        //{
        //    string reportPath = Server.MapPath("~/" + Global.ReportFolder + _strReportOutlineAgreement);
        //    ((ReportDocument)Session["rpt" + _strReportUID]).Load(reportPath);

        //    string m_strMessage = string.Empty;
        //    List<string> m_lstMessage = new List<string>();
        //    TOutlineAgreementBL m_objTOutlineAgreementBL = new TOutlineAgreementBL();
        //    OutlineAgreementRPTVM m_objOutlineAgreementVM = new OutlineAgreementRPTVM();
        //    m_objTOutlineAgreementBL.ConnectionStringName = Global.ConnStrConfigName;

        //    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
        //    List<object> m_lstFilter = new List<object>();

        //    if (!string.IsNullOrEmpty(PurchasingDocument))
        //    {
        //        m_lstFilter = new List<object>();
        //        m_lstFilter.Add(Operator.In);
        //        m_lstFilter.Add(PurchasingDocument);
        //        m_objFilter.Add(m_objOutlineAgreementVM.MapField(() => m_objOutlineAgreementVM.PurchasingDocument, false), m_lstFilter);

        //        m_lstFilter = new List<object>();
        //        m_lstFilter.Add(Operator.In);
        //        m_lstFilter.Add(VendorID);
        //        m_objFilter.Add(m_objOutlineAgreementVM.MapField(() => m_objOutlineAgreementVM.VendorID, false), m_lstFilter);

        //        m_lstFilter = new List<object>();
        //        m_lstFilter.Add(Operator.In);
        //        m_lstFilter.Add(CompanyID);
        //        m_objFilter.Add(m_objOutlineAgreementVM.MapField(() => m_objOutlineAgreementVM.CompanyID, false), m_lstFilter);

        //        m_lstFilter = new List<object>();
        //        m_lstFilter.Add(Operator.In);
        //        m_lstFilter.Add(PlantID);
        //        m_objFilter.Add(m_objOutlineAgreementVM.MapField(() => m_objOutlineAgreementVM.PlantID, false), m_lstFilter);
        //    }

        //    Dictionary<int, DataSet> m_dicTOutlineAgreementBL = m_objTOutlineAgreementBL.SelectOutlineAgreementReport(0, null, false, null, m_objFilter, null, null);
        //    m_objTOutlineAgreementBL.ConnectionStringName = Global.ConnStrConfigName;

        //    if (m_objTOutlineAgreementBL.Success)
        //        ((ReportDocument)Session["rpt" + _strReportUID]).Database.Tables[0].SetDataSource(m_dicTOutlineAgreementBL[0].Tables[0]);
        //}

        //private void MaterialDocument(string MaterialDocument)
        //{
        //    string reportPath = Server.MapPath("~/" + Global.ReportFolder + _strReportMaterialDocument);
        //    ((ReportDocument)Session["rpt" + _strReportUID]).Load(reportPath);

        //    string m_strMessage = string.Empty;
        //    List<string> m_lstMessage = new List<string>();
        //    TMaterialDocumentBL m_objTMaterialDocumentBL = new TMaterialDocumentBL();
        //    MaterialDocumentRPTVM m_objMaterialDocumentVM = new MaterialDocumentRPTVM();
        //    m_objTMaterialDocumentBL.ConnectionStringName = Global.ConnStrConfigName;

        //    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
        //    List<object> m_lstFilter = new List<object>();

        //    if (!string.IsNullOrEmpty(MaterialDocument))
        //    {
        //        m_lstFilter = new List<object>();
        //        m_lstFilter.Add(Operator.In);
        //        m_lstFilter.Add(MaterialDocument);
        //        m_objFilter.Add(m_objMaterialDocumentVM.MapField(() => m_objMaterialDocumentVM.MaterialDocument, false), m_lstFilter);
        //    }

        //    Dictionary<int, DataSet> m_dicTMaterialDocumentBL = m_objTMaterialDocumentBL.SelectMaterialDocumentReport(0, null, false, null, m_objFilter, null, null);
        //    m_objTMaterialDocumentBL.ConnectionStringName = Global.ConnStrConfigName;

        //    if (m_objTMaterialDocumentBL.Success)
        //        ((ReportDocument)Session["rpt" + _strReportUID]).Database.Tables[0].SetDataSource(m_dicTMaterialDocumentBL[0].Tables[0]);
        //}

        //private void PurchaseOrder(string PurchasingDocument, string VendorID, string CompanyID, string PlantID)
        //{
        //    string reportPath = Server.MapPath("~/" + Global.ReportFolder + _strReportPurchaseOrder);
        //    ((ReportDocument)Session["rpt" + _strReportUID]).Load(reportPath);

        //    string m_strMessage = string.Empty;
        //    List<string> m_lstMessage = new List<string>();
        //    TPurchaseOrderBL m_objTPurchaseOrderBL = new TPurchaseOrderBL();
        //    PurchaseOrderRptVM m_objPurchaseOrderVM = new PurchaseOrderRptVM();
        //    m_objTPurchaseOrderBL.ConnectionStringName = Global.ConnStrConfigName;

        //    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
        //    List<object> m_lstFilter = new List<object>();

        //    if (!string.IsNullOrEmpty(PurchasingDocument))
        //    {
        //        m_lstFilter = new List<object>();
        //        m_lstFilter.Add(Operator.In);
        //        m_lstFilter.Add(PurchasingDocument);
        //        m_objFilter.Add(m_objPurchaseOrderVM.MapField(() => m_objPurchaseOrderVM.PurchasingDocument, false), m_lstFilter);

        //        m_lstFilter = new List<object>();
        //        m_lstFilter.Add(Operator.In);
        //        m_lstFilter.Add(VendorID);
        //        m_objFilter.Add(m_objPurchaseOrderVM.MapField(() => m_objPurchaseOrderVM.VendorID, false), m_lstFilter);

        //        m_lstFilter = new List<object>();
        //        m_lstFilter.Add(Operator.In);
        //        m_lstFilter.Add(CompanyID);
        //        m_objFilter.Add(m_objPurchaseOrderVM.MapField(() => m_objPurchaseOrderVM.CompanyID, false), m_lstFilter);

        //        m_lstFilter = new List<object>();
        //        m_lstFilter.Add(Operator.In);
        //        m_lstFilter.Add(PlantID);
        //        m_objFilter.Add(m_objPurchaseOrderVM.MapField(() => m_objPurchaseOrderVM.PlantID, false), m_lstFilter);
        //    }

        //    Dictionary<int, DataSet> m_dicTPurchaseOrderBL = m_objTPurchaseOrderBL.SelectPurchaseOrderReport(0, null, false, null, m_objFilter, null, null);
        //    m_objTPurchaseOrderBL.ConnectionStringName = Global.ConnStrConfigName;

        //    if (m_objTPurchaseOrderBL.Success)
        //        ((ReportDocument)Session["rpt" + _strReportUID]).Database.Tables[0].SetDataSource(m_dicTPurchaseOrderBL[0].Tables[0]);
        //}

        //private void BillSchedule(DateTime startDate, DateTime endDate, string plantID)
        //{
        //    string reportPath = Server.MapPath("~/" + Global.ReportFolder + _strReportBillSchedule);
        //    ((ReportDocument)Session["rpt" + _strReportUID]).Load(reportPath);
        //    List<string> m_lstMessage = new List<string>();
        //    BillController m_objBillController = new BillController();
        //    string m_strMessage = string.Empty;
        //    List<BillScheduleReportVM> lstBillScheduleReportVM = GenerateListBillSchedule(startDate, endDate, plantID);

        //    if (lstBillScheduleReportVM.Count > 0)
        //    {
        //        PlantController m_objPlantController = new PlantController();
        //        Dictionary<string, object> m_dicData = new Dictionary<string, object>();
        //        PlantVM m_objPlantVM = new PlantVM();
        //        m_dicData.Add(General.GetVariableName(() => m_objPlantVM.PlantID), plantID);
        //        m_objPlantVM = m_objPlantController.GetPlantVM(m_dicData, ref m_strMessage);

        //        ((ReportDocument)Session["rpt" + _strReportUID]).SetDataSource(lstBillScheduleReportVM);
        //        ((ReportDocument)Session["rpt" + _strReportUID]).DataDefinition.FormulaFields["FRangeDate"].Text = "'" + startDate.ToString(Global.FinanceDateFormat) + " S/D " + endDate.ToString(Global.FinanceDateFormat) + "'";
        //        ((ReportDocument)Session["rpt" + _strReportUID]).DataDefinition.FormulaFields["FCompanyDesc"].Text = "'" + m_objPlantVM.CompanyDesc + "'";
        //        ((ReportDocument)Session["rpt" + _strReportUID]).DataDefinition.FormulaFields["FPlantDesc"].Text = "'" + m_objPlantVM.PlantDesc + "'";
        //    }
        //}
        //#endregion

        //private void Offering(List<string> offeringID)
        //{
        //    string reportPath = Server.MapPath("~/" + Global.ReportFolder + _strReportOffering);
        //    ((ReportDocument)Session["rpt" + _strReportUID]).Load(reportPath);

        //    TOfferingBL m_objTOfferingBL = new TOfferingBL();
        //    m_objTOfferingBL.ConnectionStringName = Global.ConnStrConfigName;
        //    OfferingReportVM m_objOfferingReportVM = new OfferingReportVM();

        //    List<string> m_lstSelect = new List<string>();
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.OfferingID, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.CombinedCustomerName, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.PlantDesc, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.PlantCity, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.PlantAddress, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.CompanyDesc, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.Quantity, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.UnitStartRent, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.UnitEndRent, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.RentPeriod, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.ProductID, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.ProductDesc, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.IsRent, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.BaseAmount, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.Amount, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.TaxAmount, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.TotalAmount, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.TotalAmountDesc, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.PaymentTermID, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.PaymentTermDesc, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.MaterialGroupDesc, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.MaterialID, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.MaterialFloor, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.MaterialBlock, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.MaterialUnit, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.MaterialUoMID, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.BillPlanID, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.FormulaID, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.FormulaDesc, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.SalesFullName, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.ContractEndDate, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.Head, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.HeadPosition, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.Signer, true));
        //    m_lstSelect.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.SignerPosition, true));

        //    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
        //    List<object> m_lstFilter = new List<object>();
        //    m_lstFilter = new List<object>();
        //    m_lstFilter.Add(Operator.In);
        //    m_lstFilter.Add(String.Join(Global.OneLineSeparated, offeringID));
        //    m_objFilter.Add(m_objOfferingReportVM.MapField(() => m_objOfferingReportVM.OfferingID, false), m_lstFilter);

        //    Dictionary<int, DataSet> m_dicTOfferingBL = m_objTOfferingBL.SelectReport(0, null, false, m_lstSelect, m_objFilter, null, null);
        //    if (m_dicTOfferingBL[0].Tables.Count > 0)
        //    {
        //        DataTable m_dtTOfferingBL = m_dicTOfferingBL[0].Tables[0];
        //        string m_strMessage = string.Empty;
        //        // Get BillPlan
        //        List<ContractUnitVM> m_lstContractUnitVM = m_dtTOfferingBL.DefaultView.ToTable(true, new string[]
        //        {
        //            General.GetVariableName(() => m_objOfferingReportVM.PaymentTermID),
        //            General.GetVariableName(() => m_objOfferingReportVM.MaterialID),
        //            General.GetVariableName(() => m_objOfferingReportVM.ProductID),
        //            General.GetVariableName(() => m_objOfferingReportVM.ProductDesc),
        //            General.GetVariableName(() => m_objOfferingReportVM.FormulaID),
        //            General.GetVariableName(() => m_objOfferingReportVM.UnitStartRent),
        //            General.GetVariableName(() => m_objOfferingReportVM.UnitEndRent)
        //        }).AsEnumerable()
        //        .Select(m_objDraftBillGenVM => new ContractUnitVM()
        //        {
        //            PaymentTermID = m_objDraftBillGenVM[General.GetVariableName(() => m_objOfferingReportVM.PaymentTermID)].ToString(),
        //            ContractUnitID = string.Empty,
        //            MaterialID = m_objDraftBillGenVM[General.GetVariableName(() => m_objOfferingReportVM.MaterialID)].ToString(),
        //            MaterialDesc = string.Empty,
        //            ProductID = m_objDraftBillGenVM[General.GetVariableName(() => m_objOfferingReportVM.ProductID)].ToString(),
        //            ProductDesc = m_objDraftBillGenVM[General.GetVariableName(() => m_objOfferingReportVM.ProductDesc)].ToString(),
        //            FormulaID = m_objDraftBillGenVM[General.GetVariableName(() => m_objOfferingReportVM.FormulaID)].ToString(),
        //            StartDate = DateTime.Parse(m_objDraftBillGenVM[General.GetVariableName(() => m_objOfferingReportVM.UnitStartRent)].ToString()),
        //            EndDate = DateTime.Parse(m_objDraftBillGenVM[General.GetVariableName(() => m_objOfferingReportVM.UnitEndRent)].ToString())
        //        }).ToList();

        //        DataTable m_dtOfferingReport = m_dtTOfferingBL.Clone();
        //        List<BillGenVM> m_lstBillGenVM = new List<BillGenVM>();
        //        ContractController m_objContractController = new ContractController();
        //        List<ContractBillPlanVM> m_lstContractBillPlanVM = m_objContractController.GenerateContractBillPlanByContractUnit(m_lstContractUnitVM, ref m_strMessage);
        //        if (m_strMessage == string.Empty)
        //        {
        //            // update list
        //            int m_intBillPlanCount = 0;
        //            int m_intContractUnitCount = 0;
        //            m_lstContractBillPlanVM.Select(m_objContractBillPlanVM =>
        //            {
        //                m_objContractBillPlanVM.BillPlanID = (m_intBillPlanCount++).ToString();
        //                return m_objContractBillPlanVM;
        //            }).ToList();
        //            foreach (ContractUnitVM m_objContractUnitVM in m_lstContractUnitVM)
        //            {
        //                m_lstContractBillPlanVM.Where(m_objContractBillPlanVM => m_objContractBillPlanVM.MaterialID == m_objContractUnitVM.MaterialID
        //                    && m_objContractBillPlanVM.ProductID == m_objContractUnitVM.ProductID
        //                    && m_objContractBillPlanVM.StartDate == m_objContractUnitVM.StartDate)
        //                    .Select(m_objContractBillPlanVM =>
        //                    {
        //                        m_objContractBillPlanVM.ContractUnitID = (m_intContractUnitCount).ToString();
        //                        return m_objContractBillPlanVM;
        //                    }).ToList();
        //                m_intContractUnitCount++;
        //            }
        //            // Get Bill Data to calculate
        //            BillController m_objBillController = new BillController();
        //            Dictionary<string, List<object>> m_dicFilterInner = new Dictionary<string, List<object>>();
        //            List<BillGenVM> m_lstDraftBillGenVM = m_objBillController.GetBillToCalculate(offeringID, Screen.Offering, ref m_strMessage, ref m_dicFilterInner);
        //            if (m_strMessage == string.Empty)
        //            {
        //                foreach (ContractBillPlanVM m_objContractBillPlanVM in m_lstContractBillPlanVM)
        //                {
        //                    DataRow[] m_arrTOfferingBL = m_dtTOfferingBL.Select(General.GetVariableName(() => m_objOfferingReportVM.MaterialID) + " = '" + m_objContractBillPlanVM.MaterialID + "' AND "
        //                        + General.GetVariableName(() => m_objOfferingReportVM.ProductID) + " = '" + m_objContractBillPlanVM.ProductID + "' AND "
        //                        + General.GetVariableName(() => m_objOfferingReportVM.UnitStartRent) + " = '" + m_objContractBillPlanVM.StartDate + "'");
        //                    foreach (DataRow m_drTOfferingBL in m_arrTOfferingBL)
        //                    {
        //                        m_drTOfferingBL[General.GetVariableName(() => m_objContractBillPlanVM.BillPlanID)] = m_objContractBillPlanVM.BillPlanID;
        //                        m_dtOfferingReport.ImportRow(m_drTOfferingBL);
        //                    }

        //                    List<BillGenVM> m_lstDraftBillGenVMTemp = m_lstDraftBillGenVM.Where(m_objDraftBillGenVM => m_objDraftBillGenVM.MaterialID == m_objContractBillPlanVM.MaterialID
        //                        && m_objDraftBillGenVM.ProductID == m_objContractBillPlanVM.ProductID && m_objDraftBillGenVM.StartDate == m_objContractBillPlanVM.StartDate).ToList();
        //                    //BillGenVM[] m_arrDraftBillGenVMTemp = new BillGenVM[m_lstDraftBillGenVMTemp.Count];
        //                    //m_lstDraftBillGenVMTemp.CopyTo(m_arrDraftBillGenVMTemp);
        //                    foreach (BillGenVM m_objDraftBillGenVMTemp in m_lstDraftBillGenVMTemp)
        //                    {
        //                        BillGenVM m_objBillGenVM = (BillGenVM)m_objDraftBillGenVMTemp.Clone();
        //                        m_objBillGenVM.ContractUnitID = m_objContractBillPlanVM.ContractUnitID;
        //                        m_objBillGenVM.BillPlanID = m_objContractBillPlanVM.BillPlanID;
        //                        m_objBillGenVM.BillPeriodStart = m_objContractBillPlanVM.BillPeriodStart;
        //                        m_objBillGenVM.PaymentPeriodStart = m_objContractBillPlanVM.PaymentPeriodStart;
        //                        m_objBillGenVM.BillPeriodEnd = m_objContractBillPlanVM.BillPeriodEnd;
        //                        m_objBillGenVM.BillDate = m_objContractBillPlanVM.BillDate;
        //                        m_objBillGenVM.DueDate = m_objContractBillPlanVM.DueDate;
        //                        m_objBillGenVM.BillPortion = m_objContractBillPlanVM.BillPortion;
        //                        m_objBillGenVM.IsLockPrice = m_objContractBillPlanVM.IsLockPrice;
        //                        m_objBillGenVM.Amount = m_objContractBillPlanVM.Amount;
        //                        m_objBillGenVM.BillPlanStatusID = m_objContractBillPlanVM.StatusID;
        //                        m_objBillGenVM.Period = m_objContractBillPlanVM.PaymentPeriodEnd.ToString(Global.PeriodDateFormat);
        //                        m_lstBillGenVM.Add(m_objBillGenVM);
        //                    }
        //                }
        //            }

        //            ConfigController m_objConfigController = new ConfigController();
        //            List<ConfigVM> m_lstMappingParameter = m_objConfigController.GetMappingParameterList("RentalContractPrice");
        //            string m_strRentalContractPrice = (from m_objParameter in m_lstMappingParameter select m_objParameter.Desc1).ToList().FirstOrDefault();
        //            m_lstMappingParameter = m_objConfigController.GetMappingParameterList("Area");
        //            string m_strArea = (from m_objParameter in m_lstMappingParameter select m_objParameter.Desc1).ToList().FirstOrDefault();

        //            List<string> m_lstErrorOfferingID = new List<string>();
        //            List<BillBaseVM> m_lstBillBaseVM = new List<BillBaseVM>();
        //            List<BillMainFeeVM> m_lstAllBillMainFeeVM = new List<BillMainFeeVM>();
        //            List<BillBaseVM> m_lstAllBillBaseVM = new List<BillBaseVM>();
        //            List<string> m_lstMessage = m_objBillController.GenerateBillBaseAndMainFee(m_lstBillGenVM, true, ref m_lstErrorOfferingID, ref m_lstAllBillBaseVM, ref m_lstBillBaseVM,
        //                ref m_lstAllBillMainFeeVM);
        //            if (m_lstMessage.Count <= 0)
        //            {
        //                foreach (BillMainFeeVM m_objBillMainFeeVM in m_lstAllBillMainFeeVM)
        //                {
        //                    DataRow[] m_arrOfferingReport = m_dtOfferingReport.Select(General.GetVariableName(() => m_objOfferingReportVM.BillPlanID) + " = '" + m_objBillMainFeeVM.BillPlanID + "'");
        //                    foreach (DataRow m_drOfferingReport in m_arrOfferingReport)
        //                    {
        //                        //m_drTOfferingBL[General.GetVariableName(() => m_objOfferingReportVM.AmountPerPeriod)] = m_objBillMainFeeVM.AmountPerPeriod;
        //                        m_drOfferingReport[General.GetVariableName(() => m_objOfferingReportVM.Amount)] = 0;
        //                        m_drOfferingReport[General.GetVariableName(() => m_objOfferingReportVM.TaxAmount)] = 0;
        //                        m_drOfferingReport[General.GetVariableName(() => m_objOfferingReportVM.TotalAmount)] = 0;
        //                    }
        //                    foreach (DataRow m_drOfferingReport in m_arrOfferingReport)
        //                    {
        //                        //m_drTOfferingBL[General.GetVariableName(() => m_objOfferingReportVM.AmountPerPeriod)] = m_objBillMainFeeVM.AmountPerPeriod;
        //                        m_drOfferingReport[General.GetVariableName(() => m_objOfferingReportVM.Amount)] = decimal.Parse(m_drOfferingReport[General.GetVariableName(() =>
        //                            m_objOfferingReportVM.Amount)].ToString()) + m_objBillMainFeeVM.Amount;
        //                        m_drOfferingReport[General.GetVariableName(() => m_objOfferingReportVM.TaxAmount)] = decimal.Parse(m_drOfferingReport[General.GetVariableName(() =>
        //                            m_objOfferingReportVM.TaxAmount)].ToString()) + m_objBillMainFeeVM.TaxAmount;
        //                        m_drOfferingReport[General.GetVariableName(() => m_objOfferingReportVM.TotalAmount)] = decimal.Parse(m_drOfferingReport[General.GetVariableName(() =>
        //                             m_objOfferingReportVM.TotalAmount)].ToString()) + m_objBillMainFeeVM.MainFeeTotal;
        //                    }
        //                }
        //                m_lstAllBillBaseVM = (
        //                    from BillBaseVM m_objBillBaseVM in m_lstAllBillBaseVM
        //                    where m_objBillBaseVM.ParameterID == m_strRentalContractPrice || m_objBillBaseVM.ParameterID == m_strArea
        //                    select m_objBillBaseVM
        //                    ).Distinct().ToList();
        //                foreach (BillBaseVM m_objBillBaseVM in m_lstAllBillBaseVM)
        //                {
        //                    DataRow[] m_arrOfferingReport = m_dtOfferingReport.Select(General.GetVariableName(() => m_objOfferingReportVM.BillPlanID) + " = '" + m_objBillBaseVM.BillPlanID + "'");
        //                    foreach (DataRow m_drOfferingReport in m_arrOfferingReport)
        //                    {
        //                        if (m_objBillBaseVM.ParameterID == m_strArea)
        //                            m_drOfferingReport[General.GetVariableName(() => m_objOfferingReportVM.Quantity)] = 0;
        //                        else
        //                            m_drOfferingReport[General.GetVariableName(() => m_objOfferingReportVM.BaseAmount)] = 0;
        //                    }
        //                    foreach (DataRow m_drOfferingReport in m_arrOfferingReport)
        //                    {
        //                        if (m_objBillBaseVM.ParameterID == m_strArea)
        //                            m_drOfferingReport[General.GetVariableName(() => m_objOfferingReportVM.Quantity)] = decimal.Parse(m_drOfferingReport[General.GetVariableName(() =>
        //                                m_objOfferingReportVM.Quantity)].ToString()) + m_objBillBaseVM.Amount;
        //                        else
        //                            m_drOfferingReport[General.GetVariableName(() => m_objOfferingReportVM.BaseAmount)] = decimal.Parse(m_drOfferingReport[General.GetVariableName(() =>
        //                                m_objOfferingReportVM.BaseAmount)].ToString()) + m_objBillBaseVM.Amount;
        //                    }
        //                }
        //            }
        //        }
        //        List<string> m_lstOfferingMaterial = (
        //            from DataRow m_drOfferingReport in m_dtOfferingReport.Rows
        //            select m_drOfferingReport[General.GetVariableName(() => m_objOfferingReportVM.OfferingID)].ToString() + "~"
        //                + m_drOfferingReport[General.GetVariableName(() => m_objOfferingReportVM.MaterialID)]
        //            ).ToList();
        //        foreach (string m_strOfferingMaterial in m_lstOfferingMaterial)
        //        {
        //            DataRow[] m_arrOfferingReport = m_dtOfferingReport.Select(General.GetVariableName(() => m_objOfferingReportVM.OfferingID) + " + '~' + "
        //                + General.GetVariableName(() => m_objOfferingReportVM.MaterialID) + " = '" + m_strOfferingMaterial + "'");
        //            long m_lngTotalAmount = (long)m_arrOfferingReport.AsEnumerable().Sum(m_drTOfferingBL => decimal.Parse(m_drTOfferingBL[General.GetVariableName(() =>
        //                m_objOfferingReportVM.TotalAmount)].ToString()));
        //            string m_strTotalAmountDesc = Global.GetTerbilang((long)m_lngTotalAmount);
        //            string[] m_arrYearMonthDay = Global.YearMonthDayDiff(DateTime.Parse(m_arrOfferingReport[0][General.GetVariableName(() => m_objOfferingReportVM.UnitEndRent)].ToString()),
        //                DateTime.Parse(m_arrOfferingReport[0][General.GetVariableName(() => m_objOfferingReportVM.UnitStartRent)].ToString()));
        //            int m_intYear = 0;
        //            int m_intMonth = 0;
        //            int m_intDay = 0;

        //            if (m_arrYearMonthDay.Count() > 0)
        //            {
        //                m_intYear = int.Parse(m_arrYearMonthDay[0].ToString());
        //                m_intMonth = int.Parse(m_arrYearMonthDay[1].ToString());
        //                m_intDay = int.Parse(m_arrYearMonthDay[2].ToString());
        //            }
        //            foreach (DataRow m_drOfferingReport in m_arrOfferingReport)
        //            {
        //                m_drOfferingReport[General.GetVariableName(() => m_objOfferingReportVM.TotalAmountDesc)] = m_strTotalAmountDesc;
        //                m_drOfferingReport[General.GetVariableName(() => m_objOfferingReportVM.RentPeriod)] = (m_intYear == 0 ? "" : m_intYear.ToString() + " Tahun ")
        //                    + (m_intMonth == 0 ? "" : m_intMonth.ToString() + " Bulan ") + (m_intDay == 0 ? "" : m_intDay.ToString() + " Hari ");
        //            }
        //        }
        //        ((ReportDocument)Session["rpt" + _strReportUID]).Database.Tables[0].SetDataSource(m_dtOfferingReport);
        //    }
        //    else
        //        throw new Exception(m_objTOfferingBL.Message);
        //}

        //#region Report Offering
        //private void Offering(bool IsSingle, string Selected)
        //{
        //    string reportPath = string.Empty;

        //    if (IsSingle)
        //    {
        //        reportPath = Server.MapPath("~/" + Global.ReportFolder + _strReportOffering);
        //    }
        //    else
        //    {
        //        reportPath = Server.MapPath("~/" + Global.ReportFolder + _strReportOfferingMultipleItem);
        //    }
        //    ((ReportDocument)Session["rpt" + _strReportUID]).Load(reportPath);
        //    TBillBL m_objTBillBL = new TBillBL();
        //    m_objTBillBL.ConnectionStringName = Global.ConnStrConfigName;
        //    Dictionary<int, DataSet> m_dicTBillBL = new Dictionary<int, DataSet>();
        //    DataTable dt = new DataTable();

        //    //Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();

        //    //TOfferingBL m_objOfferingBL = new TOfferingBL();
        //    //m_objOfferingBL.ConnectionStringName = Global.ConnStrConfigName;
        //    //TOffering m_objOffering = new TOffering();
        //    OfferingVM m_objOfferingVM = new OfferingVM();

        //    //Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

        //    //if (!string.IsNullOrEmpty(Selected))
        //    //{
        //    //    List<object> m_lstFilter = new List<object>();
        //    //    m_lstFilter.Add(Operator.In);
        //    //    m_lstFilter.Add(Selected);
        //    //    m_objFilter.Add(m_objOfferingVM.MapField(() => m_objOfferingVM.OfferingID, false), m_lstFilter);
        //    //}

        //    // Dictionary<int, DataSet> m_dicMGLOfferingBL = m_objOfferingBL.SelectBC(0, null, false, null, m_objFilter, null, null);

        //    //List<OfferingVM> m_ListOfferingVM_Source = new List<OfferingVM>();
        //    //m_ListOfferingVM_Source = (
        //    //   from DataRow m_objOfferingBL2 in m_dicMGLOfferingBL[0].Tables[0].Rows
        //    //   select new OfferingVM()
        //    //   {
        //    //       OfferingID = Convert.ToString(m_objOfferingBL2[General.GetVariableName(() => m_objOfferingVM.OfferingID)]),
        //    //       CustomerName = Convert.ToString(m_objOfferingBL2[General.GetVariableName(() => m_objOfferingVM.CustomerName)]),
        //    //       CreatedDate = Convert.ToDateTime(m_objOfferingBL2[General.GetVariableName(() => m_objOfferingVM.CreatedDate)]),
        //    //       PlantDesc = Convert.ToString(m_objOfferingBL2[General.GetVariableName(() => m_objOfferingVM.PlantDesc)]),
        //    //       CompanyDesc = Convert.ToString(m_objOfferingBL2[General.GetVariableName(() => m_objOfferingVM.CompanyDesc)]),
        //    //       LatestOfferingID = Convert.ToString(m_objOfferingBL2[General.GetVariableName(() => m_objOfferingVM.LatestOfferingID)]),
        //    //       VendorFullName = Convert.ToString(m_objOfferingBL2[General.GetVariableName(() => m_objOfferingVM.VendorFullName)]),
        //    //       LatestContractEndDate = Convert.ToDateTime(m_objOfferingBL2[General.GetVariableName(() => m_objOfferingVM.LatestContractEndDate)]),
        //    //   }).ToList();



        //    string m_strMessage = string.Empty;

        //    if (IsSingle)
        //    {
        //        List<OfferingVM> m_lstOfferingVM = new List<OfferingVM>();
        //        m_objOfferingVM = GetOfferingPrintVM(Selected, ref m_strMessage);
        //        List<OfferingUnitVM> m_lstOfferingUnitVM = GetOfferingUnitVMForPrint(m_objOfferingVM.OfferingID, ref m_strMessage);
        //        m_lstOfferingVM.Add(PopulateReportSingleItem(m_objOfferingVM, m_lstOfferingUnitVM));

        //        dt = OfferingController.ToDataTable(m_lstOfferingVM);
        //    }
        //    else
        //    {
        //        OfferingVM m_OfferingVM = GetOfferingPrintVM(Selected, ref m_strMessage);
        //        List<OfferingUnitVM> m_lstOfferingUnitVM = GetOfferingUnitVMForPrint(m_OfferingVM.OfferingID, ref m_strMessage);
        //        List<OfferingVM> m_lstOfferingVM = PopulateReportMultipleItem(m_OfferingVM, m_lstOfferingUnitVM);

        //        ((ReportDocument)Session["rpt" + _strReportUID]).DataDefinition.FormulaFields["TotalDeposit"].Text = " '" + (m_lstOfferingVM.Sum(x => x.StallDeposit) == 0 ? "0" : m_lstOfferingVM.Sum(x => x.StallDeposit).ToString(Global.BillNumberFormat)) + "' ";
        //        ((ReportDocument)Session["rpt" + _strReportUID]).DataDefinition.FormulaFields["TotalHargaSewa"].Text = " '" + m_lstOfferingVM.Sum(x => x.TotalRentPrice).ToString(Global.BillNumberFormat) + "' ";
        //        ((ReportDocument)Session["rpt" + _strReportUID]).DataDefinition.FormulaFields["TotalPPN"].Text = " '" + (m_lstOfferingVM.Sum(x => x.PPN) == 0 ? "0" : m_lstOfferingVM.Sum(x => x.PPN).ToString(Global.BillNumberFormat)) + "' ";
        //        ((ReportDocument)Session["rpt" + _strReportUID]).DataDefinition.FormulaFields["TotalHargaSewaDanPPN"].Text = " '" + (m_lstOfferingVM.Sum(x => x.TotalRentPrice) + m_lstOfferingVM.Sum(x => x.PPN)).ToString(Global.BillNumberFormat) + "' ";

        //        dt = OfferingController.ToDataTable(m_lstOfferingVM);
        //    }
        //    if (m_objTBillBL.Success)
        //        ((ReportDocument)Session["rpt" + _strReportUID]).Database.Tables[0].SetDataSource(dt);

        //}

        //#region Method
        //private OfferingVM GetOfferingPrintVM(string Selected, ref string message)
        //{
        //    OfferingVM m_objOfferingVM = new OfferingVM();
        //    TOfferingBL m_objTOfferingBL = new TOfferingBL();
        //    m_objTOfferingBL.ConnectionStringName = Global.ConnStrConfigName;
        //    TOffering m_objTOffering = new TOffering();

        //    //foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
        //    //{
        //    //    if (m_objOfferingVM.IsKey(m_kvpSelectedRow.Key))
        //    //    {
        //    //        m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
        //    //        List<object> m_lstFilter = new List<object>();
        //    //        m_lstFilter.Add(Operator.Equals);
        //    //        m_lstFilter.Add(m_kvpSelectedRow.Value);
        //    //        m_objFilter.Add(m_objOfferingVM.MapField(m_kvpSelectedRow.Key, false), m_lstFilter);
        //    //    }
        //    //}

        //    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

        //    if (!string.IsNullOrEmpty(Selected))
        //    {
        //        List<object> m_lstFilter = new List<object>();
        //        m_lstFilter.Add(Operator.In);
        //        m_lstFilter.Add(Selected);
        //        m_objFilter.Add(m_objOfferingVM.MapField(() => m_objOfferingVM.OfferingID, false), m_lstFilter);
        //    }

        //    Dictionary<int, DataSet> m_dicTOfferingBL = m_objTOfferingBL.SelectReport(0, 1, false, null, m_objFilter, null);
        //    List<OfferingVM> m_lstOfferingVM = new List<OfferingVM>();
        //    if (m_objTOfferingBL.Message == string.Empty)
        //    {
        //        DataRow m_drTPaymentBL = m_dicTOfferingBL[0].Tables[0].Rows[0];
        //        m_objOfferingVM.OfferingID = m_drTPaymentBL[General.GetVariableName(() => m_objOfferingVM.OfferingID)].ToString();
        //        m_objOfferingVM.CustomerName = m_drTPaymentBL[General.GetVariableName(() => m_objOfferingVM.CustomerName)].ToString();
        //        m_objOfferingVM.CreatedDate = DateTime.Parse(m_drTPaymentBL[General.GetVariableName(() => m_objOfferingVM.CreatedDate)].ToString());
        //        m_objOfferingVM.PlantDesc = m_drTPaymentBL[General.GetVariableName(() => m_objOfferingVM.PlantDesc)].ToString();
        //        m_objOfferingVM.CompanyDesc = m_drTPaymentBL[General.GetVariableName(() => m_objOfferingVM.CompanyDesc)].ToString();
        //        m_objOfferingVM.LatestOfferingID = m_drTPaymentBL[General.GetVariableName(() => m_objOfferingVM.LatestOfferingID)].ToString();
        //        m_objOfferingVM.VendorFullName = m_drTPaymentBL[General.GetVariableName(() => m_objOfferingVM.VendorFullName)].ToString();
        //        m_objOfferingVM.LatestContractEndDate = String.IsNullOrEmpty(m_drTPaymentBL[General.GetVariableName(() => m_objOfferingVM.LatestContractEndDate)].ToString()) ? DateTime.MinValue : DateTime.Parse(m_drTPaymentBL[General.GetVariableName(() => m_objOfferingVM.LatestContractEndDate)].ToString());
        //    }
        //    else
        //    {
        //        message = m_objTOfferingBL.Message;
        //    }

        //    return m_objOfferingVM;
        //}
        //public List<OfferingUnitVM> GetOfferingUnitVMForPrint(string offeringID, ref string message)
        //{
        //    OfferingUnitVM m_objOfferingUnitVM = new OfferingUnitVM();
        //    DOfferingUnitBL m_objDOfferingUnitBL = new DOfferingUnitBL();
        //    m_objDOfferingUnitBL.ConnectionStringName = Global.ConnStrConfigName;

        //    List<string> m_lstSelect = new List<string>();
        //    m_lstSelect.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.MaterialID, true));
        //    m_lstSelect.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.MaterialDesc, true));
        //    m_lstSelect.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.ProductID, true));
        //    m_lstSelect.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.ProductDesc, true));
        //    m_lstSelect.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.StartDate, true));
        //    m_lstSelect.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.EndDate, true));
        //    m_lstSelect.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.IsBilled, true));
        //    m_lstSelect.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.FormulaID, true));
        //    m_lstSelect.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.FormulaCode, true));
        //    m_lstSelect.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.Formula, true));
        //    m_lstSelect.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.Price, true));
        //    m_lstSelect.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.Quantity, true));
        //    m_lstSelect.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.TaxID, true));
        //    m_lstSelect.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.TaxDesc, true));
        //    m_lstSelect.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.TaxPercentage, true));
        //    m_lstSelect.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.WithholdingID, true));
        //    m_lstSelect.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.WithholdingDesc, true));
        //    m_lstSelect.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.PaymentTermID, true));
        //    m_lstSelect.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.PaymentTermDesc, true));
        //    m_lstSelect.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.BillPortion, true));
        //    m_lstSelect.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.GroupField, true));
        //    m_lstSelect.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.UoMID, true));
        //    m_lstSelect.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.PricingPeriodDesc, true));
        //    m_lstSelect.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.MaterialGroupDesc, true));
        //    m_lstSelect.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.PricingID, true));
        //    m_lstSelect.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.BillCycleID, true));
        //    m_lstSelect.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.PricingPeriodID, true));
        //    m_lstSelect.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.StaffPosition, true));
        //    m_lstSelect.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.Head, true));
        //    m_lstSelect.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.HeadPosition, true));
        //    m_lstSelect.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.CalculationBaseID, true));
        //    m_lstSelect.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.PricingDesc, true));
        //    m_lstSelect.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.Manager, true));

        //    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
        //    List<object> m_lstFilter = new List<object>();
        //    m_lstFilter.Add(Operator.Equals);
        //    m_lstFilter.Add(offeringID);
        //    m_objFilter.Add(m_objOfferingUnitVM.MapField(() => m_objOfferingUnitVM.OfferingID, false), m_lstFilter);

        //    List<OfferingUnitVM> m_lstOfferingUnitVM = new List<OfferingUnitVM>();
        //    Dictionary<int, DataSet> m_dicDOfferingUnitBL = m_objDOfferingUnitBL.SelectBC(0, null, false, m_lstSelect, m_objFilter, null);
        //    if (m_objDOfferingUnitBL.Message == string.Empty)
        //    {
        //        m_lstOfferingUnitVM = (
        //            from DataRow m_drDOfferingUnitBL in m_dicDOfferingUnitBL[0].Tables[0].Rows
        //            select new OfferingUnitVM()
        //            {
        //                MaterialID = m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.MaterialID)].ToString(),
        //                MaterialDesc = m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.MaterialDesc)].ToString(),
        //                ProductID = m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.ProductID)].ToString(),
        //                ProductDesc = m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.ProductDesc)].ToString(),
        //                StartDate = DateTime.Parse(m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.StartDate)].ToString()),
        //                EndDate = DateTime.Parse(m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.EndDate)].ToString()),
        //                IsBilled = bool.Parse(m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.IsBilled)].ToString()),
        //                FormulaID = m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.FormulaID)].ToString(),
        //                FormulaCode = m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.FormulaCode)].ToString(),
        //                Formula = m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.Formula)].ToString(),
        //                Price = decimal.Parse(m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.Price)].ToString()),
        //                Quantity = decimal.Parse(m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.Quantity)].ToString()),
        //                TaxID = m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.TaxID)].ToString(),
        //                TaxDesc = m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.TaxDesc)].ToString(),
        //                TaxPercentage = decimal.Parse(m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.TaxPercentage)].ToString()),
        //                WithholdingID = m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.WithholdingID)].ToString(),
        //                WithholdingDesc = m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.WithholdingDesc)].ToString(),
        //                PaymentTermID = m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.PaymentTermID)].ToString(),
        //                PaymentTermDesc = m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.PaymentTermDesc)].ToString(),
        //                BillPortion = m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.BillPortion)].ToString(),
        //                GroupField = m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.GroupField)].ToString(),
        //                UoMID = m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.UoMID)].ToString(),
        //                PricingPeriodDesc = m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.PricingPeriodDesc)].ToString(),
        //                MaterialGroupDesc = m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.MaterialGroupDesc)].ToString(),
        //                Floor = m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.Floor)] == null ? "" : m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.Floor)].ToString(),
        //                Block = m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.Block)] == null ? "" : m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.Block)].ToString(),
        //                Unit = m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.Unit)] == null ? "" : m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.Unit)].ToString(),
        //                Area = m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.Area)] == null ? "" : m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.Area)].ToString(),
        //                PricingID = m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.PricingID)].ToString(),
        //                BillCycleID = m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.BillCycleID)].ToString(),
        //                PricingPeriodID = int.Parse(m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.PricingPeriodID)].ToString()),
        //                StaffPosition = m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.StaffPosition)].ToString(),
        //                Head = m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.Head)].ToString(),
        //                HeadPosition = m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.HeadPosition)].ToString(),
        //                CalculationBaseID = m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.CalculationBaseID)].ToString(),
        //                PricingDesc = m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.PricingDesc)].ToString(),
        //                Manager = m_drDOfferingUnitBL[General.GetVariableName(() => m_objOfferingUnitVM.Manager)].ToString()
        //            }).ToList();
        //    }
        //    else
        //        message = m_objDOfferingUnitBL.Message;

        //    return m_lstOfferingUnitVM;
        //}
        //private OfferingVM PopulateReportSingleItem(OfferingVM offeringVM, List<OfferingUnitVM> lstOfferingUnitVM)
        //{
        //    #region check for Rental product
        //    ConfigVM m_objProductRental = new ConfigVM();
        //    UConfigBL m_objUConfigBL = new UConfigBL();
        //    m_objUConfigBL.ConnectionStringName = Global.ConnStrConfigName;
        //    UConfig m_objUConfig = new UConfig();

        //    List<string> m_lstSelect = new List<string>();
        //    m_lstSelect.Add(m_objProductRental.MapField(() => m_objProductRental.Key1, true));
        //    m_lstSelect.Add(m_objProductRental.MapField(() => m_objProductRental.Desc1, true));

        //    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

        //    List<object> m_lstFilter = new List<object>();
        //    m_lstFilter.Add(Operator.Equals);
        //    m_lstFilter.Add("Mapping");
        //    m_objFilter.Add(m_objProductRental.MapField(() => m_objProductRental.Key1, false), m_lstFilter);

        //    m_lstFilter = new List<object>();
        //    m_lstFilter.Add(Operator.Equals);
        //    m_lstFilter.Add("ProductID");
        //    m_objFilter.Add(m_objProductRental.MapField(() => m_objProductRental.Key2, false), m_lstFilter);

        //    m_lstFilter = new List<object>();
        //    m_lstFilter.Add(Operator.Equals);
        //    m_lstFilter.Add("Rental");
        //    m_objFilter.Add(m_objProductRental.MapField(() => m_objProductRental.Key3, false), m_lstFilter);

        //    Dictionary<int, DataSet> m_dicUConfigBL = m_objUConfigBL.SelectBC(0, null, false, m_lstSelect, m_objFilter, null);
        //    if (m_objUConfigBL.Message == string.Empty)
        //    {
        //        DataRow m_drUConfig = m_dicUConfigBL[0].Tables[0].Rows[0];
        //        m_objProductRental.Key1 = m_drUConfig[General.GetVariableName(() => m_objProductRental.Key1)].ToString();
        //        m_objProductRental.Desc1 = m_drUConfig[General.GetVariableName(() => m_objProductRental.Desc1)].ToString();
        //    }
        //    #endregion

        //    #region Check for Deposit Kios Product
        //    ConfigVM m_objProductDeposit = new ConfigVM();

        //    m_lstSelect = new List<string>();
        //    m_lstSelect.Add(m_objProductDeposit.MapField(() => m_objProductDeposit.Key1, true));
        //    m_lstSelect.Add(m_objProductDeposit.MapField(() => m_objProductDeposit.Desc1, true));

        //    m_objFilter = new Dictionary<string, List<object>>();

        //    m_lstFilter = new List<object>();
        //    m_lstFilter.Add(Operator.Equals);
        //    m_lstFilter.Add("Mapping");
        //    m_objFilter.Add(m_objProductDeposit.MapField(() => m_objProductDeposit.Key1, false), m_lstFilter);

        //    m_lstFilter = new List<object>();
        //    m_lstFilter.Add(Operator.Equals);
        //    m_lstFilter.Add("ProductID");
        //    m_objFilter.Add(m_objProductDeposit.MapField(() => m_objProductDeposit.Key2, false), m_lstFilter);

        //    m_lstFilter = new List<object>();
        //    m_lstFilter.Add(Operator.Equals);
        //    m_lstFilter.Add("SecurityDepositSewa");
        //    m_objFilter.Add(m_objProductDeposit.MapField(() => m_objProductDeposit.Key3, false), m_lstFilter);


        //    m_dicUConfigBL = m_objUConfigBL.SelectBC(0, null, false, m_lstSelect, m_objFilter, null);
        //    if (m_objUConfigBL.Message == string.Empty)
        //    {
        //        DataRow m_drUConfig = m_dicUConfigBL[0].Tables[0].Rows[0];
        //        m_objProductDeposit.Key1 = m_drUConfig[General.GetVariableName(() => m_objProductDeposit.Key1)].ToString();
        //        m_objProductDeposit.Desc1 = m_drUConfig[General.GetVariableName(() => m_objProductDeposit.Desc1)].ToString();
        //    }
        //    #endregion

        //    List<string> m_lstMessage = new List<string>();

        //    foreach (OfferingUnitVM unit in lstOfferingUnitVM)
        //    {
        //        BillController billController = new BillController();
        //        BillBaseVM m_objBillBaseVM = new BillBaseVM();

        //        Dictionary<string, object> m_objParameters = new Dictionary<string, object>();
        //        m_objParameters.Add("ProductID", unit.ProductID);
        //        m_objParameters.Add("MaterialID", unit.MaterialID);
        //        m_objParameters.Add("OfferingID", offeringVM.OfferingID);

        //        Dictionary<string, object> returnValue = new Dictionary<string, object>();
        //        object returnValueBase;
        //        string m_strMessage = "";
        //        if ((!String.IsNullOrEmpty(m_objProductRental.Desc1)) && m_objProductRental.Desc1.Equals(unit.ProductID))
        //        {
        //            MaterialController m_objMaterialController = new MaterialController();
        //            Dictionary<string, object> selected = new Dictionary<string, object>();
        //            List<MaterialParameterVM> m_lstMaterialParameterVM = m_objMaterialController.GetMaterialParameterVM(unit.MaterialID, ref m_strMessage);
        //            string m_strFloor = String.IsNullOrEmpty(unit.Floor) ? "-" : unit.Floor;
        //            string m_strUnit = String.IsNullOrEmpty(unit.Unit) ? "-" : unit.Unit;
        //            string m_strBlock = String.IsNullOrEmpty(unit.Block) ? "-" : unit.Block;



        //            offeringVM.FloorBlockNo = String.Format("Lt.{0} Blok {1} No. {2}", m_strFloor, m_strBlock, m_strUnit);
        //            string[] m_stryearDiff = Global.YearMonthDayDiff(unit.EndDate, unit.StartDate);
        //            offeringVM.PeriodDesc = String.Format("{0} {1} {2}", m_stryearDiff[0] == "0" ? "" : String.Format("{0} tahun", m_stryearDiff[0]), m_stryearDiff[1] == "0" ? "" : String.Format("{0} bulan", m_stryearDiff[1]), m_stryearDiff[2] == "0" ? "" : String.Format("{0} hari", m_stryearDiff[2]));

        //            returnValue = billController.CalculateContractBillPlans(m_objParameters, null);
        //            returnValue.TryGetValue("BillBaseVM", out returnValueBase);
        //            m_objBillBaseVM = (BillBaseVM)returnValueBase;
        //            offeringVM.RentPrice = m_objBillBaseVM.RawAmount;
        //            offeringVM.TotalRentPrice = m_objBillBaseVM.Amount;
        //            offeringVM.Size = String.Format("{0} {1}", Math.Round(m_objBillBaseVM.AOU, 2).ToString(), unit.UoMID);

        //            string[] m_arrBillPortion = unit.BillPortion.Split(';');
        //            if (m_arrBillPortion.Any())
        //            {
        //                if (m_arrBillPortion.Length == 1)
        //                {
        //                    offeringVM.PaymentTermDesc = String.Format("{0}%", m_arrBillPortion[0]);
        //                }
        //                else
        //                {
        //                    int m_intRemain = 100 - int.Parse(m_arrBillPortion[0]);
        //                    offeringVM.PaymentTermDesc = String.Format("DP {0}%, Sisa {1}% diangsur {2} x", m_arrBillPortion[0], m_intRemain, m_arrBillPortion.Length - 1);

        //                }
        //            }
        //            else
        //            {
        //                offeringVM.PaymentTermDesc = unit.PaymentTermDesc;
        //            }

        //            offeringVM.UoMID = unit.UoMID;
        //            offeringVM.PricingPeriodDesc = String.Format("({0})", unit.PricingDesc);
        //            offeringVM.MaterialGroupDesc = unit.MaterialGroupDesc;
        //            offeringVM.Quantity = lstOfferingUnitVM.Where(x => x.ProductID == m_objProductRental.Desc1).ToList().Count;
        //            offeringVM.StartDate = unit.StartDate;
        //            offeringVM.EndDate = unit.EndDate;
        //            offeringVM.PPN = unit.TaxPercentage * offeringVM.TotalRentPrice / 100;
        //            offeringVM.StaffPosition = unit.StaffPosition;
        //            offeringVM.Head = unit.Head;
        //            offeringVM.HeadPosition = unit.HeadPosition;
        //            if (!String.IsNullOrEmpty(unit.Manager))
        //            {
        //                offeringVM.VendorFullName = unit.Manager;
        //            }
        //        }
        //        else if ((!String.IsNullOrEmpty(m_objProductDeposit.Desc1)) && m_objProductDeposit.Desc1.Equals(unit.ProductID))
        //        {
        //            if (unit.Price == 0)
        //            {
        //                returnValue = billController.CalculateContractBillPlans(m_objParameters, null);
        //                returnValue.TryGetValue("BillBaseVM", out returnValueBase);
        //                m_objBillBaseVM = (BillBaseVM)returnValueBase;
        //                offeringVM.StallDeposit = m_objBillBaseVM.RawAmount;
        //            }
        //            else
        //            {
        //                offeringVM.StallDeposit = unit.Price;
        //            }
        //        }
        //    }
        //    return offeringVM;
        //}
        //private List<OfferingVM> PopulateReportMultipleItem(OfferingVM offeringVM, List<OfferingUnitVM> lstOfferingUnitVM)
        //{
        //    List<OfferingVM> m_lstOfferingVM = new List<OfferingVM>();

        //    #region check for Rental product
        //    ConfigVM m_objProductRental = new ConfigVM();
        //    UConfigBL m_objUConfigBL = new UConfigBL();
        //    m_objUConfigBL.ConnectionStringName = Global.ConnStrConfigName;
        //    UConfig m_objUConfig = new UConfig();

        //    List<string> m_lstSelect = new List<string>();
        //    m_lstSelect.Add(m_objProductRental.MapField(() => m_objProductRental.Key1, true));
        //    m_lstSelect.Add(m_objProductRental.MapField(() => m_objProductRental.Desc1, true));

        //    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

        //    List<object> m_lstFilter = new List<object>();
        //    m_lstFilter.Add(Operator.Equals);
        //    m_lstFilter.Add("Mapping");
        //    m_objFilter.Add(m_objProductRental.MapField(() => m_objProductRental.Key1, false), m_lstFilter);

        //    m_lstFilter = new List<object>();
        //    m_lstFilter.Add(Operator.Equals);
        //    m_lstFilter.Add("ProductID");
        //    m_objFilter.Add(m_objProductRental.MapField(() => m_objProductRental.Key2, false), m_lstFilter);

        //    m_lstFilter = new List<object>();
        //    m_lstFilter.Add(Operator.Equals);
        //    m_lstFilter.Add("Rental");
        //    m_objFilter.Add(m_objProductRental.MapField(() => m_objProductRental.Key3, false), m_lstFilter);

        //    Dictionary<int, DataSet> m_dicUConfigBL = m_objUConfigBL.SelectBC(0, null, false, m_lstSelect, m_objFilter, null);
        //    if (m_objUConfigBL.Message == string.Empty)
        //    {
        //        DataRow m_drUConfig = m_dicUConfigBL[0].Tables[0].Rows[0];
        //        m_objProductRental.Key1 = m_drUConfig[General.GetVariableName(() => m_objProductRental.Key1)].ToString();
        //        m_objProductRental.Desc1 = m_drUConfig[General.GetVariableName(() => m_objProductRental.Desc1)].ToString();
        //    }
        //    #endregion

        //    #region Check for Deposit Kios Product
        //    ConfigVM m_objProductDeposit = new ConfigVM();

        //    m_lstSelect = new List<string>();
        //    m_lstSelect.Add(m_objProductDeposit.MapField(() => m_objProductDeposit.Key1, true));
        //    m_lstSelect.Add(m_objProductDeposit.MapField(() => m_objProductDeposit.Desc1, true));

        //    m_objFilter = new Dictionary<string, List<object>>();

        //    m_lstFilter = new List<object>();
        //    m_lstFilter.Add(Operator.Equals);
        //    m_lstFilter.Add("Mapping");
        //    m_objFilter.Add(m_objProductDeposit.MapField(() => m_objProductDeposit.Key1, false), m_lstFilter);

        //    m_lstFilter = new List<object>();
        //    m_lstFilter.Add(Operator.Equals);
        //    m_lstFilter.Add("ProductID");
        //    m_objFilter.Add(m_objProductDeposit.MapField(() => m_objProductDeposit.Key2, false), m_lstFilter);

        //    m_lstFilter = new List<object>();
        //    m_lstFilter.Add(Operator.Equals);
        //    m_lstFilter.Add("SecurityDepositSewa");
        //    m_objFilter.Add(m_objProductDeposit.MapField(() => m_objProductDeposit.Key3, false), m_lstFilter);


        //    m_dicUConfigBL = m_objUConfigBL.SelectBC(0, null, false, m_lstSelect, m_objFilter, null);
        //    if (m_objUConfigBL.Message == string.Empty)
        //    {
        //        DataRow m_drUConfig = m_dicUConfigBL[0].Tables[0].Rows[0];
        //        m_objProductDeposit.Key1 = m_drUConfig[General.GetVariableName(() => m_objProductDeposit.Key1)].ToString();
        //        m_objProductDeposit.Desc1 = m_drUConfig[General.GetVariableName(() => m_objProductDeposit.Desc1)].ToString();
        //    }
        //    #endregion

        //    List<string> m_lstGroupField = lstOfferingUnitVM.Select(x => x.GroupField).Distinct().ToList();
        //    foreach (string groupField in m_lstGroupField)
        //    {
        //        OfferingVM m_objOfferingVM = new OfferingVM();
        //        m_objOfferingVM.OfferingID = offeringVM.OfferingID;
        //        m_objOfferingVM.VendorID = offeringVM.VendorID;
        //        m_objOfferingVM.VendorFullName = offeringVM.VendorFullName;
        //        m_objOfferingVM.CustomerID = offeringVM.CustomerID;
        //        m_objOfferingVM.CustomerFullName = offeringVM.CustomerFullName;
        //        m_objOfferingVM.CustomerName = offeringVM.CustomerName;
        //        m_objOfferingVM.CombinedCustomerName = offeringVM.CombinedCustomerName;
        //        m_objOfferingVM.CustomerAddress = offeringVM.CustomerAddress;
        //        m_objOfferingVM.CombinedCustomerAddress = offeringVM.CombinedCustomerAddress;
        //        m_objOfferingVM.CustomerPhone = offeringVM.CustomerPhone;
        //        m_objOfferingVM.CombinedCustomerPhone = offeringVM.CombinedCustomerPhone;
        //        m_objOfferingVM.PlantID = offeringVM.PlantID;
        //        m_objOfferingVM.PlantDesc = offeringVM.PlantDesc;
        //        m_objOfferingVM.CompanyDesc = offeringVM.CompanyDesc;
        //        m_objOfferingVM.CreatedDate = offeringVM.CreatedDate;
        //        m_objOfferingVM.LatestOfferingID = offeringVM.LatestOfferingID;
        //        m_objOfferingVM.LatestContractEndDate = offeringVM.LatestContractEndDate;

        //        List<OfferingUnitVM> m_lstOfferingUnitVM = lstOfferingUnitVM.Where(x => x.GroupField == groupField).ToList();
        //        foreach (OfferingUnitVM unit in m_lstOfferingUnitVM)
        //        {
        //            BillController billController = new BillController();
        //            BillBaseVM m_objBillBaseVM = new BillBaseVM();

        //            Dictionary<string, object> m_objParameters = new Dictionary<string, object>();
        //            m_objParameters.Add("ProductID", unit.ProductID);
        //            m_objParameters.Add("MaterialID", unit.MaterialID);
        //            m_objParameters.Add("OfferingID", m_objOfferingVM.OfferingID);

        //            Dictionary<string, object> returnValue = new Dictionary<string, object>();
        //            object returnValueBase;
        //            string m_strMessage = "";
        //            if ((!String.IsNullOrEmpty(m_objProductRental.Desc1)) && m_objProductRental.Desc1.Equals(unit.ProductID))
        //            {
        //                MaterialController m_objMaterialController = new MaterialController();
        //                Dictionary<string, object> selected = new Dictionary<string, object>();
        //                List<MaterialParameterVM> m_lstMaterialParameterVM = m_objMaterialController.GetMaterialParameterVM(unit.MaterialID, ref m_strMessage);
        //                string m_strFloor = String.IsNullOrEmpty(unit.Floor) ? "-" : unit.Floor;
        //                string m_strUnit = String.IsNullOrEmpty(unit.Unit) ? "-" : unit.Unit;
        //                string m_strBlock = String.IsNullOrEmpty(unit.Block) ? "-" : unit.Block;

        //                m_objOfferingVM.FloorBlockNo = String.Format("Lt.{0} Blok {1} No. {2}", m_strFloor, m_strBlock, m_strUnit);
        //                string[] m_stryearDiff = Global.YearMonthDayDiff(unit.EndDate, unit.StartDate);
        //                m_objOfferingVM.PeriodDesc = String.Format("{0} {1} {2}", m_stryearDiff[0] == "0" ? "" : String.Format("{0} tahun", m_stryearDiff[0]), m_stryearDiff[1] == "0" ? "" : String.Format("{0} bulan", m_stryearDiff[1]), m_stryearDiff[2] == "0" ? "" : String.Format("{0} hari", m_stryearDiff[2]));

        //                returnValue = billController.CalculateContractBillPlans(m_objParameters, null);
        //                returnValue.TryGetValue("BillBaseVM", out returnValueBase);
        //                m_objBillBaseVM = (BillBaseVM)returnValueBase;
        //                m_objOfferingVM.RentPrice = m_objBillBaseVM.RawAmount;
        //                m_objOfferingVM.TotalRentPrice = m_objBillBaseVM.Amount;
        //                m_objOfferingVM.Size = Math.Round(m_objBillBaseVM.AOU, 2).ToString();

        //                string[] m_arrBillPortion = unit.BillPortion.Split(';');
        //                if (m_arrBillPortion.Any())
        //                {
        //                    if (m_arrBillPortion.Length == 1)
        //                    {
        //                        m_objOfferingVM.PaymentTermDesc = String.Format("{0}%", m_arrBillPortion[0]);
        //                    }
        //                    else
        //                    {
        //                        int m_intRemain = 100 - int.Parse(m_arrBillPortion[0]);
        //                        m_objOfferingVM.PaymentTermDesc = String.Format("DP {0}%, Sisa {1}% diangsur {2} x", m_arrBillPortion[0], m_intRemain, m_arrBillPortion.Length - 1);
        //                    }
        //                }
        //                else
        //                {
        //                    m_objOfferingVM.PaymentTermDesc = unit.PaymentTermDesc;
        //                }

        //                m_objOfferingVM.UoMID = String.Format("({0})", unit.UoMID);
        //                m_objOfferingVM.PricingPeriodDesc = String.Format("({0})", unit.PricingDesc);
        //                m_objOfferingVM.MaterialGroupDesc = unit.MaterialGroupDesc;
        //                m_objOfferingVM.Quantity = lstOfferingUnitVM.Where(x => x.ProductID == m_objProductRental.Desc1).ToList().Count;
        //                m_objOfferingVM.StartDate = unit.StartDate;
        //                m_objOfferingVM.EndDate = unit.EndDate;
        //                m_objOfferingVM.PPN = unit.TaxPercentage * m_objOfferingVM.TotalRentPrice / 100;
        //                m_objOfferingVM.StaffPosition = unit.StaffPosition;
        //                m_objOfferingVM.Head = unit.Head;
        //                m_objOfferingVM.HeadPosition = unit.HeadPosition;
        //                if (!String.IsNullOrEmpty(unit.Manager))
        //                {
        //                    offeringVM.VendorFullName = unit.Manager;
        //                }
        //            }
        //            else if ((!String.IsNullOrEmpty(m_objProductDeposit.Desc1)) && m_objProductDeposit.Desc1.Equals(unit.ProductID))
        //            {
        //                if (unit.Price == 0)
        //                {
        //                    returnValue = billController.CalculateContractBillPlans(m_objParameters, null);
        //                    returnValue.TryGetValue("BillBaseVM", out returnValueBase);
        //                    m_objBillBaseVM = (BillBaseVM)returnValueBase;
        //                    m_objOfferingVM.StallDeposit = m_objBillBaseVM.RawAmount;
        //                }
        //                else
        //                {
        //                    m_objOfferingVM.StallDeposit = unit.Price;
        //                }
        //            }
        //        }
        //        m_lstOfferingVM.Add(m_objOfferingVM);
        //    }
        //    return m_lstOfferingVM;
        //}
        //#endregion

        //#endregion
    }
}