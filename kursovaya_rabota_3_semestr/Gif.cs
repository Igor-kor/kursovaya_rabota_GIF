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
        //Глобальная таблица цветов
        GlobalColorTable globalColorTable;
        //Блок расширения
        Extension extension;
        //Блок изображения
        Picture[] picture;
        // Блок завершения файла
        EndFile endFile;
        // Комментарий
        String comment;


        public Gif()
        {
            title = "GIF89a";
            comment = "Курсовая работа, Шарангия Игорь, igor_korenovsk@mail.ru";
            descriptor = new Descriptor();
        }

        public string Generate()
        {
            return descriptor.ToString();
        }
    }

    // логический дескриптор экрана
    class Descriptor
    {
        string W;
        string H;
        string BG;
        string R;
        string CT;
        string Size;
        string Color;
        string SF;
        public Descriptor()
        {       
            W = "100";
            H = "100";
            BG = "0";
            R = "00";
            CT = "0";
            Size = "000";
            Color = "7";
            SF = "0";
        }
    }

    class GlobalColorTable
    {

    }

    class Extension
    {

    }

    class Picture
    {

    }

    class EndFile
    {

    }
}
