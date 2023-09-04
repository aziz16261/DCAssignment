using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib
{
    public class DatabaseStorage
    {
        public class DataStruct
        {
            public String AcctUsername; 
            public String AcctPassword;

            public DataStruct()
            {
                AcctPassword = "";
                AcctUsername = "";
            }
        }
    }
}
