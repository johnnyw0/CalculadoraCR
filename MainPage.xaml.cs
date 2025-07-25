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

    private async void OnAddMateriaClicked(object sender, EventArgs e)
    {
        string notaSanitizada = NotaEntry.Text.Replace(',', '.');

        bool notaValida = double.TryParse(notaSanitizada,
                                           System.Globalization.NumberStyles.Any,
                                           System.Globalization.CultureInfo.InvariantCulture,
                                           out double nota);

        if (string.IsNullOrWhiteSpace(NomeEntry.Text) ||
            !notaValida ||
            !int.TryParse(CargaHorariaEntry.Text, out int cargaHoraria))
        {
            await DisplayAlert("Erro", "Por favor, preenche todos os campos corretamente.", "OK");
            return;
        }

        var novaMateria = new Materia
        {
            Nome = NomeEntry.Text,
            Media = nota,
            CargaHoraria = cargaHoraria
        };

        materias.Add(novaMateria);

        NomeEntry.Text = string.Empty;
        NotaEntry.Text = string.Empty;
        CargaHorariaEntry.Text = string.Empty;

        UpdateCR();
        SaveData();
    }
    

    private void OnClearMateriasClicked(object sender, EventArgs e)
    {
        Dispatcher.Dispatch(async () =>
        {
            bool resposta = await DisplayAlert("Confirmação", "Tens a certeza que queres apagar todas as matérias?", "Sim, apagar", "Não");
            if (resposta)
            {
                materias.Clear();
                UpdateCR();
                SaveData();
            }
        });
    }

    private async void OnDeleteMateriaClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;

        var materiaParaExcluir = (Materia)button.BindingContext;

        bool confirmar = await DisplayAlert("Confirmar Exclusão",
                                           $"Tem a certeza de que quer excluir a matéria '{materiaParaExcluir.Nome}'?",
                                           "Sim, Excluir",
                                           "Não");

        if (confirmar)
        {
            materias.Remove(materiaParaExcluir);

            UpdateCR();
            SaveData();
        }
    }
}