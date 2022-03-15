//(function ($) {
//    $.fn.click_on_element = function () {
//        alert('hello world');
//        return this;
//    };
//})(jQuery);

$(document).ready(function () {
    set_clicks_on_elements();

    $(window).on('scroll', function () {
        if (
            document.body.scrollTop > 20 ||
            document.documentElement.scrollTop > 20
        ) {
            $('#btn-back-to-top').css('display', 'block');
        } else {
            $('#btn-back-to-top').css('display', 'none');
        }
    });

    $('#btn-back-to-top').click(function () {
        //window.scrollTo(0, 0);
        document.body.scrollTop = 0;
        document.documentElement.scrollTop = 0;
    })
});

function set_clicks_on_elements() {
    $('.Element').click(function () {
        // Здесь нужно заполнить все нужные области placeholder-ами.
        // И все ненужное очистить.
        $('#properties_accordion').empty();
        $('#selected_chemical > span').empty();

        $(this).toggleClass('Active');

        var elements = [];
        $('.Active').each(function (i, obj) {
            elements.push($(obj).children('abbr').text());
        });
        var elements_joined = '-' + elements.join('-') + '-';

        $('#found_chemicals').load('/DataByChemical/get-chemicals-by-elements?elements=' + elements_joined,
            function (responseTxt, statusTxt, xhr) {
                if (statusTxt == "error")
                    alert("Error: " + xhr.status + ": " + xhr.statusText);

                set_clicks_on_found_chemicals();
            }
        );
    });
}

function set_clicks_on_found_chemicals() {
    $('.Chemical').click(function () {
        var chemical_id = $(this).attr('data-chemical-id');
        $('#selected_chemical * span').html($(this).html());

        $('#properties_accordion').load('/DataByChemical/get-properties-by-chemical?chemical_id=' + chemical_id, function (responseTxt, statusTxt, xhr) {
            if (statusTxt == "error")
                alert("Error: " + xhr.status + ": " + xhr.statusText);

            set_clicks_on_properties(chemical_id);
        });
    });
}

function set_clicks_on_properties(chemical_id) {
    $('.accordion-button').click(function () {
        var is_expanded = $(this).attr("aria-expanded");
        // Загружать данные только в случае открытия аккордеона.
        if (is_expanded == 'true') {
            var collapse_target = $(this).attr('data-bs-target');
            var query_params = {
                chemical_id: chemical_id,
                property_id: $(this).attr('data-property-id')
            }

            $(collapse_target).load('/DataByChemical/get-property-table?' + $.param(query_params), function (responseTxt, statusTxt, xhr) {
                if (statusTxt == "error")
                    alert("Error: " + xhr.status + ": " + xhr.statusText);
            });
        }
    });
}