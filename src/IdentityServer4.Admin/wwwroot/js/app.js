const app = {};
app.getUrlParam = function (name) {
    let result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
    if (result === null || result.length < 1) {
        return "";
    }
    return result[1];
};
app.getPathPart = function (url, part) {
    url = url.replace(/^http:\/\/[^/]+/, "");
    let parts = url.split('/');
    let result = '';
    if (!part) {
        result = parts[parts.length - 1];
    } else {
        part = part + 1;
        if (part < parts.length) {

            result = parts[part];
        }
    }
    return decodeURI(result);
};
app.successHandler = function (result, success, error) {
    if (result && result.code === 200) {
        if (success) {
            success(result);
        }
    } else {
        if (error) {
            error(result);
        } else {
            if (swal) {
                if (result.msg) {
                    swal('错误', result.msg, "error");
                } else {
                    swal('错误', '未知错误', "error");
                }
            }
        }
    }
};
app.errorHandler = function (result, error) {
    if (error) {
        error(result);
    } else {
        if (swal) {
            if (result.statusText) {
                swal('错误', result.statusText, "error");
            } else {
                swal('错误', '未知错误', "error");
            }
        }
    }
};
app.post = function (url, data, success, error) {
    $.ajax({
        url: url,
        data: JSON.stringify(data),
        headers: {
            RequestVerificationToken: $('input[name$="__RequestVerificationToken"]').val()
        },
        method: 'POST',
        dataType: 'json',
        contentType: 'application/json',
        success: function (result) {
            app.successHandler(result, success, error);
        },
        error: function (result) {
            app.errorHandler(result, error);
        }
    });
};
app.get = function (url, success, error) {
    $.ajax({
        url: url,
        method: 'GET',
        success: function (result) {
            app.successHandler(result, success, error);
        },
        error: function (result) {
            app.errorHandler(result, error);
        }
    });
};
app.delete = function (url, success, error) {
    $.ajax({
        url: url,
        method: 'DELETE',
        success: function (result) {
            app.successHandler(result, success, error);
        },
        error: function (result) {
            app.errorHandler(result, error);
        }
    });
};
app.put = function (url, data, success, error) {
    $.ajax({
        url: url,
        data: JSON.stringify(data),
        method: 'PUT',
        dataType: 'json',
        contentType: 'application/json',
        success: function (result) {
            app.successHandler(result, success, error);
        },
        error: function (result) {
            app.errorHandler(result, error);
        }
    });
};

app.head = function (url, success, error) {
    $.ajax({
        url: url,
        method: 'HEAD',
        success: function (result) {
            if (success) success();
        },
        error: function (result) {
            if (error) error();
        }
    });
};
app.ui = {};
app.pagers = {};
app.ui.initPagination = function (query, option, click) {
    let total = option.total || 1;
    let size = option.size || 10;
    let page = option.page || 1;
    let totalPages = parseInt((total / size), 10) + ((total % size) > 0 ? 1 : 0) || 1;
    let currOption = {
        startPage: page,
        totalPages: totalPages,
        visiblePages: 10,
        first: "First",
        prev: "Previous",
        next: "Next",
        last: "Last",
        onPageClick: function (event, page) {
            if (!app.pagers[query]) {
                app.pagers[query] = true;
                return;
            }
            click(page);
        }
    };

    if (app.pagers.hasOwnProperty(query)) {
        $(query).twbsPagination("destroy");
    }
    app.pagers[query] = false;
    $(query).twbsPagination(currOption);
};
app.formatDate = function (time, format = 'YY-MM-DD hh:mm:ss') {
    let date = new Date(time);

    let year = date.getFullYear(),
        month = date.getMonth() + 1,//月份是从0开始的
        day = date.getDate(),
        hour = date.getHours(),
        min = date.getMinutes(),
        sec = date.getSeconds();
    let preArr = Array.apply(null, Array(10)).map(function (elem, index) {
        return '0' + index;
    });

    return format.replace(/YY/g, year)
        .replace(/MM/g, preArr[month] || month)
        .replace(/DD/g, preArr[day] || day)
        .replace(/hh/g, preArr[hour] || hour)
        .replace(/mm/g, preArr[min] || min)
        .replace(/ss/g, preArr[sec] || sec);
};
$(function () {
    $('#btnLogout').click(function () {
        $('#logoutForm').submit();
    });
});
app.rangeCheck = function (v, min, max) {
    if (v) {
        return v >= min && v <= max;
    }
    return false;
};
app.minCheck = function (v, min) {
    return app.rangeCheck(v, min, 65535);
};
app.maxCheck = function (v, max) {
    return app.rangeCheck(v, -65535, max);
};
app.requireCheck = function (v) {
    return !!v;
};
app.mobileCheck = function (v) {
    let reg = /^1[3|4|5|7|8][0-9]{9}$/;
    return reg.test(v);
};
app.phoneCheck = function (v) {
    let reg = /([0-9]{3,4}-)?[0-9]{7,8}/;
    return reg.test(v);
};
app.equalCheck = function (v1, v2) {
    return v1 === v2;
};
app.emailCheck = function (mail) {
    let reg = /^([a-za-z0-9]+[_|_|.]?)*[a-za-z0-9]+@([a-za-z0-9]+[_|_|.]?)*[a-za-z0-9]+.[a-za-z]{2,3}$/;
    return reg.test(mail);
};
app.ui.setBusy = function () {
    $("#loading").css("display", "");
};
app.ui.clearBusy = function () {
    $("#loading").css("display", "none");
};