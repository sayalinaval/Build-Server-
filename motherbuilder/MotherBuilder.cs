////////////////////////////////////////////////////////////////////////////////
// MotherBuilder.CS - Mother Builder receives build requests from client      //
// It creates Child Builders and spawns the requests to child builders        //
// ver 1.0                                                                    //
//                                                                            //
// Author: Sayali Naval, snaval@syr.edu                                       //
// Source: Dr. Jim Fowcett                                                    //
// Application: CSE681 Project 4-Build Server                                 //
// Environment: C# console                                                    //
////////////////////////////////////////////////////////////////////////////////
//*
//*
//Mother Builder creates processes for child builder.
//It receives test requests from Client and sends to child builder which is ready.
//The number of child processes created is determined by client through GUI.
//Port number for Mother Builder is 8080.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SWTools;
using System.Diagnostics;
using System.IO;
using MessagePassingComm;

namespace Project3
{
    class MotherBuilder
    {       
        BlockingQueue<int> readyQ;       
        BlockingQueue<string> buildQ;       

        public MotherBuilder()
        {
            //Ready Quese which contains the port number for child builders which are ready
            readyQ = new BlockingQueue<int>();
            //Build Queue which contains build requests received from client
            buildQ = new BlockingQueue<string>();
        }
        //create child builder processes
        static bool createProcess(int i)
        {
            Process proc = new Process();
            string fileName = "..\\..\\..\\ConsoleApp1\\bin\\debug\\ConsoleApp1.exe";
            string absFileSpec = Path.GetFullPath(fileName);

            Console.Write("\n  attempting to start {0}", absFileSpec);
            int j = i - 8080;
            string commandline = j.ToString();
            Console.Write("\n  command line {0}", commandline);
            try
            {
                Process.Start(fileName, commandline);
            }
            catch (Exception ex)
            {
                Console.Write("\n  {0}", ex.Message);
                return false;
            }

            return true;
        }
        //send messages from mother builder to child builders
        public void motherToChild(int count)
        {
            //create connection for mother builder at port 8080
            Comm c1 = new Comm("http://localhost", 8080);
            //set up connection between mother builder and child builders
            for (int i = 8081; i <= (8080+count); ++i)
            {
                CommMessage csndMsg1 = new CommMessage(CommMessage.MessageType.request);
                csndMsg1.command = "show";
                csndMsg1.author = "Jim Fawcett";
                csndMsg1.to = "http://localhost:" + i + "/IPluggableComm";
                csndMsg1.from = "http://localhost:8080/IPluggableComm";
                c1.postMessage(csndMsg1);
            }
            //create thread to run processes simultaniously
            Thread t = new Thread(() =>
            {
                //dequeue build requests from build queue to child builder whose port number you dequeue from ready queue
                while (true)
                {
                    string x = buildQ.deQ();
                    CommMessage csndMsg = new CommMessage(CommMessage.MessageType.buildRequest);
                    csndMsg.command = "show";
                    csndMsg.author = "Jim Fawcett";
                    csndMsg.type = CommMessage.MessageType.buildRequest;
                    int port = readyQ.deQ();
                    csndMsg.to = "http://localhost:" + port + "/IPluggableComm";
                    csndMsg.from = "http://localhost:" + "8080" + "/IPluggableComm";
                    csndMsg.body = x;
                    csndMsg.show();
                    c1.postMessage(csndMsg);
                }

            });
            t.Start();
            //receive messages
            while (true)
            {
                CommMessage c2 = c1.rcvr.getMessage();
                c2.show();
                //if message is a build request, enqueue the build request to build queue
                if (c2.type.ToString() == "buildRequest")
                {
                      buildQ.enQ(c2.body);
                }
                //if message is ready message from child builder, enqueue child port number to ready queue
                if (c2.type.ToString() == "ready")
                {
                    readyQ.enQ(Int32.Parse(c2.body));
                }
                //if message is close message from client, close mother builder
                if (c2.type.ToString() == "close")
                {
                    Console.WriteLine("\nQuit Mother Builder\n");
                }              
            }

        }

        static void Main(string[] args)
        {
            Console.Title = "MotherBuilder";
            int count = Int32.Parse(args[0]);
            if (args.Count() == 0)
            {
                Console.Write("\n  please enter number of children on command line");
                return;
            }
            else
            {           
                //create child builder processes
                for (int i = 8081; i <= (8080+count); ++i)
                {
                    if (createProcess(i))
                    {
                        Console.Write(" - succeeded");
                    }
                    else
                    {
                        Console.Write(" - failed");
                    }
                }
            }
            MotherBuilder m1 = new MotherBuilder();
            m1.motherToChild(count);
            
            Console.Write("\n  Press key to exit");
            Console.ReadKey();
            Console.Write("\n  ");
        }
    }
}
