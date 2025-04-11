using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Themes.Fluent;
using Avalonia.Styling;
using System.Linq;
using System;

namespace Bwl.Framework.Avalonia
{
    public partial class AvaloniaApplication : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void SetThemeColors(ThemeVariant defaultThemeVariant,
                                    DensityStyle densityStyle,
                                    ColorPaletteResources lightTheme,
                                    ColorPaletteResources darkTheme)
        {
            this.RequestedThemeVariant = defaultThemeVariant;

            foreach (FluentTheme theme in Application.Current.Styles.OfType<FluentTheme>())
            {
                try
                {
                    theme.DensityStyle = densityStyle;
                    theme.Palettes[ThemeVariant.Light] = CloneResourcePalette(lightTheme);
                    theme.Palettes[ThemeVariant.Dark] = CloneResourcePalette(darkTheme);
                }
                catch (Exception ex)
                {
                    // Do nothing
                }
            }
        }

        private ColorPaletteResources CloneResourcePalette(ColorPaletteResources originalPalette)
        {
            var resoucePalette = new ColorPaletteResources();
            foreach (var property in typeof(ColorPaletteResources).GetProperties().Where(f => f.CanWrite))
            {
                try
                {
                    property.SetValue(resoucePalette, property.GetValue(originalPalette));
                }
                catch (Exception ex)
                {
                    // Do nothing}
                }
            }
            return resoucePalette;
        }
    }
}