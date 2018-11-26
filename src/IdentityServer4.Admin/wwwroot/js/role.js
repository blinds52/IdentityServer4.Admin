$(function () {
    $('#role').addClass('active');
    new Vue({
        el: '#view',
        data: {
            els: [],
            page: app.getUrlParam('page') || 1,
            size: app.getUrlParam('size') || 16,
            total: 0
        },
        mounted: function () {
            loadView(this);
        },
        methods: {
            remove: function (id) {
                const that = this;
                swal({
                    title: "Sure to remove this permission?",
                    type: "warning",
                    showCancelButton: true
                }, function () {
                    app.delete("/api/role/" + id, function () {
                       loadView(that);
                    });
                });
            }
        }
    });

    function loadView(vue) {
        const url = '/api/role?page=' + vue.$data.page + '&size=' + vue.$data.size;
        app.get(url, function (result) {
            vue.$data.els = result.data.result;
            vue.$data.total = result.data.total;
            vue.$data.page = result.data.page;
            vue.$data.size = result.data.size;

            app.ui.initPagination('#pagination', result.data, function (page) {
                window.location.href = '/api/role?page=' + page + '&size=' + vue.$data.size;
            });
        });
    }
});

