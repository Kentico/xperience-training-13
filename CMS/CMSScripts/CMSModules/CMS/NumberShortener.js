/** 
 * NumberShortener util function
 */
cmsdefine(function () {

    return {
        shortenNumber: function (num, decimalFraction, labelsNames) {
            var fraction = decimalFraction || 1,
                labels = labelsNames || { thousand: 'k', million: 'm', billion: 'g' };


            if (num >= 1000000000) {
                return (num / 1000000000).toFixed(fraction).replace(/\.0$/, '') + labels.billion;
            }
            if (num >= 1000000) {
                return (num / 1000000).toFixed(fraction).replace(/\.0$/, '') + labels.million;
            }
            if (num >= 1000) {
                return (num / 1000).toFixed(fraction).replace(/\.0$/, '') + labels.thousand;
            }
            return num;
        }
    };
});