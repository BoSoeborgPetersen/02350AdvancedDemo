namespace _02350AdvancedDemo.ViewModel;

public class Dialogs
{
    static readonly OpenFileDialog openDialog = new() { Title = "Open Diagram", Filter = "JSON Document (.json)|*.json", DefaultExt = "json", InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), CheckFileExists = true };
    static readonly SaveFileDialog saveDialog = new() { Title = "Save Diagram", Filter = "JSON Document (.json)|*.json", DefaultExt = "json", InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) };

    public static bool ShowNew() => MessageBox.Show("Are you sure (bla bla)?", "Warning", MessageBoxButton.YesNo) == MessageBoxResult.Yes;

    public static string ShowOpen() => openDialog.ShowDialog() == true ? openDialog.FileName : null;

    public static string ShowSave() => saveDialog.ShowDialog() == true ? saveDialog.FileName : null;
}
