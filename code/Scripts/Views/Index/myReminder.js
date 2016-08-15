$(function () {
    loadInitData();
    //$(window).resize(function () {
    //    var win_width = $(window).width();
    //    $("#dataGrid_myReminder").datagrid('resize', { width: win_width - 20 });
    //});

});


function loadInitData()
{
    $('#dataGrid_myReminder').datagrid({
        url: '/Index/load_myreminder',
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
            { field: 'serialNum', title: '待处理单据号', width: 50 },
            { field: 'reminderType', title: '单据类型', width: 50},
            {
                field: 'idOperate', title: '处理', width: 50,hidden:true
               
            },
            {
                field: 'Time_add', title: '创建日期', width: 50,
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
    loadPageTool("dataGrid_myReminder");
}





function loadPageTool(datagrid) {
    var pager = $('#' + datagrid).datagrid('getPager');	// get the pager of datagrid
    pager.pagination({
        buttons: [{
            text: '审核',
            iconCls: 'icon-tip',
            height: 50,
            handler: function () {
                //获取选择行
                var rows = $('#dataGrid_myReminder').datagrid('getSelections');
                if (rows.length != 1) {
                    MessShow("请选择1项数据进行编辑！");
                    return;
                }
                var id = rows[0].idOperate;
                var serialNUM = rows[0].serialNum;
                var url = "/Index/ReviewMidPage?id=" + id + "&serialNUM=" + serialNUM;
                var titleName = "审核";
                openModelWindow(url, titleName);
             
            }
        }],
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

function openModelWindow(url, titleName) {
    //获取当前页面的Width和高度
    var winWidth = (document.body.clientWidth - 20) < 0 ? 0 : (document.body.clientWidth - 20);
    var winheight = (document.body.clientHeight - 20) < 0 ? 0 : (document.body.clientHeight - 20);


    try {
        $("#modalwindow").window("close");
    } catch (e) { }
    var $winADD;
    $winADD = $('#modalwindow').window({
        title: titleName,
        width: winWidth,
        height: winheight,
        left: 10,
        top: 10,
        //top: (($(window).height() - 650) > 0 ? ($(window).height() - 650) : 650) * 0.5,
        //left: (($(window).width() - 1028) > 0 ? ($(window).width() - 1028) : 100) * 0.5,
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