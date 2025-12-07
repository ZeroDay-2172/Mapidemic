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
    private const string alexEmail = "AlexGriep@outlook.com";
    private const string arthurEmail = "waldma51@uwosh.edu";
    private const string arthurLinkedIn = "https://www.linkedin.com/in/arthurwaldman40/";
    private const string connorEmail = "mcguireco30@uwosh.edu";
    private const string connorLinkedIn = "https://www.linkedin.com/in/connor-mcguire-7b946b2a0/";
    private const string connorHandshake = "https://app.joinhandshake.com/profiles/ckrk55";
    private const string nengEmail = "yangne53@uwosh.edu";
    private const string nengLinkedIn = "https://www.linkedin.com/in/nengyang93/";
    private const string nengHandshake = "https://uwosh.joinhandshake.com/profiles/s6zdka";

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
            await Launcher.Default.OpenAsync($"mailto:{alexEmail}");
        }
        catch (Exception)
        {
            // Show an error popup if unable to open the email client
            await HomePage.ShowPopup("Unable to open email client.");
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
            await Launcher.Default.OpenAsync($"mailto:{arthurEmail}");
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
            // Attempt to open the LinkedIn profile
            await Launcher.Default.OpenAsync(new Uri(arthurLinkedIn));
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
            await Launcher.Default.OpenAsync($"mailto:{connorEmail}");
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
            // Attempt to open the LinkedIn profile
            await Launcher.Default.OpenAsync(new Uri(connorLinkedIn));
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
            // Attempt to open the Handshake profile
            await Launcher.Default.OpenAsync(new Uri(connorHandshake));
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
            await Launcher.Default.OpenAsync($"mailto:{nengEmail}");
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
            // Attempt to open the LinkedIn profile
            await Launcher.Default.OpenAsync(new Uri(nengLinkedIn));
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
            // Attempt to open the Handshake profile
            await Launcher.Default.OpenAsync(new Uri(nengHandshake));
        }
        catch (Exception)
        {
            // Show an error popup if unable to open the Handshake profile
            await HomePage.ShowPopup("Unable to open Handshake profile.");
        }
    }
}