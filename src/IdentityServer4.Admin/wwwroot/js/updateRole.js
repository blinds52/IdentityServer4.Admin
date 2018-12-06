$(function () {
    new Vue({
        el: '#view',
        data: {
            activeMenu: '角色管理',
            menus: menus,
            module: '编辑角色',
            moduleDescription: '',
            breadcrumb: [{
                name: '角色管理',
                href: '/role'
            }, {
                name: '编辑',
                href: '#'
            }],
            name: '',
            description: ''
        },
        created: function () {
            loadView(this);
        },
        methods: {
            update: function () {
                app.put("/api/role/" + app.getPathPart(window.location.href, 1), this.$data, function () {
                    window.location.href = '/role';
                }, function (result) {
                    swal({
                        title: "更新失败",
                        type: "error",
                        text: result.msg,
                        showCancelButton: true
                    });
                });
            }
        }
    });

    function loadView(vue) {
        const url = '/api/role/' + app.getPathPart(window.location.href, 1);
        app.get(url, function (result) {
            vue.$data.name = result.data.name;
            vue.$data.description = result.data.description;
        });
    }
});

