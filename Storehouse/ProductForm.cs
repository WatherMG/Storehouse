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
    public partial class ProductForm : Form
    {
        databaseEntities db;
        List<Category> categories;
        List<Temperature> temperatures;
        List<Expire> expires;
        List<Manufacturer> manufacturers;
        List<Supplier> suppliers;
        List<Product> products;
        public ProductForm()
        {
            InitializeComponent();

            db = new databaseEntities();
            
            
            db.Products.Load();

            
            
            products = db.Products.ToList();
            categories = db.Categories.ToList();
            temperatures = db.Temperatures.ToList();
            expires = db.Expires.ToList();
            manufacturers = db.Manufacturers.ToList();
            suppliers = db.Suppliers.ToList();
            RefreshGrid(dataGridView1);
                     
        }

        private void Product_Load(object sender, EventArgs e)
        {

        }

        public void RefreshGrid(DataGridView grid)
        {
            var prod = from p in db.Products
                       join cat in db.Categories on p.category_id equals cat.id
                       join expire in db.Expires on p.expire_date_id equals expire.id
                       join temperature in db.Temperatures on p.temperature_id equals temperature.id
                       join manufacturer in db.Manufacturers on p.manufacturer_id equals manufacturer.id
                       join supplier in db.Suppliers on p.supplier_id equals supplier.id
                       select new
                       {
                           id = p.id,
                           Код_товара = "#" + p.productCode,
                           Название = p.name,
                           Категория = cat.name,
                           Цена = p.price + " грн.",
                           В_наличии = p.in_stock,
                           Срок_годности = expire.expire_date,
                           Температура_хранения = temperature.temperature + " С°",
                           Описание = p.description,
                           Производитель = manufacturer.name,
                           Поставщик = supplier.name
                       };
            grid.DataSource = prod.ToList();
            grid.Columns[0].Visible = false;
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //добавление товара
        private void button1_Click(object sender, EventArgs e)
        {
            ProductAddFrom productAddForm = new ProductAddFrom();

            
            productAddForm.comboBox1.DataSource = categories;
            productAddForm.comboBox1.ValueMember = "id";
            productAddForm.comboBox1.DisplayMember = "name";
            productAddForm.comboBox1.SelectedValue = "";
            
            productAddForm.comboBox3.DataSource = temperatures;
            productAddForm.comboBox3.ValueMember = "id";
            productAddForm.comboBox3.DisplayMember = "temperature";
            productAddForm.comboBox3.SelectedValue = "";
            
            productAddForm.comboBox2.DataSource = expires;
            productAddForm.comboBox2.ValueMember = "id";
            productAddForm.comboBox2.DisplayMember = "expire_date";
            productAddForm.comboBox2.SelectedValue = "";

            productAddForm.comboBox4.DataSource = manufacturers;
            productAddForm.comboBox4.ValueMember = "id";
            productAddForm.comboBox4.DisplayMember = "name";
            productAddForm.comboBox4.SelectedValue = "";

            
            productAddForm.comboBox5.DataSource = suppliers;
            productAddForm.comboBox5.ValueMember = "id";
            productAddForm.comboBox5.DisplayMember = "name";
            productAddForm.comboBox5.SelectedValue = "";



            DialogResult result = productAddForm.ShowDialog(this);

            if (result == DialogResult.Cancel)
                return;
           
            Product product = new Product();
            product.productCode = productAddForm.maskedTextBox1.Text;
            product.name = productAddForm.textBox2.Text;
            product.category_id = (int)productAddForm.comboBox1.SelectedValue;
            product.price = productAddForm.numericUpDown1.Value;
            product.in_stock = Int32.Parse(productAddForm.textBox4.Text);
            product.expire_date_id = (int)productAddForm.comboBox2.SelectedValue;
            product.temperature_id = (int)productAddForm.comboBox3.SelectedValue;
            product.manufacturer_id = (int)productAddForm.comboBox4.SelectedValue;
            product.supplier_id = (int)productAddForm.comboBox5.SelectedValue;
            product.description = productAddForm.textBox1.Text;

            db.Products.Add(product);
            db.SaveChanges();
            RefreshGrid(dataGridView1);
 
            MessageBox.Show("Новый товар добавлен");

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

                Product product = db.Products.Find(id);
                ProductUpdateForm puForm = new ProductUpdateForm();

                puForm.maskedTextBox1.Text = product.productCode;
                puForm.textBox2.Text = product.name;

                puForm.comboBox1.DataSource = categories;
                puForm.comboBox1.ValueMember = "id";
                puForm.comboBox1.DisplayMember = "name";

                if (product.Category != null)
                {
                    puForm.comboBox1.SelectedValue = product.Category.id;
                }                

                puForm.comboBox2.DataSource = expires;
                puForm.comboBox2.ValueMember = "id";
                puForm.comboBox2.DisplayMember = "expire_date";

                if (product.Expire != null)
                {
                    puForm.comboBox2.SelectedValue = product.Expire.id;
                }

                puForm.comboBox3.DataSource = temperatures;
                puForm.comboBox3.ValueMember = "id";
                puForm.comboBox3.DisplayMember = "temperature";

                if (product.Temperature != null)
                {
                    puForm.comboBox3.SelectedValue = product.Temperature.id;
                }

                puForm.comboBox4.DataSource = manufacturers;
                puForm.comboBox4.ValueMember = "id";
                puForm.comboBox4.DisplayMember = "name";

                if (product.Manufacturer != null)
                {
                    puForm.comboBox4.SelectedValue = product.Manufacturer.id;
                }

                puForm.comboBox5.DataSource = suppliers;
                puForm.comboBox5.ValueMember = "id";
                puForm.comboBox5.DisplayMember = "name";

                if (product.Supplier != null)
                {
                    puForm.comboBox5.SelectedValue = product.Supplier.id;
                }

                puForm.numericUpDown1.Value = (decimal)product.price;
                puForm.textBox4.Text = product.in_stock.ToString();
                puForm.textBox1.Text = product.description;


                DialogResult result = puForm.ShowDialog(this);

                if (result == DialogResult.Cancel) { return; }

                product.productCode = puForm.maskedTextBox1.Text;
                product.name = puForm.textBox2.Text;
                product.category_id = (int)puForm.comboBox1.SelectedValue;
                product.price = (decimal)puForm.numericUpDown1.Value;
                product.in_stock = Int32.Parse(puForm.textBox4.Text);
                product.expire_date_id = (int)puForm.comboBox2.SelectedValue;
                product.temperature_id = (int)puForm.comboBox3.SelectedValue;
                product.manufacturer_id = (int)puForm.comboBox4.SelectedValue;
                product.supplier_id = (int)puForm.comboBox5.SelectedValue;
                product.description = puForm.textBox1.Text;

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                RefreshGrid(dataGridView1);

                MessageBox.Show("Товар обновлен");
                
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

                Product product = db.Products.Find(id);
                
                db.Products.Remove(product);
                db.SaveChanges();
                RefreshGrid(dataGridView1);

                MessageBox.Show("Объект удален");
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

        private void просмотрToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            ManufacturerForm mf = new ManufacturerForm();
            mf.Show();
            this.Hide();
        }

        private void просмотрИУправлениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SupplierForm sp = new SupplierForm();
            sp.Show();
            this.Hide();
        }

        private void просмотрToolStripMenuItem_Click(object sender, EventArgs e)
        {

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
