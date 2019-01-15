$(function () {
    new Vue({
        el: '#view',
        data: {
            activeMenu: '角色管理',
            menus: menus,
            module: '创建角色',
            moduleDescription: '',
            breadcrumb: [{
                name: '角色管理',
                href: '/role'
            }, {
                name: '创建',
                href: '#'
            }],
            el: {
                name: '',
                description: ''
            },
            errors: [],
        },
        methods: {
            checkRole: function () {
                this.errors = [];

                if (!app.requireCheck(this.el.name)) {
                    this.errors.push("角色名不能为空");
                }
                else {
                    if (!app.rangeCheck(this.el.name.length, 4, 30)) {
                        this.errors.push("角色名长度范围为 4-30");
                    }
                }
                if (app.requireCheck(this.el.description)) {
                    if (!app.maxCheck(this.el.description.length, 1000)) {
                        this.errors.push("角色描述不能超过 1000 字符");
                    }
                }

                return !this.errors.length
            },
            create: function () {
                if (this.checkRole()) {
                    app.post('/api/role', this.$data.el, function () {
                        window.location.href = '/role';
                    });
                }
            }
        }
    });
});