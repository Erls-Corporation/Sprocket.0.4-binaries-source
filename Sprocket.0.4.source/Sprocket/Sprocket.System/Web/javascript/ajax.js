/*
	// Sprocket AJAX Engine //
	-----------------------------------
	Nathan Ridley / snowdevil@gmail.com
*/

// Ensures existance of a universal request object that works across different browsers
if(typeof XMLHttpRequest == 'undefined')
	XMLHttpRequest = function() {
		try {
			return new ActiveXObject("Microsoft.XMLHTTP");
		} catch(ex) {
			return null;
		}
	}

/*
	Ajax methods are called via objects created dynamically by the server. The objects are
	named the same as the module that defines the server-side methods, and contain a method
	that has the same name as the server-side method. For example, the server could generate:
	Ajax.SomeModule = {
		FirstMethod : function() { Ajax.Request('SomeModule.FirstMethod', arguments); },
		SecondMethod : function() { Ajax.Request('SomeModule.SecondMethod', arguments); }
	}
	Let's assume First method is defined on the server like so:
	public int FirstMethod(string msg, int num, DateTime requestDate)
	
	In this case, to make an ajax request to SomeModule and call FirstMethod, you'd use
	something like the following javascript:
	
	Ajax.SomeModule.FirstMethod('my message', 7, new Date(), myCallbackFunction, myContextObject1, ..., myContextObjectN);
	
	You'd make sure you define a function somewhere called myCallbackFunction of course.
	The format of myCallbackFunction is:
	
	functionName(response, ajaxCallNumber, myContextObject1,...n).
	
	The arguments immediately following the callback functions are context objects.
	Context objects are simply arguments that are passed to your callback function, so ensure
	that your callback function defines the appropriate matching arguments in the function
	declaration.
	
	You can also optionally specify the object that the callback function should be called on behalf of.
	Normally simply specifying a callback function will mean that it gets called independently of where
	it is defined. So for example if you have an object called Dog and Dog has a method called Bark() and
	you specify Bark() as the callback function, then using the keyword "this" inside the Bark() method
	will cause problems because "this" won't refer to the Dog object when it is called when the Ajax
	operation returns.
		To get around this problem, instead of specifying the callback function directly, you can place
	it in a two-element array where the first element is the function, and the second element is a
	reference to the object that owns the function and that the function should be called in the context
	of. For example:
	
	var dog = new Object();
	dog.Bark = function() { alert('Woof! My name is ' + this.Name); }
	dog.Name = 'Spot';
	Ajax.SomeServerObject.SomeMethod(arg1, arg2, [dog.Bark, dog]);
	
	When the Ajax method returns from the server, it will call the Bark method as though you had typed
	dog.Bark() instead of just Bark() by itself. The line above would produce a message box that says
	"Woof! My name is Spot"
	
	If however you had called the Ajax method like this:
	Ajax.SomeServerObject.SomeMethod(arg1, arg2, dog.Bark);
	The messagebox would say:
	"Woof! My name is undefined"
	
*/
SprocketAjax = {
	RequestPool : [],
	AuthKey : '$AUTHKEY$',
	ApplicationRoot : '$APPLICATIONROOT$',
	LoadTimeStamp : '$LOADTIMESTAMP$',
	
	CreateRequest : function() {
		for(var i=0; i<this.RequestPool.length; i++) {
			var x = this.RequestPool[i];
			if(x.readyState == 4) {
				x.abort();
				return x;
			}
		}
		var x = new XMLHttpRequest();
		this.RequestPool[this.RequestPool.length] = x;
		return x;
	},
	
	// This method is to be called from server-generated methods. Those methods are called in one
	// of the following formats:
	// 1) ModuleName.MethodName(arg1, ..., argN[, callbackFunction[, context1[..., contextN]]])
	// 2) ModuleName.MethodName(arg1, ..., argN[, [callbackFunction, callAs][, context1[..., contextN]]])
	// The "context" argument is passed back to the callback function.
	// The callback is called in the format: callbackFunction(responseObject, context1, ..., contextN)
	Request : function(moduleName, args) {
		var req = this.CreateRequest();
		var serverArgsCount = 0;
		for(var i=0; i<args.length; i++) {
			var isCBFunc = false;
			var callback, callbackObject;
			if(args[i] instanceof Function) {
				isCBFunc = true;
				callback = args[i];
				callbackObject = null;
			} else if(args[i] instanceof Array) {
				if(args[i].length == 2)
					if(args[i][0] instanceof Function && args[i][1] instanceof Object) {
						isCBFunc = true;
						callback = args[i][0];
						callbackObject = args[i][1];
					}
			}
			if(isCBFunc) {
				var context = null;
				if(i+1 < args.length) {
					context = [];
					for(var k=i+1; k<args.length; k++)
						context[k-(i+1)] = args[k];
				}
				this.ExecuteCall(moduleName, args, callback, callbackObject, serverArgsCount, context);
				return;
			} else { // check the value of each argument
				if(args[i] != null)
					if(args[i].getFullYear) // is date
						args[i] = args[i].toUTCString();
				serverArgsCount++;
			}
		}
		this.ExecuteCall(moduleName, args, this.DefaultCallback, null, serverArgsCount, null);
	},
	
	callNumber : 0,
	
	// This method is used internally by the Request() method.
	ExecuteCall : function(moduleName, serverArgs, callback, callbackObject, serverArgsCount, context) {
		var cnum = this.callNumber++; // keep track of how many ajax calls have been made since the page was loaded
		this.WriteConsole('-&gt;[' + cnum + ']: ' + moduleName); // optionally log the call to a console area on the page
		var req = this.CreateRequest();
		try { req.expired = false; } catch(e) { }
		req.onreadystatechange = function() { // define the callback function
			if(req.readyState == 4) {
				try {
					if(req.expired) return;
					req.expired = true;
				} catch(e) {
				}
				if(!SprocketAjax) return;
				var contextArgs = context ? context : [];
				var callnum = cnum;
				SprocketAjax.WriteConsole('&lt;-[' + cnum + ']: ' + moduleName +
					'<span onclick="this.nextSibling.style.display=\'\';">[Response]</span><div style="display:none">' + req.responseText + '</div>');
				try {
					var response = JSON.parse(req.responseText);
				} catch(ex) {
					if(!req.responseText)
						return;
					alert('There was an error parsing the JSON response. The error was:\n\n' + ex.message + '\n\nThe response text was:\n\n' + req.responseText);
				}
				if(response) {
					if(response.__error) {
						if(response.__error.toLowerCase().indexOf('authentication failed') > -1) {
							if(SprocketAjax.authfailed)
								return;
							SprocketAjax.authfailed = true;
							SprocketAjax.AuthenticationFailed(response.__error);
						} else if(response.__exceptionType.indexOf('AjaxSessionExpiredException') > -1) {
							SprocketAjax.DefaultCallback(response);
							location.reload(true);
							return;
						} else
							SprocketAjax.DefaultCallback(response);
						return;
					}
//					SprocketAjax.LoadTimeStamp = response.__timeStamp;
//					alert('setting new timestamp to ' + response.__timeStamp);
					var call;
					if(callbackObject)
						call = 'callback.call(callbackObject, response.Data, callnum';
					else
						call = 'callback(response.Data, callnum';
					for(var i=0; i<contextArgs.length; i++)
						call += ', contextArgs[' + i + ']';
					call += ');';
					eval(call);
				}
			}
		}
		
		// prepare the request for transmission
		req.open('POST', this.ApplicationRoot + 'AjaxRequest.ajax', true);
		
		// prepare the data object for transmission
		var preparedArgs = []
		for(var i=0; i<serverArgsCount; i++) {
			if(typeof serverArgs[i] == 'function')
				break;
			preparedArgs[i] = serverArgs[i];
		}
		var data = {
			ModuleName : moduleName,
			AuthKey : this.AuthKey,
			MethodArgs : preparedArgs,
			LoadTimeStamp : SprocketAjax.LoadTimeStamp
		}
		
		// send the data asyncronously to the server and return control to calling object
		var sdata = JSON.stringify(data);
		req.send(sdata);
	},
	
	authfailed : false,
	AuthenticationFailed : function(error) {
		alert(error);
	},
	
	// This is the default callback method used when none is specified.
	DefaultCallback : function(response) {
		if(response.__error)
			alert(response.__error.replace(/\\r\\n/g,'\n'));
	},
	
	WriteConsole : function(message) {
		var c = document.getElementById('_AjaxConsole');
		if(c) c.innerHTML += '<div>' + message + '</div>';
	},
	
	LoadingMessage : '<h2>Loading</h2>' //'<div class="content-loading">Loading...</div>'
}
