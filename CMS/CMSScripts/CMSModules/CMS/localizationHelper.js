cmsdefine([], function() {

    var getLocalizedPercents = function (percents, language) {
        return (percents / 100).toLocaleString(language, {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2,
            style: 'percent'
        });
    };


    return {
        getLocalizedPercents: getLocalizedPercents
    };
});

