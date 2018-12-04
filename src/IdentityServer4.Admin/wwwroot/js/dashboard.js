$(function () {
    $('#role').addClass('home');
    new Vue({
        el: '#view',
        data: {
            userCount: 0,
            clientCount: 0,
            apiResourceCount: 0,
            lockedUserCount: 0
        },
        mounted: function () {
            loadView(this);
        }
    });

    function loadView(vue) {
        const url = '/api/dashboard';
        app.get(url, function (result) {
            vue.$data.userCount = result.data.userCount;
            vue.$data.clientCount = result.data.clientCount;
            vue.$data.apiResourceCount = result.data.apiResourceCount;
            vue.$data.lockedUserCount = result.data.lockedUserCount;
        });
    }
});

