//==================================global data==========================================//
var d_SZBM_add;
var d_ZCLB_add;
var d_CFDD_add;
var d_Other_SYNX_add = 0;
var d_Other_ZCDJ_add = 0;
var d_Other_ZCSL_add = 0;
//==================================global data==========================================//



$(function () {

    loadInitDate();

    $("#Num_PLTJ_add").hide();

    $("#Other_ZCDJ_add").numberspinner({
        min: 0,
        editable: false,
        onChange: function (value) {
            $('#Other_ZCDJ_add').numberspinner('setValue', value);
            //d_Other_ZCDJ_add = value;

        }

    });


    $("#Other_SYNX_add").numberspinner({
        min: 0,
        editable: false,
        onChange: function (value) {
            $('#Other_SYNX_add').numberspinner('setValue', value);
            //d_Other_SYNX_add = value;
            //alert(  $('#Other_SYNX_add').numberspinner('getValue'));
        }

    });


    $("#Other_ZCSL_add").numberspinner({
        min: 0,
        editable: false,
        onChange: function (value) {
            $('#Other_ZCSL_add').numberspinner('setValue', value);
            //d_Other_ZCSL_add = value;
        }

    });
    //alert(111);
});

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
       method: 'get',
       editable: false,
       //选择树节点触发事件  
       onSelect: function (node) {
           //返回树对象  
           var tree = $(this).tree;
           //选中的节点是否为叶子节点,如果不是叶子节点,清除选中  
           var isLeaf = tree('isLeaf', node.target);
           if (!isLeaf) {
               //清除选中  
               $('#ZCLB_add').combotree('clear');
           } else {
               d_ZCLB_add = $('#ZCLB_add').combotree("getValue")
               //资产类别
               $('#ZCXH_add').combobox('clear');
               load_ZCXH_add(node.text);
           }


           //资产性质
           var parent = node;
           var tree = $('#ZCLB_add').combotree('tree');
           var path = new Array();

           while (parent) {
               path.unshift(parent.id + "-" + parent.text);

               var parent = tree.tree('getParent', parent.target);
           };


           //获得根节点
           var rootText = "";

           if (path.length > 0) {
               rootText = path[0];
           }

           //$.messager.alert('提示', rootText, 'warning');
           $('#ZCXZ_add').val(rootText.trim());


       }, //全部折叠
       onLoadSuccess: function (node, data) {
           $('#ZCLB_add').combotree('tree').tree("collapseAll");
       }
   });
}


function load_SYR_add(SZBM_ID) {

    $("#SYR_add").combobox({
        valueField: 'id',
        method: 'get',
        textField: 'name',
        url: '/Dict/load_SYR_add?SZBM_ID=' + SZBM_ID,
        onSelect: function (rec) {
            $('#SYR_add').combobox('setValue', rec.id);
            $('#SYR_add').combobox('setText', rec.name);
        }
    });

}


function load_JLDW_add() {
    $("#JLDW_add").combobox({
        valueField: 'ID',
        method: 'get',
        textField: 'name_para',
        url: '/Dict/load_FS_add?TypeID=2',
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
        method: 'get',
        editable: false,
        //选择树节点触发事件  
        onSelect: function (node) {
            //返回树对象  
            var tree = $(this).tree;
            //选中的节点是否为叶子节点,如果不是叶子节点,清除选中  
            var isLeaf = tree('isLeaf', node.target);
            if (!isLeaf) {
                //清除选中  
                $('#SZBM_add').combotree('clear');
            } else {
                d_SZBM_add = $('#SZBM_add').combotree('getValue');
                load_SYR_add(node.id);

            }
            //


        }, //全部折叠
        onLoadSuccess: function (node, data) {
            $('#SZBM_add').combotree('tree').tree("collapseAll");
        }
    });

}
function load_CFDD_add() {
    $('#CFDD_add').combotree
  ({
      url: '/Dict/load_CFDD_add?id_di=9',
      valueField: 'id',
      textField: 'nameText',
      required: true,
      method: 'get',
      editable: false,
      //选择树节点触发事件  
      onSelect: function (node) {
          //返回树对象  
          var tree = $(this).tree;
          //选中的节点是否为叶子节点,如果不是叶子节点,清除选中  
          var isLeaf = tree('isLeaf', node.target);
          if (!isLeaf) {
              //清除选中  
              $('#CFDD_add').combotree('clear');
          } else {
              d_CFDD_add = $('#CFDD_add').combotree('getValue');
          }
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
        textField: 'name',
        url: '/Dict/load_GYS_add',
        method: 'get', //默认是post,不允许对静态文件访问
        columns: [[
        { field: 'name_supplier', title: '供应商', width: 99 },
        { field: 'linkman', title: '联系人', width: 99 },
        { field: 'address', title: '地址', width: 99 }
        ]],
        onClickRow: function (index, row) {
            search = false;
            $('#GYS_add').combogrid('hidePanel');
            $('#GYS_add').combogrid('setValue', row.name_supplier);
            $('#GYS_add').combogrid('setText', row.name_supplier);
            $("#LXR_add").val(row.linkman);
            //$("#LXR_add").text()(row.linkman);
            $("#GYSDD_add").val(row.address);
            //$("#GYSDD_add").textbox('setText', row.address);
            setTimeout(function () {
                search = true;
            }, 1000);

        }
    });
}

function load_ZJFS_add() {
    $("#ZJFS_add").combobox({
        valueField: 'ID',
        method: 'get',
        textField: 'name_para',
        url: '/Dict/load_FS_add?TypeID=3',
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
        method: 'get',
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
        method: 'get',
        textField: 'name_para',
        url: '/Dict/load_FS_add?TypeID=10',
        onSelect: function (rec) {
            $('#Other_ZJFS_add').combobox('setValue', rec.ID);
            $('#Other_ZJFS_add').combobox('setText', rec.name_para);
        }
    });
}




function ReComputeAssetValue() {
    alert(12);
    //var dj_asset = ($("#Other_ZCSL_add").val());
    //alert(dj_asset);
    //var sl_asset = ($("#Other_ZCDJ_add").val());
    ////var jz_asset = dj_asset * sl_asset;
    //alert(jz_asset);
    //$("#Other_ZCJZ_add").val();
}


function loadNewAssetCode() {
    $.ajax({
        type: "POST",
        url: "/Common/GetOneSerialNumber?ruleType=ZC&num=1",
        data: "",
        success: function (data) {
            if (data == $("#ZCBH_add").val()) {
                $.messager.alert('提示', '已经是最新编号!无须重复刷新', 'info');
            } else {
                $("#ZCBH_add").val(data);
            }
        }
    });
}




function loadNewAssetCode_Blocked() {
    $.messager.alert('提示', '批量添加不支持刷新编号!', 'info');
}



function showAreaNum_PLTJ_add() {
    $("#Num_PLTJ_add").toggle();
    if ($("#Num_PLTJ_add").is(":hidden")) {
        //当前是hide状态
        //alert("当前是hide状态");
        var objBt = document.getElementById("New_ZCBH_add");
        objBt.onclick = loadNewAssetCode;
        //$("#New_ZCBH_add").attr("onclick", "loadNewAssetCode");

    } else {
        //当前是show状态
        //alert("当前是show状态");
        //$("#New_ZCBH_add").removeAttr("onclick");
        var objBt = document.getElementById("New_ZCBH_add");
        objBt.onclick = loadNewAssetCode_Blocked;
    }
}





function submitForm() {

    //============================基础属性========================================//

    //获取资产编号
    var d_ZCBH_add = $("#ZCBH_add").val();
    //获取资产性质
    var zcxz = $("#ZCXZ_add").val();


    var d_ZCXZ_ID_add;

    //var d_ZCXZ_Name_add;

    //if (zcxz != "" && zcxz != null) {
    //    var zcxz_arr = zcxz.split("-");
    //    if (zcxz_arr.length == 2) {
    //        d_ZCXZ_ID_add = parseInt(zcxz_arr[0]);
    //        d_ZCXZ_Name_add = zcxz_arr[1];
    //    }
    //}

    if (d_ZCLB_add == "" || d_ZCLB_add == null) {

        d_ZCLB_add = $("#ZCLB_add").combotree("getValue");
        if (d_ZCLB_add == "" || d_ZCLB_add == undefined) {
            //$.messager.alert('提示', "调皮的肖金龙", 'info'); return;
        }

    }

    //var d_ZCLB_add;
    //= $("#ZCLB_add").combotree("getValue");


    var d_ZCMC_add = $("#ZCMC_add").val();

    var d_ZCXH_add = $('#ZCXH_add').combobox("getValue");
    //alert(d_ZCXH_add);

    var d_JLDW_add = $("#JLDW_add").combobox("getValue");


    if (d_SZBM_add == "" || d_SZBM_add == null) {

        d_SZBM_add = $("#SZBM_add").combotree("getValue");
        if (d_SZBM_add == "" || d_SZBM_add == undefined) {
            //$.messager.alert('提示', "调皮的肖金龙2", 'info'); return;
        }
    }

    //var d_SZBM_add;
    //= $("#SZBM_add").combotree("gettext")

    var d_SYR_add = $("#SYR_add").combobox("getValue");


    if (d_CFDD_add == "" || d_CFDD_add == null) {
        d_CFDD_add = $("#CFDD_add").combotree("getValue");
        if (d_CFDD_add == "" || d_CFDD_add == undefined) {
            //$.messager.alert('提示', "调皮的肖金龙3", 'info'); return;
        }
    }
    d_CFDD_add = d_CFDD_add == "" ? -1 : parseInt(d_CFDD_add);

    //var d_CFDD_add;
    //= $("#CFDD_add").combotree("getValue");

    var d_ZJFS_add = $("#ZJFS_add").combobox("getValue") == "" ? -1 : parseInt($("#ZJFS_add").combobox("getValue"));

    var d_GYS_add = $("#GYS_add").combogrid("getValue");

    var d_LXR_add = $("#LXR_add").val();

    var d_GZRQ_add = $('#GZRQ_add').datebox('getValue');

    var d_GYSDD_add = $("#GYSDD_add").val();

    var d_Check_PLZJ_add = $("#Num_PLTJ_add").is(":hidden");  //true表示未被选选中  false表示选择

    var d_Num_PLTJ_add = d_Check_PLZJ_add == true ? 1 : $("#Num_PLTJ_add").val();

    //===============================其他属性===================================//

    //var d_Other_SYNX_add = document.getElementById("Other_SYNX_add").lastChild.value();
    //parseInt($("#Other_SYNX_add").numberspinner("getValue").text);
    //= $("#Other_SYNX_add").val();

    d_Other_SYNX_add = $('#Other_SYNX_add').numberspinner('getValue')
    d_Other_SYNX_add = d_Other_SYNX_add == "" ? -1 : parseInt(d_Other_SYNX_add);

    var d_Other_ZJFS_add = $("#Other_ZJFS_add").combobox('getValue');
    d_Other_ZJFS_add = d_Other_ZJFS_add == "" ? -1 : parseInt(d_Other_ZJFS_add);

    var d_Other_JCZL_add = $("#Other_JCZL_add").val();
    d_Other_JCZL_add = d_Other_JCZL_add == "" ? -1 : parseInt(d_Other_JCZL_add);
    //var v = $('#ss').numberspinner('getValue');  // get value

    d_Other_ZCDJ_add = $('#Other_ZCDJ_add').numberspinner('getValue')
    d_Other_ZCDJ_add = d_Other_ZCDJ_add == "" ? -1 : parseInt(d_Other_ZCDJ_add);
    //var d_Other_ZCDJ_add =$('#Other_ZCDJ_add').numberspinner('getValue');  
    //= $("#Other_ZCDJ_add").val();

    //var d_Other_ZCSL_add = $("#Other_ZCSL_add").val();
    d_Other_ZCSL_add = $('#Other_ZCSL_add').numberspinner('getValue')
    d_Other_ZCSL_add = d_Other_ZCSL_add == "" ? -1 : parseInt(d_Other_ZCSL_add);

    var d_Other_ZCJZ_add = $("#Other_ZCJZ_add").val();
    d_Other_ZCJZ_add = d_Other_ZCJZ_add == "" ? -1 : parseFloat(d_Other_ZCJZ_add);

    var d_Other_YTZJ_add = $("#Other_YTZJ_add").val();;
    d_Other_YTZJ_add = d_Other_YTZJ_add == "" ? -1 : parseFloat(d_Other_YTZJ_add);

    var d_Other_LJZJ_add = $("#Other_LJZJ_add").val();
    d_Other_LJZJ_add = d_Other_LJZJ_add == "" ? -1 : parseFloat(d_Other_LJZJ_add);

    var d_Other_JZ_add = $("#Other_JZ_add").val();
    d_Other_JZ_add = d_Other_JZ_add == "" ? -1 : parseFloat(d_Other_JZ_add);

    //===============================自定义属性===================================//

    //封装成json格式创给后台
    var Asset_add = {
        "d_ZCBH_add": d_ZCBH_add,
        //"d_ZCXZ_ID_add": d_ZCXZ_ID_add,
        //"d_ZCXZ_Name_add": d_ZCXZ_Name_add,
        "d_ZCLB_add": d_ZCLB_add,
        "d_ZCMC_add": d_ZCMC_add,
        "d_ZCXH_add": d_ZCXH_add,
        "d_JLDW_add": d_JLDW_add,
        "d_SZBM_add": d_SZBM_add,
        "d_SYR_add": d_SYR_add,
        "d_ZJFS_add": d_ZJFS_add,
        "d_GYS_add": d_GYS_add,
        //"d_LXR_add": d_LXR_add,
        "d_GZRQ_add": d_GZRQ_add,
        //"d_GYSDD_add": d_GYSDD_add,
        "d_Check_PLZJ_add": d_Check_PLZJ_add,
        "d_Num_PLTJ_add": d_Num_PLTJ_add,
        "d_CFDD_add": d_CFDD_add,
        "d_Other_SYNX_add": d_Other_SYNX_add,
        "d_Other_ZJFS_add": d_Other_ZJFS_add,
        "d_Other_JCZL_add": d_Other_JCZL_add,
        "d_Other_ZCDJ_add": d_Other_ZCDJ_add,
        "d_Other_ZCSL_add": d_Other_ZCSL_add,
        "d_Other_ZCJZ_add": d_Other_ZCJZ_add,
        "d_Other_YTZJ_add": d_Other_YTZJ_add,
        "d_Other_LJZJ_add": d_Other_LJZJ_add,
        "d_Other_JZ_add": d_Other_JZ_add
    };

    //$.messager.alert('提示', d_Other_SYNX_add + "|" + d_Other_ZCSL_add + "|" + d_Other_ZCDJ_add, 'info');
    $.ajax({
        url: "/Asset/addNewAsset_hanlder",
        type: 'POST',
        data: {
            "Asset_add": JSON.stringify(Asset_add)
            //"Asset_add": JSON.stringify("\"A\":1")
        },
        beforeSend: ajaxLoading,
        success: function (data) {
            ajaxLoadEnd();
            //// TODO  
            ////alert(data);
            //$("#printLable").text(data);
            //$("#printLable").val(data);
            //$.messager.alert('提示', "调皮的肖金龙3", 'info'); return;
            var result
            if (data > 0) {
                parent.$("#modalwindow").window("close");

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

