// macro for getElementById; returns e if 
function $(e) { return (typeof(e) == 'object') ? e : document.getElementById(e); }

// is value equal to any of the following arguments (similar to SQL's IN operator)
function $in(value, arg1, arg2, arg99) {
	for(var i=1; i<arguments.length; i++)
		if(arguments[i] == value) return true;
	return false;
}

// finds all descendants of a specified element that have the named attribute which
// is optionally set to a specified value
function $childrenWithAttribute(parentElement, attributeName, attributeValue) {
	var found = (arguments.length <= 3) ? [] : arguments[3];
	var pe = $(parentElement);
	for(var i=0; i<pe.childNodes.length; i++) {
		if(!pe.childNodes[i].getAttribute) continue;
		var val = pe.childNodes[i].getAttribute(attributeName);
		if(val != '' && val != null && (val == attributeValue || !attributeValue))
			found[found.length] = pe.childNodes[i];
		$childrenWithAttribute(pe.childNodes[i], attributeName, attributeValue, found);
	}
	return found;
}

// finds the parent element that has the specified attribute, optionally with a specific
// value. If it doesn't exist, null is returned.
function $parentWithAttribute(element, attributeName, attributeValue) {
	if(!element.getAttribute)
		return null;
	var val = element.getAttribute(attributeName);
	if(val != '' && (val == attributeValue || !attributeValue))
		return element;
	if(!element.parentNode)
		return null;
	return $parentWithAttribute(element.parentNode, attributeName, attributeValue);
}

function $nullIfBlank(str) {
	if(str == '')
		return null;
	else
		return str;
}

function htmlEncode(str) {
	if(str == null) return '';
	return str.replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;');
}

function trim(str) {
	return str.replace(/^\s*((\S|(\S\s+\S))+)\s*$/, '$1');
}

function queryString(arg) {
	var qn = location.href.indexOf('?');
	if(qn == -1 || qn == location.href.length - 1) return null;
	var qs = location.href.substring(qn+1);
	var arr = qs.split(/&/g);
	for(var i=0; i<arr.length; i++) {
		var nvp = arr[i].split(/=/);
		if(nvp.length == 2 && nvp[0] == arg)
			return unescape(nvp[1]);
	}
	return null;
}

function parseDate(dateString) {
	return new Date(Date.parse(dateString));
}

function isValidEmail(email) {
	return (/^[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$/i).test(email);
}

function getGMTOffset() {
	var dt = new Date();
	var sub = dt.getUTCDate() < dt.getDate();
	var hours;
	if(sub)
		hours = ((24 - dt.getUTCHours()) + dt.getHours());
	else
		hours = 0 - (dt.getUTCHours() - dt.getHours());
	return hours;
}

Guid = {
	Empty : '00000000-0000-0000-0000-000000000000'
};

// event handler class
function Event() {
	this.handlers = [];
}

Event.prototype.AddHandler = function(funcRef, context) {
	this.handlers[this.handlers.length] = {
		funcRef : funcRef,
		context : context
	};
}

Event.prototype.Fire = function() {
	for(var i=0; i<this.handlers.length; i++) {
		if(this.handlers[i].context)
			this.handlers[i].funcRef.call(this.handlers[i].context, arguments);
		else
			this.handlers[i].funcRef(arguments);
	}
}

// timer class
function Timer(interval) {
	this.interval = interval*1;
	this.Elapsed = new Event();
}

Timer.prototype.Start = function(interval) {
	this.Stop();
	if(interval) this.interval = interval*1;
	var refthis = this;
	this.intervalID = setInterval(function() { refthis.OnTimerElapsed(); }, this.interval);
}

Timer.prototype.RunOnce = function(interval) {
	this.Stop();
	if(interval) this.interval = interval*1;
	var refthis = this;
	this.intervalID = setTimeout(function() { refthis.OnTimerElapsed(); }, this.interval);
}

Timer.prototype.Stop = function() {
	if(!this.intervalID) return;
	clearTimeout(this.intervalID);
	this.intervalID = null;
}

Timer.prototype.OnTimerElapsed = function() {
	this.Elapsed.Fire();
}

function debug() {
	var str = 'DEBUG:';
	for(var i=0; i<arguments.length; i++)
		str += '\n' + arguments[i];
	alert(str);
}
