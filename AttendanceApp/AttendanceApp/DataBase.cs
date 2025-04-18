using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace AttendanceApp
{
    internal static class DataBase
    {
        static readonly HttpClient client = new HttpClient();

        static JsonSerializerOptions opt = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true};

        const string API_URL = "http://localhost:5001/";
        //const string API_URL = "http://10.0.33.17:5001/";

        public static Session CurrentSession;

        private static async Task<T> Get<T>(string request, Guid guid, Dictionary<string, string> parms, bool withoutGuid = false)
        {
            string parmsStr = withoutGuid ? "?" : $"?guid={guid.ToString()}";
            foreach (KeyValuePair<string, string> pair in parms)
            {
                if (parmsStr.Length != 1)
                {
                    parmsStr += "&";
                }
                parmsStr += $"{pair.Key}={pair.Value}";
            }

            try
            {
                string url = $"{API_URL}{request}{parmsStr}";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStreamAsync();
                var responseData = JsonSerializer.DeserializeAsync<T>(responseBody, opt).Result;
                response.Dispose();
                return responseData;
            }
            catch (HttpRequestException e)
            {
                OnError(e);
            }

            return default;
        }

        private static void OnError(HttpRequestException e)
        {
            string message = e.HttpRequestError switch
            {
                HttpRequestError.ConnectionError => "Не удалось подключиться с серверу",
                HttpRequestError.SecureConnectionError => "Не удалось установить безопасное подключение",
                HttpRequestError.InvalidResponse => "Получен некорректный ответ",
                _ => e.Message,
            };

            int code = Convert.ToInt32(e.StatusCode);

            if (code == 900) return;

            message = code switch
            {
                400 => "Некорректный запрос",
                401 => "Не авторизован",
                _ => message,
            };
            MessageBox.Show(message);
        }

        private static async Task Post(string request, Guid guid, object parms, bool withoutGuid = false)
        {
            StringContent jsonContent = new(
                JsonSerializer.Serialize(parms),
                Encoding.UTF8,
                "application/json");

            try
            {
                string url = $"{API_URL}{request}?guid={guid}";
                HttpResponseMessage response = await client.PostAsync(url, jsonContent);
                response.EnsureSuccessStatusCode();
                response.Dispose();
                return;
            }
            catch (HttpRequestException e)
            {
                OnError(e);
            }

            jsonContent.Dispose();

            return;
        }

        public static async Task<Session> Login(string login, string password)
        {
            Session session = await Get<Session>(
                "login", Guid.Empty,
                new Dictionary<string, string> {
                { "login", login },
                { "pass", password }
            }, true);
            CurrentSession = session;
            return session;
        }

        public static async Task Ping()
        {
            await Post(
                "ping", CurrentSession.id,
                new { });
            return;
        }

        public static async Task<List<Group>> GetGroups()
        {
            return await Get<List<Group>>(
                "getGroups", CurrentSession.id,
                new Dictionary<string, string>());
        }

        public static async Task<string> GetStudentFullName(Student st)
        {

            return await Get<string>(
                "getStudentFullName", CurrentSession.id,
                new Dictionary<string, string> {
                            { "id", st.IdStudent.ToString() }
            });
        }

        public static async Task<string> GetTeacherFullName(Teacher tch)
        {
            return await Get<string>(
                "getTeacherFullName", CurrentSession.id,
                new Dictionary<string, string> {
                            { "id", tch.IdTeacher.ToString() }
            });
        }

        public static async Task<List<string>> GetGroupStudentsFullNames(Group grp)
        {
            return await Get<List<string>>(
                "getGroupStudentsFullNames", CurrentSession.id,
                new Dictionary<string, string> {
                            { "id", grp.IdGroup.ToString() }
            });
        }

        public static async Task<string> GetUserFullName()
        {
            return await Get<string>(
                "getUserFullName", CurrentSession.id,
                new Dictionary<string, string>
                {});
        }
        public static async Task<string> GetUserRole()
        {
            return await Get<string>(
                "getUserRole", CurrentSession.id,
                new Dictionary<string, string>
                {});
        }
        
        public static async Task<bool> GetIsGroupElder()
        {
            return await Get<bool>(
                "getIsGroupElder", CurrentSession.id,
                new Dictionary<string, string>
                {});
        }

        public static async Task<Dictionary<string, List<Truancy>>> GetTruanciesInMonth(Group gr, int year, int month)
        {
            return await Get<Dictionary<string, List<Truancy>>>(
                "getTruanciesInMonth", CurrentSession.id,
                new Dictionary<string, string> {
                            { "groupId", gr.IdGroup.ToString() },
                            { "year", year.ToString() },
                            { "month", month.ToString() }
            });
        }
        
        public static async Task<Dictionary<int, List<Truancy>>> GetTruanciesInPeriodForGroup(Group gr, DateTime start, DateTime end)
        {
            return await Get<Dictionary<int, List<Truancy>>>(
                "getTruanciesInPeriodForGroup", CurrentSession.id,
                new Dictionary<string, string> {
                            { "groupId", gr.IdGroup.ToString() },
                            { "start", start.ToString("yyyy-MM-dd") },
                            { "end", end.ToString("yyyy-MM-dd") }
            });
        }
                
        public static async Task<List<List<int>>> GetScheduleInPeriod(Group gr, DateTime start, DateTime end)
        {
            return await Get<List<List<int>>>(
                "getScheduleInPeriod", CurrentSession.id,
                new Dictionary<string, string> {
                            { "groupId", gr.IdGroup.ToString() },
                            { "start", start.ToString("yyyy-MM-dd") },
                            { "end", end.ToString("yyyy-MM-dd") }
            });
        }

        public static async Task<List<Discipline>> GetDisciplinesForGroupInTerm(Group group, int term)
        {
            return await Get<List<Discipline>>(
                "getDisciplinesForSpecInTerm", CurrentSession.id,
                new Dictionary<string, string> {
                                        { "specId", group.IdSpec.ToString() },
                                        { "term", term.ToString() }
            });
        }

        public static async Task<List<Teacher>> GetTeachers()
        {
            return await Get<List<Teacher>>(
                "getTeachers", CurrentSession.id,
                new Dictionary<string, string>());
        }

        public static async Task<Dictionary<string, List<Truancy>>> GetTruanciesInTerm(Group gr, int year, int term, Discipline discipline, Teacher teacher)
        {
            int discId = discipline == null ? -1 : discipline.IdDiscipline;
            int teacherId = teacher == null ? -1 : teacher.IdTeacher;
            return await Get<Dictionary<string, List<Truancy>>>(
                "getTruanciesInTerm", CurrentSession.id,
                new Dictionary<string, string> {
                            { "groupId", gr.IdGroup.ToString() },
                            { "year", year.ToString() },
                            { "term", term.ToString() },
                            { "discId", discId.ToString() },
                            { "teacherId", teacherId.ToString() }
            });
        }
        
        public static async Task<List<Truancy>> GetTruanciesOfStudent(Student st, int year, int term, Discipline discipline, Teacher teacher)
        {
            int discId = discipline == null ? -1 : discipline.IdDiscipline;
            int teacherId = teacher == null ? -1 : teacher.IdTeacher;
            return await Get<List<Truancy>>(
                "getTruanciesOfStudent", CurrentSession.id,
                new Dictionary<string, string> {
                            { "studentId", st.IdStudent.ToString() },
                            { "year", year.ToString() },
                            { "term", term.ToString() },
                            { "discId", discId.ToString() },
                            { "teacherId", teacherId.ToString() }
            });
        }

        public static async Task<List<Student>> GetStudentsOfGroup(Group grp)
        {
            return await Get<List<Student>>(
                "getStudentsOfGroup", CurrentSession.id,
                new Dictionary<string, string> {
                                        { "groupId", grp.IdGroup.ToString() }
            });
        }

        public static async Task<List<Person>> GetPersonsOfGroupAdmin(Group grp)
        {
            int id = grp == null ? -1 : grp.IdGroup;
            return await Get<List<Person>>(
                "getPersonsOfGroupAdmin", CurrentSession.id,
                new Dictionary<string, string> {
                                        { "groupId", id.ToString() }
            });
        }

        public static async Task<List<Person>> GetPersonsOfTeachersAdmin()
        {
            return await Get<List<Person>>(
                "getPersonsOfTeachersAdmin", CurrentSession.id,
                new Dictionary<string, string>());
        }

        public static async Task<List<Person>> GetPersonsOfUsersAdmin()
        {
            return await Get<List<Person>>(
                "getPersonsOfUsersAdmin", CurrentSession.id,
                new Dictionary<string, string>());
        }



        public static async Task<int> GetPlanLessons(int group, int disc, int teacher)
        {
            return Convert.ToInt32(await Get<int>(
                "getPlanLessons", CurrentSession.id,
                new Dictionary<string, string> {
                                                    { "groupId", group.ToString() },
                                                    { "discId", disc.ToString() },
                                                    { "teacherId", teacher.ToString() },
            }));
        }



        public static async Task CreateStudent(string firstName, string lastName, string middleName, Group group)
        {
            int id = group == null ? -1 : group.IdGroup;

            await Post(
                "createStudent", CurrentSession.id,
                new
                {
                    firstName = firstName,
                    lastName = lastName,
                    middleName = middleName,
                    groupId = id,
                });
        }

        public static async Task CreateTeacher(string firstName, string lastName, string middleName)
        {
            await Post(
                "createTeacher", CurrentSession.id,
                new
                {
                    firstName = firstName,
                    lastName = lastName,
                    middleName = middleName,
                });
        }

        public static async Task CreateUser(Person person, string login, string password)
        {
            await Post(
                "createUser", CurrentSession.id,
                new
                {
                    personId = person.IdPerson,
                    login = login,
                    password = password,
                });
        }

        public static async Task CreateGroup(string name, string firstName, string lastName, string middleName, Person curator)
        {
            int curatorId = curator == null ? -1 : curator.IdPerson;
            await Post(
                "createGroup", CurrentSession.id,
                new
                {
                    name = name,
                    firstName = firstName,
                    lastName = lastName,
                    middleName = middleName,
                    curatorId = curatorId,
                });
        }


        public static async Task SetTruancy(int stId, DateTime date, int lessonNumber, bool present, bool hasReason)
        {
            await Post(
                "setTruancy", CurrentSession.id,
                new
                {
                    stId = stId,
                    date = date,
                    lessonNumber = lessonNumber,
                    present = present,
                    hasReason = hasReason,
                });
        }
    }
}
