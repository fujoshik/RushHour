using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RushHour.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHashedAdminPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("bcd8bb10-a44a-42c7-b1c9-eb9986c04640"));

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Email", "FullName", "Password", "Role" },
                values: new object[] { new Guid("1900dc82-709a-48a8-a3da-e6c29423dd4c"), "admin", "John Doe", "$2a$11$Wkz.zkPZk27/jX8f2eHyMeDWFuvb2mRn1lFP/TxxanS3AJKGCfM/S", 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("1900dc82-709a-48a8-a3da-e6c29423dd4c"));

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Email", "FullName", "Password", "Role" },
                values: new object[] { new Guid("bcd8bb10-a44a-42c7-b1c9-eb9986c04640"), "admin", "John Doe", "Password123+", 0 });
        }
    }
}
