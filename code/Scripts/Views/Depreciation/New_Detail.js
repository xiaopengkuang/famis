var searchCondtiion = "1o2SYBMoasd";

function depreciation() {

    $.ajax({

        type: "post",
        url: "/AssetDeatails/LoadAsset",

        datatype: "json",//数据类型

        success: function (result) {


            alert("折旧开始执行！");


        }, error: function (msg) {
            alert("折旧失败!");
        }
    });


    $('#p').show();
    start();
}
function test() {

    $.ajax({

        type: "post",
        url: "/Depreciation/ReadExcel?path=D:\\test.xlsx",

        datatype: "json",//数据类型

        success: function (result) {
            // excel = eval('(' + result + ')');
            var buffer = result.split("*");
            for (var i = 0; i < buffer.length; i++) {
                excel = eval('(' + buffer[i] + ')');
                $.each(excel, function (name, value) {

                    alert(name + " ---> " + value);

                });
            }

        }, error: function (msg) {
            alert("折旧失败!");
        }
    });


    $('#p').show();
    start();
}
function start() {



    var value = $('#p').progressbar('getValue');
    if (value < 100) {
        value += Math.floor(Math.random() * 10);
        $('#p').progressbar('setValue', value);
        setTimeout(arguments.callee, 200);

        // $('#p').progressbar('virualble')
    }
    if (value == 100) {


        alert("折旧完成！");
        $('#p').hide();
        $('#p').progressbar('setValue', 0);
        LoadInitData_Detail(searchCondtiion);
        return null;

    }
};

$(function () {
    LoadTreeLeft();
    LoadInitData_Detail(searchCondtiion);
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
        method: 'get', //默认是post,不允许对静态文件访问
        url: '/SysSetting/loadDPByRole?role=1',
        onClick: function (node) {
            var tree = $(this).tree;
            //选中的节点是否为叶子节点,如果不是叶子节点,清除选中  
            
            var nodes = $('#mtree').tree('getChecked');
            searchCondtiion = "1" + "o" + "2_SYBM" + "o" + node.id;
            // alert(searchCondtiion);
            LoadInitData_Detail(String(searchCondtiion));

        },
        onLoadSuccess: function (node, data) {
            $('#lefttree').show();
            $('#lefttree').tree('collapseAll');
            $('#bottomtree').tree('collapseAll');
        }
    });
    $('#righttree').tree({
        animate: true,
        checkbox: false,
        method: 'get', //默认是post,不允许对静态文件访问
        url: '/SysSetting/loadATByRole?role=1',
        onClick: function (node) {
            var tree = $(this).tree;
            //选中的节点是否为叶子节点,如果不是叶子节点,清除选中  
            // alert(node.text);
            searchCondtiion = "1" + "o" + "2_ZCLB" + "o" + node.id;
           //  alert(searchCondtiion);
             LoadInitData_Detail(String(searchCondtiion));

        },
        onLoadSuccess: function (node, data) {
            $('#righttree').show();
            $('#righttree').tree('collapseAll');
        }
    });
    $('#bottomtree').tree({
        onClick: function (node) {
            var tree = $(this).tree;
            //选中的节点是否为叶子节点,如果不是叶子节点,清除选中  
            // alert(node.text);
            searchCondtiion = "1" + "o" + "2_ZCZT" + "o" + node.id;
            // alert(searchCondtiion);
             LoadInitData_Detail(String(searchCondtiion));

        },

    });
}
function Add_Invention_Deatails()
{
    $.ajax({

        type: "post",
        url: "/AssetDeatails/Add_InventDeatails?Json="+searchCondtiion+"",

        datatype: "json",//数据类型

        success: function (result) {
          
            alert("添加明细成功！");


        }, error: function (msg) {
            alert("添加明细失败！");
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
    //alert(searchCondtiion);
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



function LoadInitData_Detail(searchCondtiion) {
    //  alert("查询条件是：---" + searchCondtiion);
    $('#TableList_0_1').datagrid({
        url: '/AssetDeatails/Load_Asset?JSdata=' + searchCondtiion + '', //+ , 
        //  url: '/SysSetting/getpageOrder?role=1&tableType=1',
        method: 'post', //默认是post,不允许对静态文件访问
        width: 'auto',
        iconCls: 'icon-save',
        dataType: "json",
        //  fitColumns: true,
        rownumbers: true, //是否加行号
        pagination: true, //是否显式分页
        // onClickCell: onClickCell,
        // onEndEdit: onEndEdit,
        // height:500,
        pageSize: 15, //页容量，必须和pageList对应起来，否则会报错
        pageNumber: 1, //默认显示第几页
        pageList: [15, 30, 45],
        
         
        
       columns: [[
          { field: 'ID', title: '序号', width: 50 },
           
            { field: 'serial_number', title: '资产编号', width: 250 },
            { field: 'type_Asset', title: '资产类别', width: 150 },
            { field: 'name_Asset', title: '资产名称', width: 150 },
            { field: 'specification', title: '规格型号', width: 150 },
            { field: 'measurement', title: '计量单位', width: 150 },
            { field: 'unit_price', title: '单价', width: 150 },
            { field: 'amount', title: '数量', width: 150 },
            { field: 'Total_price', title: '总价', width: 250 },
            { field: 'department_using', title: '使用部门', width: 150 },
            { field: 'address', title: '存放地址', width: 250 },
            { field: 'owener', title: '使用人', width: 150 },
            { field: 'state_asset', title: '资产状态', width: 150 },
            { field: 'supllier', title: '供应商', width: 250 }

       ]],

        
        onDblClickRow: function (index, field, value) {
           // var row = $('#TableList_0_1').datagrid('getRowIndex', index);
            var rowdata = $('#TableList_0_1').datagrid('getData');
            var searial = rowdata.rows[index].serial_number;
            $.ajax({

                type: "post",
                url: "/Depreciation/Set_SearialNum?Json="+searial+"",

                datatype: "json",//数据类型

                success: function (result) {
                    // excel = eval('(' + result + ')');
                  
                   

                }, error: function (msg) {
                    alert("未能传出资产编号的值!");
                }
            });
            Next_Page();
           
        }
        
    });
  
  //  loadPageTool_Detail();
}
function Next_Page() {
    var $winADD;
    $winADD = $('#modalwindow').window({
        title: '明细预览',
        width: 1760,
        height: 940,
        top: 0,
        left:0,
        modal: true,
        iconCls: 'icon-add',
        closed: true,
        minimizable: false,
        maximizable: false,
        collapsible: false,
        onClose: function () {
         
      
        }
    });
    $("#modalwindow").html("<iframe width='100%' height='99%' scrolling='yes' name='ghrzFrame' frameborder='0' src='/Verify/Deatails_View'></iframe>");
    $winADD.window('open');
}
function loadPageTool_Detail() {
    var pager = $('#TableList_0_1').datagrid('getPager');	// get the pager of datagrid
    pager.pagination({
        buttons: [{
            text: '导出',
            height: 50,
            iconCls: 'icon-save',
            handler: function () {
                var filename = getNowFormatDate_FileName();

                Export(filename, $('#TableList_0_1'));
            }
        }],
        beforePageText: '第',//页数文本框前显示的汉字  
        afterPageText: '页    共 {pages} 页',
        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录'
    });
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




function myformatter(date) {
    var y = date.getFullYear();
    var m = date.getMonth() + 1;
    var d = date.getDate();
    return y + '-' + (m < 10 ? ('0' + m) : m) + '-' + (d < 10 ? ('0' + d) : d);
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