using FoundModel.DAL;
using FoundModel.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FoundModel
{
    public partial class Form1 : Form
    {
        List<TableDTO> listTable;
        string sqltext;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = "Server=.;Database=Test;User ID=sa;Password=123456";
            this.textBoxlj.Text = @"D:\FB";
            this.textBoxmmkj.Text = "Demo.Model.Model";
            this.textBoxtjlqz.Text = "Request_";
            this.textBoxgyllm.Text = "ModelDTO";
            this.textBoxtjlzdtx.Text = "SelectField";
            this.textBoxmmkj2.Text = "Demo.DTO";
            this.textBoxmmkj3.Text = "Demo.Model.CM";
            this.textBoxEflm.Text = "EFContext";

        }

        /// <summary>
        /// 连接数据库，查询表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            sqltext = this.textBox1.Text.Trim();

            using (EFHelper efh = new EFHelper(sqltext))
            {
                listTable = efh.Database.SqlQuery<TableDTO>("select name from sysobjects where xtype='u' ").ToList();
                this.dataGridView1.DataSource = listTable;
            }
            MessageBox.Show("数据库查询成功！");
        }

        /// <summary>
        /// 连接数据库，查询视图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {

            sqltext = this.textBox1.Text.Trim();

            using (EFHelper efh = new EFHelper(sqltext))
            {
                listTable = efh.Database.SqlQuery<TableDTO>("SELECT * FROM sys.VIEWS ").ToList();
                this.dataGridView1.DataSource = listTable;
            }
            MessageBox.Show("数据库查询成功！");
        }

        /// <summary>
        /// 生成实体类
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (listTable == null)
            {
                MessageBox.Show("必须先查询数据库表集合，不然你搞锤子啊！");
                return;
            }
            string lujing = this.textBoxlj.Text.Trim() + "/Model";
            string mmkj = this.textBoxmmkj.Text.Trim();
            string[] tx = this.textBoxtx.Text.Trim().Split(',');

            try
            {
                // 如果目录不存在则要先创建
                if (!Directory.Exists(lujing))
                {
                    Directory.CreateDirectory(lujing);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("路径格式错误---" + ex.Message);
                return;

            }
            foreach (TableDTO tableName in listTable)
            {
                FileStream fs = new FileStream(lujing + "/" + tableName.name + ".cs", FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs, Encoding.Default);

                sw.WriteLine("using System;");
                sw.WriteLine("using System.Collections.Generic; ");
                sw.WriteLine("using System.ComponentModel.DataAnnotations; ");               
                sw.WriteLine("");

                #region 命名空间部分
                if (mmkj != "")
                {
                    sw.WriteLine("namespace " + mmkj);
                    sw.WriteLine("{");
                }

                #region class部分

                if (tx != null && tx[0] != "")
                {
                    foreach (string item in tx)
                    {
                        sw.WriteLine("    [" + item + "]");
                    }
                }
                sw.WriteLine("    public partial class " + tableName.name);
                sw.WriteLine("    {");

                List<FieldDTO> listField = new List<FieldDTO>();
                using (EFHelper efh = new EFHelper(sqltext))
                {
                    listField = efh.Database.SqlQuery<FieldDTO>("SELECT C.name as FieldName,	 T.name as FieldType,convert(bit,C.IsNullable)  as IfNull FROM syscolumns C INNER JOIN systypes T ON C.xusertype = T.xusertype  WHERE C.id = object_id('" + tableName.name + "')").ToList();

                }

                foreach (FieldDTO field in listField)
                {
                    if (field.FieldName != null && field.FieldType != null)
                    {
                        string name = field.FieldName.ToString();
                        string type = field.FieldType.ToString();
                        switch (type)
                        {
                            case "int": type = "int"; break;
                            case "text": type = "string"; break;
                            case "bigint": type = "int"; break;
                            case "binary": type = "Byte[]"; break;
                            case "bit": type = "Boolean"; break;
                            case "char": type = "string"; break;
                            case "datetime": type = "DateTime"; break;
                            case "decimal": type = "Decimal"; break;
                            case "float": type = "Double"; break;
                            case "image": type = "Byte[]"; break;
                            case "money": type = "Decimal"; break;
                            case "nchar": type = "string"; break;
                            case "ntext": type = "string"; break;
                            case "numeric": type = "Decimal"; break;
                            case "nvarchar": type = "string"; break;
                            case "real": type = "Single"; break;
                            case "smalldatetime": type = "DateTime"; break;
                            case "smallint": type = "Int16"; break;
                            case "smallmoney": type = "Decimal"; break;
                            case "timestamp": type = "DateTime"; break;
                            case "tinyint": type = "Byte"; break;
                            case "varbinary": type = "Byte[]"; break;
                            case "varchar": type = "string"; break;
                            case "Variant": type = "Object"; break;
                            case "uniqueidentifier": type = "Guid"; break;
                            default:
                                type = "What";
                                break;
                        }

                        string isnull = "";
                        if (field.IfNull && type != "string")
                        {
                            isnull = "?";
                        }
                        if (name.ToLower().Equals("id"))
                        {
                            sw.WriteLine("        [Key]");
                        }
                        sw.WriteLine("        public " + type + isnull + " " + name + " { get; set; }");
                    }
                }
                sw.WriteLine("    }");
                #endregion

                if (mmkj != "")
                {
                    sw.WriteLine("}");
                }
                #endregion

                sw.Flush();

                sw.Close();
                fs.Close();
            }


            MessageBox.Show("实体类Model生成完成！");

        }


        /// <summary>
        /// 生成DTO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (listTable == null)
            {
                MessageBox.Show("必须先查询数据库表集合，不然你搞锤子啊！");
                return;
            }
            string lujing = this.textBoxlj.Text.Trim() + "/DTO";
            string mmkj = this.textBoxmmkj2.Text.Trim();

            try
            {
                // 如果目录不存在则要先创建
                if (!Directory.Exists(lujing))
                {
                    Directory.CreateDirectory(lujing);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("路径格式错误---" + ex.Message);
                return;
            }
            foreach (TableDTO tableName in listTable)
            {
                FileStream fs = new FileStream(lujing + "/" + tableName.name + "DTO.cs", FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs, Encoding.Default);

                sw.WriteLine("using System;");
                sw.WriteLine("using System.Collections.Generic; ");
                sw.WriteLine("");

                #region 命名空间部分
                if (mmkj != "")
                {
                    sw.WriteLine("namespace " + mmkj);
                    sw.WriteLine("{");
                }
                #region Request_Model部分(查询条件类)
                string modeldto = "";
                if (this.textBoxgyllm.Text.Trim() != "")
                {
                    modeldto = " : " + this.textBoxgyllm.Text.Trim();
                }
                sw.WriteLine("    public class " + this.textBoxtjlqz.Text.Trim() + tableName.name + modeldto);
                sw.WriteLine("    {");

                string lzdtx = "";
                if (this.textBoxtjlzdtx.Text.Trim() != "")
                {
                    lzdtx = "[" + this.textBoxtjlzdtx.Text.Trim() + "(\"and\", \"=\", \"Guid\")]";

                }
                sw.WriteLine("        " + lzdtx);
                sw.WriteLine("        public Guid? Id { get; set; }");
                sw.WriteLine("    }");
                #endregion

                #region classModel部分

                sw.WriteLine("    public class " + tableName.name + "Model");
                sw.WriteLine("    {");

                List<FieldDTO> listField = new List<FieldDTO>();
                using (EFHelper efh = new EFHelper(sqltext))
                {
                    listField = efh.Database.SqlQuery<FieldDTO>("SELECT C.name as FieldName,	 T.name as FieldType,convert(bit,C.IsNullable)  as IfNull FROM syscolumns C INNER JOIN systypes T ON C.xusertype = T.xusertype  WHERE C.id = object_id('" + tableName.name + "')").ToList();

                }

                foreach (FieldDTO field in listField)
                {
                    if (field.FieldName != null && field.FieldType != null)
                    {
                        string name = field.FieldName.ToString();
                        string type = field.FieldType.ToString();
                        switch (type)
                        {
                            case "int": type = "int"; break;
                            case "text": type = "string"; break;
                            case "bigint": type = "int"; break;
                            case "binary": type = "Byte[]"; break;
                            case "bit": type = "Boolean"; break;
                            case "char": type = "string"; break;
                            case "datetime": type = "DateTime"; break;
                            case "decimal": type = "Decimal"; break;
                            case "float": type = "Double"; break;
                            case "image": type = "Byte[]"; break;
                            case "money": type = "Decimal"; break;
                            case "nchar": type = "string"; break;
                            case "ntext": type = "string"; break;
                            case "numeric": type = "Decimal"; break;
                            case "nvarchar": type = "string"; break;
                            case "real": type = "Single"; break;
                            case "smalldatetime": type = "DateTime"; break;
                            case "smallint": type = "Int16"; break;
                            case "smallmoney": type = "Decimal"; break;
                            case "timestamp": type = "DateTime"; break;
                            case "tinyint": type = "Byte"; break;
                            case "varbinary": type = "Byte[]"; break;
                            case "varchar": type = "string"; break;
                            case "Variant": type = "Object"; break;
                            case "uniqueidentifier": type = "Guid"; break;
                            default:
                                type = "What";
                                break;
                        }

                        string isnull = "";
                        if (field.IfNull && type != "string")
                        {
                            isnull = "?";
                        }
                        sw.WriteLine("         public " + type + isnull + " " + name + " { get; set; }");
                    }
                }
                sw.WriteLine("    }");
                #endregion

                #region classDTO部分
                sw.WriteLine("    public class " + tableName.name + "DTO : " + tableName.name + "Model");
                sw.WriteLine("    {");

                sw.WriteLine("    }");
                #endregion

                if (mmkj != "")
                {
                    sw.WriteLine("}");
                }
                #endregion

                sw.Flush();

                sw.Close();
                fs.Close();
            }


            MessageBox.Show("实体类DTO生成完成！");

        }

        /// <summary>
        /// 生成EF类
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (listTable == null)
            {
                MessageBox.Show("必须先查询数据库表集合，不然你搞锤子啊！");
                return;
            }
            string name = this.textBoxEflm.Text.Trim();
            if (name == "")
            {
                MessageBox.Show("必须输入类名，不然你完个锤子啊！");
                return;

            }
            string lujing = this.textBoxlj.Text.Trim() + "";
            string mmkj = this.textBoxmmkj3.Text.Trim();

            try
            {
                // 如果目录不存在则要先创建
                if (!Directory.Exists(lujing))
                {
                    Directory.CreateDirectory(lujing);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("路径格式错误---" + ex.Message);
                return;
            }
            FileStream fs = new FileStream(lujing + "/" + name + ".cs", FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);

            sw.WriteLine("using System;");
            sw.WriteLine("using System.Collections.Generic; ");
            sw.WriteLine("");

            #region 命名空间部分
            if (mmkj != "")
            {
                sw.WriteLine("namespace " + mmkj);
                sw.WriteLine("{");
            }

            #region class部分
            sw.WriteLine("    public class " + name + " : DbContext");
            sw.WriteLine("    {");

            sw.WriteLine("        public " + name + "(DbContextOptions<" + name + "> options)");
            sw.WriteLine("            : base(options)");
            sw.WriteLine("        {");
            sw.WriteLine("        }");
            foreach (TableDTO tableName in listTable)
            {
                sw.WriteLine("        public virtual DbSet<" + tableName.name + "> " + tableName.name + " { get; set; }");
            }
            sw.WriteLine("    }");
            #endregion
            if (mmkj != "")
            {
                sw.WriteLine("}");
            }
            #endregion

            sw.Flush();

            sw.Close();
            fs.Close();

            MessageBox.Show("EF类生成完成！");

        }


    }
}
