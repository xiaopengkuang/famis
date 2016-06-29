$(function () {

    loadintiData();

});




function loadintiData()
{

    loadinit_JLDW();
    loadinit_ZJFS();

}


function loadinit_JLDW()
{
    $("#JLDW").combobox({
        valueField: 'ID',
        method: 'get',
        textField: 'name_para',
        url: '/Dict/load_FS_add?TypeID=2',
        onSelect: function (rec) {
            $('#JLDW').combobox('setValue', rec.ID);
            $('#JLDW').combobox('setText', rec.name_para);
        },
        onLoadSuccess: function ()
        {
            var data = $('#JLDW').combobox('getData');
            if (data.length > 0) {
                $('#JLDW').combobox('select', data[0].ID);
            }
        }
    });
}

function loadinit_ZJFS()
{
    $("#ZJFS").combobox({
        valueField: 'ID',
        method: 'get',
        textField: 'name_para',
        url: '/Dict/load_FS_add?TypeID=10',
        onSelect: function (rec) {
            $('#ZJFS').combobox('setValue', rec.ID);
            $('#ZJFS').combobox('setText', rec.name_para);
        },
        onLoadSuccess: function () {
            var data = $('#ZJFS').combobox('getData');
            if (data.length > 0) {
                $('#ZJFS').combobox('select', data[0].ID);
            }
        }
    });
}


function submitForm()
{
    //获取数据

    var lbbh = $("#LBBH").val();
    var lbmc = $("#LBMC").val();
    var zjnx = $("#ZJNX").val();
    var jczl = $("#JCZL").val();
    var level=$("#level").val()

    var sjlb = $("#SJLB").combobox("getValue");
    var jldw = $("#JLDW").combobox("getValue");
    var zjfs = $("#ZJFS").combobox("getValue");

    var data = {
        "lbbh": lbbh,
        "lbmc": lbmc,
        "zjnx": zjnx,
        "level":level,
        "jczl": jczl,
        "sjlb": sjlb,
        "jldw": jldw,
        "zjfs": zjfs
    };
    $.ajax({
        url: "/Dict/Handler_addNewAssetType",
        type: 'POST',
        data: {
            "data": JSON.stringify(data)
        },
        beforeSend: ajaxLoading,
        success: function (data) {
            ajaxLoadEnd();
            var result
            if (data > 0) {
                parent.$("#modalwindow").window("close");
                parent.$("#treegrid").treegrid("reload");
            } else {
                result = "系统正忙，请稍后继续！";
                $.messager.alert('警告', result, 'warning');
            }
        }
    });
    //alert(JSON.stringify(data));

}

function cancelForm()
{
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

