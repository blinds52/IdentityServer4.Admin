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
            create: function () {
                app.post('/api/user', this.$data, function () {
                    window.location.href = '/user';
                });
            }
        }
    });
});