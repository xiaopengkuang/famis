function submitData() {


    var file = document.getElementById("subFile");

    var form = document.createElement('form');

    document.body.appendChild(form);
    form.encoding = "multipart/form-data";
    form.method = "post";
    form.fileUpload = true;
    form.action = "/Asset/Handler_addAssetByExcel";
    form.target = "hidden_frame";
    var pos = file.nextSibling; //记住file在旧表单中的的位置
    form.appendChild(file);
    form.submit();
    pos.parentNode.insertBefore(file, pos);
    document.body.removeChild(form);
    sleep(1000);
    try {
        parent.$("#TableList_0_1").datagrid("reload");
        parent.$("#modalwindow").window("close");
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