﻿//$("#Area_List_0").hide();
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
        $("#TableList_0_1").datagrid('resize', { width: win_width - 220 });
    });


});

function chanegeTableType_Radio() {

    var selectType = $("input[name='table_TYPE']:checked").val();

    if (selectType == "0") {
        LoadInitData_Summary();
    } else if (selectType == "1") {
        LoadInitData_Detail();
    } else {
    }

}


function reloadDataGrid()
{
    var selectType = $("input[name='table_TYPE']:checked").val();
    if (selectType == "0") {
        LoadInitData_Summary();
    } else if (selectType == "1") {
        LoadInitData_Detail();
    } else {
    }
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
    return y + '-' + (m < 10 ? '0' + m : m) + '-' + (d < 10 ? '0' + d : d) + ' ' + (h < 10 ? '0' + h : h) + ':' + (i < 10 ? '0' + i : i) + ':' + (s < 10 ? '0' + s : s);
}

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
           
            //}

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
    reloadTable_Condition();
}


function isRootNode(tree, node) {
    var parent = tree('getParent', node.target);
    if (parent.id != null) {
        return false;
    }
    return true;
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

        if (beginDate == null || beginDate == "" || endDate == null || endDate == "") {
            MessShow("请输入查询时间！");
            return;
        }
        //将字符串转换为日期
        var begin = new Date(beginDate.replace(/-/g, "/"));
        var end = new Date(endDate.replace(/-/g, "/"));
        //js判断日期
        if (begin - end > 0) {
            MessShow("开始日期要在截止日期之前!");
            return;
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
    reloadTable_Condition();
}


//表数据重载
function reloadTable_Condition() {
    
    //先判断类型
    var selectType = $("input[name='table_TYPE']:checked").val();
    if (selectType == "0") {
        LoadInitData_Summary();
    } else if (selectType == "1") {
        LoadInitData_Detail();
    } else {
    }
}

function resetSC() {
    $("#SC_Content").val("");
    $('#beginDate_SC').datebox('setValue', '');
    $('#endDate_SC').datebox('setValue', '');
}



function LoadInitData_Detail() {
    //获取操作权限
    $.ajax({
        url: '/Common/getOperationRightsByMenu?menu=ZCTZ',
        dataType: "json",
        type: "POST",
        traditional: true,
        success: function (dataRight) {
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
                    { field: 'specification', title: '型号规格', width: 50 },
                    {
                        field: 'unit_price', title: '单价', width: 50,
                        formatter: function (data) {
                            return StringToNUM(data);
                        }
                    },
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
                    { field: 'Method_decrease', title: '减少方式', width: 50 },
                    {
                        field: 'id_viewPIC', title: '图片预览', width: 50,
                        formatter: function (data,row,index) {
                            var btn = "<a  onclick='preViewImg(" + row.ID + ")' href='javascript:void(0);' >预览</a>";
                            return btn;
                        }
                    },
                    { field: 'note', title: '备注', width: 50 }
                ]],
                singleSelect: false, //允许选择多行
                selectOnCheck: true,//true勾选会选择行，false勾选不选择行, 1.3以后有此选项
                checkOnSelect: true //true选择行勾选，false选择行不勾选, 1.3以后有此选项
            });
            loadPageTool_Detail(dataRight);
        }
    });

}

function preViewImg(id)
{
    var url = "/Asset/Asset_Sub_PIC_PreView?id="+id;
    var titleName = "图片预览"
    openModelWindow(url, titleName);
}

function loadPageTool_Detail(dataRight) {
    //alert(dataRight.add);

    var pager = $('#TableList_0_1').datagrid('getPager');	// get the pager of datagrid
    pager.pagination({
        buttons: [{
            text: '添加',
            iconCls: 'icon-add',
            height: 50,
            disabled: !dataRight.add_able,
            handler: function () {
                if (!dataRight.add_able) {
                    return;
                }
                openModelWindow("/Asset/Asset_add", "添加资产");
            }
        }, {
            text: 'Excel导入',
            iconCls: 'icon-add',
            height: 50,
            disabled: !dataRight.add_able,
            handler: function () {
                if (!dataRight.add_able)
                {
                    return;
                }
              openModelWindow("/Asset/Asset_addByExcel", "批量添加资产");
            }
        }, {
            text: '图片批量导入',
            iconCls: 'icon-add',
            height: 50,
            disabled: !dataRight.add_able,
            handler: function () {
                if (!dataRight.add_able) {
                    return;
                }
                openModelWindow("/Asset/Asset_picsUpload", "批量添加图片");
            }
        }, {
            text: '编辑',
            iconCls: 'icon-edit',
            height: 50,
            disabled: !dataRight.edit_able,
            handler: function () {
                if (!dataRight.edit_able)
                {
                    return;
                }
                //获取选择行
                var rows = $('#TableList_0_1').datagrid('getSelections');
                if (rows.length != 1)
                {
                    MessShow("请选择1项数据进行编辑！");
                    return;
                }
                var id_asset = rows[0].ID;
                var url = "/Asset/Asset_edit?id="+id_asset;
                var titleName = "编辑资产";
                openModelWindow(url, titleName);
            }
        }
        , {
            text: '删除',
            iconCls: 'icon-remove',
            height: 50,
            disabled: !dataRight.delete_able,
            handler: function () {
                if (!dataRight.delete_able)
                {
                    return;
                }

                //获取选择行
                var rows = $('#TableList_0_1').datagrid('getSelections');
                var IDS = [];
                for (var i = 0; i < rows.length; i++) {
                    IDS[i] = rows[i].ID;
                }
                //将数据传入后台
                $.ajax({
                    url: '/Asset/deleteAssets',
                    data: { "selectedIDs": IDS },
                    //data: _list,  
                    dataType: "json",
                    type: "POST",
                    traditional: true,
                    success: function () {
                        $('#TableList_0_1').datagrid('reload');
                    }
                });
            }
        }
        , {
            text: '刷新',
            height: 50,
            iconCls: 'icon-reload',
            handler: function () {
                $('#TableList_0_1').datagrid('reload');
            }
        }, {
            text: '导出',
            height: 50,
            disabled:!dataRight.export_able,
            iconCls: 'icon-save',
            handler: function () {
                if (!dataRight.export_able)
                {
                    return;
                }
                ajaxLoading();
                var url = '/ExportExcel/ExportExcel_Asset_Accounting?exportFlag=true&tableType=1&searchCondtiion=' + searchCondtiion;
                //'/Collar/LoadCollars?searchCondtiion=' + searchCondtiion
                exportData(url);
                ajaxLoadEnd();
                //var filename = getNowFormatDate_FileName();

                //Export(filename, $('#TableList_0_1'));
            }
        }],
        beforePageText: '第',//页数文本框前显示的汉字  
        afterPageText: '页    共 {pages} 页',
        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录'
    });
}
function exportData(url) {
    var form = $("<form>");//定义一个form表单
    form.attr("style", "display:none");
    form.attr("target", "");
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
function getNowFormatDate_FileName() {
    var date = new Date();
    var seperator1 = "";
    var seperator2 = "";
    var month = date.getMonth() + 1;
    var strDate = date.getDate();
    if (month >= 1 && month <= 9) {
        month = "0" + month;
    }
    if (strDate >= 0 && strDate <= 9) {
        strDate = "0" + strDate;
    }
    var currentdate = date.getFullYear() + seperator1 + month + seperator1 + strDate
            + "" + date.getHours() + seperator2 + date.getMinutes()
            + seperator2 + date.getSeconds();
    return currentdate;
}

function loadPageTool_Summary(dataRight) {

    //var editFlag = dataRight.edit == false ? true : false;
    //var exportFlag = dataRight.export == false ? true : false;

    var pager = $('#TableList_0_1').datagrid('getPager');	// get the pager of datagrid
    pager.pagination({
        buttons: [
        //    {
        //    text: '批量修改',
        //    iconCls: 'icon-edit',
        //    height: 50,
        //    disabled: !dataRight.edit_able,
        //    handler: function () {
        //        if (!dataright.edit_able)
        //        {
        //            return;
        //        }
        //        //选择的的资产改名
        //    }
        //},
        {
            text: '导出',
            iconCls: 'icon-save',
            height: 50,
            disabled: !dataRight.export_able,
            handler: function () {
                if (!dataRight.export_able)
                {
                    return;
                }
                ajaxLoading();
                var url = '/ExportExcel/ExportExcel_Asset_Accounting?exportFlag=true&tableType=0&searchCondtiion=' + searchCondtiion;
                //'/Collar/LoadCollars?searchCondtiion=' + searchCondtiion
                exportData(url);
                ajaxLoadEnd();
                ////将要选择的数据导出到Excel
                //var filename = getNowFormatDate_FileName();
                //Export(filename, $('#TableList_0_1'));
            }
        }],
        beforePageText: '第',//页数文本框前显示的汉字  
        afterPageText: '页    共 {pages} 页',
        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录'
    });
}


function LoadInitData_Summary() {

    //获取操作权限
    $.ajax({
        url: '/Common/getOperationRightsByMenu?menu=ZCTZ',
        dataType: "json",
        type: "POST",
        traditional: true,
        success: function (dataRight) {

        $('#TableList_0_1').datagrid({
            url: '/Asset/LoadAssets?tableType=0&searchCondtiion=' + searchCondtiion,
            method: 'POST', //默认是post,不允许对静态文件访问
            width: 'auto',
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
                { field: 'RowNo', checkbox: true, width: 50 },
                { field: 'AssetName', title: '资产名称', width: 50 },
                { field: 'AssetType', title: '资产类型', width: 50 },
                { field: 'specification', title: '型号规格', width: 50 },
                { field: 'measurement', title: '计量单位', width: 50 },
                { field: 'amount', title: '数量', width: 50 },
                {
                    field: 'value', title: '资产价值', width: 50,
                    formatter: function (data) {
                        return StringToNUM(data);
                    }
                }
            ]],
            singleSelect: false, //允许选择多行
            selectOnCheck: true,//true勾选会选择行，false勾选不选择行, 1.3以后有此选项
            checkOnSelect: true //true选择行勾选，false选择行不勾选, 1.3以后有此选项
        });
        loadPageTool_Summary(dataRight);
        }
    });
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



function openModelWindow(url, titleName) {
    //获取当前页面的Width和高度
    var winWidth = (document.body.clientWidth - 20) < 0 ? 0 : (document.body.clientWidth - 20);
    var winheight = (document.body.clientHeight - 20) < 0 ? 0 : (document.body.clientHeight - 20);

    
    try{
        $("#modalwindow").window("close");
    }catch(e){}
    var $winADD;
    $winADD = $('#modalwindow').window({
        title: titleName,
        width: winWidth,
        height:winheight,
        left: 10,
        top: 10,
        //top: (($(window).height() - 650) > 0 ? ($(window).height() - 650) : 650) * 0.5,
        //left: (($(window).width() - 1028) > 0 ? ($(window).width() - 1028) : 100) * 0.5,
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

function StringToNUM(datainfo)
{

    return datainfo;
    if (datainfo == null || datainfo == "") {
        return "";
    } else {
        var numNew = new Number(datainfo);
        return numNew;
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
