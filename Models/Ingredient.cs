
namespace eguiplanner_web_app.Models
{
    public class Ingredient
    {
        public string Title { get; set; }
        public float Amount { get; set; }
        public string Unit { get; set; }
        public Ingredient() { }
        public Ingredient(string newTitle, float newAmount, string newUnit)
        {
            Title = newTitle;
            Amount = newAmount;
            Unit = newUnit;
        }

    }
}
