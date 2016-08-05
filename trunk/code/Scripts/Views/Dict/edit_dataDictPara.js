$(function () {


});



function dataBind(id)
{
    $.ajax({
        url: "/Dict/Handler_getDictPara",
        type: 'POST',
        data: {
            "id": id
        },
        beforeSend: ajaxLoading,
        success: function (data) {
            ajaxLoadEnd();
            if (data!=null) {
                //alert(data[0].csmc);
                //alert(data[0].csms);
                $("#CSMC").val(data[0].csmc);
                $("#CSLX").combobox("setValue", data[0].cslx);
                $("#CSLX").combobox("setText", data[0].cslx_name);
                $("#CSMS").val(data[0].csms);
            } else {
                //alert("s");
            }
        }
    });
}


function submitForm() {

    //获取数据
    var csmc = $("#CSMC").val();

    var csms = $("#CSMS").val();

    var cslx = $("#CSLX").combobox("getValue");

    //var sfbs = $("#SFBS").is(':checked');

    var id = $("#SELECTEDID").val();

   

    var data = {
        "csmc": csmc,
        "cslx": cslx,
        "csms": csms
    };
    //alert(JSON.stringify(data));

    //return;
    $.ajax({
        url: "/Dict/Handler_UpdateDictPara",
        type: 'POST',
        data: {
            "data": JSON.stringify(data),
            "id":id
        },
        beforeSend: ajaxLoading,
        success: function (data) {
            ajaxLoadEnd();
            var result
            if (data > 0) {
                try {
                    parent.$("#modalwindow").window("close");
                    //获取选中ID

                    var target = "dataGrid_current";
                    var target2 = "DictList_current";
                    var treeid = "tree_Dict";
                    //TODO: 页面传参数 控制更新特定的数据  这样不用都刷新
                    parent.$('#' + target2).treegrid('reload');
                    parent.$('#' + target).datagrid('reload');
                    //parent.loadDataGrid(target, 0, false, treeid);
                } catch (e) {
                }
            } else if (data == -2) {
                MessShow("已存在同名参数！");
            } else {
                MessShow("添加数据失败，请稍后继续！");
            }
        }
    });
}


function cancelForm() {
    parent.$("#modalwindow").window("close");
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

//采用jquery easyui loading css效果
function ajaxLoading() {
    $("<div class=\"datagrid-mask\"></div>").css({ display: "block", width: "100%", height: $(window).height() }).appendTo("body");
    $("<div class=\"datagrid-mask-msg\"></div>").html("正在处理，请稍候。。。").appendTo("body").css({ display: "block", left: ($(document.body).outerWidth(true) - 190) / 2, top: ($(window).height() - 45) / 2 });
}
function ajaxLoadEnd() {
    $(".datagrid-mask").remove();
    $(".datagrid-mask-msg").remove();
}

