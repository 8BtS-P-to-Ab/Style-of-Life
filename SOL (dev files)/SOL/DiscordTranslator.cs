using System;
using System.Windows.Forms;

namespace SOL
{
    public partial class DiscordTranslator : Form
    {
        public DiscordTranslator()
        {
            InitializeComponent();
        }

        private void btnTranslate_Click(object sender, EventArgs e)
        {
            txtBxTranslated.Text = "";

            //globalb initializers
            string textTranslated = "";
            string textTranslating = "";
            string curChar = "";
            int i = 0;
            int charI = 0;

            //prep
            string text = txtBxEnglish.Text;
            String[] textSeperated = text.Split();
            int textLength = textSeperated.Length;

            //runtime
            foreach (var word in textSeperated)
            {                                                                           //Tforeach word
                int wordLength = word.Length;                                           //|--get the length of the word
                i++;                                                                    //|--itterate the identifier to the current word
                                                                                        //|
                foreach (char wordChar in word)                                         //|
                {                                                                       //|-Tforeach character in each word
                    charI++;                                                            //|-|----itterate the identifier to the current character of the current word
                    curChar = wordChar.ToString();                                      //|-|----convert the character to a string
                    textTranslating = curChar.Insert(0, ":regional_indicator_");        //|-|----insert the discord emoticon identifier before the character
                                                                                        //| |
                    if (textLength == 1)                                                //| |
                    {                                                                   //|-|---Tif there is only one word
                        if (charI != wordLength)                                        //| |   |
                        {                                                               //|-|---|---Tif the current character is not the last character of the word
                            textTranslating = textTranslating.Insert(21, ": ");         //|-|---|---|----add a colon and a space so the next character can start
                        }
                        else
                        {                                                        //|-|---|---\c)otherwise if the current character is the last character
                            textTranslating = textTranslating.Insert(21, ":");          //|-|---|---|----only add colon
                        }                                                               //|-|---|---|e)
                    }
                    else
                    {                                                            //|-|---\c)otherwise, if there's more than one word
                        if (i != textLength || charI != wordLength)                     //|-|---|
                        {                                                               //|-|---|---Tif the current word is not the last word and if the current character is not the last character
                            textTranslating = textTranslating.Insert(21, ": ");         //|-|---|---|----add a colon and space
                        }
                        else
                        {                                                        //|-|---|---\c)otherwise, if the current character is the last character
                            textTranslating = textTranslating.Insert(21, ":");          //|-|---|---|only add a colon
                        }                                                               //|-|---|---|e)
                    }                                                                   //|-|---|e)
                    textTranslated += textTranslating;                                  //|-|----append the result to the previous results 
                                                                                        //| |
                }                                                                       //|-|L)
                                                                                        //|
                if (i != textLength)                                                    //|
                {                                                                       //|-Tif the current word is not the last word
                    txtBxTranslated.Text += (textTranslated + " ");                     //|-|----output the resultant word  and add an additional space between words
                }
                else
                {                                                                //|-\c)otherwise if the current word is the last word
                    txtBxTranslated.Text += textTranslated;                             //|-|----simply output the result of the word
                }                                                                       //|-|e)
                textTranslated = "";                                                    //|--reset the variables
                textTranslating = "";                                                   //|
                charI = 0;                                                              //|
            }                                                                           //|L)
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtBxTranslated.Text = "";
            txtBxEnglish.Text = "";
        }

        private void sldBrOpacity_Scroll(object sender, EventArgs e)
        {
            double opacity = sldBrOpacity.Value;        //get the value from the opacity slider
            double test = (opacity / 100);              //convert to decimal

            this.Opacity = test;                        //set the opacity
        }

        private void DiscordTranslator_Load(object sender, EventArgs e)
        {
            int s;

            if (int.TryParse((Opacity * 100).ToString(), out s))
            {
                sldBrOpacity.Value = s;     //set upacity on load
            }
        }

        private void aboutBtn_Click(object sender, EventArgs e)
        {
            AboutDT test = new AboutDT();
            Form isOpen = Application.OpenForms["AboutDT"];     //get if the window is still open

            if (isOpen != null) { isOpen.Close(); }             //if it is then close it

            test.Show();
        }
    }
}
