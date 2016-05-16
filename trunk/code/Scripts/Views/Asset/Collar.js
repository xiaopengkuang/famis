//========================全局数据================================//
var searchCondtiion = "";
//========================全局数据================================//



//===================初始化数据=====================================//
$(function () {
    loadInitData();
})


function loadInitData() {
    LoadInitDatagrid();
}

function LoadInitDatagrid() {

    $('#allocationDG').datagrid({
        url: '/Asset/LoadCollars?role=1&flag=1&searchCondtiion=' + searchCondtiion,
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
            { field: 'serialNumber', title: '领用编号', width: 50 },
            { field: 'operatorUser', title: '操作人', width: 50 },
            { field: 'staff', title: '领用人', width: 50 },
            { field: 'state', title: '状态', width: 50 },
            { field: 'address', title: '地址', width: 50 },
            { field: 'department', title: '领用部门', width: 50 }
            ,
            {
                field: 'data_collar', title: '领用时间', width: 100,
                formatter: function (date) {
                    var pa = /.*\((.*)\)/;
                    var unixtime = date.match(pa)[1].substring(0, 10);
                    return getTime(unixtime);
                }
            },
            {
                field: 'date_Operated', title: '登记时间', width: 100,
                formatter: function (date) {
                    var pa = /.*\((.*)\)/;
                    var unixtime = date.match(pa)[1].substring(0, 10);
                    return getTime(unixtime);
                }
            }
        ]],
        singleSelect: true, //允许选择多行
        selectOnCheck: true,//true勾选会选择行，false勾选不选择行, 1.3以后有此选项
        checkOnSelect: true //true选择行勾选，false选择行不勾选, 1.3以后有此选项
    });
    loadPageTool();
}

function loadPageTool() {
    var pager = $('#allocationDG').datagrid('getPager');	// get the pager of datagrid
    pager.pagination({
        buttons: [{
            text: '添加',
            iconCls: 'icon-add',
            height: 50,
            handler: function () {

                var title = "添加领用单";
                var url = "/Asset/AddCollar";
                if (parent.$('#tabs').tabs('exists', title)) {
                    parent.$('#tabs').tabs('select', title);
                } else {
                    var content = '<iframe scrolling="auto" frameborder="0"  src="' + url + '" style="width:100%;height:100%;"></iframe>';
                    parent.$('#tabs').tabs('add', {
                        title: title,
                        content: content,
                        icon: 'icon-add',
                        closable: true
                    });
                }
               
            }
        }, {
            text: '删除',
            height: 50,
            iconCls: 'icon-cancel',
            handler: function () {

                //获取选择行
                var rows = $('#allocationDG').datagrid('getSelections');
                var IDS = [];
                for (var i = 0; i < rows.length; i++) {
                    IDS[i] = rows[i].ID;
                }

                if (rows.length > 0)
                {
                    //将数据传入后台
                    $.ajax({
                        url: '/Asset/deleteCollars',
                        data: { "selectedIDs": IDS },
                        //data: _list,  
                        dataType: "json",
                        type: "POST",
                        traditional: true,
                        success: function () {
                            $('#allocationDG').datagrid('reload');
                        }
                    });
                }

               



            }
        }, {
            text: '刷新',
            height: 50,
            iconCls: 'icon-reload',
            handler: function () {
                $('#allocationDG').datagrid('reload');
               
            }
        },
        {
            text: '明细',
            height: 50,
            iconCls: 'icon-tip',
            handler: function () {
                var rows = $('#allocationDG').datagrid('getSelections');
                var id_;
                if(rows.length==1)
                {
                    id_=rows[0].ID;
                } else {
                    var resultAlert = "请选择领用单！";
                    $.messager.show({
                        title: '提示',
                        msg: resultAlert,
                        showType: 'slide',
                        style: {
                            right: '',
                            top: document.body.scrollTop + document.documentElement.scrollTop,
                            bottom: ''
                        }
                    });
                    return;
                }

                

                var $winADD;
                $winADD = $('#modalwindow').window({
                    title: '领用明细',
                    width: 860,
                    height: 540,
                    top: (($(window).height() - 800) > 0 ? ($(window).height() - 800) : 200) * 0.5,
                    left: (($(window).width() - 500) > 0 ? ($(window).width() - 500) : 100) * 0.5,
                    shadow: true,
                    modal: true,
                    iconCls: 'icon-add',
                    closed: true,
                    minimizable: false,
                    maximizable: false,
                    collapsible: false,
                    onClose: function () {
                        //$('#allocationDG').datagrid('reload');
                        
                    }
                });
                $("#modalwindow").html("<iframe width='100%' height='99%'  frameborder='0' src='/Asset/DetailCollar?id=" + id_ + "'></iframe>");
                $winADD.window('open');
                //alert('刷新');
            }
        },
          {
              text: '提交',
              height: 50,
              iconCls: 'icon-redo',
              handler: function () {
                  $('#allocationDG').datagrid('reload');
                  //alert('刷新');
              }
          },
           {
               text: '审核',
               height: 50,
               iconCls: 'icon-ok',
               handler: function () {
                   $('#allocationDG').datagrid('reload');
                   //alert('刷新');
               }
           }, {
               text: '导出',
               height: 50,
               iconCls: 'icon-save',
               handler: function () {
                   var filename = getNowFormatDate_FileName();
                   Export(filename, $('#allocationDG'));
               }
           }],
        beforePageText: '第',//页数文本框前显示的汉字  
        afterPageText: '页    共 {pages} 页',
        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录'
    });
}

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

function getNowFormatDate_FileName() {
    var date = new Date();
    var seperator1 = "";
    var seperator2 = "";
    var month = date.getMonth() + 1;
    var strDate = date.getDate();
    if (month >= 1 && month <= 9) {
        month = "0" + month;
    }
    if (strDate >= 0 && strDate <= 9) {
        strDate = "0" + strDate;
    }
    var currentdate = date.getFullYear() + seperator1 + month + seperator1 + strDate
            + "" + date.getHours() + seperator2 + date.getMinutes()
            + seperator2 + date.getSeconds();
    return currentdate;
}
//===================初始化数据=====================================//

//===================操作控制数据=====================================//

//===================操作控制数据=====================================//

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



