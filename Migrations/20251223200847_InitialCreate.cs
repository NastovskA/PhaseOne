using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PhaseOne.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EnrollmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AcquiredCredits = table.Column<int>(type: "int", nullable: true),
                    CurrentSemestar = table.Column<int>(type: "int", nullable: true),
                    EducationLevel = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Degree = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AcademicRank = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    OfficeNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    HireDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Credits = table.Column<int>(type: "int", nullable: false),
                    Semester = table.Column<int>(type: "int", nullable: false),
                    Programme = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EducationLevel = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    FirstTeacherId = table.Column<int>(type: "int", nullable: true),
                    SecondTeacherId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_Teachers_FirstTeacherId",
                        column: x => x.FirstTeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Courses_Teachers_SecondTeacherId",
                        column: x => x.SecondTeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Enrollments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<long>(type: "bigint", nullable: false),
                    Semester = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Grade = table.Column<int>(type: "int", nullable: true),
                    SeminalUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProjectUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ExamPoints = table.Column<int>(type: "int", nullable: true),
                    SeminalPoints = table.Column<int>(type: "int", nullable: true),
                    ProjectPoints = table.Column<int>(type: "int", nullable: true),
                    AdditionalPoints = table.Column<int>(type: "int", nullable: true),
                    FinishDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Enrollments_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Enrollments_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "AcquiredCredits", "CurrentSemestar", "EducationLevel", "EnrollmentDate", "FirstName", "LastName", "StudentId" },
                values: new object[,]
                {
                    { 1L, 60, 3, "Bachelor", new DateTime(2022, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Elena", "Kostova", "2025001" },
                    { 2L, 120, 5, "Bachelor", new DateTime(2021, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ivan", "Nikolov", "2025002" },
                    { 3L, 30, 2, "Bachelor", new DateTime(2023, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Marija", "Stojanova", "2025003" },
                    { 4L, 180, 7, "Bachelor", new DateTime(2020, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Petar", "Iliev", "2025004" },
                    { 5L, 90, 4, "Bachelor", new DateTime(2022, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Simona", "Georgieva", "2025005" },
                    { 6L, 150, 6, "Bachelor", new DateTime(2021, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Aleksandar", "Trajkovski", "2025006" },
                    { 7L, 15, 1, "Bachelor", new DateTime(2023, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sara", "Mitreva", "2025007" }
                });

            migrationBuilder.InsertData(
                table: "Teachers",
                columns: new[] { "Id", "AcademicRank", "Degree", "FirstName", "HireDate", "LastName", "OfficeNumber" },
                values: new object[,]
                {
                    { 1, "Professor", "PhD", "Lila", new DateTime(2015, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Petrova", "101" },
                    { 2, "Assistant", "MSc", "Marko", new DateTime(2018, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Stojanovski", "102" },
                    { 3, "Associate Professor", "PhD", "Ivana", new DateTime(2016, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Koleva", "103" },
                    { 4, "Assistant", "MSc", "Stefan", new DateTime(2019, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jovanov", "104" },
                    { 5, "Professor", "PhD", "Biljana", new DateTime(2014, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ristova", "105" },
                    { 6, "Assistant", "MSc", "Miki", new DateTime(2020, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Todorov", "106" },
                    { 7, "Associate Professor", "PhD", "Dragana", new DateTime(2017, 7, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Spasova", "107" }
                });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "Credits", "EducationLevel", "FirstTeacherId", "Programme", "SecondTeacherId", "Semester", "Title" },
                values: new object[,]
                {
                    { 1, 6, "Bachelor", 1, "Computer Science", 2, 1, "Programming 1" },
                    { 2, 6, "Bachelor", 2, "Software Engineering", 3, 3, "Databases" },
                    { 3, 6, "Bachelor", 3, "Computer Science", 4, 5, "Algorithms" },
                    { 4, 6, "Bachelor", 4, "Software Engineering", 5, 4, "Web Development" },
                    { 5, 6, "Bachelor", 5, "Computer Science", 6, 6, "Operating Systems" },
                    { 6, 6, "Bachelor", 6, "Computer Science", 7, 7, "Networks" },
                    { 7, 6, "Bachelor", 7, "Software Engineering", 1, 8, "Software Engineering" }
                });

            migrationBuilder.InsertData(
                table: "Enrollments",
                columns: new[] { "Id", "AdditionalPoints", "CourseId", "ExamPoints", "FinishDate", "Grade", "ProjectPoints", "ProjectUrl", "Semester", "SeminalPoints", "SeminalUrl", "StudentId", "Year" },
                values: new object[,]
                {
                    { 1L, 5, 1, 50, new DateTime(2023, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 10, 20, "http://example.com/project1", "winter", 20, "http://example.com/seminal1", 1L, 2022 },
                    { 2L, 0, 2, 45, new DateTime(2024, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 9, 25, "http://example.com/project2", "winter", 15, "http://example.com/seminal2", 2L, 2023 },
                    { 3L, 0, 3, 40, new DateTime(2024, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 20, "http://example.com/project3", "summer", 20, "http://example.com/seminal3", 3L, 2023 },
                    { 4L, 5, 4, 35, new DateTime(2022, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 15, "http://example.com/project4", "summer", 15, "http://example.com/seminal4", 4L, 2021 },
                    { 5L, 10, 5, 50, new DateTime(2023, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 10, 20, "http://example.com/project5", "winter", 20, "http://example.com/seminal5", 5L, 2022 },
                    { 6L, 5, 6, 45, new DateTime(2024, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 9, 25, "http://example.com/project6", "summer", 15, "http://example.com/seminal6", 6L, 2023 },
                    { 7L, 0, 7, 40, new DateTime(2025, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 20, "http://example.com/project7", "winter", 20, "http://example.com/seminal7", 7L, 2024 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_FirstTeacherId",
                table: "Courses",
                column: "FirstTeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_SecondTeacherId",
                table: "Courses",
                column: "SecondTeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_CourseId_StudentId",
                table: "Enrollments",
                columns: new[] { "CourseId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentId",
                table: "Enrollments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_StudentId",
                table: "Students",
                column: "StudentId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Enrollments");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Teachers");
        }
    }
}
