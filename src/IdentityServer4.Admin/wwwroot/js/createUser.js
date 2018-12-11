$(function () {
    new Vue({
        el: '#view',
        data: {
            activeMenu: '用户管理',
            menus: menus,
            module: '创建用户',
            moduleDescription: '',
            breadcrumb: [{
                name: '用户管理',
                href: '/user'
            }, {
                name: '创建',
                href: '#'
            }],
            el: {
                userName: '',
                email: '',
                password: '',
                phoneNumber: '',
                officePhone: '',
                firstName: '',
                lastName: '',
                sex: 'Male',
                title: '',
                group: '',
                level: ''
            },
            errors: []
        },
        watch: {
            sex: function (val) {
                $("#sex").val(val).trigger('change');
            },
            title: function (val) {
                $("#title").val(val).trigger('change');
            },
            group: function (val) {
                $("#group").val(val).trigger('change');
            },
            level: function (val) {
                $("#level").val(val).trigger('change');
            }
        },
        mounted: function () {
            let that = this;
            $('.select2').select2({
                minimumResultsForSearch: Infinity
            });
            $("#sex").on('change', function (e) {
                that.el.sex = $("#sex").val();
            });
            $("#title").on('change', function (e) {
                that.el.title = $("#title").val();
            });
            $("#group").on('change', function (e) {
                that.el.group = $("#group").val();
            });
            $("#level").on('change', function (e) {
                that.el.level = $("#level").val();
            });
        },
        methods: {
            checkUser: function () {
                this.errors = [];

                if (!app.requireCheck(this.el.userName)) {
                    this.errors.push("用户名不能为空");
                }
                else {
                    if (!app.rangeCheck(this.el.userName.length, 4, 30)) {
                        this.errors.push("用户名长度范围为 4-30");
                    }
                }
                if (!app.emailCheck(this.el.email)) {
                    this.errors.push("邮箱地址不正确");
                }
                if (!app.rangeCheck(this.el.password.length, 6, 24)) {
                    this.errors.push("密码长度范围为 6-24");
                }
                if (!app.mobileCheck(this.el.phoneNumber)) {
                    this.errors.push("手机号码不正确");
                }
                if (!app.phoneCheck(this.el.officePhone)) {
                    this.errors.push("公司电话不正确");
                }
                if (!app.requireCheck(this.el.firstName)) {
                    this.errors.push("姓 不能为空");
                }
                else {
                    if (!app.rangeCheck(this.el.firstName.length, 1, 50)) {
                        this.errors.push("姓 长度范围为 1-50");
                    }
                }
                if (!app.requireCheck(this.el.lastName)) {
                    this.errors.push("名 不能为空");
                }
                else {
                    if (!app.rangeCheck(this.el.lastName.length, 1, 50)) {
                        this.errors.push("名 长度范围为 1-50");
                    }
                }
                return !this.errors.length
            },
            create: function () {
                let that = this;
                if (that.checkUser()) {
                    app.post('/api/user', this.$data.el, function () {
                        window.location.href = '/user';
                    }, function (result) {
                        that.errors = [];
                        that.errors.push(result.msg);
                    });
                }
            }
        }
    });
});