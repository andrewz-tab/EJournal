using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EJournal.Migrations
{
    /// <inheritdoc />
    public partial class constainMark : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "Value",
                table: "Marks");

            migrationBuilder.AddCheckConstraint(
                name: "Value",
                table: "Marks",
                sql: "Value >= -1 AND Value <= 5");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "Value",
                table: "Marks");

            migrationBuilder.AddCheckConstraint(
                name: "Value",
                table: "Marks",
                sql: "Value >= 0 AND Value <= 5");
        }
    }
}
