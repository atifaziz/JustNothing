#region Copyright 2018 Atif Aziz. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

namespace Optuple.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static OptionModule;

    static partial class EnumerableExtensions
    {
        public static IEnumerable<T> Filter<T>(this IEnumerable<(bool HasValue, T Value)> options)
            => options == null ? throw new ArgumentNullException(nameof(options))
             : options.SelectMany(Option.ToEnumerable);

        public static (bool HasValue, List<T> List)
            ListAll<T>(this IEnumerable<(bool HasValue, T Value)> options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            var list = new List<T>();
            foreach (var (some, item) in options)
            {
                if (!some)
                    return default;
                list.Add(item);
            }
            return Some(list);
        }

        public static (bool HasValue, T Value)
            SingleOrNone<T>(this IEnumerable<T> source)
        {
            switch (source)
            {
                case null: throw new ArgumentNullException(nameof(source));
                case IList<T> list: return list.Count == 1 ? Some(list[0]) : default;
                case IReadOnlyList<T> list: return list.Count == 1 ? Some(list[0]) : default;
                default:
                {
                    using (var e = source.GetEnumerator())
                    {
                        var item = e.MoveNext() ? Some(e.Current) : default;
                        return !e.MoveNext() ? item : default;
                    }
                }
            }
        }

        public static (bool HasValue, T Value)
            SingleOrNone<T>(this IEnumerable<T> source,
                          Func<T, bool> predicate) =>
            source.Where(predicate).SingleOrNone();

        public static (bool HasValue, T Value)
            FirstOrNone<T>(this IEnumerable<T> source)
        {
            switch (source)
            {
                case null: throw new ArgumentNullException(nameof(source));
                case IList<T> list: return list.Count > 0 ? Some(list[0]) : default;
                case IReadOnlyList<T> list: return list.Count > 0 ? Some(list[0]) : default;
                default:
                {
                    using (var e = source.GetEnumerator())
                        return e.MoveNext() ? Some(e.Current) : default;
                }
            }
        }

        public static (bool HasValue, T Value)
            FirstOrNone<T>(this IEnumerable<T> source,
                         Func<T, bool> predicate) =>
            source.Where(predicate).FirstOrNone();

        public static (bool HasValue, T Value)
            LastOrNone<T>(this IEnumerable<T> source)
        {
            switch (source)
            {
                case null: throw new ArgumentNullException(nameof(source));
                case IList<T> list: return list.Count > 0 ? Some(list[list.Count - 1]) : default;
                case IReadOnlyList<T> list: return list.Count > 0 ? Some(list[list.Count - 1]) : default;
                default:
                {
                    using (var e = source.GetEnumerator())
                    {
                        if (!e.MoveNext())
                            return default;

                        var last = e.Current;
                        while (e.MoveNext())
                            last = e.Current;
                        return Some(last);
                    }
                }
            }
        }

        public static (bool HasValue, T Value)
            LastOrNone<T>(this IEnumerable<T> source,
                         Func<T, bool> predicate) =>
            source.Where(predicate).LastOrNone();
    }
}
