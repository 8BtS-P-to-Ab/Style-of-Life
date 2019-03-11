using LibGit2Sharp;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Translators
{
    public partial class TranslatorsFrm : Form
    {
        public TranslatorsFrm()
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

        private void TranslatorsFrm_Load(object sender, EventArgs e)
        {
            if (Environment.CurrentDirectory.Contains(@"\Debug"))
            {//if in debug mode
                path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location).TrimEnd(@"\bin\Debug".ToCharArray()) + @"\Resources\Additions";
            }
            else
            {
                path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\Resources\Additions";
            }

            this.Icon = new Icon(path + @"\..\SOLICO.ico");
            clearTextBtn.BackgroundImage = Image.FromFile(path + @"\..\clear.ico");
            ReloadIBtn.BackgroundImage = Image.FromFile(path + @"\..\refresh.ico");
            ReloadDBtn.BackgroundImage = Image.FromFile(path + @"\..\refresh.ico");
            clearDwnBtn.BackgroundImage = Image.FromFile(path + @"\..\clear.ico");

            if (int.TryParse((Opacity * 100).ToString(), out int s))
            {
                sldBrOpacity.Value = s;//set opacity on load
            }

            //search the Resources>Additions folder for installed additions
            updateInstalledAddLstBx();

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
            Form isOpen = Application.OpenForms["Mass Renamer"];        //get if the window is still open
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
                        branches = Repository.ListRemoteReferences("https://github.com/shadow999999/Translators-SOL")
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

        private void InstalledAddLstBx_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void DownloadBtn_Click(object sender, EventArgs e)
        {
            while (true)
            {
                try
                {
                    string clonedRepoPath = Repository.Clone("https://github.com/shadow999999/Translators-SOL"
                        , path + @"\" + enumT.Current.ToString().Replace('-', ' '), new CloneOptions { BranchName = enumT.Current.ToString() });

                    if (Directory.Exists(path + @"\" + enumT.Current.ToString() + @"\.git"))
                    {
                        try
                        {
                            Directory.Delete(path + @"\" + enumT.Current.ToString() + @"\.git", true);
                        }
                        catch (Exception exc)
                        {
                            MessageBox.Show("An error occured while trying to delete useless junk: " + exc.Message, "Error code: " + exc.HResult);
                        }
                    }
                    //

                    break;
                }
                catch (LibGit2Sharp.LibGit2SharpException libGit2SharpException)
                {
                    if (libGit2SharpException.Message == "this remote has never connected")
                    {
                        DialogResult response = MessageBox.Show("An error occured, the file may not have downloaded and is likely corrupt, the file will be deleted." +
                            "Cuase: github may be down or you have no internet!", "Connectivity error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation);
                        if (response == DialogResult.Retry)
                        {
                            if (Directory.Exists(path + @"\" + enumT.Current.ToString()))
                            {
                                Directory.Delete(path + @"\" + enumT.Current.ToString(), true);
                            }

                        }
                        else
                        {
                            if (Directory.Exists(path + @"\" + enumT.Current.ToString()))
                            {
                                Directory.Delete(path + @"\" + enumT.Current.ToString(), true);
                                break;
                            }

                        }

                    }
                    else if (libGit2SharpException.HResult == -2146233088)
                    {
                        MessageBox.Show("You already have this installed.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    }
                    else
                    {
                        DialogResult response = MessageBox.Show("An unknown error occured whilst trying to retreive data from github: " + libGit2SharpException.Message,
                            "Error code: " + libGit2SharpException.HResult, MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation);
                        if (response == DialogResult.Retry)
                        {
                            if (Directory.Exists(path + enumT.Current.ToString()))
                            {
                                Directory.Delete(path + enumT.Current.ToString(), true);
                            }

                        }
                        else
                        {
                            if (Directory.Exists(path + enumT.Current.ToString()))
                            {
                                Directory.Delete(path + @"\" + enumT.Current.ToString(), true);
                                break;
                            }

                        }
                    }
                }
            }

            updateInstalledAddLstBx();
            //message box

            //this will mean branches will (at some point) have delemeters indicating if it's an 'override' or an 'addition' - that is if it updates existing code/forms or adds new code/forms.
        }

        private void UninstallBtn_Click(object sender, EventArgs e)
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

        private void timer1_Tick(object sender, EventArgs e)
        {

            if (AddBtn.Location.Y >= aboutBtn.Location.Y - 35 && Size.Width >= 485 && AddBtn.Width <= 140 && OpacityPnl.Width >= 471)
            {//if this button is at the about buttons location (bottom)
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

                if (Size.Width > 307)
                {
                    Size testS = Size;
                    testS.Width -= 10;//resize the form
                    Size = testS;
                }
                else if (Size.Width <= 307)
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

                if (AddBtn.Location.Y < aboutBtn.Location.Y - 25)
                {
                    InstalledMngRsr.Height += 5;//have the manager resizeer window move down

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

        private void ReloadDBtn_Click(object sender, EventArgs e)
        {
            DownloadAddLstBx.Items.Clear();

            //get all additions (git repo branches)
            while (true)
            {
                try
                {
                    branches = Repository.ListRemoteReferences("https://github.com/shadow999999/Translators-SOL")
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

        private void TranslatorsFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (Process addition in openedAdd)
            {
                if (!addition.HasExited)
                {
                    addition.Kill();
                }

            }
        }

        private void searchTxtBx_MouseClick(object sender, MouseEventArgs e)
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
                        {
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

        private void downloadSearchTxtBx_MouseClick(object sender, MouseEventArgs e)
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

        private void downloadSearchTxtBx_Leave(object sender, EventArgs e)
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

        private void downloadSearchTxtBx_TextChanged(object sender, EventArgs e)
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

        public void updateDownloadAddLstBx()
        {
            DownloadAddLstBx.Items.Clear();

            //get all additions (git repo branches)
            while (true)
            {
                try
                {
                    branches = Repository.ListRemoteReferences("https://github.com/shadow999999/Translators-SOL")
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
