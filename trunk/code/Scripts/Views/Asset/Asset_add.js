//==================================global data==========================================//
var d_SZBM_add;
var d_ZCLB_add;
var d_CFDD_add;
var d_Other_SYNX_add = 0;
var d_Other_ZCDJ_add = 0;
var d_Other_ZCSL_add = 0;

var arry_CAttr = new Array();

//==================================global data==========================================//

$(function () {

    loadInitDate();
    $("#Num_PLTJ_add").hide();
    $("#LABEL_PL").hide();


    $('#Other_ZCDJ_add').numberbox({
        onChange: function () {
            update_Value(); //设置折扣率
        }
    });

    $('#Other_ZCSL_add').numberbox({
        onChange: function () {
            update_Value(); //设置折扣率
        }
    });
    update_Value();


});

function update_Value()
{
    var sl = $('#Other_ZCSL_add').numberbox('getValue')
    var dj = $('#Other_ZCDJ_add').numberbox('getValue')
    var value_f = parseFloat(sl) * parseFloat(dj);
    $('#Other_ZCJZ_add').val(value_f);
}
 
function loadInitDate() {
    load_GYS_add();
    load_ZJFS_add();
    load_JLDW_add();
    load_SZBM_add();
    load_ZCLB_add();
    load_CFDD_add();
    load_Other_ZJFS_add();

}



function load_ZCLB_add() {
    $('#ZCLB_add').combotree
   ({
       url: '/Dict/load_ZCLB',
       valueField: 'id',
       textField: 'nameText',
       required: true,
       method: 'POST',
       editable: false,
       //选择树节点触发事件  
       onSelect: function (node) {
           //返回树对象  
           var tree = $(this).tree;
           load_ZCXH_add(node.id);
           updateZCXZ(tree, node);
           update_ZDY_attr(node.id)
       }, //全部折叠
       onLoadSuccess: function (node, data) {
           $('#ZCLB_add').combotree('tree').tree("collapseAll");
       }
   });
}

function update_ZDY_attr(id_zclb)
{
    //获取该类别有哪些属性

    $.ajax({
        url: "/Dict/Handler_loadCAttr?id="+id_zclb,
        type: 'POST',
        beforeSend: ajaxLoading,
        success: function (data) {
            ajaxLoadEnd();
            if (data) {
                initAttr(data);
            } else {
                clearCAttr();
            }
        }
    });
}

function initAttr(data) {
    document.getElementById('CATTR').innerHTML = '';
    var panelDIV = document.getElementById("CATTR");
    var row = Math.ceil(data.length / 3);
    var endIndex = data.length % 3;
    var table = document.createElement("table"); 
    var tbody = document.createElement("tbody");
    arry_CAttr = new Array();
    for (var i = 0; i < data.length; i++) {
        var tr;
        if (i % 3 == 0)
        {
             tr = document.createElement("tr");
        }
        var td1 = document.createElement("td");
        td1.style.width = "50px";
        var td2 = document.createElement("td");
        td2.style.width = "200px";

        var lable_item = document.createElement("label");
        lable_item.innerHTML = data[i].title + ":";
        var input_item = document.createElement("input");
        input_item.id = "CAttr_INPUT_" + data[i].ID;
        input_item.style.width = "200px";
        td1.appendChild(lable_item);
        td2.appendChild(input_item);
        tr.appendChild(td1);
        tr.appendChild(td2);
        if ((i + 1) % 3 == 0 || (i + 1) == data.length) {
            tbody.appendChild(tr);
        }
    }
    table.appendChild(tbody);
    panelDIV.appendChild(table);
    for (var i = 0; i < data.length; i++) {
        var id_a = "CAttr_INPUT_" + data[i].ID;
        var cattr_item = new Object();
        cattr_item.id = id_a;
        cattr_item.isTree = data[i].isTree;
        cattr_item.necessary = data[i].necessary;
        cattr_item.id_cattr = data[i].ID;
        cattr_item.type = data[i].type;
        cattr_item.type_value = data[i].type_value;
        cattr_item.type_Name = data[i].type_Name;

        //判断数据类型
        if (data[i].type_Name == "字典类型") {
            //获取其数据结构类型
            if (data[i].isTree != null && data[i].isTree == true) {
                initcombotree(id_a, data[i].type_value, data[i].necessary);
                cattr_item.InputType = 1;
            } else {
                initCombobox(id_a, data[i].type_value, data[i].necessary);
                cattr_item.InputType = 2;
            }
        } else {
            //设置成number
            initNumberBox(id_a);
            cattr_item.InputType = 3;
        }
      
       

        arry_CAttr.push(cattr_item);
    }

}

function initCombobox(id_combobox, id_dic, requiredFlag)
{
    $("#" + id_combobox).combobox({
        valueField: 'ID',
        required: requiredFlag,
        method: 'POST',
        editable: false,
        textField: 'name_para',
        url: '/Dict/load_dict_combobox?id='+id_dic,
        onSelect: function (rec) {
            $('#' + id_combobox).combobox('setValue', rec.ID);
            $('#' + id_combobox).combobox('setText', rec.name_para);
        }
    });
}


function initcombotree(id_combotree, id_dic,requiredFlag)
{
    $('#' + id_combotree).combotree
  ({
      url: '/Dict/load_dict_combotree?id='+id_dic,
      valueField: 'id',
      textField: 'nameText',
      required: requiredFlag,
      method: 'POST',
      editable: false,
      //选择树节点触发事件  
      onSelect: function (node) {
      }, //全部折叠
      onLoadSuccess: function (node, data) {
          $('#' + id_combotree).combotree('tree').tree("collapseAll");
      }
  });
}

function initNumberBox(id_Numbox)
{
    $('#'+id_Numbox).numberbox({
        min: 0,
        precision: 2
    });
}





function clearCAttr()
{
    document.getElementById('CATTR').innerHTML = '';
    var panelDIV = document.getElementById("CATTR");
    var childs = panelDIV.childNodes;
    for (var i = 0; i < childs.length; i++)
    {
        panelDIV.removeChild(childs[i]);
    }
}


function updateZCXZ(tree,node)
{
    d_ZCLB_add = $('#ZCLB_add').combotree("getValue");
    //资产类别
    //资产性质
    var parent = node;
    var tree = $('#ZCLB_add').combotree('tree');
    var path = new Array();
    while (parent) {
        path.unshift(parent.text);
        var parent = tree.tree('getParent', parent.target);
    };
    //获得根节点
    var rootText = "";
    if (path.length > 0) {
        rootText = path[0];
    }
    $('#ZCXZ_add').val(rootText);
}

function load_User_add(id_DP) {
    $("#SYRY_add").combobox({
        valueField: 'id',
        method: 'POST',
        editable: false,
        textField: 'name',
        url: '/Dict/load_User_add?id_DP=' + id_DP,
        onSelect: function (rec) {
            $('#SYRY_add').combobox('setValue', rec.id);
            $('#SYRY_add').combobox('setText', rec.name);
        }
    });

}


function load_JLDW_add() {
    $("#JLDW_add").combobox({
        valueField: 'ID',
        method: 'POST',
        editable: false,
        textField: 'name_para',
        url: '/Dict/load_FS_add?nameFlag=2_JLDW',
        onSelect: function (rec) {
            $('#JLDW_add').combobox('setValue', rec.ID);
            $('#JLDW_add').combobox('setText', rec.name_para);
        }
    });
}


function load_SZBM_add() {
    $('#SZBM_add').combotree
    ({
        url: '/Dict/load_SZBM',
        valueField: 'id',
        textField: 'nameText',
        required: true,
        method: 'POST',
        editable: false,
        //选择树节点触发事件  
        onSelect: function (node) {
            d_SZBM_add = $('#SZBM_add').combotree('getValue');
            load_User_add(node.id);
        }, //全部折叠
        onLoadSuccess: function (node, data) {
            //$('#SZBM_add').combotree('tree').tree("collapseAll");
        }
    });

}
function load_CFDD_add() {
    $('#CFDD_add').combotree
  ({
      url: '/Dict/load_DictTree?nameFlag=2_CFDD',
      valueField: 'id',
      textField: 'nameText',
      required: true,
      method: 'POST',
      editable: false,
      //选择树节点触发事件  
      onSelect: function (node) {
          //返回树对象  
          var tree = $(this).tree;
          //选中的节点是否为叶子节点,如果不是叶子节点,清除选中  
          d_CFDD_add = $('#CFDD_add').combotree('getValue');
          //
      }, //全部折叠
      onLoadSuccess: function (node, data) {
          $('#CFDD_add').combotree('tree').tree("collapseAll");
      }
  });
}


function load_GYS_add() {
    $('#GYS_add').combogrid({
        panelWidth: 300,
        value: '006',
        idField: 'code',
        editable: false,
        textField: 'name',
        url: '/Dict/load_GYS_add',
        method: 'POST', //默认是post,不允许对静态文件访问
        columns: [[
        { field: 'ID', checkbox: true, title: 'ID', width: 99, hidden: true },
        { field: 'name_supplier', title: '供应商', width: 99 },
        { field: 'linkman', title: '联系人', width: 99 },
        { field: 'address', title: '地址', width: 99 }
        ]],
        onClickRow: function (index, row) {
            //search = false;
            $('#GYS_add').combogrid('hidePanel');
            $('#GYS_add').combogrid('setValue', row.ID);
            $('#GYS_add').combogrid('setText', row.name_supplier);
            $("#LXR_add").val(row.linkman);
            $("#GYSDD_add").val(row.address);
            //setTimeout(function () {
            //    search = true;
            //}, 1000);

        }
    });
}
function load_ZJFS_add() {
    $("#ZJFS_add").combobox({
        valueField: 'ID',
        method: 'POST',
        editable: false,
        textField: 'name_para',
        url: '/Dict/load_FS_add?nameFlag=2_ZJFS_JIA',
        onSelect: function (rec) {
            $('#ZJFS_add').combobox('setValue', rec.ID);
            $('#ZJFS_add').combobox('setText', rec.name_para);
        }
    });
}

//加载规格型号
function load_ZCXH_add(assetType) {
    $("#ZCXH_add").combobox({
        valueField: 'ZCXH',
        method: 'POST',
        textField: 'ZCXH',
        url: '/Dict/load_ZCXH_add?assetType=' + assetType,
        onSelect: function (rec) {
            $('#ZCXH_add').combobox('setValue', rec.ZCXH);
            $('#ZCXH_add').combobox('setText', rec.ZCXH);
        }
    });
}

function load_Other_ZJFS_add() {
    $("#Other_ZJFS_add").combobox({
        valueField: 'ID',
        method: 'POST',
        editable: false,
        textField: 'name_para',
        url: '/Dict/load_FS_add?nameFlag=2_ZJFS_JIU',
        onSelect: function (rec) {
            $('#Other_ZJFS_add').combobox('setValue', rec.ID);
            $('#Other_ZJFS_add').combobox('setText', rec.name_para);
        }
    });
}








function showAreaNum_PLTJ_add() {
    $("#Num_PLTJ_add").toggle();
    $("#LABEL_PL").toggle();

}





function submitForm() {

    //============================基础属性========================================//

    //获取资产编号
    var d_ZCBH_add = $("#ZCBH_add").val();
    //获取资产性质
    var zcxz = $("#ZCXZ_add").val();
    var d_ZCXZ_ID_add;
    if (d_ZCLB_add == "" || d_ZCLB_add == null) {

        d_ZCLB_add = $("#ZCLB_add").combotree("getValue");
        if (d_ZCLB_add == "" || d_ZCLB_add == undefined) {
            //$.messager.alert('提示', "调皮的肖金龙", 'info'); return;
        }
    }
    var d_ZCMC_add = $("#ZCMC_add").val();

    var d_ZCXH_add = $('#ZCXH_add').combobox("getValue");
    var d_JLDW_add = $("#JLDW_add").combobox("getValue");


    if (d_SZBM_add == "" || d_SZBM_add == null) {

        d_SZBM_add = $("#SZBM_add").combotree("getValue");
        if (d_SZBM_add == "" || d_SZBM_add == undefined) {
        }
    }


    var d_SYRY_add = $("#SYRY_add").combobox("getValue");

    if (d_CFDD_add == "" || d_CFDD_add == null) {
        d_CFDD_add = $("#CFDD_add").combotree("getValue");
        if (d_CFDD_add == "" || d_CFDD_add == undefined) {
        }
    }
    d_CFDD_add = d_CFDD_add == "" ? -1 : parseInt(d_CFDD_add);


    var d_ZJFS_add = $("#ZJFS_add").combobox("getValue") == "" ? -1 : parseInt($("#ZJFS_add").combobox("getValue"));

    var d_GYS_add = $("#GYS_add").combogrid("getValue");

    var d_LXR_add = $("#LXR_add").val();

    var d_GZRQ_add = $('#GZRQ_add').datebox('getValue');

    var d_GYSDD_add = $("#GYSDD_add").val();

    var d_Check_PLZJ_add = $("#Num_PLTJ_add").is(":hidden");  //true表示未被选选中  false表示选择

    var d_Num_PLTJ_add = d_Check_PLZJ_add == true ? 1 : $("#Num_PLTJ_add").val();

    //===============================其他属性===================================//


    d_Other_SYNX_add = $('#Other_SYNX_add').numberspinner('getValue')

    var d_Other_ZJFS_add = $("#Other_ZJFS_add").combobox('getValue');

    var d_Other_JCZL_add = $("#Other_JCZL_add").val();

    d_Other_ZCDJ_add = $('#Other_ZCDJ_add').numberspinner('getValue')

    d_Other_ZCSL_add = $('#Other_ZCSL_add').numberspinner('getValue')

    var d_Other_ZCJZ_add = $("#Other_ZCJZ_add").val();

    //var d_Other_YTZJ_add = $("#Other_YTZJ_add").val();;

    //var d_Other_LJZJ_add = $("#Other_LJZJ_add").val();

    //var d_Other_JZ_add = $("#Other_JZ_add").val();

    //===============================自定义属性===================================//

    //封装成json格式创给后台
    var Asset_add = {
        "d_ZCBH_add": d_ZCBH_add,
        "d_ZCLB_add": d_ZCLB_add,
        "d_ZCMC_add": d_ZCMC_add,
        "d_ZCXH_add": d_ZCXH_add,
        "d_JLDW_add": d_JLDW_add,
        "d_SZBM_add": d_SZBM_add,
        "d_SYRY_add": d_SYRY_add,
        "d_ZJFS_add": d_ZJFS_add,
        "d_GYS_add": d_GYS_add,
        "d_GZRQ_add": d_GZRQ_add,
        "d_Check_PLZJ_add": d_Check_PLZJ_add,
        "d_Num_PLTJ_add": d_Num_PLTJ_add,
        "d_CFDD_add": d_CFDD_add,
        "d_Other_SYNX_add": d_Other_SYNX_add,
        "d_Other_ZJFS_add": d_Other_ZJFS_add,
        "d_Other_JCZL_add": d_Other_JCZL_add,
        "d_Other_ZCDJ_add": d_Other_ZCDJ_add,
        "d_Other_ZCSL_add": d_Other_ZCSL_add,
        "d_Other_ZCJZ_add": d_Other_ZCJZ_add
        //"d_Other_YTZJ_add": d_Other_YTZJ_add,
        //"d_Other_LJZJ_add": d_Other_LJZJ_add,
        //"d_Other_JZ_add": d_Other_JZ_add
    };

    var data_cattr = new Array();
    //获取自定义属性
    for (var i = 0; i < arry_CAttr.length;i++)
    {
        var new_attr = new Object();
        new_attr.ID_customAttr = arry_CAttr[i].id_cattr;
        switch (arry_CAttr[i].InputType)
        {
            case 1: {
                //valuesss += arry_CAttr[i].id_cattr + "-" + $("#" + arry_CAttr[i].id).combotree("getValue") + "\t";
                new_attr.value = $("#" + arry_CAttr[i].id).combotree("getValue");
            }; break;
            case 2: {
                //valuesss += arry_CAttr[i].id_cattr + "-" + $("#" + arry_CAttr[i].id).combobox("getValue") + "\t";
                new_attr.value = $("#" + arry_CAttr[i].id).combobox("getValue");
            }; break;
            case 3: {
                //valuesss += arry_CAttr[i].id_cattr + "-" + $("#" + arry_CAttr[i].id).val() + "\t";
                new_attr.value = $("#" + arry_CAttr[i].id).val();
            }; break;
            default:;break;
        }
        data_cattr.push(JSON.stringify(new_attr));
    }



    $.ajax({
        url: "/Asset/Handler_addNewAsset",
        type: 'POST',
        data: {
            "Asset_add": JSON.stringify(Asset_add),
            "data_cattr":JSON.stringify(data_cattr)
        },
        beforeSend: ajaxLoading,
        success: function (data) {
            ajaxLoadEnd();
            var result
            if (data > 0) {
                parent.$("#modalwindow").window("close");
                parent.reloadDataGrid();
            } else {
                result = "系统正忙，请稍后继续！";
                $.messager.alert('警告', result, 'warning');
            }


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

