using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Lab3
{
    class Program
    {
        static void Main(string[] args)
        {
            var array = new[]
            {
                new Elem
                {
                    Name = "Name0"
                },
                new Elem
                {
                    Name = "Name1"
                },
                new Elem
                {
                    Name = "Name2"
                },
                new Elem
                {
                    Name = "Name3"
                },
                new Elem
                {
                    Name = "Name4"
                },
                new Elem
                {
                    Name = "Name5"
                }
            };
            Console.WriteLine("Point 4");
            var point4 = new Point4();
            var concatResult = point4.Concat(array, 2, ",");
            Console.WriteLine(concatResult);
            Console.WriteLine(" ");

            Console.WriteLine("Point 5");
            var point5 = new Point5();
            var findResult = point5.Find(array);
            Console.WriteLine(String.Join(",", findResult.Select(e => e.Name)));
            Console.WriteLine(" ");

            Console.WriteLine("Point 6");
            var str = "Это что же получается: ходишь, ходишь в школу, а потом бац - вторая смена";
            var point6 = new Point6();
            var groups = point6.GroupByLength(str);

            foreach (var groupPoint6 in groups)
            {
                Console.WriteLine(groupPoint6.ToString());
            }
            Console.WriteLine(" ");

            Console.WriteLine("Point 7");
            var engStr = "This dog eats too much vegetables after lunch";
            var point7 = new Point7();
            var book = point7.Translate(engStr, 3);

            Console.WriteLine(book.ToString());
            Console.WriteLine("Program finished");

            Console.ReadKey();
        }
    }

    class Elem
    {
        public String Name { get; set; }
    }

    class Point4
    {
        public String Concat(IEnumerable<Elem> elems, Int32 skip, String delimiter)
        {
            if (elems == null)
            {
                throw new ArgumentException(nameof(elems));
            }

            if (skip < 0)
            {
                throw new ArgumentException(nameof(skip));
            }

            if (String.IsNullOrEmpty(delimiter))
            {
                throw new ArgumentException(nameof(delimiter));
            }

            return elems.Skip(skip)
                        .Aggregate(new StringBuilder(),
                                   (stringBuilder, item) => stringBuilder.Append($"{delimiter}{item.Name}"),
                                   stringBuilder => (stringBuilder.Length >= 2 ? stringBuilder.Remove(0, 1) : stringBuilder).ToString());
        }
    }

    class Point5
    {
        public IReadOnlyCollection<Elem> Find(IEnumerable<Elem> elems)
        {
            if (elems == null)
            {
                throw new ArgumentException(nameof(elems));
            }

            return elems.Where((elem, index) => elem.Name.Length > index)
                        .ToArray();
        }
    }

    class Point6
    {
        public IReadOnlyCollection<GroupPoint6> GroupByLength(String str)
        {
            if (String.IsNullOrEmpty(str))
            {
                throw new ArgumentException(nameof(str));
            }

            return (from splitItem in SplitStr(str)
                    where splitItem.Length > 0
                    group splitItem by splitItem.Length
                    into gr
                    select new
                    {
                        Length = gr.Key,
                        Values = gr.ToArray()
                    }).OrderByDescending(x => x.Length).Select((x, index) => new GroupPoint6
                    {
                        Number = index + 1,
                        Length = x.Length,
                        Count = x.Values.Length,
                        Values = x.Values
                    })
                .OrderByDescending(x => x.Count)
                .ToArray();
        }

        private IEnumerable<String> SplitStr(String str)
        {
            return Regex.Replace(str, "[-.?!)(,:]", String.Empty).Split(' ');
        }
    }

    class GroupPoint6
    {
        public Int32 Number { get; set; }

        public Int32 Length { get; set; }

        public Int32 Count { get; set; }

        public String[] Values { get; set; }

        public override String ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Группа {Number}. Длина {Length}. Количество {Count}");
            foreach (var groupPoint6Value in Values)
            {
                sb.AppendLine(groupPoint6Value);
            }

            return sb.ToString();
        }
    }

    class Point7
    {
        private IDictionary<String, String> _dictionary = new Dictionary<String, String>
        {
            {"this", "эта"},
            {"dog", "собака"},
            {"eats", "ест"},
            {"too", "слишком"},
            {"much", "много"},
            {"vegetables", "овощей"},
            {"after", "после"},
            {"lunch", "обеда"}
        };

        public Book Translate(String str, Int32 pageSize)
        {
            if (String.IsNullOrEmpty(str))
            {
                throw new ArgumentException(nameof(str));
            }

            if (pageSize <= 0)
            {
                throw new ArgumentException(nameof(pageSize));
            }

            return new Book
            {
                Pages = str.Split(' ')
                    .Select(x => x.ToLower())
                    .Join(_dictionary,
                        strItem => strItem,
                        dictPair => dictPair.Key,
                        (strItem, dictPair) => dictPair.Value.ToUpper())
                    .Select((word, index) => new
                    {
                        Index = index,
                        Word = word
                    })
                    .GroupBy(x => x.Index / pageSize, x => x.Word)
                    .Select(gr => new Page
                    {
                        Words = gr.ToArray()
                    }).ToArray()
            };
        }
    }

    class Book
    {
        public Page[] Pages { get; set; }

        public override String ToString()
        {
            return String.Join("\n", Pages.Select(p => p.ToString()));
        }
    }

    class Page
    {
        public String[] Words { get; set; }

        public override String ToString()
        {
            return String.Join(" ", Words);
        }
    }
}

