using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Trackfy.Models;
using static Trackfy.Components.Layout.MainLayout;

namespace Trackfy.Components.Pages
{
    public partial class Login : ComponentBase
    {
        public string username { get; set; } = "";
        public string password { get; set; } = "";
        public string currencyType { get; set; } = "";

        [CascadingParameter]
        public RequiredDetails requiredDetails { get; set; }

        // Alert message variables
        private string AlertMessage = string.Empty;
        private string AlertClass = string.Empty;

        // Method to handle form submission for login
        private async Task LoginUser()
        {
            if (string.IsNullOrEmpty(username))
            {
                await JS.InvokeVoidAsync("showAlert", "Enter username and try again.");
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                await JS.InvokeVoidAsync("showAlert", "Enter password and try again.");
                return;
            }

            // Check if the user exists in the user list
            var user_details = requiredDetails.user_info_list
                .FirstOrDefault(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (user_details != null)
            {
                if (user_details.UserPassword == password)
                {
                    // Successful login, handle first login scenario
                    if (user_details.UserType == "user")
                    {
                        requiredDetails.CurrencyTypeUser = currencyType;
                        requiredDetails.CurrentUserUsername = username;

                        var first_login_currency_type = new Dictionary<string, string>
                        {
                            { "user_username", requiredDetails.CurrentUserUsername },
                            { "firstlogin_currency_type", requiredDetails.CurrencyTypeUser }
                        };

                        // Check if this user is logging in for the first time
                        if (requiredDetails.FirstLoginCurrencyType.Any())
                        {
                            bool already_login = CheckUserFirstLogin(requiredDetails.FirstLoginCurrencyType, "user_username", requiredDetails.CurrentUserUsername);
                            if (already_login)
                            {
                                string first_login_currency_type_value = GetFirstLoginCurrencyType(requiredDetails.CurrentUserUsername);

                                // Convert available balance and debt balance using exchange rates
                                user_details.UserAvailableBalance = ConvertCurrency(user_details.UserAvailableBalance, first_login_currency_type_value, requiredDetails.CurrencyTypeUser) ?? user_details.UserAvailableBalance;
                                user_details.UserDebtBalance = ConvertCurrency(user_details.UserDebtBalance, first_login_currency_type_value, requiredDetails.CurrencyTypeUser) ?? user_details.UserDebtBalance;

                                // Show success and redirect
                                await JS.InvokeVoidAsync("showAlert", $"Login Success. Welcome to user home page. Current Currency: {GetCurrencySymbol(requiredDetails.CurrencyTypeUser)}");
                                Navigation.NavigateTo("/dashboard");
                                return;
                            }

                            // Handle first login with new currency type
                            requiredDetails.FirstLoginCurrencyType.Add(first_login_currency_type);
                            await JS.InvokeVoidAsync("showAlert", $"Login Success. Welcome to Dashboard. Current Currency: {GetCurrencySymbol(requiredDetails.CurrencyTypeUser)}");
                            Navigation.NavigateTo("/dashboard");
                            return;
                        }

                        // Add first login data for the user
                        requiredDetails.FirstLoginCurrencyType.Add(first_login_currency_type);
                        await JS.InvokeVoidAsync("showAlert", $"Login Success. Welcome to user home page. Current Currency: {GetCurrencySymbol(requiredDetails.CurrencyTypeUser)}");
                        Navigation.NavigateTo("/Dashboard");
                        return;
                    }
                }
                else
                {
                    // Incorrect password
                    await JS.InvokeVoidAsync("showAlert", "Password incorrect.");
                    return;
                }
            }
            else
            {
                // Username doesn't exist
                await JS.InvokeVoidAsync("showAlert", "Username doesn't exist.");
                return;
            }
        }

       

        // Check if the user is logging in for the first time
        public bool CheckUserFirstLogin(
            List<Dictionary<string, string>> listOfDictionaries,
            string keyToFind,
            string valueToMatch)
        {
            return listOfDictionaries.Any(dictionary => dictionary.ContainsKey(keyToFind) && dictionary[keyToFind] == valueToMatch);
        }

        // Exchange rates dictionary for converting between currencies
        private Dictionary<string, float> ExchangeRatesToUSD = new()
        {
            { "USD", 1.0f },
            { "NPR", 0.0076f },
            { "YEN", 1.26f },
            { "INR", 0.012f }
        };

        // Currency symbols dictionary
        private Dictionary<string, string> CurrencySymbols = new()
        {
            { "USD", "$" },
            { "NPR", "Rs." },
            { "YEN", "¥" },
            { "INR", "₹" }
        };

        // Get currency symbol based on currency type
        public string GetCurrencySymbol(string currencyType)
        {
            return CurrencySymbols.ContainsKey(currencyType) ? CurrencySymbols[currencyType] : "";
        }

        // Convert the amount from one currency to another
        public float? ConvertCurrency(float amount, string fromCurrency, string toCurrency)
        {
            try
            {
                if (fromCurrency == toCurrency)
                {
                    return amount; // No conversion needed if same currency
                }

                // Get exchange rates
                float fromCurrencyToUSD = ExchangeRatesToUSD.ContainsKey(fromCurrency) ? ExchangeRatesToUSD[fromCurrency] : 1.0f;
                float toCurrencyToUSD = ExchangeRatesToUSD.ContainsKey(toCurrency) ? ExchangeRatesToUSD[toCurrency] : 1.0f;

                // Convert to USD first, then to the target currency
                float amountInUSD = amount / fromCurrencyToUSD;
                return amountInUSD * toCurrencyToUSD;
            }
            catch
            {
                return amount; // If error occurs, return the original amount
            }
        }

        // Retrieve the currency type for the first login
        public string GetFirstLoginCurrencyType(string username)
        {
            var userDetails = requiredDetails.FirstLoginCurrencyType
                .FirstOrDefault(userDetails => userDetails.ContainsKey("user_username") && userDetails["user_username"] == username);
            return userDetails != null ? userDetails["firstlogin_currency_type"] : null;
        }
    }
}
