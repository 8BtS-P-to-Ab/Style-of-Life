using FolderSelect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Mass_Renamer
{
    public partial class MassRename : Form
    {
        private ushort RTC = 0;//run time counter, used for error checking
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
            MessageBox.Show("There is currently no input sanitisation, so be mindfull of what you input. This should be added soon-ish.    ");
        }

        bool DW = false;
        bool active = false;

        private void BtnMassRename_Click(object sender, EventArgs e)
        {
            active = true;
            //sanitise here
            string text = fileRenameTxtBx.Text;                                                                                                     //get the user inputted text
            int length = 0;                                                                                                                         //store the amount of files in the folder selected as 'length'
            if (textAfterNumTxtBx.Text == "Text after number (optional)") { textAfterNumTxtBx.Text = ""; }                                          //if the 'text after number' feild has not been filled; set it to nothing (so that it doesn't affect the process)
                                                                                                                                                    //
                                                                                                                                                    //FolderSelectDialog.cs and Refelector.cs is from http://www.lyquidity.com/devblog/?p=136
                                                                                                                                                    //T-//allows for the use of vita/win7 folder selector.
            FolderSelectDialog fsd = new FolderSelectDialog                                                                                     //| //
            {                                                                                                                                   //| //Tinitialise a new 'folder selection dialog box' object (controller)
                Title = "Select at leat one folder",                                                                                            //| //|---Set the dialog box's title
                InitialDirectory = @"C:\"                                                                                                       //| //|---changes the initial directory opened when the dialog box is opened to the C drive
            };                                                                                                                                  //|-//|e)
                                                                                                                                                //
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
                        var ran = new Random();                                                                                                     //|-|---|----generate a random...thing
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
                        if (!DW)                                                                                                                    //| |   |
                        {                                                                                                                           //|-|---|---Tif the warnings are NOT disabled
                            result = MessageBox.Show("Are you ABSOLUTELY sure?", "Last chance bukko!", MessageBoxButtons.YesNo,                     //| |   |   |
                            MessageBoxIcon.Question);                                                                                               //|-|---|---|----Preforme a final chech with the user (message box)
                        }
                        else { result = DialogResult.Yes; }                                                                                       //|-|---|---\c)e)otherwise set the result to "Yes"
                                                                                                                                                  //| |   |
                        if (result == DialogResult.Yes)                                                                                             //| |   |
                        {                                                                                                                           //|-|---|---Tif the dialog box returns "Yes" again of final chech
                            string ext = "";                                                                                                        //|-|---|---|----prepare to get the file's extention
                            IEnumerable<string> addT = null;                                                                                        //|-|---|---|----prepare a semi-global addition type identifyer
                                                                                                                                                    //| |   |   |          
                            if (!disableCountingTSMI.Checked)                                                                                       //| |   |   |
                            {                                                                                                                       //|-|---|---|---Tif counting is not disabled
                                if (alphabeticalTSMI.Checked)                                                                                       //|-|---|---|---|---Tif the user has defined to use alphabetical ordering type
                                { addT = Splitter.GetAdditionType(0, length.ToString()); }                                                          //|-|---|---|---|---|----get the addition type of 0 (aplhab) with a limit to character "Z"
                                else if (numericaldTSMI.Checked)                                                                                    //|-|---|---|---|---\c)otherwise if the user had defined to use alphanumerical ordering type
                                { addT = Splitter.GetAdditionType(1, length.ToString()); }                                                          //|-|---|---|---|---|----get the addition type of 1 (aplhan) with a limit of the same length as the amount of items selected
                                else if (customTSMI.Checked)                                                                                        //|-|---|---|---|---\c)otherwise if the user had defined to use an custom ordering type
                                { addT = Splitter.GetAdditionType(2, ""); }                                                                         //|-|---|---|---|---|e)--WIP
                            }
                            else
                            {                                                                                                                //|-|---|---|---\cotherwise if disabled
                                addT = Splitter.GetAdditionType(3, length.ToString());                                                              //|-|---|---|---|----get the addition type of 3 (empty return) with a limit of the same length as the amount of items selected
                            }                                                                                                                       //|-|---|---|---|e)
                                                                                                                                                    //| |   |   |
                            int i = 0;                                                                                                              //|-|---|---|----prepare to get amount of times looped
                                                                                                                                                    //| |   |   |
                            if (!randomSTBtn.Checked)                                                                                               //| |   |   |
                            {                                                                                                                       //|-|---|---|---Tif not randomizing
                                var enumT = AF.GetEnumerator();                                                                                     //|-|---|---|---|----prepare file names enumberable
                                foreach (string add in addT)
                                {                                                                                      //|-|---|---|---|---Tnew foreach loop where the enumerable is enumerated and the resultant variable is "add"
                                    if (extentionTxtBx.Text.Length > 0 && (extentionTxtBx.Text != "Extention (optional)"                            //| |   |   |   |   |
                                        && extentionTxtBx.Text != "Extention"))                                                                     //| |   |   |   |   |
                                    {                                                                                                               //|-|---|---|---|---|---Tif the user has defined an extion
                                        ext = "." + extentionTxtBx.Text;                                                                            //|-|---|---|---|---|---|---set the extention to the user defined type
                                    }
                                    else
                                    {                                                                                                        //|-|---|---|---|---|---\c)otherwise if non defined
                                        ext = Path.GetExtension(fsd.FileName + @"\" + firstFileName);                                               //|-|---|---|---|---|---|---get the current files extention
                                    }                                                                                                               //|-|---|---|---|---|---|e)
                                                                                                                                                    //| |   |   |   |   |
                                    enumT.MoveNext();                                                                                               //|-|---|---|---|---|----enumerating the enumerable so that the current file is accesable
                                    firstFileName = enumT.Current;                                                                                  //|-|---|---|---|---|----get the current file
                                    if (fileRenameTxtBx.Text.Length != 0 && (fileRenameTxtBx.Text == "What to rename the file to"                   //| |   |   |   |   |
                                        || fileRenameTxtBx.Text == "You may not rename files while sorting is disabled"))                           //| |   |   |   |   |
                                    {                                                                                                               //|-|---|---|---|---|---Tif the user has NOT defined an new name for the files to be renamed to
                                        text = firstFileName;                                                                                       //|-|---|---|---|---|---|----set name to rename to, to its own name
                                        text = text.Remove((text.Length - ext.Length), ext.Length);                                                 //|-|---|---|---|---|---|----remove the extention for replacing
                                                                                                                                                    //| |   |   |   |   |   |
                                    }                                                                                                               //|-|---|---|---|---|---|e)
                                                                                                                                                    //| |   |   |   |   |
                                    if (textAfterNumTxtBx.Text == "Text after number (optional)"                                                    //| |   |   |   |   |
                                        || textAfterNumTxtBx.Text == "Text after name")
                                    {                                                           //|-|---|---|---|---|---Tif additional text is null
                                        textAfterNumTxtBx.Text = "";                                                                                //|-|---|---|---|---|---|----set as empty
                                                                                                                                                    //| |   |   |   |   |   |
                                    }                                                                                                               //|-|---|---|---|---|---|e)
                                                                                                                                                    //| |   |   |   |   |
                                    result = DialogResult.Retry;                                                                                    //|-|---|---|---|---|----allow entry into the while loop
                                    while (result == DialogResult.Retry)
                                    {                                                                          //|-|---|---|---|---|---Twhenever the user calls to rety the rename, for whatever reason
                                        if (firstFileName != text + add + textAfterNumTxtBx.Text + ext)                                             //| |   |   |   |   |   |
                                        {                                                                                                           //|-|---|---|---|---|---|---Tif the file doesnt already has the same name as what the program is trying to rename it to
                                            Microsoft.VisualBasic.FileIO.FileSystem.RenameFile(fsd.FileName + @"\" + firstFileName,                 //| |   |   |   |   |   |   |
                                            text + add + textAfterNumTxtBx.Text + ext);                                                             //|-|---|---|---|---|---|---|----rename the file
                                            break;                                                                                                  //|-|---|---|---|---|---|>>>|>>>>break the while loop
                                        }
                                        else
                                        {                                                                                                    //|-|---|---|---|---|---|---\c)otherwise if the file already exists
                                            result = MessageBox.Show(firstFileName + " already exists, ignore the file?",                           //| |   |   |   |   |   |   |
                                                "RENAME ERROR", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Question);                       //|-|---|---|---|---|---|---|----warn the user
                                            if (result == DialogResult.Abort)                                                                       //| |   |   |   |   |   |   |
                                            {                                                                                                       //|-|---|---|---|---|---|---|---Tif the user has reported to abort process
                                                RTC = 2;                                                                                            //|-|---|---|---|---|---|---|---|----report error/abort
                                                break;                                                                                              //|-|---|---|---|---|---|>>>|>>>|>>>>break the while loop
                                            }
                                            else if (result == DialogResult.Retry)
                                            {                                                              //|-|---|---|---|---|---|---|---\c)otherwise if the user has called to retry
                                                                                                           //|-|---|---|---|---|---|---|---|----allow the loop to continue (empty space for any future errornous checking if needed)
                                            }                                                                                                       //|-|---|---|---|---|---|---|---|e)
                                        }                                                                                                           //|-|---|---|---|---|---|---|e)
                                    }                                                                                                               //|-|---|---|---|---|---|L)
                                    if (result == DialogResult.Abort) { break; }                                                                    //|-|---|---|---|---|>>>>if process must be aborted, break out of for each loop
                                }                                                                                                                   //|-|---|---|---|---|L)
                                                                                                                                                    //| |   |   |   |
                            }
                            else
                            {                                                                                                                //| |   |   |   \c)otherwise if randomised
                                foreach (string add in addT)
                                {                                                                                      //|-|---|---|---|---Tnew foreach loop where the enumerable is enumerated and the resultant variable is "add"
                                    firstFileName = AFa[i];                                                                                         //|-|---|---|---|---|----get the first, or next, file by itterating through the array
                                    if (fileRenameTxtBx.Text.Length != 0 && (fileRenameTxtBx.Text == "What to rename the file to"                   //| |   |   |   |   |
                                        || fileRenameTxtBx.Text == "You may not rename files while sorting is disabled"))                           //| |   |   |   |   |
                                    {                                                                                                               //|-|---|---|---|---|---Tif the user has NOT defined an new name for the files to be renamed to
                                        text = firstFileName;                                                                                       //|-|---|---|---|---|---|----set name to rename to, to its own name
                                    }                                                                                                               //|-|---|---|---|---|---|e)
                                                                                                                                                    //| |   |   |   |   |
                                    if (extentionTxtBx.Text.Length > 0 && (extentionTxtBx.Text != "Extention (optional)"                            //| |   |   |   |   |
                                        || extentionTxtBx.Text != "Extention"))                                                                     //| |   |   |   |   |
                                    {                                                                                                               //|-|---|---|---|---|---Tif the user has defined an extion
                                        ext = "." + extentionTxtBx.Text;                                                                            //|-|---|---|---|---|---|---set the extention to the user defined type
                                    }
                                    else
                                    {                                                                                                        //|-|---|---|---|---|---\c)otherwise if non defined
                                        ext = Path.GetExtension(fsd.FileName + @"\" + firstFileName);                                               //|-|---|---|---|---|---|---get the current files extention
                                    }                                                                                                               //|-|---|---|---|---|---|e)
                                                                                                                                                    //| |   |   |   |   |
                                    Microsoft.VisualBasic.FileIO.FileSystem.RenameFile(fsd.FileName + @"\" + firstFileName,                         //| |   |   |   |   |
                                    text + add + textAfterNumTxtBx.Text + ext);                                                                     //|-|---|---|---|---|----rename the file
                                    i++;                                                                                                            //|-|---|---|---|---|----count amount of times looped
                                }                                                                                                                   //|-|---|---|---|---|L)
                                                                                                                                                    //| |   |   |   |
                            }                                                                                                                       //| |   |   |   |e)
                            if (RTC == 0) { RTC = 1; }                                                                                              //|-|---|---|----Report succesfull run
                                                                                                                                                    //| |   |   |
                        }                                                                                                                           //|-|---|---|e)if the dialog box returns "No" of final chech; END.
                                                                                                                                                    //| |   |
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
                                        Close();                                                                                                    //|-|---|---|---|---|---|----brake out of the loop
                                                                                                                                                    //| |   |   |   |   |   |
                                    }                                                                                                               //|-|---|---|---|---|---|e)
                                                                                                                                                    //| |   |   |   |   |
                                }                                                                                                                   //|-|---|---|---|---|e)force loop as file is still invalid
                                                                                                                                                    //| |   |   |   |    
                            }                                                                                                                       //|-|---|---|---|W)file found
                                                                                                                                                    //| |   |   |
                            if (!DW)                                                                                                                //| |   |   |
                            {                                                                                                                       //|-|---|---|---Tif the warnings are NOT disabled
                                result = MessageBox.Show("Are you ABSOLUTELY sure?", "Last chance bukko!", MessageBoxButtons.YesNo,                 //| |   |   |   |
                                MessageBoxIcon.Question);                                                                                           //|-|---|---|---|----Preforme a final chech with the user (message box)
                            }
                            else { result = DialogResult.Yes; }                                                                                   //|-|---|---|---\c)e)otherwise set the result to "Yes"
                                                                                                                                                  //| |   |   |              
                            if (result == DialogResult.Yes)                                                                                         //| |   |   |
                            {                                                                                                                       //|-|---|---|---Tif the dialog box returns "Yes" of the final check
                                string ext = "";                                                                                                    //|-|---|---|---|----prepare extention
                                length = openFileDialog1.FileNames.Length;                                                                          //|-|---|---|---|----reset 'length' to the amount of files selected
                                IEnumerable<string> addT = null;                                                                                    //|-|---|---|---|----prepare a semi-global addition type identifyer
                                                                                                                                                    //| |   |   |   |          
                                if (alphabeticalTSMI.Checked)                                                                                       //|-|---|---|---|---Tif the user has defined to use alphabetical ordering type
                                { addT = Splitter.GetAdditionType(0, "Z"); }//WIP                                                                   //|-|---|---|---|---|----get the addition type of 0 (aplhab) with a limit to character "Z"
                                else if (numericaldTSMI.Checked)                                                                                    //|-|---|---|---|---\c)otherwise if the user had defined to use alphanumerical ordering type
                                { addT = Splitter.GetAdditionType(1, length.ToString()); }                                                          //|-|---|---|---|---|----get the addition type of 0 (aplhan) with a limit of the same length as the amount of items selected
                                else if (customTSMI.Checked)                                                                                        //|-|---|---|---|---\c)otherwise if the user had defined to use an custom ordering type
                                { }                                                                                                                 //|-|---|---|---|---|e)--WIP
                                int i = 0;                                                                                                          //|-|---|---|---|----prepare to get amount of times looped
                                                                                                                                                    //| |   |   |   |          
                                if (!randomSTBtn.Checked)                                                                                           //| |   |   |   |
                                {                                                                                                                   //|-|---|---|---|---Tif not randomizing
                                    foreach (string add in addT)
                                    {                                                                                  //|-|---|---|---|---|---Tnew foreach loop where the enumerable is enumerated and the resultant variable is "add"
                                        if (extentionTxtBx.Text.Length > 0 && (extentionTxtBx.Text != "Extention (optional)"                        //| |   |   |   |   |   |
                                        || extentionTxtBx.Text != "Extention"))                                                                     //| |   |   |   |   |   |
                                        {                                                                                                           //|-|---|---|---|---|---|---Tif the user has defined an extion
                                            ext = "." + extentionTxtBx.Text;                                                                        //|-|---|---|---|---|---|---|---set the extention to the user defined type
                                        }
                                        else
                                        {                                                                                                    //|-|---|---|---|---|---|---\c)otherwise if non defined
                                            ext = Path.GetExtension(fsd.FileName + @"\" + firstFileName);                                           //|-|---|---|---|---|---|---|---get the current files extention
                                        }                                                                                                           //|-|---|---|---|---|---|---|e)
                                                                                                                                                    //| |   |   |   |   |   |
                                        firstFileName = openFileDialog1.FileNames[i];                                                               //|-|---|---|---|---|---|----get the selected file
                                        if (fileRenameTxtBx.Text.Length != 0 && (fileRenameTxtBx.Text == "What to rename the file to"               //| |   |   |   |   |   |
                                        || fileRenameTxtBx.Text == "You may not rename files while sorting is disabled"))                           //| |   |   |   |   |   |
                                        {                                                                                                           //|-|---|---|---|---|---|---Tif the user has NOT defined an new name for the files to be renamed to
                                            text = firstFileName;                                                                                   //|-|---|---|---|---|---|---|----set name to rename to, to its own name
                                        }                                                                                                           //|-|---|---|---|---|---|---|e)
                                                                                                                                                    //| |   |   |   |   |   |
                                        Microsoft.VisualBasic.FileIO.FileSystem.RenameFile(firstFileName,                                           //| |   |   |   |   |   |
                                            text + add + textAfterNumTxtBx.Text + ext);                                                             //|-|---|---|---|---|---|----rename the file
                                        i++;                                                                                                        //|-|---|---|---|---|---|----count amount of times looped
                                                                                                                                                    //| |   |   |   |   |   |
                                    }                                                                                                               //|-|---|---|---|---|---|L)
                                }
                                else
                                {                                                                                                            //|-|---|---|---|---\c)otherwise if randomised
                                    foreach (string add in addT)
                                    {                                                                                  //|-|---|---|---|---|---Tnew foreach loop where the enumerable is enumerated and the resultant variable is "add"
                                        firstFileName = AFa[i];                                                                                     //|-|---|---|---|---|---|----get the first, or next, file by itterating through the array
                                        if (fileRenameTxtBx.Text.Length != 0 && (fileRenameTxtBx.Text == "What to rename the file to"               //| |   |   |   |   |   |
                                        || fileRenameTxtBx.Text == "You may not rename files while sorting is disabled"))                           //| |   |   |   |   |   |
                                        {                                                                                                           //|-|---|---|---|---|---|---Tif the user has NOT defined an new name for the files to be renamed to
                                            text = firstFileName;                                                                                   //|-|---|---|---|---|---|---|----set name to rename to, to its own name
                                        }                                                                                                           //|-|---|---|---|---|---|---|e)
                                                                                                                                                    //| |   |   |   |   |   |
                                        if (extentionTxtBx.Text.Length > 0 && (extentionTxtBx.Text != "Extention (optional)"                        //| |   |   |   |   |   |
                                        || extentionTxtBx.Text != "Extention"))                                                                     //| |   |   |   |   |   |
                                        {                                                                                                           //|-|---|---|---|---|---|---Tif the user has defined an extion
                                            ext = "." + extentionTxtBx.Text;                                                                        //|-|---|---|---|---|---|---|---set the extention to the user defined type
                                        }
                                        else
                                        {                                                                                                    //|-|---|---|---|---|---|---\c)otherwise if non defined
                                            ext = Path.GetExtension(fsd.FileName + @"\" + firstFileName);                                           //|-|---|---|---|---|---|---|---get the current files extention
                                        }                                                                                                           //|-|---|---|---|---|---|---|e)
                                                                                                                                                    //| |   |   |   |   |   |
                                        Microsoft.VisualBasic.FileIO.FileSystem.RenameFile(fsd.FileName + @"\" + firstFileName,                     //| |   |   |   |   |   |
                                        text + add + textAfterNumTxtBx.Text + ext);                                                                 //|-|---|---|---|---|---|----rename the file
                                        i++;                                                                                                        //|-|---|---|---|---|---|----count amount of times looped
                                    }                                                                                                               //|-|---|---|---|---|---|L)
                                                                                                                                                    //| |   |   |   |   |
                                }                                                                                                                   //|-|---|---|---|---|e)
                                                                                                                                                    //| |   |   |   |
                                if (RTC == 0) { RTC = 1; }                                                                                          //|-|---|---|---|----Report succesfull run
                                                                                                                                                    //| |   |   |   |
                            }                                                                                                                       //|-|---|---|---|e)
                        }
                        else
                        {                                                                                                                    //|-|---|---\c)if the user cancled file selection dialog (mutli-select)
                            RTC = 2;                                                                                                                //|-|---|---|----Report a minor error occured
                            MessageBox.Show("An error occured or the process was aborted!");                                                        //|-|---|---|----Tell them the process failed
                                                                                                                                                    //|-|---|---|
                        }                                                                                                                           //|-|---|---|e)
                    }                                                                                                                               //|-|---|e)
                }                                                                                                                                   //|-|e)
            }
            else
            {                                                                                                                                //\c)if the user cancled file selection dialog (single select)
                RTC = 2;                                                                                                                            //|--Report a minor error occured
                MessageBox.Show("An error occured or the process was aborted!");                                                                    //|--Tell them the process failed
                                                                                                                                                    //|
            }                                                                                                                                       //|e)
                                                                                                                                                    //
            if (RTC == 1)                                                                                                                           //
            {                                                                                                                                       //Tif process succesfully ran
                try                                                                                                                                 //|    
                {                                                                                                                                   //|-Ton finish of loop catch any errors of this:
                    System.Diagnostics.Process.Start(fsd.FileName);                                                                                 //|-|----open the target folder
                }                                                                                                                                   //|-|c)
                catch (Win32Exception win32Exception)                                                                                               //| |  
                {                                                                                                                                   //|-|if caught any exceptions  
                    if (RTC == 3) { RTC = 4; }                                                                                                      //|-|----if RTC has reported a caught exception previosuly, report cascading error
                    else if (RTC == 2) { RTC = 5; }                                                                                                 //|-|----otherwise if RTC has reported a caught minor error previosuly, report minor cascading error
                    else { RTC = 3; }                                                                                                               //|-|----otherwise if RTC has not reported any errors, report caught exception
                    Console.WriteLine(win32Exception.Message);                                                                                      //|-|----Tell the user the system cannot find the folder specified or something else went wrong
                }                                                                                                                                   //|-|e)
            }                                                                                                                                       //|e)
                                                                                                                                                    //
            active = false;

        }

        //UI controll
        //handles the text dissapearing and reapearing when the controll is in/out of focus, shouldn't need to ever change this
        private void fileRenameTxtBx_TextChanged(object sender, EventArgs e)
        {
            textAfterNumTxtBx.MaxLength = 255 - fileRenameTxtBx.Text.Length;

            if (textAfterNumTxtBx.Text.Length >= textAfterNumTxtBx.MaxLength && textAfterNumTxtBx.Text != "Text after number (optional)" && textAfterNumTxtBx.Text.Length != 0)
            {
                textAfterNumTxtBx.Text = textAfterNumTxtBx.Text.Remove(textAfterNumTxtBx.Text.Length - 1);

            }

            if (fileRenameTxtBx.Text.Length == 0 && extentionTxtBx.Text == "Extention (optional)") { BtnMassRename.Enabled = false; }
            else if ((fileRenameTxtBx.Text.Length != 0 && fileRenameTxtBx.Text != "What to rename the file to")
                || (extentionTxtBx.Text.Length != 0 && extentionTxtBx.Text != "Extention (optional)")) { BtnMassRename.Enabled = true; }
        }

        private void fileRenameTxtBx_Enter(object sender, EventArgs e)
        {
            if (fileRenameTxtBx.Text == "What to rename the file to")
            {
                fileRenameTxtBx.Text = "";
                fileRenameTxtBx.TextAlign = HorizontalAlignment.Left;
            }
        }

        private void fileRenameTxtBx_Leave(object sender, EventArgs e)
        {
            if (fileRenameTxtBx.Text == "")
            {
                fileRenameTxtBx.Text = "What to rename the file to";
                fileRenameTxtBx.TextAlign = HorizontalAlignment.Center;
            }
        }

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

                if ((textAfterNumTxtBx.Text.Length != 0 && textAfterNumTxtBx.Text == "Text after name" &&
                         extentionTxtBx.Text.Length != 0 && extentionTxtBx.Text == "Extention")
                         || (textAfterNumTxtBx.Text.Length == 0 && extentionTxtBx.Text.Length != 0 && extentionTxtBx.Text == "Extention")) { BtnMassRename.Enabled = false; }
                else if ((extentionTxtBx.Text.Length != 0 && extentionTxtBx.Text != "Extention")
                    || (textAfterNumTxtBx.Text.Length != 0 && textAfterNumTxtBx.Text != "Text after name")) { BtnMassRename.Enabled = true; }

            }
        }

        private void textAfterNumTxtBx_Enter(object sender, EventArgs e)
        {
            if (textAfterNumTxtBx.Text == "Text after number (optional)" || textAfterNumTxtBx.Text == "Text after name")
            {
                textAfterNumTxtBx.Text = "";
                textAfterNumTxtBx.TextAlign = HorizontalAlignment.Left;
            }
        }

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

        private void extentionTxtBx_TextChanged(object sender, EventArgs e)
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
            //if ((extention is NULL AND rename is NULL) OR (extention is empty AND rename is NULL)) then set false
            //otherwise if (rename is NOT NULL OR extention is NOT NULL) then set true
        }

        private void extentionTxtBx_Enter(object sender, EventArgs e)
        {
            if (extentionTxtBx.Text == "Extention (optional)" || extentionTxtBx.Text == "Extention")
            {
                extentionTxtBx.Text = "";
                extentionTxtBx.TextAlign = HorizontalAlignment.Left;
            }
        }

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

        //selection
        private void exitBtn_Click(object sender, EventArgs e)
        {
            if (!active)
            {
                Close();
            }
        }

        private void alphanumericSTBtn_Click(object sender, EventArgs e)
        {
            alphanumericSTBtn.Checked = true;//
            alphabraicSTBtn.Checked = false;
            regexSTBtn.Checked = false;
            randomSTBtn.Checked = false;

        }

        private void alphabraicSTBtn_Click(object sender, EventArgs e)
        {
            alphanumericSTBtn.Checked = false;
            alphabraicSTBtn.Checked = true;//
            regexSTBtn.Checked = false;
            randomSTBtn.Checked = false;

        }

        private void regexSTBtn_Click(object sender, EventArgs e)
        {
            alphabraicSTBtn.Checked = false;
            alphanumericSTBtn.Checked = false;
            regexSTBtn.Checked = true;//
            randomSTBtn.Checked = false;

        }

        private void randomSTBtn_Click(object sender, EventArgs e)
        {
            alphabraicSTBtn.Checked = false;
            alphanumericSTBtn.Checked = false;
            regexSTBtn.Checked = false;
            randomSTBtn.Checked = true;//

        }

        //ordering
        private void numericaldTSMI_Click(object sender, EventArgs e)
        {
            numericaldTSMI.Checked = true;//
            alphabeticalTSMI.Checked = false;
            customTSMI.Checked = false;
            disableCountingTSMI.Checked = false;
        }

        private void alphabeticalTSMI_Click(object sender, EventArgs e)
        {
            numericaldTSMI.Checked = false;
            alphabeticalTSMI.Checked = true;//
            customTSMI.Checked = false;
            disableCountingTSMI.Checked = false;
        }

        private void customTSMI_Click(object sender, EventArgs e)
        {
            numericaldTSMI.Checked = false;
            alphabeticalTSMI.Checked = false;
            customTSMI.Checked = true;//
            disableCountingTSMI.Checked = false;
        }

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
        private void guideBtn_Click(object sender, EventArgs e)
        {
            MRGuide test = new MRGuide();
            Form isOpen = Application.OpenForms["CounterForm"];//get if the window is still open

            if (isOpen != null) { isOpen.Close(); }//if it is then close it

            test.Show();
        }

        private void aboutBtn_Click(object sender, EventArgs e)
        {
            AboutMR test = new AboutMR();
            Form isOpen = Application.OpenForms["AboutBoxMR"];//get if the window is still open

            if (isOpen != null) { isOpen.Close(); }//if it is then close it

            test.Show();
        }

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

        private void sldBrOpacity_Scroll(object sender, EventArgs e)
        {
            double opacity = sldBrOpacity.Value;//get the value from the opacity slider
            double test = (opacity / 100);//convert to decimal

            this.Opacity = test;//set the opacity
        }


    }
}
