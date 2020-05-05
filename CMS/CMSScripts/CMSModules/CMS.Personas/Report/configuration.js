-cmsdefine([], function () {

    var getPersonaConfiguration = function(config) {
        var result = {};

        config.personas.forEach(function(persona) {
            result[persona.personaID] = persona;
        });

        return result;
    };

    return {
        getPersonaConfiguration: getPersonaConfiguration
    };
});