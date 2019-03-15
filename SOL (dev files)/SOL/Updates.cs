using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SOL
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
            addCount = Toolbox.addCount;//grab the amount of installed additions from the toolbox

            if (Environment.CurrentDirectory.Contains(@"\Debug"))
            {//if in debug mode
                path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location).TrimEnd(@"\bin\Debug".ToCharArray()) + @"\Resources\Updates\";
            }
            else
            {
                path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"Resources\Updates\";
            }

            this.Icon = new Icon(path + @"\..\SOLICO.ico");
            NextBtn.BackgroundImage = Image.FromFile(path + @"\..\arrow.ico");
            clearTextBtn.BackgroundImage = Image.FromFile(path + @"\..\clear.ico");

            var tmpImg = Image.FromFile(path + @"\..\arrow.ico");
            tmpImg.RotateFlip(RotateFlipType.Rotate180FlipNone);
            PreviousBtn.BackgroundImage = tmpImg;

            files = Directory.GetFiles(path, "*.txt");                                     //- get every file in the resources\updates folder
            primF = files.Length;

            if (addCount > 0)
            {
                Array.Resize(ref files, files.Length + 1);
                files[files.Length - 1] = path + @"\.brk";//add a break point between SOL updates and addtion updates
                primF = files.Length;

                System.Collections.ObjectModel.ReadOnlyCollection<string> adds = Microsoft.VisualBasic.FileIO.FileSystem.GetDirectories(path + @"\..\Additions\");
                foreach (string add in adds)
                {
                    Array.Resize(ref files, files.Length+1);//make room for the extra update from the addition
                    string[] pathSplit = add.Split(Convert.ToChar(92));
                    files[files.Length-1] = add + @"\" + pathSplit[pathSplit.Length - 1]
                        + @"\" + pathSplit[pathSplit.Length - 1] + @"\Resources\"
                        + pathSplit[pathSplit.Length - 1] + ".txt";

                    //handling inner additions
                    //if (Directory.Exists(files[files.Length - 1] + @"\..\Additions"))
                    //{
                    //    System.Collections.ObjectModel.ReadOnlyCollection<string> addsInner =
                    //    Microsoft.VisualBasic.FileIO.FileSystem.GetDirectories(files[files.Length - 1] + @"\..\Additions");

                    //    foreach (string addI in addsInner) {
                    //        Array.Resize(ref files, files.Length + 1);//make room for the extra update from the addition
                    //        pathSplit = addI.Split(Convert.ToChar(92));
                    //        files[files.Length - 1] = addI + @"\" + pathSplit[pathSplit.Length - 1]
                    //            + @"\" + pathSplit[pathSplit.Length - 1] + @"\Resources\"
                    //            + pathSplit[pathSplit.Length - 1] + ".txt";

                    //        //duplicate if statement
                    //    }
                    //}

                    //need to add support for infinite depth additions if going to ever use

                }

            }                                   
                                                                                                    //
            foreach (string file in files) {                                                        //T target each file seperetly
                string[] pathSplit = file.Split(Convert.ToChar(92));                                //|-- split the current file by \ (the last entry in the array is the file name)
                int tl = pathSplit[pathSplit.Length - 1].Length;                                    //|-- get the length of the tab name
                tabControler.TabPages.Add(pathSplit[pathSplit.Length -1].Remove(tl - 4, 4));        //|-- add the tab (without the .txt extention)
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
                                                                                                            
        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)                              
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
                        listBoxAll.ForeColor = Color.FromArgb(0, 0, 0);
                        UpdateList(path + tabControler.SelectedTab.Name + ".txt", listBoxAll);      //|-- update the list
                    }
                    else {
                        UpdateList(files[tabControler.SelectedIndex], listBoxAll);      //|-- update the list
                    }
                }
                                                                            //|
            }                                                               //|e)

        }

        private void clearTextBtn_Click(object sender, EventArgs e)
        {
            searchTxtBx.Text = "Search";
            searchTxtBx.ForeColor = Color.FromName("ControlDark");
            searchIndex = 0;

        }

        private void SearchTxtBx_MouseClick(object sender, MouseEventArgs e)
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

        private void SearchTxtBx_Leave(object sender, EventArgs e)
        {
            if (searchTxtBx.Text.Length <= 0)
            {
                searchTxtBx.Text = "Search";
                searchTxtBx.ForeColor = Color.FromName("ControlDark");

            }
        }

        private void SearchTxtBx_TextChanged(object sender, EventArgs e)
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

        private void NextBtn_Click(object sender, EventArgs e)
        {
            searchIndex++;
            SearchTxtBx_TextChanged(searchTxtBx, EventArgs.Empty);
            //PreviousBtn.Text = searchIndex.ToString();
        }

        private void PreviousBtn_Click(object sender, EventArgs e)
        {
            if (searchIndex > 0)
            {
                searchIndex--;
                SearchTxtBx_TextChanged(searchTxtBx, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Update the target list box with the file names' contents, 
        /// only searching for "\s" delemetres and ensuring the file is structured correctly
        /// </summary>
        /// <param name="fileFullPath"></param>
        /// <param name="list"></param>
        private void UpdateList(string fileFullPath, ListBox list) {
            ushort sC = 0;                                                                          //- '\s' count
            ushort iC = 0;                                                                          //- '\i' count
            ushort eC = 0;                                                                          //- '\e' count
            StreamReader stream = File.OpenText(fileFullPath);                                      //- open the text file's stream
            string data = stream.ReadLine();                                                        //- read the first line of the file
                                                                                                    //
            while (!stream.EndOfStream)                                                             //T while the stream is not at the end of the file
            {                                                                                       //|
                if (data.Contains(@"\s") && !data.Contains(@"\tc"))                                 //|-T if the line is a version
                {                                                                                   //| |
                    string tmp = data;

                    while (tmp.Contains(@"@\s"))
                    {
                        int lit = tmp.IndexOf(@"@\s");

                        tmp = tmp.Remove(lit, 1);
                        

                    }

                    if (tmp.Contains(@"\s")) {
                        list.Items.Add(tmp.Remove(tmp.IndexOf(@"\s"), 2));                            //|-|--- add the version to the list (without the '\s' present)
                        sC++;                                                                           //|-|--- count the amount of times a version delemeter is found

                    }
                } else if (data.Contains(@"\i")) {                                                  //|-\c)  otherwise if the delemeter is a node
                    string tmp = data;

                    while (tmp.Contains(@"@\i"))
                    {
                        int lit = tmp.IndexOf(@"@\i");

                        tmp = tmp.Remove(lit, 3);
                        

                    }

                    if (tmp.Contains(@"\i"))
                    {
                        iC++;                                                                           //|-|--- count the amount of times a node delemeter is found

                    }
                }                                                                                   //|-|e)
                                                                                                    //|
                data = stream.ReadLine();                                                           //|-- read the next line
                                                                                                    //|
                if (data.Contains(@"\e"))                                                           //|-T if the current delemeter is an end line
                {                                                                                   //| |
                    string tmp = data;

                    while (tmp.Contains(@"@\e"))
                    {
                        int lit = tmp.IndexOf(@"@\e");

                        tmp = tmp.Remove(lit, 3);
                        

                    }

                    if (tmp.Contains(@"\e"))
                    {
                        eC++;                                                                           //|-|--- count the amount of times an end delemeter is found
                    }
                }                                                                                   //|-|e)

                if (data.Contains(@"\tc("))
                {
                    string tmp = data;

                    while (tmp.Contains(@"@\tc("))
                    {
                        int lit = tmp.IndexOf(@"@\tc(");

                        tmp = tmp.Remove(lit, 4);
                        

                    }

                    if (tmp.Contains(@"\tc("))
                    {
                        int[] values = new int[3];
                        bool[] parse = new bool[3];
                        int i = 0;

                        foreach (string val in data.Remove(0, 4).Replace(")", "").Replace(" ", "").Split(','))
                        {
                            parse[i] = int.TryParse(val, out values[i]);
                            i++;
                            if (i == 3) { break; }//force the system to only ever read 3 values

                        }

                        listBoxAll.ForeColor = Color.FromArgb(values[0], values[1], values[2]);
                    }
                }

            }                                                                                       //|L)
            stream.Close();                                                                         //- close the file
                                                                                                    //
            if (sC + iC != eC) {                                                                    //T if amount special delemeters does not match the amount of end delemeters
                MessageBox.Show("An error occured while attempting to read the text file \""        //|
                    + fileFullPath + "\"; the amount of ending delemeters (" + eC                       //|
                    + ") does not match the amount of special delemeters (" + (sC + iC) + ")."      //|
                    , "File read: delemeters not equal error");                                     //|-- warn the user/developer
                Close();                                                                            //|-- end the process
            }                                                                                       //|e)

            if (sC == 0 || eC == 0) {                                                               //T if there is a missing delemeter
                MessageBox.Show("An error occured while attempting to read the text file \""        //|
                    + fileFullPath + "\"; one or more delemeters are missing " +                    //|
                    "(all of these must not be 0), title count = " + sC                             //|
                    + " end node count = " + eC, @"File read: no delemeter\s");                     //|-- warn the user/developer
                Close();                                                                            //|-- end the process
            }                                                                                       //|e)

            listBoxAll.SelectedIndex = 0;
        }
        
        /// <summary>
        /// Controlls the entire forms animations
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="view"></param>
        /// <param name="list"></param>
        private void UpdateView(string fileName, TreeView view, ListBox list) {
            int max = 355;                      //- controlls how far down the view will extend to
            index = list.SelectedIndex;         //- get the current selected index of the list box
                                                //
            if (index == pindex)                //T if the current index is equal to the previous index
            {                                   //|
                if (view.Nodes.Count > 0)       //|-T if there is a t least one update written onscreen
                {                               //| |
                    view.Nodes.Clear();         //|-|--- clear the view
                }                               //|-|e)
                                                //|
                if (view.Height < max)
                {
                    treeViewAll.BackColor = Color.FromArgb(224, 224, 224);
                }

                ReadUpdate(fileName, view);     //|-- update the view with the updates
                                                //|
                while (view.Height < max)       //|-T while the height of the view is not at the max distance
                {                               //| |
                    if (view.Height < max/2)    //|-|--T if the height of the view is not at the half point
                    {                           //| |  |
                        view.Height += 2;       //|-|--|---- increase the height by 2 per tick
                    } else {                    //|-|--\c) otherwise
                        view.Height++;          //|-|--|---- increase the height by only 1 per tick (slow down)
                    }                           //|-|--|e)
                }                               //|-|L)
            } else {                            //\c) otherwise if the current index is not the same as the previous index
                while (view.Height > 1)         //|-T while the height of the view is not at minimum
                {                               //| |
                    if (view.Height > max/2)    //|-|--T if the view height is not at the half point
                    {                           //| |  |
                        view.Height -= 2;       //|-|--|---- decrease the height by 2 per tick
                    } else {                    //|-|--\c) otherwise if at the half way point
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
                if (view.Height < max)
                {
                    treeViewAll.BackColor = Color.FromArgb(224, 224, 224);
                }

                ReadUpdate(fileName, view);     //|-- update the view with the updates
                                                //|
                while (view.Height < max)       //|-T while not at max height
                {                               //| |
                    if (view.Height < max/2)    //|-|--T if at half way point
                    {                           //| |  |
                        view.Height += 2;       //|-|--|---- increament the height by 2 per tick
                    } else {                    //|-|--\c) otherwise
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
        private void ReadUpdate(string FileName, TreeView view) {
            StreamReader stream = File.OpenText(FileName);                                                  //- open the text file's stream
            string data = stream.ReadLine();                                                                                //- read the first line in the file
            TreeNode iNode = null;                                                                                          //- stores the inner nodes
            bool atCorrectPoint = false;                                                                                    //- stores if the header is at in the correct '\s'
            bool wraparound = false;                                                                                        //- wrap around bugfix
            int leC = 0;                                                                                                    //- store the amount of times a '\e' appears in an inner or inner inner node
            uint poss = 1;
            string tmp = data;
            string tmpSan = data;
                                                                                                                            //
            while (!stream.EndOfStream)                                                                                     //T while not at the end of the update file
            {                                                                                                               //|
                tmp = data;

                while (tmp.Contains(@"@\s") || tmp.Contains(@"@\bc"))
                {
                    int lit = tmp.IndexOf(@"@\s");
                    int lit2 = tmp.IndexOf(@"@\bc");

                    if (lit != -1)
                    {
                        tmp = tmp.Remove(lit, 1);
                    }
                    else if (lit2 != -1)
                    {
                        tmp = tmp.Remove(lit2, 3);

                    }
                    else
                    {
                        int dud = 0;
                        //
                    }

                }

                if (!data.StartsWith("//") && !tmp.StartsWith(@"\bc(") && ((tmp.Contains(@"\s") && tmp == listBoxAll.Text + @"\s") || atCorrectPoint ))
                {
                    wraparound = false;                                                                                         //|-- reset wraparound

                    tmp = data;

                    while (tmp.Contains(@"@\s") || tmp.Contains(@"@\e") || tmp.Contains(@"@\i"))
                    {
                        int lit  = tmp.IndexOf(@"@\s");
                        int lit2 = tmp.IndexOf(@"@\e");
                        int lit3 = tmp.IndexOf(@"@\i");

                        if (lit != -1)
                        {
                            tmp = tmp.Remove(lit, 1);
                            //data = data.Remove(data.IndexOf("@"),1);
                        }
                        else if (lit2 != -1)
                        {
                            tmp = tmp.Remove(lit2, 3);
                            //

                        }
                        else if (lit3 != -1)
                        {
                            tmp = tmp.Remove(lit3, 3);
                            //
                        }
                        else {
                            int dud = 0;
                            //
                        }

                    }

                    if (!tmp.Contains(@"\s") && !tmp.Contains(@"\e") && !tmp.Contains(@"\i") && atCorrectPoint)              //|-T if the line doesn't contain any specail delimeters and the header is in the correct possition
                    {                                                                                                           //|-| then consider the line as normal text
                        tmpSan = data;

                        while (tmpSan.Contains(@"@\")) {
                            tmpSan = tmpSan.Remove(tmpSan.IndexOf("@"), 1);

                        }

                        TreeNode newNode = view.Nodes.Add(tmpSan);                                                                               //|-|--- add the line to the (node) list
                        tmp = data;

                        while (tmp.Contains(@"@\"))
                        {
                            int lit = tmp.IndexOf(@"@\");

                            tmp = tmp.Remove(lit, 2);
                            

                        }

                        if (newNode.Text.Contains("//"))
                        {
                            newNode.Text = newNode.Text.Remove(newNode.Text.IndexOf("//"));

                        }

                        if (tmp.Contains(@"\nbc("))
                        {
                            ColourNodes(newNode, poss);

                        }

                        if (tmp.Contains(@"\ntc("))
                        {
                            ColourNodesText(newNode, poss);

                        }
                        //| |
                    }
                    else if (tmp.Contains(@"\s") && tmp == listBoxAll.Text + @"\s")
                    {                                       //|-\c)otherwise if it is a group title and the line is the selected version
                        tmp = data;

                        while (tmp.Contains(@"@\s"))
                        {
                            int lit = tmp.IndexOf(@"@\s");

                            tmp = tmp.Remove(lit, 3);
                            

                        }

                        if (tmp.Contains(@"\s"))
                        {
                            atCorrectPoint = true;                                                                              //|-|--- set that the header is at the correct possition
                                                                                                                                //| |
                        }
                    }
                    else if (tmp.Contains(@"\i") && atCorrectPoint)
                    {                                                        //|-\c)otherwise if it is a node and the header is in the correct possiton
                        tmpSan = tmp.Remove(tmp.IndexOf(@"\i", 2));

                        while (tmpSan.Contains(@"@\"))
                        {
                            tmpSan = tmpSan.Remove(tmpSan.IndexOf("@"), 1);

                        }

                        iNode = view.Nodes.Add(tmpSan);                                              //|-|--- add the main node
                        tmp = data;

                        while (tmp.Contains(@"@\i"))
                        {
                            int lit = tmp.IndexOf(@"@\i");

                            tmp = tmp.Remove(lit, 3);
                            

                        }

                        if (iNode.Text.Contains("//"))
                        {
                            iNode.Text = iNode.Text.Remove(iNode.Text.IndexOf("//"));

                        }

                        while (tmp.Contains(@"@\"))
                        {
                            int lit = tmp.IndexOf(@"@\");

                            tmp = tmp.Remove(lit, 2);
                            

                        }

                        if (tmp.Contains(@"\nbc("))
                        {
                            ColourNodes(iNode, poss);

                        }

                        if (tmp.Contains(@"\ntc("))
                        {
                            ColourNodesText(iNode, poss);

                        }

                        wraparound = false;                                                                                     //|-|--- reset wraparound
                        while (true)                                                                                            //|-|--T Oh my, an infinite loop!   INF
                        {                                                                                                       //| |  |
                            if (data.StartsWith("//"))
                            {
                                data = stream.ReadLine();
                                poss++;
                                wraparound = true;

                            }
                            else
                            {

                                if (!wraparound)                                                                                    //|-|--|---T if not wrapping around
                                {                                                                                                   //| |  |   |
                                    data = stream.ReadLine();                                                                       //|-|--|---|----- read the next line
                                    poss++;
                                }                                                                                                   //| |  |   |e)
                                wraparound = false;                                                                                 //|-|--|---- reset wrap around

                                tmp = data;

                                while (tmp.Contains(@"@\i"))
                                {
                                    int lit = tmp.IndexOf(@"@\i");

                                    tmp = tmp.Remove(lit, 3);
                                    

                                }

                                if (tmp.Contains(@"\i"))
                                {                                                                         //|-|--|---T if the line is another node
                                    int index = tmp.IndexOf(@"\i");
                                    TreeNode[] iiNode = new TreeNode[1];                                                        //|-|--|---|----- Prepare to hold multiple inner nodes in this inner node (how meta) as iiNode
                                    tmpSan = tmp.Remove(index, 2);

                                    while (tmpSan.Contains(@"@\"))
                                    {
                                        tmpSan = tmpSan.Remove(tmpSan.IndexOf("@"), 1);

                                    }

                                    iiNode[0] = iNode.Nodes.Add(tmpSan);                             //|-|--|---|----- add the inner main node

                                    if (iiNode[0].Text.Contains("//"))
                                    {
                                        iiNode[0].Text = iiNode[0].Text.Remove(iiNode[0].Text.IndexOf("//"));

                                    }

                                    tmp = data;

                                    while (tmp.Contains(@"@\"))
                                    {
                                        int lit = tmp.IndexOf(@"@\");

                                        tmp = tmp.Remove(lit, 2);
                                        

                                    }

                                    if (tmp.Contains(@"\nbc("))
                                    {
                                        ColourNodes(iiNode[0], poss);

                                    }

                                    if (tmp.Contains(@"\ntc("))
                                    {
                                        ColourNodesText(iiNode[0], poss);

                                    }

                                    while (true)                                                                                    //|-|--|---|----T INF loop
                                    {                                                                                               //| |  |   |    |
                                        bool comment2 = false;
                                        if (data.StartsWith("//"))
                                        {
                                            data = stream.ReadLine();
                                            poss++;
                                            comment2 = true;

                                        }
                                        else
                                        {

                                            int i = 1;                                                                                  //|-|--|---|----|------ prepare to count loops
                                            if (!comment2)
                                            {
                                                data = stream.ReadLine();                                                                   //|-|--|---|----|------ read the next line
                                                poss++;
                                            }
                                            //| |  |   |    |
                                            tmp = data;

                                            while (tmp.Contains(@"@\i"))
                                            {
                                                int lit = tmp.IndexOf(@"@\i");

                                                tmp = tmp.Remove(lit, 3);
                                                

                                            }

                                            if (tmp.Contains(@"\i"))                                                                   //|-|--|---|----|-----T if the line is yet ANOTHER node
                                            {                                                                                           //| |  |   |    |     |
                                                index = tmp.IndexOf(@"\i");
                                                Array.Resize(ref iiNode, iiNode.Length + 1);                                            //|-|--|---|----|-----|------- resize iiNode[] by making it 1 larger
                                                tmpSan = tmp.Remove(index, 2);

                                                while (tmpSan.Contains(@"@\"))
                                                {
                                                    tmpSan = tmpSan.Remove(tmpSan.IndexOf("@"), 1);

                                                }

                                                iiNode[1] = iiNode[0].Nodes.Add(tmpSan);                     //|-|--|---|----|-----|------- add the inner inner main node to the inner main node

                                                while (true)                                                                            //|-|--|---|----|-----|------T INF loop
                                                {                                                                                       //| |  |   |    |     |      |
                                                    bool comment3 = false;
                                                    if (data.StartsWith("//"))
                                                    {
                                                        data = stream.ReadLine();
                                                        poss++;
                                                        comment3 = true;

                                                    }
                                                    else
                                                    {
                                                        if (!comment3)
                                                        {
                                                            data = stream.ReadLine();                                                           //|-|--|---|----|-----|------|-------- read the next line
                                                            poss++;
                                                        }

                                                        tmp = data;

                                                        while (tmp.Contains(@"@\i"))
                                                        {
                                                            int lit = tmp.IndexOf(@"@\i");

                                                            tmp = tmp.Remove(lit, 3);
                                                            

                                                        }

                                                        if (tmp.Contains(@"\i"))                                                           //|-|--|---|----|-----|------|-------T if there is YER ANOTHER NODE
                                                        {                                                                                   //| |  |   |    |     |      |       |
                                                            index = tmp.IndexOf(@"\i");
                                                            Array.Resize(ref iiNode, iiNode.Length + 1);                                    //|-|--|---|----|-----|------|-------|--------- resize iiNode[] by making it 1 larger
                                                            tmpSan = tmp.Remove(index, 2);

                                                            while (tmpSan.Contains(@"@\"))
                                                            {
                                                                tmpSan = tmpSan.Remove(tmpSan.IndexOf("@"), 1);

                                                            }

                                                            iiNode[i + 1] = iiNode[i].Nodes.Add(tmpSan);         //|-|--|---|----|-----|------|-------|--------- add the inner inner inner main node

                                                            i++;                                                                            //|-|--|---|----|-----|------|-------|--------- count loop for any further inner node additions
                                                        }                                                                                   //| |  |   |    |     |      |       |e)
                                                                                                                                            //| |  |   |    |     |      |
                                                        tmp = data;

                                                        while (tmp.Contains(@"@\e"))
                                                        {
                                                            int lit = tmp.IndexOf(@"@\e");

                                                            tmp = tmp.Remove(lit, 3);
                                                            

                                                        }

                                                        if (leC < iiNode.Length - 2 && tmp.Contains(@"\e"))
                                                        {                                //|-|--|---|----|-----|------|-------T if local count of '\e' is less than the amount of nodes-2 and the current line is '\e' (enforces that \e must be present for each \i)
                                                            leC++;                                                                          //|-|--|---|----|-----|------|-------|-------- itterate local count of '\e'
                                                            i--;                                                                            //|-|--|---|----|-----|------|-------|-------- step back up one node
                                                        }
                                                        else if (tmp.Contains(@"\e"))
                                                        {                                                  //|-|--|---|----|-----|------|-------\c) otherwise if the current line is '\e' (where at the last 2)
                                                            data = stream.ReadLine();                                                       //|-|--|---|----|-----|------|-------|-------- read the next line
                                                            poss++;
                                                            leC = 0;                                                                        //|-|--|---|----|-----|------|-------|-------- reset local '\e' count
                                                            break;                                                                          //|-|--|---|----|-----|------|>>>>>>>|>>>>>>>> BREAK;
                                                        }                                                                                   //| |  |   |    |     |      |       |e)
                                                                                                                                            //| |  |   |    |     |      |               
                                                        tmp = data;

                                                        while (tmp.Contains(@"@\i") || tmp.Contains(@"@\e"))
                                                        {
                                                            int lit = tmp.IndexOf(@"@\i");
                                                            int lit2 = tmp.IndexOf(@"@\e");

                                                            if (lit != -1)
                                                            {
                                                                tmp = tmp.Remove(lit, 3);
                                                                
                                                            }
                                                            else if (lit2 != -1)
                                                            {
                                                                tmp = tmp.Remove(lit2, 3);
                                                                

                                                            }
                                                            else {
                                                                int dud = 0;
                                                                //report an error
                                                            }

                                                        }

                                                        if (!tmp.Contains(@"\i") && !tmp.Contains(@"\e"))                                 //|-|--|---|----|-----|------|-------T if the current line isn't a new node nor an end (!'\i' nor '\e')
                                                        {                                                                                   //| |  |   |    |     |      |       |

                                                            tmpSan = data;

                                                            while (tmpSan.Contains(@"@\"))
                                                            {
                                                                tmpSan = tmpSan.Remove(tmpSan.IndexOf("@"), 1);

                                                            }

                                                            iiNode[i].Nodes.Add(tmpSan);                                                      //|-|--|---|----|-----|------|-------|-------- add the inner inner inner nodes...

                                                            if (iiNode[i].Nodes[iiNode[i].Nodes.Count - 1].Text.Contains("//"))
                                                            {
                                                                iiNode[i].Nodes[iiNode[i].Nodes.Count - 1].Text =
                                                                iiNode[i].Nodes[iiNode[i].Nodes.Count - 1].Text.Remove(iiNode[i].Nodes[iiNode[i].Nodes.Count - 1].Text.IndexOf("//"));

                                                            }

                                                            tmp = data;

                                                            while (tmp.Contains(@"@\nbc") || tmp.Contains(@"@\ntc"))
                                                            {
                                                                int lit = tmp.IndexOf(@"@\nbc");
                                                                int lit2 = tmp.IndexOf(@"@\ntc");

                                                                if (lit != -1)
                                                                {
                                                                    tmp = tmp.Remove(lit, 3);
                                                                    
                                                                }
                                                                else if (lit2 != -1)
                                                                {
                                                                    tmp = tmp.Remove(lit2, 3);
                                                                    

                                                                }
                                                                else
                                                                {
                                                                    int dud = 0;
                                                                    //report an error
                                                                }

                                                            }

                                                            if (tmp.Contains(@"\nbc("))
                                                            {
                                                                ColourNodes(iiNode[i].Nodes, poss);

                                                            }

                                                            if (tmp.Contains(@"\ntc("))
                                                            {
                                                                ColourNodesText(iiNode[i].Nodes, poss);

                                                            }

                                                        }                                                                                   //|-|--|---|----|-----|------|-------|e)

                                                        foreach (var node in iiNode)
                                                        {
                                                            if (node.Text.Contains("//"))
                                                            {
                                                                node.Text = node.Text.Remove(node.Text.IndexOf("//"));

                                                            }
                                                        }

                                                        tmp = data;

                                                        while (tmp.Contains(@"@\nbc") || tmp.Contains(@"@\ntc"))
                                                        {
                                                            int lit = tmp.IndexOf(@"@\nbc");
                                                            int lit2 = tmp.IndexOf(@"@\ntc");

                                                            if (lit != -1)
                                                            {
                                                                tmp = tmp.Remove(lit, 3);
                                                                
                                                            }
                                                            else if (lit2 != -1)
                                                            {
                                                                tmp = tmp.Remove(lit2, 3);
                                                                

                                                            }
                                                            else
                                                            {
                                                                int dud = 0;
                                                                //report an error
                                                            }

                                                        }

                                                        if (tmp.Contains(@"\nbc("))
                                                        {
                                                            ColourNodes(iiNode, poss);

                                                        }

                                                        if (tmp.Contains(@"\ntc("))
                                                        {
                                                            ColourNodesText(iiNode, poss);

                                                        }

                                                    }
                                                }                                                                                       //|-|--|---|----|-----|------|INF)
                                            }                                                                                           //|-|--|---|----|-----|e)
                                                                                                                                        //| |  |   |    |
                                            tmp = data;

                                            while (tmp.Contains(@"@\e"))
                                            {
                                                int lit = tmp.IndexOf(@"@\e");

                                                tmp = tmp.Remove(lit, 3);
                                                

                                            }

                                            if (tmp.Contains(@"\e"))                                                                   //|-|--|---|----|-----T if the current line is an end ('\e')
                                            {                                                                                           //| |  |   |    |     |
                                                data = stream.ReadLine();                                                               //|-|--|---|----|-----|------- read the next line
                                                poss++;
                                                break;                                                                                  //|-|--|---|----|>>>>>|>>>>>>> BREAK;
                                            }                                                                                           //|-|--|---|----|-----|e)
                                                                                                                                        //| |  |   |    |     
                                            tmp = data;

                                            while (tmp.Contains(@"@\i") || tmp.Contains(@"@\e"))
                                            {
                                                int lit = tmp.IndexOf(@"@\i");
                                                int lit2 = tmp.IndexOf(@"@\e");

                                                if (lit != -1)
                                                {
                                                    tmp = tmp.Remove(lit, 3);
                                                    
                                                }
                                                else if (lit2 != -1)
                                                {
                                                    tmp = tmp.Remove(lit2, 3);
                                                    

                                                }
                                                else
                                                {
                                                    int dud = 0;
                                                    //report an error
                                                }

                                            }

                                            if (!tmp.Contains(@"\i") && !tmp.Contains(@"\e") && !data.StartsWith("//"))                                         //|-|--|---|----|-----T if the current line is a normal line
                                            {                                                                                           //| |  |   |    |     |
                                                tmpSan = data;

                                                while (tmpSan.Contains(@"@\"))
                                                {
                                                    tmpSan = tmpSan.Remove(tmpSan.IndexOf("@"), 1);

                                                }

                                                iiNode[0].Nodes.Add(tmpSan);                                                              //|-|--|---|----|-----|------- add the inner inner nodes

                                            }                                                                                           //|-|--|---|----|-----|e)

                                            if (iiNode[0].Text.Contains("//"))
                                            {
                                                iiNode[0].Text = iiNode[0].Text.Remove(iiNode[0].Text.IndexOf("//"));

                                            }

                                            tmp = data;

                                            while (tmp.Contains(@"@\nbc") || tmp.Contains(@"@\ntc"))
                                            {
                                                int lit = tmp.IndexOf(@"@\nbc");
                                                int lit2 = tmp.IndexOf(@"@\ntc");

                                                if (lit != -1)
                                                {
                                                    tmp = tmp.Remove(lit, 3);
                                                    
                                                }
                                                else if (lit2 != -1)
                                                {
                                                    tmp = tmp.Remove(lit2, 3);
                                                    

                                                }
                                                else
                                                {
                                                    int dud = 0;
                                                    //report an error
                                                }

                                            }

                                            if (tmp.Contains(@"\nbc("))
                                            {
                                                ColourNodes(iiNode[0].Nodes, poss);

                                            }

                                            if (tmp.Contains(@"\ntc("))
                                            {
                                                ColourNodesText(iiNode[0].Nodes, poss);

                                            }

                                        }

                                    }                                                                                               //|-|--|---|----|INF)
                                }                                                                                                   //|-|--|---|e)
                                                                                                                                    //| |  |
                                tmp = data;

                                while (tmp.Contains(@"@\e"))
                                {
                                    int lit = tmp.IndexOf(@"@\e");

                                    tmp = tmp.Remove(lit, 3);
                                    

                                }

                                if (tmp.Contains(@"\e"))                                                                           //|-|--|---T if the current line is the end line
                                {                                                                                                   //| |  |   |
                                    data = stream.ReadLine();                                                                       //|-|--|---|----- read the next line
                                    poss++;
                                    wraparound = true;                                                                              //|-|--|---|----- activate wraparound
                                    break;                                                                                          //|-|--|>>>|>>>>> BREAK;
                                }                                                                                                   //|-|--|---|e)
                                                                                                                                    //| |  |
                                tmp = data;

                                while (tmp.Contains(@"@\i") || tmp.Contains(@"@\e"))
                                {
                                    int lit = tmp.IndexOf(@"@\i");
                                    int lit2 = tmp.IndexOf(@"@\e");

                                    if (lit != -1)
                                    {
                                        tmp = tmp.Remove(lit, 3);
                                        
                                    }
                                    else if (lit2 != -1)
                                    {
                                        tmp = tmp.Remove(lit2, 3);
                                        

                                    }
                                    else
                                    {
                                        int dud = 0;
                                        //report an error
                                    }

                                }

                                if (!tmp.Contains(@"\i") && !tmp.Contains(@"\e") && !data.StartsWith("//"))                                                 //|-|--|---T if the current line is a normal line
                                {                                                                                                   //| |  |   |
                                    tmpSan = data;

                                    while (tmpSan.Contains(@"@\"))
                                    {
                                        tmpSan = tmpSan.Remove(tmpSan.IndexOf("@"), 1);

                                    }

                                    iNode.Nodes.Add(tmpSan);//add the inner nodes                                                     //|-|--|---|----- add the inner nodes

                                    foreach (TreeNode node in iNode.Nodes)
                                    {
                                        if (node.Text.Contains("//"))
                                        {
                                            node.Text = node.Text.Remove(node.Text.IndexOf("//"));

                                        }
                                    }

                                    tmp = data;

                                    while (tmp.Contains(@"@\nbc") || tmp.Contains(@"@\ntc"))
                                    {
                                        int lit = tmp.IndexOf(@"@\nbc");
                                        int lit2 = tmp.IndexOf(@"@\ntc");

                                        if (lit != -1)
                                        {
                                            tmp = tmp.Remove(lit, 3);
                                            
                                        }
                                        else if (lit2 != -1)
                                        {
                                            tmp = tmp.Remove(lit2, 3);
                                            

                                        }
                                        else
                                        {
                                            int dud = 0;
                                            //report an error
                                        }

                                    }

                                    if (tmp.Contains(@"\nbc("))
                                    {
                                        ColourNodes(iNode.Nodes, poss);

                                    }

                                    if (tmp.Contains(@"\ntc("))
                                    {
                                        ColourNodesText(iNode.Nodes, poss);

                                    }

                                }                                                                                                   //| |  |   |e)
                                                                                                                                    //| |  |
                                tmp = data;

                                if (tmp != null)
                                {
                                    while (tmp.Contains(@"@\i"))
                                    {
                                        int lit = tmp.IndexOf(@"@\i");
                                        tmp = tmp.Remove(lit, 3);
                                        

                                    }

                                    if (tmp.Contains(@"\i"))
                                    {                                                                         //|-|--|---T if the current line is a new node
                                        wraparound = true;                                                                              //|-|--|---|----- activate wraparound
                                    }                                                                                                   //|-|--|---|e)
                                }
                            }

                        }                                                                                                       //|-|--|INF)
                    }                                                                                                           //|-|e)
                                                                                                                                //|
                    tmp = data;

                    if (tmp != null)
                    {
                        while (tmp.Contains(@"@\e"))
                        {
                            int lit = tmp.IndexOf(@"@\e");

                            tmp = tmp.Remove(lit, 2);
                            

                        }

                        if (tmp.Contains(@"\e") && atCorrectPoint)                                                                 //|-T if the current line is an end and the head is at the correct possition
                        { break; }                                                                                                  //|>|e)> BREAK;
                    }
                                                                                                                                //|
                    if (!wraparound)                                                                                            //|-T if wraparound is not active
                    {                                                                                                           //| |
                        data = stream.ReadLine();                                                                               //|-|--- read the next line
                        poss++;
                    }                                                                                                           //|-|e)
                }
                else {
                    data = stream.ReadLine();
                    poss++;

                }

                tmp = data;

                if (tmp != null)
                {
                    while (tmp.Contains(@"@\bc"))
                    {
                        int lit = tmp.IndexOf(@"@\bc");

                        tmp = tmp.Remove(lit, 2);
                        

                    }

                    if (tmp.Contains(@"\bc(") && atCorrectPoint)
                    {
                        int[] values = new int[3];
                        bool[] parse = new bool[3];
                        int i = 0;

                        foreach (string val in data.Remove(0, 4).Replace(")", "").Replace(" ", "").Split(','))
                        {
                            parse[i] = int.TryParse(val, out values[i]);
                            i++;
                            if (i == 3) { break; }//force the system to only ever read 3 values

                        }

                        treeViewAll.BackColor = Color.FromArgb(values[0], values[1], values[2]);

                    }
                }

            }                                                                                                               //|L)
            stream.Close();                                                                                                 //- close the file
        }

        /// <summary>
        /// Colours the text of all child nodes in the targeted primary/secondary node according to the parameters of a \c delemeter in the nodes name
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="possition"></param>
        private void ColourNodesText(TreeNodeCollection nodes, uint possition = 0)
        {
            int index = 0;
            int[] values = new int[3];
            bool[] parse = new bool[3];
            string tmpExtra = "";

            foreach (TreeNode node in nodes)
            {
                int i = 0;
                index = node.Text.IndexOf(@"\ntc(");
                if (index != -1)
                {//if index == -1, then there is no \c delemeter found and skip it
                    if (node.Text.IndexOf(@")\") != -1)
                    {
                        tmpExtra = node.Text.Remove(0, node.Text.IndexOf(@")\") + 1);
                        node.Text = node.Text.Remove(node.Text.IndexOf(@")\") + 1);
                    }
                    string[] stringVals = node.Text.Remove(0, index + 5).Replace(")", "").Replace(" ", "").Split(',');

                    foreach (string val in stringVals)
                    {
                        parse[i] = int.TryParse(val, out values[i]);
                        i++;
                        if (i == 3) { break; }//force the system to only ever read 3 values

                    }

                    node.ForeColor = Color.FromArgb(values[0], values[1], values[2]);//assign the background colour of the node to the alpha,red,green,blue values

                    int endIndex = node.Text.IndexOf(values[2] + ")") + values[2].ToString().Length - index + 1;
                    node.Text = node.Text.Remove(index, endIndex) + tmpExtra;//set the nodes' text to not include the delemeter and its parameters

                    if (possition > 0)
                    {
                        if (!parse[0] || !parse[1] || !parse[2])
                        {//if any did not parse
                            MessageBox.Show(@"The update logger system saw a colour (\nbc) delemeter at line "
                                    + possition + " but could not find all the values associated with it, the missing values"
                                    + " have been set to 0." + "\r\nCan see red value? " + parse[0] + " "
                                    + stringVals[0] + "\r\nCan see green value? " + parse[1] + " " + stringVals[1]
                                    + "\r\nCan see blue value? " + parse[2] + " " + stringVals[2]
                                    , "Invalid colour set in colour delemeter", MessageBoxButtons.OK
                                    , MessageBoxIcon.Information);

                        }
                    }

                }
            }

        }

        /// <summary>
        /// Colours the text of a single target node (the primary nodes) according to the parameters of a \c delemeter in the nodes name
        /// </summary>
        /// <param name="node">The node to affect</param>
        /// <param name="data">The string of data which contains the delemeter and that will be applied to the node</param>
        /// <param name="possition">An optional parameter which points to the current line the string of data comes from, excluding this disables warnings</param>
        private void ColourNodesText(TreeNode node, uint possition = 0)
        {
            int index = node.Text.IndexOf(@"\ntc(");
            int[] values = new int[3];
            bool[] parse = new bool[3];
            int i = 0;
            string tmpExtra = "";

            if (index != -1)
            {
                if (node.Text.IndexOf(@")\") != -1)
                {
                    tmpExtra = node.Text.Remove(0, node.Text.IndexOf(@")\") + 1);
                    node.Text = node.Text.Remove(node.Text.IndexOf(@")\") + 1);
                }
                string[] stringVals = node.Text.Remove(0, index + 5).Replace(")", "").Replace(" ", "").Split(',');

                foreach (string val in stringVals)
                {
                    parse[i] = int.TryParse(val, out values[i]);
                    i++;
                    if (i == 3) { break; }//force the system to only ever read 3 values

                }

                node.ForeColor = Color.FromArgb(values[0], values[1], values[2]);//assign the background colour of the node to the alpha,red,green,blue values

                int endIndex = node.Text.IndexOf(values[2] + ")") + values[2].ToString().Length - index + 1;
                node.Text = node.Text.Remove(index, endIndex) + tmpExtra;//set the nodes' text to not include the delemeter and its parameters

                if (possition > 0)
                {
                    if (!parse[0] || !parse[1] || !parse[2])
                    {//if any did not parse
                        MessageBox.Show(@"The update logger system saw a colour (\nbc) delemeter at line "
                            + possition + " but could not find all the values associated with it, the missing values"
                            + " have been set to 0." + "\r\nCan see red value? " + parse[0] + " "
                            + stringVals[0] + "\r\nCan see green value? " + parse[1] + " " + stringVals[1]
                            + "\r\nCan see blue value? " + parse[2] + " " + stringVals[2]
                            , "Invalid colour set in colour delemeter", MessageBoxButtons.OK
                            , MessageBoxIcon.Information);

                    }
                }
            }
        }

        /// <summary>
        /// Colours the text of the targeted secondary nodes according to the parameters of a \c delemeter in the nodes name
        /// </summary>
        /// <param name="nodes">The nodes to affect</param>
        /// <param name="possition">An optional parameter which points to the current line the string of data comes from, excluding this disables warnings</param>
        private void ColourNodesText(TreeNode[] nodes, uint possition = 0)
        {
            int index = 0;
            int[] values = new int[3];
            bool[] parse = new bool[3];
            string tmpExtra = "";

            foreach (TreeNode node in nodes)
            {
                int i = 0;
                index = node.Text.IndexOf(@"\ntc(");
                if (index != -1)
                {//if index == -1, then there is no \c delemeter found
                    if (node.Text.IndexOf(@")\") != -1)
                    {
                        tmpExtra = node.Text.Remove(0, node.Text.IndexOf(@")\") + 1);
                        node.Text = node.Text.Remove(node.Text.IndexOf(@")\") + 1);
                    }
                    string[] stringVals = node.Text.Remove(0, index + 5).Replace(")", "").Replace(" ", "").Split(',');

                    foreach (string val in stringVals)
                    {
                        parse[i] = int.TryParse(val, out values[i]);
                        i++;
                        if (i == 3) { break; }//force the system to only ever read 3 values

                    }

                    node.ForeColor = Color.FromArgb(values[0], values[1], values[2]);//assign the background colour of the node to the alpha,red,green,blue values

                    int endIndex = node.Text.IndexOf(values[2] + ")") + values[2].ToString().Length - index + 1;
                    node.Text = node.Text.Remove(index, endIndex) + tmpExtra;//set the nodes' text to not include the delemeter and its parameters

                    if (possition > 0)
                    {
                        if (!parse[0] || !parse[1] || !parse[2])
                        {//if any did not parse
                            MessageBox.Show(@"The update logger system saw a colour (\nbc) delemeter at line "
                            + possition + " but could not find all the values associated with it, the missing values"
                            + " have been set to 0." + "\r\nCan see red value? " + parse[0] + " "
                            + stringVals[0] + "\r\nCan see green value? " + parse[1] + " " + stringVals[1]
                            + "\r\nCan see blue value? " + parse[2] + " " + stringVals[2]
                            , "Invalid colour set in colour delemeter", MessageBoxButtons.OK
                            , MessageBoxIcon.Information);

                        }
                    }
                }
            }

        }


        /// <summary>
        /// Colours all child nodes in the targeted primary/secondary node according to the parameters of a \c delemeter in the nodes name
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="possition"></param>
        private void ColourNodes(TreeNodeCollection nodes, uint possition = 0)
        {
            int index = 0;
            int[] values = new int[3];
            bool[] parse = new bool[3];
            string tmpExtra = "";

            foreach (TreeNode node in nodes)
            {
                int i = 0;
                index = node.Text.IndexOf(@"\nbc(");
                if (index != -1)
                {//if index == -1, then there is no \c delemeter found and skip it
                    if (node.Text.IndexOf(@")\") != -1)
                    {
                        tmpExtra = node.Text.Remove(0, node.Text.IndexOf(@")\") + 1);
                        node.Text = node.Text.Remove(node.Text.IndexOf(@")\") + 1);
                    }
                    string[] stringVals = node.Text.Remove(0, index + 5).Replace(")", "").Replace(" ", "").Split(',');

                    foreach (string val in stringVals)
                    {
                        parse[i] = int.TryParse(val, out values[i]);
                        i++;
                        if (i == 3) { break; }//force the system to only ever read 3 values

                    }

                    node.BackColor = Color.FromArgb(values[0], values[1], values[2]);//assign the background colour of the node to the alpha,red,green,blue values

                    int endIndex = node.Text.IndexOf(values[2] + ")") + values[2].ToString().Length - index + 1;
                    node.Text = node.Text.Remove(index, endIndex) + tmpExtra;//set the nodes' text to not include the delemeter and its parameters

                    if (possition > 0 && index != -1)
                    {
                        if (!parse[0] || !parse[1] || !parse[2])
                        {//if any did not parse
                            MessageBox.Show(@"The update logger system saw a colour (\nbc) delemeter at line "
                                + possition + " but could not find all the values associated with it, the missing values"
                                + " have been set to 0." + "\r\nCan see red value? " + parse[0] + " "
                                + stringVals[0] + "\r\nCan see green value? " + parse[1] + " " + stringVals[1]
                                + "\r\nCan see blue value? " + parse[2] + " " + stringVals[2]
                                , "Invalid colour set in colour delemeter", MessageBoxButtons.OK
                                , MessageBoxIcon.Information);

                        }
                    }
                }
            }

        }

        /// <summary>
        /// Colours a single target node (the primary nodes) according to the parameters of a \c delemeter in the nodes name
        /// </summary>
        /// <param name="node">The node to affect</param>
        /// <param name="data">The string of data which contains the delemeter and that will be applied to the node</param>
        /// <param name="possition">An optional parameter which points to the current line the string of data comes from, excluding this disables warnings</param>
        private void ColourNodes(TreeNode node, uint possition = 0)
        {
            int index = node.Text.IndexOf(@"\nbc(");
            int[] values = new int[3];
            bool[] parse = new bool[3];
            int i = 0;
            string tmpExtra = "";

            if (index != -1)
            {
                if (node.Text.IndexOf(@")\") != -1)
                {
                    tmpExtra = node.Text.Remove(0, node.Text.IndexOf(@")\") + 1);
                    node.Text = node.Text.Remove(node.Text.IndexOf(@")\") + 1);
                }
                string[] stringVals = node.Text.Remove(0, index + 5).Replace(")", "").Replace(" ", "").Split(',');

                foreach (string val in stringVals)
                {
                    parse[i] = int.TryParse(val, out values[i]);
                    i++;
                    if (i == 3) { break; }//force the system to only ever read 3 values

                }

                node.BackColor = Color.FromArgb(values[0], values[1], values[2]);//assign the background colour of the node to the alpha,red,green,blue values

                int endIndex = node.Text.IndexOf(values[2] + ")") + values[2].ToString().Length - index + 1;
                node.Text = node.Text.Remove(index, endIndex) + tmpExtra;//set the nodes' text to not include the delemeter and its parameters

                if (possition > 0)
                {
                    if (!parse[0] || !parse[1] || !parse[2])
                    {//if any did not parse
                        MessageBox.Show(@"The update logger system saw a colour (\nbc) delemeter at line "
                        + possition + " but could not find all the values associated with it, the missing values"
                        + " have been set to 0." + "\r\nCan see red value? " + parse[0] + " "
                        + stringVals[0] + "\r\nCan see green value? " + parse[1] + " " + stringVals[1]
                        + "\r\nCan see blue value? " + parse[2] + " " + stringVals[2]
                        , "Invalid colour set in colour delemeter", MessageBoxButtons.OK
                        , MessageBoxIcon.Information);

                    }
                }
            }
        }

        /// <summary>
        /// Colours the targeted secondary nodes according to the parameters of a \c delemeter in the nodes name
        /// </summary>
        /// <param name="nodes">The nodes to affect</param>
        /// <param name="possition">An optional parameter which points to the current line the string of data comes from, excluding this disables warnings</param>
        private void ColourNodes(TreeNode[] nodes, uint possition = 0) {
            int index = 0;
            int[] values = new int[3];
            bool[] parse = new bool[3];
            string tmpExtra = "";

            foreach (TreeNode node in nodes)
            {
                int i = 0;
                index = node.Text.IndexOf(@"\nbc(");
                if (index != -1)
                {//if index == -1, then there is no \c delemeter found
                    if (node.Text.IndexOf(@")\") != -1) {//if the node has extra delemeters after this delemeter
                        tmpExtra = node.Text.Remove(0, node.Text.IndexOf(@")\")+1);//store the other delemters and text that are after this delemeter
                        node.Text = node.Text.Remove(node.Text.IndexOf(@")\")+1);//temporarily remove it all (except the closing bracket)

                    }
                    string[] stringVals = node.Text.Remove(0, index + 5).Replace(")", "").Replace(" ", "").Split(',');

                    foreach (string val in node.Text.Remove(0, index + 5).Replace(")", "").Replace(" ", "").Split(','))
                    {
                        parse[i] = int.TryParse(val, out values[i]);
                        i++;
                        if (i == 3) { break; }//force the system to only ever read 3 values

                    }

                    node.BackColor = Color.FromArgb(values[0], values[1], values[2]);//assign the background colour of the node to the alpha,red,green,blue values

                    int endIndex = node.Text.IndexOf(values[2] + ")") + values[2].ToString().Length - index + 1;
                    node.Text = node.Text.Remove(index, endIndex) + tmpExtra;//set the nodes' text to not include the delemeter and its parameters

                    if (possition > 0 && index != -1)
                    {
                        if (!parse[0] || !parse[1] || !parse[2])
                        {//if any did not parse
                            MessageBox.Show(@"The update logger system saw a colour (\nbc) delemeter at line "
                                + possition + " but could not find all the values associated with it, the missing values"
                                + " have been set to 0." + "\r\nCan see red value? " + parse[0] + " "
                                + stringVals[0] + "\r\nCan see green value? " + parse[1] + " " + stringVals[1]
                                + "\r\nCan see blue value? " + parse[2] + " " + stringVals[2]
                                , "Invalid colour set in colour delemeter", MessageBoxButtons.OK
                                , MessageBoxIcon.Information);

                        }
                    }
                }
            }

        }

    }                                                                                                                       
}
//need to comment this

    //todo:
    //add a \l delemeter for adding http links e.g. 'whateverthehell...go to here\lwww.w3schools.com\l for stuff'

