using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using eguiplanner_web_app.Models;
namespace eguiplanner_web_app.Controllers {
  public class RecipeController: Controller {
    static int counter = 0; //Used for calculation for RecipesShop
    static Recipe aRecipe = new Recipe();
    static List < Ingredient > ingsToAdd = new List < Ingredient > ();
    static List < Recipe > RecipesShop = new List < Recipe > ();
    static List < Recipe > RecipesList = new List < Recipe > ();

    [HttpGet]
    public IActionResult Index() {
      ViewData["RecipesList"] = RecipesList;
      return View();
    }

    [HttpGet]
    public IActionResult Create() {
        ViewData["Ingredients"] = ingsToAdd;
        if (!String.IsNullOrEmpty(aRecipe.Title))
          ViewData["RecipeName"] = aRecipe.Title;
        if (!String.IsNullOrEmpty(aRecipe.Description))
          ViewData["RecipeDescription"] = aRecipe.Description;
        return View();
      }
      [HttpGet]
    public IActionResult newIng() {
        ViewData["Ingredients"] = ingsToAdd;
        return View();
      }
      [HttpPost]
    public IActionResult CreateRecipe() {
        aRecipe.Title = Request.Form["Title"].ToString();
        aRecipe.Description = Request.Form["Description"].ToString();
        return RedirectToAction("Create", "Recipe");
      }
      [HttpPost]
    public IActionResult createIng() {
        Ingredient newIngredient = new Ingredient(Request.Form["ingredientName"].ToString(), Convert.ToSingle(Request.Form["ingredientAmount"]), Request.Form["ingredientUnit"].ToString());
        if (!ingsToAdd.Any())
          ingsToAdd.Add(newIngredient);
        else {
          bool sameItem = false;
          foreach(var item in ingsToAdd.ToList())
          if (item.Title == newIngredient.Title && item.Unit == newIngredient.Unit)
            sameItem = true;

          if (!sameItem)
            ingsToAdd.Add(newIngredient);
        }
        return RedirectToAction("newIng", "Recipe");
      }
      [HttpPost]
    public IActionResult deleteIng() {
      ingsToAdd.RemoveAt(Convert.ToInt32(Request.Form["valueToDelete"]));
      return RedirectToAction("newIng", "Recipe");
    }
    public IActionResult backFunc() {
      ViewData["RecipesList"] = RecipesList;
      ingsToAdd.Clear();
      RecipesShop.Clear();
      aRecipe.Title = null;
      aRecipe.Description = null;
      counter = 0;
      return View("Views/Recipe/Index.cshtml");
    }
    public IActionResult Finalize() {
      ViewData["Ingredients"] = ingsToAdd;
      if (!String.IsNullOrEmpty(aRecipe.Title))
        ViewData["RecipeName"] = aRecipe.Title;
      if (!String.IsNullOrEmpty(aRecipe.Description))
        ViewData["RecipeDescription"] = aRecipe.Description;

      return View();
    }

    public IActionResult Save(int id) {
      if (RecipesList.Count != 0) {
        var json = JsonConvert.SerializeObject(RecipesList, Formatting.Indented);
        if (id == 0) {
          System.IO.Directory.CreateDirectory("recipeDB/");
          System.IO.File.WriteAllText("recipeDB/db.json", json);
        }
      }
      return RedirectToAction("Index", "Recipe");
    }
    public IActionResult Load(int id) {
      if (id == 0) {
        if (System.IO.File.Exists("recipeDB/db.json"))
          RecipesList = JsonConvert.DeserializeObject < List < Recipe >> (System.IO.File.ReadAllText("recipeDB/db.json"));
      }
      return RedirectToAction("Index", "Recipe");
    }

    public IActionResult addRec() {
        List < Ingredient > newIngredientsListToAdd = new List < Ingredient > ();
        newIngredientsListToAdd.AddRange(ingsToAdd);
        Recipe newRecipeToAdd = new Recipe(aRecipe.Title, aRecipe.Description, newIngredientsListToAdd);
        if (!RecipesList.Contains(newRecipeToAdd))
          RecipesList.Add(newRecipeToAdd);
        return RedirectToAction("backFunc", "Recipe");
      }
      [HttpPost]
    public IActionResult handleRecBtns(int id, string action) {
      var detailsValue = Request.Form["detailsValue"];
      var modfiyValue = Request.Form["modfiyValue"];
      var valueToDelete = Request.Form["deleteValue"];

      if (detailsValue.Count != 0) {
        ViewData["RecipeToView"] = id;
        ViewData["RecipesList"] = RecipesList;

        return View("Views/Recipe/RecipeDetails.cshtml");
      } else if (valueToDelete.Count != 0) {
        RecipesList.RemoveAt(id);
        return RedirectToAction("Index", "Recipe");
      } else if (modfiyValue.Count != 0) {
        ViewData["RecipeToEdit"] = id;
        ViewData["RecipesList"] = RecipesList;
        return View("Views/Recipe/Modify.cshtml");
      }
      return RedirectToAction("Index", "Recipe");
    }
    public IActionResult Edit(int id) {
      ViewData["RecipeToEdit"] = id;
      ViewData["RecipesList"] = RecipesList;
      return View();
    }
    public IActionResult EditRecInfo(int id) {
      RecipesList[id].Title = Request.Form["Title"].ToString();
      RecipesList[id].Description = Request.Form["Description"].ToString();
      return RedirectToAction("Modify", "Recipe", new {
        Id = id
      });
    }
    public IActionResult ModifyIngredients(int id) {
      ViewData["IngredientsToEdit"] = RecipesList[id].Ingredients;
      ViewData["ID"] = id;
      return View();
    }
    public IActionResult editDeleteIng(int id) {
      RecipesList[id].Ingredients.RemoveAt(Convert.ToInt32(Request.Form["valueToDelete"]));
      return RedirectToAction("ModifyIngredients", "Recipe", new {
        Id = id
      });
    }
    public IActionResult editIng(int id) {
      Ingredient newIngredient = new Ingredient(Request.Form["ingredientName"].ToString(), Convert.ToSingle(Request.Form["ingredientAmount"]), Request.Form["ingredientUnit"].ToString());
      if (!RecipesList[id].Ingredients.Any())
        RecipesList[id].Ingredients.Add(newIngredient);
      else {
        bool sameItem = false;
        foreach(var item in RecipesList[id].Ingredients.ToList())
        if (item.Title == newIngredient.Title && item.Unit == newIngredient.Unit)
          sameItem = true;

        if (!sameItem)
          RecipesList[id].Ingredients.Add(newIngredient);
      }
      return RedirectToAction("ModifyIngredients", "Recipe", new {
        Id = id
      });

    }
    public IActionResult Shop() {
      ViewData["RecipesList"] = RecipesList;
      ViewData["AddedRecipes"] = RecipesShop;
      ViewData["ingsToAdd"] = ingsToAdd;
      return View();
    }
    public IActionResult addRecToShop(int id) {
      RecipesShop.Add(new Recipe(RecipesList[id].Title,
        RecipesList[id].Description,
        RecipesList[id].Ingredients));
      if (ingsToAdd.Count == 0) {
        for (int j = 0; j < RecipesShop[counter].Ingredients.Count; j++) {
          ingsToAdd.Add(new Ingredient(RecipesShop[counter].Ingredients[j].Title,
            RecipesShop[counter].Ingredients[j].Amount,
            RecipesShop[counter].Ingredients[j].Unit));
        }
      } else {
        for (int j = 0; j < RecipesShop[counter].Ingredients.Count; j++) {
          if (ingsToAdd.Any(x => x.Title == RecipesShop[counter].Ingredients[j].Title && x.Unit == RecipesShop[counter].Ingredients[j].Unit)) {
            int indexValue = ingsToAdd.FindIndex(x => x.Title == RecipesShop[counter].Ingredients[j].Title && x.Unit == RecipesShop[counter].Ingredients[j].Unit);
            if (indexValue != (-1))
              ingsToAdd[indexValue].Amount += RecipesShop[counter].Ingredients[j].Amount;
          } else {
            ingsToAdd.Add(new Ingredient(RecipesShop[counter].Ingredients[j].Title,
              RecipesShop[counter].Ingredients[j].Amount,
              RecipesShop[counter].Ingredients[j].Unit));
          }
        }
      }
      counter++;
      ingsToAdd.Sort((x, y) => string.Compare(x.Title, y.Title));
      return RedirectToAction("Shop", "Recipe");
    }
    public IActionResult removeRecFromShop(int id) {
      if (ingsToAdd.Count != 0) {
        for (int j = 0; j < RecipesShop[id].Ingredients.Count; j++) {
          if (ingsToAdd.Any(x => x.Title == RecipesShop[id].Ingredients[j].Title && x.Unit == RecipesShop[id].Ingredients[j].Unit)) {
            int indexValue = ingsToAdd.FindIndex(x => x.Title == RecipesShop[id].Ingredients[j].Title && x.Unit == RecipesShop[id].Ingredients[j].Unit);
            if (indexValue != (-1)) {
              ingsToAdd[indexValue].Amount -= RecipesShop[id].Ingredients[j].Amount;
              if (ingsToAdd[indexValue].Amount <= 0)
                ingsToAdd.RemoveAt(indexValue);
            }
          }
        }
      }
      if (RecipesShop.Count != 0)
        RecipesShop.RemoveAt(id);
      counter--;
      return RedirectToAction("Shop", "Recipe");
    }
  }
}