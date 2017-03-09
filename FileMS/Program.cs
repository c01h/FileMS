using System;

namespace FileMS
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = Args.Configuration.Configure<FileOp>();
            var help = new Args.Help.HelpProvider();
            var helpDoc = help.GenerateModelHelp(config);            
            Console.WriteLine(helpDoc.HelpText);
            foreach (var h in helpDoc.Members)
            {
                Console.WriteLine(h.HelpText);
            }
            var fileOp = config.CreateAndBind(args);
            if (fileOp.Action == null) {
                Console.WriteLine("action is null");
                return;
            }
            var method = fileOp.GetType().GetMethod(fileOp.Action);

            if (method == null) {
                Console.WriteLine("action not find");
                return;
            }
            method.Invoke(fileOp,null);
            //new FileMoveJoin(@"D:\mydisk320\avi\雾岛\mp4", "*.mp4", @"E:\file\名人\雾岛", "*.avi").Run();
        }
    }
}