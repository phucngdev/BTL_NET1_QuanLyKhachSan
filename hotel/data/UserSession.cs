using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hotel.models;

namespace hotel.data
{
    public class UserSession
    {
        private static UserSession _instance;

        public static UserSession Instance => _instance ??= new UserSession();

        private UserSession() { }

        public Employee LoggedInEmployee { get; private set; }

        public void SetLoggedInEmployee(Employee employee)
        {
            LoggedInEmployee = employee;
        }

        public void Clear()
        {
            LoggedInEmployee = null;
        }
    }

}
