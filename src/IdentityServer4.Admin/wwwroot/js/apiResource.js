$(function () {
    new Vue({
        el: '#view',
        data: {
            activeMenu: 'API 资源管理',
            menus: menus,
            module: 'API 资源管理',
            moduleDescription: '',
            breadcrumb: [{
                name: 'API 资源管理',
                href: '#'
            }],
            els: [],
            page: app.getUrlParam('page') || 1,
            size: app.getUrlParam('size') || 16,
            total: 0,
            q: decodeURIComponent(app.getUrlParam('q') || '')
        },
        created: function () {
            loadView(this);
        },
        methods: {
            remove: function (id) {
                const that = this;
                swal({
                    title: "确定要删除此 API 资源吗",
                    type: "warning",
                    showCancelButton: true
                }, function () {
                    app.delete("/api/api-resource/" + id, function () {
                        loadView(that);
                    });
                });
            }
        }
    });

    function loadView(vue) {
        const url = '/api/api-resource?q=' + vue.$data.q + '&page=' + vue.$data.page + '&size=' + vue.$data.size;
        app.get(url, function (result) {
            vue.$data.els = result.data.result;
            vue.$data.total = result.data.total;
            vue.$data.page = result.data.page;
            vue.$data.size = result.data.size;

            app.ui.initPagination('#pagination', result.data, function (page) {
                window.location.href = '/api-resource?q=' + vue.$data.q + '&page=' + page + '&size=' + vue.$data.size;
            });
        });
    }
});

