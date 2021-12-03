using System;
using System.Text;

namespace ChatCoreTest
{
  internal class Program
  {
    private static byte[] m_PacketData;  //封包
    private static int m_Pos; //儲存資料個數

    public static void Main(string[] args)
    {
      m_PacketData = new byte[1024]; //最大上限1024Byte
      m_Pos = 4; //從4開始，前面放資料總長度

      Write(109.99f);
      Write(109);
      Write("你好！");
      Write(55555555);
      Write(666666);
      Write(10.13f);
      Write(20211203);
      Write("今天天氣真好～");
      Write("HAPPYBIRTHDAY！");

      Console.Write($"Output Byte array(length:{m_Pos}): ");
      for (var i = 0; i < m_Pos; i++)
      {
          Console.Write(m_PacketData[i] + ",");
      }
      Console.WriteLine("\n");
      Receive(m_PacketData);
      Console.ReadLine();
    }

    private static bool WriteLen(int i)
    {
      var bytes = BitConverter.GetBytes(i);
      if (BitConverter.IsLittleEndian)
      {
            Array.Reverse(bytes);
      }
      bytes.CopyTo(m_PacketData, 0);
      return true;
    }


    private static bool Write(int i)
    {
      // convert int to byte array
      char dataType = '整';
      var bytes = BitConverter.GetBytes(i);
      var type = BitConverter.GetBytes(dataType);
      _Write(type);
      _Write(bytes);
      return true;
    }

    // write a float into a byte array
    private static bool Write(float f)
    {
      // convert int to byte array
      char dataType = '浮';
      var bytes = BitConverter.GetBytes(f);
      var type = BitConverter.GetBytes(dataType);
      _Write(type); 
      _Write(bytes);
      return true;
    }

    // write a string into a byte array
    private static bool Write(string s)
    {
      // convert string to byte array
      var bytes = Encoding.Unicode.GetBytes(s);
      char dataType = '字';
      var type = BitConverter.GetBytes(dataType);
      _Write(type);
      var Len = BitConverter.GetBytes(bytes.Length);
      _Write(Len);
      _Write(bytes);
      return true;
    }

    // write a byte array into packet's byte array
    private static void _Write(byte[] byteData)
    {
      // converter little-endian to network's big-endian
      if (BitConverter.IsLittleEndian)
      {
        Array.Reverse(byteData);
      }

      byteData.CopyTo(m_PacketData, m_Pos);
      m_Pos += byteData.Length;
      WriteLen(m_Pos);
    }

    private static void Receive(byte[] m_PacketData)
        {
            byte[] readLenData = new byte[4];
            byte[] readAllData = new byte[m_Pos];

            for (int i = 0; i < 4; i++)
            {
                readLenData[i] = m_PacketData[i];            
            }
            Array.Reverse(readLenData);
            int len = BitConverter.ToInt32(readLenData, 0)-4;

            for (int i = 0; i + 4 < m_Pos; i++)
            {
                readAllData[i] = m_PacketData[i + 4];
            }
            Analyze(len, readAllData);      

        }

        private static void Analyze(int length ,byte[] readAllData)
        {
            Console.WriteLine("資料內容總長度：" + length + "\n\n資料內容：\n");

            byte[] readType = new byte[2];
            byte[] readIntData = new byte[4];
            byte[] readfloatData = new byte[4];
            int r_Pos = 0;  //已取出的資料點
            int len = 0;

            while (!(r_Pos == length))
            {
                len = 0;
                for (int i = r_Pos; r_Pos < i + 2; r_Pos++)
                {
                    readType[len] = readAllData[r_Pos];
                    len++;
                }
                switch (ReadType(readType))
                {
                    case '整':
                        len = 0;
                        for (int i = r_Pos; i < r_Pos + 4; i++)
                        {
                            readIntData[len] = readAllData[i];
                            len++;
                        }
                        r_Pos += 4;
                        Console.WriteLine("整");
                        Console.WriteLine(ReadInt(readIntData));
                        break;
                    case '浮':
                        len = 0;
                        for (int i = r_Pos; i < r_Pos + 4; i++)
                        {
                            readfloatData[len] = readAllData[i];
                            len++;
                        }
                        Console.WriteLine("浮");
                        Console.WriteLine(ReadFloat(readfloatData));
                        r_Pos += 4;
                        break;
                    case '字':
                        len = 0;
                        for (int i = r_Pos; i < r_Pos + 4; i++)
                        {
                            readIntData[len] = readAllData[i];
                            len++;
                        }
                        Console.WriteLine("字");
                        int stringLen = ReadInt(readIntData);
                        r_Pos += 4;

                        len = 0;
                        var readStringDate = new byte[stringLen];
                        for (int i = r_Pos; i < r_Pos + stringLen; i++)
                        {
                            readStringDate[len] = readAllData[i];
                            len++;
                        }
                        Console.WriteLine(ReadsString(readStringDate));
                        break;
                }
            }
        }
       private static char ReadType(byte[] type)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(type);
            }
            Char temp = BitConverter.ToChar(type,0);
            return temp;
        }

        private static int ReadInt( byte[] intdata)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(intdata);
            }
            int temp = BitConverter.ToInt32(intdata, 0);
            return temp;
        }

        private static float ReadFloat(byte[] floatdata)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(floatdata);
            }
            float  temp = BitConverter.ToSingle(floatdata,0);
            return temp;
        }
        private static string ReadsString(byte[] stringdata)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(stringdata);
            }
            string s = Encoding.Unicode.GetString(stringdata);
            return s;
        }
    }
}
