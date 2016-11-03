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

            NetGDrive drive = new NetGDrive(LoggingEnabled:true);

            NetGSearchBuilder builder = new NetGSearchBuilder();

            builder.AddField(Field.modifiedTime).Smaller(DateTime.Now).And().PStart().AddField(Field.mimeType).Contains(MimeType.Image).Or().AddField(Field.mimeType).Contains(MimeType.Text).PEnd();

            var files = drive.GetFiles(builder.Search, 10);

            Console.WriteLine();

            foreach (var file in files)
                Console.WriteLine(file.Name);

            Console.WriteLine("\nFinished");
            Console.ReadKey();
        }


    }
}
