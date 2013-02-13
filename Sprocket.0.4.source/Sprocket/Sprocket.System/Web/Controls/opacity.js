Opacity = {
	Set : function(element, opacity) {
		if(navigator.userAgent.indexOf('MSIE') > -1) {
			element.style.zoom = 1;
			element.style.filter = 'alpha(opacity=' + opacity + ')';
		}
		else
			element.style.opacity = opacity/100;
	},
	
	Get : function(element) {
		if(navigator.userAgent.indexOf('MSIE') > -1) {
			if(element.style.filter == '') return 100;
			new RegExp('.*opacity[\s=]+([0-9\.]+)').exec(element.style.filter);
			if(isNaN(RegExp.$1) || RegExp.$1 == '') return 100;
			else return RegExp.$1 * 1;
		}
		else
			if(isNaN(element.style.opacity) || element.style.opacity == '') return 100;
			else return element.style.opacity * 100;
	},
	
	Fade : function(element, opacity, fadetime, interval, callback, /* internal -> */ startTime, startOpac) {
		if(!startTime) {
			if(!interval) interval = 20;
			startOpac = Opacity.Get(element);
			startTime = new Date();
		}
		var elapsed = new Date().getTime() - startTime.getTime();
		var perc = elapsed / fadetime;

		if(perc >= 1) {
			Opacity.Set(element, opacity);
			if(callback) callback();
			return;
		}
		
		var opac = startOpac + Math.round(perc * (opacity - startOpac));
		Opacity.Set(element, opac);
		setTimeout(function() { Opacity.Fade(element, opacity, fadetime, interval, callback, startTime, startOpac); }, interval);
	}
}
