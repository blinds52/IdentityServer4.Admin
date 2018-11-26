$(function () {
    $('#user').addClass('active');
    new Vue({
        el: '#view',
        data: {
            newPassword: ''
        },
        methods: {
            update: function () {
                app.put("/api/user/" + app.getPathPart(window.location.href, 1) + '/changepassword', this.$data, function () {
                    window.location.href = '/user';
                }, function (result) {
                    swal('更新失败', result.msg, 'error');
                });
            }
        }
    });
});

