$(function () {
    $('#permission').addClass('active');
    new Vue({
        el: '#view',
        data: {
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