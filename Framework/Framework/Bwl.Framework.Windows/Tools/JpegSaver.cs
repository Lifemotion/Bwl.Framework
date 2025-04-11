
// Copyright 2016 Igor Koshelev (igor@lifemotion.ru)

// Licensed under the Apache License, Version 2.0 (the "License");
// you may Not use this file except In compliance With the License.
// You may obtain a copy Of the License at

// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law Or agreed To In writing, software
// distributed under the License Is distributed On an "AS IS" BASIS,
// WITHOUT WARRANTIES Or CONDITIONS Of ANY KIND, either express Or implied.
// See the License For the specific language governing permissions And
// limitations under the License.

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Versioning;

namespace Bwl.Framework.Windows
{

    [SupportedOSPlatform("windows")]
    public class JpegSaver
    {
        private EncoderParameters _encoderParameters = new EncoderParameters(1);
        private ImageCodecInfo _codecInfo;
        private int _quality;

        public JpegSaver()
        {
            _codecInfo = GetCodecInfo(ImageFormat.Jpeg);
            Quality = 90;
        }

        public int Quality
        {
            get
            {
                return _quality;
            }
            set
            {
                _quality = value;
                _encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, _quality);
            }
        }

        private ImageCodecInfo GetCodecInfo(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        public ImageCodecInfo CodecInfo
        {
            get
            {
                return _codecInfo;
            }
        }

        public EncoderParameters EncoderParameters
        {
            get
            {
                return _encoderParameters;
            }
        }

        public void Save(string path, Bitmap bitmap)
        {
            bitmap.Save(path, _codecInfo, _encoderParameters);
        }

        public void Save(Stream stream, Bitmap bitmap)
        {
            bitmap.Save(stream, _codecInfo, _encoderParameters);
        }

        public byte[] SaveToBytes(Bitmap bitmap)
        {
            var stream = new MemoryStream();
            Save(stream, bitmap);
            return stream.ToArray();
        }
    }
}