using System;
using System.Collections.Generic;
using System.Text;

namespace Sprocket.Web.CMS.Security
{
	public class LoginForm
	{
		protected bool useLanguagePlaceholders = false;
		protected bool showForgotPasswordLink = true;
		protected bool showErrorMessage = false;

		protected string loginFormContainerClassName = "loginform-container";
		
		protected string labelContainerClassName = "loginform-label-container";
		protected string fieldContainerClassName = "loginform-field-container";
		protected string preserveLoginFieldClassName = "loginform-preserve-login";
		protected string usernameFieldClassName = "loginform-username";
		protected string passwordFieldClassName = "loginform-password";

		protected string errorRowContainerClassName = "loginform-row-error";
		protected string usernameRowContainerClassName = "loginform-row-username";
		protected string passwordRowContainerClassName = "loginform-row-password";
		protected string preserveLoginRowContainerClassName = "loginform-row-preserve-login";
		protected string buttonRowContainerClassName = "loginform-row-buttons";
		
		protected string loginButtonClassName = "loginform-button";
		protected string forgotPasswordLinkContainerClassName = "loginform-forgotpassword-container";
		protected string forgotPasswordLinkClassName = "loginform-forgotpassword-link";

		protected string usernameLanguagePlaceholder = "{?form-label-username?}";
		protected string passwordLanguagePlaceholder = "{?form-label-password?}";
		protected string preserveLoginLanguagePlaceholder = "{?form-label-preserve-login?}";
		protected string buttonLanguagePlaceholder = "{?form-button-login?}";
		protected string forgotPasswordLanguagePlaceholder = "{?forgot-password-link-text?}";
		protected string errorMessageLanguagePlaceholder = "{?login-error-message-placeholder?}";

		protected string usernameStandardLabel = "Username";
		protected string passwordStandardLabel = "Password";
		protected string preserveLoginStandardLabel = "Keep me logged in";
		protected string buttonStandardText = "Sign In";
		protected string forgotPasswordStandardLinkText = "Forgot your username or password?";
		protected string errorMessageStandardText = "Invalid Username or Password";

		protected string usernameTextFieldName = "LoginUsername";
		protected string passwordTextFieldName = "LoginPassword";
		protected string preserveLoginCheckboxFieldName = "PreserveLogin";

		protected string forgotPasswordLinkURL = "#";

		#region Class Properties

		public bool UseLanguagePlaceholders
		{
			get { return useLanguagePlaceholders; }
			set { useLanguagePlaceholders = value; }
		}

		public bool ShowForgotPasswordLink
		{
			get { return showForgotPasswordLink; }
			set { showForgotPasswordLink = value; }
		}

		public bool ShowErrorMessage
		{
			get { return showErrorMessage; }
			set { showErrorMessage = value; }
		}

		public string LoginFormContainerClassName
		{
			get { return loginFormContainerClassName; }
			set { loginFormContainerClassName = value; }
		}

		public string LabelContainerClassName
		{
			get { return labelContainerClassName; }
			set { labelContainerClassName = value; }
		}

		public string FieldContainerClassName
		{
			get { return fieldContainerClassName; }
			set { fieldContainerClassName = value; }
		}

		public string PreserveLoginFieldClassName
		{
			get { return preserveLoginFieldClassName; }
			set { preserveLoginFieldClassName = value; }
		}

		public string UsernameFieldClassName
		{
			get { return usernameFieldClassName; }
			set { usernameFieldClassName = value; }
		}

		public string PasswordFieldClassName
		{
			get { return passwordFieldClassName; }
			set { passwordFieldClassName = value; }
		}

		public string ErrorRowContainerClassName
		{
			get { return errorRowContainerClassName; }
			set { errorRowContainerClassName = value; }
		}

		public string UsernameRowContainerClassName
		{
			get { return usernameRowContainerClassName; }
			set { usernameRowContainerClassName = value; }
		}

		public string PasswordRowContainerClassName
		{
			get { return passwordRowContainerClassName; }
			set { passwordRowContainerClassName = value; }
		}

		public string PreserveLoginRowContainerClassName
		{
			get { return preserveLoginRowContainerClassName; }
			set { preserveLoginRowContainerClassName = value; }
		}

		public string ButtonRowContainerClassName
		{
			get { return buttonRowContainerClassName; }
			set { buttonRowContainerClassName = value; }
		}

		public string LoginButtonClassName
		{
			get { return loginButtonClassName; }
			set { loginButtonClassName = value; }
		}

		public string ForgotPasswordLinkContainerClassName
		{
			get { return forgotPasswordLinkContainerClassName; }
			set { forgotPasswordLinkContainerClassName = value; }
		}

		public string ForgotPasswordLinkClassName
		{
			get { return forgotPasswordLinkClassName; }
			set { forgotPasswordLinkClassName = value; }
		}

		public string UsernameLanguagePlaceholder
		{
			get { return usernameLanguagePlaceholder; }
			set { usernameLanguagePlaceholder = value; }
		}

		public string PasswordLanguagePlaceholder
		{
			get { return passwordLanguagePlaceholder; }
			set { passwordLanguagePlaceholder = value; }
		}

		public string PreserveLoginLanguagePlaceholder
		{
			get { return preserveLoginLanguagePlaceholder; }
			set { preserveLoginLanguagePlaceholder = value; }
		}

		public string ButtonLanguagePlaceholder
		{
			get { return buttonLanguagePlaceholder; }
			set { buttonLanguagePlaceholder = value; }
		}

		public string ForgotPasswordLanguagePlaceholder
		{
			get { return forgotPasswordLanguagePlaceholder; }
			set { forgotPasswordLanguagePlaceholder = value; }
		}

		public string ErrorMessageLanguagePlaceholder
		{
			get { return errorMessageLanguagePlaceholder; }
			set { errorMessageLanguagePlaceholder = value; }
		}

		public string UsernameStandardLabel
		{
			get { return usernameStandardLabel; }
			set { usernameStandardLabel = value; }
		}

		public string PasswordStandardLabel
		{
			get { return passwordStandardLabel; }
			set { passwordStandardLabel = value; }
		}

		public string PreserveLoginStandardLabel
		{
			get { return preserveLoginStandardLabel; }
			set { preserveLoginStandardLabel = value; }
		}

		public string ButtonStandardText
		{
			get { return buttonStandardText; }
			set { buttonStandardText = value; }
		}

		public string ForgotPasswordStandardLinkText
		{
			get { return forgotPasswordStandardLinkText; }
			set { forgotPasswordStandardLinkText = value; }
		}

		public string ErrorMessageStandardText
		{
			get { return errorMessageStandardText; }
			set { errorMessageStandardText = value; }
		}

		public string UsernameTextFieldName
		{
			get { return usernameTextFieldName; }
			set { usernameTextFieldName = value; }
		}

		public string PasswordTextFieldName
		{
			get { return passwordTextFieldName; }
			set { passwordTextFieldName = value; }
		}

		public string PreserveLoginCheckboxFieldName
		{
			get { return preserveLoginCheckboxFieldName; }
			set { preserveLoginCheckboxFieldName = value; }
		}

		public string ForgotPasswordLinkURL
		{
			get { return forgotPasswordLinkURL; }
			set { forgotPasswordLinkURL = value; }
		}

		#endregion

		public override string ToString()
		{
			string html =
			"<div class=\"" + loginFormContainerClassName + "\">";
			if (showErrorMessage)
			{
				html += "<div class=\"" + errorRowContainerClassName + "\">"
					+ (useLanguagePlaceholders ? errorMessageLanguagePlaceholder : errorMessageStandardText)
					+ "</div>";
			}
			html +=
				"<div class=\"" + usernameRowContainerClassName + "\">"
					+ "<div class=\"" + labelContainerClassName + "\">"
						+ (useLanguagePlaceholders ? usernameLanguagePlaceholder : usernameStandardLabel)
					+ "</div>"
					+ "<div class=\"" + fieldContainerClassName + "\">"
						+ "<input type=\"text\" name=\"" + usernameTextFieldName + "\" class=\"" + usernameFieldClassName + "\" />"
					+ "</div>"
				+ "</div>"
				+ "<div class=\"" + passwordRowContainerClassName + "\">"
					+ "<div class=\"" + labelContainerClassName + "\">"
						+ (useLanguagePlaceholders ? passwordLanguagePlaceholder : passwordStandardLabel)
					+ "</div>"
					+ "<div class=\"" + fieldContainerClassName + "\">"
						+ "<input type=\"password\" name=\"" + passwordTextFieldName + "\" class=\"" + passwordFieldClassName + "\" />"
					+ "</div>"
				+ "</div>"
				+ "<div class=\"" + preserveLoginRowContainerClassName + "\">"
					+ "<div class=\"" + fieldContainerClassName + "\">"
						+ "<input type=\"checkbox\" value=\"True\" name=\"" + preserveLoginCheckboxFieldName + "\" class=\"" + preserveLoginFieldClassName + "\" />"
					+ "</div>"
					+ "<div class=\"" + labelContainerClassName + "\">"
						+ (useLanguagePlaceholders ? preserveLoginLanguagePlaceholder : preserveLoginStandardLabel)
					+ "</div>"
				+ "</div>"
				+ "<div class=\"" + buttonRowContainerClassName + "\">"
					+ "<input type=\"submit\" value=\"" + (useLanguagePlaceholders ? buttonLanguagePlaceholder : buttonStandardText)
					+ "\" class=\"" + loginButtonClassName + "\" />"
				+ "</div>";
			if (showForgotPasswordLink)
			{
				html +=
				"<div class=\"" + forgotPasswordLinkContainerClassName + "\">"
					+ "<a href=\"" + forgotPasswordLinkURL + "\">"
					+ (useLanguagePlaceholders ? forgotPasswordLanguagePlaceholder : forgotPasswordStandardLinkText)
					+ "</a>"
				+ "</div>";
			}
			html += "</div>";

			return html;
		}
	}

	public class PasswordReminderForm
	{
		protected bool useLanguagePlaceholders = false;
		protected bool showErrorMessage = false;

		protected string loginFormContainerClassName = "password-reminder-form-container";

		protected string labelContainerClassName = "password-reminder-form-label-container";
		protected string fieldContainerClassName = "password-reminder-form-field-container";
		protected string usernameFieldClassName = "password-reminder-form-field-username";
		protected string emailFieldClassName = "password-reminder-form-field-email";

		protected string errorRowContainerClassName = "password-reminder-form-row-error";
		protected string usernameRowContainerClassName = "password-reminder-form-row-username";
		protected string emailRowContainerClassName = "password-reminder-form-row-email";
		protected string buttonRowContainerClassName = "password-reminder-form-row-buttons";

		protected string loginButtonClassName = "password-reminder-form-button";

		protected string usernameLanguagePlaceholder = "{?form-label-username?}";
		protected string emailLanguagePlaceholder = "{?form-label-email?}";
		protected string buttonLanguagePlaceholder = "{?form-button-send-password-reminder?}";
		protected string errorMessageLanguagePlaceholder = "{?password-reminder-error-message-placeholder?}";

		protected string usernameStandardLabel = "Username";
		protected string emailStandardLabel = "Email";
		protected string buttonStandardText = "Send Login Details";
		protected string errorMessageStandardText = "The details you specified did not match any user in our database.";

		protected string usernameTextFieldName = "Username";
		protected string emailTextFieldName = "Email";

		#region Class Properties

		public bool UseLanguagePlaceholders
		{
			get { return useLanguagePlaceholders; }
			set { useLanguagePlaceholders = value; }
		}

		public bool ShowErrorMessage
		{
			get { return showErrorMessage; }
			set { showErrorMessage = value; }
		}

		public string LoginFormContainerClassName
		{
			get { return loginFormContainerClassName; }
			set { loginFormContainerClassName = value; }
		}

		public string LabelContainerClassName
		{
			get { return labelContainerClassName; }
			set { labelContainerClassName = value; }
		}

		public string FieldContainerClassName
		{
			get { return fieldContainerClassName; }
			set { fieldContainerClassName = value; }
		}

		public string UsernameFieldClassName
		{
			get { return usernameFieldClassName; }
			set { usernameFieldClassName = value; }
		}

		public string EmailFieldClassName
		{
			get { return emailFieldClassName; }
			set { emailFieldClassName = value; }
		}

		public string ErrorRowContainerClassName
		{
			get { return errorRowContainerClassName; }
			set { errorRowContainerClassName = value; }
		}

		public string UsernameRowContainerClassName
		{
			get { return usernameRowContainerClassName; }
			set { usernameRowContainerClassName = value; }
		}

		public string EmailRowContainerClassName
		{
			get { return emailRowContainerClassName; }
			set { emailRowContainerClassName = value; }
		}

		public string ButtonRowContainerClassName
		{
			get { return buttonRowContainerClassName; }
			set { buttonRowContainerClassName = value; }
		}

		public string LoginButtonClassName
		{
			get { return loginButtonClassName; }
			set { loginButtonClassName = value; }
		}

		public string UsernameLanguagePlaceholder
		{
			get { return usernameLanguagePlaceholder; }
			set { usernameLanguagePlaceholder = value; }
		}

		public string EmailLanguagePlaceholder
		{
			get { return emailLanguagePlaceholder; }
			set { emailLanguagePlaceholder = value; }
		}

		public string ButtonLanguagePlaceholder
		{
			get { return buttonLanguagePlaceholder; }
			set { buttonLanguagePlaceholder = value; }
		}

		public string ErrorMessageLanguagePlaceholder
		{
			get { return errorMessageLanguagePlaceholder; }
			set { errorMessageLanguagePlaceholder = value; }
		}

		public string UsernameStandardLabel
		{
			get { return usernameStandardLabel; }
			set { usernameStandardLabel = value; }
		}

		public string EmailStandardLabel
		{
			get { return emailStandardLabel; }
			set { emailStandardLabel = value; }
		}

		public string ButtonStandardText
		{
			get { return buttonStandardText; }
			set { buttonStandardText = value; }
		}

		public string ErrorMessageStandardText
		{
			get { return errorMessageStandardText; }
			set { errorMessageStandardText = value; }
		}

		public string UsernameTextFieldName
		{
			get { return usernameTextFieldName; }
			set { usernameTextFieldName = value; }
		}

		public string EmailTextFieldName
		{
			get { return emailTextFieldName; }
			set { emailTextFieldName = value; }
		}

		#endregion

		public override string ToString()
		{
			string html =
			"<div class=\"" + loginFormContainerClassName + "\">";
			if (showErrorMessage)
			{
				html += "<div class=\"" + errorRowContainerClassName + "\">"
					+ (useLanguagePlaceholders ? errorMessageLanguagePlaceholder : errorMessageStandardText)
					+ "</div>";
			}
			html +=
				"<div class=\"" + usernameRowContainerClassName + "\">"
					+ "<div class=\"" + labelContainerClassName + "\">"
						+ (useLanguagePlaceholders ? usernameLanguagePlaceholder : usernameStandardLabel)
					+ "</div>"
					+ "<div class=\"" + fieldContainerClassName + "\">"
						+ "<input type=\"text\" name=\"" + usernameTextFieldName + "\" class=\"" + usernameFieldClassName + "\" />"
					+ "</div>"
				+ "</div>"
				+ "<div class=\"" + emailRowContainerClassName + "\">"
					+ "<div class=\"" + labelContainerClassName + "\">"
						+ (useLanguagePlaceholders ? emailLanguagePlaceholder : emailStandardLabel)
					+ "</div>"
					+ "<div class=\"" + fieldContainerClassName + "\">"
						+ "<input type=\"text\" name=\"" + emailTextFieldName + "\" class=\"" + emailFieldClassName + "\" />"
					+ "</div>"
				+ "</div>"
				+ "<div class=\"" + buttonRowContainerClassName + "\">"
					+ "<input type=\"submit\" value=\"" + (useLanguagePlaceholders ? buttonLanguagePlaceholder : buttonStandardText)
					+ "\" class=\"" + loginButtonClassName + "\" />"
				+ "</div>"
			+ "</div>";

			return html;

		}
	}

	public class LoginControl
	{
		private bool useLanguagePlaceholders = false;
		private string passwordReminderProcessingURL = "#";
		private string loginProcessingURL = "#";
		private string passwordReminderURL = "#";
		private string passwordSentMessageStandard = "Your password has been email to you.";
		private string passwordSentMessageLanguagePlaceholder = "{?password-reminder-sent-message?}";
		private LoginForm loginForm = new LoginForm();
		private PasswordReminderForm pwForm = new PasswordReminderForm();

		#region Class Properties

		public string PasswordSentMessageStandard
		{
			get { return passwordSentMessageStandard; }
			set { passwordSentMessageStandard = value; }
		}

		public string PasswordSentMessageLanguagePlaceholder
		{
			get { return passwordSentMessageLanguagePlaceholder; }
			set { passwordSentMessageLanguagePlaceholder = value; }
		}

		public bool UseLanguagePlaceholders
		{
			get { return useLanguagePlaceholders; }
			set { useLanguagePlaceholders = value; }
		}

		public string LoginProcessingURL
		{
			get { return loginProcessingURL; }
			set { loginProcessingURL = value; }
		}

		public string PasswordReminderURL
		{
			get { return passwordReminderURL; }
			set { passwordReminderURL = value; }
		}

		public string PasswordReminderProcessingURL
		{
			get { return passwordReminderProcessingURL; }
			set { passwordReminderProcessingURL = value; }
		}

		public LoginForm LoginForm
		{
			get { return loginForm; }
			set { loginForm = value; }
		}

		public PasswordReminderForm PwForm
		{
			get { return pwForm; }
			set { pwForm = value; }
		}

		#endregion

		protected string RenderForm(object form, string formURL)
		{
			return string.Format(
				"<form method=\"post\" action=\"{0}\">{1}</form>",
				formURL, form);
		}

		public enum State
		{
			NotLoggedIn,
			LoginError,
			ForgotPassword,
			PasswordReminderError,
			PasswordSent
		}

		public string ShowForm(State state)
		{
			switch (state)
			{
				case State.NotLoggedIn:
					loginForm.ShowErrorMessage = false;
					loginForm.ForgotPasswordLinkURL = passwordReminderURL;
					loginForm.UseLanguagePlaceholders = useLanguagePlaceholders;
					return RenderForm(loginForm, loginProcessingURL);

				case State.LoginError:
					loginForm.ShowErrorMessage = true;
					loginForm.ForgotPasswordLinkURL = passwordReminderURL;
					loginForm.UseLanguagePlaceholders = useLanguagePlaceholders;
					return RenderForm(loginForm, loginProcessingURL);

				case State.ForgotPassword:
					pwForm.ShowErrorMessage = false;
					pwForm.UseLanguagePlaceholders = useLanguagePlaceholders;
					return RenderForm(pwForm, passwordReminderProcessingURL);

				case State.PasswordReminderError:
					pwForm.ShowErrorMessage = true;
					pwForm.UseLanguagePlaceholders = useLanguagePlaceholders;
					return RenderForm(pwForm, passwordReminderProcessingURL);

				case State.PasswordSent:
					loginForm.ShowErrorMessage = true;
					loginForm.ErrorMessageStandardText = passwordSentMessageStandard;
					loginForm.ErrorMessageLanguagePlaceholder = passwordSentMessageLanguagePlaceholder;
					loginForm.ForgotPasswordLinkURL = passwordReminderURL;
					loginForm.UseLanguagePlaceholders = useLanguagePlaceholders;
					return RenderForm(loginForm, loginProcessingURL);
			}
			return null;
		}
	}
}
