var searchCondtiion = "1o使用部门o销售部";
//alert("dsd");
$(document).ready(function () {
    //多选框


    $("#operator").combobox({
        valueField: 'ID',
        method: 'get',
        textField: 'name',

        url: '/Sttaff/load_Operator',
        onSelect: function (rec) {

            $('#operator').combobox('setValue', rec.ID);
            $('#operator').combobox('setText', rec.name);


        }
    });
   // Load_Deatails(searchCondtiion);
    LoadInitData(searchCondtiion);
    LoadInitData_Detail(searchCondtiion);
  //  LoadTreeLeft();
  

});
function LoadInitData(searchCondtiion) {
    //  alert("查询条件是：---" + searchCondtiion);
    $('#TableList_0_1').datagrid({
        url: '/Depreciation/Load_Inventory?JSdata=' + searchCondtiion + '', 
        //  url: '/SysSetting/getpageOrder?role=1&tableType=1',
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
        columns: [[
            { field: 'ID', title: '序号', width: 50 },
            { field: 'serial_number', title: '盘点单号', width: 80 },
            { field: 'name_Asset', title: '资产名称', width: 50 },
            {
                field: 'date', title: '盘点日期', width: 80,

                formatter: function (date) {
                    var pa = /.*\((.*)\)/;
                    var unixtime = date.match(pa)[1].substring(0, 10);
                    return getTime(unixtime);
                }
            },
            { field: 'amountOfSys', title: '系统数量', width: 50 },
            { field: 'amountOfInv', title: '盘点数量', width: 50 },
            { field: 'difference', title: '盘点差异', width: 50 },
             { field: 'property', title: '资产性质', width: 50 },
 
             { field: 'operator', title: '操作人', width: 50 },
             { field: 'state', title: '盘点状态', width: 50 },//这里要formatter一下字体颜色。
              { field: 'date_Create', title: '制单日期', width: 50 },
            { field: 'ps', title: '备注', width: 50 }

        ]],
        singleSelect: false, //允许选择多行
        selectOnCheck: true,//true勾选会选择行，false勾选不选择行, 1.3以后有此选项
        checkOnSelect: true, //true选择行勾选，false选择行不勾选, 1.3以后有此选项
        fitColumns: true
    });
    loadPageTool();
}

function loadPageTool() {
    var pager = $('#TableList_0_1').datagrid('getPager');	// get the pager of datagrid
    pager.pagination({
        buttons: [{
            text: '添加',
            iconCls: 'icon-add',
            height: 50,
            handler: function () {
                var $winADD;
                $winADD = $('#modalwindow').window({
                    title: '添加资产',
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
                    collapsible: true,
                    fit: true,
                    fitColum:true,
                    onClose: function () {
                        $('#TableList_0_1').datagrid('reload');
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
                $("#modalwindow").html("<iframe width='100%' height='99%'  frameborder='0' src='/Asset/AddAsset'></iframe>");
                $winADD.window('open');
            }
        }, {
            text: '删除',
            iconCls: 'icon-remove',
            height: 50,
            handler: function () {
                //获取选择行
                var rows = $('#TableList_0_1').datagrid('getSelections');
                var IDS = [];
                for (var i = 0; i < rows.length; i++) {
                    IDS[i] = rows[i].ID;
                }
                //将数据传入后台
                $.ajax({
                    url: '/Asset/deleteAssets',
                    data: { "selectedIDs": IDS },
                    //data: _list,  
                    dataType: "json",
                    type: "POST",
                    traditional: true,
                    success: function () {
                        $('#TableList_0_1').datagrid('reload');
                        //var resultAlert = "成功删除记录！";
                        //$.messager.show({
                        //    title: '提示',
                        //    msg: resultAlert,
                        //    showType: 'slide',
                        //    style: {
                        //        right: '',
                        //        top: document.body.scrollTop + document.documentElement.scrollTop,
                        //        bottom: ''
                        //    }
                        //});
                    }
                });
            }
        }, {
            text: '刷新',
            height: 50,
            iconCls: 'icon-reload',
            handler: function () {
                $('#TableList_0_1').datagrid('reload');
                //alert('刷新');
            }
        }, {
            text: '导出',
            height: 50,
            iconCls: 'icon-save',
            handler: function () {
                var filename = getNowFormatDate_FileName();

                Export(filename, $('#TableList_0_1'));
            }
        }],
        beforePageText: '第',//页数文本框前显示的汉字  
        afterPageText: '页    共 {pages} 页',
        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录'
    });
}
function LoadInitData_Detail(searchCondtiion) {
    //  alert("查询条件是：---" + searchCondtiion);
    $('#TableList_0_2').datagrid({
        url: '/Depreciation/Load_Inventory_details?JSdata=' + "ZC2016050500000006" + '', //+ , 
        //  url: '/SysSetting/getpageOrder?role=1&tableType=1',yy
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
        columns: [[
            { field: 'ID', title: '序号', width: 50 },
            { field: 'state', title: '状态', width: 50 },//需要formatter字体颜色
            { field: 'amountOfSys', title: '系统数量', width: 50 },
            { field: 'amountOfInv', title: '盘点数量', width: 50 },
            { field: 'difference', title: '盘点差异', width: 50 },
            { field: 'serial_number_Asset', title: '资产编号', width: 50 },
            { field: 'type_Asset', title: '资产类别', width: 50 },
            { field: 'name_Asset', title: '资产名称', width: 50 },
            { field: 'specification', title: '规格型号', width: 50 },
            { field: 'measurement', title: '计量单位', width: 50 },
            { field: 'unit_price', title: '单价', width: 50 },
            { field: 'amount', title: '数量', width: 50 },
            { field: 'Total_price', title: '总价', width: 50 },
            { field: 'department_using', title: '使用部门', width: 50 },
            { field: 'address', title: '存放地址', width: 50 },
            { field: 'owener', title: '使用人', width: 50 },
            { field: 'state_asset', title: '资产状态', width: 50 },
            { field: 'supllier', title: '供应商', width: 50 }
                           



        ]],
        singleSelect: false, //允许选择多行
        selectOnCheck: true,//true勾选会选择行，false勾选不选择行, 1.3以后有此选项
        checkOnSelect: true, //true选择行勾选，false选择行不勾选, 1.3以后有此选项
        fitColumns: true
    });
    loadPageTool_Detail();
}

function loadPageTool_Detail() {
    var pager = $('#TableList_0_2').datagrid('getPager');	// get the pager of datagrid
    pager.pagination({
        buttons: [{
            text: '新增明细',
            iconCls: 'icon-add',
            height: 50,
            handler: function () {
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
                        $('#TableList_0_1').datagrid('reload');
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
              iconCls: 'icon-save',
              height: 50,
              handler: function () {
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
                          $('#TableList_0_2').datagrid('reload');
                          alert('盘点数据提交成功！');
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