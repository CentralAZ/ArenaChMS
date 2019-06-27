<%@ control language="c#" inherits="ArenaWeb.UserControls.Security.NewAccount, Arena" %>
		
<input type="hidden" id="iRedirect" runat="server" NAME="iRedirect">
<input type="hidden" id="iEmail" runat="server" NAME="iEmail">
<input type="hidden" id="iPassword" runat="server" name="iPassword">
	
<asp:Panel ID="pnlMessage" Runat="server" Visible="False" CssClass="errorPanel">
	<asp:Label ID="lblMessage" Runat="server" CssClass="errorText" Visible="False"></asp:Label>
	<asp:Panel ID="pnlEmailExists" Runat="server">
		<table cellpadding="0" cellspacing="0" border="0">
		<tr>
			<td valign="top"></td>
			<td valign="top" class="errorText" style="padding-left:5px">
				The e-mail address you've entered is already associated with 
				one or more existing account(s) in our system. These accounts
				are listed below. You can have the username and password of 
				any of these existing accounts e-mailed to you by clicking 
				the 'Send Info'	link next to the name.<br><br>
				<div class="smallText" style="padding-left:10px">
					<asp:PlaceHolder ID="phExistingAccounts" Runat="server"></asp:PlaceHolder>
				</div>
				<br>
				If you still wish to continue creating a new account, please click 
				the 'Create New Account' button below.<br><br>
				<asp:Button ID="btnCreate" Runat="server" CssClass="smallText" Text="Create New Account"></asp:Button>
			</td>
		</tr>
		</table>
	</asp:Panel>
</asp:Panel>
<div class="textWrap create-account">
	<table cellpadding="0" cellspacing="0" border="0" class="table02 webForm" width="100%">
	<tr>
		<td align="right"><b>First Name</b></td>
		<td>
			<asp:TextBox ID="tbFirstName" Runat="server" TabIndex="1" CssClass="formItem" maxlength="50"></asp:TextBox>
			<asp:RequiredFieldValidator ID="reqFirstName" Runat="server" ControlToValidate="tbFirstName" CssClass="errorText" Display="Dynamic" ErrorMessage="First Name is required"> *</asp:RequiredFieldValidator>		</td>
	    <td align="right"><b>Street Address</b></td>
	    <td><asp:TextBox ID="tbStreetAddress" Runat="server" TabIndex="7" CssClass="formItem" maxlength="100"></asp:TextBox>
            <asp:RequiredFieldValidator ID="reqStreetAddress" Runat="server" ControlToValidate="tbStreetAddress" CssClass="errorText" 
				Display="Dynamic" ErrorMessage="Street Address is required"> *</asp:RequiredFieldValidator>
        </td>
	    <td align="right"><b>Desired Login ID</b></td>
	    <td><asp:TextBox id="tbLoginID" Runat="server" TabIndex="14" cssclass="formItem" width="100" MaxLength="50" />        
            <asp:RequiredFieldValidator ID="reqLoginID" Runat="server" ControlToValidate="tbLoginID" CssClass="errorText" 
				Display="Dynamic" ErrorMessage="Desired Login ID is required"> *</asp:RequiredFieldValidator>        </td>
	</tr>
	<tr>
		<td align="right"><b>Last Name</b></td>
		<td>
			<asp:TextBox ID="tbLastName" Runat="server" TabIndex="2" CssClass="formItem" maxlength="50"></asp:TextBox>
			<asp:RequiredFieldValidator ID="reqLastName" Runat="server" ControlToValidate="tbLastName" CssClass="errorText" 
				Display="Dynamic" ErrorMessage="Last Name is required"> *</asp:RequiredFieldValidator>		</td>
	    <td align="right"  ><b>City</b></td>
	    <td><asp:TextBox ID="tbCity" Runat="server" CssClass="formItem" TabIndex="8" maxlength="64"></asp:TextBox>
            <asp:RequiredFieldValidator ID="reqCity" Runat="server" ControlToValidate="tbCity" CssClass="errorText" 
				Display="Dynamic" ErrorMessage="City is required"> *</asp:RequiredFieldValidator>
        </td>
	    <td align="right"><b>Password</b></td>
	    <td><asp:TextBox id="tbPassword" Runat="server" TabIndex="16" TextMode="Password" cssclass="formItem" width="100" MaxLength="100" />        
            <asp:RequiredFieldValidator ID="reqPassword" Runat="server" ControlToValidate="tbPassword" CssClass="errorText" EnableViewState="True"  
				Display="Dynamic" ErrorMessage="Password is required"> *</asp:RequiredFieldValidator>        </td>
	</tr>
	<tr>
		<td align="right"><b>E-mail</b></td>
		<td>
			<asp:TextBox ID="tbEmail" Runat="server" TabIndex="3" CssClass="formItem" maxlength="100"></asp:TextBox>
			<asp:RequiredFieldValidator ID="reqEmail" Runat="server" ControlToValidate="tbEmail" CssClass="errorText" 
				Display="Dynamic" ErrorMessage="Email is required"> *</asp:RequiredFieldValidator>
			<asp:RegularExpressionValidator id="revEmail" runat="server" ControlToValidate="tbEmail" CssClass="errorText"
				Display="Dynamic" ValidationExpression="[\w\.-]+(\+[\w-]*)?@([\w-]+\.)+[\w-]+" 
				ErrorMessage="Invalid Email Address"> *</asp:RegularExpressionValidator>		</td>
	    <td align="right"><b>State</b></td>
	    <td><asp:DropDownList ID="ddlState" Runat="server" TabIndex="9" CssClass="formItem">
            <asp:ListItem value=""></asp:ListItem>
            <asp:ListItem value="AL">Alabama</asp:ListItem>
            <asp:ListItem value="AK">Alaska</asp:ListItem>
            <asp:ListItem value="AZ">Arizona</asp:ListItem>
            <asp:ListItem value="AR">Arkansas</asp:ListItem>
            <asp:ListItem value="CA">California</asp:ListItem>
            <asp:ListItem value="CO">Colorado</asp:ListItem>
            <asp:ListItem value="CT">Connecticut</asp:ListItem>
            <asp:ListItem value="DE">Delaware</asp:ListItem>
            <asp:ListItem value="DC">District of Columbia</asp:ListItem>
            <asp:ListItem value="FL">Florida</asp:ListItem>
            <asp:ListItem value="GA">Georgia</asp:ListItem>
            <asp:ListItem value="GU">Guam</asp:ListItem>
            <asp:ListItem value="HI">Hawaii</asp:ListItem>
            <asp:ListItem value="ID">Idaho</asp:ListItem>
            <asp:ListItem value="IL">Illinois</asp:ListItem>
            <asp:ListItem value="IN">Indiana</asp:ListItem>
            <asp:ListItem value="IA">Iowa</asp:ListItem>
            <asp:ListItem value="KS">Kansas</asp:ListItem>
            <asp:ListItem value="KY">Kentucky</asp:ListItem>
            <asp:ListItem value="LA">Louisiana</asp:ListItem>
            <asp:ListItem value="ME">Maine</asp:ListItem>
            <asp:ListItem value="MD">Maryland</asp:ListItem>
            <asp:ListItem value="MA">Massachusetts</asp:ListItem>
            <asp:ListItem value="MI">Michigan</asp:ListItem>
            <asp:ListItem value="MN">Minnesota</asp:ListItem>
            <asp:ListItem value="MS">Mississippi</asp:ListItem>
            <asp:ListItem value="MO">Missouri</asp:ListItem>
            <asp:ListItem value="MT">Montana</asp:ListItem>
            <asp:ListItem value="NE">Nebraska</asp:ListItem>
            <asp:ListItem value="NV">Nevada</asp:ListItem>
            <asp:ListItem value="NH">New Hampshire</asp:ListItem>
            <asp:ListItem value="NJ">New Jersey</asp:ListItem>
            <asp:ListItem value="NM">New Mexico</asp:ListItem>
            <asp:ListItem value="NY">New York</asp:ListItem>
            <asp:ListItem value="NC">North Carolina</asp:ListItem>
            <asp:ListItem value="ND">North Dakota</asp:ListItem>
            <asp:ListItem value="OH">Ohio</asp:ListItem>
            <asp:ListItem value="OK">Oklahoma</asp:ListItem>
            <asp:ListItem value="OR">Oregon</asp:ListItem>
            <asp:ListItem value="PA">Pennsylvania</asp:ListItem>
            <asp:ListItem value="PR">Puerto Rico</asp:ListItem>
            <asp:ListItem value="RI">Rhode Island</asp:ListItem>
            <asp:ListItem value="SC">South Carolina</asp:ListItem>
            <asp:ListItem value="SD">South Dakota</asp:ListItem>
            <asp:ListItem value="TN">Tennessee</asp:ListItem>
            <asp:ListItem value="TX">Texas</asp:ListItem>
            <asp:ListItem value="UT">Utah</asp:ListItem>
            <asp:ListItem value="VT">Vermont</asp:ListItem>
            <asp:ListItem value="VI">Virgin Islands</asp:ListItem>
            <asp:ListItem value="VA">Virginia</asp:ListItem>
            <asp:ListItem value="WA">Washington</asp:ListItem>
            <asp:ListItem value="WV">West Virginia</asp:ListItem>
            <asp:ListItem value="WI">Wisconsin</asp:ListItem>
            <asp:ListItem value="WY">Wyoming</asp:ListItem>
            <asp:ListItem value="AA">AA</asp:ListItem>
            <asp:ListItem value="AE">AE</asp:ListItem>
            <asp:ListItem value="AP">AP</asp:ListItem>
          </asp:DropDownList>
            <asp:RequiredFieldValidator ID="reqState" Runat="server" ControlToValidate="ddlState" CssClass="errorText" 
				Display="Dynamic" ErrorMessage="State is required"> *</asp:RequiredFieldValidator>
        </td>
	    <td align="right"><b>Verify Password</b></td>
	    <td><asp:TextBox id="tbPassword2" Runat="server" TabIndex="17" TextMode="Password" cssclass="formItem" width="100" MaxLength="100" />        
            <asp:RequiredFieldValidator ID="reqPassword2" Runat="server" ControlToValidate="tbPassword2" CssClass="errorText" EnableViewState="True" 
				Display="Dynamic" ErrorMessage="Verify Password is required"> *</asp:RequiredFieldValidator>
            <asp:CompareValidator ID="cvPassword2" Runat="server" ControlToValidate="tbPassword2" ControlToCompare="tbPassword" Operator="Equal" 
				CssClass="errorText" Display="Dynamic" ErrorMessage="Verify Password does not equal Password"> *</asp:CompareValidator>        </td>
	</tr>
	<tr>
		<td align="right"><b>Birth Date</b></td>
		<td>
			<Arena:DateTextBox ID="tbBirthDate" Runat="server" style="width:80px" TabIndex="4" CssClass="formItem" MaxLenth="30" />
			<asp:RequiredFieldValidator ID="reqBirthDate" Runat="server" ControlToValidate="tbBirthDate" CssClass="errorText" 
				Display="Dynamic" ErrorMessage="Birth Date is required"> *</asp:RequiredFieldValidator>		</td>
	    <td align="right"><b>Zip Code</b></td>
	    <td><asp:TextBox ID="tbZipCode" Runat="server" TabIndex="10" CssClass="formItem" maxlength="24"></asp:TextBox>
            <asp:RequiredFieldValidator ID="reqZipCode" Runat="server" ControlToValidate="tbZipCode" CssClass="errorText" 
				Display="Dynamic" ErrorMessage="Zip Code is required"> *</asp:RequiredFieldValidator>
        </td>
	    <td>&nbsp;</td>
	    <td>&nbsp;</td>
	</tr>
	<tr>
		<td align="right">Marital Status</td>
		<td>
			<asp:DropDownList ID="ddlMaritalStatus" Runat="server" TabIndex="5" CssClass="formItem"></asp:DropDownList>
			<asp:RequiredFieldValidator ID="reqMaritalStatus" Runat="server" ControlToValidate="ddlMaritalStatus" CssClass="errorText" 
				Display="Dynamic" ErrorMessage="Marital Status is required"> *</asp:RequiredFieldValidator>		</td>
	    <td align="right"><b>Home Phone</b></td>
	    <td><Arena:PhoneTextBox ID="tbHomePhone" Runat="server" TabIndex="11" CssClass="formItem" Required="true" />
        </td>
	    <td>&nbsp;</td>
	    <td>&nbsp;</td>
	</tr>
	<tr>
		<td align="right"><b>Gender</b></td>
		<td>
			<asp:DropDownList ID="ddlGender" Runat="server" TabIndex="6" CssClass="formItem">
				<asp:ListItem Value=""></asp:ListItem>
				<asp:ListItem Value="0">Male</asp:ListItem>
				<asp:ListItem Value="1">Female</asp:ListItem>
			</asp:DropDownList>
			<asp:RequiredFieldValidator ID="reqGender" Runat="server" ControlToValidate="ddlGender" CssClass="errorText" 
				Display="Dynamic" ErrorMessage="Gender is required"> *</asp:RequiredFieldValidator>		</td>
	    <td align="right" class="normalText" >Work Phone</td>
	    <td><Arena:PhoneTextBox ID="tbWorkPhone" Runat="server" TabIndex="12" CssClass="formItem" ShowExtension="true" />
        </td>
	    <td>&nbsp;</td>
	    <td>&nbsp;</td>
	</tr>
	<tr>
		<td align="right">&nbsp;</td>
		<td>&nbsp;</td>
	    <td align="right" class="normalText">Cell Phone</td>
	    <td colspan="2"><Arena:PhoneTextBox ID="tbCellPhone" Runat="server" CssClass="formItem" TabIndex="13" />
     
			<asp:CheckBox ID="cbSMS" runat="server" class="smallText" Text="Enable SMS" />
        </td>
	    <td>&nbsp;</td>
	</tr>
	<tr>
		<td align="right"></td>
		<td>&nbsp;</td>
	    <td>&nbsp;</td>
	    <td>&nbsp;</td>
	    <td>&nbsp;</td>
	    <td align="right"><asp:Button ID="btnSubmit" Runat="server" CssClass="smallText" Text="Register"></asp:Button></td>
	</tr>
	</table>
</div>
