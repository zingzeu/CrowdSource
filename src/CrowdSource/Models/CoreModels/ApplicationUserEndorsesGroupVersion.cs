using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrowdSource.Models.CoreModels
{
    /// <summary>
    /// Associated entity between ApplicationUser and GroupVersion.
    /// 
    /// This is what counts the reviews.
    /// </summary>
    public class ApplicationUserEndorsesGroupVersion
    {
        public int UserId;
        public ApplicationUser User;

        public int GroupVersionId;
        public GroupVersion GroupVersion;
    }
}
