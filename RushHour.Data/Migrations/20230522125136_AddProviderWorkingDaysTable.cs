using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RushHour.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProviderWorkingDaysTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("32e64fbb-cad2-445c-b623-f5af0aaa8b5c"));

            migrationBuilder.DropColumn(
                name: "WorkingDays",
                table: "Providers");

            migrationBuilder.CreateTable(
                name: "ProviderWorkingDays",
                columns: table => new
                {
                    ProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayOfTheWeek = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderWorkingDays", x => new { x.ProviderId, x.DayOfTheWeek });
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Email", "FullName", "Password", "Role" },
                values: new object[] { new Guid("5830df6d-ca4d-4f80-9f0b-cdb2b4a206f1"), "admin", "John Doe", "$2a$11$8/lJQu2hoJln8Ilp6/9hFeRLtNAPKonUDtYmwV6vXl.8UmiQZLsRi", 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProviderWorkingDays");

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("5830df6d-ca4d-4f80-9f0b-cdb2b4a206f1"));

            migrationBuilder.AddColumn<int>(
                name: "WorkingDays",
                table: "Providers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Email", "FullName", "Password", "Role" },
                values: new object[] { new Guid("32e64fbb-cad2-445c-b623-f5af0aaa8b5c"), "admin", "John Doe", "$2a$11$ZEAZpRJaX4MdIGPN70VXXuEb1Xl5dnTUqd9Ghhi0E2nQmxRkSPT2O", 0 });
        }
    }
}
