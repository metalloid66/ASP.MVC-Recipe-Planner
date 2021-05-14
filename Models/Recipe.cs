using System.Collections.Generic;
namespace eguiplanner_web_app.Models
{
    public class Recipe
    {
        public List<Ingredient> Ingredients = new List<Ingredient>();
        public string Title { get; set; }
        public string Description { get; set; }
        public Recipe() { }
        // Performing a deep copy for the list
        public Recipe(string title, string description, List<Ingredient> newIngredients)
        {
            Title = title;
            Description = description;
            for (int i = 0; i < newIngredients.Count; i++)
            {
                Ingredients.Add(new Ingredient(newIngredients[i].Title, newIngredients[i].Amount, newIngredients[i].Unit));
            }
        }
    }
}
