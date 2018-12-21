using System;
using System.Collections.Generic;
using System.Text;

namespace CrowdSource.Core
{
    public enum AssignmentAbandonReason
    {
        Unknown,
        Timeout,
        CancelledBySystem,
        CancelledByUser
    }
}
