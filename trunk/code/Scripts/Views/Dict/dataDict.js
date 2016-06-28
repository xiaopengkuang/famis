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
var treeDictID = "tree_Dict";

function loadTree(treeid, url)
{

    
    $('#' + treeid).tree({
        animate: true,
        checkbox: false,
        method: 'POST', //默认是post,不允许对静态文件访问
        //url: '/Dict/loadTree?name=assetType',
        url: url,
        onContextMenu: function (e, node) {
            e.preventDefault();  //该方法将通知 Web 浏览器不要执行与事件关联的默认动作（如果存在这样的动作）
            $('#' + treeid).tree('select', node.target);
            var tree = $("#" + treeid).tree;
           
            //判断其是否支持右键
            //if (isRootNode(tree, node)) {
            //    return;
            //}
            $.ajax({
                url: "/Dict/Check_IsSysSet",
                type: 'POST',
                data: {
                    "table": "dataDict",
                    "id": node.id
                },
                beforeSend: ajaxLoading,
                success: function (data) {
                    ajaxLoadEnd();
                    if (data > 0) {
                    } else {
                        $("#access_menu_tree").menu("show", {
                            left: e.pageX,
                            top: e.pageY
                        });
                    }
                }
            });
        },
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
            loadDataGrid(datagridID, 0, true, treeid);
            //$('#tree_assetType').tree('collapseAll');
        }
    });
}



//Tree operation

function addNode_tree(treeID)
{
    //
    var node = $("#" + treeID).tree('getSelected');

    

    //判断其是否有父节点
    var tree=$("#"+treeID).tree;
    var parent = tree('getParent', node.target);
    var pname=node.text;
    var pid=node.id;
    if (parent.id!= null )
    {
        pname=parent.text;
        pid=parent.id;
    }

    var level=2;
    //= $('#' + treeID).treegrid('getLevel', node.id);


    var url = "/Dict/add_dataDictView?pname=" + pname + "&pid=" + pid+"&level="+level;
    var titleName = "数据字典-添加";
    //alert(url);
    openModelWindow(url,titleName)
}


//编辑左侧
function editeNode_tree(treeID)
{
    var node = $("#" + treeID).tree('getSelected');
    if (node == null) {
        return;
    }
    var tree = $("#" + treeID).tree;
    if (isRootNode(tree, node))
    {
        $.messager.alert('警告',"系统参数禁止编辑！", 'warning');
        return;
    }

    var url = "/Dict/edit_dataDictView?id=" + node.id;
    var titleName = "数据字典-编辑";
    //alert(url);
    openModelWindow(url, titleName)
}

function deleteNode_tree(treeID)
{
    var node = $("#" + treeID).tree('getSelected');
    deleteID(node.id);
}

function deleteID(id)
{
    $.ajax({
        url: "/Dict/Handler_deletedataDict",
        type: 'POST',
        data: {
            "id": id
        },
        beforeSend: ajaxLoading,
        success: function (data) {
            ajaxLoadEnd();
            var result
            if (data > 0) {
                try {
                    loadInitData();
                } catch (e) {

                }
            } else {
                result = "系统正忙，请稍后继续！";
                $.messager.alert('警告', result, 'warning');
            }
        }
    });

}


function treeGridShow()
{

    var sbtitle = document.getElementById("DIV_DictList_current");
    sbtitle.style.display='block';
}

function treeGridHide() {
    var sbtitle = document.getElementById("DIV_DictList_current");
    sbtitle.style.display = 'none';
}


function dataGridShow() {
    var sbtitle = document.getElementById("DIV_dataGrid_current");
    sbtitle.style.display = 'block';
}

function dataGridHide() {
    var sbtitle = document.getElementById("DIV_dataGrid_current");
    sbtitle.style.display = 'none';
}


function isRootNode(tree,node) {
    var parent = tree('getParent', node.target);
    if (parent.id!= null )
    {
        return false;
    }
    return true;
}


//function isSysSetting(treeID)
//{
//    var node = $("#" + treeID).tree('getSelected');
//    $.ajax({
//        url: "/Dict/Check_IsSysSet",
//        type: 'POST',
//        data: {
//            "table": "dataDict",
//            "id": node.id
//        },
//        beforeSend: ajaxLoading,
//        success: function (data) {
//            ajaxLoadEnd();
//            if (data > 0) {
//                return true
//            } else {
//                return false;
//                //deleteID(node.id);
//            }
//        }
//    });
//}


//dataGrid操作



//treeGrid操作











//只显示两列  参数名称  参数描述
function loadDataGrid(gridID,nodeID,barDisable,treeID)
{
    //获取选中行

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
    loadPageTool(gridID, barDisable, treeID);
}



function loadPageTool(datagrid, toolbarDisable,treeID) {
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
                if (datagrid == datagridID)
                {
                    var node = $('#' + treeID).tree('getSelected');
                        if (node != null) {
                            var titleName = "参数-添加";
                            var url = "/Dict/add_dataDictParaView?id_Dict=" + node.id + "&name_Dict=" + node.text;
                            openModelWindow(url, titleName)
                        } else {
                            $.messager.alert('提示', '请选择数据!', 'error');
                            return;
                        }
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
                if (datagrid == datagridID) {
                   
                    deleteIds_DataGrid(datagrid);
                }
            }
        }, {
            text: '刷新',
            iconCls: 'icon-reload',
            height: 50,
            disabled: toolbarDisable,
            handler: function () {
                if (toolbarDisable) {
                    return;
                }
                $('#' + datagrid).datagrid('reload');
              }
        }],
        beforePageText: '第',//页数文本框前显示的汉字  
        afterPageText: '页    共 {pages} 页',
        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录'
    });

}


function deleteIds_DataGrid(datagrid)
{
    var ids;
    var rows = $('#' + datagrid).datagrid('getSelections');
    if (rows.length <= 0) {
        return;
    }
    for (var i = 0; i < rows.length; i++) {
        if (i == 0) {
            ids = rows[i].id;
        } else {
            ids +="_"+ rows[i].id;
        }

    }

    $.ajax({
        url: "/Dict/Handler_deleteDictPara",
        type: 'POST',
        data: {
            "ids": ids
        },
        beforeSend: ajaxLoading,
        success: function (data) {
            ajaxLoadEnd();
            var result
            if (data > 0) {
                try {
                    //获取选中ID
                    var target = "dataGrid_current";
                    $('#' + target).datagrid('reload');
                    //parent.loadDataGrid(target, 0, false, treeid);
                } catch (e) {
                }
            } else {
                result = "系统正忙，请稍后继续！";
                $.messager.alert('警告', result, 'warning');
            }
        }
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
                addBroNode(gridID, treeDictID);
            }
        }, '-', {
            id: 'btnaddChi',
            text: '新增下级',
            iconCls: 'icon-add',
            handler: function () {
                addchild(gridID,treeDictID);
            }
        }, '-', {
            id: 'btnedit',
            text: '修改',
            iconCls: 'icon-edit',
            handler: function () {
                editNode(gridID, treeDictID);
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

//TreeGrid操作
function addBroNode(treeGrid,treeID)
{
    
    //获取选中行
    var node = $('#' + treeGrid).treegrid('getSelected');
    var treeNode = $('#' + treeID).tree('getSelected')
    var pid = 0;

    //判断是否是未选中状态
    if (node == null) {
        pid = 0;
    } else {
        var parentExist = $('#' + treeGrid).treegrid('getParent', node.id);
        if (parentExist) {
            pid = parentExist.id;
        } else {
            pid = 0;
        }
    }
    addnode_treeGrid(treeNode.id,treeNode.text,pid)
}


function addchild(treeGrid, treeID)
{
    //获取选中行
    var node = $('#' + treeGrid).treegrid('getSelected');

    //判断是否是未选中状态
    if (node == null) {
        $.messager.alert('提示', '请选择数据!', 'error');
        return;
    }
    var treeNode = $('#' + treeID).tree('getSelected')
    var parentExist = $('#' + treeGrid).treegrid('getParent', node.id);
    var pid = node.id;
    addnode_treeGrid(treeNode.id, treeNode.text, pid)
}


function addnode_treeGrid(id_Dict,name_Dict, pid)
{
    var titleName = "参数-添加";
    var url = "/Dict/add_dataDictParaView?id_Dict=" + id_Dict + "&name_Dict=" + name_Dict + "&pid=" + pid;
    openModelWindow(url, titleName);
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



function openModelWindow(url, titleName,heightPX,widthPX) {
    var $winADD;
    $winADD = $('#modalwindow').window({
        title: titleName,
        width: 500,
        height: 300,
        top: (($(window).height() - 500) > 0 ? ($(window).height() - 500) : 200) * 0.5,
        left: (($(window).width() - 300) > 0 ? ($(window).width() - 300) : 100) * 0.5,
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

//采用jquery easyui loading css效果
function ajaxLoading() {
    $("<div class=\"datagrid-mask\"></div>").css({ display: "block", width: "100%", height: $(window).height() }).appendTo("body");
    $("<div class=\"datagrid-mask-msg\"></div>").html("正在处理，请稍候。。。").appendTo("body").css({ display: "block", left: ($(document.body).outerWidth(true) - 190) / 2, top: ($(window).height() - 45) / 2 });
}
function ajaxLoadEnd() {
    $(".datagrid-mask").remove();
    $(".datagrid-mask-msg").remove();
}

