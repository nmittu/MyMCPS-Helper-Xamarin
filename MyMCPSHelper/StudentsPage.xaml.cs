using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MyMCPSHelper
{
    public partial class StudentsPage : ContentPage
    {
        public StudentsPage()
        {
            Title = "Students";
            InitializeComponent();

            Task.Run(() =>
            {
                List<string> students = App.AccMangr.getStudentNames();
                List<Student> studentsC = new List<Student>();
                foreach (string student in students){
                    studentsC.Add(new Student(student));
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    spinner.IsRunning = false;
                    spinner.IsVisible = false;
                    StudentsList.ItemsSource = studentsC;
                });
            });
        }

        void Handle_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            Task.Run(() =>
            {
                App.AccMangr.setActiveAccount(((Student)e.Item).name);
                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PushAsync(new ClassesPage());
                });
            });
        }
    }

    class Student{
        public string name { get; set; }
        public Student(string name)
        {
            this.name = name;
        }
    }
}
