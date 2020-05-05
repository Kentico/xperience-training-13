cmsdefine(['CMS/Application', 'jQuery', 'Underscore'], function (app, $, _) {

    var SmartTip = function (data, onSmartTipReady) {

        $(function() {
            var that = this,
                baseUrl = app.getData('applicationUrl') + 'cmsapi/SmartTip';

            function sendRequest() {
                $.ajax({
                    type: 'POST',
                    url: baseUrl,
                    data: JSON.stringify(data.identifier),
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json'
                });
            }

            function runToggles($first, $second) {
                $first.slideUp(250).promise().done(function () {
                    $second.delay(500).slideDown(250).fadeIn(500);
                });
            }

            function createTemplate(data) {
                // Backup original underscore template settings
                var originalTemplateSettings = _.templateSettings;

                // Replace template settings so all angular and underscore templates have the same interpolation syntax
                _.templateSettings = {
                    interpolate: /\{\{(.+?)\}\}/g
                };

                var template = _.template(data);

                // Restore original underscore template settings
                _.templateSettings = originalTemplateSettings;

                return template;
            }

            // Get HTML template of Smart Tip
            $.get(app.getData('applicationUrl') + 'CMSScripts/CMSModules/CMS/Templates/SmartTipTemplate.html', function (template) {
                var smartTipTemplate = createTemplate(template);

                that.$smartTipPlc = $(data.selector);
                that.$smartTipPlc.append(smartTipTemplate({
                    content: data.content,
                    expandedHeader: data.expandedHeader,
                    collapsedHeader: data.collapsedHeader,
                    collapsed: data.isCollapsed,
                    resources: data.resources
                }));

                var $expanded = that.$smartTipPlc.find('.js-st-expanded');
                var $collapsed = that.$smartTipPlc.find('.js-st-collapsed');

                that.$smartTipPlc.find('.js-st-expand').click(function () {
                    runToggles($collapsed, $expanded);
                    sendRequest();
                });

                that.$smartTipPlc.find('.js-st-collapse').click(function () {
                    runToggles($expanded, $collapsed);
                    sendRequest();
                });

                if (typeof onSmartTipReady === 'function') {
                    onSmartTipReady();
                }
            });
        });
        
    };
    

    return SmartTip;

});