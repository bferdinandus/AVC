using System.Collections.ObjectModel;
using AVC.Core.Models;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace AVC.Core.ViewModels
{
    public class GuestBookViewModel : MvxViewModel
    {
        private ObservableCollection<PersonModel> _people = new();
        private string _firstName;
        private string _lastName;

        public IMvxCommand AddGuestCommand { get; set; }

        public GuestBookViewModel()
        {
            AddGuestCommand = new MvxCommand(AddGuest);
        }

        public ObservableCollection<PersonModel> People
        {
            get => _people;
            set => SetProperty(ref _people, value);
        }

        public string FirstName
        {
            get => _firstName;
            set
            {
                SetProperty(ref _firstName, value);
                RaisePropertyChanged(() => FullName);
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                SetProperty(ref _lastName, value);
                RaisePropertyChanged(() => FullName);
            }
        }

        public string FullName => $"{FirstName} {LastName}";

        public void AddGuest()
        {
            PersonModel person = new()
            {
                FirstName = FirstName,
                LastName = LastName
            };

            FirstName = string.Empty;
            LastName = string.Empty;

            People.Add(person);
        }
    }
}