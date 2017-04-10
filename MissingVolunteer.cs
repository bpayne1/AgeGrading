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
    public partial class MissingVolunteer : Form
    {
        public MissingVolunteer()
        {
            InitializeComponent();
        }

        private HashSet<VolunteerPoints> mMissingMembers;
        public HashSet<VolunteerPoints> MissingMembers
        {
            get { return mMissingMembers; }
            set { mMissingMembers = value; }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            ResizeHeaders();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (MissingMembers == null || MissingMembers.Count <= 0) return;

            ResizeHeaders();
            foreach (VolunteerPoints item in MissingMembers)
            {
                if (item == null) continue;
                ListViewItem listItem = new ListViewItem(new string[] { item.FirstName, item.LastName });
                listItem.Tag = item;
                listView.Items.Add(listItem);
            }
        }

        private void ResizeHeaders()
        {
            foreach (ColumnHeader header in listView.Columns)
            {
                header.Width = listView.ClientSize.Width / listView.Columns.Count;
            }
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            Application.Exit();
            Environment.Exit(1);
        }
    }
}
