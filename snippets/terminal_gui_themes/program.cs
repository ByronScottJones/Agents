using System.Collections.ObjectModel;
using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace TerminalGuiThemeEngine;

internal static class Program
{
    private const string DefaultThemeName = "Catppuccin Mocha";

    private static int Main()
    {
        string? error = RunApp();
        if (error is null)
        {
            return 0;
        }

        // Print only after the application disposes so the message survives screen restore
        Console.Error.WriteLine(error);
        return 1;
    }

    /// <summary>Runs the UI and returns an error message, or null on success.</summary>
    private static string? RunApp()
    {
        using IApplication app = Application.Create();
        app.Init();

        IReadOnlyList<string> themeNames;
        try
        {
            themeNames = ThemeLoader.LoadThemes("themes.json");
        }
        catch (Exception ex) when (ex is FileNotFoundException or InvalidDataException)
        {
            return ex.Message;
        }

        string defaultKey = ThemeLoader.ThemeKey(DefaultThemeName);

        using Window mainWin = new()
        {
            Title = "Framework Runtime Engine Theme Switcher",
            SchemeName = $"{defaultKey}Window"
        };

        Label dropdownLabel = new()
        {
            Text = "Choose System Active Palette Theme Scheme Layout:",
            X = 3,
            Y = 2
        };

        // Drop-down list of every loaded theme
        ObservableCollection<string> collectionSource = new(themeNames);
        DropDownList themeSelector = new()
        {
            X = 3,
            Y = 4,
            Width = 35,
            ReadOnly = true,
            Source = new ListWrapper<string>(collectionSource),
            Value = DefaultThemeName
        };

        Button actionButton = new()
        {
            Text = "Interactive Action Button State Showcase",
            X = 3,
            Y = 8,
            SchemeName = $"{defaultKey}Button"
        };

        // Repaint with the selected theme's schemes
        themeSelector.ValueChanged += (_, e) =>
        {
            if (e.NewValue is not { } name || !collectionSource.Contains(name))
            {
                return;
            }

            string key = ThemeLoader.ThemeKey(name);
            mainWin.SchemeName = $"{key}Window";
            actionButton.SchemeName = $"{key}Button";
            mainWin.SetNeedsDraw();
        };

        mainWin.Add(dropdownLabel, themeSelector, actionButton);

        app.Run(mainWin);
        return null;
    }
}
