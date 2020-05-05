
// global variables (for backward compatibility with other JS code)
var menuSettings = {},
    dynamicMenus = [],
    loadingContent = [],
    CM_Init,
    CM_Over,
    CM_Out,
    CM_Up,
    CM_Receive,
    CM_MenuOver,
    CM_MenuOut,
    CM_Close,
    CM_RegisterMenuHandler,
    CM_InvokeHandlers,   
    ContextMenu,
    HideContextMenu,
    GetContextMenuParameter,
    SetContextMenuParameter,
    HideAllContextMenus,
    RegisterAsyncCloser,
    ActivateParentBorder,
    DeactivateParentBorder;

(function (w) {

    var document = w.document,
        menuTimers = [],
        menuElements = [],
        openMenus = [],
        mouseOverTimers = [],
        timerParameters = [],
        menuParameters = [],
        repositionedMenus = [],
        mouseOverLevel = [],
        CM_handlers = [],
        x = 0,
        y = 0,
        submenuShift = 10,
        show = true,
        currentMenuId = null,
        currentElem = null,
        lastMenuZindex = 20201,
        
        isFunction = function(f) {
            return !!(f && f.constructor && f.call && f.apply); // Taken from Underscore.js, see http://jsperf.com/alternative-isfunction-implementations
        },
        
        getElem = function (id) {
            return document.getElementById(id);
        },

        getScrollX = function () {
            if (typeof(w.pageXOffset) === 'number') {
                return w.pageXOffset;
            } else if (document.body && (document.body.scrollLeft || document.body.scrollTop)) {
                return document.body.scrollLeft;
            } else if (document.documentElement && (document.documentElement.scrollLeft || document.documentElement.scrollTop)) {
                return document.documentElement.scrollLeft;
            }

            return 0;
        },

        getScrollY = function () {
            if (typeof(w.pageYOffset) === 'number') {
                return w.pageYOffset;
            } else if (document.body && (document.body.scrollLeft || document.body.scrollTop)) {
                return document.body.scrollTop;
            } else if (document.documentElement && (document.documentElement.scrollLeft || document.documentElement.scrollTop)) {
                return document.documentElement.scrollTop;
            }

            return 0;
        },

        getMouseX = function (ev) {
            return ev.clientX + getScrollX();
        },

        getMouseY = function (ev) {
            return ev.clientY + getScrollY();
        },

        isLeftButton = function (ev) {
            var bt = ev.button,
                b = $cmsj.browser;
            
            if (b.msie) {
                return (bt === 0) || (bt === 1);
            } else if (b.mozilla || b.opera || b.webkit) {
                return (bt === 0);
            } else {
                return (bt === 1);
            }
        },
        
        isRightButton = function (ev) {
            return (ev.button === 2);
        },

        prepareContextMenu = function (ev) {
            var menuButtons, menuElem, s;
            
            if (ev) {
                menuElem = getElem(currentMenuId);
                if (menuElem) {
                    s = menuSettings[currentMenuId];
                    if (s.horizontal === 'Cursor') {
                        x = getMouseX(ev) + 1;
                    }
                    if (s.vertical === 'Cursor') {
                        y = getMouseY(ev) + 1;
                    }

                    show = false;
                    menuButtons = s.button;
                    if (((menuButtons === 'Right') || (menuButtons === 'Both')) && (isRightButton(ev))) {
                        show = true;
                    }
                    if (((menuButtons === 'Left') || (menuButtons === 'Both')) && (isLeftButton(ev))) {
                        show = true;
                    }
                }
            }

            return false;
        },

        stopPropagation = function (ev, force) {
            if (w.isDragging && !force) {
                return true;
            }

            if (!ev) {
                ev = w.event;
            }

            ev = $cmsj.event.fix(ev);
            ev.stopPropagation();
            ev.preventDefault();

            return false;
        },

        checkMenuHandled = function () {
            if (document.body.cmHandled) {
                return (document.body.cmHandled = false);
            }

            return true;
        },

        initContextMenu = function (menuId, elem) {
            currentMenuId = menuId;
            if (elem) {
                if (!elem.contextMenuInitialized) {
                    elem.contextMenuInitialized = true;
                    elem.onmousedown = prepareContextMenu;

                    document.body.oncontextmenu = checkMenuHandled;
                    elem.oncontextmenu = function() {
                        return false;
                    };
                }
            }

            return false;
        },
        
        ensureVisibility = function (menuElem) {
            var ctx = w.currentMenuContext, jm, se, he;
            if (ctx) {
                jm = $cmsj(menuElem);

                if (ctx.showClass) {
                    se = jm.find('.' + ctx.showClass);
                    se.show();
                }
                if (ctx.hideClass) {
                    he = jm.find('.' + ctx.hideClass);
                    he.hide();
                }
            }
        },
        
        applyY = function (menuElem) {
            var y = menuElem.y,
                s = menuElem.s,
                absOff = menuElem.absOff,
                c = menuElem.currentElem,
                bot, top, yOff, sy, maxy;

            if (s.up) {
                y -= menuElem.offsetHeight;
            }

            if (absOff) {
                y -= absOff.y;
            }

            bot = y + menuElem.offsetHeight;
            top = y;

            sy = w.scrollY;
            if (!sy) sy = document.documentElement.scrollTop;
            maxy = document.documentElement.offsetHeight + sy;
            if (bot > maxy) {
                yOff;
                switch (s.vertical) {
                    case 'Bottom':
                        yOff = menuElem.offsetHeight + c.offsetHeight;
                        break;
                    default:
                        yOff = menuElem.offsetHeight;
                        break;
                }
                y -= yOff;
                top -= yOff;
            }
            if (top < 0) {
                y -= top;
            }

            menuElem.style.top = y + 'px';
        },

        applyX = function (menuElem) {
            var x = menuElem.x,
                s = menuElem.s,
                absOff = menuElem.absOff,
                right, left, sx, maxx, xOff;

            if (s.rtl) {
                x -= menuElem.offsetWidth;
            }

            if (absOff != null) {
                x -= absOff.x;
            }

            right = x + menuElem.offsetWidth;
            left = x;

            sx = w.scrollX;
            if (!sx) sx = document.documentElement.scrollLeft;
            maxx = document.documentElement.offsetWidth + sx;
            if (right > maxx) {
                xOff = menuElem.offsetWidth;
                x -= xOff;
                left -= xOff;
            }
            if (left < 0) {
                x -= left;
            }

            menuElem.style.left = x + 'px';
        },

        visible = function (menuId) {
            var visibleHandler = w[menuId + '_ContextMenuVisible'];
            if (visibleHandler) {
                visibleHandler();
            }
        },
        
        clearHideTimeout = function (menuId) {
            if (menuTimers[menuId]) {
                clearTimeout(menuTimers[menuId]);
                menuTimers[menuId] = null;
            }
        },

        setHideTimeout = function (context, hideSubMenus) {
            menuTimers[context] = setTimeout(function() {
                hideContextMenuTimeout(context, hideSubMenus);
            }, 2000);
        },
        
        hideContextMenuTimeout = function (menuId, hideSubMenus) {
            var s = menuSettings[menuId];
            if (isCursorOnMenu(s.level)) {
                setHideTimeout(menuId, hideSubMenus);
                return;
            }

            HideContextMenu(menuId, true);
        },
        
        hideContextMenus = function (startLevel) {
            var i, menu
            for (i = openMenus.length - 1; i >= startLevel; i--) {
                menu = openMenus[i];
                if (menu) {
                    if (menu.s.level >= startLevel) {
                        menu.style.display = 'none';
                        openMenus[i] = null;
                    }
                }
            }
        },

        isCursorOnMenu = function (level) {
            var i;
            for (i = level; i < mouseOverLevel.length; i++) {
                if (mouseOverLevel[i]) {
                    return true;
                }
            }

            return false;
        },
        
        getParentRelativeOffset = function (el) {
            var offsetX = 0,
                offsetY = 0,
                parent,
                $parent,
                pos, off;

            if (el) {
                for (parent = el; parent; parent = parent.offsetParent) {
                    $parent = $cmsj(parent);
                    pos = $parent.css('position');
                    if ((pos == 'relative') || (pos == 'fixed') || (pos == 'static')) {
                        off = $parent.offset();
                        offsetX = off.left;
                        offsetY = off.top;
                        break;
                    }
                }
            }

            return { x: offsetX, y: offsetY };
        };

    // Register handler for specific menu with optional custom parameter
    CM_RegisterMenuHandler = function (menuId, handler, customParam, customParam2) {
        var localHandler = {};
        localHandler.menuId = menuId;
        localHandler.handler = handler;
        localHandler.customParam = customParam;
        localHandler.customParam2 = customParam2;
        CM_handlers.push(localHandler);
    };


    // Invoke handlers for specific menu
    CM_InvokeHandlers = function (menuId, param) {
        var i;
        for (i = 0; i < CM_handlers.length; i++) {
            if (CM_handlers[i].menuId === menuId) {
                CM_handlers[i].handler(menuId, param, CM_handlers[i].customParam, CM_handlers[i].customParam2);
            }
        }
    };

    ContextMenu = function (menuId, elem, param, forceShow, mousex, mousey, checkRelative, ev, ctx) {
        var menuElem = getElem(menuId),
            $currentElem, pos, s, currentElemWidth, currentElemHeight,
            menuButtons, isDynamicMenu, loading, jm, b,
            activeElem, updateCss, lvl;
        
        if (menuElem) {

            CM_InvokeHandlers(menuId, param);

            if (forceShow) {
                show = true;
            }
            if (elem) {
                currentElem = elem;
            }

            if (mouseOverTimers[menuId]) {
                clearInterval(mouseOverTimers[menuId]);
                mouseOverTimers[menuId] = null;
            }

            if (currentMenuId) {
                hideContextMenus(menuSettings[currentMenuId].level);
            }

            HideContextMenu(menuId);

            s = menuSettings[menuId];

            if (w.event) {
                mousex = getMouseX(w.event);
                mousey = getMouseY(w.event);

                if (!forceShow) {
                    show = false;
                    menuButtons = s.button;
                    if (((menuButtons === 'Right') || (menuButtons === 'Both')) && isRightButton(w.event)) {
                        show = true;
                        document.body.cmHandled = true;
                    }
                    if (((menuButtons === 'Left') || (menuButtons === 'Both')) && isLeftButton(w.event)) {
                        show = true;
                    }
                }
            }

            if ((mousex) && (s.horizontal === 'Cursor')) {
                x = mousex;
            }
            if ((mousey) && (s.vertical === 'Cursor')) {
                y = mousey;
            }

            $currentElem = $cmsj(currentElem);
            pos = $currentElem.offset();
            currentElemWidth = $currentElem.width();
            currentElemHeight = $currentElem.height();
            
            switch (s.vertical) {
                case 'Top':
                    y = pos.top;
                    break;
                case 'Bottom':
                    y = pos.top + currentElemHeight;
                    break;
                case 'Middle':
                    y = pos.top + currentElemHeight / 2;
                    break;
            }

            switch (s.horizontal) {
                case 'Left':
                    x = pos.left;
                    
                    if (s.rtl) {
                        if (s.level === 0) {
                            x += currentElemWidth;
                        }
                    } else {
                        if (s.level > 0) {
                            x += currentElemWidth;
                        }
                    }

                    break;
                
                case 'Right':
                    x = pos.left;
                    if (!s.rtl) {
                        x += currentElemWidth;
                    }
                    break;
                
                case 'Center':
                    x = pos.left + currentElemWidth / 2;
                    break;
                
                case 'Cursor':
                    if (!s.rtl) {
                        x++;
                    } else {
                        x--;
                    }
                    break;
            }
            
            if (s.level === 0) {
                if (s.rtl) {
                    x -= s.offsetx;
                } else {
                    x += s.offsetx;
                }
            } else {
                if (!s.rtl) {
                    x -= submenuShift;
                } else {
                    x += submenuShift;
                }
            }
            
            y += s.offsety;

            if (show) {
                SetContextMenuParameter(menuId, param);
                isDynamicMenu = (dynamicMenus[menuId]);
                if (isDynamicMenu) {
                    loading = loadingContent[menuId];
                    if (loading) {
                        menuElem.innerHTML = loading;
                    } else {
                        menuElem.innerHTML = '';
                    }
                    dynamicMenus[menuId](param);
                }

                jm = $cmsj(menuElem);

                if (s.level === 0) {
                    w.currentMenuContext = ctx;
                }
                ensureVisibility(menuElem);

                menuElem.style.display = 'block';
                menuElem.style.zIndex = lastMenuZindex++;

                // Reposition to the body
                if (s.level > 0) {
                    b = document.body;
                    if (menuElem.parentNode !== b) {
                        jm.detach().appendTo('body');
                        repositionedMenus[menuId] = menuElem;
                    }
                }

                menuElem.s = s;
                openMenus.push(menuElem);

                menuElem.currentElem = currentElem;
                menuElem.absOff = getParentRelativeOffset(menuElem);

                menuElem.y = y;
                applyY(menuElem);

                menuElem.x = x;
                applyX(menuElem);

                jm.hide().fadeIn('fast');
                jm.addClass(s.rtl ? 'RTL' : 'LTR');

                if (!isDynamicMenu) {
                    setHideTimeout(menuId);
                    visible(menuElem.id);
                }
                activeElem = currentElem;

                // Do not change container class
                updateCss = true;
                for (lvl = 0; lvl < s.activecssoffset; lvl++) {
                    if (activeElem.parentNode) {
                        activeElem = activeElem.parentNode;
                    } else {
                        updateCss = false;
                    }
                }
                if ((s.activecss !== '') && updateCss) {
                    if (s.inactivecss === '') {
                        s.inactivecss = activeElem.className;
                    }
                    activeElem.oldClassName = s.inactivecss;
                    activeElem.className = s.activecss;
                }
                menuElements[menuId] = currentElem;

                if ($currentElem.parents().hasClass("LayoutWebPartHeader")) {
                    // Layout web parts cannot be personalised
                    if (window.webPartCPVariantContextMenuId) {
                        document.getElementById(window.webPartCPVariantContextMenuId).style.display = "none";
                    }

                    // or used in MV tests
                    if (window.webPartMVTVariantContextMenuId) {
                        document.getElementById(window.webPartMVTVariantContextMenuId).style.display = "none";
                    }
                }

                if (ev) {
                    return stopPropagation(ev);
                }
            }
        }

        return undefined;
    };

    CM_Init = function (menuId) {
        var menuElem = repositionedMenus[menuId];
        if ((menuElem) && (menuElem.parentNode)) {
            menuElem.parentNode.removeChild(menuElem);
        }
    };

    HideContextMenu = function (menuId, hideSubmenus) {
        var menuElem = getElem(menuId), s, menuLevel, activecssoffset,
            activeElem, updateCss, lvl;
        
        if (menuElem) {
            if (menuTimers[menuId]) {
                s = menuSettings[menuId];
                if (hideSubmenus) {
                    menuLevel = s.level;
                    hideContextMenus(menuLevel + 1);
                }

                clearHideTimeout(menuId);

                if (mouseOverTimers[menuId]) {
                    clearInterval(mouseOverTimers[menuId]);
                    mouseOverTimers[menuId] = null;
                }
                if (menuElements[menuId]) {

                    activecssoffset = s.activecssoffset;
                    activeElem = menuElements[menuId];
                    // Do not change container class
                    updateCss = true;
                    for (lvl = 0; lvl < activecssoffset; lvl++) {
                        if (activeElem.parentNode) {
                            activeElem = activeElem.parentNode;
                        } else {
                            updateCss = false;
                        }
                    }
                    if ((s.activecss !== '') && updateCss) {
                        activeElem.className = (activeElem.oldClassName) ? activeElem.oldClassName : '';
                    }

                    menuElements[menuId] = null;
                }

                $cmsj(menuElem).hide().fadeOut('fast');
            }
        }
    };

    SetContextMenuParameter = function (menuId, param) {
        menuParameters[menuId] = param;
        var paramElem = getElem(menuId + '_parameter');
        if (paramElem) {
            paramElem.value = param;
        }

        if (w.OnSetContextMenuParameter && isFunction(OnSetContextMenuParameter)) {
            OnSetContextMenuParameter(menuId, param);
        }

        return null;
    };

    GetContextMenuParameter = function(menuId) {
        return menuParameters[menuId];
    };

    CM_Receive = function(rvalue, context) {
        var menuElem = repositionedMenus[context];

        if (!menuElem) {
            menuElem = getElem(context);
        }

        $cmsj(menuElem).html(rvalue);

        applyX(menuElem);
        applyY(menuElem);

        visible(context);
        ensureVisibility(menuElem);

        setHideTimeout(context);
    };

    CM_Close = function(menuId) {
        HideContextMenu(menuId, true);
    };

    HideAllContextMenus = function() {
        if (!isCursorOnMenu(10)) {
            hideContextMenus(0);
        }
    };

    CM_MenuOver = function(level) {
        mouseOverLevel[level] = true;
    };

    CM_MenuOut = function(level) {
        mouseOverLevel[level] = false;
    };

    CM_Out = function(ev, menuId) {
        if (!ev) {
            ev = w.event;
        }

        if (!ev.cmHandled) {
            if (mouseOverTimers[menuId]) {
                clearInterval(mouseOverTimers[menuId]);
                mouseOverTimers[menuId] = null;
            }

            ev.cmHandled = true;
        }

        return false;
    };

    CM_Over = function (ev, menuId, elem, param, getCtx) {
        var menuElem, s, forceShow, mousex, mousey;

        if (!ev) {
            ev = w.event;
        }

        initContextMenu(menuId, elem, getCtx);

        if (!ev.cmHandled) {
            menuElem = getElem(menuId);
            if (menuElem) {
                s = menuSettings[menuId];
                if (s.mouseover) {
                    currentElem = elem;

                    if ((getElem(menuId).style.display !== 'none') && (timerParameters[menuId] !== param)) {
                        HideContextMenu(menuId, true);
                    }

                    if (getElem(menuId).style.display === 'none') {
                        timerParameters[menuId] = null;
                        if (!(mouseOverTimers[menuId])) {
                            timerParameters[menuId] = param;
                            if (w.event) {
                                forceShow = true;
                                mousex = getMouseX(w.event);
                                mousey = getMouseY(w.event);
                            }
                            mouseOverTimers[menuId] = setInterval(function() {
                                ContextMenu(menuId, null, param, forceShow, mousex, mousey);
                            }, 1000);
                        }
                    }
                }
            }

            ev.cmHandled = true;
        }

        return false;
    };

    CM_Up = function (ev, menuId, elem, param, checkRelative, getCtx) {
        var ctx;
        if (!ev) {
            ev = w.event;
        }

        if (!ev.cmHandled) {
            ctx = (getCtx ? getCtx(elem) : null);

            w.suppressDrag = true;
            ContextMenu(menuId, elem, param, null, null, null, checkRelative, ev, ctx);

            ev.cmHandled = true;
        }

        return false;
    };

    RegisterAsyncCloser = function (closer) {
        if (w.Sys && w.Sys.WebForms && Sys.WebForms.PageRequestManager) {
            w.Sys.WebForms.PageRequestManager.getInstance().add_endRequest(closer);
        }
    };

    // Activates the parent border when a variant context menu is hovered
    ActivateParentBorder = function() {
        var el = document.getElementById(currentContextMenuId + '_border');
        if (w.ActivateBorder && isFunction(ActivateBorder)) {
            ActivateBorder(currentContextMenuId, el);
        }
    };

    // Deactivates the parent border when the mouse leaves a variant context menu
    DeactivateParentBorder = function() {
        var el = document.getElementById(currentContextMenuId + '_border');
        if (w.ActivateBorder && isFunction(ActivateBorder)) {
            DeactivateBorder(currentContextMenuId, el);
        }
    };
    
}(window));