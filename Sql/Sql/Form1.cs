using System.Data.SQLite;
using System.Data;

namespace Sql
{
    public partial class Form1 : Form
    {
        List<Query> queryList;
        public Form1()
        {
            InitializeComponent();
            queryList = new List<Query>();
        }
        private string ReplaceParameters(string input)
        {
            if (boxPar1.Text != "") input = input.Replace("@PAR1@", boxPar1.Text);
            if (boxPar2.Text != "") input = input.Replace("@PAR2@", boxPar2.Text);
            if (boxPar3.Text != "") input = input.Replace("@PAR3@", boxPar3.Text);
            if (boxPar4.Text != "") input = input.Replace("@PAR4@", boxPar4.Text);
            if (boxPar5.Text != "") input = input.Replace("@PAR5@", boxPar5.Text);
            if (boxPar6.Text != "") input = input.Replace("@PAR6@", boxPar6.Text);

            return input;
        }
        private void btnEsegui_Click(object sender, EventArgs e)
        {
            string connectionString = $"Data Source={pathDB.Text};Version=3", queryString = txtQuery.Text;

            if (pathDB.Text == "" || txtPathQuery.Text == "")
            {
                MessageBox.Show("Compilare tutti i campi");
                return;
            }

            DataTable dt = new DataTable();

            queryString = ReplaceParameters(queryString);

            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(queryString, connectionString);

            try
            {
                dataAdapter.Fill(dt);
                dgv.DataSource = dt;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private List<Query> DecodeFile(string input)
        {
            List<Query> output = new List<Query>();
            string[] contentSplitted = input.Split('#'), rowSplitted;
            string content, titolo;

            foreach (string queryString in contentSplitted)
            {
                if (queryString == "") continue;

                content = "";

                rowSplitted = queryString.Split("\r\n");

                titolo = rowSplitted[0];

                content = string.Join(" ", rowSplitted.Skip(1));

                Query q = new Query(titolo, content.Trim());
                output.Add(q);
            }

            return output;
        }
        private void btnPathDB_Click(object sender, EventArgs e)
        {
            OpenFileDialog opd = new OpenFileDialog();

            opd.InitialDirectory = Application.StartupPath;
            opd.Filter = "sqlite files (*.db) |*.db|Mdb files (*.mdb) |*.mdb| xls files (*.xls) |*.xls";
            opd.FilterIndex = 1;
            opd.RestoreDirectory = true;

            DialogResult result = opd.ShowDialog();

            if (result == DialogResult.OK)
            {
                pathDB.Text = opd.FileName;
            }
        }

        private void btnPathQuery_Click(object sender, EventArgs e)
        {
            OpenFileDialog opDialog = new OpenFileDialog();

            opDialog.InitialDirectory = Application.StartupPath;
            opDialog.Filter = "Text files (*.txt)|*.txt";
            opDialog.FilterIndex = 1;
            opDialog.RestoreDirectory = true;

            DialogResult dialogResult = opDialog.ShowDialog();

            if (dialogResult == DialogResult.OK) txtPathQuery.Text = opDialog.FileName;
            string fileContent;

            try
            {
                StreamReader sr = new StreamReader(txtPathQuery.Text);
                fileContent = sr.ReadToEnd();
                sr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            queryList = DecodeFile(fileContent);

            comboQuery.Items.Clear();

            comboQuery.DataSource = queryList;

            comboQuery.DisplayMember = "Titolo";
            comboQuery.ValueMember = "Contenuto";
        }

        private void comboQuery_SelectedIndexChanged(object sender, EventArgs e)
        {
            var query = (Query)comboQuery.SelectedItem;
            if (query != null) { txtQuery.Text = query.Contenuto; }
        }
    }
}