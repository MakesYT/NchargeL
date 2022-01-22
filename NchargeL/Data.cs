using NCLCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NchargeL
{
    internal class Data: INotifyPropertyChanged
    {
        public static List<User>? users;
        public static List<Client>? clients;
    public  List<Client> _clients
    {
        get { return clients; }
        set
        {
                clients = value;
            OnPropertyChanged("_clients");
        }
    }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public static void init() { 
            users = new List<User>(); 
            clients = new List<Client>();
        }
    }
}
