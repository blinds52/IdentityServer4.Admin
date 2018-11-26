$(function () {
    $('#user').addClass('active');
    new Vue({
        el: '#view',
        data: {
            els: [],
            roles: [],
            role: ''
        },
        mounted: function () {
            loadView(this);
        },
        methods: {
            remove: function (id) {
                let that = this;
                swal({
                    title: "确定要删除此角色吗?",
                    type: "warning",
                    showCancelButton: true
                }, function () {
                    app.delete("/api/user/" + app.getPathPart(window.location.href, 1) + "/role/" + id, function () {
                        loadView(that);
                    });
                });
            },
            showAddRole: function () {
                $('#add-role').modal('show');
                $('.select2').select2({
                    minimumResultsForSearch: Infinity
                });
            },
            addRole: function () {
                let that = this;
                app.post("/api/user/" + app.getPathPart(window.location.href, 1) + "/role/" + $('.select2').val(), null, function () {
                    loadView(that);
                });
            }
        }
    });

    function loadView(vue) {
        let url = '/api/user/' + app.getPathPart(window.location.href, 1) + '/role';
        app.get(url, function (result) {
            vue.$data.els = result.data;
        });
        let getRoleUrl = '/api/role?page=1&size=1000';
        app.get(getRoleUrl, function (result) {
            vue.$data.roles = result.data.result;
            vue.$data.role = vue.$data.roles[0].id;
        });
    }
});

