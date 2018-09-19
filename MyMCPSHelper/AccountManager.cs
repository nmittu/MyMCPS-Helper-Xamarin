using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using PCLCrypto;
using HtmlAgilityPack;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.ObjectModel;
using System.Collections;
using Newtonsoft.Json;
using Xamarin.Auth;
using System.Linq;
using Plugin.Connectivity;

public class MyHttpWebRequest {
	HttpWebRequest request;

	public bool AllowAutoRedirect {
		get {
			Type t = request.GetType();
			PropertyInfo pi = t.GetRuntimeProperty("AllowAutoRedirect");
			return (bool)pi.GetValue(request);
		}
		set {
			Type t = request.GetType();
			PropertyInfo pi = t.GetRuntimeProperty("AllowAutoRedirect");
            pi.SetValue(request, value);
		}
	}

    public MyHttpWebRequest(HttpWebRequest request){
        this.request = request;
    }
}


namespace MyMCPSHelper {
    public class AccountManager {
        public const String NewSessURL = "https://portal.mcpsmd.org/public/home.html";
        public const String LoginURL = "https://portal.mcpsmd.org/guardian/home.html";
        public const String ClassesBaseURL = "https://portal.mcpsmd.org/guardian/prefs/gradeByCourseSecondary.json";
        public const String TermURL = "https://portal.mcpsmd.org/guardian/prefs/termsData.json";
        public const String CategoryURL = "https://portal.mcpsmd.org/guardian/prefs/assignmentGrade_CategoryDetail.json";
        public const String AssignmentInfoURL = "https://portal.mcpsmd.org/guardian/prefs/assignmentGrade_AssignmentDetail.json";
        private HttpClient client;
        private String StudentNumber = "";
        private String StudentID = "";
        private HttpClientHandler handler;
        private CookieContainer cookies = new CookieContainer();
        private string password;
        private String termname = null;
        private Dictionary<string, string> accounts = new Dictionary<string, string>();  // Name, SN

		public AccountManager() {
			client = null;
            handler = new HttpClientHandler();
            handler.CookieContainer = cookies;
		}

        private static readonly TaskFactory _myTaskFactory = new
            TaskFactory(CancellationToken.None,
            		  TaskCreationOptions.None,
            		  TaskContinuationOptions.None,
            		  TaskScheduler.Default);

		public static TResult RunSync<TResult>(Func<Task<TResult>> func) {
			return _myTaskFactory
			  .StartNew<Task<TResult>>(func)
			  .Unwrap<TResult>()
			  .GetAwaiter()
			  .GetResult();
		}

		public static void RunSync(Func<Task> func) {
			_myTaskFactory
			  .StartNew<Task>(func)
			  .Unwrap()
			  .GetAwaiter()
			  .GetResult();
		}

        private static String Base64Encode(string plainText) {
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
			return System.Convert.ToBase64String(plainTextBytes);
		}

        private static string ToHexString(byte[] array) {
			StringBuilder hex = new StringBuilder(array.Length * 2);
			foreach (byte b in array) {
				hex.AppendFormat("{0:x2}", b);
			}
			return hex.ToString();
		}

        public string Login(String StudentId, String Password){
            if (!CrossConnectivity.Current.IsConnected || !CrossConnectivity.Current.IsRemoteReachable(NewSessURL).Result){
                return "Cannot connect to internet!";
            }

            this.password = Password;
            this.StudentID = StudentId;
            client = new HttpClient(handler);
            Dictionary<string, string> form = new Dictionary<string, string>();

            String psval = "";

            {
                HtmlDocument doc = new HtmlDocument();
                doc.Load(client.GetStreamAsync(NewSessURL).Result);

                IEnumerable<HtmlNode> inputs = doc.GetElementbyId("LoginForm").Descendants();

                foreach (HtmlNode input in inputs) {
                    if (input.GetAttributeValue("type", "") == "hidden") {
                        form.Add(input.GetAttributeValue("name", ""), input.GetAttributeValue("value", ""));
                        if (input.GetAttributeValue("name", "") == "contextData") {
                            psval = input.GetAttributeValue("value", "");
                        }
                    }
                }
            }


			form["account"] = StudentId;
			form["ldappassword"] = Password;

            var MD5alg = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Md5);
            String b64pw = Convert.ToBase64String(MD5alg.HashData(Encoding.UTF8.GetBytes(Password))).Replace('=', ' ').Trim();
			
            var alg = WinRTCrypto.MacAlgorithmProvider.OpenAlgorithm(MacAlgorithm.HmacMd5);
            CryptographicHash hasher = alg.CreateHash(Encoding.UTF8.GetBytes(psval));
			hasher.Append(Encoding.UTF8.GetBytes(b64pw));
            form["pw"] = ToHexString(hasher.GetValueAndReset());

			hasher.Append(Encoding.UTF8.GetBytes(Password.ToLower()));
			form["dbpw"] = ToHexString(hasher.GetValueAndReset());

            var content = new FormUrlEncodedContent(form);
            var response = client.PostAsync(LoginURL, content).Result;

            if (int.TryParse(StudentID, out int n)){
                HtmlDocument doc = new HtmlDocument();
                var res = response.Content.ReadAsStringAsync().Result;

                string pattern = "\\s?root.studentId\\s?=\\s?parseInt\\(('|\")(\\d+)('|\")\\);";

                MatchCollection matches = Regex.Matches(res, pattern);

                if(matches.Count > 0){
                    StudentNumber = matches[0].Groups[2].ToString();
                    return "true";
                }
            }else{
                HtmlDocument doc = new HtmlDocument();
                var res = response.Content.ReadAsStringAsync().Result;

                string pattern = "\\s?<a href=(\"|')javascript:switchStudent\\((\\d+)\\);(\"|')>(.+)<\\/a>";

                MatchCollection matches = Regex.Matches(res, pattern);

                if(matches.Count > 0){
                    for (int i = 0; i < matches.Count; i++)
                    {
                        accounts.Add(matches[i].Groups[4].ToString(), matches[i].Groups[2].ToString());
                    }
                    return "Multiple Accounts";
                }
            }

            return "Login failed!";
        }

        public string Login(){
            return Login(StudentID, password);
        }

        public List<Class> loadClasses(){
            String url = ClassesBaseURL + "?schoolid=" + cookies.GetCookies(new Uri(LoginURL))["currentSchool"].Value + "&student_number=" + StudentID + "&studentId=" + StudentNumber;
            String jsons = client.GetStringAsync(url).Result;
            return JsonConvert.DeserializeObject<List<Class>>(jsons);
        }

        public String loadTerm(){
            if (this.termname != null){
                return this.termname;
            }

            String url = TermURL + "?schoolid=" + cookies.GetCookies(new Uri(LoginURL))["currentSchool"].Value;
            String jsons = client.GetStringAsync(url).Result;
            List <Term> terms = JsonConvert.DeserializeObject<List<Term>>(jsons);

			int max = 0;
			string termname = "";
            foreach (Term term in terms){
                if (term.termname != null && Int32.Parse(term.termname.Substring(term.termname.Length -1, 1)) > max){
                    max = Int32.Parse(term.termname.Substring(term.termname.Length - 1, 1));
                    termname = term.termname;
                }
            }
            this.termname = termname;
            return termname;
        }

		public List<GradingCategory> loadCategories(string secid) {
            String url = CategoryURL + "?secid=" + secid + "&student_number=" + StudentID + "&schoolid="+cookies.GetCookies(new Uri(LoginURL))["currentSchool"].Value + "&termid=" + loadTerm();
            String jsons = client.GetStringAsync(url).Result;
            return JsonConvert.DeserializeObject<List<GradingCategory>>(jsons);
		}

        public ObservableCollection<GradeInfo> loadAssignments(string secid) {
            String url = AssignmentInfoURL + "?secid=" + secid + "&student_number=" + StudentID + "&schoolid=" + cookies.GetCookies(new Uri(LoginURL))["currentSchool"].Value + "&termid=" + loadTerm();
            String jsons = client.GetStringAsync(url).Result;
            return JsonConvert.DeserializeObject<ObservableCollection<GradeInfo>>(jsons);
        }

        public void logout(){
            accounts = new Dictionary<string, string>();
            try{
                string resp = client.GetStringAsync(LoginURL + "?ac=logoff").Result;
            }catch{}
        }

        public void saveAccount(String StudentID, String Password){
            {
                var account = AccountStore.Create().FindAccountsForService(App.Name).FirstOrDefault();
				if (account != null)
				{
                    AccountStore.Create().Delete(account, App.Name);
				}
            }

            if (!string.IsNullOrWhiteSpace(StudentID) && !string.IsNullOrWhiteSpace(Password)){
				Account account = new Account{
                    Username = StudentID
				};
                account.Properties.Add("Password", Password);
                AccountStore.Create().Save(account, App.Name);
            }
        }

        public void deleteAccount()
        {
            var account = AccountStore.Create().FindAccountsForService(App.Name).FirstOrDefault();
            if (account != null)
            {
                AccountStore.Create().Delete(account, App.Name);
            }
        }

        public Tuple<String, String> getAccount(){
            var account = AccountStore.Create().FindAccountsForService(App.Name).FirstOrDefault();
            return (account != null) ? new Tuple<string, string>(account.Username, account.Properties["Password"]) : null;
        }

        public List<string> getStudentNames(){
            return accounts.Keys.ToList();
        }

        public void setActiveAccount(string name){
            Dictionary<string, string> form = new Dictionary<string, string>();
            form.Add("selected_student_id", accounts[name]);
            var content = new FormUrlEncodedContent(form);
            var response = client.PostAsync(LoginURL, content).Result;

            {
                HtmlDocument doc = new HtmlDocument();
                var res = response.Content.ReadAsStringAsync().Result;

                string pattern = "\\s?root.studentNumber\\s?=\\s?parseInt\\(('|\")(\\d+)('|\")\\);";

                MatchCollection matches = Regex.Matches(res, pattern);

                if (matches.Count > 0)
                {
                    StudentID = matches[0].Groups[2].ToString();
                }
            }
            StudentNumber = accounts[name];
        }
    }

    class Term{
        public String termname { get; set; }
    }

}
