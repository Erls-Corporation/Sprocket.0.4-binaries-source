function TabStrip() {
	this.tabStripClassName		= 'tabstrip';
	this.tabClassName			= 'tab';
	this.tabLeftClassName		= 'left';
	this.tabRightClassName		= 'right';
	this.tabStartClassName		= 'start';
	this.tabEndClassName		= 'last';
	this.tabSelectedClassName	= 'selected';
	this.tabUnselectedClassName	= 'unselected';
	this.tabTextClassName		= 'text';
	this.finaliserClassName		= 'final';
	
	this.tabs = [];
	this.selectedIndex = 0;
}

TabStrip.prototype.AddTab = function(name, tabid, onclick, pane, selected) {
	var index = this.tabs.length;
	if(typeof(pane) == 'string') pane = document.getElementById(pane);
	var tab = {
		tabid : tabid,
		name : name,
		onclick : onclick,
		index : index,
		pane : pane
	};
	this.tabs[index] = tab;
	if(selected) this.selectedIndex = index;
}

TabStrip.prototype.Create = function() {
	var div = document.createElement('div');
	div.className = this.tabStripClassName;
	
	var refthis = this;
	for(var i=0; i<this.tabs.length; i++) {
		var tab = document.createElement('div');
		tab.index = i;
		tab.onclick = function() { refthis.SelectTab(this.index); }
		if(this.tabs[i].tabid != null) tab.id = this.tabs[i].tabid;
		this.tabs[i].element = tab;
		var text = document.createElement('div');
		text.innerHTML = this.tabs[i].name;
		text.className = this.tabTextClassName;
		tab.appendChild(text);
		div.appendChild(tab);
	}
	var ender = document.createElement('div');
	ender.innerHTML = '&nbsp;';
	ender.className = this.finaliserClassName;
	div.appendChild(ender);
	this.SelectTab(this.selectedIndex, true);
	return div;
}

TabStrip.prototype.SelectTab = function(index, ignoreClickEvent) {
	var side = 'left';
	this.selectedIndex = index;
	for(var i=0; i<this.tabs.length; i++) {
		var tab = this.tabs[i];
		if(tab.pane) {
			if(i == this.selectedIndex)
				tab.pane.style.display = '';
			else
				tab.pane.style.display = 'none';
		}
		if(i == this.selectedIndex) {
			side = 'right';
			tab.element.className = this.tabClassName + ' ' + this.tabSelectedClassName;
		} else if(i == 0)
			tab.element.className = this.tabClassName + ' ' + this.tabStartClassName + ' ' + this.tabUnselectedClassName;
		else if(i == this.tabs.length-1)
			tab.element.className = this.tabClassName + ' ' + this.tabEndClassName + ' ' + this.tabUnselectedClassName;
		else if(side == 'left')
			tab.element.className = this.tabClassName + ' ' + this.tabLeftClassName + ' ' + this.tabUnselectedClassName;
		else
			tab.element.className = this.tabClassName + ' ' + this.tabRightClassName + ' ' + this.tabUnselectedClassName;
	}
	if(this.tabs[index].onclick && !ignoreClickEvent) {
		this.tabs[index].onclick();
	}
}
