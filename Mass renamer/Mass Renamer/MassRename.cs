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
        public static int i = 0;
        int split = 0;//stores the amount of files delegated to each thread
        string[] threadProgress = new string[0];//used for debugging to check if all of the threads have renamed correctly
        bool msg = false;//supposed to suppress duplicate error messages from appearing for the user (needs work)
        string selectedPath = "";//stores what the selected path currently is based on the user input direction
        int[] ThreadClaimed = new int[0];//stores which thread (the ThreadClaimed[#] object) claimed what file (#)
        //string[] refAll = new string[0];//stores the entire list of all the delegated files (unused, might use for undo)
        string[] renamedFrom = new string[0];//stores the original name of a file. Used for handling rename conflicts and undo
        string[] renamedTo = new string[0];//stores the name the file was renamed to.
        int tmpc = 1;//stores which TMPRN file we're at

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
            if (processorCount == 0) {
                MessageBox.Show("This program does not support single core computers. Please use version 1.0.2.5 or earlier.");
                Close();

            }

            //get the path to work in
            if (Environment.CurrentDirectory.Contains(@"\Debug"))
            {//if in debug mode
                path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location).TrimEnd(@"\bin\Debug".ToCharArray()) + @"\Resources";

                if (!File.Exists(path + @"\log.txt"))
                {
                    File.CreateText(path + @"\log.txt").Close();

                }
                else {
                    errorLogger.QueueLog("", "", 4, true);

                }

                if (path.Contains("SOL"))
                {
                    errorLogger.QueueLog("INFO: Running in debug mode in SOL.");

                }
                else {
                    errorLogger.QueueLog("INFO: Running in debug mode.");

                }
            }
            else
            {
                path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\Resources";

                if (!File.Exists(path + @"\log.txt"))
                {
                    File.CreateText(path + @"\log.txt").Close();

                }
                else {
                    errorLogger.QueueLog("", "", 4, true);

                }

                if (path.Contains("SOL"))
                {
                    errorLogger.QueueLog("INFO: Running in debug mode in SOL.");

                }
                else
                {
                    errorLogger.QueueLog("INFO: Running in debug mode.");

                }
            }

            this.Icon = new Icon(path + @"\SOLICO.ico");

            //only use this for debugging the queueLog() function
            //if (!File.Exists(path + @"\test.txt"))
            //    {
            //        File.Create(path + @"\test.txt").Close();
            //    }
            //ErrorLogger TestErrorLogger = new ErrorLogger();
            //TestErrorLogger.QueueLog("", "", 4, true);
            //TestErrorLogger.QueueLog("test");
            //TestErrorLogger.QueueLog("test2", "", 4, false, 145, "BtnMassRename_Click");
            //TestErrorLogger.QueueLog("test3", "", 4, false, 123456789, "backgroundThread_RunWorkerCompleted");
            //TestErrorLogger.QueueLog("test4", "", 4, false, 123456789, "backgroundThread_RunWorkerCompleted");
            //TestErrorLogger.QueueLog("test5", "", 4, false, 123456, "backgroundThread_RunWorkerCompleted");

            //errorLogger.QueueLog("test");
            //errorLogger.QueueLog("test");
            //errorLogger.QueueLog("test");
            //errorLogger.QueueLog("test");

            //bool ttest = TestErrorLogger.AppendQueue(errorLogger.GetQueue());

            ////TestErrorLogger.ForceReTabbing(8);

            ////TestErrorLogger.RemoveItemFromQueue(2);

            ////string[] queueItem = new string[8];
            ////queueItem.SetValue("MassRename_Load()", 1);
            ////queueItem.SetValue("", 3);
            ////ttest = TestErrorLogger.RemoveItemFromQueue(queueItem);
            ////ttest = TestErrorLogger.RemoveItemFromQueue(queueItem);

            ////TestErrorLogger.RemoveItemFromQueue();

            //TestErrorLogger.ReleaseQueue(path + @"\test.txt");

            //failsafe if ref files still exist due to a unexpected fail before ref files could be reset
            foreach (var refFile in Directory.GetFiles(path, "ref*"))
            {
                File.Delete(refFile);

            }

            //FolderSelectDialog.cs and Refelector.cs is from http://www.lyquidity.com/devblog/?p=136
            //allows for the use of vita/win7 folder selector.
            fsd = new FolderSelectDialog                                                                                     
            {                                                                                                                                   //T initialise a new 'folder selection dialog box' object (controller)
                Title = "Select at leat one folder",                                                                                            //|--- Set the dialog box's title
                InitialDirectory = @"C:\"                                                                                                       //|--- changes the initial directory opened when the dialog box is opened to the C drive
            };                                                                                                                                  //|e)

            //Environment.ProcessorCount
            //System.Threading.Tasks.Dataflow.
            //https://docs.microsoft.com/en-us/visualstudio/debugger/using-the-tasks-window?view=vs-2017
            //https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.dataflow?view=netcore-2.2
            //https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.dataflow.actionblock-1?view=netcore-2.2

        }

        private void BtnMassRename_Click(object sender, EventArgs e)
        {
            split = 0;
            ThreadProgressBar.Value = 0;
            tmpc = 1;
            Array.Resize(ref renamedFrom, 0);
            Array.Resize(ref renamedTo, 0);

            errorLogger.QueueLog("INFO: Started a new mass rename instance.");
            active = true;
            BtnMassRename.Enabled = false;
            PreferencesTSMI.Enabled = false;
            StreamWriter[] refa = new StreamWriter[processorCount];
            reSync = new bool[processorCount];
            ThreadProgressBar.Refresh();

            //-------------------------------------------------------
            string text = fileRenameTxtBx.Text;                                                                                                     //get the user inputted text
            int length = 0;                                                                                                                         //store the amount of files in the folder selected as 'length'
            if (textAfterNumTxtBx.Text == "Text after number (optional)") { textAfterNumTxtBx.Text = ""; }                                          //if the 'text after number' feild has not been filled; set it to nothing (so that it doesn't affect the process)

            bool valid = true;
            string[] invalidN = new string[0];
            string[] invalidA = new string[0];
            string[] invalidE = new string[0];

            foreach (char invalidC in Path.GetInvalidFileNameChars())
            {
                if (text.Contains(invalidC.ToString()))
                {
                    valid = false;
                    Array.Resize(ref invalidN, invalidN.Length + 1);
                    invalidN[invalidN.Length - 1] = invalidC.ToString();
                    
                }

                if (textAfterNumTxtBx.Text.Contains(invalidC.ToString()))
                {
                    valid = false;
                    Array.Resize(ref invalidA, invalidA.Length + 1);
                    invalidA[invalidA.Length - 1] = invalidC.ToString();
                    
                }

                if (("." + extentionTxtBx.Text).Contains(invalidC.ToString()))
                {
                    valid = false;
                    Array.Resize(ref invalidE, invalidE.Length + 1);
                    invalidE[invalidE.Length - 1] = invalidC.ToString();

                }

            }

            if (textAfterNumTxtBx.Text.Contains(".")) {
                valid = false;
                Array.Resize(ref invalidA, invalidA.Length + 1);
                invalidA[invalidA.Length - 1] = ".";

            }

            if (extentionTxtBx.Text.Contains("."))
            {
                valid = false;
                Array.Resize(ref invalidE, invalidE.Length + 1);
                invalidE[invalidE.Length - 1] = ".";

            }

            if (!valid)
            {
                string errmsg = "Please amend these issues to start the renaming process:\n\n";
                if (invalidN.Length > 0)
                {
                    errmsg += "The name to rename the files to contains at least one illegal character: \"";

                    foreach (string invalidC in invalidN)
                    {
                        if (invalidN.Length > 1 && invalidC != invalidN[0] && invalidC != invalidN[invalidN.Length-1]) {//if there is at least 2 invalid characters and we're not on the first invalid character nor the last
                            errmsg += "\", \"";
                        }

                        if (invalidN.Length > 1 && invalidC == invalidN[invalidN.Length-1]) {//otherwise if we're on the second last invald character
                            errmsg += "\" and \"";

                        }

                        errmsg += invalidC;
                    }

                    errmsg += "\".";
                    if (invalidA.Length > 0 || invalidE.Length > 0)
                    {
                        errmsg += "\n";
                    }

                }

                if (invalidA.Length > 0)
                {
                    errmsg += "The text to include after the counter contains at least one illegal character: \"";

                    foreach (string invalidC in invalidA)
                    {
                        if (invalidA.Length > 1 && invalidC != invalidA[0] && invalidC != invalidA[invalidA.Length - 1])
                        {//if there is at least 2 invalid characters and we're not on the first invalid character nor the last
                            errmsg += "\", \"";
                        }

                        if (invalidA.Length > 1 && invalidC == invalidA[invalidA.Length - 1])
                        {//otherwise if we're on the second last invald character
                            errmsg += "\" and \"";

                        }

                        errmsg += invalidC;
                    }

                    errmsg += "\".";
                    if (invalidE.Length > 0)
                    {
                        errmsg += "\n";
                    }

                }

                if (invalidE.Length > 0)
                {
                    errmsg += "The extention to change to contains at least one illegal character: \"";

                    foreach (string invalidC in invalidE)
                    {
                        if (invalidE.Length > 1 && invalidC != invalidE[0] && invalidC != invalidE[invalidE.Length - 1])
                        {//if there is at least 2 invalid characters and we're not on the first invalid character nor the last
                            errmsg += "\", \"";
                        }

                        if (invalidE.Length > 1 && invalidC == invalidE[invalidE.Length - 1])
                        {//otherwise if we're on the second last invald character
                            errmsg += "\" and \"";

                        }

                        errmsg += invalidC;
                    }

                    errmsg += "\".";
                }

                MessageBox.Show(errmsg, "Error: Invalid character", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                BtnMassRename.Enabled = true;
                PreferencesTSMI.Enabled = true;

            }
            else
            {
                if (fsd.ShowDialog(IntPtr.Zero))                                                                                                        //show the file selector dialog box (order of presedence has that message boxes show first)
                {                                                                                                                                       //Tif the dialog box has not returned an error or has closed (0)
                    DialogResult result = DialogResult.Cancel;                                                                                          //|--set the default response from the dialog boxes to "Cancel"
                    if (!DW)                                                                                                                            //|
                    {                                                                                                                                   //|-Tif the warnings are NOT disabled
                        result = MessageBox.Show("Are you sure? ALL FILES IN THIS FOLDER WILL BE RENAMED, " +                                           //| |
                            "THIS CAN'T BE UNDON - NOT EVEN WITH CTRL+Z IN THE EXPLORER!", "Filepath: " + fsd.FileName, MessageBoxButtons.OKCancel,     //| |
                            MessageBoxIcon.Warning);                                                                                                    //|-|---warn the user of the danger (message box)
                    }
                    else { result = DialogResult.OK; }                                                                                                //|-\c)e)otherwise set the result to "OK"                           
                                                                                                                                                      //|
                    string[] AFa = new string[0];                                                                                                       //|--prepare an array version AF for the randomize selection type
                                                                                                                                                        //|
                    if (result == DialogResult.OK)                                                                                                      //|
                    {                                                                                                                                   //|-Tif the result is "OK"
                        DirectoryInfo di = new DirectoryInfo(fsd.FileName);                                                                             //|-|----get the selected directory (folder)
                        length = di.GetFiles().Length;                                                                                                  //|-|----get how many files are in the folder
                        IEnumerable<string> AF = Splitter.CustomSort(di.EnumerateFiles().Select(f => f.Name));                                          //|-|----get all files in the folder, sort by name (alphanumerically, not alphabraically)
                                                                                                                                                        //| |
                        if (alphabraicSTBtn.Checked)
                        {                                                                                                  //|-|---Tif the user has defined to sort aphabraically
                            AF = di.EnumerateFiles().Select(f => f.Name);                                                                               //|-|---|----sort
                                                                                                                                                        //| |   |
                        }
                        else if (regexSTBtn.Checked)
                        {                                                                                                //|-|---\c)if the user has defined to sort by a regular expression
                                                                                                                         //WIP  //| |   |
                                                                                                                         //| |   |
                        }
                        else if (randomSTBtn.Checked)
                        {                                                                                               //| |   \c)if the user has defined to sort randomly
                            Random ran = new Random();                                                                                                     //|-|---|----generate a random...thing
                            AFa = di.EnumerateFiles().Select(f => f.Name).OrderBy(f => ran.Next()).ToArray();                                           //|-|---|----randomise the order
                                                                                                                                                        //| |   |
                        }                                                                                                                               //|-|---|e)
                                                                                                                                                        //| |
                        string firstFileName = "";                                                                                                      //|-|----prepare to get first file's name
                                                                                                                                                        //| |
                        if (!randomSTBtn.Checked)                                                                                                       //| |
                        {                                                                                                                               //|-|---Tif the user has not selected to randomise the selection
                            firstFileName = AF.FirstOrDefault();                                                                                        //|-|---|----get the first file's name (doesn't include directory)
                            result = MessageBox.Show("Is " + firstFileName +                                                                            //| |   |
                                " the first file in the folder and/or you DONT need to select a custom set of files?", "Filepath: "                     //| |   |
                                + fsd.FileName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);                                                      //|-|---|----verify the target file is the first file of the target file order (message box)
                        }
                        else
                        {                                                                                                                        //|-|---\c)
                            result = MessageBox.Show(" Do you want to NOT select a custom set of files?", "Filepath: " + fsd.FileName,                  //| |   |
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question);                                                                      //|-|---|----verify if the user wants to select multiple files, but not every file of the folder (message box)
                        }                                                                                                                               //|-|---|e)
                                                                                                                                                        //| |
                        if (result == DialogResult.Yes)                                                                                                 //| |
                        {                                                                                                                               //|-|---Tif the dialog box returns "Yes" of target file check
                            bool testValid = true;
                            foreach (string testFile in di.EnumerateFiles().Select(f => f.Name))
                            {
                                if (testFile.StartsWith("TMPRN"))
                                {
                                    testValid = false;
                                }
                            }

                            if (testValid)
                            {

                                if (!DW)                                                                                                                    //| |   |
                                {                                                                                                                           //|-|---|---Tif the warnings are NOT disabled
                                    result = MessageBox.Show("Are you ABSOLUTELY sure?", "Last chance bukko!", MessageBoxButtons.YesNo,                     //| |   |   |
                                    MessageBoxIcon.Question);                                                                                               //|-|---|---|----Preforme a final chech with the user (message box)
                                }
                                else { result = DialogResult.Yes; }                                                                                       //|-|---|---\c)e)otherwise set the result to "Yes"
                                                                                                                                                          //| |   |
                                if (result == DialogResult.Yes)                                                                                             //| |   |
                                {                                                                                                                           //|-|---|---Tif the dialog box returns "Yes" again of final chech
                                                                                                                                                            //| |   |   |
                                    if (!randomSTBtn.Checked)                                                                                               //| |   |   |
                                    {                                                                                                                       //|-|---|---|---Tif not randomizing

                                        progress progressBar = new progress();
                                        Form isOpen = Application.OpenForms["progress"];//get if the window is still open

                                        if (isOpen != null) { isOpen.Close(); }//if it is then close it

                                        progressBar.Show();
                                        progressBar.progressBar1.Maximum = processorCount;
                                        progressBar.Text = "Getting files to rename...";

                                        split = int.Parse(Math.Ceiling((double)length / processorCount).ToString());//get the amount of files each thread will operate on (rounded up)
                                        int fc = 0;//prepare to cound the amount of file names written to the ref files
                                        StreamWriter[] stream = new StreamWriter[processorCount];//prepare to write to files

                                        threadProgress = new string[processorCount * split];
                                        for (uint pi = 0; pi < processorCount * split; pi++)
                                        {
                                            threadProgress[pi] = "";
                                        }

                                        //create ref files
                                        for (int i = 1; i <= processorCount; i++)
                                        {
                                            stream[i - 1] = File.CreateText(path + @"\ref" + i + ".txt");//create a reference file for the thread to reffer to when getting which files to rename
                                            stream[i - 1].AutoFlush = true;//write buffer to file after each WriteLine() function call

                                            foreach (string file in AF.Skip(split * (i - 1)))//get each file, skipping the files that area already delegated
                                            {
                                                stream[i - 1].WriteLine(file);//write the reference file to the file 
                                                fc++;//count the amount of files written in
                                                progressBar.Text = file + " => " + "ref" + i + @"\" + processorCount;

                                                if (fc >= split)
                                                {//if all files delegated
                                                    break;//stop delegating files
                                                }

                                            }
                                            stream[i - 1].Close();//close the stream
                                            progressBar.progressBar1.Value = i;
                                            fc = 0;//reset the var
                                        }

                                        isOpen = Application.OpenForms["progress"];//get if the window is still open
                                        if (isOpen != null)
                                        { isOpen.Close(); }

                                        //Test(fsd, openFileDialog1);
                                        ThreadProgressBar.Maximum = processorCount * split;
                                        ThreadProgressBar.Visible = true;
                                        selectedPath = fsd.FileName;
                                        ThreadProgressBar.Maximum = AF.Count();
                                        backgroundThread.RunWorkerAsync();//start multi-threading

                                    }
                                    else
                                    {                                                                                                                //| |   |   |   \c)otherwise if randomised
                                        progress progressBar = new progress();
                                        Form isOpen = Application.OpenForms["progress"];//get if the window is still open

                                        if (isOpen != null) { isOpen.Close(); }//if it is then close it

                                        progressBar.Show();
                                        progressBar.progressBar1.Maximum = processorCount;
                                        progressBar.Text = "Getting files to rename...";

                                        split = int.Parse(Math.Ceiling((double)length / processorCount).ToString());//get the amount of files each thread will operate on (rounded up)
                                        int fc = 0;//prepare to cound the amount of file names written to the ref files
                                        StreamWriter[] stream = new StreamWriter[processorCount];//prepare to write to files

                                        threadProgress = new string[processorCount * split];

                                        for (uint pi = 0; pi < processorCount * split; pi++)
                                        {
                                            threadProgress[pi] = "";
                                        }

                                        for (int i = 1; i <= processorCount; i++)
                                        {
                                            stream[i - 1] = File.CreateText(path + @"\ref" + i + ".txt");//create a reference file for the thread to reffer to when getting which files to rename
                                            stream[i - 1].AutoFlush = true;//write buffer to file after each WriteLine() function call

                                            foreach (string file in AFa.Skip(split * (i - 1)))                                                               //get each file, skipping the files that area already delegated
                                            {
                                                stream[i - 1].WriteLine(file);//write the reference file to the file 
                                                fc++;//count the amount of files written in
                                                progressBar.Text = file + " => " + "ref" + i + @"\" + processorCount;

                                                if (fc >= split)
                                                {//if all files delegated
                                                    break;//stop delegating files
                                                }

                                            }
                                            stream[i - 1].Close();//close the stream
                                            progressBar.progressBar1.Value = i;

                                            fc = 0;//reset the var
                                        }

                                        isOpen = Application.OpenForms["progress"];//get if the window is still open
                                        if (isOpen != null)
                                        { isOpen.Close(); }

                                        ThreadProgressBar.Maximum = processorCount * split;
                                        ThreadProgressBar.Visible = true;
                                        selectedPath = fsd.FileName;
                                        ThreadProgressBar.Maximum = AFa.Count();
                                        backgroundThread.RunWorkerAsync();

                                    }                                                                                                                       //| |   |   |   |e)
                                    if (RTC == 0) { RTC = 1; }                                                                                              //|-|---|---|----Report succesfull run
                                                                                                                                                            //| |   |   |
                                }                                                                                                                           //|-|---|---|e)if the dialog box returns "No" of final chech; END.
                                                                                                                                                            //| |   |
                            }
                            else {
                                MessageBox.Show("One or more files start with \"TMPRN\" which is a reserved name, please rename these files to anything else.");
                                BtnMassRename.Enabled = true;
                                PreferencesTSMI.Enabled = true;
                                active = false;
                                textAfterNumTxtBx.Text = "Text after number (optional)";
                                i = 0;
                            }
                        }
                        else
                        {                                                                                                                        //|-|---\c)if the dialog box returns "No" of target file check
                            if (!DW)                                                                                                                    //| |   |
                            {                                                                                                                           //|-|---|---Tif the warnings are NOT disabled
                                MessageBox.Show("To sort the files selected, right click on the dialog and use the 'sort by' option",                   //| |   |   |
                                        "Reminder!");                                                                                                   //|-|---|---|----Remind the user how to sort in this circumstance
                            }                                                                                                                           //| |   |   |e)
                                                                                                                                                        //| |   |
                            openFileDialog1.InitialDirectory = fsd.FileName;                                                                            //|-|---|----open the same directory
                            DialogResult dlgr = openFileDialog1.ShowDialog();                                                                           //|-|---|----show the file selecter dialog
                            openFileDialog1.Title = "Select all files to rename";                                                                       //| |   |----change the title of the dialog to signify multi-select is active
                            if (dlgr == DialogResult.OK)                                                                                                //| |   |      
                            {                                                                                                                           //|-|---|---Tif the return from the dialog is "OK" of the file selecter
                                firstFileName = null;                                                                                                   //|-|---|---|----reset the first gotten file in case the files have changed or a new folder is selected
                                                                                                                                                        //| |   |   |
                                while (firstFileName == null)                                                                                           //| |   |   |    
                                {                                                                                                                       //|-|---|---|---Twhile the first file is not gotten
                                    firstFileName = openFileDialog1.FileName;                                                                           //|-|---|---|---|----wait untill the dialog box is closed
                                                                                                                                                        //| |   |   |   |    note: unlike the above method, this returns the full directory
                                    if (firstFileName == null)                                                                                          //| |   |   |   |    
                                    {                                                                                                                   //|-|---|---|---|---Tif the first file is still not avalible
                                        DialogResult bgr = MessageBox.Show("could not get file, try again...", "Could not find the selected file!",     //| |   |   |   |   |
                                            MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);                                                    //|-|---|---|---|---|----tell the user the file might not exist (message box)
                                                                                                                                                        //| |   |   |   |   |
                                        if (bgr == DialogResult.Cancel)                                                                                 //| |   |   |   |   |
                                        {                                                                                                               //|-|---|---|---|---|---Tif the user decides it's not going to work (presses cancel)
                                            BtnMassRename.Enabled = true;
                                            PreferencesTSMI.Enabled = true;
                                            Close();                                                                                                    //|-|---|---|---|---|---|----brake out of the loop
                                                                                                                                                        //| |   |   |   |   |   |
                                        }                                                                                                               //|-|---|---|---|---|---|e)
                                                                                                                                                        //| |   |   |   |   |
                                    }                                                                                                                   //|-|---|---|---|---|e)force loop as file is still invalid
                                                                                                                                                        //| |   |   |   |    
                                }                                                                                                                       //|-|---|---|---|W)file found
                                                                                                                                                        //| |   |   |
                                bool testValid = true;
                                foreach (string testFile in openFileDialog1.SafeFileNames)
                                {
                                    if (testFile.StartsWith("TMPRN"))
                                    {
                                        testValid = false;
                                    }
                                }

                                if (testValid)
                                {
                                    if (!DW)                                                                                                                //| |   |   |
                                    {                                                                                                                       //|-|---|---|---Tif the warnings are NOT disabled
                                        result = MessageBox.Show("Are you ABSOLUTELY sure?", "Last chance bukko!", MessageBoxButtons.YesNo,                 //| |   |   |   |
                                        MessageBoxIcon.Question);                                                                                           //|-|---|---|---|----Preforme a final chech with the user (message box)
                                    }
                                    else { result = DialogResult.Yes; }                                                                                   //|-|---|---|---\c)e)otherwise set the result to "Yes"
                                                                                                                                                          //| |   |   |              
                                    if (result == DialogResult.Yes)                                                                                         //| |   |   |
                                    {                                                                                                                       //|-|---|---|---Tif the dialog box returns "Yes" of the final check

                                        length = openFileDialog1.FileNames.Length;                                                                          //|-|---|---|---|----reset 'length' to the amount of files selected
                                                                                                                                                            //| |   |   |   |          
                                        if (!randomSTBtn.Checked)                                                                                               //| |   |   |
                                        {                                                                                                                       //|-|---|---|---Tif not randomizing
                                            progress progressBar = new progress();
                                            Form isOpen = Application.OpenForms["progress"];//get if the window is still open

                                            if (isOpen != null) { isOpen.Close(); }//if it is then close it

                                            progressBar.Show();
                                            progressBar.progressBar1.Maximum = processorCount;
                                            progressBar.Text = "Getting files to rename...";

                                            split = int.Parse(Math.Ceiling((double)length / processorCount).ToString());//get the amount of files each thread will operate on (rounded up)
                                            int fc = 0;//prepare to cound the amount of file names written to the ref files
                                            StreamWriter[] stream = new StreamWriter[processorCount];//prepare to write to files

                                            threadProgress = new string[processorCount * split];

                                            for (uint pi = 0; pi < processorCount; pi++)
                                            {
                                                threadProgress[pi] = "";
                                            }

                                            for (int i = 1; i <= processorCount; i++)
                                            {
                                                stream[i - 1] = File.CreateText(path + @"\ref" + i + ".txt");//create a reference file for the thread to reffer to when getting which files to rename
                                                stream[i - 1].AutoFlush = true;//write buffer to file after each WriteLine() function call

                                                foreach (string file in openFileDialog1.SafeFileNames.Skip(split * (i - 1)))                                                               //get each file, skipping the files that area already delegated
                                                {
                                                    stream[i - 1].WriteLine(file);//write the reference file to the file 
                                                    fc++;//count the amount of files written in
                                                    progressBar.Text = file + " => " + "ref" + i + @"\" + processorCount;

                                                    if (fc >= split)
                                                    {//if all files delegated
                                                        break;//stop delegating files
                                                    }

                                                }
                                                stream[i - 1].Close();//close the stream
                                                progressBar.progressBar1.Value = i;

                                                fc = 0;//reset the var
                                            }

                                            isOpen = Application.OpenForms["progress"];//get if the window is still open
                                            if (isOpen != null)
                                            { isOpen.Close(); }

                                            ThreadProgressBar.Maximum = processorCount * split;
                                            ThreadProgressBar.Visible = true;
                                            selectedPath = firstFileName.Replace(openFileDialog1.SafeFileName, "");
                                            selectedPath = selectedPath.Remove(selectedPath.Length - 1);
                                            ThreadProgressBar.Maximum = openFileDialog1.SafeFileNames.Count();
                                            backgroundThread.RunWorkerAsync();//start multi-threading
                                        }
                                        else
                                        {                                                                                                                //| |   |   |   \c)otherwise if randomised

                                            Random ran = new Random();                                                                                                     //|-|---|----generate a random...thing
                                                                                                                                                                           //AFa = di.EnumerateFiles().Select(f => f.Name).OrderBy(f => ran.Next()).ToArray();                                           //|-|---|----randomise the order
                                            AFa = openFileDialog1.SafeFileNames.Select(f => f).OrderBy(f => ran.Next()).ToArray();                                           //|-|---|----randomise the order

                                            progress progressBar = new progress();
                                            Form isOpen = Application.OpenForms["progress"];//get if the window is still open

                                            if (isOpen != null) { isOpen.Close(); }//if it is then close it

                                            progressBar.Show();
                                            progressBar.progressBar1.Maximum = processorCount;
                                            progressBar.Text = "Getting files to rename...";

                                            split = int.Parse(Math.Ceiling((double)length / processorCount).ToString());//get the amount of files each thread will operate on (rounded up)
                                            int fc = 0;//prepare to cound the amount of file names written to the ref files
                                            StreamWriter[] stream = new StreamWriter[processorCount];//prepare to write to files

                                            threadProgress = new string[processorCount * split];

                                            for (uint pi = 0; pi < processorCount; pi++)
                                            {
                                                threadProgress[pi] = "";
                                            }

                                            for (int i = 1; i <= processorCount; i++)
                                            {
                                                stream[i - 1] = File.CreateText(path + @"\ref" + i + ".txt");//create a reference file for the thread to reffer to when getting which files to rename
                                                stream[i - 1].AutoFlush = true;//write buffer to file after each WriteLine() function call

                                                foreach (string file in AFa.Skip(split * (i - 1)))                                                               //get each file, skipping the files that area already delegated
                                                {
                                                    stream[i - 1].WriteLine(file);//write the reference file to the file 
                                                    fc++;//count the amount of files written in
                                                    progressBar.Text = file + " => " + "ref" + i + @"\" + processorCount;

                                                    if (fc >= split)
                                                    {//if all files delegated
                                                        break;//stop delegating files
                                                    }

                                                }
                                                stream[i - 1].Close();//close the stream
                                                progressBar.progressBar1.Value = i;

                                                fc = 0;//reset the var
                                            }

                                            isOpen = Application.OpenForms["progress"];//get if the window is still open
                                            if (isOpen != null)
                                            { isOpen.Close(); }

                                            ThreadProgressBar.Maximum = processorCount * split;
                                            ThreadProgressBar.Visible = true;
                                            selectedPath = firstFileName.Replace(openFileDialog1.SafeFileName, "");
                                            selectedPath = selectedPath.Remove(selectedPath.Length - 1);
                                            ThreadProgressBar.Maximum = AFa.Count();
                                            backgroundThread.RunWorkerAsync();

                                        }                                                                                                                       //| |   |   |   |e)
                                                                                                                                                                //| |   |   |   |
                                        if (RTC == 0) { RTC = 1; }                                                                                          //|-|---|---|---|----Report succesfull run
                                                                                                                                                            //| |   |   |   |
                                    }                                                                                                                       //|-|---|---|---|e)
                                }
                                else {
                                    MessageBox.Show("One or more files start with \"TMPRN\" which is a reserved name, please rename these files to anything else.");
                                    BtnMassRename.Enabled = true;
                                    PreferencesTSMI.Enabled = true;
                                    active = false;
                                    textAfterNumTxtBx.Text = "Text after number (optional)";
                                    i = 0;
                                }
                            }
                            else
                            {                                                                                                                    //|-|---|---\c)if the user cancled file selection dialog (mutli-select)
                                RTC = 2;                                                                                                                //|-|---|---|----Report a minor error occured
                                BtnMassRename.Enabled = true;
                                PreferencesTSMI.Enabled = true;
                                MessageBox.Show("An error occured or the process was aborted!");                                                        //|-|---|---|----Tell them the process failed
                                                                                                                                                        //|-|---|---|
                            }                                                                                                                           //|-|---|---|e)
                        }                                                                                                                               //|-|---|e)
                    }                                                                                                                                   //|-|e)
                }
                else
                {                                                                                                                                //\c)if the user cancled file selection dialog (single select)
                    RTC = 2;                                                                                                                            //|--Report a minor error occured
                    BtnMassRename.Enabled = true;
                    PreferencesTSMI.Enabled = true;
                    MessageBox.Show("An error occured or the process was aborted!");                                                                    //|--Tell them the process failed
                                                                                                                                                        //|
                }                                                                                                                                       //|e)
            }
                                                                                                                                                    //
                                                                                                                                                    //
            //----------------------------

        }

        //handle creating the new threads to run and runs holds this thread, stopping the UI from being frozen
        private void backgroundThread_DoWork(object sender, DoWorkEventArgs e)
        {
            file = new FileStream[processorCount];
            ThreadClaimed = new int[processorCount];
            msg = false;

            workerBlock = new ActionBlock<FolderSelectDialog>(//input type for action/function is the amount of ref files (processorCount)
                                                              // Simulate work by suspending the current thread.
            refCount => Thread(fsd, openFileDialog1),
            // Specify a maximum degree of parallelism.
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = processorCount
            });//set number as the amount of processors-1

            for (int i = 0; i < processorCount; i++)
            {
                workerBlock.Post(fsd);
            }
            workerBlock.Complete();

            workerBlock.Completion.Wait();//wait until all threads complete

            //remove the ref files
            for (int i = 1; i <= processorCount; i++)
            {
                if (file[i - 1] != null)
                {
                    file[i - 1].Close();
                    while (true)
                    {
                        try
                        {
                            File.Delete(path + @"\ref" + i + ".txt");//delete the ref files
                            File.Delete(path + @"\ref" + i + "Claimed");//delete the claim files
                            break;
                        }
                        catch
                        {
                            file[i - 1].Close();
                            System.Threading.Thread.Sleep(500);
                        }
                    }

                }
                else
                {
                    File.Delete(path + @"\ref" + i + ".txt");//delete the ref files
                    File.Delete(path + @"\ref" + i + "Claimed");//delete the claim files

                }
            }

            if (workerBlock.Completion.Exception == null)
            {
                errorLogger.QueueLog("", "", 4, true);
                errorLogger.QueueLog("All threads compeleted successfully without any exceptions!");
                errorLogger.QueueLog("", "", 4, true);

            }

            workerBlock.Completion.Dispose();//release the threads
            Array.Resize(ref file, 0);//release the file streams
            errorLogger.ReleaseQueue(path + @"\log.txt");//release the logs 
            //active = false;//set active to false

        }

        //the set of instructions each thread will run (the renmaing process)
        private void Thread(FolderSelectDialog fsd, OpenFileDialog openFileDialog1)
        {
            ErrorLogger threadErrorLogger = new ErrorLogger();
            threadErrorLogger.QueueLog("", "", 4, true);
            threadErrorLogger.QueueLog("INFO: Running thread " + System.Threading.Thread.CurrentThread.ManagedThreadId);

            //System.Threading.Thread.Sleep(sleepTimer);//wait 10 * random miliseconds so that each thread starts at a different point (wont try to access the same file all at the same time)
            if (System.Threading.Thread.CurrentThread.IsBackground)
            {
                Monitor.Enter(claimLocker);

            }
            int claimed = -1;
            int breakout = 1000 * processorCount;

            if (processorCount > 1)
            {
                while (claimed == -1)
                {
                    for (int i = 0; i < processorCount; i++)
                    {

                        if (!File.Exists(path + @"\ref" + (i + 1) + "Claimed") && file[i] == null)
                        {

                            if (!threadErrorLogger.IsFileLocked(new FileInfo(path + @"\ref" + (i + 1) + ".txt")))
                            {
                                try
                                {
                                    if (file[i] == null)
                                    {
                                        File.Create(path + @"\ref" + (i + 1) + "Claimed").Close();
                                        using (file[i] = new FileInfo(path + @"\ref" + (i + 1) + ".txt").OpenRead()) { }//try to open a read, if fails then dont set file[i] as object
                                        claimed = i + 1;
                                        Debug.WriteLine("Thread " + System.Threading.Thread.CurrentThread.ManagedThreadId + " claimed ref" + claimed + ".");
                                        break;
                                    }
                                    else
                                    {
                                        Debug.WriteLine("Thread " + System.Threading.Thread.CurrentThread.ManagedThreadId + " failed to claim ref" + claimed + " as the file was not null.");
                                        claimed = -1;
                                    }
                                }
                                catch
                                {
                                    if (file[i] != null)
                                    {
                                        file[i].Close();
                                    }
                                    file[i] = null;
                                    claimed = -1;
                                    Debug.WriteLine("Thread " + System.Threading.Thread.CurrentThread.ManagedThreadId + " failed to claim ref" + i + " as the file was in use or no longer existed.");

                                }
                            }
                            else
                            {
                                Debug.WriteLine("Thread " + System.Threading.Thread.CurrentThread.ManagedThreadId + " failed to claimed ref" + (i + 1) + " as the file was in use.");
                                System.Threading.Thread.Sleep(100);
                                i--;
                            }

                        }
                    }

                    if (breakout == 0)
                    {
                        Debug.WriteLine("CTITICAL ERROR OCCURED - CLAIMING BREAKOUT");
                        MessageBox.Show("A critical error occured during multi-threading setup and will be aborted to preserve data integrity, a log has been saved to:\n"
                            + path + @"\log.txt\nPlease report the log to the issues page on the branches' 'issues' page.", "Critical error: CLAIMING BREAKOUT"
                            , MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        threadErrorLogger.QueueLog("ERROR: A critical error occured during multi-threading setup - CLAIMING BREAKOUT.");
                        break;
                    }
                    else
                    {
                        breakout--;
                    }

                }

                Debug.WriteLine("Thread " + System.Threading.Thread.CurrentThread.ManagedThreadId + " claimed ref" + claimed + ".");
                threadErrorLogger.QueueLog("Thread " + System.Threading.Thread.CurrentThread.ManagedThreadId + " claimed ref" + claimed + ".");
                ThreadClaimed[claimed - 1] = System.Threading.Thread.CurrentThread.ManagedThreadId;//get which thread claimed which ref

                //System.Threading.Thread.Sleep(300);//wait for all threads to finish claiming
                Monitor.Exit(claimLocker);
                reSync[claimed - 1] = true;
                SpinWait.SpinUntil(delegate () { return reSync.All(item => item.Equals(true)); });//wait untill all threads reach this point

                //Monitor.Wait(claimLocker2);
                int ci = 0;

                //for debugging
                //if (file[0] != null)
                //{
                //    file[0].Close();
                //    file[0] = null;
                //}

                if (!msg)
                {
                    foreach (FileStream state in file)
                    {
                        if (state == null && ci != claimed - 1)
                        {
                            Debug.WriteLine("CTITICAL ERROR OCCURED - NULL CLAIMING");
                            threadErrorLogger.QueueLog("ERROR: A critical error occured during multi-threading setup - NULL CLAIMING. A ref file was not claimed.");
                            threadErrorLogger.ReleaseQueue(path + @"\log.txt");

                            MessageBox.Show("A critical error occured during multi-threading setup and will be aborted to preserve data integrity, a log has been saved to:\n"
                        + path + @"\log.txt"
                        + "\nPlease report the log to the issues page on the relevent branch. (" + this.Name + ")", "Critical error: NULL FILE REF"
                        , MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            ci = -1;
                            msg = true;
                            break;

                        }
                        ci++;

                    }
                }

                if (ci == -1)
                {
                    if (file[claimed - 1] == null)
                    {
                        Debug.WriteLine("CTITICAL ERROR OCCURED - NULL CLAIMING");
                        threadErrorLogger.QueueLog("ERROR: A critical error occured during multi-threading setup - NULL CLAIMING. Thread "
                            + System.Threading.Thread.CurrentThread.ManagedThreadId + " failed to claim ref" + (claimed - 1));

                    }
                }
                else
                {//if no other thread has failed to claim a file

                    if (file[claimed - 1] == null)
                    {
                        Debug.WriteLine("CTITICAL ERROR OCCURED - NULL CLAIMING");
                        threadErrorLogger.QueueLog("ERROR: A critical error occured during multi-threading setup - NULL CLAIMING. Thread "
                            + System.Threading.Thread.CurrentThread.ManagedThreadId + " failed to claim ref" + (claimed - 1));

                    }
                    else
                    {//and this thread has not failed to claim a file
                        int sleepTimer = 100 * claimed;
                        System.Threading.Thread.Sleep(sleepTimer);//wait 10 * random miliseconds so that each thread starts at a different point (wont try to access the same file all at the same time)
                        IEnumerable<string> addT = null;
                        string text = fileRenameTxtBx.Text;                                                           //get the user inputted text
                        string ext = "";
                        StreamReader stream = File.OpenText(file[claimed - 1].Name);                                      //- open the text file's stream
                        string[] tmpa = new string[split];
                        int ai = 0;
                        int li = 0;

                        while (!stream.EndOfStream)
                        {
                            tmpa[ai] = stream.ReadLine();
                            ai++;

                        }
                        stream.Close();
                        file[claimed - 1].Close();

                        if (alphabeticalTSMI.Checked)                                                                                       //|-|---|---|---|---Tif the user has defined to use alphabetical ordering type
                        {
                            int splitChar = int.Parse("A") + ((claimed * split) - 1);
                            addT = Splitter.GetAdditionType(0, splitChar.ToString(), (claimed - 1) * split);

                        }                                                                        //|-|---|---|---|---|----get the addition type of 0 (aplhab) with a limit to character "Z"
                        else if (numericaldTSMI.Checked)                                                                                    //|-|---|---|---|---\c)otherwise if the user had defined to use alphanumerical ordering type
                        { addT = Splitter.GetAdditionType(1, (split * claimed).ToString(), ((claimed - 1) * split) + 1); }                                                          //|-|---|---|---|---|----get the addition type of 0 (aplhan) with a limit of the same length as the amount of items selected
                        else if (customTSMI.Checked)                                                                                        //|-|---|---|---|---\c)otherwise if the user had defined to use an custom ordering type
                        { }                                                                                                                 //|-|---|---|---|---|e)--WIP

                        bool failedClaim = false;
                        foreach (FileStream state in file)
                        {
                            if (state == null)
                            {
                                failedClaim = true;
                            }

                        }

                        if (!failedClaim)
                        {//tripple check all files where claimed correctly
                            foreach (string add in addT)
                            {                                                                                  //|-|---|---|---|---|---Tnew foreach loop where the enumerable is enumerated and the resultant variable is "add"
                                                                                                               //| |   |   |   |   |   |
                                string firstFileName = tmpa[li];                                                               //|-|---|---|---|---|---|----get the selected file
                                if (firstFileName == null)
                                {
                                    break;
                                }
                                string test = firstFileName.Split(Convert.ToChar(92)).Last();

                                if (fileRenameTxtBx.Text.Length != 0 && (fileRenameTxtBx.Text == "What to rename the file to"               //| |   |   |   |   |   |
                                && fileRenameTxtBx.Text == "You may not rename files while sorting is disabled"))                           //| |   |   |   |   |   |
                                {                                                                                                           //|-|---|---|---|---|---|---Tif the user has NOT defined an new name for the files to be renamed to
                                    text = firstFileName;                                                                                   //|-|---|---|---|---|---|---|----set name to rename to, to its own name
                                }                                                                                                           //|-|---|---|---|---|---|---|e)
                                                                                                                                            //| |   |   |   |   |   |
                                if (extentionTxtBx.Text.Length > 0 && (extentionTxtBx.Text != "Extention (optional)"                        //| |   |   |   |   |   |
                                && extentionTxtBx.Text != "Extention"))                                                                     //| |   |   |   |   |   |
                                {                                                                                                           //|-|---|---|---|---|---|---Tif the user has defined an extention
                                    ext = "." + extentionTxtBx.Text;                                                                        //|-|---|---|---|---|---|---|---set the extention to the user defined type
                                }
                                else
                                {                                                                                                    //|-|---|---|---|---|---|---\c)otherwise if non defined
                                    ext = Path.GetExtension(firstFileName);                                           //|-|---|---|---|---|---|---|---get the current files extention
                                }                                                                                                           //|-|---|---|---|---|---|---|e)

                                if (test != text + add + textAfterNumTxtBx.Text + ext)
                                {//if the item to rename isn't already in the correct possition

                                    //if a conflict occures,
                                    if (File.Exists(fsd.FileName + @"\" + text + add + textAfterNumTxtBx.Text + ext))
                                    {
                                        int tmpi = 0;//get the file's possition in the array

                                        foreach (string item in tmpa)
                                        {
                                            if (item == text + add + textAfterNumTxtBx.Text + ext)
                                            {
                                                break;
                                            }
                                            tmpi++;

                                        }

                                        if (tmpi == tmpa.Length)
                                        {
                                            try
                                            {
                                                if (System.Threading.Thread.CurrentThread.IsBackground)
                                                {
                                                    Monitor.Enter(locker);

                                                }

                                                bool tmp = File.Exists(fsd.FileName + @"\" + text + add + textAfterNumTxtBx.Text + ext);
                                                if (File.Exists(fsd.FileName + @"\" + text + add + textAfterNumTxtBx.Text + ext))
                                                {
                                                    Array.Resize(ref renamedFrom, renamedFrom.Length + 1);
                                                    Array.Resize(ref renamedTo, renamedTo.Length + 1);
                                                    renamedFrom[renamedFrom.Length - 1] = text + add + textAfterNumTxtBx.Text + ext;
                                                    renamedTo[renamedTo.Length - 1] = "TMPRN" + tmpc + ext;

                                                    Microsoft.VisualBasic.FileIO.FileSystem.RenameFile(fsd.FileName + @"\"
                                                        + text + add + textAfterNumTxtBx.Text + ext, "TMPRN" + tmpc + ext);                                                             //|-|---|---|---|---|---|----rename the file
                                                    threadErrorLogger.QueueLog("INFO: Temporarily renamed " + text + add + textAfterNumTxtBx.Text + ext + " to TMPRN" + tmpc + ext);
                                                    threadProgress[((processorCount * claimed) - 1) + li] = "TMPRN" + tmpc + ext;
                                                    //tmpa[tmpi] = "TMPRN" + tmpc + ext;
                                                    tmpc++;
                                                }
                                                System.Threading.Monitor.Exit(locker);
                                            }
                                            catch (Exception e)
                                            {
                                                if (e.InnerException is FileNotFoundException || e.HResult == -2147024894)
                                                {//if conflict has been self solved by the other thread then just continue on normally
                                                }
                                                else
                                                {
                                                    threadErrorLogger.QueueLog("CRIT: AN ERROR OCCURED DURING THE RENAMING PROCESS AND WAS ABORTED! " +
                                                    "FAILED TO FIND THE FILE TO TEMPORARILY RENAME. " + text.ToUpper() + add.ToUpper()
                                                    + textAfterNumTxtBx.Text.ToUpper() + ext.ToUpper() + " EXISTS BUT THE REFERENCE IS MISSING. " +
                                                    "ANOTHER THREAD MAY HAVE THE REFERENCE.\nEXITING!");
                                                    //errorLogger.AppendQueue(threadErrorLogger.GetQueue());
                                                    //errorLogger.ReleaseQueue(path + @"\log.txt", true, true);

                                                    //include opening "recover me" program here to clean up files and attempt to 'undo' the renaming process
                                                    //something like https://stackoverflow.com/questions/3173775/how-to-run-external-program-via-a-c-sharp-program
                                                    //Process.Start

                                                    Invoke(new Action(() =>
                                                    {
                                                        errorLogger.AppendQueue(threadErrorLogger.GetQueue());
                                                        errorLogger.ReleaseQueue(path + @"\log.txt", true, true);
                                                        Environment.Exit(Environment.ExitCode);
                                                    }));
                                                }
                                            }

                                        }
                                        else
                                        {
                                            if (System.Threading.Thread.CurrentThread.IsBackground)
                                            {
                                                Monitor.Enter(locker);

                                            }
                                            if (File.Exists(fsd.FileName + @"\" + text + add + textAfterNumTxtBx.Text + ext))
                                            {//check again if the file exists as the threads are being delayed by the monitor
                                                try
                                                {//try to rename the file
                                                    Microsoft.VisualBasic.FileIO.FileSystem.RenameFile(fsd.FileName + @"\"
                                                    + text + add + textAfterNumTxtBx.Text + ext, "TMPRN" + tmpc + ext);                                                             //|-|---|---|---|---|---|----rename the file
                                                    threadErrorLogger.QueueLog("INFO: Temporarily renamed " + text + add + textAfterNumTxtBx.Text + ext + " to TMPRN" + tmpc + ext);
                                                    threadProgress[((processorCount * claimed) - 1) + li] = "TMPRN" + tmpc + ext;
                                                    tmpa[tmpi] = "TMPRN" + tmpc + ext;
                                                    tmpc++;
                                                }
                                                catch (Exception e)
                                                {
                                                    if (e.InnerException is FileNotFoundException)
                                                    { }
                                                    else
                                                    {
                                                        threadErrorLogger.QueueLog("CRIT: AN UNKNOWN ERROR OCCURED DURING THE RENAMING PROCESS AND WAS ABORTED! MESSAGE: " + e.Message.ToUpper());
                                                        errorLogger.AppendQueue(threadErrorLogger.GetQueue());
                                                        errorLogger.ReleaseQueue(path + @"\log.txt", true, true);
                                                        //threadErrorLogger.ReleaseQueue(path + @"\log.txt", true, true);

                                                        //include opening "recover me" program here to clean up files and attempt to 'undo' the renaming process
                                                        //something like https://stackoverflow.com/questions/3173775/how-to-run-external-program-via-a-c-sharp-program
                                                        //Process.Start

                                                        Invoke(new Action(() =>
                                                        {
                                                            Environment.Exit(Environment.ExitCode);
                                                        }));
                                                    }

                                                }
                                            }
                                            Monitor.Exit(locker);
                                        }

                                    }

                                    bool specialRename = false;
                                    for (int i1 = 0; i1 < renamedFrom.Length; i1++)
                                    {//check through all special tmprn files
                                        if (renamedFrom[i1] == firstFileName)
                                        {//if this thread is renaming a file that was renamed to a special tmprn
                                            try
                                            {//rename it back to its original name
                                                Microsoft.VisualBasic.FileIO.FileSystem.RenameFile(fsd.FileName + @"\" + renamedTo[i1],                                           //| |   |   |   |   |   |
                                                text + add + textAfterNumTxtBx.Text + ext);                                                             //|-|---|---|---|---|---|----rename the file
                                                threadErrorLogger.QueueLog("INFO: " + renamedFrom[i1] + " no longer existed so instead " + renamedTo[i1]
                                                    + " was renamed to " + text + add + textAfterNumTxtBx.Text + ext);
                                                //Microsoft.VisualBasic.FileIO.FileSystem.RenameFile(fsd.FileName + @"\" + renamedTo[i1],
                                                //renamedFrom[i1]);
                                                //threadErrorLogger.QueueLog("INFO: " + renamedFrom[i1] + " no longer existed so instead " + renamedTo[i1]
                                                //    + " was renamed to " + renamedFrom[i1]);
                                                threadProgress[(split * (claimed - 1)) + li] = text + add + textAfterNumTxtBx.Text + ext;
                                                specialRename = true;
                                                break;
                                            }
                                            catch (Exception e)
                                            {
                                                threadErrorLogger.QueueLog("CRIT: AN UNKNOWN ERROR OCCURED DURING THE RENAMING PROCESS AND WAS ABORTED! MESSAGE: " + e.Message.ToUpper());
                                                bool tmptsst = File.Exists(fsd.FileName + @"\" + renamedTo[i1]);
                                                tmptsst = File.Exists(fsd.FileName + @"\" + text + add + textAfterNumTxtBx.Text + ext);
                                                errorLogger.AppendQueue(threadErrorLogger.GetQueue());
                                                errorLogger.ReleaseQueue(path + @"\log.txt", true, true);
                                                //threadErrorLogger.ReleaseQueue(path + @"\log.txt", true, true);

                                                //include opening "recover me" program here to clean up files and attempt to 'undo' the renaming process
                                                //something like https://stackoverflow.com/questions/3173775/how-to-run-external-program-via-a-c-sharp-program
                                                //Process.Start

                                                Invoke(new Action(() =>
                                                {
                                                    Environment.Exit(Environment.ExitCode);
                                                }));

                                            }


                                        }

                                    }

                                    if (!specialRename)
                                    {
                                        try
                                        {
                                            if (File.Exists(fsd.FileName + @"\" + firstFileName))
                                            {
                                                Microsoft.VisualBasic.FileIO.FileSystem.RenameFile(fsd.FileName + @"\" + firstFileName,                                           //| |   |   |   |   |   |
                                                text + add + textAfterNumTxtBx.Text + ext);                                                             //|-|---|---|---|---|---|----rename the file
                                                threadErrorLogger.QueueLog("INFO: Renamed " + firstFileName + " to " + text + add + textAfterNumTxtBx.Text + ext);
                                                threadProgress[(split * (claimed - 1)) + li] = text + add + textAfterNumTxtBx.Text + ext;
                                            }
                                            else
                                            {
                                                int dud = 0;
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            //if (e.InnerException is FileNotFoundException)
                                            //{
                                            //    Monitor.Enter(mainLocker);
                                            //    if (File.Exists(fsd.FileName + @"\" + firstFileName)) {
                                            //        Monitor.Exit(mainLocker);

                                            //    }
                                            //}
                                            threadErrorLogger.QueueLog("CRIT: AN UNKNOWN ERROR OCCURED DURING THE RENAMING PROCESS AND WAS ABORTED! MESSAGE: " + e.Message.ToUpper());
                                            errorLogger.ReleaseQueue(path + @"\log", true, true);
                                            threadErrorLogger.ReleaseQueue(path + @"\log.txt", true, true);

                                            //include opening "recover me" program here to clean up files and attempt to 'undo' the renaming process

                                            Invoke(new Action(() =>
                                            {
                                                Environment.Exit(Environment.ExitCode);
                                            }));

                                        }
                                    }

                                }
                                li++;                                                                                                        //|-|---|---|---|---|---|----count amount of times looped
                                //ThreadProgressBar.TabIndex = 19;
                                ThreadProgressBar_TabIndexChanged(null, EventArgs.Empty);

                                //| |   |   |   |   |   |
                            }                                                                                                               //|-|---|---|---|---|---|L)
                            threadErrorLogger.QueueLog("INFO: Thread " + System.Threading.Thread.CurrentThread.ManagedThreadId
                                + " has finished renaming its delegated files and is now waiting on the other thread to finish.");

                        }
                        else
                        {
                            Debug.WriteLine("CTITICAL ERROR OCCURED - NULL CLAIMING");
                            threadErrorLogger.QueueLog("CRIT: A critical error occured during multi-threading setup - NULL CLAIMING." +
                                "A ref file was not claimed but the process was stopped before the program started renaming.");

                            MessageBox.Show("A critical error occured during multi-threading setup and will be aborted to preserve data integrity, a log has been saved to:\n"
                                + path + @"\log.txt"
                                + "\nPlease report the log to the issues page on the relevent branch. (" + this.Name + ")", "Critical error: NULL FILE REF"
                                , MessageBoxButtons.OK, MessageBoxIcon.Stop);

                        }


                    }

                }


            }

            //threadErrorLogger.ReleaseQueue(path + @"\log.txt", false, true);
            errorLogger.AppendQueue(threadErrorLogger.GetQueue(), false);
            threadErrorLogger = null;

        }

        //when the renaming process is finished
        private void backgroundThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            System.Diagnostics.Process.Start(selectedPath);
            BtnMassRename.Enabled = true;
            PreferencesTSMI.Enabled = true;
            active = false;
            textAfterNumTxtBx.Text = "Text after number (optional)";
            i = 0;
            ThreadProgressBar.Visible = false;

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
