using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProfRate.Migrations
{
    
    public partial class AddIsArchivedToEvaluation : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Evaluations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Evaluations");
        }
    }
}