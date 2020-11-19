using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


class ScktClt
{
    public Socket clt
    {
        get => CLIENT;
    }
    private Socket CLIENT;

    public bool conn
    {
        get => connect;
    }
    private bool connect = false;


    public string exit
    {
        get => Exit ( );
    }

    public string create ( string Ip , string Port )
    {
        byte [ ] msg = new byte [ 1024 ];
        int msg_len;
        try
        {
            CLIENT = new Socket ( AddressFamily.InterNetwork , SocketType.Stream , ProtocolType.Tcp );
            CLIENT.Connect ( new IPEndPoint ( IPAddress.Parse ( Ip ) , int.Parse ( Port ) ) );
            msg = new byte [ 1024 ];
            msg_len = CLIENT.Receive ( msg );
            string str = Encoding.Default.GetString ( msg , 0 , msg_len );
            if ( !str.Contains ( "/*-ConnectSuccess-*/" ) )
            {
                return "不是相對應的伺服器";
            }
            return "連接伺服器成功";
        }
        catch ( FormatException )
        {
            return "IP或PORT未輸入正確";
        }
        catch ( ObjectDisposedException )
        {
            return "**123";
        }
        catch ( SocketException )
        {
            return "\"" + Ip + ":" + Port + "\"目標位置並未開啟(請確認伺服器狀態)";
        }
    }

    public string req_res ( string str )
    {
        try
        {
            byte [ ] req = Encoding.Default.GetBytes ( str );
            int bytesSent = CLIENT.Send ( req );

            byte [ ] res = new byte [ 1024 ];
            int res_len = CLIENT.Receive ( res );
            string s = Encoding.Default.GetString ( res , 0 , res_len );

            res_len = CLIENT.Receive ( res );
            if ( !( Encoding.Default.GetString ( res , 0 , res_len ) ).Contains ( "/*-EndMsg-*/" ) )
            {
                return "未取得結束字串";
            }
            return s;
        }
        catch( ObjectDisposedException )
        {
            return "**321";
        }
        catch( SocketException )
        {
            return "目標位置已關閉連線";
        }
    }

    private string Exit ( )
    {
        byte [ ] req = Encoding.Default.GetBytes ( "/*-CltDown-*/\0" );
        int bytesSent = CLIENT.Send ( req );
        byte [ ] res = new byte [ 1024 ];
        int res_len = CLIENT.Receive ( res );
        if ( ( Encoding.Default.GetString ( res , 0 , res_len ) ).Contains ( "/*-OkayDown-*/\0" ) )
        {
            CLIENT.Shutdown ( SocketShutdown.Both );
            CLIENT.Close ( );
            return "用戶端關閉";
        }
        return "用戶端關閉失敗";
    }
}

