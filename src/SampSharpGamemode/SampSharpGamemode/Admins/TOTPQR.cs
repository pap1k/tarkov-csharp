using System;
using System.Collections;
using System.Collections.Generic;
using SampSharp.GameMode;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.World;
using QRCoder;
using System.Linq;

namespace SampSharpGameMode.Admins
{
    public class Generator
    {
        public static int blocksize = 12;
        public static int imagesize = 250;
        public static int blocks = 21;

        public static List<List<tdRectangle>> Generate(string src, int pixelsPerModule = 20)
        {
            bool drawQuietZones = true;
            List<List<tdRectangle>> data = new List<List<tdRectangle>>();
            QRCodeGenerator gen = new QRCodeGenerator();
            QRCodeData qr = gen.CreateQrCode(src, QRCodeGenerator.ECCLevel.Q);

            var size = (qr.ModuleMatrix.Count - (drawQuietZones ? 0 : 8)) * pixelsPerModule;
            var offset = drawQuietZones ? 0 : 4 * pixelsPerModule;

            for (int x = 0, i = 0; x < size + offset; x = x + pixelsPerModule, i++)
            {
                data.Add(new List<tdRectangle>());
                for (int y = 0, j = 0; y < size + offset; y = y + pixelsPerModule, j++)
                {
                    var module = qr.ModuleMatrix[(y + pixelsPerModule) / pixelsPerModule - 1][(x + pixelsPerModule) / pixelsPerModule - 1];
                    data[i].Add(new tdRectangle(x - offset, y - offset, 5, 5, !module));
                }
            }

            return data;
        }
    }
    public class tdRectangle
    {
        public int X { get; }
        public int Y { get; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool isWhite { get; }

        public tdRectangle(int x, int y, int w, int h, bool white = true)
        {
            this.X = x;
            this.Y = y;
            this.Width = w;
            this.Height = h;
            this.isWhite = white;
        }
    }
    class TOTPQR
    {
        public static List<tdRectangle> Optimize(List<List<tdRectangle>> data)
        {
            int pixels = 5;
            List<tdRectangle> optimizedCols = new List<tdRectangle>();
            bool isLastWhite = false;
            bool newcol = true;
            for(int i = 0; i < data.Count; i++)
            {
                newcol = true;
                for (int j = 0; j < data[i].Count-1; j++)
                {
                    if(data[i][j].isWhite == data[i][j + 1].isWhite)
                    {
                        if (isLastWhite != data[i][j].isWhite || newcol)
                        {
                            Console.WriteLine($"Adding {(data[i][j].isWhite ? "WHITE" : "BLACK")}");

                            optimizedCols.Add(new tdRectangle(data[i][j].X, data[i][j].Y, data[i][j].Width, data[i][j].Height + pixels, data[i][j].isWhite));
                            newcol = false;
                        }
                        else
                        {
                            optimizedCols.Last().Height += pixels;
                        }
                        isLastWhite = data[i][j].isWhite;
                    }
                }
            }
            return optimizedCols;
        }
        public static void CreateQR(List<List<tdRectangle>> data, BasePlayer player)
        {
            var tds = Optimize(data);
            Console.WriteLine($"old size: {data.Count*data.Count}, new size: {tds.Count}");
            foreach (var elem in tds)
            {
                var tdPos = new Vector2(elem.X * .1f, elem.Y * 3f);
                var td = new TextDraw(tdPos, " ");
                td.Width = elem.Width;
                td.Height = elem.Height;
                td.Font = TextDrawFont.Normal;
                td.UseBox = true;
                td.BoxColor = elem.isWhite ? Color.White : Color.Green;
                Console.WriteLine($"Showing {(elem.isWhite ? "WHITE" : "BLACK")} td[{td.Id}]: x {elem.X * .3f}, y: {elem.Y * .3f}, w: {elem.Width}, h: {elem.Height}");
                td.Show(player);
            }
            //foreach(var tdlist in data)
            //{
            //    foreach(var elem in tdlist)
            //    {
            //        var tdPos = new Vector2(elem.X*.3f, elem.Y*3f);
            //        var td = new TextDraw(tdPos, "_");
            //        td.Width = 20;
            //        td.Height = 20;
            //        td.Font = TextDrawFont.PreviewModel;
            //        td.UseBox = true;
            //        td.BoxColor = elem.isWhite ? Color.White : Color.Black;
            //        Console.WriteLine($"Showing td: x {elem.X* .3f}, y: {elem.Y* .3f}, w: {20}, h: {20}");
            //        td.Show(player);
            //    }
            //}
        }
    }
}
