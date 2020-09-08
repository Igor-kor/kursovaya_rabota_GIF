using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        ColorPalette globalPalette;
        //Блоки расширения
        Extension[] extensions;
        //Блок изображения
        Picture[] pictures;
        // Блок завершения файла
        byte endfile = 0x3B; //(;)
       
        public Gif()
        {
            // Выбран GIF89a т.к. задание предпологает анимированное изображение
            title = "GIF89a";
            descriptor = new Descriptor();
            globalPalette = new ColorPalette();
            extensions = new Extension[] { };
            pictures = new Picture[] { new Picture()};
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
            globalPalette.colors.Add(new Color(0x0A,0xB2,0x5D));
            globalPalette.colors.Add(new Color(0xC8, 0xA6, 0x2D));
            globalPalette.colors.Add(new Color(0xF3, 0xED, 0x63));
            globalPalette.colors.Add(new Color(0xBA, 0x60, 0xA5));
            globalPalette.colors.Add(new Color(0x00, 0x80, 0xC8));
            globalPalette.colors.Add(new Color(0xF1, 0x60, 0x22));
            globalPalette.colors.Add(new Color(0x00, 0x00, 0x00));
            globalPalette.colors.Add(new Color(0xFF, 0xFF, 0xFF));
            pictures[0].pictureDescriptor.Left = 0;
            pictures[0].pictureDescriptor.Top = 0;
            pictures[0].pictureDescriptor.W = 4;
            pictures[0].pictureDescriptor.H = 4;
            pictures[0].pictureDescriptor.CT = 0;
            pictures[0].pictureDescriptor.Size = 0;
            pictures[0].pictureDescriptor.SF = 0;
            pictures[0].pictureDescriptor.I = 0;
            pictures[0].subblocks = new SubblockPicture[] {new SubblockPicture() };
            pictures[0].MC = 0x03;
            pictures[0].subblocks[0].S = 0x08;
            pictures[0].subblocks[0].block = new byte[] { 0x08, 0x0A, 0xD2, 0x42, 0x90, 0x94, 0x59, 0x12 };
            descriptor.CT = 1;
            descriptor.SF = 0;
            descriptor.Size = 0b10;
            descriptor.Color = 0b10;
            descriptor.W = 4;
            descriptor.H = 4;

            return Generate();
        }
    }

    interface GetBytes
    {
        byte[] GetBytes();
    }

    // логический дескриптор экрана
    class Descriptor: GetBytes
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
            field |= (byte)(SF<<3);
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
    class Picture: GetBytes
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
            if(extension != null) bytes.AddRange(extension.GetBytes());
            if (localPallete != null) bytes.AddRange(localPallete.GetBytes());
            bytes.AddRange(pictureDescriptor.GetBytes());
            bytes.Add(MC);
            foreach(SubblockPicture e in subblocks)
            {
                bytes.AddRange(e.GetBytes());
            }
            bytes.Add(Terminator);
            return bytes.ToArray();
        }
    }

    class SubblockPicture: GetBytes
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

    class PictureDescriptor: GetBytes
    {
        // Разделитель изображений
        byte Separator = 0x2c; //(,)
        // Положение изображения по горизонтали
        public int Left ;
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
    }

    // Цветовая палитра
    class ColorPalette: GetBytes
    {
        public List<Color> colors = new List<Color>();
        
        public byte[] GetBytes()
        {
            var bytes = new List<byte>();
            foreach(Color e in colors)
            {
                bytes.Add(e.R);
                bytes.Add(e.G);
                bytes.Add(e.B);
            }
            return bytes.ToArray();
        }
    }

}
