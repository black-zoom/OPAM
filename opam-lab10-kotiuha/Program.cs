using System;
using System.Diagnostics;
using System.Text;
using System.IO;

class Program
{
    static TimeSpan Measure(Action action)
    {
        action();
        var minTime = TimeSpan.MaxValue;
        var sw = new Stopwatch();

        for (int i = 0; i < 3; i++)
        {
            sw.Restart();
            action();
            sw.Stop();
            if (sw.Elapsed < minTime) minTime = sw.Elapsed;
        }
        return minTime;
    }

    static void Task1()
    {
        Console.WriteLine("--- Task 1 ---");
        int iters = 10000;
        var t1 = Measure(() => { string s = ""; for (int i = 0; i < iters; i++) s += "A"; });
        var t2 = Measure(() => { var sb = new StringBuilder(); for (int i = 0; i < iters; i++) sb.Append("A"); });
        Console.WriteLine($"T1 (+): {t1.TotalMilliseconds} ms | StringBuilder: {t2.TotalMilliseconds} ms");

        string a = "TestString", b = "teststring";
        var t3 = Measure(() => { for (int i = 0; i < 100000; i++) { bool r = a.ToLower() == b.ToLower(); } });
        var t4 = Measure(() => { for (int i = 0; i < 100000; i++) { bool r = string.Equals(a, b, StringComparison.OrdinalIgnoreCase); } });
        Console.WriteLine($"T2 (ToLower): {t3.TotalMilliseconds} ms | OrdinalIgnoreCase: {t4.TotalMilliseconds} ms");

        string[] words1 = { "ґанок", "галас", "ґудзик", "гуляш" };
        string[] words2 = { "ґанок", "галас", "ґудзик", "гуляш" };
        Array.Sort(words1, StringComparer.Ordinal);
        Array.Sort(words2, StringComparer.CurrentCulture);
        Console.WriteLine($"T3 Ordinal: {string.Join(", ", words1)}");
        Console.WriteLine($"T3 Cultural: {string.Join(", ", words2)}");
    }

    static void Task2(string text)
    {
        Console.WriteLine("\n--- Task 2 ---");
        int[] sizes = { 1000, 10000, 50000 };
        foreach (int size in sizes)
        {
            if (size > text.Length) break;
            string sub = text.Substring(0, size);
            var tNaive = Measure(() => StringProcessor.NormalizeNaive(sub));
            var tFast = Measure(() => StringProcessor.NormalizeFast(sub));
            Console.WriteLine($"Size {size}: Naive = {tNaive.TotalMilliseconds:F4} ms | Fast = {tFast.TotalMilliseconds:F4} ms | Factor = {tNaive.TotalMilliseconds / Math.Max(tFast.TotalMilliseconds, 0.0001):F2}");
        }
    }

    static void Task3(string text)
    {
        Console.WriteLine("\n--- Task 3 ---");
        string t50 = text.Length > 50000 ? text.Substring(0, 50000) : text;
        string wMissingStart = "якогонемає" + t50.Substring(0, 10);
        string wMissingFull = "цьогословатутточнонемає";
        string repeatsText = new string('A', 10000);
        string patRepeat = new string('A', 909) + "Б";

        int c1, c2, c3;
        var t1 = Measure(() => StringProcessor.CustomIndexOf(t50, wMissingStart, out c1));
        var t2 = Measure(() => StringProcessor.CustomIndexOf(t50, wMissingFull, out c2));
        var t3 = Measure(() => StringProcessor.CustomIndexOf(repeatsText, patRepeat, out c3));

        var ti1 = Measure(() => t50.IndexOf(wMissingStart, StringComparison.Ordinal));
        var ti2 = Measure(() => t50.IndexOf(wMissingFull, StringComparison.Ordinal));
        var ti3 = Measure(() => repeatsText.IndexOf(patRepeat, StringComparison.Ordinal));

        StringProcessor.CustomIndexOf(t50, wMissingStart, out c1);
        StringProcessor.CustomIndexOf(t50, wMissingFull, out c2);
        StringProcessor.CustomIndexOf(repeatsText, patRepeat, out c3);

        Console.WriteLine($"Prose/Start: Comp={c1}, My={t1.TotalMilliseconds:F4}ms, Sys={ti1.TotalMilliseconds:F4}ms");
        Console.WriteLine($"Prose/Missing: Comp={c2}, My={t2.TotalMilliseconds:F4}ms, Sys={ti2.TotalMilliseconds:F4}ms");
        Console.WriteLine($"Repeats/Trap: Comp={c3}, My={t3.TotalMilliseconds:F4}ms, Sys={ti3.TotalMilliseconds:F4}ms");
    }

    static void Task4_And_C()
    {
        Console.WriteLine("\n--- Task 4 & C ---");
        var rnd = new Random(14);
        int optCount = 0;
        double worstDiff = 0;
        string counterEx = "";
        long t15ms = 0, t20ms = 0;

        for (int i = 0; i < 100; i++)
        {
            int n = rnd.Next(5, 16);
            var items = new Item[n];
            for (int j = 0; j < n; j++) items[j] = new Item { Weight = rnd.Next(1, 6), Value = rnd.Next(1, 21) };

            int vGreedy = KnapsackSolver.KnapsackGreedy(items, 11);
            int vBrute = KnapsackSolver.KnapsackBruteForce(items, 11, false);

            if (vGreedy == vBrute) optCount++;
            else
            {
                double diff = (double)(vBrute - vGreedy) / vBrute * 100;
                if (diff > worstDiff) worstDiff = diff;
                if (string.IsNullOrEmpty(counterEx) || n < counterEx.Split(';').Length)
                {
                    var sb = new StringBuilder();
                    foreach (var it in items) sb.Append($"({it.Weight}kg,{it.Value}$) ");
                    counterEx = sb.ToString();
                }
            }

            if (i == 0)
            {
                KnapsackSolver.KnapsackBruteForce(items, 11, false);
                int vNo = KnapsackSolver.VisitedNodes;
                KnapsackSolver.KnapsackBruteForce(items, 11, true);
                int vYes = KnapsackSolver.VisitedNodes;
                Console.WriteLine($"Task C (Pruning): Without={vNo} branches, With={vYes} branches");
            }
        }

        var items15 = new Item[15];
        var items20 = new Item[20];
        for (int j = 0; j < 15; j++) items15[j] = new Item { Weight = rnd.Next(1, 6), Value = rnd.Next(1, 21) };
        for (int j = 0; j < 20; j++) items20[j] = new Item { Weight = rnd.Next(1, 6), Value = rnd.Next(1, 21) };

        var sw = Stopwatch.StartNew();
        KnapsackSolver.KnapsackBruteForce(items15, 11, true);
        t15ms = sw.ElapsedMilliseconds;
        sw.Restart();
        KnapsackSolver.KnapsackBruteForce(items20, 11, true);
        t20ms = sw.ElapsedMilliseconds;

        Console.WriteLine($"Optimums: {optCount}/100 | Worst diff: {worstDiff:F1}%");
        Console.WriteLine($"Smallest counter-example: {counterEx}");
        Console.WriteLine($"Time 15 elems: {t15ms} ms | 20 elems: {t20ms} ms");
    }

    static void RunEdgeCases()
    {
        Console.WriteLine("\n--- Edge Cases ---");
        Console.WriteLine($"T2 Empty: '{StringProcessor.NormalizeFast("")}' == '{StringProcessor.NormalizeNaive("")}'");
        Console.WriteLine($"T2 One char: '{StringProcessor.NormalizeFast("a")}' == '{StringProcessor.NormalizeNaive("a")}'");
        Console.WriteLine($"T2 All same: '{StringProcessor.NormalizeFast("   ")}' == '{StringProcessor.NormalizeNaive("   ")}'");
        Console.WriteLine($"T2 Spaces mix: '{StringProcessor.NormalizeFast("  a  b   c  ")}' == '{StringProcessor.NormalizeNaive("  a  b   c  ")}'");

        int c;
        Console.WriteLine($"T3 Pat > Text: {StringProcessor.CustomIndexOf("a", "aa", out c)}");
        Console.WriteLine($"T3 Empty Pat: {StringProcessor.CustomIndexOf("abc", "", out c)}");
        Console.WriteLine($"T4 0 elems: {KnapsackSolver.KnapsackBruteForce(new Item[0], 11, true)}");
        Console.WriteLine($"T4 1 elem (fit): {KnapsackSolver.KnapsackBruteForce(new Item[] { new Item { Weight = 5, Value = 10 } }, 11, true)}");
    }

    static void Main()
    {
        string prose = "це тестовий текст. ";
        if (File.Exists("prose.txt"))
        {
            prose = File.ReadAllText("prose.txt");
        }
        else
        {
            var sb = new StringBuilder();
            for (int i = 0; i < 3000; i++) sb.Append(prose);
            prose = sb.ToString();
        }

        Task1();
        Task2(prose);
        Task3(prose);
        Task4_And_C();
        RunEdgeCases();
    }
}
