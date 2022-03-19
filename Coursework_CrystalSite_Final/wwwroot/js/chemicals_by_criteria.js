$(document).ready(function () {
    $('input:not([data-bs-toggle="collapse"])').prop('disabled', true);
    $('[data-bs-toggle="collapse"]').click(function () {
        var collapse_target = $(this).attr('data-bs-target');
        $(collapse_target).find('input').prop('disabled', function (i, val) {
            return !val;
        });
    });


    $(':reset').click(function () {
        $('.collapse').removeClass('show');
        $('input:not([data-bs-toggle="collapse"])').prop('disabled', true);
    });

    $('.BriefChemical > span').click(function () {
        if ($(this).hasClass(ActiveClass)) {
            $(this).removeClass(ActiveClass);
            $('.BriefChemical > span').removeClass(InactiveClass);
            $('#table_query_result * tbody > tr').show();
        } else {
            $('.BriefChemical > span').removeClass(ActiveClass);
            $('.BriefChemical > span').addClass(InactiveClass);
            $(this).addClass(ActiveClass);
            $(this).removeClass(InactiveClass);
            var chemical = $(this).html();
            $('#table_query_result * tbody > tr').filter(function () {
                $(this).toggle($(this).children('td').html().indexOf(chemical) > -1);
            });
        }
    });
});

//var ActiveClass = 'border border-primary border-1 rounded';
var ActiveClass = 'shadow p-2 mb-5 bg-light rounded';
var InactiveClass = 'text-muted';