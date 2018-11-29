$(function () {
    $('#user').addClass('active');
    new Vue({
        el: '#view',
        data: {
            els: [],
            page: app.getUrlParam('page') || 1,
            size: app.getUrlParam('size') || 16,
            total: 0,
            keyword: decodeURIComponent(app.getUrlParam('keyword') || '')
        },
        mounted: function () {
            loadView(this);
        },
        methods: {
            remove: function (id) {
                const that = this;
                swal({
                    title: "确定要删除此用户吗?",
                    type: "warning",
                    showCancelButton: true
                }, function () {
                    app.delete("/api/user/" + id, function () {
                        loadView(that);
                    });
                });
            },
            search: function () {
                loadView(this);
            }
        }
    });

    function loadView(vue) {
        const url = '/api/user?keyword=' + vue.$data.keyword + '&page=' + vue.$data.page + '&size=' + vue.$data.size;
        app.get(url, function (result) {
            vue.$data.els = result.data.result;
            vue.$data.total = result.data.total;
            vue.$data.page = result.data.page;
            vue.$data.size = result.data.size;

            app.ui.initPagination('#pagination', result.data, function (page) {
                window.location.href = '/api/user?keyword=' + vue.$data.keyword + '&page=' + page + '&size=' + vue.$data.size;
            });
        });
    }
});

