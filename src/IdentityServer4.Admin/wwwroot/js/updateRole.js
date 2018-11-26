$(function () {
    $('#role').addClass('active');
    new Vue({
        el: '#view',
        data: {
            name: '',
            description: ''
        },
        mounted: function () {
            loadView(this);
        },
        methods: {
            update: function () {
                app.put("/api/role/" + app.getPathPart(window.location.href, 1), this.$data, function () {
                    window.location.href = '/role';
                }, function () {
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
        const url = '/api/role/' + app.getPathPart(window.location.href, 1);
        app.get(url, function (result) {
            vue.$data.name = result.data.name;
            vue.$data.description = result.data.description;
        });
    }
});

