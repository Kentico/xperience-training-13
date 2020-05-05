cmsdefine(['Underscore'], function (_) {

    var svgNS = 'http://www.w3.org/2000/svg',
        minLabelClassName = 'amcharts-value-legend-min-label',
        maxLabelClassName = 'amcharts-value-legend-max-label',

        getTextElementByClassName = function (root, className) {
            return root.getElementsByClassName(className)[0];
        },


        getAreaValues = function (map) {
            var values = _.pluck(map.dataProvider.areas, 'value');

            return _.filter(values, function (value) {
                return !!value;
            });
        },


        checkArgument = function (map) {
            if (!map || !map.dataProvider || !map.dataProvider.areas) {
                throw 'Incorrect argument, "map.dataProvider.areas" has to be specified';
            }
            if (!map.containerDiv || !(map.containerDiv instanceof HTMLElement)) {
                throw 'Incorrect argument, expected object with "containerDiv" HTMLElement property';
            }
        },


        setMinLabelValue = function (map, language) {
            var minValue = _.min(getAreaValues(map));
            setLabel(map, minLabelClassName, minValue.toLocaleString(language));
        },


        setMaxLabelValue = function (map, language) {
            var maxValue = _.max(getAreaValues(map));
            setLabel(map, maxLabelClassName, maxValue.toLocaleString(language));
        },


        setLabel = function (map, labelClassName, content) {
            var label = getTextElementByClassName(map.containerDiv, labelClassName);

            if (!label) {
                return;
            }

            label.firstChild.textContent = content;

        },


        setLabels = function (map, language) {
            checkArgument(map);

            setMinLabelValue(map, language);
            setMaxLabelValue(map, language);
        };


    return {
        setLabels: setLabels
    };
});