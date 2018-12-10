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
            name: '',
            description: ''
        },
        methods: {
            create: function () {
                app.post('/api/permission', this.$data, function () {
                    window.location.href = '/permission';
                });
            }
        }
    });
});