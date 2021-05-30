using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace BottleAutomat
{
    class Worker
    {
        public static Random random = new Random(); //Initialising a random object

        //Sleping timers if random is not activated
        public static int producerSleep = 0;
        public static int splitterSleep = 0;
        public static int beerSleep = 0;
        public static int sodaSleep = 0;

        //Random sleep timers to visualise the flow
        public static bool randomSleep = false; //Set to true to use the random sleep timers
        public static int producerMin = 100;
        public static int producerMax = 1000;
        public static int splitterMin = 100;
        public static int splitterMax = 1000;
        public static int beerMin = 100;
        public static int beerMax = 1000;
        public static int sodaMin = 100;
        public static int sodaMax = 1000;

        //Queues to be used as buffers
        public static volatile Queue<Bottle> bufferToSplitter = new Queue<Bottle>();
        public static volatile Queue<Bottle> bufferToBeer = new Queue<Bottle>();
        public static volatile Queue<Bottle> bufferToSoda = new Queue<Bottle>();
        //Variable to adjust the buffer sizes
        public static int maxBottleBuffer = 10;
        public static int maxBeerBuffer = 5;
        public static int maxSodaBuffer = 5;

        //The serialnumber
        public static int serialNumber;
        //Counters to keep track of the bottles
        public static int totalBottlesIn;
        public static int beerBottleCounter;
        public static int sodaBottleCounter;




        //fields
        private string name;

        //Properties
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        //Constructors
        public Worker()
        {

        }

        public Worker(string name)
        {
            this.name = name;
        }

        //Method to create/(revive bottles from customer) and put them into the splitter buffer.
        internal void RecieveBottles()
        {
            while (true)//Loop loop
            {

                Monitor.Enter(bufferToSplitter);//Locking the producer thread
                Bottle bottle = new();//New instance of a bootle

                if (bufferToSplitter.Count < maxBottleBuffer)
                {
                    int bottletype = random.Next(0, 2);//If splitter buffer is not full,a new bottle is created with either beer or soda by a random of 0 or 1
                    if (bottletype == 0)
                    {
                        bottle.Type = "Beer";
                    };
                    if (bottletype == 1)
                    {
                        bottle.Type = "Soda";
                    };
                    serialNumber++;//Creating next serialnumber by adding one to the existing one
                    bottle.SerialNumber = serialNumber;//Adding the serial nuber to the bottle
                    bufferToSplitter.Enqueue(bottle);//adding the bottle to the splitter buffer
                    totalBottlesIn++;//Keeping track of the bottles
                    Console.WriteLine($"bottleProducer sends {bottle.Type} bottle, with serialNumber: {bottle.SerialNumber} To splitter buffer, Total bottles send: {totalBottlesIn}");
                    Console.WriteLine($"Bottles in splitter buffer: {bufferToSplitter.Count}");
                };

                if (bufferToSplitter.Count == maxBottleBuffer)//If splitter buffer is full, set thread to wait and notify the connsole that it is full
                {
                    Monitor.Wait(bufferToSplitter);//Set thread to wait state
                    Console.WriteLine($"bottleProducer is now waiting as the splitter buffer is full ");
                };

                Monitor.Pulse(bufferToSplitter);//Notify the waiting bottleSplitter thread to get ready to start the engine
                Monitor.Exit(bufferToSplitter);//Relesing the lock

                if (!randomSleep)//put thread to random sleep if Random is true in the variables
                {
                    int randomProducerSleep = random.Next(producerMin, producerMax);
                    Thread.Sleep(randomProducerSleep);
                }
                else//put thread to fixed sleep if random is false in the variables
                {
                    Thread.Sleep(producerSleep);
                }
            };
        }

        //Splitter method. Taking the bottles from the splitter buffer and put it to either the beer buffer or the soda buffer
        internal void SplitBottles()
        {
            while (true)//Loop loop
            {
                Monitor.Enter(bufferToSplitter);//Locking the splitter thread
                Bottle bottle = new();//New instance of a bootle

                if (bufferToSplitter.Count > 0)//If the splitter buffer is not empty
                {
                    bottle = bufferToSplitter.Dequeue();//take out a bottle from the queue
                    Console.WriteLine($"bottleSplitter removed {bottle.Type} bottle, with serialNumber:{bottle.SerialNumber} from splitter buffer ");
                    Console.WriteLine($"Bottles in splitterbuffer: {bufferToSplitter.Count}");
               

                };

                if (bufferToSplitter.Count == 0)//If splitter buffer is empty
                {
                    Monitor.Wait(bufferToSplitter);//Set thread to wait state
                    Console.WriteLine($"bottleSplitter is now waiting as the splitter buffer is empty");
                };

                Monitor.Pulse(bufferToSplitter);//Notify the waiting bottleProducer thread to get ready to start the engine
                Monitor.Exit(bufferToSplitter);//Relesing the lock

                //Moving the bottle to respective new buffer
                if (bottle.Type == "Beer")
                {
                    Monitor.Enter(bufferToBeer);//Locking the beerConsumer thread

                    if (bufferToBeer.Count < maxBeerBuffer)//If beerbuffer is not full
                    {
                        bufferToBeer.Enqueue(bottle);//adding the bottle to the beer buffer
                        Console.WriteLine($"bottleSplitter moved {bottle.Type} bottle, with serialNumber:{bottle.SerialNumber} to Beer Buffer ");
                        Console.WriteLine($"Bottles in beer buffer: {bufferToBeer.Count}");

                    };

                    if (bufferToBeer.Count == maxBeerBuffer)//If beerbuffer is full
                    {
                        Monitor.Wait(bufferToBeer);//Set thread to wait state
                        Console.WriteLine($"bottleSplitter is now waiting as the Beer buffer is full");
                    };

                    Monitor.Pulse(bufferToBeer);//Notify the waiting beerConsumer thread to get ready to start the engine
                    Monitor.Exit(bufferToBeer);//Relesing the lock
                };


                if (bottle.Type == "Soda")
                {
                    Monitor.Enter(bufferToSoda);//Locking the sodaConsumer thread

                    if (bufferToSoda.Count < maxSodaBuffer)//If soda buffer is not full
                    {
                        bufferToSoda.Enqueue(bottle);//adding the bottle to the soda buffer
                        Console.WriteLine($"bottleSplitter moved {bottle.Type} bottle, with serialNumber:{bottle.SerialNumber} to Soda Buffer ");
                        Console.WriteLine($"Bottles in Soda buffer: {bufferToSoda.Count}");

                    };

                    if (bufferToSoda.Count == maxSodaBuffer)//if sodabuffer is full
                    {
                        Monitor.Wait(bufferToSoda);//Set thread to wait state
                        Console.WriteLine($"bottleSplitter is now waiting as the Soda buffer is full");
                    };

                    Monitor.Pulse(bufferToSoda);//Notify the waiting sodaConsumer thread to get ready to start the engine
                    Monitor.Exit(bufferToSoda);//Relesing the lock
                };

                if (!randomSleep)//put thread to random sleep if Random is true in the variables
                {
                    int randomSplitterSleep = random.Next(splitterMin, splitterMax);
                    Thread.Sleep(randomSplitterSleep);
                }
                else//put thread to fixed sleep if random is false in the variables
                {
                    Thread.Sleep(splitterSleep);
                };
            };
        }

        //Method to get beer bottles from the beer buffer
        internal void GetBeers()
        {
            while (true)//Loop loop
            {
                Monitor.Enter(bufferToBeer);//Locking the beerConsumer thread
                Bottle bottle;//new instanace of a bootle object to contain the info of the bottle

                if (bufferToBeer.Count > 0)//If soda buffer is not empty
                {
                    bottle = bufferToBeer.Dequeue();//Take the bottle to the beer buffer
                    beerBottleCounter++;//Keeping count of the recieved beer bottles
                    Console.WriteLine($"Beer Consumer recieved a {bottle.Type} bottle, with serialNumber: {bottle.SerialNumber} ----- Total {bottle.Type} bottles handled: {beerBottleCounter} ----- Total bottles handled: {beerBottleCounter + sodaBottleCounter}");
                    Console.WriteLine($"Bottles left in Beer buffer: {bufferToBeer.Count}");
                };

                if (bufferToBeer.Count == 0)//If beer buffer is empty
                {
                    Monitor.Wait(bufferToBeer);//Set beerConsumer thread to wait state
                    Console.WriteLine($"Beer Consumer is now waiting as the Beer buffer is empty");
                };

                Monitor.Pulse(bufferToBeer);//Notify the waiting bottleSplitter thread to get ready to start the engine
                Monitor.Exit(bufferToBeer);//Relesing the lock

                if (!randomSleep)//put thread to random sleep if Random is true in the variables
                {
                    int randomBeerSleep = random.Next(beerMin, beerMax);
                    Thread.Sleep(randomBeerSleep);
                }
                else//put thread to fixed sleep if random is false in the variables
                {
                    Thread.Sleep(beerSleep);
                };
            };
        }

        //Method to get soda bottle from the soda buffer
        internal void GetSoda()
        {
            while (true)//Loop loop
            {
                Bottle bottle; //new instanace of a bootle object to contain the info of the bottle
                Monitor.Enter(bufferToSoda);//Locking the sodaConsumer thread

                if (bufferToSoda.Count > 0)//If soda buffer is not empty
                {
                    bottle = bufferToSoda.Dequeue();//Take the bottle to the soda buffer
                    sodaBottleCounter++;//Keeping count of the recieved soda bottles
                    Console.WriteLine($"Soda Consumer recieved a {bottle.Type} bottle ----- serialNumber: {bottle.SerialNumber} ----- Total {bottle.Type} bottles handled: {sodaBottleCounter} ----- Total bottles handled: {beerBottleCounter + sodaBottleCounter}");
                    Console.WriteLine($"Bottles left in Soda buffer: {bufferToSoda.Count}");
                };

                if (bufferToSoda.Count == 0)//If soda buffer is empty
                {
                    Monitor.Wait(bufferToSoda);//Set sodaConsumer thread to wait state
                    Console.WriteLine($"Soda Consumer is now waiting as the Soda buffer is empty");
                };

                Monitor.Pulse(bufferToSoda);//Notify the waiting bottleSplitter thread to get ready to start the engine
                Monitor.Exit(bufferToSoda);//Relesing the lock

                if (!randomSleep)//put thread to random sleep if Random is true in the variables
                {
                    int randomSodaSleep = random.Next(sodaMin, sodaMax);
                    Thread.Sleep(randomSodaSleep);
                }
                else//put thread to fixed sleep if random is false in the variables
                {
                    Thread.Sleep(sodaSleep);
                };
            };
        }
    }
}
