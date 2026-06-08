using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LecRate.Migrations
{
    
    public partial class ConvertToTextBasedRating : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Evaluations");

            migrationBuilder.AddColumn<int>(
                name: "AdminRating",
                table: "Lecturers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextAnswer",
                table: "Evaluations",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                defaultValue: "");
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminRating",
                table: "Lecturers");

            migrationBuilder.DropColumn(
                name: "TextAnswer",
                table: "Evaluations");

            migrationBuilder.AddColumn<byte>(
                name: "Rating",
                table: "Evaluations",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}