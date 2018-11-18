$(function () {
    $('#apiResource').addClass('active');
    const vue = new Vue({
        el: '#view',
        data: {
            els: [],
            page: app.queryString('page') || 1,
            size: app.queryString('size') || 16,
            total: 0,
            keyword: decodeURIComponent(app.queryString('keyword') || '')
        },
        mounted: function () {
            loadView(this);
        },
        methods: {
            remove: function (id) {
                const that = this;
                swal({
                    title: "Sure to remove this job?",
                    type: "warning",
                    showCancelButton: true
                }, function () {
                    app.delete("/swarm/v1.0/job/" + id, function () {
                        loadView(that);
                    });
                });
            }
        }
    });

    function loadView(vue) {
        const url = '/api/apiResource?keyword=' + vue.$data.keyword + '&page=' + vue.$data.page + '&size=' + vue.$data.size;
        app.get(url, function (result) {
            vue.$data.els = result.data.result;
            vue.$data.total = result.data.total;
            vue.$data.page = result.data.page;
            vue.$data.size = result.data.size;

            app.ui.initPagination('#pagination', result.data, function (page) {
                window.location.href = '/api/apiResource?keyword=' + vue.$data.keyword + '&page=' + page + '&size=' + vue.$data.size;
            });
        });
    }
});

