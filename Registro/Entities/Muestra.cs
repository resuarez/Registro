using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Xml.Serialization;

namespace RegistroPerforacion.Entities
{
    [Serializable]
    public class Muestra
    {
        public string Caja { get; set; }
        public double? ProfundidadArriba { get; set; }
        public double? ProfundidadAbajo { get; set; }
        public int? Numero { get; set; }
        public string Tipo { get; set; }
        public string Descripcion { get; set; }

        [XmlIgnore]
        public Bitmap Achurado { get; set; }

        [XmlElement("DataAchurado")]
        public byte[] ImagenSerialized
        {
            get
            {
                // serialize
                if (Achurado == null) return null;
                using (var ms = new MemoryStream())
                {
                    Achurado.Save(ms, ImageFormat.Bmp);
                    return ms.ToArray();
                }
            }
            set
            {
                // deserialize
                if (value == null)
                {
                    Achurado = null;
                }
                else
                {
                    using (var ms = new MemoryStream(value))
                    {
                        Achurado = new Bitmap(ms);
                    }
                }
            }
        }
    }
}