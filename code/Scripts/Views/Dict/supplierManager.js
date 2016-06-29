$(function () {


    loadIintData();

});


var dataGrid_ID = "datagrid_gys";

function loadIintData() {

    var url = "/Dict/load_supplier";
    loadDataGrid(dataGrid_ID, url, false);
}







function loadDataGrid(datagrid, url, toolbar) {

    //alert(url);
    //获取选中行
    //
    $('#' + datagrid).datagrid({
        url: url,
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
            { field: 'id', checkbox: true, width: 50, hidden: toolbar },
            { field: 'name', title: '名称', width: 50 },
            { field: 'lineMan', title: '负责人', width: 100 },
            { field: 'addree', title: '地址', width: 50 }

        ]],
        singleSelect: false, //允许选择多行
        selectOnCheck: true,//true勾选会选择行，false勾选不选择行, 1.3以后有此选项
        checkOnSelect: true //true选择行勾选，false选择行不勾选, 1.3以后有此选项
    });
    loadPageTool(datagrid, toolbar);
}

function loadPageTool(datagrid, toolbarDisable) {
    //alert(toolbar)
    var pager = $('#' + datagrid).datagrid('getPager');	// get the pager of datagrid
    pager.pagination({
        buttons: [{
            text: '添加',
            iconCls: 'icon-add',
            height: 50,
            disabled: toolbarDisable,
            handler: function () {
                if (toolbarDisable) {
                    return;
                }
                addItem();
                
            }
        },{
            text: '编辑',
            iconCls: 'icon-remove',
            height: 50,
            disabled: toolbarDisable,
            handler: function () {
                if (toolbarDisable) {
                    return;
                }
                //获取选中行：
                editItem(datagrid);
               
            }
        }, {
            text: '删除',
            iconCls: 'icon-remove',
            height: 50,
            disabled: toolbarDisable,
            handler: function () {
                if (toolbarDisable) {
                    return;
                }
                deleteItems(datagrid);
               
            }
        }],
        beforePageText: '第',//页数文本框前显示的汉字  
        afterPageText: '页    共 {pages} 页',
        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录'
    });

}

function editItem(datagrid)
{
    //获取选中行
    var rows = $('#' + datagrid).datagrid('getSelections');
    var ids;
    //alert(rows.length + "L:E");
    if (rows.length < 1)
    {
        $.messager.alert('警告', "请选择一项数据！", 'warning');
    }
    if (rows.length > 1) {
        $.messager.alert('警告', "只能选择一项数据！", 'warning');
        return;
    }


    var url = "/Dict/edit_supplierView?id=" + rows[0].id;
    var titleNam = "供应商-添加";
    openModelWindow(url, titleNam)
}

function addItem()
{
    var url = "/Dict/add_supplierView";
    var titleNam="供应商-添加";
    openModelWindow(url, titleNam)

   
}


function deleteItems(dataGrid)
{
    //获取选中行：
    var rows = $('#' + dataGrid).datagrid('getSelections');
    var ids;
    //alert(rows.length + "L:E");
    if (rows.length < 1) {
        return;
    }
    for (var i = 0; i < rows.length; i++) {
        if (i == 0) {
            ids = "" + rows[i].id;
        } else {
            ids += "_" + rows[i].id;
        }
    }
    $.ajax({
        url: "/Dict/Handler_deleteGYS",
        type: 'POST',
        data: {
            "ids": ids
        },
        beforeSend: ajaxLoading,
        success: function (data) {
            ajaxLoadEnd();
            var result;
            if (data > 0) {
                try {
                    $("#" + dataGrid).datagrid('reload');
                } catch (e) {

                }
            } else {
                result = "系统正忙，请稍后继续！";
                $.messager.alert('警告', result, 'warning');
            }
        }
    });
}





function openModelWindow(url, titleName) {
    var $winADD;
    $winADD = $('#modalwindow').window({
        title: titleName,
        width: 380,
        height: 350,
        top: (($(window).height() - 380) > 0 ? ($(window).height() - 380) : 100) * 0.5,
        left: (($(window).width() - 350) > 0 ? ($(window).width() - 350) : 200) * 0.5,
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