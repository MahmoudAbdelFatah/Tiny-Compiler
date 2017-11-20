using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TinyScanner
{
    public class Token
    {
        public string lex;
        public string tokentype;
        public int lineNumber;
    }
    public partial class Scanner : Form
    {

        List<int> lineNumber = new List<int>();
        public Scanner()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            setColumnHeaderStyle();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void clearGridViews()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            dataGridView2.Rows.Clear();
            dataGridView2.Refresh();
        }



        private void button1_Click(object sender, EventArgs e)
        {
            clearGridViews();
            string[] sourceCode = textBox1.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            ScannerPhase scanner = new ScannerPhase();
            lineNumber = scanner.getlineNumberList();
            //List<KeyValuePair<string, string>> scannedCode = new List<KeyValuePair<string,string>>();
            List<Token> tokens = new List<Token>();
            scanner.scanning(sourceCode, ref tokens);
            int cnt = 0;
            foreach (var value in tokens)
            {
                if (value.tokentype != "Error")
                {
                    dataGridView1.RowCount++;
                    dataGridView1.Rows[dataGridView1.RowCount - 2].Cells[0].Value = value.lex;
                    dataGridView1.Rows[dataGridView1.RowCount - 2].Cells[1].Value = value.tokentype;
                }
                else
                {
                    dataGridView2.RowCount++;
                    dataGridView2.Rows[dataGridView2.RowCount - 2].Cells[0].Value = value.lineNumber;
                    dataGridView2.Rows[dataGridView2.RowCount - 2].Cells[1].Value = value.lex;
                    cnt++;
                }
            }
        }

        private void setColumnHeaderStyle()
        {
            //dataGridView1
            dataGridView1.ColumnCount = 2;
            dataGridView1.ColumnHeadersVisible = true;
            dataGridView1.Columns[0].Name = "Lexeme";
            dataGridView1.Columns[1].Name = "Token Classes";
            dataGridView1.Columns[1].Width = 250;

            //dataGridView2
            dataGridView2.ColumnCount = 2;
            dataGridView2.ColumnHeadersVisible = true;
            dataGridView2.Columns[0].Name = "L.Number";
            dataGridView2.Columns[1].Name = "error";
            dataGridView2.Columns[1].Width = 250;
            // Set the column header style.
            DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();
            columnHeaderStyle.BackColor = Color.Beige;
            columnHeaderStyle.Font = new Font("Verdana", 10, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle = columnHeaderStyle;
            dataGridView2.ColumnHeadersDefaultCellStyle = columnHeaderStyle;
            dataGridView1.RowCount = 1;
            dataGridView2.RowCount = 1;
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

    }
}
