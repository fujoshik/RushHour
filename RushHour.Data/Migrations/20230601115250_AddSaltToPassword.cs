using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RushHour.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSaltToPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("4c46af00-c008-4500-8a0c-9a380ff2c39b"));

            migrationBuilder.AddColumn<string>(
                name: "Salt",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Email", "FullName", "Password", "Role", "Salt" },
                values: new object[] { new Guid("44ae9f66-8fcf-4fbc-9bcd-d0216be48b29"), "admin", "John Doe", "9CEBB39C72E2E5BF0FCE6197CA8581AE8399FAAA705D666D248190E8D9B385418EA080D649586D20F95BBAE405773A3F7C1915C548C4DD6E8657DE26298D8EB1", 0, "7syZXC7fGEG5J10cSZAaoCNqPItdQHHL3sQCMBidv70K8DjZoLRzcot1IZNrZYnXGPWwFjKs85oISlOpAO5Zdg==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("44ae9f66-8fcf-4fbc-9bcd-d0216be48b29"));

            migrationBuilder.DropColumn(
                name: "Salt",
                table: "Accounts");

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Email", "FullName", "Password", "Role" },
                values: new object[] { new Guid("4c46af00-c008-4500-8a0c-9a380ff2c39b"), "admin", "John Doe", "$2a$11$qjZKCqyHWclxo03MZoZXr.W9pGsttyLwJbbTHBs3znLgA0vRokn/u", 0 });
        }
    }
}
