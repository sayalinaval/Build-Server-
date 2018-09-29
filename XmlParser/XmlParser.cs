///////////////////////////////////////////////////////////////////////
// Xml Parser.CS - Demonstrate operations on XML                     //
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
using System.Xml.Linq;

namespace XmlParser
{
    public class CreateXml
    {
        public string author { get; set; } = "";
        public List<TestElement> testFiles { get; set; } = new List<TestElement>();
        public List<string> testedFiles { get; set; } = new List<string>();
        public List<string> testDriver { get; set; } = new List<string>();
        public XDocument doc { get; set; } = new XDocument();
        //-----parse each element of xml and get test files for each test driver in a string
        public void makeRequest()
        {
            XElement testRequestElem = new XElement("buildRequest");
            doc.Add(testRequestElem);

            XElement authorElem = new XElement("author");
            authorElem.Add(author);
            testRequestElem.Add(authorElem);

            foreach (TestElement file in testFiles)
            {
                XElement testElem = new XElement("test");
                testRequestElem.Add(testElem);

                string testDriver = file.testDriver;

                XElement testDriverElem = new XElement("testDriver");
                testDriverElem.Add(testDriver);
                testElem.Add(testDriverElem);

                foreach (string code in file.testCodes)
                {
                    XElement testedElem = new XElement("tested");
                    testedElem.Add(code);
                    testElem.Add(testedElem);
                }
            }
        }
        //-----load xml and print status
        public bool loadXml(string path)
        {
            try
            {
                doc = XDocument.Load(path);
                return true;
            }
            catch (Exception ex)
            {
                Console.Write("\n--{0}--\n", ex.Message);
                return false;
            }
        }
        //-----Save Xml
        public bool saveXml(string path)
        {
            try
            {
                doc.Save(path);
                return true;
            }
            catch (Exception ex)
            {
                Console.Write("\n--{0}--\n", ex.Message);
                return false;
            }
        }

        public List<Tuple<string, string>> parser()
        {
            IEnumerable<XElement> e = doc.Root.Descendants("test");
            List<Tuple<string, string>> listTuple = new List<Tuple<string, string>>();
            foreach (XElement x in e)
            {
                string testDriver = x.Descendants("testDriver").First().Value;
                IEnumerable<XElement> tested = x.Descendants("tested");
                string sourceCode = " ";
                foreach (XElement elem in tested)
                {
                    sourceCode += elem.Value + " ";
                }

                Tuple<string, string> t = new Tuple<string, string>(testDriver, sourceCode);

                listTuple.Add(t);

            }
            return listTuple;
        }
        //---get test driver files
        public List<string> getTestDriver()
        {
            List<string> testDriverList = new List<string>();
            IEnumerable<XElement> e = doc.Root.Descendants("test");
            foreach (XElement x in e)
            {
                string testDriver = x.Descendants("testDriver").First().Value;
                testDriver = testDriver.Substring(0, testDriver.IndexOf('.'));
                testDriver += ".cs";
                testDriverList.Add(testDriver);
            }
            return testDriverList;
        }
        //---get test files
        public class TestElement
        {
            public string testDriver { get; set; }
            public List<string> testCodes { get; set; } = new List<string>();

            public TestElement() { }
            public void addDriver(string name)
            {
                testDriver = name;
            }
            public void addCode(string name)
            {
                testCodes.Add(name);
            }
        }
        //----parse xml to get test file names
        public string parse(string propertyName)
        {

            string parseStr = doc.Descendants(propertyName).First().Value;
            if (parseStr.Length > 0)
            {
                switch (propertyName)
                {
                    case "author":
                        author = parseStr;
                        break;

                    default:
                        break;
                }
                return parseStr;
            }
            return "";
        }
        //----parse xml to get list of test file names
        public List<string> parseList(string propertyName)
        {
            List<string> values = new List<string>();

            IEnumerable<XElement> parseElems = doc.Descendants(propertyName);

            if (parseElems.Count() > 0)
            {
                switch (propertyName)
                {
                    case "tested":
                        foreach (XElement elem in parseElems)
                        {
                            values.Add(elem.Value);
                        }
                        testedFiles = values;
                        break;

                    case "testDriver":
                        foreach (XElement elem in parseElems)
                        {
                            values.Add(elem.Value);
                        }
                        testDriver = values;
                        break;

                    default:
                        break;
                }
            }
            return values;
        }
        //create xml files
        public static void makeXml()
        {
            CreateXml x = new CreateXml();
            CreateXml tr = new CreateXml();
            tr.loadXml("../../RepoStorage/BuildRequest.xml");
            x.author = tr.parse("author");
            x.testDriver.Add(tr.parse("testDriver"));
            x.testedFiles.Add(tr.parse("testedFiles"));
            string s = tr.parse("testDriver").Split('.')[0] + ".dll";
            x.testedFiles.Add(s);
            x.makeRequest();
            x.saveXml("../../BuildStorage/TestRequest.xml");
        }
        static void Main(string[] args)
        {
            Console.Title = "XmlParser";
        }
    }
}
