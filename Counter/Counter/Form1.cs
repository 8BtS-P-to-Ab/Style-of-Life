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

namespace Counter
{
    public partial class Counter : Form
    {
        public Counter()
        {
            InitializeComponent();
        }

        static int countLocal = 0;                                                  //setup a local counter tracker
        FileInfo fileI;
        string path = "";

        private void btnNewCounter_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtBxCounterName.Text))
            {                                        //Tif the new counter's name is not empty or whitespace 
                char[] invalid = Path.GetInvalidFileNameChars();                                            //|--get all file related invalid characters
                                                                                                            //|
                for (int i = 0; i < invalid.Length; i++)
                {                                                  //|-Tfor loop until looped through all the avalid characters
                    txtBxCounterName.Text = txtBxCounterName.Text.Replace(invalid[i], '-');                 //|-|----replace all invalid characters with '-'
                                                                                                            //| |
                }                                                                                           //|-|L)
                                                                                                            //|
                if (!File.Exists(Path.Combine(path, txtBxCounterName.Text + ".txt")))
                {                     //|-Tif file doesn't exist
                    var f = File.Create(Path.Combine(path, txtBxCounterName.Text + ".txt"));                //|-|----create the file in the working directory
                    f.Close();                                                                              //|-|----close the file
                    updateList();                                                                           //|-|----update the list so that the new counter is selectable 
                                                                                                            //| |
                }
                else
                {                                                                                    //|-\c)if file already exists
                    if (MessageBox.Show("File \"" + txtBxCounterName.Text + ".txt"                          //|-|----prompt to replace
                        + "\" already exists, do you want to override it?",                                 //| |
                        "File exists warning", MessageBoxButtons.YesNo,                                     //| |
                        MessageBoxIcon.Warning) == DialogResult.Yes)
                    {                                      //|-|---Tif response is yes
                        int selectedIndex = lstBxCounterSelector.FindString(txtBxCounterName.Text);         //|-|---|----get the index of file that already exists
                                                                                                            //| |   |
                        var f = File.Create(Path.Combine(path, txtBxCounterName.Text + ".txt"));            //|-|---|----replace the file (clearing it)
                        f.Close();                                                                          //|-|---|----close the file 
                        updateList();                                                                       //|-|---|----update the list view
                        lstBxCounterSelector.SelectedIndex = selectedIndex;                                 //|-|---|----select the counter item
                                                                                                            //| |   |
                    }                                                                                       //|-|---|e)
                                                                                                            //| |
                }                                                                                           //|-|e)
                                                                                                            //|
            }
            else
            {                                                                                        //\c)otherwise if the new counters name is not given
                MessageBox.Show("Cant create a new counter file without a name.");                          //|--tell the user
                                                                                                            //|
            }                                                                                               //|e)
        }

        private void Counter_Load(object sender, EventArgs e)
        {
            if (Environment.CurrentDirectory.Contains(@"\Debug"))
            {//if in debug (in visual studio)
                path = Path.Combine(Environment.CurrentDirectory, @"..\..\Resources\Counter\");     //get the current working directory
            }
            else
            {
                path = Path.Combine(Environment.CurrentDirectory, @"Resources\Counter\");          //get the current working directory
            }

            updateList();                                                   //update the list of counters, if any
                                                                            //
            if (int.TryParse((Opacity * 100).ToString(), out int s))        //Ttry to parse (the current form opacity(*100) to a string (so it can be parsed)) to an int as the var 's'
            {                                                               //|if parsed succesfully
                sldBrOpacity.Value = s;                                     //|----set opacity
            }                                                               //|e)
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            countLocal = 0;                                                     //reset the counter tracker
                                                                                //
            funcs clsFuncs = new funcs();                                       //create an new object of the secondary (non-static) functions list
            string selected = lstBxCounterSelector.Text + ".txt";               //get the currently selected counter
            string localPath = path + selected;                                 //get the file from the working folder directory (may be null)
            int selectedIndex = lstBxCounterSelector.SelectedIndex;             //get the index the selected counter item is
                                                                                //
            if (!string.IsNullOrWhiteSpace(selected))
            {                         //Tcheck if a selection has been made, if so;
                if (File.Exists(localPath))
                {                                   //|-Tcheck if the file already exists, if it does;
                                                    //| |
                    updateCounter("++", localPath);                             //|-|----update the file with an incrament
                                                                                //| |
                    var f = File.Create(localPath);                             //|-|----clear the file
                    f.Close();                                                  //|-|----close the created/updated file
                    clsFuncs.log(countLocal.ToString(), localPath);             //|-|----uppdate the file
                    updateList();                                               //|-|----update the list display
                    lstBxCounterSelector.SelectedIndex = selectedIndex;         //|-|----reselect the counter item (because the display updated)
                                                                                //| |
                }
                else
                {                                                        //|-\c)otherwise if the file does not exist
                    MessageBox.Show("Couldn't find the file to update.");       //|-|----tell the user
                                                                                //| |
                }                                                               //|-|e)
            }
            else
            {                                                            //\c)otherwise if no item is selected
                MessageBox.Show("A counter must be selected.");                 //|--tell the user to select something
                                                                                //|
            }                                                                   //|e)
        }

        private void btnMinus_Click(object sender, EventArgs e)
        {
            countLocal = 0;                                                     //reset the counter tracker
                                                                                //
            funcs clsFuncs = new funcs();                                       //create an new object of the secondary (non-static) functions list
            string selected = lstBxCounterSelector.Text + ".txt";               //get the currently selected counter
            string localPath = path + selected;                                 //get the file from the working folder directory (may be null)
            int selectedIndex = lstBxCounterSelector.SelectedIndex;             //get the index the selected counter item is
                                                                                //
                                                                                //System.Diagnostics.Debug.WriteLine(selected);                     //
            if (!string.IsNullOrWhiteSpace(selected))
            {                         //Tcheck if a selection has been made, if so;
                if (File.Exists(localPath))
                {                                   //|-Tcheck if the file already exists, if it does;
                                                    //| |
                    updateCounter("--", localPath);                             //|-|----update the file with an decrament
                                                                                //| |
                    var f = File.Create(localPath);                             //|-|----clear the file
                    f.Close();                                                  //|-|----close the created/updated file
                    clsFuncs.log(countLocal.ToString(), localPath);             //|-|----update the file
                    updateList();                                               //|-|----update the list display
                    lstBxCounterSelector.SelectedIndex = selectedIndex;         //|-|----reselect the counter item (because the display updated)
                                                                                //| |
                }
                else
                {                                                        //|-\c)otherwise if the file does not exist
                    MessageBox.Show("Couldn't find the file to update.");       //|-|----tell the user
                                                                                //| |
                }                                                               //|-|e)
            }
            else
            {                                                            //\c)otherwise if no item is selected
                MessageBox.Show("A counter must be selected.");                 //|--tell the user to select something
                                                                                //|
            }                                                                   //|e)
        }

        private void lstBxCounterSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = lstBxCounterSelector.Text + ".txt";       //get the currently selected item
            string localPath = path + selected;                         //get the location of the counter's text file
            updateCounter("", localPath);                               //update the counter display without changing the previous counter
        }

        private void sldBrOpacity_Scroll(object sender, EventArgs e)
        {
            double opacity = sldBrOpacity.Value;        //get the value from the opacity slider
            double test = (opacity / 100);              //convert to decimal

            this.Opacity = test;                        //set the opacity
        }

        private void aboutBtn_Click(object sender, EventArgs e)
        {
            AboutBox test = new AboutBox();
            Form isOpen = Application.OpenForms["aboutCount"];      //get if the window is still open

            if (isOpen != null) { isOpen.Close(); }                 //if it is then close it

            test.Show();
        }

        /// <summary>
        /// Updates the list view.
        /// </summary>
		private void updateList()
        {

            lstBxCounterSelector.Items.Clear();                                     //clear the list view
            countLocal = 0;                                                         //reset the counter tracker
            txtBxCounter.Text = countLocal.ToString();                              //update the counter view
            string[] files = System.IO.Directory.GetFiles(path, "*.txt");           //get every counter's .txt file (where the data is stored)
                                                                                    //
            if (files.Length != 0)
            {                                                //Tif there is at least one file in the working directory folder
                foreach (string file in files)
                {                                    //|-Tforeach item in the 'files' array get each file as 'file'
                    fileI = new FileInfo(file);                                     //|-|----get file info
                    int textL = fileI.Name.Length;                                  //|-|----get length of file's name
                    lstBxCounterSelector.Items.Add(fileI.Name.Remove(textL - 4));   //|-|----add item to the list and remove the .txt extention
                                                                                    //| |
                }                                                                   //|-|L)
                                                                                    //|
            }                                                                       //|e)

        }

        private void btnRemoveCounter_Click(object sender, EventArgs e)
        {
            removeSelected();

        }

        /// <summary>
        /// Removes the selected counter.
        /// </summary>
		private void removeSelected()
        {
            string selected = lstBxCounterSelector.Text + ".txt";                                   //get the currently selected counter
                                                                                                    //
            try
            {                                                                                   //Tcatch any errors of this:
                if (File.Exists(path + selected))
                {                                                 //|-Tif the file exists
                    File.Delete(path + selected);                                                   //|-|----delete it
                    updateList();                                                                   //|-|----update the list view
                                                                                                    //| |
                }
                else
                {                                                                            //|-\c)otherwise if the file doesn't exist
                    MessageBox.Show("Couldn't find the file to delete." +                           //|-|----tell the user the file doesn't exist
                        " Try reloading the program or try again.");                                //| |
                    updateList();                                                                   //| |----update the list view
                                                                                                    //| |
                }                                                                                   //|-|e)
            }
            catch (Exception)
            {                                                                   //|c)catch any exception
                System.Diagnostics.Debug.WriteLine("An unexpected error occured, try again.");      //|--tell the user an error occured
                throw;                                                                              //|--throw expection
                                                                                                    //|
            }                                                                                       //|e)
        }

        /// <summary>
        /// Updates the counter's file, use modifyer to define weather to decrement or incrament the counter or none to update the display.
        /// </summary>
        /// <param name="modifyer">Accepts either "++", "--" or ""</param>
        /// <param name="localPath">The full file path</param>
		private void updateCounter(string modifyer, string localPath)
        {

            if (lstBxCounterSelector.SelectedIndex != -1)
            {                                                   //Tif a counter is selected (null not selected)
                var stream = File.OpenText(localPath);          //|--open the text file's stream
                string data = stream.ReadToEnd();               //|--read the entire file
                stream.Close();                                 //|--close the file
                                                                //|
                int.TryParse(data, out countLocal);             //|--try to parse the stream data to an int and output to the variable 'counter' (the tracker)
                if (modifyer == "++")
                {                         //|-Tif the modifier is itterate
                    countLocal++;                               //|-|----itterate the tracker
                }
                else if (modifyer == "--")
                {                  //|-\c)otherwise if the modifier is decrament
                    countLocal--;                               //|-|----decrament the tracker by 1
                }                                               //|-|e)
                                                                //|
                txtBxCounter.Text = countLocal.ToString();      //|--update the coutner display
            }                                                   //|e)

        }
    }
}
