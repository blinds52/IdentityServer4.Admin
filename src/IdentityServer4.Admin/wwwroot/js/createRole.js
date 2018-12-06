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
            name: '',
            description: ''
        },
        methods: {
            create: function () {
                app.post('/api/role', this.$data, function () {
                    window.location.href = '/role';
                });
            }
        }
    });
});