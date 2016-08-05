$(function () {

    loadinitData()
});

function loadinitData()
{

    //加载属性类别
    var name = "SXLX";
    var url = "/Dict/load_CAttrType";
    load_combobox(name, url);
    loadDict();
}


function load_combobox(name,url)
{
    $("#" + name).combobox({
        valueField: 'id',
        method: 'POST',
        textField: 'text',
        url:url,
        onSelect: function (rec) {
            $('#' + name).combobox('setValue', rec.id);
            $('#' + name).combobox('setText', rec.text);

            //针对该页面定制化数据
            if (name == "SXLX")
            {
                opendicType();
            }
           
            //$("description_sxlx").val(rec.description)
        },
        onLoadSuccess: function () {
            var data = $('#' + name).combobox('getData');
            if (data.length > 0) {
                $('#' + name).combobox('select', data[0].id);
            }
        }
    });
}

function opendicType()
{
    //获取选中值
    var sxlx = $("#SXLX").combobox("getValue");
    var sxlx_name = $("#SXLX").combobox("getText");
    //这里可以优化成读数据看书否需要进行  
    if (sxlx_name == "字典类型") {
        $("#dicType").show(100);
    } else {
        $("#dicType").hide(100);
    }
}


function loadDict()
{
    $("#GLZD").combobox({
        valueField: 'ID',
        method: 'POST',
        editable: false,
        textField: 'name_para',
        url: '/Dict/load_dict_CAttr',
        onSelect: function (rec) {
            $('#GLZD').combobox('setValue', rec.ID);
            $('#GLZD').combobox('setText', rec.name_para);
        }
    });
}


function submitForm(id) {

    //获取数据
    var xtid = $("#XTID").val();
    var sxbt = $("#SXBT").val();
    var zdcd = $("#ZDCD").val()

    var sfbs = $("#SFBS").is(':checked');
    var sxlx = $("#SXLX").combobox("getValue");
    var glzd = $("#GLZD").combobox("getValue");
    //alert(sfbs);


    var data = {
        "xtid": xtid,
        "sxbt": sxbt,
        "zdcd": zdcd,
        "sfbs": sfbs,
        "zclb": id,
        "glzd": glzd,
        "sxlx": sxlx,
        "flag":true
    };
    //alert(JSON.stringify(data));
    $.ajax({
        url: "/Dict/Handler_InsertCAttr",
        type: 'POST',
        data: {
            "data": JSON.stringify(data),
        },
        beforeSend: ajaxLoading,
        success: function (data) {
            ajaxLoadEnd();
            var result
            if (data > 0) {
                try {
                    parent.$("#modalwindow").window("close");
                    parent.$("#datagrid_current").datagrid('reload');
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

