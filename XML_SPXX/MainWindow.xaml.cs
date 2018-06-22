using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Linq;

namespace XML_SPXX
{

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Ver.Content = "Ver."+System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Xml_savepath.Text = Savepath;
        }
        public static string TmpXml;
        public static string Xml;
        public static string Savepath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\增值税类商品编码" + DateTime.Now.ToString("yyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".xml";
        /// <summary>
        /// 将百分比转换成小数
        /// </summary>
        /// <param name="perc">百分比值，可纯为数值，或都加上%号的表示，
        public static float PerctangleToDecimal(string perc)
        {
            float f = float.Parse(perc.Substring(0, perc.Length - 1));
            return f /= 100;
        }
        /// <summary>
        /// 转换XML
        /// </summary>
        public void Foreach_Xml()
        {
            int BM = 99900001;
            //生成XML文件
            var savexml = new XDocument(
            new XDeclaration("1.0", "GB2312", null),
            new XElement("Data", new XAttribute("TYPE", "SPBIANMA"),
            new XElement("FENLEI",
            new XElement("Row", new XAttribute("BM", "999"), new XAttribute("PID", "0"), new XAttribute("MC", "转换的数据"), new XAttribute("OLD_BM", "999"), new XAttribute("OLD_PID", "0"))),
            new XElement("SPXX")));
            //输出基础XML到字符串
            TmpXml = savexml.Declaration.ToString() + savexml.ToString();
            //savexml.Save(Savepath);
            try
            {
                //读取统一编码XML文件
                XDocument doc = XDocument.Load(Xml);
                //定义并从xml字符串中加载节点（Data节点）
                XDocument rootNode = XDocument.Parse(TmpXml);
                //定位到"SPXX"节点
                XElement SPXX = rootNode.Element("Data").Element("SPXX");
                //循环读取统一编码文件
                foreach (var v in doc.Element("business").Elements("body").Elements("BMXX"))
                {
                    //定义Row节点
                    XElement Row = new XElement("Row",
                                   new XAttribute("BM", BM),
                                   new XAttribute("PID", 999),
                                   new XAttribute("MC", v.Element("SPMC").Value),
                                   new XAttribute("JM", v.Element("JM").Value),
                                   new XAttribute("SPSM", v.Element("SM").Value),
                                   new XAttribute("GGXH", v.Element("GGXH").Value),
                                   new XAttribute("JLDW", v.Element("JLDW").Value),
                                   new XAttribute("DJ", v.Element("DJ").Value),
                                   new XAttribute("OLD_BM", BM++),
                                   new XAttribute("OLD_PID", 999),
                                   new XAttribute("SL",v.Element("ZZSSL").Value),
                                   new XAttribute("HSBZ", v.Element("HSBZ").Value),
                                   new XAttribute("SPFLBM", v.Element("SPBM").Value),
                                   new XAttribute("SPFLBMPID", v.Element("PID").Value),
                                   new XAttribute("SCBM", v.Element("SCBM").Value),
                                   new XAttribute("SLLX", v.Element("SLBZ").Value),
                                   new XAttribute("SYYHBZ", v.Element("SYYHBZ").Value),
                                   new XAttribute("YHZC", v.Element("YH").Value)
                                   );

                    //将此Row节点添加到SPXX节点下
                    SPXX.Add(Row);
                }
                //设置编码
                rootNode.Declaration.Encoding = "GB2312";
                //保存对xml
                rootNode.Save(Savepath);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(),"错误",MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            }

        }

        private void SelectPath_Click(object sender, RoutedEventArgs e)
        {
            Xml = string.Empty;
            var openFileDialog = new OpenFileDialog()
            {
                Title = "选择商品和服务税收分类编码XML文件",
                Filter = "商品编码(*.xml)|*.xml"
            };
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                Xml = openFileDialog.FileName;
                Xmlpath.Text = Xml;
            }
        }

        private void SavePath_Click(object sender, RoutedEventArgs e)
        {
            Savepath = string.Empty;
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Savepath = fbd.SelectedPath+ "\\增值税类商品编码" + DateTime.Now.ToString("yyMMdd")+"_"+ DateTime.Now.ToString("HHmmss") + ".xml";
            }
            Xml_savepath.Text = Savepath;

        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if(Xml == null || Xml == "")
            {
                SelectPath_Click(sender,e);
            }
            else
            {
                Foreach_Xml();
            }

        }


        private void ToXml23_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string TOXML23;
                var openXmlFile = new OpenFileDialog()
                {
                    Title = "选择2.0.23以下版本导出的商品和服务税收分类编码",
                    Filter = "商品编码(*.xml)|*.xml"
                };
                var result = openXmlFile.ShowDialog();
                if (result == true)
                {
                    if(openXmlFile.FileName != null || openXmlFile.FileName != "")
                    {
                        XmlDocument document = new XmlDocument();
                        document.Load(openXmlFile.FileName);
                        XmlNode body = document.SelectSingleNode("business/body");
                        XmlNodeList bmxx = body.SelectNodes("BMXX");
                        for (int i = 0; i < bmxx.Count; i++)
                        {
                            string SPBM = bmxx[i].SelectSingleNode("SPBM").InnerText;
                            if (SPBM.Length == 19)
                            {
                                body.RemoveChild(bmxx[i]);
                            }
                        }
                        XmlNode yhzc = document.SelectSingleNode("business/body/YHZC");
                        body.RemoveChild(yhzc);
                        TOXML23 = document.InnerXml;
                        XmlDocument OverXml = new XmlDocument();
                        OverXml.LoadXml(TOXML23);
                        XmlNodeList bmxxlen = OverXml.SelectNodes("business/body/BMXX");
                        XmlNode COUNT = OverXml.SelectSingleNode("business/body/COUNT");
                        COUNT.InnerText = bmxxlen.Count.ToString();
                        string fullPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\2.0.23商品编码.xml";
                        OverXml.Save(fullPath);
                        MessageBox.Show("转换成功\n" + fullPath, "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("详细信息：\n" + ex, "发生错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


        }
    }
}
