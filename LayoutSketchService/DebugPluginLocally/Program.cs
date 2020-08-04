using System;
using Inventor;
using System.IO;

namespace DebugPluginLocally
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var inv = new InventorConnector())
            {
                InventorServer server = inv.GetInventorServer();

                try
                {
                    Console.WriteLine("Running locally...");
                    // run the plugin
                    DebugSamplePlugin(server);
                }
                catch (Exception e)
                {
                    string message = $"Exception: {e.Message}";
                    if (e.InnerException != null)
                        message += $"{System.Environment.NewLine}    Inner exception: {e.InnerException.Message}";

                    Console.WriteLine(message);
                }
                finally
                {
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        Console.WriteLine("Press any key to exit. All documents will be closed.");
                        Console.ReadKey();
                    }
                }
            }
        }

      
        /// <param name="app"></param>
        private static void DebugSamplePlugin(InventorServer app)
        {
           
            //NOTE:Posible enhancement: Create a copy of the main ipt file first and work with that

            // open rail-layout-copy.ipt by Inventor
            //Document doc = app.Documents.Open(sketchPath);

            // create a name value map
            Inventor.NameValueMap map = app.TransientObjects.CreateNameValueMap();
            //map.Add("_1", inputParamsPath);

            // create an instance of LayoutSketchServicePlugin
            LayoutSketchServicePlugin.SampleAutomation plugin = new LayoutSketchServicePlugin.SampleAutomation(app);

            // run the plugin
            plugin.RunWithArguments(null, map);

        }
    }
}
