using System.Text.Json;
using System.Text.Json.Serialization;

namespace AttendanceAPI
{

    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AttendanceDbContext>().ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.MapGet("/login/", (string login, string pass, AttendanceDbContext db) =>
            {
                Session? sess = DataBase.Login(login, pass, db);
                return sess == null ?
                Results.Problem("Failed to login", statusCode: 900) :
                Results.Ok(sess);
            });

            app.MapPost("/ping/", (Guid guid, AttendanceDbContext db) =>
            {
                bool success = DataBase.RenewSession(guid);
                return success ?
                Results.Ok(null) :
                Results.Problem("Unauthorized", statusCode: 401);
            });


            app.MapGet("/getStudentFullName/", (Guid guid, int id, AttendanceDbContext db) =>
            {
                return (DataBase.CheckGuid(guid) ?
                    Results.Problem("Unauthorized", statusCode: 401) :
                    DataBase.ReturnResponse(DataBase.GetStudentFullName(id, db)));
            });

            app.MapGet("/getUserFullName/", (Guid guid, AttendanceDbContext db) =>
            {
                return (DataBase.CheckGuid(guid) ?
                    Results.Problem("Unauthorized", statusCode: 401) :
                    Results.Ok(DataBase.GetUserFullName(DataBase.GetSessionUser(guid), db)));
            });

            app.MapGet("/getUserRole/", (Guid guid, AttendanceDbContext db) =>
            {
                return (DataBase.CheckGuid(guid) ?
                    Results.Problem("Unauthorized", statusCode: 401) :
                    Results.Ok(DataBase.GetUserRole(DataBase.GetSessionUser(guid), db)));
            });

            app.MapGet("/getIsGroupElder/", (Guid guid, AttendanceDbContext db) =>
            {
                return (DataBase.CheckGuid(guid) ?
                    Results.Problem("Unauthorized", statusCode: 401) :
                    Results.Ok(DataBase.GetIsGroupElder(DataBase.GetSessionUser(guid), db)));
            });

            app.MapGet("/getTeacherFullName/", (Guid guid, int id, AttendanceDbContext db) =>
            {
                return (DataBase.CheckGuid(guid) ?
                    Results.Problem("Unauthorized", statusCode: 401) :
                Results.Ok(DataBase.GetTeacherFullName(id, db)));
            });

            app.MapGet("/getGroupStudentsFullNames/", (Guid guid, int id, AttendanceDbContext db) =>
            {
                return (DataBase.CheckGuid(guid) ?
                    Results.Problem("Unauthorized", statusCode: 401) :
                Results.Ok(DataBase.GetGroupStudentsFullNames(DataBase.GetSessionUser(guid), id, db)));
            });

            app.MapGet("/getGroups/", (Guid guid, AttendanceDbContext db) =>
            {
                return (DataBase.CheckGuid(guid) ?
                    Results.Problem("Unauthorized", statusCode: 401) :
                Results.Ok(DataBase.GetGroups(DataBase.GetSessionUser(guid), db)));
            });
            
            app.MapGet("/getTruanciesInMonth/", (Guid guid, int groupId, int year, int month, AttendanceDbContext db) =>
            {
                return (DataBase.CheckGuid(guid) ?
                    Results.Problem("Unauthorized", statusCode: 401) :
                Results.Ok(DataBase.GetTruanciesInMonth(DataBase.GetSessionUser(guid), groupId, year, month, db)));
            });
            
            app.MapGet("/getTruanciesInPeriodForGroup/", (Guid guid, int groupId, DateTime start, DateTime end, AttendanceDbContext db) =>
            {
                return (DataBase.CheckGuid(guid) ?
                    Results.Problem("Unauthorized", statusCode: 401) :
                Results.Ok(DataBase.GetTruanciesInPeriod(DataBase.GetSessionUser(guid), groupId, start, end, db)));
            });
            
            app.MapGet("/getScheduleInPeriod/", (Guid guid, int groupId, DateTime start, DateTime end, AttendanceDbContext db) =>
            {
                return (DataBase.CheckGuid(guid) ?
                    Results.Problem("Unauthorized", statusCode: 401) :
                Results.Ok(DataBase.GetScheduleInPeriod(DataBase.GetSessionUser(guid), groupId, start, end, db)));
            });
            
            app.MapGet("/getDisciplinesForSpecInTerm/", (Guid guid, int specId, int term, AttendanceDbContext db) =>
            {
                return (DataBase.CheckGuid(guid) ?
                    Results.Problem("Unauthorized", statusCode: 401) :
                Results.Ok(DataBase.GetDisciplinesForSpecInTerm(specId, term, db)));
            });

            app.MapGet("/getTeachers/", (Guid guid, AttendanceDbContext db) =>
            {
                return (DataBase.CheckGuid(guid) ?
                    Results.Problem("Unauthorized", statusCode: 401) :
                Results.Ok(DataBase.GetTeachers(db)));
            });

            app.MapGet("/getTruanciesInTerm/", (Guid guid, int groupId, int year, int term, int discId, int teacherId, AttendanceDbContext db) =>
            {
                return (DataBase.CheckGuid(guid) ?
                    Results.Problem("Unauthorized", statusCode: 401) :
                Results.Ok(DataBase.GetTruanciesInTerm(DataBase.GetSessionUser(guid), groupId, year, term, discId, teacherId, db)));
            });
            
            app.MapGet("/getTruanciesOfStudent/", (Guid guid, int studentId, int year, int term, int discId, int teacherId, AttendanceDbContext db) =>
            {
                return (DataBase.CheckGuid(guid) ?
                    Results.Problem("Unauthorized", statusCode: 401) :
                Results.Ok(DataBase.GetTruanciesOfStudent(studentId, year, term, discId, teacherId, db)));
            });

            app.MapGet("/getStudentsOfGroup/", (Guid guid, int groupId, AttendanceDbContext db) =>
            {
                return (DataBase.CheckGuid(guid) ?
                    Results.Problem("Unauthorized", statusCode: 401) :
                Results.Ok(DataBase.GetStudentsOfGroup(DataBase.GetSessionUser(guid), groupId, db)));
            });

            app.MapGet("/getPersonsOfGroupAdmin/", (Guid guid, int groupId, AttendanceDbContext db) =>
            {
                return (DataBase.CheckGuid(guid) ?
                    Results.Problem("Unauthorized", statusCode: 401) :
                Results.Ok(DataBase.GetPersonsOfGroupAdmin(DataBase.GetSessionUser(guid), groupId, db)));
            });

            app.MapGet("/getPersonsOfTeachersAdmin/", (Guid guid, AttendanceDbContext db) =>
            {
                return (DataBase.CheckGuid(guid) ?
                    Results.Problem("Unauthorized", statusCode: 401) :
                Results.Ok(DataBase.GetPersonsOfTeachersAdmin(DataBase.GetSessionUser(guid), db)));
            });
            
            app.MapGet("/getPersonsOfUsersAdmin/", (Guid guid, AttendanceDbContext db) =>
            {
                return (DataBase.CheckGuid(guid) ?
                    Results.Problem("Unauthorized", statusCode: 401) :
                Results.Ok(DataBase.GetPersonsOfUsersAdmin(DataBase.GetSessionUser(guid), db)));
            });

            app.MapGet("/getPlanLessons/", (Guid guid, int groupId, int discId, int teacherId, AttendanceDbContext db) =>
            {
                return (DataBase.CheckGuid(guid) ?
                    Results.Problem("Unauthorized", statusCode: 401) :
                Results.Ok(DataBase.GetPlanLessons(groupId, discId, teacherId, db)));
            });

            app.MapPost("/createStudent/", (Guid guid, JsonElement data, AttendanceDbContext db) =>
            {
                var groupId = data.GetProperty("groupId").GetInt32();
                var firstName = data.GetProperty("firstName").GetString();
                var lastName = data.GetProperty("lastName").GetString();
                var middleName = data.GetProperty("middleName").GetString();
                return (DataBase.CheckGuid(guid) ?
                    Results.Problem("Unauthorized", statusCode: 401) :
                DataBase.ReturnResponse(DataBase.CreateStudent(DataBase.GetSessionUser(guid), groupId, firstName, lastName, middleName, db)));
            });

            app.MapPost("/createTeacher/", (Guid guid, JsonElement data, AttendanceDbContext db) =>
            {
                var firstName = data.GetProperty("firstName").GetString();
                var lastName = data.GetProperty("lastName").GetString();
                var middleName = data.GetProperty("middleName").GetString();
                return (DataBase.CheckGuid(guid) ?
                    Results.Problem("Unauthorized", statusCode: 401) :
                DataBase.ReturnResponse(DataBase.CreateTeacher(DataBase.GetSessionUser(guid), firstName, lastName, middleName, db)));
            });
            
            app.MapPost("/createGroup/", (Guid guid, JsonElement data, AttendanceDbContext db) =>
            {
                var name = data.GetProperty("name").GetString();
                var firstName = data.GetProperty("firstName").GetString();
                var lastName = data.GetProperty("lastName").GetString();
                var middleName = data.GetProperty("middleName").GetString();
                var curatorId = data.GetProperty("curatorId").GetInt32();

                return (DataBase.CheckGuid(guid) ?
                    Results.Problem("Unauthorized", statusCode: 401) :
                DataBase.ReturnResponse(DataBase.CreateGroup(DataBase.GetSessionUser(guid), name, firstName, lastName, middleName, curatorId, db)));
            });

            app.MapPost("/createUser/", (Guid guid, JsonElement data, AttendanceDbContext db) =>
            {
                var personId = data.GetProperty("personId").GetInt32();
                var login = data.GetProperty("login").GetString();
                var password = data.GetProperty("password").GetString();
                return (DataBase.CheckGuid(guid) ?
                    Results.Problem("Unauthorized", statusCode: 401) :
                DataBase.ReturnResponse(DataBase.CreateUser(DataBase.GetSessionUser(guid), personId, login, password, db)));
            });

            app.MapPost("/setTruancy/", (Guid guid, JsonElement data, AttendanceDbContext db) =>
            {
                var stId = data.GetProperty("stId").GetInt32();
                var date = data.GetProperty("date").GetDateTime();
                var lessonNumber = data.GetProperty("lessonNumber").GetInt32();
                var present = data.GetProperty("present").GetBoolean();
                var hasReason = data.GetProperty("hasReason").GetBoolean();
                return (DataBase.CheckGuid(guid) ?
                    Results.Problem("Unauthorized", statusCode: 401) :
                DataBase.ReturnResponse(DataBase.SetTruancy(DataBase.GetSessionUser(guid), stId, date, lessonNumber, present, hasReason, db)));
            });

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.Urls.Add("http://localhost:5001");
            //app.Urls.Add("http://10.0.33.17:5001");

            app.UseHttpsRedirection();

            //app.UseAuthorization();

            //for (int i = 0; i < 40; i++)
            //{
            //    DataBase.AddRandomTruancies(new AttendanceDbContext());
            //}

            DataBase.CheckSessions();

            app.Run();
        }
    }
}


