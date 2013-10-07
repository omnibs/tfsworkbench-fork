// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateColour.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the StateColour type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.DataObjects
{
    using System.Windows.Media;
    using System.Xml.Serialization;

    using Properties;

    /// <summary>
    /// Initializes instance of StateColour
    /// </summary>
    public class StateColour
    {
        /// <summary>
        /// Gets or sets the colour.
        /// </summary>
        /// <value>The colour.</value>
        [XmlIgnore]
        public Color Colour { get; set; }

        /// <summary>
        /// Gets or sets the colour as string.
        /// </summary>
        /// <value>The colour as string.</value>
        [XmlAttribute(AttributeName = "colour")]
        public string ColourAsString
        {
            get
            {
                return this.Colour.ToString();
            }

            set
            {
                var colour = ColorConverter.ConvertFromString(value);

                this.Colour = colour == null ? Settings.Default.ItemColour : (Color)colour;
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [XmlText]
        public string Value { get; set; }
    }
}
