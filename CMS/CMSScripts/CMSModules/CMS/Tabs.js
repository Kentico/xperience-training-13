/**
 * Module for tabs.
 */
cmsdefine([], function () {

    var tabNavigationLinks, tabContentContainers;
    var activeIndex = null;
    var initCalled = false;

    /**
     * Handles click event listeners on each of the links in the tab navigation.
     *
     * @param {HTMLElement} link The link to listen for events on
     * @param {Number} index The index of that link
     */
    function handleClick(link, index) {
        link.addEventListener('click', function (e) {
            e.preventDefault();
            goToTab(index);
        });
    };

    /**
     * Goes to a specific tab based on index.
     *
     * @param {Number} index The index of the tab to go to
     */
    function goToTab(index) {
        if (index !== activeIndex && index >= 0 && index <= tabNavigationLinks.length) {

            if (activeIndex != null)
            {
                tabContentContainers[activeIndex].classList.remove('active');
                tabNavigationLinks[activeIndex].classList.remove('active');
            }

            tabNavigationLinks[index].classList.add('active');
            tabContentContainers[index].classList.add('active');

            activeIndex = index;
        }
    };

    /**
     * Gets the selected tab index.
     *
     * @returns {Number} Tab index
     */
    function getSelectedTabIndex() {
        return activeIndex;
    }

    /**
     * Initializes the component by attaching event listener
     *  to each tab item which handles tab switching.
     *
     * @param {Object} options The options hash
     */
    function init(options) {
        if (!initCalled) {
            var el = document.querySelector(options.el);
            tabNavigationLinks = el.querySelectorAll(options.tabNavigationLinks);
            tabContentContainers = el.querySelectorAll(options.tabContentContainers);
            initCalled = true;
      
            for (var i = 0; i < tabNavigationLinks.length; i++) {
                var link = tabNavigationLinks[i];
                handleClick(link, i);
            }

            // Go to preselected tab or go to first one
            if (options.selectedTabIndex != null) {
                goToTab(options.selectedTabIndex);
                return;
            }
            goToTab(0);
        }
    };

    return {
        init: init,
        getSelectedTabIndex: getSelectedTabIndex
    };
});