////////////////////////////////////////////////////////////////////////
// MainWindow.xaml.cs - Client prototype GUI for Pluggable Repository //
// ver 1.0                                                            //
//                                                                    //
// Author: Sayali Naval, snaval@syr.edu                               //
// Source: Dr. Jim Fowcett                                            //
// Application: CSE681 Project 4-Build Server                         //
// Environment: C# console                                            //
////////////////////////////////////////////////////////////////////////
/*  
 *  Purpose:
 *    Prototype for a client for the Pluggable Repository.This application
 *    doesn't connect to the repository - it has no Communication facility.
 *    It simply explores the kinds of user interface elements needed for that.
 *
 *  Required Files:
 *    MainWindow.xaml, MainWindow.xaml.cs - view into repository and checkin/checkout
 *    Window1.xaml, Window1.xaml.cs       - Code and MetaData view for individual packages
 *
 *
 *  Maintenance History:
 *    ver 1.0 : 15 Jun 2017
 *    - first release
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MessagePassingComm;
using RepoStorage;
using XmlParser;
using static XmlParser.CreateXml;

namespace PluggableRepoClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<Window1> popups = new List<Window1>();
        List<TestElement> testElements = new List<TestElement>();
        Random r = new Random();
        static Comm c1 = new Comm("http://localhost", 8078);
        static public List<int> processID;

        //add test files from repository storage
        void initializeFilesListBox()
        {

            List<string> files = getListOfFiles("../../../RepoStorage/");
            foreach (string file in files)
            {
                if (file.Contains(".cs") && !file.Contains("Driver") && !file.Contains(".csproj"))
                    filesListBox.Items.Add(file);
            }

            statusLabel.Text = "Action: Double Click on any file to view content";
        }
        //add test driver files from repository storage
        void initializetestDriverListBox()
        {
            List<string> files = getListOfFiles("../../../RepoStorage");
            foreach (string file in files)
            {
                if (file.Contains("Driver"))
                    testDriverListBox.Items.Add(file);
            }
        }

        public List<string> getListOfFiles(string path)
        {
            string[] listOfFiles = Directory.GetFiles(path);
            int index = 0;
            foreach (string file in listOfFiles)
            {
                listOfFiles[index] = System.IO.Path.GetFileName(listOfFiles[index]);
                index++;
            }

            return listOfFiles.ToList<string>();
        }
        //double click on files to view content
        private void testDriverListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Window1 codePopup = new Window1();
            codePopup.Show();
            popups.Add(codePopup);

            codePopup.codeView.Blocks.Clear();
            string fileName = (string)testDriverListBox.SelectedItem;

            codePopup.codeLabel.Text = "Source code: " + fileName;

            showFile(fileName, codePopup);
            return;
        }
        //view contents of file
        private void showFile(string fileName, Window1 popUp)
        {
            string path = System.IO.Path.Combine("../../../RepoStorage", fileName);
            Paragraph paragraph = new Paragraph();
            string fileText = "";
            try
            {
                fileText = System.IO.File.ReadAllText(path);
            }
            finally
            {
                paragraph.Inlines.Add(new Run(fileText));
            }

            // add code text to code view
            popUp.codeView.Blocks.Clear();
            popUp.codeView.Blocks.Add(paragraph);
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            initializeFilesListBox();
            initializetestDriverListBox();
            initializeLogListBox();
        }
        //click on Add Test button to add selected test files
        private void AddTestButton_Click(object sender, RoutedEventArgs e)
        {
            if ((string)testDriverListBox.SelectedItem == null || filesListBox.SelectedItems.Count == 0)
                statusLabel.Text = "Status: Please select Test Driver and Code to test";
            else
            {

                TestElement te1 = new TestElement();


                te1.testDriver = (string)testDriverListBox.SelectedItem;

                foreach (string selectedItem in filesListBox.SelectedItems)
                    te1.addCode(selectedItem);

                testElements.Add(te1);

                statusLabel.Text = "Status: Added Test";
                TestRequest.IsEnabled = true;
                testDriverListBox.UnselectAll();
                filesListBox.UnselectAll();
            }
        }
        //creates mother builder process
        static bool createProcess(string numberOfProcesses)
        {
            Process p = new Process();
            //Process p1 = new Process();

            //        string fileName1 = "..\\..\\..\\RepoStorage\\bin\\debug\\RepoStorage.exe";
            string fileName2 = "..\\..\\..\\MotherBuilder\\bin\\debug\\MotherBuilder.exe";
            //       string absFileSpec1 = System.IO.Path.GetFullPath(fileName1);
            string absFileSpec2 = System.IO.Path.GetFullPath(fileName2);

            //        Console.Write("\n  attempting to start {0}", absFileSpec1);
            Console.Write("\n  attempting to start {0}", absFileSpec2);
            string commandline = numberOfProcesses;

            try
            {
                Process.Start("..\\..\\..\\MotherBuilder\\bin\\debug\\MotherBuilder.exe", commandline);
                //      Process.Start("..\\..\\..\\RepoStorage\\bin\\debug\\RepoStorage.exe", commandline);               
            }

            catch (Exception ex)
            {
                Console.Write("\n  {0}", ex.Message);
                return false;
            }
            return true;
        }

        //accept the number of child builders to be created and click on Start Mother Builder to start
        private void StartBuilderButton_Click(object sender, RoutedEventArgs e)
        {
            if (noOfProcessesTextBox.Text == "")
                statusLabel.Text = "Status: Please enter value less than 6 and greater than 0 in text box";
            else
            {
                if (Convert.ToInt32(noOfProcessesTextBox.Text) > 6 || Convert.ToInt32(noOfProcessesTextBox.Text) <= 0)
                    statusLabel.Text = "Status: Please enter value between 1 to 6";
                else
                {
                    if (createProcess(noOfProcessesTextBox.Text))
                    {
                        Console.Write(" - succeeded");
                    }
                    else
                    {
                        Console.Write(" - failed");
                    }
                }
            }
        }

        //Create build request for selected test files when you click on Create Build Request Button
        private void CreateBuildRequestButton_Click(object sender, RoutedEventArgs e)
        {
            CreateXml x1 = new CreateXml();
            x1.author = "Sayali Naval";
            x1.testFiles.AddRange(testElements);
            string fileName = r.Next(1, 100).ToString();

            string savePath = "../../../RepoStorage/BuildRequest" + fileName + ".xml";

            x1.makeRequest();
            x1.saveXml(savePath);

            BuildRequestListBox.Items.Clear();
            initializeTestListBox();
            testElements.Clear();
            //send the build requests to mother builder
            CommMessage csndMsg1 = new CommMessage(CommMessage.MessageType.request);
            csndMsg1.body = fileName;
            csndMsg1.command = "show";
            csndMsg1.author = "Jim Fawcett";
            csndMsg1.type = CommMessage.MessageType.buildRequest;
            csndMsg1.to = "http://localhost:8080/IPluggableComm";
            csndMsg1.from = "http://localhost:8078/IPluggableComm";
            csndMsg1.show();
            c1.postMessage(csndMsg1);

            statusLabel.Text = "Status: Build Request Created and sent to Mother Builder: " + fileName;
        }
        //View build request file names
        void initializeTestListBox()
        {

            String pattern = "*.xml";
            List<string> files = filesWithPattern("../../../RepoStorage", pattern);
            foreach (string file in files)
            {
                BuildRequestListBox.Items.Add(file);
            }

            statusLabel.Text = "Action: Show file content by double clicking on file";
        }

        public List<string> filesWithPattern(String path, String pattern)
        {

            string[] files = Directory.GetFiles(path, pattern);

            for (int i = 0; i < files.Length; ++i)
            {
                files[i] = System.IO.Path.GetFileName(files[i]);  // now a FileName
            }
            return files.ToList<string>();
        }

        private void filesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        //open new window to show file content
        private void showCodeButton_Click(object sender, RoutedEventArgs e)
        {
            Window1 codePopup = new Window1();
            codePopup.Show();
            popups.Add(codePopup);
        }

        private void filesListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Window1 codePopup = new Window1();
            codePopup.Show();
            popups.Add(codePopup);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (var popup in popups)
                popup.Close();
        }

        //double click on build request files to view content
        private void BuildRequestListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Window1 codePopup = new Window1();
            codePopup.Show();
            popups.Add(codePopup);

            codePopup.codeView.Blocks.Clear();
            string fileName = (string)BuildRequestListBox.SelectedItem;

            codePopup.codeLabel.Text = "Source code: " + fileName;

            showFile(fileName, codePopup);
            return;

        }

        //send stop message to Mother Builder when you click on Stop Mother Builder Button
        private void StopBuilderButton_Click(object sender, RoutedEventArgs e)
        {
            CommMessage csndMsg1 = new CommMessage(CommMessage.MessageType.request);
            csndMsg1.command = "show";
            csndMsg1.author = "Jim Fawcett";
            csndMsg1.type = CommMessage.MessageType.close;
            csndMsg1.to = "http://localhost:8080/IPluggableComm";
            csndMsg1.from = "http://localhost:8078/IPluggableComm";
            csndMsg1.show();
            c1.postMessage(csndMsg1);
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            logListBox.Items.Clear();
            String pattern = "*Log*";
            List<string> files = filesWithPattern("../../../RepoStorage/", pattern);
            foreach (string file in files)
            {
                logListBox.Items.Add(file);
            }
        }

        //function for viewing the build logs
        private void logListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Window1 codePopup = new Window1();
            codePopup.Show();
            popups.Add(codePopup);

            codePopup.codeView.Blocks.Clear();
            string fileName = (string)logListBox.SelectedItem;

            codePopup.codeLabel.Text = "Source code: " + fileName;

            showFile(fileName, codePopup);
            return;
        }

        //initialize list box for build logs
        private void initializeLogListBox()
        {
            logListBox.Items.Clear();
            String pattern = "*Log*";
            List<string> files = filesWithPattern("../../../RepoStorage/", pattern);
            foreach (string file in files)
            {
                logListBox.Items.Add(file);
            }
            statusLabel.Text = "Hit refresh to view the log file";
        }
    }
}
