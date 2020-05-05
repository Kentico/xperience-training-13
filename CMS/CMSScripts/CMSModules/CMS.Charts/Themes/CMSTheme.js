cmsdefine(['amcharts'], function(chart) {
    chart.themes.CMSTheme = {
        themeName: "CMSTheme",

        AmChart: {
            fontSize: 13,
            color: "#262524",
            backgroundColor: "#FFFFFF",
            fontFamily: "'Segoe UI', Helvetica, Verdana, Arial, sans-serif"
        },

        AmCoordinateChart: {
            colors: ["#0f7abc", "#f69c04", "#518f02", "#c0282a", "#e6c32a", "#008e98"]
        },

        AmStockChart: {
            colors: ["#0f7abc", "#f69c04", "#518f02", "#c0282a", "#e6c32a", "#008e98"]
        },

        AmSlicedChart: {
            colors: ["#0f7abc", "#f69c04", "#518f02", "#c0282a", "#e6c32a", "#008e98"],
            outlineAlpha: 1,
            outlineThickness: 2,
            labelTickColor: "#000000",
            labelTickAlpha: 0.3
        },

        AmRectangularChart: {
            zoomOutButtonColor: '#000000',
            zoomOutButtonRollOverAlpha: 0.15,
            zoomOutButtonImage: "lens.png"
        },

        AxisBase: {
            axisColor: "#000000",
            axisAlpha: 0.3,
            gridAlpha: 0.1,
            gridColor: "#000000"
        },

        ChartScrollbar: {
            color: "#000000",
            backgroundColor: "#000000",
            backgroundAlpha: 0.12,
            graphFillAlpha: 1,
            graphLineAlpha: 0,
            selectedBackgroundColor: "#FFFFFF",
            selectedBackgroundAlpha: 0.4,
            gridAlpha: 0.15,
            scrollbarHeight: 40,
            autoGridCount: true
        },

        ChartCursor: {
            cursorColor: "#0f7abc",
            color: "#FFFFFF",
            cursorAlpha: 0.3
        },

        AmLegend: {
            color: "#000000"
        },

        AmGraph: {
            lineAlpha: 0.9
        },
        GaugeArrow: {
            color: "#000000",
            alpha: 0.8,
            nailAlpha: 0,
            innerRadius: "40%",
            nailRadius: 15,
            startWidth: 15,
            borderAlpha: 0.8,
            nailBorderAlpha: 0
        },

        GaugeAxis: {
            tickColor: "#000000",
            tickAlpha: 1,
            tickLength: 15,
            minorTickLength: 8,
            axisThickness: 3,
            axisColor: '#000000',
            axisAlpha: 1,
            bandAlpha: 0.8
        },

        TrendLine: {
            lineColor: "#c03246",
            lineAlpha: 0.8
        },

        // ammap
        AreasSettings: {
            unlistedAreasColor: '#f7f7f7',
            selectedColor: '#403e3d',
            rollOverOutlineColor: '#bdbbbb',
            rollOverColor: '#403e3d',
            outlineColor: '#bdbbbb',
            color:'#d0e8ed',
            colorSolid:'#1175ae',
            outlineThickness: 1
        }
    };
});