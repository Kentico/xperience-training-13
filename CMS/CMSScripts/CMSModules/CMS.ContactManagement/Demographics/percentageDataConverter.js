cmsdefine(['Underscore'], function (_) {

    var getSum = function (data, valueField) {
            return _.reduce(data, function (memo, item) {
                return memo + item[valueField];
            }, 0);
        },

        convertDataToPercents = function (data, valueField) {
            if(!data)
            {
                throw 'Data has to be specified';
            }

            if(!valueField)
            {
                throw 'Value field has to be specified';
            }

            var sum = getSum(data, valueField);

            data.forEach(function (item) {
                item.percentage = 100 * item[valueField] / sum;
            });

            return data;
        };


    return {
        convertDataToPercents: convertDataToPercents
    };
});

