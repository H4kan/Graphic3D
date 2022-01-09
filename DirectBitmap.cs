﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

namespace MysteryProject
{
    // im not the author of this class, i found it on stack overflow
    public class DirectBitmap : IDisposable, ICloneable
    {
        public Bitmap Bitmap { get; private set; }
        public Int32[] Bits { get; set; }
        public bool Disposed { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }

        protected GCHandle BitsHandle { get; private set; }

        public DirectBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Bits = new Int32[width * height];
            for (int i = 0; i < width * height; i++)
                Bits[i] = Color.Black.ToArgb();
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());

        }

        public void SetPixel(int x, int y, Color colour)
        {
            int transformedX = x;
            int transformedY = this.Bitmap.Height - 1 - y;

            int index = transformedX + (transformedY * Width);
            int col = colour.ToArgb();

            Bits[index] = col;
        }

        public Color GetPixel(int x, int y)
        {
            int transformedX = y;
            int transformedY = this.Bitmap.Height - 1 - x;

            int index = transformedX + (transformedY * Width);
            int col = Bits[index];
            Color result = Color.FromArgb(col);

            return result;
        }

        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            Bitmap.Dispose();
            BitsHandle.Free();
        }

        public object Clone()
        {
            var copy = new DirectBitmap(this.Width, this.Height);
            this.Bits.CopyTo(copy.Bits, 0);
            return (object)copy;
        }
    }
}
