$(function () {
    new Vue({
        el: '#view',
        data: {
            activeMenu: '客户端管理',
            menus: menus,
            module: '添加客户端',
            moduleDescription: '',
            breadcrumb: [{
                name: '客户端管理',
                href: '#'
            }, {
                name: '创建',
                href: '#'
            }],
            el: {
                clientId: '',
                clientName: '',
                allowedGrantTypes: 'Implicit',
                allowAccessTokensViaBrowser: 'True',
                allowedCorsOrigins: '',
                redirectUris: '',
                postLogoutRedirectUris: '',
                requireConsent: 'True',
                allowedScopes: '',
                description: ''
            }
        },
        watch: {
            allowAccessTokensViaBrowser: function (val) {
                $("#allowAccessTokensViaBrowser").val(val).trigger('change');
            },
            allowedGrantTypes: function (val) {
                $("#allowedGrantTypes").val(val).trigger('change');
            },
            requireConsent: function (val) {
                $("#requireConsent").val(val).trigger('change');
            }
        },
        mounted: function () {
            let that = this;
            $('.select2').select2({
                minimumResultsForSearch: Infinity
            });
            $("#allowAccessTokensViaBrowser").on('change', function (e) {
                that.el.allowAccessTokensViaBrowser = $("#allowAccessTokensViaBrowser").val();
            });
            $("#allowedGrantTypes").on('change', function (e) {
                that.el.allowedGrantTypes = $("#allowedGrantTypes").val();
            });
            $("#requireConsent").on('change', function (e) {
                that.el.requireConsent = $("#requireConsent").val();
            });
        },
        methods: {
            create: function () {
                const client = this.$data.el;
                const url = "/api/client";
                app.post(url, client, function () {
                    window.location.href = '/client';
                });
            }
        }
    });
});