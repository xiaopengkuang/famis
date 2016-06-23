
$(function () {

    loadInitData();
});


function loadInitData()
{
    loadInitTreeGrid();
}

function loadInitTreeGrid()
{
    var data = {
        "total": 7, "rows": [
        { "id": 1, "name": "All Tasks", "begin": "3/4/2010", "end": "3/20/2010", "progress": 60, "iconCls": "icon-ok" },
        { "id": 2, "name": "Designing", "begin": "3/4/2010", "end": "3/10/2010", "progress": 100, "_parentId": 1, "state": "closed" },
        { "id": 21, "name": "Database", "persons": 2, "begin": "3/4/2010", "end": "3/6/2010", "progress": 100, "_parentId": 2 },
        { "id": 22, "name": "UML", "persons": 1, "begin": "3/7/2010", "end": "3/8/2010", "progress": 100, "_parentId": 2 },
        { "id": 23, "name": "Export Document", "persons": 1, "begin": "3/9/2010", "end": "3/10/2010", "progress": 100, "_parentId": 2 },
        { "id": 3, "name": "Coding", "persons": 2, "begin": "3/11/2010", "end": "3/18/2010", "progress": 80 },
        { "id": 4, "name": "Testing", "persons": 1, "begin": "3/19/2010", "end": "3/20/2010", "progress": 20 }
        ], "footer": [
            { "name": "Total Persons:", "begin": 7, "iconCls": "icon-sum" }
        ]
    };



    $('#treegrid').treegrid({
        //url: '/Dict/loadTreeGrid_AssetType',
        data:data,
        idField: 'id',
        treeField: 'name',
        fitColumns: true,
        showFooter: true,
        rownumbers: true,
        columns: [[
            { title: '名称', field: 'name', width: 180 },
            { title: '折旧方式', field: 'begin', width: 180, align: 'right' },
            { title: '折旧年限', field: 'begin2', width: 180 },
            { title: '资产值率', field: 'end', width: 180 },
            { title: '最后修改时间', field: 'progress', width: 180 },
            { title: '计量单位', field: '_parentId', width: 180 }
        ]],
        onContextMenu:function(e,row){
            e.preventDefault();  //该方法将通知 Web 浏览器不要执行与事件关联的默认动作（如果存在这样的动作）
            $("#treegrid").treegrid("select", row.id);
            $("#access_menu").menu("show",{
                left: e.pageX,
                top: e.pageY
            });
        },

        toolbar: [{
            id: 'btnrefresh',
            text: '刷新',
            iconCls: 'icon-reload',
            handler: function () {
                $('#btnrefresh').linkbutton('enable');
                alert('刷新')
            }
        }, {
            id: 'btnaddBro',
            text: '新增同级',
            iconCls: 'icon-add',
            handler: function () {
                $('#btnaddBro').linkbutton('enable');
                //alert('新增同级')
                //获取父节点
                addBroNode();

               

            }
        }, '-', {
            id: 'btnaddChi',
            text: '新增下级',
            iconCls: 'icon-add',
            handler: function () {
                $('#btnaddChi').linkbutton('enable');
                addchild();
            }
        }, '-', {
            id: 'btnedit',
            text: '修改',
            iconCls: 'icon-edit',
            handler: function () {
                $('#btnedit').linkbutton('enable');
                alert('修改')
            }
        }, '-', {
            id: 'btnremove',
            text: '删除',
            iconCls: 'icon-remove',
            handler: function () {
                $('#btnremove').linkbutton('enable');
                alert('删除')
            }
        }]
    });
}


function addBroNode()
{
   
    var node = $('#treegrid').treegrid('getSelected');

    if (node == null) {
        return;
    }

    var parentExist = $('#treegrid').treegrid('getParent', node.id);
    var parentID;
    var parentName;
    if (parentExist) {
        parentID = parentExist.id;
        parentName = parentExist.name;
        var info="?info="+parentID+"::"+parentName;
        addAssetType(info);
    } else {
        $.messager.alert('提示', '不能添加同级节点!', 'error');
    }
}

function addchild()
{
    var node = $('#treegrid').treegrid('getSelected');

    if (node == null) {
        return;
    }
    var info = "?info=" + node.id + "::" + node.name;
    addAssetType(info);
}


function addAssetType(info)
{
    var $winADD;
    $winADD = $('#modalwindow').window({
        title: '添加资产类型',
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
            //$('#TableList_0_1').datagrid('reload');
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
    $("#modalwindow").html("<iframe width='100%' height='99%'  frameborder='0' src='/Dict/Add_AssetType" + info + "'></iframe>");
    $winADD.window('open');
}

