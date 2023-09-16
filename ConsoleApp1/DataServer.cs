using DatabaseLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Console1
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class DataServer : DataServerInterface
    {
        private DatabaseClass database;

        public DataServer()
        {
            database = new DatabaseClass();
        }

        public bool CheckAccount(string username)
        {
            List<DatabaseStorage.DataStruct> dataStructList = database.GetDataStructList();

            foreach (var data in dataStructList)
            {
                if (data.AcctUsername == username)
                {
                    return true; // The account exists 
                }
            }

            database.AddUser(username);
            return false; // The account does not exist
        }
    }
}