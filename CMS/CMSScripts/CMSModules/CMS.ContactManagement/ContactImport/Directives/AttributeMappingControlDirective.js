cmsdefine([], function () {

    var Controller = function($scope) {
        this.$scope = $scope;
            this.$scope.selectedField = '';
            this.$scope.contactFieldUpdate = this.contactFieldUpdate.bind(this);
        },
        directive = function() {
            return {
                restrict: 'A',
                scope: {
                    contactFieldsMapping: '=',
                    csvColumn: '='
                },
                templateUrl: 'attributeMappingControlDirectiveTemplate.html',
                controller: Controller
            };
        };


    /**
     * Sets mapped index for currently selected field option to match index of the CSV column.
     */
    Controller.prototype.contactFieldUpdate = function() {
        var selected;
         
        this.$scope.contactFieldsMapping.forEach(function (category) {
            category.categoryMembers.forEach(function (item) {
                var selectedField = this.$scope.selectedField;

                if (item.mappedIndex === this.$scope.csvColumn.ColumnIndex) {
                    item.mappedIndex = -1;
                }

                if (selectedField && (item.name === selectedField)) {
                    selected = item;
                }
            }, this);
        }, this);

        if (selected) {
            selected.mappedIndex = this.$scope.csvColumn.ColumnIndex;
        }
    };


    Controller.$inject = [
      '$scope'
    ];

    return [directive];
});