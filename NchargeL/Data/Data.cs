using System.Collections.Generic;
using System.Collections.ObjectModel;
using NchargeL.Properties;
using NCLCore;

namespace NchargeL
{
    public class Data
    {
        public static List<User>? users;
        public static ObservableCollection<Client>? clients { get; set; }


        public static void init()
        {
            users = new List<User>();
            if (Settings.Default.User != "")
                users.Add(XmlUtil.Deserialize(typeof(User), Settings.Default.User) as User);
            clients = new ObservableCollection<Client>();
        }
    }
}