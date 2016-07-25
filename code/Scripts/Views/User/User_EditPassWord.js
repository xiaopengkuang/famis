$(function () {
    
});


function orign_passwordBlur()
{
    var op = $("#orign_password").val();
    if (op == null || op == "") {
        $("#Info_ORG").html("请输入原始密码！");
    } else {
        //检测是否对
        $("#Info_ORG").html("");
        $.ajax({
            url: "/User/isPasswordRight?pwd=" + op,
            type: 'POST',
            success: function (data) {
                if (data>0) {
                    $("#Info_ORG").html("");
                } else {
                    $("#Info_ORG").html("密码错误");
                }
            }
        });
    }
}


function recheckPWD()
{
    checkPWD();
    var pwd_f = $("#newPass").val();
    var pwd_s = $("#passwordConfirm").val();
    if (pwd_f != pwd_s) {
        $("#Info_Confirm").html("密码不匹配！");
        return;
    } else {
        $("#Info_Confirm").html("");
    }

}


function cancleSubmt()
{
    try {
        parent.$('#modalwindow_editPassword').window('close');
    } catch (e) { }
}

function submitNewPWD()
{
    var op = $("#orign_password").val();
    var pwd_f = $("#newPass").val();
    var pwd_s = $("#passwordConfirm").val();
    if (op==null||op=="")
    {
        MessShow("原始密码不能为空！");
        orign_passwordBlur();
        return;
    }

    if (pwd_f == null || pwd_f == "")
    {
        MessShow("新密码不能为空！");
        checkPWD();
        return;
    }
    if (pwd_s == null || pwd_s == "") {
        MessShow("密码不匹配！");
        recheckPWD();
        return;
    }

    $.ajax({
        url: "/User/redefinePWD?pwd_old=" + op+"&pwd_new="+pwd_f,
        type: 'POST',
        success: function (data) {
            if (data > 0) {
                //页面跳转
                try {
                    parent.window.location.href = "/User/Login";
                } catch (e) {
                    MessShow("密码修改失败！");
                }
            } else {

            }
            
        }
    });

}

function checkPWD()
{
    var pwd_f = $("#newPass").val();
    if (pwd_f == null || pwd_f == "") {
        $("#Info_NEW").html("请输入新密码！");
        return;
    } else {
        $("#Info_NEW").html("");
    }
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
