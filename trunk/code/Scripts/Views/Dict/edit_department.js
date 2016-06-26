$(function () {

});



function dataBind(bmbh) {

    $.ajax({
        url: "/Dict/Handler_getDepartment",
        type: 'POST',
        data: {
            "bmbh": bmbh
        },
        beforeSend: ajaxLoading,
        success: function (data) {
            ajaxLoadEnd();
            if (data != null) {
                $("#BMBH").val(data[0].bmbh);
                $("#BMMC").val(data[0].bmmc);


                if (data[0].sjbm == null || data[0].sjbm == "") {

                    $("#SJBM").combobox("setValue", 0);
                    $("#SJBM").combobox("setText", "");
                } else {
                    $("#SJBM").combobox("setValue", data[0].sjbm);
                    $("#SJBM").combobox("setText", data[0].sjbm_Name);
                }



            }
        }
    });
}


function submitForm(id) {
    //获取数据

    //var bmbh = $("#BMBH").val();
    var bmmc = $("#BMMC").val();
    //var sjbm = $("#SJBM").combobox("getValue");

    var data = {
        //"bmbh": bmbh,
        "bmmc": bmmc,
        //"level": level,
        //"sjbm": sjbm
    };
    //alert(JSON.stringify(data));
    $.ajax({
        url: "/Dict/Handler_UpdateDepartmen",
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
                    parent.$("#treegrid_dp").treegrid('reload');
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
    parent.$("#treegrid").treegrid('reload');
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
