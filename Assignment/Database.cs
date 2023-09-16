using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib
{
    public class DatabaseClass
    {
        private List<DatabaseStorage.DataStruct> dataStruct;

        public DatabaseClass()
        {
            dataStruct = new List<DatabaseStorage.DataStruct>();
        }

        public void AddUser(string username)
        {
            // Check if the username already exists
            if (!dataStruct.Any(data => data.AcctUsername == username))
            {
                DatabaseStorage.DataStruct data = new DatabaseStorage.DataStruct
                {
                    AcctUsername = username,
                };

                dataStruct.Add(data);
            }

            else
            {
                
            }
        }

            public List<DatabaseStorage.DataStruct> GetDataStructList() //getting the list of users
        {
            return dataStruct;
        }
    }
}
