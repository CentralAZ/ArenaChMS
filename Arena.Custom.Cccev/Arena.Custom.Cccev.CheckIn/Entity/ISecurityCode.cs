/**********************************************************************
* Description:	TBD
* Created By:   Jason Offutt @ Central Christian Church of the East Valley
* Date Created:	TBD
*
* $Workfile: ISecurityCode.cs $
* $Revision: 5 $ 
* $Header: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/Entity/ISecurityCode.cs   5   2010-09-23 13:53:58-07:00   JasonO $
* 
* $Log: /trunk/Arena.Custom.Cccev/Arena.Custom.Cccev.CheckIn/Entity/ISecurityCode.cs $
*  
*  Revision: 5   Date: 2010-09-23 20:53:58Z   User: JasonO 
*  Implementing changes suggested by HDC. 
*  
*  Revision: 4   Date: 2009-02-24 18:18:51Z   User: JasonO 
*  Updating org setting keys for provider class Luids. 
*  
*  Revision: 3   Date: 2009-01-06 16:06:08Z   User: JasonO 
**********************************************************************/

using System;
using System.Reflection;
using Arena.Core;
using Arena.Exceptions;

namespace Arena.Custom.Cccev.CheckIn.Entity
{
    public interface ISecurityCode
    {
        /// <summary>
        /// Returns security code.
        /// </summary>
        /// <returns><see cref="string"/>Security Code</returns>
        string GetSecurityCode(Person p);
    }

    public static class SecurityCodeHelper
    {
        public static Lookup DefaultSecurityCodeSystem(int organizationID)
        {
            Organization.Organization org = new Organization.Organization(organizationID);
            Lookup lookup = null;

            if (org.Settings["Cccev.SecurityCodeDefaultSystemID"] != null)
            {
                try
                {
                    lookup = new Lookup(int.Parse(org.Settings["Cccev.SecurityCodeDefaultSystemID"]));
                }
                catch { }
            }

            return lookup;
        }

        public static ISecurityCode GetSecurityCodeClass(Lookup securityCodeSystem)
        {
            Assembly assembly = Assembly.Load(securityCodeSystem.Qualifier2);

            if (assembly == null)
                return null;

            Type type = assembly.GetType(securityCodeSystem.Qualifier8) ??
                        assembly.GetType(securityCodeSystem.Qualifier2 + "." + securityCodeSystem.Qualifier8);

            if (type == null)
            {
                throw new ArenaApplicationException(string.Format("Could not find '{0}' class in '{1}' assembly.", securityCodeSystem.Qualifier2, securityCodeSystem.Qualifier8));
            }

            return (ISecurityCode)Activator.CreateInstance(type);
        }
    }
}
