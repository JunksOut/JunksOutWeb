using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JunksOut {
    public class MyRole :
        OmidID.Web.Security.EFRoleProvider<
            OmidID.Web.Security.Default.DefaultRole,// Or your custom class
            OmidID.Web.Security.Default.DefaultUserRole,// Or your custom class
            int> {
    }
}