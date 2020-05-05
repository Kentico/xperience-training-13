
function setUpSelector(clientID, postBack, noDataFound, initSet) {
    // Textbox control (holds autocomplete widget)
    var tb = $cmsj("#" + clientID + "_txtAutocomplete");

    if (tb.length > 0) {
        // Storage of selected value
        var hdnValue = $cmsj("#" + clientID + "_hdnValue");
        // Dropdown button next to the textbox
        var btnDropDown = $cmsj("#" + clientID + "_btnAutocomplete");
        // Indicates whether widget was opened when drop down button was clicked.
        var wasOpen = false;
        // Index of selected item
        var selectedIndex = 0;
        // Set true after user input is made (click, keydown)
        var initialized = false;

        var menuItem = tb.autocomplete({
            source:
             function (request, response) {
                 var dt = [];

                 // Prevent autorun (IE 11 issue)
                 if (initialized) {

                     // Store search term           
                     tb.data('searchTerm', request.term);

                     // Get results array
                     var is = initSet;

                     // Filter manually
                     var lowered = $cmsj.trim(request.term.toLowerCase());
                     is = $cmsj(is).filter(function (index) {
                         return (this.Text.toLowerCase().indexOf(lowered) != -1);
                     });

                     // Create result array
                     if ((is.length != 0) || request.term == '') {
                         // Map results set to single objects
                         dt = $cmsj.map(is, function (item) {
                             return {
                                 Text: item.Text,
                                 Value: item.Value,
                                 Hash: (item.Hash === undefined) ? '' : item.Hash,
                                 Style: (item.style === undefined) ? '' : item.style,
                                 Disabled: (item.disabled == 'disabled') ? 'disabled' : '',
                                 Class: (item.class === undefined) ? '' : item.class
                             }
                         });
                     } else {
                         // Display no data found item
                         dt.push({
                             Text: noDataFound,
                             Value: '##NODATAFOUND##',
                         });
                     }
                 }

                 // Send response
                 response(dt);
             },
            select: function (event, ui) {
                // Trigger on before change (but changed data already)
                tb.trigger('onBeforeChange', ui.item.Value);
                // Inner setting
                event.preventDefault();

                switch (ui.item.Value) {
                    case '##NODATAFOUND##':
                        // No data found field, do nothing (this field is not clickable anyway)
                        break;

                    default:
                        // Store original data, used in restoring data in false type search (focusout event)
                        var encoded = $cmsj('<div />').html(ui.item.Text).text();
                        tb.data('originalData', encoded);
                        // Store selected value to hidden field
                        hdnValue.val(ui.item.Value);
                        // Store decoded new display name to textbox
                        $cmsj(this).val(encoded);
                        // Trigger after change event
                        tb.trigger('onAfterChange', ui.item.Value);
                        // Trigger postback if set
                        if (postBack) {
                            __doPostBack(clientID, 'selectionchanged');
                        }
                        break;
                }
            },
            open: function (event, ui) {
                // Set field width based on textbox width
                var autocomplete = $cmsj(this).autocomplete("widget");
                var w = tb.width();
                // Check for min width - 300 px (border) + padding textbox
                var width = w + btnDropDown.width() + 14;
                width = Math.max(250, width);

                // Set new width
                autocomplete.css("width", width + "px");

                // Scroll to selected item position
                $cmsj(this).autocomplete("widget").scrollTop(selectedIndex * 21);
            },
            change: function (event, ui) {
                tb.val(tb.data('originalData'));
            }
        }).data("ui-autocomplete");

        // Autocomplete drop down widget
        var widget = tb.autocomplete("widget");

        menuItem._renderMenu = (function (ul, items) {
            ul.addClass('dropDownAutocompleteWidget');

            // Set maximal height to menu
            var textboxWrapper = tb.parent();
            var maxHeight = window.innerHeight - (textboxWrapper.offset().top + textboxWrapper.height()) - 10;
            maxHeight = maxHeight < 200 ? maxHeight : 200;
            ul.css('max-height', maxHeight + 'px');

            var that = this;
            $cmsj.each(items, function (index, item) {
                that._renderItemData(ul, item);

                if (hdnValue.val() == item.Value) {
                    selectedIndex = index;
                }
            });
        });

        menuItem._renderItem = (function (ul, item) {
            var searchTerm = tb.data('searchTerm');

            var style = (item.Style != '') ? ' style="' + item.Style + '"' : '';
            var itemClass = (item.Class != '') ? ' class="' + item.Class + '"' : '';
            var attrs = style + itemClass;

            // No data found not clickable
            if (item.Value == "##NODATAFOUND##") {
                return $cmsj("<li>")
                    .append("<span class=\"AutocompleteSelectorNoDataFound\">" + item.Text + "</span>")
                    .appendTo(ul);
            }

            if (item.Disabled == "disabled") {
                return $cmsj("<li" + attrs + ">")
                    .append("<span>" + item.Text + "</span>")
                    .appendTo(ul);
            }

            // Display regular field, except more items field
            if ((searchTerm !== undefined) && (searchTerm != '')) {
                var startIndex = item.Text.toLowerCase().indexOf(searchTerm.toLowerCase());
                // If field contains searched text
                if (startIndex != -1) {
                    // Before serached item
                    var start = item.Text.substring(0, startIndex);
                    // Searched item
                    var properSearch = item.Text.substring(startIndex, startIndex + searchTerm.length);
                    // Applay search class
                    var search = "<span class=\"autocompleteSelectorSearchPart\">" + properSearch + "</span>";
                    // After search text
                    var end = item.Text.substring(searchTerm.length + startIndex);

                    return $cmsj("<li" + attrs + ">")
                        .append("<a>" + start + search + end + "</a>")
                        .appendTo(ul);
                }
            }

            return $cmsj("<li " + attrs + ">")
                        .append("<a>" + item.Text + "</a>")
                        .appendTo(ul);
        })

        btnDropDown.click(function () {
            initialized = true;
            if (tb.is(':enabled')) {
                if (wasOpen) {
                    tb.autocomplete('close');
                } else {
                    // Set focus and store default text value
                    menuItem._search('');
                    tb.focus();
                    tb.select();
                }
            }

            return false;
        });

        btnDropDown.mousedown(function () {
            wasOpen = widget.is(":visible");
        })

        tb.click(function (event) {
            initialized = true;

            // If search is not pending, start searching  
            if (!widget.is(":visible")) {
                menuItem._search('');
            }
            tb.select();
        });

        tb.keydown(function () {
            initialized = true;
        });

        tb.focus(function (event) {
            // Store text when user clicks textbox
            tb.data('originalData', tb.val());
        });

        $cmsj(document).bind('mousewheel DOMMouseScroll', function (e) {
            if (!widget.is(':hover')) {
                tb.autocomplete('close');
            }
        });

        $cmsj(widget).bind('mousewheel DOMMouseScroll', function (e) {
            var scrollTo = null;

            if (e.type == 'mousewheel') {
                scrollTo = (e.originalEvent.wheelDelta * -1);
            }
            else if (e.type == 'DOMMouseScroll') {
                scrollTo = 40 * e.originalEvent.detail;
            }

            if (scrollTo) {
                e.preventDefault();
                $cmsj(this).scrollTop(scrollTo + $cmsj(this).scrollTop());
            }
        });
    }
}