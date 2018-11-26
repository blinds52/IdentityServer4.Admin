$(function () {
    $('#role').addClass('active');
    new Vue({
        el: '#view',
        data: {
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