using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Data.Entity.Core.EntityClient;
using Microsoft.Office.Interop.Excel;
using System.Runtime.Remoting.Contexts;

namespace Storehouse
{
    public partial class MainForm : Form
    {
        databaseEntities db;
        List<Category> categories;
        List<Temperature> temperatures;
        List<Expire> expires;
        List<Manufacturer> manufacturers;
        List<Supplier> suppliers;
        List<Product> products;
        public MainForm()
        {
            InitializeComponent();

            db = new databaseEntities();

            products = db.Products.ToList();
            categories = db.Categories.ToList();
            temperatures = db.Temperatures.ToList();
            expires = db.Expires.ToList();
            manufacturers = db.Manufacturers.ToList();
            suppliers = db.Suppliers.ToList();


        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            comboBox1.DataSource = categories;
            comboBox1.ValueMember = "id";
            comboBox1.DisplayMember = "name";
            //comboBox1.SelectedValue = "";
            //comboBox2.Text = "Выберите категорию";
            Random rnd = new Random();

            textBox1.Text = rnd.Next(1,50).ToString();


        }


        private void просмотрToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProductForm productForm = new ProductForm();
            productForm.Show();
            this.Hide();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            if (products.Count > 0)
            {
                products.Clear();
                comboBox2.Text = "";

            }
            comboBox1.DataSource = categories;
            comboBox1.ValueMember = "id";
            comboBox1.DisplayMember = "name";


            int cat_id = 0;
            cat_id = (int)comboBox1.SelectedValue;


            products = db.Products.Where(c => cat_id == c.Category.id).ToList();

            comboBox2.DataSource = products;
            comboBox2.ValueMember = "id";
            comboBox2.DisplayMember = "name";
            comboBox2.Text = "Выберите товар";
        }

        private void fillProductInfo(Product product)
        {
            label36.Text = product.productCode;
            label35.Text = product.name;
            label34.Text = product.Category.name;
            label33.Text = product.Manufacturer.name + ", \n" + product.Manufacturer.country + ",  \n" + product.Manufacturer.state + ", " + product.Manufacturer.city + " \n " +
                product.Manufacturer.address + "\n" + product.Manufacturer.telephone;
            label28.Text = product.Supplier.name + ",\n" + product.Supplier.country + ",\n" + product.Supplier.state + ", " + product.Supplier.city + ", \n" + product.Supplier.telephone;
            label27.Text = product.Expire.expire_date + " дней, при температуре " + product.Temperature.temperature + " градусов.";
            label26.Text = product.description;
            label21.Text = product.price + " грн.";


            if (product.category_id == 1 || product.category_id == 2 ||
                product.category_id == 3 || product.category_id == 4 ||
                product.category_id == 5)
            {
                label19.Text = product.in_stock + " кг.";
            }
            else if (product.category_id == 6)
            {
                label19.Text = product.in_stock + " л.";
            }
            else
                label19.Text = product.in_stock + " шт.";

        }

        public void FillInvoice(Invoice invoice, int id)
        {
            int count = db.Invoices.Count();
            Product product = db.Products.Find(id);

            invoice.invoice_code = (count + 1).ToString();
            label18.Text = "№ " + invoice.invoice_code;

            invoice.sale_date = dateTimePicker1.Value;
            label17.Text = invoice.sale_date.ToString();

            invoice.product_id = (int)comboBox2.SelectedValue;
            label16.Text = "# " + product.productCode;
            label15.Text = product.name;

            invoice.quantity = Int32.Parse(textBox1.Text);
            label14.Text = invoice.quantity + " кг.";

            invoice.price = (product.price * Int32.Parse(textBox1.Text));
            label13.Text = invoice.price.Value.ToString() + " грн.";

            label12.Text = product.Manufacturer.name;





        }

        private void comboBox2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string product_id = comboBox2.SelectedValue.ToString();
            int id = 0;
            bool convert = Int32.TryParse(product_id, out id);
            if (convert == false)
                return;

            Product product = db.Products.Find(id);
            if (product != null)
            {
                fillProductInfo(product);
                Invoice invoice = new Invoice();
                FillInvoice(invoice, id);
            }


        }

        private void просмотрToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            CategoryForm categoryFrom = new CategoryForm();
            categoryFrom.Show();
            this.Hide();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            DialogResult dr = about.ShowDialog(this);
            if (dr == DialogResult.Cancel) { return; }
        }

        private void просмотрToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            InvoicesForm invoicesForm = new InvoicesForm();
            invoicesForm.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string product_id = comboBox2.SelectedValue.ToString();
            int id = 0;
            bool convert = Int32.TryParse(product_id, out id);
            if (convert == false)
                return;
            Invoice invoice = new Invoice();

            FillInvoice(invoice, id);
            Product product = db.Products.Find(id);
            var minus = (product.in_stock.Value - Int32.Parse(textBox1.Text));
            if (minus >= 0)
            {
                product.in_stock = (product.in_stock - invoice.quantity);
                fillProductInfo(product);
                db.Entry(product).State = EntityState.Modified;
                db.Invoices.Add(invoice);
                db.SaveChanges();

                MessageBox.Show("Покупка совершена", "Внимание!", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show("Указанное количество товара \"" + product.name + "\" нет в наличии!", "Ошибка");
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string product_id = comboBox2.SelectedValue.ToString();
            int id = 0;
            bool convert = Int32.TryParse(product_id, out id);
            if (convert == false)
                return;
            Invoice invoice = new Invoice();
            FillInvoice(invoice, id);
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            string product_id = comboBox2.SelectedValue.ToString();
            int id = 0;
            bool convert = Int32.TryParse(product_id, out id);
            if (convert == false)
                return;
            Invoice invoice = new Invoice();
            FillInvoice(invoice, id);
        }

        private void просмотрИУправлениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ManufacturerForm mf = new ManufacturerForm();
            mf.Show();
            this.Hide();
        }

        private void просмотрToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SupplierForm sp = new SupplierForm();
            sp.Show();
            this.Hide();
        }

        public void отчетToolStripMenuItem_Click(object sender, EventArgs e)
        {
            

            SelectDateRange srd = new SelectDateRange();

            srd.dateTimePicker1.Value = (DateTime)db.Invoices.First().sale_date;

            DialogResult dr = srd.ShowDialog(this);
            DateTime startDate = srd.dateTimePicker1.Value;
            DateTime endDate = srd.dateTimePicker2.Value;
            
            List<Invoice> invoices = db.Invoices.Where(i => i.sale_date.Value >= startDate && i.sale_date.Value <= endDate).OrderByDescending(i=>i.Product.name).ToList();
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

                    //workSheet.Cells[1, "K"] = "Нужно закупить больше:";
                    //workSheet.Cells[1, "L"] = "Нужно закупить меньше:";

                    


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

                        if(!nameArray.Contains(name)){
                            nameArray.Add(name);
                        }
                        if(!sumArray.Contains(sum)){
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
                    Excel.Range rg = workSheet.get_Range("H2:H" + (row-1).ToString(), "I2:I" + (row-1).ToString());
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
                    //workSheet.Range["K1"].AutoFormat(Microsoft.Office.Interop.Excel.XlRangeAutoFormat.xlRangeAutoFormatClassic1);
                    
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
