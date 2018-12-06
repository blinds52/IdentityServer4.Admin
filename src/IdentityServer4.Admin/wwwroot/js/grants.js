$(function() {
    new Vue({
        el: '#view',
        data: {
            activeMenu: '授权记录',
            menus: menus,
            module: '授权记录',
            moduleDescription: '',
            breadcrumb: [
                {
                    name: '授权记录',
                    href: '#'
                }
            ]
        }
    });
});