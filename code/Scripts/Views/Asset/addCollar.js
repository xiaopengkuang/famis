//

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


//=======================初始化加载信息===================================//
$(function () {
    
    loadInitData();
});

function loadInitData()
{
    load_Department();
    load_CFDD_add();
}

function load_Department() {

    $('#LYBM_add').combotree
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
                $('#LYBM_add').combotree('clear');
            } else {
                load_staff_Department(node.id);
            }
            //

        }, //全部折叠
        onLoadSuccess: function (node, data) {
            $('#LYBM_add').combotree('tree').tree("collapseAll");
        }
    });

}

function load_staff_Department(SZBM_ID) {

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