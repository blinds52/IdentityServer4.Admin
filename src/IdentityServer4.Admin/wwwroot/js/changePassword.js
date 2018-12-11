$(function () {
    new Vue({
        el: '#view',
        data: {
            activeMenu: '用户管理',
            menus: menus,
            module: '修改密码',
            moduleDescription: '',
            breadcrumb: [{
                name: '用户管理',
                href: '/user'
            }, {
                name: '修改密码',
                href: '#'
            }],
            el: {
                newPassword: '',
                repeatPassword: ''
            },
            errors: []
        },
        methods: {
            checkPassword: function () {
                this.errors = [];
                if (!app.equalCheck(this.el.newPassword, this.el.repeatPassword)) {
                    this.errors.push("两次密码不一致");
                }
                else {
                    if (!app.rangeCheck(this.el.newPassword.length, 6, 24)) {
                        this.errors.push("密码长度范围为 6-24");
                    }
                }
                return !this.errors.length
            },
            update: function () {
                let that = this;
                if (that.checkPassword()) {
                    app.put("/api/user/" + app.getPathPart(window.location.href, 1) + '/changepassword', that.$data.el, function () {
                        swal({
                            title: "更新成功",
                            type: "success",
                            showCancelButton: false
                        });
                    }, function (result) {
                        that.errors = [];
                        that.errors.push(result.msg);
                    });
                }
            }
        }
    });
});

