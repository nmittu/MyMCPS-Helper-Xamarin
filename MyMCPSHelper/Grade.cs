using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MyMCPSHelper {
    public class Grade: INotifyPropertyChanged {
        public String AssignmentType { get; set; }
        public String Description { get; set; }
        public String po;
        public String Points { get{
                return po;
            } set{
                po = value;
                OnPropertyChanged("Points");
                OnPropertyChanged("Percent");
            } 
        }
        public String pos;
        public String Possible { get{
                return pos;
            } set{
                pos = value;
                OnPropertyChanged("PossibleFormatted");
                OnPropertyChanged("Percent");
            } 
        }
        public List<String> category_strs { get; set; }
        public String Percent{  get{
                try{
                    return ((float.Parse(Points) / float.Parse(Possible)) * 100).ToString("G3");
                }catch{
                    return "100";
                }
            }set{}
        }
        public String PossibleFormatted {
            get{
                return Possible;
            }
			set
			{
				Possible = value;
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
