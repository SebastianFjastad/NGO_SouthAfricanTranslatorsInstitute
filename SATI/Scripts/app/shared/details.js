//Members details page

$(function () {
    $('[data-membertype] input').change(function () {
        var $individualLabels = $('[data-individual]');
        var $companyLabels = $('[data-company]')
        if ($(this).val() === 'Company') {
            $individualLabels.hide();
            $companyLabels.show();
        }
        else {
            $individualLabels.show();
            $companyLabels.hide();
            $('#User_CompanyName').val('');
        }

        $('#detailsForm').submit(function (e) { 
            if (!$(this).valid())
                e.preventDefault();
        })
    })
})