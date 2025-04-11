using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;

namespace Bwl.Framework.Windows
{

    /// <summary>Изображение</summary>
    [SupportedOSPlatform("windows")]
    public class Image
    {
        protected Bitmap _rowData;

        /// <summary>Сохранить кадр по указанному пути.</summary>
        public virtual void SaveImage(string fName)
        {
            if (RowDataBytes is not null && RowDataBytes.Any())
            {
                var fileStream = new FileStream(fName, FileMode.CreateNew);
                fileStream.Write(RowDataBytes, 0, RowDataBytes.Length);
                fileStream.Dispose();
            }
        }

        /// <summary>
        /// Скопировать изображение
        /// </summary>
        public virtual Bitmap GetBitmap()
        {
            if (_rowData is null)
            {
                _rowData = new Bitmap(new MemoryStream(RowDataBytes));
            }
            return _rowData;
        }

        /// <summary>Сырые данные изобаржения</summary>
        public byte[] RowDataBytes { get; set; }

        public void SetImage(Bitmap image)
        {
            _rowData = image;
            if (_rowData is not null)
            {
                var memStream = new MemoryStream();
                _rowData.Save(memStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                RowDataBytes = new byte[(int)memStream.Length + 1];
                memStream.Position = 0L;
                memStream.Read(RowDataBytes, 0, (int)memStream.Length);
                memStream.Dispose();
            }
            else
            {
                RowDataBytes = null;
            }
        }
    }
}