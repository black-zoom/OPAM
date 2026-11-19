using System;

public class Item
{
    public int Weight;
    public int Value;
}

public static class KnapsackSolver
{
    public static int VisitedNodes { get; private set; }
    private static int bestVal = 0;

    public static int KnapsackGreedy(Item[] items, int capacity)
    {
        var sorted = new Item[items.Length];
        Array.Copy(items, sorted, items.Length);
        Array.Sort(sorted, (a, b) =>
        {
            double rA = (double)a.Value / a.Weight;
            double rB = (double)b.Value / b.Weight;
            return rB.CompareTo(rA);
        });

        int currentWeight = 0;
        int totalValue = 0;
        for (int i = 0; i < sorted.Length; i++)
        {
            if (currentWeight + sorted[i].Weight <= capacity)
            {
                currentWeight += sorted[i].Weight;
                totalValue += sorted[i].Value;
            }
        }
        return totalValue;
    }

    public static int KnapsackBruteForce(Item[] items, int capacity, bool usePruning)
    {
        bestVal = 0;
        VisitedNodes = 0;
        SolveRecursive(items, 0, 0, 0, capacity, usePruning);
        return bestVal;
    }

    private static void SolveRecursive(Item[] items, int idx, int curW, int curV, int capacity, bool usePruning)
    {
        VisitedNodes++;
        if (curW > capacity) return;
        if (idx == items.Length)
        {
            if (curV > bestVal) bestVal = curV;
            return;
        }
        if (usePruning)
        {
            int maxPossible = curV;
            for (int i = idx; i < items.Length; i++) maxPossible += items[i].Value;
            if (maxPossible <= bestVal) return;
        }
        SolveRecursive(items, idx + 1, curW + items[idx].Weight, curV + items[idx].Value, capacity, usePruning);
        SolveRecursive(items, idx + 1, curW, curV, capacity, usePruning);
    }
}
