using System;
using System.Collections.Generic;
using JsonSample.Models;

namespace JsonSample.Utility
{
    public class AddressComparer : IEqualityComparer<Address>
    {
        public bool Equals(Address x, Address y)
        {
            if(x.line2 == null)
            {
                x.line2 = "";
            }

            if(x.postCode == null)
            {
                x.postCode = "";
            }

            if(y.line2 == null)
            {
                y.line2 = "";
            }

            if(y.postCode == null)
            {
                y.postCode = "";
            }
            
           if (x.line1.Trim().Equals(y.line1.Trim(), StringComparison.CurrentCultureIgnoreCase)
               && String.Equals(x.line2.Trim(), y.line2.Trim(), StringComparison.CurrentCultureIgnoreCase)
                && x.country.Equals(y.country, StringComparison.CurrentCultureIgnoreCase)
               && String.Equals(x.postCode.Trim(), y.postCode.Trim(), StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }

            return false;
        }

        public int GetHashCode(Address obj)
        {
            return 0;
        }
    }
}
