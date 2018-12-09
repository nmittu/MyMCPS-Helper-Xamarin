using System;

namespace MyMCPSHelper {
    public class Class {
        public string courseName { get; set; }
        public string overallgrade { get; set; }
        public string period { get; set; }
        public string sectionid { get; set; }
        public string termid { get; set; }
        public string percent { get; set; }
        public string teacher { get; set; }
        public string room { get; set; }
        public string formattedGrade {
            get
            {
                if (percent != null)
                {
                    return   percent + "% " + overallgrade;
                }
                return overallgrade;
            }
        }
    }
}
