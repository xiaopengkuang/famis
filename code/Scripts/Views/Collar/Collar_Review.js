

function LoadInitData_datagrid(id, datagrid) {

    $('#' + datagrid).datagrid({
        url: '/Collar/LoadCollarDetailByID?id='+id,
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
            {
                field: 'state_asset', title: '资产状态', width: 50,
                formatter: function (data) {
                    if (data == "在用") {
                        return '<font color="#696969">' + data + '</font>';
                    }
                    else if (data == "借出") {
                        return '<font color="#FFD700">' + data + '</font>';
                    } else if (data == "闲置") {
                        return '<font color="#228B22">' + data + '</font>';
                    } else if (data == "报废") {
                        return '<font color="red">' + data + '</font>';
                    } else {
                        return data;
                    }
                }
            },
            { field: 'supplierID', title: '供应商', width: 50 }
        ]],
        singleSelect: false, //允许选择多行
        selectOnCheck: true,//true勾选会选择行，false勾选不选择行, 1.3以后有此选项
        checkOnSelect: true //true选择行勾选，false选择行不勾选, 1.3以后有此选项
    });
    $('#' + datagrid).datagrid('hideColumn', 'ID');
    loadPageTool_Detail(datagrid);
}

function loadPageTool_Detail(datagrid) {
    var pager = $('#' + datagrid).datagrid('getPager');	// get the pager of datagrid
    pager.pagination({
        buttons: [{
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




function submitForm(id_collar, id_state)
{
    //获取审核意见
    var shyj = $("#SHYJ_collar").val();
    var data = {
        "id_Item": id_collar,
        "id_state": id_state,
        "review": shyj
    }



    $.ajax({
        url: "/Collar/updateCollarStateByID",
        type: 'POST',
        data: {
            "data": JSON.stringify(data)
        },
        beforeSend: ajaxLoading,
        success: function (data) {
            ajaxLoadEnd();
            //// TODO  
            ////alert(data);
            //$("#printLable").text(data);
            //$("#printLable").val(data);
            //$.messager.alert('提示', "调皮的肖金龙3", 'info'); return;
            var result
            if (data > 0) {
                try{
                    parent.$("#modalwindow").window("close");
                    parent.loadInitData();
                } catch (e) {
                    $.messager.alert('警告', "系统正忙，请稍后继续！", 'warning');
                }

            } else {
                if (data == -2) {
                    $.messager.alert('警告', "请确认添加资产明细或者检查所有资产均为闲置状态！", 'warning');
                } else {
                    $.messager.alert('警告', "系统正忙，请稍后继续！", 'warning');
                }
            }


        }
    });
}



function cancelForm()
{
    try {
        parent.$("#modalwindow").window("close");
    } catch (e)
    {
        $.messager.alert('警告', "系统正忙，请稍后继续！", 'warning');
    }
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

