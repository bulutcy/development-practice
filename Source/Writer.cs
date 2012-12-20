using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace SWE_573_RWPA
{
    class Writer
    {

        private int writersDelay;
        private Semaphore threadCounter;
        public Writer(int writersDelay, Semaphore threadCounter)
        {
            this.writersDelay = writersDelay;
            this.threadCounter = threadCounter;
        }

        internal Data Data
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
    
        public void Run()
        {

            //create needed mutexes
            Mutex programEndsMutex = new Mutex(true, "programEnds");
            Semaphore wrt = Semaphore.OpenExisting("writerMutex");
            Semaphore currentRecordCount = Semaphore.OpenExisting("currentRecordCount");
            //set thread culture info in order to make easier logs
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            bool programEnds = false;
            while (!programEnds)
            {         
               //try and wait for wrt writers mutex
              
                try
                {
                    wrt.WaitOne();
                    currentRecordCount.Release();//increment current record count         
                    Data myData = new Data();
                    Random rand = new Random();
                    Thread.Sleep(rand.Next(0, writersDelay));//do something
                    short recordNum = (short)rand.Next(1, Program.RECORDCOUNT+1);
                    while (myData.writeData(recordNum) == false)
                    {
                        recordNum = (short)rand.Next(1, Program.RECORDCOUNT+1);
                        //Console.WriteLine(Thread.CurrentThread.ManagedThreadId.ToString() + ":" + recordNum);
                    };

                    //log
                    Logger.Log(Thread.CurrentThread.ManagedThreadId.ToString(), "Writer finished writing");
                    //release writers mutex
                 
                    wrt.Release();

                }
                catch (Exception e)
                {
                    wrt.Release();
                }
                //check if program should end
                if (programEndsMutex.WaitOne(0))
                {
                    programEndsMutex.ReleaseMutex();
                    programEnds = true;
                }

            }
            //thread will end, decrease threadCounter semaphore
            threadCounter.WaitOne();
      
        }
     
    }
}
