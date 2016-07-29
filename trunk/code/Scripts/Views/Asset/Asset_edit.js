//==================================global data==========================================//

var arry_CAttr = new Array();


//初始化处理 防止二重加载
var flag_load_BM = false;
var flag_load_XH = false;
var flag_load_CA = false;

var datagrid_document = "TableList_WJ";
var datagrid_equipment = "TableList_SB";
var datagrid_picture = "TableList_TP";
var ID_Asset_Current = -1;

//待处理数据
var cattrs = new Array();

//==================================global data==========================================//

$(function () {

    loadInitDate();
    $("#Num_PLTJ_add").hide();
    $("#LABEL_PL").hide();
    $("#vfalse_D").hide();


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


    $(window).resize(function () {
        var win_width = $(window).width();
        $("#TableList_WJ").datagrid('resize', { width: win_width });
    });
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

function bindData(id)
{


    ID_Asset_Current = id;
    //获取数据
    $.ajax({
        url: "/Asset/loadAsset_Toedit?id=" + id,
        type: 'POST',
        beforeSend: ajaxLoading,
        success: function (data) {
            ajaxLoadEnd();
            if (data) {
                //开始绑定数据
                //基础属性
                $("#ZCBH_add").val(data.serial_number);
                $("#ZCMC_add").val(data.name_Asset);
                $("#LXR_add").val(data.linkMan_supplier);
                $("#GYSDD_add").val(data.address_supplier);
                $("#ZCBH_add").val(data.serial_number);

                $("#ZCLB_add").combotree('setValue', data.type_Asset);
                $("#SZBM_add").combotree('setValue', data.department_Using);
                $("#CFDD_add").combotree('setValue', data.addressCF);
                $("#JLDW_add").combobox('setValue', data.measurement);
                $("#ZJFS_add").combobox('setValue', data.Method_add);
                $('#GYS_add').combogrid('setValue', data.supplierID);
                $('#GYS_add').combogrid('setText', data.supplierName);
                $("#ZCXH_add").combobox('setValue', data.specification);
                $("#ZCXH_add").combobox('setText', data.specification);
                $("#SYRY_add").combobox('setValue', data.Owener);
                $("#SYRY_add").combobox('setText', data.name_owner);

                var date_add = dateString(data.Time_Purchase);
                $("#GZRQ_add").datebox('setValue', date_add);

                //其他属性
                $('#Other_ZCDJ_add').numberbox('setValue', data.unit_price);
                $('#Other_ZCSL_add').numberbox('setValue', data.amount);
                $('#Other_SYNX_add').numberspinner('setValue', data.YearService_month);
                $("#Other_ZJFS_add").combobox('setValue', data.Method_depreciation);
                $("#Other_JCZL_add").val(data.Net_residual_rate);
                $("#Other_ZCJZ_add").val(data.value);
                $("#Other_YTZJ_add").val(data.depreciation_Month);
                $("#Other_LJZJ_add").val(data.depreciation_tatol);
                $("#Other_JZ_add").val(data.Net_value);

                //自定义属性
                cattrs = data.cattrs;
                update_ZDY_attr(data.type_Asset);
                load_sub_documents(datagrid_document, ID_Asset_Current)
                load_sub_picture(datagrid_picture, ID_Asset_Current);
                load_sub_equiment(datagrid_equipment,ID_Asset_Current);
                //延迟加载

            } else {
            }
        }
    });
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
           if (flag_load_XH == false) {

           } else {
               load_ZCXH_add(node.id);
           }

           update_ZDY_attr(node.id)
           updateZCXZ(tree, node);
       }, //全部折叠
       onLoadSuccess: function (node, data) {
           $('#ZCLB_add').combotree('tree').tree("collapseAll");
           flag_load_XH = true;

           var lb_s = $('#ZCLB_add').combotree('getValue')
           var zxch_c = $("#ZCXH_add").combobox('getValue');
           load_ZCXH_add(lb_s);
           $("#ZCXH_add").combobox('setValue', zxch_c);
       }
   });
}

function update_infoZJ(id_zclb) {
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
        cattr_item.title = data[i].title;

        //获取适配的数据
        var ca_checked = getCAttrByID(data[i].ID, cattrs);
        //判断数据类型
        if (data[i].type_Name == "字典类型") {
            //获取其数据结构类型
            if (data[i].isTree != null && data[i].isTree == true) {
                initcombotree(id_a, data[i].type_value, data[i].necessary, ca_checked);
                cattr_item.InputType = 1;
            } else {
                initCombobox(id_a, data[i].type_value, data[i].necessary, ca_checked);
                cattr_item.InputType = 2;
            }
        } else {
            //设置成number
            initNumberBox(id_a, ca_checked);
            cattr_item.InputType = 3;
        }
        arry_CAttr.push(cattr_item);
    }








}

function getCAttrByID(id_ca,cattrs)
{
    for (var i = 0; i < cattrs.length; i++)
    {
        if (cattrs[i].ID_customAttr == id_ca)
        {
            return cattrs[i];
        }
    }
    return null;
}


function initCombobox(id_combobox, id_dic, requiredFlag, ca_checked)
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
        },
        onLoadSuccess: function ()
        {
            if (ca_checked) {
                $('#' + id_combobox).combobox('setValue', ca_checked.value);

            }
        }
    });
}


function initcombotree(id_combotree, id_dic, requiredFlag, ca_checked)
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
          //设置绑定数据
          if (ca_checked) {
              $('#' + id_combotree).combotree('setValue', ca_checked.value);
          }
      }
  });
}

function initNumberBox(id_Numbox, ca_checked)
{
    $('#'+id_Numbox).numberbox({
        min: 0,
        precision: 2
    });

    if (ca_checked)
    {
        $('#' + id_Numbox).val(ca_checked.value);
    }
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
            if (flag_load_BM == false) {
                return;
            } else {
                load_User_add(node.id);
            }
        }, //全部折叠
        onLoadSuccess: function (node, data) {
            //$('#SZBM_add').combotree('tree').tree("collapseAll");
            flag_load_BM = true;
            d_SZBM_add = $('#SZBM_add').combotree('getValue');
            d_user = $('#SYRY_add').combobox('getValue');
            load_User_add(d_SZBM_add);
            $('#SYRY_add').combobox('setValue', d_user);
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


function load_sub_picture(datagrid, id_asset) {
    $('#' + datagrid).datagrid({
        url: '/Asset/load_sub_pictures?id_asset=' + id_asset,
        method: 'POST', //默认是post,不允许对静态文件访问
        width: 'auto',
        height: '300px',
        iconCls: 'icon-save',
        dataType: "json",
        fitColumns: true,
        pagePosition: 'top',
        rownumbers: true, //是否加行号 
        pagination: true, //是否显式分页 
        pageSize: 15, //页容量，必须和pageList对应起来，否则会报错 
        pageNumber: 1, //默认显示第几页 
        pageList: [15, 30, 45],//分页中下拉选项的数值 
        columns: [[
            { field: 'ID', checkbox: true, width: 50 },
            { field: 'fileNmae', title: '文件名', width: 50 },
            { field: 'user_add', title: '登记人', width: 50 },
            {
                field: 'id_download', title: '附件', width: 50,
                formatter: function (date) {
                    var btn = "<a  onclick='downloadFile_TP(" + date + ")' href='javascript:void(0)'>下载</a>";
                    return btn;
                }
            }
        ]],
        singleSelect: true, //允许选择多行
        selectOnCheck: true,//true勾选会选择行，false勾选不选择行, 1.3以后有此选项
        checkOnSelect: true //true选择行勾选，false选择行不勾选, 1.3以后有此选项
    });
    loadPageTool_Picture(datagrid);
}
function loadPageTool_Picture(datagrid) {
    var pager = $('#' + datagrid).datagrid('getPager');	// get the pager of datagrid
    pager.pagination({
        buttons: [
            {
                text: '添加',
                iconCls: 'icon-add',
                height: 50,
                handler: function () {
                    var url = "/Asset/Asset_SubPicture_add?id_asset=" + ID_Asset_Current;
                    var titleName = "新增图片";
                    openModelWindow(url, titleName);
                }
            }
        , {
            text: '删除',
            height: 50,
            iconCls: 'icon-save',
            handler: function () {
                //获取选择行
                var rows = $('#' + datagrid).datagrid('getSelections');
                if (rows.length != 1) {
                    MessShow("请选择1项数据！");
                    return;
                }
                //获取选择行
                var rows = $('#' + datagrid).datagrid('getSelections');
                if (rows.length != 1) {
                    MessShow("请选择1项数据！");
                    return;
                }
                var id_delete = rows[0].ID;
                deteteItem("TP", id_delete, datagrid);
            }
        }
        ],
        beforePageText: '第',//页数文本框前显示的汉字  
        afterPageText: '页    共 {pages} 页',
        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录'
    });
}

function load_sub_documents(datagrid,id_asset)
{
    $('#' + datagrid).datagrid({
        url: '/Asset/load_sub_documents?id_asset=' + id_asset,
        method: 'POST', //默认是post,不允许对静态文件访问
        width: 'auto',
        height: '300px',
        iconCls: 'icon-save',
        dataType: "json",
        fitColumns: true,
        pagePosition: 'top',
        rownumbers: true, //是否加行号 
        pagination: true, //是否显式分页 
        pageSize: 15, //页容量，必须和pageList对应起来，否则会报错 
        pageNumber: 1, //默认显示第几页 
        pageList: [15, 30, 45],//分页中下拉选项的数值 
        columns: [[
            { field: 'ID', checkbox: true, width: 50 },
            { field: 'fileNmae', title: '文件名', width: 50 },
            { field: 'user_add', title: '登记人', width: 50 },
            {
                field: 'id_download', title: '附件', width: 50,
                formatter: function (date) {
                    var btn = "<a  onclick='downloadFile(" + date + ")' href='javascript:void(0)'>下载</a>";
                    return btn;
                }
            },
            { field: 'abstractinfo', title: '摘要', width: 50 },
            { field: 'noteinfo', title: '备注', width: 50 }
            
        ]],
        singleSelect: true, //允许选择多行
        selectOnCheck: true,//true勾选会选择行，false勾选不选择行, 1.3以后有此选项
        checkOnSelect: true //true选择行勾选，false选择行不勾选, 1.3以后有此选项
    });
    loadPageTool_document(datagrid);
}
function loadPageTool_document(datagrid) {
    var pager = $('#' + datagrid).datagrid('getPager');	// get the pager of datagrid
    pager.pagination({
        buttons: [
            {
                text: '添加',
                iconCls: 'icon-add',
                height: 50,
                handler: function () {
                    var url = "/Asset/Asset_Subdocument_add?id_asset="+ID_Asset_Current;
                    var titleName = "新增附件名称";
                    openModelWindow(url,titleName);
                    
                }
            }
        , {
            text: '删除',
            height: 50,
            iconCls: 'icon-save',
            handler: function () {
                //获取选择行
                var rows = $('#' + datagrid).datagrid('getSelections');
                if (rows.length != 1) {
                    MessShow("请选择1项数据！");
                    return;
                }
                //获取选择行
                var rows = $('#' + datagrid).datagrid('getSelections');
                if (rows.length != 1) {
                    MessShow("请选择1项数据！");
                    return;
                }
                var id_delete = rows[0].ID;
                deteteItem("WJ", id_delete, datagrid);
            }
        }
        ],
        beforePageText: '第',//页数文本框前显示的汉字  
        afterPageText: '页    共 {pages} 页',
        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录'
    });
}
function load_sub_equiment(datagrid, id_asset) {
    $('#' + datagrid).datagrid({
        url: '/Asset/load_sub_equipment?id_asset=' + id_asset,
        method: 'POST', //默认是post,不允许对静态文件访问
        width: 'auto',
        height: '300px',
        iconCls: 'icon-save',
        dataType: "json",
        fitColumns: true,
        pagePosition: 'top',
        rownumbers: true, //是否加行号 
        pagination: true, //是否显式分页 
        pageSize: 15, //页容量，必须和pageList对应起来，否则会报错 
        pageNumber: 1, //默认显示第几页 
        pageList: [15, 30, 45],//分页中下拉选项的数值 
        columns: [[
            { field: 'ID', checkbox: true, width: 50 },
            { field: 'serialNum', title: '资产编号', width: 50 },
            { field: 'name', title: '资产名称', width: 50 },
            {
                field: 'date_add', title: '登记时间', width: 100,
                formatter: function (date) {
                    if (date==null) {
                        return "";
                    }
                    var pa = /.*\((.*)\)/;
                    var unixtime = date.match(pa)[1].substring(0, 10);
                    return getTime(unixtime);
                }
            },
            {
                field: 'specification', title: '资产类型', width: 50
            },
            { field: 'measurement', title: '计量单位', width: 50 },
            { field: 'supplier', title: '供应商', width: 50 }

        ]],
        singleSelect: true, //允许选择多行
        selectOnCheck: true,//true勾选会选择行，false勾选不选择行, 1.3以后有此选项
        checkOnSelect: true //true选择行勾选，false选择行不勾选, 1.3以后有此选项
    });
    loadPageTool_equiment(datagrid);
}
function loadPageTool_equiment(datagrid) {
    var pager = $('#' + datagrid).datagrid('getPager');	// get the pager of datagrid
    pager.pagination({
        buttons: [
            {
                text: '添加',
                iconCls: 'icon-add',
                height: 50,
                handler: function () {
                    var url = "/Asset/Asset_SubEquiment_add?id_asset=" + ID_Asset_Current;
                    var titleName = "新增附属设备";
                    openModelWindow(url, titleName);

                }
            }
        , {
            text: '删除',
            height: 50,
            iconCls: 'icon-save',
            handler: function () {
                //获取选择行
                var rows = $('#' + datagrid).datagrid('getSelections');
                if (rows.length != 1) {
                    MessShow("请选择1项数据！");
                    return;
                }
                var id_delete = rows[0].ID;
                deteteItem("SB", id_delete, datagrid);
            }
        }
        ],
        beforePageText: '第',//页数文本框前显示的汉字  
        afterPageText: '页    共 {pages} 页',
        displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录'
    });
}


function deteteItem(type,id,datagrid)
{
    //将数据传入后台
    $.ajax({
        url: '/Asset/delete_Sub_item',
        data: { "type": type,"id":id },
        dataType: "json",
        type: "POST",
        traditional: true,
        success: function (data) {
            if (data > 0)
            {
                $('#' + datagrid).datagrid('reload');
            }
        }
    });
}


function downloadFile(id)
{
    var url = "/Asset/downloadSubFileBydocID?id=" + id;
    exportData(url);

}

function downloadFile_TP(id) {
    var url = "/Asset/downloadSubPictureBydocID?id=" + id;
    exportData(url);

}

function exportData(url) {
    var form = $("<form>");//定义一个form表单
    form.attr("style", "display:none");
    form.attr("target", "hidden_frame");
    form.attr("method", "post");
    form.attr("action", url);
    var input1 = $("<input>");
    input1.attr("type", "hidden");
    input1.attr("name", "exportData");
    input1.attr("value", (new Date()).getMilliseconds());
    $("body").append(form);//将表单放置在web中
    form.append(input1);
    form.submit();//表单提交
}




function showAreaNum_PLTJ_add() {
    $("#Num_PLTJ_add").toggle();
    $("#LABEL_PL").toggle();

}

function openModelWindow(url, titleName) {

    try {
        $("#modalwindow2").window("close");
    } catch (e) { }
    var $winADD;
    $winADD = $('#modalwindow2').window({
        title: titleName,
        width: 500,
        height: 450,
        top: (($(window).height() - 450) > 0 ? ($(window).height() - 450) : 200) * 0.5,
        left: (($(window).width() - 500) > 0 ? ($(window).width() - 500) : 100) * 0.5,
        shadow: true,
        modal: true,
        iconCls: 'icon-add',
        closed: true,
        minimizable: false,
        maximizable: false,
        collapsible: false,
        onClose: function () {
        }
    });
    $("#modalwindow2").html("<iframe width='100%' height='99%'  frameborder='0' src='" + url + "'></iframe>");
    $winADD.window('open');
}



function submitForm(id_asset) {

    //============================基础属性========================================//

    //获取资产编号
    var d_ZCBH_add = $("#ZCBH_add").val();
    //获取资产性质
    var zcxz = $("#ZCXZ_add").val();
    var d_ZCXZ_ID_add;
    var d_ZCLB_add = $("#ZCLB_add").combotree("getValue");
    
    var d_ZCMC_add = $("#ZCMC_add").val();

    var d_ZCXH_add = $('#ZCXH_add').combobox("getValue");
    var d_JLDW_add = $("#JLDW_add").combobox("getValue");

    var d_SZBM_add = $("#SZBM_add").combotree("getValue");


    var d_SYRY_add = $("#SYRY_add").combobox("getValue");
    var d_CFDD_add = $("#CFDD_add").combotree("getValue");

    

    var d_ZJFS_add = $("#ZJFS_add").combobox("getValue");

    var d_GYS_add = $("#GYS_add").combogrid("getValue");

    var d_LXR_add = $("#LXR_add").val();

    var d_GZRQ_add = $('#GZRQ_add').datebox('getValue');

    var d_GYSDD_add = $("#GYSDD_add").val();

    var d_Check_PLZJ_add = $("#Num_PLTJ_add").is(":hidden");  //true表示未被选选中  false表示选择
    d_Check_PLZJ_add = true;
    var d_Num_PLTJ_add = d_Check_PLZJ_add == true ? 1 : $("#Num_PLTJ_add").val();

    //===============================其他属性===================================//


    d_Other_SYNX_add = $('#Other_SYNX_add').numberspinner('getValue')

    var d_Other_ZJFS_add = $("#Other_ZJFS_add").combobox('getValue');

    var d_Other_JCZL_add = $("#Other_JCZL_add").val();

    d_Other_ZCDJ_add = $('#Other_ZCDJ_add').numberspinner('getValue')

    d_Other_ZCSL_add = $('#Other_ZCSL_add').numberspinner('getValue')

    var d_Other_ZCJZ_add = $("#Other_ZCJZ_add").val();


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
        "ID":id_asset
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
                new_attr.value = $("#" + arry_CAttr[i].id).combotree("getValue");
            }; break;
            case 2: {
                new_attr.value = $("#" + arry_CAttr[i].id).combobox("getValue");
            }; break;
            case 3: {
                new_attr.value = $("#" + arry_CAttr[i].id).val();
            }; break;
            default:;break;
        }
        data_cattr.push(JSON.stringify(new_attr));
    }



    $.ajax({
        url: "/Asset/Handler_updateAsset",
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
                parent.$('#TableList_0_1').datagrid("reload");
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


function cancelFormSubmit() {
    try {
        parent.$("#modalwindow").window("close");
    } catch (e) {

    }
}
function checkFormat(id_asset) {
    //基础属性
    var check_obj_ZCLB = $("#ZCLB_add").combotree("getValue");
    var check_obj_ZCMC = $("#ZCMC_add").val();
    var check_obj_ZCXH = $("#ZCXH_add").combobox("getValue");
    var check_obj_JLDW = $("#JLDW_add").combobox("getValue");
    var check_obj_CFDD = $("#CFDD_add").combotree("getValue");
    var check_obj_GYS = $("#GYS_add").combogrid("getValue");
    var check_obj_GZRQ = $("#GZRQ_add").datebox("getValue");
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
    } else if (isNull(check_obj_JLDW)) {
        MessShow("计量单位不能为空");
        return;
    } else if (isNull(check_obj_CFDD)) {
        MessShow("存放地点不能为空");
        return;
    } else if (isNull(check_obj_GYS)) {
        MessShow("供应商不能为空");
        return;
    } else if (isNull(check_obj_GZRQ)) {
        MessShow("购置日期不能为空");
        return;
    } else if (isNull(check_obj_Other_JCZL)) {
        MessShow("净残值率不能为空");
        return;
    } else if (check_obj_Other_ZCDJ <= 0) {
        MessShow("资产单价不能为0");
        return;
    } else if (check_obj_Other_ZCSL <= 0) {
        MessShow("资产数量不能为0");
        return;
    } else if (check_obj_Other_SYNX <= 0) {
        MessShow("使用年限不能为0.");
        return;
    }

    //alert(check_obj_GZRQ);
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
            }; break;
            default:; break;
        }
    }
    submitForm(id_asset);

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



function dateString(date) {
    var pa = /.*\((.*)\)/;
    var unixtime = date.match(pa)[1].substring(0, 10);
    return getTime(unixtime);
}

function getTime(/** timestamp=0 **/) {
    var ts = arguments[0] || 0;
    var t, y, m, d, h, i, s;
    t = ts ? new Date(ts * 1000) : new Date();
    y = t.getFullYear();
    m = t.getMonth() + 1;
    d = t.getDate();
    h = t.getHours();
    i = t.getMinutes();
    s = t.getSeconds();
    // 可根据需要在这里定义时间格式  
    return y + '-' + (m < 10 ? '0' + m : m) + '-' + (d < 10 ? '0' + d : d);
}
