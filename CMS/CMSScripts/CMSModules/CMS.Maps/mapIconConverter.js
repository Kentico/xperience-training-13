cmsdefine([], function () {

    var svgNS = 'http://www.w3.org/2000/svg',
        homeIconGroupClassName = 'amcharts-pan-home',
        plusIconGroupClassName = 'amcharts-zoom-in',
        minusIconGroupClassName = 'amcharts-zoom-out',


        createNewTextIcon = function () {
            var text = document.createElementNS(svgNS, 'text');
            text.setAttribute('x', '8');
            text.setAttribute('y', '29');
            text.setAttribute('style', 'pointer-events: none;');
            text.setAttribute('class', 'icon- cms-icon-100');

            return text;
        },


        getIconByClassName = function (root, className) {
            return root.getElementsByClassName(className)[0];
        },


        convertIcon = function (map, groupClassName, content) {
            var iconGroup = getIconByClassName(map, groupClassName),
                textIcon = createNewTextIcon();

            if (!iconGroup) {
                return;
            }

            textIcon.textContent = content;

            iconGroup.removeChild(iconGroup.lastChild);
            iconGroup.appendChild(textIcon);
        },


        convertHomeIcon = function (map) {
            convertIcon(map, homeIconGroupClassName, '\ue680');
        },


        convertPlusIcon = function (map) {
            convertIcon(map, plusIconGroupClassName, '\ue655');
        },


        convertMinusIcon = function (map) {
            convertIcon(map, minusIconGroupClassName, '\ue656');
        },


        convertMapIconsToCMSOnes = function (map) {
            if (!map || !map.containerDiv || !(map.containerDiv instanceof HTMLElement)) {
                throw 'Incorrect argument, expected object with "containerDiv" HTMLElement property';
            }

            var div = map.containerDiv;

            convertHomeIcon(div);
            convertPlusIcon(div);
            convertMinusIcon(div);
        };


    return {
        convertMapIconsToCMSOnes: convertMapIconsToCMSOnes
    };
});
