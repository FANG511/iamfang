/*
 * 公版前端script公用函式庫,請擺在最後的script參考,以確保相關javascript框架有被載入(jQuery,kendo等)
 */

//const { callbackify } = require("util");

window.alert = function (msg) { FtcAlert(msg); };
Window.prototype.alert = function (msg) { FtcAlert(msg); };


/**
 * FtcAlert: 取代預設alert,以避免跨站alert失效
 * @param {any} alertMsg: 提示訊息內容
 */
function FtcAlert(alertMsg) {
    SweetAlert(alertMsg);
}

/**
 * FtcConfirm: 取代預設confirm,以避免跨站confirm失效
 * @param {any} cfmMsg
 */
function FtcGridDestroyConfirm(cfmMsg, grid,row) {
    SweetGridDestroyConfirm(cfmMsg, grid, row);
}

/**
 * FtcConfirm: 使用SweetConfirm取代預設confirm,以避免跨站confirm失效
 * @param {any} cfmMsg: 提示訊息內容
 * @param {any} thenFunc: sweetalert.then()使用的自訂function(isconfirm)
 */
function FtcConfirm(cfmMsg, thenFunc) {
    SweetConfirm(cfmMsg, thenFunc);
}

function KendoAlert(alertMsg) {
    $("<div></div>").kendoAlert({
        title: "提示",
        content: alertMsg,
        messages: {
            okText: "Ok"
        }
    }).data("kendoAlert").open();
}

function KendoGridDestroyConfirm(cfmMsg, grid, row) {
    kendo.confirm(cfmMsg).done(function () { grid._removeRow(row); })
    //$(".k-confirm .k-window-titlebar .k-dialog-title ").css("visibility", "collapse");
}

/**
 * KendoConfirm: 取代預設confirm,以避免跨站alert失效
 * @param {any} cfmMsg: 提示訊息內容
 * @param {any} doneFunc: kendo.confirm().done()使用的自訂function(),無參數
 * @param {any} failFunc: kendo.confirm().fail()使用的自訂function(),無參數
 */
function KendoConfirm(cfmMsg, doneFunc, failFunc) {
    kendo.confirm(cfmMsg).done(doneFunc).fail(failFunc);
}

function SweetAlert(alertMsg) {
    return swal({
        title: "提示",
        text: alertMsg
    })
}

function SweetGridDestroyConfirm(cfmMsg, grid, row) {
    swal({
        text: cfmMsg,
        icon: "warning",
        dangerMode: true,
        buttons: {
            confirm: "Yes",
            cancel: "No",
        },
    }).then(function (isConfirm) { if (isConfirm) { grid._removeRow(row); } });
}

/**
 * SweetConfirm: 取代預設confirm,以避免跨站confirm失效
 * @param {any} cfmMsg: 提示訊息內容
 * @param {any} thenFunc: sweetalert.then()使用的自訂function(isconfirm)
 */
function SweetConfirm(cfmMsg, thenFunc) {
    swal({
        text: cfmMsg,
        icon: "warning",
        buttons: {
            confirm: "Yes",
            cancel: "No",
        }
    }).then(function (isConfirm) { if (isConfirm) { thenFunc(); } });
}
