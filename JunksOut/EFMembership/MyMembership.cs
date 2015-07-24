using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JunksOut {
    public class MyMembership :
        OmidID.Web.Security.EFMembershipProvider<
            OmidID.Web.Security.Default.DefaultUser,// Or your custom class
            OmidID.Web.Security.Default.DefaultOAuthMembership,// Or your custom class
            int> {
    }
}