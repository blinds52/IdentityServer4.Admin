$(function () {
    $('#apiResource').addClass('active');
    new Vue({
        el: '#view',
        data: {
            el: {
                name: '',
                displayName: '',
                description: '',
                enabled: true,
                secrets: [],
                scopes: [],
                userClaims: ''
            },
            scope: {
                name: '',
                displayName: '',
                description: '',
                required: false,
                emphasize: false,
                showInDiscoveryDocument: true,
                userClaims: ''
            },
            secret: {
                value: '',
                description: '',
                expiration: '',
                type: ''
            }
        },
        mounted: function () {
            $('.select2').select2({
                minimumResultsForSearch: Infinity
            });
            $('#scopeRequired').val('False');
            $('#scopeEmphasize').val('False');
            $('#scopeSidd').val('True');
        },
        methods: {
            addSecret: function () {
                this.$data.el.secrets.push({
                    value: this.$data.secret.value.trim(),
                    description: this.$data.secret.description.trim(),
                    expiration: this.$data.secret.expiration,
                    type: this.$data.secret.type.trim()
                });
                this.$data.secret = {};
            },
            removeSecret: function (secret) {
                for (let i = 0; i < this.$data.el.secrets.length; ++i) {
                    if (this.$data.el.secrets[i].value === secret.value) {
                        this.$data.el.secrets.splice(i, 1);
                        return;
                    }
                }
            },
            addScope: function () {
                this.$data.el.scopes.push({
                    name: this.$data.scope.name.trim(),
                    displayName: this.$data.scope.displayName.trim(),
                    description: this.$data.scope.description.trim(),
                    required: $('#scopeRequired').val(),
                    emphasize: $('#scopeEmphasize').val(),
                    showInDiscoveryDocument: $('#scopeSidd').val(),
                    userClaims: this.$data.scope.userClaims.trim().split(';')
                });
                this.$data.scope = {};
            },
            removeScope: function (scope) {
                for (let i = 0; i < this.$data.el.scopes.length; ++i) {
                    if (this.$data.el.scopes[i].name === scope.name) {
                        this.$data.el.scopes.splice(i, 1);
                        return;
                    }
                }
            },
            create: function () {
                const apiResource = this.$data.el;
                const url = "/api/apiResource";
                apiResource.userClaims = apiResource.userClaims.split(';');
                app.post(url, apiResource, function () {
                    window.location.href = '/apiResource';
                });
            }
        }
    });
});