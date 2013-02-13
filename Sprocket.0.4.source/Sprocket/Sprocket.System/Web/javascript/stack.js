Stack = {
	frame : function(objRef, funcArgs, funcRef) {
		this.objRef = objRef;
		this.funcArgs = funcArgs;
		this.funcRef = funcRef;
	},
	stackFrames : [],
	Push : function(objRef, funcArgs, funcRef) {
		//alert('stacked');
		this.stackFrames[this.stackFrames.length] = new this.frame(objRef, funcArgs, funcRef);
	},
	Pop : function(arg) {
		//alert('unstacked');
		this.stackFrames.length--;
		return arg;
	},
	RegisterTypes : function() {
		for(var i=0; i<arguments.length; i++) {
			var typeRef = arguments[i];
			var str = typeRef.toString();
			var typeName = str.replace(/\s*function\s+(\w+)\s*\([\s\S]*/,'$1');
			var pushCode = '\nStack.Push(this, arguments, ' + typeName + '.prototype.';
			var popCode = '\nStack.Pop();\n';
			var proto = typeRef.prototype;
			var evals = [];
			for(var p in new typeRef) {
				var pf = eval(typeName + '.prototype.' + p);
				if(pf instanceof Function) {
					var redef = typeName + '.prototype.' + p + ' = function' + this.getArgDef(pf)
							  + ' {' + pushCode + p + ');\n' + this.getFuncBody(pf) + popCode + '}';
					evals[evals.length] = redef;
				}
			}
			var pushCode = '\nStack.Push(this, arguments, ' + typeName + '.prototype.constructor);\n';
			eval(typeName + ' = function' + this.getArgDef(typeRef) + ' {' + pushCode + this.getFuncBody(typeRef) + popCode + '}');
			eval(typeName).prototype = proto;
			for(var i=0; i<evals.length; i++)
			{
				//alert(evals[i]);
				eval(evals[i]);
			}
		}
		//alert('class ' + typeName + argDef + ' {\n' + funcBody + '\n}');
	},
	RegisterFunctions : function() {
		for(var i=0; i<arguments.length; i++) {
			var typeRef = eval(arguments[i]);
			
		}
	},
	getArgDef : function(typeRef) {
		return typeRef.toString().replace(/\s*function\s+\w*\s*(\([^\{]+)[\s\S]*/,'$1');
	},
	getFuncBody : function(typeRef) {
		var str = typeRef.toString().replace(/\s*function\s+\w*\s*\([^{]+\{\s*([\s\S]*)\}/,'$1');
		return str.replace(/return\s*([^;]*);/g, 'return Stack.Pop($1);');
	}
}

function x(j, k) {
	this.j = j;
	this.k = k;
}

x.prototype.add = function(a, b) {
	switch(a) {
		case 1: return 99; break;
		case 2:
			return 12345678;
		default: break;
	}
	return a + b;
}

function r() {
	
}

Stack.RegisterTypes(x, r);

var z = new x(1,2);
//alert(z.add(5,6));