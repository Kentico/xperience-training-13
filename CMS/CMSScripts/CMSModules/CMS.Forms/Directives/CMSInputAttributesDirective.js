cmsdefine([], function () {
    /**
     * This directive looks for supported attributes in the parent scope. If any of the supported 
     * value is found, it is passed to the context element.
     * If the scope specifies id attribute, its value will be used for the name attribute as well.
     *
     * This directive is valid only for input, textarea or select element.
     *
     * @example
     * ...
     * $scope = {
     *    required: true,
     *    maxlength: 50,
     *    id: "someId"
     * };
     *
     * ...
     *
     * <input type="text" data-ng-model="model" cms-input-attributes="" />
     * 
     * will transform the input to 
     *
     * <input type="text" data-ng-model="model" name="someId" id="someId" required maxlength="50" cms-input-attributes="" />
     *
     * @throws      If the directive is used for not supported element.
     */
    
    // Array of supported HTML attributes containing values
    var supportedAttributes = ["maxlength", "required", "cols", "rows", "id"],
        
    // Array of supported HTML attributes without the value
        supportedProperties = ["required"];

    return [function () {
        return {
            restrict: "A",
            link: function ($scope, $element) {
                var elementTagName = $element.prop("tagName").toLowerCase();
                
                if ((elementTagName !== "input") && (elementTagName !== "textarea") && (elementTagName !== "select")) {
                    throw "This directive can be used only for input, textarea or select element, but was used for the '" + elementTagName + "'.";
                }

                supportedAttributes.forEach(function(attribute) {
                    if ($scope[attribute]) {
                        $element.attr(attribute, $scope[attribute]);
                    }
                });
       
                supportedProperties.forEach(function (property) {
                    if ($scope[property]) {
                        $element.prop(property, true);
                    }
                });
                
                // Name is special attribute, since is usually matches the id, so it will be added even if it is not explicitly specified in the scope
                if ($scope.id) {
                    $element.attr("name", $scope.id);
                }
            }
        };
    }];
});