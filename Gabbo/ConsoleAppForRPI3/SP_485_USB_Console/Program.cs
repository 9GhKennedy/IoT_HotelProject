
using System.Threading.Tasks;
using System.IO.Ports;
using System;

namespace SP_485_USB_Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var comPort = "COM3";
            SerialPort port = new SerialPort(comPort, 9600, Parity.None, 8);
            try
            {
                
                Console.WriteLine("Hello seriale");
                bool flag = false;
                var dataSend = new char[9];
                dataSend[0]= 'P';
                dataSend[1]= '0';
                dataSend[2]= '1';
                dataSend[3]= 'R';
                dataSend[4]= '0';
                dataSend[5]= '1';
                dataSend[6]= '1';
                dataSend[7]= '3';
                dataSend[8]= 'X';


                var dataReceve = new char[9];
                port.Open();


                string obj = new string(dataSend);
                while (true)
                {
                    
                    await Task.Delay(500);
                    
                    Console.WriteLine($"messagem Send: {obj}");
                    port.Write(dataSend, 0, 9);
                    Console.WriteLine($"...");
                    while (flag == false)
                    {
                        
                        port.Read(dataReceve, 0, 9);
                        string varData = new string(dataReceve);

                        Console.WriteLine($"Data receve: {varData}");
                        
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
            }
            finally
            {
                port.Close();
            }
            


        }
    }
}
