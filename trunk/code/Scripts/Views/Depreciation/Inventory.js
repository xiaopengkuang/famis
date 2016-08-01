 
var searchCondtiion = "o,o,o,o,o";
//alert(searchCondtiion);
var datagrid; //定义全局变量datagrid
var editRow = undefined; //定义全局变量：当前编辑的行
var PDsearial = "";
var AssetState = "";
var Address = [{ "value": "1", "text": "固定资产" }, { "value": "3", "text": "低值易耗品" }];
var CurrentRow = "0";
var flag = "0";
var sysamount=0;
var isQ = "";
try {
   window.onbeforeunload = onclose;
}
catch (e)
{ };
function onclose() {
   // alert("close!");
    try{
        if (searchCondtiion.split('o,o,o').length > 1)
          SetIsQueryied("true");
        else
            SetIsQueried("false");
}
    catch (e)
    {
    }
   
}
//printExcel('D:/test.csv');
function SetIsQueryied(obj) {
    //alert(obj);
    $.ajax({

        type: "post",
        url: "/Depreciation/SetIsQueryied",
        data: { Json: obj },
        datatype: "json",//数据类型

        success: function (result) {

            
        }, error: function (msg) {

            alert("Error");
        }
    });

}
function GetIsQueried() {
    $.ajax({

        type: "post",
        url: "/Depreciation/GetIsQueryied",
        
        datatype: "json",//数据类型

        success: function (result) {
            isQ = result;
        }, error: function (msg) {

            alert("Error");
        }
    });

}
function ReSetSeachCondition() {
    $("#Invention_Code").val("");
    $('#BeginDate_SC').datebox('setValue', '');
    $('#EndDate_SC').datebox('setValue', '');
    var searchCondtiion = "o,o,o,o,o";
    LoadInitData(searchCondtiion);
}
function printExcel(obj) {
     
    var xlsApp = null;
    try {
        xlsApp = new ActiveXObject('Excel.Application');
    } catch (e) {
        alert(e + ', 原因分析: 浏览器安全级别较高导致不能创建Excel对象或者客户端没有安装Excel软件');
        return;
    }
    //var xlBook = xlsApp.Workbooks.Open('http://'+window.location.host+obj.value);  
    var xlBook = xlsApp.Workbooks.Open(obj);
    var xlsheet = xlBook.Worksheets(1);
    xlsApp.Application.Visible = false;
    xlsApp.visible = false;
    xlsheet.Printout;
    xlsApp.Quit();
    xlsApp = null;
}
$(document).ready(function () {
    //多选框
    GetIsQueried();
    extend();
    loadOperator();
   // setPDserail(PDsearial);
   // LoadInitData_Detail(PDsearial)

    LoadInitData(searchCondtiion);
    
  //  LoadTreeLeft();
  

});
function setPDserail(PDsearial)
{
    $.ajax({

        type: "post",
        url: "/Depreciation/SetPDsearialSession",
        data: { Json: PDsearial },
        datatype: "json",//数据类型

        success: function (result) {


        }, error: function (msg) {

            alert("盘点单号传递失败！");
        }
    });

}
function extend()
{
    $.extend($.fn.datagrid.defaults.editors, {
        datetimebox: {// datetimebox就是你要自定义editor的名称
            init: function (container, options) {
                var input = $('<input class="easyuidatetimebox" data-options="editable: false">').appendTo(container);
                return input.datetimebox({
                    formatter: function (date) {
                        return new Date(date).format("yyyy-MM-dd hh:mm:ss");
                    }
                });
            },
            getValue: function (target) {
                return $(target).parent().find('input.combo-value').val();
            },
            setValue: function (target, value) {
                $(target).datetimebox("setValue", value);
            },
            resize: function (target, width) {
                var input = $(target);
                if ($.boxModel == true) {
                    input.width(width - (input.outerWidth() - input.width()));
                } else {
                    input.width(width);
                }
            }
        }
    });

}
function loadOperator() {
    
    $("#operator").combobox({
        valueField: 'true_Name',
        method: 'POST',
        textField: 'true_Name',
        url: '/Rule/GetUserID',
        onLoadSuccess:function(){
            var data = $('#operator').combobox('getData');
            $("#operator").combobox('select', data[0].true_Name);
        },
        onSelect: function (rec) {
            $('#operator').combobox('setValue', rec.ID);
            $('#operator').combobox('setText', rec.true_Name);
        }
    });
   
}
function LoadBYSearchCondition()
{
   
    var searial="o";
    var begin="o";
    var end="o";
    var state="o";
    var person="o";
    if($('#Invention_Code').val()!="")
      searial=$('#Invention_Code').val();
   
    if( $('#BeginDate_SC').datebox('getValue')!="")
       begin= $('#BeginDate_SC').datebox('getValue');
  
      
    if($('#EndDate_SC').datebox('getValue')!="")
        end = $('#EndDate_SC').datebox('getValue');

    if ($('#Invention_State').combobox('getValue')!="");
    state = $('#Invention_State').combobox('getValue');

    if ($('#operator').combobox('getValue') != "");
    person = $('#operator').combobox('getValue');

   
    searchCondtiion = searial + "," + begin + "," + end + "," + state + "," + person;
    SetIsQueryied("true");
    // alert(searchCondtiion);
    flag = 1;
    LoadInitData(searchCondtiion);

}
 
function LoadInitData(searchCondtiion) {
    //alert(isQ);
    //alert(searchCondtiion);
    var datagrid; //定义全局变量datagrid
    var editRow = undefined; //定义全局变量：当前编辑的行
    //获取操作权限
    $.ajax({
        url: '/Common/getOperationRightsByMenu?menu=PDGL',
        dataType: "json",
        type: "POST",
        traditional: true,
        success: function (dataRight) {
            datagrid = $("#TableList_0_1").datagrid({


                url: '/Depreciation/Query_By_Condition?JSON=' + searchCondtiion + '',

                method: 'post', //默认是post,不允许对静态文件访问
                width: 'auto',
                height: '300px',
                fitColumn: true,
                // fit:true ,
                iconCls: 'icon-save',
                dataType: "json",

                pagePosition: 'top',
                rownumbers: true, //是否加行号 
                pagination: true, //是否显式分页 
                pageSize: 15, //页容量，必须和pageList对应起来，否则会报错 
                pageNumber: 1, //默认显示第几页 
                pageList: [15, 30, 45],//分页中下拉选项的数值 

                columns: [[//显示的列

                       { field: 'ck', checkbox: true, width: 100 },
                    { field: 'ID', title: '序号', width: 150 },
                    {
                        field: 'serial_number', title: '盘点单号', width: 180

                    },
                       {
                           field: 'property', title: '资产性质',  width: 150,
                           editor: { type: 'combobox', options: { data: Address, valueField: "text", editable: false, textField: "text" } }


                       },

                    {
                        field: 'date', title: '盘点日期', width: 180,editable: false,

                        formatter: function (date) {
                            try {
                                var pa = /.*\((.*)\)/;
                                var unixtime = date.match(pa)[1].substring(0, 10);
                                return getTime(unixtime);
                            }
                            catch (e) {
                                return "";
                            }
                        },
                        editor: { type: 'datetimebox', options: { editable:false } } 

                    },
                    {
                        field: 'amountOfSys', title: '系统数量', width: 150

                    },
                    {
                        field: 'amountOfInv', title: '盘点数量', width: 150

                    },
                    {
                        field: 'difference', title: '盘点差异', width: 150,
                        formatter: function (amount) {
                            try {
                                if (parseInt(amount) < 0)
                                    return '<span style="color:red">' + amount + '</span>';
                                else
                                    return amount;
                            }
                            catch (e) {
                                return "";
                            }
                        }

                    },

                     {
                         field: '_operator', title: '操作人',  width: 150,
                         editor: {
                             type: 'combobox', options: {
                                 valueField: 'ID', editable: false, textField: 'true_Name', url: '/Rule/GetUserID',
                                 onSelect: function (rec) {
                                     try {
                                         combobox('setValue', rec.ID);
                                         combobox('setText', rec.true_Name);
                                     }
                                     catch (e)
                                     { }

                                 }, required: false
                             }
                         }
                     },
                     {
                         field: 'state', title: '盘点状态', editble: false, width: 150,
                         formatter: function (state) {
                             try {
                                 if (state == "未盘点")
                                     return '<span style="color:red">未盘点</span>';
                                 if (state == "盘点中")
                                     return '<span style="color:green">盘点中</span>';
                                 if (state == "已盘点")
                                     return '<span style="color:grey">已盘点</span>';
                             }
                             catch (e) {
                                 return "";
                             }
                         }

                     },//这里要formatter一下字体颜色。
                      {
                          field: 'date_Create', title: '制单日期', width: 180,

                          formatter: function (date) {
                              try {
                                  var pa = /.*\((.*)\)/;
                                  var unixtime = date.match(pa)[1].substring(0, 10);
                                  return getTime(unixtime);
                              }
                              catch (e) {
                                  return "";
                              }
                          }
                      },
                     {
                         field: 'ps', title: '备注', width: 300,
                         editor: { type: 'validatebox', options: { required: false } }
                     }

                ]],
                queryParams: { action: 'query' }, //查询参数
                toolbar: [{
                    text: '添加', iconCls: 'icon-add', disabled: !dataRight.add_able, handler: function () {//添加列表的操作按钮添加，修改，删除等
                       
                        openModelWindow("/Verify/Add_Inventory", "新建盘点单")

                    }
                }, '-',
                 {
                     text: '删除', iconCls: 'icon-remove', disabled: !dataRight.delete_able, handler: function () {
                         //删除时先获取选择行
                         flag = 1;
                         var rows = datagrid.datagrid("getSelections");
                         //选择要删除的行
                         if (rows.length > 0) {
                             $.messager.confirm("提示", "你确定要删除吗?", function (r) {
                                 if (r) {
                                     var row = $('#TableList_0_1').datagrid('getSelected');
                                     if (row) {
                                         var index = $('#TableList_0_1').datagrid('getRowIndex', row);
                                         var rowdata = $('#TableList_0_1').datagrid('getData');

                                         var DeleteID = rowdata.rows[index].ID
                                         var DeleteSerail = rowdata.rows[index].serial_number;
                                         var del = $('#TableList_0_1').datagrid('deleteRow', index);
                                         //alert(DeleteID);
                                         $.ajax({

                                             type: "post",
                                             url: "/Depreciation/PDDelete",
                                             data: { ID: DeleteSerail },
                                             datatype: "json",//数据类型

                                             success: function (result) {
                                                 
                                                 $('#TableList_0_2').datagrid('reload');


                                             }, error: function (msg) {
                                                 alert("Fail");
                                                 $('#TableList_0_2').datagrid('reload');
                                             }


                                         });
                                     }
                                     //将选择到的行存入数组并用,分隔转换成字符串，
                                     //本例只是前台操作没有与数据库进行交互所以此处只是弹出要传入后台的id

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
                         if (AssetState == "已盘点")
                             $.messager.alert("提示", "该盘点单已盘点，不可修改！");
                         else {
                             //修改时要获取选择到的行
                             var rows = datagrid.datagrid("getSelections");

                             //如果只选择了一行则可以进行修改，否则不操作
                             if (rows.length == 1) {
                                 //修改之前先关闭已经开启的编辑行，当调用endEdit该方法时会触发onAfterEdit事件
                                 if (editRow != undefined) {
                                     datagrid.datagrid("endEdit", editRow);
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
                                     //datagrid.datagrid("unselectAll");
                                 }
                             }

                         }
                     }
                 }, '-',
                 {
                     text: '保存', iconCls: 'icon-save', disabled: !dataRight.edit_able&&!dataRight.add_able, handler: function () {
                         //保存时结束当前编辑的行，自动触发onAfterEdit事件如果要与后台交互可将数据通过Ajax提交后台
                         datagrid.datagrid("endEdit", editRow);
                         var row = $('#TableList_0_1').datagrid('getSelected');
                         // var row = $("#dd").datagrid('getChanges');
                         if (row) {
                             var index = $('#TableList_0_1').datagrid('getRowIndex', row);
                             var rowdata = $('#TableList_0_1').datagrid('getData');
                             var ID = rowdata.rows[index].ID;
                             var operator = rowdata.rows[index]._operator;
                              var pddate = rowdata.rows[index].date;
                           //  var ed = $('#TableList_0_1').datagrid('getEditor', { index: index, field: 'date' });
                          //   var date_PD = $(ed.target).datebox('getValue');
                          //   alert(ID + "," + operator + "," + ps + "," + pddate + "," + zctype);
                            
                             //var value = $(ed.target).combobox('getValue');

                             var ps = rowdata.rows[index].ps;
                             var zctype = rowdata.rows[index].property;
                            // alert(pddate);
                             $.ajax({

                                 type: "post",
                                 url: "/Depreciation/AddDP",
                                 data: { JSdata: ID + "," + operator + "," + ps + "," + pddate + "," + zctype },
                                 datatype: "json",//数据类型

                                 success: function (result) {


                                     $.messager.alert("提示", "修改成功！", "ok");
                                     $('#TableList_0_1').datagrid('reload');


                                 }, error: function (msg) {
                                     alert("Error");
                                 }
                             });

                         }
                         else alert("请选择您要更改的用户数据！")


                         //...
                     }
                 }, '-',
                 {
                     text: '取消编辑', iconCls: 'icon-redo',  handler: function () {
                         //取消当前编辑行把当前编辑行罢undefined回滚改变的数据,取消选择的行
                         editRow = undefined;
                         datagrid.datagrid("rejectChanges");
                         datagrid.datagrid("unselectAll");
                     }
                 }, '-',
                 {
                     text: '开始盘点', iconCls: 'icon-search', disabled: !dataRight.startpd_able, handler: function () {
                         //取消当前编辑行把当前编辑行罢undefined回滚改变的数据,取消选择的行
                         var rowdata = $('#TableList_0_1').datagrid('getData');


                         if (AssetState != "未盘点" && AssetState == "盘点中") {
                             $.messager.alert("提示", "当前盘点单已开始盘点！");
                         }
                         else if (AssetState != "未盘点" && AssetState == "已盘点") {
                             $.messager.alert("提示", "该盘点单已盘点完成！");
                         }
                         else {

                             if (sysamount == null)
                                 $.messager.alert("提示", "请先选择您要盘点资产的明细！");
                             else {
                                 $.messager.alert("提示", "请在盘点明细列表导入盘点数据的Excel");
                                 //  alert(PDsearial);
                                 $.ajax({

                                     type: "post",
                                     url: "/Depreciation/ChangeState?JSdata=2",//这里1代表未盘点，2代表盘点中，3代表已盘点

                                     datatype: "json",//数据类型

                                     success: function (result) {



                                     }, error: function (msg) {
                                         alert("未能改变当前盘点单的盘点状态！");
                                     }
                                 });
                                 $('#TableList_0_1').datagrid('reload');
                             }
                         }
                     }

                 }, '-',
                 {
                     text: '结束盘点', iconCls: 'icon-sum',disabled: !dataRight.endpd_able, handler: function () {
                         //取消当前编辑行把当前编辑行罢undefined回滚改变的数据,取消选择的行

                         if (AssetState == "未盘点") {
                             $.messager.alert("提示", "该盘点单尚未开始盘点，请核对。");
                         }
                         else if (AssetState == "已盘点")
                             $.messager.alert("提示", "该盘点单已完成盘点，不能对其修改，您可以新建盘点单，重新盘点。");
                         else {
                             $.messager.alert("提示", "盘点数据已保存");

                             $.ajax({

                                 type: "post",
                                 url: "/Depreciation/ChangeState?JSdata=3",//这里1代表未盘点，2代表盘点中，3代表已盘点

                                 datatype: "json",//数据类型

                                 success: function (result) {



                                 }, error: function (msg) {
                                     alert("未能改变当前盘点单的盘点状态！");
                                 }
                             });
                             $('#TableList_0_1').datagrid('reload');
                         }
                     }

                 }, '-',
                 {
                     text: '导出数据', iconCls: 'icon-save', disabled: !dataRight.export_able, handler: function () {
                         //取消当前编辑行把当前编辑行罢undefined回滚改变的数据,取消选择的行
                         if (!dataRight.export_able) {
                             return;
                         }
                         if (PDsearial == "" || PDsearial == null) {
                             $.messager.alert("提示", "盘点明细为空！", "error");
                             return;
                         }
                         var form = $("<form>");//定义一个form表单
                         form.attr("style", "display:none");
                         form.attr("target", "");
                         form.attr("method", "post");
                         form.attr("action", "/Verify/ExportStuPDMain?JSON= "+searchCondtiion+"");
                         var input1 = $("<input>");
                         input1.attr("type", "hidden");
                         input1.attr("name", "exportData");
                         input1.attr("value", (new Date()).getMilliseconds());
                         $("body").append(form);//将表单放置在web中
                         form.append(input1);
                         form.submit();//表单提交
                     }

                 }

                ],

                onSelect: function (index, row) {
                 
                    var rowdata = $('#TableList_0_1').datagrid('getData');
                    try {
                        var searial = rowdata.rows[index].serial_number;//暂时先用ID代替编号

                    }
                    catch (e) {
                    }
                    try {

                        var sys = rowdata.rows[index].amountOfSys;

                    }
                    catch (e) {
                    }
                    //  alert(sys+" "+ID);
                    sysamount = sys;
                    PDsearial = searial;

                    setPDserail(PDsearial);

                    $.ajax({

                        type: "post",
                        url: "/Depreciation/SetCurrentRow",
                        data: { Json: index },
                        datatype: "json",//数据类型

                        success: function (result) {


                        }, error: function (msg) {

                            alert("盘点单索引传递失败！");
                        }
                    });
                    try {
                        AssetState = rowdata.rows[index].state;
                    }
                    catch (e) {

                    }
                    LoadInitData_Detail(PDsearial);
                },
                onAfterEdit: function (rowIndex, rowData, changes) {
                    //endEdit该方法触发此事件
                    console.info(rowData);
                    editRow = undefined;
                },
                onDblClickRow: function (rowIndex, rowData) {
                    if (!dataRight.edit_able) {
                        return;
                    }
                    //双击开启编辑行
                    if (AssetState == "已盘点") {
                        $.messager.alert("提示","该盘点单已盘点，不可修改！");
                    }
                    else {
                        if (editRow != undefined) {
                            datagrid.datagrid("endEdit", editRow);
                        }
                        if (editRow == undefined) {
                            datagrid.datagrid("beginEdit", rowIndex);
                            editRow = rowIndex;
                        }
                    }
                },
                singleSelect: true, //允许选择多行
                selectOnCheck: true,//true勾选会选择行，false勾选不选择行, 1.3以后有此选项
                checkOnSelect: true, //true选择行勾选，false选择行不勾选, 1.3以后有此选项
                fitColumns: false


            });
            $('#TableList_0_1').datagrid({
                onLoadSuccess: function (data) {

                   
                    if (isQ== "true")
                    {
                        $('#TableList_0_1').datagrid('selectRow', 0);
                        SetIsQueryied("false");
                        return;
                    }
                    
                    if (flag == "0") {
                        $.ajax({

                            type: "post",
                            url: "/AssetDeatails/Get_Current_Row",

                            datatype: "json",//数据类型

                            success: function (result) {
                               // alert(result);
                                $('#TableList_0_1').datagrid('selectRow', result);

                            }, error: function (msg) {

                                alert("盘点单索引传递失败！");
                            }
                        });

                    }

                    else {
                        $('#TableList_0_1').datagrid('selectRow', 0);
                       
                    }
                    
                }
            });
        }
    });
   // loadtool();
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
            $('#TableList_0_1').datagrid('reload');
        }
    });
  
    $("#modalwindow").html("<iframe width='100%' height='99%'  frameborder='0' src='" + url + "'></iframe>");
    $winADD.window('open');
}
function cancelForm() {
    parent.$("#modalwindow").window("close");
}
function Add_PD()
{
    var ID = "-1000";
    var operator = $('#operator').combobox('getValue');
    var ps = $('#ps').val();
    var pddate = $('#pddate').datebox('getValue');
    var zctype = $('#ZCLB').combobox('getValue');
    if (pddate == "" || pddate == null) {
        $.messager.alert("提示", "盘点日期不能为空！", "error");
        return;
    }
    $.ajax({

        type: "post",
        url: "/Depreciation/AddDP",
        data: { JSdata: ID + "," + operator + "," + ps + "," + pddate + "," + zctype },
        datatype: "json",//数据类型

        success: function (result) {
         
            cancelForm();
            


        }, error: function (msg) {
            alert("Error");
        }
    });

}
function loadtool() {
    var pager = $('#TableList_0_1').datagrid('getPager');	// get the pager of datagrid
    pager.pagination({
       
        beforePageText: '第',//页数文本框前显示的汉字  
        afterPageText: '页    共 {pages} 页',
        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录'
    });
}
function MessShow(mess) {
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
function LoadInitData_Detail(PDsearial) {
    $.ajax({
        url: '/Common/getOperationRightsByMenu?menu=PDGL',
        dataType: "json",
        type: "POST",
        traditional: true,
        success: function (dataRight) {
    $('#TableList_0_2').datagrid({
        url: '/Depreciation/Load_Inventory_details?JSdata=' + PDsearial + '', //+ , 
        //  url: '/SysSetting/getpageOrder?role=1&tableType=1',yy
        method: 'post', //默认是post,不允许对静态文件访问
        width: 'auto',
        height: '300px',
       // fitColumn: true,
        // fit:true ,
        iconCls: 'icon-save',
        dataType: "json",

        pagePosition: 'top',
        rownumbers: true, //是否加行号 
        pagination: true, //是否显式分页 
        pageSize: 15, //页容量，必须和pageList对应起来，否则会报错 
        pageNumber: 1, //默认显示第几页 
        pageList: [15, 30, 45],//分页中下拉选项的数值 
        columns: [[
            { field: 'ID', title: '序号', width: 150 },
            {
                field: 'state', title: '状态', width: 150,
                formatter: function (state) {
                    switch (state) {
                        case "盘亏":
                            return '<span style="color:red">' + state + '</span>';
                            break;
                        case "持平":
                            return '<span style="color:black">' + state + '</span>';
                            break;
                        case "盘盈":
                            return '<span style="color:green">' + state + '</span>';
                            break;
                    }
                }
            },//需要formatter字体颜色
            { field: 'amountOfSys', title: '系统数量', width: 150 },
            { field: 'amountOfInv', title: '盘点数量', width: 150 },
            {
                field: 'difference', title: '盘点差异', width: 150,
                 formatter: function (amount) {
                    
                        if (parseInt(amount) < 0)
                            return '<span style="color:red">'+amount+'</span>';
                        if (parseInt(amount) == 0)
                            return amount;
                        if(parseInt(amount) >0)
                            return '<span style="color:green">' + amount + '</span>';
                  
                       
                   
                }
            },
            { field: 'serial_number_Asset', title: '资产编号', width: 150 },
            { field: 'type_Asset', title: '资产类别', width:150 },
            { field: 'name_Asset', title: '资产名称', width: 150 },
            { field: 'specification', title: '规格型号', width: 150 },
            { field: 'measurement', title: '计量单位', width: 150 },
            { field: 'unit_price', title: '单价', width: 150 },
            { field: 'amount', title: '数量', width: 150 },
            { field: 'Total_price', title: '总价', width: 150 },
            { field: 'department_using', title: '使用部门', width: 150 },
            { field: 'address', title: '存放地址', width: 150 },
            { field: 'owener', title: '使用人', width: 150 },
            {
                field: 'state_asset', title: '资产状态', width: 150
                
            },
            { field: 'supllier', title: '供应商', width: 150 }
                           



        ]],
       
        singleSelect: false, //允许选择多行
        selectOnCheck: true,//true勾选会选择行，false勾选不选择行, 1.3以后有此选项
        checkOnSelect: true, //true选择行勾选，false选择行不勾选, 1.3以后有此选项
        fitColumns: false
        
    });
    loadPageTool_Detail(dataRight);

        }
    });
    
}

function loadPageTool_Detail(dataRight) {
    var pager = $('#TableList_0_2').datagrid('getPager');	// get the pager of datagrid
    pager.pagination({
        buttons: [{
            text: '新增明细',
            iconCls: 'icon-add',
            height: 50,
            disabled: !dataRight.newDeatails_able,
            handler: function () {
                if (PDsearial == "" || PDsearial == null) {
                    $.messager.alert("提示", "请选择盘点单！", "error");
                    return;
                }
                if (!dataRight.newDeatails_able)
                    return;
                if (AssetState != "未盘点")
                {
                    $.messager.alert("提示", "只能对未盘点的盘点单新增盘点明细！请重新选择盘点单！", "error");
                    
                    return;
                }
                var $winADD;
                $winADD = $('#modalwindow').window({
                    title: '新增明细',
                    width: 1060,
                    height: 680,
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
                        AssetState = "";
                        $('#TableList_0_1').datagrid('reload');
                      //  $('#TableList_0_2').datagrid('reload');
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
                $("#modalwindow").html("<iframe width='100%' height='99%' scrolling='yes' name='ghrzFrame' frameborder='0' src='/Verify/New_Deatails'></iframe>");
                $winADD.window('open');
            }
        },
         
         {
             text: '导入盘点数据',
             disabled: !dataRight.import_able,
             iconCls: 'icon-save',
             height: 50,

             handler: function () {
                 if (!dataRight.import_able)
                     return;
                 if (PDsearial == "" || PDsearial == null) {
                     $.messager.alert("提示", "盘点明细为空！", "error");
                     return;
                 }
               
               
                     
                 if (AssetState == "未盘点")
                    
                 $.messager.alert("提示", "尚未对该盘点单添加盘点明细或尚未开始盘点！", "ok");
                 if (AssetState == "已盘点")
                     $.messager.alert("提示", "该盘点单已完成盘点，您可以新增盘点单进行您要的盘点操作", "error");
                     
                 if (AssetState == "")
                     $.messager.alert("提示", "请选择盘点单据！", "error");
                   
                 if (AssetState == "盘点中") {
                     var $winADD;
                 $winADD = $('#filewindow').window({
                     title: '导入盘点数据',
                     width: 450,
                     height: 280,
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
                         // alert("当我入梦，这个世界就将颤抖!!!");
                          
                         LoadInitData(searchCondtiion);
                         // alert('盘点数据提交成功！');
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
                 $("#filewindow").html("<iframe width='100%' height='99%' scrolling='yes' name='ghrzFrame' frameborder='0' src='/Verify/AddExcel'></iframe>");
                 $winADD.window('open'); 
             }
             }
         },
      {    
     text: '导出',
     disabled: !dataRight.exportdeatails_able,
     height: 50,
     iconCls: 'icon-save',
     handler: function () {
         if (!dataRight.exportdeatails_able)
             return;
         if (PDsearial == "" || PDsearial == null) {
             $.messager.alert("提示", "盘点明细为空！", "error");
             return;
         }
        
         

         var form = $("<form>");//定义一个form表单
         form.attr("style", "display:none");
         form.attr("target", "");
         form.attr("method", "post");
         form.attr("action", "/Verify/ExportPD_Details?JSON= " + PDsearial+ "");
         var input1 = $("<input>");
         input1.attr("type", "hidden");
         input1.attr("name", "exportData");
         input1.attr("value", (new Date()).getMilliseconds());
         $("body").append(form);//将表单放置在web中
         form.append(input1);
         form.submit();//表单提交
    }

          
        }],
        beforePageText: '第',//页数文本框前显示的汉字  
        afterPageText: '页    共 {pages} 页',
        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录'
    });
}
function BrowseFolder() {
    try {
        var Message = "请选择文件夹"; //选择框提示信息
        var Shell = new ActiveXObject("Shell.Application");
        var Folder = Shell.BrowseForFolder(0, Message, 0x0040, 0x11);//起始目录为：我的电脑
        //var Folder = Shell.BrowseForFolder(0,Message,0); //起始目录为：桌面
        if (Folder != null) {
            Folder = Folder.items(); // 返回 FolderItems 对象
            Folder = Folder.item(); // 返回 Folderitem 对象
            Folder = Folder.Path; // 返回路径
            if (Folder.charAt(Folder.length - 1) != "\\") {
                Folder = Folder + "\\";
            }
            document.all.savePath.value = Folder;
            return Folder;
        }
    } catch (e) {
        alert("Error");
    }
}
function ReadExcel()
{
    var isIE = (document.all) ?true : false;
    var isIE7 = isIE && (navigator.userAgent.indexOf('MSIE 7.0') !=-1);
    var isIE8 = isIE && (navigator.userAgent.indexOf('MSIE 8.0') !=-1);
    var isIE6 = isIE && (navigator.userAgent.indexOf('MSIE 6.0') !=-1);
                  
    var file = document.getElementById("fileupload1");
    if (isIE7 || isIE8) {
         file.select();
         //获取欲上传的文件路径
         var path = document.selection.createRange().text;
         document.selection.empty();
         }
    var filepath = document.getElementById("fileupload1").value;
    if (isIE6) 
    {
        path = filepath;
    }
        try {
             netscape.security.PrivilegeManager.enablePrivilege("UniversalXPConnect");
            } 
     catch (e) {
         alert('请更改浏览器设置');
       
        }
     
    var fname = document.getElementById("fileupload1").value;
    var file = Components.classes["@mozilla.org/file/local;1"].createInstance(Components.interfaces.nsILocalFile);
    try {
         // Back slashes for windows
         file.initWithPath( fname.replace(/\//g, "\\\\") );
         }
    catch(e) {
        if (e.result!=Components.results.NS_ERROR_FILE_UNRECOGNIZED_PATH) throw e;
          alert('无法加载文件');
         
         }
    
   alert(file.path); //取得文件路径

}
function myformatter(date) {
    var y = date.getFullYear();
    var m = date.getMonth() + 1;
    var d = date.getDate();
    return y + '-' + (m < 10 ? ('0' + m) : m) + '-' + (d < 10 ? ('0' + d) : d);
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


function ReadExcel()
{
    var tempStr = "";
    var filePath= document.all.upfile.value;
    var oXL = new ActiveXObject("Excel.application"); 
    var oWB = oXL.Workbooks.open(filePath);
    oWB.worksheets(1).select(); 
    var oSheet = oWB.ActiveSheet;
    try{
        for(var i=2;i<46;i++)
        {
            if(oSheet.Cells(i,2).value =="null" || oSheet.Cells(i,3).value =="null" )
                break;
            var a = oSheet.Cells(i,2).value.toString()=="undefined"?"":oSheet.Cells(i,2).value;
            tempStr+=("  "+oSheet.Cells(i,2).value+
             "  "+oSheet.Cells(i,3).value+
             "  "+oSheet.Cells(i,4).value+
             "  "+oSheet.Cells(i,5).value+
             "  "+oSheet.Cells(i,6).value+"\n");
        }
    }catch(e)
    {
        document.all.txtArea.value = tempStr;
    } 
    document.all.txtArea.value = tempStr;
    oXL.Quit();
    CollectGarbage();
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


// 时间格式化
Date.prototype.format = function (format) {
    /*
    * eg:format="yyyy-MM-dd hh:mm:ss";
    */
    if (!format) {
        format = "yyyy-MM-dd hh:mm:ss";
    }

    var o = {
        "M+": this.getMonth() + 1, // month
        "d+": this.getDate(), // day
        "h+": this.getHours(), // hour
        "m+": this.getMinutes(), // minute
        "s+": this.getSeconds(), // second
        "q+": Math.floor((this.getMonth() + 3) / 3), // quarter
        "S": this.getMilliseconds()
        // millisecond
    };

    if (/(y+)/.test(format)) {
        format = format.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    }

    for (var k in o) {
        if (new RegExp("(" + k + ")").test(format)) {
            format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
        }
    }
    return format;
};
function readThis() {
    var tempStr = "";
    var filePath = document.all.upfile.value;
    var oXL = new ActiveXObject("Excel.application");
    var oWB = oXL.Workbooks.open(filePath);
    oWB.worksheets(1).select();
    var oSheet = oWB.ActiveSheet;
    try {
        for (var i = 2; i < 46; i++) {
            if (oSheet.Cells(i, 2).value == "null" || oSheet.Cells(i, 3).value == "null")
                break;
            var a = oSheet.Cells(i, 2).value.toString() == "undefined" ? "" : oSheet.Cells(i, 2).value;
            tempStr += (" " + oSheet.Cells(i, 2).value + " " + oSheet.Cells(i, 3).value + " " + oSheet.Cells(i, 4).value + " " + oSheet.Cells(i, 5).value + " " + oSheet.Cells(i, 6).value + "\n");
        }
    }

    catch (e) {
        //alert(e); 
        document.all.txtArea.value = tempStr;
    }

    document.all.txtArea.value = tempStr; oXL.Quit();
    CollectGarbage();
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