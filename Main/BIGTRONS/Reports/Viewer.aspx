<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Viewer.aspx.cs" Inherits="com.SML.BIGTRONS.Reports.Viewer" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="<%=Page.ResolveUrl("~/")%>Scripts/jquery-1.10.2.min.js" type="text/javascript"></script>
    <script src="<%=Page.ResolveUrl("~/")%>Scripts/global.js" type="text/javascript"></script>
    <script type="text/javascript">
        unloadReport = function () {
            var params = {
                reportUID: getParameterByName("ReportUID")
            };
            try {
                $.ajax({
                    type: "POST",
                    url: window.location.pathname + "/UnloadReport",
                    async: false,
                    cache: false,
                    data: JSON.stringify(params),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json"
                });
            }
            catch (ex) {
            }
        };

        <%--        RunFirst = function () {
            //PreparePrintButton();
            //InsertControlOnToolbar();
            //GetPrinterList();
        };
    
        PreparePrintButton = function () {
            var oBtnPrint = $('#<%=ReportViewer.ClientID%>_toptoolbar_print');

            oBtnPrint.prop("onclick", null).attr("onclick", null);
            oBtnPrint.click(function (event) {
                //PrintReport();
                var oFrame = $("iframe", $('#<%=ReportViewer.ClientID%>'));
                window.frames[oFrame.attr("id")].contentWindow.print();
                event.preventDefault();
                return false;
            });
        };--%>
        <%--
        InsertControlOnToolbar = function () {
            var oBtnPrint = $('#<%=ReportViewer.ClientID%>_toptoolbar_print');
            oBtnPrint.parent().parent().prepend('<select name="cbxPrinterList" id="cbxPrinterList" class="printToolbar" title="Select Printer"></select>');
        };

        function GetPrinterList() {
            $.ajax({
                type: "POST",
                url: window.location.pathname + "/GetPrinterList",
                async: false,
                cache: false,
                data: JSON.stringify({}),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: GetPrinterListSuccess,
                error: GetPrinterListError
            });
        };

        function GetPrinterListSuccess(data, status, xhr) {
            var oCbxPrinterList = $("select[id$='cbxPrinterList']");
            $(oCbxPrinterList).empty();
            $.each(data.d, function (val, text) {
                oCbxPrinterList[0].options[oCbxPrinterList[0].options.length] = new Option(text, val);
            });
        }

        function GetPrinterListError(xhr, status, error) {
        }

        PrintReport = function () {
            var oCbxPrinterList = $("select[id$='cbxPrinterList']");
            var params = {
                printerName: encodeURIComponent(oCbxPrinterList.val() == null ? "" : oCbxPrinterList.val())
            };
            try {
                $.ajax({
                    type: "POST",
                    url: window.location.pathname + "/PrintReport",
                    async: false,
                    cache: false,
                    data: JSON.stringify(params),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: PrintReportSuccess,
                    error: PrintReportError
                });
            }
            catch (ex) {
            }
        };

        PrintReportSuccess = function (data, status, xhr) {
        };

        PrintReportError = function (xhr, status, error) {
        };
            --%>
        //$(function () {
        //    $(document).ready(function () {
        //        setTimeout("RunFirst()", 1);
        //    });
        //});
    </script>
</head>
<body onunload="unloadReport()">
    <form id="frmViewer" runat="server">
        <div>
            <CR:CrystalReportViewer ID="ReportViewer" runat="server" HasCrystalLogo="False" HasToggleGroupTreeButton="false" PrintMode="ActiveX"
                AutoDataBind="True" EnableParameterPrompt="false" EnableDatabaseLogonPrompt="false" ToolPanelView="None" />
        </div>
    </form>
</body>
</html>
