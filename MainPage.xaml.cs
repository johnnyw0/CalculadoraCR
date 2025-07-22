using System.Collections.ObjectModel;
using System.Text.Json;

namespace CalculadoraCR;

public partial class  MainPage : ContentPage {

    private ObservableCollection<Materia> materias = new();

    private readonly string _filePath = Path.Combine(FileSystem.AppDataDirectory, "data.json");


    public MainPage() {
        
        InitializeComponent();
        LoadData();

        MateriasListView.ItemsSource = materias;
    }


    private void SaveData() {

        var json = JsonSerializer.Serialize(materias);
        File.WriteAllText(_filePath, json);

    }

    private void LoadData() {
        
        if(!File.Exists(_filePath)) {
            return;
        }

        var json = File.ReadAllText(_filePath);

        var loadedMaterias = JsonSerializer.Deserialize<ObservableCollection<Materia>>(json);

        if (loadedMaterias != null) {

            materias.Clear();
            foreach (var materia in loadedMaterias) {
                materias.Add(materia);
            }

        }

        UpdateCR();

    }

    private void UpdateCR() {

        double result = 0;
        int totalCargaHoraria = 0;
        foreach (var materia in materias) {
            result += materia.Media * materia.CargaHoraria;
            totalCargaHoraria += materia.CargaHoraria;
        }
        if (totalCargaHoraria > 0) {
            CrLabel.Text = $"CR: {result / totalCargaHoraria:F1}";
        } else {
            CrLabel.Text = "CR: N/A";
        }

        ClearButton.IsVisible = materias.Any();
    }

    private void OnAddMateriaClicked(object sender, EventArgs e) {
    }

    private void OnClearMateriasClicked(object sender, EventArgs e) {
    }
}