using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Mapidemic.Models;

namespace Mapidemic;

public partial class SymptomsPage : ContentPage
{
    public ObservableCollection<Symptom> SymptomList { get; set; }
    public SymptomsPage()
    {
        InitializeComponent();
        //SymptomList = MauiProgram.businessLogic.GetSymptomsList().Result;
        // BindingContext = this;

        _ = Method();
    }

    public bool Method()
    {
        SymptomList = MauiProgram.businessLogic.GetSymptomsList().Result;
        DisplayAlert("", $"{SymptomList.Count}", "OK");
        return true;
    }
}