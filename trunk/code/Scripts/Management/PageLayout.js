$(function () {
    //获得页面宽高
    var pageWidth = document.body.clientWidth;
    var pageHeight = document.body.clientHeight;

    //设置页面高度
    $("#mainBody").height(pageHeight - 100);
    alert(pageHeight - 100);

});

