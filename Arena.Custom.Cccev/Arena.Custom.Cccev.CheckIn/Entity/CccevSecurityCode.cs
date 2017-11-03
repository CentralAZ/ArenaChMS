using Arena.Core;
using Arena.Custom.Cccev.CheckIn.DataLayer;

namespace Arena.Custom.Cccev.CheckIn.Entity
{
    public class CccevSecurityCode : ISecurityCode
    {
        string ISecurityCode.GetSecurityCode(Person p)
        {
            return new CccevSecurityCodeData().GetNextSecurityCode();          
        }
    }
}
