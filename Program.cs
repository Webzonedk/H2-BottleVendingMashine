using System;
using System.Threading;
using System.Collections.Generic;

namespace BottleAutomat
{
    class Program
    {
        //Please be aware that the console output will be cheating a little, and sometimes telling that a buffer is full or empty
        //right after another worker has put something in the buffer.
        //This is caused by the nature of the console, that the threads are much faster than the console output,
        //and is already running the next part of the code before the console is able to write the full sentences.
        //A bit annnoyin, I know, But I don't wish to add futher delays within the code.

        static void Main(string[] args)
        {
            //Initializing the workers
            Worker one = new Worker("Entry");
            Worker two = new Worker("Splitter");
            Worker three = new Worker("beerConsumer");
            Worker four = new Worker("sodaConsumer");

            //Creating threads with each worker doing their respective methods
            Thread bottleProducer = new Thread(one.RecieveBottles);
            Thread bottleSplitter = new Thread(two.SplitBottles);
            Thread beerConsumer = new Thread(three.GetBeers);
            Thread sodaConsumer = new Thread(four.GetSoda);

            //Strarting the Threads
            bottleProducer.Start();
            bottleSplitter.Start();
            beerConsumer.Start();
            sodaConsumer.Start();

        }

       
    }
}
