using System;
using System.Collections.Generic;
using System.Text;

namespace PPEMobile.Model
{
    public class FaceRectangle
    {
        public int top { get; set; }
        public int left { get; set; }
        public int widht { get; set; }
        public int height { get; set; }
    }

    public class FaceAttributes
    {
        public string gender { get; set; }
        public double age { get; set; }
    }

    public class FaceDetectResult
    {
        public string faceId { get; set; }
        public FaceRectangle faceRectangle { get; set; }
        public FaceAttributes faceAttributes { get; set; }
    }
}
