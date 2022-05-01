$(document).ready(function () {
    // Основная логика: проставить обработкики на нажатия в таблице Менделеева.
    set_clicks_on_elements();

    // Для кнопки "Наверх".
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
    });

    // Проверка url-параметров.
    if_url_contains_element_or_property();
});

function if_url_contains_element_or_property() {
    var urlParams = new URLSearchParams(window.location.search);
    var chemical_id = urlParams.get('chemical_id');
    if (chemical_id !== null) {
        $('#selected_chemical * span').load('/DataByChemical/get-chemical-formula?chemical_id=' + chemical_id,
            function (responseTxt, statusTxt, xhr) {
                if (statusTxt == "error") {
                    alert("Error: " + xhr.status + ": " + xhr.statusText);
                    return;
                }
                load_properties_accordion(chemical_id);

                var property_id = urlParams.get('property_id');
                if (property_id !== null) {
                    // Жду, пока подгрузятся свойства.
                    wait_for_el('#accordionProperty', function () {
                        // Скролл к нужному свойству и его открытие.
                        var property = $('#accordionProperty * [data-property-id="' + property_id + '"]');
                        $('html, body').scrollTop(property.offset().top);
                        property.trigger('click');
                    });
                }
            }
        );
    }
}

function wait_for_el(selector, callback) {
    if (jQuery(selector).length) {
        callback();
    } else {
        setTimeout(function () {
            wait_for_el(selector, callback);
        }, 100);
    }
};

// Для всплывающих подсказок по нажатию.
function activate_all_popovers() {
    $('[data-bs-toggle="popover"]').popover({ html: true });
}

function set_clicks_on_elements() {
    $('.Element').click(function () {
        // Здесь нужно заполнить все нужные области placeholder-ами.
        // И все ненужное очистить.
        $('#properties_accordion').empty();
        $('#selected_chemical * span').empty();

        $(this).toggleClass('Active');

        var elements = [];
        $('.Active').each(function (i, obj) {
            elements.push($(obj).find('abbr').text());
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

        load_properties_accordion(chemical_id);
    });
}

function load_properties_accordion(chemical_id) {
    $('#properties_accordion').load('/DataByChemical/get-properties-by-chemical?chemical_id=' + chemical_id,
        function (responseTxt, statusTxt, xhr) {
            if (statusTxt == "error") {
                alert("Error: " + xhr.status + ": " + xhr.statusText);
                return;
            }

            set_clicks_on_properties(chemical_id);
            $('html, body').scrollTop($('#selected_chemical').offset().top);
        }
    );
}

function set_clicks_on_properties(chemical_id) {
    $('.accordion-button').click(function () {
        // Открывать можно только существующие свойства.
        if ($(this).hasClass("text-muted")) {
            return;
        }

        var is_expanded = $(this).attr("aria-expanded");
        // Загружать данные только в случае открытия аккордеона.
        if (is_expanded == 'true') {
            var collapse_target = $(this).attr('data-bs-target') + ' > .accordion-body';
            var query_params = {
                chemical_id: chemical_id,
                property_id: $(this).attr('data-property-id')
            }

            $(collapse_target).load('/DataByChemical/get-property-table?' + $.param(query_params), function (responseTxt, statusTxt, xhr) {
                if (statusTxt == "error") {
                    alert("Error: " + xhr.status + ": " + xhr.statusText);
                }

                var dataTable = $(collapse_target + '* table').first().DataTable({
                    "pageLength": 25,
                    "searching": false
                });

                // Появляется ссылка на графики, если они есть у текущего соединения и свойства.
                if ($(collapse_target + ' * .insivible-chart-link').length > 0) {
                    var chartLink = $(collapse_target + ' * .insivible-chart-link').first();
                    var oldUrl = chartLink.attr("href");
                    var newUrl = oldUrl + $.param(query_params);
                    chartLink.attr("href", newUrl);

                    $.get(newUrl, function (data, status) {
                        chartLink.removeClass("insivible-chart-link");
                    });
                }
            });
        }
    });
}