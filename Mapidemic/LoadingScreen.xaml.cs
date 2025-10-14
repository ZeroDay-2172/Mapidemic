//Author(s): Connor McGuire
//
//Note that this loading screen later on will likely
//contain some sort of animation that accompanies the
//progress bar.

using System.Threading.Tasks;

namespace Mapidemic;

public partial class LoadingScreen : ContentPage
{
    public LoadingScreen()
    {
        InitializeComponent();

        AnimateLoadingBar();
    }

    /// <summary>
    /// Function that animates the loadingBar.
    /// </summary>
    public async void AnimateLoadingBar()
    {
        for (int i = 0; i < 5; i++)
        {
            loadingBar.Progress = 0.0;
            await loadingBar.ProgressTo(0.5, 5000, Easing.BounceIn);
            await loadingBar.ProgressTo(0.75, 3000, Easing.Linear);
            await loadingBar.ProgressTo(0.75, 3000, Easing.Linear);
            await loadingBar.ProgressTo(1, 2500, Easing.BounceIn);
        }
    }
}