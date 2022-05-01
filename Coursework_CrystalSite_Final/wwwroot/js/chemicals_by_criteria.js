$(document).ready(function () {
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


    $(':reset').click(function () {
        $('.collapse').removeClass('show');
        $('input:not([data-bs-toggle="collapse"])').prop('disabled', true);
    });

    $('[data-param-target]').change(function () {
        var param_name = $(this).attr('data-param-name');
        var target = $(this).attr('data-param-target');
        $('[data-param-range-name=' + param_name + ']').prop('disabled', true);
        $('[data-param-range-name=' + param_name + ']').removeClass('activeParamRange');
        $('[data-param-range-id=' + target + ']').prop('disabled', false);
        $('[data-param-range-id=' + target + ']').addClass('activeParamRange');
    });
});