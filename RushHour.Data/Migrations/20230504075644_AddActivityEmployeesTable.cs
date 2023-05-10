using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RushHour.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddActivityEmployeesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Activities_ActivityId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_ActivityId",
                table: "Employees");

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("0791f676-2509-4042-9a9c-8a753060954c"));

            migrationBuilder.DropColumn(
                name: "ActivityId",
                table: "Employees");

            migrationBuilder.CreateTable(
                name: "ActivityEmployees",
                columns: table => new
                {
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityEmployees", x => new { x.ActivityId, x.EmployeeId });
                    table.ForeignKey(
                        name: "FK_ActivityEmployees_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityEmployees_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Email", "FullName", "Password", "Role" },
                values: new object[] { new Guid("cd873efe-d362-4276-bc1a-2a60e1cd8a5a"), "admin", "John Doe", "$2a$11$jNdq.f2aKXtM6pamIa1Hj.rpM5MUe/vTWfp9DkEJet39til2DwTba", 0 });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityEmployees_EmployeeId",
                table: "ActivityEmployees",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityEmployees");

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("cd873efe-d362-4276-bc1a-2a60e1cd8a5a"));

            migrationBuilder.AddColumn<Guid>(
                name: "ActivityId",
                table: "Employees",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Email", "FullName", "Password", "Role" },
                values: new object[] { new Guid("0791f676-2509-4042-9a9c-8a753060954c"), "admin", "John Doe", "$2a$11$Ws3z47R9MbAxJ4UhcwohVO/qudq5M0yzyGDbgy3sjEyGlsi9BwLOu", 0 });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ActivityId",
                table: "Employees",
                column: "ActivityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Activities_ActivityId",
                table: "Employees",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id");
        }
    }
}
