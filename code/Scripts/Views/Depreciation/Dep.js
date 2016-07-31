
var searchCondtiion = "0";
//alert(searchCondtiion+"55568");
function depreciation() {

    $.ajax({

        type: "post",
        url: "/Depreciation/Dep_JT",

        datatype: "json",//数据类型

        success: function (result) {

            $('#p').show();
            start();

        }, error: function (msg) {
            alert("无资产信息或资产信息有误!");
            return null;
        }
    });


   
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
function start() {



    var value = $('#p').progressbar('getValue');
    if (value < 100) {
        value += Math.floor(Math.random() * 10);
        $('#p').progressbar('setValue', value);
        setTimeout(arguments.callee, 200);

        // $('#p').progressbar('virualble')
    }
    if (value == 100) {

        MessShow("折旧完成！");
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
        method: 'POST', //默认是post,不允许对静态文件访问
        url: '/Dict/loadSearchTreeByRole?treeType=DepreciationSearch',
        onClick: function (node) {
            var tree = $(this).tree;
            //选中的节点是否为叶子节点,如果不是叶子节点,清除选中  

            searchCondtiion = node.id;
           // alert(searchCondtiion);
            LoadInitData_Detail(searchCondtiion);

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
    //获取操作权限
    $.ajax({
        url: '/Common/getOperationRightsByMenu?menu=ZJGL',
        dataType: "json",
        type: "POST",
        traditional: true,
        success: function (dataRight) {
    //  alert("查询条件是：---" + searchCondtiion);
    $('#TableList_0_1').datagrid({
        url: '/Depreciation/Load_Asset?JSdata=' + searchCondtiion + '', //+ , 
        //  url: '/SysSetting/getpageOrder?role=1&tableType=1',
        method: 'post', //默认是post,不允许对静态文件访问
        width: 'auto',
        height: '300px',
    
         fit:true ,
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
            {
                field: 'unit_price', title: '单价', width: 40,
                formatter: function (money) {

                    if (String(money).split(".").length < 2)
                        return money + "¥";
                    else {
                        var arr = String(money).split(".");
                        return arr[0] + "." + arr[1].substring(0, 2) + "¥"
                    }

                }
            },
            { field: 'amount', title: '数量', width: 40 },
             {
                 field: 'Total_price', title: '资产总价', width: 50,
                 formatter: function (money) {

                     if (String(money).split(".").length < 2)
                         return money + "¥";
                     else {
                         var arr = String(money).split(".");
                         return arr[0] + "." + arr[1].substring(0, 2) + "¥"
                     }

                 }
             },
            { field: 'Method_depreciation', title: '折旧方式', width: 50 },
            { field: 'YearService_month', title: '使用年限（月）', width: 50 },
            {
                field: 'Net_residual_rate', title: '净残值率', width: 30,
                formatter: function (rate) {
                    return rate + "%";


                }
            },
            {
                field: 'depreciation_Month', title: '月提折旧', width: 50,
                formatter: function (money) {
                    if (money == null)
                        return "0¥";
                    if (String(money).split(".").length < 2)
                        return money + "¥";
                    else {
                        var arr = String(money).split(".");
                        return arr[0] + "." + arr[1].substring(0, 2) + "¥"
                    }

                }
            },
             {
                 field: 'depreciation_tatol', title: '累计折旧', width: 50,
                 formatter: function (money) {
                     if (money == null)
                         return "0¥";
                     if (String(money).split(".").length < 2)
                         return money + "¥";
                     else {
                         var arr = String(money).split(".");
                         return arr[0] + "." + arr[1].substring(0, 2) + "¥"
                     }

                 }
             },
             {
                 field: 'Net_value', title: '净值', width: 50,
                 formatter: function (money) {
                     if (money == null)
                         return "0¥";
                     if (String(money).split(".").length < 2)
                         return money + "¥";
                     else {
                         var arr = String(money).split(".");
                         return arr[0] + "." + arr[1].substring(0, 2) + "¥"
                     }

                 }
             },
              {
                  field: 'Time_Purchase', title: '购置日期', width: 80,

                  formatter: function (date) {
                      var pa = /.*\((.*)\)/;
                      var unixtime = date.match(pa)[1].substring(0, 10);
                      return getTime(unixtime);
                  }
              },


             {
                 field: 'Time_add', title: '登记日期', width: 80,
                 formatter: function (date) {
                     var pa = /.*\((.*)\)/;
                     var unixtime = date.match(pa)[1].substring(0, 10);
                     return getTime(unixtime);
                 }

             }

        ]],
        singleSelect: false, //允许选择多行
        selectOnCheck: true,//true勾选会选择行，false勾选不选择行, 1.3以后有此选项
        checkOnSelect: true, //true选择行勾选，false选择行不勾选, 1.3以后有此选项
        fitColumns: true
    });
    loadPageTool_Detail(dataRight);
  //  alert(dataRight.export_able);
}
});
}

function loadPageTool_Detail(dataRight) {
    var pager = $('#TableList_0_1').datagrid('getPager');	// get the pager of datagrid
    pager.pagination({
        buttons: [{
            text: '导出',
            height: 50,
            iconCls: 'icon-save',
            disabled: !dataRight.export_able,
            handler: function () {
                if (!dataRight.export_able) {
                    return;
                }

                var form = $("<form>");//定义一个form表单
                form.attr("style", "display:none");
                form.attr("target", "");
                form.attr("method", "post");
                form.attr("action", "/Verify/ExportStu2?JSdata= " + searchCondtiion + "");
              //  form.attr("multipart","multypart/form-data")
                var input1 = $("<input>");
                input1.attr("type", "hidden");
                input1.attr("name", "exportData");
                input1.attr("value", (new Date()).getMilliseconds());
                $("body").append(form);//将表单放置在web中
                form.append(input1);
                form.submit();//表单提交

                // @Html.ActionLink("点击导出Excel", "ExportStu2")
              /*  $.ajax({

                    type: "POST",
                    url: "/Verify/ExportStu2",


                    datatype: "json",

                    success: function (result) {



                    }, error: function (msg) {
                        alert("导出失败！");
                    }
                });*/
               /* if (!dataRight.export_able) {
                    return;
                }
                var filename = getNowFormatDate_FileName();

                Export(filename, $('#TableList_0_1'));*/
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