﻿using SharpDX;

namespace ExileCore.Shared
{
    public class HudTexture
    {
        public HudTexture()
        {
        }

        public HudTexture(string fileName)
        {
            FileName = fileName;
        }

        public string FileName { get; set; }
        public RectangleF UV { get; set; } = new RectangleF(0, 0, 1, 1);
        public float Size { get; set; } = 13;
        public Color Color { get; set; } = Color.White;
    }
}
