// JScript File

// 2D array of divs in scrollers
var scrollernodes = new Object();
// Array of scroller effects
var scrollActiveFx = new Object();
// Array of scroller effects
var scrollNextFx = new Object();
// Array of curent pages on scroller
var curentScroll = new Object();
// Array of stoped scrollers
var stopedScroll = new Object();

var Sroller = new Class({
    // inicialization of slider
    initialize: function(id, direction, moveTime, stopTime, onMouseStop, width, height) {
        this.id = id;
        this.direction = direction;
        this.moveTime = moveTime;
        this.stopTime = stopTime;
        this.width = width;
        this.height = height;
        if (onMouseStop == "True") { this.onMouseStop = true; } else { this.onMouseStop = false; }
        scrollernodes[this.id] = [];
        curentScroll[this.id] = 1;
        stopedScroll[this.id] = true;
        var scroll = $(this.id);
        var alldivs = $$("div.scrollerContent");
        for (var i = 0; i < alldivs.length; i++) {
            if ((alldivs[i]).parentNode.id == this.id) {
                scrollernodes[this.id].push(alldivs[i]);
            }
        }
        if (scrollernodes[this.id].length != 0) {
            //inicialize divs for active and next
            this.active = scrollernodes[this.id][0];
            if (scrollernodes[this.id].length > 1) {
                this.next = scrollernodes[this.id][1];
            }
            else {
                // if only one then next = active.clone
                var cloneObj = this.active.clone(true, true);
                cloneObj.inject(scroll);
                cloneObj.style.position = 'absolute';
                cloneObj.style.overflow = 'hidden';
                cloneObj.style.width = this.active.style.width;
                cloneObj.style.height = this.active.style.height;
                cloneObj.className = this.active.className;
                scrollernodes[this.id][1] = cloneObj;
                this.next = scrollernodes[this.id][1];
            }

            scrollActiveFx[this.id] = new Fx.Morph(this.active, { duration: this.moveTime });
            scrollNextFx[this.id] = new Fx.Morph(this.next, { duration: this.moveTime });

            //show first
            this.show();

            // if is set onMouseOver stop
            if (this.onMouseStop) {
                //OnMouseOver pause
                var delay = this.moveTime + this.stopTime;
                var stop = this.stopTime;
                var sroller = this;
                scroll.addEvents({
                    mouseenter: function() {
                        scrollActiveFx[sroller.id].pause();
                        scrollNextFx[sroller.id].pause();
                        clearTimeout(window[sroller.id + "timer"]);
                    },
                    mouseleave: function() {
                        if (stopedScroll[sroller.id]) {
                            window[sroller.id + "timer"] = setTimeout(function() { startScroller(sroller, delay, true); }, stop);
                        }
                        else {
                            stopedScroll[sroller.id] = false;
                            scrollActiveFx[sroller.id].resume();
                            scrollNextFx[sroller.id].resume();
                            window[sroller.id + "timer"] = setTimeout(function() { startScroller(sroller, delay, false); }, stop);
                        }
                    }
                });
            }
        }
    },
    //show start page and move to top and hide others
    show: function() {
        var dir = this.direction;
        var height = this.height;
        var width = this.width;
        var id = this.id;

        // set starting positions for all divs 
        scrollernodes[this.id].each(function(item, index) {
            item.setOpacity(0);
            if (dir == 0) { // Down->Up
                if (index == 0) { var t = 0; } else { var t = height; }
                item.setStyle('top', t);
            }
            if (dir == 1) { // Up->Down
                if (index == 0) { var t = 0; } else { var t = -height; }
                item.setStyle('top', t);
            }
            if (dir == 2) { // Left->Right
                if (index == 0) { var t = 0; var l = 0; }
                else { var t = 0; var l = -width; }
                item.setStyles({ 'top': t, 'left': l });
            }
            if (dir == 3) { // Right->Left
                if (index == 0) { var t = 0; var l = 0; }
                else { var t = 0; var l = width; }
                item.setStyles({ 'top': t, 'left': l });
            }
        });
        // show active and next div
        this.active.setOpacity(1);
        this.next.setOpacity(1);

        // show main div when everything is set
        $(this.id).style.visibility = 'visible';
    },
    //move div
    move: function() {

        stopedScroll[this.id] = false;

        scrollActiveFx[this.id] = new Fx.Morph(this.active, { duration: this.moveTime });
        scrollNextFx[this.id] = new Fx.Morph(this.next, { duration: this.moveTime });

        var thisObj = this;
        scrollNextFx[this.id].addEvent('complete', function() { thisObj.swap(); });

        if (this.direction == 0) { // Down->Up
            //active top 
            scrollActiveFx[this.id].start({ 'top': [0, -this.height] });
            //next top
            scrollNextFx[this.id].start({ 'top': [this.height, 0] });
        }
        if (this.direction == 1) { // Up->Down
            //active top 
            scrollActiveFx[this.id].start({ 'top': [0, this.height] });
            //next top
            scrollNextFx[this.id].start({ 'top': [-this.height, 0] });
        }
        if (this.direction == 2) { // Left->Right
            //active left
            scrollActiveFx[this.id].start({ 'left': [0, this.width] });
            //next left
            scrollNextFx[this.id].start({ 'left': [-this.width, 0] });
        }
        if (this.direction == 3) { // Right->Left
            //active left
            scrollActiveFx[this.id].start({ 'left': [0, -this.width] });
            //next left
            scrollNextFx[this.id].start({ 'left': [this.width, 0] });
        }

    },
    //swap divs
    swap: function() {
        stopedScroll[this.id] = true;

        //hide active div
        this.active.setOpacity(0);

        curentScroll[this.id]++;
        if (curentScroll[this.id] == scrollernodes[this.id].length) {
            curentScroll[this.id] = 0;
        }
        //swap divs
        this.active = this.next;
        this.next = scrollernodes[this.id][curentScroll[this.id]];
        //show next div
        this.next.setOpacity(1);
    }
});

function startScroller(scrollingText, time, move) {
    if (move) {
        scrollingText.move();
    }
    window[scrollingText.id + "timer"] = setTimeout(function() { startScroller(scrollingText, time, true); }, time);
}