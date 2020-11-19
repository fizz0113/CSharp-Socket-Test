using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Threading;


class ScktSrv
{

    #region 全域變數
    /*使用Th類別*/
    public Th soc_th =new Th();

    /*中斷鎖定*/
    private object LOCK_soc_list = new object();

    /*Socket 建構*/
    private Socket SERVER;

    /*Socket 使用者清單 已配置使用中*/
    public Dictionary<string , Socket> soc_list
    {
        get
        {
            lock ( LOCK_soc_list )
            {
                return socket_list;
            }
        }
    }
    private Dictionary<string , Socket> socket_list = new Dictionary<string , Socket>();

    /*尚未配置*/
    private List<int> socket_limit = new List<int>();

    /*伺服器回應set*/
    public Dictionary<string , string> re
    {
        set => reqs = value;
    }
    private Dictionary<string , string> reqs;

    /*接收最大字數set*/
    public uint msg
    {
        set => MsgLenth = value;
    }
    private uint MsgLenth;

    /*關伺服器*/
    public string exit
    {
        get => ServerClose ( );
    }
    #endregion

    #region 綁定伺服器位置
    public string create ( string Ip , string Port , int Limit )
    {
        try
        {
            /*Socket 建構函式*/
            SERVER = new Socket ( AddressFamily.InterNetwork , SocketType.Stream , ProtocolType.Tcp );

            /*Socket 綁定IP位置*/
            SERVER.Bind ( new IPEndPoint ( IPAddress.Parse ( Ip ) , int.Parse ( Port ) ) );

            /*Socket 開啟監聽*/
            SERVER.Listen ( Limit );

            for ( int i = 0 ; i < Limit ; i++ )
            {
                socket_limit.Add ( i );
            }
            return "Socket伺服器 => 伺服器成功創建 !!\n  IP : " + Ip + "\nPORT : " + Port;
        }
        catch ( SecurityException )
        {
            return "Socket伺服器 => 創建錯誤 : SE !!";
        }
        catch ( FormatException )
        {
            return "Socket伺服器 => 創建錯誤 : IP位置 或 Port號碼尚未設置 !!";
        }
        catch ( SocketException )
        {
            return "Socket伺服器 => 創建錯誤 : IP位置錯誤 或 Port號碼衝突  !!";
        }
        catch ( ArgumentOutOfRangeException )
        {
            return "Socket伺服器 => 創建錯誤 : Port號碼溢位 !!";
        }
        catch ( Exception e )
        {
            return "Socket伺服器 => 未知錯誤 : " + e;
        }
    }
    #endregion

    #region 建立使用者執行緒
    public void creth ( )
    {
        try
        {
            while ( socket_limit.Count == 0 )
            {
                Thread.Sleep ( 200 );
            }
            soc_th.start ( socket_limit [ 0 ].ToString ( ) , waitclient );
        }
        catch ( Exception e )
        {
            return;
        }
    }
    #endregion

    #region 建立使用者連線
    private void waitclient ( )
    {
        Socket s;
        try
        {
            s = SERVER.Accept ( );
            lock ( LOCK_soc_list )
            {
                socket_list.Add (  socket_limit [ 0 ].ToString(), s );
            }
            Req_and_Res ( socket_limit [ 0 ].ToString() );
        }
        catch ( ObjectDisposedException )
        {
            return;
        }
        catch ( SocketException )
        {
            return;
        }
        catch ( Exception e )
        {
            return;
        }
    }
    #endregion

    #region 接收及回應
    private void Req_and_Res ( string soc_str )
    {
        Socket s = socket_list [ soc_str ];
        socket_limit.RemoveAt ( 0 );
        byte [ ] Msg = Encoding.Default.GetBytes ( "/*-ConnectSuccess-*/\0" );
        s.Send ( Msg , Msg.Length , 0 );
        new Thread ( creth ) { IsBackground = true }.Start ( );
        while ( true )
        {
            try
            {
                /*存放傳送訊息*/
                byte [ ] msg;

                /*存放接收訊息*/
                byte [ ] get_req = new byte [ MsgLenth ];

                /*取得資料長度及接收訊息*/
                int get_req_lenth = s.Receive ( get_req , get_req.Length , 0 );

                /*將接收資料轉成字串*/
                string str = Encoding.Default.GetString ( get_req , 0 , get_req_lenth );

                /*比對及回應*/
                for ( int i = 0 ; i < reqs.Keys.Count ; i++ )
                {
                    /*接收合法訊息及回應*/
                    if ( str.Contains ( reqs.Keys.ElementAt ( i ) ) )
                    {
                        msg = Encoding.Default.GetBytes ( reqs.Values.ElementAt ( i ) + "\0" );
                        s.Send ( msg , msg.Length , 0 );
                    }
                    /*用戶端關閉連線訊息*/
                    else if ( str.Contains ( "/*-CltDown-*/\0" ) )
                    {
                        msg = Encoding.Default.GetBytes ( "/*-OkayDown-*/\0" );
                        s.Send ( msg , msg.Length , 0 );
                        stop ( soc_str );
                    }
                }

                /*傳送結束*/
                msg = Encoding.Default.GetBytes ( "/*-EndMsg-*/\0" );
                s.Send ( msg , msg.Length , 0 );
            }
            catch ( ObjectDisposedException )
            {
                stop ( soc_str );
            }
            catch ( SocketException )
            {
                stop ( soc_str );
            }
            catch ( Exception e )
            {
                stop ( soc_str );
            }
            Thread.Sleep ( 200 );
        }
    }
    #endregion

    #region 斷開指定使用者連線
    public string stop ( string soc_str )
    {
        try
        {
            Socket s;
            if ( soc_list.ContainsKey ( soc_str ) )
            {
                s = socket_list [ soc_str ];
                byte [] msg = Encoding.Default.GetBytes ( "/*-CloseClt-*/\0" );
                s.Send ( msg , msg.Length , 0 );
                socket_limit.Add ( int.Parse(soc_str) );
                lock ( LOCK_soc_list )
                {
                    socket_list.Remove ( soc_str );
                }
                soc_th.stop ( soc_str );
                GC.Collect ( );
                GC.WaitForPendingFinalizers ( );
                return "Socket伺服器 => \"" + s.RemoteEndPoint + "\" 已中斷 !!";
            }
            else
            {
                return "Socket伺服器 => 中斷錯誤 : 無此使用者 !!";
            }
        }
        catch ( Exception e )
        {
            lock ( LOCK_soc_list )
            {
                socket_list.Remove ( soc_str );
            }
            soc_th.stop ( soc_str );
            GC.Collect ( );
            GC.WaitForPendingFinalizers ( );
            return "Socket伺服器 => 未知錯誤 : " + e;
        }
    }
    #endregion

    #region 斷開與所有連線
    private string ServerClose ( )
    {
        lock ( LOCK_soc_list )
        {
            try
            {
                foreach ( Socket s in socket_list.Values )
                {
                    byte [ ] msg = Encoding.Default.GetBytes ( "/*-SrvDown-*/\0" );
                    s.Send ( msg , msg.Length , 0 );
                    s.Shutdown ( SocketShutdown.Both );
                    s.Close ( );
                }
                socket_list.Clear ( );
                socket_limit.Clear ( );
                SERVER.Close ( );
                GC.Collect ( );
                GC.WaitForPendingFinalizers ( );
                return "Socket伺服器 => " + soc_th.exit;
            }
            catch ( Exception e )
            {
                socket_list.Clear ( );
                socket_limit.Clear ( );
                SERVER.Close ( );
                GC.Collect ( );
                GC.WaitForPendingFinalizers ( );
                return "Socket伺服器 => 未知錯誤 : " + e;
            }
        }
    }
    #endregion

    ~ScktSrv ( )
    {
        GC.Collect ( );
        GC.WaitForPendingFinalizers ( );
    }
}

