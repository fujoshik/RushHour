using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RushHour.Data.Migrations
{
    /// <inheritdoc />
    public partial class ActivityNoActionWhenDeleteProvider : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Providers_ProviderId",
                table: "Activities");

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("d456fbb7-1a97-4546-990f-4a7c7cb87e78"));

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Email", "FullName", "Password", "Role" },
                values: new object[] { new Guid("5fc62a12-88d0-4fe1-8563-10ff9c47eb02"), "admin", "John Doe", "$2a$11$S0IAr22ulbz2h84rQ/KrTuIyHqkVzpKh6LlZSNly4lOWUi.Wx13Pi", 0 });

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Providers_ProviderId",
                table: "Activities",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Providers_ProviderId",
                table: "Activities");

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("5fc62a12-88d0-4fe1-8563-10ff9c47eb02"));

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Email", "FullName", "Password", "Role" },
                values: new object[] { new Guid("d456fbb7-1a97-4546-990f-4a7c7cb87e78"), "admin", "John Doe", "$2a$11$XGuP92CcIfxDNebkxGHV5uZO21OQulNTsUaWjbgiCX2mcAf8gHhNS", 0 });

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Providers_ProviderId",
                table: "Activities",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
