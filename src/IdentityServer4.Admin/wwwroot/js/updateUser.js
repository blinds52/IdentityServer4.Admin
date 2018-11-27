$(function () {
    $('#user').addClass('active');
    new Vue({
        el: '#view',
        data: {
            userName: '',
            email: '',
            phoneNumber: ''
        },
        mounted: function () {
            loadView(this);
        },
        methods: {
            update: function () {
                app.put("/api/user/" + app.getPathPart(window.location.href, 1), this.$data, function () {
                    window.location.href = '/user';
                }, function (result) {
                    swal({
                        title: "更新失败",
                        type: "error",
                        text: result.msg,
                        showCancelButton: true
                    });
                });
            }
        }
    });

    function loadView(vue) {
        const url = '/api/user/' + app.getPathPart(window.location.href, 1);
        app.get(url, function (result) {
            vue.$data.userName = result.data.userName;
            vue.$data.email = result.data.email;
            vue.$data.phoneNumber = result.data.phoneNumber;
        });
    }
});

