using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AgeGrading
{
    public partial class FindDialog : Form
    {
        public FindDialog()
        {
            InitializeComponent();
        }

        public string FindText
        {
            get { return txtFind.Text; }
            set { txtFind.Text = value; }
        }

    }
}
