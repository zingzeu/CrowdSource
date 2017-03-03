using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace CrowdSource.Models.ManageViewModels
{
    public class ChangeNickNameViewModel
    {

        [Display(Name = "新昵称")]
        [Required]
        public string NewNickName { get; set; }
    }
}
