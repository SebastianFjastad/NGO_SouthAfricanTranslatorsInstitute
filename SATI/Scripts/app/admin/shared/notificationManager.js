function NotificationManager() {

    return {
        showLoading: function () {
            $('#ajaxLoading').show();
        },
        hideLoading: function () {
            $('#ajaxLoading').hide();
        }
    }
}