using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace CsvTool
{
    /// <summary>
    /// Monster.xaml 的交互逻辑
    /// </summary>
    public partial class Monster : BasePage
    {
        Dictionary<int, string> m_MonsterType;

        public Monster()
        {
            InitializeComponent();
            Init();
        }
        
        private void Init()
        {
            BaseGrid = baseGrid;
            m_MonsterType = new Dictionary<int, string>();
            m_MonsterType.Add(0, "普通怪物");
            m_MonsterType.Add(1, "BOSS");
            MonsterTypeCB.ItemsSource = m_MonsterType;
            MonsterTypeCB.SelectedValuePath = "Key";
            MonsterTypeCB.DisplayMemberPath = "Value";
        }

        /// <summary>
        /// 根据索引设置界面中的所有数据
        /// </summary>
        public override void SetAllData(int index)
        {
            // 怪物ID
            MonsterIDTB.Text = GetCurrentPageData(index, MonsterIDTB.Tag.ToString());
            // 怪物名称
            MonsterNameTB.Text = GetCurrentPageData(index, MonsterNameTB.Tag.ToString());
            // 怪物类型
            int typeindex = 0;
            int.TryParse(GetCurrentPageData(index, MonsterTypeCB.Tag.ToString()), out typeindex);
            MonsterTypeCB.SelectedIndex = typeindex;
        }

        private void MonsterTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                TextBox tb = sender as TextBox;
                ChangeCurrentPageValue(tb.Tag.ToString(), tb.Text);
            }
        }

        private void MonsterTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox mCB = sender as ComboBox;
            ChangeCurrentPageValue(mCB.Tag.ToString(),mCB.SelectedIndex);
        }
        
    }
}
