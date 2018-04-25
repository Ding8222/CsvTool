
namespace CsvTool
{
    /// <summary>
    /// InstanceMonster.xaml 的交互逻辑
    /// </summary>
    public partial class InstanceMonster : BasePage
    {
        public InstanceMonster()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            BaseGrid = baseGrid;
        }
    }
}
