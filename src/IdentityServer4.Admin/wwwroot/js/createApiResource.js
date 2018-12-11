$(function () {
    new Vue({
        el: '#view',
        data: {
            activeMenu: 'API 资源管理',
            menus: menus,
            module: '添加 API 资源',
            moduleDescription: '',
            breadcrumb: [{
                name: 'API 资源管理',
                href: '/apiResource'
            }, {
                name: '创建',
                href: '#'
            }],
            el: {
                name: '',
                displayName: '',
                description: '',
                enabled: 'True',
                userClaims: ''
            },
            errors: []
        },
        watch: {
            enabled: function (val) {
                $("#enabled").val(val).trigger('change');
            }
        },
        mounted: function () {
            let that = this;
            $('.select2').select2({
                minimumResultsForSearch: Infinity
            });
            $("#enabled").on('change', function (e) {
                that.el.enabled = $("#enabled").val();
            });
        },
        methods: {
            create: function () {
                const apiResource = this.$data.el;
                const url = "/api/apiResource";
                if (apiResource.userClaims) {
                    apiResource.userClaims = apiResource.userClaims.split(';');
                } else {
                    apiResource.userClaims = [];
                }
                apiResource.enabled = apiResource.enabled === 'True';
                app.post(url, apiResource, function () {
                    window.location.href = '/apiResource';
                });
            }
        }
    });
});