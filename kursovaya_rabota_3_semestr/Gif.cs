using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace kursovaya_rabota_3_semestr
{
    class Gif
    {
        // Заголовок
        string title;
        // Логический дескриптор экрана
        Descriptor descriptor;
        //Глобальная палитра
        public ColorPalette globalPalette;
        //Блоки расширения
        Extension[] extensions;
        //Блок изображения
        public Picture[] pictures;
        // Блок завершения файла
        byte endfile = 0x3B; //(;)
        public string text;

        public Gif()
        {
            // Выбран GIF89a т.к. задание предпологает анимированное изображение
            title = "GIF89a";
            descriptor = new Descriptor();
            globalPalette = new ColorPalette();
            extensions = new Extension[] { };
            pictures = new Picture[] { new Picture() };
        }

        public byte[] Generate()
        {
            var bytes = new List<byte>();
            bytes.AddRange(Encoding.ASCII.GetBytes(title));
            bytes.AddRange(descriptor.GetBytes());
            bytes.AddRange(globalPalette.GetBytes());
            foreach (Extension e in extensions)
            {
                bytes.AddRange(e.GetBytes());
            }
            foreach (Picture e in pictures)
            {
                bytes.AddRange(e.GetBytes());
            }
            bytes.Add(endfile);

            return bytes.ToArray();
        }

        public byte[] TestGenerate()
        {
            globalPalette = new ColorPalette();
            globalPalette.colors.Add(new Color(0x0A, 0xB2, 0x5D));
            globalPalette.colors.Add(new Color(0xC8, 0xA6, 0x2D));
            globalPalette.colors.Add(new Color(0xF3, 0xED, 0x63));
            globalPalette.colors.Add(new Color(0xBA, 0x60, 0xA5));
            globalPalette.colors.Add(new Color(0x00, 0x80, 0xC8));
            globalPalette.colors.Add(new Color(0xF1, 0x60, 0x22));
            globalPalette.colors.Add(new Color(0x00, 0x00, 0x00));
            globalPalette.colors.Add(new Color(0xFF, 0xFF, 0xFF));


            pictures[0].pictureDescriptor.Left = 0;
            pictures[0].pictureDescriptor.Top = 0;
            pictures[0].pictureDescriptor.W = 64;
            pictures[0].pictureDescriptor.H = 64;
            pictures[0].pictureDescriptor.CT = 0;
            pictures[0].pictureDescriptor.Size = 0;
            pictures[0].pictureDescriptor.SF = 0;
            pictures[0].pictureDescriptor.I = 0;
            


            //Bitmap myBitmap = new Bitmap("Grapes.jpg");
            //globalPalette = GeneratePalette(myBitmap); 
            List<int> compresedbmp = Compress("0000222244445555");
            List<int> sortcompress = new List<int>();
            for(int i = 0; i < compresedbmp.Count(); i+=2)
            {
                if(i+1 == compresedbmp.Count())
                {
                    //sortcompress.Add(0);
                }
                else
                {
                    sortcompress.Add(compresedbmp[i + 1]);
                }               
                sortcompress.Add(compresedbmp[i]);
            }
             pictures[0].MC = 0x03;
            string bincompress = "";
            int countrazryad = 1;
            foreach(int e in sortcompress)
            {
                string temp = Convert.ToString(e, 2).ToString();
                // если количество бит больше то увиличиваем размер блока
                if (e >= Math.Pow(2, pictures[0].MC + countrazryad)) countrazryad++;
                // если в блок не хватает бит то заполняем их нулями
                while (temp.Length < pictures[0].MC+ countrazryad)
                {
                    temp = '0' + temp;
                }
                bincompress+= temp;
            }
            // здесь нужно добавить нули до кратного 8 числа!!!!!!!
            while(bincompress.Length % 8 > 0)
            {
                bincompress += '0';
            }
            int numOfBytes = bincompress.Length / 8;
            byte[] bmp = new byte[numOfBytes];
            for (int i = 0; i < numOfBytes; ++i)
            {
                bmp[i] = Convert.ToByte(bincompress.Substring(8 * i, 8), 2);
            }

            int count = 0, countblock = 0 ;
            pictures[0].subblocks = new SubblockPicture[bmp.Length/256+1];
            for (int i = 0; i < bmp.Length; i+=256)
            {
                pictures[0].subblocks[count] = new SubblockPicture();
                pictures[0].subblocks[count].block = new byte[((bmp.Length - (count * 256)) > 256) ? 256: bmp.Length%256] ;
                for(int k = i;k < i + 256 && k < bmp.Length; k++)
                {
                    pictures[0].subblocks[count].block[k%256] = bmp[k];
                    countblock++;
                }
                pictures[0].subblocks[0].S = (byte)(countblock);
                count++;
                //countblock = 0;
            }
            /*10000000 10100000 00101101 00100100 10000010 00101100 11100101 01
             *00001000 00001010 11010010 01000010 01001000 01001101 01010110 01
             *00001000 00001010 11010010 01000010 01001000 01001100 10100101 01001000
             *
             *8 0 10 0 2 13 2 4 16 4 5 19 9 5
             *#8 #0 #10 #0 #2 #13 #2 #4 #16 #4 #5 #19 #5 #9
             *
            1000 0000 08
            1010 0000 0A
            0010 1101 D2
            0010 0100 42
            1000 0010 28 90
            0010 1100 C2 94
            1110 0101 5E 59
            01           12        
            */
            text =  string.Join(" ", compresedbmp);

            descriptor.CT = 1;
            descriptor.SF = 0;
            descriptor.Size = 0b010;
            descriptor.Color = 0b010;
            descriptor.W = (byte)4;
            descriptor.H = (byte)4;
            //return Compress(GenerateImageWidthPalete(myBitmap, globalPalette)).ToArray();

            pictures[0].pictureDescriptor.W = (byte)4;
            pictures[0].pictureDescriptor.H = (byte)4;

            return Generate();
        }

        public ColorPalette GeneratePalette(Bitmap myBitmap)
        {
            ColorPalette colorPalette = new ColorPalette();
            colorPalette.colors = new List<Color>();
            for (int i = 0; i < myBitmap.Width; i++)
            {
                for (int j = 0; j < myBitmap.Height; j++)
                {
                    System.Drawing.Color pixelColor = myBitmap.GetPixel(i, j);
                    Color pixel = new Color(pixelColor.R, pixelColor.G, pixelColor.B);
                    if (colorPalette.colors.Find(x => x.Contains(pixel)) == null )
                    {
                        colorPalette.colors.Add(pixel);
                    }
                }
            }
            return colorPalette;
        }

        public string GenerateImageWidthPalete(Bitmap myBitmap, ColorPalette colorPalette)
        {
            string image = "";
            for (int i = 0; i < myBitmap.Width; i++)
            {
                for (int j = 0; j < myBitmap.Height; j++)
                {
                    System.Drawing.Color pixelColor = myBitmap.GetPixel(i, j);
                    Color pixel = new Color(pixelColor.R, pixelColor.G, pixelColor.B);
                    image += colorPalette.colors.FindIndex(x => x.Contains(pixel));
                }
            }
            return image;
        }

        public List<int> Compress(string uncompressed)
        {
            // build the dictionary
            Dictionary<string, int> dictionary = new Dictionary<string, int>();         
            for (int i = 0; i < globalPalette.colors.Count()+2; i++)
            {
                dictionary.Add((i).ToString(), i);
            }         
            string w = string.Empty;
            List<int> compressed = new List<int>();
            compressed.Add(dictionary[globalPalette.colors.Count().ToString()]);
            foreach (char c in uncompressed)
            {
                string wc = w + c;
                if (dictionary.ContainsKey(wc))
                {
                    w = wc;
                }
                else
                {
                    // write w to output
                    compressed.Add(dictionary[w]);
                    // wc is a new sequence; add it to the dictionary
                    dictionary.Add(wc, dictionary.Count);
                    w = c.ToString();
                }
            }
            
            // write remaining output if necessary
            if (!string.IsNullOrEmpty(w))
                compressed.Add(dictionary[w]);
            compressed.Add(dictionary[(globalPalette.colors.Count() + 1).ToString()]);
            return compressed;
        }

    }

    interface GetBytes
    {
        byte[] GetBytes();
    }

    // логический дескриптор экрана
    class Descriptor : GetBytes
    {
        // Ширина логического экрана
        public Int16 W;
        // Высота логического экрана
        public Int16 H;

        // Наличие глобальной палитры(0-1)
        public byte CT;
        // Цветовое разрешение исходного изображения GIF89a (0-7)
        public byte Color;
        // Палитра сортирована по значимости GIF89a(0-1)
        public byte SF;
        // Размер глобальной палитры(количество цветов)(0-7)
        public byte Size;

        // Номер цвета фона
        public byte BG;
        // Соотношение сторон исходного изображения
        public byte R;



        public byte getField()
        {
            byte field = 0b00000000;
            field |= Size;
            field |= (byte)(SF << 3);
            field |= (byte)(Color << 4);
            field |= (byte)(CT << 7);
            return (byte)field;
        }

        public Descriptor()
        {
            W = 100;
            H = 100;
            CT = 1;
            Color = 7;
            SF = 1;
            Size = 7;
            BG = 0x00;
            R = 0x00;
        }

        public byte[] GetBytes()
        {
            byte[] bytes = new byte[] { (byte)(W & 0xFF),
                (byte)((W >> 8) & 0xFF),
                (byte)(H & 0xFF),
                (byte)((H >> 8) & 0xFF),
                getField(),
                BG,
                R};
            return bytes;
        }
    }

    // Класс расширений
    class Extension : GetBytes
    {
        public byte[] GetBytes()
        {
            throw new NotImplementedException();
        }
    }

    // Изображение
    class Picture : GetBytes
    {
        // Расширение управления графикой для анимации
        Extension extension;
        // Дескриптор изображения
        public PictureDescriptor pictureDescriptor;
        // Начальный размер LZW кода
        public byte MC;
        // Локальная палитра
        ColorPalette localPallete;
        // Субблок изображения
        public SubblockPicture[] subblocks;
        // Терминатор блока
        byte Terminator = 0x00;

        public Picture()
        {
            subblocks = new SubblockPicture[] { };
            pictureDescriptor = new PictureDescriptor();
        }

        public byte[] GetBytes()
        {
            var bytes = new List<byte>();
            if (extension != null) bytes.AddRange(extension.GetBytes());
            if (localPallete != null) bytes.AddRange(localPallete.GetBytes());
            bytes.AddRange(pictureDescriptor.GetBytes());
            bytes.Add(MC);
            foreach (SubblockPicture e in subblocks)
            {
                bytes.AddRange(e.GetBytes());
            }
            bytes.Add(Terminator);
            return bytes.ToArray();
        }
    }

    class SubblockPicture : GetBytes
    {
        // Размер субблока (1-255 bytes)
        public byte S;
        // Субблок из lzw кода сжатого изображения
        public byte[] block;

        public byte[] GetBytes()
        {
            var bytes = new List<byte>();
            bytes.Add(S);
            bytes.AddRange(block);
            return bytes.ToArray();
        }
    }

    class PictureDescriptor : GetBytes
    {
        // Разделитель изображений
        byte Separator = 0x2c; //(,)
        // Положение изображения по горизонтали
        public int Left;
        // Положение изображения по вертикали
        public int Top;
        // Ширина изображения 
        public int W;
        // Высота изображения
        public int H;

        // Наличие локальной палитры
        public int CT;
        // Чересстрочная развертка
        public int I;
        // Палитра сортирована по зависимости (GIF89a)
        public int SF;
        // Размер локальной палитры
        public byte Size;

        public byte getField()
        {
            byte field = 0b00000000;
            field |= Size;
            field |= (byte)(SF << 5);
            field |= (byte)(I << 6);
            field |= (byte)(CT << 7);
            return (byte)field;
        }
        public byte[] GetBytes()
        {
            var bytes = new List<byte>();
            bytes.Add(Separator);
            bytes.Add((byte)(Left & 0xFF));
            bytes.Add((byte)((Left >> 8) & 0xFF));
            bytes.Add((byte)(Top & 0xFF));
            bytes.Add((byte)((Top >> 8) & 0xFF));
            bytes.Add((byte)(W & 0xFF));
            bytes.Add((byte)((W >> 8) & 0xFF));
            bytes.Add((byte)(H & 0xFF));
            bytes.Add((byte)((H >> 8) & 0xFF));
            bytes.Add(getField());

            return bytes.ToArray();
        }
    }

    class Color
    {
        public byte R;
        public byte G;
        public byte B;
        public Color(byte _r, byte _g, byte _b)
        {
            R = _r;
            G = _g;
            B = _b;
        }
        public bool Contains(Color e)
        {
            return e.R == R && e.G == G && e.B == B;
        }
    }

    // Цветовая палитра
    class ColorPalette : GetBytes
    {
        public List<Color> colors = new List<Color>();

        public byte[] GetBytes()
        {
            var bytes = new List<byte>();
            foreach (Color e in colors)
            {
                bytes.Add(e.R);
                bytes.Add(e.G);
                bytes.Add(e.B);
            }
            return bytes.ToArray();
        }
    }

}
