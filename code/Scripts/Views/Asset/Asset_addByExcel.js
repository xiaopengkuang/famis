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


//下载文件模板
function downloadModelEXCEL()
{
    var url = "/Asset/Handler_downloadExcelModel";
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

function sleep(numberMillis) {
    var now = new Date();
    var exitTime = now.getTime() + numberMillis;
    while (true) {
        now = new Date();
        if (now.getTime() > exitTime)
            return;
    }
}