using System.Diagnostics;
using Mapidemic.Pages.Landing;
using Mapidemic.Pages.SymptomChecker;

namespace Mapidemic.Pages.Menu;

/// <summary>
/// A class that provides a user interface for selecting menu options
/// </summary>
public partial class MenuPage : ContentPage
{
	private HomePage? homePage;
	private const string cdcLink = "https://www.cdc.gov/";

	/// <summary>
	/// The default constructor for a MenuPage
	/// </summary>
	public MenuPage()
	{
		InitializeComponent();
	}
	
	/// <summary>
    /// A function that caches the HomePage so
	/// that new pages can be pushed onto the
	/// navigation stack
    /// </summary>
	protected override void OnAppearing()
    {
        homePage = (this.Parent as HomePage)!;
    }

	/// <summary>
	/// A function that displays the symptoms page
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	public async void ScButton_Clicked(object sender, EventArgs args)
	{
		Popup.IsOpen = true;
		await Task.Yield();
		try // attempting to load the symptoms list
		{
			await MauiProgram.businessLogic.LoadSymptomsList();
			await homePage!.GetViewport().PushAsync(new SymptomsPage());
			homePage.IsPresented = false;
		}
		catch (Exception error) // catching error if the database could not be reached
		{
			await HomePage.ShowPopup("Cannot open Symptom Checker");
			Debug.WriteLine(error.Message);
		}
		finally // closing activity indicator
        {
            Popup.IsOpen = false;
        }
	}

	/// <summary>
	/// A function that displays the meet the team page
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	public async void MttButton_Clicked(object sender, EventArgs args)
	{
		await homePage!.GetViewport().PushAsync(new MeetTheTeamPage());
		homePage.IsPresented = false;
	}

	/// <summary>
	/// A function that displays the who we are page
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	public async void PpButton_Clicked(object sender, EventArgs args)
	{
		await homePage!.GetViewport().PushAsync(new PrivacyPolicyPage());
		homePage.IsPresented = false;
	}

	/// <summary>
	/// A function that displays the feedback page
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	public async void FbButton_Clicked(object sender, EventArgs args)
	{
		await homePage!.GetViewport().PushAsync(new FeedbackPage());
		homePage.IsPresented = false;
	}

	/// <summary>
	/// A function that displays the contact information page
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	public async void IwcButton_Clicked(object sender, EventArgs args)
	{
		await homePage!.GetViewport().PushAsync(new InformationWeCollectPage());
		homePage.IsPresented = false;
	}

	/// <summary>
	/// A function that directs the user to the
	/// website of the CDC
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	public async void CdcButton_Clicked(object sender, EventArgs args)
	{
		await Launcher.Default.OpenAsync(new Uri(cdcLink));
	}
}