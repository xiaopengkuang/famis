$(function () {
    var datagrid; //定义全局变量datagrid
    var editRow = undefined; //定义全局变量：当前编辑的行
    datagrid = $("#dd").datagrid({


        url: '/Notice/Invalidate_Notice',
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
                      return '<span style="color:green">' + amount + '</span>';

              }
          },
          { field: 'serial_number', title: '资产编号', width: 170 },
          { field: 'name_Asset', title: '资产名称', width: 100 },
          { field: 'type_Asset', title: '资产类别', width: 100 },
          { field: 'department_using', title: '使用部门', width: 100 },
          { field: 'specification', title: '规格型号', width: 100 },
           { field: 'unit_price', title: '单价', width: 100 },
             { field: 'amount', title: '数量', width: 100 },
                { field: 'Total_price', title: '总价', width: 100 },
                { field: 'address', title: '存放地址', width: 150 },
            { field: 'owener', title: '使用人', width: 150 },
            {
                field: 'state_asset', title: '资产状态', width: 150

            },
            { field: 'supllier', title: '供应商', width: 150 }
         
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