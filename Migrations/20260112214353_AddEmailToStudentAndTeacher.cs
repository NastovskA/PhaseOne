using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhaseOne.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailToStudentAndTeacher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Teachers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Email",
                value: "elena.kostova@feit.ukim.edu.mk");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 2L,
                column: "Email",
                value: "ivan.nikolov@feit.ukim.edu.mk");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 3L,
                column: "Email",
                value: "marija.stojanova@feit.ukim.edu.mk");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 4L,
                column: "Email",
                value: "petar.iliev@feit.ukim.edu.mk");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 5L,
                column: "Email",
                value: "simona.georgieva@feit.ukim.edu.mk");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 6L,
                column: "Email",
                value: "aleksandar.trajkovski@feit.ukim.edu.mk");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 7L,
                column: "Email",
                value: "sara.mitreva@feit.ukim.edu.mk");

            migrationBuilder.UpdateData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 1,
                column: "Email",
                value: "lila.petrova@feit.ukim.edu.mk");

            migrationBuilder.UpdateData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 2,
                column: "Email",
                value: "marko.stojanovski@feit.ukim.edu.mk");

            migrationBuilder.UpdateData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 3,
                column: "Email",
                value: "ivana.koleva@feit.ukim.edu.mk");

            migrationBuilder.UpdateData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 4,
                column: "Email",
                value: "stefan.jovanov@feit.ukim.edu.mk");

            migrationBuilder.UpdateData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 5,
                column: "Email",
                value: "biljana.ristova@feit.ukim.edu.mk");

            migrationBuilder.UpdateData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 6,
                column: "Email",
                value: "miki.todorov@feit.ukim.edu.mk");

            migrationBuilder.UpdateData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 7,
                column: "Email",
                value: "dragana.spasova@feit.ukim.edu.mk");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Students");
        }
    }
}
