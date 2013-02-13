Browser = {
	IsIE : false,
	IsFirefox : false,
	Name : 'Unknown',
	Version : '0.0',
	InitVars : function() {
		if(navigator.userAgent.indexOf('MSIE') > -1) {
			this.IsIE = true;
			this.Version = navigator.userAgent.replace(/.*MSIE ([0-9\.]+).*/, '$1');
			this.Name = 'Internet Explorer';
		} else if(navigator.userAgent.indexOf('Firefox') > -1) {
			this.IsFirefox = true;
			this.Version = navigator.userAgent.replace(/.*Firefox\/([0-9\.]+).*/, '$1');
			this.Name = 'Mozilla Firefox';
		}
	}
}
Browser.InitVars();

// firefox doesn't support the onresize event handler for elements other than document and window.
// this object aims to provide a means to fire onresize events on specified elements via a short
// recurring time interval. this method is used instead of the window/document onresize events
// because elements can sometimes be resized by parent elements without the window having been resized.
FirefoxResizeEventHandler = {
	interval : null,
	elements : [],
	Init : function() {
		if(!Browser.IsFirefox) return;
		this.interval = setInterval( function() { FirefoxResizeEventHandler.Fire(); }, 300 );
	},
	Fire : function() {
		for(var i=0; i<this.elements.length; i++) {
			var element = this.elements[i];
			if(!element.onresize || element.parentNode == null) continue;
			if(typeof(element.onresize) != 'function') continue;
			if(element.clientWidth != element.oldWidth || element.clientHeight != element.oldHeight) {
				element.oldWidth = element.clientWidth;
				element.oldHeight = element.clientHeight;
				element.onresize();
			}
		}
	},
	RegisterElement : function(element) {
		this.elements[this.elements.length] = element;
		element.oldWidth = element.clientWidth;
		element.oldHeight = element.clientHeight;
	}
}
FirefoxResizeEventHandler.Init();

// The stylesheet model in existing browsers seems somewhat lacking. This helper objects allows
// stylesheet classes to be found by name, irrespective of which stylesheet they're in.
StyleSheetHelper = {
	Rules : null,
	initialised : false,
	Initialise : function() {
		this.Rules = [];
		for(var i=0; i<document.styleSheets.length; i++) {
			var rules = document.styleSheets[i].cssRules ? document.styleSheets[i].cssRules : document.styleSheets[i].rules;
			for(var j=0; j<rules.length; j++) {
				var rule = this.Rules[rules[j].selectorText.toLowerCase()];
				if(!rule) {
					rule = [];
					this.Rules[rules[j].selectorText.toLowerCase()] = rule;
				}
				rule[rule.length] = rules[j];
			}
		}
	},
	_getSize : function(px) {
		if(typeof(px) != 'string') return 0;
		if(px.length < 3) return 0;
		if(px.substr(px.length-2,2).toLowerCase() != 'px') return 0;
		var n = px.substr(0,px.length-2);
		if(isNaN(n)) return 0;
		return parseInt(n);
	},
	CalcExtraWidth : function(stylename) {
		if(!this.initialised) this.Initialise();
		var rule = this.Rules[stylename.toLowerCase()];
		if(!rule) return 0;
		var x = 0;
		for(var i=0; i<rule.length; i++) {
			x += this._getSize(rule[i].style.borderLeftWidth);
			x += this._getSize(rule[i].style.borderRightWidth);
			x += this._getSize(rule[i].style.paddingLeft);
			x += this._getSize(rule[i].style.paddingRight);
		}
		return x;
	},
	CalcExtraHeight : function(stylename) {
		if(!this.initialised) this.Initialise();
		var rule = this.Rules[stylename.toLowerCase()];
		if(!rule) return 0;
		var x = 0;
		for(var i=0; i<rule.length; i++) {
			x += this._getSize(rule[i].style.borderTopWidth);
			x += this._getSize(rule[i].style.borderBottomWidth);
			x += this._getSize(rule[i].style.paddingTop);
			x += this._getSize(rule[i].style.paddingBottom);
		}
		return x;
	}
}
