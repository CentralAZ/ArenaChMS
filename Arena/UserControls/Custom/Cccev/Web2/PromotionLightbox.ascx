<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PromotionLightbox.ascx.cs" Inherits="ArenaWeb.UserControls.Custom.Cccev.Web2.PromotionLightbox" %>
<%@ Register TagPrefix="Arena" Namespace="Arena.Portal.UI" Assembly="Arena.Portal.UI" %>
<%@ Import Namespace="Arena.Marketing" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>

<asp:ScriptManagerProxy ID="smpScripts" runat="server" />

<script runat="server">

	#region Node Helper Class
	public class Node
	{
		public string Name { get; set; }
		public List<PromotionRequest> Promotions { get; set; }
		public List<Node> Children { get; set; }

		/// <summary>
		/// Returns the list of children nodes sorted by rules:
		///   * normal alpha sorting
		///   * or if child names are Days of week (Mon, Tue, Wed, Thu, etc.) sort accordingly
		/// </summary>
		/// <returns>Sorts the children nodes</returns>
		public void SortChildren()
		{
			if ( Children.Any( n => n.Name.StartsWith("Mon") ||  n.Name.StartsWith("Tue") ||  n.Name.StartsWith("Wed") 
				||  n.Name.StartsWith("Thu") || n.Name.StartsWith("Fri") ||  n.Name.StartsWith("Sat")
				||  n.Name.StartsWith("Sun") ) )
			{
				Children.Sort( new WeekDayComparer() );
			}
			else
			{
				Children.Sort( new NameComparer() );
			}
		}
		
		/// <summary>
		/// Adds the given node as a child node and/or checks for existance of existing node of same name.
		/// </summary>
		/// <param name="node"></param>
		/// <returns>The new or existing node</returns>
		public Node AddChild( Node node )
		{
			if ( Children == null )
			{
				Children = new List<Node>();
				Children.Add( node );
			}
			// otherwise see if it already exists,
			else
			{
				Node found = ( from n in Children where n.Name == node.Name select n ).FirstOrDefault();
				if ( found == null )
				{
					Children.Add( node );
				}
				else
				{
					node = found;
				}
			}
			return node;
		}

		/// <summary>
		/// Method to add a promotion to the node.
		/// </summary>
		/// <param name="p"></param>
		public void AddPromotion( PromotionRequest p )
		{
			if ( Promotions == null )
				Promotions = new List<PromotionRequest>();

			Promotions.Add( p );
		}
	}

	/// <summary>
	/// Helper method (an IComparer) to compare nodes by "Name" property
	/// and by proper day of week ordering when a Node's name is
	/// a day of week (ie, Sun, Sunday, etc.)
	/// </summary>
	private class WeekDayComparer : IComparer<Node>
	{
		public int Compare( Node a, Node b )
		{
			int x = IntValueFor( a.Name );
			int y = IntValueFor( b.Name );
			return x.CompareTo( y );
		}
	}

	/// <summary>
	/// Helper method (for the WeekDayComparer) to return the
	/// numeric value for a day of week.
	/// </summary>
	private static int IntValueFor( string dayofweek )
	{
		if ( dayofweek.StartsWith( "Sun" ) )
			return 0;
		else if ( dayofweek.StartsWith( "Mon" ) )
			return 1;
		else if ( dayofweek.StartsWith( "Tue" ) )
			return 2;
		else if ( dayofweek.StartsWith( "Wed" ) )
			return 3;
		else if ( dayofweek.StartsWith( "Thu" ) )
			return 4;
		else if ( dayofweek.StartsWith( "Fri" ) )
			return 5;

		return 6;
	}

	/// <summary>
	/// Helper method (an IComparer) to compare nodes by "Name" property.
	/// </summary>
	private class NameComparer : IComparer<Node>
	{
		public int Compare( Node a, Node b )
		{
			return a.Name.CompareTo( b.Name );
		}
	}
	#endregion
	
	/// <summary>
	/// A routine to convert a list of promotions into a tree
	/// where each promotion's Title (Foo/Bar/A, Foo/Bar/B, etc) is
	/// used to create the structure of the tree.
	/// </summary>
	/// <param name="promotions"></param>
	/// <returns></returns>
	public Node BuildTree( IEnumerable<PromotionRequest> promotions )
	{
		var returnArray = new Node();
		foreach ( var p in promotions )
		{
			var parts = p.Title.Split( '/' ).ToList();
			string leafPart = parts[parts.Count - 1];
			parts.RemoveAt( parts.Count - 1 );
			var parentArr = returnArray;
			foreach ( var part in parts )
			{
				var node = new Node();
				node.Name = part;
				parentArr = parentArr.AddChild( node );
			}
			
			// Now add the leaf to the lowest parent.
			parentArr.AddPromotion( p );
		}
		return returnArray;
	}

	/// <summary>
	/// Outputs promotions grouped by path based titles (where titles
	/// are in the form "Gilbert/Tues/The Event") using the given tree structure.
	/// </summary>
	/// <param name="node"></param>
	/// <param name="top"></param>
	/// <returns></returns>
	public string OutputPromotionsFromTree( Node node, bool top, int level)
	{
		StringBuilder sb = new StringBuilder();
		if ( node.Name != null )
		{
			sb.AppendFormat( "<h{0}>{1}</h{0}>", level, node.Name );
			sb.Append( "<div class='lightbox'>" );
		}
		node.SortChildren();
		
		foreach ( Node child in node.Children )
		{
			if ( child.Children != null && child.Children.Count > 0 )
			{
				sb.Append( OutputPromotionsFromTree( child, false, level + 1 ) );
			}

			if ( child.Promotions != null && child.Promotions.Count > 0 )
			{
				sb.AppendFormat( "<h{0}>{1}</h{0}>", level + 1, child.Name );
				sb.Append( OutputPromotions( child.Promotions ) );
			}
		}

		if ( node.Name != null )
		{
			sb.Append( "</div>" );
		}
		
		return sb.ToString();
	}
	
	/// <summary>
	/// Original promotion output routine.
	/// </summary>
	/// <param name="promotions"></param>
	/// <returns></returns>
	public string OutputPromotions( IEnumerable<PromotionRequest> promotions )
	{
		StringBuilder sb = new StringBuilder();
		string className = "pf-box";
		int i = 0;
		foreach (var p in promotions)
		{
			string title = p.Title;
			if ( EnableTitlePathSplitSetting )
			{
				title = title.Split( '/' ).Last();
			}
			i++;
			className = (i % NumberColumnsSetting != 0 ) ? "pf-box" : "pf-box-last";
			sb.AppendFormat( "<div class=\"{0}\">", className );
			sb.AppendFormat( "<a href=\"{0}\" rel=\"portfolio\" title=\"{1}\"><img src=\"{2}\" alt=\"{1}\" class=\"pf-img\" /></a>", GetDetailsUrl( p ), p.Title, GetImageUrl( p ) );
			sb.AppendFormat( "<h6>{0}</h6>", title );
			sb.AppendFormat( "</div>" );
 
		}
		return sb.ToString();
	}
</script>

<% var promotions = GetPromotionRequests(); %>
<div id="mainportfolio">
	<div class="porftolio-page">

	<%	    
		if ( EnableTitlePathSplitSetting )
		{
			Node root = BuildTree( promotions );
	%> 
			<%= OutputPromotionsFromTree( root, true, TitlePathLevelSetting )%>
	<%	    
		}
		else
		{
	%> 
		<%= OutputPromotions( promotions )%>
	<%	    
		}
	%> 

    </div>                                                
</div>