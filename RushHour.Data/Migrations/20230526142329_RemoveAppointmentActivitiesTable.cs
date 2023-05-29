using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RushHour.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAppointmentActivitiesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentActivities");

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("6b58f771-0e35-4b8b-b51d-2d007abb2689"));

            migrationBuilder.AddColumn<Guid>(
                name: "ActivityId",
                table: "Appointments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Email", "FullName", "Password", "Role" },
                values: new object[] { new Guid("4c46af00-c008-4500-8a0c-9a380ff2c39b"), "admin", "John Doe", "$2a$11$qjZKCqyHWclxo03MZoZXr.W9pGsttyLwJbbTHBs3znLgA0vRokn/u", 0 });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ActivityId",
                table: "Appointments",
                column: "ActivityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Activities_ActivityId",
                table: "Appointments",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Activities_ActivityId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_ActivityId",
                table: "Appointments");

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("4c46af00-c008-4500-8a0c-9a380ff2c39b"));

            migrationBuilder.DropColumn(
                name: "ActivityId",
                table: "Appointments");

            migrationBuilder.CreateTable(
                name: "AppointmentActivities",
                columns: table => new
                {
                    AppointmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentActivities", x => new { x.AppointmentId, x.ActivityId });
                    table.ForeignKey(
                        name: "FK_AppointmentActivities_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppointmentActivities_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Email", "FullName", "Password", "Role" },
                values: new object[] { new Guid("6b58f771-0e35-4b8b-b51d-2d007abb2689"), "admin", "John Doe", "$2a$11$Pw/6VlUqHwW1K8c70fzQIuZpnz5Mno7Nz2E0KhXQtqg9yE8ywdXay", 0 });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentActivities_ActivityId",
                table: "AppointmentActivities",
                column: "ActivityId");
        }
    }
}
