using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using UsingDapper.Infrastucture;
using UsingDapper.Core;
using UsingDapper.Core.DTO;

namespace UsingDapper.Infrastucture.Repositories
{
    public class DataGeneratorRepository
    {
        private readonly DapperContext _context;

        public DataGeneratorRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task InsertRandomDataAsync()
        {
            var insertStudentsQuery = @"
            INSERT INTO students (name, age) VALUES
                ('John Smith', 19),
                ('Jane Doe', 19),
                ('Michael Johnson', 21),
                ('Emily Davis', 22),
                ('Matthew Brown', 25),
                ('Olivia Wilson', 18),
                ('David Garcia', 24),
                ('Sophia Martinez', 23),
                ('James Anderson', 23),
                ('Isabella Lee', 21) ON CONFLICT DO NOTHING";

            var insertCoursesQuery = @"
            INSERT INTO courses (course_code, course_name) VALUES
                ('CS101', 'Introduction to Computer Science'),
                ('MAT202', 'Calculus II'),
                ('HIS303', 'Modern History'),
                ('BIO404', 'Biology of Cells'),
                ('PHY505', 'Quantum Mechanics'),
                ('CHE606', 'Organic Chemistry'),
                ('ECO707', 'Microeconomics'),
                ('ART808', 'Art History'),
                ('ENG909', 'English Literature'),
                ('PSY101', 'Introduction to Psychology')";

            var assignCoursesToStudentsQuery = @"
                INSERT INTO course_students (course_id, student_id) VALUES
                (1, 1),
                (1, 2),
                (2, 3),
                (3, 4),
                (4, 5),
                (1, 6),
                (2, 7),
                (3, 8),
                (4, 9),
                (1, 10)  ON CONFLICT DO NOTHING;";

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(insertStudentsQuery);
                await connection.ExecuteAsync(insertCoursesQuery);
                await connection.ExecuteAsync(assignCoursesToStudentsQuery);
            }
        }

        public async Task<IEnumerable<CourseDto>> GetCoursesWithStudentsOlderThan20Async()
        {
            var query = @"
                SELECT c.id as CourseId, c.course_code as CourseCode, c.course_name as CourseName, 
                       s.id as StudentId, s.name as StudentName, s.age as StudentAge
                FROM courses c
                JOIN course_students cs ON c.id = cs.course_id
                JOIN students s ON s.id = cs.student_id
                WHERE s.age > 20";

            using (var connection = _context.CreateConnection())
            {
                var courseDict = new Dictionary<int, CourseDto>();

                var courses = await connection.QueryAsync<CourseDto, StudentDto, CourseDto>(
                    query,
                    (course, student) =>
                    {
                        if (!courseDict.TryGetValue(course.CourseId, out var currentCourse))
                        {
                            currentCourse = course;
                            currentCourse.Students = new List<StudentDto>();
                            courseDict.Add(currentCourse.CourseId, currentCourse);
                        }
                        currentCourse.Students.Add(student);
                        return currentCourse;
                    },
                    splitOn: "StudentId"
                );

                return courseDict.Values.ToList();
            }
        }

        public async Task InsertCourseAsync(Course course)
        {
            var query = "INSERT INTO courses (course_code, course_name) VALUES (@CourseCode, @CourseName) RETURNING id";

            using (var connection = _context.CreateConnection())
            {
                course.Id = await connection.ExecuteScalarAsync<int>(query, course);
            }
        }

        public async Task InsertStudentAsync(Student student)
        {
            var query = "INSERT INTO students (name, age) VALUES (@Name, @Age) RETURNING id";

            using (var connection = _context.CreateConnection())
            {
                student.Id = await connection.ExecuteScalarAsync<int>(query, student);
            }
        }

        public async Task AssignCourseToStudentAsync(int courseId, int studentId)
        {
            var query = "INSERT INTO course_students (course_id, student_id) VALUES (@CourseId, @StudentId)";

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { CourseId = courseId, StudentId = studentId });
            }
        }

        public async Task RemoveStudentFromCourseAsync(int courseId, int studentId)
        {
            var query = "DELETE FROM course_students WHERE course_id = @CourseId AND student_id = @StudentId";

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { CourseId = courseId, StudentId = studentId });
            }
        }
    }
}
