using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI
{
    enum UserRole
    {
        Student,
        Teacher,
        Admin,
    }

    internal static class DataBase
    {
        public static List<Session> Sessions = new List<Session>();

        public static IResult ReturnResponse(object? obj)
        {
            return obj == null ? Results.Problem("Fail", statusCode: 400) : Results.Ok(obj);
        }

        public static IResult ReturnResponse(bool? obj)
        {
            IResult problem = Results.Problem("Fail", statusCode: 400);
            return obj == null ? problem : (obj == true ? Results.Ok() : problem);
        }

        public static Session? Login(string login, string password, AttendanceDbContext db)
        {
            string psw = HashHelper.GenerateHash(password);
            List<User> users = db.Users.Where(o => o.Login == login &&
                                         o.Password == psw).ToList();

            User? user = users.FirstOrDefault();

            Session? session = null;

            if (user != null)
            {
                session = new Session(user);
                Sessions.Add(session);
            }
            return session;
        }

        public static async void CheckSessions()
        {
            while (true)
            {
                List<Session> newList = [.. Sessions];
                foreach (Session session in newList)
                {
                    if (session.IsExpired())
                    {
                        Sessions.Remove(session);
                    }
                }
                await Task.Delay(1000);
            }
        }

        public static bool RenewSession(Guid id)
        {
            Session sess = Sessions.Find(o => o.id == id)!;
            if (sess != null)
            {
                sess.Renew();
            }
            return sess != null;
        }

        public static User GetSessionUser(Guid id)
        {
            Session sess = Sessions.Find(o => o.id == id)!;
            if (sess == null) return null;
            return sess.user;
        }


        public static string GetUserRole(User user, AttendanceDbContext db)
        {
            return db.Roles.Where(o => o.IdRole == user.IdRole).FirstOrDefault().Name;
        }

        public static bool GetIsGroupElder(User user, AttendanceDbContext db)
        {
            return IsGroupElder(db.Students.Where(o => o.IdPerson == user.IdPerson).FirstOrDefault(), db);
        }

        public static bool CheckGuid(Guid guid)
        {
            return Sessions.FindIndex(o => o.id == guid) == -1;
        }

        public static string? GetStudentFullName(int st, AttendanceDbContext db)
        {
            Student? s = db.Students.Where(o => o.IdStudent == st).FirstOrDefault();
            if (s == null) { return null; };
            return GetFullName(s.IdPerson, db);
        }

        public static string? GetTeacherFullName(int st, AttendanceDbContext db)
        {
            Teacher? s = db.Teachers.Where(o => o.IdTeacher == st).FirstOrDefault();
            if (s == null) { return null; };
            return GetFullName(s.IdPerson, db);
        }

        public static List<string> GetGroupStudentsFullNames(User user, int id, AttendanceDbContext db)
        {
            List<Student> s = GetStudentsOfGroup(user, id, db);
            List<string> names = new List<string>();
            foreach (Student st in s)
            {
                names.Add(GetFullName(st.IdPerson, db));
            }
            return names;
        }

        public static string? GetUserFullName(User user, AttendanceDbContext db)
        {
            return GetFullName(user.IdPerson, db);
        }

        public static string? GetFullName(int id, AttendanceDbContext db)
        {
            Person? p = db.People.Where(o => o.IdPerson == id).FirstOrDefault();
            if (p == null) { return null; };
            return p.LastName + " " + p.FirstName + " " + p.MiddleName;
        }

        public static int GetPlanLessons(int groupId, int discId, int teacherId, AttendanceDbContext db)
        {
            List<Schedule> sch = db.Schedules.Where(o => o.IdGroup == groupId &&
                                     (discId == -1 || o.IdDiscipline == discId) &&
                                     (teacherId == -1 || o.IdTeacher == teacherId)).ToList();


            return sch.Count;
        }

        public static List<Group> GetGroups(User user, AttendanceDbContext db)
        {
            if (user.IdRole == (int)UserRole.Student)
            {
                Group? group = GetStudentGroupFromUser(user, db);
                if (group == null) throw new Exception("group == null");
                return new List<Group>() { group };
            }

            return db.Groups.ToList();
        }

        public static List<Discipline> GetDisciplinesForSpecInTerm(int SpecId, int term, AttendanceDbContext db)
        {
            List<StudyPlan> plan = db.StudyPlans
                .Include(o => o.IdDisciplineNavigation)
                .Where(o => (SpecId == -1 || o.IdSpec == SpecId) &&
                            o.Term == term).ToList();

            List<Discipline> disciplines = new List<Discipline>();
            foreach (StudyPlan p in plan)
            {
                disciplines.Add(p.IdDisciplineNavigation);
            }
            return disciplines;
        }

        public static Dictionary<string, List<Truancy>> GetTruanciesInTerm(User user, int grId, int year, int term, int discId, int teacherId, AttendanceDbContext db)
        {
            DateOnly s, e;
            GetTimePeriod(year, term, out s, out e);

            List<Truancy> all = db.Truancies
                .Include(o => o.IdScheduleNavigation)
                .Where(o => o.IdScheduleNavigation.IdGroup == grId &&
                o.IdScheduleNavigation.LessonDate >= s &&
                o.IdScheduleNavigation.LessonDate <= e &&
                (discId == -1 || o.IdScheduleNavigation.IdDiscipline == discId) &&
                (teacherId == -1 || o.IdScheduleNavigation.IdTeacher == teacherId)).ToList();
            return ParseTruancies(user, grId, all, db);
        }

        public static List<Truancy> GetTruanciesOfStudent(int stId, int year, int term, int discId, int teacherId, AttendanceDbContext db)
        {
            DateOnly s, e;
            GetTimePeriod(year, term, out s, out e);

            List<Truancy> all = db.Truancies
                .Include(o => o.IdScheduleNavigation)
                .Where(o => o.IdStudent == stId &&
                o.IdScheduleNavigation.LessonDate >= s &&
                o.IdScheduleNavigation.LessonDate <= e &&
                (discId == -1 || o.IdScheduleNavigation.IdDiscipline == discId) &&
                (teacherId == -1 || o.IdScheduleNavigation.IdTeacher == teacherId)).ToList();
            return all;
        }

        public static Dictionary<int, List<Truancy>> GetTruanciesInPeriod(User user, int grId, DateTime start, DateTime end, AttendanceDbContext db)
        {
            DateOnly s = DateOnly.FromDateTime(start);
            DateOnly e = DateOnly.FromDateTime(end);

            List<Truancy> all = db.Truancies
                .Include(o => o.IdScheduleNavigation)
                .Where(o => o.IdScheduleNavigation.IdGroup == grId &&
                o.IdScheduleNavigation.LessonDate >= s &&
                o.IdScheduleNavigation.LessonDate <= e).ToList();
            return ParseTruanciesWithIndexes(user, grId, all, db);
        }

        public static List<List<int>> GetScheduleInPeriod(User user, int grId, DateTime start, DateTime end, AttendanceDbContext db)
        {
            DateOnly s = DateOnly.FromDateTime(start);
            DateOnly e = DateOnly.FromDateTime(end);

            List<Schedule> all = db.Schedules
                .Where(o => o.IdGroup == grId &&
                o.LessonDate >= s &&
                o.LessonDate <= e).ToList();

            List<List<int>> data = new List<List<int>>();
            for (DateTime i = start; i < end; i += TimeSpan.FromDays(1))
            {
                List<Schedule> thisDay = all.FindAll(o => o.LessonDate == DateOnly.FromDateTime(i));
                List<int> nums = new List<int>();
                foreach (Schedule sch in thisDay)
                {
                    nums.Add(sch.LessonNumber);
                }
                data.Add(nums);
            }

            return data;
        }

        private static void GetTimePeriod(int year, int term, out DateOnly s, out DateOnly e)
        {
            DateTime start;
            DateTime end;
            switch (term)
            {
                case 1:
                    start = new DateTime(year, 9, 1);
                    end = new DateTime(year, 12, 31);
                    break;
                case 2:
                    start = new DateTime(year + 1, 1, 1);
                    end = new DateTime(year + 1, 8, 31);
                    break;
                default:
                    start = new DateTime(year, 9, 1);
                    end = new DateTime(year + 1, 8, 31);
                    break;

            }

            s = DateOnly.FromDateTime(start);
            e = DateOnly.FromDateTime(end);
        }

        public static List<Student> GetStudentsOfGroup(User user, int grId, AttendanceDbContext db)
        {
            if (user.IdRole == (int)UserRole.Student)
            {
                Student st = GetStudentFromUser(user, db);
                if (st.IdGroup != grId)
                {
                    return new List<Student>();
                }
                if (!IsGroupElder(st, db))
                {
                    return new List<Student>() { st };
                }
            }

            return db.Students
                    .Include(o => o.IdPersonNavigation)
                    .Include(o => o.Truancies)
                    .Where(o => grId == -1 || o.IdGroup == grId).ToList();
        }

        public static List<Person> GetPersonsOfGroupAdmin(User user, int grId, AttendanceDbContext db)
        {
            if (user.IdRole != (int)UserRole.Admin)
            {
                return [];
            }
            List<Student> st = GetStudentsOfGroup(user, grId, db);
            List<Person> persons = new List<Person>();
            foreach (Student student in st)
            {
                persons.Add(student.IdPersonNavigation);
            }
            return persons;
        }
        public static List<Person> GetPersonsOfTeachersAdmin(User user, AttendanceDbContext db)
        {
            if (user.IdRole != (int)UserRole.Admin)
            {
                return [];
            }
            List<Teacher> tch = db.Teachers.Include(o => o.IdPersonNavigation).ToList();
            List<Person> persons = new List<Person>();
            foreach (Teacher teacher in tch)
            {
                persons.Add(teacher.IdPersonNavigation);
            }
            return persons;
        }
        
        public static List<Person> GetPersonsOfUsersAdmin(User user, AttendanceDbContext db)
        {
            if (user.IdRole != (int)UserRole.Admin)
            {
                return [];
            }
            List<User> usr = db.Users.Include(o => o.IdPersonNavigation).ToList();
            List<Person> persons = new List<Person>();
            foreach (User u in usr)
            {
                persons.Add(u.IdPersonNavigation);
            }
            return persons;
        }

        public static Dictionary<string, List<Truancy>> GetTruanciesInMonth(User user, int grId, int year, int month, AttendanceDbContext db)
        {
            List<Truancy> all = db.Truancies
                .Include(o => o.IdScheduleNavigation)
                .Where(o => o.IdScheduleNavigation.IdGroup == grId &&
                o.IdScheduleNavigation.LessonDate.Year == year &&
                o.IdScheduleNavigation.LessonDate.Month == month).ToList();
            return ParseTruancies(user, grId, all, db);
        }

        public static List<Teacher> GetTeachers(AttendanceDbContext db)
        {
            return db.Teachers.IgnoreAutoIncludes().ToList();
        }


        public static string GetPersonFullName(Person person)
        {
            return person == null ? "" : $"{person.LastName} {person.FirstName} {person.MiddleName}";
        }




        private static Dictionary<string, List<Truancy>> ParseTruancies(User user, int grId, List<Truancy> all, AttendanceDbContext db)
        {
            List<Student> students = GetStudentsOfGroup(user, grId, db);

            Dictionary<string, List<Truancy>> data = new Dictionary<string, List<Truancy>>();
            foreach (Student student in students)
            {
                data.Add(GetPersonFullName(student.IdPersonNavigation), all.FindAll(o => o.IdStudent == student.IdStudent));
            }

            return data;
        }

        private static Dictionary<int, List<Truancy>> ParseTruanciesWithIndexes(User user, int grId, List<Truancy> all, AttendanceDbContext db)
        {
            List<Student> students = GetStudentsOfGroup(user, grId, db);

            Dictionary<int, List<Truancy>> data = new Dictionary<int, List<Truancy>>();
            foreach (Student student in students)
            {
                data.Add(student.IdStudent, all.FindAll(o => o.IdStudent == student.IdStudent));
            }

            return data;
        }


        public static int GetPlanLessons(Specialization spec, Discipline disc, int term, AttendanceDbContext db)
        {
            return GetPlanLessonsPlan(spec.IdSpec, disc, term, db);
        }

        public static int GetPlanLessonsPlan(int idSpec, Discipline disc, int term, AttendanceDbContext db)
        {
            List<StudyPlan> plan = db.StudyPlans.Where(o => o.IdSpec == idSpec &&
                                     (disc == null || o.IdDiscipline == disc.IdDiscipline) &&
                                     o.Term == term).ToList();


            int count = 0;
            foreach (StudyPlan plan2 in plan)
            {
                count += plan2.LessonsCount;
            }
            return count;
        }

        public static Student? GetStudentFromUser(User user, AttendanceDbContext db)
        {
            return db.Students.Where(o => o.IdPerson == user.IdPerson).Include(o => o.IdPersonNavigation).FirstOrDefault();
        }

        public static Group? GetStudentGroupFromUser(User user, AttendanceDbContext db)
        {
            return db.Students.Where(o => o.IdPerson == user.IdPerson).Include(o => o.IdGroupNavigation).FirstOrDefault()!.IdGroupNavigation;
        }

        public static bool IsGroupElder(Student student, AttendanceDbContext db)
        {
            Group? group = db.Groups.Where(o => o.IdGroup == student.IdGroup).FirstOrDefault();
            if (group == null) return false;

            return group.IdElder == student.IdStudent;
        }


        public static bool CreateStudent(User user, int groupId, string firstName, string lastName, string middleName, AttendanceDbContext db)
        {
            if (user == null || user.IdRole != (int)UserRole.Admin) return false;
            if (groupId == -1) return false;

            Person person = CreatePerson(firstName, lastName, middleName, db);

            db.Students.Add(new Student
            {
                IdPerson = person.IdPerson,
                IdGroup = groupId
            });
            db.SaveChanges();

            return true;
        }

        public static bool CreateTeacher(User user, string firstName, string lastName, string middleName, AttendanceDbContext db)
        {
            if (user == null || user.IdRole != (int)UserRole.Admin) return false;
            Person person = CreatePerson(firstName, lastName, middleName, db);

            db.Teachers.Add(new Teacher
            {
                IdPerson = person.IdPerson
            });
            db.SaveChanges();

            return true;
        }

        public static bool CreateUser(User user, int personId, string login, string password, AttendanceDbContext db)
        {
            if (user == null || user.IdRole != (int)UserRole.Admin) return false;
            db.Users.Add(new User
            {
                IdPerson = personId,
                Login = login,
                Password = HashHelper.GenerateHash(password)
            });
            db.SaveChanges();
            return true;
        }
        public static bool SetTruancy(User user, int stId, DateTime date, int lessonNumber, bool present, bool hasReason, AttendanceDbContext db)
        {
            if (user == null || (user.IdRole == (int)UserRole.Student && !GetIsGroupElder(user, db))) return false;
            Student st = db.Students.Where(o => o.IdStudent == stId).First();
            
            Schedule? sch = db.Schedules.Include(o => o.Truancies).Include(o => o.IdGroupNavigation).Where(o =>
                o.LessonDate == DateOnly.FromDateTime(date) &&
                o.LessonNumber == lessonNumber &&
                o.IdGroup == st.IdGroup).FirstOrDefault();
            if (sch == null)
            {
                return true;
            }
            if (present)
            {
                Truancy? tr = sch.Truancies.Where(o => o.IdStudent == stId).FirstOrDefault();
                if (tr == null)
                {
                    return true;
                }

                db.Truancies.Remove(tr);
            }
            else
            {
                Truancy? t = sch.Truancies.Where(o => o.IdStudent == stId).FirstOrDefault();
                if (t == null)
                {
                    db.Truancies.Add(new Truancy() { IdStudent = stId, IdReason = hasReason ? 1 : 0, IdSchedule = sch.IdSchedule });
                }
                else
                {
                    t.IdReason = hasReason ? 1 : 0;
                }
            }

            db.SaveChanges();
            return true;
        }

        private static Person CreatePerson(string firstName, string lastName, string middleName, AttendanceDbContext db)
        {
            Person person = new Person
            {
                FirstName = firstName,
                LastName = lastName,
                MiddleName = middleName,
            };

            db.People.Add(person);
            db.SaveChanges();
            return person;
        }

        public static bool CreateGroup(User user, string name, string firstName, string lastName, string middleName, int curatorId, AttendanceDbContext db)
        {
            if (user == null || user.IdRole != (int)UserRole.Admin) return false;
            Person person = CreatePerson(firstName, lastName, middleName, db);
            Teacher t = db.Teachers.Where(o => o.IdPerson == curatorId).FirstOrDefault();
            if (t == null) { return false; }


            Student student = new Student
            {
                IdPerson = person.IdPerson,
            };

            db.Students.Add(new Student
            {
                IdPerson = person.IdPerson,
            });


            Group grp = new Group
            {
                Name = name,
                IdElderNavigation = student,
                IdTeacherCurator = t.IdTeacher,
            };

            db.Groups.Add(grp);
            db.SaveChanges();
            return true;
        }



        #region FILL TEST DATA

        //        public static void FillRandomSchedule(int year, int month)
        //        {
        //            Random random = new Random();
        //            int days = DateTime.DaysInMonth(year, month);

        //            List<int> grps = new List<int>();
        //            foreach (Group g in db.Groups)
        //            {
        //                grps.Add(g.IdGroup);
        //            }

        //            List<int> discps = new List<int>();
        //            foreach (Discipline g in db.Disciplines)
        //            {
        //                discps.Add(g.IdDiscipline);
        //            }

        //            List<int> tchrs = new List<int>();
        //            foreach (Teacher g in db.Teachers)
        //            {
        //                tchrs.Add(g.IdTeacher);
        //            }

        //            foreach (int groupid in grps)
        //            {
        //                for (int i = 1; i < days; i++)
        //                {
        //                    DateTime d = new DateTime(year, month, i);
        //                    if (d.DayOfWeek == DayOfWeek.Sunday)
        //                    {
        //                        continue;
        //                    }
        //                    int max = 4;
        //                    if (random.Next(0, 10) > 7)
        //                    {
        //                        max = 3;
        //                    }
        //                    for (int j = 1; j <= max; j++)
        //                    {
        //                        db.Schedules.Add(new Schedule
        //                        {
        //                            LessonNumber = j,
        //                            LessonDate = DateOnly.FromDateTime(d),
        //                            IdGroup = groupid,
        //                            IdDiscipline = discps[random.Next(0, discps.Count)],
        //                            IdTeacher = tchrs[random.Next(0, tchrs.Count)],
        //                        });
        //                    }
        //                }
        //            }

        //            db.SaveChanges();

        //        }







        public static void AddRandomTruancies(AttendanceDbContext db)
        {
            Random rnd = new Random();



            List<int> discps = new List<int>();
            foreach (Discipline g in db.Disciplines)
            {
                discps.Add(g.IdDiscipline);
            }

            List<Student> students = db.Students.ToList();
            foreach (Student st in students)
            {
                if (rnd.Next(0, 10) > 4)
                {
                    DateOnly date = DateOnly.FromDateTime(new DateTime(2024, 12, rnd.Next(1, 32)));

                    for (int i = 0; i < rnd.Next(1, 5); i++)
                    {
                        List<Schedule> foundSchedule = db.Schedules.Where(o =>
                        o.LessonDate == date &&
                        o.LessonNumber == i &&
                        o.IdGroup == st.IdGroup).ToList();

                        if (foundSchedule.Count == 0)
                        {
                            continue;
                        }


                        db.Truancies.Add(new Truancy()
                        {
                            IdSchedule = foundSchedule.First().IdSchedule,
                            IdStudent = st.IdStudent,
                            IdReason = rnd.Next(0, 2)
                        });
                    }

                }
            }

            db.SaveChanges();
        }

        //        private static void AddStudents(string[,] names, int grpId)
        //        {
        //            //List<Person> perses = new List<Person>();
        //            //for (int i = 0; i < names.GetLength(0); i++)
        //            //{
        //            //    Person pers = new Person() { LastName = names[i, 0], FirstName = names[i, 1], MiddleName = names[i, 2] };
        //            //    DataBase.db.People.Add(pers);
        //            //    perses.Add(pers);
        //            //}
        //            //DataBase.db.SaveChanges();

        //            //for (int i = 0; i < names.GetLength(0); i++)
        //            //{
        //            //    DataBase.db.Students.Add(new Student() { IdGroup = grpId, IdPerson = perses[i].IdPerson });
        //            //}

        //            //DataBase.db.SaveChanges();
        //        }


        //        private static void AddTestStudents()
        //        {
        //            string[,] names1 = new string[,]
        //{
        //                { "Шамшева", "Валентина", "Ефимовна" },
        //                { "Ягова", "Людмила", "Семеновна" },
        //                { "Олупов", "Данил", "Викторович" },
        //                { "Шакирьянова", "Ольга", "Викторовна" },
        //                { "Анпилова", "Элина", "Витальевна" },
        //                { "Мухарская", "Олеся", "Денисовна" },
        //                { "Шехватова", "Анастасия", "Витальевна" },
        //                { "Бросалина", "Алиса", "Степановна" },
        //                { "Боярчикова", "Валентина", "Игоревна" },
        //                { "Агренева", "Анастасия", "Вячеславовна" },
        //                { "Хангалов", "Роберт", "Витальевич" },
        //                { "Шумарин", "Вячеслав", "Дамирович" },
        //                { "Шеворошкина", "Надежда", "Николаевна" },
        //                { "Бекиш", "Ярослав", "Артемович" },
        //                { "Холодковская", "Мария", "Ефимовна" },
        //                { "Бычникова", "Анжелика", "Кирилловна" },
        //                { "Ахмеров", "Ефим", "Денисович" },
        //                { "Упырина", "Евгения", "Артемовна" },
        //                { "Казаринова", "Полина", "Романовна" },
        //                { "Любятина", "Алла", "Яновна" },
        //                { "Иегудина", "Олеся", "Дамировна" },
        //                { "Королев", "Антон", "Максимович" },
        //                { "Торговкина", "Александра", "Рамилевна" },
        //                { "Конышев", "Яков", "Тимурович" },
        //                { "Бушак", "Роман", "Альбертович" },
        //};

        //            string[,] names2 = new string[,]
        //            {
        //                { "Безроднова", "Елизавета", "Петровна" },
        //                { "Мосолова", "Маргарита", "Ивановна"},
        //                { "Христораднова", "Виктория", "Маратовна"},
        //                { "Мамонина", "Екатерина", "Егоровна"},
        //                { "Евстегнеев", "Евгений", "Валерьевич"},
        //                { "Якунькин", "Роберт", "Иванович"},
        //                { "Ивасишин", "Виталий", "Артурович"},
        //                { "Хитрик", "Герман", "Геннадьевич"},
        //                { "Сапельникова", "Яна", "Петровна"},
        //                { "Циблиева", "Раиса", "Александровна"},
        //                { "Плиева", "Жанна", "Степановна"},
        //                { "Асонова", "Ольга", "Тимофеевна"},
        //                { "Карпин", "Марат", "Викторович"},
        //                { "Берладин", "Валерий", "Денисович"},
        //                { "Жаврук", "Альбина", "Глебовна"},
        //                { "Транквилицкий", "Степан", "Ильич"},
        //                { "Степаненко", "Иван", "Русланович" },
        //                { "Княгинина", "Анна", "Руслановна"},
        //                { "Крянина", "Ольга", "Ярославовна"},
        //                { "Куличенко", "Игорь", "Васильевич"   },
        //                { "Аникьева", "Эльмира", "Витальевна"},
        //                { "Степушкин", "Иван", "Константинович"},
        //                { "Гриболева", "Юлия", "Дамировна"},
        //                { "Кастальский", "Ярослав", "Вячеславович" },
        //                { "Лисица", "Марсель", "Аркадьевич"},

        //            };

        //            string[,] names3 = new string[,]
        //            {
        //                { "Неверовская", "Алла", "Кирилловна" },
        //                { "Атамась", "Вадим", "Ильич"},
        //                { "Бозоян", "Георгий", "Дамирович"},
        //                { "Денев", "Виктор", "Владимирович"},
        //                { "Романченко", "Иван", "Ринатович"},
        //                { "Пилявина", "Тамара", "Игоревна"},
        //                { "Гадов", "Александр", "Егорович"},
        //                { "Бугулашвили", "Сергей", "Геннадьевич"},
        //                { "Анцифирова", "Римма", "Аркадьевна"},
        //                { "Евтушенкова", "Тамара", "Тимофеевна"},
        //                { "Самоцветова", "Вероника", "Артуровна"},
        //                { "Польщиков", "Егор", "Витальевич"},
        //                { "Брикошина", "Лидия", "Антоновна"},
        //                { "Анкаева", "Любовь", "Эдуардовна"},
        //                { "Бесценный", "Владимир", "Ильдарович"},
        //                { "Ксенафонтов", "Максим", "Анатольевич"},
        //                { "Юстова", "Ангелина", "Ефимовна" },
        //                { "Фефелов", "Филипп", "Вадимович"},
        //                { "Афанасьевский", "Тимофей", "Артемович"},
        //                { "Эйлер", "Глеб", "Григорьевич"   },
        //                { "Азарнина", "Инна", "Яковлевна"},
        //                { "Осьмакова", "Елизавета", "Викторовна"},
        //                { "Лариошкин", "Денис", "Артурович"},
        //                { "Красивый", "Петр", "Владимирович" },
        //                { "Суперанская", "Мария", "Тимофеевна"},

        //            };

        //            string[,] names4 = new string[,]
        //            {
        //                { "Аляков", "Ильдар", "Алексеевич" },
        //                { "Новакова", "Раиса", "Тимофеевна"},
        //                { "Назарук", "Виктория", "Кирилловна"},
        //                { "Худайбердыева", "Кира", "Алексеевна"},
        //                { "Бейсембаева", "Яна", "Яновна"},
        //                { "Девяшин", "Иван", "Никитович"},
        //                { "Ратушняк", "Карина", "Яковлевна"},
        //                { "Мордовцев", "Виталий", "Дмитриевич"},
        //                { "Хадарцев", "Александр", "Юрьевич"},
        //                { "Обносков", "Тимофей", "Рамилевич"},
        //                { "Гоева", "Маргарита", "Борисовна"},
        //                { "Высотина", "Александра", "Робертовна"},
        //                { "Хорошилов", "Геннадий", "Иванович"},
        //                { "Рыболова", "Алина", "Робертовна"},
        //                { "Райнина", "Диана", "Егоровна"},
        //                { "Галашова", "Людмила", "Ефимовна"},
        //                { "Аленборн", "Роберт", "Борисович" },
        //                { "Мравинский", "Никита", "Юрьевич"},
        //                { "Тепцова", "Лилия", "Вячеславовна"},
        //                { "Рожнина", "Виктория", "Никитовна"   },
        //                { "Колесниченко", "Олеся", "Михаиловна"},
        //                { "Васюк", "Евгений", "Максимович"},
        //                { "Музалевская", "Эльмира", "Владимировна"},
        //                { "Брак", "Олег", "Олегович" },
        //                { "Щеблыкина", "Анжелика", "Андреевна"},

        //            };

        //            string[,] names5 = new string[,]
        //            {
        //                { "Трофименко", "Виталий", "Андреевич" },
        //                { "Лобачева", "Александра", "Артуровна"},
        //                { "Образков", "Максим", "Русланович"},
        //                { "Анцифирова", "Надежда", "Вячеславовна"},
        //                { "Цехмистрова", "Любовь", "Ринатовна"},
        //                { "Хатуева", "Олеся", "Антоновна"},
        //                { "Копейкин", "Ярослав", "Валентинович"},
        //                { "Гусманов", "Аркадий", "Станиславович"},
        //                { "Васягин", "Эдуард", "Глебович"},
        //                { "Яблуковская", "Мария", "Алексеевна"},
        //                { "Туржанская", "Ангелина", "Филипповна"},
        //                { "Антощук", "Иван", "Станиславович"},
        //                { "Бакулева", "Наталья", "Денисовна"},
        //                { "Зимницына", "Алла", "Борисовна"},
        //                { "Козадаев", "Михаил", "Вячеславович"},
        //                { "Нащокина", "Алина", "Яковлевна"},
        //                { "Кутилов", "Вячеслав", "Марселевич" },
        //                { "Бовт", "Вячеслав", "Викторович"},
        //                { "Авдеюк", "Михаил", "Ефимович"},
        //                { "Буханко", "Тимофей", "Владимирович"   },
        //                { "Максимчук", "Сергей", "Максимович"},
        //                { "Рябченко", "Александра", "Евгеньевна"},
        //                { "Туробеева", "Любовь", "Эдуардовна"},
        //                { "Упорова", "Ольга", "Григорьевна" },
        //                { "Трефилов", "Артур", "Тимурович"},

        //            };

        //            AddStudents(names1, 1);
        //            AddStudents(names2, 2);
        //            AddStudents(names3, 3);
        //            AddStudents(names4, 4);
        //            AddStudents(names5, 5);
        //        }

        #endregion
    }
}
