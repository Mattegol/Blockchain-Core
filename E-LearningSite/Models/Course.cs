using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace E_LearningSite.Models
{
    public class Course
    {
        public int Id { get; set; }

        public Guid CourseId { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [StringLength(50)]
        public string WistiaId { get; set; }

        public long Price { get; set; }

        public string ImagePath { get; set; }

        public long Revenue { get; set; }

        [Required]
        public string TeacherId { get; set; }

        public virtual ApplicationUser Teacher { get; set; }

        public ICollection<CourseStudent> CourseStudents { get; set; } = new HashSet<CourseStudent>();
    }
}
