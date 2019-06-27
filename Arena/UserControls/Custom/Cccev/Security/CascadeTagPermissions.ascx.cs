/**********************************************************************
* Description:	Cascades tag permissions through all children of a given
*				tag as discussed in the Forum thread:
*				http://community.arenachms.com/forums/permalink/2069/2093/ShowThread.aspx#2093
* Created By:	Nick Airdo
* Date Created:	8/28/2008 5:22:20 PM
*
* $Workfile: CascadeTagPermissions.ascx.cs $
* $Revision: 5 $ 
* $Header: /trunk/Arena/UserControls/Custom/Cccev/Security/CascadeTagPermissions.ascx.cs   5   2011-10-18 15:12:24-07:00   nicka $
* 
* $Log: /trunk/Arena/UserControls/Custom/Cccev/Security/CascadeTagPermissions.ascx.cs $
*  
*  Revision: 5   Date: 2011-10-18 22:12:24Z   User: nicka 
*  Adding message for Metric cascading since it's not ready yet. 
*  
*  Revision: 4   Date: 2010-06-09 19:44:20Z   User: nicka 
*  Semi working version of new "groups" and "metrics" cascading.  Needs more 
*  testing. 
*  
*  Revision: 3   Date: 2008-10-27 20:14:10Z   User: nicka 
*  Include Events 
*  
*  Revision: 2   Date: 2008-09-12 04:49:45Z   User: nicka 
*  changed location of sProc due to Arena community permission restrictions. 
*  
*  Revision: 1   Date: 2008-09-02 17:24:35Z   User: nicka 
*  first version 
**********************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.Web.UI;

using Arena.DataLib;
using Arena.Portal;
using Arena.Core;
using Arena.Metric;
using Arena.Security;
using Arena.SmallGroup;

using ComponentArt.Web.UI;


namespace ArenaWeb.UserControls.Custom.Cccev.Security
{
	public partial class CascadeTagPermissions : PortalControl
	{
		protected int _category = 1;
		protected const string sProcURL = "http://codersforchrist.com/ArenaCommunity/sql/cascade_permissions.sql.gz";
		protected const string sProcTags = "cust_arena_sp_setTagPermissions";
		protected const string sProcGroups = "cust_arena_sp_setGroupClusterPermissions";
		protected const string sProcMetrics = "cust_arena_sp_setMetricPermissions";

		protected string sProcAsString
		{
			get { return (string)ViewState[ "sProcAsString" ]; }
			set { ViewState[ "sProcAsString" ] = value; }
		}

		protected string[] sProcs = new string[] { sProcTags, sProcGroups, sProcMetrics };

		#region Module Settings

		[TextSetting( "jQuery UI Path", "Path to jQuery UI library. Default http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/jquery-ui.min.js", false )]
        public string JQUIScriptPathSetting { get { return Setting("JQUIScriptPath", "http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/jquery-ui.min.js", false); } }

		[TextSetting( "CSS Path", "Path to the desired jQuery UI stylesheet. Default: http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/themes/base/jquery-ui.css", false )]
		public string CSSPathSetting { get { return Setting( "CSSPath", "http://ajax.googleapis.com/ajax/libs/jqueryui/1.8/themes/base/jquery-ui.css", false ); } }

		#endregion

		#region Event Handlers

		protected void Page_Load( object sender, EventArgs e )
		{
			// Add required stylesheet and script to the page
			if ( CSSPathSetting != string.Empty )
			{
				Page.Header.Controls.Add( new LiteralControl( string.Format( "<link rel=\"stylesheet\" type=\"text/css\" media=\"screen\" href=\"{0}\" />\n", CSSPathSetting ) ) );
			}

			ClearMessages();

			RegisterScripts();

			if ( ! IsPostBack )
			{
				tvGroups.MultipleSelectEnabled = false;
				if ( !new XSQLStuff().DoRequiredProcsExist( sProcs ) )
				{
					pnlCheckInstall.Visible = true;
				}
				else
				{
					BuildPermissionPanel();
					BindGroups();
                    BindMetrics();
				}
			}
		}

		private void ClearMessages()
		{
			divMsg.Attributes[ "class" ] = "ui-state-highlight ui-corner-all";
			lblPermissionStatus.Text = "";
		}

		private void RegisterScripts()
		{
			smpScripts.Scripts.Add( new ScriptReference( string.Format( "~/{0}", BasePage.JQUERY_INCLUDE ) ) );
			smpScripts.Scripts.Add( new ScriptReference( JQUIScriptPathSetting ) );
		}

        /// <summary>
        /// Method to bind Small Groups to a TreeView control.
        /// </summary>
		private void BindGroups()
		{
			GroupClusterCollection cluster = new GroupClusterCollection();
			cluster.LoadRootClusters( _category, ArenaContext.Current.Organization.OrganizationID );
			BuildGroupClusterTreeView( cluster, null );

			if ( ( tvGroups.Nodes.Count == 0 ) || ( tvGroups.Nodes[ 0 ].Nodes.Count == 0 ) )
			{
				tvGroups.Visible = false;
			}
			else if ( tvGroups.Nodes.Count == 1 )
			{
				tvGroups.Nodes[ 0 ].Expanded = true;
			}
		}

        /// <summary>
        /// Method to bind metrics to a TreeView control.
        /// </summary>
        private void BindMetrics()
        {
            MetricCollection metrics = new MetricCollection();
            metrics.LoadByParentID( ArenaContext.Current.Organization.OrganizationID, -1 );

            BuildMetricTreeView( metrics, null );

            if ( ( tvMetrics.Nodes.Count == 0 ) || ( tvMetrics.Nodes[0].Nodes.Count == 0 ) )
            {
                tvMetrics.Visible = false;
            }
            else if ( tvMetrics.Nodes.Count == 1 )
            {
                tvMetrics.Nodes[0].Expanded = true;
            }
        }

        private void BuildMetricTreeView( MetricCollection metrics, TreeViewNode parentNode )
        {
            foreach ( Arena.Metric.Metric metric in metrics )
            {
                TreeViewNode node = new TreeViewNode();
                if ( metric.Permissions.Allowed( OperationType.View, ArenaContext.Current.User ) )
                {
                    node.Text = metric.Title;
                    node.ShowCheckBox = true;
                    node.Value = metric.MetricID.ToString();
                    node.ID = metric.MetricID.ToString();
                    node.ImageUrl = "~/images/add_metric.gif";
                    node.Visible = true;

                    if ( parentNode == null )
                    {
                        // add the root, parent node
                        tvMetrics.Nodes.Add( node );
                    }
                    else
                    {
                        parentNode.Nodes.Add( node );
                    }

                    if ( metric.ChildMetrics.Count > 0 )
                    {
                        BuildMetricTreeView( metric.ChildMetrics, node );
                    }
                }

                if ( parentNode == null )
                {
                    if ( node.Nodes.Count != 0 )
                    {
                        continue;
                    }
                    tvMetrics.Nodes.Remove( node );
                }
            }
        }

		private void BuildGroupClusterTreeView( GroupClusterCollection clusters, TreeViewNode parentNode )
		{
			foreach ( GroupCluster cluster in clusters )
			{
				TreeViewNode node = new TreeViewNode();
				if ( cluster.Active && cluster.Allowed( OperationType.View, ArenaContext.Current.User, ArenaContext.Current.Person ) )
				{
					node.Text = cluster.Name;
					node.ShowCheckBox = true;
					node.Value = cluster.GroupClusterID.ToString();
					node.ID = cluster.GroupClusterID.ToString();
					node.ImageUrl = "~/images/smallgroup_cluster_level.gif";
					node.Visible = true;

					if ( parentNode == null )
					{
						tvGroups.Nodes.Add( node );
					}
					else
					{
						parentNode.Nodes.Add( node );
					}

					foreach ( Arena.SmallGroup.Group group in cluster.SmallGroups )
					{
						TreeViewNode node2;
						node2 = new TreeViewNode
						{
							Value = group.GroupID.ToString(),
							Text = ( group.Name != string.Empty ) ? group.Name : string.Format( "[Group:{0}]", group.GroupID.ToString() ),
							ShowCheckBox = false,
							ID = group.GroupClusterID.ToString(),
							ImageUrl = "~/include/componentArt/Images/small_groups.gif",
							Visible = true,
							ServerTemplateId = string.Empty
						};
						node.Nodes.Add( node2 );
						if ( node2.Visible )
						{
							ExpandParents( node2 );
						}
					}
					BuildGroupClusterTreeView( cluster.ChildClusters, node );
				}
				if ( parentNode == null )
				{
                    // TODO: do some testing to decide if this needs to be done.
					//RemoveClustersWithNoGroups( node );
					if ( node.Nodes.Count != 0 )
					{
						continue;
					}
					tvGroups.Nodes.Remove( node );
				}
			}
		}

        /// <summary>
        /// TODO: Not sure if I am going to need this
        /// </summary>
        /// <param name="node"></param>
		private void RemoveClustersWithNoGroups( TreeViewNode node )
		{
			List<TreeViewNode> list = new List<TreeViewNode>();
			foreach ( TreeViewNode node2 in node.Nodes )
			{
				if ( ! HasGroups( node2 ) )
				{
					list.Add( node2 );
				}
				RemoveClustersWithNoGroups( node2 );
			}
			foreach ( TreeViewNode node3 in list )
			{
				node.Nodes.Remove( node3 );
			}
		}

		private void ExpandParents( TreeViewNode tvn )
		{
			if ( tvn.ParentNode != null )
			{
				tvn.ParentNode.Expanded = true;
				tvn.Visible = true;
				ExpandParents( tvn.ParentNode );
			}
		}

		private bool HasGroups( TreeViewNode node )
		{
			if ( node.Visible && node.ShowCheckBox )
			{
				return true;
			}
			foreach ( TreeViewNode node2 in node.Nodes )
			{
				if ( HasGroups( node2 ) )
				{
					return true;
				}
			}
			return false;
		}

		protected void btnInstall_Click( object sender, EventArgs e )
		{
			try
			{
				new XSQLStuff().InstallSQLScript( lblStatus, sProcAsString );
				lblStatus.Text = "Install success!";
				btnInstall.Visible = false;
				btnContinue.Visible = true;
			}
			catch ( System.Exception ex )
			{
				lblStatus.Text += "There was an error trying to install the stored procedures: " + ex.Message;
			}
		}

		protected void btnContinue_Click( object sender, EventArgs e )
		{
			pnlPerformInstall.Visible = false;
			BuildPermissionPanel();
		}

		/// <summary>
		/// Handles the Tag cascade permissions event.  Ensures that a single tag has been selected.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnCopyTagPermissions_Click( object sender, EventArgs e )
		{
			btnContinue.Text = aProfilePicker.ProfileID.ToString();
			divMsg.Visible = true;
			try
			{
				if ( aProfilePicker.ProfileID > 0 )
				{
					new XSQLStuff().SetChildPermissions( sProcTags, aProfilePicker.ProfileID );
					Profile profile = new Profile( aProfilePicker.ProfileID );

					lblPermissionStatus.Text = "Permissions copied from '<b>" + profile.Name + "</b>' to all child tags.";
					lblPermissionStatus.Style.Add( "color", "#333388" );

					aProfilePicker.ProfileID = -1;
				}
				else
				{
					lblPermissionStatus.Text = "You must select a Tag first.";
				}
			}
			catch ( System.Exception ex )
			{
				aProfilePicker.ProfileID = -1;
				lblPermissionStatus.Style.Remove( "color" );
				lblPermissionStatus.Text = "There was a problem trying to copy the permissions.<br /><br />" + ex.StackTrace;
			}
		}

		/// <summary>
		/// Handles the Group cascade permissions event. Ensures that a single group has been selected.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnCopyGroupPermissions_Click( object sender, EventArgs e )
		{
			divMsg.Visible = true;
			try
			{
				if ( tvGroups.CheckedNodes.Length == 1 )
				{
					new XSQLStuff().SetChildPermissions( sProcGroups, int.Parse( tvGroups.CheckedNodes[ 0 ].Value) );

					lblPermissionStatus.Text = "Permissions copied from '<b>" + tvGroups.CheckedNodes[ 0 ].Text + "</b>' to all child groups.";
					lblPermissionStatus.Style.Add( "color", "#333388" );

					tvGroups.UnCheckAll();
				}
				else
				{
					lblPermissionStatus.Text = "You must select one Group first.";
				}
			}
			catch ( System.Exception ex )
			{
				divMsg.Attributes[ "class" ] = "errorText";
				aProfilePicker.ProfileID = -1;
				lblPermissionStatus.Style.Remove( "color" );
				lblPermissionStatus.Text = "There was a problem trying to copy the permissions.<br /><br />" + ex.StackTrace;
			}
		}

        // TODO

		/// <summary>
		/// Handles the Metric cascade permissions event. Ensures that a single metric has been selected.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnCopyMetricPermissions_Click( object sender, EventArgs e )
		{
			divMsg.Visible = true;
			try
			{
				if ( tvMetrics.CheckedNodes.Length == 1 )
				{
					new XSQLStuff().SetChildPermissions( sProcMetrics, int.Parse( tvMetrics.CheckedNodes[0].ID ) );

					lblPermissionStatus.Text = "Permissions copied from '<b>" + tvMetrics.CheckedNodes[0].Text + "</b>' to all child metrics.";
					lblPermissionStatus.Style.Add( "color", "#333388" );

					tvMetrics.UnCheckAll();
				}
				else
				{
					lblPermissionStatus.Text = "You must select one Metric first.";
				}
			}
			catch ( System.Exception ex )
			{
				aProfilePicker.ProfileID = -1;
				lblPermissionStatus.Style.Remove( "color" );
				lblPermissionStatus.Text = "There was a problem trying to copy the permissions.<br /><br />" + ex.StackTrace;
			}
		}

		/// <summary>
		/// Called when the install button is clicked. Gets the .gz file from the internet
		/// and installs the SQL script that it contains.
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnDownload_Click( object sender, EventArgs e )
		{
			pnlCheckInstall.Visible = false;
			pnlPerformInstall.Visible = true;
			StringBuilder sb = new StringBuilder();

			// Fetch the file containing the stored procedure...
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create( sProcURL );
			try
			{
				HttpWebResponse res = (HttpWebResponse)req.GetResponse();
				BinaryReader sr = new BinaryReader( res.GetResponseStream() );

				Stream stream = sr.BaseStream;
				// Uncompress the stream if it is a .gz file
				if ( sProcURL.EndsWith( ".gz" ) )
				{
					lblStatus.Text += "Decompressing GZip file...<br>";
					stream = new GZipStream( stream, CompressionMode.Decompress );
					lblStatus.Text += "...done<br>";
				}

				byte[] buffer = ReadStreamFully( stream, 8192 );
				for ( int p = 0; p < buffer.Length; p++ )
				{
					sb.Append( Convert.ToChar( buffer[ p ] ) );
				}

				// Ask the user if they want to install it.
				btnInstall.Visible = true;
				sProcAsString = sb.ToString();
				txtOutput.Text += sb.ToString();
			}
			catch ( System.Exception ex )
			{
				lblStatus.Text += ex.Message;
			}
		}

		/// <summary>
		/// Reads data from a stream until the end is reached. The
		/// data is returned as a byte array. An IOException is
		/// thrown if any of the underlying IO calls fail.
		/// </summary>
		/// <param name="stream">The stream to read data from</param>
		/// <param name="initialLength">The initial buffer length</param>
		public static byte[] ReadStreamFully( Stream stream, int initialLength )
		{
			// If we've been passed an unhelpful initial length, just use 32K.
			if ( initialLength < 1 )
			{
				initialLength = 32768;
			}

			byte[] buffer = new byte[ initialLength ];
			int read = 0;

			int chunk;
			while ( ( chunk = stream.Read( buffer, read, buffer.Length - read ) ) > 0 )
			{
				read += chunk;

				// If we've reached the end of our buffer, check to see if there's
				// any more information
				if ( read == buffer.Length )
				{
					int nextByte = stream.ReadByte();

					// End of stream? If so, we're done
					if ( nextByte == -1 )
					{
						return buffer;
					}

					// Nope. Resize the buffer, put in the byte we've just
					// read, and continue
					byte[] newBuffer = new byte[ buffer.Length * 2 ];
					Array.Copy( buffer, newBuffer, buffer.Length );
					newBuffer[ read ] = (byte)nextByte;
					buffer = newBuffer;
					read++;
				}
			}
			// Buffer is now too big. Shrink it.
			byte[] ret = new byte[ read ];
			Array.Copy( buffer, ret, read );
			return ret;
		}

		#endregion

		private void BuildPermissionPanel()
		{
			divMsg.Visible = false;
			pnlPermissions.Visible = true;
			aProfilePicker.ProfileType = Arena.Enums.ProfileType.Serving;
			aProfilePicker.IncludeEvents = true;
			aProfilePicker.DataBind();
		}

		#region SQL Stuff

		/// <summary>
		/// A SQL utility class... if this is going to become a pattern, this should be
		/// refactored to a separate class.
		/// </summary>
		class XSQLStuff : SqlData
		{
			public bool DoRequiredProcsExist( string[] sProcs )
			{
				string[] partialStatement = new string[ sProcs.Length ];

				for ( int i = 0; i < sProcs.Length; i++ )
				{
					partialStatement[ i ] = string.Format( "object_id <> OBJECT_ID(N'[{0}]')", sProcs[ i ] );
				}

				object x = ExecuteScalar( "IF EXISTS (SELECT * FROM sys.objects WHERE ( " + string.Join( " AND ", partialStatement ) + " ) AND type in (N'P', N'PC')) Select 1" );
				if ( x != null )
				{
					return true;
				}
				else
				{
					return false;
				}
			}

			/// <summary>
			/// Installs the given SQL script.
			/// </summary>
			/// <param name="sqlScript">a sql script</param>
			/// <returns>a status message indicating whether it was a success or not</returns>
			public void InstallSQLScript( Label lblStatus, string sqlScript )
			{
				string[] individualScripts = Regex.Split( sqlScript, "GO\r\n" );

				for ( int i = 0; i < individualScripts.Length; i++ )
				{
					lblStatus.Text += individualScripts[ i ];
					ExecuteNonQuery( individualScripts[i] );					
				}
			}

			/// <summary>
			/// Sets the permissions of the given item's children to match the given item.
			/// </summary>
			/// <param name="sProc">name of the stored procedure to execute</param>
			/// <param name="ID">id of the parent item</param>
			public void SetChildPermissions( string sProc, int ID )
			{
				ArrayList lst = new ArrayList();
                // if ( sProc.Contains("setGroupCluster") )
                //	lst.Add( new SqlParameter( "@parent", "a" ) );
				//else
					lst.Add( new SqlParameter( "@parent", ID ) );

				try
				{
					ExecuteNonQuery( sProc, lst );
				}
				catch ( SqlException ex )
				{
					throw ex;
				}
				finally
				{
					lst = null;
				}
			}
		}
		#endregion

	} // end class
} // end namespace