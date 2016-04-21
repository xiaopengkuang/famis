var idx = 1;
function addPanel() {
    $('#aa').accordion('add', {
        title: 'Title' + idx,
        content: '<div style="padding:10px">Content' + idx + '</div>'
    });
    idx++;
}