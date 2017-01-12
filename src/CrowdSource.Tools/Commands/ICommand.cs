using System;
using System.Collections.Generic;
using System.Text;

namespace CrowdSource.Tools.Commands
{
    public interface ICommand
    {
        void Run(string[] args);
    }
}
