
$.ajax({

    type: "post",
    url: "/AssetDeatails/Get_Serial",

    datatype: "json",//数据类型

    success: function (result) {
        var searial = result;
        $.ajax({

            type: "post",
            url: "/AssetDeatails/Get_Asset_Deatail_BySearial?Json=" + searial + "",

            datatype: "json",//数据类型

            success: function (result) {
                //alert(result);
                var ZCBH = result.split(",")[0];
                $('#ZCBH').val(ZCBH);
                var ZCXZ = result.split(",")[1];
                $('#ZCXZ').val(ZCXZ);
                var ZCLB = result.split(",")[2];
                $('#ZCLB').val(ZCLB);
                var ZCMC = result.split(",")[3];
                $('#ZCMC').val(ZCMC);
                var GGXH = result.split(",")[4];
                $('#GGXH').val(GGXH);
                var JLDW = result.split(",")[5];
                $('#JLDW').val(JLDW);
                var SZBM= result.split(",")[6];
                $('#SZBM').val(SZBM);
                var SYR = result.split(",")[7];
                $('#SYR').val(SYR);
                var CFDD = result.split(",")[8];
                $('#CFDD').val(CFDD);
                var ZJFS = result.split(",")[9];
                $('#ZJFS').val(ZJFS);
                var GYS = result.split(",")[10];
                $('#GYS').val(GYS);
                var LXR = result.split(",")[11];
                $('#LXR').val(LXR);
                var GYSDZ = result.split(",")[12];
                $('#GYSDZ').val(GYSDZ);
                var GZRQ= result.split(",")[13];
                $('#GZRQ').val(GZRQ);
                var ZCZT = result.split(",")[14];
                 $('#ZCZT').val(ZCZT);
               var DJRQ = result.split(",")[15];
                $('#DJRQ').val(DJRQ);
                var SYNX = result.split(",")[16];
                $('#SYNX').val(SYNX);
                var  ZJFS= result.split(",")[17];
                $('#ZJFS').val(ZJFS);
                var JCZL = result.split(",")[18];
                $('#JCZL').val(JCZL);
                var ZCDJ = result.split(",")[19];
                $('#ZCDJ').val(ZCDJ);
                var ZCSL = result.split(",")[20];
                $('#ZCSL').val(ZCSL);
                var ZCJZ = result.split(",")[21];
                $('#ZCJZ').val(ZCJZ);
                var YTZJ = result.split(",")[21];
                $('#YTZJ').val(YTZJ);
                var LJZJ = result.split(",")[22];
                $('#LJZJ').val(LJZJ);
                var JZ = result.split(",")[23];
                $('#JZ').val(JZ);

            }, error: function (msg) {
                alert("未能传出资产编号的值!");
            }
        });

    }, error: function (msg) {
        alert("未能传出资产编号的值!");
    }
});
