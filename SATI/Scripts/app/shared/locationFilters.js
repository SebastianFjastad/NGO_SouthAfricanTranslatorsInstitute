
$(function () {

    var $countryDropdown, $regionDropdown, $areaDropdown;

    if ($('#User_CountryId').length) {
        $countryDropdown = $('#User_CountryId');
        $regionDropdown = $('#User_RegionId');
        $areaDropdown = $('#User_AreaId');
    }
    else if ($('#CountryId_dd').length) {
        $countryDropdown = $('#CountryId_dd');
        $regionDropdown = $('#RegionId_dd');
        $areaDropdown = $('#AreaId_dd');
    }

    if ($countryDropdown && $countryDropdown.length) {
        $countryDropdown.change(function () {
            filterRegions();
            $('[data-region]', $areaDropdown).hide();
            $regionDropdown.val('');
            $areaDropdown.val('');
        })

        $regionDropdown.change(function () {
            var regionId = $(this).val();
            filterAreas(regionId);
            $areaDropdown.val('');
        })

        function filterRegions() {
            var countryId = $countryDropdown.val();
            $('[data-country]', $regionDropdown).hide();
            $('[data-country="' + countryId + '"]', $regionDropdown).show();
        }

        function filterAreas() {
            var regionId = $regionDropdown.val();
            $('[data-region]', $areaDropdown).hide();
            $('[data-region="' + regionId + '"]', $areaDropdown).show();
        }

        (function init() {
            filterRegions();
            filterAreas();
        })();
    }
})