using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RushHour.Data.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDeleteProviderIdInEmployees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("bbd557c1-4086-4078-9bfe-1d6ac54c52cf"));

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Email", "FullName", "Password", "Role" },
                values: new object[] { new Guid("e87e9c5e-1c9a-40e4-a590-a28f82d5ede1"), "admin", "John Doe", "$2a$11$qqCGaoCGnv5YHl9nAw7DMudsY6MPnBlBTRK6QFf4Fq12E4QAL1C9.", 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("e87e9c5e-1c9a-40e4-a590-a28f82d5ede1"));

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Email", "FullName", "Password", "Role" },
                values: new object[] { new Guid("bbd557c1-4086-4078-9bfe-1d6ac54c52cf"), "admin", "John Doe", "$2a$11$KolM24PBwxi25CGycPCMO.PbY7dNbe1eoKSvxB.4zhKJ8xbpbZ46i", 0 });
        }
    }
}
