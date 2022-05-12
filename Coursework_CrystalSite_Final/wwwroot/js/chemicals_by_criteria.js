$(document).ready(function () {
    // Работа с Составом элементов.
    var elementsCount = 0;
    $('#add_element').click(function () {
        $('<input>').attr({
            type: 'text',
            class: 'form-control',
            placeholder: 'Nb',
            name: 'Состав соединения:el_' + elementsCount++,
        }).appendTo('#elements');
    });
    $('#add_element').trigger('click');

    // Работа с disabled.
    $('input,select').not('[data-bs-toggle="collapse"]').prop('disabled', true);
    $('[data-bs-toggle="collapse"]').click(function () {
        var collapse_target = $(this).attr('data-bs-target');
        // Инвертирую атрибут disabled.
        $(collapse_target).find('input,select').prop('disabled', function (i, val) {

            // Свойства с выбором параметра.
            var attr = $(this).attr('data-param-range-name');
            if (typeof attr !== 'undefined' && attr !== false) {
                // Инвертировать только включенный параметр.
                if ($(this).hasClass("activeParamRange")) {
                    return !val;
                } else {
                    return true;
                }
            }

            return !val;
        });
    });

    // Работа с критериями, у которых есть выбор по параметру.
    $('[data-param-target]').change(function () {
        var param_name = $(this).attr('data-param-name');
        var target = $(this).attr('data-param-target');
        $('[data-param-range-name=' + param_name + ']').prop('disabled', true);
        $('[data-param-range-name=' + param_name + ']').removeClass('activeParamRange');
        $('[data-param-range-id=' + target + ']').prop('disabled', false);
        $('[data-param-range-id=' + target + ']').addClass('activeParamRange');
    });
});