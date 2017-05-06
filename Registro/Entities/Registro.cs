using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Xml.Serialization;

namespace RegistroPerforacion.Entities
{
    [Serializable]
    public class Registro
    {
        public Registro()
        {
            Proyecto = "";
        }

        public string Proyecto { get; set; }

        [XmlIgnore]
        public Bitmap Logo { get; set; }

        public List<Sondeo> Sondeos { get; set; }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("LogoEmpresa")]
        public byte[] LogoSerialized
        {
            get
            {
                // serialize
                if (Logo == null) return null;
                ImageConverter converter = new ImageConverter();
                return (byte[])converter.ConvertTo(Logo, typeof(byte[]));
            }
            set
            {
                // deserialize
                if (value == null)
                {
                    Logo = null;
                }
                else
                {
                    using (var ms = new MemoryStream(value))
                    {
                        Logo = new Bitmap(ms);
                    }
                }
            }
        }
    }
}