using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventor;
using Newtonsoft.Json.Linq;

namespace LayoutSketchServicePlugin
{
    class sketchParameterReader
    {
        readonly dynamic document;
        readonly string folderPath;
        //constructor
        public sketchParameterReader(Document document, string folder)
        {
            this.document = document;
            this.folderPath = folder;

        }

        public string ToJsonString()
        {
            var jsonRoot = new JObject();
            var jsonParams = new JArray();
            var sketchParameters = document.ComponentDefinition.Parameters;
            foreach(Parameter param in sketchParameters)
            {
                var jsonParam = new JObject();
                jsonParam.Add("name", param.Name);
                jsonParam.Add("value", param.Expression);
                jsonParams.Add(jsonParam);
                
            }
            
            jsonRoot.Add("sketchParameters", jsonParams);
            return jsonRoot.ToString();
            
        }
    }
}
