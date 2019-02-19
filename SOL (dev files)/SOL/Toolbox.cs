using System;
using System.Windows.Forms;

namespace SOL
{
    public partial class Toolbox : Form
    {
        public Toolbox()
        {
            InitializeComponent();
        }

        private void GoToCounter_Click(object sender, EventArgs e)
        {
            Counter test = new Counter();
            Form isOpen = Application.OpenForms["CounterForm"];     //get if the window is still open
                                                                    //
            if (isOpen != null) { isOpen.Close(); }                 //if it is then close it
                                                                    //
            test.Show();                                            //otherwise open it
        }

        private void GoToDTranslator_Click(object sender, EventArgs e)
        {
            DiscordTranslator test = new DiscordTranslator();
            Form isOpen = Application.OpenForms["DiscordTranslator"];       //get if the window is still open
                                                                            //
            if (isOpen != null) { isOpen.Close(); }                         //if it is then close it
                                                                            //
            test.Show();                                                    //otherwise open it
        }

        private void GoToRename_Click(object sender, EventArgs e)
        {
            MassRename form = new MassRename();
            Form isOpen = Application.OpenForms["Mass Renamer"];        //get if the window is still open
                                                                        //
            if (isOpen != null) { isOpen.Close(); }                     //if it is then close it
                                                                        //
            form.Show();                                                //otherwise open it
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

        private void Toolbox_Load(object sender, EventArgs e)
        {
            int s;

            if (int.TryParse((Opacity * 100).ToString(), out s))
            {
                sldBrOpacity.Value = s;//set opacity on load
            }
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
            //additions will be added later. Basically this will open a window which will get all branches and treat them as 'additional content' without ever pushing the branch to main
            //this will mean branches will (at some point) have delemeters indicating if it's an 'override' or an 'addition' - that is if it updates existing code/forms or adds new code/forms.
        }
    }
}
