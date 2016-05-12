////公共事件
//function load_Department(obj) {

//    obj.combotree
//    ({
//        url: '/Dict/load_SZBM',
//        valueField: 'id',
//        textField: 'nameText',
//        required: true,
//        method: 'get',
//        editable: false,
//        //选择树节点触发事件  
//        onSelect: function (node) {
//            //返回树对象  
//            var tree = $(this).tree;
//            //选中的节点是否为叶子节点,如果不是叶子节点,清除选中  
//            var isLeaf = tree('isLeaf', node.target);
//            if (!isLeaf) {
//                //清除选中  
//                obj.combotree('clear');
//            } else {
//            }
//            //

//        }, //全部折叠
//        onLoadSuccess: function (node, data) {
//            obj.combotree('tree').tree("collapseAll");
//        }
//    });

//}