$(function () {
    new Vue({
        el: '#view',
        data: {
            activeMenu: '权限管理',
            menus: menus,
            module: '创建权限',
            moduleDescription: '',
            breadcrumb: [{
                name: '权限管理',
                href: '/permission'
            }, {
                name: '创建',
                href: '#'
            }],
            el: {
                name: '',
                description: ''
            },
            errors: []
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
            create: function () {
                if (this.checkPermission()) {
                    app.post('/api/permission', this.$data, function () {
                        window.location.href = '/permission';
                    });
                }
            }
        }
    });
});