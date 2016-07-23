//alert("111112222222222");
$(function () {
    var datagrid; //定义全局变量datagrid
    
    datagrid = $("#Repair_dd").datagrid({


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
                field: 'nameqment', title: '装备名称', width: 100

            },
            { field: 'costrepair', title: '维修费用', width: 100 },
             { field: 'state', title: '审核状态', width: 100 },
               { field: 'supplier', title: '维修供应商', width: 100 } 
              
        ]]

 
        

    });
     
    loadPageTool_Detail();

});

function loadPageTool_Detail() {
     
    var pager = $('#Repair_dd').datagrid('getPager');	// get the pager of datagrid
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
              form.attr("action", "/Verify/ExportRP_Notice");
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
