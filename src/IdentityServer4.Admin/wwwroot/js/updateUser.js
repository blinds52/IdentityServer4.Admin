$(function () {
    $('#user').addClass('active');
    new Vue({
        el: '#view',
        data: {
            userName: '',
            email: '',
            password: '',
            phone: ''
        },
        mounted: function () {
            loadView(this);
        },
        methods: {
            update: function () {
                app.put("/api/user/" + app.getResourceName(), null, function () {
                    swal({
                        title: "更新失败",
                        type: "error",
                        showCancelButton: true
                    });
                });
            }
        }
    });

    function loadView(vue) {
        const url = '/api/user/' + app.getResourceName();
        app.get(url, function (result) {
            vue.$data.userName = result.data.userName;
            vue.$data.email = result.data.email;
            vue.$data.password = result.data.password;
            vue.$data.phone = result.data.phone;
        });
    }
});

