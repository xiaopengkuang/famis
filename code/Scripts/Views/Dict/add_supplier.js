$(function () {


});
function cancelForm() {
    parent.$("#modalwindow").window("close");
}


function submitForm()
{
    //获取数据
    var GYSMC = $("#GYSMC").val();
    var LXR = $("#LXR").val();
    var LXDH = $("#LXDH").val();
    var YX = $("#YX").val();
    var CZ = $("#CZ").val();
    var DZ = $("#DZ").val();
    //提交数据
    var data = {
        "GYSMC": GYSMC,
        "LXR": LXR,
        "LXDH": LXDH,
        "YX": YX,
        "CZ": CZ,
        "DZ": DZ
    };

    $.ajax({
        url: "/Dict/Handler_InsertSupplier",
        type: 'POST',
        data: {
            "data": JSON.stringify(data)
        },
        beforeSend: ajaxLoading,
        success: function (data) {
            ajaxLoadEnd();
            var result
            if (data > 0) {
                try {
                    parent.$("#modalwindow").window("close");
                    parent.$("#datagrid_gys").datagrid('reload');
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
