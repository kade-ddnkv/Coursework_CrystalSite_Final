$(document).ready(function () {
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
});