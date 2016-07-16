$(function () {
    var datagrid; //定义全局变量datagrid
    var editRow = undefined; //定义全局变量：当前编辑的行
    datagrid = $("#dd").datagrid({


        url: '/Notice/Repair_Notice',
        method: 'post', //默认是post,不允许对静态文件访问
        width: 'auto',
        iconCls: 'icon-save',
        dataType: "json",
        fitColumns: false,
        rownumbers: true, //是否加行号
        pagination: true, //是否显式分页
        // onClickCell: onClickCell,
        // onEndEdit: onEndEdit,
        // height:500,
        pageSize: 15, //页容量，必须和pageList对应起来，否则会报错
        pageNumber: 1, //默认显示第几页
        pageList: [15, 30, 45],
        columns: [[//显示的列
         { field: 'ID', title: '序号', width: 100 },
          {
              field: 'DaysNotice', title: '剩余日期', width: 100,
              formatter: function (amount) {

                  if (parseInt(amount) < 0)
                      return '<span style="color:red">' + amount + '</span>';
                  if (parseInt(amount) == 0)
                      return amount;
                  if (parseInt(amount) > 0)
                      return '<span style="color:blue">' + amount + '</span>';

              }
          },
          { field: 'serial_number', title: '维修编号', width: 180 },
          {
              field: 'RepairDate', title: '送修日期', width: 150,
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
              editor: "datetimebox"
          },
          {
              field: 'Returndate', title: '预计归还日期', width: 150,
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
              editor: "datetimebox"
          },
          { field: 'Reason', title: '送修原因', width: 100 },
          { field: 'RepairNote', title: '维修小计', width: 100 },
          { field: 'uapp', title: '申请人', width: 100 },
           { field: 'uaut', title: '授权人', width: 100 },
             { field: 'uview', title: '审核人', width: 100 },
               { field: 'usercreat', title: '单据创建人', width: 100 },
                {
                    field: 'createdate', title: '创建日期', width: 150,
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
                    editor: "datetimebox"
                },
                {
                    field: 'dateview', title: '审核日期', width: 150,

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
                    editor: "datetimebox"
                },
                { field: 'content_Review', title: '审核内容', width: 100 },
            
            {
                field: 'nameqment', title: '装备名称', width: 100,

            },
            { field: 'costrepair', title: '维修费用', width: 100 },
             { field: 'state', title: '审核状态', width: 100 },
               { field: 'supplier', title: '维修供应商', width: 100 },
              
        ]],


        onAfterEdit: function (rowIndex, rowData, changes) {
            //endEdit该方法触发此事件
            console.info(rowData);
            editRow = undefined;
        },
        onDblClickRow: function (rowIndex, rowData) {
            //双击开启编辑行
            if (editRow != undefined) {
                datagrid.datagrid("endEdit", editRow);
            }
            if (editRow == undefined) {
                datagrid.datagrid("beginEdit", rowIndex);
                editRow = rowIndex;
            }
        }
    });

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