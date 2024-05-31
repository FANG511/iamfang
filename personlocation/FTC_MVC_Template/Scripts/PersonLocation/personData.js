// 定義變數
var oQueryPara = {
    PersonIDww: "",
};
var oUpdataPara = { "ModifyUser": "admin", "PersonId": "" };
var dsUserInfo, dsCompany, dsFactory, dsPerson;
var gridUserInfo;
var fkCompany = [], fkFactory = [], fkUser = [], fkUserName = [], fkGender = ["男", "女"];
var viewModel;
var crudServiceBaseUrl = "/api/apiGrid";
var personLocationUrl = "/api/apiPersonLocation";

// 初始化 ViewModel
$(document).ready(function () {
    QueryString.Initial();

    viewModel = kendo.observable({
        PersonID: "",
        Name: "",
        CompanyID: "",
        Gender: "",
        Phone: ""
    });
    kendo.bind($("#divQueryParameter"), viewModel);

    // 取得公司基本資料
    dsCompany = new kendo.data.DataSource({
        transport: {
            read: {
                url: personLocationUrl + "/GetCompany",
                type: "Post",
                async: false,
                dataType: "json"
            },
            parameterMap: function (options, operation) {
                return oQueryPara;
            }
        }
    });
    dsCompany.read();
    dsCompany._data.forEach(function (e) {
        fkCompany.push({ "value": e.CompanyID, "text": e.CompanyName });
    });

    // 定義 DataSource
    dsUserInfo = new kendo.data.DataSource({
        transport: {
            read: {
                url: personLocationUrl + "/GetPersonData",
                type: "Post",
                dataType: "json"
            },
            update: {
                url: personLocationUrl + "/UpdatePersonData",
                type: "Post",
                dataType: "json"
            },
            destroy: {
                url: personLocationUrl + "/DeletePersonData",
                type: "Post",
                dataType: "json"
            },
            create: {
                url: personLocationUrl + "/AddPersonData",
                type: "Post",
                dataType: "json"
            },
            parameterMap: function (options, operation) {
                if (operation !== "read" && options.models) {
                    let newItem = options.models
                    return newItem[0];
                }
                else {
                    return oQueryPara;
                }
            }
        },
        batch: true,
        requestEnd: function (e) {
            var type = e.type;
            if (type != 'read') {
                if (e.response.ReturnCode != 0) {
                    alert(e.response.ReturnMessage);
                }
                gridRefresh();
            }
        },
        schema: {
            model: {
                id: "PersonID",
                fields: {
                    PersonID: { validation: { required: true } },
                    Name: { validation: { required: true } },
                    Gender: { validation: { required: true } },
                    Phone: { validation: { required: true } },
                    CompanyID: { validation: { required: true } },
                }
            }
        },
        pageSize: 10,
    });

    // 定義 Grid
    $("#grid").kendoGrid({
        dataSource: dsUserInfo,
        autoBind: true,
        editable: {
            mode: "popup",
            window: {
                title: "新增"
            }
        },
        height: 500,
        resizable: true,
        columnMenu: true,
        sortable: true,
        filterable: { extra: false },
        scrollable: true,
        selectable: "row",
        pageable: {
            refresh: true,
            pageSizes: true,
            buttonCount: 5
        },
        excel: {
            fileName: "UserInfo.xlsx",
            allPages: true
        },
        toolbar: ["create"],
        edit: function (e) {
            if (!e.model.isNew()) {
                var nameField = e.container.find("input[name=name]");
                var name = nameField.val();
                e.container.data("kendoWindow").title("修改");
            }
        },
        persistSelection: true,
        columns: [
            {
                command: [
                    { name: "edit", text: { edit: "", update: "儲存", cancel: "取消" } },
                    { name: "destroy", text: "" }
                ], title: " ", width: "80px"
            },
            { field: "PersonID", title: "人員ID", width: "100px" },
            { field: "Name", title: "姓名", width: "80px" },
            { field: "Gender", title: "性別", width: "100px", values: fkGender },
            { field: "Phone", title: "電話", width: "100px" },
            { field: "CompanyID", title: "公司名稱", width: "80px", values: fkCompany },
        ]
    });
    gridUserInfo = $("#grid").data("kendoGrid");
    $('#txtGender').kendoDropDownList({ dataSource: fkGender });
    $('#txtCompanyID').kendoDropDownList({
        dataTextField: "text",
        dataValueField: "value",
        dataSource: fkCompany
    });
    dsPerson = new kendo.data.DataSource({
        transport: {
            read: {
                url: personLocationUrl + "/GetPersonData",
                type: "Post",
                async: false,
                dataType: "json"
            }
        }
    });
    dsPerson.read();
    dsPerson._data.forEach(function (e) {
        fkUser.push({ "value": e.PersonId, "text": e.PersonId });
        fkUserName.push({ "value": e.Name, "text": e.Name });
    })
    $('#txtPersonId').kendoAutoComplete({
        dataTextField: "value",
        dataValueField: "value",
        dataSource: fkUser,
        select: function (e) {
            var dataId = e.dataItem.value;
            $('#txtPersonId').attr('data-id', dataId);
        }
    });
    $('#txtName').kendoAutoComplete({
        dataTextField: "value",
        dataValueField: "value",
        dataSource: fkUserName,
        select: function (e) {
            var dataId = e.dataItem.value;
            $('#txtName').attr('data-id', dataId);
        }
    });
})

// 更新 Grid 資料
function gridRefresh() {
    oQueryPara["PersonID"] = viewModel.PersonID;
    oQueryPara["Name"] = viewModel.Name;
    oQueryPara["CompanyID"] = viewModel.CompanyID;
    oQueryPara["Gender"] = viewModel.Gender;
    oQueryPara["Phone"] = viewModel.Phone;
    dsUserInfo.read();
}

// 新增一個項目
function gridAdd() {
    gridUserInfo.addRow();
}

// 匯出為 Excel
function ExportXls() {
    gridUserInfo.saveAsExcel();
}

function isEditable(e) {
    return e.NotesID == "";
}

