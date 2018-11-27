$(function () {
    $('#user').addClass('active');
    new Vue({
        el: '#view',
        data: {
            userName: '',
            email: '',
            password: '',
            phoneNumber: ''
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