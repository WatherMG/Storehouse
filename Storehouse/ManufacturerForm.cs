using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using Excel = Microsoft.Office.Interop.Excel;

namespace Storehouse
{
    public partial class ManufacturerForm : Form
    {
        databaseEntities db;
        public ManufacturerForm()
        {
            InitializeComponent();

            db = new databaseEntities();
            
            
            db.Manufacturers.Load();

            RefreshGrid(dataGridView1);
                     
        }

        private void Product_Load(object sender, EventArgs e)
        {

        }

        public void RefreshGrid(DataGridView grid)
        {
            var manufact = from p in db.Manufacturers
                       select new
                       {
                           id = p.id,
                           Название = p.name,
                           Страна = p.country,
                           Область = p.state,
                           Город = p.city,
                           Адрес = p.address,
                           Почтовый_индекс = p.ZIP,
                           Телефон = p.telephone,
                       };
            grid.DataSource = manufact.ToList();
            grid.Columns[0].Visible = false;
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //добавление товара
        private void button1_Click(object sender, EventArgs e)
        {
            ManufacturerAddForm manufacturerAddForm = new ManufacturerAddForm();

            DialogResult result = manufacturerAddForm.ShowDialog(this);

            if (result == DialogResult.Cancel)
                return;

            Manufacturer manufacturer = new Manufacturer();
            manufacturer.name = manufacturerAddForm.textBox1.Text;
            manufacturer.country = manufacturerAddForm.textBox2.Text;
            manufacturer.state = manufacturerAddForm.textBox3.Text;
            manufacturer.city = manufacturerAddForm.textBox4.Text;
            manufacturer.address = manufacturerAddForm.textBox5.Text;
            manufacturer.ZIP = manufacturerAddForm.textBox6.Text;
            manufacturer.telephone = manufacturerAddForm.textBox7.Text;

            db.Manufacturers.Add(manufacturer);
            db.SaveChanges();
            RefreshGrid(dataGridView1);
 
            MessageBox.Show("Новый производитель добавлен");

        }
        //редактирвоание товара
        private void button2_Click(object sender, EventArgs e)
        {
            
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int index = dataGridView1.SelectedRows[0].Index;
                int id = 0;
                bool converted = Int32.TryParse(dataGridView1[0, index].Value.ToString(), out id);
                if (converted == false)
                    return;

                Manufacturer manufacturer = db.Manufacturers.Find(id);
                ManufacturerAddForm muForm = new ManufacturerAddForm();
                muForm.textBox1.Text = manufacturer.name;
                muForm.textBox2.Text = manufacturer.country;
                muForm.textBox3.Text = manufacturer.state;
                muForm.textBox4.Text = manufacturer.city;
                muForm.textBox5.Text = manufacturer.address;
                muForm.textBox6.Text = manufacturer.ZIP;
                muForm.textBox7.Text = manufacturer.telephone;


                DialogResult result = muForm.ShowDialog(this);

                if (result == DialogResult.Cancel)
                    return;

                manufacturer.name = muForm.textBox1.Text;
                manufacturer.country = muForm.textBox2.Text;
                manufacturer.state = muForm.textBox3.Text;
                manufacturer.city = muForm.textBox4.Text;
                manufacturer.address = muForm.textBox5.Text;
                manufacturer.ZIP = muForm.textBox6.Text;
                manufacturer.telephone = muForm.textBox7.Text;

                db.Entry(manufacturer).State = EntityState.Modified;
                db.SaveChanges();
                RefreshGrid(dataGridView1);

                MessageBox.Show("Производитель обновлен");
                
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int index = dataGridView1.SelectedRows[0].Index;
                int id = 0;
                bool converted = Int32.TryParse(dataGridView1[0, index].Value.ToString(), out id);
                if (converted == false)
                    return;

                Manufacturer manufacturer = db.Manufacturers.Find(id);

                db.Manufacturers.Remove(manufacturer);
                db.SaveChanges();
                RefreshGrid(dataGridView1);

                MessageBox.Show("Производитель удален");
            }
        }

        private void оформитьЗаказToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainForm mainform = new MainForm();
            mainform.Show();
            this.Hide();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            DialogResult dr = about.ShowDialog(this);
            if (dr == DialogResult.Cancel) { return; }
        }

        private void просмотрToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            CategoryForm categoryFrom = new CategoryForm();
            categoryFrom.Show();
            this.Hide();
        }

        private void просмотрToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            InvoicesForm invoicesForm = new InvoicesForm();
            invoicesForm.Show();
            this.Hide();
        }

        private void просмотрToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainForm mf = new MainForm();
            mf.Show();
            this.Hide();
        }

        private void просмотрИУправлениеToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void просмотрToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SupplierForm sp = new SupplierForm();
            sp.Show();
            this.Hide();
        }

        private void отчетToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectDateRange srd = new SelectDateRange();

            srd.dateTimePicker1.Value = (DateTime)db.Invoices.First().sale_date;

            DialogResult dr = srd.ShowDialog(this);
            DateTime startDate = srd.dateTimePicker1.Value;
            DateTime endDate = srd.dateTimePicker2.Value;

            List<Invoice> invoices = db.Invoices.Where(i => i.sale_date.Value >= startDate && i.sale_date.Value <= endDate).OrderByDescending(i => i.Product.name).ToList();
            //string[] head = { "Код", "Название товара", "Цена за единицу", "Заказано КГ.", "Дата продажи", "Общая стоимость" };
            if ((dr == DialogResult.OK) && (0 < invoices.Count))
            {
                // Load Excel application
                Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();

                // Create empty workbook
                excel.Workbooks.Add();


                // Create Worksheet from active sheet
                Microsoft.Office.Interop.Excel._Worksheet workSheet = excel.ActiveSheet;





                try
                {
                    workSheet.Cells[1, "A"] = "Код накладной";
                    workSheet.Cells[1, "B"] = "Название товара";
                    workSheet.Cells[1, "C"] = "Цена за единицу";
                    workSheet.Cells[1, "D"] = "Заказано (КГ.)";
                    workSheet.Cells[1, "E"] = "Дата покупки";
                    workSheet.Cells[1, "F"] = "Сумма за покупку";

                    workSheet.Cells[1, "H"] = "Название товара";
                    workSheet.Cells[1, "I"] = "Количество заказанного товара";

                    workSheet.Cells[1, "K"] = "Нужно закупить больше:";
                    workSheet.Cells[1, "L"] = "Нужно закупить меньше:";




                    int row = 2;
                    ArrayList nameArray = new ArrayList();
                    ArrayList sumArray = new ArrayList();
                    int sum;
                    string name;
                    foreach (Invoice invoice in invoices)
                    {
                        workSheet.Cells[row, "A"] = invoice.invoice_code;
                        workSheet.Cells[row, "B"] = invoice.Product.name;
                        workSheet.Cells[row, "C"] = string.Format("{0} грн.", invoice.Product.price);
                        workSheet.Cells[row, "D"] = string.Format("{0} кг.", invoice.quantity);
                        workSheet.Cells[row, "E"] = invoice.sale_date.Value.Date;
                        workSheet.Cells[row, "F"] = string.Format("{0} грн.", invoice.price);
                        name = db.Invoices.Where(nn => nn.product_id == invoice.Product.id && nn.sale_date.Value >= startDate && nn.sale_date.Value <= endDate).First().Product.name.ToString();
                        sum = (int)db.Invoices.Where(i => i.Product.name == invoice.Product.name && i.sale_date.Value >= startDate && i.sale_date.Value <= endDate).Sum(s => s.quantity);

                        if (!nameArray.Contains(name))
                        {
                            nameArray.Add(name);
                        }
                        if (!sumArray.Contains(sum))
                        {
                            sumArray.Add(sum);
                        }

                        row++;
                    }
                    row = 2;
                    foreach (string nameRow in nameArray)
                    {
                        workSheet.Cells[row, "H"] = nameRow;
                        row++;
                    }
                    row = 2;
                    foreach (int sumRow in sumArray)
                    {
                        workSheet.Cells[row, "I"] = sumRow;
                        row++;
                    }
                    Excel.ChartObjects chartObjs = (Excel.ChartObjects)workSheet.ChartObjects(Type.Missing);
                    Excel.ChartObject chartObj = chartObjs.Add(100, 20, 150, 200);
                    Excel.Chart xlChart = chartObj.Chart;
                    Excel.Range rg = workSheet.get_Range("H2:H" + (row - 1).ToString(), "I2:I" + (row - 1).ToString());
                    xlChart.ChartType = Excel.XlChartType.xlPieExploded;
                    xlChart.SetSourceData(rg, Type.Missing);

                    //rg.FormulaLocal = "MAX(I2:I"+(row-1).ToString();

                    //workSheet.Cells[2, "M"].Value = "=MAX(I2:I" + (row - 1).ToString();
                    //workSheet.Cells[2, "N"].Value = "=MIN(I2:I" + (row - 1).ToString();



                    //for (int i = 2; i < row; i++)
                    // {
                    //     workSheet.Cells[i, "K"].Value = "=ЕСЛИ(M2=I" + i.ToString() + ";H" + i.ToString() + ";0)";
                    // }

                    //for (int i = 2; i < row; i++)
                    //  {
                    //      workSheet.Cells[i, "L"].Value = "=ЕСЛИ(N2=I" + i.ToString() + ";H" + i.ToString() + ";0)";
                    //  }

                    // rg.FormulaLocal = "MAX(I2:I"+(row-1).ToString();
                    //rg.FormulaLocal = "MIN(I2:I"+(row-1).ToString();


                    // Apply some predefined styles for data to look nicely :)
                    workSheet.Range["A1"].AutoFormat(Microsoft.Office.Interop.Excel.XlRangeAutoFormat.xlRangeAutoFormatClassic1);
                    workSheet.Range["H1"].AutoFormat(Microsoft.Office.Interop.Excel.XlRangeAutoFormat.xlRangeAutoFormatClassic1);
                    workSheet.Range["K1"].AutoFormat(Microsoft.Office.Interop.Excel.XlRangeAutoFormat.xlRangeAutoFormatClassic1);

                    // Define filename
                    string fileName = string.Format(@"{0}\ExcelData.xlsx", Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));

                    // Save this data as a file
                    workSheet.SaveAs(fileName);

                    // Display SUCCESS message
                    MessageBox.Show(string.Format("Файл '{0}' успешно сохранен!", fileName));
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Exception",
                        "Ошибка записи файла!\n" + exception.Message,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // Quit Excel application
                    excel.Quit();
                    //excel.Workbooks.Open(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
                    // Release COM objects (very important!)
                    if (excel != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);

                    if (workSheet != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(workSheet);

                    // Empty variables
                    excel = null;
                    workSheet = null;

                    // Force garbage collector cleaning
                    GC.Collect();
                }
            }
            else
                MessageBox.Show("В выбраном диапазоне, нет накладных!");
            if (dr == DialogResult.Cancel) { return; }
        }


    }
}
