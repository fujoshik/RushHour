using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RushHour.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Providers_ProviderId",
                table: "Employees");

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("1fcb83f2-f5c8-4439-ae50-783c42ae60b5"));

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Email", "FullName", "Password", "Role" },
                values: new object[] { new Guid("c3a7daae-9d26-4173-b5ed-420887cc0b1d"), "admin", "John Doe", "$2a$11$7bNXGiBVTmjLpuYADa8zQ.7FBT3MtrZg1bU0RoaVY3O3yVRu4nes2", 0 });

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Providers_ProviderId",
                table: "Employees",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Providers_ProviderId",
                table: "Employees");

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("c3a7daae-9d26-4173-b5ed-420887cc0b1d"));

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Email", "FullName", "Password", "Role" },
                values: new object[] { new Guid("1fcb83f2-f5c8-4439-ae50-783c42ae60b5"), "admin", "John Doe", "$2a$11$LIZFE13ep2IUU7ESwhc9WuxiZ7iLSp4T4j8nSXmeOk9zmCBz9W5/q", 0 });

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Providers_ProviderId",
                table: "Employees",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
