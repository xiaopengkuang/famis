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


//==============================绑定数据=============================//
function bindData(id)
{
   
    $.ajax({
        url: "/Repair/getRepairByID",
        type: 'POST',
        data: {
            "id": id
        },
        beforeSend: ajaxLoading,
        success: function (data) {
            ajaxLoadEnd();
            if (data) {
                    //////绑定人员
                    $("#UAP_add").combobox("setValue", data.userID_applying);
                    $("#UAT_add").combobox("setValue", data.userID_authorize);
                    var date1 = dateString(data.date_ToRepair);
                    $("#dateToP").datebox("setValue", date1);
                    var date2 = dateString(data.date_ToReturn);
                    $("#dateToR").datebox("setValue", date2);

                    ////绑定原因
                    $("#reason_add").val(data.reason_ToRepair);
                    $("#note_add").val(data.note_repair);
                    ////绑定流水号
                     $("#DJH_add").val(data.serialNumber);
                    //绑定资产
                    selectedAssetID = data.ID_Asset;
                    $("#serialNUM_Asset").val(data.serial_asset);
                    $("#name_Asset").val(data.Name_asset);
                    $("#name_Equipment").val(data.Name_equipment);
                    $("#Cost_Repair").numberbox("setValue", data.CostToRepair);
                    $("#supplier_repair").combogrid("setValue", data.supplierID_Torepair);
                    $("#supplier_repair").combogrid("setText", data.supplierName_Torepair);
                    $("#Address_add").val(data.Address_supplier);
                    $("#LinkMan_add").val(data.linkMan_supplier);

                    //

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



//==============================绑定数据=============================//


//function dateString(date) {
//    var pa = /.*\((.*)\)/;
//    var unixtime = date.match(pa)[1].substring(0, 10);
//    return getTime(unixtime);
//}

function dateString(date) {
    try {
        var NewDtime = new Date(parseInt(date.slice(6, 19)));
        return formatDate(NewDtime);
    } catch (e) {
        return "";
    }

}


function formatDate(dt) {
    var year = dt.getFullYear();
    var month = dt.getMonth() + 1;
    var date = dt.getDate();
    var hour = dt.getHours();
    var minute = dt.getMinutes();
    var second = dt.getSeconds();
    return year + "-" + month + "-" + date;
}

function getTime(/** timestamp=0 **/) {
    var ts = arguments[0] || 0;
    var t, y, m, d, h, i, s;
    t = ts ? new Date(ts * 1000) : new Date();
    y = t.getFullYear();
    m = t.getMonth() + 1;
    d = t.getDate();
    h = t.getHours();
    i = t.getMinutes();
    s = t.getSeconds();
    // 可根据需要在这里定义时间格式  
    return y + '-' + (m < 10 ? '0' + m : m) + '-' + (d < 10 ? '0' + d : d);
}

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
            //search = false;
            $('#' + supplier_target).combogrid('hidePanel');
            $('#' + supplier_target).combogrid('setValue', row.ID);
            $('#' + supplier_target).combogrid('setText', row.name_supplier);
            $("#" + LinkMan_add).val(row.linkman);
            $("#" + Address_add).val(row.address);
            //setTimeout(function () {
            //    search = true;
            //}, 1000);

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
function saveData(info,id) {

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
        "state_List": state_List,
        "id":id
    };

    $.ajax({
        url: "/Repair/Handler_UpdateRepairList",
        type: 'POST',
        data: {
            "data": JSON.stringify(data_repair)
        },
        beforeSend: ajaxLoading,
        success: function (data) {
            ajaxLoadEnd();

            if (data > 0) {
                try {
                    window.parent.reloadTabGrid("资产维修");
                    window.parent.$('#tabs').tabs('close', '编辑维修单');
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
                window.parent.$('#tabs').tabs('close', '编辑维修单');
            } catch (e) { }
        }
    });
}

function openModelWindow(url, titleName) {
    //获取当前页面的Width和高度
    var winWidth = (document.body.clientWidth - 20) < 0 ? 0 : (document.body.clientWidth - 20);
    var winheight = (document.body.clientHeight - 20) < 0 ? 0 : (document.body.clientHeight - 20);
    try {
        $("#modalwindow").window("close");
    } catch (e) { }
    var $winADD;
    $winADD = $('#modalwindow').window({
        title: titleName,
        //width: 1028,
        //height: 650,
        //top: (($(window).height() - 650) > 0 ? ($(window).height() - 650) : 200) * 0.5,
        //left: (($(window).width() - 1028) > 0 ? ($(window).width() - 1028) : 100) * 0.5,
        width: winWidth,
        height: winheight,
        left: 10,
        top: 10,
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


function checkFormat(id_repair) {
    //基础属性
    var check_obj_dataToP = $('#dateToP').datebox("getValue");
    var check_obj_dataToR = $('#dateToR').datebox("getValue");
    var check_obj_supplier_repair = $("#supplier_repair").combobox("getValue");
    var check_obj_UAP = $("#UAP_add").combobox("getValue");
    var check_obj_UAT = $("#UAT_add").combobox("getValue");
    var check_obj_reason = $("#reason_add").val();
    var check_obj_serialNUM_Asset = $("#serialNUM_Asset").val();
    var check_obj_name_Asset = $("#name_Asset").val();
    var check_obj_Cost_Repair = $("#Cost_Repair").numberbox("getValue");
    var check_obj_name_Equipment = $("#name_Equipment").val();
    if (isNull(check_obj_dataToP)) {
        MessShow("送修日期不能为空");
    } else if (isNull(check_obj_dataToR)) {
        MessShow("预计归还日期不能为空");
    } else if (isNull(check_obj_supplier_repair)) {
        MessShow("维修商不能为空");
    } else if (isNull(check_obj_UAP)) {
        MessShow("申请人不能为空");
    } else if (isNull(check_obj_UAT)) {
        MessShow("批准人不能为空");
    } else if (isNull(check_obj_reason)) {
        MessShow("维修原因不能为空");
    } else if (isNull(check_obj_serialNUM_Asset)) {
        MessShow("资产编号不能为空");
    } else if (isNull(check_obj_name_Asset)) {
        MessShow("资产名称不能为空");
    } else if (isNull(check_obj_Cost_Repair)) {
        MessShow("维修费用不能为空");
    } else if (isNull(check_obj_name_Equipment)) {
        MessShow("设备名称不能为空");
    } else {
        saveData('1', id_repair);
    }
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