$(function () {
    loadintiData();
});

function loadintiData() {

    loadinit_JLDW();
    loadinit_ZJFS();
}


function loadinit_JLDW() {
    $("#JLDW").combobox({
        valueField: 'ID',
        method: 'POST',
        textField: 'name_para',
        url: '/Dict/load_FS_add?nameFlag=2_JLDW',
        onSelect: function (rec) {
            $('#JLDW').combobox('setValue', rec.ID);
            $('#JLDW').combobox('setText', rec.name_para);
        },
        onLoadSuccess: function () {
            //var data = $('#JLDW').combobox('getData');
            //if (data.length > 0) {
            //    $('#JLDW').combobox('select', data[0].ID);
            //}
        }
    });
}

function loadinit_ZJFS() {
    $("#ZJFS").combobox({
        valueField: 'ID',
        method: 'POST',
        textField: 'name_para',
        url: '/Dict/load_FS_add?nameFlag=2_ZJFS_JIU',
        onSelect: function (rec) {
            $('#ZJFS').combobox('setValue', rec.ID);
            $('#ZJFS').combobox('setText', rec.name_para);
        },
        onLoadSuccess: function () {
            //var data = $('#ZJFS').combobox('getData');
            //if (data.length > 0) {
            //    $('#ZJFS').combobox('select', data[0].ID);
            //}
        }
    });
}


function dataBind(id)
{
    //alert(id);
    $.ajax({
        url: "/Dict/Handler_GetAssetType",
        type: 'POST',
        data: {
            "id": id
        },
        beforeSend: ajaxLoading,
        success: function (data) {
            ajaxLoadEnd();
            if (data != null)
            {
             
                //绑定数据
                $("#LBBH").val(data[0].lbbh);
                $("#LBMC").val(data[0].lbmc);

                $("#ZJNX").numberbox("setValue", data[0].zjnx)//赋值
                $("#JCZL").numberbox("setValue", data[0].jczl)//赋值
                if (data[0].sjlb == null || data[0].sjlb == "") {
                    $("#SJLB").combobox("setValue", 0);
                    $("#SJLB").combobox("setText", "");
                } else {
                    $("#SJLB").combobox("setValue", data[0].sjlb);
                    $("#SJLB").combobox("setText", data[0].sjlb);
                }
                
                $("#SJLB").combobox("disable");
                $("#JLDW").combobox("select", data[0].jldw);
                $("#ZJFS").combobox("select", data[0].zjfs);



            }
        }
    });
}



function submitForm(id) {
    //获取数据

    //var lbbh = $("#LBBH").val();
    var lbmc = $("#LBMC").val();
    var zjnx = $("#ZJNX").val();
    var jczl = $("#JCZL").val();

    //var sjlb = $("#SJLB").combobox("getValue");
    var jldw = $("#JLDW").combobox("getValue");
    var zjfs = $("#ZJFS").combobox("getValue");

    var data = {
        //"lbbh": lbbh,
        "lbmc": lbmc,
        "zjnx": zjnx,
        "jczl": jczl,
        //"sjlb": sjlb,
        "jldw": jldw,
        "zjfs": zjfs
    };
    $.ajax({
        url: "/Dict/Handler_updateAssetType",
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
                parent.$("#modalwindow").window("close");
                parent.$("#treegrid").treegrid('reload');
            } else if (data == -2) {
                MessShow("存在同名称的资产类别！");
            } else {
                MessShow("插入数据失败，请稍后再试！");
            }
        }
    });
    //alert(JSON.stringify(data));

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
