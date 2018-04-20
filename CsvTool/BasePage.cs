using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CsvTool
{
    public class BasePage : Page
    {
        public BasePage()
        {
            m_DataIndex = new Dictionary<string, int>();
        }

        private Grid baseGrid;
        public Grid BaseGrid
        {
            get { return baseGrid; }
            set { baseGrid = value; }
        }

        /// <summary>
        /// 当前索引
        /// </summary>
        private int currentIndex;
        public int CurrentIndex
        {
            get { return currentIndex; }
            set { currentIndex = value; }
        }

        /// <summary>
        /// 当前页面CSV文件名称
        /// </summary>
        private string csvName;
        public string CSVName
        {
            get { return csvName; }
            set { csvName = value; }
        }

        /// <summary>
        /// 数据索引
        /// </summary>
        private Dictionary<string, int> m_DataIndex;
        ComboBox DataListBox;
        GroupBox BaseGroup;
        StackPanel BasePanel;

        /// <summary>
        /// 父节点
        /// </summary>
        private MainWindow parentWin;
        public MainWindow ParentWindow
        {
            get { return parentWin; }
            set { parentWin = value; }
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public virtual void InitData(string csvname)
        {
            try
            {
                CSVName = csvname;

                var data = ParentWindow.m_DataBase[csvname];
                for (int i = 1; i <= data.Count; ++i)
                {
                    m_DataIndex.Add(data[i]["Index"], i);
                }

                DataListBox = new ComboBox
                {
                    ItemsSource = m_DataIndex,
                    SelectedValuePath = "Key",
                    DisplayMemberPath = "Key",
                    SelectedIndex = 0,
                    IsTextSearchEnabled = true,
                    IsEditable = true,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Width = 120,
                };
                DataListBox.SelectionChanged += new SelectionChangedEventHandler(DataListBox_SelectionChanged);

                BasePanel = new StackPanel();
                BasePanel.Children.Add(DataListBox);

                BaseGroup = new GroupBox
                {
                    Header = csvname + "列表",
                    Content = BasePanel
                };

                BaseGrid.Children.Add(BaseGroup);
                BaseSetAllData(m_DataIndex[DataListBox.SelectedValue.ToString()]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 列表项改变
        /// </summary>
        private void DataListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox mCB = sender as ComboBox;
            if (mCB.SelectedValue != null)
                BaseSetAllData(m_DataIndex[mCB.SelectedValue.ToString()]);
        }

        private void BaseSetAllData(int index)
        {
            if (index > 0)
            {
                CurrentIndex = index;
                SetAllData(index);
            }
        }

        /// <summary>
        /// 设置当前索引下的所有数据
        /// </summary>
        public virtual void SetAllData(int index)
        {

        }

        /// <summary>
        /// 获取指定页面某个字段的数据
        /// </summary>
        public string GetData(string csvname, int index, string field)
        {
            try
            {
                return ParentWindow.m_DataBase[csvname][index][field];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// 获取当前页面某个字段的数据
        /// </summary>
        public string GetCurrentPageData(int index, string field)
        {
            try
            {
                return ParentWindow.m_DataBase[CSVName][index][field];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// 修改指定页面某个字段的数据
        /// </summary>
        public bool SetData(string csvname, int index, string field,string value)
        {
            try
            {
                if (ParentWindow.m_DataBase[csvname][index][field] != value)
                {
                    ParentWindow.m_DataBase[csvname][index][field] = value;
                    ParentWindow.SetDataChange(csvname);
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
        /// 修改当前页面某个字段的数据
        /// </summary>
        public bool ChangeCurrentPageValue(string field, string value)
        {
            return SetData(CSVName, CurrentIndex, field, value);
        }

        /// <summary>
        /// 修改当前页面某个字段的数据
        /// </summary>
        public bool ChangeCurrentPageValue(string field, int value)
        {
            return SetData(CSVName, CurrentIndex, field, value.ToString());
        }

        /// <summary>
        /// 增加一列空数据
        /// </summary>
        public bool AddIndex(string csvname, string index)
        {
            try
            {
                var query = from d in ParentWindow.m_DataBase[csvname].AsQueryable()
                            where d.Value["Index"] == index
                            select d.Value;

                var indexdata = query.FirstOrDefault();
                if (indexdata == null)
                {
                    // 不存在的时候添加
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    foreach (var name in ParentWindow.m_DataHeader[csvname])
                    {
                        data.Add(name, "");
                    }

                    var value = m_DataIndex[m_DataIndex.Keys.Last().ToString()];
                    m_DataIndex[index] = value + 1;
                    DataListBox.Items.Refresh();
                    ParentWindow.m_DataBase[csvname].Add(value + 1, data);
                    SetData(CSVName, value + 1, "Index", index);
                    ParentWindow.SetDataChange(csvname);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 删除一列数据
        /// </summary>
        public bool DelIndex(string csvname)
        {
            try
            {
                var query = from d in ParentWindow.m_DataBase[csvname].AsQueryable()
                            where d.Key == CurrentIndex
                            select d.Value;

                var indexdata = query.FirstOrDefault();
                if (indexdata != null)
                {
                    var indexquery = from d in m_DataIndex.AsQueryable()
                                     where d.Value == CurrentIndex
                                     select d.Key;

                    var indexname = indexquery.FirstOrDefault();
                    if (indexname != null)
                    {
                        m_DataIndex.Remove(indexname);
                        ParentWindow.m_DataBase[csvname].Remove(CurrentIndex);
                        DataListBox.Items.Refresh();
                        DataListBox.SelectedIndex = 0;
                        if (m_DataIndex.Count > 0)
                        {
                            BaseSetAllData(m_DataIndex[DataListBox.SelectedValue.ToString()]);
                        }
                        ParentWindow.SetDataChange(csvname);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
        }
    }
}
