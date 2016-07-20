using System;
using System.Net.Sockets;
using System.Text;

namespace Rabscuttle.networking {
    public class StateObject{
        public Socket workSocket = null;
        public const int BUFFER_SIZE = 1024;
        public byte[] buffer = new byte[BUFFER_SIZE];
        public StringBuilder sb = new StringBuilder();

        public static void Listen_Callback(IAsyncResult ar){
            // allDone.Set();
            Socket s = (Socket) ar.AsyncState;
            Socket s2 = s.EndAccept(ar);
            StateObject so2 = new StateObject();
            so2.workSocket = s2;
            s2.BeginReceive(so2.buffer, 0, StateObject.BUFFER_SIZE,0,
            new AsyncCallback(Read_Callback), so2);
        }

        public static void Read_Callback(IAsyncResult ar){
	        StateObject so = (StateObject) ar.AsyncState;
	        Socket s = so.workSocket;

	        int read = s.EndReceive(ar);

	        if (read > 0) {
                    so.sb.Append(Encoding.ASCII.GetString(so.buffer, 0, read));
                    s.BeginReceive(so.buffer, 0, StateObject.BUFFER_SIZE, 0,
            	                             new AsyncCallback(Read_Callback), so);
	        }
	        else{
	             if (so.sb.Length > 1) {
	                  //All of the data has been read, so displays it to the console
	                  string strContent;
	                  strContent = so.sb.ToString();
	                  Console.WriteLine(String.Format("Read {0} byte from socket" +
	          	                       "data = {1} ", strContent.Length, strContent));
	             }
	             s.Close();
	        }
        }
    }
}
