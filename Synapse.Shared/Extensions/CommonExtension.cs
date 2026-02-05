using Microsoft.Extensions.Configuration;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Synapse.Shared.Extensions
{
    public static class CommonExtension
    {
        public static TModel GetOptions<TModel>(this IConfiguration configuration, string section) where TModel : new()
        {
            var model = new TModel();
            configuration.GetSection(section).Bind(model);
            return model;
        }

        public static T ConvertTo<T>(this string input)
        {
            try
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                return (T)converter.ConvertFromString(input);
            }
            catch (NotSupportedException)
            {
                return default;
            }
        }

        public static string GetGenericTypeName(this Type type)
        {
            string typeName;
            if (type.IsGenericType)
            {
                var genericTypes = string.Join(",", type.GetGenericArguments().Select(t => t.Name).ToArray());
                typeName = $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
            }
            else
            {
                typeName = type.Name;
            }

            return typeName;
        }

        public static int ToInt(this Enum value)
        {
            return Convert.ToInt32(value);
        }

        public static int ToInt(this uint value)
        {
            return Convert.ToInt32(value);
        }

        public static string ToJsonString(this JsonDocument jsonDocument)
        {
            return jsonDocument == null ? string.Empty : jsonDocument.RootElement.ToString();
        }

        public static string ToJsonString(this string input, string propertyName)
        {
            var inputs = input.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var values = inputs.Select(x => $"\"{propertyName}\": \"{x}\"");
            return $"{{{string.Join(",", values)}}}";
        }

        private static readonly JsonDocument _emptyJson = JsonDocument.Parse("{}");

        public static Guid SafeUpdate(this Guid entity, string request)
        {
            return !string.IsNullOrEmpty(request) ? new Guid(request) : entity;
        }

        public static Guid? SafeUpdate(this Guid? entity, string request)
        {
            return !string.IsNullOrEmpty(request) ? new Guid(request) : entity;
        }

        public static string SafeUpdate(this string entity, string request)
        {
            return request ?? entity;
        }

        public static T? SafeUpdate<T>(this T? entity, T? request) where T : struct
        {
            return request ?? entity;
        }

        public static T SafeUpdate<T>(this T entity, T request)
        {
            return request ?? entity;
        }

        public static double? SafeUpdate(this double? entity, double? request)
        {
            return request ?? entity;
        }

        public static decimal? SafeUpdate(this decimal? entity, double? request)
        {
            return request != null ? request.ToString().ConvertTo<decimal>() : entity;
        }

        public static Guid? SafeCreate(this string entity)
        {
            if (string.IsNullOrWhiteSpace(entity))
            {
                return null;
            }
            return new Guid(entity);
        }

        public static string CreateJsonField(string request)
        {
            return !string.IsNullOrEmpty(request) ? request : "{}";
        }

        public static Guid ToGuid(this string value)
        {
            return string.IsNullOrWhiteSpace(value) ? Guid.Empty : Guid.Parse(value);
        }

        public static Guid? NullToGuid(this string request)
        {
            return !string.IsNullOrEmpty(request) ? new Guid(request) : (Guid?)null;
        }

        public static long ToLong(this double request)
        {
            return request.ToString(CultureInfo.InvariantCulture).ConvertTo<long>();
        }

        public static string RemoveSignalture(this string inputText)
        {
            if (string.IsNullOrEmpty(inputText))
            {
                return string.Empty;
            }

            const int maxRow = 14;
            const int maxColumn = 18;
            const string word = "aAeEoOuUiIdDyY";

            var matrix = new string[maxRow, maxColumn];
            var replaceWords = new List<string>
            {
                "áàạảãâấầậẩẫăắằặẳẵ",
                "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
                "éèẹẻẽêếềệểễeeeeee",
                "ÉÈẸẺẼÊẾỀỆỂỄEEEEEE",
                "óòọỏõôốồộổỗơớờợởỡ",
                "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
                "úùụủũưứừựửữuuuuuu",
                "ÚÙỤỦŨƯỨỪỰỬỮUUUUUU",
                "íìịỉĩiiiiiiiiiiii",
                "ÍÌỊỈĨIIIIIIIIIIII",
                "đdddddddddddddddd",
                "ĐDDDDDDDDDDDDDDDD",
                "ýỳỵỷỹyyyyyyyyyyyy",
                "ÝỲỴỶỸYYYYYYYYYYYY",
            };

            for (var row = 0; row < maxRow; row++)
            {
                matrix[row, 0] = word.Substring(row, 1);
                for (var column = 1; column < maxColumn; column++)
                {
                    matrix[row, column] = replaceWords[row].Substring(column - 1, 1);
                }
            }

            var tmp = inputText;
            var result = string.Empty;

            for (var row = 0; row < maxRow; row++)
            {
                for (var column = 1; column < maxColumn; column++)
                {
                    result = tmp.Replace(matrix[row, column], matrix[row, 0]);
                    tmp = result;
                }
            }

            return result;
        }

        public static JsonDocument ToJsonDocument(this string jsonString)
        {
            return jsonString != null ? JsonDocument.Parse(jsonString) : _emptyJson;
        }

        public static JsonDocument ToJsonDocument(this string input, string propertyName)
        {
            var jsonText = input.ToJsonString(propertyName);
            return jsonText.ToJsonDocument();
        }

        public static List<Guid> ToListGuid(this string value, string delimiter = ",")
        {
            if (!string.IsNullOrEmpty(value))
            {
                return Array.ConvertAll(value.Split(delimiter), s => new Guid(s)).ToList();
            }
            throw new ArgumentException("Value cannot be null");
        }

        public static T GetValueFromDescription<T>(this string description)
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new InvalidOperationException();
            }

            foreach (var field in type.GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description.RemoveSignalture()
                        .Equals(description, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return (T)field.GetValue(null);
                    }
                }
                else
                {
                    if (field.Name.RemoveSignalture().Equals(description, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return (T)field.GetValue(null);
                    }
                }
            }

            throw new ArgumentException("Not found. ", description);
        }

        public static string GetDescription<T>(this T enumerationValue) where T : struct
        {
            var type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException($"{nameof(enumerationValue)} must be of Enum type", nameof(enumerationValue));
            }
            var memberInfo = type.GetMember(enumerationValue.ToString());

            if (memberInfo.Length <= 0)
            {
                return enumerationValue.ToString();
            }

            var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attrs.Length > 0 ? ((DescriptionAttribute)attrs[0]).Description : enumerationValue.ToString();
        }

        public static T ConvertToEnum<T>(this string entity) where T : struct
        {
            return (T)Enum.Parse(typeof(T), entity, true);
        }

        public static bool IsValidGuid(this string inputString, out Guid result)
        {
            return Guid.TryParse(inputString, out result);
        }

        public static T SafeConvertToEnum<T>(this T entity)
        {
            return (T)Enum.Parse(typeof(T), entity?.ToString()!, true);
        }

        public static IEnumerable<string> SplitBy(this string str, int chunkLength)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentException(nameof(str));
            }

            if (chunkLength < 1)
            {
                throw new ArgumentException(nameof(chunkLength));
            }

            return SplitByIterator(str, chunkLength);
        }

        private static IEnumerable<string> SplitByIterator(string str, int chunkLength)
        {
            for (var i = 0; i < str.Length; i += chunkLength)
            {
                if (chunkLength + i > str.Length)
                {
                    chunkLength = str.Length - i;
                }

                yield return str.Substring(i, chunkLength);
            }
        }

        public static bool EqualsOrdinalIgnoreCase(this string text, string other)
        {
            return text.Equals(other, StringComparison.OrdinalIgnoreCase);
        }

        public static double? ToDouble(this decimal? value)
        {
            return value.HasValue ? decimal.ToDouble(value.Value) : (double?)null;
        }

        public static decimal ToDecimal(this double request)
        {
            return request.ToString(CultureInfo.InvariantCulture).ConvertTo<decimal>();
        }

        public static decimal? ToDecimal(this double? value)
        {
            return value.HasValue ? Convert.ToDecimal(value.Value) : (decimal?)null;
        }

        public static IEnumerable<List<T>> SplitBySize<T>(this List<T> items, int chunkSize = 30)
        {
            for (var i = 0; i < items.Count; i += chunkSize)
            {
                yield return items.GetRange(i, Math.Min(chunkSize, items.Count - i));
            }
        }

        public static IEnumerable<IEnumerable<T>> SplitByPart<T>(this IEnumerable<T> items, int numberOfParts)
        {
            var i = 0;
            return items.GroupBy(x => i++ % numberOfParts);
        }

        public static string GenerateSlug(this string phrase)
        {
            var str = phrase.RemoveSignalture().ToLower();
            // invalid chars
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim
            str = str.Substring(0, str.Length <= 100 ? str.Length : 100).Trim();
            str = Regex.Replace(str, @"\s", "-"); // hyphens
            return str;
        }

        public static string ToBase64String(this Stream stream)
        {
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            var bytes = memoryStream.ToArray();
            return Convert.ToBase64String(bytes);
        }

        public static string ToVnLowerCase(this string inputText)
        {
            if (string.IsNullOrEmpty(inputText))
            {
                return string.Empty;
            }

            const string lowerCharacters = "áàạảãâấầậẩẫăắằặẳẵéèẹẻẽêếềệểễóòọỏõôốồộổỗơớờợởỡúùụủũưứừựửữíìịỉĩđýỳỵỷỹ";
            const string upperCharacters = "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴÉÈẸẺẼÊẾỀỆỂỄÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠÚÙỤỦŨƯỨỪỰỬỮÍÌỊỈĨĐÝỲỴỶỸ";
            var result = inputText.ToLower();
            for (var i = 0; i < upperCharacters.Length; i++)
            {
                result = result.Replace(upperCharacters[i], lowerCharacters[i]);
            }

            return result;
        }

        public static async Task WhenAll(this IEnumerable<Task> source, int initialCount = 1, int degreeOfParallelism = 1)
        {
            var tasks = new List<Task>();
            using var throttler = new SemaphoreSlim(initialCount, degreeOfParallelism);

            try
            {
                await throttler.WaitAsync();

                tasks.AddRange(source);

                await Task.WhenAll(tasks);
            }
            finally
            {
                throttler?.Release();
            }
        }
    }
}
