using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrowdSource.Models.CoreModels
{
    /// <summary>
    /// Associated entity between ApplicationUser and GroupVersion.
    /// 
    /// This is what counts the reviews.
    /// </summary>
    public class ApplicationUserEndorsesGroupVersion
    {
        [Key]
        public int Id { get; set; }

        //[ForeignKey("ApplicationUserForeignKey")]
        
        public ApplicationUser User { get; set; }

        //[ForeignKey("GroupVersionForeignKey")]
        public GroupVersion GroupVersion { get; set; }
    }
}
