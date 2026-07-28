using System;
using System.Collections.Generic;
using System.Text.Json;

namespace RevitMcpAddin.Commands
{
    /// <summary>
    /// Values in RevitRequest.Params are deserialized by System.Text.Json into boxed JsonElement
    /// instances (since the declared type is object). These helpers pull typed values back out.
    /// </summary>
    internal static class ParamsExtensions
    {
        public static string GetString(this Dictionary<string, object> parameters, string key, string defaultValue = null)
        {
            if (parameters == null || !parameters.TryGetValue(key, out var value) || value == null)
                return defaultValue;
            return value is JsonElement element ? element.GetString() : value.ToString();
        }

        public static double GetDouble(this Dictionary<string, object> parameters, string key, double defaultValue = 0)
        {
            if (parameters == null || !parameters.TryGetValue(key, out var value) || value == null)
                return defaultValue;
            if (value is JsonElement element)
                return element.GetDouble();
            return Convert.ToDouble(value);
        }

        public static int GetInt(this Dictionary<string, object> parameters, string key, int defaultValue = 0)
        {
            if (parameters == null || !parameters.TryGetValue(key, out var value) || value == null)
                return defaultValue;
            if (value is JsonElement element)
                return element.GetInt32();
            return Convert.ToInt32(value);
        }

        public static bool Require(this Dictionary<string, object> parameters, string key)
        {
            return parameters != null && parameters.ContainsKey(key) && parameters[key] != null;
        }
    }
}
