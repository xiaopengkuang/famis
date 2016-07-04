//========================全局数据======================================//
var CurrentList = new Array();

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

function loadInitData()
{
    load_Department();
    load_CFDD_add();
    LoadInitData_datagrid();
}

function load_Department() {
    $('#LYBM_add').combotree
     ({
         url: '/Dict/load_SZBM',
         valueField: 'id',
         textField: 'nameText',
         required: true,
         method: 'POST',
         editable: false,
         //选择树节点触发事件  
         onSelect: function (node) {
         }, //全部折叠
         onLoadSuccess: function (node, data) {
             //$('#SZBM_add').combotree('tree').tree("collapseAll");
         }
     });

}


function load_CFDD_add() {
      $('#CFDD_add').combotree({
        url: '/Dict/load_DictTree?nameFlag=2_CFDD',
        valueField: 'id',
        textField: 'nameText',
        required: true,
        method: 'POST',
        editable: false,
        //选择树节点触发事件  
        onSelect: function (node) {
            //返回树对象  
            var tree = $(this).tree;
            //选中的节点是否为叶子节点,如果不是叶子节点,清除选中  
            d_CFDD_add = $('#CFDD_add').combotree('getValue');
            //
        }, //全部折叠
        onLoadSuccess: function (node, data) {
            $('#CFDD_add').combotree('tree').tree("collapseAll");
        }
    });
}



function LoadInitData_datagrid() {

    var dataGrid = "datagrid";

    var _list = "";
    for (var i = 0; i < CurrentList.length; i++) {
        if (i == 0) {
            _list = CurrentList[i] + "";
        } else {
            _list = _list + "_" + CurrentList[i];
        }
    }
    
    $('#'+dataGrid).datagrid({
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
            { field: 'ID', checkbox: true, width: 50 },
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
    loadPageTool_Detail(dataGrid);
}

function loadPageTool_Detail(datagrid) {
    var pager = $('#' + datagrid).datagrid('getPager');	// get the pager of datagrid
    pager.pagination({
        buttons: [{
            text: '添加明细',
            iconCls: 'icon-add',
            height: 50,
            handler: function () {
                var url = "/Allocation/Allocation_SelectingAsset";
                var titleName = "选择资产";
                openModelWindow(url, titleName);
            }
        }, {
            text: '删除明细',
            iconCls: 'icon-remove',
            height:50,
            handler: function () {
                //获取选择行
                var rows = $('#' + datagrid).datagrid('getSelections');
                var IDS = [];
                for (var i = 0; i < rows.length; i++) {
                    IDS[i] = rows[i].ID;
                }
                //更新CurrentList
                removeSelect(IDS);
               
            }
        }, {
            text: '刷新',
            height: 50,
            iconCls: 'icon-reload',
            handler: function () {
                $('#' + datagrid).datagrid('reload');
                //alert('刷新');
            }
        }],
        beforePageText: '第',//页数文本框前显示的汉字  
        afterPageText: '页    共 {pages} 页',
        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录'
    });
}

function removeSelect(removeList)
{
    //alert(removeList);
    //var tmp = new Array();
    var fi = 0;
    for (var i = 0; i < removeList.length; i++) {
        var index=containAtIndex(CurrentList, removeList[i]); 
        if (index!= -1)
        {
            CurrentList.splice(index, 1);
        }
    }
    LoadInitData_datagrid();

}

function containAtIndex(list, value)
{
    for (var i = 0; i < list.length;i++)
    {
        if (list[i] == value)
        {
            return i;
        }
    }
    return -1;
}


function updateCurrentList(addList)
{
    var tmp = new Array();
    var fi = 0;
    for (var i = 0; i < CurrentList.length; i++)
    {
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
    var date_allocation = $('#date_add').datebox('getValue');

    var reason = $("#DBYY_add").val();

    var department = $("#LYBM_add").combotree("getValue");

    var address = $("#CFDD_add").combotree("getValue");


    var ps = $("#PS_add").val();

    //封装成json格式创给后台
    var listA = getListAseet_();
    var collar_add = {
        "date_allocation": date_allocation,
        "reason": reason,
        "department": department,
        "address": address,
        "ps": ps,
        "statelist": state_List,
        "assetList": listA
    };
  
    $.ajax({
        url: "/Allocation/Handler_allocation_add",
        type: 'POST',
        data: {
            "data": JSON.stringify(collar_add)
        },
        beforeSend: ajaxLoading,
        success: function (data) {
            ajaxLoadEnd();

            if (data > 0) {
                try {
                    window.parent.$('#tabs').tabs('close', '添加领用单');
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

function getListAseet_()
{
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




function cancelData() {

    $.messager.confirm('警告', '数据还未保存，您确定要取消吗?', function (r) {
        if (r) {
            try {
                window.parent.$('#tabs').tabs('close', '添加调拨单');
            } catch (e) { }
        }
    });


}

function openModelWindow(url, titleName) {
    var $winADD;
    $winADD = $('#modalwindow').window({
        title: titleName,
        width: 1000,
        height: 650,
        top: (($(window).height() - 1000) > 0 ? ($(window).height() - 1000) : 200) * 0.5,
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
