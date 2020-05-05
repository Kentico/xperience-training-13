/*
    Scripts for anchor dropup quick navigation.     
    Handles smooth scrolling to specified anchor, toggling category and dropup scrolling using jScrollPane.
*/

cmsdefine(['jQuery', 'jQueryJScrollPane'], function ($) {

    var AnchorDropup = function(data) {
        this.scrollOffset = 8;
        this.initQuickNavigation(data);
    };    

    // Get padding of wrapper div
    AnchorDropup.prototype.getDropupPadding = function() {
        return parseInt($('.anchor-dropup .dropdown-menu').css('padding-top'), 10);
    };

    // Scrolls to target anchor with offset
    AnchorDropup.prototype.smoothScroll = function(elem) {

        var target = $(elem.hash);
        var offset;

        // Toggle category
        if (target.parent().find('.ToggleImage').next().val() === 'True') {
            window.ToggleDiv(target);
        } else if (target.parent().next('td').find('.ToggleImage').next().val() === 'True') {
            window.ToggleCategory(target.parents('.EditingFormCategoryRow.Toggle'));
        }

        // Select target anchor
        target = target.length && target || $('[name=' + elem.hash.slice(1) + ']');

        if (target.length) {
            // Scroll to target with offset
            offset = target.offset().top - this.getHeaderOffset() - this.scrollOffset;
            $('html, body').stop(true, false).animate({ scrollTop: offset }, 300);
            return false;
        }
    };

    // Returns offset of header element 
    AnchorDropup.prototype.getHeaderOffset = function() {
        var headerOffset = (window.cmsHeader) ? window.cmsHeader.offsetTop : 0;
        var headerOffsetHeight = (window.cmsHeader) ? window.cmsHeader.offsetHeight : 0;

        // Return only cms-edit-menu height (header actions)
        if ((headerOffset === 0) && (headerOffsetHeight === 0)) {
            return $('.cms-edit-menu').outerHeight();
        }
        return headerOffset + headerOffsetHeight;
    };

    // Init anchor dropup wrapper using jScrollPane
    AnchorDropup.prototype.initScrollableDropup = function() {
        window.scrollTo(0, 0);
        // Get maximal height of dropup list
        var anchorMaxHeight = $('.anchor-dropup').offset().top - this.getDropupPadding() * 2 - this.getHeaderOffset() - this.scrollOffset;
        // Backup wrapper padding
        var anchorPadding = this.getDropupPadding();
        var elem = $('.anchor-dropup .dropdown-menu');

        // Set maximal height
        elem.css({
            'max-height': anchorMaxHeight + 'px'
        });
        
        elem.css({ 'padding': 0 });

        // Apply jScrollPane
        elem.jScrollPane({
            verticalGutter: 0,
            hideFocus: true
        });
        // Restore padding
        elem.css({ 'padding': anchorPadding + 'px' });

        // Fix HTML validity
        $('.jspContainer').wrap('<li></li>');
        $('.jspPane').wrapInner('<ul class="dropdown-menu dropdown-menu-inner-wrap"></ul>');
    };

    // Inits dropup functionality
    AnchorDropup.prototype.initQuickNavigation = function(offset) {

        var that = this;
        this.scrollOffset += offset;
        // Scroll to target anchor 
        $('.anchor-dropup a[href^="#"]').each(function () {
            $(this).unbind('click', function () {
                that.smoothScroll(this);
            }).click(function() {
                that.smoothScroll(this);
            });
        });

        if ($('.jspContainer').length === 0) {
            // Init jScrollPane
            this.initScrollableDropup();
        }        
    };

    return AnchorDropup;
});