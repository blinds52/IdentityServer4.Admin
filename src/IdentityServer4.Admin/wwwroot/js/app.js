var app = {};
app.queryString = function (name) {
    var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
    if (result === null || result.length < 1) {
        return "";
    }
    return result[1];
};

app.post = function (url, data, success, error) {
    $.ajax({
        url: url,
        data: JSON.stringify(data),
        method: 'POST',
        dataType: 'json',
        contentType: 'application/json',
        success: function (result) {
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
                            swal(result.msg, '', "error");
                        }
                    }
                }
            }
        },
        error: function (result) {
            if (error) {
                error(result);
            } else {
                if (swal) {
                    swal(result.msg, '', "error");
                }
            }
        }
    });
};

app.get = function (url, success, error) {
    $.ajax({
        url: url,
        method: 'GET',
        success: function (result) {
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
                            swal(result.msg, '', "error");
                        }
                    }
                }
            }
        },
        error: function (result) {
            if (error) {
                error(result);
            } else {
                if (swal) {
                    swal(result.msg, '', "error");
                }
            }
        }
    });
};

app.delete = function (url, success, error) {
    $.ajax({
        url: url,
        method: 'DELETE',
        success: function (result) {
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
                            swal(result.msg, '', "error");
                        }
                    }
                }
            }
        },
        error: function (result) {
            if (error) {
                error(result);
            } else {
                if (swal) {
                    swal(result.msg, '', "error");
                }
            }
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
                            swal(result.msg, '', "error");
                        }
                    }
                }
            }
        },
        error: function (result) {
            if (error) {
                error(result);
            } else {
                if (swal) {
                    swal(result.msg, '', "error");
                }
            }
        }
    });
};

app.ui = {};

app.ui.setBusy = function () {
    $("#loading").css("display", "");
};
app.ui.clearBusy = function () {
    $("#loading").css("display", "none");
};

app.pagers = {};
app.ui.initPagination = function (query, option, click) {
    var total = option.total || 1;
    var size = option.size || 10;
    var page = option.page || 1;
    var totalPages = parseInt((total / size), 10) + ((total % size) > 0 ? 1 : 0) || 1;
    var currOption = {
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

app.getFilter = function (key) {
    var filter = app.queryString('filter');
    if (!filter) {
        return '';
    }
    var kvs = filter.split('|');
    var filters = {};
    for (i = 0; i < kvs.length; ++i) {
        var kv = kvs[i].split('::');
        filters[kv[0]] = kv[1];
    }
    return filters[key];
};
app.formatDate = function (time, format = 'YY-MM-DD hh:mm:ss') {
    var date = new Date(time);

    var year = date.getFullYear(),
        month = date.getMonth() + 1,//月份是从0开始的
        day = date.getDate(),
        hour = date.getHours(),
        min = date.getMinutes(),
        sec = date.getSeconds();
    var preArr = Array.apply(null, Array(10)).map(function (elem, index) {
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
    $(".dropdown-trigger").dropdown();
});
 
