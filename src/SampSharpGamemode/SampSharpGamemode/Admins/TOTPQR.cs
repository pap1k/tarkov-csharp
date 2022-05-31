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

        public static List<List<bool>> Generate(string src, int pixelsPerModule = 1)
        {
            bool drawQuietZones = true;
            List<List<bool>> data = new List<List<bool>>();
            QRCodeGenerator gen = new QRCodeGenerator();
            QRCodeData qr = gen.CreateQrCode(src, QRCodeGenerator.ECCLevel.Q);

            var size = (qr.ModuleMatrix.Count - (drawQuietZones ? 0 : 8)) * pixelsPerModule;
            var offset = drawQuietZones ? 0 : 4 * pixelsPerModule;

            for (int x = 0, i = 0; x < size + offset; x = x + pixelsPerModule, i++)
            {
                data.Add(new List<bool>());
                for (int y = 0, j = 0; y < size + offset; y = y + pixelsPerModule, j++)
                {
                    var module = qr.ModuleMatrix[(y + pixelsPerModule) / pixelsPerModule - 1][(x + pixelsPerModule) / pixelsPerModule - 1];
                    data[i].Add(module);
                    Console.Write(module ? "$" : " ");
                }
                Console.WriteLine();
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
        public static List<tdRectangle> Optimize(List<List<bool>> data)
        {
            int pixels = 1;
            List<tdRectangle> SUPEROPTIMIZED = new List<tdRectangle>();
            List<string> usedids = new List<string>();
            //Этот алгоритм находит все, что можно объединить, и объединяет. То есть квадраты в квадраты, линии в линии, столбики в стоблики. MQ
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < data[i].Count - 1; j++)
                {
                    //ALKO RYTHM
                    if (data[i][j])
                    {
                        SUPEROPTIMIZED.Add(new tdRectangle(j, i, pixels, pixels, false));
                        //Чекаем сколько вправо
                        int right = 0;
                        int jj = j+1;
                        while (jj < data.Count && data[i][jj++]) right++;
                        //Чекаем сколько вниз
                        int down = 0;
                        int ii = i + 1;
                        while (ii < data.Count && data[ii++][j]) down++;
                        for (int k = i; k < data.Count; k++)
                        {
                            if (k >= data.Count)
                                break;
                            if (data[k][j])
                                down++;
                        }
                        if(down > 0 && right > 0)
                        {
                            int sqsize = 0;
                            for (int ki = 1; ki <= down; ki++)
                            {
                                bool isSq = true;
                                List<string> temp = new List<string>();
                                for(int kj = 1; kj <= ki; kj++)
                                {
                                    if (data[i + ki][j + kj])
                                        temp.Add($"{i + ki},{j + kj}");
                                    else{
                                        isSq = false;
                                        break;
                                    }
                                }
                                if (isSq)
                                {
                                    foreach (var s in temp)
                                        data[int.Parse(s.Split(',')[0])][int.Parse(s.Split(',')[1])] = false;
                                    sqsize++;
                                }
                            }
                            if(sqsize == 0)
                            {
                                bool flag = false;
                                for (int k = 0; k < right; k++)
                                {
                                    data[i][j + k] = false;
                                    SUPEROPTIMIZED.Last().Width += pixels;
                                    flag = true;
                                }
                                if (flag) SUPEROPTIMIZED.Add(new tdRectangle(j, i + 1, pixels, pixels, false));
                                for (int k = 1; k < down; k++)
                                {
                                    data[i + k][j] = false;
                                    SUPEROPTIMIZED.Last().Height += pixels;
                                }
                            }
                            else
                            {
                                SUPEROPTIMIZED.Last().Width += pixels*sqsize;
                                SUPEROPTIMIZED.Last().Height+= pixels*sqsize;
                            }
                        }
                        else
                        {
                            bool flag = false;
                            for (int k = 0; k < right; k++)
                            {
                                data[i][j + k] = false;
                                SUPEROPTIMIZED.Last().Width += pixels;
                                flag = true;
                            }
                            if (flag) SUPEROPTIMIZED.Add(new tdRectangle(j, i + 1, pixels, pixels, false));
                            for (int k = 1; k < down; k++)
                            {
                                data[i+k][j] = false;
                                SUPEROPTIMIZED.Last().Height += pixels;
                            }
                        }
                    }
                }
            }
            // OLD TRASH SHIT AND FUCKING FUCK
            //List<tdRectangle> optimizedRow = new List<tdRectangle>();
            //for(int i = 0; i < data.Count; i++)
            //{
            //    optimizedRow.Add(new tdRectangle(0, i, pixels, pixels, data[i][0]));
            //    for (int j = 0; j < data[i].Count - 1; j++)
            //    {
            //        if (data[i][j] == data[i][j + 1])
            //            optimizedRow.Last().Width += pixels;
            //        else
            //            optimizedRow.Add(new tdRectangle(j+1, i, pixels, pixels, data[i][j + 1]));
            //    }
            //}

        return SUPEROPTIMIZED;
        }
        private static PlayerTextDraw CreateRectangle(BasePlayer p, float x, float y, string text, int h, int w, Color c, int scale = 1)
        {
            var td = new PlayerTextDraw(p, new Vector2(x , y), text);
            td.Height = h*scale;
            td.Width = w*scale;
            td.PreviewZoom = 1;
            td.Font = TextDrawFont.PreviewModel;
            td.PreviewModel = -1;
            td.Outline = 0;
            td.UseBox = true;
            td.BackColor = c;
            return td;
        }
        public static List<int> CreateQR(List<List<bool>> data, BasePlayer player)
        {
            var tds = Optimize(data);
            Console.WriteLine($"old size: {data.Count * data.Count}, new size: {tds.Count}");
            int scale = 4;
            int startposx = 150, startposy = 150;
            int i = 0;
            ////back color
            //CreateRectangle(player, startposx, startposy, "_", data.Count, data.Count, Color.White, scale).Show();
            ////left sq
            //CreateRectangle(player, startposx + 4 * scale, startposy + 4 * scale, "_", 7, 7, Color.Black, scale).Show();
            //CreateRectangle(player, startposx + 5 * scale, startposy + 5 * scale, "_", 5, 5, Color.White, scale).Show();
            //CreateRectangle(player, startposx + 6 * scale, startposy + 6 * scale, "_", 3, 3, Color.Black, scale).Show();
            ////right sq
            //CreateRectangle(player, startposx + (data.Count - 4) * scale, startposy + 4 * scale, "_", 7, -7, Color.Black, scale).Show();
            //CreateRectangle(player, startposx + (data.Count - 5) * scale, startposy + 5 * scale, "_", 5, -5, Color.White, scale).Show();
            //CreateRectangle(player, startposx + (data.Count - 6) * scale, startposy + 6 * scale, "_", 3, -3, Color.Black, scale).Show();
            ////bottom sq
            //CreateRectangle(player, startposx + 4 * scale, startposy + (data.Count - 4) * scale, "_", -7, 7, Color.Black, scale).Show();
            //CreateRectangle(player, startposx + 5 * scale, startposy + (data.Count - 5) * scale, "_", -5, 5, Color.White, scale).Show();
            //CreateRectangle(player, startposx + 6 * scale, startposy + (data.Count - 6) * scale, "_", -3, 3, Color.Black, scale).Show();

            var ret = new List<int>();
            foreach (var elem in tds)
            {
                bool cond = !elem.isWhite && //if black
                    !(((elem.X >= 4 && elem.X <= 4 + 7) && (elem.Y >= 4 && elem.Y <= 4 + 7)) ||// exclude left top block
                    ((elem.X <= data.Count - 4 && elem.X >= data.Count - 7 - 4) && (elem.Y >= 4 && elem.Y <= 7 + 4)) ||// exclude right top block
                    ((elem.X >= 4 && elem.X <= 7 + 4) && (elem.Y <= data.Count - 4 && elem.Y >= data.Count - 7 - 4)));// exclude left bottom block
                if (cond)
                {
                    var tdPos = new Vector2(startposx + elem.X * scale, startposy + elem.Y * scale);
                    var td = new PlayerTextDraw(player, tdPos, " ");
                    td.Height = elem.Height * scale;
                    td.Width = elem.Width * scale;
                    td.PreviewZoom = 1000;
                    td.Font = TextDrawFont.PreviewModel;
                    td.PreviewModel = elem.X < data.Count / 2 ? 331 : 326;
                    td.Outline = 0;
                    td.UseBox = true;
                    td.BackColor = Color.Black;
                    i++;
                    td.Show();
                    ret.Add(td.Id);
                }
            }
            Console.WriteLine($"Totally screated {i + 11} TextDraws");
            return ret;
        }
    }
}
