SecurityInterface = {
	Run : function() {
		var str = '<div id="security-links"></div><div id="security-interface"></div>';
		$('user-admin-container').innerHTML = str;
		this.tabs = new TabStrip();
		this.tabs.AddTab('User Search', 'userfilter', function(){SecurityInterface.LoadUserList()});
		this.tabs.AddTab('Create User', 'createuser', function(){SecurityInterface.NewUser()});
		if(true)//{ifUserCanAccessRoleManagement}
			this.tabs.AddTab('Manage Roles', 'manageroles', function(){SecurityInterface.ManageRoles()});
		$('security-links').appendChild(this.tabs.Create());
		this.LoadUserList();
	},
	
	SetInterface : function (html) {
		document.getElementById('security-interface').innerHTML = html;
	},
	
	LoadUserList : function() {
		this.SetInterface(
			'<div id="security-edituser"></div>' +
			'<div id="security-filter">' +
				'<div class="filteroption"><span class="label">Username: </span><input type="text" size="10" onkeyup="SecurityInterface.RunUserFilter()" id="userlist-filter-username" class="textfield" /></div>' +
				'<div class="filteroption"><span class="label">First Name: </span><input type="text" size="10" onkeyup="SecurityInterface.RunUserFilter()" id="userlist-filter-firstname" class="textfield" /></div>' +
				'<div class="filteroption"><span class="label">Surname: </span><input type="text" size="10" onkeyup="SecurityInterface.RunUserFilter()" id="userlist-filter-surname" class="textfield" /></div>' +
				'<div class="filteroption"><span class="label">Email: </span><input type="text" size="10" onkeyup="SecurityInterface.RunUserFilter()" id="userlist-filter-email" class="textfield" /></div>' +
				'<div id="security-filter-display">Display: ' +
					'<input type="radio" name="userdisplaytype" id="security-filter-showfullnames" onclick="SecurityInterface.RunUserFilter();" checked="true" /> Full Names ' +
					'<input type="radio" name="userdisplaytype" id="security-filter-showusernames" onclick="SecurityInterface.RunUserFilter();" /> Usernames ' +
				'</div>' +
			'</div>' +
			'<div id="security-filter-results"></div>'
		);
		this.RunUserFilter();
	},
	
	defaultMaxFilterMatches : 50,//{defaultMaxFilterMatches} //don't change the 50; it's replaced by Sprocket at run-time.
	RunUserFilter : function(loadAll) {
		var username = $('userlist-filter-username').value;
		var firstname = $('userlist-filter-firstname').value;
		var surname = $('userlist-filter-surname').value;
		var email = $('userlist-filter-email').value;
		var max = loadAll ? 0 : this.defaultMaxFilterMatches;
		Ajax.WebSecurity.FilterUsers(username, firstname, surname, email, max, SecurityInterface.UserFilterCallback);
	},

	filterCallNum : -1,
	UserFilterCallback : function(response, callnum) {
		function MakeLink(userid, name) {
			return '<a href="javascript:void(0)" class="result" onclick="SecurityInterface.SelectUser(\'' +
				userid + '\');">' + name + '</a>';
		}
		// ensure that a lagged result does not override a more recent filter result
		if(callnum < SecurityInterface.filterCallNum) return;
		SecurityInterface.filterCallNum = callnum;
		var matches = response.Matches;
		var users = response.Users;
		var showUsernames = document.getElementById('security-filter-showusernames').checked;
		var html = '<span class="important">Found ' + matches + ' User(s)</span>' +
			(users.length > 0 ? ': ' + MakeLink(users[0].UserID, (showUsernames ? users[0].Username : users[0].Name)) : '.');
		for(var i=1; i<users.length; i++)
			html += '<span class="resultdelimiter"> / </span>' + MakeLink(users[i].UserID, showUsernames ? users[i].Username : users[i].Name);
		if(users.length > 0 && users.length < matches)
			html += ' | <span class="important"><a href="javascript:void(0)" onclick="SecurityInterface.RunUserFilter(true);">Show All ' + matches + ' Users</a></span>';
		document.getElementById('security-filter-results').innerHTML = html;
	},
	
	NewUser : function() {
		$('security-interface').innerHTML = 'Loading...';
		Ajax.WebSecurity.GetUserEditForm(null, SecurityInterface.NewUserCallback);
	},
	
	NewUserCallback : function(formLayout) {
		$('security-interface').innerHTML = formLayout.HTML;
		eval(formLayout.JavaScript);
	},
	
	OnUserSaved : function(id) {
		SecurityInterface.tabs.SelectTab(0);
		//SecurityInterface.LoadUserList();
	},
	
	SelectUser : function(userID) {
		$('security-edituser').innerHTML = 'Loading...';
		Ajax.WebSecurity.GetUserEditForm(userID, SecurityInterface.SelectUserCallback);
	},
	
	SelectUserCallback : function(formLayout) {
		$('security-edituser').innerHTML = formLayout.HTML;
		eval(formLayout.JavaScript);
	},
	
	CancelUserEdit : function() {
		$('security-edituser').innerHTML = '';
	},
	
	DeleteUser : function(userID) {
		if(!confirm('Are you sure you want to delete this user?')) return;
		var buttons = $childrenWithAttribute($('security-edituser'), 'type', 'submit');
		for(var i=0; i<buttons.length; i++) {
			if(buttons[i].value == 'Delete')
				buttons[i].value = 'Deleting...';
			buttons[i].disabled = true;
		}
		Ajax.WebSecurity.DeleteUser(userID, SecurityInterface.DeleteUserCallback);
	},
	
	DeleteUserCallback : function(result) {
		if(!result.Succeeded)
			alert(result.Message);
		Opacity.Fade($('security-edituser').firstChild, 0, 500, 20, SecurityInterface.DeleteUserFaderCallback);
	},
	
	DeleteUserFaderCallback : function() {
		$('security-edituser').innerHTML = '';
		SecurityInterface.RunUserFilter();
	},
	
	ManageRoles : function() {
		this.SetInterface(
			'<ul id="security-rolelist"></ul>' +
			'<div id="security-permissionlist"></div>'
			);
		Ajax.WebSecurity.GetAccessibleRoles(SecurityInterface.RoleListCallback);
	},
	
	RoleListCallback : function(roles) {
		var str = '<li id="security-link-createrole"><a href="javascript:void(0)" onclick="SecurityInterface.SelectRole(null);">Create New Role...</a></li>';
		for(var i=0; i<roles.length; i++)
			str += '<li class="security-link-selectrole">' +
					'<a href="javascript:void(0)" onclick="SecurityInterface.SelectRole(\'' +
					roles[i].RoleID + '\');">' + roles[i].Name + '</a></li>';
		$('security-rolelist').innerHTML = str;
		$('security-permissionlist').innerHTML = '';
	},
	
	SelectRole : function(roleID) {
		$('security-permissionlist').innerHTML = 'Loading...';
		Ajax.WebSecurity.GetRoleEditForm(roleID, SecurityInterface.SelectRoleFormCallback);
	},
	
	SelectRoleFormCallback : function(form) {
		$('security-permissionlist').innerHTML = form.HTML;
		eval(form.JavaScript);
	},
	
	OnRoleSaved : function() {
		SecurityInterface.tabs.SelectTab(2);
	},
	
	DeleteRole : function(roleID) {
		if(!confirm('Are you sure you want to delete this role?')) return;
		var buttons = $childrenWithAttribute($('security-permissionlist'), 'type', 'submit');
		for(var i=0; i<buttons.length; i++) {
			if(buttons[i].value == 'Delete')
				buttons[i].value = 'Deleting...';
			buttons[i].disabled = true;
		}
		Ajax.WebSecurity.DeleteRole(roleID, SecurityInterface.DeleteRoleCallback);
	},
	
	DeleteRoleCallback : function(roleID) {
		SecurityInterface.tabs.SelectTab(2);
	}
}