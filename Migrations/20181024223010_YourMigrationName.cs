using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CodingDojoActivities.Migrations
{
    public partial class YourMigrationName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 45, nullable: false),
                    LastName = table.Column<string>(maxLength: 45, nullable: false),
                    Password = table.Column<string>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "cdActivities",
                columns: table => new
                {
                    cdActivityId = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Date = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Duration = table.Column<int>(nullable: false),
                    DurationTimespan = table.Column<TimeSpan>(nullable: false),
                    DurationType = table.Column<string>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cdActivities", x => x.cdActivityId);
                    table.ForeignKey(
                        name: "FK_cdActivities_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Participations",
                columns: table => new
                {
                    ParticipationId = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    UserId = table.Column<int>(nullable: false),
                    cdActivityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participations", x => x.ParticipationId);
                    table.ForeignKey(
                        name: "FK_Participations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Participations_cdActivities_cdActivityId",
                        column: x => x.cdActivityId,
                        principalTable: "cdActivities",
                        principalColumn: "cdActivityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cdActivities_UserId",
                table: "cdActivities",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Participations_UserId",
                table: "Participations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Participations_cdActivityId",
                table: "Participations",
                column: "cdActivityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Participations");

            migrationBuilder.DropTable(
                name: "cdActivities");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
