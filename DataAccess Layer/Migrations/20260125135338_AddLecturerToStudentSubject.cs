using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LecRate.Migrations
{
    
    public partial class AddLecturerToStudentSubject : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LecturerId",
                table: "StudentSubjects",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentSubjects_LecturerId",
                table: "StudentSubjects",
                column: "LecturerId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSubjects_Lecturers_LecturerId",
                table: "StudentSubjects",
                column: "LecturerId",
                principalTable: "Lecturers",
                principalColumn: "LecturerId",
                onDelete: ReferentialAction.Restrict);
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentSubjects_Lecturers_LecturerId",
                table: "StudentSubjects");

            migrationBuilder.DropIndex(
                name: "IX_StudentSubjects_LecturerId",
                table: "StudentSubjects");

            migrationBuilder.DropColumn(
                name: "LecturerId",
                table: "StudentSubjects");
        }
    }
}