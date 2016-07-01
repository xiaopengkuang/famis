﻿//========================全局数据================================//
var searchCondtiion = "";
//========================全局数据================================//



//===================初始化数据=====================================//
$(function () {
    loadInitData();
})


function loadInitData() {
    //加载所有
    LoadInitDatagrid("datagrid_collor");
}

function LoadInitDatagrid(datagrid) {

    $('#' + datagrid).datagrid({
        url: '/Collar/LoadCollars?searchCondtiion=' + searchCondtiion,
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
            { field: 'serialNumber', title: '单据号', width: 50 },
            { field: 'operatorUser', title: '操作人', width: 50 },
            { field: 'state', title: '状态', width: 50 ,
                formatter: function (data)
                {
                    if (data=="草稿") {
                        return '<font color="#696969">' + data + '</font>';
                    }
                    else if (data == "待审核") {
                        return '<font color="#FFD700">' + data + '</font>';
                    } else if (data == "已审核")
                    {
                        return '<font color="#228B22">' + data + '</font>';
                    } else if (data == "退回") {
                        return '<font color="red">' + data + '</font>';
                    } else {
                        return data;
                    }
                }
            },
            { field: 'address', title: '地址', width: 50 },
            { field: 'department', title: '领用部门', width: 50 }
            ,
            {
                field: 'date_collar', title: '领用时间', width: 100,
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
        singleSelect: false, //允许选择多行
        selectOnCheck: true,//true勾选会选择行，false勾选不选择行, 1.3以后有此选项
        checkOnSelect: true //true选择行勾选，false选择行不勾选, 1.3以后有此选项
    });
    loadPageTool(datagrid);
}

function loadPageTool(datagrid) {
    var pager = $('#' + datagrid).datagrid('getPager');	// get the pager of datagrid
    pager.pagination({
        buttons: [{
            text: '添加',
            iconCls: 'icon-add',
            height: 50,
            handler: function () {

                alert("aaa");
                var title = "添加领用单";
                var url = "/Collar/AddCollarView";
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
                var rows = $('#' + datagrid).datagrid('getSelections');
                var IDS = [];
                for (var i = 0; i < rows.length; i++) {
                    IDS[i] = rows[i].ID;
                }
                if (rows.length > 0)
                {
                    //将数据传入后台
                    $.ajax({
                        url: '/Collar/deleteCollars',
                        data: { "selectedIDs": IDS },
                        //data: _list,  
                        dataType: "json",
                        type: "POST",
                        traditional: true,
                        success: function () {
                            $('#' + datagrid).datagrid('reload');
                        }
                    });
                }
            }
        }, {
            text: '刷新',
            height: 50,
            iconCls: 'icon-reload',
            handler: function () {
                $('#' + datagrid).datagrid('reload');
               
            }
        },
        {
            text: '明细',
            height: 50,
            iconCls: 'icon-tip',
            handler: function () {
                var rows = $('#' + datagrid).datagrid('getSelections');
                var id_;
                if (rows == null)
                {
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

                if(rows.length==1)
                {
                    id_=rows[0].ID;
                } else {
                    var resultAlert = "一次只能查看一个单据！";
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
                    width: 900,
                    height: 600,
                    top: (($(window).height() - 900) > 0 ? ($(window).height() - 900) : 200) * 0.5,
                    left: (($(window).width() - 600) > 0 ? ($(window).width() - 600) : 100) * 0.5,
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
                  var rows = $('#' + datagrid).datagrid('getSelections');
                  var id_;
                  if (rows == null) {
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

                  if (rows.length == 1) {
                      if (rows[0].state != "草稿") {
                          MessShow("只有草稿单据才能提交!")
                          return;
                      }
                      id_ = rows[0].ID;
                  } else {
                      for (var ii = 0; ii < rows.length; ii++)
                      {
                          if (rows[ii].state != "草稿")
                          {
                              MessShow("只有草稿单据才能提交!")
                              return;
                          }

                          if (ii == 0) {
                              id_ = rows[ii].ID;
                             
                          } else {
                              id_=id_+ "_" + rows[ii].ID;
                          }
                      }
                  }
                  updateRecordState(2, id_);
                  $('#' + datagrid).datagrid('reload');
                  //alert('刷新');
              }
          },
           {
               text: '审核',
               height: 50,
               iconCls: 'icon-ok',
               handler: function () {
                   var rows = $('#' + datagrid).datagrid('getSelections');
                   var id_;
                   if (rows == null) {
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

                   if (rows.length == 1) {
                       id_ = rows[0].ID;
                       if (rows[0].state != "待审核") {
                           MessShow("只有待审核单据才能提交!")
                           //$('#allocationDG').datagrid('reload');
                           return;
                       }

                   } else {
                       for (var ii = 0; ii < rows.length; ii++) {
                           if (rows[ii].state!="待审核") {
                               MessShow("只有待审核单据才能提交!")
                               //$('#allocationDG').datagrid('reload');
                               return;
                           }

                           if (ii == 0) {
                               id_ = rows[ii].ID;

                           } else {
                               id_ = id_ + "_" + rows[ii].ID;
                           }
                       }
                   }
                   updateRecordState(3, id_);
                   $('#' + datagrid).datagrid('reload');
                   //alert('刷新');
               }
           }, {
               text: '退回',
               height: 50,
               iconCls: 'icon-undo',
               handler: function () {
                   var rows = $('#' + datagrid).datagrid('getSelections');
                   var id_;
                   if (rows == null) {
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

                   if (rows.length == 1) {
                       id_ = rows[0].ID;
                       if (rows[0].state != "待审核") {
                           MessShow("只有待审核单据才能退回!")
                           //$('#allocationDG').datagrid('reload');
                           return;
                       }

                   } else {
                       MessShow("一次只能处理一个单据!")
                       return;
                       //for (var ii = 0; ii < rows.length; ii++) {
                       //    if (rows[ii].state != "待审核") {
                       //        MessShow("只有待审核单据才能提交!")
                       //        //$('#allocationDG').datagrid('reload');
                       //        return;
                       //    }

                       //    if (ii == 0) {
                       //        id_ = rows[ii].ID;

                       //    } else {
                       //        id_ = id_ + "_" + rows[ii].ID;
                       //    }
                       //}
                   }
                   updateRecordState(datagrid,4, id_);
                   $('#' + datagrid).datagrid('reload');
                   //alert('刷新');
               }
           },{
               text: '导出',
               height: 50,
               iconCls: 'icon-save',
               handler: function () {
                   var filename = getNowFormatDate_FileName();
                   Export(filename, $('#' + datagrid));
               }
           }],
        beforePageText: '第',//页数文本框前显示的汉字  
        afterPageText: '页    共 {pages} 页',
        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录'
    });
}



//根据单据ID更新单据状态
function updateRecordState(datagrid,state, idStr)
{
    //将数据传入后台
    $.ajax({
        url: '/Collar/updateCollarStateByID',
        data: { "state": state,"idStr":idStr },
        //data: _list,  
        dataType: "json",
        type: "POST",
        traditional: true,
        success: function () {
            $('#' + datagrid).datagrid('reload');
        }
    });

}



function MessShow(mess)
{
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



