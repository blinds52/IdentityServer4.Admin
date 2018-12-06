$(function() {
    new Vue({
        el: '#view',
        data: {
            activeMenu: '诊断',
            menus: menus,
            module: '诊断',
            moduleDescription: '',
            breadcrumb: [
                {
                    name: '诊断',
                    href: '#'
                }
            ]
        }
    });
});