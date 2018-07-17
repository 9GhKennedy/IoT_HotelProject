using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;

namespace _FinalSolution
{
    class Program
    {
        #region

        static int combo;
        static string myPiano="1";
        static int myPic_temp=1;
        static int myPic_door;
        
        #endregion

        #region PIC

        static string comPort = "COM3";
        static char[] dataR = new char[9];
        static char[] dataS = new char[9];
        static string risposta;
        static char[] vs = new char[1];
        static SerialPort port;
        static bool ticket;


        #endregion


        #region Cloud

        static string[] messageCloud= new string[4];
        static string stringConnection = "";
        #endregion


        static async Task Main(string[] args)
        {
            combo = 1;
            ticket = false;
            port = new SerialPort(comPort, 9600, Parity.None, 8, StopBits.One);
            port.Open();
            Console.WriteLine("Hello RPI3!");
            while (true)
            {
                switch (combo)
                {
                    case (0):
                        ReceiveData();
                        break;
                    case (1):
                        ResiveTemp();
                        break;
                    case (2):
                        break;
                    case (3):
                        break;
                    case (4):
                        break;
                }
                await Task.Delay(1000);
            }
        }


        static void ReceiveData()
        {
            var flag = true;

            while (flag)
            {
               

                port.DataReceived += (s, e) =>
                {
                   port.Read(dataR, 0, dataR.Length);
                   risposta = new string(dataR);
                   Console.WriteLine($": {risposta}");
                    
                   ticket = true;

                };

                if (ticket) continue;

                Task.Delay(500);

                if ((ticket == true) && (risposta == "010112345"))
                {
                    vs[0] = '4';
                    port.Write(vs, 0, vs.Length);
                    Console.Write($"Giusto : {risposta}");
                    combo = 1;
                    flag = false;
                }

                if ((ticket == true) && (risposta != "010112345"))
                {
                    vs[0] = '0';
                    port.Write(vs, 0, vs.Length);
                    Console.Write($"Sbagliato : {risposta}");
                    
                }
                Task.Delay(500);
                
                ticket = false;
                
               
                
            }
        }

        static void ResiveTemp()
        {
            //using (var conn = new SqlConnection(stringConnection))
            //{
                
            //    try {
            //        conn.Open();
            //        var cmd = conn.CreateCommand();
            //        var deviceName = "P0" + "1" + "R0" + myPiano;
            //        cmd.CommandText = "SELECT TOP 1 Dev_name, Set_room_temp, Timestamp, Flag  FROM RoomChange WHERE Dev_name = '" + deviceName + "' ORDER BY (Timestamp) ";
            //        SqlDataReader dataReader = cmd.ExecuteReader();
            //        while (dataReader.Read())
            //        {
            //            messageCloud[0] = dataReader.GetString(0);
            //            messageCloud[1] = dataReader.GetInt32(1).ToString();
            //            messageCloud[2] = dataReader.GetDateTime(2).ToString();
            //            messageCloud[3] = dataReader.GetString(3);
            //        }
                    
            //    }
            //    catch (Exception ex)
            //    {
            //        //Console.WriteLine(ex);
            //        messageCloud[0] = "P01R01";
            //        messageCloud[1] = "33";
            //        messageCloud[2] = "ccccc";
            //        messageCloud[3] = "0";
            //    }

            //}
                
            //char[] valueToSend;
            // try { 
            //    valueToSend = messageCloud[1].ToCharArray();
            //}
            //catch
            //{
            //    valueToSend = new char[]{'0','0' };
            //}
         
            //string valueT ="P01R01" + messageCloud[1] + "X";
            //char[] messageS ={ 'P', '0', '1', 'R','0', '1', valueToSend[0], valueToSend[1] };
            //Console.WriteLine($"messaggio: {valueT}");
            //port.WriteLine(valueT);
            //port.Write(messageS, 0, 8);
           

            port.DataReceived += (s, e) =>
            {
                port.Read(dataR, 0, dataR.Length);
                risposta = new string(dataR);
                Console.WriteLine($": {risposta}");
                if (risposta.Length == 9)
               
                

                Task.Delay(1000);
                if (ticket == true)
                {
                     using (var conn = new SqlConnection(stringConnection))
                     {
                         try
                         {
                             conn.Open();
                             var cmd = conn.CreateCommand();
                             string deviceName = risposta.Substring(0,6);
                             char[] Devname = risposta.Substring(0, 5).ToCharArray();
                             var Set_room_temp = risposta.Substring(6, 2);
                             var User = "User";
                             var StatusDoor = risposta.Substring(8, 1);
                             var Timestamp = DateTime.Now.ToLocalTime();
                             Console.WriteLine("INSERT INTO RoomValue (Dev_name, Room_temp, Status_door, [Timestamp]) VALUES ('" + deviceName + "', '" + Set_room_temp + "', '" + StatusDoor + "', '" + Timestamp + "')");
                             cmd.CommandText = "INSERT INTO RoomValue (Dev_name, Room_temp, Status_door, [Timestamp]) VALUES ('" + deviceName + "', '" + Set_room_temp + "', '" + StatusDoor + "', '" + Timestamp + "')";

                             SqlDataReader dataReader = cmd.ExecuteReader();

                         }
                         catch (Exception ex)
                         {
                             Console.WriteLine(ex);
                         }

                     } 
                }

            };

            Task.Delay(9000);
                
                

        }

       
    }
}
