using System;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Mapidemic.Pages.Landing;

namespace Mapidemic.Pages.Menu;

/// <summary>
/// A class that provides a user interface for displaying information on the Mapidemic team
/// </summary>
public partial class MeetTheTeamPage : TabbedPage
{
    /// <summary>
    /// The default constructor for a MeetTheTeamPage
    /// </summary>
    public MeetTheTeamPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Opens the default email client to contact Alex Griep
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnAlexEmailClicked(object sender, EventArgs e)
    {
        try // Open the default email client
        {
            // Attempt to open the email client with Alex's email address
            //await Launcher.OpenAsync("mailto:INSERT_EMAIL_HERE");
        }
        catch (Exception)
        {
            // Show an error popup if unable to open the email client
            await HomePage.ShowPopup("Unable to open email client.");
        }
    }

    /// <summary>
    /// Opens Alex Griep's LinkedIn profile in the default web browser
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnAlexLinkedInClicked(object sender, EventArgs e)
    {
        try // Open the default web browser
        {
            // Create a URI for Alex's LinkedIn profile
            //var uri = new Uri("INSERT_HTTPS_LINK_HERE");
            // Attempt to open the LinkedIn profile
            //await Launcher.OpenAsync(uri);
        }
        catch (Exception)
        {
            // Show an error popup if unable to open the LinkedIn profile
            await HomePage.ShowPopup("Unable to open LinkedIn profile.");
        }
    }

    /// <summary>
    /// Opens the default email client to contact Arthur Waldman
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnArthurEmailClicked(object sender, EventArgs e)
    {
        try // Open the default email client
        {
            // Attempt to open the email client with Arthur's email address
            await Launcher.OpenAsync("mailto:Waldma51@uwosh.edu");
        }
        catch (Exception)
        {
            // Show an error popup if unable to open the email client
            await HomePage.ShowPopup("Unable to open email client.");
        }
    }

    /// <summary>
    /// Opens Arthur Waldman's LinkedIn profile in the default web browser
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnArthurLinkedInClicked(object sender, EventArgs e)
    {
        try // Open the default web browser
        {
            // Create a URI for Arthur's LinkedIn profile
            var uri = new Uri("https://www.linkedin.com/in/arthurwaldman40/");
            // Attempt to open the LinkedIn profile
            await Launcher.OpenAsync(uri);
        }
        catch (Exception)
        {
            // Show an error popup if unable to open the LinkedIn profile
            await HomePage.ShowPopup("Unable to open LinkedIn profile.");
        }
    }

    /// <summary>
    /// Opens the default email client to contact Connor McGuire
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnConnorEmailClicked(object sender, EventArgs e)
    {
        try // Open the default email client
        {
            // Attempt to open the email client with Connor's email address
            await Launcher.OpenAsync("mailto:mcguireco30@uwosh.edu");
        }
        catch (Exception)
        {
            // Show an error popup if unable to open the email client
            await HomePage.ShowPopup("Unable to open email client.");
        }
    }

    /// <summary>
    /// Opens Connor McGuire's LinkedIn profile in the default web browser
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnConnorLinkedInClicked(object sender, EventArgs e)
    {
        try // Open the default web browser
        {
            // Create a URI for Connor's LinkedIn profile
            var uri = new Uri("https://www.linkedin.com/in/connor-mcguire-7b946b2a0/");
            // Attempt to open the LinkedIn profile
            await Launcher.OpenAsync(uri);
        }
        catch (Exception)
        {
            // Show an error popup if unable to open the LinkedIn profile
            await HomePage.ShowPopup("Unable to open LinkedIn profile.");
        }
    }

    /// <summary>
    /// Opens Connor McGuire's Handshake profile in the default web browser
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnConnorHandshakeClicked(object sender, EventArgs e)
    {
        try // Open the default web browser
        {
            // Create a URI for Connor's Handshake profile
            var uri = new Uri("https://app.joinhandshake.com/profiles/ckrk55");
            // Attempt to open the Handshake profile
            await Launcher.OpenAsync(uri);
        }
        catch (Exception)
        {
            // Show an error popup if unable to open the Handshake profile
            await HomePage.ShowPopup("Unable to open Handshake profile.");
        }
    }

    /// <summary>
    /// Opens the default email client to contact Neng Yang
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnNengEmailClicked(object sender, EventArgs e)
    {
        try // Open the default email client
        {
            // Attempt to open the email client with Neng's email address
            await Launcher.OpenAsync("mailto:yangne53@uwosh.edu");
        }
        catch (Exception)
        {
            // Show an error popup if unable to open the email client
            await HomePage.ShowPopup("Unable to open email client.");
        }
    }

    /// <summary>
    /// Opens Neng Yang's LinkedIn profile in the default web browser
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnNengLinkedInClicked(object sender, EventArgs e)
    {
        try // Open the default web browser
        {
            // Create a URI for Neng's LinkedIn profile
            var uri = new Uri("https://www.linkedin.com/in/nengyang93/");
            // Attempt to open the LinkedIn profile
            await Launcher.OpenAsync(uri);
        }
        catch (Exception)
        {
            // Show an error popup if unable to open the LinkedIn profile
            await HomePage.ShowPopup("Unable to open LinkedIn profile.");
        }
    }

    /// <summary>
    /// Opens Neng Yang's Handshake profile in the default web browser
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnNengHandshakeClicked(object sender, EventArgs e)
    {
        try // Open the default web browser
        {
            // Create a URI for Neng's Handshake profile
            var uri = new Uri("https://uwosh.joinhandshake.com/profiles/s6zdka");
            // Attempt to open the Handshake profile
            await Launcher.OpenAsync(uri);
        }
        catch (Exception)
        {
            // Show an error popup if unable to open the Handshake profile
            await HomePage.ShowPopup("Unable to open Handshake profile.");
        }
    }
}