$(document).ready(function () {
    var surnamesCount = 0;
    $('#add_surname').click(function () {
        $('<input>').attr({
            type: 'text',
            class: 'form-control',
            placeholder: 'Фамилия автора',
            name: 'sn_' + surnamesCount++
        }).appendTo('#surnames');
    });

    $('#add_surname').trigger('click');
});