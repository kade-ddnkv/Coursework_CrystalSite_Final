$(document).ready(function () {
    var dataTable = $('#tableQueryResult * table').first().DataTable({
        "pageLength": 25
    });

    $('.BriefChemical > span').click(function () {
        var html = $(this).prop('outerHTML');
        dataTable.search($(html).text()).draw();
    });
});