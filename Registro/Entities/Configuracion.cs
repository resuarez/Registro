using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Xml.Serialization;

namespace RegistroPerforacion.Entities
{
    [Serializable]
    public class Configuracion
    {
        [XmlIgnore]
        public Bitmap LogoPorDefecto { get; set; }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("LogoEmpresa")]
        public byte[] LogoSerialized
        {
            get
            {
                // serialize
                if (LogoPorDefecto == null) return null;
                ImageConverter converter = new ImageConverter();
                return (byte[])converter.ConvertTo(LogoPorDefecto, typeof(byte[]));
            }
            set
            {
                // deserialize
                if (value == null)
                {
                    LogoPorDefecto = null;
                }
                else
                {
                    using (var ms = new MemoryStream(value))
                    {
                        LogoPorDefecto = new Bitmap(ms);
                    }
                }
            }
        }

        public List<TipoEnsayo> TiposdeEnsayos { get; set; }
        public List<ColoresAchurado> ColoresdeAchurados { get; set; }
    }
}