﻿
$(function () {

    loadInitData();
});


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

function loadInitData()
{
    loadInitTreeGrid();
}

function loadInitTreeGrid()
{
    //var data = {
    //    "total": 7, "rows": [
    //    { "id": 1, "name": "All Tasks", "begin": "3/4/2010", "end": "3/20/2010", "progress": 60, "iconCls": "icon-ok" },
    //    { "id": 2, "name": "Designing", "begin": "3/4/2010", "end": "3/10/2010", "progress": 100, "_parentId": 1, "state": "closed" },
    //    { "id": 21, "name": "Database", "persons": 2, "begin": "3/4/2010", "end": "3/6/2010", "progress": 100, "_parentId": 2 },
    //    { "id": 22, "name": "UML", "persons": 1, "begin": "3/7/2010", "end": "3/8/2010", "progress": 100, "_parentId": 2 },
    //    { "id": 23, "name": "Export Document", "persons": 1, "begin": "3/9/2010", "end": "3/10/2010", "progress": 100, "_parentId": 2 },
    //    { "id": 3, "name": "Coding", "persons": 2, "begin": "3/11/2010", "end": "3/18/2010", "progress": 80 },
    //    { "id": 4, "name": "Testing", "persons": 1, "begin": "3/19/2010", "end": "3/20/2010", "progress": 20 }
    //    ]
    //    //, "footer": [
    //    //    { "name": "Total Persons:", "begin": 7, "iconCls": "icon-sum" }
    //    //]
    //};



    $('#treegrid').treegrid({
        url: '/Dict/loadTreeGrid_AssetType',
        //data:data,
        idField: 'id',
        treeField: 'lbmc',
        fitColumns: true,
        showFooter: true,
        rownumbers: true,
        columns: [[
            { title: '名称', field: 'lbmc', width: 180 },
            { title: '折旧方式', field: 'zjfs', width: 180, align: 'right' },
            { title: '折旧年限', field: 'zjnx', width: 180 },
            { title: '资产值率', field: 'jczl', width: 180 },
            {
                title: '最后修改时间', field: 'lastEditTime', width: 180,
                formatter: function (date) {
                        var pa = /.*\((.*)\)/;
                        var unixtime = date.match(pa)[1].substring(0, 10);
                        return getTime(unixtime);
                } 
            },
            { title: '计量单位', field: 'jldw', width: 180 }
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
                $("#treegrid").treegrid('reload');
            }
        }, {
            id: 'btnaddBro',
            text: '新增同级',
            iconCls: 'icon-add',
            handler: function () {
                $('#btnaddBro').linkbutton('enable');
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
                editNode();
            }
        }, '-', {
            id: 'btnremove',
            text: '删除',
            iconCls: 'icon-remove',
            handler: function () {
                $('#btnremove').linkbutton('enable');
                deletNode();
            }
        }]
    });
}


function addBroNode()
{
   
    var node = $('#treegrid').treegrid('getSelected');

    
    if (node == null) {
        $.messager.alert('提示', '请选择数据!', 'error');
        return;
    }
    var level = $('#treegrid').treegrid('getLevel', node.id);
    var parentExist = $('#treegrid').treegrid('getParent', node.id);
    var parentID;
    var parentName;
    if (parentExist) {
        parentID = parentExist.id;
        parentName = parentExist.lbmc;

        var info = "?pid=" + parentID + "&pname=" + parentName + "&level=" + level;
        addAssetType(info,"资产类别-添加同级");
    } else {
        $.messager.alert('提示', '不能添加同级节点!', 'error');
        return;
    }
}

function addchild()
{
    var node = $('#treegrid').treegrid('getSelected');

    if (node == null) {
        return;
    }
    var level = $('#treegrid').treegrid('getLevel', node.id)+1;
    var info = "?pid=" + node.id + "&pname=" + node.lbmc + "&level=" + level;
    addAssetType(info, "资产类别-添加下级");
}

function editNode()
{
    var node = $('#treegrid').treegrid('getSelected');
    if (node == null) {
        $.messager.alert('提示', '请选择数据!', 'error');
        return;
    }

    //var level = $('#treegrid').treegrid('getLevel', node.id);
    var info = "?id=" + node.id + "&name=" + node.lbmc;
    editAssetType(info);
}

function editAssetType(info)
{
    var titleName = "编辑资产类型";
    var url = "/Dict/edit_AssetType" + info;
    openModelWindow(url, titleName);
}
function deletNode()
{
    var node = $('#treegrid').treegrid('getSelected');
    if (node == null) {
        $.messager.alert('提示', '请选择数据!', 'error');
        return;
    }
}


function addAssetType(info,title)
{
    var titleName = title;
    var url = "/Dict/add_AssetType" + info;
    //alert(url);
    openModelWindow(url, titleName);
    
}


function openModelWindow(url,titleName)
{
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
    $("#modalwindow").html("<iframe width='100%' height='99%'  frameborder='0' src='"+url+"'></iframe>");
    $winADD.window('open');
}
