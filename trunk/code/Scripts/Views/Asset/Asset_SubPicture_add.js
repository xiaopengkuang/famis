

function checkForm(id_asset)
{
    var name = $("#name").val();
    if (isNull(name)) {
        MessShow("图片名称不能为空");
        return;
    }
    submitData(id_asset)
}


function submitData(id_asset)
{

   
    var name = $("#name").val();
    var file = document.getElementById("subFile");

    var form = document.createElement('form');

    document.body.appendChild(form);
    form.encoding = "multipart/form-data";
    form.method = "post";
    form.fileUpload =true;
    form.action = "/Asset/uploadPicture?id_asset=" + id_asset + "&name=" + name;
    form.target = "hidden_frame";
    var pos = file.nextSibling; //记住file在旧表单中的的位置
    form.appendChild(file);
    form.submit();
    pos.parentNode.insertBefore(file, pos);
    document.body.removeChild(form);
    sleep(1000);
    try {
        parent.$("#TableList_TP").datagrid("reload");
        parent.$("#modalwindow2").window("close");
    } catch (e) { }

}
function sleep(numberMillis) {
    var now = new Date();
    var exitTime = now.getTime() + numberMillis;
    while (true) {
        now = new Date();
        if (now.getTime() > exitTime)
            return;
    }
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