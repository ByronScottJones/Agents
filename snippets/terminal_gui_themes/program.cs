using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Terminal.Gui;

namespace TerminalGuiThemeEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.Init();

            // Load configurations using fallback pipeline layers
            ThemeLoader.LoadThemes("themes.json");

            var themesList = new List<string> {
                "Default 16-color friendly", "One Dark Pro", "Dracula", "Tokyo Night", 
                "Catppuccin Mocha", "GitHub Dark", "Night Owl", "Gruvbox Material", 
                "SynthWave '84", "Microsoft Default CUA"
            };

            // Set up our core view container using the explicit default target requested (Catppuccin Mocha)
            var currentScheme = Colors.ColorSchemes["CatppuccinMochaWindow"];
            var top = new Toplevel() { ColorScheme = currentScheme };

            var mainWin = new Window("Framework Runtime Engine Theme Switcher")
            {
                X = 0, Y = 0, Width = Dim.Fill(), Height = Dim.Fill(), ColorScheme = currentScheme
            };

            var dropdownLabel = new Label("Choose System Active Palette Theme Scheme Layout:") { X = 3, Y = 2 };

            // ComboBox Dropdown element matching required v2 interaction workflows
            var collectionSource = new ObservableCollection<string>(themesList);
            var themeSelector = new ComboBox()
            {
                X = 3, Y = 4, Width = 35, Height = 6
            };
            themeSelector.SetSource(collectionSource);
            themeSelector.SelectedItem = themesList.IndexOf("Catppuccin Mocha");

            var actionButton = new Button("Interactive Action Button State Showcase")
            {
                X = 3, Y = 12, ColorScheme = Colors.ColorSchemes["CatppuccinMochaButton"]
            };

            // Event pipeline handling changing repaint states across frames
            themeSelector.SelectedItemChanged += (args) =>
            {
                if (themeSelector.SelectedItem < 0 || themeSelector.SelectedItem >= collectionSource.Count) return;

                string rawName = collectionSource[themeSelector.SelectedItem];
                string sanitizedName = rawName.Replace(" ", "");
                
                string targetWinKey = $"{sanitizedName}Window";
                string targetBtnKey = $"{sanitizedName}Button";

                if (Colors.ColorSchemes.ContainsKey(targetWinKey))
                {
                    mainWin.ColorScheme = Colors.ColorSchemes[targetWinKey];
                    top.ColorScheme = Colors.ColorSchemes[targetWinKey];
                    actionButton.ColorScheme = Colors.ColorSchemes[targetBtnKey];
                    
                    // Mark rendering targets as dirty to cycle console frame layouts
                    mainWin.SetNeedsDisplay();
                    top.SetNeedsDisplay();
                }
            };

            mainWin.Add(dropdownLabel, themeSelector, actionButton);
            top.Add(mainWin);
            
            Application.Run(top);
            Application.Shutdown();
        }
    }
}
