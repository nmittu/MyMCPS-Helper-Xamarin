using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MyMCPSHelper {
    public class GradeInfo: INotifyPropertyChanged {
        public String AssignmentType { get; set; }
        public String Description { get; set; }
        public String OrigGrade = null;
        public String grade;
        public String Grade { get{
                return grade;
            } set{
                grade = value;
                if(OrigGrade == null){
                    OrigGrade = value;
                }
            } 
        }
        public String po = null;
        public String Points { get{
                if (Grade != null && Grade.ToLower() == "x" || Grade.ToLower() == "z"){
                    return Grade;
                }
                return po;
            } set{
                if (value.ToLower() == "x" || value.ToLower() == "z"){
                    Grade = value;
                }else if(po != null) {
                    Grade = "";
                }
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
				if (Grade != null && Grade.ToLower() == "x")
				{
					return Grade;
				}else if (Grade != null && Grade.ToLower() == "z")
				{
					return "0";
				}
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
