$(function () {
    new Vue({
        el: '#view',
        data: {
            activeMenu: '权限管理',
            menus: menus,
            module: '权限管理',
            moduleDescription: '',
            breadcrumb: [{
                name: '权限管理',
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
                    title: "确定要删除此权限吗?",
                    text: '删除权限会把用户权限和角色权限级连删除',
                    type: "warning",
                    showCancelButton: true
                }, function () {
                    app.delete("/api/permission/" + id, function () {
                        loadView(that);
                    });
                });
            }
        }
    });

    function loadView(vue) {
        const url = '/api/permission?page=' + vue.$data.page + '&size=' + vue.$data.size;
        app.get(url, function (result) {
            vue.$data.els = result.data.result;
            vue.$data.total = result.data.total;
            vue.$data.page = result.data.page;
            vue.$data.size = result.data.size;

            app.ui.initPagination('#pagination', result.data, function (page) {
                window.location.href = '/api/permission?page=' + page + '&size=' + vue.$data.size;
            });
        });
    }
});

