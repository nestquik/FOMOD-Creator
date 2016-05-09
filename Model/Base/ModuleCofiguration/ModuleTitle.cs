using System;
using System.ComponentModel;
using System.Xml.Serialization;
using AspectInjector.Broker;
using FomodInfrastructure.Aspect;
using FomodModel.Base.ModuleCofiguration.Enum;

namespace FomodModel.Base.ModuleCofiguration
{
    /// <summary>
    ///     Describes the display properties of the module title.
    /// </summary>
    [Aspect(typeof(AspectINotifyPropertyChanged))]
    [Serializable]
    public class ModuleTitle
    {
        /// <summary>
        ///     ModuleTitle class constructor
        /// </summary>
        public ModuleTitle()
        {
            Position = ModuleTitlePosition.Left;
        }

        /// <summary>
        ///     The identifying name of the condition flag.
        /// </summary>
        [XmlAttribute("position")]
        [DefaultValue(ModuleTitlePosition.Left)]
        public ModuleTitlePosition Position { get; set; }

        /// <summary>
        ///     The colour to use for the title.
        /// </summary>
        [XmlAttribute("colour", DataType = "hexBinary")]
        public byte[] Colour { get; set; }

        [XmlText]
        public string Value { get; set; }
    }
}