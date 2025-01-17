using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trackfy.Models;
using static Trackfy.Components.Layout.MainLayout;

namespace Trackfy.Components.Pages
{
    public partial class Dashboard: ComponentBase
    {
        public string TransactionsFlow { get; set; }
        public string TransactionsType { get; set; }
        public DateTime TransactionsDate { get; set; }
        public float TransactionsAmount { get; set; }
        public string TransactionsSource { get; set; }
        public string TransactionsNote { get; set; }
        public string TransactionsTittle { get; set; }
        public string TransactionsId { get; set; }
        public string DebtId { get; set; }

        public bool IsDateRangePickerVisible { get; set; } = false;
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }


        public float HighestInflowTransaction { get; set; } = 0.0f;
        public float LowestInflowTransaction { get; set; } = 0.0f;
        public float HighestOutflowTransaction { get; set; } = 0.0f;
        public float LowestOutflowTransaction { get; set; } = 0.0f;
        public float TotalInflow { get; set; } = 0.0f;
        public float TotalOutflow { get; set; } = 0.0f;

        // Properties for Debts
        public float HighestDebt { get; set; } = 0.0f;
        public float LowestDebt { get; set; } = 0.0f;

        public string SearchTittleTransactions { get; set; } = "";
        public string SortTansactionsType { get; set; } = "";

        [CascadingParameter]
        public RequiredDetails requiredDetails { get; set; }


        protected override void OnParametersSet()
        {
            UpdateTransactionProperties();
            UpdateDebtProperties();
            StateHasChanged();
        }


        public void ToggleDateRangePicker()
        {
            IsDateRangePickerVisible = !IsDateRangePickerVisible;
        }

        

        public async Task<int> ClearDebt()
        {
            try
            {
                if (GetUserDebtBalance() <= 0.0f)
                {
                    await JS.InvokeVoidAsync("console.log", "Debt not clear.No debt to pay.");
                    await JS.InvokeVoidAsync("showAlert", "Debt not clear.No debt to pay.");
                    return 1;
                }
                if (GetUserAvailableBalance() > GetUserDebtBalance())
                {
                    float remain_amount = GetUserAvailableBalance() - GetUserDebtBalance();
                    var user_data = requiredDetails.user_info_list.FirstOrDefault(x => x.Username == requiredDetails.CurrentUserUsername);
                    user_data.UserDebtBalance = 0.0f;
                    user_data.UserAvailableBalance = remain_amount;
                    await JS.InvokeVoidAsync("console.log", "Debt clear.");
                    await JS.InvokeVoidAsync("showAlert", "Debt clear.");
                    return 1;
                }
                if (GetUserAvailableBalance() < GetUserDebtBalance())
                {
                    await JS.InvokeVoidAsync("console.log", "Debt not clear.Insufficient balance.");
                    await JS.InvokeVoidAsync("showAlert", "Debt not clear.Insufficient balance.");
                    return 1;
                }
                await JS.InvokeVoidAsync("console.log", "Clear debt fail.Try again.");
                await JS.InvokeVoidAsync("showAlert", "Clear debt fail.Try again.");
                return 2;
            }
            catch (Exception obj)
            {
                await JS.InvokeVoidAsync("console.log", "Exception caught.");
                await JS.InvokeVoidAsync("console.log", $"{obj.ToString()}");
                await JS.InvokeVoidAsync("showAlert", "Clear debt fail.Try again.");
                return 3;
            }

        }

        public async Task<int> CheckOutflow()
        {
            try
            {
                if (String.IsNullOrEmpty(TransactionsAmount.ToString()))
                {
                    await JS.InvokeVoidAsync("console.log", $"Update fail.Empty transactions amount.");
                    await JS.InvokeVoidAsync("showAlert", "Update fail.Empty transactions transactions amount");
                    return 0;
                }

                if (GetUserAvailableBalance() >= TransactionsAmount)
                {
                    await JS.InvokeVoidAsync("console.log", "Sufficient amount for outflow.");
                    await JS.InvokeVoidAsync("showAlert", "Sufficient amount for outflow.");
                    return 1;
                }

                else
                {
                    await JS.InvokeVoidAsync("console.log", "InSufficient amount for outflow.");
                    await JS.InvokeVoidAsync("showAlert", "InSufficient amount for outflow.");
                    return 2;
                }

            }
            catch (Exception obj)
            {
                await JS.InvokeVoidAsync("console.log", "Exception caught.");
                await JS.InvokeVoidAsync("console.log", $"{obj.ToString()}");
                await JS.InvokeVoidAsync("showAlert", "Check outflow fail.Try again.");
                return 3;
            }
        }

        public async Task<int> UpdateTittleTransactions()
        {
            try
            {

                if (String.IsNullOrEmpty(TransactionsId) || String.IsNullOrEmpty(TransactionsTittle))
                {
                    await JS.InvokeVoidAsync("console.log", $"Update fail.Empty transactions Id or tittle.");
                    await JS.InvokeVoidAsync("showAlert", "Update fail.Empty transactions Id  or tittle.");
                    return 0;
                }
                var transactions_data = requiredDetails.transactions_info_list.FirstOrDefault(x => x.UserUsername == requiredDetails.CurrentUserUsername && x.TrId == TransactionsId);
               

                if (transactions_data != null )
                {

                    transactions_data.TrTitle = TransactionsTittle;
                
                    await JS.InvokeVoidAsync("console.log", $"Update success");
                    await JS.InvokeVoidAsync("showAlert", "Update success");
                    return 1;
                }
                await JS.InvokeVoidAsync("console.log", $"Update fail");
                await JS.InvokeVoidAsync("showAlert", "Update fail");
                return 0;

            }
            catch (Exception obj)
            {
                await JS.InvokeVoidAsync("console.log", "Exception caught.");
                await JS.InvokeVoidAsync("console.log", $"{obj.ToString()}");
                await JS.InvokeVoidAsync("showAlert", "Update tittle fail.Try again.");
                return 3;
            }
        }

        public async Task<int> UpdateNoteTransactions()
        {
            try
            {
                if (String.IsNullOrEmpty(TransactionsId) || String.IsNullOrEmpty(TransactionsNote))
                {
                    await JS.InvokeVoidAsync("console.log", $"Update fail.Empty transactions Id or debt Id or note");
                    await JS.InvokeVoidAsync("showAlert", "Update fail.Empty transactions Id or debt Id or note");
                    return 0;
                }
                var transactions_data = requiredDetails.transactions_info_list.FirstOrDefault(x => x.UserUsername == requiredDetails.CurrentUserUsername && x.TrId == TransactionsId);
        

                if (transactions_data != null)
                {
                    transactions_data.TrNote = TransactionsNote;
                    await JS.InvokeVoidAsync("console.log", $"Update success");
                    await JS.InvokeVoidAsync("showAlert", "Update success");
                    return 1;

                }
                await JS.InvokeVoidAsync("console.log", $"Update fail");
                await JS.InvokeVoidAsync("showAlert", "Update fail");
                return 0;

            }
            catch (Exception obj)
            {
                await JS.InvokeVoidAsync("console.log", "Exception caught.");
                await JS.InvokeVoidAsync("console.log", $"{obj.ToString()}");
                await JS.InvokeVoidAsync("showAlert", "Update tittle fail.Try again.");
                return 3;
            }
        }

        public async Task<int> UpdateTittleDebt()
        {
            try
            {
                if (String.IsNullOrEmpty(DebtId) || String.IsNullOrEmpty(TransactionsTittle))
                {
                    await JS.InvokeVoidAsync("console.log", $"Update fail.Empty debt Id or tittle");
                    await JS.InvokeVoidAsync("showAlert", "Update fail.Empty debt Id or tittle");
                    return 0;
                }
                var debt_data = requiredDetails.debt_info_list.FirstOrDefault(x => x.Username == requiredDetails.CurrentUserUsername && x.DebtId == DebtId);
                if (debt_data != null)
                {
                    debt_data.DebtTitle = TransactionsTittle;
                    await JS.InvokeVoidAsync("console.log", $"Update success");
                    await JS.InvokeVoidAsync("showAlert", "Update success");
                    return 1;

                }
                await JS.InvokeVoidAsync("console.log", $"Update fail");
                await JS.InvokeVoidAsync("showAlert", "Update fail");
                return 0;

            }
            catch (Exception obj)
            {
                await JS.InvokeVoidAsync("console.log", "Exception caught.");
                await JS.InvokeVoidAsync("console.log", $"{obj.ToString()}");
                await JS.InvokeVoidAsync("showAlert", "Update tittle fail.Try again.");
                return 3;
            }
        }

        public async Task<int> UpdateNoteDebt()
        {
            try
            {
                if (String.IsNullOrEmpty(DebtId) || String.IsNullOrEmpty(TransactionsNote))
                {
                    await JS.InvokeVoidAsync("console.log", $"Update fail.Empty debt Id or note");
                    await JS.InvokeVoidAsync("showAlert", "Update fail.Empty debt Id or note");
                    return 0;
                }

                var debt_data = requiredDetails.debt_info_list.FirstOrDefault(x => x.Username == requiredDetails.CurrentUserUsername && x.DebtId == DebtId);
                if (debt_data != null)
                {

                    debt_data.DebtNote = TransactionsNote;
                    await JS.InvokeVoidAsync("console.log", $"Update success");
                    await JS.InvokeVoidAsync("showAlert", "Update success");
                    return 1;

                }
                await JS.InvokeVoidAsync("console.log", $"Update fail");
                await JS.InvokeVoidAsync("showAlert", "Update fail");
                return 0;

            }
            catch (Exception obj)
            {
                await JS.InvokeVoidAsync("console.log", "Exception caught.");
                await JS.InvokeVoidAsync("console.log", $"{obj.ToString()}");
                await JS.InvokeVoidAsync("showAlert", "Update tittle fail.Try again.");
                return 3;
            }
        }

        public bool UpdateDictionaryValue(
          List<Dictionary<string, string>> listOfDictionaries,
          string keyToFind,
          string valueToMatch,
          string keyToUpdate,
          string newValue)
        {
            try
            {
                foreach (var dictionary in listOfDictionaries)
                {
                    if (dictionary.ContainsKey(keyToFind) && dictionary[keyToFind] == valueToMatch)
                    {
                        if (dictionary.ContainsKey(keyToUpdate))
                        {
                            dictionary[keyToUpdate] = newValue;
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception obj)
            {
                return false;
            }
        }

        public async Task Logout()
        {
            try
            {

                bool update_last_currency_type_value = UpdateDictionaryValue(requiredDetails.FirstLoginCurrencyType, "user_username",
                requiredDetails.CurrentUserUsername, "firstlogin_currency_type", requiredDetails.CurrencyTypeUser);
                if (update_last_currency_type_value == true)
                {
                    requiredDetails.CurrencyTypeUser = "";
                    requiredDetails.CurrentUserUsername = "";
                    await JS.InvokeVoidAsync("console.log", "Logout success");
                    await JS.InvokeVoidAsync("showAlert", "Logout success.");
                    Navigation.NavigateTo("/");
                }
                else
                {
                    await JS.InvokeVoidAsync("console.log", "Update currency type value fail.");
                    await JS.InvokeVoidAsync("showAlert", "Logout failed.");
                }

            }
            catch (Exception)
            {
                await JS.InvokeVoidAsync("console.log", "Exception caught in logout method.");
                await JS.InvokeVoidAsync("showAlert", "Logout failed.");
            }
        }

        // Method to get the UserDebtBalance for a specific user by username
        public float GetUserDebtBalance()
        {
            try
            {
                var user = requiredDetails.user_info_list.FirstOrDefault(u => u.Username == requiredDetails.CurrentUserUsername);
                if (user != null)
                {
                    return user.UserDebtBalance;
                }
                else
                {
                    return 0.0f;
                }
            }
            catch (Exception ex)
            {
                return 0.0f;
            }
        }

        // Method to get the UserAvailableBalance for a specific user by username

        public string FormatBalance(float balance)
        {
            return balance.ToString("N2"); // Format with commas and 2 decimal places
        }

        public float GetUserAvailableBalance()
        {
            try
            {
                var user = requiredDetails.user_info_list.FirstOrDefault(u => u.Username == requiredDetails.CurrentUserUsername);
                if (user != null)
                {
                    return user.UserAvailableBalance;
                }
                else
                {
                    return 0.0f;
                }
            }
            catch (Exception ex)
            {
                return 0.0f;
            }
        }

        [JSInvokable]
        public async Task CancelTransaction()
        {
            await JS.InvokeVoidAsync("showAlert", "Transaction canceled.");
        }

        [JSInvokable]
        public async Task ProceedWithDebt()
        {
            try
            {
                var user_data = requiredDetails.user_info_list
                         .FirstOrDefault(user => user.Username == requiredDetails.CurrentUserUsername);

                if (user_data != null)
                {
                    float amount_insufficient = TransactionsAmount - user_data.UserAvailableBalance;

                    DebtModel debtModel = new DebtModel(
                             DebtId: Guid.NewGuid().ToString(),
                             TransactionType: "Debt",
                             DebtTitle: TransactionsTittle,
                             DebtDate: TransactionsDate,
                             DebtSource: TransactionsSource,
                             DebtAmount: amount_insufficient,
                             TransactionFlow: "In",
                             DebtStatus: "to pay",
                             TransactionStatus: TransactionsType,
                             Username: requiredDetails.CurrentUserUsername,
                             debtNote: TransactionsNote
                              );


                    TransactionModel transaction1 = new TransactionModel(
                       trId: Guid.NewGuid().ToString(),
                       trType: "Debt",
                       trTitle: TransactionsTittle,
                       trFlow: "In",
                       trDate: TransactionsDate,
                       trSource: TransactionsSource,
                       trNote: TransactionsNote,
                       trAmount: amount_insufficient,
                       userUsername: requiredDetails.CurrentUserUsername
                       );

                    user_data.UserAvailableBalance = user_data.UserAvailableBalance + amount_insufficient;  //sufficient amount


                    TransactionModel transaction2 = new TransactionModel(
                       trId: Guid.NewGuid().ToString(),
                       trType: TransactionsType,
                       trTitle: TransactionsTittle,
                       trFlow: TransactionsFlow,
                       trDate: TransactionsDate,
                       trSource: TransactionsSource,
                       trNote: TransactionsNote,
                       trAmount: user_data.UserAvailableBalance,
                       userUsername: requiredDetails.CurrentUserUsername
                       );

                    user_data.UserAvailableBalance = 0.0F;
                    user_data.UserDebtBalance = amount_insufficient;

                    requiredDetails.debt_info_list.Add(debtModel);
                    requiredDetails.transactions_info_list.Add(transaction1);
                    requiredDetails.transactions_info_list.Add(transaction2);
                    await JS.InvokeVoidAsync("console.log", "transaction ssuccess");
                    UpdateTransactionProperties();
                    UpdateDebtProperties();
                    StateHasChanged();
                    await JS.InvokeVoidAsync("showAlert", "Transactions success.");
                    return;
                }
                UpdateTransactionProperties();
                UpdateDebtProperties();
                StateHasChanged();
                return;
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("console.log", "exception caught in proceed with debt.");
                await JS.InvokeVoidAsync("showAlert", "Transactions cancel.");
            }
        }

        public async Task DoTransactions()
        {
            try
            {
                if
                (
                   string.IsNullOrEmpty(TransactionsFlow) ||
                   string.IsNullOrEmpty(TransactionsType) ||
                   // TransactionsDate == default(DateTime) ||
                   TransactionsAmount == 0.0f ||
                   string.IsNullOrEmpty(TransactionsSource) ||
                   string.IsNullOrEmpty(TransactionsNote) ||
                   string.IsNullOrEmpty(TransactionsTittle)
                // string.IsNullOrEmpty(TransactionsId)
                )
                {
                    await JS.InvokeVoidAsync("console.log", "Fail try again.Fill transactions details properly.");
                    await JS.InvokeVoidAsync("showAlert", "Fail try again.Fill transactions details properly.");
                    return;
                }

                await JS.InvokeVoidAsync("console.log", "transactions type value");
                await JS.InvokeVoidAsync("console.log", $"{TransactionsType}");

                if(TransactionsFlow=="In" && TransactionsType=="Debit")
                {
                    await JS.InvokeVoidAsync("console.log", "Trransactions type can not be debit when flow is In.");
                    await JS.InvokeVoidAsync("showAlert", "Trransactions type can not be debit when flow is In.");
                    return;

                }

                if (TransactionsFlow == "Out" && TransactionsType == "Credit")
                {
                    await JS.InvokeVoidAsync("console.log", "Trransactions type can not be Credit when flow is Out.");
                    await JS.InvokeVoidAsync("showAlert", "Trransactions type can not be Credit when flow is Out.");
                    return;

                }

                if (TransactionsType == "Credit")
                {

                    var debt_data = requiredDetails.debt_info_list.FirstOrDefault(x => x.DebtStatus == "to pay");
                    if (debt_data != null)
                    {
                        var debt_data_to_pay = requiredDetails.debt_info_list
                                     .Where(debt => debt.DebtStatus == "to pay")
                                     .ToList();
                        float initial_debt_amount_to_pay = 0.0f;
                        foreach (var debt in debt_data_to_pay)
                        {
                            initial_debt_amount_to_pay += (float)debt.DebtAmount;
                        }

                        if (TransactionsAmount == initial_debt_amount_to_pay) //the tr amount == debt amount to pay
                        {
                            DebtModel debtModel = new DebtModel(
                               DebtId: Guid.NewGuid().ToString(),
                               TransactionType: "Debt",
                               DebtTitle: TransactionsTittle,
                               DebtDate: TransactionsDate,
                               DebtSource: TransactionsSource,
                               DebtAmount: initial_debt_amount_to_pay,
                               TransactionFlow: "Out",
                               DebtStatus: "paid",
                               TransactionStatus: "Credit",
                               Username: requiredDetails.CurrentUserUsername,
                               debtNote: TransactionsNote
                                );

                            foreach (var debt in debt_data_to_pay)  //after clearing debt the to pay must change to paid
                            {
                                debt.DebtStatus = "paid";
                            }

                            TransactionModel transaction = new TransactionModel(
                      trId: Guid.NewGuid().ToString(),
                      trType: "Debt",
                      trTitle: TransactionsTittle,
                      trFlow: "Out",
                      trDate: TransactionsDate,
                      trSource: TransactionsSource,
                      trNote: TransactionsNote,
                      trAmount: TransactionsAmount,
                      userUsername: requiredDetails.CurrentUserUsername
                      );

                            requiredDetails.debt_info_list.Add(debtModel);
                            requiredDetails.transactions_info_list.Add(transaction);

                            var user_data = requiredDetails.user_info_list.FirstOrDefault(x => x.Username == requiredDetails.CurrentUserUsername);
                            user_data.UserDebtBalance = 0.0f;
                            user_data.UserAvailableBalance = 0.0f;

                            await JS.InvokeVoidAsync("console.log", "Transactions success");
                            await JS.InvokeVoidAsync("console.log", $"{debtModel.DebtId}");
                            UpdateTransactionProperties();
                            UpdateDebtProperties();
                            StateHasChanged();
                            await JS.InvokeVoidAsync("showAlert", "Transactions success");
                            return;
                        }

                        if (TransactionsAmount > initial_debt_amount_to_pay)  //tr mount is greater then debt amount to pay
                        {
                            float reamin_amount = TransactionsAmount - initial_debt_amount_to_pay;

                            DebtModel debtModel = new DebtModel(
                                      DebtId: Guid.NewGuid().ToString(),
                                      TransactionType: "Debt",
                                      DebtTitle: TransactionsTittle,
                                      DebtDate: TransactionsDate,
                                      DebtSource: TransactionsSource,
                                      DebtAmount: initial_debt_amount_to_pay,
                                      TransactionFlow: "Out",
                                      DebtStatus: "paid",
                                      TransactionStatus: "Credit",
                                      Username: requiredDetails.CurrentUserUsername,
                                      debtNote: TransactionsNote
                                         );

                            foreach (var debt in debt_data_to_pay)  //after clearing debt the to pay must change to paid
                            {
                                debt.DebtStatus = "paid";
                            }

                            TransactionModel transaction1 = new TransactionModel(
                               trId: Guid.NewGuid().ToString(),
                               trType: "Debt",
                               trTitle: TransactionsTittle,
                               trFlow: "Out",
                               trDate: TransactionsDate,
                               trSource: TransactionsSource,
                               trNote: TransactionsNote,
                               trAmount: initial_debt_amount_to_pay,
                               userUsername: requiredDetails.CurrentUserUsername
                               );

                            TransactionModel transaction2 = new TransactionModel(
                            trId: Guid.NewGuid().ToString(),
                            trType: TransactionsType,
                            trTitle: TransactionsTittle,
                            trFlow: "In",
                            trDate: TransactionsDate,
                            trSource: TransactionsSource,
                            trNote: TransactionsNote,
                            trAmount: reamin_amount,
                            userUsername: requiredDetails.CurrentUserUsername
                            );

                            requiredDetails.debt_info_list.Add(debtModel);
                            requiredDetails.transactions_info_list.Add(transaction1);
                            requiredDetails.transactions_info_list.Add(transaction2);

                            var user_data = requiredDetails.user_info_list.FirstOrDefault(x => x.Username == requiredDetails.CurrentUserUsername);
                            user_data.UserDebtBalance = 0.0f;
                            user_data.UserAvailableBalance = user_data.UserAvailableBalance + reamin_amount;

                            await JS.InvokeVoidAsync("console.log", "Transactions success");
                            await JS.InvokeVoidAsync("console.log", $"{debtModel.DebtId}");
                            UpdateTransactionProperties();
                            UpdateDebtProperties();
                            StateHasChanged();
                            await JS.InvokeVoidAsync("showAlert", "Transactions success");
                            return;




                        }

                        if (TransactionsAmount < initial_debt_amount_to_pay)
                        {
                            var user_data = requiredDetails.user_info_list.FirstOrDefault(x => x.Username == requiredDetails.CurrentUserUsername);
                            float total_available_amount_use_to_pay = user_data.UserAvailableBalance + TransactionsAmount;

                            if (initial_debt_amount_to_pay < total_available_amount_use_to_pay)
                            {

                                DebtModel debtModel = new DebtModel(
                                        DebtId: Guid.NewGuid().ToString(),
                                        TransactionType: "Debt",
                                        DebtTitle: TransactionsTittle,
                                        DebtDate: TransactionsDate,
                                        DebtSource: TransactionsSource,
                                        DebtAmount: initial_debt_amount_to_pay,
                                        TransactionFlow: "Out",
                                        DebtStatus: "paid",
                                        TransactionStatus: "Credit",
                                        Username: requiredDetails.CurrentUserUsername,
                                        debtNote: TransactionsNote
                                           );

                                foreach (var debt in debt_data_to_pay)  //after clearing debt the to pay must change to paid
                                {
                                    debt.DebtStatus = "paid";
                                }

                                TransactionModel transaction = new TransactionModel(
                                trId: Guid.NewGuid().ToString(),
                                trType: "Debt",
                                trTitle: TransactionsTittle,
                                trFlow: "Out",
                                trDate: TransactionsDate,
                                trSource: TransactionsSource,
                                trNote: TransactionsNote,
                                trAmount: initial_debt_amount_to_pay,
                                userUsername: requiredDetails.CurrentUserUsername
                                );

                                requiredDetails.transactions_info_list.Add(transaction);
                                requiredDetails.debt_info_list.Add(debtModel);

                                float new_current_user_balance = total_available_amount_use_to_pay - initial_debt_amount_to_pay;
                                var user_data_value = requiredDetails.user_info_list.FirstOrDefault(x => x.Username == requiredDetails.CurrentUserUsername);
                                user_data_value.UserAvailableBalance = new_current_user_balance;
                                user_data_value.UserDebtBalance = 0.0f;
                                await JS.InvokeVoidAsync("console.log", "Transactions success");
                                await JS.InvokeVoidAsync("console.log", $"{debtModel.DebtId}");
                                UpdateTransactionProperties();
                                UpdateDebtProperties();
                                StateHasChanged();
                                await JS.InvokeVoidAsync("showAlert", "Transactions success");
                                return;
                            }

                            if (initial_debt_amount_to_pay == total_available_amount_use_to_pay)
                            {
                                DebtModel debtModel = new DebtModel(
                                DebtId: Guid.NewGuid().ToString(),
                                TransactionType: "Debt",
                                DebtTitle: TransactionsTittle,
                                DebtDate: TransactionsDate,
                                DebtSource: TransactionsSource,
                                DebtAmount: initial_debt_amount_to_pay,
                                TransactionFlow: "Out",
                                DebtStatus: "paid",
                                TransactionStatus: "Credit",
                                Username: requiredDetails.CurrentUserUsername,
                                debtNote: TransactionsNote
                                 );

                                foreach (var debt in debt_data_to_pay)  //after clearing debt the to pay must change to paid
                                {
                                    debt.DebtStatus = "paid";
                                }

                                TransactionModel transaction = new TransactionModel(
                           trId: Guid.NewGuid().ToString(),
                           trType: "Debt",
                           trTitle: TransactionsTittle,
                           trFlow: "Out",
                           trDate: TransactionsDate,
                           trSource: TransactionsSource,
                           trNote: TransactionsNote,
                           trAmount: initial_debt_amount_to_pay,
                           userUsername: requiredDetails.CurrentUserUsername
                           );

                                requiredDetails.debt_info_list.Add(debtModel);
                                requiredDetails.transactions_info_list.Add(transaction);

                                var user_data_value = requiredDetails.user_info_list.FirstOrDefault(x => x.Username == requiredDetails.CurrentUserUsername);
                                user_data_value.UserDebtBalance = 0.0f;
                                user_data_value.UserAvailableBalance = 0.0f;

                                await JS.InvokeVoidAsync("console.log", "Transactions success");
                                await JS.InvokeVoidAsync("console.log", $"{debtModel.DebtId}");
                                UpdateTransactionProperties();
                                UpdateDebtProperties();
                                StateHasChanged();
                                await JS.InvokeVoidAsync("showAlert", "Transactions success");

                            }

                            if (initial_debt_amount_to_pay > total_available_amount_use_to_pay)
                            {
                                string guid = Guid.NewGuid().ToString();
                                TransactionModel transactionsModel = new TransactionModel
                                          (
                                        trId: guid,
                                        trType: TransactionsType,
                                        trTitle: TransactionsTittle,
                                        trFlow: TransactionsFlow,
                                        trDate: TransactionsDate,
                                        trSource: TransactionsSource,
                                        trNote: TransactionsNote,
                                        trAmount: TransactionsAmount,
                                        userUsername: requiredDetails.CurrentUserUsername
                                    );
                                requiredDetails.transactions_info_list.Add(transactionsModel);
                                float new_available_balance = user_data.UserAvailableBalance + TransactionsAmount;
                                user_data.UserAvailableBalance = new_available_balance;
                                await JS.InvokeVoidAsync("console.log", "Transactions success");
                                await JS.InvokeVoidAsync("console.log", $"{transactionsModel.TrId}");
                                UpdateTransactionProperties();
                                UpdateDebtProperties();
                                StateHasChanged();
                                await JS.InvokeVoidAsync("showAlert", "Transactions success");
                            }

                        }

                    } //to pay not present
                    else
                    {
                        string guid = Guid.NewGuid().ToString();
                        TransactionModel transactionsModel = new TransactionModel
                                  (
                                trId: guid,
                                trType: TransactionsType,
                                trTitle: TransactionsTittle,
                                trFlow: TransactionsFlow,
                                trDate: TransactionsDate,
                                trSource: TransactionsSource,
                                trNote: TransactionsNote,
                                trAmount: TransactionsAmount,
                                userUsername: requiredDetails.CurrentUserUsername
                            );
                        requiredDetails.transactions_info_list.Add(transactionsModel);
                        
                        var user_data = requiredDetails.user_info_list.FirstOrDefault(x => x.Username == requiredDetails.CurrentUserUsername);
                        if (user_data != null)
                        {
                            float new_available_balance = user_data.UserAvailableBalance + TransactionsAmount;
                            user_data.UserAvailableBalance = new_available_balance;
                            // Additional logic...
                        }
                        else
                        {
                            await JS.InvokeVoidAsync("console.log", "User data not found.");
                            await JS.InvokeVoidAsync("showAlert", "User data not found.");
                        }
                        await JS.InvokeVoidAsync("console.log", "Transactions success");
                        await JS.InvokeVoidAsync("console.log", $"{transactionsModel.TrId}");
                        UpdateTransactionProperties();
                        UpdateDebtProperties();
                        StateHasChanged();
                        await JS.InvokeVoidAsync("showAlert", "Transactions success");

                    }

                } //transaction type credit

                if (TransactionsType == "Debit")
                {
                    var user_data = requiredDetails.user_info_list.FirstOrDefault(x => x.Username == requiredDetails.CurrentUserUsername);

                    if (TransactionsAmount < user_data.UserAvailableBalance)
                    {
                        string guid = Guid.NewGuid().ToString();
                        TransactionModel transactionsModel = new TransactionModel
                                  (
                                trId: guid,
                                trType: TransactionsType,
                                trTitle: TransactionsTittle,
                                trFlow: TransactionsFlow,
                                trDate: TransactionsDate,
                                trSource: TransactionsSource,
                                trNote: TransactionsNote,
                                trAmount: TransactionsAmount,
                                userUsername: requiredDetails.CurrentUserUsername
                            );
                        requiredDetails.transactions_info_list.Add(transactionsModel);
                        var user_data_value = requiredDetails.user_info_list.FirstOrDefault(x => x.Username == requiredDetails.CurrentUserUsername);
                        float new_available_balance = user_data_value.UserAvailableBalance - TransactionsAmount;
                        user_data_value.UserAvailableBalance = new_available_balance;
                        await JS.InvokeVoidAsync("console.log", "Transactions success");
                        await JS.InvokeVoidAsync("console.log", $"{transactionsModel.TrId}");
                        UpdateTransactionProperties();
                        UpdateDebtProperties();
                        StateHasChanged();
                        await JS.InvokeVoidAsync("showAlert", "Transactions success");
                        return;
                    }
                    if (TransactionsAmount > user_data.UserAvailableBalance)
                    {
                        try
                        {
                            float amount_insufficient = TransactionsAmount - user_data.UserAvailableBalance;
                            // Show an alert with options
                            await JS.InvokeVoidAsync("showConfirm",
                                $"Insufficient funds! Shortage: {amount_insufficient}. Do you want to proceed with debt?",
                                DotNetObjectReference.Create(this)); 
                        }
                        catch (Exception obj)
                        {
                            await JS.InvokeVoidAsync("console.log", "exception caught.");
                            await JS.InvokeVoidAsync("console.log", "Trnsactions fail.");
                            await JS.InvokeVoidAsync("showAlert", "Transactions fail");
                            return;
                        }


                    }

                }
                StateHasChanged();
            }
            catch (Exception obj)
            {
                await JS.InvokeVoidAsync("console.log", "exception caught");
                await JS.InvokeVoidAsync("console.log", $"{obj.ToString()}");
                await JS.InvokeVoidAsync("showAlert", "Transactions fail");

            }

        }

        public void UpdateTransactionProperties()
        {
            try
            {
                // Ensure the transactions list is not null
                var userTransactions = requiredDetails.transactions_info_list?
                    .Where(t => t.UserUsername == requiredDetails.CurrentUserUsername);

                if (userTransactions == null || !userTransactions.Any())
                {
                    // If null or empty, reset all properties to default values
                    HighestInflowTransaction = 0.0f;
                    LowestInflowTransaction = 0.0f;
                    HighestOutflowTransaction = 0.0f;
                    LowestOutflowTransaction = 0.0f;
                    TotalInflow = 0.0f;
                    TotalOutflow = 0.0f;
                    StateHasChanged();
                    return;
                }

                // Calculate highest inflow
                HighestInflowTransaction = userTransactions
                    .Where(t => t.TrFlow.ToLower() == "in")
                    .OrderByDescending(t => t.TrAmount)
                    .FirstOrDefault()?.TrAmount ?? 0.0f;

                // Calculate lowest inflow
                LowestInflowTransaction = userTransactions
                    .Where(t => t.TrFlow.ToLower() == "in")
                    .OrderBy(t => t.TrAmount)
                    .FirstOrDefault()?.TrAmount ?? 0.0f;

                // Calculate highest outflow
                HighestOutflowTransaction = userTransactions
                    .Where(t => t.TrFlow.ToLower() == "out")
                    .OrderByDescending(t => t.TrAmount)
                    .FirstOrDefault()?.TrAmount ?? 0.0f;

                // Calculate lowest outflow
                LowestOutflowTransaction = userTransactions
                    .Where(t => t.TrFlow.ToLower() == "out")
                    .OrderBy(t => t.TrAmount)
                    .FirstOrDefault()?.TrAmount ?? 0.0f;

                // Calculate total inflow
                TotalInflow = userTransactions
                    .Where(t => t.TrFlow.ToLower() == "in")
                    .Sum(t => t.TrAmount);

                // Calculate total outflow
                TotalOutflow = userTransactions
                    .Where(t => t.TrFlow.ToLower() == "out")
                    .Sum(t => t.TrAmount);
                StateHasChanged();
                return;
            }
            catch (Exception ex)
            {
                // Log exception if needed (e.g., Console.WriteLine or logging framework)
                // Reset to default values in case of error
                HighestInflowTransaction = 0.0f;
                LowestInflowTransaction = 0.0f;
                HighestOutflowTransaction = 0.0f;
                LowestOutflowTransaction = 0.0f;
                TotalInflow = 0.0f;
                TotalOutflow = 0.0f;
                StateHasChanged();
                return;
            }
        }

        private void UpdateDebtProperties()
        {
            try
            {
                // Ensure the debts list is not null
                var userDebts = requiredDetails.debt_info_list?
                    .Where(d => d.Username == requiredDetails.CurrentUserUsername);

                if (userDebts == null || !userDebts.Any())
                {
                    // If null or empty, reset all properties to default values
                    HighestDebt = 0.0f;
                    LowestDebt = 0.0f;
                    StateHasChanged();
                    return;
                }

                // Calculate highest debt
                HighestDebt = userDebts
                    .OrderByDescending(d => d.DebtAmount)
                    .FirstOrDefault()?.DebtAmount ?? 0.0f;

                // Calculate lowest debt
                LowestDebt = userDebts
                    .OrderBy(d => d.DebtAmount)
                    .FirstOrDefault()?.DebtAmount ?? 0.0f;
                StateHasChanged();
                return;
            }
            catch (Exception ex)
            {
                // Log exception if needed (e.g., Console.WriteLine or logging framework)
                // Reset to default values in case of error
                HighestDebt = 0.0f;
                LowestDebt = 0.0f;
                StateHasChanged();
                return;
            }
        }



  

}
}
