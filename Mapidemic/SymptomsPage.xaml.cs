using System.Collections.ObjectModel;

namespace Mapidemic;

public partial class SymptomsPage : ContentPage
{
    private ObservableCollection<string> symptomList;
    public SymptomsPage()
    {
        InitializeComponent();
        symptomList = MauiProgram.businessLogic.GetSymptomsList();
    }
}