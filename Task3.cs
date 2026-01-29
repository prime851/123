using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SchoolNTTasks
{
    public static class JsonReportBuilder
    {
        public static void BuildReport(string valuesPath, string testsPath, string reportPath)
        {
            using var valuesStream = File.OpenRead(valuesPath);
            using var testsStream = File.OpenRead(testsPath);
            
            var valuesDoc = JsonDocument.Parse(valuesStream);
            var testsNode = JsonNode.Parse(testsStream);
            
            var valueMap = BuildValueMap(valuesDoc);
            FillNodeValues(testsNode?["tests"], valueMap);
            
            var json = JsonSerializer.Serialize(testsNode, 
                new JsonSerializerOptions { WriteIndented = true });
            
            File.WriteAllText(reportPath, json);
        }

        private static Dictionary<int, string> BuildValueMap(JsonDocument doc)
        {
            var map = new Dictionary<int, string>();
            
            foreach (var element in doc.RootElement.GetProperty("values").EnumerateArray())
            {
                map[element.GetProperty("id").GetInt32()] = 
                    element.GetProperty("value").GetString();
            }
            
            return map;
        }

        private static void FillNodeValues(JsonNode node, Dictionary<int, string> valueMap)
        {
            if (node is JsonArray array)
            {
                foreach (var item in array)
                {
                    FillNodeValues(item, valueMap);
                }
            }
            else if (node is JsonObject obj)
            {
                if (obj.ContainsKey("id") && 
                    obj["id"]?.GetValue<int>() is int id && 
                    valueMap.TryGetValue(id, out var value))
                {
                    obj["value"] = value;
                }
                
                if (obj.ContainsKey("values") && obj["values"] is JsonNode valuesNode)
                {
                    FillNodeValues(valuesNode, valueMap);
                }
            }
        }
    }
}
