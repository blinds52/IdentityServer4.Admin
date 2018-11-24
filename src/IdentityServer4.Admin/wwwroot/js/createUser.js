$(function () {
    $('#apiResource').addClass('active');
    new Vue({
        el: '#view',
        data: {
            userName: '',
            email: '',
            password: '',
            phone: ''
        },
        methods: {
            create: function () {
                const user = this.$data.el;
                app.post('/api/user', user, function () {
                    window.location.href = '/user';
                });
            }
        }
    });
});