$(function () {
    new Vue({
        el: '#view',
        data: {
            activeMenu: '权限管理',
            menus: menus,
            module: '编辑权限',
            moduleDescription: '',
            breadcrumb: [{
                name: '权限管理',
                href: '/permission'
            }, {
                name: '编辑',
                href: '#'
            }],
            el: {
                name: '',
                description: ''
            },
            errors: []
        },
        created: function () {
            loadView(this);
        },
        methods: {
            checkPermission: function () {
                this.errors = [];

                if (!app.requireCheck(this.el.name)) {
                    this.errors.push("权限名不能为空");
                }
                else {
                    if (!app.rangeCheck(this.el.name, 4, 30)) {
                        this.errors.push("权限名长度范围为 4-30");
                    }
                }
                if (app.requireCheck(this.el.description)) {
                    if (!app.maxCheck(this.el.description, 1000)) {
                        this.errors.push("权限描述不能超过 1000 字符");
                    }
                }

                return !this.errors.length
            },
            update: function () {
                if (this.checkPermission()) {
                    app.put("/api/permission/" + app.getPathPart(window.location.href, 1), this.$data, function () {
                        swal({
                            title: "更新成功",
                            type: "success",
                            showCancelButton: false
                        });
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
        }
    });

    function loadView(vue) {
        const url = '/api/permission/' + app.getPathPart(window.location.href, 1);
        app.get(url, function (result) {
            vue.$data.name = result.data.name;
            vue.$data.description = result.data.description;
        });
    }
});

