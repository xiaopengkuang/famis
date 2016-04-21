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

           
            var title;
            if ($.browser.msie) {
                title = document.createElement("<li role='presentation' id='tab_" + id + "'></li>");
                var tmpA = document.createElement("<a href='#" + id + "' aria-controls='" + id + "' role='tab' data-toggle='tab'></a>");
                tmpA.innerHTML(opts.title);
                title.appendChild(tmpA)
            } else { 
                title = $('<li>', {
                    'role': 'presentation',
                    'id': 'tab_' + id
                }).append(
                    $('<a>', {
                        'href': '#' + id,
                        'aria-controls': id,
                        'role': 'tab',
                        'data-toggle': 'tab'
                    }).html(opts.title)
                );
            }

            //是否允许关闭
            if (Addtabs.options.close) {

                if ($.browser.msie) {
                    var tmpLI = document.createElement("<i class='close-tab glyphicon glyphicon-remove'></i>");
                    title.appendChild(tmpLI);
                } else{
                  title.append(
                    $('<i>',{class:'close-tab glyphicon glyphicon-remove'})
                );
                }

              
            }

          
                //if($.browser.msie) {

                //    var txt = document.createElement("<input type='text' name='txtName'>")

                //} else {

                //    var txt = document.createElement("input");

                //    txt.type = "text";

                //    txt.name = "txtName";

                //}

            //创建新TAB的内容
            var content;
            if ($.browser.msie) {
                content = document.createElement("<div class='tab-pane' id='" + id + "' role='tabpanel'></div>");
            }else{
                content= $('<div>', {
                    'class': 'tab-pane',
                    'id': id,
                    'role': 'tabpanel'
                });
            }
            //是否指定TAB内容
            if (opts.content) {
                content.append(opts.content);
            } else if (Addtabs.options.iframeUse && !opts.ajax) {//没有内容，使用IFRAME打开链接
                if ($.browser.msie) {
                    var tmpIFrame = document.createElement("<iframe class='iframeClass' height='" + Addtabs.options.iframeHeight + "' frameborder='no' src='" + opts.url + "'></iframe>");
                    content.appendChild(tmpIFrame);
                }else{

                    content.append(
                        $('<iframe>', {
                            'class': 'iframeClass',
                            'height': Addtabs.options.iframeHeight,
                            'frameborder': "no",
                            'border': "0",
                            'src': opts.url
                        })
                    );
                }
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
        var dropdown;

        if ($.browser.msie) {

            dropdown = document.createElement("<li class='dropdown pull-right hide tabdrop'></li>");
            var tmpA2 = document.createElement("<a class='dropdown-toggle' data-toggle='dropdown' href='#'></a>");
            var tmpI2 = document.createElement("<i class='glyphicon glyphicon-align-justify'></i>");
            var tmpB2 = document.createElement("<b class='caret'></b>");
            tmpI2.appendChild(tmpB2);
            tmpA2.appendChild(tmpI2);
            
            var tmpUL2 = document.createElement("<ul class='dropdown-menu'></ul>");
            dropdown.appendChild(tmpA2).appendChild(tmpUL2);
        }else{
        dropdown= $('<li>', {
            'class': 'dropdown pull-right hide tabdrop'
        }).append(
            $('<a>', {
                'class': 'dropdown-toggle',
                'data-toggle': 'dropdown',
                'href': '#'
            }).append(
                $('<i>', {'class': "glyphicon glyphicon-align-justify"})
            ).append(
                $('<b>', {'class': 'caret'})
            )
        ).append(
            $('<ul>', {'class': "dropdown-menu"})
        )
        }
        //检测是否已增加
        if (!$('.tabdrop').html()) {
            dropdown.prependTo(element);
        } else {
            dropdown = element.find('.tabdrop');
        }
        //检测是否有下拉样式
        if (element.parent().is('.tabs-below')) {
            dropdown.addClass('dropup');
        }
        var collection = 0;

        //检查超过一行的标签页
        element.append(dropdown.find('li'))
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
            dropdown.removeClass('hide');
            if (dropdown.find('.active').length == 1) {
                dropdown.addClass('active');
            } else {
                dropdown.removeClass('active');
            }
        } else {
            dropdown.addClass('hide');
        }
    }
}