function reloadTabGrid(title) {
    if (title == "资产领用")
    {
        reloadDataGrid_collar(title);
    }else if(title=="资产调拨")
    {
        reloadDataGrid_allocation(title);
    }else if(title=="资产维修")
    {
        reloadDataGrid_repair(title);
    }else if(title=="资产借出")
    {
        reloadDataGrid_borrow(title);
    }
    else if (title == "资产归还") {
        reloadDataGrid_return(title);
    }
    else if (title == "资产减少") {
        reloadDataGrid_reduction(title);
    }
}


function reloadDataGrid_collar(title)
{
    if ($("#tabs").tabs('exists', title)) {
        $('#tabs').tabs('select', title);
        //window.top.reload_datagrid_collar.call();
        try {
            window.top.reload_datagrid_collar.call();
        } catch (e) {
        }
    }
}


function reloadDataGrid_allocation(title) {
    if ($("#tabs").tabs('exists', title)) {
        $('#tabs').tabs('select', title);
        try {
            window.top.reload_datagrid_allocation.call();
        } catch (e) {
        }
    }
}

function reloadDataGrid_repair(title) {
    if ($("#tabs").tabs('exists', title)) {
        $('#tabs').tabs('select', title);
        try {
            window.top.reload_datagrid_repair.call();
        } catch (e) {
        }
    }
}

function reloadDataGrid_borrow(title) {
    if ($("#tabs").tabs('exists', title)) {
        $('#tabs').tabs('select', title);
        try {
            window.top.reload_datagrid_borrow.call();
        } catch (e) {
        }
    }
}

function reloadDataGrid_return(title) {
    if ($("#tabs").tabs('exists', title)) {
        $('#tabs').tabs('select', title);
        try {
            window.top.reload_datagrid_return.call();
        } catch (e) {
        }
    }
}
function reloadDataGrid_reduction(title) {
    if ($("#tabs").tabs('exists', title)) {
        $('#tabs').tabs('select', title);
        try{
            window.top.reload_datagrid_Reduction.call();
        }catch(e){
        }
    }
}




function reloadMyReMindTabs()
{
    if ($("#tabs").tabs('exists', "首页")) {
        //$('#tabs').tabs('select', "首页");
        //MessShow("要刷新了！");
        try {
            window.top.reload_Index_MYRED.call();
        } catch (e) {
        }
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
