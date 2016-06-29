var searchCondtiion = "1o使用部门o销售部";
 
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
         
            searchCondtiion = "1"+"o"+"使用部门"+"o"+node.text;
            LoadInitData_Detail();

        },
        onLoadSuccess: function (node, data) {
            $('#lefttree').show();
            $('#lefttree').tree('collapseAll');
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
            searchCondtiion = "1" + "o" + "资产类别" + "o" + node.text;
            LoadInitData_Detail();

        },
        onLoadSuccess: function (node, data) {
            $('#righttree').show();
            $('#righttree').tree('collapseAll');
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



function LoadInitData_Detail() {
  //  alert("查询条件是：---" + searchCondtiion);
    $('#TableList_0_1').datagrid({
        url: '/Depreciation/Load_Asset?JSdata='+searchCondtiion+'', //+ , 
      //  url: '/SysSetting/getpageOrder?role=1&tableType=1',
        method: 'post', //默认是post,不允许对静态文件访问
        width: 'auto',
        height: '300px',
       // fit:true ,
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
            { field: 'ID', title: '序号', width: 50 },
            { field: 'department_Using', title: '使用部门', width: 50 },
            { field: 'serial_number', title: '资产编号', width: 80 },
            { field: 'name_Asset', title: '资产名称', width: 50 },
           
            { field: 'specification', title: '型号规范', width: 50 },
            { field: 'unit_price', title: '单价', width: 50 },
            { field: 'amount', title: '数量', width: 50 },
             { field: 'Total_price', title: '资产总价', width: 50 },
            { field: 'Method_depreciation', title: '折旧方式', width: 50 },
            { field: 'YearService_month', title: '使用年限（月）', width: 50 },
            { field: 'Net_residual_rate', title: '净残值率', width: 50 },
            { field: 'depreciation_Month', title: '月提折旧', width: 50 },
             { field: 'depreciation_tatol', title: '累计折旧', width: 50 },
             { field: 'Net_value', title: '净值', width: 50 },
              {
                  field: 'Time_Purchase', title: '购置日期', width: 50,
                   
                  formatter: function (date) {
                      getNowFormatDate_FileName(date)
              }
              },


             {
                 field: 'Time_add', title: '登记日期', width: 50,
                 
                 formatter: function (date) {
                     getNowFormatDate_FileName(date)
    } 
             }
          
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