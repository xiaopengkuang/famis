var searchCondtiion = "";

$(function () {
    LoadTreeLeft();
    LoadInitData_Detail();
    $(".SC_Date_Accounting").show();
    $(".SC_Content_Accounting").hide();
    $("#Accounting_SC").combobox({
        onChange: function (n, o) {
            //n 表示new  value
            //o 表示 old value
            if (n == "GZRQ" || n == "DJRQ") {
                $(".SC_Date_Accounting").show();
                $(".SC_Content_Accounting").hide();
            } else {
                $(".SC_Date_Accounting").hide();
                $(".SC_Content_Accounting").show();
            }
        }

    });

    $(window).resize(function () {
        var win_width = $(window).width();
        $("#TableList_0_1").datagrid('resize', { width: win_width});
    });
});



function LoadTreeLeft() {
    //获取查询条件
    $('#lefttree').tree({
        animate: true,
        checkbox: false,
        method: 'POST', //默认是post,不允许对静态文件访问
        url: '/Dict/loadSearchTreeByRole?treeType=Accounting',
        onClick: function (node) {

            var tree = $(this).tree;
            if (isRootNode(tree, node)) {
                // alert(node.id);
                SearchByCondition_LeftTree(node.id, "all");
            } else {
                // alert(node.id);
                SearchByCondition_LeftTree(node.id, node.text);
            }
        },
        onLoadSuccess: function (node, data) {
            $('#lefttree').show();
            $('#lefttree').tree('collapseAll');
        }
    });
}
//Left 左侧事件查询
function SearchByCondition_LeftTree(nodeID, nodetext) {
    var jsonSC = {
        "typeFlag": "left",
        "nodeID": nodeID,
        "nodeText": nodetext
    }
    searchCondtiion = JSON.stringify(jsonSC);
    LoadInitData_Detail();
}

function isRootNode(tree, node) {
    var parent = tree('getParent', node.target);
    if (parent.id != null) {
        return false;
    }
    return true;
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


//根据输入查询条件查询
function SearchByCondition_right() {
    var jsonSC;

    //获取资产类型
    var TypeAsset = $("#Accounting_SC_ZCTY").combobox("getValue");

    //获取查询类型：时间还是其他调价
    var valueSC = $("#Accounting_SC").combobox("getValue");
    if (valueSC == "GZRQ" || valueSC == "DJRQ") {
        //获取日期
        var beginDate = $('#beginDate_SC').datebox('getValue');
        var endDate = $('#endDate_SC').datebox('getValue');

        if (beginDate == null || beginDate == "" || endDate == null || endDate == "")
        {
            MessShow("请输入查询时间！");
            return;
        }
        //将字符串转换为日期
        var begin = new Date(beginDate.replace(/-/g, "/"));
        var end = new Date(endDate.replace(/-/g, "/"));
        //js判断日期
        if (begin - end > 0) {
            MessShow("开始日期要在截止日期之前!");
            return ;
        }


        jsonSC = {
            "typeFlag": "right",
            "DataType": "Date",
            "begin": beginDate,
            "dataName": valueSC,
            "end": endDate,
            "TypeAsset": TypeAsset
        }
    } else {
        //获取查询内容
        var contentSC = $("#SC_Content").val();
        jsonSC = {
            "typeFlag": "right",
            "DataType": "content",
            "dataName": valueSC,
            "contentSC": contentSC,
            "TypeAsset": TypeAsset
        }
    }
    searchCondtiion = JSON.stringify(jsonSC);
    LoadInitData_Detail();
}


function LoadInitData_Detail() {
    $('#TableList_0_1').datagrid({
        url: '/Asset/LoadAssets?tableType=1&searchCondtiion=' + searchCondtiion,
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
            { field: 'barcode', title: '条码编号', width: 50 },
            { field: 'supplierID', title: '供应商', width: 50 }
        ]],
        singleSelect: false, //允许选择多行
        selectOnCheck: true,//true勾选会选择行，false勾选不选择行, 1.3以后有此选项
        checkOnSelect: true //true选择行勾选，false选择行不勾选, 1.3以后有此选项
    });
    loadPageTool_Detail();
}



function loadPageTool_Detail() {
    var pager = $('#TableList_0_1').datagrid('getPager');	// get the pager of datagrid
    pager.pagination({
        buttons: [{
            text: '打印条形码',
            iconCls: 'icon-add',
            height: 50,
            handler: function () {
               
                //获取选中项
              var rows = $('#TableList_0_1').datagrid('getSelections');
              if (rows.length <1) {
                  MessShow("请选择1项数据！");
                  return;
              }
                var IDS = [];
                for (var i = 0; i < rows.length; i++) {
                   // alert(rows[i].ID);
                    myPreview3(rows[i].ID);
                } 

                var id = rows[0].ID;




                ////将数据传入后台
                //$.ajax({
                //    url: '/SysSetting/printBarcode',
                //    data: { "selectedIDs": IDS },
                //    //data: _list,  
                //    dataType: "json",
                //    type: "POST",
                //    traditional: true,
                //    success: function () {
                //        $('#TableList_0_1').datagrid('reload');
                //    }
                //});
            }
        },{
            text: '下载二维码（ZIP）',
            iconCls: 'icon-add',
            height: 50,
            handler: function () {
               
                //获取选中项
              var rows = $('#TableList_0_1').datagrid('getSelections');
                if (rows.length <1)
                {
                    MessShow("请选择1项数据！");
                    return;
                }
                var IDS = [];
                var idsStr = "";
                for (var i = 0; i < rows.length; i++) {
                    IDS[i] = rows[i].ID;
                    if (i == 0) {
                        idsStr = rows[i].ID;
                    } else {
                        idsStr =idsStr+"_"+ rows[i].ID;

                    }
                } 
                var id = rows[0].ID;
                exportData(idsStr);
            }
        }],
        beforePageText: '第',//页数文本框前显示的汉字  
        afterPageText: '页    共 {pages} 页',
        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录'
    });
}


function exportData(ids)
{
    var url = "/Download/download_qrcodes_zip?ids=" + ids;
    var form = $("<form>");//定义一个form表单
    form.attr("style", "display:none");
    form.attr("target", "hidden_frame");
    form.attr("method", "post");
    form.attr("action", url);
    var input1 = $("<input>");
    input1.attr("type", "hidden");
    input1.attr("name", "exportData");
    input1.attr("value", (new Date()).getMilliseconds());
    $("body").append(form);//将表单放置在web中
    form.append(input1);
    form.submit();//表单提交
}


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


//function buildEAN13() {
//    $.ajax({
//        url: "/BarCode/rebuiltarCode",
//        type: 'POST',
//        beforeSend: ajaxLoading,
//        success: function (data) {
//            ajaxLoadEnd();
//            if (data > 0) {
//            } else {
//                MessShow("服务器中已存在所有数据，无须重新生成！");
//            }


//        }
//    });
//}

function buildQRCode() {
    $.ajax({
        url: "/QrCode/rebuiltQRCODE",
        type: 'POST',
        beforeSend: ajaxLoading,
        success: function (data) {
            ajaxLoadEnd();
            if (data > 0) {
            } else {
                MessShow("服务器中已存在所有数据，无须重新生成！");
            }


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
