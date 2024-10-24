using System;
using System.Collections.Generic;

namespace VitualBreaks.Models
{
    public partial class BackgroundImage
    {
        public int Id { get; set; }
        public string? FileName { get; set; }
        public byte[]? ImageData { get; set; }

    }
}
