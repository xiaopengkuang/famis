//$(document).ready(function() {
//    var options = {
//        target: '#hidden_frame',
//        // 从服务传过来的数据显示在这个div内部
//       // 也就是ajax局部刷新
//        //beforeSubmit: checkForm,
//    // ajax提交之前的处理
//        //success:  showResponse
//    // 处理之后的处理
//    };
//    $('#FORM1').submit(function () {
//        $(this).ajaxSubmit(options);
//        return false; 
//        //非常重要，如果是false，则表明是不跳转
//        //在本页上处理，也就是ajax，如果是非false，则传统的form跳转。
//    });
//});


function checkForm(id_asset)
{
    var noteInfo = $("#noteInfo").val();
    var abstractInfo = $("#abstractInfo").val();
    var name = $("#name").val();
    if (isNull(name)) {
        MessShow("资产名称不能为空");
        return;
    }
    submitData(id_asset)
}


function submitData(id_asset)
{

   
    var noteInfo = $("#noteInfo").val();
    var abstractInfo = $("#abstractInfo").val();
    var name = $("#name").val();
    var file = document.getElementById("subFile");

    var form = document.createElement('form');

    document.body.appendChild(form);
    form.encoding = "multipart/form-data";
    form.method = "post";
    form.fileUpload =true;
    form.action = "/Asset/uploadDocument?id_asset=" + id_asset + "&name=" + name + "&noteInfo=" + noteInfo + "&abstractInfo=" + abstractInfo ;
    form.target = "hidden_frame";
    var pos = file.nextSibling; //记住file在旧表单中的的位置
    form.appendChild(file);
    form.submit();
    pos.parentNode.insertBefore(file, pos);
    document.body.removeChild(form);
    
    try {
        parent.$("#TableList_WJ").datagrid("reload");
        parent.$("#modalwindow2").window("close");
    } catch (e) { }

}


function reloadData_parent()
{

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