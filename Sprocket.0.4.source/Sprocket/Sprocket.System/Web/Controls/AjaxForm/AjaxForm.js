AjaxFormValidator = {
	SetGroupVisibility:			function(containerElement, valueElement, visibleByDefault /*, validationExceptions... */) {
									var ve = $(valueElement); var ce = $(containerElement);
									
								},
	SetGroupEnabled:			function(containerElement, valueElement, visibleByDefault /*, validationExceptions... */) {
									
								},
	AddFieldValidationCheck:	function(valueElement, errorMsgElement, validateOnServer, clientValidationFunction) {
									var ve = $(valueElement);
									
									var validate;
									if(validateOnServer && clientValidationFunction) validate = 'both';
									else if(validateOnServer) validate = 'server';
									else if(clientValidationFunction) validate = 'client';
									else {
										ve.setAttribute('validatetype', null);
										return;
									}
									ve.setAttribute('validatetype', validate);

									if(!clientValidationFunction) return;

									var tag = ve.tagName.toLowerCase();
									if(!$in(tag, 'input', 'textarea', 'select')) return;
									if(tag == 'input') if(!$in(ve.type.toLowerCase(), 'text', 'password', 'checkbox')) return;

									if(!ve.GetInputValue) {
										if(tag == 'select')
											ve.GetInputValue = function() { return this.options[this.selectedIndex].value; };
										else if(ve.type == 'checkbox')
											ve.GetInputValue = function() { return this.checked; };
										else
											ve.GetInputValue = function() { return this.value; };
										ve.onblur = AjaxFormValidator.ValidateField;
										ve.validate = AjaxFormValidator.ValidateField;
										ve.validations = [];
									}
									ve.validations[ve.validations.length] = {
										clientValidationFunction : clientValidationFunction,
										errorMsgElement : $(errorMsgElement)
									}
								},
	ValidateField:				function(skipServerValidation) {
									if(skipServerValidation != true) skipServerValidation = false;
									var b;
									var value = this.GetInputValue();
									for(var i=0; i<this.validations.length; i++) {
										var v = this.validations[i];
										if(v.clientValidationFunction) {
											var msg = v.clientValidationFunction(value);
											if(msg) {
												v.errorMsgElement.innerHTML = msg;
												return false;
											} else
												v.errorMsgElement.innerHTML = '';
										}
										var form = $parentWithAttribute(this, 'formpart', 'form');
										var formname = form.getAttribute('name');
										if($in(this.getAttribute('validatetype'),'server','both') && !skipServerValidation) {
											Ajax.AjaxFormHandler.ValidateField(
												formname, this.name, this.GetInputValue(),
												$nullIfBlank(Browser.IsIE ? form.recordid : form.getAttribute('recordid')),
												AjaxFormValidator.ValidateFieldCallback,
												v.errorMsgElement);
										}
										else if(skipServerValidation)
											return true;
									}
									return true;
								},
	ValidateFieldCallback:		function(response, cbid, errorMsgElement) {
									if(!response.IsValid)
										errorMsgElement.innerHTML = response.Message;
								},
	ValidateForm:				function(element, callbackValidationSuccessFuncRef) {
									var form = $parentWithAttribute(element, 'formpart', 'form');
									var fields = $childrenWithAttribute(form, 'validatetype');
									var valid = true;
									for(var i=0; i<fields.length; i++)
										if((typeof fields[i].validate) == 'function')
											if(!fields[i].validate(true))
												valid = false;
									if(valid) {
										setTimeout(function() {
											element.disabled = true; // disable the button
											element.value = "Saving..."; // and keep the user in the loop :P
										}, 10);
										
										if(form.tagName.toLowerCase() != 'form') { // perform ajax submit if not using a form tag
											var fldvalues = {};
											var blocks = $childrenWithAttribute(form, 'formpart', 'block');
											for(var i=0; i<blocks.length; i++) {
												var block = {};
												var flds = $childrenWithAttribute(blocks[i], 'name');
												if(flds.length > 0) {
													for(var j=0; j<flds.length; j++) {
														if(flds[j].type == 'submit')
															continue;
														if(flds[j].type == 'checkbox')
															block[flds[j].name] = flds[j].checked;
														else if(flds[j].GetInputValue || flds[j].value != undefined)
															block[flds[j].name] = flds[j].GetInputValue ? flds[j].GetInputValue() : flds[j].value;
													}
													fldvalues[blocks[i].getAttribute('name')] = block;
												}
											}
											
											var formobj = {
												RecordID : $nullIfBlank(form.getAttribute('RecordID')),
												Name : form.getAttribute('name'),
												FieldBlocks : fldvalues
											}
											
											Ajax.AjaxFormHandler.Submit(formobj, AjaxFormValidator.AjaxFormSubmitCallback, callbackValidationSuccessFuncRef, element);
											// send to server
											
										}
									}
									return valid;
								},
	AjaxFormSubmitCallback:		function(form, callNum, callbackValidationSuccessFuncRef, button) {
									if(callbackValidationSuccessFuncRef == undefined)
										alert('The supplied callback function which was to be\n' +
											  'executed upon successful validation of the form\n' +
											  'has not yet been defined.');
									button.disabled = false;
									button.value = "Save";
									var hasErrors = false;
									for(var i=0; i<form.Blocks.length; i++)
										for(var j=0; j<form.Blocks[i].Fields.length; j++) {
											var field = form.Blocks[i].Fields[j];
											if(field.ErrorMessage) {
												var err = $('ajaxform-error-' + field.Name);
												if(err) err.innerHTML = field.ErrorMessage;
												hasErrors = true;
											}
										}
									if(!hasErrors && callbackValidationSuccessFuncRef)
										callbackValidationSuccessFuncRef(form.RecordID);
								},
	FindFormElement:			function(element, id) {
									if((id && element.id == id) || (!id && element.tagName.toLowerCase() == 'form')) return element;
									if(!element.parentNode) return null;
									return FindFormElement(element.parentNode);
								}
}
