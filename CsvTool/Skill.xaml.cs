﻿
namespace CsvTool
{
    /// <summary>
    /// Skill.xaml 的交互逻辑
    /// </summary>
    public partial class Skill : BasePage
    {
        public Skill()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            BaseGrid = baseGrid;
        }

        /// <summary>
        /// 根据索引设置界面中的所有数据
        /// </summary>
        public override void SetAllData(int index)
        {

        }
    }
}
