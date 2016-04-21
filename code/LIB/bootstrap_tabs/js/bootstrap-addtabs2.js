/**
 * Website: http://git.oschina.net/hbbcs/bootStrap-addTabs
 *
 * Version : 0.6
 *
 * Created by joe on 2016-2-4.
 */

$.fn.addtabs = function (options) {
    obj = $(this);
    Addtabs.options = $.extend({
        content: '', //直接指定所有页面TABS内容
        close: true, //是否可以关闭
        monitor: 'body', //监视的区域
        iframeUse: true, //使用iframe还是ajax
        iframeHeight: $(document).height() - 107, //固定TAB中IFRAME高度,根据需要自己修改
        method: 'init',
        callback: function () { //关闭后回调函数
        }
    }, options || {});


    $(Addtabs.options.monitor).on('click', '[data-addtab]', function () {
        Addtabs.add({
            id: $(this).attr('data-addtab'),
            title: $(this).attr('title') ? $(this).attr('title') : $(this).html(),
            content: Addtabs.options.content ? Addtabs.options.content : $(this).attr('content'),
            url: $(this).attr('url'),
            ajax: $(this).attr('ajax') ? true : false
        });
    });

    obj.on('click', '.close-tab', function () {
        var id = $(this).prev("a").attr("aria-controls");
        Addtabs.close(id);
    });


    obj.on('mouseover', '.close-tab', function () {
        $(this).removeClass('glyphicon-remove').addClass('glyphicon-remove-circle');
    })

    obj.on('mouseout', '.close-tab', function () {
        $(this).removeClass('glyphicon-remove-circle').addClass('glyphicon-remove');
    })

    $(window).resize(function () {
        obj.find('iframe').attr('height', Addtabs.options.iframeHeight);
        Addtabs.drop();
    });

};

window.Addtabs = {
    options:{},
    add: function (opts) {
        var id = 'tab_' + opts.id;
        obj.find('.active').removeClass('active');
        //如果TAB不存在，创建一个新的TAB
        if (!$("#" + id)[0]) {
            //创建新TAB的title

           
            //var title = document.createElement("<li role='presentation' id='tab_" + id + "'>");
            var title = document.createElement("li");
            title.setAttribute("role", "presentation");
            title.setAttribute("id", "tab_"+id);
            //var tmpA = document.createElement("<a href='#" + id + "' aria-controls='" + id + "' role='tab' data-toggle='tab'>");
            var tmpA = document.createElement("a");
            tmpA.setAttribute("href", "#");
            tmpA.setAttribute("aria-controls", id+"");
            tmpA.setAttribute("role", "tab");
            tmpA.setAttribute("data-toggle", "tab");
            tmpA.innerHTML=opts.title;
            title.appendChild(tmpA)
            //if (true) {
            //    title = document.createElement("<li role='presentation' id='tab_" + id + "'>");
            //    var tmpA = document.createElement("<a href='#" + id + "' aria-controls='" + id + "' role='tab' data-toggle='tab'>");
            //    tmpA.innerHTML(opts.title);
            //    title.appendChild(tmpA)
            //} else { 
            //    title = $('<li>', {
            //        'role': 'presentation',
            //        'id': 'tab_' + id
            //    }).append(
            //        $('<a>', {
            //            'href': '#' + id,
            //            'aria-controls': id,
            //            'role': 'tab',
            //            'data-toggle': 'tab'
            //        }).html(opts.title)
            //    );
            //}

            //是否允许关闭
            if (Addtabs.options.close) {


                //var tmpLI = document.createElement("<i class='close-tab glyphicon glyphicon-remove'>");

                var tmpLI = document.createElement("i");
                tmpLI.setAttribute("class", "close-tab glyphicon glyphicon-remove");
                title.appendChild(tmpLI);
                //if (true) {
                //    var tmpLI = document.createElement("<i class='close-tab glyphicon glyphicon-remove'>");
                //    title.appendChild(tmpLI);
                //} else{
                //  title.append(
                //    $('<i>',{class:'close-tab glyphicon glyphicon-remove'})
                //);
                //}

              
            }

          
                //if($.browser.msie) {

                //    var txt = document.createElement("<input type='text' name='txtName'>")

                //} else {

                //    var txt = document.createElement("input");

                //    txt.type = "text";

                //    txt.name = "txtName";

                //}

            //创建新TAB的内容
            //var content = document.createElement("<div class='tab-pane' id='" + id + "' role='tabpanel'>");
            var content = document.createElement("div");
            content.setAttribute("class", "tab-pane");
            content.setAttribute("id", id);
            content.setAttribute("role", "tabpanel");
            //if (true) {
            //    content = document.createElement("<div class='tab-pane' id='" + id + "' role='tabpanel'>");
            //}else{
            //    content= $('<div>', {
            //        'class': 'tab-pane',
            //        'id': id,
            //        'role': 'tabpanel'
            //    });
            //}
            //是否指定TAB内容
            if (opts.content) {
                content.innerHTML=opts.content;
            } else if (Addtabs.options.iframeUse && !opts.ajax) {//没有内容，使用IFRAME打开链接
                //var tmpIFrame = document.createElement("<iframe class='iframeClass' height='" + Addtabs.options.iframeHeight + "' frameborder='no' src='" + opts.url + "'>");
                var tmpIFrame = document.createElement("iframe");
                tmpIFrame.setAttribute("class", "iframeClass");
                tmpIFrame.setAttribute("height", Addtabs.options.iframeHeight);
                tmpIFrame.setAttribute("frameborder", "no");
                tmpIFrame.setAttribute("src", opts.url);
                content.appendChild(tmpIFrame);


                //if (true) {
                //    var tmpIFrame = document.createElement("<iframe class='iframeClass' height='" + Addtabs.options.iframeHeight + "' frameborder='no' src='" + opts.url + "'>");
                //    content.appendChild(tmpIFrame);
                //}else{

                //    content.append(
                //        $('<iframe>', {
                //            'class': 'iframeClass',
                //            'height': Addtabs.options.iframeHeight,
                //            'frameborder': "no",
                //            'border': "0",
                //            'src': opts.url
                //        })
                //    );
                //}
            } else {
                $.get(opts.url, function (data) {
                    content.append(data);
                });
            }
            //加入TABS
            obj.children('.nav-tabs').append(title);
            obj.children(".tab-content").append(content);
        }

        //激活TAB
        $("#tab_" + id).addClass('active');
        $("#" + id).addClass("active");
        Addtabs.drop();
    },
    close: function (id) {
        //如果关闭的是当前激活的TAB，激活他的前一个TAB
        if (obj.find("li.active").attr('id') == "tab_" + id) {
            $("#tab_" + id).prev().addClass('active');
            $("#" + id).prev().addClass('active');
        }
        //关闭TAB
        $("#tab_" + id).remove();
        $("#" + id).remove();
        Addtabs.drop();
        Addtabs.options.callback();
    },
    drop: function () {
        element = obj.find('.nav-tabs');
        //创建下拉标签
        //var dropdown = document.createElement("<li class='dropdown pull-right hide tabdrop'>");
        var dropdown = document.createElement("li");
        dropdown.setAttribute("class","dropdown pull-right hide tabdrop");
        //var tmpA2 = document.createElement("<a class='dropdown-toggle' data-toggle='dropdown' href='#'>");
        var tmpA2 = document.createElement("a");
        tmpA2.setAttribute("class", "dropdown-toggle");
        tmpA2.setAttribute("data-toggle", "dropdown");
        tmpA2.setAttribute("href", "#");
        //var tmpI2 = document.createElement("<i class='glyphicon glyphicon-align-justify'>");
        var tmpI2 = document.createElement("i");
        tmpI2.setAttribute("class", "glyphicon glyphicon-align-justifys");
        //var tmpB2 = document.createElement("<b class='caret'>");
        var tmpB2 = document.createElement("b");
        tmpB2.setAttribute("class", "caret");

        tmpI2.appendChild(tmpB2);
        tmpA2.appendChild(tmpI2);

        //var tmpUL2 = document.createElement("<ul class='dropdown-menu'>");
        var tmpUL2 = document.createElement("ul");
        tmpUL2.setAttribute("class","dropdown-menu");
        dropdown.appendChild(tmpA2).appendChild(tmpUL2);

        //if (true) {

        //    dropdown = document.createElement("<li class='dropdown pull-right hide tabdrop'>");
        //    var tmpA2 = document.createElement("<a class='dropdown-toggle' data-toggle='dropdown' href='#'>");
        //    var tmpI2 = document.createElement("<i class='glyphicon glyphicon-align-justify'>");
        //    var tmpB2 = document.createElement("<b class='caret'>");
        //    tmpI2.appendChild(tmpB2);
        //    tmpA2.appendChild(tmpI2);
            
        //    var tmpUL2 = document.createElement("<ul class='dropdown-menu'>");
        //    dropdown.appendChild(tmpA2).appendChild(tmpUL2);
        //}else{
        //dropdown= $('<li>', {
        //    'class': 'dropdown pull-right hide tabdrop'
        //}).append(
        //    $('<a>', {
        //        'class': 'dropdown-toggle',
        //        'data-toggle': 'dropdown',
        //        'href': '#'
        //    }).append(
        //        $('<i>', {'class': "glyphicon glyphicon-align-justify"})
        //    ).append(
        //        $('<b>', {'class': 'caret'})
        //    )
        //).append(
        //    $('<ul>', {'class': "dropdown-menu"})
        //)
        //}
        //检测是否已增加
        if (!$('.tabdrop').html()) {
            //$('.nav-tabs').append(dropdown);
            element.append(dropdown);
            //insertAdjacentElement(element,dropdown);
            //element.appendChild(dropdown);
            //dropdown.prependTo(element);
        } else {
            dropdown = element.find('.tabdrop');
        }
        //检测是否有下拉样式
        if (element.parent().is('.tabs-below')) {
            //dropdown.addClass('dropup');
            dropdown.setAttribute('class', 'dropup');
        }
        var collection = 0;

        //检查超过一行的标签页
        element.append(dropdown.getElementsByTagName('li'))
            .find('>li')
            .not('.tabdrop')
            .each(function () {
                if (this.offsetTop > 0 || element.width() - $(this).position().left - $(this).width() < 53) {
                    dropdown.find('ul').append($(this));
                    collection++;
                }
            });

        //如果有超出的，显示下拉标签
        if (collection > 0) {
            //dropdown.removeClass('hide');
            dropdown.setAttribute("class", 'hide');
            if (dropdown.getAttribute('.active').length == 1) {
                dropdown.addClass('active');
                dropdown.setAttribute('class','active');
            } else {
                //dropdown.removeClass('active');
                dropdown.removeAttribute("class", "active")
            }
        } else {
            dropdown.setAttribute("class",'hide');
        }
    }
}