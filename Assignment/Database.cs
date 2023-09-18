using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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
            if (!dataStruct.Any(data => data.AcctUsername == username))
            {
                DatabaseStorage.DataStruct data = new DatabaseStorage.DataStruct
                {
                    AcctUsername = username,
                };

                dataStruct.Add(data); //adding user to database
            }

            else
            {
                throw new FaultException("Username already exists");
            }
        }

        public void RemoveUser(string username)
        {
            // Check if the username exists, and if it does, remove it
            var userToRemove = dataStruct.FirstOrDefault(data => data.AcctUsername == username);
            if (userToRemove != null)
            {
                dataStruct.Remove(userToRemove);
            }
            else
            {
                throw new FaultException("Username not found");
            }
        }

        public List<DatabaseStorage.DataStruct> GetDataStructList() //getting the list of users
        {
            return dataStruct;
        }
    }
}
