using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace E_LearningSite.Models
{
    public class CourseStudent
    {
        public int Id { get; set; }

        [Required]
        public Guid CourseId { get; set; }

        public Course Course { get; set; }

        public Guid CourseStudentId { get; set; }

        public string StudentId { get; set; }

        public virtual ApplicationUser Student { get; set; }
    }
}
