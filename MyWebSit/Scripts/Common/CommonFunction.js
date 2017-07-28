/**
 * CommonFunction公共函数库
 * 基础插件库1：jQuery
 * 2016年04月14日 董阳
 */

(function ($) {
    if (!$.isPlainObject($.CommonFunction)) {
        /* 字符串对象扩展方法 */
        String.prototype.startsWith = function (prefix) {
            return this.slice(0, prefix.length) === prefix;
        };
        String.prototype.endsWith = function (suffix) {
            return this.indexOf(suffix, this.length - suffix.length) !== -1;
        };

        $CommonFunction = {
            /**
             * function：生成Guid
             * @return string
             */
            createGuid: function () {
                return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
                    var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
                    return v.toString(16).toUpperCase();
                });
            },

            /**
             * function：生成默认Guid
             * @return string
             */
            createDefaultGuid: function () {
                return "00000000-0000-0000-0000-000000000000";
            },

            /**
             * function：去除前后空白字符（包括全角与半角）
             * @param str 原字符串
             * @return string
             */
            trimSpaceChar: function (str) {
                return str.replace(/(^\s*)|(\s*$)/g, "").replace(/(^　*)|(　*$)/g, "");
            },

            /**
             * function：解码实体对象中的所有属性
             * @param object
             * @returns object
             */
            decodeObject: function (object) {
                for (var key in object) {
                    if ($.type(object[key]) == "string") {
                        object[key] = decodeURIComponent(object[key]);
                    } else if ($.isPlainObject(object[key])) {
                        object[key] = $CommonFunction.decodeObject(object[key]);
                    } else if ($.isArray(object[key])) {
                        object[key] = $CommonFunction.decodeObjectList(object[key]);
                    }
                }
                return object;
            },

            /**
             * function：解码对象数组
             * @param list
             * @returns list
             */
            decodeObjectList: function (list) {
                if (!$.isArray(list)) {
                    return [];
                }
                for (var i = 0; i < list.length; i++) {
                    if ($.type(list[i]) == "string") {
                        list[i] = decodeURIComponent(list[i]);
                    } else {
                        list[i] = $CommonFunction.decodeObject(list[i]);
                    }
                }
                return list;
            }
        };
    }
})(jQuery);