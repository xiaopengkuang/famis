var myDate = new Date();
var date = myDate.toLocaleDateString();
var dptid = "o";
LoadMain();

function tt() {
       
    $.ajax({

        type: "post",
        url: "/Rule/GetRoleID",

        datatype: "json",//数据类型

        success: function (result) {
               
            return result;
             

        }, error: function (msg) {
            alert("Error");
            return "";
        }
    });
}
function SetEditID(obj) {

    $.ajax({

        type: "post",
        url: "/Depreciation/SetEditID",
        data: { Json: obj },
        datatype: "json",//数据类型

        success: function (result) {


        }, error: function (msg) {

            alert("Error");
        }
    });

}
function load_SZBM_add() {
    $('#SZBM_add').combotree
    ({
        url: '/Dict/load_SZBM?RoleID=',
        valueField: 'id',
        textField: 'nameText',
        required: true,
        method: 'POST',
        editable: false,
        //选择树节点触发事件  
        onSelect: function (node) {
            d_SZBM_add = $('#SZBM_add').combotree('getValue');
            load_User_add(node.id);
        }, //全部折叠
        onLoadSuccess: function (node, data) {
            //$('#SZBM_add').combotree('tree').tree("collapseAll");
        }
    });

}
function load_User_add(id_DP) {
    $("#SYRY_add").combobox({
        valueField: 'id',
        method: 'POST',
        editable: false,
        textField: 'name',
        url: '/Dict/load_User_add?id_DP=' + id_DP,
        onSelect: function (rec) {
            $('#SYRY_add').combobox('setValue', rec.id);
            $('#SYRY_add').combobox('setText', rec.name);
        }
    });

}
function LoadMain() {
    $.ajax({
        url: '/Common/getOperationRightsByMenu?menu=YHGL',
        dataType: "json",
        type: "POST",
        traditional: true,
        success: function (dataRight) {
            var datagrid; //定义全局变量datagrid
            var editRow = undefined; //定义全局变量：当前编辑的行
            datagrid = $("#dd").datagrid({


                url: '/SysSetting/getUser?role=1&tableType=1',
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
                   {
                       field: 'ID', title: '用户ID', width: 100
                   
                   },
                 {
                     field: 'name_User', title: '用户名', width: 100,
                     editor: { type: 'validatebox', options: { required: false } }
                 },
                  {
                      field: 'password_User', title: '密码', width: 100,
                      editor: { type: 'validatebox', options: { required: false } }
                  },
                   {
                       field: 'true_Name', title: '真实姓名', width: 100,
                       editor: { type: 'validatebox', options: { required: false } }
                   },
                     {
                         field: 'roleID_User', title: '角色', width: 100,
                         editor: {
                             type: 'combobox', options: {
                                 valueField: 'name', textField: 'name', editable: false,url: '/Rule/GetRoleID',
                                 onSelect: function (rec) {
                                     try {
                                         combobox('setValue', rec.name);
                                         combobox('setText', rec.name);
                                     }
                                     catch (e)
                                     { }

                                 }, required: false
                             }
                         }
                     },
                      {
                          field: 'ID_DepartMent', title: '部门', width: 100,
                          editor: {
                              type: 'combotree', options: {
                                  valueField: 'nameText', textField: 'nameText', method: 'POST', editable: false, url: '/Dict/load_SZBM',
                                  onSelect: function (node) {
                                      try {
                                      
                                          dptid =node.text;
                                      
                                          // combotree('getValue');
                                      
                                      }
                                      catch (e)
                                      { }

                                  },required: false
                              }
                          }
                      } 

                ]],
                queryParams: { action: 'query' }, //查询参数
                toolbar: [{
                    text: '添加', iconCls: 'icon-add', disabled: !dataRight.add_able, handler: function () {//添加列表的操作按钮添加，修改，删除等
                        //添加时先判断是否有开启编辑的行，如果有则把开户编辑的那行结束编辑
                        openModelWindow("/SysSetting/AddUser","添加用户");

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
                                             url: "/SysSetting/UserDelete",
                                             data: { ID: DeleteID },
                                             datatype: "json",//数据类型

                                             success: function (result) {
                                           



                                             }, error: function (msg) {
                                                 alert("删除用户失败！");
                                                 $('#List').datagrid('reload');
                                             }


                                         });
                                     }
                                     //将选择到的行存入数组并用,分隔转换成字符串，
                                     //本例只是前台操作没有与数据库进行交互所以此处只是弹出要传入后台的id
                                     // alert(ids.join(','));
                                 }
                             });
                         }
                         else {
                             $.messager.alert("提示", "请选择要删除的行!", "error");
                         }
                     }
                 }, '-',
                 {
                     text: '修改', iconCls: 'icon-edit', disabled: !dataRight.edit_able, handler: function () {
                         //修改时要获取选择到的行
                         var row = $('#dd').datagrid('getSelected');
                          
                            
                         
                         if (row) {

                             var index = $('#dd').datagrid('getRowIndex', row);
                             var rowdata = $('#dd').datagrid('getData');
                             // alert("xiugai "+index);
                             var EID = rowdata.rows[index].ID;
                             //alert(EID);
                             SetEditID(EID);

                             openModelWindow("/SysSetting/Edit_User", "修改用户");

                         }
                         else {
                             $.messager.alert("提示", "请选择要编辑的行!", "error");
                             return;
                         }

                     }
                 },'-',
                 {
                     text: '刷新', iconCls: 'icon-undo', handler: function () {
                         //取消当前编辑行把当前编辑行罢undefined回滚改变的数据,取消选择的行
                         editRow = undefined;
                         datagrid.datagrid("reload");
                         
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
                    try {
                        console.info(rowData);
                        editRow = undefined;
                    }
                    catch (e)
                    { }
                },
                onDblClickRow: function (rowIndex, rowData) {
                    //双击开启编辑行
                    var row = $('#dd').datagrid('getSelected');;
                    
                    if (row) {

                        var index = $('#dd').datagrid('getRowIndex', row);
                        var rowdata = $('#dd').datagrid('getData');
                        // alert("xiugai "+index);
                        var EID = rowdata.rows[index].ID;
                        //alert(EID);
                        SetEditID(EID);

                        openModelWindow("/SysSetting/Edit_User", "修改用户");

                    }
                },
            
                singleSelect: true, //允许选择多行
                selectOnCheck: true,//true勾选会选择行，false勾选不选择行, 1.3以后有此选项
                checkOnSelect: true //true选择行勾选，false选择行不勾选, 1.3以后有此选项
         
            });
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
 
 