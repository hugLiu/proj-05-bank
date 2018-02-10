String.prototype.trim = function () {
    //用正则表达式将前后空格用空字符串替代。   
    return this.replace(/(^\s*)|(\s*$)/g, "");
}
//Array.forEach implementation for IE support..  
//https://developer.mozilla.org/en/JavaScript/Reference/Global_Objects/Array/forEach  
if (!Array.prototype.forEach) {
    Array.prototype.forEach = function (callback, thisArg) {
        var T, k;
        if (this == null) {
            throw new TypeError(" this is null or not defined");
        }
        var O = Object(this);
        var len = O.length >>> 0; // Hack to convert O.length to a UInt32  
        if ({}.toString.call(callback) != "[object Function]") {
            throw new TypeError(callback + " is not a function");
        }
        if (thisArg) {
            T = thisArg;
        }
        k = 0;
        while (k < len) {
            var kValue;
            if (k in O) {
                kValue = O[k];
                callback.call(T, kValue, k, O);
            }
            k++;
        }
    };
}

//对数组的扩展，返回数组每个元素处理后返回值的新数组,空值不加
Array.prototype.cast = function (func) {
    var r = [];
    this.forEach(function (i) {
        var r1 = func(i);
        if (r1 != null) r.push(r1);
    });
    return r;
}; //这个分号不能少

//为jQuery对象也提供如上的扩展
(function ($) {
    $.fn.cast = function (func) {
        var r = [];
        this.each(function (i) {
            var r1 = func(this, i);
            if (r1 != null) r.push(r1);
        });
        return r;
    }
})(jQuery);

var _submitButtons;
function getSubmitButtons() {
    _submitButtons = _submitButtons || $('body').find('.frame-submit');
}

//以下代码将按钮和noSubmit,beforeSubmit,afterSubmit三个事件绑定。
jQuery.extend({
    /*注册按钮事件 
     options的格式 ： 
    {  
        id : '按钮ID',
        text:'按钮文本', 
        //以上两者选其一，以ID优先

        noSubmit:不执行提交操作的方法,
        beforeSubmit:表单数据验证前方法,
        beforeAjax:向服务器发请求之前方法, 
        afterSubmit:提交后接收返回数据的方法
      }
    */
    regButton: function (options) {
        getSubmitButtons();
        function getSubmitButtonByText(text) {
            var btn;
            _submitButtons.each(function () {
                if ($(this).text().trim() == text) {
                    btn = this;
                    return false;
                }
            });
            return btn;
        }

        function getSubmitButtonById(id) {
            var btn;
            _submitButtons.each(function () {
                if (this.id == id) {
                    btn = this;
                    return false;
                }
            });
            return btn;
        }

        var btn = options.id ? getSubmitButtonById(options.id) : getSubmitButtonByText(options.text);
        if (btn) {
            btn.noSubmit = options.noSubmit;
            btn.beforeSubmit = options.beforeSubmit;
            btn.beforeAjax = options.beforeAjax;
            btn.afterSubmit = options.afterSubmit;
        }
        else {
            //todo:增加此按钮
        }
    }
});

//为了兼容mini-ui和jquery的api而作的通用按钮对象
function getUButton(btn) {
    var b;
    if (mini) {
        var b = mini.get(btn);
        if (b) return b;
        b = $(btn);
    }
    b.enable = function () { b.removeAttr('disabled') };
    b.disable = function () { b.attr('disabled', 'disabled') };
    b.setValue = function (val) { b.val(val) };
    b.getValue = function (val) { b.val() };
    return b;
}

//为了兼容mini-ui和jquery的api而作的通用表单对象
function getUForm(form) {
    if (mini) {
        return new mini.Form(form[0]);
    }
    f.validate = function () { };
    f.isValid = function () { return true; };
    return f;
}

$(function () {
    if (typeof (_userMenus) == 'undefined') {
        _userMenus = parent._userMenus;
    }

    if (!_userMenus) {
        _userMenus = $.getSync(_userMenusUrl);
    }

    function getFunctionInfo(id) {
        for (var i in _userMenus.UserMenu) {
            if (_userMenus.UserMenu[i].Id == id) {
                return _userMenus.UserMenu[i];
            }
        }
        return null;
    }

    function getFunctionTagIds() {
        return _userMenus.UserMenu.cast(function () {
            var el = document.getElementById(this.Id);
            if (el != null) {
                return _userMenus.UserMenu[i].id;
            }
        });
    }

    $('form').each(function () {
        var form = $(this);
        // alert(this.attr('id'));
        getSubmitButtons();
        if (_submitButtons.length == 0) return;

        var sv;

        _submitButtons.click(function (e) {
            e.preventDefault();
            sv = ($(this).text() || "").trim();

            //获取提交按钮对象 by_zjf
            var miniButton = getUButton(this);
            //添加判断如果按钮禁用状态,不做提交操作 by_zjf
            //if (!miniButton.enabled) {
            if (miniButton.enabled == false) {
                return false;
            }

            if (this.noSubmit) {
                this.noSubmit(sv);
                return;
            }

            form[sv] = this;
            if (this.id) {
                var func = getFunctionInfo(this.id);
                if (func) {
                    form.attr("action", func.Location);
                    form.attr("method", func.Method == 0 ? "GET" : "POST");
                }
            }
            form.submit();
        });

        //处理表单的回车事件，如果有一个按钮标识为class=frame-default,则默认接受回车提交
        var defaultBtn = form.find('.frame-default');

        form.keydown(function (e) {
            if (e.which == 13) {
                if (defaultBtn.length > 0) {
                    form.find('.frame-default').click();
                }
            }
        });

        form.submit(
            function (e) {
                e.preventDefault();
                //if (!sv) return;
                var miniButton = getUButton(form[sv]);

                function afterSubmitFunc(r) {
                    if (r.Type) { //只要在后端返回确切的JsonTips类型数据时，才自动显示提示
                        showTips(r);
                    }
                    miniButton.enable();
                    if (r.Type == "error") {
                        return;
                    }
                    r.text = ajaxObj.text;

                    form.trigger("afterSubmit", r);

                    if (form[sv].afterSubmit) {
                        form[sv].afterSubmit(r);
                    }

                    if (r.ReturnValue && r.ReturnValue.Url) {
                        try {
                            var link = '<a id="__returnUrl" href="' + r.ReturnValue.Url + '" target="' + (r.ReturnValue.Target || '_self') + '" style="display:none">go</a>';
                            form.append(link);
                            $('#__returnUrl')[0].click();
                        }
                        catch (e) {
                            location.href = r.ReturnValue.Url;
                        }
                    }
                }
                var ajaxObj = {
                    text: sv,
                    url: form.attr("action"),
                    type: form.attr("method"),
                    data: "auto-serialize",
                    validate: true,
                    cancel: false, //增加cancel字段用于在beforeSubmit时有机会撤消提交
                    success: afterSubmitFunc,
                    error: function (xhr) {
                        //升级到jquery 1.9之后，responseText返回空值也认为是错误。
                        if (xhr.status == "200") {
                            afterSubmitFunc(xhr.responseText);
                        }
                        else {
                            showTips({ Type: "error", Title: form.attr("method") + "请求出错!", Details: xhr.responseText });
                            miniButton.enable();
                        }
                    }
                };

                form.trigger("beforeSubmit", ajaxObj);
                if (ajaxObj.cancel) return;

                if (form[sv].beforeSubmit && (form[sv].beforeSubmit(ajaxObj) == false)) {
                    ajaxObj.cancel = true;
                }

                if (ajaxObj.cancel) return;

                if (ajaxObj.validate) {
                    var miniForm = getUForm(form);
                    miniForm.validate();
                    //if (miniForm.isValid() == false) return;
                }

                if (!ajaxObj.cancel) {
                    form.trigger("beforeAjax", ajaxObj);
                    if (ajaxObj.cancel) return;

                    if (form[sv].beforeAjax && (form[sv].beforeAjax(ajaxObj) == false)) {
                        ajaxObj.cancel = true;
                    }
                    if (ajaxObj.cancel) return;

                    if (ajaxObj.data == "auto-serialize") {
                        ajaxObj.data = form.serialize();
                    }

                    miniButton.disable();
                    $.ajax(ajaxObj);
                }
            });
    });

    //自动调整所有使用'frame-fit'样式的元素高度为到底部
    window.onresize = function () {
        //获取元素距离顶部的绝对高度
        var fixDiv = $('.frame-fit');
        if (fixDiv.length == 0) return;
        fixDiv.each(function () {
            var y = $(this).offset().top;
            //浏览器时下窗口文档body的总高度 包括border padding -绝对高度
            var height = $(document.body).outerHeight(true) - y;
            $(this).css("height", height);
        });
    };

    //点击带frame-clicktoggle标记的元素以外其他地方消失
    $(document).click(function () {
        $('.frame-clicktoggle').hide();
    })

    //点击带frame-clicktoggle标记的元素本身不消失
    $('.frame-clicktoggle').on("click", function (e) {
        e.stopPropagation();
    });

    //点击带frame-clicktoggle标记的ID-toggle的元素则切换目标的显示/隐藏
    $('.frame-clicktoggle').each(function () {
        var that = this;
        $('#' + $(this).attr("id") + "-toggle").click(function (e) {
            e.stopPropagation();
            $(that).toggle();
        });
    });

    window.onresize();

    //搜索框的回车处理
    $('.frame-search').keydown(function (e) {
        if (e.which == 13) {
            e.preventDefault();
            $('.frame-search .frame-submit').click();
        }
    });

    //搜索框的事件处理
    $.regButton({
        text: '搜索',
        noSubmit: function () {
            //获取搜索框文本
            var key = $('.frame-search :text').val();

            //找到class='frame-search-grid'或' .frame-search-tree' 的mini-grid或mini-tree
            $('.frame-search-grid, .frame-search-tree').each(function () {
                var gridId = this.id;
                var grid = mini.get("#" + gridId);
                grid.load({ key: key });
            });
        }
    });
});

_time_out = 0;
function showTips(tips, title, message) {
    if (top.showTips && this != top) {
        return top.showTips(tips, title, message);
    }
    function showDetails() {
        mini.open({
            url: bootPATH + "blank.html",
            title: "错误详情",
            onload: function () {
                var iframe = this.getIFrameEl();
                iframe.contentWindow.document.write(tips.Details);
            },
        });
    }

    if (!tips) return;
    if (title || message) {
        tips = {
            Type: tips,
            Title: title,
            Message: message
        }
    }
    switch (tips.Type) {
        case "error":
            toastr["error"](tips.Message, tips.Title || "错误");
            var tipsDiv = $('.toast-error');
            if (tips.Details) {
                tipsDiv.append("点击查看详情...");
                tipsDiv.click(showDetails);
            }
            break;
        case "warning":
            toastr["warning"](tips.Message, tips.Title || "警告");
            break;
        case "success":
            toastr["success"](tips.Message, tips.Title);
            break;
        default:
            toastr["info"](tips.Message, tips.Title);
            break;
    }
}

function setCookie(name, value, days) {
    if (!days) {
        document.cookie = name + "=" + escape(value);
        return;
    }
    var exp = new Date();
    exp.setTime(exp.getTime() + days * 24 * 60 * 60 * 1000);
    document.cookie = name + "=" + escape(value) + ";expires=" + exp.toGMTString();
}

function getCookie(name) {
    var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");
    if (arr = document.cookie.match(reg)) return unescape(arr[2]);
    else return null;
}

function delCookie(name) {//为了删除指定名称的cookie，可以将其过期时间设定为一个过去的时间
    var date = new Date();
    date.setTime(date.getTime() - 10000);
    document.cookie = name + "=; expires=" + date.toGMTString();
}

jQuery.extend({
    //$.getJSON方法有缓存，但用POST不符合http规范。所以额外定义此扩展
    //用于无缓存获取json数据,并统一处理错误信息
    newGET: function (url, data, callback, dataType) {
        if (typeof (data) == 'function') {
            callback = data;
            data = null;
        }
        $.ajax({
            url: url,
            data: data,
            success: function (data) {
                if (data.Type) {
                    showTips(data);
                }
                if (callback) callback(data);
            },
            dataType: dataType || "json",
            error: function (xhr) {
                if (xhr.status == "0") {
                    //ajax在完成之前请求已经被取消（ajax请求没有发出），
                    //由于，例如：页面已经跳转或跳转太快、浏览器输入新的url、按钮立即新的点击等（确定）
                }
                //升级到jquery 1.9之后，responseText返回空值也认为是错误。
                else if (xhr.status == "200") {
                    r = xhr.responseText;
                    if (callback) callback(r);
                }
                else {
                    showTips({ Type: "error", Title: url + ":'newGET()'请求出错!", Details: xhr.responseText });
                }
            },
            cache: false
        });
    },

    newPOST: function (url, data, callback, dataType) {
        if (typeof (data) == 'function') {
            callback = data;
            data = null;
        }
        $.ajax({
            type: 'POST',
            url: url,
            data: data,
            success: function (data) {
                if (data.Type) {
                    showTips(data);
                }
                if (callback) callback(data);
            },
            dataType: dataType || "json",
            error: function (xhr) {
                if (xhr.status == "0") {
                    //ajax在完成之前请求已经被取消（ajax请求没有发出），
                    //由于，例如：页面已经跳转或跳转太快、浏览器输入新的url、按钮立即新的点击等（确定）
                }
                //升级到jquery 1.9之后，responseText返回空值也认为是错误。
                else if (xhr.status == "200") {
                    r = xhr.responseText;
                    if (callback) callback(r);
                }
                else {
                    showTips({ Type: "error", Title: url + ":'newPOST()'请求出错!", Details: xhr.responseText });
                }
            },
            cache: false
        });
    },

    //发起一个同步请求,直接返回结果
    getSync: function (url, data, dataType) {
        var r;
        $.ajax({
            url: url,
            async: false,
            data: data,
            success: function (re) {
                r = re;
            },
            error: function (xhr) {
                if (xhr.status == "0") {
                    //ajax在完成之前请求已经被取消（ajax请求没有发出），
                    //由于，例如：页面已经跳转或跳转太快、浏览器输入新的url、按钮立即新的点击等（确定）
                }
                //升级到jquery 1.9之后，responseText返回空值也认为是错误。
                else if (xhr.status == "200") {
                    r = xhr.responseText;
                }
                else {
                    showTips({
                        Type: "error", Title: "'getSync()'请求'" + url + "'时出错!", Details: xhr.responseText
                    });
                }
            },
            dataType: dataType || "json"
        });
        return r;
    },

    //发起一个POST同步请求,直接返回结果
    postSync: function (url, data, dataType) {
        var r;
        $.ajax({
            type: 'POST',
            url: url,
            async: false,
            data: data,
            success: function (re) { r = re; },
            error: function (xhr) {
                if (xhr.status == "0") {
                    //ajax在完成之前请求已经被取消（ajax请求没有发出），
                    //由于，例如：页面已经跳转或跳转太快、浏览器输入新的url、按钮立即新的点击等（确定）
                }
                //升级到jquery 1.9之后，responseText返回空值也认为是错误。
                else if (xhr.status == "200") {
                    r = xhr.responseText;
                }
                else {
                    showTips({
                        Type: "error", Title: "'postSync()'请求'" + url + "'时出错!", Details: xhr.responseText
                    });
                }
            },
            dataType: dataType || "json"
        });
        return r;
    },

    //js的多语言API，可以在后台动态添加多语言的Key
    lang: function (key /*,p1,p2...*/) {
        JStr = JStr || {};
        var val = JStr[key];
        if (!val) {
            setTimeout(function () { $.newPOST(bootPATH + "../AppCenter/Res/AddKey", { key: key }); }, 2000);
            val = key;
        }
        //process{0},{1}.. replace with p1, p2... 
        for (var i = 1; i < arguments.length; i++) {
            val = val.replace('{' + (i-1) + '}', arguments[i]);
        }
        return val;
    }
});

/*
对URL的操作的类，基于 http://www.cnblogs.com/loogn/archive/2012/04/25/2469501.html
但基于jquery的全局函数
使用示例
var myurl= $.urlParser("http://www.baidu.com?a=1");
myurl.set("b","hello"); //添加了b=hello
alert (myurl.url());
myurl.remove("b"); //删除了b
alert(myurl.get ("a"));//取参数a的值，这里得到1
myurl.set("a",23); //修改a的值为23
alert (myurl.url());
*/
(function ($) {
    var objURL = function (url) {
        this.ourl = url || window.location.href;
        this.href = "";//?前面部分
        this.params = {};//url参数对象
        this.jing = "";//#及后面部分
        this.init();
    }

    //分析url,得到?前面存入this.href,参数解析为this.params对象，#号及后面存入this.jing
    objURL.prototype.init = function () {
        var str = this.ourl;
        var index = str.indexOf("#");
        if (index > 0) {
            this.jing = str.substr(index);
            str = str.substring(0, index);
        }
        index = str.indexOf("?");
        if (index > 0) {
            this.href = str.substring(0, index);
            str = str.substr(index + 1);
            var parts = str.split("&");
            for (var i = 0; i < parts.length; i++) {
                var kv = parts[i].split("=");
                this.params[kv[0]] = kv[1];
            }
        }
        else {
            this.href = this.ourl;
            this.params = {};
        }
    }
    //只是修改this.params
    objURL.prototype.set = function (key, val) {
        this.params[key] = val;
    }
    //只是设置this.params
    objURL.prototype.remove = function (key) {
        this.params[key] = undefined;
    }
    //根据三部分组成操作后的url
    objURL.prototype.url = function () {
        var strurl = this.href;
        var objps = [];//这里用数组组织,再做join操作
        for (var k in this.params) {
            if (this.params[k]) {
                objps.push(k + "=" + encodeURIComponent(this.params[k]));
            }
        }
        if (objps.length > 0) {
            strurl += "?" + objps.join("&");
        }
        if (this.jing.length > 0) {
            strurl += this.jing;
        }
        return strurl;
    }
    //得到参数值
    objURL.prototype.get = function (key) {
        return decodeURIComponent(this.params[key] || '');
    }
    $.urlParser = function (url) { return new objURL(url); };
})(jQuery);

var lastUrl = null;
function goUrl(url) {
    if (top.iframe) {
        lastUrl = top.iframe.contentWindow.location.href;
        top.iframe.contentWindow.location.href = url;
    }
    else {
        setCookie("_lastUrl" + location.port, url);
        location.href = url;
    }
}

function goBack() {
    if (top.iframe) {
        top.iframe.contentWindow.location.href = lastUrl;
    }
    else {
        location.href = getCookie("_lastUrl" + location.port);
    }
}