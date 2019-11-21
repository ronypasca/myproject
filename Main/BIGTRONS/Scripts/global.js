var defaultDateFormat = "dd-MM-yyyy";
var defaultDateTimeFormat = "dd-MM-yyyy HH:mm:ss";
var defaultNumberFormat = "0,000.00";
var billNumberFormat = "0,000";
var integerNumberFormat = "0,000";
var sQueueName = "IPMessage";

var formatNumber = function (field, allowNegative) {
    Ext.util.Format.thousandSeparator = ",";
    Ext.util.Format.decimalSeparator = ".";
    field.setValue(Ext.util.Format.number(field.getValue().replace((allowNegative ? /[^-0-9\\.]+/g : /[^0-9\\.]+/g), ''), defaultNumberFormat));
}

var formatBill = function (field, allowNegative) {
    Ext.util.Format.thousandSeparator = ",";
    Ext.util.Format.decimalSeparator = ".";
    field.setValue(Ext.util.Format.number(field.getValue().replace((allowNegative ? /[^-0-9\\.]+/g : /[^0-9\\.]+/g), ''), billNumberFormat));
}

var formatValueBill = function (value, allowNegative) {
    Ext.util.Format.thousandSeparator = ",";
    Ext.util.Format.decimalSeparator = ".";
    var val = Ext.util.Format.number(value.replace((allowNegative ? /[^-0-9\\.]+/g : /[^0-9\\.]+/g), ''), billNumberFormat);
    return val;
}

var formatInt = function (field, allowNegative) {
    Ext.util.Format.thousandSeparator = ",";
    Ext.util.Format.decimalSeparator = ".";
    field.setValue(Ext.util.Format.number(field.getValue().replace((allowNegative ? /[^-0-9\\.]+/g : /[^0-9\\.]+/g), ''), integerNumberFormat));
}

var formatPlainNumber = function (field, allowNegative) {
    Ext.util.Format.thousandSeparator = ",";
    Ext.util.Format.decimalSeparator = ".";
    field.setValue(Ext.util.Format.number(field.getValue().replace((allowNegative ? /[^-0-9\\.]+/g : /[^0-9\\.]+/g), ''), ''));
}

var getValueText = function () {
    var value = this.getComponent(0).getValue();
    return (Ext.isEmpty(value) ? "" : "*") + value;
}

var getValueDate = function () {
    var value = this.getComponent(0).getValue();
    try {
        return (!Ext.isEmpty(value) ? "=" + formatDate(dateFromJSON(Ext.Date.clearTime(value, true).getTime()), defaultDateFormat) : "");
    }
    catch (ex) { }
}

var getValueDateRange = function () {
    var valueStart = this.getComponent(0).getValue();
    var valueEnd = this.getComponent(1).getValue();
    var ret=((!Ext.isEmpty(valueStart) ? ">=" + formatDate(dateFromJSON(Ext.Date.clearTime(valueStart, true).getTime()), defaultDateFormat) : "")
        + (!Ext.isEmpty(valueEnd) ? "<=" + formatDate(dateFromJSON(Ext.Date.clearTime(valueEnd, true).getTime()), defaultDateFormat) : ""));
    return ret;
}

var getValueDateTimeRange = function () {
    var valueStart = this.getComponent(0).getValue();
    var valueEnd = this.getComponent(1).getValue();
    var ret = ((!Ext.isEmpty(valueStart) ? ">=" + formatDate(dateFromJSON(Ext.Date.clearTime(valueStart, true).getTime()), defaultDateTimeFormat) : "")
        + (!Ext.isEmpty(valueEnd) ? "<=" + formatDate(dateFromJSON(Ext.Date.clearTime(valueEnd, true).getTime()), defaultDateTimeFormat) : ""));
    return ret;
}

var getValueDateTimeRange = function () {
    var valueStart = this.getComponent(0).getValue();
    var valueEnd = this.getComponent(1).getValue();
    var ret = ((!Ext.isEmpty(valueStart) ? ">=" + formatDate(dateFromJSON(Ext.Date.clearTime(valueStart, true).getTime()), defaultDateTimeFormat) : "")
        + (!Ext.isEmpty(valueEnd) ? "<=" + formatDate(dateFromJSON(Ext.Date.clearTime(valueEnd, true).getTime()), defaultDateTimeFormat) : ""));
    return ret;
}

var getValueDateTimeRange = function () {
    var valueStart = this.getComponent(0).getValue();
    var valueEnd = this.getComponent(1).getValue();
    return ((!Ext.isEmpty(valueStart) ? ">=" + formatDate(dateFromJSON(valueStart.getTime()), defaultDateTimeFormat) : "")
        + (!Ext.isEmpty(valueEnd) ? "<=" + formatDate(dateFromJSON(valueEnd.getTime()), defaultDateTimeFormat) : ""));
}

var getValueNumeric = function () {
    var value = this.getComponent(0).getValue().replace(/[^-0-9\.]+/g, "");
    return value;
}

var getValueNumericRange = function () {
    var valueStart = this.getComponent(0).getValue().replace(/[^-0-9\.]+/g, "");
    var valueEnd = this.getComponent(1).getValue().replace(/[^-0-9\.]+/g, "");
    return ((!Ext.isEmpty(valueStart) ? ">=" + valueStart : "") + (!Ext.isEmpty(valueEnd) ? "<=" + valueEnd : ""));
}

var showInfoAlertRedirect = function (title, message, destinationUrl) {
    Ext.MessageBox.show({
        closable: true,
        message: message,
        iconCls: "icon-information",
        title: title,
        buttons: Ext.MessageBox.YES,
        fn: function () {
            location.assign(destinationUrl);
        },
        icon: Ext.MessageBox.INFO
    });
}

var showInfoAlert = function (title, message) {
    Ext.MessageBox.show({
        closable: true,
        message: message,
        iconCls: "icon-information",
        title: title,
        buttons: Ext.MessageBox.OK,
        icon: Ext.MessageBox.INFO
    });
}

var showErrorAlert = function (title, message) {
    Ext.MessageBox.show({
        closable: true,
        message: message,
        iconCls: "icon-exclamation",
        title: title,
        buttons: Ext.MessageBox.OK,
        icon: Ext.MessageBox.ERROR
    });
}

var showWarningAlert = function (title, message) {
    Ext.MessageBox.show({
        closable: true,
        message: message,
        iconCls: "icon-exclamation",
        title: title,
        buttons: Ext.MessageBox.OK,
        icon: Ext.MessageBox.WARNING
    });
}

var showInfo = function (title, message) {
    Ext.Msg.info({
        closable: true,
        html: message,
        pinEvent: 'mouseover',
        hideDelay: 2500,
        ui: 'info',
        title: title,
        iconCls: 'icon-error',
        queue: sQueueName
    });
}

var showError = function (title, message) {
    Ext.Msg.info({
        closable: true,
        html: message,
        pinEvent: 'mouseover',
        hideDelay: 2500,
        ui: 'danger',
        title: title,
        iconCls: 'icon-exclamation',
        queue: sQueueName
    });
}

var showWarning = function (title, message) {
    Ext.Msg.info({
        closable: true,
        html: message,
        pinEvent: 'mouseover',
        hideDelay: 2500,
        ui: 'warning',
        title: title,
        iconCls: 'icon-error',
        queue: sQueueName
    });
}

var numericValue = function (value) {
    value = value.replace(/[^-0-9\.]+/g, "");
    return value;
}

var getModelNameOfGridPanel = function (ControlGridPanel) {
    var arrModelName = new Array();
    var iCount = -1;
    if (Ext.getCmp(ControlGridPanel) != undefined) {
        var oControlGridPanel = Ext.getCmp(ControlGridPanel);
        for (var curRowKey in oControlGridPanel) {
            if (oControlGridPanel.hasOwnProperty(curRowKey)) {
                var rowGridList = oControlGridPanel[curRowKey];
                var oGridListStore = rowGridList["store"]["model"]["fields"];
                for (var keyRowGridList in oGridListStore) {
                    if (keyRowGridList == oGridListStore.length - 1)
                        break;
                    if (oGridListStore.hasOwnProperty(keyRowGridList)) {
                        var oItems = oGridListStore[keyRowGridList];
                        var oItem = oItems["name"];
                        iCount++;
                        arrModelName.push(oItem);
                    }
                }
                break;
            }
        }
    }
    return arrModelName;
}

var exportGrid = function (grid, selectedOnly, visibleOnly, controller, type, fileName) {
    grid.submitData(
        {
            selectedOnly: selectedOnly,
            visibleOnly: visibleOnly,
            excludeId: true
        },
        {
            isUpload: true,
            url: controller + "/ExportGrid?type=" + type + "&fileName=" + fileName
        }
    );
}

function getParameterByName(name, url) {
    if (!url) {
        url = window.location.href;
    }
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}

var printGrid = function (grid, window) {
    window.show();
    var oBody = window.getBody();

    oBody.document.body.innerHTML = grid.body.dom.innerHTML;
    oBody.document.getElementById(grid.view.el.id).style.height = "auto";
    oBody.document.getElementById(grid.view.scroller.id).style.height = "auto";
}