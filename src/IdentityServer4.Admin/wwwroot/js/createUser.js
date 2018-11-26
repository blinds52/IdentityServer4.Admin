$(function () {
    $('#apiResource').addClass('active');
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
                const user = this.$data;
                app.post('/api/user', user, function () {
                    window.location.href = '/user';
                });
            }
        }
    });
});