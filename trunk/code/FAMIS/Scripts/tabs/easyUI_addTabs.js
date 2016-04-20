function addTab(title, url) {
    if ($('#tabs').tabs('exists', title)) {
        $('#tabs').tabs('select', title);
    } else {
        var content = '<iframe scrolling="auto" frameborder="0"  src="' + url + '" style="width:100%;height:100%;"></iframe>';
        $('#tabs').tabs('add', {
            title: title,
            content: content,
            closable: true
        });
    }
}

function goNewPage(url, title) {
    addTab(title, url);
}

$(document).ready(function () {

    goNewPage("/Home/Welcome", "Welcom");
});
$( function () {
    goNewPage("/Home/Welcome", "Welcom");
});

