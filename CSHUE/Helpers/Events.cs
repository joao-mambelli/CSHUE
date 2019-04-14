using System.Collections.Generic;
using System.Configuration;
using System.Drawing;

namespace CSHUE.Helpers
{
    /// <summary>
    /// Defines an event property.
    /// </summary>
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class EventProperty
    {
        public List<UniqueLight> Lights { get; set; }
        public List<string> SelectedLights { get; set; }
    }

    /// <summary>
    /// Defines an event containing "OnlyBrightness" option property.
    /// </summary>
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class EventBrightnessProperty
    {
        public List<UniqueBrightnessLight> Lights { get; set; }
        public List<string> SelectedLights { get; set; }
    }

    /// <summary>
    /// Defines an unique light.
    /// </summary>
    public class UniqueLight
    {
        public string Id { get; set; }
        public SerializableColor Color { get; set; }
        public byte Brightness { get; set; }
    }

    /// <summary>
    /// Defines an unique containing "OnlyBrightness" option light.
    /// </summary>
    public class UniqueBrightnessLight
    {
        public string Id { get; set; }
        public SerializableColor Color { get; set; }
        public byte Brightness { get; set; }
        public bool OnlyBrightness { get; set; }
    }

    /// <summary>
    /// Represents a color, stored in a way that it can be serialized.
    /// </summary>
    public class SerializableColor
    {
        /// <summary>
        /// The red component.
        /// </summary>
        public byte Red { get; set; }

        /// <summary>
        /// The green component.
        /// </summary>
        public byte Green { get; set; }

        /// <summary>
        /// The blue component.
        /// </summary>
        public byte Blue { get; set; }

        /// <summary>
        /// Converts a <see cref="SerializableColor"/> to a <see cref="Color"/>.
        /// </summary>
        /// <param name="src">The color to convert.</param>
        public static implicit operator Color(SerializableColor src)
        {
            return Color.FromArgb(src.Red, src.Green, src.Blue);
        }

        /// <summary>
        /// Converts a <see cref="Color"/> to a <see cref="SerializableColor"/>.
        /// </summary>
        /// <param name="src">The color to convert.</param>
        public static implicit operator SerializableColor(Color src)
        {
            return new SerializableColor
            {
                Red = src.R,
                Green = src.G,
                Blue = src.B
            };
        }
    }
}
