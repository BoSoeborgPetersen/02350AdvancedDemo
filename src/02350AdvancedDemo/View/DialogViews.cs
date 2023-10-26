namespace _02350AdvancedDemo.ViewModel;

public class DialogViews
{
    private static readonly OpenFileDialog openDialog = new() { Title = "Open Diagram", Filter = "XML Document (.xml)|*.xml", DefaultExt = "xml", InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), CheckFileExists = true };
    private static readonly SaveFileDialog saveDialog = new() { Title = "Save Diagram", Filter = "XML Document (.xml)|*.xml", DefaultExt = "xml", InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) };

    public static bool ShowNew() => MessageBox.Show("Are you sure (bla bla)?", "Warning", MessageBoxButton.YesNo) == MessageBoxResult.Yes;

    public static string ShowOpen() => openDialog.ShowDialog() == true ? openDialog.FileName : null;

    public static string ShowSave() => saveDialog.ShowDialog() == true ? saveDialog.FileName : null;
}
