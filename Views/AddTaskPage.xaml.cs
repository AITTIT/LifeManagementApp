using LifeManagementApp.ViewModels;

namespace LifeManagementApp.Views;

public partial class AddTaskPage : ContentPage
{
    private readonly AddTaskViewModel viewModel;    // keep a reference
    public AddTaskPage(AddTaskViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = viewModel;
    }

    // pure UI reaction → an event is still the right tool
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        TitleEntry.Focus();
        await viewModel.OnAppearingAsync();
    }
}