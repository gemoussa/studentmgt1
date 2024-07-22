using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UsingDapper.Infrastucture.Repositories;
using UsingDapper.Core;

namespace UsingDapper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly DataGeneratorRepository _dataGeneratorRepository;

        public CourseController(DataGeneratorRepository dataGeneratorRepository)
        {
            _dataGeneratorRepository = dataGeneratorRepository;
        }

        [HttpGet("students/older-than-20")]
        public async Task<IActionResult> GetCoursesWithStudentsOlderThan20()
        {
            var courses = await _dataGeneratorRepository.GetCoursesWithStudentsOlderThan20Async();
            return Ok(courses);
        }

        [HttpPost("insert-course")]
        public async Task<IActionResult> InsertCourse([FromBody] Course course)
        {
            await _dataGeneratorRepository.InsertCourseAsync(course);
            return Ok(course);
        }

        [HttpPost("insert-student")]
        public async Task<IActionResult> InsertStudent([FromBody] Student student)
        {
            await _dataGeneratorRepository.InsertStudentAsync(student);
            return Ok(student);
        }

        [HttpPost("assign-course")]
        public async Task<IActionResult> AssignCourseToStudent(int courseId, int studentId)
        {
            await _dataGeneratorRepository.AssignCourseToStudentAsync(courseId, studentId);
            return Ok();
        }

        [HttpDelete("remove-student")]
        public async Task<IActionResult> RemoveStudentFromCourse(int courseId, int studentId)
        {
            await _dataGeneratorRepository.RemoveStudentFromCourseAsync(courseId, studentId);
            return Ok();
        }
    }
}
