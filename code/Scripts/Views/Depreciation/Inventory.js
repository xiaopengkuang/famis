var searchCondtiion = "1o使用部门o销售部";
//alert("asdaddsd");
$(document).ready(function () {
    //多选框


    $("#operator").combobox({
        valueField: 'ID',
        method: 'get',
        textField: 'name',

        url: '/Sttaff/load_Operator',
        onSelect: function (rec) {

            $('#operator').combobox('setValue', rec.ID);
            $('#operator').combobox('setText', rec.name);


        }
    });
    LoadInitData(searchCondtiion);
    LoadInitData_Detail(searchCondtiion);
  

});
function LoadInitData(searchCondtiion) {
    //  alert("查询条件是：---" + searchCondtiion);
    $('#TableList_0_1').datagrid({
        url: '/Depreciation/Load_Asset?JSdata=' + searchCondtiion + '', //+ , 
        //  url: '/SysSetting/getpageOrder?role=1&tableType=1',
        method: 'post', //默认是post,不允许对静态文件访问
        width: 'auto',
        height: '300px',
        fitColumn: true,
        // fit:true ,
        iconCls: 'icon-save',
        dataType: "json",

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
    loadPageTool();
}

function loadPageTool() {
    var pager = $('#TableList_0_1').datagrid('getPager');	// get the pager of datagrid
    pager.pagination({
        buttons: [{
            text: '添加',
            iconCls: 'icon-add',
            height: 50,
            handler: function () {
                var $winADD;
                $winADD = $('#modalwindow').window({
                    title: '添加资产',
                    width: 860,
                    height: 540,
                    top: (($(window).height() - 800) > 0 ? ($(window).height() - 800) : 200) * 0.5,
                    left: (($(window).width() - 500) > 0 ? ($(window).width() - 500) : 100) * 0.5,
                    shadow: true,
                    modal: true,
                    iconCls: 'icon-add',
                    closed: true,
                    minimizable: false,
                    maximizable: false,
                    collapsible: false,
                    onClose: function () {
                        $('#TableList_0_1').datagrid('reload');
                        //    var resultAlert = "成功插入记录！";
                        //    $.messager.show({
                        //        title: '提示',
                        //        msg: resultAlert,
                        //        showType: 'slide',
                        //        style: {
                        //            right: '',
                        //            top: document.body.scrollTop + document.documentElement.scrollTop,
                        //            bottom: ''
                        //        }
                        //    });
                    }
                });
                $("#modalwindow").html("<iframe width='100%' height='99%'  frameborder='0' src='/Asset/AddAsset'></iframe>");
                $winADD.window('open');
            }
        }, {
            text: '删除',
            iconCls: 'icon-remove',
            height: 50,
            handler: function () {
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
                        //var resultAlert = "成功删除记录！";
                        //$.messager.show({
                        //    title: '提示',
                        //    msg: resultAlert,
                        //    showType: 'slide',
                        //    style: {
                        //        right: '',
                        //        top: document.body.scrollTop + document.documentElement.scrollTop,
                        //        bottom: ''
                        //    }
                        //});
                    }
                });
            }
        }, {
            text: '刷新',
            height: 50,
            iconCls: 'icon-reload',
            handler: function () {
                $('#TableList_0_1').datagrid('reload');
                //alert('刷新');
            }
        }, {
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
function LoadInitData_Detail(searchCondtiion) {
    //  alert("查询条件是：---" + searchCondtiion);
    $('#TableList_0_2').datagrid({
        url: '/Depreciation/Load_Asset?JSdata=' + searchCondtiion + '', //+ , 
        //  url: '/SysSetting/getpageOrder?role=1&tableType=1',
        method: 'post', //默认是post,不允许对静态文件访问
        width: 'auto',
        height: '300px',
        fitColumn: true,
        // fit:true ,
        iconCls: 'icon-save',
        dataType: "json",

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
    loadPageTool_Detail();
}

function loadPageTool_Detail() {
    var pager = $('#TableList_0_2').datagrid('getPager');	// get the pager of datagrid
    pager.pagination({
        buttons: [{
            text: '新增明细',
            height: 50,
            iconCls: 'icon-add',
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