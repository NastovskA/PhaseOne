using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhaseOne.Migrations
{
    /// <inheritdoc />
    public partial class EnrollmentYearSemesterUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Semester",
                table: "Enrollments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_CourseId_StudentId_Year_Semester",
                table: "Enrollments",
                columns: new[] { "CourseId", "StudentId", "Year", "Semester" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Enrollments_CourseId_StudentId_Year_Semester",
                table: "Enrollments");

            migrationBuilder.AlterColumn<string>(
                name: "Semester",
                table: "Enrollments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);
        }
    }
}
