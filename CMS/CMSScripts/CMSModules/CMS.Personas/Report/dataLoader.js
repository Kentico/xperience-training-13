cmsdefine(['jQuery', 'Underscore', 'CMS/Application'], function ($, _, application) {

    var convertViewModelToKeyValuePair = function(viewModel) {
            var result = {};
            result[viewModel.PersonaID] = viewModel.Contacts;
            return result;
        },


        assign = function (objects) {
            var result = {};

            objects.forEach(function(object) {
                for (var key in object) {
                    if (object.hasOwnProperty(key)) {
                        result[key] = object[key];
                    }
                }
            });

            return result;
        },

        
        prepareData = function(data) {
            var groupedByDate = _.groupBy(data,
                function(i) {
                    return i.Date;
                }),
                result = [];

            Object.keys(groupedByDate).forEach(function(key) {
                var personaContactsPairs = groupedByDate[key].map(convertViewModelToKeyValuePair);
                personaContactsPairs.push({ 'date': key });
                result.push(assign(personaContactsPairs));
            });

            return result;
        },

        
        loadData = function() {
            return $.get(application.getData('applicationUrl') + 'cmsapi/PersonaContactHistory/Get').then(prepareData);
        };

    return {
        loadData: loadData
    };
});