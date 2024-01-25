using MySql.Data.MySqlClient;
using StudiaZadanko.Interfaces;
using StudiaZadanko.Models;
using StudiaZadanko.Repositories;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace StudiaZadanko
{
    public partial class MainWindow : Window
    {
        IPersonRepository personRepository;

        public string? tempName;
        public string? tempSurname;
        public string? tempPhoneNumber;
        public string? tempAddress;
        public string? tempEmail;
        public MainWindow()
        {
            InitializeComponent();
            personRepository = new PersonRepository();
            SetUpData();
        }

        private async void SetUpData()
        {
            var data = await GetAllAsync();
            PersonList.ItemsSource = data;
        }


        private async Task<IEnumerable<Person>> GetAllAsync()
        {
            try
            {
                return await Task.Run(personRepository.GetAll);
            }
            catch (MySqlException err)
            {
                MessageBox.Show(err.Message);
                throw;
            }
        }

        private async Task DeleteById(int id)
        {
            try
            {
                await Task.Run(() => personRepository.Delete(id))
                    .ContinueWith(task => {
                        Dispatcher.Invoke(() =>
                        {
                            SetUpData();
                        });
                        
                    });

            }
            catch (MySqlException err)
            {
                MessageBox.Show(err.Message);
                throw;
            }
        }

        private async Task AddNew(Person person)
        {
            try
            {
                await Task.Run(() => personRepository.Add(person))
                    .ContinueWith(task => {
                        Dispatcher.Invoke(() =>
                        {
                            SetUpData();
                        });

                    });

            }
            catch (MySqlException err)
            {
                MessageBox.Show(err.Message);
                throw;
            }
        }


        //BUTTONS' TRIGGERS SECTION

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (PersonList.SelectedItem != null)
            {
                Person selectedPerson = (Person)PersonList.SelectedItem;
                Task.Run(() => DeleteById(selectedPerson.ID));

            }
        }

        private void AddMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tempSurname) && !string.IsNullOrEmpty(tempPhoneNumber) &&
        !string.IsNullOrEmpty(tempAddress) && !string.IsNullOrEmpty(tempEmail))
            {
              
                Person newPerson = new Person
                {
                    name = tempName,
                    surname = tempSurname,
                    phone_number = tempPhoneNumber,
                    address = tempAddress,
                    email = tempEmail
                };
                Task.Run(() => AddNew(newPerson));
                tempName = tempSurname = tempPhoneNumber = tempAddress = tempEmail = null;
            }
            else
            {
                MessageBox.Show("Dodaj brakujące dane (Nazwisko, Numer telefonu, Adres, Email)", "Brakujące dane", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void PersonList_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            string? editedColumnHeader = e.Column.Header.ToString();
            string? editedValue = (e.EditingElement as TextBox)?.Text;

            if(editedColumnHeader != null && editedValue != null) 
            {
                Debug.WriteLine(editedColumnHeader);
                switch (editedColumnHeader.ToLower())
                {
                    case "imię":
                        tempName = editedValue;
                        break;
                    case "nazwisko":
                        tempSurname = editedValue;
                        break;
                    case "e-mail":
                        tempEmail = editedValue;
                        break;
                    case "numer telefonu":
                        tempPhoneNumber = editedValue;
                        break;
                    case "adres":
                        tempAddress = editedValue;
                        break;
                    default:
                     
                        break;

                }
            }
            Debug.WriteLine(tempName);
            Debug.WriteLine(tempSurname);
            Debug.WriteLine(tempEmail);
            Debug.WriteLine(tempPhoneNumber);
            Debug.WriteLine(tempAddress);
        }
    }
}
