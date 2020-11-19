using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Socket_test
{
    class Program
    {
        static Th th = new Th();
        static ScktSrv sck = new ScktSrv();

        static void Main ( string [ ] args )
        {
            Dictionary<string , string> re = new Dictionary<string , string> ( );
            re.Add ( "123" , "321" );
            re.Add ( "456" , "654" );
            re.Add ( "789" , "987" );
            sck.re = re;

            sck.msg = 30;
            Console.WriteLine ( sck.create ( "106.104.139.182" , "8882" , 10 ) );
            sck.creth ( ) ;
            Console.WriteLine ( th.start ( "chklist" , chklist ) );
            Console.WriteLine ( "The highest generation is {0}" , GC.MaxGeneration );


            Console.WriteLine ( "\n\n\n================================================>>>>>>按任意建關閉\"0\"號使用者\n" );
            Console.ReadKey ( );
            Console.WriteLine ( "\n\n!!!>>>>" + sck.stop ( 0.ToString ( ) ) );

            Console.WriteLine ( "Generation(Th): {0}" , GC.GetGeneration ( th ) );
            Console.WriteLine ( "Generation(Sck): {0}" , GC.GetGeneration ( sck ) );
            Console.WriteLine ( "Total Memory: {0}" , GC.GetTotalMemory ( true ) );


            Console.WriteLine ( "\n\n\n================================================>>>>>>按任意建關閉Socket伺服器\n" );
            Console.ReadKey ( );
            Console.WriteLine ( "\n\n!!!>>>>" + sck.exit );

            Console.WriteLine ( "Generation(Th): {0}" , GC.GetGeneration ( th ) );
            Console.WriteLine ( "Generation(Sck): {0}" , GC.GetGeneration ( sck ) );
            Console.WriteLine ( "Total Memory: {0}" , GC.GetTotalMemory ( true ) );

            Console.WriteLine ( "\n\n\n================================================>>>>>>按任意建關閉所有執行緒\n" );
            Console.ReadKey ( );
            Console.WriteLine ( "\n\n!!!>>>>主程式" + th.exit );

            Console.WriteLine ( "Generation(Th): {0}" , GC.GetGeneration ( th ) );
            Console.WriteLine ( "Total Memory: {0}" , GC.GetTotalMemory ( true ) );
            Console.WriteLine ( "Generation(Sck): {0}" , GC.GetGeneration ( sck ) );
            Console.WriteLine ( "Total Memory: {0}" , GC.GetTotalMemory ( true ) );

            Console.WriteLine ( "\n\n\n================================================>>>>>>按任意建結束\n" );
            Console.ReadKey ( );
        }

        static void chklist ( )
        {
            Dictionary<string , Socket> soc_list = new Dictionary<string , Socket> ( );
            while ( true )
            {
                Console.WriteLine ( "/------------------------------------------------/" );
                Console.WriteLine ( "\t使用中Thread : {0}" , sck.soc_th.th_list.Count );
                Console.WriteLine ( "\t使用中Socket : {0}" , soc_list.Count );
                soc_list = sck.soc_list;
                if( soc_list .Count > 0 )
                Console.WriteLine ( "\n\t位置 : " );
                try
                {
                    foreach ( string str in soc_list.Keys )
                    {
                        Console.WriteLine ( "\tNO.{0} => {1}\n" , str , soc_list [ str ].RemoteEndPoint );
                    }
                }
                catch ( InvalidOperationException ) { }
                Console.WriteLine ( "/------------------------------------------------/" );
                Thread.Sleep ( 1000 );
            }
        }
    }
}
