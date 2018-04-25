
namespace CsvTool
{
    /// <summary>
    /// MapMonster.xaml 的交互逻辑
    /// </summary>
    public partial class MapMonster : BasePage
    {
        public MapMonster()
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
