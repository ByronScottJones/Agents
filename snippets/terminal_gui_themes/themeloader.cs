using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using Terminal.Gui;

namespace TerminalGuiThemeEngine
{
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

        public static void LoadThemes(string jsonPath)
        {
            if (!File.Exists(jsonPath)) return;

            try
            {
                string jsonContent = File.ReadAllText(jsonPath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var root = JsonSerializer.Deserialize<ThemeRoot>(jsonContent, options);

                if (root?.Themes == null) return;

                foreach (var theme in root.Themes)
                {
                    string sanitizedName = theme.Name.Replace(" ", "");
                    Colors.ColorSchemes[$"{sanitizedName}Window"] = CreateScheme(theme.Components.Window);
                    Colors.ColorSchemes[$"{sanitizedName}Dialog"] = CreateScheme(theme.Components.Dialog);
                    Colors.ColorSchemes[$"{sanitizedName}Menu"] = CreateScheme(theme.Components.Menu);
                    Colors.ColorSchemes[$"{sanitizedName}Button"] = CreateScheme(theme.Components.Button);
                    Colors.ColorSchemes[$"{sanitizedName}Border"] = CreateScheme(theme.Components.Border);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Theme parsing error: {ex.Message}");
            }
        }

        private static ColorScheme CreateScheme(ComponentStates states)
        {
            return new ColorScheme
            {
                Normal = ParseColorPair(states.Normal),
                Focus = ParseColorPair(states.Focus),
                HotNormal = ParseColorPair(states.HotNormal),
                HotFocus = ParseColorPair(states.HotFocus)
            };
        }

        private static Attribute ParseColorPair(string colorPairStr)
        {
            if (string.IsNullOrWhiteSpace(colorPairStr)) return new Attribute(Color.White, Color.Black);

            string[] parts = colorPairStr.Split('/');
            if (parts.Length != 2) return new Attribute(Color.White, Color.Black);

            return new Attribute(ParseIndividualColor(parts[0]), ParseIndividualColor(parts[1]));
        }

        private static Color ParseIndividualColor(string colorStr)
        {
            colorStr = colorStr.Trim();

            if (colorStr.StartsWith("#"))
            {
                try
                {
                    int r = Convert.ToInt32(colorStr.Substring(1, 2), 16);
                    int g = Convert.ToInt32(colorStr.Substring(3, 2), 16);
                    int b = Convert.ToInt32(colorStr.Substring(5, 2), 16);

                    // If terminal supports 24-bit TrueColor, return target RGB natively
                    if (SupportsTrueColor)
                    {
                        return new Color(r, g, b);
                    }

                    // Otherwise, execute fallback interpolation down to nearest 16-color ANSI space cell
                    return GetNearestAnsiColor(r, g, b);
                }
                catch
                {
                    return Color.White;
                }
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
                "white" or "gray" => Color.Gray,
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
                    Math.Pow(b - kvp.Value.B, 2)
                );

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestColor = kvp.Key;
                }
            }

            return closestColor;
        }
    }

    #region Serialization DTO Classes
    public class ThemeRoot { public List<ThemeConfig> Themes { get; set; } }
    public class ThemeConfig { public string Name { get; set; } public ComponentLayout Components { get; set; } }
    public class ComponentLayout { public ComponentStates Window { get; set; } public ComponentStates Dialog { get; set; } public ComponentStates Menu { get; set; } public ComponentStates Button { get; set; } public ComponentStates Border { get; set; } }
    public class ComponentStates { public string Normal { get; set; } public string Focus { get; set; } [JsonPropertyName("hotNormal")] public string HotNormal { get; set; } [JsonPropertyName("hotFocus")] public string HotFocus { get; set; } }
    #endregion
}
