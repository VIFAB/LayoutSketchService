using System.Collections.Generic;
using Autodesk.Forge.DesignAutomation.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Interaction
{
    /// <summary>
    /// Customizable part of Publisher class.
    /// </summary>
    internal partial class Publisher
    {
        /// <summary>
        /// Constants.
        /// </summary>
        private static class Constants
        {
            private const int EngineVersion = 2021;
            public static readonly string Engine = $"Autodesk.Inventor+{EngineVersion}";

            public const string Description = "PUT DESCRIPTION HERE";

            internal static class Bundle
            {
                public static readonly string Id = "LayoutSketchService";
                public const string Label = "alpha";

                public static readonly AppBundle Definition = new AppBundle
                {
                    Engine = Engine,
                    Id = Id,
                    Description = Description
                };
            }

            internal static class Activity
            {
                public static readonly string Id = Bundle.Id;
                public const string Label = Bundle.Label;
            }

            internal static class Parameters
            {
                public const string InventorDoc = nameof(InventorDoc);
                public const string JsonParams = nameof(JsonParams);
                public const string OutputJson = nameof(OutputJson);
            }
        }


        /// <summary>
        /// Get command line for activity.
        /// </summary>
        private static List<string> GetActivityCommandLine()
        {
            return new List<string> { $"$(engine.path)\\InventorCoreConsole.exe /al $(appbundles[{Constants.Activity.Id}].path)" };
        }

        /// <summary>
        /// Get activity parameters.
        /// </summary>
        private static Dictionary<string, Parameter> GetActivityParams()
        {
            return new Dictionary<string, Parameter>
                    {
                        {
                            Constants.Parameters.InventorDoc,
                            new Parameter
                            {
                                Verb = Verb.Get,
                                Zip = false,
                                LocalName = "sketchLayout.ipt",
                                Description = "sketchLayout"
                            }
                        },
                        {
                            Constants.Parameters.JsonParams,
                            new Parameter
                            {
                                Verb = Verb.Get,
                                Zip = false,
                                Description = "JSON Object containing parameters to update",
                                LocalName = "JsonParameters"
                            }
                        },
                        {
                            Constants.Parameters.OutputJson,
                            new Parameter
                            {
                                Verb = Verb.Put,
                                LocalName = "result.json",
                                Description = "Resulting JSON Object",
                                Ondemand = false,
                                Required = false
                            }
                        }
                    };
        }

        /// <summary>
        /// Get arguments for workitem.
        /// </summary>
        private static Dictionary<string, IArgument> GetWorkItemArgs()
        {


            // TODO: update the URLs below with real values
            return new Dictionary<string, IArgument>
                    {
                         {    
                             Constants.Parameters.InventorDoc,
                             new XrefTreeArgument
                             {

                                 Url = string.Format("https://developer.api.autodesk.com/oss/v2/buckets/{0}/objects/{1}", "layoutsketchservice", "rail-layout-copy.ipt"),
                                 Headers = new Dictionary<string, string>()
                                 {
                                     { "Authorization", "Bearer " + InternalToken.access_token }
                                 }
                             }
                        },
                        {
                            Constants.Parameters.JsonParams,
                            new XrefTreeArgument
                            {
                                Verb = Verb.Get,
                                Url = "data:application/json,{\"RISE\":\"47\", \"RUN\":\"77\"}"
                            }
                        },
                        {
                            Constants.Parameters.OutputJson,
                            new XrefTreeArgument
                            {
                                Verb = Verb.Put,
                                Url = string.Format("https://developer.api.autodesk.com/oss/v2/buckets/{0}/objects/{1}", "layoutsketchservice", "result.json"),
                                Headers = new Dictionary<string, string>()
                                {
                                     { "Authorization", "Bearer " + InternalToken.access_token}
                                }
                            }
                        }
                    };
        }
    }
}
