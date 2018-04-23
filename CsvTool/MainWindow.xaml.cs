using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using CsvHelper;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace CsvTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        // 修改过数据的页面名称
        List<string> m_ChangeDataPageList;
        // 页面名称列表
        List<Page> m_PageList;
        string m_FilePath;
        string m_CurrentPageName;
        // <表名,<index,>
        public Dictionary<string, Dictionary<int, Dictionary<string,string>>> m_DataBase;
        // 表名，头列表
        public Dictionary<string, List<string>> m_DataHeader;
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                this.Closing += MainWindow_Closing;
                m_PageList = new List<Page>();
                m_DataBase = new Dictionary<string, Dictionary<int, Dictionary<string, string>>>();
                m_DataHeader = new Dictionary<string, List<string>>();
                m_ChangeDataPageList = new List<string>();
                SelectPath();
                Navigate("Monster");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        
        /// <summary>
        /// 导航到某个界面
        /// </summary>
        private void Navigate(string name)
        {
            try
            {
                m_CurrentPageName = name;
                BaseGroupBox.Header = "当前文件：" + name + ".csv";
                Page con = m_PageList.Find((s) => s.Title.Equals(name));
                if (con != null)
                {
                    this.mainFrame.Content = con;
                    return;
                }

                string uri = "CsvTool." + name;
                Type type = Type.GetType(uri);
                if (type != null)
                {
                    //实例化Page页
                    object obj = type.Assembly.CreateInstance(uri);
                    Page control = obj as Page;
                    this.mainFrame.Content = control;
                    PropertyInfo[] infos = type.GetProperties();
                    foreach (PropertyInfo info in infos)
                    {
                        //将MainWindow设为page页的ParentWin
                        if (info.Name == "ParentWindow")
                        {
                            info.SetValue(control, this, null);
                            break;
                        }
                    }
                    LoadCsv(name);
                    BasePage basecon = obj as BasePage;
                    basecon.InitData(name);
                    basecon.Name = name;
                    m_PageList.Add(control);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 配置设置
        /// </summary>
        public static void SetAppSetting(string key, string value)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (config.AppSettings.Settings[key].Value != null)
                    config.AppSettings.Settings[key].Value = value;
                else
                    config.AppSettings.Settings.Add(key, value);

                config.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 获取设置
        /// </summary>
        public static string GetAppSetting(string key)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            return config.AppSettings.Settings[key].Value;
        }

        /// <summary>
        /// 选择CSV路径
        /// </summary>
        private bool SelectPath(bool bForce = false)
        {
            try
            {
                m_FilePath = GetAppSetting("FilePath");
                if (bForce || m_FilePath == "")
                {
                    CommonOpenFileDialog dialog = new CommonOpenFileDialog
                    {
                        Title = "请选择Csv配置文件所在路径",
                        IsFolderPicker = true
                    };
                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        string folderName = dialog.FileName;
                        if (folderName != "")
                        {
                            m_FilePath = folderName;
                            SetAppSetting("FilePath", m_FilePath);
                            return true;
                        }
                    }
                    dialog.Dispose();
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 加载CSV
        /// </summary>
        private void LoadCsv(string name)
        {
            try
            {
                string databasename;
                var query = from d in m_DataBase.AsQueryable()
                            where d.Key == name
                            select d.Key;
                databasename = query.FirstOrDefault();

                if (databasename == null)
                {
                    StreamReader sr = new StreamReader(m_FilePath + "\\" + name + ".csv", Encoding.UTF8);
                    try
                    {
                        var parser = new CsvParser(sr);
                        Dictionary<int, Dictionary<string, string>> m_Data = new Dictionary<int, Dictionary<string, string>>();
                        int nIndex = 1;
                        List<string> header = new List<string>();
                        var headerrow = parser.Read();
                        if (headerrow != null)
                        {
                            foreach (string element in headerrow)
                            {
                                header.Add(element);
                            }
                        }
                        m_DataHeader.Add(name, header);
                        while (true)
                        {
                            var row = parser.Read();
                            if (row != null)
                            {
                                Dictionary<string, string> list = new Dictionary<string, string>();
                                int i = 0;
                                foreach (string element in row)
                                {
                                    list.Add(header[i],element);
                                    ++i;
                                }

                                m_Data.Add(nIndex, list);
                                ++nIndex;
                            }
                            else
                                break;
                        }
                        m_DataBase.Add(name, m_Data);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                        return;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                SelectPath(true);
                LoadCsv(name);
            }
        }
        
        /// <summary>
        /// 数据修改状态
        /// </summary>
        public void SetDataChange(string name)
        {
            if (m_ChangeDataPageList.Find(s => s == name) == null)
            {
                m_ChangeDataPageList.Add(name);
            }
        }


        /// <summary>
        /// 保存数据
        /// </summary>
        private bool SaveData(string csvname)
        {
            try
            {
                if (m_ChangeDataPageList.Find(s => s == csvname) != null) 
                {
                    StreamWriter sr = new StreamWriter(m_FilePath + "\\" + csvname + ".csv", false, Encoding.UTF8);
                    var csv = new CsvWriter(sr);
                    List<string> list = new List<string>();

                    var namedata = m_DataBase[csvname];
                    var headerdata = m_DataHeader[csvname];
                    if (namedata == null || headerdata == null)
                        return false;

                    foreach (var header in headerdata)
                    {
                        list.Add(header);
                    }

                    foreach (var data in namedata)
                    {
                        Dictionary<string, string> value = data.Value;
                        for (int i = 0; i < headerdata.Count; ++i)
                        {
                            value.TryGetValue(headerdata[i], out string ret);
                            if (ret == null)
                            {
                                return false;
                            }
                            list.Add(ret);
                        }
                    }

                    int index = 0;
                    foreach (var item in list)
                    {
                        csv.WriteField(item);
                        ++index;
                        if (index == headerdata.Count)
                        {
                            index = 0;
                            csv.NextRecord();
                        }
                    }
                    csv.Flush();
                    sr.Close();
                    m_ChangeDataPageList.Remove(csvname);
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        public void AddIndex(string index)
        {
            BasePage page = mainFrame.Content as BasePage;
            page.AddIndex(m_CurrentPageName, index);
        }
        
        /// <summary>
        /// 主程序关闭事件
        /// </summary>
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (m_ChangeDataPageList.Count > 0)
            {
                if (MessageBox.Show("有修改未保存!是否退出？", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// CSV路径选择菜单
        /// </summary>
        private void MenuItem_SelectPath_Click(object sender, RoutedEventArgs e)
        {
            SelectPath(true);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        private void MenuItem_DelIndex_Click(object sender, RoutedEventArgs e)
        {
            BasePage page = mainFrame.Content as BasePage;
            page.DelIndex(m_CurrentPageName);
        }

        /// <summary>
        /// 保存菜单
        /// </summary>
        private void MenuItem_Save_Click(object sender, RoutedEventArgs e)
        {
            SaveData(m_CurrentPageName);
        }

        /// <summary>
        /// 全部保存菜单
        /// </summary>
        private void MenuItem_SaveAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var page in m_PageList)
            {
                SaveData(page.Name.ToString());
            }
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        private void MenuItem_AddIndex_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InputBox input = new InputBox
                {
                    ParentWindow = this,
                    Title = "添加数据",
                    ResizeMode = ResizeMode.NoResize,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };
                input.TipsTextBlock.Text = "请出入列表序号";
                input.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 导航按钮
        /// </summary>
        private void BtnNav_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                Navigate(btn.Tag.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
