using System.Numerics;
using System.Text;
using System.Windows;

namespace track_widths.Desktop.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int amperage;
        public int width;
        public int thikness;
        public int rise_t;
        public int аmbient_t;
        public int lenght;

        public class Input_menu
        {
            public string Dimension_table { get; set; } = " ";
            public double Multi { get; set; } = 0.0;
        }


        


        public MainWindow()
        {
            InitializeComponent();
            InitializeCombobox();
           
        }


        public void InitializeCombobox()
        {
            amperageCombobox.ItemsSource = new Input_menu[]
            {
                new() {Dimension_table = "A", Multi = 1.0},
                new() {Dimension_table = "mA", Multi = 0.001},
                new() {Dimension_table = "mkA", Multi = 0.000001}
            };


            widthCombobox.ItemsSource = new Input_menu[]
            {
                new() {Dimension_table = "мил", Multi = 0.0254},
                new() {Dimension_table = "см", Multi = 10.0},
                new() {Dimension_table = "мм", Multi = 1.0},
                new() {Dimension_table = "мкм", Multi = 0.001},
                new() {Dimension_table = "дюйм", Multi = 25.4},
            };


            thiknessCombobox.ItemsSource = new Input_menu[]
            {
                new() {Dimension_table = "унция/фут^2", Multi = 0.035},
                new() {Dimension_table = "мил", Multi = 0.0254},
                new() {Dimension_table = "см", Multi = 10.0},
                new() {Dimension_table = "мм", Multi = 1.0},
                new() {Dimension_table = "мкм", Multi = 0.001},
                new() {Dimension_table = "дюйм", Multi = 25.4}
            };


            rise_tCombobox.ItemsSource = new Input_menu[]
            {
                new() {Dimension_table = "C", Multi = 1.0},
                new() {Dimension_table = "K", Multi = 0.0254},
            };
        }


    }
}