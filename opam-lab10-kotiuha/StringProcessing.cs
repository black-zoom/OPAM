using System.Text;

public static class StringProcessor
{
    public static string NormalizeNaive(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        string s = text.Trim().ToLowerInvariant();
        while (s.Contains("  "))
        {
            s = s.Replace("  ", " ");
        }
        return s;
    }

    public static string NormalizeFast(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        var sb = new StringBuilder(text.Length);
        bool inSpace = true;

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (char.IsWhiteSpace(c))
            {
                if (!inSpace)
                {
                    sb.Append(' ');
                    inSpace = true;
                }
            }
            else
            {
                sb.Append(char.ToLowerInvariant(c));
                inSpace = false;
            }
        }

        if (sb.Length > 0 && sb[sb.Length - 1] == ' ')
        {
            sb.Length--;
        }
        return sb.ToString();
    }

    public static int CustomIndexOf(string text, string pat, out int comparisons)
    {
        comparisons = 0;
        if (string.IsNullOrEmpty(pat)) return 0;
        if (string.IsNullOrEmpty(text) || pat.Length > text.Length) return -1;

        for (int i = 0; i <= text.Length - pat.Length; i++)
        {
            bool match = true;
            for (int j = 0; j < pat.Length; j++)
            {
                comparisons++;
                if (text[i + j] != pat[j])
                {
                    match = false;
                    break;
                }
            }
            if (match) return i;
        }
        return -1;
    }
}
