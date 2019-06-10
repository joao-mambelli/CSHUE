using System.Collections.Generic;
using System.Configuration;
using System.Windows.Media;

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
        public string UniqueId { get; set; }
        public string Id { get; set; }
        public Color Color { get; set; }
        public byte Brightness { get; set; }
    }

    /// <summary>
    /// Defines an unique containing "OnlyBrightness" option light.
    /// </summary>
    public class UniqueBrightnessLight
    {
        public string UniqueId { get; set; }
        public string Id { get; set; }
        public Color Color { get; set; }
        public byte Brightness { get; set; }
        public bool OnlyBrightness { get; set; }
    }
}
