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
            checkRole: function () {
                this.errors = [];

                if (!app.requireCheck(this.el.name)) {
                    this.errors.push("角色名不能为空");
                }
                else {
                    if (!app.rangeCheck(this.el.name, 4, 30)) {
                        this.errors.push("角色名长度范围为 4-30");
                    }
                }
                if (app.requireCheck(this.el.description)) {
                    if (!app.maxCheck(this.el.description, 1000)) {
                        this.errors.push("角色描述不能超过 1000 字符");
                    }
                }

                return !this.errors.length
            },
            update: function () {
                if (this.checkRole()) {
                    app.put("/api/role/" + app.getPathPart(window.location.href, 1), this.$data, function () {
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
        const url = '/api/role/' + app.getPathPart(window.location.href, 1);
        app.get(url, function (result) {
            vue.$data.name = result.data.name;
            vue.$data.description = result.data.description;
        });
    }
});

