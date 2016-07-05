//========================全局数据======================================//
var selectedAssetID=null;
//========================全局数据======================================//




//====================================================================//
function myformatter(date) {
    var y = date.getFullYear();
    var m = date.getMonth() + 1;
    var d = date.getDate();
    return y + '-' + (m < 10 ? ('0' + m) : m) + '-' + (d < 10 ? ('0' + d) : d);
}
function myparser(s) {
    if (!s) return new Date();
    var ss = (s.split('-'));
    var y = parseInt(ss[0], 10);
    var m = parseInt(ss[1], 10);
    var d = parseInt(ss[2], 10);
    if (!isNaN(y) && !isNaN(m) && !isNaN(d)) {
        return new Date(y, m - 1, d);
    } else {
        return new Date();
    }
}
//====================================================================//

//=======================初始化加载信息===================================//
$(function () {
    loadInitData();
});

function loadInitData() {
    load_User("UAP_add");
    load_User("UAT_add");
    loadSupplier("supplier_repair", "LinkMan_add", "Address_add");
}


function loadSupplier(supplier_target, LinkMan_add, Address_add)
{
    $('#' + supplier_target).combogrid({
        panelWidth: 300,
        value: '006',
        idField: 'code',
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
            search = false;
            $('#' + supplier_target).combogrid('hidePanel');
            $('#' + supplier_target).combogrid('setValue', row.ID);
            $('#' + supplier_target).combogrid('setText', row.name_supplier);
            $("#" + LinkMan_add).val(row.linkman);
            $("#" + Address_add).val(row.address);
            setTimeout(function () {
                search = true;
            }, 1000);

        }
    });
}

function load_User(comboboxID) {
    $("#" + comboboxID).combobox({
        valueField: 'id',
        method: 'POST',
        textField: 'name',
        url: '/Dict/load_User_add',
        onSelect: function (rec) {
            $('#' + comboboxID).combobox('setValue', rec.id);
            $('#' + comboboxID).combobox('setText', rec.name);
        }
    });
}


function loadSeletedAsset(id_asset)
{
    $.ajax({
        url: "/Asset/getAssetByID",
        type: 'POST',
        data: {
            "id": id_asset
        },
        beforeSend: ajaxLoading,
        success: function (data) {
            ajaxLoadEnd();
            if (data != null) {
                //数据绑定
                $("#serialNUM_Asset").val(data.serial_number);
                $("#name_Asset").val(data.name_Asset);
                $("#name_Equipment").val('');
                $('#Cost_Repair').numberbox('clear');
            } else {
                selectedAssetID = null;
                clearSelected();
            }


        }
    });
}




function SelectAsset()
{
    if (selectedAssetID != null)
    {
        MessShow("请先删除选中的资产！");
        return;
    }
    //先判断是否已近添加过Asset
    var titleName = "选择资产";
    var url = "/Repair/Repair_SelectingAsset";
    openModelWindow(url, titleName);
}

function clearSelected()
{
    selectedAssetID = null;
    //清空数据
    $("#serialNUM_Asset").val('');
    $("#name_Asset").val('');
    $("#name_Equipment").val('');
    $('#Cost_Repair').numberbox('clear');
}



function updateSelected(id_selectd)
{
    selectedAssetID = id_selectd;
    loadSeletedAsset(selectedAssetID);
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
//==================================================================================//


//==============================================================获取表单数据===========================================================================//
function saveData(info) {

    var state_List;
    if (info == "1") {
        state_List = 1;

    } else if (info == "2") {
        state_List = 2;
    } else {

    }
    //获取页面数据
    var dateToP = $('#dateToP').datebox('getValue');
    var dateToR = $('#dateToR').datebox('getValue');

    var UAP_add = $("#UAP_add").combobox("getValue");
    var UAT_add = $("#UAT_add").combobox("getValue");
    var supplier_repair = $("#supplier_repair").combogrid("getValue");

    var reason_add = $("#reason_add").val();
    var note_add = $("#note_add").val();
  
    var id_asset_repair = selectedAssetID;
    var serialNUM_Asset = $("#serialNUM_Asset").val();
    var name_Asset = $("#name_Asset").val();
    var name_Equipment = $("#name_Equipment").val();

    var Cost_Repair = $("#Cost_Repair").numberbox("getValue");




    //封装成json格式创给后台
    var data_repair = {
        "dateToP": dateToP,
        "dateToR": dateToR,
        "UAP_add": UAP_add,
        "UAT_add": UAT_add,
        "supplier_repair":supplier_repair,
        "reason_add": reason_add,
        "note_add": note_add,
        "id_asset_repair": id_asset_repair,
        "serialNUM_Asset": serialNUM_Asset,
        "name_Asset": name_Asset,
        "name_Equipment": name_Equipment,
        "Cost_Repair": Cost_Repair,
        "state_List":state_List
    };

    $.ajax({
        url: "/Repair/Handler_InsertRepairList",
        type: 'POST',
        data: {
            "data": JSON.stringify(data_repair)
        },
        beforeSend: ajaxLoading,
        success: function (data) {
            ajaxLoadEnd();

            if (data > 0) {
                try {
                    window.parent.$('#tabs').tabs('close', '添加维修单');
                } catch (e) {
                    $.messager.alert('提示', '系统忙，请手动关闭该面板', 'info');
                }
            } else {
                if (data == -4) {
                    $.messager.alert('警告', "异常来了,亲休息一下吧！", 'warning');
                } else {
                    $.messager.alert('警告', "系统正忙，请稍后继续！", 'warning');
                }
            }


        }
    });
}





function cancelData() {

    $.messager.confirm('警告', '数据还未保存，您确定要取消吗?', function (r) {
        if (r) {
            try {
                window.parent.$('#tabs').tabs('close', '添加维修单');
            } catch (e) { }
        }
    });
}

function openModelWindow(url, titleName) {
    var $winADD;
    $winADD = $('#modalwindow').window({
        title: titleName,
        width: 1028,
        height: 650,
        top: (($(window).height() - 1028) > 0 ? ($(window).height() - 1028) : 200) * 0.5,
        left: (($(window).width() - 650) > 0 ? ($(window).width() - 650) : 100) * 0.5,
        shadow: true,
        modal: true,
        iconCls: 'icon-add',
        closed: true,
        minimizable: false,
        maximizable: false,
        collapsible: false,
        onClose: function () {
        }
    });
    $("#modalwindow").html("<iframe width='100%' height='99%'  frameborder='0' src='" + url + "'></iframe>");
    $winADD.window('open');
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

//==============================================================获取表单数据===========================================================================//
