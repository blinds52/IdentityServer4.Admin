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
            userName: '',
            email: '',
            phoneNumber: '',
            oldPassword: '',
            newPassword: '',
            lastName: '',
            firstName: '',
            officePhone: '',
            roles: '',
            sex: 'Male',
            title: '',
            group: '',
            level: ''
        },
        created: function () {
            loadView(this);
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
                that.sex = $("#sex").val();
            });
            $("#title").on('change', function (e) {
                that.title = $("#title").val();
            });
            $("#group").on('change', function (e) {
                that.group = $("#group").val();
            });
            $("#level").on('change', function (e) {
                that.level = $("#level").val();
            });
        },
        methods: {
            update: function () {
                app.put("/api/user/" + app.getPathPart(window.location.href, 1) + '/profile', {
                    userName: this.$data.userName,
                    email: this.$data.email,
                    phoneNumber: this.$data.phoneNumber,
                    lastName: this.$data.lastName,
                    firstName: this.$data.firstName,
                    officePhone: this.$data.officePhone,
                    sex: this.$data.sex,
                    title: this.$data.title,
                    group: this.$data.group,
                    level: this.$data.level
                }, function () {
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
            },
            changePassword: function () {
                app.put("/api/user/" + app.getPathPart(window.location.href, 1) + '/password', {
                    oldPassword: this.$data.oldPassword,
                    newPassword: this.$data.newPassword
                }, function () {
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
    });

    function loadView(vue) {
        const url = '/api/user/' + app.getPathPart(window.location.href, 1) + '/profile';
        app.get(url, function (result) {
            vue.$data.userName = result.data.userName;
            vue.$data.email = result.data.email;
            vue.$data.phoneNumber = result.data.phoneNumber;
            vue.$data.lastName = result.data.lastName;
            vue.$data.firstName = result.data.firstName;
            vue.$data.officePhone = result.data.officePhone;
            vue.$data.sex = result.data.sex;
            vue.$data.group = result.data.group;
            vue.$data.title = result.data.title;
            vue.$data.level = result.data.level;
            vue.$data.roles = result.data.roles;
        });
    }
});

