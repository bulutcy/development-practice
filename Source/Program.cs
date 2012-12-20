using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

/*
 * 
 
 * The purpose of the software project is to develop an application in C#, Windows environment, to do mainly the following.

1.	Simulate Readers&Writers problem.
2.	Log every action to a file.

The design is based on RWPA Requirements Specification Document, Revision 1.1, in file Korkmaz-P1-RWPA-RSD-2012-11-05-Rev-1.1. 
  and RWPA Design Specification Document, Revision 1.1, in file Korkmaz-P1-RWPA-DSD-2012-11-05-Rev-1.1. 

 


*/

namespace SWE_573_RWPA
{
    class Program
    {
        public static int BYTECOUNT;
        public static int RECORDCOUNT;

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

        internal Writer Writer
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        internal Reader Reader
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
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
    
        static void Main(string[] args)
        {

            //mutex signal gfor sending end signal to child threads
            Mutex programEndsMutex = new Mutex(true, "programEnds");
            Semaphore wrt = new Semaphore(1, 1, "writerMutex");
           //set config file
            AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", "config.rwpa");

            //read values from config file
            int readersNumber = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["readersNumber"]);
            int writersNumber = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["writersNumber"]);
            int readersDelay = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["delayN"]);
            int writersDelay = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["delayM"]);
            int demoLength = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["demoLength"]);
            BYTECOUNT = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["BYTECOUNT"]);
            RECORDCOUNT = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings["RECORDCOUNT"]);
            //multiple value mutex(semaphore) for counting threads
            Semaphore currentRecordCount = new Semaphore(0, RECORDCOUNT, "currentRecordCount");
            Semaphore threadCounter = new Semaphore(readersNumber + writersNumber, readersNumber + writersNumber, "threadCounter");
    
            string logFile = @"log.rwpa";

            //create log file if already not exist
            if (!File.Exists(logFile))
            {
                Logger.sw = File.CreateText(logFile);
                Logger.Log(Thread.CurrentThread.ManagedThreadId.ToString(), "Log file is created.");
                Logger.sw.Close();
            
            }

            //open log file in append mode
            Logger.sw = File.AppendText(logFile);
            Logger.Log(Thread.CurrentThread.ManagedThreadId.ToString(), "Program starts.");

            //create data file
            Data.createDataFile();
            Logger.Log(Thread.CurrentThread.ManagedThreadId.ToString(), "Data file is created with " + RECORDCOUNT + " records, total of: " + (RECORDCOUNT * BYTECOUNT)+" bytes.");

            //create writer processes
            for (int c = 0; c < writersNumber; c++)
            {
                Writer thr = new Writer(writersDelay, threadCounter);
                Thread thread = new Thread(new ThreadStart(thr.Run));
                thread.Start();



            }
            //create reader processes
            for (int c = 0; c < readersNumber; c++)
            {
                Reader thr = new Reader(readersDelay, threadCounter);
                Thread thread = new Thread(new ThreadStart(thr.Run));
                thread.Start();



            }
      
            //wait X minutes to demo ends
            Thread.Sleep(demoLength * 60 * 1000);

            //release programEnds mutexin order to send signal to child threads
           programEndsMutex.ReleaseMutex();

            //checking if all threads have ended properly
           bool threadsEnded = false;
           while (!threadsEnded)
           {
               //try to decrement threadCounter semaphore
               if (threadCounter.WaitOne(0))
               {
                   threadCounter.Release();
               }
               else//if decrement fails, it means all threads are ended
               {
                   threadsEnded = true;
               }
               Thread.Sleep(50);

           }
            //close log file
           Logger.Log(Thread.CurrentThread.ManagedThreadId.ToString(), "Program ends.");
           Logger.sw.Close();
         
         
        }

    }
}
