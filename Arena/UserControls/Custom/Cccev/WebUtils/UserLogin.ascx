<%@ control language="c#" inherits="ArenaWeb.UserControls.Security.UserLogin, Arena" %>

	<asp:ScriptManagerProxy ID="smpScripts" runat="server" />
	
	<script runat="server">
	    private void Page_Init(object sender, EventArgs e)
	    {
	        if (!Page.IsPostBack)
	        {
                smpScripts.Scripts.Add(new ScriptReference(string.Format("~/{0}", BasePage.JQUERY_INCLUDE)));
                smpScripts.Scripts.Add(new ScriptReference(string.Format("~/Templates/Cccev/liger/js/form.js")));
	        }
	    }
	       
		private void Page_PreRender( object sender, EventArgs e )
		{
			txtPassword.Attributes.Add( "onkeydown", "if(event.which || event.keyCode){if ((event.which == 13) || (event.keyCode == 13)) {document.getElementById('" + btnSignin.ClientID + "').click();return false;}} else {return true}; " );
			cbRemember.Attributes.Add( "onkeydown", "if(event.which || event.keyCode){if ((event.which == 13) || (event.keyCode == 13)) {document.getElementById('" + btnSignin.ClientID + "').click();return false;}} else {return true}; " );
		}
	</script>
<script type="text/javascript">
	function pageLoad()
	{
		//initCastomForms();
		// show the popup-light-holder if there is a
		// user message being displayed (ie, meaning we're not done yet).
		var message = $get("<%= lblMessage.ClientID %>");
		if (message)
		{
			SetCheckBox();
			$('.popup-light-holder').show();

			// attach the close event to the X button
			$('.central-login a.btn').click(function()
			{
				$('.popup-light-holder').hide();
				$('.overlayBg').fadeOut();
				return true;
			});
		}
	}

	function SetCheckBox()
	{
		var checkbox = $get("<%= cbRemember.ClientID %>");
		checkbox.click(function()
		{
			//if ($(this).hasClass("checkboxArea"))
			if ( $(this).checked )
			{
				$(this).removeClass().addClass('checkboxAreaChecked');
			}
			else
			{
				$(this).removeClass().addClass('checkboxArea');
			}
		});
		
	}
</script>

<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
<ContentTemplate>
<input type="hidden" id="iRedirect" runat="server" NAME="iRedirect" />
<!-- took out "width: 500px;" from the below div's style to center it -->
<div class="popup-light-holder" style=" height: 500px; z-index: 999;" >
  <div class="central-login-holder">
   <div class="central-login">
    <div class="login-box">
     <h2 class="img">my central login</h2>
     <a href="#" class="btn">btn</a>
     <div class="form-holder">
		<asp:label id="lblMessage" cssClass="errorText" runat="server" Visible="true" />
		<asp:Panel id="pnlLogin" runat="server" Visible="true" CssClass="pnlLogin">
			<div class="loginWrap">
				<asp:Panel ID="pnlImportantNote" runat="Server" CssClass="important notice" Visible="false"/>
			</div>
		</asp:Panel>
		
       <div class="subscribe">
        <label><%=LoginIDCaptionSetting%>:</label>
        <div class="text"><asp:TextBox id="txtLoginId" cssclass="formItem" runat="server" ValidationGroup="userLogin" /></div>
        <label class="mar-top">Password:</label>
        <div class="text"><asp:TextBox id="txtPassword" textmode="password" cssclass="formItem" runat="server" ValidationGroup="userLogin" /></div>
        <div class="check-box"  style="display: block"><asp:checkbox id="cbRemember" runat="server" Text="" ValidationGroup="userLogin" /></div>
        <label for="chek" class="chek">Remember Me</label>
        <asp:Button id="btnSignin" runat="server" text="" CssClass="btn" onclick="btnSignin_Click" ValidationGroup="userLogin" />
       </div>
      <div class="link-holder">
       <p><span>{</span><a href="<%= string.Format("default.aspx?page={0}&mode=login", CreateAccountPageSetting) %>">Create an Account</a><span class="separator">&nbsp;</span><a href="<%= string.Format("default.aspx?page={0}&mode=password", SendAccountInfoPageSetting) %>">Forgot Password</a><span>}</span></p>
      </div>
      
<asp:Panel ID="pnlChangePassword" CssClass="changePass" runat="server" Visible="False" DefaultButton="btnChangePassword">
	<h3>Your password has expired.  Please change it before continuing.</h3>
	<table class="changePassTable">
		<tr>
			<td>New Password:</td>
			<td><asp:TextBox ID="txtNewPassword" TextMode="Password" CssClass="formItem" runat="server" />
				<asp:RequiredFieldValidator ControlToValidate="txtNewPassword" ID="rfvNewPassword" Runat= "server" ErrorMessage="Password is required!" CssClass="errorText error" Display="None" SetFocusOnError="true"></asp:RequiredFieldValidator>
				<asp:RegularExpressionValidator ControlToValidate="txtNewPassword" ID="revNewPassword" Runat="server" ErrorMessage="Invalid Password" CssClass="errorText error" ValidationExpression="\w+" EnableClientScript="false"></asp:RegularExpressionValidator>
			</td>
		</tr>
		<tr>
			<td>Confirm Password:</td>
			<td><asp:TextBox ID="txtNewPassword2" TextMode="Password" CssClass="formItem" runat="server" />
				<asp:RequiredFieldValidator ControlToValidate="txtNewPassword2" ID="rfvNewPassword2" Runat= "server" ErrorMessage="Password confirmation is required!" CssClass="errorText error" Display="None" SetFocusOnError="true"></asp:RequiredFieldValidator>
				<asp:CompareValidator ID="cvNewPassword2" Runat="server" ControlToValidate="txtNewPassword2" ControlToCompare="txtNewPassword" ErrorMessage="Password confirmation must match password!" CssClass="errorText error" Display="None" Operator="Equal"></asp:CompareValidator>
			</td>
		</tr>
		<tr>
			<td>&nbsp;</td>
			<td><asp:Button ID="btnChangePassword" runat="server" Text="Change Password" CssClass="smallText" OnClick="btnChangePassword_Click" /></td>
		</tr>
	</table>
</asp:Panel>

<div style="display: none">
<asp:Panel ID="pnlCreateAccount" CssClass="module createAccount" Runat="server" Visible="False">

	<h3>Register for an Account:</h3>
	<p>If you do not currently have a login account and would like to set one up, click the "Create Account" button below.</p>
	<p>Note: If you aready have an account that you are using for another area of our website, 
	you do not need to create a new account. Use your current account to login.</p>
	<asp:Button ID="btnCreateAccount" Runat="server" Text="Create Account" CssClass="smallText"></asp:Button>
</asp:Panel>

<asp:Panel ID="pnlForgot" CssClass="module forgotPass" Runat="server" Visible="False">
	<h3>Forgot Your Password:</h3>
	<p>If you have forgotten your password, use the button below to reset your password.</p>
	<asp:Button ID="btnSend" Runat="server" Text="Forgot Password" CssClass="smallText"></asp:Button>
	
	<h3>Forgot Your <%=LoginIDCaptionSetting%>:</h3>
	<p>If you have forgotten your <%=LoginIDCaptionSetting%>, use the button below to have your <%=LoginIDCaptionSetting%> emailed to you.</p>
	<asp:Button ID="btnLoginSend" Runat="server" Text="Forgot Login ID" CssClass="smallText"></asp:Button>
	
</asp:Panel>
</div>

     </div>
    </div>
   </div>
  </div>
 </div>
</ContentTemplate>
</asp:UpdatePanel>