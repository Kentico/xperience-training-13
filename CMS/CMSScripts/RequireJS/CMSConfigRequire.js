/**
 * Configures RequireJS library
 * @param {baseUrl} url used by RequireJS to load modules.
 * @param {pathToCmsScripts} path to CMSScripts folder in CMSApp.
 */
CMSConfigRequire = function (baseUrl, pathToCmsScripts) {
    cmsrequire.config({
        baseUrl: baseUrl,

        paths: {
            'jQuery': pathToCmsScripts + 'jQuery/jquery-core',
            'jQueryUI': pathToCmsScripts + 'Vendor/jQueryUI/jquery-ui-1.10.4.min',
            'Underscore': pathToCmsScripts + 'Underscore/underscore.min',
            'jQueryJScrollPane': pathToCmsScripts + 'jquery-jscrollpane',
            'jQueryFancySelector': pathToCmsScripts + 'jquery/jquery-fancyselect',
            'jQueryDatePicker': pathToCmsScripts + 'jquery/jquery-ui-datetimepicker',
            'select2': pathToCmsScripts + 'Select2/select2',

            'angular': pathToCmsScripts + 'Vendor/Angular-1.5.5/angular.min',
            'angular-resource': pathToCmsScripts + 'Vendor/Angular-1.5.5/angular-resource.min',
            'angular-sanitize': pathToCmsScripts + 'Vendor/Angular-1.5.5/angular-sanitize.min',
            'angular-route': pathToCmsScripts + 'Vendor/Angular-1.5.5/angular-route.min',
            'angular-animate': pathToCmsScripts + 'Vendor/Angular-1.5.5/angular-animate.min',
            'angular-ellipsis': pathToCmsScripts + 'Vendor/Angular/angular-ellipsis.min',
            'angularSortable': pathToCmsScripts + 'Vendor/Angular/sortable',
            'uiBootstrap': pathToCmsScripts + 'Bootstrap/ui-bootstrap-custom-tpls-1.3.3',

            'lodash': pathToCmsScripts + 'Vendor/LoDash/lodash',
            'csv-parser': pathToCmsScripts + 'Vendor/CSV-JS/csv',
            'sortable': pathToCmsScripts + 'Vendor/SortableJS/sortable.min',

            'amcharts': pathToCmsScripts + 'CMSModules/CMS.Charts/amCharts/amcharts',
            'amcharts.cmstheme': pathToCmsScripts + 'CMSModules/CMS.Charts/Themes/CMSTheme',
            'amcharts.funnel': pathToCmsScripts + 'CMSModules/CMS.Charts/amCharts/funnel',
            'amcharts.gauge': pathToCmsScripts + 'CMSModules/CMS.Charts/amCharts/gauge',
            'amcharts.pie': pathToCmsScripts + 'CMSModules/CMS.Charts/amCharts/pie',
            'amcharts.radar': pathToCmsScripts + 'CMSModules/CMS.Charts/amCharts/radar',
            'amcharts.serial': pathToCmsScripts + 'CMSModules/CMS.Charts/amCharts/serial',
            'amcharts.xy': pathToCmsScripts + 'CMSModules/CMS.Charts/amCharts/xy',
            'amcharts.gantt': pathToCmsScripts + 'CMSModules/CMS.Charts/amCharts/gantt',

            'amcharts.ammap' : pathToCmsScripts + 'CMSModules/CMS.Maps/ammap/ammap_amcharts_extension'

        },
        shim: {
            'jQuery': { exports: '$cmsj' },
            'jQueryUI': { deps: ['jQuery'] },
            'jQueryDatePicker': { deps: ['jQueryUI'] },
            'Underscore': { exports: '_' },
            'jQueryJScrollPane': { deps: ['jQuery'] },
            'jQueryFancySelector': { deps: ['jQuery'] },
            'select2': { deps: ['jQuery'] },

            'angular': { exports: 'angular', deps: ['jQuery'] },
            'angular-resource': { deps: ['angular'] },
            'angular-sanitize': { deps: ['angular'] },
            'angular-route': { deps: ['angular'] },
            'angular-animate': { deps: ['angular'] },
            'lodash': { exports: '_' },
            'angular-ellipsis': { deps: ['angular'] },
            'angularSortable': { deps: ['jQuery', 'jQueryUI', 'angular'] },
            'csv-parser': { exports: 'CSV' },
            'sortable': { exports: 'Sortable' },

            'uiBootstrap': { deps: ['angular', 'angular-animate'] },

            'amcharts': {
                exports: 'AmCharts',
                init: function () {
                    AmCharts.isReady = true;
                }
            },
            'amcharts.cmstheme': {
                deps: ['amcharts']
            },
            'amcharts.funnel': {
                deps: ['amcharts'],
                exports: 'AmCharts'
            },
            'amcharts.gauge': {
                deps: ['amcharts'],
                exports: 'AmCharts'
            },
            'amcharts.pie': {
                deps: ['amcharts'],
                exports: 'AmCharts'
            },
            'amcharts.radar': {
                deps: ['amcharts'],
                exports: 'AmCharts'
            },
            'amcharts.serial': {
                deps: ['amcharts'],
                exports: 'AmCharts'
            },
            'amcharts.xy': {
                deps: ['amcharts'],
                exports: 'AmCharts'
            },
            'amcharts.gantt': {
                deps: ['amcharts', 'amcharts.serial'],
                exports: 'AmCharts'
            },
            'amcharts.ammap': {
                deps: ['amcharts'],
                exports: 'AmCharts'
            }
        },

        priority: [
            'jQuery',
            'jQueryUI',
            'angular',
            'angular-route',
            'angular-animate',
            'angularSortable'
        ]
    });
};