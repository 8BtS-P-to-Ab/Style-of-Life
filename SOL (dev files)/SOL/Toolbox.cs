using LibGit2Sharp;
using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Net;

namespace SOL
{
    public partial class Toolbox : Form
    {
        public Toolbox()
        {
            InitializeComponent();
        }

        IEnumerable branches = null;//stores all the additions
        IEnumerator enumT = null;//stores the enumerable's current possition and other such data
        string path = "";
        int org = 0;
        //bool click = false;
        bool rot = false;
        Process[] openedAdd = new Process[0];
        object[] removed = new object[0];//stores the set of installed additions lstbx objects that where removed
        int[] removedIndex = new int[0];//and their respective indicies
        public static int addCount = 0;

        private void Toolbox_Load(object sender, EventArgs e)
        {
            //gathering working directory...
            if (Environment.CurrentDirectory.Contains(@"\Debug"))
            {//if in debug mode
                path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location).TrimEnd(@"\bin\Debug".ToCharArray()) + @"\Resources\Additions";
            }
            else
            {
                path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\Resources\Additions";
            }

            //loading images and icons...
            this.Icon = new Icon(path + @"\..\SOLICO.ico");
            clearTextBtn.BackgroundImage = Image.FromFile(path + @"\..\clear.ico");
            ReloadIBtn.BackgroundImage = Image.FromFile(path + @"\..\refresh.ico");
            ReloadDBtn.BackgroundImage = Image.FromFile(path + @"\..\refresh.ico");
            clearDwnBtn.BackgroundImage = Image.FromFile(path + @"\..\clear.ico");

            //setting start opacity...
                if (int.TryParse((Opacity * 100).ToString(), out int s))
            {
                sldBrOpacity.Value = s;//set opacity on load
            }

            //searching the Resources>Additions folder for installed additions...
            updateInstalledAddLstBx();

            //checking if any additions failed to install & cleanup...
            string[] names = new string[0];
            ushort zn = 0;
            ushort dn = 0;
            System.Collections.ObjectModel.ReadOnlyCollection<string> adds = Microsoft.VisualBasic.FileIO.FileSystem.GetDirectories(path);
            System.Collections.ObjectModel.ReadOnlyCollection<string> files = Microsoft.VisualBasic.FileIO.FileSystem.GetFiles(path);
            System.Collections.Generic.List<string> all = new System.Collections.Generic.List<string>();
            all = adds.ToList();

            foreach (string file in files)
            {
                all.Add(file);
            }

            foreach (string add in all) {
                string[] pathSplit = add.Split(Convert.ToChar(92));
                if (pathSplit[pathSplit.Length - 1] != ".gitplzdontignoreme")
                {
                    if (File.Exists(add))
                    {
                        File.Delete(add);
                        zn++;
                        Array.Resize(ref names, names.Length + 1);
                        names[names.Length - 1] = pathSplit[pathSplit.Length - 1].Remove(pathSplit[pathSplit.Length - 1].Length - 4, 4);

                    }

                    if (pathSplit[pathSplit.Length - 1].Contains("-"))
                    {
                        Directory.Delete(add, true);
                        dn++;

                        if (names.Length != 0)
                        {
                            if (names[names.Length - 1] != pathSplit[pathSplit.Length - 1])
                            {
                                Array.Resize(ref names, names.Length + 1);
                                names[names.Length - 1] = pathSplit[pathSplit.Length - 1].Replace('-', ' ');

                            }
                        }
                        else
                        {
                            Array.Resize(ref names, names.Length + 1);
                            names[names.Length - 1] = pathSplit[pathSplit.Length - 1].Replace('-', ' ');

                        }

                    }
                }

            }

            if (names.Length == 1)
            {
                MessageBox.Show("One addition; " + names[0] + ", did not complete their" +
                    " installation process, as such it was removed and must be re-downloaded.");
            }
            else if (names.Length > 0) {
                string text = "Multiple additions; ";

                //place each additions' name in the message
                foreach (string name in names) {
                    if (name != names[names.Length - 2])
                    {//if not at the end
                        text += name + ", ";
                    }
                    else {
                        text += name + " and ";
                    }

                }

                MessageBox.Show(text + "did not complete their installation processes, as " +
                    "such they where removed and must be re-downloaded.");

                //re-searching the Resources>Additions folder for installed additions...
                updateInstalledAddLstBx();

            }

            //getting Y location of the "manage additions" button for animation...
            org = AddBtn.Location.Y;

        }

        private void aboutBtn_Click(object sender, EventArgs e)
        {
            AboutBox test = new AboutBox();
            Form isOpen = Application.OpenForms["AboutBox"];        //get if the window is still open
                                                                    //
            if (isOpen != null) { isOpen.Close(); }                 //if it is then close it
                                                                    //
            test.Show();                                            //otherwise open it
        }

        private void sldBrOpacity_Scroll(object sender, EventArgs e)
        {
            double opacity = sldBrOpacity.Value;        //get the value from the opacity slider
            double test = (opacity / 100);              //convert to decimal

            this.Opacity = test;                        //set the opacity
        }

        private void updatesBtn_Click(object sender, EventArgs e)
        {
            Updates form = new Updates();
            Form isOpen = Application.OpenForms["Updates"];        //get if the window is still open
                                                                        //
            if (isOpen != null) { isOpen.Close(); }                     //if it is then close it
                                                                        //
            form.Show();                                                //otherwise open it
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            DownloadAddLstBx.Items.Clear();
            DialogResult response = DialogResult.Ignore;
            //get all additions (git repo branches)
            if (AddBtn.Location.Y <= org)
            {
                while (true)
                {
                    try
                    {
                        branches = Repository.ListRemoteReferences("https://github.com/shadow999999/Style-of-Life")
                                     .Where(elem => elem.IsLocalBranch)
                                     .Select(elem => elem.CanonicalName
                                                         .Replace("refs/heads/", ""));

                        foreach (string branch in branches)
                        {//get each addition seperetly
                            if (branch != "master")
                            {
                                string branchT = branch.Replace('-', ' ');
                                DownloadAddLstBx.Items.Add(branchT);
                            }//as long as the branch is not the main, add it to the list

                        }
                        enumT = branches.GetEnumerator();
                        DownloadAddLstBx.SelectedIndex = 0;//forces ProgramsLstBx_SelectedIndexChanged to trigger
                        break;
                    }
                    catch (LibGit2Sharp.LibGit2SharpException libGit2SharpException)
                    {
                        if (libGit2SharpException.Message == "this remote has never connected")
                        {
                            response = MessageBox.Show("An error occured; github may be down or you have no internet!",
                                "Connectivity error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation);
                            if (response == DialogResult.Cancel) { break; }
                            //handle later
                        }
                        else
                        {
                            MessageBox.Show("An unknown error occured whilst trying to retreive data from github: " + libGit2SharpException.Message);
                            break;
                        }
                    }
                }
            }

            if (response != DialogResult.Cancel)
            {
                InstalledAddLstBx.SelectionMode = SelectionMode.One;
                timer1.Start();

                if (AddBtn.Location.Y <= org)
                {
                    InstalledAddLstBx.SelectionMode = SelectionMode.MultiExtended;
                    Padding test = label5.Padding;
                    test.Right = 127;
                    label5.Padding = test;
                    rot = false;

                }//if this button is at the top possition (it's original location), reset rotation and set the opacity label back to default
            }

        }

        private void ProgramsLstBx_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void DownloadBtn_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(path + @"\" + enumT.Current.ToString().Replace('-', ' ')))
            {
                DialogResult response = DialogResult.Cancel;
                while (true)
                {
                    try
                    {
                        if (!File.Exists(path + @"\" + enumT.Current.ToString() + ".zip"))
                        {//if the .zip file doesn't already exist
                            using (var client = new WebClient())
                            {
                                client.Headers.Add("user-agent", "Anything");
                                client.DownloadFile(
                                    "https://github.com/shadow999999/Style-of-Life/archive/" + enumT.Current.ToString() + ".zip",
                                    path + @"\" + enumT.Current.ToString() + ".zip");//download the zip version of the repository, branch
                            }
                        }
                        else {
                            response = MessageBox.Show("The zip file for this addition already exists, the installtion process may have been pre-emptively " +
                                "stopped before the process could fail gracefully. This file is likely corrupt, would you like to redownload?",
                                "Program may have stopped pre-emptively", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                            if (response == DialogResult.Yes)
                            {
                                //delete the files
                                if (Directory.Exists(path + @"\Style-of-Life-" + enumT.Current.ToString()))
                                {
                                    Directory.Delete(path + @"\Style-of-Life-" + enumT.Current.ToString(), true);
                                }

                                if (Directory.Exists(path + @"\" + enumT.Current.ToString().Replace('-', ' ')))
                                {
                                    Directory.Delete(path + @"\" + enumT.Current.ToString().Replace('-', ' '), true);
                                }

                            }
                        }

                        //ensure the file is not currently in use (being scanned by an AV)
                        FileInfo fileI = new FileInfo(path + @"\" + enumT.Current.ToString() + ".zip");
                        ErrorLogger threadCheck = new ErrorLogger();
                        response = DialogResult.OK;

                        while (true)
                        {
                            int breakOut = 10;
                            while (threadCheck.IsFileLocked(fileI) && breakOut != 0)
                            {//if the thread is locked
                                Thread.Sleep(1000);//wait for any AV programs to finish "licking" the new file
                                breakOut--;//force the while loop to end after 10 seconds (10 loops)
                            }

                            if (breakOut == 0)
                            {
                                response = MessageBox.Show("Can not continue with installtion; the zip file is currently in use",
                                "File locked", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation);
                                if (response == DialogResult.Cancel) {
                                    break;
                                }
                            }

                            if (!threadCheck.IsFileLocked(fileI)) {
                                break;//if file is no longer locked
                            }

                        }

                        if (response == DialogResult.Cancel)
                        {
                            break;
                        }

                        response = DialogResult.No;
                        if (!Directory.Exists(path + @"\Style-of-Life-" + enumT.Current.ToString()))
                        {//if the uncompressed files are not already present
                            System.IO.Compression.ZipFile.ExtractToDirectory(path + @"\" + enumT.Current.ToString() + ".zip", path + @"\");//unpack the zip file, note: you can use ZipFile.CreateFromDirectory to create zip files
                        }
                        else {
                            response = MessageBox.Show("A directory for this addition already exists, the installtion process may have been pre-emptively " +
                                "stopped before the process could fail gracefully. The install is likely corrupt, would you like to redownload?",
                                "Program may have stopped pre-emptively", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                            if (response == DialogResult.Yes) {
                                //delete the files
                                if (Directory.Exists(path + @"\Style-of-Life-" + enumT.Current.ToString()))
                                {
                                    Directory.Delete(path + @"\Style-of-Life-" + enumT.Current.ToString(), true);
                                }

                                if (Directory.Exists(path + @"\" + enumT.Current.ToString().Replace('-', ' ')))
                                {
                                    Directory.Delete(path + @"\" + enumT.Current.ToString().Replace('-', ' '), true);
                                }

                            }
                        }

                        if (response == DialogResult.No)
                        {
                            Directory.Move(path + @"\Style-of-Life-" + enumT.Current.ToString(), path + @"\" + enumT.Current.ToString().Replace('-', ' '));//rename the folder to its correct name
                        }

                        if (File.Exists(path + @"\" + enumT.Current.ToString() + ".zip")) {
                            File.Delete(path + @"\" + enumT.Current.ToString() + ".zip");                        //delete the .zip file
                        }

                        break;
                    }
                    catch (Exception exception)
                    {
                            MessageBox.Show("An error occured whilst trying to add a new addition and will be aborted, reason: " + exception.Message,
                            "Error code: " + exception.HResult, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                        if (File.Exists(path + @"\" + enumT.Current.ToString() + ".zip"))
                        {//if the zip file still exists
                            File.Delete(path + @"\" + enumT.Current.ToString() + ".zip");//delete the .zip file as it may not have downloaded correctly (causing cascading error)
                        }

                        if (Directory.Exists(path + @"\Style-of-Life-" + enumT.Current.ToString()))
                        {//if the file managed to extract the folders
                            Directory.Delete(path + @"\Style-of-Life-" + enumT.Current.ToString(), true);//delete the folder as an error may have occred while extracting that was not caught (causing cascading error)
                        }

                        if (Directory.Exists(path + @"\" + enumT.Current.ToString().Replace('-', ' ')))
                        {//if the file managed to managed to rename
                            Directory.Delete(path + @"\" + enumT.Current.ToString().Replace('-', ' '), true);//delete the folder as an error occred while moving the folder contents to the renamed folder
                        }

                     }

                }

            }
            else {
                MessageBox.Show("You already have this installed.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            updateInstalledAddLstBx();
            //message box

            //this will mean branches will (at some point) have delemeters indicating if it's an 'override' or an 'addition' - that is if it updates existing code/forms or adds new code/forms.
        }

        private void RemoveBtn_Click(object sender, EventArgs e)
        {
            foreach (string item in InstalledAddLstBx.SelectedItems)
            {
                if (Directory.Exists(path + @"\" + item))
                {
                    Directory.Delete(path + @"\" + item, true);
                }
            }

            InstalledAddLstBx.Items.Clear();
            updateInstalledAddLstBx();
        }
        //8
        private void timer1_Tick(object sender, EventArgs e)
        {

            if (AddBtn.Location.Y >= aboutBtn.Location.Y - 35 && Size.Width >= 485 && AddBtn.Width <= 140 && OpacityPnl.Width >= 471)
            {//if this button is at the about buttons location
                if (!rot)
                {//and not rotating
                    timer1.Stop();//stop timer
                }

                OpacityPnl.Width += 10; ;

                Size testS = Size;
                testS.Width += 10;//resize the form
                Size = testS;

                AddBtn.Width = 133;
                AddBtn.Text = "Stop managing additions";
                ForceUpdateBtn.Visible = false;
                updatesBtn.Visible = false;
                aboutBtn.Visible = false;
                rot = true;//start rotation
            }

            if (rot)
            {//if rotating back up

                if (OpacityPnl.Width > 309)
                {
                    OpacityPnl.Width -= 10; ;
                    Padding test = label5.Padding;
                    test.Right -= 5;
                    label5.Padding = test;
                }

                if (Size.Width > 325)
                {
                    Size testS = Size;
                    testS.Width -= 10;//resize the form
                    Size = testS;
                }
                else if (Size.Width <= 325)
                {
                    ReloadDBtn.Visible = false;
                    DownloadAddLstBx.Visible = false;
                    DownloadBtn.Visible = false;
                    downloadSearchTxtBx.Visible = false;
                    clearDwnBtn.Visible = false;
                }

                if (AddBtn.Location.Y > org)
                {//and this button is not at the top (its original possition)
                    ReloadDBtn.Visible = true;
                    //ForceUpdateBtn.Visible = true;
                    updatesBtn.Visible = true;
                    aboutBtn.Visible = true;
                    InstalledMngRsr.Height -= 5;//have the manager window move back up

                }
                else if (AddBtn.Location.Y <= org)
                {
                    AddBtn.Text = "Manage additions";
                }

                if (AddBtn.Width < 284)
                {
                    AddBtn.Width += 10;
                }
                else if (AddBtn.Width > 284)
                {
                    AddBtn.Width = 284;
                    UninstallBtn.Visible = false;
                }

            }
            else
            {
                UninstallBtn.Visible = true;
                DownloadAddLstBx.Visible = true;
                DownloadBtn.Visible = true;
                downloadSearchTxtBx.Visible = true;
                clearDwnBtn.Visible = true;

                if (OpacityPnl.Width < 471)
                {
                    OpacityPnl.Width += 10;
                    Padding test = label5.Padding;
                    test.Right += 5;
                    label5.Padding = test;
                }

                if (Size.Width < 485)
                {
                    Size testS = Size;
                    testS.Width += 10;//resize the form
                    Size = testS;
                }

                if (AddBtn.Location.Y < aboutBtn.Location.Y - 35)
                {
                    InstalledMngRsr.Height += 5;//have the manager window move down

                }

                if (AddBtn.Width > 140)
                {
                    AddBtn.Width -= 10;
                    //AddBtn.Text = "" + AddBtn.Location.X;

                }
                else if (AddBtn.Width <= 140)
                {
                    AddBtn.Width = 140;
                }

            }

        }

        private void DownloadAddLstBx_SelectedIndexChanged(object sender, EventArgs e)
        {
            enumT = branches.GetEnumerator();
            int i = 0;
            while (i != DownloadAddLstBx.SelectedIndex + 1)
            {
                enumT.MoveNext();
                i++;

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DownloadAddLstBx.Items.Clear();

            //get all additions (git repo branches)
            while (true)
            {
                try
                {
                    branches = Repository.ListRemoteReferences("https://github.com/shadow999999/Style-of-Life")
                                 .Where(elem => elem.IsLocalBranch)
                                 .Select(elem => elem.CanonicalName
                                                     .Replace("refs/heads/", ""));

                    foreach (string branch in branches)
                    {//get each addition seperetly
                        if (branch != "master")
                        {
                            string branchT = branch.Replace('-', ' ');
                            DownloadAddLstBx.Items.Add(branchT);
                        }//as long as the branch is not the main, add it to the list

                    }
                    enumT = branches.GetEnumerator();
                    DownloadAddLstBx.SelectedIndex = 0;//forces ProgramsLstBx_SelectedIndexChanged to trigger
                    break;
                }
                catch (LibGit2Sharp.LibGit2SharpException libGit2SharpException)
                {
                    if (libGit2SharpException.Message == "this remote has never connected")
                    {
                        DialogResult response = MessageBox.Show("An error occured; github may be down or you have no internet!",
                            "Connectivity error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation);
                        if (response == DialogResult.Cancel)
                        {
                            break;
                        }
                        //handle later
                    }
                    else
                    {
                        MessageBox.Show("An unknown error occured whilst trying to retreive data from github: " + libGit2SharpException.Message);
                        break;
                    }
                }
            }
        }

        private void InstalledAddLstBx_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            bool processExists = false;
            Process addition = null;

            for (int i = 0; i < openedAdd.Length; i++)
            {
                addition = openedAdd[i];
                if (addition.ProcessName == InstalledAddLstBx.SelectedItem.ToString())
                {
                    processExists = true;
                    break;
                }
            }

            if (!processExists)
            {
                if (File.Exists(path + @"\" + InstalledAddLstBx.SelectedItem + @"\" + InstalledAddLstBx.SelectedItem
                + @"\" + InstalledAddLstBx.SelectedItem + @"\bin\Debug\" + InstalledAddLstBx.SelectedItem + ".exe"))//yea i know...
                {
                    Array.Resize(ref openedAdd, openedAdd.Length + 1);
                    openedAdd[openedAdd.Length - 1] = Process.Start(path + @"\" + InstalledAddLstBx.SelectedItem + @"\" + InstalledAddLstBx.SelectedItem
                        + @"\" + InstalledAddLstBx.SelectedItem + @"\bin\Debug\" + InstalledAddLstBx.SelectedItem + ".exe");

                }
                else
                {
                    MessageBox.Show("Could not find executable. " + path + @"\" + InstalledAddLstBx.SelectedItem + @"\" + InstalledAddLstBx.SelectedItem
                        + @"\" + InstalledAddLstBx.SelectedItem + @"\bin\Debug\" + InstalledAddLstBx.SelectedItem + ".exe");
                }
            }
            else
            {
                //MessageBox.Show("The program is already running.");
                addition.Kill();
                addition.Start();//restart the process
                //(in order to be able to force a process to move itself to the center of the screen,
                //the function will have to be exposed specifically by that process itself)

            }

        }

        private void ReloadIBtn_Click(object sender, EventArgs e)
        {
            updateInstalledAddLstBx();
        }

        private void UpdateAddBtn_Click(object sender, EventArgs e)
        {

        }

        private void Toolbox_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (Process addition in openedAdd)
            {
                if (!addition.HasExited)
                {
                    addition.Kill();
                }

            }
        }

        private void textBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (searchTxtBx.Text.Length > 0 && searchTxtBx.Text == "Search")
            {
                if (searchTxtBx.Focused)
                {
                    searchTxtBx.Text = "";
                    searchTxtBx.ForeColor = Color.Black;
                }
            }
        }

        private void searchTxtBx_Leave(object sender, EventArgs e)
        {
            if (searchTxtBx.Text.Length <= 0)
            {
                searchTxtBx.Text = "Search";
                searchTxtBx.ForeColor = Color.FromName("ControlDark");
                InstalledAddLstBx.SelectionMode = SelectionMode.One;
                InstalledAddLstBx.Items.Clear();
                removed = new object[0];//stores the set of installed additions lstbx objects that where removed
                removedIndex = new int[0];//and their respective indicies
                updateInstalledAddLstBx();
            }
            else
            {
                InstalledAddLstBx.SelectionMode = SelectionMode.One;
                if (InstalledAddLstBx.Items.Count > 0)
                {
                    InstalledAddLstBx.SelectedIndex = 0;
                }

            }

        }

        private void searchTxtBx_TextChanged(object sender, EventArgs e)
        {
            if (addCount > 0)
            {
                if (searchTxtBx.Text.Length > 0)
                {
                    for (int i = 0; i < removed.Length; i++)
                    {
                        InstalledAddLstBx.Items.Insert(removedIndex[i], removed[i]);
                    }

                    //InstalledAddLstBx.SelectionMode = SelectionMode.MultiSimple;
                    int itemCount = InstalledAddLstBx.Items.Count;
                    int[] tmp = new int[0];
                    removed = new object[0];//stores the set of installed additions lstbx objects that where removed
                    removedIndex = new int[0];//and their respective indicies
                    InstalledAddLstBx.SelectedIndex = 0;

                    //InstalledAddLstBx.ClearSelected();
                    for (int i = 0; i < itemCount; i++)
                    {
                        InstalledAddLstBx.ClearSelected();
                        InstalledAddLstBx.SetSelected(i, true);
                        if (InstalledAddLstBx.GetItemText(InstalledAddLstBx.SelectedItem).ToLower().Contains(searchTxtBx.Text.ToLower()))
                        {//check if the current item has any of the characters/text in their name
                            Array.Resize(ref tmp, tmp.Length + 1);
                            tmp[tmp.Length - 1] = InstalledAddLstBx.SelectedIndex;

                        }
                        else
                        {
                            Array.Resize(ref removed, removed.Length + 1);
                            Array.Resize(ref removedIndex, removedIndex.Length + 1);
                            removed[removed.Length - 1] = InstalledAddLstBx.SelectedItem;//store the removed item
                            removedIndex[removedIndex.Length - 1] = InstalledAddLstBx.SelectedIndex;//store the removed item's index
                            InstalledAddLstBx.Items.RemoveAt(InstalledAddLstBx.SelectedIndex);
                            itemCount--;//account for the fact that there is now 1 less item in the list
                            i--;

                        }
                    }

                    InstalledAddLstBx.ClearSelected();
                    //for debugging
                    //for (int i = 0; i < tmp.Length; i++)
                    //{
                    //    InstalledAddLstBx.SetSelected(tmp[i], true);

                    //}
                    if (InstalledAddLstBx.Items.Count > 0)
                    {
                        InstalledAddLstBx.SelectedIndex = 0;
                    }

                }
                else
                {
                    InstalledAddLstBx.SelectionMode = SelectionMode.One;
                    for (int i = 0; i < removed.Length; i++)
                    {
                        InstalledAddLstBx.Items.Insert(removedIndex[i], removed[i]);
                    }
                    removed = new object[0];//stores the set of installed additions lstbx objects that where removed
                    removedIndex = new int[0];//and their respective indicies
                    if (InstalledAddLstBx.Items.Count > 0)
                    {
                        InstalledAddLstBx.SelectedIndex = 0;
                    }

                }
            }

        }

        private void clearTextBtn_Click(object sender, EventArgs e)
        {
            searchTxtBx.Text = "Search";
            searchTxtBx.ForeColor = Color.FromName("ControlDark");

            InstalledAddLstBx.SelectionMode = SelectionMode.One;
            InstalledAddLstBx.Items.Clear();
            removed = new object[0];//stores the set of installed additions lstbx objects that where removed
            removedIndex = new int[0];//and their respective indicies
            updateInstalledAddLstBx();

            if (InstalledAddLstBx.Items.Count > 0)
            {
                InstalledAddLstBx.SelectedIndex = 0;
            }

        }

        private void downloadTxtBx_MouseClick(object sender, MouseEventArgs e)
        {
            if (downloadSearchTxtBx.Text.Length > 0 && downloadSearchTxtBx.Text == "Search")
            {
                if (downloadSearchTxtBx.Focused)
                {
                    downloadSearchTxtBx.Text = "";
                    downloadSearchTxtBx.ForeColor = Color.Black;
                }
            }
        }

        private void downloadTxtBx_Leave(object sender, EventArgs e)
        {
            if (downloadSearchTxtBx.Text.Length <= 0)
            {
                downloadSearchTxtBx.Text = "Search";
                downloadSearchTxtBx.ForeColor = Color.FromName("ControlDark");
                DownloadAddLstBx.SelectionMode = SelectionMode.One;
                removed = new object[0];//stores the set of installed additions lstbx objects that where removed
                removedIndex = new int[0];//and their respective indicies
                updateDownloadAddLstBx();
            }
            else
            {
                DownloadAddLstBx.SelectionMode = SelectionMode.One;
                if (DownloadAddLstBx.Items.Count > 0)
                {
                    DownloadAddLstBx.SelectedIndex = 0;
                }

            }

        }

        private void downloadTxtBx_TextChanged(object sender, EventArgs e)
        {
            if (addCount > 0)
            {
                if (downloadSearchTxtBx.Text.Length > 0)
                {
                    for (int i = 0; i < removed.Length; i++)
                    {
                        DownloadAddLstBx.Items.Insert(removedIndex[i], removed[i]);
                    }

                    //DownloadAddLstBx.SelectionMode = SelectionMode.MultiSimple;
                    int itemCount = DownloadAddLstBx.Items.Count;
                    int[] tmp = new int[0];
                    removed = new object[0];//stores the set of installed additions lstbx objects that where removed
                    removedIndex = new int[0];//and their respective indicies
                    DownloadAddLstBx.SelectedIndex = 0;

                    //DownloadAddLstBx.ClearSelected();
                    for (int i = 0; i < itemCount; i++)
                    {
                        DownloadAddLstBx.ClearSelected();
                        DownloadAddLstBx.SetSelected(i, true);
                        if (DownloadAddLstBx.GetItemText(DownloadAddLstBx.SelectedItem).ToLower().Contains(downloadSearchTxtBx.Text.ToLower()))
                        {
                            Array.Resize(ref tmp, tmp.Length + 1);
                            tmp[tmp.Length - 1] = DownloadAddLstBx.SelectedIndex;

                        }
                        else
                        {
                            Array.Resize(ref removed, removed.Length + 1);
                            Array.Resize(ref removedIndex, removedIndex.Length + 1);
                            removed[removed.Length - 1] = DownloadAddLstBx.SelectedItem;//store the removed item
                            removedIndex[removedIndex.Length - 1] = DownloadAddLstBx.SelectedIndex;//store the removed item's index
                            DownloadAddLstBx.Items.RemoveAt(DownloadAddLstBx.SelectedIndex);
                            itemCount--;//account for the fact that there is now 1 less item in the list
                            i--;

                        }
                    }

                    DownloadAddLstBx.ClearSelected();
                    //for debugging
                    //for (int i = 0; i < tmp.Length; i++)
                    //{
                    //    DownloadAddLstBx.SetSelected(tmp[i], true);

                    //}
                    if (DownloadAddLstBx.Items.Count > 0)
                    {
                        DownloadAddLstBx.SelectedIndex = 0;
                    }

                }
                else
                {
                    DownloadAddLstBx.SelectionMode = SelectionMode.One;
                    for (int i = 0; i < removed.Length; i++)
                    {
                        DownloadAddLstBx.Items.Insert(removedIndex[i], removed[i]);
                    }
                    removed = new object[0];//stores the set of installed additions lstbx objects that where removed
                    removedIndex = new int[0];//and their respective indicies
                    if (DownloadAddLstBx.Items.Count > 0)
                    {
                        DownloadAddLstBx.SelectedIndex = 0;
                    }

                }
            }
        }

        private void clearDwnBtn_Click(object sender, EventArgs e)
        {
            downloadSearchTxtBx.Text = "Search";
            downloadSearchTxtBx.ForeColor = Color.FromName("ControlDark");

            DownloadAddLstBx.SelectionMode = SelectionMode.One;
            removed = new object[0];//stores the set of installed additions lstbx objects that where removed
            removedIndex = new int[0];//and their respective indicies
            updateDownloadAddLstBx();

            if (DownloadAddLstBx.Items.Count > 0)
            {
                DownloadAddLstBx.SelectedIndex = 0;
            }
        }

        public void updateDownloadAddLstBx() {
            DownloadAddLstBx.Items.Clear();

            //get all additions (git repo branches)
            while (true)
            {
                try
                {
                    branches = Repository.ListRemoteReferences("https://github.com/shadow999999/Style-of-Life")
                                 .Where(elem => elem.IsLocalBranch)
                                 .Select(elem => elem.CanonicalName
                                                     .Replace("refs/heads/", ""));

                    foreach (string branch in branches)
                    {//get each addition seperetly
                        if (branch != "master")
                        {
                            string branchT = branch.Replace('-', ' ');
                            DownloadAddLstBx.Items.Add(branchT);
                        }//as long as the branch is not the main, add it to the list

                    }
                    enumT = branches.GetEnumerator();
                    DownloadAddLstBx.SelectedIndex = 0;//forces ProgramsLstBx_SelectedIndexChanged to trigger
                    break;
                }
                catch (LibGit2Sharp.LibGit2SharpException libGit2SharpException)
                {
                    if (libGit2SharpException.Message == "this remote has never connected")
                    {
                        DialogResult response = MessageBox.Show("An error occured; github may be down or you have no internet!",
                            "Connectivity error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation);
                        if (response == DialogResult.Cancel)
                        {
                            break;
                        }
                        //handle later
                    }
                    else
                    {
                        MessageBox.Show("An unknown error occured whilst trying to retreive data from github: " + libGit2SharpException.Message);
                        break;
                    }
                }
            }
        }

        public void updateInstalledAddLstBx()
        {
            InstalledAddLstBx.Items.Clear();
            addCount = 0;
            System.Collections.ObjectModel.ReadOnlyCollection<string> adds = Microsoft.VisualBasic.FileIO.FileSystem.GetDirectories(path);
            foreach (string add in adds)
            {
                string[] pathSplit = add.Split(Convert.ToChar(92));
                InstalledAddLstBx.Items.Add(pathSplit[pathSplit.Length - 1]);
                addCount++;

            }

        }
    }
}
