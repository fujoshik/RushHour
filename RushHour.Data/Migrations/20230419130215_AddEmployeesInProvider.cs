using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RushHour.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeesInProvider : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("b9fc28b0-bb1f-4dd4-9973-5dfc662025a2"));

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Email", "FullName", "Password", "Role" },
                values: new object[] { new Guid("1fcb83f2-f5c8-4439-ae50-783c42ae60b5"), "admin", "John Doe", "$2a$11$LIZFE13ep2IUU7ESwhc9WuxiZ7iLSp4T4j8nSXmeOk9zmCBz9W5/q", 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("1fcb83f2-f5c8-4439-ae50-783c42ae60b5"));

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Email", "FullName", "Password", "Role" },
                values: new object[] { new Guid("b9fc28b0-bb1f-4dd4-9973-5dfc662025a2"), "admin", "John Doe", "$2a$11$QnkO7fypuYRChbZzhESy..jqU72TJFBYWjj7MEkuHXqhfcLSDMGQG", 0 });
        }
    }
}
