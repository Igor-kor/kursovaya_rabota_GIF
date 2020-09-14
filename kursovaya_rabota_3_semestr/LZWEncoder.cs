using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kursovaya_rabota_3_semestr
{
    class LZWEncoder
    {
        public Dictionary<string, int> dict = new Dictionary<string, int>();
        ANSI table = null;

        int codeLen = 8;
        public LZWEncoder()
        {
            table = new ANSI();
            dict = table.Table;
        }

        public string EncodeToCodes(string input)
        {
            StringBuilder sb = new StringBuilder();

            int i = 0;
            string w = "";
            sb.Append(dict[(dict.Count()-2).ToString()] + ", ");

            while (i < input.Length)
            {
                w = input[i].ToString();

                i++;

                while (dict.ContainsKey(w) && i < input.Length)
                {
                    w += input[i];
                    i++;
                }

                if (dict.ContainsKey(w) == false)
                {
                    string matchKey = w.Substring(0, w.Length - 1);
                    sb.Append(dict[matchKey] + ", ");

                    dict.Add(w, dict.Count);
                    i--;
                }
                else
                {
                    sb.Append(dict[w] + ", ");
                }
            }
            sb.Append(dict[(dict.Count() - 1).ToString()] + ", ");
            return sb.ToString();
        }

        public string Encode(string input)
        {
            StringBuilder sb = new StringBuilder();

            int i = 0;
            string w = "";
            sb.Append(Convert.ToString(dict["8"], 2).FillWithZero(codeLen));
            while (i < input.Length)
            {
                w = input[i].ToString();

                i++;

                while (dict.ContainsKey(w) && i < input.Length)
                {
                    w += input[i];
                    i++;
                }

                if (dict.ContainsKey(w) == false)
                {
                    string matchKey = w.Substring(0, w.Length - 1);
                    sb.Append(Convert.ToString(dict[matchKey], 2).FillWithZero(codeLen));

                    if (Convert.ToString(dict.Count, 2).Length > codeLen)
                        codeLen++;

                    dict.Add(w, dict.Count);
                    i--;
                }
                else
                {
                    sb.Append(Convert.ToString(dict[w], 2).FillWithZero(codeLen));

                    if (Convert.ToString(dict.Count, 2).Length > codeLen)
                        codeLen++;

                }
            }
            sb.Append(Convert.ToString(dict["9"], 2).FillWithZero(codeLen));

            return sb.ToString();
        }



        public byte[] EncodeToByteList(string input)
        {
            string encodedInput = Encode(input);
            return encodedInput.ToByteArray();
        }

    }

    public static class ExtensionMethods
    {
        public static string FillWithZero(this string value, int len)
        {
            while (value.Length < len)
            {
                value = "0" + value;
            }

            return value;
        }

        public static byte[] ToByteArray(this string value)
        {
            List<byte> l = new List<byte>();

            int i = 0;
            for (i = 0; i < value.Length; i += 8)
            {
                string bs = "";
                if (i + 8 <= value.Length)
                {
                    bs = value.Substring(i, 8);
                }
                else
                {
                    bs = value.Substring(i, value.Length - i);
                }

                byte b = Convert.ToByte(bs, 2);

                l.Add(b);
            }

            return l.ToArray();
        }
    }

    public class ANSI
    {
        Dictionary<string, int> table = new Dictionary<string, int>();
        public Dictionary<string, int> Table
        {
            get
            {
                return table;
            }
        }

        public ANSI()
        {
            for (int i = '0'; i <= '9'; i++)
            {
                table.Add(System.Text.Encoding.Default.GetString(new byte[1] { Convert.ToByte(i) }), i);
            }
        }

    }
}
