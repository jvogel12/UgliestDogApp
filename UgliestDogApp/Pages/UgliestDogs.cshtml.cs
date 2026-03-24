 using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

public class UgliestDogsModel : PageModel
{
    // List used to populate a dropdown menu of dogs
    public List<SelectListItem> DogList { get; set; }

    // Holds the selected dog's full details
    public Dog SelectedDog { get; set; }

    // Runs when the page is first loaded (GET request)
    public void OnGet()
    {
        LoadDogList(); // Populate dropdown list
    }

    // Runs when the form is submitted (POST request)
    public void OnPost(string selectedDog)
    {
        LoadDogList(); // Reload dropdown so it persists after post

        // Check if a dog was selected
        if (!string.IsNullOrEmpty(selectedDog))
        {
            // Convert selectedDog (string) to int and fetch dog details
            SelectedDog = GetDogById(int.Parse(selectedDog));
        }
    }

    // Loads all dogs (Id + Name) into the dropdown list
    private void LoadDogList()
    {
        DogList = new List<SelectListItem>(); // Initialize list

        // Create connection to SQLite database
        using (var connection = new SqliteConnection("Data Source=UgliestDogs.db"))
        {
            connection.Open(); // Open database connection

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name FROM Dogs"; // Query for dropdown data

            // Execute query and read results
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read()) // Loop through each row
                {
                    DogList.Add(new SelectListItem
                    {
                        Value = reader.GetInt32(0).ToString(), // Dog Id (value submitted)
                        Text = reader.GetString(1)             // Dog Name (shown in dropdown)
                    });
                }
            }
        }
    }

    // Retrieves full dog details based on selected Id
    private Dog GetDogById(int id)
    {
        using (var connection = new SqliteConnection("Data Source=UgliestDogs.db"))
        {
            connection.Open(); // Open database connection

            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Dogs WHERE Id = @Id"; // Query with parameter

            // Add parameter to prevent SQL injection
            command.Parameters.AddWithValue("@Id", id);

            using (var reader = command.ExecuteReader())
            {
                // If a matching record is found
                if (reader.Read())
                {
                    // Map database fields to Dog object
                    return new Dog
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Breed = reader.GetString(2),
                        Year = reader.GetInt32(3),
                        ImageFileName = reader.GetString(4)
                    };
                }
            }
        }

        return null; // Return null if no dog is found
    }
}

// Represents a Dog record from the database
public class Dog
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Breed { get; set; }
    public int Year { get; set; }
    public string ImageFileName { get; set; }
}