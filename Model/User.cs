namespace FitnessApp.Models
{
    public class User
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Email { get; set; } = string.Empty;
        public double Height { get; set; }
        public double Weight { get; set; }
        public double BMI { get; set; }
    }
}