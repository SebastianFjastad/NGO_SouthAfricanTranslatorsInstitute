function DataService() {
    function makeRequest(params, path, callback, method) {
        globals.notificationManager.showLoading();

        var dataToSend = ko.mapping.toJSON(params);

        $.ajax({
            type: method,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            url: path,
            data: dataToSend,
            cache: false
        })
            .done(function (result) {
                callback(result);
            })
            .fail(function (e) {
            })
            .always(function () {
                globals.notificationManager.hideLoading();
            });
    }

    function get(params, path, callback) { makeRequest(params, path, callback, "GET") }

    function post(params, path, callback) { makeRequest(params, path, callback, "POST") }

    return {
        get: get,
        post: post
    }
}