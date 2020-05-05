cmsdefine([], function() {

    function ObjectWithEvents() {
        this.events = {};
    }


    ObjectWithEvents.prototype.createEvent = function(event) {
        if(!event)
        {
            throw 'Event argument has to be specified';
        }

        if(this.events[event])
        {
            throw 'Event was already added to the event collection';
        }

        this.events[event] = [];
    };


    ObjectWithEvents.prototype.addListener = function(event, handler) {
        if (!event) {
            throw 'Event argument has to be specified';
        }

        if (!handler) {
            throw 'Event handler has to be specified';
        }

        if (!this.events[event]) {
            throw 'Given event does not exist';
        }

        this.removeListener(event, handler);
        this.events[event].push({
            handler: handler
        });
    };


    ObjectWithEvents.prototype.removeListener = function(event, handler) {
        if (!event) {
            throw 'Event argument has to be specified';
        }

        if (!handler) {
            throw 'Event handler has to be specified';
        }

        if (!this.events[event]) {
            throw 'Given event does not exist';
        }

        var ev = this.events[event];
        for (var i = ev.length - 1; i >= 0; i--) {
            if (ev[i].handler === handler) {
                ev.splice(i, 1);
            }
        }
    };


    ObjectWithEvents.prototype.raise = function(event) {
        if (!event) {
            throw 'Event argument has to be specified';
        }

        if(!event.type) {
            throw 'Event type has to be specified';
        }

        var handlers = this.events[event.type];
        if (!handlers) {
            throw 'Given event does not exist';
        }

        handlers.forEach(function(handler) {
            handler.handler.call(undefined, event);
        });
    };

    return ObjectWithEvents;
});