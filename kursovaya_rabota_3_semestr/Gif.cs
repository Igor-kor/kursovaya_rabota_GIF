using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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



            Bitmap myBitmap = new Bitmap("Grapes.gif");
            globalPalette = GeneratePalette(myBitmap);
            string bincompress = Compress(GenerateImageWidthPalete(myBitmap, globalPalette));
            string uncomp = "";
            int hw = 64;
            for (int i = 0; i < (hw * hw); i++)
            {
                uncomp += ((char)(i % 8)).ToString();
            }

            //string bincompress = Compress(uncomp);
            // максимальное значение 8, TODO: сейчас только 7!
            pictures[0].MC = (byte)getBitsCount(globalPalette.colors.Count());

            // здесь нужно добавить нули до кратного 8 числа!!!!!!!  
            while (bincompress.Length % 8 > 0)
            {
                bincompress = '0' + bincompress;
            }
            int numOfBytes = bincompress.Length / 8;
            byte[] bmp = new byte[numOfBytes];
            var countbmp = 0;
            for (int i = numOfBytes - 1; i >= 0; i--)
            {
                bmp[countbmp++] = Convert.ToByte(bincompress.Substring(8 * i, 8), 2);
            }

            int count = 0, countS = 0;
            int sizeblock = 255;
            pictures[0].subblocks = new SubblockPicture[bmp.Length / sizeblock + 1];
            for (int i = 0; i < bmp.Length; i += sizeblock)
            {
                pictures[0].subblocks[count] = new SubblockPicture();
                for (int k = i; k < (i + sizeblock) && k < bmp.Length; k++)
                {
                    pictures[0].subblocks[count].block[k - i] = bmp[k];
                    countS++;
                }
                pictures[0].subblocks[count].S = (byte)countS;
                countS = 0;
                count++;
            }



            descriptor.CT = 1;
            descriptor.SF = 0;
            descriptor.Size = (byte)(getBitsCount(globalPalette.colors.Count()) - 1);
            descriptor.Color = (byte)5;
            descriptor.W = (byte)myBitmap.Width;
            descriptor.H = (byte)myBitmap.Height;

            pictures[0].pictureDescriptor.W = (byte)myBitmap.Width;
            pictures[0].pictureDescriptor.H = (byte)myBitmap.Height;
            /*
            descriptor.W = hw;
            descriptor.H = hw;

            pictures[0].pictureDescriptor.W = hw;
            pictures[0].pictureDescriptor.H = hw;
            */
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
                     if (colorPalette.colors.FindIndex(p=>p.Contains(pixel)) ==-1)
                     {
                         colorPalette.colors.Add(pixel);
                     }
                 }
             }      
            //количество цветов в палитре должно быть кратным степени 2
            while(colorPalette.colors.Count()<Math.Pow(2,getBitsCount(colorPalette.colors.Count()))) colorPalette.colors.Add(new Color(0,0,0));
            return colorPalette;
        }

        public string GenerateImageWidthPalete(Bitmap myBitmap, ColorPalette colorPalette)
        {
            string image = "";
            for (int i = 0; i < myBitmap.Width; i++)
            {
                for (int j = 0; j < myBitmap.Height; j++)
                {
                    System.Drawing.Color pixelColor = myBitmap.GetPixel(j,i );
                    Color pixel = new Color(pixelColor.R, pixelColor.G, pixelColor.B);
                    image += (char)colorPalette.colors.FindIndex(x => x.Contains(pixel));
                }
            }
            return image;
        }

        public string Compress(string uncompressed)
        {
            // build the dictionary
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            for (int i = 0; i < globalPalette.colors.Count() + 2; i++)
            {
                dictionary.Add(((char)i).ToString(), i);
            }
            string w = string.Empty;
            string bincompress = "";
            bincompress = Bincompress(dictionary[((char)globalPalette.colors.Count()).ToString()], dictionary.Count) + bincompress;
            foreach (char c in uncompressed)
            {
                string wc = w + c;
                if (dictionary.ContainsKey(wc))
                {
                    w = wc;
                }
                else
                {
                    bincompress = Bincompress(dictionary[w], dictionary.Count) + bincompress;
                    // wc is a new sequence; add it to the dictionary
                    dictionary.Add(wc, dictionary.Count);
                    w = c.ToString();
                    if (getBitsCount(dictionary.Count) == 12)
                    {
                        w = string.Empty;
                        dictionary.Clear();
                        for (int i = 0; i < globalPalette.colors.Count() + 2; i++)
                        {
                            dictionary.Add(((char)i).ToString(), i);
                        }
                        bincompress = Bincompress(dictionary[((char)globalPalette.colors.Count()).ToString()], dictionary.Count) + bincompress;
                    }
                }
            }
            // write remaining output if necessary
            if (!string.IsNullOrEmpty(w))
                bincompress = Bincompress(dictionary[w], dictionary.Count) + bincompress;
            bincompress = Bincompress(dictionary[((char)(globalPalette.colors.Count() + 1)).ToString()], dictionary.Count) + bincompress;
            return bincompress;
        }
        static int getBitsCount(int alfabetLength)
        {
            return (int)Math.Ceiling(Math.Log(alfabetLength, 2));
        }

        public string Bincompress(int word, int countrazryad)
        {
            string temp = Convert.ToString(word, 2).ToString();
            // если в блок не хватает бит то заполняем их нулями
            while (temp.Length < getBitsCount(countrazryad))
            {
                temp = '0' + temp;
            }
            return temp;
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
        public int W;
        // Высота логического экрана
        public int H;

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
                if (e != null)
                    bytes.AddRange(e.GetBytes());
            }
            bytes.Add(Terminator);
            return bytes.ToArray();
        }
    }

    class SubblockPicture : GetBytes
    {

        public SubblockPicture()
        {
            block = new byte[256];
        }
        // Размер субблока (1-255 bytes)
        public byte S;
        // Субблок из lzw кода сжатого изображения
        public byte[] block;

        public byte[] GetBytes()
        {
            var bytes = new List<byte>();
            bytes.Add(S);
            for (int i = 0; i < S; i++)
            {
                bytes.Add(block[i]);
            }
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