$(function () {
    loadInitData();
});


function loadInitData()
{
    load_GYS_add();
    load_JLDW_add();
}

function load_JLDW_add() {
    $("#JLDW_add").combobox({
        valueField: 'ID',
        method: 'POST',
        editable: false,
        textField: 'name_para',
        url: '/Dict/load_FS_add?nameFlag=2_JLDW',
        onSelect: function (rec) {
            $('#JLDW_add').combobox('setValue', rec.ID);
            $('#JLDW_add').combobox('setText', rec.name_para);
        },
        onLoadSuccess: function () {
            var data = $('#JLDW_add').combobox('getData');
            if (data.length > 0) {
                $('#JLDW_add').combobox('select', data[0].ID);
            }
        }
    });
}
function load_GYS_add() {
    $('#GYS_add').combogrid({
        panelWidth: 300,
        value: '006',
        idField: 'code',
        editable: false,
        textField: 'name',
        url: '/Dict/load_GYS_add',
        method: 'POST', //默认是post,不允许对静态文件访问
        columns: [[
        { field: 'ID', checkbox: true, title: 'ID', width: 99, hidden: true },
        { field: 'name_supplier', title: '供应商', width: 99 },
        { field: 'linkman', title: '联系人', width: 99 },
        { field: 'address', title: '地址', width: 99 }
        ]],
        onClickRow: function (index, row) {
            //search = false;
            $('#GYS_add').combogrid('hidePanel');
            $('#GYS_add').combogrid('setValue', row.ID);
            $('#GYS_add').combogrid('setText', row.name_supplier);
        },
        onLoadSuccess: function () {
            var dg = $('#GYS_add').combogrid('grid');
            var rows = dg.datagrid("getRows");
            if (rows.length > 0) {
                $('#GYS_add').combogrid('setValue', rows[0].ID);
                $('#GYS_add').combogrid('setText', rows[0].name_supplier);
            }
        }
    });
}

function checkFormat(id_asset)
{
    var serialNum = $("#ZCBH_add").val();
    var name = $("#ZCMC_add").val();
    var note = $("#note").val();
    var specification = $("#ZCXH_add").val();
    var measurement = $("#JLDW_add").combobox("getValue");
    var value = $("#Other_ZCJZ_add").numberbox("getValue");
    var supplier = $("#GYS_add").combogrid("getValue");
    if (isNull(serialNum))
    {
        MessShow("资产编号不能为空");
        return;
    }
    if (isNull(name)) {
        MessShow("资产名称不能为空");
        return;
    }
    if (isNull(specification)) {
        MessShow("资产型号不能为空");
        return;
    }
    if (isNull(measurement)) {
        MessShow("计量单位不能为空");
        return;
    }
    if (isNull(supplier)) {
        MessShow("供应商不能为空");
        return;
    }
    if (isNull(value)) {
        MessShow("资产价值不能为空不能为空");
        return;
    }
    submitData(id_asset)
}


function submitData(id_asset)
{
    var serialNum = $("#ZCBH_add").val();
    var name = $("#ZCMC_add").val();
    var note = $("#note").val();
    var specification = $("#ZCXH_add").val();
    var measurement = $("#JLDW_add").combobox("getValue");
    var value = $("#Other_ZCJZ_add").numberbox("getValue");
    var supplier = $("#GYS_add").combogrid("getValue");
    //var id_a = parseInt(id_asset);
    var data = {
        "serialNum": serialNum,
        "name": name,
        "note": note,
        "specification": specification,
        "measurement": measurement,
        "value": value,
        "supplier": supplier,
        "id_asset": id_asset
    };
    $.ajax({
        url: "/Asset/Handler_add_subEquiment",
        type: 'POST',
        data: {
            "data": JSON.stringify(data)
        },
        beforeSend: ajaxLoading,
        success: function (data) {
            ajaxLoadEnd();
            var result
            if (data > 0) {
                parent.$("#TableList_SB").datagrid("reload");
                parent.$("#modalwindow2").window("close");
            } else {
                result = "系统正忙，请稍后继续！";
                $.messager.alert('警告', result, 'warning');
            }

        }
    });


}

//判值是否为空
function isNull(data) {
    return (data == "" || data == undefined || data == null) ? true : false;
}
function MessShow(mess) {
    $.messager.show({
        title: '提示',
        msg: mess,
        showType: 'slide',
        style: {
            right: '',
            top: document.body.scrollTop + document.documentElement.scrollTop,
            bottom: ''
        }
    });
}

//采用jquery easyui loading css效果
function ajaxLoading() {
    $("<div class=\"datagrid-mask\"></div>").css({ display: "block", width: "100%", height: $(window).height() }).appendTo("body");
    $("<div class=\"datagrid-mask-msg\"></div>").html("正在处理，请稍候。。。").appendTo("body").css({ display: "block", left: ($(document.body).outerWidth(true) - 190) / 2, top: ($(window).height() - 45) / 2 });
}
function ajaxLoadEnd() {
    $(".datagrid-mask").remove();
    $(".datagrid-mask-msg").remove();
}

