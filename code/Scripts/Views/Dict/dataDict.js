$(function () {
    loadInitData();
});


function loadInitData() {
    loadDictTree();
}

function loadDictTree()
{
    var treeid = "tree_Dict";
    var url = "/Dict/loadTree?name=Dict";
    loadTree(treeid, url);
}


var datagridID = "dataGrid_current";
var treegridID = "DictList_current";


function loadTree(treeid, url)
{

    
    $('#' + treeid).tree({
        animate: true,
        checkbox: false,
        method: 'POST', //默认是post,不允许对静态文件访问
        //url: '/Dict/loadTree?name=assetType',
        url: url,
        onClick: function (node) {
            var tree = $(this).tree;
            //Step 1: 先判断其是否是根节点 如果是根节点则点击无效
            if (isRootNode(tree, node))
            {
                return;
            }
            //Step 1:获取表格类型
            $.ajax({
                type: "POST",
                url: "/Dict/isTreeType_DictData",
                data: {"id": node.id},
                success: function (data) {

                    if (data > 0) {
                        dataGridHide();
                        treeGridShow();
                        var actionURL = "/Dict/loadTreeGrid?name=dictPara&dictID=" + node.id;
                        loadTreeGrid(actionURL, treegridID);
                    } else {
                        dataGridShow();
                        treeGridHide();
                        loadDataGrid(datagridID, node.id, false, treeid);
                    }
                }
            });

        },
        onLoadSuccess: function (node, data) {
            $('#' + treeid).show();

            dataGridShow();
            treeGridHide();
            loadDataGrid(datagridID, 0, false, treeid);
            //$('#tree_assetType').tree('collapseAll');
        }
    });
}

function treeGridShow()
{

    var sbtitle = document.getElementById("DIV_DictList_current");
    sbtitle.style.display='block';

    //if(sbtitle){
    //    if(sbtitle.style.display=='block'){
    //        sbtitle.style.display='none';
    //    }else{
    //        sbtitle.style.display='block';
    //    }

    //$("#DIV_dataGrid_current").sty
    //if ($("#DIV_DictList_current").is(":hidden"))
    //{
    //    $("#DIV_DictList_current").toggle();
    //}
}

function treeGridHide() {
    var sbtitle = document.getElementById("DIV_DictList_current");
    sbtitle.style.display = 'none';
    ////if ($("#DIV_DictList_current").is(":hidden")) {
    ////}
    ////$("#DIV_DictList_current").toggle();
}


function dataGridShow() {
    var sbtitle = document.getElementById("DIV_dataGrid_current");
    sbtitle.style.display = 'block';
    //if ($("#DIV_dataGrid_current").is(":hidden")) {
    //    $("#DIV_dataGrid_current").toggle();
    //}
}

function dataGridHide() {
    var sbtitle = document.getElementById("DIV_dataGrid_current");
    sbtitle.style.display = 'none';
    //if ($("#DIV_dataGrid_current").is(":hidden")) {
    //}
    //$("#DIV_dataGrid_current").toggle();
}


//function isTreeType_DictData(id)
//{
//    $.ajax({
//        type: "POST",
//        url: "/Dict/isTreeType_DictData",
//        data: {
//            "id":id
//        },
//        success: function (data) {
//            alert(data);
//            if (data > 0)
//            {
//                return 1;
//            }
//            return 0;
//        }
//    });
//}


function isRootNode(tree,node) {
    var parent = tree('getParent', node.target);
    if (parent.id!= null )
    {
        return false;
    }
    return true;
}



//只显示两列  参数名称  参数描述
function loadDataGrid(gridID,nodeID,barDisable,treeID)
{
    //获取选中行
    //

    alert(gridID);
    $('#' + gridID).datagrid({
        url: "/Dict/loadDataGrid?name=dictPara&dictID=" + nodeID,
        //url: '/Asset/load_attrs_current?assetTypeID=' + selectType,
        method: 'POST', //默认是post,不允许对静态文件访问
        width: 'auto',
        height: 'auto',
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
            { field: 'id', checkbox: true, width: 50, hidden: barDisable },
            { field: 'name', title: '参数名称1', width: 50 },
            { field: 'description', title: '参数描述1', width: 100 }

        ]],
        singleSelect: false, //允许选择多行
        selectOnCheck: true,//true勾选会选择行，false勾选不选择行, 1.3以后有此选项
        checkOnSelect: true //true选择行勾选，false选择行不勾选, 1.3以后有此选项
    });
    //loadPageTool(gridID, barDisable, treeID);
}



function loadPageTool(datagrid, toolbarDisable) {
    //alert(toolbar)
    var pager = $('#' + datagrid).datagrid('getPager');	// get the pager of datagrid
    pager.pagination({
        buttons: [{
            text: '添加',
            iconCls: 'icon-add',
            height: 50,
            disabled: toolbarDisable,
            handler: function () {
                if (toolbarDisable) {
                    return;
                }
                //if (datagrid == "datagrid_current") {
                //    //获取选中的树节点
                //    var node = $('#' + treeID).tree('getSelected');
                //    if (node != null) {
                //        var titleName = "参数-添加";
                //        var url = "/Dict/add_dictPara?id=" + node.id + "&name=" + node.text;
                //        openModelWindow(url, titleName)

                //    } else {
                //        $.messager.alert('提示', '请选择数据!', 'error');
                //        return;
                //    }
                //    //alert("选中的是：" + node.id);
                //}
            }
        }, {
            text: '删除',
            iconCls: 'icon-remove',
            height: 50,
            disabled: toolbarDisable,
            handler: function () {
                if (toolbarDisable) {
                    return;
                }
                //获取选中的datagrid节点
                //if (datagrid == "datagrid_current") {

                //    var rows = $('#' + datagrid).datagrid('getSelections');
                //    var ids;
                //    //alert(rows.length + "L:E");
                //    if (rows.length < 1) {
                //        return;
                //    }
                //    for (var i = 0; i < rows.length; i++) {
                //        if (i == 0) {
                //            ids = "" + rows[i].id;
                //        } else {
                //            ids += "_" + rows[i].id;
                //        }
                //    }
                //    $('#' + datagrid).datagrid('clearChecked')

                //}
                //if (toolbar) {
                //    return;
                //}

                //alert("2");
            }
        }],
        beforePageText: '第',//页数文本框前显示的汉字  
        afterPageText: '页    共 {pages} 页',
        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录'
    });

}

function loadTreeGrid(url,gridID)
{
    $('#' + gridID).treegrid({
        url:url,
        //data:data,
        idField: 'id',
        treeField: 'name',
        fitColumns: true,
        showFooter: true,
        rownumbers: true,
        columns: [[
            { title: '参数名称2', field: 'name', width: 100 },
            { title: '参数描述2', field: 'description', width: 100 }
           
        ]],
        onContextMenu: function (e, row) {
            e.preventDefault();  //该方法将通知 Web 浏览器不要执行与事件关联的默认动作（如果存在这样的动作）
            $("#" + gridID).treegrid("select", row.id);
            $("#access_menu").menu("show", {
                left: e.pageX,
                top: e.pageY
            });
        },
        toolbar: [{
            id: 'btnrefresh',
            text: '刷新',
            iconCls: 'icon-reload',
            handler: function () {
                $("#" + gridID).treegrid('reload');
            }
        }, '-', {
            id: 'expendALL',
            text: '展开',
            iconCls: 'icon-add',
            handler: function () {
                //获取父节点
                $("#" + gridID).treegrid('expandAll');
            }
        }, '-', {
            id: 'closeALL',
            text: '收缩',
            iconCls: 'icon-remove',
            handler: function () {
                //获取父节点
                $("#" + gridID).treegrid('collapseAll');
            }
        }, '-', {
            id: 'btnaddBro',
            text: '新增同级',
            iconCls: 'icon-add',
            handler: function () {
                //获取父节点
                addBroNode(gridID);
            }
        }, '-', {
            id: 'btnaddChi',
            text: '新增下级',
            iconCls: 'icon-add',
            handler: function () {
                addchild(gridID);
            }
        }, '-', {
            id: 'btnedit',
            text: '修改',
            iconCls: 'icon-edit',
            handler: function () {
                editNode(gridID);
            }
        }, '-', {
            id: 'btnremove',
            text: '删除',
            iconCls: 'icon-remove',
            handler: function () {
                deletNode(gridID);
            }
        }]
    });
}





function reloadTree(treeName)
{
    $("#" + treeName).tree('reload');
}
function expandALL(treeName) {
    $("#" + treeName).tree('expandAll');
}
function closeALL(treeName) {
    $('#' + treeName).tree('collapseAll');
}