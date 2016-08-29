//========================全局数据======================================//
var CurrentList = new Array();
//========================全局数据======================================//

//========================方法======================================//

function bindData(id)
{
    $.ajax({
        url: "/Reduction/loadReductionByID",
        type: 'POST',
        data: {
            "id": id
        },
        beforeSend: ajaxLoading,
        success: function (data) {
            ajaxLoadEnd();
            if (data) {
              //绑定数据：
                $("#serialNum").val(data.Serial_number);

                var date1 = dateString(data.date_reduction);
                $("#date_reduction").datebox("setValue", date1);

                $("#FS_SJ").combobox("setValue", data.method_reduction);
                $("#FS_SJ").combobox("setText", data.methodName_reduction);


                //////绑定人员
                $("#UAP_add").combobox("setValue", data.userID_apply);
                $("#UAP_add").combobox("setText", data.user_apply);
                $("#UAT_add").combobox("setValue", data.userID_approver);
                $("#UAT_add").combobox("setText", data.user_approver);
                ////绑定原因
                $("#reason_add").val(data.reason_reduce);
                $("#note_add").val(data.note_reduce);
                CurrentList = data.ids_asset;
                LoadInitData_datagrid();

                $("#UAP_add").combobox('disable');
                $("#UAT_add").combobox('disable');
                $("#FS_SJ").combobox('disable');
                $("#date_reduction").datebox('disable');
                $('#date_reduction').datebox({
                    disabled: true,
                    editable: false
                });


            } else {
                
                return;
            }
        }
    });
}


$(function () {

    loadInitData();
});

function loadInitData()
{
    load_User("UAP_add");
    load_User("UAT_add");
    load_FS();
    LoadInitData_datagrid();
}
//获取用户
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

function load_FS() {
    $("#FS_SJ").combobox({
        valueField: 'ID',
        method: 'POST',
        editable: false,
        textField: 'name_para',
        url: '/Dict/load_FS_add?nameFlag=2_JSFS',
        onSelect: function (rec) {
            $('#FS_SJ').combobox('setValue', rec.ID);
            $('#FS_SJ').combobox('setText', rec.name_para);
        }
    });
}

//加载选中
function LoadInitData_datagrid()
{
    //获取
    var _list = "";
    for (var i = 0; i < CurrentList.length; i++) {
        if (i == 0) {
            _list = CurrentList[i] + "";
        } else {
            _list = _list + "_" + CurrentList[i];
        }
    }

    $('#datagrid_reduction_add').datagrid({
        url: '/Common/Load_SelectedAsset?selectedIDs=' + _list,
        method: 'POST', //默认是post,不允许对静态文件访问
        width: 'auto',
        height: '300px',
        iconCls: 'icon-save',
        dataType: "json",
        fitColumns: true,
        pagePosition: 'top',
        rownumbers: true, //是否加行号 
        pagination: true, //是否显式分页 
        pageSize: 15, //页容量，必须和pageList对应起来，否则会报错 
        pageNumber: 1, //默认显示第几页 
        pageList: [15, 30, 45],//分页中下拉选项的数值 
        columns: [[
            { field: 'ID', checkbox: true, width: 50,hidden:true },
            { field: 'serial_number', title: '资产编号', width: 50 },
            { field: 'name_Asset', title: '资产名称', width: 50 },
            { field: 'type_Asset', title: '资产类型', width: 50 },
            { field: 'specification', title: '型号规范', width: 50 },
            { field: 'unit_price', title: '单价', width: 50 },
            { field: 'amount', title: '数量', width: 50 },
            { field: 'department_Using', title: '使用部门', width: 50 },
            { field: 'addressCF', title: '地址', width: 50 },
            { field: 'Method_add', title: '添加方式', width: 50 },
            { field: 'state_asset', title: '资产状态', width: 50 },
            { field: 'supplierID', title: '供应商', width: 50 }
        ]],
        singleSelect: false, //允许选择多行
        selectOnCheck: true,//true勾选会选择行，false勾选不选择行, 1.3以后有此选项
        checkOnSelect: true //true选择行勾选，false选择行不勾选, 1.3以后有此选项
    });
    loadPageTool_Detail();

}
function loadPageTool_Detail() {
    var pager = $('#datagrid_reduction_add').datagrid('getPager');	// get the pager of datagrid
    pager.pagination({
       
    buttons: [{
        text: '下载明细',
        height: 50,
        iconCls: 'icon-reload',
        handler: function () {
            //取消当前编辑行把当前编辑行罢undefined回滚改变的数据,取消选择的行
            var serial = $("#serialNum").val();
            //alert(serial);
            var form = $("<form>");//定义一个form表单
            form.attr("style", "display:none");
            form.attr("target", "");
            form.attr("method", "post");
            form.attr("action", "/Verify/Export?serial=" + serial + "&type=" + "JS");
            var input1 = $("<input>");
            input1.attr("type", "hidden");
            input1.attr("name", "exportData");
            input1.attr("value", (new Date()).getMilliseconds());
            $("body").append(form);//将表单放置在web中
            form.append(input1);
            form.submit();//表单提交
        }
    }],
        beforePageText: '第',//页数文本框前显示的汉字  
        afterPageText: '页    共 {pages} 页',
        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录'
    });
}

function removeSelect(removeList) {
    //alert(removeList);
    //var tmp = new Array();
    var fi = 0;
    for (var i = 0; i < removeList.length; i++) {
        var index = containAtIndex(CurrentList, removeList[i]);
        if (index != -1) {
            CurrentList.splice(index, 1);
        }
    }
    LoadInitData_datagrid();

}

function containAtIndex(list, value) {
    for (var i = 0; i < list.length; i++) {
        if (list[i] == value) {
            return i;
        }
    }
    return -1;
}


function updateCurrentList(addList) {
    var tmp = new Array();
    var fi = 0;
    for (var i = 0; i < CurrentList.length; i++) {
        tmp[i] = CurrentList[i];
        fi++;
    }
    for (var i = 0; i < addList.length; i++) {
        tmp[fi] = addList[i];
        fi++;
    }
    CurrentList = tmp;
    //alert(tmp.toString())
    LoadInitData_datagrid();
}
function getListAseet_() {
    var _list = "";
    for (var i = 0; i < CurrentList.length; i++) {
        if (i == 0) {
            _list = CurrentList[i] + "";
        } else {
            _list = _list + "_" + CurrentList[i];
        }
    }
    return _list;
}





//========================方法======================================//






//=================================================================//

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
function dateString(date) {
    var pa = /.*\((.*)\)/;
    var unixtime = date.match(pa)[1].substring(0, 10);
    return getTime(unixtime);
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

//====================================================================//