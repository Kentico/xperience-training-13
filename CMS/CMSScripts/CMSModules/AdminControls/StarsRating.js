/**
 *  Scripts for stars rating control.
 */
cmsdefine(['CMS/WebFormCaller'], function (caller) {

    var StarsRating = function (serverData) {
        var ratingElement = document.getElementById(serverData.id);
        var readOnly = false;
        var waitingStarCssClass = serverData.waitingStarCssClass;
        var filledStarCssClass = serverData.filledStarCssClass;
        var emptyStarCssClass = serverData.emptyStarCssClass;
        var rtlDirection = serverData.rtlDirection;
        var uniqueID = serverData.uniqueID;
        var ratingValue = serverData.ratingValue;
        var starsStorage;

        function setRequiredClassIcon(item, toIcon, removeExisting) {
            if (removeExisting) {
                item.classList.remove(filledStarCssClass);
                item.classList.remove(emptyStarCssClass);
            }

            item.classList.add(toIcon);
        }

        function highlightSelectedIcons(index, fullIconType, removeExisting) {
            starsStorage.forEach(function (element, i) {
                if (((i <= index) && !rtlDirection) || ((starsStorage.length - i <= starsStorage.length - index) && rtlDirection)) {
                    setRequiredClassIcon(element, fullIconType, removeExisting);
                } else {
                    setRequiredClassIcon(element, emptyStarCssClass, removeExisting);
                }
            });
        }

        function clickEventHandler(value) {
            if (readOnly) {
                return;
            }
            readOnly = true;

            highlightSelectedIcons(value - 1, waitingStarCssClass, false);

            if (rtlDirection) {
                value = starsStorage.length - value + 1;
            }

            caller.doPostback({
                targetControlUniqueId: uniqueID,
                args: value
            });
        }

        function mouseOverEventHandler(index) {
            if (readOnly) {
                return;
            }

            ratingElement.title = index + 1;
            if (rtlDirection) {
                ratingElement.title = starsStorage.length - index;
            }

            highlightSelectedIcons(index, filledStarCssClass, true);
        }

        function mouseOutEventHandler() {
            if (readOnly) {
                return;
            }
            if (ratingValue === 0) {
                starsStorage.forEach(function (element) {
                    setRequiredClassIcon(element, emptyStarCssClass, true);
                });
            } else {
                starsStorage.forEach(function () {
                    highlightSelectedIcons(ratingValue - 1, filledStarCssClass, true);
                });
            }
        }

        function bindEventListeners(item, index) {
            item.addEventListener('click', clickEventHandler.bind(null, index + 1), false);
            item.addEventListener('mouseover', mouseOverEventHandler.bind(null, index), false);
            item.addEventListener('mouseout', mouseOutEventHandler, false);
        }

        if (ratingElement) {
            starsStorage = [];

            for(var i = 0; i < ratingElement.children.length; i++)
            {
                var element = ratingElement.children[i];

                // gather all rating elements
                if(element.tagName === 'SPAN'){
                    starsStorage.push(element);

                    // bind event listeners
                    bindEventListeners(element, i);
                }
            }
        }
    };

    return StarsRating;
});