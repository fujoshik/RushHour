using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RushHour.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProviderIdInActivitiesNoAction : Migration
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
                keyValue: new Guid("e87e9c5e-1c9a-40e4-a590-a28f82d5ede1"));

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Email", "FullName", "Password", "Role" },
                values: new object[] { new Guid("0e25817b-196d-4545-809b-cd7e3cbf742e"), "admin", "John Doe", "$2a$11$8Rzv9U76k8UWz1fn40WxoeXHNdEHFuXvAHC8azoQXTsCCm39FGa66", 0 });

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
                keyValue: new Guid("0e25817b-196d-4545-809b-cd7e3cbf742e"));

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Email", "FullName", "Password", "Role" },
                values: new object[] { new Guid("e87e9c5e-1c9a-40e4-a590-a28f82d5ede1"), "admin", "John Doe", "$2a$11$qqCGaoCGnv5YHl9nAw7DMudsY6MPnBlBTRK6QFf4Fq12E4QAL1C9.", 0 });

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Providers_ProviderId",
                table: "Activities",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
