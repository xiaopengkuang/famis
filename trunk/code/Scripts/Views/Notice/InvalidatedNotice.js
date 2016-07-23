 
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
        pagePosition: 'top',
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
         
        ]]


     
         
    });
    loadPageTool_Detail();

});

function loadPageTool_Detail() {
    var pager = $('#dd').datagrid('getPager');	// get the pager of datagrid
    pager.pagination({
        buttons: [{
            text: '导出',
            height: 50,
            iconCls: 'icon-save',
            handler: function () {
                var form = $("<form>");//定义一个form表单
                form.attr("style", "display:none");
                form.attr("target", "");
                form.attr("method", "post");
                form.attr("action", "/Verify/ExportIV_Notice");
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