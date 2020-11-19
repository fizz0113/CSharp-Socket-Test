using System;
using System.Collections.Generic;
using System.Security;
using System.Threading;


class Th
{

    #region 全域變數

    /*鎖定*/
    private object LOCK_thlist = new object();

    /*執行緒清單*/
    private Dictionary<string, Thread> thread_list = new Dictionary<string, Thread>();

    /*取得使用者清單*/
    public Dictionary<string , Thread> th_list
    {
        get
        {
            lock ( LOCK_thlist )
            {
                return thread_list;
            }
        }
    }

    /*中斷所有執行緒*/
    public string exit
    {
        get => ExitAllThread ( );
    }

    #endregion

    #region 啟動執行緒不用傳參
    public string start ( string ThreadName , ThreadStart Function )
    {
        try
        {
            lock ( LOCK_thlist )
            {
                thread_list.Add ( ThreadName , new Thread ( Function )
                {
                    Name = ThreadName ,
                    IsBackground = true
                } );
            }
            thread_list [ ThreadName ].Start ( );
            return "\"" + ThreadName + "\"" + " 執行開始 !!";
        }
        catch ( OutOfMemoryException )
        {
            return "記憶體不足 ! " + "\"" + ThreadName + "\"  未能建立 !!";
        }
        catch ( ArgumentException )
        {
            return "有相同名稱 ! " + "\"" + ThreadName + "\"  未能建立 !!";
        }
        catch ( Exception e )
        {
            return "未知錯誤 : " + e;
        }
    }
    #endregion

    #region 啟動執行緒傳參
    public string start ( string ThreadName , ParameterizedThreadStart Function , object Value )
    {
        try
        {
            lock ( LOCK_thlist )
            {
                thread_list.Add ( ThreadName , new Thread ( Function )
                {
                    Name = ThreadName ,
                    IsBackground = true
                } );
            }
            thread_list [ ThreadName ].Start ( Value );
            return "\"" + ThreadName + "\"" + " 執行開始 !!";
        }
        catch ( OutOfMemoryException )
        {
            return "記憶體不足 ! " + "\"" + ThreadName + "\"  未能建立 !!";
        }
        catch ( ArgumentException )
        {
            return "有相同名稱 ! " + "\"" + ThreadName + "\"  未能建立 !!";
        }
        catch ( Exception e )
        {
            return "未知錯誤 : " + e;
        }
    }
    #endregion

    #region 等待某執行緒
    public string join ( string WaintThreadName )
    {
        try
        {
            if ( th_list.ContainsKey ( WaintThreadName ) )
            {
                th_list [ WaintThreadName ].Join ( );
                return "等待 \"" + WaintThreadName + "\" 成功 !!";
            }
            else
            {
                return "執行緒等待錯誤 : 無\"" + WaintThreadName + "\"執行緒 !!";
            }
        }
        catch ( ThreadInterruptedException )
        {
            return "執行緒等待錯誤 : \"" + WaintThreadName + "\"被中斷 !!";
        }
        catch ( ThreadStateException )
        {
            return "執行緒等待錯誤 : \"" + WaintThreadName + "\"尚未被調用 !!";
        }
        catch ( Exception e )
        {
            return "未知錯誤 : " + e;
        }
    }
    #endregion

    #region 中斷指定執行緒
    public string stop ( string ThreadName )
    {
        try
        {
            if ( th_list.ContainsKey ( ThreadName ) )
            {
                Thread t = th_list [ ThreadName ];
                lock ( LOCK_thlist )
                {
                    thread_list.Remove ( ThreadName );
                }
                t.Abort ( );
                t.DisableComObjectEagerCleanup ( );
                GC.Collect ( );
                GC.WaitForPendingFinalizers ( );
                return ThreadName + " 已中斷 !!";
            }
            else
            {
                return "中斷執行緒錯誤 : 無\"" + ThreadName + "\" 執行緒!!";
            }
        }
        catch ( SecurityException )
        {
            return "權限不足!!";
        }
        catch ( Exception e )
        {
            return "未知錯誤 : " + e;
        }
    }
    #endregion

    #region 中斷所有執行緒
    private string ExitAllThread ( )
    {
        lock ( LOCK_thlist )
        {
            try
            {
                foreach ( string s in thread_list.Keys )
                {
                    if ( thread_list [ s ].IsAlive )
                    {
                        thread_list [ s ].Abort ( );
                        thread_list [ s ].DisableComObjectEagerCleanup ( );
                    }
                }
                thread_list.Clear ( );
                GC.Collect ( );
                GC.WaitForPendingFinalizers ( );
                return "所有執行緒已關閉!";
            }
            catch ( Exception e )
            {
                return "未知錯誤 : " + e;
            }
        }
    }
    #endregion

    ~Th ( )
    {
        GC.Collect ( );
        GC.WaitForPendingFinalizers ( );
    }
}

