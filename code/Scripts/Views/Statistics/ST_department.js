//=======================================全局数据==========================================//
var searchCondition="";
//=======================================全局数据==========================================//


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

//得到当前日期
formatterDate = function (date) {
    var day = date.getDate() > 9 ? date.getDate() : "0" + date.getDate();
    var month = (date.getMonth() + 1) > 9 ? (date.getMonth() + 1) : "0"
    + (date.getMonth() + 1);
    return date.getFullYear() + '-' + month + '-' + day;
};

formatterDate_begin = function (date) {
   
    return date.getFullYear() + '-01-01';
};
//=======================================================================================================//
//初始化数据：
$(function () {
    $('#beginDate_SC').datebox('setValue', formatterDate_begin(new Date()));
    $('#endDate_SC').datebox('setValue', formatterDate(new Date()));

    jsonSC = {
        "orderType": "department"
    }
    searchCondition = JSON.stringify(jsonSC);
    loadInitData();
});
function loadInitData()
{
    load_ZCLB_add();
    load_GYS_add();
    load_TYPE_ASSETTYPE();
    LoadInitData_Detail();
}

function clearALL()
{
    clearAssetType();
    clearSupplier();
}


function clearAssetType()
{
    $('#AssetType_ST').combotree("clear");
}
function clearSupplier() {
    $('#supplier_ST').combogrid("clear");
}
function load_GYS_add() {
    $('#supplier_ST').combogrid({
        panelWidth: 300,
        value: '006',
        idField: 'code',
        editable: false,
        textField: 'name',
        url: '/Dict/load_GYS_add',
        method: 'POST', //默认是post,不允许对静态文件访问
        columns: [[
        { field: 'ID', checkbox: true, title: 'ID', width: 99, hidden: true },
        { field: 'name_supplier', title: '供应商', width: 99 },
        { field: 'linkman', title: '联系人', width: 99 },
        { field: 'address', title: '地址', width: 99 }
        ]],
        onClickRow: function (index, row) {
            //search = false;
            $('#supplier_ST').combogrid('hidePanel');
            $('#supplier_ST').combogrid('setValue', row.ID);
            $('#supplier_ST').combogrid('setText', row.name_supplier);
        },
        onLoadSuccess: function () {
        }

    });
}
function load_ZCLB_add() {
    $('#AssetType_ST').combotree
   ({
       url: '/Dict/load_ZCLB',
       valueField: 'id',
       textField: 'nameText',
       //required: true,
       method: 'POST',
       editable: false,
       //选择树节点触发事件  
       onSelect: function (node) {
         
       }, //全部折叠
       onLoadSuccess: function (node, data) {
       }
   });
}
function load_TYPE_ASSETTYPE() {
    $("#ATAttr_ST").combobox({
        valueField: 'ID',
        method: 'POST',
        editable: false,
        textField: 'name_para',
        url: '/Dict/load_Type_AssetType',
        onSelect: function (rec) {
        },
        onLoadSuccess: function () {
            var data = $('#ATAttr_ST').combobox('getData');
            if (data.length > 0) {
                $('#ATAttr_ST').combobox('select', data[0].ID);
            }
        }
    });
}

function chanegeTableType_Radio() {

    var selectType = $("input[name='table_TYPE']:checked").val();

    if (selectType == "0") {
        LoadInitData_Summary();
    } else if (selectType == "1") {
        LoadInitData_Detail();
    } else {
    }

}
//=======================================================================================================//
//======================================加载datagrid数据=================================================================//
function LoadInitData_Summary()
{
    $('#datagrid_ST_DE').datagrid({
        url: '/Statistics/LoadAssets?tableType=0&searchCondtiion=' + searchCondition,
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
            { field: 'RowNo', checkbox: false,hidden:true, width: 50 },
            { field: 'department_Using', title: '使用部门', width: 50 },
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
}
function LoadInitData_Detail()
{
    $('#datagrid_ST_DE').datagrid({
        url: '/Statistics/LoadAssets?tableType=1&searchCondtiion=' + searchCondition,
        method: 'POST', //默认是post,不允许对静态文件访问
        width: 'auto',
        height: '600px',
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
            { field: 'ID', checkbox: false, hidden: true, width: 50 },
            { field: 'department_Using', title: '使用部门', width: 50 },
            { field: 'type_Asset', title: '资产类别', width: 50 },
            { field: 'serial_number', title: '资产编号', width: 50 },
            { field: 'name_Asset', title: '资产名称', width: 50 },
            { field: 'specification', title: '型号规格', width: 50 },
            //{
            //    field: 'unit_price', title: '单价', width: 50,
            //    formatter: function (data) {
            //        return StringToNUM(data);
            //    }
            //},
            { field: 'amount', title: '数量', width: 50 },
            //{ field: 'addressCF', title: '地址', width: 50 },
            //{ field: 'Method_add', title: '添加方式', width: 50 },
            //{
            //    field: 'state_asset', title: '资产状态', width: 50,
            //    formatter: function (data) {
            //        if (data == "在用") {
            //            return '<font color="#696969">' + data + '</font>';
            //        }
            //        else if (data == "借出") {
            //            return '<font color="#FFD700">' + data + '</font>';
            //        } else if (data == "闲置") {
            //            return '<font color="#228B22">' + data + '</font>';
            //        } else if (data == "报废") {
            //            return '<font color="red">' + data + '</font>';
            //        } else {
            //            return data;
            //        }
            //    }
            //},
            { field: 'supplierID', title: '供应商', width: 50 },
            { field: 'note', title: '备注', width: 50 }
        ]],
        singleSelect: true, //允许选择多行
        selectOnCheck: true,//true勾选会选择行，false勾选不选择行, 1.3以后有此选项
        checkOnSelect: true //true选择行勾选，false选择行不勾选, 1.3以后有此选项
    });
}
function StringToNUM(datainfo) {

    return datainfo;
    if (datainfo == null || datainfo == "") {
        return "";
    } else {
        var numNew = new Number(datainfo);
        return numNew;
    }
}
//======================================加载datagrid数据=================================================================//


function doSearch()
{
    //获取日期类型
    var Type_date = $("#dateType").combobox("getValue");
    //获取日期值
    //获取日期
    var beginDate = $('#beginDate_SC').datebox('getValue');
    var endDate = $('#endDate_SC').datebox('getValue');

    if (beginDate == null || beginDate == "" || endDate == null || endDate == "")
    {
        MessShow("起始时间/结束时间不可空！");
        return;
    }

    //获取资产类型
    var ID_TypeAsset = $("#AssetType_ST").combotree("getValue");
    //获取资产性质
    var IDTYPE_TypeAsset = $("#ATAttr_ST").combobox("getValue");
    //获取供应商
    var ID_supplier = $("#supplier_ST").combogrid("getValue");
    var ID_supplier_TEXT = $("#supplier_ST").combogrid("getValue");


    jsonSC = {
        "dateType": Type_date,
        "beginDate": beginDate,
        "endDate": endDate,
        "orderType": "department",
        "Type_AssetType": IDTYPE_TypeAsset,
        "supplier": ID_supplier,
        "AssetType": ID_TypeAsset
    }
    searchCondition = JSON.stringify(jsonSC);
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
