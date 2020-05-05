cmsdefine(['CMS/EventHub', 'CMS/NavigationBlocker', 'Underscore', 'jQuery', 'jQueryFancySelector'], function (EventHub, NavigationBlocker, _, $) {

    var Header = function(data) {
        var $selector = $('#' + data.selectorId),
            $window = $(window),
            $mainIframe = $('iframe[name="cmsdesktop"]'),
            $dashboardLink = $('#' + data.dashboardLinkId),
            $opener, that = this;

        // Apply fancy style
        $selector.fancySelect();

        // Show site selector panel after "redesign"
        $('.site-selector').show();

        $opener = $('.fancy-select .dropdown');

        this.$opener = $opener;
        this.$options = $('.fancy-select .dropdown-menu');
        this.$mainIframe = $mainIframe;

        this.navigationBlocker = new NavigationBlocker();

        // Check UI changes on every option change
        this.$options.find('li').click(function(e) {
            // If user clicked on already selected site, do not perform POSTBACK which would cause multiple subscriptions to events and Application Dashboard would not work anymore
            if (_.contains(this.classList, "selected")) {
                that.fancySelectorHide();
                e.preventDefault();
                return false;
            }


            if (!that.navigationBlocker.canNavigate()) {
                that.fancySelectorHide();
                e.preventDefault();
                return false;
            };
        });

        // Attach dashboard link click handler
        $dashboardLink.click(function(e) {
            // Do not handle the middle button or ctrl key
            if (e.which !== 2 && !e.ctrlKey) {
                // Open dashboard manually
                EventHub.publish('DashboardClicked', e);
            }
        });

        $opener.click(function() {
            if (that.$options.hasClass('open')) {
                that.fancySelectorUpdateMaxHeight();
            }
        });

        // Reinitialize scroll on every window.resize
        $window.resize(_.debounce(function() {
            if (that.$options) {
                that.fancySelectorUpdateMaxHeight();
            }
        }, 1000 / 4));

        EventHub.subscribe("GlobalClick", function (e) {
            if (e.target !== $opener[0]) {
                that.fancySelectorHide();
            }
        });
    };


    Header.prototype.fancySelectorUpdateMaxHeight = function() {
        // Update max-height to 80% height of the administration panel
        var anchorMaxHeight = this.$mainIframe.height() * 0.8;

        this.$options.css({
            'max-height': anchorMaxHeight + 'px'
        });
    };


    Header.prototype.fancySelectorHide = function() {
        this.$options.removeClass('open');
        this.$opener.removeClass('open');
    };

    return Header;
});