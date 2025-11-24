namespace Mapidemic;
using System;
using Mapidemic.Models;
using Microsoft.Maui.Controls;

public partial class FeedbackPage : ContentPage
{
    public FeedbackPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// A function that submits user feedback to the database
    /// </summary>
    /// <param name="feedback"></param>
    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        // Gather feedback data
        var message = FeedbackEditor.Text?.Trim();
        // Default to "General" if no category is selected
        var category = CategoryPicker.SelectedItem as string ?? "General";

        // Validate input
        if (string.IsNullOrWhiteSpace(message))
        {
            await DisplayAlert("Error! Missing feedback!",
                               "Please enter a message before submitting.",
                               "OK");
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
                StatusLabel.Text = "Thanks! Your feedback has been submitted.";
                StatusLabel.IsVisible = true;
                // Clear input field
                FeedbackEditor.Text = string.Empty; 
                // Reset category picker
                CategoryPicker.SelectedIndex = -1; 
            } 
            else
            {
                // Handle submission failure
                StatusLabel.Text = "Could not submit feedback. Please try again.";
                StatusLabel.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            // Handle submission failure
            StatusLabel.Text = "Could not submit feedback. Please try again.";
            StatusLabel.IsVisible = true;
            await DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            // Re-enable the submit button
            SubmitButton.IsEnabled = true;
        }
    }

    /// <summary>
    /// Clears status message when the feedback editor gains focus
    /// </summary>
    private void FeedbackEditor_Focused(object sender, FocusEventArgs e)
    {
        // Clear status message
        StatusLabel.Text = string.Empty;
        // Hide status label
        StatusLabel.IsVisible = false;
    }
}