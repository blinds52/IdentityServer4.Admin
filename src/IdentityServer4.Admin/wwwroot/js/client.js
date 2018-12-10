$(function () {
    new Vue({
        el: '#view',
        data: {
            activeMenu: '客户端管理',
            menus: menus,
            module: '客户端管理',
            moduleDescription: '',
            breadcrumb: [{
                name: '客户端管理',
                href: '#'
            }],
            els: [],
            page: app.getUrlParam('page') || 1,
            size: app.getUrlParam('size') || 16,
            total: 0
        },
        created: function () {
            loadView(this);
        },
        methods: {
            remove: function (id) {
                const that = this;
                swal({
                    title: "确定要删除此客户端吗",
                    type: "warning",
                    showCancelButton: true
                }, function () {
                    app.delete("/api/client/" + id, function () {
                        loadView(that);
                    });
                });
            }
        }
    });

    function loadView(vue) {
        const url = '/api/client?keyword=' + vue.$data.keyword + '&page=' + vue.$data.page + '&size=' + vue.$data.size;
        app.get(url, function (result) {
            vue.$data.els = result.data.result;
            vue.$data.total = result.data.total;
            vue.$data.page = result.data.page;
            vue.$data.size = result.data.size;

            app.ui.initPagination('#pagination', result.data, function (page) {
                window.location.href = '/api/client?keyword=' + vue.$data.keyword + '&page=' + page + '&size=' + vue.$data.size;
            });
        });
    }
});

