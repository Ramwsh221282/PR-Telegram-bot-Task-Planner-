using System.Text.RegularExpressions;

namespace RocketTaskPlanner.Utilities.StringUtilities;

/// <summary>
/// Utility extension методы для работы со строками
/// </summary>
public static partial class StringUtils
{
    /// <summary>
    /// Массив предлогов и союзов
    /// </summary>
    private static readonly string[] PrepositionsAndConjunctions =
    [
        "и",
        "в",
        "во",
        "не",
        "на",
        "с",
        "со",
        "к",
        "кто",
        "что",
        "как",
        "то",
        "из",
        "по",
        "у",
        "благодаря",
        "для",
        "а",
        "но",
        "если",
        "когда",
        "пока",
        "чтобы",
        "т. д.",
        "а",
        "или",
    ];

    private const RegexOptions Options =
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;

    /// <summary>
    /// Очистка строки от пунктуации, переносов строк, лишних пробелов
    /// </summary>
    /// <param name="input">Строка</param>
    /// <returns>
    /// строка, очищенная от пунктуации, переносов строк, лишних пробелов
    /// </returns>
    public static string CleanString(this string? input) =>
        string.IsNullOrWhiteSpace(input)
            ? string.Empty
            : input
                .CleanFromPunctuation()
                .CleanFromExtraSpaces()
                .CleanFromNewLines()
                .CleanFromExtraSpaces();

    /// <summary>
    /// Очистка строки от <inheritdoc cref="PrepositionsAndConjunctions"/>
    /// </summary>
    /// <param name="input">Строка</param>
    /// <returns>
    /// Строка, очищенная от <inheritdoc cref="PrepositionsAndConjunctions"/>
    /// </returns>
    public static string CleanStringFromPrepositionsAndConjunctions(this string? input) =>
        string.IsNullOrWhiteSpace(input)
            ? string.Empty
            : PrepositionsAndConjunctionsRegex.Replace(input, "").CleanString();

    /// <summary>
    /// Форматирование строки, чтобы оставить только цифры в в ней.
    /// </summary>
    /// <param name="input">Строка</param>
    /// <returns>Строка с только цифрами</returns>
    public static string KeepOnlyDigitsInString(this string? input) =>
        string.IsNullOrWhiteSpace(input) ? string.Empty : OnlyDigitsRegex().Replace(input, " ");

    /// <summary>
    /// Форматирование строки на удаление лишних пробелов между словами.
    /// </summary>
    /// <param name="input">Строка</param>
    /// <returns>
    /// Отформатированная строка без лишних пробелов между словами.
    /// </returns>
    public static string RemoveExtraSpaces(this string? input) =>
        string.IsNullOrWhiteSpace(input)
            ? string.Empty
            : ExtraSpacesCleanRegex2().Replace(input, " ").Trim();

    /// <summary>
    /// Очистка строки от знаков пунктуации
    /// </summary>
    /// <param name="input">Строка</param>
    /// <returns>Строка без знаков пунктуации</returns>
    private static string CleanFromPunctuation(this string input)
    {
        string withoutPunctuation = PunctuationCleanRegex().Replace(input, " ");
        return withoutPunctuation;
    }

    /// <summary>
    /// <inheritdoc cref="RemoveExtraSpaces"/>
    /// </summary>
    private static string CleanFromExtraSpaces(this string input)
    {
        string withoutExtraSpaces = ExtraSpacesCleanRegex().Replace(input, " ").Trim();
        return withoutExtraSpaces;
    }

    /// <summary>
    /// Удаление переноса строк
    /// </summary>
    /// <param name="input">Строка</param>
    /// <returns>Строка без переноса строк</returns>
    private static string CleanFromNewLines(this string input)
    {
        string withoutNewLines = NewLineCleanRegex().Replace(input, " ").ReplaceLineEndings();
        return withoutNewLines;
    }

    /// <summary>
    /// Regex для очистки знаков пунктуации
    /// </summary>
    /// <returns>
    ///     Regex для очистки знаков пунктуации
    /// </returns>
    [GeneratedRegex(@"(w*|s*)[,|.|;|:|!|?|`|@|#|$|%|^|&|*|(|)|-|+|\|](w*|s*)", Options)]
    private static partial Regex PunctuationCleanRegex();

    /// <summary>
    /// Regex для очистки переноса строк
    /// </summary>
    /// <returns>
    /// Regex для очистки переноса строк
    /// </returns>

    [GeneratedRegex(@"[\\r\\n]+", Options)]
    private static partial Regex NewLineCleanRegex();

    /// <summary>
    /// Regex для очистки лишних пробелов между словами
    /// </summary>
    /// <returns>
    /// Regex для очистки лишних пробелов между словами
    /// </returns>

    [GeneratedRegex(@"\s{2,}", Options)]
    private static partial Regex ExtraSpacesCleanRegex();

    // Regex для очистки союзов

    private static readonly string PrepositionsAndConjunctionsPattern =
        @"\b(" + string.Join("|", PrepositionsAndConjunctions) + @")\b";

    private static readonly Regex PrepositionsAndConjunctionsRegex = new(
        PrepositionsAndConjunctionsPattern,
        Options
    );

    /// <summary>
    /// Regex для очистки слов от букв, только цифры
    /// </summary>
    /// <returns>
    /// Regex для очистки слов от букв, только цифры
    /// </returns>
    [GeneratedRegex(@"\D+", Options)]
    private static partial Regex OnlyDigitsRegex();

    /// <summary>
    /// Regex для удаления лишних пробелов между словами
    /// </summary>
    /// <returns>
    /// Regex для удаления лишних пробелов между словами
    /// </returns>

    [GeneratedRegex(@"\s+", Options)]
    private static partial Regex ExtraSpacesCleanRegex2();
}
