using System.Collections.Generic;

namespace NchargeL
{
    internal class DataBase
    {
        public static List<User> users;
        public static void init() { users = new List<User>(); }
    }
}