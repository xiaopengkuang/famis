var tabs;
$(document).ready(function () {


    


    $(function () {
        var h = $(document).height() - 30;
        //h = $("#leftside").height;
        $("#tabs").height(h); //关于这里我要说明一下，如果不设置高度的话，它默认并不是发100%占满屏幕的，所以我这里使用了计算的方式，初始化界面高度
        tabs = $('#tabs').cleverTabs();
        $(window).bind('resize', function () {
            tabs.resizePanelContainer();
        });
        alert(1);
        tabs.add({
            url: '/Home/Welcome',
            label: 'Welcome',
            //开启Tab后是否锁定(不允许关闭，默认: false)
            lock: true
        });
        $('input[type="button"]').button();
    });

});

    function addTab(url, name) {
        //$("#tabs").height($("#leftside").height());
        tabs.add({
            url: url,
            label: name,
            lock: false
        });
    }
    function goNewPage(url, name) {
        //self.parent.frames["mainFrame"].addTab(url, name);
        addTab(url, name);
        alert(2);
    }

