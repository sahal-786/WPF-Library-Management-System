using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Managment_System
{
    internal class AppContext
    {
        public static int LoggedInUserId { get; private set; }

        public static void SetLoggedInUserId(int userId)
        {
            LoggedInUserId = userId;
        }
    }
}
