using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CrowdSource.Tools.Commands
{
    public interface ICommand
    {
        Task RunAsync(IWebHost host, string[] args);
    }
}
