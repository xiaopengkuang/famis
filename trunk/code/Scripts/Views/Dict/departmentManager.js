
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

function loadInitData() {
    loadInitTreeGrid();
}

function loadInitTreeGrid() {
    $('#treegrid_dp').treegrid({
        url: '/Dict/loadTreeGrid?name=department',
        //data:data,
        idField: 'id',
        treeField: 'name',
        fitColumns: true,
        showFooter: true,
        rownumbers: true,
        columns: [[
            { title: '名称', field: 'name', width: 180 },
            { title: '操作人', field: 'operatorName', width: 180, align: 'right' }
            ,
            {
                title: '创建时间', field: 'time', width: 180,
                formatter: function (date) {
                    var pa = /.*\((.*)\)/;
                    var unixtime = date.match(pa)[1].substring(0, 10);
                    return getTime(unixtime);
                }
            }
        ]],
        onContextMenu: function (e, row) {
            e.preventDefault();  //该方法将通知 Web 浏览器不要执行与事件关联的默认动作（如果存在这样的动作）
            $("#treegrid_dp").treegrid("select", row.id);
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
                $('#btnrefresh').linkbutton('enable');
                $("#treegrid_dp").treegrid('reload');
            }
        }, '-', {
            id: 'expendALL',
            text: '展开',
            iconCls: 'icon-add',
            handler: function () {
                $('#btnaddBro').linkbutton('enable');
                //获取父节点
                $("#treegrid_dp").treegrid('expandAll');
            }
        }, '-', {
            id: 'closeALL',
            text: '收缩',
            iconCls: 'icon-remove',
            handler: function () {
                $('#btnaddBro').linkbutton('enable');
                //获取父节点
                $("#treegrid_dp").treegrid('collapseAll');
            }
        }, '-', {
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
    $("#treegrid_dp").treegrid("expandAll");
}


function addBroNode() {

    var node = $('#treegrid_dp').treegrid('getSelected');


    if (node == null) {
        $.messager.alert('提示', '请选择数据!', 'error');
        return;
    }
    var level = $('#treegrid_dp').treegrid('getLevel', node.id);
    var parentExist = $('#treegrid_dp').treegrid('getParent', node.id);
    var parentID;
    var parentName;
    if (parentExist) {
        parentID = parentExist.id;
        parentName = parentExist.name;

        var info = "?pid=" + parentID + "&pname=" + parentName + "&level=" + level;
        addDepartment(info, "部门-添加同级");
    } else {
        $.messager.alert('提示', '不能添加同级节点!', 'error');
        return;
    }
}

function addchild() {
    var node = $('#treegrid_dp').treegrid('getSelected');

    if (node == null) {
        return;
    }
    var level = $('#treegrid_dp').treegrid('getLevel', node.id) + 1;
    var info = "?pid=" + node.id + "&pname=" + node.name + "&level=" + level;
    addDepartment(info, "部门-添加下级");
}

function editNode() {
    var node = $('#treegrid_dp').treegrid('getSelected');
    if (node == null) {
        $.messager.alert('提示', '请选择数据!', 'error');
        return;
    }

    //var level = $('#treegrid').treegrid('getLevel', node.id);
    var info = "?id=" + node.id + "&name=" + node.name;
    editDepartment(info);
}

function editDepartment(info) {
    var titleName = "编辑部门";
    var url = "/Dict/edit_departmentView" + info;
    openModelWindow(url, titleName);
}
function deletNode() {
    var node = $('#treegrid_dp').treegrid('getSelected');
    if (node == null) {
        $.messager.alert('提示', '请选择数据!', 'error');
        return;
    }
}


function addDepartment(info, title) {
    var titleName = title;
    var url = "/Dict/add_departmentView" + info;
    //alert(url);
    //alert(info);
    openModelWindow(url, titleName);

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
