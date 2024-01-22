using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructures
{
    public static class Extensions
    {
        public static T Extract<T>(this List<T> list, Random random)
        {
            return list.ExtractAt(random.Next(list.Count));
        }

        public static void Extract<T>(this List<T> list, T item)
        {
            list.ExtractWhere(i => i.Equals(item));
        }

        public static T ExtractAt<T>(this List<T> list, int i)
        {
            T result = list[i];
            list[i] = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return result;
        }

        public static IEnumerable<T> ExtractMany<T>(this List<T> list, int count, Random random)
        {
            for (int i = 0; i < count; i++)
            {
                yield return list.Extract(random);
            }
        }

        public static void ExtractWhere<T>(this List<T> list, Func<T, bool> predicate)
        {
            int i = 0;
            while (i < list.Count)
            {
                if (predicate(list[i]))
                {
                    list.ExtractAt(i);
                }
                else
                {
                    i++;
                }
            }
        }

        public static void Shuffle<T>(this IList<T> list, Random random)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int j = random.Next(list.Count - i);
                j = j + i;
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        public static void ApplyDistribution<TItem, TQuality>(this Dictionary<TItem, double> sizedItems, Dictionary<TQuality, double> distribution, Action<TItem, TQuality> setQuality)
        {
            Dictionary<TQuality, double> thresholds = new Dictionary<TQuality, double>();          

            double threshold = 0;
            foreach (TQuality quality in distribution.Keys)
            {
                threshold += distribution[quality];
                thresholds[quality] = threshold;
            }

            double factor = threshold / sizedItems.Values.Sum();

            List<TQuality> ordered = thresholds.Keys.OrderBy(q => thresholds[q]).ToList();

            double a = 0;
            foreach (TItem item in sizedItems.Keys)
            {
                a += factor * sizedItems[item];

                TQuality quality = a > threshold ? ordered.Last() : ordered.First(q => thresholds[q] >= a);

                setQuality(item, quality);
            }
        }


        public static Dictionary<TItem, TQuality> ApplyDistribution<TItem, TQuality>(this Dictionary<TItem, double> sizedItems, Dictionary<TQuality, double> distribution)
        {
            Dictionary<TItem, TQuality> result = new Dictionary<TItem, TQuality>();
            sizedItems.ApplyDistribution(distribution, (i, q) => result[i] = q);
            return result;
        }

        public static Dictionary<TItem, TQuality> ApplyDistribution<TItem, TQuality>(this IEnumerable<TItem> set, Dictionary<TQuality, double> distribution)
        {
            Dictionary<TItem, double> sizedItems = set.ToDictionary(i => i, i => 1.0);
            return sizedItems.ApplyDistribution(distribution);
        }

        public static Dictionary<TItem, TQuality> ApplyDistribution<TItem, TQuality>(this IEnumerable<TItem> set, Dictionary<TQuality, double> distribution, Random random)
        {
            List<TItem> list = set.ToList();
            list.Shuffle(random);
            return list.ApplyDistribution(distribution);
        }

        public static int NextSeed(this Random random) => random.Next(int.MinValue, int.MaxValue);

        public static IEnumerable<T> Select<T>(this int count, Func<int, T> selector) => Enumerable.Range(0, count).Select(selector);

        public static Dictionary<T, double> Normalize<T>(this Dictionary<T, double> dict)
        {
            Dictionary<T, double> result = new Dictionary<T, double>();
            double sum = dict.Sum(p => p.Value);

            foreach (T item in dict.Keys)
            {
                result[item] = dict[item] / sum;
            }
            return result;
        }

        public static T MinBy<T>(this IEnumerable<T> items, Func<T, double> func)
        {
            T result = items.First();
            double min = func(result);

            foreach (T item in items)
            {
                double val = func(item);
                if (val < min)
                {
                    result = item;
                    min = val;
                }
            }

            return result;
        }

        public static T MaxBy<T>(this IEnumerable<T> items, Func<T, double> func)
        {
            T result = items.First();
            double max = func(result);

            foreach (T item in items)
            {
                double val = func(item);
                if (val > max)
                {
                    result = item;
                    max = val;
                }
            }

            return result;
        }

        public static T Peek2<T>(this Stack<T> stack)
        {
            if (stack.Count < 2)
            {
                throw new Exception("Stack contains less than 2 elements");
            }

            T top = stack.Pop();
            T top2 = stack.Peek();
            stack.Push(top);
            return top2;
        }
    }

}
