//Allows using console.log with no error for any browser
if (!window.console) {
    (function () {
        var names = ["log", "debug", "info", "warn", "error",
            "assert", "dir", "dirxml", "group", "groupEnd", "time",
            "timeEnd", "count", "trace", "profile", "profileEnd"],
            i, l = names.length;

        window.console = {};

        for (i = 0; i < l; i++) {
            window.console[names[i]] = function () { };
        }
    }());
}

/*
Convert string value to JSON object.
Check first if string to convert is not already a JSON object then try to convert using jQuery parseJSON.
*/
function toJson(value) {
    var jobj;
    if ($.isPlainObject(value))
        jobj = value;
    else
        jobj = $.parseJSON(value);

    return jobj;
}