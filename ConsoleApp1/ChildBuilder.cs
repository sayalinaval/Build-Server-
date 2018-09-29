///////////////////////////////////////////////////////////////////////
// ChildBuilder.CS - Child Builders created by Mother Builder        //
// ver 1.0                                                           //
//                                                                   //
// Author: Sayali Naval, snaval@syr.edu                              //
// Source: Dr. Jim Fowcett                                           //
// Application: CSE681 Project 4-Build Server                        //
// Environment: C# console                                           //
///////////////////////////////////////////////////////////////////////

//Child Builder is created by Mother Builder and receives Build Requests from Mother Builder.
//It parses the Build Requests and requests files from Repository.
//Repository sends the requested files to Child Builder which child will later build.
//Child Builders have port numbers from 8081 to 8086.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Threading;
using MessagePassingComm;
using RepoStorage;
using XmlParser;
using System.Xml.Linq;
using Logger;
using System.Xml;

namespace ChildBuilder
{
    class ChildBuilder
    {
        static Comm c1;
        static logger log;

        //----Send ready messages to Mother Builder
        static void readyMsgToMother(int port)
        {
            CommMessage csndMsg = new CommMessage(CommMessage.MessageType.ready);
            csndMsg.command = "show";
            csndMsg.author = "Jim Fawcett";
            csndMsg.to = "http://localhost:8080/IPluggableComm";
            csndMsg.from = "http://localhost:" + port + "/IPluggableComm";
            csndMsg.body = port.ToString();
            c1.postMessage(csndMsg);                      
        }

        //-----read test files and build test driver to create dll files and send log to repository
        public static void SendToBuild(List<string> m, int port)
        {
            string testdriver = m[0];
            m.RemoveAt(0);
            BuildCs(testdriver, m, port);
        }

        public static void BuildCs(string testDriver, List<string> l, int port)
        {
            Console.Write("\n\nBuilding file");
            Console.Write("\n\n=================\n\n");
            string testFiles = "";
            foreach (string file in l)
            {
                testFiles += file;
                testFiles += " ";
            }

            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            p.StartInfo.Arguments = "/Ccsc /target:library " + testDriver;
            foreach (string x in l)
            {
                p.StartInfo.Arguments += " " + x;
            }
            Console.Write("print this" + p.StartInfo.Arguments);
            p.StartInfo.WorkingDirectory = "../../../BuildChild" + port.ToString();
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.WaitForExit();

            //send dll files to Repo storage
            sendDllFiles(testDriver, port);

            string errors = p.StandardError.ReadToEnd();
            string output = p.StandardOutput.ReadToEnd();

            log.ErrorLog(errors);
            log.ErrorLog(output);

            Console.WriteLine("\n\n Build Process Completed");
            Console.WriteLine("\n\n error = " + errors);
            Console.WriteLine("\n\n output = " + output);
        }

        static void sendDllFiles(string testDriver, int port)
        {
            string d = testDriver.Split('.')[0] + ".dll";
            string fromStorage1 = "../../../BuildChild" + port.ToString();
            string toStorage1 = "..//..//..//RepoStorage";
            c1.postFile(d, fromStorage1, toStorage1);
        }

        static void sendLogFile(int port)
        {
            string fromStorage1 = "../../../BuildChild" + port.ToString();
            string toStorage1 = "..//..//..//RepoStorage";
            c1.postFile("BuildLog"+port+".txt", fromStorage1, toStorage1);
        }

        static void Main(string[] args)
        {
            Console.Title = "ChildBuilder";         
            Console.Write("\nChild #{0} ", args[0]);
            int count = Int32.Parse(args[0]);
            int port = count + 8080;
            Console.WriteLine("\nPort # " + port);
            log = new logger("..//..//..//BuildChild" + port + "//BuildLog"+port+".txt");
            string toPath = "../../../BuildChild" + port.ToString();
            System.IO.Directory.CreateDirectory(Path.GetFullPath(toPath)).ToString();
            //create connection for child builder
            c1 = new Comm("http://localhost", port);

            readyMsgToMother(port);

            while (true)
            {
                CommMessage c2 = c1.getMessage();
                c2.show();
                List<Tuple<string, string>> listTuple = new List<Tuple<string, string>>();
                List<string> n = new List<string>();
                //Receive build requests from mother builder and parse the xml file to get test file names
                if (c2.type.ToString() == "buildRequest")
                {
                    string savePath = "../../../RepoStorage/BuildRequest"+c2.body+".xml";
                    Console.Write("\nsave path is: " + savePath);
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

                    tr.parseList("tested");
                    Console.Write("\n  testedFiles are:");
                    foreach (string file in tr.testedFiles)
                    {
                        Console.Write("\n    \"{0}\"", file);
                        n.Add(file);
                    }
                    foreach (string f1 in n)
                    {
                        //request test files to repository
                        CommMessage r1 = new CommMessage(CommMessage.MessageType.fileTransfer);
                        r1.command = "show";
                        r1.author = "Jim Fawcett";
                        r1.body = toPath;
                        r1.file = f1;
                        r1.to = "http://localhost:8079/IPluggableComm";
                        r1.from = "http://localhost:" + port + "/IPluggableComm";
                        r1.show();
                        c1.postMessage(r1);
                    }
                    Thread.Sleep(2000);

                    //build files
                    SendToBuild(n, port);

                    sendLogFile(port);

                    Thread.Sleep(2000);
                    
                    //create test request
                    CreateXml x1 = new CreateXml();
                    CreateXml tr1 = new CreateXml();
                    tr1.loadXml(savePath);
                    x1.author = tr1.parse("author");
                    List<string> s1 = new List<string>();
                    foreach (string t1 in tr1.parseList("testDriver"))
                    {
                        string t2 = t1.Split('.')[0] + ".dll";
                        s1.Add(t2);
                        
                    }
                    x1.testedFiles = s1;                 

                    XmlDocument xmlDoc = new XmlDocument();

                    XmlNode testRequestElem = xmlDoc.CreateElement("testRequest");
                    xmlDoc.AppendChild(testRequestElem);

                    XmlNode authorNode = xmlDoc.CreateElement("author");
                    testRequestElem.AppendChild(authorNode);
                    authorNode.InnerText = x1.author;

                    XmlNode rootNode = xmlDoc.CreateElement("test");
                    testRequestElem.AppendChild(rootNode);

                    foreach (string t in s1)
                    {
                        XmlNode userNode = xmlDoc.CreateElement("testDriver");
                        int temp = t.IndexOf(".");
                        string temp1 = t.Substring(0, temp);
                        temp1 = temp1 + ".dll";
                        userNode.InnerText = temp1;
                        rootNode.AppendChild(userNode);
                    }

                    xmlDoc.Save("..//..//..//RepoStorage/TestRequest"+c2.body+".xml");

                    //send test request file to test harness
                    string fileName1 = c2.body;
                    CommMessage csndMsg1 = new CommMessage(CommMessage.MessageType.request);
                    csndMsg1.body = fileName1;
                    csndMsg1.command = "show";
                    csndMsg1.author = "Jim Fawcett";
                    csndMsg1.type = CommMessage.MessageType.testRequest;
                    csndMsg1.to = "http://localhost:8089/IPluggableComm";
                    csndMsg1.from = "http://localhost:" + port + "/IPluggableComm";
                    csndMsg1.show();
                    c1.postMessage(csndMsg1);

                    //send ready message to mother builder once child gets free
                    readyMsgToMother(port);
                }
            }
        }
    }
}
