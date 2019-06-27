<%@ control language="c#" inherits="ArenaWeb.UserControls.Security.ChangePassword, Arena" %>
<asp:UpdatePanel ID="up" runat="server">
    <ContentTemplate>
        <asp:Panel ID="pnlEntry" Runat="server" DefaultButton="btnChangePassword">
            <ul>
                <li>
                    <label for="<%= txtLoginId.ClientID %>">Login ID:</label>
                    <asp:TextBox id="txtLoginId" width="100" cssclass="formItem" runat="server" />
                </li>
                <li>
                    <label for="<%= txtCurrentPassword.ClientID %>">Current Password:</label>
                    <asp:TextBox id="txtCurrentPassword" width="100" textmode="password" cssclass="formItem" runat="server" />
                </li>
                <li>
                    <label for="<%= txtNewPassword.ClientID %>">New Password:</label>
                    <asp:TextBox id="txtNewPassword" width="100" textmode="password" cssclass="formItem" runat="server" />
                </li>
                <li>
                    <label for="<%= txtNewPassword2.ClientID %>">Confirm Password:</label>
                    <asp:TextBox id="txtNewPassword2" width="100" textmode="password" cssclass="formItem" runat="server" />
                </li>
            </ul>
            <asp:Button id="btnChangePassword" runat="server" text="Change Password" CssClass="smallText" onclick="btnChangePassword_Click" />
        </asp:Panel>

	    <asp:label id="lblMessage" cssClass="highlightText" runat="server" />

        <Arena:ModalPopup ID="mdlPopup" runat="server" BehaviorID="mdlChangePasswordPopup" Title="Change Password">
        <Content>
            <p>This login is associated with a network account.  If you change your password, the password for your network account will also be changed.</p>
            <p>Do you want to continue?</p>
            </div>
        </Content>
        <Buttons>
            <asp:Button ID="btnNo" runat="server" Text="  No  " CssClass="formButton" OnClientClick="$find('mdlChangePasswordPopup').hide(); return false;" />
            <asp:Button ID="btnYes" runat="server" Text="  Yes  " CssClass="formButton" OnClick="btnYes_Click" />
        </Buttons>
        </Arena:ModalPopup>
    </ContentTemplate>
</asp:UpdatePanel>