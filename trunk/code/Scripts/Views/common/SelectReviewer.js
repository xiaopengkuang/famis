
function loadUserByType(menuName)
{
    $('#datagrid_reviewer').datagrid({
        url: '/Common/LoadReviewer?menuName=' + menuName,
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
            { field: 'id', checkbox: true, width: 50 },
            { field: 'name', title: '审核用户', width: 50 }
           
        ]],
        singleSelect: true, //允许选择多行
        selectOnCheck: true,//true勾选会选择行，false勾选不选择行, 1.3以后有此选项
        checkOnSelect: true //true选择行勾选，false选择行不勾选, 1.3以后有此选项
    });
    loadPageTool();
}
function loadPageTool() {
    var pager = $('#datagrid_reviewer').datagrid('getPager');	// get the pager of datagrid
    pager.pagination({
        buttons: [{
            text: '选择',
            height: 50,
            iconCls: 'icon-reload',
            handler: function () {
                var row = $('#datagrid_reviewer').datagrid('getSelected');
                if (row)
                {
                    try{
                        parent.SubmitToUser(row.id);
                        parent.$('#modalwindow').window(close);
                    }catch(e)
                    {
                        MessShow("系统忙！请稍后再试。");
                    }
                }
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
