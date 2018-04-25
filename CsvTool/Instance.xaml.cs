
namespace CsvTool
{
    /// <summary>
    /// Instance.xaml 的交互逻辑
    /// </summary>
    public partial class Instance : BasePage
    {
        public Instance()
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
