using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace CsvTool
{
    /// <summary>
    /// Page2.xaml 的交互逻辑
    /// </summary>
    public partial class NPC : BasePage
    {
        Dictionary<int, string> m_NPCType;
        public NPC()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            BaseGrid = baseGrid;
            m_NPCType = new Dictionary<int, string>();
            m_NPCType.Add(0, "药店");
            m_NPCType.Add(1, "武器店");
            m_NPCType.Add(2, "杂货店");
            NPCTypeCB.ItemsSource = m_NPCType;
            NPCTypeCB.SelectedValuePath = "Key";
            NPCTypeCB.DisplayMemberPath = "Value";
        }

        /// <summary>
        /// 根据索引设置界面中的所有数据
        /// </summary>
        public override void SetAllData(int index)
        {
            // NPCID
            NPCIDTB.Text = GetCurrentPageData(index, NPCIDTB.Tag.ToString());
            // NPC类型
            int typeindex = 0;
            int.TryParse(GetCurrentPageData(index, NPCTypeCB.Tag.ToString()), out typeindex);
            NPCTypeCB.SelectedIndex = typeindex;
        }

        private void NPCTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox tb = sender as TextBox;
                ChangeCurrentPageValue(tb.Tag.ToString(), tb.Text);
            }
        }
    }
}
