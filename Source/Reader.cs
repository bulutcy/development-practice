using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;


namespace SWE_573_RWPA
{
    class Reader
    {
        static int rc = 0;


        private int readersDelay;

        private Semaphore threadCounter;

        public Reader(int readersDelay, Semaphore threadCounter)
        {
            this.readersDelay = readersDelay;
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
            Mutex mutex = new Mutex(false, "readerMutex");
            Semaphore wrt = Semaphore.OpenExisting("writerMutex");
            Semaphore currentRecordCount = Semaphore.OpenExisting("currentRecordCount");
            //set thread culture info in order to make easier logs         
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            bool programEnds = false;
            while (!programEnds)
            {
                //readers critical section start
                mutex.WaitOne();
                rc++;
                if (rc == 1)//if this thread is the only active reader, prevent writers
                    wrt.WaitOne();

                if (!currentRecordCount.WaitOne(0)) //decrement current record count
                {
                    rc--;
                    if (rc == 0)//if no more active readers, release writers mutex
                        wrt.Release();
                    mutex.ReleaseMutex();
                }
                else
                {
                

                    mutex.ReleaseMutex();
                    //readers critical section end
                    Data myData = new Data();
                    Random rand = new Random();
                    Thread.Sleep(rand.Next(0, readersDelay));//do something
                    //readers critical section start
                    mutex.WaitOne();
                    short recordNum = (short)rand.Next(1, Program.RECORDCOUNT + 1);
                    while (myData.readData(recordNum) == false)
                    {
                        recordNum = (short)rand.Next(1, Program.RECORDCOUNT + 1);
                        //Console.WriteLine(Thread.CurrentThread.ManagedThreadId.ToString() + ":" + recordNum);
                    };

                    //log
                    Logger.Log(Thread.CurrentThread.ManagedThreadId.ToString(), "Reader finished reading");
                    myData.makeNull(recordNum);
                    rc--;
                    if (rc == 0)//if no more active readers, release writers mutex
                        wrt.Release();
                    mutex.ReleaseMutex();
                    //readers critical section end
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
