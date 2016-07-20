var searchCondtiion = "";
//===================================Global=============================================//

$(function () {
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
    LoadInitData_Detail();

    $(window).resize(function () {
        var win_width = $(window).width();
        $("#TableList_0_1").datagrid('resize', { width: win_width});
    });
});

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

//根据输入查询条件查询
function SearchByCondition_right() {
    var jsonSC;

    //获取资产类型
    //var TypeAsset = $("#Accounting_SC_ZCTY").combobox("getValue");

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
            "TypeAsset": "all"
        }
    } else {
        //获取查询内容
        var contentSC = $("#SC_Content").val();
        jsonSC = {
            "typeFlag": "right",
            "DataType": "content",
            "dataName": valueSC,
            "contentSC": contentSC,
            "TypeAsset": "all"
        }
    }
    searchCondtiion = JSON.stringify(jsonSC);
    alert(searchCondtiion.toString());
    LoadInitData_Detail();
}
function resetSC() {
    $("#SC_Content").val("");
    $('#beginDate_SC').datebox('setValue', '');
    $('#endDate_SC').datebox('setValue', '');
}

function LoadInitData_Detail() {
 $('#TableList_0_1').datagrid({
              url: '/WXSearch/LoadAssets?searchCondtiion=' + searchCondtiion,
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
                    { field: 'ID', checkbox: false, width: 50 },
                    { field: 'serial_number', title: '资产编号', width: 50 },
                    { field: 'name_Asset', title: '资产名称', width: 50 },
                    { field: 'type_Asset', title: '资产类型', width: 50 },
                    { field: 'specification', title: '型号规范', width: 50 },
                    { field: 'unit_price', title: '单价', width: 50 },
                    { field: 'amount', title: '数量', width: 50 },
                    { field: 'department_Using', title: '使用部门', width: 50 },
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
                singleSelect: true, //允许选择多行
                selectOnCheck: true,//true勾选会选择行，false勾选不选择行, 1.3以后有此选项
                checkOnSelect: true //true选择行勾选，false选择行不勾选, 1.3以后有此选项
            });
}