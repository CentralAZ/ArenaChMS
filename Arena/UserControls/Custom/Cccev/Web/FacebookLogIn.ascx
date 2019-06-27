<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FacebookLogIn.ascx.cs" Inherits="ArenaWeb.UserControls.Custom.Cccev.Web.FacebookLogIn" %>

<div class="fb-auth" id="fb-root">
    <input type="hidden" id="ihState" name="ihState" runat="server"/>
    <div id="log-in">
        <h3>Sign in with your Facebook account</h3>
        <a href="#" class="facebook-login fbbutton" title="Log In">Log In</a>
    </div>

    <div id="user-info"></div>

    <div id="signup-form">
        <div class="greeting"></div>
        <ol>
            <li>
                <a href="#" class="im-new" title="I'm new around here">I'm new around here</a>
                <div>
                    <asp:ValidationSummary ID="vsSignup" runat="server" ValidationGroup="fbSignup" />
                    <dl>
                        <dt>First Name</dt>
                        <dd><asp:TextBox ID="tbFirstName" runat="server" Enabled="false" ValidationGroup="fbSignup"  /></dd>
                        <dt>Last Name</dt>
                        <dd><asp:Textbox ID="tbLastName" runat="server" Enabled="false" ValidationGroup="fbSignup"  /></dd>
                        <dt>Email Address</dt>
                        <dd>
                            <asp:Textbox ID="tbEmail" runat="server" ValidationGroup="fbSignup"  />
                            <asp:RequiredFieldValidator ID="reqEmail" runat="server" ControlToValidate="tbEmail"
                                ErrorMessage="Email is required." ValidationGroup="fbSignup" />
                            <asp:RegularExpressionValidator ID="regexEmail" runat="server" ControlToValidate="tbEmail" 
                                ValidationExpression="" ErrorMessage="Please enter a valid email address." />
                        </dd>
                        <dt>Birthdate</dt>
                        <dd>
                            <Arena:DateTextbox ID="tbBirthdate" runat="server" ValidationGroup="fbSignup"  />
                            <asp:RequiredFieldValidator ID="reqBirthdate" runat="server" ControlToValidate="tbBirthdate"
                                ErrorMessage="Birthdate is required." ValidationGroup="fbSignup" />
                        </dd>
                        <dt>Phone Number</dt>
                        <dd>
                            <asp:Textbox ID="tbPhone" runat="server" ValidationGroup="fbSignup"  />
                            <asp:RequiredFieldValidator ID="reqPhone" runat="server" ControlToValidate="tbPhone"
                                ErrorMessage="Phone Number is required." ValidationGroup="fbSignup" />
                        </dd>
                        <dt>Zip Code</dt>
                        <dd>
                            <asp:Textbox ID="tbZip" runat="server" ValidationGroup="fbSignup" />
                            <asp:RequiredFieldValidator ID="reqZip" runat="server" ControlToValidate="tbZip"
                                ErrorMessage="Zip Code is required." ValidationGroup="fbSignup" />
                        </dd>
                    </dl>
                    <asp:HiddenField ID="ihAccessToken" runat="server" />
                    <p>
                        <asp:Label ID="lblSignupError" runat="server" CssClass="errorText" />
                        <asp:Button ID="btnRegister" runat="server" Text="Register" CssClass="facebook-register fbbutton" ValidationGroup="fbSignup" />
                        <a href="#" class="fblogout" title="That's not me">That's not me</a>
                    </p>
                </div>
            </li>
            <li>
                <a href="#" class="have-account" title="I have a Central account">I have a Central account</a>
                <div>
                    <asp:ValidationSummary ID="vsLogin" runat="server" ValidationGroup="login" />
                    <dl>
                        <dt>Username</dt>
                        <dd>
                            <asp:Textbox ID="tbUsername" runat="server" ValidationGroup="login" />
                            <asp:RequiredFieldValidator ID="reqUsername" runat="server" ControlToValidate="tbUsername"
                                ErrorMessage="Username is required." ValidationGroup="login" />
                        </dd>
                        <dt>Password</dt>
                        <dd>
                            <asp:Textbox ID="tbPassword" runat="server" TextMode="Password" ValidationGroup="login" />
                            <asp:RequiredFieldValidator ID="reqPassword" runat="server" ControlToValidate="tbPassword"
                                ErrorMessage="Password is required." ValidationGroup="login" />
                        </dd>
                    </dl>
                    <asp:Checkbox ID="cbRememberMe" Text="Remember me?" runat="server" />                    
                    <p>
                        <asp:Label ID="lblLoginError" runat="server" CssClass="errorText" />
                        <asp:Button ID="btnLogin" runat="server" Text="Sign in" ValidationGroup="login" />
                        <a href="default.aspx" title="Cancel">Cancel</a>
                    </p>
                    <p class="no-pad"><strong>Uh oh...</strong> Need help?</p>
                    <ul class="buttons">
                        <li><a href="default.aspx?page=<%= ForgotPasswordPageSetting %>&mode=password" title="I forgot my password" class="buttonStyle">I forgot my password</a></li>
                        <li><a href="default.aspx?page=<%= ForgotUsernamePageSetting %>&mode=login" title="I forgot my username" class="buttonStyle">I forgot my username</a></li>
                    </ul>
                </div>
            </li>
        </ol>
    </div>
</div>
<script type="text/javascript">
    $(function () {
        CentralAZ.Facebook.Helpers.Authentication.config = {
            redirectPath: '<%= GetRedirectPath() %>',
            appID: '<%= FacebookAppIDSetting %>',
            state: '<%= state %>'
        };
        
        CentralAZ.Facebook.Helpers.Authentication.initLogin();
    });
</script>