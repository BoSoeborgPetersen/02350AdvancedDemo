namespace _02350AdvancedDemo.View;

public partial class MainWindow : RibbonWindow
{
    MainViewModel VM;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = VM = Ioc.Default.GetService<MainViewModel>();
        VM.Init();
    }
}
