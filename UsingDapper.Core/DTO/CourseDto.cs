using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsingDapper.Core.DTO
{
    public class CourseDto
    {
        public int CourseId { get; set; }
        public string? CourseCode { get; set; }
        public string? CourseName { get; set; }
        public List<StudentDto> Students { get; set; }
    }
}
