//========================全局数据======================================//
var CurrentList = new Array();

//========================全局数据======================================//




//====================================================================//
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
//====================================================================//

//=======================初始化加载信息===================================//
$(function () {
    
    loadInitData();
});

function loadInitData()
{
    load_Department();
    load_CFDD_add();
    LoadInitData_datagrid();
}

function load_Department() {

    $('#LYBM_add').combotree
    ({
        url: '/Dict/load_SZBM',
        valueField: 'id',
        textField: 'nameText',
        required: true,
        method: 'get',
        editable: false,
        //选择树节点触发事件  
        onSelect: function (node) {
            //返回树对象  
            var tree = $(this).tree;
            //选中的节点是否为叶子节点,如果不是叶子节点,清除选中  
            var isLeaf = tree('isLeaf', node.target);
            if (!isLeaf) {
                //清除选中  
                $('#LYBM_add').combotree('clear');
            } else {
                load_staff_Department(node.id);
            }
            //

        }, //全部折叠
        onLoadSuccess: function (node, data) {
            $('#LYBM_add').combotree('tree').tree("collapseAll");
        }
    });

}

function load_staff_Department(SZBM_ID) {

    $("#SYR_add").combobox({
        valueField: 'id',
        method: 'get',
        textField: 'name',
        url: '/Dict/load_SYR_add?SZBM_ID=' + SZBM_ID,
        onSelect: function (rec) {
            $('#SYR_add').combobox('setValue', rec.id);
            $('#SYR_add').combobox('setText', rec.name);
        }
    });

}


function load_CFDD_add() {
    $('#CFDD_add').combotree
  ({
      url: '/Dict/load_CFDD_add?id_di=9',
      valueField: 'id',
      textField: 'nameText',
      required: true,
      method: 'get',
      editable: false,
      //选择树节点触发事件  
      onSelect: function (node) {
          //返回树对象  
          var tree = $(this).tree;
          //选中的节点是否为叶子节点,如果不是叶子节点,清除选中  
          var isLeaf = tree('isLeaf', node.target);
          if (!isLeaf) {
              //清除选中  
              $('#CFDD_add').combotree('clear');
          } else {
              d_CFDD_add = $('#CFDD_add').combotree('getValue');
          }
          //


      }, //全部折叠
      onLoadSuccess: function (node, data) {
          $('#CFDD_add').combotree('tree').tree("collapseAll");
      }
  });
}



function LoadInitData_datagrid() {

    var _list = "";
    for (var i = 0; i < CurrentList.length; i++) {
        if (i == 0) {
            _list = CurrentList[i] + "";
        } else {
            _list = _list + "_" + CurrentList[i];
        }
    }
    
    $('#collar_DG').datagrid({
        url: '/Asset/Load_SelectedAsset?role=1&state=16&flag=1&selectedIDs=' + _list,
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
            { field: 'state_asset', title: '资产状态', width: 50 },
            { field: 'supplierID', title: '供应商', width: 50 }
            //{
            //    field: 'time_LastLogined', title: '上次登录时间', width: 100,
            //    formatter: function (date) {
            //        var pa = /.*\((.*)\)/;
            //        var unixtime = date.match(pa)[1].substring(0, 10);
            //        return getTime(unixtime);
            //    }
            //}
        ]],
        singleSelect: false, //允许选择多行
        selectOnCheck: true,//true勾选会选择行，false勾选不选择行, 1.3以后有此选项
        checkOnSelect: true //true选择行勾选，false选择行不勾选, 1.3以后有此选项
    });
    loadPageTool_Detail();
}

function loadPageTool_Detail() {
    var pager = $('#collar_DG').datagrid('getPager');	// get the pager of datagrid
    pager.pagination({
        buttons: [{
            text: '添加明细',
            iconCls: 'icon-add',
            height: 50,
            handler: function () {
                var $winADD;
                $winADD = $('#modalwindow').window({
                    title: '选择资产',
                    width: 860,
                    height: 540,
                    top: (($(window).height() - 800) > 0 ? ($(window).height() - 860) : 200) * 0.5,
                    left: (($(window).width() - 500) > 0 ? ($(window).width() - 540) : 100) * 0.5,
                    shadow: true,
                    modal: true,
                    iconCls: 'icon-add',
                    closed: true,
                    minimizable: false,
                    maximizable: false,
                    collapsible: false,
                    onClose: function () {
                        LoadInitData_datagrid();
                    }
                });
                $("#modalwindow").html("<iframe width='100%' height='99%'  frameborder='0' src='/Asset/Collar_SelectAsset'></iframe>");
                $winADD.window('open');
            }
        }, {
            text: '删除明细',
            iconCls: 'icon-remove',
            height: 50,
            handler: function () {
                //获取选择行
                var rows = $('#collar_DG').datagrid('getSelections');
                var IDS = [];
                for (var i = 0; i < rows.length; i++) {
                    IDS[i] = rows[i].ID;
                }
                //将数据传入后台
               
            }
        }, {
            text: '刷新',
            height: 50,
            iconCls: 'icon-reload',
            handler: function () {
                $('#collar_DG').datagrid('reload');
                //alert('刷新');
            }
        }],
        beforePageText: '第',//页数文本框前显示的汉字  
        afterPageText: '页    共 {pages} 页',
        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录'
    });
}




function updateCurrentList(addList)
{
    var tmp = new Array();
    var fi = 0;
    for (var i = 0; i < CurrentList.length; i++)
    {
        tmp[i] = CurrentList[i];
        fi++;
    }
    for (var i = 0; i < addList.length; i++) {
        tmp[fi] = addList[i];
        fi++;
    }
    CurrentList = tmp;
}
function saveData()
{

}

function cancelData() {
    $.messager.confirm('警告', '数据还未保存，您确定要取消吗?', function (r) {
        if (r) {
            window.parent.$('#tabs').tabs('close', '添加领用单');
        } 
    });
}

//==================================================================================//


//function showAsset() {
//    var $winADD;
//    $winADD = $('#modalwindow').window({
//        title: '选择资产',
//        width: 860,
//        height: 540,
//        top: (($(window).height() - 800) > 0 ? ($(window).height() - 800) : 200) * 0.5,
//        left: (($(window).width() - 500) > 0 ? ($(window).width() - 500) : 100) * 0.5,
//        shadow: true,
//        modal: true,
//        iconCls: 'icon-add',
//        closed: true,
//        minimizable: false,
//        maximizable: false,
//        collapsible: false,
//        onClose: function () {
//            LoadInitData_datagrid();
            
//        }
//    });
//    $("#modalwindow").html("<iframe width='100%' height='99%'  frameborder='0' src='/Asset/Collar_SelectAsset'></iframe>");
//    $winADD.window('open');
//}