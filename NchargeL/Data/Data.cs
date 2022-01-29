using NCLCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NchargeL
{
    public class Data
    {

        public static List<User>? users;
        public static ObservableCollection<Client>? clients { get; set; }



        public static void init()
        {

            users = new List<User>();
            if (Properties.Settings.Default.User != "")
                users.Add(XmlUtil.Deserialize(typeof(User), Properties.Settings.Default.User) as User);
            clients = new ObservableCollection<Client>();
        }

    }
}


