cmsdefine([], function() {

    var before = function(object, method, fn) {
            var temp = object[method];
            object[method] = function () {
                fn.apply(object);
                temp.apply(object, arguments);
            };
        },


        after = function(object, method, fn) {
            var temp = object[method];
            object[method] = function () {
                temp.apply(object, arguments);
                fn.apply(object);

            };
        };


    return {
        after: after,
        before: before
    }
});
