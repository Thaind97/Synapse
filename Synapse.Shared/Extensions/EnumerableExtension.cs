using System.ComponentModel;

namespace Synapse.Shared.Extensions
{
    public static class EnumerableExtension
    {
        [System.Diagnostics.DebuggerStepThrough]
        public static IEnumerable<T> Empty<T>()
        {
            return Enumerable.Empty<T>();
        }

        public static IEnumerable<T> Singleton<T>(T value)
        {
            return new[] { value };
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            return !source.EmptyIfNull().Any();
        }

        public static bool Any<T>(this IEnumerable<T> source, bool handleNull)
        {
            return source.EmptyIfNull().Any();
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> items)
        {
            return items ?? Enumerable.Empty<T>();
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static IEnumerable<TValue> OfValues<TValue>(params TValue[] values)
        {
            return values;
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static void Iter<TItem>(this IEnumerable<TItem> items, Action<TItem> action)
        {
            items.Iteri((item, _) => action(item));
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static void Iteri<TItem>(this IEnumerable<TItem> items, Action<TItem, int> action)
        {
            var index = 0;
            foreach (var item in items.EmptyIfNull())
                action(item, index++);
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static IEnumerable<TRes> Selecti<TItem, TRes>(this IEnumerable<TItem> items, Func<TItem, int, TRes> action)
        {
            var index = 0;
            foreach (var item in items.EmptyIfNull())
                yield return action(item, index++);
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> children)
        {
            return items.EmptyIfNull().SelectMany(item => EnumerableExtension.Singleton(item).Concat(children(item).Flatten(children)));
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static IList<T> AddEx<T>(this IEnumerable<T> items, T value)
        {
            var list = items.ToList();
            list.Add(value);

            return list;
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static IEnumerable<T> AddRangeEx<T>(this IEnumerable<T> items, IEnumerable<T> value)
        {
            var list = items.ToList();
            list.AddRange(value);
            return list;
        }

        public static List<List<T>> ChunkBy<T>(this IEnumerable<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static bool Contains<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            return !items.EmptyIfNull().Where(predicate).IsEmpty();
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static bool ContainsEx<T>(this IEnumerable<T> items, T value, StringComparison compareOption = StringComparison.CurrentCulture)
        {
            if (compareOption != StringComparison.CurrentCulture)
            {
                return items.Contains(i => i.ToString().Equals(value.ToString(), compareOption));
            }
            else
            {
                return items.Contains(i => i.Equals(value));
            }
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static bool ContainsIgnoreCase<T>(this IEnumerable<T> items, T value)
        {
            return items.Contains(i => i.ToString().Equals(value.ToString(), StringComparison.InvariantCultureIgnoreCase));
        }

        public static string GetEnumDescriptions<T>(this T e) where T : IConvertible
        {
            if (!(e is Enum))
                return string.Empty;

            var field = e.GetType().GetField(e.ToString());
            var customAttribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            var description = customAttribute.Length > 0 ? ((DescriptionAttribute)customAttribute[0]).Description : string.Empty;

            return description;
        }
    }
}
