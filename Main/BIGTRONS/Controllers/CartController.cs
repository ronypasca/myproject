using Ciloci.Flee;
using com.SML.Lib.Common;
using com.SML.BIGTRONS.DataAccess;
using com.SML.BIGTRONS.Enum;
using com.SML.BIGTRONS.Models;
using com.SML.BIGTRONS.ViewModels;
using Ext.Net;
using Ext.Net.MVC;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using XMVC = Ext.Net.MVC;
using NPOI.HSSF.UserModel;
using System.IO;
using NPOI.SS.UserModel;

namespace com.SML.BIGTRONS.Controllers
{
    public class CartController : BaseController
    {
        private readonly string title = "Cart";
        private readonly string dataSessionName = "FormData";
        private readonly string detailCartSessionName = "DetailCart";

        #region Public Action

        public ActionResult Index()
        {
            //base.Initialize();
            ViewBag.Title = title;
            return View();
        }
        public ActionResult List()
        {
            //Global.HasAccess = GetHasAccess();
            //if (!Global.HasAccess.Read)
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            if (Session[dataSessionName] != null)
                Session[dataSessionName] = null;

            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 0;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.HomePage),
                RenderMode = RenderMode.AddTo,
                ViewName = "_List",
                WrapByScriptTag = false
            };
        }
        public ActionResult  ClearExcelFile(string filename)
        {
            string fullPath = Request.MapPath("~/Content/" + filename);
            //if (System.IO.File.Exists(fullPath))
            //    System.IO.File.Delete(fullPath);
            return this.Direct();
        }
        public ActionResult ExportExcelReturnDirect(string Selected,string GridStructure, string ItemTypeID)
        {
            string m_strMessage = string.Empty;
            //Dictionary < string, object>  m_dicBudgetPlan =  new Dictionary<string, object>();
            //m_dicBudgetPlan.Add(BudgetPlanVM.Prop.BudgetPlanID.Name, BPID);
            //BudgetPlanVM m_objBudgetPlanVM = GetSelectedData(m_dicBudgetPlan,"", ref m_strMessage);
            BudgetPlanVM m_objBudgetPlanVM = JSON.Deserialize<BudgetPlanVM>(Selected);

           string m_strfilename = $"BUDGET PLAN - [{m_objBudgetPlanVM.Description}].xls";
            if (this.Request.Params["GridStructure"] != null)
            {
                DataTable m_dtBudgetPlanStructure = new DataTable();
                m_dtBudgetPlanStructure.Columns.Add(new DataColumn("No.", typeof(string)));
                m_dtBudgetPlanStructure.Columns.Add(new DataColumn(BudgetPlanVersionStructureVM.Prop.ItemDesc.Desc, typeof(string)));
                m_dtBudgetPlanStructure.Columns.Add(new DataColumn(BudgetPlanVersionStructureVM.Prop.Specification.Desc, typeof(string)));
                m_dtBudgetPlanStructure.Columns.Add(new DataColumn(BudgetPlanVersionStructureVM.Prop.UoMDesc.Desc, typeof(string)));
                m_dtBudgetPlanStructure.Columns.Add(new DataColumn(BudgetPlanVersionStructureVM.Prop.Volume.Desc, typeof(string)));
                m_dtBudgetPlanStructure.Columns.Add(new DataColumn("MAT", typeof(string)));
                m_dtBudgetPlanStructure.Columns.Add(new DataColumn("WAG", typeof(string)));
                m_dtBudgetPlanStructure.Columns.Add(new DataColumn("MISC", typeof(string)));
                //dt.Columns.Add(new DataColumn(BudgetPlanVersionStructureVM.Prop.WageAmount.Desc, typeof(string)));
                //dt.Columns.Add(new DataColumn(BudgetPlanVersionStructureVM.Prop.MiscAmount.Desc, typeof(string)));
                m_dtBudgetPlanStructure.Columns.Add(new DataColumn(BudgetPlanVersionStructureVM.Prop.TotalUnitPrice.Desc, typeof(string)));
                m_dtBudgetPlanStructure.Columns.Add(new DataColumn(BudgetPlanVersionStructureVM.Prop.Total.Desc, typeof(string)));

                Dictionary<string, object>[] m_arrBudgetStructureChild = JSON.Deserialize<Dictionary<string, object>[]>(this.Request.Params["GridStructure"]);
                if (m_arrBudgetStructureChild == null)
                    m_arrBudgetStructureChild = new List<Dictionary<string, object>>().ToArray();

                foreach (Dictionary<string, object> m_dicBudgetVersionStructureVM in m_arrBudgetStructureChild)
                {
                    bool m_boolAllowGetPrice = true;
                    if (m_dicBudgetVersionStructureVM.ContainsKey("displayprice"))
                        m_boolAllowGetPrice = Convert.ToBoolean(m_dicBudgetVersionStructureVM["displayprice"]);

                    //m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.ItemDesc.Name.ToLower()].ToString();
                    string No = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.SequenceDesc.Name.ToLower()] == null ? "" : m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.SequenceDesc.Name.ToLower()].ToString();
                    string ItemDesc = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.ItemDesc.Name.ToLower()] == null ? "" : m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.ItemDesc.Name.ToLower()].ToString();
                    string Specification = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.Specification.Name.ToLower()] == null ? "" : m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.Specification.Name.ToLower()].ToString();
                    string UoM = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.UoMID.Name.ToLower()] == null ? "" : m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.UoMID.Name.ToLower()].ToString();
                    string Volume = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.Volume.Name.ToLower()] == null ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.Volume.Name.ToLower()]).ToString("#,#0.00#");
                    string MAG = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name.ToLower()] == null ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name.ToLower()]).ToString("#,#");
                    string WAG = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.WageAmount.Name.ToLower()] == null ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.WageAmount.Name.ToLower()]).ToString("#,#");
                    string MISC = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.MiscAmount.Name.ToLower()] == null ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.MiscAmount.Name.ToLower()]).ToString("#,#");
                    string TotalUP = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.TotalUnitPrice.Name.ToLower()] == null ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.TotalUnitPrice.Name.ToLower()]).ToString("#,#");
                    string Total = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.Total.Name.ToLower()] == null ? "" : Convert.ToDecimal(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.Total.Name.ToLower()]).ToString("#,#");
                    string ItemTypeId = m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.ItemTypeID.Name.ToLower()] == null ? "" : m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.ItemTypeID.Name.ToLower()].ToString();

                    if (No.Split('.').ToList().Count() == 1 && No != string.Empty && No != "1")
                    {
                        m_dtBudgetPlanStructure.Rows.Add("", "", "", "", "", "", "", "", "", "");
                    }

                    switch (ItemTypeId)
                    {
                        case "BOI": m_dtBudgetPlanStructure.Rows.Add(No, ItemDesc, Specification, UoM, Volume, MAG, WAG, MISC, TotalUP, Total);  break;
                        case "AHS": {
                                if (string.IsNullOrEmpty(ItemTypeID))
                                {
                                    decimal totalup = (MAG == "" ? 0 : Convert.ToDecimal(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name.ToLower()])) + (WAG == "" ? 0 : Convert.ToDecimal(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.WageAmount.Name.ToLower()])) + (MISC == "" ? 0 : Convert.ToDecimal(m_dicBudgetVersionStructureVM[BudgetPlanVersionStructureVM.Prop.MiscAmount.Name.ToLower()]));
                                    m_dtBudgetPlanStructure.Rows.Add(No, ItemDesc, Specification, UoM, "", MAG, WAG, MISC, totalup.ToString("#,#"), Total);
                                }
                                break;
                            }
                        case "": m_dtBudgetPlanStructure.Rows.Add(No, ItemDesc, "", "", "", "", "", "", "", Total);break;
                        default:
                            {
                                if (string.IsNullOrEmpty(ItemTypeID))
                                {
                                    if (!m_boolAllowGetPrice)
                                        m_dtBudgetPlanStructure.Rows.Add(No, ItemDesc, Specification, UoM, "", "", "", "", "", Total);
                                    else
                                        m_dtBudgetPlanStructure.Rows.Add(No, ItemDesc, Specification, UoM, "", MAG, WAG, MISC, "", Total); ;
                                }
                            }break;
                    }
                    
                }
                
                ExportExcel(m_dtBudgetPlanStructure, m_strfilename);

            }
            return this.Direct(m_strfilename);
        }
        public void ExportExcel(DataTable sourceTable, string fileName)
        {
            HSSFWorkbook m_objWorkbook = new HSSFWorkbook();
            MemoryStream m_objmemoryStream = new MemoryStream();
            HSSFSheet m_objsheet = (HSSFSheet)m_objWorkbook.CreateSheet("BPL");
            HSSFRow m_objTitleRow = (HSSFRow)m_objsheet.CreateRow(0);
            HSSFRow m_objheaderRow = (HSSFRow)m_objsheet.CreateRow(1);

            var FontReg = m_objWorkbook.CreateFont();
            FontReg.FontHeightInPoints = 11;
            FontReg.FontName = "Calibri";
            FontReg.IsBold = false;

            var FontBold = m_objWorkbook.CreateFont();
            FontBold.FontHeightInPoints = 11;
            FontBold.FontName = "Calibri";
            FontBold.IsBold = true;


            #region Title
            m_objTitleRow.HeightInPoints = 26;

            var m_objTitleFont = m_objWorkbook.CreateFont();
            m_objTitleFont.FontHeightInPoints = 20;
            m_objTitleFont.FontName = "Calibri";
            m_objTitleFont.IsBold = true;

            HSSFCell m_objtitleCell = (HSSFCell)m_objTitleRow.CreateCell(0);
            m_objtitleCell.SetCellValue(fileName.Replace(".xls",""));

            var mergedTitlecell = new NPOI.SS.Util.CellRangeAddress(m_objTitleRow.RowNum, m_objTitleRow.RowNum, 0, sourceTable.Columns.Count-1);
            m_objsheet.AddMergedRegion(mergedTitlecell);

            HSSFCellStyle titleCellStyle = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            titleCellStyle.Alignment = HorizontalAlignment.Center;
            titleCellStyle.SetFont(m_objTitleFont);
            titleCellStyle.WrapText = true;
            titleCellStyle.VerticalAlignment = VerticalAlignment.Center;
            titleCellStyle.ShrinkToFit = true;
            m_objtitleCell.CellStyle = titleCellStyle;
            #endregion

            #region Header
            // Create Style
            HSSFCellStyle headerCellStyle = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            //headerCellStyle.FillForegroundColor = HSSFColor.Aqua.Index;
            headerCellStyle.FillPattern = FillPattern.SparseDots;
            headerCellStyle.VerticalAlignment = VerticalAlignment.Center;
            headerCellStyle.Alignment = HorizontalAlignment.Center;
            headerCellStyle.BorderBottom = BorderStyle.Thin;
            headerCellStyle.BorderTop = BorderStyle.Thin;
            headerCellStyle.BorderRight = BorderStyle.Thin;
            headerCellStyle.SetFont(FontBold);
            headerCellStyle.WrapText = true;

            // Handling header..
            int columnum = 0;
            foreach (DataColumn column in sourceTable.Columns)
            {
                // Create New Cell

                m_objheaderRow.Height = 700;
                HSSFCell headerCell = (HSSFCell)m_objheaderRow.CreateCell(column.Ordinal);

                
                // Set Cell Value
                if (columnum == 5)
                    headerCell.SetCellValue("Unit Price");
                else if (columnum == 6)
                    headerCell.SetCellValue("");
                else if (columnum == 7)
                    headerCell.SetCellValue("");
                else
                    headerCell.SetCellValue(column.ColumnName);

                headerCell.CellStyle = headerCellStyle;

                if (columnum == 5 || columnum == 6 || columnum == 7)
                {
                    if (columnum == 5)
                    {
                        var mergedcell = new NPOI.SS.Util.CellRangeAddress(m_objheaderRow.RowNum, 1, columnum, (columnum + 2));
                        m_objsheet.AddMergedRegion(mergedcell);
                    }
                }
                else
                {
                    var mergedcell = new NPOI.SS.Util.CellRangeAddress(m_objheaderRow.RowNum, 2, columnum, columnum);
                    m_objsheet.AddMergedRegion(mergedcell);
                }
                columnum++;
            }

            //Margin Row
            HSSFRow m_objdataRowMargin = (HSSFRow)m_objsheet.CreateRow(2);
            int NumberColumnMargin = 0;
            foreach (DataColumn column in sourceTable.Columns)
            {
                // Set Cell With Style
                if (column.Ordinal == 5)
                {
                    HSSFCell itemCell = (HSSFCell)m_objdataRowMargin.CreateCell(column.Ordinal);
                    itemCell.SetCellValue(BudgetPlanVersionStructureVM.Prop.MaterialAmount.Desc);
                    itemCell.CellStyle = headerCellStyle;
                }
                else if (column.Ordinal == 6)
                {
                    HSSFCell itemCell = (HSSFCell)m_objdataRowMargin.CreateCell(column.Ordinal);
                    itemCell.SetCellValue(BudgetPlanVersionStructureVM.Prop.WageAmount.Desc);
                    itemCell.CellStyle = headerCellStyle;
                }
                else if (column.Ordinal == 7)
                {
                    HSSFCell itemCell = (HSSFCell)m_objdataRowMargin.CreateCell(column.Ordinal);
                    itemCell.SetCellValue(BudgetPlanVersionStructureVM.Prop.MiscAmount.Desc);
                    itemCell.CellStyle = headerCellStyle;
                }
                //else
                //{
                //    HSSFCell itemCell = (HSSFCell)m_objdataRowMargin.CreateCell(column.Ordinal);
                //    itemCell.SetCellValue("");
                //    itemCell.CellStyle = headerCellStyle;
                //}
                NumberColumnMargin++;
            }

            #endregion

            #region Data
            //Create Style for col 1
            HSSFCellStyle itemCellStyleCol1FontReg = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellStyleCol1FontReg.BorderBottom = BorderStyle.Thin;
            itemCellStyleCol1FontReg.BorderRight = BorderStyle.Thin;
            itemCellStyleCol1FontReg.WrapText = true;           
            itemCellStyleCol1FontReg.VerticalAlignment = VerticalAlignment.Top;
            itemCellStyleCol1FontReg.Alignment = HorizontalAlignment.Right;
            itemCellStyleCol1FontReg.SetFont(FontReg);

            HSSFCellStyle itemCellStyleCol1FontBold = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellStyleCol1FontBold.BorderBottom = BorderStyle.Thin;
            itemCellStyleCol1FontBold.BorderRight = BorderStyle.Thin;
            itemCellStyleCol1FontBold.WrapText = true;
            itemCellStyleCol1FontBold.VerticalAlignment = VerticalAlignment.Top;
            itemCellStyleCol1FontBold.Alignment = HorizontalAlignment.Right;
            itemCellStyleCol1FontBold.SetFont(FontBold);

            //Create Style for col 2
            List<HSSFCellStyle> lst_indentStyle = new List<HSSFCellStyle>();

            HSSFCellStyle indentStyle = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            indentStyle.BorderBottom = BorderStyle.Thin;
            indentStyle.BorderRight = BorderStyle.Thin;
            indentStyle.WrapText = true;
            indentStyle.VerticalAlignment = VerticalAlignment.Top;
            indentStyle.Alignment = HorizontalAlignment.Left;
            indentStyle.SetFont(FontBold);
            lst_indentStyle.Add(indentStyle);

            //Create Style for col 3
            HSSFCellStyle itemCellStyleCol3Bold = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellStyleCol3Bold.BorderBottom = BorderStyle.Thin;
            itemCellStyleCol3Bold.BorderRight = BorderStyle.Thin;
            itemCellStyleCol3Bold.WrapText = true;
            itemCellStyleCol3Bold.VerticalAlignment = VerticalAlignment.Top;
            itemCellStyleCol3Bold.Alignment = HorizontalAlignment.Left;
            itemCellStyleCol3Bold.SetFont(FontBold);

            HSSFCellStyle itemCellStyleCol3 = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellStyleCol3.BorderBottom = BorderStyle.Thin;
            itemCellStyleCol3.BorderRight = BorderStyle.Thin;
            itemCellStyleCol3.WrapText = true;
            itemCellStyleCol3.VerticalAlignment = VerticalAlignment.Top;
            itemCellStyleCol3.Alignment = HorizontalAlignment.Left;
            itemCellStyleCol3.SetFont(FontReg);

            //Create Style for col 4
            HSSFCellStyle itemCellStyleCol4 = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellStyleCol4.BorderBottom = BorderStyle.Thin;
            itemCellStyleCol4.BorderRight = BorderStyle.Thin;
            itemCellStyleCol4.WrapText = true;
            itemCellStyleCol4.VerticalAlignment = VerticalAlignment.Top;
            itemCellStyleCol4.Alignment = HorizontalAlignment.Center;

            // Create Style for Default Value
            HSSFCellStyle itemCellStyleDefaultValue = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellStyleDefaultValue.BorderBottom = BorderStyle.Thin;
            itemCellStyleDefaultValue.BorderRight = BorderStyle.Thin;
            itemCellStyleDefaultValue.WrapText = true;
            itemCellStyleDefaultValue.VerticalAlignment = VerticalAlignment.Top;
            itemCellStyleDefaultValue.Alignment = HorizontalAlignment.Right;
            itemCellStyleDefaultValue.SetFont(FontReg);

            HSSFCellStyle itemCellStyleDefaultValueBold = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellStyleDefaultValueBold.BorderBottom = BorderStyle.Thin;
            itemCellStyleDefaultValueBold.BorderRight = BorderStyle.Thin;
            itemCellStyleDefaultValueBold.WrapText = true;
            itemCellStyleDefaultValueBold.VerticalAlignment = VerticalAlignment.Top;
            itemCellStyleDefaultValueBold.Alignment = HorizontalAlignment.Right;
            itemCellStyleDefaultValueBold.SetFont(FontBold);

            // Create Style for Summary
            HSSFCellStyle itemCellSrtyleSummary = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellSrtyleSummary.BorderBottom = BorderStyle.Thin;
            itemCellSrtyleSummary.BorderRight = BorderStyle.Thin;
            itemCellSrtyleSummary.WrapText = true;
            itemCellSrtyleSummary.VerticalAlignment = VerticalAlignment.Top;
            itemCellSrtyleSummary.Alignment = HorizontalAlignment.Left;
            itemCellSrtyleSummary.Indention = 2;
            itemCellSrtyleSummary.SetFont(FontBold);

            HSSFCellStyle itemCellSrtyleSummaryIndent = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellSrtyleSummaryIndent.BorderBottom = BorderStyle.Thin;
            itemCellSrtyleSummaryIndent.BorderRight = BorderStyle.Thin;
            itemCellSrtyleSummaryIndent.WrapText = true;
            itemCellSrtyleSummaryIndent.VerticalAlignment = VerticalAlignment.Top;
            itemCellSrtyleSummaryIndent.Alignment = HorizontalAlignment.Left;
            itemCellSrtyleSummaryIndent.Indention = 4;
            itemCellSrtyleSummaryIndent.SetFont(FontReg);

            HSSFCellStyle itemCellSrtyleSummaryIndentBold = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
            itemCellSrtyleSummaryIndentBold.BorderBottom = BorderStyle.Thin;
            itemCellSrtyleSummaryIndentBold.BorderRight = BorderStyle.Thin;
            itemCellSrtyleSummaryIndentBold.WrapText = true;
            itemCellSrtyleSummaryIndentBold.VerticalAlignment = VerticalAlignment.Top;
            itemCellSrtyleSummaryIndentBold.Alignment = HorizontalAlignment.Left;
            itemCellSrtyleSummaryIndentBold.Indention = 4;
            itemCellSrtyleSummaryIndentBold.SetFont(FontBold);


            //Create Row Data
            int rowIndex = 3;
            foreach (DataRow row in sourceTable.Rows)
            {               
                //Style Setting
                HSSFRow m_objdataRow = (HSSFRow)m_objsheet.CreateRow(rowIndex);
                int NumberColumn = 0;
                int indent = 0;
                bool isHiLevelBOI = false;
                bool firstCellisEmpty = false;
                bool isSummaryValueToBold = false;
                foreach (DataColumn column in sourceTable.Columns)
                {       
                    // Set Cell With Style
                    if (NumberColumn == 0)
                    {
                        indent = row[column].ToString().Split('.').ToList().Count();
                        isHiLevelBOI = row[column].ToString() != string.Empty && indent < 2 ? true : false;
                        firstCellisEmpty = row[column].ToString() == string.Empty;
                        HSSFCell itemCell = (HSSFCell)m_objdataRow.CreateCell(column.Ordinal);
                        itemCell.SetCellValue(row[column].ToString());
                        if (indent == 1 && isHiLevelBOI == true)
                            itemCell.CellStyle = itemCellStyleCol1FontBold;
                        else
                            itemCell.CellStyle = itemCellStyleCol1FontReg;
                    }
                    else if (NumberColumn == 1)
                    {
                        HSSFCell itemCell = (HSSFCell)m_objdataRow.CreateCell(column.Ordinal);
                        itemCell.SetCellValue(row[column].ToString());
                        string containingValue = row[column].ToString().ToLower();
                        if (firstCellisEmpty)
                        {
                            if (containingValue.Contains("sub total"))
                            {
                                itemCell.SetCellValue("∑  Sub Total");
                                itemCell.CellStyle = itemCellSrtyleSummary;
                                isSummaryValueToBold = true;
                            }
                            else if (containingValue.Contains("pembulatan"))
                            {
                                itemCell.CellStyle = itemCellSrtyleSummaryIndentBold;
                                isSummaryValueToBold = true;
                            }
                            else
                                itemCell.CellStyle = itemCellSrtyleSummaryIndent;
                        }
                        else
                        {
                            if (indent > 1)
                            {
                                if (lst_indentStyle.Count() < indent)
                                {
                                    HSSFCellStyle indentStyleAdditional = (HSSFCellStyle)m_objWorkbook.CreateCellStyle();
                                    indentStyleAdditional.BorderBottom = BorderStyle.Thin;
                                    indentStyleAdditional.BorderRight = BorderStyle.Thin;
                                    indentStyleAdditional.WrapText = true;
                                    indentStyleAdditional.VerticalAlignment = VerticalAlignment.Top;
                                    indentStyleAdditional.Alignment = HorizontalAlignment.Left;
                                    indentStyleAdditional.Indention = (short)(indent - 1);
                                    lst_indentStyle.Add(indentStyleAdditional);
                                }
                            }
                            itemCell.CellStyle = lst_indentStyle[indent - 1];
                        }
                    }
                    else if (NumberColumn == 2)
                    {
                        HSSFCell itemCell = (HSSFCell)m_objdataRow.CreateCell(column.Ordinal);
                        itemCell.SetCellValue(row[column].ToString());
                        itemCell.CellStyle = indent == 1 && isHiLevelBOI == true ? itemCellStyleCol3Bold : itemCellStyleCol3;
                    }
                    else if (NumberColumn == 3)
                    {
                        HSSFCell itemCell = (HSSFCell)m_objdataRow.CreateCell(column.Ordinal);
                        itemCell.SetCellValue(row[column].ToString());
                        itemCell.CellStyle = itemCellStyleCol4;
                    }
                    else
                    {
                        HSSFCell itemCell = (HSSFCell)m_objdataRow.CreateCell(column.Ordinal);
                        itemCell.SetCellValue(row[column].ToString());
                        var builtinFormatId = HSSFDataFormat.GetBuiltinFormat("#,##0.00");
                        itemCell.CellStyle = indent == 1 && (isHiLevelBOI == true || isSummaryValueToBold==true) ? itemCellStyleDefaultValueBold : itemCellStyleDefaultValue;
                    }
                    NumberColumn++;
                }                
                rowIndex++;
            }
            #endregion

            m_objsheet.AutoSizeColumn(1);
            m_objsheet.AutoSizeColumn(5);
            m_objsheet.AutoSizeColumn(6);
            m_objsheet.AutoSizeColumn(7);
            m_objsheet.AutoSizeColumn(8);
            m_objsheet.AutoSizeColumn(9);
            m_objsheet.SetColumnWidth(2, 10000);
            
            m_objWorkbook.Write(m_objmemoryStream);
            DirectoryInfo di = new DirectoryInfo(Request.MapPath("~/Content/"));
            foreach (FileInfo filez in di.GetFiles())
            {
                string ext = Path.GetExtension(filez.Extension);
                if (ext == ".xls")
                    filez.Delete();
            }
            //Save to server before download
            FileStream file = new FileStream(Server.MapPath("~/Content/"+ fileName), FileMode.Create, FileAccess.Write);
            m_objmemoryStream.WriteTo(file);
            file.Close();
            m_objmemoryStream.Close();
            m_objmemoryStream.Flush();
        }


        public ActionResult Browse(string ControlCatalogCartID,string ControlCatalogCartDesc, string FilterCatalogCartID = "", string FilterCatalogCartDesc = "")
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            ViewDataDictionary m_vddBudgetPlan = new ViewDataDictionary();
            m_vddBudgetPlan.Add("Control" + CatalogCartVM.Prop.CatalogCartID.Name, ControlCatalogCartID);
            m_vddBudgetPlan.Add("Control" + CatalogCartVM.Prop.CatalogCartDesc.Name, ControlCatalogCartDesc);
            m_vddBudgetPlan.Add(CatalogCartVM.Prop.CatalogCartID.Name, FilterCatalogCartID);
            m_vddBudgetPlan.Add(CatalogCartVM.Prop.CatalogCartDesc.Name, FilterCatalogCartDesc);
            
            return new XMVC.PartialViewResult
            {
                RenderMode = RenderMode.Auto,
                WrapByScriptTag = false,
                ViewData = m_vddBudgetPlan,
                ViewName = "../Cart/_Browse"
            };
        }
        public ActionResult ReadBrowse(StoreRequestParameters parameters)
        {
            TCatalogCartDA m_objTCatalogCartDA = new TCatalogCartDA();
            m_objTCatalogCartDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;
            List<object> m_lstFilter = new List<object>();
            FilterHeaderConditions m_fhcMCatalogCart = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcMCatalogCart.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = CatalogCartVM.Prop.Map(m_strDataIndex, false);
                    
                    if (m_strConditionOperator != Global.OpComparation)
                    {
                        m_lstFilter.Add(Global.GetOperator(m_strConditionOperator));
                        m_lstFilter.Add(m_objValue);
                        m_objFilter.Add(m_strDataIndex, m_lstFilter);
                    }
                    else
                    {
                        object m_objStart = null;
                        object m_objEnd = null;
                        foreach (KeyValuePair<string, object> m_kvpFilterDetail in (List<KeyValuePair<string, object>>)m_objValue)
                        {
                            switch (m_kvpFilterDetail.Key)
                            {
                                case Global.OpLessThan:
                                case Global.OpLessThanEqual:
                                    m_objEnd = m_kvpFilterDetail.Value;
                                    break;
                                case Global.OpGreaterThan:
                                case Global.OpGreaterThanEqual:
                                    m_objStart = m_kvpFilterDetail.Value;
                                    break;
                            }
                        }
                        if (m_objStart != null || m_objEnd != null)
                            m_lstFilter.Add((m_objStart != null ? (m_objEnd != null ? Operator.Between
                                : Operator.GreaterThanEqual) : (m_objEnd != null ? Operator.LessThanEqual
                                : Operator.None)));
                        m_lstFilter.Add(m_objStart != null ? m_objStart : "");
                        m_lstFilter.Add(m_objEnd != null ? m_objEnd : "");
                        m_objFilter.Add(m_strDataIndex, m_lstFilter);
                    }
                }
            }

            
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add((int)CartStatus.Approved);
            m_objFilter.Add(CatalogCartVM.Prop.StatusID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicMCatalogCartDA = m_objTCatalogCartDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpCatalogCartBL in m_dicMCatalogCartDA)
            {
                m_intCount = m_kvpCatalogCartBL.Key;
                break;
            }

            List<CatalogCartVM> m_lstCatalogCartVM = new List<CatalogCartVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(CatalogCartVM.Prop.CatalogCartID.MapAlias);
                m_lstSelect.Add(CatalogCartVM.Prop.CatalogCartDesc.MapAlias);
                m_lstSelect.Add(CatalogCartVM.Prop.StatusDesc.MapAlias);
                m_lstSelect.Add(CatalogCartVM.Prop.StatusID.MapAlias);
                m_lstSelect.Add(CatalogCartVM.Prop.CreatedDate.MapAlias);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(CatalogCartVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));


                m_dicMCatalogCartDA = m_objTCatalogCartDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objTCatalogCartDA.Message == string.Empty)
                {
                    m_lstCatalogCartVM = (
                        from DataRow m_drMCatalogCartDA in m_dicMCatalogCartDA[0].Tables[0].Rows
                        select new CatalogCartVM()
                        {
                            CatalogCartID = m_drMCatalogCartDA[CatalogCartVM.Prop.CatalogCartID.Name].ToString(),
                            CatalogCartDesc = m_drMCatalogCartDA[CatalogCartVM.Prop.CatalogCartDesc.Name].ToString(),
                            StatusDesc = m_drMCatalogCartDA[CatalogCartVM.Prop.StatusDesc.Name].ToString(),
                            StatusID = int.Parse(m_drMCatalogCartDA[CatalogCartVM.Prop.StatusID.Name].ToString()),
                            CreatedDate = DateTime.Parse(m_drMCatalogCartDA[CatalogCartVM.Prop.CreatedDate.Name].ToString())
                        }
                    ).ToList();
                }
            }
            return this.Store(m_lstCatalogCartVM, m_intCount);
        }
        public ActionResult Read(StoreRequestParameters parameters)
        {
            
            TCatalogCartDA m_objTCatalogCartDA = new TCatalogCartDA();
            m_objTCatalogCartDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;
            
            FilterHeaderConditions m_fhcTCatalogCart = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcTCatalogCart.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = CatalogCartVM.Prop.Map(m_strDataIndex, false);
                   
                    if (m_strConditionOperator != Global.OpComparation)
                    {
                        m_lstFilter.Add(Global.GetOperator(m_strConditionOperator));
                        m_lstFilter.Add(m_objValue);
                        m_objFilter.Add(m_strDataIndex, m_lstFilter);
                    }
                    else
                    {
                        object m_objStart = null;
                        object m_objEnd = null;
                        foreach (KeyValuePair<string, object> m_kvpFilterDetail in (List<KeyValuePair<string, object>>)m_objValue)
                        {
                            switch (m_kvpFilterDetail.Key)
                            {
                                case Global.OpLessThan:
                                case Global.OpLessThanEqual:
                                    m_objEnd = m_kvpFilterDetail.Value;
                                    break;
                                case Global.OpGreaterThan:
                                case Global.OpGreaterThanEqual:
                                    m_objStart = m_kvpFilterDetail.Value;
                                    break;
                            }
                        }
                        if (m_objStart != null || m_objEnd != null)
                            m_lstFilter.Add((m_objStart != null ? (m_objEnd != null ? Operator.Between
                                : Operator.GreaterThanEqual) : (m_objEnd != null ? Operator.LessThanEqual
                                : Operator.None)));
                        m_lstFilter.Add(m_objStart != null ? m_objStart : "");
                        m_lstFilter.Add(m_objEnd != null ? m_objEnd : "");
                        m_objFilter.Add(m_strDataIndex, m_lstFilter);
                    }
                }
            }
            Dictionary<int, DataSet> m_dicTCatalogCartDA = m_objTCatalogCartDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpItemBL in m_dicTCatalogCartDA)
            {
                m_intCount = m_kvpItemBL.Key;
                break;
            }

            List<CatalogCartVM> m_lsTCatalogCartVM = new List<CatalogCartVM>();
            if (m_intCount > 0)
            {
                m_boolIsCount = false;
                List<string> m_lstSelect = new List<string>();
                m_lstSelect.Add(CatalogCartVM.Prop.CatalogCartID.MapAlias);
                m_lstSelect.Add(CatalogCartVM.Prop.CatalogCartDesc.MapAlias);
                m_lstSelect.Add(CatalogCartVM.Prop.StatusDesc.MapAlias);
                m_lstSelect.Add(CatalogCartVM.Prop.StatusID.MapAlias);

                //GetUserFilters(ref m_objFilter);

                Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
                foreach (DataSorter m_dtsOrder in parameters.Sort)
                    m_dicOrder.Add(CatalogCartVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));


                m_dicTCatalogCartDA = m_objTCatalogCartDA.SelectBC(0, null, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
                if (m_objTCatalogCartDA.Message == string.Empty)
                {
                    m_lsTCatalogCartVM = (
                        from DataRow m_drTCatalogCartDA in m_dicTCatalogCartDA[0].Tables[0].Rows
                        select new CatalogCartVM()
                        {
                            CatalogCartID = m_drTCatalogCartDA[CatalogCartVM.Prop.CatalogCartID.Name].ToString(),
                            CatalogCartDesc = m_drTCatalogCartDA[CatalogCartVM.Prop.CatalogCartDesc.Name].ToString(),
                            StatusDesc = m_drTCatalogCartDA[CatalogCartVM.Prop.StatusDesc.Name].ToString(),
                            StatusID = int.Parse(m_drTCatalogCartDA[CatalogCartVM.Prop.StatusID.Name].ToString())
                        }
                    ).Distinct().ToList();
                }
            }
            return this.Store(m_lsTCatalogCartVM, m_intCount);
        }
        public ActionResult GetListBudgetPlanTemplateStructure(StoreRequestParameters parameters, string FilterBudgetPlanTemplateID)
        {
            BudgetPlanTemplateStructureVM m_objMBudgetPlanTemplateStructureVM = new BudgetPlanTemplateStructureVM();
            DBudgetPlanTemplateStructureDA m_objMBudgetPlanTemplateStructureDA = new DBudgetPlanTemplateStructureDA();
            m_objMBudgetPlanTemplateStructureDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = parameters.Start;
            int m_intLength = parameters.Limit;
            bool m_boolIsCount = true;

            FilterHeaderConditions m_fhcTBudgetPlan = new FilterHeaderConditions(this.Request.Params[General.EnumDesc(Params.FilterHeader)]);
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            foreach (FilterHeaderCondition m_fhcFilter in m_fhcTBudgetPlan.Conditions)
            {
                string m_strDataIndex = m_fhcFilter.DataIndex;
                string m_strConditionOperator = m_fhcFilter.Operator;
                object m_objValue = Global.GetFilterConditionValue(m_fhcFilter);

                if (m_strDataIndex != string.Empty)
                {
                    m_strDataIndex = BudgetPlanVM.Prop.Map(m_strDataIndex, false);
                    List<object> m_lstFilter = new List<object>();
                    if (m_strConditionOperator != Global.OpComparation)
                    {
                        m_lstFilter.Add(Global.GetOperator(m_strConditionOperator));
                        m_lstFilter.Add(m_objValue);
                        m_objFilter.Add(m_strDataIndex, m_lstFilter);
                    }
                    else
                    {
                        object m_objStart = null;
                        object m_objEnd = null;
                        foreach (KeyValuePair<string, object> m_kvpFilterDetail in (List<KeyValuePair<string, object>>)m_objValue)
                        {
                            switch (m_kvpFilterDetail.Key)
                            {
                                case Global.OpLessThan:
                                case Global.OpLessThanEqual:
                                    m_objEnd = m_kvpFilterDetail.Value;
                                    break;
                                case Global.OpGreaterThan:
                                case Global.OpGreaterThanEqual:
                                    m_objStart = m_kvpFilterDetail.Value;
                                    break;
                            }
                        }
                        if (m_objStart != null || m_objEnd != null)
                            m_lstFilter.Add((m_objStart != null ? (m_objEnd != null ? Operator.Between
                                : Operator.GreaterThanEqual) : (m_objEnd != null ? Operator.LessThanEqual
                                : Operator.None)));
                        m_lstFilter.Add(m_objStart != null ? m_objStart : "");
                        m_lstFilter.Add(m_objEnd != null ? m_objEnd : "");
                        m_objFilter.Add(m_strDataIndex, m_lstFilter);
                    }
                }
            }

            if (!string.IsNullOrEmpty(FilterBudgetPlanTemplateID))
            {
                List<object> m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(FilterBudgetPlanTemplateID);
                m_objFilter.Add(BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.Map, m_lstFilter);
            }

            //Dictionary<int, DataSet> m_dicMBudgetPlanTemplateStructureDA = m_objMBudgetPlanTemplateStructureDA.SelectBC(m_intSkip, m_intLength, m_boolIsCount, null, m_objFilter, null, null, null, null);
            //int m_intCount = 0;

            //foreach (KeyValuePair<int, DataSet> m_kvpBudgetPlanTemplateDA in m_dicMBudgetPlanTemplateStructureDA)
            //{
            //    m_intCount = m_kvpBudgetPlanTemplateDA.Key;
            //    break;
            //}

            List<BudgetPlanVersionStructureVM> lstBudgetPlanVersionStructureVM = new List<BudgetPlanVersionStructureVM>();
            //if (m_intCount > 0)
            //{
            m_boolIsCount = false;
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.Version.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.Sequence.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentSequence.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.HasChild.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemTypeID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemTypeID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.IsDefault.MapAlias);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            foreach (DataSorter m_dtsOrder in parameters.Sort)
                m_dicOrder.Add(BudgetPlanVM.Prop.Map(m_dtsOrder.Property.Substring(m_dtsOrder.Property.LastIndexOf(".") + 1)), General.EnumFromDesc<OrderDirection>(General.EnumName(m_dtsOrder.Direction)));

            Dictionary<int, DataSet> m_dicMBudgetPlanTemplateStructureDA = m_objMBudgetPlanTemplateStructureDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objMBudgetPlanTemplateStructureDA.Message == string.Empty)
            {
                lstBudgetPlanVersionStructureVM = (
                    from DataRow m_drMBudgetPlanTemplateStructureDA in m_dicMBudgetPlanTemplateStructureDA[0].Tables[0].Rows
                    select new BudgetPlanVersionStructureVM()
                    {
                        ItemDesc = m_drMBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemDesc.Name].ToString(),
                        //BudgetPlanTemplateID = m_drMBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.Name].ToString(),
                        ItemID = m_drMBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemID.Name].ToString(),
                        Version = Convert.ToInt16(m_drMBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.Version.Name].ToString()),
                        Sequence = Convert.ToInt16(m_drMBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.Sequence.Name].ToString()),
                        ParentItemID = m_drMBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentItemID.Name].ToString(),
                        ParentSequence = Convert.ToInt16(m_drMBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentSequence.Name].ToString()),
                        ParentVersion = Convert.ToInt16(m_drMBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentVersion.Name].ToString())
                        //HasChild = Convert.ToBoolean(m_drMBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.HasChild.Name].ToString()),
                        //ItemTypeID = m_drMBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString(),
                        //ParentItemTypeID = m_drMBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentItemTypeID.Name].ToString(),
                        //IsDefault = Convert.ToBoolean(m_drMBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.IsDefault.Name].ToString()),
                        //EnableDefault = false
                    }
                ).ToList();
            }
            //}


            return this.Store(lstBudgetPlanVersionStructureVM);
        }
        public ActionResult Home()
        {
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 0;
            return this.Direct();
        }
        public ActionResult Add(string Caller, string Selected)
        {
            //Global.HasAccess = GetHasAccess();
            //if (!Global.HasAccess.Add)
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            bool IsCallerFromGetData = false;
            Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();
            List<CartItemVM>  m_objSelectedRows = JSON.Deserialize<List<CartItemVM>>(Selected);

            string m_strMessage = string.Empty;
            CatalogCartVM m_objCatalogCartVM = new CatalogCartVM();
            m_objCatalogCartVM.CreatedDate = DateTime.Now;
            m_objCatalogCartVM.ModifiedDate = DateTime.Now;
            //m_objBudgetPlanVM.Unit = 1;
            m_objCatalogCartVM.ListCartItemVM = new List<CartItemVM>();
            if (m_objSelectedRows.Count > 0)
            {

                if (Session[detailCartSessionName] != null)
                {
                    List<CartItemVM> m_objSessionData = JSON.Deserialize<List<CartItemVM>>(Session[detailCartSessionName].ToString());
                    foreach (var item in m_objSelectedRows)
                    {
                        CartItemVM objSession = m_objSessionData.Where(d => d.ItemID == item.ItemID && d.VendorID == item.VendorID).FirstOrDefault();
                        if (objSession == null)
                        {
                            item.Qty = 1;
                            m_objSessionData.Add(item);
                        }
                        else
                        {
                            if (item.Qty <= 0) item.Qty = 1;
                            objSession.Qty = objSession.Qty + item.Qty;
                        }
                    }

                    Session[detailCartSessionName] = JSON.Serialize(m_objSessionData);
                }
                else
                {
                    List<CartItemVM> m_lsobjCartItemVM = new List<CartItemVM>();
                    foreach (var item in m_objSelectedRows)
                    {
                        item.Qty = 1;
                        if(!m_lsobjCartItemVM.Any(x => x.ItemID.Equals(item.ItemID) && x.VendorID.Equals(item.ItemID)))
                            m_lsobjCartItemVM.Add(item);
                    }
                    Session[detailCartSessionName] = JSON.Serialize(m_lsobjCartItemVM);
                }
                Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Added));

            } 
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
            }
           
            return this.Direct();
           
        }
        
        public ActionResult Checkout(string Caller, string Selected)
        {
            //Global.HasAccess = GetHasAccess();
            //if (!Global.HasAccess.Update)
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();
            List<CartItemVM> m_objCartItemVM = new List<CartItemVM>();
            if (Session[detailCartSessionName]!=null)
             m_objCartItemVM = JSON.Deserialize<List<CartItemVM>>(Session[detailCartSessionName].ToString());

            CatalogCartVM m_objCatalogCartVM = new CatalogCartVM();
            m_objCatalogCartVM.CreatedDate = DateTime.Now;
            m_objCatalogCartVM.ModifiedDate = DateTime.Now;
            //m_objBudgetPlanVM.Unit = 1;
            m_objCatalogCartVM.ListCartItemVM = m_objCartItemVM;
           

            ViewDataDictionary m_vddCartItem = new ViewDataDictionary();
            m_vddCartItem.Add(General.EnumDesc(Params.Caller), Caller??string.Empty);
            m_vddCartItem.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objCatalogCartVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddCartItem,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult Detail(string Caller, string Selected, string BudgetPlanID)
        {
            //Global.HasAccess = GetHasAccess();
            //if (!Global.HasAccess.Read)
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));


            string m_strMessage = string.Empty;
            CatalogCartVM m_objCatalogCartVM = new CatalogCartVM();
            Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();

            if (Caller == General.EnumDesc(Buttons.ButtonCancel))
            {
                if (Session[dataSessionName] != null)
                {
                    try
                    {
                        m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Session[dataSessionName].ToString());
                    }
                    catch (Exception ex)
                    {
                        m_strMessage = ex.Message;
                    }
                    Session[dataSessionName] = null;
                }
                else
                    m_strMessage = General.EnumDesc(MessageLib.Unknown);
            }
            else if (Caller == General.EnumDesc(Buttons.ButtonList))
                m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
            else if (Caller == "ComboBoxVersion")
            {
                if (!string.IsNullOrEmpty(Selected))
                {
                    m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);
                }
                else
                {
                    NameValueCollection m_nvcParams = this.Request.Params;
                    m_dicSelectedRow = GetFormData(m_nvcParams, BudgetPlanID);
                }
            }
            else if (Caller == General.EnumDesc(Buttons.ButtonSave))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                m_dicSelectedRow = GetFormData(m_nvcParams, BudgetPlanID);
            }

            if (m_dicSelectedRow.Count > 0)
                m_objCatalogCartVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            //m_objCatalogCartVM.ListCartItemVM = new List<CartItemVM>();
                        
            ViewDataDictionary m_vddBudgetPlan = new ViewDataDictionary();
            m_vddBudgetPlan.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonDetail));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objCatalogCartVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddBudgetPlan,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult Update(string Caller, string Selected)
        {
            //Global.HasAccess = GetHasAccess();
            //if (!Global.HasAccess.Update)
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            Dictionary<string, object> m_dicSelectedRow = new Dictionary<string, object>();
            if (Caller == General.EnumDesc(Buttons.ButtonList))
            {
                m_dicSelectedRow = JSON.Deserialize<Dictionary<string, object>>(Selected);

            }
            else if (Caller == General.EnumDesc(Buttons.ButtonDetail))
            {
                NameValueCollection m_nvcParams = this.Request.Params;
                m_dicSelectedRow = GetFormData(m_nvcParams, string.Empty);
                Session[dataSessionName] = JSON.Serialize(m_dicSelectedRow);
            }

            string m_strMessage = string.Empty;
            CatalogCartVM m_objCatalogCartVM = new CatalogCartVM();
            if (m_dicSelectedRow.Count > 0)
                m_objCatalogCartVM = GetSelectedData(m_dicSelectedRow, ref m_strMessage);
            

            if (m_strMessage != string.Empty)
            {
                Global.ShowErrorAlert(title, m_strMessage);
                return this.Direct();
            }

            //m_objCatalogCartVM.ListCartItemVM = new List<CartItemVM>();


            ViewDataDictionary m_vddCatalogCart = new ViewDataDictionary();
            m_vddCatalogCart.Add(General.EnumDesc(Params.Caller), Caller);
            m_vddCatalogCart.Add(General.EnumDesc(Params.Action), General.EnumDesc(Buttons.ButtonUpdate));
            this.GetCmp<Container>(General.EnumDesc(Params.PageContainer)).ActiveIndex = 1;
            return new XMVC.PartialViewResult
            {
                ClearContainer = true,
                ContainerId = General.EnumName(Params.PageOne),
                Model = m_objCatalogCartVM,
                RenderMode = RenderMode.AddTo,
                ViewData = m_vddCatalogCart,
                ViewName = "_Form",
                WrapByScriptTag = false
            };
        }
        public ActionResult Delete(string Selected)
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<CatalogCartVM> m_lstSelectedRow = new List<CatalogCartVM>();
            m_lstSelectedRow = JSON.Deserialize<List<CatalogCartVM>>(Selected);

            TCatalogCartDA m_objTCatalogCartDA = new TCatalogCartDA();
            DCatalogCartItemsDA d_objDBudgetPlanVersionDA = new DCatalogCartItemsDA();
            d_objDBudgetPlanVersionDA.ConnectionStringName = Global.ConnStrConfigName;
            m_objTCatalogCartDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();
            Dictionary<string, List<object>> m_dicSet = new Dictionary<string, List<object>>();
            List<object> m_lstSet = new List<object>();

            List<string> m_lstKey = new List<string>();
            try
            {
                foreach (CatalogCartVM m_objCatalogCartVM in m_lstSelectedRow)
                {
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifCatalogCartVM = m_objCatalogCartVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifCatalogCartVM in m_arrPifCatalogCartVM)
                    {
                        string m_strFieldName = m_pifCatalogCartVM.Name;
                        object m_objFieldValue = m_pifCatalogCartVM.GetValue(m_objCatalogCartVM);
                        if (m_objCatalogCartVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(CatalogCartVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_lstSet.Add(typeof(int));
                    m_lstSet.Add((int)CartStatus.Deleted); //Deleted
                    m_dicSet.Add(CatalogCartVM.Prop.StatusID.Map, m_lstSet);

                    m_objTCatalogCartDA.UpdateBC(m_dicSet, m_objFilter, false);

                    if (m_objTCatalogCartDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objTCatalogCartDA.Message);
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                return this.Direct(false, ex.Message);

            }
            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Deleted));
                return this.Direct(true, string.Empty);
            }
            else
                return this.Direct(false, String.Join(Global.NewLineSeparated, m_lstMessage));

        }
        public ActionResult Verify(string Selected)
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Verify)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<CatalogCartVM> m_lstSelectedRow = new List<CatalogCartVM>();
            m_lstSelectedRow = JSON.Deserialize<List<CatalogCartVM>>(Selected);

            TCatalogCartDA m_objTCatalogCartDA = new TCatalogCartDA();
            DCatalogCartItemsDA d_objDBudgetPlanVersionDA = new DCatalogCartItemsDA();
            d_objDBudgetPlanVersionDA.ConnectionStringName = Global.ConnStrConfigName;
            m_objTCatalogCartDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();
            string m_strMessage = string.Empty;
            Dictionary<string, List<object>> m_dicSet = new Dictionary<string, List<object>>();
            List<object> m_lstSet = new List<object>();
            List<string> m_lstKey = new List<string>();

            try
            {
                foreach (CatalogCartVM m_objCatalogCartVM in m_lstSelectedRow)
                {
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifCatalogCartVM = m_objCatalogCartVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifCatalogCartVM in m_arrPifCatalogCartVM)
                    {
                        string m_strFieldName = m_pifCatalogCartVM.Name;
                        object m_objFieldValue = m_pifCatalogCartVM.GetValue(m_objCatalogCartVM);
                        if (m_objCatalogCartVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_objFieldValue.ToString());
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Equals);
                            m_lstFilter.Add(m_objFieldValue);
                            m_objFilter.Add(CatalogCartVM.Prop.Map(m_strFieldName, false), m_lstFilter);
                        }
                        else break;
                    }

                    m_lstSet.Add(typeof(int));
                    m_lstSet.Add((int)CartStatus.Submitted);
                    m_dicSet.Add(CatalogCartVM.Prop.StatusID.Map, m_lstSet);

                    m_objTCatalogCartDA.UpdateBC(m_dicSet, m_objFilter, false);

                    if (m_objTCatalogCartDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objTCatalogCartDA.Message);
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                return this.Direct(false, ex.Message);

            }
            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Verified));
                return this.Direct(true, string.Empty);
            }
            else
                return this.Direct(false, String.Join(Global.NewLineSeparated, m_lstMessage));

        }
        public ActionResult Unverify(string Selected)
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Unverify)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<BudgetPlanVM> m_lstSelectedRow = new List<BudgetPlanVM>();
            m_lstSelectedRow = JSON.Deserialize<List<BudgetPlanVM>>(Selected);

            TBudgetPlanDA m_objTBudgetPlanDA = new TBudgetPlanDA();
            DBudgetPlanVersionDA d_objDBudgetPlanVersionDA = new DBudgetPlanVersionDA();
            d_objDBudgetPlanVersionDA.ConnectionStringName = Global.ConnStrConfigName;

            DBudgetPlanVersionEntryDA m_objDBudgetPlanVersionEntryDA = new DBudgetPlanVersionEntryDA();
            m_objDBudgetPlanVersionEntryDA.ConnectionStringName = Global.ConnStrConfigName;

            m_objTBudgetPlanDA.ConnectionStringName = Global.ConnStrConfigName;
            List<string> m_lstMessage = new List<string>();

            try
            {
                foreach (BudgetPlanVM m_objBudgetPlanVM in m_lstSelectedRow)
                {
                    Dictionary<string, string> m_lstKey = new Dictionary<string, string>();
                    Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                    PropertyInfo[] m_arrPifBudgetPlanVM = m_objBudgetPlanVM.GetType().GetProperties();

                    foreach (PropertyInfo m_pifBudgetPlanVM in m_arrPifBudgetPlanVM)
                    {
                        string m_strFieldName = m_pifBudgetPlanVM.Name;
                        object m_objFieldValue = m_pifBudgetPlanVM.GetValue(m_objBudgetPlanVM);
                        if (m_objBudgetPlanVM.IsKey(m_strFieldName))
                        {
                            m_lstKey.Add(m_strFieldName, m_objFieldValue.ToString());
                        }
                        else if (m_strFieldName.Equals(BudgetPlanVM.Prop.StatusID.Name))
                        {
                            m_lstKey.Add(m_strFieldName, m_objFieldValue.ToString());
                        }
                    }

                    if (m_lstKey.Count > 0)
                    {
                        DBudgetPlanVersion m_objDBudgetPlanVersion = new DBudgetPlanVersion();
                        m_objDBudgetPlanVersion.BudgetPlanID = m_lstKey[BudgetPlanVM.Prop.BudgetPlanID.Name].ToString();
                        m_objDBudgetPlanVersion.BudgetPlanVersion = Convert.ToInt32(m_lstKey[BudgetPlanVM.Prop.BudgetPlanVersion.Name]);
                        m_objDBudgetPlanVersion.StatusID = Convert.ToInt32(m_lstKey[BudgetPlanVM.Prop.StatusID.Name]);

                        d_objDBudgetPlanVersionDA.Data = m_objDBudgetPlanVersion;
                        d_objDBudgetPlanVersionDA.Select();

                        if (d_objDBudgetPlanVersionDA.Data.StatusID == 2 && d_objDBudgetPlanVersionDA.Data.BudgetPlanVersion > 1)//approved and version more than 1
                        {
                            DBudgetPlanVersionEntry m_objDBudgetPlanVersionEntry = new DBudgetPlanVersionEntry();

                            List<string> m_lstSelect = new List<string>();
                            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanID.MapAlias);
                            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanTypeID.MapAlias);
                            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanTypeDesc.MapAlias);
                            m_lstSelect.Add(BudgetPlanVM.Prop.BudgetPlanVersion.MapAlias);
                            m_lstSelect.Add(BudgetPlanVM.Prop.StatusID.MapAlias);
                            m_lstSelect.Add(BudgetPlanVM.Prop.StatusDesc.MapAlias);

                            Dictionary<string, List<object>> m_dicFilter = new Dictionary<string, List<object>>();
                            List<object> m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.Contains);
                            m_lstFilter.Add(m_objDBudgetPlanVersion.BudgetPlanID);
                            m_dicFilter.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanID.Map, m_lstFilter);

                            m_lstFilter = new List<object>();
                            m_lstFilter.Add(Operator.In);
                            m_lstFilter.Add(m_objDBudgetPlanVersion.BudgetPlanVersion);
                            m_dicFilter.Add(BudgetPlanVersionEntryVM.Prop.BudgetPlanVersion.Map, m_lstFilter);


                            Dictionary<int, DataSet> m_dicDBudgetPlanVersionEntryDA = m_objDBudgetPlanVersionEntryDA.SelectBC(0, 1, true, null, m_dicFilter, null, null, null);
                            int m_intCount = 0;

                            foreach (KeyValuePair<int, DataSet> m_kvpDBudgetPlanVersionEntryDA in m_dicDBudgetPlanVersionEntryDA)
                            {
                                m_intCount = m_kvpDBudgetPlanVersionEntryDA.Key;
                                break;
                            }

                            if (m_objDBudgetPlanVersionEntryDA.Success && m_objDBudgetPlanVersionEntryDA.Message == string.Empty && m_intCount > 0)
                                m_objDBudgetPlanVersion.StatusID = 99;//deleted
                            else
                                m_objDBudgetPlanVersion.StatusID = 0; //draft
                        }

                        d_objDBudgetPlanVersionDA.Update(false);
                    }

                    if (m_objTBudgetPlanDA.Message != string.Empty)
                        m_lstMessage.Add("[" + string.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objTBudgetPlanDA.Message);
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                return this.Direct(false, ex.Message);

            }
            if (m_lstMessage.Count <= 0)
            {
                Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Unverified));
                return this.Direct(true, string.Empty);
            }
            else
                return this.Direct(false, String.Join(Global.NewLineSeparated, m_lstMessage));

        }
        public ActionResult Save(string Action)
        {
            //if (!(Action == General.EnumDesc(Buttons.ButtonAdd) ? HasAccess(General.GetVariableName(() => new HasAccessVM().Add))
            //    : HasAccess(General.GetVariableName(() => new HasAccessVM().Update))))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            List<string> m_lstMessage = new List<string>();
            TCatalogCartDA m_objTCatalogCartDA = new TCatalogCartDA();
            DCatalogCartItemsDA m_objDCatalogCartItemsDA = new DCatalogCartItemsDA();

            CatalogCartVM m_objCatalogCartVM = new CatalogCartVM();
            CartItemVM m_objCartItemVM = new CartItemVM();
            TCatalogCart m_objTCatalogCart = new TCatalogCart();

            string m_strCatalogCartID = string.Empty;
            m_objTCatalogCartDA.ConnectionStringName = Global.ConnStrConfigName;

            string m_strTransName = "CatalogCart";
            object m_objDBConnection = null;
            m_objDBConnection = m_objTCatalogCartDA.BeginTrans(m_strTransName);
            try
            {
                m_strCatalogCartID = this.Request.Params[CatalogCartVM.Prop.CatalogCartID.Name];
                string m_strDescription = this.Request.Params[CatalogCartVM.Prop.CatalogCartDesc.Name];
                string m_strStatusID = this.Request.Params[CatalogCartVM.Prop.StatusID.Name];

                string m_strListCartItemVM = this.Request.Params[CatalogCartVM.Prop.ListCartItemVM.Name];

                List<CartItemVM> m_lstListCartItemVM = JSON.Deserialize<List<CartItemVM>>(m_strListCartItemVM);

                m_objCatalogCartVM.CatalogCartID = m_strCatalogCartID;
                m_objCatalogCartVM.CatalogCartDesc = m_strDescription;
                m_objCatalogCartVM.StatusID = string.IsNullOrEmpty(m_strStatusID)?0: int.Parse(m_strStatusID);
                
                m_lstMessage = IsSaveValid(Action, m_objCatalogCartVM);
                if (m_lstMessage.Count <= 0)
                {

                    #region TCatalogCart

                    m_objTCatalogCart.CatalogCartID = m_objCatalogCartVM.CatalogCartID;
                    m_objTCatalogCartDA.Data = m_objTCatalogCart;

                    if (Action != General.EnumDesc(Buttons.ButtonAdd))
                        m_objTCatalogCartDA.Select();

                    m_objTCatalogCart.CatalogCartDesc = m_objCatalogCartVM.CatalogCartDesc;

                    m_objDBConnection = m_objTCatalogCartDA.BeginTrans(m_strTransName);
                    if (Action == General.EnumDesc(Buttons.ButtonAdd) || string.IsNullOrEmpty(m_objTCatalogCartDA.Data.CatalogCartID))
                        m_objTCatalogCartDA.Insert(true, m_objDBConnection);
                    else
                        m_objTCatalogCartDA.Update(true, m_objDBConnection);

                    if (!m_objTCatalogCartDA.Success || m_objTCatalogCartDA.Message != string.Empty)
                    {
                        m_objTCatalogCartDA.EndTrans(ref m_objDBConnection, m_strTransName);
                        return this.Direct(false, m_objTCatalogCartDA.Message);
                    }
                    m_strCatalogCartID = m_objTCatalogCartDA.Data.CatalogCartID;
                    #endregion

                    #region DCatalogCartItems


                    m_objDCatalogCartItemsDA.ConnectionStringName = Global.ConnStrConfigName;

                    if (Action == General.EnumDesc(Buttons.ButtonUpdate))
                    {
                        Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
                        List<object> m_lstFilter = new List<object>();
                        m_lstFilter.Add(Operator.Equals);
                        m_lstFilter.Add(m_strCatalogCartID);
                        m_objFilter.Add(CartItemVM.Prop.CatalogCartID.Map, m_lstFilter);

                        m_objDCatalogCartItemsDA.DeleteBC(m_objFilter, true, m_objDBConnection);
                    }
                    foreach (CartItemVM objCartItemVM in m_lstListCartItemVM)
                    {
                        DCatalogCartItems m_objDCatalogCartItems = new DCatalogCartItems();

                        if (objCartItemVM.ItemID != null)
                        {
                            m_objDCatalogCartItems.CatalogCartItemID = Guid.NewGuid().ToString().Replace("-", ""); ;
                            m_objDCatalogCartItems.CatalogCartID = m_strCatalogCartID;
                            m_objDCatalogCartItems.Qty = objCartItemVM.Qty;
                            m_objDCatalogCartItems.ItemID = objCartItemVM.ItemID;
                            m_objDCatalogCartItems.Amount = objCartItemVM.Amount;
                            m_objDCatalogCartItems.ItemPriceID = objCartItemVM.ItemPriceID;
                            m_objDCatalogCartItems.ValidFrom = (DateTime) objCartItemVM.ValidFrom;
                            m_objDCatalogCartItems.VendorID = objCartItemVM.VendorID;

                            m_objDCatalogCartItemsDA.Data = m_objDCatalogCartItems;
                            m_objDCatalogCartItemsDA.Insert(true, m_objDBConnection);

                            if (!m_objDCatalogCartItemsDA.Success || m_objDCatalogCartItemsDA.Message != string.Empty)
                            {
                                m_objDCatalogCartItemsDA.EndTrans(ref m_objDBConnection, false, m_strTransName);
                                return this.Direct(false, m_objDCatalogCartItemsDA.Message);
                            }
                        }
                    }

                    #endregion



                    if (!m_objTCatalogCartDA.Success || m_objTCatalogCartDA.Message != string.Empty)
                        m_lstMessage.Add(m_objTCatalogCartDA.Message);
                    else
                        m_objTCatalogCartDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
                }
            }
            catch (Exception ex)
            {
                m_lstMessage.Add(ex.Message);
                m_objTCatalogCartDA.EndTrans(ref m_objDBConnection, true, m_strTransName);
            }
            if (m_lstMessage.Count <= 0)
            {
                Session[detailCartSessionName] = null;
                Global.ShowInfoAlert(title, General.EnumDesc(MessageLib.Saved));
                return Home();
            }
            Global.ShowErrorAlert(title, string.Join(Global.NewLineSeparated, m_lstMessage));
            return this.Direct(true);
        }
        public ActionResult GetListBudgetPlanVersion(StoreRequestParameters parameters, string BudgetPlanID)
        {
            List<BudgetPlanVersionVM> m_lstBudgetPlanVersionVM = new List<BudgetPlanVersionVM>();
            DBudgetPlanVersionDA m_objDBudgetPlanVersionDA = new DBudgetPlanVersionDA();
            m_objDBudgetPlanVersionDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVersionVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVM.Prop.BudgetPlanVersion.MapAlias);

            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanID);
            m_objFilter.Add(BudgetPlanVersionVM.Prop.BudgetPlanID.Map, m_lstFilter);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(BudgetPlanVersionVM.Prop.BudgetPlanVersion.Map, OrderDirection.Ascending);


            Dictionary<int, DataSet> m_dicDBudgetPlanVersionDA = m_objDBudgetPlanVersionDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, m_dicOrder);
            if (m_objDBudgetPlanVersionDA.Message == string.Empty)
            {
                m_lstBudgetPlanVersionVM = (
                from DataRow m_drDBudgetPlanVersionDA in m_dicDBudgetPlanVersionDA[0].Tables[0].Rows
                select new BudgetPlanVersionVM()
                {
                    BudgetPlanVersion = int.Parse(m_drDBudgetPlanVersionDA[BudgetPlanVersionVM.Prop.BudgetPlanVersion.Name].ToString()),
                    BudgetPlanID = m_drDBudgetPlanVersionDA[BudgetPlanVersionVM.Prop.BudgetPlanID.Name].ToString()
                }).Distinct().ToList();
            }

            return this.Store(m_lstBudgetPlanVersionVM); ;
        }
        /*public ActionResult LoadTemplateStructure(string BudgetPlanID, string BudgetPlanTemplateID, string jsonItemPrice)
        {
            ItemPriceVM dataItemPrice = JSON.Deserialize<ItemPriceVM>(jsonItemPrice);

            if (HasBudgetPlanAlreadyCreated(BudgetPlanID, BudgetPlanTemplateID, dataItemPrice.ProjectID, dataItemPrice.ClusterID, dataItemPrice.UnitTypeID))
            {
                Global.ShowErrorAlert(title, "Budget Plan with template [" + BudgetPlanTemplateID + "] was created");
                return this.Direct();
            }

            NodeCollection m_nodeCollectChild = new Ext.Net.NodeCollection();

            BudgetPlanTemplateStructureVM m_objDBudgetPlanTemplateStructureVM = new BudgetPlanTemplateStructureVM();
            DBudgetPlanTemplateStructureDA m_objDBudgetPlanTemplateStructureDA = new DBudgetPlanTemplateStructureDA();
            m_objDBudgetPlanTemplateStructureDA.ConnectionStringName = Global.ConnStrConfigName;


            List<BudgetPlanTemplateStructureVM> lstBudgetPlanTemplateStructureVM = new List<BudgetPlanTemplateStructureVM>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanTemplateID);
            m_objFilter.Add(BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(0);
            m_objFilter.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.None);
            m_lstFilter.Add(string.Empty);
            m_objFilter.Add(string.Format("({0} IS NULL OR {0} = 1)", BudgetPlanTemplateStructureVM.Prop.IsDefault.Map), m_lstFilter);


            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.Version.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.Sequence.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentSequence.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.HasChild.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemTypeID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.IsDefault.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemTypeID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.UoMDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.UoMID.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Key3.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc1.MapAlias);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(BudgetPlanTemplateStructureVM.Prop.Sequence.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicDBudgetPlanTemplateStructureDA = m_objDBudgetPlanTemplateStructureDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, m_dicOrder);
            if (m_objDBudgetPlanTemplateStructureDA.Message == string.Empty)
            {
                lstBudgetPlanTemplateStructureVM = (
                        from DataRow m_drDBudgetPlanTemplateStructureDA in m_dicDBudgetPlanTemplateStructureDA[0].Tables[0].Rows
                        select new BudgetPlanTemplateStructureVM()
                        {
                            BudgetPlanTemplateID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.Name].ToString(),
                            ItemID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemID.Name].ToString(),
                            ItemDesc = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemDesc.Name].ToString(),
                            Version = Convert.ToInt16(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.Version.Name].ToString()),
                            Sequence = Convert.ToInt16(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.Sequence.Name].ToString()),
                            ParentItemID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentItemID.Name].ToString(),
                            ParentSequence = Convert.ToInt16(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentSequence.Name].ToString()),
                            ParentVersion = Convert.ToInt16(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentVersion.Name].ToString()),
                            HasChild = Convert.ToBoolean(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.HasChild.Name].ToString()),
                            ItemTypeID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString(),
                            IsBOI = m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Key3.Name].ToString() == m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString()
                                    && m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Desc1.Name].ToString() == "TRUE",
                            IsAHS = m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Key3.Name].ToString() == m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString()
                                    && m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Desc1.Name].ToString() == "FALSE",
                            IsDefault = (string.IsNullOrEmpty(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.IsDefault.Name].ToString()) ? null :
                                        (bool?)(bool.Parse(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.IsDefault.Name].ToString()))),
                            ParentItemTypeID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentItemTypeID.Name].ToString(),
                            UoMDesc = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.UoMDesc.Name].ToString(),
                            UoMID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.UoMID.Name].ToString()

                        }).ToList();


            }

            foreach (BudgetPlanTemplateStructureVM item in lstBudgetPlanTemplateStructureVM)
            {
                string m_strSpecification = string.Empty;

                Node node = new Node();
                NodeCollection nodeChildCollection = LoadChildBPTemplateStructure(new BudgetPlanTemplateStructureVM()
                {
                    BudgetPlanTemplateID = item.BudgetPlanTemplateID,
                    ItemID = item.ItemID,
                    Version = item.Version,
                    Sequence = item.Sequence,
                    ParentItemID = item.ParentItemID,
                    ParentVersion = item.ParentVersion,
                    ParentSequence = item.ParentSequence
                }, dataItemPrice);

                //TODO: 
                if (nodeChildCollection.Any())
                {
                    item.MaterialAmount = 0;
                    item.WageAmount = 0;
                    item.MiscAmount = 0;
                    foreach (var node_ in nodeChildCollection)
                    {

                        if (item.IsBOI && (bool)node_.AttributesObject.GetType().GetProperties()[13].GetValue(node_.AttributesObject, null))
                        {
                            item.MaterialAmount = null;
                            item.WageAmount = null;
                            item.MiscAmount = null;
                        }
                        else
                        {
                            if (item.IsBOI) m_strSpecification += node_.AttributesObject.GetType().GetProperties()[0].GetValue(node_.AttributesObject, null).ToString() + ", ";
                            item.MaterialAmount += (decimal?)node_.AttributesObject.GetType().GetProperties()[15].GetValue(node_.AttributesObject, null) == null ? 0 : (decimal?)node_.AttributesObject.GetType().GetProperties()[15].GetValue(node_.AttributesObject, null);
                            item.WageAmount += (decimal?)node_.AttributesObject.GetType().GetProperties()[16].GetValue(node_.AttributesObject, null) == null ? 0 : (decimal?)node_.AttributesObject.GetType().GetProperties()[16].GetValue(node_.AttributesObject, null);
                            item.MiscAmount += (decimal?)node_.AttributesObject.GetType().GetProperties()[17].GetValue(node_.AttributesObject, null) == null ? 0 : (decimal?)node_.AttributesObject.GetType().GetProperties()[17].GetValue(node_.AttributesObject, null);
                        }
                    }
                }

                node = new Node()
                {
                    Expanded = nodeChildCollection.Count > 0,
                    Expandable = nodeChildCollection.Count > 0,
                    Leaf = nodeChildCollection.Count > 0 ? false : true,
                    AttributesObject = new
                    {
                        itemdesc = item.ItemDesc,
                        budgetplantemplateid = item.BudgetPlanTemplateID,
                        itemid = item.ItemID,
                        version = item.Version,
                        sequence = item.Sequence,
                        parentitemid = item.ParentItemID,
                        parentversion = item.ParentVersion,
                        parentsequence = item.ParentSequence,
                        haschild = nodeChildCollection.Any(),
                        itemtypeid = item.ItemTypeID,
                        parentitemtypeid = item.ItemTypeID,
                        isdefault = item.IsDefault,
                        uomdesc = item.UoMDesc,
                        isboi = item.IsBOI,
                        isahs = item.IsAHS,
                        materialamount = (item.MaterialAmount == 0 ? null : item.MaterialAmount),
                        wageamount = (item.WageAmount == 0 ? null : item.WageAmount),
                        miscamount = (item.MiscAmount == 0 ? null : item.MiscAmount),
                        leaf = nodeChildCollection.Count > 0 ? false : true,
                        specification = (m_strSpecification.Length > 0 ? m_strSpecification.Substring(0, m_strSpecification.Length - 2) : ""),
                        uomid = item.UoMID
                    },
                    Icon = Icon.Folder
                };

                node.Children.AddRange(nodeChildCollection);
                m_nodeCollectChild.Add(node);
            }


            return this.Store(m_nodeCollectChild);

        }*/
        
        public ActionResult GetStatusList(StoreRequestParameters parameters)
        {
            List<StatusVM> m_objStatusVM = new List<StatusVM>();

            DataAccess.MStatusDA m_objMStatusDA = new DataAccess.MStatusDA();
            m_objMStatusDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(StatusVM.Prop.StatusDesc.MapAlias);
            m_lstSelect.Add(StatusVM.Prop.StatusID.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("DBudgetPlanVersion");
            m_objFilter.Add(StatusVM.Prop.TableName.Name, m_lstFilter);

            Dictionary<int, System.Data.DataSet> m_dicMStatusDA = m_objMStatusDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objMStatusDA.Message == String.Empty)
            {
                for (int i = 0; i < m_dicMStatusDA[0].Tables[0].Rows.Count; i++)
                {
                    m_objStatusVM.Add(new StatusVM() { StatusDesc = m_dicMStatusDA[0].Tables[0].Rows[i][StatusVM.Prop.StatusDesc.Name].ToString(), StatusID = int.Parse(m_dicMStatusDA[0].Tables[0].Rows[i][StatusVM.Prop.StatusID.Name].ToString()) });
                }
            }

            return this.Store(m_objStatusVM);
        }

        public ActionResult LoadCartDetailItem(string CatalogCartID)
        {
            List<CartItemVM> m_lsCartItemVM = GetCartItemList(CatalogCartID);
            return this.Store(m_lsCartItemVM, m_lsCartItemVM.Count);
        }

        #region Budget Plan Structure Action

        public ActionResult AddBudgetPlanVersionStructure(string Caller, string Selected, string ControlBudgetPlanTemplateID, string ControlItemID, string ControlItemDesc,
             string ControlVersion, string ControlSequence, string ControlItemTypeID, string ControlParentItemID, string ControlParentItemDesc,
             string ControlParentVersion, string ControlParentSequence, string ControlParentItemTypeID, string ControlIsDefault, string ControlUoMDesc, string ChildItemTypeID)
        {
            BudgetPlanVersionStructureVM m_objBudgetPlanVersionStructureVM = new BudgetPlanVersionStructureVM();
            m_objBudgetPlanVersionStructureVM = JSON.Deserialize<BudgetPlanVersionStructureVM>(Selected);
            ChildItemTypeID = JSON.Deserialize<string>(ChildItemTypeID);

            if (!(GetFilterTemplateConfig("TRUE", m_objBudgetPlanVersionStructureVM.ItemTypeID).Any() && !(ChildItemTypeID != m_objBudgetPlanVersionStructureVM.ItemTypeID)))
            {
                Global.ShowErrorAlert("Budget Plan", "Cannot add structure inside selected item!");
            }
            else
            {

                BudgetPlanTemplateController m_ctrlBudgetPlanController = new BudgetPlanTemplateController();
                return m_ctrlBudgetPlanController.BrowseStructureTree(ControlItemID, ControlItemDesc, ControlVersion, ControlSequence,
                    ControlItemTypeID, ControlParentItemID, ControlParentVersion, ControlParentSequence, ControlParentItemTypeID, ControlIsDefault,
                    m_objBudgetPlanVersionStructureVM.BudgetPlanTemplateID, null,null, General.EnumDesc(Buttons.ButtonAdd));
            }

            return this.Direct();
        }

        public ActionResult UpdateBudgetPlanVersionStructure(string Caller, string Selected, string ControlBudgetPlanTemplateID, string ControlItemID, string ControlItemDesc,
             string ControlVersion, string ControlSequence, string ControlItemTypeID, string ControlParentItemID, string ControlParentItemDesc,
             string ControlParentVersion, string ControlParentSequence, string ControlParentItemTypeID, string ControlIsDefault, string ControlUoMDesc, string RegionID, string ProjectID, string ClusterID, string UnitTypeID)
        {
            if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Read)))
                return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));

            BudgetPlanVersionStructureVM m_objBudgetPlanVersionStructureVM = new BudgetPlanVersionStructureVM();
            m_objBudgetPlanVersionStructureVM = JSON.Deserialize<BudgetPlanVersionStructureVM>(Selected);

            if (m_objBudgetPlanVersionStructureVM != null)
            {
                if (GetFilterTemplateConfig("FALSE", m_objBudgetPlanVersionStructureVM.ItemTypeID).Any())
                {

                    BudgetPlanTemplateController m_ctrlBudgetPlanController = new BudgetPlanTemplateController();
                    return m_ctrlBudgetPlanController.BrowseStructure(ControlItemID, ControlItemDesc, ControlVersion, ControlSequence,
                        ControlItemTypeID, ControlParentItemID, ControlParentVersion, ControlParentSequence, ControlParentItemTypeID, ControlIsDefault,
                        m_objBudgetPlanVersionStructureVM.BudgetPlanTemplateID, m_objBudgetPlanVersionStructureVM.ParentItemID, m_objBudgetPlanVersionStructureVM.ParentVersion.ToString(), General.EnumDesc(Buttons.ButtonUpdate));
                }
                else
                {
                    ItemPriceVM objItemPrice = new ItemPriceVM() { RegionID = RegionID, ProjectID = ProjectID, ClusterID = ClusterID, UnitTypeID = UnitTypeID };
                    UnitPriceAnalysisController m_ctrlUnitPriceAnalysisController = new UnitPriceAnalysisController();
                    return m_ctrlUnitPriceAnalysisController.BrowseChildUnion(ControlItemID, ControlItemDesc, ControlUoMDesc, ControlItemTypeID, "", "", ControlVersion, ControlItemTypeID, "", "", m_objBudgetPlanVersionStructureVM.ItemVersionChildID,
                        m_objBudgetPlanVersionStructureVM.ParentItemID, m_objBudgetPlanVersionStructureVM.ParentItemTypeID, m_objBudgetPlanVersionStructureVM.ParentVersion.ToString(), m_objBudgetPlanVersionStructureVM.ParentSequence.ToString(), JSON.Serialize(objItemPrice));
                }
            }
            return this.Direct();

        }
        public ActionResult DeleteBudgetPlanVersionStructure(string Selected)
        {
            //if (!HasAccess(General.GetVariableName(() => new HasAccessVM().Delete)))
            //    return this.Direct(false, General.EnumDesc(MessageLib.NotAuthorized));


            return this.Direct();
        }
        public ActionResult UpdateNodeTemplateStructure(string Selected, string jsonItemPrice)
        {
            NodeCollection nodeChildCollection = new NodeCollection();
            Dictionary<string, bool> m_dicDisplayPrice = DisplayPrice();

            BudgetPlanVersionStructureVM m_objSelected = new BudgetPlanVersionStructureVM();
            m_objSelected = JSON.Deserialize<BudgetPlanVersionStructureVM>(Selected);

            ItemPriceVM dataItemPrice = JSON.Deserialize<ItemPriceVM>(jsonItemPrice);

            Node m_node = new Node();
            m_node.Expandable = true;
            m_node.Expanded = true;

            nodeChildCollection = LoadChildItemVersion(new ItemVersionChildVM()
            {
                ItemID = m_objSelected.ItemID,
                Version = m_objSelected.Version,
                Sequence = m_objSelected.Sequence,
                ChildItemID = m_objSelected.ChildItemID,
                ChildVersion = m_objSelected.ChildVersion
            }, dataItemPrice);

            if (nodeChildCollection.Any())
            {
                m_objSelected.MaterialAmount = 0;
                m_objSelected.WageAmount = 0;
                m_objSelected.MiscAmount = 0;
                foreach (var node_ in nodeChildCollection)
                {
                    m_objSelected.MaterialAmount += (decimal?)node_.AttributesObject.GetType().GetProperties()[15].GetValue(node_.AttributesObject, null) == null ? 0 : (decimal?)node_.AttributesObject.GetType().GetProperties()[15].GetValue(node_.AttributesObject, null);
                    m_objSelected.WageAmount += (decimal?)node_.AttributesObject.GetType().GetProperties()[16].GetValue(node_.AttributesObject, null) == null ? 0 : (decimal?)node_.AttributesObject.GetType().GetProperties()[16].GetValue(node_.AttributesObject, null);
                    m_objSelected.MiscAmount += (decimal?)node_.AttributesObject.GetType().GetProperties()[17].GetValue(node_.AttributesObject, null) == null ? 0 : (decimal?)node_.AttributesObject.GetType().GetProperties()[17].GetValue(node_.AttributesObject, null);
                }
            }

            m_node = new Node()
            {
                Expanded = nodeChildCollection.Count > 0,
                Expandable = nodeChildCollection.Count > 0,
                AttributesObject = new
                {
                    itemdesc = m_objSelected.ItemDesc,
                    budgetplantemplateid = m_objSelected.BudgetPlanTemplateID,
                    itemversionchildid = m_objSelected.ItemVersionChildID,
                    itemid = string.IsNullOrEmpty(m_objSelected.ChildItemID) ? m_objSelected.ItemID : m_objSelected.ChildItemID,
                    version = m_objSelected.Version,
                    sequence = m_objSelected.Sequence,
                    uomdesc = m_objSelected.UoMDesc,
                    parentitemid = m_objSelected.ParentItemID,
                    parentversion = m_objSelected.ParentVersion,
                    parentsequence = m_objSelected.ParentSequence,
                    haschild = nodeChildCollection.Any(),
                    itemtypeid = m_objSelected.ItemTypeID,
                    parentitemtypeid = m_objSelected.ParentItemTypeID,
                    isboi = m_objSelected.IsBOI,
                    isahs = m_objSelected.IsAHS,
                    materialamount = (m_objSelected.MaterialAmount ??0) * (m_objSelected.Coefficient??1),
                    wageamount = (m_objSelected.WageAmount ?? 0) * (m_objSelected.Coefficient??1),
                    miscamount = (m_objSelected.MiscAmount ?? 0) * (m_objSelected.Coefficient??1),
                    totalunitprice = 0,
                    total = 0,
                    leaf = nodeChildCollection.Count > 0 ? false : true,
                    specification = string.Empty,
                    uomid = m_objSelected.UoMID,
                    coefficient = m_objSelected.Coefficient,
                    displayprice = (m_dicDisplayPrice.ContainsKey(m_objSelected.ItemTypeID) ? m_dicDisplayPrice[m_objSelected.ItemTypeID] : true)
                },
                Icon = m_objSelected.IsAHS ? Icon.Table : Icon.PageWhite
            };

            m_node.Children.AddRange(nodeChildCollection);

            return this.Store(m_node);
        }
        public ActionResult AddNodeTemplateStructure(string Selected, string jsonItemPrice)
        {
            NodeCollection nodeChildCollection = new NodeCollection();
            Dictionary<string, bool> m_dicDisplayPrice = DisplayPrice();

            BudgetPlanVersionStructureVM m_objSelected = new BudgetPlanVersionStructureVM();
            m_objSelected = JSON.Deserialize<BudgetPlanVersionStructureVM>(Selected);

            ItemPriceVM dataItemPrice = JSON.Deserialize<ItemPriceVM>(jsonItemPrice);

            Node m_node = new Node();
            m_node.Expandable = true;
            m_node.Expanded = true;

            List<BudgetPlanTemplateStructureVM> m_BudgetPlanTemplateStructureVM = new List<BudgetPlanTemplateStructureVM>();
            nodeChildCollection = LoadChildBPTemplateStructure(new BudgetPlanTemplateStructureVM() { BudgetPlanTemplateID = m_objSelected.BudgetPlanTemplateID, ItemID = m_objSelected.ItemID, Version = m_objSelected.Version, Sequence = m_objSelected.Sequence, ParentItemID = m_objSelected.ParentItemID, ParentVersion = m_objSelected.ParentVersion, ParentSequence = m_objSelected.ParentSequence }, dataItemPrice,m_BudgetPlanTemplateStructureVM);

            if (nodeChildCollection.Any())
            {
                m_objSelected.MaterialAmount = 0;
                m_objSelected.WageAmount = 0;
                m_objSelected.MiscAmount = 0;
                foreach (var node_ in nodeChildCollection)
                {
                    if (m_objSelected.IsBOI && (bool)node_.AttributesObject.GetType().GetProperties()[13].GetValue(node_.AttributesObject, null))
                    {
                        m_objSelected.MaterialAmount = null;
                        m_objSelected.WageAmount = null;
                        m_objSelected.MiscAmount = null;
                    }
                    else
                    {
                        m_objSelected.MaterialAmount += (decimal?)node_.AttributesObject.GetType().GetProperties()[15].GetValue(node_.AttributesObject, null) == null ? 0 : (decimal?)node_.AttributesObject.GetType().GetProperties()[15].GetValue(node_.AttributesObject, null);
                        m_objSelected.WageAmount += (decimal?)node_.AttributesObject.GetType().GetProperties()[16].GetValue(node_.AttributesObject, null) == null ? 0 : (decimal?)node_.AttributesObject.GetType().GetProperties()[16].GetValue(node_.AttributesObject, null);
                        m_objSelected.MiscAmount += (decimal?)node_.AttributesObject.GetType().GetProperties()[17].GetValue(node_.AttributesObject, null) == null ? 0 : (decimal?)node_.AttributesObject.GetType().GetProperties()[17].GetValue(node_.AttributesObject, null);
                    }
                }
            }

            m_node = new Node()
            {
                Expanded = nodeChildCollection.Count > 0,
                Expandable = nodeChildCollection.Count > 0,
                AttributesObject = new
                {
                    itemdesc = m_objSelected.ItemDesc,
                    budgetplantemplateid = m_objSelected.BudgetPlanTemplateID,
                    itemversionchildid = m_objSelected.ItemVersionChildID,
                    itemid = m_objSelected.ItemID,
                    version = m_objSelected.Version,
                    sequence = m_objSelected.Sequence,
                    uomdesc = m_objSelected.UoMDesc,
                    parentitemid = m_objSelected.ParentItemID,
                    parentversion = m_objSelected.ParentVersion,
                    parentsequence = m_objSelected.ParentSequence,
                    haschild = nodeChildCollection.Any(),
                    itemtypeid = m_objSelected.ItemTypeID,
                    parentitemtypeid = m_objSelected.ItemTypeID,
                    isboi = m_objSelected.IsBOI,
                    isahs = m_objSelected.IsAHS,
                    materialamount = (m_objSelected.MaterialAmount == 0 ? null : m_objSelected.MaterialAmount),
                    wageamount = (m_objSelected.WageAmount == 0 ? null : m_objSelected.WageAmount),
                    miscamount = (m_objSelected.MiscAmount == 0 ? null : m_objSelected.MiscAmount),
                    leaf = nodeChildCollection.Count > 0 ? false : true,
                    totalunitprice = 0,
                    total = 0,
                    specification = string.Empty,
                    uomid = m_objSelected.UoMID,
                    displayprice = (m_dicDisplayPrice.ContainsKey(m_objSelected.ItemTypeID) ? m_dicDisplayPrice[m_objSelected.ItemTypeID] : true)
                },
                Icon = m_objSelected.IsBOI ? Icon.Folder : (m_objSelected.IsAHS ? Icon.Table : Icon.PageWhite)
            };

            m_node.Children.AddRange(nodeChildCollection);

            return this.Store(m_node);
        }

        #endregion

        #endregion

        #region Direct Method

        public ActionResult GetCatalogCart(string ControlCatalogCartID, string ControlCatalogCartDesc, string ControlStatusID, string ControlStatusDesc,
            string ControlCatalogCartTypeID, string ControlCatalogCartTypeDesc, string ControlCatalogCartVersion, string FilterCatalogCartID, string FilterCatalogCartDesc, bool Exact = false)
        {
            try
            {
                Dictionary<int, List<CatalogCartVM>> m_dicCatalogCartData = GetCatalogCartData(true, FilterCatalogCartID, FilterCatalogCartDesc);
                KeyValuePair<int, List<CatalogCartVM>> m_kvpCatalogCartVM = m_dicCatalogCartData.AsEnumerable().ToList()[0];
                if (m_kvpCatalogCartVM.Key < 1 || (m_kvpCatalogCartVM.Key > 1 && Exact))
                    return this.Direct(false);
                else if (m_kvpCatalogCartVM.Key > 1 && !Exact)
                    return Browse(ControlCatalogCartID, ControlCatalogCartDesc, FilterCatalogCartID, FilterCatalogCartDesc);

                m_dicCatalogCartData = GetCatalogCartData(false, FilterCatalogCartID, FilterCatalogCartDesc);
                CatalogCartVM m_objCatalogCartVM = m_dicCatalogCartData[0][0];
                this.GetCmp<TextField>(ControlCatalogCartID).Value = m_objCatalogCartVM.CatalogCartID;
                this.GetCmp<TextField>(ControlCatalogCartDesc).Value = m_objCatalogCartVM.CatalogCartDesc;
                this.GetCmp<TextField>(ControlStatusDesc).Value = m_objCatalogCartVM.StatusDesc;
                this.GetCmp<TextField>(ControlStatusID).Value = m_objCatalogCartVM.StatusID;
                return this.Direct(true);
            }
            catch (Exception ex)
            {
                return this.Direct(false, ex.Message);
            }
        }

        #endregion

        #region Private Method       
        private bool HasBudgetPlanAlreadyCreated(string budgetPlanID, string budgetPlanTemplateID, string projectID, string clusterID, string unitTypeID)
        {

            TBudgetPlanDA m_objTBudgetPlanDA = new TBudgetPlanDA();
            m_objTBudgetPlanDA.ConnectionStringName = Global.ConnStrConfigName;
            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_dicFilter = new Dictionary<string, List<object>>();

            //if (!string.IsNullOrEmpty(budgetPlanID))
            //{
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.NotEqual);
            m_lstFilter.Add(budgetPlanID);
            m_dicFilter.Add(BudgetPlanVM.Prop.BudgetPlanID.Map, m_lstFilter);
            //}

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(budgetPlanTemplateID);
            m_dicFilter.Add(BudgetPlanVM.Prop.BudgetPlanTemplateID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(projectID);
            m_dicFilter.Add(BudgetPlanVM.Prop.ProjectID.Map, m_lstFilter);

            //if (!string.IsNullOrEmpty(clusterID))
            //{
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(clusterID);
            m_dicFilter.Add(BudgetPlanVM.Prop.ClusterID.Map, m_lstFilter);
            //}

            //if (!string.IsNullOrEmpty(unitTypeID))
            //{
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(unitTypeID);
            m_dicFilter.Add(BudgetPlanVM.Prop.UnitTypeID.Map, m_lstFilter);
            //}

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(0);
            m_dicFilter.Add(BudgetPlanVM.Prop.StatusID.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicTBudgetPlanDA = m_objTBudgetPlanDA.SelectBC(0, 1, true, null, m_dicFilter, null, null, null);
            int m_intCount = 0;

            foreach (KeyValuePair<int, DataSet> m_kvpBudgetPlan in m_dicTBudgetPlanDA)
            {
                m_intCount = m_kvpBudgetPlan.Key;
                break;
            }

            return (m_intCount > 0 ? true : false);

        }
        private BudgetPlanVersionVM GetLastBudgetPlanVersion(string BudgetPlanID)
        {
            BudgetPlanVersionVM m_objBudgetPlanVersionVM = new BudgetPlanVersionVM();
            DBudgetPlanVersionDA m_objDBudgetPlanVersionDA = new DBudgetPlanVersionDA();
            m_objDBudgetPlanVersionDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVersionVM.Prop.BudgetPlanID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVM.Prop.BudgetPlanVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionVM.Prop.StatusID.MapAlias);

            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanID);
            m_objFilter.Add(BudgetPlanVersionVM.Prop.BudgetPlanID.Map, m_lstFilter);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(BudgetPlanVersionVM.Prop.BudgetPlanVersion.Map, OrderDirection.Descending);


            Dictionary<int, DataSet> m_dicDBudgetPlanVersionDA = m_objDBudgetPlanVersionDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, m_dicOrder);
            if (m_objDBudgetPlanVersionDA.Message == string.Empty)
            {
                DataRow m_drDBudgetPlanVersionDA = m_dicDBudgetPlanVersionDA[0].Tables[0].Rows[0];
                m_objBudgetPlanVersionVM = new BudgetPlanVersionVM()
                {
                    BudgetPlanVersion = int.Parse(m_drDBudgetPlanVersionDA[BudgetPlanVersionVM.Prop.BudgetPlanVersion.Name].ToString()),
                    BudgetPlanID = m_drDBudgetPlanVersionDA[BudgetPlanVersionVM.Prop.BudgetPlanID.Name].ToString(),
                    StatusID = int.Parse(m_drDBudgetPlanVersionDA[BudgetPlanVersionVM.Prop.StatusID.Name].ToString())
                };
            }

            return m_objBudgetPlanVersionVM; ;
        }
        private ItemVersionChildVM GetFormula(string ItemVersionChildID)
        {

            DItemVersionChildDA m_objItemVersionChildDA = new DItemVersionChildDA();
            m_objItemVersionChildDA.ConnectionStringName = Global.ConnStrConfigName;
            object m_objDBConnection = m_objItemVersionChildDA.BeginConnection();

            ItemVersionChildVM m_objItemVersionChildVM = new ItemVersionChildVM();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(ItemVersionChildID);
            m_objFilter.Add(ItemVersionChildVM.Prop.ItemVersionChildID.Map, m_lstFilter);


            //Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            //m_dicOrder.Add(ItemVersionChildVM.Prop.Sequence.Map, OrderDirection.Ascending);

            bool m_boolIsCount = false;
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemVersionChildID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildItemID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildVersion.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Sequence.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Formula.MapAlias);

            Dictionary<int, DataSet> m_dicMItemVersionDA = m_objItemVersionChildDA.SelectBC(0, 1, m_boolIsCount, m_lstSelect, m_objFilter, null, null, null, null);

            if (m_objItemVersionChildDA.Message == "")
            {

                DataRow m_drMItemVersionChildDA = m_dicMItemVersionDA[0].Tables[0].Rows[0];
                m_objItemVersionChildVM = new ItemVersionChildVM()
                {
                    ItemVersionChildID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemVersionChildID.Name].ToString(),
                    ChildItemID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemID.Name].ToString(),
                    ChildVersion = (int)m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildVersion.Name],
                    Sequence = (int)m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Sequence.Name],
                    Formula = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Formula.Name].ToString()
                };
            }

            return m_objItemVersionChildVM;
        }
        private List<string> IsSaveValid(string Action, CatalogCartVM objCatalogCartVM)
        {
            List<string> m_lstReturn = new List<string>();

            if (string.IsNullOrEmpty(objCatalogCartVM.CatalogCartDesc))
                m_lstReturn.Add(BudgetPlanVM.Prop.Description.Desc + " " + General.EnumDesc(MessageLib.mustFill));
       
            return m_lstReturn;
        }
        private Dictionary<string, object> GetFormData(NameValueCollection parameters, string BudgetPlanID = "")
        {
            Dictionary<string, object> m_dicReturn = new Dictionary<string, object>();
            m_dicReturn.Add(CatalogCartVM.Prop.CatalogCartID.Name, (parameters[CatalogCartVM.Prop.CatalogCartID.Name].ToString() == string.Empty ? BudgetPlanID : parameters[BudgetPlanVM.Prop.BudgetPlanID.Name]));
            m_dicReturn.Add(CatalogCartVM.Prop.CatalogCartDesc.Name, parameters[CatalogCartVM.Prop.CatalogCartDesc.Name]);
            //m_dicReturn.Add(BudgetPlanVM.Prop.CreatedDate.Name, parameters[BudgetPlanVM.Prop.CreatedDate.Name]);
            //m_dicReturn.Add(BudgetPlanVM.Prop.ModifiedDate.Name, parameters[BudgetPlanVM.Prop.ModifiedDate.Name]);
            m_dicReturn.Add(CatalogCartVM.Prop.StatusDesc.Name, parameters[CatalogCartVM.Prop.StatusDesc.Name]);
            m_dicReturn.Add(CatalogCartVM.Prop.StatusID.Name, parameters[CatalogCartVM.Prop.StatusID.Name]);

            return m_dicReturn;
        }
        
        private List<ConfigVM> GetFilterTemplateConfig(string filterDesc1, string filterKey3)
        {

            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilteru = new Dictionary<string, List<object>>();


            List<string> m_lstUPA = new List<string>();
            List<string> m_lstSelectb = new List<string>();
            m_lstSelectb.Add(ConfigVM.Prop.Key3.MapAlias);

            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name);
            m_objFilteru.Add(ConfigVM.Prop.Key1.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("BudgetPlanTemplate");
            m_objFilteru.Add(ConfigVM.Prop.Key2.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.In);
            m_lstFilter.Add(filterKey3);
            m_objFilteru.Add(ConfigVM.Prop.Key3.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(filterDesc1);
            m_objFilteru.Add(ConfigVM.Prop.Desc1.Map, m_lstFilter);

            Dictionary<int, DataSet> m_dicUConfigDA = m_objUConfigDA.SelectBC(0, null, false, m_lstSelectb, m_objFilteru, null, null, null);
            List<ConfigVM> m_lstConfigVM = new List<ConfigVM>();
            if (m_objUConfigDA.Message == string.Empty)
            {
                m_lstConfigVM = (
                        from DataRow m_drUConfigDA in m_dicUConfigDA[0].Tables[0].Rows
                        select new ConfigVM()
                        {
                            Key3 = m_drUConfigDA[ConfigVM.Prop.Key3.Name].ToString()
                        }
                    ).ToList();
            }
            return m_lstConfigVM;

        }
        private List<ConfigVM> GetItemTypeDisplayPriceConfig()
        {

            UConfigDA m_objUConfigDA = new UConfigDA();
            m_objUConfigDA.ConnectionStringName = Global.ConnStrConfigName;
            List<object> m_lstFilter = new List<object>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ConfigVM.Prop.Key2.MapAlias);

            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name);
            m_objFilter.Add(ConfigVM.Prop.Key1.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add("DisplayPrice");
            m_objFilter.Add(ConfigVM.Prop.Key3.Map, m_lstFilter);
            
            Dictionary<int, DataSet> m_dicUConfigDA = m_objUConfigDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, null);
            List<ConfigVM> m_lstConfigVM = new List<ConfigVM>();
            if (m_objUConfigDA.Message == string.Empty)
            {
                m_lstConfigVM = (
                        from DataRow m_drUConfigDA in m_dicUConfigDA[0].Tables[0].Rows
                        select new ConfigVM()
                        {
                            Key2 = m_drUConfigDA[ConfigVM.Prop.Key2.Name].ToString()
                        }
                    ).ToList();
            }
            return m_lstConfigVM;

        }
        private CatalogCartVM GetSelectedData(Dictionary<string, object> selected, ref string message)
        {
            CatalogCartVM m_objCatalogCartVM = new CatalogCartVM();
            TCatalogCartDA m_objTCatalogCartDA = new TCatalogCartDA();
            m_objTCatalogCartDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(CatalogCartVM.Prop.CatalogCartID.MapAlias);
            m_lstSelect.Add(CatalogCartVM.Prop.CatalogCartDesc.MapAlias);
            m_lstSelect.Add(CatalogCartVM.Prop.StatusDesc.MapAlias);
            m_lstSelect.Add(CatalogCartVM.Prop.StatusID.MapAlias);

            List<string> m_lstKey = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            foreach (KeyValuePair<string, object> m_kvpSelectedRow in selected)
            {
                if (m_objCatalogCartVM.IsKey(m_kvpSelectedRow.Key))
                {
                    List<object> m_lstFilter = new List<object>();
                    if (m_objCatalogCartVM.IsKey(m_kvpSelectedRow.Key))
                    {
                        m_lstKey.Add(m_kvpSelectedRow.Value.ToString());
                        m_lstFilter = new List<object>();
                        m_lstFilter.Add(Operator.Equals);
                        m_lstFilter.Add(m_kvpSelectedRow.Value);
                        m_objFilter.Add(CatalogCartVM.Prop.Map(m_kvpSelectedRow.Key, false), m_lstFilter);

                    }

                }
            }


            Dictionary<int, DataSet> m_dicTCatalogCartDA = m_objTCatalogCartDA.SelectBC(0, 1, false, m_lstSelect, m_objFilter, null, null, null);
            if (m_objTCatalogCartDA.Message == string.Empty)
            {
                DataRow m_drTCatalogCartDA = m_dicTCatalogCartDA[0].Tables[0].Rows[0];
                m_objCatalogCartVM.CatalogCartID = m_drTCatalogCartDA[CatalogCartVM.Prop.CatalogCartID.Name].ToString();
                m_objCatalogCartVM.CatalogCartDesc = m_drTCatalogCartDA[CatalogCartVM.Prop.CatalogCartDesc.Name].ToString();
                m_objCatalogCartVM.StatusDesc = m_drTCatalogCartDA[CatalogCartVM.Prop.StatusDesc.Name].ToString();
                m_objCatalogCartVM.StatusID = (int)m_drTCatalogCartDA[CatalogCartVM.Prop.StatusID.Name];
                m_objCatalogCartVM.ListCartItemVM = GetCartItemList(m_objCatalogCartVM.CatalogCartID);
            }
            else
                message = "[" + String.Join(Global.OneLineSeparated, m_lstKey) + "] " + m_objTCatalogCartDA.Message;

            return m_objCatalogCartVM;
        }
        private NodeCollection GrandTotalNode(string ClsIcon, decimal? RABTotal, decimal? ContractorFeesValue, decimal sizeValue)
        {
            NodeCollection _ndCollectionSubTotalDetail = new NodeCollection();

            Dictionary<string,string> m_dicNameOfChildBudgetPlanTotal = new Dictionary<string,string>();
            //m_strNameOfChildBudgetPlanTotalList.Add(BudgetPlanVM.Prop.Subtotal.Desc);
            m_dicNameOfChildBudgetPlanTotal.Add(BudgetPlanVM.Prop.ContractorFee.Name,BudgetPlanVM.Prop.ContractorFee.Desc);
            m_dicNameOfChildBudgetPlanTotal.Add(BudgetPlanVM.Prop.Total.Name,BudgetPlanVM.Prop.Total.Desc);
            m_dicNameOfChildBudgetPlanTotal.Add(BudgetPlanVM.Prop.Rounding.Name, BudgetPlanVM.Prop.Rounding.Desc);
            m_dicNameOfChildBudgetPlanTotal.Add(BudgetPlanVM.Prop.Tax.Name, BudgetPlanVM.Prop.Tax.Desc);
            //m_dicNameOfChildBudgetPlanTotal.Add(BudgetPlanVM.Prop.GrandTotal.Name, BudgetPlanVM.Prop.GrandTotal.Desc);
            m_dicNameOfChildBudgetPlanTotal.Add(BudgetPlanVM.Prop.AreaSize.Name, BudgetPlanVM.Prop.AreaSize.Desc);
            m_dicNameOfChildBudgetPlanTotal.Add(BudgetPlanVM.Prop.BasicPriceI.Name, BudgetPlanVM.Prop.BasicPriceI.Desc);
            m_dicNameOfChildBudgetPlanTotal.Add(BudgetPlanVM.Prop.BasicPriceII.Name, BudgetPlanVM.Prop.BasicPriceII.Desc);
            m_dicNameOfChildBudgetPlanTotal.Add(BudgetPlanVM.Prop.BasicPriceIII.Name, BudgetPlanVM.Prop.BasicPriceIII.Desc);

            List<decimal> m_decValueOfChildBudgetPlanTotalList = new List<decimal>();

            //Fee
            m_decValueOfChildBudgetPlanTotalList.Add((decimal)ContractorFeesValue);

            //Total
            m_decValueOfChildBudgetPlanTotalList.Add((decimal)(RABTotal + (RABTotal * ContractorFeesValue / 100)));

            //Rounding
            decimal m_decRoundDown = (decimal)(RABTotal + (RABTotal * ContractorFeesValue / 100));
            m_decRoundDown = m_decRoundDown / 1000;
            m_decRoundDown = Math.Floor(m_decRoundDown);
            m_decRoundDown = m_decRoundDown * 1000;
            m_decValueOfChildBudgetPlanTotalList.Add(m_decRoundDown);

            //Tax
            m_decValueOfChildBudgetPlanTotalList.Add((decimal)(m_decRoundDown * Convert.ToDecimal(0.1)));

            //GrandTotal
            decimal m_decGrandT = m_decRoundDown + ((decimal)(m_decRoundDown * Convert.ToDecimal(0.1)));
            m_decValueOfChildBudgetPlanTotalList.Add(m_decGrandT);

            //AreaSize
            m_decValueOfChildBudgetPlanTotalList.Add(sizeValue);

            //BasicPI
            m_decValueOfChildBudgetPlanTotalList.Add((decimal)RABTotal / sizeValue);

            //BasicPII
            //ValueOfChildBudgetPlanTotal.Add((decimal)(RoundDown + (RoundDown * ContractorFeesValue)) / sizeValue);
            m_decValueOfChildBudgetPlanTotalList.Add((decimal)m_decRoundDown / sizeValue);

            //BasicPIII
            //ValueOfChildBudgetPlanTotal.Add((RoundDown + ((decimal)(RoundDown * Convert.ToDecimal(0.1)))) / sizeValue);
            m_decValueOfChildBudgetPlanTotalList.Add(m_decGrandT / sizeValue);

            int n = 0;
            foreach (KeyValuePair<string,string> item in m_dicNameOfChildBudgetPlanTotal)
            {
                Node childBPlanTotal = new Node();
                childBPlanTotal.Expandable = false;
                childBPlanTotal.Expanded = false;
                childBPlanTotal.IconCls = ClsIcon;
                childBPlanTotal.NodeID = item.Key;
                childBPlanTotal.AttributesObject = new
                {
                    itemdesc = item.Value,//string.Format("{0} {1}", m_strNumbering, item.ItemDesc),
                    budgetplanid = string.Empty,
                    budgetplanversionstructureid = string.Empty,
                    budgetplantemplateid = string.Empty,
                    itemid = string.Empty,
                    version = (int?)null,
                    sequence = (int?)null,
                    parentitemid = string.Empty,
                    parentversion = (int?)null,
                    parentsequence = (int?)null,
                    specification = string.Empty,
                    volume = (decimal?)null, //11
                    materialamount = (decimal?)null,//12
                    wageamount = (decimal?)null,//13
                    miscamount = (decimal?)null,//14
                    itemversionchildid = string.Empty,
                    uomdesc = string.Empty,
                    itemtypeid = string.Empty,
                    isboi = false,
                    isahs = false,
                    haschild = false,
                    totalunitprice = (decimal?)null,
                    total = m_decValueOfChildBudgetPlanTotalList[n],
                    leaf = true,
                    uomid = string.Empty,
                    sequencedesc = "",
                    isGrandTotal = true
                };
                _ndCollectionSubTotalDetail.Add(childBPlanTotal);
                n++;
            }

            return _ndCollectionSubTotalDetail;
        }
        private Dictionary<string, bool> DisplayPrice()
        {
            Dictionary<string, bool> m_dicItemTypeForDisplayPrice = new Dictionary<string, bool>();
            string m_strMenuObject = GetMenuObject("DisplayPrice");
            List<ConfigVM> m_lstConfig = GetItemTypeDisplayPriceConfig();
            bool m_boolValueObject = Convert.ToBoolean(string.IsNullOrEmpty(m_strMenuObject) ?"True": m_strMenuObject);

            foreach (var item in m_lstConfig)
            {
                m_dicItemTypeForDisplayPrice.Add(item.Key2, m_boolValueObject);
            }
            return m_dicItemTypeForDisplayPrice;
        }

        private List<BudgetPlanTemplateStructureVM> GetBudgetPlanTemplateStructure(string budgetPlanTemplateID) {

            BudgetPlanTemplateStructureVM m_objDBudgetPlanTemplateStructureVM = new BudgetPlanTemplateStructureVM();
            DBudgetPlanTemplateStructureDA m_objDBudgetPlanTemplateStructureDA = new DBudgetPlanTemplateStructureDA();
            m_objDBudgetPlanTemplateStructureDA.ConnectionStringName = Global.ConnStrConfigName;


            List<BudgetPlanTemplateStructureVM> lstBudgetPlanTemplateStructureVM = new List<BudgetPlanTemplateStructureVM>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(budgetPlanTemplateID);
            m_objFilter.Add(BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.None);
            m_lstFilter.Add(string.Empty);
            m_objFilter.Add(string.Format("({0} IS NULL OR {0} = 1)", BudgetPlanTemplateStructureVM.Prop.IsDefault.Map), m_lstFilter);


            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.Version.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.Sequence.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentSequence.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemTypeID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.IsDefault.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemTypeID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.UoMDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.UoMID.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Key3.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc1.MapAlias);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(BudgetPlanTemplateStructureVM.Prop.Sequence.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicDBudgetPlanTemplateStructureDA = m_objDBudgetPlanTemplateStructureDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, m_dicOrder);
            if (m_objDBudgetPlanTemplateStructureDA.Message == string.Empty)
            {
                lstBudgetPlanTemplateStructureVM = (
                        from DataRow m_drDBudgetPlanTemplateStructureDA in m_dicDBudgetPlanTemplateStructureDA[0].Tables[0].Rows
                        select new BudgetPlanTemplateStructureVM()
                        {
                            BudgetPlanTemplateID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.Name].ToString(),
                            ItemID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemID.Name].ToString(),
                            ItemDesc = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemDesc.Name].ToString(),
                            Version = Convert.ToInt16(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.Version.Name].ToString()),
                            Sequence = Convert.ToInt16(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.Sequence.Name].ToString()),
                            ParentItemID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentItemID.Name].ToString(),
                            ParentSequence = Convert.ToInt16(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentSequence.Name].ToString()),
                            ParentVersion = Convert.ToInt16(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentVersion.Name].ToString()),
                            ItemTypeID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString(),
                            IsBOI = m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Key3.Name].ToString() == m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString()
                                    && m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Desc1.Name].ToString() == "TRUE",
                            IsAHS = m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Key3.Name].ToString() == m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString()
                                    && m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Desc1.Name].ToString() == "FALSE",
                            IsDefault = (string.IsNullOrEmpty(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.IsDefault.Name].ToString()) ? null :
                                        (bool?)(bool.Parse(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.IsDefault.Name].ToString()))),
                            ParentItemTypeID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentItemTypeID.Name].ToString(),
                            UoMDesc = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.UoMDesc.Name].ToString(),
                            UoMID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.UoMID.Name].ToString()

                        }).ToList();


            }

            return lstBudgetPlanTemplateStructureVM;
        }
        #endregion

        #region Public Method
        public string VerifyStructureBeforePackage(string BPID)
        {
            string message = "";
            List<BudgetPlanVersionStructureVM> m_lstBudgetPlanVersionStructureVM = new List<BudgetPlanVersionStructureVM>();
            DBudgetPlanVersionStructureDA m_objDBudgetPlanVersionStructureDA = new DBudgetPlanVersionStructureDA();
            m_objDBudgetPlanVersionStructureDA.ConnectionStringName = Global.ConnStrConfigName;

            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersionStructureID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanID.MapAlias);
            //m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanTemplateID.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.MapAlias);
            //m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ItemVersionChildID.MapAlias);
            //m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ItemID.MapAlias);
            //m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Version.MapAlias);
            //m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Sequence.MapAlias);
            //m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ParentItemID.MapAlias);
            //m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ParentItemDesc.MapAlias);
            //m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ParentVersion.MapAlias);
            m_lstSelect.Add("DB4.ChildConfig AS [ChildConfig]");
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.Volume.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.MaterialAmount.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.WageAmount.MapAlias);
            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.MiscAmount.MapAlias);

            m_lstSelect.Add(BudgetPlanVersionStructureVM.Prop.ItemTypeID.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Key3.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc1.MapAlias);

            m_lstSelect.Add(ItemVersionChildVM.Prop.Coefficient.MapAlias);

            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BPID);
            m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(1);
            m_objFilter.Add(BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.Map, m_lstFilter);

            Dictionary<string, OrderDirection> m_objOrderBy = new Dictionary<string, OrderDirection>();
            m_objOrderBy.Add(BudgetPlanVersionStructureVM.Prop.Sequence.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicDBudgetPlanVersionStructureDA = m_objDBudgetPlanVersionStructureDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, m_objOrderBy);
            if (m_objDBudgetPlanVersionStructureDA.Message == string.Empty)
            {
                foreach (DataRow m_drDBudgetPlanVersionStructureDA in m_dicDBudgetPlanVersionStructureDA[0].Tables[0].Rows)
                {
                    BudgetPlanVersionStructureVM addthis = new BudgetPlanVersionStructureVM();
                    addthis.BudgetPlanVersionStructureID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.BudgetPlanVersionStructureID.Name].ToString();
                    addthis.BudgetPlanID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.BudgetPlanID.Name].ToString();
                    addthis.BudgetPlanVersion = int.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.Name].ToString());
                    addthis.Volume = ((decimal?)m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Volume.Name] == 0 ? (decimal?)null :
                               decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Volume.Name].ToString()));
                    addthis.MaterialAmount = ((decimal?)m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name] == 0 ? (decimal?)null :
                               decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name].ToString()));
                    addthis.WageAmount = ((decimal?)m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.WageAmount.Name] == 0 ? (decimal?)null :
                               decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.WageAmount.Name].ToString()));
                    addthis.MiscAmount = ((decimal?)m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.MiscAmount.Name] == 0 ? (decimal?)null :
                               decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.MiscAmount.Name].ToString()));
                    addthis.ItemTypeID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ItemTypeID.Name].ToString();
                    
                    bool CheckThis = (string.IsNullOrEmpty(m_drDBudgetPlanVersionStructureDA["ChildConfig"].ToString()) ? false : (bool.Parse(m_drDBudgetPlanVersionStructureDA["ChildConfig"].ToString())));
                    if (CheckThis)
                    {
                        if (addthis.Volume == 0 || (addthis.MaterialAmount + addthis.WageAmount + addthis.MiscAmount == 0))
                        {
                            message += "invalid boi leaf";
                            break;
                        }
                    }
                    else if (addthis.ItemTypeID != "BOI" && addthis.ItemTypeID != "AHS")
                    {
                        if ((addthis.MaterialAmount==null?0: addthis.MaterialAmount) + (addthis.WageAmount == null ? 0 : addthis.WageAmount) + (addthis.MiscAmount == null ? 0 : addthis.MiscAmount) == 0)
                        {
                            message += "invalid boi ahs child";
                            break;
                        }
                    }
                    
                    
                    //m_lstBudgetPlanVersionStructureVM.Add(addthis);
                }
                //m_lstBudgetPlanVersionStructureVM.Distinct();

                    //m_lstBudgetPlanVersionStructureVM = (
                    //from DataRow m_drDBudgetPlanVersionStructureDA in m_dicDBudgetPlanVersionStructureDA[0].Tables[0].Rows
                    //select new BudgetPlanVersionStructureVM()
                    //{
                    //    BudgetPlanVersionStructureID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.BudgetPlanVersionStructureID.Name].ToString(),
                    //    BudgetPlanID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.BudgetPlanID.Name].ToString(),
                    //    BudgetPlanVersion = int.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.BudgetPlanVersion.Name].ToString()),
                    //    Volume = ((decimal?)m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Volume.Name] == 0 ? (decimal?)null :
                    //                decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.Volume.Name].ToString())),
                    //    MaterialAmount = ((decimal?)m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name] == 0 ? (decimal?)null :
                    //                decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name].ToString())),
                    //    WageAmount = ((decimal?)m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.WageAmount.Name] == 0 ? (decimal?)null :
                    //                decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.WageAmount.Name].ToString())),
                    //    MiscAmount = ((decimal?)m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.MiscAmount.Name] == 0 ? (decimal?)null :
                    //                decimal.Parse(m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.MiscAmount.Name].ToString())),

                //    ItemTypeID = m_drDBudgetPlanVersionStructureDA[BudgetPlanVersionStructureVM.Prop.ItemTypeID.Name].ToString(),
                //    }).Distinct().ToList();

            }
            else
                message = m_objDBudgetPlanVersionStructureDA.Message;

            return message;
        }
        public NodeCollection GetNodeTemplateStructure(string BudgetPlanTemplateID, ItemPriceVM dataItemPrice)
        {

            NodeCollection m_nodeCollectChild = new Ext.Net.NodeCollection();
            Dictionary<string, bool> m_dicDisplayPrice = DisplayPrice();

            BudgetPlanTemplateStructureVM m_objDBudgetPlanTemplateStructureVM = new BudgetPlanTemplateStructureVM();
            DBudgetPlanTemplateStructureDA m_objDBudgetPlanTemplateStructureDA = new DBudgetPlanTemplateStructureDA();
            m_objDBudgetPlanTemplateStructureDA.ConnectionStringName = Global.ConnStrConfigName;

            List<BudgetPlanTemplateStructureVM> m_lstBudgetPlanTemplateStructureVM = GetBudgetPlanTemplateStructure(BudgetPlanTemplateID);

            List<BudgetPlanTemplateStructureVM> lstBudgetPlanTemplateStructureVM = new List<BudgetPlanTemplateStructureVM>();
            lstBudgetPlanTemplateStructureVM = m_lstBudgetPlanTemplateStructureVM.Where(d => d.ParentItemID == "0").ToList();
            /*List <BudgetPlanTemplateStructureVM> lstBudgetPlanTemplateStructureVM = new List<BudgetPlanTemplateStructureVM>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(BudgetPlanTemplateID);
            m_objFilter.Add(BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(0);
            m_objFilter.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.None);
            m_lstFilter.Add(string.Empty);
            m_objFilter.Add(string.Format("({0} IS NULL OR {0} = 1)", BudgetPlanTemplateStructureVM.Prop.IsDefault.Map), m_lstFilter);


            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.Version.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.Sequence.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentSequence.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentVersion.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemTypeID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.IsDefault.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemTypeID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.UoMDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.UoMID.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Key3.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc1.MapAlias);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(BudgetPlanTemplateStructureVM.Prop.Sequence.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicDBudgetPlanTemplateStructureDA = m_objDBudgetPlanTemplateStructureDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, m_dicOrder);
            if (m_objDBudgetPlanTemplateStructureDA.Message == string.Empty)
            {
                lstBudgetPlanTemplateStructureVM = (
                        from DataRow m_drDBudgetPlanTemplateStructureDA in m_dicDBudgetPlanTemplateStructureDA[0].Tables[0].Rows
                        select new BudgetPlanTemplateStructureVM()
                        {
                            BudgetPlanTemplateID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.Name].ToString(),
                            ItemID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemID.Name].ToString(),
                            ItemDesc = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemDesc.Name].ToString(),
                            Version = Convert.ToInt16(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.Version.Name].ToString()),
                            Sequence = Convert.ToInt16(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.Sequence.Name].ToString()),
                            ParentItemID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentItemID.Name].ToString(),
                            ParentSequence = Convert.ToInt16(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentSequence.Name].ToString()),
                            ParentVersion = Convert.ToInt16(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentVersion.Name].ToString()),
                            ItemTypeID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString(),
                            IsBOI = m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Key3.Name].ToString() == m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString()
                                    && m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Desc1.Name].ToString() == "TRUE",
                            IsAHS = m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Key3.Name].ToString() == m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString()
                                    && m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Desc1.Name].ToString() == "FALSE",
                            IsDefault = (string.IsNullOrEmpty(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.IsDefault.Name].ToString()) ? null :
                                        (bool?)(bool.Parse(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.IsDefault.Name].ToString()))),
                            ParentItemTypeID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentItemTypeID.Name].ToString(),
                            UoMDesc = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.UoMDesc.Name].ToString(),
                            UoMID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.UoMID.Name].ToString()

                        }).ToList();


            }*/
            foreach (BudgetPlanTemplateStructureVM item in lstBudgetPlanTemplateStructureVM)
            {
                string m_strSpecification = string.Empty;

                Node node = new Node();
                NodeCollection nodeChildCollection = LoadChildBPTemplateStructure(new BudgetPlanTemplateStructureVM()
                {
                    BudgetPlanTemplateID = item.BudgetPlanTemplateID,
                    ItemID = item.ItemID,
                    Version = item.Version,
                    Sequence = item.Sequence,
                    ParentItemID = item.ParentItemID,
                    ParentVersion = item.ParentVersion,
                    ParentSequence = item.ParentSequence
                }, dataItemPrice, m_lstBudgetPlanTemplateStructureVM);

                if (nodeChildCollection.Any())
                {
                    item.MaterialAmount = 0;
                    item.WageAmount = 0;
                    item.MiscAmount = 0;
                    foreach (var node_ in nodeChildCollection)
                    {

                        if (item.IsBOI && (bool)node_.AttributesObject.GetType().GetProperties()[13].GetValue(node_.AttributesObject, null))
                        {
                            item.MaterialAmount = null;
                            item.WageAmount = null;
                            item.MiscAmount = null;
                        }
                        else
                        {
                            if (item.IsBOI) m_strSpecification += node_.AttributesObject.GetType().GetProperties()[0].GetValue(node_.AttributesObject, null).ToString() + ", ";
                            item.MaterialAmount += (decimal?)node_.AttributesObject.GetType().GetProperties()[15].GetValue(node_.AttributesObject, null) == null ? 0 : (decimal?)node_.AttributesObject.GetType().GetProperties()[15].GetValue(node_.AttributesObject, null);
                            item.WageAmount += (decimal?)node_.AttributesObject.GetType().GetProperties()[16].GetValue(node_.AttributesObject, null) == null ? 0 : (decimal?)node_.AttributesObject.GetType().GetProperties()[16].GetValue(node_.AttributesObject, null);
                            item.MiscAmount += (decimal?)node_.AttributesObject.GetType().GetProperties()[17].GetValue(node_.AttributesObject, null) == null ? 0 : (decimal?)node_.AttributesObject.GetType().GetProperties()[17].GetValue(node_.AttributesObject, null);
                        }
                    }
                }

                node = new Node()
                {
                    Expanded = nodeChildCollection.Count > 0,
                    Expandable = nodeChildCollection.Count > 0,
                    Leaf = nodeChildCollection.Count > 0 ? false : true,
                    AttributesObject = new
                    {
                        itemdesc = item.ItemDesc,
                        budgetplantemplateid = item.BudgetPlanTemplateID,
                        itemid = item.ItemID,
                        version = item.Version,
                        sequence = item.Sequence,
                        parentitemid = item.ParentItemID,
                        parentversion = item.ParentVersion,
                        parentsequence = item.ParentSequence,
                        haschild = nodeChildCollection.Any(),
                        itemtypeid = item.ItemTypeID,
                        parentitemtypeid = item.ItemTypeID,
                        isdefault = item.IsDefault,
                        uomdesc = item.UoMDesc,
                        isboi = item.IsBOI,
                        isahs = item.IsAHS,
                        materialamount = (item.MaterialAmount == 0 ? null : item.MaterialAmount),
                        wageamount = (item.WageAmount == 0 ? null : item.WageAmount),
                        miscamount = (item.MiscAmount == 0 ? null : item.MiscAmount),
                        leaf = nodeChildCollection.Count > 0 ? false : true,
                        specification = (m_strSpecification.Length > 0 ? m_strSpecification.Substring(0, m_strSpecification.Length - 2) : ""),
                        uomid = item.UoMID,
                        total = 0,
                        displayprice = (m_dicDisplayPrice.ContainsKey(item.ItemTypeID) ? m_dicDisplayPrice[item.ItemTypeID] : true)
                    },
                    Icon = Icon.Folder
                };

                node.Children.AddRange(nodeChildCollection);
                m_nodeCollectChild.Add(node);
            }

            #region Grand Total

            string ClsIcon = "display: none !important;";

            Node m_ndEmpty = new Node();
            m_ndEmpty.IconCls = ClsIcon;
            m_ndEmpty.Expanded = false;
            m_ndEmpty.Expandable = false;
            m_ndEmpty.AttributesObject = new
            {
                itemdesc = string.Empty,
                budgetplanid = string.Empty,
                budgetplanversionstructureid = string.Empty,
                budgetplantemplateid = string.Empty,
                itemid = string.Empty,
                version = (int?)null,
                sequence = (int?)null,
                parentitemid = string.Empty,
                parentversion = (int?)null,
                parentsequence = (int?)null,
                specification = string.Empty,
                volume = (decimal?)null, //11
                materialamount = (decimal?)null,//12
                wageamount = (decimal?)null,//13
                miscamount = (decimal?)null,//14
                itemversionchildid = string.Empty,
                uomdesc = string.Empty,
                itemtypeid = string.Empty,
                isboi = false,
                isahs = false,
                haschild = false,
                totalunitprice = (decimal?)null,
                total = (decimal?)null,
                leaf = true,
                uomid = string.Empty,
                sequencedesc = "",
                isGrandTotal = true
            };
            m_nodeCollectChild.Add(m_ndEmpty);


            Node m_ndBPlanTotal = new Node();
            m_ndBPlanTotal.Icon = Icon.Sum;
            m_ndBPlanTotal.Expanded = true;
            m_ndBPlanTotal.Expandable = true;
            m_ndBPlanTotal.NodeID = BudgetPlanVM.Prop.Subtotal.Name;
            m_ndBPlanTotal.AttributesObject = new
            {
                itemdesc = BudgetPlanVM.Prop.Subtotal.Desc,
                budgetplanid = string.Empty,
                budgetplanversionstructureid = string.Empty,
                budgetplantemplateid = string.Empty,
                itemid = string.Empty,
                version = (int?)null,
                sequence = (int?)null,
                parentitemid = string.Empty,
                parentversion = (int?)null,
                parentsequence = (int?)null,
                specification = string.Empty,
                volume = (decimal?)null, //11
                materialamount = (decimal?)null,//12
                wageamount = (decimal?)null,//13
                miscamount = (decimal?)null,//14
                itemversionchildid = string.Empty,
                uomdesc = string.Empty,
                itemtypeid = string.Empty,
                isboi = false,
                isahs = false,
                haschild = true,
                totalunitprice = (decimal?)null,
                total = 0,
                leaf = false,
                uomid = string.Empty,
                sequencedesc = "",
                isGrandTotal = true
            };

            m_ndBPlanTotal.Children.AddRange(GrandTotalNode(ClsIcon, 0, 0, 1));
            m_nodeCollectChild.Add(m_ndBPlanTotal);

            #endregion

            return m_nodeCollectChild;

        }
        public NodeCollection LoadChildBPTemplateStructure(BudgetPlanTemplateStructureVM dataParent, ItemPriceVM dataItemPrice, List<BudgetPlanTemplateStructureVM> budgetPlanTemplateStructures)
        {
            NodeCollection nodeCollection = new NodeCollection();
            Dictionary<string, bool> m_dicDisplayPrice = DisplayPrice();

            BudgetPlanTemplateStructureVM m_objDBudgetPlanTemplateStructureVM = new BudgetPlanTemplateStructureVM();
            DBudgetPlanTemplateStructureDA m_objDBudgetPlanTemplateStructureDA = new DBudgetPlanTemplateStructureDA();
            m_objDBudgetPlanTemplateStructureDA.ConnectionStringName = Global.ConnStrConfigName;

            List<BudgetPlanTemplateStructureVM> lstBudgetPlanTemplateStructureVM = new List<BudgetPlanTemplateStructureVM>();
            if (!budgetPlanTemplateStructures.Any()) budgetPlanTemplateStructures = GetBudgetPlanTemplateStructure(dataParent.BudgetPlanTemplateID);
             lstBudgetPlanTemplateStructureVM = budgetPlanTemplateStructures.Where(d => 
                                                            d.ParentItemID.Equals(dataParent.ItemID) &&
                                                            d.ParentVersion == dataParent.Version && 
                                                            d.ParentSequence == dataParent.Sequence).ToList();
            /*Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();

            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(dataParent.BudgetPlanTemplateID);
            m_objFilter.Add(BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(dataParent.ItemID);
            m_objFilter.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(dataParent.Version);
            m_objFilter.Add(BudgetPlanTemplateStructureVM.Prop.ParentVersion.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(dataParent.Sequence);
            m_objFilter.Add(BudgetPlanTemplateStructureVM.Prop.ParentSequence.Map, m_lstFilter);


            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.None);
            m_lstFilter.Add(string.Empty);
            m_objFilter.Add(string.Format("({0} IS NULL OR {0} = 1)", BudgetPlanTemplateStructureVM.Prop.IsDefault.Map), m_lstFilter);


            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.Version.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.Sequence.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentSequence.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentVersion.MapAlias);
            //m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.Coefficient.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ItemTypeID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.ParentItemTypeID.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.IsDefault.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.UoMDesc.MapAlias);
            m_lstSelect.Add(BudgetPlanTemplateStructureVM.Prop.UoMID.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Key3.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc1.MapAlias);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(BudgetPlanTemplateStructureVM.Prop.Sequence.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicDBudgetPlanTemplateStructureDA = m_objDBudgetPlanTemplateStructureDA.SelectBC(0, null, false, m_lstSelect, m_objFilter, null, null, m_dicOrder);
            if (m_objDBudgetPlanTemplateStructureDA.Message == string.Empty)
            {
                lstBudgetPlanTemplateStructureVM = (
                    from DataRow m_drDBudgetPlanTemplateStructureDA in m_dicDBudgetPlanTemplateStructureDA[0].Tables[0].Rows
                    select new BudgetPlanTemplateStructureVM()
                    {
                        BudgetPlanTemplateID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.BudgetPlanTemplateID.Name].ToString(),
                        ItemID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemID.Name].ToString(),
                        ItemDesc = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemDesc.Name].ToString(),
                        Version = Convert.ToInt16(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.Version.Name].ToString()),
                        Sequence = Convert.ToInt16(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.Sequence.Name].ToString()),
                        ParentItemID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentItemID.Name].ToString(),
                        ParentSequence = Convert.ToInt16(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentSequence.Name].ToString()),
                        ParentVersion = Convert.ToInt16(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentVersion.Name].ToString()),
                        //HasChild = Convert.ToBoolean(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.HasChild.Name].ToString()),
                        ItemTypeID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString(),
                        IsBOI = m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Key3.Name].ToString() == m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString()
                                    && m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Desc1.Name].ToString() == "TRUE",
                        IsAHS = m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Key3.Name].ToString() == m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString()
                                    && m_drDBudgetPlanTemplateStructureDA[ConfigVM.Prop.Desc1.Name].ToString() == "FALSE",
                        IsDefault = (string.IsNullOrEmpty(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.IsDefault.Name].ToString()) ? null :
                                    (bool?)(bool.Parse(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.IsDefault.Name].ToString()))),
                        ParentItemTypeID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.ParentItemTypeID.Name].ToString(),
                        UoMDesc = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.UoMDesc.Name].ToString(),
                        UoMID = m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.UoMID.Name].ToString(),
                        //Coefficient = (string.IsNullOrEmpty(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.Coefficient.Name].ToString()) ? null :
                        //            (decimal?)(decimal.Parse(m_drDBudgetPlanTemplateStructureDA[BudgetPlanTemplateStructureVM.Prop.Coefficient.Name].ToString()))),


                    }).ToList();
            }
            */

            foreach (BudgetPlanTemplateStructureVM item in lstBudgetPlanTemplateStructureVM)
            {
                string m_strSpecification = string.Empty;

                Node node = new Node();
                NodeCollection nodeCollection_ = LoadChildBPTemplateStructure(new BudgetPlanTemplateStructureVM()
                {
                    BudgetPlanTemplateID = dataParent.BudgetPlanTemplateID,
                    ItemID = item.ItemID,
                    Version = item.Version,
                    Sequence = item.Sequence,
                    ParentItemID = item.ParentItemID,
                    ParentVersion = item.ParentVersion,
                    ParentSequence = item.ParentSequence
                },
                    dataItemPrice, budgetPlanTemplateStructures);


                if (item.IsAHS)
                {
                    nodeCollection_ = LoadChildItemVersion(new ItemVersionChildVM()
                    {
                        ItemID = item.ItemID,
                        Version = item.Version,
                        Sequence = item.Sequence
                    },
                    dataItemPrice);

                }


                node = new Node();
                node.Expanded = nodeCollection_.Count > 0;
                node.Expandable = nodeCollection_.Count > 0;
                node.Leaf = nodeCollection_.Count > 0 ? false : true;

                if (nodeCollection_.Any())
                {
                    item.MaterialAmount = 0;
                    item.WageAmount = 0;
                    item.MiscAmount = 0;
                    foreach (var node_ in nodeCollection_)
                    {

                        if (item.IsBOI && (bool)node_.AttributesObject.GetType().GetProperties()[13].GetValue(node_.AttributesObject, null))
                        {
                            item.MaterialAmount = null;
                            item.WageAmount = null;
                            item.MiscAmount = null;

                        }
                        else
                        {
                            if (item.IsBOI) m_strSpecification += node_.AttributesObject.GetType().GetProperties()[0].GetValue(node_.AttributesObject, null).ToString() + ", ";
                            item.MaterialAmount += (decimal?)node_.AttributesObject.GetType().GetProperties()[15].GetValue(node_.AttributesObject, null) == null ? 0 : (decimal?)node_.AttributesObject.GetType().GetProperties()[15].GetValue(node_.AttributesObject, null);
                            item.WageAmount += (decimal?)node_.AttributesObject.GetType().GetProperties()[16].GetValue(node_.AttributesObject, null) == null ? 0 : (decimal?)node_.AttributesObject.GetType().GetProperties()[16].GetValue(node_.AttributesObject, null);
                            item.MiscAmount += (decimal?)node_.AttributesObject.GetType().GetProperties()[17].GetValue(node_.AttributesObject, null) == null ? 0 : (decimal?)node_.AttributesObject.GetType().GetProperties()[17].GetValue(node_.AttributesObject, null);
                        }
                    }
                }

                node.Icon = item.IsBOI ? Icon.Folder : Icon.Table;
                node.AttributesObject = new
                {
                    itemdesc = item.ItemDesc,
                    budgetplantemplateid = item.BudgetPlanTemplateID,
                    itemid = item.ItemID,
                    version = item.Version,
                    sequence = item.Sequence,
                    parentitemid = dataParent.ItemID,
                    parentversion = dataParent.Version,
                    parentsequence = dataParent.Sequence,
                    haschild = nodeCollection_.Any(),
                    itemtypeid = item.ItemTypeID,
                    parentitemtypeid = dataParent.ParentItemTypeID,
                    isdefault = item.IsDefault,
                    uomdesc = item.UoMDesc,
                    isboi = item.IsBOI,
                    isahs = item.IsAHS,
                    materialamount = (item.MaterialAmount == 0 ? null : item.MaterialAmount),
                    wageamount = (item.WageAmount == 0 ? null : item.WageAmount),
                    miscamount = (item.MiscAmount == 0 ? null : item.MiscAmount),
                    leaf = nodeCollection_.Count > 0 ? false : true,
                    specification = (m_strSpecification.Length > 0 ? m_strSpecification.Substring(0, m_strSpecification.Length - 2) : ""),
                    uomid = item.UoMID,
                    totalunitprice = 0,
                    total = 0,
                    displayprice = (m_dicDisplayPrice.ContainsKey(item.ItemTypeID) ? m_dicDisplayPrice[item.ItemTypeID] : true)
                };

                node.Children.AddRange(nodeCollection_);
                nodeCollection.Add(node);
            }


            return nodeCollection;

        }
        public NodeCollection LoadChildBPVersionStructure(BudgetPlanVersionStructureVM dataParent, List<BudgetPlanVersionStructureVM> listBudgetPlanStructure, bool IsRefreshPrice)
        {
            decimal? m_decTotal = 0;

            Dictionary<string, bool> m_dicDisplayPrice = DisplayPrice();

            ItemPriceVM dataItemPrice = new ItemPriceVM();
            NodeCollection m_nodeCollection = new NodeCollection();

            BudgetPlanVersionStructureVM m_objMBudgetPlanVersionStructureVM = new BudgetPlanVersionStructureVM();
            DBudgetPlanVersionStructureDA m_objMBudgetPlanVersionStructureDA = new DBudgetPlanVersionStructureDA();
            m_objMBudgetPlanVersionStructureDA.ConnectionStringName = Global.ConnStrConfigName;

            List<BudgetPlanVersionStructureVM> m_lstBudgetPlanStructure =
                listBudgetPlanStructure.Where(d => d.ParentItemID == dataParent.ItemID &&
                                                    d.ParentVersion == dataParent.Version &&
                                                    d.ParentSequence == dataParent.Sequence
                                    ).OrderBy(d => d.Sequence).ToList();


            foreach (BudgetPlanVersionStructureVM item in m_lstBudgetPlanStructure)
            {
                NodeCollection m_nodeCollectionChild = LoadChildBPVersionStructure(new BudgetPlanVersionStructureVM() { ItemID = item.ItemID, Version = item.Version, Sequence = item.Sequence }, listBudgetPlanStructure,IsRefreshPrice);

                m_decTotal = (item.Total == 0 ? (decimal?)null : item.Total);
                if (m_nodeCollectionChild.Any())
                {
                    foreach (var node_ in m_nodeCollectionChild)
                    {
                        if (item.IsBOI && (bool)node_.AttributesObject.GetType().GetProperties()[18].GetValue(node_.AttributesObject, null))
                        {
                            m_decTotal = (m_decTotal ?? 0m) + ((decimal?)node_.AttributesObject.GetType().GetProperties()[22].GetValue(node_.AttributesObject, null) == null ? 0m : (decimal?)node_.AttributesObject.GetType().GetProperties()[22].GetValue(node_.AttributesObject, null));
                        }

                        if (IsRefreshPrice)
                        {

                            if (item.IsBOI && (bool)node_.AttributesObject.GetType().GetProperties()[18].GetValue(node_.AttributesObject, null))
                            {
                                item.MaterialAmount = null;
                                item.WageAmount = null;
                                item.MiscAmount = null;
                            }
                            else if (item.IsAHS)
                            {
                                decimal? m_decNodeMaterial = (decimal?)node_.AttributesObject.GetType().GetProperties()[12].GetValue(node_.AttributesObject, null) == null ? 0m : (decimal?)node_.AttributesObject.GetType().GetProperties()[12].GetValue(node_.AttributesObject, null);
                                decimal? m_decNodeWage = (decimal?)node_.AttributesObject.GetType().GetProperties()[13].GetValue(node_.AttributesObject, null) == null ? 0m : (decimal?)node_.AttributesObject.GetType().GetProperties()[13].GetValue(node_.AttributesObject, null);
                                decimal? m_decNodeMisc = (decimal?)node_.AttributesObject.GetType().GetProperties()[14].GetValue(node_.AttributesObject, null) == null ? 0m : (decimal?)node_.AttributesObject.GetType().GetProperties()[14].GetValue(node_.AttributesObject, null);

                                item.MaterialAmount = (item.MaterialAmount ?? 0m) + m_decNodeMaterial;
                                item.WageAmount = (item.WageAmount ?? 0m) + m_decNodeWage;
                                item.MiscAmount = (item.MiscAmount ?? 0m) + m_decNodeMisc;

                            }
                            else// || (item.IsBOI && (bool)node_.AttributesObject.GetType().GetProperties()[19].GetValue(node_.AttributesObject, null))
                            {
                                decimal? m_decNodeMaterial = (decimal?)node_.AttributesObject.GetType().GetProperties()[12].GetValue(node_.AttributesObject, null) == null ? 0m : (decimal?)node_.AttributesObject.GetType().GetProperties()[12].GetValue(node_.AttributesObject, null);
                                decimal? m_decNodeWage = (decimal?)node_.AttributesObject.GetType().GetProperties()[13].GetValue(node_.AttributesObject, null) == null ? 0m : (decimal?)node_.AttributesObject.GetType().GetProperties()[13].GetValue(node_.AttributesObject, null);
                                decimal? m_decNodeMisc = (decimal?)node_.AttributesObject.GetType().GetProperties()[14].GetValue(node_.AttributesObject, null) == null ? 0m : (decimal?)node_.AttributesObject.GetType().GetProperties()[14].GetValue(node_.AttributesObject, null);

                                item.MaterialAmount = m_decNodeMaterial;
                                item.WageAmount = m_decNodeWage;
                                item.MiscAmount = m_decNodeMisc;
                            }

                        }

                    }
                }
                
                if (IsRefreshPrice) {
                    item.MaterialAmount = (item.MaterialAmount ?? 0m) * (item.Coefficient==0m?1m: item.Coefficient);
                    item.WageAmount = (item.WageAmount ?? 0m) * (item.Coefficient == 0m ? 1m : item.Coefficient);
                    item.MiscAmount = (item.MiscAmount ?? 0m) * (item.Coefficient == 0m ? 1m : item.Coefficient);
                }
                

                Node node = new Node()
                {
                    Expanded = m_nodeCollectionChild.Count > 0,
                    Expandable = m_nodeCollectionChild.Count > 0,
                    AttributesObject = new
                    {
                        itemdesc = item.ItemDesc,//string.Format("{0} {1}", m_strNumbering, item.ItemDesc),
                        budgetplanid = item.BudgetPlanID,
                        budgetplanversionstructureid = item.BudgetPlanVersionStructureID,
                        budgetplantemplateid = item.BudgetPlanTemplateID,
                        itemid = item.ItemID,
                        version = item.Version,
                        sequence = item.Sequence,
                        parentitemid = item.ParentItemID,
                        parentversion = item.ParentVersion,
                        parentsequence = item.ParentSequence,
                        specification = item.Specification,
                        volume = item.Volume, //11
                        materialamount = item.MaterialAmount ?? 0,//12
                        wageamount = item.WageAmount ?? 0,//13
                        miscamount = item.MiscAmount ?? 0,//14
                        itemversionchildid = item.ItemVersionChildID,
                        uomdesc = item.UoMDesc,
                        itemtypeid = item.ItemTypeID,
                        isboi = item.IsBOI,//18
                        isahs = item.IsAHS,
                        haschild = m_nodeCollectionChild.Any(),
                        totalunitprice = (item.Total > 0 ? item.TotalUnitPrice : ((item.Volume != null && item.Volume != 0) ? item.TotalUnitPrice : (decimal?)null)),
                        total = (item.Total > 0 ? item.Total : ((item.Volume != null && item.Volume != 0) ? item.Total : (m_decTotal != 0 ? m_decTotal :(decimal?)null))),
                        leaf = m_nodeCollectionChild.Count > 0 ? false : true,
                        uomid = item.UoMID,
                        sequencedesc = "",
                        displayprice = (m_dicDisplayPrice.ContainsKey(item.ItemTypeID)? m_dicDisplayPrice[item.ItemTypeID] : true),
                        coefficient = item.Coefficient
                    },
                    Icon = item.IsBOI ? Icon.Folder : (item.IsAHS ? Icon.Table : Icon.PageWhite)
                };
                node.Children.AddRange(m_nodeCollectionChild);
                m_nodeCollection.Add(node);
            }


            return m_nodeCollection;

        }
        public NodeCollection LoadChildItemVersion(ItemVersionChildVM dataParent, ItemPriceVM dataItemPrice)
        {

            NodeCollection m_nodeCollectChild = new Ext.Net.NodeCollection();
            Dictionary<string, bool> m_dicDisplayPrice = DisplayPrice();

            DItemVersionChildDA m_objMItemVersionChildDA = new DItemVersionChildDA();
            m_objMItemVersionChildDA.ConnectionStringName = Global.ConnStrConfigName;

            int m_intSkip = 0;
            bool m_boolIsCount = true;

            List<ItemVersionChildVM> m_lstItemVersionChildVM = new List<ItemVersionChildVM>();
            List<BudgetPlanTemplateStructureVM> m_lstBudgetPlanTemplateStructureVM = new List<BudgetPlanTemplateStructureVM>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();


            List<object> m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(dataParent.ItemID);
            m_objFilter.Add(ItemVersionChildVM.Prop.ItemID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Equals);
            m_lstFilter.Add(dataParent.Version);
            m_objFilter.Add(ItemVersionChildVM.Prop.Version.Map, m_lstFilter);

            if (!string.IsNullOrEmpty(dataParent.ChildItemID))
            {
                m_objFilter = new Dictionary<string, List<object>>();
                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(dataParent.ChildItemID);
                m_objFilter.Add(ItemVersionChildVM.Prop.ItemID.Map, m_lstFilter);

                m_lstFilter = new List<object>();
                m_lstFilter.Add(Operator.Equals);
                m_lstFilter.Add(dataParent.ChildVersion);
                m_objFilter.Add(ItemVersionChildVM.Prop.Version.Map, m_lstFilter);
            }

            m_boolIsCount = false;
            List<string> m_lstSelect = new List<string>();
            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemVersionChildID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemDesc.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ItemTypeID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildItemID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.ChildVersion.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.VersionDesc.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Coefficient.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.UoMDesc.MapAlias);
            m_lstSelect.Add(UoMVM.Prop.UoMID.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Formula.MapAlias);
            m_lstSelect.Add(ItemVersionChildVM.Prop.Sequence.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Key3.MapAlias);
            m_lstSelect.Add(ConfigVM.Prop.Desc1.MapAlias);

            Dictionary<string, OrderDirection> m_dicOrder = new Dictionary<string, OrderDirection>();
            m_dicOrder.Add(ItemVersionChildVM.Prop.Sequence.Map, OrderDirection.Ascending);

            Dictionary<int, DataSet> m_dicMItemVersionDA = m_objMItemVersionChildDA.SelectBC(m_intSkip, null, m_boolIsCount, m_lstSelect, m_objFilter, null, null, m_dicOrder, null);
            if (m_objMItemVersionChildDA.Message == string.Empty)
            {
                m_lstItemVersionChildVM = (
                    from DataRow m_drMItemVersionChildDA in m_dicMItemVersionDA[0].Tables[0].Rows
                    select new ItemVersionChildVM()
                    {
                        ItemVersionChildID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemVersionChildID.Name].ToString(),
                        ItemID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemID.Name].ToString(),
                        ItemDesc = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemDesc.Name].ToString(),
                        Version = Convert.ToInt16(m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildVersion.Name].ToString()),
                        ItemTypeID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemTypeID.Name].ToString(),
                        ChildItemID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemID.Name].ToString(),
                        ChildVersion = (int)m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildVersion.Name],
                        VersionDesc = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.VersionDesc.Name].ToString(),
                        Sequence = (int)m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Sequence.Name],
                        SequenceDesc = dataParent.SequenceDesc + m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Sequence.Name].ToString(),
                        Coefficient = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Coefficient.Name].ToString() == String.Empty ? 0 : (decimal)m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Coefficient.Name],
                        UoMDesc = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.UoMDesc.Name].ToString(),
                        UoMID = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.UoMID.Name].ToString(),
                        Formula = m_drMItemVersionChildDA[ItemVersionChildVM.Prop.Formula.Name].ToString(),
                        IsBOI = m_drMItemVersionChildDA[ConfigVM.Prop.Key3.Name].ToString() == m_drMItemVersionChildDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString()
                                    && m_drMItemVersionChildDA[ConfigVM.Prop.Desc1.Name].ToString() == "TRUE",
                        IsAHS = m_drMItemVersionChildDA[ConfigVM.Prop.Key3.Name].ToString() == m_drMItemVersionChildDA[BudgetPlanTemplateStructureVM.Prop.ItemTypeID.Name].ToString()
                                    && m_drMItemVersionChildDA[ConfigVM.Prop.Desc1.Name].ToString() == "FALSE",
                        MaterialAmount =  GetUnitPrice(m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemVersionChildID.Name].ToString(), m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemID.Name].ToString(), dataItemPrice, m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemTypeID.Name].ToString(), BudgetPlanVersionStructureVM.Prop.MaterialAmount.Name),
                        WageAmount = GetUnitPrice(m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemVersionChildID.Name].ToString(), m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemID.Name].ToString(), dataItemPrice, m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemTypeID.Name].ToString(), BudgetPlanVersionStructureVM.Prop.WageAmount.Name),
                        MiscAmount = GetUnitPrice(m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemVersionChildID.Name].ToString(), m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ChildItemID.Name].ToString(), dataItemPrice, m_drMItemVersionChildDA[ItemVersionChildVM.Prop.ItemTypeID.Name].ToString(), BudgetPlanVersionStructureVM.Prop.MiscAmount.Name),
                    }
                ).ToList();
            }

            int m_intSequence = 0;
            int m_parentSequence = (dataParent.Sequence);
            foreach (ItemVersionChildVM item in m_lstItemVersionChildVM)
            {

                m_intSequence = ++m_parentSequence;

                NodeCollection m_nodeChildCollection = LoadChildItemVersion(new ItemVersionChildVM()
                {
                    ItemVersionChildID = item.ItemVersionChildID,
                    ItemID = item.ItemID,
                    Version = item.Version,
                    Sequence = m_intSequence,
                    ChildItemID = item.ChildItemID,
                    ChildVersion = item.ChildVersion
                }, dataItemPrice);

                if (m_nodeChildCollection.Any())
                {
                    item.MaterialAmount = 0;
                    item.WageAmount = 0;
                    item.MiscAmount = 0;
                    foreach (var node_ in m_nodeChildCollection)
                    {
                        item.MaterialAmount += (decimal?)node_.AttributesObject.GetType().GetProperties()[15].GetValue(node_.AttributesObject, null) == null ? 0 : (decimal?)node_.AttributesObject.GetType().GetProperties()[15].GetValue(node_.AttributesObject, null);
                        item.WageAmount += (decimal?)node_.AttributesObject.GetType().GetProperties()[16].GetValue(node_.AttributesObject, null) == null ? 0 : (decimal?)node_.AttributesObject.GetType().GetProperties()[16].GetValue(node_.AttributesObject, null);
                        item.MiscAmount += (decimal?)node_.AttributesObject.GetType().GetProperties()[17].GetValue(node_.AttributesObject, null) == null ? 0 : (decimal?)node_.AttributesObject.GetType().GetProperties()[17].GetValue(node_.AttributesObject, null);
                    }
                }


                Node node = new Node();
                node = new Node()
                {
                    Expanded = m_nodeChildCollection.Count > 0,
                    Expandable = m_nodeChildCollection.Count > 0,
                    Leaf = m_nodeChildCollection.Count == 0,
                    AttributesObject = new
                    {
                        itemdesc = item.ItemDesc,
                        itemversionchildid = item.ItemVersionChildID,
                        itemid = item.ChildItemID,
                        version = item.ChildVersion,
                        sequence = m_intSequence,
                        uomdesc = item.UoMDesc,
                        parentitemid = item.ItemID,
                        parentversion = dataParent.Version,
                        parentsequence = dataParent.Sequence,
                        haschild = m_nodeChildCollection.Any(),
                        itemtypeid = item.ItemTypeID,
                        parentitemtypeid = dataParent.ItemTypeID,
                        isboi = item.IsBOI,
                        isahs = item.IsAHS,
                        isdefault = item.IsDefault,
                        materialamount = item.MaterialAmount * item.Coefficient,
                        wageamount = item.WageAmount * item.Coefficient,
                        miscamount = item.MiscAmount * item.Coefficient,

                        leaf = m_nodeChildCollection.Count > 0 ? false : true,
                        uomid = item.UoMID,
                        displayprice = (m_dicDisplayPrice.ContainsKey(item.ItemTypeID) ? m_dicDisplayPrice[item.ItemTypeID] : true)
                    },
                    Icon = m_nodeChildCollection.Count > 0 ? Icon.Table : Icon.PageWhite
                };
                node.Children.AddRange(m_nodeChildCollection);
                m_nodeCollectChild.Add(node);
            }

            return m_nodeCollectChild;

        }
        public Dictionary<int, List<CatalogCartVM>> GetCatalogCartData(bool isCount, string CatalogCartID, string Description)
        {
            int m_intCount = 0;
            List<CatalogCartVM> m_lstCatalogCartVM = new List<ViewModels.CatalogCartVM>();
            Dictionary<int, List<CatalogCartVM>> m_dicReturn = new Dictionary<int, List<CatalogCartVM>>();

            List<string> m_lstSelect = new List<string>();
            Dictionary<string, List<object>> m_objFilter = new Dictionary<string, List<object>>();
            List<object> m_lstFilter = new List<object>();

            TCatalogCartDA m_objTCatalogCartDA = new TCatalogCartDA();
            m_objTCatalogCartDA.ConnectionStringName = Global.ConnStrConfigName;

            m_lstSelect = new List<string>();
            m_lstSelect.Add(CatalogCartVM.Prop.CatalogCartID.MapAlias);
            m_lstSelect.Add(CatalogCartVM.Prop.CatalogCartDesc.MapAlias);
            m_lstSelect.Add(CatalogCartVM.Prop.StatusID.MapAlias);
            m_lstSelect.Add(CatalogCartVM.Prop.StatusDesc.MapAlias);

            m_objFilter = new Dictionary<string, List<object>>();
            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(CatalogCartID);
            m_objFilter.Add(CatalogCartVM.Prop.CatalogCartID.Map, m_lstFilter);

            m_lstFilter = new List<object>();
            m_lstFilter.Add(Operator.Contains);
            m_lstFilter.Add(Description);
            m_objFilter.Add(CatalogCartVM.Prop.CatalogCartDesc.Map, m_lstFilter);


            Dictionary<int, DataSet> m_dicTCatalogCartDA = m_objTCatalogCartDA.SelectBC(0, null, isCount, m_lstSelect, m_objFilter, null, null, null);
            if (m_objTCatalogCartDA.Message == string.Empty)
            {
                if (isCount)
                    foreach (KeyValuePair<int, DataSet> m_kvpItemBL in m_dicTCatalogCartDA)
                    {
                        m_intCount = m_kvpItemBL.Key;
                        break;
                    }
                else
                {
                    m_lstCatalogCartVM = (
                        from DataRow m_drTCatalogCartDA in m_dicTCatalogCartDA[0].Tables[0].Rows
                        select new CatalogCartVM()
                        {
                            CatalogCartID = m_drTCatalogCartDA[CatalogCartVM.Prop.CatalogCartID.Name].ToString(),
                            CatalogCartDesc = m_drTCatalogCartDA[CatalogCartVM.Prop.CatalogCartDesc.Name].ToString(),
                            StatusID = int.Parse(m_drTCatalogCartDA[CatalogCartVM.Prop.StatusID.Name].ToString()),
                            StatusDesc = m_drTCatalogCartDA[CatalogCartVM.Prop.StatusDesc.Name].ToString()
                        }
                    ).ToList();
                }
            }
            m_dicReturn.Add(m_intCount, m_lstCatalogCartVM);
            return m_dicReturn;
        }

        #endregion
    }
}