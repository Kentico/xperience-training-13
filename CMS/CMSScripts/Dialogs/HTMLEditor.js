function InsertSelectedItem(obj) {
    if (obj) {
        if (window.wopener != null) {
            if (window.wopener.CKEDITOR != null) {
                window.wopener.CMSPlugin.insert(obj);
            } else if (window.wopener.insert != null) {
                window.wopener.insert(obj);
            }
        }
    }
}

function GetSelectedItem() {
    var obj = {};
    if ((window.wopener != null) && ((window.wopener.currentEditor != null) || (window.wopener.CKEDITOR != null))) {
        CMSPlugin = window.wopener.CMSPlugin;
        CKEDITOR = window.wopener.CKEDITOR;
        var editorInst = (window.wopener.currentEditor || CMSPlugin.currentEditor);
        var selection = CMSPlugin.selection, originalParent, originalElem = selElem = null;
        if (selection) {
            if (CMSPlugin.selectedElement) {
                originalElem = selElem = CMSPlugin.selectedElement;
            }
            else {
                originalElem = selElem = selection.getSelectedElement();
            }
        }

        // If element is selected
        if (selElem) {
            if (selElem.hasAttribute && selElem.hasAttribute('cms_inline')) {
                originalParent = selElem.$.parentNode;
                selElem = editorInst.restoreRealElement(selElem);
            }
            if (selElem.$) {
                selElem = selElem.$;
            }

            var elemName = selElem.tagName.toLowerCase();
            switch (elemName) {
                case 'object':
                case 'cke:object':
                    obj = CMSPlugin.getInline(selElem, originalElem);
                    obj = GetLinkObj(obj, selElem, editorInst, originalParent);
                    break;

                case 'img':
                    obj = GetImageObj(selElem, editorInst);
                    break;

                case 'a':
                    obj = GetLinkObj(obj, selElem, editorInst);
                    break;
            }
        } else if (selection) {
            selElem = selection.getStartElement();
            if (selElem) {
                var linkElement = GetLinkElement(selElem.$);
                if (linkElement) {
                    obj = GetLinkObj(obj, selElem.$, editorInst);
                }
            }
            // If obj not found (is still empty), try GetRanges
            if ($cmsj.isEmptyObject(obj)) {
                selElem = selection.getRanges(true)[0];
                if (selElem != null) {
                    var d = selElem.startOffset - selElem.endOffset;
                    if (Math.abs(d) > 0) {
                        obj.link_text = "##LINKTEXT##";
                        obj.anchor_linktext = "##LINKTEXT##";
                        obj.email_linktext = "##LINKTEXT##";
                    }
                }
            }
        }
    }
    // Escape all values
    for (var i in obj) {
        obj[i] = encodeURIComponent(obj[i]);
    }

    return obj;
}

function GetAnchorNames(editor) {
    var aAnchors = [];

    if ((window.wopener != null) && ((window.wopener.currentEditor != null) || (window.wopener.CKEDITOR != null))) {
        var editor = (window.wopener.currentEditor || window.wopener.CMSPlugin.currentEditor),
        i,
        elements = editor.document.getElementsByTag('img'),
        realAnchors = window.wopener.CMSPlugin.getNodeList(editor.document.$.anchors),
        anchors = [];

        for (var i = 0; i < elements.count() ; i++) {
            var item = elements.getItem(i);
            if (item.data('cke-realelement') && item.data('cke-real-element-type') == 'anchor')
                anchors.push(editor.restoreRealElement(item));
        }

        for (i = 0; i < realAnchors.count() ; i++)
            anchors.push(realAnchors.getItem(i));

        for (i = 0; i < anchors.length; i++) {
            item = anchors[i].getAttribute('name');
            if (item && item.length > 0) {
                aAnchors.push(item);
            }
        }
    }

    return aAnchors;
}

function GetIds() {
    var aIds = [];

    if ((window.wopener != null) && ((window.wopener.currentEditor != null) || (window.wopener.CKEDITOR != null))) {
        var editor = (window.wopener.currentEditor || window.wopener.CMSPlugin.currentEditor);

        // Define a recursive function that search for the Ids.
        var fGetIds = function (parent) {
            for (var i = 0; i < parent.childNodes.length; i++) {
                var sId = parent.childNodes[i].id;

                // Check if the Id is defined for the element.
                if (sId && sId.length > 0)
                    aIds[aIds.length] = sId;

                // Recursive call.
                fGetIds(parent.childNodes[i]);
            }
        }; // Start the recursive calls.
        fGetIds(editor.document.$.body);
    }

    return aIds;
}

function GetImageObj(selElem, editor) {
    if (selElem.tagName.toLowerCase() == "img") {
        obj = new Object();
        obj.img_url = selElem.getAttribute('data-cke-saved-src');
        if (obj.img_url == null) {
            obj.img_url = selElem.src;
        }
        obj.img_alt = selElem.alt;
        obj.img_width = (parseInt(selElem.style.width, 10) > 0 ? parseInt(selElem.style.width, 10) : parseInt(selElem.width, 10));
        obj.img_height = (parseInt(selElem.style.height, 10) > 0 ? parseInt(selElem.style.height, 10) : parseInt(selElem.height, 10));
        obj.img_borderwidth = (parseInt(selElem.style.borderWidth, 10) > 0 ? parseInt(selElem.style.borderWidth, 10) : parseInt(selElem.border, 10));
        obj.img_bordercolor = GetColor(selElem.style.borderColor);
        obj.img_align = (selElem.align ? selElem.align : (selElem.style.cssFloat ? selElem.style.cssFloat : (selElem.style.styleFloat ? selElem.style.styleFloat : (selElem.style['vartical-align'] ? selElem.style['vartical-align'] : selElem.style.verticalAlign))));
        obj.img_hspace = (parseInt(selElem.hspace, 10) > 0 ? selElem.hspace : selElem.style.marginLeft);
        obj.img_vspace = (parseInt(selElem.vspace, 10) > 0 ? selElem.vspace : selElem.style.marginTop);
        // Advanced tab
        obj.img_id = selElem.id;
        obj.img_tooltip = selElem.title;
        obj.img_class = selElem.className;
        // Skip border if parsing failed (for named colors)
        var skipBorder = false;
        if ((selElem.style.borderColor != null) && (selElem.style.borderWidth != '') && (obj.img_bordercolor == '')) {
            obj.img_borderwidth = -1;
            skipBorder = true;
        }
        // Skip borders if there is different border for sides
        var borderWidthMatch = true;
        if ((selElem.style.borderTopWidth != selElem.style.borderRightWidth) || (selElem.style.borderRightWidth != selElem.style.borderBottomWidth) || (selElem.style.borderBottomWidth != selElem.style.borderLeftWidth)) {
            borderWidthMatch = false;
        }
        var borderStyleMatch = true;
        if ((selElem.style.borderTopStyle != selElem.style.borderRightStyle) || (selElem.style.borderRightStyle != selElem.style.borderBottomStyle) || (selElem.style.borderBottomStyle != selElem.style.borderLeftStyle)) {
            borderStyleMatch = false;
        }
        var borderColorMatch = true;
        if ((selElem.style.borderTopColor != selElem.style.borderRightColor) || (selElem.style.borderRightColor != selElem.style.borderBottomColor) || (selElem.style.borderBottomColor != selElem.style.borderLeftColor)) {
            borderColorMatch = false;
        }
        if (!borderWidthMatch || !borderStyleMatch || !borderColorMatch) {
            skipBorder = true;
        }
        // Skip margins if there is different margin for sides
        var skipMargin = false;
        if ((selElem.style.marginLeft != selElem.style.marginRight) || (selElem.style.marginTop != selElem.style.marginBottom)) {
            skipMargin = true;
        }
        var s = selElem.style.cssText.toLowerCase().split(';');
        var currentStyle = null;
        var outStyle = '';
        for (var i = 0; i < s.length; i++) {
            currentStyle = s[i].replace(/^\s+|\s+$/g, '');
            if (currentStyle != '') {
                if ((currentStyle.indexOf('border') >= 0)) {
                    if (skipBorder) {
                        outStyle += currentStyle + ';';
                        // Reset border if margins are diferent
                        obj.img_borderwidth = -1;
                        obj.img_bordercolor = -1;
                    }
                } else if ((currentStyle.indexOf('margin') >= 0)) {
                    if (skipMargin) {
                        outStyle += currentStyle + ';';
                        // Reset H. Space and V. Space if margins are diferent
                        obj.img_hspace = -1;
                        obj.img_vspace = -1;
                    }
                } else if ((currentStyle.indexOf('width') == -1) &&
                (currentStyle.indexOf('height') == -1) &&
                (currentStyle.indexOf('float') == -1) &&
                (currentStyle.indexOf('vertical-align') == -1) &&
                (currentStyle.indexOf('solid:') == -1)) {
                    outStyle += currentStyle + ';';
                }
            }
        }
        obj.img_style = outStyle;
        // Get link from selected Elem
        obj = GetLinkObj(obj, selElem, editor);
        // Additional parameters
        obj.img_dir = selElem.dir;
        obj.img_usemap = selElem.useMap;
        obj.img_longdescription = selElem.longDesc;
        obj.img_lang = selElem.lang;

        return obj;
    }
}

function GetLinkObj(obj, selElem, editor, originalParent) {
    if (selElem != null) {
        var link = GetLinkElement(selElem, originalParent);
        if (link != null) {
            if (obj == null) {
                obj = new Object();
            }
            var url = link.getAttribute('data-cke-saved-href');
            if (url == null) {
                url = link.href;
            }

            var isEmail = /mailto:(.*)/i.test(url);
            var isAnchor = /^#.*/.test(url);
            var hideText = editor.getSelection();

            if (isEmail) {
                var to = url.match(/mailto:([^?]*)(?:.*)/i);
                var cc = url.match(/(?:.*)(?:[^b]cc=([^&]*)&?)(?:.*)/i);
                var bcc = url.match(/(?:.*)(?:bcc=([^&]*)&?)(?:.*)/i);
                var subject = url.match(/(?:.*)(?:subject=([^&]*)&?)(?:.*)/i);
                var body = url.match(/(?:.*)(?:body=([^&]*)&?)(?:.*)/i);

                obj.email_linktext = link.textContent;
                obj.email_protocol = this.GetProtocol(url);
                obj.email_url = link.href;
                obj.email_to = (to ? to[1] : '');
                obj.email_cc = (cc ? cc[1] : '');
                obj.email_bcc = (bcc ? bcc[1] : '');
                obj.email_subject = (subject ? subject[1] : '');
                obj.email_body = (body ? body[1] : '');
                obj.email_target = link.target;
                obj.email_id = link.id;
                obj.email_name = link.name;
                obj.email_tooltip = link.title;
                obj.email_class = link.className.replace('cke_anchor', '');
                obj.email_style = (link.style.cssText ? link.style.cssText.toLowerCase() : '');
                obj.img_link = url;
            } else if (isAnchor) {
                var anchor_name = url.match(/(?:.*)#(.*)/);
                obj.anchor_linktext = link.innerText;
                obj.anchor_target = link.target;
                obj.anchor_id = link.id;
                obj.anchor_name = (anchor_name[1] ? anchor_name[1] : '');
                obj.anchor_tooltip = link.title;
                obj.anchor_class = link.className.replace('cke_anchor', '');
                obj.anchor_style = (link.style.cssText ? link.style.cssText.toLowerCase() : '');
                obj.anchor_protocol = this.GetProtocol(url);
                obj.img_link = url;
            } else {
                obj.img_link = url;
                obj.img_target = link.target;
                obj.link_target = link.target;
                var protocolIndex = url.indexOf('://');
                if (protocolIndex != -1) {
                    obj.link_url = url;
                    obj.link_protocol = url.substring(0, protocolIndex + 3);
                } else {
                    obj.link_url = url;
                    obj.link_protocol = 'other';
                }
                obj.link_text = (link.textContent ? link.textContent : '');
                obj.link_id = link.id;
                obj.link_name = link.name;
                obj.link_tooltip = link.title;
                obj.link_class = link.className.replace('cke_anchor', '');
                obj.link_style = (link.style.cssText ? link.style.cssText.toLowerCase() : '');
            }
        }
    }
    return obj;
}

function GetLinkElement(node, originalParent) {
    if ((node != null) && (node.tagName != null)) {
        if (node.tagName.toLowerCase() == "a") {
            return node;
        } else {
            if (node.tagName.toLowerCase() == "body") {
                return null;
            } else if (node.parentNode) {
                return GetLinkElement(node.parentNode);
            }
            else {
                return GetLinkElement(originalParent);
            }
        }
    }
    return null;
}

function GetColor(color) {
    if (color) {
        color = color.toLowerCase();
        if (color.indexOf('rgb') != -1) {
            var match = color.match(/rgb(?:[^0-9]*)([0-9]+)(?:[^0-9]*)([0-9]+)(?:[^0-9]*)([0-9]+)(?:[^0-9]*)/);
            var r = parseInt(match[1], 10);
            var g = parseInt(match[2], 10);
            var b = parseInt(match[3], 10);
            return '#' + (r > 16 ? r.toString(16) : '0' + r.toString(16)) + (g > 16 ? g.toString(16) : '0' + g.toString(16)) + (b > 16 ? b.toString(16) : '0' + b.toString(16));
        } else {
            if (color.indexOf('#') != -1) {
                var match = color.match(/(?:[^#0-9a-f]*)([a-f0-9]+)/);
                if (match[1] != null) {
                    return '#' + match[1];
                }
            } else {
                // Named color (eg. red)
                return color;
            }
        }
    }
    else {
        // return 'not set'
        return -1;
    }
}

function GetProtocol(url) {
    var i = url.indexOf('://');
    if (i > 0) {
        return url.substring(0, i + 3);
    }
    return '';
}