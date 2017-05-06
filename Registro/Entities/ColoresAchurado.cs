using System;
using System.Drawing;
using System.Xml.Serialization;

namespace RegistroPerforacion.Entities
{
    [Serializable]
    public class ColoresAchurado
    {
        public string Name { get; set; }
        public string HexCode { get; set; }

        [XmlIgnore]
        public Bitmap Imagen
        {
            get
            {
                var bmp = new Bitmap(38, 38);
                using (var g = Graphics.FromImage(bmp))
                {
                    g.Clear(ColorTranslator.FromHtml(HexCode ?? "#ffffff"));
                }
                return bmp;
            }
        }
    }
}