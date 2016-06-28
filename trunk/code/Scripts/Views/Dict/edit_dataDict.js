$(function () {

});


function dataBind(id)
{
    $.ajax({
        url: "/Dict/Handler_getDataDict",
        type: 'POST',
        data: {
            "id": id
        },
        beforeSend: ajaxLoading,
        success: function (data) {
            ajaxLoadEnd();
            if (data != null) {
                $("#CSMC").val(data[0].csmc);
                $("#CSLX").combobox("setValue", data[0].cslx);
                $("#CSLX").combobox("setText", data[0].cslx_name);
                if (data[0].isTree ==true) {
                    $("#SJJG_list").removeAttr("checked");
                    $("#SJJG_tree").attr("checked","checked");
                } else {
                    $("#SJJG_tree").removeAttr("checked");
                    $("#SJJG_list").attr("checked", "checked");
                }
            }
        }
    });
}


function submitForm(id) {

    //获取数据
    var csmc = $("#CSMC").val();

    var cslx = $("#CSLX").combobox("getValue");

    var radio=document.getElementsByName("SJJG");

    var selectvalue = null;   //  selectvalue为radio中选中的值



    for (var i = 0; i < radio.length; i++) {
        if (radio[i].checked == true) {
            selectvalue = radio[i].value;
            break;
        }
    }
    if (selectvalue == null)
    {
        $.messager.alert('警告', "请选择数据结构类型", 'warning');
        return;
    }

    var data = {
        "csmc": csmc,
        "cslx": cslx,
        "isTree": selectvalue == "1" ? true : false,
    };
    $.ajax({
        url: "/Dict/Handler_UpdatedataDic",
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
                    parent.loadInitData();
                } catch (e) {
                }
            } else {
                result = "系统正忙，请稍后继续！";
                $.messager.alert('警告', result, 'warning');
            }
        }
    });
}


function cancelForm() {
    parent.$("#modalwindow").window("close");
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

