using System;
using System.ComponentModel;
namespace MyMCPSHelper {
    public class GradingCategory: INotifyPropertyChanged {
        public String Description { get; set; }
        public String weight;
        public String Weight { get{
                return ((int)(float.Parse(weight))).ToString();
            } 
            set{
                weight = value;
            } 
        }
        public String pe;
        public String PointsEarned { get{
                return pe;
            } set{
                pe = value;
                OnPropertyChanged("PointsEarned");
                OnPropertyChanged("Formated");
            }
        }
        public String pp;
        public String PointsPossible { get{
                return pp;
            } set{
                pp = value;
				OnPropertyChanged("PointsPossible");
				OnPropertyChanged("Formated");
            } 
        }
        public String Percent { get; set; }
        public String Formated {
            get{
                try{
                    if (PointsPossible == "0"){
                        return "0/0 (100%)";
                    }
                    if (Description != "" ){
                        return ((int)float.Parse(PointsEarned)).ToString() + "/" + ((int)float.Parse(PointsPossible)).ToString() + " (" + Percent + "%)";
                    }
                    return ((float.Parse(PointsEarned) / float.Parse(PointsPossible)) * 100).ToString("G3") + "%";
                } catch{
                    return "0";
                }
            }
        }

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			var changed = PropertyChanged;
			if (changed != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
    }
}
