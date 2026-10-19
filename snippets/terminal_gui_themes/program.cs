using System.Collections.ObjectModel;
using Terminal.Gui;

namespace TerminalGuiThemeEngine;

internal static class Program
{
    private const string DefaultThemeName = "Catppuccin Mocha";

    private static int Main()
    {
        // Load themes before touching the terminal so failures print normally
        IReadOnlyList<string> themeNames;
        try
        {
            themeNames = ThemeLoader.LoadThemes("themes.json");
        }
        catch (Exception ex) when (ex is FileNotFoundException or InvalidDataException)
        {
            Console.Error.WriteLine(ex.Message);
            return 1;
        }

        Application.Init();

        // Set up the core view container using the default theme
        var currentScheme = Colors.ColorSchemes[$"{ThemeLoader.ThemeKey(DefaultThemeName)}Window"];
        var top = new Toplevel { ColorScheme = currentScheme };

        var mainWin = new Window("Framework Runtime Engine Theme Switcher")
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            ColorScheme = currentScheme
        };

        var dropdownLabel = new Label("Choose System Active Palette Theme Scheme Layout:") { X = 3, Y = 2 };

        // ComboBox dropdown listing every loaded theme
        var collectionSource = new ObservableCollection<string>(themeNames);
        var themeSelector = new ComboBox
        {
            X = 3,
            Y = 4,
            Width = 35,
            Height = 6
        };
        themeSelector.SetSource(collectionSource);
        themeSelector.SelectedItem = collectionSource.IndexOf(DefaultThemeName);

        var actionButton = new Button("Interactive Action Button State Showcase")
        {
            X = 3,
            Y = 12,
            ColorScheme = Colors.ColorSchemes[$"{ThemeLoader.ThemeKey(DefaultThemeName)}Button"]
        };

        // Repaint with the selected theme's schemes
        themeSelector.SelectedItemChanged += _ =>
        {
            if (themeSelector.SelectedItem < 0 || themeSelector.SelectedItem >= collectionSource.Count)
            {
                return;
            }

            string key = ThemeLoader.ThemeKey(collectionSource[themeSelector.SelectedItem]);

            if (Colors.ColorSchemes.TryGetValue($"{key}Window", out var windowScheme)
                && Colors.ColorSchemes.TryGetValue($"{key}Button", out var buttonScheme))
            {
                mainWin.ColorScheme = windowScheme;
                top.ColorScheme = windowScheme;
                actionButton.ColorScheme = buttonScheme;

                // Mark rendering targets as dirty to cycle console frame layouts
                mainWin.SetNeedsDisplay();
                top.SetNeedsDisplay();
            }
        };

        mainWin.Add(dropdownLabel, themeSelector, actionButton);
        top.Add(mainWin);

        Application.Run(top);
        Application.Shutdown();
        return 0;
    }
}
