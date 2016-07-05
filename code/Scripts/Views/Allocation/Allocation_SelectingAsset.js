//====================================Global Data==================================//
var CurrentList = new Array();  //当前添加的数据
var searchCondtiion = "";
//====================================Global Data==================================//



//====================================Init Data==================================//
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
    loadInitData();
});

function loadInitData()
{
    load_SC_Tree();
    
    LoadInitData_Detail();
}





function load_SC_Tree() {

    //获取查询条件
    $('#lefttree').tree({
        animate: true,
        checkbox: false,
        method: 'POST', //默认是post,不允许对静态文件访问
        url: '/Dict/loadSearchTreeByRole?treeType=allocationSearch',
        onClick: function (node) {
            var tree = $(this).tree;
            //选中的节点是否为叶子节点,如果不是叶子节点,清除选中  
            SearchByCondition_LeftTree(node.id, node.text);
        },
        onLoadSuccess: function (node, data) {
            $('#lefttree').show();
            $('#lefttree').tree("collapseAll");
        }
    });
}


function LoadInitData_Detail() {

    var _list = "";
    try{
    
        for (var i = 0; i < parent.CurrentList.length; i++) {
            if (i == 0) {
                _list = parent.CurrentList[i]+"";
            } else {
                _list =_list+ "_"+parent.CurrentList[i];
            }
        }
    }catch(e){
    }

    $('#tableList_collar').datagrid({
        url: '/Common/LoadAsset_ByState?stateID=2&searchCondtiion=' + searchCondtiion + "&selectedIDs=" + _list,
        //url: '/Common/LoadAsset_Collor?searchCondtiion=' + searchCondtiion + "&selectedIDs=" + _list,
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
    loadPageTool_Detail();
}

function loadPageTool_Detail() {
    var pager = $('#tableList_collar').datagrid('getPager');	// get the pager of datagrid
    pager.pagination({
        buttons: [{
            text: '刷新',
            height: 50,
            iconCls: 'icon-reload',
            handler: function () {
                $('#tableList_collar').datagrid('reload');
                //alert('刷新');
            }
        }, {
            text: '选择资产',
            height: 50,
            iconCls: 'icon-add',
            handler: function () {
              //获取选择中的行数
                //获取选择行
                var rows = $('#tableList_collar').datagrid('getSelections');
                var IDS = [];
                for (var i = 0; i < rows.length; i++) {
                    IDS[i] = rows[i].ID;
                }
                //alert(IDS.toString());
                try{
                    parent.updateCurrentList(IDS);
                    parent.$("#modalwindow").window("close");
                }catch(e){
                }
               
            }
        }],
        beforePageText: '第',//页数文本框前显示的汉字  
        afterPageText: '页    共 {pages} 页',
        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录'
    });
}
//====================================Init Data==================================//




//====================================Function==================================//

function resetSC() {
    $("#SC_Content").val("");
    $('#beginDate_SC').datebox('setValue', '');
    $('#endDate_SC').datebox('setValue', '');
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
    var TypeAsset = "ALL";
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

//表数据重载
function reloadTable_Condition() {
    LoadInitData_Detail();
}
//====================================Function==================================//