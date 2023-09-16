using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Console1
{ 
    [ServiceContract]
    public interface DataServerInterface
    {
        [OperationContract]
        bool CheckAccount(String username);
    }
}
