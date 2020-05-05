/**
 * GridActions is a helper constructor object for working with actions within a unigrid server control.
 * It attaches click event on entire grid (instead of on every action) and then calls methods
 * for a given actions. See constructor comment for informations about module configuration.
 *
 * Currently it reacts only to left-click actions, but it can easily be expanded to react to other events.
 */
cmsdefine(['jQuery', 'Underscore'], function ($, _) {

    var defaultOptions = {
            actionButtonSelector: '.js-unigrid-action'
        },  


        /**
         * Helper constructor to work with unigrid action
         * @param {Object} options Options to configure this object. It can contain following properties:
         *     @param {String} gridSelector jQuery selector that matches the grid wrapper element.
         *     @param {String} actionButtonSelector jQuery selector that matches every action button.
         *     @param {Object} actionButtonActions Hashtable, where keys are jQuery selectors that matches given actions
         *                     and values are corresponding functions that will be invoked on action click.
         * See https://kentico.atlassian.net/wiki/x/PwCSAw for more details.
         */
        GridActions = function (options) {
            _.extend(options, defaultOptions);

            var $grid = $(options.gridSelector);

            // Attach click handler to entire grid instead of attaching it to every
            // action within it. It is much better for performance reasons.
            $grid.on('click', function (e) {
                var $target = $(e.target),
                    targetData;

                // Check if the click was done on action, otherwise
                // tre to find the closest action upwards in the DOM.
                if (!$target.is(options.actionButtonSelector)) {
                    $target = $target.closest(options.actionButtonSelector);
                }

                if ($target.length === 0 || $target.is(':disabled')) {
                    return true;
                }

                // Get target data. This will be sent to action callback.
                targetData = $target.data();

                // Iterate through every action and call correct action callback.
                _.each(options.actionButtonsActions, function (action, actionSelector) {
                    if ($target.is(actionSelector)) {
                        action(targetData);
                    }
                });

                return false;
            });
        };

    return GridActions;
});