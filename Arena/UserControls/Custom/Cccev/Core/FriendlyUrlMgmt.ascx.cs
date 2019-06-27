/**********************************************************************
* Description:   Friendly URL Management Module
* Created By:    Derek Mangrum
* Date Created:  8/10/2011 12:59:00  PM
*
* $Workfile: FriendlyUrlMgmt.ascx.cs $
* $Revision: 18 $ 
* $Header: /trunk/Arena/UserControls/Custom/Cccev/Core/FriendlyUrlMgmt.ascx.cs   18   2011-08-26 13:03:04-07:00   derekm $
* 
* $Log: /trunk/Arena/UserControls/Custom/Cccev/Core/FriendlyUrlMgmt.ascx.cs $
*  
*  Revision: 18   Date: 2011-08-26 20:03:04Z   User: derekm 
*  Cleaned up (column widths, etc.) 
*  
*  Revision: 17   Date: 2011-08-26 19:52:24Z   User: derekm 
*  Forgot to turn editEnabled back on 
*  
*  Revision: 16   Date: 2011-08-26 19:43:28Z   User: derekm 
*  Added size option to QR Code Request 
*  
*  Revision: 15   Date: 2011-08-25 22:38:16Z   User: nicka 
*  More PROTOTYPE code 
*  
*  Revision: 14   Date: 2011-08-25 21:56:40Z   User: nicka 
*  PROTOTYPE code for Derek. 
*  
*  Revision: 13   Date: 2011-08-25 19:38:03Z   User: derekm 
*  - Removed UpdatePanel 
*  - Corrected updates to DDL and GridView on Adds/Deletes/Updates 
*  
*  Revision: 12   Date: 2011-08-24 22:46:22Z   User: derekm 
*  Added delete button and fixed DropDownList show it refreshes on adds and 
*  deletes. 
*  
*  Revision: 11   Date: 2011-08-19 15:06:39Z   User: derekm 
*  - Fixed the datagrid paging issue. 
*  
*  Revision: 10   Date: 2011-08-19 04:39:01Z   User: derekm 
*  . 
*  
*  Revision: 9   Date: 2011-08-19 04:10:24Z   User: derekm 
*  - Added a Module Setting for "Virtual Directory Physicl Path Home". This 
*  folder contains the folders that the Virtual Directories map to. 
*  - Replaced additional hard-coded values with Module Settings. 
*  
*  Revision: 8   Date: 2011-08-18 23:31:35Z   User: derekm 
*  Refactored the VanityUrl calls to only pull once for both the report and 
*  the add/edit form. 
*  
*  Revision: 7   Date: 2011-08-18 20:01:42Z   User: derekm 
*  Major revisions: 
*  - Added the Add/Edit functionality to the page 
*  - Security-based visibility of items on the page 
*  
*  Revision: 6   Date: 2011-08-18 16:59:59Z   User: derekm 
*  Removed !Postback block so datatable is re-fetched on each postback 
*  
*  Revision: 5   Date: 2011-08-18 16:33:00Z   User: nicka 
*  Added no wrap for the QR code field and a mini snippet showing module edit 
*  security checking. 
*  
*  Revision: 4   Date: 2011-08-16 22:10:45Z   User: derekm 
*  Replace 'vanity' verbage to 'friendly' verbage 
*  
*  Revision: 3   Date: 2011-08-15 18:08:49Z   User: derekm 
*  Goofing around. 
*  
*  Revision: 2   Date: 2011-08-10 20:06:12Z   User: derekm 
*  - Removed the Delete Button 
*  - Added Module Parameter for "Friendly URL Request Form" 
*  
*  Revision: 1   Date: 2011-08-10 19:44:42Z   User: derekm 
**********************************************************************/
using System;
using System.Linq;
using Arena.Portal;
using ComponentArt.Web.UI;
using System.Text;
using Arena.Security;
using Microsoft.Web.Administration;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web;

#region ReponseHelper = WIN!
//
// Grabbed from http://weblogs.asp.net/infinitiesloop/archive/2007/09/25/response-redirect-into-a-new-window-with-extension-methods.aspx
//
public static class ResponseHelper
{
	public static void Redirect(this HttpResponse response,
		string url,
		string target,
		string windowFeatures)
	{

		if ((String.IsNullOrEmpty(target) ||
			target.Equals("_self", StringComparison.OrdinalIgnoreCase)) &&
			String.IsNullOrEmpty(windowFeatures))
		{
			response.Redirect(url);
		}
		else
		{
			Page page = (Page)HttpContext.Current.Handler;
			if (page == null)
			{
				throw new InvalidOperationException(
					"Cannot redirect to new window outside Page context.");
			}

			url = page.ResolveClientUrl(url);

			string script;
			if (!String.IsNullOrEmpty(windowFeatures))
			{
				script = @"window.open(""{0}"", ""{1}"", ""{2}"");";
			}
			else
			{
				script = @"window.open(""{0}"", ""{1}"");";
			}

			script = String.Format(script, url, target, windowFeatures);
			ScriptManager.RegisterStartupScript(page,
				typeof(Page),
				"Redirect",
				script,
				true);
		}
	}
}
#endregion

namespace ArenaWeb.UserControls.Custom.Cccev.Core
{
	public partial class FriendlyUrlMgmt : PortalControl
	{
		#region Module Settings

		[TextSetting("Base Web URL", "This is the base URL for your public website. (Ex: http://www.centralaz.com) ", true)]
		public string BaseWebUrlSetting
		{
			get
			{
				return Setting("BaseWebUrl", "", true);
			}
		}

		[TextSetting("IIS Website Name", "This is the exact name of the IIS website you are accessing. (Default: 'Default Web Site') ", false)]
		public string IisWebsiteNameSetting
		{
			get
			{
				return Setting("IisWebsiteName", "Default Web Site", false);
			}
		}

		[TextSetting("Friendly URL Request Form", "The URL to request a new, or modify a current, friendly URL. (Default: 'http://arena/Arena/default.aspx?page=3991&assignmentTypeID=51' ", false)]
		public string FriendlyUrlRequestFormSetting
		{
			get
			{
				return Setting("FriendlyUrlRequestForm", "http://arena/Arena/default.aspx?page=3991&assignmentTypeID=51", false);
			}
		}

		[TextSetting("Location for physical folders associated with IIS Virtual Directories", @"This is the parent folder for all of the IIS Virtual Directory physical folders. (Default: 'C:\inetpub\_RedirectedSites\' ", false)]
		public string VirtualDirectoryPhysicalFolderHomeSetting
		{
			get
			{
				return Setting("VirtualDirectoryPhysicalFolderHome", @"C:\inetpub\_RedirectedSites\", false);
			}
		}

		#endregion

		bool _editEnabled;

		protected void Page_Init(object sender, EventArgs e)
		{
			grdVirtualDirectoryList.ReBind += grdVirtualDirectoryList_ReBind;
			grdVirtualDirectoryList.ItemCommand += new DataGridCommandEventHandler(grdVirtualDirectoryList_ItemCommand);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			_editEnabled = CurrentModule.Permissions.Allowed(OperationType.Edit, CurrentUser);

			if (!IsPostBack)
			{
				divAddEditRemoveForm.Visible = false;
				DataTable dtVirtualDirectories = GetVirtualDirectories();
				grdVirtualDirectoryList.DataSource = dtVirtualDirectories;
				grdVirtualDirectoryList.DataBind();
				ddlFriendlyUrls.DataSource = dtVirtualDirectories;
				ddlFriendlyUrls.DataBind();
			}

			btnShowHideFormDiv.Visible = _editEnabled;
			btnFriendlyURLRequest.Visible = !_editEnabled;
			btnFriendlyURLRequest2.Visible = !_editEnabled;
		}

		private void LoadFriendlyUrlDdl(DataTable dtVirtualDirectories)
		{
			ddlFriendlyUrls.Items.Clear();
			ListItem i = new ListItem("<add new>", "new");
			ddlFriendlyUrls.Items.Add(i);
			ddlFriendlyUrls.DataSource = dtVirtualDirectories;
			ddlFriendlyUrls.DataTextField = dtVirtualDirectories.Columns["FriendlyURL"].ToString();
			ddlFriendlyUrls.DataBind();
			ddlFriendlyUrls.SelectedValue = "new";
		}

		private void ClearForm()
		{
			ddlFriendlyUrls.SelectedValue = "new";

			FriendlyUrlName.Text = string.Empty;
			RedirectDestination.Text = string.Empty;
			ExactDestination.Checked = true;
			ChildOnly.Checked = false;
			ddlStatusCode.SelectedValue = "Permanent";
		}

		void grdVirtualDirectoryList_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			if (e.CommandName == "GetQRCode")
			{
				string size = ((TextBox)e.Item.FindControl("txtQRCodeSize")).Text;
				string friendlyName = e.Item.Cells[0].Text;
				string target = "_blank";
				string url = (string.Format("http://api.qrserver.com/v1/create-qr-code/?data={0}/{1}&size={2}x{2}", BaseWebUrlSetting, friendlyName, size));
				string windowFeatures = string.Empty;
				Response.Redirect(url, target, windowFeatures);
				((TextBox)e.Item.FindControl("txtQRCodeSize")).Text = string.Empty;
			}
		}

		private void grdVirtualDirectoryList_ReBind(object sender, EventArgs e)
		{
			DataTable dtVirtualDirectories = GetVirtualDirectories();
			grdVirtualDirectoryList.DataSource = dtVirtualDirectories;
			grdVirtualDirectoryList.DataBind();
		}

		private DataTable GetVirtualDirectories()
		{

			ServerManager serverManager = new ServerManager();

			Application app = serverManager.Sites[IisWebsiteNameSetting].Applications["/"];

			DataTable dtFriendlyURLS = new DataTable();
			dtFriendlyURLS.Columns.Add(new DataColumn("FriendlyURL", typeof(System.String)));
			dtFriendlyURLS.Columns.Add(new DataColumn("Destination", typeof(System.String)));
			dtFriendlyURLS.Columns.Add(new DataColumn("ExactDestination", typeof(System.Boolean)));
			dtFriendlyURLS.Columns.Add(new DataColumn("ChildOnly", typeof(System.Boolean)));
			dtFriendlyURLS.Columns.Add(new DataColumn("HttpResponseStatus", typeof(System.String)));
			dtFriendlyURLS.Columns.Add(new DataColumn("QRCode", typeof(System.String)));


			var vDs = (from vDir in app.VirtualDirectories
					   where vDir.Path != "/"
					   orderby vDir.Path
					   select vDir);


			foreach (VirtualDirectory vD in vDs)
			{
				Configuration config = serverManager.GetWebConfiguration(IisWebsiteNameSetting + vD.Path);
				ConfigurationSection httpRedirectSection = config.GetSection("system.webServer/httpRedirect");

				DataRow dRow = dtFriendlyURLS.NewRow();
				char[] charsToTrim = { '/' };
				dRow["FriendlyURL"] = vD.Path.ToString().Trim(charsToTrim);
				dRow["Destination"] = httpRedirectSection["destination"].ToString();
				dRow["ExactDestination"] = (bool)httpRedirectSection["exactDestination"];
				dRow["ChildOnly"] = (bool)httpRedirectSection["childOnly"];
				switch (httpRedirectSection["httpResponseStatus"].ToString())
				{
					case "301":
						dRow["HttpResponseStatus"] = "Permanent";
						break;
					case "302":
						dRow["HttpResponseStatus"] = "Found";
						break;
					case "307":
						dRow["HttpResponseStatus"] = "Temporary";
						break;
				}
				string fullFriendlyURL = BaseWebUrlSetting + vD.Path;
				dRow["QRCode"] = fullFriendlyURL;

				dtFriendlyURLS.Rows.Add(dRow);
			}

			return dtFriendlyURLS;
		}

		private void CreateHttpRedirectSettings(string vDirPath)
		{
			ServerManager serverManager = new ServerManager();

			Configuration config = serverManager.GetWebConfiguration(IisWebsiteNameSetting + vDirPath);
			ConfigurationSection httpRedirectSection = config.GetSection("system.webServer/httpRedirect");
			httpRedirectSection["enabled"] = "true";
			httpRedirectSection["destination"] = RedirectDestination.Text;
			httpRedirectSection["exactDestination"] = ExactDestination.Checked.ToString();
			httpRedirectSection["childOnly"] = ChildOnly.Checked.ToString();
			httpRedirectSection["httpResponseStatus"] = ddlStatusCode.SelectedValue;

			serverManager.CommitChanges();
		}

		protected void ddlFriendlyUrls_SelectedIndexChanged(object sender, EventArgs e)
		{
			lblProcessReport.Text = string.Empty;
			lblOutput.Text = string.Empty;

			DataTable dtVirtualDirectories = GetVirtualDirectories();

			if (ddlFriendlyUrls.SelectedValue != "new")
			{
				foreach (DataRow dataRow in dtVirtualDirectories.Rows)
				{
					if (dataRow["FriendlyURL"].ToString() == ddlFriendlyUrls.SelectedValue)
					{
						FriendlyUrlName.Text = dataRow["FriendlyURL"].ToString();
						RedirectDestination.Text = dataRow["Destination"].ToString();
						ExactDestination.Checked = (bool)dataRow["ExactDestination"];
						ChildOnly.Checked = (bool)dataRow["ChildOnly"];
						ddlStatusCode.SelectedValue = dataRow["HttpResponseStatus"].ToString();
						break;
					}
				}
			}
			else
			{
				ClearForm();
			}
		}

		private bool DirectoryExists(string directoryToFind)
		{
			bool directoryFound = false;
			string basePath = VirtualDirectoryPhysicalFolderHomeSetting;  //@"C:\inetpub\_RedirectedSites\";
			string fullPath = basePath + directoryToFind;

			foreach (string d in Directory.GetDirectories(basePath))
			{
				if (d == fullPath)
				{
					directoryFound = true;
				}
			}

			return directoryFound;
		}

		protected void btnCancel_Click(object sender, EventArgs e)
		{
			ClearForm();
		}

		protected void btnDelete_Click(object sender, EventArgs e)
		{
			bool proceed = true;
			if (string.IsNullOrEmpty(FriendlyUrlName.Text))
			{
				proceed = false;
			}

			if (ddlFriendlyUrls.SelectedValue != "new" && proceed == true)
			{
				string basePath = VirtualDirectoryPhysicalFolderHomeSetting; // @"C:\inetpub\_RedirectedSites\";
				string vDirPath = "/" + FriendlyUrlName.Text;

				ServerManager serverManager = new ServerManager();
				Application app = serverManager.Sites[IisWebsiteNameSetting].Applications["/"];
				VirtualDirectory vDir = app.VirtualDirectories[vDirPath];
				app.VirtualDirectories.Remove(vDir);
				serverManager.CommitChanges();

				Directory.Delete(vDir.PhysicalPath, true);

				DataTable dtVirtualDirectories = GetVirtualDirectories();
				LoadFriendlyUrlDdl(dtVirtualDirectories);
				grdVirtualDirectoryList.DataSource = dtVirtualDirectories;
				grdVirtualDirectoryList.DataBind();
				ClearForm();

				lblProcessReport.Text = "Process Complete.";
			}
			else
			{
				ClearForm();
				lblProcessReport.Text = "No Friendly URL selected.";
			}
		}

		protected void btnSave_Click(object sender, EventArgs e)
		{
			DataTable dtVirtualDirectories = GetVirtualDirectories();
			bool newVanityUrl = true;

			if (!string.IsNullOrEmpty(FriendlyUrlName.Text) && (ddlFriendlyUrls.SelectedValue == "new") && !DirectoryExists(FriendlyUrlName.Text))
			{
				foreach (DataRow dataRow in dtVirtualDirectories.Rows)
				{
					if (dataRow["FriendlyURL"].ToString() == FriendlyUrlName.Text)
					{
						lblOutput.Text = "Virtual Directory " + FriendlyUrlName.Text + " already exists!";
						newVanityUrl = false;
						break;
					}
				}

				if (newVanityUrl)
				{
					string basePath = VirtualDirectoryPhysicalFolderHomeSetting; // @"C:\inetpub\_RedirectedSites\";
					string fullPath = basePath + FriendlyUrlName.Text;
					Directory.CreateDirectory(fullPath);

					string vDirPath = "/" + FriendlyUrlName.Text;
					ServerManager serverManager = new ServerManager();
					Application app = serverManager.Sites[IisWebsiteNameSetting].Applications["/"];
					app.VirtualDirectories.Add(vDirPath, fullPath);
					serverManager.CommitChanges();

					CreateHttpRedirectSettings(vDirPath);

					dtVirtualDirectories = GetVirtualDirectories();
					LoadFriendlyUrlDdl(dtVirtualDirectories);
					grdVirtualDirectoryList.DataSource = dtVirtualDirectories;
					grdVirtualDirectoryList.DataBind();

					ClearForm();
					lblProcessReport.Text = "Process Complete.";
				}
			}
			else
			{
				if (!string.IsNullOrEmpty(FriendlyUrlName.Text))
				{
					lblOutput.Text = "Not new or " + FriendlyUrlName.Text + " exists.";
					string vDirPath = "/" + FriendlyUrlName.Text;
					if (DirectoryExists(FriendlyUrlName.Text))
					{
						CreateHttpRedirectSettings(vDirPath);

						dtVirtualDirectories = GetVirtualDirectories();
						grdVirtualDirectoryList.DataSource = dtVirtualDirectories;
						grdVirtualDirectoryList.DataBind();

						ClearForm();
						lblProcessReport.Text = "Process Complete.";
					}
					else
					{
						lblProcessReport.Text = "ERROR: Physical folder does not exist.";
					}
				}
			}
		}

		protected void btnFriendlyURLRequest_Click(object sender, EventArgs e)
		{
			Response.Redirect(FriendlyUrlRequestFormSetting, "_blank", "");
		}

		protected void btnShowHideFormDiv_Click(object sender, EventArgs e)
		{
			if (_editEnabled)
			{
				if (divAddEditRemoveForm.Visible)
				{
					divAddEditRemoveForm.Visible = false;
					btnShowHideFormDiv.Text = "Show Add/Edit Friendly URL Form";
				}
				else
				{
					divAddEditRemoveForm.Visible = true;
					LoadFriendlyUrlDdl(GetVirtualDirectories());
					ClearForm();
					btnShowHideFormDiv.Text = "Hide Add/Edit Friendly URL Form";
				}
			}
		}

	
	}
}