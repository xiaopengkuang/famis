$(function () {
    loadInitData();
});

function loadInitData()
{
    loadInitTree();
}



function loadInitTree()
{
        //获取查询条件
    $('#tree_assetType').tree({
            animate: true,
            checkbox: false,
            method: 'POST', //默认是post,不允许对静态文件访问
            url: '/Dict/loadTree?name=assetType',
            onClick: function (node) {
                var tree = $(this).tree;
                //选中的节点是否为叶子节点,如果不是叶子节点,清除选中  
                //var isLeaf = tree('isLeaf', node.target);
                //if (isLeaf) {
                //    SearchByCondition_LeftTree(node.id, node.text);
                //}
                //alert(node.id+" - "+node.text)
                loadAttributes(node.id);
            },
            onLoadSuccess: function (node, data) {
                $('#tree_assetType').show();
                //$('#tree_assetType').tree('collapseAll');
            }
    });

    loadAttributes(0);
}





function loadAttributes(selectType)
{
    //alert("selectType："+selectType);
    var url = '/Dict/load_attrs_current?assetTypeID=' + selectType;
    loadDataGridAttrs("datagrid_current", url, false);


    var url = '/Dict/load_attrs_inhert?assetTypeID=' + selectType;
    loadDataGridAttrs("datagrid_inhert", url,true);
}



function loadDataGridAttrs(datagrid,url,toolbar)
{

    //alert(url);
    //获取选中行
    //
    $('#' + datagrid).datagrid({
        url: url,
        //url: '/Asset/load_attrs_current?assetTypeID=' + selectType,
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
            { field: 'id', checkbox: true, width: 50, hidden: toolbar },
            { field: 'xtID', title: '系统ID', width: 50 },
            { field: 'sxbt', title: '属性标题', width: 100 },
            { field: 'zdcd', title: '最大长度', width: 50 },
            { field: 'sfbx', title: '是否必须', width: 50 },
            { field: 'sxlx', title: '属性类型', width: 100 },
            { field: 'glzdlx', title: '关联字典类型', width: 100 }
           
        ]],
        singleSelect: false, //允许选择多行
        selectOnCheck: true,//true勾选会选择行，false勾选不选择行, 1.3以后有此选项
        checkOnSelect: true //true选择行勾选，false选择行不勾选, 1.3以后有此选项
    });
    loadPageTool(datagrid,toolbar);
}

function loadPageTool(datagrid, toolbarDisable)
{
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


                if (datagrid == "datagrid_current")
                {
                    //获取选中的树节点
                    var node = $('#tree_assetType').tree('getSelected');
                    if (node != null) {
                        var titleName = "自定义属性-添加";
                        var url = "/Dict/add_customAttrView?id=" + node.id + "&name=" + node.text;
                        openModelWindow(url, titleName)

                    } else {
                        $.messager.alert('提示', '请选择资产类别!', 'error');
                        return;
                    }
                    //alert("选中的是：" + node.id);
                }
              

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
                if (datagrid == "datagrid_current") {

                    var rows = $('#' + datagrid).datagrid('getSelections');
                    var ids ;
                    //alert(rows.length + "L:E");
                    if (rows.length < 1)
                    {
                        return;
                    }
                    for (var i = 0; i < rows.length; i++) {
                        if (i == 0) {
                            ids =""+ rows[i].id;
                        } else {
                            ids += "_"+rows[i].id;
                        }
                    }
                    deleteCAttr(ids);
                    $('#' + datagrid).datagrid('clearChecked')
                    
                }
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

function deleteCAttr(attrID)
{
    //基础参数无法删除
    //系统固定
    
    $.ajax({
        url: "/Dict/Handler_deleteCAttr",
        type: 'POST',
        data: {
            "ids": attrID
        },
        beforeSend: ajaxLoading,
        success: function (data) {
            ajaxLoadEnd();
            var result
            if (data > 0) {
                try {
                    $("#datagrid_current").datagrid('reload');
                } catch (e) {

                }
            } else {
                result = "系统正忙，请稍后继续！";
                $.messager.alert('警告', result, 'warning');
            }
        }
    });
}





function openModelWindow(url, titleName) {
    var $winADD;
    $winADD = $('#modalwindow').window({
        title: titleName,
        width: 500,
        height: 350,
        top: (($(window).height() - 500) > 0 ? ($(window).height() - 500) : 200) * 0.5,
        left: (($(window).width() - 350) > 0 ? ($(window).width() - 350) : 100) * 0.5,
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

