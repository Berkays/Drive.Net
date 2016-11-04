using System;
using DriveNET;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(180, 30);
            Console.ForegroundColor = ConsoleColor.White;

            NetGDrive drive = new NetGDrive(LoggingEnabled: true);

            NetGSearchBuilder builder = new NetGSearchBuilder();

            builder.AddField(Field.modifiedTime).Smaller(DateTime.Now);

            for (int i = 0; i < 10; i++)
            {
                var files = drive.GetFiles(builder, 10);

                Console.WriteLine("\n");

                foreach (var file in files.Result)
                    Console.WriteLine(file.Name);
            }

            drive.Dispose();

            Console.WriteLine("\nFinished");
            Console.ReadKey();
        }


    }
}
