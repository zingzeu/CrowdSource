using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using CrowdSource.Models.CoreModels;


namespace CrowdSource.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public List<ApplicationUserEndorsesGroupVersion> MyReviews;
        public List<Suggestion> SuggestionsAuthored;

        public string NickName { get; set; }
    }
}
