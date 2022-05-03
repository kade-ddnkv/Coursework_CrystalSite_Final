$(document).ready(function () {
    // Проставить обработчики на нажатия в таблице Менделеева.
    set_clicks_on_elements();
});

function set_clicks_on_elements() {
    $('.Element').click(function () {
        $(this).toggleClass('Active');

        var elements = [];
        $('.Active').each(function (i, obj) {
            elements.push($(obj).find('abbr').text());
        });

        if (elements.length > 0) {
            var elements_joined = '-' + elements.join('-') + '-';
            $('#found_chemicals').load('/search-chemicals/by-elements?elements=' + elements_joined,
                function (responseTxt, statusTxt, xhr) {
                    if (statusTxt == "error")
                        alert("Error: " + xhr.status + ": " + xhr.statusText);
                }
            );
        } else {
            $('#found_chemicals').empty();
        }
    });
}