
function Search(satiAdminBaseUrl) {
    if (!satiAdminBaseUrl) {
        alert('SATI Admin URL must be passed in on construction, eg. new Search(http://realurl.com/Search/Index)')
        return false;
    }

    // #region Variables
    var $searchContainer = $('#searchContainer');
    var $memberDetailsContainer = $('#memberDetailsContainer');
    var $fromLanguageDropdown,
        $toLanguageDropdown,
        $clearFiltersBtn,
        $skillsDropdown,
        $searchResults,
        $searchForm
    // #endregion

    // #region Search Loading

    //bind the variables once the search DOM fragment has loaded
    function initBindings() {
        $fromLanguageDropdown = $searchContainer.find('#FromLanguageId');
        $toLanguageDropdown = $searchContainer.find('#ToLanguageId');
        $skillsDropdown = $searchContainer.find('#SkillId');
        $searchResults = $searchContainer.find('#searchResults');
        $searchForm = $searchContainer.find('#searchFormContainer form');
    }

    function initDataTable() {
        $searchResults.find('#resultsTable').DataTable({'order': []});
        $('#loadingSpinner').hide();
    }

    //load the search page from the SATI server
    $searchContainer.load(satiAdminBaseUrl + '/Search', {}, initBindings);

    $searchContainer.on('submit', 'form', function (e) {
        e.preventDefault();
        $searchContainer.find('#search').click();
    })

    //submit the search params
    $searchContainer.on('click', '#search', function () {
        $('#loadingSpinner').show();
        var searchParams = $searchForm.serialize();
        $searchResults.load(satiAdminBaseUrl + '/Results', searchParams, initDataTable);
    })

    //load the member details
    $searchContainer.on('click', '[data-member-id]', function () {
        var memberId = $(this).data('member-id');
        $memberDetailsContainer.load(satiAdminBaseUrl + '/MemberDetails', { id: memberId },
            function () {
                $searchContainer.fadeOut('fast', function () { $memberDetailsContainer.fadeIn('fast') });
            });
    })

    // #endregion

    // #region UI Behaviour

    //filter out To dropdown value which is in Form
    $searchContainer.on('change', '#FromLanguageId', function () {
        var selectedId = $(this).val();
        if (selectedId) {
            $toLanguageDropdown.val('');
            $toLanguageDropdown.find('option').show();
            $toLanguageDropdown.find('option[value="' + selectedId + '"]').hide();
        }
    })

    //clear all form filters
    $searchContainer.on('click', '#clearFilters', function (e) {
        $('#searchFormContainer').find('input, select').val('');
    })

    //when the skills change then filter out languages that are transitive (if only one language is required)
    $searchContainer.on('change', '#SkillId', function () {
        if ($('option:selected', this).data('istransitive') == 'False') {
            $toLanguageDropdown.hide().val('');
            $fromLanguageDropdown.find('option:first-child').text('Language');
        }
        else {
            $fromLanguageDropdown.find('option:first-child').text('From Language');
            $toLanguageDropdown.show();
        }
    })

    $memberDetailsContainer.on('click', '#backBtn', function () {
        $memberDetailsContainer.fadeOut('fast', function () { $searchContainer.fadeIn('fast') })
    })

    // #endregion
}