$(function () {
    $('#modalwindow_editPassword').window('close');
});



function openWindow_EditPass()
{
    var url = "/User/EditPassWord";
    var titleName = "修改密码";
    openModelWindow(url,titleName);
}

function openModelWindow(url, titleName) {

    try {
        $("#modalwindow_editPassword").window("close");
    } catch (e) { }
    var $winADD;
    $winADD = $('#modalwindow_editPassword').window({
        title: titleName,
        width: 400,
        height: 300,
        top: (($(window).height() - 300) > 0 ? ($(window).height() - 300) : 150) * 0.5,
        left: (($(window).width() - 400) > 0 ? ($(window).width() - 400) : 300) * 0.5,
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
    $("#modalwindow_editPassword").html("<iframe width='100%' height='99%'  frameborder='0' src='" + url + "'></iframe>");
    $winADD.window('open');
}