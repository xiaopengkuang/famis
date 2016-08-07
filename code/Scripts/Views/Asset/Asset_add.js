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

    $('#tabs_add').tabs('disableTab', "附件设备");
    $('#tabs_add').tabs('disableTab', "附属文件");
    $('#tabs_add').tabs('disableTab', "附属图片");
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
    ajaxLoading();
    load_GYS_add();
    load_ZJFS_add();
    load_JLDW_add();
    load_SZBM_add();
    load_ZCLB_add();
    load_GZRQ_add();
    load_CFDD_add();
    load_Other_ZJFS_add();
    ajaxLoadEnd();
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
           //更新资产型号
           load_ZCXH_add(node.id);
           //更新资产性质
           updateZCXZ(tree, node);
           //更新资产自定义属性
           update_ZDY_attr(node.id);
           //更新折旧年限、折旧率
           update_infoZJ(node.id);
       }, //全部折叠
       onLoadSuccess: function (node, data) {
           var ctree = $('#ZCLB_add').combotree('tree');
           var roots = ctree.tree("getRoots");
           if (roots.length > 0) {
               $('#ZCLB_add').combotree('setValue', roots[0].id);
           }
       }
   });
}

function update_infoZJ(id_zclb)
{
    //获取该类别有哪些属性

    $.ajax({
        url: "/Dict/Handler_load_infoZJ?id_zclb=" + id_zclb,
        type: 'POST',
        beforeSend: ajaxLoading,
        success: function (data) {
            ajaxLoadEnd();
            if (data) {
                $('#Other_SYNX_add').numberspinner("setValue", data.period_Depreciation);
                $('#Other_JCZL_add').val(data.Net_residual_rate);
            } else {
                $('#Other_SYNX_add').numberspinner("setValue", 0);
                $('#Other_JCZL_add').val("0");
            }
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
        td1.style.width = "80px";
        var td2 = document.createElement("td");
        td2.style.width = "170px";

        var lable_item = document.createElement("label");
        lable_item.innerHTML = data[i].title + ":";
        var input_item = document.createElement("input");
        input_item.id = "CAttr_INPUT_" + data[i].ID;
        input_item.style.width = "170px";
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
        cattr_item.title = data[i].title;
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
            //设置成Str
            initNumberStrBox(id_a);  //设置为数字字符串类型
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

function initNumberStrBox(id_Numbox)
{
    //$('#'+id_Numbox).numberbox({
    //    min: 0,
    //    precision: 2
    //});
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
        },
        onLoadSuccess: function () {
            //var data = $('#SYRY_add').combobox('getData');
            //if (data.length > 0) {
            //    $('#SYRY_add').combobox('select', data[0].id);
            //}
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
        },
        onLoadSuccess: function () {
            var data = $('#JLDW_add').combobox('getData');
            if (data.length > 0) {
                $('#JLDW_add').combobox('select', data[0].ID);
            }
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
          var ctree = $('#CFDD_add').combotree('tree');
          var roots = ctree.tree("getRoots");
          if (roots.length > 0) {
              $('#CFDD_add').combotree('setValue', roots[0].id);
          }
      }
  });
}

function myformatter(date) {
    var y = date.getFullYear();
    var m = date.getMonth() + 1;
    var d = date.getDate();
    return y + '-' + (m < 10 ? ('0' + m) : m) + '-' + (d < 10 ? ('0' + d) : d);
}

function myparser(s) {
    if (!s) return new Date();
    var ss = (s.split('-'));
    var y = parseInt(ss[0], 10);
    var m = parseInt(ss[1], 10);
    var d = parseInt(ss[2], 10);
    if (!isNaN(y) && !isNaN(m) && !isNaN(d)) {
        return new Date(y, m - 1, d);
    } else {
        return new Date();
    }
}

function load_GZRQ_add() {
    var curr_time = new Date();
    $("#GZRQ_add").datebox("setValue", myformatter(curr_time));
   
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
        },
        onLoadSuccess: function () {
            //var dg = $('#GYS_add').combogrid('grid');
            //var rows = dg.datagrid("getRows");
            //if (rows.length > 0)
            //{
            //    $('#GYS_add').combogrid('setValue', rows[0].ID);
            //    $('#GYS_add').combogrid('setText', rows[0].name_supplier);
            //    $("#LXR_add").val(rows[0].linkman);
            //    $("#GYSDD_add").val(rows[0].address);
            //}
            
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
        },
        onLoadSuccess: function () {
            var data = $('#ZJFS_add').combobox('getData');
            if (data.length > 0)
            {
                $('#ZJFS_add').combobox('select', data[0].ID);
            }
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
        },
        onLoadSuccess: function () {
            var data = $('#ZCXH_add').combobox('getData');
            if (data.length > 0)
            {
                $('#ZCXH_add').combobox('select', data[0].ZCXH);
            }

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
        },
        onLoadSuccess: function () {
            var data = $('#Other_ZJFS_add').combobox('getData');
            if (data.length > 0) {
                $('#Other_ZJFS_add').combobox('select', data[0].ID);
            }
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
    //var d_ZCXZ_ID_add;
    //if (d_ZCLB_add == "" || d_ZCLB_add == null) {

    //    d_ZCLB_add = $("#ZCLB_add").combotree("getValue");
    //    if (d_ZCLB_add == "" || d_ZCLB_add == undefined) {
    //        //$.messager.alert('提示', "调皮的肖金龙", 'info'); return;
    //    }
    //}
    d_ZCLB_add = $("#ZCLB_add").combotree("getValue");

    var d_ZCMC_add = $("#ZCMC_add").val();

    var d_ZCXH_add = $('#ZCXH_add').combobox("getValue");
    var d_JLDW_add = $("#JLDW_add").combobox("getValue");

    d_SZBM_add = $("#SZBM_add").combotree("getValue");

    //if (d_SZBM_add == "" || d_SZBM_add == null) {

    //    d_SZBM_add = $("#SZBM_add").combotree("getValue");
    //    if (d_SZBM_add == "" || d_SZBM_add == undefined) {
    //    }
    //}


    var d_SYRY_add = $("#SYRY_add").combobox("getValue");

    //if (d_CFDD_add == "" || d_CFDD_add == null) {
    //    d_CFDD_add = $("#CFDD_add").combotree("getValue");
    //    if (d_CFDD_add == "" || d_CFDD_add == undefined) {
    //    }
    //}
    d_CFDD_add = $("#CFDD_add").combotree("getValue");

    //d_CFDD_add = d_CFDD_add == "" ? -1 : parseInt(d_CFDD_add);


    var d_ZJFS_add = $("#ZJFS_add").combobox("getValue") == "" ? -1 : parseInt($("#ZJFS_add").combobox("getValue"));

    var d_GYS_add = $("#GYS_add").combogrid("getValue");

    var d_LXR_add = $("#LXR_add").val();

    var d_GZRQ_add = $('#GZRQ_add').datebox('getValue');

    var d_GYSDD_add = $("#GYSDD_add").val();

    var d_note_add = $("#note_add").val();
    var code_oldSYS = $("#code_oldSYS").val();

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
        "d_Other_ZCJZ_add": d_Other_ZCJZ_add,
        "d_note_add": d_note_add,
        "code_oldSYS": code_oldSYS
        //"d_Other_YTZJ_add": d_Other_YTZJ_add,
        //"d_Other_LJZJ_add": d_Other_LJZJ_add,
        //"d_Other_JZ_add": d_Other_JZ_add
    };

    var data_cattr = new Array();
    var data_cattr_STR = "";
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
                data_cattr_STR = data_cattr_STR + "{'ID_customAttr':" + new_attr.ID_customAttr + ",'value':" + new_attr.value + "}";

            }; break;
            case 2: {
                //valuesss += arry_CAttr[i].id_cattr + "-" + $("#" + arry_CAttr[i].id).combobox("getValue") + "\t";
                new_attr.value = $("#" + arry_CAttr[i].id).combobox("getValue");
                data_cattr_STR = data_cattr_STR + "{'ID_customAttr':" + new_attr.ID_customAttr + ",'value':" + new_attr.value + "}";
            }; break;
            case 3: {
                //valuesss += arry_CAttr[i].id_cattr + "-" + $("#" + arry_CAttr[i].id).val() + "\t";
                new_attr.value = "'" + $("#" + arry_CAttr[i].id).val() + "'";
                data_cattr_STR = data_cattr_STR + "{'ID_customAttr':" + new_attr.ID_customAttr + ",'value':'" + new_attr.value + "'}";
            }; break;
            default:;break;
        }
        data_cattr.push(JSON.stringify(new_attr));
    }



    $.ajax({
        url: "/Asset/Handler_addNewAsset",
        type: 'POST',
        dataType:"json",
        data: {
            "Asset_add": JSON.stringify(Asset_add),
            "data_cattr":JSON.stringify(data_cattr)
            //"data_cattr": data_cattr_STR
        },
        beforeSend: ajaxLoading,
        success: function (data) {
            ajaxLoadEnd();
            var result
            if (data > 0) {
                parent.$("#TableList_0_1").datagrid("reload");
                parent.$("#modalwindow").window("close");
            }  else if(data==-11) {
                MessShow("请勿插入重复编号！");
            }else {
                result = "保存数据失败！";
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

function checkFormat() {
    //基础属性
    var check_obj_ZCLB = $("#ZCLB_add").combotree("getValue");
    var check_obj_ZCMC = $("#ZCMC_add").val();
    var check_obj_ZCXH = $("#ZCXH_add").combobox("getValue");
    var check_obj_CFDD = $("#CFDD_add").combotree("getValue");
    var check_obj_GYS = $("#GYS_add").combogrid("getValue");
    //其他属性
    var check_obj_Other_SYNX = $("#Other_SYNX_add").val();
    var check_obj_Other_JCZL = $("#Other_JCZL_add").val();
    var check_obj_Other_ZCDJ = $("#Other_ZCDJ_add").val();
    var check_obj_Other_ZCSL = $("#Other_ZCSL_add").val();

    if (isNull(check_obj_ZCLB)) {
        MessShow("资产类别不能为空");
        return;
    } else if (isNull(check_obj_ZCMC)) {
        MessShow("资产名称不能为空");
        return;
    } else if (isNull(check_obj_ZCXH)) {
        MessShow("规格型号不能为空");
        return;
    } else if (isNull(check_obj_CFDD)) {
        MessShow("存放地点不能为空");
        return;
    } else if (isNull(check_obj_GYS)) {
        MessShow("供应商不能为空");
        return;
    } else if (isNull(check_obj_Other_JCZL)) {
        MessShow("净残值率不能为空");
        return;
    } else if (check_obj_Other_ZCDJ <= 0) {
        MessShow("资产单价只能为正值");
        return;
    } else if (check_obj_Other_ZCSL <= 0) {
        MessShow("资产数量只能为正值");
        return;
    } else if (check_obj_Other_SYNX <= 0) {
        MessShow("使用年限只能为正值");
        return;
    }

    //自定义属性
    for (var i = 0; i < arry_CAttr.length; i++) {
        var new_attr = new Object();
        new_attr.ID_customAttr = arry_CAttr[i].id_cattr;
        switch (arry_CAttr[i].InputType) {
            case 1: {
                //valuesss += arry_CAttr[i].id_cattr + "-" + $("#" + arry_CAttr[i].id).combotree("getValue") + "\t";
                new_attr.value = $("#" + arry_CAttr[i].id).combotree("getValue");
                if (arry_CAttr[i].necessary == true && isNull(new_attr.value)) {
                    MessShow(arry_CAttr[i].title + "不能为空");
                    return;
                }
                //alert(new_attr.value);
            }; break;
            case 2: {
                //valuesss += arry_CAttr[i].id_cattr + "-" + $("#" + arry_CAttr[i].id).combobox("getValue") + "\t";
                new_attr.value = $("#" + arry_CAttr[i].id).combobox("getValue");
                if (arry_CAttr[i].necessary == true && isNull(new_attr.value)) {
                    MessShow(arry_CAttr[i].title + "不能为空");
                    return;
                }
                //alert(new_attr.value);
            }; break;
            case 3: {
                //valuesss += arry_CAttr[i].id_cattr + "-" + $("#" + arry_CAttr[i].id).val() + "\t";
                new_attr.value = $("#" + arry_CAttr[i].id).val();
                if (arry_CAttr[i].necessary == true && isNull(new_attr.value)) {
                    MessShow(arry_CAttr[i].title + "不能为空");
                    return;
                }
                //alert(new_attr.value);
            }; break;
            default:; break;
        }
    }

    submitForm();
   
}



function cancelFormSubmit()
{
    try {
        parent.$("#modalwindow").window("close");
    } catch (e) {

    }
}
//判值是否为空
function isNull(data) {
    return (data == "" || data == undefined || data == null) ? true : false;
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

