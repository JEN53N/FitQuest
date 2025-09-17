using FitnessApp.Models;
using FitnessApp.Services;

namespace FitnessApp;

public partial class GreetingPage : ContentPage
{
    private readonly User _user;
    private string? _uploadedVideoPath;
    private readonly ApiService _apiService;

    public GreetingPage(User user)
    {
        InitializeComponent();
        _user = user;
        _apiService = new ApiService();
        SetupGreeting();
    }

    private void SetupGreeting()
    {
        GreetingLabel.Text = $"Hello {_user.Name}!\nWelcome to your fitness journey!";
    }

    private async void OnUploadVideoClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Select a video file",
                FileTypes = FilePickerFileType.Videos
            });

            if (result != null)
            {
                _uploadedVideoPath = result.FullPath;

                // Show BMI and analysis button
                BmiLabel.Text = $"Your BMI is: {_user.BMI}";
                BmiLabel.IsVisible = true;

                StatusLabel.Text = "Video uploaded successfully! Ready for analysis.";
                StatusLabel.IsVisible = true;

                AnalyzeButton.IsVisible = true;

                await DisplayAlert("Success", "Video uploaded successfully!", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to upload video: {ex.Message}", "OK");
        }
    }

    private async void OnAnalyzeClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_uploadedVideoPath))
        {
            await DisplayAlert("Error", "No video uploaded", "OK");
            return;
        }

        try
        {
            StatusLabel.Text = "Analyzing pushups... Please wait.";
            StatusLabel.TextColor = Colors.Orange;

            AnalyzeButton.IsEnabled = false;
            AnalyzeButton.Text = "Analyzing...";

            // Send video to localhost:8000 for analysis
            var result = await _apiService.AnalyzePushupsAsync(_uploadedVideoPath);

            if (result.Success)
            {
                ResultLabel.Text = $"Analysis Complete!\nPushups detected: {result.PushupCount}";
                ResultLabel.IsVisible = true;
                StatusLabel.Text = "Analysis completed successfully!";
                StatusLabel.TextColor = Colors.Green;
            }
            else
            {
                StatusLabel.Text = "Analysis failed. Please try again.";
                StatusLabel.TextColor = Colors.Red;
                await DisplayAlert("Error", result.ErrorMessage ?? "Analysis failed", "OK");
            }
        }
        catch (Exception ex)
        {
            StatusLabel.Text = "Error during analysis.";
            StatusLabel.TextColor = Colors.Red;
            await DisplayAlert("Error", $"Analysis failed: {ex.Message}", "OK");
        }
        finally
        {
            AnalyzeButton.IsEnabled = true;
            AnalyzeButton.Text = "Analyze Pushups";
        }
    }
}
