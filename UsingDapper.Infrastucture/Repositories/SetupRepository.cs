using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using UsingDapper.Infrastucture;

namespace UsingDapper.Infrastucture.Repositories
{
    public class SetupRepository
    {
        private readonly DapperContext _context;

        public SetupRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task CreateTablesAsync()
        {
            var studentsTableQuery = @"
                CREATE TABLE IF NOT EXISTS students
                (
                    id SERIAL PRIMARY KEY,
                    name VARCHAR(100) NOT NULL,
                    age INTEGER NOT NULL
                );";

            var coursesTableQuery = @"
                CREATE TABLE IF NOT EXISTS courses
                (
                    id SERIAL PRIMARY KEY,
                    course_code VARCHAR(20) NOT NULL,
                    course_name VARCHAR(100) NOT NULL
                );";

            var courseStudentsTableQuery = @"
                CREATE TABLE IF NOT EXISTS course_students
                (
                    course_id INTEGER NOT NULL,
                    student_id INTEGER NOT NULL,
                    PRIMARY KEY (course_id, student_id),
                    FOREIGN KEY (course_id) REFERENCES courses(id),
                    FOREIGN KEY (student_id) REFERENCES students(id)
                );";

            using (var connection = _context.CreateConnection())
            {
                
                    await connection.ExecuteAsync(studentsTableQuery);
                    await connection.ExecuteAsync(coursesTableQuery);
                    await connection.ExecuteAsync(courseStudentsTableQuery);
                   
            }
        }
    }
}
