using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubbyNetwork.Interfaces
{
    public interface ISendEmail
    {
        void Send(string to, string toName, string subject, string body);
    }
}
