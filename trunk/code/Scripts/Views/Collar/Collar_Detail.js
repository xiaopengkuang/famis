

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
        buttons: [{
            text: '下载明细',
            height: 50,
            iconCls: 'icon-reload',
            handler: function () {
                //取消当前编辑行把当前编辑行罢undefined回滚改变的数据,取消选择的行
                var serial = $("#DJH_add").val();
                //alert(serial);
                var form = $("<form>");//定义一个form表单
                form.attr("style", "display:none");
                form.attr("target", "");
                form.attr("method", "post");
                form.attr("action", "/Verify/Export?serial=" + serial + "&type=" + "LY");
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

