﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MyMCPSHelper {
    public partial class AssignmentInfo : ContentPage, INotifyPropertyChanged {
        List<String> category_strs;
        List<GradingCategory> categories;
        ObservableCollection<Grade> grades;

        public AssignmentInfo(String SectionID, String className) {
            InitializeComponent();

            category_strs = new List<string>();

            Title = className;

            Task.Run(() => {
				categories = App.AccMangr.loadCategories(SectionID);
				grades = App.AccMangr.loadAssignments(SectionID);

				List<GradingCategory> tempC = new List<GradingCategory>();
				foreach (GradingCategory cat in categories)
				{
					if (cat != null && cat.Description != null)
					{
						tempC.Add(cat);
						category_strs.Add(cat.Description);
					}
				}
				categories = tempC;

				ObservableCollection<Grade> tempG = new ObservableCollection<Grade>();
				foreach (Grade g in grades)
				{
					if (g != null && g.Description != null)
					{
						tempG.Add(g);
						g.category_strs = category_strs;
						foreach (String category in category_strs)
						{
							if (g.AssignmentType.StartsWith(category))
							{
								g.AssignmentType = category;
							}
						}
					}
				}
				grades = tempG;

                Device.BeginInvokeOnMainThread(() => {
					//categories.Add(new GradingCategory{ PointsPossible="100", PointsEarned="", Description="", Percent="", Weight=""});
					OverviewList.ItemsSource = categories;
					AssignmentList.ItemsSource = grades;
				});
            });
        }

        public AssignmentInfo(){
            InitializeComponent();

            OverviewList.ItemsSource = new List<GradingCategory> {new GradingCategory {Description="All Tasks/Assignments", Percent="100.00", PointsEarned="150", PointsPossible="150", Weight="100"}};
            AssignmentList.ItemsSource = new List<Grade> { new Grade { Description = "Scarlet Letter Assessment 1 This is very long ...", AssignmentType = "All Tasks/Assignments", Points = "15", Possible = "15" } };
        }

        public Dictionary<String, Tuple<Tuple<String, String>, String>> CalculateGrade(){
            Dictionary<String, List<float>> totals = new Dictionary<String, List<float>>();

            foreach (String category in category_strs){
                totals[category] = new List<float> {0, 0};
            }

            foreach (Grade g in grades){
                List<float> pair = totals[g.AssignmentType];
                try {
                    pair[0] += float.Parse(g.Points);
                } catch{
                    continue;
                }
                try
                {
                    pair[1] += float.Parse(g.Possible);
                }catch{
                    pair[0] -= float.Parse(g.Points);
                    continue;
                }
            }

            float total = 0;
            Dictionary<String, Tuple<Tuple<String, String>, String>> ret = new Dictionary<string, Tuple<Tuple<string, string>, string>>();
            foreach (String category in category_strs){
                ret[category] = new Tuple<Tuple<string, string>, string>(new Tuple<string, string>(totals[category][0].ToString(), totals[category][1].ToString()), (100 * totals[category][0] / totals[category][1]).ToString("G3"));
                float weight = 0;
                float totalWeights = 0;
                foreach (GradingCategory gc in categories){
                    if (gc.Description != ""){
                        if (gc.Description == category){
                            weight = float.Parse(gc.Weight);
                        }
                        if (totals[gc.Description][1] != 0){
                            totalWeights += float.Parse(gc.Weight);
                        }
                    }
                }
                if (totals[category][1] != 0){
                    total += (totals[category][0] / totals[category][1]) * (weight/totalWeights);
                }
            }
            ret["total"] = new Tuple<Tuple<string, string>, string>(null, (total*100).ToString("G3"));
            return ret;
        }

        public void repopulate(Dictionary<String, Tuple<Tuple<String, String>, String>> grades){
            foreach (GradingCategory cat in categories){
                if (cat.Description != "")
                {
                    cat.PointsEarned = grades[cat.Description].Item1.Item1;
                    cat.PointsPossible = grades[cat.Description].Item1.Item2;
                    cat.Percent = grades[cat.Description].Item2;
                }
            }
            totalGLabel.Text = grades["total"].Item2 + "%";
        }

        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e) {
            repopulate(CalculateGrade());
        }

        void Handle_Clicked(object sender, System.EventArgs e){
            grades.Add(new Grade { Description="New Assignment", AssignmentType=category_strs[0], category_strs=category_strs, Points="", Possible="10"});
        }

        void Handle_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e){
            repopulate(CalculateGrade());
        }

        void Handle_Recolor(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                Label label = (Label)sender;
                String text = label.Text;
                if (text.Substring(text.Length - 1, 1) == "%")
                {
                    text = text.Substring(0, text.Length - 1);
                }

                if (float.Parse(text) >= 89.5)
                {
                    label.TextColor = Color.Green;
                }
                else if (float.Parse(text) >= 79.5)
                {
                    label.TextColor = Color.Blue;
                }
                else if (float.Parse(text) >= 69.5)
                {
                    label.TextColor = Color.Orange;
                }
                else if (float.Parse(text) >= 59.5)
                {
                    label.TextColor = Color.DarkOrange;
                }
                else
                {
                    label.TextColor = Color.Red;
                }
            }catch{}
        }

		public void OnDelete(object sender, EventArgs e){
			var mi = ((MenuItem)sender);
            grades.Remove((Grade)mi.CommandParameter);
            repopulate(CalculateGrade());
		}
    }
}