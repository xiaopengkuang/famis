function reloadTabGrid(title) {
    if ($("#tabs").tabs('exists', title)) {
        $('#tabs').tabs('select', title);
        window.top.reload_datagrid_collar.call();
    }
}