using FolderSelect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using System.Windows.Forms;

namespace Mass_Renamer
{
    public partial class MassRename : Form
    {
        private ushort RTC = 0;//run time counter, used for error checking (needs re-intergration)
        //0=new run
        //1=succesfull run
        //2=A minor error occured or the process was aborted by the user
        //3=critical error caught in a try statement
        //4=multiple ciritical errors caught in at least 2 try statements, possible cascading error
        //5=A minor error occured that may have caused a cascading error
        //6=Process finished succesfully but reported an error after finishing (usually harmless)

        public MassRename()
        {
            InitializeComponent();

            BtnMassRename.Enabled = false;
            
        }

        string path;//stores the working directory
        int processorCount = Environment.ProcessorCount -1;//get amount of processors - 1 as one thread is already being used by the UI & Main processes
        bool DW = false;//stores weather or not the warnings have been disabled
        bool active = false;//stores if the program is currently mass renaming or not
        int split = 0;//stores the amount of files delegated to each thread
        string[] threadProgress = new string[0];//used for debugging to check if all of the threads have renamed correctly
        bool msg = false;//suppresses duplicate error messages from appearing for the user
        string selectedPath = "";//stores what the selected path currently is based on the user input direction
        //string[] refAll = new string[0];//stores the entire list of all the delegated files (unused, might use for undo)
        string[] renamedFrom = new string[0];//stores the original name of a file. Used for handling rename conflicts and undo
        string[] renamedTo = new string[0];//stores the name the file was renamed to.
        int tmpc = 1;//stores which TMPRN file we're at

        int[] ThreadClaimed = new int[0];//stores which thread (the ThreadClaimed[#] object) claimed what file (#) [currently only used for debugging, will be used for better delegtion]
        string[][] delegated = new string[0][];//stores the delegated files (currently unused, will be used for better delegation)

        //lockers (e.g. for use with Monitor.Enter())
        private static object locker = new object();//a generic object to 'bottle neck' the threads for handling cross delegation problems
        private static object mainLocker = new object();//a generic object to 'bottle neck' the threads for waiting for access to files (unused)
        private static object claimLocker = new object();//a generic object to 'bottle neck' the threads for claiming their delegations
        bool[] reSync = new bool[0];//stores weather the threads have resynced yet or not (once all are true the cycleWait() ends)

        FolderSelectDialog fsd;//object prepared for main route of btnMassRename_Click
        ErrorLogger errorLogger = new ErrorLogger();//the primary (main thread) error logger, allowing access to log() and queueLog()
        ActionBlock<FolderSelectDialog> workerBlock = null;//the object which creates the threads
        Graphics gr;//only used for the main/UI threadProgressBar's text, this is just here so that each thread doesn't create their own object
        FileStream[] file = new FileStream[0];//stores the FileInfo of the claimed file for the thread for the file stream the thread will open using the FileInfo

        private void MassRename_Load(object sender, EventArgs e)
        {
            //check if the users system is a single core system
            if (processorCount == 0)
            {
                MessageBox.Show("This program does not support single core computers. Please use version 1.0.2.5 or earlier.");
                Close();

            }

            //get the path to work in
            if (Environment.CurrentDirectory.Contains(@"\Debug"))
            {                                                                                               //T if in debug mode
                path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly                           //|-- get folder location of the launched executable and its resources folder
                    .GetEntryAssembly().Location).TrimEnd(@"\bin\Debug".ToCharArray()) + @"\Resources";     //|
                                                                                                            //|
                if (!File.Exists(path + @"\log.txt"))                                                       //|
                {                                                                                           //|-T check if the logging file doesnt already exists
                    File.CreateText(path + @"\log.txt").Close();                                            //|-|--- Create the logging file
                                                                                                            //| |
                } else {                                                                                    //|-\c) otherwise if it already exists
                    errorLogger.QueueLog("", "", 4, true);                                                  //|-|--- Append an empty space to seperate the previous log from this ones
                                                                                                            //| |
                }                                                                                           //|-|e)
                                                                                                            //|
                if (path.Contains("SOL"))                                                                   //|
                {                                                                                           //|-T Check if the current working directory contains "SOL" (check if launched from SOL)
                    errorLogger.QueueLog("INFO: Running in debug mode in SOL.");                            //|-|--- log that the program is running in SOL in debug mode
                                                                                                            //| |
                } else {                                                                                    //|-\c) otherwise if not launched from SOL (launched .exe directly)
                    errorLogger.QueueLog("INFO: Running in debug mode.");                                   //|-|--- log that the program is running in direct debug mode
                                                                                                            //| |
                }                                                                                           //|-|e)
            } else {                                                                                        //\c) otherwise if in realease mode
                path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly                           //|-- get folder location of the launched executable and its resources folder
                    .GetEntryAssembly().Location) + @"\Resources";                                          //|
                                                                                                            //|
                if (!File.Exists(path + @"\log.txt"))                                                       //|
                {                                                                                           //|-T check if the logging file doesnt already exists
                    File.CreateText(path + @"\log.txt").Close();                                            //|-|--- Create the logging file
                                                                                                            //| |
                } else {                                                                                    //|-\c) otherwise if it already exists
                    errorLogger.QueueLog("", "", 4, true);                                                  //|-|--- Append an empty space to seperate the previous log from this ones
                                                                                                            //| |
                }                                                                                           //|-|e)
                                                                                                            //|
                if (path.Contains("SOL"))                                                                   //|
                {                                                                                           //|-T Check if the current working directory contains "SOL" (check if launched from SOL)
                    errorLogger.QueueLog("INFO: Running in release mode in SOL.");                          //|-|--- Log that the program is running in SOL in release mode 
                                                                                                            //| |
                } else {                                                                                    //|-\c) otherwise if not launched from SOL (launched .exe directly)
                    errorLogger.QueueLog("INFO: Running in release mode.");                                 //|-|--- log that the program is running in direct release mode
                                                                                                            //| |
                }                                                                                           //| |e)
            }                                                                                               //|e)

            //set resources
            this.Icon = new Icon(path + @"\SOLICO.ico");

            //failsafe if ref files still exist due to a unexpected fail before ref files could be reset
            foreach (var refFile in Directory.GetFiles(path, "ref*"))
            {
                File.Delete(refFile);

            }

            //FolderSelectDialog.cs and Refelector.cs is from http://www.lyquidity.com/devblog/?p=136
            //Initialize the folder selection dialog. Allows for the use of vita/win7 folder selector.
            fsd = new FolderSelectDialog                                                                                     
            {                                               //T initialise a new 'folder selection dialog box' object (controller)
                Title = "Select at leat one folder",        //|--- Set the dialog box's title
                InitialDirectory = @"C:\"                   //|--- changes the initial directory opened when the dialog box is opened to the C drive
            };                                              //|e)

        }

        //handles prombting the user for input and getting the files to target
        private void BtnMassRename_Click(object sender, EventArgs e)
        {
            errorLogger.QueueLog("INFO: Started a new mass rename instance.");

            //reseting variables
            split = 0;                                                                                                                                  //-reset the amount of files delegated per thread
            ThreadProgressBar.Value = 0;                                                                                                                //-reset the thread progress bar's progress
            tmpc = 1;                                                                                                                                   //-reset the TMPRN file # to default
            reSync = new bool[processorCount];                                                                                                          //-reset the thread re syncer locker
            Array.Resize(ref renamedFrom, 0);                                                                                                           //-clear the cross delegation array helper "renamedFrom"
            Array.Resize(ref renamedTo, 0);                                                                                                             //-clear the cross delegation array helper "renamedTo"
                                                                                                                                                        //
            //setting globals                                                                                                                           //
            active = true;                                                                                                                              //-set that the program is now mass renaming
            BtnMassRename.Enabled = false;                                                                                                              //-disable the mass rename button
            PreferencesTSMI.Enabled = false;                                                                                                            //-disable the File>preferences tool strip menu item
            ThreadProgressBar.Refresh();                                                                                                                //-update the thread progress bar's UI to display correctly
                                                                                                                                                        //
            //setting locals                                                                                                                            //
            string text = fileRenameTxtBx.Text;                                                                                                         //-get the user inputted text
            int length = 0;                                                                                                                             //-store the amount of files in the folder selected as 'length'
                                                                                                                                                        //
            //setting temporary locals                                                                                                                  //
            bool valid = true;                                                                                                                          //-use a bool variable to check if any naming conventions are incorrect and stopping the renaming if so
            string[] invalidN = new string[0];                                                                                                          //-store each invalid character found in the fileRenameTxtBxs' text via an array
            string[] invalidA = new string[0];                                                                                                          //-store each invalid character found in the textAfterNumTxtBx' text via an array
            string[] invalidE = new string[0];                                                                                                          //-store each invalid character found in the extentionTxtBx' text via an array
                                                                                                                                                        //
            foreach (char invalidC in Path.GetInvalidFileNameChars())                                                                                   //
            {                                                                                                                                           //T loop through each invalid file name character 
                if (text.Contains(invalidC.ToString()))                                                                                                 //|
                {                                                                                                                                       //|-T if the fileRenameTxtBxs' text contains the current character to check
                    valid = false;                                                                                                                      //|-|--- set valid to false
                    Array.Resize(ref invalidN, invalidN.Length + 1);                                                                                    //|-|--- store what the character is which exists in the text box's text
                    invalidN[invalidN.Length - 1] = invalidC.ToString();                                                                                //| |
                                                                                                                                                        //| |
                }                                                                                                                                       //|-|e)
                                                                                                                                                        //|
                if (textAfterNumTxtBx.Text.Contains(invalidC.ToString()))                                                                               //|
                {                                                                                                                                       //|-T if the textAfterNumTxtBx' text contains the current character to check
                    valid = false;                                                                                                                      //|-|--- set valid to false
                    Array.Resize(ref invalidA, invalidA.Length + 1);                                                                                    //|-|--- store what the character is which exists in the text box's text
                    invalidA[invalidA.Length - 1] = invalidC.ToString();                                                                                //| |
                                                                                                                                                        //| |
                }                                                                                                                                       //|-|e)
                                                                                                                                                        //|
                if (("." + extentionTxtBx.Text).Contains(invalidC.ToString()))                                                                          //|
                {                                                                                                                                       //|-T if the extentionTxtBx' text contains the current character to check
                    valid = false;                                                                                                                      //|-|--- set valid to false
                    Array.Resize(ref invalidE, invalidE.Length + 1);                                                                                    //|-|--- store what the character is which exists in the text box's text
                    invalidE[invalidE.Length - 1] = invalidC.ToString();                                                                                //| |
                                                                                                                                                        //| |
                }                                                                                                                                       //|-|e)
                                                                                                                                                        //|
            }                                                                                                                                           //|L)
                                                                                                                                                        //
            if (textAfterNumTxtBx.Text.Contains("."))                                                                                                   //
            {                                                                                                                                           //T if textAfterNumTxtBxs' text has a "." in it
                valid = false;                                                                                                                          //|-- set valid to false
                Array.Resize(ref invalidA, invalidA.Length + 1);                                                                                        //|-- store what the character is which exists in the text box's text
                invalidA[invalidA.Length - 1] = ".";                                                                                                    //|
                                                                                                                                                        //|
            }                                                                                                                                           //|e)
                                                                                                                                                        //
            if (extentionTxtBx.Text.Contains("."))                                                                                                      //
            {                                                                                                                                           //T if textAfterNumTxtBxs' text has a "." in it
                valid = false;                                                                                                                          //|-- set valid to false
                Array.Resize(ref invalidE, invalidE.Length + 1);                                                                                        //|-- store what the character is which exists in the text box's text
                invalidE[invalidE.Length - 1] = ".";                                                                                                    //|
                                                                                                                                                        //|
            }                                                                                                                                           //|e)
                                                                                                                                                        //
            if (!valid)                                                                                                                                 //
            {                                                                                                                                           //T If there is at least one invalid character in the text boxes
                string errmsg = "Please amend these issues to start the renaming process:\n\n";                                                         //|-- store the first part of the message to show the user
                                                                                                                                                        //|
                if (invalidN.Length > 0)                                                                                                                //|
                {                                                                                                                                       //|-T if the number of invalid characters in fileRenameTxtBx's text is at least 1
                    errmsg += "The name to rename the files to contains at least one illegal character: \"";                                            //|-|--- add to the message that the fileRenameTxtBx's text contains ar least one illeagl character
                                                                                                                                                        //| |
                    foreach (string invalidC in invalidN)                                                                                               //| |
                    {                                                                                                                                   //|-|--T loop through each invalid character found
                        if (invalidN.Length > 1 && invalidC != invalidN[0] && invalidC != invalidN[invalidN.Length-1]) {                                //|-|--|---T if there is at least 2 invalid characters and we're not on the first invalid character nor the last
                            errmsg += "\", \"";                                                                                                         //|-|--|---|----- add a ", " (including quotation marks) to the message
                        }                                                                                                                               //|-|--|---|e)
                                                                                                                                                        //| |  |
                        if (invalidN.Length > 1 && invalidC == invalidN[invalidN.Length-1]) {                                                           //|-|--|---T otherwise? if we're on the second last invald character
                            errmsg += "\" and \"";                                                                                                      //|-|--|---|----- add a " and " (including quotation marks) to the message
                                                                                                                                                        //| |  |   |
                        }                                                                                                                               //|-|--|---|e)
                                                                                                                                                        //| |  |
                        errmsg += invalidC;                                                                                                             //|-|--|---- add the invalid character to the message
                    }                                                                                                                                   //|-|--|L)
                                                                                                                                                        //| |
                    errmsg += "\".";                                                                                                                    //|-|--- add ". (including the quotation mark) to the messege
                    if (invalidA.Length > 0 || invalidE.Length > 0)                                                                                     //| |
                    {                                                                                                                                   //|-|--T if there are more invalid characters in either of the other text boxes
                        errmsg += "\n";                                                                                                                 //|-|--|---- add a new line character to the message
                    }                                                                                                                                   //|-|--|e)
                                                                                                                                                        //| |
                }                                                                                                                                       //|-|e)
                                                                                                                                                        //|
                if (invalidA.Length > 0)                                                                                                                //|
                {                                                                                                                                       //|-T if the number of invalid characters in textAfterNumTxtBx's text is at least 1
                    errmsg += "The text to include after the counter contains at least one illegal character: \"";                                      //|-|--- add to the message that the textAfterNumTxtBx's text contains ar least one illeagl character
                                                                                                                                                        //| |
                    foreach (string invalidC in invalidA)                                                                                               //| |
                    {                                                                                                                                   //|-|--T loop through each invalid character found
                        if (invalidA.Length > 1 && invalidC != invalidA[0] && invalidC != invalidA[invalidA.Length - 1]) {                              //|-|--|---T if there is at least 2 invalid characters and we're not on the first invalid character nor the last
                            errmsg += "\", \"";                                                                                                         //|-|--|---|----- add a ", " (including quotation marks) to the message
                        }                                                                                                                               //|-|--|---|e)
                                                                                                                                                        //| |  |
                        if (invalidA.Length > 1 && invalidC == invalidA[invalidA.Length - 1]) {                                                         //|-|--|---T otherwise? if we're on the second last invald character
                            errmsg += "\" and \"";                                                                                                      //|-|--|---|----- add a " and " (including quotation marks) to the message
                                                                                                                                                        //| |  |   |
                        }                                                                                                                               //|-|--|---|e)
                                                                                                                                                        //| |  |
                        errmsg += invalidC;                                                                                                             //|-|--|---- add the invalid character to the message
                    }                                                                                                                                   //|-|--|L)
                                                                                                                                                        //| |
                    errmsg += "\".";                                                                                                                    //|-|--- add ". (including the quotation mark) to the messege
                    if (invalidE.Length > 0)                                                                                                            //| |
                    {                                                                                                                                   //|-|--T if there are more invalid characters in either of the other text boxes
                        errmsg += "\n";                                                                                                                 //|-|--|---- add a new line character to the message
                    }                                                                                                                                   //|-|--|e)
                                                                                                                                                        //| |
                }                                                                                                                                       //|-|e)
                                                                                                                                                        //|
                if (invalidE.Length > 0)                                                                                                                //|
                {                                                                                                                                       //|-T if the number of invalid characters in textAfterNumTxtBx's text is at least 1
                    errmsg += "The extention to change to contains at least one illegal character: \"";                                                 //|-|--- add to the message that the textAfterNumTxtBx's text contains ar least one illeagl character
                                                                                                                                                        //| |
                    foreach (string invalidC in invalidE)                                                                                               //| |
                    {                                                                                                                                   //|-|--T loop through each invalid character found
                        if (invalidE.Length > 1 && invalidC != invalidE[0] && invalidC != invalidE[invalidE.Length - 1]) {                              //|-|--|---T if there is at least 2 invalid characters and we're not on the first invalid character nor the last
                            errmsg += "\", \"";                                                                                                         //|-|--|---|----- add a ", " (including quotation marks) to the message
                        }                                                                                                                               //|-|--|---|e)
                                                                                                                                                        //| |  |
                        if (invalidE.Length > 1 && invalidC == invalidE[invalidE.Length - 1]) {                                                         //|-|--|---T otherwise? if we're on the second last invald character
                            errmsg += "\" and \"";                                                                                                      //|-|--|---|----- add a " and " (including quotation marks) to the message
                                                                                                                                                        //| |  |   |
                        }                                                                                                                               //|-|--|---|e)
                                                                                                                                                        //| |  |
                        errmsg += invalidC;                                                                                                             //|-|--|---- add the invalid character to the message
                    }                                                                                                                                   //|-|--|L)
                                                                                                                                                        //| |
                    errmsg += "\".";                                                                                                                    //|-|--- add ". (including the quotation mark) to the messege
                }                                                                                                                                       //|-|e)
                                                                                                                                                        //|
                MessageBox.Show(errmsg, "Error: Invalid character", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);                                  //|-- warn the user of the invalid characters present in the text boxes
                BtnMassRename.Enabled = true;                                                                                                           //|-- re-enable the mass rename button
                PreferencesTSMI.Enabled = true;                                                                                                         //|-- re-enable the File>Preferences TSMI
                                                                                                                                                        //|
            } else {                                                                                                                                    //\c) otherwise if there are no invalid characters in any of the text feilds
                if (textAfterNumTxtBx.Text == "Text after number (optional)") { textAfterNumTxtBx.Text = ""; }                                          //|-Te) if the 'text after number' feild has not been filled; set it to nothing (so that it doesn't affect the process)
                                                                                                                                                        //|
                if (fsd.ShowDialog(IntPtr.Zero))                                                                                                        //|-- show the file selector dialog box (order of presedence has that message boxes show first)
                {                                                                                                                                       //|-T if the dialog box has not returned an error or has closed (0)
                    DialogResult result = DialogResult.Cancel;                                                                                          //|-|--- set the default response from the dialog boxes to "Cancel"
                                                                                                                                                        //| |
                    if (!DW)                                                                                                                            //| |
                    {                                                                                                                                   //|-|--T if the warnings are NOT disabled
                        result = MessageBox.Show("Are you sure? ALL FILES IN THIS FOLDER WILL BE RENAMED, " +                                           //| |  |
                            "THIS CAN'T BE UNDON - NOT EVEN WITH CTRL+Z IN THE EXPLORER!", "Filepath: " + fsd.FileName, MessageBoxButtons.OKCancel,     //| |  |
                            MessageBoxIcon.Warning);                                                                                                    //|-|--|---- warn the user of the danger (message box)
                    } else { result = DialogResult.OK; }                                                                                                //|-|--\c)e) otherwise set the result to "OK"                           
                                                                                                                                                        //| |
                    string[] AFa = new string[0];                                                                                                       //|-|--- prepare an array version AF for the randomize selection type
                                                                                                                                                        //| |
                    if (result == DialogResult.OK)                                                                                                      //| |
                    {                                                                                                                                   //|-|--T if the result is "OK"
                        DirectoryInfo di = new DirectoryInfo(fsd.FileName);                                                                             //|-|--|---- get the selected directory (folder)
                        length = di.GetFiles().Length;                                                                                                  //|-|--|---- get how many files are in the folder
                        IEnumerable<string> AF = Splitter.CustomSort(di.EnumerateFiles().Select(f => f.Name));                                          //|-|--|---- get all files in the folder, sort by name (alphanumerically, not alphabraically)
                                                                                                                                                        //| |  |
                        if (alphabraicSTBtn.Checked) {                                                                                                  //|-|--|---T if the user has defined to sort aphabraically
                            AF = di.EnumerateFiles().Select(f => f.Name);                                                                               //|-|--|---|----- sort
                                                                                                                                                        //| |  |   |
                        } else if (regexSTBtn.Checked) {                                                                                                //|-|--|---\c) if the user has defined to sort by a regular expression
                                                                                                                                                        //| |  |   |WIP
                                                                                                                                                        //| |  |   |
                        } else if (randomSTBtn.Checked) {                                                                                               //|-|--|---\c) if the user has defined to sort randomly
                            Random ran = new Random();                                                                                                  //|-|--|---|----- generate a random...thing
                            AFa = di.EnumerateFiles().Select(f => f.Name).OrderBy(f => ran.Next()).ToArray();                                           //|-|--|---|----- randomise the order
                                                                                                                                                        //| |  |   |
                        }                                                                                                                               //|-|--|---|e)
                                                                                                                                                        //| |  |
                        string firstFileName = "";                                                                                                      //|-|--|---- prepare to get first file's name
                                                                                                                                                        //| |  |
                        if (!randomSTBtn.Checked)                                                                                                       //| |  |
                        {                                                                                                                               //|-|--|---T if the user has not selected to randomise the selection
                            firstFileName = AF.FirstOrDefault();                                                                                        //|-|--|---|----- get the first file's name (doesn't include directory)
                            result = MessageBox.Show("Is " + firstFileName +                                                                            //| |  |   |
                                " the first file in the folder and/or you DONT need to select a custom set of files?", "Filepath: "                     //| |  |   |
                                + fsd.FileName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);                                                      //|-|--|---|----- verify the target file is the first file of the target file order (message box)
                        } else {                                                                                                                        //|-|--|---\c)
                            result = MessageBox.Show(" Do you want to NOT select a custom set of files?", "Filepath: " + fsd.FileName,                  //| |  |   |
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question);                                                                      //|-|--|---|----- verify if the user wants to select multiple files, but not every file of the folder (message box)
                        }                                                                                                                               //|-|--|---|e)
                                                                                                                                                        //| |  |
                        if (result == DialogResult.Yes)                                                                                                 //| |  |
                        {                                                                                                                               //|-|--|---T if the dialog box returns "Yes" of target file check
                            bool testValid = true;                                                                                                      //|-|--|---|----- store a variable (testValid) defining if this test case occurs:
                            foreach (string testFile in di.EnumerateFiles().Select(f => f.Name))                                                        //| |  |   |
                            {                                                                                                                           //|-|--|---|----T loop for every file selected
                                if (testFile.StartsWith("TMPRN"))                                                                                       //| |  |   |    |
                                {                                                                                                                       //|-|--|---|----|-----T if the current file to test starts with "TMPRN"
                                    testValid = false;                                                                                                  //|-|--|---|----|-----|------- set test case as failed (testValid as false)
                                }                                                                                                                       //|-|--|---|----|-----|e) (implied: otherwise keep testValid as true)
                            }                                                                                                                           //|-|--|---|----|L)
                                                                                                                                                        //| |  |   |
                            if (testValid)                                                                                                              //| |  |   |
                            {                                                                                                                           //|-|--|---|----T if test case succeeded (testValid is true)
                                                                                                                                                        //| |  |   |    |
                                if (!DW)                                                                                                                //| |  |   |    |
                                {                                                                                                                       //|-|--|---|----|-----T if the warnings are NOT disabled
                                    result = MessageBox.Show("Are you ABSOLUTELY sure?", "Last chance bukko!", MessageBoxButtons.YesNo,                 //| |  |   |    |     |
                                    MessageBoxIcon.Question);                                                                                           //|-|--|---|----|-----|------- Preforme a final chech with the user (message box)
                                } else { result = DialogResult.Yes; }                                                                                   //|-|--|---|----|-----\c)e) otherwise set the result to "Yes"
                                                                                                                                                        //| |  |   |    |
                                if (result == DialogResult.Yes)                                                                                         //| |  |   |    |
                                {                                                                                                                       //|-|--|---|----|-----T if the dialog box returns "Yes" again of final chech
                                                                                                                                                        //| |  |   |    |     |
                                    if (!randomSTBtn.Checked)                                                                                           //| |  |   |    |     |
                                    {                                                                                                                   //|-|--|---|----|-----|------T if not randomizing
                                                                                                                                                        //| |  |   |    |     |      |
                                        split = int.Parse(Math.Ceiling((double)length / processorCount).ToString());                                    //|-|--|---|----|-----|------|-------- get the amount of files each thread will operate on (rounded up) [variable name: split]
                                        progress progressBar = new progress();                                                                          //|-|--|---|----|-----|------|-------- create a new other-form progress bar
                                        Form isOpen = Application.OpenForms["progress"];                                                                //|-|--|---|----|-----|------|-------- get if another window of this is still open (incase an error occured and this was never closed )
                                        if (isOpen != null) { isOpen.Close(); }                                                                         //|-|--|---|----|-----|------|-------Te) if it is then close it
                                                                                                                                                        //| |  |   |    |     |      |
                                        progressBar.Show();                                                                                             //|-|--|---|----|-----|------|-------- show the progress bar
                                        progressBar.progressBar1.Maximum = processorCount * split;                                                      //|-|--|---|----|-----|------|-------- set the maxiumum to the aproximate amount of files
                                        progressBar.Text = "Getting files to rename...";                                                                //|-|--|---|----|-----|------|-------- change the title of the progress bar to "Getting files to rename..."
                                                                                                                                                        //| |  |   |    |     |      |
                                        int fc = 0;                                                                                                     //|-|--|---|----|-----|------|-------- prepare to cound the amount of file names written to the ref files
                                        StreamWriter[] stream = new StreamWriter[processorCount];                                                       //|-|--|---|----|-----|------|-------- prepare to write to files
                                                                                                                                                        //| |  |   |    |     |      |
                                        threadProgress = new string[processorCount * split];                                                            //|-|--|---|----|-----|------|-------- reset threadProgress[]
                                        for (uint pi = 0; pi < processorCount * split; pi++)                                                            //| |  |   |    |     |      |
                                        {                                                                                                               //| |  |   |    |     |      |
                                            threadProgress[pi] = "";                                                                                    //| |  |   |    |     |      |
                                        }                                                                                                               //| |  |   |    |     |      |
                                                                                                                                                        //| |  |   |    |     |      |
                                        //create ref files                                                                                              //| |  |   |    |     |      |
                                        for (int i = 1; i <= processorCount; i++)                                                                       //| |  |   |    |     |      |
                                        {                                                                                                               //|-|--|---|----|-----|------|-------T loop for the amount of processors avalible to use starting from i=1
                                            stream[i - 1] = File.CreateText(path + @"\ref" + i + ".txt");                                               //|-|--|---|----|-----|------|-------|--------- create a reference file for the thread to reffer to when getting which files to rename
                                            stream[i - 1].AutoFlush = true;                                                                             //|-|--|---|----|-----|------|-------|--------- write buffer to file after each WriteLine() function call
                                                                                                                                                        //| |  |   |    |     |      |       |
                                            foreach (string file in AF.Skip(split * (i - 1)))                                                           //| |  |   |    |     |      |       |
                                            {                                                                                                           //|-|--|---|----|-----|------|-------|--------T (loop) get each file, skipping the files that area already delegated
                                                stream[i - 1].WriteLine(file);                                                                          //|-|--|---|----|-----|------|-------|--------|---------- write the reference file to the file 
                                                fc++;                                                                                                   //|-|--|---|----|-----|------|-------|--------|---------- count the amount of files written in
                                                progressBar.Text = file + " => " + "ref" + i + @"\" + processorCount;                                   //|-|--|---|----|-----|------|-------|--------|---------- change the progress bars title to reflect which file is being delegated to what file and how much progress has been made
                                                                                                                                                        //| |  |   |    |     |      |       |        |
                                                if (fc >= split)                                                                                        //| |  |   |    |     |      |       |        |
                                                {                                                                                                       //|-|--|---|----|-----|------|-------|--------|---------T if all files delegated
                                                    break;                                                                                              //|-|--|---|----|-----|------|-------|--------|---------|----------- stop delegating files
                                                }                                                                                                       //|-|--|---|----|-----|------|-------|--------|---------|e)
                                                                                                                                                        //| |  |   |    |     |      |       |        |
                                            }                                                                                                           //|-|--|---|----|-----|------|-------|--------|L)
                                            stream[i - 1].Close();                                                                                      //|-|--|---|----|-----|------|-------|--------- close the streams
                                            progressBar.progressBar1.Value++;                                                                           //|-|--|---|----|-----|------|-------|--------- itterate the progress bar
                                            fc = 0;                                                                                                     //|-|--|---|----|-----|------|-------|--------- reset the file count
                                        }                                                                                                               //|-|--|---|----|-----|------|-------|e)
                                                                                                                                                        //| |  |   |    |     |      |
                                        isOpen = Application.OpenForms["progress"];                                                                     //|-|--|---|----|-----|------|-------- get if the progress window is still open
                                        if (isOpen != null) { isOpen.Close(); }                                                                         //|-|--|---|----|-----|------|-------Te) if it is then close it
                                                                                                                                                        //| |  |   |    |     |      |
                                        ThreadProgressBar.Visible = true;                                                                               //|-|--|---|----|-----|------|-------- make the thread progress bar visible
                                        selectedPath = fsd.FileName;                                                                                    //|-|--|---|----|-----|------|-------- set the currently selected path to this routs selected path
                                        ThreadProgressBar.Maximum = AF.Count();                                                                         //|-|--|---|----|-----|------|-------- set the thread progress bar's maximum to the amount of files there are
                                        backgroundThread.RunWorkerAsync();                                                                              //|-|--|---|----|-----|------|-------- start multi-threading (go to backgroundWorker1_DoWork())
                                                                                                                                                        //| |  |   |    |     |      |
                                    } else {                                                                                                            //|-|--|---|----|-----|------\c)otherwise if randomised
                                        progress progressBar = new progress();                                                                          //|-|--|---|----|-----|------|-------- create a new other-form progress bar
                                        Form isOpen = Application.OpenForms["progress"];                                                                //|-|--|---|----|-----|------|-------- get if another window of this is still open (incase an error occured and this was never closed )
                                        if (isOpen != null) { isOpen.Close(); }                                                                         //|-|--|---|----|-----|------|-------Te) if it is then close it
                                                                                                                                                        //| |  |   |    |     |      |
                                        progressBar.Show();                                                                                             //|-|--|---|----|-----|------|-------- show the progress bar
                                        progressBar.progressBar1.Maximum = processorCount;                                                              //|-|--|---|----|-----|------|-------- set the maxiumum to the aproximate amount of files
                                        progressBar.Text = "Getting files to rename...";                                                                //|-|--|---|----|-----|------|-------- change the title of the progress bar to "Getting files to rename..."
                                                                                                                                                        //| |  |   |    |     |      |
                                        split = int.Parse(Math.Ceiling((double)length / processorCount).ToString());                                    //|-|--|---|----|-----|------|-------- get the amount of files each thread will operate on (rounded up) [variable name: split]
                                        int fc = 0;                                                                                                     //|-|--|---|----|-----|------|-------- prepare to cound the amount of file names written to the ref files
                                        StreamWriter[] stream = new StreamWriter[processorCount];                                                       //|-|--|---|----|-----|------|-------- prepare to write to files
                                                                                                                                                        //| |  |   |    |     |      |
                                        threadProgress = new string[processorCount * split];                                                            //|-|--|---|----|-----|------|-------- reset threadProgress[]
                                                                                                                                                        //| |  |   |    |     |      |
                                        for (uint pi = 0; pi < processorCount * split; pi++)                                                            //| |  |   |    |     |      |
                                        {                                                                                                               //| |  |   |    |     |      |
                                            threadProgress[pi] = "";                                                                                    //| |  |   |    |     |      |
                                        }                                                                                                               //| |  |   |    |     |      |
                                                                                                                                                        //| |  |   |    |     |      |
                                        for (int i = 1; i <= processorCount; i++)                                                                       //| |  |   |    |     |      |
                                        {                                                                                                               //|-|--|---|----|-----|------|-------T loop for the amount of processors avalible to use starting from i=1
                                            stream[i - 1] = File.CreateText(path + @"\ref" + i + ".txt");                                               //|-|--|---|----|-----|------|-------|--------- create a reference file for the thread to reffer to when getting which files to rename
                                            stream[i - 1].AutoFlush = true;                                                                             //|-|--|---|----|-----|------|-------|--------- write buffer to file after each WriteLine() function call
                                                                                                                                                        //| |  |   |    |     |      |       |
                                            foreach (string file in AFa.Skip(split * (i - 1)))                                                          //| |  |   |    |     |      |       |     
                                            {                                                                                                           //|-|--|---|----|-----|------|-------|--------T (loop) get each file, skipping the files that area already delegated
                                                stream[i - 1].WriteLine(file);                                                                          //|-|--|---|----|-----|------|-------|--------|---------- write the reference file to the file 
                                                fc++;                                                                                                   //|-|--|---|----|-----|------|-------|--------|---------- count the amount of files written in
                                                progressBar.Text = file + " => " + "ref" + i + @"\" + processorCount;                                   //|-|--|---|----|-----|------|-------|--------|---------- change the progress bars title to reflect which file is being delegated to what file and how much progress has been made
                                                                                                                                                        //| |  |   |    |     |      |       |        |
                                                if (fc >= split)                                                                                        //| |  |   |    |     |      |       |        |
                                                {                                                                                                       //|-|--|---|----|-----|------|-------|--------|---------T if all files delegated
                                                    break;                                                                                              //|-|--|---|----|-----|------|-------|--------|---------|----------- stop delegating files
                                                }                                                                                                       //|-|--|---|----|-----|------|-------|--------|---------|e)
                                                                                                                                                        //| |  |   |    |     |      |       |        |
                                            }                                                                                                           //|-|--|---|----|-----|------|-------|--------|L)
                                            stream[i - 1].Close();                                                                                      //|-|--|---|----|-----|------|-------|--------- close the streams
                                            progressBar.progressBar1.Value = i;                                                                         //|-|--|---|----|-----|------|-------|--------- itterate the progress bar
                                            fc = 0;                                                                                                     //|-|--|---|----|-----|------|-------|--------- reset the file count
                                        }                                                                                                               //|-|--|---|----|-----|------|-------|e)
                                                                                                                                                        //| |  |   |    |     |      |
                                        isOpen = Application.OpenForms["progress"];                                                                     //|-|--|---|----|-----|------|-------- get if the progress window is still open
                                        if (isOpen != null) { isOpen.Close(); }                                                                         //|-|--|---|----|-----|------|-------Te) if it is then close it
                                                                                                                                                        //| |  |   |    |     |      |
                                        ThreadProgressBar.Visible = true;                                                                               //|-|--|---|----|-----|------|-------- make the thread progress bar visible
                                        selectedPath = fsd.FileName;                                                                                    //|-|--|---|----|-----|------|-------- set the currently selected path to this routs selected path
                                        ThreadProgressBar.Maximum = AFa.Count();                                                                        //|-|--|---|----|-----|------|-------- set the thread progress bar's maximum to the amount of files there are
                                        backgroundThread.RunWorkerAsync();                                                                              //|-|--|---|----|-----|------|-------- start multi-threading (go to backgroundWorker1_DoWork())
                                                                                                                                                        //| |  |   |    |     |      |
                                    }                                                                                                                   //|-|--|---|----|-----|------|e)
                                    if (RTC == 0) { RTC = 1; }                                                                                          //|-|--|---|----|-----|------- Report succesfull run
                                                                                                                                                        //| |  |   |    |     |
                                }                                                                                                                       //|-|--|---|----|-----|e)if the dialog box returns "No" of final chech; END.
                                                                                                                                                        //| |  |   |    |
                            } else {                                                                                                                    //|-|--|---|----\c) otherwise if test case fails (testValid is false)
                                MessageBox.Show("One or more files start with \"TMPRN\" which is a reserved name," +                                    //| |  |   |    |
                                    " please rename these files to anything else.");                                                                    //|-|--|---|----|------ warn the user that a file is named with a resevered namespace
                                BtnMassRename.Enabled = true;                                                                                           //|-|--|---|----|------ re-enable the mass rename button
                                PreferencesTSMI.Enabled = true;                                                                                         //|-|--|---|----|------ re-enable the File>Preferences TSMI
                                active = false;                                                                                                         //|-|--|---|----|------ set that the program is no longer mass renaming
                                textAfterNumTxtBx.Text = "Text after number (optional)";                                                                //|-|--|---|----|------ reset the text in the textAfterNumTxtBx
                            }                                                                                                                           //|-|--|---|----|e)
                        } else {                                                                                                                        //|-|--|---\c) if the dialog box returns "No" of target file check
                            if (!DW)                                                                                                                    //| |  |   |
                            {                                                                                                                           //|-|--|---|----T if the warnings are NOT disabled
                                MessageBox.Show("To sort the files selected, right click on the dialog and use the 'sort by' option",                   //| |  |   |    |
                                        "Reminder!");                                                                                                   //|-|--|---|----|------ Remind the user how to sort in this circumstance
                            }                                                                                                                           //|-|--|---|----|e)
                                                                                                                                                        //| |  |   |
                            openFileDialog1.InitialDirectory = fsd.FileName;                                                                            //|-|--|---|----- open the same directory
                            DialogResult dlgr = openFileDialog1.ShowDialog();                                                                           //|-|--|---|----- show the file selecter dialog
                            openFileDialog1.Title = "Select all files to rename";                                                                       //|-|--|---|----- change the title of the dialog to signify multi-select is active
                                                                                                                                                        //| |  |   |
                            if (dlgr == DialogResult.OK)                                                                                                //| |  |   |
                            {                                                                                                                           //|-|--|---|----T if the return from the dialog is "OK" of the file selecter
                                firstFileName = null;                                                                                                   //|-|--|---|----|------ reset the first gotten file in case the files have changed or a new folder is selected
                                                                                                                                                        //| |  |   |    |
                                while (firstFileName == null)                                                                                           //| |  |   |    |
                                {                                                                                                                       //|-|--|---|----|-----T (loop) while the first file is not gotten
                                    firstFileName = openFileDialog1.FileName;                                                                           //|-|--|---|----|-----|------- wait untill the dialog box is closed
                                                                                                                                                        //| |  |   |    |     |       note: unlike the above route, this returns the full directory
                                    if (firstFileName == null)                                                                                          //| |  |   |    |     |
                                    {                                                                                                                   //|-|--|---|----|-----|------T if the first file is still not avalible
                                        DialogResult bgr = MessageBox.Show("could not get file, try again...", "Could not find the selected file!",     //| |  |   |    |     |      |
                                            MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);                                                    //|-|--|---|----|-----|------|-------- tell the user the file might not exist (message box)
                                                                                                                                                        //| |  |   |    |     |      |
                                        if (bgr == DialogResult.Cancel)                                                                                 //| |  |   |    |     |      |
                                        {                                                                                                               //|-|--|---|----|-----|------|-------T if the user decides it's not going to work (presses cancel)
                                            BtnMassRename.Enabled = true;                                                                               //|-|--|---|----|-----|------|-------|--------- re-enable the mass rename button
                                            PreferencesTSMI.Enabled = true;                                                                             //|-|--|---|----|-----|------|-------|--------- re-enable the File>Preferences TSMI
                                            Close();                                                                                                    //|-|--|---|----|-----|------|-------|--------- brake out of the loop
                                                                                                                                                        //| |  |   |    |     |      |       |
                                        }                                                                                                               //|-|--|---|----|-----|------|-------|e)
                                                                                                                                                        //| |  |   |    |     |      |
                                    }                                                                                                                   //|-|--|---|----|-----|------|e) force loop as file is still invalid
                                                                                                                                                        //| |  |   |    |     |    
                                }                                                                                                                       //|-|--|---|----|-----|W)file found
                                                                                                                                                        //| |  |   |    |
                                bool testValid = true;                                                                                                  //|-|--|---|----|------ store a variable (testValid) defining if this test case occurs:
                                foreach (string testFile in openFileDialog1.SafeFileNames)                                                              //| |  |   |    |
                                {                                                                                                                       //|-|--|---|----|-----T loop for every file selected
                                    if (testFile.StartsWith("TMPRN"))                                                                                   //| |  |   |    |     |
                                    {                                                                                                                   //|-|--|---|----|-----|------T if the current file to test starts with "TMPRN"
                                        testValid = false;                                                                                              //|-|--|---|----|-----|------|-------- set test case as failed (testValid as false)
                                    }                                                                                                                   //|-|--|---|----|-----|------|e) (implied: otherwise keep testValid as true)
                                }                                                                                                                       //|-|--|---|----|-----|L)
                                                                                                                                                        //| |  |   |    |
                                if (testValid)                                                                                                          //| |  |   |    |
                                {                                                                                                                       //|-|--|---|----|-----T if test case succeeded (testValid is true)
                                    if (!DW)                                                                                                            //| |  |   |    |     |
                                    {                                                                                                                   //|-|--|---|----|-----|-----T if the warnings are NOT disabled
                                        result = MessageBox.Show("Are you ABSOLUTELY sure?", "Last chance bukko!", MessageBoxButtons.YesNo,             //| |  |   |    |     |     |
                                        MessageBoxIcon.Question);                                                                                       //|-|--|---|----|-----|-----|------- Preforme a final chech with the user (message box)
                                    }                                                                                                                   //| |  |   |    |     |     |
                                    else { result = DialogResult.Yes; }                                                                                 //|-|--|---|----|-----|-----\c)e)otherwise set the result to "Yes"
                                                                                                                                                        //| |  |   |    |     |
                                    if (result == DialogResult.Yes)                                                                                     //| |  |   |    |     |
                                    {                                                                                                                   //|-|--|---|----|-----|-----Tif the dialog box returns "Yes" of the final check
                                        length = openFileDialog1.FileNames.Length;                                                                      //|-|--|---|----|-----|-----|------- reset 'length' to the amount of files selected
                                                                                                                                                        //| |  |   |    |     |     |
                                        if (!randomSTBtn.Checked)                                                                                       //| |  |   |    |     |     |
                                        {                                                                                                               //|-|--|---|----|-----|-----|------T if not randomizing
                                            progress progressBar = new progress();                                                                      //|-|--|---|----|-----|-----|------|-------- create a new other-form progress bar
                                            Form isOpen = Application.OpenForms["progress"];                                                            //|-|--|---|----|-----|-----|------|-------- get if another window of this is still open (incase an error occured and this was never closed )
                                            if (isOpen != null) { isOpen.Close(); }                                                                     //|-|--|---|----|-----|-----|------|-------Te) if it is then close it
                                                                                                                                                        //| |  |   |    |     |     |      |
                                            progressBar.Show();                                                                                         //|-|--|---|----|-----|-----|------|-------- show the progress bar
                                            progressBar.progressBar1.Maximum = processorCount;                                                          //|-|--|---|----|-----|-----|------|-------- set the maxiumum to the aproximate amount of files
                                            progressBar.Text = "Getting files to rename...";                                                            //|-|--|---|----|-----|-----|------|-------- change the title of the progress bar to "Getting files to rename..."
                                                                                                                                                        //| |  |   |    |     |     |      |
                                            split = int.Parse(Math.Ceiling((double)length / processorCount).ToString());                                //|-|--|---|----|-----|-----|------|-------- get the amount of files each thread will operate on (rounded up) [variable name: split]
                                            int fc = 0;                                                                                                 //|-|--|---|----|-----|-----|------|-------- prepare to cound the amount of file names written to the ref files
                                            StreamWriter[] stream = new StreamWriter[processorCount];                                                   //|-|--|---|----|-----|-----|------|-------- prepare to write to files
                                                                                                                                                        //| |  |   |    |     |     |      |
                                            threadProgress = new string[processorCount * split];                                                        //|-|--|---|----|-----|-----|------|-------- reset threadProgress[]
                                                                                                                                                        //| |  |   |    |     |     |      |
                                            for (uint pi = 0; pi < processorCount; pi++)                                                                //| |  |   |    |     |     |      |
                                            {                                                                                                           //| |  |   |    |     |     |      |
                                                threadProgress[pi] = "";                                                                                //| |  |   |    |     |     |      |
                                            }                                                                                                           //| |  |   |    |     |     |      |
                                                                                                                                                        //| |  |   |    |     |     |      |
                                            for (int i = 1; i <= processorCount; i++)                                                                   //| |  |   |    |     |     |      |
                                            {                                                                                                           //|-|--|---|----|-----|-----|------|-------T loop for the amount of processors avalible to use starting from i=1
                                                stream[i - 1] = File.CreateText(path + @"\ref" + i + ".txt");                                           //|-|--|---|----|-----|-----|------|-------|--------- create a reference file for the thread to reffer to when getting which files to rename
                                                stream[i - 1].AutoFlush = true;                                                                         //|-|--|---|----|-----|-----|------|-------|--------- write buffer to file after each WriteLine() function call
                                                                                                                                                        //| |  |   |    |     |     |      |       |
                                                foreach (string file in openFileDialog1.SafeFileNames.Skip(split * (i - 1)))                            //| |  |   |    |     |     |      |       |
                                                {                                                                                                       //|-|--|---|----|-----|-----|------|-------|--------T (loop) get each file, skipping the files that area already delegated
                                                    stream[i - 1].WriteLine(file);                                                                      //|-|--|---|----|-----|-----|------|-------|--------|---------- write the reference file to the file 
                                                    fc++;                                                                                               //|-|--|---|----|-----|-----|------|-------|--------|---------- count the amount of files written in
                                                    progressBar.Text = file + " => " + "ref" + i + @"\" + processorCount;                               //|-|--|---|----|-----|-----|------|-------|--------|---------- change the progress bars title to reflect which file is being delegated to what file and how much progress has been made
                                                                                                                                                        //| |  |   |    |     |     |      |       |        |
                                                    if (fc >= split)                                                                                    //| |  |   |    |     |     |      |       |        |
                                                    {                                                                                                   //|-|--|---|----|-----|-----|------|-------|--------|---------T if all files delegated
                                                        break;                                                                                          //|-|--|---|----|-----|-----|------|-------|--------|---------|----------- stop delegating files
                                                    }                                                                                                   //|-|--|---|----|-----|-----|------|-------|--------|---------|e)
                                                                                                                                                        //| |  |   |    |     |     |      |       |        |
                                                }                                                                                                       //|-|--|---|----|-----|-----|------|-------|--------|L)
                                                stream[i - 1].Close();                                                                                  //|-|--|---|----|-----|-----|------|-------|--------- close the streams
                                                progressBar.progressBar1.Value = i;                                                                     //|-|--|---|----|-----|-----|------|-------|--------- itterate the progress bar
                                                fc = 0;                                                                                                 //|-|--|---|----|-----|-----|------|-------|--------- reset the file count
                                            }                                                                                                           //|-|--|---|----|-----|-----|------|-------|e)
                                                                                                                                                        //| |  |   |    |     |     |      |
                                            isOpen = Application.OpenForms["progress"];                                                                 //|-|--|---|----|-----|-----|------|-------- get if the progress window is still open
                                            if (isOpen != null) { isOpen.Close(); }                                                                     //|-|--|---|----|-----|-----|------|-------Te) if it is then close it
                                                                                                                                                        //| |  |   |    |     |     |      |
                                            ThreadProgressBar.Visible = true;                                                                           //|-|--|---|----|-----|-----|------|-------- make the thread progress bar visible
                                            selectedPath = firstFileName.Replace(openFileDialog1.SafeFileName, "");                                     //|-|--|---|----|-----|-----|------|-------- set the currently selected path to this routes selected file path (- the file)
                                            selectedPath = selectedPath.Remove(selectedPath.Length - 1);                                                //|-|--|---|----|-----|-----|------|-------- set the currently selected path to this routes selected path (- the \ at the end)
                                            ThreadProgressBar.Maximum = openFileDialog1.SafeFileNames.Count();                                          //|-|--|---|----|-----|-----|------|-------- set the thread progress bar's maximum to the amount of files there are
                                            backgroundThread.RunWorkerAsync();                                                                          //|-|--|---|----|-----|-----|------|-------- start multi-threading (go to backgroundWorker1_DoWork())
                                        } else {                                                                                                        //|-|--|---|----|-----|-----|------\c) otherwise if randomised
                                            Random ran = new Random();                                                                                  //|-|--|---|----|-----|-----|------|-------- generate a random...thing
                                            AFa = openFileDialog1.SafeFileNames.Select(f => f).OrderBy(f => ran.Next()).ToArray();                      //|-|--|---|----|-----|-----|------|-------- randomise the order of the selected files
                                                                                                                                                        //| |  |   |    |     |     |      |
                                            progress progressBar = new progress();                                                                      //|-|--|---|----|-----|-----|------|-------- create a new other-form progress bar
                                            Form isOpen = Application.OpenForms["progress"];                                                            //|-|--|---|----|-----|-----|------|-------- get if another window of this is still open (incase an error occured and this was never closed )
                                            if (isOpen != null) { isOpen.Close(); }                                                                     //|-|--|---|----|-----|-----|------|-------Te) if it is then close it
                                                                                                                                                        //| |  |   |    |     |     |      |
                                            progressBar.Show();                                                                                         //|-|--|---|----|-----|-----|------|-------- show the progress bar
                                            progressBar.progressBar1.Maximum = processorCount;                                                          //|-|--|---|----|-----|-----|------|-------- set the maxiumum to the aproximate amount of files
                                            progressBar.Text = "Getting files to rename...";                                                            //|-|--|---|----|-----|-----|------|-------- change the title of the progress bar to "Getting files to rename..."
                                                                                                                                                        //| |  |   |    |     |     |      |
                                            split = int.Parse(Math.Ceiling((double)length / processorCount).ToString());                                //|-|--|---|----|-----|-----|------|-------- get the amount of files each thread will operate on (rounded up) [variable name: split]
                                            int fc = 0;                                                                                                 //|-|--|---|----|-----|-----|------|-------- prepare to cound the amount of file names written to the ref files
                                            StreamWriter[] stream = new StreamWriter[processorCount];                                                   //|-|--|---|----|-----|-----|------|-------- prepare to write to files
                                                                                                                                                        //| |  |   |    |     |     |      |
                                            threadProgress = new string[processorCount * split];                                                        //|-|--|---|----|-----|-----|------|-------- reset threadProgress[]
                                                                                                                                                        //| |  |   |    |     |     |      |
                                            for (uint pi = 0; pi < processorCount; pi++)                                                                //| |  |   |    |     |     |      |
                                            {                                                                                                           //| |  |   |    |     |     |      |
                                                threadProgress[pi] = "";                                                                                //| |  |   |    |     |     |      |
                                            }                                                                                                           //| |  |   |    |     |     |      |
                                                                                                                                                        //| |  |   |    |     |     |      |
                                            for (int i = 1; i <= processorCount; i++)                                                                   //| |  |   |    |     |     |      |
                                            {                                                                                                           //|-|--|---|----|-----|-----|------|-------T loop for the amount of processors avalible to use starting from i=1
                                                stream[i - 1] = File.CreateText(path + @"\ref" + i + ".txt");                                           //|-|--|---|----|-----|-----|------|-------|--------- create a reference file for the thread to reffer to when getting which files to rename
                                                stream[i - 1].AutoFlush = true;                                                                         //|-|--|---|----|-----|-----|------|-------|--------- write buffer to file after each WriteLine() function call
                                                                                                                                                        //| |  |   |    |     |     |      |       |
                                                foreach (string file in AFa.Skip(split * (i - 1)))                                                      //| |  |   |    |     |     |      |       |     
                                                {                                                                                                       //|-|--|---|----|-----|-----|------|-------|--------T (loop) get each file, skipping the files that area already delegated
                                                    stream[i - 1].WriteLine(file);                                                                      //|-|--|---|----|-----|-----|------|-------|--------|---------- write the reference file to the file 
                                                    fc++;                                                                                               //|-|--|---|----|-----|-----|------|-------|--------|---------- count the amount of files written in
                                                    progressBar.Text = file + " => " + "ref" + i + @"\" + processorCount;                               //|-|--|---|----|-----|-----|------|-------|--------|---------- change the progress bars title to reflect which file is being delegated to what file and how much progress has been made
                                                                                                                                                        //| |  |   |    |     |     |      |       |        |
                                                    if (fc >= split)                                                                                    //| |  |   |    |     |     |      |       |        |
                                                    {                                                                                                   //|-|--|---|----|-----|-----|------|-------|--------|---------T if all files delegated
                                                        break;                                                                                          //|-|--|---|----|-----|-----|------|-------|--------|---------|----------- stop delegating files
                                                    }                                                                                                   //|-|--|---|----|-----|-----|------|-------|--------|---------|e)
                                                                                                                                                        //| |  |   |    |     |     |      |       |        |
                                                }                                                                                                       //|-|--|---|----|-----|-----|------|-------|--------|L)
                                                stream[i - 1].Close();                                                                                  //|-|--|---|----|-----|-----|------|-------|--------- close the streams
                                                progressBar.progressBar1.Value = i;                                                                     //|-|--|---|----|-----|-----|------|-------|--------- itterate the progress bar
                                                fc = 0;                                                                                                 //|-|--|---|----|-----|-----|------|-------|--------- reset the file count
                                            }                                                                                                           //|-|--|---|----|-----|-----|------|-------|e)
                                                                                                                                                        //| |  |   |    |     |     |      |
                                            isOpen = Application.OpenForms["progress"];                                                                 //|-|--|---|----|-----|-----|------|-------- get if the progress window is still open
                                            if (isOpen != null) { isOpen.Close(); }                                                                     //|-|--|---|----|-----|-----|------|-------Te) if it is then close it
                                                                                                                                                        //| |  |   |    |     |     |      |
                                            ThreadProgressBar.Visible = true;                                                                           //|-|--|---|----|-----|-----|------|-------- make the thread progress bar visible
                                            selectedPath = firstFileName.Replace(openFileDialog1.SafeFileName, "");                                     //|-|--|---|----|-----|-----|------|-------- set the currently selected path to this routes selected file path (- the file)
                                            selectedPath = selectedPath.Remove(selectedPath.Length - 1);                                                //|-|--|---|----|-----|-----|------|-------- set the currently selected path to this routes selected path (- the \ at the end)
                                            ThreadProgressBar.Maximum = AFa.Count();                                                                    //|-|--|---|----|-----|-----|------|-------- set the thread progress bar's maximum to the amount of files there are
                                            backgroundThread.RunWorkerAsync();                                                                          //|-|--|---|----|-----|-----|------|-------- start multi-threading (go to backgroundWorker1_DoWork())
                                                                                                                                                        //| |  |   |    |     |     |      |
                                        }                                                                                                               //|-|--|---|----|-----|-----|------|e)
                                        if (RTC == 0) { RTC = 1; }                                                                                      //|-|--|---|----|-----|-----|------- Report succesfull run
                                                                                                                                                        //| |  |   |    |     |     |
                                    }                                                                                                                   //|-|--|---|----|-----|-----|e)
                                } else {                                                                                                                //|-|--|---|----|-----\c) otherwise if test case is false (testValid is false)
                                    MessageBox.Show("One or more files start with \"TMPRN\" which is a reserved name," +                                //| |  |   |    |     |
                                        " please rename these files to anything else.");                                                                //|-|--|---|----|-----|------ warn the user that a file is named with a resevered namespace
                                    BtnMassRename.Enabled = true;                                                                                       //|-|--|---|----|-----|------ re-enable the mass rename button
                                    PreferencesTSMI.Enabled = true;                                                                                     //|-|--|---|----|-----|------ re-enable the File>Preferences TSMI
                                    active = false;                                                                                                     //|-|--|---|----|-----|------ set that the program is no longer mass renaming
                                    textAfterNumTxtBx.Text = "Text after number (optional)";                                                            //|-|--|---|----|-----|------ reset the text in the textAfterNumTxtBx
                                }                                                                                                                       //|-|--|---|----|-----|e)
                            } else {                                                                                                                    //|-|--|---|----\c) otherwise if the user cancled file selection dialog (mutli-select)
                                RTC = 2;                                                                                                                //|-|--|---|----|------ Report a minor error occured
                                BtnMassRename.Enabled = true;                                                                                           //|-|--|---|----|------ re-enable the mass rename button
                                PreferencesTSMI.Enabled = true;                                                                                         //|-|--|---|----|------ re-enable the File>Preferences TSMI
                                MessageBox.Show("An error occured or the process was aborted!");                                                        //|-|--|---|----|------ tell them the process failed
                                                                                                                                                        //|-|--|---|    |
                            }                                                                                                                           //|-|--|---|----|e)
                        }                                                                                                                               //|-|--|---|e)
                    }                                                                                                                                   //|-|--|e)
                } else {                                                                                                                                //|-\c) if the user cancled file selection dialog (single select)
                    RTC = 2;                                                                                                                            //|-|--- Report a minor error occured
                    BtnMassRename.Enabled = true;                                                                                                       //|-|--- re-enable the mass rename button
                    PreferencesTSMI.Enabled = true;                                                                                                     //|-|--- re-enable the File>Preferences TSMI
                    MessageBox.Show("An error occured or the process was aborted!");                                                                    //|-|--- Tell them the process failed
                                                                                                                                                        //|-|
                }                                                                                                                                       //|-|e)
            }                                                                                                                                           //|e)

        }

        //handles creating the new threads to run and runs holds this thread, stopping the UI from being frozen
        private void backgroundThread_DoWork(object sender, DoWorkEventArgs e)
        {
            file = new FileStream[processorCount];                                                          //- reset and prepare to get a new set of ref files
            ThreadClaimed = new int[processorCount];                                                        //- reset and prepare to get which thread claimed what file
            msg = false;                                                                                    //- reset the message blocker
                                                                                                            //
            workerBlock = new ActionBlock<FolderSelectDialog>(                                              //T create a block which tracks all the threads created by this block which preform this action:
            refCount => Thread(fsd, openFileDialog1),                                                       //|-- run thread()
                                                                                                            //|
            new ExecutionDataflowBlockOptions                                                               //|with the options:
            {                                                                                               //|
                MaxDegreeOfParallelism = processorCount                                                     //|-- Specify a maximum degree of parallelism (how many threads will run) being equal to the amount of avalible processors.
            });                                                                                             //|e)
                                                                                                            //
            for (int i = 0; i < processorCount; i++)                                                        //T loop for amount of processors avalible
            {                                                                                               //|
                workerBlock.Post(fsd);                                                                      //|-- create a new thread and feed the thread the info on the folder select dialog (fsd)
            }                                                                                               //|L)
            workerBlock.Complete();                                                                         //run all created threads to completion
            workerBlock.Completion.Wait();                                                                  //wait this thread (kinda) untill all threads created by the block are finished
                                                                                                            //
            for (int i = 1; i <= processorCount; i++)                                                       //
            {                                                                                               //T loop for amount of processors avalible
                if (file[i - 1] != null)                                                                    //|
                {                                                                                           //|-T if the array refference to a ref file is not null
                    file[i - 1].Close();                                                                    //|-|--- close the file
                                                                                                            //| |
                    while (true)                                                                            //| |
                    {                                                                                       //|-|--- loop until break
                        try                                                                                 //|-|
                        {                                                                                   //|-|--T try
                            File.Delete(path + @"\ref" + i + ".txt");                                       //|-|--| delete the ref files
                            File.Delete(path + @"\ref" + i + "Claimed");                                    //|-|--| delete the claim files
                            break;                                                                          //|>|>>> break
                        } catch {                                                                           //|-|--T catch any errors and do:
                            file[i - 1].Close();                                                            //|-|--|---- try to close the file again
                            System.Threading.Thread.Sleep(500);                                             //|-|--|---- wait 0.5 seconds for anything that might be using the file like an AV
                        }                                                                                   //|-|--|e)
                    }                                                                                       //|-|INF)
                                                                                                            //|-|
                } else {                                                                                    //|-|c) otherwise if the array refference to a ref file is
                    File.Delete(path + @"\ref" + i + ".txt");                                               //|-|delete the ref files
                    File.Delete(path + @"\ref" + i + "Claimed");                                            //|-|delete the claim files
                                                                                                            //|-|
                }                                                                                           //|-|e)
            }                                                                                               //|L)
                                                                                                            //
            if (workerBlock.Completion.Exception == null && file.All(item => item != null))                 //
            {                                                                                               //T if the worker finished without any exceptions and all items of file are not null
                errorLogger.QueueLog("", "", 4, true);                                                      //|
                errorLogger.QueueLog("All threads compeleted successfully without any exceptions!");        //|-- log that everything worked fine
                errorLogger.QueueLog("", "", 4, true);                                                      //|
                                                                                                            //|
            }                                                                                               //|e)
                                                                                                            //
            workerBlock.Completion.Dispose();                                                               //- release the threads
            Array.Resize(ref file, 0);                                                                      //- release the file streams
            errorLogger.ReleaseQueue(path + @"\log.txt");                                                   //- release the logs 

        }

        //the set of instructions each thread will run (the renmaing process)
        private void Thread(FolderSelectDialog fsd, OpenFileDialog openFileDialog1)
        {
            //logging
            ErrorLogger threadErrorLogger = new ErrorLogger();
            threadErrorLogger.QueueLog("", "", 4, true);
            threadErrorLogger.QueueLog("INFO: Running thread " + System.Threading.Thread.CurrentThread.ManagedThreadId);

            if (System.Threading.Thread.CurrentThread.IsBackground)
            {                                                                                                                                   
                Monitor.Enter(claimLocker);                                                                                                     //- force each thread which hits this point to either claim the locker or wait until the object is claimable
                                                                                                                                                //note: having this in the current if statement is actually required as the threads that are waiting are looping through this if statement
            }                                                                                                                                   //
                                                                                                                                                //
            int claimed = -1;                                                                                                                   //- set the ref claimed by this thread to "claimed none"
            int breakout = 1000 * processorCount;                                                                                               //- set the breakout spinTime (how many times the bellow must spin before forced breakout, stops infinite loops)
                                                                                                                                                //
            if (processorCount > 1)                                                                                                             //
            {                                                                                                                                   //T if there is at least 2 processors or more
                                                                                                                                                //|
                for (int i = 0; i < processorCount; i++)                                                                                        //|
                {                                                                                                                               //|-T loop for the amount of avalible processors
                    if (file[i] == null)                                                                                                        //| |
                    {                                                                                                                           //|-|--T if the the i'th file (current loop itteration) is null
                        file[i] = new FileInfo(path + @"\ref" + (i + 1) + ".txt").OpenRead();                                                   //|-|--|--- assign it the i'th loop +1 ref
                        claimed = i + 1;                                                                                                        //|-|--|--- store the ref which was claimed by this thread
                        Debug.WriteLine("Thread " + System.Threading.Thread.CurrentThread.ManagedThreadId + " claimed ref"                      //| |  |
                            + claimed + ".");                                                                                                   //| |  |
                        break;                                                                                                                  //|>|>>|>>> break;
                    }                                                                                                                           //|-|--|e)
                                                                                                                                                //| |
                }                                                                                                                               //|-|L)
                                                                                                                                                //|
                Debug.WriteLine("Thread " + System.Threading.Thread.CurrentThread.ManagedThreadId + " claimed ref" + claimed + ".");            //|
                threadErrorLogger.QueueLog("Thread " + System.Threading.Thread.CurrentThread.ManagedThreadId + " claimed ref" + claimed + "."); //|
                ThreadClaimed[claimed - 1] = System.Threading.Thread.CurrentThread.ManagedThreadId;                                             //|- get which thread claimed which ref
                                                                                                                                                //|
                Monitor.Exit(claimLocker);                                                                                                      //|- allow the next thread to flow through the above
                reSync[claimed - 1] = true;                                                                                                     //|- shout that this thread has finished cliaming and is waiting for re-sync
                SpinWait.SpinUntil(delegate () { return reSync.All(item => item.Equals(true)); });                                              //|- wait untill all threads reach this point
                                                                                                                                                //|
                int ci = 0;                                                                                                                     //|- prepare to compare current itteration of the bellow foreach loop
                                                                                                                                                //|
                //for debugging                                                                                                                 //|
                //if (file[0] != null)                                                                                                          //|
                //{                                                                                                                             //|
                //    file[0].Close();                                                                                                          //|
                //    file[0] = null;                                                                                                           //|
                //}                                                                                                                             //|
                                                                                                                                                //|
                if (!msg)                                                                                                                       //|
                {                                                                                                                               //|-T if an error message of this (bellow) same type has not already been sent
                    foreach (FileStream state in file)                                                                                          //| |
                    {                                                                                                                           //|-|--T (loop) get the state of each file (null or not)
                        if (state == null && ci != claimed - 1)                                                                                 //| |  |
                        {                                                                                                                       //|-|--|---T if the state of the file is null and that file is not the one claimed by this thread
                            Monitor.Enter(claimLocker);                                                                                         //|-|--|---|----- lock only one thread to do:
                            Debug.WriteLine("CTITICAL ERROR OCCURED - NULL CLAIMING");                                                          //| |  |   |
                            threadErrorLogger.QueueLog("ERROR: A critical error occured during multi-threading setup - NULL CLAIMING."          //|-|--|---|----- log that an error occured
                                + "ref" + ci + " was not claimed.");                                                                            //| |  |   |
                            threadErrorLogger.ReleaseQueue(path + @"\log.txt");                                                                 //|-|--|---|----- output the log to the logging file
                                                                                                                                                //| |  |   |    
                            if (!msg)                                                                                                           //| |  |   |    
                            {                                                                                                                   //|-|--|---|----T if an error message of this (bellow) same type still has not already been sent
                                MessageBox.Show("A critical error occured during multi-threading setup and will be aborted to preserve data "   //|-|--|---|----|------ tell the user an error occured and what to do
                                    + "integrity, a log has been saved to:\n" + path + @"\log.txt"                                              //| |  |   |    |
                                    + "\nPlease report the log to the issues page on the relevent branch. (" + this.Name + ")"                  //| |  |   |    |
                                    , "Critical error: NULL FILE REF", MessageBoxButtons.OK, MessageBoxIcon.Stop);                              //| |  |   |    |
                                msg = true;                                                                                                     //|-|--|---|----|------ set that this message has been sent and does not need to be sent again
                            }                                                                                                                   //|-|--|---|----|e)
                                                                                                                                                //| |  |   |
                            ci = -1;                                                                                                            //|-|--|---|----- set other checks to that an error has occured
                            Monitor.Exit(claimLocker);                                                                                          //|-|--|---|----- allow next thread through
                            break;                                                                                                              //|>|>>|>>>|>>>>> break;
                                                                                                                                                //| |  |   |    
                        }                                                                                                                       //|-|--|---|e)
                        ci++;                                                                                                                   //|-|--|---- (implied: otherwise) store that its going to the next state
                                                                                                                                                //|-|--|
                    }                                                                                                                           //|-|--|L)
                }                                                                                                                               //|-|e)
                                                                                                                                                //|
                if (ci == -1)                                                                                                                   //|
                {                                                                                                                               //|-T if above has reported an error occured
                    if (file[claimed - 1] == null)                                                                                              //| |
                    {                                                                                                                           //|-|--T and the file claimed by this thread is null
                        Debug.WriteLine("CTITICAL ERROR OCCURED - NULL CLAIMING");                                                              //| |  |
                        threadErrorLogger.QueueLog("ERROR: A critical error occured during multi-threading setup - NULL CLAIMING. Thread "      //|-|--|---- log that an error occured on which thread of which ref
                            + System.Threading.Thread.CurrentThread.ManagedThreadId + " failed to claim ref" + (claimed - 1));                  //| |  |
                        threadErrorLogger.ReleaseQueue(path + @"\log.txt");                                                                     //|-|--|---- output the log to the logging file
                                                                                                                                                //| |  |
                    }                                                                                                                           //|-|--|e)
                } else {                                                                                                                        //|-\c) otherwise if no other thread has failed to claim a file
                                                                                                                                                //| |
                    if (file[claimed - 1] == null)                                                                                              //| |
                    {                                                                                                                           //|-|--T if the file claimed by this thread is null
                        Debug.WriteLine("CTITICAL ERROR OCCURED - NULL CLAIMING");                                                              //| |--|
                        threadErrorLogger.QueueLog("ERROR: A critical error occured during multi-threading setup - NULL CLAIMING. Thread "      //|-|--|---- log that an error occured on which thread of which ref
                            + System.Threading.Thread.CurrentThread.ManagedThreadId + " failed to claim ref" + (claimed - 1));                  //| |--|
                        threadErrorLogger.ReleaseQueue(path + @"\log.txt");                                                                     //|-|--|---- output the log to the logging file
                                                                                                                                                //| |--|
                    } else {                                                                                                                    //|-|--\c) otherwise if this thread has not failed to claim a file
                        int sleepTimer = 100 * claimed;                                                                                         //|-|--|---- set time to sleep for forcing de-sync after re-sync to 100ms between each thread        
                        System.Threading.Thread.Sleep(sleepTimer);                                                                              //|-|--|---- force desync
                        IEnumerable<string> addT = null;                                                                                        //|-|--|---- prepare to store the addition after text as an enumerable
                        string text = fileRenameTxtBx.Text;                                                                                     //|-|--|---- get the user inputted text
                        string ext = "";                                                                                                        //|-|--|---- prepare to store the extention
                        StreamReader stream = File.OpenText(file[claimed - 1].Name);                                                            //|-|--|---- open the ref's stream
                        string[] tmpa = new string[split];                                                                                      //|-|--|---- prepare to store the delegated data in an array (for handling naming conflicts and cross delegation naming conflictions)
                        int ai = 0;                                                                                                             //| |  |
                        int li = 0;                                                                                                             //|-|--|---- prepare to store the location the foreach statement of addT is at 
                                                                                                                                                //| |  |
                        while (!stream.EndOfStream)                                                                                             //| |  |
                        {                                                                                                                       //|-|--|---T (loop) while not at the end of the stream (not at end of file)
                            tmpa[ai] = stream.ReadLine();                                                                                       //|-|--|---|----- read a line of the file into the tmpa array
                            ai++;                                                                                                               //| |  |   |
                                                                                                                                                //| |  |   |
                        }                                                                                                                       //|-|--|---|W)
                                                                                                                                                //| |  |
                        stream.Close();                                                                                                         //|-|--|----- close the ref streamReader
                        file[claimed - 1].Close();                                                                                              //|-|--|----- close the ref fileStream
                                                                                                                                                //| |  |
                        if (alphabeticalTSMI.Checked)                                                                                           //| |  |
                        {                                                                                                                       //|-|--|---T if the user has defined to use alphabetical sorting
                            int splitChar = int.Parse("A") + ((claimed * split) - 1);                                                           //|-|--|---|----- get the current character this thread will start sorting from (get delegated sorting set start)
                            addT = Splitter.GetAdditionType(0, splitChar.ToString(), (claimed - 1) * split);                                    //|-|--|---|----- get the enumerator, called an "addition", of the character each file will have attatched as a sorter where the enumerable will end on the last delegated letter 
                                                                                                                                                //| |  |   |
                        } else if (numericaldTSMI.Checked) {                                                                                    //|-|--|---\c) otherwise if the user has defined to use alphanumerical sorting
                            addT = Splitter.GetAdditionType(1, (split * claimed).ToString(), ((claimed - 1) * split) + 1);                      //|-|--|---|----- get the enumerator, called an "addition", of the number each file will have attatched as a sorter where the enumerable will end on the last delegated number 
                        } else if (customTSMI.Checked) {                                                                                        //|-|--|---\c) otherwise if the user has defined to use a custom sorting method
                                                                                                                                                //|X|XX|XXX| currently unused
                        }                                                                                                                       //|-|--|---|e)
                                                                                                                                                //| |  |
                        bool failedClaim = false;                                                                                               //|-|--|---- prepare to get if a thread failed to claim a ref delegation as "failedClaim"
                        foreach (FileStream state in file)                                                                                      //| |  |
                        {                                                                                                                       //|-|--|---T loop through each ref file as a 'state'
                            if (state == null)                                                                                                  //| |  |   |
                            {                                                                                                                   //|-|--|---|----T if the state is null
                                failedClaim = true;                                                                                             //|-|--|---|----|------ a thread failed to claim a ref, set failedClaim as true
                            }                                                                                                                   //| |  |   |    |e)
                                                                                                                                                //| |  |   |
                        }                                                                                                                       //| |  |   |L)
                                                                                                                                                //| |  |
                        if (!failedClaim)                                                                                                       //| |  |
                        {                                                                                                                       //|-|--|---T (if) double check all files where claimed correctly
                            foreach (string add in addT)                                                                                        //| |  |   |
                            {                                                                                                                   //|-|--|---|----T (loop) generate and get each addition to add to the renaming process of this file related to this foreach possition
                                string firstFileName = tmpa[li];                                                                                //|-|--|---|----|------ get the file related to this foreach possition
                                                                                                                                                //| |  |   |    |
                                if (firstFileName == null)                                                                                      //| |  |   |    |
                                {                                                                                                               //|-|--|---|----|-----T if the current ref file does not exist
                                    Debug.WriteLine("CTITICAL ERROR OCCURED - NULL CLAIMING");                                                  //| |  |   |    |     |
                                    threadErrorLogger.QueueLog("CRIT: A critical error occured during multi-threading inner-runtime" +          //|-|--|---|----|-----|------- queue a log that an error occured
                                        " - NULL CLAIMING. A ref file was not claimed and the process is likely to have not have stopped " +    //| |  |   |    |     |
                                        "before the program started renaming.");                                                                //| |  |   |    |     |
                                                                                                                                                //| |  |   |    |     |
                                    //start error recovery program                                                                              //| |  |   |    |     |
                                                                                                                                                //| |  |   |    |     |
                                    Invoke(new Action(() =>                                                                                     //|-|--|---|----|-----|------T force all threads to run this action
                                                    {                                                                                           //| |  |   |    |     |      |
                                                        errorLogger.AppendQueue(threadErrorLogger.GetQueue());                                  //|-|--|---|----|-----|------|-------- append the above log to the main error logger queue
                                                        errorLogger.ReleaseQueue(path + @"\log.txt", true, true);                               //|-|--|---|----|-----|------|-------- release the queue
                                                        Environment.Exit(Environment.ExitCode);                                                 //|-|--|---|----|-----|------|-------- force the program to exit out on all threads used by this program
                                                    }));                                                                                        //| |  |   |    |     |      |e)
                                }                                                                                                               //|-|--|---|----|-----|e)
                                string firstNameSplit = firstFileName.Split(Convert.ToChar(92)).Last();                                         //|-|--|---|----|------ get the name of the file to rename
                                                                                                                                                //| |  |   |    |
                                if (fileRenameTxtBx.Text.Length != 0 && (fileRenameTxtBx.Text == "What to rename the file to"                   //| |  |   |    |
                                && fileRenameTxtBx.Text == "You may not rename files while sorting is disabled"))                               //| |  |   |    |
                                {                                                                                                               //|-|--|---|----|-----T if the user has defined a name to rename the files to
                                    text = firstFileName;                                                                                       //|-|--|---|----|-----|------- set the text to rename the file to as the users input
                                }                                                                                                               //|-|--|---|----|-----|e)
                                                                                                                                                //| |  |   |    |
                                if (extentionTxtBx.Text.Length > 0 && (extentionTxtBx.Text != "Extention (optional)"                            //| |  |   |    |
                                && extentionTxtBx.Text != "Extention"))                                                                         //| |  |   |    |
                                {                                                                                                               //|-|--|---|----|-----T if the user has defined an extention to change every file to
                                    ext = "." + extentionTxtBx.Text;                                                                            //|-|--|---|----|-----|------- set the extention to the users input
                                } else {                                                                                                        //|-|--|---|----|-----\c) otherwise if the user has not defined an extention
                                    ext = Path.GetExtension(firstFileName);                                                                     //|-|--|---|----|-----|------- set the extnetion as the extention of the current file
                                }                                                                                                               //|-|--|---|----|-----|e)
                                                                                                                                                //| |  |   |    |
                                if (firstNameSplit != text + add + textAfterNumTxtBx.Text + ext)                                                //| |  |   |    |
                                {                                                                                                               //|-|--|---|----|-----T if the item to rename isn't already in the correct possition
                                                                                                                                                //| |  |   |    |     |
                                    if (File.Exists(fsd.FileName + @"\" + text + add + textAfterNumTxtBx.Text + ext))                           //| |  |   |    |     |
                                    {                                                                                                           //|-|--|---|----|-----|------T if a file conflict occures (file already exists)
                                        int tmpi = 0;                                                                                           //|-|--|---|----|-----|------|-------- prepare to get the file's possition in the array (tmpa) as tmpi
                                                                                                                                                //| |  |   |    |     |      |
                                        foreach (string item in tmpa)                                                                           //| |  |   |    |     |      |
                                        {                                                                                                       //|-|--|---|----|-----|------|-------T (loop) go through each file delegated to this thread
                                            if (item == text + add + textAfterNumTxtBx.Text + ext)                                              //| |  |   |    |     |      |       |
                                            {                                                                                                   //|-|--|---|----|-----|------|-------|--------T find the index in ref (and the array) the conflict file exists
                                                break;                                                                                          //|>|>>|>>>|>>>>|>>>>>|>>>>>>|>>>>>>>|>>>>>>>>|>>>>>>>>> if found, break out of loop
                                            }                                                                                                   //|-|--|---|----|-----|------|-------|--------|e)
                                            tmpi++;                                                                                             //|-|--|---|----|-----|------|-------|--------- itterate tmpi
                                                                                                                                                //| |  |   |    |     |      |       |      
                                        }                                                                                                       //|-|--|---|----|-----|------|-------|L)
                                                                                                                                                //| |  |   |    |     |      |
                                        if (tmpi == tmpa.Length)                                                                                //| |  |   |    |     |      |
                                        {                                                                                                       //|-|--|---|----|-----|------|-------T if could find the conflicting file in this delegation
                                            try                                                                                                 //|-|--|---|----|-----|------|-------| then we're dealing with a 'cross delegation' issue where the delegated file is being renamed on another thread
                                            {                                                                                                   //|-|--|---|----|-----|------|-------|--------T try
                                                if (System.Threading.Thread.CurrentThread.IsBackground)                                         //| |  |   |    |     |      |       |        |
                                                {                                                                                               //|-|--|---|----|-----|------|-------|--------|---------T if this thread is a background thread (which should always be true, this is requied to 'hold' other threads which fail the bellow)
                                                    Monitor.Enter(locker);                                                                      //|-|--|---|----|-----|------|-------|--------|---------|----------- 'bottle neck' any threads moving through here to move one at a time
                                                                                                                                                //| |  |   |    |     |      |       |        |         |
                                                }                                                                                               //|-|--|---|----|-----|------|-------|--------|---------|e)
                                                                                                                                                //| |  |   |    |     |      |       |        |         
                                                if (File.Exists(fsd.FileName + @"\" + text + add + textAfterNumTxtBx.Text + ext))               //| |  |   |    |     |      |       |        |         
                                                {                                                                                               //|-|--|---|----|-----|------|-------|--------|---------T
                                                    Array.Resize(ref renamedFrom, renamedFrom.Length + 1);                                      //|-|--|---|----|-----|------|-------|--------|---------|----------- rezise renamedFrom array to be one larger
                                                    Array.Resize(ref renamedTo, renamedTo.Length + 1);                                          //|-|--|---|----|-----|------|-------|--------|---------|----------- rezise renamedTo array to be one larger
                                                    renamedFrom[renamedFrom.Length - 1] = text + add + textAfterNumTxtBx.Text + ext;            //|-|--|---|----|-----|------|-------|--------|---------|----------- add the conflict file's name to the renamedFrom array
                                                    renamedTo[renamedTo.Length - 1] = "TMPRN" + tmpc + ext;                                     //|-|--|---|----|-----|------|-------|--------|---------|----------- add the conflict file's new name to the renamedTo array
                                                                                                                                                //| |  |   |    |     |      |       |        |         |
                                                    Microsoft.VisualBasic.FileIO.FileSystem.RenameFile(fsd.FileName + @"\"                      //|-|--|---|----|-----|------|-------|--------|---------|----------- rename the file
                                                        + text + add + textAfterNumTxtBx.Text + ext, "TMPRN" + tmpc + ext);                     //| |  |   |    |     |      |       |        |         |
                                                    threadErrorLogger.QueueLog("INFO: Temporarily renamed " + text + add                        //| |  |   |    |     |      |       |        |         |
                                                        + textAfterNumTxtBx.Text + ext + " to TMPRN" + tmpc + ext);                             //| |  |   |    |     |      |       |        |         |
                                                    threadProgress[((processorCount * claimed) - 1) + li] = "TMPRN" + tmpc + ext;               //| |  |   |    |     |      |       |        |         |
                                                    tmpc++;                                                                                     //|-|--|---|----|-----|------|-------|--------|---------|----------- itterate tmpc
                                                }                                                                                               //|-|--|---|----|-----|------|-------|--------|---------|e)
                                                System.Threading.Monitor.Exit(locker);                                                          //|-|--|---|----|-----|------|-------|--------|---------- release the bottle neck to allow the next thread to lock the above bit
                                                                                                                                                //| |  |   |    |     |      |       |        |         
                                            } catch (Exception e) {                                                                             //|-|--|---|----|-----|------|-------|--------|c) catch any errors and do:
                                                if (e.InnerException is FileNotFoundException || e.HResult == -2147024894)                      //| |  |   |    |     |      |       |        |  
                                                {                                                                                               //|-|--|---|----|-----|------|-------|--------|---------T if conflict has been self solved by the other thread then just continue on normally
                                                } else {                                                                                        //|-|--|---|----|-----|------|-------|--------|---------\c) otherwise if any other error occued
                                                    threadErrorLogger.QueueLog("CRIT: AN ERROR OCCURED DURING THE RENAMING PROCESS AND WAS"     //|-|--|---|----|-----|------|-------|--------|---------|----------- queue that an error occued
                                                        + " ABORTED! FAILED TO FIND THE FILE TO TEMPORARILY RENAME. " + text.ToUpper()          //| |  |   |    |     |      |       |        |         |
                                                        + add.ToUpper()                                                                         //| |  |   |    |     |      |       |        |         |
                                                    + textAfterNumTxtBx.Text.ToUpper() + ext.ToUpper() + " EXISTS BUT THE REFERENCE IS" +       //| |  |   |    |     |      |       |        |         |
                                                    " MISSING. ANOTHER THREAD MAY HAVE THE REFERENCE.\nEXITING!");                              //| |  |   |    |     |      |       |        |         |
                                                    //errorLogger.AppendQueue(threadErrorLogger.GetQueue());                                    //| |  |   |    |     |      |       |        |         |
                                                    //errorLogger.ReleaseQueue(path + @"\log.txt", true, true);                                 //| |  |   |    |     |      |       |        |         |
                                                                                                                                                //| |  |   |    |     |      |       |        |         |
                            //include opening "recover me" program here to clean up files and attempt to 'undo' the renaming process            //| |  |   |    |     |      |       |        |         |
                            //something like https://stackoverflow.com/questions/3173775/how-to-run-external-program-via-a-c-sharp-program      //| |  |   |    |     |      |       |        |         |
                                                    //Process.Start                                                                             //| |  |   |    |     |      |       |        |         |
                                                                                                                                                //| |  |   |    |     |      |       |        |         |
                                                    Invoke(new Action(() =>                                                                     //|-|--|---|----|-----|------|-------|--------|---------|----------T force all threads to run this action
                                                    {                                                                                           //| |  |   |    |     |      |       |        |         |          |
                                                        errorLogger.AppendQueue(threadErrorLogger.GetQueue());                                  //|-|--|---|----|-----|------|-------|--------|---------|----------|------------ append the above log to the main error logger queue
                                                        errorLogger.ReleaseQueue(path + @"\log.txt", true, true);                               //|-|--|---|----|-----|------|-------|--------|---------|----------|------------ release the queue
                                                        Environment.Exit(Environment.ExitCode);                                                 //|-|--|---|----|-----|------|-------|--------|---------|----------|------------ force the program to exit out on all threads used by this program
                                                    }));                                                                                        //|-|--|---|----|-----|------|-------|--------|---------|----------|e)
                                                }                                                                                               //|-|--|---|----|-----|------|-------|--------|---------|e)
                                            }                                                                                                   //|-|--|---|----|-----|------|-------|--------|e)
                                                                                                                                                //| |  |   |    |     |      |       |
                                        } else {                                                                                                //|-|--|---|----|-----|------|-------\c)
                                            if (System.Threading.Thread.CurrentThread.IsBackground)                                             //|-|--|---|----|-----|------|-------|
                                            {                                                                                                   //|-|--|---|----|-----|------|-------|--------T if the thread is a background thread
                                                Monitor.Enter(locker);                                                                          //|-|--|---|----|-----|------|-------|--------|---------- 'bottle neck' any threads moving through here to move one at a time, also affects the above locker (so that a 'same time' rename doesn't occur)
                                                                                                                                                //| |  |   |    |     |      |       |        |
                                            }                                                                                                   //|-|--|---|----|-----|------|-------|--------|e)
                                                                                                                                                //| |  |   |    |     |      |       |
                                            if (File.Exists(fsd.FileName + @"\" + text + add + textAfterNumTxtBx.Text + ext))                   //| |  |   |    |     |      |       |
                                            {                                                                                                   //|-|--|---|----|-----|------|-------|--------T (if) check again if the file exists as the threads are being delayed by the monitor (the locker)
                                                try                                                                                             //| |  |   |    |     |      |       |        |
                                                {                                                                                               //|-|--|---|----|-----|------|-------|--------|---------T try
                                                    Microsoft.VisualBasic.FileIO.FileSystem.RenameFile(fsd.FileName + @"\"                      //|-|--|---|----|-----|------|-------|--------|---------|----------- rename the file
                                                    + text + add + textAfterNumTxtBx.Text + ext, "TMPRN" + tmpc + ext);                         //| |  |   |    |     |      |       |        |         |
                                                    threadErrorLogger.QueueLog("INFO: Temporarily renamed " + text + add                        //| |  |   |    |     |      |       |        |         |
                                                        + textAfterNumTxtBx.Text + ext + " to TMPRN" + tmpc + ext);                             //| |  |   |    |     |      |       |        |         |
                                                    threadProgress[((processorCount * claimed) - 1) + li] = "TMPRN" + tmpc + ext;               //| |  |   |    |     |      |       |        |         |
                                                    tmpa[tmpi] = "TMPRN" + tmpc + ext;                                                          //|-|--|---|----|-----|------|-------|--------|---------|----------- update the ref array (tmpa) to the new file name
                                                    tmpc++;                                                                                     //|-|--|---|----|-----|------|-------|--------|---------|----------- itterate tmpc
                                                } catch (Exception e) {                                                                         //|-|--|---|----|-----|------|-------|--------|---------|c) catch any error
                                                    if (e.InnerException is FileNotFoundException || e.HResult == -2147024894)                  //|-|--|---|----|-----|------|-------|--------|---------|----------T if conflict has been self solved by the other thread then just continue on normally
                                                    {                                                                                           //| |  |   |    |     |      |       |        |         |          |
                                                    } else {                                                                                    //|-|--|---|----|-----|------|-------|--------|---------|----------\c) otherwise if any other error type
                                                        threadErrorLogger.QueueLog("CRIT: AN UNKNOWN ERROR OCCURED DURING THE RENAMING PROCESS" //|-|--|---|----|-----|------|-------|--------|---------|----------|------------ log that an error occured
                                                            + " AND WAS ABORTED! MESSAGE: " + e.Message.ToUpper());                             //| |  |   |    |     |      |       |        |         |          |
                                                        //threadErrorLogger.ReleaseQueue(path + @"\log.txt", true, true);                       //| |  |   |    |     |      |       |        |         |          |
                                                                                                                                                //| |  |   |    |     |      |       |        |         |          |
                                                        //include opening "recover me" program here to clean up files and attempt to 'undo'     //| |  |   |    |     |      |       |        |         |          |
                                                        //the renaming process. something like                                                  //| |  |   |    |     |      |       |        |         |          |
                                       //https://stackoverflow.com/questions/3173775/how-to-run-external-program-via-a-c-sharp-program          //| |  |   |    |     |      |       |        |         |          |
                                                        //Process.Start                                                                         //| |  |   |    |     |      |       |        |         |          |
                                                                                                                                                //| |  |   |    |     |      |       |        |         |          |
                                                        Invoke(new Action(() =>                                                                 //|-|--|---|----|-----|------|-------|--------|---------|----------|-----------T force all threads to run this action
                                                        {                                                                                       //| |  |   |    |     |      |       |        |         |          |           |
                                                            errorLogger.AppendQueue(threadErrorLogger.GetQueue());                              //|-|--|---|----|-----|------|-------|--------|---------|----------|-----------|------------- append the above log to the main error logger queue
                                                            errorLogger.ReleaseQueue(path + @"\log.txt", true, true);                           //|-|--|---|----|-----|------|-------|--------|---------|----------|-----------|------------- release the queue
                                                            Environment.Exit(Environment.ExitCode);                                             //|-|--|---|----|-----|------|-------|--------|---------|----------|-----------|------------- force the program to exit out on all threads used by this program
                                                        }));                                                                                    //|-|--|---|----|-----|------|-------|--------|---------|----------|-----------|e)
                                                    }                                                                                           //|-|--|---|----|-----|------|-------|--------|---------|----------|e)
                                                                                                                                                //| |  |   |    |     |      |       |        |         |
                                                }                                                                                               //|-|--|---|----|-----|------|-------|--------|---------|e)
                                            }                                                                                                   //|-|--|---|----|-----|------|-------|        |e)
                                            Monitor.Exit(locker);                                                                               //|-|--|---|----|-----|------|-------|--------- 'bottle neck' any threads moving through here to move one at a time
                                        }                                                                                                       //|-|--|---|----|-----|------|-------|e)
                                                                                                                                                //| |  |   |    |     |      |
                                    }                                                                                                           //|-|--|---|----|-----|------|e)
                                                                                                                                                //| |  |   |    |     |
                                    bool specialRename = false;                                                                                 //|-|--|---|----|-----|------ store weather or not bellow occured (cross delegation - other thread handler)
                                    for (int i1 = 0; i1 < renamedFrom.Length; i1++)                                                             //| |  |   |    |     |
                                    {                                                                                                           //|-|--|---|----|-----|------T check through all special tmprn files
                                        if (renamedFrom[i1] == firstFileName)                                                                   //| |  |   |    |     |      |
                                        {                                                                                                       //|-|--|---|----|-----|------|-------T if this thread is renaming a file that was renamed to a special tmprn
                                            try                                                                                                 //| |  |   |    |     |      |       |      
                                            {                                                                                                   //|-|--|---|----|-----|------|-------|--------T try
                                                Microsoft.VisualBasic.FileIO.FileSystem.RenameFile(fsd.FileName + @"\" + renamedTo[i1],         //|-|--|---|----|-----|------|-------|--------|--------- rename it back to its original name
                                                text + add + textAfterNumTxtBx.Text + ext);                                                     //| |  |   |    |     |      |       |        |
                                                threadErrorLogger.QueueLog("INFO: " + renamedFrom[i1] + " no longer existed so instead "        //| |  |   |    |     |      |       |        |
                                                    + renamedTo[i1] + " was renamed to " + text + add + textAfterNumTxtBx.Text + ext);          //| |  |   |    |     |      |       |        |
                                                threadProgress[(split * (claimed - 1)) + li] = text + add + textAfterNumTxtBx.Text + ext;       //| |  |   |    |     |      |       |        |
                                                specialRename = true;                                                                           //|-|--|---|----|-----|------|-------|--------|--------- set that a rename has already occured for this target file
                                                break;                                                                                          //|>|>>|>>>|>>>>|>>>>>|>>>>>>|>>>>>>>|>>>>>>>>|>>>>>>>>> break;
                                            } catch (Exception e) {                                                                             //|-|--|---|----|-----|------|-------|--------|c) catch any error
                                                threadErrorLogger.QueueLog("CRIT: AN UNKNOWN ERROR OCCURED DURING THE RENAMING PROCESS AND" +   //|-|--|---|----|-----|------|-------|--------|--------- queue that an error occured
                                                    " WAS ABORTED! MESSAGE: " + e.Message.ToUpper());                                           //| |  |   |    |     |      |       |        |
                                                threadErrorLogger.QueueLog("INFO: renamedTo[i1] exists? "                                       //| |  |   |    |     |      |       |        |
                                                    + File.Exists(fsd.FileName + @"\" + renamedTo[i1]));                                        //| |  |   |    |     |      |       |        |
                                                threadErrorLogger.QueueLog("INFO: " + text + add + textAfterNumTxtBx.Text + ext + " exists? "   //| |  |   |    |     |      |       |        |
                                                    + File.Exists(fsd.FileName + @"\" + text + add + textAfterNumTxtBx.Text + ext));            //| |  |   |    |     |      |       |        |
                                                //threadErrorLogger.ReleaseQueue(path + @"\log.txt", true, true);                               //| |  |   |    |     |      |       |        |
                                                                                                                                                //| |  |   |    |     |      |       |        |
                                 //include opening "recover me" program here to clean up files and attempt to 'undo' the renaming process       //| |  |   |    |     |      |       |        |
                                 //something like https://stackoverflow.com/questions/3173775/how-to-run-external-program-via-a-c-sharp-program //| |  |   |    |     |      |       |        |
                                                //Process.Start                                                                                 //| |  |   |    |     |      |       |        |
                                                                                                                                                //| |  |   |    |     |      |       |        |
                                                Invoke(new Action(() =>                                                                         //|-|--|---|----|-----|------|-------|--------|---------T force all threads to run this action
                                                {                                                                                               //| |  |   |    |     |      |       |        |         |
                                                    errorLogger.AppendQueue(threadErrorLogger.GetQueue());                                      //|-|--|---|----|-----|------|-------|--------|---------|---------- append the above log to the main error logger queue
                                                    errorLogger.ReleaseQueue(path + @"\log.txt", true, true);                                   //|-|--|---|----|-----|------|-------|--------|---------|---------- release the queue
                                                    Environment.Exit(Environment.ExitCode);                                                     //|-|--|---|----|-----|------|-------|--------|---------|---------- force the program to exit out on all threads used by this program
                                                }));                                                                                            //|-|--|---|----|-----|------|-------|--------|---------|e)
                                                                                                                                                //| |  |   |    |     |      |       |        |
                                            }                                                                                                   //|-|--|---|----|-----|------|-------|--------|e)
                                                                                                                                                //| |  |   |    |     |      |       |
                                                                                                                                                //| |  |   |    |     |      |       |
                                        }                                                                                                       //|-|--|---|----|-----|------|-------|e)
                                                                                                                                                //| |  |   |    |     |      |
                                    }                                                                                                           //|-|--|---|----|-----|------|L)
                                                                                                                                                //| |  |   |    |     |
                                    if (!specialRename)                                                                                         //| |  |   |    |     |
                                    {                                                                                                           //|-|--|---|----|-----|------T if the above did not occur
                                        try                                                                                                     //| |  |   |    |     |      |
                                        {                                                                                                       //|-|--|---|----|-----|------|-------T try
                                            if (File.Exists(fsd.FileName + @"\" + firstFileName))                                               //| |  |   |    |     |      |       |
                                            {                                                                                                   //|-|--|---|----|-----|------|-------|--------T if the file to rename exists
                                                Microsoft.VisualBasic.FileIO.FileSystem.RenameFile(fsd.FileName + @"\" + firstFileName,         //|-|--|---|----|-----|------|-------|--------|---------- rename the file
                                                text + add + textAfterNumTxtBx.Text + ext);                                                     //| |  |   |    |     |      |       |        |
                                                threadErrorLogger.QueueLog("INFO: Renamed " + firstFileName + " to " + text + add               //| |  |   |    |     |      |       |        |
                                                    + textAfterNumTxtBx.Text + ext);                                                            //| |  |   |    |     |      |       |        |
                                                threadProgress[(split * (claimed - 1)) + li] = text + add + textAfterNumTxtBx.Text + ext;       //| |  |   |    |     |      |       |        |
                                            } else {                                                                                            //|-|--|---|----|-----|------|-------|--------|\c)
                                                int dud = 0;                                                                                    //| |  |   |    |     |      |       |        |
                                            }                                                                                                   //|-|--|---|----|-----|------|-------|--------|e)
                                        } catch (Exception e) {                                                                                 //|-|--|---|----|-----|------|-------|c)
                                            threadErrorLogger.QueueLog("CRIT: AN UNKNOWN ERROR OCCURED DURING THE RENAMING PROCESS AND WAS" +   //|-|--|---|----|-----|------|-------|--------- log that an unknown error occured
                                                " ABORTED! MESSAGE: " + e.Message.ToUpper());                                                   //| |  |   |    |     |      |       |
                                            threadErrorLogger.QueueLog("INFO: renamedTo[i1] exists? "                                           //| |  |   |    |     |      |       |
                                                    + File.Exists(fsd.FileName + @"\" + firstFileName));                                        //| |  |   |    |     |      |       |
                                                threadErrorLogger.QueueLog("INFO: " + text + add + textAfterNumTxtBx.Text + ext + " exists? "   //| |  |   |    |     |      |       |
                                                    + File.Exists(fsd.FileName + @"\" + text + add + textAfterNumTxtBx.Text + ext));            //| |  |   |    |     |      |       |
                                                                                                                                                //| |  |   |    |     |      |       |
                                            //include opening "recover me" program here to clean up files and attempt to 'undo' the             //| |  |   |    |     |      |       |
                                            //renaming process                                                                                  //| |  |   |    |     |      |       |
                                                                                                                                                //| |  |   |    |     |      |       |
                                            Invoke(new Action(() =>                                                                             //|-|--|---|----|-----|------|-------|--------T force all threads to run this action
                                            {                                                                                                   //| |  |   |    |     |      |       |        |
                                                errorLogger.ReleaseQueue(path + @"\log", true, true);                                           //|-|--|---|----|-----|------|-------|--------|-------- append the above log to the main error logger queue
                                                threadErrorLogger.ReleaseQueue(path + @"\log.txt", true, true);                                 //|-|--|---|----|-----|------|-------|--------|-------- release the queue
                                                Environment.Exit(Environment.ExitCode);                                                         //|-|--|---|----|-----|------|-------|--------|-------- force the program to exit out on all threads used by this program
                                            }));                                                                                                //|-|--|---|----|-----|------|-------|--------|e)
                                                                                                                                                //| |  |   |    |     |      |       |
                                        }                                                                                                       //|-|--|---|----|-----|------|-------|e)
                                    }                                                                                                           //|-|--|---|----|-----|------|e)
                                                                                                                                                //| |  |   |    |     |
                                }                                                                                                               //|-|--|---|----|-----|e)
                                li++;                                                                                                           //|-|--|---|----|------ count amount of times looped
                                ThreadProgressBar_TabIndexChanged(null, EventArgs.Empty);                                                       //|-|--|---|----|------ force the tread progress bar to itterate
                                                                                                                                                //| |  |   |    |
                            }                                                                                                                   //|-|--|---|----|L)
                            threadErrorLogger.QueueLog("INFO: Thread " + System.Threading.Thread.CurrentThread.ManagedThreadId                  //| |  |   |
                                + " has finished renaming its delegated files and is now waiting on the other thread to finish.");              //| |  |   |
                                                                                                                                                //| |  |   |
                        } else {                                                                                                                //|-|--|---\c) otherwise if failClaim is true (all files where not claimed correctly)
                            Debug.WriteLine("CTITICAL ERROR OCCURED - NULL CLAIMING");                                                          //| |  |   |
                            threadErrorLogger.QueueLog("CRIT: A critical error occured during multi-threading inter-setup - NULL CLAIMING." +   //|-|--|---|----- queue that an error occured
                                "A ref file was not claimed but the process may not have stopped before the program started renaming.");        //| |  |   |
                            threadErrorLogger.ReleaseQueue(path + @"\log.txt");                                                                 //|-|--|---|----- release the queue
                                                                                                                                                //| |  |   |
                            MessageBox.Show("A critical error occured during multi-threading setup and will be aborted to preserve data" +      //| |  |   |
                                " integrity, a log has been saved to:\n" + path + @"\log.txt"                                                   //| |  |   |
                                + "\nPlease report the log to the issues page on the relevent branch. (" + this.Name + ")"                      //| |  |   |
                                , "Critical error: NULL FILE REF", MessageBoxButtons.OK, MessageBoxIcon.Stop);                                  //| |  |   |
                                                                                                                                                //| |  |   |
                        }                                                                                                                       //|-|--|---|e)
                                                                                                                                                //| |  |
                                                                                                                                                //| |  |
                    }                                                                                                                           //|-|--|e)
                                                                                                                                                //| |
                }                                                                                                                               //|-|e)
                                                                                                                                                //|
                                                                                                                                                //|
            }                                                                                                                                   //|e)
                                                                                                                                                //
            if (file.All(item => item != null))                                                                                                 //
            {                                                                                                                                   //T if none of the files are null
                errorLogger.AppendQueue(threadErrorLogger.GetQueue(), false);                                                                   //|--- release all the queued logs to main logger
            }                                                                                                                                   //|e)
                                                                                                                                                //
            threadErrorLogger = null;                                                                                                           //- dispose of the thread error logger

        }

        //when the renaming process is finished
        private void backgroundThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            System.Diagnostics.Process.Start(selectedPath);                 //- open the selceted folder for the user
            BtnMassRename.Enabled = true;                                   //- re-enable the mass rename button
            PreferencesTSMI.Enabled = true;                                 //- re-enable the preferences TSMI
            active = false;                                                 //- set that the program is not longer mass renaming
            textAfterNumTxtBx.Text = "Text after number (optional)";        //- set the text after number text box to display its previous text so that it isn't just blank
            ThreadProgressBar.Visible = false;                              //- hide the thread progress bar

            //for debugging
            //foreach (string item in threadProgress)
            //{
            //    if (item != "")
            //    {
            //        errorLogger.QueueLog("DEBG: " + item);
            //    }

            //}

        }


        //UX controll (User Experience)
        //Mostly handles the text dissapearing and reapearing when the controll is in/out of focus and the threads' progress bar

        //Sets the fileRenameTxtBx text to empty if the user wants to start editing the text
        private void fileRenameTxtBx_Enter(object sender, EventArgs e)
        {
            if (fileRenameTxtBx.Text == "What to rename the file to")
            {
                fileRenameTxtBx.Text = "";
                fileRenameTxtBx.TextAlign = HorizontalAlignment.Left;
            }
        }

        //Sets the fileRenameTxtBx text back to its original text if the user no longer wants to edit the text (as has not entered any)
        private void fileRenameTxtBx_Leave(object sender, EventArgs e)
        {
            if (fileRenameTxtBx.Text == "")
            {
                fileRenameTxtBx.Text = "What to rename the file to";
                fileRenameTxtBx.TextAlign = HorizontalAlignment.Center;
            }
        }

        //Sets the textAfterNumTxtBx text to empty if the user wants to start editing the text
        private void textAfterNumTxtBx_Enter(object sender, EventArgs e)
        {
            if (textAfterNumTxtBx.Text == "Text after number (optional)" || textAfterNumTxtBx.Text == "Text after name")
            {
                textAfterNumTxtBx.Text = "";
                textAfterNumTxtBx.TextAlign = HorizontalAlignment.Left;
            }
        }

        //Sets the textAfterNumTxtBx text based on if counting type is disabled or not
        private void textAfterNumTxtBx_Leave(object sender, EventArgs e)
        {
            if (textAfterNumTxtBx.Text == "")
            {
                if (fileRenameTxtBx.Enabled == true)
                {
                    textAfterNumTxtBx.Text = "Text after number (optional)";
                }
                else
                {
                    textAfterNumTxtBx.Text = "Text after name";
                }
                textAfterNumTxtBx.TextAlign = HorizontalAlignment.Center;
            }
        }

        //Sets the extentionTxtBx text to empty if the user wants to start editing the text
        private void extentionTxtBx_Enter(object sender, EventArgs e)
        {
            if (extentionTxtBx.Text == "Extention (optional)" || extentionTxtBx.Text == "Extention")
            {
                extentionTxtBx.Text = "";
                extentionTxtBx.TextAlign = HorizontalAlignment.Left;
            }
        }

        //Sets the extentionTxtBx text based on if counting type is disabled or not
        private void extentionTxtBx_Leave(object sender, EventArgs e)
        {
            if (extentionTxtBx.Text == "")
            {
                if (fileRenameTxtBx.Enabled == true)
                {
                    extentionTxtBx.Text = "Extention (optional)";
                }
                else
                {
                    extentionTxtBx.Text = "Extention";
                }
                extentionTxtBx.TextAlign = HorizontalAlignment.Center;
            }
        }

        //Only occurs when a thread wants to itterate the progress bar
        private void ThreadProgressBar_TabIndexChanged(object sender, EventArgs e)
        {

            Invoke(new Action(() =>
            {
                ThreadProgressBar.Value++;

                //https://www.codeproject.com/Articles/31406/Add-the-Percent-or-Any-Text-into-a-Standard-Progre
                int percent = (int)(((double)(ThreadProgressBar.Value - ThreadProgressBar.Minimum) /
                (double)(ThreadProgressBar.Maximum - ThreadProgressBar.Minimum)) * 100);
                int decimalP = (int)(((((double)(ThreadProgressBar.Value - ThreadProgressBar.Minimum) /
                (double)(ThreadProgressBar.Maximum - ThreadProgressBar.Minimum)) * 100) - percent) * 100);

                ThreadProgressBar.Refresh();//refresh the drawing of the progress bar
                using (gr = ThreadProgressBar.CreateGraphics())
                {//create the percent text
                    SetStyle(ControlStyles.OptimizedDoubleBuffer, true);//helps reduce flickering.
                    gr.DrawString(percent.ToString() + "." + decimalP.ToString() + "%",
                    SystemFonts.DefaultFont, Brushes.Black,
                    new PointF(ThreadProgressBar.Width / 2 - (gr.MeasureString(percent.ToString() + "." + decimalP.ToString() + "%",
                    SystemFonts.DefaultFont).Width / 2.0F),
                    ThreadProgressBar.Height / 2 - (gr.MeasureString(percent.ToString() + "." + decimalP.ToString() + "%",
                    SystemFonts.DefaultFont).Height / 2.0F)));
                }

            }));

        }

        //Sets the forms opacity
        private void sldBrOpacity_Scroll(object sender, EventArgs e)
        {
            double opacity = sldBrOpacity.Value;//get the value from the opacity slider
            double test = (opacity / 100);//convert to decimal

            this.Opacity = test;//set the opacity
        }


        //UI controll (User Interaction)
        //Handles user interaction which change the way the mass renaming process functions

        //Exits the application.
        private void exitBtn_Click(object sender, EventArgs e)
        {
            if (!active)
            {
                Close();
            }
        }

            //selection
        //Handles setting the selection type to alphanumerical and unsetting the other selection types
        private void alphanumericSTBtn_Click(object sender, EventArgs e)
        {
            alphanumericSTBtn.Checked = true;//
            alphabraicSTBtn.Checked = false;
            regexSTBtn.Checked = false;
            randomSTBtn.Checked = false;

        }

        //Handles setting the selection type to alphabraic and unsetting the other selection types
        private void alphabraicSTBtn_Click(object sender, EventArgs e)
        {
            alphanumericSTBtn.Checked = false;
            alphabraicSTBtn.Checked = true;//
            regexSTBtn.Checked = false;
            randomSTBtn.Checked = false;

        }

        //Unused
        private void regexSTBtn_Click(object sender, EventArgs e)
        {
            alphabraicSTBtn.Checked = false;
            alphanumericSTBtn.Checked = false;
            regexSTBtn.Checked = true;//
            randomSTBtn.Checked = false;

        }

        //Handles setting the selection type to random and unsetting the other selection types
        private void randomSTBtn_Click(object sender, EventArgs e)
        {
            alphabraicSTBtn.Checked = false;
            alphanumericSTBtn.Checked = false;
            regexSTBtn.Checked = false;
            randomSTBtn.Checked = true;//

        }

            //ordering
        //Handles setting the counting type to alphanumerical and unsetting the other counting types
        private void numericaldTSMI_Click(object sender, EventArgs e)
        {
            numericaldTSMI.Checked = true;//
            alphabeticalTSMI.Checked = false;
            customTSMI.Checked = false;
            disableCountingTSMI.Checked = false;
        }

        //Handles setting the counting type to alphabetical and unsetting the other counting types
        private void alphabeticalTSMI_Click(object sender, EventArgs e)
        {
            numericaldTSMI.Checked = false;
            alphabeticalTSMI.Checked = true;//
            customTSMI.Checked = false;
            disableCountingTSMI.Checked = false;
        }
        
        //Unused
        private void customTSMI_Click(object sender, EventArgs e)
        {
            numericaldTSMI.Checked = false;
            alphabeticalTSMI.Checked = false;
            customTSMI.Checked = true;//
            disableCountingTSMI.Checked = false;
        }

        //Handles disabling the main renaming text box, chaning its and the other boxes' text, moving the focus and unsetting the other counting types.
        private void disableCountingTSMI_Click(object sender, EventArgs e)
        {
            if (disableCountingTSMI.Checked == false)
            {
                disableCountingTSMI.Checked = true;//
                numericaldTSMI.Checked = false;
                alphabeticalTSMI.Checked = false;
                customTSMI.Checked = false;

                numericaldTSMI.Enabled = false;
                alphabeticalTSMI.Enabled = false;
                customTSMI.Enabled = false;

                fileRenameTxtBx.Text = "You may not rename files while sorting is disabled";
                fileRenameTxtBx.Enabled = false;
                fileRenameTxtBx.TextAlign = HorizontalAlignment.Center;
                extentionTxtBx.Text = "Extention";
                extentionTxtBx.TextAlign = HorizontalAlignment.Center;
                textAfterNumTxtBx.Text = "Text after name";
                textAfterNumTxtBx.TextAlign = HorizontalAlignment.Center;

                textAfterNumTxtBx.Focus();
                extentionTxtBx.Focus();//fixes a bug with the controll not knowing it's focused by unfocusing and refocusing

            }
            else
            {
                fileRenameTxtBx.Text = "What to rename the file to";
                fileRenameTxtBx.Enabled = true;
                fileRenameTxtBx.TextAlign = HorizontalAlignment.Center;
                extentionTxtBx.Text = "Extention (optional)";
                extentionTxtBx.TextAlign = HorizontalAlignment.Center;
                textAfterNumTxtBx.Text = "Text after number (optional)";
                textAfterNumTxtBx.TextAlign = HorizontalAlignment.Center;

                disableCountingTSMI.Checked = false;
                numericaldTSMI.Checked = true;

                numericaldTSMI.Enabled = true;
                alphabeticalTSMI.Enabled = true;
                //customTSMI.Enabled = true;

            }
        }

            //help
        //Openes the guide page.
        private void guideBtn_Click(object sender, EventArgs e)
        {
            MRGuide test = new MRGuide();
            Form isOpen = Application.OpenForms["CounterForm"];//get if the window is still open

            if (isOpen != null) { isOpen.Close(); }//if it is then close it

            test.Show();
        }

        //Openes the about page.
        private void aboutBtn_Click(object sender, EventArgs e)
        {
            AboutMR test = new AboutMR();
            Form isOpen = Application.OpenForms["AboutBoxMR"];//get if the window is still open

            if (isOpen != null) { isOpen.Close(); }//if it is then close it

            test.Show();
        }

            //other
        //Sets the global variable which idsables warnings between true and false.
        private void disableWarningsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!DW)
            {
                DW = true;
            }
            else
            {
                DW = false;
            }
        }

        //Handles disallowing the user to enter more than 255 characters between the fileRenameTxtBx and the textAfterNumTxtBx as well as disabling 
        //the mass rename button when the mass renaming process is active and when the fileRenameTxtBx or extentionTxtBx have any count of characters.
        private void fileRenameTxtBx_TextChanged(object sender, EventArgs e)
        {
            textAfterNumTxtBx.MaxLength = 255 - fileRenameTxtBx.Text.Length;

            if (textAfterNumTxtBx.Text.Length >= textAfterNumTxtBx.MaxLength && textAfterNumTxtBx.Text != "Text after number (optional)" && textAfterNumTxtBx.Text.Length != 0)
            {
                textAfterNumTxtBx.Text = textAfterNumTxtBx.Text.Remove(textAfterNumTxtBx.Text.Length - 1);

            }

            if (!active)
            {
                if (fileRenameTxtBx.Text.Length == 0 && extentionTxtBx.Text == "Extention (optional)") { BtnMassRename.Enabled = false; }
                else if ((fileRenameTxtBx.Text.Length != 0 && fileRenameTxtBx.Text != "What to rename the file to")
                    || (extentionTxtBx.Text.Length != 0 && extentionTxtBx.Text != "Extention (optional)")) { BtnMassRename.Enabled = true; }
            }
        }

        //Handles disallowing the user to enter more than 255 characters between the fileRenameTxtBx and the textAfterNumTxtBx as well as disabling 
        //the mass rename button when the mass renaming process is active and when the fileRenameTxtBx or extentionTxtBx have any count of characters.
        private void textAfterNumTxtBx_TextChanged(object sender, EventArgs e)
        {
            if (fileRenameTxtBx.Enabled == true)
            {
                fileRenameTxtBx.MaxLength = 255 - textAfterNumTxtBx.Text.Length;

                if (fileRenameTxtBx.Text.Length >= fileRenameTxtBx.MaxLength && fileRenameTxtBx.Text != "What to rename the file to" && fileRenameTxtBx.Text.Length != 0)
                {
                    fileRenameTxtBx.Text = fileRenameTxtBx.Text.Remove(fileRenameTxtBx.Text.Length - 1);

                }
            }
            else
            {

                if (!active)
                {
                    if ((textAfterNumTxtBx.Text.Length != 0 && textAfterNumTxtBx.Text == "Text after name" &&
                             extentionTxtBx.Text.Length != 0 && extentionTxtBx.Text == "Extention")
                             || (textAfterNumTxtBx.Text.Length == 0 && extentionTxtBx.Text.Length != 0 && extentionTxtBx.Text == "Extention")) { BtnMassRename.Enabled = false; }
                    else if ((extentionTxtBx.Text.Length != 0 && extentionTxtBx.Text != "Extention")
                        || (textAfterNumTxtBx.Text.Length != 0 && textAfterNumTxtBx.Text != "Text after name")) { BtnMassRename.Enabled = true; }
                }

            }
        }

        //Handles disabling the mass rename button when the process is active, disabling the button when there is or is not any text in the
        //fileRenameTxtBx and/or extentionTxtBx and disabling the button when there is or is not any text in the fileRenameTxtBx and/or extentionTxtBx
        //when the counting type is set to "disalbed".
        private void extentionTxtBx_TextChanged(object sender, EventArgs e)
        {

            if (!active)
            {
                if (fileRenameTxtBx.Enabled == true)
                {
                    if ((extentionTxtBx.Text.Length != 0 && extentionTxtBx.Text == "Extention (optional)" &&
                        fileRenameTxtBx.Text.Length != 0 && fileRenameTxtBx.Text == "What to rename the file to")
                        || (extentionTxtBx.Text.Length == 0 && fileRenameTxtBx.Text.Length != 0 && fileRenameTxtBx.Text == "What to rename the file to")) { BtnMassRename.Enabled = false; }
                    else if ((fileRenameTxtBx.Text.Length != 0 && fileRenameTxtBx.Text != "What to rename the file to")
                        || (extentionTxtBx.Text.Length != 0 && extentionTxtBx.Text != "Extention (optional)")) { BtnMassRename.Enabled = true; }
                }
                else
                {
                    //inverse of bellow

                    if ((extentionTxtBx.Text.Length != 0 && extentionTxtBx.Text == "Extention" &&
                         fileRenameTxtBx.Text.Length != 0 && fileRenameTxtBx.Text == "Text after name")
                         || (extentionTxtBx.Text.Length == 0 && textAfterNumTxtBx.Text.Length != 0 && textAfterNumTxtBx.Text == "Text after name")) { BtnMassRename.Enabled = false; }
                    else if ((textAfterNumTxtBx.Text.Length != 0 && textAfterNumTxtBx.Text != "Text after name")
                        || (extentionTxtBx.Text.Length != 0 && extentionTxtBx.Text != "Extention")) { BtnMassRename.Enabled = true; }
                }
            }
            //if ((extention is NULL AND rename is NULL) OR (extention is empty AND rename is NULL)) then set false
            //otherwise if (rename is NOT NULL OR extention is NOT NULL) then set true
        }

        //Handles when the form begins to close while the renaming process is and isn't active, releasing all queued logs and asking the user if they
        //want to wait for the process to finish.
        private void MassRename_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = DialogResult.No;
            if (file.Length == 0)
            {//if file reference is empty
                errorLogger.QueueLog("INFO: Form Closing!");
                errorLogger.QueueLog("", "", 4, true);
            }
            else
            {
                errorLogger.QueueLog("WARN: The user attempted to close the program while the process was still running.");
                result = MessageBox.Show("Are you sure you want to exit? The program is still renaming files" +
                    ", exiting while this continues may result in undesired affects!", "Rename process still active", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                {
                    while (true)
                    {
                        if (file.Length == 0)
                        {
                            break;
                        }

                        int breakOut = 10;
                        while (file.Length > 0 && breakOut != 0)
                        {
                            System.Threading.Thread.Sleep(1000);

                            if (file.Length == 0)
                            {//check every second if the renaming has finished
                                break;
                            }
                            breakOut--;

                        }

                        result = MessageBox.Show("Continue waiting?", "Process is taking a long time", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.No)
                        {
                            errorLogger.QueueLog("WARN: The program is taking a long time to finish the process.");
                            break;
                        }
                    }

                }

                errorLogger.QueueLog("INFO: Form Closing!");
                errorLogger.QueueLog("", "", 4, true);

            }

            //File.Create(path + @"\log.txt").Close();
            errorLogger.ReleaseQueue(path + @"\log.txt", true, true);

        }

    }
}
