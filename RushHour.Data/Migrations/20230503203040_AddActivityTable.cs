using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RushHour.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddActivityTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("1a5bb014-67a9-4d78-a51f-809407b5dd20"));

            migrationBuilder.AddColumn<Guid>(
                name: "ActivityId",
                table: "Employees",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    ProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activities_Providers_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Providers",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Email", "FullName", "Password", "Role" },
                values: new object[] { new Guid("0791f676-2509-4042-9a9c-8a753060954c"), "admin", "John Doe", "$2a$11$Ws3z47R9MbAxJ4UhcwohVO/qudq5M0yzyGDbgy3sjEyGlsi9BwLOu", 0 });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ActivityId",
                table: "Employees",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_ProviderId",
                table: "Activities",
                column: "ProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Activities_ActivityId",
                table: "Employees",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Activities_ActivityId",
                table: "Employees");

            migrationBuilder.DropTable(
                name: "Activities");

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

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Email", "FullName", "Password", "Role" },
                values: new object[] { new Guid("1a5bb014-67a9-4d78-a51f-809407b5dd20"), "admin", "John Doe", "$2a$11$iSvMrpeBkYp17uxleoi3tOd3fjldVNolScjri3HeVAYBxc3ObH.zO", 0 });
        }
    }
}
