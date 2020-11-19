using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Socket_test2
{
    class Program
    {
        static ScktClt sc = new ScktClt ( );

        public static void Main ( String [ ] args )
        {
            
            Console.WriteLine ( sc.create ( "106.104.139.182" , "8882" ) );
            Console.ReadKey ( );
            Console.WriteLine ( sc.exit );
            Console.ReadKey ( );
            
        }
    }
}
