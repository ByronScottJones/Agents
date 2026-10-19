using System.Text.Json;
using System.Text.Json.Serialization;
using Terminal.Gui.Configuration;
using Terminal.Gui.Drawing;
using Attribute = Terminal.Gui.Drawing.Attribute;

namespace TerminalGuiThemeEngine;

public static class ThemeLoader
{
    // Global flag determining whether to output 24-bit TrueColor or snap down to the 16-color ANSI space
    private static readonly bool SupportsTrueColor =
        Environment.GetEnvironmentVariable("COLORTERM") is "truecolor" or "24bit";

    // Reference map for the 16 ANSI colors used as Euclidean distance nodes
    private static readonly Dictionary<ColorName16, (int R, int G, int B)> AnsiColorMap = new()
    {
        { ColorName16.Black, (0, 0, 0) },
        { ColorName16.Blue, (0, 0, 170) },
        { ColorName16.Green, (0, 170, 0) },
        { ColorName16.Cyan, (0, 170, 170) },
        { ColorName16.Red, (170, 0, 0) },
        { ColorName16.Magenta, (170, 0, 170) },
        { ColorName16.Yellow, (170, 85, 0) }, // dark yellow, rendered as brown
        { ColorName16.Gray, (170, 170, 170) },
        { ColorName16.DarkGray, (85, 85, 85) },
        { ColorName16.BrightBlue, (85, 85, 255) },
        { ColorName16.BrightGreen, (85, 255, 85) },
        { ColorName16.BrightCyan, (85, 255, 255) },
        { ColorName16.BrightRed, (255, 85, 85) },
        { ColorName16.BrightMagenta, (255, 85, 255) },
        { ColorName16.BrightYellow, (255, 255, 85) },
        { ColorName16.White, (255, 255, 255) }
    };

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private static readonly Attribute DefaultAttribute = new(new Color(ColorName16.Gray), new Color(ColorName16.Black));

    /// <summary>
    /// Reads the theme file and registers five schemes per theme with the
    /// <see cref="SchemeManager"/>: <c>{Key}Window</c>, <c>Dialog</c>, <c>Menu</c>,
    /// <c>Button</c>, and <c>Border</c>. Assign them to a view with
    /// <c>view.SchemeName</c>. Call this after <c>app.Init()</c>.
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
            root = JsonSerializer.Deserialize<ThemeRoot>(File.ReadAllText(jsonPath), JsonOptions);
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
            SchemeManager.AddScheme($"{key}Window", CreateScheme(theme.Components.Window));
            SchemeManager.AddScheme($"{key}Dialog", CreateScheme(theme.Components.Dialog));
            SchemeManager.AddScheme($"{key}Menu", CreateScheme(theme.Components.Menu));
            SchemeManager.AddScheme($"{key}Button", CreateScheme(theme.Components.Button));
            SchemeManager.AddScheme($"{key}Border", CreateScheme(theme.Components.Border));
            names.Add(theme.Name);
        }

        return names;
    }

    /// <summary>Builds the scheme name prefix for a theme name.</summary>
    public static string ThemeKey(string themeName) =>
        themeName.Replace(" ", string.Empty).Replace("'", string.Empty);

    private static Scheme CreateScheme(ComponentStates states) =>
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
            if (colorStr.Length != 7 || !Color.TryParse(colorStr, null, out Color rgb))
            {
                return new Color(ColorName16.White);
            }

            // If terminal supports 24-bit TrueColor, return target RGB natively
            if (SupportsTrueColor)
            {
                return rgb;
            }

            // Otherwise, fall back to the nearest 16-color ANSI value
            return new Color(GetNearestAnsiColor(rgb.R, rgb.G, rgb.B));
        }

        return new Color(colorStr.ToLowerInvariant() switch
        {
            "black" => ColorName16.Black,
            "blue" => ColorName16.Blue,
            "green" => ColorName16.Green,
            "cyan" => ColorName16.Cyan,
            "red" => ColorName16.Red,
            "magenta" => ColorName16.Magenta,
            "brown" or "yellow" => ColorName16.Yellow,
            "white" or "gray" or "brightgray" => ColorName16.Gray,
            "darkgray" => ColorName16.DarkGray,
            "brightblue" => ColorName16.BrightBlue,
            "brightgreen" => ColorName16.BrightGreen,
            "brightcyan" => ColorName16.BrightCyan,
            "brightred" => ColorName16.BrightRed,
            "brightmagenta" => ColorName16.BrightMagenta,
            "brightyellow" => ColorName16.BrightYellow,
            "brightwhite" => ColorName16.White,
            _ => ColorName16.White
        });
    }

    private static ColorName16 GetNearestAnsiColor(int r, int g, int b)
    {
        ColorName16 closestColor = ColorName16.White;
        double minDistance = double.MaxValue;

        foreach (var kvp in AnsiColorMap)
        {
            // 3D Euclidean distance between the target and the ANSI reference color
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
