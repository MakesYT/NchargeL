using NCLCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NchargeL
{
    public class Data
    {

        public static List<User>? users;
        public static ObservableCollection<Client>? clients { get; set; }
        
        

        public static void init()
        {

            users = new List<User>();
            clients = new ObservableCollection<Client>();
        }
       
    }
}


