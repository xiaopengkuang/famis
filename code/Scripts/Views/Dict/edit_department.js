$(function () {
    load_SZBM_add();
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
                    //$("#SJBM").combobox("setValue", 0);
                    //$("#SJBM").combobox("setText", "");
                } else {
                    $("#SJBM").combotree("setValue", data[0].sjbm);
                    $("#SJBM").combotree("setText", data[0].sjbm_Name);
                }

            }
        }
    });
}

function load_SZBM_add() {
    $('#SJBM').combotree
    ({
        url: '/Dict/load_SZBM',
        valueField: 'id',
        textField: 'nameText',
        required: true,
        method: 'POST',
        editable: false,
        //选择树节点触发事件  
        onSelect: function (node) {
        }, //全部折叠
        onLoadSuccess: function (node, data) {
        }
    });

}


function submitForm(id) {
    //获取数据

    //var bmbh = $("#BMBH").val();
    var bmmc = $("#BMMC").val();
    //var sjbm = $("#SJBM").combobox("getValue");
    var sjbm = $('#SJBM').combotree("getValue");

    var data = {
        //"bmbh": bmbh,
        "bmmc": bmmc,
        "sjbm": sjbm
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
            } else if (data == -2) {
                MessShow("已存在同名称部门！");
            } else if(data==-11)
            {
                MessShow("上级部门不能为该部门及其子部门");
            }else {
                MessShow("添加数据失败，请稍后继续！");
            }
        }
    });
}

function cancelForm() {
    parent.$("#modalwindow").window("close");
    parent.$("#treegrid").treegrid('reload');
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
