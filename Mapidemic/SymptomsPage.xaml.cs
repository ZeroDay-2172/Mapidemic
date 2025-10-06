using System.Collections.ObjectModel;

namespace Mapidemic;

public partial class SymptomsPage : ContentPage
{
    private ObservableCollection<string> symptoms;
    public SymptomsPage()
    {
        InitializeComponent();
    }
}