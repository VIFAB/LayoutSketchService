/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Partner Development
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Inventor;
using Autodesk.Forge.DesignAutomation.Inventor.Utils;
using System.Collections.Generic;

using Newtonsoft.Json;
using System.IO;

namespace LayoutSketchServicePlugin
{
    [ComVisible(true)]
    public class SampleAutomation
    {
        private readonly InventorServer inventorApplication;

        public SampleAutomation(InventorServer inventorApp)
        {
            inventorApplication = inventorApp;
        }

        public void Run(Document doc)
        {
            LogTrace("Run called ");
            RunWithArguments(doc, null);

        }

        public void RunWithArguments(Document doc, NameValueMap map)
        {
            LogTrace("Starting...");

            try
            {
                
                using (new HeartBeat())
                {


                    LogTrace("Getting project and current directory");
                    // get project directory & current directory
                    string projectdir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                    string currentDir = System.IO.Directory.GetCurrentDirectory();
                    //get inputParams.json absolute path
                    LogTrace("Getting inputParams.json path");
                    //Local debug
                    //string inputParamsPath = System.IO.Path.Combine(projectdir, @"inputfiles\", "inputParams.json");
                    //Forge
                    //string paramData = (string)map.Value[$"_1"];
                    //LogTrace($"Reading param data {paramData}");
                    string inputParamsPath = System.IO.Path.Combine(currentDir, "JsonParameters");

                    // open rail-layout-copy.ipt by Inventor
                    // get sketch file absolute path
                    LogTrace("Opening sketch layout ipt");

                    //Local debug
                    //string sketchPath = System.IO.Path.Combine(projectdir, @"inputFiles\", "rail-layout-copy.ipt");
                    //Forge
                    LogTrace("Current Dir" + currentDir);
                    LogTrace("Project Dir" + projectdir);
                    string sketchPath = System.IO.Path.Combine(currentDir, "sketchLayout.ipt");

                    Document document = inventorApplication.Documents.Open(sketchPath);
                    LogTrace("Processing " + document.FullFileName);

                    //Local debug
                    //Dictionary<string, string> parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(System.IO.File.ReadAllText(inputParamsPath));
                    //Forge
                    Dictionary<string, string> parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(System.IO.File.ReadAllText(inputParamsPath));

                    LogTrace("Reading json input parameters");
                    foreach (KeyValuePair<string, string> entry in parameters)
                    {
                        var paramName = entry.Key;
                        var paramValue = entry.Value;
                        LogTrace($" params: {paramName}, {paramValue}");
                        ChangeParam(document, paramName, paramValue);
                    }

                    document.Update();
                    
                    LogTrace("Document updated");
                    string resultDir = System.IO.Path.Combine(currentDir, "result");
                    System.IO.Directory.CreateDirectory(resultDir);
                    LogTrace("Reading updated parameters");
                    var reader = new sketchParameterReader(document, resultDir);
                    string json = reader.ToJsonString();
                    //Local Debug
                    //string jsonPath = System.IO.Path.Combine(resultDir, "outputSketchParameters.json");
                    string jsonPath = System.IO.Path.Combine(resultDir, "result.json");
                    LogTrace("Writing output json file");
                    System.IO.File.WriteAllText(jsonPath, json);
                    System.IO.File.ReadAllText(jsonPath);
                    
                    //Local debug
                    //document.SaveAs(resultDir + "/rail-layout-Risexx.ipt", false);
                    LogTrace("Closing document");
                    document.Close();
                }
                
            }
            catch (Exception e)
            {
                LogError("Processing failed. " + e.ToString());
            }


        }

        public void ChangeParam(Document doc, string paramName, string paramValue)
        {
            UserParameters userParams;

            if (doc.DocumentType == DocumentTypeEnum.kPartDocumentObject)
            {
                LogTrace("Part Document");
                PartComponentDefinition partComponentDef = ((PartDocument)doc).ComponentDefinition;
                Parameters docParams = partComponentDef.Parameters;
                userParams = docParams.UserParameters;
            }
           
            else
            {
                LogTrace("Unknown Document");
                // unsupported doc type, throw exception
                throw new Exception("Unsupported document type: " + doc.DocumentType.ToString());
            }

            using (new HeartBeat())
            {
                try
                {
                    LogTrace($"Setting {paramName} to {paramValue}");
                    UserParameter userParam = userParams[paramName];
                    userParam.Expression = paramValue;
                }
                catch (Exception e)
                {
                    LogError("Cannot update '{0}' parameter. ({1})", paramName, e.Message);
                }
            }
        }

        #region Logging utilities

        /// <summary>
        /// Log message with 'trace' log level.
        /// </summary>
        private static void LogTrace(string format, params object[] args)
        {
            Trace.TraceInformation(format, args);
        }

        /// <summary>
        /// Log message with 'trace' log level.
        /// </summary>
        private static void LogTrace(string message)
        {
            Trace.TraceInformation(message);
        }

        /// <summary>
        /// Log message with 'error' log level.
        /// </summary>
        private static void LogError(string format, params object[] args)
        {
            Trace.TraceError(format, args);
        }

        /// <summary>
        /// Log message with 'error' log level.
        /// </summary>
        private static void LogError(string message)
        {
            Trace.TraceError(message);
        }

        #endregion
    }
}