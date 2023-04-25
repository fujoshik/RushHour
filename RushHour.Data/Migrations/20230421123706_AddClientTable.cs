using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RushHour.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddClientTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("c3a7daae-9d26-4173-b5ed-420887cc0b1d"));

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clients_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Email", "FullName", "Password", "Role" },
                values: new object[] { new Guid("1a5bb014-67a9-4d78-a51f-809407b5dd20"), "admin", "John Doe", "$2a$11$iSvMrpeBkYp17uxleoi3tOd3fjldVNolScjri3HeVAYBxc3ObH.zO", 0 });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_AccountId",
                table: "Clients",
                column: "AccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("1a5bb014-67a9-4d78-a51f-809407b5dd20"));

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Email", "FullName", "Password", "Role" },
                values: new object[] { new Guid("c3a7daae-9d26-4173-b5ed-420887cc0b1d"), "admin", "John Doe", "$2a$11$7bNXGiBVTmjLpuYADa8zQ.7FBT3MtrZg1bU0RoaVY3O3yVRu4nes2", 0 });
        }
    }
}
