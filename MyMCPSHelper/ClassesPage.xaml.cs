﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MyMCPSHelper {
    public partial class ClassesPage : ContentPage {
        public ClassesPage() {
            Title = "Classes";
            InitializeComponent();

            Task.Run(() => {
				var classes = App.AccMangr.loadClasses();
				List<Class> classCells = new List<Class>();
				foreach (Class clas in classes)
				{
                    if (clas != null && clas.period != null && clas.period.Length >= 2 && Int32.Parse(clas.period.Substring(0, 2)) <= 8 && clas.courseName.ToLower() != "lunch")
					{
						classCells.Add(clas);
					}
				}

                Device.BeginInvokeOnMainThread(() => {
                   ClassesList.ItemsSource = classCells;
				});
            });
        }

        void Handle_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e) {
            if (((Class)e.Item).overallgrade != "") {
                Navigation.PushAsync(new AssignmentInfo(((Class)e.Item).sectionid, ((Class)e.Item).courseName));
            }
        }

        void Handle_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            Label label = (Label)sender;
            switch(label.Text){
                case "A":
                    label.TextColor = Color.Green;
                    break;
				case "B":
                    label.TextColor = Color.Blue;
					break;
				case "C":
                    label.TextColor = Color.Orange;
					break;
				case "D":
                    label.TextColor = Color.DarkOrange;
					break;
				case "E":
                    label.TextColor = Color.Red;
					break;
				case "F":
                    label.TextColor = Color.Red;
					break;
            }
        }
    }
}
