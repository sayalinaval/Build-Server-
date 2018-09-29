///////////////////////////////////////////////////////////////////////
// TestHarness.CS - Testing of executable files.                     //
// ver 1.0                                                           //
//                                                                   //
// Author: Sayali Naval, snaval@syr.edu                              //
// Source: Dr. Jim Fowcett                                           //
// Application: CSE681 Project 4-Build Server                        //
// Environment: C# console                                           //
///////////////////////////////////////////////////////////////////////
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MessagePassingComm;
using XmlParser;
using System.IO;
using System.Xml.Linq;
using System.Reflection;
using Logger;

namespace TestHarness
{
    public class TestHarness
    {
        Comm c1;
        logger log;

        public TestHarness()
        {
            c1 = new Comm("http://localhost", 8089);
            log = new logger("..//..//..//TestStorage//TestLog.txt");
        }
        public void receiveTestFiles()
        {
            while (true)
            {
                CommMessage c2 = c1.getMessage();
                c2.show();
                List<Tuple<string, string>> listTuple = new List<Tuple<string, string>>();
                List<string> n = new List<string>();
                //Receive test requests from child builder and parse the xml file to get dll file names
                if (c2.type.ToString() == "testRequest")
                {
                    string savePath = "../../../RepoStorage/TestRequest" + c2.body + ".xml";
                    string fileName = File.ReadAllText(savePath);
                    CreateXml tr = new CreateXml();
                    tr.doc = XDocument.Parse(fileName);
                    Console.Write("\n{0}", tr.doc.ToString());
                    Console.Write("\n");

                    tr.parseList("testDriver");
                    Console.Write("\ntest drivers are:");
                    foreach (string file in tr.testDriver)
                    {
                        Console.Write("\n    \"{0}\"", file);
                        n.Add(file);
                    }
                    foreach (string f1 in n)
                    {
                        Console.Write("\nfile name: " + f1);
                        //request test files to repository
                        CommMessage r1 = new CommMessage(CommMessage.MessageType.request);
                        r1.command = "show";
                        r1.author = "Jim Fawcett";
                        r1.file = f1;
                        r1.to = "http://localhost:8079/IPluggableComm";
                        r1.from = "http://localhost:8089/IPluggableComm";
                        r1.show();
                        c1.postMessage(r1);
                    }
                    Thread.Sleep(2000);
                    initiateLoadAndTest(n);
                    sendLogFile();
                    Console.WriteLine("\nTest Request Processed");
                }
            }
        }

        void sendLogFile()
        {
            string fromStorage1 = "..//..//..//TestStorage";
            string toStorage1 = "..//..//..//RepoStorage";
            c1.postFile("TestLog.txt", fromStorage1, toStorage1);
        }

        //------load test file from test file location and test request
        public void initiateLoadAndTest(List<string> files)
        {
            testersLocation = GuessTestersParentDir() + "/../TestStorage/";
            Console.Write("\ntesters location: " + testersLocation);
            // convert testers relative path to absolute path

            testersLocation = Path.GetFullPath(testersLocation);
            Console.Write("\n  Loading Test Modules from:\n    {0}\n", testersLocation);
            foreach (var testDriver in files)
            {
                Console.Write(testDriver);
                string result = loadAndExerciseTesters(testDriver);
                log.ErrorLog(result);

                Console.Write("\n\n  {0}", result);
                Console.Write("\n\n");

            }
        }

        public static string testersLocation { get; set; } = ".";

        static Assembly LoadFromComponentLibFolder(object sender, ResolveEventArgs args)
        {
            Console.Write("\n  Called binding error event handler");
            string folderPath = testersLocation;
            string assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
            if (!File.Exists(assemblyPath)) return null;
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;
        }
        //----load assemblies from testersLocation and run their tests

        public string loadAndExerciseTesters(string dllFile)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromComponentLibFolder);

            try
            {
                // load each assembly found in testersLocation

                string[] files = Directory.GetFiles(testersLocation, "*.dll");

                Assembly asm = Assembly.Load(File.ReadAllBytes(testersLocation + dllFile));
                // exercise each tester found in assembly
                Type[] types = asm.GetTypes();
                foreach (Type t in types)
                {
                    // if type supports ITest interface then run test
                    if (t.GetInterface("ConsoleApp1.ITest", true) != null)
                    {
                        if (!runSimulatedTest(t, asm))
                        {
                            Console.Write("\n  Test {0} failed to run", t.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "Simulated test completed";
        }

        //----run tester t from assembly asm

        bool runSimulatedTest(Type t, Assembly asm)
        {
            try
            {
                Console.Write(
                  "\n  Attempting to create instance of {0}", t.ToString()
                  );
                object obj = asm.CreateInstance(t.ToString());

                // announce test

                MethodInfo method = t.GetMethod("add");
                if (method != null)
                    method.Invoke(obj, new object[0]);

                // run test

                bool status = false;
                method = t.GetMethod("test");
                if (method != null)
                    status = (bool)method.Invoke(obj, new object[0]);

                Func<bool, string> act = (bool pass) =>
                {
                    if (pass)
                        return "Passed";
                    return "Failed";
                };
                Console.Write("\n  Test {0}", act(status));             
            }
            catch (Exception ex)
            {
                Console.Write("\n  Test failed with message \"{0}\"", ex.Message);
                return false;
            }
            return true;
        }

        //----extract name of current directory without its parents

        public string GuessTestersParentDir()
        {
            string dir = Directory.GetCurrentDirectory();
            int pos = dir.LastIndexOf(Path.DirectorySeparatorChar);
            string name = dir.Remove(0, pos + 1).ToLower();
            if (name == "debug")
                return "../..";
            else
                return "../..";
        }
        //----run demonstration

    static void Main(string[] args)
        {
            Console.Title = "TestHarness";
            TestHarness s = new TestHarness();
            s.receiveTestFiles();
        }
    }
}
