$(document).ready(function () {
    var surnamesCount = 0;
    $('#add_surname').click(function () {
        $('<input>').attr({
            type: 'text',
            class: 'form-control',
            placeholder: 'Фамилия автора',
            name: 'sn_' + surnamesCount
        }).appendTo('#surnames');
        surnamesCount += 1;
    });

    $('#add_surname').trigger('click');
});

var new_surname = `
<div class="py-1">
    <input type="text" class="form-control" placeholder="Фамилия автора">
</div>`;