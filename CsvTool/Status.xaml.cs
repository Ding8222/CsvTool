using System;
using System.Collections.Generic;
using System.Linq;
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

namespace CsvTool
{
    /// <summary>
    /// Status.xaml 的交互逻辑
    /// </summary>
    public partial class Status : BasePage
    {
        public Status()
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
