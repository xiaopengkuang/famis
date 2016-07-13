//========================全局数据================================//
var searchCondtiion = "";

var ID_datagrid = "datagrid_reduction";
var ID_targetState = null;
var ID_Reduction = null;

//========================全局数据================================//



//===================初始化数据=====================================//
$(function () {
    loadInitData();
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

    //setTimeout('refresh()', 15000);
})
function refresh() {
    //是否要判断是否存在新增标签
    loadInitData();
}

function loadInitData() {
    //加载所有
    LoadInitDatagrid("datagrid_reduction");
}



function LoadInitDatagrid(datagrid) {

  
    loadDataGrid(datagrid);

  
}


function SubmitToUser(userID)
{
    updateRecordState(ID_datagrid, ID_targetState, ID_Reduction,userID);
}


function loadDataGrid(datagrid)
{
    //获取操作权限
    $.ajax({
        url: '/Common/getOperationRightsByMenu?menu=ZCJS',
        dataType: "json",
        type: "POST",
        traditional: true,
        success: function (dataRight) {
        $('#' + datagrid).datagrid({
            url: '/Reduction/LoadReduction?searchCondtiion=' + searchCondtiion,
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
                {
                    field: 'date_reduction', title: '减少时间', width: 100,
                    formatter: function (date) {
                        if (date == null) {
                            return "";
                        }
                        var pa = /.*\((.*)\)/;
                        var unixtime = date.match(pa)[1].substring(0, 10);
                        return getTime(unixtime);
                    }
                },
                { field: 'method_reduction', title: '减少方式', width: 50 },
                { field: 'user_apply', title: '申请人', width: 50 },
                { field: 'user_approve', title: '批准人', width: 50 },
                {
                    field: 'state_list', title: '单据状态', width: 50,
                    formatter: function (data) {
                        if (data == "草稿") {
                            return '<font color="#696969">' + data + '</font>';
                        }
                        else if (data == "待审核") {
                            return '<font color="#FFD700">' + data + '</font>';
                        } else if (data == "已审核") {
                            return '<font color="#228B22">' + data + '</font>';
                        } else if (data == "退回") {
                            return '<font color="red">' + data + '</font>';
                        } else {
                            return data;
                        }
                    }
                },
                { field: 'user_operate', title: '登记人员', width: 50 },
                {
                    field: 'dateTime_add', title: '登记时间', width: 100,
                    formatter: function (date) {
                        if (date == null) {
                            return "";
                        }
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
        loadPageTool(datagrid, dataRight);
        }
    });
}

function loadPageTool(datagrid, dataRight) {
    var pager = $('#' + datagrid).datagrid('getPager');	// get the pager of datagrid
    pager.pagination({
        buttons: [{
            text: '添加',
            iconCls: 'icon-add',
            disabled: !dataRight.add_able,
            height: 50,
            handler: function () {
                if (!dataRight.add_able) {
                    return;
                }
                var title = "添加减少单据";
                var url = "/Reduction/Reduction_add";
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
            text: '编辑',
            height: 50,
            disabled: !dataRight.edit_able,
            iconCls: 'icon-edit',
            handler: function () {
                if (!dataRight.edit_able) {
                    return;
                }
                ////获取选择行
                var rows = $('#' + datagrid).datagrid('getSelections');
                if (rows.length != 1)
                {
                    return;
                }

                if (rows[0].state_list == "草稿" || rows[0].state_list == "退回") {
                } else {
                    MessShow("只有草稿/退回单据才能提交!")
                    return;
                }
                var id = rows[0].ID;
                $.ajax({
                    url: "/Reduction/RightToEdit",
                    type: 'POST',
                    data: {
                        "id": id
                    },
                    beforeSend: ajaxLoading,
                    success: function (data) {
                        ajaxLoadEnd();
                        if (data > 0) {
                            var title = "编辑减少";
                            var url = "/Reduction/Reduction_edit?id=" + id;
                            if (parent.$('#tabs').tabs('exists', title)) {
                                parent.$('#tabs').tabs('close', title);
                            } 
                            var content = '<iframe scrolling="auto" frameborder="0"  src="' + url + '" style="width:100%;height:100%;"></iframe>';
                            parent.$('#tabs').tabs('add', {
                                    title: title,
                                    content: content,
                                    icon: 'icon-add',
                                    closable: true
                             });
                            
                        } else {
                            $.messager.alert('警告', "暂无该单据的编辑权限！", 'warning');
                            return;
                        }
                    }
                });
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
            disabled: !dataRight.view_able,
            iconCls: 'icon-tip',
            handler: function () {
                if (!dataRight.view_able) {
                    return;
                }
                var rows = $('#' + datagrid).datagrid('getSelections');
                var id_;
                if (rows == null)
                {
                    $.messager.alert('提示', '请选择领用单!', 'info');
                    return;
                }
                if(rows.length==1)
                {
                    id_=rows[0].ID;
                } else {
                    $.messager.alert('提示', '一次只能查看一个单据!', 'info');
                    return;
                }
                var titleName = "减少-查看明细";
                var url = "/Reduction/Reduction_detail?id=" + id_;
                openModelWindow(url, titleName);
            }
        },
          {
              text: '提交',
              height: 50,
              disabled: !dataRight.submit_able,
              iconCls: 'icon-redo',
              handler: function () {
                  if (!dataRight.submit_able) {
                      return;
                  }
                  var rows = $('#' + datagrid).datagrid('getSelections');
                  var id_;
                  if (rows == null) {
                      MessShow("请选择领用单!");
                      return;
                  }
                  if (rows.length == 1) {
                      if (rows[0].state_list != "草稿") {
                          MessShow("只有草稿单据才能提交!")
                          return;
                      }
                      var url = "/Common/SelectReviewer?menuName=ZCJS"
                      var titleName = "选择管理员";
                      ID_Reduction = rows[0].ID;
                      ID_targetState = 2;
                      //选择超级管理员
                      openModelWindow(url,titleName);
                  } else {
                      MessShow("请勿多选!")
                      return;
                  }
              }
          },
           {
               text: '审核',
               height: 50,
               iconCls: 'icon-ok',
               disabled: !dataRight.review_able,
               handler: function () {
                   if (!dataRight.review_able) {
                       return;
                   }
                   var rows = $('#' + datagrid).datagrid('getSelections');
                   var id_;
                   if (rows == null) {
                       MessShow("请选择领用单！");
                       return;
                   }
                   if (rows.length == 1) {
                       id_ = rows[0].ID;
                       if (rows[0].state_list != "待审核") {
                           MessShow("只有待审核单据才能提交!")
                           return;
                       }
                       var titleName = "审核";
                       var url = "/Reduction/Reduction_review?id=" + id_;
                       openModelWindow(url, titleName);
                   } else {
                   }
               }
           }
           , {
               text: '导出',
               height: 50,
               disabled:!dataRight.export_able,
               iconCls: 'icon-save',
               handler: function () {
                   if (!dataRight.export_able) {
                       return;
                   }
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
function updateRecordState(datagrid,id_target, id,id_reviewer)
{

    var data = {
        "id_Item": id,
        "id_reviewer":id_reviewer,  //用户提交订单时候显示  用户ID
        "id_state": id_target
    }
    //将数据传入后台
    $.ajax({
        url: '/Reduction/updateReductionStateByID',
        data: { "data": JSON.stringify(data) },
        dataType: "json",
        type: "POST",
        traditional: true,
        success: function (data) {
            if(data>0)
            {
                $('#' + datagrid).datagrid('reload');
            } else {
                if (data == -2) {
                    $.messager.alert('警告', "非闲置状态资产不能进行领取！", 'warning');
                } else {
                    $.messager.alert('警告', "系统正忙，请稍后继续！", 'warning');
                }

            }
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
    return y + '-' + (m < 10 ? '0' + m : m) + '-' + (d < 10 ? '0' + d : d);
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



function openModelWindow(url, titleName) {
    var $winADD;
    $winADD = $('#modalwindow').window({
        title: titleName,
        width: 900,
        height: 700,
        top: (($(window).height() - 900) > 0 ? ($(window).height() - 900) : 200) * 0.5,
        left: (($(window).width() - 700) > 0 ? ($(window).width() - 700) : 100) * 0.5,
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
