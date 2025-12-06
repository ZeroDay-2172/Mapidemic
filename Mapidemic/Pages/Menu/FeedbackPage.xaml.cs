using Mapidemic.Models;
using System.Diagnostics;
using Mapidemic.Pages.Landing;

namespace Mapidemic.Pages.Menu;

/// <summary>
/// A class that provides a user interface for providing the development team with feedback
/// </summary>
public partial class FeedbackPage : ContentPage
{
    /// <summary>
    /// The default constructor for the FeedbackPage
    /// </summary>
    public FeedbackPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// A function that submits user feedback to the database
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        // Gather category data
        string category;
        var option = CategoryPicker.SelectedItem;
        if (option == null)
        {
            await HomePage.ShowPopup("Please enter a category");
            return;
        }
        else
        {
            category = option.ToString()!;
        }
        
        // Gather feedback data
        var message = FeedbackEditor.Text?.Trim();

        // Validate input
        if (string.IsNullOrWhiteSpace(message))
        {
            await HomePage.ShowPopup("Please enter a message");
            return;
        }

        // Disable the submit button to prevent multiple submissions
        SubmitButton.IsEnabled = false;

        try
        {
            // Create feedback instance
            var feedback = new Feedback
            {
                Message = message,
                Category = category,
                CreatedAt = DateTimeOffset.UtcNow
            };

            // Call the business logic to submit feedback
            bool success = await MauiProgram.businessLogic.SubmitFeedbackAsync(feedback);
            
            if(success)
            {
                // Provide user feedback
                await HomePage.ShowPopup("Feedback submitted");
                // Clear input field
                FeedbackEditor.Text = string.Empty; 
                // Reset category picker
                CategoryPicker.SelectedIndex = -1; 
            } 
            else
            {
                // Handle submission failure
                await HomePage.ShowPopup("Unable to submit feedback");
            }
        }
        catch (Exception ex)
        {
            // Handle submission failure
            await HomePage.ShowPopup("Unable to submit feedback");
            Debug.WriteLine(ex.Message);
        }
        finally
        {
            // Re-enable the submit button
            SubmitButton.IsEnabled = true;
        }
    }
}