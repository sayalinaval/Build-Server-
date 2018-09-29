///////////////////////////////////////////////////////////////////////
// RepoStorage.CS - Send Build Requests to Mother Builder.           //
// Send test files to child builder when requested                   //
// ver 1.0                                                           //
//                                                                   //
// Author: Sayali Naval, snaval@syr.edu                              //
// Source: Dr. Jim Fowcett                                           //
// Application: CSE681 Project 4-Build Server                        //
// Environment: C# console                                           //
///////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePassingComm;
using System.Xml.Linq;
using System.IO;
using XmlParser;

namespace RepoStorage
{
    class RepoStorage
    {
        //sends some build requests to mother builder
        public void repoToMother()
        {
            string fileName1 = "1";
            string fileName2 = "2";
            string fileName3 = "3";
            Comm r1 = new Comm("http://localhost", 8079);
            CommMessage csndMsg1 = new CommMessage(CommMessage.MessageType.request);
            csndMsg1.body = fileName1;
            csndMsg1.command = "show";
            csndMsg1.author = "Jim Fawcett";
            csndMsg1.type = CommMessage.MessageType.buildRequest;
            csndMsg1.to = "http://localhost:8080/IPluggableComm";
            csndMsg1.from = "http://localhost:8079/IPluggableComm";
            csndMsg1.show();
            r1.postMessage(csndMsg1);

            CommMessage csndMsg2 = new CommMessage(CommMessage.MessageType.request);
            csndMsg2.body = fileName2;
            csndMsg2.command = "show";
            csndMsg2.author = "Jim Fawcett";
            csndMsg2.type = CommMessage.MessageType.buildRequest;
            csndMsg2.to = "http://localhost:8080/IPluggableComm";
            csndMsg2.from = "http://localhost:8079/IPluggableComm";
            csndMsg2.show();
            r1.postMessage(csndMsg2);

            CommMessage csndMsg3 = new CommMessage(CommMessage.MessageType.request);
            csndMsg3.body = fileName3;
            csndMsg3.command = "show";
            csndMsg3.author = "Jim Fawcett";
            csndMsg3.type = CommMessage.MessageType.buildRequest;
            csndMsg3.to = "http://localhost:8080/IPluggableComm";
            csndMsg3.from = "http://localhost:8079/IPluggableComm";
            csndMsg3.show();
            r1.postMessage(csndMsg3);

            //receive requests from child builder and send those files to that child builder
            while (true)
            {
                CommMessage c = r1.rcvr.getMessage();
                 
                if (c.type == CommMessage.MessageType.fileTransfer)
                {
                    string fromPath = "../../../RepoStorage";
                    string toPath = c.body;
                    string f = c.file;
                    Console.WriteLine("\ntransferring file \"{0}\"", f);
                    r1.postFile(f, fromPath, toPath);
                    c.show();
                }
                //send dll files to test harness
                if (c.type == CommMessage.MessageType.request)
                {
                    string toPath1 = "../../../TestStorage";
                    System.IO.Directory.CreateDirectory(Path.GetFullPath(toPath1)).ToString();
                    string fromPath1 = "../../../RepoStorage";
                    string f1 = c.file;
                    Console.WriteLine("\ntransferring file \"{0}\"", f1);
                    r1.postFile(f1, fromPath1, toPath1);
                    c.show();
                }
            }
        }

            static void Main(string[] args)
            {
            Console.Title = "RepoStorage";
            RepoStorage s = new RepoStorage();
            s.repoToMother();
            }
    }
}
