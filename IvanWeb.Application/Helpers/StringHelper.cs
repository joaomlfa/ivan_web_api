using System.Globalization;
using System.Text;

namespace IvanWeb.Application.Helpers;

public static class StringHelper
{
    public static bool IsTolerableMatch(string expected, string actual)
    {
        var cleanExpected = Normalize(expected);
        var cleanActual = Normalize(actual);

        // Se bater 100% após limpar acentos e espaços, já retorna sucesso
        if (cleanExpected == cleanActual) return true;

        int distance = LevenshteinDistance(cleanExpected, cleanActual);

        // Regra de Tolerância justa: 
        // Permite 1 erro de digitação a cada 5 caracteres da resposta original (máximo de 3 erros)
        int maxTyposAllowed = Math.Min(cleanExpected.Length / 5, 3);

        return distance <= maxTyposAllowed;
    }

    // Remove acentos, espaços e pontuações, deixando só letras e números em minúsculo
    private static string Normalize(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;

        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark && char.IsLetterOrDigit(c))
            {
                stringBuilder.Append(char.ToLowerInvariant(c));
            }
        }
        return stringBuilder.ToString();
    }

    // Algoritmo matemático que calcula a diferença entre duas strings
    private static int LevenshteinDistance(string s, string t)
    {
        if (string.IsNullOrEmpty(s)) return string.IsNullOrEmpty(t) ? 0 : t.Length;
        if (string.IsNullOrEmpty(t)) return s.Length;

        int[] v0 = new int[t.Length + 1];
        int[] v1 = new int[t.Length + 1];

        for (int i = 0; i < v0.Length; i++) v0[i] = i;

        for (int i = 0; i < s.Length; i++)
        {
            v1[0] = i + 1;
            for (int j = 0; j < t.Length; j++)
            {
                int cost = (s[i] == t[j]) ? 0 : 1;
                v1[j + 1] = Math.Min(Math.Min(v1[j] + 1, v0[j + 1] + 1), v0[j] + cost);
            }
            for (int j = 0; j < v0.Length; j++) v0[j] = v1[j];
        }
        return v1[t.Length];
    }
}