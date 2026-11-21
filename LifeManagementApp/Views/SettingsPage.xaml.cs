using LifeManagementApp.ViewModels;

namespace LifeManagementApp.Views;

public partial class SettingsPage : ContentPage
{
	private readonly SettingsViewModel viewModel;

	public SettingsPage(SettingsViewModel viewModel)
	{
		InitializeComponent();
		this.viewModel = viewModel;
		BindingContext = viewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await viewModel.OnAppearingAsync();
	}
}