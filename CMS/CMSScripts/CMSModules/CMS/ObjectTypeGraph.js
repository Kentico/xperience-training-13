cmsdefine(['CMS/Application', 'jQuery'], function (app, $) {

    var Module = function (data) {

        var config = data,
        // Network options - see: http://visjs.org/docs/network/
        options = {
            physics: {
                enabled: true,
                hierarchicalRepulsion: {
                    centralGravity: 0.0,
                    springLength: 400,
                    springConstant: 0.01,
                    nodeDistance: 175,
                    damping: 1
                }
            },
            nodes: {
                borderWidth: 0,
                borderWidthSelected: 0,
                font: {
                    face: 'Segoe UI, Helvetica, Verdana, Arial, sans-serif'
                },
                shape: 'box',
                shapeProperties: {
                    borderRadius: 0
                },
                labelHighlightBold: false
            },
            edges: {
                smooth: {
                    enabled: false,
                    type: 'dynamic',
                    roundness: 0.5
                },
                selectionWidth: 0,
                hoverWidth: 0
            },
            layout: {
                hierarchical: {
                    enabled: true,
                    blockShifting: true,
                    edgeMinimization: true,
                    levelSeparation: 140,
                    parentCentralization: true,
                    direction: 'UD',        // UD, DU, LR, RL
                    sortMethod: 'hubsize'   // hubsize, directed
                }
            },
            interaction: {
                dragView: true,
                dragNodes: false,
                zoomView: true,
                hover: true,
                hoverConnectedEdges: false
            },
            groups: {
                current: {
                    color: {
                        background: '#497d04',
                        highlight: {
                            background: '#497d04'
                        },
                        hover: {
                            background: '#497d04'
                        }
                    },
                    font: {
                        color: '#ffffff',
                        size: 18
                    }
                },
                parentchild: {
                    color: {
                        background: '#1175ae',
                        highlight: {
                            background: '#1175ae'
                        },
                        hover: {
                            background: '#1175ae'
                        }
                    },
                    font: {
                        color: '#ffffff'
                    }
                },
                childbinding: {
                    color: {
                        background: '#a3a2a2',
                        highlight: {
                            background: '#a3a2a2'
                        },
                        hover: {
                            background: '#a3a2a2'
                        }
                    },
                    font: {
                        color: '#262524'
                    }
                },
                otherbinding: {
                    color: {
                        background: '#d6d9d6',
                        highlight: {
                            background: '#d6d9d6'
                        },
                        hover: {
                            background: '#d6d9d6'
                        }
                    },
                    font: {
                        color: '#262524'
                    }
                },
                bindingbranch: {
                    shape: 'text',
                    title: null
                }
            }
        },
        nodesArray = [],
        nodes = new vis.DataSet(nodesArray),
        edgesArray = [],
        edges = new vis.DataSet(edgesArray),
        graphData = {
            nodes: nodes,
            edges: edges
        },
        container = document.getElementById(config.networkId), // Do not convert to jQuery, vis.Network requires the raw DOM object and cannot work with the jQuery wrapper
        network = new vis.Network(container, graphData, options),
        filterOptions = $('#' + config.filterId + ' input[type="checkbox"]'),

        setNetworkData = function (graphData) {
            network.setData(graphData);
        },

        loadData = function (objectType, scope) {
            $.getJSON(app.getData('applicationUrl') + 'cmsapi/ObjectTypeGraph?objectType=' + objectType + '&scope=' + scope, setNetworkData);
        },

        linkToObjectType = function (params) {
            if (params.nodes[0]) {
                var targetObjectType = params.nodes[0].split('_')[0];
                if (targetObjectType.indexOf('|branch') == -1) {
                    $(location).attr('href', $(location).attr('pathname') + '?objectType=' + targetObjectType);
                }
            }
        },

        getScope = function () {
            var scope = 0;
            for (var i = 0; i < filterOptions.length; i++) {
                if (filterOptions[i].checked) {
                    scope += parseInt(filterOptions[i].value, 10);
                }
            }

            return scope;
        },

        reLoadFilteredData = function () {
            loadData(config.objectType, getScope());
        },

        setPointerCursor = function () {
            container.style.cursor = 'pointer';
        },

        resetCursor = function () {
            container.removeAttribute('style');
        },

        init = function () {
            for (var i = 0; i < filterOptions.length; i++) {
                filterOptions[i].onclick = reLoadFilteredData;
            }

            loadData(config.objectType, 15);
            network.on('click', linkToObjectType);
            network.on('hoverNode', setPointerCursor);
            network.on('blurNode', resetCursor);
        };

        init();
    };

    return Module;
});