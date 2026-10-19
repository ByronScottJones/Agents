using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Terminal.Gui;

namespace TerminalGuiThemeEngine;

public static class ThemeLoader
{
    // Global flag determining whether to output 24-bit TrueColor or snap down to the 16-color ANSI space
    private static readonly bool SupportsTrueColor =
        Environment.GetEnvironmentVariable("COLORTERM") is "truecolor" or "24bit";

    // Immutable reference map for standard ANSI colors used as Euclidean distance nodes
    private static readonly Dictionary<Color, (int R, int G, int B)> AnsiColorMap = new()
    {
        { Color.Black, (0, 0, 0) },
        { Color.Blue, (0, 0, 170) },
        { Color.Green, (0, 170, 0) },
        { Color.Cyan, (0, 170, 170) },
        { Color.Red, (170, 0, 0) },
        { Color.Magenta, (170, 0, 170) },
        { Color.Brown, (170, 85, 0) }, // "Yellow" alternate
        { Color.Gray, (170, 170, 170) }, // "White" alternate
        { Color.DarkGray, (85, 85, 85) },
        { Color.BrightBlue, (85, 85, 255) },
        { Color.BrightGreen, (85, 255, 85) },
        { Color.BrightCyan, (85, 255, 255) },
        { Color.BrightRed, (255, 85, 85) },
        { Color.BrightMagenta, (255, 85, 255) },
        { Color.BrightYellow, (255, 255, 85) },
        { Color.BrightWhite, (255, 255, 255) }
    };

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private static readonly Attribute DefaultAttribute = new(Color.White, Color.Black);

    /// <summary>
    /// Reads the theme file and registers five color schemes per theme
    /// (<c>{Name}Window</c>, <c>Dialog</c>, <c>Menu</c>, <c>Button</c>, <c>Border</c>).
    /// Spaces and apostrophes are removed from theme names to build the keys.
    /// </summary>
    /// <returns>The theme names in file order.</returns>
    /// <exception cref="FileNotFoundException">The theme file does not exist.</exception>
    /// <exception cref="InvalidDataException">The file is not valid theme JSON.</exception>
    public static IReadOnlyList<string> LoadThemes(string jsonPath)
    {
        if (!File.Exists(jsonPath))
        {
            throw new FileNotFoundException("Theme file not found.", jsonPath);
        }

        ThemeRoot? root;
        try
        {
            string jsonContent = File.ReadAllText(jsonPath);
            root = JsonSerializer.Deserialize<ThemeRoot>(jsonContent, JsonOptions);
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException($"Theme parsing error in '{jsonPath}': {ex.Message}", ex);
        }

        if (root is null)
        {
            throw new InvalidDataException($"Theme file '{jsonPath}' is empty.");
        }

        var names = new List<string>();
        foreach (var theme in root.Themes)
        {
            string key = ThemeKey(theme.Name);
            Colors.ColorSchemes[$"{key}Window"] = CreateScheme(theme.Components.Window);
            Colors.ColorSchemes[$"{key}Dialog"] = CreateScheme(theme.Components.Dialog);
            Colors.ColorSchemes[$"{key}Menu"] = CreateScheme(theme.Components.Menu);
            Colors.ColorSchemes[$"{key}Button"] = CreateScheme(theme.Components.Button);
            Colors.ColorSchemes[$"{key}Border"] = CreateScheme(theme.Components.Border);
            names.Add(theme.Name);
        }

        return names;
    }

    /// <summary>Builds the color scheme key prefix for a theme name.</summary>
    public static string ThemeKey(string themeName) =>
        themeName.Replace(" ", string.Empty).Replace("'", string.Empty);

    private static ColorScheme CreateScheme(ComponentStates states) =>
        new()
        {
            Normal = ParseColorPair(states.Normal),
            Focus = ParseColorPair(states.Focus),
            HotNormal = ParseColorPair(states.HotNormal),
            HotFocus = ParseColorPair(states.HotFocus)
        };

    private static Attribute ParseColorPair(string? colorPairStr)
    {
        if (string.IsNullOrWhiteSpace(colorPairStr))
        {
            return DefaultAttribute;
        }

        string[] parts = colorPairStr.Split('/');
        if (parts.Length != 2)
        {
            return DefaultAttribute;
        }

        return new Attribute(ParseIndividualColor(parts[0]), ParseIndividualColor(parts[1]));
    }

    private static Color ParseIndividualColor(string colorStr)
    {
        colorStr = colorStr.Trim();

        if (colorStr.StartsWith('#'))
        {
            // Expect #RRGGBB; anything else falls back to white
            if (colorStr.Length != 7
                || !int.TryParse(colorStr.AsSpan(1, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int r)
                || !int.TryParse(colorStr.AsSpan(3, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int g)
                || !int.TryParse(colorStr.AsSpan(5, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int b))
            {
                return Color.White;
            }

            // If terminal supports 24-bit TrueColor, return target RGB natively
            if (SupportsTrueColor)
            {
                return new Color(r, g, b);
            }

            // Otherwise, fall back to the nearest 16-color ANSI value
            return GetNearestAnsiColor(r, g, b);
        }

        return colorStr.ToLowerInvariant() switch
        {
            "black" => Color.Black,
            "blue" => Color.Blue,
            "green" => Color.Green,
            "cyan" => Color.Cyan,
            "red" => Color.Red,
            "magenta" => Color.Magenta,
            "brown" or "yellow" => Color.Brown,
            "white" or "gray" or "brightgray" => Color.Gray,
            "darkgray" => Color.DarkGray,
            "brightblue" => Color.BrightBlue,
            "brightgreen" => Color.BrightGreen,
            "brightcyan" => Color.BrightCyan,
            "brightred" => Color.BrightRed,
            "brightmagenta" => Color.BrightMagenta,
            "brightyellow" => Color.BrightYellow,
            "brightwhite" => Color.BrightWhite,
            _ => Color.White
        };
    }

    private static Color GetNearestAnsiColor(int r, int g, int b)
    {
        Color closestColor = Color.White;
        double minDistance = double.MaxValue;

        foreach (var kvp in AnsiColorMap)
        {
            // Core 3D Euclidean vector distance calculation mapping target channels
            double distance = Math.Sqrt(
                Math.Pow(r - kvp.Value.R, 2) +
                Math.Pow(g - kvp.Value.G, 2) +
                Math.Pow(b - kvp.Value.B, 2));

            if (distance < minDistance)
            {
                minDistance = distance;
                closestColor = kvp.Key;
            }
        }

        return closestColor;
    }
}

#region Serialization DTOs
public sealed record ThemeRoot
{
    public List<ThemeConfig> Themes { get; init; } = [];
}

public sealed record ThemeConfig
{
    public string Name { get; init; } = string.Empty;
    public ComponentLayout Components { get; init; } = new();
}

public sealed record ComponentLayout
{
    public ComponentStates Window { get; init; } = new();
    public ComponentStates Dialog { get; init; } = new();
    public ComponentStates Menu { get; init; } = new();
    public ComponentStates Button { get; init; } = new();
    public ComponentStates Border { get; init; } = new();
}

public sealed record ComponentStates
{
    public string? Normal { get; init; }
    public string? Focus { get; init; }

    [JsonPropertyName("hotNormal")]
    public string? HotNormal { get; init; }

    [JsonPropertyName("hotFocus")]
    public string? HotFocus { get; init; }
}
#endregion
