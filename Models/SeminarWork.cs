    namespace PhaseOne.Models
    {
        public class SeminarWork
        {
            public int Id { get; set; }

            public long StudentId { get; set; }
            public int CourseId { get; set; }

            public string FilePath { get; set; }
            public DateTime UploadDate { get; set; }

            public Student Student { get; set; }
            public Course Course { get; set; }
        }
    }
