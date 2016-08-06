﻿LoadMain();
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
            $('#dd').datagrid('reload');
        }
    });

    $("#modalwindow").html("<iframe width='100%' height='99%'  frameborder='0' src='" + url + "'></iframe>");
    $winADD.window('open');
}
function cancelForm() {
    LoadMain();
    parent.$("#modalwindow").window("close");
}
function LoadMain() {
    $.ajax({
        url: '/Common/getOperationRightsByMenu?menu=JSGL',
        dataType: "json",
        type: "POST",
        traditional: true,
        success: function (dataRight) {
            var datagrid; //定义全局变量datagrid
            var editRow = undefined; //定义全局变量：当前编辑的行
            datagrid = $("#dd").datagrid({


                url: '/SysSetting/getpageOrder',
                method: 'post', //默认是post,不允许对静态文件访问
                width: 'auto',
                iconCls: 'icon-save',
                dataType: "json",
                fitColumns: true,
                rownumbers: true, //是否加行号
                pagination: true, //是否显式分页
                // onClickCell: onClickCell,
                // onEndEdit: onEndEdit,
                // height:500,
                pageSize: 15, //页容量，必须和pageList对应起来，否则会报错
                pageNumber: 1, //默认显示第几页
                pageList: [15, 30, 45],
                columns: [[//显示的列

                   { field: 'ck', checkbox: true, width: 100 },
                 { field: 'ID', title: '角色ID', width: 100 },
                  {
                      field: 'name', title: '角色名', width: 100,
                      editor: { type: 'validatebox', options: { required: false } }
                  },
                   {
                       field: 'description', title: '描述', width: 100,
                       editor: { type: 'validatebox', options: { required: false } }
                   }
                ]],
                queryParams: { action: 'query' }, //查询参数
                toolbar: [{
                    text: '添加', iconCls: 'icon-add', disabled: !dataRight.add_able, handler: function () {//添加列表的操作按钮添加，修改，删除等
                        openModelWindow("/SysSetting/AddRole", "新增角色");
                    }
                }, '-',
                 {
                     text: '删除', iconCls: 'icon-remove', disabled: !dataRight.delete_able, handler: function () {
                         //删除时先获取选择行
                         var rows = datagrid.datagrid("getSelections");
                         //选择要删除的行
                         if (rows.length > 0) {
                             $.messager.confirm("提示", "你确定要删除吗?", function (r) {
                                 if (r) {
                                     var row = $('#dd').datagrid('getSelected');
                                     if (row) {
                                         var index = $('#dd').datagrid('getRowIndex', row);
                                         var rowdata = $('#dd').datagrid('getData');

                                         var DeleteID = rowdata.rows[index].ID
                                         var del = $('#dd').datagrid('deleteRow', index);
                                         //alert(DeleteID);
                                         $.ajax({

                                             type: "post",
                                             url: "/SysSetting/RoleDelete",
                                             data: { ID: DeleteID },
                                             datatype: "json",//数据类型

                                             success: function (result) {
                                                 if (result == "Supper") {
                                                     $.messager.alert("提示", "超级管理员不可删除！", "error");

                                                     $('#dd').datagrid('reload');

                                                 }


                                             }, error: function (msg) {
                                                 //alert("删除角色失败！请先将该角色上的权限全部取消，然后尝试删除该角色。");
                                                 $('#List').datagrid('reload');
                                             }


                                         });
                                     }
                                     //将选择到的行存入数组并用,分隔转换成字符串，
                                     //本例只是前台操作没有与数据库进行交互所以此处只是弹出要传入后台的id
                                     //alert(ids.join(','));
                                 }
                             });
                         }
                         else {
                             $.messager.alert("提示", "请选择要删除的行", "error");
                         }
                     }
                 }, '-',

                 {
                     text: '修改', iconCls: 'icon-edit', disabled: !dataRight.edit_able, handler: function () {


                         var rows = datagrid.datagrid("getSelections");
                         var index = datagrid.datagrid("getRowIndex", rows[0]);
                         $("#dd").datagrid('selectRow', index);
                         //如果只选择了一行则可以进行修改，否则不操作
                         if (rows.length == 1) {
                             //修改之前先关闭已经开启的编辑行，当调用endEdit该方法时会触发onAfterEdit事件
                             if (editRow != undefined) {
                                 datagrid.datagrid("endEdit", editRow);
                                 var index = datagrid.datagrid("getRowIndex", rows[0]);
                                 $("#dd").datagrid('selectRow', index);
                             }
                             //当无编辑行时
                             if (editRow == undefined) {
                                 //获取到当前选择行的下标
                                 var index = datagrid.datagrid("getRowIndex", rows[0]);

                                 //开启编辑
                                 datagrid.datagrid("beginEdit", index);
                                 //把当前开启编辑的行赋值给全局变量editRow
                                 editRow = index;
                                 //当开启了当前选择行的编辑状态之后，
                                 //应该取消当前列表的所有选择行，要不然双击之后无法再选择其他行进行编辑
                                 //  datagrid.datagrid("unselectAll");

                             }
                         }


                     }
                 }, '-',
                 {
                     text: '保存', iconCls: 'icon-save', disabled: !dataRight.add_able && !dataRight.add_able, handler: function () {
                         //保存时结束当前编辑的行，自动触发onAfterEdit事件如果要与后台交互可将数据通过Ajax提交后台
                         datagrid.datagrid("endEdit", editRow);
                         var row = $('#dd').datagrid('getSelected');
                         // var row = $("#dd").datagrid('getChanges');
                         if (row) {
                             var index = $('#dd').datagrid('getRowIndex', row);
                             var rowdata = $('#dd').datagrid('getData');
                             var id = rowdata.rows[index].ID;
                             var myname = rowdata.rows[index].name;

                             var des = rowdata.rows[index].description;

                             //  alert(id + "," + myname + "," + des);
                             if (myname == "" || myname == null) {
                                 $.messager.alert("提示", "角色名不能为空", "error");
                                 var rows = datagrid.datagrid("getSelections");
                                 var index = datagrid.datagrid("getRowIndex", rows[0]);
                                 $("#dd").datagrid('selectRow', index);
                                 //如果只选择了一行则可以进行修改，否则不操作
                                 if (rows.length == 1) {
                                     //修改之前先关闭已经开启的编辑行，当调用endEdit该方法时会触发onAfterEdit事件
                                     if (editRow != undefined) {
                                         datagrid.datagrid("endEdit", editRow);
                                         var index = datagrid.datagrid("getRowIndex", rows[0]);
                                         $("#dd").datagrid('selectRow', index);
                                     }
                                     //当无编辑行时
                                     if (editRow == undefined) {
                                         //获取到当前选择行的下标
                                         var index = datagrid.datagrid("getRowIndex", rows[0]);

                                         //开启编辑
                                         datagrid.datagrid("beginEdit", index);
                                         //把当前开启编辑的行赋值给全局变量editRow
                                         editRow = index;
                                         //当开启了当前选择行的编辑状态之后，
                                         //应该取消当前列表的所有选择行，要不然双击之后无法再选择其他行进行编辑
                                         //  datagrid.datagrid("unselectAll");

                                     }
                                 }
                                 return;
                             }
                             if (des == "" || des == null)
                                 des = "..."
                             $.ajax({

                                 type: "post",
                                 url: "/Rule/AddRole",
                                 data: { JSdata: id + "," + myname + "," + des },
                                 datatype: "json",//数据类型

                                 success: function (result) {
                                     if (result == "name_exist") {
                                         $.messager.alert("提示", "该角色名已存在！", "error");
                                         $('#dd').datagrid('reload');
                                         return;
                                     }
                                     if (result == "Supper")

                                         // alert("伦家是超级管理员，你是奈何不了我的哦！");
                                         $.messager.alert("提示", "超级管理员不可做编辑或删除！", "error");
                                     else {
                                         //alert("添加成功！");
                                         $('#dd').datagrid('reload');
                                     }


                                 }, error: function (msg) {
                                     alert("Error");
                                 }
                             });

                         }
                         else $.messager.alert("提示", "请选择您要更改的用户数据！", "error");// alert("请选择您要更改的用户数据！")


                         //...
                     }
                 }, '-',
                 {
                     text: '取消编辑', iconCls: 'icon-redo', handler: function () {
                         //取消当前编辑行把当前编辑行罢undefined回滚改变的数据,取消选择的行
                         editRow = undefined;
                         datagrid.datagrid("rejectChanges");
                         datagrid.datagrid("unselectAll");
                     }
                 }, '-', {
                     text: '权限更改',
                     disabled: !dataRight.rightedit_able,
                     iconCls: 'icon-save',
                     height: 50,
                     handler: function () {
                         if (!dataRight.rightedit_able)
                             return;
                         var row = $('#dd').datagrid('getSelected');
                         if (row) {
                             var index = $('#dd').datagrid('getRowIndex', row);
                             var rowdata = $('#dd').datagrid('getData');

                             var EdTID = rowdata.rows[index].ID;
                             loadPageTool_Detail(EdTID);

                         }
                         else
                             // alert("请先选中要修改的角色！");
                             $.messager.alert("提示", "请先选中要修改的角色！", "error")

                         //  window.open("", "", "top=300,left=800,width=550,height=500");
                     }
                 }
                ],
                onClickRow: function (index, data) {
                    // alert(index);
                    if (editRow != undefined) {
                        datagrid.datagrid("endEdit", editRow);
                    }
                },
                onCheck: function (index, data) {
                    if (editRow != undefined) {
                        datagrid.datagrid("endEdit", editRow);
                    }
                },
                onAfterEdit: function (rowIndex, rowData, changes) {
                    //endEdit该方法触发此事件
                    console.info(rowData);
                    editRow = undefined;
                },
                onDblClickRow: function (rowIndex, rowData) {
                    //双击开启编辑行
                    if (!dataRight.edit_able) {
                        return;
                    }
                    if (editRow != undefined) {
                        datagrid.datagrid("endEdit", editRow);
                    }
                    if (editRow == undefined) {
                        datagrid.datagrid("beginEdit", rowIndex);
                        editRow = rowIndex;
                    }
                },


                singleSelect: true, //允许选择多行
                selectOnCheck: true,//true勾选会选择行，false勾选不选择行, 1.3以后有此选项
                checkOnSelect: true //true选择行勾选，false选择行不勾选, 1.3以后有此选项

            });
           
           
        }
    });

}

function Edit(datagrid, editRow) {

    //修改时要获取选择到的行
    var rows = datagrid.datagrid("getSelections");
    var index = datagrid.datagrid("getRowIndex", rows[0]);
    $("#dd").datagrid('selectRow', index);
    //如果只选择了一行则可以进行修改，否则不操作
    if (rows.length == 1) {
        //修改之前先关闭已经开启的编辑行，当调用endEdit该方法时会触发onAfterEdit事件
        if (editRow != undefined) {
            datagrid.datagrid("endEdit", editRow);
            var index = datagrid.datagrid("getRowIndex", rows[0]);
            $("#dd").datagrid('selectRow', index);
        }
        //当无编辑行时
        if (editRow == undefined) {
            //获取到当前选择行的下标
            var index = datagrid.datagrid("getRowIndex", rows[0]);

            //开启编辑
            datagrid.datagrid("beginEdit", index);
            //把当前开启编辑的行赋值给全局变量editRow
            editRow = index;
            //当开启了当前选择行的编辑状态之后，
            //应该取消当前列表的所有选择行，要不然双击之后无法再选择其他行进行编辑
            //  datagrid.datagrid("unselectAll");

        }
    }
}
function loadPageTool_Detail(EdTID) {

    var $winADD;
    $winADD = $('#modalwindow').window({
        title: '更改权限',
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


        }
    });
    $("#modalwindow").html("<iframe width='100%' height='99%' scrolling='yes' name='ghrzFrame' frameborder='0' src='/SysSetting/RightManage?" + EdTID + "'></iframe>");
    $winADD.window('open');
}