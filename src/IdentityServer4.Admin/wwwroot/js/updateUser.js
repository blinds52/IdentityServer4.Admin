$(function () {
    new Vue({
        el: '#view',
        data: {
            activeMenu: '用户管理',
            menus: menus,
            module: '编辑用户',
            moduleDescription: '',
            breadcrumb: [{
                name: '用户管理',
                href: '/user'
            }, {
                name: '编辑',
                href: '#'
            }],
            userName: '',
            email: '',
            phoneNumber: '',
            lastName: '',
            firstName: '',
            officePhone: '',
            sex: 'Male',
            title: '',
            group: '',
            level: ''
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
        created: function () {
            loadView(this);
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
                let data = this.$data;
                app.put("/api/user/" + app.getPathPart(window.location.href, 1), data, function () {
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
        const url = '/api/user/' + app.getPathPart(window.location.href, 1);
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
        });
    }
});

