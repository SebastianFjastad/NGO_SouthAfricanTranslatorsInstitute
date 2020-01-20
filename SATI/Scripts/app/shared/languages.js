
function languages(saveUrl) {

    $('#saveMemberLanguage').click(function () {

        var languageId = $('#languageDropdown').val();
        var memberId = $("#User_Id").val();

        $.post(saveUrl,
            { memberId: memberId, languageId: languageId },
            function () {
                window.location.reload();
            });
    });

}