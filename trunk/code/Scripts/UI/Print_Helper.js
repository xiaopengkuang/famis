//test();
function test() {

    $.ajax({

        type: "post",
        url: "/SysSetting/Getbase64",

        datatype: "json",//数据类型

        success: function (result) {
            // alert(result);
            document.getElementById("insert").innerHTML = '<img src="' + "data:image/png;base64," + result + '" height="65" width="200"  />'; //'<img src="' + "data:image/png;base64," + result + '" height="" width="200"  />';
        }, error: function (msg) {

            alert("msg");
        }
    });
}
function toPrint() {

    window.print();
    alert('正在打印中，请您稍候……\n\n打印完成后请您按确定返回。');

}

var LODOP; //声明为全局变量 

function myPreview() {

    CreatePrintPage();
    //LODOP.PRINT();
    LODOP.PREVIEW();

};

function CreatePrintPage() {


    LODOP = getLodop(document.getElementById('LODOP_OB'), document.getElementById('LODOP_EM'));



    var strFormHtml = document.getElementById("insert").innerHTML;

    //这里的”divdiv1“是标签的名称。

    LODOP.PRINT_INIT(".....");

    LODOP.SET_PRINT_PAGESIZE(1, 790, 250, "4×1"); //A4纸张纵向打印

    LODOP.SET_PRINT_STYLE("FontSize", 9);

    LODOP.ADD_PRINT_HTM("5%", "15%", "100%", "100%", strFormHtml);
    LODOP.SET_PRINT_STYLEA(0, "Stretch", 2);

    //四个数值分别表示Top,Left,Width,Height

};
function Get_BitMap() {

}
 
function myPreview3(ID,type) {

    $.ajax({

        type: "post",
        url: "/SysSetting/Getbase64?Asset_ID=" + ID + "",

        datatype: "json",//数据类型

        success: function (result) {
            LODOP = getLodop();

            LODOP.PRINT_INIT("条形码打印机_SH");


            LODOP.SET_PRINT_PAGESIZE(1, 790, 220, "4×1"); 
            LODOP.ADD_PRINT_IMAGE(0, 60, 790, 200, '<img border="0" src="' + "data:image/png;base64," + result + '"  />');//"<img border='0' src='http://s1.sinaimg.cn/middle/4fe4ba17hb5afe2caa990&690' />");
            LODOP.SET_PRINT_STYLEA(0, "Stretch", 2);//按原图比例(不变形)缩放模式
           
                LODOP.PRINT();
            
           // LODOP.PREVIEW();
        }, error: function (msg) {

            alert("msg");
        }
    });

};