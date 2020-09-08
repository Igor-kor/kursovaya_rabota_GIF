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
            pictures = new Picture[] { };
        }

        public string Generate()
        {
            string extString = "";
            foreach(Extension e in extensions)
            {
                extString += e.GetBytes();
            }
            string pictureString = "";
            foreach (Picture e in pictures)
            {
                pictureString += e.GetBytes();
            }
            //return title+descriptor.GetString()+globalPalette.GetString()+extString+pictureString+endfile;
            return title+ Encoding.ASCII.GetString(descriptor.GetBytes()) +(char)endfile;
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
        Int16 W;
        // Высота логического экрана
        Int16 H;

        // Наличие глобальной палитры
        int CT;
        // Цветовое разрешение исходного изображения GIF89a
        int Color;
        // Палитра сортирована по значимости GIF89a
        int SF;
        // Размер глобальной палитры(количество цветов)
        int Size;

        // Номер цвета фона
        byte BG;
        // Соотношение сторон исходного изображения
        byte R;
      
        public Descriptor()
        {       
            W = 100;
            H = 100;
            BG = 0x00;
            R = 0x00;
            CT = 0;
            Size = 0;
            Color = 0;
            SF = 0;
        }
        //data[0] = (byte) (width & 0xFF);   data[1] = (byte) ((width >> 8) & 0xFF);
        public byte[] GetBytes()
        {
            byte[] bytes = new byte[] { (byte)(W & 0xFF), 
                (byte)((W >> 8) & 0xFF),
                (byte)(H & 0xFF),
                (byte)((H >> 8) & 0xFF),
                new byte(),
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
        PictureDescriptor pictureDescriptor;
        // Начальный размер LZW кода
        int MC;
        // Локальная палитра
        ColorPalette localPallete;
        // Субблок изображения
        SubblockPicture[] subblocks;
        // Терминатор блока
        const byte Terminator = 0x00;

        public byte[] GetBytes()
        {
            throw new NotImplementedException();
        }
    }

    class SubblockPicture: GetBytes
    {
        // Размер субблока (1-255 bytes)
        int S;
        // Субблок из lzw кода сжатого изображения
        string block;

        public byte[] GetBytes()
        {
            throw new NotImplementedException();
        }
    }

    class PictureDescriptor: GetBytes
    {
        // Разделитель изображений
        byte Separator = 0x2c; //(,)
        // Положение изображения по горизонтали
        int Left;
        // Положение изображения по вертикали
        int Top;
        // Ширина изображения 
        int W;
        // Высота изображения
        int H;

        // Наличие локальной палитры
        int CT;
        // Чересстрочная развертка
        int I;
        // Палитра сортирована по зависимости (GIF89a)
        int SF;
        // Зарезервированно нулями
        string Reserved = "00";
        // Размер локальной палитры
        int Size;

        public byte[] GetBytes()
        {
            throw new NotImplementedException();
        }
    }

    class Color
    {
        byte R;
        byte G;
        byte B;
    }

    // Цветовая палитра
    class ColorPalette: GetBytes
    {
        Color[] colors;

        public byte[] GetBytes()
        {
            throw new NotImplementedException();
        }
    }

}
