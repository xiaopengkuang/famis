//==============================================================================//
var searchCondition = "";

//==============================================================================//

$(function () {
    updateData();

});




function updateData()
{
    //获取数据
    var searchTypeInfo = $('#searchType option:selected').val();//选中的值
    var AttrOfAsset = $('#Attr_asset option:selected').val();//选中的值
    var groupByInfo = $("input[name='value_Type']:checked").val();
    var dataInfo = {
        "searchTypeInfo": searchTypeInfo,
        "AttrOfAsset": AttrOfAsset,
        "groupByInfo": groupByInfo
    };
    //searchCondition = jQuery.stringify(dataInfo)
    searchCondition = JSON.stringify(dataInfo);
    loadData();
}



function loadData()
{
    $("#contanis").empty();
    var dom = document.getElementById('contanis');
    dom.style.width = document.body.clientWidth-40;
    var mycharts = echarts.init(dom);
    // 接下来就是 ajax部分了 动态加载数据才是根本的 数据固定多没意思

    $.ajax({
        type: "POST",
        async: true,            //异步请求（同步请求将会锁住<a href="/projecteactual/web-server-browser-cache-1.html" class="keylink" title=" 浏览器的缓存原理及缓存方式" target="_blank">浏览器</a>，用户其他操作必须等待请求完成才可以执行）
        url: "/Index/loadStatisticsInfo?searchCondition=" + searchCondition,
        data: {},//demo 没加条件
        dataType: "json",        //返回数据形式为json
        beforeSend: ajaxLoading,
        success: function (result) {
            ajaxLoadEnd()
            var legendFlag = true;
            var name = new Array();
            

            for (var i = 0; i < result.length; i++) {
                if (i >50) {
                    name.push(false);
                } else {
                    name.push(result[i].name);

                }
            }
            option = {
                title: {
                    text: '资产数据一览',
                    subtext: '',
                    x: 'center'
                },
                tooltip: {
                    trigger: 'item',
                    formatter: "{a} <br/>{b} : {c} ({d}%)"
                },
                legend: {
                    orient: 'vertical',
                    left: 'right',
                    data: name
                },
                series: [
                    {
                        name: '资产数据',
                        type: 'pie',
                        radius: '30%',
                        center: ['40%', '50%'],
                        data: result,
                        itemStyle: {
                            emphasis: {
                                shadowBlur: 10,
                                shadowOffsetX: 0,
                                shadowColor: 'rgba(0, 0, 0, 0.5)' //这怎么会有个.5呢？ 看来还是要看看H5哟
                            }
                        }
                    }
                ]
            };
            mycharts.setOption(option);


        },
        error: function (errorMsg) {
            //请求失败时执行该函数
            alert("图表请求数据失败!");

        }

    });
}
//采用jquery easyui loading css效果
function ajaxLoading() {
    $("<div class=\"datagrid-mask\"></div>").css({ display: "block", width: "100%", height: $(window).height() }).appendTo("body");
    $("<div class=\"datagrid-mask-msg\"></div>").html("正在处理，请稍候。。。").appendTo("body").css({ display: "block", left: ($(document.body).outerWidth(true) - 190) / 2, top: ($(window).height() - 45) / 2 });
}
function ajaxLoadEnd() {
    $(".datagrid-mask").remove();
    $(".datagrid-mask-msg").remove();
}
