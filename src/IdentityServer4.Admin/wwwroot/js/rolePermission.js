$(function () {
    $('#user').addClass('active');
    new Vue({
        el: '#view',
        data: {
            els: [],
            permissions: []
        },
        mounted: function () {
            loadView(this);
        },
        methods: {
            remove: function (id) {
                let that = this;
                swal({
                    title: "Sure to remove this role?",
                    type: "warning",
                    showCancelButton: true
                }, function () {
                    app.delete("/api/role/" + app.getPathPart(window.location.href, 1) + "/permission/" + id, function () {
                        loadView(that);
                    });
                });
            },
            showAddPermission: function () {
                $('#add-role').modal('show');
                $('.select2').select2({
                    minimumResultsForSearch: Infinity
                });
            },
            addPermission: function () {
                let that = this;
                app.post("/api/role/" + app.getPathPart(window.location.href, 1) + "/permission/" + $('.select2').val(), null, function () {
                    loadView(that);
                });
            }
        }
    });

    function loadView(vue) {
        let url = '/api/role/' + app.getPathPart(window.location.href, 1) + '/permission';
        app.get(url, function (result) {
            vue.$data.els = result.data.result;
        });
        let getRoleUrl = '/api/permission?page=1&size=1000';
        app.get(getRoleUrl, function (result) {
            vue.$data.permissions = result.data.result;
            $('.select2').val(result.data.result[0].id)
        });
    }
});

