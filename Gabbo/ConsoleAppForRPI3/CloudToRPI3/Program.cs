using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Ports;
using System.Net.Http;
using System.Threading.Tasks;

namespace CloudToRPI3
{

    static class Program
    {
        //-->Status Device<--//
        private static string myPic_Name;
        private static int myPic_temp;
        private static int myPic_door;

        //-->Element RPI3 config<--//
        private static int myPiano = 01;
        public static int myRoom = 9;
        private static int myDevSelect = 0;
        private static int[] old_Value = new int[myRoom];

        private static int comand=0;

        private static void clearRoom()
        {
            for(int i =0; i< myRoom; i++)
            {
                old_Value[i] = 0;
            }
        }

        private static async Task Main(string[] args)
        {
            

            comand = 0;
            clearRoom();
            Console.WriteLine("Hi! piano: " + myPiano);
            
            while (true) {
                switch (comand)
                {
                    case (0):
                        await ReciveData();
                        break;
                    case (1):
                        await SendOnPic();
                        break;
                    case (2):
                        break;
                }

                Task.Delay(500);
            }
            
        }

        private static async Task ReciveData()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();
            using (var conn = new SqlConnection(configuration["SqlConnectionssString"]))
            {
                try
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    var deviceName = "P0" + myDevSelect+"R0"+myPiano;
                    Console.WriteLine(deviceName);
                    cmd.CommandText = "SELECT TOP 1 Dev_name, Set_room_temp, Timestamp  FROM RoomChange WHERE Dev_name = '" + deviceName + "' ORDER BY (Timestamp) ";
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    int i = 0;
                    while (dataReader.Read())
                    {
                        
                        var dev_name = dataReader.GetString(0);
                        var set_room_temp = dataReader.GetInt32(1);
                        var set_Timestamp = dataReader.GetDateTime(2);

                        if (set_room_temp != old_Value[myDevSelect])
                        {
                            i++;
                            old_Value[myDevSelect] = set_room_temp;
                            myPic_temp = set_room_temp;
                        }
                    }
                    if (i == 0)
                    {
                        Console.WriteLine("valore già salvato ");
                    }
                    else
                    {
                        Console.WriteLine("valore da salvare ");
                    }
                    conn.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Errore ReciveData: "+ex);
                }
                comand = 1;
            }
        }

        private static async Task SendOnPic()
        {
            

            if (myDevSelect < 8)
            {
                Console.WriteLine(">_ " +myDevSelect + " temperatura da inviare " + myPic_temp );
                
                myPic_temp = 0;
                Console.WriteLine(" ");
                myDevSelect++;
            }
            else
            {
                myDevSelect = 0;
            }
            comand = 0;
        }

        


    }
}
