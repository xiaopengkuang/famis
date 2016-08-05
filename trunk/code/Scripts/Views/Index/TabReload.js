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