$(function () {
    new Vue({
        el: '#view',
        data: {
            activeMenu: '个人信息',
            menus: menus,
            module: '个人信息',
            moduleDescription: '',
            breadcrumb: [{
                name: '个人信息',
                href: '#'
            }],
            el: {
                userName: '',
                email: '',
                phoneNumber: '',
                lastName: '',
                firstName: '',
                officePhone: '',
                roles: '',
                sex: 'Male',
                title: '',
                group: '',
                level: ''
            },
            changePasswordDto: {
                oldPassword: '',
                newPassword: '',
                repeatPassword: ''
            },
            errors: []
        },
        created: function () {
            loadView(this);
        },
        watch: {
            el: function (val) {
                $("#sex").val(val.sex).trigger('change');
                $("#title").val(val.title).trigger('change');
                $("#group").val(val.group).trigger('change');
                $("#level").val(val.level).trigger('change');
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
            checkProfile: function () {
                this.errors = [];

                if (!app.requireCheck(this.el.userName)) {
                    this.errors.push("用户名不能为空");
                } else {
                    if (!app.rangeCheck(this.el.userName.length, 4, 30)) {
                        this.errors.push("用户名长度范围为 4-30");
                    }
                }
                if (!app.emailCheck(this.el.email)) {
                    this.errors.push("邮箱地址不正确");
                }
                if (!app.mobileCheck(this.el.phoneNumber)) {
                    this.errors.push("手机号码不正确");
                }
                if (!app.phoneCheck(this.el.officePhone)) {
                    this.errors.push("公司电话不正确");
                }
                if (!app.requireCheck(this.el.firstName)) {
                    this.errors.push("姓 不能为空");
                } else {
                    if (!app.rangeCheck(this.el.firstName.length, 1, 50)) {
                        this.errors.push("姓 长度范围为 1-50");
                    }
                }
                if (!app.requireCheck(this.el.lastName)) {
                    this.errors.push("名 不能为空");
                } else {
                    if (!app.rangeCheck(this.el.lastName.length, 1, 50)) {
                        this.errors.push("名 长度范围为 1-50");
                    }
                }
                return !this.errors.length
            },
            checkPassword: function () {
                this.errors = [];
                if (!app.requireCheck(this.changePasswordDto.oldPassword)) {
                    this.errors.push("旧密码不能为空");
                }
                if (!app.requireCheck(this.changePasswordDto.newPassword)) {
                    this.errors.push("新密码不能为空");
                }
                {
                    if (!app.rangeCheck(this.changePasswordDto.newPassword.length, 6, 24)) {
                        this.errors.push("密码长度范围为 6-24");
                    }
                }
                if (!app.equalCheck(this.changePasswordDto.newPassword, this.changePasswordDto.repeatPassword)) {
                    this.errors.push("两次密码不一致");
                }
                return !this.errors.length
            },
            update: function () {
                if (this.checkProfile()) {
                    app.put("/api/user/" + app.getPathPart(window.location.href, 1) + '/profile', this.$data.el, function () {
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
                            showCancelButton: false
                        });
                    });
                }
            },
            changePassword: function () {
                if (this.checkPassword()) {
                    app.put("/api/user/" + app.getPathPart(window.location.href, 1) + '/password', this.$data.changePasswordDto, function () {
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
                            showCancelButton: false
                        });
                    });
                }
            }
        }
    });

    function loadView(vue) {
        const url = '/api/user/' + app.getPathPart(window.location.href, 1) + '/profile';
        app.get(url, function (result) {
            vue.$data.el = {};
            vue.$data.el.userName = result.data.userName;
            vue.$data.el.email = result.data.email;
            vue.$data.el.phoneNumber = result.data.phoneNumber;
            vue.$data.el.lastName = result.data.lastName;
            vue.$data.el.firstName = result.data.firstName;
            vue.$data.el.officePhone = result.data.officePhone;
            vue.$data.el.sex = result.data.sex;
            vue.$data.el.group = result.data.group;
            vue.$data.el.title = result.data.title;
            vue.$data.el.level = result.data.level;
            vue.$data.el.roles = result.data.roles;
        });
    }
});

