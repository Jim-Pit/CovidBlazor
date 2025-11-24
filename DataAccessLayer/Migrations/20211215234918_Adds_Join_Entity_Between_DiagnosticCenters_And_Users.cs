using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DbDesign.Migrations
{
    public partial class Adds_Join_Entity_Between_DiagnosticCenters_And_Users : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiagnosticCenterUsers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiagnosticCenterId = table.Column<int>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiagnosticCenterUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiagnosticCenterUsers_DiagnosticCenters_DiagnosticCenterId",
                        column: x => x.DiagnosticCenterId,
                        principalTable: "DiagnosticCenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiagnosticCenterUsers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiagnosticCenterUsers_UserId",
                table: "DiagnosticCenterUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DiagnosticCenterUsers_DiagnosticCenterId_UserId",
                table: "DiagnosticCenterUsers",
                columns: new[] { "DiagnosticCenterId", "UserId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiagnosticCenterUsers");
        }
    }
}
