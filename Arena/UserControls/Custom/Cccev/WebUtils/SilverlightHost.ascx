
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SilverlightHost.ascx.cs" Inherits="ArenaWeb.UserControls.Custom.Cccev.WebUtils.SilverlightHost" %>
<%@ Register TagPrefix="Arena" Namespace="Arena.Portal.UI" Assembly="Arena.Portal.UI" %>
    <script type="text/javascript">
        function onSilverlightError(sender, args) {
        
            var appSource = "";
            if (sender != null && sender != 0) {
                appSource = sender.getHost().Source;
            } 
            var errorType = args.ErrorType;
            var iErrorCode = args.ErrorCode;
            
            var errMsg = "Unhandled Error in Silverlight 2 Application " +  appSource + "\n" ;

            errMsg += "Code: "+ iErrorCode + "    \n";
            errMsg += "Category: " + errorType + "       \n";
            errMsg += "Message: " + args.ErrorMessage + "     \n";

            if (errorType == "ParserError")
            {
                errMsg += "File: " + args.xamlFile + "     \n";
                errMsg += "Line: " + args.lineNumber + "     \n";
                errMsg += "Position: " + args.charPosition + "     \n";
            }
            else if (errorType == "RuntimeError")
            {           
                if (args.lineNumber != 0)
                {
                    errMsg += "Line: " + args.lineNumber + "     \n";
                    errMsg += "Position: " +  args.charPosition + "     \n";
                }
                errMsg += "MethodName: " + args.methodName + "     \n";
            }

            throw new Error(errMsg);
        }
    </script>
    <%
		if (ShowErrorsSetting == "True")
		{
			
	%>
    <!-- Runtime errors from Silverlight will be displayed here.
	This will contain debugging information and should be removed or hidden when debugging is completed -->
	<div id='errorLocation' style="font-size: small;color: Gray;"></div>
	<%
		}
    %>
    <div id="silverlightControlHost">
		<object data="data:application/x-silverlight," type="application/x-silverlight-2" width='<%= WidthSetting %>' height='<%= HeightSetting %>'>
			<param name="source" value='<%= XapFileSetting %>'/>
			<param name="onerror" value="onSilverlightError" />
			<param name="background" value="white" />
			<param name="InitParameters" value='<%= InitParamsSetting %>' />
			
			<a href="http://go.microsoft.com/fwlink/?LinkID=108182" style="text-decoration: none;">
     			<img src="http://go.microsoft.com/fwlink/?LinkId=108181" alt="Get Microsoft Silverlight" style="border-style: none"/>
			</a>
		</object>
		<iframe style='visibility:hidden;height:0;width:0;border:0px'></iframe>
    </div>