using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RushHour.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointmentActivitesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                keyValue: new Guid("5fc62a12-88d0-4fe1-8563-10ff9c47eb02"));

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
                values: new object[] { new Guid("3703acad-2e09-4904-a650-84eb84f0c20a"), "admin", "John Doe", "$2a$11$Ukiosf4.LqeCQRthTf.DNeeFzvSN9GPpPX2fK.Z1jvRmngISymVkK", 0 });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentActivities_ActivityId",
                table: "AppointmentActivities",
                column: "ActivityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentActivities");

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("3703acad-2e09-4904-a650-84eb84f0c20a"));

            migrationBuilder.AddColumn<Guid>(
                name: "ActivityId",
                table: "Appointments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Email", "FullName", "Password", "Role" },
                values: new object[] { new Guid("5fc62a12-88d0-4fe1-8563-10ff9c47eb02"), "admin", "John Doe", "$2a$11$S0IAr22ulbz2h84rQ/KrTuIyHqkVzpKh6LlZSNly4lOWUi.Wx13Pi", 0 });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ActivityId",
                table: "Appointments",
                column: "ActivityId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Activities_ActivityId",
                table: "Appointments",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id");
        }
    }
}
