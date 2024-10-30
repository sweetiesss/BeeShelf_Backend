using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeStore_Repository.Enums
{
    public enum RequestStatus
    {
        Draft,
        Pending,
        Canceled,
        Processing,
        Delivered,
        Failed,
        Completed
    }
}
