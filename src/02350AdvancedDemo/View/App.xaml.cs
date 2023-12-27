namespace _02350AdvancedDemo.View;

public partial class App : Application
{
    public App()
    {
        Ioc.Default.ConfigureServices(
            new ServiceCollection()
                .AddSingleton<Repository>()

                //.AddAutoMapper(typeof(App))

                .AddSingleton<ClipboardService>()
                .AddSingleton<MappingService>()
                .AddSingleton<MouseService>()
                .AddSingleton<SelectionBoxService>()
                .AddSingleton<StateService>()

                .AddSingleton<UndoRedoController>()

                .AddTransient<Dialogs>()
                .AddTransient<MainViewModel>()
                .AddTransient<SidePanelViewModel>()
                .AddTransient<LineViewModel>()
                .AddSingleton<Func<LineViewModel>>(sp => sp.GetService<LineViewModel>)
                .AddTransient<DashLineViewModel>()
                .AddSingleton<Func<DashLineViewModel>>(sp => sp.GetService<DashLineViewModel>)
                .AddTransient<SquareViewModel>()
                .AddTransient<CircleViewModel>()
                .BuildServiceProvider());
    }
}
