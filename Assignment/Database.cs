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

        public void AddUser(string username, string password) //adding a user to the database
        {
            DatabaseStorage.DataStruct data = new DatabaseStorage.DataStruct
            {
                AcctUsername = username,
                AcctPassword = password
            };

            dataStruct.Add(data);
        }

        public List<DatabaseStorage.DataStruct> GetDataStructList() //getting the list of users
        {
            return dataStruct;
        }
    }
}
