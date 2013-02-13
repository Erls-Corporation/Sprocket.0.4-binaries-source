/*
	Splitter Window Control
	-----------------------------------
	Designed and Coded by Nathan Ridley
	© 2005 ERSYSGroup : www.ersysgroup.com
	-----------------------------------

	USAGE:
	You can create a splitter window two ways:
	1) Programatically. Build all of your windows on the fly with javascript code.
	2) From HTML. Write HTML that defines your windows and panes and then point the splitter window control at the HTML.
	
*/

//-----------------------------------------------------------------------------------------------------------------------
// Static Class: NRSplitterWindowEngine
// Global master object (static class) for all splitter windows.

NRSplitterWindowEngine = {
	Register : function(element) {
		if(typeof(element) == 'string') element = document.getElementById(element);
		var sw = new NRSplitterWindow();
		sw.CreateFrom(element);
	},
	GetCurrentPane : function(element) {
		while(element) {
			if(element.pane) return element.pane;
			element = element.parentNode;
		}
		return null;
	},
	DefaultPaneCssClass : 'pane',
	DefaultResizerCssClass : 'resizer'
}

//-----------------------------------------------------------------------------------------------------------------------
// Class: NRSplitterWindow
// Container class for a splitter window object. Required to create and initialise a new splitter window.

function NRSplitterWindow() {
	this.panes = [];
	this.gapSize = 0;
	this.resizerSize = 0; //set to 0 to disable
	this.resizerPosition = 'center'; //start|center|end - position within gap to place the resizer bar
	this.reducePaneWidthBy = 0; // change this value to cater for css padding and border size
	this.reducePaneHeightBy = 0; // change this value to cater for css padding and border size
}

// Initialises the splitter window from an existing html element.
NRSplitterWindow.prototype.CreateFrom = function(element) {
	// assign initial values from custom html attributes
	this.isHorizontal = (element.getAttribute('orientation').toLowerCase() != 'vertical');
	this.reducePaneWidthBy = element.getAttribute('reducePaneWidthBy') ? parseInt(element.getAttribute('reducePaneWidthBy')) : this.reducePaneWidthBy;
	this.reducePaneHeightBy = element.getAttribute('reducePaneHeightBy') ? parseInt(element.getAttribute('reducePaneHeightBy')) : this.reducePaneHeightBy;
	this.gapSize = element.getAttribute('gapSize') ? parseInt(element.getAttribute('gapSize')) : this.gapSize;
	this.resizerSize = element.getAttribute('resizerSize') ? parseInt(element.getAttribute('resizerSize')) : this.resizerSize;
	this.resizerPosition = element.getAttribute('resizerPosition') ? element.getAttribute('resizerPosition') : this.resizerPosition;
	element.splitterWindow = this;

	// each child element is a pane; iterate through each and load its properties
	for(var i=0; i<element.childNodes.length; i++) {
		var child = element.childNodes[i];
		if(child.nodeType != 1) continue; // make sure the node is an element, and not plain text or something like that
		var pane = new NRSplitterWindowPane(this); // create the new pane object
		pane.CreateFrom(child); // initialise the pane using custom attributes on the html tag
		this.panes[this.panes.length] = pane; // add the new pane to the collection for this splitter window
	}
	this.InitialiseSplitterElement(element);
	this.AssignPaneIndices();
	this.InitialisePaneLayouts();
	this.InitialiseAllPaneStyles();
	this.CalculatePaneSizes();
	this.UpdateDisplay();
}

// Initialises the splitter window from scratch inside a specified element
NRSplitterWindow.prototype.CreateIn = function(element) {
	// to be completed
	element.splitterWindow = this;
	for(var i=0; i<this.panes.length; i++)
		this.panes[i].Create();
	this.InitialiseSplitterElement(element);
}

// Initialises the main splitter window pane within the browser DOM. Used internally - don't call.
NRSplitterWindow.prototype.InitialiseSplitterElement = function(element) {
	this.element = element;
	this.element.style.overflow = 'hidden'; // allows resize to fire
	this.element.style.width = '100%';
	this.element.style.height = '100%';
	this.element.style.position = 'absolute';
	// attach the event handler so that the splitter window resizes properly when the parent element is resized
	this.element.onresize = function(e) { this.splitterWindow.OnResize(e); }
	this.element.onmousemove = function(e) { this.splitterWindow.OnMouseMove(e); }
	this.element.onmouseup = function(e) { this.splitterWindow.OnMouseUp(e); }
	FirefoxResizeEventHandler.RegisterElement(element);
}

// this method calculates the "virtual" sizes of each pane. The sizes calculated here are used afterwards
// as a basis for calculating the actual pixel sizes of the panes. These sizes are relevant regardless of
// the parent splitter window size.
NRSplitterWindow.prototype.InitialisePaneLayouts = function(panes, preventHide) {
	if(!panes) panes = this.panes;
	if(panes.length == 0) return; // if we have no panes, there's nothing to do at this point :P
	var arr = []; // holds the indices of panes without sizes
	var foundNullSize = false; // we must find at least one null size. if none are found, the last pane is forced to null size.

	var lastVisibleIndex = -1;
	for(var i=0; i<panes.length; i++) { // find panes with no specified original size
		var pane = panes[i];
		pane.dynamicSize = null; // reset to null to ensure that panes with changed sizes are sized correctly

		if(pane.hidden) { // hide or show the pane depending on its 'hidden' status
			if(!preventHide) pane.element.style.display = 'none';
			continue;
		}
		lastVisibleIndex = i;
		pane.element.style.display = 'block';
		
		if(!pane.size || (i == panes.length-1 && !foundNullSize))
		{ // if the current pane has no valid specified size or we're at the last pane and still haven't found a null size
			pane.size = null; // for the forced-to-null pane or panes with empty strings and zeros for the size
			foundNullSize = true;
			if(lastVisibleIndex > -1)
				arr[arr.length] = lastVisibleIndex; // store the pane index and increment the count
		}
	}
	if(!foundNullSize && lastVisibleIndex > -1)
		arr[0] = lastVisibleIndex;
	
	if(arr.length == 0) return;
	// all panes with no specified size need to take up an equal percentage of space remaining after other
	// panes have used up a certain amount of screen space. [pane].dynamicSize represents the size percentage
	var chunksize = Math.floor(100 / arr.length); // what % of remaining space should each non-sized pane be?
	var remainder = 100 % arr.length; // the last pane might need to be a bit bigger if we can't divide perfectly into 100
	panes[arr[arr.length-1]].dynamicSize = chunksize + remainder; // set the dynamic size of the last pane
	for(var i=arr.length-2; i>=0; i--) // split the divided up remaining space amongst the other unsized panes
		panes[arr[i]].dynamicSize = chunksize;
}

// Initialises default styles for panes in relation the orientation of the main splitter window.
// This function should be called when dynamically changing the orientation of the splitter window at runtime.
NRSplitterWindow.prototype.InitialiseAllPaneStyles = function(panes) {
	if(!panes) panes = this.panes;
	for(var i=0; i<panes.length; i++)
		this.InitialisePaneStyles(panes[i]);
}

NRSplitterWindow.prototype.InitialisePaneStyles = function(pane) {
	pane.element.style.position = 'absolute';
	pane.element.style.overflow = 'auto';
	pane.element.style.display = 'block';
	pane.element.className = pane.cssClass;
	pane.element.style.zIndex = '1';
	if(this.isHorizontal) {
		pane.element.style.top = 0;
	} else {
		pane.element.style.left = 0;
	}
}

// this method calculates the actual pixel size of each pane. panes are allocated size in the following order:
// 1. specified percentages are calculated on full window size
// 2. specified pixel sizes are added
// 3. remaining panes have their size calculated from final remaining space
NRSplitterWindow.prototype.CalculatePaneSizes = function(panes) {
	if(!panes) panes = this.panes;
	if(panes.length == -1) return;
	
	var fullsize = this.isHorizontal ? this.element.clientWidth : this.element.clientHeight;
	var pixelcount = 0; // counts how much of the window space we've used
	var visiblepanes = 0;
	var arr = []; // stores the indices of null-sized panes

	// the following loop calculates the pixel sizes of all the panes that have a specified size.
	// any pane that has no specified size is noted so that we can divide up remaining space afterwards.
	for(var i=0; i<panes.length; i++) {
		var pane = panes[i];
		if(pane.hidden) continue;
		visiblepanes++;

		// check if we're using a dynamic size
		if(pane.dynamicSize) {
			arr[arr.length] = i; // record this pane in the list of null-sized panes
			continue; // go to the next pane for now
		}
		
		if(isNaN(pane.size)) { // if this is a percentage e.g. '35%' (assume non-numeric values are percentages)
			// get the number of pixels represented by this percentage
			pane.currentSize = Math.floor((parseInt(pane.size.substr(0,pane.size.length-1))/100) * fullsize);
			pixelcount += pane.currentSize; // add those pixels to the preset pixels count
		} else {
			pane.currentSize = (pane.size*1)
			pixelcount += pane.currentSize;
		}
	}
	
	// divide the remaining window pixels amongst the panes with no original size
	if(arr.length > 0) {
		// remaining space needs to exclude space for the gaps dividing the panes
		var remainingspace = Math.max(fullsize - pixelcount - (this.gapSize*(visiblepanes-1)), 0);
		var usedpixels = 0; // counts how much of [remainingspace] we've used
		for(var i=0; i<arr.length-1; i++) {
			var pane = panes[arr[i]];
			pane.currentSize = Math.floor(remainingspace * pane.dynamicSize / 100);
			usedpixels += pane.currentSize;
		}
		panes[arr[arr.length-1]].currentSize = remainingspace - usedpixels; // the last pane gets however many pixels remain
	}
}

NRSplitterWindow.prototype.UpdateDisplay = function(panes, preserveResizers) {
	// now that we've calculated actual pixel sizes for every pane, set the element sizes and positions accordingly
	if(!panes) panes = this.panes;
	var c = 0;
	for(var i=0; i<panes.length; i++) {
		var pane = panes[i];
		var ps = pane.element.style;
		pane.currentPixelOffset = c;
		if(this.isHorizontal) {
			ps.height = Math.max(this.element.clientHeight - pane.reducePaneHeightBy,0) + 'px';
			ps.width = Math.max(pane.currentSize - pane.reducePaneWidthBy,0) + 'px';
			ps.left = c + 'px';
		} else {
			ps.width = Math.max(this.element.clientWidth - pane.reducePaneWidthBy,0) + 'px';
			ps.height = Math.max(pane.currentSize - pane.reducePaneHeightBy,0) + 'px';
			ps.top = c + 'px';
		}
		if(pane.hidden) continue; // hidden panes are skipped
		c += pane.currentSize + this.gapSize;
		if(!preserveResizers) pane.InitialiseResizer();
	}
}

NRSplitterWindow.prototype.SetOrientation = function(newOrientation) {
	this.isHorizontal = newOrientation.toLowerCase() != 'vertical';
	this.InitialiseAllPaneStyles();
	this.CalculatePaneSizes();
	this.UpdateDisplay();
}

NRSplitterWindow.prototype.AssignPaneIndices = function(panes) {
	if(!panes) panes = this.panes;
	for(var i=0; i<panes.length; i++) {
		panes[i].index = i;
		panes[i].element.pane = panes[i];
	}
}

NRSplitterWindow.prototype.CopyPanes = function() {
	var panes = [];
	for(var i=0; i<this.panes.length; i++)
		panes[i] = this.panes[i].Duplicate();
	return panes;
}

NRSplitterWindow.prototype.InsertPane = function(pane, index, useEffect, callback) {
	pane.Create();
	pane.InitialiseStyles();
	this.panes.splice(index, 0, pane);
	this.AssignPaneIndices();
	if(useEffect) {
		pane.hidden = true;
		this.InitialisePaneLayouts();
		this.CalculatePaneSizes();
		this.UpdateDisplay();
		this.HidePane(index);
		Opacity.Set(this.panes[index].element, 0);
		this.RevealPane(index, true, callback);
	} else {
		this.InitialisePaneLayouts();
		this.CalculatePaneSizes();
		this.UpdateDisplay();
	}
}

NRSplitterWindow.prototype.RemovePane = function(index, useEffect, callback) {
	var refthis = this;
	this.HidePane(index, useEffect, function() {
		var pane = refthis.panes[index];
		pane.element.parentNode.removeChild(pane.element);
		pane.element = null;
		refthis.panes.splice(index, 1);
		for(var i=0; i<refthis.panes.length; i++)
			refthis.panes[i].index = i; //reassign indices
		if(callback) callback();
	});
}

NRSplitterWindow.prototype.RevealPane = function(index, useEffect, callback) {
	if(!this.panes[index].hidden) return;
	if(!useEffect) {
		this.panes[index].hidden = false;
		this.panes[index].element.style.display = 'block';
		Opacity.Set(this.panes[index].element, 100);
		this.InitialisePaneLayouts();
		this.CalculatePaneSizes();
		this.UpdateDisplay();
		return;
	}
	
	this.panes[index].hidden = false;
	var newPanes = this.CopyPanes();
	newPanes[index].hidden = false;
	newPanes[index].element.style.display = 'block';
	this.InitialisePaneLayouts(newPanes);
	this.CalculatePaneSizes(newPanes);

	Opacity.Fade(this.panes[index].element, 100, 300, 20);							 
	var refthis = this;
	this.SlideTo(newPanes, 300, 20, callback);
}

NRSplitterWindow.prototype.HidePane = function(index, useEffect, callback) {
	if(this.panes[index].hidden) return;
	if(useEffect) {
		var newPanes = this.CopyPanes();
		newPanes[index].hidden = true;
		this.InitialisePaneLayouts(newPanes, true);
		this.CalculatePaneSizes(newPanes);
		this.SlideTo(newPanes, 300, 20, callback);
		Opacity.Fade(this.panes[index].element, 0, 300, 20);
	} else {
		this.panes[index].hidden = true;
		this.InitialisePaneLayouts();
		this.CalculatePaneSizes();
		this.UpdateDisplay();
		Opacity.Set(this.panes[index].element, 0);
		if(callback) callback();
	}
}

NRSplitterWindow.prototype.SlideTo = function(newPanes, timespan, interval, callback, /* internal --> */ startTime, oldPanes) {
	if(!startTime) {
		if(!interval) interval = 20;
		startTime = new Date();
		oldPanes = this.CopyPanes();
		
		var offset = 0;
		for(var i=0; i<newPanes.length; i++) {
			
			newPanes[i].element.style.zIndex = newPanes[i].hidden || Opacity.Get(newPanes[i].element) < 100 ? 2 : 3;
			if(this.panes[i].resizer) this.panes[i].resizer.Remove(); // remove the resizer control
			if(newPanes[i].hidden || oldPanes[i].hidden) continue;
			newPanes[i].currentPixelOffset = offset;
			offset += newPanes[i].currentSize + this.gapSize;
		}
	}
	var elapsed = new Date().getTime() - startTime.getTime();
	var perc = elapsed / timespan;

	if(perc >= 1) {
		this.panes = newPanes;
		this.AssignPaneIndices(); // this line is required to ensure that the element.pane references are updated with the new panes
		this.InitialisePaneLayouts();
		this.CalculatePaneSizes();
		this.UpdateDisplay();
		if(callback) setTimeout(callback, 1);
		return;
	}
	
	for(var i=0; i<newPanes.length; i++) {

		var oldOffset = oldPanes[i].currentPixelOffset; // get the original pixel offset of the pane
		var oldSize = oldPanes[i].currentSize;
		var offset = oldOffset + Math.round((newPanes[i].currentPixelOffset - oldOffset) * perc); // work out the offset position based on how far we are through the animation
		var size = oldSize + Math.round((newPanes[i].currentSize - oldSize) * perc);
		var e = this.panes[i].element; // get the element
		if(this.isHorizontal) {
			e.style.left = offset + 'px';
			e.style.width = Math.max(size - this.panes[i].reducePaneWidthBy,1) + 'px';
		} else {
			e.style.top = offset + 'px';
			e.style.height = Math.max(size - this.panes[i].reducePaneHeightBy,1) + 'px';
		}
	}

	var refthis = this;
	
	setTimeout(function() { refthis.SlideTo(newPanes, timespan, interval, callback, startTime, oldPanes) }, interval);
}

NRSplitterWindow.prototype.OnMouseUp = function(e) {
}

NRSplitterWindow.prototype.OnResize = function(e) {
	this.CalculatePaneSizes();
	this.UpdateDisplay();
}

NRSplitterWindow.prototype.OnMouseMove = function(e) {
	if(this.draggingResizer) {
		var c, t;
		if(e) {
			c = { x : e.layerX, y : e.layerY };
			t = e.target;
			e.stopPropagation();
		} else {
			e = window.event;
			c = { x : e.offsetX, y : e.offsetY };
			t = e.srcElement;
			e.cancelBubble = true;
		}
		while(t != this.element && t.parentNode) {
			c.x += t.offsetLeft;
			c.y += t.offsetTop;
			t = t.parentNode;
		}
		this.resizerRef.OnMouseMove(c.x, c.y);
	}
}

NRSplitterWindow.prototype.OnMouseUp = function(e) {
	if(!this.draggingResizer) return;
	this.resizerRef.OnMouseUp();
}

//-----------------------------------------------------------------------------------------------------------------------
// Class: NRSplitterWindowPane
// Represents one pane within a splitter window. Each NRSplitterWindow
// object must contain at least one instance of an NRSplitterWindowPane
// in order to function correctly.

function NRSplitterWindowPane(swParent) {
	this.swParent = swParent; // [internal] the parent splitter window
	this.element = null; // [internal] reference to the html element that holding the pane's contents
	this.size = null; // stores original specified size only
	this.dynamicSize = null; // [internal] used for dynamic sizing of panes that have no specified width. stored as % of remaining space
	this.currentSize = null; // [internal] stores the current size of the pane in pixels. this is updated internally as required.
	this.currentPixelOffset = null; // [internal] stores the pane's current pixel offset from the start of the splitter window
	this.currentMinSize = null; // the minimum size of the pane in pixels, relevant for when the minimum size is a percentage
	this.currentMaxSize = null; // the maximum size of the pane in pixels, relevant for when the maximum size is a percentage

	this.cssClass = NRSplitterWindowEngine.DefaultPaneCssClass;
	this.reducePaneWidthBy = swParent.reducePaneWidthBy; // change this value to cater for css padding and border size
	this.reducePaneHeightBy = swParent.reducePaneHeightBy; // change this value to cater for css padding and border size
	this.hidden = false; // if true, the pane and accompanying divider are completely ignored
	this.minSize = 5; // minimum allowable size of the pane. ignored if size is not specified
	this.maxSize = null; // maximum allowable size of the pane. ignored if size is not specified
	this.resizable = true; // if true, the pane's size is locked and resizing will not affect it
	
	this.initialContent  = null; // only used if created with the Create() method
	
	this.randomid = new Date();
}

// CreateFrom() - Creates a splitter window pane from the supplied html element. Used internally - don't call.
NRSplitterWindowPane.prototype.CreateFrom = function(element) {
	this.size = element.getAttribute('size');
	this.minSize = element.getAttribute('minsize');
	this.maxSize = element.getAttribute('maxsize');
	this.reducePaneWidthBy = element.getAttribute('reducePaneWidthBy') ? parseInt(element.getAttribute('reducePaneWidthBy')) : this.reducePaneWidthBy;
	this.reducePaneHeightBy = element.getAttribute('reducePaneHeightBy') ? parseInt(element.getAttribute('reducePaneHeightBy')) : this.reducePaneHeightBy;
	this.cssClass = element.className ? element.className : this.cssClass;
	this.element = element;
	this.element.pane = this;
	this.resizer = null;
	this.InitialiseStyles();
}

// Create()
NRSplitterWindowPane.prototype.Create = function() {
	if(this.element) return;
	var div = document.createElement('div');
	if(this.initialContent) {
		div.innerHTML = this.initialContent;
		this.initialContent = null;
	}
	this.element = div;
	this.swParent.element.appendChild(div);
	div.pane = this;
	this.InitialiseStyles();
}

// InitialiseResizer()
NRSplitterWindowPane.prototype.InitialiseResizer = function() {
	if(this.resizer) {
		this.resizer.Remove();
		this.resizer = null;
	}
	if(!this.element.previousSibling) return;
	this.resizer = new NRSplitterWindowResizer(this);
	this.resizer.Initialise();
}

// Duplicate()
NRSplitterWindowPane.prototype.Duplicate = function() {
	var pane = new NRSplitterWindowPane(this.swParent);
	pane.swParent = this.swParent;
	pane.element = this.element;
	pane.size = this.size;
	pane.dynamicSize = this.dynamicSize;
	pane.currentSize = this.currentSize;
	pane.currentPixelOffset = this.currentPixelOffset;
	pane.cssClass = this.cssClass;
	pane.reducePaneWidthBy = this.reducePaneWidthBy;
	pane.reducePaneHeightBy = this.reducePaneHeightBy;
	pane.hidden = this.hidden;
	pane.minSize = this.minSize;
	pane.maxSize = this.maxSize;
	pane.resizable = this.maxSize;
	pane.index = this.index;
	pane.randomid = this.randomid;
	return pane;
}

// Hide()
NRSplitterWindowPane.prototype.Hide = function(useEffect, callback) {
	this.swParent.HidePane(this.index, useEffect, callback);
}

// Reveal()
NRSplitterWindowPane.prototype.Reveal = function(useEffect, callback) {
	this.swParent.RevealPane(this.index, useEffect, callback);
}

// Remove()
NRSplitterWindowPane.prototype.Remove = function(useEffect, callback) {
	this.swParent.RemovePane(this.index, useEffect, callback);
}

// Resize()
NRSplitterWindowPane.prototype.Resize = function(newSize, useEffect, callback) {
	this.swParent.ResizePane(this.index, useEffect, callback);
}

// PrependPane()
NRSplitterWindowPane.prototype.PrependPane = function(pane, useEffect, callback) {
	var refthis = this;
	var cb = function() {
	}
	this.swParent.InsertPane(pane, this.index, useEffect, cb);
}

// AppendPane();
NRSplitterWindowPane.prototype.AppendPane = function(pane, useEffect, callback) {
	var refthis = this;
	var cb = function() {
//		refthis.element.appendChild(document.createTextNode('[me?]'));
//		for(var i=0; i<refthis.swParent.panes.length; i++)
//			refthis.swParent.panes[i].element.appendChild(document.createTextNode('[pane ' + i + ',index:' + refthis.swParent.panes[i].index + '/' + refthis.index + ']'));
	}
	this.swParent.InsertPane(pane, this.index+1, useEffect, cb); //function() {alert('my index is now ' + refthis.index);}
}

// toString()
NRSplitterWindowPane.prototype.toString = function() {
	return 'pane ' + this.index; //'(' + this.index + ':' + this.currentPixelOffset + ',' + this.currentSize + ',' + this.dynamicSize + ')';
}

// InitialiseStyles()
NRSplitterWindowPane.prototype.InitialiseStyles = function() {
	this.element.style.position = 'absolute';
	this.element.style.overflow = 'auto';
	this.element.style.display = 'block';
	this.element.className = this.cssClass;
	this.element.style.zIndex = '1';
	if(this.swParent.isHorizontal) {
		this.element.style.top = 0;
	} else {
		this.element.style.left = 0;
	}
}

//-----------------------------------------------------------------------------------------------------------------------
// Class: NRSplitterWindowResizer
// Represents the resizer control that sits between two panes. The control is operated the same way a normal
// windows explorer splitter control would operate, via click and drag.

function NRSplitterWindowResizer(pane) {
	this.pane = pane;
	this.element = null;
}

NRSplitterWindowResizer.prototype.Initialise = function() {
	var sw = this.pane.swParent;
	if(!sw.resizerSize) return;

	this.element = document.createElement('div');
	this.element.style.position = 'absolute';
	this.element.style.zIndex = '5'; // make sure resizer bars sit above the panes when overlapping occurs
	this.element.style.overflow = 'hidden';
	this.element.className = NRSplitterWindowEngine.DefaultResizerCssClass;
	this.element.unselectable = 'on';
	this.element.onselectstart = function() { return false; }
	this.element.resizer = this;
	this.element.onmousedown = function(e) { this.resizer.OnMouseDown(e); }
	this.element.onmouseup = function(e) { this.resizer.OnMouseUp(e); }
	this.element.dragging = false;
	
	var pos = this.pane.currentPixelOffset;
	// calculate where in preceding gap the resizer should sit
	switch(sw.resizerPosition) {
		case 'start': pos -= sw.gapSize; break;
		case 'end': pos -= sw.resizerSize; break;
		default:
			if(sw.gapSize == sw.resizerSize)
				pos -= sw.gapSize;
			else
				pos -= Math.floor(sw.gapSize/2) + Math.floor(sw.resizerSize/2);
			break;
	}
	if(sw.isHorizontal) {
		this.element.style.top = '0';
		this.element.style.left = pos + 'px';
		this.leftpos = pos;
		this.toppos = 0;
		this.element.style.width = this.pane.swParent.resizerSize + 'px';
		this.element.style.height = '100%';
		this.element.style.cursor = 'e-resize';
	} else {
		this.element.style.left = '0';
		this.element.style.top = pos + 'px';
		this.leftpos = 0;
		this.toppos = pos;
		this.element.style.height = this.pane.swParent.resizerSize + 'px';
		this.element.style.width = '100%';
		this.element.style.cursor = 'n-resize';
	}
	sw.element.appendChild(this.element);
	Opacity.Set(this.element, 0);
}

// calculates the minimum number of pixels from the start of the splitter window that the resizer should be allowed to move to
NRSplitterWindowResizer.prototype.GetMinOffset = function() {
	var clientsize = this.pane.swParent.isHorizontal ? this.pane.swParent.element.clientWidth : this.pane.swParent.element.clientHeight;
	var tmp = this.pane.minSize.toString();
	if(tmp.substr(tmp.length-1, 1) == '%')
		this.pane.currentMinSize = Math.round(clientsize * parseInt(tmp.substr(0, tmp.length-1)) / 100);
	else if(!isNaN(tmp))
		this.pane.currentMinSize = parseInt(tmp);
	else
		this.pane.currentMinSize = 1;
	return this.pane.currentPixelOffset + this.pane.currentMinSize;
	/*
	for(var i=this.pane.index; i>=0; i--) {
		var pane = this.pane.swParent.panes[i];
		if(pane.hidden) continue;
		if(pane.minSize) {
			var tmp = pane.minSize.toString();
			if(tmp.substr(tmp.length-1, 1) == '%') {
				pane.currentMinSize = Math.round(clientsize * parseInt(tmp.substr(0, tmp.length-1)) / 100);
			} else if(!isNaN(tmp))
				pane.currentMinSize = parseInt(tmp);
			else
				pane.currentMinSize = 1;
		} else
			pane.currentMinSize = 1;
		minoffset += pane.currentMinSize;
	}
	minoffset += this.pane.index * this.pane.swParent.gapSize;
	return minoffset;
	*/
}

NRSplitterWindowResizer.prototype.GetMaxOffset = function() {
	var clientsize = this.pane.swParent.isHorizontal ? this.pane.swParent.element.clientWidth : this.pane.swParent.element.clientHeight;
	var tmp = this.pane.minSize.toString();
	if(tmp.substr(tmp.length-1, 1) == '%')
		this.pane.currentMinSize = Math.round(clientsize * parseInt(tmp.substr(0, tmp.length-1)) / 100);
	else if(!isNaN(tmp))
		this.pane.currentMinSize = parseInt(tmp);
	else
		this.pane.currentMinSize = 1;
	return this.pane.currentPixelOffset + this.pane.currentMinSize;
	

	var maxoffset = 0;
	var clientsize = this.pane.swParent.isHorizontal ? this.pane.swParent.element.clientWidth : this.pane.swParent.element.clientHeight;
	for(var i=this.pane.index; i<this.pane.swParent.panes.length; i++) {
		var pane = this.pane.swParent.panes[i];
		if(pane.maxSize) {
			var tmp = pane.minSize.toString();
			if(tmp.substr(tmp.length-1, 1) == '%') {
				pane.currentMaxSize = Math.round(clientsize * parseInt(tmp.substr(0, tmp.length-1)) / 100);
			} else if(!isNaN(tmp))
				pane.currentMaxSize = parseInt(tmp);
			else
				pane.currentMaxSize = 1;
		} else
			pane.currentMaxSize = 1;
		minoffset += pane.currentMinSize;
	}
	minoffset += this.pane.index * this.pane.swParent.gapSize;
	return minoffset;
}

NRSplitterWindowResizer.prototype.OnMouseDown = function(e) {
	Opacity.Set(this.element, 50);
	this.dragging = true;
	this.pane.swParent.draggingResizer = true;
	this.pane.swParent.resizerRef = this;
	this.minOffset = this.GetMinOffset();
}

NRSplitterWindowResizer.prototype.OnMouseUp = function(e) {
	if(!this.dragging) return;
	Opacity.Set(this.element, 0);
	this.dragging = false;
	this.pane.swParent.draggingResizer = false;
	this.pane.swParent.resizerRef = null;
	// to do: (1) prevent the resizer from moving past the minimum position and (2) resize panes on mouseup
}

NRSplitterWindowResizer.prototype.OnMouseMove = function(x, y) {
	var sw = this.pane.swParent;
	if(sw.isHorizontal) {
		this.element.style.left = Math.max(x - Math.floor(sw.gapSize/2), this.minOffset) + 'px';
	} else {
		this.element.style.top = Math.max(y - Math.floor(sw.gapSize/2), this.minOffset) + 'px';
	}
}

NRSplitterWindowResizer.prototype.Remove = function() {
	if(this.element) {
		if(this.element.parentNode)
			this.element.parentNode.removeChild(this.element);
		this.element = null;
	}
}
