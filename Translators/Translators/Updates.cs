using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Translators
{
    public partial class Updates : Form
    {
        public Updates()
        {
            InitializeComponent();
        }

        int index = -1;                                                                            //stores the current list box index
        int pindex = -1;                                                                           //stores the previous list box index
        bool loaded = false;                                                                       //indicates if the the Updates_Load function has finished
        string path = "";
        int addCount = 0;
        int primF = 0;
        string[] files;
        ushort searchIndex = 0;

        private void Updates_Load(object sender, EventArgs e)
        {

            addCount = TranslatorsFrm.addCount;//grab the amount of installed additions from the toolbox

            if (Environment.CurrentDirectory.Contains(@"\Debug"))
            {//if in debug mode
                path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location).TrimEnd(@"\bin\Debug".ToCharArray()) + @"\Resources\";
            }
            else
            {
                path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"Resources\";
            }

            this.Icon = new Icon(path + @"\SOLICO.ico");
            NextBtn.BackgroundImage = Image.FromFile(path + @"\arrow.ico");
            clearTextBtn.BackgroundImage = Image.FromFile(path + @"\clear.ico");

            var tmpImg = Image.FromFile(path + @"\arrow.ico");
            tmpImg.RotateFlip(RotateFlipType.Rotate180FlipNone);
            PreviousBtn.BackgroundImage = tmpImg;

            files = Directory.GetFiles(path, "*.txt");                                     //- get every file in the resources\updates folder
            primF = files.Length;

            if (addCount > 0)
            {
                Array.Resize(ref files, files.Length + 1);
                files[files.Length - 1] = path + @"\.brk";//add a break point between SOL updates and addtion updates
                primF = files.Length;

                System.Collections.ObjectModel.ReadOnlyCollection<string> adds = Microsoft.VisualBasic.FileIO.FileSystem.GetDirectories(path + @"\Additions\");
                foreach (string add in adds)
                {
                    Array.Resize(ref files, files.Length + 1);//make room for the extra update from the addition
                    string[] pathSplit = add.Split(Convert.ToChar(92));
                    files[files.Length - 1] = add + @"\" + pathSplit[pathSplit.Length - 1]
                        + @"\" + pathSplit[pathSplit.Length - 1] + @"\Resources\"
                        + pathSplit[pathSplit.Length - 1] + ".txt";

                }

            }
            //
            foreach (string file in files)
            {                                                        //T target each file seperetly
                string[] pathSplit = file.Split(Convert.ToChar(92));                                //|-- split the current file by \ (the last entry in the array is the file name)
                int tl = pathSplit[pathSplit.Length - 1].Length;                                    //|-- get the length of the tab name
                tabControler.TabPages.Add(pathSplit[pathSplit.Length - 1].Remove(tl - 4, 4));        //|-- add the tab (without the .txt extention)
                                                                                                     //|
            }                                                                                       //|L)
                                                                                                    //
            for (int i = 0; i < files.Length; i++)                                                  //T for loop of files.length length itterating from 0
            {                                                                                       //|
                tabControler.SelectTab(i);//only way to target tabs                                 //|--target the 'i'th tab
                tabControler.SelectedTab.Name = tabControler.SelectedTab.Text;                      //|-- set its name to its text so it can be targeted properly
            }                                                                                       //|--L)
                                                                                                    //
            tabControler.SelectTab(0);                                                              //- reset focus to the first tab
            loaded = true;                                                                          //- set loaded to true
                                                                                                    //
            UpdateList(path + tabControler.SelectedTab.Name + ".txt", listBoxAll);                                  //- update the list
            listBoxAll.SelectedIndex = 0;                                                           //- set the top version as the selected one of the list box
            UpdateView(path + tabControler.SelectedTab.Name + ".txt", treeViewAll, listBoxAll);                     //- update the view

        }

        private void listBoxAll_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControler.SelectedIndex < primF)
            {                                                               //if one of the primary tabs (SOL updates e.c.t.)
                UpdateView(path + tabControler.SelectedTab.Name + ".txt", treeViewAll, listBoxAll);//updates the view (the tree)
            }
            else
            {
                UpdateView(files[tabControler.SelectedIndex], treeViewAll, listBoxAll);      //|-- update the list
            }
        }

        private void TabControler_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (loaded)                                                     //T if the form has finished loading
            {                                                               //|
                pindex = -1;                                                //|-- reset the previous index selected (so the animation triggers)
                listBoxAll.Items.Clear();                                   //|-- clear the list box
                if (tabControler.SelectedTab.Text != "")
                {
                    if (tabControler.SelectedIndex < primF)
                    {                                                               //if one of the primary tabs (SOL updates e.c.t.)
                        UpdateList(path + tabControler.SelectedTab.Name + ".txt", listBoxAll);      //|-- update the list
                    }
                    else
                    {
                        UpdateList(files[tabControler.SelectedIndex], listBoxAll);      //|-- update the list
                    }
                }
                //|
            }                                                               //|e)
        }

        /// <summary>
        /// Update the target list box with the file names' contents, 
        /// only searching for "\s" delemetres and ensuring the file is structured correctly
        /// </summary>
        /// <param name="fileFullPath"></param>
        /// <param name="list"></param>
        private void UpdateList(string fileFullPath, ListBox list)
        {
            int sC = 0;                                                                             //- '\s' count
            int iC = 0;                                                                             //- '\i' count
            int eC = 0;                                                                             //- '\e' count
            StreamReader stream = File.OpenText(fileFullPath);                          //- open the text file's stream
            string data = stream.ReadLine();                                                        //- read the first line of the file
                                                                                                    //
            while (!stream.EndOfStream)                                                             //T while the stream is not at the end of the file
            {                                                                                       //|
                if (data.Contains(@"\s"))                                                           //|-T if the line is a version
                {                                                                                   //| |
                    list.Items.Add(data.Remove((data.Length - 2), 2));                              //|-|--- add the version to the list (without the '\s' present)
                    sC++;                                                                           //|-|--- count the amount of times a version delemeter is found
                }
                else if (data.Contains(@"\i"))
                {                                                  //|-\c)  otherwise if the delemeter is a node
                    iC++;                                                                           //|-|--- count the amount of times a node delemeter is found
                }                                                                                   //|-|e)
                                                                                                    //|
                data = stream.ReadLine();                                                           //|-- read the next line
                                                                                                    //|
                if (data.Contains(@"\e"))                                                           //|-T if the current delemeter is an end line
                {                                                                                   //| |
                    eC++;                                                                           //|-|--- count the amount of times an end delemeter is found
                }                                                                                   //|-|e)
            }                                                                                       //|L)
            stream.Close();                                                                         //- close the file
                                                                                                    //
            if (sC + iC != eC)
            {                                                                    //T if amount special delemeters does not match the amount of end delemeters
                MessageBox.Show("An error occured while attempting to read the text file \""        //|
                    + fileFullPath + "\"; the amount of ending delemeters (" + eC                       //|
                    + ") does not match the amount of special delemeters (" + (sC + iC) + ")."      //|
                    , "File read: delemeters not equal error");                                     //|-- warn the user/developer
                Close();                                                                            //|-- end the process
            }                                                                                       //|e)

            if (sC == 0 || eC == 0)
            {                                                               //T if there is a missing delemeter
                MessageBox.Show("An error occured while attempting to read the text file \""        //|
                    + fileFullPath + "\"; one or more delemeters are missing " +                    //|
                    "(all of these must not be 0), title count = " + sC                             //|
                    + " end node count = " + eC, @"File read: no delemeter\s");                     //|-- warn the user/developer
                Close();                                                                            //|-- end the process
            }                                                                                       //|e)

        }

        /// <summary>
        /// Controlls the entire forms animations
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="view"></param>
        /// <param name="list"></param>
        private void UpdateView(string fileName, TreeView view, ListBox list)
        {
            int max = 380;                      //- controlls how far down the view will extend to
            index = list.SelectedIndex;         //- get the current selected index of the list box
                                                //
            if (index == pindex)                //T if the current index is equal to the previous index
            {                                   //|
                if (view.Nodes.Count > 0)       //|-T if there is a t least one update written onscreen
                {                               //| |
                    view.Nodes.Clear();         //|-|--- clear the view
                }                               //|-|e)
                                                //|
                ReadUpdate(fileName, view);     //|-- update the view with the updates
                                                //|
                while (view.Height < max)       //|-T while the height of the view is not at the max distance
                {                               //| |
                    if (view.Height < max / 2)    //|-|--T if the height of the view is not at the half point
                    {                           //| |  |
                        view.Height += 2;       //|-|--|---- increase the height by 2 per tick
                    }
                    else
                    {                    //|-|--\c) otherwise
                        view.Height++;          //|-|--|---- increase the height by only 1 per tick (slow down)
                    }                           //|-|--|e)
                }                               //|-|L)
            }
            else
            {                            //\c) otherwise if the current index is not the same as the previous index
                while (view.Height > 1)         //|-T while the height of the view is not at minimum
                {                               //| |
                    if (view.Height > max / 2)    //|-|--T if the view height is not at the half point
                    {                           //| |  |
                        view.Height -= 2;       //|-|--|---- decrease the height by 2 per tick
                    }
                    else
                    {                    //|-|--\c) otherwise if at the half way point
                        view.Height--;          //|-|--|---- decrease the height by 1 per tick (slow down)
                    }                           //|-|--|e)
                                                //| |
                    if (view.Height == 2)       //|-|--T if at height of 2
                    {                           //| |  |
                        view.Nodes.Clear();     //|-|--|---- clear the list (so that the previous text does appear on the new text)
                    }                           //|-|--|e)
                                                //| |
                }                               //|-|L)
                                                //|
                ReadUpdate(fileName, view);     //|-- update the view with the updates
                                                //|
                while (view.Height < max)       //|-T while not at max height
                {                               //| |
                    if (view.Height < max / 2)    //|-|--T if at half way point
                    {                           //| |  |
                        view.Height += 2;       //|-|--|---- increament the height by 2 per tick
                    }
                    else
                    {                    //|-|--\c) otherwise
                        view.Height++;          //|-|--|---- increament the height by 1 per tick (slow down)
                    }                           //|-|--|e)
                }                               //|-|L)
                                                //|
            }                                   //|e)
            pindex = list.SelectedIndex;        //- set the previous index to the current one

        }

        /// <summary>
        /// Updates the main window of the update's text, according to the selected item of the list box
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="view"></param>
        private void ReadUpdate(string FileName, TreeView view)
        {
            StreamReader stream = File.OpenText(FileName);                                                  //- open the text file's stream
            string data = stream.ReadLine();                                                                                //- read the first line in the file
            TreeNode iNode = null;                                                                                          //- stores the inner nodes
            bool atCorrectPoint = false;                                                                                    //- stores if the header is at in the correct '\s'
            bool wraparound = false;                                                                                        //- wrap around bugfix
            int leC = 0;                                                                                                    //- store the amount of times a '\e' appears in an inner or inner inner node
                                                                                                                            //
            while (!stream.EndOfStream)                                                                                     //T while not at the end of the update file
            {                                                                                                               //|
                if (!data.StartsWith("//"))
                {
                    wraparound = false;                                                                                         //|-- reset wraparound
                    if (!data.Contains(@"\s") && !data.Contains(@"\e") && !data.Contains(@"\i") && atCorrectPoint)              //|-T if the line doesn't contain any specail delimeters and the header is in the correct possition
                    {                                                                                                           //|-| then consider the line as normal text
                        view.Nodes.Add(data);                                                                               //|-|--- add the line to the (node) list
                                                                                                                            //| |
                    }
                    else if (data.Contains(@"\s") && data == listBoxAll.Text + @"\s")
                    {                                       //|-\c)otherwise if it is a group title and the line is the selected version
                        atCorrectPoint = true;                                                                              //|-|--- set that the header is at the correct possition
                                                                                                                            //| |
                    }
                    else if (data.Contains(@"\i") && atCorrectPoint)
                    {                                                        //|-\c)otherwise if it is a node and the header is in the correct possiton
                        iNode = view.Nodes.Add(data.Remove((data.Length - 2), 2));                                              //|-|--- add the main node
                        wraparound = false;                                                                                     //|-|--- reset wraparound
                        while (true)                                                                                            //|-|--T Oh my, an infinite loop!   INF
                        {                                                                                                       //| |  |
                            if (data.StartsWith("//"))
                            {
                                data = stream.ReadLine();
                                wraparound = true;

                            }
                            else
                            {

                                if (!wraparound)                                                                                    //|-|--|---T if not wrapping around
                                {                                                                                                   //| |  |   |
                                    data = stream.ReadLine();                                                                       //|-|--|---|----- read the next line
                                }                                                                                                   //| |  |   |e)
                                wraparound = false;                                                                                 //|-|--|---- reset wrap around

                                if (data.Contains(@"\i"))
                                {                                                                         //|-|--|---T if the line is another node
                                    TreeNode[] iiNode = new TreeNode[1];                                                        //|-|--|---|----- Prepare to hold multiple inner nodes in this inner node (how meta) as iiNode
                                    iiNode[0] = iNode.Nodes.Add(data.Remove((data.Length - 2), 2));                             //|-|--|---|----- add the inner main node
                                    while (true)                                                                                    //|-|--|---|----T INF loop
                                    {                                                                                               //| |  |   |    |
                                        bool comment2 = false;
                                        if (data.StartsWith("//"))
                                        {
                                            data = stream.ReadLine();
                                            comment2 = true;

                                        }
                                        else
                                        {

                                            int i = 1;                                                                                  //|-|--|---|----|------ prepare to count loops
                                            if (!comment2)
                                            {
                                                data = stream.ReadLine();                                                                   //|-|--|---|----|------ read the next line
                                            }
                                            //| |  |   |    |
                                            if (data.Contains(@"\i"))                                                                   //|-|--|---|----|-----T if the line is yet ANOTHER node
                                            {                                                                                           //| |  |   |    |     |
                                                Array.Resize(ref iiNode, iiNode.Length + 1);                                            //|-|--|---|----|-----|------- resize iiNode[] by making it 1 larger
                                                iiNode[1] = iiNode[0].Nodes.Add(data.Remove((data.Length - 2), 2));                     //|-|--|---|----|-----|------- add the inner inner main node to the inner main node
                                                while (true)                                                                            //|-|--|---|----|-----|------T INF loop
                                                {                                                                                       //| |  |   |    |     |      |
                                                    bool comment3 = false;
                                                    if (data.StartsWith("//"))
                                                    {
                                                        data = stream.ReadLine();
                                                        comment3 = true;

                                                    }
                                                    else
                                                    {
                                                        if (!comment3)
                                                        {
                                                            data = stream.ReadLine();                                                           //|-|--|---|----|-----|------|-------- read the next line
                                                        }

                                                        if (data.Contains(@"\i"))                                                           //|-|--|---|----|-----|------|-------T if there is YER ANOTHER NODE
                                                        {                                                                                   //| |  |   |    |     |      |       |
                                                            Array.Resize(ref iiNode, iiNode.Length + 1);                                    //|-|--|---|----|-----|------|-------|--------- resize iiNode[] by making it 1 larger
                                                            iiNode[i + 1] = iiNode[i].Nodes.Add(data.Remove((data.Length - 2), 2));         //|-|--|---|----|-----|------|-------|--------- add the inner inner inner main node
                                                            i++;                                                                            //|-|--|---|----|-----|------|-------|--------- count loop for any further inner node additions
                                                        }                                                                                   //| |  |   |    |     |      |       |e)
                                                                                                                                            //| |  |   |    |     |      |
                                                        if (leC < iiNode.Length - 2 && data.Contains(@"\e"))
                                                        {                                //|-|--|---|----|-----|------|-------T if local count of '\e' is less than the amount of nodes-2 and the current line is '\e' (enforces that \e must be present for each \i)
                                                            leC++;                                                                          //|-|--|---|----|-----|------|-------|-------- itterate local count of '\e'
                                                            i--;                                                                            //|-|--|---|----|-----|------|-------|-------- step back up one node
                                                        }
                                                        else if (data.Contains(@"\e"))
                                                        {                                                  //|-|--|---|----|-----|------|-------\c) otherwise if the current line is '\e' (where at the last 2)
                                                            data = stream.ReadLine();                                                       //|-|--|---|----|-----|------|-------|-------- read the next line
                                                            leC = 0;                                                                        //|-|--|---|----|-----|------|-------|-------- reset local '\e' count
                                                            break;                                                                          //|-|--|---|----|-----|------|>>>>>>>|>>>>>>>> BREAK;
                                                        }                                                                                   //| |  |   |    |     |      |       |e)
                                                                                                                                            //| |  |   |    |     |      |               
                                                        if (!data.Contains(@"\i") && !data.Contains(@"\e"))                                 //|-|--|---|----|-----|------|-------T if the current line isn't a new node nor an end (!'\i' nor '\e')
                                                        {                                                                                   //| |  |   |    |     |      |       |
                                                            iiNode[i].Nodes.Add(data);                                                      //|-|--|---|----|-----|------|-------|-------- add the inner inner inner nodes...
                                                        }                                                                                   //|-|--|---|----|-----|------|-------|e)
                                                    }
                                                }                                                                                       //|-|--|---|----|-----|------|INF)
                                            }                                                                                           //|-|--|---|----|-----|e)
                                                                                                                                        //| |  |   |    |
                                            if (data.Contains(@"\e"))                                                                   //|-|--|---|----|-----T if the current line is an end ('\e')
                                            {                                                                                           //| |  |   |    |     |
                                                data = stream.ReadLine();                                                               //|-|--|---|----|-----|------- read the next line
                                                break;                                                                                  //|-|--|---|----|>>>>>|>>>>>>> BREAK;
                                            }                                                                                           //|-|--|---|----|-----|e)
                                                                                                                                        //| |  |   |    |     
                                            if (!data.Contains(@"\i") && !data.Contains(@"\e") && !data.StartsWith("//"))                                         //|-|--|---|----|-----T if the current line is a normal line
                                            {                                                                                           //| |  |   |    |     |
                                                iiNode[0].Nodes.Add(data);                                                              //|-|--|---|----|-----|------- add the inner inner nodes
                                            }                                                                                           //|-|--|---|----|-----|e)
                                        }

                                    }                                                                                               //|-|--|---|----|INF)
                                }                                                                                                   //|-|--|---|e)
                                                                                                                                    //| |  |
                                if (data.Contains(@"\e"))                                                                           //|-|--|---T if the current line is the end line
                                {                                                                                                   //| |  |   |
                                    data = stream.ReadLine();                                                                       //|-|--|---|----- read the next line
                                    wraparound = true;                                                                              //|-|--|---|----- activate wraparound
                                    break;                                                                                          //|-|--|>>>|>>>>> BREAK;
                                }                                                                                                   //|-|--|---|e)
                                                                                                                                    //| |  |
                                if (!data.Contains(@"\i") && !data.Contains(@"\e") && !data.StartsWith("//"))                                                 //|-|--|---T if the current line is a normal line
                                {                                                                                                   //| |  |   |
                                    iNode.Nodes.Add(data);//add the inner nodes                                                     //|-|--|---|----- add the inner nodes
                                }                                                                                                   //| |  |   |e)
                                                                                                                                    //| |  |
                                if (data.Contains(@"\i"))
                                {                                                                         //|-|--|---T if the current line is a new node
                                    wraparound = true;                                                                              //|-|--|---|----- activate wraparound
                                }                                                                                                   //|-|--|---|e)
                            }

                        }                                                                                                       //|-|--|INF)
                    }                                                                                                           //|-|e)
                                                                                                                                //|
                    if (data.Contains(@"\e") && atCorrectPoint)                                                                 //|-T if the current line is an end and the head is at the correct possition
                    { break; }                                                                                                  //|>|e)> BREAK;
                                                                                                                                //|
                    if (!wraparound)                                                                                            //|-T if wraparound is not active
                    {                                                                                                           //| |
                        data = stream.ReadLine();                                                                               //|-|--- read the next line
                    }                                                                                                           //|-|e)
                }
                else
                {
                    data = stream.ReadLine();

                }

            }                                                                                                               //|L)
            stream.Close();                                                                                                 //- close the file
        }

        private void NextBtn_Click(object sender, EventArgs e)
        {
            searchIndex++;
            searchTxtBx_TextChanged(searchTxtBx, EventArgs.Empty);
            //PreviousBtn.Text = searchIndex.ToString();
        }

        private void PreviousBtn_Click(object sender, EventArgs e)
        {
            if (searchIndex > 0)
            {
                searchIndex--;
                searchTxtBx_TextChanged(searchTxtBx, EventArgs.Empty);
            }
        }

        private void clearTextBtn_Click(object sender, EventArgs e)
        {
            searchTxtBx.Text = "Search";
            searchTxtBx.ForeColor = Color.FromName("ControlDark");
            searchIndex = 0;
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

            }
        }

        private void searchTxtBx_TextChanged(object sender, EventArgs e)
        {
            if (searchTxtBx.Text != "")
            {
                ushort tempSI = searchIndex;

                if (addCount > 0)
                {
                    for (int i = 0; i < tabControler.TabCount; i++)
                    {
                        if (tabControler.GetControl(i).Text.ToLower().Contains(searchTxtBx.Text.ToLower()))
                        {
                            if (tempSI == 0)
                            {
                                tabControler.SelectTab(i);
                                searchTxtBx.Focus();
                                searchTxtBx.SelectionStart = searchTxtBx.Text.Length;
                                break;
                            }
                            else
                            {
                                tempSI--;
                            }
                        }

                        if (searchIndex != 0 && i == tabControler.TabCount - 1)
                        {
                            searchIndex--;
                            //have this set to 0 instead of decrementing to enable wraparound
                        }

                    }

                }
            }

        }

    }
}
