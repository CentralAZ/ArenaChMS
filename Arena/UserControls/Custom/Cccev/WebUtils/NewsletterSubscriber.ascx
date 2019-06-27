<%@ Control Language="C#" ClassName="NewsletterSubscriber" inherits="ArenaWeb.UserControls.Core.NewsletterSubscribe, Arena" %>

<div class="form-holder">
    <asp:Panel ID="subscribeForm" runat="server" CssClass="subscribe">
        <div class="cover">
            <div class="holder">
                <asp:Label ID="Label1" AssociatedControlID="txtFirstName" runat="server">First Name:</asp:Label>
                <div class="small">
                    <asp:TextBox ID="txtFirstName" Runat="server" CssClass="small" />
                </div>
                <asp:RequiredFieldValidator ID="rfFirstName" ControlToValidate="txtFirstName" runat="server" CssClass="errorText" ErrorMessage="First name required" Display="Dynamic">*</asp:RequiredFieldValidator>
            </div>
            
            <div class="holder">
                <asp:Label ID="Label2" AssociatedControlID="txtLastName" runat="server">Last Name:</asp:Label>
                <div class="small">
                    <asp:TextBox ID="txtLastName" Runat="server" CssClass="small" />
                </div>
                <asp:RequiredFieldValidator ID="rfLastName" ControlToValidate="txtLastName" Runat="server" CssClass="errorText" ErrorMessage="Last name required" Display="Dynamic">*</asp:RequiredFieldValidator>
            </div>
        </div>
        
        <div class="cover">
            <asp:Label ID="Label3" AssociatedControlID="txtEmail" runat="server">Email:</asp:Label>
            <div class="wide">
                <asp:TextBox ID="txtEmail" Runat="server" CssClass="wide" />
            </div>
            <asp:RequiredFieldValidator ID="rfEmail" ControlToValidate="txtEmail" Runat=server CssClass="errorText" ErrorMessage="Email required" Display="Dynamic">*</asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator id="reEmail" ControlToValidate="txtEmail" CssClass="errorText" ErrorMessage="Please enter a valid e-mail address" runat="server" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" Display="Dynamic">*</asp:RegularExpressionValidator>
        </div>
        
        <div class="cover">
            <div class="holder">
                <asp:Label ID="Label4" AssociatedControlID="ddGender" runat="server">Gender:</asp:Label>
                <asp:DropDownList id="ddGender" Runat="server" />
                <asp:CompareValidator id="cvGender" CssClass="errorText" ErrorMessage="Please select your gender" ControlToValidate="ddGender" ValueToCompare="-1" Type="Integer" Operator="GreaterThan" runat="server" Display="Dynamic">*</asp:CompareValidator>
            </div>
            
            <asp:Button ID="btnSubscribe" CssClass="btn" Runat="server" Text="Sign Up"></asp:Button>
        </div>
    </asp:Panel>
    
    <asp:Panel ID="subscribeResponse" CssClass="subscribeResponse success" Runat="server" Visible="false">
        <asp:Label ID="lblResponse" Runat="server"></asp:Label>
    </asp:Panel>
</div>