$(function () {
    new Vue({
        el: '#view',
        data: {
            activeMenu: '首页',
            menus: menus,
            module: '首页',
            moduleDescription: '',
            breadcrumb: [],
            userCount: 0,
            clientCount: 0,
            apiResourceCount: 0,
            lockedUserCount: 0
        },
        created: function () {
            loadView(this);
        },
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

