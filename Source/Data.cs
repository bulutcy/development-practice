using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;


namespace SWE_573_RWPA
{
    class Data
    {


        private short recordNum;
        private char[] dateTime = new char[17];
        private int pid;
        private char[] key = new char[16];
        private char[] encData;
        private string data;
        public Data()
        {

        }

        internal CryptoHelper CryptoHelper
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        internal Logger Logger
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
    
        public bool readData(short recordNum)
        {
            System.IO.FileStream fileStream = new System.IO.FileStream("data.rwpa", System.IO.FileMode.Open, System.IO.FileAccess.Read);
            //byte[] byteArray = new byte[byteCount];
            // seek to position of data
            fileStream.Seek((recordNum - 1) * Program.BYTECOUNT, System.IO.SeekOrigin.Begin);
            //Read data
            byte[] tempArr = new byte[2];
            fileStream.Read(tempArr, 0, tempArr.Length);
            this.recordNum = BitConverter.ToInt16(tempArr, 0);
            if (this.recordNum == 0) // if record num is null, close file and return not found signal=false
            {
                // close file stream
                fileStream.Close();
                return false;
            }
            else
            {
                //read record attributes
                tempArr = new byte[17];
                fileStream.Read(tempArr, 0, tempArr.Length);
                this.dateTime = Encoding.Default.GetChars(tempArr);

                tempArr = new byte[4];
                fileStream.Read(tempArr, 0, tempArr.Length);
                this.pid = BitConverter.ToInt32(tempArr, 0);

                byte[] arrKey = new byte[16];
                fileStream.Read(arrKey, 0, arrKey.Length);
                this.key = Encoding.Default.GetChars(arrKey);

                byte[] arrEncData = new byte[Program.BYTECOUNT - 39];
                fileStream.Read(arrEncData, 0, arrEncData.Length);
                this.encData = Encoding.Default.GetChars(arrEncData);

                //CryptoHelper cryptoHelper = new CryptoHelper(arrKey);
              // this.data = cryptoHelper.Decrypt(arrEncData);
                this.data = CryptoHelper.DecryptStringFromBytes_Aes(arrEncData, Encoding.Default.GetBytes(this.key), Encoding.Default.GetBytes(this.key));
            
                // close file stream
                fileStream.Close();
                Logger.Log(Thread.CurrentThread.ManagedThreadId.ToString(), "Reader has read the record: " + recordNum +
                                                                        ", with key :'" + new string(this.key) + "'" +
                                                                        ", data:'" + this.data + "'");
                return true;

            }
           
        }
        public bool writeData(short recordNum)
        {
            System.IO.FileStream fileStream = new System.IO.FileStream("data.rwpa", System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite);
            byte[] byteArray = new byte[Program.BYTECOUNT];
            // seek to position of data
            fileStream.Seek((recordNum - 1) * Program.BYTECOUNT, System.IO.SeekOrigin.Begin);
            //Write data
            byte[] tempArr = new byte[2];
            fileStream.Read(tempArr, 0, tempArr.Length);
            short oldRecordNum = BitConverter.ToInt16(tempArr, 0);
            //check if record already exist-do not overwrite
            if (oldRecordNum != 0)
            {
                fileStream.Close();
                return false;
            }
            else
            {
                // seek back
                fileStream.Seek((recordNum - 1) * Program.BYTECOUNT, System.IO.SeekOrigin.Begin);

                fileStream.Write(BitConverter.GetBytes(recordNum), 0, 2);
                DateTime dt = DateTime.Now;
                fileStream.Write(Encoding.Default.GetBytes(String.Format("{0:yyyymmddHHmmssfff}", dt)), 0, 17);
                fileStream.Write(BitConverter.GetBytes(Thread.CurrentThread.ManagedThreadId), 0, 4);
                char[] strKey = this.getRandomString(16);
                fileStream.Write(Encoding.Default.GetBytes(strKey), 0, 16);
                char[] strData = this.getRandomString(Program.BYTECOUNT - 39);
             //   CryptoHelper cryptoHelper = new CryptoHelper(Encoding.Default.GetBytes(strKey));
               // byte[] cypher = cryptoHelper.Encrypt(Encoding.Default.GetBytes(strData));
                byte[] cypher = CryptoHelper.EncryptStringToBytes_Aes(new string(strData), Encoding.Default.GetBytes(strKey), Encoding.Default.GetBytes(strKey));
             
               
                fileStream.Write(cypher, 0, cypher.Length);
                fileStream.Close();

                Logger.Log(Thread.CurrentThread.ManagedThreadId.ToString(), "Writer have writed to record: " + recordNum +
                                                                            ", with key :'" + new string(strKey) + "'" +
                                                                            ", data:'" + new string(strData) + "'");
                return true;
            }                                      
        }

        private char[] getRandomString(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];
            DateTime dt = DateTime.Now;
            var random = new Random(length+(int)dt.Ticks);

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return stringChars;
        }

        public void makeNull(short recordNum)
        {
            System.IO.FileStream fileStream = new System.IO.FileStream("data.rwpa", System.IO.FileMode.Open, System.IO.FileAccess.Write);
            byte[] byteArray = new byte[Program.BYTECOUNT];
            // seek to position of data
            fileStream.Seek((recordNum - 1) * Program.BYTECOUNT, System.IO.SeekOrigin.Begin);
            //Write data
            fileStream.Write(byteArray, 0, byteArray.Length);
            fileStream.Close();
            Logger.Log(Thread.CurrentThread.ManagedThreadId.ToString(), "Reader has nulled the record: " + recordNum);
        }

        public static void createDataFile()
         {
             // Open file

            System.IO.FileStream fileStream = new System.IO.FileStream("data.rwpa", System.IO.FileMode.Create, System.IO.FileAccess.Write);
            byte[] byteArray = new byte[Program.BYTECOUNT];
            // Writes a block of bytes to this stream using data from a byte array.
            for (int i = 0; i < Program.RECORDCOUNT; i++)
            {
                fileStream.Write(byteArray, 0, byteArray.Length);
            }
            // close file stream
            fileStream.Close();

        }
    }
}
