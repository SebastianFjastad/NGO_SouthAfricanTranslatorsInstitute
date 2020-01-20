//Members Capabilities Page

function capabilities(
    memberId,
    addCapabilityUrl,
    capabilityDetailsUrl,
    saveSpecializationUrl,
    saveAccreditationUrl,
    saveIsAccreditedUrl) {

    var accreditationsPickerOriginal = '';
    var specialisationsPickerOriginal = '';

    var $skillsDropdown = $('#skillsDropdown');
    var $fromLanguageDropdown = $('#fromLanguage');
    var $toLanguageDropdown = $('#toLanguage');
    var $saveCapabilityBtn = $('#saveCapability');
    var $addCapabilityErrorMessage = $('#saveCapabilityError');


    $skillsDropdown.change(function () {
        if ($('option:selected', this).data('is-transitive') === 'False') {
            $toLanguageDropdown.val('');
            $toLanguageDropdown.closest('#toLanguageContainer').hide();
        } else {
            $toLanguageDropdown.closest('#toLanguageContainer').show();
        }
    });

    $fromLanguageDropdown.change(function () {
        //filter out To dropdown value which is in From
        var selectedId = $(this).val();

        if (selectedId) {
            $toLanguageDropdown.val('');
            $toLanguageDropdown.find('option').show();
            $toLanguageDropdown.find('option[value="' + selectedId + '"]').hide();
        }
    });

    $('#clearSaveCapability').click(function () {
        clearCapabilityDropdowns();
    });

    function clearCapabilityDropdowns() {
        $skillsDropdown.add($fromLanguageDropdown).add($toLanguageDropdown).val('');
    }

    $saveCapabilityBtn.click(function () {
        if (!validateAddCapability()) return false;

        var params = {
            memberId: memberId,
            skillId: $skillsDropdown.val(),
            fromLanguageId: $fromLanguageDropdown.val(),
            toLanguageId: $toLanguageDropdown.val()
        }

        $.post(addCapabilityUrl, params, addCapabilityComplete);
    });

    function validateAddCapability() {
        return true;
    }

    function addCapabilityComplete(response) {
        if (response.Success) {
            window.location.reload();
        } else if (response.ErrorMessage) {
            $addCapabilityErrorMessage.text(response.ErrorMessage).show();
        }
    }

    $('#capabilitiesTable').on('click', '[data-capability-id]', function () {
        var $tr = $(this).closest('tr');

        $tr.siblings().removeClass('selectedRow');

        $tr.addClass('selectedRow');

        $.get(capabilityDetailsUrl,
            { capabilityId: $(this).data('capability-id'), memberId: memberId },
            function (result) {
                var $result = $(result);

                accreditationsPickerOriginal = $result[8].innerHTML;

                specialisationsPickerOriginal = $result[4].innerHtml;

                $('#capabilityDetails').html(result);
            });
    });

    //pop across picker code
    var $container = $('#capabilityDetails');

    $container.on('click', '.editSpecialisations', () => {
        $('[data-specialisations-picker]').show();
    });

    $container.on('click', '.editAccreditations', () => {
        $('[data-accreditations-picker]').show();
    });

    $container.on('click', '[data-unselected] tr, [data-selected] tr', function () {
        $(this).toggleClass('selectedRow');
    });

    //on click right arrow then take selected left and add to right hand panel
    $container.on('click', '.addBtn', function () {
        var selectedRows = $(this)
            .closest('div')
            .siblings('[data-unselected]')
            .find('tr.selectedRow').remove();

        $(this).closest('div')
            .siblings('[data-selected]')
            .find('table tbody')
            .append(selectedRows);

        deselectAllRows();
    });

    $container.on('click', '.removeBtn', function () {
        var selectedRows = $(this)
            .closest('div')
            .siblings('[data-selected]')
            .find('tr.selectedRow').remove();

        $(this).closest('div')
            .siblings('[data-unselected]')
            .find('table tbody')
            .append(selectedRows);

        deselectAllRows();
    });

    function deselectAllRows() {
        $container.find('.selectedRow').removeClass('selectedRow');
    }

    //close picker dialog
    $container.on('click', '.closeBtn', function () {
        var $pickerContainer = $(this).closest('.pickerContainer');
        $pickerContainer.hide();

        $pickerContainer.html(accreditationsPickerOriginal);
    });

    //save picker data
    $container.on('click', '.saveAttributesBtn', function () {
        var $pickerContainer = $(this).closest('.pickerContainer');
        var isSpecialisationsDialog = $pickerContainer.is('[data-specialisations-picker]');
        var ids = _.map($pickerContainer.find('[data-selected] [data-selected-id]'), function (x) {
            return $(x).data('selected-id');
        });

        var capabilityId = $('#Capability_CapabilityId').val();

        var params = { capabilityId: capabilityId, ids: ids };

        var saveUrl = isSpecialisationsDialog ? saveSpecializationUrl : saveAccreditationUrl;

        $.post(saveUrl, params, function () { window.location.reload() });
    });

    //save isAccredited
    $container.on('click', '.saveIsAccreditedBtn', function () {
        var capabilityId = $('#Capability_CapabilityId').val();
        var isAccredited = $('#Capability_IsAccredited').is(':checked');
        $.post(saveIsAccreditedUrl, { capabilityId: capabilityId, isAccredited: isAccredited }, function () { window.location.reload();})
    })
}