using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography; // For SHA-256
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Trackfy.Models;
using static Trackfy.Components.Layout.MainLayout;

namespace Trackfy.Components.Pages
{
    public partial class Register : ComponentBase
    {
        string username = "";
        string password = "";
        string first_name = "";
        string last_name = "";
        float initial_available_balance = 0.0f;
        float initial_debt_balance = 0.0f;

        [CascadingParameter]
        public RequiredDetails requiredDetails { get; set; }

        private static readonly string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private static readonly string FolderPath = Path.Combine(DesktopPath, "LocalDB");
        private static readonly string FilePath = Path.Combine(FolderPath, "appdata.json");

        // Load AppData (Users, Transactions, Debts) from JSON file
        private AppData LoadData()
        {
            if (!File.Exists(FilePath))
            {
                return new AppData(); // Return empty data if file doesn't exist
            }

            try
            {
                var json = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<AppData>(json) ?? new AppData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
                return new AppData();
            }
        }

        // Save AppData (Users, Transactions, Debts) to JSON file
        private void SaveData(AppData data)
        {
            EnsureFolderExists();

            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                var json = JsonSerializer.Serialize(data, options);
                File.WriteAllText(FilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data: {ex.Message}");
            }
        }

        private void EnsureFolderExists()
        {
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
        }

        private bool ValidateInput()
        {
            return !(string.IsNullOrEmpty(first_name) ||
                     string.IsNullOrEmpty(last_name) ||
                     string.IsNullOrEmpty(username) ||
                     string.IsNullOrEmpty(password));
        }

        // Method to hash the password using SHA-256
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convert the password to a byte array and compute the hash
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert the byte array to a hexadecimal string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashedBytes.Length; i++)
                {
                    builder.Append(hashedBytes[i].ToString("x2")); // "x2" formats the byte as a 2-digit hexadecimal
                }
                return builder.ToString();
            }
        }

        public async Task SignInUser()
        {
            try
            {
                if (!ValidateInput())
                {
                    await JS.InvokeVoidAsync("showAlert", "Provide all details correctly.");
                    return;
                }

                // Load the existing user data from the JSON file
                var appData = LoadData();
                requiredDetails.user_info_list = appData.Users ?? new List<UserModel>();

                if (requiredDetails.user_info_list.Any(x => x.Username == username))
                {
                    await JS.InvokeVoidAsync("showAlert", "Username already exists. Use a different one.");
                    return;
                }

                // Hash the password using SHA-256
                string hashedPassword = HashPassword(password);

                // Create new UserModel object with hashed password
                UserModel newUser = new UserModel(
                    user_username: username,
                    user_userPassword: hashedPassword, // Store the hashed password
                    user_AvailableBalance: initial_available_balance,
                    user_DebtBalance: initial_debt_balance,
                    user_firstName: first_name,
                    user_lastName: last_name,
                    user_type: "user"
                );

                // Add new user to the list
                requiredDetails.user_info_list.Add(newUser);

                // Save the updated data back to the JSON file
                SaveData(new AppData
                {
                    Users = requiredDetails.user_info_list,
                    Transactions = appData.Transactions,
                    Debts = appData.Debts
                });

                await JS.InvokeVoidAsync("showAlert", "Sign-in successful!");
                Navigation.NavigateTo("/register", forceLoad: false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Sign-in failed: {ex.Message}");
                await JS.InvokeVoidAsync("showAlert", "Sign-in failed. Please try again.");
            }
        }
    }
}