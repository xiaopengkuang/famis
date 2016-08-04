function bindUser(openid)
{
    var username = $("#username").val();
    var password = $("#password").val();

    if (password == "" || username == "" ||openid==undefined||openid==null|| openid == "")
    {
        MessShow("未获取微信用户信息/用户信息不能为空！");
        return;
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
