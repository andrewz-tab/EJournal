using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EJournal.Migrations
{
    /// <inheritdoc />
    public partial class addAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "EMail", "Login", "Password", "PhoneNumber", "TypeUserKey", "isActivate", "isChanged", "isRequiredChangePassword" },
                values: new object[] { 101, "qwerty@mail.com", "admin", "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918", null, 2, true, false, false });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "AccountKey", "Description" },
                values: new object[] { 101, 101, null });

            migrationBuilder.InsertData(
                table: "PersonalDatas",
                columns: new[] { "Id", "AccountKey", "DateBirth", "FullName", "PassId", "SNILS", "gender" },
                values: new object[] { 101, 101, new DateTime(2002, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Хузахметов Андрей Александрович", null, "12345678901", 0 });

            migrationBuilder.InsertData(
                table: "EmployeeRole",
                columns: new[] { "EmployeesId", "RolesId" },
                values: new object[] { 101, 3 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EmployeeRole",
                keyColumns: new[] { "EmployeesId", "RolesId" },
                keyValues: new object[] { 101, 3 });

            migrationBuilder.DeleteData(
                table: "PersonalDatas",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 101);
        }
    }
}
