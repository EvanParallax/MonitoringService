using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Lab3
{
    class Elem
    {
        public String Name { get; set; }
    }

    class Task6Formatter
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

    public class Book
    {
        public Page[] Pages { get; set; }

        public override String ToString()
        {
            return String.Join("\n", Pages.Select(p => p.ToString()));
        }
    }

    public class Page
    {
        public String[] Words { get; set; }

        public override String ToString()
        {
            return String.Join(" ", Words);
        }
    }

    class Program
    {
        private static IEnumerable<String> SplitStr(String str)
        {
            return Regex.Replace(str, "[-.?!)(,:]", String.Empty).Split(' ');
        }

        static void Task4()
        {
            Console.WriteLine("Task 4");
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

            string delimiter = ",";
            int skip = 3;

            var result = array.Skip(skip)
                .Aggregate(new StringBuilder(),
                    (stringBuilder, item) => stringBuilder.Append($"{delimiter}{item.Name}"),
                    stringBuilder => (stringBuilder.Length >= 2 ? stringBuilder.Remove(0, 1) : stringBuilder).ToString());

            Console.WriteLine(result);
            Console.WriteLine(" ");
        }

        static void Task5()
        {
            Console.WriteLine("Task 5");
            var array = new[]
            {
                new Elem
                {
                    Name = "Name0"
                },
                new Elem
                {
                    Name = "Name11"
                },
                new Elem
                {
                    Name = "Name222"
                },
                new Elem
                {
                    Name = "N3"
                },
                new Elem
                {
                    Name = "Name"
                },
                new Elem
                {
                    Name = "Name5"
                }
            };

            var result = array.Where((elem, index) => elem.Name.Length > index)
                .ToArray();

            Console.WriteLine(String.Join(",", result.Select(e => e.Name)));
            Console.WriteLine(" ");
        }

        static void Task6()
        {
            Console.WriteLine("Task 6");
            var str = "Это что же получается: ходишь, ходишь в школу, а потом бац - вторая смена";
            var result = (from splitItem in SplitStr(str)
                    where splitItem.Length > 0
                    group splitItem by splitItem.Length
                    into gr
                    select new
                    {
                        Length = gr.Key,
                        Values = gr.ToArray()
                    }).OrderByDescending(x => x.Length).Select((x, index) => new Task6Formatter()
                {
                    Number = index + 1,
                    Length = x.Length,
                    Count = x.Values.Length,
                    Values = x.Values
                })
                .OrderByDescending(x => x.Count)
                .ToArray();
            foreach (var item in result)
            {
                Console.WriteLine(item.ToString());
            }

            Console.WriteLine(" ");
        }

        static void Task7()
        {
            Console.WriteLine("Task 7");
            IDictionary<String, String> dictionary = new Dictionary<String, String>
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

            var engStr = "This dog eats too much vegetables after lunch";
            var result = new Book
            {
                Pages = engStr.Split(' ')
                    .Select(x => x.ToLower())
                    .Join(dictionary,
                        strItem => strItem,
                        dictPair => dictPair.Key,
                        (strItem, dictPair) => dictPair.Value.ToUpper())
                    .Select((word, index) => new
                    {
                        Index = index,
                        Word = word
                    })
                    .GroupBy(x => x.Index / 3, x => x.Word)
                    .Select(gr => new Page
                    {
                        Words = gr.ToArray()
                    }).ToArray()
            };
            Console.WriteLine(result.ToString());
            Console.WriteLine(" ");
        }

        static void Main(string[] args)
        {
            Task4();
            Task5();
            Task6();
            Task7();

            Console.WriteLine("Program finished");

            Console.ReadKey();
        }
    }
}

