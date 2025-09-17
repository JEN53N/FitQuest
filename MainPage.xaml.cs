using FitnessApp.Models;

namespace FitnessApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnSignupClicked(object sender, EventArgs e)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
            string.IsNullOrWhiteSpace(AgeEntry.Text) ||
            string.IsNullOrWhiteSpace(EmailEntry.Text) ||
            string.IsNullOrWhiteSpace(HeightEntry.Text) ||
            string.IsNullOrWhiteSpace(WeightEntry.Text))
        {
            await DisplayAlert("Error", "Please fill in all fields", "OK");
            return;
        }

        // Validate numeric inputs
        if (!int.TryParse(AgeEntry.Text, out int age) ||
            !double.TryParse(HeightEntry.Text, out double height) ||
            !double.TryParse(WeightEntry.Text, out double weight))
        {
            await DisplayAlert("Error", "Please enter valid numbers for age, height, and weight", "OK");
            return;
        }

        // Calculate BMI
        double bmi = weight / Math.Pow(height / 100, 2);

        // Create user model
        var user = new User
        {
            Name = NameEntry.Text,
            Age = age,
            Email = EmailEntry.Text,
            Height = height,
            Weight = weight,
            BMI = Math.Round(bmi, 1)
        };

        // Navigate to greeting page
        await Navigation.PushAsync(new GreetingPage(user));
    }
}