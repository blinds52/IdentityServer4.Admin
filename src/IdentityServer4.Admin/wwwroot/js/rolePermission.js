$(function () {
    new Vue({
        el: '#view',
        data: {
            activeMenu: '角色管理',
            menus: menus,
            module: '角色权限',
            moduleDescription: '',
            breadcrumb: [{
                name: '角色管理',
                href: '/role'
            }, {
                name: '权限',
                href: '#'
            }],
            els: [],
            permissions: []
        },
        created: function () {
            loadView(this);
        },
        methods: {
            remove: function (id) {
                let that = this;
                swal({
                    title: "确定要删除此权限吗?",
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
            vue.$data.total = result.data.total;
            vue.$data.page = result.data.page;
            vue.$data.size = result.data.size;
        });
        let getRoleUrl = '/api/permission?page=1&size=1000';
        app.get(getRoleUrl, function (result) {
            vue.$data.permissions = result.data.result;
            if (result.data.result.length > 0)
                $('.select2').val(result.data.result[0].permissionId)
        });
    }
});

