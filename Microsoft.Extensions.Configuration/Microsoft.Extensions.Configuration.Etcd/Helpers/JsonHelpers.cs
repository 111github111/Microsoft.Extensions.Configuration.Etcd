﻿using System.Text.Json;

namespace Microsoft.Extensions.Configuration.Etcd.Helpers
{
    public class JsonHelpers
    {
        /// <summary>提取 json 对象为 IEnumerable 集合</summary>
        /// <param name="jsonElement"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        /// <remarks>
        /// 代码来源：<br />
        /// https://github.com/Kralizek/AWSSecretsManagerConfigurationExtensions/blob/master/src/Kralizek.Extensions.Configuration.AWSSecretsManager/Internal/SecretsManagerConfigurationProvider.cs#L104-L154
        /// </remarks>
        public static IEnumerable<(string key, string value)> ExtractValues(JsonElement? jsonElement, string prefix)
        {
            if (jsonElement == null)
            {
                yield break;
            }
            var element = jsonElement.Value;
            switch (element.ValueKind)
            {
                case JsonValueKind.Array:
                    {
                        var currentIndex = 0;
                        foreach (var el in element.EnumerateArray())
                        {
                            var secretKey = $"{prefix}{ConfigurationPath.KeyDelimiter}{currentIndex}";
                            foreach (var (key, value) in ExtractValues(el, secretKey))
                            {
                                yield return (key, value);
                            }
                            currentIndex++;
                        }
                        break;
                    }
                case JsonValueKind.Number:
                    {
                        var value = element.GetRawText();
                        yield return (prefix, value);
                        break;
                    }
                case JsonValueKind.String:
                    {
                        var value = element.GetString() ?? "";
                        yield return (prefix, value);
                        break;
                    }
                case JsonValueKind.True:
                    {
                        var value = element.GetBoolean();
                        yield return (prefix, value.ToString());
                        break;
                    }
                case JsonValueKind.False:
                    {
                        var value = element.GetBoolean();
                        yield return (prefix, value.ToString());
                        break;
                    }
                case JsonValueKind.Object:
                    {
                        foreach (var property in element.EnumerateObject())
                        {
                            var secretKey = $"{prefix}{ConfigurationPath.KeyDelimiter}{property.Name}";
                            foreach (var (key, value) in ExtractValues(property.Value, secretKey))
                            {
                                yield return (key, value);
                            }
                        }
                        break;
                    }
                case JsonValueKind.Undefined:
                case JsonValueKind.Null:
                default:
                    {
                        throw new FormatException("unsupported json token");
                    }
            }
        }
    }
}