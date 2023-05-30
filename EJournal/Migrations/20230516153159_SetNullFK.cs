using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EJournal.Migrations
{
    /// <inheritdoc />
    public partial class SetNullFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Employees_EmployeeKey",
                table: "Classes");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Employees_EmployeeKey",
                table: "Classes",
                column: "EmployeeKey",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Employees_EmployeeKey",
                table: "Classes");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Employees_EmployeeKey",
                table: "Classes",
                column: "EmployeeKey",
                principalTable: "Employees",
                principalColumn: "Id");
        }
    }
}
