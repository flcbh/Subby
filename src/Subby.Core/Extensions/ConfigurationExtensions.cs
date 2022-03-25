using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Subby.Core.Entities;

namespace Subby.Core.Extensions
{
    public static class ConfigurationExtensions
    {
        public static string GetValueOrDefault(this IEnumerable<Configuration> configurations , string key)
        {
            return configurations?.Where(x => x.Key == key).FirstOrDefault()?.Value;
        }
        
        public static T GetOrDefault<T>(this IEnumerable<Configuration> configurations, string key, T? defaultValue = null) where T : struct
        {
            var values = configurations.Where(x => x.Key == key).GroupBy(x => x.Key).Select(g => new Configuration
            {
                Value = g.FirstOrDefault()?.Value,
                Key = g.FirstOrDefault()?.Key
                
            }).ToDictionary(
                y => y.Key,
                y => y.Value
            );
            
            var innerValues = new Dictionary<string, string>(values, StringComparer.OrdinalIgnoreCase);
            
            return innerValues.ContainsKey(key)
                ? (T)Convert.ChangeType(innerValues[key], typeof(T), CultureInfo.InvariantCulture)
                : defaultValue ?? default(T);
        }
    }
}