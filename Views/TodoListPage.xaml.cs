using LifeManagementApp.ViewModels;

namespace LifeManagementApp.Views;

public partial class TodoListPage : ContentPage
{
	private readonly TodoListViewModel viewModel;
	public TodoListPage(TodoListViewModel viewModel)
	{
		InitializeComponent();
		this.viewModel = viewModel;
		BindingContext = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
		await viewModel.LoadAsync();		// NEW (task 10): from the repository - only the first time
		viewModel.ApplySort();
    }
}