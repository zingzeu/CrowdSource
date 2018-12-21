﻿using CrowdSource.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrowdSource.ViewComponents
{
    public class DisplayGroupViewComponent : ViewComponent
    {
        private readonly IDataLogic _logic;

        public DisplayGroupViewComponent(IDataLogic logic)
        {
            _logic = logic;
        }

        public async Task<IViewComponentResult> InvokeAsync (int GroupId)
        {
            ViewData["FileName"] = GetGroupImgUrl(GroupId);
            return View("DisplayGroup","/segments/"+GetGroupImgUrl(GroupId));
        }

        private string GetGroupImgUrl(int GroupId)
        {
            Dictionary<string,string> meta = _logic.GetGroupMetadata(GroupId);
            return meta["ImgFileName"];

        }
    }
}
