using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TfsWorkbench.UIElements.ValueConverters
{
    public class BackgroundBrushConverter : IValueConverter
    {
        private static readonly ObservableCollection<string> colourOptions = 
            new ObservableCollection<string> { "Yellow", "Green", "Cyan", "Red", "Purple", "Blue", "White", "Orange", "Pink", "Gray" };

        public static ObservableCollection<string> ColourOptions
        {
            get { return colourOptions; }
        }

        public Brush DefaultColour { get; set; }

        public Brush Pink { get; set; }

        public Brush Orange { get; set; }

        public Brush Green { get; set; }

        public Brush Cyan { get; set; }

        public Brush Purple { get; set; }

        public Brush Blue { get; set; }

        public Brush Red { get; set; }

        public Brush Yellow { get; set; }

        public Brush White { get; set; }

        public Brush Gray { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var key = value as string ?? string.Empty;

            switch (key.ToLower())
            {
                case "pink":
                case "#ffffc0cb":
                    return Pink;
                case "orange":
                case "#ffffa500":
                    return Orange;
                case "green":
                case "#ff008000":
                    return Green;
                case "cyan":
                case "#ff00ffff":
                    return Cyan;
                case "purple":
                case "#ff800080":
                    return Purple;
                case "blue":
                case "#ff0000ff":
                    return Blue;
                case "red":
                case "#ffff0000":
                    return Red;
                case "yellow":
                case "#ffffff00":
                    return Yellow;
                case "white":
                case "#ffffffff":
                    return White;
                case "gray":
                case "#ffdcdcdc":
                    return Gray;
                default:
                    return DefaultColour;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
